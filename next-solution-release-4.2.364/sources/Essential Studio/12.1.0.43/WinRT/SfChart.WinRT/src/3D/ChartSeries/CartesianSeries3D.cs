#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CartesianSeries
    /// </summary>
    public abstract class CartesianSeries3D: ChartSeries3D ,ISupportAxes3D
    {
        #region Properties

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
        public ChartAxisBase3D XAxis
        {
            get { return (ChartAxisBase3D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxisBase3D), typeof(CartesianSeries3D), new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// Get or Set YAxis property
        /// </summary>
        public RangeAxisBase3D YAxis
        {
            get { return (RangeAxisBase3D)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(RangeAxisBase3D), typeof(CartesianSeries3D), new PropertyMetadata(null, OnYAxisChanged));

        /// <summary>
        /// Gets actual series X-axis.
        /// </summary>
        /// <remarks>
        /// Gets actual XAxis for series with respect to chart type and <see cref="ChartSeriesBase.IsRotated" /> value.
        /// </remarks>
        ChartAxis ISupportAxes.ActualXAxis 
        { 
            get { return ActualXAxis; }
        }

        /// <summary>
        /// Gets actual series Y-axis.
        /// </summary>
        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }

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
            DependencyProperty.Register("IsTransposed", typeof(bool), typeof(CartesianSeries3D), new PropertyMetadata(false, OnTransposeChanged));

        private static void OnTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianSeries3D).OnTransposeChanged((bool)e.NewValue);
        }

        #endregion

        #region methods

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CartesianSeries3D) d).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CartesianSeries3D) d).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
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
                newAxis.Orientation = Orientation.Vertical;
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
                newAxis.Orientation = IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
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
                newAxis.Orientation = Orientation.Horizontal;
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

            foreach (var segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }
            if (IsSideBySide)
            {
                if (SideBySideInfoRangePad != null && !SideBySideInfoRangePad.IsEmpty)
                {
                    bool isAlterRange = ((this.ActualXAxis is NumericalAxis3D && (this.ActualXAxis as NumericalAxis3D).RangePadding == NumericalPadding.None)
                   || (this.ActualXAxis is DateTimeAxis3D && (this.ActualXAxis as DateTimeAxis3D).RangePadding == DateTimeRangePadding.None));
                    XRange = isAlterRange ? new DoubleRange(XRange.Start - SideBySideInfoRangePad.Start, XRange.End - SideBySideInfoRangePad.End)
                        : new DoubleRange(XRange.Start + SideBySideInfoRangePad.Start, XRange.End + SideBySideInfoRangePad.End);
                }
            }
        }
        #endregion
    }
}
