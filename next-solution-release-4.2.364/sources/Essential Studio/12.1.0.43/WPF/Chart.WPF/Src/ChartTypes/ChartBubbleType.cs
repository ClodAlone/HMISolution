// <copyright file="ChartBubbleType.cs" company="Syncfusion">
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
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Bubble chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartBubbleType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartBubbleSegment : ChartSegment
    {   
        #region Dependency properties

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Radius dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the X co-ordinate value. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the X co-ordinate of the bubble.</remarks>
        /// <value>The X co-ordinate value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate value. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the Y co-ordinate of the bubble.</remarks>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the width of the bubble's ellipse.</remarks>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the height of ellipse.</remarks>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radius. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the radius of the bubble.</remarks>
        /// <value>The radius.</value>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        private IChartDataPoint m_point = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartBubbleSegment"/> class.
        /// </summary>
        /// <remarks>Default segment's template is being assigned automatically.</remarks>
        static ChartBubbleSegment()
        {
            Type type = typeof(ChartBubbleSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBubbleSegment"/> class.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        public ChartBubbleSegment(IChartDataPoint point, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            m_point = point;
            this.SetXRange(m_point.X, m_point.X);
            this.SetYRange(m_point.Y, m_point.Y);
            if (this.Series.Area.EnableDepthAxis && m_point.Values.Length > 2)
                this.SetZRange(m_point.Values[2], m_point.Values[2]);
            //if (this.Series.ActualXAxis.RangeCalculationMode != RangeCalculationMode.ConsistentAcrossChartTypes)
            //{
            //    this.SetYRange(m_point.Y - this.Series.ActualYAxis.VisibleInterval, m_point.Y + this.Series.ActualYAxis.VisibleInterval);
            //    if (this.Series.ActualXAxis.RangePadding == ChartRangePaddingType.None || this.Series.IsIndexed)
            //    {
            //        this.SetXRange(m_point.X - this.Series.ActualXAxis.VisibleInterval, m_point.X + this.Series.ActualXAxis.VisibleInterval);
            //        return;
            //    }
            //    this.SetXRange(m_point.X, m_point.X);
            //}
            //else
            //{
            //    this.SetXRange(m_point.X, m_point.X);
            //    if (this.Series.ActualYAxis.RangePadding != ChartRangePaddingType.None)
            //    {
            //        this.SetYRange(m_point.Y - this.Series.ActualYAxis.VisibleInterval, m_point.Y + this.Series.ActualYAxis.VisibleInterval);
            //        return;
            //    }
            //    this.SetYRange(m_point.Y, m_point.Y);
            //}
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartBubbleSegment"/>
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
            Point pt = transformer.TransformToVisible(m_point.X, m_point.Y);

            this.X = pt.X - Radius;
            this.Y = pt.Y - Radius;
            this.Height = 2 * Radius;
            this.Width = 2 * Radius;
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            Point3D pt = transformer.TransformToVisible(m_point.X, m_point.Y, m_point.Values.Length > 1 ? m_point.Values[1] : 0.06);

            Geometry3D.Geometry = MeshGenerator.Sphere(2 * Radius / 1000, 50);

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
            Geometry3D.Material = materialGroup;

            Geometry3D.Transform = new TranslateTransform3D(pt.X - 0.5, pt.Y - 0.5, pt.Z);
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
    /// Represents ChartBubleType class
    /// </summary>
    /// <remarks>
    /// Bubble Chart is an extension of the Scatter Chart (or XY-chart) where each data
    /// marker is represented by a circle whose dimension form a third variable.
    /// Consequently, bubble charts allow three-variable comparisons allowing for easy
    /// visualization of complex interdependencies that are not apparent in two-variable
    /// charts. Bubble charts are frequently used in market and product comparison
    /// studies.
    /// </remarks>
    /// <seealso cref="ChartBubbleSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartBubbleType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the MinRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty MinRadiusProperty =
      DependencyProperty.RegisterAttached("MinRadius", typeof(double), typeof(ChartBubbleType), new ChartPropertyMetadata(10d, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the MaxRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxRadiusProperty =
      DependencyProperty.RegisterAttached("MaxRadius", typeof(double), typeof(ChartBubbleType), new ChartPropertyMetadata(30d, new PropertyChangedCallback(OnValueChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets the require data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None;
            }
        }
        #endregion

        #region Constants
        /// <summary>
        /// Declares c_symbolSize
        /// </summary>
        private const double C_symbolSize = 10d;
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the minimal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The minimal radius</returns>
        public static double GetMinRadius(ChartSeries series)
        {
            return (double)series.GetValue(MinRadiusProperty);
        }

        /// <summary>
        /// Sets the minimal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetMinRadius(ChartSeries series, double value)
        {
            series.SetValue(MinRadiusProperty, value);
        }

        /// <summary>
        /// Gets the maximal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The double radius</returns>
        /// <seealso cref="ChartBubbleType"/>
        public static double GetMaxRadius(ChartSeries series)
        {
            return (double)series.GetValue(MaxRadiusProperty);
        }

        /// <summary>
        /// Sets the max radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        /// <seealso cref="ChartBubbleType"/>
        public static void SetMaxRadius(ChartSeries series, double value)
        {
            series.SetValue(MaxRadiusProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries ser = d as ChartSeries;
            if (ser != null && ser.Area != null)
            {
                ser.Invalidate();
            }
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
            return "Bubble";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            double maxValue = double.MinValue;
            double minRadius = GetMinRadius(series);
            double radius = GetMaxRadius(series) - minRadius;

            for (int i = 0; i < points.Length; i++)
            {
                maxValue = Math.Max(points[i].DataPoint.Values[1], maxValue);
            }

            for (int i = 0; i < points.Length; i++)
            {
                ////If Emptypoint, make a difference in segment rendering
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                           // series.Segments[i].Interior = series.Interior;
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            ChartBubbleSegment segment = new ChartBubbleSegment(points[i].DataPoint, points[i], series);
                            segment.Radius = minRadius + radius *Math.Abs( points[i].DataPoint.Values[1] / maxValue);
                            series.Segments.Add(segment);
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series,series.EmptyPointSymbolTemplate));
                            //series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                    }
                    else
                    {
                        //series.Adornments.Clear();                        
                        ChartBubbleSegment segment = new ChartBubbleSegment(points[i].DataPoint, points[i], series);
                        segment.Radius = minRadius + radius * Math.Abs(points[i].DataPoint.Values[1] / maxValue);
                        series.Segments.Add(segment);
                        series.Segments[i].Interior = Brushes.Transparent;
                        series.Segments[i].Stroke = Brushes.Transparent;
                    }
                }
                else
                {
                    ChartBubbleSegment segment = new ChartBubbleSegment(points[i].DataPoint, points[i], series);
                    segment.Radius = minRadius + radius * Math.Abs(points[i].DataPoint.Values[1] / maxValue);// points[i].DataPoint.Values[1] / maxValue;
                    series.Segments.Add(segment);
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length ; i++)
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

        /// <summary>
        /// Updates the series.
        /// </summary>
        /// <param name="series">The Chart series</param>
        /// <param name="points">The indexed points</param>
        /// <seealso cref="ChartBubbleType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }

        #endregion
    }
}
