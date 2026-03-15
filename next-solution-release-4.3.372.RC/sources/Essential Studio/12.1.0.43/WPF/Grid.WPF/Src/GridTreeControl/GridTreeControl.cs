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
using System.ComponentModel;
#if !SILVERLIGHT
using System.Data;
#else
//using System.Reflection;
#endif
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Diagnostics;
using System.Diagnostics;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;
using System.Collections.Specialized;

using System.Collections.ObjectModel;

using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using Syncfusion.Windows.Data;


namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// A ContentControl derived class that aggregates the functionality of a GridTreeControlImpl object into a
    /// ContentControl. GridTreeControlImpl is a GridControl derived class that displays multicolumn tree data in 
    /// a tree-like grid. You populate the tree by handling a single event, RequestTreeItems in which you return 
    /// a list of objects that belong to a particular node. GridTreeControl wraps this tree grid into a ContentControl
    /// object that allows you access to its control Template.
    /// </summary>
    /// <remarks>
    /// While handling RequestTreeItems is the only thing required for data to appear in your tree, you can use additional
    /// property setting to customized the look and feel of the tree. You use the Columns collection to define content and order
    /// of the multiple columns appearing in the tree. For each column, you can specify Column.StyleInfo which is a GridStyleInfo
    /// object that controls the grid cell display properties (like BackColor, Format, Fonts, etc.) of a the column.
    /// 
    /// The TreeGridControl also supports sorting by clicking on a column header. There are also tree-wide properties like ReadOnly, 
    /// SupportRowSizing, MarkRowBrush, ColumnHeaderStyle, FreezeExpandColumn, LevelStyles, RowHeaderWidth,
    /// ShowColumnHeaders, ShowColumnHeaderBorders, AllowSorting and ShowRowHeaders that allow you to control the look and
    /// feel of the tree grid.
    /// 
    /// The TreeGridControlImpl is bound to a collection of GridNode objects found in TreeGridControl.Nodes. There is a one to one mapping between the
    /// rows in the underlying GridControl and GridNodes in TreeGridControlImpl.Nodes. Given a GridNode, you can get at the underlying data object
    /// passed in via the RequestTreeItems event by referencing GridNode.Item. The property, GridTreeControl.InternalGrid gives you access to
    /// the associated TreeGridControlImpl object.
    /// </remarks>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    [StyleTypedProperty(Property = "ScrollViewerStyle", StyleTargetType = typeof(ScrollViewer))]
    [TemplatePart(Name = GridTreeControl.TemplateGrid, Type = typeof(GridTreeControlImpl))]
    public class GridTreeControl : ContentControl , IDisposable
    {
        #region constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GridTreeControl()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(GridTreeControl);
#endif
            if (GridControl.IsSecurityGranted)
            {
                GridTreeControl.ValidateLicense();
            }
            Columns = new ObservableCollection<GridTreeColumn>(); //create a new empty list for this instance
            this.LevelStyles = new List<GridStyleInfo>();
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
#if !SILVERLIGHT
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridTreeControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif
        }


        static GridTreeControl()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTreeControl), new FrameworkPropertyMetadata(typeof(GridTreeControl)));
             FlowDirectionProperty.OverrideMetadata(typeof(GridTreeControl), new FrameworkPropertyMetadata(OnFlowDirectionChanged));
#endif
        }
        #endregion

        /// <summary>
        /// Name used for the GridTreeControlImpl object in the default template.
        /// </summary>
        public const string TemplateGrid = "PART_GridControl";

        private bool isGridLoaded = false;
        private bool isTrackSelectionOnCollectionChangeBeforeGridLoaded = false;
        private bool isVisualStyleChangedBeforeGridLoaded = false;
        private bool isShowRowHeadersChangedBeforeGridLoaded = false;
        private bool isAllowSortChangedBeforeGridLoaded = false;
        private bool isDefaultColumnWidthChangedBeforeGridLoaded = false;
        private bool isAllowDragChangedBeforeGridLoaded = false;
        private bool isPercentSizingBehaviorBeforeGridLoaded = false;
        private bool isNotifyPropertyChangedSetBeforeGridLoaded = false;
        private bool isFreezeExpandColumnChangedBeforeGridLoaded = false;
        private bool isReadOnlyChangedBeforeGridLoaded = false;
        private bool isSupportRowSizingChangedBeforeGridLoaded = false;
        private bool isEnableNodeSelectionChangedBeforeGridLoaded = false;
        private bool isEnableSelectionsChangedBeforeGridLoaded = false;
        private bool isEnableHotRowMarkerChangedBeforeGridLoaded = false;
        private bool isShowExpandColumnBordersChangedBeforeGridLoaded = false;
        private bool isRowHeaderWidthChangedBeforeGridLoaded = false;
        private bool isShowColumnHeadersChangedBeforeGridLoaded = false;
        private bool isAutoPopulateColumnsChangedBeforeGridLoaded = false;
        private bool isAutoGenerateColumnsInfoChangedBeforeGridLoaded = false;
        private bool isColumnsChangedBeforeGridLoaded = false;
        private bool isAllowAutoSizingNodeColumnChangedBeforeGridLoaded = false;
        private bool isColumnHeaderStyleChangedBeforeGridLoaded = false;
        private bool isLevelStylesChangedBeforeGridLoaded = false;
        private bool isSupportsVisualStylesBeforeGridLoaded = false;
        private bool isHideEmptyChildGlyphsChangedBeforeGridLoaded = false;
        private bool isSupportNodeImagesChangedBeforeGridLoaded = false;
        

#if !SILVERLIGHT
        /// <summary>
        /// Called when [flow direction changed].
        /// </summary>
        /// <param name="dpo">The dpo.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>        
        private static void OnFlowDirectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridTreeControl;
            grid.Model.TableStyle.FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), args.NewValue.ToString(), true);
//If FlowDirection set at the run time then grid doesnt get refreshed. So we need to refresh here if FlowDirection set after grid loaded
            if(grid.IsLoaded)
                grid.InternalGrid.InvalidateCells();
        }
#endif

        #region events
        /// <summary>
        /// Occurs after the Nodes are populated in the Grid.
        /// </summary>
        public event EventHandler NodesPopulated;
        internal void OnNodesPopulated()
        {
            var handler = this.NodesPopulated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// Occurs when the Model is loaded and the Template is applied. This is useful to listen to Model events when
        /// the control is initialized. 
        /// <para></para>
        /// <code lang="C#">            
        ///             this.treeControl.ModelLoaded += (sender, args) =&gt;
        ///             {
        ///                 this.treeControl.Model.QueryCellInfo += new
        /// Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventHandler(Model_QueryCellInfo);
        ///             };
        ///             </code>
        /// </summary>
        public event EventHandler ModelLoaded;
#if !SILVERLIGHT
        public static readonly RoutedEvent RequestTreeItemsEvent = EventManager.RegisterRoutedEvent(
           "RequestTreeItems",
           RoutingStrategy.Direct,
           typeof(GridTreeRequestTreeItemsHandler),
           typeof(GridTreeControl));
#else
        private event GridTreeRequestTreeItemsHandler RequestTreeItemsEvent;
#endif

        #region QueryVisibleColumnInfo

        /// <summary>
        /// Occurs when [query visible column info].
        /// </summary>
        public event GridTreeQueryVisibleColumnInfoEventHandler QueryVisibleColumnInfo;

        /// <summary>
        /// Raises the <see cref="E:QueryVisibleColumnInfo"/> event.
        /// </summary>
        /// <param name="Args">The <see cref="Syncfusion.Windows.Controls.Grid.GridTreeQueryVisibleColumnInfoEventArgs"/> instance containing the event data.</param>
        protected virtual void OnQueryVisibleColumnInfo(GridTreeQueryVisibleColumnInfoEventArgs Args)
        {
            if (QueryVisibleColumnInfo != null)
                QueryVisibleColumnInfo(this, Args);
        }

        /// <summary>
        /// Raises the query visible column info.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridTreeQueryVisibleColumnInfoEventArgs"/> instance containing the event data.</param>
        public void RaiseQueryVisibleColumnInfo(GridTreeQueryVisibleColumnInfoEventArgs e)
        {
            OnQueryVisibleColumnInfo(e);
        }

        #endregion

        /// <summary>
        /// This event is used to request an IEnumerable object that holds the child item objects for a particular parent item.
        /// </summary>
        /// <remarks>
        /// In order to see data displayed in the TreeGrid, you must handle this event. The events args pass in a parent object, 
        /// and your event handler needs to provide an IEnumerable object that contains the childs objects for this pareent object.
        /// If the parent object is null, you should provide the collection of root objects for the tree.
        /// </remarks>
        public event GridTreeRequestTreeItemsHandler RequestTreeItems
        {
            add
            {
#if !SILVERLIGHT
                AddHandler(GridTreeControl.RequestTreeItemsEvent, value, false);
#else
                this.RequestTreeItemsEvent += value;
#endif
            }
            remove
            {
#if !SILVERLIGHT
                RemoveHandler(GridTreeControl.RequestTreeItemsEvent, value);
#else
                this.RequestTreeItemsEvent -= value;
#endif
            }
        }

        public void RaiseRequestTreeItems(GridTreeRequestChildListEventArgs e)
        {
            GridTreeRequestTreeItemsEventArgs e1 = new GridTreeRequestTreeItemsEventArgs(e.ParentItem,
#if !SILVERLIGHT
 GridTreeControl.RequestTreeItemsEvent,
#endif
 this);
#if !SILVERLIGHT
            RaiseEvent(e1);
#else
            OnRaiseRequestTreeItems(e1);
#endif
            e.ChildList = e1.ChildList;


        }
#if SILVERLIGHT
        private void OnRaiseRequestTreeItems(GridTreeRequestTreeItemsEventArgs e1)
        {
            if (this.RequestTreeItemsEvent != null)
            {
                this.RequestTreeItemsEvent(this, e1);
            }
        }
#endif

#if !SILVERLIGHT

        public static readonly RoutedEvent RequestNodeImageEvent = EventManager.RegisterRoutedEvent(
            "RequestNodeImage",
            RoutingStrategy.Direct,
            typeof(GridTreeRequestNodeImageHandler),
            typeof(GridTreeControl));
#else
        private event GridTreeRequestNodeImageHandler RequestNodeImageEvent;
#endif

        /// <summary>
        /// This event is used to request an image for the specified node item. This event is only raised if <see cref="SupportNodeImages"/> is true.
        /// </summary>
        public event GridTreeRequestNodeImageHandler RequestNodeImage
        {
            add
            {
#if !SILVERLIGHT
                AddHandler(GridTreeControl.RequestNodeImageEvent, value, false);
#else
                this.RequestNodeImageEvent += value;
#endif
            }
            remove
            {
#if !SILVERLIGHT
                RemoveHandler(GridTreeControl.RequestNodeImageEvent, value);
#else
                this.RequestNodeImageEvent -= value;
#endif
            }
        }

        public void RaiseRequestNodeImage(GridTreeRequestNodeImageEventArgs e)
        {

            GridTreeRequestNodeImageEventArgs e1 = new GridTreeRequestNodeImageEventArgs(e.Item,
#if !SILVERLIGHT
 GridTreeControl.RequestNodeImageEvent,
#endif
 this);
#if !SILVERLIGHT
            RaiseEvent(e1);
#else
            OnRaiseRequestNodeImage(e1);
#endif
            e.NodeImage = e1.NodeImage;
        }

#if SILVERLIGHT
        private void OnRaiseRequestNodeImage(GridTreeRequestNodeImageEventArgs e1)
        {
            if (this.RequestNodeImageEvent != null)
            {
                this.RequestNodeImageEvent(this, e1);
            }
        }
#endif

        /// <summary>
        /// Event which is raised as GridTreeNodes are created so derived tree nodes can be used.
        /// </summary>
        public event GridTreeCreatingNodeHandler CreatingTreeNode;

        /// <summary>
        /// Raises the CreatingNode event.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnCreatingTreeNode(GridTreeCreatingNodeEventArgs e)
        {
            if (CreatingTreeNode != null)
            {
                CreatingTreeNode(this, e);
            }
        }

        /// <summary>
        /// Raises the CreatingTreeNode event.
        /// </summary>
        /// <param name="e">The event argument.</param>
        public void RaiseCreatingTreeNode(GridTreeCreatingNodeEventArgs e)
        {
            OnCreatingTreeNode(e);
        }
        #endregion

        #region dependency properties

        /// <summary>
        /// DependencyProperty for <see cref="GridTreeControl.SelectedNode"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register(
            "SelectedNode",
            typeof(object),
            typeof(GridTreeControl),
#if !SILVERLIGHT
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedNodeChanged));
#else
 new PropertyMetadata(null, OnSelectedNodeChanged));
