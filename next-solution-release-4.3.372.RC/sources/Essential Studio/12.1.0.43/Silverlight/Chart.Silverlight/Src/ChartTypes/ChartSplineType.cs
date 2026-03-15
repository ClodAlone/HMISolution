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
    /// Represents spline chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartSplineType"/>
    public class ChartSplineSegment : Segment
    {
        #region Dependency properties
        /// <summary>
        ///  Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
    DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartSplineSegment), new PropertyMetadata(null));

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
        /// Get or Set Template property
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the point1.
        /// </summary>
        /// <value>The point1.</value>
        public ChartPoint Point1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point2.
        /// </summary>
        /// <value>The point2.</value>
        public ChartPoint Point2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point3.
        /// </summary>
        /// <value>The point3.</value>
        public ChartPoint Point3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point4.
        /// </summary>
        /// <value>The point4.</value>
        public ChartPoint Point4
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
        /// Initializes a new instance of the <see cref="ChartSplineSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <param name="point4">The point4.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="correspondingPoint2">The corresponding point2.</param>
        /// <param name="series">The series.</param>
        internal ChartSplineSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, ChartPoint correspondingPoint1, ChartPoint correspondingPoint2, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint1, correspondingPoint2 })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartSplineType), ChartTypes.Spline);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = this.Template;
                this.SegmentTemplate = this.Template;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
            Point4 = point4;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            double xval = series.Type == ChartTypes.Spline ? (series.XAxis.VisibleRange.Start * (-1)) : (series.YAxis.VisibleRange.Start * (-1));
            double yval = series.Type == ChartTypes.Spline ? (series.YAxis.VisibleRange.Start * (-1)) : (series.XAxis.VisibleRange.Start * (-1));

            Point point1 = transformer.TransformToVisible(Point1.X + xval, Point1.Y + yval, series);
            Point point2 = transformer.TransformToVisible(Point2.X + xval, Point2.Y + yval, series);
            Point point3 = transformer.TransformToVisible(Point3.X + xval, Point3.Y + yval, series);
            Point point4 = transformer.TransformToVisible(Point4.X + xval, Point4.Y + yval, series);
            if (this.X1 != point1.X || X2 != point4.X || Y1 != point1.Y || Y2 != point4.Y)
            {
                PathFigure figure = new PathFigure();
                PathFigureCollection figures = new PathFigureCollection();
                figure.StartPoint = point1;
                figure.Segments.Add(new BezierSegment() { Point1 = point3, Point2 = point4, Point3 = point2 });
                figures.Add(figure);
                this.Geometry = new PathGeometry() { Figures = figures };
                this.X1 = point1.X;
                this.X2 = point4.X;
                this.Y1 = point1.Y;
                this.Y2 = point4.Y;
            }
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

            double bx = (3 * (x3 - x2)) - cx;
            double by = (3 * (y3 - y3)) - cy;

            double ay = y4 - y1 - by - cy;

            double r1, r2;

            if (ChartPyramidType.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2))
            {
                if (r1 >= 0 && r1 <= 1)
                {
                    double y = (ay * r1 * r1 * r1) + (by * r1 * r1) + (cy * r1) + y1;
                    range = DoubleRange.Union(range, y);
                }

                if (r2 >= 0 && r2 <= 1)
                {
                    double y = (ay * r2 * r2 * r2) + (by * r2 * r2) + (cy * r2) + y1;
                    range = DoubleRange.Union(range, y);
                }
            }

            return range;
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.Point1 = null;
            this.Point2 = null;
            this.Point3 = null;
            this.Point4 = null;
        }
    }

    /// <summary>
    /// Represents spline chart type.
    /// </summary>
    /// <remarks>
    /// Spline Chart is similar to a Line Chart except that it connects the different
    /// data points using splines instead of straight lines.
    /// </remarks>
    /// <seealso cref="ChartSplineSegment"/>
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
          DependencyProperty.RegisterAttached("SplineCoefficient", typeof(double), typeof(ChartSplineType), new PropertyMetadata(1d));

        /// <summary>
        /// Identifies the BreakLineForNonIndexedData dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForNonIndexedDataProperty =
          DependencyProperty.RegisterAttached("BreakLineForNonIndexedData", typeof(bool), typeof(ChartSplineType), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the BreakLineForDoublePointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForDoublePointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForDoublePointsDistanceMoreThan", typeof(double), typeof(ChartSplineType), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BreakLineForTimeSpanPointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForTimeSpanPointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForTimeSpanPointsDistanceMoreThan", typeof(TimeSpan), typeof(ChartSplineType), new PropertyMetadata(new TimeSpan()));

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
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
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
                for (int i = 1, count = points.Count; i < count; i++)
                {
                    if (double.IsNaN(points[i].Y))
                        points[i].EmptyPoint = true;
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
                }
                this.NaturalSpline(points, out yCoef);
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                    series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[0].X - 0.5, points[0].Y) : points[0], points, series, 0d));
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
                    ChartPoint point1 = new ChartPoint(startPoint.X , startPoint.Y );
                    ChartPoint point2 = new ChartPoint(endPoint.X, endPoint.Y);
                    ChartPoint point3 = new ChartPoint(startControlPoint.X , startControlPoint.Y);
                    ChartPoint point4 = new ChartPoint(endControlPoint.X , endControlPoint.Y);

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        point1.X -= 0.5;
                        point2.X -= 0.5;
                        point3.X -= 0.5;
                        point4.X -= 0.5;
                    }
                    if (points[i].EmptyPoint && series.ShowEmptyPoints == true )
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        {
                            series.Segments.Add(new ScatterSegment(new ChartPoint(points[i-1].X, points[i].Y), new ChartPoint(points[i-1].X, points[i].Y), points[i], series));
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

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
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
        protected void NaturalSpline(ChartPointsCollection points, out double[] ys2)
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
                if (points[i - 1].Y.Equals(double.NaN) || points[i].Y.Equals(double.NaN) || points[i - 1].X.Equals(double.NaN) || points[i].X.Equals(double.NaN) || points[i + 1].Y.Equals(double.NaN) || points[i + 1].X.Equals(double.NaN))
                {
                    points[i - 1].Y = points[i - 1].Y.Equals(double.NaN) ? 0 : points[i - 1].Y;
                    points[i].Y = points[i].Y.Equals(double.NaN) ? 0 : points[i].Y;
                    points[i + 1].Y = points[i + 1].Y.Equals(double.NaN) ? 0 : points[i + 1].Y;
                }
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
                    p = 1 / ((d1 * ys2[i - 1]) + (2 * d2));

                    ys2[i] = -p * d3;
                    u[i] = p * ((a * ((dy1 / d3) - (dy2 / d1))) - (d1 * u[i - 1]));
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = (ys2[k] * ys2[k + 1]) + u[k];
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
        protected void GetBezierControlPoints(ChartPoint point1, ChartPoint point2, double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;
            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = (2 * point1.X) + point2.X;
            double dx2 = point1.X + (2 * point2.X);

            double dy1 = (2 * point1.Y) + point2.Y;
            double dy2 = point1.Y + (2 * point2.Y);

            double y1 = One_thrid * (dy1 - (One_thrid * deltaX2 * (ys1 + (0.5f * ys2))));
            double y2 = One_thrid * (dy2 - (One_thrid * deltaX2 * ((0.5f * ys1) + ys2)));

            controlPoint1 = new ChartPoint(dx1 * One_thrid, y1);
            controlPoint2 = new ChartPoint(dx2 * One_thrid, y2);
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
