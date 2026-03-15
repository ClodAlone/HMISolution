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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents point and figure enumeration.
    /// </summary>
    public enum ChartPointAndFigure
    {
        /// <summary>
        /// Determines that point should be drawn.
        /// </summary>
        Point,

        /// <summary>
        /// Determines that figure should be drawn.
        /// </summary>
        Figure
    }

    /// <summary>
    /// Represents Point and Figure chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartPointAndFigureType"/>
    public sealed class ChartPointAndFigureSegment : Segment
    {
        #region Members
        /// <summary>
        /// Declares m_bottomLeftPoint
        /// </summary>
        private ChartPoint m_bottomLeftPoint;

        /// <summary>
        /// Declares m_topRightPoint
        /// </summary>
        private ChartPoint m_topRightPoint;
        #endregion

        #region Dependency properties
        /// <summary>
        ///  Identifies the Template property dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartPointAndFigureSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the AreaPoints depency property.
        /// </summary>
        public static readonly DependencyProperty AreaPointsProperty =
          DependencyProperty.Register("AreaPoints", typeof(Geometry), typeof(ChartPointAndFigureSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Shape dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(ChartPointAndFigure), typeof(ChartPointAndFigureSegment), new PropertyMetadata(ChartPointAndFigure.Figure));

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set template property
        /// </summary>
        public DataTemplate Template
        {
            get
            {
                return (DataTemplate)GetValue(TemplateProperty);
            }

            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Area Points co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The AreaPoints value.</value>
        public Geometry AreaPoints
        {
            get
            {
                return (Geometry)GetValue(AreaPointsProperty);
            }

            set
            {
                SetValue(AreaPointsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X co-ordinate of segment.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of segment.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of segment.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shape.
        /// </summary>
        /// <value>The shape.</value>
        public ChartPointAndFigure Shape
        {
            get { return (ChartPointAndFigure)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointAndFigureSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartPointAndFigureSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPointsCollection correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartPointAndFigureType), ChartTypes.PointAndFigure);
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

            m_bottomLeftPoint = bottomLeftPnt;
            m_topRightPoint = topRightPnt;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            Rect columnRect = new Rect(blPoint, trPoint);

            this.X = columnRect.X;
            this.Y = double.IsNaN(columnRect.Y) ? 0 : columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = double.IsNaN(columnRect.Height) ? 0 : columnRect.Height;
            this.AreaPoints = this.GetGeometry(this.Shape);
        }

        internal Geometry GetGeometry(ChartPointAndFigure shape)
        {
            Geometry geometry = null;
            PathFigure figure = new PathFigure();
            figure.IsClosed = false;
            PathFigure figure1 = new PathFigure();
            figure1.IsClosed = false;
            PathFigureCollection figures = new PathFigureCollection();
            switch (shape)
            {
                case ChartPointAndFigure.Figure:
                    figure.StartPoint = new Point(0, 0);
                    figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(1, 1) });
                    figure1.StartPoint = new Point(1, 0);
                    figure1.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(0, 1) });
                    figures.Add(figure);
                    figures.Add(figure1);
                    geometry = new PathGeometry() { Figures = figures };
                    break;
                case ChartPointAndFigure.Point:
                   geometry = new EllipseGeometry() { RadiusX = 1, RadiusY = 1 };
                   break;
            }

            return geometry;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            m_bottomLeftPoint = null;
            m_topRightPoint = null;
        }
    }

    /// <summary>
    /// Class implementation for ChartPointAndFigure
    /// </summary>
    public class ChartPointAndFigureType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the FigureCost dependency property.
        /// </summary>
        public static readonly DependencyProperty FigureCostProperty =
      DependencyProperty.RegisterAttached("FigureCost", typeof(double), typeof(ChartPointAndFigureType), new PropertyMetadata(1d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the ReversalAmount dependency property.
        /// </summary>
        public static readonly DependencyProperty ReversalAmountProperty =
      DependencyProperty.RegisterAttached("ReversalAmount", typeof(double), typeof(ChartPointAndFigureType), new PropertyMetadata(1d, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the StartFrom dependency property.
        /// </summary>
        public static readonly DependencyProperty StartFromProperty =
      DependencyProperty.RegisterAttached("StartFrom", typeof(ChartPointAndFigure), typeof(ChartPointAndFigureType), new PropertyMetadata(ChartPointAndFigure.Point, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the starting series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The start from value</returns>
        public static ChartPointAndFigure GetStartFrom(ChartSeries series)
        {
            return (ChartPointAndFigure)series.GetValue(StartFromProperty);
        }

        /// <summary>
        /// Sets the starting series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetStartFrom(ChartSeries series, ChartPointAndFigure value)
        {
            series.SetValue(StartFromProperty, value);
        }

        /// <summary>
        /// Gets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns the reversal amount</returns>
        public static double GetReversalAmount(ChartSeries series)
        {
            return (double)series.GetValue(ReversalAmountProperty);
        }

        /// <summary>
        /// Sets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetReversalAmount(ChartSeries series, double value)
        {
            series.SetValue(ReversalAmountProperty, value);
        }

        /// <summary>
        /// Gets the figure cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns the figure cost</returns>
        public static double GetFigureCost(ChartSeries series)
        {
            return (double)series.GetValue(FigureCostProperty);
        }

        /// <summary>
        /// Sets the figure cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetFigureCost(ChartSeries series, double value)
        {
            series.SetValue(FigureCostProperty, value);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 2);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            double figureCost = GetFigureCost(series) <= 0 ? 1 : GetFigureCost(series);
            double reversalAmount = GetReversalAmount(series);
            List<Segment> drawingList = new List<Segment>();
            double currX = 1;
            double currLow = double.NaN;
            double currHigh = double.NaN;
            ChartPointAndFigure currShape = GetStartFrom(series);
            ChartPointsCollection correspondingPoints = new ChartPointsCollection();
            series.minimum = 0;

            for (int i = 0; i < points.Count; i++)
            {
                ChartPoint dataPoint = points[i];
                ////currX = dataPoint.X + (series.XAxis.VisibleRange.Start * (-1));
                double low = Math.Min(dataPoint.Values[0], dataPoint.Values[1]);
                double high = Math.Max(dataPoint.Values[0], dataPoint.Values[1]);

                low = ChartPointAndFigureType.Round(low, figureCost, false);
                high = ChartPointAndFigureType.Round(high, figureCost, true);

                if (i == 0)
                {
                    currLow = low;
                    currHigh = high;
                }
                else if (currShape == ChartPointAndFigure.Point)
                {
                    double minLow = Math.Min(currLow, low);

                    if (high >= currLow + reversalAmount)
                    {
                        drawingList.AddRange(GenerateSegments(currX, minLow, currHigh, correspondingPoints, currShape, series, figureCost));

                        correspondingPoints.Clear();
                        currShape = ChartPointAndFigure.Figure;
                        currLow = minLow + figureCost;
                        currHigh = high;
                        currX++;
                    }
                    else
                    {
                        currLow = minLow;
                    }
                }
                else
                {
                    double maxHigh = Math.Max(currHigh, high);

                    if (low <= currHigh - reversalAmount)
                    {
                        drawingList.AddRange(GenerateSegments(currX, currLow, maxHigh, correspondingPoints, currShape, series, figureCost));

                        correspondingPoints.Clear();
                        currShape = ChartPointAndFigure.Point;
                        currLow = low;
                        currHigh = maxHigh - figureCost;
                        currX++;
                    }
                    else
                    {
                        currHigh = maxHigh;
                    }
                }

                correspondingPoints.Add(points[i]);
            }

            series.maximum = currX;
            if (correspondingPoints.Count != 0)
            {
                drawingList.AddRange(GenerateSegments(currX, currLow, currHigh, correspondingPoints, currShape, series, figureCost));
            }

            drawingList.Reverse();
            foreach (Segment segment in drawingList)
            {
                series.Segments.Add(segment);
            }
        }

        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="div">The divider.</param>
        /// <param name="up">if set to <c>true</c> value will be rounded up.</param>
        /// <returns>The Round off value</returns>
        protected static double Round(double x, double div, bool up)
        {
            return (int)(up ? Math.Ceiling(x / div) : Math.Floor(x / div)) * div;
        }

        /// <summary>
        /// Updates chart.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region implementation
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Generates the segments.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Ending value.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="shape">The shape value.</param>
        /// <param name="series">The series.</param>
        /// <param name="delta">The delta.</param>
        /// <returns>Returns the ChartSegments</returns>
        private static Segment[] GenerateSegments(double x, double from, double to, ChartPointsCollection correspondingPoints, ChartPointAndFigure shape, ChartSeries series, double delta)
        {
            int count = (int)Math.Round((to - from) / delta);
            Segment[] segments = new Segment[count];

            for (int i = 0; i < count; i++)
            {
                ChartPoint pt1 = new ChartPoint(x - 0.5d, from + (i * delta));
                ChartPoint pt2 = new ChartPoint(x + 0.5d, from + ((i + 1) * delta));
                ChartPointAndFigureSegment segment = new ChartPointAndFigureSegment(pt1, pt2, correspondingPoints, series);

                ////segment.AxisLabelInfo = new ChartAxisLabelInfo(x, correspondingPoints[0].DataPoint.X);
                segment.Shape = shape;
                if (shape == ChartPointAndFigure.Point)
                {
                    segment.Interior = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    segment.Interior = new SolidColorBrush(Colors.Green);
                }

                segments[i] = segment;
            }

            return segments;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "PointAndFigure";
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
