#region Using statements

using System;
using Common.InputSupport;
using PPJoy;

#endregion

namespace JoyMapper
{
    /// <summary>
    ///     Represents a control on a virtual device
    /// </summary>
    [Serializable]
    public sealed class VirtualControlInfo
    {
        /// <summary>
        ///     stores an integer indicating the zero-based index of this control
        ///     in the collection of similar controls on the same PPJoy virtual device
        /// </summary>
        private readonly int _controlNum = -1;

        /// <summary>
        ///     stores a ControlType enumeration value indicating
        ///     what kind of virtual control this is (button or axis)
        /// </summary>
        private readonly ControlType _controlType = ControlType.Unknown;

        private VirtualDeviceInfo _parent;

        /// <summary>
        ///     Creates a new VirtualControlInfo object.
        /// </summary>
        /// <param name="parent">a VirtualDeviceInfo object representing the virtual device to which this control is attached</param>
        /// <param name="controlType">
        ///     a value from the ControlType enumeration, specifying what type of control this object should
        ///     represent (button, axis)
        /// </param>
        /// <param name="controlNum">
        ///     a zero-based index identifying the "offset" of this control in
        ///     the collection of similar virtual controls.  For example, Digital0 would be {controlType= ControlType.Button;
        ///     controlNum=0}; Analog0 would be {controlType=ControlType.Axis; controlNum=0}, etc.
        ///     and the first virtual axis
        /// </param>
        public VirtualControlInfo(VirtualDeviceInfo parent, ControlType controlType, int controlNum)
        {
            //TODO: document this method
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            var PPJoyDeviceNumber = parent.VirtualDeviceNum;
            if (controlType != ControlType.Axis && controlType != ControlType.Button)
            {
                throw new ArgumentOutOfRangeException(nameof(controlType));
            }
            if (PPJoyDeviceNumber <= 0 || PPJoyDeviceNumber > VirtualJoystick.MaxVirtualDevices)
            {
                throw new ArgumentOutOfRangeException(nameof(parent.VirtualDeviceNum),
                    @"Must be in the range 1 - " + VirtualJoystick.MaxVirtualDevices);
            }
            if (controlType == ControlType.Axis)
            {
                if (controlNum < 0 || controlNum >= VirtualJoystick.MaxAnalogDataSources)
                {
                    throw new ArgumentOutOfRangeException(nameof(controlNum),
                        @"Must be in the range 0 - (VirtualJoystick.MaxAnalogAxes-1) when controlType == ControlType.Axis");
                }
            }
            else if (controlType == ControlType.Button)
            {
                if (controlNum < 0 || controlNum >= VirtualJoystick.MaxDigitalDataSources)
                {
                    throw new ArgumentOutOfRangeException(nameof(controlNum),
                        @"Must be in the range 0 - (VirtualJoystick.MaxDigitalButtons-1) when controlType == ControlType.Button");
                }
            }
            _parent = new VirtualDeviceInfo(PPJoyDeviceNumber);
            _controlType = controlType;
            _controlNum = controlNum;
        }

        /// <summary>
        ///     Default constructor is hidden, so class can only be instantiated by calling
        ///     some other constructor.
        /// </summary>
        private VirtualControlInfo()
        {
        }

        /// <summary>
        ///     Gets the zero-based index of this control in the collection of similar controls
        /// </summary>
        public int ControlNum => _controlNum;

        /// <summary>
        ///     Gets a ControlType enumeration value representing the type of this control (axis, button, Pov)
        /// </summary>
        public ControlType ControlType => _controlType;

        /// <summary>
        ///     Gets/sets a VirtualDeviceInfo object representing the physical device on which this control appears.
        /// </summary>
        public VirtualDeviceInfo Parent
        {
            get => _parent;
            set => _parent = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (GetType() != obj.GetType()) return false;

            // safe because of the GetType check
            var vc = (VirtualControlInfo) obj;

            // use this pattern to compare value members
            if (!_controlType.Equals(vc.ControlType)) return false;
            if (!_controlNum.Equals(vc.ControlNum)) return false;
            // use this pattern to compare reference members
            if (!Equals(Parent, vc.Parent)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return "Parent:{" + Parent + "}:ControlType:" + ControlType + ":ControlNum:" + ControlNum;
        }
    }
}