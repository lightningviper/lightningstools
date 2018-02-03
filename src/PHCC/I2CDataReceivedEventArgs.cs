using System;
using System.Runtime.InteropServices;

namespace Phcc
{
    /// <summary>
    ///   <see cref = "I2CDataReceivedEventArgs" /> objects hold I2C data 
    ///   that is received when the PHCC motherboard signals that new
    ///   I2C data has arrived.  This data is provided by the 
    ///   <see cref = "Device.I2CDataReceived" /> event.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class I2CDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        ///   Creates an instance of 
        ///   the <see cref = "I2CDataReceivedEventArgs" /> class.
        /// </summary>
        public I2CDataReceivedEventArgs()
        {
        }

        /// <summary>
        ///   Creates an instance of
        ///   the <see cref = "I2CDataReceivedEventArgs" /> class.
        /// </summary>
        /// <param name = "address">The address of the I2C device that is 
        ///   providing the data during this event.</param>
        /// <param name = "data">The data being provided by the I2C device
        ///   during this event.</param>
        public I2CDataReceivedEventArgs(short address, byte data)
        {
            Address = address;
            Data = data;
        }

        /// <summary>
        ///   Gets/sets the address of the I2C device that is providing 
        ///   the data during this event.
        /// </summary>
        public short Address { get; set; }

        /// <summary>
        ///   Gets/sets the data being provided by the I2C device 
        ///   during this event.
        /// </summary>
        public byte Data { get; set; }
    }
}