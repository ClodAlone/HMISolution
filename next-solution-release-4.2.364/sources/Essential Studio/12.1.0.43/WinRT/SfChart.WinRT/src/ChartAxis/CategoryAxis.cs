#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CategoryAxis
    /// </summary>
    public class CategoryAxis: ChartAxisBase2D
    {
        #region properties

#if NETFX_CORE
        /// <summary>
        /// Gets or Sets a value that determines the interval between labels. If this property is not set, interval will be calculated automatically. 
        /// Interval must be of double type
        /// </summary>
        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(CategoryAxis), new PropertyMetadata(null, OnIntervalChanged));
               
#else
        /// <summary>
        /// Gets or Sets a value that determines the interval between labels. If this property is not set, interval will be calculated automatically.
        /// </summary>
        public double? Interval
        {
            get { return (double?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double?), typeof(CategoryAxis), new PropertyMetadata(null, OnIntervalChanged));

#endif

        public LabelPlacement LabelPlacement
        {
            get { return (LabelPlacement)GetValue(LabelPlacementProperty); }
            set { SetValue(LabelPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register("LabelPlacement", typeof(LabelPlacement), typeof(CategoryAxis), new PropertyMetadata(LabelPlacement.OnTicks, OnIntervalChanged));
        #endregion

        #region methods

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null)
            (d as CategoryAxis).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when interval property changed 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            return CategoryAxisHelper.GetLabelContent(this, position);
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            return CategoryAxisHelper.CalculateActualInterval(this, range, availableSize, Interval);
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            return CategoryAxisHelper.ApplyRangePadding(this, range, interval, LabelPlacement);
        }
        /// <summary>
        /// Method implementation for Generate Visiblie labels for CategoryAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            CategoryAxisHelper.GenerateVisibleLabels(this, LabelPlacement);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new CategoryAxis();
            (obj as CategoryAxis).Interval = this.Interval;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
