// <copyright file="ChartHiLoAreaType.cs" company="Syncfusion">
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
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Represents HiLo area segment type.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartHiLoAreaType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartHiLoAreaSegment : ChartRangeAreaSegment
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHiLoAreaSegment"/> class.
        /// </summary>
        /// <param name="points">The ChartData points</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        /// <param name="isHighLow">The HighLow bool value</param>
        public ChartHiLoAreaSegment(IChartDataPoint[] points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series, bool isHighLow)
            : base(points, correspondingPoints, series, isHighLow)
        {
            Binding interiorBinding = new Binding();
            interiorBinding.Path = new PropertyPath(ChartSeries.InteriorProperty);
            if (isHighLow)
            {
                interiorBinding.Source = series;
            }
            else
            {
                foreach (ChartSeries interiorSeries in series.Area.Series)
                {
                    if (interiorSeries != series && interiorSeries.IsVisible)
                    {
                        interiorBinding.Source = interiorSeries;
                        break;
                    }
                }
            }

            BindingOperations.SetBinding(this, FillBrushProperty, interiorBinding);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartHiLoAreaSegment"/>
        public override void Update(IChartTransformer transformer)
        {

            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }
            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            PathFigure figure = new PathFigure();

            int startIndex = 1;
            int endIndex = AreaPoints.Length - 2;

            if (this.Series.Segments.IndexOf(this) == 0)
            {
                startIndex = 2;
            }

            if (this.Series.Segments.IndexOf(this) == this.Series.Segments.Count - 1)
            {
                endIndex = AreaPoints.Length - 1;
            }

            if (AreaPoints.Length > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y);

                for (int i = startIndex; i < AreaPoints.Length; i += 2)
                {
                    figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y), true));
                }

                for (int i = endIndex; i >= 1; i -= 2)
                {
                    figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y), true));
                }

                figure.IsClosed = true;
            }

            this.Geometry = new PathGeometry(new PathFigure[] { figure });
        }
        #endregion
    }

    /// <summary>
    /// Represents HiLoArea type class
    /// </summary>
    /// <remarks>
    /// HiLoArea Chart is a variation of Area Chart type that is normally used in stock
    /// analysis.
    /// </remarks>
    /// <seealso cref="ChartHiLoAreaSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   public class ChartHiLoAreaType : ChartRangeAreaType
    {
        #region Properties
        /// <summary>
        /// Gets the requirement for data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Determines whether the this chart type is compatible with specified type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>
        /// <c>true</c> if the type is compatible; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCompatible(ChartType type)
        {
            return !(type is ChartHiLoAreaType) && type.AxesType == ChartAxesType.CartesianAxes && !type.IsRotated;
        }

        internal List<ChartIndexedDataPoint> segmentSummaryPoints = new List<ChartIndexedDataPoint>();
        #endregion

        #region Pulic methods
        /// <summary>
        /// Returns the string representation.
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartHiLoAreaType"/>
        public override string ToString()
        {
            return "HiLoArea";
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            Point point1;
            Point point2;
            Point point3;
            Point point4;
            Point? crossPoint;
            List<Point> segPoints = new List<Point>();
            segmentSummaryPoints = new List<ChartIndexedDataPoint>();
            ChartSeriesCollection compatibleSeries = new ChartSeriesCollection();
            foreach (ChartSeries areaSeries in series.Area.Series)
            {
                if (areaSeries.Type == ChartTypes.HiLoArea && areaSeries.IsVisible)
                {
                    compatibleSeries.Add(areaSeries);
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        double secondY = (compatibleSeries.Count <= 1) ? 0 : compatibleSeries[1].GetPoint(i).Y;
                        segmentSummaryPoints.Add(new ChartIndexedDataPoint(new ChartPoint(points[i].DataPoint.X, new double[] { points[i].DataPoint.Y, secondY }), i));
                
                    }
                }
                else
                {
                    double secondY = (compatibleSeries.Count <= 1) ? 0 : compatibleSeries[1].GetPoint(i).Y;
                    segmentSummaryPoints.Add(new ChartIndexedDataPoint(new ChartPoint(points[i].DataPoint.X, new double[] { points[i].DataPoint.Y, secondY }), i));
                }
            }
            if (segmentSummaryPoints.Count > 0)
            {
                segPoints.Add(new Point(segmentSummaryPoints[0].DataPoint.X, segmentSummaryPoints[0].DataPoint.Values[0]));
                segPoints.Add(new Point(segmentSummaryPoints[0].DataPoint.X, segmentSummaryPoints[0].DataPoint.Values[1]));

                bool isHighLow = (segPoints[0].Y > segPoints[1].Y) ? true : false;
                int i;
                for ( i= 0; i < segmentSummaryPoints.Count - 1; i++)
                {
                    //if (!(i >= 5 && i <= 7))
                    //{
                        point1 = new Point(segmentSummaryPoints[i].DataPoint.X, segmentSummaryPoints[i].DataPoint.Values[0]);
                        point2 = new Point(segmentSummaryPoints[i + 1].DataPoint.X, segmentSummaryPoints[i + 1].DataPoint.Values[0]);
                        point3 = new Point(segmentSummaryPoints[i].DataPoint.X, segmentSummaryPoints[i].DataPoint.Values[1]);
                        point4 = new Point(segmentSummaryPoints[i + 1].DataPoint.X, segmentSummaryPoints[i + 1].DataPoint.Values[1]);
                        crossPoint = ChartRangeAreaType.GetCrossPoint(point1, point2, point3, point4);

                        if (crossPoint != null)
                        {
                            segPoints.Add(crossPoint.Value);
                            isHighLow = points[i].DataPoint.Values[0] > points[i].DataPoint.Values[1];
                            series.Segments.Add(new ChartHiLoAreaSegment(Point2ChartPoint(segPoints), segmentSummaryPoints.ToArray(), series, isHighLow));
                            //isHighLow = !isHighLow;
                            segPoints = new List<Point>();
                            segPoints.Add(crossPoint.Value);
                        }

                        segPoints.Add(point2);
                        segPoints.Add(point4);
                    //}
                }   
                isHighLow = segmentSummaryPoints[i].DataPoint.Values[0] > segmentSummaryPoints[i].DataPoint.Values[1];//points[i].DataPoint.Values[0] > points[i].DataPoint.Values[1];
                series.Segments.Add(new ChartHiLoAreaSegment(Point2ChartPoint(segPoints), segmentSummaryPoints.ToArray(), series, isHighLow));

                if (series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Clear();
                    int index = -1;
                    for (i = 0; i < points.Length; i++)
                    {
                        double y1 = segmentSummaryPoints[i].DataPoint.Values[0];
                        double y2 = segmentSummaryPoints[i].DataPoint.Values[1];
                        //Add for SD11357
                        if (points[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                {
                                    series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                    series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                }
                                else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                         (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                                {
                                    series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                }
                                else
                                {
                                    series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                }
                            }
                        }
                        else
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                            else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                     (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
