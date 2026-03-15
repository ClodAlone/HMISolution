#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for StackingSeriesBase3D
    /// </summary>
    public abstract class StackingSeriesBase3D : XyDataSeries3D
    {
        #region Properties

        internal bool StackValueCalculated;

        /// <summary>
        /// Get or Set GroupingLabel for Stacking Series.
        /// </summary>
        public string GroupingLabel
        {
            get { return (string)GetValue(GroupingLabelProperty); }
            set { SetValue(GroupingLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StackingGroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupingLabelProperty =
            DependencyProperty.Register("GroupingLabel", typeof(string), typeof(StackingSeriesBase3D), new PropertyMetadata(null, OnGroupingLabelChanged));

        private static void OnGroupingLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StackingSeriesBase3D series = (d as StackingSeriesBase3D);
            if (series != null && series.Area != null && series.ActualArea != null)
            {
                series.ActualArea.SBSInfoCalculated = false;
                series.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Get or Set YRangeStartValues property
        /// </summary>
        protected internal IList<double> YRangeStartValues { get; set; }
        /// <summary>
        /// Get or Set YRangeEndvalues property
        /// </summary>
        protected internal IList<double> YRangeEndValues { get; set; }

        #endregion

        #region methods
        /// <summary>
        /// Return double value from the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected double GetStackedYValue(int index)
        {
             return YRangeEndValues[index];
        }

        /// <summary>
        /// Finds the nearest point in ChartSeries relative to the mouse point/touch position.
        /// </summary>
        /// <param name="point">The co-ordinate point representing the current mouse point /touch position.</param>
        /// <param name="x">x-value of the nearest point.</param>
        /// <param name="y">y-value of the nearest point</param>
        /// <param name="stackedYValue"></param>
        public override void FindNearestChartPoint(Point point, out double x, out double y, out double stackedYValue)
        {
            base.FindNearestChartPoint(point, out x, out y, out stackedYValue);
            if (double.IsNaN(x) || double.IsNaN(y)) return;
            if (ActualXValues is IList<double> && !IsIndexed)
            {
                var xValues = ActualXValues as IList<double>;
                stackedYValue = GetStackedYValue(xValues.IndexOf(x));
            }
            else
                stackedYValue = GetStackedYValue((int)x);
        }

        /// <summary>
        /// Returns the stacked value of the series.
        /// </summary>
        /// <param name="series">ChartSeries</param>
        /// <returns>StackedYValues class instance</returns>
        [ClassReference(IsReviewed = false)]
        public StackingValues GetCumulativeStackValues(ChartSeriesBase series)
        {
            if (Area.StackedValues == null || !Area.StackedValues.Keys.Contains(series))
                CalculateStackingValues();
            return Area.StackedValues != null && Area.StackedValues.Keys.Contains(series) ? Area.StackedValues[series] : null;
        }

        private void CalculateStackingValues()
        {
            Area.StackedValues = new Dictionary<object, StackingValues>();
            var stackingSeries = from series in Area.VisibleSeries
                where series is StackingSeriesBase3D
                select series;

            var enumerable = stackingSeries as ChartSeriesBase[] ?? stackingSeries.ToArray();
            var i = 0;
            foreach (var series in enumerable)
            {
                //To split the series into groups according to their labels.
                 var seriesGroups =
                      from seriesGroup in series.ActualYAxis.RegisteredSeries
                      group seriesGroup by (seriesGroup as StackingSeriesBase3D).GroupingLabel into groups
                      select new { GroupingPath = groups.Key,  Series = groups };
                 foreach (var label in seriesGroups)
                 {
                     bool reCalculation = true;
                     var lastValPosition = new List<double>();
                     var lastValNeg = new List<double>();
                     foreach (ChartSeriesBase chartSeries in label.Series)
                     {
                         if (!(chartSeries is StackingSeriesBase3D)) continue;
                         if (!chartSeries.IsSeriesVisible) continue;
                         if (((StackingSeriesBase3D)chartSeries).StackValueCalculated) break;

                         var values = new StackingValues { StartValues = new List<double>(), EndValues = new List<double>() };
                         var yValues = ((XyDataSeries3D)chartSeries).YValues;
                         var j = 0;
                         foreach (var yValue in yValues)
                         {
                             double lastValue;
                             var currentValue = double.IsNaN(yValue) ? 0 : yValue;

                             if (lastValPosition.Count <= j)
                                 lastValPosition.Add(0);
                             if (lastValNeg.Count <= j)
                                 lastValNeg.Add(0);
                             if (values.StartValues.Count <= j)
                             {
                                 values.StartValues.Add(0);
                                 values.EndValues.Add(0);
                             }

                             if (currentValue >= 0)
                             {
                                 lastValue = lastValPosition[j];
                                 if (chartSeries.GetType().Name.Contains("100Series"))
                                     currentValue =
                                         Area.GetPercentByIndex(
                                             (label.Series as IList<ISupportAxes>).OfType<StackingSeriesBase3D>().ToList(), j,
                                             currentValue, reCalculation);
                                 lastValPosition[j] += currentValue;
                             }
                             else
                             {
                                 lastValue = lastValNeg[j];
                                 if (chartSeries.GetType().Name.Contains("100Series"))
                                     currentValue =
                                         Area.GetPercentByIndex(
                                             (label.Series as IList<ISupportAxes>).OfType<StackingSeriesBase3D>().ToList(), j,
                                             currentValue, reCalculation);
                                 lastValNeg[j] += currentValue;
                             }
                             values.StartValues[j] = lastValue;
                             values.EndValues[j] = currentValue + lastValue;
                             j++;
                         }
                         i++;
                         Area.StackedValues.Add(chartSeries, values);
                         ((StackingSeriesBase3D)chartSeries).StackValueCalculated = true;
                         reCalculation = false;
                     }
                 }
            }
            foreach (var chartSeries in enumerable.OfType<StackingSeriesBase3D>())
            {
                (chartSeries).StackValueCalculated = false;
            }
        }
        #endregion
    }
}
