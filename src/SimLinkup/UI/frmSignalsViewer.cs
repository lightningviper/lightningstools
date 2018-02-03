using System;
using System.Windows.Forms;
using Common.MacroProgramming;

namespace SimLinkup.UI
{
    public partial class frmSignalsViewer : Form
    {
        public frmSignalsViewer()
        {
            InitializeComponent();
        }

        public SignalList<Signal> Signals
        {
            get => signalsView.Signals;
            set => signalsView.Signals = value;
        }

        protected override void OnShown(EventArgs e)
        {
            signalsView.UpdateContents();
            base.OnShown(e);
        }
    }
}