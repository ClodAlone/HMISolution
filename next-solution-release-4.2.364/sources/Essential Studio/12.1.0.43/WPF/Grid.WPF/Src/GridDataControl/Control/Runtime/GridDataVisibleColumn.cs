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
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
#if !SILVERLIGHT
    using Syncfusion.Windows.Shared;
#endif
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Data;
    using System.Linq.Expressions;
    using System.Windows.Data;
    using System.Collections.Specialized;
    using System.Xml.Serialization;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Globalization;
    using Syncfusion.Windows.Controls.Grid;
   
    

#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataVisibleColumns
#if !SILVERLIGHT
 : FreezableCollection<GridDataVisibleColumn>, IDisposable
#else
 : ObservableCollection<GridDataVisibleColumn>, IDisposable
#endif
    {
        public GridDataVisibleColumns()
        {
#if !SILVERLIGHT
            ((INotifyCollectionChanged)this).CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
#endif
        }

#if !SILVERLIGHT
        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

#else
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
#endif

            if (this.TableModel == null || this.IsInSuspend)
            {

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (GridDataVisibleColumn v in e.NewItems)
                    {
                        this.WireVisibleColumnDescriptor(v);
                        this.ValidateColumn(v);
                        // if we have a nested table, we put an empty column in the end to maintain the delta, so handle column++ differently
                        this.TableModel.InsertColumns(this.TableModel.ResolveVisibleColumnIndexToPosition(e.NewStartingIndex), 1);
                        var shouldSetColumnCount = this.Count == 1 ? true : false;
                        this.TableModel.RefreshColumns(shouldSetColumnCount, false);
                        //var colIndex = this.IndexOf(v);
                        //var actualColIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(colIndex);
                        //this.TableModel.ColumnWidths[actualColIndex] = v.ActualWidth;//((GridDataControlBaseImpl)this.TableModel.Grid).GridDataColumnSizer.ApplyColumnSizer(v);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // if we have a nested table, we put an empty column in the end to maintain the delta, so handle column++ differently
                var column = e.OldItems[0] as GridDataVisibleColumn;
                this.UnwireVisibleColumnDescriptor(column);
                this.TableModel.RemoveColumns(this.TableModel.ResolveVisibleColumnIndexToPosition(e.OldStartingIndex), 1);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.TableModel.Reset();
                if (this.TableProperties.ItemsSource != null)
                    this.TableModel.ColumnCount = this.TableModel.TableProperties.ShowRowHeader ? 1 : 0;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var prevColumn = e.OldItems[e.OldStartingIndex] as GridDataVisibleColumn;
                this.UnwireVisibleColumnDescriptor(prevColumn);
                var newColumn = e.NewItems[e.NewStartingIndex] as GridDataVisibleColumn;
                this.WireVisibleColumnDescriptor(newColumn);
            }

            if (this.TableModel != null && this.TableModel.Table != null && this.TableModel.Table.HasStackedHeaders)
            {
                var range = GridRangeInfo.Rows(0, this.TableModel.TableProperties.StackedHeaderRows.Count);
                this.TableModel.InvalidateCell(range);
            }

            if (this.TableModel.ColumnAutoSizer != null)
            {
                this.TableModel.ColumnAutoSizer.RefreshAll();
            }
            var AllowFiltersVisibleColumn = this.FirstOrDefault(column => column.AllowFilter == true);
            if (AllowFiltersVisibleColumn != null && !AllowFiltersVisibleColumn.IsAdvancedFilteringMode)
                if (this.TableModel.View != null)
                    (this.TableModel.View as IExcelLikeFilterExt).IsExcelLikeFilter = true;
        }

        public void Dispose()
        {
#if !SILVERLIGHT
            ((INotifyCollectionChanged)this).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
#endif
            this.isInSuspend = true;
            foreach (var visibleCol in this)
            {
                this.UnwireVisibleColumnDescriptor(visibleCol);
                visibleCol.SetTableModel(null);
                visibleCol.Dispose();
            }

            this.Clear();
            this.isInSuspend = false;

            this.TableModel = null;
        }

        [XmlIgnore]
        public GridDataTableModel TableModel
        {
            get;
            private set;
        }

        [XmlIgnore]
        internal GridDataTableProperties TableProperties
        {
            get
            {
                return this.TableModel.TableProperties;
            }
        }

        public IEnumerable<string> GetMappingNames()
        {
            foreach (GridDataVisibleColumn col in this)
                if (!col.IsUnbound)
                    yield return col.MappingName;
        }

        internal void SetTableModel(GridDataTableModel model)
        {
            if (this.TableModel != model)
            {
                this.TableModel = model;
#if !SILVERLIGHT
                if (this.TableModel.View is GridDataTableCollectionViewWrapper)
                {
                    var view = this.TableModel.View as GridDataTableCollectionViewWrapper;
                    view.EnsureRecordsInitialized();
                }
#endif
                foreach (var column in this)
                {
                    this.ValidateColumn(column);
                }
            }
            if (this.TableModel != null)// && this.TableModel.SourceList != null)
            {
                foreach (var column in this)
                {
                    this.WireVisibleColumnDescriptor(column);
                }
            }

            if (this.TableModel != null && this.TableProperties.ItemsSource == null)
            {
                if (this.Count != 0)
                {
                    this.TableModel.ColumnCount = this.Count;
                }
            }
            if (this.TableModel != null && this.TableModel.Grid != null)
            {
                foreach (var col in this)
                {
                    if (col.AllowDrag)
                    {
                        var dragController = col.TableModel.Grid.MouseControllerDispatcher.Find(GridDataGroupDragMouseController.MouseControllerName);
                        if (dragController == null)
                            col.TableModel.Grid.MouseControllerDispatcher.Add(new GridDataGroupDragMouseController(col.TableModel.Grid));
                    }
                }
            }
        }

        public GridDataVisibleColumn this[string mappingName]
        {
            get
            {
                var column = this.FirstOrDefault(v => v.MappingName == mappingName);
                return column;
            }
        }

        private bool isInSuspend = false;
        [XmlIgnore]
        public bool IsInSuspend
        {
            get { return this.isInSuspend; }
        }

        public void SuspendEvents()
        {
            this.isInSuspend = true;
        }

        public void ResumeEvents()
        {
            this.isInSuspend = false;
        }

        public void Move(int fromIndex, int toIndex)
        {
            // if (fromIndex != null && toIndex != null) int value is never equal to null
            {
                var column = this[fromIndex];
                this.RemoveAt(fromIndex);
                this.Insert(toIndex, column);
            }
        }

        internal List<GridDataVisibleColumn> GetActualVisibleColumns()
        {
            return this.Where(v => !v.IsHidden).ToList();
        }

        private void ValidateColumn(GridDataVisibleColumn v)
        {
            this.TableModel.RaiseQueryVisibleColumnInfo(new QueryVisibleColumnInfoArgs() { VisibleColumn = v });
            if (!this.ValidateVisibleColumn(v))
            {
                throw new InvalidOperationException(string.Format("{0} - {1}", GridDataResourceWrapper.InvalidColumn, v.MappingName));
            }
        }

        internal bool ValidateVisibleColumn(GridDataVisibleColumn v)
        {
            if (!v.IsUnbound)
            {
                if (v.MappingName == null && v.Binding != null)
                {
                    v.MappingName = v.Binding.Path.Path;
                }
                if (this.TableModel.View != null)
                {
                    var itemProperties = this.TableModel.View.GetItemProperties();
                    var isValid = itemProperties != null ? (v.Binding != null ? true : itemProperties.GetPropertyDescriptor(v.MappingName) != null) : false;
                    //var isValid = itemProperties != null ? itemProperties.GetPropertyDescriptor(v.MappingName) != null : false;

                    if (this.TableModel.Table.IsDynamicBound && v.Binding == null)
                    {
#if SyncfusionFramework4_0
                        var record = this.TableModel.View.Records.Count > 0 ? this.TableModel.View.Records[0] : null;
                        if (record != null)
                        {
                            isValid = Syncfusion.Dynamic.DynamicHelper.ValidateProperty((System.Dynamic.IDynamicMetaObjectProvider)record.Data, v.MappingName);
                        }
#endif
                    }

                    if (itemProperties != null && itemProperties.Count > 0 && itemProperties.GetPropertyDescriptor(v.MappingName) != null
                            && !typeof(IComparable).IsAssignableFrom(itemProperties.GetPropertyDescriptor(v.MappingName).PropertyType) && v.ColumnType != null
                            && !v.ColumnType.IsValueType
                            && v.ValueConverter == null)
                    {
                        if (v.SortMemberPath == null)
                        {
                            v.AllowSort = false;
                        }
                        else if (!typeof(IComparable).IsAssignableFrom(itemProperties.GetPropertyDescriptor(v.SortMemberPath).PropertyType))
                        {
                            v.AllowSort = false;
                        }
                    }
                    if (v.SortMemberPath != null && itemProperties != null && itemProperties.Count > 0 && itemProperties.GetPropertyDescriptor(v.SortMemberPath) != null
                            && !typeof(IComparable).IsAssignableFrom(itemProperties.GetPropertyDescriptor(v.SortMemberPath).PropertyType) && v.ColumnType != null
                            && !v.ColumnType.IsValueType
                            && v.ValueConverter == null)
                    {
                        v.AllowSort = false;
                    }

                    if (!isValid && itemProperties != null && itemProperties.Count > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnFiltersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.isInSuspend || this.TableModel == null)
            {
                return;
            }
            bool ClearFilter = true;
            foreach (var col in this)
            {
                if (col.Filters.Count != 0)
                {
                    ClearFilter = false;
                    break;
                }
            }
            if (ClearFilter)
            {
                if (this.TableModel.View != null)
                    this.TableModel.View.RefreshFilters();
                     
            }
            this.ApplyFilter(e.Action == NotifyCollectionChangedAction.Reset);
        }

        public void ApplyFilter(bool fullRefresh)
        {
            var filters = this.TableProperties.VisibleColumns.OfType<IFilterDefinition, GridDataVisibleColumn>();
            if (filters.Count > 0 && this.TableModel.View != null)
            {
                this.TableModel.View.FilterPredicates = filters;

                this.TableModel.RefreshDisplay(true);

                if (!fullRefresh)
                {
                    var startIdx = this.TableModel.ResolveStartIndexBasedOnPosition();
                    this.TableModel.InvalidateCell(GridRangeInfo.Rows(startIdx, this.TableModel.RowCount));
                }
                else
                {
                    foreach (var visibleCol in this.Where(v => v.IsAdvancedFilteringMode))
                    {
                        // clear all internal references
                        visibleCol.FilterPane.Dispose();
                    }
                    if(this.TableProperties.ShowFilterBar)
                        this.TableModel.InvalidateDisplay();
                }
            }
        }

        internal void WireVisibleColumnDescriptor(GridDataVisibleColumn column)
        {
            column.Filters.CollectionChanged -= OnFiltersCollectionChanged;
            column.Filters.CollectionChanged += OnFiltersCollectionChanged;
            column.WidthChanged += this.OnColumnWidthChanged;
            column.CellItemTemplateChanged += this.OnColumnCellTemplateChanged;
            column.CellEditTemplateChanged += this.OnColumnCellTemplateChanged;
            column.IsReadOnlyChanged += this.OnColumnIsReadOnlyChanged;
            column.HeaderCellTemplateChanged += this.OnColumnHeaderCellTemplateChanged;
            column.HeaderTextChanged += this.OnColumnHeaderTextChanged;
            column.SetTableModel(this.TableModel);
        }

        internal void UnwireVisibleColumnDescriptor(GridDataVisibleColumn column)
        {
            column.Filters.CollectionChanged -= OnFiltersCollectionChanged;
            column.WidthChanged -= this.OnColumnWidthChanged;
            column.CellItemTemplateChanged -= this.OnColumnCellTemplateChanged;
            column.CellEditTemplateChanged -= this.OnColumnCellTemplateChanged;
            column.IsReadOnlyChanged -= this.OnColumnIsReadOnlyChanged;
            column.HeaderCellTemplateChanged -= this.OnColumnHeaderCellTemplateChanged;
            column.HeaderTextChanged -= this.OnColumnHeaderTextChanged;
        }

        #region internal event methods and helpers

        private void RefreshColumn(int idx)
        {
            var colIdx = this.TableModel.ResolveVisibleColumnIndexToPosition(idx);
            if (this.Count > 0)
            {
                // refresh the whole column
                this.TableModel.InvalidateCell(GridRangeInfo.Col(colIdx));
#if SILVERLIGHT
                this.TableModel.InvalidateVisual(true);
#endif
            }
        }

        private void OnColumnCellTemplateChanged(object sender, GridDataValueEventArgs<System.Windows.DataTemplate> e)
        {
            if (this.TableModel == null || this.IsInSuspend)
            {
                return;
            }

            var colIdx = this.IndexOf((GridDataVisibleColumn)sender);
            this.RefreshColumn(colIdx);
        }

        private void OnColumnHeaderCellTemplateChanged(object sender, GridDataValueEventArgs<DataTemplate> e)
        {
          
            if (this.TableModel == null || this.IsInSuspend)
            {
                return;
            }

            var colIdx = this.IndexOf((GridDataVisibleColumn)sender);          
            this.RefreshColumn(colIdx);
        }

        private void OnColumnHeaderTextChanged(object sender, GridDataValueEventArgs<string> e)
        {
            if (this.TableModel == null || this.IsInSuspend)
            {
                return;
            }           
            var idx = this.IndexOf((GridDataVisibleColumn)sender);          
            this.RefreshColumn(idx);          
        }

        private void OnColumnIsReadOnlyChanged(object sender, GridDataValueEventArgs<bool> e)
        {
            var colIdx = this.IndexOf((GridDataVisibleColumn)sender);
            this.RefreshColumn(colIdx);
        }

        private void OnColumnWidthChanged(object sender, GridDataValueEventArgs<GridDataControlLength> e)
        {

            if (e.Handled || this.TableModel == null || this.IsInSuspend)
            {
                return;
            }

            if (this.Count > 0)
            {
                //if (this.TableModel.Sizer != null && this.TableModel.Grid != null)
                //{
                //    this.TableModel.Sizer.SetActualWidth();
                //}

                GridDataVisibleColumn column = sender as GridDataVisibleColumn;
                var colIdx = this.TableModel.ResolveVisibleColumnIndexToPosition(this.IndexOf(column));
                //var colIdx = this.VisibleColumns.IndexOf(column);

                //// adjust colIdx values
                //colIdx = this.ShowRowHeader ? colIdx + 1 : colIdx;
                //colIdx = this.TableModel.Table.HasNestedTables ? colIdx + 1 : colIdx;
                if (colIdx > -1)
                {
                    if (column.MinimumWidth != 0 && column.MaximumWidth != 0)
                    {
                        if (column.MinimumWidth < column.MaximumWidth)
                            this.TableModel.ColumnWidths[colIdx] = column.ActualWidth;// ((GridDataControlBaseImpl)this.TableModel.Grid).GridDataColumnSizer.ApplyColumnSizer(column);
                        else if (column.MinimumWidth > column.MaximumWidth)
                            this.TableModel.ColumnWidths[colIdx] = column.MaximumWidth;
                    }
                    else
                        this.TableModel.ColumnWidths[colIdx] = column.ActualWidth;
                    this.TableModel.InvalidateCell(GridRangeInfo.Col(colIdx));
                }

            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the Visible column details for the DataSource.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [XmlInclude(typeof(GridDataUnboundVisibleColumn))]
    public class GridDataVisibleColumn
#if !SILVERLIGHT
 : Freezable, IFilterDefinition, INotifyPropertyChanged, IDisposable
#else
 : DependencyObject, IFilterDefinition, INotifyPropertyChanged
#endif
    {

        private Binding _VisibleColumnBinding;

        [XmlIgnore]
        public Binding Binding
        {
            get { return _VisibleColumnBinding; }
            set
            {
                _VisibleColumnBinding = value;
                ColumnWrapper.SetValueBinding(_VisibleColumnBinding);
            }
        }

        private GridDataVisibleColumnWrapper _ColumnWrapper;
        internal GridDataVisibleColumnWrapper ColumnWrapper
        {
            get
            {
                if (_ColumnWrapper == null)
                    _ColumnWrapper = new GridDataVisibleColumnWrapper();
                return _ColumnWrapper;
            }
        }

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var column = d as GridDataVisibleColumn;
            if (column.TableModel != null)
            {
                var prevHeaderStyle = args.OldValue as GridDataColumnStyle;
                if (prevHeaderStyle != null)
                {
                    prevHeaderStyle.Changed -= new Syncfusion.Windows.Styles.StyleChangedEventHandler(OnHeaderStyleChanged);
                }

                var colIdx = column.TableModel.TableProperties.VisibleColumns.IndexOf(column);
                colIdx = column.TableModel.ResolveVisibleColumnIndexToPosition(colIdx);
                var rowIdx = column.TableModel.TableProperties.StackedHeaderRows.Count;
                if (column.HeaderStyle != null)
                {
                    column.HeaderStyle.CellIdentity = new GridDataTableStyleInfoIdentity(null, rowIdx, colIdx);
                    column.HeaderStyle.CellIdentity.Column = column;
                    column.HeaderStyle.Changed += new Syncfusion.Windows.Styles.StyleChangedEventHandler(OnHeaderStyleChanged);
                }

                column.TableModel.InvalidateCell(GridRangeInfo.Cell(rowIdx, colIdx));
            }
        }

        static void OnHeaderStyleChanged(object sender, Syncfusion.Windows.Styles.StyleChangedEventArgs e)
        {
            var style = sender as GridDataStyleInfo;
            var column = style.CellIdentity.Column;
            if (column != null && column.TableModel != null && column.TableModel.Grid != null)
            {
                column.TableModel.Grid.InvalidateCells();
            }
        }

        public virtual GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion



        //private static readonly DependencyProperty ExcelLikeFilterPaneProperty = DependencyProperty.Register(
        // "ExcelLikeFilterPane",
        // typeof(GridDataExcelLikeFilterPane),
        // typeof(GridDataVisibleColumn),
        // new PropertyMetadata(null));

        private GridDataExcelLikeFilterPane _excelLikeFilterPane;
        [XmlIgnore]
        public GridDataExcelLikeFilterPane ExcelLikeFilterPane
        {
            get
            {
                return _excelLikeFilterPane;
            }

            set
            {
                _excelLikeFilterPane = value;
            }
        }


        private static readonly DependencyProperty ExcelLikeFilterPaneStyleProperty = DependencyProperty.Register(
          "ExcelLikeFilterPaneStyle",
          typeof(Style),
          typeof(GridDataVisibleColumn),
          new PropertyMetadata(null));


        [XmlIgnore]
        public  Style ExcelLikeFilterPaneStyle
        {
            get
            {
                return (Style)this.GetValue(GridDataVisibleColumn.ExcelLikeFilterPaneStyleProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.ExcelLikeFilterPaneStyleProperty, value);
            }
        }




        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public virtual GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            if (DesignerProperties.IsInDesignTool)
            {
                return;
            }
#endif
            var column = d as GridDataVisibleColumn;
            var prevColumn = args.OldValue as GridDataColumnStyle;
            if (prevColumn != null)
            {
                prevColumn.Changed -= new Syncfusion.Windows.Styles.StyleChangedEventHandler(OnColumnStyleChanged);
            }

            if (column.TableModel != null)
            {
                var colIdx = column.TableModel.TableProperties.VisibleColumns.IndexOf(column);
                colIdx = column.TableModel.ResolveVisibleColumnIndexToPosition(colIdx);
                if (column.ColumnStyle != null)
                {
                    column.ColumnStyle.CellIdentity = new GridDataTableStyleInfoIdentity(null, -1, colIdx);
                    column.ColumnStyle.CellIdentity.Column = column;
                    // wire the events after init
                    column.ColumnStyle.Changed += new Syncfusion.Windows.Styles.StyleChangedEventHandler(OnColumnStyleChanged);
                    column.ColumnStyle.ReadOnly = column.IsReadOnly;
                }

                column.TableModel.InvalidateCell(GridRangeInfo.Col(colIdx));
#if SILVERLIGHT
                column.TableModel.InvalidateVisual(true);
#endif
            }
        }

        static void OnColumnStyleChanged(object sender, Syncfusion.Windows.Styles.StyleChangedEventArgs e)
        {
            var style = sender as GridDataStyleInfo;
            var column = style.CellIdentity.Column;
            if (column != null && column.TableModel != null && column.TableModel.TableProperties != null)
            {
                var colIdx = column.TableModel.TableProperties.VisibleColumns.IndexOf(column);
                column.TableModel.InvalidateCell(GridRangeInfo.Col(column.TableModel.ResolveVisibleColumnIndexToPosition(colIdx)));
                column.TableModel.InvalidateVisual(true);
            }
        }

        #endregion

        #region FilterBehavior

        /// <summary>
        /// FilterBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterBehaviorProperty =
            DependencyProperty.Register("FilterBehavior", typeof(FilterBehavior), typeof(GridDataVisibleColumn),
                new PropertyMetadata(FilterBehavior.StronglyTyped));


        /// <summary>
        /// Gets or sets the FilterBehavior property. This dependency property 
        /// indicates ....
        /// </summary>
        public FilterBehavior FilterBehavior
        {
            get { return (FilterBehavior)GetValue(FilterBehaviorProperty); }
            set { SetValue(FilterBehaviorProperty, value); }
        }


        #endregion

#if !SILVERLIGHT
        [XmlIgnore]
        public FilterPanePosition FilterPanePosition
        {
            get { return (FilterPanePosition)GetValue(FilterPanePositionProperty); }
            set { SetValue(FilterPanePositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterPanePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterPanePositionProperty =
            DependencyProperty.Register("FilterPanePosition", typeof(FilterPanePosition), typeof(GridDataVisibleColumn), new PropertyMetadata(null));

#endif

        /// <summary>
        /// Dependency property for FilterBarMode.
        /// </summary>
        public static readonly DependencyProperty FilterBarModeProperty =
            DependencyProperty.Register("FilterBarMode", typeof(GridDataFilterBarMode), typeof(GridDataVisibleColumn), new PropertyMetadata(GridDataFilterBarMode.Immediate));

        /// <summary>
        /// Dependency property for AllowFilters.
        /// </summary>
        public static readonly DependencyProperty AllowFilterProperty = DependencyProperty.Register(
            "AllowFilter",
            typeof(bool),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(false, OnAllowFilterChanged));

        /// <summary>
        /// Dependency property for AllowSort.
        /// </summary>
        public static readonly DependencyProperty AllowSortProperty = DependencyProperty.Register(
           "AllowSort",
           typeof(bool),
           typeof(GridDataVisibleColumn),
           new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for AllowDrag.
        /// </summary>
        public static readonly DependencyProperty AllowDragProperty = DependencyProperty.Register(
           "AllowDrag",
           typeof(bool),
           typeof(GridDataVisibleColumn),
           new PropertyMetadata(false, OnAllowDragChanged));

        /// <summary>
        /// Dependency property for AllowGroup.
        /// </summary>
        public static readonly DependencyProperty AllowGroupProperty = DependencyProperty.Register(
           "AllowGroup",
           typeof(bool),
           typeof(GridDataVisibleColumn),
            new PropertyMetadata(true, OnAllowGroupChanged));

        /// <summary>
        /// Dependency property for AllowResize.
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.Register(
           "AllowResize",
           typeof(bool),
           typeof(GridDataVisibleColumn),
#if !SILVERLIGHT
 new PropertyMetadata(true, OnAllowResizeChanged, CoerceAllowResize));
#else
 new PropertyMetadata(true, OnAllowResizeChanged));
#endif

        /// <summary>
        /// Dependency property for AutoFit.
        /// </summary>
        public static readonly DependencyProperty AutoFitProperty = DependencyProperty.Register(
           "AutoFit",
           typeof(bool),
           typeof(GridDataVisibleColumn),
           new PropertyMetadata(false, new PropertyChangedCallback(OnAutoFitChanged)));

        /// <summary>
        /// Dependency property for HeaderCellTemplate.
        /// </summary>
        public static readonly DependencyProperty HeaderCellTemplateProperty = DependencyProperty.Register(
            "HeaderCellTemplate",
            typeof(DataTemplate),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnHeaderCellTemplatePropertyChanged));

        /// <summary>
        /// Dependency property for HeaderText.
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText",
            typeof(string),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnHeaderTextChanged));

        /// <summary>
        /// Dependency property for IncrementSeed.
        /// </summary>
        public static readonly DependencyProperty IncrementSeedProperty = DependencyProperty.Register(
            "IncrementSeed",
            typeof(int),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(1));

        /// <summary>
        /// Dependency property for IsIdentity.
        /// </summary>
        public static readonly DependencyProperty IsIdentityProperty = DependencyProperty.Register(
            "IsIdentity",
            typeof(bool),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(false));

        /// <summary>
        /// Dependency property for IsReadOnly.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnIsReadOnlyChanged));

        /// <summary>
        /// Dependency property for MappingName.
        /// </summary>
        public static readonly DependencyProperty MappingNameProperty = DependencyProperty.Register(
            "MappingName",
            typeof(string),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnMappingNameChanged));

        /// <summary>
        /// Dependency property for Width.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(GridDataControlLength),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(GridDataControlLength.None, OnWidthChanged));

        /// <summary>
        /// Dependency property for Width.
        /// </summary>
        public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register(
            "MinimumWidth",
            typeof(double),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(0d));

        public static readonly DependencyProperty MaximumWidthProperty = DependencyProperty.Register(
            "MaximumWidth",
            typeof(double),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(0d));

        public static readonly DependencyProperty ReferenceFieldsProperty = DependencyProperty.Register(
          "ReferenceFields",
          typeof(string),
          typeof(GridDataVisibleColumn),new PropertyMetadata(string.Empty));

        public string ReferenceFields
        {
            get
            {
                return (string)this.GetValue(GridDataVisibleColumn.ReferenceFieldsProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.ReferenceFieldsProperty, value);
            }
        }

        internal bool IsLengthUnitTypeChanged
        {
            get;
            set;
        }

        #region ShowAdvanceFilteringOptioninExcelLikeFiltering

        public static readonly DependencyProperty ShowAdvanceFilteringOptioninExcelLikeFilteringProperty =
           DependencyProperty.Register("ShowAdvanceFilteringOptioninExcelLikeFiltering", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(true));

        public bool ShowAdvanceFilteringOptioninExcelLikeFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.ShowAdvanceFilteringOptioninExcelLikeFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataVisibleColumn.ShowAdvanceFilteringOptioninExcelLikeFilteringProperty, value);
            }
        }

        /// <summary>
        /// Enables / disables default formatting based on CellType
        /// </summary>
        public bool CanFormatExcelLikeFilterText
        {
            get { return (bool)GetValue(CanFormatExcelLikeFilterTextProperty); }
            set { SetValue(CanFormatExcelLikeFilterTextProperty, value); }
        }

        public static readonly DependencyProperty CanFormatExcelLikeFilterTextProperty =
            DependencyProperty.Register("CanFormatExcelLikeFilterText", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(false));
          

        #endregion

        #region ShowSearchOptioninExcelLikeFiltering

        public static readonly DependencyProperty ShowSearchOptioninExcelLikeFilteringProperty =
           DependencyProperty.Register("ShowSearchOptioninExcelLikeFiltering", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(true));

        public bool ShowSearchOptioninExcelLikeFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.ShowSearchOptioninExcelLikeFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataVisibleColumn.ShowSearchOptioninExcelLikeFilteringProperty, value);
            }
        }

       

        #endregion

        #region ShowSortOptioninExcelLikeFiltering

        public static readonly DependencyProperty ShowSortOptioninExcelLikeFilteringProperty =
           DependencyProperty.Register("ShowSortOptioninExcelLikeFiltering", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(true));

        public bool ShowSortOptioninExcelLikeFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.ShowSortOptioninExcelLikeFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataVisibleColumn.ShowSortOptioninExcelLikeFilteringProperty, value);
            }
        }       

        #endregion

        [XmlIgnore]
        public double ActualWidth
        {
            get
            {
                // this if condition uses in GridDataColumnSizer to assign the columnwidth to ActualWidth in case of UnitType as None.
                if (this.Width.UnitType == GridControlLengthUnitType.None)
                {
                        // Checking Minimum and Maximum width limits
                        if (this.Width.Value < this.MinimumWidth && this.MinimumWidth != 0)
                            this.Width = new GridDataControlLength(this.MinimumWidth);
                        if (this.Width.Value > this.MaximumWidth && this.MaximumWidth != 0)
                            this.Width = new GridDataControlLength(this.MaximumWidth);
                    return this.Width.Value;
                }
                if (this.Width.UnitType == GridControlLengthUnitType.None)
                {
                    // Checking Minimum and Maximum width limits
                    if (this.actualWidth < this.MinimumWidth && this.MinimumWidth != 0)
                        this.actualWidth = this.MinimumWidth;
                    if (this.actualWidth > this.MaximumWidth && this.MaximumWidth != 0)
                        this.actualWidth = this.MaximumWidth;
                }
                return this.actualWidth;
            }

            internal set
            {
                if (this.Width.UnitType == GridControlLengthUnitType.None)
                {
                    // Checking Minimum and Maximum width limits while assigning values
                    if (value < this.MinimumWidth && this.MinimumWidth != 0)
                        value = this.MinimumWidth;
                    if (value > this.MaximumWidth && this.MaximumWidth != 0)
                        value = this.MaximumWidth;
                }
                if (this.actualWidth == value)
                {
                    return;
                }
                this.actualWidth = value;

                if (this.TableModel != null && !this.TableModel.IsInColumnRefresh && !this.TableModel.TableProperties.VisibleColumns.IsInSuspend && !this.IsHidden)
                {
                    var colIdx = this.TableModel.ResolveVisibleColumnIndexToPosition(this.TableModel.TableProperties.VisibleColumns.IndexOf(this));
                    if (colIdx > -1 && this.TableModel.ColumnWidths[colIdx] != value)
                    {
                        this.TableModel.ColumnWidths[colIdx] = value;
                        this.TableModel.InvalidateCell(GridRangeInfo.Col(colIdx));
                    }
                    this.isActualWidthSet = true;
                }
            }
        }

        internal bool isActualWidthSet = false;

        private double actualWidth = GridDataVisibleColumn.DefaultWidth;

        public static readonly double MinWidth = 20d;
        public static readonly double DefaultWidth = 150d;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleColumnDescriptor"/> class.
        /// </summary>
        public GridDataVisibleColumn()
        {
            this.IsInSuspend = false;
        }

        
        

        public event EventHandler<GridDataValueEventArgs<DataTemplate>> HeaderCellTemplateChanged;

        public event EventHandler<GridDataValueEventArgs<string>> HeaderTextChanged;

        public event EventHandler<GridDataValueEventArgs<bool>> IsReadOnlyChanged;

        public event EventHandler<GridDataValueEventArgs<string>> MappingNameChanged;

        public event EventHandler<GridDataValueEventArgs<GridDataControlLength>> WidthChanged;

        public bool IsAdvancedFilteringMode
        {
            get
            {
                return this.FilterPane != null;
            }
        }

        public static readonly DependencyProperty FilterPaneProperty = DependencyProperty.Register(
            "FilterPane",
            typeof(GridDataFilteringPane),
            typeof(GridDataVisibleColumn),
#if !SILVERLIGHT
 new PropertyMetadata(null, null, CoerceFilterPaneProperty));
