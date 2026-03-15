#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies types of break line.
    /// </summary>
    public enum ChartBreakLineType
    {
        /// <summary>
        /// The straight line.
        /// </summary>
        Straight,
        /// <summary>
        /// The wave line.
        /// </summary>
        Wave,
        /// <summary>
        /// The randomize line.
        /// </summary>
        Randomize
    }

    /// <summary>
    /// Specifies modes of breaks.
    /// </summary>
    public enum ChartBreaksMode
    {
        /// <summary>
        /// Breaks isn't used.
        /// </summary>
        None,
        /// <summary>
        /// Chart automatically calculate the breaks.
        /// </summary>
        Auto,
        /// <summary>
        /// Breaks is set manually.
        /// </summary>
        Manual
    }

    /// <summary>
    /// Contains the appearance properties of break lines.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ChartAxisBreakInfo
    {
        #region Members
        private ChartBreakLineType m_lineType = ChartBreakLineType.Wave;
        private Color m_lineColor = Color.Black;
        private Color m_spacingColor = Color.White;
        private DashStyle m_lineStyle = DashStyle.Solid;
        private float m_lineWidth = 1f;
        private double m_lineSpacing = 1f;
        #endregion //Members

        #region Events
        /// <summary>
        /// Occurs when appearance was changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion //Events

        #region Properties
        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>The type of the line.</value>
        [DefaultValue(ChartBreakLineType.Wave)]
        [Description("Gets or sets the type of the line.")]
        public ChartBreakLineType LineType
        {
            get { return m_lineType; }
            set
            {
                if (m_lineType != value)
                {
                    m_lineType = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        [DefaultValue(typeof(Color), "Black")]
        [Description("Gets or sets the color of the line.")]
        public Color LineColor
        {
            get { return m_lineColor; }
            set
            {
                if (m_lineColor != value)
                {
                    m_lineColor = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <value>The line style.</value>
        [DefaultValue(DashStyle.Solid)]
        [Description("Gets or sets the line style.")]
        public DashStyle LineStyle
        {
            get { return m_lineStyle; }
            set
            {
                if (m_lineStyle != value)
                {
                    m_lineStyle = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        /// <value>The width of the line.</value>
        [DefaultValue(1f)]
        [Description("Gets or sets the width of the line.")]
        public float LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                if (m_lineWidth != value)
                {
                    m_lineWidth = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the line spacing .
        /// </summary>
        /// <value>The line spacing .</value>
        [DefaultValue(1d)]
        [Description("Gets or sets the line spacing .")]
        public double LineSpacing
        {
            get { return m_lineSpacing; }
            set
            {
                value = ChartMath.MinMax(value, 0, 10);

                if (m_lineSpacing != value)
                {
                    m_lineSpacing = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the spacing.
        /// </summary>
        /// <value>The color of the spacing.</value>
        [DefaultValue(typeof(Color), "White")]
        [Description("Gets or sets the color of the spacing.")]
        public Color SpacingColor
        {
            get { return m_spacingColor; }
            set
            {
                if (m_spacingColor != value)
                {
                    m_spacingColor = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        #endregion //Properties

        #region Implementation
        /// <summary>
        /// Draws the break line.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="from">The start point of break line.</param>
        /// <param name="to">The end point of break line.</param>
        internal void DrawBreakLine(Graphics g, PointF from, PointF to)
        {
            float dx = to.X - from.X;
            float dy = to.Y - from.Y;
            float d = (float)Math.Sqrt(dx * dx + dy * dy);
            float offset = (float)m_lineSpacing / 2;

            float offsetX = offset * dy / d;
            float offsetY = offset * dx / d;

            PointF[] points = null;

            switch (m_lineType)
            {
                case ChartBreakLineType.Straight:
                    points = new PointF[]{ from, new PointF(from.X + 0.25f * dx, from.Y + 0.25f * dy),
						new PointF(from.X + 0.75f * dx, from.Y + 0.75f * dy),to};
                    break;

                case ChartBreakLineType.Wave:
                    points = RenderingHelper.GetWaveBeziersPoints(from, to, 10, 3);
                    break;

                case ChartBreakLineType.Randomize:
                    {
                        points = RenderingHelper.GetRendomBeziersPoints(from, to, 10, 3);
                    }
                    break;
            }

            if (m_lineSpacing > 0)
            {
                GraphicsPath gp = new GraphicsPath();
                PointF[] nearPoints = new PointF[points.Length];
                PointF[] farPoints = new PointF[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    nearPoints[i] = new PointF(points[i].X - offsetX, points[i].Y - offsetY);
                    farPoints[points.Length - i - 1] = new PointF(points[i].X + offsetX, points[i].Y + offsetY);
                }

                gp.AddBeziers(nearPoints);
                gp.AddBeziers(farPoints);

                using (SolidBrush sb = new SolidBrush(m_spacingColor))
                {
                    g.FillPath(sb, gp);
                }

                using (Pen pen = this.GetPen())
                {
                    g.DrawBeziers(pen, nearPoints);
                    g.DrawBeziers(pen, farPoints);
                }
            }
            else
            {
                using (Pen pen = this.GetPen())
                {
                    g.DrawBeziers(pen, points);
                }
            }
        }
        /// <summary>
        /// Draws the break line.
        /// </summary>
        /// <param name="g3d">The <see cref="Graphics3D"/>.</param>
        /// <param name="from">The start point of break line.</param>
        /// <param name="to">The end point of break line.</param>
        internal void DrawBreakLine(Graphics3D g3d, PointF from, PointF to)
        {
            float dx = to.X - from.X;
            float dy = to.Y - from.Y;
            float d = (float)Math.Sqrt(dx * dx + dy * dy);
            float offset = (float)m_lineSpacing / 2;

            float offsetX = offset * dy / d;
            float offsetY = offset * dx / d;

            PointF[] points = null;

            switch (m_lineType)
            {
                case ChartBreakLineType.Straight:
                    points = new PointF[]{ from, new PointF(from.X + 0.25f * dx, from.Y + 0.25f * dy),
						new PointF(from.X + 0.75f * dx, from.Y + 0.75f * dy),to};
                    break;

                case ChartBreakLineType.Wave:
                    points = RenderingHelper.GetWaveBeziersPoints(from, to, 10, 3);
                    break;

                case ChartBreakLineType.Randomize:
                    {
                        points = RenderingHelper.GetRendomBeziersPoints(from, to, 10, 3);
                    }
                    break;
            }

            Plane3D pahtPlane = new Plane3D(new Vector3D(0, 0, 1), 0);

            Pen pen = this.GetPen();

            if (m_lineSpacing > 0)
            {
                GraphicsPath gp = new GraphicsPath();
                GraphicsPath nGp = new GraphicsPath();
                GraphicsPath fGp = new GraphicsPath();

                PointF[] nearPoints = new PointF[points.Length];
                PointF[] farPoints = new PointF[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    nearPoints[i] = new PointF(points[i].X - offsetX, points[i].Y - offsetY);
                    farPoints[points.Length - i - 1] = new PointF(points[i].X + offsetX, points[i].Y + offsetY);
                }

                gp.AddBeziers(nearPoints);
                gp.AddBeziers(farPoints);
                nGp.AddBeziers(nearPoints);
                fGp.AddBeziers(farPoints);

                Path3D spacing3D = Path3D.FromGraphicsPath(gp, pahtPlane, 0, new SolidBrush(m_spacingColor), null);
                Path3D near3D = Path3D.FromGraphicsPath(nGp, pahtPlane, 0, null, pen);
                Path3D far3D = Path3D.FromGraphicsPath(fGp, pahtPlane, 0, null, pen);

                g3d.AddPolygon(new Path3DCollect(new Polygon[] { spacing3D, near3D, far3D }));
            }
            else
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddBeziers(points);
                g3d.AddPolygon(Path3D.FromGraphicsPath(gp, pahtPlane, 0, null, pen));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Pen GetPen()
        {
            Pen pen = new Pen(m_lineColor, m_lineWidth);
            pen.DashStyle = m_lineStyle;
            return pen;
        }
        /// <summary>
        /// Raises the <see cref="E:AppearanceChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnChanged(EventArgs args)
        {
            if (Changed != null)
            {
                Changed(this, args);
            }
        }
        #endregion
    }

    /// <summary>
    /// Specifies the single range segment of axis.
    /// </summary>
    class ChartAxisSegment
    {
        #region Members
        private DoubleRange m_range;
        private double m_length;
        private double m_interval;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public DoubleRange Range
        {
            get { return m_range; }
            set { m_range = value; }
        }
        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public double Interval
        {
            get { return m_interval; }
            set { m_interval = value; }
        }
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get { return m_length; }
            set { m_length = value; }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChartAxisRange
    {
        #region Members
        private ChartAxis m_axis = null;
        private DoubleRange m_visibleRange = DoubleRange.Empty;

        private ArrayList m_segments = new ArrayList();
        private double m_breakAmount = 0.2;

        private ArrayList m_ranges = new ArrayList();
        private double m_visibleSum = 0;

        private bool m_isEmpty = true;
        private bool m_needUpdate = false;

        private ChartBreaksMode m_mode = ChartBreaksMode.Manual;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when appearance was changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion //Events

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                if (m_needUpdate)
                {
                    this.Recalculate();
                    m_needUpdate = false;
                }

                return m_isEmpty;
            }
        }
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        [DefaultValue(ChartBreaksMode.Manual)]
        public ChartBreaksMode BreaksMode
        {
            get { return m_mode; }
            set
            {
                if (m_mode != value)
                {
                    m_mode = value;
                    m_needUpdate = true;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Specifies the minimal ratio of differences between Y values. 
        /// If this value is 0.25, that means the axis will be broken if more a quarter of the chart space is empty. 
        /// </summary>
        /// <remarks>
        /// The value range is form 0.0 to 1.0.
        /// </remarks>
        /// <value>The break amount.</value>
        [DefaultValue(0.2d)]
        public double BreakAmount
        {
            get { return m_breakAmount; }
            set
            {
                value = ChartMath.MinMax(value, 0, 1);

                if (m_breakAmount != value)
                {
                    m_breakAmount = value;
                    m_needUpdate = true;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the segments.
        /// </summary>
        /// <value>The segments.</value>
        internal IList Segments
        {
            get
            {
                return m_segments;
            }
        }
        /// <summary>
        /// Gets the breaks.
        /// </summary>
        /// <value>The breaks.</value>
        internal IList Breaks
        {
            get
            {
                return m_ranges;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisRange"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        internal ChartAxisRange(ChartAxis axis)
        {
            m_axis = axis;
            m_axis.VisibleRangeChanged += new EventHandler(OnVisibleRangeChanged);
            m_visibleRange = new DoubleRange(m_axis.VisibleRange.min, m_axis.VisibleRange.max);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Computes the breaks by specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Compute(ChartSeriesCollection series)
        {
            bool supported = true;
            ArrayList values = new ArrayList();

            #region Compute values
            foreach (ChartSeries ser in series)
            {
                if (ser.Compatible && m_axis == ser.ActualYAxis)
                {
                    if (ser.BaseStackingType != ChartSeriesBaseStackingType.NotStacked)
                    {
                        supported = false;
                    }

                    foreach (ChartPoint point in ser.Points)
                    {
                        values.Add(point.YValues[0]);
                    }

                    if (ser.OriginDependent)
                    {
                        values.Add(m_axis.CustomOrigin ? m_axis.Origin : 0);
                    }
                }
            }

            values.Sort();
            #endregion

            if (supported && values.Count > 0)
            {
                #region Compute segments
                m_segments.Clear();

                DoubleRange baseRange = new DoubleRange(m_axis.Range.min, m_axis.Range.max);

                int pointsBySegment = 0;
                double lengthCoef = 1d / values.Count;
                double start = (double)values[0];
                double pos = 0;
                double last = double.NaN;

                foreach (double value in values)
                {
                    if (!double.IsNaN(last))
                    {
                        if (value - last > m_breakAmount * baseRange.Delta)
                        {
                            ChartAxisSegment segment = new ChartAxisSegment();

                            segment.Range = new DoubleRange(start, last);

                            if (pointsBySegment == 1)
                            {
                                segment.Range = DoubleRange.Inflate(segment.Range, m_axis.Range.interval / 2);
                            }

                            segment.Length = pointsBySegment * lengthCoef;
                            segment.Interval = segment.Range.Delta / (m_axis.DesiredIntervals * segment.Length);

                            pos += segment.Length;

                            m_segments.Add(segment);

                            pointsBySegment = 0;
                            start = value;
                        }
                    }

                    last = value;
                    pointsBySegment++;
                }

                if (pos < 1)
                {
                    ChartAxisSegment axisSegment = new ChartAxisSegment();

                    axisSegment.Range = new DoubleRange(start, (double)values[values.Count - 1]);

                    if (pointsBySegment == 1)
                    {
                        axisSegment.Range = DoubleRange.Inflate(axisSegment.Range, m_axis.Range.interval / 2);
                    }

                    axisSegment.Length = 1d - pos;
                    axisSegment.Interval = axisSegment.Range.Delta / (m_axis.DesiredIntervals * axisSegment.Length);

                    m_segments.Add(axisSegment);
                }

                #endregion

                #region Compute nice ranges
                foreach (ChartAxisSegment segment in m_segments)
                {
                    DoubleRange range = segment.Range;
                    double interval = Math.Min(segment.Interval, m_axis.Range.interval);

                    m_axis.CalculateNiceRange(ref range, ref interval, m_axis.RangePaddingType);

                    segment.Interval = interval;
                    segment.Range = range;
                }

                //ChartAxisSegment firstSegment = m_segments[0] as ChartAxisSegment;
                //ChartAxisSegment lastSegment = m_segments[m_segments.Count - 1] as ChartAxisSegment;

                //if (firstSegment.Range.Delta < firstSegment.Range.Start - m_axis.Range.min)
                //{
                //  firstSegment.Range = new DoubleRange(m_axis.Range.min, firstSegment.Range.End + firstSegment.Range.Delta);
                //}
                //else
                //{
                //  firstSegment.Range = new DoubleRange(m_axis.Range.min, firstSegment.Range.End);
                //}

                //lastSegment.Range = new DoubleRange(lastSegment.Range.Start, m_axis.Range.max);
                #endregion
            }

            m_needUpdate = true;
            //this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Unions the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        public void Union(DoubleRange range)
        {
            bool isNeedAdd = true;

            for (int i = 0; i < m_ranges.Count; )
            {
                DoubleRange current = (DoubleRange)m_ranges[i];

                if (range.Inside(current))
                {
                    m_ranges.RemoveAt(i);
                }
                else if (current.Inside(range))
                {
                    isNeedAdd = false;
                    break;
                }
                else if (current.IsIntersects(range))
                {
                    range += current;
                    m_ranges.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (isNeedAdd)
            {
                m_ranges.Add(range);
            }

            m_needUpdate = true;
            this.OnChanged(EventArgs.Empty);
        }
        /// <summary>
        /// Excludes the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        public void Exclude(DoubleRange range)
        {
            for (int i = 0; i < m_ranges.Count; i++)
            {
                DoubleRange current = (DoubleRange)m_ranges[i];

                if (range.Inside(current))
                {
                    m_ranges.RemoveAt(i);
                    i--;
                }
                else if (current.Inside(range))
                {
                    m_ranges[i] = new DoubleRange(current.Start, range.Start);
                    m_ranges.Insert(i + 1, new DoubleRange(range.End, current.End));
                }
                else if (current.IsIntersects(range))
                {
                    if (current.Start > range.Start && current.End > range.Start)
                    {
                        m_ranges[i] = new DoubleRange(range.End, current.End);
                    }
                    else
                    {
                        m_ranges[i] = new DoubleRange(current.Start, range.Start);
                    }
                }
            }

            m_needUpdate = true;
            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            m_ranges.Clear();
            m_needUpdate = true;
            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the specified value is visible.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVisible(double value)
        {
            if (!m_isEmpty)
            {
                if (m_mode == ChartBreaksMode.Auto)
                {
                    foreach (ChartAxisSegment axisSegment in m_segments)
                    {
                        if (axisSegment.Range.Inside(value))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else if (m_mode == ChartBreaksMode.Manual)
                {
                    foreach (DoubleRange range in m_ranges)
                    {
                        if (range.Start < value && range.End >= value)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return m_visibleRange.Inside(value);
        }

        /// <summary>
        /// Values to coefficient.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double ValueToCoeficient(double value)
        {
            double result = 0;

            if (!m_isEmpty)
            {
                switch (m_mode)
                {
                    case ChartBreaksMode.None:
                        result = m_visibleRange.Extrapolate(value);
                        break;

                    case ChartBreaksMode.Auto:
                        result = this.AutoValueToCoeficient(value);
                        break;

                    case ChartBreaksMode.Manual:
                        result = this.ManualValueToCoeficient(value);
                        break;
                }
            }
            else
            {
                result = m_visibleRange.Extrapolate(value);
            }

            return result;
        }
        /// <summary>
        /// Coefficients to value.
        /// </summary>
        /// <param name="coeficient">The coefficient.</param>
        /// <returns></returns>
        public double CoeficientToValue(double coeficient)
        {
            double result = 0;

            if (!m_isEmpty)
            {
                switch (m_mode)
                {
                    case ChartBreaksMode.None:
                        result = m_visibleRange.Interpolate(coeficient);
                        break;

                    case ChartBreaksMode.Auto:
                        result = this.AutoCoeficientToValue(coeficient);
                        break;

                    case ChartBreaksMode.Manual:
                        result = this.ManualCoeficientToValue(coeficient);
                        break;
                }
            }
            else
            {
                result = m_visibleRange.Interpolate(coeficient);
            }

            return result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Computes coefficient by the specified value for the automatic mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double AutoValueToCoeficient(double value)
        {
            double result = 0;

            foreach (ChartAxisSegment segment in m_segments)
            {
                if (segment.Range > value)
                {
                    break;
                }
                else if (segment.Range.Inside(value))
                {
                    result += segment.Length * segment.Range.Extrapolate(value);
                    break;
                }
                else
                {
                    result += segment.Length;
                }
            }

            return result;
        }
        /// <summary>
        /// Computes value by the specified coefficient for the automatic mode.
        /// </summary>
        /// <param name="coeficient">The coefficient.</param>
        /// <returns></returns>
        private double AutoCoeficientToValue(double coeficient)
        {
            double result = 0;

            foreach (ChartAxisSegment segment in m_segments)
            {
                if (coeficient < segment.Length)
                {
                    result += segment.Range.Interpolate(coeficient / segment.Length);
                    break;
                }
                else
                {
                    coeficient -= segment.Length;
                }
            }

            return result;
        }
        /// <summary>
        ///  Computes coefficient by the specified value for the manual mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double ManualValueToCoeficient(double value)
        {
            double result = value - m_visibleRange.Start;

            foreach (DoubleRange range in m_ranges)
            {
                DoubleRange intersection = DoubleRange.Intersect(range, m_visibleRange);

                if (!intersection.IsEmpty && intersection.Start < value)
                {
                    result -= Math.Min(intersection.Delta, value - intersection.Start);
                }
            }

            return result / (m_visibleRange.Delta - m_visibleSum);
        }
        /// <summary>
        /// Compute the coefficient of value for manual mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double ManualCoeficientToValue(double value)
        {
            double result = (m_visibleRange.Delta - m_visibleSum) * value + m_visibleRange.Start;

            foreach (DoubleRange range in m_ranges)
            {
                DoubleRange intersection = DoubleRange.Intersect(range, m_visibleRange);

                if (!intersection.IsEmpty && intersection.Start < value)
                {
                    result += intersection.Delta;
                }
            }

            return result;
        }

        /// <summary>
        /// Raises the <see cref="E:AppearanceChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnChanged(EventArgs args)
        {
            if (Changed != null)
            {
                Changed(this, args);
            }
        }
        /// <summary>
        /// Called when visible range is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnVisibleRangeChanged(object sender, EventArgs e)
        {
            m_visibleRange = new DoubleRange(m_axis.VisibleRange.min, m_axis.VisibleRange.max);
            this.Recalculate();
        }
        /// <summary>
        /// Recalculates the ranges.
        /// </summary>
        internal void Recalculate()
        {
            if (m_mode == ChartBreaksMode.Manual)
            {
                m_visibleSum = 0;

                foreach (DoubleRange range in m_ranges)
                {
                    DoubleRange intersection = DoubleRange.Intersect(range, m_visibleRange);

                    if (!intersection.IsEmpty)
                    {
                        m_visibleSum += intersection.Delta;
                    }
                }

                m_isEmpty = m_ranges.Count == 0;
            }
            else if (m_mode == ChartBreaksMode.Auto)
            {
                m_isEmpty = m_segments.Count == 0;
            }
            else
            {
                m_isEmpty = true;
            }
        }
        #endregion
    }
}
