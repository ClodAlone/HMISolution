#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class ChartAxisBase2D : ChartAxis
    {

        #region ctor

        public ChartAxisBase2D()
        {
            DefaultStyleKey = typeof(ChartAxisBase2D);
            GridLines = new List<Line>();
            MinorGridLines = new List<Line>();
            m_VisibleLabels = new ObservableCollection<ChartAxisLabel>();
            Binding visibilityBinding = new Binding();
            visibilityBinding.Source = this;
            visibilityBinding.Path = new PropertyPath("Visibility");
            BindingOperations.SetBinding(this, AxisVisibilityProperty, visibilityBinding);
            StripLines = new ChartStripLines();
        }

        #endregion

        #region fields

#if WINDOWS_PHONE
        bool isUpdateStripDispatched = false;
#else
        IAsyncAction updateStripLinesAction;
#endif

        private ChartCartesianAxisPanel axisPanel;

        private Panel labelsPanel;

        private Panel elementsPanel;

#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
        SfChartResizableBar sfChartResizableBar;
        double rangeEnd = 1;
#endif

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets zoom position. Value must fall within 0 to 1. It determines starting value of visible range
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register("ZoomPosition", typeof(double), typeof(ChartAxisBase2D), new PropertyMetadata(0d, OnZoomPositionChanged));

        /// <summary>
        /// Gets or Sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ChartAxisBase2D), new PropertyMetadata(1d, OnZoomFactorChanged));

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisBase2D).OnZoomDataChanged(e);
        }

        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisBase2D).OnZoomDataChanged(e);
        }
        private void OnZoomDataChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Gets or sets the strip lines.
        /// </summary>
        /// <value>
        /// The strip lines.
        /// </value>
        public ChartStripLines StripLines
        {
            get { return (ChartStripLines)GetValue(StripLinesProperty); }
            set { SetValue(StripLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StripLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StripLinesProperty =
            DependencyProperty.Register("StripLines", typeof(ChartStripLines), typeof(ChartAxisBase2D), new PropertyMetadata(null, OnStripLinesChanged));

        private static void OnStripLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAxisBase2D).OnStripLinesChanged(args.NewValue as ChartStripLines, args.OldValue as ChartStripLines);
        }

        private void OnStripLinesChanged(ChartStripLines newValue, ChartStripLines oldValue)
        {
            if (newValue != null)
            {
                newValue.CollectionChanged += OnStripLinesCollectionChanged;
                foreach (var stripLine in newValue)
                {
                    stripLine.PropertyChanged += OnStripLinesPropertyChanged;
                    if (axisPanel != null && !axisPanel.Children.Contains(stripLine))
                    {
                        axisPanel.Children.Add(stripLine);
                    }
                }
            }
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnStripLinesCollectionChanged;
                foreach (var stripLine in oldValue)
                {
                    stripLine.PropertyChanged -= OnStripLinesPropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(stripLine))
                    {
                        axisPanel.Children.Remove(stripLine);
                    }
                }
            }
            UpdateStripsLines();
        }