#endif

        /// <summary>
        /// Gets or sets the selected Node.
        /// </summary>
        /// <value>The selected Node.</value>
        public object SelectedNode
        {
            get
            {
                return (object)this.GetValue(GridTreeControl.SelectedNodeProperty);
            }

            set
            {
                if (this.SelectedNode != value)
                {
                    this.SetValue(GridTreeControl.SelectedNodeProperty, value);
                }
            }
        }

        private static void OnSelectedNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridTree = d as GridTreeControl;
            gridTree.OnSelectedNodeChanged(args.NewValue);           
        }

        private void OnSelectedNodeChanged(object record)
        {
            if (record!=null&&this.SelectedNodes != null&&! this.SelectedNodes.Contains(record as GridTreeNode))
            {
                this.SelectedNodes.Add(record as GridTreeNode);
            }
        }  

        /// <summary>
        /// Gets or sets whether no action is taken by the GridTreeControl on the occurrence 
        /// of an IBindingList.ListChangedType.Reset event.
        /// <remarks>
        /// If you are binding to a DataTable/DataView, when you call DataTable.AcceptChanges(),
        /// the corresponding DataView raises a IBindingList.ListChanged Reset event. When the GridTreeControl
        /// responds to this event, its default action is to just reload the entire tree which will cause
        /// the expand state of the existing nodes to be lost. So, if you are calling DataTable.AcceptChanges,
        /// you can set IgnoreResetOnListChanged = true to avoid losing the existing expand states.
        /// </remarks>
        /// </summary>
        public static readonly DependencyProperty IgnoreResetOnListChangedProperty = DependencyProperty.Register(
         "IgnoreResetOnListChanged",
         typeof(bool),
         typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, null)
#else
 new PropertyMetadata(false, null)
#endif
);       
       
        public bool IgnoreResetOnListChanged
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.IgnoreResetOnListChangedProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.IgnoreResetOnListChangedProperty, value);
            }
        }

        private bool isFooterRowsChangedBeforeGridLoaded = false;
        /// <summary>
        /// Gets or sets the FooterRows count.
        /// </summary>
        public int FooterRows
        {
            get { return (int)GetValue(FooterRowsProperty); }
            set { SetValue(FooterRowsProperty, value); }
        }

        public static readonly DependencyProperty FooterRowsProperty =
            DependencyProperty.Register("FooterRows", typeof(int), typeof(GridTreeControl), new PropertyMetadata(0, OnFooterRowsPropertyChanged));

        private static void OnFooterRowsPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.FooterRows = (int)args.NewValue;
            }
            else
            {
                grid.isFooterRowsChangedBeforeGridLoaded = true;
            }
        }

        private bool isFrozenRowsChangedBeforeGridLoaded = false;
        /// <summary>
        /// Gets or sets the FrozenRows count.
        /// </summary>
        public int FrozenRows
        {
            get { return (int)GetValue(FrozenRowsProperty); }
            set { SetValue(FrozenRowsProperty, value); }
        }

        public static readonly DependencyProperty FrozenRowsProperty =
            DependencyProperty.Register("FrozenRows", typeof(int), typeof(GridTreeControl), new PropertyMetadata(0, OnFrozenRowsPropertyChanged));

        private static void OnFrozenRowsPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.FrozenRows = (int)args.NewValue;
            }
            else
            {
                grid.isFrozenRowsChangedBeforeGridLoaded = true;
            }
        }

        bool isUnboundRowsCountChangedBeforeGridLoaded = false;
        /// <summary>
        /// Gets or sets the Bnbound Row Count for the Model. This would add additional rows
        /// to the Top of the Grid after the Column headers or Bottom of the Grid based on UnboundRowPosition
        /// </summary>
        public int UnboundRowsCount
        {
            get { return (int)GetValue(UnboundRowsCountProperty); }
            set { SetValue(UnboundRowsCountProperty, value); }
        }

        public static readonly DependencyProperty UnboundRowsCountProperty =
            DependencyProperty.Register("UnboundRowsCount", typeof(int), typeof(GridTreeControl), new PropertyMetadata(0,OnUnboundRowsCountChanged));

        private static void OnUnboundRowsCountChanged(DependencyObject d,DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.UnboundRowsCount = (int)args.NewValue;
            }
            else
            {
                grid.isUnboundRowsCountChangedBeforeGridLoaded = true;
            }
        }

        bool isUnboundRowPositionChangedBeforeGridLoaded = false;
        /// <summary>
        /// Gets or sets the UnboundRowPosition
        /// </summary>
        public Position UnboundRowPosition
        {
            get { return (Position)GetValue(UnboundRowPositionProperty); }
            set { SetValue(UnboundRowPositionProperty, value); }
        }

        public static readonly DependencyProperty UnboundRowPositionProperty =
            DependencyProperty.Register("UnboundRowPosition", typeof(Position), typeof(GridTreeControl), new PropertyMetadata(Position.Bottom,OnUnboundRowPositionChanged));

        private static void OnUnboundRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.UnboundRowPosition = (Position)args.NewValue;
            }
            else
            {
                grid.isUnboundRowPositionChangedBeforeGridLoaded = true;
            }
        }

        #region ScrollViewerStyle

        [Obsolete("Not sure if needed")]
        public Style ScrollViewerStyle
        {
            get { return (Style)GetValue(ScrollViewerStyleProperty); }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollViewerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollViewerStyleProperty =
            DependencyProperty.Register("ScrollViewerStyle", typeof(Style), typeof(GridTreeControl), new PropertyMetadata(null, OnScrollViewerStylePropertyChanged));

        private bool isScrollViewerStyleChangedBeforeGridLoaded = false;

        private static void OnScrollViewerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded && grid.PartScrollViewer != null && args.NewValue is Style)
            {
                grid.PartScrollViewer.Style = (Style)args.NewValue;
            }
            else
            {
                grid.isScrollViewerStyleChangedBeforeGridLoaded = true;
            }
        }

        #endregion

        #region AllowDragColumns
        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.AllowDragColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AllowDragColumnsProperty = DependencyProperty.Register(
            "AllowDragColumns",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnAllowDragColumnsChanged)
#else
 new PropertyMetadata(false, OnAllowDragColumnsChanged)
#endif
);

        private static void OnAllowDragColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.AllowDragColumns = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowDragChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow drag columns].
        /// </summary>
        /// <value><c>true</c> if [allow drag columns]; otherwise, <c>false</c>.</value>
        public bool AllowDragColumns
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.AllowDragColumnsProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.AllowDragColumnsProperty, value);
            }
        }
        #endregion

        #region AllowSort

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.AllowSort" />.
        /// </summary>
        public static readonly DependencyProperty AllowSortProperty = DependencyProperty.Register(
            "AllowSort",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnAllowSortPropertyChanged)
#else
 new PropertyMetadata(true, OnAllowSortPropertyChanged)
#endif
);

        private static void OnAllowSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControl grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.AllowSort = (bool)args.NewValue;

            }
            else
            {
                grid.isAllowSortChangedBeforeGridLoaded = true;
            }

            // grid.Model.ColumnSelection = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sort].
        /// </summary>
        /// <value><c>true</c> if [allow sort]; otherwise, <c>false</c>.</value>
        public bool AllowSort
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.AllowSortProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.AllowSortProperty, value);
            }
        }
        #endregion

        #region DefaultColumnWidth
        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.DefaultColumnWidth"/>.
        /// </summary>
        public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register(
            "DefaultColumnWidth",
            typeof(double),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(90d, OnDefaultColumnWidthChanged)
#else
 new PropertyMetadata(90d, OnDefaultColumnWidthChanged)
#endif
);

        private static void OnDefaultColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControl grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.Model.ColumnWidths.DefaultLineSize = (double)args.NewValue;
            }
            else
            {
                grid.isDefaultColumnWidthChangedBeforeGridLoaded = true;
            }

        }

        /// <summary>
        /// Gets or sets the default width of the column.
        /// </summary>
        /// <value>The default width of the column.</value>
        public double DefaultColumnWidth
        {
            get
            {
                return (double)this.GetValue(GridTreeControl.DefaultColumnWidthProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.DefaultColumnWidthProperty, value);
            }
        }

        #endregion

        #region ShowRowHeader

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.ShowRowHeader"/>.
        /// </summary>
        public static readonly DependencyProperty ShowRowHeaderProperty = DependencyProperty.Register(
            "ShowRowHeader",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnShowRowHeaderChanged)
