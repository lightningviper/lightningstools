using System.Runtime.InteropServices;

namespace Henkie.HSI.Board2
{
    /// <summary>
    ///   Enumeration of subaddresses of the commands that can be sent to the device
    /// </summary>
    [ComVisible(true)]
    public enum CommandSubaddress : byte
    {

        /// <summary>
        ///   Move course deviation indicator in "range" 000 – 255
        /// </summary>
        CDI_000TO255 = 0,
        /// <summary>
        ///   Move course deviation indicator in "range" 256 – 511
        /// </summary>
        CDI_256TO511 = 1,
        /// <summary>
        ///   Move course deviation indicator in "range" 512 – 767
        /// </summary>
        CDI_512TO767 = 2,
        /// <summary>
        ///   Move course deviation indicator in "range" 768 – 1023
        /// </summary>
        CDI_768TO1023 = 3,


        /// <summary>
        ///  Navigation Warning flag (0=visible, 1=not visible)
        /// </summary>
        NAVIGATION_WARNING_FLAG = 4,
        /// <summary>
        ///  TO/FROM indication (0=none, 1=TO, 2=FROM)
        /// </summary>
        TO_FROM_INDICATION = 5,

        /// <summary>
        ///   Request heading info update (results in one reply message)
        /// </summary>
        REQUEST_HEADING_INFO_UPDATE = 6,
        /// <summary>
        ///   Convert heading value to degrees (1 = convert)
        /// </summary>
        CONVERT_HEADING_VALUE_TO_DEGREES = 7,
        /// <summary>
        ///   Heading value hysteresis threshold (0x00 to 0x7F)
        /// </summary>
        HEADING_VALUE_HYSTERISIS_THRESHOLD = 8,

        /// <summary>
        ///   Request course info update (results in one reply message)
        /// </summary>
        REQUEST_COURSE_INFO_UPDATE = 9,
        /// <summary>
        ///   Convert course value to degrees (1 = convert)
        /// </summary>
        CONVERT_COURSE_VALUE_TO_DEGREES = 10,
        /// <summary>
        ///   Heading value hysteresis threshold (0x00 to 0x7F)
        /// </summary>
        COURSE_VALUE_HYSTERISIS_THRESHOLD = 11,

        /// <summary>
        ///   USB messaging option 
        ///     0 – send never (disabled)
        ///     1 – send only on request (as reply to command 6 or 9)
        ///     2 – send on interval (periodically)
        ///     3 – as option 2, but only if the value has changed
        /// </summary>
        USB_MESSAGING_OPTIONS= 12,
        /// <summary>
        ///  USB Message Time Interval (0x00 to 0xFF)
        /// (1 tick = 4 ms delay), default is 25 ticks.
        ///</summary>
        USB_MESSAGE_TIME_INTERVAL=13,

        /// <summary>
        /// Sine/Cosine Alignment
        ///</summary>
        SINE_COSINE_ALIGNMENT = 14,


        /// <summary>
        ///   Set course synchro exciter in "range" 000 – 255
        /// </summary>
        COURSE_SYNCHRO_EXCITER_000TO255 = 15,
        /// <summary>
        ///   Set course synchro exciter in "range" 256 – 511
        /// </summary>
        COURSE_SYNCHRO_EXCITER_256TO511 = 16,
        /// <summary>
        ///   Set course synchro exciter in "range" 512 – 767
        /// </summary>
        COURSE_SYNCHRO_EXCITER_512TO767 = 17,
        /// <summary>
        ///   Set course synchro exciter in "range" 768 – 1023
        /// </summary>
        COURSE_SYNCHRO_EXCITER_768TO1023 = 18,

        /// <summary>
        ///   Set course synchro exciter stator coil offset value (lower 8 bits)
        /// </summary>
        SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_LSB = 19,
        /// <summary>
        ///   Set course synchro exciter stator coil offset value (upper 2 bits)
        /// </summary>
        SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_MSB = 20,
        /// <summary>
        ///   Load course synchro exciter stator coil offset (0x01 =S1, 0x02 = S2, 0x04 = S3)
        /// </summary>
        LOAD_COURSE_SYNCHRO_EXCITER_OFFSET_STATOR_COIL_MASK = 21,
        /// <summary>


        /// <summary>
        ///   Set value of user-defined digital output A; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_A = 22,
        /// <summary>
        ///   Set value of user-defined digital output B; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_B = 23,
        /// <summary>
        ///   Set value of user-defined digital output X; 0=logic zero (false), 1=logic one (true)
        /// </summary>
        DIG_OUT_X = 24,




        /// <summary>
        ///   Set amplitude (coarse setpoint) of COURSE EXCITER stator signal S1 deferred
        /// </summary>
        COURSE_EXCITER_S1_COARSE_SETPOINT_DEFERRED = 25,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of COURSE EXCITER stator signal S2 deferred
        /// </summary>
        COURSE_EXCITER_S2_COARSE_SETPOINT_DEFERRED = 26,
        /// <summary>
        ///   Set amplitude (coarse setpoint) of COURSE EXCITER stator signal S3 deferred
        /// </summary>
        COURSE_EXCITER_S3_COARSE_SETPOINT_DEFERRED = 27,
        /// <summary>
        ///   set polarity of COURSE EXCITER stator signals S3,S2,S1 and load amplitude (coarse setpoint) (lsb=S1 polarity)
        /// </summary>
        COURSE_EXCITER_SX_POLARITY_AND_LOAD = 28,








        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 29,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 30,
        


    	/// <summary>
        ///   Configure DIAG LED operation mode
        ///   0 LED always OFF 
        ///   1 LED always ON 
        ///   2 LED flashes at heart beat rate (power-up default) 
        ///   3 LED toggles ON/OFF state per accepted command
        ///   4 LED is ON during DOA packet reception
        /// </summary>
        DIAG_LED = 31,


        /// <summary>
        ///  IDENTIFY (USB only): send identification “HSI #2 vA.B $xy” 
        /// </summary>
        IDENTIFY = 48,

        /// <summary>
        ///  DEBUG (USB only): USB DEBUG enabled (data value ='Y' or 'N')
        /// </summary>
        USB_DEBUG = 49,



    }

}
