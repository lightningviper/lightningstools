using System;
using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Powerdown feature control bit masks
    /// </summary>
    [ComVisible(false)]
    [Flags]
    internal enum PowerDownBits: byte
    {
        /// <summary>
        ///  Delay time (bits 0-5)
        /// </summary>
        DelayTime=0x1F,
        /// <summary>
        ///  Power down level (bit 6)
        /// </summary>
        Level = 0x40,
        /// <summary>
        ///  Power down enabled flag (bit 7)
        /// </summary>
        Enabled = 0x80,
    }
}
