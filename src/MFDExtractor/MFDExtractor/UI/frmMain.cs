using System;
using System.Windows.Forms;
using MFDExtractor.Properties;
using log4net;

namespace MFDExtractor.UI
{
    /// <summary>
    ///     Code-behind for the MFD Extractor's main form
    /// </summary>
    public partial class frmMain : Form
    {
        /// <summary>
        ///     Flag to indicate that the form is closing; allows cleanup methods to detect this fact when called
        ///     in multiple contexts
        /// </summary>
        private bool _isClosing;

        /// <summary>
        ///     Reference to an instance of the Options form
        /// </summary>
        private frmOptions _optionsForm;

        /// <summary>
        ///     Default constructor for the form
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }



        /// <summary>
        ///     Event handler for the Form's Load event
        /// </summary>
        /// <param name="sender">The object raising this event</param>
        /// <param name="e">Event arguments for the Form's Load event</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            Visible = false; //hide the form, initially
            Width = 0;
            Height = 0;

            //set the form's titlebar equal to the app's name as defined in the app properties file
            Text = Application.ProductName + @" v" + Application.ProductVersion;
            nfyTrayIcon.Text = Text;
            //disable checks for illegal cross-thread calls, as there are some code chunks that do
            //directly update the UI thread 
            //TODO: eliminate the need for this "crutch" by properly implementing non-UI thread interaction
            //with the UI thread via calls to .Invoke
            CheckForIllegalCrossThreadCalls = false;

            //set up initial menu item enabled/states for the context menu (tray menu) items
            mnuCtxStart.Enabled = true;
            mnuCtxStop.Enabled = false;

            //load the application's settings from the config file
            Settings settings = Settings.Default;

            //configure an instance of the main Extractor engine 
            Extractor extractor = Extractor.GetInstance();

            //register for Extractor events
            extractor.Stopped += extractor_Stopped;
            extractor.Starting += extractor_Starting;
            extractor.Started += extractor_Started;
            extractor.Stopping += extractor_Stopping;

            //if we're supposed to automatically start the Extractor (per the user-config settings), start it
            //now
            if (settings.StartOnLaunch)
            {
                extractor.Start();
            }
        }


        /// <summary>
        ///     Event handler for the Extractor engine's Stopping event
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Event arguments for the Stopping event</param>
        private void extractor_Stopping(object sender, EventArgs e)
        {
            //while the Extractor is stopping, disable the ability to Start or Stop the Extractor
            //via the tray icon menu
            mnuCtxStart.Enabled = false;
            mnuCtxStop.Enabled = false;
        }

        /// <summary>
        ///     Event handler for the Extractor engine's Started event
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Event arguments for the Started event</param>
        private void extractor_Started(object sender, EventArgs e)
        {
            //when the Extractor engine has fully started, then enable the Stop action item on 
            //the MFD Extractor's system tray icon menu
            mnuCtxStart.Enabled = false;
            mnuCtxStop.Enabled = true;
        }

        /// <summary>
        ///     Event handler for the Extractor engine's Starting event
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Event arguments for the Starting event</param>
        private void extractor_Starting(object sender, EventArgs e)
        {
            //While the Extractor engine is Starting up, disable any Start or Stop actions on the
            //MFD Extractor's system tray icon menu
            mnuCtxStart.Enabled = false;
            mnuCtxStop.Enabled = false;
        }

        /// <summary>
        ///     Event handler for the Extractor engine's Stopped event
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Event arguments for the Stopped event</param>
        private void extractor_Stopped(object sender, EventArgs e)
        {
            //When the Extractor engine has fully stopped, re-enable the Start action menu item on 
            //the MFD Extractor's system tray icon menu
            mnuCtxStart.Enabled = true;
            mnuCtxStop.Enabled = false;
        }

        /// <summary>
        ///     Event handler for the Form's Closed event
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Event arguments for the Form's Closed event</param>
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //When the user closes the MFD Extractor's main form (i.e. clicks the Exit action menu item
            //on the system tray icon's menu); then we need to inform the keyboard polling thread to quit,
            //and then unload the main form.  When all active threads have quit, the program will be unloaded
            //from memory.
            _isClosing = true;
            Quit();
        }

        /// <summary>
        ///     Perform an orderly shutdown of the MFD Extractor.
        /// </summary>
        private void Quit()
        {
            //Stop the Extractor engine
            Extractor.GetInstance().Stop();
            Extractor.DisposeInstance();
            //remove the applicaton's tray icon from the tray
            nfyTrayIcon.Visible = false;
            //Close the application's main form
            if (!_isClosing)
            {
                Close();
            }
        }

        /// <summary>
        ///     Starts the MFD Extractor engine
        /// </summary>
        private static void Start()
        {
            //Start the MFD Extractor engine
            Extractor.GetInstance().Start();
        }

        /// <summary>
        ///     Stops the MFD Extractor engine
        /// </summary>
        private static void Stop()
        {
            //Stop the MFD Extractor engine
            Extractor.GetInstance().Stop();
        }

        /// <summary>
        ///     Display the application's Options dialog box
        /// </summary>
        private void ShowOptionsDialog()
        {
            if (_optionsForm == null)
            {
                Extractor extractor = Extractor.GetInstance(); //get a reference to the main Extractor engine
                mnuCtxExit.Enabled = false; //disable the Exit action on the system tray icon's menu
                //Stop();

                //display the application's Options form to the user
                _optionsForm = new frmOptions();
                _optionsForm.ShowDialog(this);

                extractor.LoadSettings();
                //ask the Extractor engine to refresh its settings from the application's user-config file

                //re-enable the Exit action on the system tray icon's menu
                mnuCtxExit.Enabled = true;

                //free the memory used by the reference to the Options form
                _optionsForm = null;
            }
            else
            {
                //if the Options form is already being displayed, just bring it to the front
                _optionsForm.BringToFront();
            }
        }

        /// <summary>
        ///     Event handler for the system tray icon's Options menu item's Click event
        /// </summary>
        /// <param name="sender">Object that raised this event</param>
        /// <param name="e">Event handler for the menu item's Click event</param>
        private void mnuCtxOptions_Click(object sender, EventArgs e)
        {
            //show the Options form to the user
            ShowOptionsDialog();
        }

        /// <summary>
        ///     Event handler for the system tray icon's Exit menu item's Click event
        /// </summary>
        /// <param name="sender">Object that raised this event</param>
        /// <param name="e">Event handler for the menu item's Click event</param>
        private void mnuCtxExit_Click(object sender, EventArgs e)
        {
            //Shut down the application
            Quit();
        }

        /// <summary>
        ///     Event handler for the system tray icon's Start menu item's Click event
        /// </summary>
        /// <param name="sender">Object that raised this event</param>
        /// <param name="e">Event handler for the menu item's Click event</param>
        private void mnuCtxStart_Click(object sender, EventArgs e)
        {
            //Start the Extractor engine
            Start();
        }

        /// <summary>
        ///     Event handler for the system tray icon's Stop menu item's Click event
        /// </summary>
        /// <param name="sender">Object that raised this event</param>
        /// <param name="e">Event arguments for the menu item's Click event</param>
        private void mnuCtxStop_Click(object sender, EventArgs e)
        {
            //Stop the Extractor engine
            Stop();
        }
    }
}