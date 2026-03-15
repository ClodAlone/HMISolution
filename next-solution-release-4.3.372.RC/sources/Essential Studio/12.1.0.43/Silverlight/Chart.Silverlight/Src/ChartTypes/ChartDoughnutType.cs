#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
    /// Represents Doughnut chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silvelright Chart building system.</remarks>
    public class ChartDoughnutSegment : ChartPieSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the DoughnutCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
            DependencyProperty.Register("DoughnutCoefficient", typeof(double), typeof(ChartDoughnutSegment), new PropertyMetadata(0.2d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the doughnut coefficient. This is a dependency property.
        /// </summary>
        /// <remarks>Represents doughnut coefficient that corresponds for interior hole radius.</remarks>
        /// <value>The doughnut coefficient.</value>
        public double DoughnutCoefficient
        {
            get { return (double)GetValue(DoughnutCoefficientProperty); }
            set { SetValue(DoughnutCoefficientProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDoughnutSegment"/> class.
        /// </summary>
        /// <param name="startAngle">The start angle</param>
        /// <param name="endAngle">The end angle</param>
        /// <param name="correspondingPoint">The corresponding point</param>
        /// <param name="series">The chart series</param>
        public ChartDoughnutSegment(double startAngle, double endAngle, ChartPoint correspondingPoint, ChartSeries series)
            : base(startAngle, endAngle, correspondingPoint, series)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <remarks>Method is being called internally in order to update segment.</remarks>
        public override void Update(IChartTransformer transformer)
        {
            double[] segRadius = new double[Series.Area.Series.Count];
            for (int i = 0; i < Series.Area.Series.Count; i++)
            {
                if (i == 0)
                    segRadius[i] = Math.Pow(2, Series.Area.Series.Count);
                else
                    segRadius[i] = segRadius[i - 1] / 2;
            }
            double radius = 0.8 * transformer.GetMinimumValue() / segRadius[this.Series.Area.Series.IndexOf(Series)];
            double dradius = this.DoughnutCoefficient * radius;

            CenterPoint = transformer.GetCenterPoint();
            if (this.IsExploded)
            {
                actualCenterPoint = CenterPoint;
                CenterPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + ExplodeRadius, CenterPoint.Y), AngleOfSlice);
            }

            startPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + radius, CenterPoint.Y), StartAngle);
            endPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + radius, CenterPoint.Y), EndAngle);

            if (isSinglePoint || (EndAngle - StartAngle) == 360)
            {
                GeometryGroup geometryGroup = new GeometryGroup();
                geometryGroup.FillRule = FillRule.EvenOdd;
                EllipseGeometry ellipse = new EllipseGeometry();
                ellipse.Center = CenterPoint;
                ellipse.RadiusX = ellipse.RadiusY = radius;
                EllipseGeometry ellipse1 = new EllipseGeometry();
                ellipse1.Center = CenterPoint;
                ellipse1.RadiusX = ellipse1.RadiusY = dradius; 
                geometryGroup.Children.Add(ellipse);
                geometryGroup.Children.Add(ellipse1);
                this.GeometryPoints = geometryGroup;

                GeometryGroup clipgeometryGroup = new GeometryGroup();
                clipgeometryGroup.FillRule = FillRule.EvenOdd;
                EllipseGeometry clipellipse = new EllipseGeometry();
                clipellipse.Center = CenterPoint;
                clipellipse.RadiusX = clipellipse.RadiusY = radius;
                EllipseGeometry clipellipse1 = new EllipseGeometry();
                clipellipse1.Center = CenterPoint;
                clipellipse1.RadiusX = clipellipse1.RadiusY = dradius;
                clipgeometryGroup.Children.Add(clipellipse);
                clipgeometryGroup.Children.Add(clipellipse1);
                this.ClippingPoints = clipgeometryGroup;
            }

            if (series.EnableEffects==true)
            {
                double startx = CenterPoint.X - radius;
                double starty = CenterPoint.Y - radius;
                double width = radius * 2d;
                PathGeometry externalpath = new PathGeometry();
                PathFigureCollection expfc = new PathFigureCollection();
                PathFigure expf = new PathFigure();

                expf.StartPoint = new Point(startx, starty);
                expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty) });
                expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty + width) });
                expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty + width) });
                expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty) });

                RadialGradientBrush brush = ResourceManager.GetSeriesLayer(typeof(ChartPieType), "doughnutlayer") as RadialGradientBrush;
                brush.GradientStops[2].Offset = (brush.GradientStops[2].Offset / 0.5d) * this.DoughnutCoefficient;
                brush.GradientStops[3].Offset = (brush.GradientStops[3].Offset / 0.5d) * this.DoughnutCoefficient;
                brush.GradientStops[1].Offset = (brush.GradientStops[0].Offset + brush.GradientStops[2].Offset) / 2d;
                this.LayerBrush = brush;
                expfc.Add(expf);
                externalpath.Figures = expfc;

                if (StartAngle != EndAngle)
                {
                    this.ExternalPoints = externalpath;
                }
            }

            PathGeometry pg = new PathGeometry();
            if (startPoint != endPoint && (m_startPoint != startPoint || m_endPoint != endPoint) && !isSinglePoint)
            {
                startDPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + dradius, CenterPoint.Y), StartAngle);
                endDPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + dradius, CenterPoint.Y), EndAngle);

                PathFigure figure = new PathFigure();
                figure.StartPoint = startPoint;

                arcseg = new ArcSegment();
                arcseg.Point = endPoint;
                arcseg.Size = new Size(radius, radius);
                arcseg.RotationAngle = EndAngle - StartAngle;
                arcseg.IsLargeArc = EndAngle - StartAngle > 180;
                arcseg.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(arcseg);

                lineseg = new System.Windows.Media.LineSegment();
                lineseg.Point = endDPoint;
                figure.Segments.Add(lineseg);

                doughnutArgSeg = new ArcSegment();
                doughnutArgSeg.Point = startDPoint;
                doughnutArgSeg.Size = new Size(dradius, dradius);
                doughnutArgSeg.RotationAngle = EndAngle - StartAngle;
                doughnutArgSeg.IsLargeArc = EndAngle - StartAngle > 180;
                doughnutArgSeg.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(doughnutArgSeg);

                figure.IsClosed = true;
                pg.Figures.Add(figure);
                this.GeometryPoints = pg;
                ////m_startPoint = startPoint;
                ////m_endPoint = endPoint;
            }

            PathGeometry clippingpg = new PathGeometry();
            if (startPoint != endPoint && (m_startPoint != startPoint || m_endPoint != endPoint) && !isSinglePoint && series.EnableEffects==true)
            {
                Point startDPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + dradius, CenterPoint.Y), StartAngle);
                Point endDPoint = Rotation.GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + dradius, CenterPoint.Y), EndAngle);

                PathFigure figure = new PathFigure();
                figure.StartPoint = startPoint;

                ArcSegment arc = new ArcSegment();
                arc.Point = endPoint;
                arc.Size = new Size(radius, radius);
                arc.RotationAngle = EndAngle - StartAngle;
                arc.IsLargeArc = EndAngle - StartAngle > 180;
                arc.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(arc);

                System.Windows.Media.LineSegment lineseg = new System.Windows.Media.LineSegment();
                lineseg.Point = endDPoint;
                figure.Segments.Add(lineseg);

                ArcSegment arc1 = new ArcSegment();
                arc1.Point = startDPoint;
                arc1.Size = new Size(dradius, dradius);
                arc1.RotationAngle = EndAngle - StartAngle;
                arc1.IsLargeArc = EndAngle - StartAngle > 180;
                arc1.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(arc1);

                figure.IsClosed = true;
                clippingpg.Figures.Add(figure);
                this.ClippingPoints = clippingpg;
                m_startPoint = startPoint;
                m_endPoint = endPoint;
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

    /// <summary>
    /// Represents ChartDoughnutType class
    /// </summary>
    /// <remarks>
    /// Doughnut charts are pie charts with a hole, whose value is specified as the
    /// doughnut coefficient. The Doughnut Chart is best suited for presenting data in
    /// proportions.
    /// </remarks>
    /// <seealso cref="ChartDoughnutSegment"/>
    public class ChartDoughnutType : ChartPieType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the DoughnutCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
      DependencyProperty.RegisterAttached("DoughnutCoefficient", typeof(double), typeof(ChartDoughnutType), new PropertyMetadata(0.2d, new PropertyChangedCallback(OnCoefficientChanged)));
        #endregion

        ///// <summary>
        ///// Initializes a new instance of the ChartDoughnutType class. 
        ///// </summary>
        //internal ChartDoughnutType()
        //{
        //}

        #region Public methods
        /// <summary>
        /// Gets the doughnut coefficient.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Doughnut Coefficient</returns>
        public static double GetDoughnutCoefficient(ChartSeries series)
        {
            return (double)series.GetValue(DoughnutCoefficientProperty);
        }

        /// <summary>
        /// Sets the doughnut coefficient.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetDoughnutCoefficient(ChartSeries series, double value)
        {
            series.SetValue(DoughnutCoefficientProperty, value);
        }
        #endregion

        #region Implementation
        private static void OnCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Doughnut";
        }

        /// <summary>
        /// Creates the segment.
        /// </summary>
        /// <param name="startAngle">The start angle</param>
        /// <param name="endAngle">The end angle</param>
        /// <param name="correspondingPoint">The corresponding point</param>
        /// <param name="series">The chart series</param>
        /// <returns>The segment</returns>
        protected override ChartPieSegment CreateSegment(double startAngle, double endAngle, ChartPoint correspondingPoint, ChartSeries series)
        {
            ChartDoughnutSegment segment = new ChartDoughnutSegment(startAngle, endAngle, correspondingPoint, series);

            segment.DoughnutCoefficient = GetDoughnutCoefficient(series);

            return segment;
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
