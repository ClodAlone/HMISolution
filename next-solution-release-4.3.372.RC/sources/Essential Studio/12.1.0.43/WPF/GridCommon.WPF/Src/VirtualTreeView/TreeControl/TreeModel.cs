#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// The TreeModel maintains a hierarchical collection of expandable <see cref="TreeNode"/>
    /// items. A flattened representations of the hierarchy of nodes can be accessed through
    /// the <see cref="VisibleNodes"/> property where each visible node is mapped to a row index
    /// and vice versa. <see cref="TreeNodesFlattenedList"/> also implements <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/>
    /// and the <see cref="VirtualTreeView"/> assigns it to <see cref="Syncfusion.Windows.Controls.Scroll.ScrollAxisControl.RowHeightsProvider"/>
    /// in order to be able to pixel scroll through visible and expanded nodes. <para/>
    /// Each node can have a unique height but a default height for all nodes can be set with
    /// the <see cref="DefaultHeight"/> property. Each node also maintains its expansion state
    /// and when collapsing, expanding a grand parent node all expansion state of child elements are 
    /// remembered.<para/>
    /// The <see cref="NodeItemProperties"/> property can hold a collection of PropertyDescriptors
    /// of the underlying data source. <see cref="TreeColumn.MappingName"/> identifies properties
    /// in this collection so that cells can display values from the <see cref="TreeNode.Data"/>
    /// of a <see cref="TreeNode"/>.
    /// </summary>
    public class TreeModel
    {
        TreeColumns columns;
        TreeLevels levels;
        TreeStyleInfo columnHeaderStyle = new TreeStyleInfo();
        TreeStyleInfo cellStyle = new TreeStyleInfo();
        double defaultWidth = 40;
        double defaultHeight = 30;
        int defaultItemRows = 1;
        TreeNode rootNode;
        TreeNodesFlattenedList visibleNodes;
        Pen gridLinePen;
        int headerRowCount;
        int footerRowCount;
        PropertyDescriptorCollection nodeItemProperties = PropertyDescriptorCollection.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeModel"/> class.
        /// </summary>
        public TreeModel()
        {
            columns = new TreeColumns(this);
            levels = new TreeLevels(this);
            rootNode = new TreeNode();
            rootNode.TreeModel = this;
            visibleNodes = new TreeNodesFlattenedList(this);

            gridLinePen = new Pen(Brushes.DarkGray, 1);
            gridLinePen.Freeze();
            
            cellStyle.Background = SystemColors.WindowBrush;
            cellStyle.Borders.Bottom = gridLinePen;
            cellStyle.Borders.Right = gridLinePen;

            columnHeaderStyle.Background = SystemColors.ControlBrush;
            columnHeaderStyle.Borders.Bottom = gridLinePen;
            columnHeaderStyle.Borders.Right = gridLinePen;
            headerRowCount = 1;
            footerRowCount = 0;
        }

        /// <summary>
        /// Gets or sets the collection of PropertyDescriptors
        /// of the underlying data source. <see cref="TreeColumn.MappingName"/> identifies properties
        /// in this collection so that cells can display values from the <see cref="TreeNode.Data"/>
        /// of a <see cref="TreeNode"/>.
        /// </summary>
        /// <value>The node item properties.</value>
        public PropertyDescriptorCollection NodeItemProperties
        {
            get { return nodeItemProperties; }
            set { nodeItemProperties = value; }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <value>The root node.</value>
        public TreeNode RootNode
        {
            get { return rootNode; }
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public TreeColumns Columns
        {
            get { return columns; }
            set 
            {
                if (columns != value)
                {
                    columns = new TreeColumns(this);
                    if (value != null)
                    {
                        foreach (TreeColumn column in value)
                            columns.Add(column);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>The levels.</value>
        public TreeLevels Levels
        {
            get { return levels; }
            set 
            {
                if (levels != value)
                {
                    levels = new TreeLevels(this);
                    if (value != null)
                    {
                        foreach (TreeLevel level in value)
                            levels.Add(level);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the visible nodes which is a flattened representations of the hierarchy of nodes where 
        /// each visible node is mapped to a row index
        /// and vice versa. <see cref="TreeNodesFlattenedList"/> also implements <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/>
        /// and the <see cref="VirtualTreeView"/> assigns it to <see cref="Syncfusion.Windows.Controls.Scroll.ScrollAxisControl.RowHeightsProvider"/>
        /// in order to be able to pixel scroll through visible and expanded nodes. 
        /// </summary>
        /// <value>The visible nodes.</value>
        public TreeNodesFlattenedList VisibleNodes
        {
            get { return visibleNodes; }
        }
        
        /// <summary>
        /// Gets or sets the default style for cells in the model.
        /// </summary>
        /// <value>The cell style.</value>
        public TreeStyleInfo CellStyle
        {
            get { return cellStyle; }
            set { cellStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets or sets the default style for column headers in the model.
        /// </summary>
        /// <value>The column header style.</value>
        public TreeStyleInfo ColumnHeaderStyle
        {
            get { return columnHeaderStyle; }
            set { columnHeaderStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets or sets the header row count.
        /// </summary>
        /// <value>The header row count.</value>
        public int HeaderRowCount
        {
            get { return headerRowCount; }
            set 
            {
                if (headerRowCount != value)
                {
                    headerRowCount = value;
                    VisibleNodes.NotifyHeaderCountChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the footer row count.
        /// </summary>
        /// <value>The footer row count.</value>
        public int FooterRowCount
        {
            get { return footerRowCount; }
            set
            {
                if (footerRowCount != value)
                {
                    footerRowCount = value;
                    VisibleNodes.NotifyFooterCountChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the header column count.
        /// </summary>
        /// <value>The header column count.</value>
        public int HeaderColumnCount
        {
            get { return Columns.FrozenHeaderCount; }
            set
            {
                Columns.FrozenHeaderCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer column count.
        /// </summary>
        /// <value>The footer column count.</value>
        public int FooterColumnCount
        {
            get { return Columns.FrozenFooterCount; }
            set
            {
                Columns.FrozenFooterCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        /// <value>The height of the header.</value>
        public double HeaderHeight
        {
            get { return rootNode.ItemHeight; }
            set { rootNode.ItemHeight = value; }
        }

        /// <summary>
        /// Gets or sets the default width of columns.
        /// </summary>
        /// <value>The default width.</value>
        public double DefaultWidth
        {
            get { return defaultWidth; }
            set { defaultWidth = value; }
        }

        /// <summary>
        /// Gets or sets the default height of nodes.
        /// </summary>
        /// <value>The default height.</value>
        public double DefaultHeight
        {
            get { return defaultHeight; }
            set
            {
                if (defaultHeight != value)
                {
                    defaultHeight = value;
                    VisibleNodes.NotifyDefaultSizeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of rows for each node. (TODO: Not sure if this is fully implemented.)
        /// </summary>
        /// <value>The default item rows.</value>
        public int DefaultItemRows
        {
            // TODO: Not sure if this is fully implemented.
            get { return defaultItemRows; }
            set { defaultItemRows = value; }
        }

        /// <summary>
        /// Occurs when the visible line count changed for example if nodes were expanded or collapsed.
        /// </summary>
        public event EventHandler LineCountChanged;

        /// <summary>
        /// Raises the <see cref="LineCountChanged"/> event.
        /// </summary>
        public void RaiseLineCountChanged()
        {
            if (LineCountChanged != null)
                LineCountChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when node was expanded.
        /// </summary>
        public event TreeNodeEventHandler NodeExpanded;

        /// <summary>
        /// Occurs when a node was collapsed.
        /// </summary>
        public event TreeNodeEventHandler NodeCollapsed;


        /// <summary>
        /// Raises the <see cref="NodeExpanded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="TreeNodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeExpanded(TreeNodeEventArgs e)
        {
            if (NodeExpanded != null)
                NodeExpanded(this, e);

            VisibleNodes.NotifyLineCountChanged();
        }

        /// <summary>
        /// Raises the <see cref="NodeCollapsed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="TreeNodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeCollapsed(TreeNodeEventArgs e)
        {
            if (NodeCollapsed != null)
                NodeCollapsed(this, e);

            VisibleNodes.NotifyLineCountChanged();
        }



    }
}
