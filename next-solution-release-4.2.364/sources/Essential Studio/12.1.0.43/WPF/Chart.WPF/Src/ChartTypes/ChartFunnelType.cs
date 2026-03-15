// <copyright file="ChartFunnelType.cs" company="Syncfusion">
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
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Shared;

  

    /// <summary>
    /// Represents Funnel chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartFunnelType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartFunnelSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartFunnelSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the MinWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(ChartFunnelSegment), new PropertyMetadata(40d));

        /// <summary>
        /// Identifies the IsExploded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(ChartFunnelSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the ExplodedOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedOffsetProperty =
            DependencyProperty.Register("ExplodedOffset", typeof(double), typeof(ChartFunnelSegment), new PropertyMetadata(15d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width of the minimal segment part. This is a dependency property.
        /// </summary>
        /// <value>The width of the min.</value>
        public double MinWidth
        {
            get
            {
                return (double)GetValue(MinWidthProperty);
            }

            set
            {
                SetValue(MinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the geometry. This is a dependency property. This is a dependency property.
        /// </summary>
        /// <value>The geometry.</value>
        public Geometry Geometry
        {
            get
            {
                return (Geometry)GetValue(GeometryProperty);
            }

            set
            {
                SetValue(GeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment is exploded. This is a dependency property.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is exploded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExploded
        {
            get
            {
                return (bool)GetValue(IsExplodedProperty);
            }

            set
            {
                SetValue(IsExplodedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment exploded offset. This is a dependency property.
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
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_top, m_bottom, m_topRadius, m_bottomRadius
        /// </summary>
        private double m_top, m_bottom, m_topRadius, m_bottomRadius;

        /// <summary>
        /// Initializes m_viewport
        /// </summary>
        private Rect m_viewport;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartFunnelSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment's template is being assigned automatically.
        /// </remarks>
        static ChartFunnelSegment()
        {
            Type type = typeof(ChartFunnelSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFunnelSegment"/> class.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="height">The height.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartFunnelSegment(double y, double height, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            m_top = y;
            m_bottom = y + height;
            m_topRadius = y / 2;
            m_bottomRadius = (y + height) / 2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFunnelSegment"/> class.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="height">The height.</param>
        /// <param name="widthTop">The width top.</param>
        /// <param name="widthBottom">The width bottom.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartFunnelSegment(double y, double height, double widthTop, double widthBottom, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            if (correspondingPoint.DataPoint.Visible)
            {
                m_top = y;
                m_bottom = y + height;
                m_topRadius = (1 - widthTop) / 2;
                m_bottomRadius = (1 - widthBottom) / 2;
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
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
            //else
            //{
            //    this.Stroke = Series.Stroke;
            //    this.StrokeThickness = Series.StrokeThickness;
            //}

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

                    double top = m_top;
                    double bottom = m_bottom;

                    double minRadius = 0.5 * (1d - this.MinWidth / rect.Width);
                    bool isBroken = (m_topRadius >= minRadius) ^ (m_bottomRadius > minRadius);
                    double bY = minRadius * (m_bottom - m_top) / (m_bottomRadius - m_topRadius);
                    double topRadius = Math.Min(m_topRadius, minRadius);
                    double bottomRadius = Math.Min(m_bottomRadius, minRadius);

                    figure.StartPoint = new Point(rect.X + topRadius * rect.Width, rect.Y + top * rect.Height);
                    figure.Segments.Add(new LineSegment(new Point(rect.X + (1 - topRadius) * rect.Width, rect.Y + top * rect.Height), true));

                    if (isBroken)
                    {
                        figure.Segments.Add(new LineSegment(new Point(rect.X + (1 - minRadius) * rect.Width, rect.Y + bY * rect.Height), true));
                    }

                    figure.Segments.Add(new LineSegment(new Point(rect.X + (1 - bottomRadius) * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2), true));
                    figure.Segments.Add(new LineSegment(new Point(rect.X + bottomRadius * rect.Width, rect.Y + bottom * rect.Height - Series.StrokeThickness / 2), true));

                    if (isBroken)
                    {
                        figure.Segments.Add(new LineSegment(new Point(rect.X + minRadius * rect.Width, rect.Y + bY * rect.Height), true));
                    }

                    figure.IsClosed = true;

                    this.Geometry = new PathGeometry(new PathFigure[] { figure });
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
                //Rect rect = Rect.Inflate(transformer.Viewport, -this.ExplodedOffset / 100, 0);
                //if (this.IsExploded)
                //{
                //    rect.X += this.ExplodedOffset;
                //}
                double top =m_top;
                double bottom = m_bottom;
                double minRadius = 0.1;
                double topRadius =1.5 * (0.5 - m_topRadius);
                double bottomRadius =1.5 * (0.5 - m_bottomRadius);

                //bool isBroken = (m_topRadius >= minRadius) ^ (m_bottomRadius > minRadius);
                //double bY = minRadius * (m_bottom - m_top) / (m_bottomRadius - m_topRadius);
                //double topRadius = Math.Max(m_topRadius, minRadius);
                //double bottomRadius = Math.Max(m_bottomRadius, minRadius);
                //double height = top - bottom;
                
                double bY = 0.5 * (1d - this.MinWidth / 384) * (m_bottom - m_top) / (m_bottomRadius - m_topRadius);
                bool isBroken =Math.Round(bottomRadius,5) <= 0;
                double brokenPartRadius = Math.Min(topRadius, minRadius);
                Point[] points;
                if (isBroken)
                {
                    points = new Point[5];
                    points[0] = new Point(top, 0);
                    points[1] = new Point(top, topRadius);
                    points[2] = new Point(bY, brokenPartRadius);
                    points[3] = new Point(bottom, brokenPartRadius);
                    points[4] = new Point(bottom, 0);
                }
                else
                {
                    points = new Point[4];
                    points[0] = new Point(top, 0);
                    points[1] = new Point(top, topRadius);
                    points[2] = new Point(bottom, bottomRadius);
                    points[3] = new Point(bottom, 0);
                }

                Geometry3D.Geometry = ChartMeshGeometry3DBuilder.BuildCylindricPolyline(points, ChartMeshGeometry3DAxis.Y, 40, true,this.IsExploded);
                MaterialGroup materialGroup;

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                ChartStyleModel styleModel = Series.Area.ColorModel;
                if (!this.Series.ColorEach.HasValue || (bool)this.Series.ColorEach)
                    styleModel.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, this.CorrespondingPoints[0].Index);
                else
                    BindingUtils.SetBinding(difuseMaterial, Series, DiffuseMaterial.BrushProperty, ChartSeries.InteriorProperty);
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);

                Geometry3D.Material = materialGroup;
                Transform3DGroup transformGroup = new Transform3DGroup();
                TranslateTransform3D translateTr = new TranslateTransform3D(0, 0.5, 0);
                RotateTransform3D rotateTr = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 180));
                transformGroup.Children.Add(rotateTr);

                transformGroup.Children.Add(translateTr);
                Geometry3D.Transform = transformGroup;
            }
        }
        #endregion

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
            this.Geometry = null;
            this.Geometry3D = null;            
            base.Dispose();
        }
    }

    /// <summary>
    /// Represents ChartFunnelType class
    /// </summary>
    /// <remarks>
    /// The Funnel chart is a single series chart representing the data as portions of
    /// 100%, and this chart does not use any axes. Funnel chart can be viewed as 2D or
    /// 3D.
    /// </remarks>
    /// <seealso cref="ChartFunnelSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartFunnelType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the FunnelMode dependency property.
        /// </summary>
        public static readonly DependencyProperty FunnelModeProperty =
          DependencyProperty.RegisterAttached("FunnelMode", typeof(ChartFunnelMode), typeof(ChartFunnelType), new ChartPropertyMetadata(ChartFunnelMode.YIsHeight, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the GapRatio dependency property.
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty = ChartPyramidType.GapRatioProperty.AddOwner(typeof(ChartFunnelType));

        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedIndexProperty = ChartPieType.ExplodedIndexProperty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets axes type that are required for chart to be built.
        /// </summary>
        /// <value></value>
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
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.NotRequiresAxis | ChartTypeFlags.Indexed;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Gets the gap ratio.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The gap ratio</returns>
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
        /// Gets the funnel mode.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Funnel mode</returns>
        public static ChartFunnelMode GetFunnelMode(ChartSeries series)
        {
            return (ChartFunnelMode)series.GetValue(FunnelModeProperty);
        }

        /// <summary>
        /// Sets the funnel mode.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetFunnelMode(ChartSeries series, ChartFunnelMode value)
        {
            series.SetValue(FunnelModeProperty, value);
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
        ///  <c>true</c> if the type is compatible; otherwise, <c>false</c>.
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
            series.Adornments.Clear();
            double sumValues = 0;
            double gapRatio = GetGapRatio(series);
            int explodedIndex = GetExplodedIndex(series);
            int count = series.PointsCount;
            ChartFunnelMode funnelMode = GetFunnelMode(series);
            ////List<ChartSegment> drawingList = new List<ChartSegment>(count);
            ////List<ChartSegment> adornmentsList = needAdornments ? new List<ChartSegment>(count) : null;
            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];
            ChartStyleModel styleModel = series.Area.ColorModel;
            double length = 0;

            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
                if (cdpwiA[i].DataPoint.Visible)
                {
                    sumValues += Math.Max(0, Math.Abs(cdpwiA[i].DataPoint.Y));
                    ////Looking thru all visible points to count them.
                    length++;
                }
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            double currY = 0;

            if (funnelMode == ChartFunnelMode.YIsHeight)
            {
                double spacing = gapRatio / (length - 1);
                double coefHeight = (1 - gapRatio) / sumValues;

                for (int i = cdpwiA.Length - 1; i >= 0; i--)
                {
                    double height = 0;
                    ChartFunnelSegment segment;
                    if (cdpwiA[i].DataPoint.Visible)
                    {
                        height = Math.Abs(cdpwiA[i].DataPoint.Values[0]);
                        segment = new ChartFunnelSegment(currY, coefHeight * height, cdpwiA[i], series);
                    }
                    else
                    {
                        segment = new ChartFunnelSegment(currY, coefHeight, cdpwiA[i], series);
                    }

                    cdpwiA[i].DataPoint.ParentSegment = segment;

                    segment.IsExploded = cdpwiA[i].Index == explodedIndex;
                    if (cdpwiA[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            segment.Interior = series.EmptyPointInterior.Clone();
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
                            series.Segments.Add(segment);
                        }

                    }
                    else
                    {
                        series.Segments.Add(segment);
                    }
                    

                    if (cdpwiA[i].DataPoint.Visible)
                    {
                        if (needAdornments)
                        {
                            CreateAdornment(cdpwiA[i], series, currY, coefHeight * height, sumValues);
                        }

                        currY += spacing + coefHeight * height;
                    }
                }
            }
            else
            {
                ////Calculating offset & height with respect to points visibility.
                double offset = 1d / (length - 1);
                double height = (1 - gapRatio) / (length - 1);

                ////Go thru all points.
                for (int i = 1; i < cdpwiA.Length; i++)
                {
                    double w1 = Math.Abs(cdpwiA[i - 1].DataPoint.Values[0]);
                    double w2 = Math.Abs(cdpwiA[i].DataPoint.Values[0]);
                    ChartFunnelSegment segment;
                    ////If corresponding point should not be visible.
                    if (cdpwiA[i].DataPoint.Visible)
                    {
                        ////Previous point is not visible.
                        if (!cdpwiA[i - 1].DataPoint.Visible)
                        {
                            ////Go backwards till visible point is found.
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (cdpwiA[j].DataPoint.Visible)
                                {
                                    ////Change w1 value to visible point value.
                                    w1 = Math.Abs(cdpwiA[j].DataPoint.Values[0]);
                                    break;
                                }
                            }
                        }
                        ////Creating real segment.
                        segment = new ChartFunnelSegment(currY, height, w1 / sumValues, w2 / sumValues, cdpwiA[i], series);
                    }
                    else
                    {
                        ////Creating empty segment just to keek all segments in a series.
                        segment = new ChartFunnelSegment(0, 0, 0, 0, cdpwiA[i], series);
                    }

                    cdpwiA[i].DataPoint.ParentSegment = segment;
                    segment.IsExploded = cdpwiA[i].Index == explodedIndex;
                    styleModel.SetBinding(segment, ChartFunnelSegment.InteriorProperty, cdpwiA[i].Index);

                    series.Segments.Add(segment);

                    ////If current datapoint is visible - fill adornments info and increase the counter.
                    if (cdpwiA[i].DataPoint.Visible)
                    {
                        if (needAdornments)
                        {
                            ////Add an adornment information.
                            CreateAdornment(cdpwiA[i], series, currY, height, w1 / sumValues, w2 / sumValues, sumValues);
                        }

                        currY += offset;
                    }
                }
            }
            if (series.Area.View3DMode)
                series.Update3D();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartFunnelType"/>
        public override string ToString()
        {
            return "Funnel";
        }

        /// <summary>
        /// Updates the series.
        /// </summary>
        /// <param name="series">The Chart series</param>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }

        /// <summary>
        /// Creates an adornment.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        /// <param name="currY">The curr Y.</param>
        /// <param name="height">The height.</param>
        /// <param name="sumValues">The sum values.</param>
        private static void CreateAdornment(ChartIndexedDataPoint point, ChartSeries series, double currY, double height, double sumValues)
        {
            double x = 0;
            double y = 0;

            double topRadius = (1 - currY) / 2;
            double bottomRadius = (1 - currY - height) / 2;
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
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.25;
                        break;
                    case HorizontalAlignment.Right:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.75;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                }
            }

            ChartAccumulationAdornment ador = new ChartAccumulationAdornment(y, x, point, series);
            ador.MinRadius = 0;

            switch (series.AdornmentsInfo.SegmentLabelContent)
            {
                case LabelContent.Percentage:
                    ador.SegmentLabel = (point.DataPoint.Y * 100 / sumValues).ToString(series.AdornmentsInfo.SegmentLabelFormat, CultureInfo.InvariantCulture) + "%";
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
            ////adornmentsList.Add(new ChartAccumulationAdornment(currY + 0.5 * coefHeight * height, cdpwiA[i], series));
            ////adornmentsList.Add(new ChartAccumulationAdornment(currY + height / 2, cdpwiA[i], series));
        }

        /// <summary>
        /// Creates an adornment.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        /// <param name="currY">The curr Y.</param>
        /// <param name="height">The height.</param>
        /// <param name="topRadius">The top radius.</param>
        /// <param name="bottomRadius">The bottom radius.</param>
        /// <param name="sumValues">The sum values.</param>
        private static void CreateAdornment(ChartIndexedDataPoint point, ChartSeries series, double currY, double height, double topRadius, double bottomRadius, double sumValues)
        {
            double x = 0;
            double y = 0;

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

            if (series.AdornmentsInfo.SegmentIsOut)
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
                        break;
                    case HorizontalAlignment.Right:
                        x = 0.5 + topRadius + w + 0.05;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                }
            }
            else if (series.AdornmentsInfo.SegmentShowLine)
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
            else
            {
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.5;
                        break;
                    case HorizontalAlignment.Left:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.25;
                        break;
                    case HorizontalAlignment.Right:
                        x = (1 - bottomRadius) / 2 + bottomRadius * 0.75;
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

            ////adornmentsList.Add(ador);
            series.Adornments.Add(ador);
        }

        #endregion
    }
}
