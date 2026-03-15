#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartStackingColumn100Type
    /// </summary>
    public class ChartStackingColumn100Type : ChartColumnType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies ShowValueAsProbability attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValueAsProbabilityProperty =
            DependencyProperty.RegisterAttached("ShowValueAsProbability", typeof(bool), typeof(ChartStackingColumn100Type), new PropertyMetadata(false, new PropertyChangedCallback(OnShowValueAsProbabilityChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Called when [show value as probability changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowValueAsProbabilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.LoadArea();
            }
        }

        /// <summary>
        /// Gets the show value as probability.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <returns>True is the value is to be displayed as probability</returns>
        public static bool GetShowValueAsProbability(ChartArea area)
        {
            return (bool)area.GetValue(ShowValueAsProbabilityProperty);
        }

        /// <summary>
        /// Sets the show value as probability.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/>.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowValueAsProbability(ChartArea area, bool value)
        {
            area.SetValue(ShowValueAsProbabilityProperty, value);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "StackingColumn100";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            Update(series);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            if (series.Area != null)
            {
                SetRange(series, points, 1);
                DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);

                if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
                {
                    points = series.Data;
                    series.IsIndexed = false;
                }

                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
                int seriesIndex = series.Area.Series.IndexOf(series);
                bool isLower = seriesIndex == 0;
                bool isUpper = seriesIndex == series.Area.Series.Count - 1;
                for (int i = 0; i < points.Count; i++)
                {
                    double x1 = points[i].X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                    double x2 = points[i].X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)); 
                    DoubleRange yRange = GetPercentageStackInfo(series, points[i], i);
                    ChartPoint cdpBottomLeft = new ChartPoint(x1, yRange.Start + (series.YAxis.VisibleRange.Start * (-1)));
                    ChartPoint cdpRightTop = new ChartPoint(x2, yRange.End + (series.YAxis.VisibleRange.Start * (-1)));
                    if (!double.IsNaN(points[i].Y))
                        series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));

                    double yValue = (cdpBottomLeft.Y + cdpRightTop.Y)/2;
                    if (series.AdornmentsInfo != null)
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            yValue = cdpRightTop.Y - sbsInfo.End * 4;
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            yValue = cdpBottomLeft.Y + sbsInfo.End * 4;
                    }

                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                        if (!double.IsNaN(points[i].Y))
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                    }
                }
            }
        }

        internal static DoubleRange GetPercentageStackInfo(ChartSeries series, ChartPoint point, int index)
        {
            List<ChartSeries> visibleSeries = (from ser in series.Area.Series
                                               where ser.Visibility == Visibility.Visible
                                               select ser).Cast<ChartSeries>().ToList();
            int seriesIndex = visibleSeries.IndexOf(series);
            double yStart = 0;
            double yEnd = 0;
            double sum = 0;
            int coeff = ChartStackingColumn100Type.GetShowValueAsProbability(series.Area) ? 1 : 100;
            foreach (ChartSeries chartSeries in visibleSeries)
            {
                if (chartSeries.Data.Count > index)
                {
                    sum += chartSeries.Data[index].Y;
                }
            }

            yEnd = point.Y / sum * coeff;

            for (int i = seriesIndex - 1; i >= 0; i--)
            {
                ChartSeries targetSeries = visibleSeries[i];
                if (targetSeries.Data.Count > index)
                {
                    double columnShift = targetSeries.Data[index].Y / sum * coeff;
                    yStart += columnShift;
                    yEnd += columnShift;
                }
            }

            return new DoubleRange(yStart, yEnd);
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
