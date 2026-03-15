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
using System.Linq;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Foundation;
#endif
using System.Reflection;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;
#if WPF
using System.Data;
#endif
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Charts
{
    public class SfDateTimeRangeNavigator : SfRangeNavigator
    {
        #region member variable

#if SILVERLIGHT_UNCOMMON
        private bool slUpdate = false;
#endif

#if WPF || SILVERLIGHT_UNCOMMON
        private bool isScrolling=false;
#endif
        private bool isRightSet = false;
        
        private bool isUpdate = false;
        
        private Panel hover;

        private Panel tool;

        private UIElementsRecycler<TextBlock> upperLabelRecycler;

        private UIElementsRecycler<TextBlock> lowerLabelRecycler;

        private UIElementsRecycler<Line> upperGridLineRecycler;

        private UIElementsRecycler<Line> innerGridLineRecycler;

        private UIElementsRecycler<Line> lowerGridLineRecycler;

        private DataTemplate leftTemplate;

        private DataTemplate rightTemplate;

        private Panel upperLabelBar;

        private Panel lowerLabelBar;

        private Panel lowerLineBar;

        private Panel upperLineBar;

        private Rect[] labelElementBounds;

        private ObservableCollection<double> lowerLabelBounds;

        private ObservableCollection<double> upperLabelBounds;

        private ObservableCollection<ChartAxisLabel> upperBarLabels;

        private ObservableCollection<ChartAxisLabel> lowerBarLabels;

        private ObservableCollection<string> navigatorIntervals;
        
        private DateTime maximumDateTimeValue = DateTime.MinValue;
        
        private DateTime minimumDateTimeValue = DateTime.MinValue;
        
        private double totalNoofDays;
        
        private bool isMinMaxSet;

        private ObservableCollection<double> daysvalue = new ObservableCollection<double>();

        private double txtblockwidth;        

        #endregion

        #region ctor
       

        public SfDateTimeRangeNavigator()
        {
            this.DefaultStyleKey = typeof(SfDateTimeRangeNavigator);
            upperBarLabels = new ObservableCollection<ChartAxisLabel>();
            lowerBarLabels = new ObservableCollection<ChartAxisLabel>();
            this.Loaded += OnSfDateTimeRangeNavigatorLoaded;
            if(Intervals!=null)
                Intervals.CollectionChanged += OnIntervalsCollectionChanged;
        }

        
        #endregion       

        #region dp     

        /// <summary>
        /// Gets or sets intervals collection to render labels of SfDateTimeRangeNavigator.
        /// </summary>
        public Intervals Intervals
        {
            get { return (Intervals)GetValue(IntervalsProperty); }
            set { SetValue(IntervalsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Intervals.
        public static readonly DependencyProperty IntervalsProperty =
            DependencyProperty.Register("Intervals", typeof(Intervals), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(new Intervals(), OnIntervalChanged));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator rangeNavigator = (d as SfDateTimeRangeNavigator);
            if (rangeNavigator != null)
            {
                if (e.OldValue != null)
                    (e.OldValue as Intervals).CollectionChanged -= rangeNavigator.OnIntervalsCollectionChanged;
                (e.NewValue as Intervals).CollectionChanged += rangeNavigator.OnIntervalsCollectionChanged;
                if ((rangeNavigator.upperLabelBar != null && rangeNavigator.navigator.TrackSize != 0) || (rangeNavigator.lowerLabelBar != null && rangeNavigator.navigator.TrackSize != 0))
                    rangeNavigator.Update();
            }
        }


        /// <summary>
        /// Gets or Sets the Minimum Starting Range of the SfDateTimeRangeNavigator.
        /// </summary>

        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(object), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(DateTime.MinValue, OnMinimumMaximumChanged));


        /// <summary>
        /// Gets or Sets the Maximum Ending Range of the SfDateTimeRangeNavigator.
        /// </summary>
        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(object), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(DateTime.MinValue, OnMinimumMaximumChanged));

        private static void OnMinimumMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if((d as SfDateTimeRangeNavigator)!=null)
                (d as SfDateTimeRangeNavigator).OnMinimumMaximumChanged();
        }

        private void OnMinimumMaximumChanged()
        {
            if (Convert.ToDateTime(Minimum) != DateTime.MinValue && Convert.ToDateTime(Maximum) != DateTime.MinValue)
            {
                Refresh();
                CalculateRange();
                CalculateTooltipPosition();
                if(ViewRangeStart!=null)
                OnViewRangeStartChanged();
                if(ViewRangeEnd!=null)
                OnViewRangeEndChanged();
            }
        }

#if WPF
        /// <summary>
        /// Gets or sets an object source used to render range.
        /// </summary>
        /// <value>The DataSource value.</value>
        [ClassReference(IsReviewed = false)]
        public object ItemsSource
        {
            get
            {
                return (object)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(object), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null, new PropertyChangedCallback(OnItemSourceChanged)));
#else
        /// <summary>
        /// Gets or sets an IEnumerable source used to render range.
        /// </summary>
        /// <value>The DataSource value.</value>
        [ClassReference(IsReviewed = false)]
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null, new PropertyChangedCallback(OnItemSourceChanged)));
#endif

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             if (e.NewValue == null)
                (d as SfDateTimeRangeNavigator).ClearLabels();
             else
                (d as SfDateTimeRangeNavigator).OnDataSourceChanged(e);
        }

        protected void OnDataSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            Refresh();
            CalculateRange();
            CalculateTooltipPosition();
            if (args.OldValue is INotifyCollectionChanged)
            {
                (args.OldValue as INotifyCollectionChanged).CollectionChanged -= OnSfRangeNavigatorCollectionChanged;
            }

            if (args.NewValue is INotifyCollectionChanged)
            {
                (args.NewValue as INotifyCollectionChanged).CollectionChanged += OnSfRangeNavigatorCollectionChanged;
            }
        }

        /// <summary>
        /// Gets an IEnumerable source for the particular selected region
        /// </summary>
        public object SelectedData
        {
            get { return (object)GetValue(SelectedDataProperty); }
            private set { SetValue(SelectedDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedData.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDataProperty =
            DependencyProperty.Register("SelectedData", typeof(object), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null));

        void OnSfRangeNavigatorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset && (XValues as IList<DateTime>)!=null)
            {
                (XValues as IList<DateTime>).Clear();
                ClearLabels();
            }

            Refresh();
            if (ViewRangeEnd != null && ViewRangeStart != null)
            {
                OnViewRangeStartChanged();
                OnViewRangeEndChanged();
            }
        }
        void Refresh()
        {
            GeneratePoints();
            UpdateLayout();
        }

        internal override void OnViewRangeStartChanged()
        {
            if (!(ViewRangeStart is double) && navigator != null && navigator.TrackSize != 0 && totalNoofDays != 0d)
            {
                 var noofdays = (Convert.ToDateTime(ViewRangeStart) - minimumDateTimeValue).TotalDays;
                 var onedayInterval = navigator.TrackSize / totalNoofDays;
                 var startleft = noofdays * onedayInterval;
                 var startrange = 1 * (startleft / (navigator.TrackSize));
                 navigator.IsValueChangedTrigger = false;
                 navigator.RangeStart = startrange < 0 ? 0 : startrange;
            }
            CalculateSelectedData();
            CalculateTooltipPosition();
        }

        internal override void OnViewRangeEndChanged()
        {
            if (!(ViewRangeEnd is double) && navigator != null && navigator.TrackSize != 0 && totalNoofDays != 0d)
            {
                  var noofdays = (Convert.ToDateTime(ViewRangeEnd) - this.minimumDateTimeValue).TotalDays;
                  var onedayInterval = (navigator.TrackSize / this.totalNoofDays);
                  var endright = noofdays * onedayInterval;
                  var endrange = 1 * (endright / (navigator.TrackSize));
                  navigator.IsValueChangedTrigger = false;
                  navigator.RangeEnd = endrange > 1 ? 1 : endrange;
            }
            
            CalculateSelectedData();
            CalculateTooltipPosition();
        }

        internal override void OnZoomFactorChanged(double newValue)
        {
            if (navigator == null)
            {
                base.OnZoomFactorChanged(newValue);
            }
            else if (navigator != null)
            {
                isUpdate = true;
                navigator.IsValueChangedTrigger = false;
                if ((navigator.RangeEnd != navigator.Maximum && navigator.isFarDragged) || zoomFactor != 1d)
                {
                    navigator.RangeEnd = ZoomPosition + newValue;
                }
                isUpdate = false;
            }
        }

        internal override void OnZoomPositionChanged(double newValue)
        {
            if (navigator != null)
            {
                navigator.IsValueChangedTrigger = false;
                navigator.RangeEnd = ZoomFactor + newValue;
                navigator.IsValueChangedTrigger = false;
                navigator.RangeStart = newValue;
            }
            else if (navigator == null)
            {
                base.OnZoomPositionChanged(newValue);
            }
        }

        /// <summary>
        /// Gets or Sets to show ToolTip.
        /// </summary>
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(false, OnShowToolTipChanged));


        /// <summary>
        /// Gets or sets template for the left side ToolTip.
        /// </summary>
        public DataTemplate LeftToolTipTemplate
        {
            get { return (DataTemplate)GetValue(LeftToolTipTemplateProperty); }
            set { SetValue(LeftToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolTipTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftToolTipTemplateProperty =
            DependencyProperty.Register("LeftToolTipTemplate", typeof(DataTemplate), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null, OnLeftToolTipTemplateChanged));

        private static void OnLeftToolTipTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator navigator = (d as SfDateTimeRangeNavigator);
            if (navigator!=null && navigator.tool != null && navigator.ShowToolTip && navigator.tool.Children.Count > 0)
                (navigator.tool.Children[0] as ContentControl).ContentTemplate = (DataTemplate)e.NewValue;
        }

        private static void OnShowToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).UpdateTooltipVisibility();
        }

        /// <summary>
        /// Gets or sets template for the right side ToolTip.
        /// </summary>
        public DataTemplate RightToolTipTemplate
        {
            get { return (DataTemplate)GetValue(RightToolTipTemplateProperty); }
            set { SetValue(RightToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightToolTipTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightToolTipTemplateProperty =
            DependencyProperty.Register("RightToolTipTemplate", typeof(DataTemplate), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null, OnRightToolTipTemplateChanged));

        private static void OnRightToolTipTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator navigator = (d as SfDateTimeRangeNavigator);
            if (navigator!=null && navigator.tool != null && navigator.ShowToolTip && navigator.tool.Children.Count > 0)
            {
                (navigator.tool.Children[1] as ContentControl).ContentTemplate = (DataTemplate)e.NewValue;
                navigator.CalculateTooltipPosition();
            }
        }

        /// <summary>
        /// Gets or sets label format for ToolTip.
        /// </summary>
        public string ToolTipLabelFormat
        {
            get { return (string)GetValue(ToolTipLabelFormatProperty); }
            set { SetValue(ToolTipLabelFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipLableFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipLabelFormatProperty =
            DependencyProperty.Register("ToolTipLabelFormat", typeof(string), typeof(SfDateTimeRangeNavigator), new PropertyMetadata("dd/MMM/yyyy",OnToolTipFormatChanged));

        private static void OnToolTipFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).CalculateTooltipPosition();
        }

        
        /// <summary>
        /// Gets or Sets the property path of the x data in ItemsSource.
        /// </summary>
        public string XBindingPath
        {
            get { return (string)GetValue(XBindingPathProperty); }
            set { SetValue(XBindingPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XBindingPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XBindingPathProperty =
            DependencyProperty.Register("XBindingPath", typeof(string), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(string.Empty, OnXBindingPathChanged));

        private static void OnXBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SfDateTimeRangeNavigator navigator = d as SfDateTimeRangeNavigator;
            if (navigator.ItemsSource != null)
                navigator.GeneratePoints();
        }

        /// <summary>
        /// Gets or Sets the styles for the lowerlabelbar of SfDateTimeRangeNavigator.
        /// </summary>
        public LabelBarStyle LowerLevelBarStyle
        {
            get { return (LabelBarStyle)GetValue(LowerLevelBarStyleProperty); }
            set { SetValue(LowerLevelBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowerBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowerLevelBarStyleProperty =
            DependencyProperty.Register("LowerLevelBarStyle", typeof(LabelBarStyle), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the styles for the higherlabelbar of SfDateTimeRangeNavigator.
        /// </summary>
        public LabelBarStyle HigherLevelBarStyle
        {
            get { return (LabelBarStyle)GetValue(HigherLevelBarStyleProperty); }
            set { SetValue(HigherLevelBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HigherLevelStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HigherLevelBarStyleProperty =
            DependencyProperty.Register("HigherLevelBarStyle", typeof(LabelBarStyle), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the higher label style.
        /// </summary>
        /// <value>
        /// The higher label style.
        /// </value>
        public Style HigherLabelStyle
        {
            get { return (Style)GetValue(HigherLabelStyleProperty); }
            set { SetValue(HigherLabelStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HigherLabelStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HigherLabelStyleProperty =
            DependencyProperty.Register("HigherLabelStyle", typeof(Style), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lower label style.
        /// </summary>
        /// <value>
        /// The lower label style.
        /// </value>
        public Style LowerLabelStyle
        {
            get { return (Style)GetValue(LowerLabelStyleProperty); }
            set { SetValue(LowerLabelStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowerLabelStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowerLabelStyleProperty =
            DependencyProperty.Register("LowerLabelStyle", typeof(Style), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a value that indicates whether to show grid lines inside the content.
        /// </summary>
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(true,OnShowGridlinesChanged));

        private static void OnShowGridlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).Update();
        }

        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register("RangePadding", typeof(NavigatorRangePadding), typeof(SfDateTimeRangeNavigator), new PropertyMetadata(NavigatorRangePadding.Round));

        public NavigatorRangePadding RangePadding
        {
            get { return (NavigatorRangePadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }
        #endregion

        #region Methods

        private void OnIntervalsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (navigator!=null && (upperLabelBar != null && navigator.TrackSize != 0) || (lowerLabelBar != null && navigator.TrackSize != 0))
            {
                if (e.Action== NotifyCollectionChangedAction.Reset  && (upperLabelRecycler.Count > 0 || lowerLabelRecycler.Count > 0))
                    ClearLabels();
                Update();
            }
        }



        void OnSfDateTimeRangeNavigatorSizeChanged(object sender, SizeChangedEventArgs e)
        {
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (scrollbar != null && navigator != null)
            {
                navigator.ClearValue(ResizableScrollBar.WidthProperty);
                this.navigator.Width = this.ActualWidth * scrollbar.Scale;
                xrange = -((this.ActualWidth * scrollbar.Scale) * scrollbar.RangeStart);
                double _margin = xrange == 0 ? navigator.ResizableThumbSize : xrange;
                this.navigator.Margin = new Thickness(_margin, 0, 0, 0);

                if (navigator.Content != null)
                {
                    double newmargin = xrange >= -navigator.ResizableThumbSize ? xrange : xrange + navigator.ResizableThumbSize;
#if WPF
                    upperLabelBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    upperLabelBar.UpdateLayout();
#endif
                    if (!scrollbar.isFarDragged)
                    {
                        upperLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                        upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        upperLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                        upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                        lowerLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                        lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                    else if (!scrollbar.isNearDragged && xrange != 0)
                    {
                        upperLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                        upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        upperLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                        upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                        lowerLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                        lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                    if (tool != null && navigator != null)
                        tool.Width = navigator.TrackSize * scrollbar.Scale;
#if WPF
                    innerGridlines.UpdateLayout();                  
                    lowerLabelBar.UpdateLayout();
                    lowerLineBar.UpdateLayout();
                    upperLineBar.UpdateLayout();
#endif
                }
            }
#endif
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
        }


        void OnSfDateTimeRangeNavigatorLoaded(object sender, RoutedEventArgs e)
        {
          
#if !SILVERLIGHT_UNCOMMON
            if (navigator != null)
            {
                if (upperLabelBar != null)
                {
                    upperLabelBar.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
                    upperLineBar.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
                }
                if (lowerLabelBar != null)
                {
                    lowerLabelBar.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
                    lowerLineBar.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
                }
                if(hover!=null)
                    hover.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
                if(innerGridlines!=null)
                    innerGridlines.Margin = new Thickness(navigator.ResizableThumbSize, 0, 0, 0);
            }
#endif
            CalculateRange();
           
            if(navigator!=null)
                Update();

            this.SizeChanged += OnSfDateTimeRangeNavigatorSizeChanged;

        }

        private void CalculateRange()
        {
            if (this.ViewRangeStart != null && this.ViewRangeEnd != null && totalNoofDays!=0 && !(this.ViewRangeStart is double) && !(this.ViewRangeEnd is double) && navigator != null && minimumDateTimeValue != DateTime.MinValue)
            {
                var ondayinterval = (navigator.TrackSize / totalNoofDays);
                var startleft = (Convert.ToDateTime(this.ViewRangeStart) - minimumDateTimeValue).TotalDays * ondayinterval;
                var endright = (Convert.ToDateTime(this.ViewRangeEnd) - minimumDateTimeValue).TotalDays * ondayinterval;

                if (Convert.ToDateTime(ViewRangeStart) != minimumDateTimeValue)
                {
                    navigator.IsValueChangedTrigger = false;
                    navigator.RangeStart = 1 * (startleft / (navigator.TrackSize));
                }
                if (Convert.ToDateTime(ViewRangeEnd) != maximumDateTimeValue)
                {
                    navigator.IsValueChangedTrigger = false;
                    navigator.RangeEnd = 1 * (endright / (navigator.TrackSize));
                }
            }
        }
        
        private void UpdateTooltipVisibility()
        {
            if (tool == null) return;
            if (ShowToolTip)
            {
                (tool.Children[0] as UIElement).Visibility = Visibility.Visible;
                (tool.Children[1] as UIElement).Visibility = Visibility.Visible;
                CalculateTooltipPosition();
            }
            else
            {
                (tool.Children[0] as UIElement).Visibility = Visibility.Collapsed;
                (tool.Children[1] as UIElement).Visibility = Visibility.Collapsed;
            }
        }

        private Panel innerGridlines;
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            upperLabelBar = this.GetTemplateChild("PART_UPPERBAR") as Panel;
            upperLineBar = this.GetTemplateChild("PART_UPPERLINE") as Panel;
            lowerLabelBar = this.GetTemplateChild("PART_LOWERBAR") as Panel;
            lowerLineBar = this.GetTemplateChild("PART_LOWERLINE") as Panel;
            hover = this.GetTemplateChild("Part_Hover") as Panel;
            tool = this.GetTemplateChild("Part_Tooltip") as Panel;
            innerGridlines = this.GetTemplateChild("Part_Content_line") as Panel;
            this.leftTemplate = ChartDictionaries.GenericCommonDictionary["leftTooltipTemplate"] as DataTemplate;
            this.rightTemplate = ChartDictionaries.GenericCommonDictionary["rightTooltipTemplate"] as DataTemplate;
            if(upperLabelBar!=null)
                upperLabelRecycler = new UIElementsRecycler<TextBlock>(upperLabelBar);
            if(lowerLabelBar!=null)
                lowerLabelRecycler = new UIElementsRecycler<TextBlock>(lowerLabelBar);
            if(innerGridlines!=null)
                innerGridLineRecycler = new UIElementsRecycler<Line>(innerGridlines);
            if(upperLineBar!=null)
                upperGridLineRecycler = new UIElementsRecycler<Line>(upperLineBar);
            if(lowerLineBar!=null)
                lowerGridLineRecycler = new UIElementsRecycler<Line>(lowerLineBar);

#if SILVERLIGHT_UNCOMMON
            if (upperLabelBar != null)
            {
            upperLabelBar.Background = HigherLevelBarStyle.Background;
            upperLabelBar.Margin = new Thickness(1, 0, 0, 0);
            upperLineBar.Margin = new Thickness(1, 0, 0, 0);
            }
            if (lowerLabelBar != null)
            {
            lowerLabelBar.Background = LowerLevelBarStyle.Background;
            lowerLabelBar.Margin = new Thickness(1, 0, 0, 0);
            lowerLineBar.Margin = new Thickness(1, 0, 0, 0);
            }
            if(hover!=null)
            hover.Margin = new Thickness(1, 0, 0, 0);
            if(navigator!=null && (navigator.Content is Panel))
            (navigator.Content as Panel).Margin = new Thickness(1, 0, 0, 0);
#endif
#if !WINDOWS_PHONE
            if (upperLabelBar != null)
            {
                upperLabelBar.Background = HigherLevelBarStyle.Background;
                upperLabelBar.AddHandler(PointerMovedEvent, new PointerEventHandler(UpperLabelBar_PointerMoved), true);
                upperLabelBar.AddHandler(PointerExitedEvent, new PointerEventHandler(UpperLabelBar_PointerExited), true);
                upperLabelBar.AddHandler(PointerPressedEvent, new PointerEventHandler(UpperLabelBar_PointerPressed), true);
            }
            if (lowerLabelBar != null)
            {
                lowerLabelBar.Background = LowerLevelBarStyle.Background;
                lowerLabelBar.PointerMoved += LowerLabelBar_PointerMoved;
                lowerLabelBar.PointerExited += LowerLabelBar_PointerExited;
                lowerLabelBar.PointerPressed += LowerLabelBar_PointerPressed;
            }
#else
            if (upperLabelBar != null)
            {
            upperLabelBar.Background = HigherLevelBarStyle.Background;
            upperLabelBar.MouseMove+=UpperLabelBar_MouseMove;
            upperLabelBar.MouseLeave+=UpperLabelBar_MouseLeave;
            upperLabelBar.MouseLeftButtonDown+=UpperLabelBar_MouseLeftButtonDown;
            }
            if (lowerLabelBar != null)
            {
            lowerLabelBar.Background = LowerLevelBarStyle.Background;
            lowerLabelBar.MouseMove+=LowerLabelBar_MouseMove;
            lowerLabelBar.MouseLeave+=LowerLabelBar_MouseLeave;
            lowerLabelBar.MouseLeftButtonDown+=LowerLabelBar_MouseLeftButtonDown;
            }
#endif
            GeneratePoints();
            UpdateTooltipVisibility();

        }

        void AddDaysValues(DateTime currentDate)
        {
            if (currentDate <= maximumDateTimeValue)
                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
            else
                daysvalue.Add((maximumDateTimeValue - minimumDateTimeValue).TotalDays);
        }

        internal void GeneratePoints()
        {
            bool isPointsGenerated = false;
            if (this.ItemsSource != null && !string.IsNullOrEmpty(this.XBindingPath))
            {
#if WPF
                if (ItemsSource is DataTable)
                    GenerateDataTablePoints();
                else
#endif
                    GeneratePropertyPoints();
                isPointsGenerated = true;
            }

            if (XValues != null && XValues is IList<DateTime> && (XValues as IList<DateTime>).Count != 0 && isPointsGenerated)
            {
                if (RangePadding == NavigatorRangePadding.Round)
                {
                    maximumDateTimeValue =
                        (XValues as IList<DateTime>).Max().AddHours(new TimeSpan(0, 23, 59, 59).TotalHours -
                                                                    (XValues as IList<DateTime>).Max().Hour);

                    minimumDateTimeValue =
                        (XValues as IList<DateTime>).Min().AddHours(new TimeSpan(0, 0, 00, 01).TotalHours -
                                                                    (XValues as IList<DateTime>).Min().Hour);
                }
                else
                {
                    maximumDateTimeValue = (XValues as IList<DateTime>).Max();
                    minimumDateTimeValue = (XValues as IList<DateTime>).Min();
                }
            }
            else if(Convert.ToString(Minimum) != "0" && Convert.ToString(Maximum) != "1")
            {
                 maximumDateTimeValue = Convert.ToDateTime(Maximum.ToString());
                 minimumDateTimeValue = Convert.ToDateTime(Minimum.ToString());
            }
            if (maximumDateTimeValue != DateTime.MinValue && minimumDateTimeValue != DateTime.MinValue)
            {
                isMinMaxSet = true;
                totalNoofDays = (maximumDateTimeValue.ToOADate() - minimumDateTimeValue.ToOADate()) == 0 ? 1 : (maximumDateTimeValue.ToOADate() - minimumDateTimeValue.ToOADate());
                if ((upperLabelRecycler != null && upperLabelRecycler.Count>0) || (lowerLabelRecycler!=null && lowerLabelRecycler.Count>0))
                    CalculateSelectedData();
            }
            else
            {
                isMinMaxSet = false;
                totalNoofDays = 0;
            }

            if (navigator!=null && (upperLabelBar != null && navigator.TrackSize != 0) || (lowerLabelBar != null && navigator.TrackSize != 0))
                Update();
        }

        private void GeneratePropertyPoints()
        {
            DataEnd = 0;
#if WPF
                IEnumerator enumerator = (this.ItemsSource as IEnumerable).GetEnumerator();
#else
                IEnumerator enumerator = this.ItemsSource.GetEnumerator();
#endif
                if (enumerator.MoveNext())
                {
                    var xPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(this.XBindingPath);
                    IPropertyAccessor xPropertyAccessor = null;
                    if (xPropertyInfo != null)
                    {
                        xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                        Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                        this.XValues = new List<DateTime>();
                        IList<DateTime> xValue = XValues as List<DateTime>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            xValue.Add((DateTime)xVal); DataEnd++;
                        } while (enumerator.MoveNext());
                    }
                }
        }
      
        void CalculateTooltipPosition()
        {
             if (tool != null && ShowToolTip && isMinMaxSet && navigator != null && navigator.TrackSize != 0)
             {
                tool.Visibility = Visibility.Visible;
                var onedayInterval = this.navigator.TrackSize / this.totalNoofDays;
                var leftTooltip = tool.Children[0] as ContentControl;
                var rightTooltip = tool.Children[1] as ContentControl;
                var noofdays = (this.navigator.RangeEnd * this.navigator.TrackSize) / onedayInterval;
                var chkdays = (this.navigator.RangeStart * this.navigator.TrackSize) / onedayInterval;
                leftTooltip.ContentTemplate = this.LeftToolTipTemplate == null ? this.leftTemplate : this.LeftToolTipTemplate;
                rightTooltip.ContentTemplate = this.RightToolTipTemplate == null ? this.rightTemplate : this.RightToolTipTemplate;
                leftTooltip.Content = this.minimumDateTimeValue.AddDays(chkdays).ToString(this.ToolTipLabelFormat, CultureInfo.CurrentCulture);
                rightTooltip.Content = this.minimumDateTimeValue.AddDays(noofdays).ToString(this.ToolTipLabelFormat, CultureInfo.CurrentCulture);
                leftTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rightTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF || SILVERLIGHT_UNCOMMON
                if (!isScrolling)
                {
                    leftTooltip.UpdateLayout();
                    rightTooltip.UpdateLayout();
                }
#endif
                var leftwidth = leftTooltip.DesiredSize.Width;
                var rightwidth = rightTooltip.DesiredSize.Width;
                Canvas.SetLeft(tool.Children[0], (this.navigator.RangeStart * this.navigator.TrackSize) - leftwidth);
                Canvas.SetLeft(tool.Children[1], (this.navigator.RangeEnd * this.navigator.TrackSize));
                isRightSet = false;
                leftTooltip.Margin = new Thickness(0, 0, 0, 0);
                rightTooltip.Margin = new Thickness(0, 0, 0, 0);
                if (Canvas.GetLeft(leftTooltip) <= 0)
                {
                    leftTooltip.ContentTemplate =this.LeftToolTipTemplate == null ? this.rightTemplate: this.LeftToolTipTemplate;
                    Canvas.SetLeft(tool.Children[0], (this.navigator.RangeStart * this.navigator.ActualWidth));
                    isRightSet = true;
                }
                if (Canvas.GetLeft(rightTooltip) + rightwidth >= navigator.ActualWidth)
                {
                    rightTooltip.ContentTemplate = this.RightToolTipTemplate==null? this.leftTemplate: this.RightToolTipTemplate;
                    Canvas.SetLeft(tool.Children[1], (this.navigator.RangeEnd * (this.navigator.ActualWidth)) - rightwidth);
                }
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
                if (Canvas.GetLeft(leftTooltip) / navigator.TrackSize <= scrollbar.RangeStart)
                {
                    leftTooltip.ContentTemplate = this.LeftToolTipTemplate== null ? this.rightTemplate: this.LeftToolTipTemplate;
                    if (navigator.RangeStart <= scrollbar.RangeStart)
                        Canvas.SetLeft(tool.Children[0], (this.scrollbar.RangeStart * this.navigator.TrackSize));
                    else
                        Canvas.SetLeft(tool.Children[0], ((Canvas.GetLeft(leftTooltip) / navigator.TrackSize) * this.navigator.TrackSize) + leftwidth);
                    if (navigator.RangeEnd <= scrollbar.RangeStart)
                        Canvas.SetLeft(tool.Children[1], (this.scrollbar.RangeStart * this.navigator.TrackSize));
                    isRightSet = true;
                }
                if ((Canvas.GetLeft(rightTooltip) + rightwidth) / navigator.ActualWidth >= scrollbar.RangeEnd)
                {
                    rightTooltip.ContentTemplate = this.RightToolTipTemplate== null? this.leftTemplate: this.RightToolTipTemplate;
                    if (Math.Round(navigator.RangeEnd) >= scrollbar.RangeEnd)
                        Canvas.SetLeft(tool.Children[1], (this.scrollbar.RangeEnd * this.navigator.TrackSize+ navigator.ResizableThumbSize) - rightwidth);
                    else
                        Canvas.SetLeft(tool.Children[1], ((Canvas.GetLeft(rightTooltip) / navigator.ActualWidth) * this.navigator.ActualWidth) - rightwidth);
                    if (navigator.RangeStart >= scrollbar.RangeEnd)
                        Canvas.SetLeft(tool.Children[0], (this.scrollbar.RangeEnd * this.navigator.TrackSize) - leftwidth);
                }
#endif

                if (Canvas.GetLeft(leftTooltip) + leftwidth >= Canvas.GetLeft(rightTooltip))
                {
                    if (!isRightSet)
                        leftTooltip.Margin = new Thickness(0, 30, 0, 0);
                    else
                        rightTooltip.Margin = new Thickness(0, 30, 0, 0);
                }
                else
                {
                    if (!isRightSet)
                        leftTooltip.Margin = new Thickness(0, 0, 0, 0);
                    else
                        rightTooltip.Margin = new Thickness(0, 0, 0, 0);
                }
            }
             if (!isMinMaxSet && tool != null)
                 tool.Visibility = Visibility.Collapsed;
#if WPF || SILVERLIGHT_UNCOMMON
             isScrolling = false;
#endif
        }
        protected override void OnValueChanged()
        {
            CalculateTooltipPosition();
            ChangeViewRange();
            base.OnValueChanged();
        }
        private void ChangeViewRange()
        {
            if(navigator != null && this.navigator.TrackSize > 0 && totalNoofDays != 0d)
            {
                var onedayInterval = this.navigator.TrackSize / this.totalNoofDays;
                var noofdays = (this.navigator.RangeStart * this.navigator.TrackSize) / onedayInterval;
                this.ViewRangeStart = this.minimumDateTimeValue.AddDays(noofdays);
                noofdays = (this.navigator.RangeEnd * this.navigator.TrackSize) / onedayInterval;
                this.ViewRangeEnd = this.minimumDateTimeValue.AddDays(noofdays);
            }
        }

        internal override void CalculateSelectedData()
        {
            m_selected = new ObservableCollection<object>();
            if (navigator!=null)
            {
                navigator.IsValueChangedTrigger = false;
                var start = (DataStart + navigator.RangeStart*(DataEnd - DataStart));
                int datastart = start % ((int)start == 0 ? 1 : (int)start) > 0.98 ? (int)Math.Round(start) : (int)start;
                var end = (DataStart + (navigator.RangeEnd - navigator.RangeStart) * (DataEnd - DataStart));
                int  dataend =(int) Math.Round(start + end);
                int count = 0;
                if (this.ItemsSource != null)
                {
#if WPF
                    IEnumerable itemSource= this.ItemsSource is DataTable ?(ItemsSource as DataTable).Rows : (ItemsSource as IEnumerable);
#else
                    IEnumerable itemSource= this.ItemsSource;
#endif
                    foreach (var item in itemSource)
                    {
                        if (count >= datastart && count < dataend)
                            m_selected.Add(item);
                        count++;
                    }
                    this.SelectedData = m_selected;
                }
                navigator.IsValueChangedTrigger = false;
                this.ZoomFactor = navigator.RangeEnd - navigator.RangeStart;
                navigator.IsValueChangedTrigger = false;
                this.ZoomPosition = navigator.RangeStart;
                isUpdate = false;
            }
#if SILVERLIGHT_UNCOMMON
            if (ItemsSource != null && navigator != null && !slUpdate)
            {
                if ((upperLabelBar != null)||(dockPosition == "Lower" && lowerLabelBar != null))
                {
                    Update();
                    slUpdate = true;
                }
            }
#endif
            int labelcount = -1;
            if (upperLabelBar != null && upperLabelBounds != null)
            {
                for (int i = 0; i < upperLabelBounds.Count; i++)
                {

                    if (navigator.RangeStart * navigator.TrackSize < upperLabelBounds[i] && navigator.RangeEnd * navigator.TrackSize >= upperLabelBounds[i])
                    {
                        (upperLabelBar.Children[i] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                        labelcount = i;
                    }
                    else if (i == 0 && (navigator.RangeStart * navigator.TrackSize >= 0 && navigator.RangeStart * navigator.TrackSize <= upperLabelBounds[i]) && (navigator.RangeEnd * navigator.TrackSize >= 0 && navigator.RangeEnd * navigator.TrackSize <= upperLabelBounds[i]))
                    {
                        (upperLabelBar.Children[i] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                    }
                    else if (i != 0 && (navigator.RangeStart * navigator.TrackSize >= upperLabelBounds[i - 1] && navigator.RangeStart * navigator.TrackSize<=upperLabelBounds[i]) &&(navigator.RangeEnd * navigator.TrackSize >= upperLabelBounds[i - 1] && navigator.RangeEnd * navigator.TrackSize<=upperLabelBounds[i]))
                    {
                        (upperLabelBar.Children[i] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                    }
                    else
                    {
                        upperLabelBar.Children[i].ClearValue(TextBlock.ForegroundProperty);
                        (upperLabelBar.Children[i] as TextBlock).Style = HigherLabelStyle;
                    }                        
                    if (labelcount != -1 && navigator.RangeEnd * navigator.TrackSize > upperLabelBounds[labelcount] && labelcount + 1 != upperLabelBar.Children.Count)
                    {
                        (upperLabelBar.Children[labelcount + 1] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                    }

                }
            }
            labelcount = -1;
            if (lowerLabelBar != null && lowerLabelBounds != null)
            {
                for (int i = 0; i < lowerLabelBounds.Count; i++)
                {
                    if (navigator.RangeStart * navigator.TrackSize < lowerLabelBounds[i] && navigator.RangeEnd * navigator.TrackSize >= lowerLabelBounds[i])
                    {
                        (lowerLabelBar.Children[i] as TextBlock).Foreground = LowerLevelBarStyle.SelectedLabelBrush;
                        labelcount = i;
                    }
                    else if (i == 0 && (navigator.RangeStart * navigator.TrackSize >= 0 && navigator.RangeStart * navigator.TrackSize <= lowerLabelBounds[i]) && (navigator.RangeEnd * navigator.TrackSize >= 0 && navigator.RangeEnd * navigator.TrackSize <= lowerLabelBounds[i]))
                    {
                        (lowerLabelBar.Children[i] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                    }
                    else if (i != 0 && (navigator.RangeStart * navigator.TrackSize >= lowerLabelBounds[i - 1] && navigator.RangeStart * navigator.TrackSize <= lowerLabelBounds[i]) && (navigator.RangeEnd * navigator.TrackSize >= lowerLabelBounds[i - 1] && navigator.RangeEnd * navigator.TrackSize <= lowerLabelBounds[i]))
                    {
                        (lowerLabelBar.Children[i] as TextBlock).Foreground = HigherLevelBarStyle.SelectedLabelBrush;
                    }
                    else
                    {
                        lowerLabelBar.Children[i].ClearValue(TextBlock.ForegroundProperty);
                        (lowerLabelBar.Children[i] as TextBlock).Style = LowerLabelStyle;
                    }

                    if (labelcount != -1 && navigator.RangeEnd * navigator.TrackSize > lowerLabelBounds[labelcount] && labelcount + 1 != lowerLabelBar.Children.Count)
                    {
                        (lowerLabelBar.Children[labelcount + 1] as TextBlock).Foreground = LowerLevelBarStyle.SelectedLabelBrush;
                    }
                }
            }

        }


#if WINDOWS_PHONE
        void LowerLabelBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#else
        void LowerLabelBar_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE
            Point pt = e.GetPosition(lowerLabelBar);
#else
            Windows.UI.Input.PointerPoint Pointer = e.GetCurrentPoint(lowerLabelBar);
            Point pt = new Point(Pointer.Position.X, Pointer.Position.Y);
#endif
            if (lowerLabelBounds != null && lowerLabelBounds.Count > 1)
            for (int i = 0; i < lowerLabelBounds.Count; i++)
            {
                if (pt.X > lowerLabelBounds[i] && i + 1 == lowerLabelBounds.Count)
                {
                    double startleft = lowerLabelBounds[i];
                    double endright = 1;
                    LabelSelection(startleft, endright);
                }
                else if (pt.X > lowerLabelBounds[i] && pt.X < lowerLabelBounds[i + 1])
                {
                    double startleft = lowerLabelBounds[i];
                    double endright = lowerLabelBounds[i+1];
                    LabelSelection(startleft, endright);
                    break;
                }
                else if (pt.X < lowerLabelBounds[i])
                {
                    double startleft = 0;
                    double endright = lowerLabelBounds[0];
                    LabelSelection(startleft, endright);
                }
            }
        }

        void LabelSelection(double startleft, double endright)
        {
            var rangeEnd = 1 * (endright / (navigator.TrackSize));
            navigator.IsValueChangedTrigger = rangeEnd == navigator.RangeEnd;
            navigator.RangeStart = (1 * (startleft / (navigator.TrackSize))) == navigator.Maximum ? 0.999 : 1 * (startleft / (navigator.TrackSize));
            navigator.IsValueChangedTrigger = true;
            navigator.RangeEnd = navigator.RangeStart>rangeEnd? navigator.Maximum: rangeEnd;
        }

#if WINDOWS_PHONE
        void UpperLabelBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#else
        void UpperLabelBar_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {

#if WINDOWS_PHONE
            Point pt = e.GetPosition(upperLabelBar);
#else
            Windows.UI.Input.PointerPoint Pointer = e.GetCurrentPoint(upperLabelBar);
            Point pt = new Point(Pointer.Position.X, Pointer.Position.Y);
#endif
            if(upperLabelBounds!=null  && upperLabelBounds.Count>1)
            for (int i = 0; i < upperLabelBounds.Count; i++)
            {
                if (pt.X > upperLabelBounds[i] && i + 1 == upperLabelBounds.Count)
                {
                    double startleft = upperLabelBounds[i];
                    double endright = 1;
                    LabelSelection(startleft, endright);
                }
                else if (pt.X > upperLabelBounds[i] && pt.X < upperLabelBounds[i + 1])
                {
                    double startleft = upperLabelBounds[i];
                    double endright = upperLabelBounds[i + 1];
                    LabelSelection(startleft, endright);
                    break;
                }
                else if (pt.X < upperLabelBounds[i])
                {
                    double startleft = 0;
                    double endright = upperLabelBounds[0];
                    LabelSelection(startleft, endright);
                }
            }
       }

#if WINDOWS_PHONE
        void LowerLabelBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
#else
        void LowerLabelBar_PointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            if(hover!=null)
                (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Colors.Transparent);
        }

#if WINDOWS_PHONE
        void LowerLabelBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
#else
        void LowerLabelBar_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE
            Point pt = e.GetPosition(lowerLabelBar);
#else
            Windows.UI.Input.PointerPoint Pointer = e.GetCurrentPoint(lowerLabelBar);
            Point pt = new Point(Pointer.Position.X, Pointer.Position.Y);
#endif
            if(lowerLabelBounds!=null)
            for (int i = 0; i < lowerLabelBounds.Count; i++)
            {
                if (i + 1 < lowerLabelBounds.Count && pt.X > lowerLabelBounds[i] && pt.X < lowerLabelBounds[i + 1])
                {
                    Canvas.SetLeft((hover.Children[0] as Rectangle),lowerLabelBounds[i]);
                    (hover.Children[0] as Rectangle).Height = navigator.ActualHeight;
                    (hover.Children[0] as Rectangle).Width = lowerLabelBounds[i+1] - lowerLabelBounds[i];
                    (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                    hover.Opacity = 0.5;
                     break;
                }
                else if (pt.X < lowerLabelBounds[i])
                {
                    Canvas.SetLeft((hover.Children[0] as Rectangle), 0);
                    (hover.Children[0] as Rectangle).Height = navigator.ActualHeight;
                    (hover.Children[0] as Rectangle).Width = lowerLabelBounds[0];
                    (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                    hover.Opacity = 0.5;
                }
            }
        }

#if WINDOWS_PHONE
        void UpperLabelBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)

#else
        void UpperLabelBar_PointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            if(hover!=null)
                (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Colors.Transparent);
        }

#if WINDOWS_PHONE
        void UpperLabelBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
#else
        void UpperLabelBar_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif        
        {

#if WINDOWS_PHONE
            Point pt = e.GetPosition(upperLabelBar);
#else
            Windows.UI.Input.PointerPoint Pointer = e.GetCurrentPoint(upperLabelBar);
            Point pt = new Point(Pointer.Position.X, Pointer.Position.Y);
#endif
            if(upperLabelBounds!=null)
            for (int i = 0; i < upperLabelBounds.Count; i++)
            {
                if (i + 1 < upperLabelBounds.Count && pt.X > upperLabelBounds[i] && pt.X < upperLabelBounds[i + 1])
                {
                    Canvas.SetLeft((hover.Children[0] as Rectangle), upperLabelBounds[i]);
                    (hover.Children[0] as Rectangle).Height = navigator.ActualHeight;
                    (hover.Children[0] as Rectangle).Width = upperLabelBounds[i + 1] - upperLabelBounds[i];
                    (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                    hover.Opacity = 0.5;
                    break;
                }
                else if (pt.X < upperLabelBounds[i])
                {
                    Canvas.SetLeft((hover.Children[0] as Rectangle), 0);
                    (hover.Children[0] as Rectangle).Height = navigator.ActualHeight;
                    (hover.Children[0] as Rectangle).Width = upperLabelBounds[0];
                    (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                    hover.Opacity = 0.5;
                }
            }
        }

        void InsertLabels(string dockposition)
        {
            int count = 0;
            if (dockposition == "Lower" && lowerLabelBar!=null)
            {
               double eachinterval = navigator.TrackSize / totalNoofDays;
                double lineleft = daysvalue[count] * eachinterval;
                double setleft = 0;
                if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                    setleft = (lineleft / 2) - (txtblockwidth / 2);
                else if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                    setleft = 0;
                else
                    setleft = lineleft - txtblockwidth;
                labelElementBounds = new Rect[lowerLabelRecycler.Count];
                lowerLabelBounds = new ObservableCollection<double>();
                foreach (TextBlock txt in lowerLabelRecycler)
                {
                    txt.Visibility = Visibility.Visible;
                    if (setleft < 0)
                        txt.Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(txt, setleft);
                    Line ln1 = lowerGridLineRecycler[count];
#if NETFX_CORE || WINDOWS_PHONE8 || WINDOWS_PHONE7
                    Canvas.SetTop(txt, 2);
                    ln1.Stroke = new SolidColorBrush(Color.FromArgb(255, 42, 42, 42));
#else
                    ln1.Stroke = new SolidColorBrush(Color.FromArgb(255, 198, 198, 198));

#endif
                    ln1.X1 = Math.Round(lineleft);
                    ln1.X2 = Math.Round(lineleft);
                    ln1.Y1 = 0;
                    ln1.Y2 = 17;
                    ln1.StrokeThickness = 0.5;
                    if (ShowGridLines)
                    {
                        Line ln = innerGridLineRecycler[count];
                       
#if NETFX_CORE || WINDOWS_PHONE8 || WINDOWS_PHONE7
                        Canvas.SetTop(txt, 2);
                        ln.Stroke = new SolidColorBrush(Color.FromArgb(255, 42, 42, 42));
#else
                        ln.Stroke = new SolidColorBrush(Color.FromArgb(255,198,198,198));
#endif
                        ln.X1 = Math.Round(lineleft);
                        ln.X2 = Math.Round(lineleft);
                        ln.Y1 = 0;
                        ln.Y2 = navigator.ActualHeight;
                        ln.StrokeThickness = 0.5;
                    }
                    lowerLabelBounds.Add(lineleft);
#if WPF
                    labelElementBounds[count] = new Rect(setleft, 0, txt.DesiredSize.Width, 17);
#else
                    labelElementBounds[count] = new Rect(setleft, 0, txt.ActualWidth, 17);
#endif
                    if (count + 1 < daysvalue.Count)
                        lineleft = daysvalue[count + 1] * eachinterval;
                    if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                        setleft = ((lineleft - daysvalue[count] * eachinterval) / 2) - (txtblockwidth / 2) + (daysvalue[count] * eachinterval);
                    else if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                        setleft = (daysvalue[count] * eachinterval);
                    else
                        setleft = lineleft - txtblockwidth;
                    if (count != 0 && ((daysvalue[count] * eachinterval) - (daysvalue[count - 1] * eachinterval) < txtblockwidth))
                    {
                        txt.Visibility = Visibility.Collapsed;
                    }
                   
                    count++;
                }
            }
            else
            {
                upperLabelBounds = new ObservableCollection<double>();
                double eachinterval=0;
                if (XValues != null && (XValues as IList<DateTime>).Count>0)
                {
                    if ((XValues as IList<DateTime>).Count == 1)
                        eachinterval = navigator.TrackSize/maximumDateTimeValue.ToOADate();
                    else
                        eachinterval = navigator.TrackSize/
                                       (maximumDateTimeValue.ToOADate() -
                                        minimumDateTimeValue.ToOADate());
                }
                else if (Convert.ToString(Minimum) != "0" && Convert.ToString(Maximum) != "1")
                    eachinterval = navigator.TrackSize / (Convert.ToDateTime(Maximum).ToOADate() - Convert.ToDateTime(Minimum).ToOADate());

                double lineleft = daysvalue[count] * eachinterval;
                double setleft = 0;
                if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                    setleft = (lineleft / 2) - (txtblockwidth / 2);
                else if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                    setleft = 0;
                else
                    setleft = lineleft - txtblockwidth;

                labelElementBounds = new Rect[upperLabelRecycler.Count];
                foreach (TextBlock txt in upperLabelRecycler)
                {
                    txt.Visibility = Visibility.Visible;
                    if (setleft < 0)
                        txt.Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(txt, setleft);
                    Line ln = upperGridLineRecycler[count];
#if NETFX_CORE || WINDOWS_PHONE8 || WINDOWS_PHONE7
                    Canvas.SetTop(txt, 2);
                    ln.Stroke = new SolidColorBrush(Color.FromArgb(255,42, 42, 42));
#else
                    ln.Stroke = new SolidColorBrush(Color.FromArgb(255, 198, 198, 198));
#endif
                    ln.X1 = lineleft;
                    ln.X2 = lineleft;
                    ln.Y1 = 0;
                    ln.StrokeThickness = 0.5;
                    ln.Y2 = upperLineBar.ActualHeight;
#if WPF
                    labelElementBounds[count] = new Rect(setleft, 0, txt.DesiredSize.Width, 17);
#else
                    labelElementBounds[count] = new Rect(setleft, 0, txt.ActualWidth, 17);
#endif
                    upperLabelBounds.Add(lineleft);
                    if(count+1<daysvalue.Count)
                    lineleft = daysvalue[count+1] * eachinterval;
                    if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                        setleft = ((lineleft - daysvalue[count] * eachinterval) / 2) - (txtblockwidth / 2) + (daysvalue[count] * eachinterval);
                    else if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                        setleft = (daysvalue[count]*eachinterval)+ln.StrokeThickness;
                    else
                        setleft = lineleft-txtblockwidth-ln.StrokeThickness;
                    if (count != 0 && ((daysvalue[count] * eachinterval) - (daysvalue[count - 1] * eachinterval) < txtblockwidth))
                    {
                        txt.Visibility = Visibility.Collapsed;
                    }
                    count++;
                }
            }
#if SILVERLIGHT_UNCOMMON
            slUpdate = true;
#endif
            CalculateSelectedData();
        }

        void SetSecondInterval()
        {

        }

        void SetMinuteInterval(int minuteinterval,string dockposition)
        {

        }

        void SetHourInterval(int hourinterval,string dockposition)
        {
            daysvalue.Clear();
            txtblockwidth = 0;
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            labelElementBounds = null;
            string content= string.Empty;
            var currentDate = minimumDateTimeValue;
            switch (hourinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("hh tt", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetHourInterval(1, dockposition);
                    
                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("ht", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetHourInterval(2, dockposition);
                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("ht", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(2);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetHourInterval(3, dockposition);
                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("ht", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(4);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        ClearLabels(dockposition);
                    break;
            }
        }

        void SetDayInterval(int dayinterval, string dockposition)
        {
            daysvalue.Clear(); txtblockwidth = 0;
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            string content = string.Empty;
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            switch (dayinterval)
            {
                case 0:
                    while(currentDate<=maximumDateTimeValue)
                    {
                        content = currentDate.ToString("dddd, MMMM d, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                                
                        currentDate= new DateTime(currentDate.Year,currentDate.Month,currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }
                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                        if (navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth / 2))
                        {
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Hour"))
                                SetDayInterval(0, "Upper");
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Hour"))
                                SetHourInterval(0, "Lower");
                        }
                    }
                    else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                    {
                            InsertLabels(dockposition);
                    }
                    else
                            SetDayInterval(1, dockposition);
                    
                    break;
                case 1:
                    while(currentDate<=maximumDateTimeValue)
                    {
                          content = currentDate.ToString("ddd, MMM d, yyyy", CultureInfo.CurrentCulture);
                          if (dockposition == "Lower")
                              lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                          else
                              upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                          currentDate= new DateTime(currentDate.Year,currentDate.Month,currentDate.Day).AddDays(1);
                          AddDaysValues(currentDate);
                    }
                        if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                            InsertLabels(dockposition);
                        else
                            SetDayInterval(2, dockposition);
                    break;
                case 2:
                    while(currentDate<=maximumDateTimeValue)
                    {
                        content = currentDate.ToString("dddd, d", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                        {
                           lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }
                        currentDate= new DateTime(currentDate.Year,currentDate.Month,currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }
                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetDayInterval(3, dockposition);
                    
                    break;
                case 3:
                    while(currentDate<=maximumDateTimeValue)
                    {
                        content = currentDate.Day.ToString();
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });                              
                        currentDate= new DateTime(currentDate.Year,currentDate.Month,currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Week"))
                        SetWeekInterval(0, dockposition);
                    else
                        ClearLabels(dockposition);
                    
                    break;
                default:
                    break;
            }
        }

        private int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
      
        void SetWeekInterval(int weekinterval, string dockposition)
        {
            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
                
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content= string.Empty;
            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }
            switch (weekinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = "Week" + GetWeekNumber(currentDate) + currentDate.ToString(" MMMM, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                              currentDate=  currentDate.AddDays(1);
                            }
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                          currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                          currentDate=  currentDate.AddDays(7);
                        }
                        AddDaysValues(currentDate);

                    }
                        if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                        {
                            InsertLabels(dockposition);
                            if (navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth / 2))
                            {
                                if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Day"))
                                SetWeekInterval(0, "Upper");
                                if (navigatorIntervals.Contains("Day"))
                                    SetDayInterval(0, "Lower");
                                else if(navigatorIntervals.Contains("Hour"))
                                    SetHourInterval(0,"Lower");
                                else if(navigatorIntervals.Count == 0)
                                    SetDayInterval(0, "Lower");
                            }
                        }
                        else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                        {
                            InsertLabels(dockposition);
                        }
                        else
                            SetWeekInterval(1, dockposition);
                    
                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = "Week" + GetWeekNumber(currentDate) + currentDate.ToString(" MMMM, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }
                        AddDaysValues(currentDate);

                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetWeekInterval(2, dockposition);
                    
                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = "Week" + GetWeekNumber(currentDate);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }
                        AddDaysValues(currentDate);
                    }
                        if (GenerateLabelContainers(dockposition))
                        {
                            InsertLabels(dockposition);
                        }
                        else
                            SetWeekInterval(3, dockposition);
                    
                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content= "W" + GetWeekNumber(currentDate);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                                                
                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Month"))
                        SetMonthInterval(0, dockposition);
                    else
                        ClearLabels(dockposition);
                     
                    break;
                default:
                    break;
            }
        }
        string dockPosition = "Lower";
       
        void SetMonthInterval(int monthinterval,string dockposition)
        {
            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content = string.Empty;
            switch (monthinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("MMMM, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                        {
                            lowerBarLabels.Add(new ChartAxisLabel { LabelContent=  content });
                        }
                        else
                        {
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }
                        
                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                        
                    }
                        if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                        {
                            InsertLabels(dockposition);
                            if (navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth * 30))
                            {
                                if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Week"))
                                    SetMonthInterval(0, "Upper");
                                if (navigatorIntervals.Contains("Week"))
                                    SetWeekInterval(0, "Lower");
                                else if (navigatorIntervals.Contains("Day"))
                                    SetDayInterval(0, "Lower");
                                else if (navigatorIntervals.Count == 0)
                                    SetWeekInterval(0, "Lower");
                            }
                            else
                                SetQuarterInterval(0, "Upper");
                        }
                        else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                        {
                            InsertLabels(dockposition);
                        }
                        else
                            SetMonthInterval(1, dockposition);
                    
                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("MMMM", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetMonthInterval(2, dockposition);
                    
                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content = currentDate.ToString("MMM", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        SetMonthInterval(3, dockposition);
                    
                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        content= currentDate.ToString("MMM", CultureInfo.CurrentCulture).Substring(0,1);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        
                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Quarter"))
                        SetQuarterInterval(0, dockposition);
                    else
                        ClearLabels(dockposition);
                    break;
                default:
                    break;
            };
        }

        private void ClearLabels(string dockposition)
        {
            if (dockposition == "Lower")
            {
                lowerBarLabels.Clear();
                if(lowerLabelBounds!=null)
                lowerLabelBounds.Clear();
            }
            else
            {
                upperBarLabels.Clear();
                if(upperLabelBounds!=null)
                upperLabelBounds.Clear();
            }
            GenerateLabelContainers(dockposition);
        }

        void SetQuarterInterval(int quarterinterval,string dockpostion)
        {
            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockpostion == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
        
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content=string.Empty;
            
            switch (quarterinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            content = "Quarter 1, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            content = "Quarter 2, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            content = "Quarter 3, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            content = "Quarter 4, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year+1, 1, 1);
                        }
                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() {LabelContent= content});
                        
                        AddDaysValues(currentDate);
                    }
                    if (dockpostion == "Lower" && GenerateLabelContainers(dockpostion))
                        {
                            InsertLabels(dockpostion);
                            if (navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > txtblockwidth / 2)
                            {
                                if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Month"))
                                    SetQuarterInterval(0, "Upper");
                                if (navigatorIntervals.Contains("Month"))
                                    SetMonthInterval(0, "Lower");
                                else if (navigatorIntervals.Contains("Week"))
                                    SetWeekInterval(0, "Lower");
                                else if (navigatorIntervals.Contains("Day"))
                                    SetDayInterval(0, "Lower");
                                else if (navigatorIntervals.Count == 0)
                                    SetMonthInterval(0, "Lower");
                            }
                        }
                        else if (dockpostion == "Upper" && GenerateLabelContainers(dockpostion))
                        {
                            InsertLabels(dockpostion);
                        }
                        else
                            SetQuarterInterval(1, dockpostion);
                        break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            content = "Quarter 1, " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            content = "Quarter 2, " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            content = "Quarter 3, " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            content = "Quarter 4, " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year+1, 1, 1);
                        }
                        if (dockpostion == "Lower")
                        {
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }
                        else
                        {
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }
                        AddDaysValues(currentDate);
                    }
                        if (GenerateLabelContainers(dockpostion))
                            InsertLabels(dockpostion);
                        else
                            SetQuarterInterval(2, dockpostion);
                    
                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            content = "Q1, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            content = "Q2, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            content = "Q3, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            content = "Q4, " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year+1, 1, 1);
                        }
                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockpostion))
                        InsertLabels(dockpostion);
                    else
                        SetQuarterInterval(3, dockpostion);
                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            content = "Q1";
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            content = "Q2";
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            content = "Q3";
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            content = "Q4";
                            currentDate = new DateTime(currentDate.Year+1, 1, 1);
                        }
                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        AddDaysValues(currentDate);
                    }
                    if (GenerateLabelContainers(dockpostion))
                        InsertLabels(dockpostion);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Year"))
                    {
                        ClearLabels("Upper");
                        SetYearInterval(0, "Lower");
                    }
                    else
                        ClearLabels(dockpostion);
                    break;
                default:
                    break;
            }
        }
        
        void SetYearInterval(int yearInterval, string dockposition)
        {
            if (isMinMaxSet)
            {
                bool isLastCategory = false;
                if (dockposition == "Upper")
                    upperBarLabels.Clear();
                else
                    lowerBarLabels.Clear();
                daysvalue.Clear();
                var currentDate = minimumDateTimeValue;
                switch(yearInterval)
                {
                    case 0:
                        while (currentDate <= maximumDateTimeValue)
                        {
                            if(dockposition=="Upper")
                                upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yyyy", CultureInfo.CurrentCulture) });
                            else
                                lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yyyy", CultureInfo.CurrentCulture) });
                            if ((currentDate - minimumDateTimeValue).TotalDays != 0)
                                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }
                        break;
                    case 1:
                        while (currentDate <= maximumDateTimeValue)
                        {
                            if (dockposition == "Upper")
                                upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yy", CultureInfo.CurrentCulture) });
                            else
                                lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yy", CultureInfo.CurrentCulture) });
                            if ((currentDate - minimumDateTimeValue).TotalDays != 0)
                                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            isLastCategory = true;
                        }
                        break;
                    default:
                        break;
                }
                daysvalue.Add((maximumDateTimeValue - minimumDateTimeValue).TotalDays);
                if (GenerateLabelContainers(dockposition))
                    InsertLabels(dockposition);
                else if (!isLastCategory)
                    SetYearInterval(1, dockposition);
                else
                    ClearLabels(dockposition);
             
            }

            if (navigatorIntervals.Contains("Quarter") && dockPosition!="Lower")
                SetQuarterInterval(0, dockPosition);
            else if (navigatorIntervals.Contains("Month") && dockPosition != "Lower")
                SetMonthInterval(0, dockPosition);
            else if (navigatorIntervals.Contains("Week") && dockPosition != "Lower")
                SetWeekInterval(0, dockPosition);
            else if (navigatorIntervals.Contains("Day") && dockPosition != "Lower")
                SetDayInterval(0, dockPosition);
            else if (navigatorIntervals.Contains("Hour") && dockPosition != "Lower")
                SetHourInterval(0, dockPosition);
        }

        private bool GenerateLabelContainers(string postion)
        {
            txtblockwidth = 0;
            int i=0;
            ObservableCollection<ChartAxisLabel> labels = new ObservableCollection<ChartAxisLabel>();
            UIElementsRecycler<TextBlock> labelsRecyler = null;
            Panel panel;
            if (postion == "Upper")
            {
                labelsRecyler = upperLabelRecycler;
                labels = upperBarLabels;
                panel = upperLabelBar;
                upperGridLineRecycler.GenerateElements(upperBarLabels.Count);
            }
            else
            {
                labelsRecyler = lowerLabelRecycler;
                labels = lowerBarLabels;
                panel = lowerLabelBar;
                lowerGridLineRecycler.GenerateElements(lowerBarLabels.Count);
                innerGridLineRecycler.GenerateElements(lowerBarLabels.Count);
            }

            labelsRecyler.GenerateElements(labels.Count);
            foreach (var label in labels)
            {
                  TextBlock textBlock = labelsRecyler[i];
                  textBlock.Text = label.LabelContent.ToString();
                  textBlock.Visibility = Visibility.Visible;
                 // textBlock.Style = labelStyle;
                  textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                  i++;
                  textBlock.Measure(navigator.DesiredSize);
#if WPF
                  txtblockwidth = Math.Max(textBlock.DesiredSize.Width, txtblockwidth);
#else
                  txtblockwidth = Math.Max(textBlock.ActualWidth, txtblockwidth);
#endif
            }
            return (txtblockwidth * panel.Children.Count < navigator.TrackSize);
        }
        
        internal void Update()
        {
            navigatorIntervals = new ObservableCollection<string>();
            string currentdockPos = "Upper";
            if (isMinMaxSet)
            {
                if (navigator != null)
                    navigator.IsValueChangedTrigger = true;
                DateTime start = minimumDateTimeValue;
                DateTime end = maximumDateTimeValue;
                double difference = (end - start).TotalDays;
                bool isLowerGenerated = false;
                if (Intervals!=null && Intervals.Count > 0)
                {
                    foreach (Interval navinterval in Intervals)
                    {
                        navigatorIntervals.Add(navinterval.IntervalType.ToString());
                    }

                    if (navigatorIntervals.Contains("Year") && difference >= 364)
                    {
                        SetYearInterval(0, currentdockPos);
                        currentdockPos = "Lower";
                    }
                    if (navigatorIntervals.Contains("Quarter") && difference >= 90)
                    {
                        SetQuarterInterval(0, currentdockPos);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";

                    }
                    if (navigatorIntervals.Contains("Month") && difference >= 30 && !isLowerGenerated)
                    {
                        SetMonthInterval(0, currentdockPos);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }
                    if (navigatorIntervals.Contains("Week") && difference >= 7 && !isLowerGenerated)
                    {
                        SetWeekInterval(0, currentdockPos);
                       if (currentdockPos == "Lower")
                           isLowerGenerated = true;
                       else
                           currentdockPos = "Lower";
                    }
                    if (navigatorIntervals.Contains("Day") && difference >= 1 && !isLowerGenerated)
                    {
                        SetDayInterval(0, currentdockPos);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }
                    if (navigatorIntervals.Contains("Hour") && !isLowerGenerated)
                    {
                        SetHourInterval(0, currentdockPos);
                    }
                    if (upperLabelRecycler.Count == 0 && upperLabelBar != null)
                       SetYearInterval(0,"Upper");
                    if (lowerLabelBar != null && !isLowerGenerated)
                         SetQuarterInterval(0, dockPosition);
                }
                else
                {
                    if (upperLabelBar != null)
                        SetYearInterval(0, "Upper");
                    if (lowerLabelBar != null)
                        SetQuarterInterval(0, dockPosition);
                }
            }
            CalculateTooltipPosition();
        }
        
        protected override void OnTimeLineSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(navigator!=null)
                Update();            
        }

        protected override void OnTimeLineValueChanged(object sender, EventArgs e)
        {
            if (!isUpdate)
            {
                CalculateSelectedData();
            }
            OnValueChanged();
        }

        protected override void OnScrollbarValueChanged(object sender, EventArgs e)
        {
#if WPF || SILVERLIGHT_UNCOMMON
            isScrolling = true;
#endif
            base.OnScrollbarValueChanged(sender, e);
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (navigator.Content != null)
            {
                double newmargin = xrange +navigator.ResizableThumbSize;
                double size = xrange == 0 ? navigator.DesiredSize.Width : navigator.TrackSize;
                if(!scrollbar.isFarDragged)
                {
                    if (tool != null)
                    {
                        tool.Width = size * scrollbar.Scale;
                        tool.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                    upperLabelBar.Width = size * scrollbar.Scale;
                    upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    upperLineBar.Width = size * scrollbar.Scale;
                    upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Width = size * scrollbar.Scale;
                    lowerLineBar.Width = size * scrollbar.Scale;
                    lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                }
                else if (!scrollbar.isNearDragged && xrange != 0)
                {
                    if (tool != null)
                    {
                        tool.Width = navigator.TrackSize * scrollbar.Scale;
                        tool.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                    upperLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                    upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    upperLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                    upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Width = navigator.TrackSize * scrollbar.Scale;
                    lowerLineBar.Width = navigator.TrackSize * scrollbar.Scale;
                    lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                }
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
            }
            CalculateTooltipPosition();
#endif
        }

#if WPF
        private void GenerateDataTablePoints()
        {
            DataEnd = 0;
            IEnumerator enumerator = (ItemsSource as DataTable).Rows.GetEnumerator();
            if (enumerator.MoveNext())
            {
                this.XValues = new List<DateTime>();
                IList<DateTime> xValue = this.XValues as List<DateTime>;
                do
                {
                    object xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                    xValue.Add((DateTime)xVal); DataEnd++;
                } while (enumerator.MoveNext());
            }
        }
#endif

        void ClearLabels()
        {
            minimumDateTimeValue = DateTime.MinValue;
            maximumDateTimeValue = DateTime.MinValue;
            if(upperBarLabels!=null)
                upperBarLabels.Clear();
            if(lowerBarLabels!=null)
                lowerBarLabels.Clear();
            if(lowerLabelBounds!=null)
                lowerLabelBounds.Clear();
            if(upperLabelBounds!=null)
                upperLabelBounds.Clear();
            if (upperLabelBar != null && lowerLabelBar!=null)
            {
                GenerateLabelContainers("Upper");
                GenerateLabelContainers("Lower");
                CalculateTooltipPosition();
            }
        }


        #endregion

    }

    [ClassReference(IsReviewed = false)]
    public class Intervals : ObservableCollection<Interval>
    {
        public Intervals()
        {
        }
    }

    public class Interval : DependencyObject
    {
        public Interval()
        {

        }
        /// <summary>
        /// Gets or sets interval type in which the navigator values should be displayed.
        /// </summary>
        public NavigatorIntervalType IntervalType
        {
            get { return (NavigatorIntervalType)GetValue(IntervalTypeProperty); }
            set { SetValue(IntervalTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register("IntervalType", typeof(NavigatorIntervalType), typeof(Interval), new PropertyMetadata(NavigatorIntervalType.Year));

        
    }
}

