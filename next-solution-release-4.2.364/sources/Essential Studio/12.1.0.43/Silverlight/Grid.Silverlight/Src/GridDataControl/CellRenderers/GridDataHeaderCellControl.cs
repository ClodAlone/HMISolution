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
    //using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Linq;
    using System.Linq;
    using System.Collections.Generic;
    using Syncfusion.Windows.Data;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Media.Animation;
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.Tools.Controls;
    using System.Globalization;
    using System.Collections.Specialized;

    /// <summary>
    /// GridDataHeaderCellControl displays the header content for GridDataControl.
    /// </summary>
    /// <remarks>
    /// It has Dependency properties that is used in a customized ControlTemplate for
    /// showing the default values.
    /// </remarks>
    [TemplatePart(Name = GridDataHeaderCellControl.PART_DropDownBorder, Type = typeof(Border))]
    [TemplatePart(Name = GridDataHeaderCellControl.MainBorder, Type = typeof(Border))]
    [TemplatePart(Name = GridDataHeaderCellControl.MainGrid, Type = typeof(Grid))]
    [TemplatePart(Name = GridDataHeaderCellControl.TemplatePopup, Type = typeof(Popup))]
    [TemplatePart(Name = GridDataHeaderCellControl.TemplateTextblock, Type = typeof(TextBlock))]

    public class GridDataHeaderCellControl : Control, IDisposable
    {
        public const string MainBorder = "PART_MainBorder";
        public const string MainGrid = "PART_MainGrid";
        public string SelectAllText = GridDataResourceWrapper.SelectAllFilter;
        public new const double MinWidth = 25.0d;
        private const string PART_DropDownBorder = "PART_DropDownBorder";
        private Border mainBorder;
        private Grid mainGrid;
        private Border filterBorder;
        private Border sortBorder;
        private TextBlock NumberBlock;
        private Border textBorder;
        private ToggleButton filterButton;


        #region Private Variables

        //This flag set while first time propertyChanged triggered.
        bool FilterElementPropertyChangedCalled = false;
        //This falg used to find out propertychanged calls from SelectAll Checkbox
        bool propertyChangedFromSelectAll = false;

        #endregion

        /// <summary>
        /// DependencyProperty for IsDropDownOpen.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
        "IsDropDownOpen",
        typeof(bool),
        typeof(GridDataHeaderCellControl),
        new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        /// DependencyProperty for IsReadOnly.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SortStringProperty = DependencyProperty.Register("SortString", typeof(string), typeof(GridDataHeaderCellControl), new PropertyMetadata(String.Empty));

        public string SortString
        {
            get { return (string)this.GetValue(GridDataHeaderCellControl.SortStringProperty); }
            internal set { this.SetValue(GridDataHeaderCellControl.SortStringProperty, value); }
        }
        /// <summary>
        /// DependencyProperty for StaysOpenOnEdit.
        /// </summary>
        public static readonly DependencyProperty StaysOpenOnEditProperty = DependencyProperty.Register(
            "StaysOpenOnEdit",
            typeof(bool),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(false));

        public const string TemplatePopup = "PART_Popup";
        public const string TemplateTextblock = "TemplateTextblock";
        public const string TemplateCheckedListBox = "PART_CheckedListBox";

        public GridDataHeaderCellControl()
        {
            this.DefaultStyleKey = typeof(GridDataHeaderCellControl);
            this.IsKeyFocusedForAdvancedFilterTextBox = false;
            //this.MouseEnter += new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            this.MouseLeave += new MouseEventHandler(GridDataHeaderCellControl_MouseLeave);
            DependencyObjectExtensions.SetEnableMousePosition(this, true);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.VisibleColumn != null)
            {
                this.VisibleColumn.Filters.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnFiltersCollectionChanged);
                this.visibleColumn.PropertyChanged -= new PropertyChangedEventHandler(visibleColumn_PropertyChanged);
                this.VisibleColumn = null;
            }

            this.RenderStyle = null;
        }

        #endregion

        void GridDataHeaderCellControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.TableModel != null)
            {
                if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }

        void GridDataHeaderCellControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.TableModel != null)
            {
                if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
                {
                    VisualStateManager.GoToState(this, "Normal", false);

                    if (this.mainBorder != null)
                    {
                        var groups = VisualStateManager.GetVisualStateGroups(this.mainBorder);
                        var commonStates = groups[0] as VisualStateGroup;
                        var visualState = commonStates.States[1] as VisualState;
                        var objectParentFrame = visualState.Storyboard.Children[0] as ObjectAnimationUsingKeyFrames;
                        var objectBorderFrame = visualState.Storyboard.Children[1] as ObjectAnimationUsingKeyFrames;
                        var discreteFrame = objectParentFrame.KeyFrames[0] as DiscreteObjectKeyFrame;
                        var discreteBorderFrame = objectBorderFrame.KeyFrames[0] as DiscreteObjectKeyFrame;
                        Storyboard.SetTargetName(objectParentFrame, mainBorder.Name);
                        Storyboard.SetTargetName(objectBorderFrame, mainBorder.Name);
                        if (e.OriginalSource == filterBorder)
                        {
                            Storyboard.SetTargetName(objectParentFrame, filterBorder.Name);
                            Storyboard.SetTargetName(objectBorderFrame, filterBorder.Name);
                            discreteFrame.Value = this.HeaderOptionsHoverBackground;
                            discreteBorderFrame.Value = this.HeaderOptionsBorderBrush;
                        }
                        else if (e.OriginalSource == sortBorder || e.OriginalSource == NumberBlock)
                        {
                            Storyboard.SetTargetName(objectParentFrame, sortBorder.Name);
                            Storyboard.SetTargetName(objectBorderFrame, sortBorder.Name);
                            discreteFrame.Value = this.HeaderOptionsHoverBackground;
                            discreteBorderFrame.Value = this.HeaderOptionsBorderBrush;
                        }
                        else
                        {
                            discreteFrame.Value = this.HoverBackground;
                        }
                    }

                    VisualStateManager.GoToState(this, "MouseOver", false);
                }
            }
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCell = d as GridDataHeaderCellControl;
            if (headerCell.AdvancedFilteringPane != null)
            {
                var gridDataFilteringPane = headerCell.AdvancedFilteringPane as GridDataFilteringPane;
                var wrapperInstance = gridDataFilteringPane.GetFilterWrapper();
                wrapperInstance.Background = (Brush)args.NewValue;
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

        /// <summary>
        /// Gets the TextBlock UIElement associated with the Header Cell Control.
        /// </summary>
        public TextBlock TextBlockPart
        {
            get;
            private set;
        }

        public static readonly DependencyProperty IsAdvanceFilteringProperty = DependencyProperty.Register("IsAdvanceFiltering", typeof(bool), typeof(GridDataHeaderCellControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsAdvanceFilteringChanged)));


        public bool IsAdvanceFiltering
        {
            get { return (bool)this.GetValue(GridDataHeaderCellControl.IsAdvanceFilteringProperty); }
            set { this.SetValue(GridDataHeaderCellControl.IsAdvanceFilteringProperty, value); }
        }

        private static void OnIsAdvanceFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCell = d as GridDataHeaderCellControl;
            if ((bool)args.NewValue)
            {
                if (headerCell.PART_FilterPopupHost != null)
                {
                    headerCell.PART_FilterPopupHost.Visibility = Visibility.Collapsed;
                }
            }
        }


        /// <summary>
        /// Dependency Property for IsColumnOptionsDropDownOpen.
        /// </summary>
        public static readonly DependencyProperty IsColumnOptionsDropDownOpenProperty = DependencyProperty.Register(
            "IsColumnOptionsDropDownOpen",
            typeof(bool),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(OnIsColumnOptionsOpenChanged));

        private static void OnIsColumnOptionsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl gdc = d as GridDataHeaderCellControl;
            gdc.RaiseOnIsColumnOptionsChanged();

            //gdc.CoerceValue(ToolTipService.IsEnabledProperty);*/
        }

        public event EventHandler IsColumnOptionsChanged;

        private void RaiseOnIsColumnOptionsChanged()
        {
            if (this.IsColumnOptionsChanged != null)
            {
                this.IsColumnOptionsChanged(this, EventArgs.Empty);
            }
        }

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
            get
            {
                return (bool)this.GetValue(GridDataHeaderCellControl.IsDropDownOpenProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.IsDropDownOpenProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnOptionsBackgroundProperty = DependencyProperty.Register(
           "ColumnOptionsBackground",
           typeof(Brush),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush ColumnOptionsBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.ColumnOptionsBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnOptionsForegroundProperty = DependencyProperty.Register(
           "ColumnOptionsForeground",
           typeof(Brush),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush ColumnOptionsForeground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsForegroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.ColumnOptionsForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnOptionsButtonBackgroundProperty = DependencyProperty.Register(
           "ColumnOptionsButtonBackground",
           typeof(Brush),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsButtonBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.ColumnOptionsButtonBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnOptionsCloseButtonBrushProperty = DependencyProperty.Register(
           "ColumnOptionsCloseButtonBrush",
           typeof(Brush),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.ColumnOptionsCloseButtonBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.ColumnOptionsCloseButtonBrushProperty, value);
            }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
            "HoverBackground",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush HoverBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.HoverBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.HoverBackgroundProperty, value);
            }
        }



        public Brush HoverForeground
        {
            get { return (Brush)GetValue(HoverForegroundProperty); }
            set { SetValue(HoverForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoverForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));



        public static readonly DependencyProperty HeaderInnerBorderBrushProperty = DependencyProperty.Register(
            "HeaderInnerBorderBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush HeaderInnerBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.HeaderInnerBorderBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.HeaderInnerBorderBrushProperty, value);
            }
        }

        public static readonly DependencyProperty HeaderInnerBorderThicknessProperty = DependencyProperty.Register(
            "HeaderInnerBorderThickness",
            typeof(Thickness),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(GridDataHeaderCellControl.HeaderInnerBorderThicknessProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.HeaderInnerBorderThicknessProperty, value);
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get { return (Brush)GetValue(HeaderOptionsHoverBackgroundProperty); }
            set { SetValue(HeaderOptionsHoverBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderOptionsHoverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderOptionsHoverBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsHoverBackground", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(null));




        public Brush HeaderOptionsBorderBrush
        {
            get { return (Brush)GetValue(HeaderOptionsBorderBrushProperty); }
            set { SetValue(HeaderOptionsBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderOptionsBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderOptionsBorderBrushProperty =
            DependencyProperty.Register("HeaderOptionsBorderBrush", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(null));




        public Brush HeaderOptionsCheckedBackground
        {
            get { return (Brush)GetValue(HeaderOptionsCheckedBackgroundProperty); }
            set { SetValue(HeaderOptionsCheckedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderOptionsCheckedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderOptionsCheckedBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsCheckedBackground", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(null));


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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <b>True</b> if this instance ; otherwise, <b>false</b>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(GridDataHeaderCellControl.IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visible column to which this instance belongs.
        /// </summary>
        private GridDataVisibleColumn visibleColumn = null;
        public GridDataVisibleColumn VisibleColumn
        {
            get
            {
                return this.visibleColumn;
            }
            private set
            {
                if (value != null)
                {
                    if (this.visibleColumn != null)
                    {
                        this.visibleColumn.PropertyChanged -= new PropertyChangedEventHandler(visibleColumn_PropertyChanged);
                        this.visibleColumn.Filters.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnFiltersCollectionChanged);
                    }
                    this.visibleColumn = value;
                    if (this.visibleColumn != null)
                    {
                        this.IsFilterApplied = this.VisibleColumn.Filters.Count > 0;
                        this.visibleColumn.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnFiltersCollectionChanged);
                        this.visibleColumn.PropertyChanged += new PropertyChangedEventHandler(visibleColumn_PropertyChanged);
                    }
                }
            }
        }

        internal void SetVisibleColumn(GridDataVisibleColumn column)
        {
            if (column != null)
            {
                this.VisibleColumn = column;
                if (this.VisibleColumn.TableModel is GridDataChildTableModel)
                {
                    this.Unloaded += new RoutedEventHandler(GridDataHeaderCellControl_Unloaded);
                }
            }
        }

        private void GridDataHeaderCellControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(GridDataHeaderCellControl_Unloaded);
            if (this.VisibleColumn != null)
            {
                this.VisibleColumn.Filters.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnFiltersCollectionChanged);
                this.VisibleColumn = null;
            }
            //Unwire the ExcelLikeFiltering Events
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

        private void visibleColumn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AllowGroup")
            {
                this.SetGroupColumnWidth();
            }
        }

        public GridRenderStyleInfo RenderStyle
        {
            get;
            internal set;
        }

        /// <summary>
        /// DependencyProperty for ColumnOptionsButtonVisibility.
        /// </summary>
        public static readonly DependencyProperty ColumnOptionsButtonVisibilityProperty = DependencyProperty.Register(
            "ColumnOptionsButtonVisibility",
            typeof(Visibility),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets a value indicating whether the Column Options Button has to be shown.
        /// </summary>
        /// <value><b>True</b> if this instance ; otherwise, <b>false</b>.</value>
        public Visibility ColumnOptionsButtonVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.ColumnOptionsButtonVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.ColumnOptionsButtonVisibilityProperty, value);
            }
        }

        #region ExcelLikeFilterAdvVisibility
        public static readonly DependencyProperty ExcelLikeFilterAdvVisibilityProperty = DependencyProperty.Register("ExcelLikeFilterAdvVisibility", typeof(bool), typeof(GridDataHeaderCellControl), new PropertyMetadata(false, OnExcelLikeFilterAdvVisibilityChanged));

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

        private static void OnExcelLikeFilterAdvVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridDataHeaderCellControl;
            if ((bool)args.NewValue)
            {
                headerCellControl.PART_FilterPopupHost.Visibility = Visibility.Visible;
                //headerCellControl.PopupHost.Visibility = Visibility.Collapsed;
            }
            else
            {
                headerCellControl.PART_FilterPopupHost.Visibility = Visibility.Collapsed;
                // headerCellControl.PopupHost.Visibility = Visibility.Visible;
            }

        }

        #endregion

        /// <summary>
        /// DependencyProperty for FilterButtonVisibility.
        /// </summary>
        public static readonly DependencyProperty FilterButtonVisibilityProperty = DependencyProperty.Register(
            "FilterButtonVisibility",
            typeof(Visibility),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(Visibility.Collapsed, OnFilterButtonVisibilityChanged));

        public Visibility FilterButtonVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.FilterButtonVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterButtonVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty FilterBackgroundProperty = DependencyProperty.Register(
            "FilterBackground",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush FilterBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty FilterInnerBrushProperty = DependencyProperty.Register(
            "FilterInnerBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush FilterInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterOuterBrushProperty = DependencyProperty.Register(
            "FilterOuterBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush FilterOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterOuterBrushProperty, value);
            }
        }


        #region FilterAppliedInnerBrush (DependencyProperty)

        /// <summary>
        /// Gets / Sets the FilterAppliedInnerBrush.
        /// </summary>
        public Brush FilterAppliedInnerBrush
        {
            get { return (Brush)GetValue(FilterAppliedInnerBrushProperty); }
            set { SetValue(FilterAppliedInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterAppliedInnerBrushProperty = DependencyProperty.Register("FilterAppliedInnerBrush", typeof(Brush), typeof(GridDataHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        #endregion

        #region IsFilterApplied (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsFilterApplied property.
        /// </summary>
        public bool IsFilterApplied
        {
            get { return (bool)GetValue(IsFilterAppliedProperty); }
            set { SetValue(IsFilterAppliedProperty, value); }
        }

        public static readonly DependencyProperty IsFilterAppliedProperty = DependencyProperty.Register("IsFilterApplied", typeof(bool), typeof(GridDataHeaderCellControl), new PropertyMetadata(false, OnIsFilterAppliedChanged));

        private static void OnIsFilterAppliedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var value = (bool)args.NewValue;
            System.Diagnostics.Debug.WriteLine("Value2 {0}", value);
        }

        #endregion

        public static readonly DependencyProperty FilterHoverInnerBrushProperty = DependencyProperty.Register(
            "FilterHoverInnerBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush FilterHoverInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterHoverInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterHoverInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterHoverOuterBrushProperty = DependencyProperty.Register(
            "FilterHoverOuterBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush FilterHoverOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterHoverOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterHoverOuterBrushProperty, value);
            }
        }

        public static readonly DependencyProperty FilterHoverBackgroundProperty = DependencyProperty.Register(
            "FilterHoverBackground",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush FilterHoverBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.FilterHoverBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterHoverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for SortVisibility.
        /// </summary>
        public static readonly DependencyProperty SortVisibilityProperty = DependencyProperty.Register(
            "SortVisibility",
            typeof(Visibility),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(Visibility.Collapsed, OnSortVisibilityChanged));

        private static void OnSortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridDataHeaderCellControl;
            if (headerCellControl.mainGrid != null)
            {
                headerCellControl.SetSortColumnWidth();
            }
        }

        public static readonly DependencyProperty AscVisibilityProperty = DependencyProperty.Register(
           "AscVisibility",
           typeof(Visibility),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty DescVisibilityProperty = DependencyProperty.Register(
           "DescVisibility",
           typeof(Visibility),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(Visibility.Collapsed));

        public Visibility AscVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.AscVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.AscVisibilityProperty, value);
            }
        }

        public Visibility DescVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.DescVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.DescVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether if the Sort Icon has to be shown.
        /// </summary>
        public Visibility SortVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataHeaderCellControl.SortVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.SortVisibilityProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for SortDirection.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(ListSortDirection),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Sort Direction for this instance.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get
            {
                return (ListSortDirection)this.GetValue(GridDataHeaderCellControl.SortDirectionProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.SortDirectionProperty, value);
            }
        }

        public static readonly DependencyProperty SortPathProperty = DependencyProperty.Register(
           "SortPath",
           typeof(Path),
           typeof(GridDataHeaderCellControl),
           new PropertyMetadata(null));

        public Path SortPath
        {
            get
            {
                return (Path)this.GetValue(GridDataHeaderCellControl.SortPathProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.SortPathProperty, value);
            }
        }



        /// <summary>
        /// DependencyProperty for SortBrush.
        /// </summary>
        public static readonly DependencyProperty SortBrushProperty = DependencyProperty.Register(
            "SortBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the brush value for the Sort Icon.
        /// </summary>
        public Brush SortBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.SortBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.SortBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets the Column Options Popup host attached to this instance.
        /// </summary>
        /// <value>The popup host.</value>
        public Popup ColumnOptionsPopupHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the popup host attached to this instance.
        /// </summary>
        /// <value>The popup host.</value>
        public Popup PopupHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the PAR t_ filter popup host.
        /// </summary>
        /// <value>The PAR t_ filter popup host.</value>
        public GridDataExcelLikeFilterPane PART_FilterPopupHost { get; private set; }

        /// <summary>
        /// Gets or sets the advanced filtering pane.
        /// </summary>
        /// <value>The advanced filtering pane.</value>
        public IGridDataFilterAction AdvancedFilteringPane
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PopupHost stays open during edit.
        /// </summary>
        /// <value>
        /// <b>True</b> if ; otherwise, <b>false</b>.
        /// </value>
        public bool StaysOpenOnEdit
        {
            get
            {
                return (bool)this.GetValue(GridDataHeaderCellControl.StaysOpenOnEditProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.StaysOpenOnEditProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(GridDataHeaderCellControl.TextProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.TextProperty, value);
            }
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

        /// Gets the checked list box part attached to the PopupHost.
        /// </summary>
        /// <value>The checked list box part.</value>
        public GridDataCheckedListBoxControl CheckedListBoxPart
        {
            get;
            private set;
        }


        private bool isTemplateApplied = false;
        public override void OnApplyTemplate()
        {
            if (textBorder != null)
            {
                textBorder.MouseEnter -= new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (sortBorder != null)
            {
                sortBorder.MouseEnter -= new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (NumberBlock != null)
            {
                NumberBlock.MouseEnter -= new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);     
            }

            if (filterBorder != null)
            {
                filterBorder.MouseEnter -= new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (this.filterButton != null)
            {
                this.filterButton.Checked -= new RoutedEventHandler(filterButton_Checked);
                this.filterButton.Unchecked -= new RoutedEventHandler(filterButton_Unchecked);
            }


            //ForExcel Like Filtering
           
            this.PART_FilterPopupHost = this.GetTemplateChild("PART_FilterDropDown") as GridDataExcelLikeFilterPane;
            this.LoadExcelLikeAdvanceFiltering();

            WireExcelLikeFilteringEvents();
            if (this.VisibleColumn != null && this.visibleColumn.TableModel!=null)
            {
                this.SetVisualStyleForExcelLikeFiltering(this.VisibleColumn.TableModel.TableProperties.VisualStyle.ToString());
                this.PART_FilterPopupHost.ExcelLikeFilterAdvVisibility = !this.VisibleColumn.TableModel.TableProperties.EnableLegacyFiltering;
                this.ExcelLikeFilterAdvVisibility = !this.VisibleColumn.TableModel.TableProperties.EnableLegacyFiltering;
            }  

            if (this.SortVisibility == System.Windows.Visibility.Visible)
                this.PART_FilterPopupHost.SortOrder = this.SortDirection.ToString();
            if (!this.ExcelLikeFilterAdvVisibility)
            {
                this.PART_FilterPopupHost.ClearFilterVisibility = false;
                this.PART_FilterPopupHost.SortOptionVisibility = false;
                this.PART_FilterPopupHost.SearchOptionVisibility = false;
                this.PART_FilterPopupHost.OkCancelButtonVisibility = false;
                this.PART_FilterPopupHost.ResizingOptionVisibility = false;
                this.PART_FilterPopupHost.AdvanceFilteringOptionVisibility = false;
            }
            else
            {
                if (VisibleColumn != null)
                {
                    this.PART_FilterPopupHost.SortOptionVisibility = this.VisibleColumn.ShowSortOptioninExcelLikeFiltering;
                    this.PART_FilterPopupHost.SearchOptionVisibility = this.VisibleColumn.ShowSearchOptioninExcelLikeFiltering;
                }

            }


           
            //Till this.

            filterBorder = this.GetTemplateChild("filterBorder") as Border;
            sortBorder = this.GetTemplateChild("sortBorder") as Border;
            textBorder = this.GetTemplateChild("textBorder") as Border;
            NumberBlock = this.GetTemplateChild("NumberBlock") as TextBlock;
            filterButton = this.GetTemplateChild("toggleButton") as ToggleButton;

            if (textBorder != null)
            {
                textBorder.MouseEnter += new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (NumberBlock != null)
            {
                NumberBlock.MouseEnter += new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (sortBorder != null)
            {
                sortBorder.MouseEnter += new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (filterBorder != null)
            {
                filterBorder.MouseEnter += new MouseEventHandler(GridDataHeaderCellControl_MouseEnter);
            }

            if (this.filterButton != null)
            {
                this.filterButton.Checked += new RoutedEventHandler(filterButton_Checked);
                this.filterButton.Unchecked += new RoutedEventHandler(filterButton_Unchecked);
            }
            if (this.CheckedListBoxPart != null)
            {
                this.UnwireItems(this.CheckedListBoxPart);
                this.CheckedListBoxPart.Items.Clear();
                this.CheckedListBoxPart.LostFocus -= new RoutedEventHandler(this.CheckedListBoxPart_LostFocus);
                this.CheckedListBoxPart = null;
            }

            if (this.TextBlockPart != null)
            {
                this.TextBlockPart.LostFocus -= new RoutedEventHandler(this.InnerTextBlock_LostFocus);
                this.TextBlockPart = null;
            }

            if (this.AdvancedFilteringPane != null)
            {
                this.isAdvancedFilteringModeLoaded = false;
                this.AdvancedFilteringPane = null;
            }

            if (this.PopupHost != null)
            {
                this.PopupHost.Opened -= new EventHandler(PopupHost_Opened);
                this.PopupHost = null;
            }          

            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.mainBorder = this.GetTemplateChild(GridDataHeaderCellControl.MainBorder) as Border;
            this.mainGrid = this.GetTemplateChild(GridDataHeaderCellControl.MainGrid) as Grid;
            // sort column width
            this.SetSortColumnWidth();
            this.SetGroupColumnWidth();
            if (this.HasVisibleColumn)
            {
                var groupingIndicator = this.GetTemplateChild("PART_GroupingIndicator") as GridDataGroupingIndicator;
                groupingIndicator.SetVisibleColumn(this.visibleColumn);
            }

            this.SetFilterButtonWidth();

            this.PopupHost = this.mainGrid.FindElementOfType<Popup>(); //this.GetTemplateChild(GridDataHeaderCellControl.TemplatePopup) as Popup;
            if (this.PopupHost != null)
            {
                this.PopupHost.Opened += new EventHandler(PopupHost_Opened);
            }

            if (this.HasVisibleColumn && this.VisibleColumn.AllowFilter && this.VisibleColumn.TableModel != null && !this.VisibleColumn.IsUnbound)
            {
                var filterParentBorder = this.GetTemplateChild("PART_DropDownBorder") as Border;
                if (!this.VisibleColumn.IsAdvancedFilteringMode)
                {
                    this.LoadExcelFilteringMode(filterParentBorder, false);
                }
                else
                {
                    this.LoadAdvancedFilteringMode(filterParentBorder);
                }
            }

            GridStyleInfo style = null;

            this.TextBlockPart = this.GetTemplateChild(GridDataHeaderCellControl.TemplateTextblock) as TextBlock;
            if (this.TextBlockPart != null && this.HasVisibleColumn)
            {
                this.TextBlockPart.LostFocus += new RoutedEventHandler(this.InnerTextBlock_LostFocus);
                if (this.HasVisibleColumn && this.TableModel != null)
                {
                    var rowIndex = this.TableModel.TableProperties.StackedHeaderRows.Count;
                    var colIndex = this.TableModel.TableProperties.VisibleColumns.IndexOf(this.VisibleColumn);
                    colIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(colIndex);
                    style = this.VisibleColumn.HeaderStyle != null ? this.VisibleColumn.HeaderStyle : this.TableModel[rowIndex, colIndex];
                    var tb = this.TextBlockPart;
                }
            }
            else if (this.RenderStyle != null && this.TextBlockPart != null)
            {
                style = this.RenderStyle;
            }
            if (style != null)
            {
                var tb = this.TextBlockPart;
                var font = style.ReadOnlyFont;
                tb.FontFamily = font.FontFamily;
                tb.FontSize = font.FontSize;
                tb.FontStretch = font.FontStretch;
                tb.FontWeight = font.FontWeight;
                tb.FontStyle = font.FontStyle;
                tb.Foreground = style.Foreground;
                tb.HorizontalAlignment = style.HorizontalAlignment;
                tb.Margin = style.TextMargins.ToThickness();
                tb.Padding = style.BorderMargins.ToThickness();
                tb.TextDecorations = font.TextDecorations;
                tb.TextWrapping = style.TextWrapping;
                tb.TextTrimming = style.TextTrimming;
                tb.VerticalAlignment = style.VerticalAlignment;
            }          
        }

        


        #region ExcelLikeFilteringRegion

        /// <summary>
        /// Wires the excel like filtering events.
        /// </summary>
        void WireExcelLikeFilteringEvents()
        {
            if (this.PART_FilterPopupHost != null)
            {
                this.PART_FilterPopupHost.OkButtonClick += new OkButtonClickEventHandler(OnFilterPopupHostOkButtonClick);
                this.PART_FilterPopupHost.ClearMenuItemClick += new ClearMenuItemClickEventHandler(OnFilterPopupHostClearMenuItemClick);
                this.PART_FilterPopupHost.PopupOpened += new PopupOpenedEventHandler(OnFilterPopupHostPopupOpened);
                this.PART_FilterPopupHost.SortMenuItemClick += new SortMenuItemClickEventHandler(OnFilterPopupHostSortMenuItemClick);
                this.PART_FilterPopupHost.OnFilterElementChanged += new OnFilterElementPropertyChangedEventHandler(FilterPopupHostOnFilterElementChanged);
                this.PART_FilterPopupHost.SelectAllUnCheckBoxChecked += new SelectAllCheckBoxUnCheckedEventHandler(PART_FilterPopupHost_SelectAllUnCheckBoxChecked);
                this.PART_FilterPopupHost.SelectAllCheckBoxChecked += new SelectAllCheckBoxCheckedEventHandler(PART_FilterPopupHost_SelectAllCheckBoxChecked);
            }
        }

        /// <summary>
        /// Un Wire excel like filtering events.
        /// </summary>
        void UnWireExcelLikeFilteringEvents()
        {

            if (this.PART_FilterPopupHost != null)
            {
                this.PART_FilterPopupHost.OkButtonClick -= new OkButtonClickEventHandler(OnFilterPopupHostOkButtonClick);
                this.PART_FilterPopupHost.ClearMenuItemClick -= new ClearMenuItemClickEventHandler(OnFilterPopupHostClearMenuItemClick);
                this.PART_FilterPopupHost.PopupOpened -= new PopupOpenedEventHandler(OnFilterPopupHostPopupOpened);
                this.PART_FilterPopupHost.SortMenuItemClick -= new SortMenuItemClickEventHandler(OnFilterPopupHostSortMenuItemClick);
                this.PART_FilterPopupHost.OnFilterElementChanged -= new OnFilterElementPropertyChangedEventHandler(FilterPopupHostOnFilterElementChanged);
                this.PART_FilterPopupHost.SelectAllUnCheckBoxChecked -= new SelectAllCheckBoxUnCheckedEventHandler(PART_FilterPopupHost_SelectAllUnCheckBoxChecked);
                this.PART_FilterPopupHost.SelectAllCheckBoxChecked -= new SelectAllCheckBoxCheckedEventHandler(PART_FilterPopupHost_SelectAllCheckBoxChecked);
            }

        }

        /// <summary>
        /// Uns the hook filter element property changed.
        /// </summary>
        /// <param name="filterList">The filter list.</param>
        void UnHookFilterElementPropertyChanged(List<FilterElement> filterList)
        {
            foreach (var item in filterList)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(this.PART_FilterPopupHost.OnFilterElementPropertyChanged);
            }
        }


        void SetVisualStyleForExcelLikeFiltering(string VisualStyle)
        {

            if (this.PART_FilterPopupHost.Style != null)
            {
                return;
            }
            this.PART_FilterPopupHost.VisualStyle = VisualStyle;


            string sourcePath = "";
            if (VisualStyle != "Default" && VisualStyle != "Office2003" && VisualStyle!="Custom" && VisualStyle != "DefaultOffice2007Blue" && VisualStyle != "DefaultOffice2007Silver" && VisualStyle != "DefaultOffice2007Black")
            {
                sourcePath = "/Syncfusion.Grid.Silverlight;component/GridDataControl/ExcelLikeFiltering/Themes/" + VisualStyle.ToString() + ".xaml";
            }
            else
            {
                sourcePath = @"/Syncfusion.Grid.Silverlight;component/GridDataControl/ExcelLikeFiltering/Themes/Generic.xaml";
            }

            ResourceDictionary rd1 = new ResourceDictionary();
            rd1.Source = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
            // this.PART_FilterPopupHost.Resources.MergedDictionaries.Add(rd1);
            this.Resources.MergedDictionaries.Add(rd1);
        }

        /// <summary>
        /// Handles the SelectAllCheckBoxChecked event of the PART_FilterPopupHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxCheckedEventArgs"/> instance containing the event data.</param>
        void PART_FilterPopupHost_SelectAllCheckBoxChecked(object sender, SelectAllCheckBoxCheckedEventArgs args)
        {
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
            UpdateExcelFilterDetails(null, false);
            this.TableModel.IsInFilter = true;

            this.TableModel.BeginInit();
            var items = args.FilterElements.ToList<FilterElement>();
            items.Where(c => c.IsSelected == true).ForEach<FilterElement>(c =>
            {

                this.ShowOrHideRow(c.ActualValue, false, false);
            });

            this.ShowOrHideRow(null, false, true);
            this.TableModel.ForceFilterRefresh();
            this.TableModel.EndInit();
            this.TableModel.IsInFilter = false;
        }

        /// <summary>
        /// Filters the popup host on filter element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.OnFilterElementPropertyChangedEventArgs"/> instance containing the event data.</param>
        void FilterPopupHostOnFilterElementChanged(object sender, OnFilterElementPropertyChangedEventArgs args)
        {
            FilterElementPropertyChangedCalled = true;
            var filterElement = args.FilterElement;
            if (!propertyChangedFromSelectAll)
            {
                UpdateExcelFilterDetails(filterElement, args.SelectAllChecked);

                this.TableModel.IsInFilter = true;

                {
                    if (!(bool)filterElement.IsSelected)
                    {
                        this.ShowOrHideRow(filterElement.ActualValue, false, true);
                    }
                    else
                    {
                        this.ShowOrHideRow(filterElement.ActualValue, true, true);
                    }
                }
            }

            this.TableModel.IsInFilter = false;
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
                ListSortDirection sortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), args.SortString, false);
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
                this.VisibleColumn.TableModel.InvalidateCell(GridRangeInfo.Row(0));
#endif
                //this.TableProperties.WireEvents();
                rowHeights.ResumeUpdates();
            }
        }


        /// <summary>
        /// Closes the opened filter popup.
        /// </summary>
        void CloseOpenedFilterPopup()
        {

            var grid = this.FindParentElementOfType<GridDataControl>();
            if (grid.CurrentScrollChild == null)
                grid.CurrentScrollChild = this.FindParentElementOfType<ScrollControlChildFrame>();
            var children = DependencyObjectExtensions.FindElementsOfType<GridDataHeaderCellControl>(grid.CurrentScrollChild);
           
            var parent = this.FindParentElementOfType<ScrollControlChildFrame>();            

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null && child.PART_FilterPopupHost != this.PART_FilterPopupHost && child.IsDropDownOpen)
                    {
                        child.CloseFilterDropDown();
                    }
                }
            }
            grid.CurrentScrollChild = this.FindParentElementOfType<ScrollControlChildFrame>();
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
            List<FilterElement> distinctList = null;

            if (this.visibleColumn.TableModel == null || this.visibleColumn.TableModel.View == null)
                return;

            var records = this.VisibleColumn.TableModel.View.Records;
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
            if (this.visibleColumn.Filters.Any())
            {

                IExcelLikeFilterExt excelFilterView = this.VisibleColumn.TableModel.View as IExcelLikeFilterExt;
                IEnumerable<object> distinctRecords = null;
                System.Linq.Expressions.Expression columnPredicate = null;
                System.Linq.Expressions.ParameterExpression columnParamExpression = null;
                System.Linq.Expressions.ParameterExpression paramExpression = null;
                System.Linq.Expressions.Expression predicate = null;

                var filteredRecords = this.VisibleColumn.TableModel.View.SourceCollection.AsQueryable();

                excelFilterView.GetColumnPredicateExpression(filteredRecords, out columnPredicate, out columnParamExpression, VisibleColumn);
                filteredRecords = filteredRecords.Where(columnParamExpression, System.Linq.Expressions.Expression.Not(columnPredicate));
                excelFilterView.ExcelFilterPredicates(filteredRecords, out predicate, out paramExpression, VisibleColumn.MappingName);
                if (predicate != null)
                    filteredRecords = filteredRecords.Where(paramExpression, predicate);

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

            if (distinctList != null)
                distinctList.ForEach(lstItem => lstItem.PropertyChanged += this.PART_FilterPopupHost.OnFilterElementPropertyChanged);

            args.ItemsSource = distinctList;
            if (this.visibleColumn.Filters.Count > 0)
            {
                FilterPopupHost.ClearFilterEnable = true;
            }
            else
                FilterPopupHost.ClearFilterEnable = false;

            SetOffsetforPopUp();
        }

        private string GetFormatedString(object item)
        {
            if (item != null && item is DateTime)
            {
                if (this.VisibleColumn.ColumnStyle != null && !String.IsNullOrEmpty(this.VisibleColumn.ColumnStyle.Format))
                    return Convert.ToDateTime(item).ToString(this.VisibleColumn.ColumnStyle.Format);
                else
                    return item.ToString();
            }
            else if (item != null && item != DBNull.Value)
            {
                if (this.VisibleColumn.ColumnStyle != null && !String.IsNullOrEmpty(this.VisibleColumn.ColumnStyle.Format))
                    return string.Format(this.VisibleColumn.ColumnStyle.Format, item);
                else
                    return item.ToString();
            }
            else
                return GridDataResourceWrapper.BlankFilterString;
        }

        /// <summary>
        /// Sets the offsetfor pop up.
        /// </summary>
        void SetOffsetforPopUp()
        {
            var toggleButton = this.GetTemplateChild("toggleButton") as GridDataFilterToggleButton;
            this.PART_FilterPopupHost.FilterPopUp.VerticalOffset = this.ActualHeight - 5;
            var popUpWindowWidth = 250d;
            UIElement applicationRoot = Application.Current.RootVisual;
            var popUpXEnd = toggleButton.PointFromRootVisual().X + popUpWindowWidth;
            if (popUpXEnd > applicationRoot.RenderSize.Width)
            {
                //If Popup opens outside of the window then it off set should change.
                this.PART_FilterPopupHost.FilterPopUp.HorizontalOffset = toggleButton.ActualWidth;
            }
            else
            {
                var offset = (this.PointFromRootVisual().X + this.ActualWidth - this.PART_FilterPopupHost.FilterPopUp.PointFromRootVisual().X) - toggleButton.ActualWidth;
                this.PART_FilterPopupHost.FilterPopUp.HorizontalOffset = offset - 5;
            }
        }

        /// <summary>
        /// Loads the advance filtering.
        /// </summary>
        private void LoadExcelLikeAdvanceFiltering()
        {
            if (VisibleColumn == null||visibleColumn.TableModel == null || VisibleColumn.TableModel.View==null)
            {
                return;
            }

            if (this.VisibleColumn.ColumnType == typeof(int) || this.VisibleColumn.ColumnType == typeof(Double))
            {


                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.NumberFilters;
            }
            else if (this.VisibleColumn.ColumnType == typeof(DateTime))
            {
                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.DateFilters;

            }

            else
            {
                this.PART_FilterPopupHost.ColumnType = GridDataResourceWrapper.TextFilters;

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
        /// Clears the current column filter.
        /// </summary>
        protected void ClearCurrentColumnFilter()
        {
            if (this.visibleColumn.TableModel == null)
                return;
            var excelFilter = (this.VisibleColumn.TableModel.View as IExcelLikeFilterExt);
            //excelFilter.IsExcelLikeFilter = true;
            this.VisibleColumn.TableModel.ClearFilterColumn(this.VisibleColumn, null, FilterType.Equals, PredicateType.And, true, true);
            //excelFilter.IsExcelLikeFilter = false;

        }

        /// <summary>
        /// Called when [filter popup host ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.OkButtonClikEventArgs"/> instance containing the event data.</param>
        void OnFilterPopupHostOkButtonClick(object sender, OkButtonClikEventArgs args)
        {
            if (this.visibleColumn.TableModel == null)
                return;
            var excelFilter = (this.VisibleColumn.TableModel.View as IExcelLikeFilterExt);
            //excelFilter.IsExcelLikeFilter = true;

            this.ApplyFilters();

            //excelFilter.IsExcelLikeFilter = false;

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
        private void CreateFilterPredicates(IEnumerable<FilterElement> source, int checkedItemsCount, int unCheckedItemsCount)
        {
#if !Silverlight4
            HashSet<object> sourceHashset = new HashSet<object>(source);
#endif
            if (unCheckedItemsCount == 0 && !this.PART_FilterPopupHost.isSourceChangedasSearchedItems)
            {
                if (filterPredicate != null && filterPredicate.Count > 0)
                    filterPredicate.Clear();
            }
            else
            {

                if (checkedItemsCount > unCheckedItemsCount && unCheckedItemsCount > 0)
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

        #endregion


        private void filterButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                filterBorder.Background = Brushes.Transparent;
                filterBorder.BorderBrush = Brushes.Transparent;
            }
        }

        private void filterButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                filterBorder.Background = this.TableModel.GetHeaderOptionsCheckedBackground();
                filterBorder.BorderBrush = this.TableModel.GetHeaderOptionsBorderBrush();
            }
        }

        private void CheckedListBoxPart_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsDropDownOpen && (!this.IsMouseOverInnerTextBlock || !this.IsMouseOverPopupHost || !this.StaysOpenOnEdit))
            {
                this.CloseFilterDropDown();
            }
        }

        /// <summary>
        /// Loads the excel pane with the filters. The header cell renderer will always reload the filters OnActivated.
        /// </summary>
        /// <param name="filterParentBorder"></param>
        /// <param name="shouldUpdateFilters"></param>
        private void LoadExcelFilteringMode(Border filterParentBorder, bool shouldUpdateFilters)
        {          

            this.CheckedListBoxPart = new GridDataCheckedListBoxControl();
            this.CheckedListBoxPart.Height = 250;
            this.CheckedListBoxPart.Width = 180;
            this.CheckedListBoxPart.LostFocus += new RoutedEventHandler(this.CheckedListBoxPart_LostFocus);
            // filterParentBorder.Child = this.CheckedListBoxPart;
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                this.CheckedListBoxPart.Background = this.ColumnOptionsBackground;
                this.CheckedListBoxPart.Foreground = this.ColumnOptionsForeground;
            }
            VisualContainer.SetWantsMouseInput(this.CheckedListBoxPart, true);
            if (shouldUpdateFilters)// && !this.VisibleColumn.IsUnbound)
            {
                //this.UpdateFilters();
            }
        }

        private void WireItems(GridDataCheckedListBoxControl checkedListBox)
        {
            checkedListBox.Items.ForEach<GridDataFilterCheckedListBoxItem>(c =>
            {
                c.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertyChanged);
            });
        }
        private void UnwireItems(GridDataCheckedListBoxControl checkedListBox)
        {

            checkedListBox.Items.ForEach<GridDataFilterCheckedListBoxItem>(c =>
            {
                c.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertyChanged);
            });
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isInEnsuringSelectAll)
            {
                return;
            }

            var item = sender as GridDataFilterCheckedListBoxItem;
            this.TableModel.IsInFilter = true;
            if (e.PropertyName == "IsChecked" && item.Text == GridDataResourceWrapper.SelectAllFilter)
            {
                var checkedListBox = this.CheckedListBoxPart;
                this.UnwireItems(checkedListBox);
                this.TableModel.BeginInit();
                this.TableModel.IsInFilter = true;
                var items = checkedListBox.Items.ToList<GridDataFilterCheckedListBoxItem>();
                items.Where(c => c.IsChecked == !item.IsChecked).ForEach<GridDataFilterCheckedListBoxItem>(c =>
                {
                    c.IsChecked = item.IsChecked;
                    if (c.Text != GridDataResourceWrapper.SelectAllFilter && c.Text != this.TableModel.TableProperties.NullFilterText)
                    {
                        this.ShowOrHideRow(c.Text, c.IsChecked.Value, false);
                    }
                });
                this.ShowOrHideRow(string.Empty, true, true);
                this.TableModel.EndInit();
                this.TableModel.IsInFilter = false;
                this.WireItems(checkedListBox);
            }
            else if (e.PropertyName == "IsChecked" && item.Text != GridDataResourceWrapper.SelectAllFilter && item.Text != this.TableModel.TableProperties.NullFilterText)
            {
                if (!item.IsChecked.Value)
                {
                    this.ShowOrHideRow(item.Text, false, true);
                }
                else
                {
                    this.ShowOrHideRow(item.Text, true, true);
                }
            }
            else if (e.PropertyName == "IsChecked" && item.Text == this.TableModel.TableProperties.NullFilterText)
            {
                // its a NULL value,
                if (!item.IsChecked.Value)
                {
                    this.ShowOrHideRow(null, false, true);
                }
                else
                {
                    this.ShowOrHideRow(null, true, true);
                }
            }

            this.EnsureSelectAllItem(this.CheckedListBoxPart);
            this.TableModel.IsInFilter = false;
        }

        private bool isInEnsuringSelectAll = false;
        private void EnsureSelectAllItem(GridDataCheckedListBoxControl checkedListBox)
        {
            this.isInEnsuringSelectAll = true;
            var items = (IList)checkedListBox.Items;
            var selectedItem = items[0] as GridDataFilterCheckedListBoxItem;
            List<GridDataFilterCheckedListBoxItem> checkedItems = new List<GridDataFilterCheckedListBoxItem>();
            for (int i = 1; i < items.Count; i++)
            {
                var item = items[i] as GridDataFilterCheckedListBoxItem;
                if (item.IsChecked.HasValue && item.IsChecked.Value)
                {
                    checkedItems.Add(item);
                }
            }

            var finalCount = items.Count;
            if (checkedItems.Count == (finalCount - 1))
            {
                selectedItem.IsChecked = true;
            }
            else if (checkedItems.Count == 0)
            {
                selectedItem.IsChecked = false;
            }
            else
            {
                selectedItem.IsChecked = null;
            }
            this.isInEnsuringSelectAll = false;
        }

        private void ShowOrHideRow(object filterValue, bool show, bool canApplyFilter)
        {
            this.TableModel.FilterColumn(this.VisibleColumn, filterValue, show, FilterType.NotEquals, canApplyFilter);
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
            excelFilter.IsSelectAllFiltered = SelectAllCheckBoxChecked;// this.SelectAllCheckBox.IsChecked;

            if ((currentItem == null || SelectAllCheckBoxChecked != null && propertyChangedFromSelectAll) && filterColumns.Count == 0)
            {
                return;
            }

        }      

        private void SetSortColumnWidth()
        {
            var sortColumn = this.mainGrid.ColumnDefinitions[1];
            if (this.SortVisibility == Visibility.Visible)
            {
                sortColumn.Width = new GridLength(GridDataHeaderCellControl.MinWidth, GridUnitType.Pixel);
            }
            else
            {
                sortColumn.Width = new GridLength(0);
            }
        }

        private void SetGroupColumnWidth()
        {
            var groupColumn = this.mainGrid.ColumnDefinitions[2];
            if (this.HasVisibleColumn && this.VisibleColumn.AllowGroup && this.VisibleColumn.ShowGroupIndicator)
            {
                groupColumn.Width = new GridLength(GridDataHeaderCellControl.MinWidth, GridUnitType.Pixel);
            }
            else
            {
                groupColumn.Width = new GridLength(0);
            }
        }

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

                /* case HorizontalAlignment.Stretch:
                     textAlignment = TextAlignment.;
                     break;*/
            }

            return textAlignment;
        }

        private bool HasVisibleColumn
        {
            get
            {
                return this.VisibleColumn != null;
            }
        }

        private bool isAdvancedFilteringModeLoaded = false;
        protected virtual void LoadAdvancedFilteringMode(Border filterParentBorder)
        {
#if SILVERLIGHT
            if (filterParentBorder == null)
            {
                return;
            }
#endif
            if (!this.isAdvancedFilteringModeLoaded)
            {
                this.isAdvancedFilteringModeLoaded = true;
            }
            this.IsAdvanceFiltering = true;

            this.AdvancedFilteringPane = this.VisibleColumn.FilterPane as IGridDataFilterAction;
            var filterPane = this.AdvancedFilteringPane as GridDataFilteringPane;
            if (filterPane == null)
            {
                return;
            }

            if (filterPane.Parent != null)
            {
                var parentBorder = filterPane.Parent as Border;
                parentBorder.Child = null;
            }

            filterParentBorder.Child = filterPane;
#if SILVERLIGHT
            filterPane.KeyDown -= new KeyEventHandler(filterPane_KeyDown);
            filterPane.KeyDown += new KeyEventHandler(filterPane_KeyDown);
#endif
            filterPane.VisibleColumn = this.VisibleColumn;
            VisualContainer.SetWantsMouseInput(filterPane, true);
            var filterWrapper = filterPane.GetFilterWrapper();
            if (this.VisibleColumn.Filters != null && this.VisibleColumn.Filters.Count > 0)
            {
                var filterPredicate = this.VisibleColumn.Filters[this.VisibleColumn.Filters.Count - 1];
                if (filterWrapper.VisibleColumn == null)
                {
                    filterWrapper.SetVisibleColumn(this.VisibleColumn);
                }

                filterWrapper.IsInSuspend = !this.isTemplateApplied;
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

                filterWrapper.PredicateType = filterPredicate.PredicateType;
                if (filterPane.PredicateType != filterPredicate.PredicateType)
                {
                    filterPane.PredicateType = filterPredicate.PredicateType;
                }

                if (filterWrapper.IsInSuspend)
                {
                    filterWrapper.IsInSuspend = false;
                }
            }
            filterWrapper.IsInSuspend = true;
            filterWrapper.Background = this.Background;
            filterWrapper.Foreground = this.Foreground;
            filterWrapper.SetVisibleColumn(this.VisibleColumn);
            this.AdvancedFilteringPane.SetDataContext(filterWrapper);
            filterWrapper.IsInSuspend = false;
        }

