// <copyright file="ChartStepLineType.cs" company="Syncfusion">
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
    using System.Windows.Shapes;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents Step line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartStepLineType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartStepLineSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Declares m_point1
        /// </summary>
        private IChartDataPoint m_point1;
        
        /// <summary>
        /// Declares m_point2
        /// </summary>
        private IChartDataPoint m_point2;

        /// <summary>
        /// Declares m_stepPoint
        /// </summary>
        private IChartDataPoint m_stepPoint;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
      
        /// <summary>
        /// Identifies the StepX dependency property.
        /// </summary>
        public static readonly DependencyProperty StepXProperty =
            DependencyProperty.Register("StepX", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
      
        /// <summary>
        /// Identifies the StepY dependency property.
        /// </summary>
        public static readonly DependencyProperty StepYProperty =
            DependencyProperty.Register("StepY", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));
     
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartStepLineSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the x1. This is a dependency property.
        /// </summary>
        /// <value>The x1 value.</value>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the x2. This is a dependency property.
        /// </summary>
        /// <value>The x2 value.</value>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the y1. This is a dependency property.
        /// </summary>
        /// <value>The y1 value.</value>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the y2. This is a dependency property.
        /// </summary>
        /// <value>The y2 value.</value>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Gets or sets the step X. This is a dependency property.
        /// </summary>
        /// <value>The step X.</value>
        public double StepX
        {
            get { return (double)GetValue(StepXProperty); }
            set { SetValue(StepXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the step Y. This is a dependency property.
        /// </summary>
        /// <value>The step Y.</value>
        public double StepY
        {
            get { return (double)GetValue(StepYProperty); }
            set { SetValue(StepYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segment's points. This is a dependency property.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartStepLineSegment"/> class.
        /// </summary>
        static ChartStepLineSegment()
        {
            Type type = typeof(ChartStepLineSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStepLineSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="stepPoint">The step point.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="correspondingPoint2">The corresponding point2.</param>
        /// <param name="series">The series.</param>
        internal ChartStepLineSegment(IChartDataPoint point1, IChartDataPoint stepPoint, IChartDataPoint point2, ChartIndexedDataPoint correspondingPoint1, ChartIndexedDataPoint correspondingPoint2, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint1, correspondingPoint2 })
        {
            m_point1 = point1;
            m_point2 = point2;
            m_stepPoint = stepPoint;

            this.SetXRange(point1.X, point2.X);
            this.SetYRange(point1.Y, point2.Y);
            if (this.Series.Area.EnableDepthAxis && point1.Values.Length > 1 && point2.Values.Length > 1)
                this.SetZRange(point1.Values[1], point2.Values[1]);
        }
        #endregion

        #region Implmentation

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartStepLineSegment"/>
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
            Point point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
            Point point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);
            Point stepPoint = transformer.TransformToVisible(m_stepPoint.X, m_stepPoint.Values[0]);
            if (X1 != point1.X || X2 != point2.X || Y1 != point1.Y || Y2 != point2.Y || StepX != stepPoint.X || StepY != stepPoint.Y)
            {
                this.X1 = point1.X;
                this.X2 = point2.X;
                this.Y1 = point1.Y;
                this.Y2 = point2.Y;
                this.StepX = stepPoint.X;
                this.StepY = stepPoint.Y;
                this.Points = new PointCollection(new Point[] { point1, stepPoint, point2 });
            }
        }
        
        /// <summary>
        /// Draw3s the D segment.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            Point3D point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y, this.Series.Area.EnableDepthAxis && m_point1.Values.Length > 1 ? m_point1.Values[1] : 0.2);
            point1=new Point3D(point1.X,point1.Y,this.Series.Area.EnableDepthAxis==true?(1-point1.Z):0.2);
            Point3D point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y, this.Series.Area.EnableDepthAxis && m_point2.Values.Length > 1 ? m_point2.Values[1] : 0.2);
            point2 = new Point3D(point2.X, point2.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point2.Z) : 0.2);
            Point3D stepPoint = transformer.TransformToVisible(m_stepPoint.X, m_stepPoint.Values[0], this.Series.Area.EnableDepthAxis && m_point1.Values.Length > 1 ? m_point1.Values[1] : 0.2);
            stepPoint=new Point3D(stepPoint.X,stepPoint.Y,this.Series.Area.EnableDepthAxis == true ?(1-stepPoint.Z):0.2);
            point1.Y = 1 - point1.Y;
            point2.Y = 1 - point2.Y;
            stepPoint.Y = 1 - stepPoint.Y;

            Geometry3D.Geometry = MeshGenerator.StepLine(point1, point2, stepPoint);

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
            Geometry3D.Transform = new TranslateTransform3D(-0.5, -0.5, (this.Series.Area.Series.IndexOf(this.Series) + 3) * 0.05);
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
        }
        #endregion
    }
    
    /// <summary>
    /// Represents ChartStepLineType class
    /// </summary>
    /// <remarks>
    /// Step Line Charts use horizontal and vertical lines to connect data points
    /// resulting in a step like progression.
    /// </remarks>
    /// <seealso cref="ChartStepLineSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartStepLineType : ChartType
    {
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
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                ChartPoint stepPoint = new ChartPoint(points[i].DataPoint.X, points[i + 1].DataPoint.Values[0]);
                stepPoint.Values = points[i+1].DataPoint.Values;
                if (points[i].DataPoint.EmptyPoint)
                {
                    if(series.ShowEmptyPoints )
                    {
                    if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                    {
                        series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
 
                    }
                    else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                    {
                        series.Segments.Add(new ChartStepLineSegment(points[i].DataPoint, stepPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                        series.Segments[series.Segments.Count -1].Interior = series.EmptyPointInterior;
                        if (series.Segments.Count > 2)
                        {
                            series.Segments[series.Segments.Count - 2].Interior = series.EmptyPointInterior;
                        }

                    }
                    else 
                    {
                        series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
 
                    }
                    }

                }
               
                else
                {                   
                        series.Segments.Add(new ChartStepLineSegment(points[i].DataPoint, stepPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                        if (points[i +1].DataPoint.EmptyPoint && (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior))
                        {
                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;                       
                        }                   
                    if(points[i+1].DataPoint.EmptyPoint && !(series.ShowEmptyPoints))
                    {
                        series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;                       
                    }
                }
               
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
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
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartStepLineType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartStepLineType"/>
        public override string ToString()
        {
            return "StepLine";
        }
    }
}
