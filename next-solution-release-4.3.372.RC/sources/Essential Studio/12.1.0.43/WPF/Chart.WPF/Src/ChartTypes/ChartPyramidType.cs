// <copyright file="ChartPyramidType.cs" company="Syncfusion">
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
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Shapes;   
    using System.Windows.Input;
    using System.Globalization;
    using System.Windows.Media.Media3D;
    using Syncfusion.Windows.Shared;
    using System.Windows.Controls;

   

    /// <summary>
    /// Class represents adornment that is specific to axesless chart types.
    /// </summary>
    /// <remarks>
    /// ChartSeries Adornments are used to display values in a chart segment related to
    /// it. Values from data points such as X value, Y value or other properties from
    /// data source can be displayed. 
    /// <para><b>Note: </b>This will not require any additional settings for adornments.
    /// Adornments could be added as shown below. <b>ChartAccumulationAdornment</b>
    /// class will be internally used to set adornments for axesless chart types.</para>
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// <!--Chart with Adornments-->
    ///   &lt;sfchart:Chart&gt;
    ///             &lt;sfchart:ChartArea Background="LightGray" GridBackground="White"&gt;  
    ///                 &lt;sfchart:ChartSeries Type="Column" &gt;
    ///                &lt;sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                         &lt;sfchart:ChartAdornmentInfo 
    /// LabelContentPath="DataPoint.X" Visible="True"  /&gt;
    ///                     &lt;/sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;       
    ///             &lt;/sfchart:ChartArea&gt;           
    ///         &lt;/sfchart:Chart&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// ChartSeries series = Chart1.Areas[0].Series[0];      
    /// ChartAdornmentInfo adornments = series.AdornmentsInfo;
    /// adornments.LabelContentPath = "DataPoint.X";
    /// adornments.Visible = true;
    /// </code>
    /// </example>
    /// <seealso cref="ChartAdornment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAccumulationAdornment : ChartAdornment
    {
        #region Members
        /// <summary>
        /// Initializes m_y
        /// </summary>
        private double m_y = 0;

        /// <summary>
        /// Initializes m_x
        /// </summary>
        private double m_x = 0;
        #endregion

        /// <summary>
        /// Identifies the ExplodedOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedOffsetProperty =
            DependencyProperty.Register("ExplodedOffset", typeof(double), typeof(ChartAccumulationAdornment), new ChartPropertyMetadata(15d, ChartPropertyMetadataOptions.AffectsRedraw));

        /// <summary>
        /// Gets or sets the exploded offset. This is a dependency property.
        /// </summary>
        /// <value>The exploded offset.</value>
        public double ExplodedOffset
        {
            get
            {
                return (double)GetValue(ExplodedOffsetProperty);
            }

            set
            {
                SetValue(ExplodedOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum radius.
        /// </summary>
        /// <value>The min <see cref="Double"/> radius.</value>
        public double MinRadius
        {
            get;
            set;
        }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAccumulationAdornment"/> class.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        public ChartAccumulationAdornment(double startAngle, double endAngle, ChartIndexedDataPoint point, ChartSeries series)
            : base(new ChartPoint(0, 0), point, series)
        {
            m_y = startAngle;
            m_x = endAngle;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of the class that implements <see cref="IChartTransformer"/></param>
        public override void Update(IChartTransformer transformer)
        {
            Rect viewport = Rect.Inflate(transformer.Viewport, -this.ExplodedOffset, 0);
            ////Rect viewport = transformer.Viewport;

            double center = viewport.Width / 2;
            ////double minRadius = 0.5 * (1d - 40d / viewport.Width);

            double radius = Math.Max(Math.Abs(m_x * viewport.Width - center), MinRadius);
            if (m_x * viewport.Width < center)
            {
                this.X = viewport.X + center - radius;
            }
            else
            {
                this.X = viewport.X + center + radius;
            }
            ////this.X = viewport.X + center + Math.Max(MinRadius, m_x * viewport.Width - center);
            ////if (MinRadius > (m_x * viewport.Width - center))
            ////{
            ////this.X = viewport.Width viewport.X + m_x * viewport.Width;
            ////}
            ////else
            ////{
            //// this.X = viewport.X + m_x * viewport.Width;
            ////}

            ////this.X = viewport.X + m_x * viewport.Width;
            this.Y = viewport.Y + m_y * viewport.Height;
        }
        #endregion
    }

    /// <summary>
    /// Represents Pyramid chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartPyramidType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartPyramidSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_y
        /// </summary>
        private double m_y = 0d;

        /// <summary>
        /// Initializes m_height
        /// </summary>
        private double m_height = 0d;

        /// <summary>
        /// Initializes m_viewport
        /// </summary>
        private Rect m_viewport;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartPyramidSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsExploded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(ChartPyramidSegment), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsRedraw));

        /// <summary>
        /// Identifies the ExplodedOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedOffsetProperty =
            DependencyProperty.Register("ExplodedOffset", typeof(double), typeof(ChartPyramidSegment), new ChartPropertyMetadata(15d, ChartPropertyMetadataOptions.AffectsRedraw));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the geometry. This is a dependency property.
        /// </summary>
        /// <value>The geometry.</value>
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is exploded. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is exploded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExploded
        {
            get { return (bool)GetValue(IsExplodedProperty); }
            set { SetValue(IsExplodedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the exploded offset. This is a dependency property.
        /// </summary>
        /// <value>The exploded offset.</value>
        public double ExplodedOffset
        {
            get { return (double)GetValue(ExplodedOffsetProperty); }
            set { SetValue(ExplodedOffsetProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartPyramidSegment"/> class.
        /// </summary>
        /// <remarks>
        /// During initialization the default template for Kagi segment is being created.
        /// </remarks>
        static ChartPyramidSegment()
        {
            Type type = typeof(ChartPyramidSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPyramidSegment"/> class.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="height">The height.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        public ChartPyramidSegment(double y, double height, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            m_y = y;
            m_height = height;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartPyramidSegment"/>
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
            ////Rect rect = transformer.Viewport;
            Rect rect = Rect.Inflate(transformer.Viewport, -this.ExplodedOffset, 0);
            if (m_viewport != rect)
            {
                if (rect.IsEmpty)
                {
                    this.Geometry = null;
                }
                else
                {
                    PathFigure figure = new PathFigure();

                    if (this.IsExploded)
                    {
                        rect.X += this.ExplodedOffset;
                    }

                    if (m_height != 0)
                    {
                        double top = m_y;
                        double bottom = m_y + m_height;
                        double topRadius = 0.5d * (1d - m_y);
                        double bottomRadius = 0.5d * (1d - bottom);

                        figure.StartPoint = new Point(rect.X + topRadius * rect.Width, rect.Y + top * rect.Height);
                        figure.Segments.Add(new LineSegment(new Point(rect.X + (1 - topRadius) * rect.Width, rect.Y + top * rect.Height), true));
                        figure.Segments.Add(new LineSegment(new Point(rect.X + (1 - bottomRadius) * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2), true));
                        figure.Segments.Add(new LineSegment(new Point(rect.X + bottomRadius * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2), true));
                        figure.IsClosed = true;

                        this.Geometry = new PathGeometry(new PathFigure[] { figure });
                    }
                }

                m_viewport = rect;
            }
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            if (this.CorrespondingPoints[0].DataPoint.Visible)
            {
                //double top = m_y;
                //double bottom = m_y + m_height;
                //double topRadius = 0.5d * (1d - m_y);
                //double bottomRadius = 0.5d * (1d - bottom);

                double top = m_y;
                double bottom = m_y + m_height;
                double topRadius = 0.5d * (m_y);
                double bottomRadius = 0.5d * (bottom);

                Geometry3D.Geometry = MeshGenerator.Cone(topRadius, bottomRadius, m_height - 0.0001, 100, this.IsExploded);
                MaterialGroup materialGroup;
                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                

                //TextBlock tb = new TextBlock();
                //tb.Margin = new Thickness(5);
                //tb.Height = 5;
                //tb.Width = 5;
                //tb.Text = "D";
                //tb.FontSize = 6;
                //tb.VerticalAlignment = VerticalAlignment.Center;
                //tb.Foreground = Brushes.White;
                //tb.Background = Brushes.Black;
                //tb.HorizontalAlignment = HorizontalAlignment.Center;
                //DiffuseMaterial difuseMaterialVisual = new DiffuseMaterial();
                //visual.Visual = tb;
                //difuseMaterialVisual.Brush = visual;

                Binding binding = new Binding("Interior");
                binding.Source = Series;
                ChartStyleModel styleModel = Series.Area.ColorModel;
                if (!this.Series.ColorEach.HasValue || (bool)this.Series.ColorEach)
                    styleModel.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, this.CorrespondingPoints[0].Index);
                else
                    BindingUtils.SetBinding(difuseMaterial, Series, DiffuseMaterial.BrushProperty, ChartSeries.InteriorProperty);

                materialGroup = new MaterialGroup();

                //materialGroup.Children.Add(difuseMaterialColor);
                //materialGroup.Children.Add(difuseMaterialVisual);
                materialGroup.Children.Add(difuseMaterial);
                Geometry3D.Material = materialGroup;
               
                Geometry3D.Transform = new TranslateTransform3D(0, -m_y - m_height / 2 + 0.5, 0);
               
                //Transform3DGroup transformGroup = new Transform3DGroup();
                //TranslateTransform3D translateTr = new TranslateTransform3D(0, -m_y - m_height / 2 + 0.5, 0);
                //transformGroup.Children.Add(translateTr);
                //RotateTransform3D rotateTr = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 45));
                //transformGroup.Children.Add(rotateTr);
                //Geometry3D.Transform = transformGroup;

                Geometry3DGroup.Children.Add(Geometry3D);

            } 
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartPyramidType class
    /// </summary>
    /// <remarks>
    /// Pyramid chart is similar to the funnel chart. Its often used for geographical
    /// purposes. The Pyramid Chart type displays the data which when totalled will be
    /// 100%. This type of chart is a single series chart representing the data as
    /// portions of 100%, and this chart does not use any axes.
    /// </remarks>
    /// <seealso cref="ChartPyramidSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartPyramidType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the PyramidMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PyramidModeProperty =
          DependencyProperty.RegisterAttached("PyramidMode", typeof(ChartPyramidMode), typeof(ChartPyramidMode), new PropertyMetadata(ChartPyramidMode.Linear));

        /// <summary>
        /// Identifies the GapRatio dependency property.
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty =
          DependencyProperty.RegisterAttached("GapRatio", typeof(double), typeof(ChartPyramidType), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        private static readonly DependencyProperty ExplodedIndexProperty = ChartPieType.ExplodedIndexProperty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.NotRequiresAxis | ChartTypeFlags.Indexed;
            }
        }

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
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the pyramid mode.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The PyramidMode</returns>
        public static ChartPyramidMode GetPyramidMode(ChartSeries series)
        {
            return (ChartPyramidMode)series.GetValue(PyramidModeProperty);
        }

        /// <summary>
        /// Sets the pyramid mode.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetPyramidMode(ChartSeries series, ChartPyramidMode value)
        {
            series.SetValue(PyramidModeProperty, value);
        }

        /// <summary>
        /// Gets the gap ratio.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Gap Ratio</returns>
        public static double GetGapRatio(ChartSeries series)
        {
            return (double)series.GetValue(GapRatioProperty);
        }

        /// <summary>
        /// Sets the gap ratio.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetGapRatio(ChartSeries series, double value)
        {
            series.SetValue(GapRatioProperty, value);
        }

        /// <summary>
        /// Gets the exploded indices.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Exploded Index</returns>
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

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            
                bool needAdornments = series.AdornmentsInfo.Visible;

                int explodedIndex = GetExplodedIndex(series);
                int count = series.PointsCount;

                double sumValues = 0;
                double gapRatio = GetGapRatio(series);
                ChartPyramidMode pyramidMode = GetPyramidMode(series);
                List<ChartSegment> drawingList = new List<ChartSegment>(count);
                List<ChartSegment> adornmentsList = needAdornments ? new List<ChartSegment>(count) : null;
                ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];
                ChartStyleModel styleModel = series.Area.ColorModel;
                double length = 0;
                for (int i = 0; i < count; i++)
                {
                    cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
                    if (cdpwiA[i].DataPoint.Visible)
                    {
                        sumValues += Math.Max(0, Math.Abs(cdpwiA[i].DataPoint.Y));
                        length++;
                    }
                }

                Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

                double currY = 0;
                double gapHieght = gapRatio / (length - 1);

                if (pyramidMode == ChartPyramidMode.Linear)
                {
                    #region Linear Mode
                    double coef = 1d / (sumValues * (1 + gapRatio / (1 - gapRatio)));

                    for (int i = 0; i < cdpwiA.Length; i++)
                    {
                        double height = 0;

                        ChartPyramidSegment segment;

                        if (cdpwiA[i].DataPoint.Visible)
                        {
                            height = coef * Math.Abs(cdpwiA[i].DataPoint.Values[0]);
                            segment = new ChartPyramidSegment(currY, height, cdpwiA[i], series);
                        }
                        else
                        {
                            segment = new ChartPyramidSegment(currY, 0, cdpwiA[i], series);
                        }

                        cdpwiA[i].DataPoint.ParentSegment = segment;
                        segment.IsExploded = cdpwiA[i].Index == explodedIndex;
                        if (cdpwiA[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                segment.Interior = series.EmptyPointInterior;
                            }
                            else
                            {
                                segment.Interior = Brushes.Transparent;
                            }
                        }
                        else
                        {
                            styleModel.SetBinding(segment, ChartSegment.InteriorProperty, i);
                        }
                        if (cdpwiA[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                drawingList.Add(segment);
                            }

                        }
                        else
                        {
                            drawingList.Add(segment);
                        }

                        if (cdpwiA[i].DataPoint.Visible)
                        {
                            if (needAdornments)
                            {
                                CreateAdornment(cdpwiA[i], series, adornmentsList, currY, height, sumValues);
                                ////adornmentsList.Add(new ChartAccumulationAdornment(currY + height / 2, cdpwiA[i], series));
                            }

                            currY += gapHieght + height;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Surface Mode
                    double[] ys = new double[cdpwiA.Length];
                    double[] heights = new double[cdpwiA.Length];
                    double preSum = GetSurfaceHeight(0, sumValues);

                    for (int i = 0; i < cdpwiA.Length; i++)
                    {
                        ys[i] = currY;
                        heights[i] = GetSurfaceHeight(currY, Math.Abs(cdpwiA[i].DataPoint.Values[0]));
                        if (cdpwiA[i].DataPoint.Visible)
                        {
                            currY += heights[i] + gapHieght * preSum;
                        }
                    }

                    double coef = 1 / (currY - gapHieght * preSum);
                    ChartPyramidSegment segment;
                    for (int i = 0; i < cdpwiA.Length; i++)
                    {
                        if (cdpwiA[i].DataPoint.Visible)
                        {
                            segment = new ChartPyramidSegment(coef * ys[i], coef * heights[i], cdpwiA[i], series);
                        }
                        else
                        {
                            segment = new ChartPyramidSegment(0, 0, cdpwiA[i], series);
                        }

                        cdpwiA[i].DataPoint.ParentSegment = segment;

                        segment.IsExploded = cdpwiA[i].Index == explodedIndex;
                        styleModel.SetBinding(segment, ChartSegment.InteriorProperty, i);

                        drawingList.Add(segment);

                        if (needAdornments && cdpwiA[i].DataPoint.Visible)
                        {
                            CreateAdornment(cdpwiA[i], series, adornmentsList, coef * ys[i], coef * heights[i], sumValues);
                            ////adornmentsList.Add(new ChartAccumulationAdornment(coef * (ys[i] + heights[i] / 2), cdpwiA[i], series));
                        }
                    }
                    #endregion
                }

                if (needAdornments)
                {
                    drawingList.AddRange(adornmentsList);
                }

                foreach (ChartSegment segment in drawingList)
                {
                    series.Segments.Add(segment);
                }
                if (series.Area.View3DMode)
                    series.Update3D();
        }

        /// <summary>
        /// Creates an adornment.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        /// <param name="adornmentsList">The adornments list.</param>
        /// <param name="currY">The curr Y.</param>
        /// <param name="height">The height.</param>
        /// <param name="sumValues">The sum values.</param>
        private static void CreateAdornment(ChartIndexedDataPoint point, ChartSeries series, List<ChartSegment> adornmentsList, double currY, double height, double sumValues)
        {
            double x = 0;
            double y = 0;

            ////double topRadius = (currY) / 2;
            ////double bottomRadius = (currY + height) / 2;
            double topRadius = 0.5d * currY;
            double bottomRadius = 0.5d * (currY + height);
            switch (series.AdornmentsInfo.SegmentVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    y = currY + height * 0.75;
                    break;
                case VerticalAlignment.Center:
                    y = currY + height * 0.5;
                    break;
                case VerticalAlignment.Top:
                    y = currY + height * 0.25;
                    break;
                case VerticalAlignment.Stretch:
                    y = currY + height * 0.5;
                    break;
            }

            if (series.AdornmentsInfo.SegmentShowLine)
            {
                double radi = -(topRadius - bottomRadius);
                double h = y - currY;
                double w = h * radi / height;
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = 0.5;
                        break;
                    case HorizontalAlignment.Left:
                        x = 0.5 - topRadius - w;
                        ////-0.1;
                        break;
                    case HorizontalAlignment.Right:
                        x = 0.5 + topRadius + w;
                        ////+0.1;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                }
            }
            else if (series.AdornmentsInfo.SegmentIsOut)
            {
                double radi = -(topRadius - bottomRadius);
                double h = y - currY;
                double w = h * radi / height;
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = 0.5;
                        break;
                    case HorizontalAlignment.Left:
                        x = 0.5 - topRadius - w - 0.05;
                        ////-0.1;
                        break;
                    case HorizontalAlignment.Right:
                        x = 0.5 + topRadius + w + 0.05;
                        ////+0.1;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                }
            }
            else
            {
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                    case HorizontalAlignment.Left:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.15;
                        break;
                    case HorizontalAlignment.Right:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.85;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                }
            }

            ChartAccumulationAdornment ador = new ChartAccumulationAdornment(y, x, point, series);

            switch (series.AdornmentsInfo.SegmentLabelContent)
            {
                case LabelContent.Percentage:
                    ador.SegmentLabel = (point.DataPoint.Y * 100 / sumValues).ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture) + "%";
                    break;
                case LabelContent.XValue:
                    ador.SegmentLabel = point.DataPoint.X.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.YValue:
                    ador.SegmentLabel = point.DataPoint.Y.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.YofTot:
                    ador.SegmentLabel = point.DataPoint.Y.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture) + " of " + sumValues.ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.DateTime:
                    ador.SegmentLabel = DateTime.FromOADate(point.DataPoint.X).ToString(series.AdornmentsInfo.SegmentLabelDataTimeFormat, CultureInfo.CurrentCulture);
                    break;
            }

            if (series.ShowEmptyPoints == false)
            {
                if (point.DataPoint.EmptyPoint == false)
                    series.Adornments.Add(ador);
            }
            else
            {
                series.Adornments.Add(ador);
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <seealso cref="ChartPyramidType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }

        /// <summary>
        /// Gets the height of the surface.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="surface">The surface.</param>
        /// <returns>The Surface Height</returns>
        public static double GetSurfaceHeight(double y, double surface)
        {
            double r1, r2;

            if (ChartMath.SolveQuadraticEquation(1, 2 * y, -surface, out r1, out r2))
            {
                return Math.Max(r1, r2);
            }

            return double.NaN;
        }
        #endregion
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
        /// <seealso cref="ChartPyramidType"/>
        public override string ToString()
        {
            return "Pyramid";
        }
    }
}
