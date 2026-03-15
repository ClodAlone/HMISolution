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
    public class NumericalAxis:RangeAxisBase
    {
        #region properties

        // Summary:
        //     Gets or sets a NumericalPadding that describes the padding of a NumericalAxis.
        //
        // Returns:
        //     The NumericalPadding that is used to set the padding of the NumericalAxis. 
        //     The default is Syncfusion.UI.Xaml.Charts.NumericalPadding.Auto.
        private NumericalPadding actualRangePadding;
        internal NumericalPadding ActualRangePadding
        {
            get
            {
                SfChart sfArea = Area as SfChart;
                if (RangePadding == NumericalPadding.Auto && sfArea != null && sfArea.Series != null && sfArea.Series.Count > 0)
                {
                    if ((Orientation == Orientation.Vertical && !sfArea.Series[0].IsActualTransposed)||
                        (Orientation == Orientation.Horizontal && sfArea.Series[0].IsActualTransposed))
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
            get { return (object)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(NumericalAxis), new PropertyMetadata(null, OnIntervalChanged));
        /// <summary>
        /// Get or Set minimun property
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
            DependencyProperty.Register("Minimum", typeof(object), typeof(NumericalAxis), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// Get or Set Maximun property
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
            DependencyProperty.Register("Maximum", typeof(object), typeof(NumericalAxis), new PropertyMetadata(null, OnMaximumChanged));
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
            DependencyProperty.Register("Interval", typeof(double?), typeof(NumericalAxis), new PropertyMetadata(null, OnIntervalChanged));

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
            DependencyProperty.Register("Minimum", typeof(double?), typeof(NumericalAxis), new PropertyMetadata(null, OnMinimumChanged));

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
            DependencyProperty.Register("Maximum", typeof(double?), typeof(NumericalAxis), new PropertyMetadata(null, OnMaximumChanged));
#endif
        public NumericalPadding RangePadding
        {
            get { return (NumericalPadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register("RangePadding", typeof(NumericalPadding), typeof(NumericalAxis), new PropertyMetadata(NumericalPadding.Auto,OnPropertyChanged));

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
            DependencyProperty.Register("StartRangeFromZero", typeof(bool), typeof(NumericalAxis), new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnPropertyChanged();
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
            (d as NumericalAxis).OnMinimumChanged(e);
        }
        /// <summary>
        /// Called Maximum property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnMaximumChanged(e);
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
            (d as NumericalAxis).OnIntervalChanged(e);
        }
        /// <summary>
        /// Called when interval changed
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
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? ActualRange.Start+1 : range.End);
                else if(Maximum!=null)
                    return new DoubleRange(double.IsNaN(range.Start) ? ActualRange.End-1 : range.Start, ActualRange.End);
                return range;
            }
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            base.CalculateVisibleRange(avalableSize);
            NumericalAxisHelper.CalculateVisibleRange(this, avalableSize, Interval);
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (Minimum == null && Maximum == null)  //Executes when Minimum and Maximum aren't set.
               return NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, ActualRangePadding, RangePaddingFactor);
            else if (Minimum != null && Maximum != null)  //Executes when Minimum and Maximum are set.
                return range;
            else
            {
                //Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval,  ActualRangePadding, RangePaddingFactor);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            obj = new NumericalAxis();
            (obj as NumericalAxis).Minimum = this.Minimum;
            (obj as NumericalAxis).Maximum = this.Maximum;
            (obj as NumericalAxis).StartRangeFromZero = this.StartRangeFromZero;
            (obj as NumericalAxis).Interval = this.Interval;
            return base.CloneAxis(obj);
        }

        #endregion
    }
}