#else
 new PropertyMetadata(null));
#endif

#if !SILVERLIGHT
        private static object CoerceFilterPaneProperty(DependencyObject d, object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (type == typeof(GridDataFilteringPane))
                {
                    return new GridDataTextFilteringPane();
                }
            }

            return value;
        }
#endif

        [XmlIgnore]
        public GridDataFilteringPane FilterPane
        {
            get
            {
                return (GridDataFilteringPane)this.GetValue(GridDataVisibleColumn.FilterPaneProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.FilterPaneProperty, value);
            }
        }
 
         public static readonly DependencyProperty FilterBarStyleProperty = DependencyProperty.Register(
            "DropDownFilterBar",
            typeof(GridDataFilterBarStyle),
            typeof(GridDataVisibleColumn), new PropertyMetadata(null));
       
         [XmlIgnore]
         public GridDataFilterBarStyle FilterBarStyle
         {
             get
             {
                 return (GridDataFilterBarStyle)this.GetValue(GridDataVisibleColumn.FilterBarStyleProperty);
             }

             set
             {
                 this.SetValue(GridDataVisibleColumn.FilterBarStyleProperty, value);
             }
         }


        public GridDataFilterBarMode FilterBarMode
        {
            get { return (GridDataFilterBarMode)GetValue(GridDataVisibleColumn.FilterBarModeProperty); }
            set { SetValue(GridDataVisibleColumn.FilterBarModeProperty, value); }
        }

        public static readonly DependencyProperty IsCaseSensitiveFilterProperty = DependencyProperty.Register(
            "IsCaseSensitiveFilter",
            typeof (bool),
            typeof (GridDataVisibleColumn), new PropertyMetadata(false));

        public bool IsCaseSensitiveFilter
        {
            get { return (bool)GetValue(GridDataVisibleColumn.IsCaseSensitiveFilterProperty); }
            set { SetValue(GridDataVisibleColumn.IsCaseSensitiveFilterProperty, value); }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether [allow filter].
        /// </summary>
        public bool AllowFilter
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AllowFilterProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AllowFilterProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether [allow sort].
        /// </summary>
        public bool AllowSort
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AllowSortProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AllowSortProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether [allow drag].
        /// </summary>
        public bool AllowDrag
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AllowDragProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AllowDragProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether [allow group].
        /// </summary>
        public bool AllowGroup
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AllowGroupProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AllowGroupProperty, value);
            }
        }

        private static void OnAllowGroupChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var column = dpo as GridDataVisibleColumn;
            column.RaisePropertyChanged("AllowGroup");
        }

        /// <summary>
        ///  Gets or sets a value indicating whether [allow resize].
        /// </summary>
        public bool AllowResize
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AllowResizeProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AllowResizeProperty, value);
            }
        }

        /// <summary>
        /// When true, the column will be resized automatically to fit the content. This will also include the header cells.
        /// </summary>
        [Obsolete("This property is depreciated, Use Width = new GridDataControlLength(1d, GridDataControlLengthType.Auto)")]
        public bool AutoFit
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.AutoFitProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.AutoFitProperty, value);
            }
        }

        #region ValueConverterParameter (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ValueConverterParameter property. 
        /// </summary>
        public object ValueConverterParameter
        {
            get { return (object)GetValue(ValueConverterParameterProperty); }
            set { SetValue(ValueConverterParameterProperty, value); }
        }
        public static readonly DependencyProperty ValueConverterParameterProperty = DependencyProperty.Register("ValueConverterParameter", typeof(object), typeof(GridDataVisibleColumn), new PropertyMetadata(null));

        #endregion

        #region ValueConverter (DependencyProperty)

        /// <summary>
        /// To store the value converter function of appropriate column
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        internal Func<string, object, object> ValueConverterFunc = null;

        /// <summary>
        /// Gets / Sets the ValueConverter property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public IValueConverter ValueConverter
        {
            get { return (IValueConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }

        public static readonly DependencyProperty ValueConverterProperty = DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(GridDataVisibleColumn), new PropertyMetadata(null));

        #endregion

        public static readonly DependencyProperty ShowColumnOptionsProperty = DependencyProperty.Register(
            "ShowColumnOptions",
            typeof(bool),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(false, OnShowColumnOptionsChanged));

        private static void OnShowColumnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var visibleColumn = d as GridDataVisibleColumn;
            if (visibleColumn.TableModel == null)
            {
                return;
            }

            var tableModel = visibleColumn.TableModel;
            var colIndex = tableModel.ResolveVisibleColumnIndexToPosition(tableModel.TableProperties.VisibleColumns.IndexOf(visibleColumn)); //visibleColumn.GetColumnIndexOnGrid();
            var rowIndex = visibleColumn.TableModel.TableProperties.StackedHeaderRows.Count;
            visibleColumn.TableModel.InvalidateCell(GridRangeInfo.Cell(rowIndex, colIndex));
        }

        /// <summary>
        /// Gets or Sets the ShowColumnOptions feature for this column.
        /// </summary>
        public bool ShowColumnOptions
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.ShowColumnOptionsProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.ShowColumnOptionsProperty, value);
            }
        }

        [XmlIgnore]
        private Type columnType = null;
        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        [XmlIgnore]
        public Type ColumnType
        {
            internal set
            {
                _VisibleColumnType = value;
            }
            get
            {
                if (this.TableModel != null)
                {
                    if (!this.IsUnbound)
                    {
                        if (this.TableModel.View != null)
                        {
                            // Get Complex property's PropertyDescriptor
                            var pdc = this.TableModel.View.GetItemProperties();
                            if (pdc != null)
                            {
#if SyncfusionFramework4_0
                                if (!this.TableModel.Table.IsDynamicBound)
                                {
#endif
                                    if (this._VisibleColumnType == null)
                                    {
                                        var pd = pdc.GetPropertyDescriptor(this.MappingName);
                                        if (pd == null)
                                           throw new InvalidOperationException(string.Format("{0} - {1}", GridDataResourceWrapper.InvalidColumn, this.MappingName));
                                        if (NullableHelper.IsNullableType(pd.PropertyType))
                                        {
                                            this.columnType = NullableHelper.GetUnderlyingType(pd.PropertyType);
                                        }
                                        else
                                        {
                                            this.columnType = pd.PropertyType;
                                        }
                                    }
                                    else
                                    {
                                        this.columnType = this._VisibleColumnType;
                                    }
#if SyncfusionFramework4_0
                                }
                                else
                                {
                                    if (this.VisibleColumnType == null)
                                        this.columnType = this.GetDynamicColumnType();
                                    else
                                        this.columnType = this.VisibleColumnType;
                                }
#endif
                            }
                            else
                            {
                                throw new InvalidOperationException("Type not found in this column");
                            }
                        }
                    }
                    else
                    {
                        var type = this.TableModel.GetUnboundType(this.MappingName);
                        if (type != null)
                        {
                            this.columnType = type;
                        }
                        else
                        {
                            this.columnType = typeof(string);
                        }
                    }
                }

                return this.columnType;
            }
        }

        private bool IsDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(this);
        }

        [XmlIgnore]
        private Type _VisibleColumnType;

        [XmlIgnore]
        public Type VisibleColumnType
        {
            get 
            {
                if (IsDesignTime())
                    return this.ColumnType;
                else
                    return _VisibleColumnType;
            }
            set
            {
                _VisibleColumnType = value;
                ColumnType = _VisibleColumnType;
            }
        }

        #region DataTemplate


        public event EventHandler<GridDataValueEventArgs<DataTemplate>> CellItemTemplateChanged;

        public event EventHandler<GridDataValueEventArgs<DataTemplate>> CellEditTemplateChanged;

        /// <summary>
        /// Dependency property for CellTemplate.
        /// </summary>
        public static readonly DependencyProperty CellEditItemTemplateProperty = DependencyProperty.Register(
            "CellEditItemTemplate",
            typeof(DataTemplate),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnCellEditItemTemplateChanged));

        private static void OnCellEditItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            if (column != null)
            {
                column.OnCellEditItemTemplateChanged((DataTemplate)args.NewValue);
            }

            column.RaisePropertyChanged("CellEditItemTemplate");
        }

        private void OnCellEditItemTemplateChanged(DataTemplate dataTemplate)
        {
            if (this.CellEditTemplateChanged != null)
            {
                this.CellEditTemplateChanged(this, new GridDataValueEventArgs<DataTemplate>(dataTemplate));
            }
        }

        /// <summary>
        /// Gets or sets the cell template for each column.
        /// </summary>
        /// <value>The cell template.</value>
        [XmlIgnore]
        public DataTemplate CellEditItemTemplate
        {
            get
            {
                return this.GetValue(GridDataVisibleColumn.CellEditItemTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.CellEditItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for CellTemplate.
        /// </summary>
        public static readonly DependencyProperty CellItemTemplateProperty = DependencyProperty.Register(
            "CellItemTemplate",
            typeof(DataTemplate),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnCellItemTemplateChanged));

        private static void OnCellItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            if (column != null)
            {
                column.OnCellItemTemplateChanged((DataTemplate)args.NewValue);
            }

            column.RaisePropertyChanged("CellItemTemplate");
        }

        /// <summary>
        /// Gets or sets the cell template for each column.
        /// </summary>
        /// <value>The cell template.</value>
        [XmlIgnore]
        public DataTemplate CellItemTemplate
        {
            get
            {
                return this.GetValue(GridDataVisibleColumn.CellItemTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.CellItemTemplateProperty, value);
            }
        }

        #endregion

        [XmlIgnore]
        private ObservableCollection<FilterPredicate> filters;
        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>The filters.</value>
        [XmlIgnore]
        public ObservableCollection<FilterPredicate> Filters
        {
            get
            {
                if (this.filters == null)
                {
                    this.filters = new ObservableCollection<FilterPredicate>();
                }
                return this.filters;
            }
        }

        /// <summary>
        /// Gets or sets the header cell data template. Easily customize the display of the
        /// header cell thru normal WPF bound controls. Also, we can apply WPF animation with properties
        /// from GridStyleInfo that is passed to the Content of the DataTemplate.
        /// <para></para>
        /// <code lang="XAML">
        /// &lt;syncfusion:GridDataVisibleColumn
        /// MappingName=&quot;OrderID&quot; HeaderText=&quot;Order ID&quot;
        /// Width=&quot;90&quot;&gt;
        ///                     &lt;syncfusion:GridDataVisibleColumn.HeaderCellTemplate&gt;
        ///                         &lt;DataTemplate&gt;
        ///                             &lt;TextBlock Text=&quot;{Binding
        /// Path=CellBoundValue}&quot;
        /// syncfusion:VisualContainer.WantsMouseInput=&quot;False&quot;
        /// Background=&quot;Blue&quot; Foreground=&quot;Yellow&quot; /&gt;
        ///                         &lt;/DataTemplate&gt;
        ///                     &lt;/syncfusion:GridDataVisibleColumn.HeaderCellTemplate&gt;
        ///                 &lt;/syncfusion:GridDataVisibleColumn&gt;
        /// </code>
        /// </summary>
        [XmlIgnore]
        public DataTemplate HeaderCellTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridDataVisibleColumn.HeaderCellTemplateProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.HeaderCellTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            get { return this.GetValue(GridDataVisibleColumn.HeaderTextProperty) as string; }
            set { this.SetValue(GridDataVisibleColumn.HeaderTextProperty, value); }
        }

        public int IncrementSeed
        {
            get
            {
                return (int)this.GetValue(GridDataVisibleColumn.IncrementSeedProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.IncrementSeedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is identity.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is identity; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.IsIdentityProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.IsIdentityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the mapping.
        /// </summary>
        /// <value>The name of the mapping.</value>
        public string MappingName
        {
            get
            {
                return (string)this.GetValue(GridDataVisibleColumn.MappingNameProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.MappingNameProperty, value);
            }
        }

        public double MinimumWidth
        {
            get
            {
                return (double)this.GetValue(GridDataVisibleColumn.MinimumWidthProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.MinimumWidthProperty, value);
            }
        }

        public double MaximumWidth
        {
            get
            {
                return (double)this.GetValue(GridDataVisibleColumn.MaximumWidthProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.MaximumWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public GridDataControlLength Width
        {
            get
            {
                return (GridDataControlLength)this.GetValue(GridDataVisibleColumn.WidthProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.WidthProperty, value);
            }
        }

        public static readonly DependencyProperty IsUnboundProperty = DependencyProperty.Register(
            "IsUnbound",
            typeof(bool),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this column is unbound. This also indicates that the column's cell value is not bound to the underlying data source. It has to be manually populated or the GridDataUnboundVisibleColumn.UnboundFormat can be set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is unbound; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsUnbound
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.IsUnboundProperty);
            }

            protected set
            {
                this.SetValue(GridDataVisibleColumn.IsUnboundProperty, value);
            }
        }

#if SyncfusionFramework4_0

        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(GridDataVisibleColumn), new PropertyMetadata(DataType.String));

        public DataType DataType
        {
            get
            {
                return (DataType)this.GetValue(GridDataVisibleColumn.DataTypeProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.DataTypeProperty, value);
            }
        }

        internal Type GetDynamicColumnType()
        {
            Type dynColumnType = null;
            switch (this.DataType)
            {
                case DataType.Boolean:
                    dynColumnType = typeof(bool);
                    break;
                case DataType.DateTime:
                    dynColumnType = typeof(DateTime);
                    break;
                case DataType.Decimal:
                    dynColumnType = typeof(Decimal);
                    break;
                case DataType.Double:
                    dynColumnType = typeof(double);
                    break;
                case DataType.Float:
                    dynColumnType = typeof(float);
                    break;
                case DataType.Int16:
                    dynColumnType = typeof(Int16);
                    break;
                case DataType.Int32:
                    dynColumnType = typeof(Int32);
                    break;
                case DataType.Int64:
                    dynColumnType = typeof(Int64);
                    break;
                case DataType.NullableBoolean:
                    dynColumnType = typeof(Nullable<bool>);
                    break;
                case DataType.NullableDateTime:
                    dynColumnType = typeof(Nullable<DateTime>);
                    break;
                case DataType.NullableDecimal:
                    dynColumnType = typeof(Nullable<Decimal>);
                    break;
                case DataType.NullableDouble:
                    dynColumnType = typeof(Nullable<Double>);
                    break;
                case DataType.NullableFloat:
                    dynColumnType = typeof(Nullable<float>);
                    break;
                case DataType.NullableInt16:
                    dynColumnType = typeof(Nullable<Int16>);
                    break;
                case DataType.NullableInt32:
                    dynColumnType = typeof(Nullable<Int32>);
                    break;
                case DataType.NullableInt64:
                    dynColumnType = typeof(Nullable<Int64>);
                    break;
                default:
                    dynColumnType = typeof(string);
                    break;
            }

            return dynColumnType;
        }

#endif
        public static readonly DependencyProperty SortMemberPathProperty = DependencyProperty.Register(
            "SortMemberPath",
            typeof(string),
            typeof(GridDataVisibleColumn),
            new PropertyMetadata(OnSortMemberPathChanged));

        private static void OnSortMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var column = d as GridDataVisibleColumn;
            if (column.TableModel != null)
            {
                column.TableModel.SortColumn(column);
            }
        }

        public string SortMemberPath
        {
            get
            {
                return (string)this.GetValue(GridDataVisibleColumn.SortMemberPathProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.SortMemberPathProperty, value);
            }
        }

        #region UpdateMode
        public UpdateMode UpdateMode
        {
            get { return (UpdateMode)GetValue(UpdateModeProperty); }
            set { SetValue(UpdateModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateModeProperty = DependencyProperty.Register("UpdateMode", typeof(UpdateMode), typeof(GridDataVisibleColumn), new PropertyMetadata(UpdateMode.LostFocus));

        #endregion

        //private bool autoFitChangedBeforeModelLoaded = false;
        private static void OnAutoFitChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            var value = (bool)args.NewValue;
            if (value)
            {
                column.Width = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
            }
            else
            {
                column.Width = GridDataControlLength.None;
            }
            //if (column.TableModel == null)
            //{
            //column.autoFitChangedBeforeModelLoaded = true;
            //return;
            //}

            //column.DoAutoFit((bool)args.NewValue);
        }

        [XmlIgnore]
        public bool IsInSuspend
        {
            get;
            internal set;
        }

        //internal void DoAutoFit(bool value)
        //{
        //    if (value)
        //    {
        //        this.Width = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
        //    }
        //    else
        //    {
        //        if (!this.IsInSuspend)
        //        {
        //            this.ActualWidth = this.TableModel.TableProperties.DefaultColumnWidth;
        //        }
        //    }
        //}

        //internal int GetColumnIndexOnGrid()
        //{
        //    int colIndex = this.TableModel.TableProperties.VisibleColumns.IndexOf(this);
        //    colIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(colIndex);
        //    return colIndex;
        //}
        private static void OnAllowFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn Column = d as GridDataVisibleColumn;
            if (Column.TableModel == null || Column.TableModel.TableProperties == null || Column.TableModel.View == null )
                return;
            var allowFiltersVisibleColumn = Column.TableModel.TableProperties.VisibleColumns.FirstOrDefault(column => column.AllowFilter == true);
            var filterpaneColumn = Column.TableModel.TableProperties.VisibleColumns.FirstOrDefault(column => column.FilterPane != null);
            if (allowFiltersVisibleColumn != null && filterpaneColumn == null && !allowFiltersVisibleColumn.IsAdvancedFilteringMode && !Column.TableModel.TableProperties.ShowFilterBar)
                (Column.TableModel.View as IExcelLikeFilterExt).IsExcelLikeFilter = true;
        }

        private static void OnAllowDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            if (column.TableModel == null)
            {
                return;
            }

            var dragController = column.TableModel.Grid.MouseControllerDispatcher.Find(GridDataGroupDragMouseController.MouseControllerName);
            if ((bool)args.NewValue)
            {
                if (dragController == null)
                {
                    column.TableModel.Grid.MouseControllerDispatcher.Add(new GridDataGroupDragMouseController(column.TableModel.Grid));
                }
            }
        }

        private static object CoerceAllowResize(DependencyObject d, object value)
        {
            var column = d as GridDataVisibleColumn;
            if (column.AutoFit)
            {
                return false;
            }

            return value;
        }

        private static void OnAllowResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            if (column.TableModel == null)
            {
                return;
            }

            var resizeColsController = column.TableModel.Grid.MouseControllerDispatcher.Find("ResizeColumnsMouseController");
            if ((bool)args.NewValue)
            {
                if (resizeColsController == null)
                {
                    column.TableModel.Grid.MouseControllerDispatcher.Add(new GridResizeColumnsMouseController(column.TableModel.Grid));
                }
            }
            column.RaisePropertyChanged("AllowResize");
        }

        private void OnCellItemTemplateChanged(DataTemplate value)
        {
            if (this.CellItemTemplateChanged != null)
            {
                this.CellItemTemplateChanged(this, new GridDataValueEventArgs<DataTemplate>(value));
            }
        }

        private void OnHeaderCellTemplateChanged(DataTemplate value)
        {
            if (this.HeaderCellTemplateChanged != null)
            {
                this.HeaderCellTemplateChanged(this, new GridDataValueEventArgs<DataTemplate>(value));
            }
        }

        private static void OnHeaderCellTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var visibleColumn = d as GridDataVisibleColumn;
            visibleColumn.OnHeaderCellTemplateChanged((DataTemplate)args.NewValue);
            visibleColumn.RaisePropertyChanged("HeaderCellTemplate");
        }

        private void OnHeaderTextChanged(string headerText)
        {
            if (this.HeaderTextChanged != null)
            {
                this.HeaderTextChanged(this, new GridDataValueEventArgs<string>(headerText));
            }
        }

        private static void OnHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var column = d as GridDataVisibleColumn;

            if (args.NewValue == null)
                return;

            column.OnHeaderTextChanged(args.NewValue.ToString());

            if (column.TableModel != null)
            {
                var dataGrid = column.TableModel.Grid.FindParentElementOfType<GridDataControl>();
                if (dataGrid != null)
                {
                    if (dataGrid.ShowGroupDropArea && dataGrid.GroupDropAreaGrid != null &&
                        dataGrid.GroupDropAreaGrid.Model != null)
                    {
                        dataGrid.GroupDropAreaGrid.InvalidateCells();
                    }
                    if (column.TableModel.Table.HasGroups)
                    {
                        column.TableModel.View.TopLevelGroup.Invalidate(0,
                                                                        column.TableModel.View.TopLevelGroup.Groups
                                                                              .Count);
                    }
                    if (!column.IsInSuspend)
                    {
                        int headerRowIndex = dataGrid.Model.HeaderRows - 1;
                        dataGrid.Model.Grid.ArrangedCellUIElements.Unload(
                            GridRangeInfo.Row(headerRowIndex).ToCellSpan(dataGrid.Model));
                    }
                    dataGrid.Model.Grid.InvalidateVisual(true);
                }
            }
            column.RaisePropertyChanged("HeaderText");
        }

        private void OnIsReadOnlyChanged(bool value)
        {
            if (this.IsReadOnlyChanged != null)
            {
                this.IsReadOnlyChanged(this, new GridDataValueEventArgs<bool>(value));
            }
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var column = d as GridDataVisibleColumn;
            column.OnIsReadOnlyChanged((bool)args.NewValue);
            column.RaisePropertyChanged("IsReadOnly");
        }

        private static void OnMappingNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            if (args.NewValue != null)
            {
                var mappingName = args.NewValue.ToString();
                column.OnMappingNameChanged(mappingName);
                if (column.HeaderText == string.Empty || column.HeaderText == null)
                {
                    if (column.TableModel != null)
                        column.HeaderText = mappingName;
                }
                column.RaisePropertyChanged("MappingName");
            }
        }

        private void OnMappingNameChanged(string value)
        {
            if (this.MappingNameChanged != null)
            {
                this.MappingNameChanged(this, new GridDataValueEventArgs<string>(value));
            }
        }

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataVisibleColumn column = d as GridDataVisibleColumn;
            column.OnWidthChanged((GridDataControlLength)args.NewValue);
            if (column.TableModel != null && column.Width.UnitType != column.TableModel.TableProperties.ColumnSizer)
            {
                column.TableModel.ColumnAutoSizer.SetWidthAndStar(column);
            }

            column.RaisePropertyChanged("Width");
        }

        private void OnWidthChanged(GridDataControlLength value)
        {
            GridDataControlLength minimumwidth = new GridDataControlLength(this.MinimumWidth);
            GridDataControlLength maximumwidth = new GridDataControlLength(this.MaximumWidth);
            if (value.UnitType == GridControlLengthUnitType.None)
            {
                if (value.Value < minimumwidth.Value)
                    value = minimumwidth;
                if ((value.Value > maximumwidth.Value) && (maximumwidth.Value != 0))
                    value = maximumwidth;
            }
            if (this.WidthChanged != null)
            {
                this.WidthChanged(this, new GridDataValueEventArgs<GridDataControlLength>(value));
            }
        }

        internal GridDataTableModel TableModel
        {
            get;
            private set;
        }

        internal void SetTableModel(GridDataTableModel gridDataTableModel)
        {
            this.TableModel = gridDataTableModel;
            //if (this.autoFitChangedBeforeModelLoaded && this.AutoFit && gridDataTableModel != null)
            //{
            //    this.autoFitChangedBeforeModelLoaded = false;
            //    this.DoAutoFit(this.AutoFit);
            //}
        }

        public virtual void InitializeFrom(GridDataVisibleColumn other)
        {
            this.AllowFilter = other.AllowFilter;
            this.AllowDrag = other.AllowDrag;
            this.AllowGroup = other.AllowGroup;
            this.AllowResize = other.AllowResize;
            this.AllowSort = other.AllowSort;
            this.AutoFit = other.AutoFit;
            this.CellItemTemplate = other.CellItemTemplate;
            this.CellEditItemTemplate = other.CellEditItemTemplate;
            this.ColumnStyle = other.ColumnStyle;
            this.filters = null;
            this.HeaderCellTemplate = other.HeaderCellTemplate;
            this.HeaderText = other.HeaderText;
            this.IncrementSeed = other.IncrementSeed;
            this.IsHidden = other.IsHidden;
            this.IsIdentity = other.IsIdentity;
            this.IsReadOnly = other.IsReadOnly;
            this.IsUnbound = other.IsUnbound;
            this.MappingName = other.MappingName;
            this.SortMemberPath = other.SortMemberPath;
            this.MinimumWidth = other.MinimumWidth;
            this.MaximumWidth = other.MaximumWidth;
            this.Width = other.Width;
            this.ShowColumnOptions = other.ShowColumnOptions;
            //this.ActualWidth = other.ActualWidth;
            this.FilterPane = other.FilterPane;
            this.FilterBehavior = other.FilterBehavior;
            this.ValueConverter = other.ValueConverter;
            this.HeaderStyle = other.HeaderStyle;
            if (other.Filters.Count > 0)
            {
                if (this.filters != null)
                {
                    this.filters.Clear();
                }
                else
                {
                    this.filters = new ObservableCollection<FilterPredicate>();
                }

                this.IsInSuspend = true;
                foreach (var filter in other.Filters)
                {
                    this.filters.Add(filter);
                }
                this.IsInSuspend = false;
            }

        }

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string propertyName)
        {
            var propertyChangedEvent = this.PropertyChanged;
            if (propertyChangedEvent != null)
            {
                propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

#if SILVERLIGHT
        public static readonly DependencyProperty ShowGroupIndicatorProperty = DependencyProperty.Register("ShowGroupIndicator", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(false, OnShowGroupIndicatorChanged));

        private static void OnShowGroupIndicatorChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var column = dpo as GridDataVisibleColumn;
            if (column.TableModel == null)
            {
                return;
            }

            var model = column.TableModel;
            int colIndex = model.TableProperties.VisibleColumns.IndexOf(column);
            model.InvalidateCell(GridRangeInfo.Cell(model.ResolveDefaultColumnOffset(), model.ResolveVisibleColumnIndexToPosition(colIndex)));
        }

        public bool ShowGroupIndicator
        {
            get
            {
                return (bool)this.GetValue(GridDataVisibleColumn.ShowGroupIndicatorProperty);
            }

            set
            {
                this.SetValue(GridDataVisibleColumn.ShowGroupIndicatorProperty, value);
            }
        }
#endif

        #region ColumnHeaderStyle(DependencyProperty)
        [XmlIgnore]
        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        public static readonly DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register("ColumnHeaderStyle", typeof(Style), typeof(GridDataVisibleColumn), new PropertyMetadata(null));

        #endregion

        #region IsHidden (DependencyProperty)

        /// <summary>
        /// Gets / Sets if the VisibleColumn is hidden
        /// </summary>
        public bool IsHidden
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }

        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(GridDataVisibleColumn), new PropertyMetadata(false, OnIsHiddenChanged));

        private static void OnIsHiddenChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var column = dpo as GridDataVisibleColumn;
            if (column.TableModel != null && !column.IsInSuspend)
            {
                column.TableModel.Table.RefreshHiddenColumns();
                if (!column.IsHidden)
                {
                    column.TableModel.ColumnAutoSizer.SetWidthAndStar(column);
                }
                else
                {
                    column.TableModel.ColumnAutoSizer.SetStarWidth();
                }     
            }
        }

        #endregion


