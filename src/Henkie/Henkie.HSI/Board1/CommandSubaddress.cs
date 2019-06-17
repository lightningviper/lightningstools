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
        ///   Set RANGE miles indicator "ones" digit (0-9)
        /// </summary>
        RANGE_ONES_DIGIT_0TO9 = 8,
        /// <summary>
        ///   Set RANGE miles indicator "tens" digit (0-9)
        /// </summary>
        RANGE_TENS_DIGIT_0TO9 = 9,
        /// <summary>
        ///   Set RANGE miles indicator "hundreds" digit (0-9)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_0TO9 = 10,

        /// <summary>
        ///  Set RANGE invalid indicator (bar), 0=bar visible, 1=bar hidden
        /// </summary>
        RANGE_INVALID = 11,

        /// <summary>
        ///   Set value of user-defined digital output 1; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_1 = 12,
        /// <summary>
        ///   Set value of user-defined digital output 2; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_2 = 13,

        /// <summary>
        ///   Set BEARING synchro stator S1 offset LSB value
        /// </summary>
        BEARING_S1_OFFSET_LSB = 14,
        /// <summary>
        ///   Set BEARING synchro stator S1 offset MSB value
        /// </summary>
        BEARING_S1_OFFSET_MSB = 15,
        /// <summary>
        ///   Set BEARING synchro stator S2 offset LSB value
        /// </summary>
        BEARING_S2_OFFSET_LSB = 16,
        /// <summary>
        ///   Set BEARING synchro stator S2 offset MSB value
        /// </summary>
        BEARING_S2_OFFSET_MSB = 17,
        /// <summary>
        ///   Set BEARING synchro stator S3 offset LSB value
        /// </summary>
        BEARING_S3_OFFSET_LSB = 18,
        /// <summary>
        ///   Set BEARING synchro stator S3 offset MSB value
        /// </summary>
        BEARING_S3_OFFSET_MSB = 19,

        /// <summary>
        ///   Set HEADING synchro stator S1 offset LSB value
        /// </summary>
        HEADING_S1_OFFSET_LSB = 20,
        /// <summary>
        ///   Set HEADING synchro stator S1 offset MSB value
        /// </summary>
        HEADING_S1_OFFSET_MSB = 21,
        /// <summary>
        ///   Set HEADING synchro stator S2 offset LSB value
        /// </summary>
        HEADING_S2_OFFSET_LSB = 22,
        /// <summary>
        ///   Set HEADING synchro stator S2 offset MSB value
        /// </summary>
        HEADING_S2_OFFSET_MSB = 23,
        /// <summary>
        ///   Set HEADING synchro stator S3 offset LSB value
        /// </summary>
        HEADING_S3_OFFSET_LSB = 24,
        /// <summary>
        ///   Set HEADING synchro stator S3 offset MSB value
        /// </summary>
        HEADING_S3_OFFSET_MSB = 25,

        /// <summary>
        ///   Set RANGE "ones" digit synchro X stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_ONES_DIGIT_X_STATOR_OFFSET = 26,
        /// <summary>
        ///   Set RANGE "ones" digit synchro Y stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_ONES_DIGIT_Y_STATOR_OFFSET = 27,
        /// <summary>
        ///   Set RANGE "tens" digit synchro X stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_TENS_DIGIT_X_STATOR_OFFSET = 28,
        /// <summary>
        ///   Set RANGE "tens" digit synchro Y stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_TENS_DIGIT_Y_STATOR_OFFSET = 29,
        /// <summary>
        ///   Set RANGE "hundreds" digit synchro X stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_X_STATOR_OFFSET = 30,
        /// <summary>
        ///   Set RANGE "hundreds" digit synchro Y stator offset (0x00 to 0xFF)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_Y_STATOR_OFFSET = 31,

        /// <summary>
        ///   Set RANGE indication (3-digit) in range 000 – 255 
        /// </summary>
        RANGE_3DIGIT_0TO255 = 32,
        /// <summary>
        ///   Set RANGE indication (3-digit) in range 256 – 511 
        /// </summary>
        RANGE_3DIGIT_256TO511 = 33,
        /// <summary>
        ///   Set RANGE indication (3-digit) in range 512 – 767 
        /// </summary>
        RANGE_3DIGIT_512TO767 = 34,
        /// <summary>
        ///   Set RANGE indication (3-digit) in range 768 – 999 
        /// </summary>
        RANGE_3DIGIT_768TO999 = 35,
        /// <summary>
        ///   Set RANGE update mode (0="jump scroll", 1="smooth scroll")
        /// </summary>
        RANGE_UPDATE_MODE = 36,




        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S1 deferred
        /// </summary>
        BEARING_S1_COARSE_SETPOINT_DEFERRED = 37,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S2 deferred
        /// </summary>
        BEARING_S2_COARSE_SETPOINT_DEFERRED = 38,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of BEARING stator signal S3 deferred
        /// </summary>
        BEARING_S3_COARSE_SETPOINT_DEFERRED = 39,
	    /// <summary>
        ///   set polarity of BEARING stator signals S3,S2,S1 and load amplitude (coarse setpoint) (lsb=S1 polarity)
        /// </summary>
	    BEARING_SX_POLARITY_AND_LOAD = 40,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S1 deferred
        /// </summary>
        HEADING_S1_COARSE_SETPOINT_DEFERRED = 41,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S2 deferred
        /// </summary>
        HEADING_S2_COARSE_SETPOINT_DEFERRED = 42,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of HEADING stator signal S3 deferred
        /// </summary>
        HEADING_S3_COARSE_SETPOINT_DEFERRED = 43,
        /// <summary>
        ///   set polarity of HEADING stator signals S3,S2,S1 and load amplitude (coarse setpoint) (lsb=S1 polarity)
        /// </summary>
        HEADING_SX_POLARITY_AND_LOAD = 44,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "ones" digit X stator signal deferred
        /// </summary>
        RANGE_ONES_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 45,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "ones" digit Y stator signal deferred
        /// </summary>
        RANGE_ONES_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 46,
        /// <summary>
        ///   set polarity of RANGE "ones" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_ONES_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 47,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "tens" digit X stator signal deferred
        /// </summary>
        RANGE_TENS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 48,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "tens" digit Y stator signal deferred
        /// </summary>
        RANGE_TENS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 49,
        /// <summary>
        ///   set polarity of RANGE "tens" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_TENS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 50,

        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "hundreds" digit X stator signal deferred
        /// </summary>
        RANGE_HUNDREDS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED = 51,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of RANGE "hundreds" digit Y stator signal deferred
        /// </summary>
        RANGE_HUNDREDS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED = 52,
        /// <summary>
        ///   set polarity of RANGE "hundreds" digit X and Y stator signals and load amplitude (coarse setpoint) (lsb=X polarity)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT = 53,


        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 54,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 55,
        


    	/// <summary>
        ///   Configure DIAG LED operation mode
        /// </summary>
        DIAG_LED = 56,


        /// <summary>
        ///  IDENTIFY (USB only): send identification “HSI #1 vA.B $xy” 
        /// </summary>
        IDENTIFY = 57,

        /// <summary>
        ///  DEBUG (USB only): USB DEBUG enabled (data value ='Y' or 'N')
        /// </summary>
        USB_DEBUG = 58,



    }

}
