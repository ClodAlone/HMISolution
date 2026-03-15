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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents a collection of <see cref="TreeNodeAdv"/> objects.
	/// </summary>
	/// <remarks>
	/// The <see cref="Add"/>, <see cref="Remove"/> and <see cref="RemoveAt"/> methods
	/// enable you to add and remove individual tree nodes from the collection. You can
	/// also use the <see cref="AddRange"/> or <see cref="Clear"/> methods to
	/// add or remove all the tree nodes from the collection.
	/// </remarks>
	[Editor(typeof(TreeNodeAdvCollectionEditor),typeof(UITypeEditor))]
	public class TreeNodeAdvCollection : ArrayListExt
	{
		// Fields
        internal IComparer comparer = null;
		/// <summary>
		/// Creates a new instance of the collection.
		/// </summary>
		public TreeNodeAdvCollection()
		{
		}

		public event CollectionChangeEventHandler BeforeRemoving;

		public override void RemoveAt( int index )
		{
			OnBeforeRemoving( index );

			base.RemoveAt( index );
		}

		protected virtual void OnBeforeRemoving( int index )
		{
			TreeNodeAdv removingNode = this[ index ];
			
			CollectionChangeEventArgs e = 
				new CollectionChangeEventArgs( CollectionChangeAction.Remove, removingNode );

			RaiseBeforeRemoving( e );
		}

		protected void RaiseBeforeRemoving( CollectionChangeEventArgs e )
		{
			if( this.BeforeRemoving != null )
			{
				BeforeRemoving( this, e );
			}
		}

		/// </override>
		protected override void OnCollectionChanged(CollectionChangeEventArgs args)
		{
			base.OnCollectionChanged(args);
		}

		/// <summary>
		/// Gets / sets a reference to the TreeNodeAdv at the specified index location in the
		/// collection.
		/// In C#, this property is the indexer for the TreeNodeAdvCollection class.
		/// </summary>
		/// <param name="index">The location of the TreeNodeAdv in the collection.</param>
		/// <value>The reference to the TreeNodeAdv.</value>
		public new TreeNodeAdv this[int index]
		{
			get
			{
				return (TreeNodeAdv)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Adds a <see cref="TreeNodeAdv"/> to the collection.
		/// </summary>
		/// <param name="node">The <see cref="TreeNodeAdv"/> to add.</param>
		/// <returns>The position of the added node in the list.</returns>
		public virtual int Add(TreeNodeAdv node)
		{
			if(node.TreeView != null && node.TreeView.Root == node)
				throw new ArgumentException("Cannot add a root node to the TreeNodeAdvCollection.", "node");

			return base.Add(node);
		}
		/// <summary>
		/// Adds an array of TreeNodeAdv objects to the collection.
		/// </summary>
		/// <param name="items">An array of <see cref="TreeNodeAdv"/> objects to add to the collection.</param>
		public void AddRange(TreeNodeAdv[] items)
		{
			base.AddRange(items);
		}

		
        /// </override>
	
        public override void Sort()
		{
			Sort(SortOrder.Ascending);
		}

		/// <summary>
		/// Sorts the collection using the specified sort order.
		/// </summary>
		/// <param name="order">One of the <see cref="SortOrder"/> entries.</param>
		public virtual void Sort(SortOrder order)
		{
			if(order == SortOrder.None) return;
			if(this.comparer!=null)
				this.Sort(comparer);
			else
				base.Sort();
			if(order == SortOrder.Descending)
				this.Reverse();
		}
	}
	/// <summary>
	/// Specifies the list of <see cref="TreeNodeAdv"/>s currently selected in a <see cref="TreeViewAdv"/>.
	/// </summary>
	/// <remarks>
	/// <p>This collection contains references to all selected nodes in the <b>TreeViewAdv</b>.
	/// Adding a <see cref="TreeNodeAdv"/> to this collection will select the node. Removing a node from this collection will deselect the node.</p>
	/// <p>Whenever a node is selected/deselected, either by user action or in code,
	/// it is automatically added/removed from the <b>SelectedNodes</b> collection.</p>
	/// <p>This collection is read-only, which means you can add and remove elements but cannot change
	/// the existing elements.</p>
	/// <p>You can listen to new selections being added to this collection using the collection's
	/// <see cref="Syncfusion.Collections.ArrayListExt.CollectionChanged"/> event handler
	/// or listen to the <see cref="TreeViewAdv"/>'s <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.BeforeSelect"/> event.</p>
	/// </remarks>
	public class SelectedNodesCollection : ArrayListExt
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <remarks>This collection is always read-only. But you can still add/remove nodes.</remarks>
		public SelectedNodesCollection()
		{
			this.ForceReadOnly = true;
		}
		/// </override>
		protected override void AddHandlers(object item)
		{
			if(item != null && !(item is TreeNodeAdv))
			{
				throw new ArgumentException("Only elements of type TreeNodeAdv can be added to the SelectedNodesCollection.");
			}
			if(this.Count > 1)
			{
				TreeNodeAdv newNode = item as TreeNodeAdv;
				// Find an item that is not this item:
				foreach(TreeNodeAdv node in this)
				{
					if(node != newNode)
					{
						// If all-selection is allowed, don't bother with this check
						bool allSelect = node.TreeView == null || node.TreeView.SelectionMode == TreeSelectionMode.MultiSelectAll;
						if(node.Parent != newNode.Parent
							&& !allSelect)
						{
							this.Remove(newNode);
							throw new NotSupportedException("Cannot add a TreeNodeAdv into a SelectedNodesCollection that is not a sibling of the existing nodes in the collection.");
						}
					}
				}
			}
			base.AddHandlers(item);
		}
		/// <summary>
		/// Returns a node at the specified index.
		/// </summary>
		/// <remarks>Note that you cannot change the entry at a particular index.</remarks>
		public new TreeNodeAdv this[int index]
		{
			get{return (TreeNodeAdv)base[index];}
		}

		/// </override>
		public override object Clone()
		{
			SelectedNodesCollection nodes = new SelectedNodesCollection();
			foreach(object o in this)
				nodes.Add(o);
			return nodes;
		}

		internal void SetFixedSize(bool fixedSize)
		{
			this.ForceFixedSize = fixedSize;
		}
	}
}
