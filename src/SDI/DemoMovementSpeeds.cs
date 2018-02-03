using System;
using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   DEMO feature movement speed
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum DemoMovementSpeeds : byte
    {
        /// <summary>
        /// 100 milliseconds
        /// </summary>
        x100ms = 0x00,
        /// <summary>
        ///  500 milliseconds
        /// </summary>
        x500ms = 0x01,
        /// <summary>
        ///  1 second
        /// </summary>
        x1sec = 0x02,
        /// <summary>
        /// 2 seconds
        /// </summary>
        x2sec = 0x03
    }
}
