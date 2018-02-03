using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    ///<summary>
    ///  A <see cref = "DirectionalPovMapping" /> object represents 
    ///  a specific type of Point-of-View (Pov) control that can 
    ///  be defined on a PPJoy joystick <see cref = "Device" />. 
    ///  A <see cref = "DirectionalPovMapping" /> defines the Pov's
    ///  number (index) and the <see cref = "DirectionalPovDataSources" />
    ///  that will provide the <see cref = "DirectionalPovMapping" />'s state values 
    ///  that it will report to Windows.
    ///</summary>
    ///<remarks>
    ///  A <see cref = "DirectionalPovMapping" /> sources its values from one or more 
    ///  Digital (or quasi-Digital) <see cref = "DirectionalPovDataSources" />.
    ///  <para /><i>Contrast this behavior with that of a <see cref = "ContinuousPovMapping" />,
    ///            which sources its values from a single Analog or Reversed <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see>.</i>
    ///</remarks>
    ///<seealso cref = "ContinuousPovDataSources" />
    ///<seealso cref = "ContinuousPovMapping" />
    ///<seealso cref = "DirectionalPovDataSources" />
    ///<seealso cref = "PovMapping" />
    ///<seealso cref = "Device" />
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class DirectionalPovMapping : PovMapping
    {
        /// <summary>
        ///   The data source that will cause the EAST direction to be reported
        /// </summary>
        private DirectionalPovDataSources _eastDataSource = DirectionalPovDataSources.None;

        /// <summary>
        ///   The data source that will cause the NORTH direction to be reported
        /// </summary>
        private DirectionalPovDataSources _northDataSource = DirectionalPovDataSources.None;

        /// <summary>
        ///   The data source that will cause the SOUTH direction to be reported
        /// </summary>
        private DirectionalPovDataSources _southDataSource = DirectionalPovDataSources.None;

        /// <summary>
        ///   The data source that will cause the WEST direction to be reported
        /// </summary>
        private DirectionalPovDataSources _westDataSource = DirectionalPovDataSources.None;

        /// <summary>
        ///   Creates a new <see cref = "DirectionalPovMapping" /> object.
        /// </summary>
        /// <seealso cref = "PovMapping" />
        public DirectionalPovMapping()
        {
        }

        /// <summary>
        ///   Creates a new <see cref = "DirectionalPovMapping" /> object.
        /// </summary>
        /// <param name = "controlNumber">The zero-based index of this <see cref = "PovMapping" />
        ///   in the collection of <see cref = "PovMapping" />s defined on the same PPJoy <see cref = "Device" />.  
        ///   For example, the first <see cref = "PovMapping" /> in the collection 
        ///   will have a <paramref name = "controlNumber" /> of 0,
        ///   the second <see cref = "PovMapping" /> will have a 
        ///   <paramref name = "controlNumber" /> of 1, and so forth.
        /// </param>
        /// <seealso cref = "Mapping.ControlNumber" />
        /// <seealso cref = "PovMapping" />
        public DirectionalPovMapping(int controlNumber)
            : base(controlNumber)
        {
        }

        /// <summary>
        ///   Gets/sets the <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see> 
        ///   that this <see cref = "DirectionalPovMapping" /> will use to determine
        ///   that it should report to Windows that it is being pressed 
        ///   in the <b>North</b> direction.
        /// </summary>
        /// <remarks>
        ///   When this property is set to a <b>Digital </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>North</b> direction to Windows 
        ///   whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSources</see>'s value is <see langword = "true" />.  Similarily,
        ///   a value of <b>centered</b> will be reported to Windows, whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is <see langword = "false" /> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Max </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b>  in the <b>North</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>maximum</b> value.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at 
        ///   any other value other than its <b>maximum</b> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Min </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>North</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>minimum</b>.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report 
        ///   a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is at any other value other than its <b>minimum</b> (and
        ///   no other direction is reporting <b>pressed</b>).
        /// </remarks>
        /// <seealso cref = "DirectionalPovDataSources" />
        /// <seealso cref = "DirectionalPovMapping.NorthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.SouthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.EastDataSource" />
        public DirectionalPovDataSources NorthDataSource
        {
            get { return _northDataSource; }
            set { _northDataSource = value; }
        }

        /// <summary>
        ///   Gets/sets the <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see> 
        ///   that this <see cref = "DirectionalPovMapping" /> will use to determine
        ///   that it should report to Windows that it is being pressed 
        ///   in the <b>South</b> direction.
        /// </summary>
        /// <remarks>
        ///   When this property is set to a <b>Digital </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>South</b> direction to Windows 
        ///   whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSources</see>'s value is <see langword = "true" />.  Similarily,
        ///   a value of <b>centered</b> will be reported to Windows, whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is <see langword = "false" /> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Max </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b>  in the <b>South</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>maximum</b> value.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at 
        ///   any other value other than its <b>maximum</b> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Min </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>South</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>minimum</b>.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report 
        ///   a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is at any other value other than its <b>minimum</b> (and
        ///   no other direction is reporting <b>pressed</b>).
        /// </remarks>
        /// <seealso cref = "DirectionalPovDataSources" />
        /// <seealso cref = "DirectionalPovMapping.NorthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.WestDataSource" />
        /// <seealso cref = "DirectionalPovMapping.EastDataSource" />
        public DirectionalPovDataSources SouthDataSource
        {
            get { return _southDataSource; }
            set { _southDataSource = value; }
        }

        /// <summary>
        ///   Gets/sets the <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see> 
        ///   that this <see cref = "DirectionalPovMapping" /> will use to determine
        ///   that it should report to Windows that it is being pressed 
        ///   in the <b>West</b> direction.
        /// </summary>
        /// <remarks>
        ///   When this property is set to a <b>Digital </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>West</b> direction to Windows 
        ///   whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSources</see>'s value is <see langword = "true" />.  Similarily,
        ///   a value of <b>centered</b> will be reported to Windows, whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is <see langword = "false" /> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Max </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b>  in the <b>West</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>maximum</b> value.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at 
        ///   any other value other than its <b>maximum</b> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Min </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>West</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>minimum</b>.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report 
        ///   a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is at any other value other than its <b>minimum</b> (and
        ///   no other direction is reporting <b>pressed</b>).
        /// </remarks>
        /// <seealso cref = "DirectionalPovDataSources" />
        /// <seealso cref = "DirectionalPovMapping.NorthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.SouthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.EastDataSource" />
        public DirectionalPovDataSources WestDataSource
        {
            get { return _westDataSource; }
            set { _westDataSource = value; }
        }

        /// <summary>
        ///   Gets/sets the <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see> 
        ///   that this <see cref = "DirectionalPovMapping" /> will use to determine
        ///   that it should report to Windows that it is being pressed 
        ///   in the <b>East</b> direction.
        /// </summary>
        /// <remarks>
        ///   When this property is set to a <b>Digital </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>East</b> direction to Windows 
        ///   whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSources</see>'s value is <see langword = "true" />.  Similarily,
        ///   a value of <b>centered</b> will be reported to Windows, whenever the Digital <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is <see langword = "false" /> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Max </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b>  in the <b>East</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>maximum</b> value.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at 
        ///   any other value other than its <b>maximum</b> (and no other direction is reporting <b>pressed</b>).
        ///   <para />When this property is set to an <b>Analog-Min </b> <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>, 
        ///   then this <see cref = "DirectionalPovMapping" /> will report a value of <b>pressed</b> in the <b>East</b> direction to Windows 
        ///   whenever the Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s value is at its <b>minimum</b>.  
        ///   Similarily, this <see cref = "DirectionalPovMapping" /> will report 
        ///   a value of <b>centered</b> to Windows whenever the 
        ///   Analog <see cref = "DirectionalPovDataSources">DirectionalPovDataSource</see>'s 
        ///   value is at any other value other than its <b>minimum</b> (and
        ///   no other direction is reporting <b>pressed</b>).
        /// </remarks>
        /// <seealso cref = "DirectionalPovDataSources" />
        /// <seealso cref = "DirectionalPovMapping.NorthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.SouthDataSource" />
        /// <seealso cref = "DirectionalPovMapping.WestDataSource" />
        public DirectionalPovDataSources EastDataSource
        {
            get { return _eastDataSource; }
            set { _eastDataSource = value; }
        }
    }
}