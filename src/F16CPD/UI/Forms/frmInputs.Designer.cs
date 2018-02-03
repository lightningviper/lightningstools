namespace F16CPD.UI.Forms
{
    partial class frmInputs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInputs));
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.pbCpdBezel = new System.Windows.Forms.PictureBox();
            this.txtConfigureInputsInstructions = new System.Windows.Forms.TextBox();
            this.btnClearAllInputAssignments = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbCpdBezel)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(11, 423);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 0;
            this.cmdOk.Text = "&OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(93, 423);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // pbCpdBezel
            // 
            this.pbCpdBezel.Image = global::F16CPD.Properties.Resources.cpdbezel;
            this.pbCpdBezel.Location = new System.Drawing.Point(174, 3);
            this.pbCpdBezel.Name = "pbCpdBezel";
            this.pbCpdBezel.Size = new System.Drawing.Size(256, 409);
            this.pbCpdBezel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCpdBezel.TabIndex = 2;
            this.pbCpdBezel.TabStop = false;
            this.pbCpdBezel.Click += new System.EventHandler(this.pbCpdBezel_Click);
            // 
            // txtConfigureInputsInstructions
            // 
            this.txtConfigureInputsInstructions.BackColor = System.Drawing.SystemColors.Info;
            this.txtConfigureInputsInstructions.Location = new System.Drawing.Point(11, 3);
            this.txtConfigureInputsInstructions.Multiline = true;
            this.txtConfigureInputsInstructions.Name = "txtConfigureInputsInstructions";
            this.txtConfigureInputsInstructions.Size = new System.Drawing.Size(157, 409);
            this.txtConfigureInputsInstructions.TabIndex = 3;
            // 
            // btnClearAllInputAssignments
            // 
            this.btnClearAllInputAssignments.Location = new System.Drawing.Point(275, 423);
            this.btnClearAllInputAssignments.Name = "btnClearAllInputAssignments";
            this.btnClearAllInputAssignments.Size = new System.Drawing.Size(152, 23);
            this.btnClearAllInputAssignments.TabIndex = 4;
            this.btnClearAllInputAssignments.Text = "&Clear All Input Assignments";
            this.btnClearAllInputAssignments.UseVisualStyleBackColor = true;
            this.btnClearAllInputAssignments.Click += new System.EventHandler(this.btnClearAllInputAssignments_Click);
            // 
            // frmInputs
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(439, 458);
            this.ControlBox = false;
            this.Controls.Add(this.btnClearAllInputAssignments);
            this.Controls.Add(this.txtConfigureInputsInstructions);
            this.Controls.Add(this.pbCpdBezel);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInputs";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assign Inputs";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmInputs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbCpdBezel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.PictureBox pbCpdBezel;
        private System.Windows.Forms.TextBox txtConfigureInputsInstructions;
        private System.Windows.Forms.Button btnClearAllInputAssignments;
    }
}