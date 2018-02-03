using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace Phcc.DeviceManager.UI
{
    public partial class frmPromptForPeripheralAddress : Form
    {
        public frmPromptForPeripheralAddress()
        {
            InitializeComponent();
        }

        public byte BaseAddress { get; set; }
        public List<byte> ProhibitedBaseAddresses { get; set; }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var wasValid = ValidateBaseAddress();
            if (wasValid)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateBaseAddress()
        {
            errorProvider1.Clear();
            var baseAddressAsEntered = txtBaseAddress.Text;
            byte val = 0;
            var valid = TryParseBaseAddress(baseAddressAsEntered, out val);
            if (!valid)
            {
                errorProvider1.SetError(txtBaseAddress,
                                        "Invalid hexadecimal or decimal byte value.\nHex values should be preceded by the\ncharacters '0x' (zero, x) and\nshould be in the range 0x00-0xFF.\nDecimal values should be in the range 0-255.");
            }
            else
            {
                if (ProhibitedBaseAddresses != null)
                {
                    foreach (var prohibitedAddress in ProhibitedBaseAddresses)
                    {
                        if (prohibitedAddress == val)
                        {
                            valid = false;
                            errorProvider1.SetError(txtBaseAddress,
                                                    "That peripheral address is already being used by another peripheral.");
                            break;
                        }
                    }
                }
                if (valid)
                {
                    BaseAddress = val;
                }
            }
            return valid;
        }

        private bool TryParseBaseAddress(string baseAddress, out byte val)
        {
            var valid = true;
            val = 0;
            if (!String.IsNullOrEmpty(baseAddress))
            {
                baseAddress = baseAddress.Trim();
            }
            if (String.IsNullOrEmpty(baseAddress))
            {
                valid = false;
            }
            else
            {
                if (baseAddress.StartsWith("0x") || baseAddress.StartsWith("0X"))
                {
                    baseAddress = baseAddress.Substring(2, baseAddress.Length - 2);
                    valid = Byte.TryParse(baseAddress, NumberStyles.HexNumber, null, out val);
                }
                else
                {
                    valid = Byte.TryParse(baseAddress, out val);
                }
            }
            return valid;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtBaseAddress_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
        }
    }
}