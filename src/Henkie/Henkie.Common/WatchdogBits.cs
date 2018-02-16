using System;
using System.Runtime.InteropServices;

namespace Henkie.Common
{
    /// <summary>
    ///   ConfigureWatchdog control bit masks
    /// </summary>
    [ComVisible(false)]
    [Flags]
    internal enum WatchdogBits : byte
    {
        /// <summary>
        ///  Data (bits 0-5)
        /// </summary>
        Data = 0x1F,
        /// <summary>
        ///  Enable (bit 7)
        /// </summary>
        Enable = 0x80,
    }
}
