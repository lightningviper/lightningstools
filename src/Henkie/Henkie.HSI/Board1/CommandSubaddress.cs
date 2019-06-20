using System.Runtime.InteropServices;

namespace Henkie.HSI.Board1
{
    /// <summary>
    ///   Enumeration of subaddresses of the commands that can be sent to the device
    /// </summary>
    [ComVisible(true)]
    public enum CommandSubaddress : byte
    {
        /// <summary>
        ///   Move BEARING indicator in "range" 0000 – 0255 (000 degrees to 090 degrees)
        /// </summary>
        BEARING_0TO90 = 0,
        /// <summary>
        ///   Move BEARING indicator in “range” 0256 – 0511 (090 degrees to 180 degrees)
        /// </summary>
        BEARING_90TO180 = 1,
        /// <summary>
        ///   Move BEARING indicator in “range” 0512 – 0767 (180 degrees to 270 degrees)
        /// </summary>
        BEARING_180TO270 = 2,
        /// <summary>
        ///   Move BEARING indicator in “range” 0768 – 1023 (270 degrees to 360 degrees)
        /// </summary>
        BEARING_270TO360 = 3,

        /// <summary>
        ///   Move HEADING indicator in "range" 0000 – 0255 (000 degrees to 090 degrees)
        /// </summary>
        HEADING_0TO90 = 4,
        /// <summary>
        ///   Move HEADING indicator in “range” 0256 – 0511 (090 degrees to 180 degrees)
        /// </summary>
        HEADING_90TO180 = 5,
        /// <summary>
        ///   Move HEADING indicator in “range” 0512 – 0767 (180 degrees to 270 degrees)
        /// </summary>
        HEADING_180TO270 = 6,
        /// <summary>
        ///   Move HEADING indicator in “range” 0768 – 1023 (270 degrees to 360 degrees)
        /// </summary>
        HEADING_270TO360 = 7,

        /// <summary>
        ///   Set RANGE miles indicator "ones" digit in "range" 0000 – 0255 (000 degrees to 090 degrees)
        /// </summary>
        RANGE_ONES_DIGIT_0TO90 = 8,
        /// <summary>
        ///   Set RANGE miles indicator "ones" digit in  “range” 0256 – 0511 (090 degrees to 180 degrees)
        /// </summary>
        RANGE_ONES_DIGIT_90TO180 = 9,
        /// <summary>
        ///   Set RANGE miles indicator "ones" digit in “range” 0512 – 0767 (180 degrees to 270 degrees)
        /// </summary>
        RANGE_ONES_DIGIT_180TO270 = 10,
        /// <summary>
        ///   Set RANGE miles indicator "ones" digit in “range” 0768 – 1023 (270 degrees to 360 degrees)
        /// </summary>
        RANGE_ONES_DIGIT_270TO360 = 11,


        /// <summary>
        ///   Set RANGE miles indicator "tens" digit in "range" 0000 – 0255 (000 degrees to 090 degrees)
        /// </summary>
        RANGE_TENS_DIGIT_0TO90 = 12,
        /// <summary>
        ///   Set RANGE miles indicator "tens" digit in  “range” 0256 – 0511 (090 degrees to 180 degrees)
        /// </summary>
        RANGE_TENS_DIGIT_90TO180 = 13,
        /// <summary>
        ///   Set RANGE miles indicator "tens" digit in “range” 0512 – 0767 (180 degrees to 270 degrees)
        /// </summary>
        RANGE_TENS_DIGIT_180TO270 = 14,
        /// <summary>
        ///   Set RANGE miles indicator "tens" digit in “range” 0768 – 1023 (270 degrees to 360 degrees)
        /// </summary>
        RANGE_TENS_DIGIT_270TO360 = 15,

        /// <summary>
        ///   Set RANGE miles indicator "hundreds" digit in "range" 0000 – 0255 (000 degrees to 090 degrees)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_0TO90 = 16,
        /// <summary>
        ///   Set RANGE miles indicator "hundreds" digit in  “range” 0256 – 0511 (090 degrees to 180 degrees)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_90TO180 = 17,
        /// <summary>
        ///   Set RANGE miles indicator "hundreds" digit in “range” 0512 – 0767 (180 degrees to 270 degrees)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_180TO270 = 18,
        /// <summary>
        ///   Set RANGE miles indicator "hundreds" digit in “range” 0768 – 1023 (270 degrees to 360 degrees)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_270TO360 = 19,


