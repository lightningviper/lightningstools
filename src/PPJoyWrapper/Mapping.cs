using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   <see cref = "Mapping" /> is the base class for all <see cref = "Mapping" /> types.
    ///   A <see cref = "Mapping" /> represents a control on 
    ///   a PPJoy Virtual Joystick <see cref = "Device" /> such as a Point-of-View hat,
    ///   an axis, or a button.  <see cref = "Mapping" />s declare the presence of a
    ///   specific control, and its position (index) among other controls of the
    ///   same type on the same <see cref = "Device" />.  <see cref = "Mapping" />s also define
    ///   the data sources that feed these virtual controls state information, which, in turn,
    ///   is reported to Windows and is accessable via DirectInput.
    /// </summary>
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Mapping
    {
        internal int m_controlNumber; //index of this control within a 
        //collection of all other controls 
        //of the same type on the same device
        /// <summary>
        ///   Creates a new Mapping object.
        /// </summary>
        /// <param name = "controlNumber">an index to use for this control in the collection of all other controls of the same type on the same device.</param>
        public Mapping(int controlNumber)
        {
            m_controlNumber = controlNumber;
        }

        /// <summary>
        ///   Creates a new Mapping object.
        /// </summary>
        public Mapping()
        {
        }

        /// <summary>
        ///   Gets/sets the index to use for this control in the collection 
        ///   of all other controls of the same type on the same device.
        /// </summary>
        public int ControlNumber
        {
            get { return m_controlNumber; }
            set { m_controlNumber = value; }
        }
    }
}