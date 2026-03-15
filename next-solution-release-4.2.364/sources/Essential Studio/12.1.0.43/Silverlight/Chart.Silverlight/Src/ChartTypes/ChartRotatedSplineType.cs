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
    /// class implentation for ChartRotatedSplineType
    /// </summary>
    public class ChartRotatedSplineType : ChartSplineType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            series.Area.PrimaryAxis.Orientation = Orientation.Vertical;
            series.Area.SecondaryAxis.Orientation = Orientation.Horizontal;
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            bool dotSegmentRequired = ChartSplineType.GetBreakLineForNonIndexedData(series) && !series.IsIndexed;
            double gapCount = 0;
            if (dotSegmentRequired)
            {
                if (series.XAxis.ValueType == ChartValueType.Double)
                {
                    gapCount = ChartSplineType.GetBreakLineForDoublePointsDistanceMoreThan(series);
                }
                else
                {
                    gapCount = (DateTime.Now + (TimeSpan)ChartSplineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(series)).ToOADate() - DateTime.Now.ToOADate();
                }
            }

            dotSegmentRequired = dotSegmentRequired && (gapCount > 0);
            double c1 = GetSplineCoefficient(series);

            double[] yCoef;
            if (points.Count >= 2)
            {
                this.NaturalSpline(points, out yCoef);
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                    series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[0].X - 0.5, points[0].Y) : points[0], points, series, 0d));
                }

                for (int i = 1, count = points.Count; i < count; i++)
                {
                    if (points[i].EmptyPoint && series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointValue == EmptyPointValue.Zero)
                        {
                            points[i].Y = 0;
                        }
                        else
                        {
                            if (i + 1 == points.Count)
                                points[i].Y = points[i - 1].Y / 2;
                            else
                            {
                                int index;
                                for (index = i + 1; index < points.Count; index++)
                                    if (!double.IsNaN(points[index].Y))
                                        break;
                                if (i == 0)
                                    points[i].Y = (index == points.Count ? 40 : points[index].Y) / 2;
                                else
                                    points[i].Y = points[i - 1].Y / 2 + (index == points.Count ? 40 : points[index].Y) / 2;
                            }
                        }

                    }
                    if (points[i - 1].Y.Equals(double.NaN) || points[i].Y.Equals(double.NaN) || points[i - 1].X.Equals(double.NaN) || points[i].X.Equals(double.NaN))
                        continue;
                    ChartPoint startPoint = points[i - 1];
                    ChartPoint endPoint = points[i];
                    ChartPoint startControlPoint = null;
                    ChartPoint endControlPoint = null;
                    GetBezierControlPoints(startPoint, endPoint, yCoef[i - 1], yCoef[i], out startControlPoint, out endControlPoint);
                    ChartPoint point1 = new ChartPoint(startPoint.Y, startPoint.X);
                    ChartPoint point2 = new ChartPoint(endPoint.Y, endPoint.X);
                    ChartPoint point3 = new ChartPoint(startControlPoint.Y, startControlPoint.X);
                    ChartPoint point4 = new ChartPoint(endControlPoint.Y, endControlPoint.X);

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        point1.Y -= 0.5;
                        point2.Y -= 0.5;
                        point3.Y -= 0.5;
                        point4.Y -= 0.5;
                    }

                    if (points[i].EmptyPoint && series.ShowEmptyPoints == true)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        {
                            series.Segments.Add(new ScatterSegment(new ChartPoint(points[i].Y, points[i].X), new ChartPoint(points[i].Y, points[i].X), points[i], series));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartSplineSegment(point1, point2, point3, point4, points[i - 1], points[i], series));
                        }
                    }
                    else
                    {
                        if (dotSegmentRequired)
                        {
                            if (gapCount >= points[i].X - points[i - 1].X)
                            {
                                series.Segments.Add(new ChartSplineSegment(point1, point2, point3, point4, points[i - 1], points[i], series));
                            }
                        }
                        else
                        {
                            series.Segments.Add(new ChartSplineSegment(point1, point2, point3, point4, points[i - 1], points[i], series));
                            if ((points[i - 1].EmptyPoint || points[i].EmptyPoint) && (series.ShowEmptyPoints) && series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                            }
                            else if ((points[i - 1].EmptyPoint || points[i].EmptyPoint) && !(series.ShowEmptyPoints))
                            {
                                series.Segments[series.Segments.Count - 1].Interior = new SolidColorBrush(Colors.Transparent);
                            }
                            else if ((points[i - 1].EmptyPoint || points[i].EmptyPoint) && (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior))
                            {
                                series.Segments[series.Segments.Count - 1].Interior = new SolidColorBrush(Colors.Transparent);
                            }
                        }
                    }

                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                        if (series.Area.Host == Host.OLAPChart)
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].X - 0.5, points[i].Y), points, series, 0d));
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                        }
                    }
                }
            }
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
