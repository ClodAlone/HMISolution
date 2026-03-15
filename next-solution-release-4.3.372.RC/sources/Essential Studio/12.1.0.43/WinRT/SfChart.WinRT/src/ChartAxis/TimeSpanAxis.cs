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
    /// Class implementation for TimeSpanAxis
    /// </summary>
    public class TimeSpanAxis:RangeAxisBase
    {
        #region properties


#if NETFX_CORE
        /// <summary>
        /// Get or Set intervalproperty
        /// </summary>
        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

       
       /// <summary>
        ///  Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(TimeSpanAxis), new PropertyMetadata(null, OnIntervalChanged));



        /// <summary>
        /// Get or Set MinimunProperty
        /// </summary>
        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(object), typeof(TimeSpanAxis), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set MaximunProperty
        /// </summary>
        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(object), typeof(TimeSpanAxis), new PropertyMetadata(null, OnMaximumChanged));
#else
        /// <summary>
        /// Get or Set Interval property
        /// </summary>
        public TimeSpan? Interval
        {
            get { return (TimeSpan?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan?), typeof(TimeSpanAxis), new PropertyMetadata(null, OnIntervalChanged));

        

        /// <summary>
        ///get or set  Minimum property
        /// </summary>
        public TimeSpan? Minimum
        {
            get { return (TimeSpan?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(TimeSpan?), typeof(TimeSpanAxis), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        public TimeSpan? Maximum
        {
            get { return (TimeSpan?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(TimeSpan?), typeof(TimeSpanAxis), new PropertyMetadata(null, OnMaximumChanged));
#endif

        #endregion

        #region methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            if (Minimum != null && Maximum != null && Interval != null)
            {
                TimeSpanAxisHelper.GenerateVisibleLabels(this);
            }
            else
                base.GenerateVisibleLabels();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnMaximumChanged(e);
        }
        /// <summary>
        /// Called when Maximum property changed 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }
        /// <summary>
        /// Method implementation for Minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        private void OnMinMaxChanged()
        {
            if (Minimum != null || Maximum != null)
            {
#if NETFX_CORE
                TimeSpan minTimeSpan = Minimum is TimeSpan ? (TimeSpan)Minimum : (Minimum != null ? TimeSpan.Parse(Minimum.ToString()) : TimeSpan.MinValue );
                TimeSpan maxTimeSpan = Maximum is TimeSpan ? (TimeSpan)Maximum : (Maximum != null ? TimeSpan.Parse(Maximum.ToString()) : TimeSpan.MaxValue);
                ActualRange = new DoubleRange(minTimeSpan.TotalMilliseconds, maxTimeSpan.TotalMilliseconds);
#else
                double minTimeSpan = Minimum == null ? TimeSpan.MinValue.TotalMilliseconds : Minimum.Value.TotalMilliseconds;
                double maxTimeSpan = Maximum == null ? TimeSpan.MaxValue.TotalMilliseconds : Maximum.Value.TotalMilliseconds;
                ActualRange = new DoubleRange(minTimeSpan, maxTimeSpan);
#endif
            }
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when Interval property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (Interval == null)
                return base.CalculateNiceInterval(range, availableSize);
#if NETFX_CORE
            TimeSpan timeSpan = Interval is TimeSpan ? (TimeSpan)Interval : TimeSpan.Parse(Interval.ToString());
            return timeSpan.TotalMilliseconds;
#else
            return Interval.Value.TotalMilliseconds;
#endif
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (Minimum == null && Maximum == null) //Executes when Minimum and Maximum aren't set.
                return base.CalculateActualRange();
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return ActualRange;
            else
            {
                //Executes when either Minimum or Maximum.
                DoubleRange range = base.CalculateActualRange();
                if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? TimeSpan.MaxValue.TotalMilliseconds : range.End);
                else if (Maximum != null)
                    return new DoubleRange(double.IsNaN(range.Start) ? TimeSpan.MinValue.TotalMilliseconds : range.Start, ActualRange.End);
                return range;
            }
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (Minimum == null && Maximum == null) //Executes when Minimum and Maximum aren't set.
                return base.ApplyRangePadding(range, interval);
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return range;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = base.ApplyRangePadding(range, interval);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Return Object from the given double value 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            return TimeSpan.FromMilliseconds(position).ToString(LabelFormat, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            base.CalculateVisibleRange(avalableSize);
            TimeSpanAxisHelper.CalculateVisibleRange(this, Interval, avalableSize);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new TimeSpanAxis();
            (obj as TimeSpanAxis).Interval = this.Interval;
            (obj as TimeSpanAxis).Minimum = this.Minimum;
            (obj as TimeSpanAxis).Maximum = this.Maximum;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
