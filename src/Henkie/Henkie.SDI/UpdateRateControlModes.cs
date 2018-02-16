using System;
using System.Runtime.InteropServices;

namespace Henkie.SDI
{
    /// <summary>
    ///   URC modes
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum UpdateRateControlModes: byte
    {
        /// <summary>
        /// Limit 
        /// </summary>
        Limit = 0x00,
        /// <summary>
        /// Smooth
        /// </summary>
        Smooth = 0x01 <<6,
        /// <summary>
        /// Speed
        /// </summary>
        Speed = 0x02 <<6,
        /// <summary>
        /// Miscellaneous
        /// </summary>
        Miscellaneous = 0x03 <<6,
    }
}
