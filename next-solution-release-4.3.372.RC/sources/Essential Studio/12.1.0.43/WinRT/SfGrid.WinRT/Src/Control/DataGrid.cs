#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
﻿using System.IO;
using System.Runtime.Serialization;
using Syncfusion.Data.Extensions;
#if !WP
using System.ComponentModel.DataAnnotations;
#endif
using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Text;
#if WinRT
using Syncfusion.UI.Xaml.Utility;
﻿using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Dynamic;
using Syncfusion.Data;
using Syncfusion.Dynamic;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Windows.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Data;
using System.Windows.Markup;
using System.Resources;
#endif

#if WPF
using System.Data;
using System.Dynamic;
using Syncfusion.Dynamic;
﻿using Syncfusion.Windows.Shared;

#endif
using System.Collections;
#if !WP
using System.Dynamic;
using Syncfusion.Dynamic;
#endif
namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(VisualContainer))]
#endif
    [ClassReference(IsReviewed = false)]
    [StyleTypedProperty(Property = "CellStyle", StyleTargetType = typeof(GridCell))]
    [StyleTypedProperty(Property = "RowStyle", StyleTargetType = typeof(VirtualizingCellsControl))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(GridHeaderCellControl))]
    [StyleTypedProperty(Property = "CaptionSummaryRowStyle", StyleTargetType = typeof(CaptionSummaryRowControl))]
    [StyleTypedProperty(Property = "TableSummaryRowStyle", StyleTargetType = typeof(TableSummaryRowControl))]
    [StyleTypedProperty(Property = "GroupSummaryRowStyle", StyleTargetType = typeof(GroupSummaryRowControl))]
    [StyleTypedProperty(Property = "GroupSummaryCellStyle", StyleTargetType = typeof(GridGroupSummaryCell))]
    [StyleTypedProperty(Property = "CaptionSummaryCellStyle", StyleTargetType = typeof(GridCaptionSummaryCell))]
    [StyleTypedProperty(Property = "TableSummaryCellStyle", StyleTargetType = typeof(GridTableSummaryCell))]
    [TemplatePart(Name = "PART_GroupDropArea", Type = typeof(GroupDropArea))]
    [TemplatePart(Name = "PART_VisualContainer", Type = typeof(VisualContainer))]
#if !WP
    [StyleTypedProperty(Property = "FilterPopupStyle", StyleTargetType = typeof(GridFilterControl))]
    public class SfDataGrid : Control, INotifyDependencyPropertyChanged, IDetailsViewNotifier, IDisposable
#else
    public class SfDataGrid : Control, IDisposable
