using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   An <see cref = "AxisMapping" /> object represents an axis control that is 
    ///   defined on a PPJoy joystick <see cref = "Device" />.  An <see cref = "AxisMapping" /> describes the 
    ///   axis' number (index), the type of axis control defined, and the PPJoy <see cref = "AxisDataSources">AxisDataSource</see> from which the 
    ///   <see cref = "AxisMapping" /> receives the values which it reports to Windows.
    /// </summary>
    /// <seealso cref = "Device" />
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class AxisMapping : Mapping
    {
        /// <seealso cref = "AxisType" />
        private AxisTypes _axisType = AxisTypes.Unknown;

        /// <seealso cref = "MaxDataSource" />
        private AxisDataSources _maxDataSource = AxisDataSources.None;

        /// <seealso cref = "MinDataSource" />
        private AxisDataSources _minDataSource = AxisDataSources.None;

        /// <summary>
        ///   Creates a new <see cref = "AxisMapping" /> object.
        /// </summary>
        /// <seealso cref = "Device" />
        public AxisMapping()
        {
        }

        /// <summary>
        ///   Creates a new <see cref = "AxisMapping" /> object.
        /// </summary>
        /// <param name = "controlNumber">The zero-based index of this <see cref = "AxisMapping" />
        ///   in the collection of <see cref = "AxisMapping" />s defined on the same PPJoy <see cref = "Device" />.  
        ///   For example, the first <see cref = "AxisMapping" /> in the collection will have a 
        ///   <paramref name = "controlNumber " /> of 0; the second <see cref = "AxisMapping" /> will 
        ///   have a <paramref name = "controlNumber" /> of 1; and so forth.
        /// </param>
        /// <seealso cref = "Mapping.ControlNumber" />
        /// <seealso cref = "Device" />
        public AxisMapping(int controlNumber)
            : base(controlNumber)
        {
        }

        /// <summary>
        ///   Gets/sets the type of Windows axis that this <see cref = "AxisMapping" /> will report
        ///   itself as.
        /// </summary>
        /// <seealso cref = "AxisTypes" />
        public AxisTypes AxisType
        {
            get { return _axisType; }
            set { _axisType = value; }
        }

        ///<summary>
        ///  Gets/sets the PPJoy <see cref = "AxisDataSources">AxisDataSource</see> that this <see cref = "AxisMapping" /> 
        ///  will use as the source of the values that it will report to Windows.
        ///</summary>
        ///<remarks>
        ///  <para />When the <see cref = "MinDataSource" /> property is set to 
        ///  a Digital <see cref = "AxisDataSources">AxisDataSource</see>, 
        ///  then this <see cref = "AxisMapping" /> will report its <b>minimum</b>
        ///  value to Windows whenever the Digital <see cref = "AxisDataSources">AxisDataSource</see>'s value is 
        ///  <see langword = "true" />.
        ///  <para />When this property is set to an Analog or Reversed <see cref = "AxisDataSources">AxisDataSource</see>,
        ///  then this <see cref = "AxisMapping" />  will report the value 
        ///  provided by the Analog or Reversed <see cref = "AxisDataSources">AxisDataSource</see> assigned to 
        ///  the <see cref = "MinDataSource" /> property, and will ignore any <see cref = "AxisDataSources">AxisDataSource</see> set in 
        ///  the <see cref = "MaxDataSource" /> property.
        ///  <para />
        ///  <para />
        ///  <hr />
        ///  <b>Details:</b>
        ///  PPJoy <see cref = "AxisMapping" />s can be driven from either Digital or Analog <see cref = "AxisDataSources" />.  
        ///  <para />
        ///  <para />
        ///  <b>Digital <see cref = "AxisDataSources" />:</b>
        ///  <para />
        ///  If an <see cref = "AxisMapping" /> is driven by a (pair of) <b>Digital</b> <see cref = "AxisDataSources" />, 
        ///  then the <see cref = "AxisMapping" /> can only report one of two possible values -- 
        ///  <b>Minimum</b> and <b>Maximum</b>.
        ///  How this works is as follows:<para />
        ///  If the value of the <b>Digital</b> data source assigned 
        ///  to the <see cref = "AxisMapping" />'s
        ///  <see cref = "MinDataSource" /> property is <see langword = "true" />, 
        ///  then the <see cref = "AxisMapping" /> will report its value as being the <b>minimum</b>
        ///  axis value.  
        ///  <para />
        ///  Alternatively, if the value of the <b>Digital</b> data source
        ///  assigned to the <see cref = "AxisMapping" />'s <b><see cref = "MaxDataSource" /></b> property 
        ///  is <see langword = "true" />, then the <see cref = "AxisMapping" /> will 
        ///  report its value as being the <b>maximum</b> axis value.  
        /// 
        ///  If <b>both</b> the <see cref = "MinDataSource" /> and the <b><see cref = "MaxDataSource" /></b>'s 
        ///  values read the same value (either <see langword = "true" /> or <see langword = "false" />, the
        ///  behavior is undefined.
        ///  <para />
        ///  <b>Note:</b> If the <see cref = "MinDataSource" /> property is set to a <b>Digital</b> <see cref = "AxisDataSources">AxisDataSource</see>, then 
        ///  the <see cref = "MaxDataSource" /> property should also be set to a <b>Digital</b> <see cref = "AxisDataSources">AxisDataSource</see>.  You cannot
        ///  set one data source to Digital and the other to Analog, nor should you set the <see cref = "MinDataSource" /> property without also setting 
        ///  the <see cref = "MaxDataSource" /> property.
        ///  <para />
        ///  <b>Analog/Reversed <see cref = "AxisDataSources">AxisDataSource</see>:</b>
        ///  <para />
        ///  If an <see cref = "AxisMapping" /> is driven by an <b>Analog</b> or 
        ///  <b>Reversed</b> <see cref = "AxisDataSources">AxisDataSource</see>, 
        ///  then the <see cref = "AxisMapping" /> will report its 
        ///  value to Windows, based on the value of the underlying Analog or 
        ///  Reversed <see cref = "AxisDataSources">AxisDataSource</see> 
        ///  which is assigned to the <see cref = "AxisMapping" />'s
        ///  <see cref = "MinDataSource" /> property.  
        ///  <para />
        ///  For Analog <see cref = "AxisDataSources">AxisDataSource</see>s, when the 
        ///  value of the underlying <see cref = "AxisDataSources">AxisDataSource</see> increases, 
        ///  the value reported by the <see cref = "AxisMapping" /> to Windows will increase proportionately.  
        ///  <para />
        ///  For <b>Reversed</b> <see cref = "AxisDataSources">AxisDataSources</see>, 
        ///  when the value of the underlying <see cref = "AxisDataSources">AxisDataSource</see> 
        ///  <i>decreases</i>, the value reported by the <see cref = "AxisMapping" /> 
        ///  to Windows will <i>increase</i> proportionately.
        ///  <para />
        ///  <b>Note:</b> If the <see cref = "MinDataSource" /> property is set to an 
        ///  <b>Analog</b> <see cref = "AxisDataSources">AxisDataSource</see> or 
        ///  a <b>Reversed</b> <see cref = "AxisDataSources">AxisDataSource</see>, 
        ///  then the value of the <see cref = "MaxDataSource" /> property 
        ///  will be ignored.
        ///</remarks>
        ///<seealso cref = "MaxDataSource" />
        ///|
        ///<seealso cref = "AxisDataSources" />
        public AxisDataSources MinDataSource
        {
            get { return _minDataSource; }
            set { _minDataSource = value; }
        }

        /// <summary>
        ///   Gets/sets the PPJoy <see cref = "AxisDataSources">AxisDataSource</see> that 
        ///   this <see cref = "AxisMapping" /> will use as the source of the values that it will report to Windows.
        /// </summary>
        /// <remarks>
        ///   When this property is set to a Digital <see cref = "AxisDataSources">AxisDataSource</see>, 
        ///   then this <see cref = "AxisMapping" /> will report its 
        ///   <b>maximum</b> value to Windows whenever the Digital <see cref = "AxisDataSources">AxisDataSource</see>'s value is 
        ///   <see langword = "true" />.
        ///   <para />To use the <see cref = "MaxDataSource" /> property, you must first 
        ///   set the <see cref = "MinDataSource" /> property to a 
        ///   Digital (boolean) <see cref = "AxisDataSources">AxisDataSource</see>.
        ///   <para /><b>Note:</b> If the <see cref = "MinDataSource" /> property is not set to a 
        ///   Digital (boolean) <see cref = "AxisDataSources">AxisDataSource</see>, then 
        ///   setting the <see cref = "MaxDataSource" /> property has no effect.
        /// </remarks>
        /// <seealso cref = "MinDataSource" />
        /// <seealso cref = "AxisDataSources" />
        public AxisDataSources MaxDataSource
        {
            get { return _maxDataSource; }
            set { _maxDataSource = value; }
        }
    }
}