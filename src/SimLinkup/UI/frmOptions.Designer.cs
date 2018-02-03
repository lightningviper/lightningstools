using System.ComponentModel;
using System.Windows.Forms;
using SimLinkup.UI.UserControls;

namespace SimLinkup.UI
{
    partial class frmOptions
    {

        /// Required designer variable.
        /// </summary>
        private IContainer components = null;


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


        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStartup = new System.Windows.Forms.TabPage();
            this.gbStartupOptions = new System.Windows.Forms.GroupBox();
            this.chkMinimizeWhenStarted = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
            this.chkStartAutomaticallyWhenLaunched = new System.Windows.Forms.CheckBox();
            this.chkLaunchAtSystemStartup = new System.Windows.Forms.CheckBox();
            this.tabPlugins = new System.Windows.Forms.TabPage();
            this.tabPluginsSubtabs = new System.Windows.Forms.TabControl();
            this.tabHardwareSupport = new System.Windows.Forms.TabPage();
            this.hardwareSupportModuleList = new SimLinkup.UI.UserControls.ModuleList();
            this.tabSimSupport = new System.Windows.Forms.TabPage();
            this.simSupportModuleList = new SimLinkup.UI.UserControls.ModuleList();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabStartup.SuspendLayout();
            this.gbStartupOptions.SuspendLayout();
            this.tabPlugins.SuspendLayout();
            this.tabPluginsSubtabs.SuspendLayout();
            this.tabHardwareSupport.SuspendLayout();
            this.tabSimSupport.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(16, 2);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(100, 28);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(124, 4);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(100, 28);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cmdOK);
            this.splitContainer1.Panel2.Controls.Add(this.cmdCancel);
            this.splitContainer1.Size = new System.Drawing.Size(765, 348);
            this.splitContainer1.SplitterDistance = 307;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPlugins);
            this.tabControl1.Controls.Add(this.tabStartup);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(765, 307);
            this.tabControl1.TabIndex = 0;
            // 
            // tabStartup
            // 
            this.tabStartup.AutoScroll = true;
            this.tabStartup.Controls.Add(this.gbStartupOptions);
            this.tabStartup.Location = new System.Drawing.Point(4, 25);
            this.tabStartup.Margin = new System.Windows.Forms.Padding(4);
            this.tabStartup.Name = "tabStartup";
            this.tabStartup.Padding = new System.Windows.Forms.Padding(4);
            this.tabStartup.Size = new System.Drawing.Size(757, 278);
            this.tabStartup.TabIndex = 1;
            this.tabStartup.Text = "Startup";
            this.tabStartup.UseVisualStyleBackColor = true;
            // 
            // gbStartupOptions
            // 
            this.gbStartupOptions.Controls.Add(this.chkMinimizeWhenStarted);
            this.gbStartupOptions.Controls.Add(this.chkMinimizeToSystemTray);
            this.gbStartupOptions.Controls.Add(this.chkStartAutomaticallyWhenLaunched);
            this.gbStartupOptions.Controls.Add(this.chkLaunchAtSystemStartup);
            this.gbStartupOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbStartupOptions.Location = new System.Drawing.Point(4, 4);
            this.gbStartupOptions.Margin = new System.Windows.Forms.Padding(4);
            this.gbStartupOptions.Name = "gbStartupOptions";
            this.gbStartupOptions.Padding = new System.Windows.Forms.Padding(4);
            this.gbStartupOptions.Size = new System.Drawing.Size(749, 270);
            this.gbStartupOptions.TabIndex = 5;
            this.gbStartupOptions.TabStop = false;
            this.gbStartupOptions.Text = "Startup Options";
            // 
            // chkMinimizeWhenStarted
            // 
            this.chkMinimizeWhenStarted.AutoSize = true;
            this.chkMinimizeWhenStarted.Location = new System.Drawing.Point(8, 108);
            this.chkMinimizeWhenStarted.Margin = new System.Windows.Forms.Padding(4);
            this.chkMinimizeWhenStarted.Name = "chkMinimizeWhenStarted";
            this.chkMinimizeWhenStarted.Size = new System.Drawing.Size(169, 21);
            this.chkMinimizeWhenStarted.TabIndex = 3;
            this.chkMinimizeWhenStarted.Text = "&Minimize when started";
            this.chkMinimizeWhenStarted.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToSystemTray
            // 
            this.chkMinimizeToSystemTray.AutoSize = true;
            this.chkMinimizeToSystemTray.Location = new System.Drawing.Point(8, 80);
            this.chkMinimizeToSystemTray.Margin = new System.Windows.Forms.Padding(4);
            this.chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
            this.chkMinimizeToSystemTray.Size = new System.Drawing.Size(183, 21);
            this.chkMinimizeToSystemTray.TabIndex = 2;
            this.chkMinimizeToSystemTray.Text = "&Minimize to System Tray";
            this.chkMinimizeToSystemTray.UseVisualStyleBackColor = true;
            // 
            // chkStartAutomaticallyWhenLaunched
            // 
            this.chkStartAutomaticallyWhenLaunched.AutoSize = true;
            this.chkStartAutomaticallyWhenLaunched.Location = new System.Drawing.Point(8, 52);
            this.chkStartAutomaticallyWhenLaunched.Margin = new System.Windows.Forms.Padding(4);
            this.chkStartAutomaticallyWhenLaunched.Name = "chkStartAutomaticallyWhenLaunched";
            this.chkStartAutomaticallyWhenLaunched.Size = new System.Drawing.Size(245, 21);
            this.chkStartAutomaticallyWhenLaunched.TabIndex = 1;
            this.chkStartAutomaticallyWhenLaunched.Text = "&Start automatically when launched";
            this.chkStartAutomaticallyWhenLaunched.UseVisualStyleBackColor = true;
            // 
            // chkLaunchAtSystemStartup
            // 
            this.chkLaunchAtSystemStartup.AutoSize = true;
            this.chkLaunchAtSystemStartup.Location = new System.Drawing.Point(8, 23);
            this.chkLaunchAtSystemStartup.Margin = new System.Windows.Forms.Padding(4);
            this.chkLaunchAtSystemStartup.Name = "chkLaunchAtSystemStartup";
            this.chkLaunchAtSystemStartup.Size = new System.Drawing.Size(193, 21);
            this.chkLaunchAtSystemStartup.TabIndex = 0;
            this.chkLaunchAtSystemStartup.Text = "&Launch at System Startup";
            this.chkLaunchAtSystemStartup.UseVisualStyleBackColor = true;
            // 
            // tabPlugins
            // 
            this.tabPlugins.Controls.Add(this.tabPluginsSubtabs);
            this.tabPlugins.Location = new System.Drawing.Point(4, 25);
            this.tabPlugins.Margin = new System.Windows.Forms.Padding(4);
            this.tabPlugins.Name = "tabPlugins";
            this.tabPlugins.Padding = new System.Windows.Forms.Padding(4);
            this.tabPlugins.Size = new System.Drawing.Size(757, 278);
            this.tabPlugins.TabIndex = 2;
            this.tabPlugins.Text = "Plug-ins";
            this.tabPlugins.UseVisualStyleBackColor = true;
            // 
            // tabPluginsSubtabs
            // 
            this.tabPluginsSubtabs.Controls.Add(this.tabHardwareSupport);
            this.tabPluginsSubtabs.Controls.Add(this.tabSimSupport);
            this.tabPluginsSubtabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPluginsSubtabs.Location = new System.Drawing.Point(4, 4);
            this.tabPluginsSubtabs.Margin = new System.Windows.Forms.Padding(4);
            this.tabPluginsSubtabs.Name = "tabPluginsSubtabs";
            this.tabPluginsSubtabs.SelectedIndex = 0;
            this.tabPluginsSubtabs.Size = new System.Drawing.Size(749, 270);
            this.tabPluginsSubtabs.TabIndex = 0;
            // 
            // tabHardwareSupport
            // 
            this.tabHardwareSupport.AutoScroll = true;
            this.tabHardwareSupport.Controls.Add(this.hardwareSupportModuleList);
            this.tabHardwareSupport.Location = new System.Drawing.Point(4, 25);
            this.tabHardwareSupport.Margin = new System.Windows.Forms.Padding(4);
            this.tabHardwareSupport.Name = "tabHardwareSupport";
            this.tabHardwareSupport.Padding = new System.Windows.Forms.Padding(4);
            this.tabHardwareSupport.Size = new System.Drawing.Size(741, 241);
            this.tabHardwareSupport.TabIndex = 0;
            this.tabHardwareSupport.Text = "Hardware Support Modules";
            this.tabHardwareSupport.UseVisualStyleBackColor = true;
            // 
            // hardwareSupportModuleList
            // 
            this.hardwareSupportModuleList.AutoSize = true;
            this.hardwareSupportModuleList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hardwareSupportModuleList.HardwareSupportModules = null;
            this.hardwareSupportModuleList.Location = new System.Drawing.Point(4, 4);
            this.hardwareSupportModuleList.Name = "hardwareSupportModuleList";
            this.hardwareSupportModuleList.SimSupportModules = null;
            this.hardwareSupportModuleList.Size = new System.Drawing.Size(733, 233);
            this.hardwareSupportModuleList.TabIndex = 0;
            // 
            // tabSimSupport
            // 
            this.tabSimSupport.AutoScroll = true;
            this.tabSimSupport.Controls.Add(this.simSupportModuleList);
            this.tabSimSupport.Location = new System.Drawing.Point(4, 25);
            this.tabSimSupport.Margin = new System.Windows.Forms.Padding(4);
            this.tabSimSupport.Name = "tabSimSupport";
            this.tabSimSupport.Padding = new System.Windows.Forms.Padding(4);
            this.tabSimSupport.Size = new System.Drawing.Size(741, 241);
            this.tabSimSupport.TabIndex = 1;
            this.tabSimSupport.Text = "Sim Support Modules";
            this.tabSimSupport.UseVisualStyleBackColor = true;
            // 
            // simSupportModuleList
            // 
            this.simSupportModuleList.AutoSize = true;
            this.simSupportModuleList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simSupportModuleList.HardwareSupportModules = null;
            this.simSupportModuleList.Location = new System.Drawing.Point(4, 4);
            this.simSupportModuleList.Name = "simSupportModuleList";
            this.simSupportModuleList.SimSupportModules = null;
            this.simSupportModuleList.Size = new System.Drawing.Size(733, 233);
            this.simSupportModuleList.TabIndex = 1;
            // 
            // frmOptions
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(765, 348);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabStartup.ResumeLayout(false);
            this.gbStartupOptions.ResumeLayout(false);
            this.gbStartupOptions.PerformLayout();
            this.tabPlugins.ResumeLayout(false);
            this.tabPluginsSubtabs.ResumeLayout(false);
            this.tabHardwareSupport.ResumeLayout(false);
            this.tabHardwareSupport.PerformLayout();
            this.tabSimSupport.ResumeLayout(false);
            this.tabSimSupport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button cmdOK;
        private Button cmdCancel;
        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabStartup;
        private TabPage tabPlugins;
        private TabControl tabPluginsSubtabs;
        private TabPage tabHardwareSupport;
        private TabPage tabSimSupport;
        private GroupBox gbStartupOptions;
        private CheckBox chkMinimizeWhenStarted;
        private CheckBox chkMinimizeToSystemTray;
        private CheckBox chkStartAutomaticallyWhenLaunched;
        private CheckBox chkLaunchAtSystemStartup;
        private ModuleList hardwareSupportModuleList;
        private ModuleList simSupportModuleList;
    }
}