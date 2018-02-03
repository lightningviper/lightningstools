using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using F4SharedMemMirror.Properties;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace F4SharedMemMirror
{
    public partial class frmMain : Form
    {
        private Mirror _mirror;

        public frmMain()
        {
            InitializeComponent();
        }

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
                        startupKey.SetValue(Application.ProductName, Application.ExecutablePath,
                                            RegistryValueKind.String);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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
                        startupKey.DeleteValue(Application.ProductName, false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            Settings.Default.LaunchAtWindowsStartup = chkLaunchAtSystemStartup.Checked;
            Settings.Default.Save();
        }

        private void chkStartMirroringWhenLaunched_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.StartMirroringWhenLaunched = chkStartMirroringWhenLaunched.Checked;
            Settings.Default.Save();
        }

        private void chkMinimizeToSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.MinimizeToSystemTray = chkMinimizeToSystemTray.Checked;
            Settings.Default.Save();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: " + Application.ProductVersion;
            nfyTrayIcon.Icon = Icon;
            foreach (var priority in Enum.GetNames(typeof (ThreadPriority)))
            {
                if (priority.ToLowerInvariant() != "highest")
                {
                    cbPriority.Items.Add(priority);
                }
            }
            LoadSettings();
            if (chkStartMirroringWhenLaunched.Checked)
            {
                StartMirroring();
            }
        }

        private void LoadSettings()
        {
            Settings.Default.Reload();
            chkMinimizeToSystemTray.Checked = Settings.Default.MinimizeToSystemTray;
            chkStartMirroringWhenLaunched.Checked = Settings.Default.StartMirroringWhenLaunched;
            chkLaunchAtSystemStartup.Checked = Settings.Default.LaunchAtWindowsStartup;
            chkRunMinimized.Checked = Settings.Default.RunMinimized;
            rdoClientMode.Checked = Settings.Default.RunAsClient;
            rdoServerMode.Checked = Settings.Default.RunAsServer;
            txtServerIPAddress.Text = Settings.Default.ServerIPAddress;
            txtServerPortNum.Text = Settings.Default.ServerPortNum;
            nudPollFrequency.Value = Settings.Default.PollingFrequencyMillis;
            cbPriority.SelectedItem = Enum.GetName(typeof (ThreadPriority), Settings.Default.Priority);

            UpdateWindowsStartupRegKey();
        }

        private void rdoClientMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoClientMode.Checked)
            {
                txtServerIPAddress.Enabled = true;
                txtServerPortNum.Enabled = true;
                Settings.Default.RunAsClient = rdoClientMode.Checked;
                Settings.Default.RunAsServer = !rdoClientMode.Checked;
                Settings.Default.Save();
            }
        }

        private void rdoServerMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoServerMode.Checked)
            {
                txtServerIPAddress.Enabled = false;
                Settings.Default.RunAsServer = rdoServerMode.Checked;
                Settings.Default.RunAsClient = !rdoServerMode.Checked;
                Settings.Default.Save();
            }
        }

        private void MinimizeToSystemTray()
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            nfyTrayIcon.Visible = true;
        }

        private void RestoreFromSystemTray()
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            nfyTrayIcon.Visible = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartMirroring();
        }

        private void StopMirroring()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            btnStop.Enabled = false;
            mnuNfyStopMirroring.Enabled = false;
            if (_mirror != null)
            {
                _mirror.StopMirroring();
                try
                {
                    _mirror.Dispose();
                    _mirror = null;
                }
                catch
                {
                }
            }
            gbNetworkingOptions.Enabled = true;
            gbGeneralOptions.Enabled = true;
            gbPerformanceOptions.Enabled = true;
            btnStart.Enabled = true;
            mnuNfyStartMirroring.Enabled = true;
        }

        private void StartMirroring()
        {
            Thread.CurrentThread.Priority = Settings.Default.Priority;
            IPAddress address = null;
            if (rdoClientMode.Checked)
            {
                var validIpAddress = false;
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
            var portNum = 21142;
            Int32.TryParse(Settings.Default.ServerPortNum, out portNum);
            btnStart.Enabled = false;
            mnuNfyStartMirroring.Enabled = false;
            if (_mirror != null)
            {
                _mirror.StopMirroring();
            }
            btnStart.Enabled = false;
            mnuNfyStartMirroring.Enabled = false;
            _mirror = new Mirror();
            if (rdoClientMode.Checked)
            {
                _mirror.NetworkingMode = NetworkingMode.Client;
                _mirror.ClientIPAddress = address;
            }
            else if (rdoServerMode.Checked)
            {
                _mirror.NetworkingMode = NetworkingMode.Server;
            }
            _mirror.PortNumber = (ushort) portNum;

            gbPerformanceOptions.Enabled = false;
            gbNetworkingOptions.Enabled = false;
            gbGeneralOptions.Enabled = false;
            if (chkRunMinimized.Checked)
            {
                if (chkMinimizeToSystemTray.Checked)
                {
                    MinimizeToSystemTray();
                }
                else
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
            btnStop.Enabled = true;
            mnuNfyStopMirroring.Enabled = true;
            _mirror.StartMirroring();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                if (chkMinimizeToSystemTray.Checked)
                {
                    MinimizeToSystemTray();
                }
            }
            else if (WindowState != FormWindowState.Minimized)
            {
                RestoreFromSystemTray();
            }
        }

        private void mnuNfyRestore_Click(object sender, EventArgs e)
        {
            RestoreFromSystemTray();
        }

        private void mnuNfyStopMirroring_Click(object sender, EventArgs e)
        {
            StopMirroring();
        }

        private void mnuNfyStartMirroring_Click(object sender, EventArgs e)
        {
            StartMirroring();
        }

        private void mnuNfyExit_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Quit()
        {
            nfyTrayIcon.Visible = false;
            Application.Exit();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Quit();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMirroring();
        }

        private void txtServerIPAddress_Leave(object sender, EventArgs e)
        {
            Settings.Default.ServerIPAddress = txtServerIPAddress.Text;
            Settings.Default.Save();
        }

        private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedPriority =
                (ThreadPriority) Enum.Parse(typeof (ThreadPriority), (string) cbPriority.SelectedItem);
            Settings.Default.Priority = selectedPriority;
            Settings.Default.Save();
        }

        private void nudPollFrequency_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.PollingFrequencyMillis = (int) nudPollFrequency.Value;
            Settings.Default.Save();
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
                Settings.Default.Save();
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

        private void chkRunMinimized_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.RunMinimized = chkRunMinimized.Checked;
            Settings.Default.Save();
        }
    }
}