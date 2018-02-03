using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    ///<summary>
    ///  A <see cref = "ContinuousPovMapping" /> object represents 
    ///  a specific type of Point-of-View (Pov) control that can 
    ///  be defined on a PPJoy joystick <see cref = "Device" />. 
    ///  A <see cref = "ContinuousPovMapping" /> defines the Pov's
    ///  number (index) and the <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> 
    ///  that will provide the <see cref = "ContinuousPovMapping" />'s state values that it will report to Windows.
    ///</summary>
    ///<remarks>
    ///  A <see cref = "ContinuousPovMapping" /> sources its values from a single 
    ///  Analog or Reversed <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see>.
    ///  <para /><i>Contrast this behavior with that of a <see cref = "DirectionalPovMapping" /> control,
    ///            which sources its values from a set of Digital <see cref = "DirectionalPovDataSources" />.</i>
    ///</remarks>
    ///<seealso cref = "ContinuousPovDataSources" />
    ///<seealso cref = "DirectionalPovMapping" />
    ///<seealso cref = "DirectionalPovDataSources" />
    ///<seealso cref = "PovMapping" />
    ///<seealso cref = "Device" />
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class ContinuousPovMapping : PovMapping
    {
        /// <seealso cref = "DataSource" />
        private ContinuousPovDataSources _dataSource = ContinuousPovDataSources.None;

        /// <summary>
        ///   Creates a new <see cref = "ContinuousPovMapping" /> object.
        /// </summary>
        /// <seealso cref = "PovMapping" />
        public ContinuousPovMapping()
        {
        }

        /// <summary>
        ///   Creates a new <see cref = "ContinuousPovMapping" /> object.
        /// </summary>
        /// <param name = "controlNumber">The zero-based index of this <see cref = "PovMapping" />
        ///   in the collection of <see cref = "PovMapping" />s defined on the same PPJoy <see cref = "Device" />.  
        ///   For example, the first <see cref = "PovMapping" /> in the collection 
        ///   will have a <paramref name = "controlNumber " /> of 0,
        ///   the second <see cref = "PovMapping" /> will have a 
        ///   <paramref name = "controlNumber" /> of 1, and so forth.
        /// </param>
        /// <seealso cref = "Mapping.ControlNumber" />
        /// <seealso cref = "PovMapping" />
        public ContinuousPovMapping(int controlNumber)
            : base(controlNumber)
        {
        }

        ///<summary>
        ///  Gets/sets the PPJoy <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> that this <see cref = "PovMapping" /> will use as the source
        ///  of the values that it will report to Windows.
        ///</summary>
        ///<remarks>
        ///  When an <b>Analog </b> <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> is assigned, then, as the 
        ///  <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see>'s value increases, the values reported to 
        ///  Windows by this <see cref = "PovMapping" /> will increase proportionately, proceeding 
        ///  clock-wise from <b>North</b>.
        ///  <para />When a <b>Reversed </b> <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> is assigned, 
        ///  the value of the Reversed <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> itself 
        ///  will <i>decrease</i> as the value of the corresponding 
        ///  Analog <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see>
        ///  <i> increases</i>. This, in turn, means that the value reported 
        ///  to Windows by this <see cref = "PovMapping" /> will 
        ///  <i>decrease</i>, as the value of the corresponding Analog <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see> 
        ///  <i> increases.</i>
        ///  <para /><b>Note:</b> When the assigned <see cref = "ContinuousPovDataSources">ContinuousPovDataSource</see>'s 
        ///  value is set to -1, this <see cref = "PovMapping" /> will report its 
        ///  position as <b>centered</b>.
        ///</remarks>
        ///<seealso cref = "ContinuousPovDataSources" />
        ///<seealso cref = "PovMapping" />
        public ContinuousPovDataSources DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }
    }
}