#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7

        /// <summary>
        /// Gets or sets a value indicating whether [enable scroll bar resizing].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable scroll bar resizing]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableScrollBarResizing
        {
            get { return (bool)GetValue(EnableScrollBarResizingProperty); }
            set { SetValue(EnableScrollBarResizingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableScrollBarResizing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableScrollBarResizingProperty =
            DependencyProperty.Register("EnableScrollBarResizing", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, OnScrollBarResizableChanged));


        private static void OnScrollBarResizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAxisBase2D).OnScrollBarResizableChanged();
        }

        private void OnScrollBarResizableChanged()
        {
            if (sfChartResizableBar != null)
                sfChartResizableBar.UpdateResizable(EnableScrollBarResizing);
        }

        /// <summary>
        /// Gets or Sets a value that determines whether to enable or disable scroll bar.
        /// </summary>
        public bool EnableScrollBar
        {
            get { return (bool)GetValue(EnableScrollBarProperty); }
            set { SetValue(EnableScrollBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableScrollBar. This enables the resizable scroll bar view in the chart.
        public static readonly DependencyProperty EnableScrollBarProperty = DependencyProperty.Register("EnableScrollBar", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, OnEnableScrollBarValueChanged));

        private static void OnEnableScrollBarValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = d as ChartAxisBase2D;
            if (chartAxis.Area != null && chartAxis.sfChartResizableBar != null)
            {
                chartAxis.sfChartResizableBar.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                chartAxis.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Gets or Sets a value that determines whether to defer zooming.
        /// </summary>
        public bool DeferredScrolling
        {
            get { return (bool)GetValue(DeferredScrollingProperty); }
            set { SetValue(DeferredScrollingProperty, value); }
        }

        // Using  a DependencyProperty as the backing store for DeferredScrolling. This enable the deferred binding to zoom position and zoom factor.
        public static readonly DependencyProperty DeferredScrollingProperty = DependencyProperty.Register("DeferredScrolling", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

#if NETFX_CORE
        /// <summary>
        /// Gets or Sets a value to enable or disable touch mode of the scroll bar.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouchMode. This enables the touch mode for the resizable scroll bar etc...
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register("EnableTouchMode", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, OnEnableTouchModeChanged));

#else
#if WPF || SILVERLIGHT_UNCOMMON
        /// <summary>
        /// Gets or Sets the EnableTouchMode for the resizable scroll bar.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouchMode. This enables the touch mode for the resizable scroll bar etc...
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register("EnableTouchMode", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false,OnEnableTouchModeChanged));
#endif
#endif

#if WPF || SILVERLIGHT_UNCOMMON || NETFX_CORE
        private static void OnEnableTouchModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as ChartAxisBase2D;
#if NETFX_CORE
            //For IR 17848 -  	Designer crashes when rebuild the SfChart
            bool _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
            if (!(bool)e.NewValue && _isInDesignMode)
                axis.EnableTouchMode = true;
            else
            {
                if (!_isInDesignMode)
                    axis.ChangeStyle((bool)e.NewValue);
            }
#else
                axis.ChangeStyle((bool)e.NewValue);
#endif
            if (axis.EnableScrollBar && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        internal void ChangeStyle(bool modeValue)
        {
            if (sfChartResizableBar != null)
            {
                if (modeValue == true)
                {
                    if (OpposedPosition)
                    {
                        VisualStateManager.GoToState(this, "OpposedTouchModeStyle", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "TouchModeStyle", true);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(this, "CommonStyle", true);
                }
#if NETFX_CORE
                if (axisPanel != null)
                    axisPanel.InvalidateMeasure();
#endif
                sfChartResizableBar.UpdateResizable(EnableScrollBarResizing);
            }
            
        }
#endif
#endif

#endregion

        #region methods

        internal override void CreateLineRecycler()
        {
            if (this.Area != null && (Area as SfChart).GridLinesPanel != null)
            {
                GridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                MinorGridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
            }
        }

        private void OnStripLinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ChartStripLine item in e.NewItems)
                {
                    item.PropertyChanged += OnStripLinesPropertyChanged;
                    if (axisPanel != null && !axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Add(item);
                    }
                }
                if (Area != null)
                    UpdateStripsLines();

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ChartStripLine item in e.OldItems)
                {
                    item.PropertyChanged -= OnStripLinesPropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Remove(item);
                    }
                }
                UpdateStripsLines();
            }
        }

        private void UpdateStripsLines()
        {
            if (Area != null)
                (Area as SfChart).UpdateStripLines();
#if !WINDOWS_PHONE
            updateStripLinesAction = null;
#else
            isUpdateStripDispatched=false;
#endif
        }

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render chart Axis.
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            {
                base.OnApplyTemplate();
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
                sfChartResizableBar = this.GetTemplateChild("sfchartResizableBar") as SfChartResizableBar;
                if (sfChartResizableBar != null)
                {
                    sfChartResizableBar.Visibility = EnableScrollBar ? Visibility.Visible : Visibility.Collapsed;
                    sfChartResizableBar.Axis = this;
                    sfChartResizableBar.Orientation = this.Orientation;
#if NETFX_CORE
                    //For IR 17848 -  	Designer crashes when rebuild the SfChart
                    bool _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
                    if (!_isInDesignMode)
#endif
                        if (this.EnableScrollBar)
                        {
                            ChangeStyle(this.EnableTouchMode);
                        }
                }
#endif
                labelsPanel = this.GetTemplateChild("axisLabelsPanel") as Panel;
                elementsPanel = this.GetTemplateChild("axisElementPanel") as Panel;
                axisPanel = this.GetTemplateChild("axisPanel") as ChartCartesianAxisPanel;
                if (axisPanel != null)
                {
                    foreach (var stripLine in StripLines)
                    {
                        if (!axisPanel.Children.Contains(stripLine))
                            axisPanel.Children.Add(stripLine);
                    }
                }
                UpdatePanels();

                axisPanel.Axis = this;

                headerContent = this.GetTemplateChild("headerContent") as ContentControl;

                if (axisElementsUpdateRequired)
                {
                    if (axisElementsPanel != null)
                        axisElementsPanel.UpdateElements();
                    if (axisLabelsPanel != null)
                        axisLabelsPanel.UpdateElements();
                    axisElementsUpdateRequired = false;
                }

                if (this.Area != null && (Area as SfChart).GridLinesPanel != null)
                {
                    GridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                    MinorGridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                }
            }
        }

        internal override void ComputeDesiredSize(Size size)
        {
            this.ClearValue(HeightProperty);
            this.ClearValue(WidthProperty);
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
            if (sfChartResizableBar != null)
            {
                sfChartResizableBar.ClearValue(Control.WidthProperty);
                sfChartResizableBar.ClearValue(Control.HeightProperty);
            }
#endif
            AvailableSize = size;
            CalculateRangeAndInterval(size);
            if (Visibility != Visibility.Collapsed)
            {
                ApplyTemplate();
                if (axisPanel != null)
                {
                    UpdatePanels();
                    UpdateLabels();
                    ComputedDesiredSize = axisPanel.ComputeSize(size);
                }
            }
            else
            {
                ActualPlotOffset = PlotOffset;
                InsidePadding = 0;
                UpdateLabels();
                ComputedDesiredSize = Orientation == Orientation.Horizontal
                                          ? new Size(size.Width, 0)
                                          : new Size(0, size.Height);
            }
        }

        private void OnStripLinesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