#else
 new PropertyMetadata(false, OnShowRowHeaderChanged)
#endif
);

        private static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControl grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ShowRowHeader = (bool)args.NewValue;
            }
            else
            {
                grid.isShowRowHeadersChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show row header].
        /// </summary>
        /// <value><c>true</c> if [show row header]; otherwise, <c>false</c>.</value>
        public bool ShowRowHeader
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.ShowRowHeaderProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.ShowRowHeaderProperty, value);
            }
        }
        #endregion

        #region VisualStyle

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.VisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.Register(
            "VisualStyle",
            typeof(VisualStyle),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(VisualStyle.Default, OnVisualStyleChanged)
#else
 new PropertyMetadata(VisualStyle.Default, OnVisualStyleChanged)
#endif
);

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControl grid = d as GridTreeControl;
#if !SILVERLIGHT
            var visualStyle = SkinStorage.GetVisualStyle(d);
            var value = ((VisualStyle)args.NewValue).ToString();
            if (visualStyle != value)
            {
                // we just ensure that the DO has the same visual style with SkinStorage              
                if (value == VisualStyle.Default.ToString() && visualStyle == value)
                {
                    SkinStorage.SetVisualStyle(d, value);
                }
            }
#endif

            if (grid.isGridLoaded)
            {
                grid.internalGrid.CustomVisualStyle = grid.CustomVisualStyle;
                grid.InternalGrid.VisualStyle = (VisualStyle)args.NewValue;
            }
            else
            {
                grid.isVisualStyleChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)this.GetValue(GridTreeControl.VisualStyleProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.VisualStyleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.VisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty CustomVisualStyleProperty = DependencyProperty.Register(
            "CustomVisualStyle",
            typeof(IGridTreeVisualStyle),
            typeof(GridTreeControl), null);

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public IGridTreeVisualStyle CustomVisualStyle
        {
            get
            {
                return (IGridTreeVisualStyle)this.GetValue(GridTreeControl.CustomVisualStyleProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.CustomVisualStyleProperty, value);
            }
        }

        #endregion

        #region NotifyPropertyChanges
        /// <summary>
        /// DependencyProperty for <see cref="GridTreeControl.NotifyPropertyChanges"/>.
        /// </summary>
        public static readonly DependencyProperty NotifyPropertyChangesProperty = DependencyProperty.Register("NotifyPropertyChanges",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnNotifyPropertyChanged)
#else
 new PropertyMetadata(false, OnNotifyPropertyChanged)
#endif
);

        private static void OnNotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.NotifyPropertyChanges = (bool)args.NewValue;
            }
            else
            {
                grid.isNotifyPropertyChangedSetBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether NotifyPropertyChanges is true / false. Set this to true, GridTreeControl will listen to
        /// property changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if [notify property changes]; otherwise, <c>false</c>.
        /// </value>
        public bool NotifyPropertyChanges
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.NotifyPropertyChangesProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.NotifyPropertyChangesProperty, value);
            }
        }
        #endregion

        #region PercentSizingBehavior
        /// <exclude/>
        public static readonly DependencyProperty PercentSizingBehaviorProperty = DependencyProperty.Register(
            "PercentSizingBehavior",
            typeof(GridPercentColumnSizingBehavior),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(GridPercentColumnSizingBehavior.None, OnPercentSizingBehaviorChanged)
#else
 new PropertyMetadata(GridPercentColumnSizingBehavior.None, OnPercentSizingBehaviorChanged)
#endif
);

        static void OnPercentSizingBehaviorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid != null)
            {
                if (grid.isGridLoaded)
                {
                    grid.InternalGrid.ColumnWidthSizer.SizingBehavior = (GridPercentColumnSizingBehavior)e.NewValue;
                }
                else
                {
                    grid.isPercentSizingBehaviorBeforeGridLoaded = true;
                }

            }

        }

        /// <summary>
        /// Gets or sets the percentage sizing behavior in the GridTreeControl.
        /// </summary>
        /// <remarks>
        /// Set the GridTreeColumn.PercentWidth property to enable a particular column to
        /// participate in the automatic sizing as the TreeGridControl client width changes. 
        /// Depending upon the value of PercentSizingBehavior, the free client width left after
        /// all the columns whose PrecentWidth is not set has been subtracted, is proportionally
        /// allocated among all those columns whose PercentWidth is set.
        /// </remarks>
        public GridPercentColumnSizingBehavior PercentSizingBehavior
        {
            get { return (GridPercentColumnSizingBehavior)GetValue(PercentSizingBehaviorProperty); }
            set
            {
                SetValue(PercentSizingBehaviorProperty, value);
            }
        }
        #endregion

        #region FreezeExpandColumn
        /// <exclude/>
        public static readonly DependencyProperty FreezeExpandColumnProperty = DependencyProperty.Register(
            "FreezeExpandColumn",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnFreezeExpandColumnChanged)
#else
 new PropertyMetadata(true, OnReadOnlyChanged)
#endif
);

        private static void OnFreezeExpandColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControl grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.FreezeExpandColumn = (bool)args.NewValue;
                grid.OnNeedRedrawGrid(d, args);
            }
            else
            {
                grid.isFreezeExpandColumnChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets of sets whether the column with the expand/contract glyph is scrollable.
        /// </summary>
        public bool FreezeExpandColumn
        {
            get { return (bool)GetValue(FreezeExpandColumnProperty); }
            set
            {
                SetValue(FreezeExpandColumnProperty, value);
                this.InternalGrid.FrozenColumns = value ? 2 : 1;
            }
        }
        #endregion

        #region ReadOnly

        /// <exclude/>
        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register(
            "ReadOnly",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnReadOnlyChanged)
#else
 new PropertyMetadata(true, OnReadOnlyChanged)
#endif
);

        static void OnReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ReadOnly = (bool)e.NewValue;
            }
            else
            {
                grid.isReadOnlyChangedBeforeGridLoaded = true;
            }
        }
        /// <summary>
        /// Gets or sets whether the GridTreeControl is ReadOnly.
        /// </summary>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);

            }
        }

        #endregion

        #region SupportRowSizing
        /// <exclude/>
        public static readonly DependencyProperty SupportRowSizingProperty = DependencyProperty.Register(
            "SupportRowSizing",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnSupportRowSizingChanged)
#else
 new PropertyMetadata(false, OnSupportRowSizingChanged)
#endif
);

        static void OnSupportRowSizingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.SupportRowSizing = (bool)e.NewValue;
            }
            else
            {
                grid.isSupportRowSizingChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether your user can size row heights with the mouse.
        /// </summary>
        public bool SupportRowSizing
        {
            get
            {
                return (bool)GetValue(SupportRowSizingProperty);
            }
            set
            {
                SetValue(SupportRowSizingProperty, value);
            }
        }
        #endregion

        #region SupportNodeImages
        /// <exclude/>
        public static readonly DependencyProperty SupportNodeImagesProperty = DependencyProperty.Register(
            "SupportNodeImages",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnSupportNodeImagesChanged)
#else
 new PropertyMetadata(false, OnSupportNodeImagesChanged)
#endif
);

        static void OnSupportNodeImagesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.SupportNodeImages = (bool)e.NewValue;
            }
            else
            {
                grid.isSupportNodeImagesChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether an event is raised to request an image to be displayed for a node.
        /// </summary>
        public bool SupportNodeImages
        {
            get
            {
                return (bool)GetValue(SupportNodeImagesProperty);
            }
            set
            {
                SetValue(SupportNodeImagesProperty, value);
            }
        }
        #endregion

        #region SortClickAction


        /// <summary>
        /// Enables/Disables the sorting  on mouse double click.
        /// </summary>
        /// <value>The sort click action.</value>
        public SortClickAction SortClickAction
        {
            get { return (SortClickAction)GetValue(SortClickActionProperty); }
            set { SetValue(SortClickActionProperty, value); }
        }

        public static readonly DependencyProperty SortClickActionProperty =
            DependencyProperty.Register("SortClickAction", typeof(SortClickAction), typeof(GridTreeControl), new PropertyMetadata(SortClickAction.SingleClick));

        #endregion

        #region EnableNodeSelection

        /// <exclude/>
        public static readonly DependencyProperty EnableNodeSelectionProperty = DependencyProperty.Register(
            "EnableNodeSelection",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnEnableNodeSelectionChanged)
#else
 new PropertyMetadata(true, OnEnableNodeSelectionChanged)
#endif
);

        static void OnEnableNodeSelectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableNodeSelection = (bool)e.NewValue;
            }
            else
            {
                grid.isEnableNodeSelectionChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether your user can select whole row when a cell is clicked with the mouse.
        /// </summary>
        /// <remarks>
        /// Setting this property to true also sets GridTreeControl.InternalGrid.Model.Options.ListBoxSelectionMode to
        /// MultiExtended. If you want the selected behavior to be something other than MultiExtended, you
        /// will need to explicitly set it after setting this property.
        /// </remarks>
        public bool EnableNodeSelection
        {
            get
            {
                return (bool)GetValue(EnableNodeSelectionProperty);
            }
            set
            {
                SetValue(EnableNodeSelectionProperty, value);
            }
        }

        #endregion

        #region TrackSelectionOnCollectionChange
        /// <summary>
        /// Gets or Sets whether to track the selection when the binded collection changed
        /// </summary>
        public bool TrackSelectionOnCollectionChange
        {
            get
            {
                return (bool)GetValue(TrackSelectionOnCollectionChangeProperty);
            }
            set
            {
                SetValue(TrackSelectionOnCollectionChangeProperty, value);
            }
        }

        public static readonly DependencyProperty TrackSelectionOnCollectionChangeProperty = DependencyProperty.Register(
            "TrackSelectionOnCollectionChange",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnTrackSelectionOnCollectionChangeChanged)
#else
 new PropertyMetadata(false, OnTrackSelectionOnCollectionChangeChanged)
#endif
);

        static void OnTrackSelectionOnCollectionChangeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.TrackSelectionOnCollectionChange = (bool)e.NewValue;
            }
            else
            {
                grid.isTrackSelectionOnCollectionChangeBeforeGridLoaded = true;
            }
        }

        #endregion SelectionsTrackInsertsDeletes
        
        #region EnableSelections

        /// <exclude/>
        public static readonly DependencyProperty EnableSelectionsProperty = DependencyProperty.Register(
            "EnableSelections",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnEnableSelectionsChanged)
