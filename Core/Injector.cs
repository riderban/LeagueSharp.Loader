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

            Injector.LoadAssembly(MainWindowHandle, assembly);
        }

        public void UnloadAll()
        {
            if (!IsInjected)
            {
                return;
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
            Injector.SendConfig(MainWindowHandle);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process = null;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Injector
    {
        public delegate void OnInjectDelegate(LeagueInstance instance);

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static InjectDLLDelegate InjectDLL;
        public static List<LeagueInstance> ActiveInstances = new List<LeagueInstance>();

        private static List<Process> LeagueProcessList
        {
            get { return Process.GetProcessesByName("League of Legends").ToList(); }
        }

        static Injector()
        {
            OnInject += instance => Task.Factory.StartNew(
                () =>
                {
                    instance.Login(Config.Instance.Username, Config.Instance.Password);
                    instance.SendConfig();

                    foreach (var assembly in Config.Instance.SelectedProfile.InstalledAssemblies.Where(a => a.Inject))
                    {
                        instance.LoadAssembly(assembly);
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

            while (true)
            {
                if (Config.Instance.Install)
                {
                }

                Thread.Sleep(3000);
            }
        }

        public static void Bootstrap()
        {
            var hModule = Interop.LoadLibrary(Directories.BootstrapFilePath);

            if (!(hModule != IntPtr.Zero))
            {
                return;
            }

            var procAddress = Interop.GetProcAddress(hModule, "_InjectDLL@8");

            if (!(procAddress != IntPtr.Zero))
            {
                return;
            }

            InjectDLL =
                Marshal.GetDelegateForFunctionPointer(procAddress, typeof (InjectDLLDelegate)) as InjectDLLDelegate;
            Log.InfoFormat("{0} injected at {1}", Path.GetFileName(Directories.BootstrapFilePath), hModule);
        }

        public static void LoadAssembly(IntPtr hWnd, LeagueSharpAssembly assembly)
        {
            if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                assembly.State == AssemblyState.Ready)
            {
                var str = string.Format("load \"{0}\"", assembly.PathToBinary);
                Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
                Log.InfoFormat("load \"{0}\"", assembly.PathToBinary);
            }
        }

        public static void UnloadAll(IntPtr hWnd)
        {
            const string str = "unload \"all\"";
            Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
            Log.Info(str);
        }

        public static void UnloadAssembly(IntPtr hWnd, LeagueSharpAssembly assembly)
        {
            if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                assembly.State == AssemblyState.Ready)
            {
                var str = string.Format("unload \"{0}\"", Path.GetFileName(assembly.PathToBinary));
                Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.AppDomainManager, str);
                Log.InfoFormat("unload \"{0}\"", Path.GetFileName(assembly.PathToBinary));
            }
        }

        public static void SendLoginCredentials(IntPtr hWnd, string user, string password)
        {
            var str = string.Format("LOGIN|{0}|{1}", user, password);
            Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.Core, str);
            Log.InfoFormat("LOGIN|{0}", user);
        }

        public static void SendConfig(IntPtr hWnd)
        {
            var str = string.Format(
                "{0}{1}{2}{3}", (Config.Instance.Settings.GameSettings[0].Value == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[3].Value == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[1].Value == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[2].Value == "True") ? "2" : "0");

            Interop.SendWindowMessage(hWnd, Interop.WindowMessageTarget.Core, str);
            Log.InfoFormat("CONFIG|{0}", str);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate bool InjectDLLDelegate(int processId, string path);
    }
}