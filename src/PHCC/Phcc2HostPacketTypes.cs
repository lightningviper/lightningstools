using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    ///   Enumeration of packet types that PHCC returns over the serial 
    ///   interface
    /// </summary>
    internal enum Phcc2HostPacketTypes : byte
    {
        /// <summary>
        ///   Packet terminator containing all zeros
        /// </summary>
        AllBitsZero = 0x00,
        /// <summary>
        ///   Packet header indicating a digital input change notification 
        ///   packet
        /// </summary>
        DigitalInputUpdatePacket = 0x20,
        /// <summary>
        ///   Packet header indicating an analog input change notification 
        ///   packet
        /// </summary>
        AnalogInputUpdatePacket = 0x40,
        /// <summary>
        ///   Packet header indicating data received from an attached 
        ///   I2C peripheral
        /// </summary>
        I2CDataReceivedPacket = 0x60,
        /// <summary>
        ///   Packet header indicating a packet containing the values of all 
        ///   digital inputs
        /// </summary>
        DigitalInputsFullDumpPacket = 0x80,
        /// <summary>
        ///   Packet header indicating a packet containing the values of all 
        ///   prioritized and non-prioritized analog inputs
        /// </summary>
        AnalogInputsFullDumpPacket = 0xA0,
        /// <summary>
        ///   Packet terminator containing all ones
        /// </summary>
        AllBitsOne = 0xFF,
        /// <summary>
        ///   Bitmask for determining the packet type from the
        ///   3 most-significant bits of the first byte of a data packet
        /// </summary>
        PacketTypeMask = 0xE0,
    }

}
