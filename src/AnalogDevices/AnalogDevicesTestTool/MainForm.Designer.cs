namespace AnalogDevicesTestTool
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
            this.gbDACOutputs = new System.Windows.Forms.GroupBox();
            this.rdoDAC39 = new System.Windows.Forms.RadioButton();
            this.rdoDAC38 = new System.Windows.Forms.RadioButton();
            this.rdoDAC31 = new System.Windows.Forms.RadioButton();
            this.rdoDAC37 = new System.Windows.Forms.RadioButton();
            this.rdoDAC30 = new System.Windows.Forms.RadioButton();
            this.rdoDAC36 = new System.Windows.Forms.RadioButton();
            this.rdoDAC23 = new System.Windows.Forms.RadioButton();
            this.rdoDAC35 = new System.Windows.Forms.RadioButton();
            this.rdoDAC29 = new System.Windows.Forms.RadioButton();
            this.rdoDAC34 = new System.Windows.Forms.RadioButton();
            this.rdoDAC22 = new System.Windows.Forms.RadioButton();
            this.btnUpdateAllDACOutputs = new System.Windows.Forms.Button();
            this.rdoDAC33 = new System.Windows.Forms.RadioButton();
            this.btnResumeAllDACOutputs = new System.Windows.Forms.Button();
            this.rdoDAC28 = new System.Windows.Forms.RadioButton();
            this.btnSuspendAllDACOutputs = new System.Windows.Forms.Button();
            this.rdoDAC32 = new System.Windows.Forms.RadioButton();
            this.rdoDAC15 = new System.Windows.Forms.RadioButton();
            this.rdoDAC27 = new System.Windows.Forms.RadioButton();
            this.rdoDAC21 = new System.Windows.Forms.RadioButton();
            this.rdoDAC26 = new System.Windows.Forms.RadioButton();
            this.rdoDAC7 = new System.Windows.Forms.RadioButton();
            this.rdoDAC25 = new System.Windows.Forms.RadioButton();
            this.rdoDAC20 = new System.Windows.Forms.RadioButton();
            this.rdoDAC24 = new System.Windows.Forms.RadioButton();
            this.rdoDAC14 = new System.Windows.Forms.RadioButton();
            this.rdoDAC19 = new System.Windows.Forms.RadioButton();
            this.rdoDAC6 = new System.Windows.Forms.RadioButton();
            this.rdoDAC18 = new System.Windows.Forms.RadioButton();
            this.rdoDAC13 = new System.Windows.Forms.RadioButton();
            this.rdoDAC17 = new System.Windows.Forms.RadioButton();
            this.rdoDAC5 = new System.Windows.Forms.RadioButton();
            this.rdoDAC16 = new System.Windows.Forms.RadioButton();
            this.rdoDAC12 = new System.Windows.Forms.RadioButton();
            this.rdoDAC4 = new System.Windows.Forms.RadioButton();
            this.rdoDAC11 = new System.Windows.Forms.RadioButton();
            this.rdoDAC3 = new System.Windows.Forms.RadioButton();
            this.rdoDAC10 = new System.Windows.Forms.RadioButton();
            this.rdoDAC2 = new System.Windows.Forms.RadioButton();
            this.rdoDAC9 = new System.Windows.Forms.RadioButton();
            this.rdoDAC8 = new System.Windows.Forms.RadioButton();
            this.rdoDAC1 = new System.Windows.Forms.RadioButton();
            this.rdoDAC0 = new System.Windows.Forms.RadioButton();
            this.gbDataValues = new System.Windows.Forms.GroupBox();
            this.txtVoutCalculated = new System.Windows.Forms.TextBox();
            this.lblVout = new System.Windows.Forms.Label();
            this.txtDACChannelOffset = new System.Windows.Forms.TextBox();
            this.lblDACChannelOffset = new System.Windows.Forms.Label();
            this.rdoDataValueB = new System.Windows.Forms.RadioButton();
            this.txtDACChannelGain = new System.Windows.Forms.TextBox();
            this.lblDACChannelGain = new System.Windows.Forms.Label();
            this.rdoDataValueA = new System.Windows.Forms.RadioButton();
            this.txtDataValueA = new System.Windows.Forms.TextBox();
            this.lblDataValueA = new System.Windows.Forms.Label();
            this.lblDataValueB = new System.Windows.Forms.Label();
            this.txtDataValueB = new System.Windows.Forms.TextBox();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cboDevices = new System.Windows.Forms.ComboBox();
            this.gbDeviceOptions = new System.Windows.Forms.GroupBox();
            this.lblTemperatureStatus = new System.Windows.Forms.Label();
            this.txtOverTempStatus = new System.Windows.Forms.TextBox();
            this.chkOverTempShutdownEnabled = new System.Windows.Forms.CheckBox();
            this.lblVREF0 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtVREF0 = new System.Windows.Forms.TextBox();
            this.btnPerformSoftPowerDown = new System.Windows.Forms.Button();
            this.lblVREF1 = new System.Windows.Forms.Label();
            this.btnSoftPowerUp = new System.Windows.Forms.Button();
            this.txtVREF1 = new System.Windows.Forms.TextBox();
            this.txtOffsetDAC0 = new System.Windows.Forms.TextBox();
            this.lblGroup1Offset = new System.Windows.Forms.Label();
            this.lblOffsetDAC0 = new System.Windows.Forms.Label();
            this.txtOffsetDAC1 = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.mainPanel.SuspendLayout();
            this.gbDACOutputs.SuspendLayout();
            this.gbDataValues.SuspendLayout();
            this.gbDeviceOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.gbDACOutputs);
            this.mainPanel.Controls.Add(this.lblDevice);
            this.mainPanel.Controls.Add(this.cboDevices);
            this.mainPanel.Controls.Add(this.gbDeviceOptions);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1236, 775);
            this.mainPanel.TabIndex = 1;
            // 
            // gbDACOutputs
            // 
            this.gbDACOutputs.Controls.Add(this.rdoDAC39);
            this.gbDACOutputs.Controls.Add(this.rdoDAC38);
            this.gbDACOutputs.Controls.Add(this.rdoDAC31);
            this.gbDACOutputs.Controls.Add(this.rdoDAC37);
            this.gbDACOutputs.Controls.Add(this.rdoDAC30);
            this.gbDACOutputs.Controls.Add(this.rdoDAC36);
            this.gbDACOutputs.Controls.Add(this.rdoDAC23);
            this.gbDACOutputs.Controls.Add(this.rdoDAC35);
            this.gbDACOutputs.Controls.Add(this.rdoDAC29);
            this.gbDACOutputs.Controls.Add(this.rdoDAC34);
            this.gbDACOutputs.Controls.Add(this.rdoDAC22);
            this.gbDACOutputs.Controls.Add(this.btnUpdateAllDACOutputs);
            this.gbDACOutputs.Controls.Add(this.rdoDAC33);
            this.gbDACOutputs.Controls.Add(this.btnResumeAllDACOutputs);
            this.gbDACOutputs.Controls.Add(this.rdoDAC28);
            this.gbDACOutputs.Controls.Add(this.btnSuspendAllDACOutputs);
            this.gbDACOutputs.Controls.Add(this.rdoDAC32);
            this.gbDACOutputs.Controls.Add(this.rdoDAC15);
            this.gbDACOutputs.Controls.Add(this.rdoDAC27);
            this.gbDACOutputs.Controls.Add(this.rdoDAC21);
            this.gbDACOutputs.Controls.Add(this.rdoDAC26);
            this.gbDACOutputs.Controls.Add(this.rdoDAC7);
            this.gbDACOutputs.Controls.Add(this.rdoDAC25);
            this.gbDACOutputs.Controls.Add(this.rdoDAC20);
            this.gbDACOutputs.Controls.Add(this.rdoDAC24);
            this.gbDACOutputs.Controls.Add(this.rdoDAC14);
            this.gbDACOutputs.Controls.Add(this.rdoDAC19);
            this.gbDACOutputs.Controls.Add(this.rdoDAC6);
            this.gbDACOutputs.Controls.Add(this.rdoDAC18);
            this.gbDACOutputs.Controls.Add(this.rdoDAC13);
            this.gbDACOutputs.Controls.Add(this.rdoDAC17);
            this.gbDACOutputs.Controls.Add(this.rdoDAC5);
            this.gbDACOutputs.Controls.Add(this.rdoDAC16);
            this.gbDACOutputs.Controls.Add(this.rdoDAC12);
            this.gbDACOutputs.Controls.Add(this.rdoDAC4);
            this.gbDACOutputs.Controls.Add(this.rdoDAC11);
            this.gbDACOutputs.Controls.Add(this.rdoDAC3);
            this.gbDACOutputs.Controls.Add(this.rdoDAC10);
            this.gbDACOutputs.Controls.Add(this.rdoDAC2);
            this.gbDACOutputs.Controls.Add(this.rdoDAC9);
            this.gbDACOutputs.Controls.Add(this.rdoDAC8);
            this.gbDACOutputs.Controls.Add(this.rdoDAC1);
            this.gbDACOutputs.Controls.Add(this.rdoDAC0);
            this.gbDACOutputs.Controls.Add(this.gbDataValues);
            this.gbDACOutputs.Location = new System.Drawing.Point(36, 288);
            this.gbDACOutputs.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDACOutputs.Name = "gbDACOutputs";
            this.gbDACOutputs.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDACOutputs.Size = new System.Drawing.Size(1176, 458);
            this.gbDACOutputs.TabIndex = 0;
            this.gbDACOutputs.TabStop = false;
            this.gbDACOutputs.Text = "DAC Outputs";
            // 
            // rdoDAC39
            // 
            this.rdoDAC39.AutoSize = true;
            this.rdoDAC39.Location = new System.Drawing.Point(880, 213);
            this.rdoDAC39.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC39.Name = "rdoDAC39";
            this.rdoDAC39.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC39.TabIndex = 47;
            this.rdoDAC39.Text = "DAC 39";
            this.rdoDAC39.UseVisualStyleBackColor = true;
            // 
            // rdoDAC38
            // 
            this.rdoDAC38.AutoSize = true;
            this.rdoDAC38.Location = new System.Drawing.Point(756, 213);
            this.rdoDAC38.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC38.Name = "rdoDAC38";
            this.rdoDAC38.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC38.TabIndex = 46;
            this.rdoDAC38.Text = "DAC 38";
            this.rdoDAC38.UseVisualStyleBackColor = true;
            // 
            // rdoDAC31
            // 
            this.rdoDAC31.AutoSize = true;
            this.rdoDAC31.Location = new System.Drawing.Point(880, 169);
            this.rdoDAC31.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC31.Name = "rdoDAC31";
            this.rdoDAC31.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC31.TabIndex = 39;
            this.rdoDAC31.Text = "DAC 31";
            this.rdoDAC31.UseVisualStyleBackColor = true;
            // 
            // rdoDAC37
            // 
            this.rdoDAC37.AutoSize = true;
            this.rdoDAC37.Location = new System.Drawing.Point(632, 213);
            this.rdoDAC37.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC37.Name = "rdoDAC37";
            this.rdoDAC37.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC37.TabIndex = 45;
            this.rdoDAC37.Text = "DAC 37";
            this.rdoDAC37.UseVisualStyleBackColor = true;
            // 
            // rdoDAC30
            // 
            this.rdoDAC30.AutoSize = true;
            this.rdoDAC30.Location = new System.Drawing.Point(756, 169);
            this.rdoDAC30.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC30.Name = "rdoDAC30";
            this.rdoDAC30.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC30.TabIndex = 38;
            this.rdoDAC30.Text = "DAC 30";
            this.rdoDAC30.UseVisualStyleBackColor = true;
            // 
            // rdoDAC36
            // 
            this.rdoDAC36.AutoSize = true;
            this.rdoDAC36.Location = new System.Drawing.Point(508, 213);
            this.rdoDAC36.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC36.Name = "rdoDAC36";
            this.rdoDAC36.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC36.TabIndex = 44;
            this.rdoDAC36.Text = "DAC 36";
            this.rdoDAC36.UseVisualStyleBackColor = true;
            // 
            // rdoDAC23
            // 
            this.rdoDAC23.AutoSize = true;
            this.rdoDAC23.Location = new System.Drawing.Point(880, 125);
            this.rdoDAC23.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC23.Name = "rdoDAC23";
            this.rdoDAC23.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC23.TabIndex = 31;
            this.rdoDAC23.Text = "DAC 23";
            this.rdoDAC23.UseVisualStyleBackColor = true;
            // 
            // rdoDAC35
            // 
            this.rdoDAC35.AutoSize = true;
            this.rdoDAC35.Location = new System.Drawing.Point(384, 213);
            this.rdoDAC35.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC35.Name = "rdoDAC35";
            this.rdoDAC35.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC35.TabIndex = 43;
            this.rdoDAC35.Text = "DAC 35";
            this.rdoDAC35.UseVisualStyleBackColor = true;
            // 
            // rdoDAC29
            // 
            this.rdoDAC29.AutoSize = true;
            this.rdoDAC29.Location = new System.Drawing.Point(632, 169);
            this.rdoDAC29.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC29.Name = "rdoDAC29";
            this.rdoDAC29.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC29.TabIndex = 37;
            this.rdoDAC29.Text = "DAC 29";
            this.rdoDAC29.UseVisualStyleBackColor = true;
            // 
            // rdoDAC34
            // 
            this.rdoDAC34.AutoSize = true;
            this.rdoDAC34.Location = new System.Drawing.Point(260, 213);
            this.rdoDAC34.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC34.Name = "rdoDAC34";
            this.rdoDAC34.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC34.TabIndex = 42;
            this.rdoDAC34.Text = "DAC 34";
            this.rdoDAC34.UseVisualStyleBackColor = true;
            // 
            // rdoDAC22
            // 
            this.rdoDAC22.AutoSize = true;
            this.rdoDAC22.Location = new System.Drawing.Point(756, 125);
            this.rdoDAC22.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC22.Name = "rdoDAC22";
            this.rdoDAC22.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC22.TabIndex = 30;
            this.rdoDAC22.Text = "DAC 22";
            this.rdoDAC22.UseVisualStyleBackColor = true;
            // 
            // btnUpdateAllDACOutputs
            // 
            this.btnUpdateAllDACOutputs.Location = new System.Drawing.Point(756, 287);
            this.btnUpdateAllDACOutputs.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnUpdateAllDACOutputs.Name = "btnUpdateAllDACOutputs";
            this.btnUpdateAllDACOutputs.Size = new System.Drawing.Size(308, 44);
            this.btnUpdateAllDACOutputs.TabIndex = 54;
            this.btnUpdateAllDACOutputs.Text = "Update All DAC Outputs";
            this.btnUpdateAllDACOutputs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdateAllDACOutputs.UseVisualStyleBackColor = true;
            this.btnUpdateAllDACOutputs.Click += new System.EventHandler(this.btnUpdateAllDacOutputs_Click);
            // 
            // rdoDAC33
            // 
            this.rdoDAC33.AutoSize = true;
            this.rdoDAC33.Location = new System.Drawing.Point(136, 213);
            this.rdoDAC33.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC33.Name = "rdoDAC33";
            this.rdoDAC33.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC33.TabIndex = 41;
            this.rdoDAC33.Text = "DAC 33";
            this.rdoDAC33.UseVisualStyleBackColor = true;
            // 
            // btnResumeAllDACOutputs
            // 
            this.btnResumeAllDACOutputs.Location = new System.Drawing.Point(756, 387);
            this.btnResumeAllDACOutputs.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnResumeAllDACOutputs.Name = "btnResumeAllDACOutputs";
            this.btnResumeAllDACOutputs.Size = new System.Drawing.Size(308, 44);
            this.btnResumeAllDACOutputs.TabIndex = 56;
            this.btnResumeAllDACOutputs.Text = "Resume All DAC Outputs";
            this.btnResumeAllDACOutputs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResumeAllDACOutputs.UseVisualStyleBackColor = true;
            this.btnResumeAllDACOutputs.Click += new System.EventHandler(this.btnResumeAllDACOutputs_Click);
            // 
            // rdoDAC28
            // 
            this.rdoDAC28.AutoSize = true;
            this.rdoDAC28.Location = new System.Drawing.Point(508, 169);
            this.rdoDAC28.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC28.Name = "rdoDAC28";
            this.rdoDAC28.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC28.TabIndex = 36;
            this.rdoDAC28.Text = "DAC 28";
            this.rdoDAC28.UseVisualStyleBackColor = true;
            // 
            // btnSuspendAllDACOutputs
            // 
            this.btnSuspendAllDACOutputs.Location = new System.Drawing.Point(756, 338);
            this.btnSuspendAllDACOutputs.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnSuspendAllDACOutputs.Name = "btnSuspendAllDACOutputs";
            this.btnSuspendAllDACOutputs.Size = new System.Drawing.Size(308, 44);
            this.btnSuspendAllDACOutputs.TabIndex = 55;
            this.btnSuspendAllDACOutputs.Text = "Suspend All DAC Outputs";
            this.btnSuspendAllDACOutputs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSuspendAllDACOutputs.UseVisualStyleBackColor = true;
            this.btnSuspendAllDACOutputs.Click += new System.EventHandler(this.btnSuspendAllDACOutputs_Click);
            // 
            // rdoDAC32
            // 
            this.rdoDAC32.AutoSize = true;
            this.rdoDAC32.Location = new System.Drawing.Point(12, 213);
            this.rdoDAC32.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC32.Name = "rdoDAC32";
            this.rdoDAC32.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC32.TabIndex = 40;
            this.rdoDAC32.Text = "DAC 32";
            this.rdoDAC32.UseVisualStyleBackColor = true;
            // 
            // rdoDAC15
            // 
            this.rdoDAC15.AutoSize = true;
            this.rdoDAC15.Location = new System.Drawing.Point(880, 81);
            this.rdoDAC15.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC15.Name = "rdoDAC15";
            this.rdoDAC15.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC15.TabIndex = 23;
            this.rdoDAC15.Text = "DAC 15";
            this.rdoDAC15.UseVisualStyleBackColor = true;
            // 
            // rdoDAC27
            // 
            this.rdoDAC27.AutoSize = true;
            this.rdoDAC27.Location = new System.Drawing.Point(384, 169);
            this.rdoDAC27.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC27.Name = "rdoDAC27";
            this.rdoDAC27.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC27.TabIndex = 35;
            this.rdoDAC27.Text = "DAC 27";
            this.rdoDAC27.UseVisualStyleBackColor = true;
            // 
            // rdoDAC21
            // 
            this.rdoDAC21.AutoSize = true;
            this.rdoDAC21.Location = new System.Drawing.Point(632, 125);
            this.rdoDAC21.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC21.Name = "rdoDAC21";
            this.rdoDAC21.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC21.TabIndex = 29;
            this.rdoDAC21.Text = "DAC 21";
            this.rdoDAC21.UseVisualStyleBackColor = true;
            // 
            // rdoDAC26
            // 
            this.rdoDAC26.AutoSize = true;
            this.rdoDAC26.Location = new System.Drawing.Point(260, 169);
            this.rdoDAC26.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC26.Name = "rdoDAC26";
            this.rdoDAC26.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC26.TabIndex = 34;
            this.rdoDAC26.Text = "DAC 26";
            this.rdoDAC26.UseVisualStyleBackColor = true;
            // 
            // rdoDAC7
            // 
            this.rdoDAC7.AutoSize = true;
            this.rdoDAC7.Location = new System.Drawing.Point(880, 37);
            this.rdoDAC7.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC7.Name = "rdoDAC7";
            this.rdoDAC7.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC7.TabIndex = 15;
            this.rdoDAC7.Text = "DAC 7";
            this.rdoDAC7.UseVisualStyleBackColor = true;
            // 
            // rdoDAC25
            // 
            this.rdoDAC25.AutoSize = true;
            this.rdoDAC25.Location = new System.Drawing.Point(136, 169);
            this.rdoDAC25.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC25.Name = "rdoDAC25";
            this.rdoDAC25.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC25.TabIndex = 33;
            this.rdoDAC25.Text = "DAC 25";
            this.rdoDAC25.UseVisualStyleBackColor = true;
            // 
            // rdoDAC20
            // 
            this.rdoDAC20.AutoSize = true;
            this.rdoDAC20.Location = new System.Drawing.Point(508, 125);
            this.rdoDAC20.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC20.Name = "rdoDAC20";
            this.rdoDAC20.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC20.TabIndex = 28;
            this.rdoDAC20.Text = "DAC 20";
            this.rdoDAC20.UseVisualStyleBackColor = true;
            // 
            // rdoDAC24
            // 
            this.rdoDAC24.AutoSize = true;
            this.rdoDAC24.Location = new System.Drawing.Point(12, 169);
            this.rdoDAC24.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC24.Name = "rdoDAC24";
            this.rdoDAC24.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC24.TabIndex = 32;
            this.rdoDAC24.Text = "DAC 24";
            this.rdoDAC24.UseVisualStyleBackColor = true;
            // 
            // rdoDAC14
            // 
            this.rdoDAC14.AutoSize = true;
            this.rdoDAC14.Location = new System.Drawing.Point(756, 81);
            this.rdoDAC14.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC14.Name = "rdoDAC14";
            this.rdoDAC14.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC14.TabIndex = 22;
            this.rdoDAC14.Text = "DAC 14";
            this.rdoDAC14.UseVisualStyleBackColor = true;
            // 
            // rdoDAC19
            // 
            this.rdoDAC19.AutoSize = true;
            this.rdoDAC19.Location = new System.Drawing.Point(384, 125);
            this.rdoDAC19.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC19.Name = "rdoDAC19";
            this.rdoDAC19.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC19.TabIndex = 27;
            this.rdoDAC19.Text = "DAC 19";
            this.rdoDAC19.UseVisualStyleBackColor = true;
            // 
            // rdoDAC6
            // 
            this.rdoDAC6.AutoSize = true;
            this.rdoDAC6.Location = new System.Drawing.Point(756, 37);
            this.rdoDAC6.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC6.Name = "rdoDAC6";
            this.rdoDAC6.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC6.TabIndex = 14;
            this.rdoDAC6.Text = "DAC 6";
            this.rdoDAC6.UseVisualStyleBackColor = true;
            // 
            // rdoDAC18
            // 
            this.rdoDAC18.AutoSize = true;
            this.rdoDAC18.Location = new System.Drawing.Point(260, 125);
            this.rdoDAC18.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC18.Name = "rdoDAC18";
            this.rdoDAC18.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC18.TabIndex = 26;
            this.rdoDAC18.Text = "DAC 18";
            this.rdoDAC18.UseVisualStyleBackColor = true;
            // 
            // rdoDAC13
            // 
            this.rdoDAC13.AutoSize = true;
            this.rdoDAC13.Location = new System.Drawing.Point(632, 81);
            this.rdoDAC13.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC13.Name = "rdoDAC13";
            this.rdoDAC13.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC13.TabIndex = 21;
            this.rdoDAC13.Text = "DAC 13";
            this.rdoDAC13.UseVisualStyleBackColor = true;
            // 
            // rdoDAC17
            // 
            this.rdoDAC17.AutoSize = true;
            this.rdoDAC17.Location = new System.Drawing.Point(136, 125);
            this.rdoDAC17.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC17.Name = "rdoDAC17";
            this.rdoDAC17.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC17.TabIndex = 25;
            this.rdoDAC17.Text = "DAC 17";
            this.rdoDAC17.UseVisualStyleBackColor = true;
            // 
            // rdoDAC5
            // 
            this.rdoDAC5.AutoSize = true;
            this.rdoDAC5.Location = new System.Drawing.Point(632, 37);
            this.rdoDAC5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC5.Name = "rdoDAC5";
            this.rdoDAC5.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC5.TabIndex = 13;
            this.rdoDAC5.Text = "DAC 5";
            this.rdoDAC5.UseVisualStyleBackColor = true;
            // 
            // rdoDAC16
            // 
            this.rdoDAC16.AutoSize = true;
            this.rdoDAC16.Location = new System.Drawing.Point(12, 125);
            this.rdoDAC16.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC16.Name = "rdoDAC16";
            this.rdoDAC16.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC16.TabIndex = 24;
            this.rdoDAC16.Text = "DAC 16";
            this.rdoDAC16.UseVisualStyleBackColor = true;
            // 
            // rdoDAC12
            // 
            this.rdoDAC12.AutoSize = true;
            this.rdoDAC12.Location = new System.Drawing.Point(508, 81);
            this.rdoDAC12.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC12.Name = "rdoDAC12";
            this.rdoDAC12.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC12.TabIndex = 20;
            this.rdoDAC12.Text = "DAC 12";
            this.rdoDAC12.UseVisualStyleBackColor = true;
            // 
            // rdoDAC4
            // 
            this.rdoDAC4.AutoSize = true;
            this.rdoDAC4.Location = new System.Drawing.Point(508, 37);
            this.rdoDAC4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC4.Name = "rdoDAC4";
            this.rdoDAC4.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC4.TabIndex = 12;
            this.rdoDAC4.Text = "DAC 4";
            this.rdoDAC4.UseVisualStyleBackColor = true;
            // 
            // rdoDAC11
            // 
            this.rdoDAC11.AutoSize = true;
            this.rdoDAC11.Location = new System.Drawing.Point(384, 81);
            this.rdoDAC11.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC11.Name = "rdoDAC11";
            this.rdoDAC11.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC11.TabIndex = 19;
            this.rdoDAC11.Text = "DAC 11";
            this.rdoDAC11.UseVisualStyleBackColor = true;
            // 
            // rdoDAC3
            // 
            this.rdoDAC3.AutoSize = true;
            this.rdoDAC3.Location = new System.Drawing.Point(384, 37);
            this.rdoDAC3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC3.Name = "rdoDAC3";
            this.rdoDAC3.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC3.TabIndex = 11;
            this.rdoDAC3.Text = "DAC 3";
            this.rdoDAC3.UseVisualStyleBackColor = true;
            // 
            // rdoDAC10
            // 
            this.rdoDAC10.AutoSize = true;
            this.rdoDAC10.Location = new System.Drawing.Point(260, 81);
            this.rdoDAC10.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC10.Name = "rdoDAC10";
            this.rdoDAC10.Size = new System.Drawing.Size(117, 29);
            this.rdoDAC10.TabIndex = 18;
            this.rdoDAC10.Text = "DAC 10";
            this.rdoDAC10.UseVisualStyleBackColor = true;
            // 
            // rdoDAC2
            // 
            this.rdoDAC2.AutoSize = true;
            this.rdoDAC2.Location = new System.Drawing.Point(260, 37);
            this.rdoDAC2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC2.Name = "rdoDAC2";
            this.rdoDAC2.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC2.TabIndex = 10;
            this.rdoDAC2.Text = "DAC 2";
            this.rdoDAC2.UseVisualStyleBackColor = true;
            // 
            // rdoDAC9
            // 
            this.rdoDAC9.AutoSize = true;
            this.rdoDAC9.Location = new System.Drawing.Point(136, 81);
            this.rdoDAC9.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC9.Name = "rdoDAC9";
            this.rdoDAC9.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC9.TabIndex = 17;
            this.rdoDAC9.Text = "DAC 9";
            this.rdoDAC9.UseVisualStyleBackColor = true;
            // 
            // rdoDAC8
            // 
            this.rdoDAC8.AutoSize = true;
            this.rdoDAC8.Location = new System.Drawing.Point(12, 81);
            this.rdoDAC8.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC8.Name = "rdoDAC8";
            this.rdoDAC8.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC8.TabIndex = 16;
            this.rdoDAC8.Text = "DAC 8";
            this.rdoDAC8.UseVisualStyleBackColor = true;
            // 
            // rdoDAC1
            // 
            this.rdoDAC1.AutoSize = true;
            this.rdoDAC1.Location = new System.Drawing.Point(136, 37);
            this.rdoDAC1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC1.Name = "rdoDAC1";
            this.rdoDAC1.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC1.TabIndex = 9;
            this.rdoDAC1.Text = "DAC 1";
            this.rdoDAC1.UseVisualStyleBackColor = true;
            // 
            // rdoDAC0
            // 
            this.rdoDAC0.AutoSize = true;
            this.rdoDAC0.Checked = true;
            this.rdoDAC0.Location = new System.Drawing.Point(12, 37);
            this.rdoDAC0.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDAC0.Name = "rdoDAC0";
            this.rdoDAC0.Size = new System.Drawing.Size(105, 29);
            this.rdoDAC0.TabIndex = 8;
            this.rdoDAC0.TabStop = true;
            this.rdoDAC0.Text = "DAC 0";
            this.rdoDAC0.UseVisualStyleBackColor = true;
            // 
            // gbDataValues
            // 
            this.gbDataValues.Controls.Add(this.txtVoutCalculated);
            this.gbDataValues.Controls.Add(this.lblVout);
            this.gbDataValues.Controls.Add(this.txtDACChannelOffset);
            this.gbDataValues.Controls.Add(this.lblDACChannelOffset);
            this.gbDataValues.Controls.Add(this.rdoDataValueB);
            this.gbDataValues.Controls.Add(this.txtDACChannelGain);
            this.gbDataValues.Controls.Add(this.lblDACChannelGain);
            this.gbDataValues.Controls.Add(this.rdoDataValueA);
            this.gbDataValues.Controls.Add(this.txtDataValueA);
            this.gbDataValues.Controls.Add(this.lblDataValueA);
            this.gbDataValues.Controls.Add(this.lblDataValueB);
            this.gbDataValues.Controls.Add(this.txtDataValueB);
            this.gbDataValues.Location = new System.Drawing.Point(14, 275);
            this.gbDataValues.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDataValues.Name = "gbDataValues";
            this.gbDataValues.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDataValues.Size = new System.Drawing.Size(730, 156);
            this.gbDataValues.TabIndex = 26;
            this.gbDataValues.TabStop = false;
            this.gbDataValues.Text = "Data Values";
            // 
            // txtVoutCalculated
            // 
            this.txtVoutCalculated.BackColor = System.Drawing.SystemColors.Control;
            this.txtVoutCalculated.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVoutCalculated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoutCalculated.Location = new System.Drawing.Point(572, 67);
            this.txtVoutCalculated.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtVoutCalculated.Name = "txtVoutCalculated";
            this.txtVoutCalculated.ReadOnly = true;
            this.txtVoutCalculated.Size = new System.Drawing.Size(132, 32);
            this.txtVoutCalculated.TabIndex = 25;
            this.txtVoutCalculated.TabStop = false;
            this.txtVoutCalculated.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblVout
            // 
            this.lblVout.AutoSize = true;
            this.lblVout.Location = new System.Drawing.Point(508, 73);
            this.lblVout.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblVout.Name = "lblVout";
            this.lblVout.Size = new System.Drawing.Size(62, 25);
            this.lblVout.TabIndex = 24;
            this.lblVout.Text = "Vout:";
            // 
            // txtDACChannelOffset
            // 
            this.txtDACChannelOffset.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDACChannelOffset.Location = new System.Drawing.Point(108, 44);
            this.txtDACChannelOffset.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtDACChannelOffset.MaxLength = 4;
            this.txtDACChannelOffset.Name = "txtDACChannelOffset";
            this.txtDACChannelOffset.Size = new System.Drawing.Size(64, 31);
            this.txtDACChannelOffset.TabIndex = 48;
            this.txtDACChannelOffset.Text = "8000";
            this.txtDACChannelOffset.Click += new System.EventHandler(this.txtDACChannelOffset_Click);
            this.txtDACChannelOffset.Validating += new System.ComponentModel.CancelEventHandler(this.txtDACChannelOffset_Validating);
            this.txtDACChannelOffset.Validated += new System.EventHandler(this.txtDACChannelOffset_Validated);
            // 
            // lblDACChannelOffset
            // 
            this.lblDACChannelOffset.AutoSize = true;
            this.lblDACChannelOffset.Location = new System.Drawing.Point(26, 50);
            this.lblDACChannelOffset.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDACChannelOffset.Name = "lblDACChannelOffset";
            this.lblDACChannelOffset.Size = new System.Drawing.Size(69, 25);
            this.lblDACChannelOffset.TabIndex = 15;
            this.lblDACChannelOffset.Text = "Offset";
            // 
            // rdoDataValueB
            // 
            this.rdoDataValueB.AutoSize = true;
            this.rdoDataValueB.Location = new System.Drawing.Point(226, 100);
            this.rdoDataValueB.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDataValueB.Name = "rdoDataValueB";
            this.rdoDataValueB.Size = new System.Drawing.Size(27, 26);
            this.rdoDataValueB.TabIndex = 52;
            this.rdoDataValueB.UseVisualStyleBackColor = true;
            this.rdoDataValueB.Click += new System.EventHandler(this.rdoDataValueB_Click);
            // 
            // txtDACChannelGain
            // 
            this.txtDACChannelGain.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDACChannelGain.Location = new System.Drawing.Point(108, 94);
            this.txtDACChannelGain.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtDACChannelGain.MaxLength = 4;
            this.txtDACChannelGain.Name = "txtDACChannelGain";
            this.txtDACChannelGain.Size = new System.Drawing.Size(64, 31);
            this.txtDACChannelGain.TabIndex = 49;
            this.txtDACChannelGain.Text = "FFFF";
            this.txtDACChannelGain.Click += new System.EventHandler(this.txtDACChannelGain_Click);
            this.txtDACChannelGain.Validating += new System.ComponentModel.CancelEventHandler(this.txtDACChannelGain_Validating);
            this.txtDACChannelGain.Validated += new System.EventHandler(this.txtDACChannelGain_Validated);
            // 
            // lblDACChannelGain
            // 
            this.lblDACChannelGain.AutoSize = true;
            this.lblDACChannelGain.Location = new System.Drawing.Point(26, 100);
            this.lblDACChannelGain.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDACChannelGain.Name = "lblDACChannelGain";
            this.lblDACChannelGain.Size = new System.Drawing.Size(57, 25);
            this.lblDACChannelGain.TabIndex = 17;
            this.lblDACChannelGain.Text = "Gain";
            // 
            // rdoDataValueA
            // 
            this.rdoDataValueA.AutoSize = true;
            this.rdoDataValueA.Checked = true;
            this.rdoDataValueA.Location = new System.Drawing.Point(226, 50);
            this.rdoDataValueA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdoDataValueA.Name = "rdoDataValueA";
            this.rdoDataValueA.Size = new System.Drawing.Size(27, 26);
            this.rdoDataValueA.TabIndex = 50;
            this.rdoDataValueA.TabStop = true;
            this.rdoDataValueA.UseVisualStyleBackColor = true;
            this.rdoDataValueA.Click += new System.EventHandler(this.rdoDataValueA_Click);
            // 
            // txtDataValueA
            // 
            this.txtDataValueA.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDataValueA.Location = new System.Drawing.Point(406, 42);
            this.txtDataValueA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtDataValueA.MaxLength = 4;
            this.txtDataValueA.Name = "txtDataValueA";
            this.txtDataValueA.Size = new System.Drawing.Size(56, 31);
            this.txtDataValueA.TabIndex = 51;
            this.txtDataValueA.Text = "FFFF";
            this.txtDataValueA.Click += new System.EventHandler(this.txtDataValueA_Click);
            this.txtDataValueA.Validating += new System.ComponentModel.CancelEventHandler(this.txtDataValueA_Validating);
            this.txtDataValueA.Validated += new System.EventHandler(this.txtDataValueA_Validated);
            // 
            // lblDataValueA
            // 
            this.lblDataValueA.AutoSize = true;
            this.lblDataValueA.Location = new System.Drawing.Point(254, 50);
            this.lblDataValueA.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDataValueA.Name = "lblDataValueA";
            this.lblDataValueA.Size = new System.Drawing.Size(138, 25);
            this.lblDataValueA.TabIndex = 19;
            this.lblDataValueA.Text = "Data Value A";
            // 
            // lblDataValueB
            // 
            this.lblDataValueB.AutoSize = true;
            this.lblDataValueB.Location = new System.Drawing.Point(254, 100);
            this.lblDataValueB.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDataValueB.Name = "lblDataValueB";
            this.lblDataValueB.Size = new System.Drawing.Size(138, 25);
            this.lblDataValueB.TabIndex = 21;
            this.lblDataValueB.Text = "Data Value B";
            // 
            // txtDataValueB
            // 
            this.txtDataValueB.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDataValueB.Location = new System.Drawing.Point(408, 94);
            this.txtDataValueB.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtDataValueB.MaxLength = 4;
            this.txtDataValueB.Name = "txtDataValueB";
            this.txtDataValueB.Size = new System.Drawing.Size(56, 31);
            this.txtDataValueB.TabIndex = 53;
            this.txtDataValueB.Text = "0";
            this.txtDataValueB.Click += new System.EventHandler(this.txtDataValueB_Click);
            this.txtDataValueB.Validating += new System.ComponentModel.CancelEventHandler(this.txtDataValueB_Validating);
            this.txtDataValueB.Validated += new System.EventHandler(this.txtDataValueB_Validated);
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(36, 29);
            this.lblDevice.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(78, 25);
            this.lblDevice.TabIndex = 38;
            this.lblDevice.Text = "Device";
            // 
            // cboDevices
            // 
            this.cboDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDevices.FormattingEnabled = true;
            this.cboDevices.Location = new System.Drawing.Point(130, 23);
            this.cboDevices.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cboDevices.Name = "cboDevices";
            this.cboDevices.Size = new System.Drawing.Size(1078, 33);
            this.cboDevices.TabIndex = 0;
            this.cboDevices.SelectedIndexChanged += new System.EventHandler(this.cboDevices_SelectedIndexChanged);
            // 
            // gbDeviceOptions
            // 
            this.gbDeviceOptions.Controls.Add(this.lblTemperatureStatus);
            this.gbDeviceOptions.Controls.Add(this.txtOverTempStatus);
            this.gbDeviceOptions.Controls.Add(this.chkOverTempShutdownEnabled);
            this.gbDeviceOptions.Controls.Add(this.lblVREF0);
            this.gbDeviceOptions.Controls.Add(this.btnReset);
            this.gbDeviceOptions.Controls.Add(this.txtVREF0);
            this.gbDeviceOptions.Controls.Add(this.btnPerformSoftPowerDown);
            this.gbDeviceOptions.Controls.Add(this.lblVREF1);
            this.gbDeviceOptions.Controls.Add(this.btnSoftPowerUp);
            this.gbDeviceOptions.Controls.Add(this.txtVREF1);
            this.gbDeviceOptions.Controls.Add(this.txtOffsetDAC0);
            this.gbDeviceOptions.Controls.Add(this.lblGroup1Offset);
            this.gbDeviceOptions.Controls.Add(this.lblOffsetDAC0);
            this.gbDeviceOptions.Controls.Add(this.txtOffsetDAC1);
            this.gbDeviceOptions.Location = new System.Drawing.Point(36, 73);
            this.gbDeviceOptions.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDeviceOptions.Name = "gbDeviceOptions";
            this.gbDeviceOptions.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDeviceOptions.Size = new System.Drawing.Size(1176, 185);
            this.gbDeviceOptions.TabIndex = 26;
            this.gbDeviceOptions.TabStop = false;
            this.gbDeviceOptions.Text = "Device Options";
            // 
            // lblTemperatureStatus
            // 
            this.lblTemperatureStatus.AutoSize = true;
            this.lblTemperatureStatus.Location = new System.Drawing.Point(522, 137);
            this.lblTemperatureStatus.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTemperatureStatus.Name = "lblTemperatureStatus";
            this.lblTemperatureStatus.Size = new System.Drawing.Size(201, 25);
            this.lblTemperatureStatus.TabIndex = 38;
            this.lblTemperatureStatus.Text = "Temperature Status";
            // 
            // txtOverTempStatus
            // 
            this.txtOverTempStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOverTempStatus.Location = new System.Drawing.Point(734, 131);
            this.txtOverTempStatus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtOverTempStatus.Name = "txtOverTempStatus";
            this.txtOverTempStatus.ReadOnly = true;
            this.txtOverTempStatus.Size = new System.Drawing.Size(94, 32);
            this.txtOverTempStatus.TabIndex = 37;
            this.txtOverTempStatus.TabStop = false;
            this.txtOverTempStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chkOverTempShutdownEnabled
            // 
            this.chkOverTempShutdownEnabled.AutoSize = true;
            this.chkOverTempShutdownEnabled.Location = new System.Drawing.Point(36, 135);
            this.chkOverTempShutdownEnabled.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkOverTempShutdownEnabled.Name = "chkOverTempShutdownEnabled";
            this.chkOverTempShutdownEnabled.Size = new System.Drawing.Size(455, 29);
            this.chkOverTempShutdownEnabled.TabIndex = 36;
            this.chkOverTempShutdownEnabled.Text = "Over-Temperature Auto Shutdown Enabled";
            this.chkOverTempShutdownEnabled.UseVisualStyleBackColor = true;
            this.chkOverTempShutdownEnabled.Click += new System.EventHandler(this.chkOverTempShutdownEnabled_Click);
            // 
            // lblVREF0
            // 
            this.lblVREF0.AutoSize = true;
            this.lblVREF0.Location = new System.Drawing.Point(30, 35);
            this.lblVREF0.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblVREF0.Name = "lblVREF0";
            this.lblVREF0.Size = new System.Drawing.Size(86, 25);
            this.lblVREF0.TabIndex = 22;
            this.lblVREF0.Text = "VREF 0";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(844, 29);
            this.btnReset.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(308, 38);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtVREF0
            // 
            this.txtVREF0.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVREF0.Location = new System.Drawing.Point(130, 29);
            this.txtVREF0.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtVREF0.MaxLength = 5;
            this.txtVREF0.Name = "txtVREF0";
            this.txtVREF0.Size = new System.Drawing.Size(76, 31);
            this.txtVREF0.TabIndex = 1;
            this.txtVREF0.Text = "5";
            this.txtVREF0.Click += new System.EventHandler(this.txtVREF0_Click);
            this.txtVREF0.Validating += new System.ComponentModel.CancelEventHandler(this.txtVREF0_Validating);
            this.txtVREF0.Validated += new System.EventHandler(this.txtVREF0_Validated);
            // 
            // btnPerformSoftPowerDown
            // 
            this.btnPerformSoftPowerDown.Location = new System.Drawing.Point(524, 77);
            this.btnPerformSoftPowerDown.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnPerformSoftPowerDown.Name = "btnPerformSoftPowerDown";
            this.btnPerformSoftPowerDown.Size = new System.Drawing.Size(308, 38);
            this.btnPerformSoftPowerDown.TabIndex = 6;
            this.btnPerformSoftPowerDown.Text = "Perform Soft Power Down";
            this.btnPerformSoftPowerDown.UseVisualStyleBackColor = true;
            this.btnPerformSoftPowerDown.Click += new System.EventHandler(this.btnPerformSoftPowerDown_Click);
            // 
            // lblVREF1
            // 
            this.lblVREF1.AutoSize = true;
            this.lblVREF1.Location = new System.Drawing.Point(30, 85);
            this.lblVREF1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblVREF1.Name = "lblVREF1";
            this.lblVREF1.Size = new System.Drawing.Size(86, 25);
            this.lblVREF1.TabIndex = 24;
            this.lblVREF1.Text = "VREF 1";
            // 
            // btnSoftPowerUp
            // 
            this.btnSoftPowerUp.Location = new System.Drawing.Point(524, 27);
            this.btnSoftPowerUp.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnSoftPowerUp.Name = "btnSoftPowerUp";
            this.btnSoftPowerUp.Size = new System.Drawing.Size(308, 38);
            this.btnSoftPowerUp.TabIndex = 5;
            this.btnSoftPowerUp.Text = "Perform Soft Power Up";
            this.btnSoftPowerUp.UseVisualStyleBackColor = true;
            this.btnSoftPowerUp.Click += new System.EventHandler(this.btnSoftPowerUp_Click);
            // 
            // txtVREF1
            // 
            this.txtVREF1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVREF1.Location = new System.Drawing.Point(130, 79);
            this.txtVREF1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtVREF1.MaxLength = 5;
            this.txtVREF1.Name = "txtVREF1";
            this.txtVREF1.Size = new System.Drawing.Size(76, 31);
            this.txtVREF1.TabIndex = 2;
            this.txtVREF1.Text = "5";
            this.txtVREF1.Click += new System.EventHandler(this.txtVREF1_Click);
            this.txtVREF1.Validating += new System.ComponentModel.CancelEventHandler(this.txtVREF1_Validating);
            this.txtVREF1.Validated += new System.EventHandler(this.txtVREF1_Validated);
            // 
            // txtOffsetDAC0
            // 
            this.txtOffsetDAC0.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtOffsetDAC0.Location = new System.Drawing.Point(406, 29);
            this.txtOffsetDAC0.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtOffsetDAC0.MaxLength = 4;
            this.txtOffsetDAC0.Name = "txtOffsetDAC0";
            this.txtOffsetDAC0.Size = new System.Drawing.Size(56, 31);
            this.txtOffsetDAC0.TabIndex = 3;
            this.txtOffsetDAC0.Text = "2000";
            this.txtOffsetDAC0.Click += new System.EventHandler(this.txtOffsetDAC0_Click);
            this.txtOffsetDAC0.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffsetDAC0_Validating);
            this.txtOffsetDAC0.Validated += new System.EventHandler(this.txtOffsetDAC0_Validated);
            // 
            // lblGroup1Offset
            // 
            this.lblGroup1Offset.AutoSize = true;
            this.lblGroup1Offset.Location = new System.Drawing.Point(244, 85);
            this.lblGroup1Offset.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblGroup1Offset.Name = "lblGroup1Offset";
            this.lblGroup1Offset.Size = new System.Drawing.Size(137, 25);
            this.lblGroup1Offset.TabIndex = 35;
            this.lblGroup1Offset.Text = "Offset DAC 1";
            // 
            // lblOffsetDAC0
            // 
            this.lblOffsetDAC0.AutoSize = true;
            this.lblOffsetDAC0.Location = new System.Drawing.Point(244, 35);
            this.lblOffsetDAC0.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblOffsetDAC0.Name = "lblOffsetDAC0";
            this.lblOffsetDAC0.Size = new System.Drawing.Size(137, 25);
            this.lblOffsetDAC0.TabIndex = 33;
            this.lblOffsetDAC0.Text = "Offset DAC 0";
            // 
            // txtOffsetDAC1
            // 
            this.txtOffsetDAC1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtOffsetDAC1.Location = new System.Drawing.Point(404, 79);
            this.txtOffsetDAC1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtOffsetDAC1.MaxLength = 4;
            this.txtOffsetDAC1.Name = "txtOffsetDAC1";
            this.txtOffsetDAC1.Size = new System.Drawing.Size(58, 31);
            this.txtOffsetDAC1.TabIndex = 4;
            this.txtOffsetDAC1.Text = "2000";
            this.txtOffsetDAC1.Click += new System.EventHandler(this.txtOffsetDAC1_Click);
            this.txtOffsetDAC1.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffsetDAC1_Validating);
            this.txtOffsetDAC1.Validated += new System.EventHandler(this.txtOffsetDAC1_Validated);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnUpdateAllDACOutputs;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 775);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "MainForm";
            this.Text = "Analog Devices AD53xx DenseDAC Eval Board Test Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.gbDACOutputs.ResumeLayout(false);
            this.gbDACOutputs.PerformLayout();
            this.gbDataValues.ResumeLayout(false);
            this.gbDataValues.PerformLayout();
            this.gbDeviceOptions.ResumeLayout(false);
            this.gbDeviceOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.RadioButton rdoDataValueB;
        private System.Windows.Forms.RadioButton rdoDataValueA;
        private System.Windows.Forms.Label lblDataValueB;
        private System.Windows.Forms.TextBox txtDataValueB;
        private System.Windows.Forms.Label lblDataValueA;
        private System.Windows.Forms.TextBox txtDataValueA;
        private System.Windows.Forms.Label lblDACChannelGain;
        private System.Windows.Forms.TextBox txtDACChannelGain;
        private System.Windows.Forms.Label lblDACChannelOffset;
        private System.Windows.Forms.TextBox txtDACChannelOffset;
        private System.Windows.Forms.RadioButton rdoDAC39;
        private System.Windows.Forms.RadioButton rdoDAC38;
        private System.Windows.Forms.RadioButton rdoDAC37;
        private System.Windows.Forms.RadioButton rdoDAC36;
        private System.Windows.Forms.RadioButton rdoDAC35;
        private System.Windows.Forms.RadioButton rdoDAC34;
        private System.Windows.Forms.RadioButton rdoDAC33;
        private System.Windows.Forms.RadioButton rdoDAC32;
        private System.Windows.Forms.Button btnUpdateAllDACOutputs;
        private System.Windows.Forms.RadioButton rdoDAC31;
        private System.Windows.Forms.RadioButton rdoDAC30;
        private System.Windows.Forms.RadioButton rdoDAC29;
        private System.Windows.Forms.RadioButton rdoDAC28;
        private System.Windows.Forms.RadioButton rdoDAC27;
        private System.Windows.Forms.RadioButton rdoDAC26;
        private System.Windows.Forms.RadioButton rdoDAC25;
        private System.Windows.Forms.RadioButton rdoDAC24;
        private System.Windows.Forms.RadioButton rdoDAC23;
        private System.Windows.Forms.RadioButton rdoDAC22;
        private System.Windows.Forms.RadioButton rdoDAC21;
        private System.Windows.Forms.RadioButton rdoDAC20;
        private System.Windows.Forms.RadioButton rdoDAC19;
        private System.Windows.Forms.RadioButton rdoDAC18;
        private System.Windows.Forms.RadioButton rdoDAC17;
        private System.Windows.Forms.RadioButton rdoDAC16;
        private System.Windows.Forms.Button btnResumeAllDACOutputs;
        private System.Windows.Forms.RadioButton rdoDAC15;
        private System.Windows.Forms.RadioButton rdoDAC14;
        private System.Windows.Forms.RadioButton rdoDAC13;
        private System.Windows.Forms.RadioButton rdoDAC12;
        private System.Windows.Forms.RadioButton rdoDAC11;
        private System.Windows.Forms.RadioButton rdoDAC10;
        private System.Windows.Forms.RadioButton rdoDAC9;
        private System.Windows.Forms.RadioButton rdoDAC8;
        private System.Windows.Forms.Button btnSuspendAllDACOutputs;
        private System.Windows.Forms.GroupBox gbDACOutputs;
        private System.Windows.Forms.RadioButton rdoDAC7;
        private System.Windows.Forms.RadioButton rdoDAC6;
        private System.Windows.Forms.RadioButton rdoDAC5;
        private System.Windows.Forms.RadioButton rdoDAC4;
        private System.Windows.Forms.RadioButton rdoDAC3;
        private System.Windows.Forms.RadioButton rdoDAC2;
        private System.Windows.Forms.RadioButton rdoDAC1;
        private System.Windows.Forms.RadioButton rdoDAC0;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblGroup1Offset;
        private System.Windows.Forms.TextBox txtOffsetDAC1;
        private System.Windows.Forms.Label lblOffsetDAC0;
        private System.Windows.Forms.Button btnPerformSoftPowerDown;
        private System.Windows.Forms.TextBox txtOffsetDAC0;
        private System.Windows.Forms.TextBox txtVREF1;
        private System.Windows.Forms.Button btnSoftPowerUp;
        private System.Windows.Forms.Label lblVREF1;
        private System.Windows.Forms.TextBox txtVREF0;
        private System.Windows.Forms.Label lblVREF0;
        private System.Windows.Forms.ComboBox cboDevices;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblVout;
        private System.Windows.Forms.GroupBox gbDeviceOptions;
        private System.Windows.Forms.GroupBox gbDataValues;
        private System.Windows.Forms.TextBox txtVoutCalculated;
        private System.Windows.Forms.Label lblTemperatureStatus;
        private System.Windows.Forms.TextBox txtOverTempStatus;
        private System.Windows.Forms.CheckBox chkOverTempShutdownEnabled;

    }
}

