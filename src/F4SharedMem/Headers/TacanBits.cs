using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    public enum TacanBits : byte
    {
        band = 0x01,   // true in this bit position if band is X
        mode = 0x02,   // true in this bit position if domain is air to air
    };
}
