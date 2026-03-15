#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Data;
using System.Linq.Expressions;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using Key = Windows.System.VirtualKey;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using ListSortDirection = Syncfusion.Data.ListSortDirection;
using System.Collections;
using System.Threading.Tasks;
using Syncfusion.UI.Xaml.Utility;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections;
using System.Threading;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;
using System.Globalization;
using Syncfusion.Windows.Tools.Controls;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    [TemplatePart(Name = "PART_FilterPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_OkButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_ClearFilterButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SortAscendingButton", Type = typeof(SortButton))]
    [TemplatePart(Name = "PART_SortDescendingButton", Type = typeof(SortButton))]
    [TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ThumbGripper", Type = typeof(Thumb))]
    [TemplatePart(Name = "CheckboxFilterControl", Type = typeof(CheckboxFilterControl))]
    public class GridFilterControl : ContentControl, IDisposable, INotifyPropertyChanged
    {

        #region Private Members
        IPropertyAccessProvider provider = null;
        List<FilterElement> distinctCollection = null;
        private bool HasSearchedItems = false;
        static double minWidth;
        static double minHeight;
        double filterPopupHeight;
        double filterPopupWidth;
        bool isResizing;
        string colName;
        private string emptyStringValue = string.Empty;
        GridColumn column;
        private AdvancedFilterType advancedFilterType = AdvancedFilterType.TextFilter;
#if !WPF
        Rect localToggleRect;
        Point localWindowPoint;
        double localTogglepadding;
        List<Control> filterPopUpChildControls = new List<Control>();
#endif
        bool allowBlankFilters;
        int checkedItemsCount, unCheckedItemsCount;
        List<FilterPredicate> filterPredicate = null;
        ICollectionViewAdv excelFilterView;
        System.Linq.Expressions.Expression predicate;
        ParameterExpression paramExpression;
#if !WinRT
        BackgroundWorker bgWorkertoPopulate = new BackgroundWorker();
#endif
        #endregion

        #region Ctor

        public GridFilterControl()
        {
            this.DefaultStyleKey = typeof(GridFilterControl);
            this.Loaded += OnGridFilterControlLoaded;
        }

        #endregion

        #region CLR Properties

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal GridColumn Column
        {
            get { return column; }
            set
            {
                column = value;
                if (column != null)
                {
                    colName = column.MappingName;
                    if (AdvancedFilterControl != null)
                    {
#if !WP
                        if (Column.IsUnbound || Column.DataGrid.View.IsDynamicBound)
                            AdvancedFilterControl.ColumnDataType = typeof(string);
#else
                        if (Column.IsUnbound)
                            AdvancedFilterControl.ColumnDataType = typeof(string);
#endif
                        else
                        {
                            var pdc = this.Column.DataGrid.View.GetItemProperties();
                            var pd = pdc.GetPropertyDescriptor(this.Column.MappingName);
                            if (pd == null)
                                AdvancedFilterControl.ColumnDataType = typeof(string);
                            else
                                AdvancedFilterControl.ColumnDataType = pd.PropertyType;
                        }
                        this.AdvancedFilterType = this.GetAdvancedFilterType();
                    }
#if !WinRT
                    if (Column is GridMaskColumn)
                    {
                        var column1 = Column as GridMaskColumn;
                        emptyStringValue = MaskedEditorModel.GetMaskedText(column1.Mask, string.Empty,
                            column1.DateSeparator,
                            column1.TimeSeparator,
                            column1.DecimalSeparator,
                            NumberFormatInfo.CurrentInfo.NumberGroupSeparator,
                            column1.PromptChar,
                            NumberFormatInfo.CurrentInfo.CurrencySymbol);
                    }
#endif
                    this.FilteredFrom = Column.FilteredFrom;
                }
            }
        }

        /// <summary>
        /// Gets or sets AdvancedFilterType for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal AdvancedFilterType AdvancedFilterType
        {
            get
            {
                return advancedFilterType;
            }
            set
            {
                if (advancedFilterType != value)
                {
                    advancedFilterType = value;
                    SetFilterColumnType();
                }
            }
        }


        #endregion

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the style for AdvancedFilter.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>

        public Style AdvancedFilterStyle
        {
            get { return (Style)GetValue(AdvancedFilterStyleProperty); }
            set { SetValue(AdvancedFilterStyleProperty, value); }
        }

        public static readonly DependencyProperty AdvancedFilterStyleProperty =
            GridDependencyProperty.Register("AdvancedFilterStyle", typeof(Style), typeof(GridFilterControl), new GridPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for CheckboxFilter.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Style CheckboxFilterStyle
        {
            get { return (Style)GetValue(CheckboxFilterStyleProperty); }
            set { SetValue(CheckboxFilterStyleProperty, value); }
        }

        public static readonly DependencyProperty CheckboxFilterStyleProperty =
            GridDependencyProperty.Register("CheckboxFilterStyle", typeof(Style), typeof(GridFilterControl), new GridPropertyMetadata(null));


        #region ResizingThumbVisibility

        /// <summary>
        /// DependencyProperty Registration for SearchOptionVisibility 
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ResizingThumbVisibilityProperty = DependencyProperty.Register(
           "ResizingThumbVisibility", typeof(Visibility), typeof(GridFilterControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether the Search option to visible or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public Visibility ResizingThumbVisibility
        {
            get { return (Visibility)this.GetValue(GridFilterControl.ResizingThumbVisibilityProperty); }
            set { this.SetValue(GridFilterControl.ResizingThumbVisibilityProperty, value); }
        }

        #endregion

        #region SortOptionVisibilityProperty

        /// <summary>
        /// DependencyProperty Registration for SortOptionVisibility
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SortOptionVisibilityProperty = DependencyProperty.Register(
          "SortOptionVisibility", typeof(Visibility), typeof(GridFilterControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether [sort option visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [sort option visibility]; otherwise, <c>false</c>.
        /// </value>
        public Visibility SortOptionVisibility
        {
            get { return (Visibility)this.GetValue(GridFilterControl.SortOptionVisibilityProperty); }
            set { this.SetValue(GridFilterControl.SortOptionVisibilityProperty, value); }
        }

        #endregion

        #region AllowBlankFiltersProperty

        /// <summary>
        /// DependencyProperty Registration for AllowBlankFilters
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowBlankFiltersProperty = DependencyProperty.Register(
              "AllowBlankFilters", typeof(bool), typeof(GridFilterControl), new PropertyMetadata(true, null));

        /// Gets or sets a value indicating whether to allow the Blank Filters.
        /// 
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowBlankFilters
        {
            get { return (bool)this.GetValue(GridFilterControl.AllowBlankFiltersProperty); }
            set { this.SetValue(GridFilterControl.AllowBlankFiltersProperty, value); }
        }

        #endregion

        #region ImmediateUpdateColumnFilter

        /// <summary>
        /// DependencyProperty registration for ImmediateUpdateColumnFilter
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ImmediateUpdateColumnFilterProperty =
            DependencyProperty.Register("ImmediateUpdateColumnFilter", typeof(bool), typeof(GridFilterControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether ImmediateUpdateColumnFilter.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ImmediateUpdateColumnFilter
        {
            get { return (bool)GetValue(ImmediateUpdateColumnFilterProperty); }
            set { SetValue(ImmediateUpdateColumnFilterProperty, value); }
        }

        #endregion

        #region FilterPopupHeight

        /// <summary>
        /// DependencyProperty registration for FilterPopupHeight
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilterPopupHeightFilterProperty =
            DependencyProperty.Register("FilterPopupHeight", typeof(double), typeof(GridFilterControl), new PropertyMetadata(minHeight, OnFilterPopupHeightChanged));

        private static void OnFilterPopupHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excelFilterControl = d as GridFilterControl;
            if (!excelFilterControl.isResizing)
                excelFilterControl.filterPopupHeight = (double)e.NewValue;
        }

        /// <summary>
        /// Gets or sets height for Filter popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double FilterPopupHeight
        {
            get { return (double)GetValue(FilterPopupHeightFilterProperty); }
            set { SetValue(FilterPopupHeightFilterProperty, value); }
        }

        #endregion

        #region FilterPopupWidth

        /// <summary>
        /// DependencyProperty registration for FilterPopupWidth
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilterPopupWidthFilterProperty =
            DependencyProperty.Register("FilterPopupWidth", typeof(double), typeof(GridFilterControl), new PropertyMetadata(minWidth, OnFilterPopupWidthChanged));

        private static void OnFilterPopupWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excelFilterControl = d as GridFilterControl;
            if (!excelFilterControl.isResizing)
                excelFilterControl.filterPopupWidth = (double)e.NewValue;
        }

        /// <summary>
        /// Gets or sets width for filter popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double FilterPopupWidth
        {
            get { return (double)GetValue(FilterPopupWidthFilterProperty); }
            set { SetValue(FilterPopupWidthFilterProperty, value); }
        }

        #endregion


        #region IsAdvancedFilterVisible
        /// <summary>
        /// DependencyProperty Registration for IsAdvancedFilterVisible of GridFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty IsAdvancedFilterVisibleProperty = DependencyProperty.Register(
          "IsAdvancedFilterVisible", typeof(bool), typeof(GridFilterControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets IsAdvancedFilterVisible for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool IsAdvancedFilterVisible
        {
            get { return (bool)this.GetValue(GridFilterControl.IsAdvancedFilterVisibleProperty); }
            set { this.SetValue(GridFilterControl.IsAdvancedFilterVisibleProperty, value); }
        }

        #endregion
        #region FilterMode
        /// <summary>
        /// DependencyProperty Registration for FilterMode of GridFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilterModeProperty = DependencyProperty.Register(
          "FilterMode", typeof(FilterMode), typeof(GridFilterControl), new PropertyMetadata(FilterMode.Both, OnFilterModePropertyChanged));

        /// <summary>
        /// Gets or sets FilterMode for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public FilterMode FilterMode
        {
            get { return (FilterMode)this.GetValue(GridFilterControl.FilterModeProperty); }
            set { this.SetValue(GridFilterControl.FilterModeProperty, value); }
        }
        /// <summary>
        /// Dependency call back for FilterMode property Changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private static void OnFilterModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var filterCtrl = obj as GridFilterControl;
            if (filterCtrl.FilterMode == FilterMode.AdvancedFilter)
                filterCtrl.IsAdvancedFilterVisible = true;
            else if (filterCtrl.FilterMode == FilterMode.CheckboxFilter)
                filterCtrl.IsAdvancedFilterVisible = false;
#if !WPF
            filterCtrl.SetFilteredFromVisibility();
#endif
        }

        #endregion

        #region FilteredFrom
        /// <summary>
        /// DependencyProperty Registration for FilteredFrom of GridFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilteredFromProperty = DependencyProperty.Register(
          "FilteredFrom", typeof(FilteredFrom), typeof(GridFilterControl), new PropertyMetadata(FilteredFrom.None, OnFilteredFromChanged));

        /// <summary>
        /// Gets or sets FilteredFrom for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public FilteredFrom FilteredFrom
        {
            get { return (FilteredFrom)this.GetValue(GridFilterControl.FilteredFromProperty); }
            set { this.SetValue(GridFilterControl.FilteredFromProperty, value); }
        }
        private static void OnFilteredFromChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var gridFilterCtrl = obj as GridFilterControl;
#if !WPF
            if (gridFilterCtrl.CheckboxFilterControl != null)
                gridFilterCtrl.CheckboxFilterControl.FilteredFrom = gridFilterCtrl.FilteredFrom;
            gridFilterCtrl.SetFilteredFromVisibility();
#endif
            gridFilterCtrl.Column.FilteredFrom = gridFilterCtrl.FilteredFrom;
        }

        #endregion
#if !WPF
        private Visibility filteredFromVisibility = Visibility.Collapsed;
        public Visibility FilteredFromVisibility
        {
            get { return filteredFromVisibility; }
            set
            {
                if (filteredFromVisibility != value)
                {
                    filteredFromVisibility = value;
                    OnPropertyChanged("FilteredFromVisibility");
                }
            }
        }
        
#endif
        public string AscendingSortString
        {
            get { return (string)GetValue(AscendingSortStringProperty); }
            set { SetValue(AscendingSortStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AscendingSortString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AscendingSortStringProperty =
            DependencyProperty.Register("AscendingSortString", typeof(string), typeof(GridFilterControl), new PropertyMetadata(""));

        public string DescendingSortString
        {
            get { return (string)GetValue(DescendingSortStringProperty); }
            set { SetValue(DescendingSortStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescendingSortString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescendingSortStringProperty =
            DependencyProperty.Register("DescendingSortString", typeof(string), typeof(GridFilterControl), new PropertyMetadata(""));

        #region IsOpenProperty

        /// <summary>
        /// Dependency property Registration for IsOpen
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
              "IsOpen", typeof(bool), typeof(GridFilterControl), new PropertyMetadata(false, OnIsOpenPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return (bool)this.GetValue(GridFilterControl.IsOpenProperty); }
            set { this.SetValue(GridFilterControl.IsOpenProperty, value); }
        }

        /// <summary>
        /// Dependency call back for IsOpen property Changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if WinRT
        private async static void OnIsOpenPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
#else
        private static void OnIsOpenPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
#endif
        {

            var gridFilterCtrl = obj as GridFilterControl;
            if (gridFilterCtrl.Column.MappingName == null)
                throw new InvalidOperationException("MappingName is neccessary for Sorting, Grouping and Filtering");

            if (gridFilterCtrl.Column != null && !gridFilterCtrl.Column.IsUnbound && gridFilterCtrl.Column.DataGrid.View != null)
            {
                var typeInfos = gridFilterCtrl.Column.DataGrid.View.GetItemProperties();
#if !WPF
                var typeInfo = typeInfos.FirstOrDefault(t => t.Key == gridFilterCtrl.Column.MappingName);
                if (typeInfo.Value != null && (typeInfo.Value.PropertyType == typeof(int) || typeInfo.Value.PropertyType == typeof(Double) || typeInfo.Value.PropertyType == typeof(Decimal) || typeInfo.Value.PropertyType == typeof(int?) || typeInfo.Value.PropertyType == typeof(double?) || typeInfo.Value.PropertyType == typeof(decimal?) || typeInfo.Value.PropertyType == typeof(long) || typeInfo.Value.PropertyType == typeof(long?)
                   || typeInfo.Value.PropertyType == typeof(short) || typeInfo.Value.PropertyType == typeof(short?) || typeInfo.Value.PropertyType == typeof(ushort) || typeInfo.Value.PropertyType == typeof(ushort?)
                   || typeInfo.Value.PropertyType == typeof(float) || typeInfo.Value.PropertyType == typeof(float?) || typeInfo.Value.PropertyType == typeof(byte) || typeInfo.Value.PropertyType == typeof(byte?) || typeInfo.Value.PropertyType == typeof(sbyte) | typeInfo.Value.PropertyType == typeof(sbyte?)
                   || typeInfo.Value.PropertyType == typeof(uint) || typeInfo.Value.PropertyType == typeof(uint?) || typeInfo.Value.PropertyType == typeof(ulong) || typeInfo.Value.PropertyType == typeof(ulong?)))

#else
                var typeInfo = typeInfos.Find(gridFilterCtrl.Column.MappingName, false);
                if (typeInfo != null && (typeInfo.PropertyType == typeof(int) || typeInfo.PropertyType == typeof(Double) || typeInfo.PropertyType == typeof(Decimal) || typeInfo.PropertyType == typeof(int?) || typeInfo.PropertyType == typeof(double?) || typeInfo.PropertyType == typeof(decimal?) || typeInfo.PropertyType == typeof(long) || typeInfo.PropertyType == typeof(long?)
                    || typeInfo.PropertyType == typeof(short) || typeInfo.PropertyType == typeof(short?) || typeInfo.PropertyType == typeof(ushort) || typeInfo.PropertyType == typeof(ushort?)
                    || typeInfo.PropertyType == typeof(float) || typeInfo.PropertyType == typeof(float?) || typeInfo.PropertyType == typeof(byte) || typeInfo.PropertyType == typeof(byte?) || typeInfo.PropertyType == typeof(sbyte) | typeInfo.PropertyType == typeof(sbyte?)
                    || typeInfo.PropertyType == typeof(uint) || typeInfo.PropertyType == typeof(uint?) || typeInfo.PropertyType == typeof(ulong) || typeInfo.PropertyType == typeof(ulong?)))
#endif
                {
                    gridFilterCtrl.AscendingSortString = GridResourceWrapper.SortNumberAscending;
                    gridFilterCtrl.DescendingSortString = GridResourceWrapper.SortNumberDescending;
                }
#if !WPF
                else if (typeInfo.Value != null && (typeInfo.Value.PropertyType == typeof(DateTime) || typeInfo.Value.PropertyType == typeof(DateTime?) || typeInfo.Value.PropertyType == typeof(TimeSpan) || typeInfo.Value.PropertyType == typeof(TimeSpan?)))
#else
                else if (typeInfo != null && (typeInfo.PropertyType == typeof(DateTime) || typeInfo.PropertyType == typeof(DateTime?) || typeInfo.PropertyType == typeof(TimeSpan) || typeInfo.PropertyType == typeof(TimeSpan?)))
#endif
                {
                    gridFilterCtrl.AscendingSortString = GridResourceWrapper.SortDateAscending;
                    gridFilterCtrl.DescendingSortString = GridResourceWrapper.SortDateDescending;
                }
                else
                {
                    gridFilterCtrl.AscendingSortString = GridResourceWrapper.SortStringAscending;
                    gridFilterCtrl.DescendingSortString = GridResourceWrapper.SortStringDescending;
                }

            }
            else
            {
                gridFilterCtrl.AscendingSortString = GridResourceWrapper.SortStringAscending;
                gridFilterCtrl.DescendingSortString = GridResourceWrapper.SortStringDescending;
            }
            if (gridFilterCtrl.Column.FilterBehavior == FilterBehavior.StringTyped)
            {
                gridFilterCtrl.AscendingSortString = GridResourceWrapper.SortStringAscending;
                gridFilterCtrl.DescendingSortString = GridResourceWrapper.SortStringDescending;
            }
            gridFilterCtrl.InitializeGridFilterPane();
        }


        #endregion

        public static readonly DependencyProperty FilterColumnTypeProperty = DependencyProperty.Register(
            "FilterColumnType", typeof(string), typeof(GridFilterControl), new PropertyMetadata(GridResourceWrapper.TextFilters));

        /// <summary>
        /// Gets or sets FilterColumnType for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string FilterColumnType
        {
            get { return (string)this.GetValue(GridFilterControl.FilterColumnTypeProperty); }
            set { this.SetValue(GridFilterControl.FilterColumnTypeProperty, value); }
        }
        #endregion

#if !WinRT
        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.Column.DataGrid.View != null)
                this.GenerateItemSource();
        }
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CheckboxFilterControl.HasItemsSource)
                SetItemSource();
        }
#endif

#if !WPF
        #region HorizontalOffsetProperty

        /// <summary>
        /// Dependency property Registration for HorizontalOffset
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
              "HorizontalOffset", typeof(double), typeof(GridFilterControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets HorizontalOffset for the Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double HorizontalOffset
        {
            get { return (double)this.GetValue(GridFilterControl.HorizontalOffsetProperty); }
            set { this.SetValue(GridFilterControl.HorizontalOffsetProperty, value); }
        }

        #endregion

        #region VerticalOffsetProperty

        /// <summary>
        /// Dependency property Registration for VerticalOffset
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
              "VerticalOffset", typeof(double), typeof(GridFilterControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the VerticalOffset for Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double VerticalOffset
        {
            get { return (double)this.GetValue(GridFilterControl.VerticalOffsetProperty); }
            set { this.SetValue(GridFilterControl.VerticalOffsetProperty, value); }
        }


        #endregion
#endif

        #region UIElements

        /// <summary>
        /// Gets or sets the ok button.
        /// </summary>
        /// <value>The ok button.</value>
        internal Button OkButton;

        /// <summary>
        /// Gets or sets the cancel button.
        /// </summary>
        /// <value>The cancel button.</value>
        private Button CancelButton;

        /// <summary>
        /// Gets or sets the resizing thumb.
        /// </summary>
        /// <value>The resizing thumb.</value>
        Thumb ResizingThumb;

        /// <summary>
        /// Gets or sets the filter pop up.
        /// </summary>
        /// <value>The filter pop up.</value>
        internal Popup FilterPopUp;

        /// <summary>
        /// Gets or sets the Button for clear filters.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        Button Part_ClearFilterButton;

        /// <summary>
        /// Gets or sets the Button for Advance filters.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        Button Part_AdvancedFilterButton;

        /// <summary>
        /// Gets or sets the button for Ascending sort.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        SortButton PART_SortAscendingButton;

        /// <summary>
        /// Gets or sets the button for Descending sort.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        SortButton PART_SortDescendingButton;

#if !WPF
        /// <summary>
        /// Gets or sets the Filter PopUp Border.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        Border FilterPopUpBorder;
#endif

        /// <summary>
        /// Gets or sets the control for CheckboxFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        CheckboxFilterControl CheckboxFilterControl;

        /// <summary>
        /// Gets or sets the control for AdvancedFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        AdvancedFilterControl AdvancedFilterControl;

        #endregion

        #region Wire/UnWire Events

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {
#if !WinRT
            bgWorkertoPopulate.DoWork += bgWorker_DoWork;
            bgWorkertoPopulate.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
#endif

            if (FilterPopUp != null)
            {
#if SILVERLIGHT
                FilterPopUp.Loaded += FilterPopUp_Loaded;
#endif
                FilterPopUp.Opened += OnFilterPopUpOpened;
#if !WPF
                FilterPopUpBorder.KeyDown += OnFilterPopUpBorderKeyDown;
#endif
#if !WinRT
                FilterPopUp.AddHandler(Popup.KeyDownEvent, (KeyEventHandler)OnKeyDown, true);
#endif
            }

            if (OkButton != null)
                OkButton.Click += OnFilterOkButtonClick;

            if (CancelButton != null)
                CancelButton.Click += OnCancelButtonClick;

            if (Part_AdvancedFilterButton != null)
                Part_AdvancedFilterButton.Click += OnAdvancedFiltersButtonClick;

            if (Part_ClearFilterButton != null)
                Part_ClearFilterButton.Click += OnClearFilterClick;

            if (PART_SortAscendingButton != null)
                PART_SortAscendingButton.Click += OnSortAscendingButtonClick;

            if (PART_SortDescendingButton != null)
                PART_SortDescendingButton.Click += OnSortDescendingButtonClick;

            if (this.ResizingThumb != null)
            {
#if WinRT
                this.ResizingThumb.PointerEntered += OnResizingThumbEntered;
                this.ResizingThumb.PointerMoved += OnResizingThumbMoved;
                this.ResizingThumb.PointerExited += OnResizingThumbExited;
                this.ResizingThumb.PointerReleased += OnResizingThumbReleased;
#else
                this.ResizingThumb.DragDelta += OnResizingThumbMoved;
#endif

            }

            if (this.Column != null && this.Column.DataGrid != null && this.Column.DataGrid.View != null)
                provider = this.Column.DataGrid.View.GetPropertyAccessProvider();

            this.Unloaded += OnFilterDropDownUnloaded;

#if SILVERLIGHT
            this.FilterPopUp.Closed += (s, e) =>
            {
                var popupAncestor = FindPopupAncestor((Popup)s);
                if (popupAncestor == null) { return; }
                //remove the click event
                popupAncestor.RemoveHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown);

            };
#endif

            if (this.Column != null && this.Column.DataGrid != null && this.Column.DataGrid.View != null)
                excelFilterView = this.Column.DataGrid.View;

        }
#if SILVERLIGHT
        void FilterPopUp_Loaded(object sender, RoutedEventArgs e)
        {
            if ((Column != null && Column.DataGrid != null && !Column.DataGrid.Validations.CheckForValidation(true)))
                ((Popup)sender).IsOpen = false;
            var popupAncestor = FindPopupAncestor((Popup)sender);
            if (popupAncestor == null)
            { return; }
            //add click event to the parent
            popupAncestor.AddHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
        }
#endif

        /// <summary>
        /// UnWires the events.
        /// </summary>
#if WinRT
        private async void UnWireEvents()
#else
        private void UnWireEvents()
#endif
        {
#if !WinRT
            bgWorkertoPopulate.DoWork -= bgWorker_DoWork;
            bgWorkertoPopulate.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;
#endif
            if (this.distinctCollection != null)
                this.distinctCollection = null;

            if (FilterPopUp != null)
            {
                FilterPopUp.Opened -= OnFilterPopUpOpened;
#if !WPF
                FilterPopUpBorder.KeyDown -= OnFilterPopUpBorderKeyDown;
#endif
#if !WinRT
                FilterPopUp.RemoveHandler(Popup.KeyDownEvent, (KeyEventHandler)OnKeyDown);
#endif
            }

            if (OkButton != null)
                OkButton.Click -= OnFilterOkButtonClick;

            if (CancelButton != null)
                CancelButton.Click -= OnCancelButtonClick;

            if (Part_AdvancedFilterButton != null)
                Part_AdvancedFilterButton.Click -= OnAdvancedFiltersButtonClick;

            if (Part_ClearFilterButton != null)
                Part_ClearFilterButton.Click -= OnClearFilterClick;

            if (PART_SortAscendingButton != null)
                PART_SortAscendingButton.Click -= OnSortAscendingButtonClick;

            if (PART_SortDescendingButton != null)
                PART_SortDescendingButton.Click -= OnSortDescendingButtonClick;

            if (this.ResizingThumb != null)
            {
#if WinRT
                this.ResizingThumb.PointerEntered -= OnResizingThumbEntered;
                this.ResizingThumb.PointerMoved -= OnResizingThumbMoved;
                this.ResizingThumb.PointerExited -= OnResizingThumbExited;
                this.ResizingThumb.PointerReleased -= OnResizingThumbReleased;
#else
                this.ResizingThumb.DragDelta -= OnResizingThumbMoved;
#endif
            }
#if WinRT

            await ThreadPool.RunAsync(delegate(IAsyncAction operation)
            {
                if (this.CheckboxFilterControl != null && this.CheckboxFilterControl.FilterListBoxItem != null)
                    CheckboxFilterControl.FilterListBoxItem.ForEach(lstItem => lstItem.PropertyChanged -= this.OnFilterElementPropertyChanged);
            }, WorkItemPriority.Normal);

#else
            if (this.CheckboxFilterControl != null && this.CheckboxFilterControl.FilterListBoxItem != null)
                CheckboxFilterControl.FilterListBoxItem.ForEach(lstItem => lstItem.PropertyChanged -= this.OnFilterElementPropertyChanged);
#endif
            this.Unloaded -= OnFilterDropDownUnloaded;
        }

        /// <summary>
        /// OnFilterDropDownUnloaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.RoutedEventArgs">RoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        void OnFilterDropDownUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnWireEvents();
        }
        #endregion

        #region Popup State Listner

        /// <summary>
        /// Called when [filter pop up opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnFilterPopUpOpened(object sender, object e)
        {
            if ((Column != null && Column.DataGrid != null && !Column.DataGrid.Validations.CheckForValidation(true)))
                ((Popup)sender).IsOpen = false;

#if SILVERLIGHT
            var popupAncestor = FindPopupAncestor((Popup)sender);
            if (popupAncestor == null)
            { return; }
            //add click event to the parent
            popupAncestor.AddHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
#endif

        }

#if SILVERLIGHT
        private static FrameworkElement FindPopupAncestor(Popup popup)
        {
            var ancestor = (FrameworkElement)popup;

            while (true)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(ancestor) as FrameworkElement;

                if (parent == null)
                { return ancestor; }

                ancestor = parent;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = GridUtil.FindDescendant(e.OriginalSource, typeof(ToggleButton)) as ToggleButton;
            if (parent == null || parent.Tag == null || parent.Tag.GetType() != typeof(GridHeaderCellControl) || (!(bool)parent.IsChecked && this.IsOpen))
                this.IsOpen = false;
        }

#endif

#if !WinRT

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (this.IsOpen)
                        this.IsOpen = false;
                    break;
            }
        }

#endif
        #endregion

        #region Clear Filter

        /// <summary>
        /// Called when [clear filter clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnClearFilterClick(object sender, RoutedEventArgs e)
        {
            if (this.Column != null)
            {
                this.Column.DataGrid.GridModel.ClearFilters(this.Column);
                this.Column.DataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Filtering, null));
            }
            this.IsOpen = false;
            this.FilteredFrom = FilteredFrom.None;
            this.CheckboxFilterControl.FilterListBoxItem = null;
            this.CheckboxFilterControl.FilterListBoxItem = new List<FilterElement>();
        }

        #endregion

        #region Button Click Listner

        /// <summary>
        /// Called when [cancel button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
#if WinRT
            this.Column.DataGrid.Focus(FocusState.Programmatic);
#endif
#if WPF
            this.Column.DataGrid.Focus();
#endif
        }

        /// <summary>
        /// Called when [AdvancedFilter button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnAdvancedFiltersButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.FilterMode == FilterMode.Both)
            {
                IsAdvancedFilterVisible = !IsAdvancedFilterVisible;
                if (IsAdvancedFilterVisible && FilteredFrom == FilteredFrom.CheckboxFilter)
                    this.AdvancedFilterControl.ResetAdvancedFilterControlValues();
                ResetVisbleFilteringControl();
            }
        }

        /// <summary>
        /// Called when [ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnFilterOkButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsAdvancedFilterVisible)
            {
                var error1 = this.AdvancedFilterControl["FilterValue1"];
                var error2 = this.AdvancedFilterControl["FilterValue2"];
                if (!string.IsNullOrEmpty(error1) || !string.IsNullOrEmpty(error2))
                    return;
            }
            this.InvokeFilter();
            this.IsOpen = false;
        }

        /// <summary>
        /// Called when Ascending Sort button click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.RoutedEventArgs">RoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if WinRT
        async void OnSortAscendingButtonClick(object sender, RoutedEventArgs e)
