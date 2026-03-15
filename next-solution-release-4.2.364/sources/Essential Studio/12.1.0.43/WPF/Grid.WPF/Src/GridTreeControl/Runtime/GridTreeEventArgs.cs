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
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
    #region Event Args

    #region GridTreeCreatingNodeEventArgs
    /// <summary>
    /// The class that defines the arguments for the GridTreeCreatingNode event.
    /// </summary>
    public class GridTreeCreatingNodeEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">The indent level for the new node.</param>
        /// <param name="item">The underlying item that defines this new node.</param>
        /// <param name="expanded">The initial expand state for this new node.</param>
        /// <param name="parentNode">The parent GridTreeNode for this new node.</param>
        public GridTreeCreatingNodeEventArgs(int level, object item, bool expanded, GridTreeNode parentNode)
            : base()
        {
            this.item = item;
            this.level = level;
            this.expanded = expanded;
            this.parentNode = parentNode;
        }

        private object item = null;

        /// <summary>
        /// Gets the item whose node is being created.
        /// </summary>
        public object Item
        {
            get { return item; }
        }

        int level = 0;

        /// <summary>
        /// Gets the node level of the new node.
        /// </summary>
        public int Level
        {
            get { return level; }
        }

        bool expanded = false;

        /// <summary>
        /// Gets the expand state of the new node.
        /// </summary>
        public bool Expanded
        {
            get { return expanded; }
        }

        GridTreeNode parentNode = null;

        /// <summary>
        /// Gets the parent GridTreeNode for the new node.
        /// </summary>
        public GridTreeNode ParentNode
        {
            get { return parentNode; }
        }

        GridTreeNode node = null;

        /// <summary>
        /// Gets or sets the new GridTreeNode. Set this value to your derived GridTreeNode object.
        /// </summary>
        public GridTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

    }

    #endregion

    #region GridTreeRequestNodeImageEventArgs
    /// <summary>
    /// The class that defines the arguments for the GridTreeRequestTreeItems event.
    /// </summary>
    public class GridTreeRequestNodeImageEventArgs :
#if !SILVERLIGHT
 RoutedEventArgs
#else
 SyncfusionRoutedEventArgs
#endif
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentItem">The parent object whose child list is being requested.</param>
        /// <param name="routedEvent">The rounted event.</param>
        /// <param name="source">The GridTreeControl.</param>
        public GridTreeRequestNodeImageEventArgs(object item,
#if !SILVERLIGHT
 RoutedEvent routedEvent,

#endif
 object source)
            :
#if !SILVERLIGHT
 base(routedEvent, source)
#else
 base(source)
