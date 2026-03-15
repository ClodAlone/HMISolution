#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Controls.Grid
{

    /// <summary>
    /// Encapsulates the properties of a row within the TreeGrid.
    /// </summary>
    public class GridTreeNode : IComparable, ISelectable
    {
        /// <summary>
        /// Encapsulates the properties of a row within the TreeGrid.
        /// </summary>
        /// <param name="level">The indent level of the row. Must be greater than or equal to 0.</param>
        /// <param name="item">The data item associated with the row.</param>
        /// <param name="expanded">Indicates whether the row is expanded or not.</param>
        /// <param name="parentNode">The parent GridTreeNode of this GridTreeNode.</param>
        public GridTreeNode(int level, object item, bool expanded, GridTreeNode parentNode)
        {
            this.level = level;
            this.item = item;
            this.expanded = expanded;
            this.parentNode = parentNode;

            childNodes = new List<GridTreeNode>();

            //set a unique identifier for this node.
            this.id = HashCode;
            hashCode++;
            //skip the value zero so it as it is used as a special identifier
            if (hashCode == 0)
                hashCode = 1;

        }

        int level;

        /// <summary>
        /// Gets or sets the indent level for this node.
        /// </summary>
        public int Level
        {
            get { return level; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Must be greater than or equal to zero.");
                level = value;
            }
        }

        private object item;

        /// <summary>
        /// Get or sets the data object associated with this node.
        /// </summary>
        public object Item
        {
            get { return item; }
            set { item = value; }
        }

        private object parentItem;

        /// <summary>
        /// Gets or sets the parent data object associated with this node.
        /// </summary>
        public object ParentItem
        {
            get { return parentItem; }
            set { parentItem = value; }
        }

        private List<GridTreeNode> childNodes;

        /// <summary>
        /// Gets or sets the children of this node, if any.
        /// </summary>
        public List<GridTreeNode> ChildNodes
        {
            get { return childNodes; }
            set { childNodes = value; }
        }

        private bool expanded = false;

        /// <summary>
        /// Gets or sets whether the node is expanded.
        /// </summary>
        public bool Expanded
        {
            get { return expanded; }
            set { expanded = value; }
        }

        private bool hasChildNodes;

        /// <summary>
        /// Gets or sets whether this node has child nodes. 
        /// </summary>
        public bool HasChildNodes
        {
            get { return hasChildNodes; }
            set { hasChildNodes = value; }
        }

        private double nodeHeight = double.NaN;

        /// <summary>
        /// Gets or sets the row height for this node.
        /// </summary>
        /// <remarks>If this value in double.NAN, then the default rowheight will be used.</remarks>
        public double NodeHeight
        {
            get { return nodeHeight; }
            set { nodeHeight = value; }
        }

        GridTreeNode parentNode = null;
        /// <summary>
        /// Gets the parent GridTreeNode of this GridTreeNode.
        /// </summary>
        public GridTreeNode ParentNode
        {
            get { return parentNode; }
        }

        bool isSelected = false;
        /// <summary>
        /// Gets whether node is selected. Do not use the setter on this property if you want the 
        /// grid to respond to your changing of the property.
        /// </summary>
        /// <remarks>The setter on this property is for internal use only. To change the value
        /// of IsSelected from code, you should use the grid.SelectedNodes.SetSelected method. 
        /// Using this method guarantees the grid will properly respond to the changing of the
        /// IsSelected property.
        /// </remarks>
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value; 
            }
        }

        private List<string> selectedColumns;

        /// <summary>
        /// Gets the selected columns for a GridTreeControl for use when EnableNodeSelection = false and EnableSelections = true.
        /// </summary>
        public List<string> SelectedColumns
        {
            get 
            {
                if (selectedColumns == null)
                {
                    selectedColumns = new List<string>();
                }
                return selectedColumns; 
            }
        }

        internal int id;
        private static int hashCode = 0;

        internal static int HashCode
        {
            get
            {
                if (hashCode == 0)
                    hashCode = int.MinValue;
                return GridTreeNode.hashCode;
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            GridTreeNode nObj = obj as GridTreeNode;
            if (nObj == null)
                return 1;
            return this.id.CompareTo(nObj.id);
        }

        #endregion
    }
}
