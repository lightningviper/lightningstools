namespace MFDExtractor.UI
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
            if (disposing) {
                Common.Util.DisposeObject(components);
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
            this.nfyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuCtx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCtxStart = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCtxOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCtxExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtx.SuspendLayout();
            this.SuspendLayout();
            // 
            // nfyTrayIcon
            // 
            this.nfyTrayIcon.ContextMenuStrip = this.mnuCtx;
            this.nfyTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfyTrayIcon.Icon")));
            this.nfyTrayIcon.Text = "Falcon MFD Extractor";
            this.nfyTrayIcon.Visible = true;
            // 
            // mnuCtx
            // 
            this.mnuCtx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCtxStart,
            this.mnuCtxStop,
            this.toolStripMenuItem2,
            this.mnuCtxOptions,
            this.toolStripMenuItem1,
            this.mnuCtxExit});
            this.mnuCtx.Name = "mnuCtx";
            this.mnuCtx.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuCtx.Size = new System.Drawing.Size(135, 104);
            // 
            // mnuCtxStart
            // 
            this.mnuCtxStart.Image = ((System.Drawing.Image)(resources.GetObject("mnuCtxStart.Image")));
            this.mnuCtxStart.Name = "mnuCtxStart";
            this.mnuCtxStart.Size = new System.Drawing.Size(134, 22);
            this.mnuCtxStart.Text = "&Start";
            this.mnuCtxStart.Click += new System.EventHandler(this.mnuCtxStart_Click);
            // 
            // mnuCtxStop
            // 
            this.mnuCtxStop.Image = ((System.Drawing.Image)(resources.GetObject("mnuCtxStop.Image")));
            this.mnuCtxStop.Name = "mnuCtxStop";
            this.mnuCtxStop.Size = new System.Drawing.Size(134, 22);
            this.mnuCtxStop.Text = "S&top";
            this.mnuCtxStop.Click += new System.EventHandler(this.mnuCtxStop_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 6);
            // 
            // mnuCtxOptions
            // 
            this.mnuCtxOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuCtxOptions.Image")));
            this.mnuCtxOptions.Name = "mnuCtxOptions";
            this.mnuCtxOptions.Size = new System.Drawing.Size(134, 22);
            this.mnuCtxOptions.Text = "&Options...";
            this.mnuCtxOptions.Click += new System.EventHandler(this.mnuCtxOptions_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 6);
            // 
            // mnuCtxExit
            // 
            this.mnuCtxExit.Name = "mnuCtxExit";
            this.mnuCtxExit.Size = new System.Drawing.Size(134, 22);
            this.mnuCtxExit.Text = "&Exit";
            this.mnuCtxExit.Click += new System.EventHandler(this.mnuCtxExit_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.mnuCtx.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.NotifyIcon nfyTrayIcon;
        private System.Windows.Forms.ContextMenuStrip mnuCtx;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxExit;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxStart;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxStop;
    }
}