using System.Collections.Generic;
using System.Windows.Forms;
using SimLinkup.Signals;

namespace SimLinkup.UI
{
    public partial class frmMappings : Form
    {
        private IEnumerable<SignalMapping> _mappings = new List<SignalMapping>();

        public frmMappings()
        {
            InitializeComponent();
        }

        public IEnumerable<SignalMapping> Mappings
        {
            get => _mappings;
            set
            {
                _mappings = value;
                RefreshMappings();
            }
        }

        private void RefreshMappings()
        {
            SignalsList.SuspendLayout();
            SignalsList.Items.Clear();
            foreach (var mapping in Mappings)
                SignalsList.Items.Add(
                    new ListViewItem(new[]
                    {
                        mapping.Source.SourceFriendlyName,
                        mapping.Source.FriendlyName,
                        mapping.Destination.SourceFriendlyName,
                        mapping.Destination.FriendlyName
                    }));
            SignalsList.ResumeLayout(true);
        }
    }
}