#region Using statements
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using log4net;
using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

#endregion

namespace F4SharedMemViewer
{
    /// <summary>
    /// Main program class.  
    /// </summary>
    public static class Program
    {
        #region Class variable declarations

        // private members
        private static Form mainForm;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Static methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException((Exception)e.ExceptionObject);
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            LogException(t.Exception);
        }

        private static void LogException(Exception e)
        {
            _log.Error(e.Message, e);
        }

        /// <summary>
        /// The main entry point for the application.
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

            Thread.CurrentThread.Name = "MainThread";


            mainForm = new SharedMemoryViewer();
            
            Application.Run(mainForm);
        }

        
        #endregion
    }
}