#endif
    {
        #region Fields

        protected VisualContainer container;
        private GridCellRendererCollection cellRenderers = null;
        private GroupDropArea groupDropArea;
        internal bool IsColumnSizerInitialized;
        internal int headerLineCount = 1;
        private bool isGridLoaded;
        private bool suspendForColumnPopulation;
        internal bool inRowHeaderChange;
        private bool isViewPropertiesEnsured = false;
        internal bool isselectedindexchanged = false;
        internal bool isselecteditemchanged = false;
#if !WP
        internal DetailsViewManager DetailsViewManager;
#else
        internal bool suspendZooming;
#endif
        internal ValidationHelper Validations;
#if !WinRT
        internal bool suspendForColumnMove;
        private int oldIndexForMove;
#endif


#if WinRT
        private static double headerRowHeight = 45d;
        private static double rowHeight = 45d;
#elif WP 
        static double headerRowHeight = 75d;
        static double rowHeight = 75d;  
#else
        static double headerRowHeight = 24d;
        static double rowHeight = 24d;
#endif

        #endregion

        #region Internal Property

        internal string GroupCaptionConstant = "{ColumnName} : {Key} - {ItemsCount} Items";

        internal bool HasUnboundColumns = false;

        internal int HeaderLineCount
        {
            get { return headerLineCount; }
        }

        internal RowGenerator RowGenerator = null;

#if !WP
        [Cloneable(false)]
#endif
        internal GridColumnResizingController GridColumnResizingController { get; set; }
#if !WP
        [Cloneable(false)]
#endif
        internal GridModel GridModel { get; set; }
#if !WP
        [Cloneable(false)]
#endif
        internal GridColumnSizer GridColumnSizer { get; set; }

        internal bool hasCaptionSummaryRowStyle;
        internal bool hasGroupSummaryRowStyle;
        internal bool hasTableSummaryRowStyle;

        internal bool hasCaptionSummaryRowStyleSelector;
        internal bool hasGroupSummaryRowStyleSelector;
        internal bool hasTableSummaryRowStyleSelector;

        internal bool hasGroupSummaryCellStyle;
        internal bool hasGroupSummaryCellStyleSelector;

        internal bool hasCaptionSummaryCellStyle;
        internal bool hasCaptionSummaryCellStyleSelector;

        internal bool hasTableSummaryCellStyle;
        internal bool hasTableSummaryCellStyleSelector;

        internal bool hasCellTemplateSelector;
        internal bool hasCellTemplate;
        internal bool hasCellStyleSelector;
        internal bool hasCellStyle;
        internal bool hasRowStyleSelector;
        internal bool hasRowStyle;
        internal bool hasAlternatingRowStyle;
        internal bool hasAlternatingRowStyleSelector;

        #endregion

        #region Public Property
#if !WP
        [Cloneable(false)]
#endif
        public ICollectionViewAdv View
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public GridColumnDragDropController GridColumnDragDropController { get; set; }

        public bool ShowBusyIndicator { get; set; }

        internal VisualContainer VisualContainer
        {
            get { return container; }
        }

        internal GroupDropArea GroupDropArea
        {
            get { return groupDropArea; }
        }

        /// <summary>
        /// Property Which will return the collection of Renderers.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridCellRendererCollection CellRenderers
        {
            get { return cellRenderers; }
        }

#if !WP
        [Cloneable(false)]
#endif
        public IGridSelectionController SelectionController { get; set; }

        #endregion

        #region Dependency property

        public GridValidationMode GridValidationMode
        {
            get { return (GridValidationMode)GetValue(GridValidationModeProperty); }
            set { SetValue(GridValidationModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridValidationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridValidationModeProperty =
            GridDependencyProperty.Register("GridValidationMode", typeof(GridValidationMode), typeof(SfDataGrid), new GridPropertyMetadata(GridValidationMode.None, OnGridValidationPropertyChanded));
#if !WP
        [Cloneable(false)]
#endif
        public int FrozenColumnCount
        {
            get { return (int)GetValue(FrozenColumnCountProperty); }
            set { SetValue(FrozenColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnCountProperty =
            GridDependencyProperty.Register("FrozenColumnCount", typeof(int), typeof(SfDataGrid), new GridPropertyMetadata(0, OnFrozenColumnCountPropertyChanged));

        /// <summary>
        /// Get or Set the ItemsSource for SfDataGrid
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for ItemsSource property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ItemsSourceProperty =
            GridDependencyProperty.Register("ItemsSource", typeof(object), typeof(SfDataGrid), new GridPropertyMetadata(null, OnItemsSourceChanged));

        public Type SourceType
        {
            get { return (Type)GetValue(SourceTypeProperty); }
            set { SetValue(SourceTypeProperty, value); }
        }

        public static readonly DependencyProperty SourceTypeProperty = GridDependencyProperty.Register("SourceType", typeof(Type), typeof(SfDataGrid), new GridPropertyMetadata(null));

        public bool UsePLINQ
        {
            get { return (bool)GetValue(UsePLINQProperty); }
            set { SetValue(UsePLINQProperty, value); }
        }

        public static readonly DependencyProperty UsePLINQProperty =
            GridDependencyProperty.Register("UsePLINQ", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a thickness for CurrentCell border.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Thickness CurrentCellBorderThickness
        {
            get { return (Thickness)GetValue(CurrentCellBorderThicknessProperty); }
            set { SetValue(CurrentCellBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Dependency Registration of CurrentCellBorder thickness
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CurrentCellBorderThicknessProperty =
            GridDependencyProperty.Register("CurrentCellBorderThickness", typeof(Thickness), typeof(SfDataGrid), new GridPropertyMetadata(new Thickness(2)));

        /// <summary>
        /// Gets or sets Brush for CurrentCell Border
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush CurrentCellBorderBrush
        {
            get { return (Brush)GetValue(CurrentCellBorderBrushProperty); }
            set { SetValue(CurrentCellBorderBrushProperty, value); }
        }

        /// <summary>
        /// Dependency Registration of CurrentCell border brush.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CurrentCellBorderBrushProperty =
            GridDependencyProperty.Register("CurrentCellBorderBrush", typeof(Brush), typeof(SfDataGrid), new GridPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets sorting action by single or double click.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public SortClickAction SortClickAction
        {
            get { return (SortClickAction)GetValue(SortClickActionProperty); }
            set { SetValue(SortClickActionProperty, value); }
        }

        public static readonly DependencyProperty SortClickActionProperty =
            GridDependencyProperty.Register("SortClickAction", typeof(SortClickAction), typeof(SfDataGrid), new GridPropertyMetadata(SortClickAction.SingleClick, null));

        /// <summary>
        /// Gets or sets a value indicating whether Grid can able to be Tristate in sorting.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowTriStateSorting
        {
            get { return (bool)this.GetValue(AllowTriStateSortingProperty); }
            set { this.SetValue(AllowTriStateSortingProperty, value); }
        }

        public static readonly DependencyProperty AllowTriStateSortingProperty =
            GridDependencyProperty.Register("AllowTriStateSorting", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, null));

        /// <summary>
        /// Gets or sets a value indicating whether Grid can show order number of SortColumnDescriptions.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ShowSortNumbers
        {
            get { return (bool)this.GetValue(SortNumberProperty); }
            set { this.SetValue(SortNumberProperty, value); }
        }

        public static readonly DependencyProperty SortNumberProperty =
            GridDependencyProperty.Register("ShowSortNumbers", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnSortNumberPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether Grid can sort or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowSorting
        {
            get { return (bool)this.GetValue(AllowSortingProperty); }
            set { this.SetValue(AllowSortingProperty, value); }
        }

        public static readonly DependencyProperty AllowSortingProperty =
            GridDependencyProperty.Register("AllowSorting", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(true, OnAllowSortChanged));

        /// <summary>
        /// Gets the sort columns.
        /// </summary>
        /// <value>The sort columns.</value>
#if !WP
        [Cloneable(false)]
#endif
        public SortColumnDescriptions SortColumnDescriptions
        {
            get { return (SortColumnDescriptions)GetValue(SortColumnDescriptionsProperty); }
        }

        public static readonly DependencyProperty SortColumnDescriptionsProperty =
            GridDependencyProperty.Register("SortColumnDescriptions", typeof(ObservableCollection<SortColumnDescription>), typeof(SfDataGrid), new GridPropertyMetadata(new SortColumnDescriptions()));

#if !WP
        [Cloneable(false)]
#endif
        public StackedHeaderRows StackedHeaderRows
        {
            get { return (StackedHeaderRows)GetValue(StackedHeaderRowsProperty); }
            set { SetValue(StackedHeaderRowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StackedHeaders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StackedHeaderRowsProperty =
            GridDependencyProperty.Register("StackedHeaderRows", typeof(StackedHeaderRows), typeof(SfDataGrid), new GridPropertyMetadata(new StackedHeaderRows(), OnStackedHeadersChanged));

        /// <summary>
        /// Get the Comparers for Custom Sorting
        /// </summary>
        public SortComparers SortComparers
        {
            get { return (SortComparers)GetValue(SortComparersProperty); }
        }

        public static readonly DependencyProperty SortComparersProperty =
            GridDependencyProperty.Register("SortComparers", typeof(SortComparers), typeof(SfDataGrid),new GridPropertyMetadata(new SortComparers()));

        /// <summary>
        /// Property which is get or set the Grid Columns
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public Columns Columns
        {
            get { return (Columns)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Dependency registration of GridColumns property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ColumnsProperty =
            GridDependencyProperty.Register("Columns", typeof(Columns), typeof(SfDataGrid), new GridPropertyMetadata(new Columns(), OnColumnsPropertyChanged));

        private static void OnColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (e.OldValue != e.NewValue)
            {
                if (e.OldValue is Columns)
                    (e.OldValue as INotifyCollectionChanged).CollectionChanged -= grid.OnGridColumnCollectionChanged;
                if (e.NewValue is Columns)
                    (e.NewValue as INotifyCollectionChanged).CollectionChanged += grid.OnGridColumnCollectionChanged;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether populate columns auto matically or not
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AutoGenerateColumns
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            GridDependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(true, OnAutoGenerateColumnsChanged));

        /// <summary>
        /// Gets or sets whether columns should be created automatically for all fields in the underlying data source
        /// </summary>
        public AutoGenerateColumnsMode AutoGenerateColumnsMode
        {
            get { return (AutoGenerateColumnsMode)GetValue(AutoGenerateColumnsModeProperty); }
            set { SetValue(AutoGenerateColumnsModeProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateColumnsModeProperty =
            GridDependencyProperty.Register("AutoGenerateColumnsMode", typeof(AutoGenerateColumnsMode), typeof(SfDataGrid), new GridPropertyMetadata(AutoGenerateColumnsMode.Reset, OnAutoGenerateColumnsModeChanged));

        /// <summary>
        /// Gets or Set the value for SelectedItem
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectedItem
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectedItemProperty =
#if WPF
 DependencyProperty.Register("SelectedItem", typeof(object), typeof(SfDataGrid),
                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));
#else
 GridDependencyProperty.Register("SelectedItem", typeof(object), typeof(SfDataGrid),
                                        new GridPropertyMetadata(null, OnSelectedItemChanged));
#endif
        /// <summary>
        /// Gets the value of Selected items in SfDataGrid.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
        }

        /// <summary>
        /// Dependency registration of SelectedItems
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectedItemsProperty =
            GridDependencyProperty.Register("SelectedItems", typeof(ObservableCollection<object>), typeof(SfDataGrid), new GridPropertyMetadata(new ObservableCollection<object>(), OnSelectedItemsPropertyChanged));

        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null || !grid.isGridLoaded)
                return;
            if (e.OldValue is INotifyCollectionChanged)
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= grid.OnSelectedItemsChanged;
            if (e.NewValue is INotifyCollectionChanged)
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += grid.OnSelectedItemsChanged;
        }

        /// <summary>
        /// Gets or sets the Selection Mode of SfDataGrid
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridSelectionMode SelectionMode
        {
            get { return (GridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Dependency registration for Selection Mode property.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectionModeProperty =
            GridDependencyProperty.Register("SelectionMode", typeof(GridSelectionMode), typeof(SfDataGrid), new GridPropertyMetadata(GridSelectionMode.Single, OnSelectionModeChanged));

        /// <summary>
        /// Gets or sets a value indicating whether Selection should present in PointerPressed or Pointer Released.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowSelectionOnPointerPressed
        {
            get { return (bool)GetValue(AllowSelectionOnPointerPressedProperty); }
            set { SetValue(AllowSelectionOnPointerPressedProperty, value); }
        }

        /// <summary>
        /// Depecdency registration of AllowSelectionOnPointerPressed
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowSelectionOnPointerPressedProperty =
            GridDependencyProperty.Register("AllowSelectionOnPointerPressed", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));

        /// <summary>
        /// Gets or sets value for SelectionBrush
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush RowSelectionBrush
        {
            get { return (Brush)GetValue(RowSelectionBrushProperty); }
            set { SetValue(RowSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for SelectionBrush
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowSelectionBrushProperty =
            GridDependencyProperty.Register("RowSelectionBrush", typeof(Brush), typeof(SfDataGrid), new GridPropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 128, 128, 128)), OnRowSelectionBackgroundChanged));

        /// <summary>
        /// Gets or sets value for GroupCaptionRowSelectionBrush
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush GroupRowSelectionBrush
        {
            get { return (Brush)GetValue(GroupRowSelectionBrushProperty); }
            set { SetValue(GroupRowSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for GroupCaptionRowSelectionBrush
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty GroupRowSelectionBrushProperty =
            GridDependencyProperty.Register("GroupRowSelectionBrush", typeof(Brush), typeof(SfDataGrid), new GridPropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 120, 120, 120)), OnGroupRowSelectionBrushChanged));

        /// <summary>
        /// Gets or sets the SelectedIndex property. 
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectedIndex
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectedIndexProperty =
            GridDependencyProperty.Register("SelectedIndex", typeof(int), typeof(SfDataGrid), new GridPropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// Allows to set the custom style for the SfDataGrid's Row.
        /// </summary>
        /// <value>Style</value>
        /// <remarks>Define the custom style and set as RowStyle</remarks>
        public Style RowStyle
        {
            get { return (Style)GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        public static readonly DependencyProperty RowStyleProperty =
            GridDependencyProperty.Register("RowStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnRowStyleChanged));

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Allows to set the Custom Style for SfDataGrid's Row and Selected Row Style .
        /// </summary>
        /// <value>StyleSelector</value>
        /// <remarks>Style which is to be choosed based on some condition given by the user and it is applied to the Virtualizing cells control</remarks>
        public StyleSelector RowStyleSelector
        {
            get { return (StyleSelector)GetValue(RowStyleSelectorProperty); }
            set { SetValue(RowStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty RowStyleSelectorProperty =
            GridDependencyProperty.Register("RowStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnRowStyleSelectorChanged));

#endif

        public static readonly DependencyProperty AlternationCountProperty =
            DependencyProperty.Register("AlternationCount", typeof(int), typeof(SfDataGrid), new PropertyMetadata(2, OnAlternationCountChanged));

        private static void OnAlternationCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null || !grid.isGridLoaded)
                return;
            grid.UpdateRowStyle();
        }

        public int AlternationCount
        {
            get { return (int)GetValue(AlternationCountProperty); }
            set { SetValue(AlternationCountProperty, value); }
        }

        public static readonly DependencyProperty AlternatingRowStyleProperty =
            DependencyProperty.Register("AlternatingRowStyle", typeof(Style), typeof(SfDataGrid), new PropertyMetadata(null, OnAlternatingRowStyleChanged));

        private static void OnAlternatingRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasAlternatingRowStyle = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateRowStyle();
        }

        public Style AlternatingRowStyle
        {
            get { return (Style)GetValue(AlternatingRowStyleProperty); }
            set { SetValue(AlternatingRowStyleProperty, value); }
        }

#if !SILVERLIGHT && !WP

        public static readonly DependencyProperty AlternatingRowStyleSelectorProperty =
            DependencyProperty.Register("AlternatingRowStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new PropertyMetadata(null, OnAlternatingRowStyleSelectorChanged));

        private static void OnAlternatingRowStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasAlternatingRowStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateRowStyle();
        }

        public StyleSelector AlternatingRowStyleSelector
        {
            get { return (StyleSelector)GetValue(AlternatingRowStyleSelectorProperty); }
            set { SetValue(AlternatingRowStyleSelectorProperty, value); }
        }

#endif

        /// <summary>
        /// Gets or sets the style that is used when rendering the data grid cells.
        /// </summary>
        /// <value></value>
        /// <remarks>Whether defined as an inline style or as a resource, the Style defines the appearance of cells in the SfDataGrid, 
        /// and should specify a TargetType of GridCell. You typically specify setters for individual properties, 
        /// and might also use a setter for the Template property if you wanted to change the composition of elements. 
        /// For information that can help you decide whether to define styles inline or as resources, see Inline Styles and Templates.</remarks>
        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        public static readonly DependencyProperty CellStyleProperty =
            GridDependencyProperty.Register("CellStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCellStyleChanged));

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets or sets the StyleSelector for the GridCell.
        /// </summary>
        /// <value>StyleSelector</value>
        /// <remarks>Style which is to be choosed based on some condition given by the user and it is applied to GridCell </remarks>
        public StyleSelector CellStyleSelector
        {
            get { return (StyleSelector)GetValue(CellStyleSelectorProperty); }
            set { SetValue(CellStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty CellStyleSelectorProperty =
            GridDependencyProperty.Register("CellStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCellStyleSelectorChanged));

        /// <summary>
        /// Gets or sets the Cell Template Selector for the GridCell.
        /// </summary>
        /// <value>DataTemplateSelector</value>
        /// <remarks>This Describes the cell Template based on Some condition which is given by the user and is to applied on the GridCell. </remarks>
        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty CellTemplateSelectorProperty =
            GridDependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCellTemplateSelectorChanged));
#endif

        /// <summary>
        /// Gets or sets the style that is used when rendering the row headers.
        /// </summary>
        /// <value>Style</value>
        /// <remarks>Whether defined as an inline style or as a resource, the Style defines the appearance of Header cells in the SfDataGrid, 
        /// and should specify a TargetType of GridDataHeaderCellControl. You typically specify setters for individual properties, 
        /// and might also use a setter for the Template property if you wanted to change the composition of elements. 
        /// For information that can help you decide whether to define styles inline or as resources, see Inline Styles and Templates.</remarks>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderStyleProperty =
            GridDependencyProperty.Register("HeaderStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnHeaderStyleChanged));

        /// <summary>
        /// Gets or sets a template used to display the header of the Columns.
        /// </summary>
        /// <value>DataTemplate</value>
        /// <remarks>This is a dependency property</remarks>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            GridDependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SfDataGrid), new GridPropertyMetadata(null, OnHeaderTemplateChanged));

        /// <summary>
        /// Gets or sets the height of Header Row of DataGrid.
        /// </summary>
        /// <value>Double</value>
        /// <remarks>This is a dependency property, it set the Height of Header row of GirdDataControl.</remarks>
        public double HeaderRowHeight
        {
            get { return (double)GetValue(HeaderRowHeightProperty); }
            set { SetValue(HeaderRowHeightProperty, value); }
        }

        public static readonly DependencyProperty HeaderRowHeightProperty =
            GridDependencyProperty.Register("HeaderRowHeight", typeof(double), typeof(SfDataGrid), new GridPropertyMetadata(SfDataGrid.headerRowHeight, OnHeaderRowHeightChanged));

        /// <summary>
        /// Gets or sets the height of all the Rows in SfDataGrid.
        /// </summary>
        /// <value>Double</value>
        /// <remarks>This is a dependency property, it set the Height of all the rows in GirdDataControl.</remarks>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            GridDependencyProperty.Register("RowHeight", typeof(double), typeof(SfDataGrid), new GridPropertyMetadata(SfDataGrid.rowHeight, OnRowHeightChanged));

        /// <summary>
        /// Gets the GroupColumn values which is needed for Grouping
        /// </summary>
        /// <value></value>
        /// <remarks>Collection of GroupColumnDescriptions</remarks>
#if !WP
        [Cloneable(false)]
#endif
        public GroupColumnDescriptions GroupColumnDescriptions
        {
            get { return (GroupColumnDescriptions)GetValue(GroupColumnDescriptionsProperty); }
        }

        /// <summary>
        /// Dependency Registration for GroupColumnDescriptions
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty GroupColumnDescriptionsProperty =
            GridDependencyProperty.Register("GroupColumnDescriptions", typeof(GroupColumnDescriptions), typeof(SfDataGrid), new GridPropertyMetadata(null));

        /// <summary>
        /// Gets the values of Summary Rows which is needed for GroupSummarys
        /// </summary>
        /// <value></value>
        /// <remarks>Collection of ISummaryRow</remarks>
#if !WP
        [Cloneable(false)]
#endif
        public ObservableCollection<GridSummaryRow> GroupSummaryRows
        {
            get { return (ObservableCollection<GridSummaryRow>)GetValue(GroupSummaryRowsProperty); }
            set { SetValue(GroupSummaryRowsProperty, value); }
        }

        /// <summary>
        /// Dependency registration of GroupSummaryRows
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty GroupSummaryRowsProperty =
            GridDependencyProperty.Register("GroupSummaryRows", typeof(ObservableCollection<GridSummaryRow>), typeof(SfDataGrid), new GridPropertyMetadata(null, OnGroupSummaryRowsPropertyChanged));

        /// <summary>
        /// Gets or sets value Summary Row which is need for GroupCaption Summary
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridSummaryRow CaptionSummaryRow
        {
            get { return (GridSummaryRow)GetValue(CaptionSummaryRowProperty); }
            set { SetValue(CaptionSummaryRowProperty, value); }
        }

        /// <summary>
        /// Dependency Registration of Caption Summary Row.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CaptionSummaryRowProperty =
            GridDependencyProperty.Register("CaptionSummaryRow", typeof(GridSummaryRow), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCaptionSummaryRowChanged));

        /// <summary>
        /// Gets the Summary Rows which is need for Grid Table Summary
        /// </summary>
        /// <value></value>
        /// <remarks>Collection of ISummaryRow</remarks>
#if !WP
        [Cloneable(false)]
#endif
        public ObservableCollection<GridSummaryRow> TableSummaryRows
        {
            get { return (ObservableCollection<GridSummaryRow>)GetValue(TableSummaryRowsProperty); }
            set { SetValue(TableSummaryRowsProperty, value); }
        }

        /// <summary>
        /// Dependency Registration of TableSummaryRows
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty TableSummaryRowsProperty =
            GridDependencyProperty.Register("TableSummaryRows", typeof(ObservableCollection<GridSummaryRow>), typeof(SfDataGrid), new GridPropertyMetadata(null, OnTableSummaryRowsPropertyChanged));

        /// <summary>
        /// Gets or sets Comparer object for Grouped columns sorting.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IComparer<Group> SummaryGroupComparer
        {
            get { return (IComparer<Group>)GetValue(SummaryGroupComparerProperty); }
            set { SetValue(SummaryGroupComparerProperty, value); }
        }

        /// <summary>
        /// Dependency Registration of SummaryGroupComparer
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SummaryGroupComparerProperty =
            GridDependencyProperty.Register("SummaryGroupComparer", typeof(IComparer<Group>), typeof(SfDataGrid), new GridPropertyMetadata(null, OnSummaryGroupComparerChanged));

        /// <summary>
        /// Gets or sets Width according to the Sizer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WP
        [Cloneable(false)]
#endif
        public GridLengthUnitType ColumnSizer
        {
            get { return (GridLengthUnitType)this.GetValue(ColumnSizerProperty); }
            set { this.SetValue(ColumnSizerProperty, value); }
        }

        public static readonly DependencyProperty ColumnSizerProperty =
            GridDependencyProperty.Register("ColumnSizer", typeof(GridLengthUnitType), typeof(SfDataGrid), new GridPropertyMetadata(GridLengthUnitType.None, OnColumnSizerChanged));

        public bool ShowColumnWhenGrouped
        {
            get { return (bool)GetValue(ShowColumnWhenGroupedProperty); }
            set { SetValue(ShowColumnWhenGroupedProperty, value); }
        }

        public static readonly DependencyProperty ShowColumnWhenGroupedProperty =
            GridDependencyProperty.Register("ShowColumnWhenGrouped", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(true));

        public bool AllowFrozenGroupHeaders
        {
            get { return (bool)GetValue(AllowFrozenGroupHeadersProperty); }
            set { SetValue(AllowFrozenGroupHeadersProperty, value); }
        }

        public static readonly DependencyProperty AllowFrozenGroupHeadersProperty =
            GridDependencyProperty.Register("AllowFrozenGroupHeaders", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAllowFixedGroupCaptionsChanged));

        public Style CaptionSummaryRowStyle
        {
            get { return (Style)GetValue(CaptionSummaryRowStyleProperty); }
            set { SetValue(CaptionSummaryRowStyleProperty, value); }
        }

        public static readonly DependencyProperty CaptionSummaryRowStyleProperty =
            GridDependencyProperty.Register("CaptionSummaryRowStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCaptionSummaryRowStyleChanged));

        public Style GroupSummaryRowStyle
        {
            get { return (Style)GetValue(GroupSummaryRowStyleProperty); }
            set { SetValue(GroupSummaryRowStyleProperty, value); }
        }

        public static readonly DependencyProperty GroupSummaryRowStyleProperty =
            GridDependencyProperty.Register("GroupSummaryRowStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnGroupSummaryRowStyleChanged));

        public Style TableSummaryRowStyle
        {
            get { return (Style)GetValue(TableSummaryRowStyleProperty); }
            set { SetValue(TableSummaryRowStyleProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryRowStyleProperty =
            GridDependencyProperty.Register("TableSummaryRowStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnTableSummaryRowStyleChanged));

#if !SILVERLIGHT && !WP
        public StyleSelector CaptionSummaryRowStyleSelector
        {
            get { return (StyleSelector)GetValue(CaptionSummaryRowStyleSelectorProperty); }
            set { SetValue(CaptionSummaryRowStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty CaptionSummaryRowStyleSelectorProperty =
            GridDependencyProperty.Register("CaptionSummaryRowStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCaptionSummaryRowStyleSelectorChanged));

        public StyleSelector GroupSummaryRowStyleSelector
        {
            get { return (StyleSelector)GetValue(GroupSummaryRowStyleSelectorProperty); }
            set { SetValue(GroupSummaryRowStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty GroupSummaryRowStyleSelectorProperty =
            GridDependencyProperty.Register("GroupSummaryRowStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnGroupSummaryRowStyleSelectorChanged));

        public StyleSelector TableSummaryRowStyleSelector
        {
            get { return (StyleSelector)GetValue(TableSummaryRowStyleSelectorProperty); }
            set { SetValue(TableSummaryRowStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryRowStyleSelectorProperty =
            GridDependencyProperty.Register("TableSummaryRowStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnTableSummaryRowStyleSelectorChanged));

        public StyleSelector GroupSummaryCellStyleSelector
        {
            get { return (StyleSelector)GetValue(GroupSummaryCellStyleSelectorProperty); }
            set { SetValue(GroupSummaryCellStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty GroupSummaryCellStyleSelectorProperty =
            GridDependencyProperty.Register("GroupSummaryCellStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnGroupSummaryCellStyleSelectorChanged));

        public StyleSelector CaptionSummaryCellStyleSelector
        {
            get { return (StyleSelector)GetValue(CaptionSummaryCellStyleSelectorProperty); }
            set { SetValue(CaptionSummaryCellStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty CaptionSummaryCellStyleSelectorProperty =
            GridDependencyProperty.Register("CaptionSummaryCellStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCaptionSummaryCellStyleSelectorChanged));

        public StyleSelector TableSummaryCellStyleSelector
        {
            get { return (StyleSelector)GetValue(TableSummaryCellStyleSelectorProperty); }
            set { SetValue(TableSummaryCellStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryCellStyleSelectorProperty =
            GridDependencyProperty.Register("TableSummaryCellStyleSelector", typeof(StyleSelector), typeof(SfDataGrid), new GridPropertyMetadata(null, OnTableSummaryCellStyleSelectorChanged));

#endif
        public bool IsDynamicItemsSource
        {
            get { return (bool)GetValue(IsDynamicItemsSourceProperty); }
            set { SetValue(IsDynamicItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty IsDynamicItemsSourceProperty =
            GridDependencyProperty.Register("IsDynamicItemsSource", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));


        public int DataFetchSize
        {
            get { return (int)GetValue(DataFetchSizeProperty); }
            set { SetValue(DataFetchSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataFetchSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataFetchSizeProperty =
            GridDependencyProperty.Register("DataFetchSize", typeof(int), typeof(SfDataGrid), new GridPropertyMetadata(5));

        public Style GroupSummaryCellStyle
        {
            get { return (Style)GetValue(GroupSummaryCellStyleProperty); }
            set { SetValue(GroupSummaryCellStyleProperty, value); }
        }

        public static readonly DependencyProperty GroupSummaryCellStyleProperty =
            GridDependencyProperty.Register("GroupSummaryCellStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnGroupSummaryCellStyleChanged));

        public Style CaptionSummaryCellStyle
        {
            get { return (Style)GetValue(CaptionSummaryCellStyleProperty); }
            set { SetValue(CaptionSummaryCellStyleProperty, value); }
        }

        public static readonly DependencyProperty CaptionSummaryCellStyleProperty =
            GridDependencyProperty.Register("CaptionSummaryCellStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnCaptionSummaryCellStyleChanged));

        public Style TableSummaryCellStyle
        {
            get { return (Style)GetValue(TableSummaryCellStyleProperty); }
            set { SetValue(TableSummaryCellStyleProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryCellStyleProperty =
            GridDependencyProperty.Register("TableSummaryCellStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null, OnTableSummaryCellStyleChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupDropArea is true / false.
        /// </summary>
        /// <value><c>true</c> if [show group drop area]; otherwise, <c>false</c>.</value>
#if !WP
        [Cloneable(false)]
#endif
        public bool ShowGroupDropArea
        {
            get { return (bool)GetValue(ShowGroupDropAreaProperty); }
            set { SetValue(ShowGroupDropAreaProperty, value); }
        }

        public static readonly DependencyProperty ShowGroupDropAreaProperty =
            GridDependencyProperty.Register("ShowGroupDropArea", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether AllowResizingColumns is true / false.
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        public bool AllowResizingColumns
        {
            get { return (bool)GetValue(AllowResizingColumnsProperty); }
            set { SetValue(AllowResizingColumnsProperty, value); }
        }

        public static readonly DependencyProperty AllowResizingColumnsProperty =
            GridDependencyProperty.Register("AllowResizingColumns", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAllowResisizingColumnsChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [allow resizing hidden columns].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow resizing hidden columns]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowResizingHiddenColumns
        {
            get { return (bool)GetValue(AllowResizingHiddenColumnsProperty); }
            set { SetValue(AllowResizingHiddenColumnsProperty, value); }
        }

        /// <summary>
        /// The allow resizing hidden columns property
        /// </summary>
        public static readonly DependencyProperty AllowResizingHiddenColumnsProperty =
            GridDependencyProperty.Register("AllowResizingHiddenColumns", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAllowResisizingHiddenColumnsChanged));

        /// <summary>
        /// Gets or sets a value indicating whether RowHeader should be Visible or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ShowRowHeader
        {
            get { return (bool)GetValue(ShowRowHeaderProperty); }
            set { SetValue(ShowRowHeaderProperty, value); }
        }

        public static readonly DependencyProperty ShowRowHeaderProperty =
            GridDependencyProperty.Register("ShowRowHeader", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnShowRowHeaderChanged));

        /// <summary>
        /// Gets or sets RowHeaderWidth.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderWidthProperty =
            GridDependencyProperty.Register("RowHeaderWidth", typeof(double), typeof(SfDataGrid), new GridPropertyMetadata
                (
#if !WinRT
24d
#else
45d
#endif
, OnRowHeaderWidthChanged));


#if !WP
        public GridCopyPasteOption GridCopyPasteOption
        {
            get { return (GridCopyPasteOption)GetValue(GridCopyPasteOptionProperty); }
            set { SetValue(GridCopyPasteOptionProperty, value); }
        }

        public static readonly DependencyProperty GridCopyPasteOptionProperty =
            GridDependencyProperty.Register("GridCopyPasteOption", typeof(GridCopyPasteOption), typeof(SfDataGrid), new GridPropertyMetadata(GridCopyPasteOption.CopyData | GridCopyPasteOption.PasteData));

        [Cloneable(false)]
        public IGridCopyPaste GridCopyPaste
        {
            get { return (IGridCopyPaste)GetValue(GridCopyPasteProperty); }
            set { SetValue(GridCopyPasteProperty, value); }
        }

        public static readonly DependencyProperty GridCopyPasteProperty =
            GridDependencyProperty.Register("GridCopyPaste", typeof(IGridCopyPaste), typeof(SfDataGrid), new GridPropertyMetadata(null));
#endif
#if !WinRT
        [TypeConverter(typeof(GridSummaryFormatConverter))]
#endif
        public string GroupCaptionTextFormat
        {
            get { return (string)GetValue(GroupCaptionTextFormatProperty); }
            set
            {
#if !WPF
                if (value != null)
                {
                    var formattedValue = value.SummaryFormatedString();
                    SetValue(GroupCaptionTextFormatProperty, formattedValue);
                }
                else
                    SetValue(GroupCaptionTextFormatProperty, null);
#else
                SetValue(GroupCaptionTextFormatProperty, value);
#endif
            }
        }

        public static readonly DependencyProperty GroupCaptionTextFormatProperty =
            GridDependencyProperty.Register("GroupCaptionTextFormat", typeof(string), typeof(SfDataGrid), new GridPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether [allow drag columns].
        /// </summary>
        /// <value><c>true</c> if [allow drag columns]; otherwise, <c>false</c>.</value>
        public bool AllowDraggingColumns
        {
            get { return (bool)GetValue(AllowDraggingColumnsProperty); }
            set { SetValue(AllowDraggingColumnsProperty, value); }
        }

        public static readonly DependencyProperty AllowDraggingColumnsProperty =
            GridDependencyProperty.Register("AllowDraggingColumns", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating the visibility of group drop area
        /// </summary>
        public bool IsGroupDropAreaExpanded
        {
            get { return (bool)GetValue(IsGroupDropAreaExpandedProperty); }
            set { SetValue(IsGroupDropAreaExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupDropAreaExpandedProperty =
            GridDependencyProperty.Register("IsGroupDropAreaExpanded", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, IsGroupDropAreaExpandedPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [allow group].
        /// </summary>
        /// <value><c>true</c> if [allow group]; otherwise, <c>false</c>.</value>
        public bool AllowGrouping
        {
            get { return (bool)GetValue(AllowGroupingProperty); }
            set { SetValue(AllowGroupingProperty, value); }
        }

        public static readonly DependencyProperty AllowGroupingProperty =
            GridDependencyProperty.Register("AllowGrouping", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the group drop area text.
        /// </summary>
        /// <value>The group drop area text.</value>
        public string GroupDropAreaText
        {
            get { return (string)GetValue(GroupDropAreaTextProperty); }
            set { SetValue(GroupDropAreaTextProperty, value); }
        }

#if WP
        public static readonly DependencyProperty GroupDropAreaTextProperty =
            GridDependencyProperty.Register("GroupDropAreaText", typeof(string), typeof(SfDataGrid), new GridPropertyMetadata("Drag To Group", OnGroupDropAreaTextChanged));
#else
        public static readonly DependencyProperty GroupDropAreaTextProperty =
            GridDependencyProperty.Register("GroupDropAreaText", typeof(string), typeof(SfDataGrid), new GridPropertyMetadata(GridResourceWrapper.GroupDropAreaText, OnGroupDropAreaTextChanged));
#endif
        private static void OnGroupDropAreaTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.groupDropArea != null)
                grid.groupDropArea.GroupDropAreaText = grid.GroupDropAreaText;
        }

        public LiveDataUpdateMode LiveDataUpdateMode
        {
            get { return (LiveDataUpdateMode)GetValue(LiveDataUpdateModeProperty); }
            set { SetValue(LiveDataUpdateModeProperty, value); }
        }

        public static readonly DependencyProperty LiveDataUpdateModeProperty =
            GridDependencyProperty.Register("LiveDataUpdateMode", typeof(LiveDataUpdateMode), typeof(SfDataGrid), new GridPropertyMetadata(LiveDataUpdateMode.Default, OnLiveDataUpdateModePropertyChanged));

        public bool AutoExpandGroups
        {
            get { return (bool)GetValue(AutoExpandGroupsProperty); }
            set { SetValue(AutoExpandGroupsProperty, value); }
        }

        public static readonly DependencyProperty AutoExpandGroupsProperty =
            GridDependencyProperty.Register("AutoExpandGroups", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAutoExpandGroupsChanged));

#if !WP

        /// <summary>
        /// Gets or Sets a value indicating what Triggers will cause cells to enter Edit Mode
        /// </summary>
        public EditTrigger EditTrigger
        {
            get
            {
                return (EditTrigger)GetValue(EditTriggerProperty);
            }
            set
            {
                SetValue(EditTriggerProperty, value);
            }
        }

        /// <summary>
        /// Dependency Registration for EditTrigger property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty EditTriggerProperty =
           GridDependencyProperty.Register("EditTrigger", typeof(EditTrigger), typeof(SfDataGrid), new GridPropertyMetadata(EditTrigger.OnDoubleTap, null));

        /// <summary>
        /// Gets or Sets if the DataGrid is Editable
        /// </summary>
        public bool AllowEditing
        {
            get { return (bool)this.GetValue(SfDataGrid.AllowEditingProperty); }
            set { this.SetValue(SfDataGrid.AllowEditingProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for AllowEditing property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowEditingProperty =
            GridDependencyProperty.Register("AllowEditing", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAllowEditingChanged));
#if !WP
        /// <summary>
        /// Gets or sets a value indicating whether User can delete Rows by pressing Delete Key
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowDeleting
        {
            get { return (bool)GetValue(AllowDeletingProperty); }
            set { SetValue(AllowDeletingProperty, value); }
        }

        public static readonly DependencyProperty AllowDeletingProperty =
            DependencyProperty.Register("AllowDeleting", typeof(bool), typeof(SfDataGrid), new PropertyMetadata(false));
#endif

        public bool AllowFiltering
        {
            get { return (bool)GetValue(AllowFilteringPropertyProperty); }
            set { SetValue(AllowFilteringPropertyProperty, value); }
        }

        public static readonly DependencyProperty AllowFilteringPropertyProperty =
            GridDependencyProperty.Register("AllowFiltering", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false, OnAllowFiltersChanged));

        public Style FilterPopupStyle
        {
            get { return (Style)GetValue(FilterPopupStyleProperty); }
            set { SetValue(FilterPopupStyleProperty, value); }
        }

        public static readonly DependencyProperty FilterPopupStyleProperty =
            GridDependencyProperty.Register("FilterPopupStyle", typeof(Style), typeof(SfDataGrid), new GridPropertyMetadata(null));

        public DataTemplate FilterPopupTemplate
        {
            get { return (DataTemplate)GetValue(FilterPopupTemplateProperty); }
            set { SetValue(FilterPopupTemplateProperty, value); }
        }

        public static readonly DependencyProperty FilterPopupTemplateProperty =
            GridDependencyProperty.Register("FilterPopupTemplate", typeof(DataTemplate), typeof(SfDataGrid), new GridPropertyMetadata(null));

        
        public NavigationMode NavigationMode
        {
            get { return (NavigationMode)GetValue(NavigationModeProperty); }
            set { SetValue(NavigationModeProperty, value); }
        }

        public static readonly DependencyProperty NavigationModeProperty =
            GridDependencyProperty.Register("NavigationMode", typeof(NavigationMode), typeof(SfDataGrid), new GridPropertyMetadata(NavigationMode.Cell, OnNavigationModeChanged));

        [Cloneable(false)]
        public DetailsViewDefinition DetailsViewDefinition
        {
            get { return (DetailsViewDefinition)GetValue(DetailsViewDefinitionProperty); }
            set { SetValue(DetailsViewDefinitionProperty, value); }
        }

        public static readonly DependencyProperty DetailsViewDefinitionProperty =
            GridDependencyProperty.Register("DetailsViewDefinition", typeof(DetailsViewDefinition), typeof(SfDataGrid), new GridPropertyMetadata(null, OnDetailsViewDefinitionChanged));

#else
        public bool EnableZooming
        {
            get { return (bool)GetValue(EnableZoomingProperty); }
            set { SetValue(EnableZoomingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableZooming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableZoomingProperty =
            GridDependencyProperty.Register("EnableZooming", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(false));

        public double ZoomScale
        {
			get { return (double)GetValue(ZoomScaleProperty); }
            set { SetValue(ZoomScaleProperty, value); }
        }

        public static readonly DependencyProperty ZoomScaleProperty =
            GridDependencyProperty.Register("ZoomScale", typeof(double), typeof(SfDataGrid), new GridPropertyMetadata(1.0, OnZoomScaleChanged));

#endif

#if WPF
        #region ContextMenu DP

        /// <summary>
        /// Gets or sets Context menu for Column Header.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu HeaderContextMenu
        {
            get { return (ContextMenu)GetValue(HeaderContextMenuProperty); }
            set { SetValue(HeaderContextMenuProperty, value); }
        }

        public static readonly DependencyProperty HeaderContextMenuProperty =
            GridDependencyProperty.Register("HeaderContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));


        /// <summary>
        /// Gets or sets ContextMenu for GroupDropAreaItem.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu GroupDropItemContextMenu
        {
            get { return (ContextMenu)GetValue(GroupDropItemContextMenuProperty); }
            set { SetValue(GroupDropItemContextMenuProperty, value); }
        }

        public static readonly DependencyProperty GroupDropItemContextMenuProperty =
            GridDependencyProperty.Register("GroupDropItemContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));


        /// <summary>
        /// Gets or sets ContextMenu for Group Drop Area.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu GroupDropAreaContextMenu
        {
            get { return (ContextMenu)GetValue(GroupDropAreaContextMenuProperty); }
            set { SetValue(GroupDropAreaContextMenuProperty, value); }
        }

        public static readonly DependencyProperty GroupDropAreaContextMenuProperty =
            GridDependencyProperty.Register("GroupDropAreaContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));

        /// <summary>
        /// Gets or sets ContextMenu for Record Cells.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu RecordContextMenu
        {
            get { return (ContextMenu)GetValue(RecordContextMenuProperty); }
            set { SetValue(RecordContextMenuProperty, value); }
        }

        public static readonly DependencyProperty RecordContextMenuProperty =
            GridDependencyProperty.Register("RecordContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));


        /// <summary>
        /// Gets or sets ContextMenu for Grouup Summary cells.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu GroupSummaryContextMenu
        {
            get { return (ContextMenu)GetValue(SummaryContextMenuProperty); }
            set { SetValue(SummaryContextMenuProperty, value); }
        }

        public static readonly DependencyProperty SummaryContextMenuProperty =
            GridDependencyProperty.Register("GroupSummaryContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));


        /// <summary>
        /// Gets or sets ContextMenu for Table summary cells.
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu TableSummaryContextMenu
        {
            get { return (ContextMenu)GetValue(TableSummaryContextMenuProperty); }
            set { SetValue(TableSummaryContextMenuProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryContextMenuProperty =
            GridDependencyProperty.Register("TableSummaryContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));


        /// <summary>
        /// Gets or sets ContextMenu for Group Cation cells..
        /// </summary>
        /// <value>Context Menu</value>
        /// <remarks></remarks>
        public ContextMenu GroupCaptionContextMenu
        {
            get { return (ContextMenu)GetValue(GroupCaptionContextMenuProperty); }
            set { SetValue(GroupCaptionContextMenuProperty, value); }
        }

        public static readonly DependencyProperty GroupCaptionContextMenuProperty =
            GridDependencyProperty.Register("GroupCaptionContextMenu", typeof(ContextMenu), typeof(SfDataGrid), new GridPropertyMetadata(null));

        #endregion
#endif

        public Brush RowHoverHighlightingBrush
        {
            get { return (Brush)GetValue(RowHoverHighlightingBrushProperty); }
            set { SetValue(RowHoverHighlightingBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHoverHighlightingBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHoverHighlightingBrushProperty =
            DependencyProperty.Register("RowHoverHighlightingBrush", typeof(Brush), typeof(SfDataGrid), new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnRowHoverHighlightingBrushPropertyChanged));


        public bool AllowRowHoverHighlighting
        {
            get { return (bool)GetValue(AllowRowHoverHighlightingProperty); }
            set { SetValue(AllowRowHoverHighlightingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowRowHoverHighlighting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowRowHoverHighlightingProperty =
            DependencyProperty.Register("AllowRowHoverHighlighting", typeof(bool), typeof(SfDataGrid), new PropertyMetadata(false));

        /// <summary>
        /// Get or Set the AddNewRow position of SfDataGrid
        /// </summary>
#if !WP
        [Cloneable(false)]
#endif
        public AddNewRowPosition AddNewRowPosition
        {
            get { return (AddNewRowPosition)GetValue(AddNewRowPositionProperty); }
            set { SetValue(AddNewRowPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddNewRowPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddNewRowPositionProperty =
            DependencyProperty.Register("AddNewRowPosition", typeof(AddNewRowPosition), typeof(SfDataGrid), new PropertyMetadata(AddNewRowPosition.None, OnAddNewRowPositionChanged));

#if !WP
        /// <summary>
        /// Gets the Printing Settings to Initalize the Print Options
        /// </summary>
        public PrintSettings PrintSettings
        {
            get { return (PrintSettings)GetValue(PrintSettingsProperty); }
            set { SetValue(PrintSettingsProperty, value); }
        }

        public static readonly DependencyProperty PrintSettingsProperty =
            DependencyProperty.Register("PrintSettings", typeof(PrintSettings), typeof(SfDataGrid),
                                        new PropertyMetadata(new PrintSettings()));

        public EditorSelectionBehavior EditorSelectionBehavior
        {
            get { return (EditorSelectionBehavior)GetValue(EditorSelectionBehaviorProperty); }
            set { SetValue(EditorSelectionBehaviorProperty, value); }
        }

        public static readonly DependencyProperty EditorSelectionBehaviorProperty =
            GridDependencyProperty.Register("EditorSelectionBehavior", typeof(EditorSelectionBehavior), typeof(SfDataGrid), new GridPropertyMetadata(EditorSelectionBehavior.SelectAll));

#endif
        /// <summary>
        /// Property which holds the Current selected row data value.
        /// To-Do : As of now CurrentItem doesn't maintaing in CollectionChange, Sorting, Grouping and Filtering operation,
        /// due to ResetSelectedRows and RefreshSelectedItems methods in SelectionController. We need more time to test if change these methods.
        /// Hence we planned to implement this behaviour with Cell Selection feature.
        /// </summary>
        public object CurrentItem
        {
            get { return (object)GetValue(CurrentItemProperty); }
            set { SetValue(CurrentItemProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentItem
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(object), typeof(SfDataGrid), new PropertyMetadata(null, OnCurrentItemChanged));

        #endregion

        #region Dependency Property Call back

        /// <summary>
        /// Dependency call back for IsGroupDropAreaExpandedPropertyChanged.
        /// </summary>
        private static void IsGroupDropAreaExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.groupDropArea != null)
                grid.GroupDropArea.IsExpanded = (bool)e.NewValue;
        }

        /// <summary>
        /// Dependency call back for ItemsSource property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            grid.UnWireEvents();
            if (grid.SelectionController != null && grid.SelectionController.SelectedRows.Any())
                grid.ResetSelectionValues();

            if (grid.GridModel != null)
                grid.GridModel.EndEdit();
            if (args.OldValue != null)
            {
                if (grid.GroupDropArea != null)
                    grid.GroupDropArea.RemoveAllGroupDropItems();
                grid.DisposeViewOnItemsSourceChanged();
                grid.GridModel.RefreshDataRow();
            }

            grid.ShowBusyIndicator = false;
            grid.SetSourceList(args.NewValue);
            grid.EnsureViewProperties();
            grid.RowGenerator.OnItemSourceChanged(args);
            grid.RefreshHeaderLineCount();
            grid.UpdateRowAndColumnCount(false);
            if (grid.VisualContainer != null)
                grid.VisualContainer.OnItemSourceChanged();

            grid.RaiseItemsSourceChanged(args.OldValue, args.NewValue);
        }

        protected virtual void DisposeViewOnItemsSourceChanged()
        {
            this.View.Dispose();
        }

        /// <summary>
        /// Dependency Call back for Show Sort Numbers
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnSortNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (!grid.isGridLoaded || grid.View == null)
                return;

            if ((bool) e.NewValue)
            {
                if (grid.View.SortDescriptions.Count > 1)
                    grid.GridModel.ShowSortNumbers();
            }
            else
                grid.GridModel.CollapseSortNumber();
            grid.GridColumnSizer.RefreshAll();
        }

        /// <summary>
        /// Dependency Call back for ColumnSizer
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnColumnSizerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            grid.GridColumnSizer.RefreshAll();
        }

        private static void OnAllowSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.isGridLoaded)
            {
                //grid.GridColumnSizer.RefreshAll();   
            }
        }

        /// <summary>
        /// Dependency call backk for AutoGenerateColumns.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnAutoGenerateColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid != null && grid.AutoGenerateColumns && grid.AutoGenerateColumnsMode != AutoGenerateColumnsMode.None)
                grid.GenerateGridColumns();
        }

        /// <summary>
        /// Dependency call backk for AutoGenerateColumnsMode.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnAutoGenerateColumnsModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid != null && grid.AutoGenerateColumns && grid.AutoGenerateColumnsMode != AutoGenerateColumnsMode.None)
                grid.GenerateGridColumns();
        }

        /// <summary>
        /// Dependency call back for SelectedItem
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded || grid.View == null)
            {
                grid.isselecteditemchanged = true;
                return;
            }
            grid.isselecteditemchanged = false;

            grid.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle()
                {
                    NewValue = args.NewValue,
                    OldValue = args.OldValue,
                    PropertyName = "SelectedItem"
                });
        }

        /// <summary>
        /// Dependency call back for SelectionMode.
        /// If selection mode changed to Single current tow selection only maintained.
        /// If selection mode changed to None current cell selection only maintained all other selection shoud be cleared.
        /// For other two modes selection will be maintained.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnSelectionModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;
#if !WP
            if (grid.DetailsViewManager.HasDetailsView)
            {
                foreach (var detailsview in grid.DetailsViewDefinition)
                {
                    var detailsViewGrid = (detailsview as GridViewDefinition).DataGrid;
                    detailsViewGrid.SelectionMode = (GridSelectionMode)args.NewValue;
                }
            }
#endif

            grid.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle() { NewValue = args.NewValue, OldValue = args.OldValue, PropertyName = "SelectionMode" });
            foreach (var column in grid.Columns.OfType<GridCheckBoxColumn>())
                grid.RowGenerator.UpdateBinding(column);
        }

        /// <summary>
        /// Dependency call back for SelectionBackground.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnRowSelectionBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            grid.SelectionController.RowSelectionBrush = (Brush)args.NewValue;
        }

        /// <summary>
        /// Depndency call back for GroupCaptionRowSelectionBrush
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnGroupRowSelectionBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            grid.SelectionController.GroupRowSelectionBrush = (Brush)args.NewValue;
        }

        /// <summary>
        /// Dependency call back for Selected Index.
        /// User can set the SelectedIndex dynamically.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded || grid.View == null)
            {
                grid.isselectedindexchanged = true;
                return;
            }
            grid.isselectedindexchanged = false;
            grid.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle()
                {
                    NewValue = args.NewValue,
                    OldValue = args.OldValue,
                    PropertyName = "SelectedIndex"
                });
        }

        /// <summary>
        /// Dependency call back method of Row Height Property.
        /// Sets the Default row height of all the Rows in SfDataGrid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnRowHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            grid.VisualContainer.RowHeights.DefaultLineSize = (double) args.NewValue;
            grid.VisualContainer.InvalidateMeasure();
        }

        /// <summary>
        /// Dependency call back method of Header Row Height Property.
        /// Sets the Default row height of Header Row in SfDataGrid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnHeaderRowHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            for (int i = 0; i < grid.HeaderLineCount; i++)
            {
                grid.VisualContainer.RowHeights[i] = (double) args.NewValue;
            }
#if !WP
            if (grid is DetailsViewDataGrid)
                grid.DetailsViewManager.RefreshParentDataGrid(grid);
#endif
            grid.VisualContainer.InvalidateMeasure();
        }

        /// <summary>
        /// Dependency call back method of CaptionSummaryRow. Which is helpes to upadete the view while settinf the CaptionSummaryRowDynamically
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnCaptionSummaryRowChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = obj as SfDataGrid;
            if (dataGrid == null || !dataGrid.isGridLoaded || dataGrid.View == null)
                return;
            dataGrid.OnCaptionSummaryRowChanged(args.NewValue as GridSummaryRow);
        }

        /// <summary>
        /// Dependency call back for CellStyle property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null)
                return;
            grid.hasCellStyle = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateCellStyles();
            }
        }

        private static void OnGroupSummaryCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasGroupSummaryCellStyle = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        private static void OnGroupSummaryCellStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasGroupSummaryCellStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        private static void OnCaptionSummaryCellStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCaptionSummaryCellStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        private static void OnCaptionSummaryCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCaptionSummaryCellStyle = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        private static void OnTableSummaryCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasTableSummaryCellStyle = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        /// <summary>
        /// Dependency call back for CellStyleSelector property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnCellStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCellStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateCellStyles();
            }
        }

        private static void OnTableSummaryCellStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasTableSummaryCellStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateSummariesCellStyle();
            }
        }

        /// <summary>
        /// Dependency call back for CellTemplateSelector property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnCellTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCellTemplateSelector = e.NewValue != null;
            if (grid.isGridLoaded)
            {
                grid.UpdateCellStyles();
            }
        }

        /// <summary>
        /// Dependency call back for RowStyleSelector property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnRowStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasRowStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateRowStyle();
        }

        /// <summary>
        /// Dependency call back for RowStyle property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasRowStyle = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateRowStyle();
        }

        private static void OnGroupSummaryRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasGroupSummaryRowStyle = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        private static void OnGroupSummaryRowStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasGroupSummaryRowStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        private static void OnCaptionSummaryRowStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCaptionSummaryRowStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        private static void OnCaptionSummaryRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasCaptionSummaryRowStyle = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        private static void OnTableSummaryRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasTableSummaryRowStyle = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        private static void OnTableSummaryRowStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null) return;
            grid.hasTableSummaryRowStyleSelector = e.NewValue != null;
            if (grid.isGridLoaded)
                grid.UpdateSummariesRowStyle();
        }

        /// <summary>
        /// Dependency call back for HeaderStyle property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.isGridLoaded)
            {
                grid.UpdateHeaderRowStyle();
            }
        }

        /// <summary>
        /// Dependency call back for HeaderTemplate property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.isGridLoaded)
            {
                grid.UpdateHeaderRowStyle();
            }
        }

        private static void OnLiveDataUpdateModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid.View != null)
                grid.View.LiveDataUpdateMode = grid.LiveDataUpdateMode;
        }

        private static void OnFrozenColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.isGridLoaded)
                grid.VisualContainer.FrozenColumns = grid.ResolveToScrollColumnIndex((int) e.NewValue);
        }

        private static void OnAutoExpandGroupsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid.View != null)
            {
                grid.View.AutoExpandGroups = (bool)args.NewValue;
            }
        }

        private static void OnAllowFixedGroupCaptionsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid.VisualContainer != null)
            {
                grid.VisualContainer.AllowFixedGroupCaptions = grid.AllowFrozenGroupHeaders;
                grid.VisualContainer.InvalidateMeasureInfo();
            }
        }
#if !WP
        private static void OnAllowFiltersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            if (grid.Columns.Count > 0)
                grid.RefreshFilterIconVisibility();
        }
#else
        private static void OnZoomScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDataGrid grid = d as SfDataGrid;
            if (grid.VisualContainer != null)
            {
                grid.VisualContainer.ZoomScale = grid.ZoomScale;
            }
        }
#endif
        private static void OnAllowEditingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            foreach (var column in grid.Columns.OfType<GridCheckBoxColumn>())
                grid.RowGenerator.UpdateBinding(column);
        }

        private static void OnGroupSummaryRowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null || !grid.isGridLoaded || grid.View == null)
                return;

            var groupSummaryRows = e.NewValue as ObservableCollection<GridSummaryRow>;
            if (groupSummaryRows == null) 
                return;

            grid.View.SummaryRows.Clear();
            grid.InitializeGroupSummaryRows();
        }

        private static void OnTableSummaryRowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid == null || !grid.isGridLoaded || grid.View == null)
                return;

            var tableSummaryRows = e.NewValue as ObservableCollection<GridSummaryRow>;
            if (tableSummaryRows == null)
                return;

            grid.View.TableSummaryRows.Clear();
            grid.InitializeTableSummaries();
            grid.GridModel.InitializeGridTableSummaryRow();
            if (e.OldValue != null)
            {
                (e.OldValue as ObservableCollection<GridSummaryRow>).ForEach(row =>
                    {
                        if (row is GridTableSummaryRow)
                            (row as GridTableSummaryRow).TableSummaryPositionChanged = null;
                    });
            }

            grid.RefreshHeaderLineCount();
            grid.UpdateRowAndColumnCount(false);
        }

        private static void OnNavigationModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
#if !WP
            if (grid.DetailsViewManager.HasDetailsView)
            {
                foreach (var detailsview in grid.DetailsViewDefinition)
                {
                    var detailsViewGrid = (detailsview as GridViewDefinition).DataGrid;
                    detailsViewGrid.NavigationMode = (NavigationMode)args.NewValue;
                }
            }
#endif
            if(grid.isGridLoaded)
            {
                grid.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle() { NewValue = args.NewValue, OldValue = args.OldValue, PropertyName = "NavigationMode" });
            }
        }

#if !WP
        private static void OnDetailsViewDefinitionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            grid.RowGenerator.Items.OfType<DetailsViewDataRow>().ForEach(row =>
                {
                    row.CatchedRowIndex = -1;
                });
            if (grid.View != null)
                grid.View.Refresh();
            grid.UpdateRowAndColumnCount(false);
            grid.VisualContainer.NeedToRefreshColumn = true;
            grid.VisualContainer.InvalidateMeasure();
        }

        private static void OnHideEmptyGridViewDefinitionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            if (grid.DetailsViewManager.HasDetailsView)
            {
                grid.RowGenerator.Items.OfType<DataRow>().ForEach(row =>
                    {
                        row.CheckForDetailsViewExpanderVisibilty();
                    });
            }
        }

#endif

        private static void OnStackedHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            grid.RowGenerator.RemoveStackedHeader();
            grid.RefreshHeaderLineCount();
            grid.UpdateRowAndColumnCount(false);
            grid.RowGenerator.Items.ForEach(row => row.RowIndex = -1);
            grid.VisualContainer.InvalidateMeasureInfo();
        }

        private static void OnGridValidationPropertyChanded(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (grid.isGridLoaded)
                grid.UpdateValidationMode();
        }

        private static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            (grid.VisualContainer.ColumnWidths as LineSizeCollection).SuspendUpdates();
            if ((bool) e.NewValue)
            {
                grid.SelectionController.HandleCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
#if SILVERLIGHT
                        null
#else
                                                         new List<object>()
#endif
                                                         , -1), CollectionChangedReason.ColumnsCollection);
                grid.container.InsertColumns(0, 1);
                grid.container.ColumnWidths[0] = grid.RowHeaderWidth;
                grid.container.UpdateScrollBars();
                if (grid.FrozenColumnCount > 0)
                    grid.VisualContainer.FrozenColumns = grid.ResolveToScrollColumnIndex(grid.FrozenColumnCount);
                else
                    grid.VisualContainer.FrozenColumns = 1;
                grid.container.NeedToRefreshColumn = true;
                grid.container.InvalidateMeasure();
            }
            else
            {
                grid.inRowHeaderChange = true;
                grid.SelectionController.HandleCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
#if SILVERLIGHT
                        null
#else
                                                         new List<object>()
#endif
                                                         , 0), CollectionChangedReason.ColumnsCollection);
                grid.VisualContainer.RemoveColumns(0, 1);
                grid.inRowHeaderChange = false;
                if (grid.FrozenColumnCount > 0)
                    grid.VisualContainer.FrozenColumns = grid.ResolveToScrollColumnIndex(grid.FrozenColumnCount);
                else
                    grid.VisualContainer.FrozenColumns = 0;
                grid.container.UpdateScrollBars();
                grid.container.NeedToRefreshColumn = true;
                grid.container.InvalidateMeasure();
            }
            (grid.VisualContainer.ColumnWidths as LineSizeCollection).ResumeUpdates();
            if (grid.VisualContainer.ColumnCount > 0)
            {
                var addNewRow = grid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
                if (addNewRow != null)
                    (addNewRow.WholeRowElement as AddNewRowControl).UpdateTextBorder();
                grid.RowGenerator.RefreshStackedHeaders();
                grid.GridColumnSizer.RefreshAll();
            }
        }

        private static void OnRowHeaderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            if (grid.ShowRowHeader)
            {
                grid.VisualContainer.ColumnWidths[0] = (double)(e.NewValue);
                grid.GridColumnSizer.RefreshAll();
            }
        }

        private static void OnRowHoverHighlightingBrushPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            grid.SelectionController.RowHoverBackgroundBrush = (Brush)args.NewValue;
        }

        private static void OnSummaryGroupComparerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid.View != null)
            {
                grid.View.BeginInit();
                grid.View.GroupComparer = args.NewValue as IComparer<Group>;
                grid.SelectionController.ClearSelections(false);
                grid.View.EndInit();
            }
        }

        private static void OnAddNewRowPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
#if !WP
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded || grid.View == null)
                return;

            var oldValue = (AddNewRowPosition) args.OldValue;
            if (oldValue != AddNewRowPosition.None)
            {
                grid.SelectionController.CurrentCellManager.EndEdit();
                if (grid.View.IsAddingNew)
                    grid.GridModel.addNewRowController.CommitAddNew();
            }

            var addNewRow = grid.RowGenerator.Items.FirstOrDefault(item => item.IsAddNewRow);
            if (addNewRow != null)
                addNewRow.RowIndex = -1;
            grid.RefreshHeaderLineCount();
            grid.UpdateRowAndColumnCount(false);
            grid.RowGenerator.Items.ForEach(row => row.RowIndex = -1);
            grid.SelectionController.HandleGridOperations(
                new GridOperationsHandle(GridOperation.AddNewRow,
                    new AddNewRowOperationHandle(AddNewRowOperation.PlacementChange, args)));
            if (grid.VisualContainer != null)
                grid.VisualContainer.InvalidateMeasureInfo();
#endif
        }

        private static void OnAllowResisizingHiddenColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;

            if (grid.VisualContainer != null && grid.AllowResizingColumns && (bool)e.NewValue)
            {
                grid.Columns.ForEach(col =>
                {
                    if (col.IsHidden)
                        grid.GridColumnResizingController.ProcessResizeStateManager(col);
                });
            }
            else if (grid.VisualContainer != null)
            {
                grid.Columns.ForEach(col => grid.GridColumnResizingController.ProcessResizeStateManager(col));
            }
        }

        private static void OnAllowResisizingColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as SfDataGrid;
            if (!grid.isGridLoaded)
                return;
            if (grid.VisualContainer != null && grid.AllowResizingHiddenColumns && (bool)e.NewValue)
            {
                grid.Columns.ForEach(col =>
                {
                    if (col.IsHidden)
                        grid.GridColumnResizingController.ProcessResizeStateManager(col);
                });
            }
            else if (grid.VisualContainer != null)
            {
                grid.Columns.ForEach(col => grid.GridColumnResizingController.ProcessResizeStateManager(col));
            }
        }

        private static void OnCurrentItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as SfDataGrid;
            if (grid.View != null)
            {
                var handle = new SelectionPropertyChangeHandle()
                    {
                        NewValue = args.NewValue,
                        OldValue = args.OldValue,
                        PropertyName = "CurrentItem"
                    };
                grid.SelectionController.HandleSelectionPropertyChanges(handle);
            }
        }

        #endregion

        #region Ctor

        static SfDataGrid()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SfDataGrid), new FrameworkPropertyMetadata(typeof(SfDataGrid)));
#endif
        }

        public SfDataGrid()
        {
#if !WPF
            base.DefaultStyleKey = typeof(SfDataGrid);
#if WinRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateRailsX |
                                  ManipulationModes.TranslateRailsY | ManipulationModes.TranslateInertia;
#endif
#endif
            this.InitializeCollections();
            this.RowGenerator = new RowGenerator(this);
            this.SelectionController = new GridSelectionController(this);
            this.GridModel = new GridModel(this);
            this.GridColumnSizer = new GridColumnSizer(this);
            this.GridColumnResizingController = new GridColumnResizingController(this);
            this.GridColumnDragDropController = new GridColumnDragDropController(this);
#if !WP
            this.DetailsViewManager = new DetailsViewManager(this);
            this.GridCopyPaste = new GridCutCopyPaste(this);
#endif
            cellRenderers = new GridCellRendererCollection(this);
            Validations = new ValidationHelper(this);
            this.InitializeCellRendererCollection();
        }

        protected virtual void UnWireEvents()
        {
            if (!isEventsFired)
                return;
            UnWireDataGridEvents();
            UnWireViewEvents();
            if (this.GridModel != null)
                this.GridModel.UnWireEvents();
            isEventsFired = false;
        }

        private bool isEventsFired = false;

        protected virtual void WireEvents()
        {
            if (isEventsFired)
                return;

            WireDataGridEvents();
            WireViewEvents();
            if (this.GridModel != null)
                this.GridModel.WireEvents();
            isEventsFired = true;
        }

        #endregion

        #region Override methods

        #region OnApplyTemplate

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.container = GetTemplateChild("PART_VisualContainer") as VisualContainer;
            this.groupDropArea = base.GetTemplateChild("PART_GroupDropArea") as GroupDropArea;
#if WinRT
            this.container.ScrollOwner = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
#endif
            if (this.groupDropArea != null)
                this.groupDropArea.dataGrid = this;
            this.RefreshContainerAndView();
        }

        protected bool IsChanged(DependencyProperty dependencyProperty)
        {
            if (this.ReadLocalValue(dependencyProperty) == DependencyProperty.UnsetValue)
                return false;
            return true;
        }

        protected virtual void RefreshContainerAndView()
        {
            if (this.container == null || this.RowGenerator == null) 
                return;
#if WinRT
            this.container.ContainerKeydown = OnContainerKeyDown;
#elif !WPF
            this.container.ContainerKeydown = OnContainerKeyDown;
#endif
            this.container.DragBorderBrush = this.BorderBrush;
            this.container.DragBorderThickness = this.BorderThickness;
            this.container.AllowFixedGroupCaptions = this.AllowFrozenGroupHeaders;
            this.container.SetRowGenerator(this.RowGenerator);
#if WP && !WP7
            this.container.SetZoomScale = SetZoomScale;
#endif
            this.UnWireEvents();
            if (this.ItemsSource != null)
            {
                this.SetSourceList(this.ItemsSource);
                this.RaiseItemsSourceChanged(null, this.ItemsSource);
            }

            this.RefreshHeaderLineCount();
            this.UpdateRowAndColumnCount(true);
            this.EnsureProperties();
            if (this.View != null)
                this.EnsureViewProperties();
            this.isGridLoaded = true;
            this.WireEvents();
        }

        protected virtual void RefreshHeaderLineCount()
        {
            headerLineCount = 1;
            if (StackedHeaderRows.Count > 0)
                headerLineCount += StackedHeaderRows.Count;
            if (AddNewRowPosition == AddNewRowPosition.Top)
                headerLineCount += 1;
            headerLineCount += this.GetTableSummaryCount(TableSummaryRowPosition.Top);
        }

        #endregion

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
#if WinRT
            
            if (availableSize.Width!=0.0 && availableSize.Height!=0.0)
             {
                if (container != null)
                {
                    var groupDropAreaClipSize = 0d;
                    if (groupDropArea != null && ShowGroupDropArea)
                        groupDropAreaClipSize = groupDropArea.IsExpanded
                                              ? groupDropArea.MaxHeight
                                              : groupDropArea.MinHeight;
                    container.ViewPortSize = new Size(availableSize.Width - (BorderThickness.Left + BorderThickness.Right),
                                                      availableSize.Height - groupDropAreaClipSize);
                }
             }
#endif
            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #endregion

        #region Handle selection Operation
#if !WPF
        /// <summary>
        /// Handling the keydown operations of SfDataGrid
        /// </summary>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Input.KeyRoutedEventArgs">KeyRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
#if !WP
            bool result = SelectionController.HandleKeyDown(e);
#endif
#if !SILVERLIGHT && !WP
            if (result)
                e.Handled = true;
#endif
        }
#endif

#if WPF
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!e.OriginalSource.Equals(this))
            {
                var el = e.OriginalSource as DependencyObject;
                
                if (el != null)
                {
                    if (el is ComboBoxItem)
                    {
                        var sender = GridUtil.FindDescendant(el, typeof(ComboBox));
                        if (sender != null && VisualContainer.GetWantsMouseInput(sender, this) == true)
                            return;
                    }
                    if (VisualContainer.GetWantsMouseInput(el, this) == true)
                    {
                        var currentCell = this.SelectionController.CurrentCellManager.CurrentCell;
                        if (currentCell != null && currentCell.IsEditing && currentCell.Renderer.IsDropDownable)
                        {
                            if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Up && e.Key != Key.Down &&
                                e.Key != Key.Escape && e.Key != Key.F2)
                                return;
                        }
                        else
                            return;
                    }
                }
            }
            e.Handled = SelectionController.HandleKeyDown(e);
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            var isLeftButtonPressed = e.ChangedButton == MouseButton.Left;
            if (!this.IsKeyboardFocusWithin && isLeftButtonPressed)
                this.Focus();
        }      

#endif
#if SILVERLIGHT
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!e.Handled && this.NavigationMode == NavigationMode.Row && base.Focus())
            {
                e.Handled = true;
            }
        }
#endif
        //Need to check in WPF and Silverlight
#if !WinRT && !WP
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            var rowColumnIndex = SelectionController.CurrentCellManager.CurrentCellIndex;
            var dataRow = RowGenerator.Items.FirstOrDefault(item => item.RowIndex == rowColumnIndex.RowIndex);
            if (dataRow != null)
            {
                var dataColumn = dataRow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == rowColumnIndex.ColumnIndex);
                char text;
                char.TryParse(e.Text, out text);
                if (dataColumn != null && !(dataColumn.Renderer is GridCellTemplateRenderer) && !dataColumn.IsEditing && char.IsLetterOrDigit(text) && SelectionController.CurrentCellManager.BeginEdit())
                    dataColumn.Renderer.PreviewTextInput(e);
            }
            base.OnTextInput(e);
        }
#endif

#if !WPF
        /// <summary>
        /// Raises the <see cref="E:ContainerKeyDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected bool OnContainerKeyDown(KeyEventArgs e)
        {
#if !WP
            SelectionController.HandleKeyDown(e);
#endif
            return e.Handled;
        }
#endif

        /// <summary>
        /// Here we handled Manipulation Started event to avoid the selection while manipulating SfDataGrid
        /// </summary>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs">ManipulationStartedRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if WinRT
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
#else
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
#endif
        {
            base.OnManipulationStarted(e);
            e.Handled = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method which is used to Select the Multiple rows by passing the starting index and end index.
        /// Multiple row will be selected only for Multiple and Extended mode selection.
        /// This is not applicable for Single and None selection mode.
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="endRowIndex"></param>
        /// <remarks></remarks>
        private void SelectRows(int startRowIndex, int endRowIndex)
        {
            this.SelectionController.SelectRows(startRowIndex, endRowIndex);
        }

        /// <summary>
        /// Get UnBoundCell Value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object GetUnBoundCellValue(GridColumn column, object record)
        {
            var col = column as GridUnBoundColumn;
            if (col == null || record == null)
                return string.Empty;
            object value = null;

#if !SILVERLIGHT && !WP
            if (col.Format != string.Empty)
#else
            if (col.Format != string.Empty)
#endif
            {
                value = col.Format.FormatByName(null, (key) =>
                {
                    var itemProperties = this.View.GetItemProperties();
                    var pd = itemProperties.GetPropertyDescriptor(key);
                    if (pd != null)
                    {
                        return pd.GetValue(record);
                    }
                    else
                    {
                        if (!col.CaseSensitive)
                        {
                            string s1 = key.ToLower();

#if !WPF
                            foreach (var kvp in itemProperties)
                            {
                                if (s1 == kvp.Value.Name.ToLower())
                                {
                                    key = kvp.Value.Name;
                                    break;
                                }
                            }
#else
                            foreach (PropertyDescriptor kvp in itemProperties)
                            {
                                if (s1 == kvp.Name.ToLower())
                                {
                                    key = kvp.Name;
                                    break;
                                }
                            }
#endif
                            if (itemProperties.GetPropertyDescriptor(key) != null)
                                return itemProperties.GetPropertyDescriptor(key).GetValue(record);

                        }
                    }
                    return null;
                });
                value = value.ToString().Substring(1, value.ToString().Length - 2);
            }
#if !SILVERLIGHT && !WP
            else if (col.Expression != string.Empty)
#else
            else if (col.Expression != string.Empty)
#endif
            {
                value = col.ComputedValue(record);
            }
            var handledValue = RaiseQueryUnboundValue(UnBoundActions.QueryData, value, column, record);
            if (handledValue == null)
                handledValue = record;
            return handledValue;
        }

        public void ScrollInView(RowColumnIndex rowColumnIndex)
        {
            if (rowColumnIndex.RowIndex < this.VisualContainer.ScrollRows.LineCount && rowColumnIndex.ColumnIndex < this.VisualContainer.ScrollColumns.LineCount)
            {
                if (rowColumnIndex.RowIndex >= 0)
                    this.VisualContainer.ScrollRows.ScrollInView(rowColumnIndex.RowIndex);
                if (rowColumnIndex.ColumnIndex >= 0)
                    this.VisualContainer.ScrollColumns.ScrollInView(rowColumnIndex.ColumnIndex);
                this.VisualContainer.InvalidateMeasureInfo();
            }
        }
        /// <summary>
        /// Method which is used to Select All the rows in SfDataGrid
        /// SelectAll method only works for Multiple and Extended mode selection.
        /// </summary>
        /// <remarks></remarks>
        public void SelectAll()
        {
            this.SelectionController.SelectAll();
        }

        /// <summary>
        /// Method which is used to clear all the Selection present in Grid.
        /// </summary>
        /// <param name="exceptCurrentRow">If set to <see langword="true"/>, then Current row will not clear ; otherwise all the selection will be cleared, .</param>
        /// <remarks></remarks>
        public void ClearSelections(bool exceptCurrentRow)
        {
            this.SelectionController.ClearSelections(exceptCurrentRow);
        }

#if WPF || SILVERLIGHT

        /// <summary>
        /// Invokes the Print Preview of Grid's View
        /// </summary>
        /// <remarks></remarks>
        public void ShowPrintPreview()
        {
#if WPF
            var window = new ChromelessWindow
            {
                Content = new GridPrintPreviewControl(this, PrintSettings.PrintManagerBase),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            SkinStorage.SetEnableOptimization(window, false);
            SkinStorage.SetVisualStyle(window, "Metro");
            if (PrintSettings.PrintPreviewWindowStyle != null)
                window.Style = PrintSettings.PrintPreviewWindowStyle;
            else
            {
                var resources = new ResourceDictionary
                {
                    Source =
                        new Uri("/Syncfusion.SfGrid.WPF;component/Print/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };

                window.Style = resources["ChromelessWindowStyle"] as Style;
            }
            window.ShowDialog();
#elif SILVERLIGHT
            var window = new ChildWindow
            {
                Content = new GridPrintPreviewControl(this, PrintSettings.PrintManagerBase),
                Width = Application.Current.Host.Content.ActualWidth - 100,
                Height = Application.Current.Host.Content.ActualHeight - 50
            };
            var res = new Uri("/Syncfusion.SfGrid.Silverlight;component/Print/Themes/MetroStyle.xaml",
                              UriKind.RelativeOrAbsolute);
            var resources = new ResourceDictionary
            {
                Source =res
            };
            window.Resources.MergedDictionaries.Add(resources);
            window.Show();      
#endif
        }
#endif

#if !WP
        /// <summary>
        /// Invokes the Print of Grid's View with Default Print Settings.
        /// </summary>
        public void Print()
        {
            if (PrintSettings.PrintManagerBase == null)
                PrintSettings.PrintManagerBase = new GridPrintManager(this);
            PrintSettings.PrintManagerBase.Print();
        }
#endif
        /// <summary>
        /// Method which helps to make the Grouping by passing the column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="converter"></param>
        /// <remarks></remarks>
        internal void GroupBy(string columnName, IValueConverter converter)
        {
            this.GridModel.GroupBy(columnName, converter);

            //While made new group all the selection should clear and selection should maintained in 1st row.
            this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, columnName, 0)));
        }

        /// <summary>
        /// Method which helps to make the Grouping by passing the column name and its position.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="converter"></param>
        /// <remarks></remarks>
        internal void GroupBy(string columnName, int insertAt, IValueConverter converter)
        {
            this.GridModel.GroupBy(columnName, insertAt, converter);

            //While made new group all the selection should clear and selection should maintained in 1st row.
            this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, columnName, insertAt)));
        }

        /// <summary>
        /// Methos which helps you to remove the grouping by passing the column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <remarks></remarks>
        internal void RemoveGroup(string columnName)
        {
            this.GridModel.RemoveGroup(columnName);
            this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, columnName, 0)));
        }

        /// <summary>
        /// Method which helps you to Expand all the Groups
        /// </summary>
        /// <remarks></remarks>
        public void ExpandAllGroup()
        {
            if (this.View != null && this.GridModel.HasGroup)
            {
                this.View.TopLevelGroup.ExpandAll();
                this.UpdateRowCountAndScrollBars();
                this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, null));
                this.GridModel.RefreshDataRow();
            }
        }

        /// <summary>
        /// Method which helps to collapse all the groups.
        /// </summary>
        /// <remarks></remarks>
        public void CollapseAllGroup()
        {
            if (this.View != null && this.GridModel.HasGroup)
            {
                this.View.TopLevelGroup.CollapseAll();
                this.SelectionController.ClearSelections(false);
                this.UpdateRowCountAndScrollBars();
                this.GridModel.RefreshDataRow();
            }
        }

        /// <summary>
        /// Method which helps to Expand all the groups in specific level
        /// </summary>
        /// <param name="groupLevel"></param>
        /// <remarks></remarks>
        public void ExpandGroupsAtLevel(int groupLevel)
        {
            if (groupLevel <= this.View.TopLevelGroup.GetMaxLevel())
            {
                this.GridModel.ExpandGroupsAtLevel(this.View.TopLevelGroup.Groups, groupLevel);
                this.UpdateRowCountAndScrollBars();
                this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, null));
                this.GridModel.RefreshDataRow();
            }
        }

        /// <summary>
        /// Method which helps to collapse all the groups in spaecific level
        /// </summary>
        /// <param name="groupLevel"></param>
        /// <remarks></remarks>
        public void CollapseGroupsAtLevel(int groupLevel)
        {
            if (groupLevel <= this.View.TopLevelGroup.GetMaxLevel())
            {
                this.GridModel.CollapseGroupsAtLevel(this.View.TopLevelGroup.Groups, groupLevel);
                this.UpdateRowCountAndScrollBars();
                this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, null));
                this.GridModel.RefreshDataRow();
            }
        }

        /// <summary>
        /// Expand the Group
        /// </summary>
        /// <param name="group"></param>
        /// <remarks></remarks>
        public void ExpandGroup(Group group)
        {
            if (this.GridModel != null)
            {
                this.GridModel.ExpandGroup(group);
                //this.UpdateRowCountAndScrollBars();
                this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, null));
                this.GridModel.RefreshDataRow();
            }
        }

        /// <summary>
        /// Collapse the Group.
        /// </summary>
        /// <param name="group"></param>
        /// <remarks></remarks>
        public void CollapseGroup(Group group)
        {
            if (this.GridModel != null)
            {
                this.GridModel.CollapseGroup(group);
                //this.UpdateRowCountAndScrollBars();
                this.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, null));
                this.GridModel.RefreshDataRow();
            }
        }

