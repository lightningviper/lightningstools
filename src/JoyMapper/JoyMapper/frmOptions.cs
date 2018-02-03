using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using JoyMapper.Properties;
using log4net;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace JoyMapper
{
    internal sealed partial class frmOptions : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(frmOptions));
        private bool _cancelClose;
        private OpenFileDialog fileBrowseDialog;

        public frmOptions()
        {
            InitializeComponent();
        }

        private void chkLoadDefaultMappingFile_CheckedChanged(object sender, EventArgs e)
        {
            cmdBrowse.Enabled = chkLoadDefaultMappingFile.Checked;
            txtDefaultMappingFile.Enabled = chkLoadDefaultMappingFile.Checked;
            chkStartMappingOnProgramLaunch.Enabled = chkLoadDefaultMappingFile.Checked;
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            fileBrowseDialog.ShowDialog();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            var set = Settings.Default;
            if (chkLoadDefaultMappingFile.Checked)
            {
                if (string.IsNullOrEmpty(txtDefaultMappingFile.Text))
                {
                    errorProvider1.SetError(txtDefaultMappingFile,
                        "Please select a mapping to load on startup, or disable auto-loading.");
                    _cancelClose = true;
                    return;
                }
                var fileName = txtDefaultMappingFile.Text;
                var fi = new FileInfo(fileName);
                if (!fi.Exists)
                {
                    errorProvider1.SetError(txtDefaultMappingFile,
                        "The mapping file selected does not exist. Please select a valid mapping file to load on startup, or disable auto-loading.");
                    _cancelClose = true;
                    return;
                }
            }
            set.DefaultMappingFile = txtDefaultMappingFile.Text;
            set.MinimizeOnMappingStart = chkMinimizeToTrayWhenMapping.Checked;
            set.StartMappingOnLaunch = chkStartMappingOnProgramLaunch.Checked;
            set.LoadDefaultMappingFile = chkLoadDefaultMappingFile.Checked;
            set.EnableAutoHighlighting = chkAutoHighlighting.Checked;
            set.PollEveryNMillis = (int) nudPollingPeriod.Value;
            set.Save();
            if (chkLaunchAtStartup.Checked)
            {
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
        }

        private void fileBrowseDialog_FileOk(object sender, CancelEventArgs e)
        {
            var fileName = fileBrowseDialog.FileName;
            txtDefaultMappingFile.Text = fileName;
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _cancelClose;
            _cancelClose = false;
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            fileBrowseDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                RestoreDirectory = false,
                ShowHelp = false,
                ShowReadOnly = false,
                SupportMultiDottedExtensions = true,
                Title = @"Select default mappings file",
                ValidateNames = true,
                Filter = @"JoyMapper mapping files (*.map)|*.map|All files (*.*)|*.*"
            };
            fileBrowseDialog.FileOk += fileBrowseDialog_FileOk;


            var set = Settings.Default;
            chkStartMappingOnProgramLaunch.Checked = set.StartMappingOnLaunch;
            chkMinimizeToTrayWhenMapping.Checked = set.MinimizeOnMappingStart;
            txtDefaultMappingFile.Text = set.DefaultMappingFile;
            chkLoadDefaultMappingFile.Checked = set.LoadDefaultMappingFile;
            chkAutoHighlighting.Checked = set.EnableAutoHighlighting;
            cmdBrowse.Enabled = chkLoadDefaultMappingFile.Checked;
            txtDefaultMappingFile.Enabled = chkLoadDefaultMappingFile.Checked;
            chkStartMappingOnProgramLaunch.Enabled = chkLoadDefaultMappingFile.Checked;

            nudPollingPeriod.Value = Settings.Default.PollEveryNMillis;
            var startupKeyValue = string.Empty;
            var c = new Computer();
            try
            {
                using (
                    var startupKey =
                        c.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    if (startupKey != null)
                        startupKeyValue = (string) startupKey.GetValue(Application.ProductName, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex.Message, ex);
            }

            chkLaunchAtStartup.Checked = !string.IsNullOrEmpty(startupKeyValue);
        }
    }
}