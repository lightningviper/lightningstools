namespace MFDExtractor.UI
{
    partial class frmBMSOptions
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
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this._errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.dlgBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.grpSharedMemOptions = new System.Windows.Forms.GroupBox();
            this.chkEnable3DModeExtraction = new System.Windows.Forms.CheckBox();
            this.grpExportedImagesColorDepth = new System.Windows.Forms.GroupBox();
            this.rdoUsePrimaryColorDepth = new System.Windows.Forms.RadioButton();
            this.rdoThirtyTwoBit = new System.Windows.Forms.RadioButton();
            this.rdoTwentyFourBit = new System.Windows.Forms.RadioButton();
            this.rdoSixteenBit = new System.Windows.Forms.RadioButton();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.lblFrames = new System.Windows.Forms.Label();
            this.udBatchSize = new System.Windows.Forms.NumericUpDown();
            this.pnlSharedMemOptions = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblBmsInstallationPath = new System.Windows.Forms.Label();
            this.txtBmsInstallationPath = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._errProvider)).BeginInit();
            this.grpSharedMemOptions.SuspendLayout();
            this.grpExportedImagesColorDepth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udBatchSize)).BeginInit();
            this.pnlSharedMemOptions.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Enabled = false;
            this.cmdOk.Location = new System.Drawing.Point(36, 10);
            this.cmdOk.Margin = new System.Windows.Forms.Padding(6);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(174, 48);
            this.cmdOk.TabIndex = 8;
            this.cmdOk.Text = "&OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(222, 10);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(6);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(170, 48);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // _errProvider
            // 
            this._errProvider.ContainerControl = this;
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.dlgBrowse.ShowNewFolderButton = false;
            // 
            // grpSharedMemOptions
            // 
            this.grpSharedMemOptions.Controls.Add(this.chkEnable3DModeExtraction);
            this.grpSharedMemOptions.Controls.Add(this.grpExportedImagesColorDepth);
            this.grpSharedMemOptions.Controls.Add(this.lblBatchSize);
            this.grpSharedMemOptions.Controls.Add(this.lblFrames);
            this.grpSharedMemOptions.Controls.Add(this.udBatchSize);
            this.grpSharedMemOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSharedMemOptions.Enabled = false;
            this.grpSharedMemOptions.Location = new System.Drawing.Point(0, 0);
            this.grpSharedMemOptions.Margin = new System.Windows.Forms.Padding(6);
            this.grpSharedMemOptions.Name = "grpSharedMemOptions";
            this.grpSharedMemOptions.Padding = new System.Windows.Forms.Padding(6);
            this.grpSharedMemOptions.Size = new System.Drawing.Size(1000, 365);
            this.grpSharedMemOptions.TabIndex = 20;
            this.grpSharedMemOptions.TabStop = false;
            this.grpSharedMemOptions.Text = "Shared Memory Options";
            // 
            // chkEnable3DModeExtraction
            // 
            this.chkEnable3DModeExtraction.AutoSize = true;
            this.chkEnable3DModeExtraction.Location = new System.Drawing.Point(36, 37);
            this.chkEnable3DModeExtraction.Margin = new System.Windows.Forms.Padding(6);
            this.chkEnable3DModeExtraction.Name = "chkEnable3DModeExtraction";
            this.chkEnable3DModeExtraction.Size = new System.Drawing.Size(696, 29);
            this.chkEnable3DModeExtraction.TabIndex = 2;
            this.chkEnable3DModeExtraction.Text = "Enable exporting of 3D cockpit instrument images to shared memory";
            this.chkEnable3DModeExtraction.UseVisualStyleBackColor = true;
            // 
            // grpExportedImagesColorDepth
            // 
            this.grpExportedImagesColorDepth.Controls.Add(this.rdoUsePrimaryColorDepth);
            this.grpExportedImagesColorDepth.Controls.Add(this.rdoThirtyTwoBit);
            this.grpExportedImagesColorDepth.Controls.Add(this.rdoTwentyFourBit);
            this.grpExportedImagesColorDepth.Controls.Add(this.rdoSixteenBit);
            this.grpExportedImagesColorDepth.Location = new System.Drawing.Point(36, 131);
            this.grpExportedImagesColorDepth.Margin = new System.Windows.Forms.Padding(6);
            this.grpExportedImagesColorDepth.Name = "grpExportedImagesColorDepth";
            this.grpExportedImagesColorDepth.Padding = new System.Windows.Forms.Padding(6);
            this.grpExportedImagesColorDepth.Size = new System.Drawing.Size(948, 215);
            this.grpExportedImagesColorDepth.TabIndex = 7;
            this.grpExportedImagesColorDepth.TabStop = false;
            this.grpExportedImagesColorDepth.Text = "Color depth for exported images";
            // 
            // rdoUsePrimaryColorDepth
            // 
            this.rdoUsePrimaryColorDepth.AutoSize = true;
            this.rdoUsePrimaryColorDepth.Location = new System.Drawing.Point(24, 38);
            this.rdoUsePrimaryColorDepth.Margin = new System.Windows.Forms.Padding(6);
            this.rdoUsePrimaryColorDepth.Name = "rdoUsePrimaryColorDepth";
            this.rdoUsePrimaryColorDepth.Size = new System.Drawing.Size(460, 29);
            this.rdoUsePrimaryColorDepth.TabIndex = 4;
            this.rdoUsePrimaryColorDepth.Text = "Use primary monitor\'s color depth ( default )";
            this.rdoUsePrimaryColorDepth.UseVisualStyleBackColor = true;
            // 
            // rdoThirtyTwoBit
            // 
            this.rdoThirtyTwoBit.AutoSize = true;
            this.rdoThirtyTwoBit.Location = new System.Drawing.Point(24, 171);
            this.rdoThirtyTwoBit.Margin = new System.Windows.Forms.Padding(6);
            this.rdoThirtyTwoBit.Name = "rdoThirtyTwoBit";
            this.rdoThirtyTwoBit.Size = new System.Drawing.Size(314, 29);
            this.rdoThirtyTwoBit.TabIndex = 7;
            this.rdoThirtyTwoBit.Text = "32-bit color depth ( slowest )";
            this.rdoThirtyTwoBit.UseVisualStyleBackColor = true;
            // 
            // rdoTwentyFourBit
            // 
            this.rdoTwentyFourBit.AutoSize = true;
            this.rdoTwentyFourBit.Location = new System.Drawing.Point(24, 127);
            this.rdoTwentyFourBit.Margin = new System.Windows.Forms.Padding(6);
            this.rdoTwentyFourBit.Name = "rdoTwentyFourBit";
            this.rdoTwentyFourBit.Size = new System.Drawing.Size(210, 29);
            this.rdoTwentyFourBit.TabIndex = 6;
            this.rdoTwentyFourBit.Text = "24-bit color depth";
            this.rdoTwentyFourBit.UseVisualStyleBackColor = true;
            // 
            // rdoSixteenBit
            // 
            this.rdoSixteenBit.AutoSize = true;
            this.rdoSixteenBit.Checked = true;
            this.rdoSixteenBit.Location = new System.Drawing.Point(24, 83);
            this.rdoSixteenBit.Margin = new System.Windows.Forms.Padding(6);
            this.rdoSixteenBit.Name = "rdoSixteenBit";
            this.rdoSixteenBit.Size = new System.Drawing.Size(454, 29);
            this.rdoSixteenBit.TabIndex = 5;
            this.rdoSixteenBit.TabStop = true;
            this.rdoSixteenBit.Text = "16-bit color depth ( fastest, recommended )";
            this.rdoSixteenBit.UseVisualStyleBackColor = true;
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(30, 85);
            this.lblBatchSize.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(514, 25);
            this.lblBatchSize.TabIndex = 5;
            this.lblBatchSize.Text = "Export a new set of images to shared memory, every";
            // 
            // lblFrames
            // 
            this.lblFrames.AutoSize = true;
            this.lblFrames.Location = new System.Drawing.Point(652, 85);
            this.lblFrames.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(135, 25);
            this.lblFrames.TabIndex = 6;
            this.lblFrames.Text = "video frames";
            // 
            // udBatchSize
            // 
            this.udBatchSize.Location = new System.Drawing.Point(570, 81);
            this.udBatchSize.Margin = new System.Windows.Forms.Padding(6);
            this.udBatchSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBatchSize.Name = "udBatchSize";
            this.udBatchSize.Size = new System.Drawing.Size(70, 31);
            this.udBatchSize.TabIndex = 3;
            this.udBatchSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // pnlSharedMemOptions
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pnlSharedMemOptions, 2);
            this.pnlSharedMemOptions.Controls.Add(this.grpSharedMemOptions);
            this.pnlSharedMemOptions.Location = new System.Drawing.Point(6, 81);
            this.pnlSharedMemOptions.Margin = new System.Windows.Forms.Padding(6);
            this.pnlSharedMemOptions.Name = "pnlSharedMemOptions";
            this.pnlSharedMemOptions.Size = new System.Drawing.Size(1000, 365);
            this.pnlSharedMemOptions.TabIndex = 22;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlButtons.AutoSize = true;
            this.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlButtons.Controls.Add(this.cmdCancel);
            this.pnlButtons.Controls.Add(this.cmdOk);
            this.pnlButtons.Location = new System.Drawing.Point(6, 458);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(6);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(398, 65);
            this.pnlButtons.TabIndex = 24;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.pnlButtons, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pnlSharedMemOptions, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pnlHeader, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1012, 529);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // pnlHeader
            // 
            this.pnlHeader.AutoSize = true;
            this.pnlHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.pnlHeader, 2);
            this.pnlHeader.Controls.Add(this.lblBmsInstallationPath);
            this.pnlHeader.Controls.Add(this.txtBmsInstallationPath);
            this.pnlHeader.Controls.Add(this.cmdBrowse);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(6, 6);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(6);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 63);
            this.pnlHeader.TabIndex = 21;
            // 
            // lblBmsInstallationPath
            // 
            this.lblBmsInstallationPath.AutoSize = true;
            this.lblBmsInstallationPath.Location = new System.Drawing.Point(6, 23);
            this.lblBmsInstallationPath.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBmsInstallationPath.Name = "lblBmsInstallationPath";
            this.lblBmsInstallationPath.Size = new System.Drawing.Size(240, 25);
            this.lblBmsInstallationPath.TabIndex = 3;
            this.lblBmsInstallationPath.Text = "BMS Installation Folder:";
            // 
            // txtBmsInstallationPath
            // 
            this.txtBmsInstallationPath.Enabled = false;
            this.txtBmsInstallationPath.Location = new System.Drawing.Point(366, 17);
            this.txtBmsInstallationPath.Margin = new System.Windows.Forms.Padding(6);
            this.txtBmsInstallationPath.Name = "txtBmsInstallationPath";
            this.txtBmsInstallationPath.Size = new System.Drawing.Size(458, 31);
            this.txtBmsInstallationPath.TabIndex = 0;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(840, 17);
            this.cmdBrowse.Margin = new System.Windows.Forms.Padding(6);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(142, 40);
            this.cmdBrowse.TabIndex = 1;
            this.cmdBrowse.Text = "&Browse...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // frmBMSOptions
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(1012, 529);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBMSOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BMS Advanced Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmBMSOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this._errProvider)).EndInit();
            this.grpSharedMemOptions.ResumeLayout(false);
            this.grpSharedMemOptions.PerformLayout();
            this.grpExportedImagesColorDepth.ResumeLayout(false);
            this.grpExportedImagesColorDepth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udBatchSize)).EndInit();
            this.pnlSharedMemOptions.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ErrorProvider _errProvider;
        private System.Windows.Forms.FolderBrowserDialog dlgBrowse;
        private System.Windows.Forms.GroupBox grpSharedMemOptions;
        private System.Windows.Forms.CheckBox chkEnable3DModeExtraction;
        private System.Windows.Forms.GroupBox grpExportedImagesColorDepth;
        private System.Windows.Forms.RadioButton rdoUsePrimaryColorDepth;
        private System.Windows.Forms.RadioButton rdoThirtyTwoBit;
        private System.Windows.Forms.RadioButton rdoTwentyFourBit;
        private System.Windows.Forms.RadioButton rdoSixteenBit;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.Label lblFrames;
        private System.Windows.Forms.NumericUpDown udBatchSize;
        private System.Windows.Forms.Panel pnlSharedMemOptions;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblBmsInstallationPath;
        private System.Windows.Forms.TextBox txtBmsInstallationPath;
        private System.Windows.Forms.Button cmdBrowse;
    }
}