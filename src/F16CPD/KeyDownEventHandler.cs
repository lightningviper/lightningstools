using Common.Collections;
using Common.Win32;
using F16CPD.Mfd.Controls;
using F16CPD.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F16CPD
{
    internal interface IKeyDownEventHandler
    {
        void HandleKeyDownEvent(object sender, KeyEventArgs e);
    }
    class KeyDownEventHandler:IKeyDownEventHandler
    {
        private F16CpdMfdManager _manager;
        private SerializableDictionary<CpdInputControls, ControlBinding> _controlBindings;
        public KeyDownEventHandler(SerializableDictionary<CpdInputControls, ControlBinding> controlBindings,F16CpdMfdManager manager)
        {
            _manager = manager;
            _controlBindings = controlBindings;
        }
        public void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (_manager == null || _manager.SimSupportModule == null || !_manager.SimSupportModule.IsSimRunning ||
                _manager.SimSupportModule.IsSendingInput)
            {
                return;
            }
            var keyDown = e.KeyCode;


            if ((NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0)
            {
                keyDown |= Keys.Shift;
                //SHIFT is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0)
            {
                keyDown |= Keys.Control;
                //CONTROL is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_MENU) & 0x8000) != 0)
            {
                keyDown |= Keys.Alt;
                //ALT is pressed
            }

            foreach (var binding in _controlBindings.Values)
            {
                if (binding != null && binding.BindingType == BindingType.Keybinding)
                {
                    if ((binding.Keys & Keys.KeyCode) != Keys.None)
                    {
                        if (
                            ((keyDown & Keys.KeyCode) == (binding.Keys & Keys.KeyCode))
                            &&
                            ((keyDown & Keys.Modifiers) == (binding.Keys & Keys.Modifiers))
                            )
                        {
                            var controlType = binding.CpdInputControl;
                            if (controlType != CpdInputControls.Unknown)
                            {
                                {
                                    _manager.FireHandler(controlType);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
