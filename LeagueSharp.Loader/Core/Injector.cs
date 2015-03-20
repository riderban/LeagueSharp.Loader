using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LeagueSharp.Loader.Core.Interop;
using LeagueSharp.Loader.Core.Service;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using LeagueSharp.Loader.Service;

namespace LeagueSharp.Loader.Core
{
    /// <summary>
    ///     L# Bootstrap
    ///     Loader       -> create ILoaderService host
    ///     Loader       -> create FileMapping Local\\LeagueSharpBootstrap with SandboxFilePath
    ///     Loader       -> LoadLibrary Bootstrapper
    ///     Loader       -> call InjectModule(DWORD procId, LPCWSTR modulePath)
    ///     Bootstrapper -> create CLR/Inject Core into process
    ///     Bootstrapper -> create Sandbox
    ///     Sandbox      -> connect to Loader.Service Impl
    ///     Sandbox      -> request LoginCredentials
    ///     Sandbox      -> request Configuration
    ///     Sandbox      -> request AssemblyPathList
    ///     Sandbox      -> create AppDomain
    ///     Sandbox      -> bootstrap Common
    /// </summary>
    internal class LeagueInstance
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly List<LeagueSharpAssembly> Assemblies = new List<LeagueSharpAssembly>();

        public int Id
        {
            get { return Process.Id; }
        }

        public bool IsInjected
        {
            get
            {
                if (Process == null || Process.HasExited)
                {
                    return false;
                }

                try
                {
                    return Process.Modules.Cast<ProcessModule>()
                        .Any(m => m.ModuleName == Path.GetFileName(Directories.CoreFilePath));
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                    return false;
                }
            }
        }

        public bool IsLoggedIn { get; set; }
        public IntPtr MainWindowHandle { get; set; }
        public Process Process { get; set; }

        public LeagueInstance(Process process)
        {
            Process = process;
            Process.Exited += Process_Exited;
            Injector.ActiveInstances.Add(this);
            Log.InfoFormat("League Instance #{0} created", Id);
        }

        public bool Inject()
        {
            if (IsInjected || !Interop.Interop.GetWindowText(MainWindowHandle).Contains("League of Legends (TM) Client"))
            {
                return false;
            }

            if (Injector.InjectDll == null)
            {
                Injector.Bootstrap();
            }

            if (Injector.InjectDll != null)
            {
                Injector.InjectDll(Id, Directories.CoreFilePath);
                Injector.RaiseOnInject(this);
                return true;
            }

            return false;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process = null;

            lock (Injector.ActiveInstances)
            {
                if (Injector.ActiveInstances.Contains(this))
                {
                    Injector.ActiveInstances.Remove(this);
                }
            }
        }
    }

    internal class Injector
    {
        public delegate void OnInjectDelegate(LeagueInstance instance);

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static InjectDLLDelegate InjectDll;
        public static List<LeagueInstance> ActiveInstances = new List<LeagueInstance>();
        private static ServiceHost _serviceHost;
        public static event OnInjectDelegate OnInject;

        public static void RaiseOnInject(LeagueInstance instance)
        {
            if (OnInject != null)
            {
                OnInject(instance);
            }
        }

        public static void PulseTask()
        {
            Thread.CurrentThread.IsBackground = true;

            Log.Info("Injection Task started");

            while (true)
            {
                try
                {
                    if (Config.Instance.Install)
                    {
                        var processList = Process.GetProcessesByName("League of Legends");

                        lock (ActiveInstances)
                        {
                            foreach (var process in processList.Where(p => ActiveInstances.All(i => i.Id != p.Id)))
                            {
                                var instance = new LeagueInstance(process);

                                if (instance.Inject())
                                {
                                    Log.InfoFormat("{0} injected into {1}", Path.GetFileName(Directories.CoreFilePath),
                                        instance.Id);
                                }
                                else
                                {
                                    Log.WarnFormat("Injection failed {0} - Injection State:{1}", instance.Id,
                                        instance.IsInjected);
                                }

                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                }

                Thread.Sleep(3000);
            }
        }

        private static void CreateServiceHost()
        {
            if (_serviceHost == null)
            {
                _serviceHost = ServiceFactory.ShareInterface<LoaderService>();
                _serviceHost.Faulted += (sender, args) =>
                {
                    try
                    {
                        Log.Info("Service Host Error, try restart");
                        _serviceHost.Close(); // TODO: check if necessary
                        _serviceHost = null;
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e);
                    }

                    Task.Delay(500).ContinueWith(task => { CreateServiceHost(); });
                };

                Log.InfoFormat("Created Service Endpoint {0}{1}", _serviceHost.BaseAddresses, typeof (LoaderService));
            }
        }

        public static void Bootstrap()
        {
            try
            {
                CreateServiceHost();

                var mmf = MemoryMappedFile.CreateNew("Local\\LeagueSharpBootstrap", 260*2,
                    MemoryMappedFileAccess.Write);

                using (var writer = mmf.CreateViewAccessor())
                {
                    var path = Directories.SandboxFilePath.ToCharArray();
                    writer.WriteArray(0, path, 0, path.Length);
                }

                const string procedureName = "_InjectDLL@8";
                var hModule = Interop.Interop.LoadLibrary(Directories.SandboxFilePath); 

                if (hModule.IsZero())
                {
                    Log.WarnFormat("LoadLibrary failed {0}", Directories.SandboxFilePath);
                    return;
                }

                var proc = Interop.Interop.GetProcAddress(hModule, procedureName);

                if (proc.IsZero())
                {
                    Log.WarnFormat("Procedure [{0}] not found at 0x{1}", procedureName, hModule.ToString("X"));
                    return;
                }

                InjectDll = Marshal.GetDelegateForFunctionPointer(proc, typeof (InjectDLLDelegate)) as InjectDLLDelegate;
                Log.InfoFormat("{0} injected at 0x{1}", Path.GetFileName(Directories.SandboxFilePath),
                    hModule.ToString("X"));
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate bool InjectDLLDelegate(int processId, string path);
    }
}