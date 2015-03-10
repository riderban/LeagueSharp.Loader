using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeagueSharp.Loader.Core
{
    internal class KeyboardHook
    {
        public delegate void OnKeyUp(int vKeyCode);
        public static List<int> HookedKeys = new List<int>();
        private static readonly KeyboardProc Proc = HookProc;
        private static IntPtr _hHook = IntPtr.Zero;
        public static event OnKeyUp OnKeyUpTrigger;

        #region Native

        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        public static void SetHook()
        {
            using (var curModule = Process.GetCurrentProcess().MainModule)
            {
                _hHook = SetWindowsHookEx(13, Proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public static void UnHook()
        {
            if (_hHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hHook);
            }
        }

        public static IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)0x101)
            {
                if (OnKeyUpTrigger != null && HookedKeys.Contains(Marshal.ReadInt32(lParam)))
                {
                    OnKeyUpTrigger(Marshal.ReadInt32(lParam));
                }
            }
            return CallNextHookEx(_hHook, code, (int)wParam, lParam);
        }
    }
}