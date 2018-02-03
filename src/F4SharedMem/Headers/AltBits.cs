using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Flags]
    [Serializable]
    public enum AltBits : int
    {
        CalType = 0x01,	// true if calibration in inches of Mercury (Hg), false if in hectoPascal (hPa)
        PneuFlag = 0x02,	// true if PNEU flag is visible
    };
}
