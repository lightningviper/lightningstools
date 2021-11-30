namespace DTSCardTestTool
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.gbSetSerialNum = new System.Windows.Forms.GroupBox();
            this.lblSerial = new System.Windows.Forms.Label();
            this.btnSetSerial = new System.Windows.Forms.Button();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.gbSetAngle = new System.Windows.Forms.GroupBox();
            this.nudAngle = new System.Windows.Forms.NumericUpDown();
            this.lblAngle = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.mainPanel.SuspendLayout();
            this.gbSetSerialNum.SuspendLayout();
            this.gbSetAngle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.gbSetSerialNum);
            this.mainPanel.Controls.Add(this.gbSetAngle);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(6);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(889, 319);
            this.mainPanel.TabIndex = 1;
            // 
            // gbSetSerialNum
            // 
            this.gbSetSerialNum.Controls.Add(this.lblSerial);
            this.gbSetSerialNum.Controls.Add(this.btnSetSerial);
            this.gbSetSerialNum.Controls.Add(this.txtSerial);
            this.gbSetSerialNum.Location = new System.Drawing.Point(36, 32);
            this.gbSetSerialNum.Name = "gbSetSerialNum";
            this.gbSetSerialNum.Size = new System.Drawing.Size(821, 104);
            this.gbSetSerialNum.TabIndex = 12;
            this.gbSetSerialNum.TabStop = false;
            this.gbSetSerialNum.Text = "Digital to Synchro Card";
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.Location = new System.Drawing.Point(17, 45);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(91, 25);
            this.lblSerial.TabIndex = 2;
            this.lblSerial.Text = "Serial #:";
            // 
            // btnSetSerial
            // 
            this.btnSetSerial.Location = new System.Drawing.Point(479, 37);
            this.btnSetSerial.Name = "btnSetSerial";
            this.btnSetSerial.Size = new System.Drawing.Size(326, 41);
            this.btnSetSerial.TabIndex = 0;
            this.btnSetSerial.Text = "Set &Serial and Initialize";
            this.btnSetSerial.UseVisualStyleBackColor = true;
            this.btnSetSerial.Click += new System.EventHandler(this.btnSetSerial_Click);
            // 
            // txtSerial
            // 
            this.txtSerial.Location = new System.Drawing.Point(108, 39);
            this.txtSerial.MaxLength = 8;
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(355, 31);
            this.txtSerial.TabIndex = 1;
            // 
            // gbSetAngle
            // 
            this.gbSetAngle.Controls.Add(this.nudAngle);
            this.gbSetAngle.Controls.Add(this.lblAngle);
            this.gbSetAngle.Enabled = false;
            this.gbSetAngle.Location = new System.Drawing.Point(36, 172);
            this.gbSetAngle.Name = "gbSetAngle";
            this.gbSetAngle.Size = new System.Drawing.Size(624, 104);
            this.gbSetAngle.TabIndex = 11;
            this.gbSetAngle.TabStop = false;
            this.gbSetAngle.Text = "Set Angle";
            // 
            // nudAngle
            // 
            this.nudAngle.DecimalPlaces = 2;
            this.nudAngle.Location = new System.Drawing.Point(118, 45);
            this.nudAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudAngle.Name = "nudAngle";
            this.nudAngle.Size = new System.Drawing.Size(120, 31);
            this.nudAngle.TabIndex = 13;
            this.nudAngle.ValueChanged += new System.EventHandler(this.nudAngle_ValueChanged);
            // 
            // lblAngle
            // 
            this.lblAngle.AutoSize = true;
            this.lblAngle.Location = new System.Drawing.Point(39, 47);
            this.lblAngle.Name = "lblAngle";
            this.lblAngle.Size = new System.Drawing.Size(73, 25);
            this.lblAngle.TabIndex = 10;
            this.lblAngle.Text = "Angle:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 319);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MainForm";
            this.Text = "DTS Card Test Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainPanel.ResumeLayout(false);
            this.gbSetSerialNum.ResumeLayout(false);
            this.gbSetSerialNum.PerformLayout();
            this.gbSetAngle.ResumeLayout(false);
            this.gbSetAngle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Button btnSetSerial;
        private System.Windows.Forms.GroupBox gbSetAngle;
        private System.Windows.Forms.Label lblAngle;
        private System.Windows.Forms.GroupBox gbSetSerialNum;
        private System.Windows.Forms.NumericUpDown nudAngle;
    }
}