#if !WP
        public void Serialize(Stream stream)
        {
            var serializablegrid = SerializationHelper.CopyFromDataGrid(this);
            var serializer = new DataContractSerializer(typeof(SerializableDataGrid));
            serializer.WriteObject(stream, serializablegrid);
        }


        public void Deserialize(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(SerializableDataGrid));
            var grid = serializer.ReadObject(stream);
            UpdateAndRefreshGrid(grid as SerializableDataGrid);
        }
#endif

        /// <summary>
        /// Clear all the filters.
        /// </summary>
        /// <remarks></remarks>
        public void ClearFilters()
        {
#if !WP
            if (this.Columns != null)
                this.Columns.ForEach(column => this.GridModel.ClearFilters(column));
#endif
        }

        /// <summary>
        /// Clear filter for corresponding column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <remarks></remarks>
        public void ClearFilter(string columnName)
        {
#if !WP
            if (this.Columns != null)
                this.GridModel.ClearFilters(this.Columns.FirstOrDefault(column => column.MappingName == columnName));
#endif
        }

        /// <summary>
        /// Clear filter for corresponding column.
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public void ClearFilter(GridColumn column)
        {
            ClearFilter(column.MappingName);
        }


#if WinRT
        public async void Serialize(StorageFile storageFile)
        {
            using (var stream = await storageFile.OpenStreamForWriteAsync())
            {
                Serialize(stream);
            }
        }

        public async void Deserialize(StorageFile storageFile)
        {
            using (var stream = await storageFile.OpenStreamForReadAsync())
            {
                Deserialize(stream);
            }
        }
