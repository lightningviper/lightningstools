using System;
using System.Runtime.InteropServices;

namespace Phcc
{
    /// <summary>
    ///   <see cref = "AnalogInputChangedEventArgs" /> objects hold data that the PHCC motherboard 
    ///   provides whenever an analog input 
    ///   value has changed.  The <see cref = "Device.AnalogInputChanged" /> event  
    ///   provides <see cref = "AnalogInputChangedEventArgs" /> event-args 
    ///   objects during the raising of each event.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class AnalogInputChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Creates an instance of the <see cref = "AnalogInputChangedEventArgs" /> class.
        /// </summary>
        public AnalogInputChangedEventArgs()
        {
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "AnalogInputChangedEventArgs" /> class.
        /// </summary>
        /// <param name = "index">The index of the analog input whose 
        ///   value has changed.
        ///   The first 3 analog input indexes represent the prioritized 
        ///   analog inputs (ANP1-ANP3); the remaining 32 analog 
        ///   input indexes represent the standard 
        ///   non-prioritized analog inputs (AN1-AN32).</param>
        /// <param name = "newValue">The new value of the 
        ///   indicated analog input.</param>
        public AnalogInputChangedEventArgs(byte index, short newValue)
        {
            Index = index;
            NewValue = newValue;
        }

        /// <summary>
        ///   Gets/sets the index of the analog input whose 
        ///   value has changed.  
        ///   The first 3 analog input indexes represent the 
        ///   prioritized analog inputs (ANP1-ANP3); the remaining 
        ///   32 analog input indexes represent the standard analog
        ///   non-prioritized inputs (AN1-AN32).
        /// </summary>
        public byte Index { get; set; }

        /// <summary>
        ///   Gets/sets the new value of the indicated analog input.
        /// </summary>
        public short NewValue { get; set; }
    }
}