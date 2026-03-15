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
using System.Globalization;
using System.Windows.Data;
using Syncfusion.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartscaleBreak
    /// </summary>
    public class ChartScaleBreak : DependencyObject
    {
        #region Dependency Properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for BreakRange.
        /// </summary>
        public static readonly DependencyProperty BreakRangeProperty =
            DependencyProperty.Register("BreakRange", typeof(DoubleRange), typeof(ChartScaleBreak), new PropertyMetadata(new DoubleRange(), new PropertyChangedCallback(OnBreakRangeChanged)));

        /// <summary>
        /// Gets or sets value for BreakRange
        /// </summary>
        public DoubleRange BreakRange
        {
            get { return (DoubleRange)GetValue(BreakRangeProperty); }
            set { SetValue(BreakRangeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LineType.
        /// </summary>
        public static readonly DependencyProperty LineTypeProperty =
            DependencyProperty.Register("LineType", typeof(ScaleBreakLineTypes), typeof(ChartScaleBreak), new PropertyMetadata(ScaleBreakLineTypes.Wave, new PropertyChangedCallback(OnLineTypeChanged)));

        /// <summary>
        /// Gets or sets the value for LineType dependency property.
        /// </summary>
        public ScaleBreakLineTypes LineType
        {
            get { return (ScaleBreakLineTypes)GetValue(LineTypeProperty); }
            set { SetValue(LineTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LineColor.
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(ChartScaleBreak), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLineColorChanged)));

        /// <summary>
        /// Gets or sets the value for LineColor dependency property.
        /// </summary>
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LineThickness.
        /// </summary>
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(ChartScaleBreak), new PropertyMetadata(1d, new PropertyChangedCallback(OnLineThicknessChanged)));

        /// <summary>
        /// Gets or sets the value for LineThickness dependency property.
        /// </summary>
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SpaceColor.
        /// </summary>
        public static readonly DependencyProperty SpaceColorProperty =
            DependencyProperty.Register("SpaceColor", typeof(Brush), typeof(ChartScaleBreak), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnSpaceColorChanged)));

        /// <summary>
        /// Gets the value for SpaceColor dependency property.
        /// </summary>
        public Brush SpaceColor
        {
            get { return (Brush)GetValue(SpaceColorProperty); }
            set { SetValue(SpaceColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SpaceWidth.
        /// </summary>
        public static readonly DependencyProperty SpaceWidthProperty =
            DependencyProperty.Register("SpaceWidth", typeof(double), typeof(ChartScaleBreak), new PropertyMetadata(2d, new PropertyChangedCallback(OnSpaceWidthChanged)));

        /// <summary>
        /// Gets or sets the value for SpaceWidth dependency property.
        /// </summary>
        public double SpaceWidth
        {
            get { return (double)GetValue(SpaceWidthProperty); }
            set { SetValue(SpaceWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeDashArray.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(double[]), typeof(ChartScaleBreak), new PropertyMetadata(new double[]{}, new PropertyChangedCallback(OnStrokeDashArrayChanged)));

        /// <summary>
        /// Gets or sets the value for StrokeDashArray dependency property.
        /// </summary>
        public double[] StrokeDashArray
        {
            get { return (double[])GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        #endregion

        #region Members
        internal DoubleRange m_breakRange = new DoubleRange(); 
        #endregion

        #region PropertyChangedCallBack Methods

        /// <summary>
        /// Executes when BreakRange changed
        /// </summary>
        private static void OnBreakRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scaleBreak = d as ChartScaleBreak;
            scaleBreak.m_breakRange = (DoubleRange)e.NewValue;

            if (scaleBreak != null && scaleBreak.m_axis != null && scaleBreak.m_axis.ManualBreaks != null && scaleBreak.m_axis.Area != null)
            {
                if (scaleBreak.m_axis.BreaksMode == ScaleBreaksModes.Manual)
                {
                    scaleBreak.m_axis.m_manualBreakRanges.Clear();
                    scaleBreak.m_axis.Area.Breaks.Clear();
                    foreach (ChartScaleBreak brk in scaleBreak.m_axis.ManualBreaks)
                    {
                        scaleBreak.m_axis.m_manualBreakRanges.Add(brk.m_breakRange);
                        scaleBreak.m_axis.Area.Breaks.Add(brk);
                    }
                    scaleBreak.m_axis.CalculateSumBreaks();
                    scaleBreak.scaleBreakPanel.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Executes when LineType changed
        /// </summary>
        private static void OnLineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Executes when LineColor changed
        /// </summary>
        private static void OnLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Executes when LineThickness changed
        /// </summary>
        private static void OnLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak.m_axis != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Executes when SpaceColor changed
        /// </summary>
        private static void OnSpaceColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak.m_axis != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Executes when SpaceWidth changed
        /// </summary>
        private static void OnSpaceWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak.m_axis != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Executes when StrokeDashArray changed
        /// </summary>
        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartScaleBreak scalebreak = d as ChartScaleBreak;
            if (scalebreak.m_axis != null && scalebreak.scaleBreakPanel != null)
            {
                scalebreak.scaleBreakPanel.InvalidateMeasure();
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Members
        
        internal ChartAxis m_axis = null;
        internal ChartScaleBreakPanel scaleBreakPanel = null;
        private double m_breakSegmentsLength = 0;
        private List<double> m_values = new List<double>();
        private List<DoubleRange> m_yAxisSegments = new List<DoubleRange>();
        internal List<DoubleRange> m_breakSegments = new List<DoubleRange>();
        internal List<ChartScaleBreakSegment> m_segments = new List<ChartScaleBreakSegment>();

        #endregion

        #region Implementation

        /// <summary>
        /// Unions the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="axis"></param>
        private void Union(DoubleRange range, ChartAxis axis)
        {
            bool isNeedAdd = true;

            if (this.m_axis != null && this.m_axis.m_enableBreaks && this.m_axis.BreaksMode == ScaleBreaksModes.Manual)
            {
                this.m_axis.m_manualBreakRanges.Clear();
                for (int i = 0; i < axis.m_manualBreakRanges.Count; )
                {
                    DoubleRange current = axis.m_manualBreakRanges[i];

                    if (range.Inside(current))
                    {
                        axis.m_manualBreakRanges.RemoveAt(i);
                    }
                    else if (current.Inside(range))
                    {
                        isNeedAdd = false;
                        break;
                    }
                    else if (current.Intersects(range))
                    {
                        range += current;
                        axis.m_manualBreakRanges.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            if (isNeedAdd)
            {
                axis.m_manualBreakRanges.Add(range);
            }
        }

        /// <summary>
        /// To compute the auto break segments
        /// </summary>
        /// <param name="series"></param>
        internal void Compute(SeriesCollection series)
        {
            bool supported = true;
            m_values = new List<double>();
            m_yAxisSegments = new List<DoubleRange>();
            m_breakSegments = new List<DoubleRange>();

            #region Compute values
            foreach (ChartSeries ser in series)
            {
                if (m_axis == ser.YAxis && ser.Data != null && ser.Data.Count > 0)
                {
                    if (ser.Type == ChartTypes.StackingArea || ser.Type == ChartTypes.StackingBar || ser.Type == ChartTypes.StackingBar100 ||
                        ser.Type == ChartTypes.StackingColumn || ser.Type == ChartTypes.StackingColumn100)
                    {
                        supported = false;
                    }

                    for (int i = 0; i < ser.Data.Count; i++)
                    {
                        for (int j = 0; j < ser.Data[i].Values.Length; j++)
                        {
                            if (!m_values.Contains(ser.Data[i].Values[j]))
                                m_values.Add(ser.Data[i].Values[j]);
                        }
                    }

                    if (ser.Type == ChartTypes.Area || ser.Type == ChartTypes.Bar || ser.Type == ChartTypes.Column ||
                        ser.Type == ChartTypes.SplineArea || ser.Type == ChartTypes.StepArea || ser.Type == ChartTypes.Histogram)
                    {
                        m_values.Add(m_axis.Origin != 0 ? m_axis.Origin : 0);
                    }
                }
            }

            m_values.Sort();

            if (m_values.Count > 0)
            {
                double iStart = m_axis.VisibleRange.Start;
                double iEnd = m_axis.VisibleRange.Start;
                while (iEnd < m_axis.VisibleRange.End)
                {
                    iEnd += m_axis.VisibleInterval;
                    m_yAxisSegments.Add(new DoubleRange(iStart, iEnd));
                    iStart = iEnd;
                }
            }

            for (int i = 0; i < m_values.Count; i++)
            {
                bool isIntervalValue = false;
                foreach (DoubleRange range in m_yAxisSegments)
                {
                    if (range.Inside((double)m_values[i]))
                    {
                        if (m_breakSegments.Count == 0)
                        {
                            m_breakSegments.Add(range);
                            if ((double)m_values[i] % m_axis.VisibleInterval == 0)
                                continue;
                            else
                                break;
                        }
                        else if (!m_breakSegments.Contains(range))
                        {
                            m_breakSegments.Add(range);
                            if ((double)m_values[i] % m_axis.VisibleInterval == 0 && !isIntervalValue)
                            {
                                isIntervalValue = true;
                                continue;
                            }
                            else
                                break;
                        }
                        else if (m_breakSegments.Contains(range) && (double)m_values[i] % m_axis.VisibleInterval == 0)
                        {
                            isIntervalValue = true;
                            continue;
                        }
                        else
                            break;
                    }
                }
            }
            #endregion

            if (supported && m_values.Count > 0)
            {
                #region Compute segments
                m_segments.Clear();

                DoubleRange baseRange = new DoubleRange(m_axis.VisibleRange.Start, m_axis.VisibleRange.End);

                double start = (double)m_values[0];
                double last = double.NaN;
                int index = -1;

                foreach (double value in m_values)
                {
                    index++;
                    if (!double.IsNaN(last) && m_axis.m_autoBreakThresholdCoefficient > 0)
                    {
                        if (value - last > m_axis.m_autoBreakThresholdCoefficient * baseRange.Delta)
                        {
                            ChartScaleBreakSegment segment = new ChartScaleBreakSegment();

                            segment.Range = new DoubleRange(start, last);

                            double _start = segment.Range.Start - (m_axis.VisibleInterval / 2);
                            double _end = segment.Range.End + (m_axis.VisibleInterval / 2);

                            if (_start >= m_axis.VisibleRange.Start && _end <= m_axis.VisibleRange.End)
                            {
                                segment.Range = new DoubleRange(_start, _end);
                            }
                            m_segments.Add(segment);

                            start = value;
                        }
                    }

                    last = value;
                }

                if (m_segments.Count == 0)
                {
                    m_axis.m_enableBreaks = false;
                }
                else
                {
                    ChartScaleBreakSegment lastSegment = m_segments[m_segments.Count - 1] as ChartScaleBreakSegment;
                    if (lastSegment.Range.End < m_axis.VisibleRange.End)
                    {
                        ChartScaleBreakSegment axisSegment = new ChartScaleBreakSegment();
                        axisSegment.Range = new DoubleRange(start, m_axis.VisibleRange.End);
                        axisSegment.Range = new DoubleRange(axisSegment.Range.Start - (m_axis.VisibleInterval / 2), m_axis.VisibleRange.End);
                        m_segments.Add(axisSegment);
                    }
                }

                for (int i = 0, ci = m_segments.Count - 1; i < ci; i++)
                {
                    ChartScaleBreakSegment axisSegment1 = m_segments[i] as ChartScaleBreakSegment;
                    ChartScaleBreakSegment axisSegment2 = m_segments[i + 1] as ChartScaleBreakSegment;

                    DoubleRange brkRange = new DoubleRange(axisSegment1.Range.End, axisSegment2.Range.Start);
                    bool isIntervalValue = false;
                    if (brkRange.Delta > 0)
                    {
                        foreach (DoubleRange range in m_yAxisSegments)
                        {
                            if (range.Inside(brkRange.End))
                            {
                                if (!m_breakSegments.Contains(range))
                                {
                                    m_breakSegments.Add(range);
                                    if (brkRange.End % m_axis.VisibleInterval == 0 && !isIntervalValue)
                                    {
                                        isIntervalValue = true;
                                        continue;
                                    }
                                    else
                                        break;
                                }
                                else if (m_breakSegments.Contains(range) && brkRange.End % m_axis.VisibleInterval == 0)
                                {
                                    isIntervalValue = true;
                                    continue;
                                }
                                else
                                    break;
                            }
                        }
                    }
                }

                #endregion

                Comparer comparer = new Comparer(new CultureInfo("en-US"));
                for (int i = 0; i < m_breakSegments.Count - 1; i++)
                {
                    for (int j = i; j <= m_breakSegments.Count - 1; j++)
                    {
                        var compare = comparer.Compare(m_breakSegments[i].Start, m_breakSegments[j].Start);
                        if (compare > 0)
                        {
                            var temp = m_breakSegments[i];
                            m_breakSegments[i] = m_breakSegments[j];
                            m_breakSegments[j] = temp;
                        }
                    }
                }

                m_breakSegmentsLength = 1d / m_breakSegments.Count;
            }

        }

        /// <summary>
        /// Computes coefficient by the specified value for the automatic mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double AutoValueToCoefficient(double value)
        {
            double result = 0;

            if (m_segments != null && m_segments.Count > 0 && m_axis.BreaksMode == ScaleBreaksModes.Auto)
            {
                for (int i = 0; i < m_breakSegments.Count; i++)
                {
                    if (m_breakSegments[i].Inside(value))
                    {
                        result += m_breakSegmentsLength * ((value - m_breakSegments[i].Start) / (m_breakSegments[i].End - m_breakSegments[i].Start));
                        break;
                    }
                    else
                    {
                        result += m_breakSegmentsLength;
                    }
                }
            }
            else if (m_axis != null)
            {
                result = (value - m_axis.VisibleRange.Start) / m_axis.VisibleRange.Delta;
            }

            return result;
        }

        /// <summary>
        /// Computes value by the specified coefficient for the automatic mode.
        /// </summary>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns></returns>
        public double AutoCoefficientToValue(double coefficient)
        {
            double result = 0;
            double length = m_breakSegmentsLength;
            if (m_segments != null && m_segments.Count > 0 && m_axis.BreaksMode == ScaleBreaksModes.Auto)
            {
                for (int i = 0; i < m_breakSegments.Count; i++)
                {
                    DoubleRange segment = m_breakSegments[i];
                    if (coefficient == 0)
                    {
                        result = m_breakSegments[0].Start;
                        break;
                    }
                    if (coefficient < length)
                    {
                        result = segment.Start + (segment.Delta * ((length - coefficient) / m_breakSegmentsLength));
                        break;
                    }
                    else if (coefficient == Math.Round(length, 1))
                    {
                        result = segment.End;
                        break;
                    }
                    else
                    {
                        length += m_breakSegmentsLength;
                    }
                }
            }
            else if (this.m_axis != null)
            {
                result = m_axis.VisibleRange.Start + m_axis.VisibleRange.Delta * coefficient;
            }

            return result;
        }

        /// <summary>
        /// Draws the break line.
        /// </summary>
        /// <param name="actualSize"></param>
        /// <param name="from">The start point of break line.</param>
        /// <param name="to">The end point of break line.</param>
        /// <param name="scaleBreak"></param>
        internal Path DrawBreakLine(ChartScaleBreak scaleBreak, Size actualSize, Point from, Point to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double d = (double)Math.Sqrt(dx * dx + dy * dy);
            double offset = this.SpaceWidth;

            double offsetX = offset * dy / d;
            double offsetY = offset * dx / d;

            Point[] points = null;
            PathGeometry pathGeo = new PathGeometry();
            PathFigure figure = new PathFigure();

            //ContentPresenter scaleBrk = scaleBreak as ContentPresenter;
            Path path= new Path();
            path.Stroke = this.LineColor;
            path.StrokeThickness = this.LineThickness;
            path.Fill = this.SpaceColor;
            DoubleCollection dc = new DoubleCollection();
            foreach (double var in this.StrokeDashArray)
                dc.Add(var);
            path.StrokeDashArray = dc;

            switch (this.LineType)
            {
                case ScaleBreakLineTypes.StraightLine:
                    points = new Point[]{ from, new Point(from.X + 0.25f * dx, from.Y + 0.25f * dy),
                             new Point(from.X + 0.75f * dx, from.Y + 0.75f * dy),to};
                    break;

                case ScaleBreakLineTypes.Wave:
                    points = GetWaveBeziersPoints(from, to, 20, 5);
                    break;

                case ScaleBreakLineTypes.Randomize:
                    points = GetRandomBeziersPoints(from, to, 10, 3);
                    break;
            }

            if (this.SpaceWidth > 0 && points.Length > 0)
            {
                Point[] nearPoints = new Point[points.Length];
                Point[] farPoints = new Point[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    nearPoints[i] = new Point(points[i].X - offsetX, points[i].Y - offsetY);
                    farPoints[points.Length - i - 1] = new Point(points[i].X + offsetX, points[i].Y + offsetY);
                }

                figure.StartPoint = nearPoints[0];

                if (this.LineType == ScaleBreakLineTypes.StraightLine)
                {
                    for (int i = 1; i < nearPoints.Length; i++ )
                    {
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = nearPoints[i] });
                    }
                    for (int i = 0; i < farPoints.Length; i++ )
                    {
                        figure.Segments.Add(new System.Windows.Media.LineSegment() { Point = farPoints[i] });
                    }
                }
                else
                {
                    Point point1, point2, point3;

                    for (int i = 1; i < nearPoints.Length; i = i + 3)
                    {
                        point1 = new Point(nearPoints[i].X, nearPoints[i].Y);
                        point2 = new Point(nearPoints[i + 1].X, nearPoints[i + 1].Y);
                        point3 = new Point(nearPoints[i + 2].X, nearPoints[i + 2].Y);
                        figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                    }
                    figure.Segments.Add(new BezierSegment() { Point1 = farPoints[0], Point2 = farPoints[0], Point3 = farPoints[0] });
                    for (int i = 1; i < nearPoints.Length; i = i + 3)
                    {
                        point1 = new Point(farPoints[i].X, farPoints[i].Y);
                        point2 = new Point(farPoints[i + 1].X, farPoints[i + 1].Y);
                        point3 = new Point(farPoints[i + 2].X, farPoints[i + 2].Y);
                        figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                    }
                }               
            }
            else
            {
                figure.StartPoint = points[0];

                Point point1, point2, point3;
                for (int k = 1; k < points.Length; k = k + 3)
                {
                    point1 = new Point(points[k].X, points[k].Y);
                    point2 = new Point(points[k + 1].X, points[k + 1].Y);
                    point3 = new Point(points[k + 2].X, points[k + 2].Y);
                    figure.Segments.Add(new BezierSegment() { Point1 = point1, Point2 = point2, Point3 = point3 });
                }
            }
            pathGeo.Figures.Add(figure);
            path.Data = pathGeo;
            return path;
        }

        /// <summary>
        /// Gets the wave beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        private static Point[] GetWaveBeziersPoints(Point pt1, Point pt2, int count, float fault)
        {
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            double length = (double)Math.Sqrt(dx * dx + dy * dy);
            double nx = fault * dy / length;
            double ny = fault * dx / length;
            double sx = dx / count;
            double sy = dy / count;

            Point[] points = new Point[3 * count + 1];

            for (int i = 0; i < count; i++)
            {
                points[3 * i] = new Point(pt1.X + sx * i, pt1.Y + sy * i);
                points[3 * i + 1] = new Point(pt1.X + sx * (i + 0.5f) + nx, pt1.Y + sy * (i + 0.5f) + ny);
                points[3 * i + 2] = new Point(pt1.X + sx * (i + 0.5f) - nx, pt1.Y + sy * (i + 0.5f) - ny);
            }

            points[points.Length - 1] = pt2;
            return points;
        }

        /// <summary>
        /// Gets the random beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        private static Point[] GetRandomBeziersPoints(Point pt1, Point pt2, int count, float fault)
        {
            Random rand = new Random(10);

            int sectors = 3 * count + 1;
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            double length = (double)Math.Sqrt(dx * dx + dy * dy);
            double nx = dy / length;
            double ny = dx / length;
            double sx = dx / (sectors - 1);
            double sy = dy / (sectors - 1);

            Point[] points = new Point[sectors];

            points[0] = pt1;
            points[sectors - 1] = pt2;

            for (int i = 1; i < sectors - 1; i++)
            {
                double f = (double)(2 * fault * rand.NextDouble() - fault);
                points[i] = new Point(pt1.X + i * sx + f * nx, pt1.Y + i * sy + f * ny);
            }

            return points;
        }
       
        #endregion
    }

    /// <summary>
    /// Specifies the single range segment of axis.
    /// </summary>
    public class ChartScaleBreakSegment
    {
        #region Members
        private DoubleRange m_range;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public DoubleRange Range
        {
            get { return m_range; }
            set { m_range = value; }
        }
        #endregion
    }
}
