using System.ComponentModel;
using System.Windows.Forms;

namespace SimLinkup.UI.UserControls
{
    partial class ModuleBasicVisualizer
    {
 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

 
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

        #region Component Designer generated code

 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ModuleBasicVisualizer));
            this.panel = new System.Windows.Forms.Panel();
            this.btnShowSignals = new System.Windows.Forms.Button();
            this.lblModuleName = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.btnShowSignals);
            this.panel.Controls.Add(this.lblModuleName);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(400, 20);
            this.panel.TabIndex = 0;
            // 
            // btnShowSignals
            // 
            this.btnShowSignals.Image = ((System.Drawing.Image)(resources.GetObject("btnShowSignals.Image")));
            this.btnShowSignals.Location = new System.Drawing.Point(0, 0);
            this.btnShowSignals.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowSignals.Name = "btnShowSignals";
            this.btnShowSignals.Size = new System.Drawing.Size(20, 20);
            this.btnShowSignals.TabIndex = 1;
            this.btnShowSignals.UseVisualStyleBackColor = true;
            this.btnShowSignals.Click += new System.EventHandler(this.btnShowSignals_Click);
            // 
            // lblModuleName
            // 
            this.lblModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModuleName.AutoSize = true;
            this.lblModuleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModuleName.Location = new System.Drawing.Point(28, 1);
            this.lblModuleName.Name = "lblModuleName";
            this.lblModuleName.Size = new System.Drawing.Size(106, 17);
            this.lblModuleName.TabIndex = 0;
            this.lblModuleName.Text = "Module Name";
            // 
            // ModuleBasicVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "ModuleBasicVisualizer";
            this.Size = new System.Drawing.Size(400, 20);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panel;
        private Button btnShowSignals;
        private Label lblModuleName;
    }
}
