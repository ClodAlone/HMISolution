#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
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
    /// Class implementation for ChartStepAreaType
    /// </summary>
    public class ChartStepAreaType : ChartAreaType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            ChartPointsCollection newPoints = new ChartPointsCollection();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y))
                    newPoints.Add(points[i]);
            }
            SetRange(series, newPoints, 1);
            Polypoints.Clear();
            series.sum = (from point in newPoints select point.Y).Sum();
            for (int i = 0; i < newPoints.Count - 1; i++)
            {
                if (newPoints[i].EmptyPoint && series.ShowEmptyPoints)
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
                if (double.IsNaN(newPoints[i].Y))
                    continue;
                double x1 = newPoints[i].X + (series.XAxis.VisibleRange.Start * (-1));
                double y1 = newPoints[i].Y + (series.YAxis.VisibleRange.Start * (-1));

                //if (series.Area.Host == Host.OLAPChart)
                //{
                //    x1 -= 0.5;
                //    y1 -= 0.5;
                //}

                Polypoints.Add(new ChartPoint(x1, y1));
                ChartPoint steppoint = new ChartPoint(newPoints[i].X + (series.XAxis.VisibleRange.Start * (-1)), newPoints[i + 1].Y + (series.YAxis.VisibleRange.Start * (-1)));
                //if (series.Area.Host == Host.OLAPChart )
                //{
                //    steppoint.X -= 0.5;
                //    steppoint.Y -= 0.5;
                //}

                Polypoints.Add(steppoint);


                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                    series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(newPoints[i].X - 0.5, newPoints[i].Y) : newPoints[i], newPoints, series, 0d));
                }
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                series.Adornments.Add(new ChartAdornment(newPoints[newPoints.Count - 1], newPoints, series, 0d));
            }

            if (series.IsIndexed == false)
            {
                sordpoints();
            }


            Polypoints.Add(new ChartPoint(newPoints[newPoints.Count - 1].X + (series.XAxis.VisibleRange.Start * (-1)), newPoints[newPoints.Count - 1].Y + (series.YAxis.VisibleRange.Start * (-1))));
            if (series.Area.Host == Host.OLAPChart)
            {
                Polypoints[0].X -= 1;
                Polypoints.Insert(1, new ChartPoint(1, Polypoints[0].Y));
            }
            double yValue = 0;
            yValue = series.XAxis.Origin - (series.YAxis.VisibleRange.Start + series.YAxis.LineStrokeThickness / 2);
            Polypoints.Insert(0, new ChartPoint(Polypoints[0].X,  yValue));
            Polypoints.Add(new ChartPoint(Polypoints[Polypoints.Count - 1].X,  yValue));
            series.Segments.Add(new AreaSegment(Polypoints, newPoints, series));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "StepArea";
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
