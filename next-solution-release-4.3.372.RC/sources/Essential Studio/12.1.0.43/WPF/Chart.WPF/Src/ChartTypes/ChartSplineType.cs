// <copyright file="ChartSplineType.cs" company="Syncfusion">
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
    using System.Windows.Data;
    using System.Windows.Shapes;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents spline chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartSplineType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSplineSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ChartSplineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ChartSplineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ChartSplineSegment), new PropertyMetadata(0d));
      
        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ChartSplineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartSplineSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the point1.
        /// </summary>
        /// <value>The point1.</value>
        public IChartDataPoint Point1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point2.
        /// </summary>
        /// <value>The point2.</value>
        public IChartDataPoint Point2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point3.
        /// </summary>
        /// <value>The point3.</value>
        public IChartDataPoint Point3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point4.
        /// </summary>
        /// <value>The point4.</value>
        public IChartDataPoint Point4
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the x1 point. This is a dependency property.
        /// </summary>
        /// <value>The x1 value.</value>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the x2 point. This is a dependency property.
        /// </summary>
        /// <value>The x2 value.</value>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the y1 point. This is a dependency property.
        /// </summary>
        /// <value>The y1 value.</value>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the y2 point. This is a dependency property.
        /// </summary>
        /// <value>The y2 value.</value>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
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

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartSplineSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is being assigned automatically.
        /// </remarks>
        static ChartSplineSegment()
        {
            Type type = typeof(ChartSplineSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSplineSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <param name="point4">The point4.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="correspondingPoint2">The corresponding point2.</param>
        /// <param name="series">The series.</param>
        internal ChartSplineSegment(IChartDataPoint point1, IChartDataPoint point2, IChartDataPoint point3, IChartDataPoint point4, ChartIndexedDataPoint correspondingPoint1, ChartIndexedDataPoint correspondingPoint2, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint1, correspondingPoint2 })
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
            Point4 = point4;

            this.SetXRange(Point1.X, Point4.X);
            yRange = GetSplineRange(Point1.X, Point1.Values[0], Point2.X, Point2.Values[0], Point3.X, Point3.Values[0], Point4.X, Point4.Values[0]);
            if (this.Series.Area.EnableDepthAxis && point1.Values.Length > 1 && point4.Values.Length > 1)
                this.SetZRange(point1.Values[1], point4.Values[1]);
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
            Point point1 = transformer.TransformToVisible(Point1.X, Point1.Y);
            Point point2 = transformer.TransformToVisible(Point2.X, Point2.Y);
            Point point3 = transformer.TransformToVisible(Point3.X, Point3.Y);
            Point point4 = transformer.TransformToVisible(Point4.X, Point4.Y);
            if (this.X1 != point1.X || X2 != point4.X || Y1 != point1.Y || Y2 != point4.Y)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = point1;
                double gapCount;
                ////condition check for BreakLineForNonIndexedData
                if (ChartSplineType.GetBreakLineForNonIndexedData(this.Series) && !this.Series.IsIndexed && !this.Series.Area.View3DMode)
                {
                    if (this.Series.ActualXAxis.ValueType == ChartValueType.Double)
                    {
                        gapCount = ChartSplineType.GetBreakLineForDoublePointsDistanceMoreThan(this.Series);
                    }
                    else
                    {
                        gapCount = (DateTime.Now + (TimeSpan)ChartSplineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(this.Series)).ToOADate() - DateTime.Now.ToOADate();
                    }

                    if (gapCount > 0)
                    {
                        if (Point4.X - Point1.X <= gapCount)
                        {
                            figure.Segments.Add(new BezierSegment(point2, point3, point4, true));
                        }
                        else
                        {
                            figure.Segments.Add(new BezierSegment(point2, point3, point4, false));
                        }
                    }
                }
                else
                {
                    figure.Segments.Add(new BezierSegment(point2, point3, point4, true));
                }

                this.Geometry = new PathGeometry(new PathFigure[] { figure });
                this.X1 = point1.X;
                this.X2 = point4.X;
                this.Y1 = point1.Y;
                this.Y2 = point4.Y;
            }
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            Point3D point1 = transformer.TransformToVisible(Point1.X, Point1.Y, this.Series.Area.EnableDepthAxis && Point1.Values.Length > 1 ? Point1.Values[1] : 0.06);
            Point3D point2 = transformer.TransformToVisible(Point2.X, Point2.Y, this.Series.Area.EnableDepthAxis && Point2.Values.Length > 1 ? Point2.Values[1] : 0.06);
            Point3D point3 = transformer.TransformToVisible(Point3.X, Point3.Y, this.Series.Area.EnableDepthAxis && Point3.Values.Length > 1 ? Point3.Values[1] : 0.06);
            Point3D point4 = transformer.TransformToVisible(Point4.X, Point4.Y, this.Series.Area.EnableDepthAxis && Point4.Values.Length > 1 ? Point4.Values[1] : 0.06);
            point1.Y = 1 - point1.Y;
            point2.Y = 1 - point2.Y;
            point3.Y = 1 - point3.Y;
            point4.Y = 1 - point4.Y;

            GeometryModel3D model = new GeometryModel3D();

            model.Geometry = MeshGenerator.SplineSegment(point1, point2, point3, point4);

            MaterialGroup materialGroup;

            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
            Binding binding = new Binding("Interior");
            binding.Source = Series;
            bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
            if (!colorEachValue)
                BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
            else
                this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
            materialGroup = new MaterialGroup();
            materialGroup.Children.Add(difuseMaterial);

            model.Material = materialGroup;
            model.Transform = new TranslateTransform3D(-0.5, -0.5, 0.5);

            Geometry3DGroup.Children.Add(model);
        }

        /// <summary>
        /// Gets the spline range.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="x3">The x3 value.</param>
        /// <param name="y3">The y3 value.</param>
        /// <param name="x4">The x4 value.</param>
        /// <param name="y4">The y4 value.</param>
        /// <returns>The SplineRange</returns>
        internal protected DoubleRange GetSplineRange(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            DoubleRange range = DoubleRange.Union(new double[] { y1, y2, y3, y4 });

            double cx = 3 * (x2 - x1);
            double cy = 3 * (y2 - y1);

            double bx = 3 * (x3 - x2) - cx;
            double by = 3 * (y3 - y3) - cy;

            double ay = y4 - y1 - by - cy;

            double r1, r2;

            if (ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2))
            {
                if (r1 >= 0 && r1 <= 1)
                {
                    double y = ay * r1 * r1 * r1 + by * r1 * r1 + cy * r1 + y1;
                    range = DoubleRange.Union(range, y);
                }

                if (r2 >= 0 && r2 <= 1)
                {
                    double y = ay * r2 * r2 * r2 + by * r2 * r2 + cy * r2 + y1;
                    range = DoubleRange.Union(range, y);
                }
            }

            return range;
        }
        #endregion
    }

    /// <summary>
    /// Represents spline chart type.
    /// </summary>
    /// <remarks>
    /// Spline Chart is similar to a Line Chart except that it connects the different
    /// data points using splines instead of straight lines.
    /// </remarks>
    /// <seealso cref="ChartSplineSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSplineType : ChartType
    {
        #region Attached properties
        
        /// <summary>
        /// Gets the break line for non indexed data.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>The bool value for BreakLineForNonIndexedData</returns>
        public static bool GetBreakLineForNonIndexedData(DependencyObject obj)
        {
            return (bool)obj.GetValue(BreakLineForNonIndexedDataProperty);
        }

        /// <summary>
        /// Sets the break line for non indexed data.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <seealso cref="ChartSplineType"/>
        public static void SetBreakLineForNonIndexedData(DependencyObject obj, bool value)
        {
            obj.SetValue(BreakLineForNonIndexedDataProperty, value);
        }

        /// <summary>
        /// Gets the break line for double points distance more than.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>Break line for Double points distance</returns>
        public static double GetBreakLineForDoublePointsDistanceMoreThan(DependencyObject obj)
        {
            return (double)obj.GetValue(BreakLineForDoublePointsDistanceMoreThanProperty);
        }
        
        /// <summary>
        /// Sets the break line for double points distance more than.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        /// <seealso cref="ChartSplineType"/>
        public static void SetBreakLineForDoublePointsDistanceMoreThan(DependencyObject obj, double value)
        {
            obj.SetValue(BreakLineForDoublePointsDistanceMoreThanProperty, value);
        }
        
        /// <summary>
        /// Gets the break line for time span points distance more than.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>Break line for TimeSpan points distance</returns>
        public static TimeSpan GetBreakLineForTimeSpanPointsDistanceMoreThan(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(BreakLineForTimeSpanPointsDistanceMoreThanProperty);
        }

        /// <summary>
        /// Sets the break line for time span points distance more than.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        /// <seealso cref="ChartSplineType"/>
        public static void SetBreakLineForTimeSpanPointsDistanceMoreThan(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(BreakLineForTimeSpanPointsDistanceMoreThanProperty, value);
        }

        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Splinecoefficient.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SplineCoefficientProperty =
          DependencyProperty.RegisterAttached("SplineCoefficient", typeof(double), typeof(ChartSplineAreaType), new ChartPropertyMetadata(1d, ChartPropertyMetadataOptions.AffectsUpdate));
      
        /// <summary>
        /// Identifies the BreakLineForNonIndexedData dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForNonIndexedDataProperty =
          DependencyProperty.RegisterAttached("BreakLineForNonIndexedData", typeof(bool), typeof(ChartSplineType), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsUpdate));
      
        /// <summary>
        /// Identifies the BreakLineForDoublePointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForDoublePointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForDoublePointsDistanceMoreThan", typeof(double), typeof(ChartSplineType), new ChartPropertyMetadata(0d, ChartPropertyMetadataOptions.AffectsUpdate));
     
        /// <summary>
        /// Identifies the BreakLineForTimeSpanPointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForTimeSpanPointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForTimeSpanPointsDistanceMoreThan", typeof(TimeSpan), typeof(ChartSplineType), new ChartPropertyMetadata(new TimeSpan(), ChartPropertyMetadataOptions.AffectsUpdate));

        #endregion

        #region Properties
        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None | ChartTypeFlags.Indexed;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Gets the spline coefficient.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The SplineCoefficient</returns>
        protected static double GetSplineCoefficient(ChartSeries series)
        {
            return (double)series.GetValue(SplineCoefficientProperty);
        }

        /// <summary>
        /// Sets the spline coefficient.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        protected static void SetSplineCoefficient(ChartSeries series, double value)
        {
            series.SetValue(SplineCoefficientProperty, value);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            bool dotSegmentRequired = ChartLineType.GetBreakLineForNonIndexedData(series) && !series.IsIndexed && !series.Area.View3DMode;
            double gapCount = 0;
            if (dotSegmentRequired)
            {
                if (series.ActualXAxis.ValueType == ChartValueType.Double)
                {
                    gapCount = ChartSplineType.GetBreakLineForDoublePointsDistanceMoreThan(series);
                }
                else
                {
                    gapCount = (DateTime.Now + (TimeSpan)ChartSplineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(series)).ToOADate() - DateTime.Now.ToOADate();
                }
            }

            dotSegmentRequired = dotSegmentRequired && (gapCount > 0);
            
            double[] yCoef;
            if (points.Length >= 2)
            {
                NaturalSpline(points, out yCoef);

                for (int i = 1, count = points.Length; i < count; i++)
                {
                    IChartDataPoint startPoint = points[i - 1].DataPoint;
                    IChartDataPoint endPoint = points[i].DataPoint;

                    ChartPoint startControlPoint = null;
                    ChartPoint endControlPoint = null;

                    GetBezierControlPoints(startPoint, endPoint, yCoef[i - 1], yCoef[i], out startControlPoint, out endControlPoint);
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                            {
                                if (i == 1 && points[0].DataPoint.EmptyPoint)
                                {
                                    series.Segments.Add(new ChartEmptySymbolSegment(points[i - 1].DataPoint, points[i - 1], series, series.EmptyPointSymbolTemplate));
                                }
                                series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                     
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                series.Segments.Add(new ChartSplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, points[i - 1], points[i], series));
                                series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;                               
                            }
                            else 
                            {
                                if (i == 1 && points[0].DataPoint.EmptyPoint)
                                {
                                    series.Segments.Add(new ChartEmptySymbolSegment(points[i - 1].DataPoint, points[i - 1], series, series.EmptyPointSymbolTemplate));
                                }
                                series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                     
                            }
                        }
                        else
                        {
                           var segment=new ChartSplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, points[i - 1], points[i], series);
                           segment.Interior = Brushes.Transparent;
                           segment.Stroke = Brushes.Transparent;
                           series.Segments.Add(segment);
                        }
                    }
                    else
                    {
                        series.Segments.Add(new ChartSplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, points[i - 1], points[i], series));
                        if (points[i-1].DataPoint.EmptyPoint && (series.ShowEmptyPoints) && series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                        }
                        else if (points[i - 1].DataPoint.EmptyPoint && !(series.ShowEmptyPoints))
                        {
                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                        }
                        else if (points[i - 1].DataPoint.EmptyPoint && (series.EmptyPointStyle == EmptyPointStyle.Symbol ||series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior ))
                        {
                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                        }
                        //if (i > 2)
                        //{
                        //    if (points[i - 2].DataPoint.EmptyPoint && (series.ShowEmptyPoints) && series.EmptyPointStyle == EmptyPointStyle.Symbol && series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        //    {
                        //        series.Segments[series.Segments.Count - 2].Interior = Brushes.Transparent;
                        //    }
                        //}
                    }

                    ////Add dangling points in missing segments
                    if (dotSegmentRequired && i + 1 <= points.Length)
                    {
                        if (points[i].DataPoint.X - points[i - 1].DataPoint.X > gapCount)
                        {
                            if (points[i + 1].DataPoint.X - points[i].DataPoint.X > gapCount)
                            {
                                ChartIndexedDataPoint indexedPoint2 = points[i];
                                series.Segments.Add(new ChartSymbolSegment(indexedPoint2.DataPoint, indexedPoint2, series));
                            }
                        }
                    }
                }

                if (series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                         //Add for SD11357
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
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartSplineType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartSplineType"/>
        public override string ToString()
        {
            return "Spline";
        }
        #endregion

        #region Implementation
        
        /// <summary>
        /// Naturals the spline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="ys2">The ys2 value.</param>
        internal static void NaturalSpline(ChartIndexedDataPoint[] points, out double[] ys2)
        {
            int count = points.Length;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count - 1];
            double p;

            ys2[0] = u[0] = 0;
            ys2[count - 1] = 0;

            for (int i = 1; i < count - 1; i++)
            {
                double d1 = points[i].DataPoint.X - points[i - 1].DataPoint.X;
                double d2 = points[i + 1].DataPoint.X - points[i - 1].DataPoint.X;
                double d3 = points[i + 1].DataPoint.X - points[i].DataPoint.X;
                double dy1 = points[i + 1].DataPoint.Y - points[i].DataPoint.Y;
                double dy2 = points[i].DataPoint.Y - points[i - 1].DataPoint.Y;

                if (points[i].DataPoint.X == points[i - 1].DataPoint.X || points[i].DataPoint.X == points[i + 1].DataPoint.X)
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
}
