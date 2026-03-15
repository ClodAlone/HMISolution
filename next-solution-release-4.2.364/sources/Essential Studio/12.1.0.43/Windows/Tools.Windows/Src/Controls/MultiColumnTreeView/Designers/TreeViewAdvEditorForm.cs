#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class TreeViewAdvEditorForm : Form
    {
        #region Class members

        private IServiceProvider m_provider = null;

        private TreeViewAdvDragHighlightTracker treeViewDragHighlightTracker = null;

        /// <summary>Helps keep track of the node that is being dragged.</summary>
        private TreeNodeAdv currentSourceNode;
        #endregion

        #region Form controls

        private ButtonAdv addNode;

        private ButtonAdv addChild;

        private TreeViewAdvEditorPropertyGrid propertyEditor;

        private ButtonAdv ok;

        private ButtonAdv cancel;

        private MultiColumnTreeView originalTree;

        private ButtonAdv remove;

        private CheckBox select;

        private ButtonAdv btnSort;

        private PropertyGridContextMenu pgMenu;

        private TreeViewAdvEditor tree;

        private Panel pnlBody;

        private Splitter splitterBody;
  
        private GradientPanel gradientPanelForm;

        private Panel pnlTree;

        private Panel pnlProperties;

        private Label lblProperties;

        private Label lblNodes;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        #endregion

        #region Class Initialize/Finalize methods

        public TreeViewAdvEditorForm()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            tree.DesignModeInternal = true;
            this.propertyEditor.HelpVisible = true;
        }

        public TreeViewAdvEditorForm(MultiColumnTreeView treeViewAdv, IServiceProvider provider)
            : this()
        {
            // update provider references
            m_provider = provider;
            propertyEditor.Provider = m_provider;

            this.pgMenu = new PropertyGridContextMenu(this.propertyEditor);

            originalTree = treeViewAdv;
            tree.Columns.AddRange(treeViewAdv.Columns.Clone());
            tree.Root = treeViewAdv.Root.Clone();
            tree.InteractiveCheckBoxes = treeViewAdv.InteractiveCheckBoxes;
            tree.ItemHeight = treeViewAdv.ItemHeight;
            tree.ShowCheckBoxes = treeViewAdv.ShowCheckBoxes;
            tree.ShowOptionButtons = treeViewAdv.ShowOptionButtons;
            tree.ShowColumnsHeader = treeViewAdv.ShowColumnsHeader;
            tree.ShowPlusMinus = treeViewAdv.ShowPlusMinus;
            tree.HideSelection = false;
            tree.ThemesEnabled = treeViewAdv.ThemesEnabled;
            tree.NodeCount = treeViewAdv.NodeCount;
            tree.AllowDrop = true;
            tree.Font = treeViewAdv.Font;
            tree.ForeColor = treeViewAdv.ForeColor;
            tree.DesignModeInternal = true;
            tree.AutoAdjustMultiLineHeight = treeViewAdv.AutoAdjustMultiLineHeight;
            tree.ShowRootLines = treeViewAdv.ShowRootLines;

            foreach (string name in treeViewAdv.BaseStyles.Keys)
            {
                tree.BaseStyles[name] = treeViewAdv.BaseStyles[name];
            }

            // copy image list settings
            tree.NodeStateImageList = treeViewAdv.NodeStateImageList;
            tree.StateImageList = treeViewAdv.StateImageList;
            tree.LeftImageList = treeViewAdv.LeftImageList;
            tree.RightImageList = treeViewAdv.RightImageList;

            tree.DefaultCollapseImageIndex = treeViewAdv.DefaultCollapseImageIndex;
            tree.DefaultExpandImageIndex = treeViewAdv.DefaultExpandImageIndex;

            this.treeViewDragHighlightTracker = new TreeViewAdvDragHighlightTracker(tree);
            this.treeViewDragHighlightTracker.EdgeSensitivityOnTop = this.tree.ItemHeight / 4;
            this.treeViewDragHighlightTracker.EdgeSensitivityAtBottom = this.tree.ItemHeight / 4;
            this.treeViewDragHighlightTracker.QueryAllowedPositionsForNode +=
              new QueryAllowedPositionsEventHandler(this.TreeDragDrop_QueryAllowedPositionsForNode);

            tree.DragOver += new DragEventHandler(this.TreeViewAdv_DragOver);
            tree.QueryContinueDrag += new QueryContinueDragEventHandler(this.TreeViewAdv_QueryContinueDrag);
            tree.DragLeave += new EventHandler(this.TreeViewAdv_DragLeave);
            tree.DragDrop += new DragEventHandler(this.TreeViewAdv_DragDrop);
            tree.ItemDrag += new ItemDragEventHandler(this.TreeViewAdv_ItemDrag);
            tree.Root.RecalculateAllDimensions();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.pgMenu.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdvStyleInfo treeColumnAdvStyleInfo1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdvStyleInfo();
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItemStyleInfo treeNodeAdvSubItemStyleInfo1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItemStyleInfo();
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvStyleInfo treeNodeAdvStyleInfo1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvStyleInfo();
            this.addNode = new Syncfusion.Windows.Forms.ButtonAdv();
            this.addChild = new Syncfusion.Windows.Forms.ButtonAdv();
            this.propertyEditor = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeViewAdvEditorPropertyGrid();
            this.ok = new Syncfusion.Windows.Forms.ButtonAdv();
            this.cancel = new Syncfusion.Windows.Forms.ButtonAdv();
            this.remove = new Syncfusion.Windows.Forms.ButtonAdv();
            this.gradientPanelForm = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.lblProperties = new System.Windows.Forms.Label();
            this.splitterBody = new System.Windows.Forms.Splitter();
            this.pnlTree = new System.Windows.Forms.Panel();
            this.tree = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeViewAdvEditor();
            this.lblNodes = new System.Windows.Forms.Label();
            this.btnSort = new Syncfusion.Windows.Forms.ButtonAdv();
            this.select = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gradientPanelForm)).BeginInit();
            this.gradientPanelForm.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.pnlTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
            this.SuspendLayout();
            // 
            // addNode
            // 
            this.addNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addNode.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.addNode.ComboEditBackColor = System.Drawing.Color.Empty;
            this.addNode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addNode.IsMouseDown = false;
            this.addNode.Location = new System.Drawing.Point(16, 384);
            this.addNode.Name = "addNode";
            this.addNode.Size = new System.Drawing.Size(76, 24);
            this.addNode.TabIndex = 1;
            this.addNode.Text = "&Add Node";
            this.addNode.UseVisualStyle = true;
            this.addNode.Click += new System.EventHandler(this.AddNode_Click);
            // 
            // addChild
            // 
            this.addChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addChild.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.addChild.ComboEditBackColor = System.Drawing.Color.Empty;
            this.addChild.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addChild.IsMouseDown = false;
            this.addChild.Location = new System.Drawing.Point(96, 384);
            this.addChild.Name = "addChild";
            this.addChild.Size = new System.Drawing.Size(76, 24);
            this.addChild.TabIndex = 2;
            this.addChild.Text = "A&dd Child";
            this.addChild.UseVisualStyle = true;
            this.addChild.Click += new System.EventHandler(this.AddChild_Click);
            // 
            // propertyEditor
            // 
            this.propertyEditor.BackColor = System.Drawing.SystemColors.Control;
            this.propertyEditor.CommandsVisibleIfAvailable = true;
            this.propertyEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyEditor.HelpVisible = false;
            this.propertyEditor.LargeButtons = false;
            this.propertyEditor.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyEditor.Location = new System.Drawing.Point(0, 23);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.Size = new System.Drawing.Size(310, 345);
            this.propertyEditor.TabIndex = 4;
            this.propertyEditor.Text = "propertyGrid1";
            this.propertyEditor.ToolbarVisible = false;
            this.propertyEditor.ViewBackColor = System.Drawing.SystemColors.Window;
            this.propertyEditor.ViewForeColor = System.Drawing.SystemColors.WindowText;
            // 
            // OK
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.ok.ComboEditBackColor = System.Drawing.Color.Empty;
            this.ok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ok.IsMouseDown = false;
            this.ok.Location = new System.Drawing.Point(466, 416);
            this.ok.Name = "OK";
            this.ok.Size = new System.Drawing.Size(76, 24);
            this.ok.TabIndex = 6;
            this.ok.Text = "&OK";
            this.ok.UseVisualStyle = true;
            this.ok.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.cancel.ComboEditBackColor = System.Drawing.Color.Empty;
            this.cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancel.IsMouseDown = false;
            this.cancel.Location = new System.Drawing.Point(546, 416);
            this.cancel.Name = "Cancel";
            this.cancel.Size = new System.Drawing.Size(76, 24);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyle = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Remove
            // 
            this.remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.remove.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.remove.ComboEditBackColor = System.Drawing.Color.Empty;
            this.remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.remove.IsMouseDown = false;
            this.remove.Location = new System.Drawing.Point(176, 384);
            this.remove.Name = "Remove";
            this.remove.Size = new System.Drawing.Size(76, 24);
            this.remove.TabIndex = 3;
            this.remove.Text = "&Remove";
            this.remove.UseVisualStyle = true;
            this.remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // gradientPanelForm
            // 
            this.gradientPanelForm.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, new System.Drawing.Color[]
        {
          System.Drawing.Color.FromArgb( ( ( byte )( 238 ) ), ( ( byte )( 243 ) ), ( ( byte )( 250 ) ) ),
          System.Drawing.Color.White,
          System.Drawing.Color.FromArgb( ( ( byte )( 238 ) ), ( ( byte )( 243 ) ), ( ( byte )( 250 ) ) )
        });
            this.gradientPanelForm.BorderColor = System.Drawing.Color.Black;
            this.gradientPanelForm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gradientPanelForm.Controls.Add(this.pnlBody);
            this.gradientPanelForm.Controls.Add(this.btnSort);
            this.gradientPanelForm.Controls.Add(this.addChild);
            this.gradientPanelForm.Controls.Add(this.ok);
            this.gradientPanelForm.Controls.Add(this.addNode);
            this.gradientPanelForm.Controls.Add(this.cancel);
            this.gradientPanelForm.Controls.Add(this.remove);
            this.gradientPanelForm.Controls.Add(this.select);
            this.gradientPanelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanelForm.DockPadding.All = 8;
            this.gradientPanelForm.IgnoreThemeBackground = true;
            this.gradientPanelForm.Location = new System.Drawing.Point(0, 0);
            this.gradientPanelForm.Name = "gradientPanelForm";
            this.gradientPanelForm.Size = new System.Drawing.Size(632, 446);
            this.gradientPanelForm.TabIndex = 0;
            // 
            // pnlBody
            // 
            this.pnlBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
              | System.Windows.Forms.AnchorStyles.Left)
              | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBody.BackColor = System.Drawing.Color.Transparent;
            this.pnlBody.Controls.Add(this.pnlProperties);
            this.pnlBody.Controls.Add(this.splitterBody);
            this.pnlBody.Controls.Add(this.pnlTree);
            this.pnlBody.Location = new System.Drawing.Point(8, 8);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(616, 368);
            this.pnlBody.TabIndex = 0;
            // 
            // pnlProperties
            // 
            this.pnlProperties.Controls.Add(this.propertyEditor);
            this.pnlProperties.Controls.Add(this.lblProperties);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProperties.Location = new System.Drawing.Point(306, 0);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(310, 368);
            this.pnlProperties.TabIndex = 6;
            // 
            // lblProperties
            // 
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProperties.Location = new System.Drawing.Point(0, 0);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(310, 23);
            this.lblProperties.TabIndex = 5;
            this.lblProperties.Text = "&Properties:";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitterBody
            // 
            this.splitterBody.BackColor = System.Drawing.Color.FromArgb(239, 244, 250);
            this.splitterBody.Location = new System.Drawing.Point(300, 0);
            this.splitterBody.MinSize = 200;
            this.splitterBody.Name = "splitterBody";
            this.splitterBody.Size = new System.Drawing.Size(6, 368);
            this.splitterBody.TabIndex = 1;
            this.splitterBody.TabStop = false;
            // 
            // pnlTree
            // 
            this.pnlTree.BackColor = System.Drawing.Color.Transparent;
            this.pnlTree.Controls.Add(this.tree);
            this.pnlTree.Controls.Add(this.lblNodes);
            this.pnlTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTree.Location = new System.Drawing.Point(0, 0);
            this.pnlTree.Name = "pnlTree";
            this.pnlTree.Size = new System.Drawing.Size(300, 368);
            this.pnlTree.TabIndex = 5;
            // 
            // tree
            // 
            this.tree.AllowDrop = true;
            this.tree.BackgroundColor = new Syncfusion.Drawing.BrushInfo();
            treeColumnAdvStyleInfo1.AreaBackground = new Syncfusion.Drawing.BrushInfo(System.Drawing.Color.Transparent);
            treeNodeAdvStyleInfo1.EnsureDefaultOptionedChild = true;
            this.tree.BaseStylePairs.AddRange(new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair[]
        {
          new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair( "Standard - Column", treeColumnAdvStyleInfo1 ),
          new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair( "Standard - SubItem", treeNodeAdvSubItemStyleInfo1 ),
          new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair( "Standard", treeNodeAdvStyleInfo1 )
        });
            this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // tree.HelpTextControl
            // 
            this.tree.HelpTextControl.BackgroundColor = new Syncfusion.Drawing.BrushInfo();
            this.tree.HelpTextControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tree.HelpTextControl.Location = new System.Drawing.Point(732, 0);
            this.tree.HelpTextControl.Name = "helpText";
            this.tree.HelpTextControl.Size = new System.Drawing.Size(50, 17);
            this.tree.HelpTextControl.TabIndex = 0;
            this.tree.HelpTextControl.Text = "help text";
            this.tree.Location = new System.Drawing.Point(0, 23);
            this.tree.Name = "tree";
            this.tree.PathSeparator = "/";
            this.tree.Size = new System.Drawing.Size(300, 345);
            this.tree.TabIndex = 0;
            // 
            // tree.ToolTipControl
            // 
            this.tree.ToolTipControl.BackColor = System.Drawing.SystemColors.Info;
            this.tree.ToolTipControl.BackgroundColor = new Syncfusion.Drawing.BrushInfo();
            this.tree.ToolTipControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tree.ToolTipControl.Location = new System.Drawing.Point(642, 0);
            this.tree.ToolTipControl.Name = "toolTip";
            this.tree.ToolTipControl.Size = new System.Drawing.Size(6, 6);
            this.tree.ToolTipControl.TabIndex = 1;
            this.tree.ToolTipControl.Text = "toolTip";
            this.tree.AfterSelect += new System.EventHandler(this.Tree_AfterSelect);
            // 
            // lblNodes
            // 
            this.lblNodes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNodes.Location = new System.Drawing.Point(0, 0);
            this.lblNodes.Name = "lblNodes";
            this.lblNodes.Size = new System.Drawing.Size(300, 23);
            this.lblNodes.TabIndex = 1;
            this.lblNodes.Text = "&Nodes:";
            this.lblNodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSort
            // 
            this.btnSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSort.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnSort.ComboEditBackColor = System.Drawing.Color.Empty;
            this.btnSort.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSort.IsMouseDown = false;
            this.btnSort.Location = new System.Drawing.Point(546, 384);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(76, 24);
            this.btnSort.TabIndex = 5;
            this.btnSort.Text = "&Sort";
            this.btnSort.UseVisualStyle = true;
            this.btnSort.Click += new System.EventHandler(this.BtnSort_Click);
            // 
            // select
            // 
            this.select.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.select.BackColor = System.Drawing.Color.Transparent;
            this.select.Checked = true;
            this.select.CheckState = System.Windows.Forms.CheckState.Checked;
            this.select.Location = new System.Drawing.Point(404, 384);
            this.select.Name = "select";
            this.select.Size = new System.Drawing.Size(128, 24);
            this.select.TabIndex = 4;
            this.select.Text = "Select added &node";
            this.select.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TreeViewAdvEditorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.gradientPanelForm);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "TreeViewAdvEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TreeViewAdv NodeCollection Editor";
            this.Load += new System.EventHandler(this.TreeViewAdvEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gradientPanelForm)).EndInit();
            this.gradientPanelForm.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.pnlTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Class event handlers
    
        private void AddNode_Click(object sender, EventArgs e)
        {
            TreeNodeAdv node = new TreeNodeAdv("Node" + tree.NodeCount.ToString());

            if (tree.SelectedNode != null && tree.SelectedNode.Parent != null)
            {
                this.tree.SelectedNode.Parent.Nodes.Add(node);
            }
            else
            {
                tree.Nodes.Add(node);
            }

            tree.NodeCount++;

            if (this.select.Checked)
            {
                tree.SelectedNode = node;
            }

            tree.Root.RecalculateAllDimensions();
            tree.Invalidate();
        }

        private void AddChild_Click(object sender, EventArgs e)
        {
            TreeNodeAdv node = new TreeNodeAdv("Node" + tree.NodeCount.ToString());

            if (tree.SelectedNode != null)
            {
                this.tree.SelectedNode.Nodes.Add(node);
                tree.NodeCount++;
                this.tree.SelectedNode.Expand();
            }

            if (this.select.Checked)
            {
                tree.SelectedNode = node;
            }

            tree.Invalidate();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            // recalculating MaxX, it is used by horizontal scroller
            tree.HScrollBar.Value = 0;
            tree.HScrollPos = 0;
            tree.Update();

            originalTree.Root = tree.Root.Clone();
            originalTree.MakeDirty();
            originalTree.Root.Visible = true;
            originalTree.NodeCount = tree.NodeCount;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Tree_AfterSelect(object sender, EventArgs e)
        {
            this.propertyEditor.SelectedObject = tree.SelectedNode;
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode != null && tree.SelectedNode.Parent != null)
            {
                TreeNodeAdv parent = tree.SelectedNode.Parent;
                tree.SelectedNode.Parent.Nodes.Remove(tree.SelectedNode);

                // tree.SelectedNode = parent;
            }
        }
        private void BtnSort_Click(object sender, EventArgs e)
        {
            if (this.tree.SelectedNode != null)
            {
                this.tree.SelectedNode.Sort();
            }
        }

        private void TreeViewAdvEditorForm_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region SOURCE_DRAG_DROP

        private void TreeViewAdv_ItemDrag(object sender, ItemDragEventArgs e)
        {
            MultiColumnTreeView treeViewAdv = sender as MultiColumnTreeView;

            // The TreeViewAdv always provides an array of selected nodes.
            TreeNodeAdv[] nodes = e.Item as TreeNodeAdv[];

            // Let us get only the first selected node.
            TreeNodeAdv node = nodes[0];

            // Only allow move
            DragDropEffects result = treeViewAdv.DoDragDrop(node, DragDropEffects.Move);
        }
        private void TreeViewAdv_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            // Cancel draggin when Escape was pressed.
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
            }
        }
        #endregion // SOURCE_DRAG_DROP

        #region DEST_DRAGDROP_LOGIC
        private void TreeViewAdv_DragOver(object sender, DragEventArgs e)
        {
            // Determine drag effects
            bool droppable = true;
            TreeNodeAdv destinationNode = null;
            MultiColumnTreeView treeView = sender as MultiColumnTreeView;
            Point ptInTree = treeView.PointToClient(new Point(e.X, e.Y));
            this.currentSourceNode = null;

            // Looking for a single tree node.
            if (e.Data.GetDataPresent(typeof(TreeNodeAdv)))
            {
                // Get the destination and source node.
                destinationNode = treeView.GetNodeAtPoint(ptInTree);
                if (destinationNode == null)
                {
                    destinationNode = treeView.Root;
                }
                TreeNodeAdv sourceNode = (TreeNodeAdv)e.Data.GetData(typeof(TreeNodeAdv));

                // Cache this for use later in the TreeDragDrop_QueryAllowedPositionsForNode event handler.
                this.currentSourceNode = sourceNode;
                droppable = this.CanDrop(sourceNode, destinationNode);
            }
            else
            {
                droppable = false;
            }

            // If Moving is allowed:
            e.Effect = droppable ? DragDropEffects.Move : DragDropEffects.None;

            if (droppable)
            {
                // Let the highlight tracker keep track of the current highlight node.
                this.treeViewDragHighlightTracker.SetHighlightNode(destinationNode, ptInTree);
            }
        }

        private bool CanDrop(TreeNodeAdv sourceNode, TreeNodeAdv destinationNode)
        {
            return !(sourceNode.TreeView != this.tree || // Support drag and drop only within the same tree
              destinationNode == null || // Cannot drop into empty area
              destinationNode.IsNodeRelative(sourceNode) || // Cannot drop over the source's parent
              destinationNode == sourceNode // Or over itself
              );
        }

        private void TreeViewAdv_DragLeave(object sender, EventArgs e)
        {
            // Let the highlight tracker keep track of the current highlight node.
            this.treeViewDragHighlightTracker.ClearHighlightNode();
        }

        private void TreeViewAdv_DragDrop(object sender, DragEventArgs e)
        {
            MultiColumnTreeView treeView = sender as MultiColumnTreeView;

            // Get the destination and source node.
            TreeNodeAdv sourceNode = (TreeNodeAdv)e.Data.GetData(typeof(TreeNodeAdv));
            TreeNodeAdv destinationNode = this.treeViewDragHighlightTracker.HighlightNode;
            TreeViewDropPositions dropPosition = this.treeViewDragHighlightTracker.DropPosition;

            // Clear the highlight info in the tracker.
            this.treeViewDragHighlightTracker.ClearHighlightNode();

            this.currentSourceNode = null;

            // Move the source node based on the tracked info.
            if (destinationNode != null)
            {
                switch (dropPosition)
                {
                    case TreeViewDropPositions.AboveNode:
                        sourceNode.Move(destinationNode, NodePositions.Previous);
                        break;
                    case TreeViewDropPositions.BelowNode:
                        sourceNode.Move(destinationNode, NodePositions.Next);
                        break;
                    case TreeViewDropPositions.OnNode:
                        sourceNode.Move(destinationNode.Nodes);
                        destinationNode.Expand();
                        break;
                }
            }

            treeView.SelectedNode = sourceNode;
        }

        /// <summary>Specifiy the allowed drop positions for the specified highlight node.</summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void TreeDragDrop_QueryAllowedPositionsForNode(object sender, QueryAllowedPositionsEventArgs e)
        {
            if (e.HighlightNode != this.currentSourceNode)
            {
                e.AllowedPositions = TreeViewDropPositions.AboveNode |
                  TreeViewDropPositions.BelowNode |
                  TreeViewDropPositions.OnNode;
            }
            else
            {
                e.AllowedPositions = TreeViewDropPositions.None;
            }

            // Only if the source node is droppable
            // and droppable ON the node (not beside it)
            e.ShowSelectionHighlight = this.CanDrop(this.currentSourceNode, e.HighlightNode) &&
              (e.NewDropPosition == TreeViewDropPositions.OnNode);
        }
        #endregion // DEST_DRAGDROP_LOGIC
    }
}