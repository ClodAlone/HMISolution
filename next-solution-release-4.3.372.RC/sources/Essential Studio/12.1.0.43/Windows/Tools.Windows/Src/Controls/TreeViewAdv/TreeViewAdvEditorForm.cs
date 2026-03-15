#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	class TreeViewAdvEditorPropertyGrid : PropertyGrid
	{
		private IServiceProvider m_provider = null;

		public TreeViewAdvEditorPropertyGrid( IServiceProvider provider ): base()
		{
			m_provider = provider;
		}


		protected override object GetService(Type service)
		{
			object svc = base.GetService(service);

			if (svc == null && service == typeof(System.ComponentModel.Design.IDesignerHost))
			{
				if (m_provider != null)
				{
					svc = m_provider.GetService(service);
				}
			}
				
			return svc;
		}

	}

	/// <summary>
	/// Summary description for TreeViewAdvEditorForm.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public class TreeViewAdvEditorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button addNode;
		private System.Windows.Forms.Button addChild;
		private TreeViewAdvEditorPropertyGrid propertyGrid1;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;
		private TreeViewAdvEditor tree;
		private TreeViewAdv originalTree;
		private System.Windows.Forms.Button Remove;
		private Syncfusion.Windows.Forms.Tools.GradientPanel gradientPanel1;
		private System.Windows.Forms.CheckBox select;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Button button1;
		private PropertyGridContextMenu pgMenu;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		private IServiceProvider m_provider = null;

		public TreeViewAdvEditorForm( TreeViewAdv Tree, IServiceProvider provider )
		{
			m_provider = provider;

			InitializeComponent();

			this.pgMenu = new PropertyGridContextMenu(this.propertyGrid1);
            this.ShowIcon = false;
            this.AcceptButton = OK;
            this.CancelButton = Cancel;
/*			TreeNodeAdv[] nodes = new TreeNodeAdv[Tree.Nodes.Count];
			Tree.Nodes.CopyTo(nodes,0);
			tree.Nodes.Clear();
			tree.Nodes.AddRange(nodes);
*/			originalTree = Tree;
			tree.Root = Tree.Root.Clone();
			tree.InteractiveCheckBoxes = Tree.InteractiveCheckBoxes;
			tree.ItemHeight = Tree.ItemHeight;
			tree.ShowCheckBoxes = Tree.ShowCheckBoxes;
			tree.ShowOptionButtons = Tree.ShowOptionButtons;
			tree.ShowPlusMinus = Tree.ShowPlusMinus;
			tree.HideSelection = false;
			tree.ThemesEnabled = Tree.ThemesEnabled;
			tree.NodeCount = Tree.NodeCount;
			tree.AllowDrop = true;
			tree.Font = Tree.Font;
			tree.ForeColor = Tree.ForeColor;
			foreach(string name in Tree.BaseStyles.Keys)
				tree.BaseStyles[name] = Tree.BaseStyles[name];

			tree.NodeStateImageList = Tree.NodeStateImageList;
			tree.DefaultCollapseImageIndex = Tree.DefaultCollapseImageIndex;
			tree.DefaultExpandImageIndex = Tree.DefaultExpandImageIndex;

			this.treeViewDragHighlightTracker = new TreeViewAdvDragHighlightTracker(tree);
			this.treeViewDragHighlightTracker.EdgeSensitivityOnTop = this.tree.ItemHeight / 4;
			this.treeViewDragHighlightTracker.EdgeSensitivityAtBottom = this.tree.ItemHeight / 4;
			this.treeViewDragHighlightTracker.QueryAllowedPositionsForNode +=
				new QueryAllowedPositionsEventHandler(this.TreeDragDrop_QueryAllowedPositionsForNode);

			tree.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewAdv_DragOver);
			tree.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.treeViewAdv_QueryContinueDrag);
			tree.DragLeave += new System.EventHandler(this.treeViewAdv_DragLeave);
			tree.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewAdv_DragDrop);
			tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewAdv_ItemDrag);
            tree.SelectedNodes.CollectionChanged += new CollectionChangeEventHandler( SelectedNodes_CollectionChanged );

            if( this.tree.SelectedNodes.Count == 0 )
            {
                this.addChild.Enabled = false;
            }
		}

		public TreeViewAdvEditorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//			PopulateTreeView();

		}

        internal TreeViewAdv Tree
        {
            get
            {
                return this.originalTree;
            }
        }
