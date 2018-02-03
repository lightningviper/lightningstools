namespace TlkTool.UI
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTlkFile = new System.Windows.Forms.TabPage();
            this.tvTlkFileContents = new System.Windows.Forms.TreeView();
            this.tabComms = new System.Windows.Forms.TabPage();
            this.lvComms = new System.Windows.Forms.ListView();
            this.tabEvals = new System.Windows.Forms.TabPage();
            this.lvEvals = new System.Windows.Forms.ListView();
            this.tabFrags = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdStopPlayingFrag = new System.Windows.Forms.ToolStripButton();
            this.cmdPlayFrag = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lvFrags = new System.Windows.Forms.ListView();
            this.tabControl1.SuspendLayout();
            this.tabTlkFile.SuspendLayout();
            this.tabComms.SuspendLayout();
            this.tabEvals.SuspendLayout();
            this.tabFrags.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTlkFile);
            this.tabControl1.Controls.Add(this.tabComms);
            this.tabControl1.Controls.Add(this.tabEvals);
            this.tabControl1.Controls.Add(this.tabFrags);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 53);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(895, 544);
            this.tabControl1.TabIndex = 0;
            // 
            // tabTlkFile
            // 
            this.tabTlkFile.Controls.Add(this.tvTlkFileContents);
            this.tabTlkFile.Location = new System.Drawing.Point(4, 25);
            this.tabTlkFile.Margin = new System.Windows.Forms.Padding(4);
            this.tabTlkFile.Name = "tabTlkFile";
            this.tabTlkFile.Padding = new System.Windows.Forms.Padding(4);
            this.tabTlkFile.Size = new System.Drawing.Size(887, 515);
            this.tabTlkFile.TabIndex = 0;
            this.tabTlkFile.Text = "TLK File";
            this.tabTlkFile.UseVisualStyleBackColor = true;
            // 
            // tvTlkFileContents
            // 
            this.tvTlkFileContents.AllowDrop = true;
            this.tvTlkFileContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTlkFileContents.Location = new System.Drawing.Point(4, 4);
            this.tvTlkFileContents.Margin = new System.Windows.Forms.Padding(4);
            this.tvTlkFileContents.Name = "tvTlkFileContents";
            this.tvTlkFileContents.ShowPlusMinus = false;
            this.tvTlkFileContents.Size = new System.Drawing.Size(879, 507);
            this.tvTlkFileContents.TabIndex = 0;
            this.tvTlkFileContents.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvTlkFileContents_DragDrop);
            this.tvTlkFileContents.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvTlkFileContents_DragEnter);
            // 
            // tabComms
            // 
            this.tabComms.Controls.Add(this.lvComms);
            this.tabComms.Location = new System.Drawing.Point(4, 25);
            this.tabComms.Margin = new System.Windows.Forms.Padding(4);
            this.tabComms.Name = "tabComms";
            this.tabComms.Padding = new System.Windows.Forms.Padding(4);
            this.tabComms.Size = new System.Drawing.Size(887, 515);
            this.tabComms.TabIndex = 1;
            this.tabComms.Text = "Comms";
            this.tabComms.UseVisualStyleBackColor = true;
            // 
            // lvComms
            // 
            this.lvComms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvComms.Location = new System.Drawing.Point(4, 4);
            this.lvComms.MultiSelect = false;
            this.lvComms.Name = "lvComms";
            this.lvComms.Size = new System.Drawing.Size(879, 507);
            this.lvComms.TabIndex = 0;
            this.lvComms.UseCompatibleStateImageBehavior = false;
            this.lvComms.View = System.Windows.Forms.View.Details;
            // 
            // tabEvals
            // 
            this.tabEvals.Controls.Add(this.lvEvals);
            this.tabEvals.Location = new System.Drawing.Point(4, 25);
            this.tabEvals.Margin = new System.Windows.Forms.Padding(4);
            this.tabEvals.Name = "tabEvals";
            this.tabEvals.Size = new System.Drawing.Size(887, 515);
            this.tabEvals.TabIndex = 2;
            this.tabEvals.Text = "Evals";
            this.tabEvals.UseVisualStyleBackColor = true;
            // 
            // lvEvals
            // 
            this.lvEvals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEvals.Location = new System.Drawing.Point(0, 0);
            this.lvEvals.MultiSelect = false;
            this.lvEvals.Name = "lvEvals";
            this.lvEvals.Size = new System.Drawing.Size(887, 515);
            this.lvEvals.TabIndex = 0;
            this.lvEvals.UseCompatibleStateImageBehavior = false;
            this.lvEvals.View = System.Windows.Forms.View.Details;
            // 
            // tabFrags
            // 
            this.tabFrags.Controls.Add(this.lvFrags);
            this.tabFrags.Location = new System.Drawing.Point(4, 25);
            this.tabFrags.Margin = new System.Windows.Forms.Padding(4);
            this.tabFrags.Name = "tabFrags";
            this.tabFrags.Size = new System.Drawing.Size(887, 515);
            this.tabFrags.TabIndex = 3;
            this.tabFrags.Text = "Frags";
            this.tabFrags.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.cmdStopPlayingFrag,
            this.cmdPlayFrag});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(895, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "&New";
            this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // cmdStopPlayingFrag
            // 
            this.cmdStopPlayingFrag.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdStopPlayingFrag.Image = global::TlkTool.UI.Properties.Resources.stop;
            this.cmdStopPlayingFrag.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdStopPlayingFrag.Name = "cmdStopPlayingFrag";
            this.cmdStopPlayingFrag.Size = new System.Drawing.Size(23, 22);
            this.cmdStopPlayingFrag.Text = "Stop";
            this.cmdStopPlayingFrag.Click += new System.EventHandler(this.cmdStopPlayingFrag_Click);
            // 
            // cmdPlayFrag
            // 
            this.cmdPlayFrag.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdPlayFrag.Image = global::TlkTool.UI.Properties.Resources.start;
            this.cmdPlayFrag.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdPlayFrag.Name = "cmdPlayFrag";
            this.cmdPlayFrag.Size = new System.Drawing.Size(23, 22);
            this.cmdPlayFrag.Text = "Play";
            this.cmdPlayFrag.Click += new System.EventHandler(this.cmdPlayFrag_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(895, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(173, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(139, 24);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Location = new System.Drawing.Point(0, 597);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(895, 22);
            this.statusStrip1.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lvFrags
            // 
            this.lvFrags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFrags.Location = new System.Drawing.Point(0, 0);
            this.lvFrags.Name = "lvFrags";
            this.lvFrags.Size = new System.Drawing.Size(887, 515);
            this.lvFrags.TabIndex = 0;
            this.lvFrags.UseCompatibleStateImageBehavior = false;
            this.lvFrags.View = System.Windows.Forms.View.Details;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 619);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Falcon AI Comms Editing Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabTlkFile.ResumeLayout(false);
            this.tabComms.ResumeLayout(false);
            this.tabEvals.ResumeLayout(false);
            this.tabFrags.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTlkFile;
        private System.Windows.Forms.TabPage tabComms;
        private System.Windows.Forms.TabPage tabEvals;
        private System.Windows.Forms.TabPage tabFrags;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TreeView tvTlkFileContents;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cmdPlayFrag;
        private System.Windows.Forms.ToolStripButton cmdStopPlayingFrag;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView lvComms;
        private System.Windows.Forms.ListView lvEvals;
        private System.Windows.Forms.ListView lvFrags;

    }
}

