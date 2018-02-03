using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   <see cref = "PovMapping" /> is the base class for all <see cref = "PovMapping" /> types.  
    ///   A <see cref = "PovMapping" /> is a type of <see cref = "Mapping" /> that declares and
    ///   defines a Point-of-View control on a PPJoy Virtual Joystick <see cref = "Device" />.
    /// </summary>
    /// <seealso cref = "Mapping" />
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PovMapping : Mapping
    {
        /// <summary>
        ///   Creates a new <see cref = "PovMapping" /> object.
        /// </summary>
        /// <seealso cref = "Mapping" />
        public PovMapping()
        {
        }

        /// <summary>
        ///   Creates a new <see cref = "PovMapping" /> object.
        /// </summary>
        /// <param name = "controlNumber">The zero-based index of this <see cref = "PovMapping" /> 
        ///   in the collection of <see cref = "PovMapping" />s defined on a single PPJoy <see cref = "Device" />.  
        ///   For example, the first <see cref = "PovMapping" /> in the collection will have a <paramref name = "controlNumber " /> of 0,
        ///   the second <see cref = "PovMapping" /> will have a <paramref name = "controlNumber" /> of 1, and so forth.
        /// </param>
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "Mapping.ControlNumber" />
        public PovMapping(int controlNumber)
            : base(controlNumber)
        {
        }
    }
}