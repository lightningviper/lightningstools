using System;
using System.ComponentModel;
using System.Windows.Forms;
using SimLinkup.Properties;

namespace SimLinkup.UI
{
    public partial class frmMain : Form
    {
        public static Runtime.Runtime SharedRuntime;

        public frmMain()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SharedRuntime != null && SharedRuntime.IsRunning)
            {
                Stop();
            }
            base.OnClosing(e);
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            ToolsOptions();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }


        private static void DisposeRuntime()
        {
            if (SharedRuntime == null) return;
            Common.Util.DisposeObject(SharedRuntime);
            SharedRuntime = null;
        }

        private void FileExit()
        {
            Stop();
            Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            DisposeRuntime();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            nfyTrayIcon.Text = Application.ProductName;
            Text = Application.ProductName + " v" + Application.ProductVersion;
            SharedRuntime = new Runtime.Runtime();
            PopulateSignalsView();
            if (Settings.Default.StartRunningWhenLaunched)
            {
                Start();
            }
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized) return;
            if (Settings.Default.MinimizeToSystemTray)
            {
                MinimizeToTray();
            }
        }

        private void HelpAbout()
        {
            using (var helpAbout = new HelpAbout())
            {
                helpAbout.ShowDialog(this);
            }
        }

        private void MinimizeToTray()
        {
            nfyTrayIcon.Visible = true;
            WindowState = FormWindowState.Minimized;
            Visible = false;
            ShowInTaskbar = false;
        }

        private void mnuActionsStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void mnuActionsStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            FileExit();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            HelpAbout();
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
            ToolsOptions();
        }

        private void mnuTrayExit_Click(object sender, EventArgs e)
        {
            FileExit();
        }

        private void mnuTrayOptions_Click(object sender, EventArgs e)
        {
            ToolsOptions();
        }

        private void mnuTrayRestore_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void mnuTrayStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void mnuTrayStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void nfyTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                RestoreFromTray();
            }
        }

        private void PopulateSignalsView()
        {
            signalsView.ScriptingContext = SharedRuntime.ScriptingContext;
            signalsView.Signals = SharedRuntime.ScriptingContext.AllSignals;
            signalsView.UpdateContents();
        }

        private void RestoreFromTray()
        {
            Visible = true;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            nfyTrayIcon.Visible = false;
        }

        private void Start()
        {
            btnStop.Enabled = true;
            mnuActionsStop.Enabled = true;
            mnuTrayStop.Enabled = true;

            mnuToolsOptions.Enabled = false;
            btnOptions.Enabled = false;
            mnuTrayOptions.Enabled = false;

            btnStart.Enabled = false;
            mnuActionsStart.Enabled = false;
            mnuTrayStart.Enabled = false;

            if (Settings.Default.MinimizeWhenStarted)
            {
                WindowState = FormWindowState.Minimized;
            }

            SharedRuntime.Start();
        }

        private void Stop()
        {
            if (SharedRuntime != null && SharedRuntime.IsRunning)
            {
                SharedRuntime.Stop();
            }
            btnStop.Enabled = false;
            mnuActionsStop.Enabled = false;
            mnuTrayStop.Enabled = false;

            mnuToolsOptions.Enabled = true;
            mnuTrayOptions.Enabled = true;
            btnOptions.Enabled = true;

            mnuActionsStart.Enabled = true;
            btnStart.Enabled = true;
            mnuTrayStart.Enabled = true;
        }

        private void ToolsOptions()
        {
            using (var frmOptions = new frmOptions())
            {
                frmOptions.ShowDialog(this);
            }
        }

        private void mnuViewUpdateInRealtime_Click(object sender, EventArgs e)
        {
            mnuViewUpdateInRealtime.Checked = !mnuViewUpdateInRealtime.Checked;
            signalsView.UpdateInRealtime = mnuViewUpdateInRealtime.Checked;
        }
    }
}