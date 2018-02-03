using System;
using System.Collections.Generic;
using log4net;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace Common.InputSupport.DirectInput
{
    /// <summary>
    ///     Represents a specific physical DirectInput input device (gaming device)
    ///     such as a joystick, gaming wheel, etc.
    /// </summary>
    [Serializable]
    public class DIPhysicalDeviceInfo : PhysicalDeviceInfo
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DIPhysicalDeviceInfo));
        private Guid _guid = Guid.Empty;

        /// <summary>
        ///     Constructs a DIPhysicalDeviceInfo, given a DirectInput
        ///     Device Instance GUID and an (optional) alias ("Friendly name")
        ///     to use for the device
        /// </summary>
        /// <param name="guid">
        ///     a GUID containing the DirectInput
        ///     Device Instance GUID of the physical input device to be
        ///     represented by the newly-created object
        /// </param>
        /// <param name="alias">
        ///     a string containing a "friendly name" (alias)
        ///     to associate with the device being represented
        /// </param>
        public DIPhysicalDeviceInfo(Guid guid, string alias) : base(guid, alias)
        {
            _guid = guid;
        }

        /// <summary>
        /// </summary>
        private DIPhysicalDeviceInfo()
        {
        }

        public int DeviceNum { get; set; }

        public Guid Guid
        {
            get => _guid;
            set => _guid = value;
        }

        /// <summary>
        ///     Discovers the physical controls that appear on this device,
        ///     as reported by DirectInput, and stores them as an array
        ///     of PhysicalControlInfo objects at the instance level.
        ///     NOT guaranteed to be successful -- if the calls to
        ///     DirectInput fail or if the device
        ///     is not currently registered, then the controls list will remain
        ///     unpopulated.
        /// </summary>
        protected override void LoadControls()
        {
            if (ControlsLoaded)
            {
                return;
            }
            try
            {
                if (!Manager.GetDeviceAttached(new Guid(Key.ToString())))
                {
                    return;
                }
            }
            catch (OutOfMemoryException e)
            {
                _log.Debug(e.Message, e);
                return;
            }
            catch (NullReferenceException e)
            {
                _log.Debug(e.Message, e);
            }
            catch (DirectXException e)
            {
                _log.Debug(e.Message, e);
                return;
            }
            catch (AccessViolationException e2)
            {
                _log.Debug(e2.Message, e2);
                return;
            }
            var controls = new List<PhysicalControlInfo>();
            var joystick = Util.GetDIDevice(new Guid(Key.ToString()));
            if (joystick == null)
            {
                return;
            }

            var dol = joystick.GetObjects(DeviceObjectTypeFlags.Axis);
            var lastSlider = -1;
            var lastAxis = -1;
            foreach (DeviceObjectInstance doi in dol)
                if (doi.ObjectType == ObjectTypeGuid.Slider)
                {
                    lastSlider++;
                    PhysicalControlInfo control = new DIPhysicalControlInfo(this, doi, lastSlider,
                        "Slider " + (lastSlider + 1));
                    controls.Add(control);
                }
                else if ((doi.ObjectId & (int) DeviceObjectTypeFlags.Axis) != 0)
                {
                    if (!(doi.ObjectType == ObjectTypeGuid.XAxis ||
                          doi.ObjectType == ObjectTypeGuid.YAxis ||
                          doi.ObjectType == ObjectTypeGuid.ZAxis ||
                          doi.ObjectType == ObjectTypeGuid.RxAxis ||
                          doi.ObjectType == ObjectTypeGuid.RyAxis ||
                          doi.ObjectType == ObjectTypeGuid.RzAxis))
                    {
                        continue;
                    }
                    lastAxis++;
                    var axisName = "Unknown";
                    if (doi.ObjectType == ObjectTypeGuid.XAxis)
                    {
                        axisName = "X Axis";
                    }
                    else if (doi.ObjectType == ObjectTypeGuid.YAxis)
                    {
                        axisName = "Y Axis";
                    }
                    else if (doi.ObjectType == ObjectTypeGuid.ZAxis)
                    {
                        axisName = "Z Axis";
                    }
                    else if (doi.ObjectType == ObjectTypeGuid.RxAxis)
                    {
                        axisName = "X Rotation Axis";
                    }
                    else if (doi.ObjectType == ObjectTypeGuid.RyAxis)
                    {
                        axisName = "Y Rotation Axis";
                    }
                    else if (doi.ObjectType == ObjectTypeGuid.RzAxis)
                    {
                        axisName = "Z Rotation Axis";
                    }

                    PhysicalControlInfo control = new DIPhysicalControlInfo(this, doi, lastAxis, axisName);
                    controls.Add(control);
                }
            var lastButton = -1;
            dol = joystick.GetObjects(DeviceObjectTypeFlags.Button);
            foreach (DeviceObjectInstance doi in dol)
            {
                lastButton++;
                PhysicalControlInfo control = new DIPhysicalControlInfo(this, doi, lastButton,
                    "Button " + (lastButton + 1));
                controls.Add(control);
            }

            var lastPov = -1;
            dol = joystick.GetObjects(DeviceObjectTypeFlags.Pov);
            foreach (DeviceObjectInstance doi in dol)
            {
                lastPov++;
                PhysicalControlInfo control = new DIPhysicalControlInfo(this, doi, lastPov, "Hat " + (lastPov + 1));
                controls.Add(control);
            }
            _controls = new PhysicalControlInfo[controls.Count];
            var thisControlIndex = 0;
            foreach (var control in controls)
            {
                _controls[thisControlIndex] = control;
                thisControlIndex++;
            }
            ControlsLoaded = true;
        }
    }
}