#else
 new PropertyMetadata(true, OnEnableSelectionsChanged)
#endif
);

        static void OnEnableSelectionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                if (!(bool)e.NewValue)
                {
                    grid.EnableNodeSelection = false;
                    grid.InternalGrid.Model.Options.AllowSelection = GridSelectionFlags.None;
                }
                else
                {
                    grid.InternalGrid.Model.Options.AllowSelection = GridSelectionFlags.Any;
                    grid.InternalGrid.Model.Options.ListBoxSelectionMode = GridSelectionMode.None;                    
                    grid.InternalGrid.Model.Options.ExcelLikeCurrentCell = true;
                }
            }
            else
            {
                grid.isEnableSelectionsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether selections are allowed in teh GridTreeControl.
        /// </summary>

        public bool EnableSelections
        {
            get
            {
                return (bool)GetValue(EnableSelectionsProperty);
            }
            set
            {
                SetValue(EnableSelectionsProperty, value);
            }
        }
         #endregion

        #region EnableRenderCheckIfGlyphNeeded
        /// <exclude/>
        public static readonly DependencyProperty EnableRenderCheckIfGlyphNeededProperty = DependencyProperty.Register(
            "EnableRenderCheckIfGlyphNeeded",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnEnableRenderCheckIfGlyphNeededChanged)
#else
 new PropertyMetadata(false, OnEnableRenderCheckIfGlyphNeededChanged)
#endif
);

        static void OnEnableRenderCheckIfGlyphNeededChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
               
               grid.EnableRenderCheckIfGlyphNeeded = (bool)e.NewValue; 
                
            }

        }

        /// <summary>
        /// Gets or sets whether Sorting are allowed  while editing the cells in GridTreeControl.
        /// </summary>
        public bool EnableRenderCheckIfGlyphNeeded 
        {
            get
            {
                return (bool)GetValue(EnableRenderCheckIfGlyphNeededProperty);
            }
            set
            {
                SetValue(EnableRenderCheckIfGlyphNeededProperty, value);
            }
        }
#endregion

        #region EnableSortOnEdit
        /// <exclude/>
        public static readonly DependencyProperty EnableSortOnEditProperty = DependencyProperty.Register(
            "EnableSortOnEdit",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true)
#else
 new PropertyMetadata(true)
#endif
);

        /// <summary>
        /// Gets or sets whether Sorting are allowed  while editing the cells in GridTreeControl.
        /// </summary>
        /// 
        [Obsolete]
        public bool EnableSortOnEdit
        {
            get
            {
                return (bool)GetValue(EnableSortOnEditProperty );
            }
            set
            {
                SetValue(EnableSortOnEditProperty, value);
            }
        }

        #endregion
        #region EnableMultiColumnSorting
        /// <exclude/>
        public static readonly DependencyProperty EnableMultiColumnSortingProperty = DependencyProperty.Register(
            "EnableMultiColumnSorting",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true)
#else
 new PropertyMetadata(true)
#endif
);


        /// <summary>
        /// Gets or sets a value indicating whether [enable multi column sorting].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable multi column sorting]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMultiColumnSorting
        {
            get
            {
                return (bool)GetValue(EnableMultiColumnSortingProperty);
            }
            set
            {
                SetValue(EnableMultiColumnSortingProperty, value);
            }
        }

        #endregion

        #region EnableTriStateSorting
        /// <exclude/>
        public static readonly DependencyProperty EnableTriStateSortingProperty = DependencyProperty.Register(
            "EnableTriStateSorting",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false)
#else
 new PropertyMetadata(false)
#endif
);
        /// <summary>
        /// Gets or sets a value indicating whether [enable tri state sorting].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable tri state sorting]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTriStateSorting
        {
            get
            {
                return (bool)GetValue(EnableTriStateSortingProperty);
            }
            set
            {
                SetValue(EnableTriStateSortingProperty, value);
            }
        }

        #endregion

        #region SortingOptions
        public GridTreeSortingOptions SortingOptions
        {
            get { return (GridTreeSortingOptions)GetValue(SortingOptionsProperty); }
            set { SetValue(SortingOptionsProperty, value); }
        }

        public static readonly DependencyProperty SortingOptionsProperty =
            DependencyProperty.Register("SortingOptions", typeof(GridTreeSortingOptions), typeof(GridTreeControl), new PropertyMetadata(GridTreeSortingOptions.Default));
        #endregion

        #region ParentPropertyName

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.ParentPropertyName"/>.
        /// </summary>
        public static readonly DependencyProperty ParentPropertyNameProperty = DependencyProperty.Register(
            "ParentPropertyName",
            typeof(string),
            typeof(GridTreeControl),
            new PropertyMetadata(string.Empty));

#if !SILVERLIGHT
        public static readonly RoutedEvent ParentPropertyNameChangedEvent = EventManager.RegisterRoutedEvent("ParentPropertyNameChanged", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridTreeControl));


        public event GridRoutedEventHandler ParentPropertyNameChanged
        {
            add
            {
                this.AddHandler(GridTreeControl.ParentPropertyNameChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridTreeControl.ParentPropertyNameChangedEvent, value);
            }
        }
#else
        public event GridRoutedEventHandler ParentPropertyNameChanged;

        protected virtual void OnParentPropertyNameChanged(SyncfusionRoutedEventArgs args)
        {
            GridRoutedEventHandler handler = ParentPropertyNameChanged;
            if (handler != null) handler(this, args);
        }
#endif

        /// <summary>
        /// Gets or sets the property name for the parent object where <see cref="ItemsSource"/> is used to 
        /// define the root items for this tree when a self-relation is determining the tree structure. If you are using
        /// a self-relation to define the tree structure, instead of the root collection, this list should contain the
        /// flat list of objects for the self-relation.
        /// </summary>
        /// <value>The parent property name.</value>
        /// <remarks>
        /// Use this property only if you do not want to use the <see cref="RequestChildItems"/> event to populate
        /// the tree nodes directly. You set this property to be the IEnumerable collection of root nodes for this
        /// tree. There are requirements for the populating the tree using this ItemsSource property:
        /// 
        /// 1) All nodes (both root and children) must share the same column schema.
        /// 
        /// 2) The child items are either defined in one of these two ways:
        ///    a) As an IEnumerable property of the parent item. In this case, the <see cref="ChildPropertyName"/> holds
        /// the name of this property.
        /// <example>
        ///   <para>This code illustrates how you would use this ItemsSource property to define a tree where
        ///   you have a collection of Person objects named personCollection, and each Person object in the personCollection
        ///   has an IEnumerable property named Children that itself holds an IEnumerable collection of Person objects.
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = personCollection; //just the root Persons
        ///   gridTreeControl1.ChildPropertyName = "Children";
        ///   gridTreeControl1.ParentPropertyName = ""; //empty ParentPropertyName implies child collection
        ///   </code>
        /// </example>
        /// b) As a self-relation between two of the properties of the underlying object. In this case, the <see cref="ChildPropertyName"/> 
        /// must be set to the name of key property, and the <see cref="ParentPropertyName"/> must be set to the parent object for this item.
        /// <example>
        ///   <para>This code illustrates how you might set up a self-relation using a collection
        ///   of employees where each object has an EmployeeID property and a ReportsTo property that defines
        ///   the tree structure on the IEnumerable employees collection.</para>
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = employees; //all employees
        ///   gridTreeControl1.ChildPropertyName = "EmployeeID";
        ///   gridTreeControl1.ParentPropertyName = "ReportsTo"; //nonempty ParentPropertyName implies self-relation
        ///   gridTreeControl1.SelfRelationRootValue = -1; //key value used to pick root values
        ///   </code>
        /// </example>
        /// </remarks>
        public string ParentPropertyName
        {
            get
            {
                return (string)this.GetValue(GridTreeControl.ParentPropertyNameProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.ParentPropertyNameProperty, value);
            }
        }

        #endregion

        #region SelfRelationRootValue

        private object selfRelationRootValue = null;

        /// <summary>
        /// Gets or sets the value that defines the root object in a self-related tree when 
        /// <see cref="ItemsSource"/> is used to define the underlying tree data.
        /// </summary>
        public object SelfRelationRootValue
        {
            get { return selfRelationRootValue; }
            set { selfRelationRootValue = value; }
        }


        #endregion

        #region ChildPropertyName

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.ChildPropertyName"/>.
        /// </summary>
        public static readonly DependencyProperty ChildPropertyNameProperty = DependencyProperty.Register(
            "ChildPropertyName",
            typeof(string),
            typeof(GridTreeControl),
            new PropertyMetadata(string.Empty));

#if !SILVERLIGHT
        public static readonly RoutedEvent ChildPropertyNameChangedEvent = EventManager.RegisterRoutedEvent("ChildPropertyNameChanged", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridTreeControl));

        public event GridRoutedEventHandler ChildPropertyNameChanged
        {
            add
            {
                this.AddHandler(GridTreeControl.ChildPropertyNameChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridTreeControl.ChildPropertyNameChangedEvent, value);
            }
        }
#else
        public event GridRoutedEventHandler ChildPropertyNameChanged;

        protected virtual void OnChildPropertyNameChanged(SyncfusionRoutedEventArgs args)
        {
            GridRoutedEventHandler handler = ChildPropertyNameChanged;
            if (handler != null) handler(this, args);
        }
