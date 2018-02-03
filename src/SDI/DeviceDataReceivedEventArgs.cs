using System;
using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   <see cref = "DeviceDataReceivedEventArgs" /> objects provides data received from the device.
    ///   The <see cref = "Device.DataReceived" /> event
    ///   provides <see cref = "DeviceDataReceivedEventArgs" /> event-args
    ///   objects during the raising of each event.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class DeviceDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}