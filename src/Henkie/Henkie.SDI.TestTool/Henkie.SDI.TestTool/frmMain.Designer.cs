namespace Henkie.SDI.TestTool
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblDeviceAddress = new System.Windows.Forms.Label();
            this.epErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbRawDataControl = new System.Windows.Forms.GroupBox();
            this.lblSubAddr = new System.Windows.Forms.Label();
            this.txtSubAddr = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblDataByte = new System.Windows.Forms.Label();
            this.txtDataByte = new System.Windows.Forms.TextBox();
            this.lblSerialPort = new System.Windows.Forms.Label();
            this.cbSerialPort = new System.Windows.Forms.ComboBox();
            this.lblIdentification = new System.Windows.Forms.Label();
            this.gbLED = new System.Windows.Forms.GroupBox();
            this.rdoToggleLEDPerAcceptedCommand = new System.Windows.Forms.RadioButton();
            this.rdoLEDFlashesAtHeartbeatRate = new System.Windows.Forms.RadioButton();
            this.rdoLEDAlwaysOn = new System.Windows.Forms.RadioButton();
            this.rdoLEDAlwaysOff = new System.Windows.Forms.RadioButton();
            this.gbWatchdog = new System.Windows.Forms.GroupBox();
            this.lblCountdownDesc = new System.Windows.Forms.Label();
            this.lblWatchdogCountdown = new System.Windows.Forms.Label();
            this.nudWatchdogCountdown = new System.Windows.Forms.NumericUpDown();
            this.chkWatchdogEnabled = new System.Windows.Forms.CheckBox();
            this.btnDisableWatchdog = new System.Windows.Forms.Button();
            this.gbPowerDown = new System.Windows.Forms.GroupBox();
            this.lblDelayDescr = new System.Windows.Forms.Label();
            this.lblPowerDownDelayTime = new System.Windows.Forms.Label();
            this.nudPowerDownDelay = new System.Windows.Forms.NumericUpDown();
            this.gbPowerDownLevel = new System.Windows.Forms.GroupBox();
            this.rdoPowerDownLevelHalf = new System.Windows.Forms.RadioButton();
            this.rdoPowerDownLevelFull = new System.Windows.Forms.RadioButton();
            this.chkPowerDownEnabled = new System.Windows.Forms.CheckBox();
            this.gbUSBDebug = new System.Windows.Forms.GroupBox();
            this.chkUSBDebugEnabled = new System.Windows.Forms.CheckBox();
            this.gbDemo = new System.Windows.Forms.GroupBox();
            this.chkStartDemo = new System.Windows.Forms.CheckBox();
            this.gbDemoSpeedAndStepping = new System.Windows.Forms.GroupBox();
            this.lblDemoMovementSpeed = new System.Windows.Forms.Label();
            this.cboDemoMovementStepSize = new System.Windows.Forms.ComboBox();
            this.cboDemoMovementSpeed = new System.Windows.Forms.ComboBox();
            this.lblDemoMovementStepSizeDesc = new System.Windows.Forms.Label();
            this.lblDemoMovementStepSize = new System.Windows.Forms.Label();
            this.lblDemoMovementSpeedDesc = new System.Windows.Forms.Label();
            this.gbModus = new System.Windows.Forms.GroupBox();
            this.rdoDemoModusStartToEndJumpToStart = new System.Windows.Forms.RadioButton();
            this.rdoModusStartToEndToStart = new System.Windows.Forms.RadioButton();
            this.gbDemoStartAndEndPositions = new System.Windows.Forms.GroupBox();
            this.lblDemoEndPositionHex = new System.Windows.Forms.Label();
            this.lblDemoStartPositionHex = new System.Windows.Forms.Label();
            this.lblDemoEndPositionDegrees = new System.Windows.Forms.Label();
            this.lblDemoStartPositionDegrees = new System.Windows.Forms.Label();
            this.nudDemoEndPositionDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblDemoEndPositionDecimal = new System.Windows.Forms.Label();
            this.nudDemoStartPositionDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblDemoStartPosition = new System.Windows.Forms.Label();
            this.gbStatorBaseAngles = new System.Windows.Forms.GroupBox();
            this.btnUpdateStatorBaseAngles = new System.Windows.Forms.Button();
            this.lblStatorS3BaseAngleHex = new System.Windows.Forms.Label();
            this.lblStatorS2BaseAngleHex = new System.Windows.Forms.Label();
            this.lblStatorS1BaseAngleHex = new System.Windows.Forms.Label();
            this.lblStatorS3BaseAngleDegrees = new System.Windows.Forms.Label();
            this.lblStatorS2BaseAngleDegrees = new System.Windows.Forms.Label();
            this.lblStatorS1BaseAngleDegrees = new System.Windows.Forms.Label();
            this.lblStatorS3BaseAngleMSB = new System.Windows.Forms.Label();
            this.lblStatorS2BaseAngleMSB = new System.Windows.Forms.Label();
            this.lblStatorS1BaseAngleMSB = new System.Windows.Forms.Label();
            this.lblStatorS3BaseAngleLSB = new System.Windows.Forms.Label();
            this.lblStatorS2BaseAngleLSB = new System.Windows.Forms.Label();
            this.lblStatorS1BaseAngleLSB = new System.Windows.Forms.Label();
            this.nudStatorS3BaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lblStatorS3BaseAngle = new System.Windows.Forms.Label();
            this.nudStatorS2BaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lblStatorS2BaseAngle = new System.Windows.Forms.Label();
            this.nudStatorS1BaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lblStatorS1BaseAngle = new System.Windows.Forms.Label();
            this.gbMovementLimits = new System.Windows.Forms.GroupBox();
            this.lblLimitMaximumHex = new System.Windows.Forms.Label();
            this.lblLimitMaximumDegrees = new System.Windows.Forms.Label();
            this.lblLimitMinimumHex = new System.Windows.Forms.Label();
            this.lblLimitMinimumDegrees = new System.Windows.Forms.Label();
            this.lblLimitMinDesc = new System.Windows.Forms.Label();
            this.lblLimitMaxDesc = new System.Windows.Forms.Label();
            this.nudLimitMax = new System.Windows.Forms.NumericUpDown();
            this.lblLimitMax = new System.Windows.Forms.Label();
            this.nudLimitMin = new System.Windows.Forms.NumericUpDown();
            this.lblLimitMin = new System.Windows.Forms.Label();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSynchroSetup = new System.Windows.Forms.TabPage();
            this.gbUpdateRateControl = new System.Windows.Forms.GroupBox();
            this.nudUpdateRateControlSpeed = new System.Windows.Forms.NumericUpDown();
            this.chkUpdateRateControlShortestPath = new System.Windows.Forms.CheckBox();
            this.lblUpdateRateControlSpeedDesc = new System.Windows.Forms.Label();
            this.lblUpdateRateControlSpeed = new System.Windows.Forms.Label();
            this.gbURCMode = new System.Windows.Forms.GroupBox();
            this.lblURCSmoothModeSmoothUpdates = new System.Windows.Forms.Label();
            this.cboURCSmoothModeSmoothUpdates = new System.Windows.Forms.ComboBox();
            this.lblURCSmoothModeThreshold = new System.Windows.Forms.Label();
            this.lblURCSmoothModeThresholdHex = new System.Windows.Forms.Label();
            this.lblURCSmoothModeThresholdDegrees = new System.Windows.Forms.Label();
            this.nudURCSmoothModeThresholdDecimal = new System.Windows.Forms.NumericUpDown();
            this.rdoURCSmoothMode = new System.Windows.Forms.RadioButton();
            this.lblURCLimitThreshold = new System.Windows.Forms.Label();
            this.lblURCLimitThresholdHex = new System.Windows.Forms.Label();
            this.lblURCLimitThresholdDegrees = new System.Windows.Forms.Label();
            this.rdoURCLimitMode = new System.Windows.Forms.RadioButton();
            this.nudURCLimitThresholdDecimal = new System.Windows.Forms.NumericUpDown();
            this.tabSynchroControl = new System.Windows.Forms.TabPage();
            this.gbSetStatorAmplitudeAndPolarityImmediate = new System.Windows.Forms.GroupBox();
            this.btnUpdateStatorAmplitudesAndPolarities = new System.Windows.Forms.Button();
            this.lblStatorAmplitudeAndPolarityUpdateMode = new System.Windows.Forms.Label();
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates = new System.Windows.Forms.RadioButton();
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates = new System.Windows.Forms.RadioButton();
            this.lblStators = new System.Windows.Forms.Label();
            this.lblS3AmplitudeDecimal = new System.Windows.Forms.Label();
            this.lblS2AmplitudeDecimal = new System.Windows.Forms.Label();
            this.lblS1AmplitudeDecimal = new System.Windows.Forms.Label();
            this.lblPolarities = new System.Windows.Forms.Label();
            this.lblAmplitudes = new System.Windows.Forms.Label();
            this.chkS3Polarity = new System.Windows.Forms.CheckBox();
            this.chkS2Polarity = new System.Windows.Forms.CheckBox();
            this.chkS1Polarity = new System.Windows.Forms.CheckBox();
            this.lblS3AmplitudeHex = new System.Windows.Forms.Label();
            this.nudS3AmplitudeDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblS3AmplitudePolarity = new System.Windows.Forms.Label();
            this.lblS2AmplitudeHex = new System.Windows.Forms.Label();
            this.nudS2AmplitudeDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblS2AmplitudePolarity = new System.Windows.Forms.Label();
            this.lblS1AmplitudeHex = new System.Windows.Forms.Label();
            this.nudS1AmplitudeDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblS1AmplitudePolarity = new System.Windows.Forms.Label();
            this.gbMoveIndicatorCoarseResolution = new System.Windows.Forms.GroupBox();
            this.lblMoveIndicatorCoarseResolutionHex = new System.Windows.Forms.Label();
            this.lblMoveIndicatorCoarseResolutionDegrees = new System.Windows.Forms.Label();
            this.nudMoveIndicatorCoarseResolutionDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblMoveIndicatorCoarseResolutionPositionDecimal = new System.Windows.Forms.Label();
            this.gbIndicatorMovementControl = new System.Windows.Forms.GroupBox();
            this.lblMoveIndicatorInQuadrant = new System.Windows.Forms.Label();
            this.cboMoveIndicatorInQuadrant = new System.Windows.Forms.ComboBox();
            this.lblMoveIndicatorToPositionHex = new System.Windows.Forms.Label();
            this.lblMoveIndicatorToPositionDegrees = new System.Windows.Forms.Label();
            this.nudMoveIndicatorToPositionDecimal = new System.Windows.Forms.NumericUpDown();
            this.lblMoveIndicatorToPosition = new System.Windows.Forms.Label();
            this.tabDemoMode = new System.Windows.Forms.TabPage();
            this.tabDigitalAndPWMOutputs = new System.Windows.Forms.TabPage();
            this.gbDigitalAndPWMOutputs = new System.Windows.Forms.GroupBox();
            this.lblPWM_OUT_Hex = new System.Windows.Forms.Label();
            this.nudPWM_OUT_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboPWM_OUT_Value = new System.Windows.Forms.ComboBox();
            this.cboPWM_OUT_Mode = new System.Windows.Forms.ComboBox();
            this.lblPWM_OUT = new System.Windows.Forms.Label();
            this.lblDIG_PWM_7_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_7_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_7_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_7_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_7 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_6_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_6_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_6_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_6_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_6 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_5_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_5_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_5_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_5_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_5 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_4_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_4_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_4_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_4_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_4 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_3_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_3_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_3_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_3_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_3 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_2_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_2_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.cboDIG_PWM_2_Value = new System.Windows.Forms.ComboBox();
            this.cboDIG_PWM_2_Mode = new System.Windows.Forms.ComboBox();
            this.lblDIG_PWM_2 = new System.Windows.Forms.Label();
            this.lblDIG_PWM_1_Hex = new System.Windows.Forms.Label();
            this.nudDIG_PWM_1_DutyCycle = new System.Windows.Forms.NumericUpDown();
            this.lblDutyCycle = new System.Windows.Forms.Label();
            this.cboDIG_PWM_1_Value = new System.Windows.Forms.ComboBox();
            this.lblDigitalAndPWMChannelValues = new System.Windows.Forms.Label();
            this.cboDIG_PWM_1_Mode = new System.Windows.Forms.ComboBox();
            this.lblDigitalOrPWM = new System.Windows.Forms.Label();
            this.lblChannels = new System.Windows.Forms.Label();
            this.lblDIG_PWM_1 = new System.Windows.Forms.Label();
            this.tabRawData = new System.Windows.Forms.TabPage();
            this.tabDiagnosticLED = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.epErrorProvider)).BeginInit();
            this.gbRawDataControl.SuspendLayout();
            this.gbLED.SuspendLayout();
            this.gbWatchdog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchdogCountdown)).BeginInit();
            this.gbPowerDown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPowerDownDelay)).BeginInit();
            this.gbPowerDownLevel.SuspendLayout();
            this.gbUSBDebug.SuspendLayout();
            this.gbDemo.SuspendLayout();
            this.gbDemoSpeedAndStepping.SuspendLayout();
            this.gbModus.SuspendLayout();
            this.gbDemoStartAndEndPositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDemoEndPositionDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDemoStartPositionDecimal)).BeginInit();
            this.gbStatorBaseAngles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS3BaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS2BaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS1BaseAngle)).BeginInit();
            this.gbMovementLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMin)).BeginInit();
            this.gbMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSynchroSetup.SuspendLayout();
            this.gbUpdateRateControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpdateRateControlSpeed)).BeginInit();
            this.gbURCMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudURCSmoothModeThresholdDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudURCLimitThresholdDecimal)).BeginInit();
            this.tabSynchroControl.SuspendLayout();
            this.gbSetStatorAmplitudeAndPolarityImmediate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudS3AmplitudeDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS2AmplitudeDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS1AmplitudeDecimal)).BeginInit();
            this.gbMoveIndicatorCoarseResolution.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveIndicatorCoarseResolutionDecimal)).BeginInit();
            this.gbIndicatorMovementControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveIndicatorToPositionDecimal)).BeginInit();
            this.tabDemoMode.SuspendLayout();
            this.tabDigitalAndPWMOutputs.SuspendLayout();
            this.gbDigitalAndPWMOutputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPWM_OUT_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_7_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_6_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_5_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_4_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_3_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_2_DutyCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_1_DutyCycle)).BeginInit();
            this.tabRawData.SuspendLayout();
            this.tabDiagnosticLED.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDeviceAddress
            // 
            this.lblDeviceAddress.Location = new System.Drawing.Point(0, 0);
            this.lblDeviceAddress.Name = "lblDeviceAddress";
            this.lblDeviceAddress.Size = new System.Drawing.Size(100, 23);
            this.lblDeviceAddress.TabIndex = 0;
            // 
            // epErrorProvider
            // 
            this.epErrorProvider.ContainerControl = this;
            // 
            // gbRawDataControl
            // 
            this.gbRawDataControl.Controls.Add(this.lblSubAddr);
            this.gbRawDataControl.Controls.Add(this.txtSubAddr);
            this.gbRawDataControl.Controls.Add(this.btnSend);
            this.gbRawDataControl.Controls.Add(this.lblDataByte);
            this.gbRawDataControl.Controls.Add(this.txtDataByte);
            this.gbRawDataControl.Location = new System.Drawing.Point(3, 3);
            this.gbRawDataControl.Name = "gbRawDataControl";
            this.gbRawDataControl.Size = new System.Drawing.Size(152, 152);
            this.gbRawDataControl.TabIndex = 0;
            this.gbRawDataControl.TabStop = false;
            this.gbRawDataControl.Text = "Raw Data Control";
            // 
            // lblSubAddr
            // 
            this.lblSubAddr.AutoSize = true;
            this.lblSubAddr.Location = new System.Drawing.Point(19, 61);
            this.lblSubAddr.Name = "lblSubAddr";
            this.lblSubAddr.Size = new System.Drawing.Size(66, 13);
            this.lblSubAddr.TabIndex = 2;
            this.lblSubAddr.Text = "S&ubaddress:";
            this.lblSubAddr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSubAddr
            // 
            this.txtSubAddr.Location = new System.Drawing.Point(91, 58);
            this.txtSubAddr.MaxLength = 4;
            this.txtSubAddr.Name = "txtSubAddr";
            this.txtSubAddr.Size = new System.Drawing.Size(46, 20);
            this.txtSubAddr.TabIndex = 3;
            this.txtSubAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSubAddr.Leave += new System.EventHandler(this.txtSubAddr_Leave);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(21, 116);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(113, 23);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "&Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblDataByte
            // 
            this.lblDataByte.AutoSize = true;
            this.lblDataByte.Location = new System.Drawing.Point(27, 87);
            this.lblDataByte.Name = "lblDataByte";
            this.lblDataByte.Size = new System.Drawing.Size(57, 13);
            this.lblDataByte.TabIndex = 4;
            this.lblDataByte.Text = "Data &Byte:";
            this.lblDataByte.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDataByte
            // 
            this.txtDataByte.Location = new System.Drawing.Point(91, 84);
            this.txtDataByte.MaxLength = 4;
            this.txtDataByte.Name = "txtDataByte";
            this.txtDataByte.Size = new System.Drawing.Size(46, 20);
            this.txtDataByte.TabIndex = 5;
            this.txtDataByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDataByte.Leave += new System.EventHandler(this.txtDataByte_Leave);
            // 
            // lblSerialPort
            // 
            this.lblSerialPort.AutoSize = true;
            this.lblSerialPort.Location = new System.Drawing.Point(5, 9);
            this.lblSerialPort.Name = "lblSerialPort";
            this.lblSerialPort.Size = new System.Drawing.Size(58, 13);
            this.lblSerialPort.TabIndex = 5;
            this.lblSerialPort.Text = "Serial &Port:";
            // 
            // cbSerialPort
            // 
            this.cbSerialPort.FormattingEnabled = true;
            this.cbSerialPort.Location = new System.Drawing.Point(70, 6);
            this.cbSerialPort.Name = "cbSerialPort";
            this.cbSerialPort.Size = new System.Drawing.Size(121, 21);
            this.cbSerialPort.TabIndex = 4;
            this.cbSerialPort.SelectedIndexChanged += new System.EventHandler(this.cbSerialPort_SelectedIndexChanged);
            // 
            // lblIdentification
            // 
            this.lblIdentification.AutoSize = true;
            this.lblIdentification.Location = new System.Drawing.Point(216, 9);
            this.lblIdentification.Name = "lblIdentification";
            this.lblIdentification.Size = new System.Drawing.Size(70, 13);
            this.lblIdentification.TabIndex = 6;
            this.lblIdentification.Text = "Identification:";
            // 
            // gbLED
            // 
            this.gbLED.Controls.Add(this.rdoToggleLEDPerAcceptedCommand);
            this.gbLED.Controls.Add(this.rdoLEDFlashesAtHeartbeatRate);
            this.gbLED.Controls.Add(this.rdoLEDAlwaysOn);
            this.gbLED.Controls.Add(this.rdoLEDAlwaysOff);
            this.gbLED.Location = new System.Drawing.Point(3, 3);
            this.gbLED.Margin = new System.Windows.Forms.Padding(1);
            this.gbLED.Name = "gbLED";
            this.gbLED.Padding = new System.Windows.Forms.Padding(1);
            this.gbLED.Size = new System.Drawing.Size(245, 97);
            this.gbLED.TabIndex = 7;
            this.gbLED.TabStop = false;
            this.gbLED.Text = "Onboard Diagnostic LED";
            // 
            // rdoToggleLEDPerAcceptedCommand
            // 
            this.rdoToggleLEDPerAcceptedCommand.AutoSize = true;
            this.rdoToggleLEDPerAcceptedCommand.Location = new System.Drawing.Point(3, 70);
            this.rdoToggleLEDPerAcceptedCommand.Margin = new System.Windows.Forms.Padding(1);
            this.rdoToggleLEDPerAcceptedCommand.Name = "rdoToggleLEDPerAcceptedCommand";
            this.rdoToggleLEDPerAcceptedCommand.Size = new System.Drawing.Size(241, 17);
            this.rdoToggleLEDPerAcceptedCommand.TabIndex = 11;
            this.rdoToggleLEDPerAcceptedCommand.Text = "Toggle LED ON/OFF per accepted command";
            this.rdoToggleLEDPerAcceptedCommand.UseVisualStyleBackColor = true;
            this.rdoToggleLEDPerAcceptedCommand.CheckedChanged += new System.EventHandler(this.rdoToggleLEDPerAcceptedCommand_CheckedChanged);
            // 
            // rdoLEDFlashesAtHeartbeatRate
            // 
            this.rdoLEDFlashesAtHeartbeatRate.AutoSize = true;
            this.rdoLEDFlashesAtHeartbeatRate.Checked = true;
            this.rdoLEDFlashesAtHeartbeatRate.Location = new System.Drawing.Point(3, 52);
            this.rdoLEDFlashesAtHeartbeatRate.Margin = new System.Windows.Forms.Padding(1);
            this.rdoLEDFlashesAtHeartbeatRate.Name = "rdoLEDFlashesAtHeartbeatRate";
            this.rdoLEDFlashesAtHeartbeatRate.Size = new System.Drawing.Size(162, 17);
            this.rdoLEDFlashesAtHeartbeatRate.TabIndex = 10;
            this.rdoLEDFlashesAtHeartbeatRate.TabStop = true;
            this.rdoLEDFlashesAtHeartbeatRate.Text = "Flash LED at Heartbeat Rate";
            this.rdoLEDFlashesAtHeartbeatRate.UseVisualStyleBackColor = true;
            this.rdoLEDFlashesAtHeartbeatRate.CheckedChanged += new System.EventHandler(this.rdoLEDFlashesAtHeartbeatRate_CheckedChanged);
            // 
            // rdoLEDAlwaysOn
            // 
            this.rdoLEDAlwaysOn.AutoSize = true;
            this.rdoLEDAlwaysOn.Location = new System.Drawing.Point(3, 34);
            this.rdoLEDAlwaysOn.Margin = new System.Windows.Forms.Padding(1);
            this.rdoLEDAlwaysOn.Name = "rdoLEDAlwaysOn";
            this.rdoLEDAlwaysOn.Size = new System.Drawing.Size(101, 17);
            this.rdoLEDAlwaysOn.TabIndex = 9;
            this.rdoLEDAlwaysOn.Text = "LED Always ON";
            this.rdoLEDAlwaysOn.UseVisualStyleBackColor = true;
            this.rdoLEDAlwaysOn.CheckedChanged += new System.EventHandler(this.rdoLEDAlwaysON_CheckedChanged);
            // 
            // rdoLEDAlwaysOff
            // 
            this.rdoLEDAlwaysOff.AutoSize = true;
            this.rdoLEDAlwaysOff.Location = new System.Drawing.Point(3, 16);
            this.rdoLEDAlwaysOff.Margin = new System.Windows.Forms.Padding(1);
            this.rdoLEDAlwaysOff.Name = "rdoLEDAlwaysOff";
            this.rdoLEDAlwaysOff.Size = new System.Drawing.Size(105, 17);
            this.rdoLEDAlwaysOff.TabIndex = 8;
            this.rdoLEDAlwaysOff.Text = "LED Always OFF";
            this.rdoLEDAlwaysOff.UseVisualStyleBackColor = true;
            this.rdoLEDAlwaysOff.CheckedChanged += new System.EventHandler(this.rdoLEDAlwaysOff_CheckedChanged);
            // 
            // gbWatchdog
            // 
            this.gbWatchdog.Controls.Add(this.lblCountdownDesc);
            this.gbWatchdog.Controls.Add(this.lblWatchdogCountdown);
            this.gbWatchdog.Controls.Add(this.nudWatchdogCountdown);
            this.gbWatchdog.Controls.Add(this.chkWatchdogEnabled);
            this.gbWatchdog.Controls.Add(this.btnDisableWatchdog);
            this.gbWatchdog.Location = new System.Drawing.Point(159, 46);
            this.gbWatchdog.Margin = new System.Windows.Forms.Padding(1);
            this.gbWatchdog.Name = "gbWatchdog";
            this.gbWatchdog.Padding = new System.Windows.Forms.Padding(1);
            this.gbWatchdog.Size = new System.Drawing.Size(186, 109);
            this.gbWatchdog.TabIndex = 9;
            this.gbWatchdog.TabStop = false;
            this.gbWatchdog.Text = "Configure Watchdog Timer";
            // 
            // lblCountdownDesc
            // 
            this.lblCountdownDesc.AutoSize = true;
            this.lblCountdownDesc.BackColor = System.Drawing.SystemColors.Info;
            this.lblCountdownDesc.Location = new System.Drawing.Point(5, 79);
            this.lblCountdownDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblCountdownDesc.Name = "lblCountdownDesc";
            this.lblCountdownDesc.Size = new System.Drawing.Size(169, 13);
            this.lblCountdownDesc.TabIndex = 7;
            this.lblCountdownDesc.Text = "0=use firmware default countdown";
            // 
            // lblWatchdogCountdown
            // 
            this.lblWatchdogCountdown.AutoSize = true;
            this.lblWatchdogCountdown.Location = new System.Drawing.Point(5, 61);
            this.lblWatchdogCountdown.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblWatchdogCountdown.Name = "lblWatchdogCountdown";
            this.lblWatchdogCountdown.Size = new System.Drawing.Size(61, 13);
            this.lblWatchdogCountdown.TabIndex = 3;
            this.lblWatchdogCountdown.Text = "Countdown";
            // 
            // nudWatchdogCountdown
            // 
            this.nudWatchdogCountdown.Location = new System.Drawing.Point(69, 61);
            this.nudWatchdogCountdown.Margin = new System.Windows.Forms.Padding(1);
            this.nudWatchdogCountdown.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.nudWatchdogCountdown.Name = "nudWatchdogCountdown";
            this.nudWatchdogCountdown.Size = new System.Drawing.Size(60, 20);
            this.nudWatchdogCountdown.TabIndex = 2;
            this.nudWatchdogCountdown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudWatchdogCountdown.ValueChanged += new System.EventHandler(this.nudWatchdogCountdown_ValueChanged);
            // 
            // chkWatchdogEnabled
            // 
            this.chkWatchdogEnabled.AutoSize = true;
            this.chkWatchdogEnabled.Checked = true;
            this.chkWatchdogEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWatchdogEnabled.Location = new System.Drawing.Point(7, 42);
            this.chkWatchdogEnabled.Margin = new System.Windows.Forms.Padding(1);
            this.chkWatchdogEnabled.Name = "chkWatchdogEnabled";
            this.chkWatchdogEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkWatchdogEnabled.TabIndex = 1;
            this.chkWatchdogEnabled.Text = "Enabled";
            this.chkWatchdogEnabled.UseVisualStyleBackColor = true;
            this.chkWatchdogEnabled.CheckedChanged += new System.EventHandler(this.chkWatchdogEnabled_CheckedChanged);
            // 
            // btnDisableWatchdog
            // 
            this.btnDisableWatchdog.Location = new System.Drawing.Point(3, 16);
            this.btnDisableWatchdog.Margin = new System.Windows.Forms.Padding(1);
            this.btnDisableWatchdog.Name = "btnDisableWatchdog";
            this.btnDisableWatchdog.Size = new System.Drawing.Size(113, 22);
            this.btnDisableWatchdog.TabIndex = 0;
            this.btnDisableWatchdog.Text = "&Disable Watchdog";
            this.btnDisableWatchdog.UseVisualStyleBackColor = true;
            this.btnDisableWatchdog.Click += new System.EventHandler(this.btnDisableWatchdog_Click);
            // 
            // gbPowerDown
            // 
            this.gbPowerDown.Controls.Add(this.lblDelayDescr);
            this.gbPowerDown.Controls.Add(this.lblPowerDownDelayTime);
            this.gbPowerDown.Controls.Add(this.nudPowerDownDelay);
            this.gbPowerDown.Controls.Add(this.gbPowerDownLevel);
            this.gbPowerDown.Controls.Add(this.chkPowerDownEnabled);
            this.gbPowerDown.Location = new System.Drawing.Point(1, 224);
            this.gbPowerDown.Margin = new System.Windows.Forms.Padding(1);
            this.gbPowerDown.Name = "gbPowerDown";
            this.gbPowerDown.Padding = new System.Windows.Forms.Padding(1);
            this.gbPowerDown.Size = new System.Drawing.Size(453, 81);
            this.gbPowerDown.TabIndex = 9;
            this.gbPowerDown.TabStop = false;
            this.gbPowerDown.Text = "Synchro Power Down Control";
            // 
            // lblDelayDescr
            // 
            this.lblDelayDescr.AutoSize = true;
            this.lblDelayDescr.BackColor = System.Drawing.SystemColors.Info;
            this.lblDelayDescr.Location = new System.Drawing.Point(193, 56);
            this.lblDelayDescr.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDelayDescr.Name = "lblDelayDescr";
            this.lblDelayDescr.Size = new System.Drawing.Size(159, 13);
            this.lblDelayDescr.TabIndex = 6;
            this.lblDelayDescr.Text = "0=use firmware default of 512ms";
            // 
            // lblPowerDownDelayTime
            // 
            this.lblPowerDownDelayTime.AutoSize = true;
            this.lblPowerDownDelayTime.Location = new System.Drawing.Point(131, 34);
            this.lblPowerDownDelayTime.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblPowerDownDelayTime.Name = "lblPowerDownDelayTime";
            this.lblPowerDownDelayTime.Size = new System.Drawing.Size(59, 13);
            this.lblPowerDownDelayTime.TabIndex = 5;
            this.lblPowerDownDelayTime.Text = "Delay (ms) ";
            // 
            // nudPowerDownDelay
            // 
            this.nudPowerDownDelay.Increment = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudPowerDownDelay.Location = new System.Drawing.Point(131, 55);
            this.nudPowerDownDelay.Margin = new System.Windows.Forms.Padding(1);
            this.nudPowerDownDelay.Maximum = new decimal(new int[] {
            2016,
            0,
            0,
            0});
            this.nudPowerDownDelay.Name = "nudPowerDownDelay";
            this.nudPowerDownDelay.Size = new System.Drawing.Size(60, 20);
            this.nudPowerDownDelay.TabIndex = 4;
            this.nudPowerDownDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPowerDownDelay.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nudPowerDownDelay.ValueChanged += new System.EventHandler(this.nudPowerDownDelay_ValueChanged);
            // 
            // gbPowerDownLevel
            // 
            this.gbPowerDownLevel.Controls.Add(this.rdoPowerDownLevelHalf);
            this.gbPowerDownLevel.Controls.Add(this.rdoPowerDownLevelFull);
            this.gbPowerDownLevel.Location = new System.Drawing.Point(4, 34);
            this.gbPowerDownLevel.Margin = new System.Windows.Forms.Padding(1);
            this.gbPowerDownLevel.Name = "gbPowerDownLevel";
            this.gbPowerDownLevel.Padding = new System.Windows.Forms.Padding(1);
            this.gbPowerDownLevel.Size = new System.Drawing.Size(123, 38);
            this.gbPowerDownLevel.TabIndex = 5;
            this.gbPowerDownLevel.TabStop = false;
            this.gbPowerDownLevel.Text = "Power Down Level";
            // 
            // rdoPowerDownLevelHalf
            // 
            this.rdoPowerDownLevelHalf.AutoSize = true;
            this.rdoPowerDownLevelHalf.Location = new System.Drawing.Point(59, 16);
            this.rdoPowerDownLevelHalf.Margin = new System.Windows.Forms.Padding(1);
            this.rdoPowerDownLevelHalf.Name = "rdoPowerDownLevelHalf";
            this.rdoPowerDownLevelHalf.Size = new System.Drawing.Size(44, 17);
            this.rdoPowerDownLevelHalf.TabIndex = 7;
            this.rdoPowerDownLevelHalf.Text = "Half";
            this.rdoPowerDownLevelHalf.UseVisualStyleBackColor = true;
            this.rdoPowerDownLevelHalf.CheckedChanged += new System.EventHandler(this.rdoPowerDownLevelHalf_CheckedChanged);
            // 
            // rdoPowerDownLevelFull
            // 
            this.rdoPowerDownLevelFull.AutoSize = true;
            this.rdoPowerDownLevelFull.Checked = true;
            this.rdoPowerDownLevelFull.Location = new System.Drawing.Point(3, 16);
            this.rdoPowerDownLevelFull.Margin = new System.Windows.Forms.Padding(1);
            this.rdoPowerDownLevelFull.Name = "rdoPowerDownLevelFull";
            this.rdoPowerDownLevelFull.Size = new System.Drawing.Size(41, 17);
            this.rdoPowerDownLevelFull.TabIndex = 6;
            this.rdoPowerDownLevelFull.TabStop = true;
            this.rdoPowerDownLevelFull.Text = "Full";
            this.rdoPowerDownLevelFull.UseVisualStyleBackColor = true;
            this.rdoPowerDownLevelFull.CheckedChanged += new System.EventHandler(this.rdoPowerDownLevelFull_CheckedChanged);
            // 
            // chkPowerDownEnabled
            // 
            this.chkPowerDownEnabled.AutoSize = true;
            this.chkPowerDownEnabled.Location = new System.Drawing.Point(3, 16);
            this.chkPowerDownEnabled.Margin = new System.Windows.Forms.Padding(1);
            this.chkPowerDownEnabled.Name = "chkPowerDownEnabled";
            this.chkPowerDownEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkPowerDownEnabled.TabIndex = 4;
            this.chkPowerDownEnabled.Text = "Enabled";
            this.chkPowerDownEnabled.UseVisualStyleBackColor = true;
            this.chkPowerDownEnabled.CheckedChanged += new System.EventHandler(this.chkPowerDownEnabled_CheckedChanged);
            // 
            // gbUSBDebug
            // 
            this.gbUSBDebug.Controls.Add(this.chkUSBDebugEnabled);
            this.gbUSBDebug.Location = new System.Drawing.Point(159, 3);
            this.gbUSBDebug.Margin = new System.Windows.Forms.Padding(1);
            this.gbUSBDebug.Name = "gbUSBDebug";
            this.gbUSBDebug.Padding = new System.Windows.Forms.Padding(1);
            this.gbUSBDebug.Size = new System.Drawing.Size(185, 40);
            this.gbUSBDebug.TabIndex = 9;
            this.gbUSBDebug.TabStop = false;
            this.gbUSBDebug.Text = "USB Debug";
            // 
            // chkUSBDebugEnabled
            // 
            this.chkUSBDebugEnabled.AutoSize = true;
            this.chkUSBDebugEnabled.Location = new System.Drawing.Point(3, 16);
            this.chkUSBDebugEnabled.Margin = new System.Windows.Forms.Padding(1);
            this.chkUSBDebugEnabled.Name = "chkUSBDebugEnabled";
            this.chkUSBDebugEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkUSBDebugEnabled.TabIndex = 8;
            this.chkUSBDebugEnabled.Text = "Enabled";
            this.chkUSBDebugEnabled.UseVisualStyleBackColor = true;
            this.chkUSBDebugEnabled.CheckedChanged += new System.EventHandler(this.chkUSBDebugEnabled_CheckedChanged);
            // 
            // gbDemo
            // 
            this.gbDemo.Controls.Add(this.chkStartDemo);
            this.gbDemo.Controls.Add(this.gbDemoSpeedAndStepping);
            this.gbDemo.Controls.Add(this.gbModus);
            this.gbDemo.Controls.Add(this.gbDemoStartAndEndPositions);
            this.gbDemo.Location = new System.Drawing.Point(1, 1);
            this.gbDemo.Margin = new System.Windows.Forms.Padding(1);
            this.gbDemo.Name = "gbDemo";
            this.gbDemo.Padding = new System.Windows.Forms.Padding(1);
            this.gbDemo.Size = new System.Drawing.Size(453, 239);
            this.gbDemo.TabIndex = 9;
            this.gbDemo.TabStop = false;
            this.gbDemo.Text = "Demo Mode";
            // 
            // chkStartDemo
            // 
            this.chkStartDemo.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkStartDemo.AutoSize = true;
            this.chkStartDemo.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.chkStartDemo.Location = new System.Drawing.Point(16, 210);
            this.chkStartDemo.Margin = new System.Windows.Forms.Padding(1);
            this.chkStartDemo.Name = "chkStartDemo";
            this.chkStartDemo.Size = new System.Drawing.Size(70, 23);
            this.chkStartDemo.TabIndex = 12;
            this.chkStartDemo.Text = "Start Demo";
            this.chkStartDemo.UseVisualStyleBackColor = true;
            this.chkStartDemo.CheckedChanged += new System.EventHandler(this.chkStartDemo_CheckedChanged);
            // 
            // gbDemoSpeedAndStepping
            // 
            this.gbDemoSpeedAndStepping.Controls.Add(this.lblDemoMovementSpeed);
            this.gbDemoSpeedAndStepping.Controls.Add(this.cboDemoMovementStepSize);
            this.gbDemoSpeedAndStepping.Controls.Add(this.cboDemoMovementSpeed);
            this.gbDemoSpeedAndStepping.Controls.Add(this.lblDemoMovementStepSizeDesc);
            this.gbDemoSpeedAndStepping.Controls.Add(this.lblDemoMovementStepSize);
            this.gbDemoSpeedAndStepping.Controls.Add(this.lblDemoMovementSpeedDesc);
            this.gbDemoSpeedAndStepping.Location = new System.Drawing.Point(9, 75);
            this.gbDemoSpeedAndStepping.Margin = new System.Windows.Forms.Padding(1);
            this.gbDemoSpeedAndStepping.Name = "gbDemoSpeedAndStepping";
            this.gbDemoSpeedAndStepping.Padding = new System.Windows.Forms.Padding(1);
            this.gbDemoSpeedAndStepping.Size = new System.Drawing.Size(439, 62);
            this.gbDemoSpeedAndStepping.TabIndex = 11;
            this.gbDemoSpeedAndStepping.TabStop = false;
            this.gbDemoSpeedAndStepping.Text = "Speed and Stepping";
            // 
            // lblDemoMovementSpeed
            // 
            this.lblDemoMovementSpeed.AutoSize = true;
            this.lblDemoMovementSpeed.Location = new System.Drawing.Point(5, 19);
            this.lblDemoMovementSpeed.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoMovementSpeed.Name = "lblDemoMovementSpeed";
            this.lblDemoMovementSpeed.Size = new System.Drawing.Size(94, 13);
            this.lblDemoMovementSpeed.TabIndex = 31;
            this.lblDemoMovementSpeed.Text = "Movement Speed:";
            // 
            // cboDemoMovementStepSize
            // 
            this.cboDemoMovementStepSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDemoMovementStepSize.FormattingEnabled = true;
            this.cboDemoMovementStepSize.Items.AddRange(new object[] {
            " 1 step",
            " 2 steps",
            " 4 steps",
            " 6 steps",
            " 8 steps",
            "10 steps",
            "12 steps",
            "14 steps",
            "16 steps",
            "18 steps",
            "20 steps",
            "22 steps",
            "24 steps",
            "26 steps",
            "28 steps",
            "30 steps"});
            this.cboDemoMovementStepSize.Location = new System.Drawing.Point(134, 38);
            this.cboDemoMovementStepSize.Margin = new System.Windows.Forms.Padding(1);
            this.cboDemoMovementStepSize.Name = "cboDemoMovementStepSize";
            this.cboDemoMovementStepSize.Size = new System.Drawing.Size(77, 21);
            this.cboDemoMovementStepSize.TabIndex = 51;
            this.cboDemoMovementStepSize.SelectedIndexChanged += new System.EventHandler(this.cboDemoMovementStepSize_SelectedIndexChanged);
            // 
            // cboDemoMovementSpeed
            // 
            this.cboDemoMovementSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDemoMovementSpeed.FormattingEnabled = true;
            this.cboDemoMovementSpeed.Items.AddRange(new object[] {
            "100 ms",
            "500 ms",
            "1 second",
            "2 seconds"});
            this.cboDemoMovementSpeed.Location = new System.Drawing.Point(134, 18);
            this.cboDemoMovementSpeed.Margin = new System.Windows.Forms.Padding(1);
            this.cboDemoMovementSpeed.Name = "cboDemoMovementSpeed";
            this.cboDemoMovementSpeed.Size = new System.Drawing.Size(77, 21);
            this.cboDemoMovementSpeed.TabIndex = 10;
            this.cboDemoMovementSpeed.SelectedIndexChanged += new System.EventHandler(this.cboDemoMovementSpeed_SelectedIndexChanged);
            // 
            // lblDemoMovementStepSizeDesc
            // 
            this.lblDemoMovementStepSizeDesc.AutoSize = true;
            this.lblDemoMovementStepSizeDesc.Location = new System.Drawing.Point(212, 41);
            this.lblDemoMovementStepSizeDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoMovementStepSizeDesc.Name = "lblDemoMovementStepSizeDesc";
            this.lblDemoMovementStepSizeDesc.Size = new System.Drawing.Size(168, 13);
            this.lblDemoMovementStepSizeDesc.TabIndex = 50;
            this.lblDemoMovementStepSizeDesc.Text = "(Increment in Position Per Update)";
            // 
            // lblDemoMovementStepSize
            // 
            this.lblDemoMovementStepSize.AutoSize = true;
            this.lblDemoMovementStepSize.Location = new System.Drawing.Point(5, 40);
            this.lblDemoMovementStepSize.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoMovementStepSize.Name = "lblDemoMovementStepSize";
            this.lblDemoMovementStepSize.Size = new System.Drawing.Size(108, 13);
            this.lblDemoMovementStepSize.TabIndex = 33;
            this.lblDemoMovementStepSize.Text = "Movement Step Size:";
            // 
            // lblDemoMovementSpeedDesc
            // 
            this.lblDemoMovementSpeedDesc.AutoSize = true;
            this.lblDemoMovementSpeedDesc.Location = new System.Drawing.Point(212, 19);
            this.lblDemoMovementSpeedDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoMovementSpeedDesc.Name = "lblDemoMovementSpeedDesc";
            this.lblDemoMovementSpeedDesc.Size = new System.Drawing.Size(168, 13);
            this.lblDemoMovementSpeedDesc.TabIndex = 49;
            this.lblDemoMovementSpeedDesc.Text = "(Delay Between Position Updates)";
            // 
            // gbModus
            // 
            this.gbModus.Controls.Add(this.rdoDemoModusStartToEndJumpToStart);
            this.gbModus.Controls.Add(this.rdoModusStartToEndToStart);
            this.gbModus.Location = new System.Drawing.Point(9, 140);
            this.gbModus.Margin = new System.Windows.Forms.Padding(1);
            this.gbModus.Name = "gbModus";
            this.gbModus.Padding = new System.Windows.Forms.Padding(1);
            this.gbModus.Size = new System.Drawing.Size(439, 57);
            this.gbModus.TabIndex = 11;
            this.gbModus.TabStop = false;
            this.gbModus.Text = "Modus";
            // 
            // rdoDemoModusStartToEndJumpToStart
            // 
            this.rdoDemoModusStartToEndJumpToStart.AutoSize = true;
            this.rdoDemoModusStartToEndJumpToStart.Location = new System.Drawing.Point(6, 34);
            this.rdoDemoModusStartToEndJumpToStart.Margin = new System.Windows.Forms.Padding(1);
            this.rdoDemoModusStartToEndJumpToStart.Name = "rdoDemoModusStartToEndJumpToStart";
            this.rdoDemoModusStartToEndJumpToStart.Size = new System.Drawing.Size(354, 17);
            this.rdoDemoModusStartToEndJumpToStart.TabIndex = 54;
            this.rdoDemoModusStartToEndJumpToStart.Text = "Sweep \"up\" from start to end position, then jump back to start position";
            this.rdoDemoModusStartToEndJumpToStart.UseVisualStyleBackColor = true;
            this.rdoDemoModusStartToEndJumpToStart.CheckedChanged += new System.EventHandler(this.rdoDemoModusStartToEndJumpToStart_CheckedChanged);
            // 
            // rdoModusStartToEndToStart
            // 
            this.rdoModusStartToEndToStart.AutoSize = true;
            this.rdoModusStartToEndToStart.Checked = true;
            this.rdoModusStartToEndToStart.Location = new System.Drawing.Point(6, 16);
            this.rdoModusStartToEndToStart.Margin = new System.Windows.Forms.Padding(1);
            this.rdoModusStartToEndToStart.Name = "rdoModusStartToEndToStart";
            this.rdoModusStartToEndToStart.Size = new System.Drawing.Size(419, 17);
            this.rdoModusStartToEndToStart.TabIndex = 53;
            this.rdoModusStartToEndToStart.TabStop = true;
            this.rdoModusStartToEndToStart.Text = "Sweep \"up\" from start to end position, then sweep \"down\" from end to start positi" +
    "on";
            this.rdoModusStartToEndToStart.UseVisualStyleBackColor = true;
            this.rdoModusStartToEndToStart.CheckedChanged += new System.EventHandler(this.rdoModusStartToEndToStart_CheckedChanged);
            // 
            // gbDemoStartAndEndPositions
            // 
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoEndPositionHex);
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoStartPositionHex);
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoEndPositionDegrees);
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoStartPositionDegrees);
            this.gbDemoStartAndEndPositions.Controls.Add(this.nudDemoEndPositionDecimal);
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoEndPositionDecimal);
            this.gbDemoStartAndEndPositions.Controls.Add(this.nudDemoStartPositionDecimal);
            this.gbDemoStartAndEndPositions.Controls.Add(this.lblDemoStartPosition);
            this.gbDemoStartAndEndPositions.Location = new System.Drawing.Point(9, 16);
            this.gbDemoStartAndEndPositions.Margin = new System.Windows.Forms.Padding(1);
            this.gbDemoStartAndEndPositions.Name = "gbDemoStartAndEndPositions";
            this.gbDemoStartAndEndPositions.Padding = new System.Windows.Forms.Padding(1);
            this.gbDemoStartAndEndPositions.Size = new System.Drawing.Size(439, 53);
            this.gbDemoStartAndEndPositions.TabIndex = 9;
            this.gbDemoStartAndEndPositions.TabStop = false;
            this.gbDemoStartAndEndPositions.Text = "Start and End Positions";
            // 
            // lblDemoEndPositionHex
            // 
            this.lblDemoEndPositionHex.AutoSize = true;
            this.lblDemoEndPositionHex.Location = new System.Drawing.Point(261, 34);
            this.lblDemoEndPositionHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoEndPositionHex.Name = "lblDemoEndPositionHex";
            this.lblDemoEndPositionHex.Size = new System.Drawing.Size(29, 13);
            this.lblDemoEndPositionHex.TabIndex = 47;
            this.lblDemoEndPositionHex.Text = "Hex:";
            // 
            // lblDemoStartPositionHex
            // 
            this.lblDemoStartPositionHex.AutoSize = true;
            this.lblDemoStartPositionHex.Location = new System.Drawing.Point(261, 14);
            this.lblDemoStartPositionHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoStartPositionHex.Name = "lblDemoStartPositionHex";
            this.lblDemoStartPositionHex.Size = new System.Drawing.Size(29, 13);
            this.lblDemoStartPositionHex.TabIndex = 46;
            this.lblDemoStartPositionHex.Text = "Hex:";
            // 
            // lblDemoEndPositionDegrees
            // 
            this.lblDemoEndPositionDegrees.AutoSize = true;
            this.lblDemoEndPositionDegrees.Location = new System.Drawing.Point(176, 34);
            this.lblDemoEndPositionDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoEndPositionDegrees.Name = "lblDemoEndPositionDegrees";
            this.lblDemoEndPositionDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblDemoEndPositionDegrees.TabIndex = 45;
            this.lblDemoEndPositionDegrees.Text = "Degrees:";
            // 
            // lblDemoStartPositionDegrees
            // 
            this.lblDemoStartPositionDegrees.AutoSize = true;
            this.lblDemoStartPositionDegrees.Location = new System.Drawing.Point(176, 14);
            this.lblDemoStartPositionDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoStartPositionDegrees.Name = "lblDemoStartPositionDegrees";
            this.lblDemoStartPositionDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblDemoStartPositionDegrees.TabIndex = 44;
            this.lblDemoStartPositionDegrees.Text = "Degrees:";
            // 
            // nudDemoEndPositionDecimal
            // 
            this.nudDemoEndPositionDecimal.Location = new System.Drawing.Point(117, 32);
            this.nudDemoEndPositionDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudDemoEndPositionDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDemoEndPositionDecimal.Name = "nudDemoEndPositionDecimal";
            this.nudDemoEndPositionDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudDemoEndPositionDecimal.TabIndex = 39;
            this.nudDemoEndPositionDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDemoEndPositionDecimal.ValueChanged += new System.EventHandler(this.nudDemoEndPositionDecimal_ValueChanged);
            // 
            // lblDemoEndPositionDecimal
            // 
            this.lblDemoEndPositionDecimal.AutoSize = true;
            this.lblDemoEndPositionDecimal.Location = new System.Drawing.Point(6, 34);
            this.lblDemoEndPositionDecimal.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoEndPositionDecimal.Name = "lblDemoEndPositionDecimal";
            this.lblDemoEndPositionDecimal.Size = new System.Drawing.Size(74, 13);
            this.lblDemoEndPositionDecimal.TabIndex = 38;
            this.lblDemoEndPositionDecimal.Text = "End (decimal):";
            // 
            // nudDemoStartPositionDecimal
            // 
            this.nudDemoStartPositionDecimal.Location = new System.Drawing.Point(117, 13);
            this.nudDemoStartPositionDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudDemoStartPositionDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDemoStartPositionDecimal.Name = "nudDemoStartPositionDecimal";
            this.nudDemoStartPositionDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudDemoStartPositionDecimal.TabIndex = 37;
            this.nudDemoStartPositionDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDemoStartPositionDecimal.ValueChanged += new System.EventHandler(this.nudDemoStartPositionDecimal_ValueChanged);
            // 
            // lblDemoStartPosition
            // 
            this.lblDemoStartPosition.AutoSize = true;
            this.lblDemoStartPosition.Location = new System.Drawing.Point(6, 14);
            this.lblDemoStartPosition.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDemoStartPosition.Name = "lblDemoStartPosition";
            this.lblDemoStartPosition.Size = new System.Drawing.Size(77, 13);
            this.lblDemoStartPosition.TabIndex = 36;
            this.lblDemoStartPosition.Text = "Start (decimal):";
            // 
            // gbStatorBaseAngles
            // 
            this.gbStatorBaseAngles.Controls.Add(this.btnUpdateStatorBaseAngles);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS3BaseAngleHex);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS2BaseAngleHex);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS1BaseAngleHex);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS3BaseAngleDegrees);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS2BaseAngleDegrees);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS1BaseAngleDegrees);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS3BaseAngleMSB);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS2BaseAngleMSB);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS1BaseAngleMSB);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS3BaseAngleLSB);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS2BaseAngleLSB);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS1BaseAngleLSB);
            this.gbStatorBaseAngles.Controls.Add(this.nudStatorS3BaseAngle);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS3BaseAngle);
            this.gbStatorBaseAngles.Controls.Add(this.nudStatorS2BaseAngle);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS2BaseAngle);
            this.gbStatorBaseAngles.Controls.Add(this.nudStatorS1BaseAngle);
            this.gbStatorBaseAngles.Controls.Add(this.lblStatorS1BaseAngle);
            this.gbStatorBaseAngles.Location = new System.Drawing.Point(1, 1);
            this.gbStatorBaseAngles.Margin = new System.Windows.Forms.Padding(1);
            this.gbStatorBaseAngles.Name = "gbStatorBaseAngles";
            this.gbStatorBaseAngles.Padding = new System.Windows.Forms.Padding(1);
            this.gbStatorBaseAngles.Size = new System.Drawing.Size(453, 118);
            this.gbStatorBaseAngles.TabIndex = 10;
            this.gbStatorBaseAngles.TabStop = false;
            this.gbStatorBaseAngles.Text = "Stator Base Angles";
            // 
            // btnUpdateStatorBaseAngles
            // 
            this.btnUpdateStatorBaseAngles.Location = new System.Drawing.Point(103, 86);
            this.btnUpdateStatorBaseAngles.Margin = new System.Windows.Forms.Padding(1);
            this.btnUpdateStatorBaseAngles.Name = "btnUpdateStatorBaseAngles";
            this.btnUpdateStatorBaseAngles.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateStatorBaseAngles.TabIndex = 8;
            this.btnUpdateStatorBaseAngles.Text = "&Update";
            this.btnUpdateStatorBaseAngles.UseVisualStyleBackColor = true;
            this.btnUpdateStatorBaseAngles.Click += new System.EventHandler(this.btnUpdateStatorBaseAngles_Click);
            // 
            // lblStatorS3BaseAngleHex
            // 
            this.lblStatorS3BaseAngleHex.AutoSize = true;
            this.lblStatorS3BaseAngleHex.Location = new System.Drawing.Point(269, 66);
            this.lblStatorS3BaseAngleHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS3BaseAngleHex.Name = "lblStatorS3BaseAngleHex";
            this.lblStatorS3BaseAngleHex.Size = new System.Drawing.Size(29, 13);
            this.lblStatorS3BaseAngleHex.TabIndex = 24;
            this.lblStatorS3BaseAngleHex.Text = "Hex:";
            // 
            // lblStatorS2BaseAngleHex
            // 
            this.lblStatorS2BaseAngleHex.AutoSize = true;
            this.lblStatorS2BaseAngleHex.Location = new System.Drawing.Point(269, 46);
            this.lblStatorS2BaseAngleHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS2BaseAngleHex.Name = "lblStatorS2BaseAngleHex";
            this.lblStatorS2BaseAngleHex.Size = new System.Drawing.Size(29, 13);
            this.lblStatorS2BaseAngleHex.TabIndex = 23;
            this.lblStatorS2BaseAngleHex.Text = "Hex:";
            // 
            // lblStatorS1BaseAngleHex
            // 
            this.lblStatorS1BaseAngleHex.AutoSize = true;
            this.lblStatorS1BaseAngleHex.Location = new System.Drawing.Point(269, 27);
            this.lblStatorS1BaseAngleHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS1BaseAngleHex.Name = "lblStatorS1BaseAngleHex";
            this.lblStatorS1BaseAngleHex.Size = new System.Drawing.Size(29, 13);
            this.lblStatorS1BaseAngleHex.TabIndex = 22;
            this.lblStatorS1BaseAngleHex.Text = "Hex:";
            // 
            // lblStatorS3BaseAngleDegrees
            // 
            this.lblStatorS3BaseAngleDegrees.AutoSize = true;
            this.lblStatorS3BaseAngleDegrees.Location = new System.Drawing.Point(185, 66);
            this.lblStatorS3BaseAngleDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS3BaseAngleDegrees.Name = "lblStatorS3BaseAngleDegrees";
            this.lblStatorS3BaseAngleDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblStatorS3BaseAngleDegrees.TabIndex = 21;
            this.lblStatorS3BaseAngleDegrees.Text = "Degrees:";
            // 
            // lblStatorS2BaseAngleDegrees
            // 
            this.lblStatorS2BaseAngleDegrees.AutoSize = true;
            this.lblStatorS2BaseAngleDegrees.Location = new System.Drawing.Point(185, 46);
            this.lblStatorS2BaseAngleDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS2BaseAngleDegrees.Name = "lblStatorS2BaseAngleDegrees";
            this.lblStatorS2BaseAngleDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblStatorS2BaseAngleDegrees.TabIndex = 20;
            this.lblStatorS2BaseAngleDegrees.Text = "Degrees:";
            // 
            // lblStatorS1BaseAngleDegrees
            // 
            this.lblStatorS1BaseAngleDegrees.AutoSize = true;
            this.lblStatorS1BaseAngleDegrees.Location = new System.Drawing.Point(185, 27);
            this.lblStatorS1BaseAngleDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS1BaseAngleDegrees.Name = "lblStatorS1BaseAngleDegrees";
            this.lblStatorS1BaseAngleDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblStatorS1BaseAngleDegrees.TabIndex = 19;
            this.lblStatorS1BaseAngleDegrees.Text = "Degrees:";
            // 
            // lblStatorS3BaseAngleMSB
            // 
            this.lblStatorS3BaseAngleMSB.AutoSize = true;
            this.lblStatorS3BaseAngleMSB.Location = new System.Drawing.Point(385, 66);
            this.lblStatorS3BaseAngleMSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS3BaseAngleMSB.Name = "lblStatorS3BaseAngleMSB";
            this.lblStatorS3BaseAngleMSB.Size = new System.Drawing.Size(33, 13);
            this.lblStatorS3BaseAngleMSB.TabIndex = 18;
            this.lblStatorS3BaseAngleMSB.Text = "MSB:";
            // 
            // lblStatorS2BaseAngleMSB
            // 
            this.lblStatorS2BaseAngleMSB.AutoSize = true;
            this.lblStatorS2BaseAngleMSB.Location = new System.Drawing.Point(385, 46);
            this.lblStatorS2BaseAngleMSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS2BaseAngleMSB.Name = "lblStatorS2BaseAngleMSB";
            this.lblStatorS2BaseAngleMSB.Size = new System.Drawing.Size(33, 13);
            this.lblStatorS2BaseAngleMSB.TabIndex = 17;
            this.lblStatorS2BaseAngleMSB.Text = "MSB:";
            // 
            // lblStatorS1BaseAngleMSB
            // 
            this.lblStatorS1BaseAngleMSB.AutoSize = true;
            this.lblStatorS1BaseAngleMSB.Location = new System.Drawing.Point(385, 27);
            this.lblStatorS1BaseAngleMSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS1BaseAngleMSB.Name = "lblStatorS1BaseAngleMSB";
            this.lblStatorS1BaseAngleMSB.Size = new System.Drawing.Size(33, 13);
            this.lblStatorS1BaseAngleMSB.TabIndex = 16;
            this.lblStatorS1BaseAngleMSB.Text = "MSB:";
            // 
            // lblStatorS3BaseAngleLSB
            // 
            this.lblStatorS3BaseAngleLSB.AutoSize = true;
            this.lblStatorS3BaseAngleLSB.Location = new System.Drawing.Point(333, 66);
            this.lblStatorS3BaseAngleLSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS3BaseAngleLSB.Name = "lblStatorS3BaseAngleLSB";
            this.lblStatorS3BaseAngleLSB.Size = new System.Drawing.Size(30, 13);
            this.lblStatorS3BaseAngleLSB.TabIndex = 15;
            this.lblStatorS3BaseAngleLSB.Text = "LSB:";
            // 
            // lblStatorS2BaseAngleLSB
            // 
            this.lblStatorS2BaseAngleLSB.AutoSize = true;
            this.lblStatorS2BaseAngleLSB.Location = new System.Drawing.Point(333, 46);
            this.lblStatorS2BaseAngleLSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS2BaseAngleLSB.Name = "lblStatorS2BaseAngleLSB";
            this.lblStatorS2BaseAngleLSB.Size = new System.Drawing.Size(30, 13);
            this.lblStatorS2BaseAngleLSB.TabIndex = 14;
            this.lblStatorS2BaseAngleLSB.Text = "LSB:";
            // 
            // lblStatorS1BaseAngleLSB
            // 
            this.lblStatorS1BaseAngleLSB.AutoSize = true;
            this.lblStatorS1BaseAngleLSB.Location = new System.Drawing.Point(333, 27);
            this.lblStatorS1BaseAngleLSB.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS1BaseAngleLSB.Name = "lblStatorS1BaseAngleLSB";
            this.lblStatorS1BaseAngleLSB.Size = new System.Drawing.Size(30, 13);
            this.lblStatorS1BaseAngleLSB.TabIndex = 13;
            this.lblStatorS1BaseAngleLSB.Text = "LSB:";
            // 
            // nudStatorS3BaseAngle
            // 
            this.nudStatorS3BaseAngle.Location = new System.Drawing.Point(131, 64);
            this.nudStatorS3BaseAngle.Margin = new System.Windows.Forms.Padding(1);
            this.nudStatorS3BaseAngle.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudStatorS3BaseAngle.Name = "nudStatorS3BaseAngle";
            this.nudStatorS3BaseAngle.Size = new System.Drawing.Size(47, 20);
            this.nudStatorS3BaseAngle.TabIndex = 12;
            this.nudStatorS3BaseAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudStatorS3BaseAngle.ValueChanged += new System.EventHandler(this.nudStatorS3BaseAngle_ValueChanged);
            // 
            // lblStatorS3BaseAngle
            // 
            this.lblStatorS3BaseAngle.AutoSize = true;
            this.lblStatorS3BaseAngle.Location = new System.Drawing.Point(15, 66);
            this.lblStatorS3BaseAngle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS3BaseAngle.Name = "lblStatorS3BaseAngle";
            this.lblStatorS3BaseAngle.Size = new System.Drawing.Size(97, 13);
            this.lblStatorS3BaseAngle.TabIndex = 11;
            this.lblStatorS3BaseAngle.Text = "S3 offset (decimal):";
            // 
            // nudStatorS2BaseAngle
            // 
            this.nudStatorS2BaseAngle.Location = new System.Drawing.Point(131, 45);
            this.nudStatorS2BaseAngle.Margin = new System.Windows.Forms.Padding(1);
            this.nudStatorS2BaseAngle.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudStatorS2BaseAngle.Name = "nudStatorS2BaseAngle";
            this.nudStatorS2BaseAngle.Size = new System.Drawing.Size(47, 20);
            this.nudStatorS2BaseAngle.TabIndex = 10;
            this.nudStatorS2BaseAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudStatorS2BaseAngle.ValueChanged += new System.EventHandler(this.nudStatorS2BaseAngle_ValueChanged);
            // 
            // lblStatorS2BaseAngle
            // 
            this.lblStatorS2BaseAngle.AutoSize = true;
            this.lblStatorS2BaseAngle.Location = new System.Drawing.Point(15, 46);
            this.lblStatorS2BaseAngle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS2BaseAngle.Name = "lblStatorS2BaseAngle";
            this.lblStatorS2BaseAngle.Size = new System.Drawing.Size(97, 13);
            this.lblStatorS2BaseAngle.TabIndex = 9;
            this.lblStatorS2BaseAngle.Text = "S2 offset (decimal):";
            // 
            // nudStatorS1BaseAngle
            // 
            this.nudStatorS1BaseAngle.Location = new System.Drawing.Point(131, 25);
            this.nudStatorS1BaseAngle.Margin = new System.Windows.Forms.Padding(1);
            this.nudStatorS1BaseAngle.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudStatorS1BaseAngle.Name = "nudStatorS1BaseAngle";
            this.nudStatorS1BaseAngle.Size = new System.Drawing.Size(47, 20);
            this.nudStatorS1BaseAngle.TabIndex = 8;
            this.nudStatorS1BaseAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudStatorS1BaseAngle.ValueChanged += new System.EventHandler(this.nudStatorS1BaseAngle_ValueChanged);
            // 
            // lblStatorS1BaseAngle
            // 
            this.lblStatorS1BaseAngle.AutoSize = true;
            this.lblStatorS1BaseAngle.Location = new System.Drawing.Point(15, 27);
            this.lblStatorS1BaseAngle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorS1BaseAngle.Name = "lblStatorS1BaseAngle";
            this.lblStatorS1BaseAngle.Size = new System.Drawing.Size(97, 13);
            this.lblStatorS1BaseAngle.TabIndex = 0;
            this.lblStatorS1BaseAngle.Text = "S1 offset (decimal):";
            // 
            // gbMovementLimits
            // 
            this.gbMovementLimits.Controls.Add(this.lblLimitMaximumHex);
            this.gbMovementLimits.Controls.Add(this.lblLimitMaximumDegrees);
            this.gbMovementLimits.Controls.Add(this.lblLimitMinimumHex);
            this.gbMovementLimits.Controls.Add(this.lblLimitMinimumDegrees);
            this.gbMovementLimits.Controls.Add(this.lblLimitMinDesc);
            this.gbMovementLimits.Controls.Add(this.lblLimitMaxDesc);
            this.gbMovementLimits.Controls.Add(this.nudLimitMax);
            this.gbMovementLimits.Controls.Add(this.lblLimitMax);
            this.gbMovementLimits.Controls.Add(this.nudLimitMin);
            this.gbMovementLimits.Controls.Add(this.lblLimitMin);
            this.gbMovementLimits.Location = new System.Drawing.Point(1, 122);
            this.gbMovementLimits.Margin = new System.Windows.Forms.Padding(1);
            this.gbMovementLimits.Name = "gbMovementLimits";
            this.gbMovementLimits.Padding = new System.Windows.Forms.Padding(1);
            this.gbMovementLimits.Size = new System.Drawing.Size(453, 99);
            this.gbMovementLimits.TabIndex = 10;
            this.gbMovementLimits.TabStop = false;
            this.gbMovementLimits.Text = "Movement Limits";
            // 
            // lblLimitMaximumHex
            // 
            this.lblLimitMaximumHex.AutoSize = true;
            this.lblLimitMaximumHex.Location = new System.Drawing.Point(269, 59);
            this.lblLimitMaximumHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMaximumHex.Name = "lblLimitMaximumHex";
            this.lblLimitMaximumHex.Size = new System.Drawing.Size(29, 13);
            this.lblLimitMaximumHex.TabIndex = 30;
            this.lblLimitMaximumHex.Text = "Hex:";
            // 
            // lblLimitMaximumDegrees
            // 
            this.lblLimitMaximumDegrees.AutoSize = true;
            this.lblLimitMaximumDegrees.Location = new System.Drawing.Point(185, 59);
            this.lblLimitMaximumDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMaximumDegrees.Name = "lblLimitMaximumDegrees";
            this.lblLimitMaximumDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblLimitMaximumDegrees.TabIndex = 29;
            this.lblLimitMaximumDegrees.Text = "Degrees:";
            // 
            // lblLimitMinimumHex
            // 
            this.lblLimitMinimumHex.AutoSize = true;
            this.lblLimitMinimumHex.Location = new System.Drawing.Point(269, 17);
            this.lblLimitMinimumHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMinimumHex.Name = "lblLimitMinimumHex";
            this.lblLimitMinimumHex.Size = new System.Drawing.Size(29, 13);
            this.lblLimitMinimumHex.TabIndex = 26;
            this.lblLimitMinimumHex.Text = "Hex:";
            // 
            // lblLimitMinimumDegrees
            // 
            this.lblLimitMinimumDegrees.AutoSize = true;
            this.lblLimitMinimumDegrees.Location = new System.Drawing.Point(185, 17);
            this.lblLimitMinimumDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMinimumDegrees.Name = "lblLimitMinimumDegrees";
            this.lblLimitMinimumDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblLimitMinimumDegrees.TabIndex = 25;
            this.lblLimitMinimumDegrees.Text = "Degrees:";
            // 
            // lblLimitMinDesc
            // 
            this.lblLimitMinDesc.AutoSize = true;
            this.lblLimitMinDesc.BackColor = System.Drawing.SystemColors.Info;
            this.lblLimitMinDesc.Location = new System.Drawing.Point(131, 36);
            this.lblLimitMinDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMinDesc.Name = "lblLimitMinDesc";
            this.lblLimitMinDesc.Size = new System.Drawing.Size(94, 13);
            this.lblLimitMinDesc.TabIndex = 8;
            this.lblLimitMinDesc.Text = "0=no limit minimum";
            // 
            // lblLimitMaxDesc
            // 
            this.lblLimitMaxDesc.AutoSize = true;
            this.lblLimitMaxDesc.BackColor = System.Drawing.SystemColors.Info;
            this.lblLimitMaxDesc.Location = new System.Drawing.Point(131, 78);
            this.lblLimitMaxDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMaxDesc.Name = "lblLimitMaxDesc";
            this.lblLimitMaxDesc.Size = new System.Drawing.Size(109, 13);
            this.lblLimitMaxDesc.TabIndex = 11;
            this.lblLimitMaxDesc.Text = "255=no limit maximum";
            // 
            // nudLimitMax
            // 
            this.nudLimitMax.Location = new System.Drawing.Point(131, 58);
            this.nudLimitMax.Margin = new System.Windows.Forms.Padding(1);
            this.nudLimitMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLimitMax.Name = "nudLimitMax";
            this.nudLimitMax.Size = new System.Drawing.Size(47, 20);
            this.nudLimitMax.TabIndex = 28;
            this.nudLimitMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLimitMax.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLimitMax.ValueChanged += new System.EventHandler(this.nudLimitMax_ValueChanged);
            // 
            // lblLimitMax
            // 
            this.lblLimitMax.AutoSize = true;
            this.lblLimitMax.Location = new System.Drawing.Point(6, 59);
            this.lblLimitMax.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMax.Name = "lblLimitMax";
            this.lblLimitMax.Size = new System.Drawing.Size(112, 13);
            this.lblLimitMax.TabIndex = 27;
            this.lblLimitMax.Text = "LIMIT_MAX (decimal):";
            // 
            // nudLimitMin
            // 
            this.nudLimitMin.Location = new System.Drawing.Point(131, 16);
            this.nudLimitMin.Margin = new System.Windows.Forms.Padding(1);
            this.nudLimitMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLimitMin.Name = "nudLimitMin";
            this.nudLimitMin.Size = new System.Drawing.Size(47, 20);
            this.nudLimitMin.TabIndex = 26;
            this.nudLimitMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLimitMin.ValueChanged += new System.EventHandler(this.nudLimitMin_ValueChanged);
            // 
            // lblLimitMin
            // 
            this.lblLimitMin.AutoSize = true;
            this.lblLimitMin.Location = new System.Drawing.Point(6, 17);
            this.lblLimitMin.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLimitMin.Name = "lblLimitMin";
            this.lblLimitMin.Size = new System.Drawing.Size(109, 13);
            this.lblLimitMin.TabIndex = 25;
            this.lblLimitMin.Text = "LIMIT_MIN (decimal):";
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.tabControl1);
            this.gbMain.Location = new System.Drawing.Point(1, 27);
            this.gbMain.Margin = new System.Windows.Forms.Padding(1);
            this.gbMain.Name = "gbMain";
            this.gbMain.Padding = new System.Windows.Forms.Padding(1);
            this.gbMain.Size = new System.Drawing.Size(530, 493);
            this.gbMain.TabIndex = 11;
            this.gbMain.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSynchroSetup);
            this.tabControl1.Controls.Add(this.tabSynchroControl);
            this.tabControl1.Controls.Add(this.tabDigitalAndPWMOutputs);
            this.tabControl1.Controls.Add(this.tabRawData);
            this.tabControl1.Controls.Add(this.tabDemoMode);
            this.tabControl1.Controls.Add(this.tabDiagnosticLED);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(1, 14);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(528, 478);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSynchroSetup
            // 
            this.tabSynchroSetup.Controls.Add(this.gbUpdateRateControl);
            this.tabSynchroSetup.Controls.Add(this.gbMovementLimits);
            this.tabSynchroSetup.Controls.Add(this.gbPowerDown);
            this.tabSynchroSetup.Controls.Add(this.gbStatorBaseAngles);
            this.tabSynchroSetup.Location = new System.Drawing.Point(4, 22);
            this.tabSynchroSetup.Margin = new System.Windows.Forms.Padding(1);
            this.tabSynchroSetup.Name = "tabSynchroSetup";
            this.tabSynchroSetup.Size = new System.Drawing.Size(520, 452);
            this.tabSynchroSetup.TabIndex = 5;
            this.tabSynchroSetup.Text = "Synchro Setup";
            this.tabSynchroSetup.UseVisualStyleBackColor = true;
            // 
            // gbUpdateRateControl
            // 
            this.gbUpdateRateControl.Controls.Add(this.nudUpdateRateControlSpeed);
            this.gbUpdateRateControl.Controls.Add(this.chkUpdateRateControlShortestPath);
            this.gbUpdateRateControl.Controls.Add(this.lblUpdateRateControlSpeedDesc);
            this.gbUpdateRateControl.Controls.Add(this.lblUpdateRateControlSpeed);
            this.gbUpdateRateControl.Controls.Add(this.gbURCMode);
            this.gbUpdateRateControl.Location = new System.Drawing.Point(3, 308);
            this.gbUpdateRateControl.Margin = new System.Windows.Forms.Padding(1);
            this.gbUpdateRateControl.Name = "gbUpdateRateControl";
            this.gbUpdateRateControl.Padding = new System.Windows.Forms.Padding(1);
            this.gbUpdateRateControl.Size = new System.Drawing.Size(453, 146);
            this.gbUpdateRateControl.TabIndex = 11;
            this.gbUpdateRateControl.TabStop = false;
            this.gbUpdateRateControl.Text = "Update Rate Control (URC)";
            // 
            // nudUpdateRateControlSpeed
            // 
            this.nudUpdateRateControlSpeed.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudUpdateRateControlSpeed.Location = new System.Drawing.Point(43, 99);
            this.nudUpdateRateControlSpeed.Margin = new System.Windows.Forms.Padding(1);
            this.nudUpdateRateControlSpeed.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudUpdateRateControlSpeed.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudUpdateRateControlSpeed.Name = "nudUpdateRateControlSpeed";
            this.nudUpdateRateControlSpeed.Size = new System.Drawing.Size(47, 20);
            this.nudUpdateRateControlSpeed.TabIndex = 41;
            this.nudUpdateRateControlSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudUpdateRateControlSpeed.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudUpdateRateControlSpeed.ValueChanged += new System.EventHandler(this.nudUpdateRateControlSpeed_ValueChanged);
            // 
            // chkUpdateRateControlShortestPath
            // 
            this.chkUpdateRateControlShortestPath.AutoSize = true;
            this.chkUpdateRateControlShortestPath.Checked = true;
            this.chkUpdateRateControlShortestPath.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateRateControlShortestPath.Location = new System.Drawing.Point(7, 119);
            this.chkUpdateRateControlShortestPath.Margin = new System.Windows.Forms.Padding(1);
            this.chkUpdateRateControlShortestPath.Name = "chkUpdateRateControlShortestPath";
            this.chkUpdateRateControlShortestPath.Size = new System.Drawing.Size(145, 17);
            this.chkUpdateRateControlShortestPath.TabIndex = 44;
            this.chkUpdateRateControlShortestPath.Text = "Move using shortest path";
            this.chkUpdateRateControlShortestPath.UseVisualStyleBackColor = true;
            this.chkUpdateRateControlShortestPath.CheckedChanged += new System.EventHandler(this.chkUpdateRateControlShortestPath_CheckedChanged);
            // 
            // lblUpdateRateControlSpeedDesc
            // 
            this.lblUpdateRateControlSpeedDesc.AutoSize = true;
            this.lblUpdateRateControlSpeedDesc.Location = new System.Drawing.Point(94, 100);
            this.lblUpdateRateControlSpeedDesc.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblUpdateRateControlSpeedDesc.Name = "lblUpdateRateControlSpeedDesc";
            this.lblUpdateRateControlSpeedDesc.Size = new System.Drawing.Size(155, 13);
            this.lblUpdateRateControlSpeedDesc.TabIndex = 43;
            this.lblUpdateRateControlSpeedDesc.Text = "ms delay time between updates";
            // 
            // lblUpdateRateControlSpeed
            // 
            this.lblUpdateRateControlSpeed.AutoSize = true;
            this.lblUpdateRateControlSpeed.Location = new System.Drawing.Point(5, 100);
            this.lblUpdateRateControlSpeed.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblUpdateRateControlSpeed.Name = "lblUpdateRateControlSpeed";
            this.lblUpdateRateControlSpeed.Size = new System.Drawing.Size(41, 13);
            this.lblUpdateRateControlSpeed.TabIndex = 41;
            this.lblUpdateRateControlSpeed.Text = "Speed:";
            // 
            // gbURCMode
            // 
            this.gbURCMode.Controls.Add(this.lblURCSmoothModeSmoothUpdates);
            this.gbURCMode.Controls.Add(this.cboURCSmoothModeSmoothUpdates);
            this.gbURCMode.Controls.Add(this.lblURCSmoothModeThreshold);
            this.gbURCMode.Controls.Add(this.lblURCSmoothModeThresholdHex);
            this.gbURCMode.Controls.Add(this.lblURCSmoothModeThresholdDegrees);
            this.gbURCMode.Controls.Add(this.nudURCSmoothModeThresholdDecimal);
            this.gbURCMode.Controls.Add(this.rdoURCSmoothMode);
            this.gbURCMode.Controls.Add(this.lblURCLimitThreshold);
            this.gbURCMode.Controls.Add(this.lblURCLimitThresholdHex);
            this.gbURCMode.Controls.Add(this.lblURCLimitThresholdDegrees);
            this.gbURCMode.Controls.Add(this.rdoURCLimitMode);
            this.gbURCMode.Controls.Add(this.nudURCLimitThresholdDecimal);
            this.gbURCMode.Location = new System.Drawing.Point(3, 16);
            this.gbURCMode.Margin = new System.Windows.Forms.Padding(1);
            this.gbURCMode.Name = "gbURCMode";
            this.gbURCMode.Padding = new System.Windows.Forms.Padding(1);
            this.gbURCMode.Size = new System.Drawing.Size(346, 77);
            this.gbURCMode.TabIndex = 1;
            this.gbURCMode.TabStop = false;
            this.gbURCMode.Text = "Mode";
            // 
            // lblURCSmoothModeSmoothUpdates
            // 
            this.lblURCSmoothModeSmoothUpdates.AutoSize = true;
            this.lblURCSmoothModeSmoothUpdates.Location = new System.Drawing.Point(35, 55);
            this.lblURCSmoothModeSmoothUpdates.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCSmoothModeSmoothUpdates.Name = "lblURCSmoothModeSmoothUpdates";
            this.lblURCSmoothModeSmoothUpdates.Size = new System.Drawing.Size(89, 13);
            this.lblURCSmoothModeSmoothUpdates.TabIndex = 40;
            this.lblURCSmoothModeSmoothUpdates.Text = "Smooth Updates:";
            // 
            // cboURCSmoothModeSmoothUpdates
            // 
            this.cboURCSmoothModeSmoothUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboURCSmoothModeSmoothUpdates.FormattingEnabled = true;
            this.cboURCSmoothModeSmoothUpdates.Items.AddRange(new object[] {
            "Adaptive",
            "2 steps",
            "4 steps",
            "8 steps"});
            this.cboURCSmoothModeSmoothUpdates.Location = new System.Drawing.Point(131, 53);
            this.cboURCSmoothModeSmoothUpdates.Margin = new System.Windows.Forms.Padding(1);
            this.cboURCSmoothModeSmoothUpdates.Name = "cboURCSmoothModeSmoothUpdates";
            this.cboURCSmoothModeSmoothUpdates.Size = new System.Drawing.Size(77, 21);
            this.cboURCSmoothModeSmoothUpdates.TabIndex = 39;
            this.cboURCSmoothModeSmoothUpdates.SelectedIndexChanged += new System.EventHandler(this.cboSmoothModeSmoothUpdates_SelectedIndexChanged);
            // 
            // lblURCSmoothModeThreshold
            // 
            this.lblURCSmoothModeThreshold.AutoSize = true;
            this.lblURCSmoothModeThreshold.Location = new System.Drawing.Point(67, 36);
            this.lblURCSmoothModeThreshold.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCSmoothModeThreshold.Name = "lblURCSmoothModeThreshold";
            this.lblURCSmoothModeThreshold.Size = new System.Drawing.Size(57, 13);
            this.lblURCSmoothModeThreshold.TabIndex = 35;
            this.lblURCSmoothModeThreshold.Text = "Threshold:";
            // 
            // lblURCSmoothModeThresholdHex
            // 
            this.lblURCSmoothModeThresholdHex.AutoSize = true;
            this.lblURCSmoothModeThresholdHex.Location = new System.Drawing.Point(277, 36);
            this.lblURCSmoothModeThresholdHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCSmoothModeThresholdHex.Name = "lblURCSmoothModeThresholdHex";
            this.lblURCSmoothModeThresholdHex.Size = new System.Drawing.Size(29, 13);
            this.lblURCSmoothModeThresholdHex.TabIndex = 37;
            this.lblURCSmoothModeThresholdHex.Text = "Hex:";
            // 
            // lblURCSmoothModeThresholdDegrees
            // 
            this.lblURCSmoothModeThresholdDegrees.AutoSize = true;
            this.lblURCSmoothModeThresholdDegrees.Location = new System.Drawing.Point(192, 36);
            this.lblURCSmoothModeThresholdDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCSmoothModeThresholdDegrees.Name = "lblURCSmoothModeThresholdDegrees";
            this.lblURCSmoothModeThresholdDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblURCSmoothModeThresholdDegrees.TabIndex = 36;
            this.lblURCSmoothModeThresholdDegrees.Text = "Degrees:";
            // 
            // nudURCSmoothModeThresholdDecimal
            // 
            this.nudURCSmoothModeThresholdDecimal.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudURCSmoothModeThresholdDecimal.Location = new System.Drawing.Point(131, 34);
            this.nudURCSmoothModeThresholdDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudURCSmoothModeThresholdDecimal.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudURCSmoothModeThresholdDecimal.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudURCSmoothModeThresholdDecimal.Name = "nudURCSmoothModeThresholdDecimal";
            this.nudURCSmoothModeThresholdDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudURCSmoothModeThresholdDecimal.TabIndex = 38;
            this.nudURCSmoothModeThresholdDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudURCSmoothModeThresholdDecimal.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudURCSmoothModeThresholdDecimal.ValueChanged += new System.EventHandler(this.nudURCSmoothModeThresholdDecimal_ValueChanged);
            // 
            // rdoURCSmoothMode
            // 
            this.rdoURCSmoothMode.AutoSize = true;
            this.rdoURCSmoothMode.Location = new System.Drawing.Point(5, 34);
            this.rdoURCSmoothMode.Margin = new System.Windows.Forms.Padding(1);
            this.rdoURCSmoothMode.Name = "rdoURCSmoothMode";
            this.rdoURCSmoothMode.Size = new System.Drawing.Size(61, 17);
            this.rdoURCSmoothMode.TabIndex = 34;
            this.rdoURCSmoothMode.TabStop = true;
            this.rdoURCSmoothMode.Text = "Smooth";
            this.rdoURCSmoothMode.UseVisualStyleBackColor = true;
            this.rdoURCSmoothMode.CheckedChanged += new System.EventHandler(this.rdoURCSmoothMode_CheckedChanged);
            // 
            // lblURCLimitThreshold
            // 
            this.lblURCLimitThreshold.AutoSize = true;
            this.lblURCLimitThreshold.Location = new System.Drawing.Point(67, 16);
            this.lblURCLimitThreshold.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCLimitThreshold.Name = "lblURCLimitThreshold";
            this.lblURCLimitThreshold.Size = new System.Drawing.Size(57, 13);
            this.lblURCLimitThreshold.TabIndex = 31;
            this.lblURCLimitThreshold.Text = "Threshold:";
            // 
            // lblURCLimitThresholdHex
            // 
            this.lblURCLimitThresholdHex.AutoSize = true;
            this.lblURCLimitThresholdHex.Location = new System.Drawing.Point(277, 16);
            this.lblURCLimitThresholdHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCLimitThresholdHex.Name = "lblURCLimitThresholdHex";
            this.lblURCLimitThresholdHex.Size = new System.Drawing.Size(29, 13);
            this.lblURCLimitThresholdHex.TabIndex = 32;
            this.lblURCLimitThresholdHex.Text = "Hex:";
            // 
            // lblURCLimitThresholdDegrees
            // 
            this.lblURCLimitThresholdDegrees.AutoSize = true;
            this.lblURCLimitThresholdDegrees.Location = new System.Drawing.Point(192, 16);
            this.lblURCLimitThresholdDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblURCLimitThresholdDegrees.Name = "lblURCLimitThresholdDegrees";
            this.lblURCLimitThresholdDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblURCLimitThresholdDegrees.TabIndex = 31;
            this.lblURCLimitThresholdDegrees.Text = "Degrees:";
            // 
            // rdoURCLimitMode
            // 
            this.rdoURCLimitMode.AutoSize = true;
            this.rdoURCLimitMode.Location = new System.Drawing.Point(5, 16);
            this.rdoURCLimitMode.Margin = new System.Windows.Forms.Padding(1);
            this.rdoURCLimitMode.Name = "rdoURCLimitMode";
            this.rdoURCLimitMode.Size = new System.Drawing.Size(46, 17);
            this.rdoURCLimitMode.TabIndex = 0;
            this.rdoURCLimitMode.TabStop = true;
            this.rdoURCLimitMode.Text = "Limit";
            this.rdoURCLimitMode.UseVisualStyleBackColor = true;
            this.rdoURCLimitMode.CheckedChanged += new System.EventHandler(this.rdoURCLimitMode_CheckedChanged);
            // 
            // nudURCLimitThresholdDecimal
            // 
            this.nudURCLimitThresholdDecimal.Location = new System.Drawing.Point(131, 14);
            this.nudURCLimitThresholdDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudURCLimitThresholdDecimal.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.nudURCLimitThresholdDecimal.Name = "nudURCLimitThresholdDecimal";
            this.nudURCLimitThresholdDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudURCLimitThresholdDecimal.TabIndex = 33;
            this.nudURCLimitThresholdDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudURCLimitThresholdDecimal.ValueChanged += new System.EventHandler(this.nudURCLimitThresholdDecimal_ValueChanged);
            // 
            // tabSynchroControl
            // 
            this.tabSynchroControl.Controls.Add(this.gbSetStatorAmplitudeAndPolarityImmediate);
            this.tabSynchroControl.Controls.Add(this.gbMoveIndicatorCoarseResolution);
            this.tabSynchroControl.Controls.Add(this.gbIndicatorMovementControl);
            this.tabSynchroControl.Location = new System.Drawing.Point(4, 22);
            this.tabSynchroControl.Margin = new System.Windows.Forms.Padding(1);
            this.tabSynchroControl.Name = "tabSynchroControl";
            this.tabSynchroControl.Size = new System.Drawing.Size(520, 452);
            this.tabSynchroControl.TabIndex = 7;
            this.tabSynchroControl.Text = "Synchro Control";
            this.tabSynchroControl.UseVisualStyleBackColor = true;
            // 
            // gbSetStatorAmplitudeAndPolarityImmediate
            // 
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.btnUpdateStatorAmplitudesAndPolarities);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblStatorAmplitudeAndPolarityUpdateMode);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.rdoStatorAmplitudeAndPolarityDeferredUpdates);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.rdoStatorAmplitudeAndPolarityImmediateUpdates);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblStators);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS3AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS2AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS1AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblPolarities);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblAmplitudes);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.chkS3Polarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.chkS2Polarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.chkS1Polarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS3AmplitudeHex);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.nudS3AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS3AmplitudePolarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS2AmplitudeHex);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.nudS2AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS2AmplitudePolarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS1AmplitudeHex);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.nudS1AmplitudeDecimal);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Controls.Add(this.lblS1AmplitudePolarity);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Location = new System.Drawing.Point(7, 95);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Margin = new System.Windows.Forms.Padding(1);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Name = "gbSetStatorAmplitudeAndPolarityImmediate";
            this.gbSetStatorAmplitudeAndPolarityImmediate.Padding = new System.Windows.Forms.Padding(1);
            this.gbSetStatorAmplitudeAndPolarityImmediate.Size = new System.Drawing.Size(399, 177);
            this.gbSetStatorAmplitudeAndPolarityImmediate.TabIndex = 43;
            this.gbSetStatorAmplitudeAndPolarityImmediate.TabStop = false;
            this.gbSetStatorAmplitudeAndPolarityImmediate.Text = "Set Stator Amplitude/Polarity";
            // 
            // btnUpdateStatorAmplitudesAndPolarities
            // 
            this.btnUpdateStatorAmplitudesAndPolarities.Location = new System.Drawing.Point(179, 138);
            this.btnUpdateStatorAmplitudesAndPolarities.Margin = new System.Windows.Forms.Padding(1);
            this.btnUpdateStatorAmplitudesAndPolarities.Name = "btnUpdateStatorAmplitudesAndPolarities";
            this.btnUpdateStatorAmplitudesAndPolarities.Size = new System.Drawing.Size(59, 29);
            this.btnUpdateStatorAmplitudesAndPolarities.TabIndex = 54;
            this.btnUpdateStatorAmplitudesAndPolarities.Text = "&Update";
            this.btnUpdateStatorAmplitudesAndPolarities.UseVisualStyleBackColor = true;
            this.btnUpdateStatorAmplitudesAndPolarities.Click += new System.EventHandler(this.btnUpdateStatorAmplitudesAndPolarities_Click);
            // 
            // lblStatorAmplitudeAndPolarityUpdateMode
            // 
            this.lblStatorAmplitudeAndPolarityUpdateMode.AutoSize = true;
            this.lblStatorAmplitudeAndPolarityUpdateMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatorAmplitudeAndPolarityUpdateMode.Location = new System.Drawing.Point(5, 112);
            this.lblStatorAmplitudeAndPolarityUpdateMode.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStatorAmplitudeAndPolarityUpdateMode.Name = "lblStatorAmplitudeAndPolarityUpdateMode";
            this.lblStatorAmplitudeAndPolarityUpdateMode.Size = new System.Drawing.Size(87, 13);
            this.lblStatorAmplitudeAndPolarityUpdateMode.TabIndex = 53;
            this.lblStatorAmplitudeAndPolarityUpdateMode.Text = "Update Mode:";
            // 
            // rdoStatorAmplitudeAndPolarityDeferredUpdates
            // 
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.AutoSize = true;
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Checked = true;
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Location = new System.Drawing.Point(99, 110);
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Margin = new System.Windows.Forms.Padding(1);
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Name = "rdoStatorAmplitudeAndPolarityDeferredUpdates";
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Size = new System.Drawing.Size(66, 17);
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.TabIndex = 45;
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.TabStop = true;
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.Text = "Deferred";
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.UseVisualStyleBackColor = true;
            this.rdoStatorAmplitudeAndPolarityDeferredUpdates.CheckedChanged += new System.EventHandler(this.rdoStatorAmplitudeAndPolarityDeferredUpdates_CheckedChanged);
            // 
            // rdoStatorAmplitudeAndPolarityImmediateUpdates
            // 
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.AutoSize = true;
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.Location = new System.Drawing.Point(165, 110);
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.Margin = new System.Windows.Forms.Padding(1);
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.Name = "rdoStatorAmplitudeAndPolarityImmediateUpdates";
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.Size = new System.Drawing.Size(73, 17);
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.TabIndex = 44;
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.Text = "Immediate";
            this.rdoStatorAmplitudeAndPolarityImmediateUpdates.UseVisualStyleBackColor = true;
            // 
            // lblStators
            // 
            this.lblStators.AutoSize = true;
            this.lblStators.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStators.Location = new System.Drawing.Point(32, 19);
            this.lblStators.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblStators.Name = "lblStators";
            this.lblStators.Size = new System.Drawing.Size(41, 13);
            this.lblStators.TabIndex = 52;
            this.lblStators.Text = "Stator";
            // 
            // lblS3AmplitudeDecimal
            // 
            this.lblS3AmplitudeDecimal.AutoSize = true;
            this.lblS3AmplitudeDecimal.Location = new System.Drawing.Point(136, 84);
            this.lblS3AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS3AmplitudeDecimal.Name = "lblS3AmplitudeDecimal";
            this.lblS3AmplitudeDecimal.Size = new System.Drawing.Size(52, 13);
            this.lblS3AmplitudeDecimal.TabIndex = 51;
            this.lblS3AmplitudeDecimal.Text = "(decimal):";
            // 
            // lblS2AmplitudeDecimal
            // 
            this.lblS2AmplitudeDecimal.AutoSize = true;
            this.lblS2AmplitudeDecimal.Location = new System.Drawing.Point(136, 63);
            this.lblS2AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS2AmplitudeDecimal.Name = "lblS2AmplitudeDecimal";
            this.lblS2AmplitudeDecimal.Size = new System.Drawing.Size(52, 13);
            this.lblS2AmplitudeDecimal.TabIndex = 50;
            this.lblS2AmplitudeDecimal.Text = "(decimal):";
            // 
            // lblS1AmplitudeDecimal
            // 
            this.lblS1AmplitudeDecimal.AutoSize = true;
            this.lblS1AmplitudeDecimal.Location = new System.Drawing.Point(136, 42);
            this.lblS1AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS1AmplitudeDecimal.Name = "lblS1AmplitudeDecimal";
            this.lblS1AmplitudeDecimal.Size = new System.Drawing.Size(52, 13);
            this.lblS1AmplitudeDecimal.TabIndex = 49;
            this.lblS1AmplitudeDecimal.Text = "(decimal):";
            // 
            // lblPolarities
            // 
            this.lblPolarities.AutoSize = true;
            this.lblPolarities.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPolarities.Location = new System.Drawing.Point(79, 19);
            this.lblPolarities.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblPolarities.Name = "lblPolarities";
            this.lblPolarities.Size = new System.Drawing.Size(49, 13);
            this.lblPolarities.TabIndex = 48;
            this.lblPolarities.Text = "Polarity";
            // 
            // lblAmplitudes
            // 
            this.lblAmplitudes.AutoSize = true;
            this.lblAmplitudes.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmplitudes.Location = new System.Drawing.Point(136, 19);
            this.lblAmplitudes.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblAmplitudes.Name = "lblAmplitudes";
            this.lblAmplitudes.Size = new System.Drawing.Size(62, 13);
            this.lblAmplitudes.TabIndex = 47;
            this.lblAmplitudes.Text = "Amplitude";
            // 
            // chkS3Polarity
            // 
            this.chkS3Polarity.AutoSize = true;
            this.chkS3Polarity.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkS3Polarity.Location = new System.Drawing.Point(95, 82);
            this.chkS3Polarity.Margin = new System.Windows.Forms.Padding(1);
            this.chkS3Polarity.Name = "chkS3Polarity";
            this.chkS3Polarity.Size = new System.Drawing.Size(35, 18);
            this.chkS3Polarity.TabIndex = 46;
            this.chkS3Polarity.Text = " ";
            this.chkS3Polarity.UseVisualStyleBackColor = true;
            this.chkS3Polarity.CheckedChanged += new System.EventHandler(this.chkS3Polarity_CheckedChanged);
            // 
            // chkS2Polarity
            // 
            this.chkS2Polarity.AutoSize = true;
            this.chkS2Polarity.Location = new System.Drawing.Point(95, 62);
            this.chkS2Polarity.Margin = new System.Windows.Forms.Padding(1);
            this.chkS2Polarity.Name = "chkS2Polarity";
            this.chkS2Polarity.Size = new System.Drawing.Size(15, 14);
            this.chkS2Polarity.TabIndex = 45;
            this.chkS2Polarity.UseVisualStyleBackColor = true;
            this.chkS2Polarity.CheckedChanged += new System.EventHandler(this.chkS2Polarity_CheckedChanged);
            // 
            // chkS1Polarity
            // 
            this.chkS1Polarity.AutoSize = true;
            this.chkS1Polarity.Location = new System.Drawing.Point(95, 42);
            this.chkS1Polarity.Margin = new System.Windows.Forms.Padding(1);
            this.chkS1Polarity.Name = "chkS1Polarity";
            this.chkS1Polarity.Size = new System.Drawing.Size(15, 14);
            this.chkS1Polarity.TabIndex = 44;
            this.chkS1Polarity.UseVisualStyleBackColor = true;
            this.chkS1Polarity.CheckedChanged += new System.EventHandler(this.chkS1Polarity_CheckedChanged);
            // 
            // lblS3AmplitudeHex
            // 
            this.lblS3AmplitudeHex.AutoSize = true;
            this.lblS3AmplitudeHex.Location = new System.Drawing.Point(328, 84);
            this.lblS3AmplitudeHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS3AmplitudeHex.Name = "lblS3AmplitudeHex";
            this.lblS3AmplitudeHex.Size = new System.Drawing.Size(29, 13);
            this.lblS3AmplitudeHex.TabIndex = 32;
            this.lblS3AmplitudeHex.Text = "Hex:";
            // 
            // nudS3AmplitudeDecimal
            // 
            this.nudS3AmplitudeDecimal.Location = new System.Drawing.Point(192, 82);
            this.nudS3AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudS3AmplitudeDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudS3AmplitudeDecimal.Name = "nudS3AmplitudeDecimal";
            this.nudS3AmplitudeDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudS3AmplitudeDecimal.TabIndex = 31;
            this.nudS3AmplitudeDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudS3AmplitudeDecimal.ValueChanged += new System.EventHandler(this.nudS3AmplitudeDecimal_ValueChanged);
            // 
            // lblS3AmplitudePolarity
            // 
            this.lblS3AmplitudePolarity.AutoSize = true;
            this.lblS3AmplitudePolarity.Location = new System.Drawing.Point(50, 84);
            this.lblS3AmplitudePolarity.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS3AmplitudePolarity.Name = "lblS3AmplitudePolarity";
            this.lblS3AmplitudePolarity.Size = new System.Drawing.Size(20, 13);
            this.lblS3AmplitudePolarity.TabIndex = 30;
            this.lblS3AmplitudePolarity.Text = "S3";
            // 
            // lblS2AmplitudeHex
            // 
            this.lblS2AmplitudeHex.AutoSize = true;
            this.lblS2AmplitudeHex.Location = new System.Drawing.Point(328, 63);
            this.lblS2AmplitudeHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS2AmplitudeHex.Name = "lblS2AmplitudeHex";
            this.lblS2AmplitudeHex.Size = new System.Drawing.Size(29, 13);
            this.lblS2AmplitudeHex.TabIndex = 29;
            this.lblS2AmplitudeHex.Text = "Hex:";
            // 
            // nudS2AmplitudeDecimal
            // 
            this.nudS2AmplitudeDecimal.Location = new System.Drawing.Point(192, 61);
            this.nudS2AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudS2AmplitudeDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudS2AmplitudeDecimal.Name = "nudS2AmplitudeDecimal";
            this.nudS2AmplitudeDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudS2AmplitudeDecimal.TabIndex = 28;
            this.nudS2AmplitudeDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudS2AmplitudeDecimal.ValueChanged += new System.EventHandler(this.nudS2AmplitudeDecimal_ValueChanged);
            // 
            // lblS2AmplitudePolarity
            // 
            this.lblS2AmplitudePolarity.AutoSize = true;
            this.lblS2AmplitudePolarity.Location = new System.Drawing.Point(51, 63);
            this.lblS2AmplitudePolarity.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS2AmplitudePolarity.Name = "lblS2AmplitudePolarity";
            this.lblS2AmplitudePolarity.Size = new System.Drawing.Size(20, 13);
            this.lblS2AmplitudePolarity.TabIndex = 27;
            this.lblS2AmplitudePolarity.Text = "S2";
            // 
            // lblS1AmplitudeHex
            // 
            this.lblS1AmplitudeHex.AutoSize = true;
            this.lblS1AmplitudeHex.Location = new System.Drawing.Point(328, 42);
            this.lblS1AmplitudeHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS1AmplitudeHex.Name = "lblS1AmplitudeHex";
            this.lblS1AmplitudeHex.Size = new System.Drawing.Size(29, 13);
            this.lblS1AmplitudeHex.TabIndex = 26;
            this.lblS1AmplitudeHex.Text = "Hex:";
            // 
            // nudS1AmplitudeDecimal
            // 
            this.nudS1AmplitudeDecimal.Location = new System.Drawing.Point(192, 40);
            this.nudS1AmplitudeDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudS1AmplitudeDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudS1AmplitudeDecimal.Name = "nudS1AmplitudeDecimal";
            this.nudS1AmplitudeDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudS1AmplitudeDecimal.TabIndex = 24;
            this.nudS1AmplitudeDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudS1AmplitudeDecimal.ValueChanged += new System.EventHandler(this.nudS1AmplitudeDecimal_ValueChanged);
            // 
            // lblS1AmplitudePolarity
            // 
            this.lblS1AmplitudePolarity.AutoSize = true;
            this.lblS1AmplitudePolarity.Location = new System.Drawing.Point(51, 42);
            this.lblS1AmplitudePolarity.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblS1AmplitudePolarity.Name = "lblS1AmplitudePolarity";
            this.lblS1AmplitudePolarity.Size = new System.Drawing.Size(20, 13);
            this.lblS1AmplitudePolarity.TabIndex = 23;
            this.lblS1AmplitudePolarity.Text = "S1";
            // 
            // gbMoveIndicatorCoarseResolution
            // 
            this.gbMoveIndicatorCoarseResolution.Controls.Add(this.lblMoveIndicatorCoarseResolutionHex);
            this.gbMoveIndicatorCoarseResolution.Controls.Add(this.lblMoveIndicatorCoarseResolutionDegrees);
            this.gbMoveIndicatorCoarseResolution.Controls.Add(this.nudMoveIndicatorCoarseResolutionDecimal);
            this.gbMoveIndicatorCoarseResolution.Controls.Add(this.lblMoveIndicatorCoarseResolutionPositionDecimal);
            this.gbMoveIndicatorCoarseResolution.Location = new System.Drawing.Point(7, 51);
            this.gbMoveIndicatorCoarseResolution.Margin = new System.Windows.Forms.Padding(1);
            this.gbMoveIndicatorCoarseResolution.Name = "gbMoveIndicatorCoarseResolution";
            this.gbMoveIndicatorCoarseResolution.Padding = new System.Windows.Forms.Padding(1);
            this.gbMoveIndicatorCoarseResolution.Size = new System.Drawing.Size(399, 41);
            this.gbMoveIndicatorCoarseResolution.TabIndex = 42;
            this.gbMoveIndicatorCoarseResolution.TabStop = false;
            this.gbMoveIndicatorCoarseResolution.Text = "Move Indicator (Coarse Resolution)";
            // 
            // lblMoveIndicatorCoarseResolutionHex
            // 
            this.lblMoveIndicatorCoarseResolutionHex.AutoSize = true;
            this.lblMoveIndicatorCoarseResolutionHex.Location = new System.Drawing.Point(327, 19);
            this.lblMoveIndicatorCoarseResolutionHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorCoarseResolutionHex.Name = "lblMoveIndicatorCoarseResolutionHex";
            this.lblMoveIndicatorCoarseResolutionHex.Size = new System.Drawing.Size(29, 13);
            this.lblMoveIndicatorCoarseResolutionHex.TabIndex = 26;
            this.lblMoveIndicatorCoarseResolutionHex.Text = "Hex:";
            // 
            // lblMoveIndicatorCoarseResolutionDegrees
            // 
            this.lblMoveIndicatorCoarseResolutionDegrees.AutoSize = true;
            this.lblMoveIndicatorCoarseResolutionDegrees.Location = new System.Drawing.Point(243, 19);
            this.lblMoveIndicatorCoarseResolutionDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorCoarseResolutionDegrees.Name = "lblMoveIndicatorCoarseResolutionDegrees";
            this.lblMoveIndicatorCoarseResolutionDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblMoveIndicatorCoarseResolutionDegrees.TabIndex = 25;
            this.lblMoveIndicatorCoarseResolutionDegrees.Text = "Degrees:";
            // 
            // nudMoveIndicatorCoarseResolutionDecimal
            // 
            this.nudMoveIndicatorCoarseResolutionDecimal.Location = new System.Drawing.Point(191, 17);
            this.nudMoveIndicatorCoarseResolutionDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudMoveIndicatorCoarseResolutionDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudMoveIndicatorCoarseResolutionDecimal.Name = "nudMoveIndicatorCoarseResolutionDecimal";
            this.nudMoveIndicatorCoarseResolutionDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudMoveIndicatorCoarseResolutionDecimal.TabIndex = 24;
            this.nudMoveIndicatorCoarseResolutionDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudMoveIndicatorCoarseResolutionDecimal.ValueChanged += new System.EventHandler(this.nudMoveIndicatorCoarseResolutionDecimal_ValueChanged);
            // 
            // lblMoveIndicatorCoarseResolutionPositionDecimal
            // 
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.AutoSize = true;
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.Location = new System.Drawing.Point(95, 19);
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.Name = "lblMoveIndicatorCoarseResolutionPositionDecimal";
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.Size = new System.Drawing.Size(92, 13);
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.TabIndex = 23;
            this.lblMoveIndicatorCoarseResolutionPositionDecimal.Text = "Position (decimal):";
            // 
            // gbIndicatorMovementControl
            // 
            this.gbIndicatorMovementControl.Controls.Add(this.lblMoveIndicatorInQuadrant);
            this.gbIndicatorMovementControl.Controls.Add(this.cboMoveIndicatorInQuadrant);
            this.gbIndicatorMovementControl.Controls.Add(this.lblMoveIndicatorToPositionHex);
            this.gbIndicatorMovementControl.Controls.Add(this.lblMoveIndicatorToPositionDegrees);
            this.gbIndicatorMovementControl.Controls.Add(this.nudMoveIndicatorToPositionDecimal);
            this.gbIndicatorMovementControl.Controls.Add(this.lblMoveIndicatorToPosition);
            this.gbIndicatorMovementControl.Location = new System.Drawing.Point(7, 6);
            this.gbIndicatorMovementControl.Margin = new System.Windows.Forms.Padding(1);
            this.gbIndicatorMovementControl.Name = "gbIndicatorMovementControl";
            this.gbIndicatorMovementControl.Padding = new System.Windows.Forms.Padding(1);
            this.gbIndicatorMovementControl.Size = new System.Drawing.Size(399, 41);
            this.gbIndicatorMovementControl.TabIndex = 0;
            this.gbIndicatorMovementControl.TabStop = false;
            this.gbIndicatorMovementControl.Text = "Move Indicator Within Quadrants";
            // 
            // lblMoveIndicatorInQuadrant
            // 
            this.lblMoveIndicatorInQuadrant.AutoSize = true;
            this.lblMoveIndicatorInQuadrant.Location = new System.Drawing.Point(5, 19);
            this.lblMoveIndicatorInQuadrant.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorInQuadrant.Name = "lblMoveIndicatorInQuadrant";
            this.lblMoveIndicatorInQuadrant.Size = new System.Drawing.Size(54, 13);
            this.lblMoveIndicatorInQuadrant.TabIndex = 41;
            this.lblMoveIndicatorInQuadrant.Text = "Quadrant:";
            // 
            // cboMoveIndicatorInQuadrant
            // 
            this.cboMoveIndicatorInQuadrant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoveIndicatorInQuadrant.FormattingEnabled = true;
            this.cboMoveIndicatorInQuadrant.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cboMoveIndicatorInQuadrant.Location = new System.Drawing.Point(63, 17);
            this.cboMoveIndicatorInQuadrant.Margin = new System.Windows.Forms.Padding(1);
            this.cboMoveIndicatorInQuadrant.Name = "cboMoveIndicatorInQuadrant";
            this.cboMoveIndicatorInQuadrant.Size = new System.Drawing.Size(29, 21);
            this.cboMoveIndicatorInQuadrant.TabIndex = 40;
            this.cboMoveIndicatorInQuadrant.SelectedIndexChanged += new System.EventHandler(this.cboMoveIndicatorInQuadrant_SelectedIndexChanged);
            // 
            // lblMoveIndicatorToPositionHex
            // 
            this.lblMoveIndicatorToPositionHex.AutoSize = true;
            this.lblMoveIndicatorToPositionHex.Location = new System.Drawing.Point(327, 19);
            this.lblMoveIndicatorToPositionHex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorToPositionHex.Name = "lblMoveIndicatorToPositionHex";
            this.lblMoveIndicatorToPositionHex.Size = new System.Drawing.Size(29, 13);
            this.lblMoveIndicatorToPositionHex.TabIndex = 26;
            this.lblMoveIndicatorToPositionHex.Text = "Hex:";
            // 
            // lblMoveIndicatorToPositionDegrees
            // 
            this.lblMoveIndicatorToPositionDegrees.AutoSize = true;
            this.lblMoveIndicatorToPositionDegrees.Location = new System.Drawing.Point(243, 19);
            this.lblMoveIndicatorToPositionDegrees.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorToPositionDegrees.Name = "lblMoveIndicatorToPositionDegrees";
            this.lblMoveIndicatorToPositionDegrees.Size = new System.Drawing.Size(50, 13);
            this.lblMoveIndicatorToPositionDegrees.TabIndex = 25;
            this.lblMoveIndicatorToPositionDegrees.Text = "Degrees:";
            // 
            // nudMoveIndicatorToPositionDecimal
            // 
            this.nudMoveIndicatorToPositionDecimal.Location = new System.Drawing.Point(191, 17);
            this.nudMoveIndicatorToPositionDecimal.Margin = new System.Windows.Forms.Padding(1);
            this.nudMoveIndicatorToPositionDecimal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudMoveIndicatorToPositionDecimal.Name = "nudMoveIndicatorToPositionDecimal";
            this.nudMoveIndicatorToPositionDecimal.Size = new System.Drawing.Size(47, 20);
            this.nudMoveIndicatorToPositionDecimal.TabIndex = 24;
            this.nudMoveIndicatorToPositionDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudMoveIndicatorToPositionDecimal.ValueChanged += new System.EventHandler(this.nudMoveIndicatorToPosition_ValueChanged);
            // 
            // lblMoveIndicatorToPosition
            // 
            this.lblMoveIndicatorToPosition.AutoSize = true;
            this.lblMoveIndicatorToPosition.Location = new System.Drawing.Point(95, 19);
            this.lblMoveIndicatorToPosition.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMoveIndicatorToPosition.Name = "lblMoveIndicatorToPosition";
            this.lblMoveIndicatorToPosition.Size = new System.Drawing.Size(92, 13);
            this.lblMoveIndicatorToPosition.TabIndex = 23;
            this.lblMoveIndicatorToPosition.Text = "Position (decimal):";
            // 
            // tabDemoMode
            // 
            this.tabDemoMode.Controls.Add(this.gbDemo);
            this.tabDemoMode.Location = new System.Drawing.Point(4, 22);
            this.tabDemoMode.Margin = new System.Windows.Forms.Padding(1);
            this.tabDemoMode.Name = "tabDemoMode";
            this.tabDemoMode.Size = new System.Drawing.Size(520, 452);
            this.tabDemoMode.TabIndex = 6;
            this.tabDemoMode.Text = "Demo Mode";
            this.tabDemoMode.UseVisualStyleBackColor = true;
            // 
            // tabDigitalAndPWMOutputs
            // 
            this.tabDigitalAndPWMOutputs.Controls.Add(this.gbDigitalAndPWMOutputs);
            this.tabDigitalAndPWMOutputs.Location = new System.Drawing.Point(4, 22);
            this.tabDigitalAndPWMOutputs.Margin = new System.Windows.Forms.Padding(1);
            this.tabDigitalAndPWMOutputs.Name = "tabDigitalAndPWMOutputs";
            this.tabDigitalAndPWMOutputs.Padding = new System.Windows.Forms.Padding(1);
            this.tabDigitalAndPWMOutputs.Size = new System.Drawing.Size(520, 452);
            this.tabDigitalAndPWMOutputs.TabIndex = 8;
            this.tabDigitalAndPWMOutputs.Text = "Digital and PWM Outputs";
            this.tabDigitalAndPWMOutputs.UseVisualStyleBackColor = true;
            // 
            // gbDigitalAndPWMOutputs
            // 
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblPWM_OUT_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudPWM_OUT_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboPWM_OUT_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboPWM_OUT_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblPWM_OUT);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_7_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_7_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_7_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_7_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_7);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_6_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_6_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_6_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_6_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_6);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_5_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_5_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_5_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_5_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_5);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_4_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_4_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_4_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_4_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_4);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_3_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_3_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_3_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_3_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_3);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_2_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_2_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_2_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_2_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_2);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_1_Hex);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.nudDIG_PWM_1_DutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDutyCycle);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_1_Value);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDigitalAndPWMChannelValues);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.cboDIG_PWM_1_Mode);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDigitalOrPWM);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblChannels);
            this.gbDigitalAndPWMOutputs.Controls.Add(this.lblDIG_PWM_1);
            this.gbDigitalAndPWMOutputs.Location = new System.Drawing.Point(3, 3);
            this.gbDigitalAndPWMOutputs.Margin = new System.Windows.Forms.Padding(1);
            this.gbDigitalAndPWMOutputs.Name = "gbDigitalAndPWMOutputs";
            this.gbDigitalAndPWMOutputs.Padding = new System.Windows.Forms.Padding(1);
            this.gbDigitalAndPWMOutputs.Size = new System.Drawing.Size(403, 237);
            this.gbDigitalAndPWMOutputs.TabIndex = 0;
            this.gbDigitalAndPWMOutputs.TabStop = false;
            this.gbDigitalAndPWMOutputs.Text = "Digital and PWM Outputs";
            // 
            // lblPWM_OUT_Hex
            // 
            this.lblPWM_OUT_Hex.AutoSize = true;
            this.lblPWM_OUT_Hex.Location = new System.Drawing.Point(323, 214);
            this.lblPWM_OUT_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblPWM_OUT_Hex.Name = "lblPWM_OUT_Hex";
            this.lblPWM_OUT_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblPWM_OUT_Hex.TabIndex = 80;
            this.lblPWM_OUT_Hex.Text = "Hex:";
            // 
            // nudPWM_OUT_DutyCycle
            // 
            this.nudPWM_OUT_DutyCycle.Location = new System.Drawing.Point(232, 213);
            this.nudPWM_OUT_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudPWM_OUT_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudPWM_OUT_DutyCycle.Name = "nudPWM_OUT_DutyCycle";
            this.nudPWM_OUT_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudPWM_OUT_DutyCycle.TabIndex = 79;
            this.nudPWM_OUT_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPWM_OUT_DutyCycle.ValueChanged += new System.EventHandler(this.nudPWM_OUT_DutyCycle_ValueChanged);
            // 
            // cboPWM_OUT_Value
            // 
            this.cboPWM_OUT_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPWM_OUT_Value.Enabled = false;
            this.cboPWM_OUT_Value.FormattingEnabled = true;
            this.cboPWM_OUT_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboPWM_OUT_Value.Location = new System.Drawing.Point(163, 212);
            this.cboPWM_OUT_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboPWM_OUT_Value.Name = "cboPWM_OUT_Value";
            this.cboPWM_OUT_Value.Size = new System.Drawing.Size(58, 21);
            this.cboPWM_OUT_Value.TabIndex = 78;
            this.cboPWM_OUT_Value.Visible = false;
            this.cboPWM_OUT_Value.SelectedIndexChanged += new System.EventHandler(this.cboPWM_OUT_Value_SelectedIndexChanged);
            // 
            // cboPWM_OUT_Mode
            // 
            this.cboPWM_OUT_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPWM_OUT_Mode.Enabled = false;
            this.cboPWM_OUT_Mode.FormattingEnabled = true;
            this.cboPWM_OUT_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboPWM_OUT_Mode.Location = new System.Drawing.Point(95, 212);
            this.cboPWM_OUT_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboPWM_OUT_Mode.Name = "cboPWM_OUT_Mode";
            this.cboPWM_OUT_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboPWM_OUT_Mode.TabIndex = 77;
            this.cboPWM_OUT_Mode.SelectedIndexChanged += new System.EventHandler(this.cboPWM_OUT_Mode_SelectedIndexChanged);
            // 
            // lblPWM_OUT
            // 
            this.lblPWM_OUT.AutoSize = true;
            this.lblPWM_OUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPWM_OUT.Location = new System.Drawing.Point(3, 214);
            this.lblPWM_OUT.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblPWM_OUT.Name = "lblPWM_OUT";
            this.lblPWM_OUT.Size = new System.Drawing.Size(70, 13);
            this.lblPWM_OUT.TabIndex = 76;
            this.lblPWM_OUT.Text = "PWM_OUT";
            // 
            // lblDIG_PWM_7_Hex
            // 
            this.lblDIG_PWM_7_Hex.AutoSize = true;
            this.lblDIG_PWM_7_Hex.Location = new System.Drawing.Point(323, 191);
            this.lblDIG_PWM_7_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_7_Hex.Name = "lblDIG_PWM_7_Hex";
            this.lblDIG_PWM_7_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_7_Hex.TabIndex = 75;
            this.lblDIG_PWM_7_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_7_DutyCycle
            // 
            this.nudDIG_PWM_7_DutyCycle.Location = new System.Drawing.Point(232, 189);
            this.nudDIG_PWM_7_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_7_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_7_DutyCycle.Name = "nudDIG_PWM_7_DutyCycle";
            this.nudDIG_PWM_7_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_7_DutyCycle.TabIndex = 74;
            this.nudDIG_PWM_7_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_7_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_7_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_7_Value
            // 
            this.cboDIG_PWM_7_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_7_Value.FormattingEnabled = true;
            this.cboDIG_PWM_7_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_7_Value.Location = new System.Drawing.Point(163, 188);
            this.cboDIG_PWM_7_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_7_Value.Name = "cboDIG_PWM_7_Value";
            this.cboDIG_PWM_7_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_7_Value.TabIndex = 73;
            this.cboDIG_PWM_7_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_7_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_7_Mode
            // 
            this.cboDIG_PWM_7_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_7_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_7_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_7_Mode.Location = new System.Drawing.Point(95, 188);
            this.cboDIG_PWM_7_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_7_Mode.Name = "cboDIG_PWM_7_Mode";
            this.cboDIG_PWM_7_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_7_Mode.TabIndex = 72;
            this.cboDIG_PWM_7_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_7_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_7
            // 
            this.lblDIG_PWM_7.AutoSize = true;
            this.lblDIG_PWM_7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_7.Location = new System.Drawing.Point(3, 191);
            this.lblDIG_PWM_7.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_7.Name = "lblDIG_PWM_7";
            this.lblDIG_PWM_7.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_7.TabIndex = 71;
            this.lblDIG_PWM_7.Text = "DIG_PWM_7";
            // 
            // lblDIG_PWM_6_Hex
            // 
            this.lblDIG_PWM_6_Hex.AutoSize = true;
            this.lblDIG_PWM_6_Hex.Location = new System.Drawing.Point(323, 168);
            this.lblDIG_PWM_6_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_6_Hex.Name = "lblDIG_PWM_6_Hex";
            this.lblDIG_PWM_6_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_6_Hex.TabIndex = 70;
            this.lblDIG_PWM_6_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_6_DutyCycle
            // 
            this.nudDIG_PWM_6_DutyCycle.Location = new System.Drawing.Point(232, 166);
            this.nudDIG_PWM_6_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_6_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_6_DutyCycle.Name = "nudDIG_PWM_6_DutyCycle";
            this.nudDIG_PWM_6_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_6_DutyCycle.TabIndex = 69;
            this.nudDIG_PWM_6_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_6_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_6_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_6_Value
            // 
            this.cboDIG_PWM_6_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_6_Value.FormattingEnabled = true;
            this.cboDIG_PWM_6_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_6_Value.Location = new System.Drawing.Point(163, 165);
            this.cboDIG_PWM_6_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_6_Value.Name = "cboDIG_PWM_6_Value";
            this.cboDIG_PWM_6_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_6_Value.TabIndex = 68;
            this.cboDIG_PWM_6_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_6_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_6_Mode
            // 
            this.cboDIG_PWM_6_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_6_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_6_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_6_Mode.Location = new System.Drawing.Point(95, 165);
            this.cboDIG_PWM_6_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_6_Mode.Name = "cboDIG_PWM_6_Mode";
            this.cboDIG_PWM_6_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_6_Mode.TabIndex = 67;
            this.cboDIG_PWM_6_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_6_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_6
            // 
            this.lblDIG_PWM_6.AutoSize = true;
            this.lblDIG_PWM_6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_6.Location = new System.Drawing.Point(3, 168);
            this.lblDIG_PWM_6.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_6.Name = "lblDIG_PWM_6";
            this.lblDIG_PWM_6.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_6.TabIndex = 66;
            this.lblDIG_PWM_6.Text = "DIG_PWM_6";
            // 
            // lblDIG_PWM_5_Hex
            // 
            this.lblDIG_PWM_5_Hex.AutoSize = true;
            this.lblDIG_PWM_5_Hex.Location = new System.Drawing.Point(323, 144);
            this.lblDIG_PWM_5_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_5_Hex.Name = "lblDIG_PWM_5_Hex";
            this.lblDIG_PWM_5_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_5_Hex.TabIndex = 65;
            this.lblDIG_PWM_5_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_5_DutyCycle
            // 
            this.nudDIG_PWM_5_DutyCycle.Location = new System.Drawing.Point(232, 142);
            this.nudDIG_PWM_5_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_5_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_5_DutyCycle.Name = "nudDIG_PWM_5_DutyCycle";
            this.nudDIG_PWM_5_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_5_DutyCycle.TabIndex = 64;
            this.nudDIG_PWM_5_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_5_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_5_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_5_Value
            // 
            this.cboDIG_PWM_5_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_5_Value.FormattingEnabled = true;
            this.cboDIG_PWM_5_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_5_Value.Location = new System.Drawing.Point(163, 142);
            this.cboDIG_PWM_5_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_5_Value.Name = "cboDIG_PWM_5_Value";
            this.cboDIG_PWM_5_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_5_Value.TabIndex = 63;
            this.cboDIG_PWM_5_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_5_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_5_Mode
            // 
            this.cboDIG_PWM_5_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_5_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_5_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_5_Mode.Location = new System.Drawing.Point(95, 142);
            this.cboDIG_PWM_5_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_5_Mode.Name = "cboDIG_PWM_5_Mode";
            this.cboDIG_PWM_5_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_5_Mode.TabIndex = 62;
            this.cboDIG_PWM_5_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_5_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_5
            // 
            this.lblDIG_PWM_5.AutoSize = true;
            this.lblDIG_PWM_5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_5.Location = new System.Drawing.Point(3, 144);
            this.lblDIG_PWM_5.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_5.Name = "lblDIG_PWM_5";
            this.lblDIG_PWM_5.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_5.TabIndex = 61;
            this.lblDIG_PWM_5.Text = "DIG_PWM_5";
            // 
            // lblDIG_PWM_4_Hex
            // 
            this.lblDIG_PWM_4_Hex.AutoSize = true;
            this.lblDIG_PWM_4_Hex.Location = new System.Drawing.Point(323, 121);
            this.lblDIG_PWM_4_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_4_Hex.Name = "lblDIG_PWM_4_Hex";
            this.lblDIG_PWM_4_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_4_Hex.TabIndex = 60;
            this.lblDIG_PWM_4_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_4_DutyCycle
            // 
            this.nudDIG_PWM_4_DutyCycle.Location = new System.Drawing.Point(232, 119);
            this.nudDIG_PWM_4_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_4_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_4_DutyCycle.Name = "nudDIG_PWM_4_DutyCycle";
            this.nudDIG_PWM_4_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_4_DutyCycle.TabIndex = 59;
            this.nudDIG_PWM_4_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_4_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_4_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_4_Value
            // 
            this.cboDIG_PWM_4_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_4_Value.FormattingEnabled = true;
            this.cboDIG_PWM_4_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_4_Value.Location = new System.Drawing.Point(163, 118);
            this.cboDIG_PWM_4_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_4_Value.Name = "cboDIG_PWM_4_Value";
            this.cboDIG_PWM_4_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_4_Value.TabIndex = 58;
            this.cboDIG_PWM_4_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_4_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_4_Mode
            // 
            this.cboDIG_PWM_4_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_4_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_4_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_4_Mode.Location = new System.Drawing.Point(95, 118);
            this.cboDIG_PWM_4_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_4_Mode.Name = "cboDIG_PWM_4_Mode";
            this.cboDIG_PWM_4_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_4_Mode.TabIndex = 57;
            this.cboDIG_PWM_4_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_4_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_4
            // 
            this.lblDIG_PWM_4.AutoSize = true;
            this.lblDIG_PWM_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_4.Location = new System.Drawing.Point(3, 121);
            this.lblDIG_PWM_4.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_4.Name = "lblDIG_PWM_4";
            this.lblDIG_PWM_4.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_4.TabIndex = 56;
            this.lblDIG_PWM_4.Text = "DIG_PWM_4";
            // 
            // lblDIG_PWM_3_Hex
            // 
            this.lblDIG_PWM_3_Hex.AutoSize = true;
            this.lblDIG_PWM_3_Hex.Location = new System.Drawing.Point(323, 97);
            this.lblDIG_PWM_3_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_3_Hex.Name = "lblDIG_PWM_3_Hex";
            this.lblDIG_PWM_3_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_3_Hex.TabIndex = 55;
            this.lblDIG_PWM_3_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_3_DutyCycle
            // 
            this.nudDIG_PWM_3_DutyCycle.Location = new System.Drawing.Point(232, 96);
            this.nudDIG_PWM_3_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_3_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_3_DutyCycle.Name = "nudDIG_PWM_3_DutyCycle";
            this.nudDIG_PWM_3_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_3_DutyCycle.TabIndex = 54;
            this.nudDIG_PWM_3_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_3_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_3_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_3_Value
            // 
            this.cboDIG_PWM_3_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_3_Value.FormattingEnabled = true;
            this.cboDIG_PWM_3_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_3_Value.Location = new System.Drawing.Point(163, 95);
            this.cboDIG_PWM_3_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_3_Value.Name = "cboDIG_PWM_3_Value";
            this.cboDIG_PWM_3_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_3_Value.TabIndex = 53;
            this.cboDIG_PWM_3_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_3_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_3_Mode
            // 
            this.cboDIG_PWM_3_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_3_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_3_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_3_Mode.Location = new System.Drawing.Point(95, 95);
            this.cboDIG_PWM_3_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_3_Mode.Name = "cboDIG_PWM_3_Mode";
            this.cboDIG_PWM_3_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_3_Mode.TabIndex = 52;
            this.cboDIG_PWM_3_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_3_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_3
            // 
            this.lblDIG_PWM_3.AutoSize = true;
            this.lblDIG_PWM_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_3.Location = new System.Drawing.Point(3, 97);
            this.lblDIG_PWM_3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_3.Name = "lblDIG_PWM_3";
            this.lblDIG_PWM_3.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_3.TabIndex = 51;
            this.lblDIG_PWM_3.Text = "DIG_PWM_3";
            // 
            // lblDIG_PWM_2_Hex
            // 
            this.lblDIG_PWM_2_Hex.AutoSize = true;
            this.lblDIG_PWM_2_Hex.Location = new System.Drawing.Point(323, 74);
            this.lblDIG_PWM_2_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_2_Hex.Name = "lblDIG_PWM_2_Hex";
            this.lblDIG_PWM_2_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_2_Hex.TabIndex = 50;
            this.lblDIG_PWM_2_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_2_DutyCycle
            // 
            this.nudDIG_PWM_2_DutyCycle.Location = new System.Drawing.Point(232, 72);
            this.nudDIG_PWM_2_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_2_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_2_DutyCycle.Name = "nudDIG_PWM_2_DutyCycle";
            this.nudDIG_PWM_2_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_2_DutyCycle.TabIndex = 49;
            this.nudDIG_PWM_2_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_2_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_2_DutyCycle_ValueChanged);
            // 
            // cboDIG_PWM_2_Value
            // 
            this.cboDIG_PWM_2_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_2_Value.FormattingEnabled = true;
            this.cboDIG_PWM_2_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_2_Value.Location = new System.Drawing.Point(163, 71);
            this.cboDIG_PWM_2_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_2_Value.Name = "cboDIG_PWM_2_Value";
            this.cboDIG_PWM_2_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_2_Value.TabIndex = 48;
            this.cboDIG_PWM_2_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_2_Value_SelectedIndexChanged);
            // 
            // cboDIG_PWM_2_Mode
            // 
            this.cboDIG_PWM_2_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_2_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_2_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_2_Mode.Location = new System.Drawing.Point(95, 71);
            this.cboDIG_PWM_2_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_2_Mode.Name = "cboDIG_PWM_2_Mode";
            this.cboDIG_PWM_2_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_2_Mode.TabIndex = 47;
            this.cboDIG_PWM_2_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_2_Mode_SelectedIndexChanged);
            // 
            // lblDIG_PWM_2
            // 
            this.lblDIG_PWM_2.AutoSize = true;
            this.lblDIG_PWM_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_2.Location = new System.Drawing.Point(3, 74);
            this.lblDIG_PWM_2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_2.Name = "lblDIG_PWM_2";
            this.lblDIG_PWM_2.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_2.TabIndex = 46;
            this.lblDIG_PWM_2.Text = "DIG_PWM_2";
            // 
            // lblDIG_PWM_1_Hex
            // 
            this.lblDIG_PWM_1_Hex.AutoSize = true;
            this.lblDIG_PWM_1_Hex.Location = new System.Drawing.Point(323, 51);
            this.lblDIG_PWM_1_Hex.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_1_Hex.Name = "lblDIG_PWM_1_Hex";
            this.lblDIG_PWM_1_Hex.Size = new System.Drawing.Size(29, 13);
            this.lblDIG_PWM_1_Hex.TabIndex = 45;
            this.lblDIG_PWM_1_Hex.Text = "Hex:";
            // 
            // nudDIG_PWM_1_DutyCycle
            // 
            this.nudDIG_PWM_1_DutyCycle.Location = new System.Drawing.Point(232, 49);
            this.nudDIG_PWM_1_DutyCycle.Margin = new System.Windows.Forms.Padding(1);
            this.nudDIG_PWM_1_DutyCycle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDIG_PWM_1_DutyCycle.Name = "nudDIG_PWM_1_DutyCycle";
            this.nudDIG_PWM_1_DutyCycle.Size = new System.Drawing.Size(61, 20);
            this.nudDIG_PWM_1_DutyCycle.TabIndex = 44;
            this.nudDIG_PWM_1_DutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDIG_PWM_1_DutyCycle.ValueChanged += new System.EventHandler(this.nudDIG_PWM_1_DutyCycle_ValueChanged);
            // 
            // lblDutyCycle
            // 
            this.lblDutyCycle.AutoSize = true;
            this.lblDutyCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDutyCycle.Location = new System.Drawing.Point(229, 19);
            this.lblDutyCycle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDutyCycle.Name = "lblDutyCycle";
            this.lblDutyCycle.Size = new System.Drawing.Size(68, 13);
            this.lblDutyCycle.TabIndex = 43;
            this.lblDutyCycle.Text = "Duty Cycle";
            // 
            // cboDIG_PWM_1_Value
            // 
            this.cboDIG_PWM_1_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_1_Value.FormattingEnabled = true;
            this.cboDIG_PWM_1_Value.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cboDIG_PWM_1_Value.Location = new System.Drawing.Point(163, 48);
            this.cboDIG_PWM_1_Value.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_1_Value.Name = "cboDIG_PWM_1_Value";
            this.cboDIG_PWM_1_Value.Size = new System.Drawing.Size(58, 21);
            this.cboDIG_PWM_1_Value.TabIndex = 42;
            this.cboDIG_PWM_1_Value.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_1_Value_SelectedIndexChanged);
            // 
            // lblDigitalAndPWMChannelValues
            // 
            this.lblDigitalAndPWMChannelValues.AutoSize = true;
            this.lblDigitalAndPWMChannelValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitalAndPWMChannelValues.Location = new System.Drawing.Point(160, 19);
            this.lblDigitalAndPWMChannelValues.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDigitalAndPWMChannelValues.Name = "lblDigitalAndPWMChannelValues";
            this.lblDigitalAndPWMChannelValues.Size = new System.Drawing.Size(39, 13);
            this.lblDigitalAndPWMChannelValues.TabIndex = 41;
            this.lblDigitalAndPWMChannelValues.Text = "Value";
            // 
            // cboDIG_PWM_1_Mode
            // 
            this.cboDIG_PWM_1_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDIG_PWM_1_Mode.FormattingEnabled = true;
            this.cboDIG_PWM_1_Mode.Items.AddRange(new object[] {
            "Digital",
            "PWM"});
            this.cboDIG_PWM_1_Mode.Location = new System.Drawing.Point(95, 48);
            this.cboDIG_PWM_1_Mode.Margin = new System.Windows.Forms.Padding(1);
            this.cboDIG_PWM_1_Mode.Name = "cboDIG_PWM_1_Mode";
            this.cboDIG_PWM_1_Mode.Size = new System.Drawing.Size(59, 21);
            this.cboDIG_PWM_1_Mode.TabIndex = 40;
            this.cboDIG_PWM_1_Mode.SelectedIndexChanged += new System.EventHandler(this.cboDIG_PWM_1_Mode_SelectedIndexChanged);
            // 
            // lblDigitalOrPWM
            // 
            this.lblDigitalOrPWM.AutoSize = true;
            this.lblDigitalOrPWM.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitalOrPWM.Location = new System.Drawing.Point(92, 19);
            this.lblDigitalOrPWM.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDigitalOrPWM.Name = "lblDigitalOrPWM";
            this.lblDigitalOrPWM.Size = new System.Drawing.Size(38, 13);
            this.lblDigitalOrPWM.TabIndex = 2;
            this.lblDigitalOrPWM.Text = "Mode";
            // 
            // lblChannels
            // 
            this.lblChannels.AutoSize = true;
            this.lblChannels.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChannels.Location = new System.Drawing.Point(3, 19);
            this.lblChannels.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblChannels.Name = "lblChannels";
            this.lblChannels.Size = new System.Drawing.Size(53, 13);
            this.lblChannels.TabIndex = 1;
            this.lblChannels.Text = "Channel";
            // 
            // lblDIG_PWM_1
            // 
            this.lblDIG_PWM_1.AutoSize = true;
            this.lblDIG_PWM_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDIG_PWM_1.Location = new System.Drawing.Point(3, 51);
            this.lblDIG_PWM_1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblDIG_PWM_1.Name = "lblDIG_PWM_1";
            this.lblDIG_PWM_1.Size = new System.Drawing.Size(80, 13);
            this.lblDIG_PWM_1.TabIndex = 0;
            this.lblDIG_PWM_1.Text = "DIG_PWM_1";
            // 
            // tabRawData
            // 
            this.tabRawData.Controls.Add(this.gbUSBDebug);
            this.tabRawData.Controls.Add(this.gbRawDataControl);
            this.tabRawData.Controls.Add(this.gbWatchdog);
            this.tabRawData.Location = new System.Drawing.Point(4, 22);
            this.tabRawData.Margin = new System.Windows.Forms.Padding(1);
            this.tabRawData.Name = "tabRawData";
            this.tabRawData.Size = new System.Drawing.Size(520, 452);
            this.tabRawData.TabIndex = 4;
            this.tabRawData.Text = "Raw Data";
            this.tabRawData.UseVisualStyleBackColor = true;
            // 
            // tabDiagnosticLED
            // 
            this.tabDiagnosticLED.Controls.Add(this.gbLED);
            this.tabDiagnosticLED.Location = new System.Drawing.Point(4, 22);
            this.tabDiagnosticLED.Margin = new System.Windows.Forms.Padding(1);
            this.tabDiagnosticLED.Name = "tabDiagnosticLED";
            this.tabDiagnosticLED.Padding = new System.Windows.Forms.Padding(1);
            this.tabDiagnosticLED.Size = new System.Drawing.Size(520, 452);
            this.tabDiagnosticLED.TabIndex = 0;
            this.tabDiagnosticLED.Text = "Diagnostic LED";
            this.tabDiagnosticLED.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(533, 495);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.lblSerialPort);
            this.Controls.Add(this.cbSerialPort);
            this.Controls.Add(this.lblIdentification);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDI Test Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.epErrorProvider)).EndInit();
            this.gbRawDataControl.ResumeLayout(false);
            this.gbRawDataControl.PerformLayout();
            this.gbLED.ResumeLayout(false);
            this.gbLED.PerformLayout();
            this.gbWatchdog.ResumeLayout(false);
            this.gbWatchdog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchdogCountdown)).EndInit();
            this.gbPowerDown.ResumeLayout(false);
            this.gbPowerDown.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPowerDownDelay)).EndInit();
            this.gbPowerDownLevel.ResumeLayout(false);
            this.gbPowerDownLevel.PerformLayout();
            this.gbUSBDebug.ResumeLayout(false);
            this.gbUSBDebug.PerformLayout();
            this.gbDemo.ResumeLayout(false);
            this.gbDemo.PerformLayout();
            this.gbDemoSpeedAndStepping.ResumeLayout(false);
            this.gbDemoSpeedAndStepping.PerformLayout();
            this.gbModus.ResumeLayout(false);
            this.gbModus.PerformLayout();
            this.gbDemoStartAndEndPositions.ResumeLayout(false);
            this.gbDemoStartAndEndPositions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDemoEndPositionDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDemoStartPositionDecimal)).EndInit();
            this.gbStatorBaseAngles.ResumeLayout(false);
            this.gbStatorBaseAngles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS3BaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS2BaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatorS1BaseAngle)).EndInit();
            this.gbMovementLimits.ResumeLayout(false);
            this.gbMovementLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMin)).EndInit();
            this.gbMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabSynchroSetup.ResumeLayout(false);
            this.gbUpdateRateControl.ResumeLayout(false);
            this.gbUpdateRateControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpdateRateControlSpeed)).EndInit();
            this.gbURCMode.ResumeLayout(false);
            this.gbURCMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudURCSmoothModeThresholdDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudURCLimitThresholdDecimal)).EndInit();
            this.tabSynchroControl.ResumeLayout(false);
            this.gbSetStatorAmplitudeAndPolarityImmediate.ResumeLayout(false);
            this.gbSetStatorAmplitudeAndPolarityImmediate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudS3AmplitudeDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS2AmplitudeDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS1AmplitudeDecimal)).EndInit();
            this.gbMoveIndicatorCoarseResolution.ResumeLayout(false);
            this.gbMoveIndicatorCoarseResolution.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveIndicatorCoarseResolutionDecimal)).EndInit();
            this.gbIndicatorMovementControl.ResumeLayout(false);
            this.gbIndicatorMovementControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMoveIndicatorToPositionDecimal)).EndInit();
            this.tabDemoMode.ResumeLayout(false);
            this.tabDigitalAndPWMOutputs.ResumeLayout(false);
            this.gbDigitalAndPWMOutputs.ResumeLayout(false);
            this.gbDigitalAndPWMOutputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPWM_OUT_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_7_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_6_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_5_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_4_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_3_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_2_DutyCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDIG_PWM_1_DutyCycle)).EndInit();
            this.tabRawData.ResumeLayout(false);
            this.tabDiagnosticLED.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDeviceAddress;
        private System.Windows.Forms.ErrorProvider epErrorProvider;
        private System.Windows.Forms.Label lblSerialPort;
        private System.Windows.Forms.ComboBox cbSerialPort;
        private System.Windows.Forms.Label lblIdentification;
        private System.Windows.Forms.GroupBox gbRawDataControl;
        private System.Windows.Forms.Label lblSubAddr;
        private System.Windows.Forms.TextBox txtSubAddr;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblDataByte;
        private System.Windows.Forms.TextBox txtDataByte;
        private System.Windows.Forms.GroupBox gbLED;
        private System.Windows.Forms.RadioButton rdoToggleLEDPerAcceptedCommand;
        private System.Windows.Forms.RadioButton rdoLEDFlashesAtHeartbeatRate;
        private System.Windows.Forms.RadioButton rdoLEDAlwaysOn;
        private System.Windows.Forms.RadioButton rdoLEDAlwaysOff;
        private System.Windows.Forms.GroupBox gbWatchdog;
        private System.Windows.Forms.Label lblWatchdogCountdown;
        private System.Windows.Forms.NumericUpDown nudWatchdogCountdown;
        private System.Windows.Forms.CheckBox chkWatchdogEnabled;
        private System.Windows.Forms.Button btnDisableWatchdog;
        private System.Windows.Forms.GroupBox gbPowerDown;
        private System.Windows.Forms.Label lblPowerDownDelayTime;
        private System.Windows.Forms.NumericUpDown nudPowerDownDelay;
        private System.Windows.Forms.GroupBox gbPowerDownLevel;
        private System.Windows.Forms.RadioButton rdoPowerDownLevelHalf;
        private System.Windows.Forms.RadioButton rdoPowerDownLevelFull;
        private System.Windows.Forms.CheckBox chkPowerDownEnabled;
        private System.Windows.Forms.Label lblDelayDescr;
        private System.Windows.Forms.Label lblCountdownDesc;
        private System.Windows.Forms.GroupBox gbUSBDebug;
        private System.Windows.Forms.CheckBox chkUSBDebugEnabled;
        private System.Windows.Forms.GroupBox gbDemo;
        private System.Windows.Forms.GroupBox gbStatorBaseAngles;
        private System.Windows.Forms.Label lblStatorS3BaseAngleMSB;
        private System.Windows.Forms.Label lblStatorS2BaseAngleMSB;
        private System.Windows.Forms.Label lblStatorS1BaseAngleMSB;
        private System.Windows.Forms.Label lblStatorS3BaseAngleLSB;
        private System.Windows.Forms.Label lblStatorS2BaseAngleLSB;
        private System.Windows.Forms.Label lblStatorS1BaseAngleLSB;
        private System.Windows.Forms.NumericUpDown nudStatorS3BaseAngle;
        private System.Windows.Forms.Label lblStatorS3BaseAngle;
        private System.Windows.Forms.NumericUpDown nudStatorS2BaseAngle;
        private System.Windows.Forms.Label lblStatorS2BaseAngle;
        private System.Windows.Forms.NumericUpDown nudStatorS1BaseAngle;
        private System.Windows.Forms.Label lblStatorS1BaseAngle;
        private System.Windows.Forms.Label lblStatorS3BaseAngleDegrees;
        private System.Windows.Forms.Label lblStatorS2BaseAngleDegrees;
        private System.Windows.Forms.Label lblStatorS1BaseAngleDegrees;
        private System.Windows.Forms.Label lblStatorS3BaseAngleHex;
        private System.Windows.Forms.Label lblStatorS2BaseAngleHex;
        private System.Windows.Forms.Label lblStatorS1BaseAngleHex;
        private System.Windows.Forms.Button btnUpdateStatorBaseAngles;
        private System.Windows.Forms.GroupBox gbMovementLimits;
        private System.Windows.Forms.NumericUpDown nudLimitMax;
        private System.Windows.Forms.Label lblLimitMax;
        private System.Windows.Forms.NumericUpDown nudLimitMin;
        private System.Windows.Forms.Label lblLimitMin;
        private System.Windows.Forms.Label lblLimitMinDesc;
        private System.Windows.Forms.Label lblLimitMaxDesc;
        private System.Windows.Forms.GroupBox gbDemoStartAndEndPositions;
        private System.Windows.Forms.Label lblDemoEndPositionHex;
        private System.Windows.Forms.Label lblDemoStartPositionHex;
        private System.Windows.Forms.Label lblDemoEndPositionDegrees;
        private System.Windows.Forms.Label lblDemoStartPositionDegrees;
        private System.Windows.Forms.NumericUpDown nudDemoEndPositionDecimal;
        private System.Windows.Forms.Label lblDemoEndPositionDecimal;
        private System.Windows.Forms.NumericUpDown nudDemoStartPositionDecimal;
        private System.Windows.Forms.Label lblDemoStartPosition;
        private System.Windows.Forms.Label lblLimitMaximumHex;
        private System.Windows.Forms.Label lblLimitMaximumDegrees;
        private System.Windows.Forms.Label lblLimitMinimumHex;
        private System.Windows.Forms.Label lblLimitMinimumDegrees;
        private System.Windows.Forms.Label lblDemoMovementSpeed;
        private System.Windows.Forms.ComboBox cboDemoMovementSpeed;
        private System.Windows.Forms.Label lblDemoMovementStepSize;
        private System.Windows.Forms.GroupBox gbModus;
        private System.Windows.Forms.RadioButton rdoDemoModusStartToEndJumpToStart;
        private System.Windows.Forms.RadioButton rdoModusStartToEndToStart;
        private System.Windows.Forms.ComboBox cboDemoMovementStepSize;
        private System.Windows.Forms.Label lblDemoMovementStepSizeDesc;
        private System.Windows.Forms.Label lblDemoMovementSpeedDesc;
        private System.Windows.Forms.GroupBox gbDemoSpeedAndStepping;
        private System.Windows.Forms.CheckBox chkStartDemo;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDiagnosticLED;
        private System.Windows.Forms.TabPage tabSynchroSetup;
        private System.Windows.Forms.TabPage tabDemoMode;
        private System.Windows.Forms.TabPage tabSynchroControl;
        private System.Windows.Forms.TabPage tabRawData;
        private System.Windows.Forms.GroupBox gbUpdateRateControl;
        private System.Windows.Forms.GroupBox gbURCMode;
        private System.Windows.Forms.Label lblURCLimitThreshold;
        private System.Windows.Forms.Label lblURCLimitThresholdHex;
        private System.Windows.Forms.Label lblURCLimitThresholdDegrees;
        private System.Windows.Forms.RadioButton rdoURCLimitMode;
        private System.Windows.Forms.NumericUpDown nudURCLimitThresholdDecimal;
        private System.Windows.Forms.GroupBox gbIndicatorMovementControl;
        private System.Windows.Forms.Label lblURCSmoothModeThreshold;
        private System.Windows.Forms.Label lblURCSmoothModeThresholdHex;
        private System.Windows.Forms.Label lblURCSmoothModeThresholdDegrees;
        private System.Windows.Forms.NumericUpDown nudURCSmoothModeThresholdDecimal;
        private System.Windows.Forms.RadioButton rdoURCSmoothMode;
        private System.Windows.Forms.Label lblURCSmoothModeSmoothUpdates;
        private System.Windows.Forms.ComboBox cboURCSmoothModeSmoothUpdates;
        private System.Windows.Forms.CheckBox chkUpdateRateControlShortestPath;
        private System.Windows.Forms.Label lblUpdateRateControlSpeedDesc;
        private System.Windows.Forms.Label lblUpdateRateControlSpeed;
        private System.Windows.Forms.NumericUpDown nudUpdateRateControlSpeed;
        private System.Windows.Forms.Label lblMoveIndicatorInQuadrant;
        private System.Windows.Forms.ComboBox cboMoveIndicatorInQuadrant;
        private System.Windows.Forms.Label lblMoveIndicatorToPositionHex;
        private System.Windows.Forms.Label lblMoveIndicatorToPositionDegrees;
        private System.Windows.Forms.NumericUpDown nudMoveIndicatorToPositionDecimal;
        private System.Windows.Forms.Label lblMoveIndicatorToPosition;
        private System.Windows.Forms.GroupBox gbMoveIndicatorCoarseResolution;
        private System.Windows.Forms.Label lblMoveIndicatorCoarseResolutionHex;
        private System.Windows.Forms.Label lblMoveIndicatorCoarseResolutionDegrees;
        private System.Windows.Forms.NumericUpDown nudMoveIndicatorCoarseResolutionDecimal;
        private System.Windows.Forms.Label lblMoveIndicatorCoarseResolutionPositionDecimal;
        private System.Windows.Forms.GroupBox gbSetStatorAmplitudeAndPolarityImmediate;
        private System.Windows.Forms.Label lblS3AmplitudeHex;
        private System.Windows.Forms.NumericUpDown nudS3AmplitudeDecimal;
        private System.Windows.Forms.Label lblS3AmplitudePolarity;
        private System.Windows.Forms.Label lblS2AmplitudeHex;
        private System.Windows.Forms.NumericUpDown nudS2AmplitudeDecimal;
        private System.Windows.Forms.Label lblS2AmplitudePolarity;
        private System.Windows.Forms.Label lblS1AmplitudeHex;
        private System.Windows.Forms.NumericUpDown nudS1AmplitudeDecimal;
        private System.Windows.Forms.Label lblS1AmplitudePolarity;
        private System.Windows.Forms.CheckBox chkS3Polarity;
        private System.Windows.Forms.CheckBox chkS2Polarity;
        private System.Windows.Forms.CheckBox chkS1Polarity;
        private System.Windows.Forms.Label lblPolarities;
        private System.Windows.Forms.Label lblAmplitudes;
        private System.Windows.Forms.Label lblStators;
        private System.Windows.Forms.Label lblS3AmplitudeDecimal;
        private System.Windows.Forms.Label lblS2AmplitudeDecimal;
        private System.Windows.Forms.Label lblS1AmplitudeDecimal;
        private System.Windows.Forms.Button btnUpdateStatorAmplitudesAndPolarities;
        private System.Windows.Forms.Label lblStatorAmplitudeAndPolarityUpdateMode;
        private System.Windows.Forms.RadioButton rdoStatorAmplitudeAndPolarityDeferredUpdates;
        private System.Windows.Forms.RadioButton rdoStatorAmplitudeAndPolarityImmediateUpdates;
        private System.Windows.Forms.TabPage tabDigitalAndPWMOutputs;
        private System.Windows.Forms.GroupBox gbDigitalAndPWMOutputs;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_1_DutyCycle;
        private System.Windows.Forms.Label lblDutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_1_Value;
        private System.Windows.Forms.Label lblDigitalAndPWMChannelValues;
        private System.Windows.Forms.ComboBox cboDIG_PWM_1_Mode;
        private System.Windows.Forms.Label lblDigitalOrPWM;
        private System.Windows.Forms.Label lblChannels;
        private System.Windows.Forms.Label lblDIG_PWM_1;
        private System.Windows.Forms.Label lblDIG_PWM_1_Hex;
        private System.Windows.Forms.Label lblDIG_PWM_3_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_3_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_3_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_3_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_3;
        private System.Windows.Forms.Label lblDIG_PWM_2_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_2_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_2_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_2_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_2;
        private System.Windows.Forms.Label lblDIG_PWM_4_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_4_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_4_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_4_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_4;
        private System.Windows.Forms.Label lblDIG_PWM_5_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_5_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_5_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_5_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_5;
        private System.Windows.Forms.Label lblDIG_PWM_6_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_6_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_6_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_6_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_6;
        private System.Windows.Forms.Label lblDIG_PWM_7_Hex;
        private System.Windows.Forms.NumericUpDown nudDIG_PWM_7_DutyCycle;
        private System.Windows.Forms.ComboBox cboDIG_PWM_7_Value;
        private System.Windows.Forms.ComboBox cboDIG_PWM_7_Mode;
        private System.Windows.Forms.Label lblDIG_PWM_7;
        private System.Windows.Forms.Label lblPWM_OUT_Hex;
        private System.Windows.Forms.NumericUpDown nudPWM_OUT_DutyCycle;
        private System.Windows.Forms.Label lblPWM_OUT;
        private System.Windows.Forms.ComboBox cboPWM_OUT_Value;
        private System.Windows.Forms.ComboBox cboPWM_OUT_Mode;
    }
}

