using System;
using System.Windows.Forms;

namespace SimLinkup.UI.UserControls
{
    public partial class ModuleBasicVisualizer : UserControl
    {
        public ModuleBasicVisualizer()
        {
            InitializeComponent();
        }

        public string ModuleName
        {
            get => lblModuleName.Text;
            set => lblModuleName.Text = value;
        }

        public event EventHandler ShowSignals;

        private void btnShowSignals_Click(object sender, EventArgs e)
        {
            ShowSignals?.Invoke(this, null);
        }
    }
}