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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Specifies the mode in which the Y values should be interpreted in the Pyramid chart.
    /// </summary>
    public enum ChartPyramidMode
    {
        /// <summary>
        /// The Y values are proportional to the length of the sides of the pyramid.
        /// </summary>
        Linear,

        /// <summary>
        /// The Y values are proportional to the surface area of the corresponding blocks.
        /// </summary>
        Surface
    }
    /// <summary>
    /// Class implementation for ChartAccumulationAdornment
    /// </summary>
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

        #region Properties
        /// <summary>
        /// Identifies the ExplodedOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedOffsetProperty =
            DependencyProperty.Register("ExplodedOffset", typeof(double), typeof(ChartAccumulationAdornment), new PropertyMetadata(15d));

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
        #endregion 

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAccumulationAdornment"/> class.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="point">The point.</param>
        /// <param name="series">The series.</param>
        public ChartAccumulationAdornment(double startAngle, double endAngle, ChartPoint point, ChartSeries series)
            : base(point, series.Data, series, 0)
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
            double gapratio = ChartFunnelType.GetGapRatio(this.series);            
            Rect viewport = ChartPyramidSegment.Inflate(transformer.Viewport, -this.ExplodedOffset, 0);
            double center = viewport.Width / 2;
            double radius = Math.Max(Math.Abs((m_x * viewport.Width) - center), MinRadius);
            double labelheight = 0d;
            double viewportX = viewport.X;
            List<ContentPresenter> val = (from data in this.series.Presenter.Children.OfType<ContentPresenter>() where data.Content is Segment select data).Cast<ContentPresenter>().ToList();
            foreach (ContentPresenter ui in val)
            {
                 ChartAdornment adorn = ui.Content as ChartAdornment;
                 if (adorn != null && adorn.Series.AdornmentsInfo != null)
                 {
                     if (VisualTreeHelper.GetChildrenCount(ui as ContentPresenter) > 0)
                     {
                         Canvas canvas = VisualTreeHelper.GetChild(ui as ContentPresenter, 0) as Canvas;
                         if (canvas != null)
                         {
                             ContentControl ele = VisualTreeHelper.GetChild(canvas, 0) as ContentControl;
                             ele.Measure(new Size(viewport.Width, viewport.Height));
                             Size size = ele.DesiredSize;                             
                             if (series.Type == ChartTypes.Pyramid &&adorn.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center && adorn.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Right && adorn.X != 0)
                             {
                                  labelheight = -(((1 - gapratio) / this.series.sum) * (size.Height/2) * 0.5);
                                  viewportX = 0d;
                             }
                             else if (series.Type == ChartTypes.Funnel && adorn.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center && adorn.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Left && adorn.X != 0)
                             {
                                 labelheight = (((1 - gapratio) / this.series.sum) * (size.Height/2) * 0.5);
                                 viewportX = viewport.X * 2;
                             }
                         }
                     }
                 }             
            }

            if (m_x * viewport.Width < center)
            {
                this.X = viewportX + center - radius;
            }
            else
            {
                this.X = viewportX + center + radius;
            }

            this.Y = (viewport.Y + ((m_y + labelheight) * viewport.Height));
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
    /// Represents Pyramid chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartPyramidType"/>
    public sealed class ChartPyramidSegment : Segment
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
        ///  Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartPyramidSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartPyramidSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsExploded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(ChartPyramidSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the ExplodedOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedOffsetProperty =
            DependencyProperty.Register("ExplodedOffset", typeof(double), typeof(ChartPyramidSegment), new PropertyMetadata(15d));
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
        /// Initializes a new instance of the <see cref="ChartPyramidSegment"/> class.
        /// </summary>
        /// <param name="y">The y value.</param>
        /// <param name="height">The height.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        public ChartPyramidSegment(double y, double height, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartPyramidType), ChartTypes.Pyramid);
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

            m_y = y;
            m_height = height;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Update(IChartTransformer transformer)
        {
            Rect rect = Inflate(transformer.Viewport, -this.ExplodedOffset, 0);
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

                        figure.StartPoint = new Point(rect.X + (topRadius * rect.Width), rect.Y + (top * rect.Height));
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + ((1 - topRadius) * rect.Width), rect.Y + (top * rect.Height)) });
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point=new Point(rect.X + ((1 - bottomRadius) * rect.Width), rect.Y + (bottom * rect.Height) - (Series.StrokeThickness / 2)) });
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rect.X + (bottomRadius * rect.Width), rect.Y + (bottom * rect.Height) - (Series.StrokeThickness / 2)) });
                        figure.IsClosed = true;
                        PathFigureCollection figures=new PathFigureCollection();
                        figures.Add(figure);
                        this.Geometry = new PathGeometry() { Figures=figures };
                    }
                }

                m_viewport = rect;
            }
        }

        /// <summary>
        /// REturn Rect variable from the given values
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect Inflate(Rect rect, double width, double height)
        {
            return new Rect(Math.Abs(rect.X + width), Math.Abs(rect.Y + height), Math.Abs(rect.Width + width - 30), Math.Abs(rect.Height + height));
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
    /// Represents ChartPyramidType class
    /// </summary>
    /// <remarks>
    /// Pyramid chart is similar to the funnel chart. Its often used for geographical
    /// purposes. The Pyramid Chart type displays the data which when totalled will be
    /// 100%. This type of chart is a single series chart representing the data as
    /// portions of 100%, and this chart does not use any axes.
    /// </remarks>
    /// <seealso cref="ChartPyramidSegment"/>
    public sealed class ChartPyramidType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the PyramidMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PyramidModeProperty =
          DependencyProperty.RegisterAttached("PyramidMode", typeof(ChartPyramidMode), typeof(ChartPyramidType), new PropertyMetadata(ChartPyramidMode.Linear, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the GapRatio dependency property.
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty =
          DependencyProperty.RegisterAttached("GapRatio", typeof(double), typeof(ChartPyramidType), new PropertyMetadata(0d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the ExplodedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplodedIndexProperty =
            DependencyProperty.RegisterAttached("ExplodedIndex", typeof(int), typeof(ChartPyramidType), new PropertyMetadata(-1, new PropertyChangedCallback(OnDataChanged)));
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

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            bool needAdornments = series.AdornmentsInfo != null && series.AdornmentsInfo.Visible;
            double[] datapoints = null;
            int explodedIndex = GetExplodedIndex(series);
            int count = points.Count;
            Brush[] brush = series.Area.ColorModel.CurrentPalette;
            double sumValues = 0;
            double gapRatio = GetGapRatio(series);
            ChartPyramidMode pyramidMode = GetPyramidMode(series);
            List<Segment> drawingList = new List<Segment>(count);
            List<Segment> adornmentsList = needAdornments ? new List<Segment>(count) : null;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].EmptyPoint == true && series.ShowEmptyPoints == true)
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
                double currY = 0;
                double gapHieght = gapRatio / (datapoints.Length - 1);

                if (pyramidMode == ChartPyramidMode.Linear)
                {
                    #region Linear Mode
                    double coef = 1d / (sumValues * (1 + (gapRatio / (1 - gapRatio))));

                    for (int i = 0; i < points.Count; i++)
                    {
                        double height = 0;
                        ChartPyramidSegment segment;
                        if (points[i].EmptyPoint == true && series.ShowEmptyPoints == true)
                        {
                            height = coef * Math.Abs(points[i].Values[0]);
                            segment = new ChartPyramidSegment(currY, height, points[i], series);

                            segment.IsExploded = i == explodedIndex;
                            segment.Interior = segment.PaletteInterior = series.EmptyPointInterior ;
                            drawingList.Add(segment);
                            if (needAdornments)
                            {
                                CreateAdornment(points[i], series, adornmentsList, currY, height, sumValues);
                            }
                            currY += gapHieght + height;
                        }
                        else
                        {
                            if (double.IsNaN(points[i].Y))
                                continue;
                           
                            if (points[i].Visible)
                            {
                                height = coef * Math.Abs(points[i].Values[0]);
                                segment = new ChartPyramidSegment(currY, height, points[i], series);
                            }
                            else
                            {
                                segment = new ChartPyramidSegment(currY, 0, points[i], series);
                            }

                            segment.IsExploded = i == explodedIndex;
                            if (brush != null)
                                segment.Interior = segment.PaletteInterior = brush[i % brush.Length];
                            drawingList.Add(segment);
                            if (points[i].Visible)
                            {
                                if (needAdornments)
                                {
                                    CreateAdornment(points[i], series, adornmentsList, currY, height, sumValues);
                                }

                                currY += gapHieght + height;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Surface Mode
                    double[] ys = new double[datapoints.Length];
                    double[] heights = new double[datapoints.Length];
                    double preSum = GetSurfaceHeight(0, sumValues);

                    for (int i = 0; i < points.Count; i++)
                    {
                        if (double.IsNaN(points[i].Y))
                            continue;
                        ys[i] = currY;
                        heights[i] = GetSurfaceHeight(currY, Math.Abs(points[i].Values[0]));
                        if (points[i].Visible)
                        {
                            currY += heights[i] + (gapHieght * preSum);
                        }
                    }

                    double coef = 1 / (currY - (gapHieght * preSum));
                    ChartPyramidSegment segment;
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (points[i].Visible)
                        {
                            segment = new ChartPyramidSegment(coef * ys[i], coef * heights[i], points[i], series);
                        }
                        else
                        {
                            segment = new ChartPyramidSegment(0, 0, points[i], series);
                        }

                        segment.IsExploded = i == explodedIndex;
                        segment.Interior = segment.PaletteInterior = brush[i % brush.Length];
                        drawingList.Add(segment);
                        if (needAdornments && points[i].Visible)
                        {
                            CreateAdornment(points[i], series, adornmentsList, coef * ys[i], coef * heights[i], sumValues);
                        }
                    }
                    #endregion
                }

                if (needAdornments)
                {
                    ////drawingList.AddRange(adornmentsList);
                    foreach (Segment seg in adornmentsList)
                    {
                        series.Adornments.Add(seg);
                    }
                }

                foreach (Segment segment in drawingList)
                {
                    series.Segments.Add(segment);
                }
            }
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
        private static void CreateAdornment(ChartPoint point, ChartSeries series, List<Segment> adornmentsList, double currY, double height, double sumValues)
        {
            double x = 0;
            double y = 0;

            ////double topRadius = (currY) / 2;
            ////double bottomRadius = (currY + height) / 2;
            double topRadius = 0.5d * currY;
            double bottomRadius = 0.5d * (currY + height);
            double midRadius = 0.5d * (currY + height / 2);
            double labelPositionRadius = 0; 
            switch (series.AdornmentsInfo.SegmentVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    y = currY + (height * 0.75);
                    labelPositionRadius = 0.5d * (currY + height * 0.75);
                    break;
                case VerticalAlignment.Center:
                    y = currY + (height * 0.5);
                    labelPositionRadius = midRadius;
                    break;
                case VerticalAlignment.Top:
                    y = currY + (height * 0.25);
                    labelPositionRadius = 0.5d * (currY + height * 0.25);
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
                        break;
                    case HorizontalAlignment.Right:
                        x = 0.5 + topRadius + w;
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
                        break;
                    case HorizontalAlignment.Right:
                        x = 0.5 + topRadius + w + 0.05;
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
            adornmentsList.Add(ador);
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
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

            if (SolveQuadraticEquation(1, 2 * y, -surface, out r1, out r2))
            {
                return Math.Max(r1, r2);
            }

            return double.NaN;
        }

        /// <summary>
        /// Solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a">The A component</param>
        /// <param name="b">The B component</param>
        /// <param name="c">The C component</param>
        /// <param name="root1">First root.</param>
        /// <param name="root2">Second root.</param>
        /// <returns>Bool value</returns>
        public static bool SolveQuadraticEquation(double a, double b, double c, out double root1, out double root2)
        {
            root1 = 0;
            root2 = 0;

            if (a != 0)
            {
                double d = (b * b) - (4 * a * c);

                if (d >= 0)
                {
                    double sd = Math.Sqrt(d);

                    root1 = (-b - sd) / (2 * a);
                    root2 = (-b + sd) / (2 * a);

                    return true;
                }
            }
            else if (b != 0)
            {
                root1 = -c / b;
                root2 = -c / b;

                return true;
            }

            return false;
        }
        #endregion
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
        public override string ToString()
        {
            return "Pyramid";
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
