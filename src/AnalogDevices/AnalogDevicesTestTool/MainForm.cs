using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AnalogDevices;
using log4net;

namespace AnalogDevicesTestTool
{
    public partial class MainForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MainForm));

        private bool _initialized;
        private DenseDacEvalBoard _selectedDevice;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetApplicationTitle();
            EnumerateDevices();
            AddEventHandlersForAllDACChannelSelectRadioButtions();
            UpdateDeviceValuesInUI();
            SelectDACChannel0();
        }

        private void SetApplicationTitle()
        {
            Text = Application.ProductName + " v" + Application.ProductVersion;
        }

        private void EnumerateDevices()
        {
            try
            {
                cboDevices.Items.Clear();
                var availableDevices = DenseDacEvalBoard.Enumerate();
                foreach (var device in availableDevices)
                {
                    device.DeviceState.UseRegisterCache = true;
                    device.IsThermalShutdownEnabled =
                        true; //enable over temp protection automatically during device enumeration
                }
                cboDevices.Items.AddRange(availableDevices);
                cboDevices.DisplayMember = "SymbolicName";
                if (availableDevices.Any())
                {
                    cboDevices.SelectedItem = cboDevices.Items[0];
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void AddEventHandlersForAllDACChannelSelectRadioButtions()
        {
            foreach (var radioButton in gbDACOutputs.Controls.OfType<RadioButton>()
                .Where(x => x.Text.StartsWith("DAC", StringComparison.Ordinal)))
                radioButton.CheckedChanged += DACChannelSelectRadioButton_CheckedChanged;
        }

        private void ReadbackSelectedDACChannelOffset()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            var dacChannelOffset = _selectedDevice.GetDacChannelOffset(selectedDacChannel);
            txtDACChannelOffset.Text = dacChannelOffset.ToString("X").PadLeft(4, '0');
        }

        private void ReadbackSelectedDACChannelGain()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            var dacChannelGain = _selectedDevice.GetDacChannelGain(selectedDacChannel);
            txtDACChannelGain.Text = dacChannelGain.ToString("X").PadLeft(4, '0');
        }

        private void ReadbackSelectedDACChannelDataValueA()
        {
            if (_selectedDevice != null)
            {
                var selectedDacChannel = DetermineSelectedDACChannelAddress();
                var dacChannelDataValueA = _selectedDevice.GetDacChannelDataValueA(selectedDacChannel);
                txtDataValueA.Text = dacChannelDataValueA.ToString("X").PadLeft(4, '0');
            }
        }

        private void ReadbackSelectedDACChannelDataValueB()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            var dacChannelDataValueB = _selectedDevice.GetDacChannelDataValueB(selectedDacChannel);
            txtDataValueB.Text = dacChannelDataValueB.ToString("X").PadLeft(4, '0');
        }

        private void ReadbackSelectedDACChannelDataSource()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            var dacChannelDataSource = _selectedDevice.GetDacChannelDataSource(selectedDacChannel);
            if (dacChannelDataSource == DacChannelDataSource.DataValueA)
            {
                rdoDataValueA.Checked = true;
            }
            else
            {
                rdoDataValueB.Checked = true;
            }
        }

        private void DACChannelSelectRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton) sender).Checked)
            {
                return;
            }
            UpdateUIValuesForSelectedDACChannel();
        }

        private void UpdateUIValuesForSelectedDACChannel()
        {
            ReadbackSelectedDACChannelDataSource();
            ReadbackSelectedDACChannelDataValueA();
            ReadbackSelectedDACChannelDataValueB();
            ReadbackSelectedDACChannelOffset();
            ReadbackSelectedDACChannelGain();
            UpdateCalculatedOutputVoltage();
        }

        private ChannelAddress DetermineSelectedDACChannelAddress()
        {
            var radioButton = gbDACOutputs.Controls.OfType<RadioButton>()
                .First(x => x.Checked && x.Text.StartsWith("DAC", StringComparison.Ordinal));
            var text = radioButton.Text;
            var DACNumber = int.Parse(text.Replace("DAC ", ""));
            return (ChannelAddress) (DACNumber + 8);
        }

        private void ValidateVREF(Control control)
        {
            ValidateFloat(control, 2, 5);
        }

        private void ValidateDACChannelOffset(Control control)
        {
            ValidateUnsignedInteger(control, 0x0000, 0xFFFF);
        }

        private void ValidateDACChannelGain(Control control)
        {
            ValidateUnsignedInteger(control, 0x0000, 0xFFFF);
        }

        private void ValidateOffsetDAC(Control control)
        {
            ValidateUnsignedInteger(control, 0x0000, 0x3FFF);
        }


        private void txtVREF0_Validating(object sender, CancelEventArgs e)
        {
            ValidateVREF(txtVREF0);
        }

        private void txtVREF1_Validating(object sender, CancelEventArgs e)
        {
            ValidateVREF(txtVREF1);
        }

        private void txtOffsetDAC0_Validating(object sender, CancelEventArgs e)
        {
            ValidateOffsetDAC(txtOffsetDAC0);
        }

        private void txtOffsetDAC1_Validating(object sender, CancelEventArgs e)
        {
            ValidateOffsetDAC(txtOffsetDAC1);
        }

        private void txtDACChannelOffset_Validating(object sender, CancelEventArgs e)
        {
            ValidateDACChannelOffset(txtDACChannelOffset);
        }

        private void txtDACChannelOffset_Validated(object sender, EventArgs e)
        {
            SetDACChannelOffset();
        }

        private void SetDACChannelOffset()
        {
            var parsed = ushort.TryParse(txtDACChannelOffset.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dacChannelOffset);
            if (!parsed)
            {
                return;
            }
            UpdateCalculatedOutputVoltage();
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            _selectedDevice.SetDacChannelOffset(selectedDacChannel, dacChannelOffset);
        }

        private void txtDACChannelGain_Validated(object sender, EventArgs e)
        {
            SetDACChannelGain();
        }

        private void SetDACChannelGain()
        {
            var parsed = ushort.TryParse(txtDACChannelGain.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dacChannelGain);
            if (!parsed)
            {
                return;
            }
            UpdateCalculatedOutputVoltage();
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            _selectedDevice.SetDacChannelGain(selectedDacChannel, dacChannelGain);
        }

        private void txtDACChannelGain_Validating(object sender, CancelEventArgs e)
        {
            ValidateDACChannelGain(txtDACChannelGain);
        }


        private void cboDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDevice = (DenseDacEvalBoard) cboDevices.SelectedItem;
            UpdateDeviceValuesInUI();
            SelectDACChannel0();
        }

        private void UpdateDeviceValuesInUI()
        {
            UpdateGroupOffsetDataInUIToDeviceValues();
            UpdateUIValuesForSelectedDACChannel();
            UpdateCalculatedOutputVoltage();
            UpdateOverTemperatureValuesInUIFromDevice();
        }

        private void UpdateOverTemperatureValuesInUIFromDevice()
        {
            if (_selectedDevice != null)
            {
                var isOverTemp = _selectedDevice.IsOverTemperature;
                if (isOverTemp)
                {
                    txtOverTempStatus.Text = "HOT";
                    txtOverTempStatus.BackColor = Color.Red;
                    txtOverTempStatus.ForeColor = Color.White;
                }
                else
                {
                    txtOverTempStatus.Text = "OK";
                    txtOverTempStatus.BackColor = Color.Green;
                    txtOverTempStatus.ForeColor = Color.White;
                }
                var overTempShutdownEnabled = _selectedDevice.IsThermalShutdownEnabled;
                chkOverTempShutdownEnabled.Checked = overTempShutdownEnabled;
            }
            else
            {
                txtOverTempStatus.Text = "N/A";
                txtOverTempStatus.BackColor = Color.Empty;
                txtOverTempStatus.ForeColor = Color.Black;
            }
        }

        private void SelectDACChannel0()
        {
            var DAC0RadioButton = gbDACOutputs.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Text == "DAC 0");
            if (DAC0RadioButton != null)
            {
                DAC0RadioButton.Checked = true;
            }
        }

        private void UpdateGroupOffsetDataInUIToDeviceValues()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            txtOffsetDAC0.Text = _selectedDevice.OffsetDAC0.ToString("X").PadLeft(4, '0');
            txtOffsetDAC1.Text = _selectedDevice.OffsetDAC1.ToString("X").PadLeft(4, '0');
        }


        private void txtDataValueA_Validating(object sender, CancelEventArgs e)
        {
            ValidateUnsignedInteger(txtDataValueA, 0x0000, 0xFFFF);
        }

        private void txtDataValueB_Validating(object sender, CancelEventArgs e)
        {
            ValidateUnsignedInteger(txtDataValueB, 0x0000, 0xFFFF);
        }

        private void ValidateUnsignedInteger(Control control, int low, int high)
        {
            errorProvider1.SetError(control, string.Empty);
            var text = control.Text;
            var parsed = ushort.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort val);
            if (!parsed || val < low || val > high)
            {
                errorProvider1.SetError(control,
                    $"Value must be a positive hexadecimal integer between {low:X} and {high:X}");
            }
        }

        private void ValidateFloat(Control control, float low, float high)
        {
            errorProvider1.SetError(control, string.Empty);
            var text = control.Text;
            var parsed = float.TryParse(text, out float val);
            if (!parsed || float.IsNaN(val) || float.IsInfinity(val) || val < low || val > high)
            {
                errorProvider1.SetError(control,
                    $"Value must be a positive integer or decimal between {low} and {high}");
            }
        }

        private void Initialize()
        {
            UpdateOffsetDAC0();
            UpdateOffsetDAC1();
            SetDACChannelOffset();
            SetDACChannelGain();
            SetDACChannelDataSourceAndUpdateDataValue();
            _initialized = true;
        }

        private void btnUpdateAllDacOutputs_Click(object sender, EventArgs e)
        {
            UpdateCalculatedOutputVoltage();
            if (_selectedDevice == null)
            {
                return;
            }
            if (!_initialized)
            {
                Initialize();
            }
            _selectedDevice.UpdateAllDacOutputs();
        }

        private void SetDACChannelDataSourceAndUpdateDataValue()
        {
            if (_selectedDevice == null)
            {
                return;
            }
            var selectedDacChannel = DetermineSelectedDACChannelAddress();
            if (rdoDataValueA.Checked)
            {
                _selectedDevice.SetDacChannelDataSource(selectedDacChannel, DacChannelDataSource.DataValueA);
                UpdateDataValueA();
            }
            else
            {
                _selectedDevice.SetDacChannelDataSource(selectedDacChannel, DacChannelDataSource.DataValueB);
                UpdateDataValueB();
            }
        }

        private void btnResumeAllDACOutputs_Click(object sender, EventArgs e)
        {
            UpdateCalculatedOutputVoltage();
            _selectedDevice?.ResumeAllDacOutputs();
        }

        private void btnSuspendAllDACOutputs_Click(object sender, EventArgs e)
        {
            UpdateCalculatedOutputVoltage();
            _selectedDevice?.SuspendAllDacOutputs();
        }

        private void btnSoftPowerUp_Click(object sender, EventArgs e)
        {
            _selectedDevice?.PerformSoftPowerUp();
        }

        private void btnPerformSoftPowerDown_Click(object sender, EventArgs e)
        {
            _selectedDevice?.PerformSoftPowerDown();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (_selectedDevice != null)
            {
                var tempShutdownEnabled = chkOverTempShutdownEnabled.Checked;
                _selectedDevice.Reset();
                _selectedDevice.IsThermalShutdownEnabled |= tempShutdownEnabled;
                UpdateDeviceValuesInUI();
            }
        }


        private void rdoDataValueA_Click(object sender, EventArgs e)
        {
            if (rdoDataValueA.Checked)
            {
                SetDACChannelDataSourceAndUpdateDataValue();
                UpdateCalculatedOutputVoltage();
            }
        }

        private void rdoDataValueB_Click(object sender, EventArgs e)
        {
            if (rdoDataValueB.Checked)
            {
                SetDACChannelDataSourceAndUpdateDataValue();
                UpdateCalculatedOutputVoltage();
            }
        }

        private void txtOffsetDAC0_Validated(object sender, EventArgs e)
        {
            UpdateOffsetDAC0();
        }

        private void UpdateOffsetDAC0()
        {
            var parsed = ushort.TryParse(txtOffsetDAC0.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort offsetDAC0);
            if (parsed)
            {
                UpdateCalculatedOutputVoltage();
                if (_selectedDevice != null)
                {
                    _selectedDevice.OffsetDAC0 = offsetDAC0;
                }
            }
        }

        private void txtOffsetDAC1_Validated(object sender, EventArgs e)
        {
            UpdateOffsetDAC1();
        }

        private void UpdateOffsetDAC1()
        {
            var parsed = ushort.TryParse(txtOffsetDAC1.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort offsetDAC1);
            if (parsed)
            {
                UpdateCalculatedOutputVoltage();
                if (_selectedDevice != null)
                {
                    _selectedDevice.OffsetDAC1 = offsetDAC1;
                }
            }
        }

        private void txtDataValueA_Validated(object sender, EventArgs e)
        {
            UpdateDataValueA();
        }

        private void UpdateDataValueA()
        {
            var parsed = ushort.TryParse(txtDataValueA.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dataValueA);
            if (parsed)
            {
                UpdateCalculatedOutputVoltage();
                if (_selectedDevice != null)
                {
                    var selectedDacChannel = DetermineSelectedDACChannelAddress();
                    _selectedDevice.SetDacChannelDataValueA(selectedDacChannel, dataValueA);
                }
            }
        }

        private void txtDataValueB_Validated(object sender, EventArgs e)
        {
            UpdateDataValueB();
        }

        private void UpdateDataValueB()
        {
            var parsed = ushort.TryParse(txtDataValueB.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dataValueB);
            if (parsed)
            {
                UpdateCalculatedOutputVoltage();
                if (_selectedDevice != null)
                {
                    var selectedDacChannel = DetermineSelectedDACChannelAddress();
                    _selectedDevice.SetDacChannelDataValueB(selectedDacChannel, dataValueB);
                }
            }
        }

        private void UpdateCalculatedOutputVoltage()
        {
            var dacChannel = DetermineSelectedDACChannelAddress();
            var calculatedOutputVoltage = Vout(dacChannel);
            var asText = calculatedOutputVoltage.ToString("F3");
            if (calculatedOutputVoltage >= 0)
            {
                asText = "+" + asText;
            }
            txtVoutCalculated.Text = asText;
        }

        private static float Vout(ushort dacChannelDataValue, ushort dacChannelOffsetValue, ushort dacChannelGainValue,
            ushort offsetDACvalue, float Vref)
        {
            var dac_code = (ushort) (dacChannelDataValue * (dacChannelGainValue + (long) 1) / 65536) +
                           (long) dacChannelOffsetValue - 32768;
            if (dac_code >= 65535)
            {
                dac_code = 65535;
            }
            if (dac_code < 0)
            {
                dac_code = 0;
            }
            return 4 * Vref * ((dac_code - 4 * (long) offsetDACvalue) / (float) 65536);
        }

        private float Vout(ChannelAddress dacChannel)
        {
            var dacChannelDataSource = rdoDataValueA.Checked
                ? DacChannelDataSource.DataValueA
                : DacChannelDataSource.DataValueB;

            var parsed = ushort.TryParse(txtDataValueA.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dataValueA);
            if (!parsed)
            {
                return 0;
            }
            parsed = ushort.TryParse(txtDataValueB.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dataValueB);
            if (!parsed)
            {
                return 0;
            }

            var dacChannelDataValue = dacChannelDataSource == DacChannelDataSource.DataValueA
                ? dataValueA
                : dataValueB;

            parsed = ushort.TryParse(txtDACChannelOffset.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dacChannelOffsetValue);
            if (!parsed)
            {
                return 0;
            }
            parsed = ushort.TryParse(txtDACChannelGain.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out ushort dacChannelGainValue);
            if (!parsed)
            {
                return 0;
            }

            ushort offsetDACValue;
            float vRef;
            if (dacChannel >= ChannelAddress.Group0Channel0 && dacChannel <= ChannelAddress.Group0Channel7)
            {
                parsed = ushort.TryParse(txtOffsetDAC0.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out offsetDACValue);
                if (!parsed)
                {
                    return 0;
                }
                parsed = float.TryParse(txtVREF0.Text, out vRef);
                if (!parsed)
                {
                    return 0;
                }
            }
            else if (dacChannel >= ChannelAddress.Group1Channel0 && dacChannel <= ChannelAddress.Group4Channel7)
            {
                parsed = ushort.TryParse(txtOffsetDAC1.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out offsetDACValue);
                if (!parsed)
                {
                    return 0;
                }
                parsed = float.TryParse(txtVREF1.Text, out vRef);
                if (!parsed)
                {
                    return 0;
                }
            }

            else
            {
                parsed = ushort.TryParse(txtOffsetDAC0.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out offsetDACValue);
                if (!parsed)
                {
                    return 0;
                }
                parsed = float.TryParse(txtVREF0.Text, out vRef);
                if (!parsed)
                {
                    return 0;
                }
            }
            return Vout(dacChannelDataValue, dacChannelOffsetValue, dacChannelGainValue, offsetDACValue, vRef);
        }

        private void txtVREF0_Click(object sender, EventArgs e)
        {
            txtVREF0.SelectAll();
        }

        private void txtVREF1_Click(object sender, EventArgs e)
        {
            txtVREF1.SelectAll();
        }

        private void txtOffsetDAC0_Click(object sender, EventArgs e)
        {
            txtOffsetDAC0.SelectAll();
        }

        private void txtOffsetDAC1_Click(object sender, EventArgs e)
        {
            txtOffsetDAC1.SelectAll();
        }

        private void txtDACChannelOffset_Click(object sender, EventArgs e)
        {
            txtDACChannelOffset.SelectAll();
        }

        private void txtDACChannelGain_Click(object sender, EventArgs e)
        {
            txtDACChannelGain.SelectAll();
        }

        private void txtDataValueA_Click(object sender, EventArgs e)
        {
            txtDataValueA.SelectAll();
        }

        private void txtDataValueB_Click(object sender, EventArgs e)
        {
            txtDataValueB.SelectAll();
        }

        private void chkOverTempShutdownEnabled_Click(object sender, EventArgs e)
        {
            if (_selectedDevice != null)
            {
                _selectedDevice.IsThermalShutdownEnabled = chkOverTempShutdownEnabled.Checked;
            }
        }

        private void txtVREF0_Validated(object sender, EventArgs e)
        {
            UpdateCalculatedOutputVoltage();
        }

        private void txtVREF1_Validated(object sender, EventArgs e)
        {
            UpdateCalculatedOutputVoltage();
        }
    }
}