#endif

        /// <summary>
        /// Gets or sets the property name for the child object where <see cref="ItemsSource"/> is used to 
        /// define the root items for this tree. If you are using
        /// a self-relation to define the tree structure, instead of the root collection, this list should contain the
        /// flat list of objects for the self-relation.
        /// </summary>
        /// <value>The child property name.</value>
        /// <remarks>
        /// Use this property only if you do not want to use the <see cref="RequestChildItems"/> event to populate
        /// the tree nodes directly. You set this property to be the IEnumerable collection of root nodes for this
        /// tree. There are requirements for the populating the tree using this ItemsSource property:
        /// 
        /// 1) All nodes (both root and children) must share the same column schema.
        /// 
        /// 2) The child items are either defined in one of these two ways:
        ///    a) As an IEnumerable property of the parent item. In this case, the <see cref="ChildPropertyName"/> holds
        /// the name of this property.
        /// <example>
        ///   <para>This code illustrates how you would use this ItemsSource property to define a tree where
        ///   you have a collection of Person objects named personCollection, and each Person object in the personCollection
        ///   has an IEnumerable property named Children that itself holds an IEnumerable collection of Person objects.
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = personCollection; //just the root Persons
        ///   gridTreeControl1.ChildPropertyName = "Children";
        ///   gridTreeControl1.ParentPropertyName = ""; //empty ParentPropertyName implies child collection
        ///   </code>
        /// </example>
        /// b) As a self-relation between two of the properties of the underlying object. In this case, the <see cref="ChildPropertyName"/> 
        /// must be set to the name of key property, and the <see cref="ParentPropertyName"/> must be set to the parent object for this item.
        /// <example>
        ///   <para>This code illustrates how you might set up a self-relation using a collection
        ///   of employees where each object has an EmployeeID property and a ReportsTo property that defines
        ///   the tree structure on the IEnumerable employees collection.</para>
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = employees; //all employees
        ///   gridTreeControl1.ChildPropertyName = "EmployeeID";
        ///   gridTreeControl1.ParentPropertyName = "ReportsTo"; //nomempty ParentPropertyName implies self-relation
        ///   gridTreeControl1.SelfRelationRootValue = -1; //value that picks root members from teh falt list
        ///   </code>
        /// </example>
        /// </remarks>
        public string ChildPropertyName
        {
            get
            {
                return (string)this.GetValue(GridTreeControl.ChildPropertyNameProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.ChildPropertyNameProperty, value);
            }
        }

        #endregion

        #region ItemsSource

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(GridTreeControl),
            new PropertyMetadata(OnItemsSourceChanged));


        static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl tree = o as GridTreeControl;            
            //Resets the InternalGrid when ItemSource changed with null value.
            if (e.NewValue == null && tree.internalGrid != null && tree.internalGrid.RootNodes != null)
            {
                tree.InternalGrid.RootNodes.Clear();
            }

            //Clear the SelectedRanges
            if (e.OldValue !=null && tree.Model.SelectedRanges != null)
                tree.Model.SelectedRanges.Clear();
            //Reset the SelectedNodes collection
            if (tree.SelectedNodes.Count > 0)
                tree.SelectedNodes.Clear();

            //Resets the GridTreeVisibleColumns when itemsource changes
            if (e.OldValue != null && tree != null)
            {
                if (tree.InternalGrid != null)
                {
                    tree.InternalGrid.CurrentCell.Deactivate();
                    tree.InternalGrid.UnWireCollectionChangedEvent(e.OldValue);

                    if (e.OldValue is INotifyCollectionChanged)
                    {
                        //tree.internalGrid.UnWireCollectionChangedEvent(e.OldValue);
                        tree.InternalGrid.ClearChildBindingsFromICollectionChanged();
                    }
#if !SILVERLIGHT
                    else if (e.OldValue is IBindingList)
                    {
                        tree.InternalGrid.UnwireBindingListEvents((IBindingList)e.OldValue);
                        tree.internalGrid.ClearChildBindingsFromIListChanged();
                    }
#endif
                }
                if (tree.Columns != null && tree.AutoPopulateColumns)
                    tree.Columns.Clear();
            }

            if (tree != null)
            {
#if !SILVERLIGHT
                SyncfusionRoutedEventArgs newEventArgs = new SyncfusionRoutedEventArgs(GridTreeControl.ItemsSourceChangedEvent, tree);
                if (newEventArgs != null)
                {
                    tree.RaiseEvent(newEventArgs);
                }
#else
                if (tree.ItemsSourceChanged != null)
                {
                    tree.ItemsSourceChanged(tree, SyncfusionRoutedEventArgs.Empty);
                }
#endif
            }

            if (tree != null && tree.InternalGrid != null)
            {
                tree.InternalGrid.Nodes.Clear(); 
                if (tree.ItemsSource != null)
                {
                    if (tree.InternalGrid.Columns.Count > 0 && tree.AllowAutoSizingNodeColumn)
                    {
                        if (tree.InternalGrid.PercentSizingBehavior == GridPercentColumnSizingBehavior.None)
                            tree.InternalGrid.Columns[0].Width = tree.InternalGrid.ColumnWidths.DefaultLineSize;
                        else
                            tree.InternalGrid.ColumnWidthSizer.ApplySizes();
                    }
                    tree.InternalGrid.ItemPropertyType = null; //force properties to be reloaded
                    tree.InternalGrid.pdc = null; //force properties to be reloaded 
                    tree.InternalGrid.PopulateTree();
                    tree.InternalGrid.WireCollectionChangedEvent(tree.ItemsSource);
                    if (tree.Columns.Count > 0)
                    {
                        if (tree.InternalGrid.ExpandStateAtStartUp == GridTreeStartUpExpandState.AllNodesExpanded)
                        {
                            tree.ExpandAllNodes();
                        }
                        else if (tree.InternalGrid.ExpandStateAtStartUp == GridTreeStartUpExpandState.NoNodesExpanded)
                        {
                            tree.CollapseAllNodes();
                        }
                        else if (tree.InternalGrid.ExpandStateAtStartUp == GridTreeStartUpExpandState.RootNodesExpanded)
                        {
                            foreach (GridTreeNode node in tree.InternalGrid.RootNodes)
                            {
                                tree.ExpandNode(node);
                            }
                        }
                    }
                }
                else
                    tree.InternalGrid.PopulateTree();
                tree.InternalGrid.InvalidateCells();
            }
        }

#if !SILVERLIGHT
        public static readonly RoutedEvent ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridTreeControl));

        public event GridRoutedEventHandler ItemsSourceChanged
        {
            add
            {
                this.AddHandler(GridTreeControl.ItemsSourceChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridTreeControl.ItemsSourceChangedEvent, value);
            }
        }
#else
        public event GridRoutedEventHandler ItemsSourceChanged;
#endif
        /// <summary>
        /// Gets or sets the IEnumerable list that holds the root collection for this GridTreeControl. If you are using
        /// a self-relation to define the tree structure, instead of the root collection, this list should contain the
        /// flat list of objects for the self-relation.
        /// </summary>
        /// <value>The underlying data source.</value>
        /// <remarks>
        /// Use this property only if you do not want to use the <see cref="RequestChildItems"/> event to populate
        /// the tree nodes directly. You set this property to be the IEnumerable collection of root nodes for this
        /// tree. There are requirements for the populating the tree using this ItemsSource property:
        /// 
        /// 1) All nodes (both root and children) must share the same column schema.
        /// 
        /// 2) The child items are either defined in one of these two ways:
        ///    a) As an IEnumerable property of the parent item. In this case, the <see cref="ChildPropertyName"/> holds
        /// the name of this property.
        /// <example>
        ///   <para>This code illustrates how you would use this ItemsSource property to define a tree where
        ///   you have a collection of Person objects named personCollection, and each Person object in the personCollection
        ///   has an IEnumerable property named Children that itself holds an IEnumerable collection of Person objects.
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = personCollection; //just the root Persons
        ///   gridTreeControl1.ChildPropertyName = "Children";
        ///   gridTreeControl1.ParentPropertyName = ""; //empty ParentPropertyName implies child collection
        ///   </code>
        /// </example>
        /// b) As a self-relation between two of the properties of the underlying object. In this case, the <see cref="ChildPropertyName"/> 
        /// must be set to the name of key property, and the <see cref="ParentPropertyName"/> must be set to the parent object for this item.
        /// <example>
        ///   <para>This code illustrates how you might set up a self-relation using a collection
        ///   of employees where each object has an EmployeeID property and a ReportsTo property that defines
        ///   the tree structure on the IEnumerable employees collection.</para>
        ///   <code lang="C#">
        ///   gridTreeControl1.ItemsSource = employees; //all employees
        ///   gridTreeControl1.ChildPropertyName = "EmployeeID";
        ///   gridTreeControl1.ParentPropertyName = "ReportsTo"; //nomempty ParentPropertyName implies self-relation
        ///   gridTreeControl1.SelfRelationRootValue = -1; //value that picks root members from teh falt list
        ///   </code>
        /// </example>
        /// </remarks>
        public object ItemsSource
        {
            get
            {
                return this.GetValue(GridTreeControl.ItemsSourceProperty);
            }

            set
            {
                if (ItemsSource != value)
                {
                    this.SetValue(GridTreeControl.ItemsSourceProperty, value);
                }
            }
        }

        #endregion

        #region EnableHotRowMarker

        /// <exclude/>
        public static readonly DependencyProperty EnableHotRowMarkerProperty = DependencyProperty.Register(
            "EnableHotRowMarker",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnEnableHotRowMarkerChanged)
#else
 new PropertyMetadata(true, OnEnableHotRowMarkerChanged)
#endif
);

        static void OnEnableHotRowMarkerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableHotRowMarker = (bool)e.NewValue;
            }
            else
            {
                grid.isEnableHotRowMarkerChangedBeforeGridLoaded = true;
            }
        }
        /// <summary>
        /// Gets or sets whether the row under the current mouse position is redrawn using
        /// the HotRowMarker as a background brush.
        /// </summary>
        public bool EnableHotRowMarker
        {
            get
            {
                return (bool)GetValue(EnableHotRowMarkerProperty);
            }
            set
            {
                SetValue(EnableHotRowMarkerProperty, value);
            }
        }
        #endregion