#if WINDOWS_PHONE
            if (!isUpdateStripDispatched)
            {
#if WPF 
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(UpdateStripsLines));
#else
                Dispatcher.BeginInvoke(UpdateStripsLines);
#endif
                isUpdateStripDispatched = true;
            }
            
#else
            if (updateStripLinesAction == null && Area != null)
            {
                updateStripLinesAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdateStripsLines);
            }
#endif
        }

        void UpdatePanels()
        {
            if (axisPanel != null)
            {
                if (AxisLayoutPanel is ChartPolarAxisLayoutPanel
                    && !(axisLabelsPanel is ChartCircularAxisPanel))
                {
                    if (axisLabelsPanel != null)
                    {
                        axisLabelsPanel.DetachElements();
                    }
                    if (axisElementsPanel != null)
                    {
                        axisElementsPanel.DetachElements();
                    }
                    axisPanel.LayoutCalc.Clear();
                    axisLabelsPanel = new ChartCircularAxisPanel(labelsPanel)
                    {
                        Axis = this
                    };
                    axisPanel.LayoutCalc.Add(axisLabelsPanel as ILayoutCalculator);
                }
                else if (!(AxisLayoutPanel is ChartPolarAxisLayoutPanel)
                    && !(axisLabelsPanel is ChartCartesianAxisLabelsPanel))
                {
                    if (axisLabelsPanel != null)
                    {
                        axisLabelsPanel.DetachElements();
                    }
                    if (axisElementsPanel != null)
                    {
                        axisElementsPanel.DetachElements();
                    }
                    axisPanel.LayoutCalc.Clear();
                    axisLabelsPanel = new ChartCartesianAxisLabelsPanel(labelsPanel)
                    {
                        Axis = this
                    };
                    axisElementsPanel = new ChartCartesianAxisElementsPanel(elementsPanel)
                    {
                        Axis = this
                    };
                    axisPanel.LayoutCalc.Add(axisLabelsPanel as ILayoutCalculator);
                    axisPanel.LayoutCalc.Add(axisElementsPanel as ILayoutCalculator);
                }
            }
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            VisibleRange = ActualRange;
            VisibleInterval = ActualInterval;
            if (ZoomFactor < 1 || ZoomPosition > 0)
            {
                DoubleRange baseRange = ActualRange;

                double start = ActualRange.Start + ZoomPosition * ActualRange.Delta;
                double end = start + ZoomFactor * ActualRange.Delta;

                if (start < baseRange.Start)
                {
                    end = end + (baseRange.Start - start);
                    start = baseRange.Start;
                }

                if (end > baseRange.End
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
 || (this.EnableScrollBar && sfChartResizableBar != null && Math.Round(end) == baseRange.End && sfChartResizableBar.RangeEnd == rangeEnd 
                    && !(this.RegisteredSeries[0] as ChartSeries).IsActualTransposed)
#endif
)
                {
                    start = start - (end - baseRange.End);
                    end = baseRange.End;
                }
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
                rangeEnd = sfChartResizableBar != null ? sfChartResizableBar.RangeEnd : 1;
#endif

                VisibleRange = new DoubleRange(start, end);
            }
        }

        protected internal override void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            base.OnAxisBoundsChanged(args);
#if NETFX_CORE || WPF || SILVERLIGHT_UNCOMMON
            if (sfChartResizableBar != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    sfChartResizableBar.Width = ArrangeRect.Width;
                    if (Area.VisibleSeries.Count != 0)
                    {
                        this.Width = ArrangeRect.Width;
                    }
                }
                else
                {
                    sfChartResizableBar.Height = ArrangeRect.Height;
                    if (Area.VisibleSeries.Count != 0)
                    {
                        this.Height = ArrangeRect.Height;
                    }
                }
            }
#endif
            if (axisPanel != null)
            {
                axisPanel.ArrangeElements(new Size(ArrangeRect.Width, ArrangeRect.Height));
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            (obj as ChartAxisBase2D).ZoomFactor = ZoomFactor;
            (obj as ChartAxisBase2D).ZoomPosition = ZoomPosition;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
