using System;
using System.Windows.Forms;
using log4net;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using SimLinkup.Properties;

namespace SimLinkup.UI
{
    public partial class frmOptions : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(frmOptions));

        public frmOptions()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var valid = ValidateSettings();
            if (valid)
            {
                SaveSettings();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void DiscoverPlugins()
        {
            hardwareSupportModuleList.HardwareSupportModules = Runtime.Runtime.GetRegisteredHardwareSupportModules();
            simSupportModuleList.SimSupportModules = Runtime.Runtime.GetRegisteredSimSupportModules();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " v" + Application.ProductVersion + " Options";
            LoadSettings();
            DiscoverPlugins();
        }

        private void LoadSettings()
        {
            chkLaunchAtSystemStartup.Checked = Settings.Default.LaunchAtWindowsStartup;
            chkMinimizeToSystemTray.Checked = Settings.Default.MinimizeToSystemTray;
            chkMinimizeWhenStarted.Checked = Settings.Default.MinimizeWhenStarted;
            chkStartAutomaticallyWhenLaunched.Checked = Settings.Default.StartRunningWhenLaunched;
        }

        private void SaveSettings()
        {
            Settings.Default.StartRunningWhenLaunched = chkStartAutomaticallyWhenLaunched.Checked;
            Settings.Default.MinimizeToSystemTray = chkMinimizeToSystemTray.Checked;
            Settings.Default.MinimizeWhenStarted = chkMinimizeWhenStarted.Checked;
            UpdateWindowsStartupRegKey();
            Settings.Default.Save();
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
                        startupKey?.SetValue(Application.ProductName, Application.ExecutablePath,
                            RegistryValueKind.String);
                    }
                }
                catch (Exception ex)
                {
                    _log.Debug(ex.Message, ex);
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
                        startupKey?.DeleteValue(Application.ProductName, false);
                    }
                }
                catch (Exception ex)
                {
                    _log.Debug(ex.Message, ex);
                }
            }
            Settings.Default.LaunchAtWindowsStartup = chkLaunchAtSystemStartup.Checked;
            Settings.Default.Save();
        }

        private static bool ValidateSettings()
        {
            return true;
        }
    }
}