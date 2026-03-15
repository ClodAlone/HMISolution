#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Data;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;
    using Syncfusion.Windows.ComponentModel;
    using System.Security.Permissions;
    using Syncfusion.Windows.Data;
    using Syncfusion.Linq;
    using System.Collections.Specialized;
    using Syncfusion.Windows.Controls.Grid.Automation.Peers;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls.Primitives;
    using Microsoft.Windows.Themes;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Automation.Peers;
	using System.Windows.Threading;

    /// <summary>
    /// Grid Data control is specifically designed for the scenarios, 
    /// where you need to bound the grid to an external data source and 
    /// customize the data view by performing the operations such as grouping, sorting, 
    /// summarizing, filtering, conditional formats and unbound fields. It can display
    /// nested grids with hierarchical data and can also display multiple unrelated tables in one grid.
    /// </summary>
    /// <remarks>
    /// The GDC supports, 
    /// <para></para>
    /// <list type="bullet">
    /// <item>
    /// <description>Sorting.</description></item>
    /// <item>
    /// <description>Filtering.</description></item>
    /// <item>
    /// <description>Different Cell types.</description></item>
    /// <item>
    /// <description>CUD operations on the underlying list.</description></item>
    /// <item>
    /// <description>Relations (Master-Details).</description></item>
    /// <item>
    /// <description>Update the Grid under high frequency.</description></item></list>
    /// <para></para>
    /// <para>It also includes support for Deferred scrolling, that was included in the
    /// .NET FW 3.5 SP1 with the ScrollView control. High frequency updates are handled
    /// inside the grid with great efficiency. The GDC uses optimized RB tree for internal
    /// structures.</para>
    /// <para></para>
    /// <para>The GDC also includes a derived
    /// GridDataStyleInfo which holds style information in cell by cell basis. You can
    /// directly find the type of cell by accessing the
    /// <see cref = "Syncfusion.Windows.Controls.Grid.GridDataStyleInfo.CellIdentity.TableCellType"/>.</para>
    /// </remarks>
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif

    [Bindable(true), DefaultBindingProperty("ItemsSource")]
    [TemplatePart(Name = GridDataControl.TemplateGrid, Type = typeof(GridDataControlBaseImpl))]
    [StyleTypedProperty(Property = "RowStyle", StyleTargetType = typeof(GridDataRowControl))]
    [StyleTypedProperty(Property = "AlternateRowStyle", StyleTargetType = typeof(GridDataRowControl))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(GridDataHeaderCellControl))]
    [StyleTypedProperty(Property = "ColumnOptionPaneStyle", StyleTargetType = typeof(GridDataColumnOptionsPane))]
    [StyleTypedProperty(Property="ScrollViewerStyle",StyleTargetType=typeof(ScrollViewer))]    
    [StyleTypedProperty(Property="ContextMenuStyle",StyleTargetType=typeof(ContextMenu))]
    public partial class GridDataControl : ContentControl, IDisposable
    {

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (GridDataControl.GetOverrideVisualStyle(this))
            {
                var skinStorageVisualStyle = SkinStorage.GetVisualStyle(this);
                if (skinStorageVisualStyle != null && skinStorageVisualStyle.ToString().Equals("Metro") &&
                    this.VisualStyle != VisualStyle.Metro && !this.hasVisualStyleChanged)
                    this.VisualStyle = VisualStyle.Metro;

                if (e.Property.Name.Equals("MetroBorderBrush") || e.Property.Name.Equals("MetroBrush") ||
                    e.Property.Name.Equals("MetroFocusedBorderBrush") || e.Property.Name.Equals("MetroFontFamily") ||
                    e.Property.Name.Equals("MetroForegroundBrush") ||
                    e.Property.Name.Equals("MetroHighlightedForegroundBrush") ||
                    e.Property.Name.Equals("MetroHoverBrush") || e.Property.Name.Equals("MetroPanelBackgroundBrush"))
                {
                    if (this.TableProperties != null && this.TableProperties.Model != null)
                        this.TableProperties.UpdateVisualStyle();
                }
            }

        }

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataControl"/> class.
        /// </summary>
        public GridDataControl()
        {
            if (GridDataControl.IsSecurityGranted)
            {
                GridDataControl.ValidateLicense();                
            }
            this.SelectedItems = new ObservableCollection<object>();
            resetSelectedItems = false;
            this.TableProperties = GetTableProperties();
            ((INotifyCollectionChanged)this.VisibleColumns).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnVisibleColumnsChanged);
            ((INotifyCollectionChanged)this.ConditionalFormats).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnConditionalFormatsChanged);
            this.SelectedItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
            this.Model.SelectionChanging += new GridSelectionChangingEventHandler(Model_SelectionChanging);
            this.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
            this.Loaded += new RoutedEventHandler(GridDataControl_Loaded);
            this.GotFocus += new RoutedEventHandler(GridDataControl_GotFocus);
            ColumnChooserColumns = new GridDataVisibleColumns();
            this.Model.TableStyle.Borders.Changed += new Styles.StyleChangedEventHandler(OnTableBordersChanged);
        }
        
        void GridDataControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridDataControlBaseImpl && e.Source is GridDataControl)
            {
                var currentCell = this.Model.CurrencyManager.CurrentCell;
                if (currentCell != null && currentCell.IsEditing)
                {
                    IGridCellRenderer renderer = currentCell.Renderer;
                    UIElement element;
                    if (renderer != null)
                    {
                        element = renderer.CurrentCellUIElement;
                        if (element != null)
                            element.Focus();
                    }
                }
            }
        }

        void GridDataControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.isGridLoaded)
            {
                if(this.Visibility != System.Windows.Visibility.Visible)
                    this.Model.ColumnAutoSizer.IsGridDataControlLoaded = true;
                this.isColumnSizerChangedBeforeGridLoaded = true;
                return;
            }
            this.Model.ColumnAutoSizer.IsGridDataControlLoaded = true;
            this.Model.ColumnAutoSizer.RefreshAll();
            if (this.ColumnSizer == GridControlLengthUnitType.AutoOnLoad || this.ColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill)
            {
                this.Model.ColumnAutoSizer.IsAutoOnLoad = true;
            }

            if (this.ApplySizingAfterLoad && this.Model != null && this.Model.ColumnWidths != null && this.BorderThickness != null)
            {
                double widthOfLastColumn = this.Model.ColumnWidths.TotalExtent - this.Model.ColumnWidths[this.Model.ColumnWidths.LineCount - 1];
                double d = double.NaN;
                FrameworkElement fe = this.Parent as FrameworkElement;
                if (fe != null)
                {
                    d = fe.ActualWidth;
                    while (fe != null && !(fe is ScrollViewer))
                    {
                        fe = fe.Parent() as FrameworkElement;
                    }
                    if (fe is ScrollViewer)
                    {
                        //d = fe.ActualWidth;
                        d = Math.Min(fe.ActualWidth, (fe as ScrollViewer).ViewportWidth); // Change has been done to fix the issue 8076.
                    }                    
                }

                /// Following changes has been done to fix the issue 8076 ( Layout exceeds problem in GridDataControl with low resolution systems) as per the Clay suggestion.
                if (double.IsNaN(this.Width) && !double.IsNaN(d) && d < this.ActualWidth)
                {
                        this.Width = d;
                        if (this.ColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill || this.ColumnSizer == GridControlLengthUnitType.AutoWithLastColumnFill)
                        {
                            this.Model.ColumnWidths[this.Model.ColumnWidths.LineCount - 1] = d - widthOfLastColumn - 3; ///-3 has been deducted since the the actual width size of the scrollviewer has right width which 
                                                                                                                        ///leads enable scrollviewer when column sizer is AutoOnLoadWithLastColumnFill 
                        }
                        if (this.ColumnSizer == GridControlLengthUnitType.Star)
                        {

                            double colWidth = (d-3) / this.Model.ColumnCount;
                            for (int i = 0; i < this.Model.ColumnCount; i++)
                            {
                                this.Model.Views.First().SetColumnWidth(i, colWidth);
                            }
                        }
                        // Change has been done to fix the issue 8076.
                        else if (this.Width > this.Model.ColumnWidths.TotalExtent)
                        {
                            this.Width = this.Model.ColumnWidths.TotalExtent + 3;
                        }
                    
                    if (this.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                    {
                        this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    }
                }
            }
            SetUpdateModeProperty();
            //this.Model.TableStyle.Borders.Changed += new Styles.StyleChangedEventHandler(OnTableBordersChanged);
        }


        static GridDataControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(typeof(GridDataControl)));

            EndEditCommand.InputGestures.Add(new KeyGesture(Key.F3));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(EndEditCommand, new ExecutedRoutedEventHandler(OnExecutedEndEdit), new CanExecuteRoutedEventHandler(OnCanExecuteEndEdit)));
            SearchKeyCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(SearchKeyCommand, SearchKeyExecuted, OnCanSearchKeyExecute));
            FlowDirectionProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFlowDirectionChanged));
            ForegroundProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnForegroundChanged));
            BackgroundProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnBackgroundChanged));
            FontFamilyProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFontFamilyChanged));
            FontSizeProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFontSizeChanged));
            FontStretchProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFontStretchChanged));
            FontStyleProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFontStyleChanged));
            FontWeightProperty.OverrideMetadata(typeof(GridDataControl), new FrameworkPropertyMetadata(OnFontWeightChanged));
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.NotifyPropertyChanges = false;        
            if (this.chooser != null)
            {
                chooser.Closed -= new EventHandler(chooser_Closed);
                chooser = null;
            }
            if (this.ctorModel != null)
                this.ctorModel.CurrencyManager.CurrentRecordSelectionChanged -= new GridDataCurrentRecordSelectionChangedEventHandler(this.OnCurrentRecordSelectionChanged);
            BindingOperations.ClearAllBindings(this);
            DisposeGrid();
            if (scrollViewer != null)
            {
                scrollViewer.Content = null;
                scrollViewer = null;
            }
            GC.SuppressFinalize(this);
        }
        # region Disposing all properties and events.
        internal void DisposeGrid()
        {
            this.SelectedItem = null;
            this.SelectedItems.Clear();
            this.oldSelectedItem = null;
            this.ItemsSource = null;
            if (this.TableProperties != null)
            {
                if (this.ConditionalFormats != null)
                    ((INotifyCollectionChanged)this.ConditionalFormats).CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnConditionalFormatsChanged);
                if (this.VisibleColumns != null)
                    ((INotifyCollectionChanged)this.VisibleColumns).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnVisibleColumnsChanged); 
                this.SelectedItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
            }

            if (this.Model.Grid != null)
                UnwireGrid(this.Model.Grid);

            if (this.Model != null)
            {
                this.Model.Dispose();
                if (this.GroupDropAreaGrid != null)
                {
                    this.GroupDropAreaGrid.Dispose(true);
                    GC.SuppressFinalize(this.GroupDropAreaGrid);
                    this.GroupDropAreaGrid = null;
                    var groupDropAreaScrollViewer = this.GetTemplateChild("PART_GroupDropAreaScrollViewer") as ScrollViewer;
                    if (groupDropAreaScrollViewer != null)
                        groupDropAreaScrollViewer.Content = null;
                    groupDropAreaScrollViewer = null;
                }
                if (this.TableProperties != null)
                {
                    this.TableProperties.Dispose();
                    this.TableProperties = null;
                }
                this.ctorModel = null;
                // this.Model.Options = null;
                if (this.PART_CloseButton != null)
                {
                    this.PART_CloseButton.Click -= new RoutedEventHandler(PART_CloseButton_Click);
                    this.PART_CloseButton = null;
                }
                this.PART_BorderStatusBar = null;
            }
            if (this.InternalGrid != null)
            { 
                this.InternalGrid.Dispose(true);
                this.InternalGrid.Model = null;
                this.InternalGrid = null;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Called when [flow direction changed].
        /// </summary>
        /// <param name="dpo">The dpo.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFlowDirectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.Model.TableStyle.FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), args.NewValue.ToString());
//If FlowDirection set at the run time then grid doesnt get refreshed. So we need to refresh here if FlowDirection set after grid loaded
            if(grid.IsLoaded)
                grid.Model.Grid.InvalidateCells();
        }

        void OnTableBordersChanged(object sender, Styles.StyleChangedEventArgs e)
        {
            switch (e.Sip.PropertyName)
            {
                case "Left":
                    this.Model.TableStyle.IsLeftBorderChanged = true;
                    break;
                case "Top":
                    this.Model.TableStyle.IsTopBorderChanged = true;
                    break;
                case "Right":
                    this.Model.TableStyle.IsRightBorderChanged = true;
                    break;
                case "Bottom":
                    this.Model.TableStyle.IsBottomBorderChanged = true;
                    break;
            }
        }

        internal bool hasForegroundChanged = false;

        private static void OnForegroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasForegroundChanged = args.NewValue != SystemColors.ControlTextBrush ? true : false;

            if (grid.hasForegroundChanged)
            {
                grid.ApplyForegroundColor(args.NewValue);
                grid.Model.TableStyle.Foreground = args.NewValue as Brush;
                grid.Model.HeaderStyle.Foreground = grid.Model.HeaderStyle.HasHeaderForeGround ? grid.Model.HeaderStyle.Foreground : args.NewValue as Brush;
                grid.Model.IndentColumnStyle.Foreground = grid.Model.IndentColumnStyle.HasHeaderForeGround ? grid.Model.IndentColumnStyle.Foreground : args.NewValue as Brush;
            }
            //grid.hasForegroundChanged = false;  
        }

        private void ApplyForegroundColor(object newValue)
        {
            Brush newBrush = newValue as Brush;
            this.Foreground = newBrush;
            if (this.RowForeground == null && this.RowForeground != Brushes.Transparent)
            {
                this.RowForeground = newBrush;
            }
            this.RowForeground = newBrush;
        }

        internal bool hasBackgroundChanged = false;

        private static void OnBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;

            if (!grid.TableProperties.IsInternalChange)
            {
                grid.hasBackgroundChanged = args.NewValue != SystemColors.WindowBrush ? true : false;
                if (grid.hasBackgroundChanged)
                    grid.ApplyColor(args.NewValue);
            }
        }

        private void ApplyColor(object newValue)
        {
            Brush newBrush = newValue as Brush;
            this.Background = newBrush;
            
            if (!this.TableProperties.IsRowBackgroundChangedExternally && !this.TableProperties.IsInternalChange)
                this.TableProperties.RowBackground = newBrush;
            if (!this.TableProperties.IsAlternatingRowBackgroundChangedExternally && !this.TableProperties.IsInternalChange)
                this.TableProperties.AlternatingRowBackground = newBrush;
            this.Model.TableStyle.Background = newBrush;
           //this.Model.HeaderStyle.Background = this.Model.HeaderStyle.HasHeaderBackGround ? this.Model.HeaderStyle.Background : newBrush;
           //this.Model.IndentColumnStyle.Background = this.Model.IndentColumnStyle.HasHeaderBackGround ? this.Model.IndentColumnStyle.Background : newBrush;
        }

        internal bool hasFontFamilyChanged = false;
        private static void OnFontFamilyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasFontFamilyChanged = args.NewValue != null ? true : false;
        }

        internal bool hasFontSizeChanged = false;
        private static void OnFontSizeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasFontSizeChanged = args.NewValue != null ? true : false;
        }

        internal bool hasFontStretchChanged = false;
        private static void OnFontStretchChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasFontStretchChanged = args.NewValue != null ? true : false;
        }

        internal bool hasFontStyleChanged = false;
        private static void OnFontStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasFontStyleChanged = args.NewValue != null ? true : false;
        }

        internal bool hasFontWeightChanged = false;
        private static void OnFontWeightChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.hasFontWeightChanged = args.NewValue != null ? true : false;
        }

        #region EndEdit Routed Command

        /// <summary>
        /// RoutedCommand for EndEdit action.
        /// </summary>
        public static RoutedCommand EndEditCommand = new RoutedCommand(GridDataControl.EndEditCommandText, typeof(GridDataControl));

        public const string EndEditCommandText = "GridDataControl_EndEditCommandText";

        private static void OnExecutedEndEdit(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridDataControl)sender).OnExecutedEndEdit(args);
        }

        protected virtual void OnExecutedEndEdit(ExecutedRoutedEventArgs args)
        {
            if (this.Model.IsEditing)
            {
                this.Model.CurrencyManager.ConfirmChanges();
                args.Handled = true;
            }
        }

        private static void OnCanExecuteEndEdit(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridDataControl)sender).OnCanExecuteEndEdit(args);
        }

        protected virtual void OnCanExecuteEndEdit(CanExecuteRoutedEventArgs args)
        {
            if (this.Model.IsEditing)
            {
                args.Handled = true;
                args.CanExecute = true;
            }
        }

        #endregion

        public const string SearchKeyCommandText = "GridDataControl_SearchKeyCommandText";

        public static RoutedCommand SearchKeyCommand = new RoutedCommand(GridDataControl.SearchKeyCommandText, typeof(GridDataControl));

        private static void SearchKeyExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridDataControl)sender).OnSearchKeyExecuted(args);
        }

        protected virtual void OnSearchKeyExecuted(ExecutedRoutedEventArgs args)
        {
            var grid = this.InternalGrid;
            if (grid != null)
            {
                var currentCell = grid.CurrentCell;
                var rowIdx = this.TableProperties.StackedHeaderRows.Count;
                // MoveTo would automatically trigger Activate in the HeaderCellRenderer
                var headerCellRenderer = grid.CellRenderers["HeaderCell"] as GridDataHeaderCellRenderer;
                headerCellRenderer.ShouldShowAdvancedFilterPaneOnActivated = true;
                if (currentCell != null)
                {
                    var colIdx = currentCell.ColumnIndex;
                    colIdx = this.Model.ResolvePositionToVisibleColumnIndex(colIdx);
                    var col = colIdx > -1 && colIdx < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIdx] : null;
                    if (col != null && col.AllowFilter && col.IsAdvancedFilteringMode)
                    {
                        currentCell.MoveTo(rowIdx, currentCell.ColumnIndex);
                    }
                }
            }
        }

        private static void OnCanSearchKeyExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridDataControl)sender).OnCanSearchKeyExecute(args);
        }

        protected virtual void OnCanSearchKeyExecute(CanExecuteRoutedEventArgs args)
        {
            var grid = this.InternalGrid;
            if (grid != null)
            {
                var currentCell = grid.CurrentCell;
                // var rowIdx = this.TableProperties.StackedHeaderRows.Count; Unused local variable
                if (currentCell != null)
                {
                    var colIdx = currentCell.ColumnIndex;
                    colIdx = this.Model.ResolvePositionToVisibleColumnIndex(colIdx);
                    var col = colIdx > -1 && colIdx < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIdx] : null;
                    if (col != null && col.AllowFilter && col.IsAdvancedFilteringMode)
                    {
                        args.Handled = true;
                        args.CanExecute = true;
                    }
                }
            }
        }

        #endregion  

        private const double StatusBarHeight = 30.00;

        public bool ShowFilterStatusMessage
        {
            get { return (bool)GetValue(ShowFilterStatusMessageProperty); }
            set { SetValue(ShowFilterStatusMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFilterStatusMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFilterStatusMessageProperty =
            DependencyProperty.Register("ShowFilterStatusMessage", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnStatusBarMessageChanged));

        public string StatusBarMessage
        {
            get { return (string)GetValue(GridDataControl.StatusBarMessageProperty); }
            set { SetValue(GridDataControl.StatusBarMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarMessageProperty =
            DependencyProperty.Register("StatusBarMessage", typeof(string), typeof(GridDataControl), new PropertyMetadata("", OnStatusBarMessageChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AddNewRowPosition" />.
        /// </summary>
        public static readonly DependencyProperty AddNewRowPositionProperty = DependencyProperty.Register(
            "AddNewRowPosition",
            typeof(Position),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(Position.Top, OnAddNewRowPositionChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.UnboundRowPosition" />.
        /// </summary>
        public static readonly DependencyProperty UnboundRowPositionProperty = DependencyProperty.Register(
            "UnboundRowPosition",
            typeof(Position),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(Position.Top, OnUnboundRowPositionChanged));

        public static readonly DependencyProperty TableSummaryPositionProperty = DependencyProperty.Register(
          "TableSummaryPosition",
          typeof(Position),
          typeof(GridDataControl),
          new PropertyMetadata(Position.Bottom, OnTableSummaryPositionChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowDelete" />.
        /// </summary>
        public static readonly DependencyProperty AllowDeleteProperty = DependencyProperty.Register(
            "AllowDelete",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowDeletePropertyChanged));

        public static readonly DependencyProperty AllowMultipleRecordDeletionProperty = DependencyProperty.Register(
           "AllowMultipleRecordDeletion",
           typeof(bool),
           typeof(GridDataControl),
           new FrameworkPropertyMetadata(false, OnAllowMultipleRecordDeletionChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowEdit" />.
        /// </summary>
        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register(
            "AllowEdit",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowEditPropertyChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowGroup" />.
        /// </summary>
        public static readonly DependencyProperty AllowGroupProperty = DependencyProperty.Register(
            "AllowGroup",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowGroupPropertyChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowSort" />.
        /// </summary>
        public static readonly DependencyProperty AllowSortProperty = DependencyProperty.Register(
            "AllowSort",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowSortPropertyChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.SortingOptions" />.
        /// </summary>
        public static readonly DependencyProperty SortingOptionsProperty = DependencyProperty.Register(
            "SortingOptions",
            typeof(SortingOptions),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(SortingOptions.Default, OnSortingOptionsPropertyChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AlternatingRowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty = DependencyProperty.Register(
            "AlternatingRowBackground",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnAlternatingRowBackgroundChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AlternatingRowCount"/>.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowCountProperty = DependencyProperty.Register(
            "AlternatingRowCount",
            typeof(int),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(2, OnAlternatingRowCountPropertyChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AutoPopulateColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AutoPopulateColumnsProperty = DependencyProperty.Register(
            "AutoPopulateColumns",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAutoPopulateColumnsChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AutoPopulateRelations"/>.
        /// </summary>
        public static readonly DependencyProperty AutoPopulateRelationsProperty = DependencyProperty.Register(
            "AutoPopulateRelations",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAutoPopulateRelationsChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.DefaultColumnWidth"/>.
        /// </summary>
        public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register(
            "DefaultColumnWidth",
            typeof(double),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(150d, OnDefaultColumnWidthChanged));

        /// <summary>
        /// DependencyPropery for <see cref = "GridDataControl.IsDeferredScrollingEnabled" />.
        /// </summary>
        public static readonly DependencyProperty IsDeferredScrollingEnabledProperty = DependencyProperty.Register(
            "IsDeferredScrollingEnabled",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyPropery for <see cref = "GridDataControl.DetailsViewTemplate" />.
        /// </summary>
        public static readonly DependencyProperty DetailsViewTemplateProperty = DependencyProperty.Register(
            "DetailsViewTemplate", 
            typeof(DataTemplate), 
            typeof(GridDataControl), 
            new FrameworkPropertyMetadata(OnDetailsViewTemplateChanged));

        /// <summary>
        /// Gets or sets a value for EnableRenderOptimization
        /// Setting true will optimize the scrolling performance of the GridDataControl
        /// </summary>

        public EnableRenderOptimization EnableRenderOptimization
        {
            get
            {
                return (EnableRenderOptimization)this.GetValue(GridDataControl.EnableRenderOptimizationProperty);
            }
            set
            {
                this.SetValue(GridDataControl.EnableRenderOptimizationProperty, value);
            }
        }


        /// <summary>
        /// DependencyPropery for <see cref = "GridDataControl.EnableRenderOptimizationProperty" />.
        /// </summary>
        public static readonly DependencyProperty EnableRenderOptimizationProperty = DependencyProperty.Register(
            "EnableRenderOptimization",
            typeof(EnableRenderOptimization),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(EnableRenderOptimization.None, OnEnableRenderOptimizationChanged));

        private bool isEnableRenderOptimizationSetBeforeLoaded = false;

        private static void OnEnableRenderOptimizationChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableRenderOptimization = (EnableRenderOptimization)args.NewValue;
            }
            else
            {
                grid.isEnableRenderOptimizationSetBeforeLoaded = true;
            }
        }

#if !SILVERLIGHT

        private DelegateCommand<object> columnChooserCommand;
        public DelegateCommand<object> ColumnChooserCommand
        {
            get
            {
                if (columnChooserCommand == null)
                    columnChooserCommand = new DelegateCommand<object>(ShowColumnChooserGrid);

                return columnChooserCommand;
            }
        }

        void ShowColumnChooserGrid(object parameter)
        {
            this.ShowColumnChooser();
        }
#endif

        #region FilterBehavior

        /// <summary>
        /// FilterBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterBehaviorProperty =
            DependencyProperty.Register("FilterBehavior", typeof(FilterBehavior), typeof(GridDataControl),
                new FrameworkPropertyMetadata(FilterBehavior.StronglyTyped,
                    new PropertyChangedCallback(OnFilterBehaviorChanged)));

        /// <summary>
        /// Gets or sets the FilterBehavior property. This dependency property 
        /// indicates ....
        /// </summary>
        public FilterBehavior FilterBehavior
        {
            get { return (FilterBehavior)GetValue(FilterBehaviorProperty); }
            set { SetValue(FilterBehaviorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the FilterBehavior property.
        /// </summary>
        private static void OnFilterBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.FilterBehavior = (FilterBehavior)e.NewValue;
        }

        #endregion

#if !SILVERLIGHT
#region OverrideVisualStule

        /// <summary>
        /// Dependency attached property for GetOverrideVisualStyle/SetOverrideVisualStyle.
        /// </summary>
        private static readonly DependencyProperty OverrideVisualStyleProperty = DependencyProperty.RegisterAttached("OverrideVisualStyle",
                                                                                                           typeof (bool),
                                                                                                           typeof (GridDataControl),
                                                                                                           new PropertyMetadata(null));
        /// <summary>
        /// Gets a boolean value if the DependencyObject should be loaded with override visual style.
        /// </summary>
        /// <param name="dpo"></param>
        /// <returns></returns>
        public static bool GetOverrideVisualStyle(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridDataControl.OverrideVisualStyleProperty);
        }

        /// <summary>
        /// Sets a boolean value if the DependencyObject should be loaded with override visual style.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value"></param>
        public static void SetOverrideVisualStyle(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridDataControl.OverrideVisualStyleProperty, value);
        }

#endregion
#endif

        #region SortClickAction

        /// <summary>
        /// SortClickAction Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortClickActionProperty =
            DependencyProperty.Register("SortClickAction", typeof(SortClickAction), typeof(GridDataControl),
                new PropertyMetadata(SortClickAction.SingleClick,
                    new PropertyChangedCallback(OnSortClickActionChanged)));

        /// <summary>
        /// Gets or sets the SortClickAction property. This dependency property 
        /// indicates ....
        /// </summary>
        public SortClickAction SortClickAction
        {
            get { return (SortClickAction)GetValue(SortClickActionProperty); }
            set { SetValue(SortClickActionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the SortClickAction property.
        /// </summary>
        private static void OnSortClickActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.SortClickAction = (SortClickAction)e.NewValue;
        }

        #endregion

        #region ReserveSpaceForIcons

        /// <summary>
        /// ReserveSpaceForIcons Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReserveSpaceForIconsProperty =
            DependencyProperty.Register("ReserveSpaceForIcons", typeof(bool), typeof(GridDataControl),
                new FrameworkPropertyMetadata(false, OnReserveSpaceForIconsChanged));


        private static void OnReserveSpaceForIconsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.ReserveSpaceForIcons = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the ReserveSpaceForIcons property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool ReserveSpaceForIcons
        {
            get { return (bool)GetValue(ReserveSpaceForIconsProperty); }
            set { SetValue(ReserveSpaceForIconsProperty, value); }
        }

        #endregion

        #region GroupDropAreaHeight

        /// <summary>
        /// GroupDropAreaHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty GroupDropAreaHeightProperty =
            DependencyProperty.Register("GroupDropAreaHeight", typeof(double), typeof(GridDataControl),
                new PropertyMetadata((double)50, OnGroupDropAreaHeightChanged));

        /// <summary>
        /// Gets or sets the GroupDropAreaHeight property. This dependency property 
        /// indicates height specified for group drop area.
        /// </summary>
        public double GroupDropAreaHeight
        {
            get { return (double)GetValue(GroupDropAreaHeightProperty); }
            set { SetValue(GroupDropAreaHeightProperty, value); }
        }

        bool isGroupDropAreaGridHeightChangedBeforeGridLoaded = false;
        /// <summary>
        /// Handles changes to the GroupDropAreaHeight property.
        /// </summary>
        private static void OnGroupDropAreaHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.GroupDropAreaHeight = (double)e.NewValue;
            }
            else
                grid.isGroupDropAreaGridHeightChangedBeforeGridLoaded = true;
        }


        #endregion

        #region HideEmptyChildGrid

        /// <summary>
        /// HideEmptyChildGrid Dependency Property
        /// </summary>
        public static readonly DependencyProperty HideEmptyChildGridProperty =
            DependencyProperty.Register("HideEmptyChildGrid", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnHideEmptyChildGridChanged)));

        /// <summary>
        /// Gets or sets the HideEmptyChildGrid property. This dependency property 
        /// indicates whether empty child grid should be hidded or not.
        /// </summary>
        public bool HideEmptyChildGrid
        {
            get { return (bool)GetValue(HideEmptyChildGridProperty); }
            set { SetValue(HideEmptyChildGridProperty, value); }
        }

        /// <summary>
        /// Handles changes to the HideEmptyChildGrid property.
        /// </summary>
        private static void OnHideEmptyChildGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.HideEmptyChildGrid = (bool)e.NewValue;
        }

        #endregion

        #region ScrollViewerStyle


        [Obsolete("Not sure if needed")]
        public Style ScrollViewerStyle
        {
            get { return (Style)GetValue(ScrollViewerStyleProperty); }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollViewerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollViewerStyleProperty =
            DependencyProperty.Register("ScrollViewerStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(null));        

        #endregion

        #region ContextMenuStyle



        public Style ContextMenuStyle
        {
            get { return (Style)GetValue(ContextMenuStyleProperty); }
            set { SetValue(ContextMenuStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextMenuStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuStyleProperty =
            DependencyProperty.Register("ContextMenuStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(OnContextMenuStyleChanged));

        private static void OnContextMenuStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.Model.GridContextMenu.Style = (Style)e.NewValue;
        }
        

        #endregion

        private bool isGridLoaded = false;

        private bool isItemsSourceLoadedBeforeGridLoaded = false;

        private bool isVisualStyleChangedBeforeGridLoaded = false;

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnItemsSourceChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.NullFilterText" />.
        /// </summary>
        public static readonly DependencyProperty NullFilterTextProperty = DependencyProperty.Register(
            "NullFilterText",
            typeof(string),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata("None", OnNullFilterTextChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.RowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register(
            "RowBackground",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnRowBackgroundChanged));

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.FilterBarMode"/>.
        /// </summary>
        /// <value><c>Immediate</c> FilterBarMode; otherwise, <c>OnEnter</c>.</value>
        public static readonly DependencyProperty FilterBarModeProperty =
            DependencyProperty.Register("FilterBarMode", typeof(GridDataFilterBarMode), typeof(GridDataControl), new PropertyMetadata(GridDataFilterBarMode.Immediate, OnFilterBarModeChanged));

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.AlphaNumericFilterType"/>.
        /// </summary>
        /// <value><c>WithWildcard</c> AlphaNumericFilterType; otherwise, <c>WithoutWildcard</c>.</value>
        public static readonly DependencyProperty AlphaNumericFilterTypeProperty =
            DependencyProperty.Register("AlphaNumericFilterType", typeof(AlphaNumericFilterType), typeof(GridDataControl), new PropertyMetadata(AlphaNumericFilterType.WithWildcard, OnAlphaNumericFilterTypeChanged));



        public FilterPanePosition FilterPanePosition
        {
            get { return (FilterPanePosition)GetValue(FilterPanePostionProperty); }
            set { SetValue(FilterPanePostionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterPanePostion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterPanePostionProperty =
            DependencyProperty.Register("FilterPanePostion", typeof(FilterPanePosition), typeof(GridDataControl), new PropertyMetadata(null, OnFilterPanePositionChanged));


        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.ShowFilterBar"/>.
        /// </summary>
        /// <value><c>true</c> if [show Filterbar]; otherwise, <c>false</c>.</value>
        public static readonly DependencyProperty ShowFilterBarProperty =
            DependencyProperty.Register("ShowFilterBar", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnShowFilterBarChanged));

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.ShowAddNewRow"/>.
        /// </summary>
        /// <value><c>true</c> if [show add new row]; otherwise, <c>false</c>.</value>
        public static readonly DependencyProperty ShowAddNewRowProperty = DependencyProperty.Register(
            "ShowAddNewRow",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnShowAddNewRowChanged));

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.ShowRecordPlusMinus"/>
        /// </summary>
        /// <value>
        /// <c>true</c> if [show record plus minus]; otherwise, <c>false</c>.
        /// </value>
        public static readonly DependencyProperty ShowRecordPlusMinusProperty = DependencyProperty.Register(
            "ShowRecordPlusMinus",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnShowRecordPlusMinusChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.ShowRowHeader"/>.
        /// </summary>
        public static readonly DependencyProperty ShowRowHeaderProperty = DependencyProperty.Register(
            "ShowRowHeader",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(false, OnShowRowHeaderChanged));

        public const string TemplateGrid = "PART_GridControl";

        public const string TemplateGroupDropAreaGrid = "PART_GroupDropAreaGrid";

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.VisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.Register(
            "VisualStyle",
            typeof(VisualStyle),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(VisualStyle.Default, OnVisualStyleChanged));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.CustomVisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty CustomVisualStyleProperty = DependencyProperty.Register(
            "CustomVisualStyle",
            typeof(IGridDataVisualStyle),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowDragColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AllowDragColumnsProperty = DependencyProperty.Register(
            "AllowDragColumns",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(false, OnAllowDragColumnsChanged));

        #region ModelProvider (DependencyProperty)

        /// <summary>
        /// Gets / sets the ModelProvider
        /// </summary>
        public IGridModelProvider ModelProvider
        {
            get { return (IGridModelProvider)GetValue(ModelProviderProperty); }
            set { SetValue(ModelProviderProperty, value); }
        }

        public static readonly DependencyProperty ModelProviderProperty = DependencyProperty.Register("ModelProvider", typeof(IGridModelProvider), typeof(GridDataControl), new PropertyMetadata(null, OnModelProviderChanged));

        private static void OnModelProviderChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            var modelProvider = args.NewValue as IGridModelProvider;
            if (modelProvider != null)
            {
                modelProvider.SetTableModel(grid.Model);
            }
        }

        #endregion

        #region SortWhenGrouped

        /// <summary>
        /// SortWhenGrouped Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortWhenGroupedProperty =
            DependencyProperty.Register("SortWhenGrouped", typeof(bool), typeof(GridDataControl),
                new FrameworkPropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnSortWhenGroupedChanged)));

        /// <summary>
        /// Gets or sets the SortWhenGrouped property. This dependency property 
        /// indicates whether sorting is perfored while is group
        /// </summary>
        public bool SortWhenGrouped
        {
            get { return (bool)GetValue(SortWhenGroupedProperty); }
            set { SetValue(SortWhenGroupedProperty, value); }
        }

        /// <summary>
        /// Handles changes to the SortWhenGrouped property.
        /// </summary>
        private static void OnSortWhenGroupedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.SortWhenGrouped = (bool)e.NewValue;
        }


        #endregion

        #region IncludeHeaderTextOnCopy

        /// <summary>
        /// IncludeHeaderTextOnCopy Dependency Property
        /// </summary>
        public static readonly DependencyProperty IncludeHeaderTextOnCopyProperty =
            DependencyProperty.Register("IncludeHeaderTextOnCopy", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata(true, OnIncludeHeaderTextOnCopyChanged));

        /// <summary>
        /// Gets or sets the IncludeHeaderTextOnCopy property. This dependency property 
        /// indicates whether header text has to be added while copying.
        /// </summary>
        public bool IncludeHeaderTextOnCopy
        {
            get { return (bool)GetValue(IncludeHeaderTextOnCopyProperty); }
            set { SetValue(IncludeHeaderTextOnCopyProperty, value); }
        }

        /// <summary>
        /// Call back for IncludeHeaderTextOnCopy
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIncludeHeaderTextOnCopyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.IncludeHeaderTextOnCopy = (bool)args.NewValue;
        }

        #endregion

        #region FooterRows

        public static readonly DependencyProperty FooterRowsProperty = DependencyProperty.Register("FooterRows", typeof(int), typeof(GridDataControl), new FrameworkPropertyMetadata(0, OnFooterRowsChanged));

        private static void OnFooterRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.FooterRows = (int)args.NewValue;
        }

        public int FooterRows
        {
            get
            {
                return (int)this.GetValue(GridDataControl.FooterRowsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.FooterRowsProperty, value);
            }
        }

        #endregion

        #region HeaderRows
        public static readonly DependencyProperty HeaderRowsProperty = DependencyProperty.Register("HeaderRows", typeof(int), typeof(GridDataControl), new FrameworkPropertyMetadata(1, OnHeaderRowsChanged));

        private static void OnHeaderRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.HeaderRows = (int)args.NewValue;
        }

        public int HeaderRows
        {
            get
            {
                return (int)this.GetValue(GridDataControl.HeaderRowsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.HeaderRowsProperty, value);
            }
        }

        #endregion

        #region FooterColumns

        public static readonly DependencyProperty FooterColumnsProperty = DependencyProperty.Register("FooterColumns", typeof(int), typeof(GridDataControl), new FrameworkPropertyMetadata(0, OnFooterColumnsChanged));

        private static void OnFooterColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.FooterColumns = (int)args.NewValue;
        }

        public int FooterColumns
        {
            get
            {
                return (int)this.GetValue(GridDataControl.FooterColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.FooterColumnsProperty, value);
            }
        }

        #endregion

        #region HeaderColumns

        public static readonly DependencyProperty HeaderColumnsProperty = DependencyProperty.Register("HeaderColumns", typeof(int), typeof(GridDataControl), new FrameworkPropertyMetadata(0, OnHeaderColumnsChanged));

        private static void OnHeaderColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.HeaderColumns = (int)args.NewValue;
        }

        public int HeaderColumns
        {
            get
            {
                return (int)this.GetValue(GridDataControl.HeaderColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.HeaderColumnsProperty, value);
            }
        }

        #endregion

        #region FrozenRows (DependencyProperty)

        /// <summary>
        /// Gets / Sets FrozenRows.
        /// </summary>
        public int FrozenRows
        {
            get { return (int)GetValue(FrozenRowsProperty); }
            set { SetValue(FrozenRowsProperty, value); }
        }

        public static readonly DependencyProperty FrozenRowsProperty = DependencyProperty.Register("FrozenRows", typeof(int), typeof(GridDataControl), new PropertyMetadata(1, OnFrozenRowsChanged));

        private static void OnFrozenRowsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FrozenRows = (int)args.NewValue;
        }

        #endregion

        #region FrozenColumns (DependencyProperty)

        /// <summary>
        /// Gets / Sets the FrozenColumns
        /// </summary>
        public int FrozenColumns
        {
            get { return (int)GetValue(FrozenColumnsProperty); }
            set { SetValue(FrozenColumnsProperty, value); }
        }

        public static readonly DependencyProperty FrozenColumnsProperty = DependencyProperty.Register("FrozenColumns", typeof(int), typeof(GridDataControl), new PropertyMetadata(0, OnFrozenColumnsChanged));

        private static void OnFrozenColumnsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FrozenColumns = (int)args.NewValue;
        }

        #endregion

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ColumnSizer"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnSizerProperty = DependencyProperty.Register(
            "ColumnSizer",
            typeof(GridControlLengthUnitType),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(GridControlLengthUnitType.None, OnColumnSizerChanged));

        private bool isColumnSizerChangedBeforeGridLoaded = false;

        private static void OnColumnSizerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ColumnSizer = (GridControlLengthUnitType)args.NewValue;
            }
            else
            {
                grid.isColumnSizerChangedBeforeGridLoaded = true;
            }
        }

        #region HeaderCellTemplate

        /// <summary>
        /// HeaderTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderCellTemplate", typeof(DataTemplate), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnHeaderTemplateChanged)));


        public DataTemplate HeaderCellTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the HeaderTemplate property.
        /// </summary>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.HeaderCellTemplate = (DataTemplate)e.NewValue;
        }

        #endregion

        #region DefaultHeaderRowHeight

        /// <summary>
        /// DefaultHeaderRowHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty DefaultHeaderRowHeightProperty =
            DependencyProperty.Register("DefaultHeaderRowHeight", typeof(Double), typeof(GridDataControl),
                new FrameworkPropertyMetadata(GridDataTableModel.HeaderRowHeight,
                    new PropertyChangedCallback(OnDefaultHeaderRowHeightChanged)));

        /// <summary>
        /// Gets or sets the DefaultHeaderRowHeight property. This dependency property 
        /// indicates DefaultHeaderRowHeight of HeaderCells.
        /// </summary>
        public Double DefaultHeaderRowHeight
        {
            get { return (Double)GetValue(DefaultHeaderRowHeightProperty); }
            set { SetValue(DefaultHeaderRowHeightProperty, value); }
        }

        private bool isDefaultHeaderRowHeightChangedBeforeGridLoaded = false;

        /// <summary>
        /// Handles changes to the DefaultHeaderRowHeight property.
        /// </summary>
        private static void OnDefaultHeaderRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.DefaultHeaderRowHeight = (double)args.NewValue;
            }
            else
            {
                grid.isDefaultHeaderRowHeightChangedBeforeGridLoaded = true;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the column sizer.
        /// </summary>
        /// <value>The column sizer.</value>
        public GridControlLengthUnitType ColumnSizer
        {
            get
            {
                return (GridControlLengthUnitType)this.GetValue(GridDataControl.ColumnSizerProperty);
            }
            set
            {
                this.SetValue(GridDataControl.ColumnSizerProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.ShowFilters"/>.
        /// </summary>
        public static readonly DependencyProperty ShowFiltersProperty = DependencyProperty.Register(
            "ShowFilters",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnShowFiltersPropertyChanged));

        private bool isShowFiltersChangedBeforeGridLoaded = false;

        private static void OnShowFiltersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowFilters = (bool)args.NewValue;
            }
            else
            {
                grid.isShowFiltersChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowColumnOptions"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowColumnOptionsProperty = DependencyProperty.Register(
            "ShowColumnOptions",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnShowColumnOptionsPropertyChanged));

        private bool isShowColumnOptionsPropertyChangedBeforeGridLoaded = false;
        private static void OnShowColumnOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowColumnOptions = (bool)args.NewValue;
            }
            else
            {
                grid.isShowColumnOptionsPropertyChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.AllowResize"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowResizeRowsProperty = DependencyProperty.Register(
            "AllowResizeRows",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowResizeRowsChanged));

        private bool isAllowResizeRowsChangedBeforeLoaded = false;

        private static void OnAllowResizeRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowResizeRows = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowResizeRowsChangedBeforeLoaded = true;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ClearAllOnItemSourceChange"/> property.
        /// </summary>
        public static readonly DependencyProperty ClearAllOnItemSourceChangeProperty = DependencyProperty.Register(
            "ClearAllOnItemSourceChange",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(false, OnClearAllOnItemsSourceChangeChanged));

        private static void OnClearAllOnItemsSourceChangeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ClearAllOnItemSourceChange = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ClearAllOnItemSource is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [clear all on item source change]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearAllOnItemSourceChange
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ClearAllOnItemSourceChangeProperty);
            }
            set
            {
                this.SetValue(GridDataControl.ClearAllOnItemSourceChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowResizeRows is true / false.
        /// </summary>
        /// <value><c>true</c> if [allow resize rows]; otherwise, <c>false</c>.</value>
        public bool AllowResizeRows
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowResizeRowsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowResizeRowsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.AllowResizeColumns"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowResizeColumnsProperty = DependencyProperty.Register(
            "AllowResizeColumns",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnAllowResizeColumnsChanged));

        private bool isAllowResizeColumnsChangedBeforeLoaded = false;

        private static void OnAllowResizeColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowResizeColumns = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowResizeColumnsChangedBeforeLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowResizeColumns is true / false.
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        public bool AllowResizeColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowResizeColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowResizeColumnsProperty, value);
            }
        }



        //Follwing property only for paging support

        public static readonly DependencyProperty IsViewLevelPagingProperty = DependencyProperty.Register(
            "IsViewLevelPaging",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, IsViewLevelPagingPropertyChanged));

        public bool IsViewLevelPaging
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsViewLevelPagingProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsViewLevelPagingProperty, value);
            }
        }

        private static void IsViewLevelPagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.IsViewLevelPaging = (bool)args.NewValue;

        }


        public static readonly DependencyProperty EnablePagingProperty = DependencyProperty.Register(
           "EnablePaging",
           typeof(bool),
           typeof(GridDataControl),
           new PropertyMetadata(false, EnablePagingPropertyChanged));

        public bool EnablePaging
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.EnablePagingProperty);
            }

            set
            {
                this.SetValue(GridDataControl.EnablePagingProperty, value);
            }
        }

        private static void EnablePagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.EnablePaging = (bool)args.NewValue;

        }
        //Till This

        private bool isAllowDragChangedBeforeGridLoaded = false;

        private static void OnAllowDragColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowDragColumns = (bool)args.NewValue;
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
                return (bool)this.GetValue(GridDataControl.AllowDragColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowDragColumnsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.DragIndicatorInnerBrush"/>.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorInnerBrushProperty = DependencyProperty.Register(
            "DragIndicatorInnerBrush",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnDragIndicatorInnerBrushChanged));

        private bool isDragIndicatorInnerBrushChangedBeforeGridLoaded = false;

        private static void OnDragIndicatorInnerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.DragIndicatorInnerBrush = (Brush)args.NewValue;
            }
            else
            {
                grid.isDragIndicatorInnerBrushChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the drag indicator inner brush.
        /// </summary>
        /// <value>The drag indicator inner brush.</value>
        public Brush DragIndicatorInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataControl.DragIndicatorInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DragIndicatorInnerBrushProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.DragIndicatorOuterBrush"/>.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorOuterBrushProperty = DependencyProperty.Register(
            "DragIndicatorOuterBrush",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnDragIndicatorOuterBrushChanged));

        private bool isDragIndicatorOuterBrushChangedBeforeGridLoaded = false;

        private static void OnDragIndicatorOuterBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.DragIndicatorOuterBrush = (Brush)args.NewValue;
            }
            else
            {
                grid.isDragIndicatorOuterBrushChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the drag indicator outer brush.
        /// </summary>
        /// <value>The drag indicator outer brush.</value>
        public Brush DragIndicatorOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataControl.DragIndicatorOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DragIndicatorOuterBrushProperty, value);
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.IsSynchronizedWithCurrentItem"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty = DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnIsSynchronizedWithCurrentItemChanged));

        private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.IsSynchronizedWithCurrentItem = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is synchronized with current item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is synchronized with current item; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronizedWithCurrentItem
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsSynchronizedWithCurrentItemProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsSynchronizedWithCurrentItemProperty, value);
            }
        }
