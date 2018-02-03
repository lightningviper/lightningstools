namespace Phcc.DeviceManager.UI
{
    partial class frmCalibrateServo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCalibrateServo));
            this.CalibrateServoWizard = new Common.UI.Wizard.Wizard();
            this.CalibrateServoWizardPage1 = new Common.UI.Wizard.WizardPage();
            this.nudCalibrateServoOffset = new System.Windows.Forms.NumericUpDown();
            this.lblCalibrateServoOffset = new System.Windows.Forms.Label();
            this.trkCalibrateServoOffset = new System.Windows.Forms.TrackBar();
            this.CalibrateServoWizardStartPage = new Common.UI.Wizard.WizardPage();
            this.infoPage1 = new Common.UI.Wizard.InfoPage();
            this.calibrateServoWizardPage2 = new Common.UI.Wizard.WizardPage();
            this.nudCalibrateServoGain = new System.Windows.Forms.NumericUpDown();
            this.lblCalibrateServoGain = new System.Windows.Forms.Label();
            this.trkCalibrateServoGain = new System.Windows.Forms.TrackBar();
            this.wizhdrCalibrateServoPage1 = new Common.UI.Wizard.Header();
            this.header1 = new Common.UI.Wizard.Header();
            this.CalibrateServoWizard.SuspendLayout();
            this.CalibrateServoWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrateServoOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCalibrateServoOffset)).BeginInit();
            this.CalibrateServoWizardStartPage.SuspendLayout();
            this.calibrateServoWizardPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrateServoGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCalibrateServoGain)).BeginInit();
            this.SuspendLayout();
            // 
            // CalibrateServoWizard
            // 
            this.CalibrateServoWizard.Controls.Add(this.calibrateServoWizardPage2);
            this.CalibrateServoWizard.Controls.Add(this.CalibrateServoWizardPage1);
            this.CalibrateServoWizard.Controls.Add(this.CalibrateServoWizardStartPage);
            this.CalibrateServoWizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CalibrateServoWizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalibrateServoWizard.Location = new System.Drawing.Point(0, 0);
            this.CalibrateServoWizard.Name = "CalibrateServoWizard";
            this.CalibrateServoWizard.Pages.AddRange(new Common.UI.Wizard.WizardPage[] {
            this.CalibrateServoWizardStartPage,
            this.CalibrateServoWizardPage1,
            this.calibrateServoWizardPage2});
            this.CalibrateServoWizard.Size = new System.Drawing.Size(445, 281);
            this.CalibrateServoWizard.TabIndex = 0;
            // 
            // CalibrateServoWizardPage1
            // 
            this.CalibrateServoWizardPage1.Controls.Add(this.header1);
            this.CalibrateServoWizardPage1.Controls.Add(this.nudCalibrateServoOffset);
            this.CalibrateServoWizardPage1.Controls.Add(this.lblCalibrateServoOffset);
            this.CalibrateServoWizardPage1.Controls.Add(this.trkCalibrateServoOffset);
            this.CalibrateServoWizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CalibrateServoWizardPage1.IsFinishPage = false;
            this.CalibrateServoWizardPage1.Location = new System.Drawing.Point(0, 0);
            this.CalibrateServoWizardPage1.Name = "CalibrateServoWizardPage1";
            this.CalibrateServoWizardPage1.Size = new System.Drawing.Size(445, 233);
            this.CalibrateServoWizardPage1.TabIndex = 2;
            // 
            // nudCalibrateServoOffset
            // 
            this.nudCalibrateServoOffset.Location = new System.Drawing.Point(114, 70);
            this.nudCalibrateServoOffset.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudCalibrateServoOffset.Name = "nudCalibrateServoOffset";
            this.nudCalibrateServoOffset.Size = new System.Drawing.Size(67, 21);
            this.nudCalibrateServoOffset.TabIndex = 4;
            this.nudCalibrateServoOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCalibrateServoOffset.ValueChanged += new System.EventHandler(this.nudCalibrateServoOffset_ValueChanged);
            // 
            // lblCalibrateServoOffset
            // 
            this.lblCalibrateServoOffset.AutoSize = true;
            this.lblCalibrateServoOffset.Location = new System.Drawing.Point(12, 72);
            this.lblCalibrateServoOffset.Name = "lblCalibrateServoOffset";
            this.lblCalibrateServoOffset.Size = new System.Drawing.Size(96, 13);
            this.lblCalibrateServoOffset.TabIndex = 3;
            this.lblCalibrateServoOffset.Text = "Calibration Offset:";
            // 
            // trkCalibrateServoOffset
            // 
            this.trkCalibrateServoOffset.LargeChange = 1024;
            this.trkCalibrateServoOffset.Location = new System.Drawing.Point(12, 88);
            this.trkCalibrateServoOffset.Maximum = 65535;
            this.trkCalibrateServoOffset.Name = "trkCalibrateServoOffset";
            this.trkCalibrateServoOffset.Size = new System.Drawing.Size(433, 45);
            this.trkCalibrateServoOffset.SmallChange = 256;
            this.trkCalibrateServoOffset.TabIndex = 1;
            this.trkCalibrateServoOffset.TickFrequency = 1024;
            this.trkCalibrateServoOffset.Scroll += new System.EventHandler(this.trkCalibrateServoOffset_Scroll);
            // 
            // CalibrateServoWizardStartPage
            // 
            this.CalibrateServoWizardStartPage.Controls.Add(this.infoPage1);
            this.CalibrateServoWizardStartPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CalibrateServoWizardStartPage.IsFinishPage = false;
            this.CalibrateServoWizardStartPage.Location = new System.Drawing.Point(0, 0);
            this.CalibrateServoWizardStartPage.Name = "CalibrateServoWizardStartPage";
            this.CalibrateServoWizardStartPage.Size = new System.Drawing.Size(445, 233);
            this.CalibrateServoWizardStartPage.TabIndex = 1;
            // 
            // infoPage1
            // 
            this.infoPage1.BackColor = System.Drawing.Color.White;
            this.infoPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPage1.Image = null;
            this.infoPage1.Location = new System.Drawing.Point(0, 0);
            this.infoPage1.Name = "infoPage1";
            this.infoPage1.PageText = "This wizard enables you to calibrate (set the minimum and maximum position) of a " +
                "servo motor attached to a PHCC DOA_8SERVO card.";
            this.infoPage1.PageTitle = "Welcome to the Servo Calibration Wizard";
            this.infoPage1.Size = new System.Drawing.Size(445, 233);
            this.infoPage1.TabIndex = 0;
            // 
            // calibrateServoWizardPage2
            // 
            this.calibrateServoWizardPage2.Controls.Add(this.wizhdrCalibrateServoPage1);
            this.calibrateServoWizardPage2.Controls.Add(this.nudCalibrateServoGain);
            this.calibrateServoWizardPage2.Controls.Add(this.lblCalibrateServoGain);
            this.calibrateServoWizardPage2.Controls.Add(this.trkCalibrateServoGain);
            this.calibrateServoWizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calibrateServoWizardPage2.IsFinishPage = false;
            this.calibrateServoWizardPage2.Location = new System.Drawing.Point(0, 0);
            this.calibrateServoWizardPage2.Name = "calibrateServoWizardPage2";
            this.calibrateServoWizardPage2.Size = new System.Drawing.Size(445, 233);
            this.calibrateServoWizardPage2.TabIndex = 3;
            // 
            // nudCalibrateServoGain
            // 
            this.nudCalibrateServoGain.Location = new System.Drawing.Point(50, 70);
            this.nudCalibrateServoGain.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCalibrateServoGain.Name = "nudCalibrateServoGain";
            this.nudCalibrateServoGain.Size = new System.Drawing.Size(49, 21);
            this.nudCalibrateServoGain.TabIndex = 8;
            this.nudCalibrateServoGain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCalibrateServoGain.ValueChanged += new System.EventHandler(this.nudCalibrateServoGain_ValueChanged);
            // 
            // lblCalibrateServoGain
            // 
            this.lblCalibrateServoGain.AutoSize = true;
            this.lblCalibrateServoGain.Location = new System.Drawing.Point(12, 72);
            this.lblCalibrateServoGain.Name = "lblCalibrateServoGain";
            this.lblCalibrateServoGain.Size = new System.Drawing.Size(32, 13);
            this.lblCalibrateServoGain.TabIndex = 7;
            this.lblCalibrateServoGain.Text = "Gain:";
            // 
            // trkCalibrateServoGain
            // 
            this.trkCalibrateServoGain.LargeChange = 4;
            this.trkCalibrateServoGain.Location = new System.Drawing.Point(12, 88);
            this.trkCalibrateServoGain.Maximum = 255;
            this.trkCalibrateServoGain.Name = "trkCalibrateServoGain";
            this.trkCalibrateServoGain.Size = new System.Drawing.Size(427, 45);
            this.trkCalibrateServoGain.TabIndex = 6;
            this.trkCalibrateServoGain.TickFrequency = 4;
            this.trkCalibrateServoGain.Scroll += new System.EventHandler(this.trkCalibrateServoGain_Scroll);
            // 
            // wizhdrCalibrateServoPage1
            // 
            this.wizhdrCalibrateServoPage1.BackColor = System.Drawing.SystemColors.Control;
            this.wizhdrCalibrateServoPage1.CausesValidation = false;
            this.wizhdrCalibrateServoPage1.Description = "Adjust the gain until the servo motor is at its maximum position.";
            this.wizhdrCalibrateServoPage1.Dock = System.Windows.Forms.DockStyle.Top;
            this.wizhdrCalibrateServoPage1.Image = ((System.Drawing.Image)(resources.GetObject("wizhdrCalibrateServoPage1.Image")));
            this.wizhdrCalibrateServoPage1.Location = new System.Drawing.Point(0, 0);
            this.wizhdrCalibrateServoPage1.Name = "wizhdrCalibrateServoPage1";
            this.wizhdrCalibrateServoPage1.Size = new System.Drawing.Size(445, 64);
            this.wizhdrCalibrateServoPage1.TabIndex = 9;
            this.wizhdrCalibrateServoPage1.Title = "Adjust Gain";
            // 
            // header1
            // 
            this.header1.BackColor = System.Drawing.SystemColors.Control;
            this.header1.CausesValidation = false;
            this.header1.Description = "Adjust the calibration offset until the servo motor is at its minimum position.";
            this.header1.Dock = System.Windows.Forms.DockStyle.Top;
            this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
            this.header1.Location = new System.Drawing.Point(0, 0);
            this.header1.Name = "header1";
            this.header1.Size = new System.Drawing.Size(445, 64);
            this.header1.TabIndex = 5;
            this.header1.Title = "Adjust Calibration Offset";
            // 
            // frmCalibrateServo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 281);
            this.Controls.Add(this.CalibrateServoWizard);
            this.Name = "frmCalibrateServo";
            this.Text = "Calibrate Servo Motor";
            this.CalibrateServoWizard.ResumeLayout(false);
            this.CalibrateServoWizardPage1.ResumeLayout(false);
            this.CalibrateServoWizardPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrateServoOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCalibrateServoOffset)).EndInit();
            this.CalibrateServoWizardStartPage.ResumeLayout(false);
            this.calibrateServoWizardPage2.ResumeLayout(false);
            this.calibrateServoWizardPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrateServoGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCalibrateServoGain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private global::Common.UI.Wizard.Wizard CalibrateServoWizard;
        private global::Common.UI.Wizard.WizardPage CalibrateServoWizardStartPage;
        private global::Common.UI.Wizard.WizardPage CalibrateServoWizardPage1;
        private global::Common.UI.Wizard.InfoPage infoPage1;
        private System.Windows.Forms.NumericUpDown nudCalibrateServoOffset;
        private System.Windows.Forms.Label lblCalibrateServoOffset;
        private System.Windows.Forms.TrackBar trkCalibrateServoOffset;
        private Common.UI.Wizard.WizardPage calibrateServoWizardPage2;
        private System.Windows.Forms.NumericUpDown nudCalibrateServoGain;
        private System.Windows.Forms.Label lblCalibrateServoGain;
        private System.Windows.Forms.TrackBar trkCalibrateServoGain;
        private Common.UI.Wizard.Header wizhdrCalibrateServoPage1;
        private Common.UI.Wizard.Header header1;
    }
}