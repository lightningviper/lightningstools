using System;
using System.Runtime.InteropServices;

namespace Henkie.FuelFlow
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
        /// <summary>
        ///  User defined digital output #3
        /// </summary>
        DIG_OUT_3 = 1 << 3,
        /// <summary>
        ///  User defined digital output #4
        /// </summary>
        DIG_OUT_4 = 1 << 4,
        /// <summary>
        ///  User defined digital output #5
        /// </summary>
        DIG_OUT_5 = 1 << 5,
    }
}
