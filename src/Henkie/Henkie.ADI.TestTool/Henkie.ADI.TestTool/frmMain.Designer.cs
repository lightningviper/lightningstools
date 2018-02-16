namespace Henkie.ADI.TestTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblDeviceAddress = new System.Windows.Forms.Label();
            this.epErrorProvider = new System.Windows.Forms.ErrorProvider();
            this.gbPitchRawDataControl = new System.Windows.Forms.GroupBox();
            this.lblPitchSubAddr = new System.Windows.Forms.Label();
            this.txtPitchSubAddr = new System.Windows.Forms.TextBox();
            this.btnPitchSendRaw = new System.Windows.Forms.Button();
            this.lblPitchDataByte = new System.Windows.Forms.Label();
            this.txtPitchDataByte = new System.Windows.Forms.TextBox();
            this.lblPitchDeviceSerialPort = new System.Windows.Forms.Label();
            this.cbPitchDeviceSerialPort = new System.Windows.Forms.ComboBox();
            this.lblPitchDeviceIdentification = new System.Windows.Forms.Label();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabADI = new System.Windows.Forms.TabPage();
            this.nudRollIndication = new System.Windows.Forms.NumericUpDown();
            this.nudSpherePitchIndication = new System.Windows.Forms.NumericUpDown();
            this.nudRateOfTurnIndicator = new System.Windows.Forms.NumericUpDown();
            this.nudGlideslopeIndicatorVertical = new System.Windows.Forms.NumericUpDown();
            this.nudGlideslopeIndicatorHorizontal = new System.Windows.Forms.NumericUpDown();
            this.lblSphereRollIndication = new System.Windows.Forms.Label();
            this.lblSpherePitchIndication = new System.Windows.Forms.Label();
            this.lblRateOfTurnIndicator = new System.Windows.Forms.Label();
            this.lblGlideslopeIndicatorVertical = new System.Windows.Forms.Label();
            this.lblGlideslopeIndicatorHorizontal = new System.Windows.Forms.Label();
            this.chkEnableRollAndPitch = new System.Windows.Forms.CheckBox();
            this.chkEnableGlideslope = new System.Windows.Forms.CheckBox();
            this.chkEnableFlagsAndRot = new System.Windows.Forms.CheckBox();
            this.chkAuxFlagVisible = new System.Windows.Forms.CheckBox();
            this.chkLocFlagVisible = new System.Windows.Forms.CheckBox();
            this.chkGsFlagVisible = new System.Windows.Forms.CheckBox();
            this.tabRawData = new System.Windows.Forms.TabPage();
            this.gbRollRawDataControl = new System.Windows.Forms.GroupBox();
            this.lblRollSubAddr = new System.Windows.Forms.Label();
            this.txtRollSubAddr = new System.Windows.Forms.TextBox();
            this.btnRollSendRaw = new System.Windows.Forms.Button();
            this.lblRollDataByte = new System.Windows.Forms.Label();
            this.txtRollDataByte = new System.Windows.Forms.TextBox();
            this.lblRollDeviceSerialPort = new System.Windows.Forms.Label();
            this.cbRollDeviceSerialPort = new System.Windows.Forms.ComboBox();
            this.lblRollDeviceIdentification = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epErrorProvider)).BeginInit();
            this.gbPitchRawDataControl.SuspendLayout();
            this.gbMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabADI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRollIndication)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpherePitchIndication)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRateOfTurnIndicator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGlideslopeIndicatorVertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGlideslopeIndicatorHorizontal)).BeginInit();
            this.tabRawData.SuspendLayout();
            this.gbRollRawDataControl.SuspendLayout();
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
            // gbPitchRawDataControl
            // 
            this.gbPitchRawDataControl.Controls.Add(this.lblPitchSubAddr);
            this.gbPitchRawDataControl.Controls.Add(this.txtPitchSubAddr);
            this.gbPitchRawDataControl.Controls.Add(this.btnPitchSendRaw);
            this.gbPitchRawDataControl.Controls.Add(this.lblPitchDataByte);
            this.gbPitchRawDataControl.Controls.Add(this.txtPitchDataByte);
            this.gbPitchRawDataControl.Location = new System.Drawing.Point(3, 3);
            this.gbPitchRawDataControl.Name = "gbPitchRawDataControl";
            this.gbPitchRawDataControl.Size = new System.Drawing.Size(152, 152);
            this.gbPitchRawDataControl.TabIndex = 0;
            this.gbPitchRawDataControl.TabStop = false;
            this.gbPitchRawDataControl.Text = "Raw Data Control (Pitch)";
            // 
            // lblPitchSubAddr
            // 
            this.lblPitchSubAddr.AutoSize = true;
            this.lblPitchSubAddr.Location = new System.Drawing.Point(18, 61);
            this.lblPitchSubAddr.Name = "lblPitchSubAddr";
            this.lblPitchSubAddr.Size = new System.Drawing.Size(66, 13);
            this.lblPitchSubAddr.TabIndex = 2;
            this.lblPitchSubAddr.Text = "S&ubaddress:";
            this.lblPitchSubAddr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPitchSubAddr
            // 
            this.txtPitchSubAddr.Location = new System.Drawing.Point(90, 58);
            this.txtPitchSubAddr.MaxLength = 4;
            this.txtPitchSubAddr.Name = "txtPitchSubAddr";
            this.txtPitchSubAddr.Size = new System.Drawing.Size(46, 20);
            this.txtPitchSubAddr.TabIndex = 3;
            this.txtPitchSubAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPitchSubAddr.Leave += new System.EventHandler(this.txtPitchSubAddr_Leave);
            // 
            // btnPitchSendRaw
            // 
            this.btnPitchSendRaw.Location = new System.Drawing.Point(21, 116);
            this.btnPitchSendRaw.Name = "btnPitchSendRaw";
            this.btnPitchSendRaw.Size = new System.Drawing.Size(114, 23);
            this.btnPitchSendRaw.TabIndex = 6;
            this.btnPitchSendRaw.Text = "&Send";
            this.btnPitchSendRaw.UseVisualStyleBackColor = true;
            this.btnPitchSendRaw.Click += new System.EventHandler(this.btnPitchSendRaw_Click);
            // 
            // lblPitchDataByte
            // 
            this.lblPitchDataByte.AutoSize = true;
            this.lblPitchDataByte.Location = new System.Drawing.Point(28, 87);
            this.lblPitchDataByte.Name = "lblPitchDataByte";
            this.lblPitchDataByte.Size = new System.Drawing.Size(57, 13);
            this.lblPitchDataByte.TabIndex = 4;
            this.lblPitchDataByte.Text = "Data &Byte:";
            this.lblPitchDataByte.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPitchDataByte
            // 
            this.txtPitchDataByte.Location = new System.Drawing.Point(90, 84);
            this.txtPitchDataByte.MaxLength = 4;
            this.txtPitchDataByte.Name = "txtPitchDataByte";
            this.txtPitchDataByte.Size = new System.Drawing.Size(46, 20);
            this.txtPitchDataByte.TabIndex = 5;
            this.txtPitchDataByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPitchDataByte.Leave += new System.EventHandler(this.txtPitchDataByte_Leave);
            // 
            // lblPitchDeviceSerialPort
            // 
            this.lblPitchDeviceSerialPort.AutoSize = true;
            this.lblPitchDeviceSerialPort.Location = new System.Drawing.Point(8, 9);
            this.lblPitchDeviceSerialPort.Name = "lblPitchDeviceSerialPort";
            this.lblPitchDeviceSerialPort.Size = new System.Drawing.Size(106, 13);
            this.lblPitchDeviceSerialPort.TabIndex = 5;
            this.lblPitchDeviceSerialPort.Text = "Pitch SDI Serial &Port:";
            // 
            // cbPitchDeviceSerialPort
            // 
            this.cbPitchDeviceSerialPort.FormattingEnabled = true;
            this.cbPitchDeviceSerialPort.Location = new System.Drawing.Point(119, 7);
            this.cbPitchDeviceSerialPort.Name = "cbPitchDeviceSerialPort";
            this.cbPitchDeviceSerialPort.Size = new System.Drawing.Size(121, 21);
            this.cbPitchDeviceSerialPort.TabIndex = 4;
            this.cbPitchDeviceSerialPort.SelectedIndexChanged += new System.EventHandler(this.cbPitchDeviceSerialPort_SelectedIndexChanged);
            // 
            // lblPitchDeviceIdentification
            // 
            this.lblPitchDeviceIdentification.AutoSize = true;
            this.lblPitchDeviceIdentification.Location = new System.Drawing.Point(265, 9);
            this.lblPitchDeviceIdentification.Name = "lblPitchDeviceIdentification";
            this.lblPitchDeviceIdentification.Size = new System.Drawing.Size(70, 13);
            this.lblPitchDeviceIdentification.TabIndex = 6;
            this.lblPitchDeviceIdentification.Text = "Identification:";
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.tabControl1);
            this.gbMain.Location = new System.Drawing.Point(2, 47);
            this.gbMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbMain.Name = "gbMain";
            this.gbMain.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbMain.Size = new System.Drawing.Size(530, 240);
            this.gbMain.TabIndex = 11;
            this.gbMain.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabADI);
            this.tabControl1.Controls.Add(this.tabRawData);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(2, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(526, 223);
            this.tabControl1.TabIndex = 0;
            // 
            // tabADI
            // 
            this.tabADI.Controls.Add(this.nudRollIndication);
            this.tabADI.Controls.Add(this.nudSpherePitchIndication);
            this.tabADI.Controls.Add(this.nudRateOfTurnIndicator);
            this.tabADI.Controls.Add(this.nudGlideslopeIndicatorVertical);
            this.tabADI.Controls.Add(this.nudGlideslopeIndicatorHorizontal);
            this.tabADI.Controls.Add(this.lblSphereRollIndication);
            this.tabADI.Controls.Add(this.lblSpherePitchIndication);
            this.tabADI.Controls.Add(this.lblRateOfTurnIndicator);
            this.tabADI.Controls.Add(this.lblGlideslopeIndicatorVertical);
            this.tabADI.Controls.Add(this.lblGlideslopeIndicatorHorizontal);
            this.tabADI.Controls.Add(this.chkEnableRollAndPitch);
            this.tabADI.Controls.Add(this.chkEnableGlideslope);
            this.tabADI.Controls.Add(this.chkEnableFlagsAndRot);
            this.tabADI.Controls.Add(this.chkAuxFlagVisible);
            this.tabADI.Controls.Add(this.chkLocFlagVisible);
            this.tabADI.Controls.Add(this.chkGsFlagVisible);
            this.tabADI.Location = new System.Drawing.Point(4, 22);
            this.tabADI.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabADI.Name = "tabADI";
            this.tabADI.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabADI.Size = new System.Drawing.Size(518, 197);
            this.tabADI.TabIndex = 5;
            this.tabADI.Text = "ADI";
            this.tabADI.UseVisualStyleBackColor = true;
            // 
            // nudRollIndication
            // 
            this.nudRollIndication.Location = new System.Drawing.Point(173, 159);
            this.nudRollIndication.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudRollIndication.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudRollIndication.Name = "nudRollIndication";
            this.nudRollIndication.Size = new System.Drawing.Size(60, 20);
            this.nudRollIndication.TabIndex = 15;
            this.nudRollIndication.ValueChanged += new System.EventHandler(this.nudRollIndication_ValueChanged);
            // 
            // nudSpherePitchIndication
            // 
            this.nudSpherePitchIndication.Location = new System.Drawing.Point(173, 139);
            this.nudSpherePitchIndication.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudSpherePitchIndication.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudSpherePitchIndication.Name = "nudSpherePitchIndication";
            this.nudSpherePitchIndication.Size = new System.Drawing.Size(60, 20);
            this.nudSpherePitchIndication.TabIndex = 14;
            this.nudSpherePitchIndication.ValueChanged += new System.EventHandler(this.nudSpherePitchIndication_ValueChanged);
            // 
            // nudRateOfTurnIndicator
            // 
            this.nudRateOfTurnIndicator.Location = new System.Drawing.Point(173, 120);
            this.nudRateOfTurnIndicator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudRateOfTurnIndicator.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRateOfTurnIndicator.Name = "nudRateOfTurnIndicator";
            this.nudRateOfTurnIndicator.Size = new System.Drawing.Size(60, 20);
            this.nudRateOfTurnIndicator.TabIndex = 13;
            this.nudRateOfTurnIndicator.ValueChanged += new System.EventHandler(this.nudRateOfTurnIndicator_ValueChanged);
            // 
            // nudGlideslopeIndicatorVertical
            // 
            this.nudGlideslopeIndicatorVertical.Location = new System.Drawing.Point(173, 101);
            this.nudGlideslopeIndicatorVertical.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudGlideslopeIndicatorVertical.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGlideslopeIndicatorVertical.Name = "nudGlideslopeIndicatorVertical";
            this.nudGlideslopeIndicatorVertical.Size = new System.Drawing.Size(60, 20);
            this.nudGlideslopeIndicatorVertical.TabIndex = 12;
            this.nudGlideslopeIndicatorVertical.ValueChanged += new System.EventHandler(this.nudGlideslopeIndicatorVertical_ValueChanged);
            // 
            // nudGlideslopeIndicatorHorizontal
            // 
            this.nudGlideslopeIndicatorHorizontal.Location = new System.Drawing.Point(173, 82);
            this.nudGlideslopeIndicatorHorizontal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudGlideslopeIndicatorHorizontal.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGlideslopeIndicatorHorizontal.Name = "nudGlideslopeIndicatorHorizontal";
            this.nudGlideslopeIndicatorHorizontal.Size = new System.Drawing.Size(60, 20);
            this.nudGlideslopeIndicatorHorizontal.TabIndex = 11;
            this.nudGlideslopeIndicatorHorizontal.ValueChanged += new System.EventHandler(this.nudGlideslopeIndicatorHorizontal_ValueChanged);
            // 
            // lblSphereRollIndication
            // 
            this.lblSphereRollIndication.AutoSize = true;
            this.lblSphereRollIndication.Location = new System.Drawing.Point(16, 160);
            this.lblSphereRollIndication.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSphereRollIndication.Name = "lblSphereRollIndication";
            this.lblSphereRollIndication.Size = new System.Drawing.Size(120, 13);
            this.lblSphereRollIndication.TabIndex = 10;
            this.lblSphereRollIndication.Text = "Sphere ROLL indication";
            // 
            // lblSpherePitchIndication
            // 
            this.lblSpherePitchIndication.AutoSize = true;
            this.lblSpherePitchIndication.Location = new System.Drawing.Point(16, 141);
            this.lblSpherePitchIndication.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSpherePitchIndication.Name = "lblSpherePitchIndication";
            this.lblSpherePitchIndication.Size = new System.Drawing.Size(124, 13);
            this.lblSpherePitchIndication.TabIndex = 9;
            this.lblSpherePitchIndication.Text = "Sphere PITCH indication";
            // 
            // lblRateOfTurnIndicator
            // 
            this.lblRateOfTurnIndicator.AutoSize = true;
            this.lblRateOfTurnIndicator.Location = new System.Drawing.Point(16, 122);
            this.lblRateOfTurnIndicator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRateOfTurnIndicator.Name = "lblRateOfTurnIndicator";
            this.lblRateOfTurnIndicator.Size = new System.Drawing.Size(111, 13);
            this.lblRateOfTurnIndicator.TabIndex = 8;
            this.lblRateOfTurnIndicator.Text = "Rate of Turn Indicator";
            // 
            // lblGlideslopeIndicatorVertical
            // 
            this.lblGlideslopeIndicatorVertical.AutoSize = true;
            this.lblGlideslopeIndicatorVertical.Location = new System.Drawing.Point(16, 102);
            this.lblGlideslopeIndicatorVertical.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGlideslopeIndicatorVertical.Name = "lblGlideslopeIndicatorVertical";
            this.lblGlideslopeIndicatorVertical.Size = new System.Drawing.Size(138, 13);
            this.lblGlideslopeIndicatorVertical.TabIndex = 7;
            this.lblGlideslopeIndicatorVertical.Text = "Glideslope Indicator Vertical";
            // 
            // lblGlideslopeIndicatorHorizontal
            // 
            this.lblGlideslopeIndicatorHorizontal.AutoSize = true;
            this.lblGlideslopeIndicatorHorizontal.Location = new System.Drawing.Point(16, 83);
            this.lblGlideslopeIndicatorHorizontal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGlideslopeIndicatorHorizontal.Name = "lblGlideslopeIndicatorHorizontal";
            this.lblGlideslopeIndicatorHorizontal.Size = new System.Drawing.Size(150, 13);
            this.lblGlideslopeIndicatorHorizontal.TabIndex = 6;
            this.lblGlideslopeIndicatorHorizontal.Text = "Glideslope Indicator Horizontal";
            // 
            // chkEnableRollAndPitch
            // 
            this.chkEnableRollAndPitch.AutoSize = true;
            this.chkEnableRollAndPitch.Location = new System.Drawing.Point(188, 49);
            this.chkEnableRollAndPitch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkEnableRollAndPitch.Name = "chkEnableRollAndPitch";
            this.chkEnableRollAndPitch.Size = new System.Drawing.Size(137, 17);
            this.chkEnableRollAndPitch.TabIndex = 5;
            this.chkEnableRollAndPitch.Text = "ENABLE Roll and Pitch";
            this.chkEnableRollAndPitch.UseVisualStyleBackColor = true;
            this.chkEnableRollAndPitch.CheckedChanged += new System.EventHandler(this.chkEnableRollAndPitch_CheckedChanged);
            // 
            // chkEnableGlideslope
            // 
            this.chkEnableGlideslope.AutoSize = true;
            this.chkEnableGlideslope.Location = new System.Drawing.Point(188, 31);
            this.chkEnableGlideslope.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkEnableGlideslope.Name = "chkEnableGlideslope";
            this.chkEnableGlideslope.Size = new System.Drawing.Size(120, 17);
            this.chkEnableGlideslope.TabIndex = 4;
            this.chkEnableGlideslope.Text = "ENABLE Glideslope";
            this.chkEnableGlideslope.UseVisualStyleBackColor = true;
            this.chkEnableGlideslope.CheckedChanged += new System.EventHandler(this.chkEnableGlideslope_CheckedChanged);
            // 
            // chkEnableFlagsAndRot
            // 
            this.chkEnableFlagsAndRot.AutoSize = true;
            this.chkEnableFlagsAndRot.Location = new System.Drawing.Point(188, 12);
            this.chkEnableFlagsAndRot.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkEnableFlagsAndRot.Name = "chkEnableFlagsAndRot";
            this.chkEnableFlagsAndRot.Size = new System.Drawing.Size(143, 17);
            this.chkEnableFlagsAndRot.TabIndex = 3;
            this.chkEnableFlagsAndRot.Text = "ENABLE Flags and ROT";
            this.chkEnableFlagsAndRot.UseVisualStyleBackColor = true;
            this.chkEnableFlagsAndRot.CheckedChanged += new System.EventHandler(this.chkEnableFlagsAndRot_CheckedChanged);
            // 
            // chkAuxFlagVisible
            // 
            this.chkAuxFlagVisible.AutoSize = true;
            this.chkAuxFlagVisible.Location = new System.Drawing.Point(18, 49);
            this.chkAuxFlagVisible.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkAuxFlagVisible.Name = "chkAuxFlagVisible";
            this.chkAuxFlagVisible.Size = new System.Drawing.Size(104, 17);
            this.chkAuxFlagVisible.TabIndex = 2;
            this.chkAuxFlagVisible.Text = "AUX Flag Visible";
            this.chkAuxFlagVisible.UseVisualStyleBackColor = true;
            this.chkAuxFlagVisible.CheckedChanged += new System.EventHandler(this.chkAuxFlagVisible_CheckedChanged);
            // 
            // chkLocFlagVisible
            // 
            this.chkLocFlagVisible.AutoSize = true;
            this.chkLocFlagVisible.Location = new System.Drawing.Point(18, 31);
            this.chkLocFlagVisible.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkLocFlagVisible.Name = "chkLocFlagVisible";
            this.chkLocFlagVisible.Size = new System.Drawing.Size(103, 17);
            this.chkLocFlagVisible.TabIndex = 1;
            this.chkLocFlagVisible.Text = "LOC Flag Visible";
            this.chkLocFlagVisible.UseVisualStyleBackColor = true;
            this.chkLocFlagVisible.CheckedChanged += new System.EventHandler(this.chkLocFlagVisible_CheckedChanged);
            // 
            // chkGsFlagVisible
            // 
            this.chkGsFlagVisible.AutoSize = true;
            this.chkGsFlagVisible.Location = new System.Drawing.Point(18, 12);
            this.chkGsFlagVisible.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkGsFlagVisible.Name = "chkGsFlagVisible";
            this.chkGsFlagVisible.Size = new System.Drawing.Size(97, 17);
            this.chkGsFlagVisible.TabIndex = 0;
            this.chkGsFlagVisible.Text = "GS Flag Visible";
            this.chkGsFlagVisible.UseVisualStyleBackColor = true;
            this.chkGsFlagVisible.CheckedChanged += new System.EventHandler(this.chkGsFlagVisible_CheckedChanged);
            // 
            // tabRawData
            // 
            this.tabRawData.Controls.Add(this.gbRollRawDataControl);
            this.tabRawData.Controls.Add(this.gbPitchRawDataControl);
            this.tabRawData.Location = new System.Drawing.Point(4, 22);
            this.tabRawData.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabRawData.Name = "tabRawData";
            this.tabRawData.Size = new System.Drawing.Size(518, 197);
            this.tabRawData.TabIndex = 4;
            this.tabRawData.Text = "Raw Data";
            this.tabRawData.UseVisualStyleBackColor = true;
            // 
            // gbRollRawDataControl
            // 
            this.gbRollRawDataControl.Controls.Add(this.lblRollSubAddr);
            this.gbRollRawDataControl.Controls.Add(this.txtRollSubAddr);
            this.gbRollRawDataControl.Controls.Add(this.btnRollSendRaw);
            this.gbRollRawDataControl.Controls.Add(this.lblRollDataByte);
            this.gbRollRawDataControl.Controls.Add(this.txtRollDataByte);
            this.gbRollRawDataControl.Location = new System.Drawing.Point(161, 3);
            this.gbRollRawDataControl.Name = "gbRollRawDataControl";
            this.gbRollRawDataControl.Size = new System.Drawing.Size(152, 152);
            this.gbRollRawDataControl.TabIndex = 7;
            this.gbRollRawDataControl.TabStop = false;
            this.gbRollRawDataControl.Text = "Raw Data Control (Roll)";
            // 
            // lblRollSubAddr
            // 
            this.lblRollSubAddr.AutoSize = true;
            this.lblRollSubAddr.Location = new System.Drawing.Point(18, 61);
            this.lblRollSubAddr.Name = "lblRollSubAddr";
            this.lblRollSubAddr.Size = new System.Drawing.Size(66, 13);
            this.lblRollSubAddr.TabIndex = 2;
            this.lblRollSubAddr.Text = "S&ubaddress:";
            this.lblRollSubAddr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRollSubAddr
            // 
            this.txtRollSubAddr.Location = new System.Drawing.Point(90, 58);
            this.txtRollSubAddr.MaxLength = 4;
            this.txtRollSubAddr.Name = "txtRollSubAddr";
            this.txtRollSubAddr.Size = new System.Drawing.Size(46, 20);
            this.txtRollSubAddr.TabIndex = 3;
            this.txtRollSubAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRollSubAddr.Leave += new System.EventHandler(this.txtRollSubAddr_Leave);
            // 
            // btnRollSendRaw
            // 
            this.btnRollSendRaw.Location = new System.Drawing.Point(21, 116);
            this.btnRollSendRaw.Name = "btnRollSendRaw";
            this.btnRollSendRaw.Size = new System.Drawing.Size(114, 23);
            this.btnRollSendRaw.TabIndex = 6;
            this.btnRollSendRaw.Text = "&Send";
            this.btnRollSendRaw.UseVisualStyleBackColor = true;
            this.btnRollSendRaw.Click += new System.EventHandler(this.btnRollSendRaw_Click);
            // 
            // lblRollDataByte
            // 
            this.lblRollDataByte.AutoSize = true;
            this.lblRollDataByte.Location = new System.Drawing.Point(28, 87);
            this.lblRollDataByte.Name = "lblRollDataByte";
            this.lblRollDataByte.Size = new System.Drawing.Size(57, 13);
            this.lblRollDataByte.TabIndex = 4;
            this.lblRollDataByte.Text = "Data &Byte:";
            this.lblRollDataByte.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRollDataByte
            // 
            this.txtRollDataByte.Location = new System.Drawing.Point(90, 84);
            this.txtRollDataByte.MaxLength = 4;
            this.txtRollDataByte.Name = "txtRollDataByte";
            this.txtRollDataByte.Size = new System.Drawing.Size(46, 20);
            this.txtRollDataByte.TabIndex = 5;
            this.txtRollDataByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRollDataByte.Leave += new System.EventHandler(this.txtRollDataByte_Leave);
            // 
            // lblRollDeviceSerialPort
            // 
            this.lblRollDeviceSerialPort.AutoSize = true;
            this.lblRollDeviceSerialPort.Location = new System.Drawing.Point(8, 32);
            this.lblRollDeviceSerialPort.Name = "lblRollDeviceSerialPort";
            this.lblRollDeviceSerialPort.Size = new System.Drawing.Size(100, 13);
            this.lblRollDeviceSerialPort.TabIndex = 13;
            this.lblRollDeviceSerialPort.Text = "Roll SDI Serial &Port:";
            // 
            // cbRollDeviceSerialPort
            // 
            this.cbRollDeviceSerialPort.FormattingEnabled = true;
            this.cbRollDeviceSerialPort.Location = new System.Drawing.Point(119, 30);
            this.cbRollDeviceSerialPort.Name = "cbRollDeviceSerialPort";
            this.cbRollDeviceSerialPort.Size = new System.Drawing.Size(121, 21);
            this.cbRollDeviceSerialPort.TabIndex = 12;
            this.cbRollDeviceSerialPort.SelectedIndexChanged += new System.EventHandler(this.cbRollDeviceSerialPort_SelectedIndexChanged);
            // 
            // lblRollDeviceIdentification
            // 
            this.lblRollDeviceIdentification.AutoSize = true;
            this.lblRollDeviceIdentification.Location = new System.Drawing.Point(265, 32);
            this.lblRollDeviceIdentification.Name = "lblRollDeviceIdentification";
            this.lblRollDeviceIdentification.Size = new System.Drawing.Size(70, 13);
            this.lblRollDeviceIdentification.TabIndex = 14;
            this.lblRollDeviceIdentification.Text = "Identification:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(534, 288);
            this.Controls.Add(this.lblRollDeviceSerialPort);
            this.Controls.Add(this.cbRollDeviceSerialPort);
            this.Controls.Add(this.lblRollDeviceIdentification);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.lblPitchDeviceSerialPort);
            this.Controls.Add(this.cbPitchDeviceSerialPort);
            this.Controls.Add(this.lblPitchDeviceIdentification);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ADI Test Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.epErrorProvider)).EndInit();
            this.gbPitchRawDataControl.ResumeLayout(false);
            this.gbPitchRawDataControl.PerformLayout();
            this.gbMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabADI.ResumeLayout(false);
            this.tabADI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRollIndication)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpherePitchIndication)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRateOfTurnIndicator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGlideslopeIndicatorVertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGlideslopeIndicatorHorizontal)).EndInit();
            this.tabRawData.ResumeLayout(false);
            this.gbRollRawDataControl.ResumeLayout(false);
            this.gbRollRawDataControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDeviceAddress;
        private System.Windows.Forms.ErrorProvider epErrorProvider;
        private System.Windows.Forms.Label lblPitchDeviceSerialPort;
        private System.Windows.Forms.ComboBox cbPitchDeviceSerialPort;
        private System.Windows.Forms.Label lblPitchDeviceIdentification;
        private System.Windows.Forms.GroupBox gbPitchRawDataControl;
        private System.Windows.Forms.Label lblPitchSubAddr;
        private System.Windows.Forms.TextBox txtPitchSubAddr;
        private System.Windows.Forms.Button btnPitchSendRaw;
        private System.Windows.Forms.Label lblPitchDataByte;
        private System.Windows.Forms.TextBox txtPitchDataByte;

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRawData;
        private System.Windows.Forms.Label lblRollDeviceSerialPort;
        private System.Windows.Forms.ComboBox cbRollDeviceSerialPort;
        private System.Windows.Forms.Label lblRollDeviceIdentification;
        private System.Windows.Forms.GroupBox gbRollRawDataControl;
        private System.Windows.Forms.Label lblRollSubAddr;
        private System.Windows.Forms.TextBox txtRollSubAddr;
        private System.Windows.Forms.Button btnRollSendRaw;
        private System.Windows.Forms.Label lblRollDataByte;
        private System.Windows.Forms.TextBox txtRollDataByte;
        private System.Windows.Forms.TabPage tabADI;
        private System.Windows.Forms.Label lblSphereRollIndication;
        private System.Windows.Forms.Label lblSpherePitchIndication;
        private System.Windows.Forms.Label lblRateOfTurnIndicator;
        private System.Windows.Forms.Label lblGlideslopeIndicatorVertical;
        private System.Windows.Forms.Label lblGlideslopeIndicatorHorizontal;
        private System.Windows.Forms.CheckBox chkEnableRollAndPitch;
        private System.Windows.Forms.CheckBox chkEnableGlideslope;
        private System.Windows.Forms.CheckBox chkEnableFlagsAndRot;
        private System.Windows.Forms.CheckBox chkAuxFlagVisible;
        private System.Windows.Forms.CheckBox chkLocFlagVisible;
        private System.Windows.Forms.CheckBox chkGsFlagVisible;
        private System.Windows.Forms.NumericUpDown nudRollIndication;
        private System.Windows.Forms.NumericUpDown nudSpherePitchIndication;
        private System.Windows.Forms.NumericUpDown nudRateOfTurnIndicator;
        private System.Windows.Forms.NumericUpDown nudGlideslopeIndicatorVertical;
        private System.Windows.Forms.NumericUpDown nudGlideslopeIndicatorHorizontal;
    }
}