#elif WP

        private void SetZoomScale(double _ZoomScale)
        {
            if (!this.suspendZooming && EnableZooming)
            {
                var args = RaiseZoomingEvent(new GridZoomingEventArgs(_ZoomScale));
                if (!args.Cancel)
                {
                    this.ZoomScale = args.ZoomScale;
                    RaiseZoomedEvent(new GridZoomedEventArgs(this.ZoomScale));
                }
            }
        }

        public void HideIndentColumn(int ColumnIndex)
        {
            if (View != null && this.View.GroupDescriptions.Count > ColumnIndex)
            {
                this.VisualContainer.ColumnWidths.SetHidden(ColumnIndex, ColumnIndex, true);
                this.GridColumnSizer.RefreshAll();
            }
        }
        public void ShowIndentColumn(int ColumnIndex)
        {
            if (View != null && this.View.GroupDescriptions.Count > ColumnIndex)
            {
                this.VisualContainer.ColumnWidths.SetHidden(ColumnIndex, ColumnIndex, false);
                this.GridColumnSizer.RefreshAll();
            }
        }
#endif

#if !WP
        public void MoveCurrentCell(RowColumnIndex rowColumnIndex)
        {
            this.SelectionController.MoveCurrentCell(rowColumnIndex);
        }
#endif


        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when the Items Source are changed
        /// </summary>
        public event GridItemsSourceChangedEventHandler ItemsSourceChanged;

        /// <summary>
        /// Occurs when the sort columns are changing.
        /// </summary>
        public event GridSortColumnsChangingEventHandler SortColumnsChanging;

        /// <summary>
        /// Occurs when the sort columns are changed.
        /// </summary>
        public event GridSortColumnsChangedEventHandler SortColumnsChanged;

        /// <summary>
        /// Occurs when Selection changed. 
        /// </summary>
        /// <remarks></remarks>
        public event GridSelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when Selection changing. 
        /// </summary>
        /// <remarks>User can cancel the selection by setting args.Cancel is true</remarks>
        public event GridSelectionChangingEventHandler SelectionChanging;

#if !WP
        /// <summary>
        /// Occurs when Records are deleting from the Grid. 
        /// </summary>
        /// <remarks>User can cancel the selection by setting args.Cancel is true</remarks>
        public event RecordDeletingEventHandler RecordDeleting;

        /// <summary>
        /// Occurs when Records are deleted from the Grid. 
        /// </summary>
        /// <remarks>User can cancel the selection by setting args.Cancel is true</remarks>
        public event RecordDeletedEventHandler RecordDeleted;
#endif

        /// <summary>
        /// Occurs when Group Expanding
        /// </summary>
        /// <remarks>User can cancel the expanding operation by setting args.Cancel is true</remarks>
        public event GroupChangingEventHandler GroupExpanding;

        /// <summary>
        /// Occurs when Group Collapsing. 
        /// </summary>
        /// <remarks>User can cancel the collpasing operation by setting args.Cancel is true</remarks>
        public event GroupChangingEventHandler GroupCollapsing;

        /// <summary>
        /// Occurs when GroupExpanded. 
        /// </summary>
        /// <remarks></remarks>
        public event GroupChangedEventHandler GroupExpanded;

        /// <summary>
        /// Occurs when GroupCollapssed. 
        /// </summary>
        /// <remarks></remarks>
        public event GroupChangedEventHandler GroupCollapsed;

        /// <summary>
        ///  Occurs when the Grid columns are resizied
        /// </summary>
        public event ResizingColumnsEventHandler ResizingColumns;


        /// <summary>
        /// Occurs when UnboundColumn intializing and updating. 
        /// </summary>
        /// <remarks></remarks>
        public event QueryUnbounColumnValueHandler QueryUnboundColumnValue;


        /// <summary>
        /// Occurs when Column Drag and Drop functioning. 
        /// </summary>
        /// <remarks></remarks>
        public event QueryColumnDraggingEventHandler QueryColumnDragging;

#if !WP

        /// <summary>
        /// Occurs when Copy operation is performing. 
        /// </summary>
        /// <remarks></remarks>
        public event GridCopyPasteEventHandler GridCopyContent;

        /// <summary>
        /// Occurs when Paste operation is performing. 
        /// </summary>
        /// <remarks></remarks>
        public event GridCopyPasteEventHandler GridPasteContent;

        /// <summary>
        /// Occurs when the Filter gets changed. 
        /// </summary>
        /// <remarks></remarks>
        public event GridFilterEventHandler FilterChanged;

        /// <summary>
        /// Occurs when the filter gets changing. 
        /// </summary>
        /// <remarks></remarks>
        public event GridFilterEventHandler FilterChanging;

#if WPF
        /// <summary>
        /// Occurs when While the conext Menu open for various Grid Parts. 
        /// </summary>
        /// <remarks></remarks>
        public event GridContextMenuOpeningEventHandler GridContextMenuOpening;