#endif

        {
            this.Item = item;
        }

        private object item;

        /// <summary>
        /// Gets or sets the item whose node image is being requested.
        /// </summary>
        public object Item
        {
            get { return item; }
            set { item = value; }
        }

        private BitmapImage nodeImage;
        /// <summary>
        /// The image for the item.
        /// </summary>
        public BitmapImage NodeImage
        {
            get { return nodeImage; }
            set { nodeImage = value; }
        }

    }
    #endregion

    #region GridTreeGlyphDrawingEventArgs
    /// <summary>
    /// Event arguments for the GlyphDrawing event.
    /// </summary>
    public class GridTreeGlyphDrawingEventArgs : EventArgs
    {
        PathGeometry geometry;
        /// <summary>
        /// Gets the PathGeometry that you should populate to define the glyph.
        /// </summary>
        public PathGeometry Geometry
        {
            get { return geometry; }
        }
        Point startPoint;
        /// <summary>
        /// Gets the top-left point of the glyph.
        /// </summary>
        public Point StartPoint
        {
            get { return startPoint; }
        }
        bool isHot;
        /// <summary>
        /// Gets whether the mouse is over the glyph.
        /// </summary>
        public bool IsHot
        {
            get { return isHot; }
        }
        bool opened;
        /// <summary>
        /// Gets whether the cell is expanded.
        /// </summary>
        public bool Opened
        {
            get { return opened; }
        }
#if !SILVERLIGHT
        DrawingContext dc;
        /// <summary>
        /// Gets the DrawingContext being used to draw this cell.
        /// </summary>
        public DrawingContext DC
        {
            get { return dc; }
        }
#else
#endif

        /// <summary>
        /// Event arguments for event use in the custom drawing of the expand glyph.
        /// </summary>
        /// <param name="geometry">Add PathFigures to this PathGeometry to define the glyph. </param>
        /// <param name="startPoint">Top-left point of the glyph figure.</param>
        /// <param name="isHot">Indicates whether the mouse is over the glyph.</param>
        /// <param name="opened">Indicates whether the node is open.</param>
        public GridTreeGlyphDrawingEventArgs(PathGeometry geometry, Point startPoint, bool isHot, bool opened
#if !SILVERLIGHT
, DrawingContext dc
#endif
)
        {
            this.geometry = geometry;
            this.startPoint = startPoint;
            this.isHot = isHot;
            this.opened = opened;
#if !SILVERLIGHT
            this.dc = dc;
#endif
        }
    }

    #endregion

    #region GridTreeQueryUnknownPropertyEventArgs
    /// <summary>
    /// The class that defines the arguments for the GridTreeQueryUnknownColumn event.
    /// </summary>
    public class GridTreeQueryUnknownPropertyEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="node">The GridTreeNode whose property is being requested.</param>
        /// <param name="value">The value for the requested property.</param>
        /// <param name="propertyName">The name of the requested property.</param>
        public GridTreeQueryUnknownPropertyEventArgs(GridTreeNode node, string propertyName)
            : base()
        {
            this.node = node;
            this.propertyName = propertyName;
        }

        private GridTreeNode node = null;

        /// <summary>
        /// Gets the node whose property is being requested.
        /// </summary>
        public object Node
        {
            get { return node; }
        }

        object value = null;

        /// <summary>
        /// Gets or sets the requested value..
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        string propertyName = "";

        /// <summary>
        /// Gets or sets the name of the requested property.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }
    }
    #endregion

    #region GridTreeRequestChildListEventArgs
    /// <summary>
    /// Passes arguments for the RequestChildList event.
    /// </summary>
    public class GridTreeRequestChildListEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentItem">The parent tree object for this node.</param>
        public GridTreeRequestChildListEventArgs(object parentItem)
        {
            this.parentItem = parentItem;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentItem">The parent tree object for this node.</param>
        /// <param name="resetChildAndRepopulate">Indicates whether to force the child collection to be reloaded.</param>
        public GridTreeRequestChildListEventArgs(object parentItem, bool resetChildAndRepopulate)
        {
            this.resetChildAndRepopulate = resetChildAndRepopulate;
            this.parentItem = parentItem;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentNode">The parent GridTreeNode object for this node.</param>
        /// <param name="parentItem">The parent tree object for this node.</param>
        /// <param name="resetChildAndRepopulate">Indicates whether to force the child collection to be reloaded.</param>
        public GridTreeRequestChildListEventArgs(GridTreeNode parentNode, object parentItem, bool resetChildAndRepopulate)
        {
            this.resetChildAndRepopulate = resetChildAndRepopulate;
            this.parentItem = parentItem;
            this.parentNode = parentNode;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentNode">The parent GridTreeNode object for this node.</param>
        /// <param name="parentItem">The parent tree object for this node.</param>
        public GridTreeRequestChildListEventArgs(GridTreeNode parentNode, object parentItem)
        {
            this.parentItem = parentItem;
            this.parentNode = parentNode;
        }
        GridTreeNode parentNode;

        /// <summary>
        /// Gets or sets the parent GridTreeNode for this node.
        /// </summary>
        public GridTreeNode ParentNode
        {
            get { return parentNode; }
            set { parentNode = value; }
        }

        bool resetChildAndRepopulate = false;

        /// <summary>
        /// Gets or sets whether to force the child collection to be reloaded.
        /// </summary>
        public bool ResetChildAndRepopulate
        {
            get { return resetChildAndRepopulate; }
            set { resetChildAndRepopulate = value; }
        }

        private object parentItem;

        /// <summary>
        /// Gets or sets the parent tree object for this node.
        /// </summary>
        public object ParentItem
        {
            get { return parentItem; }
            set { parentItem = value; }
        }

        private IEnumerable childList = null;

        /// <summary>
        /// Gets or sets the child list for this node.
        /// </summary>
        public IEnumerable ChildList
        {
            get { return childList; }
            set { childList = value; }
        }
    }

    #endregion

    #region GridTreeRequestTreeItemsEventArgs
    /// <summary>
    /// The class that defines the arguments for the GridTreeRequestTreeItems event.
    /// </summary>
    public class GridTreeRequestTreeItemsEventArgs :
#if !SILVERLIGHT
 RoutedEventArgs
#else
 SyncfusionRoutedEventArgs
#endif

    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentItem">The parent object whose child list is being requested.</param>
        /// <param name="routedEvent">The rounted event.</param>
        /// <param name="source">The GridTreeControl.</param>
        public GridTreeRequestTreeItemsEventArgs(object parentItem,
#if !SILVERLIGHT
 RoutedEvent routedEvent,
#endif
 object source)
            :
#if !SILVERLIGHT
 base(routedEvent, source)
#else
 base(source)
#endif
        {
            this.parentItem = parentItem;
        }

        private object parentItem;

        /// <summary>
        /// Gets or sets the parest item whose children are being requested.
        /// </summary>
        public object ParentItem
        {
            get { return parentItem; }
            set { parentItem = value; }
        }

        private IEnumerable childList = null;

        /// <summary>
        /// Gets or sets the list of children that belong to the ParentItem.
        /// </summary>
        public IEnumerable ChildList
        {
            get { return childList; }
            set { childList = value; }
        }
    }
    #endregion

    #endregion

    #region GridTreeUnboundColumn

    public delegate void GridTreeQueryUnboundColumnEventHandler(object sender, GridTreeUnboundColumnEventArgs Args);

    public delegate void GridTreeQueryUnboundCellInfoEventHandler(object sender, GridTreeUnboundCellInfoEventArgs Args);

    public class GridTreeUnboundColumnEventArgs : SyncfusionHandledEventArgs
    {
        public GridTreeUnboundColumnEventArgs()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="GridTreeUnboundColumnEventArgs"/> class.
        /// </summary>
        /// <param name="Style">The style.</param>
        /// <param name="Node">The node.</param>
        /// <param name="Record">The record.</param>
        /// <param name="UnboundColumn">The unbound column.</param>
        public GridTreeUnboundColumnEventArgs(GridStyleInfo Style, GridTreeNode Node, object Record, GridTreeUnboundColumn UnboundColumn)
        {
            this.Style = Style;
            this.Node = Node;
            this.Record = Record;
            this.UnboundColumn = UnboundColumn;
        }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public GridStyleInfo Style { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public GridTreeNode Node { get; set; }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; set; }

        /// <summary>
        /// Gets or sets the unbound column.
        /// </summary>
        /// <value>The unbound column.</value>
        public GridTreeUnboundColumn UnboundColumn { get; set; }
    }

    public class GridTreeUnboundCellInfoEventArgs: SyncfusionHandledEventArgs
    {
        public GridTreeUnboundCellInfoEventArgs ()
	    {}
        /// <summary>
        /// Initializes a new instance of the <see cref="GridTreeUnboundCellInfoEventArgs"/> class.
        /// </summary>
        /// <param name="Cell">The cell.</param>
        /// <param name="Style">The style.</param>
        public GridTreeUnboundCellInfoEventArgs (RowColumnIndex Cell, GridStyleInfo Style, GridTreeUnboundColumn UnboundColumn)
        {
            this.Cell=Cell;
            this.Style=Style;
            this.Column= UnboundColumn;
        }

        /// <summary>
        /// Gets or sets the cell.
        /// </summary>
        /// <value>The cell.</value>
        public RowColumnIndex Cell { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public GridStyleInfo Style { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        public GridTreeUnboundColumn Column{ get; set; }
    }
    #endregion

    #region GridTreeQueryVisibleColumnInfo

    public delegate void GridTreeQueryVisibleColumnInfoEventHandler(object sender, GridTreeQueryVisibleColumnInfoEventArgs Args);

    public class GridTreeQueryVisibleColumnInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the visible column.
        /// </summary>
        /// <value>The visible column.</value>
        public GridTreeColumn VisibleColumn { get; set; }
    }
    #endregion

    #region Event Handler

    #region TreeCreatingNode event
    /// <summary>
    /// Event that is raised as a GridTreeNode is created, allowing you to use derived GridTreeNode objects.
    /// </summary>
    /// <param name="sender">The GridTreeControlImpl.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void GridTreeCreatingNodeHandler(object sender, GridTreeCreatingNodeEventArgs args);
    #endregion

    #region GridTreeRequestNodeImageEvent
    /// <summary>
    /// Event that is used to provide node images on demand.
    /// </summary>
    /// <param name="sender">The GridTreeControl.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void GridTreeRequestNodeImageHandler(object sender, GridTreeRequestNodeImageEventArgs args);
    #endregion

    #region GlyphDrawing event
    /// <summary>
    /// Event that is used to populate the childs node collection objects on demand as the parent node is expanded.
    /// </summary>
    /// <param name="sender">The GridTreeControl.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void GridTreeGlyphDrawingHandler(object sender, GridTreeGlyphDrawingEventArgs args);
    #endregion

    #region QueryUnknownProperty event
    /// <summary>
    /// Event that is raised when the value of an unknown column is requested.
    /// </summary>
    /// <param name="sender">The GridTreeControlImpl.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void GridTreeQueryUnknownPropertyHandler(object sender, GridTreeQueryUnknownPropertyEventArgs args);
    #endregion

    #region GridTreeRequestTreeItemsEvent
    /// <summary>
    /// Event that is used to populate the childs node collection objects on demand as the parent node is expanded.
    /// </summary>
    /// <param name="sender">The GridTreeControl.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void GridTreeRequestTreeItemsHandler(object sender, GridTreeRequestTreeItemsEventArgs args);
    #endregion

    #endregion

    #region Enums

    #region enum GridTreeExpandGlyph
    /// <summary>
    /// Enumerates the possible glyphs that appear in the expand cell.
    /// </summary>
    public enum GridTreeExpandGlyph
    {
        /// <summary>
        /// Displays a triangle.
        /// </summary>
        Triangle,
        /// <summary>
        /// Displayes a +- glyph.
        /// </summary>
        PlusMinus,
#if !SILVERLIGHT
        /// <summary>
        /// Displays a ++- glyph with treelines.
        /// </summary>
        PlusMinusLines,
#endif
        /// <summary>
        /// Raises an event where you can define the glyph.
        /// </summary>
        Custom,

        /// <summary>
        /// Displays themed based expander as in the GridTreeControl
        /// </summary>
        Themed

    }
    #endregion

    #region enum GridRowType

    /// <summary>
    /// Enumerations for the possible row types in the grid.
    /// </summary>
    public enum GridTreeRowType
    {
        /// <summary>
        /// Columns header row
        /// </summary>
        Header, //column header rows
        /// <summary>
        /// Row conatining a tree node
        /// </summary>
        Node,   //normal node row
        /// <summary>
        /// Unbound row under the tree contents
        /// </summary>
        Footer, //unbound row at the bottom
        /// <summary>
        /// Unbound row in the tree contents at the top / bottom
        /// </summary>
        UnboundRow, //unbound row at the top / bottom
        /// <summary>
        /// A tree node whose node.Data is some string that is displayed in a single covered cell.
        /// </summary>
        Caption //caption is a node with node.Data = "some string"
    }
    #endregion

    #region enum StartUpExpandState
    /// <summary>
    /// Enumerates possible expand states at startup.
    /// </summary>
    public enum GridTreeStartUpExpandState
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Root nodes expanded.
        /// </summary>
        RootNodesExpanded,
        /// <summary>
        /// All nodes expanded.
        /// </summary>
        AllNodesExpanded,
        /// <summary>
        /// All nodes collapsed.
        /// </summary>
        NoNodesExpanded
    }

    #endregion

    #region InvalidateAction
    /// <summary>
    /// Indicates the invalidate action taken when a value in a cell changes.
    /// </summary>
    public enum InvalidateAction
    {
        /// <summary>
        /// Only the changed cell is invalidated.
        /// </summary>
        Cell,
        /// <summary>
        /// The entire row holding the changed cell is invalidated.
        /// </summary>
        Row,
        /// <summary>
        /// The entire grid is invalidated when any cell in the gid is changed.
        /// </summary>
        Grid
    }
    #endregion

    #region Node Sizing Options
    public enum GridNodeAutosizingOption
    {
        //Allows Node size based on NodeCount
        BasedOnNodeCount = 0,
        //Allows Node size based on Level
        BasedOnLevel = 1
    }
    #endregion

    #region GridTreeSortingOptions
    public enum GridTreeSortingOptions
    {
        /// <summary>
        /// Sorting Enabled always
        /// </summary>
        Default = 0x00,
        /// <summary>
        /// Sorting Disabled when cell value is edited
        /// </summary>
        DisableSortingOnEdit = 0x01,
        /// <summary>
        /// Sorting Disabled when cell property value changed
        /// </summary>
        DisableSortingOnPropertyChange = 0x02
    }
    #endregion
    #endregion

}
