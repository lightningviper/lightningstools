using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.UI;
using Common.UI.UserControls;
using SimLinkup.Scripting;
using Timer = System.Windows.Forms.Timer;

namespace SimLinkup.UI.UserControls
{
    public partial class SignalsView : UserControl
    {
        private readonly ListViewColumnSorter _columnSorter;

        public SignalsView()
        {
            InitializeComponent();
            lvSignals.DoubleBuffered(true);
            _columnSorter = new ListViewColumnSorter {SortColumn = 0};
            lvSignals.ListViewItemSorter = _columnSorter;
            var timer = new Timer {Interval = 30};
            timer.Tick += (s, e) =>
            {
                if (UpdateInRealtime)
                {
                    pbVisualization.Invalidate();
                }
            };
            timer.Enabled = true;
            pbVisualization.Paint += SignalsView_Paint;
        }

        public bool UpdateInRealtime { get; set; } = true;

        public ScriptingContext ScriptingContext { get; set; }
        public SignalList<Signal> Signals { get; set; }

        private IHardwareSupportModule SelectedHardwareSupportModule => HardwareSupportModuleFor(tvSignals
            .SelectedNode);

        private ListViewItem SelectedListViewItem => lvSignals.SelectedItems.Count > 0
            ? lvSignals.SelectedItems[0]
            : null;

        private Signal SelectedSignal => SignalFor(SelectedListViewItem);

        public void UpdateContents()
        {
            BuildTreeView();
        }

        private void BuildTreeView()
        {
            tvSignals.BeginUpdate();
            tvSignals.Nodes.Clear();
            var distinctSignalSources = Signals?.GetDistinctSignalSourceNames().OrderBy(x => x).ToList();
            if (distinctSignalSources != null && distinctSignalSources.Count > 0)
            {
                foreach (var signalSource in distinctSignalSources)
                {
                    var signalSourceTreeNode = new TreeNode(signalSource)
                    {
                        Tag = "SOURCE:" + signalSource
                    };
                    tvSignals.Nodes.Add(signalSourceTreeNode);

                    var signalsThisSource =
                        Signals.GetSignalsFromSource(signalSource);

                    var signalCategoriesThisSource = signalsThisSource.GetDistinctSignalCategories()
                        .OrderBy(x => x)
                        .ToList();
                    foreach (var signalCategory in signalCategoriesThisSource)
                    {
                        var signalCategoryTreeNode = new TreeNode(signalCategory)
                        {
                            Tag = "CATEGORY:" + signalCategory
                        };
                        signalSourceTreeNode.Nodes.Add(signalCategoryTreeNode);
                        var signalsThisCategory = signalsThisSource.GetSignalsByCategory(signalCategory);
                        if (signalsThisCategory == null) continue;
                        var signalCollections = signalsThisCategory.GetDistinctSignalCollectionNames()
                            .OrderBy(x => x)
                            .ToList();
                        foreach (var signalCollectionName in signalCollections)
                        {
                            var signalCollectionTreeNode = new TreeNode(signalCollectionName)
                            {
                                Tag = "COLLECTION:" + signalCollectionName
                            };
                            signalCategoryTreeNode.Nodes.Add(signalCollectionTreeNode);
                        }
                    }
                }
            }
            //tvSignalCategories.Sort();
            tvSignals.EndUpdate();
        }

        private void ClearListView()
        {
            lvSignals.Clear();
            lvSignals.Groups.Clear();
        }

        private static ListViewItem CreateListViewItemFromSignal(Signal signal)
        {
            var lvi = new ListViewItem
            {
                Text = signal.FriendlyName,
                Tag = signal
            };
            return lvi;
        }

        private IHardwareSupportModule HardwareSupportModuleFor(TreeNode treeNode)
        {
            return treeNode != null && ScriptingContext != null
                ? ScriptingContext.HardwareSupportModules
                    .FirstOrDefault(x => x.FriendlyName == treeNode.Text)
                : null;
        }

        private void lvSignals_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                _columnSorter.Order = _columnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _columnSorter.SortColumn = e.Column;
                _columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lvSignals.Sort();
        }

        private static Signal SignalFor(ListViewItem listViewItem)
        {
            return listViewItem?.Tag as Signal;
        }

