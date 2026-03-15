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
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools
{

	public class TreeViewAdvSelectionEventArgs : EventArgs
	{
		SelectedNodesCollection selectedNodes;
		TreeViewAdvAction action;
		
		public TreeViewAdvSelectionEventArgs(SelectedNodesCollection selectedNodes, TreeViewAdvAction action)
		{
			this.selectedNodes = selectedNodes;
			this.action = action;
		}
		public SelectedNodesCollection SelectedNodes
		{
			get{return this.selectedNodes;}
		}
		public TreeViewAdvAction Action
		{
			get{return this.action;}
		}
	}

	/// <summary>
	/// Provides data for the <see cref="TreeViewAdv.BeforeSelect"/> event.
	/// </summary>
	public class TreeViewAdvCancelableSelectionEventArgs : TreeViewAdvSelectionEventArgs
	{
		bool cancel;
		public TreeViewAdvCancelableSelectionEventArgs(SelectedNodesCollection selectedNodes, TreeViewAdvAction action, bool cancel)
			: base(selectedNodes, action)
		{
			this.cancel = cancel;
		}

		public bool Cancel
		{
			get{return this.cancel;}
			set{this.cancel = value;}
		}
	}

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeExpand"/> event of the TreeViewAdv control.
	/// </summary>
	public delegate void TreeViewAdvNodeEventHandler(object sender, TreeViewAdvNodeEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="TreeViewAdv.AfterCollapse"/> event.
    /// </summary>
	public class TreeViewAdvNodeEventArgs : EventArgs
	{
		TreeNodeAdv node;

		public TreeViewAdvNodeEventArgs(TreeNodeAdv node)
		{
			this.node = node;
		}
        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> which is associated with the action.
        /// </summary>
		public TreeNodeAdv Node
		{
			get{return this.node;}
		}
	}

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeExpand"/> event of the TreeViewAdv control.
	/// </summary>
	public delegate void TreeViewAdvCancelableNodeEventHandler(object sender,TreeViewAdvCancelableNodeEventArgs e);
    /// <summary>
    /// Custom EventArgs class which is used in <see cref="TreeViewAdv.BeforeExpand"/> event.
    /// </summary>
	public class TreeViewAdvCancelableNodeEventArgs : TreeViewAdvNodeEventArgs
	{
		bool cancel = false;
        /// <summary>
        /// Initializes the TreeViewAdvCancelableNodeEventArgs class
        /// </summary>
        /// <param name="node">The node which is associated with the event</param>
        /// <param name="cancel"></param>
		public TreeViewAdvCancelableNodeEventArgs(TreeNodeAdv node, bool cancel)
			: base(node)
		{
			this.cancel = cancel;
		}
        /// <summary>
        /// A boolean property which indicates whether the event is to be cancelled.
        /// </summary>
		public bool Cancel
		{
			get{return this.cancel;}
			set{this.cancel = value;}
		}
	}

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeCheck"/> event of the TreeViewAdv control.
	/// </summary>
	public delegate void TreeViewAdvBeforeCheckEventHandler(object sender,TreeNodeAdvBeforeCheckEventArgs e);
    /// <summary>
    /// Custom EventArgs class that is passed to BeforeCheck event of <see cref="TreeViewAdv.BeforeCheck"/> event.
    /// </summary>
	public class TreeNodeAdvBeforeCheckEventArgs : TreeViewAdvCancelableNodeEventArgs
	{
		private CheckState state;
        /// <summary>
        /// Initializes the TreeNodeAdvBeforeCheckEventArgs class 
        /// </summary>
        /// <param name="node">The node which is involved in the action</param>
        /// <param name="cancel">Parameter to indicate whether the action should be cancelled</param>
        /// <param name="newState">The new state of check box associated with node</param>
		public TreeNodeAdvBeforeCheckEventArgs(TreeNodeAdv node, bool cancel, CheckState newState)
			: base(node, cancel)
		{
			this.state = newState;
		}
        /// <summary>
        /// Gets the checkstate of the node
        /// </summary>
		public CheckState NewCheckState
		{
			get{return this.state;}
		}
	}

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeSelect"/> event.
	/// </summary>
	public delegate void TreeNodeAdvBeforeSelectEventHandler(object sender, TreeViewAdvCancelableSelectionEventArgs args);

	/// <summary>
	/// Specifies the action that raised a TreeViewAdv event.
	/// </summary>
	public enum TreeViewAdvAction
	{
		/// <summary>
		/// The event was caused by a keystroke.
		/// </summary>
		ByKeyboard,
		/// <summary>
		/// The event was caused by a mouse operation.
		/// </summary>
		ByMouse,
		/// <summary>
		/// The event was caused by the <see cref="TreeNodeAdv"/> collapsing.
		/// </summary>
		Collapse,
		/// <summary>
		/// The event was caused by the <see cref="TreeNodeAdv"/> expanding.
		/// </summary>
		Expand,
		/// <summary>
		/// The action that caused the event is unknown.
		/// </summary>
		Unknown
	}
}
