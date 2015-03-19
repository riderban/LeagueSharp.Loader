using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace LeagueSharp.Loader.Core.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;

        public int mouseData;
        // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.

        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }
}