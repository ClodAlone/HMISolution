#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.ComponentModel;
using System.Text;
using Syncfusion.Windows.Controls.Cells;

using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// Holds the tree node, column and cell row column index a <see cref="TreeStyleInfo"/>
    /// was created for and implements the inheritance mechanism for base styles.
    /// </summary>
    public class TreeStyleInfoIdentity : StyleInfoIdentityBase
    {
        TreeNode treeNode;
        TreeColumn column;
        TreeLevel level;
        RowColumnIndex cellPos;

        // Cache.
        IStyleInfo[] cachedBaseStyles = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="column">The column.</param>
        /// <param name="level">The level.</param>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public TreeStyleInfoIdentity(TreeNode treeNode, TreeColumn column, TreeLevel level, RowColumnIndex cellRowColumnIndex)
        {
            this.treeNode = treeNode;
            this.column = column;
            this.level = level;
            this.cellPos = cellRowColumnIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        public TreeStyleInfoIdentity(TreeColumn column)
        {
            this.column = column;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public TreeStyleInfoIdentity(TreeStyleInfoIdentity other)
        {
            this.treeNode = other.treeNode;
            this.column = other.column;
            this.level = other.level;
            this.cellPos = other.cellPos;
        }

        /// <summary>
        /// Releases all resources used by the component.
        /// </summary>
        public override void Dispose()
        {
            treeNode = null;
            column = null;
            cachedBaseStyles = null;
            base.Dispose(); // will call GC.SupressFinalize
        }

        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <value>The tree node.</value>
        public TreeNode TreeNode
        {
            get { return treeNode; }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public TreeColumn Column
        {
            get { return column; }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public TreeLevel Level
        {
            get { return level; }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get { return treeNode.TreeModel; }
        }

        /// <summary>
        /// The row index.
        /// </summary>
        public int RowIndex
        {

            get { return cellPos.RowIndex; }
        }

        /// <summary>
        /// The column index.
        /// </summary>
        public int ColumnIndex
        {

            get { return cellPos.ColumnIndex; }
        }

        /// <summary>
        /// The cell coordinates.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                return cellPos;
            }
        }

        /// <summary>
        /// Resets the base styles cache.
        /// </summary>
        public void ResetBaseStylesCache()
        {
            cachedBaseStyles = null;
        }

        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null)
            {
                ArrayList styleList = new ArrayList();
                TreeModel treeModel = treeNode.TreeModel;

                if (treeNode != null)
                {
                    if (treeNode.ParentNode == null)
                    {
                        // Current element 
                        AddStyle(treeNode.CellStyle, styleList, treeModel);

                        AddStyle(column.HeaderStyle, styleList, treeModel);
                        AddStyle(treeNode.TreeModel.ColumnHeaderStyle, styleList, treeModel);
                    }
                    else
                    {
                        // Current element and parent elements
                        AddStyle(treeNode.CellStyle, styleList, treeModel);

                        TreeNode parentNode = treeNode.ParentNode;
                        while (parentNode != null)
                        {
                            AddStyle(parentNode.CellStyle, styleList, treeModel);

                            parentNode = parentNode.ParentNode;
                        }

                        if (level != null)
                            AddStyle(level.CellStyle, styleList, treeModel);

                        if (column != null)
                            AddStyle(column.CellStyle, styleList, treeModel);

                        AddStyle(treeNode.TreeModel.CellStyle, styleList, treeModel);
                    }
                }
                else
                {
                    AddStyle(treeModel.ColumnHeaderStyle, styleList, treeModel);
                }
 
                cachedBaseStyles = new TreeStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
            }

            return cachedBaseStyles;
        }

        /// <summary>
        /// Adds the style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="styleList">The style list.</param>
        /// <param name="model">The model.</param>
        static void AddStyle(IStyleInfo style, ArrayList styleList, TreeModel model)
        {
            if (style != null && !style.IsEmpty)
            {
                styleList.Add(style);
                // TODO: Add support for base styles map (see Grid)
                //AddBaseStyle(style, styleList, model);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            //sb.Append(" {");
            //sb.Append("rowIndex = ");
            ////sb.Append(RowIndex.ToString());
            //sb.Append(", colIndex = ");
            ////sb.Append(ColumnIndex.ToString());
            //sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Overridden. If the style is not offline, saves its changes in the TreeNode.
        /// Note: At the moment TreeStyleInfoIdentity does not support comminting changes in TreeNode.
        /// </summary>
        /// <param name="style">A reference to the <see cref="TreeStyleInfo"/> object.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> that identifies the changed style property.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            // TODO: At the moment TreeStyleInfoIdentity does not support comminting changes in TreeNode.
            // Make style permanent in GridData.
            //if (!offLine && data != null)
            {
                //data.CommitStyle(CellRowColumnIndex, (TreeStyleInfo) style);
            }
            cachedBaseStyles = null;
        }


        internal void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            cellPos = cellRowColumnIndex;
        }
    }
}
