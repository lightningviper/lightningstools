using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DTSCard;
using log4net;

namespace DTSCardTestTool
{
    public partial class MainForm : Form
    {
        private DTSCardManaged _dtsCard=null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetApplicationTitle();
        }

        private void SetApplicationTitle()
        {
            Text = Application.ProductName + " v" + Application.ProductVersion;
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

        private void btnSetSerial_Click(object sender, EventArgs e)
        {
            if (_dtsCard != null)
            {
                _dtsCard.Dispose();
                _dtsCard = null;
            }
            _dtsCard = new DTSCardManaged();
            _dtsCard.SetSerial(txtSerial.Text);
            var result = _dtsCard.Init();
            if (result == (int)DTSCardOperationStatus.Success)
            {
                ClearError(txtSerial);
                SendAngleUpdate();
            }
            else
            {
                SetError(txtSerial, result);
            }
        }
        private void SetError(Control control, int result)
        {
            if (result == (int)DTSCardOperationStatus.Success)
            {
                errorProvider1.SetError(control, String.Empty);
            }
            else if (result == (int)DTSCardOperationStatus.CardNotFound)
            {
                errorProvider1.SetError(control, "Device not found with this serial #");
            }
            else if (result == (int)DTSCardOperationStatus.NoCallback)
            {
                errorProvider1.SetError(control, "Error: No callback");
            }
            else if (result == (int)DTSCardOperationStatus.SendPending)
            {
                errorProvider1.SetError(control, "Error: Send pending");
            }
            else if (result == (int)DTSCardOperationStatus.Timeout)
            {
                errorProvider1.SetError(control, "Error: Timeout");
            }
            else if (result == (int)DTSCardOperationStatus.WindowsError)
            {
                errorProvider1.SetError(control, $"Error: {_dtsCard.WinError}");
            }
            else if (result == (int)DTSCardOperationStatus.Undefined)
            {
                errorProvider1.SetError(control, $"Error (undefined)");
            }
        }
        private void ClearError(Control control)
        {
            errorProvider1.SetError(control, string.Empty);
        }
        private void nudAngle_ValueChanged(object sender, EventArgs e)
        {
            SendAngleUpdate();
        }

        private void SendAngleUpdate()
        {
            if (_dtsCard != null)
            {
                _dtsCard.SetAngle((double)nudAngle.Value);
                var result = _dtsCard.Update();
                SetError(txtSerial, result);
            }
        }
    }
}