#endif

        #region UpdateMode
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.UpdateMode"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateModeProperty = DependencyProperty.Register("UpdateMode", typeof(UpdateMode), typeof(GridDataControl), new PropertyMetadata(UpdateMode.LostFocus, OnUpdateModeChanged));

        private bool isUpdateModeChangedBeforeGridLoaded = false;
        private static void OnUpdateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.UpdateMode = (UpdateMode)args.NewValue;
            }
            else
            {
                grid.isUpdateModeChangedBeforeGridLoaded = true;
            }
        }


        /// <summary>
        /// Gets or sets the add new row position.
        /// </summary>
        /// <value>The add new row position.</value>
        public Position UnboundRowPosition
            {
            get
                {
                return (Position)this.GetValue(GridDataControl.UnboundRowPositionProperty);
                }

            set
                {
                this.SetValue(GridDataControl.UnboundRowPositionProperty, value);
                }
            }



        /// <summary>
        /// Gets or sets the update mode.
        /// </summary>
        /// <value>The update mode.</value>
        public UpdateMode UpdateMode
        {
            get
            {
                return (UpdateMode)this.GetValue(GridDataControl.UpdateModeProperty);
            }

            set
            {
                this.SetValue(GridDataControl.UpdateModeProperty, value);
            }
        }

        #endregion

        #region TableSummaryPosition

        private static void OnTableSummaryPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.TableSummaryPosition = (Position)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the update mode.
        /// </summary>
        /// <value>The update mode.</value>
        public Position TableSummaryPosition
        {
            get
            {
                return (Position)this.GetValue(GridDataControl.TableSummaryPositionProperty);
            }

            set
            {
                this.SetValue(GridDataControl.TableSummaryPositionProperty, value);
            }
        }

        #endregion


        #region HideColumnsWhenGrouped (DependencyProperty)

        /// <summary>
        /// Gets / Sets if columns have to be hidden when grouped
        /// </summary>
        public bool HideColumnsWhenGrouped
        {
            get { return (bool)GetValue(HideColumnsWhenGroupedProperty); }
            set { SetValue(HideColumnsWhenGroupedProperty, value); }
        }

        public static readonly DependencyProperty HideColumnsWhenGroupedProperty = DependencyProperty.Register("HideColumnsWhenGrouped", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnHideColumnsWhenGroupedChanged));

        private static void OnHideColumnsWhenGroupedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.HideColumnsWhenGrouped = (bool)args.NewValue;
        }

        #endregion


        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (resetSelectedItems || this.Model == null || this.Model.IsInDeteteRecord)
            {
                return;
            }

            try
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        int recordIndex = this.Model.View.Records.IndexOfRecord(e.NewItems[0]);
                        int rowIndex;
                        if (!Model.Table.HasGroups)
                        {
                            rowIndex = this.Model.ResolvePositionToIndex(recordIndex);
                        }
                        else
                        {
                            rowIndex = this.Model.ResolveGroupRecordPositionToIndex(recordIndex);//Get the rowIndex when the DataGrid is Grouping
                        }
                        if (rowIndex < 0)
                        {
                            return;
                        }
                        int x = this.ShowRowHeader == true ? 1 : 0;
                        GridRangeInfo range = new GridRangeInfo(rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                        if (!this.Model.SelectedRanges.Contains(range))
                        {
                            this.Model.SelectedRanges.Add(range);
                            this.Model.InvalidateCell(range);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        if (!this.Model.Table.HasGroups)
                            recordIndex = this.Model.View.Records.IndexOfRecord(e.OldItems[0]);
                        else
                            recordIndex = this.Model.View.TopLevelGroup.IndexOf(e.OldItems[0]);
                        rowIndex = this.Model.ResolvePositionToIndex(recordIndex);
                        //below code modified for the different behavior of Selected items, selecting by pressing Shift and Ctrl button.
                        range = GridRangeInfo.Row(rowIndex);//new GridRangeInfo(GridRangeInfoType.Rows,rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                        if (!range.IsEmpty)
                        {
                            this.Model.Selections.Remove(range);
                            this.Model.InvalidateCell(range);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
                        this.Model.SelectedRanges.Clear();
                        foreach (GridRangeInfo r in rangeList)
                        {
                            this.Model.InvalidateCell(r);
                        }
                        break;

                }
                this.Model.InvalidateVisual(true);
            }
            catch
            { }
        }

        void OnVisibleColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Model != null && this.ItemsSource == null)
            {
                int visColCount = this.VisibleColumns.Count;
                if (visColCount != 0 && !this.VisibleColumns[visColCount - 1].IsHidden)
                {
                    this.Model.ColumnCount = this.VisibleColumns.Count;
                    return;
                }
            }
        }

        private void OnConditionalFormatsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var cf = args.NewItems[0] as GridDataConditionalFormat;
                cf.SetTableModel(this.Model);
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridDataControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        /// <summary>
        /// Occurs when the Model is loaded. This is useful to listen to Model events when
        /// the control is initialized. 
        /// <para></para>
        /// <code lang="C#">            
        ///             this.dataGrid.ModelLoaded += (sender, args) =&gt;
        ///             {
        ///                 this.dataGrid.Model.QueryCellInfo += new
        /// Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventHandler(Model_QueryCellInfo);
        ///             };
        ///             </code>
        /// </summary>
        public event EventHandler ModelLoaded;

        /// <summary>
        /// Gets or sets the add new row position.
        /// </summary>
        /// <value>The add new row position.</value>
        public Position AddNewRowPosition
        {
            get
            {
                return (Position)this.GetValue(GridDataControl.AddNewRowPositionProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AddNewRowPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow delete].
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        public bool AllowDelete
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowDeleteProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowDeleteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow multiple record deletion].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow multiple record deletion]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowMultipleRecordDeletion
        {
            get
            {
                return (bool)GetValue(GridDataControl.AllowMultipleRecordDeletionProperty);
            }
            set
            {
                SetValue(GridDataControl.AllowMultipleRecordDeletionProperty, value);
            }
        }

       
        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowEditProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow group].
        /// </summary>
        /// <value><c>true</c> if [allow group]; otherwise, <c>false</c>.</value>
        public bool AllowGroup
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowGroupProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [Disable Sorting OnEdit].
        /// </summary>
        public SortingOptions SortingOptions
        {
            get
            {
                return (SortingOptions)this.GetValue(GridDataControl.SortingOptionsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.SortingOptionsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sort].
        /// </summary>
        /// <value><c>true</c> if [allow sort]; otherwise, <c>false</c>.</value>
        public bool AllowSort
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowSortProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowSortProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alternating row background.
        /// </summary>
        /// <value>The alternating row background.</value>
        public Brush AlternatingRowBackground
        {
            get
            {
                return this.GetValue(GridDataControl.AlternatingRowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.AlternatingRowBackgroundProperty, value);
            }
        }

        #region AlternatingRowForeground Properties

        /// <summary>
        /// Gets or sets the alternating row background.
        /// </summary>
        /// <value>The alternating row background.</value>
        public Brush AlternatingRowForeground
        {
            get
            {
                return this.GetValue(GridDataControl.AlternatingRowForegroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.AlternatingRowForegroundProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AlternatingRowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowForegroundProperty = DependencyProperty.Register(
            "AlternatingRowForeground",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnAlternatingRowForegroundChanged));

        private static void OnAlternatingRowForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlternatingRowForeground = (Brush)args.NewValue;
        }

        #endregion

        /// <summary>
        /// Gets or sets the alternating row count.
        /// </summary>
        /// <value>The alternating row count.</value>
        public int AlternatingRowCount
        {
            get
            {
                return (int)this.GetValue(GridDataControl.AlternatingRowCountProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AlternatingRowCountProperty, value);
            }
        }

        /// <summary>
        /// The property determines whether the VisibleColumns are to be auto populated or not
        /// </summary>
        /// <value><c>true</c> if [auto populate columns]; otherwise, <c>false</c>.</value>
        public bool AutoPopulateColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AutoPopulateColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AutoPopulateColumnsProperty, value);
            }
        }

        /// <summary>
        ///  The property determines whether the Column Type are to be auto generated or not
        ///  Setting of this property actually set the column type to VisibleColumn based on property type bound to columns
        /// </summary>
        public bool AutoGenerateColumnsInfo
        {
            get { return (bool)GetValue(AutoGenerateColumnsInfoProperty); }
            set { SetValue(AutoGenerateColumnsInfoProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateColumnsInfoProperty =
            DependencyProperty.Register("AutoGenerateColumnsInfo", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnAutoGenerateColumnsInfoChanged));

        private static void OnAutoGenerateColumnsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoGenerateColumnsInfo = (bool)args.NewValue;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [auto populate relations].
        /// </summary>
        /// <value>
        /// <c>true</c> if [auto populate relations]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoPopulateRelations
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AutoPopulateRelationsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AutoPopulateRelationsProperty, value);
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
                return (double)this.GetValue(GridDataControl.DefaultColumnWidthProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DefaultColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
        public bool HasError
        {
            get
            {
                if (this.InternalGrid != null && this.InternalGrid.Model != null && this.Model.CurrencyManager.CurrentRecordIndex >-1)
                {
                    var tableModel = this.InternalGrid.Model as GridDataTableModel;
                    return tableModel.CurrencyManager.HasError;
                }

                return false;
            }
        }

        internal GridDataControlBaseImpl InternalGrid
        {
            get;
            private set;
        }

        internal GridDataGroupDropAreaGridImpl GroupDropAreaGrid
        {
            get;
            set;
        }

#if !SILVERLIGHT
        internal GridDataColumnChooserWindow ColumnChooserGrid
        {
            get;
            private set;
        }
#endif

        #region SelectedItem

        /// <summary>
        /// Gets or sets a value indicating whether to select the first row on load or not.
        /// </summary>
        public bool SelectFirstRowOnLoad
        {
            get { return (bool)GetValue(SelectFirstRowOnLoadProperty); }
            set { SetValue(SelectFirstRowOnLoadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectFirstRowOnLoad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectFirstRowOnLoadProperty =
            DependencyProperty.Register("SelectFirstRowOnLoad", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnSelectFirstRowOnLoadChanged));


        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.SelectedItem"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.OnSelectedItemChanged(args.NewValue);
            grid.oldSelectedItem = args.NewValue;
        }

        private bool isSelectedItemSetBeforeLoaded = false;
        private bool selectedItemChangedFlag = false;
        private void OnSelectedItemChanged(object record)
        {
            if (record == null && !this.Model.CurrencyManager.IsGroupCaptionCell && !this.Model.CurrencyManager.IsGroupCaptionSummaryCoveredCell && !this.Model.CurrencyManager.ISummarysEmptyCell)
            {                                
                this.Model.CurrencyManager.Reset();
                if (this.Model.View != null && this.Model.View.Records != null && this.oldSelectedItem != null)
                {
                    var rowIdx = this.Model.ResolvePositionToIndex(this.Model.View.Records.IndexOfRecord(this.oldSelectedItem));
                    var selectedRow = GridRangeInfo.Row(rowIdx);
                    if (this.Model.SelectedRanges.Contains(selectedRow))
                        this.Model.SelectedRanges.Remove(selectedRow);
                    this.Model.InvalidateCell(selectedRow);
                }
                return;
            }

            if (this.Model.View == null)
            {
                this.isSelectedItemSetBeforeLoaded = true;
                return;
            }

            if (!this.innerSelectionChange && record != null)
            {
                this.Model.View.MoveCurrentTo(record);
            }
            if (!this.Model.IsInSort && !this.Model.IsInGroup && !this.Model.IsInFilter)
                ResetSelectedItems();
            this.selectedItemChangedFlag = true;
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return (object)this.GetValue(GridDataControl.SelectedItemProperty);
            }

            set
            {
                if (this.SelectedItem != value)
                {
                    this.SetValue(GridDataControl.SelectedItemProperty, value);
                }
            }
        }

        #endregion

        #region SelectedItems

        private void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (this.Model.View == null)
            {
                return;
            }
            if (SelectedChildModel != null)
            {
                //if ((e.Reason == GridSelectionReason.MouseDown || e.Reason== GridSelectionReason.Clear) && SelectedChildModel.Grid.CurrentCell.HasCurrentCell)
                //{
                //    SelectedChildModel.Grid.CurrentCell.Deactivate();
                //}
                //if (SelectedChildModel.Grid != null)
                //{
                //    var nestedGrid = SelectedChildModel.Grid as GridDataCellNestedGridEditor;
                //    if(nestedGrid != null)
                //        nestedGrid.ClearChildGridSelections(SelectedChildModel);
                //}
                SelectedChildModel = null;
            }
            if (this.selectedItemChangedFlag)
            {
                //If Model.IsInSort then we dont reset the selected Items, bcoz based on the selected Items we update the selected ranges while sorting.
                if ((e.Reason == GridSelectionReason.MouseUp || (e.Reason == GridSelectionReason.ArrowKey) && !this.Model.IsInSort&&!this.Model.IsInGroup))
                {
                    this.ResetSelectedItems();
                    this.selectedItemChangedFlag = false;
                }
            }
            else if (e.Reason == GridSelectionReason.MouseUp && e.Range.Height > 0) //&& e.Range.Width > 0) /**selection is made on records not on cells so Width is not used inside the method **/
            {
                // bool flag = false; Variable is assigned but it is never used.
                for (int index = e.Range.Top; index <= e.Range.Bottom; index++)
                {
                    int recordIndex = this.Model.ResolveIndexToRecordPosition(index);
                    object record = null;
                    if (this.Model.View != null && this.Model.Table != null)
                    {
                        if (!this.Model.Table.HasGroups)
                        {
                            if (recordIndex < this.Model.View.Records.Count && recordIndex > -1)
                            {
                                record = this.Model.View.Records.GetItemAt(recordIndex);
                            }
                        }
                        else
                        {
                            if (this.Model.View.TopLevelGroup != null && this.Model.View.TopLevelGroup.DisplayElements != null && this.Model.View.TopLevelGroup.DisplayElements.Count >= recordIndex)
                            {
                                var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                                if (recordEntry != null)
                                {
                                    record = recordEntry.Data;
                                }
                            }
                        }
                    }
                }

                //if (flag)
                {
                    this.ResetSelectedItems();
                }
            }          

            //We do not reset the selected items while performing sorting, grouping, filtering.
            if ((e.Reason == GridSelectionReason.SelectRange || e.Reason == GridSelectionReason.Clear||e.Reason==GridSelectionReason.ArrowKey) && !this.Model.IsInSort && !this.Model.IsInFilter&&!this.Model.IsInGroup)                        
            {
                ResetSelectedItems();
            }

            //While deleting the row following operation should needed to maintain the selection.
            if (e.Reason == GridSelectionReason.DeleteRow)
            {
                //Here we Update the selected ranges based on Selected Items and Move the Current Cell.
                this.Model.UpdateSelectedRanges();
                if (this.Model.SelectedRanges.Count == 0)
                {
                    this.Model.SelectedRanges.Add(e.Range);
                    if (this.InternalGrid.CurrentCell.RowIndex == -1 && !this.Model.SelectedRanges.ActiveRange.IsEmpty)
                        this.InternalGrid.CurrentCell.MoveTo(this.Model.SelectedRanges.ActiveRange.Top, this.InternalGrid.CurrentCell.ColumnIndex);
                }
                else if(this.InternalGrid.CurrentCell.RowIndex == -1)
                {
                    this.InternalGrid.CurrentCell.MoveTo(this.Model.SelectedRanges.ActiveRange.Top, this.InternalGrid.CurrentCell.ColumnIndex);
                }
            }
        }
      
        private void Model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if (e.ClickRange == GridRangeInfo.Empty)
                return;

            if (e.Reason == GridSelectionReason.MouseDown || e.Reason == GridSelectionReason.ArrowKey || e.Reason == GridSelectionReason.Clear || (e.Reason == GridSelectionReason.MouseUp && Model.Options.AllowSelectionOnMouseUp)) // || e.Reason == GridSelectionReason.SetCurrentCell)
            {
                if (!e.ClickRange.IsEmpty)
                {
                    var rowColIdx = new RowColumnIndex(e.ClickRange.Bottom, this.Model.Grid.NavigateWithArrowKeysCellsRange.Left);
                    var style = this.Model.Grid.GetRenderStyleInfo(rowColIdx).ModelStyle as GridDataStyleInfo;
                    if (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                    {
                        var args = this.OnRaiseRecordSelectionChanging(sender, e);
                        if (args.Cancel)
                            e.Cancel = true;
                    }
                }
            }
        }

        internal GridDataRecordSelectionChangingEventArgs OnRaiseRecordSelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            var newRowColIndex = new RowColumnIndex(e.ClickRange.Bottom, e.ClickRange.Right);
            var view = this.Model.View; 
            //var oldSelectedItem = this.SelectedItem != null ? this.SelectedItem : null;
            var newSelectedItem = this.Model.View.Records.Count > 0 ?
                (this.Model.Table.HasGroups ? this.Model.Table.GroupModel.DisplayElements.ElementAt(this.Model.ResolveGroupPositionToIndex(newRowColIndex.RowIndex)) : this.Model.Table.GetRecordFromRow(newRowColIndex.RowIndex)) : null;
            
            var recIndex = this.Model.ResolveGroupPositionToIndex(this.Model.CurrencyManager.CurrentCell.RowIndex);

            var args = new GridDataRecordSelectionChangingEventArgs()
            {
                OldItem = this.SelectedItem, 
                OldIndex = this.Model.CurrencyManager.CurrentCell.CellRowColumnIndex,
                NewItem = view.CreateRecordEntry(newSelectedItem),
                NewIndex = newRowColIndex,
                OldRecordIndex = this.Model.Table.HasGroups ? recIndex : this.Model.CurrencyManager.CurrentRecordIndex,
                NewRecordIndex = this.Model.ResolveIndexToRecordPosition(newRowColIndex.RowIndex),
                Reason = e.Reason
            };
           
            this.Model.Table.RaiseRecordSelectionChanging(args);
            return args;
        }

        private object oldSelectedItem;
        public ObservableCollection<object> SelectedItems
        {
            get;
            set;
        }

        private bool resetSelectedItems = false;

        private void ResetSelectedItems()
        {
            resetSelectedItems = true;
            this.innerSelectionChange = true;

            var removedItems = new List<object>();

            foreach (var rec in this.SelectedItems)
            {
                removedItems.Add(rec);
            }

            this.SelectedItems.Clear();
            var dictionary = new Dictionary<int, object>();
            GridDataTableModel tableModel = this.Model;            
            foreach (GridRangeInfo range in this.Model.SelectedRanges)
            {
                GridRangeInfo internalrange = range;
                if (internalrange.RangeType == GridRangeInfoType.Table)
                    internalrange = range.ExpandRange(range.Top, range.Left, Model.RowCount, Model.ColumnCount);
                for (int index = internalrange.Top; index <= internalrange.Bottom; index++)
                {
                    int recordIndex = tableModel.ResolveIndexToRecordPosition(index);
                    object record = null;
                    if (!this.Model.Table.HasGroups)
                    {
                        if (this.Model.View != null && this.Model.View.Records != null)
                        {
                            if (recordIndex < this.Model.View.Records.Count)
                            {
                                if (recordIndex > -1)
                                    record = this.Model.View.Records.GetItemAt(recordIndex);
                            }
                        }
                    }
                    else
                    {
                        if (this.Model.View.TopLevelGroup.DisplayElements.Count >= recordIndex)
                        {
                            var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                            if (recordEntry != null)
                            {
                                record = recordEntry.Data;
                            }
                        }
                    }
                    if (record != null)
                    {
                        if (!dictionary.ContainsKey(recordIndex))
                        {
                            dictionary.Add(recordIndex, record);
                        }
                    }
                }
            }
            if (!this.Model.IsInFilter)
            {
                foreach (var record in dictionary.Values)
                {                    
                    SelectedItems.Add(record);                   
                }
                
                dictionary.Clear();
            }
            resetSelectedItems = false;

            #region  Raise the RaiseRecordsSelectionChanged event.

            var addedItems = new List<object>();
            if (!this.Model.IsInFilter)
            {
                foreach (var rec in this.SelectedItems)
                {
                    if (!removedItems.Contains(rec))
                        addedItems.Add(rec);
                }

                foreach (var rec in this.SelectedItems)
                {
                    if (removedItems.Contains(rec))
                    {
                        removedItems.Remove(rec);
                    }
                }                
            }
            
            this.innerSelectionChange = false;

            if (this.ListBoxSelectionMode != GridSelectionMode.None && (removedItems.Count>0 || addedItems.Count>0))//if this.ListBoxSelectionMode == GridSelectionMode.None  then RecordSelectionChanged event should not fired.
                this.Model.Table.RaiseRecordsSelectionChanged(new GridDataRecordsSelectionChangedEventArgs(removedItems, addedItems));

            #endregion
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deferred scrolling enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is deferred scrolling enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeferredScrollingEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsDeferredScrollingEnabledProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsDeferredScrollingEnabledProperty, value);
            }
        }

        #region ItemsSource

        public static readonly RoutedEvent ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridDataControl));

        protected virtual void OnItemsSourceChanged(SyncfusionRoutedEventArgs e)
        {
            try
            {
                if (!this.ensuredProperties)
                {
                    this.EnsureProperties();
                }

                if (this.ItemsSource != null && this.isSelectedItemSetBeforeLoaded && this.SelectedItem != null && this.Model.View != null)
                {
                    this.Model.View.MoveCurrentTo(this.SelectedItem);
                    if (!this.SelectedItems.Contains(this.SelectedItem))
                    {
                        this.SelectedItems.Add(this.SelectedItem);
                    }
                }
                else
                {
                    /// To clear the selected item on itemsoruce change.
                    this.SelectedItem = null;
                }

                // The first row will be seleted based on the SelectFirstRowOnLoad  
                if (this.SelectFirstRowOnLoad && this.ItemsSource != null && this.SelectedItem == null && this.Model.View != null && this.AutoFocusCurrentItem)
                {
                    if (this.Model.View.Records.Count > 0)
                        this.SelectedItem = this.Model.View.Records.GetItemAt(0);
                    else
                        this.SelectedItem = this.Model.View.CurrentItem;

                    if (this.Model.SelectedRanges.Count == 0)
                    {
                        int Rowindex = this.Model.CurrencyManager.CurrentRecordIndex > -1
                                           ? this.Model.CurrencyManager.CurrentRecordIndex
                                           : 0;
                        if (this.GroupedColumns.Count > 0)
                            this.Model.CurrencyManager.MoveTo(this.Model.CurrencyManager.CurrentRecordIndex);
                        else
                            this.Model.CurrencyManager.MoveTo(Rowindex);
                    }
                }
                else if (this.SelectFirstRowOnLoad)
                {
                    this.SelectedItem = null;
                }

                if (isGridLoaded && this.GroupDropAreaGrid != null)
                {
                    this.GroupDropAreaGrid.ApplyWidths();
                }

                //((INotifyCollectionChanged)this.VisibleColumns).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnVisibleColumnsChanged);
            }
            catch
            {

            }

            base.RaiseEvent(e);
        }

        public event GridRoutedEventHandler ItemsSourceChanged
        {
            add
            {
                this.AddHandler(GridDataControl.ItemsSourceChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.ItemsSourceChangedEvent, value);
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public object ItemsSource
        {
            get
            {
                return this.GetValue(GridDataControl.ItemsSourceProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ItemsSourceProperty, value);
            }
        }

        #endregion

        private GridDataTableModel ctorModel = null;

        /// <summary>
        /// Gets the model. Once the template gets applied, the Model object will return. Otherwise, it would be null. So use this after the AppliedTemplate flow is called.
        /// </summary>
        /// <value>The model.</value>
        public GridDataTableModel Model
        {
            get
            {
                if (this.InternalGrid != null)
                {

                    return this.InternalGrid.Model as GridDataTableModel;
                }

                if (this.ctorModel == null)
                {
                    this.ctorModel = this.OnModelCreated();
                    this.ctorModel.CurrencyManager.CurrentRecordSelectionChanged += new GridDataCurrentRecordSelectionChangedEventHandler(this.OnCurrentRecordSelectionChanged);
                }

                return this.ctorModel;
            }
        }

#if SyncfusionFramework4_0

        #region IsDynamicItemsSource (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsDynamicItemsSource property. Set this to true if ItemsSource is a collection of 'dynamic' objects. Default value is false.
        /// </summary>
        public bool IsDynamicItemsSource
        {
            get { return (bool)GetValue(GridDataControl.IsDynamicItemsSourceProperty); }
            set { SetValue(GridDataControl.IsDynamicItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty IsDynamicItemsSourceProperty = DependencyProperty.Register("IsDynamicItemsSource", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnIsDynamicItemsSourceChanged));

        private static void OnIsDynamicItemsSourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.IsDynamicItemsSource = (bool)args.NewValue;
        }

        #endregion
#endif
        
        private bool innerSelectionChange = false;

        private void OnCurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            this.innerSelectionChange = true;
            var view = this.Model.View;
            //var record = args.NewIndex > -1 && args.NewIndex < this.Model.SourceListCount ? view.Records[args.NewIndex] : null;
            object record;
            if (!this.Model.Table.HasGroups)
                record = args.NewIndex > -1 && args.NewIndex < this.Model.View.Records.Count ? view.Records[args.NewIndex] : null;
            else
                record = args.NewIndex > -1 && args.NewIndex < this.Model.View.TopLevelGroup.DisplayElements.Count ? view.TopLevelGroup.DisplayElements[args.NewIndex] : null;    
            
            if (this.Model.Table.HasGroups)
            {
                if (args.Record != null)
                {
                    record = args.Record as RecordEntry;
                }

                //if (this.Model.View.TopLevelGroup.DisplayElements.Count >= args.NewIndex)
                //{
                //    var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[args.NewIndex] as RecordEntry;
                //    if (recordEntry != null)
                //    {
                //        record = recordEntry;
                //    }
                //}
            }

            if (record != null)
            {
                this.SelectedItem = ((RecordEntry)record).Data;
            }
            else
            {
                this.SelectedItem = null;
            }
            this.innerSelectionChange = false;
        }

        /// <summary>
        /// Gets or sets the null filter text.
        /// </summary>
        /// <value>The null filter text.</value>
        public string NullFilterText
        {
            get
            {
                return (string)this.GetValue(GridDataControl.NullFilterTextProperty);
            }

            set
            {
                this.SetValue(GridDataControl.NullFilterTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the relations.
        /// </summary>
        /// <value>The relations.</value>
        public FreezableCollection<GridDataRelation> Relations
        {
            get
            {
                return this.TableProperties.Relations;
            }

            set
            {
                this.TableProperties.Relations = value;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.OneTimePopulateRelations"/> property.
        /// </summary>
        public static readonly DependencyProperty OneTimePopulateRelationsProperty = DependencyProperty.Register("OneTimePopulateRelations", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnOneTimePopulatePropertyChanged));

        private static void OnOneTimePopulatePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.OneTimePopulateRelations = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether OneTimePopulateRelations is true / false. If set to true,
        /// this will pre-populate the relations when the ItemsSource is bound to the grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [one time populate relations]; otherwise, <c>false</c>.
        /// </value>
        public bool OneTimePopulateRelations
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.OneTimePopulateRelationsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.OneTimePopulateRelationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the row background.
        /// </summary>
        /// <value>The row background.</value>
        public Brush RowBackground
        {
            get
            {
                return this.GetValue(GridDataControl.RowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.RowBackgroundProperty, value);
            }
        }

        #region RowForeground
        /// <summary>
        /// Gets or sets the row background.
        /// </summary>
        /// <value>The row background.</value>
        public Brush RowForeground
        {
            get
            {
                return this.GetValue(GridDataControl.RowForegroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.RowForegroundProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.RowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty RowForegroundProperty = DependencyProperty.Register(
            "RowForeground",
            typeof(Brush),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnRowForegroundChanged));

        private static void OnRowForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.RowForeground = (Brush)args.NewValue;
        }
        #endregion

        public GridDataFilterBarMode FilterBarMode
        {
            get { return (GridDataFilterBarMode)GetValue(FilterBarModeProperty); }
            set { SetValue(FilterBarModeProperty, value); }
        }


        public AlphaNumericFilterType AlphaNumericFilterType
        {
            get { return (AlphaNumericFilterType)GetValue(AlphaNumericFilterTypeProperty); }
            set { SetValue(AlphaNumericFilterTypeProperty, value); }
        }

        public bool ShowFilterBar
        {
            get { return (bool)GetValue(ShowFilterBarProperty); }
            set { SetValue(ShowFilterBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show add new row].
        /// </summary>
        /// <value><c>true</c> if [show add new row]; otherwise, <c>false</c>.</value>
        public bool ShowAddNewRow
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowAddNewRowProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowAddNewRowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show record plus minus].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [show record plus minus]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRecordPlusMinus
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowRecordPlusMinusProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowRecordPlusMinusProperty, value);
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
                return (bool)this.GetValue(GridDataControl.ShowRowHeaderProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowRowHeaderProperty, value);
            }
        }


        #region ShowRowHeaderArrow (DependencyProperty)

        /// <summary>
        /// Gets / Sets ShowRowHeaderArrow property.
        /// </summary>
        public bool ShowRowHeaderArrow
        {
            get { return (bool)GetValue(ShowRowHeaderArrowProperty); }
            set { SetValue(ShowRowHeaderArrowProperty, value); }
        }

        public static readonly DependencyProperty ShowRowHeaderArrowProperty = DependencyProperty.Register("ShowRowHeaderArrow", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnShowRowHeaderArrowChanged));

        private static void OnShowRowHeaderArrowChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ShowRowHeaderArrow = (bool)args.NewValue;
        }

        #endregion


        public static readonly DependencyProperty TablePropertiesProperty = DependencyProperty.Register("TableProperties", typeof(GridDataTableProperties), typeof(GridDataControl), new FrameworkPropertyMetadata(null));

        internal GridDataTableProperties TableProperties
        {
            get
            {
                return (GridDataTableProperties)this.GetValue(GridDataControl.TablePropertiesProperty);
            }
            set
            {
                this.SetValue(GridDataControl.TablePropertiesProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.IsGroupsExpanded"/> property.
        /// </summary>
        public static readonly DependencyProperty IsGroupsExpandedProperty = DependencyProperty.Register("IsGroupsExpanded", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnIsGroupsExpandedChanged));

        private bool isGroupsExpandedSetBeforeLoad = false;

        private static void OnIsGroupsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                grid.TableProperties.IsGroupsExpanded = value;
            }
            else
            {
                grid.isGroupsExpandedSetBeforeLoad = true;
            }
        }

        public AddNewRowBehaviour AddNewRowBehaviour
        {
            get { return (AddNewRowBehaviour)GetValue(AddNewRowBehaviourProperty); }
            set { SetValue(AddNewRowBehaviourProperty, value); }
        }

        public static readonly DependencyProperty AddNewRowBehaviourProperty =
            DependencyProperty.Register("AddNewRowBehaviour", typeof(AddNewRowBehaviour), typeof(GridDataControl), new PropertyMetadata(AddNewRowBehaviour.Default, OnAddnewRowBehaviourChanged));

        private static void OnAddnewRowBehaviourChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (AddNewRowBehaviour)args.NewValue;
                grid.TableProperties.AddNewRowBehaviour = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is groups expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is groups expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsGroupsExpanded
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsGroupsExpandedProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsGroupsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the grouped columns.
        /// </summary>
        /// <value>The grouped columns.</value>
        public FreezableCollection<GridDataGroupColumn> GroupedColumns
        {
            get
            {
                return this.TableProperties.GroupedColumns;
            }

            set
            {
                this.TableProperties.GroupedColumns = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort columns.
        /// </summary>
        /// <value>The sort columns.</value>
        public FreezableCollection<GridDataSortColumn> SortColumns
        {
            get
            {
                return this.TableProperties.SortColumns;
            }

            set
            {
                this.TableProperties.SortColumns = value;
            }
        }

        /// <summary>
        /// Gets or sets the summary rows.
        /// </summary>
        /// <value>The summary rows.</value>
        public FreezableCollection<GridDataSummaryRow> SummaryRows
        {
            get
            {
                return this.TableProperties.SummaryRows;
            }

            set
            {
                this.TableProperties.SummaryRows = value;
            }
        }

        /// <summary>
        /// Gets or sets the table summary rows.
        /// </summary>
        /// <value>The table summary rows.</value>
        public FreezableCollection<GridDataSummaryRow> TableSummaryRows
        {
            get
            {
                return this.TableProperties.TableSummaryRows;
            }

            set
            {
                this.TableProperties.TableSummaryRows = value;
            }
        }

        /// <summary>
        /// Gets or sets the stacked header rows.
        /// </summary>
        /// <value>The stacked header rows.</value>
        public FreezableCollection<GridDataStackedHeaderRow> StackedHeaderRows
        {
            get
            {
                return this.TableProperties.StackedHeaderRows;
            }

            set
            {
                this.TableProperties.StackedHeaderRows = value;
            }
        }

        /// <summary>
        /// Gets or sets the conditional formats.
        /// </summary>
        /// <value>The conditional formats.</value>
        public FreezableCollection<GridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return this.TableProperties.ConditionalFormats;
            }

            set
            {
                this.TableProperties.ConditionalFormats = value;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowTableSummaries"/>.
        /// </summary>
        public static readonly DependencyProperty ShowTableSummariesProperty = DependencyProperty.Register(
            "ShowTableSummaries",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnShowTableSummariesChanged));

        private static void OnShowTableSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ShowTableSummaries = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowTableSummaries is true / false.
        /// </summary>
        /// <value><c>true</c> if [show table summaries]; otherwise, <c>false</c>.</value>
        public bool ShowTableSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowTableSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowTableSummariesProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowGroupCaptionPlusMinus"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGroupCaptionPlusMinusProperty = DependencyProperty.Register(
            "ShowGroupCaptionPlusMinus",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnShowGroupCaptionPlusMinus));

        private static void OnShowGroupCaptionPlusMinus(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ShowGroupCaptionPlusMinus = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupCaptionPlusMinus is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show group caption plus minus]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGroupCaptionPlusMinus
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupCaptionPlusMinusProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupCaptionPlusMinusProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowGroupSummaries"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGroupSummariesProperty = DependencyProperty.Register(
            "ShowGroupSummaries",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(true, OnShowGroupSummariesChanged));

        private bool isShowGroupSummariesChanged = false;

        private static void OnShowGroupSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowGroupSummaries = (bool)args.NewValue;
            }
            else
            {
                grid.isShowGroupSummariesChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupSummaries is true / false.
        /// </summary>
        /// <value><c>true</c> if [show group summaries]; otherwise, <c>false</c>.</value>
        public bool ShowGroupSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupSummariesProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowGroupSummaryInCaption"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGroupSummaryInCaptionProperty = DependencyProperty.Register(
            "ShowGroupSummaryInCaption",
            typeof(bool),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnShowGroupSummaryInCaptionChanged));

        private bool isShowGroupSummaryInCaptionLoaded = false;

        private static void OnShowGroupSummaryInCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowGroupSummaryInCaption = (bool)args.NewValue;
            }
            else
            {
                grid.isShowGroupSummaryInCaptionLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupSummaryInCaption is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show group summary in caption]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGroupSummaryInCaption
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupSummaryInCaptionProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupSummaryInCaptionProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.GroupCaptionText"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupCaptionTextProperty = DependencyProperty.Register(
            "GroupCaptionText",
            typeof(string),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnGroupCaptionTextChanged));

        internal const string GroupCaptionConstant = "{ColumnName} : {Key} - {ItemsCount} Items";

        private bool isGroupCaptionTextChanged = false;

        private static void OnGroupCaptionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            var value = args.NewValue.ToString();
            if (grid.isGridLoaded)
            {
                grid.TableProperties.GroupCaptionText = value != string.Empty ? value : GridDataControl.GroupCaptionConstant;
            }
            else
            {
                grid.isGroupCaptionTextChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the Group Caption Text. There are three properties that are
        /// necessary for the caption to display, 
        /// <para></para>
        /// <list type="bullet">
        /// <item>
        /// <description>Name - Defines the Name of the group.</description></item>
        /// <item>
        /// <description>Count - Defines the group count.</description></item>
        /// <item>
        /// <description>ColumnName - Defines the name of the column that is
        /// grouped.</description></item></list>
        /// <para></para>
        /// <para>Default value of the GroupCaptionText is, {ColumnName} : {Name} - {Count}
        /// Items. If the ShowGroupSummaryInCaption is set, this value would be overridden by 
        /// the CaptionSummaryRow.Title.</para>
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(GridDataFormatConverter))]
        public string GroupCaptionText
        {
            get
            {
                return (string)this.GetValue(GridDataControl.GroupCaptionTextProperty);
            }

            set
            {
                this.SetValue(GridDataControl.GroupCaptionTextProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.CaptionSummaryRow"/> property.
        /// </summary>
        public static readonly DependencyProperty CaptionSummaryRowProperty = DependencyProperty.Register(
            "CaptionSummaryRow",
            typeof(GridDataSummaryRow),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnCaptionSummaryRowChanged));

        private bool isCaptionSummaryRowChanged = false;

        private static void OnCaptionSummaryRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.CaptionSummaryRow = (GridDataSummaryRow)args.NewValue;
            }
            else
            {
                grid.isCaptionSummaryRowChanged = true;
            }
        }

        #region StyleManager

        public GridDataStyleManager StyleManager
        {
            get { return (GridDataStyleManager)GetValue(StyleManagerProperty); }
            set { SetValue(StyleManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleManagerProperty =
            DependencyProperty.Register("StyleManager", typeof(GridDataStyleManager), typeof(GridDataControl), new PropertyMetadata(OnStyleManagerChanged));


        private static void OnStyleManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl dataControl = d as GridDataControl;
            GridDataStyleManager styleManager = args.NewValue as GridDataStyleManager;
            if (styleManager != null)
            {
                styleManager.gridDataControl = dataControl;
                if (dataControl.isGridLoaded)
                {
                    dataControl.Model.TableProperties.StyleManager = styleManager;
                    if (dataControl.InternalGrid != null)
                        dataControl.InternalGrid.StyleManager = styleManager;
                }
                else
                {
                    dataControl.isStyleManagerChanged = true;
                }
            }
        }

        private bool isStyleManagerChanged = false;

        #endregion

        #region EnableLegacyStyle

        private bool isEnableLegacyStyleBeforeGridLoaded = true;

        public bool EnableLegacyStyle
        {
            get { return (bool)GetValue(EnableLegacyStyleProperty); }
            set { SetValue(EnableLegacyStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableLegacyStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableLegacyStyleProperty =
            DependencyProperty.Register("EnableLegacyStyle", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableLegacyStyleChanged));

        private static void OnEnableLegacyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.EnableLegacyStyle = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableLegacyStyleBeforeGridLoaded = true;
            }
        }

        #endregion

        #region EnableParentTableStyleToChildTable
        private bool isEnableParentStyleToChildTableBeforeGridLoaded = true;
        /// <summary>
        /// Gets or sets the parent table style to child table.
        /// </summary>
        public bool EnableParentStyleToChildTable
        {
            get { return (bool)GetValue(EnableParentStyleToChildTableProperty); }
            set { SetValue(EnableParentStyleToChildTableProperty, value); }
        }
        
        public static readonly DependencyProperty EnableParentStyleToChildTableProperty =
            DependencyProperty.Register("EnableParentStyleToChildTable", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableParentStyleToChildTableChanged));

        private static void OnEnableParentStyleToChildTableChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.EnableParentStyleToChildTable = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableParentStyleToChildTableBeforeGridLoaded = true;
            }
        }
        #endregion

        #region EnableLegacyFiltering

        public static readonly DependencyProperty EnableLegacyFilteringProperty =
           DependencyProperty.Register("EnableLegacyFiltering", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnEnableLegacyFilteringChanged));

        public bool EnableLegacyFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.EnableLegacyFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataControl.EnableLegacyFilteringProperty, value);
            }
        }

        private static void OnEnableLegacyFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.EnableLegacyFiltering = (bool)args.NewValue;

        }

        #endregion


        /// <summary>
        /// Gets or sets the caption summary row.
        /// </summary>
        /// <value>The caption summary row.</value>
        public GridDataSummaryRow CaptionSummaryRow
        {
            get
            {
                return (GridDataSummaryRow)this.GetValue(GridDataControl.CaptionSummaryRowProperty);
            }

            set
            {
                this.SetValue(GridDataControl.CaptionSummaryRowProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowGroupDropArea"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGroupDropAreaProperty = DependencyProperty.Register(
            "ShowGroupDropArea",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnShowGroupDropAreaChanged));


        private bool isShowGroupDropAreaChangedBeforeGridLoaded = false;

        private static void OnShowGroupDropAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            var value = (bool)args.NewValue;
            grid.TableProperties.ShowGroupDropArea = value;
            if (value && grid.GroupedColumns.Count > 0)
                grid.GroupDropAreaGrid.InvalidateCells();//Have to InValidate the GroupDropAreaGrid if any column already grouped
            if (value && !grid.AllowDragColumns)
                grid.AllowDragColumns = true;
        }
          

        private void LoadGroupDropAreaGrid()
        {
            //var groupDropAreaScrollViewer = this.GetTemplateChild("PART_GroupDropAreaScrollViewer") as ScrollViewer;
            if (this.InternalGrid != null && this.InternalGrid.Model is GridDataTableModel)
            {
                this.GroupDropAreaGrid.AttachParentGrid((GridDataTableModel)this.InternalGrid.Model, this.InternalGrid);
                if (this.GroupDropAreaText != null && this.GroupDropAreaText != string.Empty)
                {
                    this.GroupDropAreaGrid.Model.GroupDropAreaText = this.GroupDropAreaText;
                    this.GroupDropAreaGrid.InvalidateCells();
                }
            }
            
           
        }
