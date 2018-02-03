using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using SDI;
using log4net;

namespace ADITestTool
{
    public partial class frmMain : Form
    {
        private Device _pitchSdiDevice = new Device();
        private Device _rollSdiDevice = new Device();
        private ReadOnlyCollection<string> _serialPorts;
        private ILog _log = LogManager.GetLogger(typeof(frmMain));
        public frmMain()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public ReadOnlyCollection<string> SerialPorts
        {
            get { return _serialPorts; }
        }



        private void frmMain_Load(object sender, EventArgs e)
        {
            epErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            Text = Application.ProductName + " v" + Application.ProductVersion;
            SetupSerialPorts();
            ResetErrors();
            Activate();
        }

        private void SetupSerialPorts()
        {
            EnumerateSerialPorts();
            cbPitchDeviceSerialPort.Sorted = true;
            cbRollDeviceSerialPort.Sorted = true;
            cbPitchDeviceSerialPort.Items.Clear();
            cbRollDeviceSerialPort.Items.Clear();
            foreach (var port in _serialPorts)
            {
                cbPitchDeviceSerialPort.Items.Add(port);
                cbPitchDeviceSerialPort.Text = port;
                cbRollDeviceSerialPort.Items.Add(port);
                cbRollDeviceSerialPort.Text = port;
                Application.DoEvents();
            }
            ChangePitchDeviceSerialPort();
            ChangeRollDeviceSerialPort();

        }

        private void EnumerateSerialPorts()
        {
            var ports = new Ports();
            _serialPorts = ports.SerialPortNames;
        }

