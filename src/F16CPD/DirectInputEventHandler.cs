using Common.Collections;
using Common.InputSupport;
using Common.InputSupport.DirectInput;
using F16CPD.Mfd.Controls;
using F16CPD.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD
{
    internal interface IDirectInputEventHandler
    {
        void MediatorPhysicalControlStateChanged(object sender, PhysicalControlStateChangedEventArgs e);
    }
    class DirectInputEventHandler:IDirectInputEventHandler
    {
        private SerializableDictionary<CpdInputControls, ControlBinding> _controlBindings;
        private F16CpdMfdManager _manager;
        public DirectInputEventHandler(SerializableDictionary<CpdInputControls, ControlBinding> controlBindings,
            F16CpdMfdManager manager)
        {
            _controlBindings = controlBindings;
            _manager = manager;
        }
        public void MediatorPhysicalControlStateChanged(object sender, PhysicalControlStateChangedEventArgs e)
        {
            if (e == null || e.Control == null || e.Control.Parent == null) return;
            if (e.Control.ControlType == ControlType.Axis || e.Control.ControlType == ControlType.Unknown) return;
            if (_controlBindings != null)
            {
                foreach (var binding in _controlBindings.Values)
                {
                    if (binding.BindingType == BindingType.Unknown || binding.BindingType == BindingType.Keybinding)
                        continue;
                    var control = (DIPhysicalControlInfo)e.Control;
                    var device = (DIPhysicalDeviceInfo)e.Control.Parent;
                    if (binding.DirectInputDevice != null && device.Guid != Guid.Empty &&
                        binding.DirectInputDevice.Guid != Guid.Empty && device.Guid == binding.DirectInputDevice.Guid)
                    {
                        //something on a device we're interested in just changed
                        if (control.Equals(binding.DirectInputControl))
                        {
                            if (binding.BindingType == BindingType.DirectInputButtonBinding &&
                                control.ControlType == ControlType.Button)
                            {
                                if (e.CurrentState == 1 && e.PreviousState != 1)
                                {
                                    //this button was just pressed, so fire the attached event
                                    if (_manager != null) _manager.FireHandler(binding.CpdInputControl);
                                }
                                break;
                            }
                            if (binding.BindingType == BindingType.DirectInputPovBinding &&
                                control.ControlType == ControlType.Pov)
                            {
                                var currentDegrees = e.CurrentState / 100.0f;
                                if (e.CurrentState == -1) currentDegrees = -1;
                                /*  POV directions in degrees
                                          0
                                    337.5  22.5   
                                   315         45
                                 292.5           67.5
                                270                90
                                 247.5           112.5
                                  225          135
                                    202.5  157.5
                                        180
                                 */
                                PovDirections? direction = null;
                                if ((currentDegrees > 337.5 && currentDegrees <= 360) ||
                                    (currentDegrees >= 0 && currentDegrees <= 22.5))
                                {
                                    direction = PovDirections.Up;
                                }
                                else if (currentDegrees > 22.5 && currentDegrees <= 67.5)
                                {
                                    direction = PovDirections.UpRight;
                                }
                                else if (currentDegrees > 67.5 && currentDegrees <= 112.5)
                                {
                                    direction = PovDirections.Right;
                                }
                                else if (currentDegrees > 112.5 && currentDegrees <= 157.5)
                                {
                                    direction = PovDirections.DownRight;
                                }
                                else if (currentDegrees > 157.5 && currentDegrees <= 202.5)
                                {
                                    direction = PovDirections.Down;
                                }
                                else if (currentDegrees > 202.5 && currentDegrees <= 247.5)
                                {
                                    direction = PovDirections.DownLeft;
                                }
                                else if (currentDegrees > 247.5 && currentDegrees <= 292.5)
                                {
                                    direction = PovDirections.Left;
                                }
                                else if (currentDegrees > 292.5 && currentDegrees <= 337.5)
                                {
                                    direction = PovDirections.UpLeft;
                                }

                                if (direction != null && direction == binding.PovDirection)
                                {
                                    if (_manager != null) _manager.FireHandler(binding.CpdInputControl);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

    }
}
