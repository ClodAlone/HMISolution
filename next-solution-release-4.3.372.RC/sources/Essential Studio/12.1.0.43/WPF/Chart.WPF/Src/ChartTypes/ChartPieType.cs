// <copyright file="ChartPieType.cs" company="Syncfusion">
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
    using System.Globalization;
    using System.Windows.Media.Media3D;
    using System.Windows.Markup;
    using Syncfusion.Windows.Shared;
    using System.Threading;

    /// <summary>
    /// Class represent Pie-typed adornment. 
    /// </summary>
    /// <remarks>
    /// This adornment type is specific for chart types as Pie, Doughnut, Funnel and Pyramid.
    /// Class instances are created internally and no user code is required except special cases.
    /// </remarks>
    /// <seealso cref="ChartPieAdornment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartPieAdornment : ChartAdornment
    {
        #region Members
        /// <summary>
        /// Initializes m_angle
        /// </summary>
        private double m_angle = 0;

        /// <summary>
        /// Initializes m_radius
        /// </summary>
        private double m_radius = 0;
        #endregion

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

        #region Properties
        /// <summary>
        /// Identifies the PieAdornmentSegmentMode dependency property
        /// </summary>
        public static readonly DependencyProperty AdornmentSegmentsModeProperty =
            DependencyProperty.RegisterAttached("AdornmentSegmentsMode", typeof(AdornmentSegmentModes), typeof(ChartAdornmentInfo), new UIPropertyMetadata(AdornmentSegmentModes.Horizontal, new PropertyChangedCallback(OnAdornmentSegmentsModeChanged)));

        /// <summary>
        /// Sets the pie adornment segments arrangement mode
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static AdornmentSegmentModes GetAdornmentSegmentsMode(ChartSeries series)
        {
            return (AdornmentSegmentModes)series.GetValue(AdornmentSegmentsModeProperty);
        }

        /// <summary>
        /// Gets the pie adornment segments arrangement mode
        /// </summary>
        /// <param name="series"></param>
        /// <param name="value"></param>
        public static void SetAdornmentSegmentsMode(ChartSeries series, AdornmentSegmentModes value)
        {
            series.SetValue(AdornmentSegmentsModeProperty, value);
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPieAdornment"/> class.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        public ChartPieAdornment(double angle, double radius, ChartIndexedDataPoint point, ChartSeries series)
            : base(new ChartPoint(0, 0), point, series)
        {
            m_angle = angle;
            m_radius = radius;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartPieAdornment"/>
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
            base.Update(transformer);
            int count=0;
            if(this.Series!=null && this.Series.Area!=null)
            count=this.Series.Area.Series.Count;
            double radius = m_radius * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
            Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);

            this.X = (center.X) + radius * Math.Cos(m_angle);
            this.Y = center.Y + radius * Math.Sin(m_angle);
        }

        /// <summary>
        /// Called when AdornmentSegmentsMode property is changed
        /// </summary>
        /// <param name="d">The DependencyObject d (ChartSeries).</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAdornmentSegmentsModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                ChartSeries series = d as ChartSeries;
                if(series != null)
                    series.ChartType.Update(series);
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents Pie chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartPieSegment : ChartSegment
    {
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

        /// <summary>
        /// Identifies the AngleOfSliceRotation dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey AngleOfSliceRotationPropertyKey =
            DependencyProperty.RegisterReadOnly("AngleOfSliceRotation", typeof(double), typeof(ChartPieSegment), new FrameworkPropertyMetadata(0d));
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the External Layer Geometry points dependency property.
        /// </summary>
        public static readonly DependencyProperty ExternalPointsProperty =
             DependencyProperty.Register("ExternalPoints", typeof(Geometry), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Layer Brush color dependency property.
        /// </summary>
        public static readonly DependencyProperty LayerBrushProperty =
           DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(ChartPieSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartPieSegment), new PropertyMetadata(null));

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
        /// Identifies the AngleOfSliceRotation dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleOfSliceRotationProperty = AngleOfSliceRotationPropertyKey.DependencyProperty;


        /// <summary>
        /// Identifies the PieCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty PieCoefficientProperty =
            DependencyProperty.Register("PieCoefficient", typeof(double), typeof(ChartDoughnutSegment), new PropertyMetadata(0.4d));
        #endregion


        /// <summary>
        /// Gets or sets the Pie coefficient. This is a dependency property.
        /// </summary>
        /// <remarks>Represents pie coefficient that corresponds for interior hole radius.</remarks>
        /// <value>The Pie coefficient.</value>
        public double PieCoefficient
        {
            get { return (double)GetValue(PieCoefficientProperty); }
            set { SetValue(PieCoefficientProperty, value); }
        }




        #region Properties

        /// <summary>
        /// Gets or sets the end angle.
        /// </summary>
        /// <value>The end angle.</value>
        protected double EndAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value of the AngleOfSliceRotation. This is a dependency property.
        /// </summary>
        public double AngleOfSliceRotation
        {
            get { return (double)GetValue(AngleOfSliceRotationProperty); }
        }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>The start angle.</value>
        protected double StartAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the geometry. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the geometry of the segment.</remarks>
        /// <value>The geometry.</value>
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
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

        /// <summary>
        /// Gets or sets the External Layer Geometry points. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the Geometry points.</remarks>
        /// <value>The Geometry points.</value>
        public Geometry ExternalPoints
        {
            get { return (Geometry)GetValue(ExternalPointsProperty); }
            set { SetValue(ExternalPointsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the External Layer Brush. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the Brush color.</remarks>
        /// <value>The Layer Brush.</value>
        public Brush LayerBrush
        {
            get { return (Brush)GetValue(LayerBrushProperty); }
            set { SetValue(LayerBrushProperty, value); }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartPieSegment"/> class.
        /// </summary>
        /// <remarks>Default segment's template is being assigned automatically.</remarks>
        static ChartPieSegment()
        {
            Type type = typeof(ChartPieSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPieSegment"/> class.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartPieSegment(double startAngle, double endAngle, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            StartAngle = startAngle;
            EndAngle = endAngle;         
            SetValue(AngleOfSliceRotationPropertyKey, (startAngle + endAngle) / 2);
        }
        #endregion

   
        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <remarks>Method is being called internally in order to update segment.</remarks>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>        
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }

            base.Update(transformer);
            double[] segRadius = new double[Series.Area.Series.Count];
            List<ChartSeries> VisiblePieSeries = new List<ChartSeries>();
            foreach (ChartSeries ser in Series.Area.Series)
            {
                if (ser.IsVisible && ser.Type == ChartTypes.Pie)
                {
                    VisiblePieSeries.Add(ser);
                }
            }
            for (int i = 0; i < VisiblePieSeries.Count; i++)
            {
                if (i == 0)
                    segRadius[i] = Math.Pow(2, VisiblePieSeries.Count);
                else
                    segRadius[i] = segRadius[i - 1] / 2;
            }

            if (VisiblePieSeries.IndexOf(Series) == 0)
            {
                ////double radius = Math.Max(0, Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2 - ExplodeRadius) * 0.8;
                double radius = 0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / segRadius[0];//((Series.Area.Series.Count + 2) / (Series.Area.Series.IndexOf(Series) + 1));
                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                if (this.IsExploded)
                {
                    center += ExplodeRadius * new Vector(Math.Cos(AngleOfSliceRotation), Math.Sin(AngleOfSliceRotation));
                }

                if (this.CorrespondingPoints[0].DataPoint.Visible == true && this.CorrespondingPoints[0].DataPoint.Y > 0)
                {
                    double startx = center.X - radius;
                    double starty = center.Y - radius;
                    double width = radius * 2d;
                    PathGeometry externalpath = new PathGeometry();
                    PathFigureCollection expfc = new PathFigureCollection();
                    PathFigure expf = new PathFigure();

                    expf.StartPoint = new Point(startx, starty);
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty + width) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty + width) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty) });

                    BrushConverter brush = new BrushConverter();
                    RadialGradientBrush lBrush = new RadialGradientBrush();
                    lBrush.RadiusX = 1.1360000371932983;
                    lBrush.RadiusY = 1.1360000371932983;
                    GradientStop gs = new GradientStop((brush.ConvertFromInvariantString("#66FFFFFF") as SolidColorBrush).Color, 0.843);
                    lBrush.GradientStops.Add(gs);
                    gs = new GradientStop((brush.ConvertFromInvariantString("#72FFFFFF") as SolidColorBrush).Color, 0.0);
                    lBrush.GradientStops.Add(gs);
                    gs = new GradientStop((brush.ConvertFromInvariantString("#72FFFFFF") as SolidColorBrush).Color, 0.443);
                    lBrush.GradientStops.Add(gs);
                    gs = new GradientStop((brush.ConvertFromInvariantString("#663D3D3D") as SolidColorBrush).Color, 0.427);
                    lBrush.GradientStops.Add(gs);
                    gs = new GradientStop((brush.ConvertFromInvariantString("#66C3C3C3") as SolidColorBrush).Color, 0.343);
                    lBrush.GradientStops.Add(gs);

                    this.LayerBrush = lBrush;

                    expfc.Add(expf);
                    externalpath.Figures = expfc;
                    this.ExternalPoints = externalpath;

                    if ((Series.Area.Parent as Chart).ChartVisualStyle.ToString().Contains("WithBorder"))
                    {
                        StrokeThickness = 1;
                        this.Stroke = Brushes.White;
                    }
                    else if ((Series.Area.Parent as Chart).ChartVisualStyle.ToString() != "None")
                    {
                        if (Series.Stroke == Brushes.Black || Series.StrokeThickness == 0)
                        {
                            this.Stroke = Brushes.Transparent;
                            StrokeThickness = 0;
                        }
                        else
                        {
                            BindingUtils.SetBinding(this, Series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
                            BindingUtils.SetBinding(this, Series, StrokeProperty, ChartSeries.StrokeProperty);
                        }
                    }
                    else
                    {
                        BindingUtils.SetBinding(this, Series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
                        BindingUtils.SetBinding(this, Series, StrokeProperty, ChartSeries.StrokeProperty);
                    }
                }

                Point startPoint = center + radius * new Vector(Math.Cos(StartAngle), Math.Sin(StartAngle));
                Point endPoint = center + radius * new Vector(Math.Cos(EndAngle), Math.Sin(EndAngle));
                if (isSinglePoint || (EndAngle - StartAngle) == Math.PI * 2)
                {
                    this.Geometry = new EllipseGeometry(center, radius, radius, null);
                }

                if (m_startPoint != startPoint || m_endPoint != endPoint)
                {
                    if (EndAngle != StartAngle)
                    {
                        PathFigure figure = new PathFigure();
                        figure.StartPoint = center;
                        figure.Segments.Add(new LineSegment(startPoint, true) { IsSmoothJoin = true });
                        //Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.ToString());
                        //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                        figure.Segments.Add(new ArcSegment(endPoint, new Size(radius, radius), Math.Round(EndAngle - StartAngle), (EndAngle - StartAngle > Math.PI), SweepDirection.Clockwise, true));
                        figure.Segments.Add(new LineSegment(center, true) { IsSmoothJoin = true });
                        figure.IsClosed = true;
                        this.Geometry = new PathGeometry(new PathFigure[] { figure });
                        m_startPoint = startPoint;
                        m_endPoint = endPoint;
                    }
                    else
                    {
                        Geometry = null;
                    }
                }
            }
            else if (VisiblePieSeries.IndexOf(Series) >= 1)
            {
                double radius = 0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / segRadius[VisiblePieSeries.IndexOf(Series)];//((Series.Area.Series.Count * 2) / (Series.Area.Series.IndexOf(Series) + 1));
                double dradius = this.PieCoefficient * radius;

                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                if (this.IsExploded)
                {
                    center += this.ExplodeRadius * new Vector(Math.Cos(AngleOfSliceRotation), Math.Sin(AngleOfSliceRotation));
                }

                Point startPoint = center + radius * new Vector(Math.Cos(StartAngle), Math.Sin(StartAngle));
                Point endPoint = center + radius * new Vector(Math.Cos(EndAngle), Math.Sin(EndAngle));

                if (this.CorrespondingPoints[0].DataPoint.Visible == true)
                {
                    double startx = center.X - radius;
                    double starty = center.Y - radius;
                    double width = radius * 2d;
                    PathGeometry externalpath = new PathGeometry();
                    PathFigureCollection expfc = new PathFigureCollection();
                    PathFigure expf = new PathFigure();

                    expf.StartPoint = new Point(startx, starty);
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx + width, starty + width) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty + width) });
                    expf.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(startx, starty) });

                    this.LayerBrush = XamlReader.Parse("<RadialGradientBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'> <GradientStop Color='#663D3D3D' Offset='1'/> <GradientStop Color='#66DBDBE1' Offset='0.75'/> <GradientStop Color='#663D3D3D' Offset='0.5'/><GradientStop Color='#00737373' Offset='0.4999'/></RadialGradientBrush>") as Brush;
                    RadialGradientBrush brush = this.LayerBrush as RadialGradientBrush;
                    brush.GradientStops[2].Offset = (brush.GradientStops[2].Offset / 0.5d) * this.PieCoefficient;
                    brush.GradientStops[3].Offset = (brush.GradientStops[3].Offset / 0.5d) * this.PieCoefficient;
                    brush.GradientStops[1].Offset = (brush.GradientStops[0].Offset + brush.GradientStops[2].Offset) / 2d;
                    //this.LayerBrush = brush;
                    expfc.Add(expf);
                    externalpath.Figures = expfc;
                    if ((Series.Area.Parent as Chart).ChartVisualStyle.ToString().Contains("WithBorder"))
                    {
                        StrokeThickness = 1;
                        this.Stroke = Brushes.White;
                    }
                    else if ((Series.Area.Parent as Chart).ChartVisualStyle.ToString() != "None")
                    {
                        if (Series.Stroke == Brushes.Black || Series.StrokeThickness == 0)
                        {
                            this.Stroke = Brushes.Transparent;
                            StrokeThickness = 0;
                        }
                        else
                        {
                            BindingUtils.SetBinding(this, Series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
                            BindingUtils.SetBinding(this, Series, StrokeProperty, ChartSeries.StrokeProperty);
                        }
                    }
                    else
                    {
                        BindingUtils.SetBinding(this, Series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
                        BindingUtils.SetBinding(this, Series, StrokeProperty, ChartSeries.StrokeProperty);
                    }
                    if (StartAngle != EndAngle)
                    {
                        this.ExternalPoints = externalpath;
                    }
                }

                if (isSinglePoint)
                {
                    GeometryGroup geometryGroup = new GeometryGroup();
                    geometryGroup.Children.Add(new EllipseGeometry(center, radius, radius, null));
                    geometryGroup.Children.Add(new EllipseGeometry(center, dradius, dradius, null));
                    geometryGroup.FillRule = FillRule.EvenOdd;
                    this.Geometry = geometryGroup;
                }

                if (startPoint != endPoint && (m_startPoint != startPoint || m_endPoint != endPoint))
                {
                    Point startDPoint = center + dradius * new Vector(Math.Cos(StartAngle), Math.Sin(StartAngle));
                    Point endDPoint = center + dradius * new Vector(Math.Cos(EndAngle), Math.Sin(EndAngle));

                    PathFigure figure = new PathFigure();

                    figure.StartPoint = startPoint;
                    figure.Segments.Add(new ArcSegment(endPoint, new Size(radius, radius), EndAngle - StartAngle, (EndAngle - StartAngle > Math.PI), SweepDirection.Clockwise, true));
                    figure.Segments.Add(new LineSegment(endDPoint, true));
                    figure.Segments.Add(new ArcSegment(startDPoint, new Size(dradius, dradius), EndAngle - StartAngle, (EndAngle - StartAngle > Math.PI), SweepDirection.Counterclockwise, true));
                    figure.IsClosed = true;

                    this.Geometry = new PathGeometry(new PathFigure[] { figure });
                    m_startPoint = startPoint;
                    m_endPoint = endPoint;
                }
            }

        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            bool isExplodedVisible = false;
            base.Update(transformer);

            Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
            double[] segRadius = new double[Series.Area.Series.Count];
            for (int i = 0; i < Series.Area.Series.Count; i++)
            {
                if (i == 0)
                    segRadius[i] = Math.Pow(2, Series.Area.Series.Count);
                else
                    segRadius[i] = segRadius[i - 1] / 2;
            }
            if (this.IsExploded)
            {
                foreach (ChartSegment segment in this.Series.Segments)
                {
                    if (segment != this)
                    {
                        if (segment.CorrespondingPoints[0].DataPoint.Visible)
                        {
                            isExplodedVisible = true;
                        }
                    }
                }
            }

            int index = this.Series.Area.Series.IndexOf(Series);
            if (index == 0)
            {
                double radius = 0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / segRadius[0];
                GeometryModel3D geometryModel = new GeometryModel3D();
                geometryModel.Geometry = MeshGenerator.PieSegment(new Point(0, 0), radius, StartAngle, EndAngle, this.IsExploded, isExplodedVisible);

                MaterialGroup materialGroup;

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                //difuseMaterial.Color = Brushes.Black;
                ////BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                ChartStyleModel styleModel = Series.Area.ColorModel;
                if (!this.Series.ColorEach.HasValue || (bool)this.Series.ColorEach)
                    styleModel.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, this.CorrespondingPoints[0].Index);
                else
                    BindingUtils.SetBinding(difuseMaterial, Series, DiffuseMaterial.BrushProperty, ChartSeries.InteriorProperty);

                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);


                geometryModel.Material = materialGroup;
                Geometry3DGroup.Children.Add(geometryModel);
            }
            else
            {
                double radius = 1.5 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / segRadius[this.Series.Area.Series.IndexOf(Series)];
                double dradius = ChartPieType.GetPieCoefficient(this.Series) * radius;

                Geometry3D.Geometry = MeshGenerator.DoughnutSegment(new Point(0, 0), radius, dradius, StartAngle, EndAngle, this.IsExploded, this.ExplodeRadius, isExplodedVisible);

                //ModelUIElement3D d=new ModelUIElement3D();
                //d.Model = Geometry3D;

                MaterialGroup materialGroup;

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                ////BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                ChartStyleModel styleModel = Series.Area.ColorModel;
                styleModel.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, this.CorrespondingPoints[0].Index);
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);

                Geometry3D.Material = materialGroup;
            }


            //Geometry3DGroup.Children.Add();

        }
        #endregion
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
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartPieType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets axes type that are required for chart to be built.
        /// </summary>
        public override ChartAxesType AxesType
        {
            get
            {
                return ChartAxesType.None;
            }
        }

        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.NotRequiresAxis | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedIndexProperty =
          DependencyProperty.RegisterAttached("ExplodedIndex", typeof(int), typeof(ChartPieType), new ChartPropertyMetadata(-1, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the ExplodedAll dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedAllProperty =
          DependencyProperty.RegisterAttached("ExplodedAll", typeof(bool), typeof(ChartPieType), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the ExplodeRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
          DependencyProperty.RegisterAttached("ExplodeRadius", typeof(double), typeof(ChartPieType), new ChartPropertyMetadata(10d, ChartPropertyMetadataOptions.AffectsUpdate));


        /// <summary>
        /// Identifies the PieCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty PieCoefficientProperty =
          DependencyProperty.RegisterAttached("PieCoefficient", typeof(double), typeof(ChartPieType), new ChartPropertyMetadata(0.7d, new PropertyChangedCallback(OnvalueChaged), new CoerceValueCallback(OnCoerceValue), ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion

        #region Public methods

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static double GetPieCoefficient(ChartSeries series)
        {
            return (double)series.GetValue(PieCoefficientProperty);
        }

        /// <summary>
        /// Set PieCoefficient to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="value"></param>
        public static void SetPieCoefficient(ChartSeries series, double value)
        {
            series.SetValue(PieCoefficientProperty, value);
        }

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
        /// Determines whether the this type is compatible with specified type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>
        /// <c>true</c> if the type is compatible; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCompatible(ChartType type)
        {
            return false;
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
            int explodedIndex = GetExplodedIndex(series);
            bool explodedAll = GetExplodedAll(series);
            bool isExplodedPresent = explodedAll || (explodedIndex >= 0);
            double explodeRadius = GetExplodeRadius(series);
            double sumValues = 0;
            double length = 0;
            ChartStyleModel styleModel = series.Area.ColorModel;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].DataPoint.Visible)
                {
                    sumValues += Math.Max(0, Math.Abs(points[i].DataPoint.Y));
                    length++;
                }
            }
            double currAngle = 0;
            double coef = ChartMath.DoublePI / sumValues;

            for (int i = 0; i < points.Length; i++)
            {
                double angle = 0;
                ChartPieSegment segment;
                if (points[i].DataPoint.Visible)
                {
                    angle = coef * Math.Max(0, Math.Abs(points[i].DataPoint.Y));
                    if ((length == 1 || points[i].DataPoint.Y == sumValues) && points[i].DataPoint.Visible && !double.IsNaN(angle))
                    {
                        segment = this.CreateSegment(0, ChartMath.DoublePI, points[i], series);
                        segment.PieCoefficient = GetPieCoefficient(series);
                        segment.isSinglePoint = true;
                    }
                    else
                    {
                        segment = this.CreateSegment(currAngle, currAngle + angle, points[i], series);
                        segment.PieCoefficient = GetPieCoefficient(series);
                    }
                }
                else
                {
                    segment = this.CreateSegment(currAngle, currAngle, points[i], series);
                    segment.PieCoefficient = GetPieCoefficient(series);
                }
                ////series.GetPoint(i).ParentSegment = segment;
                points[i].DataPoint.ParentSegment = segment;
                if (isExplodedPresent)
                {
                    segment.ExplodeRadius = explodeRadius;
                    segment.IsExploded = explodedAll || (points[i].Index == explodedIndex);
                }
                //styleModel.SetBinding(segment, ChartSegment.InteriorProperty, i);
                //series.Segments.Add(segment);
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        segment.Interior = series.EmptyPointInterior;
                    }
                    else
                    {
                        if (!series.ColorEach.HasValue)
                            styleModel.SetBinding(segment, ChartSegment.InteriorProperty, i);
                        else
                            series.UpdateColorEachSegments(series, segment, i, null);
                    }
                }
                else
                {
                    if(!series.ColorEach.HasValue)
                        styleModel.SetBinding(segment, ChartSegment.InteriorProperty, (i % series.Area.ColorModel.CurrentPalette.Length));
                    else
                        series.UpdateColorEachSegments(series, segment, i, null);
                }
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        series.Segments.Add(segment);
                    }

                }
                else
                {
                    series.Segments.Add(segment);
                }

             
                currAngle += angle;
            }
            
            if (series.AdornmentsInfo.Visible)
            {
                CreateAdornments(series, points, sumValues, coef);
            }
            if (series.Area.View3DMode)
                series.Update3D();
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartPieType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();            
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }

        /// <summary>
        /// Calculates the adornments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <param name="sumValues">The summary values.</param>
        /// <param name="coef">The coefficient.</param>
        protected void CreateAdornments(ChartSeries series, ChartIndexedDataPoint[] points, double sumValues, double coef)
        {
            double currAngle = 0;
            double proc = 100 / sumValues;
            series.Adornments.Clear();
            List<ChartSeries> VisiblePieSeries = new List<ChartSeries>();
            foreach (ChartSeries ser in series.Area.Series)
            {
                if (ser.IsVisible && (ser.Type == ChartTypes.Pie))
                {
                    VisiblePieSeries.Add(ser);
                }
                else if (ser.Type == ChartTypes.Doughnut)
                {
                    VisiblePieSeries.Add(ser);
                }
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (!points[i].DataPoint.Visible)
                {
                    continue;
                }

                double radius = 0d;
                double angle = double.IsInfinity(coef) ? 0 : coef * Math.Max(0, Math.Abs(points[i].DataPoint.Y));
                double adornmentsAngle = 0d;

                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        adornmentsAngle = currAngle + angle / 2;
                        break;
                    case HorizontalAlignment.Left:
                        adornmentsAngle = currAngle + angle / 4;
                        break;
                    case HorizontalAlignment.Right:
                        adornmentsAngle = currAngle + angle / 1.3;
                        break;
                    case HorizontalAlignment.Stretch:
                        adornmentsAngle = currAngle + angle / 2;
                        break;
                }

                if (series.AdornmentsInfo.SegmentShowLine)
                {
                    double max = 7.5;
                    double radiuspoint = 1;
                    if (VisiblePieSeries.Count > 3 && VisiblePieSeries.Count < 6)
                        radiuspoint = max - (VisiblePieSeries.Count - 3);
                    else if (VisiblePieSeries.Count >= 6)
                        radiuspoint = max - (VisiblePieSeries.Count - 4);
                    else
                        radiuspoint = max;
                    if ((VisiblePieSeries.IndexOf(series) == 0))
                        if (VisiblePieSeries.Count < 3)
                            radius = SetRadius(8, series, VisiblePieSeries);
                        else
                            radius = SetRadius(6 / VisiblePieSeries.Count, series, VisiblePieSeries);
                    else
                    {
                        if (VisiblePieSeries.Count > 3)
                            if (VisiblePieSeries.IndexOf(series) < 5)
                                radius = SetRadius(radiuspoint - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series))), series, VisiblePieSeries);
                            else
                                radius = SetRadius((8 - (VisiblePieSeries.Count - 3.5)) - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series) + 1)), series, VisiblePieSeries);
                        else
                            radius = SetRadius(8, series, VisiblePieSeries);
                    }
                    //if ( index == 0)
                    //{
                    //    if (series.Area.Series.Count % 2 == 0)
                    //        radius = 0.8 / series.Area.Series.Count;
                    //    else
                    //        radius = 0.8 / (series.Area.Series.Count + 1);
                    //}
                    //else
                    //{
                    //    if (series.Area.Series.Count % 2 == 0)
                    //        radius = ((8 / series.Area.Series.Count) *(2 * index)) * Math.Pow(10,-1);
                    //    else
                    //        radius = ((8 / (series.Area.Series.Count + 1)) * (2 * index)) * Math.Pow(10, -1);
                    //}
                    //radius = SetRadius(8, series, VisiblePieSeries);
                }
                else if (series.AdornmentsInfo.SegmentIsOut)
                {
                    radius = SetRadius(9, series, VisiblePieSeries); ;
                }
                else
                {
                    double max = 7.5;
                    double radiuspoint =1;
                    if (VisiblePieSeries.Count > 3 && VisiblePieSeries.Count < 6)
                        radiuspoint = max - (VisiblePieSeries.Count - 3);
                    else if (VisiblePieSeries.Count >= 6)
                        radiuspoint = max - (VisiblePieSeries.Count - 4);
                    else
                        radiuspoint = max;
                    switch (series.AdornmentsInfo.SegmentVerticalAlignment)
                    {                       
                        case VerticalAlignment.Bottom:
                            if ((VisiblePieSeries.IndexOf(series) == 0))
                                radius = SetRadius(2, series, VisiblePieSeries);
                            else
                            {
                                if (VisiblePieSeries.Count > 3)
                                    radius = SetRadius(radiuspoint - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series))), series, VisiblePieSeries);
                                else
                                    radius = SetRadius((ChartPieType.GetPieCoefficient(series) / Math.Pow(10, -1)), series, VisiblePieSeries);
                            }
                            break;
                        case VerticalAlignment.Center:
                            if ((VisiblePieSeries.IndexOf(series) == 0))
                                radius = SetRadius(4, series, VisiblePieSeries);
                            else
                            {
                                if (VisiblePieSeries.Count > 3)
                                    radius = SetRadius(radiuspoint - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series))), series, VisiblePieSeries);
                                else
                                    radius = SetRadius((ChartPieType.GetPieCoefficient(series) / Math.Pow(10, -1)), series, VisiblePieSeries);
                            }
                            break;
                        case VerticalAlignment.Top:
                            if ((VisiblePieSeries.IndexOf(series) == 0))
                                radius = SetRadius(6, series, VisiblePieSeries);
                            else
                            {
                                if (VisiblePieSeries.Count > 3)
                                    radius = SetRadius(radiuspoint - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series))), series, VisiblePieSeries);
                                else
                                    radius = SetRadius((ChartPieType.GetPieCoefficient(series) / Math.Pow(10, -1)), series, VisiblePieSeries);
                            }
                            break;
                        case VerticalAlignment.Stretch:
                            if ((VisiblePieSeries.IndexOf(series) == 0))
                                radius = SetRadius(4, series, VisiblePieSeries);
                            else
                            {
                                if (VisiblePieSeries.Count > 3)
                                    radius = SetRadius(radiuspoint - (VisiblePieSeries.Count - (VisiblePieSeries.IndexOf(series))), series, VisiblePieSeries);
                                else
                                    radius = SetRadius((ChartPieType.GetPieCoefficient(series) / Math.Pow(10, -1)), series, VisiblePieSeries);
                            }
                            break;
                    }
                }

                ChartPieAdornment ador = new ChartPieAdornment(adornmentsAngle, radius, points[i], series);
                ador.LabelAngle = adornmentsAngle;

                if (series.AdornmentsInfo.SegmentShowLine)
                {
                    if (adornmentsAngle > Math.PI * 1.64 || adornmentsAngle <= Math.PI * 0.25)
                    {
                        ador.ConnectorAlignment = ConnectorAlignment.Right;
                    }
                    else if (adornmentsAngle > Math.PI * 0.25 && adornmentsAngle <= Math.PI * 0.75)
                    {
                        ador.ConnectorAlignment = ConnectorAlignment.Bottom;
                    }
                    else if (adornmentsAngle > Math.PI * 0.75 && adornmentsAngle <= Math.PI * 1.25)
                    {
                        ador.ConnectorAlignment = ConnectorAlignment.Left;
                    }
                    else if (adornmentsAngle > Math.PI * 1.25 && adornmentsAngle <= Math.PI * 1.75)
                    {
                        ador.ConnectorAlignment = ConnectorAlignment.Top;
                    }
                }

                switch (series.AdornmentsInfo.SegmentLabelContent)
                {
                    case LabelContent.Percentage:
                        ador.SegmentLabel = (points[i].DataPoint.Y * proc).ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture) + "%";
                        break;
                    case LabelContent.XValue:
                        ador.SegmentLabel = points[i].DataPoint.X.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        break;
                    case LabelContent.YValue:
                        ador.SegmentLabel = points[i].DataPoint.Y.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        break;
                    case LabelContent.YofTot:
                        ador.SegmentLabel = points[i].DataPoint.Y.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture) + " of " + sumValues.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        break;
                    case LabelContent.DateTime:
                        ador.SegmentLabel = DateTime.FromOADate(points[i].DataPoint.X).ToString(series.AdornmentsInfo.SegmentLabelDataTimeFormat, CultureInfo.CurrentCulture);
                        break;
                }
                if (series.ShowEmptyPoints == false)
                {
                    if (points[i].DataPoint.EmptyPoint == false)
                    series.Adornments.Add(ador);
                }
                else
                {                
                    series.Adornments.Add(ador);
                }
                currAngle += angle;
            }
        }
        private static void OnvalueChaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        private static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            if ((double)baseValue > 1.0)
            {
                return (object)1.0;
            }
            else return baseValue;
        }

        private double SetRadius(double rad, ChartSeries series, List<ChartSeries> VisibleSeries)
        {
            int index = VisibleSeries.IndexOf(series);
            double radius = 0;
            if (index == 0)
            {
                if (VisibleSeries.Count % 2 == 0 || VisibleSeries.Count == 1)
                    radius = (rad * Math.Pow(10, -1)) / VisibleSeries.Count;
                else
                    radius = (rad * Math.Pow(10, -1)) / VisibleSeries.Count;
            }
            else
            {
                if (VisibleSeries.Count ==2)
                    radius = ((rad / (VisibleSeries.Count)) * (2 * index)) * Math.Pow(10, -1);
                else
                    radius = ((rad / (VisibleSeries.Count + 1)) * (2 * index)) * Math.Pow(10, -1);
            }
            return radius;
        }
        /// <summary>
        /// Creates the segment.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <returns>Returns new ChartPieSegment</returns>
        protected virtual ChartPieSegment CreateSegment(double startAngle, double endAngle, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
        {
            return new ChartPieSegment(startAngle, endAngle, correspondingPoint, series);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartPieType"/>
        public override string ToString()
        {
            return "Pie";
        }
        #endregion
    }
}
