using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EWMU_LineOfText
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] chars;
    }
}
