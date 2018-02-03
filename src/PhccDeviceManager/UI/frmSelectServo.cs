using System;
using System.Windows.Forms;

namespace Phcc.DeviceManager.UI
{
    public partial class frmSelectServo : Form
    {
        public frmSelectServo()
        {
            InitializeComponent();
        }

        public int SelectedServo { get; set; }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            SelectedServo = -1;
            if (rdoServo1.Checked)
            {
                SelectedServo = 1;
            }
            else if (rdoServo2.Checked)
            {
                SelectedServo = 2;
            }
            else if (rdoServo3.Checked)
            {
                SelectedServo = 3;
            }
            else if (rdoServo4.Checked)
            {
                SelectedServo = 4;
            }
            else if (rdoServo5.Checked)
            {
                SelectedServo = 5;
            }
            else if (rdoServo6.Checked)
            {
                SelectedServo = 6;
            }
            else if (rdoServo7.Checked)
            {
                SelectedServo = 7;
            }
            else if (rdoServo8.Checked)
            {
                SelectedServo = 8;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}