        private void cbPitchDeviceSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangePitchDeviceSerialPort();
        }
        private void cbRollDeviceSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeRollDeviceSerialPort();
        }
        private void ChangePitchDeviceSerialPort()
        {
            ChangeSerialPort(cbPitchDeviceSerialPort, ref _pitchSdiDevice, lblPitchDeviceIdentification);
        }
        private void ChangeRollDeviceSerialPort()
        {
            ChangeSerialPort(cbRollDeviceSerialPort, ref _rollSdiDevice, lblRollDeviceIdentification);
        }
        private void ChangeSerialPort(ComboBox serialPortSelectionComboBox, ref Device sdiDevice, Label deviceIdentificationLabel)
        {
            var selectedPort = serialPortSelectionComboBox.Text;
            if (String.IsNullOrWhiteSpace(selectedPort)) return;
            try
            {
                if (sdiDevice != null) DisposeSDIDevice(ref sdiDevice);
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
            }
            try
            {
                sdiDevice = new Device(selectedPort);
                sdiDevice.DisableWatchdog();
                sdiDevice.ConfigureUsbDebug(enable: false);

                var identification = sdiDevice.Identify().TrimEnd();
                if (!string.IsNullOrWhiteSpace(identification))
                {
                    deviceIdentificationLabel.Text = "Identification:" + identification;
                }
            }
            catch (Exception ex)
            {
                DisposeSDIDevice(ref sdiDevice);
                deviceIdentificationLabel.Text = "Identification:";
                _log.Debug(ex);
            }
            ResetErrors();
        }

       private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeSDIDevice(ref _pitchSdiDevice);
            DisposeSDIDevice(ref _rollSdiDevice);
        }

        private void DisposeSDIDevice(ref Device sdiDevice)
        {
            if (sdiDevice != null)
            {
                try
                {
                    sdiDevice.Dispose();
                    sdiDevice = null;
                }
                catch (Exception ex)
                {
                    _log.Debug(ex);
                }
            }
        }

        private void ResetErrors()
        {
            epErrorProvider.Clear();
            SetErrorsForSerialDeviceSelection(cbPitchDeviceSerialPort, lblPitchDeviceIdentification);
            SetErrorsForSerialDeviceSelection(cbRollDeviceSerialPort, lblRollDeviceIdentification);
            UpdateUIControlsEnabledOrDisabledState();
        }

        private void SetErrorsForSerialDeviceSelection(ComboBox serialPortSelectionComboBox, Label deviceIdentificationLabel)
        {
            if (String.IsNullOrEmpty(serialPortSelectionComboBox.Text) || deviceIdentificationLabel.Text == "Identification:")
            {
                epErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                epErrorProvider.SetError(serialPortSelectionComboBox,
                                         "No serial port is selected, or no SDI device is detected on the selected serial port.");
            }
        }

        private void tcTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetErrors();
        }

        private void txtPitchSubAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtPitchSubAddr, out val);
        }
        private void txtRollSubAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtRollSubAddr, out val);
        }
        private void btnPitchSendRaw_Click(object sender, EventArgs e)
        {
            SendRaw(txtPitchSubAddr, txtPitchDataByte, _pitchSdiDevice, ()=>PitchDeviceIsValid);
        }
        private void btnRollSendRaw_Click(object sender, EventArgs e)
        {
            SendRaw(txtRollSubAddr, txtRollDataByte, _rollSdiDevice, ()=>RollDeviceIsValid);
        }
        private void SendRaw(TextBox subAddressTextBox, TextBox dataByteTextBox, Device sdiDevice, Func<bool> deviceValidationFunction)
        {
            byte subAddr = 0;
            byte data = 0;
            bool valid = ValidateHexTextControl(subAddressTextBox, out subAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(dataByteTextBox, out data);
            if (valid)
            {
                if (deviceValidationFunction())
                {
                    try
                    {
                        sdiDevice.SendCommand((CommandSubaddress)subAddr, data);
                    }
                    catch (Exception ex)
                    {
                        _log.Debug(ex);
                    }
                }
            }
        }

        private void txtPitchDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtPitchDataByte, out val);
        }
        private void txtRollDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtRollDataByte, out val);
        }

        private void UpdateUIControlsEnabledOrDisabledState()
        {
            gbPitchRawDataControl.Enabled = PitchDeviceIsValid;
            gbRollRawDataControl.Enabled = RollDeviceIsValid;
        }
        private bool PitchDeviceIsValid
        {
            get
            {
                return _pitchSdiDevice != null && !string.IsNullOrWhiteSpace(_pitchSdiDevice.COMPort);
            }
        }
        private bool RollDeviceIsValid
        {
            get
            {
                return _rollSdiDevice != null && !string.IsNullOrWhiteSpace(_rollSdiDevice.COMPort);
            }
        }
        private bool IsPitch(string deviceIdentification)
        {
            return PitchDeviceIsValid &&
                    (
                        deviceIdentification.ToLowerInvariant().EndsWith("30") 
                            ||
                        deviceIdentification.ToLowerInvariant().EndsWith("48")
                    );
        }
        private bool IsRoll(string deviceIdentification)
        {
            return RollDeviceIsValid &&
                (
                    deviceIdentification.ToLowerInvariant().EndsWith("32")
                        ||
                    deviceIdentification.ToLowerInvariant().EndsWith("50")
                );
        }
        private bool ValidateHexTextControl(TextBox textControl, out byte val)
        {
            // First create an instance of the call stack   
            var callStack = new StackTrace();
            var frames = callStack.GetFrames();
            for (var i = 1; i < frames.Length; i++)
            {
                var frame = frames[i];
                var method = frame.GetMethod();
                // Get the declaring type and method names    
                var declaringType = method.DeclaringType.Name;
                var methodName = method.Name;
                if (methodName != null && methodName.Contains("ValidateHex"))
                {
                    val = 0x00;
                    return true;
                }
            }

            ResetErrors();
            var text = textControl.Text.Trim().ToUpperInvariant();
            textControl.Text = text;
            var parsed = false;
            if (text.StartsWith("0X"))
            {
                text = text.Substring(2, text.Length - 2);
                textControl.Text = "0x" + text;
                parsed = Byte.TryParse(text, NumberStyles.HexNumber, null, out val);
            }
            else
            {
                parsed = Byte.TryParse(text, out val);
            }
            if (!parsed)
            {
                epErrorProvider.SetError(textControl,
                                         "Invalid hexadecimal or decimal byte value.\nHex values should be preceded by the\ncharacters '0x' (zero, x) and\nshould be in the range 0x00-0xFF.\nDecimal values should be in the range 0-255.");
            }
            return parsed;
        }

        private void chkGsFlagVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGsFlagVisible.Checked)
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)15, 0);
            }
            else
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)15, 1);
            }
        }

        private void chkLocFlagVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLocFlagVisible.Checked)
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)17, 0);
            }
            else
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)17, 1);
            }
        }

        private void chkAuxFlagVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAuxFlagVisible.Checked)
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)18, 0);
            }
            else
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)18, 1);
            }
        }

        private void chkEnableFlagsAndRot_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableFlagsAndRot.Checked)
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)16, 1);
            }
            else
            {
                _rollSdiDevice.SendCommand((CommandSubaddress)16, 0);
            }
        }

        private void chkEnableGlideslope_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableGlideslope.Checked)
            {
                _pitchSdiDevice.SendCommand((CommandSubaddress)16, 1);
            }
            else
            {
                _pitchSdiDevice.SendCommand((CommandSubaddress)16, 0);
            }
        }

        private void chkEnableRollAndPitch_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableRollAndPitch.Checked)
            {
                _pitchSdiDevice.SendCommand((CommandSubaddress)15, 1);
            }
            else
            {
                _pitchSdiDevice.SendCommand((CommandSubaddress)15, 0);
            }
        }

        private void nudGlideslopeIndicatorHorizontal_ValueChanged(object sender, EventArgs e)
        {
            _pitchSdiDevice.SendCommand((CommandSubaddress)17, (byte)nudGlideslopeIndicatorHorizontal.Value);
        }

        private void nudGlideslopeIndicatorVertical_ValueChanged(object sender, EventArgs e)
        {
            _pitchSdiDevice.SendCommand((CommandSubaddress)18, (byte)nudGlideslopeIndicatorVertical.Value);
        }

        private void nudRateOfTurnIndicator_ValueChanged(object sender, EventArgs e)
        {
            _rollSdiDevice.SendCommand((CommandSubaddress)22, (byte)nudRateOfTurnIndicator.Value);
        }

        private void nudSpherePitchIndication_ValueChanged(object sender, EventArgs e)
        {
            var value = nudSpherePitchIndication.Value;
            var address = 0;
            if (value < 256)
            {
                address = 0;
            }
            else if (value >= 256 && value < 512)
            {
                address = 1;
                value -= 256;
            }
            else if (value >= 512 && value < 768)
            {
                address = 2;
                value -= 512;
            }
            else
            {
                address = 3;
                value -= 768;
            }
            _pitchSdiDevice.SendCommand((CommandSubaddress)address, (byte)value);
        }

        private void nudRollIndication_ValueChanged(object sender, EventArgs e)
        {
            var value = nudRollIndication.Value;
            var address = 0;
            if (value <256)
            {
                address = 0;
            }
            else if (value >= 256 && value <512)
            {
                address = 1;
                value -= 256;
            }
            else if (value >=512 && value < 768)
            {
                address = 2;
                value -= 512;
            }
            else
            {
                address = 3;
                value -= 768;
            }
            _rollSdiDevice.SendCommand((CommandSubaddress)address, (byte)value);
        }
    }
}