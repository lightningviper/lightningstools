using Common.UI.UserControls;
namespace F16CPD.UI.Forms
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tbToolbar = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.gbStartupOptions = new System.Windows.Forms.GroupBox();
            this.chkStartWhenLaunched = new System.Windows.Forms.CheckBox();
            this.chkLaunchAtSystemStartup = new System.Windows.Forms.CheckBox();
            this.gbNetworkingOptions = new System.Windows.Forms.GroupBox();
            this.rdoStandaloneMode = new System.Windows.Forms.RadioButton();
            this.txtServerPortNum = new System.Windows.Forms.TextBox();
            this.lblPortNum = new System.Windows.Forms.Label();
            this.lblServerIPAddress = new System.Windows.Forms.Label();
            this.txtServerIPAddress = new Common.UI.UserControls.IPAddressControl();
            this.rdoServerMode = new System.Windows.Forms.RadioButton();
            this.rdoClientMode = new System.Windows.Forms.RadioButton();
            this.nfyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuNfyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuNfyStart = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNfyStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNfyOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNfyExit = new System.Windows.Forms.ToolStripMenuItem();
            this.gbPerformanceOptions = new System.Windows.Forms.GroupBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.cbPriority = new System.Windows.Forms.ComboBox();
            this.lblPollFrequencyMillis = new System.Windows.Forms.Label();
            this.nudPollFrequency = new System.Windows.Forms.NumericUpDown();
            this.lblPollForUpdatesEvery = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.cmdAssignInputs = new System.Windows.Forms.Button();
            this.grpPFDOptions = new System.Windows.Forms.GroupBox();
            this.gbVertVelocityOptions = new System.Windows.Forms.GroupBox();
            this.rdoVertVelocityInUnitFeet = new System.Windows.Forms.RadioButton();
            this.rdoVertVelocityInThousands = new System.Windows.Forms.RadioButton();
            this.nudCourseHeadingAdjustmentSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblCourseHeadingAdjustmentSpeed = new System.Windows.Forms.Label();
            this.cbOutputRotation = new System.Windows.Forms.ComboBox();
            this.lblRotation = new System.Windows.Forms.Label();
            this.grpNorthHeadingOptions = new System.Windows.Forms.GroupBox();
            this.rdoNorth000 = new System.Windows.Forms.RadioButton();
            this.rdoNorth360 = new System.Windows.Forms.RadioButton();
            this.cmdRescueOutputWindow = new System.Windows.Forms.Button();
            this.tbToolbar.SuspendLayout();
            this.gbStartupOptions.SuspendLayout();
            this.gbNetworkingOptions.SuspendLayout();
            this.mnuNfyMenu.SuspendLayout();
            this.gbPerformanceOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollFrequency)).BeginInit();
            this.grpPFDOptions.SuspendLayout();
            this.gbVertVelocityOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCourseHeadingAdjustmentSpeed)).BeginInit();
            this.grpNorthHeadingOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbToolbar
            // 
            this.tbToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tbToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnStop});
            this.tbToolbar.Location = new System.Drawing.Point(0, 0);
            this.tbToolbar.Name = "tbToolbar";
            this.tbToolbar.Size = new System.Drawing.Size(511, 25);
            this.tbToolbar.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(54, 22);
            this.btnStart.Text = "St&art ";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(52, 22);
            this.btnStop.Text = "St&op ";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbStartupOptions
            // 
            this.gbStartupOptions.Controls.Add(this.chkStartWhenLaunched);
            this.gbStartupOptions.Controls.Add(this.chkLaunchAtSystemStartup);
            this.gbStartupOptions.Location = new System.Drawing.Point(12, 172);
            this.gbStartupOptions.Name = "gbStartupOptions";
            this.gbStartupOptions.Size = new System.Drawing.Size(229, 73);
            this.gbStartupOptions.TabIndex = 4;
            this.gbStartupOptions.TabStop = false;
            this.gbStartupOptions.Text = "Startup Options";
            // 
            // chkStartWhenLaunched
            // 
            this.chkStartWhenLaunched.AutoSize = true;
            this.chkStartWhenLaunched.Location = new System.Drawing.Point(6, 42);
            this.chkStartWhenLaunched.Name = "chkStartWhenLaunched";
            this.chkStartWhenLaunched.Size = new System.Drawing.Size(191, 17);
            this.chkStartWhenLaunched.TabIndex = 1;
            this.chkStartWhenLaunched.Text = "&Start  automatically when launched";
            this.chkStartWhenLaunched.UseVisualStyleBackColor = true;
            this.chkStartWhenLaunched.CheckedChanged += new System.EventHandler(this.chkStartWhenLaunched_CheckedChanged);
            // 
            // chkLaunchAtSystemStartup
            // 
            this.chkLaunchAtSystemStartup.AutoSize = true;
            this.chkLaunchAtSystemStartup.Location = new System.Drawing.Point(6, 19);
            this.chkLaunchAtSystemStartup.Name = "chkLaunchAtSystemStartup";
            this.chkLaunchAtSystemStartup.Size = new System.Drawing.Size(148, 17);
            this.chkLaunchAtSystemStartup.TabIndex = 0;
            this.chkLaunchAtSystemStartup.Text = "&Launch at System Startup";
            this.chkLaunchAtSystemStartup.UseVisualStyleBackColor = true;
            this.chkLaunchAtSystemStartup.CheckedChanged += new System.EventHandler(this.chkLaunchAtSystemStartup_CheckedChanged);
            // 
            // gbNetworkingOptions
            // 
            this.gbNetworkingOptions.Controls.Add(this.rdoStandaloneMode);
            this.gbNetworkingOptions.Controls.Add(this.txtServerPortNum);
            this.gbNetworkingOptions.Controls.Add(this.lblPortNum);
            this.gbNetworkingOptions.Controls.Add(this.lblServerIPAddress);
            this.gbNetworkingOptions.Controls.Add(this.txtServerIPAddress);
            this.gbNetworkingOptions.Controls.Add(this.rdoServerMode);
            this.gbNetworkingOptions.Controls.Add(this.rdoClientMode);
            this.gbNetworkingOptions.Location = new System.Drawing.Point(12, 70);
            this.gbNetworkingOptions.Name = "gbNetworkingOptions";
            this.gbNetworkingOptions.Size = new System.Drawing.Size(229, 97);
            this.gbNetworkingOptions.TabIndex = 2;
            this.gbNetworkingOptions.TabStop = false;
            this.gbNetworkingOptions.Text = "Networking Options";
            // 
            // rdoStandaloneMode
            // 
            this.rdoStandaloneMode.AutoSize = true;
            this.rdoStandaloneMode.Location = new System.Drawing.Point(9, 19);
            this.rdoStandaloneMode.Name = "rdoStandaloneMode";
            this.rdoStandaloneMode.Size = new System.Drawing.Size(79, 17);
            this.rdoStandaloneMode.TabIndex = 6;
            this.rdoStandaloneMode.TabStop = true;
            this.rdoStandaloneMode.Text = "Stan&dalone";
            this.rdoStandaloneMode.UseVisualStyleBackColor = true;
            this.rdoStandaloneMode.CheckedChanged += new System.EventHandler(this.rdoStandaloneMode_CheckedChanged);
            // 
            // txtServerPortNum
            // 
            this.txtServerPortNum.Location = new System.Drawing.Point(112, 66);
            this.txtServerPortNum.MaxLength = 5;
            this.txtServerPortNum.Name = "txtServerPortNum";
            this.txtServerPortNum.Size = new System.Drawing.Size(87, 20);
            this.txtServerPortNum.TabIndex = 5;
            this.txtServerPortNum.Leave += new System.EventHandler(this.txtServerPortNum_Leave);
            this.txtServerPortNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerPortNum_KeyPress);
            // 
            // lblPortNum
            // 
            this.lblPortNum.AutoSize = true;
            this.lblPortNum.Location = new System.Drawing.Point(3, 69);
            this.lblPortNum.Name = "lblPortNum";
            this.lblPortNum.Size = new System.Drawing.Size(103, 13);
            this.lblPortNum.TabIndex = 4;
            this.lblPortNum.Text = "Server &Port Number:";
            // 
            // lblServerIPAddress
            // 
            this.lblServerIPAddress.AutoSize = true;
            this.lblServerIPAddress.Location = new System.Drawing.Point(3, 43);
            this.lblServerIPAddress.Name = "lblServerIPAddress";
            this.lblServerIPAddress.Size = new System.Drawing.Size(95, 13);
            this.lblServerIPAddress.TabIndex = 3;
            this.lblServerIPAddress.Text = "Server &IP Address:";
            // 
            // txtServerIPAddress
            // 
            this.txtServerIPAddress.AllowInternalTab = false;
            this.txtServerIPAddress.AutoHeight = true;
            this.txtServerIPAddress.BackColor = System.Drawing.SystemColors.Window;
            this.txtServerIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.txtServerIPAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtServerIPAddress.Location = new System.Drawing.Point(112, 42);
            this.txtServerIPAddress.MinimumSize = new System.Drawing.Size(87, 20);
            this.txtServerIPAddress.Name = "txtServerIPAddress";
            this.txtServerIPAddress.ReadOnly = false;
            this.txtServerIPAddress.Size = new System.Drawing.Size(87, 20);
            this.txtServerIPAddress.TabIndex = 2;
            this.txtServerIPAddress.Text = "...";
            this.txtServerIPAddress.Leave += new System.EventHandler(this.txtServerIPAddress_Leave);
            // 
            // rdoServerMode
            // 
            this.rdoServerMode.AutoSize = true;
            this.rdoServerMode.Location = new System.Drawing.Point(151, 19);
            this.rdoServerMode.Name = "rdoServerMode";
            this.rdoServerMode.Size = new System.Drawing.Size(56, 17);
            this.rdoServerMode.TabIndex = 1;
            this.rdoServerMode.TabStop = true;
            this.rdoServerMode.Text = "S&erver";
            this.rdoServerMode.UseVisualStyleBackColor = true;
            this.rdoServerMode.CheckedChanged += new System.EventHandler(this.rdoServerMode_CheckedChanged);
            // 
            // rdoClientMode
            // 
            this.rdoClientMode.AutoSize = true;
            this.rdoClientMode.Location = new System.Drawing.Point(94, 19);
            this.rdoClientMode.Name = "rdoClientMode";
            this.rdoClientMode.Size = new System.Drawing.Size(51, 17);
            this.rdoClientMode.TabIndex = 0;
            this.rdoClientMode.TabStop = true;
            this.rdoClientMode.Text = "&Client";
            this.rdoClientMode.UseVisualStyleBackColor = true;
            this.rdoClientMode.CheckedChanged += new System.EventHandler(this.rdoClientMode_CheckedChanged);
            // 
            // nfyTrayIcon
            // 
            this.nfyTrayIcon.ContextMenuStrip = this.mnuNfyMenu;
            this.nfyTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfyTrayIcon.Icon")));
            this.nfyTrayIcon.Text = "F-16 Center Pedestal Display";
            this.nfyTrayIcon.DoubleClick += new System.EventHandler(this.nfyTrayIcon_DoubleClick);
            // 
            // mnuNfyMenu
            // 
            this.mnuNfyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNfyStart,
            this.mnuNfyStop,
            this.toolStripMenuItem1,
            this.mnuNfyOptions,
            this.toolStripMenuItem2,
            this.mnuNfyExit});
            this.mnuNfyMenu.Name = "contextMenuStrip1";
            this.mnuNfyMenu.Size = new System.Drawing.Size(123, 104);
            // 
            // mnuNfyStart
            // 
            this.mnuNfyStart.Image = global::F16CPD.Properties.Resources.start;
            this.mnuNfyStart.Name = "mnuNfyStart";
            this.mnuNfyStart.Size = new System.Drawing.Size(122, 22);
            this.mnuNfyStart.Text = "St&art";
            this.mnuNfyStart.Click += new System.EventHandler(this.mnuNfyStart_Click);
            // 
            // mnuNfyStop
            // 
            this.mnuNfyStop.Enabled = false;
            this.mnuNfyStop.Image = global::F16CPD.Properties.Resources.stop;
            this.mnuNfyStop.Name = "mnuNfyStop";
            this.mnuNfyStop.Size = new System.Drawing.Size(122, 22);
            this.mnuNfyStop.Text = "S&top";
            this.mnuNfyStop.Click += new System.EventHandler(this.mnuNfyStop_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuNfyOptions
            // 
            this.mnuNfyOptions.Name = "mnuNfyOptions";
            this.mnuNfyOptions.Size = new System.Drawing.Size(122, 22);
            this.mnuNfyOptions.Text = "&Options";
            this.mnuNfyOptions.Click += new System.EventHandler(this.mnuNfyOptions_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuNfyExit
            // 
            this.mnuNfyExit.Name = "mnuNfyExit";
            this.mnuNfyExit.Size = new System.Drawing.Size(122, 22);
            this.mnuNfyExit.Text = "E&xit";
            this.mnuNfyExit.Click += new System.EventHandler(this.mnuNfyExit_Click);
            // 
            // gbPerformanceOptions
            // 
            this.gbPerformanceOptions.Controls.Add(this.lblPriority);
            this.gbPerformanceOptions.Controls.Add(this.cbPriority);
            this.gbPerformanceOptions.Controls.Add(this.lblPollFrequencyMillis);
            this.gbPerformanceOptions.Controls.Add(this.nudPollFrequency);
            this.gbPerformanceOptions.Controls.Add(this.lblPollForUpdatesEvery);
            this.gbPerformanceOptions.Location = new System.Drawing.Point(247, 70);
            this.gbPerformanceOptions.Name = "gbPerformanceOptions";
            this.gbPerformanceOptions.Size = new System.Drawing.Size(247, 97);
            this.gbPerformanceOptions.TabIndex = 3;
            this.gbPerformanceOptions.TabStop = false;
            this.gbPerformanceOptions.Text = "Performance Options";
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(31, 52);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(41, 13);
            this.lblPriority.TabIndex = 4;
            this.lblPriority.Text = "Priorit&y:";
            // 
            // cbPriority
            // 
            this.cbPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPriority.FormattingEnabled = true;
            this.cbPriority.Location = new System.Drawing.Point(78, 49);
            this.cbPriority.Name = "cbPriority";
            this.cbPriority.Size = new System.Drawing.Size(122, 21);
            this.cbPriority.TabIndex = 3;
            this.cbPriority.SelectedIndexChanged += new System.EventHandler(this.cbPriority_SelectedIndexChanged);
            // 
            // lblPollFrequencyMillis
            // 
            this.lblPollFrequencyMillis.AutoSize = true;
            this.lblPollFrequencyMillis.Location = new System.Drawing.Point(180, 25);
            this.lblPollFrequencyMillis.Name = "lblPollFrequencyMillis";
            this.lblPollFrequencyMillis.Size = new System.Drawing.Size(20, 13);
            this.lblPollFrequencyMillis.TabIndex = 2;
            this.lblPollFrequencyMillis.Text = "ms";
            // 
            // nudPollFrequency
            // 
            this.nudPollFrequency.Location = new System.Drawing.Point(121, 23);
            this.nudPollFrequency.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudPollFrequency.Name = "nudPollFrequency";
            this.nudPollFrequency.Size = new System.Drawing.Size(53, 20);
            this.nudPollFrequency.TabIndex = 1;
            this.nudPollFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPollFrequency.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudPollFrequency.ValueChanged += new System.EventHandler(this.nudPollFrequency_ValueChanged);
            // 
            // lblPollForUpdatesEvery
            // 
            this.lblPollForUpdatesEvery.AutoSize = true;
            this.lblPollForUpdatesEvery.Location = new System.Drawing.Point(3, 23);
            this.lblPollForUpdatesEvery.Name = "lblPollForUpdatesEvery";
            this.lblPollForUpdatesEvery.Size = new System.Drawing.Size(112, 13);
            this.lblPollForUpdatesEvery.TabIndex = 0;
            this.lblPollForUpdatesEvery.Text = "Poll for &updates every ";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(9, 338);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Version:";
            // 
            // cmdAssignInputs
            // 
            this.cmdAssignInputs.Location = new System.Drawing.Point(12, 41);
            this.cmdAssignInputs.Name = "cmdAssignInputs";
            this.cmdAssignInputs.Size = new System.Drawing.Size(109, 23);
            this.cmdAssignInputs.TabIndex = 0;
            this.cmdAssignInputs.Text = "Assign I&nputs...";
            this.cmdAssignInputs.UseVisualStyleBackColor = true;
            this.cmdAssignInputs.Click += new System.EventHandler(this.cmdAssignInputs_Click);
            // 
            // grpPFDOptions
            // 
            this.grpPFDOptions.Controls.Add(this.gbVertVelocityOptions);
            this.grpPFDOptions.Controls.Add(this.nudCourseHeadingAdjustmentSpeed);
            this.grpPFDOptions.Controls.Add(this.lblCourseHeadingAdjustmentSpeed);
            this.grpPFDOptions.Controls.Add(this.cbOutputRotation);
            this.grpPFDOptions.Controls.Add(this.lblRotation);
            this.grpPFDOptions.Controls.Add(this.grpNorthHeadingOptions);
            this.grpPFDOptions.Location = new System.Drawing.Point(247, 173);
            this.grpPFDOptions.Name = "grpPFDOptions";
            this.grpPFDOptions.Size = new System.Drawing.Size(247, 178);
            this.grpPFDOptions.TabIndex = 5;
            this.grpPFDOptions.TabStop = false;
            this.grpPFDOptions.Text = "Primary Flight Display Options";
            // 
            // gbVertVelocityOptions
            // 
            this.gbVertVelocityOptions.Controls.Add(this.rdoVertVelocityInUnitFeet);
            this.gbVertVelocityOptions.Controls.Add(this.rdoVertVelocityInThousands);
            this.gbVertVelocityOptions.Location = new System.Drawing.Point(6, 70);
            this.gbVertVelocityOptions.Name = "gbVertVelocityOptions";
            this.gbVertVelocityOptions.Size = new System.Drawing.Size(226, 43);
            this.gbVertVelocityOptions.TabIndex = 8;
            this.gbVertVelocityOptions.TabStop = false;
            this.gbVertVelocityOptions.Text = "Show Vertical Velocity Readout As";
            // 
            // rdoVertVelocityInUnitFeet
            // 
            this.rdoVertVelocityInUnitFeet.AutoSize = true;
            this.rdoVertVelocityInUnitFeet.Location = new System.Drawing.Point(77, 19);
            this.rdoVertVelocityInUnitFeet.Name = "rdoVertVelocityInUnitFeet";
            this.rdoVertVelocityInUnitFeet.Size = new System.Drawing.Size(46, 17);
            this.rdoVertVelocityInUnitFeet.TabIndex = 1;
            this.rdoVertVelocityInUnitFeet.TabStop = true;
            this.rdoVertVelocityInUnitFeet.Text = "Feet";
            this.rdoVertVelocityInUnitFeet.UseVisualStyleBackColor = true;
            this.rdoVertVelocityInUnitFeet.CheckedChanged += new System.EventHandler(this.rdoVertVelocityInUnitFeet_CheckedChanged);
            // 
            // rdoVertVelocityInThousands
            // 
            this.rdoVertVelocityInThousands.AutoSize = true;
            this.rdoVertVelocityInThousands.Location = new System.Drawing.Point(7, 19);
            this.rdoVertVelocityInThousands.Name = "rdoVertVelocityInThousands";
            this.rdoVertVelocityInThousands.Size = new System.Drawing.Size(64, 17);
            this.rdoVertVelocityInThousands.TabIndex = 0;
            this.rdoVertVelocityInThousands.TabStop = true;
            this.rdoVertVelocityInThousands.Text = "Feet/1K";
            this.rdoVertVelocityInThousands.UseVisualStyleBackColor = true;
            this.rdoVertVelocityInThousands.CheckedChanged += new System.EventHandler(this.rdoVertVelocityInThousands_CheckedChanged);
            // 
            // nudCourseHeadingAdjustmentSpeed
            // 
            this.nudCourseHeadingAdjustmentSpeed.Location = new System.Drawing.Point(194, 119);
            this.nudCourseHeadingAdjustmentSpeed.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudCourseHeadingAdjustmentSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCourseHeadingAdjustmentSpeed.Name = "nudCourseHeadingAdjustmentSpeed";
            this.nudCourseHeadingAdjustmentSpeed.Size = new System.Drawing.Size(38, 20);
            this.nudCourseHeadingAdjustmentSpeed.TabIndex = 6;
            this.nudCourseHeadingAdjustmentSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCourseHeadingAdjustmentSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCourseHeadingAdjustmentSpeed.ValueChanged += new System.EventHandler(this.nudCourseHeadingAdjustmentSpeed_ValueChanged);
            // 
            // lblCourseHeadingAdjustmentSpeed
            // 
            this.lblCourseHeadingAdjustmentSpeed.AutoSize = true;
            this.lblCourseHeadingAdjustmentSpeed.Location = new System.Drawing.Point(6, 121);
            this.lblCourseHeadingAdjustmentSpeed.Name = "lblCourseHeadingAdjustmentSpeed";
            this.lblCourseHeadingAdjustmentSpeed.Size = new System.Drawing.Size(150, 13);
            this.lblCourseHeadingAdjustmentSpeed.TabIndex = 5;
            this.lblCourseHeadingAdjustmentSpeed.Text = "Fast CRS/HDG Adjust Speed:";
            // 
            // cbOutputRotation
            // 
            this.cbOutputRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOutputRotation.FormattingEnabled = true;
            this.cbOutputRotation.Location = new System.Drawing.Point(111, 145);
            this.cbOutputRotation.Name = "cbOutputRotation";
            this.cbOutputRotation.Size = new System.Drawing.Size(121, 21);
            this.cbOutputRotation.TabIndex = 4;
            this.cbOutputRotation.SelectedIndexChanged += new System.EventHandler(this.cbOutputRotation_SelectedIndexChanged);
            // 
            // lblRotation
            // 
            this.lblRotation.AutoSize = true;
            this.lblRotation.Location = new System.Drawing.Point(6, 148);
            this.lblRotation.Name = "lblRotation";
            this.lblRotation.Size = new System.Drawing.Size(85, 13);
            this.lblRotation.TabIndex = 3;
            this.lblRotation.Text = "Output Rotation:";
            // 
            // grpNorthHeadingOptions
            // 
            this.grpNorthHeadingOptions.Controls.Add(this.rdoNorth000);
            this.grpNorthHeadingOptions.Controls.Add(this.rdoNorth360);
            this.grpNorthHeadingOptions.Location = new System.Drawing.Point(6, 19);
            this.grpNorthHeadingOptions.Name = "grpNorthHeadingOptions";
            this.grpNorthHeadingOptions.Size = new System.Drawing.Size(226, 45);
            this.grpNorthHeadingOptions.TabIndex = 2;
            this.grpNorthHeadingOptions.TabStop = false;
            this.grpNorthHeadingOptions.Text = "Display North Headings As";
            // 
            // rdoNorth000
            // 
            this.rdoNorth000.AutoSize = true;
            this.rdoNorth000.Location = new System.Drawing.Point(55, 19);
            this.rdoNorth000.Name = "rdoNorth000";
            this.rdoNorth000.Size = new System.Drawing.Size(43, 17);
            this.rdoNorth000.TabIndex = 1;
            this.rdoNorth000.TabStop = true;
            this.rdoNorth000.Text = "000";
            this.rdoNorth000.UseVisualStyleBackColor = true;
            this.rdoNorth000.CheckedChanged += new System.EventHandler(this.rdoNorth000_CheckedChanged);
            // 
            // rdoNorth360
            // 
            this.rdoNorth360.AutoSize = true;
            this.rdoNorth360.Location = new System.Drawing.Point(6, 19);
            this.rdoNorth360.Name = "rdoNorth360";
            this.rdoNorth360.Size = new System.Drawing.Size(43, 17);
            this.rdoNorth360.TabIndex = 0;
            this.rdoNorth360.TabStop = true;
            this.rdoNorth360.Text = "360";
            this.rdoNorth360.UseVisualStyleBackColor = true;
            this.rdoNorth360.CheckedChanged += new System.EventHandler(this.rdoNorth360_CheckedChanged);
            // 
            // cmdRescueOutputWindow
            // 
            this.cmdRescueOutputWindow.Location = new System.Drawing.Point(320, 41);
            this.cmdRescueOutputWindow.Name = "cmdRescueOutputWindow";
            this.cmdRescueOutputWindow.Size = new System.Drawing.Size(174, 23);
            this.cmdRescueOutputWindow.TabIndex = 6;
            this.cmdRescueOutputWindow.Text = "&Reset Output Window";
            this.cmdRescueOutputWindow.UseVisualStyleBackColor = true;
            this.cmdRescueOutputWindow.Click += new System.EventHandler(this.cmdRescueOutputWindow_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 359);
            this.Controls.Add(this.grpPFDOptions);
            this.Controls.Add(this.tbToolbar);
            this.Controls.Add(this.cmdRescueOutputWindow);
            this.Controls.Add(this.gbNetworkingOptions);
            this.Controls.Add(this.gbStartupOptions);
            this.Controls.Add(this.cmdAssignInputs);
            this.Controls.Add(this.gbPerformanceOptions);
            this.Controls.Add(this.lblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "F-16 Center Pedestal Display Configuration";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.tbToolbar.ResumeLayout(false);
            this.tbToolbar.PerformLayout();
            this.gbStartupOptions.ResumeLayout(false);
            this.gbStartupOptions.PerformLayout();
            this.gbNetworkingOptions.ResumeLayout(false);
            this.gbNetworkingOptions.PerformLayout();
            this.mnuNfyMenu.ResumeLayout(false);
            this.gbPerformanceOptions.ResumeLayout(false);
            this.gbPerformanceOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollFrequency)).EndInit();
            this.grpPFDOptions.ResumeLayout(false);
            this.grpPFDOptions.PerformLayout();
            this.gbVertVelocityOptions.ResumeLayout(false);
            this.gbVertVelocityOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCourseHeadingAdjustmentSpeed)).EndInit();
            this.grpNorthHeadingOptions.ResumeLayout(false);
            this.grpNorthHeadingOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tbToolbar;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.GroupBox gbStartupOptions;
        private System.Windows.Forms.CheckBox chkStartWhenLaunched;
        private System.Windows.Forms.CheckBox chkLaunchAtSystemStartup;
        private System.Windows.Forms.GroupBox gbNetworkingOptions;
        private System.Windows.Forms.RadioButton rdoServerMode;
        private System.Windows.Forms.RadioButton rdoClientMode;
        private System.Windows.Forms.NotifyIcon nfyTrayIcon;
        private System.Windows.Forms.ContextMenuStrip mnuNfyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyStart;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyStop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyExit;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.TextBox txtServerPortNum;
        private System.Windows.Forms.Label lblPortNum;
        private System.Windows.Forms.Label lblServerIPAddress;
        private IPAddressControl txtServerIPAddress;
        private System.Windows.Forms.GroupBox gbPerformanceOptions;
        private System.Windows.Forms.Label lblPollFrequencyMillis;
        private System.Windows.Forms.NumericUpDown nudPollFrequency;
        private System.Windows.Forms.Label lblPollForUpdatesEvery;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.ComboBox cbPriority;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.RadioButton rdoStandaloneMode;
        private System.Windows.Forms.Button cmdAssignInputs;
        private System.Windows.Forms.GroupBox grpPFDOptions;
        private System.Windows.Forms.GroupBox grpNorthHeadingOptions;
        private System.Windows.Forms.RadioButton rdoNorth000;
        private System.Windows.Forms.RadioButton rdoNorth360;
        private System.Windows.Forms.Button cmdRescueOutputWindow;
        private System.Windows.Forms.ComboBox cbOutputRotation;
        private System.Windows.Forms.Label lblRotation;
        private System.Windows.Forms.NumericUpDown nudCourseHeadingAdjustmentSpeed;
        private System.Windows.Forms.Label lblCourseHeadingAdjustmentSpeed;
        private System.Windows.Forms.GroupBox gbVertVelocityOptions;
        private System.Windows.Forms.RadioButton rdoVertVelocityInThousands;
        private System.Windows.Forms.RadioButton rdoVertVelocityInUnitFeet;

    }
}