        private void SignalsView_Paint(object sender, PaintEventArgs e)
        {
            UpdateVisualization();
        }


        private void tvSignals_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvSignals.SelectedNode.Tag != null && tvSignals.SelectedNode.Parent?.Tag != null)
            {
                UpdateListViewAfterTreeViewItemSelect();
            }
            else
            {
                ClearListView();
            }
        }

        private void UpdateListViewAfterTreeViewItemSelect()
        {
            lvSignals.Clear();
            lvSignals.Groups.Clear();

            var collectionIdentifier = tvSignals.SelectedNode.Tag as string;
            if (collectionIdentifier != null &&
                !collectionIdentifier.StartsWith("COLLECTION", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (collectionIdentifier != null)
            {
                collectionIdentifier = collectionIdentifier.Substring("COLLECTION:".Length);
                var categoryIdentifier = (tvSignals.SelectedNode.Parent.Tag as string)?.Substring("CATEGORY:".Length);
                var sourceIdentifier =
                    (tvSignals.SelectedNode.Parent.Parent.Tag as string)?.Substring("SOURCE:".Length);

                lvSignals.Columns.Clear();
                lvSignals.Columns.Add(nameof(Signal));

                lvSignals.BeginUpdate();
                var signalsThisSource = Signals.GetSignalsFromSource(sourceIdentifier);
                var signalsThisCategory = signalsThisSource.GetSignalsByCategory(categoryIdentifier);
                var signalsThisCollection = signalsThisCategory.GetSignalsByCollection(collectionIdentifier);
                if (signalsThisCollection != null)
                {
                    var subCollections = signalsThisCollection.GetUniqueSubcollections(collectionIdentifier)
                        .OrderBy(x => x)
                        .ToList();


                    if (subCollections.Count > 0)
                    {
                        var signalsAlreadyAdded = new SignalList<Signal>();
                        foreach (var subcollectionName in subCollections)
                        {
                            var lvg = new ListViewGroup(subcollectionName, HorizontalAlignment.Left);
                            lvSignals.Groups.Add(lvg);
                            var signalsThisSubcollection =
                                signalsThisCollection
                                    .GetSignalsBySubcollectionName(collectionIdentifier, subcollectionName)
                                    .OrderBy(x => x.FriendlyName)
                                    .ToList();
                            foreach (var signal in signalsThisSubcollection)
                            {
                                var lvi = CreateListViewItemFromSignal(signal);
                                lvg.Items.Add(lvi);
                                lvSignals.Items.Add(lvi);
                                signalsAlreadyAdded.Add(signal);
                            }
                        }
                        foreach (var signal in signalsThisCollection.Except(signalsAlreadyAdded))
                        {
                            var lvi = CreateListViewItemFromSignal(signal);
                            lvSignals.Items.Add(lvi);
                        }
                    }
                    else
                    {
                        foreach (var signal in signalsThisCollection)
                        {
                            var lvi = CreateListViewItemFromSignal(signal);
                            lvSignals.Items.Add(lvi);
                        }
                    }
                }
            }
            for (var i = 0; i < lvSignals.Columns.Count; i++)
            {
                lvSignals.Columns[i].TextAlign = HorizontalAlignment.Left;
                lvSignals.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            lvSignals.EndUpdate();
            lvSignals.Sort();
        }

        private void UpdateModuleVisualization(Graphics graphics, Rectangle targetRectangle)
        {
            var hsm = SelectedHardwareSupportModule;
            hsm?.Render(graphics, targetRectangle);
        }

        private void UpdateSignalGraph(Graphics graphics, Rectangle targetRectangle)
        {
            SelectedSignal.DrawGraph(graphics, targetRectangle);
        }

        private void UpdateVisualization()
        {
            if (pbVisualization.Image == null)
            {
                pbVisualization.Image = new Bitmap(pbVisualization.ClientSize.Width, pbVisualization.ClientSize.Height);
            }

            using (var graphics = Graphics.FromImage(pbVisualization.Image))
            {
                var targetRectangle = new Rectangle(0, 0, pbVisualization.Image.Width, pbVisualization.Image.Height);
                if (SelectedSignal == null)
                {
                    UpdateModuleVisualization(graphics, targetRectangle);
                }
                else
                {
                    UpdateSignalGraph(graphics, targetRectangle);
                }
            }
        }
    }
}