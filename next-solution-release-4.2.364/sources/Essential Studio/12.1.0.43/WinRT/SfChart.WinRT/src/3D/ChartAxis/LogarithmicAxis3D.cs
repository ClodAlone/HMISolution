#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for LogarithmicAxis3D
    /// </summary>
    public class LogarithmicAxis3D : RangeAxisBase3D
    {
        #region ctor

        /// <summary>
        /// Called when instance created for LogarithmicAxis3D 
        /// </summary>
        public LogarithmicAxis3D()
        {
            IsLogarithmic = true;
        }

        #endregion

        #region properties

#if NETFX_CORE
        /// <summary>
        /// Get or Set Interval property
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
            DependencyProperty.Register("Interval", typeof(object), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// Get or Set Minimum property
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
            DependencyProperty.Register("Minimum", typeof(object), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
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
            DependencyProperty.Register("Maximum", typeof(object), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnMaximumChanged));
#else

        /// <summary>
        /// Get or Set IntervalProperty
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
            DependencyProperty.Register("Interval", typeof(double?), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// Get or Set Minimum property
        /// </summary>
        public double? Minimum
        {
            get { return (double?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double?), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        public double? Maximum
        {
            get { return (double?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double?), typeof(LogarithmicAxis3D), new PropertyMetadata(null, OnMaximumChanged));
#endif

        /// <summary>
        /// Gets or Sets logarithmic base.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double LogarithmicBase
        {
            get { return (double)GetValue(LogarithmicBaseProperty); }
            set { SetValue(LogarithmicBaseProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LogarithmicBase.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LogarithmicBaseProperty =
            DependencyProperty.Register("LogarithmicBase", typeof(double), typeof(LogarithmicAxis3D), new PropertyMetadata(10d, OnLogarithmicAxisValueChanged));
        #endregion

        #region methods

        private static void OnLogarithmicAxisValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        [ClassReference(IsReviewed = false)]
        public override double CoefficientToValue(double value)
        {
            return Math.Pow(LogarithmicBase, base.CoefficientToValueCalc(value));
        }

        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            return LogarithmicAxisHelper.CalculateNiceInterval(this, actualRange, availableSize);
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            LogarithmicAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, Interval, LogarithmicBase);
        }
        /// <summary>
        /// Method implementation for Add SmallTicks for axis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="logarithmicBase"></param>
        internal protected override void AddSmallTicksPoint(double position, double logarithmicBase)
        {
            LogarithmicAxisHelper.AddSmallTicksPoint(this, position, logarithmicBase, SmallTicksPerInterval);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D) d).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D) d).OnMaximumChanged(e);
        }
        /// <summary>
        /// Called when maximum changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }
        /// <summary>
        /// Called when minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        private void OnMinMaxChanged()
        {
            LogarithmicAxisHelper.OnMinMaxChanged(this, Minimum, Maximum, LogarithmicBase);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D) d).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when Interval changed
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
            if (Interval == null)
                return CalculateNiceInterval(range, availableSize);
#if NETFX_CORE
            return Convert.ToDouble(Interval);
#else
            return Interval.Value;
#endif
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (Minimum == null && Maximum == null) //Executes when Minimum and Maximum aren't set.
                return LogarithmicAxisHelper.CalculateActualRange(this, base.CalculateActualRange(), LogarithmicBase);
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum aren set.
                return ActualRange;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                range = LogarithmicAxisHelper.CalculateActualRange(this, range, LogarithmicBase);
                if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, range.End);
                else if (Maximum != null)
                    return new DoubleRange(range.Start, ActualRange.End);
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
                //Executes when either Minimum and Maximum is set.
                DoubleRange baseRange = base.ApplyRangePadding(range, interval);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Return the object Value from the given double value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            return Math.Round(Math.Pow(this.LogarithmicBase, Math.Log(position, LogarithmicBase)), CRoundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new LogarithmicAxis3D();
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
