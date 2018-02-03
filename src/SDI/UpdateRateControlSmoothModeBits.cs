using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Bitmasks for URC Smooth Mode
    /// </summary>
    [ComVisible(false)]
    internal enum UpdateRateControlSmoothModeBits : byte
    {
        /// <summary>
        ///  Smoothing minimum threshold value
        /// </summary>
        SmoothingMinimumThresholdValue= 0x3c,
        /// <summary>
        ///  Smooth update 
        /// </summary>
        SmoothUpdate = 0x03,
    }
}
