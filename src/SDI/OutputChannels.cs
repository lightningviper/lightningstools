using System;
using System.Runtime.InteropServices;

namespace SDI
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
        ///  On-board Op-Amp buffered PWM
        /// </summary>
        PWM_OUT = 1,
        /// <summary>
        ///  User defined output #1
        /// </summary>
        DIG_PWM_1 = 1 << 1,
        /// <summary>
        ///  User defined output #2
        /// </summary>
        DIG_PWM_2 = 1 << 2,
        /// <summary>
        ///  User defined output #3
        /// </summary>
        DIG_PWM_3 = 1 << 3,
        /// <summary>
        ///  User defined output #4
        /// </summary>
        DIG_PWM_4 = 1 << 4,
        /// <summary>
        ///  User defined output #5
        /// </summary>
        DIG_PWM_5 = 1 << 5,
        /// <summary>
        ///  User defined output #6
        /// </summary>
        DIG_PWM_6 = 1 << 6,
        /// <summary>
        ///  User defined output #7
        /// </summary>
        DIG_PWM_7 = 1 << 7,
    }
}