#if !SILVERLIGHT
        #region EnableRenderOptimization
        
        public EnableRenderOptimization EnableRenderOptimization
        {
            get
            {
                return (EnableRenderOptimization)this.GetValue(GridTreeControl.EnableRenderOptimizationProperty);
            }
            set
            {
                this.SetValue(GridTreeControl.EnableRenderOptimizationProperty, value);
            }
        }

        /// <summary>
        /// DependencyPropery for <see cref = "GridTreeControl.EnableRenderOptimizationProperty" />.
        /// </summary>
        public static readonly DependencyProperty EnableRenderOptimizationProperty = DependencyProperty.Register(
            "EnableRenderOptimization",
            typeof(EnableRenderOptimization),
            typeof(GridTreeControl),
            new FrameworkPropertyMetadata(EnableRenderOptimization.None, OnEnableRenderOptimizationChanged));

        private bool isEnableRenderOptimizationSetBeforeLoaded = false;

        private static void OnEnableRenderOptimizationChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableRenderOptimization = (EnableRenderOptimization)args.NewValue;
            }
            else
            {
                grid.isEnableRenderOptimizationSetBeforeLoaded = true;
            }
        }
        #endregion
#endif

        #region ShowExpandColumnBorders

        /// <exclude/>
        public static readonly DependencyProperty ShowExpandColumnBordersProperty = DependencyProperty.Register(
            "ShowExpandColumnBorders",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnShowExpandColumnBordersChanged)
#else
 new PropertyMetadata(true, OnShowExpandColumnBordersChanged)
#endif
);

        static void OnShowExpandColumnBordersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ShowExpandColumnBorders = (bool)e.NewValue;
            }
            else
            {
                grid.isShowExpandColumnBordersChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether the grid lines are seen in the expand column.
        /// </summary>
        public bool ShowExpandColumnBorders
        {
            get { return (bool)GetValue(ShowExpandColumnBordersProperty); }
            set { SetValue(ShowExpandColumnBordersProperty, value); }
        }

        #endregion

        #region RowHeaderWidth

        /// <exclude/>
        public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register(
            "RowHeaderWidth",
            typeof(double),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(20d, OnRowHeaderWidthChanged)
#else
 new PropertyMetadata(20d, OnRowHeaderWidthChanged)
#endif
);

        static void OnRowHeaderWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.RowHeaderWidth = (double)e.NewValue;
            }
            else
            {
                grid.isRowHeaderWidthChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the width of the row header column.
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set
            {
                SetValue(RowHeaderWidthProperty, value);
            }
        }


        #endregion

        #region ShowColumnHeaders

        /// <exclude/>
        public static readonly DependencyProperty ShowColumnHeadersProperty = DependencyProperty.Register(
            "ShowColumnHeaders",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnShowColumnHeadersChanged)
#else
 new PropertyMetadata(true, OnShowColumnHeadersChanged)
