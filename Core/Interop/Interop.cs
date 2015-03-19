using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace LeagueSharp.Loader.Core.Interop
{


    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class Interop
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpFileMappingAttributes,
            FileMapProtection flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow,
            string lpName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            HookType hookType, 
            HookProc lpfn, 
            IntPtr hMod, 
            uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(
            IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk, 
            int nCode, 
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk, 
            int nCode, 
            WM wParam, 
            [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk, 
            int nCode, 
            WM wParam, 
            [In] MSLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(
            string lpModuleName);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(
            IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(
            IntPtr hWnd,
            int nCmdShow);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(
            string dllToLoad);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procedureName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(
            IntPtr ZeroOnly,
            string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(
            IntPtr hWnd,
            StringBuilder strText,
            int maxCount);

        internal static string GetWindowText(IntPtr hWnd)
        {
            var size = GetWindowTextLength(hWnd);
            if (size++ > 0)
            {
                var builder = new StringBuilder(size);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }
            return String.Empty;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowTextLength(
            IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(
            EnumWindowsProc enumProc,
            IntPtr lParam);

        internal static void SendWindowMessage(IntPtr wnd, WindowMessageTarget channel, string data)
        {
            var lParam = new COPYDATASTRUCT {cbData = (int) channel, dwData = data.Length*2 + 2, lpData = data};
            SendMessage(wnd, 74U, IntPtr.Zero, ref lParam);
        }

        internal static bool IsZero(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }

        internal delegate IntPtr HookProc(
            int code,
            IntPtr wParam,
            IntPtr lParam);

        internal delegate bool EnumWindowsProc(
            IntPtr hWnd,
            IntPtr lParam);

        internal enum WindowMessageTarget
        {
            AppDomainManager = 1,
            Core = 2
        }
    }
}