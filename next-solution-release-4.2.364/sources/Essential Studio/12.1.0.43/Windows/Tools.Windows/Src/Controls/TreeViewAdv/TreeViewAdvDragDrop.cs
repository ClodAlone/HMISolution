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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.Serialization;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the position where a drop can occur during drag-and-drop.
	/// </summary>
	[System.Flags()]
	public enum TreeViewDropPositions
	{
		None = 0,
		OnNode = 1,
		AboveNode = 2,
		BelowNode = 4,
		All = OnNode | AboveNode | BelowNode
	}

	/// <summary>
	/// Provides data for the <see cref="TreeViewAdvDragHighlightTracker.QueryDragInsertInfo"/>
	/// event.
	/// </summary>
	public class QueryDragInsertInfoEventArgs : EventArgs
	{
		#region Fields
		private Color m_clrDragInsertColor = Color.Black;
		private TreeNodeAdv m_nodeTree;
		#endregion Fields

		#region Properties
        /// <summary>
        /// The Color of DragHighLightTracker can be changed based on the tree's background Color 
        /// </summary>
		public Color DragInsertColor
		{
			get
			{
				return m_clrDragInsertColor;
			}
			set
			{
				if ( m_clrDragInsertColor != value )
					m_clrDragInsertColor = value;
			}
		}

		public TreeNodeAdv Node
		{
			get { return m_nodeTree; }
		}

		#endregion Properties

		#region Constructor
		public QueryDragInsertInfoEventArgs( TreeNodeAdv node )
		{
			m_nodeTree = node;	
		}

		#endregion Constructor
	}

	/// <summary>
	/// Provides data for the <see cref="TreeViewAdvDragHighlightTracker.QueryAllowedPositionsForNode"/>
	/// event.
	/// </summary>
	public class QueryAllowedPositionsEventArgs : EventArgs
	{
		private TreeNodeAdv highLightNode;
		private TreeViewDropPositions highlightPosition;
		private TreeViewDropPositions allowedPositions;
		private bool showSelectionHighlight = true;

		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="highLightNode">The node over which the mouse is during drag-and-drop.</param>
		/// <param name="highlightPosition">The computed drop-position.</param>
		/// <param name="allowedPositions">The allowed drop-positions.</param>
		public QueryAllowedPositionsEventArgs(TreeNodeAdv highLightNode,
			TreeViewDropPositions highlightPosition, TreeViewDropPositions allowedPositions)
		{
			this.highLightNode = highLightNode;
			this.highlightPosition = highlightPosition;
			this.allowedPositions = allowedPositions;
			this.showSelectionHighlight = true;
		}
		/// <summary>
		/// Indicates whether the drag over node should be drawn with the selection highlight.
		/// </summary>
		/// <value>True to show the highlight; false otherwise. Default is true.</value>
		[DefaultValue(true)]
		public bool ShowSelectionHighlight
		{
			get{return this.showSelectionHighlight;}
			set
			{
				this.showSelectionHighlight = value;
			}
		}
		/// <summary>
		/// Returns the currently highlighted node.
		/// </summary>
		public TreeNodeAdv HighlightNode{get{return this.highLightNode;}}
		/// <summary>
		/// Returns the computed drop position.
		/// </summary>
		public TreeViewDropPositions NewDropPosition{get{return this.highlightPosition;}}
		/// <summary>
		/// Gets / sets the allowed drop-positions.
		/// </summary>
		/// <remarks>Change this value if you want to prevent drop in the
		/// <see cref="NewDropPosition"/>.</remarks>
		public TreeViewDropPositions AllowedPositions
		{
			get{return this.allowedPositions;}
			set
			{
				this.allowedPositions = value;
			}
		}
	}

	/// <summary>
	/// TreeViewAdv Drag and Drop UI helper class.
	/// </summary>
	/// <remarks>
	/// <p>Use this class (will be referred to as "tracker" below) only when you want to support dropping adjacent (above or below) to a node.
	/// Otherwise simply listen to the <see cref="System.Windows.Forms.Control.DragOver"/> and <see cref="System.Windows.Forms.Control.DragDrop"/> events and provide/use the <see cref="System.Windows.Forms.DropEffect"/>.
	/// When used, this tracker will draw indicators above or below a node while dragging an item
	/// over the node. The tracker needs to be updated as shown below in the DragXXX events.</p>
	/// <p>
	/// The usage semantics for the tracker is as follows:
	/// </p>
	/// <list type="number">
	/// <item><description>Create an instance of this class and listen to the <see cref="QueryAllowedPositionsForNode"/> event.</description></item>
	/// <item><description>In the <see cref="TreeViewAdv"/>'s <b>DragOver</b> event update the highlighted node with a call to the
	/// <see cref="SetHighlightNode"/> method.</description></item>
	/// <item><description>In the <see cref="TreeViewAdv"/>'s <b>DragLeave</b> event clear the highlighted node with a call to the
	/// <see cref="ClearHighlightNode"/> method.</description></item>
	/// <item><description>In the <see cref="QueryAllowedPositionsForNode"/> event handler, specify
	/// whether a computed adjacent position should be allowed.</description></item>
	/// <item><description>In the <see cref="TreeViewAdv"/>'s <b>DragDrop</b> event get the highlighted node
	/// and the drop-position from the tracker and insert the new node(s) accordingly.</description></item>
	/// </list>
	/// </remarks>
	/// <example>
	/// Please take a look at our sample in the ..\Essential Tools\Samples\Tree Package\TreeViewAdvDragDrop folder
	/// for more information.
	/// </example>
	public class TreeViewAdvDragHighlightTracker : IDisposable, ITreeNodeAdvPaintFilter
	{
		#region VARIABLES
		TreeViewAdv tree;
		TreeNodeAdv highlightNode = null;
		TreeViewDropPositions dropPosition = TreeViewDropPositions.None;
		int waitIntervalBeforeExpand = 750;
		System.Windows.Forms.Timer currentExpandTimer = null;
		int edgeSensitivity = 0;
		int edgeSensitivityTop = -1;
		int edgeSensitivityBottom = -1;
		bool hotTrackDragOverNode = true;
		#endregion VARIABLES

		#region INIT
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="tree">The <see cref="TreeViewAdv"/> instance where drag-and-drop will occur.</param>
		/// <remarks>
		///
		/// </remarks>
		public TreeViewAdvDragHighlightTracker(TreeViewAdv tree)
		{
			this.tree = tree;
			this.tree.paintFilter = this;
		}

		void IDisposable.Dispose()
		{
			if(this.currentExpandTimer != null)
				this.CurrentExpandTimer = null;
		}
		#endregion INIT

		#region HIGHLIGHT_NODE_TRACKER
		/// <summary>
		/// Occurs before drawing a highlight indicator above or below a node.
		/// </summary>
		/// <remarks>You can listen to this event and prevent drawing highlights
		/// for some specific nodes.</remarks>
		public event QueryAllowedPositionsEventHandler QueryAllowedPositionsForNode;

        /// <summary>
        /// Occurs before drawing a DragInsert position.
        /// </summary>
        /// <remarks>You can listen to this event and change DragInsert highlight color.</remarks>
        /// 
		public event QueryDragInsertInfoEventHandler QueryDragInsertInfo;

		/// <summary>
		/// This property will soon be replaced by the
		/// <see cref="EdgeSensitiviyOnTop"/> and <see cref="EdgeSensitiviyAtBottom"/>
		/// properties, please use them instead.
		/// </summary>
		/// <value>Default is zero.</value>
		[DefaultValue(0)]
		[Obsolete("This property will soon be replaced by the EdgeSensitiviyOnTop and EdgeSensitiviyAtBottom properties, please use them instead.")]
		public int EdgeSensitivity
		{
			get{return this.edgeSensitivity;}
			set
			{
				this.EdgeSensitivityOnTop = value;
				this.EdgeSensitivityAtBottom = value;
			}
		}

		/// <summary>
		/// Gets / sets a height for the top edge of a node while dragging over.
		/// </summary>
		/// <value>Default is -1.</value>
		/// <remarks>
		/// <p>
		/// By default (when value is negative), the top 1/3 of the node will be considered the top edge.
		/// This value will be used when you allow dropping before a node using 
        /// the <see cref="QueryAllowedPositionsEventArgs.AllowedPositions"/> property. 
        /// Set this to zero if you never want to drop on top of a node.
		/// </p>
		/// </remarks>
		[DefaultValue(-1)]
		public int EdgeSensitivityOnTop
		{
			get{return this.edgeSensitivityTop;}
			set
			{
				this.edgeSensitivityTop = value;
			}
		}

		/// <summary>
		/// Gets / sets a height for the bottom edge of a node while dragging over.
		/// </summary>
		/// <value>Default is -1.</value>
		/// <remarks>
		/// <p>
		/// By default (when value is negative), the bottom 1/3 of the node will be considered the bottom edge.
		/// This value will be used when you allow dropping below a node using the 
        /// <see cref="QueryAllowedPositionsEventArgs.AllowedPositions"/> property. 
        /// Set this to zero if you never want to drop below a node.
		/// </p>
		/// </remarks>
		[DefaultValue(-1)]
		public int EdgeSensitivityAtBottom
		{
			get{return this.edgeSensitivityBottom;}
			set
			{
				this.edgeSensitivityBottom = value;
			}
		}

		/// <summary>
		/// Indicates whether the drag over node should be drawn with the selected background.
		/// </summary>
		/// <default>True to draw selected; false otherwise. Default is true.</default>
		private bool HotTrackDragOverNode
		{
			get{return this.hotTrackDragOverNode;}
			set
			{
				this.hotTrackDragOverNode = value;
			}
		}


		/// <summary>
		/// Returns the node over which the mouse is hovering for drop.
		/// </summary>
		public TreeNodeAdv HighlightNode
		{
			get{return this.highlightNode;}
		}
		/// <summary>
		/// Returns the computed drop-position around the highlight node.
		/// </summary>
		public TreeViewDropPositions DropPosition
		{
			get{return this.dropPosition;}
		}

		/// <summary>
		/// Clears the current highlight node setting.
		/// </summary>
		/// <remarks>Call this method from the TreeViewAdv's <b>DragLeave</b> and <b>DragDrop</b>(after
		/// you get the highlight information from the tracker class) events.</remarks>
		public void ClearHighlightNode()
		{
			if(this.highlightNode != null)
				this.tree.Invalidate(this.highlightNode.Bounds);
			this.highlightNode = null;
			dropPosition = TreeViewDropPositions.None;
		}

		/// <summary>
		/// Sets the node over which the mouse is currently hovering during drag-drop.
		/// </summary>
		/// <param name="highlightNode">The new highlight node.</param>
		/// <param name="ptInTree">The point in tree where the mouse is in the tree view's client co-ordinates.</param>
		/// <remarks>
		/// Call this method from the TreeViewAdv's <b>DragOver</b> method.
		/// </remarks>
		public void SetHighlightNode(TreeNodeAdv highlightNode, Point ptInTree)
		{
			if(highlightNode == null)
			{
				this.ClearHighlightNode();
				return;
			}

			TreeViewDropPositions newPosition = TreeViewDropPositions.OnNode;

			Rectangle nodeBounds = highlightNode.Bounds;
			int topEdgeHeight = this.EdgeSensitivityOnTop;
			if(topEdgeHeight == -1)
				topEdgeHeight = nodeBounds.Height / 3;

			if(ptInTree.Y >= nodeBounds.Top && ptInTree.Y <= (nodeBounds.Top + topEdgeHeight))
				newPosition = TreeViewDropPositions.AboveNode;
			else
			{
				int bottomEdgeHeight = this.EdgeSensitivityAtBottom;
				if(bottomEdgeHeight == -1)
					bottomEdgeHeight = nodeBounds.Height / 3;

				if(ptInTree.Y >= (nodeBounds.Bottom - bottomEdgeHeight)
					&& ptInTree.Y < nodeBounds.Bottom)
					newPosition = TreeViewDropPositions.BelowNode;
			}

			if(this.highlightNode != highlightNode || this.dropPosition != newPosition)
			{
				if(this.highlightNode != null)
				{
					Rectangle oldBounds = this.highlightNode.Bounds;
					oldBounds.Inflate(0, 5);
					this.tree.Invalidate(oldBounds);
				}
				this.highlightNode = highlightNode;
				this.dropPosition = newPosition;
				this.hotTrackDragOverNode = true;

				// Query for the allowed drop positions for this node.
				if(this.highlightNode !=null)
				{
					QueryAllowedPositionsEventArgs args =
						new QueryAllowedPositionsEventArgs(this.highlightNode, this.dropPosition,
						TreeViewDropPositions.All);

					if(this.QueryAllowedPositionsForNode != null)
						this.QueryAllowedPositionsForNode(this, args);

					this.dropPosition = this.dropPosition & args.AllowedPositions;
					this.hotTrackDragOverNode = args.ShowSelectionHighlight;
				}

				if(this.WaitTimeBeforeExpand >= 0)
					this.CurrentExpandTimer = new System.Windows.Forms.Timer();

				if(this.highlightNode != null)
				{
					Rectangle htNodeBounds = this.highlightNode.Bounds;
					htNodeBounds.Inflate(0, 5);
					this.tree.Invalidate(htNodeBounds);
				}

				// Also redraw the selected node
				if(this.tree.SelectedNodes.Count > 0)
					this.tree.Invalidate(this.tree.SelectedNodesBounds);
			}
		}
		#endregion HIGHLIGHT_NODE_TRACKER

		#region HIGHLIGHT_NODE_EXPANDER
		/// <summary>
		/// Gets / sets the time interval after which a node will be expanded on mouse
		/// hover during drag and drop.
		/// </summary>
		/// <value>Time in milliseconds. Default is 750.</value>
		/// <remarks>
		/// If you do not want a node to expand on drag over, set this value to -1.
		/// </remarks>
		[DefaultValue(750),
		Description("Specifies the time interval after which a node will be expanded on mouse hover during drag and drop.")
		]
		public int WaitTimeBeforeExpand
		{
			get{return this.waitIntervalBeforeExpand;}
			set
			{this.waitIntervalBeforeExpand = value;}
		}
		// Using a new timer every time to workaround 1.1 bug.
		internal Timer CurrentExpandTimer
		{
			get{return this.currentExpandTimer;}
			set
			{
				if(this.currentExpandTimer != value)
				{
					if(this.currentExpandTimer != null)
					{
						this.currentExpandTimer.Tick -= new EventHandler(this.ExpandTimer_Tick);
						this.currentExpandTimer.Stop();
						this.currentExpandTimer.Dispose();
					}

					this.currentExpandTimer = value;

					if(this.currentExpandTimer != null)
					{
						this.currentExpandTimer.Interval = this.waitIntervalBeforeExpand;
						this.currentExpandTimer.Tick += new EventHandler(this.ExpandTimer_Tick);
						this.currentExpandTimer.Start();
					}
				}
			}
		}
		private void ExpandTimer_Tick(object sender, EventArgs e)
		{
			// We used to expand only if the drop position was OnNode, upon customer request
			// we now do it for any kind of drop position.
			if(this.highlightNode != null)
			{
				this.highlightNode.Expand();
			}
			this.CurrentExpandTimer = null;
		}
		#endregion HIGHLIGHT_NODE_EXPANDER

		#region PAINTING
		bool ITreeNodeAdvPaintFilter.OnBeforeNodePaint(TreeNodeAdvPaintEventArgs e)
		{
			if(this.HighlightNode == null)
				return false;

			if(e.Node == this.HighlightNode && this.HotTrackDragOverNode)
				e.ForeColor = this.tree.SelectedNodeForeColor;
			else if(this.tree.SelectedNodes.Contains(e.Node))
			{
				e.Selected = false;
				e.HotTracked = false;
				e.ForeColor = e.Node.GetForeColor(false, false);
			}

			return false;
		}

		bool ITreeNodeAdvPaintFilter.OnNodeBackgroundPaint(TreeNodeAdvPaintBackgroundEventArgs e)
		{
			if(this.HighlightNode == null)
				return false;

			if(e.Node == this.HighlightNode && this.HotTrackDragOverNode)
			{
				e.BrushInfo = this.tree.SelectedNodeBackground;
				return true;
			}
			else if(this.tree.SelectedNodes.Contains(e.Node))
			{
				e.Selected = false;
				e.BrushInfo = BrushInfo.Empty;
			}
			return false;
		}

		void ITreeNodeAdvPaintFilter.OnAfterNodePaint(TreeNodeAdvPaintEventArgs e)
		{
			if(this.HighlightNode == null)
				return;

			if(e.Node == this.HighlightNode)
			{
				//Draw the black line when dragging
				if(this.dropPosition == TreeViewDropPositions.AboveNode
					|| this.dropPosition == TreeViewDropPositions.BelowNode)
				{
					TreeNodeAdv node = e.Node;
					if(node !=null)
					{
						// Expects some gap for some reason, otherwise, paints incorrectly.
						int xGap = 2;

						int nNodeX, nWidth;
						if (node.GetIsMirrored())
						{
							int nBordWidth = node.TreeView.BorderStyle == BorderStyle.Fixed3D ? 2 : 1;
							nNodeX = xGap + 2*nBordWidth;
							int nRight = node.NodeX - xGap;
							nWidth = nRight - nNodeX;
						}
						else
						{
							nNodeX = node.NodeX + xGap;
							nWidth = this.tree.Width-nNodeX-10-xGap-(this.tree.VScrollBar.Enabled?SystemInformation.VerticalScrollBarWidth:0);
						}

						int nY = ( this.dropPosition == TreeViewDropPositions.AboveNode ?
							node.Bounds.Top : node.Bounds.Bottom );

						Color clrDragInsertPen = GetDrawInsertColor(e.Node);

						this.DrawDragInsert( e.Graphics, nNodeX, nY, nWidth, clrDragInsertPen );
					}
				}
			}
		}

		private void DrawDragInsert(Graphics g,int x,int y,int width, Color clrDragInsertPen )
		{
			Pen pen = new Pen( clrDragInsertPen, 2 );
			g.DrawLines(pen,new Point[]{
										   new Point(x,y),
										   new Point(width+x,y),
										   new Point(width+x+2,y-2),
										   new Point(width+x+2,y+2),
										   new Point(width+x,y),
										   new Point(x,y),
										   // Note, it's just -1 not -2 for some reason!
										   new Point(x-1,y-1),
										   new Point(x-1,y+1),
										   new Point(x,y)
									   });

		}

		private Color GetDrawInsertColor( TreeNodeAdv node )
		{
			QueryDragInsertInfoEventArgs args = new QueryDragInsertInfoEventArgs( node );

			if ( this.QueryDragInsertInfo != null )
				this.QueryDragInsertInfo( this, args );

			return args.DragInsertColor;
		}

		#endregion PAINTING
	}
	/// <summary>
	/// Handles the <see cref="TreeViewAdv.QueryAllowedPositionsForNode"/> event.
	/// </summary>
	public delegate void QueryAllowedPositionsEventHandler(object sender, QueryAllowedPositionsEventArgs args);

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.QueryDragInsertInfo"/> event.
	/// </summary>
	public delegate void QueryDragInsertInfoEventHandler( object sender, QueryDragInsertInfoEventArgs args );
}