#endif

        /// <summary>
        /// Occurs when the Items source for List populating. 
        /// </summary>
        /// <remarks></remarks>
        public event GridFilterItemsPopulatedEventHandler FilterItemsPopulated;

        /// <summary>
        /// Occurs when the Items source for List populating. 
        /// </summary>
        /// <remarks></remarks>
        public event GridFilterItemsPopulatingEventHandler FilterItemsPopulating;

        /// <summary>
        ///  Occurs when the Current Cell is Activating
        /// </summary>
        public event CurrentCellActivatingEventHandler CurrentCellActivating;

        /// <summary>
        /// Occurs when the Current Cell is Activated
        /// </summary>
        public event CurrentCellActivatedEventHandler CurrentCellActivated;

        /// <summary>
        /// Occurs when Editing is Initiated
        /// </summary>
        public event CurrentCellBeginEditEventHandler CurrentCellBeginEdit;

        /// <summary>
        /// Occurs When Editing is Completed
        /// </summary>
        public event CurrentCellEndEditEventHandler CurrentCellEndEdit;

        /// <summary>
        /// Occurs when current cell is validating
        /// </summary>
        public event CurrentCellValidatingEventHandler CurrentCellValidating;

        /// <summary>
        /// Occurs when current cell Selection Changed
        /// </summary>
        public event CurrentCellDropDownSelectionChangedEventHandler CurrentCellDropDownSelectionChanged;

        /// <summary>
        /// Occurs when current cell Request Navigation
        /// </summary>
        public event CurrentCellRequestNavigateEventHandler CurrentCellRequestNavigate;

        /// <summary>
        /// Occurs when current cell value Changed
        /// </summary>
        public event CurrentCellValueChangedEventHandler CurrentCellValueChanged;

        /// <summary>
        /// Occurs when current cell is validated
        /// </summary>
        public event CurrentCellValidatedEventHandler CurrentCellValidated;

        /// <summary>
        /// Occurs when the row lost it focus.
        /// </summary>
        public event RowValidatingEventHandler RowValidating;

        /// <summary>
        /// Occurs when the row is validated.
        /// </summary>
        public event RowValidatedEventHandler RowValidated;

        public event AddNewRowInitiatingEventHandler AddNewRowInitiating;
#else
        public event GridZoomingEventHandler GridZooming;

        public event GridZoomedEventHandler GridZoomed;
#endif

        public event AutoGeneratingColumnEventHandler AutoGeneratingColumn;

        #endregion

        #region Internal Event Helper methods

        /// <summary>
        /// Helper method to raise the SortColumnChanging event
        /// </summary>
        /// <param name="addedColumns"></param>
        /// <param name="removedColumns"></param>
        /// <param name="action"></param>
        /// <param name="cancelScroll">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseSortColumnsChanging(IList<SortColumnDescription> addedColumns, IList<SortColumnDescription> removedColumns, NotifyCollectionChangedAction action, out bool cancelScroll)
        {
            var args = new GridSortColumnsChangingEventArgs(addedColumns, removedColumns, action, this);
            RaiseSortColumnsChanging(args);
            cancelScroll = args.CancelScroll;
            return !args.Cancel;
        }

        internal void RaiseSortColumnsChanging(GridSortColumnsChangingEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.SortColumnsChanging != null)
                NotifyListener.RootDataGrid.RaiseSortColumnsChanging(args);
#endif
            if (this.SortColumnsChanging != null)
            {
                this.SortColumnsChanging(this, args);
            }
        }

        /// <summary>
        /// Helper method to raise the SortColumn changed method
        /// </summary>
        /// <param name="addedColumns"></param>
        /// <param name="removedColumns"></param>
        /// <param name="action"></param>
        /// <remarks></remarks>
        internal void RaiseSortColumnsChanged(IList<SortColumnDescription> addedColumns, IList<SortColumnDescription> removedColumns, NotifyCollectionChangedAction action)
        {
            var args = new GridSortColumnsChangedEventArgs(addedColumns, removedColumns, action, this);
            RaiseSortColumnsChanged(args);
        }

        internal void RaiseSortColumnsChanged(GridSortColumnsChangedEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.SortColumnsChanged != null)
                NotifyListener.RootDataGrid.RaiseSortColumnsChanged(args);
#endif
            if (this.SortColumnsChanged != null)
            {
                this.SortColumnsChanged(this, args);
            }
        }

        /// <summary>
        /// Method which is used to raise the ItemsSourceChanged Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridItemsSourceChangedEventArgs">GridItemsSourceChangedEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal void RaiseItemsSourceChangedEvent(GridItemsSourceChangedEventArgs args)
        {
#if !WP
            if (NotifyListener != null && IsRootDetailsViewGrid && NotifyListener.RootDataGrid.ItemsSourceChanged != null)
                NotifyListener.RootDataGrid.RaiseItemsSourceChangedEvent(args);
#endif
            if (this.ItemsSourceChanged != null)
            {
                ItemsSourceChanged(this, args);
            }
        }

        protected void RaiseItemsSourceChanged(object oldItemsSource, object newItemsSOurce)
        {
            var args = new GridItemsSourceChangedEventArgs(this, oldItemsSource, newItemsSOurce);
            this.RaiseItemsSourceChangedEvent(args);
        }

        /// <summary>
        /// Method which is used to raise the SelectionChanging Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridSelectionChangingEventArgs">GridSelectionChangingEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseSelectionChagingEvent(GridSelectionChangingEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.SelectionChanging != null)
                return NotifyListener.RootDataGrid.RaiseSelectionChagingEvent(args);
#endif
            if (this.SelectionChanging != null)
            {
                SelectionChanging(this, args);
                return args.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Method which is used to rasie the selection changed event.
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs">GridSelectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseSelectionChangedEvent(GridSelectionChangedEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.SelectionChanged != null)
                NotifyListener.RootDataGrid.RaiseSelectionChangedEvent(args);
#endif
            if (this.SelectionChanged != null)
            {
                SelectionChanged(this, args);
            }
        }

#if !WP
        /// <summary>
        /// Helper method to raise Current Cell Changing Event
        /// </summary>
        /// <param name="e"></param>
        internal bool RaiseCurrentCellActivatingEvent(CurrentCellActivatingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellActivating != null)
                return NotifyListener.RootDataGrid.RaiseCurrentCellActivatingEvent(e);
            if (this.CurrentCellActivating != null)
            {
                CurrentCellActivating(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Helper method to raise Current Cell Changed Event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseCurrentCellActivatedEvent(CurrentCellActivatedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellActivated != null)
                NotifyListener.RootDataGrid.RaiseCurrentCellActivatedEvent(e);
            if (this.CurrentCellActivated != null)
                CurrentCellActivated(this, e);
        }

        internal void RaiseCurrentCellValueChangedEvent(CurrentCellValueChangedEventArgs e)
        {
            if (CurrentCellValueChanged != null)
            {
                CurrentCellValueChanged(this, e);
            }
        }

        /// <summary>
        /// Helper method to raise Current Cell Begin Edit Event
        /// </summary>
        /// <param name="e"></param>
        internal bool RaiseCurrentCellBeginEditEvent(CurrentCellBeginEditEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellBeginEdit != null)
                return NotifyListener.RootDataGrid.RaiseCurrentCellBeginEditEvent(e);
            if (this.CurrentCellBeginEdit != null)
            {
                CurrentCellBeginEdit(this, e);
                return e.Cancel;
            }
            return false;
        }

#if !WP
        /// <summary>
        /// Methods which is used to raise RecordDeleting Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs">RecordDeletingEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseRecordDeletingEvent(RecordDeletingEventArgs args)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.RecordDeleting != null)
                return NotifyListener.RootDataGrid.RaiseRecordDeletingEvent(args);
            if (RecordDeleting == null) return false;
            RecordDeleting(this, args);
            return args.Cancel;
        }

        /// <summary>
        /// Method which is used to raise RecordDeleted Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs">RecordDeletedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseRecordDeletedEvent(RecordDeletedEventArgs args)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.RecordDeleted != null)
                NotifyListener.RootDataGrid.RaiseRecordDeletedEvent(args);
            if (RecordDeleted != null)
                RecordDeleted(this, args);
        }
#endif

        /// <summary>
        /// Helper method to raise Current Cell End Edit Event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseCurrentCellEndEditEvent(CurrentCellEndEditEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellEndEdit != null)
                NotifyListener.RootDataGrid.RaiseCurrentCellEndEditEvent(e);
            if (this.CurrentCellEndEdit != null)
            {
                CurrentCellEndEdit(this, e);
            }
        }

        #region Validation Events

        /// <summary>
        /// Helper method to raise Current Cell Validating Event
        /// </summary>
        internal bool RaiseCurrentCellValidatingEvent(CurrentCellValidatingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellValidating != null)
                return NotifyListener.RootDataGrid.RaiseCurrentCellValidatingEvent(e);
            if (this.CurrentCellValidating != null)
                CurrentCellValidating(this, e);
            return e.IsValid;
        }

        /// <summary>
        /// Helper method to raise Current Cell Validated Event
        /// </summary>
        internal void RaiseCurrentCellValidatedEvent(CurrentCellValidatedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.CurrentCellValidated != null)
                NotifyListener.RootDataGrid.RaiseCurrentCellValidatedEvent(e);
            if (this.CurrentCellValidated != null)
                CurrentCellValidated(this, e);
        }
        /// <summary>
        /// Helper method to raise Row validating event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs">RowValidatingEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal bool RaiseRowValidatingEvent(RowValidatingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.RowValidating != null)
                return NotifyListener.RootDataGrid.RaiseRowValidatingEvent(e);
            if (this.RowValidating != null)
                RowValidating(this, e);
            return e.IsValid;
        }

        /// <summary>
        ///  Helper method to raise Row validated event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs">RowValidatedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseRowValidatedEvent(RowValidatedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.RowValidated != null)
                NotifyListener.RootDataGrid.RaiseRowValidatedEvent(e);
            if (this.RowValidated != null)
                RowValidated(this, e);
        }

        #endregion


        internal void RaiseCurrentCellDropDownSelectionChangedEvent(CurrentCellDropDownSelectionChangedEventArgs e)
        {
            if (CurrentCellDropDownSelectionChanged != null)
                CurrentCellDropDownSelectionChanged(this, e);
        }

        internal bool CurrentCellRequestNavigateEvent(CurrentCellRequestNavigateEventArgs e)
        {
            if (CurrentCellRequestNavigate != null)
            {
                CurrentCellRequestNavigate(this, e);
                return e.Handled;
            }
            return false;
        }

#endif


        /// <summary>
        /// Helper method to raise UnboundColumn initializing and updating
        /// </summary>
        /// <param name="getAction">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="setAction">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="column"></param>
        /// <param name="cell"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal object RaiseQueryUnboundValue(UnBoundActions action, object value, GridColumn column, object record)
        {
            var args = new GridUnboundColumnEventsArgs(action, value, column, record, this);
            return RaiseQueryUnboundValue(args);
        }

        internal object RaiseQueryUnboundValue(GridUnboundColumnEventsArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.QueryUnboundColumnValue != null)
                return NotifyListener.RootDataGrid.RaiseQueryUnboundValue(args);
#endif
            if (this.QueryUnboundColumnValue != null)
            {
                this.QueryUnboundColumnValue(this, args);
            }
            return args.Value;
        }

        /// <summary>
        /// Helper method to raise Drag and Drop function.
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.QueryColumnDraggingEventArgs">QueryColumnDraggingEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseQueryColumnDragging(QueryColumnDraggingEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.QueryColumnDragging != null)
                return NotifyListener.RootDataGrid.RaiseQueryColumnDragging(args);
#endif
            if (this.QueryColumnDragging != null)
            {
                this.QueryColumnDragging(this, args);
            }
            return args.Cancel;
        }

        /// <summary>
        /// Helper method to raise the Group Expanding Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GroupChangingEventArgs">GroupChangingEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseGroupExpandingEvent(GroupChangingEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GroupExpanding != null)
                return NotifyListener.RootDataGrid.RaiseGroupExpandingEvent(args);
#endif
            if (this.GroupExpanding != null)
                this.GroupExpanding(this, args);
            return args.Cancel;
        }

        /// <summary>
        /// Helper method to raise the Group Collapsing Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GroupChangingEventArgs">GroupChangingEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool RaiseGroupCollapsingEvent(GroupChangingEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GroupCollapsing != null)
                return NotifyListener.RootDataGrid.RaiseGroupCollapsingEvent(args);
#endif
            if (this.GroupCollapsing != null)
                this.GroupCollapsing(this, args);
            return args.Cancel;
        }

        /// <summary>
        /// Helper method to raise the Group Expanded Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GroupChangedEventArgs">GroupChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseGroupExpandedEvent(GroupChangedEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GroupExpanded != null)
                NotifyListener.RootDataGrid.RaiseGroupExpandedEvent(args);
#endif
            if (this.GroupExpanded != null)
                this.GroupExpanded(this, args);
        }

        /// <summary>
        /// Helper method to raise the Group Collapsed Event
        /// </summary>
        /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GroupChangedEventArgs">GroupChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseGroupCollapsedEvent(GroupChangedEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GroupCollapsed != null)
                NotifyListener.RootDataGrid.RaiseGroupCollapsedEvent(args);
#endif
            if (this.GroupCollapsed != null)
                this.GroupCollapsed(this, args);
        }

        /// <summary>
        /// Helper method to raise the Resize Columns Event
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal bool RaiseResizingColumnsEvent(ResizingColumnsEventArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.ResizingColumns != null)
                return NotifyListener.RootDataGrid.RaiseResizingColumnsEvent(args);
#endif
            if (ResizingColumns != null)
                this.ResizingColumns(this, args);
            return args.Cancel;
        }




#if !WP

        internal GridCopyPasteEventArgs RaisePasteContentEvent(GridCopyPasteEventArgs args)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GridPasteContent != null)
                return NotifyListener.RootDataGrid.RaisePasteContentEvent(args);
            if (GridPasteContent != null)
                this.GridPasteContent(this, args);
            return args;
        }

        internal GridCopyPasteEventArgs RaiseCopyContentEvent(GridCopyPasteEventArgs args)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.GridCopyContent != null)
                return NotifyListener.RootDataGrid.RaiseCopyContentEvent(args);
            if (GridCopyContent != null)
                this.GridCopyContent(this, args);
            return args;
        }
        /// <summary>
        /// Helper method to raise the FilterChanging event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridFilterEventArgs">GridFilterEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal bool RaiseFilterChanging(GridFilterEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.FilterChanging != null)
                NotifyListener.RootDataGrid.RaiseFilterChanging(e);
            if (this.FilterChanging != null)
                this.FilterChanging(this, e);

            return e.Handled;
        }

        /// <summary>
        /// Helper method to raise the FilterChanged event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridFilterEventArgs">GridFilterEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseFilterChanged(GridFilterEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.FilterChanged != null)
                NotifyListener.RootDataGrid.RaiseFilterChanged(e);
            if (this.FilterChanged != null)
                this.FilterChanged(this, e);
        }

        /// <summary>
        /// Helper method to raise the FilterItemsPopulating event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridFilterItemsPopulatingEventArgs">GridFilterListItemsPopulatingEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal bool RaiseFilterListItemsPopulating(GridFilterItemsPopulatingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.FilterItemsPopulating != null)
                NotifyListener.RootDataGrid.RaiseFilterListItemsPopulating(e);
            if (this.FilterItemsPopulating != null)
                this.FilterItemsPopulating(this, e);

            return e.Handled;
        }

        /// <summary>
        /// Helper method to raise the FilterItemsPopulated event
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridFilterItemsPopulatedEventArgs">GridFilterListItemsPopulatingEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaiseFilterListItemsPopulated(GridFilterItemsPopulatedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.FilterItemsPopulated != null)
                NotifyListener.RootDataGrid.RaiseFilterListItemsPopulated(e);
            if (this.FilterItemsPopulated != null)
                this.FilterItemsPopulated(this, e);
        }
#else
        internal GridZoomingEventArgs RaiseZoomingEvent(GridZoomingEventArgs args)
        {
            if (GridZooming != null)
                this.GridZooming(this, args);
            return args;
        }

        internal GridZoomedEventArgs RaiseZoomedEvent(GridZoomedEventArgs args)
		{
            if (GridZoomed != null)
                this.GridZoomed(this, args);
            return args;
        }
#endif


        internal AutoGeneratingColumnArgs RaiseAutoGeneratingEvent(AutoGeneratingColumnArgs args)
        {
#if !WP
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.AutoGeneratingColumn != null)
                return NotifyListener.RootDataGrid.RaiseAutoGeneratingEvent(args);
#endif
            if (AutoGeneratingColumn != null)
                this.AutoGeneratingColumn(this, args);
            return args;
        }

#if WPF
        internal bool RaiseGridContextMenuEvent(GridContextMenuEventArgs e)
        {
            if (this.GridContextMenuOpening != null)
                this.GridContextMenuOpening(this, e);

            return e.Handled;
        }
#endif

#if !WP
        internal object RaiseAddNewRowInitiatingEvent(AddNewRowInitiatingEventArgs args)
        {
            if (AddNewRowInitiating != null)
                AddNewRowInitiating(this, args);
            return args.NewObject;
        }
#endif

        #endregion

        #region Private methods

        private void OnStackeColumnsChildChanged()
        {
            //this.RowGenerator.RefreshStackedHeaders();
            var headerRows = this.RowGenerator.Items.Where(row => row.RowRegion == RowRegion.Header);
            headerRows.ForEach(row =>
            {
                row.RowIndex = -1;
            });
            this.VisualContainer.InvalidateMeasure();
        }

        private void InitializeStackedColumnChildDelegate()
        {
            if (this.StackedHeaderRows == null)
                return;
            this.StackedHeaderRows.ForEach(row => row.StackedColumns.ForEach(col =>
            {
                if (col.ChildColumnChanged == null)
                    col.ChildColumnChanged = OnStackeColumnsChildChanged;
            }));
        }

        internal IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
#if WPF
                if (source is DataTable)
                {
                    result = ((DataTable)source).DefaultView;
                }
                else if (source is DataView)
                {
                    result = source as DataView;
                }
                else
#endif
                    result = source as IEnumerable;
            }
            return result;
        }

        private void InitialGroup()
        {
            if (this.GroupColumnDescriptions.Count <= 0)
                return;

            this.GridModel.Suspend();
            foreach (var column in this.GroupColumnDescriptions)
            {
                var isAvailable = true;
#if !WP
                if (!this.View.IsDynamicBound)
#else
                    isAvailable = CheckColumnNameinItemProperties(column.ColumnName);
#endif

                if (!isAvailable)
                    continue;
                
                if (!this.View.GroupDescriptions.Any(desc => (desc as PropertyGroupDescription).PropertyName == column.ColumnName))
                {
                    this.View.GroupDescriptions.Add(new PropertyGroupDescription(column.ColumnName, column.Converter));
                    if (this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == column.ColumnName) == default(SortDescription))
                        this.View.SortDescriptions.Add(new SortDescription(column.ColumnName, ListSortDirection.Ascending));
                }
                else
                    throw new InvalidOperationException("GroupColumnDescription already exist in DataGrid.GroupColumnDescriptions");
            }
            this.GridModel.Resume();
        }

        internal bool CheckColumnNameinItemProperties(string ColumnName)
        {
            if (this.View != null && this.Columns.Any(col => col.MappingName != null && col.MappingName.Equals(ColumnName)))
            {
                var provider = View.GetItemProperties();
#if !WP
                if (View.IsDynamicBound)
                {
                    if (View.Records.Count < 0) return false;
                    var dynObj = View.Records[0].Data as IDynamicMetaObjectProvider;
                    if (dynObj != null)
                    {
                        var metaType = dynObj.GetType();
                        var metaData = dynObj.GetMetaObject(System.Linq.Expressions.Expression.Parameter(metaType, metaType.Name));
                        if (!metaData.GetDynamicMemberNames().Contains(ColumnName))
                            return false;
                    }
                }
                else
#endif
                    if (!ColumnName.Contains("."))
                    {
                        if (provider.Find(ColumnName, false) == null)
                            return false;
                    }
                    else
                    {
                        var itemproperties = provider;
                        string[] propertyNameList = ColumnName.Split('.');
                        int complexPropertyCount = propertyNameList.Count();
                        for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                        {
                            var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                            if (tempProperyDescriptor != null)
                            {
#if WPF
                                itemproperties = TypeDescriptor.GetProperties(tempProperyDescriptor.PropertyType);
#else
                                itemproperties = new PropertyInfoCollection(tempProperyDescriptor.PropertyType);
#endif
                            }
                            else
                                return false;
                        }
                    }
                return true;
            }
            return false;
        }

        private void InitialSort()
        {
            if (this.SortColumnDescriptions.Count > 0)
            {
                foreach (var sortColumn in this.SortColumnDescriptions)
                {
                    if (sortColumn == null) throw new ArgumentNullException("Sort Column");

                    var isAvailable = true;
#if !WP
                    if (!this.View.IsDynamicBound)
#else
                        isAvailable = CheckColumnNameinItemProperties(sortColumn.ColumnName);
#endif

                    if (!isAvailable)
                        continue;

                    if (View.SortDescriptions.All(desc => desc.PropertyName != sortColumn.ColumnName))
                        View.SortDescriptions.Add(new SortDescription(sortColumn.ColumnName,
                                                                      sortColumn.SortDirection));
                    else
                        throw new InvalidOperationException("SortColumnDescription already exist in DataGrid.SortColumnDescriptions");
                }
            }

            if (this.SortComparers.Count > 0)
            {
                foreach (var sortcomparer in this.SortComparers)
                {
                    if (!View.SortComparers.Contains(sortcomparer))
                        View.SortComparers.Add(sortcomparer);
                }
            }
        }
        
        internal virtual void SetSourceList(object itemsSource)
        {
            this.UnWireEvents(); 
            CreateCollectionView(itemsSource);
            this.WireEvents();
            DeferRefresh();
        }

        internal void CreateCollectionView(object itemsSource)
        {
            this.View = this.CreateCollectionViewAdv(this.GetSourceList(itemsSource), this);

            if (this.AutoGenerateColumns && this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.Reset)
            {
                foreach (var item in this.Columns.Where(c => c.IsAutoGenerated).ToList())
                    this.Columns.Remove(item);
            }
            else if (this.AutoGenerateColumns && this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.ResetAll)
                this.Columns.Clear();

            if (View == null) return;

            if (VisualContainer != null && this.AutoGenerateColumns && ((!this.Columns.Any() && this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.RetainOld) || (this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.Reset || this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.ResetAll)))
                this.GenerateGridColumns();

#if !WP
            if (AutoGenerateRelations) 
                GenerateGridRelations();
#endif
            this.View.GroupComparer = this.SummaryGroupComparer;
            this.View.LiveDataUpdateMode = this.LiveDataUpdateMode;
            this.View.AutoExpandGroups = this.AutoExpandGroups;
#if !WP
            this.View.IsDynamicBound = this.IsDynamicItemsSource;
#endif

#if WPF
            (this.View as CollectionViewAdv).DispatchOwner = this.Dispatcher;
#endif
        }
        
        internal void DeferRefresh()
        {
            if (View == null)
                return;
            using (View.DeferRefresh())
            {
                InitialSort();
                InitialGroup();

                foreach (var summaryRow in this.GroupSummaryRows)
                {
                    this.View.SummaryRows.Add(summaryRow);
                }

                foreach (var summaryRow in this.TableSummaryRows)
                {
                    this.View.TableSummaryRows.Add(summaryRow);
                }

                if (this.CaptionSummaryRow != null)
                {
                    this.View.CaptionSummaryRow = this.CaptionSummaryRow;
                }
            }
        }

        internal ICollectionViewAdv CreateCollectionViewAdv(IEnumerable source, SfDataGrid dataGrid)
        {
            ICollectionViewAdv view = null;
            if (source != null)
            {
#if WPF
                if (!dataGrid.CheckIsLegacyDataTable(source))
                {
#endif
                    if (source is GridQueryableCollectionViewWrapper)
                        view = source as GridQueryableCollectionViewWrapper;
#if !WP
                    else if (source is Syncfusion.Data.PagedCollectionView)
                    {
                        view = source as Syncfusion.Data.PagedCollectionView;
                        if (view is GridPagedCollectionViewWrapper)
                            (view as GridPagedCollectionViewWrapper).SetDataGrid(this);
                    }
#endif
                    else if (source is VirtualizingCollectionView)
                    {
                        view = source as VirtualizingCollectionView;
                        if (view is GridVirtualizingCollectionView)
                            (view as GridVirtualizingCollectionView).SetDataGrid(this);
                        dataGrid.ShowBusyIndicator = true;
                    }
                    else
                        view = new GridQueryableCollectionViewWrapper(source, dataGrid);
                    if (dataGrid.SourceType != null)
                        (view as GridQueryableCollectionViewWrapper).SetSourceType(dataGrid.SourceType);

#if !SILVERLIGHT && !SyncfusionFramework3_5
                    if (view is GridQueryableCollectionViewWrapper)
                        ((GridQueryableCollectionViewWrapper)view).UsePLINQ = dataGrid.UsePLINQ;
#endif

#if WPF
                }
                else if (source is PagedCollectionView)
                {
                    view = source as PagedCollectionView;
                }
                else if (source is GridDataTableCollectionViewWrapper)
                {
                    view = source as GridDataTableCollectionViewWrapper;
                }
                else
                {
                    view = new GridDataTableCollectionViewWrapper(source, dataGrid);
                    if (dataGrid.SourceType != null)
                        (view as DataTableCollectionView).SetSourceType(dataGrid.SourceType);
                }
#endif
            }
            return view;
        }

