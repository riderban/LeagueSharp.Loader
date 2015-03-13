using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.Core
{
    internal class Injector
    {
        public delegate void OnInjectDelegate(IntPtr hwnd);

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static InjectDLLDelegate injectDLL;

        public static bool IsInjected
        {
            get { return LeagueProcess.Any(IsProcessInjected); }
        }

        public static List<IntPtr> LeagueInstances
        {
            get { return FindWindows("League of Legends (TM) Client"); }
        }

        private static List<Process> LeagueProcess
        {
            get { return Process.GetProcessesByName("League of Legends").ToList(); }
        }

        public static event OnInjectDelegate OnInject;

        private static bool IsProcessInjected(Process leagueProcess)
        {
            if (leagueProcess != null)
            {
                try
                {
                    return
                        leagueProcess.Modules.Cast<ProcessModule>()
                            .Any(
                                processModule => processModule.ModuleName == Path.GetFileName(Directories.CoreFilePath));
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                }
            }
            return false;
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            var size = Interop.GetWindowTextLength(hWnd);
            if (size++ > 0)
            {
                var builder = new StringBuilder(size);
                Interop.GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }
            return String.Empty;
        }

        private static List<IntPtr> FindWindows(string title)
        {
            var windows = new List<IntPtr>();
            Interop.EnumWindows(delegate(IntPtr wnd, IntPtr param)
            {
                if (GetWindowText(wnd).Contains(title))
                {
                    windows.Add(wnd);
                }
                return true;
            }, IntPtr.Zero);
            return windows;
        }

        private static void ResolveInjectDll()
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
            injectDLL =
                Marshal.GetDelegateForFunctionPointer(procAddress, typeof (InjectDLLDelegate)) as InjectDLLDelegate;
            Log.Info("Core injected " + hModule);
        }

        public static void Pulse()
        {
            if (LeagueProcess == null)
            {
                return;
            }

            //Don't inject untill we checked that there are not updates for the loader.
            if (AppUpdater.Updating || !AppUpdater.CheckedForUpdates)
            {
                return;
            }

            foreach (var instance in LeagueProcess)
            {
                try
                {
                    Config.Instance.LeagueOfLegendsExePath = instance.Modules[0].FileName;
                    if (!IsProcessInjected(instance) && AppUpdater.UpdateCore(instance.Modules[0].FileName, true).Item1)
                    {
                        if (injectDLL == null)
                        {
                            ResolveInjectDll();
                        }
                        if (injectDLL != null &&
                            GetWindowText(instance.MainWindowHandle).Contains("League of Legends (TM) Client"))
                        {
                            injectDLL(instance.Id, Directories.CoreFilePath);
                            if (OnInject != null)
                            {
                                OnInject(instance.MainWindowHandle);
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static void LoadAssembly(IntPtr wnd, LeagueSharpAssembly assembly)
        {
            if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                assembly.State == AssemblyState.Ready)
            {
                var str = string.Format("load \"{0}\"", assembly.PathToBinary);
                Interop.SendWindowMessage(wnd, Interop.WindowMessageTarget.AppDomainManager, str);
                Log.InfoFormat("load \"{0}\"", assembly.PathToBinary);
            }
        }

        public static void UnloadAssembly(IntPtr wnd, LeagueSharpAssembly assembly)
        {
            if (assembly.Type != AssemblyType.Unknown && assembly.Type != AssemblyType.Library &&
                assembly.State == AssemblyState.Ready)
            {
                var str = string.Format("unload \"{0}\"", Path.GetFileName(assembly.PathToBinary));
                Interop.SendWindowMessage(wnd, Interop.WindowMessageTarget.AppDomainManager, str);
                Log.InfoFormat("unload \"{0}\"", Path.GetFileName(assembly.PathToBinary));
            }
        }

        public static void SendLoginCredentials(IntPtr wnd, string user, string passwordHash)
        {
            var str = string.Format("LOGIN|{0}|{1}", user, passwordHash);
            Interop.SendWindowMessage(wnd, Interop.WindowMessageTarget.Core, str);
            Log.InfoFormat("LOGIN|{0}", user);
        }

        public static void SendConfig(IntPtr wnd)
        {
            var str = string.Format(
                "{0}{1}{2}{3}", (Config.Instance.Settings.GameSettings[0].SelectedValue == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[3].SelectedValue == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[1].SelectedValue == "True") ? "1" : "0",
                (Config.Instance.Settings.GameSettings[2].SelectedValue == "True") ? "2" : "0");

            Interop.SendWindowMessage(wnd, Interop.WindowMessageTarget.Core, str);
            Log.InfoFormat("CONFIG|{0}", str);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate bool InjectDLLDelegate(int processId, string path);
    }
}