using System.ComponentModel;
using SimLinkup.UI.UserControls;

namespace SimLinkup.UI
{
    partial class frmSignalsViewer
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

        #region Windows Form Designer generated code


        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.signalsView = new SimLinkup.UI.UserControls.SignalsView();
            this.SuspendLayout();
            // 
            // signalsView
            // 
            this.signalsView.AutoSize = true;
            this.signalsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalsView.Location = new System.Drawing.Point(0, 0);
            this.signalsView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.signalsView.Name = "signalsView";
            this.signalsView.Signals = null;
            this.signalsView.Size = new System.Drawing.Size(1847, 954);
            this.signalsView.TabIndex = 0;
            // 
            // frmSignalsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1847, 954);
            this.Controls.Add(this.signalsView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSignalsViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Signals";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SignalsView signalsView;
    }
}