using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace LeagueSharp.Loader.Core.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct COPYDATASTRUCT
    {
        public int cbData;
        public int dwData;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpData;
    }
}