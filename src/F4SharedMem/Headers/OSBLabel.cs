using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    // MFD On Screen Button Labels(OF)
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OSBLabel
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public sbyte[] Line1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public sbyte[] Line2;
        [MarshalAs(UnmanagedType.I1)]
        public bool Inverted;
    }
}
