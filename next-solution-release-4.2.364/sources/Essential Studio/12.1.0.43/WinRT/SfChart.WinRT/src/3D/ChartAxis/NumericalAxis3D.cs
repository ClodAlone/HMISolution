#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for RangeAxisBase
    /// </summary>
    public class NumericalAxis3D:RangeAxisBase3D
    {
        #region properties

        // Summary:
        //     Gets or sets a NumericalPadding that describes the padding of a NumericalAxis3D.
        //
        // Returns:
        //     The NumericalPadding that is used to set the padding of the NumericalAxis3D. 
        //     The default is Syncfusion.UI.Xaml.Charts.NumericalPadding.Auto.
        private NumericalPadding actualRangePadding;
        internal NumericalPadding ActualRangePadding
        {
            get
            {
                SfChart3D sfArea3D = Area as SfChart3D;
                if (RangePadding == NumericalPadding.Auto && sfArea3D != null && sfArea3D.Series != null && (Area as SfChart3D).Series.Count > 0)
                {
                    if ((Orientation == Orientation.Vertical && !sfArea3D.Series[0].IsActualTransposed) ||
                        (Orientation == Orientation.Horizontal && sfArea3D.Series[0].IsActualTransposed))
                        return NumericalPadding.Round;
                }
                return (NumericalPadding)GetValue(RangePaddingProperty);
            }
            set
            {
                actualRangePadding = value;
            }
        }

#if NETFX_CORE
        /// <summary>
        /// Get or Set IntervalProperty
        /// </summary>
        public object Interval
        {
            get { return GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(NumericalAxis3D), new PropertyMetadata(null, OnIntervalChanged));
        /// <summary>
        /// Get or Set minimum property
        /// </summary>
        public object Minimum
        {
            get { return GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(object), typeof(NumericalAxis3D), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        public object Maximum
        {
            get { return GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(object), typeof(NumericalAxis3D), new PropertyMetadata(null, OnMaximumChanged));
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
            DependencyProperty.Register("Interval", typeof(double?), typeof(NumericalAxis3D), new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// Get or Set Minimum property
        /// </summary>
        public double? Minimum
        {
            get { return (double?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double?), typeof(NumericalAxis3D), new PropertyMetadata(null, OnMinimumChanged));

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
            DependencyProperty.Register("Maximum", typeof(double?), typeof(NumericalAxis3D), new PropertyMetadata(null, OnMaximumChanged));
#endif
        public NumericalPadding RangePadding
        {
            get { return (NumericalPadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register("RangePadding", typeof(NumericalPadding), typeof(NumericalAxis3D), new PropertyMetadata(NumericalPadding.Auto, OnPropertyChanged));

        public int RangePaddingFactor
        {
            get { return (int)GetValue(RangePaddingFactorProperty); }
            set { SetValue(RangePaddingFactorProperty, value); }
        }

        public static readonly DependencyProperty RangePaddingFactorProperty =
            DependencyProperty.Register("RangePaddingFactor", typeof(int), typeof(NumericalAxis), new PropertyMetadata(20, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets a value that indicates whether to start range from zero when IsAutoSetRange is enabled.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool StartRangeFromZero
        {
            get { return (bool)GetValue(StartRangeFromZeroProperty); }
            set { SetValue(StartRangeFromZeroProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StartRangeFromZero.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartRangeFromZeroProperty =
            DependencyProperty.Register("StartRangeFromZero", typeof(bool), typeof(NumericalAxis3D), new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericalAxis3D = d as NumericalAxis3D;
            if (numericalAxis3D != null) numericalAxis3D.OnPropertyChanged();
        }

        #endregion

        #region methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            if (Minimum != null && Maximum != null && Interval != null)
            {
                NumericalAxisHelper.GenerateVisibleLabels(this, SmallTicksPerInterval);
            }
            else
                base.GenerateVisibleLabels();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((NumericalAxis3D) d).OnMinimumChanged(e);
        }
        /// <summary>
        /// Called Maximum property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericalAxis3D) d).OnMaximumChanged(e);
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
        /// called when Minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        private void OnMinMaxChanged()
        {
            NumericalAxisHelper.OnMinMaxChanged(this, Maximum, Minimum);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericalAxis3D) d).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when interval changed
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
                return base.CalculateNiceInterval(range, availableSize);
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
            if (ActualRange.IsEmpty) //Executes when Minimum and Maximum aren't set.
                return base.CalculateActualRange();
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return ActualRange;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (StartRangeFromZero && range.Start > 0)
                    return new DoubleRange(0, range.End);
                else if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? double.PositiveInfinity : range.End);
                else if (Maximum != null)
                    return new DoubleRange(double.IsNaN(range.Start) ? double.NegativeInfinity : range.Start, ActualRange.End);
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
               return NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, ActualRangePadding, RangePaddingFactor);
            else if (Minimum != null && Maximum != null) //Executes when Minimum and Maximum are set.
                return range;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, ActualRangePadding, RangePaddingFactor);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new NumericalAxis();
            (obj as NumericalAxis).Minimum = Minimum;
            (obj as NumericalAxis).Maximum = Maximum;
            (obj as NumericalAxis).StartRangeFromZero = StartRangeFromZero;
            (obj as NumericalAxis).Interval = Interval;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
