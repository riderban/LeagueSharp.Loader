using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.Core
{
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
            if (IsInjected || !Interop.GetWindowText(MainWindowHandle).Contains("League of Legends (TM) Client"))
            {
                return false;
            }

            if (Injector.InjectDLL == null)
            {
                Injector.Bootstrap();
            }

            if (Injector.InjectDLL != null)
            {
                Injector.InjectDLL(Id, Directories.CoreFilePath);
                Injector.RaiseOnInject(this);
                return true;
            }

            return false;
        }

        public void LoadAssembly(LeagueSharpAssembly assembly)
        {
            if (!IsInjected)
            {
                return;
            }

            lock (Assemblies)
            {
                if (!Assemblies.Contains(assembly))
                {
                    return;
                }

                Assemblies.Add(assembly);
            }

            Injector.LoadAssembly(MainWindowHandle, assembly);
        }

        public void UnloadAssembly(LeagueSharpAssembly assembly)
        {
            if (!IsInjected)
            {
                return;
            }

            lock (Assemblies)
            {
                if (Assemblies.Contains(assembly))
                {
                    return;
                }

                Assemblies.Remove(assembly);
            }

            Injector.UnloadAssembly(MainWindowHandle, assembly);
        }

        public void UnloadAll()
        {
            if (!IsInjected)
            {
                return;
            }

            lock (Assemblies)
            {
                Assemblies.Clear();
            }

            Injector.UnloadAll(MainWindowHandle);
        }

        public void Login(string user, string password)
        {
            if (!IsInjected)
            {
                return;
            }

            if (!IsLoggedIn)
            {
                Injector.SendLoginCredentials(MainWindowHandle, user, password);
                IsLoggedIn = true;
            }
        }

        public void SendConfig()
        {
            if (!IsInjected)
            {
                return;
            }

            Injector.SendConfig(MainWindowHandle);
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Injector
    {
        public delegate void OnInjectDelegate(LeagueInstance instance);

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static InjectDLLDelegate InjectDLL;
        public static List<LeagueInstance> ActiveInstances = new List<LeagueInstance>();

        static Injector()
        {
            OnInject += instance => Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        instance.Login(Config.Instance.Username, Config.Instance.Password);
                        instance.SendConfig();

                        foreach (
                            var assembly in Config.Instance.SelectedProfile.InstalledAssemblies.Where(a => a.Inject))
                        {
                            instance.LoadAssembly(assembly);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e);
                    }
                });
        }

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

        public static void Bootstrap()
        {
            try
            {
                const string procedureName = "_InjectDLL@8";
                var hModule = Interop.LoadLibrary(Directories.BootstrapFilePath);

                if (hModule.IsZero())
                {
                    Log.WarnFormat("LoadLibrary failed {0}", Directories.BootstrapFilePath);
                    return;
                }

                var proc = Interop.GetProcAddress(hModule, procedureName);

                if (proc.IsZero())
                {
                    Log.WarnFormat("Procedure [{0}] not found at 0x{1}", procedureName, hModule.ToString("X"));
                    return;
                }

                InjectDLL = Marshal.GetDelegateForFunctionPointer(proc, typeof (InjectDLLDelegate)) as InjectDLLDelegate;
                Log.InfoFormat("{0} injected at 0x{1}", Path.GetFileName(Directories.BootstrapFilePath),
                    hModule.ToString("X"));
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void LoadAssembly(IntPtr hWnd, LeagueSharpAssembly assembly)
        {
            try
            {
                if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                    assembly.State == AssemblyState.Ready)
                {
                    var str = string.Format("load \"{0}\"", assembly.PathToBinary);
                    Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
                    Log.Info(str);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void UnloadAll(IntPtr hWnd)
        {
            try
            {
                const string str = "unload \"all\"";
                Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
                Log.Info(str);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void UnloadAssembly(IntPtr hWnd, LeagueSharpAssembly assembly)
        {
            try
            {
                if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                    assembly.State == AssemblyState.Ready)
                {
                    var str = string.Format("unload \"{0}\"", Path.GetFileName(assembly.PathToBinary));
                    Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
                    Log.Info(str);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void SendLoginCredentials(IntPtr hWnd, string user, string password)
        {
            try
            {
                var str = string.Format("LOGIN|{0}|{1}", user, password);
                Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.Core, str);
                Log.InfoFormat("LOGIN|{0}", user);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void SendConfig(IntPtr hWnd)
        {
            try
            {
                var afk = (Config.Instance.Settings.GameSettings[0].Value == "True") ? "1" : "0";
                var zoom = (Config.Instance.Settings.GameSettings[3].Value == "True") ? "1" : "0";
                var console = (Config.Instance.Settings.GameSettings[1].Value == "True") ? "1" : "0";
                var tower = (Config.Instance.Settings.GameSettings[2].Value == "True") ? "2" : "0";

                var str = string.Format("{0}{1}{2}{3}", afk, zoom, console, tower);
                Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.Core, str);
                Log.Info(str);
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