#if !SILVERLIGHT
        GridDataColumnChooserWindow chooser;
        GridDataVisibleColumns ColumnChooserColumns;
        /// <summary>
        /// Shows the column chooser.
        /// </summary>
        public void ShowColumnChooser()
        {
            ShowColumnChooser(null);
        }

        /// <summary>
        /// Shows the column chooser.
        /// </summary>
        /// <param name="Columnchooseraction">The columnchooseraction.</param>
        public void ShowColumnChooser(Action<GridDataColumnChooserWindow> Columnchooseraction)
        {
            if (chooser == null)
            {
                chooser = new GridDataColumnChooserWindow() { Topmost = true };
                this.AllowDragColumns = true;
                if (Columnchooseraction != null)
                    Columnchooseraction(chooser);
                chooser.InitalizeColumnChooser((GridDataTableModel)this.InternalGrid.Model,ColumnChooserColumns);
                ((INotifyCollectionChanged)chooser.newColumnList).CollectionChanged += new NotifyCollectionChangedEventHandler(GridDataControl_CollectionChanged);
                chooser.Closed += (chooser_Closed);
            }
            this.ColumnChooserGrid = chooser;
            if (!ColumnChooserGrid.IsLoaded)
                this.ColumnChooserGrid.Show();
        }

        void GridDataControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                ColumnChooserColumns.Add((GridDataVisibleColumn)e.NewItems[0]);
            if (e.Action == NotifyCollectionChangedAction.Remove)
                ColumnChooserColumns.Remove((GridDataVisibleColumn)e.OldItems[0]);
        }

        /// <summary>
        /// Handles the Closed event of the chooser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void chooser_Closed(object sender, EventArgs e)
        {
            chooser.Closed -= (chooser_Closed);
            ((INotifyCollectionChanged)chooser.newColumnList).CollectionChanged -= new NotifyCollectionChangedEventHandler(GridDataControl_CollectionChanged);
            this.chooser = null;
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupDropArea is true / false.
        /// </summary>
        /// <value><c>true</c> if [show group drop area]; otherwise, <c>false</c>.</value>
        public bool ShowGroupDropArea
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupDropAreaProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupDropAreaProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.GroupDropAreaText"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupDropAreaTextProperty = DependencyProperty.Register(
            "GroupDropAreaText",
            typeof(string),
            typeof(GridDataControl),
            new FrameworkPropertyMetadata(OnGroupDropAreaTextChanged));

        private bool isGroupDropAreaTextChanged = false;

        private static void OnGroupDropAreaTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                if (grid.GroupDropAreaGrid != null && grid.ShowGroupDropArea)
                {
                    grid.GroupDropAreaGrid.Model.GroupDropAreaText = (string)args.NewValue;
                    grid.GroupDropAreaGrid.InvalidateCells();
                }
            }
            else
            {
                grid.isGroupDropAreaTextChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the group drop area text.
        /// </summary>
        /// <value>The group drop area text.</value>
        public string GroupDropAreaText
        {
            get
            {
                return (string)this.GetValue(GridDataControl.GroupDropAreaTextProperty);
            }

            set
            {
                this.SetValue(GridDataControl.GroupDropAreaTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visible columns.
        /// </summary>
        /// <value>The visible columns.</value>
        public GridDataVisibleColumns VisibleColumns
        {
            get
            {
                return this.TableProperties.VisibleColumns;
            }

            set
            {
                this.TableProperties.VisibleColumns = value;
            }
        }
        /// <summary>
        /// Gets or sets the custom visual style.
        /// </summary>
        /// <value>The custom visual style.</value>
        public IGridDataVisualStyle CustomVisualStyle
        {
            get
            {
                return (IGridDataVisualStyle)this.GetValue(GridDataControl.CustomVisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataControl.CustomVisualStyleProperty, value);
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
                return (VisualStyle)this.GetValue(GridDataControl.VisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataControl.VisualStyleProperty, value);
            }
        }

        #region RowDetailsTemplate

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        public DataTemplate DetailsViewTemplate
        {
            get { return (DataTemplate)GetValue(DetailsViewTemplateProperty); }
            set { SetValue(DetailsViewTemplateProperty, value); }
        }          

        #endregion


        private void HookGrid()
        {
            if (this.ctorModel == null)
            {
                this.ctorModel = OnModelCreated();
                this.ctorModel.CurrencyManager.CurrentRecordSelectionChanged += this.OnCurrentRecordSelectionChanged;
            }

            //// Hook the internal dependency
            GridDataTableModel model = this.ctorModel;
            this.ctorModel = null;

            //// set the table properties before the grid is set
            model.TableProperties = this.TableProperties;
            this.InternalGrid.Model = model;
            model.Grid = this.InternalGrid;

            if (this.isEnableLegacyStyleBeforeGridLoaded)
            {
                model.TableProperties.EnableLegacyStyle = this.EnableLegacyStyle;
            }

            if (this.isEnableParentStyleToChildTableBeforeGridLoaded)
            {
                model.TableProperties.EnableParentStyleToChildTable = this.EnableParentStyleToChildTable;
            }

            if (this.isEnableVisualStyleForEditorsSetBeforeLoaded)
            {
                model.TableProperties.EnableVisualStyleForEditors = this.EnableVisualStyleForEditors;
            }

            if (this.isVerticalPixelScrollChanged)
            {
                model.Grid.VerticalPixelScroll = this.VerticalPixelScroll;
            }

            if (this.isHorizontalPixelScrollChanged)
            {
                model.Grid.HorizontalPixelScroll = this.HorizontalPixelScroll;
            }

            if (this.isDefaultHeaderRowHeightChangedBeforeGridLoaded)
            {
                model.TableProperties.DefaultHeaderRowHeight = this.DefaultHeaderRowHeight;
            } 

            #region BlendStyling support code

            if (this.isEnableBlendStyleSetBeforeLoaded)
            {
                this.InternalGrid.EnableBlendStyling = this.EnableBlendStyling;
            }

            if (this.isRowStyleSetBeforeGridLoaded)
            {
                this.InternalGrid.RowStyle = this.RowStyle;
            }

            if (this.isAlternateRowStyleSetBeforeGridLoaded)
            {
                this.InternalGrid.AlternateRowStyle = this.AlternateRowStyle;
            }

            #endregion

            #region EnableRenderOptimization code

            if (this.isEnableRenderOptimizationSetBeforeLoaded)
            {
                this.InternalGrid.EnableRenderOptimization = this.EnableRenderOptimization;
            }
            #endregion

            if (isStyleManagerChanged)
            {
                model.TableProperties.StyleManager = this.StyleManager;
                if (InternalGrid != null)
                {
                    InternalGrid.StyleManager = StyleManager;
                }
            }

            if (this.isCaptionSummaryRowChanged)
            {
                model.TableProperties.CaptionSummaryRow = this.CaptionSummaryRow;
            }

            if (this.isShowAddNewRowChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowAddNewRow = this.ShowAddNewRow;
            }

            if (this.isShowGroupDropAreaChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowGroupDropArea = this.ShowGroupDropArea;
            }

            if (this.isShowFiltersChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowFilters = this.ShowFilters;
            }

            if (this.isSelectFirstRowOnLoadChangedBeforeGridLoaded)
            {
                model.TableProperties.SelectFirstRowOnLoad = this.SelectFirstRowOnLoad;
            }

            if (this.isShowColumnOptionsPropertyChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowColumnOptions = this.ShowColumnOptions;
            }

            if (isHeaderContextMenuLoadedBeforeGridLoaded && this.HeaderContextMenuItems!=null)
                model.TableProperties.HeaderContextMenuItems = this.HeaderContextMenuItems;

            if (isGroupHeaderContextMenuLoadedBeforeGridLoaded && this.GroupHeaderContextMenuItems!=null)
                model.TableProperties.GroupHeaderContextMenuItems = this.GroupHeaderContextMenuItems;

            if (isRecordContextMenuLoadedBeforeGridLoaded && this.RecordContextMenuItems != null)
                model.TableProperties.RecordContextMenuItems = this.RecordContextMenuItems;

            if (isEnableContextMenuChanged)
                model.EnableContextMenu = this.EnableContextMenu;

            if (this.isNotifyComplexPropertyChangedSetBeforeGridLoaded)
                model.TableProperties.NotifyComplexPropertyChanges = this.NotifyComplexPropertyChanges;

            if (this.isItemsSourceLoadedBeforeGridLoaded)
            {
                model.TableProperties.ItemsSource = this.ItemsSource;
                this.OnItemsSourceChanged(new SyncfusionRoutedEventArgs(GridDataControl.ItemsSourceChangedEvent, this));
                
                //This code added to select first record when grid grouped before loaded.
                //We skipped the MoveTo for grouping case in Dispatcher CurrentRecordManager.MoveTo, so we handle this case here.
                if (this.SelectFirstRowOnLoad && this.Model.Table.HasGroups)
                {
                    var rowIndex = this.Model.ResolvePositionToIndex(this.Model.View.CurrentPosition);
                    this.Model.CurrencyManager.CurrentCell.MoveTo(rowIndex,this.Model.ResolveDefaultColumnOffset());
                }
            }            

            if(this.isGroupDropAreaGridHeightChangedBeforeGridLoaded)
            {
                model.TableProperties.GroupDropAreaHeight = this.GroupDropAreaHeight;
            }

            if (this.ItemsSource != null && !this.ensuredProperties)
            {
                this.EnsureProperties();
            }

            else if (!this.ensuredProperties)
                this.EnsurePropertiesOnLoad();

            if (this.DisableGridScrollViewer && this.Model != null)
            {
                this.Model.Initialized += OnModelInitialized;
            }

            if (this.isVisualStyleChangedBeforeGridLoaded)
            {
                model.TableProperties.CustomVisualStyle = this.CustomVisualStyle;
                model.TableProperties.VisualStyle = this.VisualStyle;
            }
            else
            {
                if (!EnableLegacyStyle)
                    model.GridVisualStyle = new GridDataDefaultGridVisualStyle();
                else
                    model.GridVisualStyle = new GridDataLegacyDefaultGridVisualStyle();
            }
            if (this.isColumnSizerChangedBeforeGridLoaded)
            {
                //model.ColumnAutoSizer.IsGridDataControlLoaded = true;
                model.TableProperties.ColumnSizer = this.ColumnSizer;
            }

            this.EnsurePrintProperties();

            this.OnModelLoaded();
        }

        protected virtual GridDataTableProperties GetTableProperties()
        {
            return new GridDataTableProperties();
        }

        protected virtual GridDataTableModel OnModelCreated()
        {
            return new GridDataTableModel();
        }

        protected virtual void OnModelLoaded()
        {
            // this.FlowDirection - This is WPF Control Property and not added by syncfusion. Hence this can't be added again and OnPropertyChanged event can't be triggered for it. 
            // FlowDirection"RightToLeft" for Grid workd with Two instace.
            // Instance 1 :  By Syncfusion  - We just render all the cells as image which will be the mirror view of the image . This will be done in OnInitilizeContent() method of each Cell Renderer and GridTextBoxPaint.cs
            //Once we done it we will get the Grid in LeftToRight Direction only but all the cells will be in mirror view image.
            // Once you break the link betwwen thisFlowdirection and this.Model.TableStyle.FlowDirection and Set RightToLEft Only for TableStyle.FlowDirection you can see this. 
            // Instance 2: Now we Allow WPF to Render our Grid in "RightToLEft" Direction. WPF will do the rest of the WORK.
 
            // raise the Model loaded event after everything is set
            if (this.ModelLoaded != null)
            {
                this.ModelLoaded(this, EventArgs.Empty);
            }

            if (this.TableProperties.Relations.Count == 0)
            {
                this.TableProperties.NeedToRefresh = true;
            }

            if (this.TableProperties != null && this.TableProperties.Model != null && this.VisualStyle == VisualStyle.Metro && GridDataControl.GetOverrideVisualStyle(this))
                this.TableProperties.UpdateVisualStyle();
        }

        private void OnModelInitialized(object sender, EventArgs e)
        {
            if (this.DisableGridScrollViewer)
            {
                var scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
                if (scrollViewer != null)
                {
                    scrollViewer.CanContentScroll = false;
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    this.Model.Initialized -= OnModelInitialized;
                }
            }
        }

        private void SetUpdateModeProperty()
        {
            if (this.isUpdateModeChangedBeforeGridLoaded && this.VisibleColumns.Count >= 1)
            {
                this.Model.TableProperties.UpdateMode = this.UpdateMode;
            }
        }

        private bool ensuredProperties = false;
        private void EnsureProperties()
        {
            GridDataTableModel model = this.Model;
            EnsurePropertiesOnLoad();
            if (this.isEnableLegacyStyleBeforeGridLoaded)
            {
                model.TableProperties.EnableLegacyStyle = this.EnableLegacyStyle;
            }

            if (this.isEnableVisualStyleForEditorsSetBeforeLoaded)
            {
                model.TableProperties.EnableVisualStyleForEditors = this.EnableVisualStyleForEditors;
            }

            if (this.isColumnOptionPaneStyleSetBeforeGridLoaded)
            {
                this.InternalGrid.ColumnOptionPaneStyle = this.ColumnOptionPaneStyle;
            }

            if (this.isAllowGroupChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowGroup = true;
            }

            if (this.isShowRecordPlusMinusChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowRecordPlusMinus = this.ShowRecordPlusMinus;
            }

            if (this.isShowRowHeaderChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowRowHeader = this.ShowRowHeader;
            }

            if (this.isAllowSortChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowSort = this.AllowSort;
            }

            if (this.isSortingOptionsChangedBeforeGridLoaded)
            {
                model.TableProperties.SortingOptions = this.SortingOptions;
            }

            if (this.isAllowDragChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowDragColumns = this.AllowDragColumns;
            }

            if (this.isAllowResizeRowsChangedBeforeLoaded || this.AllowResizeRows)
            {
                model.TableProperties.AllowResizeRows = this.AllowResizeRows;
            }

            if (this.isAllowResizeColumnsChangedBeforeLoaded)
            {
                model.TableProperties.AllowResizeColumns = this.AllowResizeColumns;
            }

            // set groupdroparea model
            if (this.ShowGroupDropArea && this.GroupDropAreaGrid != null)
            {
                this.GroupDropAreaGrid.AttachParentGrid(model, this.InternalGrid);
            }

            if (this.hasFontSizeChanged || hasFontFamilyChanged || hasFontStyleChanged || hasFontWeightChanged || hasFontStretchChanged)
            {
                this.Model.ApplyFont();
            }

            if (this.isDragIndicatorInnerBrushChangedBeforeGridLoaded)
            {
                model.TableProperties.DragIndicatorInnerBrush = this.DragIndicatorInnerBrush;

            }

            if (this.isDragIndicatorOuterBrushChangedBeforeGridLoaded)
            {
                model.TableProperties.DragIndicatorOuterBrush = this.DragIndicatorOuterBrush;

            }

            if (this.isUpdateModeChangedBeforeGridLoaded)
            {
                SetUpdateModeProperty();
            }

            //if (this.isHeaderStyleSetBeforeGridLoaded)
            //{
            //    this.InternalGrid.HeaderStyle = this.HeaderStyle;
            //}

            if (this.isGroupDropAreaTextChanged && this.ShowGroupDropArea && this.GroupDropAreaGrid != null && this.GroupDropAreaGrid.Model != null)
            {
                this.GroupDropAreaGrid.Model.GroupDropAreaText = this.GroupDropAreaText;
            }

            if (this.isGroupCaptionTextChanged)
            {
                model.TableProperties.GroupCaptionText = this.GroupCaptionText != string.Empty ? this.GroupCaptionText : GridDataControl.GroupCaptionConstant;
            }

            if (this.isShowGroupSummariesChanged)
            {
                model.TableProperties.ShowGroupSummaries = this.ShowGroupSummaries;
            }

            if (this.isShowGroupSummaryInCaptionLoaded)
            {
                model.TableProperties.ShowGroupSummaryInCaption = this.ShowGroupSummaryInCaption;
            }

            if (this.isGroupsExpandedSetBeforeLoad)
            {
                model.TableProperties.IsGroupsExpanded = this.IsGroupsExpanded;
            }

            if (this.isExpandGroupsWhenGroupedPropertyChangedBeforeGridLoaded)
            {
                model.TableProperties.ExpandGroupsWhenGrouped = this.ExpandGroupsWhenGrouped;
            }

            if (this.isShowErrorTooltipLoadedBeforeGrid || this.ShowErrorTooltips)
            {
                GridTooltipService.SetShowErrorTooltips(this.InternalGrid, this.ShowErrorTooltips);
            }

            if (this.isShowTooltipsChangedBeforeGridLoaded || this.ShowTooltips)
            {
                GridTooltipService.SetShowTooltips(this.InternalGrid, this.ShowTooltips);
                model.TableProperties.ShowTooltips = this.ShowTooltips;
            }

            if (this.isNotifyPropertyChangedSetBeforeGridLoaded)
            {
                model.TableProperties.NotifyPropertyChanges = this.NotifyPropertyChanges;
            }

            if (this.isNotifyComplexPropertyChangedSetBeforeGridLoaded)
                model.TableProperties.NotifyComplexPropertyChanges = this.NotifyComplexPropertyChanges;

            if (this.AutoPopulateRelations)
            {
                this.Relations.ForEach(rd =>
                    {
                        rd.TableProperties.InitializeFromInternal(this.Model.TableProperties, false);
                    });
            }



            if (this.isEnableOptimizationsSetBeforeGridLoaded)
            {
                model.TableProperties.EnableOptimizations = this.EnableOptimizations;
            }
            model.TableProperties.AddNewRowBehaviour = this.AddNewRowBehaviour;
            this.ensuredProperties = true;
        }

        /// <summary>
        /// This will invoked from HookGrid when ItemSource is null. In othercases it will be invoked from EnsureProperties
        /// </summary>
        private void EnsurePropertiesOnLoad()
        {
            if (this.isHeaderStyleSetBeforeGridLoaded)
            {
                this.InternalGrid.HeaderStyle = this.HeaderStyle;
                this.Model.TableProperties.HeaderStyle = this.HeaderStyle;
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether [show filters].
        /// </summary>
        /// <value><c>true</c> if [show filters]; otherwise, <c>false</c>.</value>
        public bool ShowFilters
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowFiltersProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowFiltersProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show column options].
        /// </summary>
        /// <value><c>true</c> if [show column options]; otherwise, <c>false</c>.</value>
        public bool ShowColumnOptions
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowColumnOptionsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowColumnOptionsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ExpandGroupsWhenGrouped is true/false.
        /// </summary>
        /// <value><c>true</c> if [Expands all groups when the column is grouped]; otherwise, <c>false</c>.</value>
        public bool ExpandGroupsWhenGrouped
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ExpandGroupsWhenGroupedProperty);
            }
            set
            {
                this.SetValue(GridDataControl.ExpandGroupsWhenGroupedProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ExpandGroupsWhenGrouped"/> property.
        /// </summary>
        public static readonly DependencyProperty ExpandGroupsWhenGroupedProperty = DependencyProperty.Register(
            "ExpandGroupsWhenGrouped",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnExpandGroupsWhenGroupedPropertyChanged));

        private bool isExpandGroupsWhenGroupedPropertyChangedBeforeGridLoaded = false;

        private static void OnExpandGroupsWhenGroupedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ExpandGroupsWhenGrouped = (bool)args.NewValue;
            }
            else
            {
                grid.isExpandGroupsWhenGroupedPropertyChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowErrorTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowErrorTooltipsProperty = DependencyProperty.Register("ShowErrorTooltips", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(true, OnShowErrorTooltipsPropertyChanged));

        private bool isShowErrorTooltipLoadedBeforeGrid = false;

        private static void OnShowErrorTooltipsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                grid.TableProperties.ShowErrorTooltips = value;
                GridTooltipService.SetShowErrorTooltips(grid.InternalGrid, value);
            }
            else
            {
                grid.isShowErrorTooltipLoadedBeforeGrid = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show error tooltips].
        /// </summary>
        /// <value><c>true</c> if [show error tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowErrorTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowErrorTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowErrorTooltipsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.Register("ShowTooltips", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(OnShowTooltipsChanged));

        private bool isShowTooltipsChangedBeforeGridLoaded = false;

        private static void OnShowTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                grid.TableProperties.ShowTooltips = value;
                GridTooltipService.SetShowTooltips(grid.InternalGrid, (bool)args.NewValue);
            }
            else
            {
                grid.isShowTooltipsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowTooltips is true / false.
        /// </summary>
        /// <value><c>true</c> if [show tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowTooltipsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.NotifyPropertyChanges"/>.
        /// </summary>
        public static readonly DependencyProperty NotifyPropertyChangesProperty = DependencyProperty.Register("NotifyPropertyChanges", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnNotifyPropertyChanged));

        private bool isNotifyPropertyChangedSetBeforeGridLoaded = false;
        private static void OnNotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.NotifyPropertyChanges = (bool)args.NewValue;
            }
            else
            {
                grid.isNotifyPropertyChangedSetBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether NotifyPropertyChanges is true / false. Set this to true, GridDataControl will listen to
        /// property changes.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [notify property changes]; otherwise, <c>false</c>.
        /// </value>
        public bool NotifyPropertyChanges
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.NotifyPropertyChangesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.NotifyPropertyChangesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether NotifyComplexPropertyChanges is true / false. Set this to true, GridDataControl will listen to
        /// complex property changes. 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [notify complex property changes]; otherwise, <c>false</c>.
        /// </value>
        public bool NotifyComplexPropertyChanges
        {
            get { return (bool)GetValue(NotifyComplexPropertyChangesProperty); }
            set { SetValue(NotifyComplexPropertyChangesProperty, value); }
        }

        public static readonly DependencyProperty NotifyComplexPropertyChangesProperty =
            DependencyProperty.Register("NotifyComplexPropertyChanges", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnNotifyComplexPropertyChanged));

        private bool isNotifyComplexPropertyChangedSetBeforeGridLoaded = false;
        private static void OnNotifyComplexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.NotifyComplexPropertyChanges = (bool)args.NewValue;
            }
            else
            {
                grid.isNotifyComplexPropertyChangedSetBeforeGridLoaded = true;
            }
        }

        public bool DisableGridScrollViewer
        {
            get { return (bool)GetValue(DisableGridScrollViewerProperty); }
            set { SetValue(DisableGridScrollViewerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableGridScrollViewer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableGridScrollViewerProperty =
            DependencyProperty.Register("DisableGridScrollViewer", typeof(bool), typeof(GridDataControl), new UIPropertyMetadata(false));

        #region AllowNestedGridPadding

        /// <summary>
        /// AllowNestedGridPadding Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowNestedGridPaddingProperty =
            DependencyProperty.Register("AllowNestedGridPadding", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnAllowNestedGridPaddingChanged)));

        /// <summary>
        /// Gets or sets the AllowNestedGridPadding property. This dependency property 
        /// indicates whether padding value is set for nested grid or not.        
        /// </summary>
        public bool AllowNestedGridPadding
        {
            get { return (bool)GetValue(AllowNestedGridPaddingProperty); }
            set { SetValue(AllowNestedGridPaddingProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AllowNestedGridPadding property.
        /// </summary>
        private static void OnAllowNestedGridPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl target = (GridDataControl)d;
            target.TableProperties.AllowNestedGridPadding = (bool)e.NewValue;
        }

        #endregion

        #region VerticalPixelScroll

        /// <summary>
        /// VerticalPixelScroll Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalPixelScrollProperty =
            DependencyProperty.Register("VerticalPixelScroll", typeof(bool), typeof(GridDataControl),
                new FrameworkPropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnVerticalPixelScrollChanged)));

        /// <summary>
        /// Gets or sets the VerticalPixelScroll property. This dependency property 
        /// indicates VerticalPixelScroll is enabled or not.
        /// </summary>
        public bool VerticalPixelScroll
        {
            get { return (bool)GetValue(VerticalPixelScrollProperty); }
            set { SetValue(VerticalPixelScrollProperty, value); }
        }

        private bool isVerticalPixelScrollChanged = false;

        /// <summary>
        /// Handles changes to the VerticalPixelScroll property.
        /// </summary>
        private static void OnVerticalPixelScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            if (grid.InternalGrid != null)
            {
                grid.InternalGrid.VerticalPixelScroll = (bool)e.NewValue;
            }
            else
            {
                grid.isVerticalPixelScrollChanged = true;
            }
        }

        #endregion

        #region HorizontalPixelScroll

        /// <summary>
        /// HorizontalPixelScroll Dependency Property
        /// </summary>
        public static readonly DependencyProperty HorizontalPixelScrollProperty =
            DependencyProperty.Register("HorizontalPixelScroll", typeof(bool), typeof(GridDataControl),
                new FrameworkPropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnHorizontalPixelScrollChanged)));

        /// <summary>
        /// Gets or sets the HorizontalPixelScroll property. This dependency property 
        /// indicates HorizontalPixel is enabled for GridDataControl.
        /// </summary>
        public bool HorizontalPixelScroll
        {
            get { return (bool)GetValue(HorizontalPixelScrollProperty); }
            set { SetValue(HorizontalPixelScrollProperty, value); }
        }

        private bool isHorizontalPixelScrollChanged = false;

        /// <summary>
        /// Handles changes to the HorizontalPixelScroll property.
        /// </summary>
        private static void OnHorizontalPixelScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            if (grid.InternalGrid != null)
            {
                grid.InternalGrid.HorizontalPixelScroll = (bool)e.NewValue;
            }
            else
            {
                grid.isHorizontalPixelScrollChanged = true;
            }
        }

        #endregion

        #region AutoFocusCurrentItem (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AutoFocusCurrentItem property.
        /// </summary>
        public bool AutoFocusCurrentItem
        {
            get { return (bool)GetValue(AutoFocusCurrentItemProperty); }
            set { SetValue(AutoFocusCurrentItemProperty, value); }
        }

        public static readonly DependencyProperty AutoFocusCurrentItemProperty = DependencyProperty.Register("AutoFocusCurrentItem", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnAutoFocusCurrentItemChanged));

        private static void OnAutoFocusCurrentItemChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.AutoFocusCurrentItem = (bool)args.NewValue;
        }

        #endregion

        #region EnableOptimizations (DependencyProperty)

        /// <summary>
        /// Gets / sets to enable optimizations on the grid. When set to true, it will exclude Covered ranges and other factors that affect performance. This property has to be used when there are fast updates, and no covered cells being used.
        /// </summary>
        public bool EnableOptimizations
        {
            get { return (bool)GetValue(EnableOptimizationsProperty); }
            set { SetValue(EnableOptimizationsProperty, value); }
        }

        public static readonly DependencyProperty EnableOptimizationsProperty = DependencyProperty.Register("EnableOptimizations", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableOptimizationsChanged));

        private bool isEnableOptimizationsSetBeforeGridLoaded = false;
        private static void OnEnableOptimizationsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (!grid.IsLoaded)
            {
                grid.isEnableOptimizationsSetBeforeGridLoaded = true;
            }
            else
            {
                grid.TableProperties.EnableOptimizations = (bool)args.NewValue;
            }
        }

        #endregion

        private static void OnStatusBarMessageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var gdc = (GridDataControl)sender;
            gdc.ShowHideStatusBar();
        }

        private void ShowHideStatusBar()
        {
            if (PART_BorderStatusBar != null)
            {
                //var value = GridDataControl.ResolveVisualStyleToSkinStorage(this.VisualStyle);
                //SkinStorage.SetVisualStyle(PART_BorderStatusBar, value);

                if (!string.IsNullOrEmpty(StatusBarMessage) && this.ShowFilterStatusMessage == true) //  && this.ShowFilterStatusMessage != null removed since bool never equal to null
                    PART_BorderStatusBar.Height = StatusBarHeight;
                else
                    PART_BorderStatusBar.Height = 0;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(ScrollChrome) || e.OriginalSource.GetType() == typeof(Rectangle))
                e.Handled = true;
            base.OnMouseDoubleClick(e); 
        }

        private static void OnAddNewRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var datagrid = d as GridDataControl;
            datagrid.TableProperties.AddNewRowPosition = (Position)args.NewValue;
        }

        private static void OnUnboundRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
            {
            var datagrid = d as GridDataControl;
            datagrid.TableProperties.UnboundRowPosition = (Position)args.NewValue;
            }

        private static void OnAllowDeletePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AllowDelete = (bool)args.NewValue;
        }

        private static void OnAllowMultipleRecordDeletionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AllowMultipleRecordDeletion = (bool)args.NewValue;
        }

        private static void OnAllowEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AllowEdit = (bool)args.NewValue;
        }

        private bool isAllowGroupChangedBeforeGridLoaded = false;

        private static void OnAllowGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowGroup = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowGroupChangedBeforeGridLoaded = true;
            }
        }

        private bool isAllowSortChangedBeforeGridLoaded = false;

        private static void OnAllowSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowSort = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowSortChangedBeforeGridLoaded = true;
            }
        }

        private bool isSortingOptionsChangedBeforeGridLoaded = false;

        private static void OnSortingOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.SortingOptions = (SortingOptions)args.NewValue;
            }
            else
            {
                grid.isSortingOptionsChangedBeforeGridLoaded = true;
            }
        }

        private static void OnAlternatingRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlternatingRowBackground = (Brush)args.NewValue;
        }

        private static void OnAlternatingRowCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlternatingRowCount = (int)args.NewValue;
        }

        private static void OnDetailsViewTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as GridDataControl).TableProperties.DetailsViewTemplate = (DataTemplate)args.NewValue;
        }

        private Border PART_BorderStatusBar;
        private Button PART_CloseButton;

        private ScrollViewer scrollViewer;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.PART_BorderStatusBar = this.GetTemplateChild("PART_BorderStatusBar") as Border;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            SetGroupDropAreaGrid();
            if (this.PART_CloseButton != null)
            {
                this.PART_CloseButton.Click += new RoutedEventHandler(PART_CloseButton_Click);
            }
            this.InternalGrid = this.GetGridHost();
            if (this.InternalGrid != null)
            {
                scrollViewer.Content = this.InternalGrid;
            }

            if (this.ShowGroupDropArea)
            {
                this.LoadGroupDropAreaGrid();
            }

            if (this.InternalGrid != null)
            {
                this.isGridLoaded = true;
                this.HookGrid();
                this.WireGrid(this.InternalGrid);
            }

            ShowHideStatusBar();
        }
        bool isFilterStatusBarCloseButtonClosed = false;

        internal void SetGroupDropAreaGrid()
        {
            var groupDropAreaScrollViewer = this.GetTemplateChild("PART_GroupDropAreaScrollViewer") as ScrollViewer;
            if (groupDropAreaScrollViewer != null)
            {
                groupDropAreaScrollViewer.Content = new GridDataGroupDropAreaGridImpl();
                this.GroupDropAreaGrid = groupDropAreaScrollViewer.Content as GridDataGroupDropAreaGridImpl;//this.GetTemplateChild("Part_GroupDropAreaGrid") as GridDataGroupDropAreaGridImpl;
            }
        }

        void PART_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.StatusBarMessage = "";
            this.Model.View.BeginInit();
            this.Model.TableProperties.SuspendEvents();
            foreach (var item in this.Model.TableProperties.VisibleColumns)
            {
                if (item != null && item.Filters.Count > 0)
                {
                    item.Filters.Clear();
                    isFilterStatusBarCloseButtonClosed = true;                   
                }                
            }
            if (this.isFilterStatusBarCloseButtonClosed && this.Model.View.PagedSource != null && !this.IsViewLevelPaging)
            {
                this.Model.View.PagedSource.Filter = null;
                isFilterStatusBarCloseButtonClosed = false;
            }
            
            this.Model.TableProperties.ResumeEvents();
            this.Model.View.FilterPredicates.Clear();
            this.Model.View.EndInit();
            this.Model.InvalidateDisplay();
        }

        /// <summary>
        /// Gets the grid host. When overriden in a derived class, this can return a derived version of <see cref="GridDataControlBaseImpl"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual GridDataControlBaseImpl GetGridHost()
        {
            var internalGrid = new GridDataControlBaseImpl();
            // specify internal binding to FocusVisualStyle
            var focusVisualStyleBinding = new Binding("FocusVisualStyle") { Source = this };
            internalGrid.SetBinding(GridDataControlBaseImpl.FocusVisualStyleProperty, focusVisualStyleBinding);

            return internalGrid;
        }

        #region SourceType (DependencyProperty)

        /// <summary>
        /// gets / sets the source type for the underlying source.
        /// </summary>
        public Type SourceType
        {
            get { return (Type)GetValue(SourceTypeProperty); }
            set { SetValue(SourceTypeProperty, value); }
        }

        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register("SourceType", typeof(Type), typeof(GridDataControl), new PropertyMetadata(null, OnSourceTypeChanged));

        private static void OnSourceTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.NewValue != null)
            {
                grid.TableProperties.SourceType = (Type)args.NewValue;
            }
        }

        #endregion

        #region ExpressionFunc (DependencyProperty)

        /// <summary>
        /// Gets / Sets the expression functor for customized dynamic objects when grid uses LINQ.
        /// </summary>
        public IUnboundExpressionFunc ExpressionFunc
        {
            get { return (IUnboundExpressionFunc)GetValue(ExpressionFuncProperty); }
            set { SetValue(ExpressionFuncProperty, value); }
        }

        public static readonly DependencyProperty ExpressionFuncProperty = DependencyProperty.Register("ExpressionFunc", typeof(IUnboundExpressionFunc), typeof(GridDataControl), new PropertyMetadata(OnExpressionFuncChanged));

        private static void OnExpressionFuncChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.NewValue != null)
            {
                grid.TableProperties.ExpressionFunc = (IUnboundExpressionFunc)args.NewValue;
            }
            else
            {
                grid.TableProperties.ExpressionFunc = null;
            }
        }

        #endregion

        #region HeaderStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets the header style.
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(null, OnHeaderStyleChanged));

        private bool isHeaderStyleSetBeforeGridLoaded = false;
        private static void OnHeaderStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.HeaderStyle = args.NewValue != null ? (Style)args.NewValue : null;
                grid.Model.TableProperties.HeaderStyle = args.NewValue != null ? (Style)args.NewValue : null;
            }
            else
            {
                grid.isHeaderStyleSetBeforeGridLoaded = true;
            }
        }

        #endregion

        #region ColumnOptionPaneStyle



        public Style ColumnOptionPaneStyle
        {
            get { return (Style)GetValue(ColumnOptionPaneStyleProperty); }
            set { SetValue(ColumnOptionPaneStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnOptionPaneStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnOptionPaneStyleProperty =
            DependencyProperty.Register("ColumnOptionPaneStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(OnColumnOptionPaneStyleChanged));

        private bool isColumnOptionPaneStyleSetBeforeGridLoaded = false;
        private static void OnColumnOptionPaneStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.ColumnOptionPaneStyle = args.NewValue != null ? (Style)args.NewValue : null;
            }
            else
            {
                grid.isColumnOptionPaneStyleSetBeforeGridLoaded = true;
            }
        }

        #endregion

        #region ColumnElementStyle

        /// <summary>
        /// ColumnElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnElementStyleProperty =
            DependencyProperty.Register("ColumnElementStyle", typeof(Style), typeof(GridDataControl),
                new PropertyMetadata((Style)null,
                    new PropertyChangedCallback(OnColumnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ColumnElementStyle property. This dependency property 
        /// indicates style applicable for .
        /// </summary>
        public Style ColumnElementStyle
        {
            get { return (Style)GetValue(ColumnElementStyleProperty); }
            set { SetValue(ColumnElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ColumnElementStyle property.
        /// </summary>
        private static void OnColumnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl target = (GridDataControl)d;
            Style oldColumnElementStyle = (Style)e.OldValue;
            Style newColumnElementStyle = target.ColumnElementStyle;
            target.OnColumnElementStyleChanged(oldColumnElementStyle, newColumnElementStyle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ColumnElementStyle property.
        /// </summary>
        protected virtual void OnColumnElementStyleChanged(Style oldColumnElementStyle, Style newColumnElementStyle)
        {
        }

        #endregion

        #region RowStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets row style.
        /// </summary>
        public Style RowStyle
        {
            get { return (Style)GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register("RowStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(null, OnRowStyleChanged));

        private bool isRowStyleSetBeforeGridLoaded = false;
        private static void OnRowStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.RowStyle = (Style)args.NewValue;
            }
            else
            {
                grid.isRowStyleSetBeforeGridLoaded = true;
            }
        }

        #endregion

        #region AlternateRowStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets the alternate row style.
        /// </summary>
        public Style AlternateRowStyle
        {
            get { return (Style)GetValue(AlternateRowStyleProperty); }
            set { SetValue(AlternateRowStyleProperty, value); }
        }

        public static readonly DependencyProperty AlternateRowStyleProperty = DependencyProperty.Register("AlternateRowStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(null, OnAlternateRowStyleChanged));

        private bool isAlternateRowStyleSetBeforeGridLoaded = false;

        private static void OnAlternateRowStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.AlternateRowStyle = (Style)args.NewValue;
            }
            else
            {
                grid.isAlternateRowStyleSetBeforeGridLoaded = true;
            }
        }

        #endregion

        
        #region ShowErrorIconOnEditing
        /// <summary>
        /// Gets / sets ErrorIcon at Editing time.
        /// </summary>
        public bool ShowErrorIconOnEditing
        {
            get { return (bool)GetValue(ShowErrorIconOnEditingProperty); }
            set { SetValue(ShowErrorIconOnEditingProperty, value); }
        }

        public static readonly DependencyProperty ShowErrorIconOnEditingProperty = DependencyProperty.Register("ShowErrorIconOnEditing", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnShowErrorIconOnEditingChanged));

        private static void OnShowErrorIconOnEditingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.Model.Options.ShowErrorIconOnEditing = (bool)args.NewValue;
        }

        #endregion

        #region EnableVisualStyleForEditors
        /// <summary>
        /// Gets / sets VisualStyle for Editor Cell Types.
        /// </summary>
        public bool EnableVisualStyleForEditors
        {
            get { return (bool)GetValue(EnableVisualStyleForEditorsProperty); }
            set { SetValue(EnableVisualStyleForEditorsProperty, value); }
        }

        public static readonly DependencyProperty EnableVisualStyleForEditorsProperty = DependencyProperty.Register("EnableVisualStyleForEditors", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableVisualStyleForEditorsChanged));

        private bool isEnableVisualStyleForEditorsSetBeforeLoaded = false;

        private static void OnEnableVisualStyleForEditorsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.EnableVisualStyleForEditors = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableVisualStyleForEditorsSetBeforeLoaded = true;
            }
        }

        #endregion

        #region EnableBlendStyling (DependencyProperty)

        /// <summary>
        /// Gets / sets enable blend styling.
        /// </summary>
        public bool EnableBlendStyling
        {
            get { return (bool)GetValue(EnableBlendStylingProperty); }
            set { SetValue(EnableBlendStylingProperty, value); }
        }

        public static readonly DependencyProperty EnableBlendStylingProperty = DependencyProperty.Register("EnableBlendStyling", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableBlendStylingChanged));

        private bool isEnableBlendStyleSetBeforeLoaded = false;

        private static void OnEnableBlendStylingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableBlendStyling = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableBlendStyleSetBeforeLoaded = true;
            }
        }

        #endregion

#if !SILVERLIGHT && SyncfusionFramework4_0
        #region UsePLINQ (DependencyProperty)

        /// <summary>
        /// Gets / sets to use PLINQ
        /// </summary>
        public bool UsePLINQ
        {
            get { return (bool)GetValue(UsePLINQProperty); }
            set { SetValue(UsePLINQProperty, value); }
        }

        public static readonly DependencyProperty UsePLINQProperty = DependencyProperty.Register("UsePLINQ", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnUsePlinqChanged));

        private static void OnUsePlinqChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.UsePLINQ = (bool)args.NewValue;
        }

        #endregion