#else
        void OnSortAscendingButtonClick(object sender, RoutedEventArgs e)
#endif
        {
#if WinRT
            this.Column.DataGrid.SetBusyState("Busy");
            await Task.Delay(100);
#elif WPF
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ae) =>
            {
                Thread.Sleep(50);
                Dispatcher.Invoke(new Action(() =>
                {
#endif
                    SortAsceding();
#if WinRT

            this.Column.DataGrid.SetBusyState("Normal");
#elif WPF
                }));

            };
            worker.RunWorkerCompleted += (obj, args) => { this.Column.DataGrid.SetBusyState("Normal"); };
            this.Column.DataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#endif
        }

        /// <summary>
        /// Called when Descending Sort button click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.RoutedEventArgs">RoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if WinRT
        async void OnSortDescendingButtonClick(object sender, RoutedEventArgs e)
#else
        void OnSortDescendingButtonClick(object sender, RoutedEventArgs e)
#endif
        {
#if WinRT
            this.Column.DataGrid.SetBusyState("Busy");
            await Task.Delay(100);
#elif WPF
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ae) =>
            {
                Thread.Sleep(50);
                Dispatcher.Invoke(new Action(() =>
                {
#endif
                    SortDescending();
#if WinRT

            this.Column.DataGrid.SetBusyState("Normal");
#elif WPF
                }));

            };
            worker.RunWorkerCompleted += (obj, args) => { this.Column.DataGrid.SetBusyState("Normal"); };
            this.Column.DataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#endif
        }
        #endregion

        #region Property Change Listner

        /// <summary>
        /// Called when [filter element property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnFilterElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!CheckboxFilterControl.propertyChangedFromSelectAll)
            {
                if (this.ImmediateUpdateColumnFilter)
                    this.InvokeFilter();
                this.CheckboxFilterControl.MaintainSelectAllCheckBox();
            }
        }

        #endregion

        #region Methods
