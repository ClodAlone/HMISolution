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
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using System.Linq;
    using System.Collections.Generic;
    using Syncfusion.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using Syncfusion.Windows.Controls.Grid.Automation.Peers;
    using System.Windows.Automation.Peers;
    using System.Collections.Specialized;
    using System.Linq.Expressions;

    /// <summary>
    /// GridDataHeaderCellControl displays the header content for GridDataControl.
    /// </summary>
    /// <remarks>
    /// It has Dependency properties that is used in a customized ControlTemplate for
    /// showing the default values.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = GridDataHeaderCellControl.TemplatePopup, Type = typeof(GridDataHeaderCellControl))]
    [TemplatePart(Name = GridDataHeaderCellControl.TemplateCheckedListBox, Type = typeof(GridDataHeaderCellControl))]
    [TemplatePart(Name = GridDataHeaderCellControl.TemplateContentPresenter, Type = typeof(GridDataHeaderCellControl))]
    public class GridDataHeaderCellControl : Control, IDisposable
    {
        public const double MinimumWidth = 25.0d;
        public const double MaximumWidth = 25.0d;
        public const string TemplatePopupBorder = "PART_DropDownBorder";
        public const string TemplatePopup = "PART_Popup";
        public const string TemplateCheckedListBox = "PART_CheckedListBox";
        public const string TemplateSortBorder = "PART_SortBorder";
        public const string TemplateContentPresenter = "PART_ContentPresenter";

        #region DependencyProperties

        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return (string)this.GetValue(GridDataHeaderCellControl.TextProperty); }
            set { this.SetValue(GridDataHeaderCellControl.TextProperty, value); }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            
            GridDataHeaderCellControl headerCell = d as GridDataHeaderCellControl;
            if (!headerCell.IsInSuspend && headerCell.VisibleColumn != null)
            {
                headerCell.visibleColumn.IsInSuspend = true;
                headerCell.VisibleColumn.TableModel.TableProperties.SuspendEvents();
                headerCell.VisibleColumn.HeaderText = headerCell.Text;
                headerCell.VisibleColumn.TableModel.TableProperties.ResumeEvents();
                headerCell.visibleColumn.IsInSuspend = false;
                headerCell.VisibleColumn.TableModel.InvalidateCell(GridRangeInfo.Cell(headerCell.VisibleColumn.TableModel.CurrentCellState.RowIndex, headerCell.VisibleColumn.TableModel.CurrentCellState.ColumnIndex));
            }

            
        }
        #endregion
        #region DataTemplate
        public static readonly DependencyProperty ContentDataTemplateProperty = DependencyProperty.Register("ContentDataTemplate", typeof(DataTemplate), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Gets or sets the custom data template.
        /// </summary>
        /// <value>The custom data template.</value>
        public DataTemplate ContentDataTemplate
        {
            get { return (DataTemplate)this.GetValue(GridDataHeaderCellControl.ContentDataTemplateProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ContentDataTemplateProperty, value); }
        }
        #endregion

        #region
        public static readonly DependencyProperty ExcelLikeFilterAdvVisibilityProperty = DependencyProperty.Register("ExcelLikeFilterAdvVisibility", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(false));

        public bool ExcelLikeFilterAdvVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataHeaderCellControl.ExcelLikeFilterAdvVisibilityProperty);
            }
            set
            {
                this.SetValue(GridDataHeaderCellControl.ExcelLikeFilterAdvVisibilityProperty, value);
            }
        }

        #endregion

        #region

        public static readonly DependencyProperty ExcelLikeFilterPaneVisibilityProperty =
            DependencyProperty.Register("ExcelLikeFilterPaneVisibility", typeof (Visibility),
                                        typeof (GridDataHeaderCellControl), new FrameworkPropertyMetadata(Visibility.Collapsed));

        public Visibility ExcelLikeFilterPaneVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.ExcelLikeFilterPaneVisibilityProperty);
            }
            set
            {
                this.SetValue(GridDataHeaderCellControl.ExcelLikeFilterPaneVisibilityProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty IsAdvanceFilteringProperty = DependencyProperty.Register("IsAdvanceFiltering", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating if the Filter Popup has to
        /// be shown.
        /// </summary>
        /// <remarks>
        /// The Popup host has a GridDataCheckedListBoxControl associated with the Child.
        /// </remarks>
        /// <value>
        /// <b>True</b> if this instance ; otherwise, <b>false</b>.
        /// </value>
        public bool IsAdvanceFiltering
        {
            get { return (bool)this.GetValue(GridDataHeaderCellControl.IsAdvanceFilteringProperty); }
            set { this.SetValue(GridDataHeaderCellControl.IsAdvanceFilteringProperty, value); }
        }

        #region IsDropDownOpen

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged), new CoerceValueCallback(CoerceIsDropDownOpen)));

        /// <summary>
        /// Gets or sets a value indicating if the Filter Popup has to
        /// be shown.
        /// </summary>
        /// <remarks>
        /// The Popup host has a GridDataCheckedListBoxControl associated with the Child.
        /// </remarks>
        /// <value>
        /// <b>True</b> if this instance ; otherwise, <b>false</b>.
        /// </value>
        public bool IsDropDownOpen
        {
            get { return (bool)this.GetValue(GridDataHeaderCellControl.IsDropDownOpenProperty); }
            set { this.SetValue(GridDataHeaderCellControl.IsDropDownOpenProperty, value); }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl gdc = d as GridDataHeaderCellControl;
            gdc.RaiseOnIsDropDownChanged();
            bool newValue = (bool)args.NewValue;
            // bool oldValue = !newValue; Unused local variable
            if (!newValue)
            {
                if (gdc.HasCapture)
                {
                    Mouse.Capture(null);
                }
            }
            else
            {
                gdc.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (gdc.TableModel.Grid != null)
                            gdc.TableModel.Grid.MouseControllerDispatcher.CancelMode();
                    }), DispatcherPriority.ApplicationIdle);
            }

            gdc.CoerceValue(ToolTipService.IsEnabledProperty);
        }
        #endregion

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <b>True</b> if this instance ; otherwise, <b>false</b>.
        /// </value>
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(GridDataHeaderCellControl.IsReadOnlyProperty); }
            set { this.SetValue(GridDataHeaderCellControl.IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty StaysOpenOnEditProperty = DependencyProperty.Register("StaysOpenOnEdit", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the PopupHost stays open during edit.
        /// </summary>
        /// <value>
        /// <b>True</b> if ; otherwise, <b>false</b>.
        /// </value>
        public bool StaysOpenOnEdit
        {
            get { return (bool)this.GetValue(GridDataHeaderCellControl.StaysOpenOnEditProperty); }
            set { this.SetValue(GridDataHeaderCellControl.StaysOpenOnEditProperty, value); }
        }

        #region Sorting
        public static readonly DependencyProperty SortVisibilityProperty = DependencyProperty.Register("SortVisibility", typeof(Visibility), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Visibility.Hidden, OnSortVisibilityChanged));

        /// <summary>
        /// Gets or sets whether if the Sort Icon has to be shown.
        /// </summary>
        public Visibility SortVisibility
        {
            get { return (Visibility)this.GetValue(GridDataHeaderCellControl.SortVisibilityProperty); }
            set { this.SetValue(GridDataHeaderCellControl.SortVisibilityProperty, value); }
        }

        private static void OnSortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridDataHeaderCellControl;
            var value = (Visibility)args.NewValue;
            headerCellControl.SetSortStyle(value, headerCellControl.SortDirection);
        }

        /// <summary>
        /// Gets or sets the Sort Direction for this instance.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)this.GetValue(GridDataHeaderCellControl.SortDirectionProperty); }
            set { this.SetValue(GridDataHeaderCellControl.SortDirectionProperty, value); }
        }

        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(OnSortDirectionChanged));

        private static void OnSortDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridDataHeaderCellControl;
            var sortDirection = (ListSortDirection)args.NewValue;
            headerCellControl.SetSortStyle(headerCellControl.SortVisibility, sortDirection);
        }

        private void SetSortStyle(Visibility sortVisibility, ListSortDirection sortDirection)
        {
            switch (sortVisibility)
            {
                case Visibility.Visible:
                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        this.SortPath = GridDataResourceWrapper.BorderPathAsc;
                    }
                    else if (sortDirection == ListSortDirection.Descending)
                    {
                        this.SortPath = GridDataResourceWrapper.BorderPathDesc;
                    }
                    break;
                default:
                    this.SortPath = null;
                    break;
            }
        }

        public static readonly DependencyProperty SortBrushProperty = DependencyProperty.Register("SortBrush", typeof(Brush), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the brush value for the Sort Icon.
        /// </summary>
        public Brush SortBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.SortBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.SortBrushProperty, value); }
        }

        public static readonly DependencyProperty SortPathProperty = DependencyProperty.Register("SortPath", typeof(Geometry), typeof(GridDataHeaderCellControl));

        public Geometry SortPath
        {
            get { return (Geometry)this.GetValue(GridDataHeaderCellControl.SortPathProperty); }
            set { this.SetValue(GridDataHeaderCellControl.SortPathProperty, value); }
        }

        public static readonly DependencyProperty SortStringProperty = DependencyProperty.Register("SortString", typeof(string), typeof(GridDataHeaderCellControl), new PropertyMetadata(String.Empty));

        public string SortString
        {
            get { return (string)this.GetValue(GridDataHeaderCellControl.SortStringProperty); }
            internal set { this.SetValue(GridDataHeaderCellControl.SortStringProperty, value); }
        }
        #endregion

        #region ColumnOptionsStyles

        #region IsColumnOptionsDropDownOpen
        /// <summary>
        /// Dependency Property for IsColumnOptionsDropDownOpen.
        /// </summary>
        public static readonly DependencyProperty IsColumnOptionsDropDownOpenProperty = DependencyProperty.Register("IsColumnOptionsDropDownOpen", typeof(bool), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(OnIsColumnOptionsOpenChanged));

        /// <summary>
        /// Gets or sets a value indicating if the Column Options Popup has to be shown.
        /// </summary>
        /// <value><b>True</b> if this instance ; otherwise, <b>false</b>.</value>
        public bool IsColumnOptionsDropDownOpen
        {
            get
            {
                return (bool)this.GetValue(GridDataHeaderCellControl.IsColumnOptionsDropDownOpenProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.IsColumnOptionsDropDownOpenProperty, value);
            }
        }

        private static void OnIsColumnOptionsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl gdc = d as GridDataHeaderCellControl;
            gdc.RaiseOnIsColumnOptionsChanged();
            bool newValue = (bool)args.NewValue;
            // bool oldValue = !newValue; Unused local variable
            if (!newValue)
            {
                if (gdc.HasCapture)
                {
                    Mouse.Capture(null);
                }
            }
            gdc.CoerceValue(ToolTipService.IsEnabledProperty);
        }

        public event EventHandler IsColumnOptionsChanged;

        private void RaiseOnIsColumnOptionsChanged()
        {
            if (this.IsColumnOptionsChanged != null)
            {
                this.IsColumnOptionsChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        public static readonly DependencyProperty ColumnOptionsButtonVisibilityProperty = DependencyProperty.Register("ColumnOptionsButtonVisibility", typeof(Visibility), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Visibility.Hidden));

        /// <summary>
        /// Gets or sets a value indicating whether the Column Options Button has to be shown.
        /// </summary>
        /// <value><b>True</b> if this instance ; otherwise, <b>false</b>.</value>
        public Visibility ColumnOptionsButtonVisibility
        {
            get { return (Visibility)this.GetValue(GridDataHeaderCellControl.ColumnOptionsButtonVisibilityProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsBackgroundProperty = DependencyProperty.Register("ColumnOptionsBackground", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush ColumnOptionsBackground
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsBackgroundProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsForegroundProperty = DependencyProperty.Register("ColumnOptionsForeground", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush ColumnOptionsForeground
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsForegroundProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsForegroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsButtonBackgroundProperty = DependencyProperty.Register("ColumnOptionsButtonBackground", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush ColumnOptionsButtonBackground
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsButtonBackgroundProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsButtonBorderBrushProperty = DependencyProperty.Register("ColumnOptionsButtonBorderBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush ColumnOptionsButtonBorderBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsButtonBorderBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionPaneStyleProperty = DependencyProperty.Register("ColumnOptionPaneStyle", typeof(Style), typeof(GridDataHeaderCellControl), new PropertyMetadata(null));

        public Style ColumnOptionPaneStyle
        {
            get { return (Style)GetValue(ColumnOptionPaneStyleProperty); }
            set { SetValue(ColumnOptionPaneStyleProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsCloseButtonBrushProperty = DependencyProperty.Register("ColumnOptionsCloseButtonBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush ColumnOptionsCloseButtonBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsCloseButtonBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.ColumnOptionsCloseButtonBrushProperty, value); }
        }
        #endregion

        #region HeaderOptions
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush HoverBackground
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.HoverBackgroundProperty); }
            set { this.SetValue(GridDataHeaderCellControl.HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush HoverForeground
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.HoverForegroundProperty); }
            set { this.SetValue(GridDataHeaderCellControl.HoverForegroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderInnerBorderBrushProperty = DependencyProperty.Register("HeaderInnerBorderBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush HeaderInnerBorderBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.HeaderInnerBorderBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.HeaderInnerBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderInnerBorderThicknessProperty = DependencyProperty.Register("HeaderInnerBorderThickness", typeof(Thickness), typeof(GridDataHeaderCellControl));

        public Thickness HeaderInnerBorderThickness
        {
            get { return (Thickness)this.GetValue(GridDataHeaderCellControl.HeaderInnerBorderThicknessProperty); }
            set { this.SetValue(GridDataHeaderCellControl.HeaderInnerBorderThicknessProperty, value); }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get { return (Brush)GetValue(HeaderOptionsHoverBackgroundProperty); }
            set { SetValue(HeaderOptionsHoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsHoverBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsHoverBackground", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        public Brush HeaderOptionsCheckedBackground
        {
            get { return (Brush)GetValue(HeaderOptionsCheckedBackgroundProperty); }
            set { SetValue(HeaderOptionsCheckedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsCheckedBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsCheckedBackground", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        public Brush HeaderOptionsBorderBrush
        {
            get { return (Brush)GetValue(HeaderOptionsBorderBrushProperty); }
            set { SetValue(HeaderOptionsBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsBorderBrushProperty =
            DependencyProperty.Register("HeaderOptionsBorderBrush", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        #endregion

        #region Filtering

        public static readonly DependencyProperty FilterInnerBrushProperty = DependencyProperty.Register("FilterInnerBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush FilterInnerBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.FilterInnerBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterOuterBrushProperty = DependencyProperty.Register("FilterOuterBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush FilterOuterBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.FilterOuterBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterOuterBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterHoverInnerBrushProperty = DependencyProperty.Register("FilterHoverInnerBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush FilterHoverInnerBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.FilterHoverInnerBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterHoverInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterHoverOuterBrushProperty = DependencyProperty.Register("FilterHoverOuterBrush", typeof(Brush), typeof(GridDataHeaderCellControl));

        public Brush FilterHoverOuterBrush
        {
            get { return (Brush)this.GetValue(GridDataHeaderCellControl.FilterHoverOuterBrushProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterHoverOuterBrushProperty, value); }
        }

        internal GridStyleInfo CurrentStyle = null;

        /// <summary>
        /// DependencyProperty for FilterButtonVisibility.
        /// </summary>
        public static readonly DependencyProperty FilterButtonVisibilityProperty = DependencyProperty.Register("FilterButtonVisibility", typeof(Visibility), typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(Visibility.Hidden, OnFilterButtonVisibilityChanged));

        public Visibility FilterButtonVisibility
        {
            get { return (Visibility)this.GetValue(GridDataHeaderCellControl.FilterButtonVisibilityProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterButtonVisibilityProperty, value); }
        }

        private static void OnFilterButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl headerCell = d as GridDataHeaderCellControl;
            if (headerCell != null)
            {
                var style = GridControlBase.GetRenderStyleInfo(headerCell);
                headerCell.RefreshFilteringMode(false, style.ModelStyle);
            }
        }

        public static readonly DependencyProperty IsFilterAppliedProperty = DependencyProperty.Register("IsFilterApplied", typeof(bool), typeof(GridDataHeaderCellControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets / Sets the IsFilterApplied property.
        /// </summary>
        public bool IsFilterApplied
        {
            get { return (bool)GetValue(IsFilterAppliedProperty); }
            set { SetValue(IsFilterAppliedProperty, value); }
        }

        /// <summary>
        /// Gets / Sets the FilterAppliedInnerBrush.
        /// </summary>
        public Brush FilterAppliedInnerBrush
        {
            get { return (Brush)GetValue(FilterAppliedInnerBrushProperty); }
            set { SetValue(FilterAppliedInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterAppliedInnerBrushProperty = DependencyProperty.Register("FilterAppliedInnerBrush", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty FilterWrapperProperty = DependencyProperty.Register("FilterWrapper", typeof(GridDataFilterWrapper), typeof(GridDataHeaderCellControl));

        public GridDataFilterWrapper FilterWrapper
        {
            get { return (GridDataFilterWrapper)this.GetValue(GridDataHeaderCellControl.FilterWrapperProperty); }
            set { this.SetValue(GridDataHeaderCellControl.FilterWrapperProperty, value); }
        }


        /// <summary>
        /// Dependency property Registration for ItemsSource
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
          "ItemsSource",
          typeof(object),
          typeof(GridDataHeaderCellControl),
          new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public Object ItemsSource
        {
            get
            {
                return (object)this.GetValue(GridDataHeaderCellControl.ItemsSourceProperty);
            }
            set
            {
                this.SetValue(GridDataHeaderCellControl.ItemsSourceProperty, value);
            }
        }


        #endregion

        #endregion

        #region Constructor
        public GridDataHeaderCellControl()
        {
            this.IsKeyFocusedForAdvancedFilterTextBox = false;
            IsTabStop = false;
            KeyboardNavigation.SetIsTabStop(this, false);
            this.Loaded += new RoutedEventHandler(GridDataHeaderCellControl_Loaded);
        }

        static GridDataHeaderCellControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(typeof(GridDataHeaderCellControl)));
            // Disable tooltips on popup when it is open
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));
            BackgroundProperty.OverrideMetadata(typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(OnBackgroundChanged));
            ForegroundProperty.OverrideMetadata(typeof(GridDataHeaderCellControl), new FrameworkPropertyMetadata(OnForegroundChanged));
        }
        #endregion

        #region private Variables
        
        //This falg used to find out propertychanged calls from SelectAll Checkbox
        bool propertyChangedFromSelectAll = false;   

        #endregion

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCell = d as GridDataHeaderCellControl;
            if (headerCell.AdvancedFilteringPane != null)
            {
                var gridDataFilteringPane = headerCell.AdvancedFilteringPane as GridDataFilteringPane;
                var wrapperInstance = gridDataFilteringPane.GetFilterWrapper();

                if (Object.Equals(wrapperInstance.Background, args.NewValue))
                {
                    wrapperInstance.Background = (Brush)args.NewValue;
                }
            }
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCell = d as GridDataHeaderCellControl;
            if (headerCell.AdvancedFilteringPane != null)
            {
                var gridDataFilteringPane = headerCell.AdvancedFilteringPane as GridDataFilteringPane;
                var wrapperInstance = gridDataFilteringPane.GetFilterWrapper();
                wrapperInstance.Foreground = (Brush)args.NewValue;
            }
        }

        private bool HasCapture
        {
            get { return Mouse.Captured == this; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over inner text box.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is mouse over inner text box; otherwise, <c>false</c>.
        /// </value>
        protected bool IsMouseOverInnerTextBlock
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over popup host.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is mouse over popup host; otherwise, <c>false</c>.
        /// </value>
        protected bool IsMouseOverPopupHost
        {
            get;
            private set;
        }

        protected bool IsMouseOverColumnOptionsPopup
        {
            get;
            private set;
        }

        protected bool IsKeyFocusedForAdvancedFilterTextBox
        {
            get;
            private set;
        }

        private GridDataVisibleColumn visibleColumn;
        /// <summary>
        /// Gets or sets the visible column to which this instance belongs.
        /// </summary>
        public GridDataVisibleColumn VisibleColumn
        {
            get
            {
                return this.visibleColumn;
            }
            private set
            {
                if (this.visibleColumn != value)
                {
                    if (this.visibleColumn != null)
                        this.visibleColumn.Filters.CollectionChanged -= OnFiltersCollectionChanged;
                    this.visibleColumn = value;
                    if (this.visibleColumn != null)
                    {
                        this.IsFilterApplied = this.VisibleColumn.Filters.Count > 0;
                        this.visibleColumn.Filters.CollectionChanged += OnFiltersCollectionChanged;
                    }
                }
            }
        }

        internal void SetVisibleColumn(GridDataVisibleColumn column)
        {
            if (column != null)
            {
                this.VisibleColumn = column;
                //if (this.VisibleColumn.TableModel is GridDataChildTableModel)
                //{
                //    this.Unloaded += new RoutedEventHandler(GridDataHeaderCellControl_Unloaded);
                //}
            }
        }

        void GridDataHeaderCellControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded += new RoutedEventHandler(GridDataHeaderCellControl_Unloaded);

            //------

            this.PART_ContentPresenter = this.GetTemplateChild(GridDataHeaderCellControl.TemplateContentPresenter) as ContentPresenter;
            if (this.PART_ContentPresenter != null)
            {
                this.PART_ContentPresenter.LostFocus += new RoutedEventHandler(this.InnerTextBlock_LostFocus);
                // WPF-8806 column style differs when columns enters to viewport by scrolling.
                // this.PART_ContentPresenter.SetValue(TextBlock.ForegroundProperty, this.Foreground); 
                if (this.HasVisibleColumn && this.TableModel != null)
                {
                    //var rowIndex = this.TableModel.TableProperties.StackedHeaderRows.Count;
                    var rowIndex = this.TableModel.TableProperties.HeaderRows - 1;
                    var colIndex = this.TableModel.TableProperties.VisibleColumns.IndexOf(this.VisibleColumn);
                    colIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(colIndex);
                    var headerStyle = this.VisibleColumn.HeaderStyle;
                    var style = this.TableModel[rowIndex, colIndex];
                    if (style.HorizontalAlignment != System.Windows.HorizontalAlignment.Center)
                    {
                    }
                    if (headerStyle != null)
                    {
                        style.BeginInit();
                        style.ModifyStyle(headerStyle, Syncfusion.Windows.Styles.StyleModifyType.ApplyNew);
                        var baseStyle = this.TableModel.HeaderStyle;
                        style.ModifyStyle(baseStyle, Syncfusion.Windows.Styles.StyleModifyType.ApplyNew);
                        style.EndInit();
                    }
                    ApplyStyle(style);
                }
                else if (RenderStyle != null && RenderStyle.CellType == "SortableHeaderCell" && !LegacyEnabled)
                {
                    ApplyStyle(RenderStyle);
                }
                if (this.HasVisibleColumn && this.TableModel == null && this.VisibleColumn.HeaderStyle != null)
                {
                    var style = this.VisibleColumn.HeaderStyle;
                    ApplyStyle(style);
                }
            }

            //-------

            if (this.PART_FilterPopupHost == null)
                this.PART_FilterPopupHost = this.GetTemplateChild("PART_FilterDropDown") as GridDataExcelLikeFilterPane;
            if (this.VisibleColumn != null)
            {
                this.visibleColumn.ExcelLikeFilterPane = this.PART_FilterPopupHost;
                this.visibleColumn.Filters.CollectionChanged -= OnFiltersCollectionChanged;
                this.visibleColumn.Filters.CollectionChanged += OnFiltersCollectionChanged;
            }
            WireExcelLikeFilteringEvents();
        }

        private void GridDataHeaderCellControl_Unloaded(object sender, RoutedEventArgs e)
        {
           
            //this.Loaded -= new RoutedEventHandler(GridDataHeaderCellControl_Loaded);
            this.Unloaded -= new RoutedEventHandler(GridDataHeaderCellControl_Unloaded);
            if (this.VisibleColumn != null)
            {
                if (this.visibleColumn.ExcelLikeFilterPane != null)
                {
                    this.visibleColumn.ExcelLikeFilterPane.Dispose();
                    this.visibleColumn.ExcelLikeFilterPane = null;
                }
                this.visibleColumn.Filters.CollectionChanged -= OnFiltersCollectionChanged;
            }

            UnWireExcelLikeFilteringEvents();
            this.RenderStyle = null;
        }       

        private void OnFiltersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var filters = (ObservableCollection<FilterPredicate>)sender;
            this.IsFilterApplied = filters.Count > 0;
            if (this.PART_FilterPopupHost != null)
            {
                this.PART_FilterPopupHost.ClearFilterEnable = filters.Count > 0;
            }
            
        }

        public GridRenderStyleInfo RenderStyle
        {
            get;
            internal set;
        }
       

        public IGridDataFilterAction AdvancedFilteringPane
        {
            get;
            private set;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void CloseFilterDropDown()
        {
            if (this.IsDropDownOpen)
            {
                this.ClearValue(GridDataHeaderCellControl.IsDropDownOpenProperty);
                if (this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = false;
                    this.ExcelLikeFilterPaneVisibility = Visibility.Collapsed;
                }
            }
        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                GridDataHeaderCellControl gcd = (GridDataHeaderCellControl)d;
                if (!gcd.IsLoaded)
                {
                    gcd.ExcelLikeFilterPaneVisibility = Visibility.Visible;
                    gcd.RegisterToOpenOnLoad();
                    return false;
                }
            }

            return value;
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object value)
        {
            GridDataHeaderCellControl gcd = (GridDataHeaderCellControl)d;
            return gcd.IsDropDownOpen ? false : value;
        }

        private void InnerTextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsDropDownOpen && !this.IsMouseOverPopupHost)
            {
                this.CloseFilterDropDown();
            }
        }

        protected virtual void NavigateNextLine()
        {
        }

        protected virtual void NavigatePreviousLine()
        {
        }

        public bool LegacyEnabled { get; set; }

        #region Template PARTs
        public ContentPresenter PART_ContentPresenter { get; private set; }
        //ExcelLike filter pop up
        public GridDataExcelLikeFilterPane PART_FilterPopupHost { get; private set; }
        public GridDataFilterToggleButton ToggleButton { get; private set; }

        public Popup PART_PopupHost { get; private set; }  
        public Popup PART_ColumnOptionsPopupHost { get; private set; }
        public Border PART_DropDownBorder { get; private set; }      
      
        #endregion

        
        private bool isAdvancedFilteringModeLoaded = false;
        

        public override void OnApplyTemplate()
        {
            if (this.VisibleColumn==null )
            {
                return;
            }
            ///Blend Designing Simple Styles
            if (GridDataTableModelHelper.IsInDesignMode)
            {
                this.Background = Brushes.Silver;
                this.SortVisibility = System.Windows.Visibility.Visible;
                this.FilterButtonVisibility = System.Windows.Visibility.Visible;
                this.SortPath = GridDataResourceWrapper.BorderPathAsc;
                this.ColumnOptionsButtonVisibility = System.Windows.Visibility.Visible;
                this.ColumnOptionsForeground = Brushes.Gray;
                this.ColumnOptionsBackground = Brushes.Green;
                this.ColumnOptionsButtonBackground = Brushes.Green;
                this.FilterOuterBrush = Brushes.Green;
                this.FilterInnerBrush = Brushes.YellowGreen;
                this.FilterHoverInnerBrush = Brushes.Orange;
                this.FilterHoverOuterBrush = Brushes.OrangeRed;
            }
            if (this.VisibleColumn.ColumnHeaderStyle != null)
            {
                this.Style = this.VisibleColumn.ColumnHeaderStyle;
            }           
            if(this.VisibleColumn.TableModel.TableProperties.IsLegacyStyleEnabled)
                this.ExcelLikeFilterPaneVisibility = System.Windows.Visibility.Visible;
            //Following Code for ExcelLikeFiltering
            var gridControl = this.FindParentElementOfType<GridDataControl>();
            if (gridControl == null)
                return;

            this.ToggleButton = this.GetTemplateChild("toggleButton") as GridDataFilterToggleButton;           
            this.PART_FilterPopupHost = this.GetTemplateChild("PART_FilterDropDown") as GridDataExcelLikeFilterPane;
            this.VisibleColumn.ExcelLikeFilterPane = this.PART_FilterPopupHost;
          
            if (this.PART_FilterPopupHost != null)
            {
                if (this.visibleColumn.AllowFilter && (!gridControl.EnableLegacyFiltering || this.TableModel.TableProperties.VisualStyle==VisualStyle.Blend))
                {
                    this.PART_FilterPopupHost.StaysOpen = false;
                    this.PART_FilterPopupHost.ColumnName = this.VisibleColumn.HeaderText;
                    this.LoadExcelLikeAdvanceFiltering();
                    if (this.VisibleColumn.ExcelLikeFilterPaneStyle != null)
                    {
                        //Apply style if style applied in sample.
                        this.PART_FilterPopupHost.Style = this.VisibleColumn.ExcelLikeFilterPaneStyle;
                    }
                    else
                    {
                        //ViualStyle Style Applied here    
                        string visualStyle = this.VisibleColumn.TableModel != null
                                                 ? this.VisibleColumn.TableModel.TableProperties.VisualStyle
                                                       .ToString()
                                                 : "Default";
                        SetVisalStyleForFilterPopUp(visualStyle, false);
                        this.PART_FilterPopupHost.VisualStyle = visualStyle;
                    }
                }
               
                this.PART_FilterPopupHost.ExcelLikeFilterAdvVisibility = this.VisibleColumn.TableModel != null ? !this.VisibleColumn.TableModel.TableProperties.EnableLegacyFiltering:true;
                this.ExcelLikeFilterAdvVisibility = this.VisibleColumn.TableModel != null ? !this.VisibleColumn.TableModel.TableProperties.EnableLegacyFiltering : true ;

                if (this.SortVisibility == System.Windows.Visibility.Visible)
                    this.PART_FilterPopupHost.SortOrder = this.SortDirection.ToString();

                if (!this.ExcelLikeFilterAdvVisibility)
                {
                    //if EnableAdvanceExcelLikeFiltering=false then following options should not in visible
                    this.PART_FilterPopupHost.ClearFilterVisibility = false;
                    this.PART_FilterPopupHost.SortOptionVisibility = false;
                    this.PART_FilterPopupHost.SearchOptionVisibility = false;
                    this.PART_FilterPopupHost.OkCancelButtonVisibility = false;
                    this.PART_FilterPopupHost.ResizingOptionVisibility = false;
                    this.PART_FilterPopupHost.AdvanceFilteringOptionVisibility = false;
                }
                else
                {
                    this.PART_FilterPopupHost.SortOptionVisibility = this.VisibleColumn.ShowSortOptioninExcelLikeFiltering; 
                    this.PART_FilterPopupHost.SearchOptionVisibility = this.VisibleColumn.ShowSearchOptioninExcelLikeFiltering; 
                    this.PART_FilterPopupHost.AdvanceFilteringOptionVisibility = this.VisibleColumn.ShowAdvanceFilteringOptioninExcelLikeFiltering;
                }            
            }
            
            this.PART_DropDownBorder = this.GetTemplateChild(TemplatePopupBorder) as Border;


            if (this.PART_ContentPresenter != null)
            {
                this.PART_ContentPresenter.LostFocus -= new RoutedEventHandler(this.InnerTextBlock_LostFocus);
                this.PART_ContentPresenter = null;
            }

            if (this.AdvancedFilteringPane != null)
            {
                this.IsAdvanceFiltering = false;
                this.isAdvancedFilteringModeLoaded = false;
                this.AdvancedFilteringPane = null;
            }

            if (this.PART_PopupHost != null)
            {
                this.PART_PopupHost.Opened -= new EventHandler(PopupHost_Opened);
                this.PART_PopupHost = null;
            }           
            this.UnloadColumnOptions();

            base.OnApplyTemplate();

            if (gridControl != null)
            {
                var visualStyle = SkinStorage.GetVisualStyle(gridControl);
                if (visualStyle != string.Empty)
                {
                    SkinStorage.SetVisualStyle(this, visualStyle);
                }
            }
            
            this.PART_PopupHost = this.GetTemplateChild(GridDataHeaderCellControl.TemplatePopup) as Popup;
             
         
           
            if (this.PART_PopupHost != null)
            {
                SetFilterPanePlacement();
                this.PART_PopupHost.Opened += new EventHandler(PopupHost_Opened);
            }
           

            #region ColumnOptions
            if (this.HasVisibleColumn)
            {
                if (this.VisibleColumn.ShowColumnOptions)
                {
                    var columnOptionsPopupHost = this.GetTemplateChild("PART_ColumnOptionsPopup") as Popup;
                    if (columnOptionsPopupHost != null)
                    {
                        columnOptionsPopupHost.LostFocus += new RoutedEventHandler(this.ColumnOptionsPopupHost_LostFocus);
                        this.LoadColumnOptions(columnOptionsPopupHost);
                        this.PART_ColumnOptionsPopupHost = columnOptionsPopupHost;
                    }
                }
            }
            #endregion         

            if (this.HasVisibleColumn && this.VisibleColumn.AllowFilter && this.TableModel != null && this.TableModel.TableProperties != null && !this.TableModel.TableProperties.ShowFilterBar)//&& !this.ExcelLikeFilterAdvVisibility)// && this.VisibleColumn.TableModel != null)// && !this.VisibleColumn.TableModel.IsLegacyDataTable)            
            {   
                if (this.VisibleColumn.IsAdvancedFilteringMode)
                {
                    this.LoadAdvancedFilteringMode(this.PART_DropDownBorder);
                }               
            }
        }

        #region ExcelLikeFiltering

        private void SetVisalStyleForFilterPopUp(string visualStyle, bool isAdvanceFilterPopup)
        {
            if (!isAdvanceFilterPopup && this.PART_FilterPopupHost.Style != null)
            {
                return;
            }

            string sourcePath = "";
            if (visualStyle != null && visualStyle != "Default" && visualStyle != "Office2003" &&
                visualStyle != "Custom" && visualStyle != "DefaultOffice2007Blue" &&
                visualStyle != "DefaultOffice2007Black" && visualStyle != "DefaultOffice2007Silver")
            {
                sourcePath = @"/Syncfusion.Grid.Wpf;component/GridDataControl/ExcelLikeFiltering/Themes/" +
                             visualStyle.ToString() + ".xaml";
            }
            else
            {
                sourcePath = @"/Syncfusion.Grid.Wpf;component/GridDataControl/ExcelLikeFiltering/Themes/Generic.xaml";
            }

            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
            var gridControl = this.FindParentElementOfType<GridDataControl>();

            if (visualStyle == "Metro" && GridDataControl.GetOverrideVisualStyle(gridControl))
                MergeMetroBrush(rd, gridControl);

            if (!isAdvanceFilterPopup)
                this.PART_FilterPopupHost.Resources.MergedDictionaries.Add(rd);

            else
            {
                var filterPane = this.AdvancedFilteringPane as GridDataFilteringPane;
                if (filterPane.Style != null)
                    return;
                filterPane.Resources.MergedDictionaries.Add(rd);
            }
        }

        
        public static void MergeMetroBrush(ResourceDictionary metroskindictionary, DependencyObject obj)
        {
            ResourceDictionary dictionary = metroskindictionary;
            try
            {
                if (SkinStorage.GetMetroBrush(obj) != null)
                    dictionary["MetroBrush"] = SkinStorage.GetMetroBrush(obj);
                if (SkinStorage.GetMetroHoverBrush(obj) != null)
                    dictionary["MetroHoverBrush"] = SkinStorage.GetMetroHoverBrush(obj);
                //if (SkinStorage.GetMetroForegroundBrush(obj) != null)
                //    dictionary["MetroForegroundBrush"] = SkinStorage.GetMetroForegroundBrush(obj);
                if (SkinStorage.GetMetroFontFamily(obj) != null)
                    dictionary["MetroFontFamily"] = SkinStorage.GetMetroFontFamily(obj);
                if (SkinStorage.GetMetroBorderBrush(obj) != null)
                    dictionary["MetroBorderBrush"] = SkinStorage.GetMetroBorderBrush(obj);
                if (SkinStorage.GetMetroFocusedBorderBrush(obj) != null)
                    dictionary["MetroFocusedBorderBrush"] = SkinStorage.GetMetroFocusedBorderBrush(obj);
                //if (SkinStorage.GetMetroBackgroundBrush(obj) != null)
                //    dictionary["MetroBackgroundBrush"] = SkinStorage.GetMetroBackgroundBrush(obj);
                //if (SkinStorage.GetMetroPanelBackgroundBrush(obj) != null)
                //    dictionary["MetroPanelBackgroundBrush"] = SkinStorage.GetMetroPanelBackgroundBrush(obj);
                if (SkinStorage.GetMetroHighlightedForegroundBrush(obj) != null)
                    dictionary["MetroHighlightedForegroundBrush"] = SkinStorage.GetMetroHighlightedForegroundBrush(obj);

            }
            catch { }


        }

        private bool isWired = false;
        /// <summary>
        /// Wires the excel like filtering events.
        /// </summary>
        internal void WireExcelLikeFilteringEvents()
        {
            if (this.VisibleColumn != null && !isWired)
            {
                if (this.PART_FilterPopupHost == null)
                    this.PART_FilterPopupHost = this.GetTemplateChild("PART_FilterDropDown") as GridDataExcelLikeFilterPane;
                if (this.PART_FilterPopupHost != null)
                {
                    this.PART_FilterPopupHost.OkButtonClick += new OkButtonClickEventHandler(OnFilterPopupHostOkButtonClick);
                    this.PART_FilterPopupHost.ClearMenuItemClick += new ClearMenuItemClickEventHandler(OnFilterPopupHostClearMenuItemClick);
                    this.PART_FilterPopupHost.PopupOpened += new PopupOpenedEventHandler(OnFilterPopupHostPopupOpened);
                    this.PART_FilterPopupHost.SortMenuItemClick += new SortMenuItemClickEventHandler(OnFilterPopupHostSortMenuItemClick);
                    this.PART_FilterPopupHost.AdvanceFilteringOkButtonClick += new AdvanceFilteringOkButtonClickEventHandler(OnFilterPopupHostAdvanceFilteringOkButtonClick);
                    this.PART_FilterPopupHost.OnFilterElementChanged += new OnFilterElementPropertyChangedEventHandler(FilterPopupHostFilterElementChanged);
                    this.PART_FilterPopupHost.SelectAllUnCheckBoxChecked += new SelectAllCheckBoxUnCheckedEventHandler(PART_FilterPopupHost_SelectAllUnCheckBoxChecked);
                    this.PART_FilterPopupHost.SelectAllCheckBoxChecked += new SelectAllCheckBoxCheckedEventHandler(PART_FilterPopupHost_SelectAllCheckBoxChecked);
                    isWired = true;
                }
            }
        }
            

        

        /// <summary>
        /// UnWire the excel like filtering events.
        /// </summary>
        void UnWireExcelLikeFilteringEvents()
        {
            if (this.PART_FilterPopupHost != null && isWired == true)
            {
                this.PART_FilterPopupHost.OkButtonClick -= new OkButtonClickEventHandler(OnFilterPopupHostOkButtonClick);
                this.PART_FilterPopupHost.ClearMenuItemClick -= new ClearMenuItemClickEventHandler(OnFilterPopupHostClearMenuItemClick);
                this.PART_FilterPopupHost.PopupOpened -= new PopupOpenedEventHandler(OnFilterPopupHostPopupOpened);
                this.PART_FilterPopupHost.SortMenuItemClick -= new SortMenuItemClickEventHandler(OnFilterPopupHostSortMenuItemClick);
                this.PART_FilterPopupHost.AdvanceFilteringOkButtonClick -= new AdvanceFilteringOkButtonClickEventHandler(OnFilterPopupHostAdvanceFilteringOkButtonClick);
                this.PART_FilterPopupHost.OnFilterElementChanged -= new OnFilterElementPropertyChangedEventHandler(FilterPopupHostFilterElementChanged);
                this.PART_FilterPopupHost.SelectAllUnCheckBoxChecked -= new SelectAllCheckBoxUnCheckedEventHandler(PART_FilterPopupHost_SelectAllUnCheckBoxChecked);
                this.PART_FilterPopupHost.SelectAllCheckBoxChecked -= new SelectAllCheckBoxCheckedEventHandler(PART_FilterPopupHost_SelectAllCheckBoxChecked);
                isWired = false;
            }
        }


        #region ExcelLikeFiltering Events       
        
        /// <summary>
        /// Handles the SelectAllCheckBoxChecked event of the PART_FilterPopupHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxCheckedEventArgs"/> instance containing the event data.</param>
        void PART_FilterPopupHost_SelectAllCheckBoxChecked(object sender, SelectAllCheckBoxCheckedEventArgs args)
        {
            //While Checking SelectAllCheckBox We have clear the FilteredRecords, since all item in checked state.           
          
            TableModel.IsInFilter = true;
            this.visibleColumn.Filters.Clear();
            UpdateExcelFilterDetails(null, true);
            this.TableModel.BeginInit();
            var items = args.FilterElements;
            items.Where(c => c.IsSelected == false).ForEach<FilterElement>(c =>
            {
                this.ShowOrHideRow(c.ActualValue, true, false);
            });

            this.ShowOrHideRow(null, true, true);
            this.TableModel.ForceFilterRefresh();
            this.TableModel.EndInit();
            this.TableModel.IsInFilter = false;
        }


        /// <summary>
        /// Handles the SelectAllUnCheckBoxChecked event of the PART_FilterPopupHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxUnCheckedEventArgs"/> instance containing the event data.</param>
        void PART_FilterPopupHost_SelectAllUnCheckBoxChecked(object sender, SelectAllCheckBoxUnCheckedEventArgs args)
        {
            //UpdateExcelFilterDetails(null, false);
            //this.TableModel.IsInFilter = true;
            //this.TableModel.BeginInit();
            //var items = args.FilterElements.ToList<FilterElement>();
            //items.Where(c => c.IsSelected == true).ForEach<FilterElement>(c =>
            //{

            //    this.ShowOrHideRow(c.ActualValue, false, false);
            //});

            //this.ShowOrHideRow(null, false, true);
            //this.TableModel.ForceFilterRefresh();
            //this.TableModel.EndInit();
            //this.TableModel.IsInFilter = false;
        }

        /// <summary>
        /// Filters the popup host filter element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.OnFilterElementPropertyChangedEventArgs"/> instance containing the event data.</param>
        void FilterPopupHostFilterElementChanged(object sender, OnFilterElementPropertyChangedEventArgs args)
        {
            if (!propertyChangedFromSelectAll)//If Property Changed from Check/UnCheck select all CheckBox,then we dont do any process here
            {
                var filterElement = args.FilterElement;
                UpdateExcelFilterDetails(filterElement, args.SelectAllChecked);
                this.ApplyFilters();
            }
        }

        /// <summary>
        /// Called when [filter popup host advance filtering ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.AdvanceFilteringOkButtonClickEventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostAdvanceFilteringOkButtonClick(object sender, AdvanceFilteringOkButtonClickEventArgs args)
        {
            //For AboveAverage && BelowAverage we have calculate below
            if (args.PredicateType.ToString() != GridDataResourceWrapper.AboveAverage && args.PredicateType.ToString() != GridDataResourceWrapper.BelowAverage)
            {                
                var FilterPopUp = sender as GridDataExcelLikeFilterPane;
                if (this.visibleColumn.TableModel == null)
                    return;
                this.VisibleColumn.TableModel.IsInFilter = true;
                List<FilterPredicate> predicate = new List<FilterPredicate>();
                //For TextFilters
                if (FilterPopUp.ColumnType == GridDataResourceWrapper.TextFilters)
                {
                    if (args.FilterValue1 != null && args.FilterType1 != null)
                    {
                        if (args.FilterValue1 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue1 = null;
                        predicate.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), IsCaseSensitive = false, FilterBehavior = FilterBehavior.StringTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }

                    if (args.FilterValue2 != null && args.FilterType2 != null)
                    {
                        if (args.FilterValue2 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue2 = null;
                        predicate.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), IsCaseSensitive = false, FilterBehavior = FilterBehavior.StringTyped, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                    }

                    if (predicate.Count > 0)
                        this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, predicate, true);
                    this.VisibleColumn.TableModel.IsInFilter = false;
                }
                else if (FilterPopUp.ColumnType == GridDataResourceWrapper.NumberFilters)//For Number Filters
                {

                    List<FilterPredicate> list = new List<FilterPredicate>();
                    if (args.FilterValue1 != null)
                    {
                        double doubleValue1 = 0.0;
                        decimal decimalValue1 = 0.0m;
                        int intValue1 = 0;
                        bool isIntType = false;
                        bool isDoubleType = false;
                        bool bool1 = false;

                        if (args.FilterValue1 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue1 = null;
                        if (args.FilterValue1 == null)
                            list.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                        else
                        {
                            if (this.VisibleColumn.ColumnType == typeof(int))
                            {
                                bool1 = Int32.TryParse(args.FilterValue1.ToString(), out intValue1);
                                isIntType = true;
                            }
                            else if (this.VisibleColumn.ColumnType == typeof(double))
                            {
                                bool1 = double.TryParse(args.FilterValue1.ToString(), out doubleValue1);
                                isDoubleType = true;
                            }
                            else
                            {
                                bool1 = decimal.TryParse(args.FilterValue1.ToString(), out decimalValue1);
                            }
                            if (bool1)
                            {
                                object filtervalue = decimalValue1;
                                if (isIntType)
                                    filtervalue = intValue1;
                                else if (isDoubleType)
                                    filtervalue = doubleValue1;

                                list.Add(new FilterPredicate() { FilterValue = filtervalue, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                            }
                        }
                    }
                    if (args.FilterValue2 != null && args.FilterType2 != null)
                    {
                        double doubleValue2 = 0.0;
                        decimal decimalValue2 = 0.0m;
                        int intValue2 = 0;
                        bool isIntType = false;
                        bool isDoubleType = false;
                        bool bool2 = false;

                        if (args.FilterValue2 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue2 = null;
                        if (args.FilterValue2 == null)
                            list.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                        else
                        {
                            if (this.VisibleColumn.ColumnType == typeof(int))
                            {
                                bool2 = Int32.TryParse(args.FilterValue2.ToString(), out intValue2);
                                isIntType = true;
                            }
                            else if (this.VisibleColumn.ColumnType == typeof(double))
                            {
                                bool2 = Double.TryParse(args.FilterValue2.ToString(), out doubleValue2);
                                isDoubleType = true;
                            }
                            else
                            {
                                bool2 = Decimal.TryParse(args.FilterValue2.ToString(), out decimalValue2);
                            }
                            if (bool2)
                            {
                                object filtervalue = decimalValue2;
                                if (isIntType)
                                    filtervalue = intValue2;
                                else if (isDoubleType)
                                    filtervalue = doubleValue2;

                            list.Add(new FilterPredicate() { FilterValue = filtervalue, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), FilterBehavior = Linq.FilterBehavior.StronglyTyped, IsCaseSensitive = false, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                            }
                        }
                    }
                   
                     if(list.Count > 0)
                            this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, list, true);
                }
                    //For DateTimeFilters
                else if (FilterPopUp.ColumnType == GridDataResourceWrapper.DateFilters)
                {
                    List<FilterPredicate> list = new List<FilterPredicate>();
                    if (args.FilterValue1 != null)
                    {
                        DateTime value1;
                          if (args.FilterValue1 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue1 = null;
                          if (args.FilterValue1 == null)
                              list.Add(new FilterPredicate() { FilterValue = args.FilterValue1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                          else
                          {
                              var bool1 = DateTime.TryParse(args.FilterValue1.ToString(), out value1);
                              if (bool1)
                              {
                                  list.Add(new FilterPredicate() { FilterValue = value1, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                              }
                          }
                    }
                    if (args.FilterType2 != null && args.FilterValue2 != null)
                    {
                        DateTime value2;
                        if (args.FilterValue2 == GridDataResourceWrapper.BlankFilterText)
                            args.FilterValue2 = null;
                        if (args.FilterValue2 == null)
                            list.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType1.ToString()), PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()), IsCaseSensitive = false, FilterBehavior = Linq.FilterBehavior.StronglyTyped });
                        else
                        {
                            var bool2 = DateTime.TryParse(args.FilterValue2.ToString(), out value2);
                            if (bool2)
                            {
                                list.Add(new FilterPredicate() { FilterValue = args.FilterValue2, FilterType = FilterHelpers.GetFilterType(args.FilterType2.ToString()), FilterBehavior = Linq.FilterBehavior.StronglyTyped, IsCaseSensitive = false, PredicateType = FilterHelpers.GetPredicateType(args.PredicateType.ToString()) });
                            }
                        }
                    }
                   
                    if (list.Count > 0)
                            this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, list, true);
                }
            }
            else
            {
                //For BelowAverage and AboveAverage
                double value = 0;
                int count = 0;
                decimal value1=0;
                bool isDecimal = false;
                foreach (var it in this.VisibleColumn.TableModel.SourceList)
                {
                    object val = this.VisibleColumn.TableModel.Table.GetValue(it, this.VisibleColumn.MappingName);
                    if (val != null)
                    {
                        if (this.VisibleColumn.ColumnType == typeof(int))
                        {
                            value = value + Int32.Parse(val.ToString());
                        }
                        else if (this.VisibleColumn.ColumnType == typeof(double))
                        {
                            value = value + double.Parse(val.ToString());
                        }
                        else if (this.VisibleColumn.ColumnType == typeof(decimal))
                        {
                            value1 = value1 + decimal.Parse(val.ToString());
                            isDecimal = true;
                        }
                    }
                    
                    count = count + 1;
                }
                this.VisibleColumn.TableModel.IsInFilter = true;

                if (isDecimal)
                {
                    decimal average1 = value1/count;
                    if (args.PredicateType.ToString() == GridDataResourceWrapper.AboveAverage)
                    {
                        this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, average1, FilterType.GreaterThan,
                                                                   PredicateType.Or, false, true);
                    }
                    else
                    {
                        this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, average1, FilterType.LessThan,
                                                                   PredicateType.Or, false, true);
                    }
                }
                else
                {
                    var average = value/count;
                    if ((string) args.PredicateType == GridDataResourceWrapper.AboveAverage)
                    {
                        this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, average, FilterType.GreaterThan,
                                                                   PredicateType.Or, false, true);
                    }
                    else
                    {
                        this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, average, FilterType.LessThan,
                                                                   PredicateType.Or, false, true);
                    }
                }

                this.VisibleColumn.TableModel.IsInFilter = false;

            }

            //After filtering the record using Advance Filtering we need to Synchronize the data with ExcelLike Filtering.
            //Below code used to Synchronize the AdvanceFiltering with ExcelLikeFiltering
            var unFilteredList = this.VisibleColumn.TableModel.View.Records.Select(x => this.VisibleColumn.TableModel.Table.GetValue(x.Data, this.VisibleColumn.MappingName)).Distinct();
            // var excelFilter = (this.TableModel.View as IExcelLikeFilterExt); Unused local variable
            foreach (var item in this.PART_FilterPopupHost.ItemsSource as List<FilterElement>)
            { 
                var filteredElement = unFilteredList.FirstOrDefault(x => x == (item.ActualValue));
                if (filteredElement == null)
                {
                    var filteritem = new FilterElement() { ActualValue = item.ActualValue, IsSelected = false, Name = item.Name };
                    filteritem.PropertyChanged += new PropertyChangedEventHandler(this.PART_FilterPopupHost.OnFilterElementPropertyChanged);
                }
            }

        }

        /// <summary>
        /// Called when [filter popup host sort menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.SortMenuItemClickEventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostSortMenuItemClick(object sender, SortMenuItemClickEventArgs args)
        {
            if (this.visibleColumn.TableModel == null)
                return;
            if (this.VisibleColumn.TableModel.TableProperties.AllowSort && this.VisibleColumn.AllowSort)
            {
                //this.VisibleColumn.TableModel.SortColumn(this.VisibleColumn);
                LineSizeCollection rowHeights = (LineSizeCollection)this.VisibleColumn.TableModel.RowHeights;
                rowHeights.SuspendUpdates();

                this.VisibleColumn.TableModel.IsInSort = true;
                this.VisibleColumn.TableModel.Table.HideAllUIRows();
                //this.TableProperties.UnwireEvents();
                this.VisibleColumn.TableModel.View.BeginInit();

                var sortedColumn = this.VisibleColumn.TableModel.TableProperties.SortColumns.Where(s => s.ColumnName == this.VisibleColumn.MappingName).FirstOrDefault();
                ListSortDirection sortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), args.SortString);
                if (sortedColumn == null)
                {
                    var newSortColumn = new GridDataSortColumn() { ColumnName = this.VisibleColumn.MappingName, SortDirection = sortDirection };
                    if (!this.visibleColumn.TableModel.Table.HasGroups)
                    {
                        if (this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { newSortColumn }, null, NotifyCollectionChangedAction.Add))
                        {
                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Clear();
                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Add(newSortColumn);
                            this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { newSortColumn }, null, NotifyCollectionChangedAction.Add);
                        }
                    }
                    else
                    {
                        var sortColumnClone = new List<GridDataSortColumn>();
                        this.visibleColumn.TableModel.GetSortColumnsNotInGroup().ForEach<GridDataSortColumn>(g =>
                        {
                            sortColumnClone.Add(g);
                        });

                        if (sortColumnClone != null)
                        {
                            if (this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { newSortColumn }, null, NotifyCollectionChangedAction.Add))
                            {
                                foreach (var removecolumns in sortColumnClone)
                                {
                                    this.visibleColumn.TableModel.TableProperties.SortColumns.Remove(removecolumns);
                                }
                            }

                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Add(newSortColumn);
                            this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { newSortColumn }, null, NotifyCollectionChangedAction.Add);
                        }
                    }
                }

                else
                {
                    sortedColumn.SortDirection = sortDirection;
                    this.VisibleColumn.TableModel.TableProperties.SortColumns.Remove(sortedColumn);
                    this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanged(null, new List<GridDataSortColumn>() { sortedColumn }, NotifyCollectionChangedAction.Remove);
                    if (!this.visibleColumn.TableModel.Table.HasGroups)
                    {
                        if (this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortedColumn }, null, NotifyCollectionChangedAction.Add))
                        {
                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Clear();
                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Add(sortedColumn);
                            this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortedColumn }, null, NotifyCollectionChangedAction.Add);
                        }
                    }
                    else
                    {
                        if (this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortedColumn }, null, NotifyCollectionChangedAction.Add))
                        {
                            this.VisibleColumn.TableModel.TableProperties.SortColumns.Add(sortedColumn);
                            this.VisibleColumn.TableModel.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortedColumn }, null, NotifyCollectionChangedAction.Add);
                        }
                    }
                }

                this.VisibleColumn.TableModel.isSortingApplied = true;
                //Cancelling the AddNew row when we do sorting
                if (this.VisibleColumn.TableModel.View.IsAddingNew)
                    this.VisibleColumn.TableModel.View.CancelNew();
                this.VisibleColumn.TableModel.View.EndInit();
                this.VisibleColumn.TableModel.IsInSort = false;
#if !SILVERLIGHT
                //this.grid.InvalidateCell(GridRangeInfo.Row(0));
                this.VisibleColumn.TableModel.Grid.InvalidateCell(GridRangeInfo.Row(0).ToCellSpan(this.VisibleColumn.TableModel), true);
#else
                this.InvalidateCell(GridRangeInfo.Row(0));
#endif
                //this.TableProperties.WireEvents();
                rowHeights.ResumeUpdates();
            }
        }

        /// <summary>
        /// Called when [filter popup host popup opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.PopupOpenedEventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostPopupOpened(object sender, PopupOpenedEventArgs args)
        {
            var FilterPopupHost = sender as GridDataExcelLikeFilterPane;
            if (this.PART_FilterPopupHost == null)
                this.PART_FilterPopupHost = this.GetTemplateChild("PART_FilterDropDown") as GridDataExcelLikeFilterPane;
            this.LoadExcelLikeAdvanceFiltering();
            List<FilterElement> distinctList = null;
            if (this.visibleColumn.TableModel == null || this.visibleColumn.TableModel.View == null)
                return;

            var records = this.VisibleColumn.TableModel.View.Records;
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
            var viewRecords = records.Select(
                            x => this.VisibleColumn.TableModel.Table.GetValue(x.Data, this.VisibleColumn.MappingName))
                                             .Distinct();

            distinctList = viewRecords.Select(item =>
                                                                 new FilterElement
                                                                 {
                                                                     IsSelected = true,
                                                                     ActualValue = item,
                                                                     FormatedString = this.GetFormatedString
                                                                 }).ToList();
            
#else
            var viewRecords = records.Select(
                                        x => this.VisibleColumn.TableModel.Table.GetValue(x.Data, this.VisibleColumn.MappingName))
                                                         .Distinct();
            distinctList = viewRecords.Select(item =>
                                                                 new FilterElement
                                                                 {
                                                                     IsSelected = true,
                                                                     ActualValue = item,
                                                                     FormatedString = this.GetFormatedString
                                                                 }).ToList();
#endif



            if (this.visibleColumn.Filters.Any())
            {
                if (!(this.visibleColumn.TableModel.View is GridDataTableCollectionViewWrapper))
                {
                    IExcelLikeFilterExt excelFilterView = this.VisibleColumn.TableModel.View as IExcelLikeFilterExt;
                    IEnumerable<object> distinctRecords = null;
                    System.Linq.Expressions.Expression columnPredicate = null;
                    ParameterExpression columnParamExpression = null;
                    ParameterExpression paramExpression = null;
                    System.Linq.Expressions.Expression predicate = null;

                    var filteredRecords = this.VisibleColumn.TableModel.View.SourceCollection.AsQueryable();

                    excelFilterView.GetColumnPredicateExpression(filteredRecords, out columnPredicate, out columnParamExpression, VisibleColumn);
                    filteredRecords = filteredRecords.Where(columnParamExpression, System.Linq.Expressions.Expression.Not(columnPredicate));
                    excelFilterView.ExcelFilterPredicates(filteredRecords, out predicate, out paramExpression, VisibleColumn.MappingName);
                    if (predicate != null)
                        filteredRecords = filteredRecords.Where(paramExpression, predicate);
#if SyncfusionFramework4_0
                    if (!(filteredRecords is EnumerableQuery))
                        distinctRecords = ((IQueryable<object>)filteredRecords).ToList().Select(x => this.VisibleColumn.TableModel.Table.GetValue(x, this.VisibleColumn.MappingName)).Distinct();
                    else
#endif
                        distinctRecords = filteredRecords.Cast<object>().Select(x => this.VisibleColumn.TableModel.Table.GetValue(x, this.VisibleColumn.MappingName)).Distinct();

                    distinctRecords.ForEach((o) =>
                    {
                        distinctList.Add(new FilterElement()
                        {
                            ActualValue = o,
                            FormatedString = this.GetFormatedString
                        });
                    });
                }
                else
                {
                    PropertyDescriptorCollection clonedItemsProperties = null;
                    var source = (this.VisibleColumn.TableModel.View as GridDataTableCollectionViewWrapper).GetClonedSource(out clonedItemsProperties);
                    var columnFilterString = this.GetColumnFilterString(VisibleColumn);
                    var filterString = (this.VisibleColumn.TableModel.View as GridDataTableCollectionViewWrapper).GetFilterString(VisibleColumn.MappingName);

                    columnFilterString = "NOT(" + columnFilterString + ")";
                    if (!columnFilterString.Contains("NULL"))
                    {
                        columnFilterString = columnFilterString + " OR [" + visibleColumn.MappingName.ToString() + "] is NULL";
                    }

                    if (!string.IsNullOrEmpty(filterString))
                        filterString = ("(" + columnFilterString + ")").AndPredicate() + filterString;
                    else
                        filterString = columnFilterString;

                    source.RowFilter = filterString;

                    var distinctRecords = source.Cast<object>().Select(x => clonedItemsProperties.GetValue(x, VisibleColumn.MappingName)).Distinct();
                    distinctRecords.ForEach((o) =>
                    {
                        distinctList.Add(new FilterElement()
                        {
                            ActualValue = o,
                            FormatedString = this.GetFormatedString
                        });
                    });
                }
            }

            if (distinctList != null)
                distinctList.ForEach(lstItem => lstItem.PropertyChanged += this.PART_FilterPopupHost.OnFilterElementPropertyChanged);

            args.ItemsSource = distinctList;
            if (this.visibleColumn.Filters.Count > 0)
            {
                FilterPopupHost.ClearFilterEnable = true;
            }
            else
                FilterPopupHost.ClearFilterEnable = false;
        }


        private string GetFormatedString(object item)
        {
            if (item == null)
                return GridDataResourceWrapper.BlankFilterText;

            var columnStyle = this.VisibleColumn.ColumnStyle;
            if (columnStyle == null)
                return item.ToString();

            var hasformat = !String.IsNullOrEmpty(columnStyle.Format);

            if (!hasformat && VisibleColumn.CanFormatExcelLikeFilterText)
            {
                if (columnStyle.CellType == "DoubleEdit" || columnStyle.CellType == "ComboBox" || columnStyle.CellType == "DropDownList" || columnStyle.CellType == "CurrencyEdit" ||
                    columnStyle.CellType == "PercentEdit" || columnStyle.CellType=="UpDownEdit")
                    return this.TableModel.CellModels[columnStyle.CellType].GetFormattedText(columnStyle, item, GridCellBaseTextInfo.DisplayText);
            }

            if (!hasformat)
                return item.ToString();

            if (item is DateTime)
            {
                return Convert.ToDateTime(item).ToString(columnStyle.Format);
            }
            else if (item != DBNull.Value)
            {
                var formatable = (IFormattable)item;
                return formatable.ToString(columnStyle.Format, columnStyle.GetCulture(true));
            }
            return item.ToString();
        }
        private String GetColumnFilterString(GridDataVisibleColumn column)
        {
            var filterString = string.Empty;
            var firstLoop = false;

            int loopCount = column.Filters.Count;
            var srcItems = column.Filters;

            for (int i = 0; i < loopCount; i++)
            {
                var fp = srcItems[i];
                // For null value filtering we need to check null values.
                if (fp.FilterValue == DBNull.Value)
                {
                     var visibleColumn = this.TableModel.TableProperties.VisibleColumns[column.MappingName];
                     if (visibleColumn.ColumnType == typeof(string))
                     {
                         var columnName = (this.VisibleColumn.TableModel.View as GridDataTableCollectionViewWrapper).EscapeSpecialChars(column.MappingName);
                         if (!firstLoop)
                         {
                             if (fp.FilterType == FilterType.Equals)
                                 filterString = "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                             if (fp.FilterType == FilterType.NotEquals)
                                 filterString = "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                             firstLoop = true;
                         }
                         else
                         {
                             if (fp.FilterType == FilterType.Equals)
                             {
                                 filterString = filterString.OrPredicate();
                                 filterString += "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                             }
                             else
                             {
                                 filterString = filterString.AndPredicate();
                                 filterString += "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                             }
                         }
                     }
                     else
                     {
                         var columnName = (this.VisibleColumn.TableModel.View as GridDataTableCollectionViewWrapper).EscapeSpecialChars(column.MappingName);
                         if (!firstLoop)
                         {
                             if (fp.FilterType == FilterType.Equals)
                                 filterString = columnName + " is NULL";
                             if (fp.FilterType == FilterType.NotEquals)
                                 filterString = columnName + " is NOT NULL";
                             firstLoop = true;
                         }
                         else
                         {
                             if (fp.FilterType == FilterType.Equals)
                             {
                                 filterString = filterString.OrPredicate();
                                 filterString += columnName + " is NULL";
                             }
                             else
                             {
                                 filterString = filterString.AndPredicate();
                                 filterString += columnName + " is NOT NULL";
                             }
                         }
                     }
                }
                else if (fp.FilterValue != null)
                {
                    if (!firstLoop)
                    {
                        filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                        firstLoop = true;
                    }
                    else
                    {
                        if (fp.PredicateType == PredicateType.Or)
                        {
                            filterString = filterString.OrPredicate();
                            filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                        }
                        else
                        {
                            filterString = filterString.AndPredicate();
                            filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                        }
                    }
                }
            }

            return filterString;
        }


        /// <summary>
        /// Called when [filter popup host clear menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostClearMenuItemClick(object sender, EventArgs args)
        {
            if (this.visibleColumn.TableModel == null)
                return;
            this.VisibleColumn.TableModel.IsInFilter = true;
            this.ClearCurrentColumnFilter();
            this.VisibleColumn.TableModel.IsInFilter = false;

            this.visibleColumn.TableModel.UpdateSelectedRanges();
        }


        /// <summary>
        /// Called when [filter popup host ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.OkButtonClikEventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostOkButtonClick(object sender, OkButtonClikEventArgs args)
        {
            
            this.ApplyFilters();
            
            this.VisibleColumn.TableModel.UpdateSelectedRanges();
        }

        private void ApplyFilters()
        {
            if (this.PART_FilterPopupHost == null)
                return;

            var source = (this.PART_FilterPopupHost.ItemsSource as IEnumerable<FilterElement>);
            if (source == null)
                return;
            int checkedItemsCount, unCheckedItemsCount;

            this.VisibleColumn.TableModel.IsInFilter = true;
            checkedItemsCount = source.Where(x => x.IsSelected).Count();
            unCheckedItemsCount = this.PART_FilterPopupHost.FilterListBoxItem.Count() - checkedItemsCount;
            CreateFilterPredicates(source, checkedItemsCount, unCheckedItemsCount);
            //(this.VisibleColumn.TableModel.View as IExcelLikeFilterExt).IsExcelLikeFilter = true;
            this.VisibleColumn.TableModel.IsInFilter = false;
            this.VisibleColumn.TableModel.FilterColumn(this.VisibleColumn, filterPredicate, true);
            //(this.VisibleColumn.TableModel.View as IExcelLikeFilterExt).IsExcelLikeFilter = false;
        }

        List<FilterPredicate> filterPredicate = null;
        private void CreateFilterPredicates(IEnumerable<FilterElement> source,int checkedItemsCount,int unCheckedItemsCount)
        {
#if !Silverlight4
            HashSet<object> sourceHashset = new HashSet<object>(source.ToList<object>());
#endif
            if (unCheckedItemsCount == 0 && !this.PART_FilterPopupHost.isSourceChangedasSearchedItems)
            {
                if (filterPredicate != null && filterPredicate.Count > 0)
                    filterPredicate.Clear();
                else
                    filterPredicate = new List<FilterPredicate>();
            }
            else
            {

                if ((checkedItemsCount > unCheckedItemsCount && unCheckedItemsCount > 0) || checkedItemsCount == 0)
                {
                    filterPredicate = source.Where(x => !x.IsSelected).Select(x => new FilterPredicate() { FilterBehavior = VisibleColumn.FilterBehavior, FilterType = FilterType.NotEquals, FilterValue = x.ActualValue, PredicateType = PredicateType.And }).ToList<FilterPredicate>();
                    if (this.PART_FilterPopupHost.isSourceChangedasSearchedItems)
                        this.PART_FilterPopupHost.FilterListBoxItem.ForEach((o) =>
                        {
#if Silverlight4
                            if (!source.Contains(o))
#else
                            if (!sourceHashset.Contains(o))
#endif
                                filterPredicate.Add(new FilterPredicate() { FilterBehavior = VisibleColumn.FilterBehavior, FilterType = FilterType.NotEquals, FilterValue = o.ActualValue, PredicateType = PredicateType.And });
                        });
                }
                else
                    filterPredicate = source.Where(x => x.IsSelected).Select(x => new FilterPredicate() { FilterBehavior = VisibleColumn.FilterBehavior, FilterType = FilterType.Equals, FilterValue = x.ActualValue, PredicateType = PredicateType.Or }).ToList<FilterPredicate>();

                if (VisibleColumn.Filters.Any() && VisibleColumn.TableModel.View.FilterPredicates.Any(x => !x.Equals(VisibleColumn) && x.Filters.Any()) && filterPredicate.Any())
                {
                    bool isTypeEqual = filterPredicate.FirstOrDefault().FilterType == VisibleColumn.Filters.FirstOrDefault().FilterType;

                    if (isTypeEqual)
                    {
                        foreach (var fp in VisibleColumn.Filters)
                        {
                            var fpElement = source.FirstOrDefault(x => x.ActualValue.Equals(fp.FilterValue));
                            if (fpElement == null)
                                continue;

                            if (filterPredicate.Any(x => x.FilterValue.Equals(fp.FilterValue)))
                                continue;

                            if ((fp.FilterType == FilterType.NotEquals && !fpElement.IsSelected) || (fp.FilterType == FilterType.Equals && fpElement.IsSelected))
                                filterPredicate.Add(new FilterPredicate() { FilterBehavior = fp.FilterBehavior, FilterType = fp.FilterType, FilterValue = fp.FilterValue, PredicateType = fp.PredicateType });

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the advance filtering.
        /// </summary>
        private void LoadExcelLikeAdvanceFiltering()
        {
            if (this.VisibleColumn.ColumnType == typeof(int) || this.VisibleColumn.ColumnType == typeof(Double) || this.VisibleColumn.ColumnType==typeof(Decimal))
            {
                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.NumberFilters;
                this.PART_FilterPopupHost.AscendingSortString = GridDataResourceWrapper.NumberAscending;
                this.PART_FilterPopupHost.DescendingSortString = GridDataResourceWrapper.NumberDescending;
            }
            else if (this.VisibleColumn.ColumnType == typeof(DateTime))
            {
                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.DateFilters;
                this.PART_FilterPopupHost.AscendingSortString = GridDataResourceWrapper.DateTimeAscending;
                this.PART_FilterPopupHost.DescendingSortString = GridDataResourceWrapper.DateTimeDescending;
            }

            else
            {
                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.TextFilters;
                this.PART_FilterPopupHost.AscendingSortString = GridDataResourceWrapper.StringAscending;
                this.PART_FilterPopupHost.DescendingSortString = GridDataResourceWrapper.StringDescending;
            }
        }

        /// <summary>
        /// Gets the date time pattern string.
        /// </summary>
        /// <param name="dateTimeEdit">The date time edit.</param>
        /// <param name="dateTimePattern">The date time pattern.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object GetDateTimePatternString(GridDateTimeEditStyleInfo dateTimeEdit, DateTimePattern dateTimePattern, CultureInfo culture)
        {
            object result = null;
            switch (dateTimePattern)
            {

                case DateTimePattern.ShortDate:
                    result = culture.DateTimeFormat.ShortDatePattern;
                    break;
                case DateTimePattern.ShortTime:
                    result = culture.DateTimeFormat.ShortTimePattern;
                    break;
                case DateTimePattern.LongDate:
                    result = culture.DateTimeFormat.LongDatePattern;
                    break;
                case DateTimePattern.LongTime:
                    result = culture.DateTimeFormat.LongTimePattern;
                    break;
                case DateTimePattern.FullDateTime:
                    result = culture.DateTimeFormat.FullDateTimePattern;
                    break;
                case DateTimePattern.MonthDay:
                    result = culture.DateTimeFormat.MonthDayPattern;
                    break;
                case DateTimePattern.RFC1123:
                    result = culture.DateTimeFormat.RFC1123Pattern;
                    break;
                case DateTimePattern.SortableDateTime:
                    result = culture.DateTimeFormat.SortableDateTimePattern;
                    break;
                case DateTimePattern.UniversalSortableDateTime:
                    result = culture.DateTimeFormat.UniversalSortableDateTimePattern;
                    break;
                case DateTimePattern.YearMonth:
                    result = culture.DateTimeFormat.YearMonthPattern;
                    break;
                case DateTimePattern.CustomPattern:
                    if (dateTimeEdit.HasCustomPattern)
                    {
                        result = dateTimeEdit.CustomPattern;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Clears the current column filter.
        /// </summary>
        protected void ClearCurrentColumnFilter()
        {
            if (this.visibleColumn.TableModel == null)
                return;
            // var excelFilter = (this.VisibleColumn.TableModel.View as IExcelLikeFilterExt); Unused local variable
            //excelFilter.IsExcelLikeFilter = true;
            //this.VisibleColumn.Filters.Clear();
            this.VisibleColumn.TableModel.ClearFilterColumn(this.VisibleColumn, null, FilterType.Equals, Data.PredicateType.And, true, true);
            //excelFilter.IsExcelLikeFilter = false;

        }

        /// <summary>
        /// Updates the excel filter details.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        private void UpdateExcelFilterDetails(FilterElement currentItem, Nullable<bool> SelectAllCheckBoxChecked)
        {
            var excelFilter = (this.TableModel.View as IExcelLikeFilterExt);
            if (excelFilter == null)
            {
                return;
            }
            var filterColumns = this.VisibleColumn.TableModel.View.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();

            excelFilter.IsExcelLikeFilter = true;

            // To handle the select all filter
            excelFilter.IsSelectAllFiltered = SelectAllCheckBoxChecked;

            if ((currentItem == null || SelectAllCheckBoxChecked != null && propertyChangedFromSelectAll) && filterColumns.Count == 0)
            {
                return;
            }
        }

        #endregion

        #endregion  

        #region Styling

        private void ApplyStyle(GridStyleInfo style)
        {
            if (style == null)
                return;

            //this.Background = style.Background;
            var uiElement = this.PART_ContentPresenter;
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridModel.ActiveGridView);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridModel.ActiveGridView, style.CellRowColumnIndex);
            }
            uiElement.SetValue(TextBox.MarginProperty, margins);
            GridFontInfo font = style.ReadOnlyFont;
            uiElement.SetValue(TextBox.FontFamilyProperty, font.FontFamily);
            uiElement.SetValue(TextBox.FontSizeProperty, font.FontSize);
            uiElement.SetValue(TextBox.FontStretchProperty, font.FontStretch);
            uiElement.SetValue(TextBox.FontWeightProperty, font.FontWeight);
            uiElement.SetValue(TextBox.FontStyleProperty, font.FontStyle);
            uiElement.SetValue(TextBox.TextDecorationsProperty, font.TextDecorations);
            if (font.Orientation != 0)
                uiElement.RenderTransform = new RotateTransform(font.Orientation);

            //uiElement.SetValue(TextBox.TextAlignmentProperty, HorizontalAlignmentToTextAlignment(style.HorizontalAlignment));                
            //uiElement.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
            var dataGrid = this.FindParentElementOfType<GridDataControl>();
            if (dataGrid != null)
            {
                if (dataGrid.StackedHeaderRows.Count != 0)
                    uiElement.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
            }
            uiElement.SetValue(TextBox.HorizontalContentAlignmentProperty, style.HorizontalAlignment);
            uiElement.SetValue(TextBox.VerticalAlignmentProperty, style.VerticalAlignment);
            uiElement.SetValue(TextBox.VerticalContentAlignmentProperty, style.VerticalAlignment);
            //uiElement.SetValue(TextBox.ForegroundProperty, style.Foreground);
            //uiElement.SetValue(TextBlock.BackgroundProperty, style.Background);
            uiElement.SetValue(TextBox.TextWrappingProperty, style.TextWrapping);
            uiElement.Tag = style.TextWrapping;
            uiElement.SetValue(TextBox.CharacterCasingProperty, style.CharacterCasing);
            uiElement.SetValue(TextBox.AutoWordSelectionProperty, style.AutoWordSelection);
            uiElement.SetValue(TextBox.AcceptsReturnProperty, style.AcceptsReturn);
        }

        private TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
        {
            TextAlignment textAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

                case HorizontalAlignment.Stretch:
                    textAlignment = TextAlignment.Justify;
                    break;
            }

            return textAlignment;
        }

        #endregion

        #region Excel filtering mode

        private GridDataTableModel TableModel
        {
            get
            {
                if (this.HasVisibleColumn)
                {
                    return this.VisibleColumn.TableModel;
                }

                return null;
            }
        }
       

        private bool NeedsRefresh
        {
            get;
            set;
        }

        private GridDataHeaderCellModel HeaderCellModel
        {
            get
            {
                var style = this.RenderStyle;
                return style.CellModel as GridDataHeaderCellModel;
            }
        }

        private void ShowOrHideRow(object filterValue, bool show, bool canApplyFilter)
        {
            if (this.VisibleColumn.ColumnType == typeof(DateTime)
                && this.VisibleColumn.ColumnStyle != null
                && this.VisibleColumn.ColumnStyle.DateTimeEdit != null
                && filterValue != null
                )
            {
                var result = default(DateTime);
                if (DateTime.TryParseExact(filterValue.ToString(), this.VisibleColumn.ColumnStyle.DateTimeEdit.CustomPattern, null, DateTimeStyles.None, out result))
                {
                    this.TableModel.FilterColumn(this.VisibleColumn, result, show, FilterType.NotEquals, canApplyFilter);
                    return;
                }
            }

            this.TableModel.FilterColumn(this.VisibleColumn, filterValue, show, FilterType.NotEquals, canApplyFilter);
        }       
        
        #endregion

        #region ColumnOptions

        private bool HasVisibleColumn
        {
            get
            {
                return this.VisibleColumn != null;
            }
        }

        private void UnloadColumnOptions()
        {
            if (this.PART_ColumnOptionsPopupHost == null)
            {
                return;
            }

            this.PART_ColumnOptionsPopupHost.LostFocus -= new RoutedEventHandler(this.ColumnOptionsPopupHost_LostFocus);
            this.PART_ColumnOptionsPopupHost.Opened -= new EventHandler(columnOptionsPopupHost_Opened);
            this.PART_ColumnOptionsPopupHost = null;
        }

        protected virtual void LoadColumnOptions(Popup columnOptionsPopupHost)
        {
            var columnOptionsPane = new GridDataColumnOptionsPane() { Name = "PART_ColumnOptionsPane", VisibleColumn = VisibleColumn };
            columnOptionsPane.SetHeaderCellControl(this);
            columnOptionsPane.Background = this.ColumnOptionsBackground;
            columnOptionsPane.Foreground = this.ColumnOptionsForeground;
            columnOptionsPane.ColumnOptionsButtonBackground = this.ColumnOptionsButtonBackground;
            columnOptionsPane.ColumnOptionsButtonBorderBrush = this.ColumnOptionsButtonBorderBrush; // To add ColumnOptionsPane BorderBrush Style
            columnOptionsPane.RenderStyle = this.RenderStyle;
            var textBinding = new Binding("Text") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            columnOptionsPane.SetBinding(GridDataColumnOptionsPane.TextProperty, textBinding);
            columnOptionsPane.Style = this.ColumnOptionPaneStyle;
            var filterButtonVisibilityBinding = new Binding("FilterButtonVisibility") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            columnOptionsPane.SetBinding(GridDataColumnOptionsPane.FilterButtonVisibilityProperty, filterButtonVisibilityBinding);

            var gridControl = this.FindParentElementOfType<GridDataControl>();

            if (this.TableModel != null && this.TableModel.TableProperties.VisualStyle == VisualStyle.Metro && GridDataControl.GetOverrideVisualStyle(gridControl))
            {
                
                ResourceDictionary rd = new ResourceDictionary();
                string sourcePath = @"/Syncfusion.Grid.Wpf;component/GridDataControl/ExcelLikeFiltering/Themes/" +
                                    this.TableModel.TableProperties.VisualStyle.ToString() + ".xaml";
                rd.Source = new Uri(sourcePath, UriKind.RelativeOrAbsolute);

                MergeMetroBrush(rd, gridControl);

                columnOptionsPane.Resources.MergedDictionaries.Clear();
                columnOptionsPane.Resources.MergedDictionaries.Add(rd);
            }
            // set the child
            columnOptionsPopupHost.Child = columnOptionsPane;
            columnOptionsPopupHost.Opened += new EventHandler(columnOptionsPopupHost_Opened);
            columnOptionsPane.TemplateApplied += (s, e) =>
            {
                this.IsInSuspend = true;
                if (this.VisibleColumn.ColumnStyle != null)
                {
                    columnOptionsPane.HAlignment = this.VisibleColumn.ColumnStyle.HorizontalAlignment;
                    columnOptionsPane.VAlignment = this.VisibleColumn.ColumnStyle.VerticalAlignment;
                }

                this.IsInSuspend = false;
            };
        }

        void columnOptionsPopupHost_Opened(object sender, EventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.CloseFilterDropDown();
            }
        }

        private void ColumnOptionsPopupHost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsColumnOptionsDropDownOpen && (!this.IsMouseOverColumnOptionsPopup || !this.StaysOpenOnEdit))
            {
                this.CloseColumnOptionsDropDown();
            }
        }

        private void OnColumnOptionsPaneLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                var el = e.OriginalSource as FrameworkElement;
                if (el != null)
                {
                    if (e.OriginalSource == this)
                    {
                        if ((Mouse.Captured == null) || !GridUtil.IsObjectDescendantOfParent(sender as DependencyObject, e.OriginalSource as DependencyObject))
                        {
                            this.CloseColumnOptionsDropDown();
                        }
                    }
                }
                else
                {
                    this.CloseColumnOptionsDropDown();
                }
            }
        }

        public virtual void CloseColumnOptionsDropDown()
        {
            if (this.IsColumnOptionsDropDownOpen)
            {
                this.ClearValue(GridDataHeaderCellControl.IsColumnOptionsDropDownOpenProperty);
                if (this.IsColumnOptionsDropDownOpen)
                {
                    this.IsColumnOptionsDropDownOpen = false;
                }
            }
        }

        #endregion

        #region AdvancedFiltering

        
        protected virtual void LoadAdvancedFilteringMode(Border filterParentBorder)
        {            

            if (!this.isAdvancedFilteringModeLoaded)
            {
                this.isAdvancedFilteringModeLoaded = true;
                this.IsAdvanceFiltering = true;
            }

            if (this.VisibleColumn != null && this.VisibleColumn.FilterPane != null)
            {
                this.AdvancedFilteringPane = this.VisibleColumn.FilterPane as IGridDataFilterAction;               
            }

            var filterPane = this.AdvancedFilteringPane as GridDataFilteringPane;
            string visualStyle = this.VisibleColumn.TableModel != null ? this.VisibleColumn.TableModel.TableProperties.VisualStyle.ToString() : "Default";
            SetVisalStyleForFilterPopUp(visualStyle,true);
            if (filterPane != null && filterPane.Parent != null)
            {
                var parentBorder = filterPane.Parent as Border;
                parentBorder.Child = null;
            }

            if (filterParentBorder != null)
            {
                filterParentBorder.Child = filterPane;
            }

            if (filterPane != null)
            {
                filterPane.VisibleColumn = this.VisibleColumn;
                VisualContainer.SetWantsMouseInput(filterPane, true);
                var filterWrapper = filterPane.GetFilterWrapper();

                if (filterWrapper != null && filterWrapper.VisibleColumn == null)
                {
                    filterWrapper.SetVisibleColumn(this.VisibleColumn);
                }

                if (filterWrapper != null && this.VisibleColumn != null && this.VisibleColumn.Filters != null && this.VisibleColumn.Filters.Count > 0)
                {
                    var filterPredicate = this.VisibleColumn.Filters[this.VisibleColumn.Filters.Count - 1];
                    filterWrapper.IsInSuspend = !this.IsLoaded;
                    filterWrapper.FilterValue = filterPredicate.FilterValue;
                    filterWrapper.FilterType = filterPredicate.FilterType;
                    if (filterPane.CurrentFilterType != filterWrapper.FilterType)
                    {
                        filterPane.CurrentFilterType = filterWrapper.FilterType;
                    }

                    filterWrapper.MatchCase = filterPredicate.IsCaseSensitive;
                    if (filterPane.MatchCase != filterPredicate.IsCaseSensitive)
                    {
                        filterPane.MatchCase = filterPredicate.IsCaseSensitive;
                    }

                    filterWrapper.PredicateType = filterPane.PredicateType = filterPredicate.PredicateType;
                    if (filterPane.PredicateType != filterPredicate.PredicateType)
                    {
                        filterPane.PredicateType = filterPredicate.PredicateType;
                    }

                    if (filterWrapper.IsInSuspend)
                    {
                        filterWrapper.IsInSuspend = false;
                    }
                }

                if (filterWrapper != null)
                {
                    filterWrapper.IsInSuspend = true;
                    filterWrapper.Background = this.TableModel.GetFilterPopupBackgroundBrush();
                    filterWrapper.Foreground = this.TableModel.GetFilterPopupForeGroundBrush();
                    this.AdvancedFilteringPane.SetDataContext(filterWrapper);
                    filterWrapper.IsInSuspend = false;
                }
            }
        }

        private void SetFilterPanePlacement()
        {
            if (this.HasVisibleColumn && this.TableModel != null && this.VisibleColumn.FilterPanePosition != null)
            {
                this.PART_PopupHost.Placement = this.visibleColumn.FilterPanePosition.FilterPanePlacement;

                this.PART_PopupHost.HorizontalOffset = this.visibleColumn.FilterPanePosition.HorizontalOffset;
                this.PART_PopupHost.VerticalOffset = this.visibleColumn.FilterPanePosition.VerticalOffset;

                if (this.PART_PopupHost.Placement == PlacementMode.Custom && this.visibleColumn.FilterPanePosition.HasCallback)
                {
                    this.PART_PopupHost.CustomPopupPlacementCallback = this.visibleColumn.FilterPanePosition.CustomPlacementCallback;
                }
            }
            else if (this.TableModel != null && this.TableModel.TableProperties != null && this.TableModel.TableProperties.FilterPanePosition != null)
            {
                this.PART_PopupHost.Placement = this.TableModel.TableProperties.FilterPanePosition.FilterPanePlacement;

                this.PART_PopupHost.HorizontalOffset = this.TableModel.TableProperties.FilterPanePosition.HorizontalOffset;
                this.PART_PopupHost.VerticalOffset = this.TableModel.TableProperties.FilterPanePosition.VerticalOffset;

                if (this.PART_PopupHost.Placement == PlacementMode.Custom && this.TableModel.TableProperties.FilterPanePosition.HasCallback)
                {
                    this.PART_PopupHost.CustomPopupPlacementCallback = this.TableModel.TableProperties.FilterPanePosition.CustomPlacementCallback;
                }
            }
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.CloseFilterDropDown();
            }

            e.Handled = true;
        }

        #endregion

        private void PopupHost_Opened(object sender, EventArgs e)
        {
            if (PART_PopupHost == null) // SD17144 fixed for after scrolling the popup gets null
                this.PART_PopupHost = this.GetTemplateChild(GridDataHeaderCellControl.TemplatePopup) as Popup;
            if (this.VisibleColumn.IsAdvancedFilteringMode)
            {
                this.LoadAdvancedFilteringMode(this.PART_DropDownBorder);
                this.IsKeyFocusedForAdvancedFilterTextBox = true;
                var filterPopupGesture = this.AdvancedFilteringPane as IGridDataFilterAction;
                if (filterPopupGesture != null)
                    filterPopupGesture.Invoke();
                this.IsKeyFocusedForAdvancedFilterTextBox = false;
            }

            if (this.IsColumnOptionsDropDownOpen)
            {
                this.CloseColumnOptionsDropDown();
            }           
        }


        /// <summary>
        /// Occurs when IsDropDownOpen property is changed.
        /// </summary>
        public event EventHandler IsDropDownOpenChanged;

        private void RaiseOnIsDropDownChanged()
        {
            if (this.IsDropDownOpenChanged != null)
            {
                this.IsDropDownOpenChanged(this, EventArgs.Empty);
            }
        }

        public bool IsInSuspend
        {
            get;
            internal set;
        }

        public void RefreshFilteringMode(GridStyleInfo style)
        {
            this.RefreshFilteringMode(this.IsInSuspend, style);
        }

        internal bool IsAdvancedFilteringMode = false;

        internal void RefreshFilteringMode(bool Suspend, GridStyleInfo style)
        {
            if (style != null)
                this.CurrentStyle = (GridDataStyleInfo)style;

            if (Suspend)
                return;
            if (this.TableModel != null && this.TableModel.TableProperties != null && this.TableModel.TableProperties.ShowFilterBar)
            {
                this.FilterButtonVisibility = Visibility.Collapsed;
                return;
            }

            if (this.VisibleColumn != null)
            {
                this.VisibleColumn.AllowFilter = this.FilterButtonVisibility == Visibility.Visible ? true : false;
                this.IsFilterApplied = this.VisibleColumn.Filters.Count > 0;
            }
            if (!this.ExcelLikeFilterAdvVisibility)
            {
                var filterParentBorder = this.GetTemplateChild("PART_DropDownBorder") as Border;
                if (filterParentBorder != null)
                {
                    if (this.VisibleColumn != null && !this.VisibleColumn.IsAdvancedFilteringMode)
                    {
                        IsAdvancedFilteringMode = false;
                    }
                    else
                    {
                        this.LoadAdvancedFilteringMode(filterParentBorder);
                        IsAdvancedFilteringMode = true;
                    }
                }
            }
        }

        #region Overrides

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if ((!this.IsMouseOverPopupHost && this.IsDropDownOpen) || !this.StaysOpenOnEdit)
            {
                this.CloseFilterDropDown();
            }

            if (!this.IsMouseOverColumnOptionsPopup && this.IsColumnOptionsDropDownOpen)
            {
                this.CloseColumnOptionsDropDown();
            }

            base.OnLostFocus(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                var el = e.OriginalSource as FrameworkElement;
                if (el != null)
                {
                    if (e.OriginalSource == this)
                    {
                        if ((Mouse.Captured == null) || !GridUtil.IsObjectDescendantOfParent(this, e.OriginalSource as DependencyObject))
                        {
                            this.CloseFilterDropDown();
                            this.CloseColumnOptionsDropDown();
                        }
                    }
                    //else if (GridUtil.IsObjectDescendantOfParent(this, e.OriginalSource as DependencyObject))
                    //{
                    //    if ((this.IsDropDownOpen && (Mouse.Captured == null)) || this.IsColumnOptionsDropDownOpen && (Mouse.Captured == null))
                    //    {
                    //        Mouse.Capture(this, CaptureMode.SubTree);
                    //        e.Handled = true;
                    //    }
                    //}
                }
                else
                {
                    this.CloseFilterDropDown();
                    this.CloseColumnOptionsDropDown();
                }
            }

            base.OnLostMouseCapture(e);
        }

        // in DEBUG mode, While developing, closing down the popup results in some odd behavior sometimes.
#if !DEBUG
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if ((!this.IsMouseOverPopupHost && this.IsDropDownOpen) || !this.StaysOpenOnEdit)
            {
                if (!this.IsKeyFocusedForAdvancedFilterTextBox)
                {
                    this.CloseFilterDropDown();
                }
            }

            if (!this.IsMouseOverColumnOptionsPopup && this.IsColumnOptionsDropDownOpen)
            {
                this.CloseColumnOptionsDropDown();
            }

            base.OnLostKeyboardFocus(e);
        }
#endif

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.IsMouseOverInnerTextBlock = false;
                this.IsMouseOverPopupHost = false;
            }

            if (this.IsColumnOptionsDropDownOpen)
            {
                this.IsMouseOverColumnOptionsPopup = false;
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            if (this.ExcelLikeFilterAdvVisibility && this.PART_PopupHost != null && !this.PART_PopupHost.IsOpen)
            {
                if (!this.IsAdvanceFiltering)
                {
                    if (this.PART_FilterPopupHost != null && !this.PART_FilterPopupHost.IsMouseOver)
                    {
                        this.CloseFilterDropDown();
                    }
                }

                else if ((this.IsDropDownOpen && !this.IsMouseOverPopupHost && !this.IsMouseOverInnerTextBlock) || !this.StaysOpenOnEdit)
                {
                    this.CloseFilterDropDown();
                }
            }
            if ((this.IsColumnOptionsDropDownOpen && !this.IsMouseOverColumnOptionsPopup) || !this.StaysOpenOnEdit)
            {
                this.CloseColumnOptionsDropDown();
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //if (Mouse.Captured == this.PART_FilterPopupHost&&this.ExcelLikeFilterAdvVisibility)
            //{
            //    Mouse.Capture(null);
            //    this.CloseFilterDropDown();
            //     e.Handled = true;
            //}
            if (Mouse.Captured == this && e.OriginalSource == this)
            {
                this.CloseFilterDropDown();
                this.CloseColumnOptionsDropDown();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {            
            if (this.IsDropDownOpen)
            {
                var flag = this.PART_PopupHost != null ? this.PART_PopupHost.IsMouseOver : false;
                if(!flag)
                    flag=this.PART_FilterPopupHost!=null ? this.PART_FilterPopupHost.IsMouseOver:false;
                this.IsMouseOverPopupHost = flag;

                flag = this.PART_ContentPresenter != null ? this.PART_ContentPresenter.IsMouseOver : false;
                this.IsMouseOverInnerTextBlock = flag;
            }

            if (this.IsColumnOptionsDropDownOpen)
            {
                var flag = this.PART_ColumnOptionsPopupHost != null ? this.PART_ColumnOptionsPopupHost.IsMouseOver : false;
                this.IsMouseOverColumnOptionsPopup = flag;
            }
            e.Handled = true;
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsKeyboardFocusWithin)
            {
                if (!this.IsDropDownOpen)
                {
                    if (e.Delta < 0)
                    {
                        // moving down
                        this.NavigateNextLine();
                    }
                    else
                    {
                        // moving up
                        this.NavigatePreviousLine();
                    }
                }
            }
            else if (this.IsDropDownOpen)
            {
                e.Handled = true;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (this.IsReadOnly)
            {
                Visual originalSource = e.OriginalSource as Visual;
                Visual editableTextBlock = this.PART_ContentPresenter;
                if (((originalSource != null) && (editableTextBlock != null)) && editableTextBlock.IsAncestorOf(originalSource))
                {
                    if (this.IsDropDownOpen && !this.StaysOpenOnEdit)
                    {
                        this.CloseFilterDropDown();
                    }
                    else if (!this.IsKeyboardFocusWithin)
                    {
                        this.Focus();
                        e.Handled = true;
                    }
                }
            }

            base.OnPreviewMouseDown(e);
        }

        #endregion

        private void RegisterToOpenOnLoad()
        {
            this.Loaded += (sender, args) =>
            {
                // Open popup after it has rendered (Loaded is fired before 1st render) 
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate(object param) { CoerceValue(IsDropDownOpenProperty); return null; }), null);
            };
        }

        /// <summary>
        /// Shows the popup host.
        /// </summary>
        protected void ShowPopup()
        {
            if (!this.IsDropDownOpen)
            {
                this.IsDropDownOpen = true;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            UnWireExcelLikeFilteringEvents();
            if (this.VisibleColumn != null)
            {
                this.VisibleColumn.Filters.CollectionChanged -= OnFiltersCollectionChanged;
            }
            if (this.CurrentStyle != null)
            {
               // this.CurrentStyle.Dispose();
                this.CurrentStyle = null;
            }
            if (this.PART_FilterPopupHost != null)
            {
                this.PART_FilterPopupHost.Dispose();
                this.PART_FilterPopupHost = null;
            }
            this.PART_ColumnOptionsPopupHost = null;
            this.PART_PopupHost = null;
            this.PART_DropDownBorder = null;
            this.ToggleButton = null;
            this.PART_ContentPresenter = null;
            this.visibleColumn = null;
            this.RenderStyle = null;
        }

        #endregion
    }

    internal class GridDataEnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            object paramvalue = Enum.Parse(value.GetType(), parameterString);
            if (paramvalue.Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }

    internal class GridDataVisibilityBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }
        #endregion
    }

    public class TextAlignmentConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextAlignment textAlignment;
            var horizontalAlignment =(HorizontalAlignment)value;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

                case HorizontalAlignment.Stretch:
                    textAlignment = TextAlignment.Justify;
                    break;
            }

            return textAlignment;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridDataMinWidthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return GridDataHeaderCellControl.MinimumWidth;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(double), typeof(int))]
    internal class GridDataDoubleToIntegerConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class GridDataFilterToggleButton : ToggleButton
    {
        static GridDataFilterToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataFilterToggleButton), new FrameworkPropertyMetadata(typeof(GridDataFilterToggleButton)));
        }

        public GridDataFilterToggleButton()
        {
        }

        public static readonly DependencyProperty FilterInnerBrushProperty = DependencyProperty.Register(
            "FilterInnerBrush",
            typeof(Brush),
            typeof(GridDataFilterToggleButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush FilterInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataFilterToggleButton.FilterInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataFilterToggleButton.FilterInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterOuterBrushProperty = DependencyProperty.Register(
            "FilterOuterBrush",
            typeof(Brush),
            typeof(GridDataFilterToggleButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush FilterOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataFilterToggleButton.FilterOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataFilterToggleButton.FilterOuterBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterHoverInnerBrushProperty = DependencyProperty.Register(
            "FilterHoverInnerBrush",
            typeof(Brush),
            typeof(GridDataFilterToggleButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush FilterHoverInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataFilterToggleButton.FilterHoverInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataFilterToggleButton.FilterHoverInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterHoverOuterBrushProperty = DependencyProperty.Register(
            "FilterHoverOuterBrush",
            typeof(Brush),
            typeof(GridDataFilterToggleButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush FilterHoverOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataFilterToggleButton.FilterHoverOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataFilterToggleButton.FilterHoverOuterBrushProperty, value);
            }
        }

        #region IsFilterApplied (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsFilterApplied property.
        /// </summary>
        public bool IsFilterApplied
        {
            get { return (bool)GetValue(IsFilterAppliedProperty); }
            set { SetValue(IsFilterAppliedProperty, value); }
        }

        public static readonly DependencyProperty IsFilterAppliedProperty = DependencyProperty.Register("IsFilterApplied", typeof(bool), typeof(GridDataFilterToggleButton), new PropertyMetadata(false));

        #endregion

        #region FilterAppliedInnerBrush (DependencyProperty)

        /// <summary>
        /// Gets / Sets the FilterAppliedInnerBrush.
        /// </summary>
        public Brush FilterAppliedInnerBrush
        {
            get { return (Brush)GetValue(FilterAppliedInnerBrushProperty); }
            set { SetValue(FilterAppliedInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterAppliedInnerBrushProperty = DependencyProperty.Register("FilterAppliedInnerBrush", typeof(Brush), typeof(GridDataFilterToggleButton), new PropertyMetadata(Brushes.Red));

        #endregion

        
    }

}
