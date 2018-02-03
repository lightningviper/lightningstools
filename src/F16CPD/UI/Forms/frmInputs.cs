using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Collections;
using Common.InputSupport.DirectInput;
using F16CPD.Mfd.Controls;
using F16CPD.Properties;

namespace F16CPD.UI.Forms
{
    //TODO: provide print capability for input assignment
    public partial class frmInputs : Form
    {
        protected SerializableDictionary<CpdInputControls, ControlBinding> _controlBindings =
            new SerializableDictionary<CpdInputControls, ControlBinding>();

        public frmInputs()
        {
            InitializeComponent();
            InitializeControlBindings();
            Mediator = new Mediator(this) {RaiseEvents = true};
            LoadControlBindings();
        }

        public Mediator Mediator { get; set; }

        public SerializableDictionary<CpdInputControls, ControlBinding> ControlBindings
        {
            get { return _controlBindings; }
        }

        private void InitializeControlBindings()
        {
            if (_controlBindings == null)
            {
                _controlBindings = new SerializableDictionary<CpdInputControls, ControlBinding>();
            }
            _controlBindings.Clear();
            foreach (CpdInputControls val in Enum.GetValues(typeof (CpdInputControls)))
            {
                _controlBindings.Add(val, new ControlBinding());
            }
        }