#endif

        #region PersistGroupExpandStates (DependencyProperty)

        /// <summary>
        /// Gets / sets the persistance for group expand states when refreshed.
        /// </summary>
        public bool PersistGroupsExpandState
        {
            get { return (bool)GetValue(PersistGroupsExpandStateProperty); }
            set { SetValue(PersistGroupsExpandStateProperty, value); }
        }

        public static readonly DependencyProperty PersistGroupsExpandStateProperty = DependencyProperty.Register("PersistGroupExpandStates", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnPersistGroupExpandStateChanged));

        private static void OnPersistGroupExpandStateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.PersistGroupsExpandState = (bool)args.NewValue;
        }

        #endregion

        #region ClearMultiSelectionInNestedGrid (DependencyProperty)

        /// <summary>
        /// Gets / sets the persistance for group expand states when refreshed.
        /// </summary>
        [Obsolete]
        public bool ClearMultiSelectionInNestedGrid
        {
            get { return (bool)GetValue(ClearMultiSelectionInNestedGridProperty); }
            set { SetValue(ClearMultiSelectionInNestedGridProperty, value); }
        }

        public static readonly DependencyProperty ClearMultiSelectionInNestedGridProperty = DependencyProperty.Register("ClearMultiSelectionInNestedGrid", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnClearMultiSelectionInNestedGridChanged));

        private static void OnClearMultiSelectionInNestedGridChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            //var grid = dpo as GridDataControl;
            //if (grid != null) 
            //    grid.TableProperties.ClearMultiSelectionInNestedGrid = (bool)args.NewValue;
        }
       
        #endregion
        #region Seleced child model
        /// <summary>
        /// Gets / sets the selected child model for nestedGrid.
        /// </summary>
        public static readonly DependencyProperty SelectedChildModelProperty = DependencyProperty.Register(
          "SelectedChildModel", typeof(GridDataChildTableModel), typeof(GridDataControl), new FrameworkPropertyMetadata(null, SelectedChildModelChanged));

        /// <summary>
        /// Gets or sets the selected child model.
        /// </summary>
        /// <value>The selected child model.</value>
        public GridDataChildTableModel SelectedChildModel
        {
            get { return (GridDataChildTableModel) GetValue(SelectedChildModelProperty); }
            internal set { SetValue(SelectedChildModelProperty, value); }
        }

        private static void SelectedChildModelChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.OldValue != null)
            {
                var oldChildGridModel = args.OldValue as GridDataChildTableModel;
                if (oldChildGridModel != null && oldChildGridModel.Grid != null)
                {
                    var nestedGrid = oldChildGridModel.Grid as GridDataCellNestedGridEditor;
                    if (nestedGrid != null)
                    {
                        var cellRowColIndex = VirtualizingCellsControl.GetCellRowColumnIndex(nestedGrid);
                        if (!cellRowColIndex.IsEmpty)
                        {
                            grid.Model.InvalidateCell(cellRowColIndex);
                            grid.Model.InvalidateVisual();
                        }
                        nestedGrid.ClearChildGridSelections(oldChildGridModel);
                    }
                }
            }
            else if (args.NewValue != null)
            {
                if (grid.SelectedItems != null)
                    grid.SelectedItems.Clear();
                if (grid.SelectedItem != null)
                    grid.SelectedItem = null;
            }
        }
        #endregion
