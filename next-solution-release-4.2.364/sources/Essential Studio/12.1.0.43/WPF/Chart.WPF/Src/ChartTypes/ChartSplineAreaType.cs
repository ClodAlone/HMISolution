// <copyright file="ChartSplineAreaType.cs" company="Syncfusion">
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
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents spline area chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartSplineAreaSegment : ChartAreaSegment
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSplineAreaSegment"/> class.
        /// </summary>
        /// <param name="splinePoints">The spline points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="parentSeries">The parent series.</param>
        internal ChartSplineAreaSegment(IChartDataPoint[] splinePoints, ChartIndexedDataPoint[] correspondingPoints, ChartSeries parentSeries)
            : base(splinePoints, correspondingPoints, parentSeries)
        {
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
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
            double origin = Series.ActualYAxis.Origin;
            figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, 0);
            figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y), true));
            int i;
            
            for (i = 1; i < AreaPoints.Length; i += 3)
            {
                Point point1 = new Point();
                Point point2 = new Point();
                Point point3 = new Point();
                if (AreaPoints[0].EmptyPoint && !(Series.ShowEmptyPoints) && i == 1)
                {
                    figure.Segments.RemoveAt(figure.Segments.Count - 1);
                    
                    if ((i + 2) < AreaPoints.Length)
                    {
                        point1 = transformer.TransformToVisible(AreaPoints[i].X, origin);
                        point2 = transformer.TransformToVisible(AreaPoints[i + 1].X, origin);
                        point3 = transformer.TransformToVisible(AreaPoints[i + 2].X, origin);
                        figure.Segments.Add(new BezierSegment(point1, point2, point3, true));

                        point1 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                        point2 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                        point3 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                        figure.Segments.Add(new BezierSegment(point1, point2, point3, true));
                    }
                }
                else
                {
                    if ((i + 2) < AreaPoints.Length && AreaPoints[i + 2].EmptyPoint)
                    {
                        if (Series.ShowEmptyPoints)
                        {
                            point1 = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                            point2 = transformer.TransformToVisible(AreaPoints[i + 1].X, AreaPoints[i + 1].Y);
                            point3 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                            figure.Segments.Add(new BezierSegment(point1, point2, point3, true));
                        }
                        else
                        {

                            point1 = transformer.TransformToVisible(AreaPoints[i - 1].X, origin);
                            point2 = transformer.TransformToVisible(AreaPoints[i - 1].X, origin);
                            point3 = transformer.TransformToVisible(AreaPoints[i - 1].X, origin);
                            figure.Segments.Add(new BezierSegment(point1, point2, point3, true));
                            if ((i + 5) < AreaPoints.Length)
                            {
                                point1 = transformer.TransformToVisible(AreaPoints[i - 2].X, origin);
                                point2 = transformer.TransformToVisible(AreaPoints[i + 2].X, origin);
                                point3 = transformer.TransformToVisible(AreaPoints[i + 5].X, origin);
                                figure.Segments.Add(new BezierSegment(point1, point2, point3, true));

                                point1 = transformer.TransformToVisible(AreaPoints[i + 5].X, AreaPoints[i + 5].Y);
                                point2 = transformer.TransformToVisible(AreaPoints[i + 5].X, AreaPoints[i + 5].Y);
                                point3 = transformer.TransformToVisible(AreaPoints[i + 5].X, AreaPoints[i + 5].Y);
                                figure.Segments.Add(new BezierSegment(point1, point2, point3, true));

                                i += 3;
                            }                            
                        }
                    }
                    else
                    {
                        point1 = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                        point2 = transformer.TransformToVisible(AreaPoints[i + 1].X, AreaPoints[i + 1].Y);
                        point3 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                        figure.Segments.Add(new BezierSegment(point1, point2, point3, true));

                    }

                }
           }

            figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[i - 1].X, 0), true));
            figure.IsClosed = true;
            this.Geometry = new PathGeometry(new PathFigure[] { figure });
        }
        
        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            GeometryModel3D model = new GeometryModel3D();

            model.Geometry = MeshGenerator.SplineArea(AreaPoints, transformer);

            MaterialGroup materialGroup;

            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
            Binding binding = new Binding("Interior");
            binding.Source = Series;
            BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
            materialGroup = new MaterialGroup();
            materialGroup.Children.Add(difuseMaterial);

            model.Material = materialGroup;
            model.Transform = new TranslateTransform3D(-0.5, -0.5, (this.Series.Area.Series.IndexOf(this.Series) + 3) * 0.05);

            Geometry3DGroup.Children.Add(model);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.seriesCorrespondingPoints != null)
            {
                foreach (var item in this.seriesCorrespondingPoints)
                {
                    if (item.DataPoint is ChartPoint)
                    {
                        (item.DataPoint as ChartPoint).DisposePoint();
                    }
                }
            }
            SetValue(SeriesPropertyKey, null);
            this.AreaPoints = null;
            this.AxisLabelInfo = null;
            this.Geometry3D = null;
            this.Geometry3DGroup = null;            
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartSplineAreaType
    /// </summary>
    /// <seealso cref="ChartSplineAreaSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSplineAreaType : ChartSplineType
    {
        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            double c1 = GetSplineCoefficient(series);
            if (points.Length >= 2)
            {
                double[] yCoef;
                NaturalSpline(points, out yCoef);
                List<IChartDataPoint> segmentPoints = new List<IChartDataPoint>();
                segmentPoints.Add(points[0].DataPoint);
                for (int i = 1, count = points.Length; i < count; i++)
                {
                    IChartDataPoint startPoint = points[i - 1].DataPoint;
                    IChartDataPoint endPoint = points[i].DataPoint;
                    ChartPoint startControlPoint = null;
                    ChartPoint endControlPoint = null;
                    GetBezierControlPoints(startPoint, endPoint, yCoef[i - 1], yCoef[i], out startControlPoint, out endControlPoint);
                    segmentPoints.AddRange(new IChartDataPoint[] { startControlPoint, endControlPoint, endPoint });
                }

                series.Segments.Add(new ChartSplineAreaSegment(segmentPoints.ToArray(), points, series));

                if (series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Clear();
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartSplineAreaType"/>
        public override string ToString()
        {
            return "SplineArea";
        }
        #endregion
    }
}
