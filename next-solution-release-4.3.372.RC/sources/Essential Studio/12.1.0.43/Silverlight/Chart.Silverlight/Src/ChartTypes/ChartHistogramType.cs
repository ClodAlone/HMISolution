#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartHistogramtype
    /// </summary>
    public class ChartHistogramType : ChartColumnType
    {
        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for NumberIntervals.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalOfHistogramProperty =
      DependencyProperty.RegisterAttached("IntervalOfHistogram", typeof(double), typeof(ChartHistogramType), new PropertyMetadata(1d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawNormalDistribution.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawNormalDistributionProperty =
            DependencyProperty.RegisterAttached("DrawNormalDistribution", typeof(bool), typeof(ChartHistogramType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the draw normal distribution attached property value.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Normal Distribution</returns>
        public static bool GetDrawNormalDistribution(ChartSeries series)
        {
            return (bool)series.GetValue(DrawNormalDistributionProperty);
        }

        /// <summary>
        /// Sets the draw normal distribution.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetDrawNormalDistribution(ChartSeries series, bool value)
        {
            series.SetValue(DrawNormalDistributionProperty, value);
        }

        /// <summary>
        /// Gets the interval of histogram.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The m_visibleInterval Of Histogram</returns>
        public static double GetIntervalOfHistogram(ChartSeries series)
        {
            return (double)series.GetValue(IntervalOfHistogramProperty);
        }

        /// <summary>
        /// Sets the interval of histogram.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetIntervalOfHistogram(ChartSeries series, double value)
        {
            series.SetValue(IntervalOfHistogramProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            if (series.Area != null)
            {
                ChartPointsCollection chartpoints = GetUpdatePoints(series, points);
                SetRange(series, chartpoints, 1);
                if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
                {
                    points = series.Data;
                    series.IsIndexed = false;
                }

                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
                double interval = GetIntervalOfHistogram(series);
                if (series.BindingPathsY == null && series.sum == 0)
                {
                    double[] xdata = (from point in points where !point.Y.Equals(double.NaN) orderby point.X ascending select point.X).ToArray<double>();
                    double start = (xdata.Length != 0) ? xdata[0] : 0d;
                    double count = 0d;
                    for (int i = 0; i < xdata.Length; i++)
                    {
                        while (i < xdata.Length && xdata[i] <= start + interval)
                        {
                            count++;
                            i++;
                        }

                        i--;
                        if (i < xdata.Length)
                        {
                            series.Segments.Add(new ColumnSegment(new ChartPoint(start + (series.XAxis.VisibleRange.Start * (-1)), 0), new ChartPoint(start + interval + (series.XAxis.VisibleRange.Start * (-1)), count + (series.YAxis.VisibleRange.Start * (-1))), new ChartPoint(xdata[i], 0), series));
                        }

                        start += interval;
                        count = 0d;
                    }
                }
                else
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        if (double.IsNaN(points[i].Y))
                            continue;
                        double x1 = points[i].X + (series.XAxis.VisibleRange.Start * (-1));
                        double x2 = points[i + 1].X + (series.XAxis.VisibleRange.Start * (-1));
                        double y1 = 0;
                        double y2 = (points[i].Y / (points[i + 1].X - points[i].X)) + (series.YAxis.VisibleRange.Start * (-1));
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                        series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                     
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible == true)  
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Y), points, series, sbsInfo.Median+0.1));
                            }
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, series.YAxis.VisibleRange.Start), points, series, sbsInfo.Median + 0.1));
                            }
                            else
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (series.YAxis.VisibleRange.Start + points[i].Y) /2  ), points, series, sbsInfo.Median + 0.1));
                            }
                        }
                    }
                }

                #region Normal Distribution
                if (GetDrawNormalDistribution(series))
                {
                    double m, dev;
                    GetHistogramMeanAndDeviation(chartpoints, out m, out dev);
                    ChartPointsCollection distributionPoints = new ChartPointsCollection();

                    double min = (int)(Math.Floor(chartpoints[0].X / interval)) * interval;
                    double max = series.XAxis.VisibleRange.End;
                    double del = (max - min) / 500;
                    ChartPoint prevSegPoint = null;
                    double ty = 0d;
                    for (int i = 0; i <= 500; i++)
                    {
                        double tx = min + (i * del);
                        //double ty = NormalDistribution(tx, m, dev) * points.Count * interval;
                        if (tx < series.XAxis.VisibleRange.Start + ((series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) / 2))
                            ty = ty + 1;
                        else
                            ty = ty - 1;
                        tx = tx + (series.XAxis.VisibleRange.Start * (-1));
                        //ty = (ty + (series.YAxis.VisibleRange.Start * (-1))) * series.YAxis.VisibleRange.End;                       

                        distributionPoints.Add(new ChartPoint(tx, ty));
                        if (prevSegPoint != null)
                        {
                            series.Segments.Add(new LineSegment(prevSegPoint, new ChartPoint(tx, ty), new ChartPoint(tx, ty), series));
                        }
                        prevSegPoint = new ChartPoint(tx, ty);
                    }

                }
                #endregion
            }
        }

        /// <summary>
        /// Gets the histogram mean and deviation.
        /// </summary>
        /// <param name="points">The chart points.</param>
        /// <param name="mean">The mean value.</param>
        /// <param name="standartDeviation">The standart deviation.</param>
        private static void GetHistogramMeanAndDeviation(ChartPointsCollection points, out double mean, out double standartDeviation)
        {
            int count = points.Count;
            double sum = 0;
            sum = (from point in points select point.X).Sum();
            mean = sum / count;
            double tempmean = mean;
            sum = (from point in points select ((point.X - tempmean) * (point.X - tempmean))).Sum();
            standartDeviation = Math.Sqrt(sum / count);
        }

        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="m">The m value.</param>
        /// <param name="sigma">The sigma value.</param>
        /// <returns>The Normal Distribution</returns>
        private static double NormalDistribution(double x, double m, double sigma)
        {
            return Math.Exp(-(x - m) * (x - m) / (2 * sigma * sigma)) / Math.Sqrt(2 * Math.PI * sigma * sigma);
        }

        ChartPointsCollection GetUpdatePoints(ChartSeries series, ChartPointsCollection points)
        {
            ChartPointsCollection newPoints = new ChartPointsCollection();

            series.sum = (from point in points select point.Y).Sum();
            for (int i = 0; i < points.Count-1; i++)
            {
                newPoints.Add(new ChartPoint(points[i].X, points[i].Y / (points[i + 1].X - points[i].X)));
            }

            int n=points.Count-1;
            if (points.Count >= 1)
            {
                newPoints.Add(new ChartPoint(points[n].X, (points[n].Y / (points[n].X - points[n - 1].X))));
            }

            return newPoints;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "Histogram";
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
