using System.ComponentModel;
using System.Windows.Forms;
using SimLinkup.UI.UserControls;

namespace SimLinkup.UI
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionsStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuActionsStop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewUpdateInRealtime = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnOptions = new System.Windows.Forms.ToolStripButton();
            this.nfyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuTrayStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new System.Windows.Forms.Panel();
            this.signalsView = new SimLinkup.UI.UserControls.SignalsView();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.mnuTray.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.actionsToolStripMenuItem,
            this.mnuView,
            this.mnuTools,
            this.mnuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1635, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(44, 24);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuFileExit.Size = new System.Drawing.Size(161, 26);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuActionsStart,
            this.toolStripMenuItem2,
            this.mnuActionsStop});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // mnuActionsStart
            // 
            this.mnuActionsStart.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionsStart.Image")));
            this.mnuActionsStart.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuActionsStart.Name = "mnuActionsStart";
            this.mnuActionsStart.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuActionsStart.Size = new System.Drawing.Size(179, 26);
            this.mnuActionsStart.Text = "St&art";
            this.mnuActionsStart.Click += new System.EventHandler(this.mnuActionsStart_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuActionsStop
            // 
            this.mnuActionsStop.Enabled = false;
            this.mnuActionsStop.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionsStop.Image")));
            this.mnuActionsStop.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuActionsStop.Name = "mnuActionsStop";
            this.mnuActionsStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.mnuActionsStop.Size = new System.Drawing.Size(179, 26);
            this.mnuActionsStop.Text = "St&op";
            this.mnuActionsStop.Click += new System.EventHandler(this.mnuActionsStop_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(56, 24);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsOptions.Image")));
            this.mnuToolsOptions.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(181, 26);
            this.mnuToolsOptions.Text = "&Options...";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(53, 24);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(134, 26);
            this.mnuHelpAbout.Text = "&About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewUpdateInRealtime});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(53, 24);
            this.mnuView.Text = "&View";
            // 
            // mnuViewUpdateInRealtime
            // 
            this.mnuViewUpdateInRealtime.Checked = true;
            this.mnuViewUpdateInRealtime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuViewUpdateInRealtime.Name = "mnuViewUpdateInRealtime";
            this.mnuViewUpdateInRealtime.Size = new System.Drawing.Size(181, 26);
            this.mnuViewUpdateInRealtime.Text = "Update in &Realtime";
            this.mnuViewUpdateInRealtime.Click += new System.EventHandler(this.mnuViewUpdateInRealtime_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 804);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1635, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnStop,
            this.btnOptions});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1635, 27);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStart
            // 
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 24);
            this.btnStart.Text = "Start";
            this.btnStart.ToolTipText = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(64, 24);
            this.btnStop.Text = "Stop";
            this.btnStop.ToolTipText = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(85, 24);
            this.btnOptions.Text = "Options";
            this.btnOptions.ToolTipText = "Options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // nfyTrayIcon
            // 
            this.nfyTrayIcon.ContextMenuStrip = this.mnuTray;
            this.nfyTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfyTrayIcon.Icon")));
            this.nfyTrayIcon.Text = "Sim Linkup";
            this.nfyTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nfyTrayIcon_MouseDoubleClick);
            // 
            // mnuTray
            // 
            this.mnuTray.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTrayStart,
            this.toolStripMenuItem3,
            this.mnuTrayStop,
            this.toolStripMenuItem4,
            this.mnuTrayRestore,
            this.toolStripMenuItem6,
            this.mnuTrayOptions,
            this.toolStripMenuItem5,
            this.mnuTrayExit});
            this.mnuTray.Name = "mnuTray";
            this.mnuTray.Size = new System.Drawing.Size(180, 158);
            // 
            // mnuTrayStart
            // 
            this.mnuTrayStart.Image = ((System.Drawing.Image)(resources.GetObject("mnuTrayStart.Image")));
            this.mnuTrayStart.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuTrayStart.Name = "mnuTrayStart";
            this.mnuTrayStart.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuTrayStart.Size = new System.Drawing.Size(179, 26);
            this.mnuTrayStart.Text = "St&art";
            this.mnuTrayStart.Click += new System.EventHandler(this.mnuTrayStart_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuTrayStop
            // 
            this.mnuTrayStop.Enabled = false;
            this.mnuTrayStop.Image = ((System.Drawing.Image)(resources.GetObject("mnuTrayStop.Image")));
            this.mnuTrayStop.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuTrayStop.Name = "mnuTrayStop";
            this.mnuTrayStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.mnuTrayStop.Size = new System.Drawing.Size(179, 26);
            this.mnuTrayStop.Text = "St&op";
            this.mnuTrayStop.Click += new System.EventHandler(this.mnuTrayStop_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuTrayRestore
            // 
            this.mnuTrayRestore.Name = "mnuTrayRestore";
            this.mnuTrayRestore.Size = new System.Drawing.Size(179, 26);
            this.mnuTrayRestore.Text = "&Restore";
            this.mnuTrayRestore.Click += new System.EventHandler(this.mnuTrayRestore_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuTrayOptions
            // 
            this.mnuTrayOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuTrayOptions.Image")));
            this.mnuTrayOptions.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuTrayOptions.Name = "mnuTrayOptions";
            this.mnuTrayOptions.Size = new System.Drawing.Size(179, 26);
            this.mnuTrayOptions.Text = "&Options...";
            this.mnuTrayOptions.Click += new System.EventHandler(this.mnuTrayOptions_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuTrayExit
            // 
            this.mnuTrayExit.Name = "mnuTrayExit";
            this.mnuTrayExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuTrayExit.Size = new System.Drawing.Size(179, 26);
            this.mnuTrayExit.Text = "E&xit";
            this.mnuTrayExit.Click += new System.EventHandler(this.mnuTrayExit_Click);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.signalsView);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 55);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(1635, 749);
            this.panel.TabIndex = 3;
            // 
            // signalsView
            // 
            this.signalsView.AutoSize = true;
            this.signalsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalsView.Location = new System.Drawing.Point(0, 0);
            this.signalsView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.signalsView.Name = "signalsView";
            this.signalsView.ScriptingContext = null;
            this.signalsView.Signals = null;
            this.signalsView.Size = new System.Drawing.Size(1635, 749);
            this.signalsView.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1635, 826);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Sim Linkup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.mnuTray.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuToolsOptions;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuHelpAbout;
        private StatusStrip statusStrip1;
        private ToolStrip toolStrip1;
        private ToolStripButton btnStart;
        private ToolStripButton btnStop;
        private ToolStripButton btnOptions;
        private ToolStripMenuItem actionsToolStripMenuItem;
        private ToolStripMenuItem mnuActionsStart;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuActionsStop;
        private NotifyIcon nfyTrayIcon;
        private ContextMenuStrip mnuTray;
        private ToolStripMenuItem mnuTrayStart;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuTrayStop;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem mnuTrayOptions;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem mnuTrayExit;
        private ToolStripMenuItem mnuTrayRestore;
        private ToolStripSeparator toolStripMenuItem6;
        private Panel panel;
        private SignalsView signalsView;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem mnuViewUpdateInRealtime;
    }
}