#if !SILVERLIGHT

        #region ShowHoveringBackground (DependencyProperty)
        /// <summary>
        /// Gets or sets a value indicating whether [show hovering background].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show hovering background]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHoveringBackground
        {
            get { return (bool)GetValue(ShowHoveringBackgroundProperty); }
            set { SetValue(ShowHoveringBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ShowHoveringBackgroundProperty = DependencyProperty.Register("ShowHoveringBackground", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnShowHoveringBackgroundChanged));

        private static void OnShowHoveringBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ShowHoveringBackground = (bool)args.NewValue;
        }

        #endregion
#endif

        #region Show Sort Number (DependencyProperty)

        /// <summary>
        /// Gets or sets a value indicating whether [show sort number].
        /// </summary>
        /// <value><c>true</c> if [show sort number]; otherwise, <c>false</c>.</value>
        public bool ShowSortNumber
        {
            get { return (bool)GetValue(ShowSortNumberProperty); }
            set { SetValue(ShowSortNumberProperty, value); }
        }

        public static readonly DependencyProperty ShowSortNumberProperty = DependencyProperty.Register("ShowSortNumber", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnShowSortNumberChanged));

        private static void OnShowSortNumberChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ShowSortNumber = (bool)args.NewValue;
        }

        #endregion

        #region RetainSortWhenUnGrouped (Dependency Property)

        public bool RetainSortWhenUnGrouped
        {
            get { return (bool)GetValue(RetainSortWhenUnGroupedProperty); }
            set { SetValue(RetainSortWhenUnGroupedProperty, value); }
        }

        public static readonly DependencyProperty RetainSortWhenUnGroupedProperty = DependencyProperty.Register("RetainSortWhenUnGrouped", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnRetainSortWhenUnGroupedPropertyChanged));

        private static void OnRetainSortWhenUnGroupedPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.RetainSortWhenUnGrouped = (bool)args.NewValue;
        }

        #endregion

        #region
        public bool EnableTriStateSorting
        {
            get { return (bool)GetValue(EnableTriStateSortingProperty); }
            set { SetValue(EnableTriStateSortingProperty, value); }
        }

        public static readonly DependencyProperty EnableTriStateSortingProperty = DependencyProperty.Register("EnableTriStateSorting", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableTriStateSortingPropertyChanged));

        private static void OnEnableTriStateSortingPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.EnableTriStateSorting = (bool)args.NewValue;
        }
        #endregion

        #region FilterBarPredicateType (Dependency Property)

        public PredicateType FilterBarPredicateType
        {
            get { return (PredicateType)GetValue(FilterBarPredicateTypeProperty); }
            set { SetValue(FilterBarPredicateTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterBarPredicateTypeProperty = DependencyProperty.Register("FilterBarPredicateType", typeof(PredicateType), typeof(GridDataControl), new PropertyMetadata(PredicateType.And, OnFilterBarPredicateTypePropertyChanged));

        private static void OnFilterBarPredicateTypePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FilterBarPredicateType = (PredicateType)args.NewValue;
        }

        #endregion

        #region DefaultFilterOperator (Dependency Property)

        /// <summary>
        /// Gets or sets DefaultFilterOperator value for Filter bar.
        /// </summary>
        /// <value></value>
        /// <remarks>DefaultFilterOperator contains StartsWith, Contains and Equals property in Enum value. This is set for Default filter value where we can use the filter value with out wildcard.</remarks>
        public FilterOperatorType DefaultFilterOperator
            {
            get { return (FilterOperatorType)GetValue(DefaultFilterOperatorProperty); }
            set { SetValue(DefaultFilterOperatorProperty, value); }
            }

        public static readonly DependencyProperty DefaultFilterOperatorProperty = DependencyProperty.Register("DefaultFilterOperator", typeof(FilterOperatorType), typeof(GridDataControl), new PropertyMetadata(FilterOperatorType.StartsWith, OnDefaultFilterOperatorPropertyChanged));

        private static void OnDefaultFilterOperatorPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
            {
            var grid = dpo as GridDataControl;
            grid.TableProperties.DefaultFilterOperator = (FilterOperatorType)args.NewValue;
            }

        #endregion

        #region CustomGroupComparer (DependencyProperty)

        /// <summary>
        /// Defines the custom group comparer. This will work only when the grouped column is also sorted.
        /// </summary>
        public IComparer<Group> CustomGroupComparer
        {
            get { return (IComparer<Group>)GetValue(CustomGroupComparerProperty); }
            set { SetValue(CustomGroupComparerProperty, value); }
        }

        public static readonly DependencyProperty CustomGroupComparerProperty = DependencyProperty.Register("CustomGroupComparer", typeof(IComparer<Group>), typeof(GridDataControl), new PropertyMetadata(OnCustomGroupComparerPropertyChanged));

        private static void OnCustomGroupComparerPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.CustomGroupComparer = (IComparer<Group>)args.NewValue;
        }

        #endregion

        private static void OnAutoPopulateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoPopulateColumns = (bool)args.NewValue;                 
        }

        private static void OnAutoPopulateRelationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoPopulateRelations = (bool)args.NewValue;
        }

        private static void OnDefaultColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.DefaultColumnWidth = (double)args.NewValue;
        }

        private bool IsDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(this);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;

            if (grid.IsDesignTime())
            {
                if (!(args.NewValue is System.Data.DataTable || args.NewValue is System.Data.DataView))
                {
                    if (args.NewValue == null)
                    {
                        grid.isItemsSourceLoadedBeforeGridLoaded = true;
                        return;
                    }
                    else if (!(args.NewValue is INotifyCollectionChanged))
                    {
                        grid.isItemsSourceLoadedBeforeGridLoaded = true;
                        return;
                    }
                }
            }

            if (grid.isGridLoaded)
            {
                if (grid.TableProperties != null)
                {
                    grid.TableProperties.ItemsSource = args.NewValue;
                    grid.OnItemsSourceChanged(new SyncfusionRoutedEventArgs(GridDataControl.ItemsSourceChangedEvent, grid));
                }
            }
            else
            {
                grid.isItemsSourceLoadedBeforeGridLoaded = true;
            }

        }

        private static void OnNullFilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.NullFilterText = args.NewValue.ToString();
        }

        private static void OnRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.RowBackground = (Brush)args.NewValue;
        }

        private bool isShowAddNewRowChangedBeforeGridLoaded = false;

        private static void OnShowAddNewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowAddNewRow = (bool)args.NewValue;
            }
            else
            {
                grid.isShowAddNewRowChangedBeforeGridLoaded = true;
            }
        }

        private static void OnFilterBarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.FilterBarMode = (GridDataFilterBarMode)args.NewValue;
        }

        private static void OnShowFilterBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.ShowFilterBar = (bool)args.NewValue;
        }

        private static void OnAlphaNumericFilterTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlphaNumericFilterType = (AlphaNumericFilterType)args.NewValue;
        }

        private static void OnFilterPanePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl gri = d as GridDataControl;
            gri.TableProperties.FilterPanePosition = (FilterPanePosition)args.NewValue;
        }

        private bool isShowRecordPlusMinusChangedBeforeGridLoaded = false;

        private static void OnShowRecordPlusMinusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowRecordPlusMinus = (bool)args.NewValue;
            }
            else
            {
                grid.isShowRecordPlusMinusChangedBeforeGridLoaded = true;
            }
        }

        private bool isSelectFirstRowOnLoadChangedBeforeGridLoaded = false;

        private static void OnSelectFirstRowOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.IsLoaded)
            {
                grid.TableProperties.SelectFirstRowOnLoad = (bool)args.NewValue;
            }
            else
            {
                grid.isSelectFirstRowOnLoadChangedBeforeGridLoaded = true;
            }
        }

        private bool isShowRowHeaderChangedBeforeGridLoaded = false;

        private static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowRowHeader = (bool)args.NewValue;
            }
            else
            {
                grid.isShowRowHeaderChangedBeforeGridLoaded = true;
            }
        }

        internal bool hasVisualStyleChanged = false;
        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.hasVisualStyleChanged = args.NewValue != null;
            var visualStyle = SkinStorage.GetVisualStyle(d);
            var value = GridDataControl.ResolveVisualStyleToSkinStorage(((VisualStyle)args.NewValue));
            if (visualStyle != value)
            {
                // we just ensure that the DO has the same visual style with SkinStorage                
                if (value == VisualStyle.Default.ToString() && visualStyle == value)
                {
                    SkinStorage.SetVisualStyle(d, value);
                }
            }

            if (grid.isGridLoaded)
            {
                if (grid.TableProperties != null)
                {
                    grid.TableProperties.CustomVisualStyle = grid.CustomVisualStyle;
                    grid.TableProperties.VisualStyle = (VisualStyle)args.NewValue;
                }
            }
            else
            {
                grid.isVisualStyleChangedBeforeGridLoaded = true;
            }
