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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents spline area chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartSplineAreaSegment : AreaSegment
    {
        ChartPointsCollection areaPointsseg = new ChartPointsCollection();
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSplineAreaSegment"/> class.
        /// </summary>
        /// <param name="splinePoints">The spline points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The parent series.</param>
        internal ChartSplineAreaSegment(ChartPointsCollection splinePoints, ChartPointsCollection correspondingPoints, ChartSeries series)
            : base(splinePoints, correspondingPoints, series)
        {
            areaPointsseg = splinePoints;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            PathFigure figure = new PathFigure();
            PathFigureCollection figures = new PathFigureCollection();
            Point pt = new Point();
            figure.StartPoint = transformer.TransformToVisible(areaPointsseg[0].X + (series.XAxis.VisibleRange.Start * (-1)), series.XAxis.Origin - (series.YAxis.VisibleRange.Start), series);
            pt = figure.StartPoint;
            pt.X = figure.StartPoint.X;// +series.YAxis.LineStrokeThickness;
            pt.Y = figure.StartPoint.Y;// -series.XAxis.LineStrokeThickness;
            figure.StartPoint = pt;

            pt = transformer.TransformToVisible(areaPointsseg[0].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[0].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            pt.X = pt.X + series.YAxis.LineStrokeThickness;
            figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = pt });

            double endPt = areaPointsseg[areaPointsseg.Count - 1].X;

            int i;
            for (i = 1; i < areaPointsseg.Count; i += 3)
            {
                double origin = Series.XAxis.Origin;
                Point point1 = new Point();
                Point point2 = new Point();
                Point point3 = new Point();
                if (areaPointsseg[0].EmptyPoint && !(Series.ShowEmptyPoints) && i == 1)
                {
                    figure.Segments.RemoveAt(figure.Segments.Count - 1);

                    if ((i + 2) < areaPointsseg.Count)
                    {
                        point1 = transformer.TransformToVisible(areaPointsseg[i].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                        point2 = transformer.TransformToVisible(areaPointsseg[i + 1].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                        point3 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                        figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });

                        point1 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        point2 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        point3 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                    }
                }
                else
                {
                    if ((i + 2) < areaPointsseg.Count && areaPointsseg[i + 2].EmptyPoint)
                    {
                        if (Series.ShowEmptyPoints)
                        {
                            point1 = transformer.TransformToVisible(areaPointsseg[i].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                            point2 = transformer.TransformToVisible(areaPointsseg[i + 1].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 1].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                            point3 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                            figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                        }
                        else
                        {

                            point1 = transformer.TransformToVisible(areaPointsseg[i - 1].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                            point2 = transformer.TransformToVisible(areaPointsseg[i - 1].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                            point3 = transformer.TransformToVisible(areaPointsseg[i - 1].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                            figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                            if ((i + 5) < areaPointsseg.Count && i-2>=0)
                            {
                                point1 = transformer.TransformToVisible(areaPointsseg[i - 2].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                                point2 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                                point3 = transformer.TransformToVisible(areaPointsseg[i + 5].X + (series.XAxis.VisibleRange.Start * (-1)), origin + (series.YAxis.VisibleRange.Start * (-1)), series);
                                figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });

                                point1 = transformer.TransformToVisible(areaPointsseg[i + 5].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 5].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                                point2 = transformer.TransformToVisible(areaPointsseg[i + 5].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 5].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                                point3 = transformer.TransformToVisible(areaPointsseg[i + 5].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 5].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                                figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });

                                i += 3;
                            }
                        }
                    }
                    else
                    {
                        point1 = transformer.TransformToVisible(areaPointsseg[i].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        point2 = transformer.TransformToVisible(areaPointsseg[i + 1].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 1].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        if (areaPointsseg[i + 2].X == endPt)
                        {
                            pt = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                            pt.X = pt.X - series.XAxis.GridLineStrokeThickness;
                            pt.Y = pt.Y;
                            point3 = pt;
                        }
                        else
                            point3 = transformer.TransformToVisible(areaPointsseg[i + 2].X + (series.XAxis.VisibleRange.Start * (-1)), areaPointsseg[i + 2].Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                        figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });

                    }

                }
            }

            pt = transformer.TransformToVisible(areaPointsseg[i - 1].X + (series.XAxis.VisibleRange.Start * (-1)),  series.XAxis.Origin - (series.YAxis.VisibleRange.Start), series);
            pt.X = pt.X;// -series.XAxis.GridLineStrokeThickness;
            pt.Y = pt.Y;// -series.XAxis.LineStrokeThickness;
            figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = pt });
            figure.IsClosed = true;
            figures.Add(figure);
            this.AreaPoints = new PathGeometry() { Figures = figures };
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.areaPointsseg != null)
            {
                for (int temp = 0; temp < this.areaPointsseg.Count; temp++)
                {
                    this.areaPointsseg[temp] = null;
                }
                this.areaPointsseg.Clear();
                this.areaPointsseg = null;
            }
        }
    }

    /// <summary>
    /// Class implementation for ChartSplineAreaType
    /// </summary>
    public class ChartSplineAreaType : ChartSplineType
    {
        #region Implementation
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "SplineArea";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            for (int i = 1, count = points.Count; i < count; i++)
            {
                if (double.IsNaN(points[i].Y))
                    points[i].EmptyPoint = true;
                if (points[i].EmptyPoint && series.ShowEmptyPoints)
                {
                    if (series.EmptyPointValue == EmptyPointValue.Average)
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
                    else
                    {
                        points[i].Y = 0;
                    }

                }
            }
            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            double c1 = GetSplineCoefficient(series);
            if (points.Count >= 2)
            {
                double[] yCoef;
                this.NaturalSpline(points, out yCoef);
                ChartPointsCollection segmentPoints = new ChartPointsCollection();
                segmentPoints.Add(new ChartPoint(points[0].X, points[0].Y));
                if (series.Area.Host == Host.OLAPChart)
                {
                    segmentPoints[0].X -= 0.5;
                }
                for (int i = 1, count = points.Count; i < count; i++)
                {
                    //if (points[i - 1].Y.Equals(double.NaN) || points[i].Y.Equals(double.NaN) || points[i - 1].X.Equals(double.NaN) || points[i].X.Equals(double.NaN))
                    //    continue;
                    ChartPoint startPoint = points[i - 1];
                    ChartPoint endPoint = points[i];

                    ChartPoint startControlPoint = null;
                    ChartPoint endControlPoint = null;

                    GetBezierControlPoints(startPoint, endPoint, yCoef[i - 1], yCoef[i], out startControlPoint, out endControlPoint);
                    ChartPoint point1 = new ChartPoint(startPoint.X, startPoint.Y);
                    ChartPoint point2 = new ChartPoint(endPoint.X, endPoint.Y);
                    ChartPoint point3 = new ChartPoint(startControlPoint.X, startControlPoint.Y);
                    ChartPoint point4 = new ChartPoint(endControlPoint.X, endControlPoint.Y);

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        point1.X -= 0.5;
                        point2.X -= 0.5;
                        point3.X -= 0.5;
                        point4.X -= 0.5;
                    }
                    if (points[i].EmptyPoint || points[i - 1].EmptyPoint)
                    {
                        point3.EmptyPoint = true;
                        point2.EmptyPoint = true;
                        point4.EmptyPoint = true;
                    }
                    segmentPoints.Add(point3);
                    segmentPoints.Add(point4);
                    segmentPoints.Add(point2);
                }

                series.Segments.Add(new ChartSplineAreaSegment(segmentPoints, points, series));

                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (series.Area.Host == Host.OLAPChart)
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].X - 0.5, points[i].Y), points, series, 0d));
                        }
                        else
                        {
                            if (points[i].EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                    series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                            }
                            else
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
