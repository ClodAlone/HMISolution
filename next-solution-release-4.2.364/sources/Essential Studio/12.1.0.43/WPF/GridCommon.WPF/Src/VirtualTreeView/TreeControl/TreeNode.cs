#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Windows.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// A tree node with child nodes and default cell appearance. Each node
    /// can have multiple <see cref="ChildNodes"/>. A node can have a <see cref="Data"/>
    /// object which contents are displayed by a <see cref="VirtualTreeView"/> in 
    /// multiple columns whereas each column identifies the property displayed in the 
    /// cell with the <see cref="TreeColumn.MappingName"/>.
    /// <para/>
    /// The <see cref="GetNestedHeight"/> method returns the height of all nested
    /// child nodes and <see cref="GetNestedRows"/> returns the number of visible
    /// nested nodes.<para/>
    /// You can check with the <see cref="IsPopulated"/> property whether child nodes
    /// have been populated. Only the visible nodes need to be populated. You can
    /// listen to the tree controls NodeExpanded event and populate child nodes
    /// on demand when this event is raised.
    /// </summary>
    public class TreeNode : ITreeTableCounterSource, ISupportInitialize
    {
        TreeModel treeModel;
        TreeNode parentNode;
        TreeNodeEntry treeNodeEntry;
        TreeNodes childNodes = null;
        TreeStyleInfo cellStyle = new TreeStyleInfo();
        Dictionary<int, TreeStyleInfo> cells = new Dictionary<int, TreeStyleInfo>();
        TreeLevel level;
        object tag;

        static TreeNodes notSetChildNodes = new TreeNodes(null);

        double height = double.NaN;
        int itemRows = -1;
        bool hide;
        object data;
        bool isExpanded;
        string shortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class.
        /// </summary>
        public TreeNode()
        {
            treeNodeEntry = new TreeNodeEntry(this);
            childNodes = notSetChildNodes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        public TreeNode(TreeNode parentNode)
            : this()
        {
            SetParent(parentNode);
        }

        /// <summary>
        /// Sets the parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        internal void SetParent(TreeNode parentNode)
        {
            this.parentNode = parentNode;
            if (parentNode != null)
                this.treeModel = parentNode.TreeModel;
        }

        /// <summary>
        /// Gets or sets the default style for cells of the node.
        /// </summary>
        /// <value>The cell style.</value>
        public TreeStyleInfo CellStyle
        {
            get { return cellStyle; }
            set { cellStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets the cell styles, one for each column.
        /// </summary>
        /// <value>The cells.</value>
        public Dictionary<int, TreeStyleInfo> Cells
        {
            get { return cells; }
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        {
            get
            {
                return GetDepth();
            }
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            if (ParentNode == null)
                return 0;

            return ParentNode.GetDepth() + 1;
        }


        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public TreeLevel Level
        {
            get
            {
                if (level == null)
                {
                    int depth = GetDepth();
                    if (treeModel.Levels.Count > depth)
                        level = treeModel.Levels[depth];
                }
                return level;
            }
            internal set { level = value; }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get
            {
                if (treeModel == null && parentNode != null)
                    treeModel = parentNode.TreeModel;
                return treeModel;
            }
            internal set { treeModel = value; }
        }

        internal TreeNodeEntry TreeNodeEntry
        {
            get { return treeNodeEntry; }
            set { treeNodeEntry = value; }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public TreeNode ParentNode
        {
            get { return parentNode; }
            internal set { parentNode = value; }
        }


        /// <summary>
        /// Gets a value indicating whether the child nodes are populated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the child nodes are populated; otherwise, <c>false</c>.
        /// </value>
        public bool IsPopulated
        {
            get
            {
                return !Object.ReferenceEquals(childNodes, notSetChildNodes);
            }
        }

        /// <summary>
        /// Populates this node.
        /// </summary>
        public void Populate()
        {
            if (IsPopulated)
                return;

            childNodes = new TreeNodes(this);
        }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        public void AddChildNode(TreeNode treeNode)
        {
            Populate();
            ChildNodes.Add(treeNode);
        }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>The child nodes.</value>
        public TreeNodes ChildNodes
        {
            get
            {
                return childNodes;
            }
            set
            {
                if (childNodes != value)
                {
                    childNodes = new TreeNodes(this);
                    if (value != null)
                    {
                        foreach (TreeNode row in value)
                            childNodes.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the node.
        /// </summary>
        /// <value>The height of the node.</value>
        public double ItemHeight
        {
            get
            {
                return height;
            }
            set
            {
                SetItemHeight(value, true);
            }
        }

        /// <summary>
        /// Sets the height of the node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="raiseChangedEvent">if set to <c>true</c> raises a VisibleNodes.ItemHeightChanged event.</param>
        public void SetItemHeight(double value, bool raiseChangedEvent)
        {
            if (height != value)
            {
                height = value;
                InvalidateCounterBottomUp();
                if (raiseChangedEvent && treeModel != null && (parentNode == null || !parentNode.IsInitializing))
                    treeModel.VisibleNodes.NotifyItemHeightChanged(this);
            }
        }

        /// <summary>
        /// Gets or sets the number of rows for this node. (don't confuse with nested rows)
        /// </summary>
        /// <value>The item rows.</value>
        public int ItemRows
        {
            get
            {
                return itemRows;
            }
            set
            {
                if (itemRows != value)
                {
                    itemRows = value;
                    InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Gets the height of the node (excluding nested nodes).
        /// </summary>
        /// <returns></returns>
        public double GetItemHeight()
        {
            if (!double.IsNaN(height))
                return height;

            TreeLevel level = Level;
            if (level != null)
                return level.Height;

            return treeModel.DefaultHeight;
        }

        /// <summary>
        /// Gets the total height of the nested visible nodes.
        /// </summary>
        /// <returns></returns>
        public double GetNestedHeight()
        {
            if (!IsPopulated)
                return 0;

            return ChildNodes.GetCounter().Height;
        }

        /// <summary>
        /// Gets the number of rows for this node. (don't confuse with nested rows)
        /// </summary>
        /// <returns></returns>
        public int GetItemRows()
        {
            if (itemRows != -1)
                return itemRows;
            return treeModel.DefaultItemRows;
        }

        /// <summary>
        /// Gets the number of visible nested rows.
        /// </summary>
        /// <returns></returns>
        public int GetNestedRows()
        {
            if (!IsPopulated)
                return 0;

            return ChildNodes.GetCounter().Rows;
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName
        {
            get { return shortName == null ? data.ToString() : shortName; }
            set { shortName = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this node is hidden.
        /// </summary>
        /// <value><c>true</c> if this node is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get { return hide; }
            set
            {
                SetHidden(value, true);
            }
        }

        /// <summary>
        /// Sets the hidden.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="raiseChangedEvent">if set to <c>true</c> raise VisibleNodes.ItemHiddenChanged event.</param>
        public void SetHidden(bool value, bool raiseChangedEvent)
        {
            if (hide != value)
            {
                hide = value;
                InvalidateCounterBottomUp();
                if (raiseChangedEvent && treeModel != null && (parentNode == null || !parentNode.IsInitializing))
                    treeModel.VisibleNodes.NotifyItemHiddenChanged(this);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this node is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                SetIsExpanded(value, true);
            }
        }

        /// <summary>
        /// Sets the is expanded.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
        public void SetIsExpanded(bool value, bool raiseEvent)
        {
            if (isExpanded != value)
            {
                isExpanded = value;

                InvalidateCounterBottomUp();
                if (raiseEvent && treeModel != null && (parentNode == null || !parentNode.IsInitializing))
                {
                    if (IsExpanded)
                        treeModel.RaiseNodeExpanded(new TreeNodeEventArgs(this));
                    else
                        treeModel.RaiseNodeCollapsed(new TreeNodeEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Expands this and all child nodes.
        /// </summary>
        public void ExpandAll()
        {
            ExpandAll(true);
        }

        /// <summary>
        /// Expands this and all child nodes.
        /// </summary>
        /// <param name="invalidateCounters">if set to <c>true</c> invalidate counters top down after expanding child nodes.</param>
        public void ExpandAll(bool invalidateCounters)
        {
            Populate();
            foreach (TreeNode node in ChildNodes)
                node.ExpandAll(false);

            if (invalidateCounters)
            {
                InvalidateCounterTopDown(true);
                IsExpanded = true;
            }
            else
                isExpanded = true;
        }

        /// <summary>
        /// Collapses all child nodes.
        /// </summary>
        public void CollapseAll()
        {
            CollapseAll(true);
        }

        /// <summary>
        /// Collapses all child nodes.
        /// </summary>
        /// <param name="invalidateCounters">if set to <c>true</c> invalidate counters top down after expanding child nodes.</param>
        public void CollapseAll(bool invalidateCounters)
        {
            if (!IsPopulated)
                return;

            foreach (TreeNode row in ChildNodes)
                row.CollapseAll(false);

            if (invalidateCounters)
            {
                InvalidateCounterTopDown(true);
                IsExpanded = false;
            }
            else
                isExpanded = false;
        }


        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        #region ITreeTableCounterSource Members

        ITreeTableCounter ITreeTableCounterSource.GetCounter()
        {
            return GetCounter();
        }

        internal TreeViewCounter GetItemCounter()
        {
            if (IsHidden)
                return TreeViewCounter.Empty;

            return new TreeViewCounter(GetItemRows(), GetItemHeight());
        }

        internal TreeViewCounter GetCounter()
        {
            if (IsHidden)
                return TreeViewCounter.Empty;

            TreeViewCounter tc = GetItemCounter();

            if (IsExpanded)
                tc = tc.Combine(ChildNodes.GetCounter(), TreeViewCounterKind.CountAll);

            return tc;
        }

        /// <summary>
        /// Marks all counters dirty in this object and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            if (IsPopulated)
                childNodes.RBTree.InvalidateCounterTopDown(true);
        }

        /// <summary>
        /// Marks all counters dirty in this object and parent nodes.
        /// </summary>
        public void InvalidateCounterBottomUp()
        {
            if (treeNodeEntry != null)
                treeNodeEntry.InvalidateCounterBottomUp(true);
        }

        #endregion

        /// <summary>
        /// Gets the nested tree node at the specified height position relative to this node.
        /// </summary>
        /// <param name="height">The height. Height 0 will return this node.</param>
        /// <returns></returns>
        public TreeNode GetNestedTreeNodeAtHeight(double height)
        {
            double itemHeight = GetItemHeight();
            if (height < itemHeight)
                return this;

            if (!IsPopulated || !IsExpanded)
                return null;

            height -= itemHeight;

            TreeViewCounter heightCounter = new TreeViewCounter(0, height);
            TreeNodeEntry treeNodeEntry = (TreeNodeEntry)ChildNodes.RBTree.GetEntryAtCounterPosition(heightCounter, TreeViewCounterKind.Height);
            if (treeNodeEntry != null)
            {
                TreeViewCounter positionCounter = (TreeViewCounter)treeNodeEntry.GetCounterPosition();

                // recursive call into this method walks into child nodes of this node.
                height -= positionCounter.Height;
                return treeNodeEntry.TreeNode.GetNestedTreeNodeAtHeight(height);
            }

            return null;
        }

        /// <summary>
        /// Gets the nested tree node at row index relative to this row.
        /// </summary>
        /// <param name="rowIndex">Index of the row. 0 will return this node.</param>
        /// <returns></returns>
        public TreeNode GetNestedTreeNodeAtRowIndex(int rowIndex)
        {
            int itemRows = GetItemRows();
            if (rowIndex < itemRows)
                return this;

            if (!IsPopulated || !IsExpanded)
                return null;

            rowIndex -= itemRows;

            TreeViewCounter rowIndexCounter = new TreeViewCounter(rowIndex, 0);
            TreeNodeEntry treeNodeEntry = (TreeNodeEntry)ChildNodes.RBTree.GetEntryAtCounterPosition(rowIndexCounter, TreeViewCounterKind.Lines);
            if (treeNodeEntry != null)
            {
                TreeViewCounter positionCounter = (TreeViewCounter)treeNodeEntry.GetCounterPosition();
                rowIndex -= positionCounter.Rows;

                // recursive call into this method walks into child nodes of this node.
                return treeNodeEntry.TreeNode.GetNestedTreeNodeAtRowIndex(rowIndex);
            }

            return null;
        }

        /// <summary>
        /// Gets the next visible node.
        /// </summary>
        /// <returns></returns>
        public TreeNode GetNextVisibleNode()
        {
            if (!IsExpanded || !IsPopulated || ChildNodes.GetCounter().Rows == 0)
                return null;

            TreeNode node = ChildNodes[0];
            if (node.GetCounter().Rows == 0)
            {
                TreeNodeEntry entry = (TreeNodeEntry)ChildNodes.RBTree.GetNextNotEmptyCounterEntry(node.TreeNodeEntry, TreeViewCounterKind.Lines);
                if (entry != null)
                    node = entry.TreeNode;
            }

            return node;
        }

        /// <summary>
        /// Determines whether the given nested node is visible.
        /// </summary>
        /// <param name="childNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if nested node is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChildVisible(TreeNode childNode)
        {
            return IsPopulated && IsExpanded && !childNode.IsHidden;
        }

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            Populate();
            ChildNodes.RBTree.BeginInit();
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            ChildNodes.RBTree.EndInit();
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
                return ChildNodes.RBTree.IsInitializing;
            }
        }

        /// <summary>
        /// Initializes the initialize nodes mode for faster adding of nodes.
        /// </summary>
        /// <returns></returns>
        public IDisposable InitializeNodesMode()
        {
            return new Initializer(this);
        }

        class Initializer : IDisposable
        {
            TreeNode treeNode;

            public Initializer(TreeNode treeNode)
            {
                this.treeNode = treeNode;
                treeNode.BeginInit();
            }

            #region IDisposable Members

            public void Dispose()
            {
                treeNode.EndInit();
            }

            #endregion
        }

     }

    internal class TreeNodeEntry : TreeTableWithCounterEntry
    {
        internal TreeNodeEntry(TreeNode treeNode)
        {
            Value = treeNode;
        }

        public TreeNode TreeNode
        {
            get
            {
                return (TreeNode)base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        /// <override/>
        public override object GetSortKey()
        {
            return null;
        }

        public TreeNodes TreeNodes
        {
            get
            {
                TreeTable tree = this.Tree;
                if (tree != null)
                    return tree.Tag as TreeNodes;
                return null;
            }
        }
    }

}
