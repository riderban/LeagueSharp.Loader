using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace LeagueSharp.Loader.Core.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential)]
    internal class KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}