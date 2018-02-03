using System.ComponentModel;
using System.Windows.Forms;

namespace SimLinkup.UI.UserControls
{
    partial class SignalsView
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
            this.tvSignals = new System.Windows.Forms.TreeView();
            this.lvSignals = new System.Windows.Forms.ListView();
            this.Source = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbVisualization = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualization)).BeginInit();
            this.SuspendLayout();
            // 
            // tvSignals
            // 
            this.tvSignals.FullRowSelect = true;
            this.tvSignals.Location = new System.Drawing.Point(4, 5);
            this.tvSignals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tvSignals.Name = "tvSignals";
            this.tvSignals.Size = new System.Drawing.Size(596, 911);
            this.tvSignals.TabIndex = 3;
            this.tvSignals.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSignals_AfterSelect);
            // 
            // lvSignals
            // 
            this.lvSignals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Source});
            this.lvSignals.FullRowSelect = true;
            this.lvSignals.LabelWrap = false;
            this.lvSignals.Location = new System.Drawing.Point(608, 5);
            this.lvSignals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvSignals.MultiSelect = false;
            this.lvSignals.Name = "lvSignals";
            this.lvSignals.Size = new System.Drawing.Size(606, 911);
            this.lvSignals.TabIndex = 2;
            this.lvSignals.UseCompatibleStateImageBehavior = false;
            this.lvSignals.View = System.Windows.Forms.View.Details;
            this.lvSignals.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvSignals_ColumnClick);
            // 
            // pbSignalGraph
            // 
            this.pbVisualization.BackColor = System.Drawing.SystemColors.Control;
            this.pbVisualization.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbVisualization.Location = new System.Drawing.Point(1221, 5);
            this.pbVisualization.Name = "pbSignalGraph";
            this.pbVisualization.Size = new System.Drawing.Size(603, 600);
            this.pbVisualization.TabIndex = 0;
            this.pbVisualization.TabStop = false;
            // 
            // SignalsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pbVisualization);
            this.Controls.Add(this.lvSignals);
            this.Controls.Add(this.tvSignals);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SignalsView";
            this.Size = new System.Drawing.Size(1833, 921);
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualization)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TreeView tvSignals;
        private ListView lvSignals;
        private ColumnHeader Source;
        private PictureBox pbVisualization;
    }
}
