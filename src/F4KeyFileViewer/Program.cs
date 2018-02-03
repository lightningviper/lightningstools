#region Using statements

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Common.Application;
using F4KeyFileViewer.Properties;
using log4net;
using Microsoft.VisualBasic.ApplicationServices;
using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

#endregion

namespace F4KeyFileViewer
{
    /// <summary>
    ///   Main program class.  Contains the startup method for the F4 KeyFile Viewer application.
    /// </summary>
    public static class Program
    {
        #region Class variable declarations

        // private members
        private static Form mainForm;
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        #endregion

        #region Static methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException((Exception) e.ExceptionObject);
        }

        private static void App_UnhandledException(object sender,
                                                   Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs
                                                       e)
        {
            LogException(e.Exception);
            e.ExitApplication = false;
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            LogException(t.Exception);
        }

        private static void LogException(Exception e)
        {
            if (e is ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            else if (e is ThreadInterruptedException)
            {
                return;
            }
            _log.Error(e.Message, e);
        }

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
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
                    _log.Error(e.Message, e);
                }
            }
            // ensure only a single instance of this app runs.
            var app = new SingleInstanceApplication();
            app.StartupNextInstance += OnAppStartupNextInstance;
            app.UnhandledException += App_UnhandledException;

            mainForm = new frmMain();
            app.Run(mainForm);
        }

        /// <summary>
        ///   Event handler for processing when the another application instance tries
        ///   to startup. Bring the previous instance of the app to the front and 
        ///   process any command-line that's needed.
        /// </summary>
        /// <param name = "sender">Object sending this message.</param>
        /// <param name = "e">Event argument for this message.</param>
        private static void OnAppStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            e.BringToForeground = true;
        }

        #endregion
    }
}