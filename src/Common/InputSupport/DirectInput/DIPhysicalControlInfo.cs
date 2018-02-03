using System;
using Microsoft.DirectX.DirectInput;

namespace Common.InputSupport.DirectInput
{
    [Serializable]
    public class DIPhysicalControlInfo : PhysicalControlInfo
    {
        /// <summary>
        ///     The DirectInput Object ID of this physical control
        /// </summary>
        private readonly int _doiObjectId;

        /// <summary>
        ///     The DirectInput Object Type GUID of this physical control -- indicates its type
        ///     from a DirectInput standpoint
        /// </summary>
        private readonly Guid _doiObjectTypeGuid;

        public DIPhysicalControlInfo()
        {
        }

        /// <summary>
        ///     Creates a new DIPhysicalControlInfo object.
        /// </summary>
        /// <param name="parent">
        ///     a DIPhysicalDeviceInfo object representing
        ///     the physical device on which this control appears
        /// </param>
        /// <param name="deviceObjectInstance">
        ///     this control's DeviceObjectInstance
        ///     structure, as obtained from DirectInput
        /// </param>
        /// <param name="controlNum">
        ///     an integer indicating the relative zero-based
        ///     "offset" of this control in the collection of similar controls on the
        ///     same device.  Button 1 would be controlNum=0; the first slider would
        ///     similarily be controlNum=0; as would the first Pov control, etc.
        /// </param>
        public DIPhysicalControlInfo(DIPhysicalDeviceInfo parent, DeviceObjectInstance deviceObjectInstance,
            int controlNum)
            : base(parent, controlNum, ControlType.Unknown, AxisType.Unknown)
        {
            _doiObjectId = deviceObjectInstance.ObjectId;
            _doiObjectTypeGuid = deviceObjectInstance.ObjectType;
        }

        /// <summary>
        ///     Creates a new DIPhysicalControlInfo object.
        /// </summary>
        /// <param name="parent">
        ///     a DIPhysicalDeviceInfo object representing
        ///     the physical device on which this control appears
        /// </param>
        /// <param name="deviceObjectInstance">
        ///     this control's DeviceObjectInstance
        ///     structure, as obtained from DirectInput
        /// </param>
        /// <param name="controlNum">
        ///     an integer indicating the relative zero-based
        ///     "offset" of this control in the collection of similar controls on the
        ///     same device.  Button 1 would be controlNum=0; the first slider would
        ///     similarily be controlNum=0; as would the first Pov control, etc.
        /// </param>
        /// <param name="alias">
        ///     A string containing a "friendly name" (alias) to
        ///     associate with this control.
        /// </param>
        public DIPhysicalControlInfo(DIPhysicalDeviceInfo parent, DeviceObjectInstance deviceObjectInstance,
            int controlNum, string alias)
            : base(parent, controlNum, ControlType.Unknown, AxisType.Unknown, alias)
        {
            _doiObjectId = deviceObjectInstance.ObjectId;
            _doiObjectTypeGuid = deviceObjectInstance.ObjectType;
        }

        /// <summary>
        ///     Gets the type of axis represented by this control (if the control is
        ///     an axis -- otherwise, returns AxisType.Unknown
        /// </summary>
        public override AxisType AxisType
        {
            get
            {
                if (_axisType != AxisType.Unknown) return _axisType;
                switch (ControlType)
                {
                    case ControlType.Axis:
                        _axisType = _doiObjectTypeGuid == ObjectTypeGuid.XAxis
                            ? AxisType.X
                            : (_doiObjectTypeGuid == ObjectTypeGuid.YAxis
                                ? AxisType.Y
                                : (_doiObjectTypeGuid == ObjectTypeGuid.ZAxis
                                    ? AxisType.Z
                                    : (_doiObjectTypeGuid == ObjectTypeGuid.RxAxis
                                        ? AxisType.XR
                                        : (_doiObjectTypeGuid == ObjectTypeGuid.RyAxis
                                            ? AxisType.YR
                                            : (_doiObjectTypeGuid == ObjectTypeGuid.RzAxis
                                                ? AxisType.ZR
                                                : (_doiObjectTypeGuid == ObjectTypeGuid.Slider
                                                    ? AxisType.Slider
                                                    : AxisType.Unknown))))));
                        break;
                    case ControlType.Pov:
                        _axisType = AxisType.Pov;
                        break;
                    case ControlType.Unknown:
                        break;
                    case ControlType.Button:
                        break;
                    case ControlType.Key:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return _axisType;
            }
            set => _axisType = value;
        }

        /// <summary>
        ///     Gets the type of this control (button, axis, Pov)
        /// </summary>
        public override ControlType ControlType
        {
            get
            {
                if (_controlType != ControlType.Unknown) return _controlType;
                _controlType = _doiObjectTypeGuid == ObjectTypeGuid.Button
                    ? ControlType.Button
                    : (_doiObjectTypeGuid == ObjectTypeGuid.PointOfView
                        ? ControlType.Pov
                        : ((_doiObjectId & (int) DeviceObjectTypeFlags.Axis) != 0
                            ? ControlType.Axis
                            : ControlType.Unknown));
                return _controlType;
            }
            set => _controlType = value;
        }
    }
}