#if !WP
        private void UpdateAndRefreshGrid(SerializableDataGrid dataGrid)
        {
            UnWireSerializablePropertyEvents();
            this.SelectedItems.Clear();
            this.FrozenColumnCount = 0;
            if (this.View != null)
            {
                this.View.BeginInit();
                this.View.SortDescriptions.Clear();
                this.View.SortComparers.Clear();
                this.View.GroupDescriptions.Clear();
                this.View.SummaryRows.Clear();
                this.View.TableSummaryRows.Clear();
                this.CaptionSummaryRow = null;
                this.View.EndInit();
            }
            if (this.GroupDropArea != null)
                this.GroupDropArea.RemoveAllGroupDropItems();
            if (this.RowGenerator != null)
            {
                this.RowGenerator.Items.ForEach(row => row.VisibleColumns.ForEach(col => this.RowGenerator.UnloadUIElements(row, col)));
                this.RowGenerator.Items.Clear();
            }
            if (this.VisualContainer != null)
            {
                this.VisualContainer.OnItemSourceChanged();
                this.VisualContainer.previousArrangeWidth = 0;
                this.VisualContainer.InvalidateMeasureInfo();
            }
            SerializationHelper.CopyToDataGrid(dataGrid, this);
            InitialSort();
            InitialGroup();
            this.UpdateRowAndColumnCount(true);
            WireSerializablePropertyEvents();
            this.Columns.ForEach(x => this.GridModel.WireColumnDescriptor(x));
        }
#endif
#if !WP
        private void RefreshFilterIconVisibility()
        {
            if (this.View is Syncfusion.Data.PagedCollectionView)
            {
                if ((this.View as Syncfusion.Data.PagedCollectionView).UseOnDemandPaging)
                    return;
            }
            if (this.VisualContainer != null && this.RowGenerator != null)
            {
                var headerRowBase = this.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.GetHeaderIndex());
                foreach (var column in this.Columns)
                {
                    if (headerRowBase != null)
                    {
                        var columnBase = (headerRowBase as DataRowBase).VisibleColumns.FirstOrDefault(col => col.GridColumn != null && col.GridColumn.MappingName.Equals(column.MappingName) && col.GridColumn.Equals(column));
                        if (columnBase != null && CanSetAllowFilters(column))
                            (columnBase.ColumnElement as GridHeaderCellControl).FilterIconVisiblity = Visibility.Visible;
                        else if (columnBase != null && !CanSetAllowFilters(column))
                            (columnBase.ColumnElement as GridHeaderCellControl).FilterIconVisiblity = Visibility.Collapsed;

                    }
                }
            }
        }
#endif
#if WPF
        internal bool CheckIsLegacyDataTable(IEnumerable source)
        {
            return source is DataTable || source is DataView || source is DataTableCollectionView;
        }

        internal bool IsLegacyDataTable
        {
            get
            {
                return this.ItemsSource is DataView || this.ItemsSource is DataTable;
            }
        }
#endif

        /// <summary>
        /// update cell style for given column index
        /// </summary>
        /// <param name="visibleColumnIndex"></param>
        /// <remarks></remarks>
        private void UpdateColumnCellStyle(int visibleColumnIndex)
        {
            this.RowGenerator.Items.ForEach(row => row.VisibleColumns.ForEach(col =>
            {
                if (col.ColumnIndex == visibleColumnIndex)
                {
                    col.UpdateCellStyle();
                }
            }));
        }

        /// <summary>
        /// update style for all cells except header cells
        /// </summary>
        /// <remarks></remarks>
        private void UpdateCellStyles()
        {
            this.RowGenerator.Items.ForEach(row => row.VisibleColumns.ForEach(col => col.UpdateCellStyle()));
        }

        private void UpdateSummariesCellStyle()
        {
            this.RowGenerator.Items.ForEach(row =>
            {
                if (row.RowType != RowType.DefaultRow)
                {
                    row.VisibleColumns.ForEach(col => col.UpdateCellStyle());
                }
            });
        }

        private void UpdateSummariesRowStyle()
        {
            this.RowGenerator.Items.ForEach(row =>
            {
                if (row.RowType != RowType.DefaultRow)
                {
                    row.UpdateRowStyles(row.WholeRowElement as ContentControl);
                }
            });
        }

        /// <summary>
        /// update column header cell style
        /// </summary>
        /// <param name="visibleColumnIndex"></param>
        /// <remarks></remarks>
        private void UpdateColumnHeaderStyle(int visibleColumnIndex)
        {
            this.RowGenerator.Items.ForEach(row =>
            {
                if (row.RowRegion == RowRegion.Header)
                {
                    row.VisibleColumns.ForEach(col =>
                    {
                        if (col.ColumnIndex == visibleColumnIndex)
                        {
                            col.UpdateCellStyle();
                        }
                    });
                }
            });
        }

        /// <summary>
        /// update row header row style
        /// </summary>
        /// <remarks></remarks>
        private void UpdateHeaderRowStyle()
        {
            this.RowGenerator.Items.ForEach(row =>
            {
                if (row.RowRegion == RowRegion.Header)
                {
                    row.VisibleColumns.ForEach(col => col.UpdateCellStyle());
                }
            });
        }

        /// <summary>
        /// update row styles for grid
        /// </summary>
        /// <remarks></remarks>
        private void UpdateRowStyle()
        {
            this.RowGenerator.Items.ForEach(row => row.UpdateRowStyles(row.WholeRowElement as ContentControl));
        }

        private void UpdateValidationMode()
        {
            foreach (GridColumn column in this.Columns)
            {
                column.UpdateBindingForValidation(this.GridValidationMode);
            }

            foreach (var item in this.VisualContainer.RowsGenerator.Items)
            {
                var row = item as DataRowBase;
                if (this.GridValidationMode == Grid.GridValidationMode.None)
                {
                    row.VisibleColumns.ForEach(datacolumn =>
                    {
                        if (row.RowIndex >= this.HeaderLineCount && datacolumn.ColumnElement is GridCell)
                        {
                            (datacolumn.ColumnElement as GridCell).RemoveError();
                            datacolumn.UpdateBinding(row.RowData);
                        }
                    });
                }
                else
                {
#if !WP
                    this.Validations.ValidateColumns(row);
#endif
                    row.VisibleColumns.ForEach(datacolumn =>
                    {
                        if (row.RowIndex >= this.HeaderLineCount)
                            datacolumn.UpdateBinding(row.RowData);

                    });
                }
            }
        }


#if !SILVERLIGHT && !WP
        /// <summary>
        /// update style for given column
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        internal void OnColumnStyleChanged(GridColumn column)
        {
            var visibleColumnIndex = this.ResolveToScrollColumnIndex(this.Columns.IndexOf(column));
            if (column.CellStyle != null || column.CellStyleSelector != null)
            {
                this.UpdateColumnCellStyle(visibleColumnIndex);
            }
            else if ((column is GridTemplateColumn && (column as GridTemplateColumn).CellTemplate != null) ||
                     ((column is GridTemplateColumn && (column as GridTemplateColumn).CellTemplateSelector != null)))
            {
                this.UpdateColumnCellStyle(visibleColumnIndex);
            }
            if (column.HeaderStyle != null || column.HeaderTemplate != null)
            {
                this.UpdateColumnHeaderStyle(visibleColumnIndex);
            }
        }

#else
    /// <summary>
    /// update style for given column
    /// </summary>
    /// <param name="column"></param>
    /// <remarks></remarks>
        internal void OnColumnStyleChanged(GridColumn column)
        {
            var visibleColumnIndex = this.ResolveToScrollColumnIndex(this.Columns.IndexOf(column));
            if (column.CellStyle != null)
            {
                this.UpdateColumnCellStyle(visibleColumnIndex);
            }
            else if (column is GridTemplateColumn && (column as GridTemplateColumn).CellTemplate != null)
            {
                this.UpdateColumnCellStyle(visibleColumnIndex);
            }
            if (column.HeaderStyle != null || column.HeaderTemplate != null)
            {
                this.UpdateColumnHeaderStyle(visibleColumnIndex);
            }
        }

#endif

        internal void GenerateGridColumns()
        {
#if WP
            suspendForColumnPopulation = true;
            if (this.Columns != null && this.View != null)
            {    
                var propertyCollection = this.View.GetItemProperties();
                foreach (var keyvaluepair in propertyCollection)
                {
                    var propertyinfo = keyvaluepair.Value;
                    if (!Columns.Any(c => c.MappingName == propertyinfo.Name))
                    {
                        var column = CreateColumn(propertyinfo);
                        if (column != null)
                        {
                            var args = RaiseAutoGeneratingEvent(new AutoGeneratingColumnArgs(column, this));
                            if (!args.Cancel)
                                this.Columns.Add(column);
                        }
                    }
                }
            }
            suspendForColumnPopulation = false;
#else
            if (this is DetailsViewDataGrid && this.NotifyListener != null)
            {
                if (this.NotifyListener.RootDataGrid.Columns.Count == 0)
                {
                    this.NotifyListener.RootDataGrid.GenerateGridColumns(this.NotifyListener.RootDataGrid.Columns, this.View);
                    suspendForColumnPopulation = true;
                    CloneHelper.CloneCollection(this.NotifyListener.RootDataGrid.Columns, this.Columns, typeof(GridColumn));
                    suspendForColumnPopulation = false;
                }
            }
            else
            {
                GenerateGridColumns(this.Columns, this.View);
            }
#endif
        }
#if !WP
        private void GenerateGridColumns(Columns columns, ICollectionViewAdv view)
        {
            suspendForColumnPopulation = true;
            if (columns != null && view != null)
            {
                if (view.IsDynamicBound)
                {
                    this.GenerateGridColumnsForDynamic(columns, view);
                    suspendForColumnPopulation = false;
                    return;
                }
                var columnDisplayOrder = new List<KeyValuePair<int, GridColumn>>();
                var columnsWithoutOrder = new List<GridColumn>();
                var propertyCollection = view.GetItemProperties();
                if (StackedHeaderRows.Count<=0)
                    CreateStackedHeader(propertyCollection);
#if !WPF
                foreach (var keyvaluepair in propertyCollection)
                {
                    var propertyinfo = keyvaluepair.Value;
                    var displayAttribute = propertyinfo.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
#if !WinRT
                    var attribute = (System.ComponentModel.BindableAttribute)propertyinfo.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.BindableAttribute));
#endif
#else
                foreach (PropertyDescriptor propertyinfo in propertyCollection)
                {
                    var displayAttribute = propertyinfo.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
                    var attribute = propertyinfo.Attributes.ToList<Attribute>().FirstOrDefault(a => a is BindableAttribute);
#endif
#if !WinRT
                    if (attribute != null)
                    {
                        var bindableAttribute = attribute as BindableAttribute;
                        if (!bindableAttribute.Bindable)
                            continue;
                    }
#endif
                    var cancel = false;
                    if (displayAttribute != null && columns.All(c => c.MappingName != propertyinfo.Name))
                    {
                        var canGenerate = displayAttribute.GetAutoGenerateField();
                        if (canGenerate.HasValue && !canGenerate.Value)
                            continue;
                        var column = CreateColumn(propertyinfo, displayAttribute, out cancel);
                        if (column != null)
                        {
                            var displayOrder = displayAttribute.GetOrder();
                            if (displayOrder.HasValue)
                            {
                                var args = RaiseAutoGeneratingEvent(new AutoGeneratingColumnArgs(column, this) { Cancel = cancel });
                                if (!args.Cancel)
                                    columnDisplayOrder.Add(new KeyValuePair<int, GridColumn>(displayOrder.Value, args.Column));
                            }
                            else
                            {
                                var args = RaiseAutoGeneratingEvent(new AutoGeneratingColumnArgs(column, this) { Cancel = cancel });
                                if (!args.Cancel)
                                    columnsWithoutOrder.Add(args.Column);
                            }
                        }
                    }
                    else
                    {
                        if (columns.All(c => c.MappingName != propertyinfo.Name))
                        {
                            var column = CreateColumn(propertyinfo, null, out cancel);
                            if (column != null)
                            {
                                var args = RaiseAutoGeneratingEvent(new AutoGeneratingColumnArgs(column, this) { Cancel = cancel });
                                if (!args.Cancel)
                                    columnsWithoutOrder.Add(args.Column);
                            }
                        }
                    }
                }
                foreach (var keyValue in columnDisplayOrder.OrderBy(o => o.Key))
                    columns.Add(keyValue.Value);
                foreach (var column in columnsWithoutOrder)
                    columns.Add(column);
            }
            suspendForColumnPopulation = false;
        }
#if WPF
        private void CreateStackedHeader(PropertyDescriptorCollection propertyCollection)
#else
        private void CreateStackedHeader(PropertyInfoCollection propertyCollection)
#endif
        {
            var listOfStackedRows = new List<StackedHeaderRow>();
#if WPF
            foreach (PropertyDescriptor propertyinfo in propertyCollection)
            {
                var displayAttribute = propertyinfo.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
#else
            foreach (var keyvaluepair in propertyCollection)
            {
                var propertyinfo = keyvaluepair.Value;
                var displayAttribute = propertyinfo.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
#endif
                if (displayAttribute != null && displayAttribute.GroupName!=null)
                {
#if !WinRT
                    ResourceManager displayAttributeResourceManager = null;
#else
                    ResourceLoader displayAttributeResourceLoader=null;
#endif
                    if (displayAttribute.ResourceType != null)
                    {
#if !WinRT
                        var resourceName = displayAttribute.ResourceType.FullName.Replace('_', '.');
                        Assembly assembly = displayAttribute.ResourceType.Assembly;
                        displayAttributeResourceManager = new ResourceManager(resourceName, assembly);
#else
                        var resourceName = displayAttribute.ResourceType.Name;
                        displayAttributeResourceLoader = new ResourceLoader(resourceName);
#endif
                    }
                    string groupName = string.Empty;
#if !WinRT
                    if (displayAttributeResourceManager != null)
                        groupName = displayAttributeResourceManager.GetString(displayAttribute.GroupName, System.Globalization.CultureInfo.CurrentCulture);
#else
                    if (displayAttributeResourceLoader != null)
                        groupName = displayAttributeResourceLoader.GetString(displayAttribute.GroupName);
#endif
                    else
                        groupName = displayAttribute.GroupName;
                    if (groupName != null)
                    {
                        if (groupName.Contains("/"))
                        {
                            var headerName = groupName.Split('/');
                            var index = 0;
                            for (int i = headerName.Length - 1; i >= 0; i--)
                            {
                                var headerGroupName = headerName[i];
                                StackedHeaderRow stackedHeaderRow;
                                if (listOfStackedRows.Count > index)
                                {
                                    stackedHeaderRow = listOfStackedRows[index];
                                }
                                else
                                {
                                    stackedHeaderRow = new StackedHeaderRow();
                                    listOfStackedRows.Add(stackedHeaderRow);
                                }

                                var stackedColumn = stackedHeaderRow.StackedColumns.FirstOrDefault(column => column.HeaderText == headerGroupName);
                                if (stackedColumn == null)
                                {
                                    stackedColumn = new StackedColumn { HeaderText = headerGroupName, ChildColumns = propertyinfo.Name };
                                    stackedHeaderRow.StackedColumns.Add(stackedColumn);
                                }
                                else
                                    stackedColumn.ChildColumns += "," + propertyinfo.Name;
                                index++;
                            }
                        }
                        else
                        {
                            var index = 0;
                            StackedHeaderRow stackedHeaderRow;
                            if (listOfStackedRows.Count > index)
                            {
                                stackedHeaderRow = listOfStackedRows[index];
                            }
                            else
                            {
                                stackedHeaderRow = new StackedHeaderRow();
                                listOfStackedRows.Add(stackedHeaderRow);
                            }
                            var stackedColumn = stackedHeaderRow.StackedColumns.FirstOrDefault(column => column.HeaderText == groupName);
                            if (stackedColumn == null)
                            {
                                stackedColumn = new StackedColumn { HeaderText = groupName, ChildColumns = propertyinfo.Name };
                                stackedHeaderRow.StackedColumns.Add(stackedColumn);
                            }
                            else
                                stackedColumn.ChildColumns += "," + propertyinfo.Name;
                        }
                    }
                }
            }

            if (listOfStackedRows.Any())
            {
                var stackedHeaderRows = new StackedHeaderRows();
                for (var i = listOfStackedRows.Count -1; i >= 0; i--)
                {
                    var stackedHeaderRow = listOfStackedRows[i];
                    stackedHeaderRows.Add(stackedHeaderRow);
                }
                this.StackedHeaderRows = stackedHeaderRows;
            }
        }
#endif

#if WPF
        private GridColumn CreateColumn(PropertyDescriptor propertyinfo, DisplayAttribute displayAttribute, out bool cancelArgs)
#elif !WP
        private GridColumn CreateColumn(PropertyInfo propertyinfo, DisplayAttribute displayAttribute, out bool cancelArgs)
#else
        private GridColumn CreateColumn(PropertyInfo propertyinfo)
#endif
        {
#if !WP
            cancelArgs = false;
#if WPF
            bool isReadOnly = propertyinfo.IsReadOnly;
#else
            bool isReadOnly = !propertyinfo.CanWrite;
#endif
#else
            var cancelArgs = false;
#endif
            var canAddColumn = propertyinfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyinfo.PropertyType)
#if !WinRT
 && !(propertyinfo.PropertyType.IsArray && propertyinfo.PropertyType.GetElementType().IsPrimitive);
#else
            && !(propertyinfo.PropertyType.IsArray && propertyinfo.PropertyType.GetElementType().IsPrimitive());
#endif
            if (canAddColumn)
                cancelArgs = true;

            canAddColumn = (!GridUtil.IsComplexType(propertyinfo.PropertyType) || typeof(byte[]).IsAssignableFrom(propertyinfo.PropertyType));
            if (!canAddColumn)
                cancelArgs = true;

            if (cancelArgs)
#if !WP
                return new GridTextColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, AllowEditing = !isReadOnly };
#else
                return null;
#endif

            GridColumn column = null;
            var dataType = false;
#if WPF || SL
            var datatypeAttribute = propertyinfo.Attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (datatypeAttribute != null)
            {
                switch (datatypeAttribute.DataType)
                {
                    case DataType.PhoneNumber:
                        dataType = true;
                        column = new GridMaskColumn
                            {
                                MappingName = propertyinfo.Name,
                                Mask = "(999)999-9999",
                                HeaderText = propertyinfo.Name
                            };
                        break;
                    case DataType.Currency:
                        dataType = true;
                        column = new GridCurrencyColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name };
                        break;
                }
            }
#endif
            if (!dataType)
            {

                //Issue Fix: WRT-1291
                //If the Bound value is of type Object 'IsAssignableFrom(typeof(DateTime))' condition passes for any of the conditions and DateTimeColumn is generated (Since it is the first if clause) when AutoGenerateColumns=True
                //Hence we have checked for object type as first condition and generated it as TextColumn with AllowEditing as False
                if (propertyinfo.PropertyType.IsAssignableFrom(typeof(System.Object)))
                {
#if !WP
                    column = new GridTextColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, AllowEditing = false };
#else
                    column = new GridTextColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name};
#endif
                }
#if !WP
                else if (propertyinfo.PropertyType.IsAssignableFrom(typeof(DateTime)) || propertyinfo.PropertyType.IsAssignableFrom(typeof(DateTime?)))
                {
#if !WinRT
                    column = new GridDateTimeColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, AllowEditing = !isReadOnly, AllowNullValue = propertyinfo.PropertyType.IsAssignableFrom(typeof(DateTime?)) };
#else
                    column = new GridDateTimeColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name };
#endif
                }

                else if (propertyinfo.PropertyType.IsAssignableFrom(typeof(bool)) || propertyinfo.PropertyType.IsAssignableFrom(typeof(bool?)))
#else
                else if (propertyinfo.PropertyType.IsAssignableFrom(typeof(bool)) || propertyinfo.PropertyType.IsAssignableFrom(typeof(bool?)))
