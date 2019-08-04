namespace BMSDrawingSharedMemTester
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
            this.pbHUD = new System.Windows.Forms.PictureBox();
            this.txtHUD = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblHUDDataSize = new System.Windows.Forms.Label();
            this.pbRWR = new System.Windows.Forms.PictureBox();
            this.pbHMS = new System.Windows.Forms.PictureBox();
            this.txtRWR = new System.Windows.Forms.TextBox();
            this.txtHMS = new System.Windows.Forms.TextBox();
            this.lblRWRDataSize = new System.Windows.Forms.Label();
            this.lblHMSDataSize = new System.Windows.Forms.Label();
            this.lblRWR = new System.Windows.Forms.Label();
            this.lblHUD = new System.Windows.Forms.Label();
            this.lblHMS = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbHUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRWR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHMS)).BeginInit();
            this.SuspendLayout();
            // 
            // pbHUD
            // 
            this.pbHUD.BackColor = System.Drawing.Color.Black;
            this.pbHUD.Location = new System.Drawing.Point(3, 0);
            this.pbHUD.Name = "pbHUD";
            this.pbHUD.Size = new System.Drawing.Size(400, 400);
            this.pbHUD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHUD.TabIndex = 0;
            this.pbHUD.TabStop = false;
            // 
            // txtHUD
            // 
            this.txtHUD.CausesValidation = false;
            this.txtHUD.Location = new System.Drawing.Point(3, 422);
            this.txtHUD.MaxLength = 10000000;
            this.txtHUD.Multiline = true;
            this.txtHUD.Name = "txtHUD";
            this.txtHUD.ReadOnly = true;
            this.txtHUD.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHUD.Size = new System.Drawing.Size(400, 400);
            this.txtHUD.TabIndex = 1;
            this.txtHUD.WordWrap = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // lblHUDDataSize
            // 
            this.lblHUDDataSize.AutoSize = true;
            this.lblHUDDataSize.Location = new System.Drawing.Point(4, 403);
            this.lblHUDDataSize.Name = "lblHUDDataSize";
            this.lblHUDDataSize.Size = new System.Drawing.Size(56, 13);
            this.lblHUDDataSize.TabIndex = 2;
            this.lblHUDDataSize.Text = "Data Size:";
            // 
            // pbRWR
            // 
            this.pbRWR.BackColor = System.Drawing.Color.Black;
            this.pbRWR.Location = new System.Drawing.Point(409, 0);
            this.pbRWR.Name = "pbRWR";
            this.pbRWR.Size = new System.Drawing.Size(400, 400);
            this.pbRWR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRWR.TabIndex = 3;
            this.pbRWR.TabStop = false;
            // 
            // pbHMS
            // 
            this.pbHMS.BackColor = System.Drawing.Color.Black;
            this.pbHMS.Location = new System.Drawing.Point(815, 0);
            this.pbHMS.Name = "pbHMS";
            this.pbHMS.Size = new System.Drawing.Size(400, 400);
            this.pbHMS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHMS.TabIndex = 4;
            this.pbHMS.TabStop = false;
            // 
            // txtRWR
            // 
            this.txtRWR.CausesValidation = false;
            this.txtRWR.Location = new System.Drawing.Point(409, 422);
            this.txtRWR.MaxLength = 10000000;
            this.txtRWR.Multiline = true;
            this.txtRWR.Name = "txtRWR";
            this.txtRWR.ReadOnly = true;
            this.txtRWR.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRWR.Size = new System.Drawing.Size(400, 400);
            this.txtRWR.TabIndex = 5;
            this.txtRWR.WordWrap = false;
            // 
            // txtHMS
            // 
            this.txtHMS.CausesValidation = false;
            this.txtHMS.Location = new System.Drawing.Point(815, 422);
            this.txtHMS.MaxLength = 10000000;
            this.txtHMS.Multiline = true;
            this.txtHMS.Name = "txtHMS";
            this.txtHMS.ReadOnly = true;
            this.txtHMS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHMS.Size = new System.Drawing.Size(400, 400);
            this.txtHMS.TabIndex = 6;
            this.txtHMS.WordWrap = false;
            // 
            // lblRWRDataSize
            // 
            this.lblRWRDataSize.AutoSize = true;
            this.lblRWRDataSize.Location = new System.Drawing.Point(409, 403);
            this.lblRWRDataSize.Name = "lblRWRDataSize";
            this.lblRWRDataSize.Size = new System.Drawing.Size(56, 13);
            this.lblRWRDataSize.TabIndex = 7;
            this.lblRWRDataSize.Text = "Data Size:";
            // 
            // lblHMSDataSize
            // 
            this.lblHMSDataSize.AutoSize = true;
            this.lblHMSDataSize.Location = new System.Drawing.Point(815, 403);
            this.lblHMSDataSize.Name = "lblHMSDataSize";
            this.lblHMSDataSize.Size = new System.Drawing.Size(56, 13);
            this.lblHMSDataSize.TabIndex = 8;
            this.lblHMSDataSize.Text = "Data Size:";
            // 
            // lblRWR
            // 
            this.lblRWR.AutoSize = true;
            this.lblRWR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRWR.Location = new System.Drawing.Point(591, 403);
            this.lblRWR.Name = "lblRWR";
            this.lblRWR.Size = new System.Drawing.Size(37, 13);
            this.lblRWR.TabIndex = 10;
            this.lblRWR.Text = "RWR";
            // 
            // lblHUD
            // 
            this.lblHUD.AutoSize = true;
            this.lblHUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHUD.Location = new System.Drawing.Point(186, 403);
            this.lblHUD.Name = "lblHUD";
            this.lblHUD.Size = new System.Drawing.Size(34, 13);
            this.lblHUD.TabIndex = 11;
            this.lblHUD.Text = "HUD";
            // 
            // lblHMS
            // 
            this.lblHMS.AutoSize = true;
            this.lblHMS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHMS.Location = new System.Drawing.Point(998, 403);
            this.lblHMS.Name = "lblHMS";
            this.lblHMS.Size = new System.Drawing.Size(34, 13);
            this.lblHMS.TabIndex = 12;
            this.lblHMS.Text = "HMS";
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1216, 823);
            this.Controls.Add(this.lblHMS);
            this.Controls.Add(this.lblHUD);
            this.Controls.Add(this.lblRWR);
            this.Controls.Add(this.lblHMSDataSize);
            this.Controls.Add(this.lblRWRDataSize);
            this.Controls.Add(this.txtHMS);
            this.Controls.Add(this.txtRWR);
            this.Controls.Add(this.pbHMS);
            this.Controls.Add(this.pbRWR);
            this.Controls.Add(this.lblHUDDataSize);
            this.Controls.Add(this.txtHUD);
            this.Controls.Add(this.pbHUD);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Falcon BMS Drawing Shared Memory Test Tool";
            ((System.ComponentModel.ISupportInitialize)(this.pbHUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRWR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHMS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbHUD;
        private System.Windows.Forms.TextBox txtHUD;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblHUDDataSize;
        private System.Windows.Forms.PictureBox pbRWR;
        private System.Windows.Forms.PictureBox pbHMS;
        private System.Windows.Forms.TextBox txtRWR;
        private System.Windows.Forms.TextBox txtHMS;
        private System.Windows.Forms.Label lblRWRDataSize;
        private System.Windows.Forms.Label lblHMSDataSize;
        private System.Windows.Forms.Label lblRWR;
        private System.Windows.Forms.Label lblHUD;
        private System.Windows.Forms.Label lblHMS;
    }
}

