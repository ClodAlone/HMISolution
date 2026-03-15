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
    /// Class implementation for ChartRenkosegment
    /// </summary>
    public sealed class ChartRenkoSegment : Segment
    {
        #region Members
        /// <summary>
        /// Initializes m_transformedRectangles
        /// </summary>
        private List<Rect> m_transformedRectangles = new List<Rect>();

        /// <summary>
        /// Holds represented rectangles array that are used to build the segment.
        /// </summary>
        private Rect[] m_representedRectangles;

        /// <summary>
        /// Gets or sets a value indicating whether this segment is price up. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price up; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceUp
        {
            get { return (bool)GetValue(IsPriceUpProperty); }
            set { SetValue(IsPriceUpProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment is price down. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price down; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceDown
        {
            get { return (bool)GetValue(IsPriceDownProperty); }
            set { SetValue(IsPriceDownProperty, value); }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        ///  Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
        DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartRenkoSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty =
            DependencyProperty.Register("IsPriceUp", typeof(bool), typeof(ChartRenkoSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty =
            DependencyProperty.Register("IsPriceDown", typeof(bool), typeof(ChartRenkoSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartRenkoSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
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
        /// Get or Set Template Property
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRenkoSegment"/> class.
        /// </summary>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <param name="representedRects">The represented rects.</param>
        internal ChartRenkoSegment(ChartPoint correspondingPoint, ChartSeries series, Rect[] representedRects)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartRenkoType), ChartTypes.Renko);
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

            this.m_representedRectangles = representedRects;
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            bool shouldReassignGeometry = false;
            PathFigure figure;
            PathFigureCollection figures = new PathFigureCollection();
            for (int i = 0; i < m_representedRectangles.Length; i++)
            {
                ////Retrieving real coordinates of renko rectangle in chart area presenter.
                Point leftTopPoint = transformer.TransformToVisible(m_representedRectangles[i].Left + (series.XAxis.VisibleRange.Start * (-1)), m_representedRectangles[i].Top + (series.YAxis.VisibleRange.Start * (-1)), series);
                Point rightBottomPoint = transformer.TransformToVisible(m_representedRectangles[i].Right + (series.XAxis.VisibleRange.Start * (-1)), m_representedRectangles[i].Bottom + (series.YAxis.VisibleRange.Start * (-1)), series);
                ////Renko rectangle in real coordinates.
                Rect rectangle = new Rect(leftTopPoint, rightBottomPoint);
                if (!m_transformedRectangles.Contains(rectangle))
                {
                    ////m_transformedRectangles.RemoveAt(i);
                    if (m_transformedRectangles.Count - 1 >= i)
                    {
                        m_transformedRectangles[i] = rectangle; ////.Add(rectangle);
                    }
                    else
                    {
                        m_transformedRectangles.Add(rectangle);
                    }

                    shouldReassignGeometry = true;
                }
                ////Building rectangle figure.
                figure = new PathFigure();
                figure.StartPoint = new Point(rectangle.Left, rectangle.Top);
                figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rectangle.Right, rectangle.Top) });
                figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rectangle.Right, rectangle.Bottom) });
                figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new Point(rectangle.Left, rectangle.Bottom) });
                figure.IsClosed = true;
                figures.Add(figure);
            }

            if (shouldReassignGeometry)
            {
                this.Geometry = new PathGeometry() { Figures = figures };
                ////m_transformedRectangles.Clear();
            }

            if (m_transformedRectangles.Count > m_representedRectangles.Length)
            {
                throw null;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.m_representedRectangles != null)
                this.m_representedRectangles = null;

            if (this.m_transformedRectangles != null)
            {
                this.m_transformedRectangles.Clear();
                this.m_transformedRectangles = null;
            }
        }
    }

    /// <summary>
    /// Get or Set ChartRenKoType
    /// </summary>
    public class ChartRenkoType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the RenkoCost dependency property.
        /// </summary>
        public static readonly DependencyProperty RenkoCostProperty =
          DependencyProperty.RegisterAttached("RenkoCost", typeof(double), typeof(ChartRenkoType), new PropertyMetadata(1d, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the renko cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Renko Cost</returns>
        public static double GetRenkoCost(ChartSeries series)
        {
            return (double)series.GetValue(RenkoCostProperty);
        }

        /// <summary>
        /// Sets the renko cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetRenkoCost(ChartSeries series, double value)
        {
            series.SetValue(RenkoCostProperty, value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Renko";
        }
        #endregion

        #region implementation

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            double renkoCost = ChartRenkoType.GetRenkoCost(series) <= 0 ? 1 : ChartRenkoType.GetRenkoCost(series);
            double currentX = points[0].X;
            double dataPointsRange = points[points.Count - 1].X - points[0].X;
            double previousLow = points[0].Y;
            double previousHigh = points[0].Y;
            Rect[] representedRectangles;
            if (series.AdornmentsInfo != null)
            {
                if (series.AdornmentsInfo.Visible == true)
                {
                    series.Adornments.Add(new ChartAdornment(points[0], new ChartPoint(points[0].X, points[0].Y), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                }
            }   
            for (int i = 1; i <= points.Count - 1; i++)
            {
                if (points[i].EmptyPoint && series.ShowEmptyPoints)
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
                if (double.IsNaN(points[i].Y))
                    continue;
                ChartPoint point = points[i];

                double deltaYMin = point.Y - previousLow;
                double deltaYMax = point.Y - previousHigh;
                double deltaX;
                int renkoCount;
                if (deltaYMin <= -renkoCost)
                {
                    renkoCount = (int)Math.Floor(Math.Abs(deltaYMin / renkoCost));
                    deltaX = (point.X - currentX) / renkoCount;
                    representedRectangles = new Rect[renkoCount];
                    for (int ik = 0; ik < renkoCount; ik++, currentX += deltaX)
                    {
                        Point topLeftPoint = new Point(currentX, previousLow - (ik * renkoCost));
                        Point bottomRightPoint = new Point((currentX + deltaX), previousLow - ((ik + 1) * renkoCost));
                        representedRectangles[ik] = new Rect(topLeftPoint, bottomRightPoint);
                    }

                    ChartRenkoSegment renkoSegment = new ChartRenkoSegment(points[i - 1], series, representedRectangles);
                    renkoSegment.IsPriceDown = true;
                    renkoSegment.Stroke = new SolidColorBrush(Colors.Red);
                    series.Segments.Add(renkoSegment);
                    if (series.AdornmentsInfo != null)
                    {
                        if (series.AdornmentsInfo.Visible == true)
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Y), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                        }
                    }
                    previousLow -= renkoCount * renkoCost;
                    previousHigh = previousLow + renkoCost;
                }

                if (deltaYMax >= renkoCost)
                {
                    renkoCount = (int)Math.Floor(Math.Abs(deltaYMin / renkoCost));
                    deltaX = (point.X - currentX) / renkoCount;
                    representedRectangles = new Rect[renkoCount];
                    for (int ik = 0; ik < renkoCount; ik++, currentX += deltaX)
                    {
                        Point ltPoint = new Point(currentX, previousHigh + (ik * renkoCost));
                        Point rbPoint = new Point((currentX + deltaX), previousHigh + ((ik + 1) * renkoCost));
                        representedRectangles[ik] = new Rect(ltPoint, rbPoint);
                    }

                    ChartRenkoSegment renkoSegment = new ChartRenkoSegment(points[i - 1], series, representedRectangles);
                    renkoSegment.Stroke = new SolidColorBrush(Colors.Green);
                    ////renkoSegment.SetValue(ChartRenkoSegment.IsPriceUpPropertyKey, true);
                    renkoSegment.IsPriceUp = true;
                    series.Segments.Add(renkoSegment);
                    if (series.AdornmentsInfo != null)
                    {
                        if (series.AdornmentsInfo.Visible == true)
                        {                           
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Y), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                        }
                    }
                    previousHigh += renkoCount * renkoCost;
                    previousLow = previousHigh - renkoCost;
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
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            this.CalculateSegments(series, points);
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
