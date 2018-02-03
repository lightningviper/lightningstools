namespace JoyMapper
{
    internal sealed partial class frmOptions
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbPerformance = new System.Windows.Forms.GroupBox();
            this.lblMilliseconds = new System.Windows.Forms.Label();
            this.nudPollingPeriod = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.grpGeneralOptions = new System.Windows.Forms.GroupBox();
            this.chkMinimizeToTrayWhenMapping = new System.Windows.Forms.CheckBox();
            this.chkAutoHighlighting = new System.Windows.Forms.CheckBox();
            this.grpAppStartupOptions = new System.Windows.Forms.GroupBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.txtDefaultMappingFile = new System.Windows.Forms.TextBox();
            this.chkStartMappingOnProgramLaunch = new System.Windows.Forms.CheckBox();
            this.chkLoadDefaultMappingFile = new System.Windows.Forms.CheckBox();
            this.grpSystemStartupOptions = new System.Windows.Forms.GroupBox();
            this.chkLaunchAtStartup = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            this.gbPerformance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollingPeriod)).BeginInit();
            this.grpGeneralOptions.SuspendLayout();
            this.grpAppStartupOptions.SuspendLayout();
            this.grpSystemStartupOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbPerformance);
            this.panel1.Controls.Add(this.grpGeneralOptions);
            this.panel1.Controls.Add(this.grpAppStartupOptions);
            this.panel1.Controls.Add(this.grpSystemStartupOptions);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 332);
            this.panel1.TabIndex = 0;
            // 
            // gbPerformance
            // 
            this.gbPerformance.Controls.Add(this.lblMilliseconds);
            this.gbPerformance.Controls.Add(this.nudPollingPeriod);
            this.gbPerformance.Controls.Add(this.label1);
            this.gbPerformance.Location = new System.Drawing.Point(12, 86);
            this.gbPerformance.Name = "gbPerformance";
            this.gbPerformance.Size = new System.Drawing.Size(461, 49);
            this.gbPerformance.TabIndex = 3;
            this.gbPerformance.TabStop = false;
            this.gbPerformance.Text = "Performance";
            // 
            // lblMilliseconds
            // 
            this.lblMilliseconds.AutoSize = true;
            this.lblMilliseconds.Location = new System.Drawing.Point(256, 21);
            this.lblMilliseconds.Name = "lblMilliseconds";
            this.lblMilliseconds.Size = new System.Drawing.Size(63, 13);
            this.lblMilliseconds.TabIndex = 2;
            this.lblMilliseconds.Text = "milliseconds";
            // 
            // nudPollingPeriod
            // 
            this.nudPollingPeriod.Location = new System.Drawing.Point(197, 19);
            this.nudPollingPeriod.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudPollingPeriod.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudPollingPeriod.Name = "nudPollingPeriod";
            this.nudPollingPeriod.Size = new System.Drawing.Size(53, 20);
            this.nudPollingPeriod.TabIndex = 1;
            this.nudPollingPeriod.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Polled devices should be polled every";
            // 
            // grpGeneralOptions
            // 
            this.grpGeneralOptions.Controls.Add(this.chkMinimizeToTrayWhenMapping);
            this.grpGeneralOptions.Controls.Add(this.chkAutoHighlighting);
            this.grpGeneralOptions.Location = new System.Drawing.Point(12, 12);
            this.grpGeneralOptions.Name = "grpGeneralOptions";
            this.grpGeneralOptions.Size = new System.Drawing.Size(461, 68);
            this.grpGeneralOptions.TabIndex = 0;
            this.grpGeneralOptions.TabStop = false;
            this.grpGeneralOptions.Text = "General";
            // 
            // chkMinimizeToTrayWhenMapping
            // 
            this.chkMinimizeToTrayWhenMapping.AutoSize = true;
            this.chkMinimizeToTrayWhenMapping.Location = new System.Drawing.Point(6, 40);
            this.chkMinimizeToTrayWhenMapping.Name = "chkMinimizeToTrayWhenMapping";
            this.chkMinimizeToTrayWhenMapping.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.chkMinimizeToTrayWhenMapping.Size = new System.Drawing.Size(268, 22);
            this.chkMinimizeToTrayWhenMapping.TabIndex = 2;
            this.chkMinimizeToTrayWhenMapping.Text = "&Minimize to the system tray when mapping is started";
            this.chkMinimizeToTrayWhenMapping.UseVisualStyleBackColor = true;
            // 
            // chkAutoHighlighting
            // 
            this.chkAutoHighlighting.AutoSize = true;
            this.chkAutoHighlighting.Location = new System.Drawing.Point(6, 19);
            this.chkAutoHighlighting.Name = "chkAutoHighlighting";
            this.chkAutoHighlighting.Size = new System.Drawing.Size(292, 17);
            this.chkAutoHighlighting.TabIndex = 1;
            this.chkAutoHighlighting.Text = "&Automatically highlight controls when their state changes";
            this.chkAutoHighlighting.UseVisualStyleBackColor = true;
            // 
            // grpAppStartupOptions
            // 
            this.grpAppStartupOptions.Controls.Add(this.cmdBrowse);
            this.grpAppStartupOptions.Controls.Add(this.txtDefaultMappingFile);
            this.grpAppStartupOptions.Controls.Add(this.chkStartMappingOnProgramLaunch);
            this.grpAppStartupOptions.Controls.Add(this.chkLoadDefaultMappingFile);
            this.grpAppStartupOptions.Location = new System.Drawing.Point(12, 141);
            this.grpAppStartupOptions.Name = "grpAppStartupOptions";
            this.grpAppStartupOptions.Size = new System.Drawing.Size(461, 90);
            this.grpAppStartupOptions.TabIndex = 6;
            this.grpAppStartupOptions.TabStop = false;
            this.grpAppStartupOptions.Text = "Application Startup";
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Enabled = false;
            this.cmdBrowse.Location = new System.Drawing.Point(6, 40);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(66, 22);
            this.cmdBrowse.TabIndex = 8;
            this.cmdBrowse.Text = "&Browse...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // txtDefaultMappingFile
            // 
            this.txtDefaultMappingFile.Enabled = false;
            this.txtDefaultMappingFile.Location = new System.Drawing.Point(78, 42);
            this.txtDefaultMappingFile.Name = "txtDefaultMappingFile";
            this.txtDefaultMappingFile.ReadOnly = true;
            this.txtDefaultMappingFile.Size = new System.Drawing.Size(354, 20);
            this.txtDefaultMappingFile.TabIndex = 9;
            // 
            // chkStartMappingOnProgramLaunch
            // 
            this.chkStartMappingOnProgramLaunch.AutoSize = true;
            this.chkStartMappingOnProgramLaunch.Enabled = false;
            this.chkStartMappingOnProgramLaunch.Location = new System.Drawing.Point(78, 68);
            this.chkStartMappingOnProgramLaunch.Name = "chkStartMappingOnProgramLaunch";
            this.chkStartMappingOnProgramLaunch.Size = new System.Drawing.Size(296, 17);
            this.chkStartMappingOnProgramLaunch.TabIndex = 10;
            this.chkStartMappingOnProgramLaunch.Text = "&Start mapping automatically when JoyMapper is launched";
            this.chkStartMappingOnProgramLaunch.UseVisualStyleBackColor = true;
            // 
            // chkLoadDefaultMappingFile
            // 
            this.chkLoadDefaultMappingFile.AutoSize = true;
            this.chkLoadDefaultMappingFile.Location = new System.Drawing.Point(6, 19);
            this.chkLoadDefaultMappingFile.Name = "chkLoadDefaultMappingFile";
            this.chkLoadDefaultMappingFile.Size = new System.Drawing.Size(250, 17);
            this.chkLoadDefaultMappingFile.TabIndex = 7;
            this.chkLoadDefaultMappingFile.Text = "&Load mapping file when JoyMapper is launched";
            this.chkLoadDefaultMappingFile.UseVisualStyleBackColor = true;
            this.chkLoadDefaultMappingFile.CheckedChanged += new System.EventHandler(this.chkLoadDefaultMappingFile_CheckedChanged);
            // 
            // grpSystemStartupOptions
            // 
            this.grpSystemStartupOptions.Controls.Add(this.chkLaunchAtStartup);
            this.grpSystemStartupOptions.Location = new System.Drawing.Point(12, 237);
            this.grpSystemStartupOptions.Name = "grpSystemStartupOptions";
            this.grpSystemStartupOptions.Size = new System.Drawing.Size(461, 44);
            this.grpSystemStartupOptions.TabIndex = 11;
            this.grpSystemStartupOptions.TabStop = false;
            this.grpSystemStartupOptions.Text = "System Startup";
            // 
            // chkLaunchAtStartup
            // 
            this.chkLaunchAtStartup.AutoSize = true;
            this.chkLaunchAtStartup.Location = new System.Drawing.Point(6, 19);
            this.chkLaunchAtStartup.Name = "chkLaunchAtStartup";
            this.chkLaunchAtStartup.Size = new System.Drawing.Size(199, 17);
            this.chkLaunchAtStartup.TabIndex = 12;
            this.chkLaunchAtStartup.Text = "Launch JoyMapper at system startup";
            this.chkLaunchAtStartup.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.Controls.Add(this.cmdOk);
            this.flowLayoutPanel1.Controls.Add(this.cmdCancel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 296);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(461, 33);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(3, 3);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(69, 25);
            this.cmdOk.TabIndex = 10;
            this.cmdOk.Text = "&OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(78, 3);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(69, 25);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmOptions
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(484, 332);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOptions_FormClosing);
            this.panel1.ResumeLayout(false);
            this.gbPerformance.ResumeLayout(false);
            this.gbPerformance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPollingPeriod)).EndInit();
            this.grpGeneralOptions.ResumeLayout(false);
            this.grpGeneralOptions.PerformLayout();
            this.grpAppStartupOptions.ResumeLayout(false);
            this.grpAppStartupOptions.PerformLayout();
            this.grpSystemStartupOptions.ResumeLayout(false);
            this.grpSystemStartupOptions.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.TextBox txtDefaultMappingFile;
        private System.Windows.Forms.CheckBox chkStartMappingOnProgramLaunch;
        private System.Windows.Forms.GroupBox grpSystemStartupOptions;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckBox chkLoadDefaultMappingFile;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.GroupBox grpGeneralOptions;
        private System.Windows.Forms.CheckBox chkAutoHighlighting;
        private System.Windows.Forms.CheckBox chkMinimizeToTrayWhenMapping;
        private System.Windows.Forms.CheckBox chkLaunchAtStartup;
        private System.Windows.Forms.GroupBox grpAppStartupOptions;
        private System.Windows.Forms.GroupBox gbPerformance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMilliseconds;
        private System.Windows.Forms.NumericUpDown nudPollingPeriod;
    }
}