        private void pbCpdBezel_Click(object sender, EventArgs e)
        {
            var args = (MouseEventArgs) e;
            if (args.Button != MouseButtons.Left) return;
            var location = ((MouseEventArgs) e).Location;
            var control = DetectClickedControl(location);
            if (control != CpdInputControls.Unknown)
            {
                if (control == CpdInputControls.HsiModeControl || control == CpdInputControls.ExtFuelTransSwitch ||
                    control == CpdInputControls.FuelSelectControl || control == CpdInputControls.ParameterAdjustKnob)
                {
                    var radioButtonItems = new List<string>();
                    switch (control)
                    {
                        case CpdInputControls.HsiModeControl:
                            radioButtonItems.Add(GetControlName(CpdInputControls.HsiModeIlsTcn));
                            radioButtonItems.Add(GetControlName(CpdInputControls.HsiModeTcn));
                            radioButtonItems.Add(GetControlName(CpdInputControls.HsiModeNav));
                            radioButtonItems.Add(GetControlName(CpdInputControls.HsiModeIlsNav));
                            break;
                        case CpdInputControls.ExtFuelTransSwitch:
                            radioButtonItems.Add(GetControlName(CpdInputControls.ExtFuelSwitchTransNorm));
                            radioButtonItems.Add(GetControlName(CpdInputControls.ExtFuelSwitchTransWingFirst));
                            break;
                        case CpdInputControls.FuelSelectControl:
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectTest));
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectNorm));
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectRsvr));
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectIntWing));
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectExtWing));
                            radioButtonItems.Add(GetControlName(CpdInputControls.FuelSelectExtCtr));
                            break;
                        case CpdInputControls.ParameterAdjustKnob:
                            radioButtonItems.Add(GetControlName(CpdInputControls.ParameterAdjustKnobIncrease));
                            radioButtonItems.Add(GetControlName(CpdInputControls.ParameterAdjustKnobDecrease));
                            break;
                    }
                    using (var drillDown = new frmInputDrilldown())
                    {
                        var formLocation = drillDown.Location;
                        formLocation.X = Location.X;
                        formLocation.Y = Location.Y + (int) (Height/4.0f);

                        drillDown.Location = formLocation;
                        drillDown.RadioButtonItems = radioButtonItems;
                        var result = drillDown.ShowDialog(this);
                        if (result != DialogResult.Cancel)
                        {
                            var selectedItem = drillDown.SelectedRadioButtonItem;
                            control = !String.IsNullOrEmpty(selectedItem)
                                          ? GetControlByControlName(selectedItem)
                                          : CpdInputControls.Unknown;
                        }
                        else
                        {
                            control = CpdInputControls.Unknown;
                        }
                    }
                }

                if (control != CpdInputControls.Unknown)
                {
                    using (var sourceSelectForm = new frmInputSourceSelect())
                    {
                        var thisControlBinding = ControlBindings[control];
                        thisControlBinding.CpdInputControl = control;
                        thisControlBinding.ControlName = GetControlName(thisControlBinding.CpdInputControl);
                        sourceSelectForm.Mediator = Mediator;
                        sourceSelectForm.ControlBindings =
                            (Dictionary<CpdInputControls, ControlBinding>) ((ICloneable) ControlBindings).Clone();
                        sourceSelectForm.CpdInputControl = control;
                        sourceSelectForm.ShowDialog(this);
                        ControlBindings[control] = sourceSelectForm.ControlBindings[control];
                    }
                }
            }
        }

        private static CpdInputControls GetControlByControlName(string controlName)
        {
            var toReturn = CpdInputControls.Unknown;
            if (!String.IsNullOrEmpty(controlName))
            {
                toReturn =
                    Enum.GetValues(typeof (CpdInputControls)).Cast<CpdInputControls>().FirstOrDefault(
                        val => GetControlName(val) == controlName);
            }
            return toReturn;
        }

        private static string GetControlName(CpdInputControls control)
        {
            string toReturn = null;
            switch (control)
            {
                case CpdInputControls.Unknown:
                    toReturn = "Unknown";
                    break;
                case CpdInputControls.OsbButton1:
                    toReturn = "Option Select Button 1";
                    break;
                case CpdInputControls.OsbButton2:
                    toReturn = "Option Select Button 2";
                    break;
                case CpdInputControls.OsbButton3:
                    toReturn = "Option Select Button 3";
                    break;
                case CpdInputControls.OsbButton4:
                    toReturn = "Option Select Button 4";
                    break;
                case CpdInputControls.OsbButton5:
                    toReturn = "Option Select Button 5";
                    break;
                case CpdInputControls.OsbButton6:
                    toReturn = "Option Select Button 6";
                    break;
                case CpdInputControls.OsbButton7:
                    toReturn = "Option Select Button 7";
                    break;
                case CpdInputControls.OsbButton8:
                    toReturn = "Option Select Button 8";
                    break;
                case CpdInputControls.OsbButton9:
                    toReturn = "Option Select Button 9";
                    break;
                case CpdInputControls.OsbButton10:
                    toReturn = "Option Select Button 10";
                    break;
                case CpdInputControls.OsbButton11:
                    toReturn = "Option Select Button 11";
                    break;
                case CpdInputControls.OsbButton12:
                    toReturn = "Option Select Button 12";
                    break;
                case CpdInputControls.OsbButton13:
                    toReturn = "Option Select Button 13";
                    break;
                case CpdInputControls.OsbButton14:
                    toReturn = "Option Select Button 14";
                    break;
                case CpdInputControls.OsbButton15:
                    toReturn = "Option Select Button 15";
                    break;
                case CpdInputControls.OsbButton16:
                    toReturn = "Option Select Button 16";
                    break;
                case CpdInputControls.OsbButton17:
                    toReturn = "Option Select Button 17";
                    break;
                case CpdInputControls.OsbButton18:
                    toReturn = "Option Select Button 18";
                    break;
                case CpdInputControls.OsbButton19:
                    toReturn = "Option Select Button 19";
                    break;
                case CpdInputControls.OsbButton20:
                    toReturn = "Option Select Button 20";
                    break;
                case CpdInputControls.OsbButton21:
                    toReturn = "Option Select Button 21";
                    break;
                case CpdInputControls.OsbButton22:
                    toReturn = "Option Select Button 22";
                    break;
                case CpdInputControls.OsbButton23:
                    toReturn = "Option Select Button 23";
                    break;
                case CpdInputControls.OsbButton24:
                    toReturn = "Option Select Button 24";
                    break;
                case CpdInputControls.OsbButton25:
                    toReturn = "Option Select Button 25";
                    break;
                case CpdInputControls.OsbButton26:
                    toReturn = "Option Select Button 26";
                    break;
                case CpdInputControls.HsiModeControl:
                    toReturn = "HSI Mode Selector";
                    break;
                case CpdInputControls.HsiModeTcn:
                    toReturn = "HSI Mode Selector - TCN";
                    break;
                case CpdInputControls.HsiModeIlsTcn:
                    toReturn = "HSI Mode Selector - ILS/TCN";
                    break;
                case CpdInputControls.HsiModeNav:
                    toReturn = "HSI Mode Selector - NAV";
                    break;
                case CpdInputControls.HsiModeIlsNav:
                    toReturn = "HSI Mode Selector - ILS/NAV";
                    break;
                case CpdInputControls.ParameterAdjustKnob:
                    toReturn = "Parameter Adjust Knob";
                    break;
                case CpdInputControls.ParameterAdjustKnobIncrease:
                    toReturn = "Parameter Adjust Knob - Increase";
                    break;
                case CpdInputControls.ParameterAdjustKnobDecrease:
                    toReturn = "Parameter Adjust Knob - Decrease";
                    break;
                case CpdInputControls.FuelSelectControl:
                    toReturn = "Fuel Quantity Selector";
                    break;
                case CpdInputControls.FuelSelectTest:
                    toReturn = "Fuel Quantity Selector - TEST";
                    break;
                case CpdInputControls.FuelSelectNorm:
                    toReturn = "Fuel Quantity Selector - NORM";
                    break;
                case CpdInputControls.FuelSelectRsvr:
                    toReturn = "Fuel Quantity Selector - RSVR";
                    break;
                case CpdInputControls.FuelSelectIntWing:
                    toReturn = "Fuel Quantity Selector - INT WING";
                    break;
                case CpdInputControls.FuelSelectExtWing:
                    toReturn = "Fuel Quantity Selector - EXT WING";
                    break;
                case CpdInputControls.FuelSelectExtCtr:
                    toReturn = "Fuel Quantity Selector - EXT CTR";
                    break;
                case CpdInputControls.ExtFuelTransSwitch:
                    toReturn = "External Fuel Transfer Mode Switch";
                    break;
                case CpdInputControls.ExtFuelSwitchTransNorm:
                    toReturn = "External Fuel Transfer Mode Switch - NORM";
                    break;
                case CpdInputControls.ExtFuelSwitchTransWingFirst:
                    toReturn = "External Fuel Transfer Mode Switch - WING FIRST";
                    break;
                default:
                    break;
            }
            return toReturn;
        }

        private CpdInputControls DetectClickedControl(Point clickedPoint)
        {
            var scaleX = (pbCpdBezel.Width/345.0f);
            var scaleY = (pbCpdBezel.Height/557.0f);
            clickedPoint = new Point((int) (clickedPoint.X/scaleX), (int) (clickedPoint.Y/scaleY));

            //TOP ROW BUTTONS
            var osbButton1Rect = new Rectangle(55, 0, 97 - 55, 45);
            var osbButton2Rect = new Rectangle(104, 0, 146 - 104, 45);
            var osbButton3Rect = new Rectangle(153, 0, 193 - 153, 45);
            var osbButton4Rect = new Rectangle(200, 0, 245 - 200, 45);
            var osbButton5Rect = new Rectangle(252, 0, 292 - 52, 45);

            //RHS BUTTONS
            var osbButton6Rect = new Rectangle(302, 41, 345 - 302, 81 - 41);
            var osbButton7Rect = new Rectangle(302, 88, 345 - 302, 129 - 88);
            var osbButton8Rect = new Rectangle(302, 135, 345 - 302, 176 - 135);
            var osbButton9Rect = new Rectangle(302, 181, 345 - 302, 221 - 181);
            var osbButton10Rect = new Rectangle(302, 228, 345 - 302, 267 - 228);
            var osbButton11Rect = new Rectangle(302, 276, 345 - 302, 314 - 276);
            var osbButton12Rect = new Rectangle(302, 322, 345 - 302, 362 - 322);
            var osbButton13Rect = new Rectangle(302, 370, 345 - 302, 409 - 370);

            //BOTTOM ROW BUTTONS
            var osbButton14Rect = new Rectangle(252, 401, 292 - 52, 444 - 401);
            var osbButton15Rect = new Rectangle(200, 401, 245 - 200, 444 - 401);
            var osbButton16Rect = new Rectangle(153, 401, 193 - 153, 444 - 401);
            var osbButton17Rect = new Rectangle(104, 401, 146 - 104, 444 - 401);
            var osbButton18Rect = new Rectangle(55, 401, 97 - 55, 444 - 401);

            //LHS BUTTONS
            var osbButton19Rect = new Rectangle(0, 370, 42, 409 - 370);
            var osbButton20Rect = new Rectangle(0, 322, 42, 362 - 322);
            var osbButton21Rect = new Rectangle(0, 276, 42, 314 - 276);
            var osbButton22Rect = new Rectangle(0, 228, 42, 267 - 228);
            var osbButton23Rect = new Rectangle(0, 181, 42, 221 - 181);
            var osbButton24Rect = new Rectangle(0, 135, 42, 176 - 135);
            var osbButton25Rect = new Rectangle(0, 88, 42, 129 - 88);
            var osbButton26Rect = new Rectangle(0, 41, 42, 81 - 41);

            var hsiModeToggleRect = new Rectangle(0, 452, 75, 500 - 452);
            var headingKnobRect = new Rectangle(82, 445, 118 - 82, 496 - 445);
            var fuelSelectToggleRect = new Rectangle(176, 449, 243 - 176, 496 - 449);
            var extFuelTransSwitchRect = new Rectangle(259, 447, 327 - 259, 498 - 447);


            if (RectangleContainsPoint(osbButton1Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton1;
            }
            if (RectangleContainsPoint(osbButton2Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton2;
            }
            if (RectangleContainsPoint(osbButton3Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton3;
            }
            if (RectangleContainsPoint(osbButton4Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton4;
            }
            if (RectangleContainsPoint(osbButton5Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton5;
            }
            if (RectangleContainsPoint(osbButton6Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton6;
            }
            if (RectangleContainsPoint(osbButton7Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton7;
            }
            if (RectangleContainsPoint(osbButton8Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton8;
            }
            if (RectangleContainsPoint(osbButton9Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton9;
            }
            if (RectangleContainsPoint(osbButton10Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton10;
            }
            if (RectangleContainsPoint(osbButton11Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton11;
            }
            if (RectangleContainsPoint(osbButton12Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton12;
            }
            if (RectangleContainsPoint(osbButton13Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton13;
            }
            if (RectangleContainsPoint(osbButton14Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton14;
            }
            if (RectangleContainsPoint(osbButton15Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton15;
            }
            if (RectangleContainsPoint(osbButton16Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton16;
            }
            if (RectangleContainsPoint(osbButton17Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton17;
            }
            if (RectangleContainsPoint(osbButton18Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton18;
            }
            if (RectangleContainsPoint(osbButton19Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton19;
            }
            if (RectangleContainsPoint(osbButton20Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton20;
            }
            if (RectangleContainsPoint(osbButton21Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton21;
            }
            if (RectangleContainsPoint(osbButton22Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton22;
            }
            if (RectangleContainsPoint(osbButton23Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton23;
            }
            if (RectangleContainsPoint(osbButton24Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton24;
            }
            if (RectangleContainsPoint(osbButton25Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton25;
            }
            if (RectangleContainsPoint(osbButton26Rect, clickedPoint))
            {
                return CpdInputControls.OsbButton26;
            }
            if (RectangleContainsPoint(hsiModeToggleRect, clickedPoint))
            {
                return CpdInputControls.HsiModeControl;
            }
            if (RectangleContainsPoint(headingKnobRect, clickedPoint))
            {
                return CpdInputControls.ParameterAdjustKnob;
            }
            if (RectangleContainsPoint(fuelSelectToggleRect, clickedPoint))
            {
                return CpdInputControls.FuelSelectControl;
            }
            if (RectangleContainsPoint(extFuelTransSwitchRect, clickedPoint))
            {
                return CpdInputControls.ExtFuelTransSwitch;
            }
            return CpdInputControls.Unknown;
        }

        private static bool RectangleContainsPoint(Rectangle rect, Point p)
        {
            return (rect.X <= p.X && rect.Y <= p.Y && rect.X + rect.Width >= p.X && rect.Y + rect.Height >= p.Y);
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            SaveControlBindings();
            Close();
        }

        private void LoadControlBindings()
        {
            var bindings = Settings.Default.ControlBindings;
            if (!String.IsNullOrEmpty(bindings))
            {
                var deserialized = Common.Serialization.Util.DeserializeFromXml(bindings, ControlBindings.GetType());
                if (deserialized != null)
                {
                    var asBindings = (SerializableDictionary<CpdInputControls, ControlBinding>) deserialized;
                    foreach (var entry in asBindings)
                    {
                        var thisEntry = entry.Value;
                        if (thisEntry.BindingType == BindingType.DirectInputAxisBinding ||
                            thisEntry.BindingType == BindingType.DirectInputButtonBinding ||
                            thisEntry.BindingType == BindingType.DirectInputPovBinding)
                        {
                            if (Mediator.DeviceMonitors.ContainsKey(thisEntry.DirectInputDevice.Guid))
                            {
                                var thisBinding = _controlBindings[entry.Key];
                                thisBinding.BindingType = thisEntry.BindingType;
                                thisBinding.ControlName = thisEntry.ControlName;
                                thisBinding.CpdInputControl = thisEntry.CpdInputControl;
                                thisBinding.Keys = thisEntry.Keys;
                                thisBinding.PovDirection = thisEntry.PovDirection;
                                thisBinding.DirectInputDevice =
                                    Mediator.DeviceMonitors[thisEntry.DirectInputDevice.Guid].DeviceInfo;
                                foreach (var control in thisBinding.DirectInputDevice.Controls)
                                {
                                    if (control.ControlNum == thisEntry.DirectInputControl.ControlNum &&
                                        control.ControlType == thisEntry.DirectInputControl.ControlType)
                                    {
                                        thisBinding.DirectInputControl = (DIPhysicalControlInfo) control;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (thisEntry.BindingType == BindingType.Keybinding)
                        {
                            var thisBinding = _controlBindings[entry.Key];
                            thisBinding.CpdInputControl = thisEntry.CpdInputControl;
                            thisBinding.BindingType = thisEntry.BindingType;
                            thisBinding.ControlName = thisEntry.ControlName;
                            thisBinding.Keys = thisEntry.Keys;
                        }
                    }
                }
            }
        }

        private void SaveControlBindings()
        {
            var bindings = Common.Serialization.Util.SerializeToXml(ControlBindings, ControlBindings.GetType());
            Settings.Default.ControlBindings = bindings;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void frmInputs_Load(object sender, EventArgs e)
        {
            txtConfigureInputsInstructions.AppendText(
                "Click on a switch, button, or knob in the image on the right to assign a hardware input ");
            txtConfigureInputsInstructions.AppendText("to that control in the " + Application.ProductName + " software.");
        }

        private void btnClearAllInputAssignments_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this,
                                         "WARNING: This will clear all input assignments.  Do you want to proceed?",
                                         Application.ProductName, MessageBoxButtons.OKCancel,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                InitializeControlBindings();
            }
        }
    }
}