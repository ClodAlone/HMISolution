// <copyright file="ChartDoughnutType.cs" company="Syncfusion">
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
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Markup;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Represents Doughnut chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartDoughnutSegment : ChartPieSegment
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
        internal ChartDoughnutSegment(double startAngle, double endAngle, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
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
        /// <seealso cref="ChartDoughnutSegment"/>
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
            List<ChartSeries> VisibleSeries = new List<ChartSeries>();
            foreach (ChartSeries ser in Series.Area.Series)
            {
                if (ser.IsVisible && ser.Type == ChartTypes.Doughnut)
                {
                    VisibleSeries.Add(ser);
                }
            }
            double[] segRadius = new double[VisibleSeries.Count];
            for (int i = 0; i < VisibleSeries.Count; i++)
            {
                if (i == 0)
                    segRadius[i] = Math.Pow(2, VisibleSeries.Count);
                else
                    segRadius[i] = segRadius[i - 1] / 2;
            }
            ////double radius = Math.Max(0, Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2 - ExplodeRadius);
            double radius = (0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height)) / segRadius[VisibleSeries.IndexOf(Series)];
            double dradius = this.DoughnutCoefficient * radius;

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

                this.LayerBrush = XamlReader.Parse("<RadialGradientBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ><GradientStop Color='#30000000' Offset='1'/> <GradientStop Color='#09FFFFFF' Offset='0.739'/> <GradientStop Color='#30646464' Offset='0.5'/> <GradientStop Offset='0.495'/></RadialGradientBrush>") as Brush;
                RadialGradientBrush brush = this.LayerBrush as RadialGradientBrush;
                brush.GradientStops[2].Offset = (brush.GradientStops[2].Offset / 0.5d) * this.DoughnutCoefficient;
                brush.GradientStops[3].Offset = (brush.GradientStops[3].Offset / 0.5d) * this.DoughnutCoefficient;
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
                figure.Segments.Add(new ArcSegment(endPoint, new Size(radius, radius),Math.Round( EndAngle - StartAngle), (EndAngle - StartAngle > Math.PI), SweepDirection.Clockwise, true));
                figure.Segments.Add(new LineSegment(endDPoint, true));
                figure.Segments.Add(new ArcSegment(startDPoint, new Size(dradius, dradius),Math.Round(EndAngle - StartAngle), (EndAngle - StartAngle > Math.PI), SweepDirection.Counterclockwise, true));
                figure.IsClosed = true;

                this.Geometry = new PathGeometry(new PathFigure[] { figure });
                m_startPoint = startPoint;
                m_endPoint = endPoint;
            }
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            base.Update(transformer);
            bool isExplodedVisible = false;
            double radius = 0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
            double dradius = this.DoughnutCoefficient * radius;

            Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
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

            Geometry3D.Geometry = MeshGenerator.DoughnutSegment(new Point(0, 0), radius, dradius, StartAngle, EndAngle, this.IsExploded, this.ExplodeRadius, isExplodedVisible);

            //ModelUIElement3D d=new ModelUIElement3D();
            //d.Model = Geometry3D;

            MaterialGroup materialGroup;

            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
            Binding binding = new Binding("Interior");
            binding.Source = Series;
            ////BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
            ChartStyleModel styleModel = Series.Area.ColorModel;
            if (!this.Series.ColorEach.HasValue || (bool)this.Series.ColorEach)
                styleModel.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, this.CorrespondingPoints[0].Index);
            else
                BindingUtils.SetBinding(difuseMaterial, Series, DiffuseMaterial.BrushProperty, ChartSeries.InteriorProperty);
            materialGroup = new MaterialGroup();
            materialGroup.Children.Add(difuseMaterial);

            Geometry3D.Material = materialGroup;

            
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
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
            base.Dispose();
        }
        #endregion
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
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartDoughnutType : ChartPieType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the DoughnutCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
      DependencyProperty.RegisterAttached("DoughnutCoefficient", typeof(double), typeof(ChartDoughnutType), new ChartPropertyMetadata(0.2d, ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion

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
        protected override ChartPieSegment CreateSegment(double startAngle, double endAngle, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
        {
            ChartDoughnutSegment segment = new ChartDoughnutSegment(startAngle, endAngle, correspondingPoint, series);

            segment.DoughnutCoefficient = GetDoughnutCoefficient(series);

            return segment;
        }
        #endregion
    }
}