#if !SILVERLIGHT
            if (grid.ColumnChooserGrid != null)
            {
                grid.ColumnChooserGrid.Grid.InvalidateCell(GridRangeInfo.Table());
                grid.ColumnChooserGrid.Grid.InvalidateVisual();
            }
#endif
        }

        internal static VisualStyle ResolveSkinStorageToVisualStyle(string skinStyle)
        {
            switch (skinStyle)
            {
                case "Office2007Blue":
                    return VisualStyle.Office2007Blue;
                case "Office2007Silver":
                    return VisualStyle.Office2007Silver;
                case "Office2007Black":
                    return VisualStyle.Office2007Black;
                case "Blend":
                    return VisualStyle.Blend;
                case "Office2003":
                    return VisualStyle.Office2003;
                default:
                    return VisualStyle.Default;
            }
        }

        internal static string ResolveVisualStyleToSkinStorage(VisualStyle visualStyle)
        {
            var result = string.Empty;
            switch (visualStyle)
            {
                case VisualStyle.Office14Blue:
                    result = "Office2010Blue";
                    break;
                case VisualStyle.Office2007Blue:
                case VisualStyle.DefaultOffice2007Blue:
                case VisualStyle.BureauBlue:
                case VisualStyle.TwilightBlue:
                    result = "Office2007Blue";
                    break;
                case VisualStyle.ShinyBlue:
                    result = "ShinyBlue";
                    break;
                case VisualStyle.DefaultOffice2007Silver:
                case VisualStyle.Office2007Silver:
                    result = "Office2007Silver";
                    break;
                case VisualStyle.Office14Silver:
                    result = "Office2010Silver";
                    break;
                case VisualStyle.DefaultOffice2007Black:
                case VisualStyle.Office2007Black:
                    result = "Office2007Black";
                    break;
                case VisualStyle.SunBlack:
                case VisualStyle.ShinyRed:
                    result = "ShinyRed";
                    break;
                case VisualStyle.Office14Black:
                    result = "Office2010Black";
                    break;
                case VisualStyle.GlassyGreen:
                    result = "Office2007Black";
                    break;
                case VisualStyle.Blend:
                case VisualStyle.BureauBlack:
                    result = "Blend";
                    break;
                case VisualStyle.Office2003:
                    result = "Office2003";
                    break;
                case VisualStyle.Default:
                    result = "Default";
                    break;
                case VisualStyle.Metro:  //newly added VisualStyles
                    result = "Metro";
                    break;
                case VisualStyle.VS2010:
                    result = "VS2010";  //newly added VisualStyles
                    break;
            }
            return result;
        }

        #region  Dependency Property for Model.Options

        /// <summary>
        /// DependencyProperty for <see cref="Syncfusion.Windows.Controls.Grid.GridDataControl.ActivateCurrentCellBehavior"/>.
        /// </summary>
        public static readonly DependencyProperty ActivateCurrentCellBehaviorProperty = DependencyProperty.Register("ActivateCurrentCellBehavior", typeof(GridCellActivateAction), typeof(GridDataControl), new FrameworkPropertyMetadata(GridCellActivateAction.DblClickOnCell, OnActivateCurrentCellBehaviorChanged));

        /// <summary>
        /// Gets or sets the ActivateCurrentCellBehavior property. Specifies current cell activation behavior 
        /// when moving the current cell or clicking inside a cell. Defines when to set the focus / toggle 
        /// edit mode for the current cell.
        /// </summary>
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return (GridCellActivateAction)GetValue(ActivateCurrentCellBehaviorProperty);
            }

            set
            {
                SetValue(ActivateCurrentCellBehaviorProperty, value);
            }
        }

        private static void OnActivateCurrentCellBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ActivateCurrentCellBehavior = (GridCellActivateAction)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="Syncfusion.Windows.Controls.Grid.GridDataControl.AllowSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectionProperty = DependencyProperty.Register("AllowSelection", typeof(GridSelectionFlags), typeof(GridDataControl), new FrameworkPropertyMetadata(GridSelectionFlags.Any, OnAllowSelectionChanged));

        /// <summary>
        /// Gets or sets the allow selection. Specifies behavior for selecting cells for the grid by the user with mouse or keyboard.
        /// </summary>
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return (GridSelectionFlags)GetValue(AllowSelectionProperty);
            }

            set
            {
                SetValue(AllowSelectionProperty, value);
            }
        }

        private static void OnAllowSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.AllowSelection = (GridSelectionFlags)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="Syncfusion.Windows.Controls.Grid.GridDataControl.DrawSelectionOptions"/> property.
        /// </summary>
        public static readonly DependencyProperty DrawSelectionOptionsProperty = DependencyProperty.Register("DrawSelectionOptions", typeof(GridDrawSelectionOptions), typeof(GridDataControl), new FrameworkPropertyMetadata(OnDrawSelectionOptionsChanged));

        /// <summary>
        /// Gets or sets the draw selection options. Specify the different possible drawing selected cell options.
        /// </summary>
        /// <value>The draw selection options.</value>
        public GridDrawSelectionOptions DrawSelectionOptions
        {
            get
            {
                return (GridDrawSelectionOptions)GetValue(DrawSelectionOptionsProperty);
            }

            set
            {
                SetValue(DrawSelectionOptionsProperty, value);
            }
        }

        private static void OnDrawSelectionOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.DrawSelectionOptions = (GridDrawSelectionOptions)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ExcelLikeCurrentCell"/> property.
        /// </summary>
        public static readonly DependencyProperty ExcelLikeCurrentCellProperty = DependencyProperty.Register("ExcelLikeCurrentCell", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(true, OnExcelLikeCurrentCellChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ExcelLikeCurrentCell is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [excel like current cell]; otherwise, <c>false</c>.
        /// </value>
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return (bool)GetValue(ExcelLikeCurrentCellProperty);
            }

            set
            {
                SetValue(ExcelLikeCurrentCellProperty, value);
            }
        }

        private static void OnExcelLikeCurrentCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ExcelLikeCurrentCell = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ExcelLikeSelectionFrame"/> property.
        /// </summary>
        public static readonly DependencyProperty ExcelLikeSelectionFrameProperty = DependencyProperty.Register("ExcelLikeSelectionFrame", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(false, OnExcelLikeSelectionFrameChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ExcelLikeSelectionFrame is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [excel like selection frame]; otherwise, <c>false</c>.
        /// </value>
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return (bool)GetValue(ExcelLikeSelectionFrameProperty);
            }

            set
            {
                SetValue(ExcelLikeSelectionFrameProperty, value);
            }
        }

        private static void OnExcelLikeSelectionFrameChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ExcelLikeSelectionFrame = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.FormulaDisplayBehavior"/> property.
        /// </summary>
        public static readonly DependencyProperty FormulaDisplayBehaviorProperty = DependencyProperty.Register("FormulaDisplayBehavior", typeof(GridShowFormulaBehavior), typeof(GridDataControl), new FrameworkPropertyMetadata(OnFormulaDisplayBehaviorChanged));

        /// <summary>
        /// Gets or sets the formula display behavior.
        /// </summary>
        /// <value>The formula display behavior.</value>
        public GridShowFormulaBehavior FormulaDisplayBehavior
        {
            get
            {
                return (GridShowFormulaBehavior)GetValue(FormulaDisplayBehaviorProperty);
            }

            set
            {
                SetValue(FormulaDisplayBehaviorProperty, value);
            }
        }

        private static void OnFormulaDisplayBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.FormulaDisplayBehavior = (GridShowFormulaBehavior)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.HighlightSelectionAlphaBlend"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionAlphaBlendProperty = DependencyProperty.Register("HighlightSelectionAlphaBlend", typeof(Brush), typeof(GridDataControl), new FrameworkPropertyMetadata(OnHighlightSelectionAlphaBlendChanged));

        /// <summary>
        /// Gets or sets the highlight selection alpha blend.
        /// </summary>
        /// <value>The highlight selection alpha blend.</value>
        public Brush HighlightSelectionAlphaBlend
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionAlphaBlendProperty);
            }

            set
            {
                SetValue(HighlightSelectionAlphaBlendProperty, value);
            }
        }

        private static void OnHighlightSelectionAlphaBlendChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionAlphaBlend = (Brush)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.HighlightSelectionBackground"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionBackgroundProperty = DependencyProperty.Register("HighlightSelectionBackground", typeof(Brush), typeof(GridDataControl), new FrameworkPropertyMetadata(OnHighlightSelectionBackgroundChanged));

        /// <summary>
        /// Gets or sets the highlight selection background.
        /// </summary>
        /// <value>The highlight selection background.</value>
        public Brush HighlightSelectionBackground
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionBackgroundProperty);
            }

            set
            {
                SetValue(HighlightSelectionBackgroundProperty, value);
            }
        }

        private static void OnHighlightSelectionBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBackground = (Brush)args.NewValue;
            gridDataControl.Model.Options.highlightSelectionBackgroundChanged = true;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.HighlightSelectionBorder"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionBorderProperty = DependencyProperty.Register("HighlightSelectionBorder", typeof(Brush), typeof(GridDataControl), new FrameworkPropertyMetadata(OnHighlightSelectionBorderChanged));

        /// <summary>
        /// Gets or sets the highlight selection border.
        /// </summary>
        /// <value>The highlight selection border.</value>
        public Brush HighlightSelectionBorder
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionBorderProperty);
            }

            set
            {
                SetValue(HighlightSelectionBorderProperty, value);
            }
        }

        private static void OnHighlightSelectionBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBorder = (Brush)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.HighlightSelectionForeground"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionForegroundProperty = DependencyProperty.Register("HighlightSelectionForeground", typeof(Brush), typeof(GridDataControl), new FrameworkPropertyMetadata(OnHighlightSelectionForegroundChanged));

        /// <summary>
        /// Gets or sets the highlight selection foreground.
        /// </summary>
        /// <value>The highlight selection foreground.</value>
        public Brush HighlightSelectionForeground
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionForegroundProperty);
            }

            set
            {
                SetValue(HighlightSelectionForegroundProperty, value);
            }
        }

        private static void OnHighlightSelectionForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionForeground = (Brush)args.NewValue;
            gridDataControl.Model.Options.highlightSelectionForegroundChanged = true;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.HighlightSelectionBorderWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionBorderWidthProperty = DependencyProperty.Register("HighlightSelectionBorderWidth", typeof(double), typeof(GridDataControl), new FrameworkPropertyMetadata(OnHighlightSelectionBorderWidthChanged));

        /// <summary>
        /// Gets or sets the width of the highlight selection border.
        /// </summary>
        /// <value>The width of the highlight selection border.</value>
        public double HighlightSelectionBorderWidth
        {
            get
            {
                return (double)GetValue(HighlightSelectionBorderWidthProperty);
            }

            set
            {
                SetValue(HighlightSelectionBorderWidthProperty, value);
            }
        }

        private static void OnHighlightSelectionBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBorderWidth = (double)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ListBoxModeAllowUIElementClick"/> property.
        /// </summary>
        public static readonly DependencyProperty ListBoxModeAllowUIElementClickProperty = DependencyProperty.Register("ListBoxModeAllowUIElementClick", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(true, OnListBoxModeAllowUIElementClickChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ListBoxModeAllowUIElementClick is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [list box mode allow UI element click]; otherwise, <c>false</c>.
        /// </value>
        public bool ListBoxModeAllowUIElementClick
        {
            get
            {
                return (bool)GetValue(ListBoxModeAllowUIElementClickProperty);
            }

            set
            {
                SetValue(ListBoxModeAllowUIElementClickProperty, value);
            }
        }

        private static void OnListBoxModeAllowUIElementClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ListBoxModeAllowUIElementClick = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ListBoxSelectionMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ListBoxSelectionModeProperty = DependencyProperty.Register("ListBoxSelectionMode", typeof(GridSelectionMode), typeof(GridDataControl), new FrameworkPropertyMetadata(GridSelectionMode.One, OnListBoxSelectionModeChanged));

        /// <summary>
        /// Gets or sets the list box selection mode.
        /// </summary>
        /// <value>The list box selection mode.</value>
        public GridSelectionMode ListBoxSelectionMode
        {
            get
            {
                return (GridSelectionMode)GetValue(ListBoxSelectionModeProperty);
            }

            set
            {
                SetValue(ListBoxSelectionModeProperty, value);
            }
        }

        private static void OnListBoxSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ListBoxSelectionMode = (GridSelectionMode)args.NewValue;
            if (gridDataControl.Model.SelectedRanges != null && gridDataControl.Model.SelectedRanges.Count != 0)
            {
                gridDataControl.Model.Selections.Clear();
            }
        }

        #region AllowExcelLikeResizing

        /// <summary>
        /// AllowExcelLikeResizing Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowExcelLikeResizingProperty =
            DependencyProperty.Register("AllowExcelLikeResizing", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnAllowExcelLikeResizingChanged)));

        /// <summary>
        /// Gets or sets the AllowExcelLikeResizing property. This dependency property 
        /// indicates whether hidden column/row resizing is allowed like in Excel.
        /// </summary>
        public bool AllowExcelLikeResizing
        {
            get { return (bool)GetValue(AllowExcelLikeResizingProperty); }
            set { SetValue(AllowExcelLikeResizingProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AllowExcelLikeResizing property.
        /// </summary>
        private static void OnAllowExcelLikeResizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.AllowExcelLikeResizing = (bool)e.NewValue;
        }

        #endregion



        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ScrollFrozen"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollFrozenProperty = DependencyProperty.Register("ScrollFrozen", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(OnScrollFrozenChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ScrollFrozen is true / false.
        /// </summary>
        /// <value><c>true</c> if [scroll frozen]; otherwise, <c>false</c>.</value>
        public bool ScrollFrozen
        {
            get
            {
                return (bool)GetValue(ScrollFrozenProperty);
            }

            set
            {
                SetValue(ScrollFrozenProperty, value);
            }
        }

        private static void OnScrollFrozenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ScrollFrozen = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowCurrentCell"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowCurrentCellProperty = DependencyProperty.Register("ShowCurrentCell", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(true, OnShowCurrentCellChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ShowCurrentCell is true / false.
        /// </summary>
        /// <value><c>true</c> if [show current cell]; otherwise, <c>false</c>.</value>
        public bool ShowCurrentCell
        {
            get
            {
                return (bool)GetValue(ShowCurrentCellProperty);
            }

            set
            {
                SetValue(ShowCurrentCellProperty, value);
            }
        }

        private static void OnShowCurrentCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ShowCurrentCell = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.WrapCell"/> property.
        /// </summary>
        public static readonly DependencyProperty WrapCellProperty = DependencyProperty.Register("WrapCell", typeof(bool), typeof(GridDataControl), new FrameworkPropertyMetadata(true, OnWrapCellChanged));

        /// <summary>
        /// Gets or sets a value indicating whether WrapCell is true / false.
        /// </summary>
        /// <value><c>true</c> if [wrap cell]; otherwise, <c>false</c>.</value>
        public bool WrapCell
        {
            get
            {
                return (bool)GetValue(WrapCellProperty);
            }

            set
            {
                SetValue(WrapCellProperty, value);
            }
        }

        private static void OnWrapCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.WrapCell = (bool)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.WrapCellBehavior"/> property.
        /// </summary>
        public static readonly DependencyProperty WrapCellBehaviorProperty = DependencyProperty.Register("WrapCellBehavior", typeof(GridWrapCellBehavior), typeof(GridDataControl), new FrameworkPropertyMetadata(OnWrapCellBehaviorBehaviorChanged));

        public GridWrapCellBehavior WrapCellBehavior
        {
            get
            {
                return (GridWrapCellBehavior)GetValue(WrapCellBehaviorProperty);
            }

            set
            {
                SetValue(WrapCellBehaviorProperty, value);
            }
        }

        private static void OnWrapCellBehaviorBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.WrapCellBehavior = (GridWrapCellBehavior)args.NewValue;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.CopyPasteOption"/> property.
        /// </summary>
        public static readonly DependencyProperty CopyPasteOptionProperty = DependencyProperty.Register("CopyPasteOption", typeof(CopyPaste), typeof(GridDataControl), new FrameworkPropertyMetadata(OnCopyPasteOptionBehaviorChanged));

        /// <summary>
        /// Gets or sets the copy paste option.
        /// </summary>
        /// <value>The copy paste option.</value>
        public CopyPaste CopyPasteOption
        {
            get
            {
                return (CopyPaste)GetValue(CopyPasteOptionProperty);
            }

            set
            {
                SetValue(CopyPasteOptionProperty, value);
            }
        }

        private static void OnCopyPasteOptionBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.CopyPasteOption = (CopyPaste)args.NewValue;
        }

        #endregion

        #region GridDataControl Events

        private void WireGrid(GridControlBase grid)
        {
            grid.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(grid_CurrentCellActivating);
            grid.CurrentCellActivated += new GridRoutedEventHandler(grid_CurrentCellActivated);
            grid.CurrentCellActivateFailed += new GridCurrentCellActivateFailedEventHandler(grid_CurrentCellActivateFailed);
            grid.CurrentCellDeactivating += new GridCancelRoutedEventHandler(grid_CurrentCellDeactivating);
            grid.CurrentCellDeactivated += new GridCurrentCellDeactivatedEventHandler(grid_CurrentCellDeactivated);
            grid.CurrentCellDeactivateFailed += new GridRoutedEventHandler(grid_CurrentCellDeactivateFailed);
            grid.CurrentCellConfirmChangesFailed += new GridRoutedEventHandler(grid_CurrentCellConfirmChangesFailed);
            grid.CurrentCellAcceptedChanges += new GridRoutedEventHandler(grid_CurrentCellAcceptedChanges);
            grid.CurrentCellChanging += new GridCancelRoutedEventHandler(grid_CurrentCellChanging);
            grid.CurrentCellStartEditing += new GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellLoaded +=new GridControlBase.GridCurrentCellLoadedEvent(grid_CurrentCellLoaded);
            grid.CurrentCellEditingComplete += new GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellRejectedChanges += new GridRoutedEventHandler(grid_CurrentCellRejectedChanges);
            grid.CurrentCellChanged += new GridRoutedEventHandler(grid_CurrentCellChanged);
            grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(grid_CurrentCellMoved);
            grid.CurrentCellMoveFailed += new GridCurrentCellMoveFailedEventHandler(grid_CurrentCellMoveFailed);
            grid.CurrentCellMoving += new GridCurrentCellMovingEventHandler(grid_CurrentCellMoving);
            grid.CurrentCellValidating += new CurrentCellValidatingEventHandler(grid_CurrentCellValidating);
            grid.CurrentCellValidated += new GridRoutedEventHandler(grid_CurrentCellValidated);
            grid.CellButtonClick += new GridCellButtonClickEventHandler(grid_CellButtonClick);
            grid.DropDownSelectionChanged += new GridCellComboValueChangedEventHandler(grid_DropDownSelectionChanged);
            grid.CellClick += new GridCellClickEventHandler(grid_CellClick);
            grid.CellCursor += new GridCellCursorEventHandler(grid_CellCursor);
            grid.CellMouseHoverEnter += new GridCellMouseEventHandler(grid_CellMouseHoverEnter);
            grid.CellMouseHover += new GridCellMouseControllerEventHandler(grid_CellMouseHover);
            grid.CellMouseHoverLeave += new GridCellMouseEventHandler(grid_CellMouseHoverLeave);
            grid.CellMouseDown += new GridCellMouseControllerEventHandler(grid_CellMouseDown);
            grid.CellMouseMove += new GridCellMouseControllerEventHandler(grid_CellMouseMove);
            grid.CellMouseUp += new GridCellMouseControllerEventHandler(grid_CellMouseUp);
            grid.CellCancelMode += new GridRoutedEventHandler(grid_CellCancelMode);
            grid.CellRestoreMode += new GridRoutedEventHandler(grid_CellRestoreMode);
            grid.ResizingColumns += new GridResizingColumnsEventHandler(grid_ResizingColumns);
            grid.QueryAllowDragColumn += new GridQueryDragColumnHeaderEventHandler(grid_QueryAllowDragColumn);
            grid.ResizingRows += new GridResizingRowsEventHandler(grid_ResizingRows);
            grid.CurrentCellPreviewKeyDown += new GridCellKeyEventHandler(grid_CellPreviewKeyDown);
            grid.CurrentCellKeyDown += new GridCellKeyEventHandler(grid_CellKeyDown);
            this.Model.Table.RecordsSelectionChanged += new GridDataRecordsSelectionChangedEventHandler(Table_RecordsSelectionChanged);
            this.Model.Table.RecordSelectionChanging += new GridDataRecordSelectionChangingEventHandler(Table_RecordSelectionChanging);
#if WPF
            grid.SizeChanged += grid_SizeChanged;
#endif
        }

        private void UnwireGrid(GridControlBase grid)
        {
#if WPF
            grid.SizeChanged -= grid_SizeChanged;
#endif
            this.Model.TableStyle.Borders.Changed -= new Styles.StyleChangedEventHandler(OnTableBordersChanged);
            grid.UnWireModelWhieDispose();
            this.SelectedItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
            this.Model.SelectionChanging -= new GridSelectionChangingEventHandler(Model_SelectionChanging); 
            this.Model.SelectionChanged -= new GridSelectionChangedEventHandler(Model_SelectionChanged);
            this.Loaded -= new RoutedEventHandler(GridDataControl_Loaded);
            this.GotFocus -= new RoutedEventHandler(GridDataControl_GotFocus);
            grid.CurrentCellActivating -= new GridCurrentCellActivatingEventHandler(grid_CurrentCellActivating);
            grid.CurrentCellActivated -= new GridRoutedEventHandler(grid_CurrentCellActivated);
            grid.CurrentCellActivateFailed -= new GridCurrentCellActivateFailedEventHandler(grid_CurrentCellActivateFailed);
            grid.CurrentCellDeactivating -= new GridCancelRoutedEventHandler(grid_CurrentCellDeactivating);
            grid.CurrentCellDeactivated -= new GridCurrentCellDeactivatedEventHandler(grid_CurrentCellDeactivated);
            grid.CurrentCellDeactivateFailed -= new GridRoutedEventHandler(grid_CurrentCellDeactivateFailed);
            grid.CurrentCellConfirmChangesFailed -= new GridRoutedEventHandler(grid_CurrentCellConfirmChangesFailed);
            grid.CurrentCellAcceptedChanges -= new GridRoutedEventHandler(grid_CurrentCellAcceptedChanges);
            grid.CurrentCellChanging -= new GridCancelRoutedEventHandler(grid_CurrentCellChanging);
            grid.CurrentCellStartEditing -= new GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellLoaded -= new GridControlBase.GridCurrentCellLoadedEvent(grid_CurrentCellLoaded);
            grid.CurrentCellEditingComplete -= new GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellRejectedChanges -= new GridRoutedEventHandler(grid_CurrentCellRejectedChanges);
            grid.CurrentCellChanged -= new GridRoutedEventHandler(grid_CurrentCellChanged);
            grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(grid_CurrentCellMoved);
            grid.CurrentCellMoveFailed -= new GridCurrentCellMoveFailedEventHandler(grid_CurrentCellMoveFailed);
            grid.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(grid_CurrentCellMoving);
            grid.CurrentCellValidating -= new CurrentCellValidatingEventHandler(grid_CurrentCellValidating);
            grid.CurrentCellValidated -= new GridRoutedEventHandler(grid_CurrentCellValidated);
            grid.CellButtonClick -= new GridCellButtonClickEventHandler(grid_CellButtonClick);
            grid.DropDownSelectionChanged -= new GridCellComboValueChangedEventHandler(grid_DropDownSelectionChanged);
            grid.CellClick -= new GridCellClickEventHandler(grid_CellClick);
            grid.CellCursor -= new GridCellCursorEventHandler(grid_CellCursor);
            grid.CellMouseHoverEnter -= new GridCellMouseEventHandler(grid_CellMouseHoverEnter);
            grid.CellMouseHover -= new GridCellMouseControllerEventHandler(grid_CellMouseHover);
            grid.CellMouseHoverLeave -= new GridCellMouseEventHandler(grid_CellMouseHoverLeave);
            grid.CellMouseDown -= new GridCellMouseControllerEventHandler(grid_CellMouseDown);
            grid.CellMouseMove -= new GridCellMouseControllerEventHandler(grid_CellMouseMove);
            grid.CellMouseUp -= new GridCellMouseControllerEventHandler(grid_CellMouseUp);
            grid.CellCancelMode -= new GridRoutedEventHandler(grid_CellCancelMode);
            grid.CellRestoreMode -= new GridRoutedEventHandler(grid_CellRestoreMode);
            grid.ResizingColumns -= new GridResizingColumnsEventHandler(grid_ResizingColumns);
            grid.QueryAllowDragColumn -= new GridQueryDragColumnHeaderEventHandler(grid_QueryAllowDragColumn);
            grid.ResizingRows -= new GridResizingRowsEventHandler(grid_ResizingRows);
            grid.CurrentCellPreviewKeyDown -= new GridCellKeyEventHandler(grid_CellPreviewKeyDown);
            grid.CurrentCellKeyDown -= new GridCellKeyEventHandler(grid_CellKeyDown);
            if (this.Model.Table != null)
            {
                this.Model.Table.RecordSelectionChanging -= new GridDataRecordSelectionChangingEventHandler(Table_RecordSelectionChanging);
                this.Model.Table.RecordsSelectionChanged -= new GridDataRecordsSelectionChangedEventHandler(Table_RecordsSelectionChanged);
            }            
            if (chooser != null)
                ((INotifyCollectionChanged)chooser.newColumnList).CollectionChanged -= new NotifyCollectionChangedEventHandler(GridDataControl_CollectionChanged);
            ColumnChooserColumns.Clear();
        }

#if WPF
        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsLoaded || !GridControlBase.GetDelayLoad(this))
                return;

            var candelayload = false;
            if (e.HeightChanged)
            {
                if (e.PreviousSize.Height < e.NewSize.Height / 2)
                    candelayload = true;
            }
            if (!candelayload && e.WidthChanged)
            {
                if (e.PreviousSize.Width < e.NewSize.Width / 2)
                    candelayload = true;
            }

            if (candelayload)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.Model != null)
                        this.Model.InvalidateCell(GridRangeInfo.Table());
                }), DispatcherPriority.ApplicationIdle);
            }
        }