        /// <summary>
        ///  Set RANGE invalid indicator (bar), 0=bar visible, 1=bar hidden
        /// </summary>
        RANGE_INVALID = 20,

        /// <summary>
        ///   Set value of user-defined digital output 1; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_1 = 21,
        /// <summary>
        ///   Set value of user-defined digital output 2; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_2 = 22,

        /// <summary>
        ///   Set stator coil offset value (lower 8 bits)
        /// </summary>
        SET_STATOR_COIL_OFFSET_LSB = 23,
        /// <summary>
        ///   Set stator coil offset value (upper 2 bits)
        /// </summary>
        SET_STATOR_COIL_OFFSET_MSB = 24,
        /// <summary>
        ///   Load BEARING offset value stator coil mask (0x01 =S1, 0x02 = S2, 0x04 = S3
        /// </summary>
        LOAD_BEARING_OFFSET_STATOR_COIL_MASK = 25,
        /// <summary>
        ///   Load HEADING offset value stator coil mask (0x01 =S1, 0x02 = S2, 0x04 = S3
        /// </summary>
        LOAD_HEADING_OFFSET_STATOR_COIL_MASK = 26,
        /// <summary>
        ///   Load RANGE offset value stator coil mask (0x01 =range (ones digit) X, 0x02 = range (ones digit) Y, 0x04 = range (tens digit) X, 0x08 = range (tens digit) Y, 0x10 = range (hundreds digit) X, 0x20 = range (hundreds digit) Y
        /// </summary>
        LOAD_RANGE_OFFSET_STATOR_COIL_MASK = 27,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S1 deferred
        /// </summary>
        BEARING_S1_COARSE_SETPOINT_DEFERRED = 28,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S2 deferred
        /// </summary>
        BEARING_S2_COARSE_SETPOINT_DEFERRED = 29,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S3 deferred
        /// </summary>
        BEARING_S3_COARSE_SETPOINT_DEFERRED = 30,
	    /// <summary>
        ///   set polarity of BEARING stator signals S3,S2,S1 and load amplitude (coarse setpoint) (lsb=S1 polarity)
        /// </summary>
	    BEARING_SX_POLARITY_AND_LOAD = 31,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S1 deferred
        /// </summary>
        HEADING_S1_COARSE_SETPOINT_DEFERRED = 32,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S2 deferred
        /// </summary>
        HEADING_S2_COARSE_SETPOINT_DEFERRED = 33,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S3 deferred
        /// </summary>
        HEADING_S3_COARSE_SETPOINT_DEFERRED = 34,
        /// <summary>
        ///   set polarity of HEADING stator signals S3,S2,S1 and load amplitude (coarse setpoint) (lsb=S1 polarity)
        /// </summary>
        HEADING_SX_POLARITY_AND_LOAD = 35,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "ones" digit X stator signal deferred
        /// </summary>
        RANGE_ONES_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 36,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "ones" digit Y stator signal deferred
        /// </summary>
        RANGE_ONES_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 37,
        /// <summary>
        ///   set polarity of RANGE "ones" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_ONES_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 38,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "tens" digit X stator signal deferred
        /// </summary>
        RANGE_TENS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 39,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "tens" digit Y stator signal deferred
        /// </summary>
        RANGE_TENS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 40,
        /// <summary>
        ///   set polarity of RANGE "tens" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_TENS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 41,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "hundreds" digit X stator signal deferred
        /// </summary>
        RANGE_HUNDREDS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 42,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "hundreds" digit Y stator signal deferred
        /// </summary>
        RANGE_HUNDREDS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 43,
        /// <summary>
        ///   set polarity of RANGE "hundreds" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 44,


        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 45,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 46,
        


    	/// <summary>
        ///   Configure DIAG LED operation mode
        /// </summary>
        DIAG_LED = 47,


        /// <summary>
        ///  IDENTIFY (USB only): send identification “HSI #1 vA.B $xy” 
        /// </summary>
        IDENTIFY = 48,

        /// <summary>
        ///  DEBUG (USB only): USB DEBUG enabled (data value ='Y' or 'N')
        /// </summary>
        USB_DEBUG = 49,



    }

}