#if !WPF
        private void SetFilteredFromVisibility()
        {
            if (FilteredFrom == FilteredFrom.AdvancedFilter && FilterMode == FilterMode.Both)
                FilteredFromVisibility = Visibility.Visible;
            else
                FilteredFromVisibility = Visibility.Collapsed;
        }
#endif
        private void SetFilterColumnType()
        {
            if (this.AdvancedFilterType == AdvancedFilterType.TextFilter)
                this.FilterColumnType = GridResourceWrapper.TextFilters;
            else if (this.AdvancedFilterType == AdvancedFilterType.NumberFilter)
                this.FilterColumnType = GridResourceWrapper.NumberFilters;
            else
                this.FilterColumnType = GridResourceWrapper.DateFilters;
        }

        private AdvancedFilterType GetAdvancedFilterType()
        {

#if !WP
            if (this.Column.DataGrid.View.IsDynamicBound || this.Column.IsUnbound)
                return AdvancedFilterType.TextFilter;
#endif
            if (this.Column.FilterBehavior == FilterBehavior.StronglyTyped)
            {
                var pdc = this.Column.DataGrid.View.GetItemProperties();
                var pd = pdc.GetPropertyDescriptor(this.Column.MappingName);
                if (pd == null)
                    return AdvancedFilterType.TextFilter;
                var columnType = pd.PropertyType;

                if (columnType == typeof(int) || columnType == typeof(Double) || columnType == typeof(Decimal) || columnType == typeof(int?) || columnType == typeof(double?) || columnType == typeof(decimal?) || columnType == typeof(long) || columnType == typeof(long?) || columnType == typeof(uint) || columnType == typeof(uint?) || columnType == typeof(byte) || columnType == typeof(byte?) || columnType == typeof(float)
                   || columnType == typeof(float?) || columnType == typeof(sbyte) || columnType == typeof(sbyte?) || columnType == typeof(ulong) || columnType == typeof(ulong?) || columnType == typeof(short) || columnType == typeof(short?) || columnType == typeof(ushort) || columnType == typeof(ushort?))
                    return AdvancedFilterType.NumberFilter;
                else if (columnType == typeof(DateTime) || columnType == typeof(DateTime?) || columnType == typeof(TimeSpan) || columnType == typeof(TimeSpan?))
                    return AdvancedFilterType.DateFilter;
                else
                    return AdvancedFilterType.TextFilter;
            }
            else
                return AdvancedFilterType.TextFilter;
        }

        internal void InvokeFilter()
        {
#if WPF
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Thread.Sleep(50);
                this.Dispatcher.Invoke(new Action(ApplyFilters));
            };
            worker.RunWorkerCompleted += (o, args) => this.Column.DataGrid.SetBusyState("Normal");
            this.Column.DataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#else
            ApplyFilters();