#endif

        #region CurrentCell events

        #region CurrentCellActivating
        void grid_CurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs args)
        {
            this.OnCurrentCellActivating(args);
        }

        public static readonly RoutedEvent CurrentCellActivatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivating",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellActivatingEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Occurs before the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can disallow the activation of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.CellRowColumnIndex"/>
        /// to activate a different cell.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// Once the current cell has been activated, a <see cref="GridDataControl.CurrentCellActivated"/> event
        /// is raised or a <see cref="GridDataControl.CurrentCellActivateFailed"/> if activating the specified
        /// cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [Description("Occurs before the grid activates the specified cell as current cell."), Category("Behavior")]
        public event GridCurrentCellActivatingEventHandler CurrentCellActivating
        {
            add
            {
                AddHandler(GridDataControl.CurrentCellActivatingEvent, value, false);
            }
            remove
            {
                RemoveHandler(GridDataControl.CurrentCellActivatingEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellActivating"/> event.
        /// </summary>
        /// <param name="args">The <see cref="GridCurrentCellActivatingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellActivatingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CurrentCellActivated
        void grid_CurrentCellActivated(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellActivated(args);
        }

        public static readonly RoutedEvent CurrentCellActivatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivated",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnCurrentCellActivated(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellActivatedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs after the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs after the grid activates the specified cell as current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellActivated
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellActivatedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellActivatedEvent, value);
            }
        }
        #endregion
        #region CurrentCellActivateFailed

        void grid_CurrentCellActivateFailed(object sender, GridCurrentCellActivateFailedEventArgs args)
        {
            this.OnCurrentCellActivateFailed(args);
        }

        public static readonly RoutedEvent CurrentCellActivateFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivateFailed",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellActivateFailedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellActivateFailed"/> event.
        /// </summary>
        /// <param name="args">The <see cref="GridCurrentCellActivateFailedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellActivateFailedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs after the grid fails to activate a specific cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivateFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs after the grid fails to activate a specific cell as current cell."),
        Category("Behavior")
        ]
        public event GridCurrentCellActivateFailedEventHandler CurrentCellActivateFailed
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellActivateFailedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellActivateFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivating
        void grid_CurrentCellDeactivating(object sender, SyncfusionCancelRoutedEventArgs args)
        {
            this.OnCurrentCellDeactivating(args);
        }

        public static readonly RoutedEvent CurrentCellDeactivatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivating",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellDeactivating"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivating(SyncfusionCancelRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellDeactivatingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs before the grid the deactivates the current cell.
        /// </summary>
        /// <remarks>
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCell.Deactivate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs before the grid the deactivates the current cell."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellDeactivating
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellDeactivatingEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellDeactivatingEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivated
        void grid_CurrentCellDeactivated(object sender, GridCurrentCellDeactivatedEventArgs args)
        {
            this.OnCurrentCellDeactivated(args);
        }

        public static readonly RoutedEvent CurrentCellDeactivatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivated",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellDeactivatedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellDeactivated"/> event.
        /// </summary>
        /// <param name="args">A <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellDeactivatedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs after the grid deactivates current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called. The event occurs after any <see cref="GridDataControl.CurrentCellRejectedChanges"/>,
        ///  <see cref="GridDataControl.CurrentCellAcceptedChanges"/>, <see cref="GridDataControl.CurrentCellRejectedChanges"/>, or
        /// <see cref="GridDataControl.CurrentCellAcceptedChanges"/> are raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs after the grid deactivates current cell."),
        Category("Behavior")
        ]
        public event GridCurrentCellDeactivatedEventHandler CurrentCellDeactivated
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellDeactivatedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellDeactivatedEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivateFailed
        void grid_CurrentCellDeactivateFailed(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellDeactivateFailed(args);
        }

        public static readonly RoutedEvent CurrentCellDeactivateFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivateFailed",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellDeactivateFailed"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivateFailed(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellDeactivateFailedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs after the grid fails to deactivate the current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called and can not deactivate the current cell. The reason deactivation may fail could be
        /// that the cell's contents were invalid or any of the event handlers associated with deactivating the current cell
        /// signaled to abort this operation.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs after the grid fails to deactivate the current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellDeactivateFailed
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellDeactivateFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellDeactivateFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellConfirmChangesFailed
        void grid_CurrentCellConfirmChangesFailed(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellConfirmChangesFailed(args);
        }

        public static readonly RoutedEvent CurrentCellConfirmChangesFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellConfirmChangesFailed",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="CurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellConfirmChangesFailed(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellConfirmChangesFailedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid could not save changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges"/>
        /// method is called and its contents were modified and could not be succesfully validated
        /// or saved back to the data source.
        /// <para/>
        /// The <see cref="GridCurrentCell.Exception"/> and <see cref="GridCurrentCell.ErrorMessage"/>
        /// properties provide details why the operation failed. If you want to display a message box
        /// be sure to reset the the error state with <see cref="GridCurrentCell.ResetError"/>.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellConfirmChangesFailed
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellConfirmChangesFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellConfirmChangesFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellAcceptedChanges
        void grid_CurrentCellAcceptedChanges(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellAcceptedChanges(args);
        }

        public static readonly RoutedEvent CurrentCellAcceptedChangesEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellAcceptedChanges",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the cancelable <see cref="GridDataControl.CurrentCellAcceptedChanges"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellAcceptedChanges(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellAcceptedChangesEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid accepts changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this cancelable event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.EndEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified and validated.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// If you assign true to <see cref="CancelEventArgs.Cancel"/>, the grid will not deactivate the current
        /// cell.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellAcceptedChanges
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellAcceptedChangesEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellAcceptedChangesEvent, value);
            }
        }
        #endregion
        #region CurrentCellChanging
        void grid_CurrentCellChanging(object sender, SyncfusionCancelRoutedEventArgs args)
        {
            this.OnCurrentCellChanging(args);
        }

        public static readonly RoutedEvent CurrentCellChangingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellChanging",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellChanging"/> event.
        /// </summary>
        /// <param name="args">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellChanging(SyncfusionCancelRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellChangingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the user wants to modify contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event before the changes are applied to the active cell. You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the user wants to modify contents of the current cell."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellChanging
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellChangingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellChangingEvent, value);
            }
        }

        #endregion

        #region CurrentCellStartEditing
        void grid_CurrentCellStartEditing(object sender, SyncfusionCancelRoutedEventArgs args)
        {
            this.OnCurrentCellStartEditing(args);
        }

        public static readonly RoutedEvent CurrentCellStartEditingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellStartEditing",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellStartEditing"/> event.
        /// </summary>
        /// <param name="args">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellStartEditing(SyncfusionCancelRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellStartEditingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        /// <remarks>
        /// The grid will switch into editing mode when the user presses a key while the cell
        /// is not in editing mode or when you call <see cref="GridCurrentCell.BeginEdit"/>.
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs before the current cell switches into editing mode."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellStartEditing
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellStartEditingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellStartEditingEvent, value);
            }
        }
        #endregion

        #region CurrentCellLoaded

        void grid_CurrentCellLoaded(object sender, GridCurrentCellLoadedEventArgs args)
        {
            this.OnCurrentCellLoaded(args);
        }

        //CurrentCellLoaded Event. This event will get hooked after the CurrentCellUIElement is loaded and after the CurrentCellStartEditing event.

        public event Syncfusion.Windows.Controls.Grid.GridControlBase.GridCurrentCellLoadedEvent CurrentCellLoaded;
        
        protected virtual void OnCurrentCellLoaded(GridCurrentCellLoadedEventArgs args)
        {
            if (this.CurrentCellLoaded != null)
            {
                this.CurrentCellLoaded(this, args);
            }              
        }

        #endregion

        #region CurrentCellEditingComplete
        void grid_CurrentCellEditingComplete(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellEditingComplete(args);
        }

        public static readonly RoutedEvent CurrentCellEditingCompleteEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellEditingComplete",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellEditingComplete"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellEditingComplete(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellEditingCompleteEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid completes editing mode for the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.EndEdit"/>
        /// or <see cref="GridCurrentCell.CancelEdit"/> method is called. The event occurs after <see cref="GridDataControl.CurrentCellRejectedChanges"/>
        /// or <see cref="GridDataControl.CurrentCellAcceptedChanges"/> were raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid completes editing mode for the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellEditingComplete
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellEditingCompleteEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellEditingCompleteEvent, value);
            }
        }
        #endregion
        #region CurrentCellRejectedChanges
        void grid_CurrentCellRejectedChanges(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellRejectedChanges(args);
        }

        public static readonly RoutedEvent CurrentCellRejectedChangesEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellRejectedChanges",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellRejectedChanges"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellRejectedChanges(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellRejectedChangesEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid rejects changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.RejectChanges"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.CancelEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid rejects changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellRejectedChanges
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellRejectedChangesEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellRejectedChangesEvent, value);
            }
        }
        #endregion
        #region CurrentCellChanged
        void grid_CurrentCellChanged(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellChanged(args);
        }

        public static readonly RoutedEvent CurrentCellChangedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellChanged",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellChanged"/> event.
        /// </summary>
        /// <param name="args">A <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellChanged(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellChangedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the user changes contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event whenever changes occur, similar to a <see cref="TextBoxBase.ModifiedChanged"/> event.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the user changes contents of the current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellChanged
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellChangedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoved
        void grid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs args)
        {
            this.OnCurrentCellMoved(args);
        }

        public static readonly RoutedEvent CurrentCellMovedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoved",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMovedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellMoved"/> event.
        /// </summary>
        /// <param name="args">A <see cref="GridCurrentCellMovedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoved(GridCurrentCellMovedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellMovedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the current cell has been successfully moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell has been successfully moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMovedEventHandler CurrentCellMoved
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellMovedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellMovedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoveFailed
        void grid_CurrentCellMoveFailed(object sender, GridCurrentCellMoveFailedEventArgs args)
        {
            this.OnCurrentCellMoveFailed(args);
        }

        public static readonly RoutedEvent CurrentCellMoveFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoveFailed",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMoveFailedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellMoveFailed"/> event.
        /// </summary>
        /// <param name="args">A <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellMoveFailedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the current cell fails to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// <see cref="GridCurrentCell.ErrorMessage"/> might hold an error message
        /// why the operation failed.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMoveFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell fails to be moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMoveFailedEventHandler CurrentCellMoveFailed
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellMoveFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellMoveFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoving
        void grid_CurrentCellMoving(object sender, GridCurrentCellMovingEventArgs args)
        {
            this.OnCurrentCellMoving(args);
        }

        public static readonly RoutedEvent CurrentCellMovingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoving",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMovingEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellMoving"/> event.
        /// </summary>
        /// <param name="args">A <see cref="GridCurrentCellMovingEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoving(GridCurrentCellMovingEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellMovingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the current cell is about to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// You can disallow the activataion of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.
        /// <para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.CellRowColumnIndex"/>
        /// to activate a different cell.
        /// <para/>
        /// You can also modify the <see cref="GridCurrentCellActivatingEventArgs.Options"/>.
        /// <para/>
        /// Once the current cell has been moved, a <see cref="GridDataControl.CurrentCellMoved"/> event
        /// is raised or a <see cref="GridDataControl.CurrentCellMoveFailed"/> if moving to the specified
        /// target cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovingEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell is about to be moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMovingEventHandler CurrentCellMoving
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellMovingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellMovingEvent, value);
            }
        }
        #endregion
        #region CurrentCellValidating
        void grid_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs args)
        {
            this.OnCurrentCellValidating(args);
        }

        public static readonly RoutedEvent CurrentCellValidatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellValidating",
            RoutingStrategy.Direct,
            typeof(CurrentCellValidatingEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the cancelable <see cref="GridDataControl.CurrentCellValidating"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidating(CurrentCellValidatingEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellValidatingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid validates contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can mark the contents as invalid by by setting <see cref="CancelEventArgs.Cancel"/> to True.<para/>
        /// The grid raises this event when the <see cref="GridDataControl.CurrentCell"/> object's <see cref="GridCurrentCell.Validate"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> calls this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
        public event CurrentCellValidatingEventHandler CurrentCellValidating
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellValidatingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellValidatingEvent, value);
            }
        }
        #endregion

        #region CurrentCellValidated
        void grid_CurrentCellValidated(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCurrentCellValidated(args);
        }

        public static readonly RoutedEvent CurrentCellValidatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellValidated",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Raises the <see cref="GridDataControl.CurrentCellValidated"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidated(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CurrentCellValidatedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Occurs when the grid has successfully validated the contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridDataControl.CurrentCell"/> object
        /// in <see cref="GridDataControl"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellValidated
        {
            add
            {
                this.AddHandler(GridDataControl.CurrentCellValidatedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CurrentCellValidatedEvent, value);
            }
        }
        #endregion

        #region CellPreviewKeyDown

        void grid_CellPreviewKeyDown(object sender, GridCellKeyEventArgs args)
        {
            this.OnCurrentCellPreviewKeyDown(args);
        }

        public static readonly RoutedEvent CellPreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
            "CellPreviewKeyDown",
            RoutingStrategy.Tunnel,
            typeof(GridCellKeyEventHandler),
            typeof(GridDataControl));

        public event GridCellKeyEventHandler CellPreviewKeyDown
        {
            add
            {
                this.AddHandler(GridDataControl.CellPreviewKeyDownEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.CellPreviewKeyDownEvent, value);
            }
        }

        protected virtual void OnCurrentCellPreviewKeyDown(GridCellKeyEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellPreviewKeyDownEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion

        #region CurrentCellKeyDown

        void grid_CellKeyDown(object sender, GridCellKeyEventArgs args)
        {
            this.OnCurrentCellKeyDown(args);
        }

        public static readonly RoutedEvent CellKeyDownEvent = EventManager.RegisterRoutedEvent(
            "CellKeyDown",
            RoutingStrategy.Tunnel,
            typeof(GridCellKeyEventHandler),
            typeof(GridDataControl));

        public event GridCellKeyEventHandler CellKeyDown
        {
            add
            {
                this.AddHandler(GridDataControl.CellKeyDownEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.CellKeyDownEvent, value);
            }
        }

        protected virtual void OnCurrentCellKeyDown(GridCellKeyEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellKeyDownEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion

        #region RowValueCommitting

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. Occurs Row focus moved to another row.
        /// </summary>
        public event GridDataRowValueCommittingEventHandler RowValueCommitting;

        /// <summary>
        /// Raises the row value committing event.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="value">The value.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        internal bool RaiseRowValueCommittingEvent(GridDataRecord record, Dictionary<string, object> value, int rowIndex)
        {
            GridDataRowValueCommittingEventArgs args = new GridDataRowValueCommittingEventArgs()
            {
                Record = record,
                EditedValues = value,
                RowIndex = rowIndex
            };

            if (this.RowValueCommitting != null)
            {
                this.RowValueCommitting(this, args);
            }

            return args.Cancel;
        }

        #endregion

        #region RowValueCommitted

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. If committing row values success means this event will fires
        public event GridDataRowValueCommittedEventHandler RowValueCommitted;

        /// <summary>
        /// Raises the row value committed event.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="value">The value.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        internal bool RaiseRowValueCommittedEvent(GridDataRecord record, Dictionary<string, object> value, int rowIndex)
        {
            GridDataRowValueCommittedEventArgs args = new GridDataRowValueCommittedEventArgs()
            {
                Record = record,
                NewValues = value,
                RowIndex = rowIndex
            };

            if (this.RowValueCommitted != null)
            {
                this.RowValueCommitted(this, args);
            }

            return args.Cancel;
        }

        #endregion

        #region RowValueCommittingCancelled

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. If committing row values cancelled means this event will fires
        public event GridDataRowValueCommittingCancelledEventHandler RowValueCommittingCancelled;

        /// <summary>
        /// Raises the row value committing cancelled event.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="rowIndex">Index of the row.</param>
        internal void RaiseRowValueCommittingCancelledEvent(GridDataRecord record, int rowIndex)
        {
            GridDataRowValueCommittingCancelledEventArgs args = new GridDataRowValueCommittingCancelledEventArgs()
            {
                Record = record,
                RowIndex = rowIndex
            };

            if (this.RowValueCommittingCancelled != null)
            {
                this.RowValueCommittingCancelled(this, args);
            }
        }      
        
        #endregion

        #region RowValidating

        /// <summary>
        /// Occurs Row focus moved to another row.
        /// </summary>
        public static readonly RoutedEvent GridRowValidatingEvent = EventManager.RegisterRoutedEvent(
            "RowValidating",
            RoutingStrategy.Tunnel,
            typeof(GridDataRowValidatingEventHandler),
            typeof(GridDataControl));

        /// <summary>
        /// Occurs when [row validating].
        /// </summary>
        public event GridDataRowValidatingEventHandler RowValidating
        {
            add
            {
                this.AddHandler(GridDataControl.GridRowValidatingEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.GridRowValidatingEvent, value);
            }
        }

        /// <summary>
        /// Raises the row validating event.
        /// </summary>
        /// <param name="currentRowIndex">Index of the current row.</param>
        /// <param name="record">The record.</param>
        /// <param name="newValues">The new values.</param>
        /// <returns></returns>
        internal bool RaiseRowValidatingEvent(int currentRowIndex,object record,Dictionary<string,object> newValues )
        {
            GridDataRowValidatingEventArgs args = new GridDataRowValidatingEventArgs( )
            {
                IsValid=true,
                RowIndex=currentRowIndex,
                Record = record,
                NewValues = newValues
            };
            args.RoutedEvent = GridDataControl.GridRowValidatingEvent;
            args.Source = this;
            base.RaiseEvent(args);
            return args.IsValid;
        }

        #endregion

        #region
        void Table_RecordSelectionChanging(object sender, GridDataRecordSelectionChangingEventArgs e)
        {
            this.OnRecordSelectionChanging(e);
        }

        public static readonly RoutedEvent GridRecordsSelectionChangingEvent = EventManager.RegisterRoutedEvent(
            "RecordsSelectionChanging",
            RoutingStrategy.Tunnel,
            typeof(GridDataRecordSelectionChangingEventHandler),
            typeof(GridDataControl));

        public event GridDataRecordSelectionChangingEventHandler RecordsSelectionChanging
        {
            add
            {
                this.AddHandler(GridDataControl.GridRecordsSelectionChangingEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.GridRecordsSelectionChangingEvent, value);
            }
        }

        protected virtual void OnRecordSelectionChanging(GridDataRecordSelectionChangingEventArgs args)
        {
            args.RoutedEvent = GridDataControl.GridRecordsSelectionChangingEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        void Table_RecordsSelectionChanged(object sender, GridDataRecordsSelectionChangedEventArgs e)
        {
            this.OnRecordsSelectionChanged(e);
        }

        public static readonly RoutedEvent GridRecordsSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "RecordsSelectionChanged",
            RoutingStrategy.Tunnel,
            typeof(GridDataRecordsSelectionChangedEventHandler),
            typeof(GridDataControl));

        public event GridDataRecordsSelectionChangedEventHandler RecordsSelectionChanged
        {
            add
            {
                this.AddHandler(GridDataControl.GridRecordsSelectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.GridRecordsSelectionChangedEvent, value);
            }
        }

        protected virtual void OnRecordsSelectionChanged(GridDataRecordsSelectionChangedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.GridRecordsSelectionChangedEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }
        #endregion
        #endregion

        #region CellEvents

        #region CellButtonClick
        void grid_CellButtonClick(object sender, GridCellButtonClickEventArgs args)
        {
            this.OnCellButtonClick(args);
        }

        public static readonly RoutedEvent CellButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CellButtonClick",
            RoutingStrategy.Direct,
            typeof(GridCellButtonClickEventHandler),
            typeof(GridDataControl));

        public event GridCellButtonClickEventHandler CellButtonClick
        {
            add
            {
                this.AddHandler(GridDataControl.CellButtonClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellButtonClickEvent, value);
            }
        }

        protected virtual void OnCellButtonClick(GridCellButtonClickEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellButtonClickEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion


        #region DropDownSelectionChanged
        void grid_DropDownSelectionChanged(object sender, GridCellComboValueChangedEventArgs args)
        {
            this.OnDropDownSelectionChanged(args);
        }

        public static readonly RoutedEvent DropDownSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "DropDownSelectionChanged",
            RoutingStrategy.Direct,
            typeof(GridCellComboValueChangedEventHandler),
            typeof(GridDataControl));

        public event GridCellComboValueChangedEventHandler DropDownSelectionChanged
        {
            add
            {
                this.AddHandler(GridDataControl.DropDownSelectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.DropDownSelectionChangedEvent, value);
            }
        }

        protected virtual void OnDropDownSelectionChanged(GridCellComboValueChangedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.DropDownSelectionChangedEvent;
            args.Source = this;
            this.RaiseEvent(args);
        }

        #endregion
        #region CellClick
        void grid_CellClick(object sender, GridCellClickEventArgs args)
        {
            this.OnCellClick(args);
        }

        public static readonly RoutedEvent CellClickEvent = EventManager.RegisterRoutedEvent(
            "CellClick",
            RoutingStrategy.Bubble,
            typeof(GridCellClickEventHandler),
            typeof(GridDataControl));

        public event GridCellClickEventHandler CellClick
        {
            add
            {
                this.AddHandler(GridDataControl.CellClickEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellClickEvent, value);
            }
        }

        protected virtual void OnCellClick(GridCellClickEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellClickEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellCursor
        void grid_CellCursor(object sender, GridCellCursorEventArgs args)
        {
            this.OnGridCellCursor(args);
        }

        public static readonly RoutedEvent CellCursorEvent = EventManager.RegisterRoutedEvent("CellCursor",
            RoutingStrategy.Direct,
            typeof(GridCellCursorEventHandler),
            typeof(GridDataControl));

        public event GridCellCursorEventHandler CellCursor
        {
            add
            {
                this.AddHandler(GridDataControl.CellCursorEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellCursorEvent, value);
            }
        }

        protected virtual void OnGridCellCursor(GridCellCursorEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellCursorEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHoverEnter
        void grid_CellMouseHoverEnter(object sender, GridCellMouseEventArgs args)
        {
            this.OnCellMouseHoverEnter(args);
        }

        public static readonly RoutedEvent CellMouseHoverEnterEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHoverEnter",
            RoutingStrategy.Direct,
            typeof(GridCellMouseEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseEventHandler CellMouseHoverEnter
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseHoverEnterEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseHoverEnterEvent, value);
            }
        }

        protected virtual void OnCellMouseHoverEnter(GridCellMouseEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseHoverEnterEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHover
        void grid_CellMouseHover(object sender, GridCellMouseControllerEventArgs args)
        {
            this.OnCellMouseHover(args);
        }

        public static readonly RoutedEvent CellMouseHoverEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHover",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseControllerEventHandler CellMouseHover
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseHoverEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseHoverEvent, value);
            }
        }

        protected virtual void OnCellMouseHover(GridCellMouseControllerEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseHoverEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHoverLeave
        void grid_CellMouseHoverLeave(object sender, GridCellMouseEventArgs args)
        {
            this.OnCellMouseHoverLeave(args);
        }

        public static readonly RoutedEvent CellMouseHoverLeaveEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHoverLeave",
            RoutingStrategy.Direct,
            typeof(GridCellMouseEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseEventHandler CellMouseHoverLeave
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseHoverLeaveEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseHoverLeaveEvent, value);
            }
        }

        protected virtual void OnCellMouseHoverLeave(GridCellMouseEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseHoverLeaveEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseDown
        void grid_CellMouseDown(object sender, GridCellMouseControllerEventArgs args)
        {
            this.OnCellMouseDown(args);
        }

        public static readonly RoutedEvent CellMouseDownEvent = EventManager.RegisterRoutedEvent(
            "CellMouseDown",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseControllerEventHandler CellMouseDown
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseDownEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseDownEvent, value);
            }
        }

        protected virtual void OnCellMouseDown(GridCellMouseControllerEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseDownEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseMove
        void grid_CellMouseMove(object sender, GridCellMouseControllerEventArgs args)
        {
            this.OnCellMouseMove(args);
        }

        public static readonly RoutedEvent CellMouseMoveEvent = EventManager.RegisterRoutedEvent(
            "CellMouseMove",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseControllerEventHandler CellMouseMove
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseMoveEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseMoveEvent, value);
            }
        }

        protected virtual void OnCellMouseMove(GridCellMouseControllerEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseMoveEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseUp
        void grid_CellMouseUp(object sender, GridCellMouseControllerEventArgs args)
        {
            this.OnCellMouseUp(args);
        }

        public static readonly RoutedEvent CellMouseUpEvent = EventManager.RegisterRoutedEvent(
            "CellMouseUp",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridDataControl));

        public event GridCellMouseControllerEventHandler CellMouseUp
        {
            add
            {
                this.AddHandler(GridDataControl.CellMouseUpEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellMouseUpEvent, value);
            }
        }

        protected virtual void OnCellMouseUp(GridCellMouseControllerEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellMouseUpEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellCancelMode
        void grid_CellCancelMode(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCellCancelMode(args);
        }

        public static readonly RoutedEvent CellCancelModeEvent = EventManager.RegisterRoutedEvent(
            "CellCancelMode",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        public event GridRoutedEventHandler CellCancelMode
        {
            add
            {
                this.AddHandler(GridDataControl.CellCancelModeEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellCancelModeEvent, value);
            }
        }

        protected virtual void OnCellCancelMode(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellCancelModeEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region CellRestoreMode
        void grid_CellRestoreMode(object sender, SyncfusionRoutedEventArgs args)
        {
            this.OnCellRestoreMode(args);
        }

        public static readonly RoutedEvent CellRestoreModeEvent = EventManager.RegisterRoutedEvent(
            "CellRestoreMode",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridDataControl));

        public event GridRoutedEventHandler CellRestoreMode
        {
            add
            {
                this.AddHandler(GridDataControl.CellRestoreModeEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.CellRestoreModeEvent, value);
            }
        }

        protected virtual void OnCellRestoreMode(SyncfusionRoutedEventArgs args)
        {
            args.RoutedEvent = GridDataControl.CellRestoreModeEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region ResizingColumns
        void grid_ResizingColumns(object sender, GridResizingColumnsEventArgs args)
        {
            this.OnResizingColumns(args);
        }

        public static readonly RoutedEvent ResizingColumnsEvent = EventManager.RegisterRoutedEvent(
            "ResizingColumns",
            RoutingStrategy.Direct,
            typeof(GridResizingColumnsEventHandler),
            typeof(GridDataControl));

        public event GridResizingColumnsEventHandler ResizingColumns
        {
            add
            {
                this.AddHandler(GridDataControl.ResizingColumnsEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.ResizingColumnsEvent, value);
            }
        }

        protected virtual void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            args.RoutedEvent = GridDataControl.ResizingColumnsEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion
        #region ColumnDrag
        void grid_QueryAllowDragColumn(object sender, GridQueryDragColumnHeaderEventArgs args)
        {
            this.OnRaiseQueryAllowDragColumn(args);
        }

        public static readonly RoutedEvent QueryAllowDragColumnEvent = EventManager.RegisterRoutedEvent(
            "QueryAllowDragColumn",
            RoutingStrategy.Direct,
            typeof(GridQueryDragColumnHeaderEventHandler),
            typeof(GridDataControl));

        public event GridQueryDragColumnHeaderEventHandler QueryAllowDragColumn
        {
            add
            {
                this.AddHandler(GridDataControl.QueryAllowDragColumnEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridDataControl.QueryAllowDragColumnEvent, value);
            }
        }

        protected virtual void OnRaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs args)
        {
            args.RoutedEvent = GridDataControl.QueryAllowDragColumnEvent;
            args.Source = this;
            this.RaiseEvent(args);
        }

        #endregion
        #region ResizingRows
        void grid_ResizingRows(object sender, GridResizingRowsEventArgs args)
        {
            this.OnResizingRows(args);
        }

        public static readonly RoutedEvent ResizingRowsEvent = EventManager.RegisterRoutedEvent(
            "ResizingRows",
            RoutingStrategy.Direct,
            typeof(GridResizingRowsEventHandler),
            typeof(GridDataControl));

        public event GridResizingRowsEventHandler ResizingRows
        {
            add
            {
                this.AddHandler(GridDataControl.ResizingRowsEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridDataControl.ResizingRowsEvent, value);
            }
        }

        protected virtual void OnResizingRows(GridResizingRowsEventArgs args)
        {
            args.RoutedEvent = GridDataControl.ResizingRowsEvent;
            args.Source = this;
            base.RaiseEvent(args);
        }

        #endregion

        #endregion

        #endregion

        //void ISkinStylePropagator.OnStyleChanged(string skinStyle)
        //{
        //    var visualStyle = GridDataControl.ResolveVisualStyleToSkinStorage(this.VisualStyle);
        //    if (skinStyle != visualStyle)
        //    {
        //        // we just ensure that the DO has the same visual style with SkinStorage
        //        this.VisualStyle = ResolveSkinStorageToVisualStyle(skinStyle);
        //    }
        //}
#if !SILVERLIGHT
        #region ContextMenuOptions (Dependency Property)
        public ContextMenuOptions ContextMenuOptions
        {
            get { return (ContextMenuOptions)GetValue(ContextMenuOptionsProperty); }
            set { SetValue(ContextMenuOptionsProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuOptionsProperty =
            DependencyProperty.Register("ContextMenuOptions", typeof(ContextMenuOptions), typeof(GridDataControl), new PropertyMetadata(ContextMenuOptions.Default, OnContextMenuOptionsPropertyChanged));

        private static void OnContextMenuOptionsPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ContextMenuOptions = (ContextMenuOptions)args.NewValue;
        }

        #endregion

        public IEnumerable HeaderContextMenuItems
        {
            get { return (IEnumerable)GetValue(HeaderContextMenuItemsProperty); }
            set { SetValue(HeaderContextMenuItemsProperty, value); }
        }

        public static readonly DependencyProperty HeaderContextMenuItemsProperty =
            DependencyProperty.Register("HeaderContextMenuItems", typeof(IEnumerable), typeof(GridDataControl), new PropertyMetadata(OnHeaderContextMenuItemsChanged));

        bool isHeaderContextMenuLoadedBeforeGridLoaded = false;
        private static void OnHeaderContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
                grid.TableProperties.HeaderContextMenuItems = args.NewValue as IEnumerable;
            else
                grid.isHeaderContextMenuLoadedBeforeGridLoaded = true;
        }

        public IEnumerable GroupHeaderContextMenuItems
        {
            get { return (IEnumerable)GetValue(GroupHeaderContextMenuItemsProperty); }
            set { SetValue(GroupHeaderContextMenuItemsProperty, value); }
        }

        public static readonly DependencyProperty GroupHeaderContextMenuItemsProperty =
            DependencyProperty.Register("GroupHeaderContextMenuItems", typeof(IEnumerable), typeof(GridDataControl), new PropertyMetadata(OnGroupHeaderContextMenuItemsChanged));

        bool isGroupHeaderContextMenuLoadedBeforeGridLoaded = false;

        private static void OnGroupHeaderContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
                grid.TableProperties.GroupHeaderContextMenuItems = args.NewValue as IEnumerable;
            else
                grid.isGroupHeaderContextMenuLoadedBeforeGridLoaded = true;
        }

        public IEnumerable RecordContextMenuItems
        {
            get { return (IEnumerable)GetValue(RecordContextMenuItemsProperty); }
            set { SetValue(RecordContextMenuItemsProperty, value); }
        }
        public static readonly DependencyProperty RecordContextMenuItemsProperty =
            DependencyProperty.Register("RecordContextMenuItems", typeof(IEnumerable), typeof(GridDataControl), new PropertyMetadata(OnRecordHeaderContextMenuItemsChanged));

        bool isRecordContextMenuLoadedBeforeGridLoaded = false;

        private static void OnRecordHeaderContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
                grid.TableProperties.RecordContextMenuItems = args.NewValue as IEnumerable;
            else
                grid.isRecordContextMenuLoadedBeforeGridLoaded=true;
        }

       

        public bool EnableContextMenu
        {
            get { return (bool)GetValue(EnableContextMenuProperty); }
            set { SetValue(EnableContextMenuProperty, value); }
        }

        public static readonly DependencyProperty EnableContextMenuProperty =
            DependencyProperty.Register("EnableContextMenu", typeof(bool), typeof(GridDataControl), new PropertyMetadata(OnEnableContextMenuChanged));

        bool isEnableContextMenuChanged = false;
        private static void OnEnableContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.isEnableContextMenuChanged = true;
        }

        #region QueryCellInfo Command       


        /// <summary>
        /// Gets or sets the command to invoke when QueryCellInfo event is triggered.
        /// </summary>
        [Category("Action")]        
        public ICommand QueryCellInfoCommand
        {
            get { return (ICommand)GetValue(QueryCellInfoCommandProperty); }
            set { SetValue(QueryCellInfoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QueryCellInfoCommand.  This enables to handle the QueryCellInfo event in ViewModel.
        public static readonly DependencyProperty QueryCellInfoCommandProperty =
            DependencyProperty.Register("QueryCellInfoCommand", typeof(ICommand), typeof(GridDataControl), new PropertyMetadata(null, OnQueryCellInfoCommandChanged));

        // bool isQueryCellInfoCommandChanged = false; Variable is assigned but it is never used
        private static void OnQueryCellInfoCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.QueryCellInfoCommand = (ICommand)args.NewValue;
        }
        #endregion

        #region SortColumnChanging Command
        /// <summary>
        /// Gets or sets the command to invoke when SortColumnChanging event is triggered.
        /// </summary>
        [Category("Action")]
        public ICommand SortColumnChangingCommand
        {
            get { return (ICommand)GetValue(SortColumnChangingCommandProperty); }
            set { SetValue(SortColumnChangingCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortColumnChangingCommand.  This enables to handle the SortColumnChanging event in ViewModel.
        public static readonly DependencyProperty SortColumnChangingCommandProperty =
            DependencyProperty.Register("SortColumnChangingCommand", typeof(ICommand), typeof(GridDataControl), new PropertyMetadata(null, OnSortColumnChangingCommandChanged));

        private static void OnSortColumnChangingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.SortColumnChangingCommand = (ICommand)args.NewValue;
        }
        #endregion

#endif

#if !SILVERLIGHT
        public bool ApplySizingAfterLoad
        {
            get { return (bool)GetValue(ApplySizingAfterLoadProperty); }
            set { SetValue(ApplySizingAfterLoadProperty, value); }
        }

        public static readonly DependencyProperty ApplySizingAfterLoadProperty =
            DependencyProperty.Register("ApplySizingAfterLoad", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false));

        protected override Size MeasureOverride(Size constraint)
        {
            //if (this.ApplySizingAfterLoad && this.Model != null && this.Model.ColumnWidths != null && this.BorderThickness != null)
            //{
            //    this.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        double d = this.Model.ColumnWidths.TotalExtent + this.BorderThickness.Left + this.BorderThickness.Right + 1;

            //        if (double.IsNaN(this.Width) && d < this.ActualWidth)
            //        {
            //            this.Width = d;
            //            if (this.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
            //            {
            //                this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //            }
            //        }
            //    }), null);
            //}
            if (this.Model.Grid != null)
            {
                if ((this.ColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill || this.ColumnSizer == GridControlLengthUnitType.AutoWithLastColumnFill || this.ColumnSizer == GridControlLengthUnitType.Star) && this.Model.ColumnAutoSizer.AllowAutoCalculateSize == true)
                {
                    this.Model.Grid.canAutoCalculateWidth = true;
                }
            }
            return base.MeasureOverride(constraint);
        }
#endif

    }

#if !SILVERLIGHT
    public static class GridDataCommandManager
    {
        static GridDataCommandManager()
        {
            //Command for Expand/Collapse all the Nested table.
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(CollapseAllCommand, OnCollapseAll, OnCanExecuteCollapseAll));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(ExpandAllCommand, OnExpandAll, OnCanExecuteExpandAll));

            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(ShowPrintDialogCommand, OnExecuteShowPrintDialog, OnCanExecuteShowPrintDialog));

            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(PrintCommand, OnExecutePrint, OnCanExecutePrint));

            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(MoveNextCommand, OnExecuteMoveNext, OnCanExecuteMoveNext));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(MovePreviousCommand, OnExecuteMovePrevious, OnCanExecuteMovePrevious));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(MoveFirstCommand, OnExecuteMoveFirst, OnCanExecuteMoveFirst));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(MoveLastCommand, OnExecuteMoveLast, OnCanExecuteMoveLast));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(DeleteCommand, OnExecuteDelete, OnCanExecuteDelete));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(SortCommand, OnExecuteSort, OnCanExecuteSort));
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(GroupCommand, OnExecuteGroup, OnCanExecuteGroup));
        }

        //Command for Expand/Collapse all the Nested table.
        public static RoutedCommand CollapseAllCommand = new RoutedCommand("CollapseAllCommand", typeof(GridDataControl));
        public static RoutedCommand ExpandAllCommand = new RoutedCommand("ExpandAllCommand", typeof(GridDataControl));
        public static RoutedCommand ShowPrintDialogCommand = new RoutedCommand("ShowPrintDialogCommand", typeof(GridDataControl));
        public static RoutedCommand PrintCommand = new RoutedCommand("PrintCommand", typeof(GridDataControl));
        public static RoutedCommand MoveNextCommand = new RoutedCommand("MoveNextCommand", typeof(GridDataControl));
        public static RoutedCommand MovePreviousCommand = new RoutedCommand("MovePreviousCommand", typeof(GridDataControl));
        public static RoutedCommand MoveFirstCommand = new RoutedCommand("MovePreviousCommand", typeof(GridDataControl));
        public static RoutedCommand MoveLastCommand = new RoutedCommand("MovePreviousCommand", typeof(GridDataControl));
        public static RoutedCommand DeleteCommand = new RoutedCommand("DeleteCommand", typeof(GridDataControl));
        public static RoutedCommand SortCommand = new RoutedCommand("SortCommand", typeof(GridDataControl));
        public static RoutedCommand GroupCommand = new RoutedCommand("GroupCommand", typeof(GridDataControl));

        #region Collapse All
        private static void OnCollapseAll(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.Table.CollapseAll();
        }

        private static void OnCanExecuteCollapseAll(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            args.CanExecute = model.Table != null;
        }
        #endregion

        #region Expand All
        private static void OnExpandAll(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.Table.ExpandAll();
        }

        private static void OnCanExecuteExpandAll(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            args.CanExecute = model.Table != null;
        }
        #endregion


        #region print
        private static void OnExecutePrint(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataControl dataGrid = args.Source as GridDataControl;
            dataGrid.Print();
        }

        private static void OnCanExecutePrint(object sender, CanExecuteRoutedEventArgs args)
        {
            args.Handled = true;
            args.CanExecute = true;
        }
        #endregion

        #region ShowprintDialog
        private static void OnExecuteShowPrintDialog(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataControl dataGrid = args.Source as GridDataControl;
            dataGrid.ShowPrintDialog();
        }

        private static void OnCanExecuteShowPrintDialog(object sender, CanExecuteRoutedEventArgs args)
        {           
            args.Handled = true;           
            args.CanExecute =true;            
        }
        #endregion


        #region Move Next
        private static void OnExecuteMoveNext(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.MoveCurrentToNext();
        }

        private static void OnCanExecuteMoveNext(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            if (model.View != null)
            {
                args.CanExecute = model.View.CurrentPosition < model.View.Records.Count - 1 && model.View.CurrentItem != null;
            }
        }
        #endregion

        #region Move Previous
        private static void OnExecuteMovePrevious(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.MoveCurrentToPrevious();
        }

        private static void OnCanExecuteMovePrevious(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            if (model.View != null)
            {
                args.CanExecute = model.View.CurrentPosition > 0 && model.View.CurrentItem != null;
            }
        }
        #endregion

        #region Move First
        private static void OnExecuteMoveFirst(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.MoveCurrentToFirst();
        }

        private static void OnCanExecuteMoveFirst(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            if (model.View != null)
            {
                args.CanExecute = model.View.CurrentPosition > 0 && model.View.CurrentItem != null;
            }
        }
        #endregion

        #region Move Last
        private static void OnExecuteMoveLast(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.MoveCurrentToLast();
        }

        private static void OnCanExecuteMoveLast(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            if (model.View != null)
            {
                args.CanExecute = model.View.CurrentPosition < model.View.Records.Count - 1 && model.View.CurrentItem != null;
            }
        }
        #endregion

        #region Delete
        private static void OnExecuteDelete(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.Remove(model.View.CurrentItem);
            model.CurrencyManager.MoveTo(model.View.CurrentPosition);
        }

        private static void OnCanExecuteDelete(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            args.Handled = true;
            if (model.View != null)
            {
                args.CanExecute = model.View.CurrentItem != null;
            }
        }
        #endregion

        #region Sort
        private static void OnExecuteSort(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataControl grid = args.Source as GridDataControl;
            GridDataTableModel model = (args.Source as GridDataControl).Model;
            model.View.Remove(model.View.CurrentItem);
            if (model.View != null)
            {
                model.SortColumn(grid.VisibleColumns[args.Parameter.ToString()]);
            }
        }

        private static void OnCanExecuteSort(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataControl grid = args.Source as GridDataControl;
            // GridDataTableModel model = grid.Model; Unused local variable
            args.Handled = true;
            args.CanExecute = args.Parameter != null && grid.VisibleColumns[args.Parameter.ToString()] != null;
        }
        #endregion

        #region Group
        private static void OnExecuteGroup(object sender, ExecutedRoutedEventArgs args)
        {
            GridDataControl grid = args.Source as GridDataControl;
            // GridDataTableModel model = (args.Source as GridDataControl).Model; Unused local variable
            GridDataGroupColumn groupColumn = new GridDataGroupColumn()
            {
                ColumnName = args.Parameter.ToString()
            };
            grid.GroupedColumns.Add(groupColumn);
        }

        private static void OnCanExecuteGroup(object sender, CanExecuteRoutedEventArgs args)
        {
            GridDataControl grid = args.Source as GridDataControl;
            // GridDataTableModel model = grid.Model; Unused local variable
            args.Handled = true;
            args.CanExecute = args.Parameter != null && grid.VisibleColumns[args.Parameter.ToString()] != null;
        }
        #endregion
    }
