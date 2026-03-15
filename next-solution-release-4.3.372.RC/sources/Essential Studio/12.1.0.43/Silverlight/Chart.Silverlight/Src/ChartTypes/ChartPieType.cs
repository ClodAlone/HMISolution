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
using System.Collections;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class represent Pie-typed adornment. 
    /// </summary>
    /// <remarks>
    /// This adornment type is specific for chart types as Pie, Doughnut, Funnel and Pyramid.
    /// Class instances are created internally and no user code is required except special cases.
    /// </remarks>
    public class ChartPieAdornment : ChartAdornment
    {
        #region Members
        /// <summary>
        /// Initializes m_angle
        /// </summary>
        internal double m_angle = 0;
        internal double m_endangle = 0;
        internal double m_startangle = 0;
        /// <summary>
        /// Initializes m_radius
        /// </summary>
        private double m_radius = 0;
       
       

        #endregion


        /// <summary>
        /// Return the AdornmentMode Value from the given ChartAdornmentInfo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AdornmentMode GetAdornmentMode(ChartAdornmentInfo obj)
        {
            return (AdornmentMode)obj.GetValue(AdornmentModeProperty);
        }

        /// <summary>
        /// Set AdornmentMode to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAdornmentMode(DependencyObject obj, AdornmentMode value)
        {
            obj.SetValue(AdornmentModeProperty, value);
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for PieAdornmentMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AdornmentModeProperty =
            DependencyProperty.RegisterAttached("AdornmentMode", typeof(AdornmentMode), typeof(ChartAdornmentInfo), new PropertyMetadata(AdornmentMode.Horizontal));



        /// <summary>
        /// Gets or sets the label angle.
        /// </summary>
        /// <value>The label angle.</value>
        public double LabelAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connector is on top.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connector on top; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnectorOnTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connector alignment.
        /// </summary>
        /// <value>The connector alignment.</value>
        public ConnectorAlignment ConnectorAlignment
        {
            get;
            set;
        }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPieAdornment"/> class.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="points">The points collection.</param>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        /// <param name="startangle">The start angle.</param>
        /// <param name="endangle">The end angle.</param>
        public ChartPieAdornment(double angle, double radius, ChartPointsCollection points, ChartPoint point, ChartSeries series, double startangle, double endangle)
            : base(point, points, series, 0d)
        {
            m_angle = angle;
            m_radius = radius;
            m_startangle = startangle;
            m_endangle = endangle;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            double exploderadius = ChartPieType.GetExplodeRadius(series);
            double radius = m_radius * transformer.GetMinimumValue() / 2;
            Point center = transformer.GetCenterPoint();
            if (ChartPieType.GetExplodedIndex(series) == this.index || ChartPieType.GetExplodedAll(series) == true)
            {
                center = GeneralPointRotation(center, new Point(center.X + exploderadius, center.Y), (m_startangle + m_endangle) / 2);
            }

            if (m_angle > 0 && m_angle < 90 || m_angle > 270 && m_angle < 360)
            {
                this.X = center.X + (radius * Math.Cos(m_angle * Math.PI / 180)) + this.series.AdornmentsInfo.SegmentLabelContent.ToString().Length;
                this.Y = center.Y + (radius * Math.Sin(m_angle * Math.PI / 180));
            }
            else
            {
                this.X = center.X + (radius * Math.Cos(m_angle * Math.PI / 180)) - this.series.AdornmentsInfo.SegmentLabelContent.ToString().Length;
                this.Y = center.Y + (radius * Math.Sin(m_angle * Math.PI / 180));
            }
        }

        Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
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
    /// Represents Pie chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public class ChartPieSegment : Segment
    {
        #region Constructor
        /// <summary>
        /// Called when instance created for ChartPieSegment with following arguments
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="correspondingPoint"></param>
        /// <param name="series"></param>
        public ChartPieSegment(double startAngle, double endAngle, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartPieType), ChartTypes.Pie);
                this.SegmentTemplate = series.ActualSegmentTemplate;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            StartAngle = startAngle;
            EndAngle = endAngle;
            AngleOfSlice = (startAngle + endAngle) / 2;
        }
        #endregion

        #region Members
        /// <summary>
        /// Declares isSinglePoint
        /// </summary>
        internal bool isSinglePoint;

        /// <summary>
        /// Represents the Start point
        /// </summary>
        protected Point m_startPoint;

        /// <summary>
        /// Represents the end point
        /// </summary>
        protected Point m_endPoint;

        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartPieSegment), new PropertyMetadata(ResourceManager.GetSeriesTemplate(typeof(ChartPieType), ChartTypes.Pie)));

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryPointsProperty =
            DependencyProperty.Register("GeometryPoints", typeof(Geometry), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsExploded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(ChartPieSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the ExplodedRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
            DependencyProperty.Register("ExplodeRadius", typeof(double), typeof(ChartPieSegment), new PropertyMetadata(10d));

        /// <summary>
        /// Identifies the AngleOfSlice dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleOfSliceProperty =
            DependencyProperty.Register("AngleOfSlice", typeof(double), typeof(ChartPieSegment), new PropertyMetadata(0d));

        #endregion

        #region Properties

        /// <summary>
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end angle.
        /// </summary>
        /// <value>The end angle.</value>
        public double EndAngle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>The start angle.</value>
        public double StartAngle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the geometry. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the geometry of the segment.</remarks>
        /// <value>The geometry.</value>
        public Geometry GeometryPoints
        {
            get { return (Geometry)GetValue(GeometryPointsProperty); }
            set { SetValue(GeometryPointsProperty, value); }
        }

        /// <summary>
        /// Get or Set AngleOfSliceProperty
        /// </summary>
        public double AngleOfSlice
        {
            get { return (double)GetValue(AngleOfSliceProperty); }
            set { SetValue(AngleOfSliceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is exploded. This is a dependency property.
        /// </summary>
        /// <remarks>Exploded segment is used to visually point required segment as exploded.</remarks>
        /// <value>
        /// <c>true</c> if this instance is exploded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExploded
        {
            get { return (bool)GetValue(IsExplodedProperty); }
            set { SetValue(IsExplodedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the explode radius. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the radius of exploded segment.</remarks>
        /// <value>The explode radius.</value>
        public double ExplodeRadius
        {
            get { return (double)GetValue(ExplodeRadiusProperty); }
            set { SetValue(ExplodeRadiusProperty, value); }
        }
        #endregion

        #region Additional Effects
        /// <summary>
        ///  Identifies the ClippingPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty ClippingPointsProperty =
            DependencyProperty.Register("ClippingPoints", typeof(Geometry), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Get or set ClippingPointsProperty
        /// </summary>
        public Geometry ClippingPoints
        {
            get { return (Geometry)GetValue(ClippingPointsProperty); }
            set { SetValue(ClippingPointsProperty, value); }
        }

        /// <summary>
        /// Identifies the ExternalPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty ExternalPointsProperty =
            DependencyProperty.Register("ExternalPoints", typeof(Geometry), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set ExternalPointsProperty
        /// </summary>
        public Geometry ExternalPoints
        {
            get { return (Geometry)GetValue(ExternalPointsProperty); }
            set { SetValue(ExternalPointsProperty, value); }
        }

        /// <summary>
        /// Identifies the LayerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LayerBrushProperty =
            DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set LayerBrush property
        /// </summary>
        public Brush LayerBrush
        {
            get { return (Brush)GetValue(LayerBrushProperty); }
            set { SetValue(LayerBrushProperty, value); }
        }
        #endregion

        #region implementation

        Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }

        internal Point startPoint, endPoint, CenterPoint, startDPoint, endDPoint, actualCenterPoint;
        internal ArcSegment arcseg, doughnutArgSeg;
        internal System.Windows.Media.LineSegment pielineseg, lineseg;

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            double[] segRadius = new double[Series.Area.Series.Count];
            for (int i = 0; i < Series.Area.Series.Count; i++)
            {
                if (i == 0)
                    segRadius[i] = Math.Pow(2, Series.Area.Series.Count);
                else
                    segRadius[i] = segRadius[i - 1] / 2;
            }

            if (this.Series.Area.Series.IndexOf(Series) == 0)
            {
                double radius = 0.8 * transformer.GetMinimumValue() / segRadius[0];
                CenterPoint = transformer.GetCenterPoint();

                if (this.IsExploded)
                {
                    actualCenterPoint = CenterPoint;
                    CenterPoint = GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + ExplodeRadius, CenterPoint.Y), AngleOfSlice);
                }

                startPoint = GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + radius, CenterPoint.Y), StartAngle);
                endPoint = GeneralPointRotation(CenterPoint, new Point(CenterPoint.X + radius, CenterPoint.Y), EndAngle);
                if (isSinglePoint || (EndAngle - StartAngle) == 360)
                {
                    EllipseGeometry ellipse = new EllipseGeometry();
                    ellipse.Center = CenterPoint;
                    ellipse.RadiusX = ellipse.RadiusY = radius;
                    this.GeometryPoints = ellipse;
                    this.ClippingPoints = new EllipseGeometry() { Center = CenterPoint, RadiusX = radius, RadiusY = radius };
                }

                if (series.EnableEffects == true)
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

                    this.LayerBrush = ResourceManager.GetSeriesLayer(typeof(ChartPieType), "pielayer");
                    expfc.Add(expf);
                    externalpath.Figures = expfc;
                    this.ExternalPoints = externalpath;
                }

                PathGeometry pg = new PathGeometry();
                PathFigureCollection fig = new PathFigureCollection();
                if (!isSinglePoint)
                {
                    if (startPoint != endPoint && EndAngle != StartAngle)
                    {
                        PathFigure figure = new PathFigure();
                        figure.StartPoint = CenterPoint;

                        pielineseg = new System.Windows.Media.LineSegment();
                        pielineseg.Point = startPoint;
                        figure.Segments.Add(pielineseg);

                        arcseg = new ArcSegment();
                        arcseg.Point = endPoint;
                        arcseg.Size = new Size(radius, radius);
                        arcseg.RotationAngle = EndAngle - StartAngle;
                        arcseg.IsLargeArc = EndAngle - StartAngle > 180;
                        arcseg.SweepDirection = SweepDirection.Clockwise;
                        figure.Segments.Add(arcseg);

                        System.Windows.Media.LineSegment line2seg = new System.Windows.Media.LineSegment();
                        line2seg.Point = CenterPoint;
                        figure.Segments.Add(line2seg);
                        figure.IsClosed = true;

                        fig.Add(figure);
                        m_startPoint = startPoint;
                        m_endPoint = endPoint;
                    }
                    else
                    {
                        this.GeometryPoints = null;
                    }

                    pg.Figures = fig;
                    this.GeometryPoints = pg;
                }

                PathGeometry clippingpg = new PathGeometry();
                PathFigureCollection clippingfig = new PathFigureCollection();
                if (!isSinglePoint && series.EnableEffects == true)
                {
                    if (startPoint != endPoint && EndAngle != StartAngle)
                    {
                        PathFigure figure = new PathFigure();
                        figure.StartPoint = CenterPoint;

                        pielineseg = new System.Windows.Media.LineSegment();
                        pielineseg.Point = startPoint;
                        figure.Segments.Add(pielineseg);

                        ArcSegment arcseg = new ArcSegment();
                        arcseg.Point = endPoint;
                        arcseg.Size = new Size(radius, radius);
                        arcseg.RotationAngle = EndAngle - StartAngle;
                        arcseg.IsLargeArc = EndAngle - StartAngle > 180;
                        arcseg.SweepDirection = SweepDirection.Clockwise;
                        figure.Segments.Add(arcseg);

                        System.Windows.Media.LineSegment line2seg = new System.Windows.Media.LineSegment();
                        line2seg.Point = CenterPoint;
                        figure.Segments.Add(line2seg);
                        figure.IsClosed = true;

                        clippingfig.Add(figure);
                        m_startPoint = startPoint;
                        m_endPoint = endPoint;
                    }
                    else
                    {
                        this.ClippingPoints = null;
                    }

                    clippingpg.Figures = clippingfig;
                    this.ClippingPoints = clippingpg;
                }
            }
            else
            {
                DrawMultiplePie(transformer, segRadius[this.Series.Area.Series.IndexOf(Series)]);
            }
        }

        /// <summary>
        /// Method implementation for draw more pie segments in chart
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="segradius"></param>
        public void DrawMultiplePie(IChartTransformer transformer, double segradius)
        {
            double radius = 0.8 * transformer.GetMinimumValue() / segradius;
            double dradius = ChartPieType.GetPieCoefficient(this.series) * radius;

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

            if (series.EnableEffects == true)
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

                RadialGradientBrush brush = ResourceManager.GetSeriesLayer(typeof(ChartPieType), "pielayer") as RadialGradientBrush;
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
            if (startPoint != endPoint && (m_startPoint != startPoint || m_endPoint != endPoint) && !isSinglePoint && series.EnableEffects == true)
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
            LayerBrush = null;
        }
    }

    /// <summary>
    /// Represents Pie Chart Type class
    /// </summary>
    /// <remarks>
    /// A Pie Chart renders Y values as slices in a pie. These slices are rendered in
    /// proportion to the whole which is simply the sum of all the Y values in the
    /// series. Consequently, Pie Charts are used to visualize the proportional
    /// contribution (in terms of percentage or fraction) of categories of data to the
    /// whole data set. The X values in the data series will only be treated as nominal
    /// (categorical, qualitative) data. The Pie Chart can display only one DataSeries
    /// at a time.
    /// </remarks>
    /// <seealso cref="ChartPieSegment"/>
    public class ChartPieType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedIndexProperty =
          DependencyProperty.RegisterAttached("ExplodedIndex", typeof(int), typeof(ChartPieType), new PropertyMetadata(-1, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the ExplodedAll dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedAllProperty =
          DependencyProperty.RegisterAttached("ExplodedAll", typeof(bool), typeof(ChartPieType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the ExplodeRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
          DependencyProperty.RegisterAttached("ExplodeRadius", typeof(double), typeof(ChartPieType), new PropertyMetadata(10d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the StartAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
  DependencyProperty.RegisterAttached("StartAngle", typeof(double), typeof(ChartPieType), new PropertyMetadata(0d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the EndAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
DependencyProperty.RegisterAttached("EndAngle", typeof(double), typeof(ChartPieType), new PropertyMetadata(360d, new PropertyChangedCallback(OnDataChanged)));
        /// <summary>
        /// Identifies the PieCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty PieCoefficientProperty =
DependencyProperty.RegisterAttached("PieCoefficient", typeof(double), typeof(ChartPieType), new PropertyMetadata(0.6d, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the StartAngle Value of Pie / Doughnut.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Get Start Angle Value</returns>
        public static double GetStartAngle(ChartSeries series)
        {
            return (double)series.GetValue(StartAngleProperty);
        }

        /// <summary>
        /// Sets the StartAngle Value of Pie / Doughnut.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">Set Start Angle Value</param>
        public static void SetStartAngle(ChartSeries series, double value)
        {
            series.SetValue(StartAngleProperty, value);
        }

        /// <summary>
        /// Gets the StartAngle Value of Pie / Doughnut.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Get Start Angle Value</returns>
        public static double GetEndAngle(ChartSeries series)
        {
            return (double)series.GetValue(EndAngleProperty);
        }

        /// <summary>
        /// Sets the EndAngle Value of Pie / Doughnut.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">Set Start Angle Value</param>
        public static void SetEndAngle(ChartSeries series, double value)
        {
            series.SetValue(EndAngleProperty, value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the exploded all.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Bool value true to explode all</returns>
        public static bool GetExplodedAll(ChartSeries series)
        {
            return (bool)series.GetValue(ExplodedAllProperty);
        }

        /// <summary>
        /// Sets the exploded all.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetExplodedAll(ChartSeries series, bool value)
        {
            series.SetValue(ExplodedAllProperty, value);
        }

        /// <summary>
        /// Gets the exploded radius.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>double value explode radius</returns>
        public static double GetExplodeRadius(ChartSeries series)
        {
            return (double)series.GetValue(ExplodeRadiusProperty);
        }

        /// <summary>
        /// Sets the exploded radius.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetExplodeRadius(ChartSeries series, double value)
        {
            series.SetValue(ExplodeRadiusProperty, value);
        }

        /// <summary>
        /// Gets the exploded indices.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>int exploded index value</returns>
        public static int GetExplodedIndex(ChartSeries series)
        {
            return (int)series.GetValue(ExplodedIndexProperty);
        }

        /// <summary>
        /// Sets the exploded indices.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetExplodedIndex(ChartSeries series, int value)
        {
            series.SetValue(ExplodedIndexProperty, value);
        }

        /// <summary>
        /// Gets the Pie coefficient
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>double exploded index value</returns>
        public static double GetPieCoefficient(ChartSeries series)
        {
            return (double)series.GetValue(PieCoefficientProperty);
        }

        /// <summary>
        /// Sets the exploded indices.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetPieCoefficient(ChartSeries series, double value)
        {
            series.SetValue(PieCoefficientProperty, value);
        }

        #endregion

        #region Implementation

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.IsDataModified = true;
                series.Area.LoadArea();
            }
        }
        /// <summary>
        /// Virtula method implementation for InitializeSeriesMinMax
        /// </summary>
        /// <param name="series"></param>
        /// <param name="points"></param>
        protected virtual void InitializeSeriesMinMax(ChartSeries series, ChartPointsCollection points)
        {
            if (points.Count != 0)
            {
                double[] datapoints = (from point in points where point.Visible == true select Math.Abs(point.Y)).ToArray<double>();
                if (datapoints.Length != 0d)
                {
                    series.sum = datapoints.Sum();
                    series.minimum = datapoints.Min();
                    series.maximum = datapoints.Max();
                }
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            int explodedIndex = GetExplodedIndex(series);
            bool explodedAll = GetExplodedAll(series);
            bool isExplodedPresent = explodedAll || (explodedIndex >= 0);
            double explodeRadius = GetExplodeRadius(series);
            double sumValues = 0;
            double startAngle = GetStartAngle(series);
            double endAngle = GetEndAngle(series);

            Brush[] brush;
            brush = series.Area.ColorModel.CurrentPalette;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].EmptyPoint == true && series.ShowEmptyPoints)
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
            if (points.Count != 0)
            {
                double[] datapoints;
                if (!series.ShowEmptyPoints)
                    datapoints = (from point in points where !point.Y.Equals(double.NaN) && point.Visible == true select Math.Abs(point.Y)).ToArray<double>();
                else
                    datapoints = (from point in points where !point.Y.Equals(double.NaN) select Math.Abs(point.Y)).ToArray<double>();
                if (datapoints.Length != 0d)
                {
                    series.sum = sumValues = datapoints.Sum() == 0 ? 1 : datapoints.Sum();
                    series.minimum = datapoints.Min();
                    series.maximum = datapoints.Max();
                }
            }

            double currAngle = 0;
            for (int i = 0; i < points.Count; i++)
            {
                ChartPieSegment segment;
                double angle = 0d;
                if (points[i].EmptyPoint == true && series.ShowEmptyPoints)
                {
                    angle = (Math.Abs(points[i].Y) / sumValues) * Math.Abs(endAngle - startAngle);
                    segment = this.CreateSegment(startAngle + currAngle, startAngle + currAngle + angle, points[i], series);
                    if (isExplodedPresent)
                    {
                        segment.ExplodeRadius = explodeRadius;
                        segment.IsExploded = explodedAll || (i == explodedIndex);
                    }
                    segment.Interior = segment.PaletteInterior = series.EmptyPointInterior;
                    series.Segments.Add(segment);
                    currAngle += angle;
                }
                else
                {
                    if (double.IsNaN(points[i].Y))
                        continue;
                    if (points[i].Visible)
                    {
                        angle = (Math.Abs(points[i].Y) / sumValues) * Math.Abs(endAngle - startAngle);
                        if ((points.Count == 1 || Math.Abs(points[i].Y) == sumValues) && points[i].Visible && !double.IsNaN(angle))
                        {
                            segment = this.CreateSegment(startAngle, startAngle + 360, points[i], series);
                            segment.isSinglePoint = true;
                        }
                        else
                        {
                            segment = this.CreateSegment(startAngle + currAngle, startAngle + currAngle + angle, points[i], series);
                        }
                    }
                    else
                    {
                        segment = this.CreateSegment(currAngle, currAngle, points[i], series);
                    }

                    if (isExplodedPresent)
                    {
                        segment.ExplodeRadius = explodeRadius;
                        segment.IsExploded = explodedAll || (i == explodedIndex);
                    }
                    if(brush != null)
                        segment.Interior = segment.PaletteInterior = brush[i % brush.Length];
                    series.Segments.Add(segment);
                    currAngle += angle;
                }
                
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                CreateAdornments(series, points);
            }
        }
        /// <summary>
        /// Virtual method have return ChartPieSegment from given segments
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="correspondingPoint"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        protected virtual ChartPieSegment CreateSegment(double startAngle, double endAngle, ChartPoint correspondingPoint, ChartSeries series)
        {
            return new ChartPieSegment(startAngle, endAngle, correspondingPoint, series);
        }

        /// <summary>
        /// Calculates the adornments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected void CreateAdornments(ChartSeries series, ChartPointsCollection points)
        {
            double currAngle = 0;
            double proc = 100 / series.sum;
            double startAngle = GetStartAngle(series);
            double endAngle = GetEndAngle(series);


            for (int i = 0; i < points.Count; i++)
            {
                if (!points[i].Visible && !series.ShowEmptyPoints)
                {
                    continue;
                }
                if (double.IsNaN(points[i].Y))
                    continue;

                double radius = 0d;
                double angle = (Math.Abs(points[i].Y) / series.sum) * Math.Abs(endAngle - startAngle);
                double adornmentsAngle = 0d;

                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        adornmentsAngle = currAngle + (angle / 2);
                        break;
                    case HorizontalAlignment.Left:
                        adornmentsAngle = currAngle + (angle / 4);
                        break;
                    case HorizontalAlignment.Right:
                        adornmentsAngle = currAngle + (angle / 1);
                        break;
                    case HorizontalAlignment.Stretch:
                        adornmentsAngle = currAngle + (angle / 2);
                        break;
                }

                if (series.AdornmentsInfo.SegmentShowLine)
                {
                    radius = SetRadius(8, series); 
                }
                else if (series.AdornmentsInfo.SegmentIsOut)
                {
                    radius = SetRadius(9, series); 
                }
                else
                {
                     switch (series.AdornmentsInfo.SegmentVerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            radius = SetRadius(2, series);
                            break;
                        case VerticalAlignment.Center:
                            radius = SetRadius(4, series);
                            break;
                        case VerticalAlignment.Top:
                            radius = SetRadius(6, series);
                            break;
                        case VerticalAlignment.Stretch:
                            radius = SetRadius(4, series);
                            break;
                    }
                }

                ChartPieAdornment ador = new ChartPieAdornment(startAngle + adornmentsAngle, radius, points, points[i], series, currAngle, currAngle + angle);
                ador.LabelAngle = adornmentsAngle;
                ////series.Segments.Add(ador);
                series.Adornments.Add(ador);
                currAngle += angle;
            }
        }

        private double SetRadius(double rad, ChartSeries series)
        {
            int index = series.Area.Series.IndexOf(series);
            double radius = 0;
            if (index == 0)
            {
                if (series.Area.Series.Count % 2 == 0 || series.Area.Series.Count == 1)
                    radius = (rad * Math.Pow(10, -1)) / series.Area.Series.Count;
                else
                    radius = (rad * Math.Pow(10, -1)) / (series.Area.Series.Count + 1);
            }
            else
            {
                if (series.Area.Series.Count % 2 == 0)
                    radius = ((rad / series.Area.Series.Count) * (2 * index)) * Math.Pow(10, -1);
                else
                    radius = ((rad / (series.Area.Series.Count + 1)) * (2 * index)) * Math.Pow(10, -1);
            }
            return radius;
        }
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            this.CalculateSegments(series, points);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "Pie";
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
