using System.Runtime.InteropServices;

namespace Henkie.FuelFlow
{
    /// <summary>
    ///   Enumeration of subaddresses of the commands that can be sent to the device
    /// </summary>
    [ComVisible(true)]
    public enum CommandSubaddress : byte
    {
        /// <summary>
        ///   Move indicator in “range” 0000 – 0255
        /// </summary>
        SALT_0 = 0,
        /// <summary>
        ///   Move indicator in “range” 0256 – 0511
        /// </summary>
        SALT_1 = 1,
        /// <summary>
        ///   Move indicator in “range” 0512 – 0767
        /// </summary>
        SALT_2 = 2,
        /// <summary>
        ///   Move indicator in “range” 0768 – 1023
        /// </summary>
        SALT_3 = 3,
        /// <summary>
        ///   Move indicator in “range” 1024 – 1279
        /// </summary>
        SALT_4 = 4,
        /// <summary>
        ///   Move indicator in “range” 1280 – 1535
        /// </summary>
        SALT_5 = 5,
        /// <summary>
        ///   Move indicator in “range” 1536 – 1791
        /// </summary>
        SALT_6 = 6,
        /// <summary>
        ///   Move indicator in “range” 1792 – 2047
        /// </summary>
        SALT_7 = 7,
        /// <summary>
        ///   Move indicator in “range” 2048 – 2302
        /// </summary>
        SALT_8 = 8,
        /// <summary>
        ///   Move indicator in “range” 2304 – 2559
        /// </summary>
        SALT_9 = 9,
        /// <summary>
        ///   Move indicator in “range” 2560 – 2815
        /// </summary>
        SALT_A = 10,
        /// <summary>
        ///   Move indicator in “range” 2816 – 3071
        /// </summary>
        SALT_B = 11,
        /// <summary>
        ///   Move indicator in “range” 3072 – 332
        /// </summary>
        SALT_C = 12,
        /// <summary>
        ///   Move indicator in “range” 3328 – 3583
        /// </summary>
        SALT_D = 13,
        /// <summary>
        ///   Move indicator in “range” 3584 – 3839
        /// </summary>
        SALT_E = 14,
        /// <summary>
        ///   Move indicator in “range” 3840 – 4095
        /// </summary>
        SALT_F = 15,


	    /// <summary>
        ///   Set amplitude (coarse) of stator signal S1 deferred
        /// </summary>
        S1AMPL = 16,
	    /// <summary>
        ///   Set amplitude (coarse) of stator signal S2 deferred
        /// </summary>
        S2AMPL = 17,
	    /// <summary>
        ///   Set amplitude (coarse) of stator signal S3 deferred
        /// </summary>
        S3AMPL = 18,
	    /// <summary>
        ///   set polarity of stator signals &amp; load S[1,2,3]AMPL
        /// </summary>
	    SxPOL = 19,

        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 20,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 21,
        

        /// <summary>
        ///   Set value of user-defined digital output 1
        /// </summary>
        DIG_OUT_1 = 22,
        /// <summary>
        ///   Set value of user-defined digital output 2
        /// </summary>
        DIG_OUT_2 = 23,
        /// <summary>
        ///   Set value of user-defined digital output 3
        /// </summary>
        DIG_OUT_3 = 24,

        /// <summary>
        ///   Set value of user-defined digital output 4
        /// </summary>
        DIG_OUT_4 = 25,
        /// <summary>
        ///   Set value of user-defined digital output 5
        /// </summary>
        DIG_OUT_5 = 26,

        /// <summary>
        ///   Configure DIAG LED operation mode
        /// </summary>
        DIAG_LED = 27,


        /// <summary>
        ///   Set stator S1 base angle LSB value
        /// </summary>
        S1_BASE_ANGLE_LSB = 28,
        /// <summary>
        ///   Set stator S1 base angle MSB value
        /// </summary>
        S1_BASE_ANGLE_MSB = 29,
        /// <summary>
        ///   Set stator S2 base angle LSB value
        /// </summary>
        S2_BASE_ANGLE_LSB = 30,
        /// <summary>
        ///   Set stator S2 base angle MSB value
        /// </summary>
        S2_BASE_ANGLE_MSB = 31,
        /// <summary>
        ///   Set stator S3 base angle LSB value
        /// </summary>
        S3_BASE_ANGLE_LSB = 32,
        /// <summary>
        ///   Set stator S3 base angle MSB value
        /// </summary>
        S3_BASE_ANGLE_MSB = 33,


        /// <summary>
        ///   Set stator S1 high-accuracy angle LSB value
        /// </summary>
        S1_HIGH_ACCURACY_ANGLE_LSB = 34,
        /// <summary>
        ///   Set stator S1 high-accuracy angle MSB value
        /// </summary>
        S1_HIGH_ACCURACY_ANGLE_MSB = 35,
        /// <summary>
        ///   Set stator S2 high-accuracy angle LSB value
        /// </summary>
        S2_HIGH_ACCURACY_ANGLE_LSB = 36,
        /// <summary>
        ///   Set stator S2 high-accuracy angle MSB value
        /// </summary>
        S2_HIGH_ACCURACY_ANGLE_MSB = 37,
        /// <summary>
        ///   Set stator S3 high-accuracy angle LSB value
        /// </summary>
        S3_HIGH_ACCURACY_ANGLE_LSB = 38,
        /// <summary>
        ///   Set stator S3 high-accuracy angle MSB value
        /// </summary>
        S3_HIGH_ACCURACY_ANGLE_MSB = 39,

	    /// <summary>
        ///   Set polarity high-accurate angle values AND load
        /// </summary>
        SXPOLD = 40,

	    /// <summary>
        ///   USB only: send identification “ALT vA.B $xy”
        /// </summary>
        IDENTIFY = 41,

        /// <summary>
        ///   USB DEBUG
        /// </summary>
        USB_DEBUG = 42,



    }

}