#endif
                {
#if !WP
                    column = new GridCheckBoxColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, AllowEditing = !isReadOnly, IsThreeState = propertyinfo.PropertyType.IsAssignableFrom(typeof(bool?)) };
#else
                    column = new GridCheckBoxColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, IsThreeState = propertyinfo.PropertyType.IsAssignableFrom(typeof(bool?)) };
#endif
                }
#if !WP
                else if (propertyinfo.PropertyType.IsAssignableFrom(typeof(int)) || propertyinfo.PropertyType.IsAssignableFrom(typeof(int?)))
                {
#if WinRT
                    column = new GridNumericColumn
                    {
                        MappingName = propertyinfo.Name,
                        HeaderText = propertyinfo.Name,
                        AllowEditing = !isReadOnly,
                        AllowNullInput = propertyinfo.PropertyType.IsAssignableFrom(typeof(int?))
                    };
#else
                    column = new GridNumericColumn
                        {
                            MappingName = propertyinfo.Name,
                            HeaderText = propertyinfo.Name,
                            AllowEditing = !isReadOnly,
                            AllowNullValue = propertyinfo.PropertyType.IsAssignableFrom(typeof(int?)),
                            NumberDecimalDigits = 0
                        };
#endif
                }
                else if (propertyinfo.PropertyType.IsAssignableFrom(typeof(double)) || propertyinfo.PropertyType.IsAssignableFrom(typeof(double?)))
                {
#if WinRT
                    column = new GridNumericColumn
                    {
                        MappingName = propertyinfo.Name,
                        HeaderText = propertyinfo.Name,
                        AllowNullInput = propertyinfo.PropertyType.IsAssignableFrom(typeof(double?)),
                        AllowEditing = !isReadOnly,
                        ParsingMode = Parsers.Double
                    };
#else
                    column = new GridNumericColumn
                        {
                            MappingName = propertyinfo.Name,
                            HeaderText = propertyinfo.Name,
                            AllowEditing = !isReadOnly,
                            AllowNullValue = propertyinfo.PropertyType.IsAssignableFrom(typeof(double?)),
                        };
#endif
                }
#endif
                else
#if !WP
                    column = new GridTextColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name, AllowEditing = !isReadOnly, };
#else
                    column = new GridTextColumn { MappingName = propertyinfo.Name, HeaderText = propertyinfo.Name};
#endif
            }
#if !WP
            if (displayAttribute != null)
            {
#if !WinRT
                ResourceManager displayAttributeResourceManager = null;
#else
                ResourceLoader displayAttributeResourceLoader = null;
#endif
                if (displayAttribute.ResourceType != null)
                {
#if !WinRT
                    var resourceName = displayAttribute.ResourceType.FullName.Replace('_', '.');
                    Assembly assembly = displayAttribute.ResourceType.Assembly;
                    displayAttributeResourceManager = new ResourceManager(resourceName, assembly);
#else
                    var resourceName = displayAttribute.ResourceType.Name;
                    displayAttributeResourceLoader = new ResourceLoader(resourceName);
#endif
                }

                if (displayAttribute.Name != null)
                {
#if !WinRT
                    if (displayAttributeResourceManager != null)
                        column.HeaderText = displayAttributeResourceManager.GetString(displayAttribute.Name, System.Globalization.CultureInfo.CurrentCulture);
#else
                    if (displayAttributeResourceLoader != null)
                        column.HeaderText = displayAttributeResourceLoader.GetString(displayAttribute.Name);
#endif
                    else
                        column.HeaderText = displayAttribute.Name;
                }
                if (displayAttribute.ShortName != null)
                {
#if !WinRT
                    if (displayAttributeResourceManager != null)
                        column.HeaderText = displayAttributeResourceManager.GetString(displayAttribute.ShortName, System.Globalization.CultureInfo.CurrentCulture);
#else
                    if (displayAttributeResourceLoader != null)
                        column.HeaderText = displayAttributeResourceLoader.GetString(displayAttribute.ShortName);
#endif
                    else
                        column.HeaderText = displayAttribute.ShortName;
                }
                if (displayAttribute.GetAutoGenerateFilter() != null)
                    column.AllowFiltering = displayAttribute.GetAutoGenerateFilter().Value;
                if (displayAttribute.Description != null)
                {
                    string description = null;
#if !WinRT
                    if (displayAttributeResourceManager != null)
                        description = displayAttributeResourceManager.GetString(displayAttribute.Description, System.Globalization.CultureInfo.CurrentCulture);
#else
                    if (displayAttributeResourceLoader != null)
                        description = displayAttributeResourceLoader.GetString(displayAttribute.Description);
#endif
                    else
                        description = displayAttribute.Description.ToString();
                    if (description != null)
                    {
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append("<DataTemplate ");
                        strBuilder.Append("xmlns='http://schemas.microsoft.com/winfx/");
                        strBuilder.Append("2006/xaml/presentation' ");
                        strBuilder.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                        strBuilder.Append("> <TextBlock ");
                        strBuilder.Append("Text='");
                        strBuilder.Append(description);
                        strBuilder.Append("' />");
                        strBuilder.Append("</DataTemplate>");
#if WPF
                        var dt = (DataTemplate)XamlReader.Load(ToStream(strBuilder.ToString()));
#else
                        var dt = (DataTemplate)XamlReader.Load(strBuilder.ToString());
#endif
                        column.HeaderToolTipTemplate = dt;
                    }
                }

            }
#if WPF
            var editableAttribute = propertyinfo.Attributes.OfType<EditableAttribute>().FirstOrDefault();
#else
            var editableAttribute = propertyinfo.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
#endif
            if (editableAttribute != null)
            {
                if (editableAttribute.AllowEdit)
                {
                    this.AllowEditing = true;
                    this.NavigationMode = NavigationMode.Cell;
                    column.AllowEditing = true;
                }
                else
                    column.AllowEditing = false;
            }
#endif
            column.IsAutoGenerated = true;
            return column;
        }


#if WPF
        public Stream ToStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
#endif

#if !WP
        private void GenerateGridColumnsForDynamic(Columns columns, ICollectionViewAdv view)
        {
            if (view.Records.Count < 0)
                return;
            var dynObj = view.Records[0].Data as IDynamicMetaObjectProvider;
            if (dynObj != null)
            {
                var metaType = dynObj.GetType();
                var metaData =
                    dynObj.GetMetaObject(System.Linq.Expressions.Expression.Parameter(metaType, metaType.Name));

                foreach (var prop in metaData.GetDynamicMemberNames())
                {
#if !WPF
                    if (DynamicHelper.IsPythonType(prop))
#else
                    if (DynamicHelper.IsPythonType(prop) || DynamicHelper.IsComplexCollection(dynObj, prop))
#endif
                    {
                        continue;
                    }
                    if (columns.Any(c => c.MappingName == prop)) continue;
                    GridColumn column = new GridTextColumn
                        {
                            MappingName = prop,
                            HeaderText = prop
                        };
                    column.IsAutoGenerated = true;
                    var args = RaiseAutoGeneratingEvent(new AutoGeneratingColumnArgs(column, this));
                    if (!args.Cancel)
                        columns.Add(args.Column);
                }
            }
        }
#endif

        /// <summary>
        /// Method which will initiate the Cell Renderers Collection.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeCellRendererCollection()
        {
            cellRenderers.Add("Static", new GridCellTextBlockRenderer());
            cellRenderers.Add("Header", new GridDataHeaderCellRenderer());
            cellRenderers.Add("StackedHeader", new GridStackedHeaderCellRenderer());
            cellRenderers.Add("GroupSummary", new GridSummaryCellRenderer());
            cellRenderers.Add("CaptionSummary", new GridCaptionSummaryCellRenderer());
            cellRenderers.Add("TableSummary", new GridTableSummaryCellRenderer());
            cellRenderers.Add("TextBlock", new GridCellTextBlockRenderer());
            cellRenderers.Add("TextBox", new GridCellTextBoxRenderer());
            cellRenderers.Add("CheckBox", new GridCellCheckBoxRenderer());
            cellRenderers.Add("Template", new GridCellTemplateRenderer());
            cellRenderers.Add("Image", new GridCellImageRenderer());
            cellRenderers.Add("UnBoundColumn", new GridUnBoundCellRenderer());
            cellRenderers.Add("RowHeader", new GridRowHeaderCellRenderer());
#if !WP
            cellRenderers.Add("DetailsViewExpander", new GridDetailsViewExpanderCellRenderer());
            cellRenderers.Add("MultiColumnDropDown", new GridCellMultiColumnDropDownRenderer());
            cellRenderers.Add("ComboBox", new GridCellComboBoxRenderer());
            cellRenderers.Add("Numeric", new GridCellNumericRenderer());
            cellRenderers.Add("DateTime", new GridCellDateTimeRenderer());
            cellRenderers.Add("Hyperlink", new GridCellHyperlinkRenderer());
#if WinRT
            cellRenderers.Add("UpDown", new GridCellUpDownRenderer());
#endif
#if !WinRT
            cellRenderers.Add("Currency", new GridCellCurrencyRenderer());
            cellRenderers.Add("Percent", new GridCellPercentageRenderer());
            cellRenderers.Add("TimeSpan", new GridCellTimeSpanRenderer());
            cellRenderers.Add("Mask", new GridCellMaskRenderer());
#endif
#endif
        }

        /// <summary>
        /// Method which is helps to reset the selection based values.
        /// </summary>
        /// <remarks></remarks>
        private void ResetSelectionValues()
        {
            if (this.SelectionController != null)
                this.SelectionController.ClearSelections(false);
        }

        /// <summary>
        /// Method which is helps to hook the CollectionView based events.
        /// </summary>
        /// <remarks></remarks>
        private void WireViewEvents()
        {
            //var virtualView = View as VirtualizingCollectionView;
            //if (View == null || (View.SourceCollection == null && virtualView == null)) return;
            //if (this.View.SourceCollection is INotifyCollectionChanged)
            //{
            //    var notifyCollectionchanged = this.View.SourceCollection as INotifyCollectionChanged;
            //    notifyCollectionchanged.CollectionChanged += OnSourceCollectionChanged;
            //}
        }

        /// <summary>
        /// Method which is helps to hook the SfDataGrid based events.
        /// </summary>
        /// <remarks></remarks>
        private void WireDataGridEvents()
        {
            this.SizeChanged += OnSizeChanged;
#if WPF
            this.Loaded += OnLoaded;
#endif
        }

#if WPF
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.RowGenerator.Items.Count > 0)
            {
                this.GridModel.UpdateHeaderCells(false);
            }
        }
#endif

        /// <summary>
        /// Called when [size changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != 0)
            {
                this.GridColumnSizer.RefreshAll();
                return;
            }
            if (container != null)
                container.InvalidateMeasure();
        }

        /// <summary>
        /// GridColumns collection changed event handling
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private void OnGridColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (GridModel != null)
                this.GridModel.OnGridColumnCollectionChanged(sender, e);
            if (suspendForColumnPopulation)
                return;
#if !WP
            if (this.NotifyListener != null) this.NotifyListener.NotifyCollectionChanged(this.Columns, e, datagrid => datagrid.Columns, this, typeof(GridColumn));
#endif
            //  In detailsview grid, if column is added at runtime, visual container will be null. So visual container check is moved here.
            if (!this.isGridLoaded)
                return;

            var newStartIndex = this.ResolveToScrollColumnIndex(e.NewStartingIndex);
            var oldStartIndex = this.ResolveToScrollColumnIndex(e.OldStartingIndex);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count > 0)
                    {
                        HasUnboundColumns = e.NewItems[0] is GridUnBoundColumn;
                    }
#if !WinRT
                    if (!this.suspendForColumnMove)
#endif
                        this.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.ColumnsCollection);
#if WinRT
                    this.container.InsertColumns(newStartIndex, 1);
#else
                    if (!this.suspendForColumnMove)
                        this.container.InsertColumns(newStartIndex, 1);
                    else
                    {
#if !SILVERLIGHT && !WP
                        goto case NotifyCollectionChangedAction.Move;
#else
                        if (this.suspendForColumnMove)
                            oldStartIndex = this.oldIndexForMove;
                        this.container.RemoveColumns(oldStartIndex, 1);
                        this.container.InsertColumns(newStartIndex, 1);
#endif
                    }
#endif
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
#if !WinRT
                        if (!this.suspendForColumnMove)
                        {
#endif
                            var column = e.OldItems[0] as GridColumn;

                            this.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.ColumnsCollection);
                            this.container.RemoveColumns(oldStartIndex, 1);

                            if (this.GroupColumnDescriptions.Any(desc => desc.ColumnName == column.MappingName))
                            {
                                this.GroupColumnDescriptions.Remove(
                                    this.GroupColumnDescriptions.FirstOrDefault(
                                        desc => desc.ColumnName == column.MappingName));
                                this.GroupDropArea.RemoveGroupDropItem(column);
                            }
                            if (this.SortColumnDescriptions.Any(desc => desc.ColumnName == column.MappingName))
                                this.SortColumnDescriptions.Remove(
                                    this.SortColumnDescriptions.FirstOrDefault(
                                        desc => desc.ColumnName == column.MappingName));
#if !WinRT
                        }
                        else
                            this.oldIndexForMove = oldStartIndex;
#endif
                        HasUnboundColumns = this.Columns.Any(col => col.IsUnbound);

                    }
                    break;

#if !SILVERLIGHT && !WP
                case NotifyCollectionChangedAction.Move:
                    //We should not refresh the ColumnSizer when removing the Column
                    //this.container.RemoveColumns(oldStartIndex, 1, true);
#if !WinRT
                    if (this.suspendForColumnMove)
                        oldStartIndex = this.oldIndexForMove;
#endif
                    this.container.RemoveColumns(oldStartIndex, 1);
                    this.container.InsertColumns(newStartIndex, 1);
#if !WinRT
                    var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems[0], this.ResolveToGridVisibleColumnIndex(newStartIndex), this.ResolveToGridVisibleColumnIndex(oldStartIndex));
                    this.SelectionController.HandleCollectionChanged(args, CollectionChangedReason.ColumnsCollection);
#else
                    this.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.ColumnsCollection);
#endif
                    break;
