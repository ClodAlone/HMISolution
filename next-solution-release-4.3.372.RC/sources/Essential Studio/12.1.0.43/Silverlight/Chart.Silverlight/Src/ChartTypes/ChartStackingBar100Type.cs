#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
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
    /// Class implementation for ChartStackingBar100Type
    /// </summary>
    public class ChartStackingBar100Type : ChartBarType
    {
        #region Implementation
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
                series.Area.PrimaryAxis.Orientation = Orientation.Vertical;
                series.Area.SecondaryAxis.Orientation = Orientation.Horizontal;
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
                    double y1 = points[i].X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                    double y2 = points[i].X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                    DoubleRange xRange = ChartStackingColumn100Type.GetPercentageStackInfo(series, points[i], i);
                    ChartPoint cdpBottomLeft = new ChartPoint(xRange.Start + sbsInfo.Start - sbsCenter + (series.YAxis.VisibleRange.Start * (-1)), y1);
                    ChartPoint cdpRightTop = new ChartPoint(xRange.End + sbsInfo.End - sbsCenter + (series.YAxis.VisibleRange.Start * (-1)), y2);
                    if (!double.IsNaN(points[i].Y))
                        series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));

                    double yValue = (cdpBottomLeft.X + cdpRightTop.X) / 2;
                    if (series.AdornmentsInfo != null)
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            yValue = cdpRightTop.X - sbsInfo.End * 4;
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            yValue = cdpBottomLeft.X + sbsInfo.End * 4;
                    }

                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                        if (!double.IsNaN(points[i].Y))
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, ((sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2))); 
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "StackingBar100";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            Update(series);
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
