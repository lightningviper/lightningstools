using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///  Smoothing mode options for URC "SMOOTH" mode
    /// </summary>
    [ComVisible(false)]
    public enum UpdateRateControlSmoothingMode : byte
    {
        /// <summary>
        ///  Adaptive
        /// </summary>
        Adaptive = 0x00,
        /// <summary>
        ///  Two-step
        /// </summary>
        TwoSteps = 0x01,
        /// <summary>
        ///  Four-step
        /// </summary>
        FourSteps = 0x02,
        /// <summary>
        ///  Eight-step
        /// </summary>
        EightSteps = 0x03,
    }
}