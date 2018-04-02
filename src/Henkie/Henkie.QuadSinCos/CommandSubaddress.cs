using System.Runtime.InteropServices;

namespace Henkie.QuadSinCos
{
    /// <summary>
    ///   Enumeration of subaddresses of the commands that can be sent to the device
    /// </summary>
    [ComVisible(true)]
    public enum CommandSubaddress : byte
    {
        /// <summary>
        ///   Set ROLL indicator in “range” 0000 – 0255 
        /// </summary>
        ROLL_0 = 0,
        /// <summary>
        ///   Set ROLL indicator in “range” 0256 – 0511 
        /// </summary>
        ROLL_1 = 1,
        /// <summary>
        ///   Set ROLL indicator in “range” 0512 – 0767 
        /// </summary>
        ROLL_2 = 2,
        /// <summary>
        ///   Set ROLL indicator in “range” 0768 – 1023 
        /// </summary>
        ROLL_3 = 3,
        /// <summary>
        ///   Set PITCH indicator in “range” 0000 – 0255  
        /// </summary>
        PITCH_0 = 4,
        /// <summary>
        ///   Set PITCH indicator in “range” 0256 – 0511  
        /// </summary>
        PITCH_1 = 5,
        /// <summary>
        ///   Set PITCH indicator in “range”  0512 – 0767  
        /// </summary>
        PITCH_2 = 6,
        /// <summary>
        ///   Set PITCH indicator in “range” 0768 – 1023 
        /// </summary>
        PITCH_3 = 7,
        /// <summary>
        ///  Set SADI “OFF” flag (0 ≡ flag visible // 1 ≡ flag hidden)
        /// </summary>
        SADI_OFF_FLAG = 8,

        /// <summary>
        ///   Set SIN value 0000 – 0255 
        /// </summary>
        SIN_0 = 9,
        /// <summary>
        ///   Set SIN value 0256 – 0511
        /// </summary>
        SIN_1 = 10,
        /// <summary>
        ///   Set SIN value 0512 – 0767 
        /// </summary>
        SIN_2 = 11,
        /// <summary>
        ///   Set SIN value 0768 – 1023 
        /// </summary>
        SIN_3 = 12,
        /// <summary>
        ///   Set COS value 0000 – 0255 
        /// </summary>
        COS_0 = 13,
        /// <summary>
        ///   Set COS value 0256 – 0511
        /// </summary>
        COS_1 = 14,
        /// <summary>
        ///   Set COS value 0512 – 0767 
        /// </summary>
        COS_2 = 15,
        /// <summary>
        ///   Set SIN value 0768 – 1023 
        /// </summary>
        COS_3 = 16,
        /// <summary>
        ///   LOAD SINE/COSINE command for specified device;
        ///   0 ≡ SADI ROLL; 1 ≡ SADI PITCH; 2 ≡ DEVICE #2; 3 ≡ DEVICE #3
        /// </summary>
        LOAD_SIN_COS=17,

        /// <summary>
        ///  Set digital output value on Device 2/3 connector
        /// </summary>
        DIG_OUT_2_3 = 18,

        /// <summary>
        ///   Set ROLL SIN value Pull-To-Cage in “range” 0000 – 0255 
        /// </summary>
        SIN_R0 = 19,
        /// <summary>
        ///   Set ROLL SIN value Pull-To-Cage in “range” 0256 – 0511
        /// </summary>
        SIN_R1 = 20,
        /// <summary>
        ///   Set ROLL SIN value Pull-To-Cage in “range” 0512 – 0767 
        /// </summary>
        SIN_R2= 21,
        /// <summary>
        ///   Set ROLL SIN value Pull-To-Cage in “range” 0768 – 1023 
        /// </summary>
        SIN_R3 = 22,
        /// <summary>
        ///   Set ROLL COS value Pull-To-Cage in “range” 0000 – 0255 
        /// </summary>
        COS_R0 = 23,
        /// <summary>
        ///   Set ROLL COS value Pull-To-Cage in “range” 0256 – 0511
        /// </summary>
        COS_R1 = 24,
        /// <summary>
        ///   Set ROLL COS value Pull-To-Cage in “range” 0512 – 0767 
        /// </summary>
        COS_R2 = 25,
        /// <summary>
        ///   Set ROLL COS value Pull-To-Cage in “range” 0768 – 1023 
        /// </summary>
        COS_R3 = 26,


        /// <summary>
        ///   Set PITCH SIN value Pull-To-Cage in “range” 0000 – 0255 
        /// </summary>
        SIN_P0 = 27,
        /// <summary>
        ///   Set PITCH SIN value Pull-To-Cage in “range” 0256 – 0511
        /// </summary>
        SIN_P1 = 28,
        /// <summary>
        ///   Set PITCH SIN value Pull-To-Cage in “range” 0512 – 0767 
        /// </summary>
        SIN_P2 = 29,
        /// <summary>
        ///   Set PITCH SIN value Pull-To-Cage in “range” 0768 – 1023 
        /// </summary>
        SIN_P3 = 30,
        /// <summary>
        ///   Set PITCH COS value Pull-To-Cage in “range” 0000 – 0255 
        /// </summary>
        COS_P0 = 31,
        /// <summary>
        ///   Set PITCH COS value Pull-To-Cage in “range” 0256 – 0511
        /// </summary>
        COS_P1 = 32,
        /// <summary>
        ///   Set PITCH COS value Pull-To-Cage in “range” 0512 – 0767 
        /// </summary>
        COS_P2 = 33,
        /// <summary>
        ///   Set PITCH COS value Pull-To-Cage in “range” 0768 – 1023 
        /// </summary>
        COS_P3 = 34,


        /// <summary>
        ///   Disable watchdog timer functionality
        /// </summary>
        DISABLE_WATCHDOG = 35,
        /// <summary>
        ///   Control watchdog timer functionality
        /// </summary>
        WATCHDOG_CONTROL = 36,
        /// <summary>
        ///   Configure DIAG LED operation mode
        /// </summary>
        DIAG_LED = 37,
        /// <summary>
        ///   USB only: send identification ““QSC vA.B $xy”
        /// </summary>
        IDENTIFY = 38,
        /// <summary>
        ///   USB DEBUG
        /// </summary>
        USB_DEBUG = 39,



    }

}
