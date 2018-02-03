using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    //Source interface for events to be exposed.
    //Add GuidAttribute to the source interface to supply an explicit System.Guid.
    //Add InterfaceTypeAttribute to indicate that interface is IDispatch interface.
    /// <summary>
    ///   COM Event Source Interface
    /// </summary>
    [Guid("8709CA5D-79FA-4a63-ACF4-C99475990BC3")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface PhccEvents
    {
        /// <summary>
        ///   The <see cref = "DigitalInputChanged" /> event is raised when 
        ///   the PHCC motherboard detects that one of the digital inputs
        ///   has changed (i.e. whenever a button that is wired 
        ///   into the digital input key matrix is pressed or released).
        /// </summary>
        [DispId(1)]
        void DigitalInputChanged(object sender, DigitalInputChangedEventArgs e);

        /// <summary>
        ///   The <see cref = "AnalogInputChanged" /> event is raised when
        ///   the PHCC motherboard detects that one of the analog inputs 
        ///   has changed values (i.e. whenever an analog input signal 
        ///   changes state).
        /// </summary>
        [DispId(2)]
        void AnalogInputChanged(object sender, AnalogInputChangedEventArgs e);

        /// <summary>
        ///   The <see cref = "I2CDataReceived" /> event is raised when
        ///   the PHCC motherboard receives data from one of the attached 
        ///   I2C peripherals (if any).
        /// </summary>
        [DispId(3)]
        void I2CDataReceived(object sender, I2CDataReceivedEventArgs e);
    }



    /// <summary>
    ///   Event handler delegate for the <see cref = "Device.DigitalInputChanged" /> event.
    /// </summary>
    /// <param name = "sender">the object raising the event.</param>
    /// <param name = "e">a <see cref = "DigitalInputChangedEventArgs" /> object containing detailed information about the event.</param>
    [ComVisible(false)]
    public delegate void DigitalInputChangedEventHandler(object sender, DigitalInputChangedEventArgs e);

    /// <summary>
    ///   Event handler delegate for the <see cref = "Device.AnalogInputChanged" /> event.
    /// </summary>
    /// <param name = "sender">the object raising the event.</param>
    /// <param name = "e">a <see cref = "AnalogInputChangedEventArgs" /> object containing detailed information about the event.</param>
    [ComVisible(false)]
    public delegate void AnalogInputChangedEventHandler(object sender, AnalogInputChangedEventArgs e);

    /// <summary>
    ///   Event handler delegate for the <see cref = "Device.I2CDataReceived" /> event.
    /// </summary>
    /// <param name = "sender">the object raising the event.</param>
    /// <param name = "e">an <see cref = "I2CDataReceivedEventArgs " /> object containing detailed information about the event.</param>
    [ComVisible(false)]
    public delegate void I2CDataReceivedEventHandler(object sender, I2CDataReceivedEventArgs e);


}
