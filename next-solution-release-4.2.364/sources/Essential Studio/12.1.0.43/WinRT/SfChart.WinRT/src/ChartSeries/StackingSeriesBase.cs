#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for StackingSeriesBase
    /// </summary>
    public abstract class StackingSeriesBase:XyDataSeries
    {
        #region Properties

        internal bool stackValueCalculated;
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
            DependencyProperty.Register("GroupingLabel", typeof(string), typeof(StackingSeriesBase), new PropertyMetadata(null, OnGroupingLabelChanged));

        private static void OnGroupingLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StackingSeriesBase series = (d as StackingSeriesBase);
            if (series != null && series.Area != null && series.ActualArea!=null)
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
            if (!double.IsNaN(x) && !double.IsNaN(y))
            {
                if (this.ActualXValues is IList<double> && !this.IsIndexed)
                {
                    IList<double> xValues;
                    xValues = this.ActualXValues as IList<double>;
                    stackedYValue = this.GetStackedYValue(xValues.IndexOf(x));
                }
                else
                    stackedYValue = this.GetStackedYValue((int)x);
            }
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
            if(Area.StackedValues.Keys.Contains(series))
                return Area.StackedValues[series];
            return null;
        }

        private void CalculateStackingValues()
        {
            Area.StackedValues = new Dictionary<object, StackingValues>();
            var stackingSeries = from series in Area.VisibleSeries
                where series is StackingSeriesBase
                select series;

            foreach (var series in stackingSeries)
            {
                //To split the series into groups according to their labels.
                var seriesGroups =
                      from seriesGroup in series.ActualYAxis.RegisteredSeries
                      where seriesGroup is StackingSeriesBase
                      group seriesGroup by (seriesGroup as StackingSeriesBase).GroupingLabel into groups
                      select new { GroupingPath = groups.Key,  Series = groups };
                foreach (var label in seriesGroups)
                {
                    int i = 0;
                    var lastValPos = new List<double>();
                    var lastXValPos = new List<double>();
                    var lastValNeg = new List<double>();
                    bool reCalculation = true;
                    foreach (ChartSeriesBase chartSeries in label.Series)
                    {
                        if (!(chartSeries is StackingSeriesBase)) continue;
                        if (!chartSeries.IsSeriesVisible) continue;
                        if (((StackingSeriesBase)chartSeries).stackValueCalculated) break;

                        var values = new StackingValues();
                        values.StartValues = new List<double>();
                        values.EndValues = new List<double>();
                        var yValues = ((XyDataSeries)chartSeries).YValues;
                        var xValues = ((XyDataSeries)chartSeries).GetXValues();

                        int j = 0;
                        foreach (double yValue in yValues)
                        {
                                double lastValue = 0;
                                double currentValue = yValue;

                                if (lastValPos.Count <= j)
                                    lastValPos.Add(0);
                                if (lastValNeg.Count <= j)
                                    lastValNeg.Add(0);
                                if (lastXValPos.Count <= j)
                                    lastXValPos.Add(0);
                                if (values.StartValues.Count <= j)
                                {
                                    values.StartValues.Add(0);
                                    values.EndValues.Add(0);
                                }

                                bool checkXValues = false;
                                int xPlotIndex = 0;
                        for (int k = 0; k < lastXValPos.Count; k++)
                        {
                            if (lastXValPos[k] == xValues[j])
                            {
                                xPlotIndex = k;
                                checkXValues = true;
                                break;
                            }
                        }
                        if (checkXValues)
                        {
                            if (currentValue >= 0)
                            {
                                lastValue = lastValPos[xPlotIndex];
                                if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                    currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                lastValPos[xPlotIndex] += currentValue;
                            }
                            else
                            {
                                lastValue = lastValNeg[xPlotIndex];
                                if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                    currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                lastValNeg[xPlotIndex] += currentValue;
                            }
                        }
                        else
                        {
                            if (currentValue >= 0)
                            {
                                if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                    currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                lastValPos.Add(currentValue);
                               lastValNeg.Add(0);
                            }
                            else
                            {
                                if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                    currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                lastValPos.Add(0);
                                lastValNeg.Add(currentValue);
                            }
                            lastXValPos.Add(xValues[j]);
                         }
                            
                            values.StartValues[j] = lastValue;
                            values.EndValues[j] = currentValue + lastValue;
                            j++;
                        }
                        i++;
                        Area.StackedValues.Add(chartSeries, values);
                        ((StackingSeriesBase)chartSeries).stackValueCalculated = true;
                        reCalculation = false;
                    }
                }
                
            }
            foreach (ChartSeriesBase chartSeries in stackingSeries)
            {
                if (chartSeries is StackingSeriesBase)
                    ((StackingSeriesBase)chartSeries).stackValueCalculated = false;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class implementation for StackingValues
    /// </summary>
    public class StackingValues
    {
        /// <summary>
        /// Get or Set StartValues property
        /// </summary>
        public IList<double> StartValues { get; set; }
        /// <summary>
        /// Get or Set EndValues property
        /// </summary>
        public IList<double> EndValues { get; set; }
      
    }
}
