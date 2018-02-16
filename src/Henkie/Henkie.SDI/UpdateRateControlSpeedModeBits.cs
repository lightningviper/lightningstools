using System.Runtime.InteropServices;

namespace Henkie.SDI
{
    /// <summary>
    ///   Bit masks for URC Speed Mode
    /// </summary>
    [ComVisible(false)]
    internal enum UpdateRateControlSpeedModeBits : byte
    {
        /// <summary>
        ///  Step update delay
        /// </summary>
        StepUpdateDelay = 0x3c,
        /// <summary>
        ///  Smooth update 
        /// </summary>
        SmoothUpdate = 0x03,
    }
}
