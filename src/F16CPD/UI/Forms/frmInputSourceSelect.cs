using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.InputSupport;
using Common.InputSupport.DirectInput;
using Common.Win32;
using F16CPD.Mfd.Controls;

namespace F16CPD.UI.Forms
{
    public enum BindingType
    {
        Unknown = 0,
        Keybinding = 1,
        DirectInputButtonBinding = 2,
        DirectInputPovBinding = 3,
        DirectInputAxisBinding = 4
    }

    public partial class frmInputSourceSelect : Form
    {
        protected KeyEventHandler _keyDownHandler;
        protected Mediator _mediator;
        protected Mediator.PhysicalControlStateChangedEventHandler _mediatorHandler;

        public frmInputSourceSelect()
        {
            InitializeComponent();
            _keyDownHandler = new KeyEventHandler(KeyDownHandler);
            _mediatorHandler =
                new Mediator.PhysicalControlStateChangedEventHandler(_mediator_PhysicalControlStateChanged);
        }

        public Dictionary<CpdInputControls, ControlBinding> ControlBindings { get; set; }
        public CpdInputControls CpdInputControl { get; set; }

        public Mediator Mediator
        {
            get { return _mediator; }
            set
            {
                _mediator = value;
                if (_mediator != null)
                {
                    _mediator.PhysicalControlStateChanged += _mediatorHandler;
                    _mediator.RaiseEvents = true;
                }
            }
        }

        private void _mediator_PhysicalControlStateChanged(object sender, PhysicalControlStateChangedEventArgs e)
        {
            if (e.Control.ControlType == ControlType.Button || e.Control.ControlType == ControlType.Pov)
            {
                rdoJoystick.Checked = true;
                var control = (DIPhysicalControlInfo) e.Control;
                var device = (DIPhysicalDeviceInfo) control.Parent;
                cbJoysticks.SelectedItem = device;
                cboJoystickControl.SelectedItem = control;
                if (control.ControlType == ControlType.Pov)
                {
                    var currentDegrees = e.CurrentState/100.0f;
                    if (e.CurrentState == -1) currentDegrees = -  1;
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
                    if ((currentDegrees > 337.5 && currentDegrees <= 360) ||
                        (currentDegrees >= 0 && currentDegrees <= 22.5))
                    {
                        rdoPovUp.Checked = true;
                    }
                    else if (currentDegrees > 22.5 && currentDegrees <= 67.5)
                    {
                        rdoPovUpRight.Checked = true;
                    }
                    else if (currentDegrees > 67.5 && currentDegrees <= 112.5)
                    {
                        rdoPovRight.Checked = true;
                    }
                    else if (currentDegrees > 112.5 && currentDegrees <= 157.5)
                    {
                        rdoPovDownRight.Checked = true;
                    }
                    else if (currentDegrees > 157.5 && currentDegrees <= 202.5)
                    {
                        rdoPovDown.Checked = true;
                    }
                    else if (currentDegrees > 202.5 && currentDegrees <= 247.5)
                    {
                        rdoPovDownLeft.Checked = true;
                    }
                    else if (currentDegrees > 247.5 && currentDegrees <= 292.5)
                    {
                        rdoPovLeft.Checked = true;
                    }
                    else if (currentDegrees > 292.5 && currentDegrees <= 337.5)
                    {
                        rdoPovUpLeft.Checked = true;
                    }
                }
            }
        }

        private void frmInputSourceSelect_Load(object sender, EventArgs e)
        {
            txtHelpText.Lines = new[]
                                    {
                                        "Press and release the desired " +
                                        "keystroke/combination, or press and release " +
                                        "the desired joystick input, to assign it to this control."
                                    };
            KeyDown += _keyDownHandler;
            PopulateJoysticksComboBox();
            PopulateJoystickControlsComboBox();
            LoadControlBinding();
            EnableDisableControls();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var e = new KeyEventArgs(keyData);
            KeyDownHandler(this, e);
            return e.Handled;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (!ContainsFocus) return;
            if (
                ((e.KeyCode & Keys.KeyCode) == Keys.Tab)
                ||
                ((e.KeyCode & Keys.KeyCode) == Keys.Up)
                ||
                ((e.KeyCode & Keys.KeyCode) == Keys.Down)
                )

            {
                if (rdoKeystroke.Checked)
                {
                    e.Handled = true;
                }
            }
            else
            {
                rdoKeystroke.Checked = true;
                e.Handled = true;
            }
            UpdateKeyAssignmentData(e);
        }

