using System;
using System.Collections.Generic;
using Common.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Common.InputSupport.UI;
using MFDExtractor.Configuration;
using MFDExtractor.Properties;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using log4net;
using Common.Networking;
using LightningGauges.Renderers.F16;
using LightningGauges.Renderers.F16.AzimuthIndicator;

namespace MFDExtractor.UI
{
    public enum VVIStyles
    {
        Tape,
        Needle
    }

    /// <summary>
    ///     Code-behind for the Options form
    /// </summary>
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
        }
        private static readonly ILog _log = LogManager.GetLogger(typeof (frmOptions));

        /// <summary>
        ///     the current Extractor engine instance
        /// </summary>
        private readonly Extractor _extractor = Extractor.GetInstance();

        /// <summary>
        ///     specifies whether the extractor should be running when the Options form exits (because
        ///     the Options form specifically stops the Extractor, so it should re-start it if
        ///     it was already started prior to entering the Options form)
        /// </summary>
        private bool _extractorRunningStateOnFormOpen;
        private bool _formLoading = true;

        private bool _loadingSettings = false;
        private bool _savingSettings = false;
        private bool _settingsChanging = false;
        /// <summary>
        ///     Event handler for the form's Load event
        /// </summary>
        /// <param name="sender">the object raising this event</param>
        /// <param name="e">Event arguments for the form's Load event</param>
        private void frmOptions_Load(object sender, EventArgs e)
        {
            _extractorRunningStateOnFormOpen = Extractor.State.Running; //store current running
            //state of the Extractor engine
            //stop the Extractor engine
			if (Extractor.State.Running)
            {
                _extractor.Stop();
            }

            //put the Extractor into Test mode (displays the Test/Blank images)
			Extractor.State.OptionsFormIsShowing = true;
            //set the titlebar for the Options form
            Text = Application.ProductName + " v" + Application.ProductVersion + " Options";

            cboImageFormat.Items.Add("BMP");
            cboImageFormat.Items.Add("GIF");
            cboImageFormat.Items.Add("JPEG");
            cboImageFormat.Items.Add("PNG");
            cboImageFormat.Items.Add("TIFF");

            //force a reload of the user settings from the in-memory user-config
            LoadSettings();
            EnableRecoverButtons();
            _extractor.Start();
            _formLoading = false;

        }

        private void EnableRecoverButtons()
        {
            var recoverButtons =FindControls<PictureBox>(this)
               .Where(x => x.Name.IndexOf("recover", StringComparison.OrdinalIgnoreCase) > 0)
                    .ToList();

            foreach (var x in recoverButtons)
            {
                var instrumentName = x.Name.Substring(x.Name.IndexOf("recover", StringComparison.OrdinalIgnoreCase) + 7);
                x.Click += (s, e) => RecoverInstrument(instrumentName);
            }
        }

        private void RecoverInstrument(string instrumentName)
        {
            var settingsReader = new InstrumentFormSettingsReader();
            var instrumentSettings = settingsReader.Read(instrumentName);
            var size = new Rectangle(instrumentSettings.ULX, instrumentSettings.ULY, instrumentSettings.LRX - instrumentSettings.ULX, instrumentSettings.LRY-instrumentSettings.ULY);
            if (size.Width < 30) size.Width = 30;
            if (size.Height < 30) size.Height = 30;
            instrumentSettings.ULX = 0;
            instrumentSettings.ULY = 0;
            instrumentSettings.LRX = instrumentSettings.ULX + size.Width;
            instrumentSettings.LRY = instrumentSettings.ULY + size.Height;
            instrumentSettings.OutputDisplay = null;
            var settingsWriter = new InstrumentFormSettingsWriter();
            settingsWriter.Write(instrumentName, instrumentSettings);
            StopAndRestartExtractor();
        }
        public IEnumerable<T> FindControls<T>(Control control) where T : Control
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => FindControls<T>(ctrl))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == typeof(T)).Cast<T>();
        }
        private void UpdateCompressionTypeList()
        {
            cboCompressionType.Items.Clear();
            switch (cboImageFormat.SelectedItem.ToString())
            {
                case "BMP":
                    cboCompressionType.Items.Add("None");
                    cboCompressionType.Items.Add("RLE");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("RLE");
                    cboCompressionType.Enabled = true;
                    lblCompressionType.Enabled = true;
                    break;
                case "GIF":
                    cboCompressionType.Items.Add("None");
                    cboCompressionType.Items.Add("RLE");
                    cboCompressionType.Items.Add("LZW");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("LZW");
                    cboCompressionType.Enabled = true;
                    lblCompressionType.Enabled = true;
                    break;
                case "JPEG":
                    cboCompressionType.Items.Add("Implied");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("Implied");
                    cboCompressionType.Enabled = false;
                    lblCompressionType.Enabled = false;
                    break;
                case "PNG":
                    cboCompressionType.Items.Add("None");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("None");
                    cboCompressionType.Enabled = false;
                    lblCompressionType.Enabled = false;
                    break;
                case "TIFF":
                    cboCompressionType.Items.Add("None");
                    cboCompressionType.Items.Add("LZW");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("LZW");
                    cboCompressionType.Enabled = true;
                    lblCompressionType.Enabled = true;
                    break;
                default:
                    cboCompressionType.Items.Add("None");
                    cboCompressionType.SelectedIndex = cboCompressionType.FindString("");
                    cboCompressionType.Enabled = false;
                    lblCompressionType.Enabled = false;
                    break;
            }
        }
        private void SettingsChanged(object sender, EventArgs e)
        {
            if (_loadingSettings || _savingSettings || _settingsChanging) return;
            _settingsChanging = true;
            try {
                //store the currently-selected user control on the Options form (required because
                //when we reload the user settings in the next step, the currenty-selected
                //control will go out of focus
                var currentControl = ActiveControl;

                //reload user settings from the in-memory user config
                LoadSettings();

                //refocus the control that was in focus before we reloaded the user settings
                ActiveControl = currentControl;
            }
            finally
            {
                _settingsChanging = false;
            }
        }



        /// <summary>
        ///     Reloads user settings from the in-memory user settings class (Properties.Settings)
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                _loadingSettings = true;
                //unregister for settings change notifications
                Settings.Default.PropertyChanged -= SettingsChanged;
                //load all committed user settings from memory (these may not mirror the settings
                //which have been persisted to the user-config file on disk; that only happens 
                //when Properties.Settings.Default.Save() is called)
                Settings settings = Settings.Default;
                settings.UpgradeNeeded = false;

                //update the UI elements with the corresponding settings values that we just read in
                txtClientUseServerIpAddress.Text = settings.ClientUseServerIpAddress;
                txtNetworkClientUseServerPortNum.Text =
                    settings.ClientUseServerPortNum.ToString(CultureInfo.InvariantCulture);
                txtNetworkServerUsePortNum.Text = settings.ServerUsePortNumber.ToString(CultureInfo.InvariantCulture);

                //set the current Extractor instance's network mode as per the user-config
                var networkMode = (NetworkMode)settings.NetworkingMode;
                if (networkMode == NetworkMode.Client)
                {
                    EnableClientModeOptions();
                }
                else if (networkMode == NetworkMode.Server)
                {
                    EnableServerModeOptions();
                }
                else
                {
                    EnableStandaloneModeOptions();
                }


                chkEnableLeftMFD.Checked = settings.EnableLMFDOutput;
                chkEnableRightMFD.Checked = settings.EnableRMFDOutput;
                chkEnableHud.Checked = settings.EnableHudOutput;

                cmdRecoverHud.Enabled = chkEnableHud.Checked;
                cmdRecoverLeftMfd.Enabled = chkEnableLeftMFD.Checked;
                cmdRecoverRightMfd.Enabled = chkEnableRightMFD.Checked;

                chkStartOnLaunch.Checked = settings.StartOnLaunch;
                chkStartWithWindows.Checked = settings.LaunchWithWindows;

                txtPollDelay.Text = "" + settings.PollingDelay;
                cboImageFormat.SelectedIndex = cboImageFormat.FindString(settings.NetworkImageFormat);
                UpdateCompressionTypeList();
                cboCompressionType.SelectedIndex = cboCompressionType.FindString(settings.CompressionType);

                chkAzimuthIndicator.Checked = settings.EnableAzimuthIndicatorOutput;
                chkADI.Checked = settings.EnableADIOutput;
                chkStandbyADI.Checked = settings.EnableBackupADIOutput;
                chkAirspeedIndicator.Checked = settings.EnableASIOutput;
                chkAltimeter.Checked = settings.EnableAltimeterOutput;
                chkAOAIndexer.Checked = settings.EnableAOAIndexerOutput;
                chkAOAIndicator.Checked = settings.EnableAOAIndicatorOutput;
                chkCautionPanel.Checked = settings.EnableCautionPanelOutput;
                chkCMDSPanel.Checked = settings.EnableCMDSOutput;
                chkCompass.Checked = settings.EnableCompassOutput;
                chkDED.Checked = settings.EnableDEDOutput;
                chkFTIT1.Checked = settings.EnableFTIT1Output;
                chkFTIT2.Checked = settings.EnableFTIT2Output;
                chkAccelerometer.Checked = settings.EnableAccelerometerOutput;
                chkNOZ1.Checked = settings.EnableNOZ1Output;
                chkNOZ2.Checked = settings.EnableNOZ2Output;
                chkOIL1.Checked = settings.EnableOIL1Output;
                chkOIL2.Checked = settings.EnableOIL2Output;
                chkRPM1.Checked = settings.EnableRPM1Output;
                chkRPM2.Checked = settings.EnableRPM2Output;
                chkEPU.Checked = settings.EnableEPUFuelOutput;
                chkFuelFlow.Checked = settings.EnableFuelFlowOutput;
                chkISIS.Checked = settings.EnableISISOutput;
                chkFuelQty.Checked = settings.EnableFuelQuantityOutput;
                chkGearLights.Checked = settings.EnableGearLightsOutput;
                chkHSI.Checked = settings.EnableHSIOutput;
                chkEHSI.Checked = settings.EnableEHSIOutput;
                chkNWSIndexer.Checked = settings.EnableNWSIndexerOutput;
                chkPFL.Checked = settings.EnablePFLOutput;
                chkSpeedbrake.Checked = settings.EnableSpeedbrakeOutput;
                chkVVI.Checked = settings.EnableVVIOutput;
                chkHydA.Checked = settings.EnableHYDAOutput;
                chkHydB.Checked = settings.EnableHYDBOutput;
                chkCabinPress.Checked = settings.EnableCabinPressOutput;
                chkRollTrim.Checked = settings.EnableRollTrimOutput;
                chkPitchTrim.Checked = settings.EnablePitchTrimOutput;


                string azimuthIndicatorType = settings.AzimuthIndicatorType;
                var azimuthIndicatorStyle =
                    (AzimuthIndicator.InstrumentStyle)
                    Enum.Parse(typeof(AzimuthIndicator.InstrumentStyle), azimuthIndicatorType);
                switch (azimuthIndicatorStyle)
                {
                    case AzimuthIndicator.InstrumentStyle.IP1310ALR:
                        if (settings.AzimuthIndicator_ShowBezel)
                        {
                            rdoAzimuthIndicatorStyleDigital.Checked = false;
                            rdoAzimuthIndicatorHAFBezelType.Checked = false;
                            rdoATDPlus.Checked = false;
                            rdoAzimuthIndicatorNoBezel.Checked = false;
                            rdoAzimuthIndicatorIP1310BezelType.Checked = true;
                            rdoAzimuthIndicatorStyleScope.Checked = true;
                        }
                        else
                        {
                            rdoAzimuthIndicatorStyleDigital.Checked = false;
                            rdoAzimuthIndicatorIP1310BezelType.Checked = false;
                            rdoAzimuthIndicatorHAFBezelType.Checked = false;
                            rdoATDPlus.Checked = false;
                            rdoAzimuthIndicatorNoBezel.Checked = true;
                            rdoAzimuthIndicatorStyleScope.Checked = true;
                        }
                        break;
                    case AzimuthIndicator.InstrumentStyle.HAF:
                        rdoAzimuthIndicatorStyleDigital.Checked = false;
                        rdoAzimuthIndicatorIP1310BezelType.Checked = false;
                        rdoATDPlus.Checked = false;
                        rdoAzimuthIndicatorNoBezel.Checked = false;
                        rdoAzimuthIndicatorHAFBezelType.Checked = true;
                        rdoAzimuthIndicatorStyleScope.Checked = true;
                        break;
                    case AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay:
                        rdoAzimuthIndicatorStyleScope.Checked = false;
                        rdoAzimuthIndicatorIP1310BezelType.Checked = false;
                        rdoAzimuthIndicatorHAFBezelType.Checked = false;
                        rdoAzimuthIndicatorNoBezel.Checked = false;
                        rdoAzimuthIndicatorStyleDigital.Checked = true;
                        rdoATDPlus.Checked = true;
                        break;
                }

                string altimeterStyleString = Settings.Default.Altimeter_Style;
                var altimeterStyle =
                    (Altimeter.AltimeterOptions.F16AltimeterStyle)
                    Enum.Parse(typeof(Altimeter.AltimeterOptions.F16AltimeterStyle), altimeterStyleString);
                switch (altimeterStyle)
                {
                    case Altimeter.AltimeterOptions.F16AltimeterStyle.Electromechanical:
                        rdoAltimeterStyleElectromechanical.Checked = true;
                        rdoAltimeterStyleDigital.Checked = false;
                        break;
                    case Altimeter.AltimeterOptions.F16AltimeterStyle.Electronic:
                        rdoAltimeterStyleElectromechanical.Checked = false;
                        rdoAltimeterStyleDigital.Checked = true;
                        break;
                }

                string vviStyleString = Settings.Default.VVI_Style;
                var vviStyle = (VVIStyles)Enum.Parse(typeof(VVIStyles), vviStyleString);
                switch (vviStyle)
                {
                    case VVIStyles.Tape:
                        rdoVVIStyleNeedle.Checked = false;
                        rdoVVIStyleTape.Checked = true;
                        break;
                    case VVIStyles.Needle:
                        rdoVVIStyleNeedle.Checked = true;
                        rdoVVIStyleTape.Checked = false;
                        break;
                }
                grpVVIOptions.Enabled = chkVVI.Checked;
                grpAltimeterStyle.Enabled = chkAltimeter.Checked;
                grpAzimuthIndicatorStyle.Enabled = chkAzimuthIndicator.Checked;

                rdoFuelQuantityNeedleCModel.Checked = Settings.Default.FuelQuantityIndicator_NeedleCModel;
                rdoFuelQuantityDModel.Checked = !Settings.Default.FuelQuantityIndicator_NeedleCModel;

                gbFuelQuantityOptions.Enabled = chkFuelQty.Checked;
                chkHighlightOutputWindowsWhenContainMouseCursor.Checked = Settings.Default.HighlightOutputWindows;
                Settings.Default.PropertyChanged += SettingsChanged;
            }
            finally
            {
                _loadingSettings = false;
            }
        }

        /// <summary>
        ///     Update's the Form's ErrorProvider to notify the user that a user-input item has
        ///     an error.  This method is called during user-input validation in order to provide
        ///     feedback to the user, via the Form's ErrorProvider.
        /// </summary>
        /// <param name="control">
        ///     the <see cref="Control" /> that contains a user-input error.
        /// </param>
        /// <param name="errorString">
        ///     a descriptive message to display to the user when they hover
        ///     over the error symbol which the ErrorProvider places next to the <see cref="Control" /> containing
        ///     the error
        /// </param>
        private void SetError(Control control, string errorString)
        {
            //inform the Form's ErrorProvider of the error
            errControlErrorProvider.SetError(control, errorString);

            //locate the tab on which the specified Control has been placed
            Control parent = control.Parent;
            while (parent != null && parent.GetType() != typeof (TabPage))
            {
                parent = parent.Parent;
            }
            //make the errored control's parent Tab control visible
            tabAllTabs.SelectedTab = ((TabPage) parent);

            //now set focus to the errored control itself
            control.Focus();
        }

        /// <summary>
        ///     Validate all user input on all tabs
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if validation succeeds, or <see langword="false" />, if validation fails due to
        ///     a user-input error
        /// </returns>
        private bool ValidateSettings()
        {
            bool isValid = true; //start with the assumption that all user input is already valid

            errControlErrorProvider.Clear();
            //clear any errors from in the Form's ErrorProvider (leftovers from previous validation attempts)

            if (isValid && rdoServer.Checked)
            {
                int serverPortNum = -1;
                if (Int32.TryParse(txtNetworkServerUsePortNum.Text, out serverPortNum))
                {
                    if (serverPortNum < 0 || serverPortNum > 65535)
                    {
                        SetError(txtNetworkServerUsePortNum, "Must be in the range 0 to 65535");
                        isValid = false;
                    }
                }
                else
                {
                    SetError(txtNetworkServerUsePortNum, "Must be in the range 0 to 65535");
                    isValid = false;
                }
            }
            if (isValid && rdoClient.Checked)
            {
                int clientUseServerPortNum = -1;
                if (Int32.TryParse(txtNetworkClientUseServerPortNum.Text, out clientUseServerPortNum))
                {
                    if (clientUseServerPortNum < 0 || clientUseServerPortNum > 65535)
                    {
                        SetError(txtNetworkClientUseServerPortNum, "Must be in the range 0 to 65535");
                        isValid = false;
                    }
                }
                else
                {
                    SetError(txtNetworkClientUseServerPortNum, "Must be in the range 0 to 65535");
                    isValid = false;
                }
            }
            if (isValid && rdoClient.Checked)
            {
                IPAddress ipAddress;
                string serverIpAddress = txtClientUseServerIpAddress.Text;
                if (!IPAddress.TryParse(serverIpAddress, out ipAddress))
                {
                    SetError(txtClientUseServerIpAddress, "Please enter a valid IP address.");
                    isValid = false;
                }
            }

            if (isValid)
            {
                int pollDelay = -1;
                if (Int32.TryParse(txtPollDelay.Text, out pollDelay))
                {
                    if (pollDelay <= 0)
                    {
                        SetError(txtPollDelay, "Must be an integer > 0.");
                        isValid = false;
                    }
                }
                else
                {
                    SetError(txtPollDelay, "Must be an integer > 0.");
                    isValid = false;
                }
            }
            return isValid;
        }

        /// <summary>
        ///     Applies the current settings to the current Extractor instance, optionally
        ///     saving those settings to disk
        /// </summary>
        /// <param name="persist">
        ///     if <see langword="true" />, the current settings will be persisited to
        ///     the on-disk user-config file.  If <see langword="false" />, the settings will not be
        ///     persisited to disk.  In either case, the current Extractor instance will be
        ///     updated with the new settings.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if validation succeeds, or <see langword="false" />, if
        ///     validation fails due to a user-input error
        /// </returns>
        private bool ApplySettings(bool persist)
        {
            //Validate user settings
            if (!ValidateSettings())
            {
                return false;
            }
            //Commit the current settings to in-memory user-config settings storage (and persist them to 
            //the on-disk user-config file, if we're supposed to do that)
            SaveSettings(persist);
            return true; //if we've got this far, then validation was successful, so return true to indicate that
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            ValidateAndApplySettings();
        }

        /// <summary>
        ///     Event handler for the OK button's Click event
        /// </summary>
        /// <param name="sender">the object raising this event</param>
        /// <param name="args">event arguments for the OK button's Click event</param>
        private void cmdOk_Click(object sender, EventArgs args)
        {
            bool valid = ValidateAndApplySettings(Hide);
            if (valid)
            {
                CloseThisDialog();
            }
        }

        private void CloseThisDialog()
        {
            try
            {
                Close(); 
            }
            catch 
            {
            }
        }

        private bool ValidateAndApplySettings(Action methodToRunIfValidBeforeRestartingExtractor=null)
        {
            bool valid = false; //assume all user input is *invalid*
            try
            {
                valid = ApplySettings(true); //try to commit user settings to disk (will perform validation as well)
                if (valid)
                    //if validation succeeds, we can close the form (if not, then the form's ErrorProvider will display errors to the user)
                {
                    if (methodToRunIfValidBeforeRestartingExtractor != null)
                    {
                        methodToRunIfValidBeforeRestartingExtractor();
                    }
                    if (_extractorRunningStateOnFormOpen)
                    {
                        StopAndRestartExtractor();
                    }
                    else
                    {
						if (Extractor.State.Running)
                        {
                            _extractor.Stop(); //stop the Extractor if it's currently running
                        }
                    _extractor.LoadSettings(); //tell the Extractor to reload its settings 
                }
                }
                else
                {
                    MessageBox.Show(
                        "One or more settings are currently marked as invalid.\n\nYou must correct any settings that are marked as invalid before you can apply changes.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception e) //exceptions will cause the Options form to close
            {
                _log.Error(e.Message, e);
            }
            return valid;
        }

        /// <summary>
        ///     Saves user settings to the in-memory user-config cache (and optionally, to disk as well)
        /// </summary>
        /// <param name="persist">
        ///     if <see langword="true" />, the current settings will be persisited to
        ///     the on-disk user-config file.  If <see langword="false" />, the settings will not be
        ///     persisited to disk.  In either case, the current Extractor instance will be
        ///     updated with the new settings.
        /// </param>
        private void SaveSettings(bool persist)
        {
            _savingSettings = true;
            Settings.Default.PropertyChanged -= SettingsChanged;
            Settings settings = Settings.Default;
            settings.UpgradeNeeded = false;
            settings.NetworkImageFormat = cboImageFormat.SelectedItem.ToString();
            settings.CompressionType = cboCompressionType.SelectedItem.ToString();
            settings.EnableLMFDOutput = chkEnableLeftMFD.Checked;
            settings.EnableRMFDOutput = chkEnableRightMFD.Checked;
            settings.EnableHudOutput = chkEnableHud.Checked;

            settings.EnableAzimuthIndicatorOutput = chkAzimuthIndicator.Checked;
            settings.EnableADIOutput = chkADI.Checked;
            settings.EnableBackupADIOutput = chkStandbyADI.Checked;
            settings.EnableASIOutput = chkAirspeedIndicator.Checked;
            settings.EnableAltimeterOutput = chkAltimeter.Checked;
            settings.EnableAOAIndexerOutput = chkAOAIndexer.Checked;
            settings.EnableAOAIndicatorOutput = chkAOAIndicator.Checked;
            settings.EnableCautionPanelOutput = chkCautionPanel.Checked;
            settings.EnableCMDSOutput = chkCMDSPanel.Checked;
            settings.EnableCompassOutput = chkCompass.Checked;
            settings.EnableDEDOutput = chkDED.Checked;
            settings.EnableAccelerometerOutput = chkAccelerometer.Checked;
            settings.EnableFTIT1Output = chkFTIT1.Checked;
            settings.EnableFTIT2Output = chkFTIT2.Checked;
            settings.EnableNOZ1Output = chkNOZ1.Checked;
            settings.EnableNOZ2Output = chkNOZ2.Checked;
            settings.EnableOIL1Output = chkOIL1.Checked;
            settings.EnableOIL2Output = chkOIL2.Checked;
            settings.EnableRPM1Output = chkRPM1.Checked;
            settings.EnableRPM2Output = chkRPM2.Checked;
            settings.EnableEPUFuelOutput = chkEPU.Checked;
            settings.EnableFuelFlowOutput = chkFuelFlow.Checked;
            settings.EnableISISOutput = chkISIS.Checked;
            settings.EnableFuelQuantityOutput = chkFuelQty.Checked;
            settings.EnableGearLightsOutput = chkGearLights.Checked;
            settings.EnableHSIOutput = chkHSI.Checked;
            settings.EnableEHSIOutput = chkEHSI.Checked;
            settings.EnableNWSIndexerOutput = chkNWSIndexer.Checked;
            settings.EnablePFLOutput = chkPFL.Checked;
            settings.EnableSpeedbrakeOutput = chkSpeedbrake.Checked;
            settings.EnableVVIOutput = chkVVI.Checked;
            settings.EnableHYDAOutput = chkHydA.Checked;
            settings.EnableHYDBOutput = chkHydB.Checked;
            settings.EnableCabinPressOutput = chkCabinPress.Checked;
            settings.EnableRollTrimOutput = chkRollTrim.Checked;
            settings.EnablePitchTrimOutput = chkPitchTrim.Checked;

            if (rdoAltimeterStyleDigital.Checked)
            {
                settings.Altimeter_Style = Altimeter.AltimeterOptions.F16AltimeterStyle.Electronic.ToString();
            }
            else if (rdoAltimeterStyleElectromechanical.Checked)
            {
                settings.Altimeter_Style =
                    Altimeter.AltimeterOptions.F16AltimeterStyle.Electromechanical.ToString();
            }

            if (rdoAzimuthIndicatorStyleScope.Checked)
            {
                if (rdoAzimuthIndicatorIP1310BezelType.Checked)
                {
                    settings.AzimuthIndicatorType =
                        AzimuthIndicator.InstrumentStyle.IP1310ALR.ToString();
                    settings.AzimuthIndicator_ShowBezel = true;
                }
                else if (rdoAzimuthIndicatorHAFBezelType.Checked)
                {
                    settings.AzimuthIndicatorType =
                        AzimuthIndicator.InstrumentStyle.HAF.ToString();
                    settings.AzimuthIndicator_ShowBezel = true;
                }
                else if (rdoAzimuthIndicatorNoBezel.Checked)
                {
                    settings.AzimuthIndicatorType =
                        AzimuthIndicator.InstrumentStyle.IP1310ALR.ToString();
                    settings.AzimuthIndicator_ShowBezel = false;
                }
            }
            else if (rdoAzimuthIndicatorStyleDigital.Checked)
            {
                settings.AzimuthIndicatorType =
                    AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay.ToString();
            }

            if (rdoVVIStyleNeedle.Checked)
            {
                settings.VVI_Style = VVIStyles.Needle.ToString();
            }
            else if (rdoVVIStyleTape.Checked)
            {
                settings.VVI_Style = VVIStyles.Tape.ToString();
            }

            if (rdoFuelQuantityNeedleCModel.Checked)
            {
                settings.FuelQuantityIndicator_NeedleCModel = true;
            }
            else if (rdoFuelQuantityDModel.Checked)
            {
                settings.FuelQuantityIndicator_NeedleCModel = false;
            }

            settings.StartOnLaunch = chkStartOnLaunch.Checked;
            settings.PollingDelay = Convert.ToInt32(txtPollDelay.Text, CultureInfo.InvariantCulture);
            settings.LaunchWithWindows = chkStartWithWindows.Checked;

            if (rdoClient.Checked)
            {
                settings.NetworkingMode = (int) NetworkMode.Client;
                settings.ClientUseServerPortNum = Convert.ToInt32(txtNetworkClientUseServerPortNum.Text,
                                                                  CultureInfo.InvariantCulture);
                settings.ClientUseServerIpAddress = txtClientUseServerIpAddress.Text;
            }
            else if (rdoServer.Checked)
            {
                settings.NetworkingMode = (int) NetworkMode.Server;
                settings.ServerUsePortNumber = Convert.ToInt32(txtNetworkServerUsePortNum.Text,
                                                               CultureInfo.InvariantCulture);
            }
            else if (rdoStandalone.Checked)
            {
                settings.NetworkingMode = (int) NetworkMode.Standalone;
            }

            Settings.Default.HighlightOutputWindows = chkHighlightOutputWindowsWhenContainMouseCursor.Checked;
            if (persist) //persist the user settings to disk
            {
				_extractorRunningStateOnFormOpen = Extractor.State.Running;
                //store current Extractor instance's "isRunning" state
                settings.Save(); //save our new settings to the current settings cache (on-disk)
            }
            if (persist)
            {
                //update the Windows Registry's Run-at-startup applications list according
                //to the new user settings
                if (chkStartWithWindows.Checked)
                {
                    var c = new Computer();
                    try
                    {
                        using (
                            RegistryKey startupKey =
                                c.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)
                            )
                        {
                            startupKey.SetValue(Application.ProductName, Application.ExecutablePath,
                                                RegistryValueKind.String);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
                }
                else
                {
                    var c = new Computer();
                    try
                    {
                        using (
                            RegistryKey startupKey =
                                c.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)
                            )
                        {
                            startupKey.DeleteValue(Application.ProductName, false);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
                }
            }
            Settings.Default.PropertyChanged += SettingsChanged;
            _savingSettings = false;
        }

        /// <summary>
        ///     Event handler for the Cancel button's Click event
        /// </summary>
        /// <param name="sender">object raising this event</param>
        /// <param name="e">Event arguments for the Cancel button's Click event</param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Default.Reload(); //re-load the on-disk user settings into the in-memory user config cache
				if (Extractor.State.Running)
                {
                    _extractor.Stop(); //stop the Extractor engine if it's running
                }
                _extractor.LoadSettings(); //tell the Extractor engine to reload its settings
            }
            catch {}
            Close(); //user has cancelled out of the Options form, so close the form now
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
			Extractor.State.OptionsFormIsShowing = false;
            if (_extractorRunningStateOnFormOpen)
            {
				if (!Extractor.State.Running)
                {
                    _extractor.Start();
                }
            }
            else
            {
				if (Extractor.State.Running)
                {
                    _extractor.Stop();
                }
            }
        }

        /// <summary>
        ///     Event handler for the rdoStandalone control's CheckChanged event
        /// </summary>
        /// <param name="sender">the object raising this event</param>
        /// <param name="e">Event arguments for the rdoStandalone control's CheckChanged event</param>
        private void rdoStandalone_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoStandalone.Checked)
            {
                EnableStandaloneModeOptions();
            }
        }

        /// <summary>
        ///     Event handler for the rdoClient control's CheckChanged event
        /// </summary>
        /// <param name="sender">the object raising this event</param>
        /// <param name="e">Event arguments for the rdoClient control's CheckChanged event</param>
        private void rdoClient_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoClient.Checked)
            {
                EnableClientModeOptions();
            }
        }

        /// <summary>
        ///     Event handler for the rdoServer control's CheckChanged event
        /// </summary>
        /// <param name="sender">the object raising this event</param>
        /// <param name="e">Event arguments for the rdoServer control's CheckChanged event</param>
        private void rdoServer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoServer.Checked)
            {
                EnableServerModeOptions();
            }
        }

        /// <summary>
        ///     Enables options that are applicable for Server Mode (and disables options that are not
        ///     relevant to this mode)
        /// </summary>
        private void EnableServerModeOptions()
        {
            rdoServer.Checked = true;
            rdoClient.Checked = false;
            rdoStandalone.Checked = false;
            grpServerOptions.Enabled = true;
            grpServerOptions.Visible = true;
            grpServerOptions.BringToFront();
            grpClientOptions.Enabled = false;
            grpClientOptions.Visible = false;
            grpClientOptions.SendToBack();
//            tabHotkeysInner.Enabled = true;
            cmdBMSOptions.Enabled = true;
            errControlErrorProvider.Clear();
        }

        /// <summary>
        ///     Enables options that are applicable for Client Mode (and disables options that are not
        ///     relevant to this mode)
        /// </summary>
        private void EnableClientModeOptions()
        {
            rdoClient.Checked = true;
            rdoStandalone.Checked = false;
            rdoServer.Checked = false;
            grpClientOptions.Enabled = true;
            grpClientOptions.Visible = true;
            grpClientOptions.BringToFront();
            grpServerOptions.Enabled = false;
            grpServerOptions.Visible = false;
            grpServerOptions.SendToBack();

//            tabHotkeysInner.Enabled = false;
            cmdBMSOptions.Enabled = false;
            errControlErrorProvider.Clear();
        }

        /// <summary>
        ///     Enables options that are applicable for Standalone Mode (and disables options that are not
        ///     relevant to this mode)
        /// </summary>
        private void EnableStandaloneModeOptions()
        {
            rdoServer.Checked = false;
            rdoClient.Checked = false;
            rdoStandalone.Checked = true;
            grpServerOptions.Enabled = false;
            grpClientOptions.Enabled = false;
            grpServerOptions.Visible = false;
            grpClientOptions.Visible = false;

//            tabHotkeysInner.Enabled = true;
            cmdBMSOptions.Enabled = true;
            errControlErrorProvider.Clear();
        }

        private void StopAndRestartExtractor()
        {
            if (_formLoading) return;
			if (Extractor.State.Running)
            {
                _extractor.Stop();
                _extractor.LoadSettings();
                _extractor.Start();
            }
        }

        private void chkEnableLeftMFD_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableLMFDOutput = chkEnableLeftMFD.Checked;
            cmdRecoverLeftMfd.Enabled = chkEnableLeftMFD.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkEnableRightMFD_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableRMFDOutput = chkEnableRightMFD.Checked;
            cmdRecoverRightMfd.Enabled = chkEnableRightMFD.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkEnableHud_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableHudOutput = chkEnableHud.Checked;
            cmdRecoverHud.Enabled = chkEnableHud.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void cmdBMSOptions_Click(object sender, EventArgs e)
        {
            var options = new frmBMSOptions();
            options.BmsPath = Settings.Default.BmsPath;
            options.ShowDialog(this);
            if (!options.Cancelled)
            {
                string newBmsPath = options.BmsPath;
                if (newBmsPath != null && newBmsPath != string.Empty)
                {
                    Settings.Default.BmsPath = options.BmsPath;
                }
            }
        }

        private void cboImageFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCompressionTypeList();
        }

        private void cmdResetToDefaults_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Warning: This will reset all MFD Extractor options to their defaults.  You will lose any customizations you have made.  Do you want to continue?",
                    "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                Settings.Default.Reset();
                Settings.Default.UpgradeNeeded = false;
				bool extractorRunning = Extractor.State.Running;
                if (extractorRunning)
                {
                    _extractor.Stop();
                    _extractor.LoadSettings();
					Extractor.State.OptionsFormIsShowing = true;
                }
                if (extractorRunning)
                {
                    _extractor.Start();
                }
                LoadSettings();
            }
        }

        private void chkAOAIndicator_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableAOAIndicatorOutput = chkAOAIndicator.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkAzimuthIndicator_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableAzimuthIndicatorOutput = chkAzimuthIndicator.Checked;
            grpAzimuthIndicatorStyle.Enabled = chkAzimuthIndicator.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkADI_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableADIOutput = chkADI.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkAirspeedIndicator_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableASIOutput = chkAirspeedIndicator.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkAltimeter_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableAltimeterOutput = chkAltimeter.Checked;
            grpAltimeterStyle.Enabled = chkAltimeter.Checked;
            //grpPressureAltitudeSettings.Enabled = chkAltimeter.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkAOAIndexer_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableAOAIndexerOutput = chkAOAIndexer.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkCautionPanel_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableCautionPanelOutput = chkCautionPanel.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkCMDSPanel_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableCMDSOutput = chkCMDSPanel.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkCompass_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableCompassOutput = chkCompass.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkDED_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableDEDOutput = chkDED.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkFTIT1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableFTIT1Output = chkFTIT1.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkAccelerometer_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableAccelerometerOutput = chkAccelerometer.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkNOZ1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableNOZ1Output = chkNOZ1.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkOIL1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableOIL1Output = chkOIL1.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkRPM1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableRPM1Output = chkRPM1.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkFTIT2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableFTIT2Output = chkFTIT2.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkNOZ2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableNOZ2Output = chkNOZ2.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkOIL2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableOIL2Output = chkOIL2.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkRPM2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableRPM2Output = chkRPM2.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkEPU_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableEPUFuelOutput = chkEPU.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkFuelFlow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableFuelFlowOutput = chkFuelFlow.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkISIS_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableISISOutput = chkISIS.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkFuelQty_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableFuelQuantityOutput = chkFuelQty.Checked;
            gbFuelQuantityOptions.Enabled = chkFuelQty.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkGearLights_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableGearLightsOutput = chkGearLights.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkHSI_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableHSIOutput = chkHSI.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkEHSI_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableEHSIOutput = chkEHSI.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkNWSIndexer_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableNWSIndexerOutput = chkNWSIndexer.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkPFL_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnablePFLOutput = chkPFL.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkSpeedbrake_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableSpeedbrakeOutput = chkSpeedbrake.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkStandbyADI_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableBackupADIOutput = chkStandbyADI.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkVVI_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableVVIOutput = chkVVI.Checked;
            grpVVIOptions.Enabled = chkVVI.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkHydA_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableHYDAOutput = chkHydA.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkHydB_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableHYDBOutput = chkHydB.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkCabinPress_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableCabinPressOutput = chkCabinPress.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkRollTrim_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableRollTrimOutput = chkRollTrim.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }

        private void chkPitchTrim_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnablePitchTrimOutput = chkPitchTrim.Checked;
            //StopAndRestartExtractor();
            BringToFront();
        }


        private void cmdNV_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.NVISKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.NVISKey = serialized;
            }
        }

        private void rdoAzimuthIndicatorStyleScope_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAzimuthIndicatorStyleScope.Checked)
            {
                grpAzimuthIndicatorDigitalTypes.Visible = false;
                grpAzimuthIndicatorBezelTypes.Visible = true;
                if (!rdoAzimuthIndicatorIP1310BezelType.Checked && !rdoAzimuthIndicatorHAFBezelType.Checked)
                {
                    rdoAzimuthIndicatorIP1310BezelType.Checked = true;
                    Settings.Default.AzimuthIndicator_ShowBezel = false;
                }
                else
                {
                    Settings.Default.AzimuthIndicator_ShowBezel = true;
                }
            }
        }

        private void rdoAzimuthIndicatorIP1310BezelType_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAzimuthIndicatorIP1310BezelType.Checked)
            {
                Settings.Default.AzimuthIndicatorType =
                    AzimuthIndicator.InstrumentStyle.IP1310ALR.ToString();
                Settings.Default.AzimuthIndicator_ShowBezel = true;
                //StopAndRestartExtractor();
            }
        }

        private void rdoAzimuthIndicatorHAFBezelType_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAzimuthIndicatorHAFBezelType.Checked)
            {
                Settings.Default.AzimuthIndicatorType =
                    AzimuthIndicator.InstrumentStyle.HAF.ToString();
                Settings.Default.AzimuthIndicator_ShowBezel = true;
                //StopAndRestartExtractor();
            }
        }

        private void rdoAzimuthIndicatorNoBezel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAzimuthIndicatorNoBezel.Checked)
            {
                Settings.Default.AzimuthIndicatorType =
                    AzimuthIndicator.InstrumentStyle.IP1310ALR.ToString();
                Settings.Default.AzimuthIndicator_ShowBezel = false;
                //StopAndRestartExtractor();
            }
        }

        private void rdoAzimuthIndicatorStyleDigital_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAzimuthIndicatorStyleDigital.Checked)
            {
                grpAzimuthIndicatorDigitalTypes.Visible = true;
                grpAzimuthIndicatorBezelTypes.Visible = false;
                if (!rdoATDPlus.Checked)
                {
                    rdoATDPlus.Checked = true;
                }
            }
        }

        private void rdoATDPlus_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoATDPlus.Checked)
            {
                Settings.Default.AzimuthIndicatorType =
                    AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay.ToString();
                //StopAndRestartExtractor();
            }
        }


        private void rdoAltimeterStyleElectromechanical_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAltimeterStyleElectromechanical.Checked)
            {
                Settings.Default.Altimeter_Style =
                    Altimeter.AltimeterOptions.F16AltimeterStyle.Electromechanical.ToString();
                //StopAndRestartExtractor();
            }
        }

        private void rdoAltimeterStyleDigital_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAltimeterStyleDigital.Checked)
            {
                Settings.Default.Altimeter_Style =
                    Altimeter.AltimeterOptions.F16AltimeterStyle.Electronic.ToString();
                //StopAndRestartExtractor();
            }
        }

        private void rdoVVIStyleTape_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoVVIStyleTape.Checked)
            {
                Settings.Default.VVI_Style = VVIStyles.Tape.ToString();
                //StopAndRestartExtractor();
            }
        }

        private void rdoVVIStyleNeedle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoVVIStyleNeedle.Checked)
            {
                Settings.Default.VVI_Style = VVIStyles.Needle.ToString();
                //StopAndRestartExtractor();
            }
        }

        private void rdoFuelQuantityNeedleCModel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoFuelQuantityNeedleCModel.Checked)
            {
                Settings.Default.FuelQuantityIndicator_NeedleCModel = true;
                //StopAndRestartExtractor();
            }
        }

        private void rdoFuelQuantityDModel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoFuelQuantityDModel.Checked)
            {
                Settings.Default.FuelQuantityIndicator_NeedleCModel = false;
                //StopAndRestartExtractor();
            }
        }


        private void cmdAirspeedIndexIncreaseHotkey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.AirspeedIndexIncreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.AirspeedIndexIncreaseKey = serialized;
            }
        }

        private void cmdAirspeedIndexDecreaseHotkey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.AirspeedIndexDecreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.AirspeedIndexDecreaseKey = serialized;
            }
        }

        private void cmdEHSIMenuButtonHotkey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSIMenuButtonKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSIMenuButtonKey = serialized;
            }
        }

        private void cmdEHSIHeadingIncreaseKey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSIHeadingIncreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSIHeadingIncreaseKey = serialized;
            }
        }

        private void cmdEHSIHeadingDecreaseKey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSIHeadingDecreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSIHeadingDecreaseKey = serialized;
            }
        }

        private void cmdEHSICourseIncreaseKey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSICourseIncreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSICourseIncreaseKey = serialized;
            }
        }

        private void cmdEHSICourseDecreaseKey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSICourseDecreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSICourseDecreaseKey = serialized;
            }
        }

        private void cmdEHSICourseKnobDepressedKey_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.EHSICourseKnobDepressedKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.EHSICourseKnobDepressedKey = serialized;
            }
        }

        private void cmdAzimuthIndicatorBrightnessIncrease_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.AzimuthIndicatorBrightnessIncreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.AzimuthIndicatorBrightnessIncreaseKey = serialized;
            }
        }

        private void cmdAzimuthIndicatorBrightnessDecrease_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.AzimuthIndicatorBrightnessDecreaseKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.AzimuthIndicatorBrightnessDecreaseKey = serialized;
            }
        }

        private void cmdISISBrightButtonPressed_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.ISISBrightButtonKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.ISISBrightButtonKey = serialized;
            }
        }

        private void cmdISISStandardBrightnessButtonPressed_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.ISISStandardButtonKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.ISISStandardButtonKey = serialized;
            }
        }

        private void cmdAccelerometerResetButtonPressed_Click(object sender, EventArgs e)
        {
            var toShow = new InputSourceSelector();
            toShow.Mediator = _extractor.Mediator;
            string keyFromSettingsString = Settings.Default.AccelerometerResetKey;

            InputControlSelection keyFromSettings = null;
            try
            {
                keyFromSettings =
                    (InputControlSelection)
                    Common.Serialization.Util.DeserializeFromXml(keyFromSettingsString, typeof (InputControlSelection));
            }
            catch 
            {
            }
            if (keyFromSettings != null)
            {
                toShow.SelectedControl = keyFromSettings;
            }
            toShow.ShowDialog(this);
            InputControlSelection selection = toShow.SelectedControl;
            if (selection != null)
            {
                string serialized = Common.Serialization.Util.SerializeToXml(selection, typeof (InputControlSelection));
                Settings.Default.AccelerometerResetKey = serialized;
            }
        }

        private void chkHighlightOutputWindowsWhenContainMouseCursor_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HighlightOutputWindows = chkHighlightOutputWindowsWhenContainMouseCursor.Checked;
        }

        private void cmdRecoverLeftMfd_Click(object sender, EventArgs e)
        {
            RecoverInstrument("LMFD");
        }

        private void cmdRecoverRightMfd_Click(object sender, EventArgs e)
        {
            RecoverInstrument("RMFD");
        }

        private void cmdRecoverHud_Click(object sender, EventArgs e)
        {
            RecoverInstrument("HUD");
        }


    }
}