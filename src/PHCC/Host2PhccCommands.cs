using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    ///   Enumeration of PHCC command codes
    /// </summary>
    internal enum Host2PhccCommands : byte
    {
        /// <summary>
        ///   Set the PHCC to IDLE mode.
        /// </summary>
        Idle = 0x00,
        /// <summary>
        ///   Command the PHCC to perform a software reset on itself.
        /// </summary>
        Reset = 0x01,
        /// <summary>
        ///   Command the PHCC to start sending automatic digital input and 
        ///   analog input change notifications.
        /// </summary>
        StartTalking = 0x02,
        /// <summary>
        ///   Command the PHCC to stop sending automatic digital input and 
        ///   analog input change notifications
        /// </summary>
        StopTalking = 0x03,
        /// <summary>
        ///   Command the PHCC to send a full list of the current values 
        ///   of all digital inputs.
        /// </summary>
        GetCurrentDigitalInputValues = 0x04,
        /// <summary>
        ///   Command the PHCC to send a full list of the current values 
        ///   of all prioritized and non-prioritized analog inputs.
        /// </summary>
        GetCurrentAnalogInputValues = 0x05,
        /// <summary>
        ///   Command the PHCC to send data to an attached I2C peripheral.
        /// </summary>
        I2CSend = 0x06,
        /// <summary>
        ///   Command the PHCC to send data to an attached
        ///   Digital Output Type A (DOA) peripheral.
        /// </summary>
        DoaSend = 0x07,
        /// <summary>
        ///   Command the PHCC to send data to an attached 
        ///   Digital Output Type B (DOB) peripheral.
        /// </summary>
        DobSend = 0x08
    }

}
