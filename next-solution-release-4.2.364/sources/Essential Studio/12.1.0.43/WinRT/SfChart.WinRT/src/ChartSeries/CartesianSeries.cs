#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CartesianSeries
    /// </summary>
    public abstract class CartesianSeries: AdornmentSeries ,ISupportAxes2D
    {
        public CartesianSeries()
        {
            Trendlines = new ChartTrendLineCollection();
        }

        #region Properties
        /// <summary>
        /// Get or Set Trendlines property
        /// </summary>
        public ChartTrendLineCollection Trendlines
        {
            get { return (ChartTrendLineCollection)GetValue(TrendlinesProperty); }
            set { SetValue(TrendlinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Trendlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrendlinesProperty =
            DependencyProperty.Register("Trendlines", typeof(ChartTrendLineCollection), typeof(CartesianSeries), new PropertyMetadata(null, OnTrendlinesChanged));

        
        /// <summary>
        /// Get or Set  XRange property
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Get YRange property
        /// </summary>
        public DoubleRange YRange{ get; internal set; }

        /// <summary>
        /// Get or Set XAxis property
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxisBase2D), typeof(CartesianSeries), new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// Get or Set YAxis property
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(RangeAxisBase), typeof(CartesianSeries), new PropertyMetadata(null, OnYAxisChanged));
        
        /// <summary>
        /// Gets or Sets Orientation for Chart Series 
        /// </summary>
        public bool IsTransposed
        {
            get { return (bool)GetValue(IsTransposedProperty); }
            set { SetValue(IsTransposedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  
        public static readonly DependencyProperty IsTransposedProperty =
            DependencyProperty.Register("IsTransposed", typeof(bool), typeof(CartesianSeries), new PropertyMetadata(false, OnTransposeChanged));

        private static void OnTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianSeries).OnTransposeChanged(Convert.ToBoolean(e.NewValue));
        }

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = val;
        }

        ChartAxis ISupportAxes.ActualXAxis 
        { 
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }

        #endregion

        #region methods

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianSeries).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianSeries).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }
        /// <summary>
        /// Called when instance created for YAxis Changed 
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Area = Area;
                newAxis.RegisteredSeries.Add(this);
            }

            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }
            if (Area != null) Area.ScheduleUpdate();
            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed? Orientation.Horizontal: Orientation.Vertical;
        }
        /// <summary>
        /// Called when instance created for XAxis changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                oldAxis.VisibleRangeChanged -= OnVisibleRangeChanged;
              
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis) && newAxis!=Area.InternalPrimaryAxis)
                    Area.Axes.Add(newAxis);
                newAxis.Area = Area;
                if (!newAxis.RegisteredSeries.Contains(this))
                    newAxis.RegisteredSeries.Add(this);
                newAxis.VisibleRangeChanged += OnVisibleRangeChanged;
            }
            if (Area != null) Area.ScheduleUpdate();
            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
        }

        void OnVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            OnVisibleRangeChanged(e);
        }

        /// <summary>
        /// Called when VisibleRange property changed
        /// </summary>
        protected virtual void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }

            if (IsSideBySide)
            {
                if (SideBySideInfoRangePad != null && !SideBySideInfoRangePad.IsEmpty)
                {
                    bool isAlterRange = ((this.ActualXAxis is NumericalAxis && (this.ActualXAxis as NumericalAxis).RangePadding == NumericalPadding.None)
                   || (this.ActualXAxis is DateTimeAxis && (this.ActualXAxis as DateTimeAxis).RangePadding == DateTimeRangePadding.None));
                    XRange = isAlterRange ? new DoubleRange(XRange.Start - SideBySideInfoRangePad.Start, XRange.End - SideBySideInfoRangePad.End) 
                        : new DoubleRange(XRange.Start + SideBySideInfoRangePad.Start, XRange.End + SideBySideInfoRangePad.End);
                }
            }
             
	        foreach (var item in Trendlines)
            {
                foreach (ChartSegment segment in item.TrendlineSegments)
                {
                        XRange += segment.XRange;
                        YRange += segment.YRange;
                }                
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            if ((this as CartesianSeries).XAxis != this.Area.InternalPrimaryAxis)
                (obj as CartesianSeries).XAxis = (ChartAxisBase2D)((this as CartesianSeries).XAxis).Clone();
            if ((this as CartesianSeries).YAxis != this.Area.InternalSecondaryAxis)
                (obj as CartesianSeries).YAxis = (RangeAxisBase)((this as CartesianSeries).YAxis).Clone();
            (obj as CartesianSeries).IsTransposed = this.IsTransposed;
            return base.CloneSeries(obj);
        }

	private static void OnTrendlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cartesianSeries = d as CartesianSeries;
            if (cartesianSeries != null) cartesianSeries.OnTrendlinesChanged(e);
        }

        private void OnTrendlinesChanged(DependencyPropertyChangedEventArgs e)
        {
             if (e.OldValue != null)
            {
                if (SeriesPanel != null)
                foreach (var trend in (e.OldValue as ChartTrendLineCollection))
                {
                    if (SeriesPanel.Children.Contains(trend))
                        SeriesPanel.Children.Remove(trend);
                }
                (e.OldValue as ChartTrendLineCollection).Clear();
                (e.OldValue as ChartTrendLineCollection).CollectionChanged -= Trendlines_CollectionChanged;
               
            }
            if (e.NewValue != null)
            {
                Trendlines.CollectionChanged += Trendlines_CollectionChanged;
            }
            if (Area != null) Area.ScheduleUpdate();
        }
        private void Trendlines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var trend = e.NewItems[0] as TrendlineBase;
                if (trend != null && SeriesPanel != null && Area != null)
                {
                    trend.Series = this;
                    SeriesPanel.Children.Add(trend);
                    Area.UpdateLegend(Area.Legend, false);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var trend = e.OldItems[0] as TrendlineBase;
                if (trend != null && SeriesPanel != null)
                {
                    if (SeriesPanel.Children.Contains(trend))
                    {
                        SeriesPanel.Children.Remove(trend);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (SeriesPanel != null)
                {
                    foreach (var trend in Trendlines)
                    {
                        if (SeriesPanel.Children.Contains(trend))
                            SeriesPanel.Children.Remove(trend);
                    }
                    Trendlines.Clear();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var trend = e.NewItems[0] as TrendlineBase;
                if (trend != null && SeriesPanel != null)
                {
                    trend.Series = this;
                    SeriesPanel.Children.Add(trend);            
                }
            }
            if (Area != null) Area.ScheduleUpdate();
        }
/// <summary>
        /// Update series bound
        /// </summary>
        /// <param name="size"></param>
        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            base.UpdateOnSeriesBoundChanged(size);
            foreach (var trend in Trendlines)
            {
                if (trend.TrendlinePanel != null)
                {
                    foreach (ChartSegment segment in trend.TrendlineSegments)
                    {
                        segment.OnSizeChanged(size);
                    }
                    trend.TrendlinePanel.Update(size);
                }
            }
           
        }

        #if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            foreach (var trend in Trendlines)
            {
                if (!this.SeriesPanel.Children.Contains(trend))
                {
                    trend.Series = this;
                    this.SeriesPanel.Children.Add(trend);
                    Canvas.SetZIndex(trend, 1);
                    
                }
            }
        }

        /// <summary>
        /// Calculate Segments
        /// </summary>
        internal override void CalculateSegments()
        {
            base.CalculateSegments();
            CreateTrendline();
        }

        /// <summary>
        /// Create trend line for series
        /// </summary>
        internal virtual void CreateTrendline()
        {
            foreach (var trend in Trendlines)
            {
                trend.ApplyTemplate();
                trend.UpdateElements();
            }
        }
        

        #endregion
    }
}