#endif
        }

#if WinRT
        private async void ApplyFilters()
#else
        private void ApplyFilters()
#endif
        {
            this.Column.DataGrid.GridModel.FilterSuspend = true;
            HasSearchedItems = CheckboxFilterControl.searchedItems.Any();
            var source = (CheckboxFilterControl.ItemsSource as IEnumerable<FilterElement>);
#if WinRT
            this.Column.DataGrid.SetBusyState("Busy");
            await ThreadPool.RunAsync(delegate(IAsyncAction operation)
            {
#endif
            checkedItemsCount = source.Count(x => x.IsSelected);
            unCheckedItemsCount = this.CheckboxFilterControl.FilterListBoxItem.Count() - checkedItemsCount;
#if WinRT
            }, WorkItemPriority.Normal);
#endif

            this.RaiseOkButtonClick(this.CheckboxFilterControl.FilterListBoxItem.Where(x => x.IsSelected), this.CheckboxFilterControl.FilterListBoxItem.Where(x => !x.IsSelected));
#if WinRT
            if (!this.AdvancedFilterControl.CanGenerateUniqueItems)
            {
                if (AdvancedFilterControl.FilterValue1 != null && TypeConverterHelper.CanConvert(typeof(DateTime), AdvancedFilterControl.FilterValue1.ToString()))
                {
                    DateTime dt = Convert.ToDateTime(AdvancedFilterControl.FilterValue1);
                    AdvancedFilterControl.FilterValue1 = dt.Date;
                    AdvancedFilterControl.DateFilterValue1 = dt.Date;
                }
                if (AdvancedFilterControl.FilterValue2 != null && TypeConverterHelper.CanConvert(typeof(DateTime), AdvancedFilterControl.FilterValue2.ToString()))
                {
                    DateTime dt = Convert.ToDateTime(AdvancedFilterControl.FilterValue2);
                    AdvancedFilterControl.FilterValue2 = dt.Date;
                    AdvancedFilterControl.DateFilterValue2 = dt.Date;
                }
            }
#endif

            var ea = new OkButtonClikEventArgs()
            {
                FilterType1 = AdvancedFilterControl.FilterType1,
                FilterType2 = AdvancedFilterControl.FilterType2,
                FilterValue1 = AdvancedFilterControl.GetFirstFilterValue(),
                FilterValue2 = AdvancedFilterControl.GetSecondFilterValue(),
                ColumnType = AdvancedFilterControl.gridFilterCtrl.AdvancedFilterType,
                IsCaseSensitive1 = AdvancedFilterControl.IsCaseSensitive1,
                IsCaseSensitive2 = AdvancedFilterControl.IsCaseSensitive2,
                PredicateType = AdvancedFilterControl.IsORChecked ? "OR" : "AND"
            };

            bool CheckboxFilter = !IsAdvancedFilterVisible;

#if WinRT
            if (CheckboxFilter)
                await ThreadPool.RunAsync(operation => this.CreateFilterPredicates(source), WorkItemPriority.Normal);
            else
                await ThreadPool.RunAsync(operation => this.CreateAdvancedFilterPredicates(ea), WorkItemPriority.Normal);
            this.RefreshFilter();
            this.Column.DataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Filtering, null));
#else

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                if (CheckboxFilter)
                    CreateFilterPredicates(source);
                else
                    CreateAdvancedFilterPredicates(ea);
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                this.RefreshFilter();
                this.Column.DataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Filtering, null));
            };
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
#endif

            this.Column.DataGrid.GridModel.FilterSuspend = false;
#if WinRT
            this.Column.DataGrid.SetBusyState("Normal");
#endif
        }

        private void SortAsceding()
        {
            if (this.Column.DataGrid.View != null)
            {
                this.Column.DataGrid.GridModel.MakeSort(Column, ListSortDirection.Ascending);
            }
            this.IsOpen = false;
        }

        private void SortDescending()
        {
            if (this.Column.DataGrid.View != null)
            {
                this.Column.DataGrid.GridModel.MakeSort(Column, ListSortDirection.Descending);
            }
            this.IsOpen = false;
        }

#if WinRT
        private async void CreateFilterPredicates(IEnumerable<FilterElement> source)
#else
        private void CreateFilterPredicates(IEnumerable<FilterElement> source)
#endif
        {
#if !Silverlight4
            var sourceHashset = new HashSet<object>(source);
#endif
            if (unCheckedItemsCount == 0 && !HasSearchedItems)
            {
                if (filterPredicate == null)
                    filterPredicate = new List<FilterPredicate>();
                if (filterPredicate.Count > 0)
                    filterPredicate.Clear();
            }
            else
            {
                if (checkedItemsCount > unCheckedItemsCount && unCheckedItemsCount > 0)
                {
                    filterPredicate = source.Where(x => !x.IsSelected).Select(x => new FilterPredicate()
                    {
                        FilterBehavior = FilterBehavior.StronglyTyped,
                        FilterType = FilterType.NotEquals,
                        FilterValue = x.ActualValue,
                        IsCaseSensitive = true,
                        PredicateType = PredicateType.And
                    }).ToList();

                    if (CheckboxFilterControl.isSourceChangedasSearchedItems)
                    {
                        this.CheckboxFilterControl.FilterListBoxItem.ForEach((o) =>
                        {
#if Silverlight4
                                if (!source.Contains(o))
#else
                            if (!sourceHashset.Contains(o))
#endif
                            {
                                filterPredicate.Add(new FilterPredicate()
                                {
                                    FilterBehavior = FilterBehavior.StronglyTyped,
                                    FilterType = FilterType.NotEquals,
                                    FilterValue = o.ActualValue,
                                    IsCaseSensitive = true,
                                    PredicateType = PredicateType.And
                                });
                            }
                        });
                    }
                }
                else
                {
                    filterPredicate = source.Where(x => x.IsSelected).Select(x => new FilterPredicate()
                    {
                        FilterBehavior = FilterBehavior.StronglyTyped,
                        FilterType = FilterType.Equals,
                        FilterValue = x.ActualValue,
                        IsCaseSensitive = true,
                        PredicateType = PredicateType.Or
                    }).ToList();
                }

                var flag = Column.DataGrid.View.FilterPredicates.Any(x => !x.Equals(Column) && x.FilterPredicates.Any());

                if (Column.FilterPredicates.Any() && flag && filterPredicate.Any())
                {
                    var isTypeEqual = filterPredicate.FirstOrDefault().FilterType == column.FilterPredicates.FirstOrDefault().FilterType;

                    if (isTypeEqual)
                    {
                        foreach (var fp in Column.FilterPredicates)
                        {
                            var fpElement =
                                source.FirstOrDefault(
                                    x =>
                                    (x.ActualValue != null && x.ActualValue.Equals(fp.FilterValue)) ||
                                    (x.ActualValue == fp.FilterValue));

                            if (fpElement == null)
                                continue;

                            flag = filterPredicate.Any(
                                    x => (x.FilterValue != null && x.FilterValue.Equals(fp.FilterValue)) ||
                                         (x.FilterValue == fp.FilterValue));
                            if (flag)
                                continue;

                            if ((fp.FilterType == FilterType.NotEquals && !fpElement.IsSelected) ||
                                (fp.FilterType == FilterType.Equals && fpElement.IsSelected))
                            {
                                throw new NotImplementedException("FilterPredicate creation leads to crash");
                                //filterPredicate.Add(new FilterPredicate()
                                //    {
                                //        FilterBehavior = fp.FilterBehavior,
                                //        FilterType = fp.FilterType,
                                //        FilterValue = fp.FilterValue,
                                //        PredicateType = fp.PredicateType
                                //    });
                            }
                        }
                    }
                }
            }
            if (filterPredicate != null && filterPredicate.Count > 0)
            {
                filterPredicate[0].PredicateType = PredicateType.And;
#if WinRT
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    FilteredFrom = FilteredFrom.CheckboxFilter;
                });
#else
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    FilteredFrom = FilteredFrom.CheckboxFilter;
                }));
#endif
            }
        }
#if WinRT
        private async void CreateAdvancedFilterPredicates(OkButtonClikEventArgs args)
#else
        private void CreateAdvancedFilterPredicates(OkButtonClikEventArgs args)