//
//		private void CopyNodes(TreeNodeAdv from,TreeNodeAdv to)
//		{
//			to = from.Clone();
//			to.Nodes = new TreeNodeAdvCollection();
//
//			for(int i=0;i<from.Nodes.Count;i++)
//			{
//				TreeNodeAdv newNode = new TreeNodeAdv();
//				to.Nodes.Add(newNode);
//				CopyNodes(from.Nodes[i],to.Nodes[i]);
//			}
//		}
		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.pgMenu.Dispose();
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.addNode = new System.Windows.Forms.Button();
			this.addChild = new System.Windows.Forms.Button();
			this.propertyGrid1 = new TreeViewAdvEditorPropertyGrid( m_provider );
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.tree = new TreeViewAdvEditor();
			this.Remove = new System.Windows.Forms.Button();
			this.gradientPanel1 = new Syncfusion.Windows.Forms.Tools.GradientPanel();
			this.button1 = new System.Windows.Forms.Button();
			this.select = new System.Windows.Forms.CheckBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
			this.gradientPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			// addNode
			//
			this.addNode.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addNode.Location = new System.Drawing.Point(8, 12);
			this.addNode.Name = "addNode";
			this.addNode.Size = new System.Drawing.Size(96, 23);
			this.addNode.TabIndex = 1;
			this.addNode.Text = "Add Node";
			this.addNode.Click += new System.EventHandler(this.addNode_Click);
			//
			// addChild
			//
			this.addChild.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.addChild.Location = new System.Drawing.Point(112, 12);
			this.addChild.Name = "addChild";
			this.addChild.Size = new System.Drawing.Size(88, 23);
			this.addChild.TabIndex = 2;
			this.addChild.Text = "Add Child";
			this.addChild.Click += new System.EventHandler(this.addChild_Click);
			//
			// propertyGrid1
			//
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(208, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(232, 422);
			this.propertyGrid1.TabIndex = 3;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			//
			// OK
			//
			this.OK.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.OK.Location = new System.Drawing.Point(220, 52);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(112, 23);
			this.OK.TabIndex = 4;
			this.OK.Text = "OK";
			this.OK.Click += new System.EventHandler(this.OK_Click);
			//
			// Cancel
			//
			this.Cancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.Cancel.Location = new System.Drawing.Point(340, 52);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(96, 23);
			this.Cancel.TabIndex = 5;
			this.Cancel.Text = "Cancel";
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			//
			// tree
			//
			this.tree.AllowDrop = true;
			this.tree.BackColor = System.Drawing.SystemColors.Window;
			this.tree.BorderColor = System.Drawing.Color.Black;
			this.tree.Dock = System.Windows.Forms.DockStyle.Left;
			this.tree.Font = new System.Drawing.Font("Verdana", 8F);
			//
			// tree.HelpTextControl
			//
			this.tree.HelpTextControl.Border3DStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.tree.HelpTextControl.BorderSingle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.tree.HelpTextControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tree.HelpTextControl.Location = new System.Drawing.Point(732, 0);
			this.tree.HelpTextControl.Name = "helpText";
			this.tree.HelpTextControl.RestrictWidth = 0;
			this.tree.HelpTextControl.Size = new System.Drawing.Size(50, 17);
			this.tree.HelpTextControl.TabIndex = 0;
			this.tree.HelpTextControl.Text = "help text";
			this.tree.HorizontalThumbTrack = true;
			this.tree.HScrollPos = 0;
			this.tree.Indent = 20;
			this.tree.Name = "tree";
			this.tree.PathSeparator = "/";
			this.tree.Size = new System.Drawing.Size(200, 422);
			this.tree.TabIndex = 6;
			this.tree.ThemesEnabled = true;
			//
			// tree.ToolTipControl
			//
			this.tree.ToolTipControl.BackColor = System.Drawing.SystemColors.Info;
			this.tree.ToolTipControl.Border3DStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.tree.ToolTipControl.BorderSingle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.tree.ToolTipControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tree.ToolTipControl.Location = new System.Drawing.Point(642, 0);
			this.tree.ToolTipControl.Name = "toolTip";
			this.tree.ToolTipControl.RestrictWidth = 0;
			this.tree.ToolTipControl.Size = new System.Drawing.Size(41, 17);
			this.tree.ToolTipControl.TabIndex = 1;
			this.tree.ToolTipControl.Text = "toolTip";
			this.tree.ToolTipControl.Visible = false;
			this.tree.VerticalThumbTrack = true;
			this.tree.VScrollPos = 1;
			this.tree.AfterSelect += new EventHandler(this.tree_AfterSelect);
			//
			// Remove
			//
			this.Remove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.Remove.Location = new System.Drawing.Point(8, 52);
			this.Remove.Name = "Remove";
			this.Remove.Size = new System.Drawing.Size(192, 23);
			this.Remove.TabIndex = 7;
			this.Remove.Text = "Remove";
			this.Remove.Click += new System.EventHandler(this.Remove_Click);
			//
			// gradientPanel1
			//
			this.gradientPanel1.BorderColor = System.Drawing.Color.Black;
			this.gradientPanel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gradientPanel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.button1,
																						 this.addChild,
																						 this.OK,
																						 this.addNode,
																						 this.Cancel,
																						 this.Remove,
																						 this.select});
			this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            //this.gradientPanel1.BackgroundColor = new BrushInfo(
            //    GradientStyle.Vertical,
            //    new Color[] {	System.Drawing.Color.LightSteelBlue,
            //                    System.Drawing.Color.RoyalBlue,
            //                    System.Drawing.Color.CornflowerBlue,
            //                    System.Drawing.Color.CornflowerBlue,
            //                    System.Drawing.Color.CornflowerBlue,
            //                    System.Drawing.Color.MidnightBlue	} );
			this.gradientPanel1.Location = new System.Drawing.Point(0, 422);
			this.gradientPanel1.Name = "gradientPanel1";
			this.gradientPanel1.Size = new System.Drawing.Size(440, 80);
			this.gradientPanel1.TabIndex = 8;
			//
			// button1
			//
			this.button1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.button1.Location = new System.Drawing.Point(356, 12);
			this.button1.Name = "button1";
			this.button1.TabIndex = 9;
			this.button1.Text = "Sort";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			//
			// select
			//
			this.select.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.select.BackColor = System.Drawing.Color.Transparent;
			this.select.Checked = true;
			this.select.CheckState = System.Windows.Forms.CheckState.Checked;
			this.select.Location = new System.Drawing.Point(220, 8);
			this.select.Name = "select";
			this.select.Size = new System.Drawing.Size(128, 24);
			this.select.TabIndex = 8;
			this.select.Text = "Select added node";
			this.select.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// splitter1
			//
			this.splitter1.Location = new System.Drawing.Point(200, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 422);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			//
			// TreeViewAdvEditorForm
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 502);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.propertyGrid1,
																		  this.splitter1,
																		  this.tree,
																		  this.gradientPanel1});
			this.MinimumSize = new System.Drawing.Size(448, 100);
			this.Name = "TreeViewAdvEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TreeViewAdv NodeCollection Editor";
			this.Load += new System.EventHandler(this.TreeViewAdvEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
			this.gradientPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void addNode_Click(object sender, System.EventArgs e)
		{
			TreeNodeAdv node = new TreeNodeAdv("Node"+tree.NodeCount.ToString());

			if(tree.SelectedNode!=null && tree.SelectedNode.Parent!=null)
			{
				this.tree.SelectedNode.Parent.Nodes.Add(node);
			}
			else
			{
				tree.Nodes.Add(node);
			}
			tree.NodeCount++;
			if(this.select.Checked) tree.SelectedNode = node;
			tree.Invalidate();
		}

		private void addChild_Click(object sender, System.EventArgs e)
		{
			TreeNodeAdv node = new TreeNodeAdv("Node"+tree.NodeCount.ToString());
			if(tree.SelectedNode!=null)
			{
				this.tree.SelectedNode.Nodes.Add(node);
				tree.NodeCount++;
				this.tree.SelectedNode.Expand();
			}
			if(this.select.Checked) tree.SelectedNode = node;
			tree.Invalidate();
		}

		private void OK_Click(object sender, System.EventArgs e)
		{
			originalTree.Root = tree.Root.Clone();
            UpdateCustomControls();
			originalTree.MakeDirty();
			originalTree.Root.Visible = true;
			originalTree.NodeCount = tree.NodeCount;
            originalTree.Root.TreeView.NeedUpdateCustomControls = true;
			this.Close();
		}

        private void UpdateCustomControls()
        {
            if (originalTree.m_RemovedCustomControls.Count > 0)
            {
                ArrayList controlsToBeAddedBack = new ArrayList();
                controlsToBeAddedBack.AddRange(originalTree.m_RemovedCustomControls);

                foreach (Control ctrl in controlsToBeAddedBack)
                {
                    Hashtable entryToBeRemoved = new Hashtable();

                    //Restoring original location
                    foreach (DictionaryEntry entry in originalTree.m_ControlBounds)
                    {
                        if (entry.Key == ctrl)
                        {
                            Rectangle? bounds = entry.Value as Rectangle? ?? ctrl.Bounds;
                            ctrl.Bounds = (Rectangle)bounds;                            
                            entryToBeRemoved.Add(entry.Key, entry.Value);
                            break;
                        }
                    }

                    if (entryToBeRemoved.Count > 0)
                    {
                        foreach (DictionaryEntry entry in entryToBeRemoved)
                        {
                            if (originalTree.m_ControlBounds.ContainsKey(entry.Key))
                                originalTree.m_ControlBounds.Remove(entry.Key);
                        }
                        entryToBeRemoved.Clear();
                    }

                    //Restoring original parent
                    foreach (DictionaryEntry entry in originalTree.m_ControlParent)
                    {
                        if (entry.Key == ctrl)
                        {
                            ctrl.Parent = entry.Value as Control;
                            originalTree.m_RemovedCustomControls.Remove(ctrl);
                            entryToBeRemoved.Add(entry.Key, entry.Value);
                            if (ctrl.Parent != null)
                                ctrl.Parent.Update();
                            else
                                ctrl.Parent = originalTree.Parent;

                            break;
                        }
                    }

                    if (entryToBeRemoved.Count > 0)
                    {
                        foreach (DictionaryEntry entry in entryToBeRemoved)
                        {
                            if (originalTree.m_ControlParent.ContainsKey(entry.Key))
                                originalTree.m_ControlParent.Remove(entry.Key);                            
                        }
                        entryToBeRemoved.Clear();
                    }
                }
                originalTree.m_RemovedCustomControls.Clear();
                controlsToBeAddedBack.Clear();

                if (originalTree.Parent != null)
                    originalTree.Parent.Update();
            }
        }

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void tree_AfterSelect(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = tree.SelectedNode;
		}

		private void Remove_Click(object sender, System.EventArgs e)
		{
			if(tree.SelectedNode!=null && tree.SelectedNode.Parent!=null)
			{
				TreeNodeAdv parent = tree.SelectedNode.Parent;
				tree.SelectedNode.Parent.Nodes.Remove(tree.SelectedNode);
//				tree.SelectedNode = parent;
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if(this.tree.SelectedNode!=null)
				this.tree.SelectedNode.Sort();
		}

		private void TreeViewAdvEditorForm_Load(object sender, System.EventArgs e)
		{
		}

        private void SelectedNodes_CollectionChanged( object sender, CollectionChangeEventArgs e )
        {
            if( this.tree.SelectedNodes.Count > 0 )
            {
                this.addChild.Enabled = true;
            }
            else
            {
                this.addChild.Enabled = false;
            }
        }

		#region SOURCE_DRAG_DROP
		private TreeViewAdvDragHighlightTracker treeViewDragHighlightTracker = null;

		private void treeViewAdv_ItemDrag(object sender, ItemDragEventArgs e)
		{
			TreeViewAdv treeViewAdv = sender as TreeViewAdv;

			// The TreeViewAdv always provides an array of selected nodes.
			TreeNodeAdv[] nodes = e.Item as TreeNodeAdv[];

			// Let us get only the first selected node.
			TreeNodeAdv node = nodes[0];

			// Only allow move
			DragDropEffects result = treeViewAdv.DoDragDrop(node, DragDropEffects.Move);
		}
		private void treeViewAdv_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
		{
			// Cancel draggin when Escape was pressed.
			if(e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
			}
		}

		#endregion SOURCE_DRAG_DROP
		#region DEST_DRAGDROP_LOGIC
		// Helps keep track of the node that is being dragged.
		private TreeNodeAdv currentSourceNode;
		private void treeViewAdv_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// Determine drag effects
			bool droppable = true;
			TreeNodeAdv destinationNode = null;
			TreeViewAdv treeView = sender as TreeViewAdv;
			Point ptInTree = treeView.PointToClient(new Point(e.X, e.Y));
			this.currentSourceNode = null;

			// Looking for a single tree node.
			if( e.Data.GetDataPresent(typeof(TreeNodeAdv)))
			{
				// Get the destination and source node.
				destinationNode = treeView.GetNodeAtPoint(ptInTree);
				if( destinationNode == null )
				{
					destinationNode = treeView.Root;
				}
				TreeNodeAdv sourceNode = (TreeNodeAdv) e.Data.GetData(typeof(TreeNodeAdv));
				// Cache this for use later in the TreeDragDrop_QueryAllowedPositionsForNode event handler.
				this.currentSourceNode = sourceNode;
				droppable = this.CanDrop(sourceNode, destinationNode);
			}
			else
				droppable = false;

			if(droppable)
				// If Moving is allowed:
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.None;

			// Let the highlight tracker keep track of the current highlight node.
			this.treeViewDragHighlightTracker.SetHighlightNode(destinationNode, ptInTree);
		}
		private bool CanDrop(TreeNodeAdv sourceNode, TreeNodeAdv destinationNode)
		{
			if(// Support drag and drop only within the same tree
				sourceNode.TreeView != this.tree ||
				// Cannot drop into empty area
				destinationNode == null ||
				// Cannot drop over the source's parent
				destinationNode == sourceNode.Parent ||
				// Or over itself
				destinationNode == sourceNode
				)
				return false;
			else
				return true;
		}
		private void treeViewAdv_DragLeave(object sender, System.EventArgs e)
		{
			// Let the highlight tracker keep track of the current highlight node.
			this.treeViewDragHighlightTracker.ClearHighlightNode();
		}

		private void treeViewAdv_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			TreeViewAdv treeView = sender as TreeViewAdv;

			// Get the destination and source node.

			TreeNodeAdv sourceNode = (TreeNodeAdv) e.Data.GetData(typeof(TreeNodeAdv));

			TreeNodeAdv destinationNode = this.treeViewDragHighlightTracker.HighlightNode;

            if( this.IsChildNode( sourceNode, destinationNode ) )
            {
                this.treeViewDragHighlightTracker.ClearHighlightNode();
                return;
            }

			TreeViewDropPositions dropPosition = this.treeViewDragHighlightTracker.DropPosition;
			// Clear the highlight info in the tracker.
			this.treeViewDragHighlightTracker.ClearHighlightNode();

			this.currentSourceNode = null;

			// Move the source node based on the tracked info.
			if(destinationNode != null)
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

        // Specifiy the allowed drop positions for the specified highlight node.
        private void TreeDragDrop_QueryAllowedPositionsForNode(object sender, QueryAllowedPositionsEventArgs e)
		{
			if(e.HighlightNode != this.currentSourceNode)
				e.AllowedPositions = TreeViewDropPositions.AboveNode | TreeViewDropPositions.BelowNode
					| TreeViewDropPositions.OnNode;
			else
				// Cannot drop beside itself
				e.AllowedPositions = TreeViewDropPositions.None;

			e.ShowSelectionHighlight =
				// Only if the source node is droppable
				this.CanDrop(this.currentSourceNode, e.HighlightNode)
				// and droppable ON the node (not beside it)
				&& e.NewDropPosition == TreeViewDropPositions.OnNode;
		}

        /// <summary>
        /// Checks whether destinationNode is child node of the sourceNode.
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="destinatioNode"></param>
        /// <returns></returns>
        private bool IsChildNode( TreeNodeAdv sourceNode, TreeNodeAdv destinatioNode )
        {
            bool retValue = false;
            for( int i = 0; i < sourceNode.Nodes.Count; i++ )
            {
                if( sourceNode.Nodes[ i ] == destinatioNode )
                {
                    retValue = true;
                }
                else
                {
                    retValue = IsChildNode( sourceNode.Nodes[ i ], destinatioNode );
                }

                if( retValue == true )
                    break;
            }
            return retValue;
        }
		#endregion DEST_DRAGDROP_LOGIC
	}

	class TreeViewAdvEditor : TreeViewAdv
	{
		internal TreeViewAdvEditor()
		{
			this.m_bAutoControlsAdding = false; //must be false for designer editor.

		}

		/// <summary>
		/// Controls bitmaps.
		/// Key - control.
		/// value - bitmap.
		/// </summary>
		private Hashtable m_htControlsBitmaps = new Hashtable();

		/// <summary>
		/// Paint control in bitmap if need and save bitmap in collection.
		/// If Control has been painted return bitmap from collection.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="controlBounds"></param>
		/// <returns></returns>
		private Bitmap GetControlBitmap( Control control, Rectangle controlBounds )
		{
			Bitmap bt = null;

			if( m_htControlsBitmaps.ContainsKey( control ) )
			{
				bt = (Bitmap) m_htControlsBitmaps[ control ];
			}
			else
			{
				bool lastVisible = control.Visible;
				Point lastLocation = control.Location;

				control.Size = controlBounds.Size;

				if( !lastVisible )
				{
					control.Location = new Point( -1000, -1000 );
					control.Visible = true;
				}
				
				bt = ActiveXSnapshot.PrintWindow( control );
				m_htControlsBitmaps.Add( control, bt );

				if( !lastVisible )
				{
					control.Visible = lastVisible;
					control.Location = lastLocation;
				}
			}

			return bt;
		}

		internal override int DrawNode(Graphics g, Rectangle clip, TreeNodeAdv node, int y, int level, Point mousePos, bool mouseDown, bool background)
		{
			int rezult = base.DrawNode (g, clip, node, y, level, mousePos, mouseDown, background);
			
			if( node.CustomControl != null )
			{
				Rectangle controlBounds = node.GetCustomControlBounds();
			

				g.DrawImage( GetControlBitmap( node.CustomControl, controlBounds ), controlBounds );
			}

			return rezult;
		}

		internal override bool NeedUpdateCustomControls
		{
			get
			{
				// must be false in designer editor.
				return false;
			}
			set
			{
				base.NeedUpdateCustomControls = value;
			}
		}

		/// </override>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(Control.MouseButtons == MouseButtons.Left)
			{
				// IN the designer tree, force a disabled node to be selected:
				TreeNodeAdv mouseDownNode = this.GetNodeAtPoint(e.X, e.Y);
				if( mouseDownNode != null && mouseDownNode != this.Root
					&& mouseDownNode.Enabled == false
					&& !this.SelectedNodes.Contains(mouseDownNode))
				{
					if(this.SetSelectedNode(mouseDownNode, this.SelectedNodes, TreeViewAdvAction.ByMouse))
					{
						this.ActiveNode = mouseDownNode;
						// Important that you return here, otherwise the behavior is as if
						// the user clicked on a selected node.
						//return;
					}
				}
			}
			base.OnMouseDown(e);
		}

	}
}
