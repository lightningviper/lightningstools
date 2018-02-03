using System;
using System.Windows.Forms;

namespace Phcc.DeviceManager.UI
{
    public partial class frmCalibrateServo : Form
    {
        public frmCalibrateServo()
        {
            InitializeComponent();
        }

        public ushort Gain
        {
            get { return (ushort) nudCalibrateServoGain.Value; }
            set { nudCalibrateServoGain.Value = value; }
        }

        public ushort CalibrationOffset
        {
            get { return (ushort) nudCalibrateServoOffset.Value; }
            set { nudCalibrateServoOffset.Value = value; }
        }

        private void trkCalibrateServoGain_Scroll(object sender, EventArgs e)
        {
            nudCalibrateServoGain.Value = trkCalibrateServoGain.Value;
        }

        private void nudCalibrateServoGain_ValueChanged(object sender, EventArgs e)
        {
            trkCalibrateServoGain.Value = (int) nudCalibrateServoGain.Value;
        }

        private void trkCalibrateServoOffset_Scroll(object sender, EventArgs e)
        {
            nudCalibrateServoOffset.Value = trkCalibrateServoOffset.Value;
        }

        private void nudCalibrateServoOffset_ValueChanged(object sender, EventArgs e)
        {
            trkCalibrateServoOffset.Value = (int) nudCalibrateServoOffset.Value;
        }
    }
}