#endif
        {
            this.AdvancedFilterControl.propertyChangedfromsettingControlValues = true;
            // Check for Empty value filtering
            if (args.FilterValue1 != null && args.FilterValue1.Equals(string.Empty) && args.FilterType1.ToString() != GridResourceWrapper.Empty)
            {
                if (filterPredicate != null)
                {
                    var canRemove = true;
                    FilterPredicate tempfilterPredicate = null;
                    foreach (var fp in filterPredicate)
                    {
                        var filterValue = fp.FilterValue;
#if SILVERLIGHT || WinRT

                        if (filterValue is FilterElement)
                            filterValue = (filterValue as FilterElement).DisplayText;
#endif
                        var filterType = FilterHelpers.GetResourceWrapper(fp.FilterType, filterValue);
                        var isCaseSensitive = fp.IsCaseSensitive;
                        if ((fp.FilterValue != null ? fp.FilterValue.Equals(args.FilterValue2) : fp.FilterValue == args.FilterValue2) && filterType == (args.FilterType2 != null ? args.FilterType2.ToString() : null))
                        {
                            tempfilterPredicate = fp;
                            canRemove = false;
                        }
                    }
                    filterPredicate.Clear();
                    if (!canRemove)
                        filterPredicate.Add(tempfilterPredicate);
                }
                this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
                return;
            }
            if (args.FilterValue2 != null && args.FilterValue2.ToString() == string.Empty && args.FilterType2.ToString() != GridResourceWrapper.Empty)
            {
                if (filterPredicate != null)
                {
                    var canRemove = true;
                    FilterPredicate tempfilterPredicate = null;
                    foreach (var fp in filterPredicate)
                    {
                        var filterValue = fp.FilterValue;
#if SILVERLIGHT || WinRT

                        if (filterValue is FilterElement)
                            filterValue = (filterValue as FilterElement).DisplayText;
#endif
                        var filterType = FilterHelpers.GetResourceWrapper(fp.FilterType, filterValue);
                        if ((fp.FilterValue != null ? fp.FilterValue.Equals(args.FilterValue1) : fp.FilterValue == args.FilterValue1) && filterType == (args.FilterType1 != null ? args.FilterType1.ToString() : null))
                        {
                            tempfilterPredicate = fp;
                            canRemove = false;
                        }
                    }
                    filterPredicate.Clear();
                    if (!canRemove)
                        filterPredicate.Add(tempfilterPredicate);
                }
                this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
                return;
            }

            bool canCreateFilterpredicate = false;

            //Text Filters
            if (args.ColumnType == AdvancedFilterType.TextFilter)
            {
                #region TextFilters

                var filterBehavior = FilterBehavior.StringTyped;
                if (this.AdvancedFilterControl.ColumnDataType == typeof(object))
                    filterBehavior = FilterBehavior.StronglyTyped;
                //Preicate 1

                if (args.FilterValue1 != null && args.FilterType1 != null)
                {
                    if (args.FilterValue1.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue1 = null;
                    canCreateFilterpredicate = true;
                }
                else if (args.FilterValue1 == null && args.FilterType1 != null)
                {
                    if (args.FilterType1.Equals(GridResourceWrapper.Null) || args.FilterType1.Equals(GridResourceWrapper.NotNull))
                    {
                        args.FilterValue1 = null;
                        canCreateFilterpredicate = true;
                    }

                    else if (args.FilterType1.Equals(GridResourceWrapper.Empty) || args.FilterType1.Equals(GridResourceWrapper.NotEmpty))
                    {
                        args.FilterValue1 = emptyStringValue;
                        canCreateFilterpredicate = true;
                    }
                }

                if (filterPredicate != null)
                    filterPredicate.Clear();
                else
                    filterPredicate = new List<FilterPredicate>();
                if (canCreateFilterpredicate)
                    filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), IsCaseSensitive = args.IsCaseSensitive1, FilterBehavior = filterBehavior, PredicateType = PredicateType.And });

                //If the filtering applied only in second option
                if (!filterPredicate.Any())
                    args.PredicateType = "And";

                //Preicate 2
                if (args.FilterValue2 != null && args.FilterValue2.ToString() == string.Empty && args.FilterType2.ToString() == GridResourceWrapper.Equalss)
                {
                    if (filterPredicate != null)
                        filterPredicate.Clear();
                    this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
                    return;
                }
                if (args.FilterValue2 != null && args.FilterType2 != null)
                {
                    if (args.FilterValue2.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue2 = null;

                    filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = args.IsCaseSensitive2, FilterBehavior = filterBehavior, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                }
                else if (args.FilterValue2 == null && args.FilterType2 != null)
                {
                    if (args.FilterType2.ToString() == GridResourceWrapper.Null || args.FilterType2.ToString() == GridResourceWrapper.NotNull)
                    {
                        args.FilterValue2 = null;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = args.IsCaseSensitive2, FilterBehavior = filterBehavior, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }
                    else if (args.FilterType2.ToString() == GridResourceWrapper.Empty || args.FilterType2.ToString() == GridResourceWrapper.NotEmpty)
                    {
                        args.FilterValue2 = emptyStringValue;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = args.IsCaseSensitive2, FilterBehavior = filterBehavior, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }
                }

                #endregion
            }
            else if (args.ColumnType == AdvancedFilterType.NumberFilter) //Number Filters
            {
                #region NumberFilters

                //Predicate 1
                if (filterPredicate != null)
                    filterPredicate.Clear();
                else
                    filterPredicate = new List<FilterPredicate>();

                if (args.FilterValue1 != null && args.FilterType1 != null)
                {
                    if (args.FilterValue1.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue1 = null;
                    var filter = new FilterPredicate()
                    {
                        FilterValue = args.FilterValue1,
                        FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()),
                        IsCaseSensitive = true,
                        FilterBehavior = FilterBehavior.StronglyTyped,
                        PredicateType = PredicateType.And
                    };
                    filterPredicate.Add(filter);
                }
                else if (args.FilterValue1 == null && args.FilterType1 != null)
                {
                    if (args.FilterType1.ToString() == GridResourceWrapper.Null || args.FilterType1.ToString() == GridResourceWrapper.NotNull)
                    {
                        args.FilterValue1 = null;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = PredicateType.And });
                    }
                }
                //If the filtering applied only in second option
                if (!filterPredicate.Any())
                    args.PredicateType = "And";
                //Predicate 2
                if (args.FilterValue2 != null && args.FilterType2 != null)
                {
                    if (args.FilterValue2.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue2 = null;
                    filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                }
                else if (args.FilterValue2 == null && args.FilterType2 != null)
                {
                    if (args.FilterType2.Equals(GridResourceWrapper.Null) || args.FilterType2.Equals(GridResourceWrapper.NotNull))
                    {
                        args.FilterValue2 = null;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }
                }

                #endregion
            }
            else if (args.ColumnType == AdvancedFilterType.DateFilter)
            {
                #region DateFilter

                if (filterPredicate != null)
                    filterPredicate.Clear();
                else
                    filterPredicate = new List<FilterPredicate>();

                if (args.FilterValue1 != null && args.FilterType1 != null)
                {
                    if (args.FilterValue1.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue1 = null;

                    filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = PredicateType.And });
                }
                else if (args.FilterValue1 == null && args.FilterType1 != null)
                {
                    if (args.FilterType1.Equals(GridResourceWrapper.Null) || args.FilterType1.Equals(GridResourceWrapper.NotNull))
                    {
                        args.FilterValue1 = null;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = PredicateType.And });
                    }
                }
                //If the filtering applied only in second option
                if (!filterPredicate.Any())
                    args.PredicateType = "And";
                //Predicate 2
                if (args.FilterValue2 != null && args.FilterType2 != null)
                {
                    if (args.FilterValue2.Equals(GridResourceWrapper.Blanks))
                        args.FilterValue2 = null;
                    filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                }
                else if (args.FilterValue2 == null && args.FilterType2 != null)
                {
                    if (args.FilterType2.Equals(GridResourceWrapper.Null) || args.FilterType2.Equals(GridResourceWrapper.NotNull))
                    {
                        args.FilterValue2 = null;
                        filterPredicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = true, FilterBehavior = FilterBehavior.StronglyTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }
                }
                #endregion
            }
            this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
            if (filterPredicate.Count > 0)
#if WinRT
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    FilteredFrom = FilteredFrom.AdvancedFilter;
                });
#else
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    FilteredFrom = FilteredFrom.AdvancedFilter;
                }));
#endif
        }

        private void RefreshFilter()
        {
            this.Column.DataGrid.GridModel.FilterColumn(this.Column, filterPredicate);
            if (this.Column.DataGrid.RowGenerator.Items.Count > 0)
            {
                var dataRow = this.Column.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.Column.DataGrid.GetHeaderIndex());
                var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == this.Column.MappingName && x.GridColumn.Equals(this.Column));
                var header = dataColumn.ColumnElement as GridHeaderCellControl;
                header.ApplyFilterToggleButtonVisualState();
            }
            this.MaintainClearFilterButtonEnable();
        }

        private void MaintainDataGridAPIChanges()
        {
            if (this.Column != null)
            {
                var dataGrid = this.Column.DataGrid;
                this.ImmediateUpdateColumnFilter = this.Column.ImmediateUpdateColumnFilter;
                this.AllowBlankFilters = this.Column.AllowBlankFilters;
                if (Column.FilterPopupStyle != null)
                    this.Style = Column.FilterPopupStyle;
                else if (dataGrid.FilterPopupStyle != null)
                    this.Style = dataGrid.FilterPopupStyle;

                if (Column.FilterPopupTemplate != null)
                    this.ContentTemplate = Column.FilterPopupTemplate;
                else if (dataGrid.FilterPopupTemplate != null)
                    this.ContentTemplate = dataGrid.FilterPopupTemplate;

            }
        }

        private void MaintainSortIndication()
        {
            if (this.PART_SortAscendingButton != null && this.PART_SortDescendingButton != null)
            {
                if (this.CanAllowSort())
                {
                    this.PART_SortAscendingButton.IsEnabled = true;
                    this.PART_SortDescendingButton.IsEnabled = true;
                }
                else
                {
                    this.PART_SortAscendingButton.IsEnabled = false;
                    this.PART_SortDescendingButton.IsEnabled = false;
                }
                if (this.Column.DataGrid.View != null && this.Column.DataGrid.View.SortDescriptions != null && this.Column.DataGrid.View.SortDescriptions.Count > 0
                        && this.Column.DataGrid.View.SortDescriptions.Any(x => x.PropertyName.Equals(this.Column.MappingName)))
                {
                    var sortColumn =
                        this.Column.DataGrid.View.SortDescriptions.FirstOrDefault(
                            x => x.PropertyName.Equals(this.Column.MappingName));
                    if (sortColumn != null && sortColumn.Direction == ListSortDirection.Ascending)
                    {
                        this.PART_SortAscendingButton.IsSorted = true;
                        this.PART_SortDescendingButton.IsSorted = false;
                    }
                    else if (sortColumn != null && sortColumn.Direction == ListSortDirection.Descending)
                    {
                        this.PART_SortAscendingButton.IsSorted = false;
                        this.PART_SortDescendingButton.IsSorted = true;
                    }
                    else
                    {
                        this.PART_SortAscendingButton.IsSorted = false;
                        this.PART_SortDescendingButton.IsSorted = false;
                    }
                }
                else
                {
                    this.PART_SortAscendingButton.IsSorted = false;
                    this.PART_SortDescendingButton.IsSorted = false;
                }
            }
        }

        private void MaintainClearFilterButtonEnable()
        {
            if (this.Part_ClearFilterButton != null && Column.FilterPredicates.Count == 0)
                this.Part_ClearFilterButton.IsEnabled = false;
            else if (this.Part_ClearFilterButton != null && Column.FilterPredicates.Count > 0)
                this.Part_ClearFilterButton.IsEnabled = true;
        }

        private bool CanAllowSort()
        {
            bool canSetAllowSort = false;
            var valueColumn = this.Column.ReadLocalValue(GridColumn.AllowSortingProperty);
            if (valueColumn != DependencyProperty.UnsetValue)
                canSetAllowSort = this.Column.AllowSorting;
            if ((valueColumn == DependencyProperty.UnsetValue) && this.Column.DataGrid.AllowSorting)
                canSetAllowSort = true;
            return canSetAllowSort;
        }

