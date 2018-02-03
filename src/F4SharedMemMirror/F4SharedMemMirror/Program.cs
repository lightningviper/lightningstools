#region Using statements

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

#endregion

namespace F4SharedMemMirror
{
    /// <summary>
    ///   Main program class.  Contains the startup method for the application.
    /// </summary>
    public static class Program
    {
        #region Class variable declarations

        // private members
        private static Form mainForm;

        #endregion

        #region Static methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //MessageBox.Show(((Exception)e.ExceptionObject).ToString());
            Debug.WriteLine(e.ExceptionObject.ToString());
            //do nothing
            //TODO: log exceptions, don't show them
        }

        private static void App_UnhandledException(object sender,
                                                   Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs
                                                       e)
        {
            //MessageBox.Show(e.Exception.ToString());
            Debug.WriteLine(e.Exception.ToString());
            e.ExitApplication = false;
            //TODO: log exceptions, don't show them
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            //MessageBox.Show(t.Exception.ToString());
            //do nothing
            Debug.WriteLine(t.Exception.ToString());
            //TODO: log exceptions, don't show them
        }

        private static Process PriorProcess()
            // Returns a System.Diagnostics.Process pointing to
            // a pre-existing process with the same name as the
            // current one, if any; or null if the current process
            // is unique.
        {
            var curr = Process.GetCurrentProcess();
            var procs = Process.GetProcessesByName(curr.ProcessName);
            return procs.FirstOrDefault(p => (p.Id != curr.Id) && (p.MainModule.FileName == curr.MainModule.FileName));
        }

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (PriorProcess() != null)
            {
                return;
            }
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += UIThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException +=
                CurrentDomain_UnhandledException;

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
            // if the window is currently minimized, then restore it.
            if (mainForm.WindowState == FormWindowState.Minimized)
            {
                mainForm.WindowState = FormWindowState.Normal;
            }

            // activate the current instance of the app, so that it's shown.
            mainForm.Activate();
        }

        #endregion
    }
}