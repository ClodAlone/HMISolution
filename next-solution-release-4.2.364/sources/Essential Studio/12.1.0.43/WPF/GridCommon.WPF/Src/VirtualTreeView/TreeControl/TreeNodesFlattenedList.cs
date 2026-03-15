#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.Controls.Scroll;


namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// <see cref="TreeNodesFlattenedList"/> provides flattened representations of the hierarchy of nodes 
    /// in a <see cref="TreeModel"/>. The flattened list can be accessed through
    /// the <see cref="TreeModel.VisibleNodes"/> property and each visible node is mapped to a row index
    /// and vice versa. <see cref="TreeNodesFlattenedList"/> also implements <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/>
    /// and the <see cref="VirtualTreeView"/> assigns it to <see cref="Syncfusion.Windows.Controls.Scroll.ScrollAxisControl.RowHeightsProvider"/>
    /// in order to be able to pixel scroll through visible and expanded nodes. 
    /// <para/>
    /// Each node can have a unique height but a default height for all nodes can be set with
    /// the <see cref="TreeModel.DefaultHeight"/> property. Each node also maintains its expansion state
    /// and when collapsing, expanding a grand parent node all expansion state of child elements are 
    /// remembered.
    /// </summary>
    public class TreeNodesFlattenedList : ILineSizeHost
    {
        TreeModel treeModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodesFlattenedList"/> class.
        /// </summary>
        /// <param name="treeModel">The tree model.</param>
        public TreeNodesFlattenedList(TreeModel treeModel)
        {
            this.treeModel = treeModel;
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Controls.VirtualTreeView.TreeNode"/> at the specified index.
        /// </summary>
        /// <value></value>
        public TreeNode this[int index]
        {
            get
            {
                return treeModel.RootNode.GetNestedTreeNodeAtRowIndex(index);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the tree node at the specified cumulated height.
        /// </summary>
        /// <param name="height">The height. The root node is at height 0. </param>
        /// <returns></returns>
        public TreeNode GetTreeTreeNodeAtCumulatedHeight(double height)
        {
            return treeModel.RootNode.GetNestedTreeNodeAtHeight(height);
        }

        /// <summary>
        /// Determines whether the list contains the specified node.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the list contains the specified node; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TreeNode item)
        {
            return item != null && item.TreeModel == treeModel;
        }

        /// <summary>
        /// Gets the number of visible rows.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            { 
                TreeViewCounter total = treeModel.RootNode.GetCounter(); 
                return total.Rows; 
            }
        }

        /// <summary>
        /// Gets the total height of all visible tree nodes.
        /// </summary>
        /// <value>The total height.</value>
        public double TotalHeight
        {
            get
            {
                TreeViewCounter total = treeModel.RootNode.GetCounter();
                return total.Height;
            }
        }

        /// <summary>
        /// Returns the row index for a visible node.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <returns></returns>
        public int IndexOf(TreeNode item)
        {
            if (!Contains(item))
                return -1;

            TreeViewCounter position = TreeNodeHelper.GetCumulatedPosition(item);
            return position.Rows;
        }

        /// <summary>
        /// Gets the cumulated height of the node. The root node will return 0. 
        /// </summary>
        /// <param name="index">The row index of the node.</param>
        /// <returns></returns>
        public double GetCumulatedHeight(int index)
        {
            TreeNode node = this[index];
            return GetCumulatedHeight(node);
        }

        /// <summary>
        /// Gets the cumulated height of the node. The root node will return 0. 
        /// </summary>
        /// <param name="item">The nodex.</param>
        /// <returns></returns>
        public double GetCumulatedHeight(TreeNode item)
        {
            TreeViewCounter position = TreeNodeHelper.GetCumulatedPosition(item);
            return position.Height;
        }

        /// <summary>
        /// Gets the next visible node.
        /// </summary>
        /// <param name="index">The row index of the node.</param>
        /// <returns></returns>
        public TreeNode GetNextVisibleNode(int index)
        {
            return this[index + 1];
        }

        /// <summary>
        /// Gets the next visible node.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <returns></returns>
        public TreeNode GetNextVisibleNode(TreeNode item)
        {
            TreeNode next = TreeNodeHelper.GetNextTreeNodeStepIn(item, TreeViewCounterKind.Lines);
            return next;
        }

        #region ILineSizeHost Members

        double ILineSizeHost.GetDefaultLineSize()
        {
            return treeModel.DefaultHeight;
        }

        int ILineSizeHost.GetLineCount()
        {
            return Count;
        }

        double ILineSizeHost.GetSize(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;
            TreeNode node = this[index];
            return node.GetItemHeight();
        }

        int ILineSizeHost.GetHeaderLineCount()
        {
            return this.treeModel.HeaderRowCount;
        }

        int ILineSizeHost.GetFooterLineCount()
        {
            return this.treeModel.FooterRowCount;
        }

        bool ILineSizeHost.GetHidden(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;
            return false;
        }

        /// <summary>
        /// Occurs when a lines size was changed.
        /// </summary>
        public event RangeChangedEventHandler LineSizeChanged;

        /// <summary>
        /// Occurs when a lines hidden state changed.
        /// </summary>
        public event HiddenRangeChangedEventHandler LineHiddenChanged;

        /// <summary>
        /// Occurs when the default line size changed.
        /// </summary>
        public event DefaultLineSizeChangedEventHandler DefaultLineSizeChanged;

        /// <summary>
        /// Occurs when the line count was changed.
        /// </summary>
        public event EventHandler LineCountChanged;

        /// <summary>
        /// Occurs when the header line count was changed.
        /// </summary>
        public event EventHandler HeaderLineCountChanged;

        /// <summary>
        /// Occurs when the footer line count was changed.
        /// </summary>
        public event EventHandler FooterLineCountChanged;


#pragma warning disable 67 // Disable The event is never used warning.

        /// <summary>
        /// Occurs when lines were inserted.
        /// </summary>
        public event LinesInsertedEventHandler LinesInserted;

        /// <summary>
        /// Occurs when lines were removed.
        /// </summary>
        public event LinesRemovedEventHandler LinesRemoved;

#pragma warning restore 67

        void ILineSizeHost.InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
        }

        #endregion

        internal void NotifyItemHeightChanged(TreeNode treeNode)
        {
            if (LineSizeChanged != null)
            {
                int index = IndexOf(treeNode);
                LineSizeChanged(this, new RangeChangedEventArgs(index, index));
            }
        }

        internal void NotifyItemHiddenChanged(TreeNode treeNode)
        {
            if (LineHiddenChanged != null)
            {
                int index = IndexOf(treeNode);
                LineHiddenChanged(this, new HiddenRangeChangedEventArgs(index, index, false));
            }
        }

        internal void NotifyHeaderCountChanged()
        {
            if (HeaderLineCountChanged != null)
            {
                HeaderLineCountChanged(this, EventArgs.Empty);
            }
        }

        internal void NotifyFooterCountChanged()
        {
            if (FooterLineCountChanged != null)
            {
                FooterLineCountChanged(this, EventArgs.Empty);
            }
        }

        internal void NotifyDefaultSizeChanged()
        {
            if (DefaultLineSizeChanged != null)
            {
                DefaultLineSizeChanged(this, new DefaultLineSizeChangedEventArgs());
            }
        }

        internal void NotifyLineCountChanged()
        {
            if (LineCountChanged != null)
            {
                LineCountChanged(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
