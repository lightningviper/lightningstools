using System;
using System.Runtime.InteropServices;

namespace SDI
{
    //Source interface for events to be exposed.
    //Add GuidAttribute to the source interface to supply an explicit System.Guid.
    //Add InterfaceTypeAttribute to indicate that interface is IDispatch interface.
    /// <summary>
    ///   COM Event Source Interface
    /// </summary>
    [Guid("F65CD30C-8CF7-4902-999D-8AC58F207666")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IDeviceEvents
    {

        /// <summary>
        ///   The <see cref = "DataReceived" /> event is raised when
        ///   the device sends data the the host (PC).
        /// </summary>
        [DispId(1)]
        void DataReceived(object sender, DeviceDataReceivedEventArgs e);
    }

}