#if SILVERLIGHT
        void filterPane_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.CloseFilterDropDown();
                    break;
            }
        }
#endif

        private void PopupHost_Opened(object sender, EventArgs e)
        {
            this.CloseOpenedFilterPopup();
            if (this.VisibleColumn.IsAdvancedFilteringMode)
            {
                this.IsKeyFocusedForAdvancedFilterTextBox = true;
                var filterPopupGesture = this.AdvancedFilteringPane as IGridDataFilterAction;
                if (filterPopupGesture == null)
                {
                    throw new InvalidOperationException("Filter popup is null");
                }
                filterPopupGesture.Invoke();
                this.IsKeyFocusedForAdvancedFilterTextBox = false;
            }           

            //This Code is Added For load the FilterPane in Nested grid.
            var filterParentBorder = this.GetTemplateChild("PART_DropDownBorder") as Border;
            if (this.VisibleColumn.IsAdvancedFilteringMode && filterParentBorder.Child == null)
            {
                this.LoadAdvancedFilteringMode(filterParentBorder);
            }
            var toggleButton = this.GetTemplateChild("toggleButton") as GridDataFilterToggleButton;
            this.PopupHost.VerticalOffset = this.ActualHeight - 5;
            var popUpWindowWidth = 209d;//RenderSize not Returning actualWidth
            UIElement applicationRoot = Application.Current.RootVisual;
            var popUpXEnd = this.PopupHost.PointFromRootVisual().X + this.RenderSize.Width - 5 + popUpWindowWidth;
            if (popUpXEnd > applicationRoot.RenderSize.Width)
            {
                //If Popup opens outside of the window then it off set should change.
                //this.PopupHost.HorizontalOffset = this.RenderSize.Width - popUpWindowWidth;
                this.PopupHost.HorizontalOffset = (toggleButton.PointFromRootVisual().X - this.PopupHost.PointFromRootVisual().X)-popUpWindowWidth;
            }
            else
            {
                this.PopupHost.HorizontalOffset = this.RenderSize.Width - 22;
            }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl gdc = d as GridDataHeaderCellControl;
            gdc.RaiseOnIsDropDownChanged();
            // if ((bool)args.NewValue && !gdc.VisibleColumn.IsAdvancedFilteringMode)
            //  gdc.UpdateFilters();
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

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl headerCell = d as GridDataHeaderCellControl;
            if (!headerCell.IsInSuspend && headerCell.visibleColumn != null)
            {
                headerCell.VisibleColumn.TableModel.TableProperties.SuspendEvents();
                headerCell.VisibleColumn.HeaderText = headerCell.Text;
                headerCell.VisibleColumn.TableModel.TableProperties.ResumeEvents();
            }
        }

        private static void OnFilterButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataHeaderCellControl headerCell = d as GridDataHeaderCellControl;
            headerCell.SetFilterButtonWidth();
            if (!headerCell.IsInSuspend)
            {
                if (headerCell.TableModel != null && headerCell.TableModel.TableProperties.ShowFilterBar)
                {
                    headerCell.FilterButtonVisibility = Visibility.Collapsed;
                    return;
                }

                headerCell.VisibleColumn.AllowFilter = headerCell.FilterButtonVisibility == Visibility.Visible ? true : false;
                if (!headerCell.isAdvancedFilteringModeLoaded)
                {
                    var filterParentBorder = headerCell.GetTemplateChild("PART_DropDownBorder") as Border;
                    if (filterParentBorder != null)
                    {
                        if (!headerCell.VisibleColumn.IsAdvancedFilteringMode)
                        {
                            headerCell.LoadExcelFilteringMode(filterParentBorder, true);
                        }
                        else
                        {
                            headerCell.LoadAdvancedFilteringMode(filterParentBorder);
                        }
                    }
                }
            }
        }

        private void SetFilterButtonWidth()
        {
            if (this.mainGrid == null)
            {
                return;
            }

            var filterColumn = this.mainGrid.ColumnDefinitions[3];
            if (this.FilterButtonVisibility == Visibility.Visible)
            {
                filterColumn.Width = new GridLength(GridDataHeaderCellControl.MinWidth, GridUnitType.Pixel);
            }
            else
            {
                filterColumn.Width = new GridLength(0);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
                bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
                switch (e.Key)
                {
                    case Key.Escape:
                        this.CloseFilterDropDown();
                        e.Handled = true;
                        break;
                }
            }
        }

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

        /*         protected override void OnMouseMove(MouseEventArgs e)
                 {
                     if (this.IsDropDownOpen)
                     {
                         var flag = this.PopupHost != null ? this.PopupHost.IsMouseOver : false;
                         this.IsMouseOverPopupHost = flag;

                         flag = this.TextBlockPart != null ? this.TextBlockPart.IsMouseOver : false;
                         this.IsMouseOverInnerTextBlock = flag;
                     }

                     if (this.IsColumnOptionsDropDownOpen)
                     {
                         var flag = this.ColumnOptionsPopupHost != null ? this.ColumnOptionsPopupHost.IsMouseOver : false;
                         this.IsMouseOverColumnOptionsPopup = flag;
                     }

                     base.OnMouseMove(e);
                 }


                 protected void OnPreviewMouseDown(MouseButtonEventArgs e)
                 {
                    /* if (this.IsReadOnly)
                     {
                         Visual originalSource = e.OriginalSource as Visual;
                         Visual editableTextBlock = this.TextBlockPart;
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

                     //base.OnPreviewMouseDown(e);
                 }*/

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

        public static readonly DependencyProperty FilterWrapperProperty = DependencyProperty.Register(
            "FilterWrapper",
            typeof(GridDataFilterWrapper),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public GridDataFilterWrapper FilterWrapper
        {
            get
            {
                return (GridDataFilterWrapper)this.GetValue(GridDataHeaderCellControl.FilterWrapperProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.FilterWrapperProperty, value);
            }
        }

        public static readonly DependencyProperty GroupingIndicatorInnerBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorInnerBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush GroupingIndicatorInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.GroupingIndicatorInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.GroupingIndicatorInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty GroupingIndicatorOuterBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorOuterBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush GroupingIndicatorOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.GroupingIndicatorOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.GroupingIndicatorOuterBrushProperty, value);
            }
        }

        public static readonly DependencyProperty GroupingIndicatorHoverInnerBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorHoverInnerBrush",
            typeof(Brush),
            typeof(GridDataHeaderCellControl),
            new PropertyMetadata(null));

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataHeaderCellControl.GroupingIndicatorHoverInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataHeaderCellControl.GroupingIndicatorHoverInnerBrushProperty, value);
            }
        }
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

            object paramvalue = Enum.Parse(value.GetType(), parameterString, true);
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

            return Enum.Parse(targetType, parameterString, true);
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
                return Visibility.Collapsed;
            }
        }
        #endregion
    }

    //internal class GridDataSortDirectionConverter : IValueConverter
    //{
    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var sortDirection = (ListSortDirection)value;
    //        if (sortDirection == ListSortDirection.Ascending)
    //        {
    //            return GridDataResources.BorderStyleAsc;
    //        }
    //        else
    //        {
    //            return GridDataResources.BorderPathDesc;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    public class GridDataMinWidthConverter : IValueConverter
    {
        public GridDataMinWidthConverter()
        {

        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return GridDataHeaderCellControl.MinWidth;
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





    public class GridDataFilterToggleButton : ToggleButton
    {
        public GridDataFilterToggleButton()
        {
            this.DefaultStyleKey = typeof(GridDataFilterToggleButton);
            this.MouseEnter += new MouseEventHandler(GridDataFilterToggleButton_MouseEnter);
            this.MouseLeave += new MouseEventHandler(GridDataFilterToggleButton_MouseLeave);
            DependencyObjectExtensions.SetEnableMousePosition(this, true);
        }

        public static readonly DependencyProperty FilterInnerBrushProperty = DependencyProperty.Register(
            "FilterInnerBrush",
            typeof(Brush),
            typeof(GridDataFilterToggleButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
        /// Gets / Sets if the filter is applied.
        /// </summary>
        public bool IsFilterApplied
        {
            get { return (bool)GetValue(IsFilterAppliedProperty); }
            set { SetValue(IsFilterAppliedProperty, value); }
        }

        public static readonly DependencyProperty IsFilterAppliedProperty = DependencyProperty.Register("IsFilterApplied", typeof(bool), typeof(GridDataFilterToggleButton), new PropertyMetadata(false, OnIsFilterAppliedChanged));

        private static void OnIsFilterAppliedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var toggleButton = dpo as GridDataFilterToggleButton;
            var value = (bool)args.NewValue;
            System.Diagnostics.Debug.WriteLine(string.Format("Value {0}", value));
            if (value)
            {
                toggleButton.ApplyFilterBrush();
            }
            else
            {
                //VisualStateManager.GoToState(toggleButton, "Normal", false);
                toggleButton.GotoNormalState();
            }
        }

        private bool isFilterAppliedSetBeforeLoaded = false;
        private void ApplyFilterBrush()
        {
            if (this.mainGrid == null)
            {
                this.isFilterAppliedSetBeforeLoaded = true;
                return;
            }

            //var groups = VisualStateManager.GetVisualStateGroups(this.mainGrid);
            //var commonStates = groups[0] as VisualStateGroup;
            //var visualState = commonStates.States[2] as VisualState;
            //var colorParentFrame1 = visualState.Storyboard.Children[0] as ObjectAnimationUsingKeyFrames;
            //var p2ColorFrame = colorParentFrame1.KeyFrames[0] as DiscreteObjectKeyFrame;
            //p2ColorFrame.Value = this.FilterHoverInnerBrush;
            if (this.p2 != null)
            {
                this.p2.Fill = this.FilterAppliedInnerBrush;
            }

            if (this.path1 != null && this.path2 != null)
            {
                VisualStateManager.GoToState(this, "visualState2", true);
                this.InvalidateMeasure();
                this.InvalidateArrange();
            }            
        }

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

        public static readonly DependencyProperty FilterAppliedInnerBrushProperty = DependencyProperty.Register("FilterAppliedInnerBrush", typeof(Brush), typeof(GridDataFilterToggleButton), new PropertyMetadata(Brushes.Transparent));

        #endregion

        private Path p1;
        private Path p2;
        private Path path1;
        private Path path2;
        private Grid mainGrid;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainGrid = this.GetTemplateChild("MainGrid") as Grid;
            this.p1 = this.GetTemplateChild("p1") as Path;
            this.p2 = this.GetTemplateChild("p2") as Path;
            this.path1 = this.GetTemplateChild("path1") as Path;
            this.path2 = this.GetTemplateChild("path2") as Path;          

            if (this.isFilterAppliedSetBeforeLoaded)
            {
                this.ApplyFilterBrush();
            }
        }

        private void GridDataFilterToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.p1 != null && this.p2 != null)
            {
                //var groups = VisualStateManager.GetVisualStateGroups(this.mainGrid);
                //var commonStates = groups[0] as VisualStateGroup;
                //var visualState = commonStates.States[1] as VisualState;
                //var colorParentFrame1 = visualState.Storyboard.Children[0] as ObjectAnimationUsingKeyFrames;
                //var p1ColorFrame = colorParentFrame1.KeyFrames[0] as DiscreteObjectKeyFrame;
                //p1ColorFrame.Value = this.FilterHoverInnerBrush;
                this.p1.Fill = this.FilterHoverInnerBrush;
                this.p2.Fill = this.FilterHoverOuterBrush;
                //var colorParentFrame2 = visualState.Storyboard.Children[1] as ObjectAnimationUsingKeyFrames;
                //var p2ColorFrame = colorParentFrame2.KeyFrames[0] as DiscreteObjectKeyFrame;
                //p2ColorFrame.Value = this.FilterHoverOuterBrush;
            }

            //VisualStateManager.GoToState(this, "MouseOver", false);
        }

        private void GridDataFilterToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsFilterApplied)
            {
                this.GotoNormalState();
                //VisualStateManager.GoToState(this, "Normal", false);
            }
            else
            {
                this.ApplyFilterBrush();
            }
        }

        private void GotoNormalState()
        {
            if (this.p1 != null && this.p2 != null)
            {
                this.p1.Fill = this.FilterOuterBrush;
                this.p2.Fill = this.FilterInnerBrush;
            }

            if (this.path1 != null && this.path2 != null)
            {
                VisualStateManager.GoToState(this, "visualState1", false);                
            }
        }
    }



    #region GroupingIndicators Implementation

    public class GridDataGroupingIndicator : ToggleButton
    {
        public GridDataGroupingIndicator()
        {
            this.DefaultStyleKey = typeof(GridDataGroupingIndicator);
        }

        private Grid mainGrid;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainGrid = this.GetTemplateChild("PART_MainGrid") as Grid;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            var groups = VisualStateManager.GetVisualStateGroups(this.mainGrid);
            var commonStates = groups[0] as VisualStateGroup;
            var visualState = commonStates.States[1] as VisualState;
            var colorParentFrame1 = visualState.Storyboard.Children[0] as ObjectAnimationUsingKeyFrames;
            var p1ColorFrame = colorParentFrame1.KeyFrames[0] as DiscreteObjectKeyFrame;
            p1ColorFrame.Value = this.GroupingIndicatorHoverInnerBrush;

            var colorParentFrame2 = visualState.Storyboard.Children[1] as ObjectAnimationUsingKeyFrames;
            var p2ColorFrame = colorParentFrame2.KeyFrames[0] as DiscreteObjectKeyFrame;
            p2ColorFrame.Value = this.GroupingIndicatorHoverInnerBrush;
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private GridDataVisibleColumn visibleColumn;

        private bool HasVisibleColumn
        {
            get
            {
                return this.visibleColumn != null && !this.visibleColumn.IsUnbound;
            }
        }

        internal void SetVisibleColumn(GridDataVisibleColumn visibleColumn)
        {
            this.visibleColumn = visibleColumn;
            this.ResolveCheckedState();
        }

        private void ResolveCheckedState()
        {
            if (this.visibleColumn.TableModel != null)
            {
                var tableProperties = this.visibleColumn.TableModel.TableProperties;
                var groupedCol = tableProperties.GroupedColumns.FirstOrDefault(g => g.ColumnName == this.visibleColumn.MappingName);
                if (groupedCol != null)
                {
                    this.IsChecked = !this.IsChecked;
                }
                else
                {
                    this.IsChecked = false;
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (this.IsChecked == true)
            {
                this.GroupColumn();
            }
            else
            {
                this.UngroupColumn();
            }
        }

        private void GroupColumn()
        {
            var tableProperties = this.visibleColumn.TableModel.TableProperties;
            // add the grouping
            var groupedColumn = tableProperties.GroupedColumns.FirstOrDefault(o => o.ColumnName == this.visibleColumn.MappingName);
            if (groupedColumn == null)
            {
                this.visibleColumn.TableModel.View.BeginInit();
                var col = new GridDataGroupColumn() { ColumnName = visibleColumn.MappingName };
                tableProperties.GroupedColumns.Add(col);
                var sortColumn = tableProperties.GetSortColumnForGroup(col);
                if (sortColumn == null)
                {
                    var sortCol = new GridDataSortColumn() { ColumnName = visibleColumn.MappingName };
                    tableProperties.SortColumns.Add(sortCol);
                }
                else
                {
                    tableProperties.SortColumns.Remove(sortColumn);
                    tableProperties.SortColumns.Add(sortColumn);
                }
                this.visibleColumn.TableModel.View.EndInit();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Group Column {0} already found", groupedColumn.ColumnName));
            }
        }

        private void UngroupColumn()
        {
            var tableProperties = this.visibleColumn.TableModel.TableProperties;
            var groupedColumn = tableProperties.GroupedColumns.FirstOrDefault(o => o.ColumnName == this.visibleColumn.MappingName);
            if (groupedColumn != null)
            {
                this.visibleColumn.TableModel.View.BeginInit();
                tableProperties.GroupedColumns.Remove(groupedColumn);
                var sortCol = tableProperties.GetSortColumnForGroup(groupedColumn);
                tableProperties.SortColumns.Remove(sortCol);
                this.visibleColumn.TableModel.View.EndInit();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Group Column {0} not found", groupedColumn.ColumnName));
            }
        }

        public static readonly DependencyProperty GroupingIndicatorInnerBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorInnerBrush",
            typeof(Brush),
            typeof(GridDataGroupingIndicator),
            new PropertyMetadata(new SolidColorBrush(SystemColors.ControlLightColor)));

        public Brush GroupingIndicatorInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataGroupingIndicator.GroupingIndicatorInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataGroupingIndicator.GroupingIndicatorInnerBrushProperty, value);
            }
        }

        public static readonly DependencyProperty GroupingIndicatorOuterBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorOuterBrush",
            typeof(Brush),
            typeof(GridDataGroupingIndicator),
            new PropertyMetadata(new SolidColorBrush(SystemColors.ControlDarkColor)));

        public Brush GroupingIndicatorOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataGroupingIndicator.GroupingIndicatorOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataGroupingIndicator.GroupingIndicatorOuterBrushProperty, value);
            }
        }

        public static readonly DependencyProperty GroupingIndicatorHoverInnerBrushProperty = DependencyProperty.Register(
            "GroupingIndicatorHoverInnerBrush",
            typeof(Brush),
            typeof(GridDataGroupingIndicator),
            new PropertyMetadata(null));

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataGroupingIndicator.GroupingIndicatorHoverInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataGroupingIndicator.GroupingIndicatorHoverInnerBrushProperty, value);
            }
        }
    }

    public class GridDataUpVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;
            if (result)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class GridDataDownVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;
            if (!result)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion


}