#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Globalization;
#if WINDOWS_PHONE
using System.Windows;
using System;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using System;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DateTimeAxis3D
    /// </summary>
    public class DateTimeAxis3D : RangeAxisBase3D
    {
        #region properties

#if !WPF
        
        /// <summary>
        /// Get or Set Minimum property
        /// </summary>
        public object Minimum
        {
            get { return GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(object), typeof(DateTimeAxis3D), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        public object Maximum
        {
            get { return GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(object), typeof(DateTimeAxis3D), new PropertyMetadata(null, OnMaximumChanged));
#else
        

        /// <summary>
        /// Get or Set Minimum property
        /// </summary>
        public DateTime? Minimum
        {
            get { return (DateTime?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(DateTime?), typeof(DateTimeAxis3D), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        public DateTime? Maximum
        {
            get { return (DateTime?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(DateTime?), typeof(DateTimeAxis3D), new PropertyMetadata(null, OnMaximumChanged));

#endif

        /// <summary>
        /// Get or Set IntervalProperty
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(DateTimeAxis3D), new PropertyMetadata(0d, OnIntervalChanged));

        public DateTimeRangePadding RangePadding
        {
            get { return (DateTimeRangePadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register("RangePadding", typeof(DateTimeRangePadding), typeof(DateTimeAxis3D), new PropertyMetadata(DateTimeRangePadding.None, OnRangePaddingChanged));

        private static void OnRangePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue != null)
                ((DateTimeAxis3D) d).OnPropertyChanged();

        }



        internal DateTimeIntervalType intervalType
        {
            get;
            set;
        }

        public DateTimeIntervalType IntervalType
        {
            get { return (DateTimeIntervalType)GetValue(IntervalTypeProperty); }
            set { SetValue(IntervalTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register("IntervalType", typeof(DateTimeIntervalType), typeof(DateTimeAxis3D), new PropertyMetadata(DateTimeIntervalType.Auto, OnIntervalTypeChanged));

        private static void OnIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as DateTimeAxis3D) != null && e.NewValue != null)
            {
                (d as DateTimeAxis3D).intervalType = (d as DateTimeAxis3D).IntervalType;
            }
        }

        #endregion

        #region methods

        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {

            var dateTimeMin = actualRange.Start.FromOADate();
            var dateTimeMax = actualRange.End.FromOADate();
            var timeSpan = dateTimeMax.Subtract(dateTimeMin);

            var range = new DoubleRange(0, timeSpan.TotalDays / 365);

            var interval = base.CalculateNiceInterval(range, availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Years;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Months;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Days;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Hours;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Minutes;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);

            if (interval >= 1)
            {
                intervalType = DateTimeIntervalType.Seconds;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);

            intervalType = DateTimeIntervalType.Milliseconds;
            return interval;
        }

        /// <summary>
        /// Method implementation for Create VisibleLabels for DateTime axis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            DateTimeAxisHelper.GenerateVisibleLabels(this, intervalType);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D) d).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D) d).OnMaximumChanged(e);
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
        /// Called when minimum property Changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        private void OnMinMaxChanged()
        {
            DateTimeAxisHelper.OnMinMaxChanged(this, Minimum, Maximum);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D) d).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when Interval property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Area != null)
                Area.ScheduleUpdate();
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (Interval == 0 || double.IsNaN(Interval))
                return CalculateNiceInterval(range, availableSize);
            else if (IntervalType == DateTimeIntervalType.Auto)
                CalculateNiceInterval(range, availableSize);
            return Interval;
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (ActualRange.IsEmpty) //Executes when Minimum and Maximum aren't set.
                return base.CalculateActualRange();
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return ActualRange;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (Minimum != null)
                    return  new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? DateTime.MaxValue.ToOADate() : range.End);
                else if(Maximum!=null)
                    return new DoubleRange(double.IsNaN(range.Start) ? DateTime.MinValue.ToOADate() : range.Start, ActualRange.End);
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
                return DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, intervalType);
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return range;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, intervalType);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Return object value from the given double value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            return position.FromOADate().ToString(LabelFormat, CultureInfo.CurrentCulture);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new DateTimeAxis3D();
            (obj as DateTimeAxis3D).Interval = Interval;
            (obj as DateTimeAxis3D).Minimum = Minimum;
            (obj as DateTimeAxis3D).Maximum = Maximum;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
