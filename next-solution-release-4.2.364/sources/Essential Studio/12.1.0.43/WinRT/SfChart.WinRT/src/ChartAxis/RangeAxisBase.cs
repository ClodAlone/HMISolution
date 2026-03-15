#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for RangeAxisBase
    /// </summary>
    public abstract class RangeAxisBase : ChartAxisBase2D, IRangeAxis
    {
        #region properties

        /// <summary>
        /// Gets or Sets small tick’s interval
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int SmallTicksPerInterval
        {
            get { return (int)GetValue(SmallTicksPerIntervalProperty); }
            set { SetValue(SmallTicksPerIntervalProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallTicksPerInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallTicksPerIntervalProperty =
            DependencyProperty.Register("SmallTicksPerInterval", typeof(int), typeof(ChartAxis), new PropertyMetadata(0, new PropertyChangedCallback(OnSmallTicksPerIntervalPropertyChanged)));

        /// <summary>
        /// Gets or Sets small tick line size
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double SmallTickLineSize
        {
            get { return (double)GetValue(SmallTickLineSizeProperty); }
            set { SetValue(SmallTickLineSizeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallTickLineSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallTickLineSizeProperty =
            DependencyProperty.Register("SmallTickLineSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(5d));

        /// <summary>
        /// Gets or Sets small tick lines position
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public AxisElementPosition SmallTickLinesPosition
        {
            get { return (AxisElementPosition)GetValue(SmallTickLinesPositionProperty); }
            set { SetValue(SmallTickLinesPositionProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SmallTickLinesPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallTickLinesPositionProperty =
            DependencyProperty.Register("SmallTickLinesPosition", typeof(AxisElementPosition), typeof(ChartAxis), new PropertyMetadata(AxisElementPosition.Outside));


        #endregion
        #region methods

        DoubleRange IRangeAxis.Range
        {
            get { return ActualRange; }
        }

        private static void OnSmallTicksPerIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.smallTicksRequired = (int)e.NewValue > 0 ? true : axis.smallTicksRequired;
        }
        /// <summary>
        /// Method implementation for Add SamllTicksPoint
        /// </summary>
        /// <param name="position"></param>
        internal protected override void AddSmallTicksPoint(double position)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, VisibleInterval, SmallTicksPerInterval);
        }
        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="logarithmicbase"></param>
        internal protected override void AddSmallTicksPoint(double position, double interval)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, interval, SmallTicksPerInterval);
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            RangeAxisBaseHelper.GenerateVisibleLabels(this, SmallTickLineSize);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            (obj as RangeAxisBase).SmallTicksPerInterval = this.SmallTicksPerInterval;
            (obj as RangeAxisBase).SmallTickLinesPosition = this.SmallTickLinesPosition;
            (obj as RangeAxisBase).SmallTickLineSize = this.SmallTickLineSize;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
