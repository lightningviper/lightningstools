using System;
using System.Windows.Forms;

namespace JoyMapper
{
    public partial class frmPPJoyDevicesToCreate : Form
    {
        private int _maxNumCreateableDevices = 16;
        private int _numExistingDevices;

        public frmPPJoyDevicesToCreate()
        {
            InitializeComponent();
        }

        public int NumDevicesToCreate => Convert.ToInt32(udDevicesToCreate.Value);

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PPJoyDevicesToCreateForm_Load(object sender, EventArgs e)
        {
            _maxNumCreateableDevices = Util.GetMaxPPJoyVirtualDevicesAllowed();
            _numExistingDevices = Util.CountPPJoyVirtualDevices();
            lblNumExistingDevices.Text = _numExistingDevices.ToString();
            udDevicesToCreate.Minimum = 1;
            udDevicesToCreate.Maximum = _maxNumCreateableDevices - _numExistingDevices;
            udDevicesToCreate.Value = 1;
        }
    }
}