#endif
#if !SILVERLIGHT
    public enum ContextMenuOptions
    {
        Default,
        Custom,
        CustomWithDefault
    }
#endif
    /// <summary>
    /// Specifies the Visual Style settings for the <see cref="GridDataControl"/> and <see cref="GridTreeControl"/>.
    /// </summary>
    public enum VisualStyle
    {
        /// <summary>
        /// Default skin.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Office 2007 Blue skin.
        /// </summary>
        Office2007Blue = 1,
        /// <summary>
        /// Office 2007 Silver skin.
        /// </summary>
        Office2007Silver = 2,
        /// <summary>
        /// Office 2007 Black skin.
        /// </summary>
        Office2007Black = 3,
        /// <summary>
        /// Office 2003 skin.
        /// </summary>
        Office2003 = 4,
        /// <summary>
        /// Blend skin.
        /// </summary>
        Blend = 5,
        /// <summary>
        /// Custom skin. Implement <see cref="IGridVisualStyle"/> interface for this skin.
        /// </summary>
        Custom = 6,
        /// <summary>
        /// Glassy Green skin.
        /// </summary>
        GlassyGreen = 7,
        /// <summary>
        /// Sun Black skin.
        /// </summary>
        SunBlack = 8,
        /// <summary>
        /// Shiny Red skin.
        /// </summary>
        ShinyRed = 9,
        /// <summary>
        /// Shiny Blue skin.
        /// </summary>
        ShinyBlue = 10,
        /// <summary>
        /// Bureau Blue skin.
        /// </summary>
        BureauBlue = 11,
        /// <summary>
        /// Bureau Black skin.
        /// </summary>
        BureauBlack = 12,
        /// <summary>
        /// Twilight Blue skin.
        /// </summary>
        TwilightBlue = 13,
        /// <summary>
        /// Default Office 2007 Blue skin.
        /// </summary>
        DefaultOffice2007Blue = 14,
        /// <summary>
        /// Default Office 2007 Silver skin.
        /// </summary>
        DefaultOffice2007Silver = 15,
        /// <summary>
        /// Default Office 2007 Black skin.
        /// </summary>
        DefaultOffice2007Black = 16,
        /// <summary>
        /// Office 14 Blue skin.
        /// </summary>
        Office14Blue = 17,
        /// <summary>
        /// Office 14 Silver skin.
        /// </summary>
        Office14Silver = 18,
        /// <summary>
        /// Office 14 Black skin.
        /// </summary>
        Office14Black = 19,
        /// <summary>
        /// VS2010 Black skin.
        /// </summary>
        VS2010 = 20,
        /// <summary>
        /// Windows 7
        /// </summary>
        Windows7 = 21,
        /// <summary>
        /// Syncfusion Theme
        /// </summary>
        SyncfusionTheme = 22,
        /// <summary>
        /// Metro Theme
        /// </summary>
        Metro = 23
    }

}
