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
    /// Class implementation for DateTimeAxis
    /// </summary>
    public class DateTimeAxis : RangeAxisBase
    {

        internal double TotalnonWorkingHourse;
        private double nonWorkingHoursPerDay ;
        private List<string> nonWorkingDays;

        #region properties
        
        internal string InternalWorkingDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable business hours].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable business hours]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBusinessHours
        {
            get { return (bool)GetValue(EnableBusinessHoursProperty); }
            set { SetValue(EnableBusinessHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableBusinessHours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableBusinessHoursProperty =
            DependencyProperty.Register("EnableBusinessHours", typeof(bool), typeof(DateTimeAxis), new PropertyMetadata(false, OnEnableBusinessHoursChanged));

        private static void OnEnableBusinessHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as DateTimeAxis).OnEnableBusinessHoursChanged((bool)args.NewValue);
            
        }

        private void OnEnableBusinessHoursChanged(bool value)
        {
            if (value)
            {
                ValueToCoefficientCalc = ValueToBusinesshoursCoefficient;
                CoefficientToValueCalc = CoefficientToBusinesshoursValue;
            }
            else
            {
                ValueToCoefficientCalc = ValueToCoefficient;
                CoefficientToValueCalc = CoefficientToValue;
            }
            if (Area != null)
                Area.ScheduleUpdate();
        }

        /// <summary>
        /// Gets or sets the open working time.
        /// </summary>
        /// <value>
        /// The open working time.
        /// </value>
        public double OpenTime
        {
            get { return (double)GetValue(OpenTimeProperty); }
            set { SetValue(OpenTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenWorkingTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenTimeProperty =
            DependencyProperty.Register("OpenTime", typeof(double), typeof(DateTimeAxis), new PropertyMetadata(9d));

        /// <summary>
        /// Gets or sets the close working time.
        /// </summary>
        /// <value>
        /// The close working time.
        /// </value>
        public double CloseTime
        {
            get { return (double)GetValue(CloseTimeProperty); }
            set { SetValue(CloseTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseTimeProperty =
            DependencyProperty.Register("CloseTime", typeof(double), typeof(DateTimeAxis), new PropertyMetadata(6d));


        /// <summary>
        /// Gets or sets the work days.
        /// </summary>
        /// <value>
        /// The work days.
        /// </value>
        public Day WorkingDays
        {
            get { return (Day)GetValue(WorkingDaysProperty); }
            set { SetValue(WorkingDaysProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkingDays.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkingDaysProperty =
            DependencyProperty.Register("WorkingDays", typeof(Day), typeof(DateTimeAxis), new PropertyMetadata(Day.Monday | Day.Tuesday | Day.Wednesday | Day.Thursday | Day.Friday, OnWorkDaysChanged));

        private static void OnWorkDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((DateTimeAxis) d).InternalWorkingDays = args.NewValue.ToString();
        }

        private void CalculateNonWorkingDays(DoubleRange range)
        {
            var days = new string[] {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
            nonWorkingDays=new List<string>();
            if (double.IsNaN(range.Start)) return;
            var startDate = range.Start.FromOADate();
            var endDate = range.End.FromOADate();
            InternalWorkingDays = WorkingDays.ToString();
            foreach (var day in days)
            {
                if (!InternalWorkingDays.Contains(day))
                {
                    nonWorkingDays.Add(day);
                }
            }
            nonWorkingHoursPerDay = 24 - (CloseTime - OpenTime);
            TotalnonWorkingHourse = CalcNonWorkingHours(startDate, endDate, InternalWorkingDays, nonWorkingHoursPerDay);
            
        }

        private double CoefficientToBusinesshoursValue(double value)
        {
            var diff = CloseTime - OpenTime;
            double result;
            var start = VisibleRange.Start;
            var end = VisibleRange.End - (TotalnonWorkingHourse / 24);
            var delta = end - start;
            var eachCoeff = 1 / ((delta * 24) / diff);
            var val = 0;
            double weekEndCount = 0;
            value = IsInversed ? 1d - value : value;
            result = start + delta * value;
            if (value <= eachCoeff) return result;
            var startDate = start.FromOADate();
            var date = startDate.AddDays(Math.Floor(value / eachCoeff));
            for (var i = startDate; i <= date; i = i.AddDays(1))
            {
                if (i.DayOfWeek.ToString() == nonWorkingDays[0])
                {
                    weekEndCount++;
                }
            }
            var count = weekEndCount * 2;
            weekEndCount = 0;
            for (var i = startDate; i <= date.AddDays(count); i = i.AddDays(1))
            {
                if (i.DayOfWeek.ToString() == nonWorkingDays[0])
                {
                    weekEndCount++;
                }
            }
            var nonWorkingHours = CalcNonWorkingHours(startDate, date.AddDays(weekEndCount * 2), InternalWorkingDays, nonWorkingHoursPerDay);
            for (var i = 0; i < nonWorkingDays.Count; i++)
            {
                if (date.AddDays(weekEndCount * 2).DayOfWeek.ToString() == nonWorkingDays[i])
                {
                    val += (i + 1);
                }
            }
            result = result + (nonWorkingHours + val * nonWorkingHoursPerDay) / 24;
            return result;
        }

        private double ValueToBusinesshoursCoefficient(double value)
        {
            double result = double.NaN;
            var start = VisibleRange.Start;
            var end = VisibleRange.End - (TotalnonWorkingHourse / 24);
            var delta = end - start;
            if (!double.IsNaN(value))
            {
                value -=
                    CalcNonWorkingHours(start.FromOADate(), value.FromOADate(), InternalWorkingDays,
                        nonWorkingHoursPerDay)/24;
                result = (value - start)/delta;
            }

            return isInversed ? 1d - result : result;
        }

        private double CalcNonWorkingHours(DateTime startDate, DateTime endDate, string workingDays, double nonWorkingHoursPerDay)
        {
            double totalNonWorkinghours;
            var trimStart = DateTime.MinValue;
            var trimEnd = DateTime.MinValue;
            double totalWeek = 0;
            var nonWorkingHours = 0;
            var lastWeekEndCount = 0;
            for (var i = startDate; i <= endDate.AddHours(new TimeSpan(23,59,59).TotalHours - endDate.Hour); i = i.AddDays(1))
            {
                if (i.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (i != startDate)
                    {
                        nonWorkingHours++;
                    }
                    continue;
                }
                trimStart = i;
                break;
            }
            for (var i = endDate; i >= startDate.AddHours(-startDate.Hour); i = i.AddDays(-1))
            {
                if (i.DayOfWeek != DayOfWeek.Friday)
                {
                    if (!workingDays.Contains(i.DayOfWeek.ToString()))
                    {
                        lastWeekEndCount = 1;
                    }
                    else if (i != startDate && trimStart != DateTime.MinValue)
                    {
                        nonWorkingHours++;
                    }
                    continue;
                }
                trimEnd = i;
                break;
            }
            if (trimEnd != DateTime.MinValue && trimStart != DateTime.MinValue)
            {
                totalWeek =
                    Math.Round((double.IsNegativeInfinity(trimEnd.ToOADate() - trimStart.ToOADate())
                        ? 0
                        : trimEnd.ToOADate() - trimStart.ToOADate())/7);
            }
         

            totalNonWorkinghours = ((totalWeek*nonWorkingDays.Count)*24 + (lastWeekEndCount*(nonWorkingDays.Count*24)) +
                                    (totalWeek * (7 - nonWorkingDays.Count)) * nonWorkingHoursPerDay +
                                    nonWorkingHours*nonWorkingHoursPerDay);
            return totalNonWorkinghours;
        }

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
            DependencyProperty.Register("Minimum", typeof(object), typeof(DateTimeAxis), new PropertyMetadata(null, OnMinimumChanged));

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
            DependencyProperty.Register("Maximum", typeof(object), typeof(DateTimeAxis), new PropertyMetadata(null, OnMaximumChanged));
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
            DependencyProperty.Register("Minimum", typeof(DateTime?), typeof(DateTimeAxis), new PropertyMetadata(null, OnMinimumChanged));

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
            DependencyProperty.Register("Maximum", typeof(DateTime?), typeof(DateTimeAxis), new PropertyMetadata(null, OnMaximumChanged));

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
            DependencyProperty.Register("Interval", typeof(double), typeof(DateTimeAxis), new PropertyMetadata(0d, OnIntervalChanged));

        public DateTimeRangePadding RangePadding
        {
            get { return (DateTimeRangePadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register("RangePadding", typeof(DateTimeRangePadding), typeof(DateTimeAxis), new PropertyMetadata(DateTimeRangePadding.Auto, new PropertyChangedCallback(OnRangePaddingChanged)));

        private static void OnRangePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue != null)
                (d as DateTimeAxis).OnPropertyChanged();

        }



        internal DateTimeIntervalType ActualIntervalType
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
            DependencyProperty.Register("IntervalType", typeof(DateTimeIntervalType), typeof(DateTimeAxis), new PropertyMetadata(DateTimeIntervalType.Auto, new PropertyChangedCallback(OnIntervalTypeChanged)));

        private static void OnIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as DateTimeAxis) != null && e.NewValue!=null)
            {
                (d as DateTimeAxis).ActualIntervalType = (d as DateTimeAxis).IntervalType;
                (d as DateTimeAxis).OnPropertyChanged();
            }
        }

        #endregion

        #region methods

        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {

            DateTime dateTimeMin = actualRange.Start.FromOADate();
            DateTime dateTimeMax = actualRange.End.FromOADate();
            TimeSpan timeSpan = dateTimeMax.Subtract(dateTimeMin);

            DoubleRange range = new DoubleRange(0, timeSpan.TotalDays / 365);

            double interval = base.CalculateNiceInterval(range, availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Years;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Months;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Days;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Hours;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Minutes;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);

            if (interval >= 1)
            {
                ActualIntervalType = DateTimeIntervalType.Seconds;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);

            ActualIntervalType = DateTimeIntervalType.Milliseconds;
            return interval;
        }

        /// <summary>
        /// Method implementation for Create VisibleLabels for DateTime axis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            DateTimeAxisHelper.GenerateVisibleLabels(this, ActualIntervalType);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeAxis).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeAxis).OnMaximumChanged(e);
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
            (d as DateTimeAxis).OnIntervalChanged(e);
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
            if (EnableBusinessHours)
            {
                CalculateNonWorkingDays(range);
                range = new DoubleRange(range.Start, range.End - nonWorkingHoursPerDay / 24);
            }
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
            if (ActualRange.IsEmpty) //Executes when Minimum and Maximum aren set.
                return base.CalculateActualRange();
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum aren't set.
                return ActualRange;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (Minimum != null)
                    return   new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? DateTime.MaxValue.ToOADate() : range.End);
                else if(Maximum != null)
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
                return DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, ActualIntervalType);
            else if (Minimum != null && Maximum != null) //Executes when  Minimum and Maximum are set.
                return range;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, ActualIntervalType);
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
            return position.FromOADate().ToString(this.LabelFormat, CultureInfo.CurrentCulture); ;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            base.CalculateVisibleRange(avalableSize);

            DateTimeAxisHelper.CalculateVisibleRange(this, avalableSize, Interval);
            if (EnableBusinessHours)
            {
                CalculateNonWorkingDays(VisibleRange);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new DateTimeAxis();
            (obj as DateTimeAxis).Interval = this.Interval;
            (obj as DateTimeAxis).Minimum = this.Minimum;
            (obj as DateTimeAxis).Maximum = this.Maximum;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
