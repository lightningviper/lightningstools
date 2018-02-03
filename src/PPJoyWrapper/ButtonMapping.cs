using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   A <see cref = "ButtonMapping" /> object represents a button control that is defined 
    ///   on a PPJoy joystick <see cref = "Device" />.  A <see cref = "ButtonMapping" /> defines the button's
    ///   number (index) and the <see cref = "ButtonDataSources">ButtonDataSource</see> that will provide the <see cref = "ButtonMapping" />'s state values
    ///   that it will report to Windows.
    /// </summary>
    /// <seealso cref = "Device" />
    /// <seealso cref = "Mapping" />
    /// <seealso cref = "ButtonDataSources" />
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class ButtonMapping : Mapping
    {
        /// <seealso cref = "DataSource" />
        private ButtonDataSources _dataSource = ButtonDataSources.None;

        /// <summary>
        ///   Creates a new <see cref = "ButtonMapping" /> object.
        /// </summary>
        /// <seealso cref = "Device" />
        public ButtonMapping()
        {
        }

        /// <summary>
        ///   Creates a new <see cref = "ButtonMapping" /> object.
        /// </summary>
        /// <param name = "controlNumber">The zero-based index of this <see cref = "ButtonMapping" />  
        ///   in the collection of <see cref = "ButtonMapping" />s defined on the same PPJoy <see cref = "Device" />.  For example, 
        ///   the first <see cref = "ButtonMapping" /> in the collection will have a <paramref name = "controlNumber " /> of 0,
        ///   the second <see cref = "ButtonMapping" /> will have a <paramref name = "controlNumber" /> of 1, and so forth.
        /// </param>
        /// <seealso cref = "Mapping.ControlNumber" />
        /// <seealso cref = "Device" />
        public ButtonMapping(int controlNumber)
            : base(controlNumber)
        {
        }

        ///<summary>
        ///  Gets/sets the PPJoy <see cref = "ButtonDataSources">ButtonDataSource</see> that this <see cref = "ButtonMapping" /> will use as the source
        ///  of the values that it will report to Windows.
        ///</summary>
        ///<remarks>
        ///  When this property is set to a <b>Digital </b> <see cref = "ButtonDataSources">ButtonDataSource</see>, 
        ///  then this <see cref = "ButtonMapping" /> will report a value of <b>pressed</b> to Windows 
        ///  whenever the Digital <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is <see langword = "true" />.  Similarily,
        ///  a value of <b>unpressed</b> will be reported, whenever the Digital <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is <see langword = "false" />.
        ///  <para />When this property is set to an <b>Analog-Max </b> <see cref = "ButtonDataSources">ButtonDataSource</see>, 
        ///  then this <see cref = "ButtonMapping" /> will report a value of <b>pressed</b> to Windows 
        ///  whenever the Analog <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is at its <b>maximum</b> value.  Similarily,
        ///  this <see cref = "ButtonMapping" /> will report a value of <b>unpressed</b> to Windows whenever the Analog <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is at 
        ///  any other value other than its <b>maximum</b>.
        ///  <para />When this property is set to an <b>Analog-Min </b> <see cref = "ButtonDataSources">ButtonDataSource</see>, 
        ///  then this <see cref = "ButtonMapping" /> will report a value of <b>pressed</b> to Windows 
        ///  whenever the Analog <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is at its <b>minimum</b>.  Similarily, 
        ///  this <see cref = "ButtonMapping" /> will report a value of <b>unpressed</b> to Windows whenever the Analog <see cref = "ButtonDataSources">ButtonDataSource</see>'s value is at 
        ///  any other value other than its <b>minimum</b>.
        ///</remarks>
        ///<seealso cref = "ButtonDataSources" />
        ///<seealso cref = "ButtonMapping" />
        ///<seealso cref = "Mapping" />
        ///<seealso cref = "Device" />
        public ButtonDataSources DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }
    }
}