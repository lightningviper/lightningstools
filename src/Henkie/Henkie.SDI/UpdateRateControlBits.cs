using System;
using System.Runtime.InteropServices;

namespace Henkie.SDI
{
    /// <summary>
    ///   URC feature control bit mask
    /// </summary>
    [ComVisible(false)]
    [Flags]
    internal enum UpdateRateControlBits : byte
    {
        /// <summary>
        ///  Data (bits 0-5)
        /// </summary>
        Data = 0x1F,
        /// <summary>
        ///  Modus (bits 6-7)
        /// </summary>
        Mode = 0xC0,
    }
}