#endif
);

        static void OnShowColumnHeadersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ShowColumnHeaders = (bool)e.NewValue;
            }
            else
            {
                grid.isShowColumnHeadersChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether a header row is visible
        /// </summary>
        public bool ShowColumnHeaders
        {
            get { return (bool)GetValue(ShowColumnHeadersProperty); }
            set
            {
                SetValue(ShowColumnHeadersProperty, value);
            }
        }

        #endregion

        #region AutoPopulateColumns

        /// <exclude/>
        public static readonly DependencyProperty AutoPopulateColumnsProperty = DependencyProperty.Register(
            "AutoPopulateColumns",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnAutoPopulateColumnsChanged)
#else
 new PropertyMetadata(true, OnAutoPopulateColumnsChanged)
#endif
);

        static void OnAutoPopulateColumnsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.AutoPopulateColumns = (bool)e.NewValue;
            }
            else
            {
                grid.isAutoPopulateColumnsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// If AutoPopulateColumns is set to True, then visible column defined will be populated in the View.
        /// else all the columns in the itemsource will be populated in the view
        /// </summary>
        public bool AutoPopulateColumns
        {
            get { return (bool)GetValue(AutoPopulateColumnsProperty); }
            set
            {
                SetValue(AutoPopulateColumnsProperty, value);
            }
        }

        #endregion

        #region AutoGenerateColumnsInfo

        /// <summary>
        /// Gets or sets a value indicating whether to generate the CellType automatically
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto generate columns info]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoGenerateColumnsInfo
        {
            get { return (bool)GetValue(AutoGenerateColumnsInfoProperty); }
            set { SetValue(AutoGenerateColumnsInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenerateColumnsInfo.
        public static readonly DependencyProperty AutoGenerateColumnsInfoProperty =
            DependencyProperty.Register("AutoGenerateColumnsInfo", typeof(bool), typeof(GridTreeControl),
            new PropertyMetadata(false, OnAutoGenerateColumnsInfoChanged));

        /// <summary>
        /// Called when [auto generate columns info changed].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnAutoGenerateColumnsInfoChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl treeGrid = o as GridTreeControl;
            if (treeGrid.isGridLoaded)
                treeGrid.InternalGrid.AutoGenerateColumnsInfo = (bool)e.NewValue;
            else
                treeGrid.isAutoGenerateColumnsInfoChangedBeforeGridLoaded = (bool)e.NewValue;
        }
        #endregion

        #region Columns

        /// <exclude/>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(ObservableCollection<GridTreeColumn>),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(new ObservableCollection<GridTreeColumn>(), OnColumnsChanged)
#else
 new PropertyMetadata(new ObservableCollection<GridTreeColumn>(), OnColumnsChanged)
#endif
);

        static void OnColumnsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.Columns = (ObservableCollection<GridTreeColumn>)e.NewValue;
            }
            else
            {
                grid.isColumnsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// A collection of the TreeColumns that control the number and order of the columns
        /// that appear in the Tree.
        /// </summary>
        public ObservableCollection<GridTreeColumn> Columns
        {
            get
            {
                return (ObservableCollection<GridTreeColumn>)GetValue(ColumnsProperty);
            }
            set { SetValue(ColumnsProperty, value); }
        }
        #endregion

        #region AllowAutoSizingNodeColumn

        /// <exclude/>
        public static readonly DependencyProperty AllowAutoSizingNodeColumnProperty = DependencyProperty.Register(
            "AllowAutoSizingNodeColumn",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnAllowAutoSizingNodeColumnChanged)
#else
 new PropertyMetadata(true, OnAllowAutoSizingNodeColumnChanged)
#endif
);

        static void OnAllowAutoSizingNodeColumnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.AllowAutoSizingNodeColumn = (bool)e.NewValue;
            }
            else
            {
                grid.isAllowAutoSizingNodeColumnChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether the tree node column's width is automatically adjusted as the node levels increase.
        /// </summary>
        /// <remarks>This property is ignored is PercentageSizing is enabled and the node column is marked to participate
        /// in the percentage sizing calculations.</remarks>
        public bool AllowAutoSizingNodeColumn
        {
            get { return (bool)GetValue(AllowAutoSizingNodeColumnProperty); }
            set { SetValue(AllowAutoSizingNodeColumnProperty, value); }
        }
        #endregion


        #region AutosizingNodeColumnOption

        /// <exclude/>
        public static readonly DependencyProperty AutosizingNodeColumnOptionProperty = DependencyProperty.Register(
            "AutosizingNodeColumnOption",
            typeof(GridNodeAutosizingOption),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(GridNodeAutosizingOption.BasedOnNodeCount)
#else
 new PropertyMetadata(GridNodeAutosizingOption.BasedOnNodeCount)
#endif
);


        /// <summary>
        /// Gets or sets whether Sizing of nodes based on NodeCount or Level
        /// </summary>
        public GridNodeAutosizingOption AutosizingNodeColumnOption
        {
            get
            {
                return (GridNodeAutosizingOption)GetValue(AutosizingNodeColumnOptionProperty);
            }
            set
            {
                SetValue(AutosizingNodeColumnOptionProperty, value);
            }
        }

        #endregion

        #region ColumnHeaderStyle

        /// <exclude/>
        public static readonly DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(
            "ColumnHeaderStyle",
            typeof(GridStyleInfo),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(GridStyleInfo.Empty, OnColumnHeaderStyleChanged)
#else
 new PropertyMetadata(GridStyleInfo.Empty, OnColumnHeaderStyleChanged)
#endif
);

        static void OnColumnHeaderStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ColumnHeaderStyle = (GridStyleInfo)e.NewValue;
            }
            else
            {
                grid.isColumnHeaderStyleChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a GridStyleInfo object that defines the style information for the column headers.
        /// </summary>
        public GridStyleInfo ColumnHeaderStyle
        {
            get { return (GridStyleInfo)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        #endregion

        #region LevelStyles

        /// <exclude/>
        public static readonly DependencyProperty LevelStylesProperty = DependencyProperty.Register(
            "LevelStyles",
            typeof(List<GridStyleInfo>),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(new List<GridStyleInfo>(), OnLevelStylesChanged)
#else
 new PropertyMetadata(new List<GridStyleInfo>(), OnLevelStylesChanged)
#endif
);

        static void OnLevelStylesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.LevelStyles = (List<GridStyleInfo>)e.NewValue;
            }
            else
            {
                grid.isLevelStylesChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        ///  Gets or sets a collection of GridStyleInfo objects that will be applied to specific levels.
        /// </summary>
        public List<GridStyleInfo> LevelStyles
        {
            get { return (List<GridStyleInfo>)GetValue(LevelStylesProperty); }
            set { SetValue(LevelStylesProperty, value); }
        }

        #endregion

        #region SupportsVisualStyles

        /// <exclude/>
        public static readonly DependencyProperty SupportsVisualStylesProperty = DependencyProperty.Register(
            "SupportsVisualStyles",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnSupportsVisualStylesChanged)
#else
 new PropertyMetadata(true, OnSupportsVisualStylesChanged)
#endif
);

        static void OnSupportsVisualStylesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = o as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.SupportsVisualStyles = (bool)e.NewValue;
            }
            else
            {
                grid.isSupportsVisualStylesBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether the GridTreeControl should adjust its appearance according to the <see cref="SkinStorage.VisualStyle"/> setting.
        /// </summary>

        public bool SupportsVisualStyles
        {
            get { return (bool)GetValue(SupportsVisualStylesProperty); }
            set { SetValue(SupportsVisualStylesProperty, value); }
        }
        #endregion


        #region StyleManager

        public GridTreeStyleManager StyleManager
        {
            get { return (GridTreeStyleManager)GetValue(StyleManagerProperty); }
            set { SetValue(StyleManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleManagerProperty =
            DependencyProperty.Register("StyleManager", typeof(GridTreeStyleManager), typeof(GridTreeControl), new PropertyMetadata(null));
       
        #endregion 

        #region HideEmptyChild
        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.HideEmptyChild"/>.
        /// </summary>
        public static readonly DependencyProperty HideEmptyChildGlyphsProperty = DependencyProperty.Register(
            "HideEmptyChildGlyphs",
            typeof(bool),
            typeof(GridTreeControl),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnHideEmptyChildGlyphsChanged)
#else
 new PropertyMetadata(true, OnHideEmptyChildGlyphsChanged)
#endif
);

        private static void OnHideEmptyChildGlyphsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridTreeControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.HideEmptyChildGlyphs = (bool)args.NewValue;
            }
            else
            {
                grid.isHideEmptyChildGlyphsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets whether empty child nodes show the +/- cells when they are initially displayed.
        /// </summary>
        /// <value><c>true</c> if empty child nodes should not show the +/- ; otherwise, <c>false</c>.</value>
        /// <remarks>The default GridTreeControl raises the RequestTreeItem event for each child node as it becomes
        /// visible the first time so the GridTreeControl will not display the +/- for nodes that have no 
        /// children. If you set this property to false, the GridTreeControl will not raise the RequestTreeItems
        /// for the child until the child node is clicked to be expanded. At that point, if the child node has
        /// no children, the +/- will go away.
        /// </remarks>
        public bool HideEmptyChildGlyphs
        {
            get
            {
                return (bool)this.GetValue(GridTreeControl.HideEmptyChildGlyphsProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.HideEmptyChildGlyphsProperty, value);
            }
        }
        #endregion

        #region ExpandStateAtStartUp (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ExandStateAtStartUp property.
        /// </summary>
        public GridTreeStartUpExpandState ExpandStateAtStartUp
        {
            get { return (GridTreeStartUpExpandState)GetValue(ExpandStateAtStartUpProperty); }
            set { SetValue(ExpandStateAtStartUpProperty, value); }
        }
        private bool isExpandStateAtStartUpChangedBeforeGridLoaded = false;

        public static readonly DependencyProperty ExpandStateAtStartUpProperty = DependencyProperty.Register("ExpandStateAtStartUp", typeof(GridTreeStartUpExpandState), typeof(GridTreeControl), new PropertyMetadata(GridTreeStartUpExpandState.RootNodesExpanded, OnExpandStateAtStartUpChanged));

        private static void OnExpandStateAtStartUpChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridTreeControl;
            if (grid.InternalGrid != null)
            {
                grid.InternalGrid.ExpandStateAtStartUp = (GridTreeStartUpExpandState)args.NewValue;
            }
            else
            {
                grid.isExpandStateAtStartUpChangedBeforeGridLoaded = true;
            }
        }

        #endregion

        #region ExpandGlyphType
        /// <summary>
        /// Gets or sets the type of the glyph shown in the expand cell.
        /// </summary>
        /// <remarks>
        /// The default value is a triangle. You can also set a +- glyph, or a +-glyph with tree lines, or
        /// a custom drawn glyph. The property NodeColumnWidth reserves the required width of your glyph. The
        /// default value of NodeColumnWidth is 10 which is the setting used for the triangle glyph. For the
        /// +- glyph, the value of NodeColumnWidth is set to 14. If you want to explicitly provide a particular
        /// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        /// ExpandGlypType also possibly resets NodeColumnWidth.
        /// </remarks>
        public GridTreeExpandGlyph ExpandGlyphType
        {
            get { return (GridTreeExpandGlyph)GetValue(ExpandGlyphTypeProperty); }
            set { SetValue(ExpandGlyphTypeProperty, value); }
        }

        public static readonly DependencyProperty ExpandGlyphTypeProperty =
            DependencyProperty.Register("ExpandGlyphType", typeof(GridTreeExpandGlyph), typeof(GridTreeControl), new PropertyMetadata(GridTreeExpandGlyph.Triangle, OnExpandGlyphTypeChanged));

        bool isExpandGlyphTypeChangedBeforeGridLoaded = false;
        private static void OnExpandGlyphTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridTreeControl;
            if (grid.InternalGrid != null)
            {
                grid.InternalGrid.ExpandGlyphType = (GridTreeExpandGlyph)args.NewValue;
            }
            else
            {
                grid.isExpandGlyphTypeChangedBeforeGridLoaded = true;
            }
        }
        #endregion


        #region UpdateMode
        /// <summary>
        /// DependencyProperty for <see cref="GridTreeControl.UpdateMode"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateModeProperty = DependencyProperty.Register("UpdateMode", typeof(UpdateMode), typeof(GridTreeControl), new PropertyMetadata(UpdateMode.LostFocus, null));


        /// <summary>
        /// Gets or sets the update mode.
        /// </summary>
        /// <value>The update mode.</value>
        public UpdateMode UpdateMode
        {
            get
            {
                return (UpdateMode)this.GetValue(GridTreeControl.UpdateModeProperty);
            }

            set
            {
                this.SetValue(GridTreeControl.UpdateModeProperty, value);
            }
        }

        #endregion

        #endregion

        #region Properties
        #region EnableLegacyStyle
        public bool EnableLegacyStyle
        {
            get { return (bool)GetValue(GridTreeControl.EnableLegacyStyleProperty); }
            set { SetValue(GridTreeControl.EnableLegacyStyleProperty, value); }
        }
        
        public static readonly DependencyProperty EnableLegacyStyleProperty =
            DependencyProperty.Register("EnableLegacyStyle", typeof(bool), typeof(GridTreeControl), new PropertyMetadata(false,new PropertyChangedCallback(OnEnableLegacyStylePropertyChanged)));        

         private static void OnEnableLegacyStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
         {
             GridTreeControl grid = d as GridTreeControl;
             if (grid.InternalGrid != null)
                 grid.InternalGrid.EnableLegacyStyle = (bool)args.NewValue;
         }
        #endregion
         private GridSelectedTreeNodes ctorSelectedNodes = null;

        /// <summary>
        /// Gets a collection of the selected nodes within the GridTreeControl.
        /// </summary>
        /// <remarks>If this property is acessed before the InternalGrid has been 
        /// initialized, it will return null.</remarks>
        public GridSelectedTreeNodes SelectedNodes
        {
            get
            {
                if (ctorSelectedNodes == null)
                {
                    ctorSelectedNodes = new GridSelectedTreeNodes(null);
                }
                return ctorSelectedNodes;
            }
        }

        private GridTreeModel ctorModel = null;

        /// <summary>
        /// Gets the GridTreeModel model. 
        /// </summary>
        /// <value>The model.</value>
        public GridTreeModel Model
        {
            get
            {
                if (this.InternalGrid != null)
                {
                    return this.InternalGrid.Model as GridTreeModel;
                }

                if (ctorModel == null)
                {
                    this.ctorModel = new GridTreeModel();
                }
                return ctorModel;
            }
        }



        #endregion

        ScrollViewer PartScrollViewer;

        #region overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.internalGrid = this.GetTemplateChild(GridTreeControl.TemplateGrid) as GridTreeControlImpl;
            if (this.InternalGrid != null)
            {
                this.InternalGrid.EnableLegacyStyle = this.EnableLegacyStyle;
                this.InternalGrid.parentTreeControl = this;
                this.isGridLoaded = true;                
                this.PartScrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
                this.HookGrid(true);
            }
        }

        #endregion

        #region internal and private members

        //force the grid to redraw after a property change....
        private void OnMouseController(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl tree = o as GridTreeControl;
            if (tree != null)
            {
                GridTreeControlImpl.InitializeMouseController(tree.InternalGrid);
            }

        }
        private void OnNeedRedrawGrid(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl tree = o as GridTreeControl;
            if (tree != null)
            {
                tree.InternalGrid.UnloadArrangedCells();
                tree.InternalGrid.InvalidateVisual(true);
            }
        }
        GridTreeControlImpl internalGrid;
        public GridTreeControlImpl InternalGrid
        {
            get
            {
                return internalGrid;
            }

        }

        private bool IsDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(this);
        }

        private void HookGrid()
        {
            HookGrid(false);
        }

        private void HookGrid(bool forceShadowProperties)
        {

            if (this.ctorModel == null)
            {
                this.InternalGrid.UnwireGridEvents();
                this.ctorModel = new GridTreeModel();
            }
            this.InternalGrid.InitializeTreeModel(ctorModel);

            if (ctorSelectedNodes != null)
            {
                InternalGrid.SetSelectedNodeCollection(ctorSelectedNodes);
            }
            // Hook the internal dependency
            GridTreeModel model = ctorModel;
            ctorModel = null;

            this.InternalGrid.Model = model;

            //propagates dependency changes down to the GridTreeControlImpl after
            //template is applied.
            if (this.isVisualStyleChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.CustomVisualStyle = this.CustomVisualStyle;                
                InternalGrid.VisualStyle = this.VisualStyle;
            }
            if (this.isShowRowHeadersChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.ShowRowHeader = this.ShowRowHeader;
            }

            if (this.isShowColumnHeadersChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.ShowColumnHeaders = this.ShowColumnHeaders;
            }

            if (this.isColumnsChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.Columns = this.Columns;
            }
            if (this.isDefaultColumnWidthChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.Model.ColumnWidths.DefaultLineSize = this.DefaultColumnWidth;
            }

            if (IsDesignTime())
                return;

            if (this.isAllowDragChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.AllowDragColumns = this.AllowDragColumns;
            }

            if (this.isFrozenRowsChangedBeforeGridLoaded)
            {
                InternalGrid.FrozenRows = this.FrozenRows;
            }
            if (this.isFooterRowsChangedBeforeGridLoaded)
            {
                InternalGrid.FooterRows = this.FooterRows;
            }
            if (isUnboundRowPositionChangedBeforeGridLoaded)
            {
                InternalGrid.UnboundRowPosition = this.UnboundRowPosition;
            }
            if (isUnboundRowsCountChangedBeforeGridLoaded)
            {
                InternalGrid.UnboundRowsCount = this.UnboundRowsCount;
            }
            if (this.isAllowSortChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.AllowSort = this.AllowSort;
            }
            if (this.isNotifyPropertyChangedSetBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.NotifyPropertyChanges = this.NotifyPropertyChanges;
            }
            if (this.isPercentSizingBehaviorBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.PercentSizingBehavior = this.PercentSizingBehavior;
            }
            if (this.isFreezeExpandColumnChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.FreezeExpandColumn = this.FreezeExpandColumn;
            }
            if (this.isReadOnlyChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.ReadOnly = this.ReadOnly;
            }
            if (this.isSupportRowSizingChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.SupportRowSizing = this.SupportRowSizing;
            }
            if (this.isTrackSelectionOnCollectionChangeBeforeGridLoaded | forceShadowProperties)
            {
                InternalGrid.TrackSelectionOnCollectionChange = this.TrackSelectionOnCollectionChange;
            }
            if (this.isEnableNodeSelectionChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.EnableNodeSelection = this.EnableNodeSelection;
            }
            if (this.isEnableSelectionsChangedBeforeGridLoaded)
            {
                EnableNodeSelection = false;
                InternalGrid.Model.Options.AllowSelection = GridSelectionFlags.None;
            }
            if (this.isEnableHotRowMarkerChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.EnableHotRowMarker = this.EnableHotRowMarker;
            }

            if (this.isShowExpandColumnBordersChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.ShowExpandColumnBorders = this.ShowExpandColumnBorders;
            }
            if (this.isRowHeaderWidthChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.RowHeaderWidth = this.RowHeaderWidth;
            }
            if (this.isAllowAutoSizingNodeColumnChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.AllowAutoSizingNodeColumn = this.AllowAutoSizingNodeColumn;
            }
            if (isLevelStylesChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.LevelStyles = this.LevelStyles;
            }
            if (isSupportsVisualStylesBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.SupportsVisualStyles = this.SupportsVisualStyles;
            }
            if (this.isColumnHeaderStyleChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.ColumnHeaderStyle = this.ColumnHeaderStyle;
            }
            if (this.isHideEmptyChildGlyphsChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.HideEmptyChildGlyphs = this.HideEmptyChildGlyphs;
            }
            if (this.isSupportNodeImagesChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.SupportNodeImages = this.SupportNodeImages;
            }

            if (this.isAutoPopulateColumnsChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.AutoPopulateColumns = this.AutoPopulateColumns;
            }
            if (this.isAllowAutoSizingNodeColumnChangedBeforeGridLoaded || forceShadowProperties)
            {
                InternalGrid.AutoGenerateColumnsInfo = this.AutoGenerateColumnsInfo;
            }
#if !SILVERLIGHT
            if (this.isEnableRenderOptimizationSetBeforeLoaded)
            {
                InternalGrid.EnableRenderOptimization = this.EnableRenderOptimization;
            }
#endif
            if (this.isExpandStateAtStartUpChangedBeforeGridLoaded)
            {
                InternalGrid.ExpandStateAtStartUp = this.ExpandStateAtStartUp;
            }

            if (this.isScrollViewerStyleChangedBeforeGridLoaded && this.PartScrollViewer != null)
            {
                this.PartScrollViewer.Style = this.ScrollViewerStyle; // is obselete
            }

            if (this.isExpandGlyphTypeChangedBeforeGridLoaded)
                InternalGrid.ExpandGlyphType = this.ExpandGlyphType;

            if (this.ModelLoaded != null)
            {
                this.ModelLoaded(this, EventArgs.Empty);
            }

            InternalGrid.DoLoad();
        }

        #endregion

        #region exposed methods and events
        /// <summary>
        /// A cancelable event raised before the expand state of a node changes as a result of the user clicking the expand button.
        /// </summary>
        public event GridTreeNodeCancelEventHandler ExpandStateChanging;

        /// <summary>
        /// A notification event that is raised after a node has been expand or collapsed as the result of the user clicking the expand button.
        /// </summary>
        public event GridTreeNodeEventHandler ExpandStateChanged;

        /// <summary>
        /// Raises the ExpandStateChanging event.
        /// </summary>
        /// <param name="node">The node that was clicked.</param>
        /// <param name="action">The action to be taken.</param>
        /// <returns>True if the action should be completed, false otherwise.</returns>
        public bool OnExpandStateChanging(GridTreeNode node, GridTreeNodeActions action)
        {
            if (ExpandStateChanging != null && isGridLoaded)
            {
                GridTreeNodeCancelEventArgs e = new GridTreeNodeCancelEventArgs(node, action);
                ExpandStateChanging(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// Raises the ExpandStateChanged event.
        /// </summary>
        /// <param name="node">The node that was clicked.</param>
        /// <param name="action">The action that was taken.</param>
        public void OnExpandStateChanged(GridTreeNode node, GridTreeNodeActions action)
        {
            if (ExpandStateChanged != null && isGridLoaded)
            {
                GridTreeNodeEventArgs e = new GridTreeNodeEventArgs(node, action);
                ExpandStateChanged(this, e);
            }
        }

        /// <summary>
        /// Collapses all expanded nodes.
        /// </summary>
        public void CollapseAllNodes()
        {
            if (isGridLoaded)
            {
                InternalGrid.CollapseAllNodes();
            }
        }

        /// <summary>
        /// Collapse all passed-in node as well as child nodes of the passed-in node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        public void CollapseAllNodes(GridTreeNode n)
        {
            if (isGridLoaded)
            {
                OnExpandStateChanging(n, GridTreeNodeActions.Collapsing);
                InternalGrid.CollapseAllNodes(n);
                OnExpandStateChanged(n, GridTreeNodeActions.Collapsed);
            }
        }

        /// <summary>
        /// Expand all nodes.
        /// </summary>
        public void ExpandAllNodes()
        {
            if (isGridLoaded)
            {
                InternalGrid.ExpandAllNodes();
            }
        }

        /// <summary>
        /// Expands the given node and all of its child nodes.
        /// </summary>
        /// <param name="n">The node to be expanded.</param>
        public void ExpandAllNodes(GridTreeNode n)
        {
            if (isGridLoaded)
            {
                OnExpandStateChanging(n, GridTreeNodeActions.Expanding);
                InternalGrid.ExpandAllNodes(n);
                OnExpandStateChanged(n, GridTreeNodeActions.Expanded);
            }
        }

        /// <summary>
        /// Expands the single node.
        /// </summary>
        /// <param name="n">The node to be expanded.</param>
        public void ExpandNode(GridTreeNode n)
        {
            if (isGridLoaded)
            {
                OnExpandStateChanging(n, GridTreeNodeActions.Expanding);
                InternalGrid.ExpandNode(n);
                OnExpandStateChanged(n, GridTreeNodeActions.Expanded);
            }
        }

        /// <summary>
        /// Collapses a single node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        public void CollapseNode(GridTreeNode n)
        {
            if (isGridLoaded)
            {
                OnExpandStateChanging(n, GridTreeNodeActions.Collapsing);
                InternalGrid.CollapseNode(n);
                OnExpandStateChanged(n, GridTreeNodeActions.Collapsed);
            }
        }

        /// <summary>
        /// Expand the GridNode that corresponds to the given grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The rowIndex of the GridNode to be expanded.</param>
        public void ExpandNode(int gridRowIndex)
        {
            if (isGridLoaded)
            {
                InternalGrid.ExpandNode(gridRowIndex);
            }
        }

        /// <summary>
        /// Collapse the GridNode that corresponds to the given grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The rowIndex of the GridNode to be collapsed.</param>
        public void CollapseNode(int gridRowIndex)
        {
            if (isGridLoaded)
            {
                InternalGrid.CollapseNode(gridRowIndex);
            }
        }

        /// <summary>
        /// This method is only called if the Columns collection has not been explicitly populated by the user
        /// before the initial raising of the RequestTreesItems event in OnLoad.
        /// </summary>
        public void AutoPopulateColumnInfo()
        {
            if (isGridLoaded)
            {
                InternalGrid.AutoPopulateColumnInfo();
            }
        }

        /// <summary>
        /// This method clears the GridTreeControl of all nodes and column schema information.
        /// </summary>
        /// <remarks>
        /// If you are using an ItemsSource property (along with possibly a ChildPropertyName, 
        /// ParentPropertyName, or SelfRelationRootValue), then these properties will also
        /// be reset by a call to this Reset method.
        /// </remarks>
        public void Reset()
        {
            if (this.InternalGrid != null)
            {
                if (this.InternalGrid.CurrentCell != null && this.InternalGrid.CurrentCell.IsEditing)
                    this.InternalGrid.CurrentCell.Deactivate();
                this.CollapseAllNodes();
                this.InternalGrid.Nodes.Clear();
                this.InternalGrid.RootNodes.Clear();
                this.InternalGrid.pdc = null;
                this.InternalGrid.ItemPropertyType = null;
                this.Columns.Clear();
                this.ItemsSource = null;
                this.ChildPropertyName = "";
                this.ParentPropertyName = "";

                this.InternalGrid.ResetGrid();
            }
        }

        /// <summary>
        /// Call this method to repopulate a GridTreeControl if the GridTreeControl has been
        /// previously reset to an empty tree by calling the <see cref="GridTreeControl.Reset"/> method.
        /// </summary>
        /// <remarks>
        /// If you are using the RequestTreeItems event to populate your tree, just calling Populate() will
        /// raise the appropriate events. But if you are populating the tree by setting an ItemsSource property
        /// (along with possibly a ChildPropertyName, ParentPropertyName, or SelfRelationRootValue), then you also
        /// must set these properties properly before calling the Populate method.
        /// </remarks>
        public void Populate()
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.PopulateTree();
            }
        }
        #endregion

#if !SILVERLIGHT
        #region ContextMenuStyle
        
        public Style ContextMenuStyle
        {
            get { return (Style)GetValue(ContextMenuStyleProperty); }
            set { SetValue(ContextMenuStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextMenuStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuStyleProperty =
            DependencyProperty.Register("ContextMenuStyle", typeof(Style), typeof(GridTreeControl), new PropertyMetadata(OnContextMenuStyleChanged));

        private static void OnContextMenuStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControl grid = d as GridTreeControl;
            grid.Model.GridContextMenu.Style = (Style)e.NewValue;
        }
        
        #endregion
#endif
        public void Dispose()
        {
#if !SILVERLIGHT
            this.InternalGrid.Dispose(true);
#endif
            GC.SuppressFinalize(this);
        }
    }
}
