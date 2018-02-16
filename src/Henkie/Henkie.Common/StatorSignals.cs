using System;
using System.Runtime.InteropServices;

namespace Henkie.Common
{
    /// <summary>
    ///   Stator signals
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum StatorSignals:byte
    {
        /// <summary>
        ///   Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///   S1
        /// </summary>
        S1 = 0x01,
        /// <summary>
        ///   S2
        /// </summary>
        S2 = 0x02,
        /// <summary>
        ///   S3
        /// </summary>
        S3 = 0x04
    }
}
