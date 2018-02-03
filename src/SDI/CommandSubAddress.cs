using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Enumeration of subaddresses of the commands that can be sent to the device
    /// </summary>
    [ComVisible(true)]
    public enum CommandSubaddress : byte
    {
        /// <summary>
        ///   Move indicator in quadrant 1
        /// </summary>
        SSYNQ1 = 0,
        /// <summary>
        ///   Move indicator in quadrant 2
        /// </summary>
        SSYNQ2 = 1,
        /// <summary>
        ///   Move indicator in quadrant 3
        /// </summary>
        SSYNQ3 = 2,
        /// <summary>
        ///   Move indicator in quadrant 4
        /// </summary>
        SSYNQ4 = 3,
        /// <summary>
        ///   Move indicator (coarse resolution)
        /// </summary>
        SYN8BIT = 12,



        /// <summary>
        ///   Set amplitude of the stator signal S1 (immediate)
        /// </summary>
        S1PWM = 34,
        /// <summary>
        ///   Set amplitude of the stator signal S2 (immediate)
        /// </summary>
        S2PWM = 35,
        /// <summary>
        ///   Set amplitude of the stator signal S3 (immediate)
        /// </summary>
        S3PWM = 36,
        /// <summary>
        ///   Set polarity of the stator signals S1, S2, S3 (immediate)
        /// </summary>
        SXPOL = 37,


        /// <summary>
        ///   Set amplitude of the stator signal S1 (deferred)
        /// </summary>
        S1PWMD = 6,
        /// <summary>
        ///   Set amplitude of the stator signal S2 (deferred)
        /// </summary>
        S2PWMD = 7,
        /// <summary>
        ///   Set amplitude of the stator signal S8 (deferred)
        /// </summary>
        S3PWMD = 8,
        /// <summary>
        ///   Set polarity of the stator signals S1, S2, S3 + LOAD deferred amplitudes
        /// </summary>
        SXPOLD = 9,

        /// <summary>
        ///   Set indicator movement limit minimum
        /// </summary>
        LIMIT_MIN = 4,
        /// <summary>
        ///   Set indicator movement limit maximum
        /// </summary>
        LIMIT_MAX = 5,



        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 10,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 11,

        /// <summary>
        ///   Control the power-down feature
        /// </summary>
        POWER_DOWN = 13,



        /// <summary>
        ///   Configure user-defined digital/PWM outputs
        /// </summary>
        DIG_PWM = 14,

        /// <summary>
        ///   Set value of user-defined digital/PWM output 1
        /// </summary>
        DIG_PWM_1 = 15,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 2
        /// </summary>
        DIG_PWM_2 = 16,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 3
        /// </summary>
        DIG_PWM_3 = 17,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 4
        /// </summary>
        DIG_PWM_4 = 18,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 5
        /// </summary>
        DIG_PWM_5 = 19,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 6
        /// </summary>
        DIG_PWM_6 = 20,
        /// <summary>
        ///   Set value of user-defined digital/PWM output 7
        /// </summary>
        DIG_PWM_7 = 21,

        /// <summary>
        ///   Set duty cycle of on-board OpAmp buffered PWM output
        /// </summary>
        ONBOARD_PWM = 22,

        /// <summary>
        ///   Configure URC (Update Rate Control)
        /// </summary>
        UPDATE_RATE_CONTROL = 23,

        /// <summary>
        ///   Configure DIAG LED operation mode
        /// </summary>
        DIAG_LED = 24,

        /// <summary>
        ///   Set stator S1 base angle LSB value
        /// </summary>
        S1_BASE_ANGLE_LSB = 25,
        /// <summary>
        ///   Set stator S1 base angle MSB value
        /// </summary>
        S1_BASE_ANGLE_MSB = 26,
        /// <summary>
        ///   Set stator S2 base angle LSB value
        /// </summary>
        S2_BASE_ANGLE_LSB = 27,
        /// <summary>
        ///   Set stator S2 base angle MSB value
        /// </summary>
        S2_BASE_ANGLE_MSB = 28,
        /// <summary>
        ///   Set stator S3 base angle LSB value
        /// </summary>
        S3_BASE_ANGLE_LSB = 29,
        /// <summary>
        ///   Set stator S3 base angle MSB value
        /// </summary>
        S3_BASE_ANGLE_MSB = 30,

        /// <summary>
        ///   Configure DEMO mode
        /// </summary>
        DEMO_MODE = 31,
        /// <summary>
        ///   Set start position used for DEMO mode
        /// </summary>
        DEMO_MODE_START_POSITION = 32,
        /// <summary>
        ///   Set END position used for DEMO mode
        /// </summary>
        DEMO_MODE_END_POSITION = 33,


        /// <summary>
        ///   Request device identification (valid on USB interface only)
        /// </summary>
        IDENTIFY = 38,

        /// <summary>
        ///   USB DEBUG
        /// </summary>
        USB_DEBUG = 39,



    }

}