#if WinRT
        private async void InitializeGridFilterPane()
#else
        private void InitializeGridFilterPane()
#endif
        {
            if (this.FilterPopUp != null)
            {
                if (!IsOpen)
                {
#if WPF
                    // reseting the filterpopup placement on closing.
                    this.FilterPopUp.Placement = PlacementMode.Bottom;
#endif
                    this.distinctCollection = null;
                    return;
                }
#if WPF
                // check the filterpopup intersects with screen or not
                var screenRect = new Rect(0, 0, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
                var popupRect = this.Column.DataGrid.GridColumnDragDropController.GetControlRect(this.FilterPopUp);
                var currPopupWidth = popupRect.Width = 300;
                var currPopupHeight = popupRect.Height = 450;
                popupRect.Intersect(screenRect);
                // if intersects changing the placement mode for the filterpopup
                if (currPopupHeight > popupRect.Height)
                {
                    this.FilterPopUp.Placement = PlacementMode.Left;
                }
#endif
            }

            this.MaintainClearFilterButtonEnable();
            this.ResetVisualStates();
            this.MaintainDataGridAPIChanges();
            this.MaintainSortIndication();
            this.ResetPopupHeight();
            if (Column.FilterPredicates == null || !Column.FilterPredicates.Any())
                FilteredFrom = FilteredFrom.None;

            if (this.CheckboxFilterControl != null)
            {
                this.CheckboxFilterControl.MaintainAPIChanges();
                this.CheckboxFilterControl.IsItemSourceLoaded = false;
                this.CheckboxFilterControl.isSourceChangedasSearchedItems = false;
                if (this.CheckboxFilterControl.SearchTextBox != null && this.excelFilterView != null)
                {
                    this.CheckboxFilterControl.SearchTextBox.ClearValue(TextBox.TextProperty);
                    this.CheckboxFilterControl.searchedItems = new List<FilterElement>();
                    HasSearchedItems = false;
                }
                if (this.CheckboxFilterControl.PartScrollViewer != null)
                    this.CheckboxFilterControl.PartScrollViewer.ScrollToVerticalOffset(0);
                this.allowBlankFilters = this.AllowBlankFilters;
                var sourceFromEvent = this.RaisePopupOpened();
                if (sourceFromEvent != null && sourceFromEvent.Any())
                {
                    this.distinctCollection = sourceFromEvent.ToList();
                    this.CheckboxFilterControl.FilterListBoxItem = this.distinctCollection;
                    this.CheckboxFilterControl.ItemsSource = this.distinctCollection;
                }
                else
                {
                    var e = new GridFilterItemsPopulatingEventArgs(this.distinctCollection, this.Column, this, this.Column.DataGrid);
                    if (!this.Column.DataGrid.RaiseFilterListItemsPopulating(e))
                    {
                        if (e.ItemsSource != null && e.ItemsSource.Any())
                        {
                            this.distinctCollection = (e.ItemsSource as IEnumerable<FilterElement>).ToList();
                            this.CheckboxFilterControl.HasItemsSource = true;
                            this.SetItemSource();
                            return;
                        }
#if WinRT
                        if (this.CheckboxFilterControl != null)
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                this.GenerateItemSource();
                                if (this.CheckboxFilterControl.HasItemsSource)
                                    this.SetItemSource();
                            });
                        }

#elif WPF
                        if (this.CheckboxFilterControl != null && !this.bgWorkertoPopulate.IsBusy)
                            this.bgWorkertoPopulate.RunWorkerAsync();
#elif SILVERLIGHT
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.GenerateItemSource();
                            if (this.CheckboxFilterControl.HasItemsSource)
                                this.SetItemSource();
                        });
#endif
                    }
                }

#if WinRT
                this.Focus(FocusState.Programmatic);
#endif
            }

            if (this.AdvancedFilterControl != null)
            {
                this.AdvancedFilterControl.MaintainAPIChanges();
#if WPF
                this.AdvancedFilterControl.CasingButtonVisibility = this.Column.DataGrid.IsLegacyDataTable;
#endif
                this.AdvancedFilterControl.GenerateFilterTypeComboItems();
                this.AdvancedFilterControl.propertyChangedfromsettingControlValues = true;
                this.AdvancedFilterControl.ComboItemsSource = this.distinctCollection == null ? null : new ObservableCollection<FilterElement>(this.distinctCollection);
                this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
                if (this.AdvancedFilterControl.ComboItemsSource != null)
                {
                    var nullfilter = this.AdvancedFilterControl.ComboItemsSource.FirstOrDefault(filter => filter.ActualValue == null);
                    this.AdvancedFilterControl.ComboItemsSource.Remove(nullfilter);
                    var emptyfilter = this.AdvancedFilterControl.ComboItemsSource.FirstOrDefault(filter => filter.ActualValue != null && filter.ActualValue.Equals(string.Empty));
                    this.AdvancedFilterControl.ComboItemsSource.Remove(emptyfilter);
                }
                if (this.FilteredFrom == FilteredFrom.AdvancedFilter)
                {
                    if (this.FilterMode == FilterMode.Both)
                        IsAdvancedFilterVisible = true;
                    if (Column.FilterPredicates != null && Column.FilterPredicates.Any())
                        this.AdvancedFilterControl.SetAdvancedFilterControlValues(Column.FilterPredicates);
                    else
                        this.AdvancedFilterControl.ResetAdvancedFilterControlValues();
                }
                else
                {
                    if (this.FilterMode == FilterMode.Both)
                        IsAdvancedFilterVisible = false;
                    this.AdvancedFilterControl.ResetAdvancedFilterControlValues();
                }
                this.ResetVisbleFilteringControl();
            }
        }
#if !WPF

        internal void SetPopupPosition(Rect toggleRect, Point windowPoint, double togglePadding)
        {
            if (FilterPopUpBorder == null)
            {
                localToggleRect = toggleRect;
                localWindowPoint = windowPoint;
                localTogglepadding = togglePadding;
                return;
            }

            var actualHeight = FilterPopUpBorder.ActualHeight > 0 ? FilterPopUpBorder.ActualHeight : FilterPopUpBorder.MinHeight;
            var actualWidth = FilterPopUpBorder.ActualWidth > 0 ? FilterPopUpBorder.ActualWidth : FilterPopUpBorder.MinWidth;
#if WinRT
            var screenWidth = Window.Current.Bounds.Width;
            var screenHeight = Window.Current.Bounds.Height;
#elif SILVERLIGHT
            var screenWidth = Application.Current.RootVisual.DesiredSize.Width;
            var screenHeight = Application.Current.RootVisual.DesiredSize.Height;
#endif

            if (windowPoint.X + actualWidth > screenWidth)
                this.HorizontalOffset = (toggleRect.X + togglePadding) - actualWidth;
            else
                this.HorizontalOffset = toggleRect.X - togglePadding;
#if WinRT
            if (windowPoint.Y + actualHeight > screenHeight)
            {
                if (windowPoint.Y > actualHeight)
                    this.VerticalOffset = -actualHeight;
                else
                {
                    this.VerticalOffset = -actualHeight + (actualHeight - windowPoint.Y);
                    if (windowPoint.X + actualWidth > screenWidth)
                        this.HorizontalOffset = (toggleRect.X - togglePadding) - actualWidth;
                    else
                        this.HorizontalOffset = toggleRect.X + (toggleRect.Width - togglePadding);
                }
                if (this.FilterPopUp.ChildTransitions.Count > 0 && (this.FilterPopUp.ChildTransitions[0] is PaneThemeTransition) && (this.FilterPopUp.ChildTransitions[0] as PaneThemeTransition).Edge == EdgeTransitionLocation.Top)
                    (this.FilterPopUp.ChildTransitions[0] as PaneThemeTransition).Edge = EdgeTransitionLocation.Bottom;

            }
            else
            {
#endif
                this.VerticalOffset = toggleRect.Height;
#if WinRT
                if (this.FilterPopUp.ChildTransitions.Count > 0 && (this.FilterPopUp.ChildTransitions[0] is PaneThemeTransition) && (this.FilterPopUp.ChildTransitions[0] as PaneThemeTransition).Edge == EdgeTransitionLocation.Top)
                    (this.FilterPopUp.ChildTransitions[0] as PaneThemeTransition).Edge = EdgeTransitionLocation.Top;
            }
#endif

        }

