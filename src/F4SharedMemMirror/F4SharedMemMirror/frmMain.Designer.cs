namespace F4SharedMemMirror
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
            this.tbToolbar = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.gbGeneralOptions = new System.Windows.Forms.GroupBox();
            this.chkRunMinimized = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
            this.chkStartMirroringWhenLaunched = new System.Windows.Forms.CheckBox();
            this.chkLaunchAtSystemStartup = new System.Windows.Forms.CheckBox();
            this.gbNetworkingOptions = new System.Windows.Forms.GroupBox();
            this.txtServerPortNum = new System.Windows.Forms.TextBox();
            this.lblPortNum = new System.Windows.Forms.Label();
            this.lblServerIPAddress = new System.Windows.Forms.Label();
            this.txtServerIPAddress = new Common.UI.UserControls.IPAddressControl();
            this.rdoServerMode = new System.Windows.Forms.RadioButton();
            this.rdoClientMode = new System.Windows.Forms.RadioButton();
            this.nfyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuNfyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuNfyStartMirroring = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNfyStopMirroring = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNfyRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNfyExit = new System.Windows.Forms.ToolStripMenuItem();
            this.gbPerformanceOptions = new System.Windows.Forms.GroupBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.cbPriority = new System.Windows.Forms.ComboBox();
            this.lblPollFrequencyMillis = new System.Windows.Forms.Label();
            this.nudPollFrequency = new System.Windows.Forms.NumericUpDown();
            this.lblPollForUpdatesEvery = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tbToolbar.SuspendLayout();
            this.gbGeneralOptions.SuspendLayout();
            this.gbNetworkingOptions.SuspendLayout();
            this.mnuNfyMenu.SuspendLayout();
            this.gbPerformanceOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollFrequency)).BeginInit();
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
            this.tbToolbar.Size = new System.Drawing.Size(307, 25);
            this.tbToolbar.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(104, 22);
            this.btnStart.Text = "St&art Mirroring";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(104, 22);
            this.btnStop.Text = "St&op Mirroring";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbGeneralOptions
            // 
            this.gbGeneralOptions.Controls.Add(this.chkRunMinimized);
            this.gbGeneralOptions.Controls.Add(this.chkMinimizeToSystemTray);
            this.gbGeneralOptions.Controls.Add(this.chkStartMirroringWhenLaunched);
            this.gbGeneralOptions.Controls.Add(this.chkLaunchAtSystemStartup);
            this.gbGeneralOptions.Location = new System.Drawing.Point(16, 225);
            this.gbGeneralOptions.Name = "gbGeneralOptions";
            this.gbGeneralOptions.Size = new System.Drawing.Size(283, 115);
            this.gbGeneralOptions.TabIndex = 4;
            this.gbGeneralOptions.TabStop = false;
            this.gbGeneralOptions.Text = "General Options";
            // 
            // chkRunMinimized
            // 
            this.chkRunMinimized.AutoSize = true;
            this.chkRunMinimized.Location = new System.Drawing.Point(6, 88);
            this.chkRunMinimized.Name = "chkRunMinimized";
            this.chkRunMinimized.Size = new System.Drawing.Size(95, 17);
            this.chkRunMinimized.TabIndex = 3;
            this.chkRunMinimized.Text = "&Run Minimized";
            this.chkRunMinimized.UseVisualStyleBackColor = true;
            this.chkRunMinimized.CheckedChanged += new System.EventHandler(this.chkRunMinimized_CheckedChanged);
            // 
            // chkMinimizeToSystemTray
            // 
            this.chkMinimizeToSystemTray.AutoSize = true;
            this.chkMinimizeToSystemTray.Location = new System.Drawing.Point(6, 65);
            this.chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
            this.chkMinimizeToSystemTray.Size = new System.Drawing.Size(139, 17);
            this.chkMinimizeToSystemTray.TabIndex = 2;
            this.chkMinimizeToSystemTray.Text = "&Minimize to System Tray";
            this.chkMinimizeToSystemTray.UseVisualStyleBackColor = true;
            this.chkMinimizeToSystemTray.CheckedChanged += new System.EventHandler(this.chkMinimizeToSystemTray_CheckedChanged);
            // 
            // chkStartMirroringWhenLaunched
            // 
            this.chkStartMirroringWhenLaunched.AutoSize = true;
            this.chkStartMirroringWhenLaunched.Location = new System.Drawing.Point(6, 42);
            this.chkStartMirroringWhenLaunched.Name = "chkStartMirroringWhenLaunched";
            this.chkStartMirroringWhenLaunched.Size = new System.Drawing.Size(230, 17);
            this.chkStartMirroringWhenLaunched.TabIndex = 1;
            this.chkStartMirroringWhenLaunched.Text = "&Start mirroring automatically when launched";
            this.chkStartMirroringWhenLaunched.UseVisualStyleBackColor = true;
            this.chkStartMirroringWhenLaunched.CheckedChanged += new System.EventHandler(this.chkStartMirroringWhenLaunched_CheckedChanged);
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
            this.gbNetworkingOptions.Controls.Add(this.txtServerPortNum);
            this.gbNetworkingOptions.Controls.Add(this.lblPortNum);
            this.gbNetworkingOptions.Controls.Add(this.lblServerIPAddress);
            this.gbNetworkingOptions.Controls.Add(this.txtServerIPAddress);
            this.gbNetworkingOptions.Controls.Add(this.rdoServerMode);
            this.gbNetworkingOptions.Controls.Add(this.rdoClientMode);
            this.gbNetworkingOptions.Location = new System.Drawing.Point(16, 28);
            this.gbNetworkingOptions.Name = "gbNetworkingOptions";
            this.gbNetworkingOptions.Size = new System.Drawing.Size(283, 97);
            this.gbNetworkingOptions.TabIndex = 2;
            this.gbNetworkingOptions.TabStop = false;
            this.gbNetworkingOptions.Text = "Networking Options";
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
            this.rdoServerMode.Location = new System.Drawing.Point(63, 19);
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
            this.rdoClientMode.Location = new System.Drawing.Point(6, 19);
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
            this.nfyTrayIcon.Text = "Falcon 4 Shared Memory Mirror";
            this.nfyTrayIcon.DoubleClick += new System.EventHandler(this.nfyTrayIcon_DoubleClick);
            // 
            // mnuNfyMenu
            // 
            this.mnuNfyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNfyStartMirroring,
            this.mnuNfyStopMirroring,
            this.toolStripMenuItem1,
            this.mnuNfyRestore,
            this.toolStripMenuItem2,
            this.mnuNfyExit});
            this.mnuNfyMenu.Name = "contextMenuStrip1";
            this.mnuNfyMenu.Size = new System.Drawing.Size(152, 104);
            // 
            // mnuNfyStartMirroring
            // 
            this.mnuNfyStartMirroring.Image = global::F4SharedMemMirror.Properties.Resources.start;
            this.mnuNfyStartMirroring.Name = "mnuNfyStartMirroring";
            this.mnuNfyStartMirroring.Size = new System.Drawing.Size(151, 22);
            this.mnuNfyStartMirroring.Text = "St&art Mirroring";
            this.mnuNfyStartMirroring.Click += new System.EventHandler(this.mnuNfyStartMirroring_Click);
            // 
            // mnuNfyStopMirroring
            // 
            this.mnuNfyStopMirroring.Enabled = false;
            this.mnuNfyStopMirroring.Image = global::F4SharedMemMirror.Properties.Resources.stop;
            this.mnuNfyStopMirroring.Name = "mnuNfyStopMirroring";
            this.mnuNfyStopMirroring.Size = new System.Drawing.Size(151, 22);
            this.mnuNfyStopMirroring.Text = "St&op Mirroring";
            this.mnuNfyStopMirroring.Click += new System.EventHandler(this.mnuNfyStopMirroring_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(148, 6);
            // 
            // mnuNfyRestore
            // 
            this.mnuNfyRestore.Name = "mnuNfyRestore";
            this.mnuNfyRestore.Size = new System.Drawing.Size(151, 22);
            this.mnuNfyRestore.Text = "&Restore";
            this.mnuNfyRestore.Click += new System.EventHandler(this.mnuNfyRestore_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(148, 6);
            // 
            // mnuNfyExit
            // 
            this.mnuNfyExit.Name = "mnuNfyExit";
            this.mnuNfyExit.Size = new System.Drawing.Size(151, 22);
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
            this.gbPerformanceOptions.Location = new System.Drawing.Point(16, 131);
            this.gbPerformanceOptions.Name = "gbPerformanceOptions";
            this.gbPerformanceOptions.Size = new System.Drawing.Size(283, 88);
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
            this.cbPriority.Size = new System.Drawing.Size(121, 21);
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
            this.nudPollFrequency.Name = "nudPollFrequency";
            this.nudPollFrequency.Size = new System.Drawing.Size(53, 20);
            this.nudPollFrequency.TabIndex = 1;
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
            this.lblVersion.Location = new System.Drawing.Point(13, 343);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(85, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Product Version:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 367);
            this.Controls.Add(this.tbToolbar);
            this.Controls.Add(this.gbPerformanceOptions);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.gbNetworkingOptions);
            this.Controls.Add(this.gbGeneralOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Falcon 4 Shared Memory Mirror";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.tbToolbar.ResumeLayout(false);
            this.tbToolbar.PerformLayout();
            this.gbGeneralOptions.ResumeLayout(false);
            this.gbGeneralOptions.PerformLayout();
            this.gbNetworkingOptions.ResumeLayout(false);
            this.gbNetworkingOptions.PerformLayout();
            this.mnuNfyMenu.ResumeLayout(false);
            this.gbPerformanceOptions.ResumeLayout(false);
            this.gbPerformanceOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollFrequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tbToolbar;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.GroupBox gbGeneralOptions;
        private System.Windows.Forms.CheckBox chkMinimizeToSystemTray;
        private System.Windows.Forms.CheckBox chkStartMirroringWhenLaunched;
        private System.Windows.Forms.CheckBox chkLaunchAtSystemStartup;
        private System.Windows.Forms.GroupBox gbNetworkingOptions;
        private System.Windows.Forms.RadioButton rdoServerMode;
        private System.Windows.Forms.RadioButton rdoClientMode;
        private System.Windows.Forms.NotifyIcon nfyTrayIcon;
        private System.Windows.Forms.ContextMenuStrip mnuNfyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyStartMirroring;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyStopMirroring;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyExit;
        private System.Windows.Forms.ToolStripMenuItem mnuNfyRestore;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.TextBox txtServerPortNum;
        private System.Windows.Forms.Label lblPortNum;
        private System.Windows.Forms.Label lblServerIPAddress;
        private Common.UI.UserControls.IPAddressControl txtServerIPAddress;
        private System.Windows.Forms.GroupBox gbPerformanceOptions;
        private System.Windows.Forms.Label lblPollFrequencyMillis;
        private System.Windows.Forms.NumericUpDown nudPollFrequency;
        private System.Windows.Forms.Label lblPollForUpdatesEvery;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.ComboBox cbPriority;
        private System.Windows.Forms.CheckBox chkRunMinimized;
        private System.Windows.Forms.Label lblVersion;

    }
}