#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif

        public void Dispose()
        {
            if (this.filters != null)
            {
                this.filters.Clear();
            }

            this.filters = null;
            if (this.ExcelLikeFilterPane != null)
            {
                this.ExcelLikeFilterPane.Dispose();
                this.ExcelLikeFilterPane = null;
            }
        }
    }


#if SyncfusionFramework4_0
    public enum DataType
    {
        Int16,
        Int32,
        Int64,
        Double,
        Decimal,
        Float,
        String,
        Boolean,
        DateTime,
        NullableInt16,
        NullableInt32,
        NullableInt64,
        NullableDouble,
        NullableDecimal,
        NullableFloat,
        NullableBoolean,
        NullableDateTime,
        Enum
    }
#endif



    //AvailableCellTypes
    //•	DateTimeStyle
    //•	CheckBoxStyle
    //•	IntegerEditStyle
    //•	PercentEditStyle
    //•	DoubleEditStyle
    //•	CurrencyEditStyle
    //• UpDownEditStyle
    //• MaskEditStyle

#if !SILVERLIGHT

    public class GridDataDateTimeVisibleColumn : GridDataVisibleColumn
    {
        //Associated control
        private GridDataDateTimeVisibleColumnControl dateTimeControl;

        public GridDataDateTimeVisibleColumn()
        {
            this.dateTimeControl = new GridDataDateTimeVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.DateTimeEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle),
            typeof(GridDataDateTimeVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));
        [XmlIgnore]
        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataDateTimeVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataDateTimeVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DateTimeEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataDateTimeVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DateTimeEdit;
            }
        }
        [XmlIgnore]
        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataDateTimeVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataDateTimeVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataDateTimeVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates style for DateTime columns.
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeColumn = d as GridDataDateTimeVisibleColumn;

            if (e.NewValue != null)
            {
                dateTimeColumn.dateTimeControl.Style = (Style)e.NewValue;
                dateTimeColumn.dateTimeControl.InitializeGridStyle(dateTimeColumn.ColumnStyle);
            }
        }

        #endregion

    }

    public class GridDataDateTimeVisibleColumnControl : GridDataVisibleColumnControl
    {
        public GridDataDateTimeVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);

            gridDataColumnStyleInfo.DateTimeEdit.IsCalendarEnabled = this.IsCalendarEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsPopupEnabled = this.IsPopupEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsEditable = this.IsEditable;
            gridDataColumnStyleInfo.DateTimeEdit.NoneDateText = this.NoneDateText;
            gridDataColumnStyleInfo.DateTimeEdit.AutoCorrectedHighlightDuration = this.AutoCorrectedHighlightDuration;
            gridDataColumnStyleInfo.DateTimeEdit.CorrectForeground = this.CorrectForeground;
            gridDataColumnStyleInfo.DateTimeEdit.Cursor = this.Cursor;
            gridDataColumnStyleInfo.DateTimeEdit.CustomPattern = this.CustomPattern;
            gridDataColumnStyleInfo.DateTimeEdit.DateTimePattern = this.DateTimePattern;
            gridDataColumnStyleInfo.DateTimeEdit.IncorrectForeground = this.IncorrectForeground;
            gridDataColumnStyleInfo.DateTimeEdit.IsAnimation = this.IsAnimation;
            gridDataColumnStyleInfo.DateTimeEdit.IsAutoCorrect = this.IsAutoCorrect;
            gridDataColumnStyleInfo.DateTimeEdit.IsButtonPopUpEnabled = this.IsButtonPopUpEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsCalendarEnabled = this.IsCalendarEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsEditable = this.IsEditable;
            gridDataColumnStyleInfo.DateTimeEdit.IsEmptyDateEnabled = this.IsEmptyDateEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsEnabledRepeatButton = this.IsEnabledRepeatButton;
            gridDataColumnStyleInfo.DateTimeEdit.IsHoldMaxWidth = this.IsHoldMaxWidth;
            gridDataColumnStyleInfo.DateTimeEdit.IsPopupEnabled = this.IsPopupEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.IsScrollingOnCircle = this.IsScrollingOnCircle;
            gridDataColumnStyleInfo.DateTimeEdit.IsVisibleRepeatButton = this.IsVisibleRepeatButton;
            gridDataColumnStyleInfo.DateTimeEdit.IsWatchEnabled = this.IsWatchEnabled;
            gridDataColumnStyleInfo.DateTimeEdit.MaxDateTime = this.MaxDateTime;
            gridDataColumnStyleInfo.DateTimeEdit.MinDateTime = this.MinDateTime;
            gridDataColumnStyleInfo.DateTimeEdit.NoneDateText = this.NoneDateText;
            gridDataColumnStyleInfo.DateTimeEdit.RepeatButtonBackground = this.RepeatButtonBackground;
            gridDataColumnStyleInfo.DateTimeEdit.RepeatButtonBorderBrush = this.RepeatButtonBorderBrush;
            gridDataColumnStyleInfo.DateTimeEdit.UncertainForeground = this.UncertainForeground;
        }

        #region AutoCorrectedHighlightDuration

        /// <summary>
        /// AutoCorrectedHighlightDuration Dependency Property
        /// </summary>
        public static readonly DependencyProperty AutoCorrectedHighlightDurationProperty =
            DependencyProperty.Register("AutoCorrectedHighlightDuration", typeof(double), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((double)1d));

        /// <summary>
        /// Gets or sets the AutoCorrectedHighlightDuration property. This dependency property 
        /// indicates ....
        /// </summary>
        public double AutoCorrectedHighlightDuration
        {
            get { return (double)GetValue(AutoCorrectedHighlightDurationProperty); }
            set { SetValue(AutoCorrectedHighlightDurationProperty, value); }
        }

        #endregion

        #region CorrectForeground

        /// <summary>
        /// CorrectForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty CorrectForegroundProperty =
            DependencyProperty.Register("CorrectForeground", typeof(Brush), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata(Brushes.Blue));

        /// <summary>
        /// Gets or sets the CorrectForeground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush CorrectForeground
        {
            get { return (Brush)GetValue(CorrectForegroundProperty); }
            set { SetValue(CorrectForegroundProperty, value); }
        }

        #endregion
        
        #region CustomPattern

        /// <summary>
        /// CustomPattern Dependency Property
        /// </summary>
        public static readonly DependencyProperty CustomPatternProperty =
            DependencyProperty.Register("CustomPattern", typeof(string), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata(GridDateTimeEditStyleInfo.Default.CustomPattern));

        /// <summary>
        /// Gets or sets the CustomPattern property. This dependency property 
        /// indicates ....
        /// </summary>
        public string CustomPattern
        {
            get { return (string)GetValue(CustomPatternProperty); }
            set { SetValue(CustomPatternProperty, value); }
        }

        #endregion

        #region DateTimePattern

        /// <summary>
        /// DateTimePattern Dependency Property
        /// </summary>
        public static readonly DependencyProperty DateTimePatternProperty =
            DependencyProperty.Register("DateTimePattern", typeof(DateTimePattern), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((DateTimePattern)GridDateTimeEditStyleInfo.Default.DateTimePattern));

        /// <summary>
        /// Gets or sets the DateTimePattern property. This dependency property 
        /// indicates ....
        /// </summary>
        public DateTimePattern DateTimePattern
        {
            get { return (DateTimePattern)GetValue(DateTimePatternProperty); }
            set { SetValue(DateTimePatternProperty, value); }
        }

        #endregion

        #region IncorrectForeground

        /// <summary>
        /// IncorrectForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty IncorrectForegroundProperty =
            DependencyProperty.Register("IncorrectForeground", typeof(Brush), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((Brush)GridDateTimeEditStyleInfo.Default.IncorrectForeground));

        /// <summary>
        /// Gets or sets the IncorrectForeground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush IncorrectForeground
        {
            get { return (Brush)GetValue(IncorrectForegroundProperty); }
            set { SetValue(IncorrectForegroundProperty, value); }
        }

        #endregion

        #region IsAnimation

        /// <summary>
        /// IsAnimation Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsAnimation));

        /// <summary>
        /// Gets or sets the IsAnimation property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }

        #endregion

        #region IsAutoCorrect

        /// <summary>
        /// IsAutoCorrect Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAutoCorrectProperty =
            DependencyProperty.Register("IsAutoCorrect", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsAutoCorrect));

        /// <summary>
        /// Gets or sets the IsAutoCorrect property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsAutoCorrect
        {
            get { return (bool)GetValue(IsAutoCorrectProperty); }
            set { SetValue(IsAutoCorrectProperty, value); }
        }

        #endregion

        #region IsButtonPopUpEnabled

        /// <summary>
        /// IsButtonPopUpEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsButtonPopUpEnabledProperty =
            DependencyProperty.Register("IsButtonPopUpEnabled", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsButtonPopUpEnabled));

        /// <summary>
        /// Gets or sets the IsButtonPopUpEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsButtonPopUpEnabled
        {
            get { return (bool)GetValue(IsButtonPopUpEnabledProperty); }
            set { SetValue(IsButtonPopUpEnabledProperty, value); }
        }

        #endregion

        #region IsCalendarEnabled

        /// <summary>
        /// IsCalendarEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsCalendarEnabledProperty =
            DependencyProperty.Register("IsCalendarEnabled", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsCalendarEnabled));

        /// <summary>
        /// Gets or sets the IsCalendarEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsCalendarEnabled
        {
            get { return (bool)GetValue(IsCalendarEnabledProperty); }
            set { SetValue(IsCalendarEnabledProperty, value); }
        }

        #endregion

        #region IsEditable

        /// <summary>
        /// IsEditable Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)true));

        /// <summary>
        /// Gets or sets the IsEditable property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        #endregion

        #region IsEmptyDateEnabled

        /// <summary>
        /// IsEmptyDateEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEmptyDateEnabledProperty =
            DependencyProperty.Register("IsEmptyDateEnabled", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsEmptyDateEnabled));

        /// <summary>
        /// Gets or sets the IsEmptyDateEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsEmptyDateEnabled
        {
            get { return (bool)GetValue(IsEmptyDateEnabledProperty); }
            set { SetValue(IsEmptyDateEnabledProperty, value); }
        }

        #endregion

        #region IsEnabledRepeatButton

        /// <summary>
        /// IsEnabledRepeatButton Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEnabledRepeatButtonProperty =
            DependencyProperty.Register("IsEnabledRepeatButton", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsEnabledRepeatButton));

        /// <summary>
        /// Gets or sets the IsEnabledRepeatButton property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsEnabledRepeatButton
        {
            get { return (bool)GetValue(IsEnabledRepeatButtonProperty); }
            set { SetValue(IsEnabledRepeatButtonProperty, value); }
        }

        #endregion

        #region IsHoldMaxWidth

        /// <summary>
        /// IsHoldMaxWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsHoldMaxWidthProperty =
            DependencyProperty.Register("IsHoldMaxWidth", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsHoldMaxWidth));

        /// <summary>
        /// Gets or sets the IsHoldMaxWidth property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsHoldMaxWidth
        {
            get { return (bool)GetValue(IsHoldMaxWidthProperty); }
            set { SetValue(IsHoldMaxWidthProperty, value); }
        }

        #endregion

        #region IsPopupEnabled

        /// <summary>
        /// IsPopupEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsPopupEnabledProperty =
            DependencyProperty.Register("IsPopupEnabled", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsPopupEnabled));

        /// <summary>
        /// Gets or sets the IsPopupEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsPopupEnabled
        {
            get { return (bool)GetValue(IsPopupEnabledProperty); }
            set { SetValue(IsPopupEnabledProperty, value); }
        }

        #endregion

        #region IsScrollingOnCircle

        /// <summary>
        /// IsScrollingOnCircle Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsScrollingOnCircleProperty =
            DependencyProperty.Register("IsScrollingOnCircle", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsScrollingOnCircle));

        /// <summary>
        /// Gets or sets the IsScrollingOnCircle property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get { return (bool)GetValue(IsScrollingOnCircleProperty); }
            set { SetValue(IsScrollingOnCircleProperty, value); }
        }

        #endregion

        #region IsVisibleRepeatButton

        /// <summary>
        /// IsVisibleRepeatButton Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsVisibleRepeatButtonProperty =
            DependencyProperty.Register("IsVisibleRepeatButton", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsVisibleRepeatButton));

        /// <summary>
        /// Gets or sets the IsVisibleRepeatButton property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsVisibleRepeatButton
        {
            get { return (bool)GetValue(IsVisibleRepeatButtonProperty); }
            set { SetValue(IsVisibleRepeatButtonProperty, value); }
        }

        #endregion

        #region IsWatchEnabled

        /// <summary>
        /// IsWatchEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsWatchEnabledProperty =
            DependencyProperty.Register("IsWatchEnabled", typeof(bool), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((bool)GridDateTimeEditStyleInfo.Default.IsWatchEnabled));

        /// <summary>
        /// Gets or sets the IsWatchEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsWatchEnabled
        {
            get { return (bool)GetValue(IsWatchEnabledProperty); }
            set { SetValue(IsWatchEnabledProperty, value); }
        }

        #endregion

        #region MaxDateTime

        /// <summary>
        /// MaxDateTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxDateTimeProperty =
            DependencyProperty.Register("MaxDateTime", typeof(DateTime), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((DateTime)GridDateTimeEditStyleInfo.Default.MaxDateTime));

        /// <summary>
        /// Gets or sets the MaxDateTime property. This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime MaxDateTime
        {
            get { return (DateTime)GetValue(MaxDateTimeProperty); }
            set { SetValue(MaxDateTimeProperty, value); }
        }

        #endregion

        #region MinDateTime

        /// <summary>
        /// MinDateTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinDateTimeProperty =
            DependencyProperty.Register("MinDateTime", typeof(DateTime), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((DateTime)GridDateTimeEditStyleInfo.Default.MinDateTime));

        /// <summary>
        /// Gets or sets the MinDateTime property. This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime MinDateTime
        {
            get { return (DateTime)GetValue(MinDateTimeProperty); }
            set { SetValue(MinDateTimeProperty, value); }
        }

        #endregion

        #region NoneDateText

        /// <summary>
        /// NoneDateText Dependency Property
        /// </summary>
        public static readonly DependencyProperty NoneDateTextProperty =
            DependencyProperty.Register("NoneDateText", typeof(string), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((string)GridDateTimeEditStyleInfo.Default.NoneDateText));

        /// <summary>
        /// Gets or sets the NoneDateText property. This dependency property 
        /// indicates ....
        /// </summary>
        public string NoneDateText
        {
            get { return (string)GetValue(NoneDateTextProperty); }
            set { SetValue(NoneDateTextProperty, value); }
        }

        #endregion

        #region RepeatButtonBackground

        /// <summary>
        /// RepeatButtonBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty RepeatButtonBackgroundProperty =
            DependencyProperty.Register("RepeatButtonBackground", typeof(Brush), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((Brush)GridDateTimeEditStyleInfo.Default.RepeatButtonBackground));

        /// <summary>
        /// Gets or sets the RepeatButtonBackground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush RepeatButtonBackground
        {
            get { return (Brush)GetValue(RepeatButtonBackgroundProperty); }
            set { SetValue(RepeatButtonBackgroundProperty, value); }
        }

        #endregion

        #region RepeatButtonBorderBrush

        /// <summary>
        /// RepeatButtonBorderBrush Dependency Property
        /// </summary>
        public static readonly DependencyProperty RepeatButtonBorderBrushProperty =
            DependencyProperty.Register("RepeatButtonBorderBrush", typeof(Brush), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((Brush)GridDateTimeEditStyleInfo.Default.RepeatButtonBorderBrush));

        /// <summary>
        /// Gets or sets the RepeatButtonBorderBrush property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush RepeatButtonBorderBrush
        {
            get { return (Brush)GetValue(RepeatButtonBorderBrushProperty); }
            set { SetValue(RepeatButtonBorderBrushProperty, value); }
        }

        #endregion

        #region UncertainForeground

        /// <summary>
        /// UncertainForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty UncertainForegroundProperty =
            DependencyProperty.Register("UncertainForeground", typeof(Brush), typeof(GridDataDateTimeVisibleColumnControl),
                new PropertyMetadata((Brush)GridDateTimeEditStyleInfo.Default.UncertainForeground));

        /// <summary>
        /// Gets or sets the UncertainForeground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush UncertainForeground
        {
            get { return (Brush)GetValue(UncertainForegroundProperty); }
            set { SetValue(UncertainForegroundProperty, value); }
        }

        #endregion
    }

    public class GridDataCheckBoxVisibleColumn : GridDataVisibleColumn
    {
        private GridDataCheckBoxVisibleColumnControl checkBoxControl;

        public GridDataCheckBoxVisibleColumn()
        {
            this.checkBoxControl = new GridDataCheckBoxVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.CheckBox };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataCheckBoxVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));
        [XmlIgnore]
        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataCheckBoxVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataCheckBoxVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.CheckBox;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataCheckBoxVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.CheckBox;
            }
        }
        [XmlIgnore]
        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataCheckBoxVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataCheckBoxVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataCheckBoxVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var checkBoxColumn = d as GridDataCheckBoxVisibleColumn;

            if (e.NewValue != null)
            {
                checkBoxColumn.checkBoxControl.Style = (Style)e.NewValue;
                checkBoxColumn.checkBoxControl.InitializeGridStyle(checkBoxColumn.ColumnStyle);
            }
        }

        #endregion

    }

    public class GridDataCheckBoxVisibleColumnControl : GridDataVisibleColumnControl
    {
        public GridDataCheckBoxVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
        }
    }

