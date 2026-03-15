#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.Windows.Collections;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{

    /// <summary>
    /// A collection of <see cref="TreeNode"/> elements. The collection maintains
    /// both a list of nodes and a binary tree collection with height of nodes and
    /// number of rows. When adding, removing or changing nodes this collection ensures
    /// the binary tree collection is kept in sync with changes. 
    /// <para/>
    /// The <see cref="TreeNodes"/> collection is accessed through the <see cref="TreeNode.ChildNodes"/>
    /// property of a <see cref="TreeNode"/>. TreeNode gets the counters from this 
    /// collection when determining the total height of nested nodes and also the number
    /// of visible nested nodes.
    /// </summary>
    public class TreeNodes : IList<TreeNode>, ITreeTableCounterSource, IDisposable
    {
        private TreeTableWithCounter _inner;
        private TreeNode _parentNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodes"/> class.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        public TreeNodes(TreeNode parentNode)
        {
            _inner = new TreeTableWithCounter(new TreeViewCounter(0, 0), false);
            _inner.Tag = this;
            _parentNode = parentNode;
        }

        internal TreeTableWithCounter RBTree
        {
            get { return _inner; }
            set { _inner = value; }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public TreeNode ParentNode
        {
            get { return _parentNode; }
            internal set { _parentNode = value; }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get { return _parentNode.TreeModel; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _inner.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Controls.VirtualTreeView.TreeNode"/> at the specified index.
        /// </summary>
        /// <value></value>
        public TreeNode this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();

                TreeNodeEntry entry = (TreeNodeEntry)_inner[index];
                if (entry == null)
                    return null;
                return entry.TreeNode;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("TreeNode");

                _inner[index] = value.TreeNodeEntry;
                FixTreeAndModel(value);
            }
        }

        /// <summary>
        /// Determines whether the tree node is a direct child node (not grand child) of this collection. 
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if it is a child node; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TreeNode value)
        {
            if (value == null)
                return false;

            return _inner.Contains(value.TreeNodeEntry);
        }

        /// <summary>
        /// Returns the index of the node in this collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(TreeNode value)
        {
            if (value == null)
                return -1;
            return _inner.IndexOf(value.TreeNodeEntry);
        }

        /// <summary>
        /// Copies to another array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(TreeNode[] array, int index)
        {
            int n = 0;
            foreach (TreeNode treeNode in this)
            {
                array[index + n] = treeNode;
                n++;
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _inner.Clear();
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initializing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initializing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitializing
        {
            get
            {
                return RBTree.IsInitializing;
            }
        }

        /// <summary>
        /// Raises the line count changed.
        /// </summary>
        private void RaiseLineCountChanged()
        {
            if (!IsInitializing && _parentNode != null && _parentNode.IsExpanded && !_parentNode.IsHidden && _parentNode.TreeModel != null)
            {
                _parentNode.TreeModel.VisibleNodes.NotifyLineCountChanged();
            }
        }

        /// <summary>
        /// Removes the specified tree node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <returns></returns>
        public bool Remove(TreeNode treeNode)
        {
            if (treeNode.TreeNodeEntry == null)
                return false;

            ITreeTable tree = treeNode.TreeNodeEntry.Tree;

            // Remove it from the tree
            tree.Remove(treeNode.TreeNodeEntry);
            RaiseLineCountChanged();
            return true;
        }

        /// <summary>
        /// Inserts the node at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="treeNode">The tree node.</param>
        public void Insert(int index, TreeNode treeNode)
        {
            ITreeTable tree = treeNode.TreeNodeEntry.Tree;
            tree.Insert(index, treeNode.TreeNodeEntry);
            FixTreeAndModel(treeNode);
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Gets the next tree node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <returns></returns>
        public TreeNode GetNext(TreeNode treeNode)
        {
            if (treeNode.TreeNodeEntry == null)
                return null;

            TreeNodeEntry next = (TreeNodeEntry)treeNode.TreeNodeEntry.Tree.GetNextEntry(treeNode.TreeNodeEntry);
            if (next != null)
                return next.TreeNode;

            return null;
        }

        /// <summary>
        /// Gets the previous tree node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <returns></returns>
        public TreeNode GetPrevious(TreeNode treeNode)
        {
            if (treeNode.TreeNodeEntry == null)
                return null;

            TreeNodeEntry prev = (TreeNodeEntry)treeNode.TreeNodeEntry.Tree.GetPreviousEntry(treeNode.TreeNodeEntry);
            if (prev != null)
                return prev.TreeNode;

            return null;
        }

        /// <summary>
        /// Adds a tree node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        public void Add(TreeNode treeNode)
        {
            this._inner.Add(treeNode.TreeNodeEntry);
            FixTreeAndModel(treeNode);
            RaiseLineCountChanged();
        }

        private void FixTreeAndModel(TreeNode treeNode)
        {
            treeNode.TreeNodeEntry.Tree = this._inner;
            treeNode.TreeModel = TreeModel;
            if (treeNode.IsPopulated)
            {
                foreach (TreeNode node in treeNode.ChildNodes)
                    node.TreeModel = TreeModel;
            }
            treeNode.ParentNode = this.ParentNode;
        }

        //		public TreeNodes SyncRoot
        //		{
        //			get
        //			{
        //				return null;
        //			}
        //		}

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the collection is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Removes the child node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed size.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is synchronized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of nodes contained in the collection (not nested nodes).
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the collection.</returns>
        public int Count
        {
            get
            {
                return _inner.GetCount();
            }
        }

        #region IEnumerable<TreeNode> Members

        IEnumerator<TreeNode> IEnumerable<TreeNode>.GetEnumerator()
        {
            return new TreeNodesEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TreeNodesEnumerator(this);
        }

        #endregion

        #region ITreeTableCounterSource Members

        ITreeTableCounter ITreeTableCounterSource.GetCounter()
        {
            return GetCounter();
        }

        internal TreeViewCounter GetCounter()
        {
            return (TreeViewCounter)RBTree.GetCounterTotal();
        }

        /// <summary>
        /// Marks all counters dirty in this object and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            RBTree.InvalidateCounterTopDown(notifyCounterSource);
        }

        /// <summary>
        /// Marks all counters dirty in this object and parent nodes.
        /// </summary>
        public void InvalidateCounterBottomUp()
        {
            // Gets called from RootNodeEntry
            ParentNode.InvalidateCounterBottomUp();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="TreeNode"/> elements of a <see cref="TreeNodes"/>.
    /// </summary>
    public class TreeNodesEnumerator : IEnumerator<TreeNode>
    {
        TreeTableEnumerator _inner;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public TreeNodesEnumerator(TreeNodes collection)
        {
            _inner = new TreeTableEnumerator(collection.RBTree);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _inner.Reset();
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public TreeNode Current
        {
            get
            {
                return (TreeNode)_inner.Current.Value;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion
    }

}

