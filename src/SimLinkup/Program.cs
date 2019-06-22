using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using log4net;
using SimLinkup.Properties;
using SimLinkup.UI;

namespace SimLinkup
{
    public static class Program
    {
        private static Form mainForm;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        public static void Main()
        {
            if (PriorProcess() != null)
            {
                return;
            }
            try
            {
                Common.Win32.NativeMethods.SetProcessDpiAwareness(Common.Win32.NativeMethods.PROCESS_DPI_AWARENESS.Process_System_DPI_Aware);
            }
            catch { }

            Application.ThreadException += UIThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            mainForm = new frmMain();
            Thread.CurrentThread.Name = "MainThread";
            Application.EnableVisualStyles();

            if (Settings.Default.UpgradeNeeded)
            {
                try
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeNeeded = false;
                    Settings.Default.Save();
                }
                catch (Exception e)
                {
                    Settings.Default.Reset();
                    Settings.Default.UpgradeNeeded = false;
                    Settings.Default.Save();
                    MessageBox.Show(
                        "Error: Could not import settings from previous installation of " + Application.ProductName +
                        ".\nThis can happen if the configuration file was incorrectly edited by hand.\nDefault settings will be used instead.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                    _log.Error(e.Message, e);
                }
            }
            Application.Run(mainForm);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error(e.ExceptionObject.ToString(), (Exception) e.ExceptionObject);
        }

        private static Process PriorProcess()
        {
            var curr = Process.GetCurrentProcess();
            var procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (var p in procs)
                if (p.Id != curr.Id &&
                    p.MainModule.FileName == curr.MainModule.FileName)
                {
                    return p;
                }
            return null;
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            _log.Error(t.Exception.Message, t.Exception);
        }
    }
}