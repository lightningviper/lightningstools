#region Using statements

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using MFDExtractor.Properties;
using MFDExtractor.UI;
using log4net;

#endregion

namespace MFDExtractor
{
    /// <summary>
    ///     Main program class.  Contains the startup method for the application.
    /// </summary>
    public static class Program
    {
        #region Class variable declarations

        // private members
        private static Form _mainForm;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        #endregion

        #region Static methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null ||
                (e.ExceptionObject is ThreadAbortException || e.ExceptionObject is ThreadInterruptedException)) return;
            try
            {
                Log.Error(((Exception) e.ExceptionObject).Message, (Exception) e.ExceptionObject);
            }
            catch { }
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            if (t !=null && t.Exception != null && !(t.Exception is ThreadAbortException || t.Exception is ThreadInterruptedException))
            {
                try
                {
                    Log.Error(t.Exception.Message, t.Exception);
                }
                catch { }
            }
        }

        private static Process PriorProcess()
            // Returns a System.Diagnostics.Process pointing to
            // a pre-existing process with the same name as the
            // current one, if any; or null if the current process
            // is unique.
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                    return p;
            }
            return null;
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (PriorProcess() != null)
            {
                return;
            }

            Common.Win32.NativeMethods.SetProcessDpiAwareness(Common.Win32.NativeMethods.PROCESS_DPI_AWARENESS.Process_System_DPI_Aware);
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += UIThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException +=
                CurrentDomain_UnhandledException;
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            _mainForm = new frmMain();
            Thread.CurrentThread.Name = "MainThread";
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
                    Log.Error(e.Message, e);
                }
            }
            Application.Run(_mainForm);
        }

        #endregion
    }
}