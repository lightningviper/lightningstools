namespace JoyMapper
{
    internal sealed partial class frmMain
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
            if (disposing)
            {
                nfyTrayIcon.Dispose(); //we dispose our tray icon here
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
            var treeNode1 = new System.Windows.Forms.TreeNode("Local Devices", 0, 0);
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.treeMain = new System.Windows.Forms.TreeView();
            this.imglstImages = new System.Windows.Forms.ImageList(this.components);
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabMappings = new System.Windows.Forms.TabPage();
            this.tblMappingsLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblVirtualDevice = new System.Windows.Forms.Label();
            this.lblVirtualControl = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cboVirtualDevice = new System.Windows.Forms.ComboBox();
            this.cboVirtualControl = new System.Windows.Forms.ComboBox();
            this.tlbrMainToolbar = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefreshDevices = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAutoHighlighting = new System.Windows.Forms.ToolStripButton();
            this.mnuTopMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewRefreshDeviceList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionsCreateDefaultMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuActionsStartMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionsStopMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOpenWindowsJoystickControlPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPPJoy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPPJoyCreateNewVirtualDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPPJoySeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuPPJoyAssignMaximumCapabilities = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPPJoySeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuPPJoyOpenPPJoyControlPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.nfyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuCtxTrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuTrayCtxStartMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTrayCtxStopMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayCtxRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrayCtxExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxTreeItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCtxTreeItemsEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxTreeItemsDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCtxTreeItemsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCtxTreeItemsRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrAxisAndPovMovedStateClearingTimer = new System.Windows.Forms.Timer(this.components);
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlOuter.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabMappings.SuspendLayout();
            this.tblMappingsLayoutTable.SuspendLayout();
            this.tlbrMainToolbar.SuspendLayout();
            this.mnuTopMenu.SuspendLayout();
            this.mnuCtxTrayIcon.SuspendLayout();
            this.mnuCtxTreeItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlOuter
            // 
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.Controls.Add(this.splitMain);
            this.pnlOuter.Controls.Add(this.tlbrMainToolbar);
            this.pnlOuter.Controls.Add(this.mnuTopMenu);
            this.pnlOuter.Location = new System.Drawing.Point(0, 0);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(771, 447);
            this.pnlOuter.TabIndex = 4;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 49);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.treeMain);
            this.splitMain.Size = new System.Drawing.Size(771, 398);
            this.splitMain.Panel1MinSize = 200;
            this.splitMain.SplitterDistance = 200;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tabMain);
            this.splitMain.Panel2MinSize = 200;
            this.splitMain.TabIndex = 0;
            // 
            // treeMain
            // 
            this.treeMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMain.HideSelection = false;
            this.treeMain.ImageIndex = 0;
            this.treeMain.ImageList = this.imglstImages;
            this.treeMain.Location = new System.Drawing.Point(0, 0);
            this.treeMain.Name = "treeMain";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "nodeLocalDevices";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Local Devices";
            this.treeMain.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeMain.SelectedImageIndex = 0;
            this.treeMain.ShowNodeToolTips = true;
            this.treeMain.ShowRootLines = false;
            this.treeMain.Size = new System.Drawing.Size(200, 398);
            this.treeMain.TabIndex = 0;
            this.treeMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMain_AfterSelect);
            this.treeMain.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeMain_BeforeSelect);
            // 
            // imglstImages
            // 
            this.imglstImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstImages.ImageStream")));
            this.imglstImages.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstImages.Images.SetKeyName(0, "");
            this.imglstImages.Images.SetKeyName(1, "");
            this.imglstImages.Images.SetKeyName(2, "");
            this.imglstImages.Images.SetKeyName(3, "");
            this.imglstImages.Images.SetKeyName(4, "");
            this.imglstImages.Images.SetKeyName(5, "");
            this.imglstImages.Images.SetKeyName(6, "");
            this.imglstImages.Images.SetKeyName(7, "");
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabMappings);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(567, 398);
            this.tabMain.TabIndex = 0;
            // 
            // tabMappings
            // 
            this.tabMappings.Controls.Add(this.tblMappingsLayoutTable);
            this.tabMappings.Location = new System.Drawing.Point(4, 22);
            this.tabMappings.Name = "tabMappings";
            this.tabMappings.Size = new System.Drawing.Size(559, 372);
            this.tabMappings.TabIndex = 0;
            this.tabMappings.Text = "Mappings";
            this.tabMappings.UseVisualStyleBackColor = true;
            // 
            // tblMappingsLayoutTable
            // 
            this.tblMappingsLayoutTable.ColumnCount = 2;
            this.tblMappingsLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMappingsLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMappingsLayoutTable.Controls.Add(this.lblDescription, 0, 0);
            this.tblMappingsLayoutTable.Controls.Add(this.lblVirtualDevice, 0, 1);
            this.tblMappingsLayoutTable.Controls.Add(this.lblVirtualControl, 0, 2);
            this.tblMappingsLayoutTable.Controls.Add(this.txtDescription, 1, 0);
            this.tblMappingsLayoutTable.Controls.Add(this.cboVirtualDevice, 1, 1);
            this.tblMappingsLayoutTable.Controls.Add(this.cboVirtualControl, 1, 2);
            this.tblMappingsLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMappingsLayoutTable.Location = new System.Drawing.Point(0, 0);
            this.tblMappingsLayoutTable.Name = "tblMappingsLayoutTable";
            this.tblMappingsLayoutTable.RowCount = 4;
            this.tblMappingsLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblMappingsLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblMappingsLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblMappingsLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tblMappingsLayoutTable.Size = new System.Drawing.Size(559, 372);
            this.tblMappingsLayoutTable.TabIndex = 3;
            this.tblMappingsLayoutTable.Resize += new System.EventHandler(this.tblMappingsLayoutTable_Resize);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblDescription.Location = new System.Drawing.Point(16, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 30);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Description:";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblVirtualDevice
            // 
            this.lblVirtualDevice.AutoSize = true;
            this.lblVirtualDevice.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblVirtualDevice.Location = new System.Drawing.Point(3, 30);
            this.lblVirtualDevice.Name = "lblVirtualDevice";
            this.lblVirtualDevice.Size = new System.Drawing.Size(76, 30);
            this.lblVirtualDevice.TabIndex = 1;
            this.lblVirtualDevice.Text = "Virtual Device:";
            this.lblVirtualDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblVirtualControl
            // 
            this.lblVirtualControl.AutoSize = true;
            this.lblVirtualControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblVirtualControl.Location = new System.Drawing.Point(4, 60);
            this.lblVirtualControl.Name = "lblVirtualControl";
            this.lblVirtualControl.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.lblVirtualControl.Size = new System.Drawing.Size(75, 30);
            this.lblVirtualControl.TabIndex = 4;
            this.lblVirtualControl.Text = "Virtual Control:";
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(85, 3);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(471, 20);
            this.txtDescription.TabIndex = 0;
            // 
            // cboVirtualDevice
            // 
            this.cboVirtualDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboVirtualDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVirtualDevice.DropDownWidth = 15;
            this.cboVirtualDevice.FormattingEnabled = true;
            this.cboVirtualDevice.Location = new System.Drawing.Point(85, 33);
            this.cboVirtualDevice.Name = "cboVirtualDevice";
            this.cboVirtualDevice.Size = new System.Drawing.Size(471, 21);
            this.cboVirtualDevice.TabIndex = 1;
            this.cboVirtualDevice.SelectionChangeCommitted += new System.EventHandler(this.cboVirtualDevice_SelectionChangeCommitted);
            // 
            // cboVirtualControl
            // 
            this.cboVirtualControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboVirtualControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVirtualControl.DropDownWidth = 15;
            this.cboVirtualControl.FormattingEnabled = true;
            this.cboVirtualControl.Location = new System.Drawing.Point(85, 63);
            this.cboVirtualControl.Name = "cboVirtualControl";
            this.cboVirtualControl.Size = new System.Drawing.Size(471, 21);
            this.cboVirtualControl.TabIndex = 2;
            this.cboVirtualControl.SelectionChangeCommitted += new System.EventHandler(this.cboVirtualControl_SelectionChangeCommitted);
            // 
            // tlbrMainToolbar
            // 
            this.tlbrMainToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlbrMainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnStart,
            this.btnStop,
            this.toolStripSeparator2,
            this.btnOptions,
            this.toolStripSeparator4,
            this.btnRefreshDevices,
            this.toolStripSeparator3,
            this.btnAutoHighlighting});
            this.tlbrMainToolbar.Location = new System.Drawing.Point(0, 24);
            this.tlbrMainToolbar.Name = "tlbrMainToolbar";
            this.tlbrMainToolbar.Padding = new System.Windows.Forms.Padding(0);
            this.tlbrMainToolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tlbrMainToolbar.Size = new System.Drawing.Size(771, 25);
            this.tlbrMainToolbar.TabIndex = 4;
            this.tlbrMainToolbar.TabStop = true;
            this.tlbrMainToolbar.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(23, 22);
            this.btnNew.Text = "New";
            this.btnNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNew.ToolTipText = "New Mapping File";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOpen.ToolTipText = "Open Mapping File";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.ToolTipText = "Save Mapping File";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnStart
            // 
            this.btnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStart.Enabled = false;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(23, 22);
            this.btnStart.Text = "Start";
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.ToolTipText = "Start Mapping I/O";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(23, 22);
            this.btnStop.Text = "Stop";
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.ToolTipText = "Stop Mapping I/O";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOptions
            // 
            this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(23, 22);
            this.btnOptions.Text = "Options";
            this.btnOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefreshDevices
            // 
            this.btnRefreshDevices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefreshDevices.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshDevices.Image")));
            this.btnRefreshDevices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefreshDevices.Name = "btnRefreshDevices";
            this.btnRefreshDevices.Size = new System.Drawing.Size(23, 22);
            this.btnRefreshDevices.Text = "Refresh Devices";
            this.btnRefreshDevices.Click += new System.EventHandler(this.btnRefreshDevices_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAutoHighlighting
            // 
            this.btnAutoHighlighting.BackColor = System.Drawing.SystemColors.Control;
            this.btnAutoHighlighting.CheckOnClick = true;
            this.btnAutoHighlighting.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoHighlighting.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoHighlighting.Image")));
            this.btnAutoHighlighting.ImageTransparentColor = System.Drawing.Color.White;
            this.btnAutoHighlighting.Name = "btnAutoHighlighting";
            this.btnAutoHighlighting.Size = new System.Drawing.Size(133, 22);
            this.btnAutoHighlighting.Text = "&Auto-highlighting (on)";
            this.btnAutoHighlighting.ToolTipText = "Enables/disables automatic highlighting of controls when their state changes";
            this.btnAutoHighlighting.Click += new System.EventHandler(this.btnAutoHighlighting_Click);
            // 
            // mnuTopMenu
            // 
            this.mnuTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuActions,
            this.mnuTools,
            this.mnuPPJoy,
            this.mnuHelp});
            this.mnuTopMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuTopMenu.Name = "mnuTopMenu";
            this.mnuTopMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuTopMenu.Size = new System.Drawing.Size(771, 24);
            this.mnuTopMenu.TabIndex = 5;
            this.mnuTopMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileSeparator1,
            this.mnuFileImport,
            this.toolStripMenuItem1,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.mnuFileSeparator2,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(35, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileNew.Image")));
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuFileNew.Size = new System.Drawing.Size(152, 22);
            this.mnuFileNew.Text = "&New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileOpen.Image")));
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuFileOpen.Size = new System.Drawing.Size(152, 22);
            this.mnuFileOpen.Text = "&Open...";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileSeparator1
            // 
            this.mnuFileSeparator1.Name = "mnuFileSeparator1";
            this.mnuFileSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuFileImport
            // 
            this.mnuFileImport.Name = "mnuFileImport";
            this.mnuFileImport.Size = new System.Drawing.Size(152, 22);
            this.mnuFileImport.Text = "&Import...";
            this.mnuFileImport.Click += new System.EventHandler(this.mnuFileImport_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileSave.Image")));
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSave.Size = new System.Drawing.Size(152, 22);
            this.mnuFileSave.Text = "&Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Size = new System.Drawing.Size(152, 22);
            this.mnuFileSaveAs.Text = "Save &As...";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
            // 
            // mnuFileSeparator2
            // 
            this.mnuFileSeparator2.Name = "mnuFileSeparator2";
            this.mnuFileSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuFileExit.Size = new System.Drawing.Size(152, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewRefreshDeviceList});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(41, 20);
            this.mnuView.Text = "&View";
            // 
            // mnuViewRefreshDeviceList
            // 
            this.mnuViewRefreshDeviceList.Name = "mnuViewRefreshDeviceList";
            this.mnuViewRefreshDeviceList.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuViewRefreshDeviceList.Size = new System.Drawing.Size(185, 22);
            this.mnuViewRefreshDeviceList.Text = "&Refresh Device List";
            this.mnuViewRefreshDeviceList.Click += new System.EventHandler(this.mnuViewRefreshDeviceList_Click);
            // 
            // mnuActions
            // 
            this.mnuActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuActionsCreateDefaultMapping,
            this.mnuActionsSeparator1,
            this.mnuActionsStartMapping,
            this.mnuActionsStopMapping});
            this.mnuActions.Name = "mnuActions";
            this.mnuActions.ShortcutKeys = ((System.Windows.Forms.Keys)((((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Space)));
            this.mnuActions.Size = new System.Drawing.Size(54, 20);
            this.mnuActions.Text = "&Actions";
            // 
            // mnuActionsCreateDefaultMapping
            // 
            this.mnuActionsCreateDefaultMapping.Name = "mnuActionsCreateDefaultMapping";
            this.mnuActionsCreateDefaultMapping.Size = new System.Drawing.Size(230, 22);
            this.mnuActionsCreateDefaultMapping.Text = "Create &Default Mappings";
            this.mnuActionsCreateDefaultMapping.Visible = false;
            this.mnuActionsCreateDefaultMapping.Click += new System.EventHandler(this.mnuActionsCreateDefaultMappings_Click);
            // 
            // mnuActionsSeparator1
            // 
            this.mnuActionsSeparator1.Name = "mnuActionsSeparator1";
            this.mnuActionsSeparator1.Size = new System.Drawing.Size(227, 6);
            this.mnuActionsSeparator1.Visible = false;
            // 
            // mnuActionsStartMapping
            // 
            this.mnuActionsStartMapping.Enabled = false;
            this.mnuActionsStartMapping.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionsStartMapping.Image")));
            this.mnuActionsStartMapping.Name = "mnuActionsStartMapping";
            this.mnuActionsStartMapping.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
            this.mnuActionsStartMapping.Size = new System.Drawing.Size(230, 22);
            this.mnuActionsStartMapping.Text = "&Start Mapping";
            this.mnuActionsStartMapping.Click += new System.EventHandler(this.mnuActionsStartMapping_Click);
            // 
            // mnuActionsStopMapping
            // 
            this.mnuActionsStopMapping.Enabled = false;
            this.mnuActionsStopMapping.Image = ((System.Drawing.Image)(resources.GetObject("mnuActionsStopMapping.Image")));
            this.mnuActionsStopMapping.Name = "mnuActionsStopMapping";
            this.mnuActionsStopMapping.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Space)));
            this.mnuActionsStopMapping.Size = new System.Drawing.Size(230, 22);
            this.mnuActionsStopMapping.Text = "S&top Mapping";
            this.mnuActionsStopMapping.Click += new System.EventHandler(this.mnuActionsStopMapping_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOpenWindowsJoystickControlPanel,
            this.mnuToolsSeparator2,
            this.mnuToolsOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(44, 20);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsOpenWindowsJoystickControlPanel
            // 
            this.mnuToolsOpenWindowsJoystickControlPanel.Name = "mnuToolsOpenWindowsJoystickControlPanel";
            this.mnuToolsOpenWindowsJoystickControlPanel.ShortcutKeys = ((System.Windows.Forms.Keys)((((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.W)));
            this.mnuToolsOpenWindowsJoystickControlPanel.Size = new System.Drawing.Size(359, 22);
            this.mnuToolsOpenWindowsJoystickControlPanel.Text = "Open &Windows Joystick Control Panel...";
            this.mnuToolsOpenWindowsJoystickControlPanel.Click += new System.EventHandler(frmMain.mnuToolsOpenWindowsJoystickControlPanel_Click);
            // 
            // mnuToolsSeparator2
            // 
            this.mnuToolsSeparator2.Name = "mnuToolsSeparator2";
            this.mnuToolsSeparator2.Size = new System.Drawing.Size(356, 6);
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsOptions.Image")));
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuToolsOptions.Size = new System.Drawing.Size(359, 22);
            this.mnuToolsOptions.Text = "&Options...";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuPPJoy
            // 
            this.mnuPPJoy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPPJoyCreateNewVirtualDevices,
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks,
            this.mnuPPJoySeparator1,
            this.mnuPPJoyAssignMaximumCapabilities,
            this.mnuPPJoySeparator2,
            this.mnuPPJoyOpenPPJoyControlPanel});
            this.mnuPPJoy.Name = "mnuPPJoy";
            this.mnuPPJoy.Size = new System.Drawing.Size(48, 20);
            this.mnuPPJoy.Text = "&PPJoy";
            this.mnuPPJoy.DropDownOpening += new System.EventHandler(this.mnuPPJoy_DropDownOpening);
            // 
            // mnuPPJoyCreateNewVirtualDevices
            // 
            this.mnuPPJoyCreateNewVirtualDevices.Name = "mnuPPJoyCreateNewVirtualDevices";
            this.mnuPPJoyCreateNewVirtualDevices.Size = new System.Drawing.Size(378, 22);
            this.mnuPPJoyCreateNewVirtualDevices.Text = "&Create new PPJoy virtual joystick devices...";
            this.mnuPPJoyCreateNewVirtualDevices.Visible = false;
            this.mnuPPJoyCreateNewVirtualDevices.Click += new System.EventHandler(this.mnuPPJoyCreateNewVirtualJoystickDevices_Click);
            // 
            // mnuPPJoyRemoveAllPPJoyVirtualSticks
            // 
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks.Name = "mnuPPJoyRemoveAllPPJoyVirtualSticks";
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks.Size = new System.Drawing.Size(378, 22);
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks.Text = "&Remove all PPJoy virtual joystick devices";
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks.Visible = false;
            this.mnuPPJoyRemoveAllPPJoyVirtualSticks.Click += new System.EventHandler(this.mnuPPJoyRemoveAllPPJoyVirtualSticks_Click);
            // 
            // mnuPPJoySeparator1
            // 
            this.mnuPPJoySeparator1.Name = "mnuPPJoySeparator1";
            this.mnuPPJoySeparator1.Size = new System.Drawing.Size(375, 6);
            this.mnuPPJoySeparator1.Visible = false;
            // 
            // mnuPPJoyAssignMaximumCapabilities
            // 
            this.mnuPPJoyAssignMaximumCapabilities.Name = "mnuPPJoyAssignMaximumCapabilities";
            this.mnuPPJoyAssignMaximumCapabilities.Size = new System.Drawing.Size(378, 22);
            this.mnuPPJoyAssignMaximumCapabilities.Text = "&Assign maximum capabilities to all PPJoy Virtual Joystick devices";
            this.mnuPPJoyAssignMaximumCapabilities.Visible = false;
            this.mnuPPJoyAssignMaximumCapabilities.Click += new System.EventHandler(this.mnuPPJoyAssignMaximumCapabilities_Click);
            // 
            // mnuPPJoySeparator2
            // 
            this.mnuPPJoySeparator2.Name = "mnuPPJoySeparator2";
            this.mnuPPJoySeparator2.Size = new System.Drawing.Size(375, 6);
            this.mnuPPJoySeparator2.Visible = false;
            // 
            // mnuPPJoyOpenPPJoyControlPanel
            // 
            this.mnuPPJoyOpenPPJoyControlPanel.Name = "mnuPPJoyOpenPPJoyControlPanel";
            this.mnuPPJoyOpenPPJoyControlPanel.ShortcutKeys = ((System.Windows.Forms.Keys)((((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.P)));
            this.mnuPPJoyOpenPPJoyControlPanel.Size = new System.Drawing.Size(378, 22);
            this.mnuPPJoyOpenPPJoyControlPanel.Text = "Open &PPJoy Control Panel...";
            this.mnuPPJoyOpenPPJoyControlPanel.Click += new System.EventHandler(frmMain.mnuPPJoyOpenPPJoyControlPanel_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(40, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(159, 22);
            this.mnuHelpAbout.Text = "&About JoyMapper";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // nfyTrayIcon
            // 
            this.nfyTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfyTrayIcon.Icon")));
            this.nfyTrayIcon.Click += new System.EventHandler(this.nfyTrayIcon_Click);
            // 
            // mnuCtxTrayIcon
            // 
            this.mnuCtxTrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTrayCtxStartMapping,
            this.mnuTrayCtxStopMapping,
            this.mnuCtxSeparator1,
            this.mnuTrayCtxRestore,
            this.mnuCtxSeparator2,
            this.mnuTrayCtxExit});
            this.mnuCtxTrayIcon.Name = "contextMenuStrip1";
            this.mnuCtxTrayIcon.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuCtxTrayIcon.Size = new System.Drawing.Size(183, 104);
            // 
            // mnuTrayCtxStartMapping
            // 
            this.mnuTrayCtxStartMapping.Enabled = false;
            this.mnuTrayCtxStartMapping.Image = ((System.Drawing.Image)(resources.GetObject("mnuTrayCtxStartMapping.Image")));
            this.mnuTrayCtxStartMapping.Name = "mnuTrayCtxStartMapping";
            this.mnuTrayCtxStartMapping.Size = new System.Drawing.Size(182, 22);
            this.mnuTrayCtxStartMapping.Text = "&Start Mapping";
            this.mnuTrayCtxStartMapping.Click += new System.EventHandler(this.mnuCtxStartMapping_Click);
            // 
            // mnuTrayCtxStopMapping
            // 
            this.mnuTrayCtxStopMapping.Enabled = false;
            this.mnuTrayCtxStopMapping.Image = ((System.Drawing.Image)(resources.GetObject("mnuTrayCtxStopMapping.Image")));
            this.mnuTrayCtxStopMapping.Name = "mnuTrayCtxStopMapping";
            this.mnuTrayCtxStopMapping.Size = new System.Drawing.Size(182, 22);
            this.mnuTrayCtxStopMapping.Text = "S&top Mapping";
            this.mnuTrayCtxStopMapping.Click += new System.EventHandler(this.mnuCtxStopMapping_Click);
            // 
            // mnuCtxSeparator1
            // 
            this.mnuCtxSeparator1.Name = "mnuCtxSeparator1";
            this.mnuCtxSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // mnuTrayCtxRestore
            // 
            this.mnuTrayCtxRestore.Name = "mnuTrayCtxRestore";
            this.mnuTrayCtxRestore.Size = new System.Drawing.Size(182, 22);
            this.mnuTrayCtxRestore.Text = "&Restore editor window";
            this.mnuTrayCtxRestore.Click += new System.EventHandler(this.mnuCtxRestore_Click);
            // 
            // mnuCtxSeparator2
            // 
            this.mnuCtxSeparator2.Name = "mnuCtxSeparator2";
            this.mnuCtxSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // mnuTrayCtxExit
            // 
            this.mnuTrayCtxExit.Name = "mnuTrayCtxExit";
            this.mnuTrayCtxExit.Size = new System.Drawing.Size(182, 22);
            this.mnuTrayCtxExit.Text = "&Exit";
            this.mnuTrayCtxExit.Click += new System.EventHandler(this.mnuCtxExit_Click);
            // 
            // mnuCtxTreeItems
            // 
            this.mnuCtxTreeItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCtxTreeItemsEnable,
            this.mnuCtxTreeItemsDisable,
            this.mnuCtxTreeItemsSeparator1,
            this.mnuCtxTreeItemsRemove});
            this.mnuCtxTreeItems.Name = "mnuCtxTreeItems";
            this.mnuCtxTreeItems.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuCtxTreeItems.Size = new System.Drawing.Size(148, 76);
            // 
            // mnuCtxTreeItemsEnable
            // 
            this.mnuCtxTreeItemsEnable.Name = "mnuCtxTreeItemsEnable";
            this.mnuCtxTreeItemsEnable.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.mnuCtxTreeItemsEnable.Size = new System.Drawing.Size(147, 22);
            this.mnuCtxTreeItemsEnable.Text = "Enable";
            this.mnuCtxTreeItemsEnable.Click += new System.EventHandler(this.mnuCtxTreeItemsEnable_Click);
            // 
            // mnuCtxTreeItemsDisable
            // 
            this.mnuCtxTreeItemsDisable.Name = "mnuCtxTreeItemsDisable";
            this.mnuCtxTreeItemsDisable.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.mnuCtxTreeItemsDisable.Size = new System.Drawing.Size(147, 22);
            this.mnuCtxTreeItemsDisable.Text = "&Disable";
            this.mnuCtxTreeItemsDisable.Click += new System.EventHandler(this.mnuCtxTreeItemsDisable_Click);
            // 
            // mnuCtxTreeItemsSeparator1
            // 
            this.mnuCtxTreeItemsSeparator1.Name = "mnuCtxTreeItemsSeparator1";
            this.mnuCtxTreeItemsSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // mnuCtxTreeItemsRemove
            // 
            this.mnuCtxTreeItemsRemove.Name = "mnuCtxTreeItemsRemove";
            this.mnuCtxTreeItemsRemove.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mnuCtxTreeItemsRemove.Size = new System.Drawing.Size(147, 22);
            this.mnuCtxTreeItemsRemove.Text = "&Remove";
            this.mnuCtxTreeItemsRemove.Click += new System.EventHandler(this.mnuCtxTreeItemsRemove_Click);
            // 
            // tmrAxisAndPovMovedStateClearingTimer
            // 
            this.tmrAxisAndPovMovedStateClearingTimer.Enabled = true;
            this.tmrAxisAndPovMovedStateClearingTimer.Interval = 1000;
            this.tmrAxisAndPovMovedStateClearingTimer.Tick += new System.EventHandler(this.tmrAxisAndPovMovedStateClearingTimer_Tick);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 447);
            this.Controls.Add(this.pnlOuter);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 34);
            this.Name = "frmMain";
            this.Text = "JoyMapper - Untitled";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMappings.ResumeLayout(false);
            this.tblMappingsLayoutTable.ResumeLayout(false);
            this.tblMappingsLayoutTable.PerformLayout();
            this.tlbrMainToolbar.ResumeLayout(false);
            this.tlbrMainToolbar.PerformLayout();
            this.mnuTopMenu.ResumeLayout(false);
            this.mnuTopMenu.PerformLayout();
            this.mnuCtxTrayIcon.ResumeLayout(false);
            this.mnuCtxTreeItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabPage tabMappings;
        private System.Windows.Forms.ComboBox cboVirtualDevice;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblVirtualControl;
        private System.Windows.Forms.ComboBox cboVirtualControl;
        private System.Windows.Forms.TreeView treeMain;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TableLayoutPanel tblMappingsLayoutTable;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblVirtualDevice;
        private System.Windows.Forms.MenuStrip mnuTopMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuActions;
        private System.Windows.Forms.ToolStripMenuItem mnuActionsStartMapping;
        private System.Windows.Forms.ToolStripMenuItem mnuActionsStopMapping;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStrip tlbrMainToolbar;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.NotifyIcon nfyTrayIcon;
        private System.Windows.Forms.ToolStripButton btnOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ContextMenuStrip mnuCtxTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayCtxStartMapping;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayCtxStopMapping;
        private System.Windows.Forms.ToolStripSeparator mnuCtxSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayCtxRestore;
        private System.Windows.Forms.ToolStripSeparator mnuCtxSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayCtxExit;
        private System.Windows.Forms.ToolStripMenuItem mnuActionsCreateDefaultMapping;
        private System.Windows.Forms.ToolStripSeparator mnuActionsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuPPJoyOpenPPJoyControlPanel;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOpenWindowsJoystickControlPanel;
        private System.Windows.Forms.ToolStripMenuItem mnuViewRefreshDeviceList;
        private System.Windows.Forms.ToolStripSeparator mnuToolsSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuPPJoy;
        private System.Windows.Forms.ToolStripMenuItem mnuPPJoyCreateNewVirtualDevices;
        private System.Windows.Forms.ToolStripMenuItem mnuPPJoyAssignMaximumCapabilities;
        private System.Windows.Forms.ToolStripSeparator mnuPPJoySeparator1;
        private System.Windows.Forms.ContextMenuStrip mnuCtxTreeItems;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxTreeItemsDisable;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxTreeItemsRemove;
        private System.Windows.Forms.ToolStripSeparator mnuCtxTreeItemsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuCtxTreeItemsEnable;
        private System.Windows.Forms.ToolStripMenuItem mnuPPJoyRemoveAllPPJoyVirtualSticks;
        private System.Windows.Forms.ImageList imglstImages;
        private System.Windows.Forms.Timer tmrAxisAndPovMovedStateClearingTimer;
        private System.Windows.Forms.ToolStripButton btnAutoHighlighting;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnRefreshDevices;
        private System.Windows.Forms.ToolStripMenuItem mnuFileImport;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator mnuPPJoySeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
    }
}