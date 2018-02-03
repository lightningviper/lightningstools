using System;
using System.Diagnostics;
using Common.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using F16CPD.Properties;
using log4net;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace F16CPD.UI.Forms
{
    public partial class frmMain : Form, IDisposable
    {
        //TODO: add option to config screen to allow setting the initial transition altitude
        private static readonly ILog _log = LogManager.GetLogger(typeof (frmMain));
        private F16CpdEngine _cpdEngine;
        protected bool _isDisposed;

        public frmMain()
        {
            InitializeComponent();
        }

        public string[] CommandLineSwitches { get; set; }

        private void chkLaunchAtSystemStartup_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowsStartupRegKey();
        }

        private void UpdateWindowsStartupRegKey()
        {
            if (chkLaunchAtSystemStartup.Checked)
            {
                //update the Windows Registry's Run-at-startup applications list according
                //to the new user settings
                var c = new Computer();
                try
                {
                    using (
                        var startupKey =
                            c.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (startupKey != null)
                            startupKey.SetValue(Application.ProductName, Application.ExecutablePath,
                                                RegistryValueKind.String);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
            else
            {
                var c = new Computer();
                try
                {
                    using (
                        var startupKey =
                            c.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (startupKey != null) startupKey.DeleteValue(Application.ProductName, false);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
            Settings.Default.LaunchAtWindowsStartup = chkLaunchAtSystemStartup.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void chkStartWhenLaunched_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.StartWhenLaunched = chkStartWhenLaunched.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            MinimizeToSystemTray();
            lblVersion.Text = "Version: " + Application.ProductVersion;
            nfyTrayIcon.Icon = Icon;
            foreach (var priority in Enum.GetNames(typeof (ThreadPriority)))
            {
                if (priority.ToLowerInvariant() != "highest")
                {
                    cbPriority.Items.Add(priority);
                }
            }
            cbOutputRotation.Items.Add("No rotation");
            cbOutputRotation.Items.Add("+90 degrees");
            cbOutputRotation.Items.Add("+180 degrees");
            cbOutputRotation.Items.Add("-90 degrees");
            cmdRescueOutputWindow.Enabled = false;
            LoadSettings();
            if (chkStartWhenLaunched.Checked)
            {
                Start();
            }
        }

        private void LoadSettings()
        {
            Settings.Default.Reload();
            chkStartWhenLaunched.Checked = Settings.Default.StartWhenLaunched;
            chkLaunchAtSystemStartup.Checked = Settings.Default.LaunchAtWindowsStartup;
            if (!Settings.Default.RunAsClient && !Settings.Default.RunAsServer)
            {
                rdoStandaloneMode.Checked = true;
            }
            rdoClientMode.Checked = Settings.Default.RunAsClient;
            rdoServerMode.Checked = Settings.Default.RunAsServer;
            txtServerIPAddress.Text = Settings.Default.ServerIPAddress;
            txtServerPortNum.Text = Settings.Default.ServerPortNum;
            nudPollFrequency.Value = Settings.Default.PollingFrequencyMillis;
            cbPriority.SelectedItem = Enum.GetName(typeof (ThreadPriority), Settings.Default.Priority);
            rdoNorth360.Checked = Settings.Default.DisplayNorthAsThreeSixZero;
            rdoNorth000.Checked = !Settings.Default.DisplayNorthAsThreeSixZero;
            var rotation = Settings.Default.Rotation;
            if (rotation == RotateFlipType.RotateNoneFlipNone)
            {
                cbOutputRotation.SelectedItem = "No rotation";
            }
            else if (rotation == RotateFlipType.Rotate90FlipNone)
            {
                cbOutputRotation.SelectedItem = "+90 degrees";
            }
            else if (rotation == RotateFlipType.Rotate180FlipNone)
            {
                cbOutputRotation.SelectedItem = "+180 degrees";
            }
            else if (rotation == RotateFlipType.Rotate270FlipNone)
            {
                cbOutputRotation.SelectedItem = "-90 degrees";
            }
            nudCourseHeadingAdjustmentSpeed.Value = Math.Min(Settings.Default.FastCourseAndHeadingAdjustSpeed, 5);
            if (Settings.Default.DisplayVerticalVelocityInDecimalThousands)
            {
                rdoVertVelocityInThousands.Checked = true;
                rdoVertVelocityInUnitFeet.Checked = false;
            }
            else
            {
                rdoVertVelocityInThousands.Checked = false;
                rdoVertVelocityInUnitFeet.Checked = true;
            }
            UpdateWindowsStartupRegKey();
        }

        private void rdoClientMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoClientMode.Checked)
            {
                txtServerIPAddress.Enabled = true;
                lblServerIPAddress.Enabled = true;
                txtServerPortNum.Enabled = true;
                lblPortNum.Enabled = true;
                grpPFDOptions.Enabled = true;
                Settings.Default.RunAsClient = true;
                Settings.Default.RunAsServer = false;
                F16CPD.Util.SaveCurrentProperties();
            }
        }

        private void rdoServerMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoServerMode.Checked)
            {
                txtServerIPAddress.Enabled = false;
                lblServerIPAddress.Enabled = false;
                txtServerPortNum.Enabled = true;
                lblPortNum.Enabled = true;
                grpPFDOptions.Enabled = false;
                cmdRescueOutputWindow.Enabled = false;
                Settings.Default.RunAsServer = true;
                Settings.Default.RunAsClient = false;
                F16CPD.Util.SaveCurrentProperties();
            }
        }

        private void MinimizeToSystemTray()
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            nfyTrayIcon.Visible = true;
            Hide();
        }

        private void RestoreFromSystemTray()
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            nfyTrayIcon.Visible = false;
            Show();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void Stop()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            btnStop.Enabled = false;
            cmdRescueOutputWindow.Enabled = false;
            mnuNfyStop.Enabled = false;
            if (_cpdEngine != null)
            {
                _cpdEngine.Stop();
                Common.Util.DisposeObject(_cpdEngine);
            }
            if (!Settings.Default.RunAsServer)
            {
                grpPFDOptions.Enabled = true;
                cbOutputRotation.Enabled = true;
            }

            gbNetworkingOptions.Enabled = true;
            gbStartupOptions.Enabled = true;
            gbPerformanceOptions.Enabled = true;

            cmdAssignInputs.Enabled = true;
            btnStart.Enabled = true;
            mnuNfyStart.Enabled = true;
            lblRotation.Enabled = true;
        }

        private void Start()
        {
            Thread.CurrentThread.Priority = Settings.Default.Priority;
            if (rdoClientMode.Checked)
            {
                var validIpAddress = false;
                IPAddress address = null;
                validIpAddress = IPAddress.TryParse(txtServerIPAddress.Text, out address);
                if (String.IsNullOrEmpty(txtServerIPAddress.Text.Trim()) || !validIpAddress)
                {
                    MessageBox.Show("Please enter a valid IP address for the " + Application.ProductName + " server.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RestoreFromSystemTray();
                    txtServerIPAddress.Focus();
                    return;
                }
                if (String.IsNullOrEmpty(txtServerPortNum.Text.Trim()))
                {
                    MessageBox.Show("Please enter the port number of the " + Application.ProductName + " server.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RestoreFromSystemTray();
                    txtServerPortNum.Focus();
                    return;
                }
            }
            if (rdoServerMode.Checked)
            {
                if (String.IsNullOrEmpty(txtServerPortNum.Text.Trim()))
                {
                    MessageBox.Show(
                        "Please enter the port number to publish the " + Application.ProductName + " service on.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RestoreFromSystemTray();
                    txtServerPortNum.Focus();
                    return;
                }
            }

            var portNum = 21153;
            Int32.TryParse(Settings.Default.ServerPortNum, out portNum);
            btnStart.Enabled = false;
            mnuNfyStart.Enabled = false;
            if (_cpdEngine != null)
            {
                _cpdEngine.Stop();
                Common.Util.DisposeObject(_cpdEngine);
            }
            btnStart.Enabled = false;
            mnuNfyStart.Enabled = false;
            _cpdEngine = new F16CpdEngine();
            lblRotation.Enabled = false;
            cbOutputRotation.Enabled = false;
            gbPerformanceOptions.Enabled = false;
            gbNetworkingOptions.Enabled = false;
            gbStartupOptions.Enabled = false;
            if (!Settings.Default.RunAsServer)
            {
                cmdRescueOutputWindow.Enabled = true;
            }
            cmdAssignInputs.Enabled = false;
            grpPFDOptions.Enabled = false;
            MinimizeToSystemTray();
            btnStop.Enabled = true;
            mnuNfyStop.Enabled = true;
            _cpdEngine.Start();
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MinimizeToSystemTray();
            }
            else if (WindowState != FormWindowState.Minimized)
            {
                RestoreFromSystemTray();
            }
        }

        private void mnuNfyOptions_Click(object sender, EventArgs e)
        {
            RestoreFromSystemTray();
        }

        private void mnuNfyStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void mnuNfyStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void mnuNfyExit_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Quit()
        {
            Stop();
            LogManager.Shutdown();
            try
            {
                nfyTrayIcon.Visible = false;
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                Dispose();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                Application.Exit();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Quit();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void txtServerIPAddress_Leave(object sender, EventArgs e)
        {
            Settings.Default.ServerIPAddress = txtServerIPAddress.Text;
            F16CPD.Util.SaveCurrentProperties();
        }


        private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedPriority =
                (ThreadPriority) Enum.Parse(typeof (ThreadPriority), (string) cbPriority.SelectedItem);
            Settings.Default.Priority = selectedPriority;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void nudPollFrequency_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.PollingFrequencyMillis = (int) nudPollFrequency.Value;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void txtServerPortNum_Leave(object sender, EventArgs e)
        {
            var serverPortNum = -1;
            var parsed = Int32.TryParse(txtServerPortNum.Text, out serverPortNum);
            if (!parsed || serverPortNum < 0 || serverPortNum > 65535)
            {
                MessageBox.Show("Invalid port number.  Port number must be between 0 and 65535", Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtServerPortNum.Text = Settings.Default.ServerPortNum;
                txtServerPortNum.Focus();
            }
            else
            {
                Settings.Default.ServerPortNum = txtServerPortNum.Text;
                F16CPD.Util.SaveCurrentProperties();
            }
        }

        private void txtServerPortNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = !((e.KeyChar >= '0' && e.KeyChar <= '9') ;
        }

        private void nfyTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromSystemTray();
        }

        private void rdoStandaloneMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoStandaloneMode.Checked)
            {
                txtServerIPAddress.Enabled = false;
                lblServerIPAddress.Enabled = false;
                txtServerPortNum.Enabled = false;
                lblPortNum.Enabled = false;
                grpPFDOptions.Enabled = true;
                Settings.Default.RunAsClient = false;
                Settings.Default.RunAsServer = false;
                F16CPD.Util.SaveCurrentProperties();
            }
        }

        private void cmdAssignInputs_Click(object sender, EventArgs e)
        {
            var inputsForm = new frmInputs();
            inputsForm.ShowDialog(this);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeToSystemTray();
            }
        }

        private void rdoNorth360_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.DisplayNorthAsThreeSixZero = rdoNorth360.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void rdoNorth000_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.DisplayNorthAsThreeSixZero = !rdoNorth000.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void cmdRescueOutputWindow_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this,
                                         "This will move the output window to the upper left corner of the current monitor, and will reset the window size to the default size.  Would you like to continue?",
                                         Application.ProductName, MessageBoxButtons.OKCancel,
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                var rotation = Settings.Default.Rotation;
                Size newSize;
                if (rotation == RotateFlipType.RotateNoneFlipNone || rotation == RotateFlipType.Rotate180FlipNone)
                {
                    newSize = new Size(800, 600);
                }
                else
                {
                    newSize = new Size(600, 800);
                }
                var thisScreen = Screen.FromPoint(Location);
                _cpdEngine.Location = thisScreen.WorkingArea.Location;
                _cpdEngine.Size = newSize;

                RestoreFromSystemTray();
            }
        }

        private void cbOutputRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newRotation = RotateFlipType.RotateNoneFlipNone;
            if ((string) cbOutputRotation.SelectedItem == "No rotation")
            {
                newRotation = RotateFlipType.RotateNoneFlipNone;
            }
            else if ((string) cbOutputRotation.SelectedItem == "+90 degrees")
            {
                newRotation = RotateFlipType.Rotate90FlipNone;
            }
            else if ((string) cbOutputRotation.SelectedItem == "+180 degrees")
            {
                newRotation = RotateFlipType.Rotate180FlipNone;
            }
            else if ((string) cbOutputRotation.SelectedItem == "-90 degrees")
            {
                newRotation = RotateFlipType.Rotate270FlipNone;
            }
            Settings.Default.Rotation = newRotation;

            var oldWidth = Settings.Default.CpdWindowWidth;
            var oldHeight = Settings.Default.CpdWindowHeight;
            var newWidth = oldWidth;
            var newHeight = oldHeight;

            if (newRotation == RotateFlipType.RotateNoneFlipNone || newRotation == RotateFlipType.Rotate180FlipNone)
            {
                newWidth = Math.Min(oldHeight, oldWidth);
                newHeight = Math.Max(oldHeight, oldWidth);
            }
            else
            {
                newWidth = Math.Max(oldHeight, oldWidth);
                newHeight = Math.Min(oldHeight, oldWidth);
            }
            Settings.Default.CpdWindowWidth = newWidth;
            Settings.Default.CpdWindowHeight = newHeight;

            F16CPD.Util.SaveCurrentProperties();
        }

        private void nudCourseHeadingAdjustmentSpeed_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.FastCourseAndHeadingAdjustSpeed = (int) nudCourseHeadingAdjustmentSpeed.Value;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void rdoVertVelocityInThousands_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.DisplayVerticalVelocityInDecimalThousands = rdoVertVelocityInThousands.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        private void rdoVertVelocityInUnitFeet_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.DisplayVerticalVelocityInDecimalThousands = rdoVertVelocityInThousands.Checked;
            F16CPD.Util.SaveCurrentProperties();
        }

        #region Destructors

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~frmMain()
        {
            if (nfyTrayIcon != null)
            {
                nfyTrayIcon.Visible = false;
            }
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //dispose of managed resources here
                    Common.Util.DisposeObject(_cpdEngine);
                }
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;

        }

        #endregion
    }
}