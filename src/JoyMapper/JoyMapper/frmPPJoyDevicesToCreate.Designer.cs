namespace JoyMapper
{
    partial class frmPPJoyDevicesToCreate
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPPJoyDevicesToCreate));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblExistingPPJoyVirtualJoystickDevices = new System.Windows.Forms.Label();
            this.lblNumExistingDevices = new System.Windows.Forms.Label();
            this.lblPPJoyDevicesToCreate = new System.Windows.Forms.Label();
            this.udDevicesToCreate = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udDevicesToCreate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(15, 59);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(96, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblExistingPPJoyVirtualJoystickDevices
            // 
            this.lblExistingPPJoyVirtualJoystickDevices.AutoSize = true;
            this.lblExistingPPJoyVirtualJoystickDevices.Location = new System.Drawing.Point(12, 9);
            this.lblExistingPPJoyVirtualJoystickDevices.Name = "lblExistingPPJoyVirtualJoystickDevices";
            this.lblExistingPPJoyVirtualJoystickDevices.Size = new System.Drawing.Size(213, 13);
            this.lblExistingPPJoyVirtualJoystickDevices.TabIndex = 2;
            this.lblExistingPPJoyVirtualJoystickDevices.Text = "# of existing PPJoy Virtual Joystick devices:";
            // 
            // lblNumExistingDevices
            // 
            this.lblNumExistingDevices.AutoSize = true;
            this.lblNumExistingDevices.Location = new System.Drawing.Point(236, 9);
            this.lblNumExistingDevices.Name = "lblNumExistingDevices";
            this.lblNumExistingDevices.Size = new System.Drawing.Size(25, 13);
            this.lblNumExistingDevices.TabIndex = 3;
            this.lblNumExistingDevices.Text = "000";
            // 
            // lblPPJoyDevicesToCreate
            // 
            this.lblPPJoyDevicesToCreate.AutoSize = true;
            this.lblPPJoyDevicesToCreate.Location = new System.Drawing.Point(12, 32);
            this.lblPPJoyDevicesToCreate.Name = "lblPPJoyDevicesToCreate";
            this.lblPPJoyDevicesToCreate.Size = new System.Drawing.Size(198, 13);
            this.lblPPJoyDevicesToCreate.TabIndex = 4;
            this.lblPPJoyDevicesToCreate.Text = "PPJoy Virtual Joystick devices to create:";
            // 
            // udDevicesToCreate
            // 
            this.udDevicesToCreate.Location = new System.Drawing.Point(239, 32);
            this.udDevicesToCreate.Name = "udDevicesToCreate";
            this.udDevicesToCreate.Size = new System.Drawing.Size(41, 20);
            this.udDevicesToCreate.TabIndex = 5;
            // 
            // frmPPJoyDevicesToCreate
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(314, 94);
            this.Controls.Add(this.udDevicesToCreate);
            this.Controls.Add(this.lblPPJoyDevicesToCreate);
            this.Controls.Add(this.lblNumExistingDevices);
            this.Controls.Add(this.lblExistingPPJoyVirtualJoystickDevices);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPPJoyDevicesToCreate";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create PPJoy Virtual Joystick Devices";
            this.Load += new System.EventHandler(this.PPJoyDevicesToCreateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udDevicesToCreate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblExistingPPJoyVirtualJoystickDevices;
        private System.Windows.Forms.Label lblNumExistingDevices;
        private System.Windows.Forms.Label lblPPJoyDevicesToCreate;
        private System.Windows.Forms.NumericUpDown udDevicesToCreate;
    }
}