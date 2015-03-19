using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net;
using LeagueSharp.Loader.Core.Interop;

namespace LeagueSharp.Loader.Core
{
    internal class KeyboardHook
    {
        public delegate void OnKeyUp(int vKeyCode);

        public static event OnKeyUp OnKeyUpTrigger;

        public static void SetHook()
        {
            using (var curModule = Process.GetCurrentProcess().MainModule)
            {
                _hHook = Interop.Interop.SetWindowsHookEx(HookType.WH_KEYBOARD, Proc,
                    Interop.Interop.GetModuleHandle(curModule.ModuleName), 0);
            }

            Log.Info("Hook " + _hHook);
        }

        public static void UnHook()
        {
            Log.Info("UnHook " + _hHook);
            if (_hHook != IntPtr.Zero)
            {
                Interop.Interop.UnhookWindowsHookEx(_hHook);
            }
        }

        public static IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr) 0x101)
            {
                if (OnKeyUpTrigger != null && HookedKeys.Contains(Marshal.ReadInt32(lParam)))
                {
                    OnKeyUpTrigger(Marshal.ReadInt32(lParam));
                }
            }
            return Interop.Interop.CallNextHookEx(_hHook, code, wParam, lParam);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static List<int> HookedKeys = new List<int>();
        private static readonly Interop.Interop.HookProc Proc = HookProc;
        private static IntPtr _hHook = IntPtr.Zero;
    }
}