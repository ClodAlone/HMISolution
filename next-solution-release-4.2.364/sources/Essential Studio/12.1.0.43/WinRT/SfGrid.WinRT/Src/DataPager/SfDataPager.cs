#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Specialized;
using Syncfusion.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.UI.Xaml.Grid;
#if WinRT
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#else

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;

#endif
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    [TemplatePart(Name = "PART_NumericButtonPanel", Type = typeof(NumericButtonPanel))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollableContentViewer))]
    [TemplatePart(Name = "PART_FirstPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PreviousPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LastPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextPageButton", Type = typeof(Button))]
    [StyleTypedProperty(Property = "NumericButtonStyle", StyleTargetType = typeof(NumericButton))]
    public class SfDataPager : Control, IDisposable
    {
        #region Private Members

        private Button firstPageButton;
        private Button lastPageButton;
        private Button previousPageButton;
        private Button nextPageButton;

        private bool isPageCountNotSet;
        private bool pageIndexChangedInternally;
        private bool isLoaded;
        private bool isDelayApplyVisualState;
        private bool pageIndexChangedBeforeLoad;
        private bool isElipsisElementClicked;
        private bool enableGridPaging = true;
        private bool isPageSizeDefinedBeforeLoad;
        private bool isPageCountSetInternal = false;

        #endregion

        #region Internal Members

        internal ItemGenerator ItemGenerator;
        internal NumericButtonPanel ItemsPanel;
        internal ScrollableContentViewer ScrollViewer;
        internal PageNavigationController NavigationController;

        internal bool InManipulation;

        #endregion

        #region Public Members

        public bool EnableGridPaging
        {
            get { return enableGridPaging; }
            set { enableGridPaging = value; }
        }

        #endregion

        #region Dependency Registration

        public IEnumerable Source
        {
            get { return (IEnumerable) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof (IEnumerable), typeof (SfDataPager),
                                        new PropertyMetadata(null, OnSourcePropertyChanged));

        public PagedCollectionView PagedSource
        {
            get { return (PagedCollectionView) GetValue(PagedSourceProperty); }
            set { SetValue(PagedSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagedSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagedSourceProperty =
            DependencyProperty.Register("PagedSource", typeof (PagedCollectionView), typeof (SfDataPager),
                                        new PropertyMetadata(null));

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set 
            {
                if (UseOnDemandPaging || isPageCountSetInternal)
                    SetValue(PageCountProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(SfDataPager), new PropertyMetadata(0,OnPageCountChanged));

        public int PageSize
        {
            get { return (int) GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(SfDataPager), new PropertyMetadata(0, OnPageSizeChanged));

        public int NumericButtonCount
        {
            get { return (int) GetValue(NumericButtonCountProperty); }
            set { SetValue(NumericButtonCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumericButtonCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register("NumericButtonCount", typeof (int), typeof (SfDataPager),
                                        new PropertyMetadata(5, OnNumericButtonCountChanged));

        public int PageIndex
        {
            get { return (int) GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof (int), typeof (SfDataPager),
                                        new PropertyMetadata(0, OnPageIndexChanged));

        public Style NumericButtonStyle
        {
            get { return (Style) GetValue(NumericButtonStyleProperty); }
            set { SetValue(NumericButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumericButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumericButtonStyleProperty =
            DependencyProperty.Register("NumericButtonStyle", typeof (Style), typeof (SfDataPager),
                                        new PropertyMetadata(null));

        public PageDisplayMode DisplayMode
        {
            get { return (PageDisplayMode) GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof (PageDisplayMode), typeof (SfDataPager),
                                        new PropertyMetadata(PageDisplayMode.FirstLastPreviousNextNumeric,
                                                             OnDisplayModeChanged));

        public AutoEllipsisMode AutoEllipsisMode
        {
            get { return (AutoEllipsisMode) GetValue(AutoElipsisModeProperty); }
            set { SetValue(AutoElipsisModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoElipsisMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoElipsisModeProperty =
            DependencyProperty.Register("AutoEllipsisMode", typeof(AutoEllipsisMode), typeof(SfDataPager),
                                        new PropertyMetadata(AutoEllipsisMode.None, OnAutoEllipsisModeChanged));

        public bool UseOnDemandPaging
        {
            get { return (bool) GetValue(UseOnDemandPagingProperty); }
            set { SetValue(UseOnDemandPagingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseOnDemandPaging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseOnDemandPagingProperty =
            DependencyProperty.Register("UseOnDemandPaging", typeof (bool), typeof (SfDataPager),
                                        new PropertyMetadata(false));
        
        public Brush AccentBackground
        {
            get { return (Brush)GetValue(AccentBackgroundProperty); }
            set { SetValue(AccentBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThemeBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentBackgroundProperty =
            DependencyProperty.Register("AccentBackground", typeof(Brush), typeof(SfDataPager), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray), OnAccentThemeBrushChanged));

        public string AutoEllipsisText
        {
            get { return (string)GetValue(AutoEllipsisTextProperty); }
            set { SetValue(AutoEllipsisTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoEllipsisText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoEllipsisTextProperty =
            DependencyProperty.Register("AutoEllipsisText", typeof(string), typeof(SfDataPager), new PropertyMetadata("...",OnAutoEllipsisTextChanged));

        public Brush AccentForeground
        {
            get { return (Brush)GetValue(AccentForegroundProperty); }
            set { SetValue(AccentForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccentForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentForegroundProperty =
            DependencyProperty.Register("AccentForeground", typeof(Brush), typeof(SfDataPager), new PropertyMetadata(new SolidColorBrush(Colors.White), OnAccentForegroundBrushChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SfDataPager), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
        #endregion

        #region Public Events

        public event PageIndexChangedEventhandler PageIndexChanged;
        public event PageIndexChangingEventhandler PageIndexChanging;
        public event OnDemandLoadingEventHandler OnDemandLoading;

        #endregion

        #region Ctor

        public SfDataPager()
        {
            this.DefaultStyleKey = typeof (SfDataPager);
            ItemGenerator = new ItemGenerator(this);
            NavigationController = new PageNavigationController(this);
        }

        #endregion

        #region Dependency Callback Methods

        private static void OnPageIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (!dataPager.pageIndexChangedInternally && dataPager.isLoaded)
            {
                dataPager.NavigationController.HideCurrentPage((int) args.OldValue);
                    dataPager.MoveToPage(Convert.ToInt32(args.OldValue),Convert.ToInt32(args.NewValue));
            }

            if (!dataPager.isLoaded && !dataPager.pageIndexChangedInternally)
            {
                dataPager.pageIndexChangedBeforeLoad = true;
            }
        }

        private static void OnDisplayModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            dataPager.SetDisplayMode((PageDisplayMode) args.NewValue);
        }

        private static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;

            if (dataPager.PagedSource != null)
            {
                dataPager.UnWireEvents();
                var count = dataPager.Source != null ? dataPager.Source.Cast<object>().Count() : 0;

                if (args.NewValue == null || count == 0)
                {
                    dataPager.ItemGenerator.Items.Clear();
                    dataPager.ClearValue(SfDataPager.PagedSourceProperty);
                    dataPager.ClearValue(SfDataPager.PageIndexProperty);
                    dataPager.ClearValue(SfDataPager.NumericButtonCountProperty);
                    dataPager.ClearValue(SfDataPager.PageCountProperty);
                    dataPager.ClearValue(SfDataPager.PageSizeProperty);
                }
            }

            dataPager.InitiatePageSource((IEnumerable) args.NewValue);
            
            if (dataPager.PageSize > 0)
            {
                dataPager.InitializePageCount((IEnumerable) args.NewValue);
            }
            else
            {
                dataPager.isPageCountNotSet = true;
            }            
            
            if(dataPager.PagedSource!=null)
            dataPager.WireEvents();
            if (dataPager.ItemsPanel != null && !dataPager.pageIndexChangedBeforeLoad)
                dataPager.RefreshView();

            if (dataPager.pageIndexChangedBeforeLoad && dataPager.ItemsPanel!=null)
            {
                dataPager.MoveToPage(dataPager.PageIndex);
                dataPager.pageIndexChangedBeforeLoad = false;
            }
            dataPager.MoveToFirstPage();
            dataPager.SetVisualState();
        }

        private static void OnAccentThemeBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager.ItemGenerator != null)
            {
                dataPager.ItemGenerator.HighlightThemeBrush = (Brush) args.NewValue;
            }
        }

        private static void OnAccentForegroundBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager.ItemGenerator != null)
            {
                dataPager.ItemGenerator.HighlightForegroundBrush = (Brush)args.NewValue;
            }
        }

        private static void OnPageSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager.ItemGenerator.Items.Count() != 0)
            {
                if (dataPager.isLoaded && dataPager.PagedSource != null)
                {
                    dataPager.isPageCountSetInternal = true;
                    dataPager.PagedSource.PageSize = (int)args.NewValue;
                    dataPager.PagedSource.ResetCache();                    
                    if (!dataPager.UseOnDemandPaging)
                    {
                        dataPager.PageCount = dataPager.PagedSource.PageCount;
                        dataPager.RefreshView();
                    }
                    else
                    {
                        dataPager.PagedSource.MaxItemsCount = (int)args.NewValue * dataPager.PageCount;
                    }
                    dataPager.MoveToFirstPage();
                    if (dataPager.PageSize == 0)
                    {
                        dataPager.PageCount = 1;
                        (dataPager.ItemGenerator.Items.FirstOrDefault().Element as NumericButton).IsCurrentPage = true;
                        dataPager.SetVisualState();
                    }
                    else
                    {
                        dataPager.PageCount = dataPager.PagedSource.PageCount;
                        dataPager.RefreshView();                        
                    }                   
                    dataPager.isPageCountSetInternal = false;
                }
                else
                    dataPager.isPageSizeDefinedBeforeLoad = true;
            }
        }

        private static void OnNumericButtonCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager.ItemGenerator.Items.Count() != 0)
            {
                if (dataPager.isLoaded)
                {
                    dataPager.MoveToFirstPage();
                    if (dataPager.ItemsPanel != null)
                        dataPager.RefreshView();
                }
            }
        }

        private static void OnAutoEllipsisModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager != null && dataPager.ItemsPanel != null)
                dataPager.RefreshView();
        }
        private static void OnAutoEllipsisTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataPager = d as SfDataPager;              
            if(dataPager.ItemGenerator!=null && dataPager.ItemGenerator.Items!=null)
            { 
            var ellipsisElement=dataPager.ItemGenerator.Items.Where(item => item.IsElipsisElement);                
            foreach(var element in ellipsisElement)
            (element.Element as NumericButton).Content = e.NewValue.ToString();
            }
        } 
        private static void OnPageCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataPager = obj as SfDataPager;
            if (dataPager.ItemGenerator.Items.Count() != 0)
            {
                if (dataPager != null && dataPager.ItemsPanel != null && dataPager.UseOnDemandPaging && !dataPager.isPageCountSetInternal)
                {
                    dataPager.PagedSource.MaxItemsCount = (int)args.NewValue * dataPager.PageSize;
                    dataPager.SetVisualState();
                    if ((int)args.NewValue < (int)args.OldValue)
                    {
                        dataPager.MoveToPage(((int)args.NewValue - 1));
                    }
                    dataPager.RefreshView();
                }
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sfDataPager = (SfDataPager) d;
            sfDataPager.SetOrientationMode();
            //Since we need to Rearrange the items, to transform the individual items and Auto-Elipse Item. We are calling InvalidateArrange here.
            if (sfDataPager.ItemsPanel == null)
            {
                sfDataPager.isDelayApplyVisualState = true;
                return;
            }
            sfDataPager.ItemsPanel.InvalidateMeasure();
        }

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            isLoaded = true;

            ItemsPanel = this.GetTemplateChild("PART_NumericButtonPanel") as NumericButtonPanel;
            ScrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollableContentViewer;
            firstPageButton = this.GetTemplateChild("PART_FirstPageButton") as Button;
            lastPageButton = this.GetTemplateChild("PART_LastPageButton") as Button;
            previousPageButton = this.GetTemplateChild("PART_PreviousPageButton") as Button;
            nextPageButton = this.GetTemplateChild("PART_NextPageButton") as Button;
            if (ItemsPanel != null)
            {
                ItemsPanel.DataPager = this;
                if (isDelayApplyVisualState)
                    SetOrientationMode();
                isDelayApplyVisualState = false;
            }

            
            this.WireButtonClickEvents();            

            if (this.PagedSource != null && this.isPageSizeDefinedBeforeLoad)
            {
                this.PagedSource.PageSize = this.PageSize;
            }

            if (this.isPageCountNotSet && !UseOnDemandPaging)
            {
                this.InitializePageCount(this.Source);
            }

            if (UseOnDemandPaging)
            {
                if (EnableGridPaging)
                    this.PagedSource = new GridPagedCollectionViewWrapper();
                else
                    this.PagedSource = new PagedCollectionView();

                this.WireEvents();
                this.PagedSource.UseOnDemandPaging = this.UseOnDemandPaging;
                this.PagedSource.PageSize = this.PageSize;
                this.PagedSource.MaxItemsCount = this.PageCount*this.PageSize;
                this.PagedSource.MoveToPage(this.PageIndex);
            }

            if (pageIndexChangedBeforeLoad && this.PagedSource!=null)
            {                
                this.MoveToPage(this.PageIndex);
                pageIndexChangedBeforeLoad = false;
            }

            this.ItemGenerator.HighlightThemeBrush = AccentBackground;
            this.ItemGenerator.HighlightForegroundBrush = AccentForeground;
            this.SetVisualState();
            this.SetDisplayMode(this.DisplayMode);
        }

        private void WireButtonClickEvents()
        {
            if (firstPageButton != null)
                firstPageButton.Click += OnFirstPageButtonClick;
            if (lastPageButton != null)
                lastPageButton.Click += OnLastPageButtonClick;
            if (previousPageButton != null)
                previousPageButton.Click += OnPreviousPageButtonClick;
            if (nextPageButton != null)
                nextPageButton.Click += OnNextPageButtonClick;
        }

        private void UnWireButtonClickEvents()
        {
            if (firstPageButton != null)
                firstPageButton.Click -= OnFirstPageButtonClick;
            if (lastPageButton != null)
                lastPageButton.Click -= OnLastPageButtonClick;
            if (previousPageButton != null)
                previousPageButton.Click -= OnPreviousPageButtonClick;
            if (nextPageButton != null)
                nextPageButton.Click -= OnNextPageButtonClick;
        }
#if WinRT
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            this.HandleKey(e.Key);
        }
#else
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.HandleKey(e.Key);
        }
#endif

#if WinRT
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
#else
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
#endif
        {
            base.OnManipulationStarted(e);
            this.InManipulation = true;
            e.Handled = true;
        }

#if WinRT
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
#else
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
#endif
        {
            base.OnManipulationCompleted(e);
            this.InManipulation = false;
            e.Handled = true;
        }

#if WinRT
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            VisualStateManager.GoToState(this, "PointerEnterd", true);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, "PointerExited", true);
        }
#else

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, "PointerEnterd", true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "PointerExited", true);
        }

#endif

        #endregion

        #region Private Methods

        private void SetOrientationMode()
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    {
                        VisualStateManager.GoToState(this, "Vertical", true);
                        if (ScrollViewer != null)
                        {
                            ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        }
#if !WinRT
                        if (this.ItemsPanel != null)
                        {
                            this.ItemsPanel.DefaultItemSize.Height = 35;
                            this.ItemsPanel.DefaultItemSize.Width = 40;
                        }
#endif
                        break;
                    }
                case Orientation.Horizontal:
                    {
                        VisualStateManager.GoToState(this, "Horizontal", true);
                        if (ScrollViewer != null)
                        {
                            ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        }
#if !WinRT
                        if (this.ItemsPanel != null)
                        {
                            this.ItemsPanel.DefaultItemSize.Height = 40;
                            this.ItemsPanel.DefaultItemSize.Width = 35;
                        }
#endif
                        break;
                    }
            }
        }

        private void SetDisplayMode(PageDisplayMode displayMode)
        {
            if(!isLoaded)
                return;

            if (displayMode.HasFlag(PageDisplayMode.First))
            {
                if (firstPageButton != null)
                    this.firstPageButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (firstPageButton != null)
                    this.firstPageButton.Visibility = Visibility.Collapsed;
            }

            if (displayMode.HasFlag(PageDisplayMode.Last))
            {
                if (lastPageButton != null)
                    this.lastPageButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (lastPageButton != null)
                    this.lastPageButton.Visibility = Visibility.Collapsed;
            }

            if (displayMode.HasFlag(PageDisplayMode.Previous))
            {
                if (previousPageButton != null)
                    this.previousPageButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (previousPageButton != null)
                    this.previousPageButton.Visibility = Visibility.Collapsed;
            }

            if (displayMode.HasFlag(PageDisplayMode.Next))
            {
                if (nextPageButton != null)
                    this.nextPageButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (nextPageButton != null)
                    this.nextPageButton.Visibility = Visibility.Collapsed;
            }

            if (displayMode.HasFlag(PageDisplayMode.Numeric))
            {
                if (ScrollViewer != null)
                    this.ScrollViewer.Visibility = Visibility.Visible;
            }
            else
            {
                if (ScrollViewer != null)
                    this.ScrollViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void SetVisualState()
        {
            var count=0;
            if(!UseOnDemandPaging)
            count = this.Source != null ? this.Source.Cast<object>().Count() : 0;

            if ((this.PageSize==0 && this.PageCount==1) ||(this.PageIndex == 0 && this.PageIndex == this.PageCount - 1) 
                || (this.Source == null && this.PageCount == 0)
                || (count==0 && !UseOnDemandPaging) 
                )
                VisualStateManager.GoToState(this, "LeftRightButtonsDisabled", true);
            else if (this.PageIndex == 0)
                VisualStateManager.GoToState(this, "LeftButtonsDisabled", true);
            else if (this.PageIndex == this.PageCount - 1)
				VisualStateManager.GoToState(this, "RightButtonsDisabled", true);
			else
                VisualStateManager.GoToState(this, "Normal", true); 
	    }
#if WinRT
        private void HandleKey(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Right:
                    {
                        if(Orientation == Orientation.Horizontal)
	                        this.MoveToNextPage();
                    }
                    break;
                case VirtualKey.Left:
                    {
                        if(Orientation == Orientation.Horizontal)
	                        this.MoveToPreviousPage();
                    }
                    break;
                case VirtualKey.Up:
                    {
                        if (Orientation == Orientation.Vertical)
                            this.MoveToPreviousPage();
                    }
                    break;
                case VirtualKey.Down:
                    {
                        if (Orientation == Orientation.Vertical)
                            this.MoveToNextPage();
                    }
                    break;
                case VirtualKey.Home:
                    {
                        this.MoveToFirstPage();
                    }
                    break;
                case VirtualKey.End:
                    {
                        this.MoveToLastPage();
                    }
                    break;
            }
        }
#else
        private void HandleKey(Key key)
        {
            switch (key)
            {
                case Key.Right:
                    {
                        if(Orientation == Orientation.Horizontal)
                            this.MoveToNextPage();
                    }
                    break;
                case Key.Left:
                    {
                        if (Orientation == Orientation.Horizontal)
                            this.MoveToPreviousPage();
                    }
                    break;
                case Key.Up:
                    {
                        if (Orientation == Orientation.Vertical)
                            this.MoveToPreviousPage();
                    }
                    break;
                case Key.Down:
                    {
                        if (Orientation == Orientation.Vertical)
                            this.MoveToNextPage();
                    }
                    break;
                case Key.Home:
                    {
                        this.MoveToFirstPage();
                    }
                    break;
                case Key.End:
                    {
                        this.MoveToLastPage();
                    }
                    break;
            }
        }
#endif

        private void InitiatePageSource(IEnumerable source)
        {
            if (source != null)
            {
                if (EnableGridPaging)
                    this.PagedSource = new GridPagedCollectionViewWrapper(source);
                else
                    this.PagedSource = new PagedCollectionView(source);
            }
        }

        private void InitializePageCount(IEnumerable source)
        {
            this.isPageCountSetInternal=true;
            if (source != null && this.PageSize > 0)
            {
                this.PageCount = Math.Max(1,
                                          (int) Math.Ceiling((double) this.Source.Cast<object>().Count()/this.PageSize));
                this.PagedSource.PageSize = this.PageSize;
                this.PagedSource.MoveToPage(this.PageIndex);
            }
            else if (!UseOnDemandPaging)
            {
                this.PageCount = 0;
                if (this.PageSize == 0)
                    this.NumericButtonCount = 0;
            }
            this.isPageCountSetInternal=false;
        }

        private void WireEvents()
        {
            this.PagedSource.PropertyChanged += OnPagedSourcePropertyChanged;

            if (UseOnDemandPaging)
                this.PagedSource.OnDemandItemsLoading += OnDemandItemsLoading;           
        }

        private void UnWireEvents()
        {
            if(this.PagedSource!=null)
               this.PagedSource.PropertyChanged -= OnPagedSourcePropertyChanged;

            if (UseOnDemandPaging && this.PagedSource!=null)
                this.PagedSource.OnDemandItemsLoading -= OnDemandItemsLoading;
        }

        private void RefreshView()
        {
            this.ItemsPanel.internalOffset = true;
            this.ItemsPanel.InvalidateMeasure();
        }

        #endregion

        #region Internal Methods

        internal void MoveToPage(int pageIndex, bool isElipsisElementClicked)
        {
            this.isElipsisElementClicked = isElipsisElementClicked;
            this.MoveToPage(pageIndex);
        }

        #endregion

        #region Public Method's

        public void MoveToFirstPage()
        {
            this.MoveToPage(0);
        }

        public void MoveToLastPage()
        {
            this.MoveToPage((this.PageCount - 1));
        }

        public void MoveToNextPage()
        {
            var nextPageIndex = this.PageIndex + 1;
            if (nextPageIndex >= this.PageCount)
                return;
            this.MoveToPage(nextPageIndex);
        }

        public void MoveToPreviousPage()
        {
            if (this.PageIndex == 0)
                return;
            var prevIndex = this.PageIndex - 1;
            this.MoveToPage(prevIndex);
        }

        public void MoveToPage(int pageIndex)
        {
            MoveToPage(this.PageIndex, pageIndex);
        }

        private void MoveToPage(int oldPageIndex, int pageIndex)
        {
            var changingArgs = new PageIndexChangingEventArgs()
                {
                    NewPageIndex = pageIndex,
                    OldPageIndex = oldPageIndex
                };

            if (this.RaisePageIndexChangingEvent(changingArgs) || changingArgs.NewPageIndex < 0)
                return;

            if (this.PagedSource != null)
                this.PagedSource.MoveToPage(changingArgs.NewPageIndex);
            this.NavigationController.MoveToPage(changingArgs.NewPageIndex,isElipsisElementClicked);

            pageIndexChangedInternally = true;
            this.PageIndex = changingArgs.NewPageIndex;
            pageIndexChangedInternally = false;

            var changedArgs = new PageIndexChangedEventArgs()
                {
                    NewPageIndex = changingArgs.NewPageIndex,
                    OldPageIndex = changingArgs.OldPageIndex
                };
            this.RaisePageIndexChangedEvent(changedArgs);
            this.SetVisualState();
        }

        public void LoadDynamicItems(int startIndex, IEnumerable items)
        {
            if (this.UseOnDemandPaging)
                this.PagedSource.LoadDynamicItems(startIndex, items);
        }

        #endregion

        #region Event Call Back Methods

        private void OnNextPageButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToNextPage();
        }

        private void OnPreviousPageButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToPreviousPage();
        }

        private void OnLastPageButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToLastPage();
        }

        private void OnFirstPageButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToFirstPage();
        }

        internal bool NeedsFocusToCurrentPage = false;
        private void OnPagedSourcePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(!UseOnDemandPaging)
            NeedsFocusToCurrentPage = false;
            if (e.PropertyName == "ItemsCount")
            {
                if (this.PageCount != this.PagedSource.PageCount)
                {
                    this.isPageCountSetInternal=true;
                    this.PageCount = this.PagedSource.PageCount;
                    this.isPageCountSetInternal=false;
                    if (this.PageIndex >= this.PageCount)
                    {
                        this.MoveToPage((this.PageCount - 1));
                    }
                    this.ItemsPanel.internalOffset = true;
                    this.ItemsPanel.InvalidateMeasure();
                    this.SetVisualState();
                }
                else
                {
                    if (this.PagedSource.ItemCount == 0)
                    {
                        this.isPageCountSetInternal = true;        
                        this.PageCount = 0;
                        this.isPageCountSetInternal = false;
                        this.RefreshView();
                    }
                }               
            }

            if (e.PropertyName == "FilterPredicates")
            {               
                this.MoveToFirstPage();              
            }
            if (!UseOnDemandPaging)
            NeedsFocusToCurrentPage = true;
        }       

        private void OnDemandItemsLoading(object sender, OnDemandItemsLoadingEventArgs args)
        {
            if (this.OnDemandLoading != null)
            {
                var eventArgs = new OnDemandLoadingEventArgs() {PageSize = args.PageSize, StartIndex = args.StartIndex};
                this.OnDemandLoading(this, eventArgs);
            }
        }

        private bool RaisePageIndexChangingEvent(PageIndexChangingEventArgs args)
        {
            if (this.PageIndexChanging != null)
            {
                this.PageIndexChanging(this, args);
            }
            return args.Cancel;
        }

        private void RaisePageIndexChangedEvent(PageIndexChangedEventArgs args)
        {
            if (this.PageIndexChanged != null)
            {
                this.PageIndexChanged(this, args);
            }
        }

        #endregion

        public void Dispose()
        {
            this.UnWireEvents();
            this.UnWireButtonClickEvents();
        }
    }
}