        private void EnableDisableControls()
        {
            rdoJoystick.Enabled = cbJoysticks.Items.Count != 0;

            if (rdoKeystroke.Checked)
            {
                gbPovDirections.Enabled = false;
                txtKeystroke.Enabled = true;
                lblDeviceName.Enabled = false;
                cbJoysticks.Enabled = false;
                lblJoystickControl.Enabled = false;
                cboJoystickControl.Enabled = false;
            }
            else if (rdoJoystick.Checked)
            {
                lblDeviceName.Enabled = true;
                cbJoysticks.Enabled = true;
                lblJoystickControl.Enabled = true;
                cboJoystickControl.Enabled = true;
                txtKeystroke.Enabled = false;

                SelectCurrentJoystick();
                SelectCurrentJoystickControl();

                var control = (DIPhysicalControlInfo) cboJoystickControl.SelectedItem;
                if (control != null)
                {
                    switch (control.ControlType)
                    {
                        case ControlType.Axis:
                            gbPovDirections.Enabled = false;
                            break;
                        case ControlType.Button:
                            gbPovDirections.Enabled = false;
                            break;
                        case ControlType.Pov:
                            gbPovDirections.Enabled = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    cboJoystickControl.SelectedIndex = 0;
                }
            }
            else if (rdoNotAssigned.Checked)
            {
                gbPovDirections.Enabled = false;
                txtKeystroke.Enabled = false;
                lblDeviceName.Enabled = false;
                cbJoysticks.Enabled = false;
                lblJoystickControl.Enabled = false;
                cboJoystickControl.Enabled = false;
            }
        }

        private List<DIPhysicalDeviceInfo> GetKnownDirectInputDevices()
        {
            return
                Mediator.DeviceMonitors.Keys.Select(key => Mediator.DeviceMonitors[key]).Select(
                    monitor => monitor.DeviceInfo).ToList();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            var valid = ValidateSelections();
            if (valid)
            {
                StoreControlBinding();
                Close();
            }
        }

        private bool ValidateSelections()
        {
            var valid = true;
            if (rdoJoystick.Checked)
            {
                var control = (DIPhysicalControlInfo) cboJoystickControl.SelectedItem;
                if (control.ControlType == ControlType.Pov)
                {
                    if (!rdoPovDown.Checked && !rdoPovDownLeft.Checked && !rdoPovDownRight.Checked &&
                        !rdoPovLeft.Checked && !rdoPovRight.Checked && !rdoPovUp.Checked && !rdoPovUpLeft.Checked &&
                        !rdoPovUpRight.Checked)
                    {
                        MessageBox.Show(this,
                                        "A point-of-view (POV) hat control is selected, but no position on the hat has been selected.\nPlease choose a hat position or change the assigned input control.",
                                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        valid = false;
                    }
                }
            }
            else if (rdoKeystroke.Checked)
            {
                if (String.IsNullOrEmpty(txtKeystroke.Text) || txtKeystroke.Text == "None")
                {
                    rdoNotAssigned.Checked = true;
                }
            }
            var thisBinding = GetThisControlBinding();
            foreach (CpdInputControls inputControl in Enum.GetValues(typeof (CpdInputControls)))
            {
                if (inputControl == thisBinding.CpdInputControl || inputControl == CpdInputControls.Unknown) continue;
                var thatControlBinding = ControlBindings[inputControl];

                if (thatControlBinding != null && thatControlBinding.CpdInputControl != CpdInputControls.Unknown &&
                    thatControlBinding.BindingType != BindingType.Unknown &&
                    thatControlBinding.BindingType != BindingType.Unknown)
                {
                    if (thisBinding.BindingType == thatControlBinding.BindingType
                        && Equals(thisBinding.DirectInputControl, thatControlBinding.DirectInputControl)
                        && Equals(thisBinding.DirectInputDevice, thatControlBinding.DirectInputDevice)
                        && thisBinding.Keys == thatControlBinding.Keys
                        && thisBinding.PovDirection == thatControlBinding.PovDirection)
                    {
                        var conflictingControl = thatControlBinding.ControlName;
                        MessageBox.Show(this,
                                        "Another control (" + conflictingControl +
                                        ") is already bound to the input that you selected.", Application.ProductName,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private void StoreControlBinding()
        {
            var thisControlBinding = GetThisControlBinding();
            ControlBindings[thisControlBinding.CpdInputControl] = thisControlBinding;
        }

        private ControlBinding GetThisControlBinding()
        {
            var controlName = ControlBindings[CpdInputControl].ControlName;
            var thisControlBinding = new ControlBinding {CpdInputControl = CpdInputControl, ControlName = controlName};
            if (rdoJoystick.Checked)
            {
                var device = (DIPhysicalDeviceInfo) cbJoysticks.SelectedItem;
                var control = (DIPhysicalControlInfo) cboJoystickControl.SelectedItem;
                switch (control.ControlType)
                {
                    case ControlType.Axis:
                        thisControlBinding.BindingType = BindingType.DirectInputAxisBinding;
                        break;
                    case ControlType.Button:
                        thisControlBinding.BindingType = BindingType.DirectInputButtonBinding;
                        break;
                    case ControlType.Pov:
                        thisControlBinding.BindingType = BindingType.DirectInputPovBinding;
                        if (rdoPovUp.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.Up;
                        }
                        else if (rdoPovUpLeft.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.UpLeft;
                        }
                        else if (rdoPovUpRight.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.UpRight;
                        }
                        else if (rdoPovRight.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.Right;
                        }
                        else if (rdoPovLeft.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.Left;
                        }
                        else if (rdoPovDownLeft.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.DownLeft;
                        }
                        else if (rdoPovDownRight.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.DownRight;
                        }
                        else if (rdoPovDown.Checked)
                        {
                            thisControlBinding.PovDirection = PovDirections.Down;
                        }
                        break;
                    default:
                        break;
                }
                thisControlBinding.DirectInputDevice = device;
                thisControlBinding.DirectInputControl = control;
            }
            else if (rdoKeystroke.Checked)
            {
                thisControlBinding.BindingType = BindingType.Keybinding;
                thisControlBinding.Keys = (Keys) Enum.Parse(typeof (Keys), txtKeystroke.Text);
            }
            return thisControlBinding;
        }

        private void LoadControlBinding()
        {
            var thisControlBinding = ControlBindings[CpdInputControl];
            lblControlName.Text = "Control: " + thisControlBinding.ControlName;
            switch (thisControlBinding.BindingType)
            {
                case BindingType.Keybinding:
                    txtKeystroke.Text = GetKeyName(thisControlBinding.Keys);
                    rdoKeystroke.Checked = true;
                    break;
                case BindingType.DirectInputButtonBinding:
                    cbJoysticks.SelectedItem = thisControlBinding.DirectInputDevice;
                    cboJoystickControl.SelectedItem = thisControlBinding.DirectInputControl;
                    rdoJoystick.Checked = true;
                    break;
                case BindingType.DirectInputPovBinding:
                    ClearAllPovRadioButtons();
                    cbJoysticks.SelectedItem = thisControlBinding.DirectInputDevice;
                    cboJoystickControl.SelectedItem = thisControlBinding.DirectInputControl;
                    switch (thisControlBinding.PovDirection)
                    {
                        case PovDirections.Up:
                            rdoPovUp.Checked = true;
                            break;
                        case PovDirections.UpRight:
                            rdoPovUpRight.Checked = true;
                            break;
                        case PovDirections.Right:
                            rdoPovRight.Checked = true;
                            break;
                        case PovDirections.DownRight:
                            rdoPovDownRight.Checked = true;
                            break;
                        case PovDirections.Down:
                            rdoPovDown.Checked = true;
                            break;
                        case PovDirections.DownLeft:
                            rdoPovDownLeft.Checked = true;
                            break;
                        case PovDirections.Left:
                            rdoPovLeft.Checked = true;
                            break;
                        case PovDirections.UpLeft:
                            rdoPovUpLeft.Checked = true;
                            break;
                        default:
                            break;
                    }
                    rdoJoystick.Checked = true;
                    break;
                case BindingType.DirectInputAxisBinding:
                    cbJoysticks.SelectedItem = thisControlBinding.DirectInputDevice;
                    cboJoystickControl.SelectedItem = thisControlBinding.DirectInputControl;
                    rdoJoystick.Checked = true;
                    break;
                case BindingType.Unknown:
                    rdoNotAssigned.Checked = true;
                    break;
                default:
                    break;
            }
            EnableDisableControls();
        }

        private void ClearAllPovRadioButtons()
        {
            rdoPovUp.Checked = false;
            rdoPovUpLeft.Checked = false;
            rdoPovUpRight.Checked = false;
            rdoPovRight.Checked = false;
            rdoPovDownLeft.Checked = false;
            rdoPovDownRight.Checked = false;
            rdoPovDown.Checked = false;
            rdoPovLeft.Checked = false;
        }

        private void PopulateJoysticksComboBox()
        {
            cbJoysticks.Items.Clear();
            foreach (var pair in Mediator.DeviceMonitors)
            {
                cbJoysticks.Items.Add(pair.Value.DeviceInfo);
            }
            cbJoysticks.DisplayMember = "Alias";
        }

        private void PopulateJoystickControlsComboBox()
        {
            var thisDevice = ControlBindings[CpdInputControl].DirectInputDevice;
            cboJoystickControl.Items.Clear();
            if (thisDevice != null)
            {
                foreach (DIPhysicalControlInfo control in thisDevice.Controls)
                {
                    if (control.ControlType == ControlType.Button || control.ControlType == ControlType.Pov)
                    {
                        cboJoystickControl.Items.Add(control);
                    }
                }
            }
            cboJoystickControl.DisplayMember = "Alias";
        }

        private void SelectCurrentJoystickControl()
        {
            var curControl = ControlBindings[CpdInputControl].DirectInputControl;
            if (curControl != null)
            {
                cboJoystickControl.SelectedItem = curControl;
                if (cboJoystickControl.SelectedIndex < 0) cboJoystickControl.SelectedIndex = 0;
            }
            else
            {
                cboJoystickControl.SelectedIndex = 0;
            }
        }

        private void SelectCurrentJoystick()
        {
            var thisDevice = ControlBindings[CpdInputControl].DirectInputDevice;
            if (thisDevice != null)
            {
                cbJoysticks.SelectedItem = thisDevice;
                if (cbJoysticks.SelectedIndex < 0) cbJoysticks.SelectedIndex = 0;
            }
            else
            {
                cbJoysticks.SelectedIndex = 0;
            }
        }

        private void rdoKeystroke_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
        }

        private void rdoJoystick_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbJoysticks_SelectedIndexChanged(object sender, EventArgs e)
        {
            var thisControlBinding = ControlBindings[CpdInputControl];
            thisControlBinding.DirectInputDevice = (DIPhysicalDeviceInfo) cbJoysticks.SelectedItem;
            PopulateJoystickControlsComboBox();
            SelectCurrentJoystickControl();
        }

        private void cboJoystickControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var thisControlBinding = ControlBindings[CpdInputControl];
            thisControlBinding.DirectInputControl = (DIPhysicalControlInfo) cboJoystickControl.SelectedItem;
            EnableDisableControls();
        }


        private static string GetKeyName(Keys key)
        {
            return key.ToString();
        }

        private void UpdateKeyAssignmentData(KeyEventArgs e)
        {
            var key = e.KeyCode;

            if ((NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0)
            {
                key |= Keys.Shift;
                //SHIFT is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0)
            {
                key |= Keys.Control;
                //CONTROL is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_MENU) & 0x8000) != 0)
            {
                key |= Keys.Alt;
                //ALT is pressed
            }

            txtKeystroke.Text = GetKeyName(key);
        }

        private void frmInputSourceSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyDown -= _keyDownHandler;
            _mediator.PhysicalControlStateChanged -= _mediatorHandler;
        }
    }
}