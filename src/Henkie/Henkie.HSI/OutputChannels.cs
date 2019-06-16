using System;
using System.Runtime.InteropServices;

namespace Henkie.HSI
{
    /// <summary>
    ///   Output channels
    /// </summary>
    [Flags]
    [ComVisible(true)]
    public enum OutputChannels: byte
    {
        /// <summary>
        ///  Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///  User defined digital output #1
        /// </summary>
        DIG_OUT_1 = 1 << 1,
        /// <summary>
        ///  User defined digital output #2
        /// </summary>
        DIG_OUT_2 = 1 << 2,
    }
}
