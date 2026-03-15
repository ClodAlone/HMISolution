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
   /// Class implementation for ChartGantttype 
   /// </summary>
    public class ChartGanttType : ChartBarType
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
                SetRange(series, points, 2);
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
                for (int i = 0; i < points.Count && points[i].Values.Length >= 2; i++)
                {
                    if (points[i].EmptyPoint && series.ShowEmptyPoints == true)
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
                    double x1 = points[i].Values[1] ;
                    double x2 = points[i].Y ;
                    double y1 = points[i].X + sbsInfo.Start - sbsCenter ;
                    double y2 = points[i].X + sbsInfo.End - sbsCenter ;
                    ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                    ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                    if (!double.IsNaN(points[i].Y) && !double.IsNaN(points[i].Values[1]))
                    {
                        series.Segments.Add(new BarSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                                      
                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                    {
                        if (points[i].Values[0] > points[i].Values[1])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] - sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 1 });
                        }   
                        
                    }
                    else if (series.AdornmentsInfo != null && series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                    {
                        if (points[i].Values[0] < points[i].Values[1])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] - sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 1 });
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {
                            ////series.Segments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                            if (!double.IsNaN(points[i].Y))
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] + sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 0 });
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2) { adornemntLabelIndex = 1 });
                            }

                        }
                    }
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
            return "Gantt";
        }

       /// <summary>
       /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
       /// </summary>
       public override void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
