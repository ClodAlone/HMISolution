#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{

    /// <summary>
    /// Provides event data for the <see cref="VirtualTreeView.PrepareRenderCell"/> event
    /// of a <see cref="VirtualTreeView"/>.
    /// </summary>
    public sealed class TreePrepareRenderCellEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cellRowColumnIndex;
        TreeStyleInfo style;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreePrepareRenderCellEventArgs"/> class.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="style">The style.</param>
        public TreePrepareRenderCellEventArgs(RowColumnIndex cellRowColumnIndex, TreeStyleInfo style)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
            this.style = style;
        }

        /// <summary>
        /// Gets the cells row and column index.
        /// </summary>
        /// <value>The cell.</value>
        [TraceProperty(true)]
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                return cellRowColumnIndex;
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        [TraceProperty(true)]
        public TreeStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }

        /// <summary>
        /// Holds identity information such as row and column index for the <see cref="Style"/>.
        /// </summary>
        public TreeStyleInfoIdentity TreeNodeIdentity
        {
            get
            {
                return Style.Identity as TreeStyleInfoIdentity;
            }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.TreeModel;
                return null;
            }
        }

        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <value>The tree node.</value>
        public TreeNode TreeNode
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.TreeNode;
                return null;
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public TreeColumn Column
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.Column;
                return null;
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public TreeLevel Level
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.Level;
                return null;
            }
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get
            {
                return CellRowColumnIndex.RowIndex;
            }
        }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get
            {
                return CellRowColumnIndex.ColumnIndex;
            }
        }
    }

    /// <summary>
    /// An event handler for the <see cref="VirtualTreeView.PrepareRenderCell"/> event
    /// of a <see cref="VirtualTreeView"/>.
    /// </summary>
    public delegate void TreePrepareRenderCellEventHandler(object sender, TreePrepareRenderCellEventArgs e);

}
