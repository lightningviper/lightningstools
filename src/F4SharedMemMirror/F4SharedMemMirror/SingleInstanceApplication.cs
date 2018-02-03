#region Using statements

using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

#endregion

namespace F4SharedMemMirror
{
    /// <summary>
    ///   This class ensures that only a single instance of this application is 
    ///   ever created.
    /// </summary>
    internal sealed class SingleInstanceApplication : WindowsFormsApplicationBase
    {
        #region Constructors

        /// <summary>
        ///   Constructor that intializes the authentication mode for this app.
        /// </summary>
        /// <param name = "mode">Mode in which to run app.</param>
        internal SingleInstanceApplication(AuthenticationMode mode)
            : base(mode)
        {
            InitializeAppProperties();
        }

        /// <summary>
        ///   Default constructor.
        /// </summary>
        internal SingleInstanceApplication()
        {
            InitializeAppProperties();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///   Initializes this application with the appropriate settings.
        /// </summary>
        internal void InitializeAppProperties()
        {
            IsSingleInstance = false;
            EnableVisualStyles = false;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///   Runs the specified mainForm in this application context.
        /// </summary>
        /// <param name = "mainForm">Form that is run.</param>
        internal void Run(Form mainForm)
        {
            // set up the main form.
            MainForm = mainForm;

            // then, run the the main form.
            Run(CommandLineArgs);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Runs this.MainForm in this application context. Converts the command
        ///   line arguments correctly for the base this.Run method.
        /// </summary>
        /// <param name = "commandLineArgs">Command line collection.</param>
        private void Run(ReadOnlyCollection<string> commandLineArgs)
        {
            // convert the Collection<string> to string[], so that it can be used
            // in the Run method.
            var list = new ArrayList(commandLineArgs);
            var commandLine = (string[]) list.ToArray(typeof (string));
            Run(commandLine);
        }

        #endregion
    }
}