namespace MFDExtractor.UI
{
    partial class InstrumentForm
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
            if (disposing)
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstrumentForm));
            this.ctx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxStretchToFill = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxRotation = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotationNoRotationNoFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotatePlus90Degrees = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotateMinus90Degrees = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotate180Degrees = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxFlipHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxFlipVertically = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotatePlus90DegreesFlipHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRotatePlus90DegreesFlipVertically = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxAlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMonochrome = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMakeSquare = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxHide = new System.Windows.Forms.ToolStripMenuItem();
            this.ctx.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctx
            // 
            this.ctx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxStretchToFill,
            this.ctxSeparator1,
            this.ctxRotation,
            this.ctxSeparator2,
            this.ctxAlwaysOnTop,
            this.ctxSeparator3,
            this.ctxMonochrome,
            this.ctxSeparator4,
            this.ctxMakeSquare,
            this.ctxSeparator5,
            this.ctxHide});
            this.ctx.Name = "ctx";
            this.ctx.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ctx.ShowCheckMargin = true;
            this.ctx.ShowImageMargin = false;
            this.ctx.Size = new System.Drawing.Size(167, 166);
            // 
            // ctxStretchToFill
            // 
            this.ctxStretchToFill.Name = "ctxStretchToFill";
            this.ctxStretchToFill.Size = new System.Drawing.Size(166, 22);
            this.ctxStretchToFill.Text = "Stretch to Fill";
            this.ctxStretchToFill.Click += new System.EventHandler(this.stretchToFillToolStripMenuItem_Click);
            // 
            // ctxSeparator1
            // 
            this.ctxSeparator1.Name = "ctxSeparator1";
            this.ctxSeparator1.Size = new System.Drawing.Size(163, 6);
            // 
            // ctxRotation
            // 
            this.ctxRotation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxRotationNoRotationNoFlip,
            this.ctxRotatePlus90Degrees,
            this.ctxRotateMinus90Degrees,
            this.ctxRotate180Degrees,
            this.ctxFlipHorizontally,
            this.ctxFlipVertically,
            this.ctxRotatePlus90DegreesFlipHorizontally,
            this.ctxRotatePlus90DegreesFlipVertically});
            this.ctxRotation.Name = "ctxRotation";
            this.ctxRotation.Size = new System.Drawing.Size(166, 22);
            this.ctxRotation.Text = "Rotation/Flipping";
            // 
            // ctxRotationNoRotationNoFlip
            // 
            this.ctxRotationNoRotationNoFlip.Checked = true;
            this.ctxRotationNoRotationNoFlip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotationNoRotationNoFlip.Name = "ctxRotationNoRotationNoFlip";
            this.ctxRotationNoRotationNoFlip.Size = new System.Drawing.Size(262, 22);
            this.ctxRotationNoRotationNoFlip.Text = "No rotation/No flipping";
            this.ctxRotationNoRotationNoFlip.Click += new System.EventHandler(this.ctxRotationNoRotationNoFlip_Click);
            // 
            // ctxRotatePlus90Degrees
            // 
            this.ctxRotatePlus90Degrees.Checked = true;
            this.ctxRotatePlus90Degrees.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotatePlus90Degrees.Name = "ctxRotatePlus90Degrees";
            this.ctxRotatePlus90Degrees.Size = new System.Drawing.Size(262, 22);
            this.ctxRotatePlus90Degrees.Text = "Rotate +90 degrees";
            this.ctxRotatePlus90Degrees.Click += new System.EventHandler(this.ctxRotatePlus90Degrees_Click);
            // 
            // ctxRotateMinus90Degrees
            // 
            this.ctxRotateMinus90Degrees.Checked = true;
            this.ctxRotateMinus90Degrees.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotateMinus90Degrees.Name = "ctxRotateMinus90Degrees";
            this.ctxRotateMinus90Degrees.Size = new System.Drawing.Size(262, 22);
            this.ctxRotateMinus90Degrees.Text = "Rotate -90 degrees ";
            this.ctxRotateMinus90Degrees.Click += new System.EventHandler(this.ctxRotateMinus90Degrees_Click);
            // 
            // ctxRotate180Degrees
            // 
            this.ctxRotate180Degrees.Checked = true;
            this.ctxRotate180Degrees.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotate180Degrees.Name = "ctxRotate180Degrees";
            this.ctxRotate180Degrees.Size = new System.Drawing.Size(262, 22);
            this.ctxRotate180Degrees.Text = "Rotate 180 degrees";
            this.ctxRotate180Degrees.Click += new System.EventHandler(this.ctxRotate180Degrees_Click);
            // 
            // ctxFlipHorizontally
            // 
            this.ctxFlipHorizontally.Checked = true;
            this.ctxFlipHorizontally.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxFlipHorizontally.Name = "ctxFlipHorizontally";
            this.ctxFlipHorizontally.Size = new System.Drawing.Size(262, 22);
            this.ctxFlipHorizontally.Text = "Flip horizontally";
            this.ctxFlipHorizontally.Click += new System.EventHandler(this.ctxFlipHorizontally_Click);
            // 
            // ctxFlipVertically
            // 
            this.ctxFlipVertically.Checked = true;
            this.ctxFlipVertically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxFlipVertically.Name = "ctxFlipVertically";
            this.ctxFlipVertically.Size = new System.Drawing.Size(262, 22);
            this.ctxFlipVertically.Text = "Flip vertically";
            this.ctxFlipVertically.Click += new System.EventHandler(this.ctxFlipVertically_Click);
            // 
            // ctxRotatePlus90DegreesFlipHorizontally
            // 
            this.ctxRotatePlus90DegreesFlipHorizontally.Checked = true;
            this.ctxRotatePlus90DegreesFlipHorizontally.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotatePlus90DegreesFlipHorizontally.Name = "ctxRotatePlus90DegreesFlipHorizontally";
            this.ctxRotatePlus90DegreesFlipHorizontally.Size = new System.Drawing.Size(262, 22);
            this.ctxRotatePlus90DegreesFlipHorizontally.Text = "Rotate +90 degrees, flip horizontally";
            this.ctxRotatePlus90DegreesFlipHorizontally.Click += new System.EventHandler(this.ctxRotatePlus90DegreesFlipHorizontally_Click);
            // 
            // ctxRotatePlus90DegreesFlipVertically
            // 
            this.ctxRotatePlus90DegreesFlipVertically.Checked = true;
            this.ctxRotatePlus90DegreesFlipVertically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ctxRotatePlus90DegreesFlipVertically.Name = "ctxRotatePlus90DegreesFlipVertically";
            this.ctxRotatePlus90DegreesFlipVertically.Size = new System.Drawing.Size(262, 22);
            this.ctxRotatePlus90DegreesFlipVertically.Text = "Rotate +90 degrees, flip vertically";
            this.ctxRotatePlus90DegreesFlipVertically.Click += new System.EventHandler(this.ctxRotatePlus90DegreesFlipVertically_Click);
            // 
            // ctxSeparator2
            // 
            this.ctxSeparator2.Name = "ctxSeparator2";
            this.ctxSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // ctxAlwaysOnTop
            // 
            this.ctxAlwaysOnTop.Name = "ctxAlwaysOnTop";
            this.ctxAlwaysOnTop.Size = new System.Drawing.Size(166, 22);
            this.ctxAlwaysOnTop.Text = "Always On Top";
            this.ctxAlwaysOnTop.Click += new System.EventHandler(this.ctxAlwaysOnTop_Click);
            // 
            // ctxSeparator3
            // 
            this.ctxSeparator3.Name = "ctxSeparator3";
            this.ctxSeparator3.Size = new System.Drawing.Size(163, 6);
            // 
            // ctxMonochrome
            // 
            this.ctxMonochrome.Name = "ctxMonochrome";
            this.ctxMonochrome.Size = new System.Drawing.Size(166, 22);
            this.ctxMonochrome.Text = "&Monochrome";
            this.ctxMonochrome.Click += new System.EventHandler(this.ctxMonochrome_Click);
            // 
            // ctxSeparator4
            // 
            this.ctxSeparator4.Name = "ctxSeparator4";
            this.ctxSeparator4.Size = new System.Drawing.Size(163, 6);
            // 
            // ctxMakeSquare
            // 
            this.ctxMakeSquare.Name = "ctxMakeSquare";
            this.ctxMakeSquare.Size = new System.Drawing.Size(166, 22);
            this.ctxMakeSquare.Text = "&Make Square";
            this.ctxMakeSquare.Click += new System.EventHandler(this.ctxMakeSquare_Click);
            // 
            // ctxSeparator5
            // 
            this.ctxSeparator5.Name = "ctxSeparator5";
            this.ctxSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // ctxHide
            // 
            this.ctxHide.Name = "ctxHide";
            this.ctxHide.Size = new System.Drawing.Size(166, 22);
            this.ctxHide.Text = "Hide";
            this.ctxHide.Click += new System.EventHandler(this.ctxHide_Click);
            // 
            // InstrumentForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.Black;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.ContextMenuStrip = this.ctx;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "InstrumentForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "";
            this.TransparencyKey = System.Drawing.Color.Firebrick;
            this.ctx.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ctx;
        private System.Windows.Forms.ToolStripMenuItem ctxStretchToFill;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxHide;
        private System.Windows.Forms.ToolStripMenuItem ctxRotation;
        private System.Windows.Forms.ToolStripMenuItem ctxRotationNoRotationNoFlip;
        private System.Windows.Forms.ToolStripMenuItem ctxRotatePlus90Degrees;
        private System.Windows.Forms.ToolStripMenuItem ctxRotateMinus90Degrees;
        private System.Windows.Forms.ToolStripMenuItem ctxRotate180Degrees;
        private System.Windows.Forms.ToolStripMenuItem ctxFlipHorizontally;
        private System.Windows.Forms.ToolStripMenuItem ctxFlipVertically;
        private System.Windows.Forms.ToolStripMenuItem ctxRotatePlus90DegreesFlipHorizontally;
        private System.Windows.Forms.ToolStripMenuItem ctxRotatePlus90DegreesFlipVertically;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ctxAlwaysOnTop;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ctxMonochrome;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator4;
        private System.Windows.Forms.ToolStripMenuItem ctxMakeSquare;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator5;

    }
}