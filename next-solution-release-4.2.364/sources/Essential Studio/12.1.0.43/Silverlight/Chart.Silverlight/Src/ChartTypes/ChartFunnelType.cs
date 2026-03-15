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
    /// Lists the funnel mode options.
    /// </summary>
    public enum ChartFunnelMode
    {
        /// <summary>
        /// The specified Y value is used to compute the width of the corresponding block.
        /// </summary>
        YIsWidth,

        /// <summary>
        /// The specified Y value is used to compute the height of the corresponding block.
        /// </summary>
        YIsHeight
    }

    /// <summary>
    /// Represents Funnel chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartFunnelType"/>
    public sealed class ChartFunnelSegment : Segment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartFunnelSegment), new PropertyMetadata(null));

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
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

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
        /// Initializes a new instance of the <see cref="ChartFunnelSegment"/> class.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="height">The height.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartFunnelSegment(double y, double height, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartPyramidType), ChartTypes.Funnel);
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
        internal ChartFunnelSegment(double y, double height, double widthTop, double widthBottom, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
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

            if (correspondingPoint.Visible)
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
            Rect rect = ChartPyramidSegment.Inflate(transformer.Viewport, -this.ExplodedOffset, 0);
            if (m_viewport != rect)
            {
                if (rect.IsEmpty)
                {
                    this.Geometry = null;
                }
                else
                {
                    PathFigure figure = new PathFigure();
                    PathFigureCollection figures = new PathFigureCollection();

                    if (this.IsExploded)
                    {
                        rect.X += this.ExplodedOffset;
                    }

                    double top = m_top;
                    double bottom = m_bottom;

                    double minRadius = 0.5 * (1d - (this.MinWidth / rect.Width));
                    bool isBroken = (m_topRadius >= minRadius) ^ (m_bottomRadius > minRadius);
                    double bY = minRadius * (m_bottom - m_top) / (m_bottomRadius - m_topRadius);
                    double topRadius = Math.Min(m_topRadius, minRadius);
                    double bottomRadius = Math.Min(m_bottomRadius, minRadius);

                    figure.StartPoint = new Point(rect.X + (topRadius * rect.Width), rect.Y + (top * rect.Height));
                    figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + ((1 - topRadius) * rect.Width), rect.Y + (top * rect.Height)) });

                    if (isBroken)
                    {
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + ((1 - minRadius) * rect.Width), rect.Y + (bY * rect.Height)) });
                    }

                    figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + ((1 - bottomRadius) * rect.Width), rect.Y + (bottom * rect.Height) - (Series.StrokeThickness / 2)) });
                    figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + (bottomRadius * rect.Width), rect.Y + (bottom * rect.Height) - (Series.StrokeThickness / 2)) });

                    if (isBroken)
                    {
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + (minRadius * rect.Width), rect.Y + (bY * rect.Height)) });
                    }

                    figure.IsClosed = true;
                    figures.Add(figure);
                    this.Geometry = new PathGeometry() { Figures = figures };
                }

                m_viewport = rect;
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
    /// Represents ChartFunnelType class
    /// </summary>
    /// <remarks>
    /// The Funnel chart is a single series chart representing the data as portions of
    /// 100%, and this chart does not use any axes. Funnel chart can be viewed as 2D or
    /// 3D.
    /// </remarks>
    /// <seealso cref="ChartFunnelSegment"/>
    public class ChartFunnelType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the FunnelMode dependency property.
        /// </summary>
        public static readonly DependencyProperty FunnelModeProperty =
          DependencyProperty.RegisterAttached("FunnelMode", typeof(ChartFunnelMode), typeof(ChartFunnelType), new PropertyMetadata(ChartFunnelMode.YIsHeight, new PropertyChangedCallback(OnDataChanged)));

       /// <summary>
        /// Identifies the GapRatio dependency property.
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty =
          DependencyProperty.RegisterAttached("GapRatio", typeof(double), typeof(ChartFunnelType), new PropertyMetadata(0d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedIndexProperty =
            DependencyProperty.RegisterAttached("ExplodedIndex", typeof(int), typeof(ChartFunnelType), new PropertyMetadata(-1, new PropertyChangedCallback(OnDataChanged)));
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
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            bool needAdornments = series.AdornmentsInfo != null && series.AdornmentsInfo.Visible;
            series.Adornments.Clear();
            double sumValues = 0;
            double gapRatio = GetGapRatio(series);
            int explodedIndex = GetExplodedIndex(series);
            int count = points.Count;
            Brush[] brush = series.Area.ColorModel.CurrentPalette;
            ChartFunnelMode funnelMode = GetFunnelMode(series);
            double[] datapoints = null;
            ChartStyleModel styleModel = series.Area.ColorModel;
            double length = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
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
            }
            if (points.Count != 0)
            {
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

            if (datapoints != null)
            {
                Array.Sort<double>(datapoints);
                length = datapoints.Length;
                double currY = 0;

                if (funnelMode == ChartFunnelMode.YIsHeight)
                {
                    #region YIsHeight Mode
                    double spacing = gapRatio / (length - 1);
                    double coefHeight = (1 - gapRatio) / sumValues;

                    for (int i = 0; i < points.Count; i++)
                    {
                        double height = 0;
                        ChartFunnelSegment segment;
                        if (points[i].Visible)
                        {
                            height = Math.Abs(points[i].Values[0]);
                            segment = new ChartFunnelSegment(currY, coefHeight * height, points[i], series);
                        }
                        else
                        {
                            segment = new ChartFunnelSegment(currY, coefHeight, points[i], series);
                            segment.Stroke = new SolidColorBrush(Colors.Transparent);
                        }
                        if (points[i].EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                height = Math.Abs(points[i].Values[0]);
                                segment = new ChartFunnelSegment(currY, coefHeight * height, points[i], series);
                                segment.Interior = segment.PaletteInterior = series.EmptyPointInterior;
                            }
                            else
                            {
                                segment.Interior = segment.PaletteInterior = new SolidColorBrush(Colors.Transparent);
                            }
                        }
                        else
                        {
                            if (brush != null)
                                segment.Interior = segment.PaletteInterior = brush[i % brush.Length];
                        }
                        segment.IsExploded = i == explodedIndex;
                        if (points[i].EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                series.Segments.Add(segment);
                                if (series.AdornmentsInfo != null)
                                    CreateAdornment(points[i], series, currY, coefHeight * height, sumValues);
                            }
                        }
                        else
                        {
                            series.Segments.Add(segment);
                        }

                        if (points[i].Visible)
                        {
                            if (needAdornments)
                            {
                                CreateAdornment(points[i], series, currY, coefHeight * height, sumValues);
                            }


                        }
                        currY += spacing + (coefHeight * height);
                    }
                    #endregion
                }
                else
                {
                    #region YIsWidth Mode
                    ////Calculating offset & height with respect to points visibility.
                    double offset = 1d / (length - 1);
                    double height = (1 - gapRatio) / (length - 1);
                    offset = double.IsInfinity(offset) ? 0 : offset;
                    height = double.IsInfinity(height) ? 0 : height;

                    ////Go thru all points.
                    for (int i = 1; i < points.Count; i++)
                    {
                        double w1 = Math.Abs(points[i - 1].Values[0]);
                        double w2 = Math.Abs(points[i].Values[0]);
                        ChartFunnelSegment segment;
                        ////If corresponding point should not be visible.
                        if (points[i].Visible)
                        {
                            ////Previous point is not visible.
                            if (!points[i - 1].Visible)
                            {
                                ////Go backwards till visible point is found.
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (points[j].Visible)
                                    {
                                        ////Change w1 value to visible point value.
                                        w1 = Math.Abs(points[j].Values[0]);
                                        break;
                                    }
                                }
                            }
                            ////Creating real segment.
                            segment = new ChartFunnelSegment(currY, height, w1 / sumValues, w2 / sumValues, points[i], series);
                        }
                        else
                        {
                            ////Creating empty segment just to keek all segments in a series.
                            segment = new ChartFunnelSegment(0, 0, 0, 0, points[i], series);
                            segment.Stroke = new SolidColorBrush(Colors.Transparent);
                        }

                        segment.IsExploded = i == explodedIndex;
                        segment.Interior = segment.PaletteInterior = brush[i % brush.Length];
                        series.Segments.Add(segment);

                        ////If current datapoint is visible - fill adornments info and increase the counter.
                        if (points[i].Visible)
                        {
                            if (needAdornments && height != 0)
                            {
                                ////Add an adornment information.
                                CreateAdornment(points[i], series, currY, height, w1 / sumValues, w2 / sumValues, sumValues);
                            }

                            currY += offset;
                        }
                    }
                    #endregion
                }
            }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        private static void CreateAdornment(ChartPoint point, ChartSeries series, double currY, double height, double sumValues)
        {
            double x = 0;
            double y = 0;

            double topRadius = (1 - currY) / 2;
            double bottomRadius = (1 - currY - height) / 2;
            double midRadius = (1 - currY - (height / 2)) / 2;
            double labelPositionRadius = 0;
            switch (series.AdornmentsInfo.SegmentVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    y = currY + (height * 0.75);
                    labelPositionRadius = (1 - currY - height * 0.75) / 2;
                    break;
                case VerticalAlignment.Center:
                    y = currY + (height * 0.5);
                    labelPositionRadius = midRadius;
                    break;
                case VerticalAlignment.Top:
                    y = currY + (height * 0.25);
                    labelPositionRadius = (1 - currY - height * 0.25) / 2; 
                    break;
                case VerticalAlignment.Stretch:
                    y = currY + (height * 0.5);
                    labelPositionRadius = midRadius;
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
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
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
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
                        break;
                }
            }
            else
            {
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = (1 - labelPositionRadius) / 2 + (labelPositionRadius / 2);
                        break;
                    case HorizontalAlignment.Left:
                        x = (1 - labelPositionRadius) / 2 - (labelPositionRadius / 2) + 0.015;
                        break;
                    case HorizontalAlignment.Right:
                        x = (1 - labelPositionRadius) / 2 + (labelPositionRadius / 2) + labelPositionRadius - 0.015;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (1 - labelPositionRadius) / 2 + (labelPositionRadius / 2);
                        break;
                }
            }

            ChartAccumulationAdornment ador = new ChartAccumulationAdornment(y, x, point, series);
            ador.MinRadius = 0;
            series.Adornments.Add(ador);
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
        private static void CreateAdornment(ChartPoint point, ChartSeries series, double currY, double height, double topRadius, double bottomRadius, double sumValues)
        {
            double x = 0;
            double y = 0;

            switch (series.AdornmentsInfo.SegmentVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    y = currY + (height * 0.75);
                    break;
                case VerticalAlignment.Center:
                    y = currY + (height * 0.5);
                    break;
                case VerticalAlignment.Top:
                    y = currY + (height * 0.25);
                    break;
                case VerticalAlignment.Stretch:
                    y = currY + (height * 0.5);
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
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
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
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
                        break;
                }
            }
            else
            {
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
                        break;
                    case HorizontalAlignment.Left:
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.25);
                        break;
                    case HorizontalAlignment.Right:
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.75);
                        break;
                    case HorizontalAlignment.Stretch:
                        x = ((1 - bottomRadius) / 2) + (bottomRadius * 0.5);
                        break;
                }
            }

            ChartAccumulationAdornment ador = new ChartAccumulationAdornment(y, x, point, series);
            series.Adornments.Add(ador);
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