#endif

    public class GridDataTimeSpanEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataTimeSpanEditVisibleColumnControl timeSpanEditControl;
        public GridDataTimeSpanEditVisibleColumn()
        {
            this.timeSpanEditControl = new GridDataTimeSpanEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.UpDownEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle),
            typeof(GridDataTimeSpanEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataTimeSpanEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataTimeSpanEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.TimeSpanEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataTimeSpanEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.UpDownEdit;
            }
        }

        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataTimeSpanEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataTimeSpanEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataTimeSpanEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timsSpanEditEditColumn = (GridDataTimeSpanEditVisibleColumn)d;
            if (timsSpanEditEditColumn != null)
            {
                timsSpanEditEditColumn.timeSpanEditControl.Style = (Style)e.NewValue;
                timsSpanEditEditColumn.timeSpanEditControl.InitializeGridStyle(timsSpanEditEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataTimeSpanEditVisibleColumnControl : GridDataVisibleColumnControl
    {
        public GridDataTimeSpanEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);

            gridDataColumnStyleInfo.TimeSpanEdit.AllowNull = this.AllowNull;
            gridDataColumnStyleInfo.TimeSpanEdit.Format = this.Format;
            gridDataColumnStyleInfo.TimeSpanEdit.IncrementOnScrolling = this.IncrementOnScrolling;
            gridDataColumnStyleInfo.TimeSpanEdit.MinValue = this.MinValue;
            gridDataColumnStyleInfo.TimeSpanEdit.MaxValue = this.MaxValue;
            gridDataColumnStyleInfo.TimeSpanEdit.NullString = this.NullString;
            gridDataColumnStyleInfo.TimeSpanEdit.ShowArrowButtons = this.ShowArrowButtons;
        }

        #region AllowNull

        /// <summary>
        /// AllowNull Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((bool)GridTimeSpanEditStyleInfo.Default.AllowNull));

        /// <summary>
        /// Gets or sets the AllowNull property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }

        #endregion

        #region Format

        /// <summary>
        /// Format Dependency Property
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((string)GridTimeSpanEditStyleInfo.Default.Format));

        /// <summary>
        /// Gets or sets the Format property. This dependency property 
        /// indicates ....
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        #endregion

        #region IncrementOnScrolling

        /// <summary>
        /// IncrementOnScrolling Dependency Property
        /// </summary>
        public static readonly DependencyProperty IncrementOnScrollingProperty =
            DependencyProperty.Register("IncrementOnScrolling", typeof(bool), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((bool)GridTimeSpanEditStyleInfo.Default.IncrementOnScrolling));

        /// <summary>
        /// Gets or sets the IncrementOnScrolling property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IncrementOnScrolling
        {
            get { return (bool)GetValue(IncrementOnScrollingProperty); }
            set { SetValue(IncrementOnScrollingProperty, value); }
        }

        #endregion

        #region NullString

        /// <summary>
        /// NullString Dependency Property
        /// </summary>
        public static readonly DependencyProperty NullStringProperty =
            DependencyProperty.Register("NullString", typeof(string), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((string)GridTimeSpanEditStyleInfo.Default.NullString));

        /// <summary>
        /// Gets or sets the NullString property. This dependency property 
        /// indicates ....
        /// </summary>
        public string NullString
        {
            get { return (string)GetValue(NullStringProperty); }
            set { SetValue(NullStringProperty, value); }
        }

        #endregion

        #region MaxValue

        /// <summary>
        /// MaxValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(TimeSpan), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((TimeSpan)GridTimeSpanEditStyleInfo.Default.MaxValue));

        /// <summary>
        /// Gets or sets the MaxValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public TimeSpan MaxValue
        {
            get { return (TimeSpan)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region MinValue

        /// <summary>
        /// MinValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(TimeSpan), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((TimeSpan)GridTimeSpanEditStyleInfo.Default.MinValue));

        /// <summary>
        /// Gets or sets the MinValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public TimeSpan MinValue
        {
            get { return (TimeSpan)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region ShowArrowButtons

        /// <summary>
        /// ShowArrowButtons Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowArrowButtonsProperty =
            DependencyProperty.Register("ShowArrowButtons", typeof(bool), typeof(GridDataTimeSpanEditVisibleColumnControl),
                new PropertyMetadata((bool)GridTimeSpanEditStyleInfo.Default.IncrementOnScrolling));

        /// <summary>
        /// Gets or sets the ShowArrowButtons property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool ShowArrowButtons
        {
            get { return (bool)GetValue(ShowArrowButtonsProperty); }
            set { SetValue(ShowArrowButtonsProperty, value); }
        }

        #endregion

    }

    public class GridDataUpDownEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataUpDownEditVisibleColumnControl upDownEditControl;
        public GridDataUpDownEditVisibleColumn()
        {
            this.upDownEditControl = new GridDataUpDownEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.UpDownEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle),
            typeof(GridDataUpDownEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataUpDownEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataUpDownEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.UpDownEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataUpDownEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.UpDownEdit;
            }
        }

        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataUpDownEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataUpDownEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataUpDownEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var upDownEditColumn = (GridDataUpDownEditVisibleColumn)d;
            if (upDownEditColumn != null)
            {
                upDownEditColumn.upDownEditControl.Style = (Style)e.NewValue;
                upDownEditColumn.upDownEditControl.InitializeGridStyle(upDownEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataUpDownEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataUpDownEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);

#if !SILVERLIGHT
            gridDataColumnStyleInfo.UpDownEdit.NegativeForeground = this.NegativeForeground;
            gridDataColumnStyleInfo.UpDownEdit.FocusedBackground = this.FocusedBackground;
            gridDataColumnStyleInfo.UpDownEdit.FocusedForeground = this.FocusedForeground;
            gridDataColumnStyleInfo.UpDownEdit.FocusedBorderBrush = this.FocusedBorderBrush;
            gridDataColumnStyleInfo.UpDownEdit.AnimationSpeed = this.AnimationSpeed;
#endif
            gridDataColumnStyleInfo.UpDownEdit.MaxValue = this.MaxValue;
            gridDataColumnStyleInfo.UpDownEdit.MinValue = this.MinValue;
        }

#if !SILVERLIGHT

        #region NegativeForeground

        /// <summary>
        /// NegativeForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty NegativeForegroundProperty =
            DependencyProperty.Register("NegativeForeground", typeof(Brush), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((Brush)GridUpDownEditStyleInfo.Default.NegativeForeground));

        /// <summary>
        /// Gets or sets the NegativeForeground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush NegativeForeground
        {
            get { return (Brush)GetValue(NegativeForegroundProperty); }
            set { SetValue(NegativeForegroundProperty, value); }
        }

        #endregion

        #region FocusedBackground

        /// <summary>
        /// FocusedBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedBackgroundProperty =
            DependencyProperty.Register("FocusedBackground", typeof(Brush), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((Brush)GridUpDownEditStyleInfo.Default.FocusedBackground));

        /// <summary>
        /// Gets or sets the FocusedBackground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush FocusedBackground
        {
            get { return (Brush)GetValue(FocusedBackgroundProperty); }
            set { SetValue(FocusedBackgroundProperty, value); }
        }

        #endregion

        #region FocusedForeground

        /// <summary>
        /// FocusedForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedForegroundProperty =
            DependencyProperty.Register("FocusedForeground", typeof(Brush), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((Brush)GridUpDownEditStyleInfo.Default.FocusedForeground));

        /// <summary>
        /// Gets or sets the FocusedForeground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush FocusedForeground
        {
            get { return (Brush)GetValue(FocusedForegroundProperty); }
            set { SetValue(FocusedForegroundProperty, value); }
        }

        #endregion

        #region FocusedBorderBrush

        /// <summary>
        /// FocusedBorderBrush Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedBorderBrushProperty =
            DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((Brush)GridUpDownEditStyleInfo.Default.FocusedBorderBrush));

        /// <summary>
        /// Gets or sets the FocusedBorderBrush property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        #endregion

#endif

        #region MaxValue

        /// <summary>
        /// MaxValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((double)GridUpDownEditStyleInfo.Default.MaxValue));

        /// <summary>
        /// Gets or sets the MaxValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region MinValue

        /// <summary>
        /// MinValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((double)GridUpDownEditStyleInfo.Default.MinValue));

        /// <summary>
        /// Gets or sets the MinValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

#if !SILVERLIGHT

        #region AnimationSpeed

        /// <summary>
        /// AnimationSpeed Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register("AnimationSpeed", typeof(double), typeof(GridDataUpDownEditVisibleColumnControl),
                new PropertyMetadata((double)GridUpDownEditStyleInfo.Default.AnimationSpeed));

        /// <summary>
        /// Gets or sets the AnimationSpeed property. This dependency property 
        /// indicates ....
        /// </summary>
        public double AnimationSpeed
        {
            get { return (double)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        #endregion

#endif

    }

    public class GridDataIntegerEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataIntegerEditVisibleColumnControl integerEditcontrol;
        public GridDataIntegerEditVisibleColumn()
        {
            this.integerEditcontrol = new GridDataIntegerEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellType = "IntegerEdit" };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle),
            typeof(GridDataIntegerEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));
        [XmlIgnore]
        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataIntegerEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataIntegerEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.IntegerEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataIntegerEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.IntegerEdit;
            }
        }
        [XmlIgnore]
        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataIntegerEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataIntegerEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataIntegerEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
       [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var integerEditColumn = d as GridDataIntegerEditVisibleColumn;

            if (e.NewValue != null)
            {
                integerEditColumn.integerEditcontrol.Style = (Style)e.NewValue;
                integerEditColumn.integerEditcontrol.InitializeGridStyle(integerEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataIntegerEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataIntegerEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
        }
    }

    public class GridDataPercentEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataPercentEditVisibleColumnControl percentEditControl;

        public GridDataPercentEditVisibleColumn()
        {
            this.percentEditControl = new GridDataPercentEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.PercentEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle),
            typeof(GridDataPercentEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataPercentEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataPercentEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.PercentEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataPercentEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.PercentEdit;
            }
        }

        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataPercentEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataPercentEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataPercentEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var percentEditColumn = (GridDataPercentEditVisibleColumn)d;
            if (percentEditColumn != null)
            {
                percentEditColumn.percentEditControl.Style = (Style)e.NewValue;
                percentEditColumn.percentEditControl.InitializeGridStyle(percentEditColumn.ColumnStyle);
            }
        }

        #endregion


    }

    public class GridDataPercentEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataPercentEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
        }
    }

    [Obsolete]
    public class GridDataDouleEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataDouleEditVisibleColumnControl doubleEditControl;

        public GridDataDouleEditVisibleColumn()
        {
            this.doubleEditControl = new GridDataDouleEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.DoubleEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataDouleEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));
        [XmlIgnore]
        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataDouleEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataDouleEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DoubleEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataDouleEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DoubleEdit;
            }
        }
        [XmlIgnore]
        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataDouleEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataDouleEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataDouleEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var doubleEditColumn = (GridDataDouleEditVisibleColumn)d;
            if (doubleEditColumn != null)
            {
                doubleEditColumn.doubleEditControl.Style = (Style)e.NewValue;
                doubleEditColumn.doubleEditControl.InitializeGridStyle(doubleEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataDoubleEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataDoubleEditVisibleColumnControl doubleEditControl;

        public GridDataDoubleEditVisibleColumn()
        {
            this.doubleEditControl = new GridDataDoubleEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.DoubleEdit };            
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataDoubleEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));
        [XmlIgnore]
        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataDoubleEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataDoubleEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DoubleEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataDoubleEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.DoubleEdit;
            }
        }
        [XmlIgnore]
        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataDoubleEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataDoubleEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataDoubleEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var doubleEditColumn = (GridDataDoubleEditVisibleColumn)d;
            if (doubleEditColumn != null)
            {
                doubleEditColumn.doubleEditControl.Style = (Style)e.NewValue;
                doubleEditColumn.doubleEditControl.InitializeGridStyle(doubleEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    [Obsolete]
    public class GridDataDouleEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataDouleEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
        }
    }

    public class GridDataDoubleEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataDoubleEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
        }
    }

    public class GridDataCurrencyEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataCurrencyEditVisibleColumnControl currencyEditControl;

        public GridDataCurrencyEditVisibleColumn()
        {
            this.currencyEditControl = new GridDataCurrencyEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.CurrencyEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataCurrencyEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataCurrencyEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataCurrencyEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.CurrencyEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataCurrencyEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.CurrencyEdit;
            }
        }

        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataCurrencyEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataCurrencyEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataCurrencyEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currenyEditColumn = (GridDataCurrencyEditVisibleColumn)d;
            if (currenyEditColumn != null)
            {
                currenyEditColumn.currencyEditControl.Style = (Style)e.NewValue;
                currenyEditColumn.currencyEditControl.InitializeGridStyle(currenyEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataCurrencyEditVisibleColumnControl : GridDataNumberFormatStyleControl
    {
        public GridDataCurrencyEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);
#if !SILVERLIGHT
            gridDataColumnStyleInfo.CurrencyEdit.IsScrollingOnCircle = this.IsScrollingOnCircle;
#endif
            gridDataColumnStyleInfo.CurrencyEdit.MinValue = this.MinValue;
            gridDataColumnStyleInfo.CurrencyEdit.MaxValue = this.MaxValue;
        }

        #region MinValue

        /// <summary>
        /// MinValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(GridDataCurrencyEditVisibleColumnControl),
                new PropertyMetadata((decimal)GridCurrencyEditStyleInfo.Default.MinValue));

        /// <summary>
        /// Gets or sets the MinValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public decimal MinValue
        {
            get { return (decimal)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region MaxValue

        /// <summary>
        /// MaxValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(GridDataCurrencyEditVisibleColumnControl),
                new PropertyMetadata((decimal)GridCurrencyEditStyleInfo.Default.MaxValue));

        /// <summary>
        /// Gets or sets the MaxValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public decimal MaxValue
        {
            get { return (decimal)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

#if !SILVERLIGHT

        #region IsScrollingOnCircle

        /// <summary>
        /// IsScrollingOnCircle Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsScrollingOnCircleProperty =
            DependencyProperty.Register("IsScrollingOnCircle", typeof(bool), typeof(GridDataCurrencyEditVisibleColumn),
                new PropertyMetadata((bool)GridCurrencyEditStyleInfo.Default.IsScrollingOnCircle));

        /// <summary>
        /// Gets or sets the IsScrollingOnCircle property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get { return (bool)GetValue(IsScrollingOnCircleProperty); }
            set { SetValue(IsScrollingOnCircleProperty, value); }
        }

        #endregion

#endif

    }

    public class GridDataMaskEditVisibleColumn : GridDataVisibleColumn
    {
        private GridDataMaskEditVisibleColumnControl maskEditControl;

        public GridDataMaskEditVisibleColumn()
        {
            this.maskEditControl = new GridDataMaskEditVisibleColumnControl();
            this.ColumnStyle = new GridDataColumnStyle() { CellTypeEnum = GridDataCellType.MaskEdit };
        }

        #region ColumnStyle
        private static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataColumnStyle), typeof(GridDataMaskEditVisibleColumn), new PropertyMetadata(OnColumnStyleChanged));

        public override GridDataColumnStyle ColumnStyle
        {
            get
            {
                return this.GetValue(GridDataMaskEditVisibleColumn.ColumnStyleProperty) as GridDataColumnStyle;
            }

            set
            {
                this.SetValue(GridDataMaskEditVisibleColumn.ColumnStyleProperty, value);
            }
        }

        private static void OnColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.MaskEdit;
            }
        }

        #endregion

        #region HeaderStyle

        private static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(GridDataColumnStyle),
            typeof(GridDataMaskEditVisibleColumn),
            new PropertyMetadata(OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var newstyle = args.NewValue as GridDataColumnStyle;
                if (newstyle != null)
                    newstyle.CellTypeEnum = GridDataCellType.MaskEdit;
            }
        }

        public override GridDataColumnStyle HeaderStyle
        {
            get
            {
                return (GridDataColumnStyle)this.GetValue(GridDataMaskEditVisibleColumn.HeaderStyleProperty);
            }

            set
            {
                this.SetValue(GridDataMaskEditVisibleColumn.HeaderStyleProperty, value);
            }
        }

        #endregion

        #region ElementStyle

        /// <summary>
        /// ElementStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register("ElementStyle", typeof(Style), typeof(GridDataMaskEditVisibleColumn),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnElementStyleChanged)));

        /// <summary>
        /// Gets or sets the ElementStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        [XmlIgnore]
        public Style ElementStyle
        {
            get { return (Style)GetValue(ElementStyleProperty); }
            set { SetValue(ElementStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ElementStyle property.
        /// </summary>
        private static void OnElementStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var maskEditColumn = (GridDataMaskEditVisibleColumn)d;
            if (maskEditColumn != null)
            {
                maskEditColumn.maskEditControl.Style = (Style)e.NewValue;
                maskEditColumn.maskEditControl.InitializeGridStyle(maskEditColumn.ColumnStyle);
            }
        }

        #endregion
    }

    public class GridDataMaskEditVisibleColumnControl : GridDataVisibleColumnControl
    {
        public GridDataMaskEditVisibleColumnControl()
        {

        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            var gridDataColumnStyleInfo = gridStyle as GridDataColumnStyle;

            if (gridDataColumnStyleInfo == null)
            {
                throw new InvalidCastException("Cannot type cast to GridDataColumnStyle. GridStyleInfo should be of type GridDataColumnStyle");
            }

            base.InitializeGridStyle(gridStyle);

            gridDataColumnStyleInfo.MaskEdit.CurrencySymbol = this.CurrencySymbol;
            gridDataColumnStyleInfo.MaskEdit.DateSeparator = this.DateSeparator;
            gridDataColumnStyleInfo.MaskEdit.DecimalSeparator = this.DecimalSeparator;
            gridDataColumnStyleInfo.MaskEdit.Mask = this.Mask;
            gridDataColumnStyleInfo.MaskEdit.NumberGroupSeparator = this.NumberGroupSeparator;
            gridDataColumnStyleInfo.MaskEdit.PromptChar = this.PromptChar;
            gridDataColumnStyleInfo.MaskEdit.TimeSeparator = this.TimeSeparator;
        }

        #region CurrencySymbol

        /// <summary>
        /// CurrencySymbol Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrencySymbolProperty =
           DependencyProperty.Register("CurrencySymbol", typeof(string), typeof(GridDataMaskEditVisibleColumnControl),
               new PropertyMetadata((string)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.CurrencySymbol));
#else
GridMaskEditInfo.Default.CurrencySymbol));
#endif

        /// <summary>
        /// Gets or sets the CurrencySymbol property. This dependency property 
        /// indicates ....
        /// </summary>
        public string CurrencySymbol
        {
            get { return (string)GetValue(CurrencySymbolProperty); }
            set { SetValue(CurrencySymbolProperty, value); }
        }

        #endregion

        #region DateSeparator

        /// <summary>
        /// DateSeparator Dependency Property
        /// </summary>
        public static readonly DependencyProperty DateSeparatorProperty =
          DependencyProperty.Register("DateSeparator", typeof(string), typeof(GridDataMaskEditVisibleColumnControl),
              new PropertyMetadata((string)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.DateSeparator));
