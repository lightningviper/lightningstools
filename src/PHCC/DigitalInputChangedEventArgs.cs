using System;
using System.Runtime.InteropServices;

namespace Phcc
{
    /// <summary>
    ///   <see cref = "DigitalInputChangedEventArgs" /> objects hold data that the PHCC motherboard provides whenever a digital 
    ///   input value changes.
    ///   The <see cref = "Device.DigitalInputChanged" /> event  
    ///   provides <see cref = "DigitalInputChangedEventArgs" /> event-args 
    ///   objects during the raising of each event.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class DigitalInputChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Creates an instance of the <see cref = "DigitalInputChangedEventArgs" /> class.
        /// </summary>
        public DigitalInputChangedEventArgs()
        {
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "DigitalInputChangedEventArgs" /> class.
        /// </summary>
        /// <param name = "index">The index of the digital input 
        ///   whose value has changed.</param>
        /// <param name = "newValue">The new value of the digital input 
        ///   indicated by the <paramref name = "index" /> parameter.</param>
        public DigitalInputChangedEventArgs(short index, bool newValue)
        {
            Index = index;
            NewValue = newValue;
        }

        /// <summary>
        ///   Gets/sets the index of the digital input whose 
        ///   value has changed.
        /// </summary>
        public short Index { get; set; }

        /// <summary>
        ///   Gets/sets the new value of the indicated digital input.
        /// </summary>
        public bool NewValue { get; set; }
    }
}