#endif
                case NotifyCollectionChangedAction.Reset:
                    this.container.RemoveColumns(0, container.ColumnCount);
                    if (this.GroupColumnDescriptions.Count > 0)
                        this.GroupColumnDescriptions.Clear();
                    if (this.SortColumnDescriptions.Count > 0)
                        this.SortColumnDescriptions.Clear();
                    if (this.GroupDropArea != null)
                        this.GroupDropArea.RemoveAllGroupDropItems();
                    HasUnboundColumns = false;
                    break;
            }
            this.container.UpdateScrollBars();
            this.container.NeedToRefreshColumn = true;
            this.container.InvalidateMeasure();
            if (this.VisualContainer.ColumnCount > 0)
            {
                this.RowGenerator.RefreshStackedHeaders();
#if !WinRT
                if (!suspendForColumnMove)
#endif
                    this.GridColumnSizer.RefreshAll();
                if (this.AllowResizingHiddenColumns && this.AllowResizingColumns)
                {
                    this.GridColumnResizingController.EnsureVSMOnColumnCollectionChanged(e.OldStartingIndex, e.NewStartingIndex);
                }
            }
        }


        /// <summary>
        /// Method which is helps to Unhook the CollectionView based events.
        /// </summary>
        /// <remarks></remarks>
        private void UnWireViewEvents()
        {
            // Commented due to issue in WP
            //if (View == null || View.SourceCollection == null) return;

            //if (this.View.SourceCollection is INotifyCollectionChanged)
            //{
            //    var notifyCollectionchanged = this.View.SourceCollection as INotifyCollectionChanged;
            //    notifyCollectionchanged.CollectionChanged -= OnSourceCollectionChanged;
            //}
        }

        /// <summary>
        /// Method which is helps to Unhook the SfDataGrid based events.
        /// </summary>
        /// <remarks></remarks>
        private void UnWireDataGridEvents()
        {
            this.SizeChanged -= OnSizeChanged;
#if WPF
            this.Loaded -= OnLoaded;
#endif
        }

        /// <summary>
        /// Method to set the values for which is initialize before the Grid comes to View.
        /// </summary>
        /// <remarks></remarks>
        protected void EnsureProperties()
        {
            if (IsChanged(SfDataGrid.RowHeightProperty))
            {
                this.VisualContainer.RowHeights.DefaultLineSize = this.RowHeight;
                if (this.VisualContainer.RowCount > 0)
                {
                    for (int i = 0; i <= this.GetHeaderIndex(); i++)
                    {
                        this.VisualContainer.RowHeights[i] = this.HeaderRowHeight;
                    }
                }
            }
            else if (IsChanged(SfDataGrid.HeaderRowHeightProperty) && this.VisualContainer.RowCount > 0)
            {
                for (int i = 0; i <= this.GetHeaderIndex(); i++)
                {
                    this.VisualContainer.RowHeights[i] = this.HeaderRowHeight;
                }
            }

            if (IsChanged(SfDataGrid.GroupDropAreaTextProperty) && this.groupDropArea != null)
            {
                this.groupDropArea.GroupDropAreaText = this.GroupDropAreaText;
            }

            if (IsChanged(IsGroupDropAreaExpandedProperty) && this.groupDropArea != null)
            {
                this.groupDropArea.IsExpanded = this.IsGroupDropAreaExpanded;
            }
#if !WP
            if (IsChanged(AllowFilteringPropertyProperty) && this.Columns.Count > 0)
            {
                this.RefreshFilterIconVisibility();
            }
#endif
            if (this.IsChanged(SfDataGrid.GridValidationModeProperty))
            {
                this.UpdateValidationMode();
            }
        }

        protected void EnsureViewProperties()
        {
            if (isViewPropertiesEnsured)
                return;

            if (isselectedindexchanged)
            {
                this.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle()
                    {
                        NewValue = this.SelectedIndex,
                        OldValue = -1,
                        PropertyName = "SelectedIndex"
                    });
                isselectedindexchanged = false;
            }

            if(isselecteditemchanged)
            {
                this.SelectionController.HandleSelectionPropertyChanges(new SelectionPropertyChangeHandle()
                {
                    NewValue = this.SelectedItem,
                    OldValue = null,
                    PropertyName = "SelectedItem"
                });
                isselecteditemchanged = false;
            }
            isViewPropertiesEnsured = true;
        }

        /// <summary>
        /// Method which helps to initialize all the collection in SfDataGrid
        /// </summary>
        /// <remarks></remarks>
        private void InitializeCollections()
        {
            SetValue(ColumnsProperty, new Columns());
            SetValue(SelectedItemsProperty, new ObservableCollection<object>());
            SetValue(SortColumnDescriptionsProperty, new SortColumnDescriptions());
            SetValue(GroupColumnDescriptionsProperty, new GroupColumnDescriptions());
            SetValue(GroupSummaryRowsProperty, new ObservableCollection<GridSummaryRow>());
            SetValue(TableSummaryRowsProperty, new ObservableCollection<GridSummaryRow>());
            SetValue(StackedHeaderRowsProperty, new StackedHeaderRows());
#if !WP
            SetValue(DetailsViewDefinitionProperty, new DetailsViewDefinition());
#endif
        }

        /// <summary>
        /// Method which helps to initialize the Table Summary rows
        /// </summary>
        /// <remarks></remarks>
        private void InitializeTableSummaries()
        {
            if (this.View == null) return;
            foreach (var summaryRow in this.TableSummaryRows)
                this.View.TableSummaryRows.Add(summaryRow);
        }

        private void InitializeGroupSummaryRows()
        {
            if (this.View == null) return;
            foreach (var summaryRow in this.GroupSummaryRows)
                this.View.SummaryRows.Add(summaryRow);
        }

        private void WireSerializablePropertyEvents()
        {
            this.SortColumnDescriptions.CollectionChanged += this.GridModel.OnSortColumnsChanged;
            this.GroupColumnDescriptions.CollectionChanged += this.GridModel.OnGroupColumnDescriptionsChanged;
            this.GroupSummaryRows.CollectionChanged += this.GridModel.OnSummaryRowsChanged;
            this.TableSummaryRows.CollectionChanged += this.GridModel.OnTableSummaryRowsChanged;
        }

        private void UnWireSerializablePropertyEvents()
        {
            this.SortColumnDescriptions.CollectionChanged -= this.GridModel.OnSortColumnsChanged;
            this.GroupColumnDescriptions.CollectionChanged -= this.GridModel.OnGroupColumnDescriptionsChanged;
            this.GroupSummaryRows.CollectionChanged -= this.GridModel.OnSummaryRowsChanged;
            this.TableSummaryRows.CollectionChanged -= this.GridModel.OnTableSummaryRowsChanged;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Method which helps to update the rowcolumn count initially
        /// </summary>
        /// <param name="canGenerateVisibleColumns">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <remarks></remarks>
        internal void UpdateRowAndColumnCount(bool canGenerateVisibleColumns)
        {
            if (this.container == null)
                return;

            (VisualContainer.ColumnWidths as LineSizeCollection).SuspendUpdates();

            this.UpdateColumnCount(canGenerateVisibleColumns);
#if !WP
            if (AutoGenerateRelations)
                GenerateGridRelations();
#endif
            if (this.StackedHeaderRows.Count > 0)
            {
                this.InitializeStackedColumnChildDelegate();
            }
            
            if (this.View != null)
                this.UpdateRowCount();

            if (this.container.RowCount > 0)
            {
                for (int i = 0; i <= this.GetHeaderIndex(); i++)
                {
                    this.container.RowHeights[i] = this.HeaderRowHeight;
                }
            }
			this.HasUnboundColumns = Columns.Any(col => col.IsUnbound);
            this.container.FrozenRows = headerLineCount;
            if (this.FrozenColumnCount > 0 && this.container.ColumnCount >= this.ResolveToScrollColumnIndex(this.FrozenColumnCount))
                this.container.FrozenColumns = this.ResolveToScrollColumnIndex(this.FrozenColumnCount);
            else if (this.ShowRowHeader && this.container.ColumnCount > 1)
                this.container.FrozenColumns = 1;
            else
                this.container.FrozenColumns = 0;
            this.container.FooterColumns = 0;
            if (this.container.RowCount > headerLineCount)
                this.container.FooterRows = this.GetTableSummaryCount(TableSummaryRowPosition.Bottom);

            // When change the item source at runtime the hidden columns are maintained in hidden state. So this method is called to reset the hidden columns
            (VisualContainer.ColumnWidths as LineSizeCollection).ResetHiddenState();

            for (var i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Width == 0)
                    Columns[i].IsHidden = true;
                if (!Columns[i].IsHidden) continue;
                var index = i;
                if (ShowRowHeader)
                    index += 1;
#if !WP
                if (this.DetailsViewManager.HasDetailsView)
                    index += 1;
#endif
                if (View != null && View.GroupDescriptions.Count > 0)
                    index += GroupColumnDescriptions.Count;

                VisualContainer.ColumnWidths.SetHidden(index, index, true);
            }

            var firstIndex = ShowRowHeader ? 1 : 0;
            if (ShowRowHeader && this.container.ColumnCount > 0)
                VisualContainer.ColumnWidths[0] = this.RowHeaderWidth;
            if (View != null)
                this.View.GroupDescriptions.ForEach(
                    desc => VisualContainer.ColumnWidths[firstIndex++] = this.GridModel.IndentColumnSize);
#if !WP
            if (this.DetailsViewManager.HasDetailsView)
                VisualContainer.ColumnWidths[firstIndex] = this.GridModel.IndentColumnSize;
            if (AllowDetailsViewPadding)
                VisualContainer.RowHeights.PaddingDistance = DetailsViewPadding.Top + DetailsViewPadding.Bottom + 1;
#endif

            (VisualContainer.ColumnWidths as LineSizeCollection).ResumeUpdates();
            this.container.UpdateScrollBars();

            if (this.View == null)
            {
                if (this.AutoGenerateColumns && this.AutoGenerateColumnsMode != AutoGenerateColumnsMode.None && canGenerateVisibleColumns && this.Columns.Count <= 0)
                {
                    this.container.ColumnCount = 0;
                    this.container.FooterColumns = 0;
                    this.container.FrozenColumns = 0;
                    this.container.RowCount = 0;
                    this.container.FrozenRows = 0;
                }
                else
                {
                    this.container.RowCount = this.HeaderLineCount;
                    this.container.FrozenRows = this.HeaderLineCount;
                }
                this.container.FooterRows = 0;
                this.container.UpdateScrollBars();
            }
        }

        internal void UpdateColumnCount(bool canGenerateVisibleColumns)
        {
            int columnCount;
            if (this.AutoGenerateColumns && this.AutoGenerateColumnsMode != AutoGenerateColumnsMode.None && canGenerateVisibleColumns && (this.Columns.Count <= 0 || this.AutoGenerateColumnsMode == AutoGenerateColumnsMode.Reset))
            {
                this.GenerateGridColumns();
                columnCount = this.Columns.Count;
            }
            else
                columnCount = this.Columns.Count;
            if (this.ShowRowHeader)
                columnCount += 1;
            if (View != null && View.GroupDescriptions.Count > 0)
                columnCount += View.GroupDescriptions.Count;
#if !WP
            if (this.DetailsViewManager.HasDetailsView)
                columnCount += 1;
#endif
            this.container.ColumnCount = columnCount;
        }


        internal void RemoveLine(int removeAt, int count)
        {
            var lineSizeCollection = this.VisualContainer.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();
            int level = count;
#if !WP
            if (!this.GridModel.HasGroup)
            {
                if (this.DetailsViewManager.HasDetailsView)
                    level += this.DetailsViewDefinition.Count;
            }
#endif
            lineSizeCollection.RemoveLines(removeAt, level);
#if !WP
            if (this.DetailsViewManager.HasDetailsView)
            {
                this.RowGenerator.Items.OfType<DetailsViewDataRow>().ForEach(row =>
                {
                    if (row.CatchedRowIndex > removeAt)
                        row.CatchedRowIndex -= level;
                });
            }
#endif
            lineSizeCollection.ResumeUpdates();
        }

        internal void InsertLine(int insertAt, int count, int recordStartIndex = 0, int recordCount = 0)
        {
            var lineSizeCollection = this.VisualContainer.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();
            int level = count;
# if !WP
            if (!this.GridModel.HasGroup)
            {
                if (this.DetailsViewManager.HasDetailsView)
                    level += this.DetailsViewDefinition.Count;
            }
#endif
            lineSizeCollection.InsertLines(insertAt, level);

            lineSizeCollection.ResumeUpdates();
# if !WP
            if (this.DetailsViewManager.HasDetailsView)
            {
                this.RowGenerator.Items.OfType<DetailsViewDataRow>().ForEach(row =>
                {
                    if (row.CatchedRowIndex > insertAt)
                        row.CatchedRowIndex += level;
                });
                if (!this.GridModel.HasGroup)
                {
                    for (int i = 1; i < level; i++)
                    {
                        this.VisualContainer.RowHeights.SetHidden(insertAt + i, insertAt + i, true);
                        this.VisualContainer.RowHeights.SetNestedLines(insertAt + i, null);
                    }
                }
                else
                {
                    if (recordCount != 0)
                    {
                        var endIndex = recordStartIndex + recordCount + (recordCount * DetailsViewDefinition.Count);
                        lineSizeCollection.SetHiddenIntervalWithState(recordStartIndex, endIndex, this.DetailsViewManager.GetHiddenPattern());
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Method which helps to update the row count while grouping and filtering....
        /// </summary>
        /// <remarks></remarks>
        internal void UpdateRowCount()
        {
            var lineSizeCollection = this.VisualContainer.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();
#if !WP
            int rowCount;
            if (this.View.GroupDescriptions.Count > 0)
                rowCount = this.View.TopLevelGroup.DisplayElements.Count;
            else
                rowCount = this.DetailsViewManager.HasDetailsView ? this.View.Records.Count * (DetailsViewDefinition.Count + 1) : this.View.Records.Count;
#else
            int rowCount;
            if (this.View.GroupDescriptions.Count > 0)
                rowCount = this.View.TopLevelGroup.DisplayElements.Count;
            else
                rowCount = this.View.Records.Count;
#endif
            rowCount = rowCount + headerLineCount;
            rowCount += this.GetTableSummaryCount(TableSummaryRowPosition.Bottom);
            if (AddNewRowPosition == Grid.AddNewRowPosition.Bottom)
                rowCount += 1;
            this.container.RowCount = rowCount;

            lineSizeCollection.ResetHiddenState();
            lineSizeCollection.ResetNestedLines();

            lineSizeCollection.ResumeUpdates();
#if !WP
            if (!this.DetailsViewManager.HasDetailsView) return;

            if (this.View.GroupDescriptions.Count > 0)
                SetExpandedState(this.View.TopLevelGroup);
            else
            {
                var hiddenPattern = new List<bool> { false };
                DetailsViewDefinition.ForEach(r => hiddenPattern.Add(true));
                var startIdx = this.ResolveStartIndexBasedOnPosition();
                var endindex = this.VisualContainer.RowHeights.LineCount - this.GetTableSummaryCount(TableSummaryRowPosition.Bottom);
                lineSizeCollection.SetHiddenInterval(startIdx, endindex, hiddenPattern.ToArray());
            }
#endif
        }

        /// <summary>
        /// Methos which helps to update the view after group operation has done
        /// </summary>
        /// <remarks></remarks>
        internal void UpdateRowCountAndScrollBars()
        {
            if (this.container != null && this.View != null)
            {
                this.UpdateRowCount();
                this.container.UpdateScrollBars();
                if (this.container.ScrollOwner != null)
                    this.container.ScrollOwner.InvalidateScrollInfo();
                if (this.container.ScrollableOwner != null)
                    this.container.ScrollableOwner.InvalidateScrollInfo();
            }
        }
#if !WP
        internal bool CanSetAllowFilters(GridColumn column)
        {
            if (this.View is Syncfusion.Data.PagedCollectionView)
            {
                if ((this.View as Syncfusion.Data.PagedCollectionView).UseOnDemandPaging)
                    return false;
            }
            return column.AllowFiltering;
        }
#endif

        internal void SetBusyState(string stateName)
        {
            if (ShowBusyIndicator)
            {
                VisualStateManager.GoToState(this, stateName, true);
            }
        }

        #endregion

        #region Event Call Back Methods

        /// <summary>
        /// Method which is called when the SelectedItems Changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.SelectedItemsCollection);
        }

        /// <summary>
        /// Helper methods to update the view when the CaptionSummary row changed.
        /// </summary>
        /// <param name="row"></param>
        /// <remarks></remarks>
        private void OnCaptionSummaryRowChanged(GridSummaryRow row)
        {
            if (this.View != null)
            {
                this.View.CaptionSummaryRow = row;
            }
            this.GridModel.OnCaptionSummaryRowChanged(row);
        }

        #endregion

#if !WP
        #region Details View

        #region Fields

        internal IDetailsViewNotifyListener NotifyListener;
        internal bool AllowDetailsViewPadding;
        private bool isListenerSuspended;
        private bool IsRootDetailsViewGrid;

        #endregion

        #region Public DP, Methods and Events

        /// <summary>
        /// Gets or sets a value indicating whether auto generate relations.
        /// </summary>
        /// <value>
        /// <c>true</c> if auto generate relations; otherwise, <c>false</c>.
        /// </value>
        public bool AutoGenerateRelations
        {
            get { return (bool)GetValue(AutoGenerateRelationsProperty); }
            set { SetValue(AutoGenerateRelationsProperty, value); }
        }

        /// <summary>
        /// Gets the selected child grid whild using GridViewDefinition.
        /// </summary>
        [Cloneable(false)]
        public SfDataGrid SelectedDetailsViewGrid
        {
            get { return (SfDataGrid)GetValue(SelectedDetailsViewGridProperty); }
            internal set { SetValue(SelectedDetailsViewGridProperty, value); }
        }

        /// <summary>
        /// Indicates whether empty GridViewDefinition should be hidded or not.
        /// </summary>
        public bool HideEmptyGridViewDefinition
        {
            get { return (bool)GetValue(HideEmptyGridViewDefinitionProperty); }
            set { SetValue(HideEmptyGridViewDefinitionProperty, value); }
        }

        [Cloneable(false)]
        public Thickness DetailsViewPadding
        {
            get { return (Thickness)GetValue(DetailsViewPaddingProperty); }
            set { SetValue(DetailsViewPaddingProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateRelationsProperty =
            GridDependencyProperty.Register("AutoGenerateRelations", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(default(bool)));

        public static readonly DependencyProperty SelectedDetailsViewGridProperty =
            GridDependencyProperty.Register("SelectedDetailsViewGrid", typeof(SfDataGrid), typeof(SfDataGrid), new GridPropertyMetadata(default(SfDataGrid)));

        public static readonly DependencyProperty HideEmptyGridViewDefinitionProperty =
            GridDependencyProperty.Register("HideEmptyGridViewDefinition", typeof(bool), typeof(SfDataGrid), new GridPropertyMetadata(default(bool), OnHideEmptyGridViewDefinitionChanged));

        public static readonly DependencyProperty DetailsViewPaddingProperty =
            GridDependencyProperty.Register("DetailsViewPadding", typeof(Thickness), typeof(SfDataGrid), new GridPropertyMetadata(new Thickness(6, 6, 2, 6)));

        /// <summary>
        /// Expands all the records with Details View.
        /// </summary>
        public void ExpandAllDetailsView()
        {
            this.DetailsViewManager.ExpandAllDetailsView();
        }

        /// <summary>
        /// Collapse all the records with Details View.
        /// </summary>
        public void CollapseAllDetailsView()
        {
            this.DetailsViewManager.CollapseAllDetailsView();
        }

        /// <summary>
        /// Expands the record at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void ExpandDetailsViewAt(int recordIndex)
        {
            this.DetailsViewManager.ExpandDetailsViewAt(recordIndex);
        }

        /// <summary>
        /// Collapses the recoed at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void CollapseDetailsViewAt(int recordIndex)
        {
            this.DetailsViewManager.CollapseDetailsViewAt(recordIndex);
        }

        /// <summary>
        /// Occurs when the record is details view expanding.
        /// </summary>
        public event GridDetailsViewExpandingEventHandler DetailsViewExpanding;

        /// <summary>
        /// Occurs when the record is details view expanded.
        /// </summary>
        public event GridDetailsViewExpandedEventHandler DetailsViewExpanded;

        /// <summary>
        /// Occurs when the record is details view collapsing.
        /// </summary>
        public event GridDetailsViewCollapsingEventHandler DetailsViewCollapsing;

        /// <summary>
        /// Occurs when the record is details view collapsed.
        /// </summary>
        public event GridDetailsViewCollapsedEventHandler DetailsViewCollapsed;


        /// <summary>
        /// Occurs while auto populating the details view definition
        /// </summary>
        public event AutoGeneratingRelationsEventHandler AutoGeneratingRelations;


        #endregion

        #region Private Methods

        private void GenerateGridRelations()
        {
            if (!AutoGenerateRelations)
                return;

            if (this.Columns == null || this.View == null) return;

            if (DetailsViewDefinition != null && (DetailsViewDefinition == null || DetailsViewDefinition.Count != 0))
                return;

            if (DetailsViewDefinition == null)
                DetailsViewDefinition = new DetailsViewDefinition();
            else
                DetailsViewDefinition.Clear();

            var properties = this.View.GetItemProperties();
#if !WPF
            foreach (var keyvaluepair in properties)
            {
                var isRelationalType = GridUtil.IsComplexType(keyvaluepair.Value.PropertyType) &&
                                          !typeof(byte[]).IsAssignableFrom(keyvaluepair.Value.PropertyType);
#else
            foreach (PropertyDescriptor pd in properties)
            {
                if (!this.IsLegacyDataTable)
                {
                    var isRelationalType = GridUtil.IsComplexType(pd.PropertyType) &&
                                           !typeof(byte[]).IsAssignableFrom(pd.PropertyType);
#endif
                    if (isRelationalType)
                    {
                        var canGenerateRelation = true;
#if WPF
                        var attr = pd.Attributes.ToList<Attribute>().FirstOrDefault(a => a is DisplayAttribute);
#else
                    var attr = keyvaluepair.Value.GetCustomAttributes(typeof(Attribute), true).FirstOrDefault(a => a is DisplayAttribute);
#endif
                        if (attr != null)
                        {
                            var displayAttribute = attr as DisplayAttribute;
                            if (displayAttribute != null && displayAttribute.GetAutoGenerateField() != null)
                                canGenerateRelation = displayAttribute.AutoGenerateField;
                        }
                        if (!canGenerateRelation)
                            continue;
#if WPF
                        ExtractRelationalColumn(pd);
#else
                    ExtractRelationalColumn(keyvaluepair.Value);
#endif
                    }
#if WPF
                }
                else
                {
                    var datarelation = GridUtil.GetDataRelation(pd);
                    if (datarelation != null)
                    {
                        var cancel = false;
                        var gridView = new GridViewDefinition { RelationalColumn = datarelation.RelationName };
                        var args = this.RaiseAutoGeneratingRelationsEvent(new AutoGeneratingRelationsArgs(gridView, this) { Cancel = cancel });
                        if (!args.Cancel)
                            this.DetailsViewDefinition.Add(args.GridViewDefinition);
                    }
                }
#endif
            }
        }

#if WPF
        private void ExtractRelationalColumn(PropertyDescriptor pd)
#else
        private void ExtractRelationalColumn(PropertyInfo pd)
#endif
        {
            ////sometimes we get a byte[] array for a column that has images
            var isNestedCollection = pd.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
#if !WinRT
 && !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive);
#else
 && !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive());
#endif
            if (!isNestedCollection) return;
            var cancel = false;
            var gridView = new GridViewDefinition { RelationalColumn = pd.Name };
            var args = this.RaiseAutoGeneratingRelationsEvent(new AutoGeneratingRelationsArgs(gridView, this) { Cancel = cancel });
            if (!args.Cancel)
                this.DetailsViewDefinition.Add(args.GridViewDefinition);
        }

        internal void SetExpandedState(Group group)
        {
            var lineSizeCollection = this.VisualContainer.RowHeights as LineSizeCollection;
            if (!group.IsBottomLevel)
            {
                foreach (var childGroup in group.Groups)
                    SetExpandedState(childGroup);
            }
            else
            {
                if (group.IsExpanded)
                {
                    var startIdx = this.ResolveStartIndexOfGroup(group) + 1;
                    var recordCount = group.GetRecordCount();
                    var endIndex = startIdx + recordCount;
                    var hiddenPattern = new List<bool> { false };
                    DetailsViewDefinition.ForEach(r => hiddenPattern.Add(true));
                    lineSizeCollection.SetHiddenIntervalWithState(startIdx, endIndex, hiddenPattern.ToArray());
                    startIdx++;
                    foreach (var record in group.Records)
                    {
                        if (record.IsExpanded)
                            lineSizeCollection.SetHidden(startIdx, startIdx, false);
                        startIdx += 2;
                    }
                }
            }
        }

        #endregion

        #region internal Methods

        internal void ForceInitializeDetailsViewGrid()
        {
            IsRootDetailsViewGrid = true;
            WireEvents();
        }

        private void EnsureColumnSettings()
        {
            this.Columns.ForEach(x => this.GridModel.WireColumnDescriptor(x));
        }

        internal bool RaiseDetailsViewExpanding(GridDetailsViewExpandingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.DetailsViewExpanding != null)
                return this.NotifyListener.RootDataGrid.RaiseDetailsViewExpanding(e);
            if (DetailsViewExpanding != null) DetailsViewExpanding(this, e);
            return !e.Cancel;
        }

        internal void RaiseDetailsViewExpanded(GridDetailsViewExpandedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.DetailsViewExpanded != null)
                this.NotifyListener.RootDataGrid.RaiseDetailsViewExpanded(e);
            if (DetailsViewExpanded != null) DetailsViewExpanded(this, e);
        }

        internal bool RaiseDetailsViewCollapsing(GridDetailsViewCollapsingEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.DetailsViewCollapsing != null)
                return this.NotifyListener.RootDataGrid.RaiseDetailsViewCollapsing(e);
            if (DetailsViewCollapsing != null) DetailsViewCollapsing(this, e);
            return !e.Cancel;
        }

        internal void RaiseDetailsViewCollapsed(GridDetailsViewCollapsedEventArgs e)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.DetailsViewCollapsed != null)
                this.NotifyListener.RootDataGrid.RaiseDetailsViewCollapsed(e);
            if (DetailsViewCollapsed != null) DetailsViewCollapsed(this, e);
        }

        internal AutoGeneratingRelationsArgs RaiseAutoGeneratingRelationsEvent(AutoGeneratingRelationsArgs args)
        {
            if (NotifyListener != null && !IsRootDetailsViewGrid && NotifyListener.RootDataGrid.AutoGeneratingRelations != null)
                return NotifyListener.RootDataGrid.RaiseAutoGeneratingRelationsEvent(args);
            if (AutoGeneratingRelations != null)
                this.AutoGeneratingRelations(this, args);
            return args;
        }
        #endregion

        #region IDetailsViewNotifyListener

        IDetailsViewNotifyListener IDetailsViewNotifier.NotifyListener
        {
            get { return NotifyListener; }
        }

        bool IDetailsViewNotifier.IsListenerSuspended
        {
            get { return isListenerSuspended; }
        }

        void IDetailsViewNotifier.SetNotifierListener(IDetailsViewNotifyListener notifyListener)
        {
            this.NotifyListener = notifyListener;
        }

        void IDetailsViewNotifier.SuspendNotifyListener()
        {
            isListenerSuspended = true;
        }

        void IDetailsViewNotifier.ResumeNotifyListener()
        {
            isListenerSuspended = false;
        }

        #endregion

        #endregion

        #region INotifyDependencyPropertyChanged

        void INotifyDependencyPropertyChanged.OnDependencyPropertyChanged(string propertyName, DependencyPropertyChangedEventArgs e)
        {
            if (this.NotifyListener != null)
                this.NotifyListener.NotifyPropertyChanged(this, propertyName, e, dataGrid => dataGrid, this, typeof(SfDataGrid));
        }

        #endregion
#endif
        #region IDisposable Member

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.UnWireEvents();
            if (this.GridModel != null)
            {
                this.GridModel.Dispose();
                this.GridModel = null;
            }
            if (this.groupDropArea != null)
            {
                this.groupDropArea.Dispose();
                this.groupDropArea = null;
            }
            if (this.GridColumnDragDropController != null)
            {
                this.GridColumnDragDropController.Dispose();
                this.GridColumnDragDropController = null;
            }
            if (this.container != null)
            {
#if !WPF
                this.container.ContainerKeydown = null;
#endif
                this.container.Dispose();
                this.container = null;
            }
            if (this.RowGenerator != null)
            {
                this.RowGenerator.Dispose();
                this.RowGenerator = null;
            }
            if (this.GridColumnSizer != null)
            {
                this.GridColumnSizer.Dispose();
                this.GridColumnSizer = null;
            }
            if (this.View != null)
            {
                this.View.Dispose();
                this.View = null;
            }
            if (this.cellRenderers != null)
            {
                this.cellRenderers.Dispose();
                this.cellRenderers = null;
            }

            if (this.GroupColumnDescriptions != null)
            {
                this.GroupColumnDescriptions.Clear();
                this.ClearValue(SfDataGrid.GroupColumnDescriptionsProperty);
            }

            if (this.SortColumnDescriptions != null)
            {
                this.SortColumnDescriptions.Clear();
                this.ClearValue(SfDataGrid.SortColumnDescriptionsProperty);
            }

            if (this.SortComparers != null)
            {
                this.SortComparers.Clear();
                this.ClearValue(SfDataGrid.SortComparersProperty);
            }

            if (this.GroupSummaryRows != null)
            {
                this.GroupSummaryRows.Clear();
                this.GroupSummaryRows = null;
            }

            if (this.SelectedItems != null)
            {
                this.SelectedItems.CollectionChanged -= OnSelectedItemsChanged;
                this.SelectedItems.Clear();
            }

            if (this.Columns != null)
            {
                (this.Columns as INotifyCollectionChanged).CollectionChanged -= OnGridColumnCollectionChanged;
                this.Columns.Dispose();
                this.Columns = null;
            }

            if (this.TableSummaryRows != null)
            {
                this.TableSummaryRows.Clear();
                this.TableSummaryRows = null;
            }

            if (this.SelectionController != null)
            {
                (this.SelectionController as GridSelectionController).Dispose();
                this.SelectionController = null;
            }

            if (GridColumnResizingController != null)
            {
                GridColumnResizingController.Dispose();
                GridColumnResizingController = null;
            }

            if (this.Validations != null)
            {
                this.Validations.Dispose();
                this.Validations = null;
            }
#if !WP
            if (this.DetailsViewManager != null)
            {
                this.DetailsViewManager.Dispose();
                this.DetailsViewManager = null;
            }

            if (this.GridCopyPaste is GridCutCopyPaste)
            {
                (this.GridCopyPaste as GridCutCopyPaste).Dispose();
            }
#endif
            if (this.container != null)
            {
                this.container.Dispose();
                this.container = null;
            }
        }

        #endregion



    }
}