#else
GridMaskEditInfo.Default.DateSeparator));
#endif

        /// <summary>
        /// Gets or sets the DateSeparator property. This dependency property 
        /// indicates ....
        /// </summary>
        public string DateSeparator
        {
            get { return (string)GetValue(DateSeparatorProperty); }
            set { SetValue(DateSeparatorProperty, value); }
        }

        #endregion

        #region DecimalSeparator

        /// <summary>
        /// DecimalSeparator Dependency Property
        /// </summary>
        public static readonly DependencyProperty DecimalSeparatorProperty =
            DependencyProperty.Register("DecimalSeparator", typeof(String), typeof(GridDataMaskEditVisibleColumnControl),
                new PropertyMetadata((String)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.DecimalSeparator));
#else
GridMaskEditInfo.Default.DecimalSeparator));
#endif
        /// <summary>
        /// Gets or sets the DecimalSeparator property. This dependency property 
        /// indicates ....
        /// </summary>
        public String DecimalSeparator
        {
            get { return (String)GetValue(DecimalSeparatorProperty); }
            set { SetValue(DecimalSeparatorProperty, value); }
        }

        #endregion

        #region Mask

        /// <summary>
        /// Mask Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(GridDataMaskEditVisibleColumnControl),
                new PropertyMetadata((string)
#if SILVERLIGHT
    GridMaskEditStyleInfo.Default.Mask));