#endif
        private void ResetVisualStates()
        {
            if (PART_SortAscendingButton != null && PART_SortAscendingButton.IsEnabled)
                VisualStateManager.GoToState(PART_SortAscendingButton, "Normal", false);
            if (PART_SortDescendingButton != null && PART_SortDescendingButton.IsEnabled)
                VisualStateManager.GoToState(PART_SortDescendingButton, "Normal", false);
            if (Part_ClearFilterButton != null && Part_ClearFilterButton.IsEnabled)
                VisualStateManager.GoToState(Part_ClearFilterButton, "Normal", false);
            if (OkButton != null && OkButton.IsEnabled)
                VisualStateManager.GoToState(OkButton, "Focused", false);
            if (CancelButton != null)
                VisualStateManager.GoToState(CancelButton, "Normal", false);
        }

        private void ResetPopupHeight()
        {
            minHeight = this.MinHeight;
            minWidth = this.MinWidth;
            this.FilterPopupWidth = filterPopupWidth;
            this.FilterPopupHeight = filterPopupHeight;
        }

        private void ResetVisbleFilteringControl()
        {
            if (IsAdvancedFilterVisible)
            {
                VisualStateManager.GoToState(Part_AdvancedFilterButton, "Collapsed", true);
                VisualStateManager.GoToState(this, "AdvancedFilter", true);
                OkButton.IsEnabled = true;
            }
            else
            {
                VisualStateManager.GoToState(Part_AdvancedFilterButton, "Expanded", true);
                VisualStateManager.GoToState(this, "CheckboxFilter", true);
                if (FilteredFrom == FilteredFrom.AdvancedFilter)
                {
                    if (CheckboxFilterControl.SelectAllCheckBox != null)
                        CheckboxFilterControl.SelectAllCheckBox.IsChecked = false;
                    OkButton.IsEnabled = false;
                }
            }
        }

        private void GenerateItemSource()
        {
            IEnumerable<object> viewRecords = null;
            IEnumerable virtualItems = null;
            if (this.Column.DataGrid.View is Syncfusion.Data.PagedCollectionView)
            {
                var items =
                    (this.Column.DataGrid.View as Syncfusion.Data.PagedCollectionView).GetInternalList()
                                                                                      .Cast<object>()
                                                                                      .ToList();
                if (!items.Any())
                {
#if WPF
                    this.Dispatcher.Invoke((Action)(() =>
                    {
#endif
                        CheckboxFilterControl.HasItemsSource = false;
#if WPF
                    }));
#endif
                    return;
                }
#if WPF
                this.Dispatcher.Invoke((Action)(() =>
                    {
#endif
                        if (this.Column.IsUnbound)
                        {
                            viewRecords = items.Select(
                                x => this.Column.DataGrid.GetUnBoundCellValue(this.Column, x))
                                               .Distinct();
                        }
                        else
                            viewRecords = items.Select(
                                x => this.provider.GetValue(x, this.Column.MappingName))
                                               .Distinct();
#if WPF
                    }));
#endif
            }
            else if (this.Column.DataGrid.View is VirtualizingCollectionView)
            {
                virtualItems = (this.Column.DataGrid.View as VirtualizingCollectionView).GetInternalSource();
                if (virtualItems != null)
                {
                    var items = virtualItems.Cast<object>();
                    if (!items.Any())
                    {
#if WPF
                        this.Dispatcher.Invoke((Action)(() =>
                        {
#endif
                            CheckboxFilterControl.HasItemsSource = false;
#if WPF
                        }));
#endif
                        return;
                    }
#if WPF
                    this.Dispatcher.Invoke((Action)(() =>
                        {
#endif
                            if (this.Column.IsUnbound)
                            {
                                viewRecords = items.Select(
                                    x => this.Column.DataGrid.GetUnBoundCellValue(this.Column, x))
                                                   .Distinct();
                            }
                            else
                                viewRecords = items.Select(
                                    x => this.provider.GetValue(x, this.Column.MappingName))
                                                   .Distinct();
#if WPF
                        }));
#endif
                }
                else
                {
#if WPF
                    this.Dispatcher.Invoke((Action)(() =>
                    {
#endif
                        CheckboxFilterControl.HasItemsSource = false;
#if WPF
                    }));
#endif
                    return;
                }
            }
            else
            {
                if (this.Column.DataGrid.View.SourceCollection == null ||
                    !this.Column.DataGrid.View.SourceCollection.Cast<object>().Any())
                {
#if WPF
                    this.Dispatcher.Invoke((Action)(() =>
                    {
#endif
                        CheckboxFilterControl.HasItemsSource = false;
#if WPF
                    }));
#endif
                    return;
                }
                var records = this.Column.DataGrid.View.Records;
#if WPF
                this.Dispatcher.Invoke((Action)(() =>
                    {
#endif
                        if (this.Column.IsUnbound)
                        {
                            viewRecords = records.Select(
                                x => this.Column.DataGrid.GetUnBoundCellValue(this.Column, x.Data))
                                                 .Distinct();
                        }
                        else
                            viewRecords = records.Select(
                                x => this.provider.GetValue(x.Data, this.colName))
                                                 .Distinct();
#if WPF
                    }));
#endif
            }
#if WPF
            this.Dispatcher.Invoke((Action)(() =>
                {
#endif
                    CheckboxFilterControl.HasItemsSource = true;

#if WPF
                }));

            this.Dispatcher.Invoke((Action)(() =>
                {
#endif
                    this.distinctCollection = viewRecords.Select(item =>
                                                                 new FilterElement
                                                                 {
                                                                     IsSelected = true,
                                                                     ActualValue = item,
                                                                     FormatedString = this.GetFormatedString
                                                                 }).ToList();
#if WPF
                }));
#endif

            if (Column.FilterPredicates.Count > 0)
            {
#if WPF
                this.Dispatcher.Invoke((Action)(() =>
                    {
                        if (!excelFilterView.IsLegacyDataTable) //!(excelFilterView is GridDataTableCollectionViewWrapper))
                        {
#endif
                            var view = excelFilterView as IFilterExt;
                            IEnumerable<object> distinctRecords = null;
                            System.Linq.Expressions.Expression columnPredicate = null;
                            ParameterExpression columnParamExpression = null;
                            paramExpression = null;
                            predicate = null;

                            var filteredRecords = this.Column.DataGrid.View is VirtualizingCollectionView
                                                      ? (this.Column.DataGrid.View as VirtualizingCollectionView)
                                                            .GetSourceListForFilteringItems().AsQueryable()
                                                      : this.Column.DataGrid.View.SourceCollection.AsQueryable();

                            columnPredicate = view.GetPredicateExpression(filteredRecords,
                                                                                     out columnParamExpression,
                                                                                     Column.MappingName, true);

                            filteredRecords = filteredRecords.Where(columnParamExpression,
                                                                    System.Linq.Expressions.Expression.Not(
                                                                        columnPredicate));
                            predicate = view.GetPredicateExpression(filteredRecords,
                                                                                     out paramExpression,
                                                                                     Column.MappingName, false);
                            if (predicate != null)
                                filteredRecords = filteredRecords.Where(paramExpression, predicate);

                            if (this.Column.IsUnbound)
                            {
                                distinctRecords = filteredRecords.Cast<object>().Select(
                                    x => this.Column.DataGrid.GetUnBoundCellValue(this.Column, x))
                                                                 .Distinct();
                            }
                            else
                                distinctRecords =
                                    filteredRecords.Cast<object>().Select(x => provider.GetValue(x, colName)).Distinct();

                            distinctRecords.ForEach((o) => this.distinctCollection.Add(new FilterElement()
                            {
                                ActualValue = o,
                                FormatedString = this.GetFormatedString
                            }));
                        }
#if WPF
                        else
                        {
                            PropertyDescriptorCollection clonedItemsProperties = null;
                            var view = excelFilterView as DataTableCollectionView;
                            var source = view.GetClonedSource().DefaultView;
                            clonedItemsProperties = ((ITypedList)(source)).GetItemProperties(null);

                            var columnFilterString = view.GetFilterString(colName, true);
                            var filterString = view.GetFilterString(colName, false);

                            columnFilterString = "NOT(" + columnFilterString + ")";

                            if (!string.IsNullOrEmpty(filterString))
                                filterString = ("(" + columnFilterString + ")").AndPredicate() + filterString;
                            else
                                filterString = columnFilterString;

                            source.RowFilter = filterString;

                            var distinctRecords =
                                source.Cast<object>().Select(x => clonedItemsProperties.GetValue(x, colName)).Distinct();
                            distinctRecords.ForEach((o) => this.distinctCollection.Add(new FilterElement()
                                {
                                    ActualValue = o,
                                    FormatedString = this.GetFormatedString
                                }));
                        }

                    }));
            }
#endif
            if (!this.allowBlankFilters)
                this.distinctCollection = this.distinctCollection.Where(x => x.ActualValue != null).ToList();

            this.distinctCollection.Sort(new FilterElementAscendingOrder());
        }

        private void SetItemSource()
        {
            if (this.Column != null)
            {
                var args = new GridFilterItemsPopulatedEventArgs(this.distinctCollection, this.Column, this, this.Column.DataGrid);
                this.Column.DataGrid.RaiseFilterListItemsPopulated(args);
                this.distinctCollection = args.ItemsSource != null ? args.ItemsSource.ToList() : null;
            }

            if (this.distinctCollection != null)
                this.distinctCollection.ForEach(lstItem => lstItem.PropertyChanged += this.OnFilterElementPropertyChanged);
            this.CheckboxFilterControl.FilterListBoxItem = this.distinctCollection;

            this.CheckboxFilterControl.ItemsSource = this.distinctCollection;
            this.AdvancedFilterControl.propertyChangedfromsettingControlValues = true;
            this.AdvancedFilterControl.ComboItemsSource = this.distinctCollection == null ? null : new ObservableCollection<FilterElement>(this.distinctCollection);
            this.AdvancedFilterControl.propertyChangedfromsettingControlValues = false;
            if (this.AdvancedFilterControl.ComboItemsSource != null)
            {
                var nullfilter = this.AdvancedFilterControl.ComboItemsSource.FirstOrDefault(filter => filter.ActualValue == null);
                this.AdvancedFilterControl.ComboItemsSource.Remove(nullfilter);
                var emptyfilter = this.AdvancedFilterControl.ComboItemsSource.FirstOrDefault(filter => filter.ActualValue != null && filter.ActualValue.Equals(string.Empty));
                this.AdvancedFilterControl.ComboItemsSource.Remove(emptyfilter);
                if (Column.FilterPredicates != null && Column.FilterPredicates.Any() && this.FilteredFrom == FilteredFrom.AdvancedFilter)
                    this.AdvancedFilterControl.SetAdvancedFilterControlValues(Column.FilterPredicates);
            }
            this.CheckboxFilterControl.MaintainSelectAllCheckBox();
            this.CheckboxFilterControl.IsItemSourceLoaded = true;
#if !WinRT
            this.Focus();
#endif
        }

        public static string DBNullString = "DBNull";
        internal string GetFormatedString(object item)
        {
            if (this.Column.DisplayBinding.Converter != null)
            {
#if WinRT || WP
                var value = this.Column.DisplayBinding.Converter.Convert(item, typeof(object), this.Column.DisplayBinding.ConverterParameter, null);
#else

                var value = this.Column.DisplayBinding.Converter.Convert(item, typeof(object), this.Column.DisplayBinding.ConverterParameter, Thread.CurrentThread.CurrentCulture);
#endif
                if (value != null)
                    return value.ToString();
            }
#if !WinRT
            if (item != null && item is DateTime)
            {
                if (!String.IsNullOrEmpty(Column.DisplayBinding.StringFormat))
                    return Convert.ToDateTime(item).ToString(Column.DisplayBinding.StringFormat);
                return item.ToString();
            }

            if (item != null && item != DBNull.Value)
            {
                if (!String.IsNullOrEmpty(Column.DisplayBinding.StringFormat))
                    return string.Format(Column.DisplayBinding.StringFormat, item);
                return item.ToString();
            }
            if (item is DBNull)
                return DBNullString;
#else
            if (item != null)
                return item.ToString();
#endif
            return GridResourceWrapper.Blanks;
        }
        #endregion

        #region Events

        #region OkButtonClick

        public event OkButtonClickEventHandler OkButtonClick;

        /// <summary>
        /// Raises the ok button click.
        /// </summary>
        internal void RaiseOkButtonClick(IEnumerable<FilterElement> checkedElement, IEnumerable<FilterElement> unCheckedElement)
        {
            OnOkButtonClick(new OkButtonClikEventArgs(this) { UnCheckedElements = unCheckedElement, CheckedElements = checkedElement });
        }

        /// <summary>
        /// Raises the <see cref="OkButtonClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OkButtonClikEventArgs"/> instance containing the event data.</param>
        private void OnOkButtonClick(OkButtonClikEventArgs e)
        {
            if (this.OkButtonClick != null)
                this.OkButtonClick(this, e);
        }

        #endregion

        #region PopupOpened

        public event PopupOpenedEventHandler PopupOpened;

        /// <summary>
        /// Raises the popup opened.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<FilterElement> RaisePopupOpened()
        {
            return OnPopupOpened(new PopupOpenedEventArgs());
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private IEnumerable<FilterElement> OnPopupOpened(PopupOpenedEventArgs e)
        {
            if (PopupOpened != null)
                PopupOpened(this, e);

            return e.ItemsSource;
        }

        #endregion

        #region OnFilterElementPropertyChanged

        public event OnFilterElementPropertyChangedEventHandler OnFilterElementChanged;

        /// <summary>
        /// Raises the on filter element property changed.
        /// </summary>
        /// <param name="FilterElement">The filter element.</param>
        /// <param name="SelectAllChecked">The select all checked.</param>
        internal void RaiseOnFilterElementPropertyChanged(FilterElement FilterElement, Nullable<bool> SelectAllChecked)
        {
            FilterElementPropertyChanged(new OnFilterElementPropertyChangedEventArgs() { FilterElement = FilterElement, SelectAllChecked = SelectAllChecked });
        }

        /// <summary>
        /// Filters the element property changed.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OnFilterElementPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void FilterElementPropertyChanged(OnFilterElementPropertyChangedEventArgs e)
        {
            if (this.OnFilterElementChanged != null)
                this.OnFilterElementChanged(this, e);
        }

        #endregion

        #region Reszing

#if WinRT
        void OnResizingThumbMoved(object sender, MouseEventArgs e)
#else
        void OnResizingThumbMoved(object sender, DragDeltaEventArgs e)
#endif
        {

#if WinRT
            this.ResizingThumb.CapturePointer(e.Pointer);
            PointerPoint pt = e.GetCurrentPoint(FilterPopUpBorder);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 1);
            Point point = pt.Position;
            if (pt.Properties.IsLeftButtonPressed)
            {
                var actualWidth = point.X;
                var actualHeight = point.Y;
#else
            {
                var actualWidth = this.FilterPopupWidth + e.HorizontalChange;
                var actualHeight = this.FilterPopupHeight + e.VerticalChange;
#endif
                if (!(actualWidth > this.MaxWidth || actualWidth < this.MinWidth) && actualWidth >= 0)
                {
                    isResizing = true;
                    this.FilterPopupWidth = actualWidth;
                    isResizing = false;
                }
                if (!(actualHeight > this.MaxHeight || actualHeight < this.MinHeight) && actualHeight >= 0)
                {
                    isResizing = true;
                    this.FilterPopupHeight = actualHeight;
                    isResizing = false;
                }
                return;
            }
        }

#if WinRT
        void OnResizingThumbExited(object sender, MouseEventArgs e)
        {
            this.ResizingThumb.ReleasePointerCapture(e.Pointer);
            if (Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        void OnResizingThumbEntered(object sender, MouseEventArgs e)
        {
            this.ResizingThumb.CapturePointer(e.Pointer);
            if (Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.SizeNorthwestSoutheast)
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 1);
        }

        void OnResizingThumbReleased(object sender, MouseButtonEventArgs e)
        {
            this.ResizingThumb.ReleasePointerCapture(e.Pointer);
            if (Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }
#endif

        #endregion

        #region OnELFControlLoaded

        void OnGridFilterControlLoaded(object sender, RoutedEventArgs e)
        {
            if (this.FilterPopUp != null)
            {
                WireEvents();
                InitializeGridFilterPane();
            }
        }

        #endregion

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            FilterPopUp = this.GetTemplateChild("PART_FilterPopup") as Popup;
            OkButton = this.GetTemplateChild("PART_OkButton") as Button;
            CancelButton = this.GetTemplateChild("PART_CancelButton") as Button;
            CheckboxFilterControl = this.GetTemplateChild("PART_CheckboxFilterControl") as CheckboxFilterControl;
            Part_ClearFilterButton = this.GetTemplateChild("PART_ClearFilterButton") as Button;
            PART_SortAscendingButton = this.GetTemplateChild("PART_SortAscendingButton") as SortButton;
            PART_SortDescendingButton = this.GetTemplateChild("PART_SortDescendingButton") as SortButton;
            ResizingThumb = this.GetTemplateChild("PART_ThumbGripper") as Thumb;
#if !WPF
            FilterPopUpBorder = this.GetTemplateChild("PART_FilterPopUpBorder") as Border;
#endif
            Part_AdvancedFilterButton = this.GetTemplateChild("PART_AdvancedFilterButton") as Button;
            AdvancedFilterControl = this.GetTemplateChild("PART_AdvancedFilterControl") as AdvancedFilterControl;
            AdvancedFilterControl.gridFilterCtrl = this;
            CheckboxFilterControl.gridFilterCtrl = this;
            if (Column != null && AdvancedFilterControl != null)
            {
#if WPF || WinRT || SILVERLIGHT
                if (Column.IsUnbound || Column.DataGrid.View.IsDynamicBound)
                    AdvancedFilterControl.ColumnDataType = typeof(string);
#else
                if (Column.IsUnbound)
                    AdvancedFilterControl.ColumnDataType = typeof(string);
#endif
                else
                {
                    var pdc = this.Column.DataGrid.View.GetItemProperties();
                    var pd = pdc.GetPropertyDescriptor(this.Column.MappingName);
                    if (pd == null)
                        AdvancedFilterControl.ColumnDataType = typeof(string);
                    else
                        AdvancedFilterControl.ColumnDataType = pd.PropertyType;
                }
            }
#if WPF
            FilterPopUp.StaysOpen = true;
#endif

#if SILVERLIGHT
            WireEvents();
            InitializeGridFilterPane();
#endif

#if !WPF
            if (FilterPopUp != null && localToggleRect != null)
                SetPopupPosition(localToggleRect, localWindowPoint, localTogglepadding);
#endif

        }
#if WinRT
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            base.OnPointerMoved(e);

        }
#else
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Arrow)
                this.Cursor = Cursors.Arrow;
            base.OnMouseMove(e);
        }
