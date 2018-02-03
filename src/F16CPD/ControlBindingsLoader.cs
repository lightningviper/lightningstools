using Common.Collections;
using Common.InputSupport.DirectInput;
using F16CPD.Mfd.Controls;
using F16CPD.Properties;
using F16CPD.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD
{
    internal interface IControlBindingsLoader {
        SerializableDictionary<CpdInputControls, ControlBinding> LoadControlBindings(Mediator mediator);
    }
    internal class ControlBindingsLoader:IControlBindingsLoader
    {
        public SerializableDictionary<CpdInputControls, ControlBinding> LoadControlBindings(Mediator mediator)
        {
            var controlBindings = new SerializableDictionary<CpdInputControls, ControlBinding>();
            
            foreach (CpdInputControls val in Enum.GetValues(typeof(CpdInputControls)))
            {
                controlBindings.Add(val, new ControlBinding());
            }

            var bindings = Settings.Default.ControlBindings;
            if (!String.IsNullOrEmpty(bindings))
            {
                var deserialized = Common.Serialization.Util.DeserializeFromXml(bindings, controlBindings.GetType());
                if (deserialized != null)
                {
                    var asBindings = (SerializableDictionary<CpdInputControls, ControlBinding>)deserialized;
                    foreach (var entry in asBindings)
                    {
                        var thisEntry = entry.Value;
                        if (thisEntry.BindingType == BindingType.DirectInputAxisBinding ||
                            thisEntry.BindingType == BindingType.DirectInputButtonBinding ||
                            thisEntry.BindingType == BindingType.DirectInputPovBinding)
                        {
                            if (mediator.DeviceMonitors.ContainsKey(thisEntry.DirectInputDevice.Guid))
                            {
                                var thisBinding = controlBindings[entry.Key];
                                thisBinding.BindingType = thisEntry.BindingType;
                                thisBinding.ControlName = thisEntry.ControlName;
                                thisBinding.Keys = thisEntry.Keys;
                                thisBinding.PovDirection = thisEntry.PovDirection;
                                thisBinding.CpdInputControl = thisEntry.CpdInputControl;
                                thisBinding.DirectInputDevice =
                                    mediator.DeviceMonitors[thisEntry.DirectInputDevice.Guid].DeviceInfo;
                                foreach (var control in thisBinding.DirectInputDevice.Controls)
                                {
                                    if (control.ControlNum == thisEntry.DirectInputControl.ControlNum &&
                                        control.ControlType == thisEntry.DirectInputControl.ControlType)
                                    {
                                        thisBinding.DirectInputControl = (DIPhysicalControlInfo)control;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (thisEntry.BindingType == BindingType.Keybinding)
                        {
                            var thisBinding = controlBindings[entry.Key];
                            thisBinding.CpdInputControl = thisEntry.CpdInputControl;
                            thisBinding.BindingType = thisEntry.BindingType;
                            thisBinding.ControlName = thisEntry.ControlName;
                            thisBinding.Keys = thisEntry.Keys;
                        }
                    }
                }
            }
            return controlBindings;
        }
    }
}
