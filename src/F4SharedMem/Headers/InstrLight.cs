using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    // instrument backlight brightness
    public enum InstrLight : byte
    {
        INSTR_LIGHT_OFF = 0,
        INSTR_LIGHT_DIM = 1,
        INSTR_LIGHT_BRT = 2,
    };
}