#else
GridMaskEditInfo.Default.Mask));
#endif

        /// <summary>
        /// Gets or sets the Mask property. This dependency property 
        /// indicates ....
        /// </summary>
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        #endregion

        #region NumberGroupSeparator

        /// <summary>
        /// NumberGroupSeparator Dependency Property
        /// </summary>
        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(GridDataMaskEditVisibleColumnControl),
                new PropertyMetadata((string)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.NumberGroupSeparator));
#else
GridMaskEditInfo.Default.NumberGroupSeparator));
#endif

        /// <summary>
        /// Gets or sets the NumberGroupSeparator property. This dependency property 
        /// indicates ....
        /// </summary>
        public string NumberGroupSeparator
        {
            get { return (string)GetValue(NumberGroupSeparatorProperty); }
            set { SetValue(NumberGroupSeparatorProperty, value); }
        }

        #endregion

        #region PromptChar

        /// <summary>
        /// PromptChar Dependency Property
        /// </summary>
        public static readonly DependencyProperty PromptCharProperty =
            DependencyProperty.Register("PromptChar", typeof(char), typeof(GridDataMaskEditVisibleColumnControl),
                new PropertyMetadata((char)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.PromptChar));
#else
GridMaskEditInfo.Default.PromptChar));
#endif

        /// <summary>
        /// Gets or sets the PromptChar property. This dependency property 
        /// indicates ....
        /// </summary>
        public char PromptChar
        {
            get { return (char)GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        #endregion

        #region TimeSeparator

        /// <summary>
        /// TimeSeparator Dependency Property
        /// </summary>
        public static readonly DependencyProperty TimeSeparatorProperty =
            DependencyProperty.Register("TimeSeparator", typeof(string), typeof(GridDataMaskEditVisibleColumnControl),
                new PropertyMetadata((string)
#if SILVERLIGHT
        GridMaskEditStyleInfo.Default.TimeSeparator));
#else
GridMaskEditInfo.Default.TimeSeparator));
#endif

        /// <summary>
        /// Gets or sets the TimeSeparator property. This dependency property 
        /// indicates ....
        /// </summary>
        public string TimeSeparator
        {
            get { return (string)GetValue(TimeSeparatorProperty); }
            set { SetValue(TimeSeparatorProperty, value); }
        }

        #endregion
    }

    /// <summary>
    /// NumberFormatInfoContainerClass to hold NumberFormatInfo for all numberformat related columns
    /// </summary>
    public class GridDataNumberFormatStyleControl : GridDataVisibleColumnControl
    {
        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            base.InitializeGridStyle(gridStyle);

            if (this.NumberFormatInfo != null)
            {
                gridStyle.NumberFormat = this.NumberFormatInfo;
            }
        }

        #region NumberFormatInfo

        /// <summary>
        /// NumberFormatInfo Dependency Property
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty =
            DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(GridDataNumberFormatStyleControl),
                new PropertyMetadata(GridStyleInfo.Default.NumberFormat));

        /// <summary>
        /// Gets or sets the NumberFormatInfo property. This dependency property 
        /// indicates ....
        /// </summary>
        public NumberFormatInfo NumberFormatInfo
        {
            get { return (NumberFormatInfo)GetValue(NumberFormatInfoProperty); }
            set { SetValue(NumberFormatInfoProperty, value); }
        }

        #endregion
    }

    public class GridDataVisibleColumnControl : GridDataRowControl
    {
        public GridDataVisibleColumnControl()
        {
        }

        public override void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            base.InitializeGridStyle(gridStyle);
        }
    }

    internal class GridDataVisibleColumnWrapper : FrameworkElement
    {
        public GridDataVisibleColumnWrapper()
        {
        }

        #region Value (DependencyProperty)

        /// <summary>
        /// Gets / sets the value
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(GridDataVisibleColumnWrapper), new PropertyMetadata(null));

        #endregion

        public void SetValueBinding(Binding binding)
        {
            var previousBinding = this.GetBindingExpression(ValueProperty);
            if (previousBinding != null)
            {
                this.ClearValue(ValueProperty);
                previousBinding = null; ;
            }
            if (binding.Path != null)
                this.MappingName = binding.Path.Path;
#if !SILVERLIGHT
            if (binding.IsAsync && binding.Path != null)
                this.SetBinding(ValueProperty, binding.Path.Path);
            else
#endif
                this.SetBinding(ValueProperty, binding);
        }

        public string MappingName
        {
            get;
            set;
        }
    }
}
