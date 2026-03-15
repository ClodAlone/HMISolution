// <copyright file="ChartFastSplineType.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Linq;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Represents FastSpline chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartFastSplineType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastSplineSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartFastSplineSegment), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartFastSplineSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the points collection.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        /// <summary>
        /// Gets or sets the segment's geometry. This is a dependency property.
        /// </summary>
        /// <value>The geometry.</value>
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }
        #endregion

        #region Internal Members
        /// <summary>
        /// Initializes m_points
        /// </summary>
        internal List<IChartDataPoint> m_points;

        private PointCollection m_vpts1 = new PointCollection();        

        internal bool refresh = false;

        internal double ViewPortwidth=0;

        internal double ViewPortHeight=0;

        internal IChartTransformer temptrans;

        internal int tempcount;

        internal ChartIndexedDataPoint[] datapoints;

        private List<IChartDataPoint> smoothPoints=new List<IChartDataPoint>();

        int lastPoint = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartFastSplineSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is being assigned automatically.
        /// </remarks>
        static ChartFastSplineSegment()
        {
            Type type = typeof(ChartFastSplineSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFastSplineSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartFastSplineSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            m_points = points;
            datapoints = correspondingPoints;
            Points = new PointCollection();
            double X_MAX = m_points.Max(x => x.X);
            double Y_MAX = m_points.Max(y => y.Y);
            double X_MIN = m_points.Min(x => x.X);
            double Y_MIN = m_points.Min(y => y.Y);
            double Z_MAX = m_points.Max(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 0.0);
            double Z_MIN = m_points.Min(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 1.0);
            xRange = new DoubleRange(X_MIN, X_MAX);
            yRange = new DoubleRange(Y_MIN, Y_MAX);
            zRange = new DoubleRange(Z_MIN, Z_MAX);
        }
        #endregion

        #region Implmentation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        protected override void SetPointsForAllSeries(ChartSeries series, ChartAxis axis)
        {
            foreach (ChartSeries item in series.Area.Series)
            {
                if ((item.ActualXAxis == axis || item.ActualYAxis == axis) && item.Segments.Count != 0)
                {
                    if (item.Segments[0].GetType() == typeof(ChartFastSplineSegment))
                    {
                        ((ChartFastSplineSegment)item.Segments[0]).SetPointToNull();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SetPointToNull()
        {
            this.Points = null;
        }
        /// <summary>
        /// Method implementation for UpdateSegments for update the FastSplineSegments by given ChartSeries
        /// </summary>
        /// <param name="points"></param>
        /// <param name="series"></param>
        public void UpdateSegment(List<IChartDataPoint> points, ChartSeries series)
        {
            m_points = points;
            m_vpts1 = new PointCollection(m_points.Count);
            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {

            this.seriesCorrespondingPoints = null;
            if (this.Points != null)
            {
                this.Points.Clear();
                this.Points = null;
            }

            if (this.m_points != null)
            {
                foreach (ChartPoint item in this.m_points)
                {
                    item.Dispose();
                }
                this.m_points.Clear();
                this.m_points = null;
            }

            if (this.m_vpts1 != null)
            {
                this.m_vpts1.Clear();
                this.m_vpts1 = null;
            }
            this.Item = null;
            base.Dispose();

        }
        /// <summary>
        /// Get the points and set the range to the corresponding series
        /// </summary>
        /// <param name="point"></param>
        /// <param name="series"></param>
        public void GetSegmet(IChartDataPoint point, ChartSeries series)
        {
            m_points.Add(point);
            m_vpts1.Add(new Point());
            xRange += point.X;
            yRange += point.Y;
            SetRange(series);
        }
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            IChartDataPoint startPoint = null;
            IChartDataPoint endPoint = null;
            ChartPoint startControlPoint = null;
            ChartPoint endControlPoint = null;
            if (!this.Series.Presenter.Issizechanged && m_vpts1.Count > 0 && this.Points.Count > 0)
            {

                if (this.Points != null)
                {
                    var v_portWidth = transformer.Viewport.Width;
                    var v_portHeight = transformer.Viewport.Height;
                    var xAxis = Series.XAxis;
                    var yAxis = Series.YAxis;
                    Point pt = new Point();
                    Point lastp = new Point();
                    bool opt = Series.UseOptimization;
                    double resolution = Series.Resolution;
                    var points = m_points;
                    bool opposed = Series.IsRotated;
                    bool x_inverse = Series.XAxis.IsInversed;
                    bool y_inverse = Series.YAxis.IsInversed;
                    bool ptinit = false;
                    bool Iszoomactivated = xAxis.Area.SecondaryAxis.ZoomFactor < 1;
                    if (xAxis.IsLogarithmic || yAxis.IsLogarithmic)
                    {
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false)//&& (Iszoomactivated ? ((xAxis.VisibleRange.Inside(points[i].X - xAxis.VisibleInterval) || xAxis.VisibleRange.Inside(points[i].X + xAxis.VisibleInterval))) : true))
                        {
                            pt = transformer.TransformToVisible(points[points.Count - 1].X, points[points.Count - 1].Y);
                            ptinit = true;                           
                        }

                        if (points.Count - 1 > 0)
                        {
                            lastp = m_vpts1[m_vpts1.Count - 1];
                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {                               
                                m_vpts1.Add(pt);
                                smoothPoints.Add(points[points.Count - 1]);
                                lastp = pt;
                            }
                        }
                        else
                        {
                            if (ptinit)
                            {
                                m_vpts1.Add(pt);
                                smoothPoints.Add(points[points.Count - 1]);
                            }
                        }
                    }
                    else
                    {
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false)
                        {                           
                            ptinit = true;
                            pt = opposed ?
                              new Point(v_portWidth * yAxis.ValueToCoefficient1(points[points.Count - 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[points.Count - 1].X, x_inverse))) :
                              new Point(v_portWidth * xAxis.ValueToCoefficient1(points[points.Count - 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[points.Count - 1].Y, y_inverse)));
                        }                       
                        if (points.Count - 1 > 0)
                        {
                            lastp = m_vpts1[m_vpts1.Count - 1];
                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {
                                if (ptinit)
                                {
                                    m_vpts1.Add(pt);
                                    smoothPoints.Add(points[points.Count - 1]);
                                }
                                lastp = pt;
                            }
                        }
                        else
                        {
                            if (ptinit)
                            {
                                m_vpts1.Add(pt);
                                smoothPoints.Add(points[points.Count - 1]);
                            }
                        }

                    }                    
                    lastPoint = this.Points.Count;
                    refresh = (refresh = true ? false : true);
                }
                else
                {
                    if (m_points.Count - lastPoint == 1 && m_vpts1[m_vpts1.Count - 1].X == 0 && m_vpts1[m_vpts1.Count - 1].Y == 0)
                    {                        
                        m_vpts1[m_vpts1.Count - 1] = transformer.TransformToVisible(m_points[m_points.Count - 1].X, m_points[m_points.Count - 1].Y);
                        this.Points[this.Points.Count - 1] = m_vpts1[m_vpts1.Count - 1];
                        refresh = true;
                    }
                    else if (m_points.Count != lastPoint && (Series.UseOptimization == false))
                    {
                        for (int i = lastPoint; i < m_vpts1.Count; i++)
                        {
                            m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                        }
                        this.Points = m_vpts1;
                        refresh = true;
                    }
                    lastPoint = m_points.Count;
                }
            }
            else
            {
                temptrans = transformer;
                tempcount = m_points.Count;
                if (this.Interior != null && this.Interior.CanFreeze)
                {
                    this.Interior.Freeze();
                }

                if (this.Stroke.CanFreeze)
                {
                    this.Stroke.Freeze();
                }
                m_vpts1.Clear();
                smoothPoints.Clear();
                var v_portWidth = transformer.Viewport.Width;
                var v_portHeight = transformer.Viewport.Height;
                var xAxis = Series.XAxis;
                var yAxis = Series.YAxis;
                Point pt = new Point();
                Point prept = new Point();
                Point postpt = new Point();
                Point lastp = new Point();
                bool opt = Series.UseOptimization;
                double resolution = Series.Resolution;
                var points = m_points;
                bool opposed = Series.IsRotated;
                bool x_inverse = Series.XAxis.IsInversed;
                bool y_inverse = Series.YAxis.IsInversed;
                int preIndex = 0, postIndex = 0, j = 0;
                bool ptini = false;
                bool Iszoomactivated = xAxis.Area.SecondaryAxis.ZoomFactor < 1 || xAxis.Area.PrimaryAxis.EnableAutoScrolling == true;
                if (xAxis.IsLogarithmic || yAxis.IsLogarithmic)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false)//&& (Iszoomactivated ? ((xAxis.VisibleRange.Inside(points[i].X - xAxis.VisibleInterval) || xAxis.VisibleRange.Inside(points[i].X + xAxis.VisibleInterval))) : true))
                        {
                            pt = transformer.TransformToVisible(points[i].X, points[i].Y);
                        }
                        if (i > 0)
                        {
                            lastp = m_vpts1[m_vpts1.Count - 1];
                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {
                                m_vpts1.Add(pt);
                                smoothPoints.Add(points[i]);
                                lastp = pt;
                            }
                        }
                        else
                        {
                            m_vpts1.Add(pt);
                            smoothPoints.Add(points[i]);
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false && (Iszoomactivated ? (xAxis.VisibleRange.Inside(points[i].X)) : true))// xAxis.VisibleRange.Inside(points[i].X))//(Iszoomactivated ? (xAxis.VisibleRange.Inside(points[i].X)) : true))
                        {
                            j++;
                            pt = opposed ?
                              new Point(v_portWidth * yAxis.ValueToCoefficient1(points[i].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[i].X, x_inverse))) :
                              new Point(v_portWidth * xAxis.ValueToCoefficient1(points[i].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[i].Y, y_inverse)));
                            if (j == 1)
                            {
                                preIndex = i;
                            }
                            postIndex = i;
                            ptini = true;
                        }

                        if (m_vpts1.Count > 0)
                        {
                            lastp = m_vpts1[m_vpts1.Count - 1];

                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {
                                if (lastp != pt && ptini)
                                {
                                    m_vpts1.Add(pt);
                                    smoothPoints.Add(points[i]);
                                    lastp = pt;
                                }
                            }
                        }
                        else
                        {
                            m_vpts1.Add(pt);
                            smoothPoints.Add(points[i]);
                            lastp = pt;
                        }

                    }
                    // add prept and postpt
                    if (preIndex > 0)
                    {
                        prept = opposed ?
                         new Point(v_portWidth * yAxis.ValueToCoefficient1(points[preIndex - 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[preIndex - 1].X, x_inverse))) :
                         new Point(v_portWidth * xAxis.ValueToCoefficient1(points[preIndex - 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[preIndex - 1].Y, y_inverse)));
                        m_vpts1[0] = prept;
                    }
                    if (postIndex + 1 != points.Count && m_vpts1.Count != 0)
                    {
                        if ((Iszoomactivated ? (xAxis.VisibleRange.Inside(points[postIndex].X)) : true))
                        {
                            postpt = opposed ?
                             new Point(v_portWidth * yAxis.ValueToCoefficient1(points[postIndex + 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[postIndex + 1].X, x_inverse))) :
                             new Point(v_portWidth * xAxis.ValueToCoefficient1(points[postIndex + 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[postIndex + 1].Y, y_inverse)));
                            m_vpts1.Add(postpt);
                            smoothPoints.Add(points[postIndex + 1]);
                        }
                    }

                }
                this.Points = m_vpts1;
                lastPoint = this.Points.Count;
                double[] yCoef={0};
                PathFigureCollection figures = new PathFigureCollection();
                if (Points.Count >= 2)            
                NaturalSpline(smoothPoints, out yCoef);
                for (int i = 1; i < Points.Count; i++)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = Points[i-1];
                    startPoint = smoothPoints[i-1];
                    endPoint = smoothPoints[i];
                    GetBezierControlPoints(startPoint, endPoint, yCoef[i - 1], yCoef[i], out startControlPoint, out endControlPoint);
                    Point point2 = transformer.TransformToVisible(startControlPoint.X, startControlPoint.Y);
                    Point point3 = transformer.TransformToVisible(endControlPoint.X, endControlPoint.Y);
                    Point point4 = Points[i];
                    figure.Segments.Add(new BezierSegment(point2, point3, point4, true));
                    figures.Add(figure);
                }
                this.Geometry = new PathGeometry(figures);  
                refresh = (refresh = true ? false : true);                
            }
                                   
        }                
        #endregion

        #region Smooth Implementation

        /// <summary>
        /// Naturals the spline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="ys2">The ys2 value.</param>
        internal static void NaturalSpline(List<IChartDataPoint> points, out double[] ys2)
        {
            int count = points.Count;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count - 1];
            double p;

            ys2[0] = u[0] = 0;
            ys2[count - 1] = 0;

            for (int i = 1; i < count - 1; i++)
            {
                double d1 = points[i].X - points[i - 1].X;
                double d2 = points[i + 1].X - points[i - 1].X;
                double d3 = points[i + 1].X - points[i].X;
                double dy1 = points[i + 1].Y - points[i].Y;
                double dy2 = points[i].Y - points[i - 1].Y;

                if (points[i].X == points[i - 1].X || points[i].X == points[i + 1].X)
                {
                    ys2[i] = 0;
                    u[i] = 0;
                }
                else
                {
                    p = 1 / (d1 * ys2[i - 1] + 2 * d2);

                    ys2[i] = -p * d3;
                    u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }

        /// <summary>
        /// Gets the bezier control points.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="ys1">The ys1 value.</param>
        /// <param name="ys2">The ys2 value.</param>
        /// <param name="controlPoint1">The control point1.</param>
        /// <param name="controlPoint2">The control point2.</param>
        protected static void GetBezierControlPoints(IChartDataPoint point1, IChartDataPoint point2, double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;
            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new ChartPoint(dx1 * One_thrid, y1);
            controlPoint2 = new ChartPoint(dx2 * One_thrid, y2);
        }

        #endregion
    }

    /// <summary>
    /// Represents FastSpline chart type.
    /// </summary>
    /// <remarks>
    /// FastSpline Chart is similar to a Spline Chart except that it gives more performance
    /// </remarks>
    /// <seealso cref="ChartFastSplineSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastSplineType : ChartType
    {
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None | ChartTypeFlags.Indexed;
            }
        }
        #region Private Members
        List<IChartDataPoint> linePoints;
        #endregion
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if (series.ShowEmptyPoints == false && series.Area.EnableLazyLoading == true)
            {
                linePoints = new List<IChartDataPoint>();
                if (series.ActualYAxis.IsAutoSetRange == true || series.ActualXAxis.IsAutoSetRange == true)
                {
                    series.Segments.Clear();
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        linePoints.Add(points[i].DataPoint);
                    }
                    series.Segments.Add(new ChartFastSplineSegment(linePoints, points, series));

                    return;
                }

                if (series.Segments.Count != 0)
                {
                    ChartFastSplineSegment segment = ((ChartFastSplineSegment)series.Segments[0]);

                    if (segment.Points != null && segment.Points.Count < points.Length)
                    {
                        segment.GetSegmet(points[points.Length - 1].DataPoint, series);
                    }
                    else if (segment.Points == null || segment.Points.Count > points.Length)
                    {
                        for (int i = 0; i < points.Length; i++)
                        {
                            linePoints.Add(points[i].DataPoint);
                        }
                        segment.UpdateSegment(linePoints, series);
                        segment.refresh = true;
                    }
                }
                else
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        linePoints.Add(points[i].DataPoint);
                    }
                    series.Segments.Add(new ChartFastSplineSegment(linePoints, points, series));
                }
            }
            else if (series.Segments.Count == 0 || series.internaldata_modified || linePoints.Count > points.Length)
            {
                linePoints = new List<IChartDataPoint>();
                series.Segments.Clear();
                series.Adornments.Clear();
                ChartIndexedDataPoint[] pts = points;
                List<ChartIndexedDataPoint> tempPointArray = new List<ChartIndexedDataPoint>();
                for (int i = 0; i < pts.Length; i++)
                {
                    switch (pts[i].DataPoint.EmptyPoint)
                    {
                        case false:
                            {
                                linePoints.Add(pts[i].DataPoint);
                                tempPointArray.Add(pts[i]);
                                break;
                            }
                        case true:
                            {
                                if (linePoints.Count > 0)
                                {
                                    if (i < points.Length)
                                        linePoints.Add(points[i].DataPoint);
                                    tempPointArray.Add(points[i]);                                    
                                }
                                break;
                            }
                    }                   
                }

                if (tempPointArray.Count != 0 && linePoints.Count != 0)
                {
                    series.Segments.Add(new ChartFastSplineSegment(linePoints, tempPointArray.ToArray(), series));
                }

            }
            if (series.Segments.Count > 0)
            {
                List<ChartIndexedDataPoint> tempPointArray = new List<ChartIndexedDataPoint>();
                List<ChartIndexedDataPoint> pts = points.ToList();

                if (!series.Contains_emptypt)
                {                    
                    int cnt = linePoints.Count;                    
                    while (linePoints.Count != points.Length && linePoints.Count < points.Length)
                    {
                        linePoints.Add(points[cnt].DataPoint);
                        cnt++;
                    }
                }
                (series.Segments[0] as ChartFastSplineSegment).m_points = linePoints;
                if (series.ActualXAxis.IsAutoSetRange || series.Zoomactionenabled)
                {                   
                    double X_MAX = linePoints.Max(x => x.X);
                    double X_MIN = linePoints.Min(x => x.X);
                    (series.Segments[0] as ChartFastSplineSegment).xRange = new DoubleRange(X_MIN, X_MAX);
                    if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
                    {
                        (series.Segments[0] as ChartFastSplineSegment).xRange += (series.Segments[0] as ChartFastSplineSegment).xRange.Start - 0.5;
                        (series.Segments[0] as ChartFastSplineSegment).xRange += (series.Segments[0] as ChartFastSplineSegment).xRange.End + 0.5;
                    }

                    (series.Segments[0] as ChartFastSplineSegment).SetRange(series);

                }
                if (series.ActualYAxis.IsAutoSetRange || series.Zoomactionenabled)
                {                    

                    double Y_MAX = linePoints.Max(y => y.Y);
                    double Y_MIN = linePoints.Min(y => y.Y);

                    (series.Segments[0] as ChartFastSplineSegment).yRange = new DoubleRange(Y_MIN, Y_MAX);

                    if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
                    {
                        (series.Segments[0] as ChartFastSplineSegment).xRange += (series.Segments[0] as ChartFastSplineSegment).xRange.Start - 0.5;
                        (series.Segments[0] as ChartFastSplineSegment).xRange += (series.Segments[0] as ChartFastSplineSegment).xRange.End + 0.5;
                    }

                    (series.Segments[0] as ChartFastSplineSegment).SetRange(series);
                    if (series.Zoomactionenabled)
                    {
                        series.Zoomactionenabled = false;
                    }
                }
            }            
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartFastSplineType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if (points.Length > 0)
                this.CalculateSegments(series, points);
            else
                series.Segments.Clear();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartFastSplineType"/>
        public override string ToString()
        {
            return "FastSpline";
        }        
    }

    /// <summary>
    /// Represents spline chart  Adornments
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="FastSplinePresenter"/>
    public class FastSplinePresenter : ChartFastSeriesPresenter
    {

        /// <summary>
        /// Get or Set PointsProperty
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        ///  Identifies the points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(FastSplinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Get or Set SeriesProperty 
        /// </summary>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        
        /// <summary>
        ///  Identifies the Series  dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(FastSplinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        //double count = 0d;
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            bool useOptimization = Series.UseOptimization;

            ChartFastSplineSegment fastLineSegment = this.DataContext as ChartFastSplineSegment;
            double height = fastLineSegment.ViewPortHeight;
            double width = fastLineSegment.ViewPortwidth;

            if (Points != null)
            {
                base.VisualCollection.Clear();

                SyncDrawingVisual visual = new SyncDrawingVisual();
                PointCollection pp = this.Points;
                ChartSeries ser = Series;
                Pen pn = ser.FastTypePen;
                double adornHeight = Series.AdornmentsInfo.SymbolHeight;
                double adornWidth = Series.AdornmentsInfo.SymbolWidth;
                Brush adornBr = Series.AdornmentsInfo.SymbolInterior;
                Brush adornPenBr = Brushes.Black;
                Pen adornPen = new Pen(adornBr, 3);
               
                if (Series.AdornmentsInfo.Visible == true)
                {
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        int count = pp.Count;
                        for (int j = 0; j < count; j++)
                        {
                            visual.Index = j;
                            FormattedText text = new FormattedText(Math.Ceiling(this.Series.Data[j].Y).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black);
                            Symbol adornSymbol = Series.AdornmentsInfo.Symbol;
                            switch (adornSymbol)
                            {
                                case Symbol.Square:
                                    context.DrawRectangle(adornBr, adornPen, new Rect(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 2), new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 2)));
                                    break;
                                case Symbol.Ellipse:
                                    context.DrawEllipse(adornBr, adornPen, pp[j], adornWidth / 2, adornHeight / 2);
                                    break;
                                case Symbol.HorizontalLine:
                                    context.DrawLine(adornPen, new Point(pp[j].X - adornWidth / 2, pp[j].Y), new Point(pp[j].X + adornWidth / 2, pp[j].Y));
                                    break;
                                case Symbol.VerticalLine:
                                    context.DrawLine(adornPen, new Point(pp[j].X, pp[j].Y + adornHeight / 2), new Point(pp[j].X, pp[j].Y - adornHeight / 2));
                                    break;
                                case Symbol.Triangle:
                                    Point start = new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 2);
                                    LineSegment[] segments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 2), true) };
                                    PathFigure figure = new PathFigure(start, segments, true);
                                    PathGeometry geo = new PathGeometry(new PathFigure[] { figure });
                                    context.DrawGeometry(adornBr, null, geo);
                                    break;
                                case Symbol.InvertedTriangle:
                                    Point invertStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight / 2);
                                    LineSegment[] invertSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 2), true) };
                                    PathFigure invertFigure = new PathFigure(invertStart, invertSegments, true);
                                    PathGeometry invertGeo = new PathGeometry(new PathFigure[] { invertFigure });
                                    context.DrawGeometry(adornBr, null, invertGeo);
                                    break;
                                case Symbol.Diamond:
                                    Point diamondStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y);
                                    LineSegment[] diamondSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y), true), new LineSegment(new Point(pp[j].X, pp[j].Y + adornHeight / 2), true) };
                                    PathFigure diamondFigure = new PathFigure(diamondStart, diamondSegments, true);
                                    PathGeometry diamondGeo = new PathGeometry(new PathFigure[] { diamondFigure });
                                    context.DrawGeometry(adornBr, null, diamondGeo);
                                    break;
                                case Symbol.Hexagon:
                                    Point hexStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y);
                                    LineSegment[] hexSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true), };
                                    PathFigure hexFigure = new PathFigure(hexStart, hexSegments, true);
                                    PathGeometry hexGeo = new PathGeometry(new PathFigure[] { hexFigure });
                                    context.DrawGeometry(adornBr, null, hexGeo);
                                    break;
                                case Symbol.Pentagon:
                                    Point pentaStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y);
                                    LineSegment[] pentaSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true), };
                                    PathFigure pentaFigure = new PathFigure(pentaStart, pentaSegments, true);
                                    PathGeometry pentaGeo = new PathGeometry(new PathFigure[] { pentaFigure });
                                    context.DrawGeometry(adornBr, null, pentaGeo);
                                    break;
                                case Symbol.Plus:
                                    Point plusStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight / 6);
                                    LineSegment[] plusSegments = new LineSegment[] { 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y + adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 6), true),
                                    };
                                    PathFigure plusFigure = new PathFigure(plusStart, plusSegments, true);
                                    PathGeometry plusGeo = new PathGeometry(new PathFigure[] { plusFigure });
                                    context.DrawGeometry(adornBr, null, plusGeo);
                                    break;
                                case Symbol.Cross:
                                    Point crossStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight / 4);
                                    LineSegment[] crossSegments = new LineSegment[] { 
                                         new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y ), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X, pp[j].Y + adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y ), true),
                                    };
                                    PathFigure crossFigure = new PathFigure(crossStart, crossSegments, true);
                                    PathGeometry crossGeo = new PathGeometry(new PathFigure[] { crossFigure });
                                    context.DrawGeometry(adornBr, null, crossGeo);
                                    break;

                            }
                            Point point = new Point(pp[j].X - text.Width / 2, (pp[j].Y - text.Height / 2));

                            double xPt = Math.Round(point.X, 3);
                            double yPt = Math.Round(point.Y, 3);

                            if (xPt == -Math.Round((text.Width / 2), 3))
                                point.X += (text.Width / 2);
                            else if (xPt == Math.Round((width - (text.Width / 2)), 3))
                                point.X -= (text.Width / 2);
                            if (yPt == -Math.Round((text.Height / 2), 3))
                                point.Y += (text.Height / 2);
                            else if (yPt == Math.Round((height - (text.Height / 2)), 3))
                                point.Y -= (text.Height / 2);
                            context.DrawText(text, point);
                        }
                    }
                }

                base.VisualCollection.Add(visual);
            }

            base.OnRender(drawingContext);
        }

    }
}