#endif
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if WPF
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
#endif
            switch (e.Key)
            {
                case Key.Escape:
                    if (this.IsOpen)
                        this.IsOpen = false;
                    break;
#if SILVERLIGHT
                case Key.Tab:
                case Key.Up:
                case Key.Down:
                    this.PART_SortAscendingButton.Focus();
                    e.Handled = true;
                    break;
#endif
#if WPF
                case Key.Up:
                    if (this.FilterPopUp.IsKeyboardFocusWithin)
                    {
                        focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                        e.Handled = true;
                        return;
                    }
                    this.FilterPopUp.Child.Focus();
                    break;

                case Key.Down:
                    if (this.FilterPopUp.IsKeyboardFocusWithin)
                    {
                        focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                        return;
                    }
                    this.FilterPopUp.Child.Focus();
                    break;

                case Key.Tab:
                    if (this.FilterPopUp.IsKeyboardFocusWithin)
                        return;

                    this.FilterPopUp.Child.Focus();
                    var tabFocusedElement = Keyboard.FocusedElement as UIElement;
                    tabFocusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    e.Handled = true;
                    break;
#endif
#if WinRT
                case Key.Tab:
                    SetFocus();
                    e.Handled = true;
                    break;
                case Key.Up:
                    SetFocus();
                    e.Handled = true;
                    break;
                case Key.Down:
                    SetFocus();
                    e.Handled = true;
                    break;
#endif
            }
            base.OnKeyDown(e);
        }
#if WinRT
        private void SetFocus()
        {
            if (filterPopUpChildControls.Count == 0)
                GridUtil.GetNavigatableDescendants(this.FilterPopUpBorder, ref filterPopUpChildControls);
            foreach (var control in filterPopUpChildControls)
            {
                if (control.IsEnabled)
                {
                    control.Focus(FocusState.Programmatic);
                    break;
                }
            }
        }
#endif
#if !WPF
        void OnFilterPopUpBorderKeyDown(object sender, KeyEventArgs e)
        {

            Control focusedElement = FocusManager.GetFocusedElement() as Control;

            if (filterPopUpChildControls.Count == 0)
                GridUtil.GetNavigatableDescendants(this.FilterPopUpBorder, ref filterPopUpChildControls);

            switch (e.Key)
            {
                case Key.Escape:
                    if (this.IsOpen)
                        this.IsOpen = false;
                    break;
                case Key.Up:
                    {
                        var previousSiblingIndex = filterPopUpChildControls.IndexOf(focusedElement);
                        previousSiblingIndex = --previousSiblingIndex < 0 ? filterPopUpChildControls.Count - 1 : previousSiblingIndex;
                        var nextFocusableElement = filterPopUpChildControls[previousSiblingIndex].IsEnabled ? filterPopUpChildControls[previousSiblingIndex] : filterPopUpChildControls[--previousSiblingIndex];
#if WinRT
                        ((Control)nextFocusableElement).Focus(FocusState.Keyboard);
#elif SILVERLIGHT
                        ((Control)nextFocusableElement).Focus();
#endif
                        e.Handled = true;
                    }

                    break;

                case Key.Down:
                    {
                        var nextSiblingIndex = filterPopUpChildControls.IndexOf(focusedElement);
                        nextSiblingIndex = ++nextSiblingIndex < filterPopUpChildControls.Count ? nextSiblingIndex : 0;
                        var nextFocusableElement = filterPopUpChildControls[nextSiblingIndex].IsEnabled ? filterPopUpChildControls[nextSiblingIndex] : filterPopUpChildControls[++nextSiblingIndex];
#if WinRT
                        ((Control)nextFocusableElement).Focus(FocusState.Keyboard);
#elif SILVERLIGHT
                        ((Control)nextFocusableElement).Focus();
#endif
                        e.Handled = true;
                    }
                    break;
            }
        }
#endif

#if WPF
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseLeftButtonUp(e);
        }
#endif

        #endregion

        #region Dispose

        public void Dispose()
        {
            UnWireEvents();
            if (CheckboxFilterControl != null)
            {
                CheckboxFilterControl.Dispose();
                CheckboxFilterControl = null;
            }
            if (AdvancedFilterControl != null)
            {
                AdvancedFilterControl.Dispose();
                AdvancedFilterControl = null;
            }
            this.Loaded -= OnGridFilterControlLoaded;
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

    }

    public class SortButton : Button
    {
        #region Ctor

        public SortButton()
        {
            base.DefaultStyleKey = typeof(SortButton);
        }

        #endregion

        #region IsSortedProperty

        public bool IsSorted
        {
            get { return (bool)this.GetValue(SortButton.IsSortedProperty); }
            set { this.SetValue(SortButton.IsSortedProperty, value); }
        }

        public static readonly DependencyProperty IsSortedProperty = DependencyProperty.Register(
              "IsSorted",
              typeof(bool),
              typeof(SortButton),
              new PropertyMetadata(false, OnIsSortedChanged));

        private static void OnIsSortedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SortButton).ApplyState((bool)e.NewValue);
        }

        #endregion

        #region Icon

        public UIElement Icon
        {
            get { return (UIElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(UIElement), typeof(SortButton), new PropertyMetadata(null));

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyState(this.IsSorted);
        }

        #endregion

        #region Private Methods

        private void ApplyState(bool isSorted)
        {
            if (isSorted)
                VisualStateManager.GoToState(this, "Sorted", true);
            else
                VisualStateManager.GoToState(this, "UnSorted", true);
        }
        #endregion
    }
}
