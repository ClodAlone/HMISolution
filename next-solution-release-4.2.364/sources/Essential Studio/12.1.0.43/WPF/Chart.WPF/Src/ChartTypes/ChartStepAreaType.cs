// <copyright file="ChartStepAreaType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;

    /// <summary>
    /// Represents ChartStepAreaType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartStepAreaType : ChartAreaType
    {
        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if (points.Length > 0)
            {
                List<IChartDataPoint> areaPoints = new List<IChartDataPoint>();

                areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, series.ActualXAxis.Origin));
                areaPoints.Add(new ChartPoint(points[0].DataPoint.X, series.ActualXAxis.Origin));
                double origin = series.ActualYAxis.Origin;
                for (int i = 0; i < points.Length; i++)
                {
                    if (i != 0)
                    {
                        areaPoints.Add(new ChartPoint(points[i - 1].DataPoint.X, points[i].DataPoint.Y));
                    }
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            areaPoints.Add(points[i].DataPoint);
                        }
                        else
                        {
                            if (i > 0)
                            {
                                ChartPoint cp = new ChartPoint();
                                cp.X = points[i - 1].DataPoint.X;
                                cp.Y = origin;
                                areaPoints.Add(cp);
                            }
                            if (i < points.Length-1)
                            {
                                ChartPoint cp = new ChartPoint();
                                cp.X = points[i + 1].DataPoint.X;
                                cp.Y = origin;
                                areaPoints.Add(cp); 
                            }
                            i++;
                        }
                    }
                    else
                    {
                        areaPoints.Add(points[i].DataPoint);
                    }
                }

                series.Segments.Add(new ChartAreaSegment(areaPoints.ToArray(), points, series));

                if (series.AdornmentsInfo.Visible)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        if (points[i].DataPoint.Y < 0)
                            series.AdornmentsInfo.m_requiresSymmetricLabelling = true;
                        if (points[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                                series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        /// <seealso cref="ChartStepAreaType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartStepAreaType"/>
        public override string ToString()
        {
            return "StepArea";
        }
    }
}
