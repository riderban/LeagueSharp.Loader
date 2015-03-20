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

        internal static bool IsZero(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }
    }
}