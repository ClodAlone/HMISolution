// <copyright file="ChartBreakRange.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents ChartBreakRange
    /// </summary>
    /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartBreakRange : FrameworkElement
    {
        #region Members
        /// <summary>
        /// Initializes m_sumBreaks
        /// </summary>
        private double m_sumBreaks;
        
        /// <summary>
        /// Initializes m_ranges
        /// </summary>
        private List<DoubleRange> m_ranges = new List<DoubleRange>();
        internal List<DoubleRange> yAxisSegments;
        internal List<DoubleRange> m_breakSegments;
        internal double m_breakSegmentsLength = 0;
		private Dictionary<DoubleRange, ChartBreakRangeInfo> m_rangesInfo = new Dictionary<DoubleRange, ChartBreakRangeInfo>();
        private ChartBreakRangeInfo m_breakRangeInfo = new ChartBreakRangeInfo();
        internal ChartAxis m_axis = null;
        private double m_autoBreakThresholdCoefficient = 0.2d;
        private ArrayList m_segments = new ArrayList();
        internal ChartBreaksModes m_breaksMode = ChartBreaksModes.Manual;
        #endregion 

        #region Events
        /// <summary>
        /// Occurs when range was changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion //Events

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis. 
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartBreakRange), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartBreakRange), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for BreaksMode.
        /// </summary>
        public static readonly DependencyProperty BreaksModeProperty =
          DependencyProperty.Register("BreaksMode", typeof(ChartBreaksModes), typeof(ChartBreakRange), new UIPropertyMetadata(ChartBreaksModes.Manual));

        /// <summary>
        /// Using a DependencyProperty as the backing store for BreaksMode.
        /// </summary>
        public static readonly DependencyProperty AutoBreakThresholdCoefficientProperty =
          DependencyProperty.Register("AutoBreakThresholdCoefficient", typeof(double), typeof(ChartBreakRange), new UIPropertyMetadata(0.2d));       

       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X axis.
        /// </summary>
        /// <value>The X axis.</value>
        public ChartAxis XAxis
        {
            get
            {
                ChartAxis xAxis = (ChartAxis)GetValue(XAxisProperty);
                if (xAxis.EnableBreaks)
                    this.m_axis = xAxis;
                return (ChartAxis)GetValue(XAxisProperty);
            }

            set
            {
                SetValue(XAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y axis.
        /// </summary>
        /// <value>The Y axis.</value>
        public ChartAxis YAxis
        {
            get
            {
                ChartAxis yAxis = (ChartAxis)GetValue(YAxisProperty);
                if (yAxis.EnableBreaks)
                    this.m_axis = yAxis;
                return (ChartAxis)GetValue(YAxisProperty);
            }

            set
            {
                SetValue(YAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the breaks mode.
        /// </summary>
        public ChartBreaksModes BreaksMode
        {
            get
            {
                return (ChartBreaksModes)GetValue(BreaksModeProperty);
            }

            set
            {
                m_breaksMode = value;
                SetValue(BreaksModeProperty, value);
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the break amount.
        /// </summary>
        public double AutoBreakThresholdCoefficient
        {
            get
            {
                return (double)GetValue(AutoBreakThresholdCoefficientProperty);
            }

            set
            {
                value = ChartMath.MinMax(value, 0, 1);
                m_autoBreakThresholdCoefficient = value;
                if (value == 0)
                    m_breaksMode = ChartBreaksModes.None;
                else
                    m_breaksMode = BreaksMode;
                
                SetValue(AutoBreakThresholdCoefficientProperty, value);
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the sum breakes.
        /// </summary>
        /// <value>The sum breakes.</value>
        public double SumBreaks
        {
            get { return this.m_sumBreaks; }
        }

        /// <summary>
        /// Get the CLR property from the internal variable
        /// </summary>
        public Dictionary<DoubleRange, ChartBreakRangeInfo> Breaks
        {
            get { return m_rangesInfo; }
        }

        /// <summary>
        /// Gets a value indicating whether this range is empty.
        /// </summary>
        /// <value><c>true</c> if this range is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return this.m_ranges.Count == 0;
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
        #endregion //Properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBreakRange"/> class.
        /// </summary>
        public ChartBreakRange()
        {
        }
        /// <summary>
        /// Called when instance created for ChartBreakRange
        /// </summary>
        /// <param name="axis"></param>
        public ChartBreakRange(ChartAxis axis)
        {
            this.m_axis = axis;
        }
        #endregion //Constructor

        #region Public methods
       
        /// <summary>
        /// Computes coefficient by the specified value for the automatic mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double AutoValueToCoeficient(double value)
        {
            double result = 0;

            if (m_segments != null && m_segments.Count > 0 && m_breaksMode == ChartBreaksModes.Auto)
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
            else if(m_axis != null)
            {
                result = (value - m_axis.VisibleRange.Start) / m_axis.VisibleRange.Delta;
            }

            return result;
        }
        /// <summary>
        /// Computes value by the specified coefficient for the automatic mode.
        /// </summary>
        /// <param name="coeficient">The coefficient.</param>
        /// <returns></returns>
        public double AutoCoeficientToValue(double coeficient)
        {
            double result = 0;
            double length = m_breakSegmentsLength;
            if (m_segments != null && m_segments.Count > 0 && m_breaksMode == ChartBreaksModes.Auto)
            {
                for (int i = 0; i < m_breakSegments.Count; i++)
                {
                    DoubleRange segment = m_breakSegments[i];
                    if (coeficient == 0)
                    {
                        result = m_breakSegments[0].Start;
                        break;
                    }
                    if (coeficient < length)
                    {
                        result = segment.Start + (segment.Delta * ((length - coeficient) / m_breakSegmentsLength));
                        break;
                    }
                    else if (coeficient == Math.Round(length, 1))
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
            else if(this.m_axis != null)
            {
                result = m_axis.VisibleRange.Start + m_axis.VisibleRange.Delta * coeficient;
            }

            return result;
        }

        /// <summary>
        /// Unions the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="breakInfo"></param>
        public void Union(DoubleRange range, ChartBreakRangeInfo breakInfo)
        {
            bool isNeedAdd = true;

            if(this.m_axis.EnableBreaks)
            for (int i = 0; i < this.m_ranges.Count;)
            {
                DoubleRange current = this.m_ranges[i];

                if (range.Inside(current))
                {
                    this.m_ranges.RemoveAt(i);
                    this.m_rangesInfo.Remove(range);
                }
                else if (current.Inside(range))
                {
                    isNeedAdd = false;
                    break;
                }
                else if (current.Intersects(range))
                {
                    range += current;
                    this.m_ranges.RemoveAt(i);
                    this.m_rangesInfo.Remove(range);
                }
                else
                {
                    i++;
                }
            }

            if (isNeedAdd)
            {
                this.m_ranges.Add(range);
                this.m_rangesInfo.Add(range, breakInfo);
            }

            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Excludes the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        public void Exclude(DoubleRange range)
        {
            for (int i = 0; i < this.m_ranges.Count; i++)
            {
                DoubleRange current = this.m_ranges[i];

                if (range.Inside(current))
                {
                    this.m_ranges.RemoveAt(i);
                    this.m_rangesInfo.Remove(range);
                    i--;
                }
                else if (current.Inside(range))
                {
                    this.m_ranges[i] = new DoubleRange(current.Start, range.Start);
                    this.m_ranges.Insert(i + 1, new DoubleRange(range.End, current.End));
                    this.m_rangesInfo.Add(new DoubleRange(range.End, current.End), new ChartBreakRangeInfo());
                }
                else if (current.Intersects(range))
                {
                    if (current > range.Start)
                    {
                        this.m_ranges[i] = new DoubleRange(range.End, current.End);
                    }
                    else
                    {
                        this.m_ranges[i] = new DoubleRange(current.Start, range.Start);
                    }
                }
            }

            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.m_ranges.Clear();
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Subtracts breakes from the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result</returns>
        public double Subtract(double value)
        {
            double result = value;

            foreach (DoubleRange range in this.m_ranges)
            {
                if (range.Start < value)
                {
                    result -= Math.Min(range.Delta, value - range.Start);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds breaks to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The double value</returns>
        /// <seealso cref="ChartBreakRange"/>
        public double Add(double value)
        {
            double result = value;

            foreach (DoubleRange range in this.m_ranges)
            {
                if (range.Start < value)
                {
                    result += range.Delta;
                }
            }

            return result;
        }

        /// <summary>
        /// The IsInBreak method
        /// </summary>
        /// <param name="value">The double value</param>
        /// <returns>The bool result</returns>
        public bool IsInBreak(double value)
        {
            bool result = false;

            foreach (DoubleRange range in this.m_ranges)
            {
                if (range.Inside(value))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        #endregion //Public methods

        #region Implementation
        /// <summary>
        /// Computes the sum of breakes.
        /// </summary>
        private void CalculateSumBreakes()
        {
           this.m_sumBreaks = 0;

            for (int i = 0; i < this.m_ranges.Count; i++)
            {
               this.m_sumBreaks += this.m_ranges[i].Delta;
            }
        }
/// <summary>
/// Method implementation for Compute. It can compute the segments and values
/// </summary>
/// <param name="series"></param>
        public void Compute(ChartSeriesCollection series)
        {
            bool supported = true;
            ArrayList values = new ArrayList();
            yAxisSegments = new List<DoubleRange>();
            m_breakSegments = new List<DoubleRange>();

            #region Compute values
            foreach (ChartSeries ser in series)
            {
                if (ser.ChartType.IsCompatible(ser.ChartType) && m_axis == ser.ActualYAxis && ser.Data != null && ser.Data.Count > 0 )
                {
                    if (ser.ChartType.IsStacked )
                    {
                        supported = false;
                    }

                    for (int i = 0; i < ser.Data.Count; i++)
                    {
                        for (int j = 0; j < ser.Data[i].Values.Length; j++)
                        {
                            if(!values.Contains(ser.Data[i].Values[j]))
                            values.Add(ser.Data[i].Values[j]);
                        }
                    }

                    if (ser.OriginDependent)
                    {
                        values.Add(m_axis.IsOriginCentered ? m_axis.Origin : 0);
                    }
                }
            }

            values.Sort();

            if (values.Count > 0)
            {
                double iStart = m_axis.VisibleRange.Start;
                double iEnd = m_axis.VisibleRange.Start;
                while (iEnd < m_axis.VisibleRange.End)
                {
                    iEnd += m_axis.VisibleInterval;
                    yAxisSegments.Add(new DoubleRange(iStart, iEnd));
                    iStart = iEnd;
                }
            }

            for (int i = 0; i < values.Count; i++ )
            {
                bool isIntervalValue = false;
                foreach (DoubleRange range in yAxisSegments)
                {
                    if (range.Inside((double)values[i]))
                    {
                        if (m_breakSegments.Count == 0)
                        {
                            m_breakSegments.Add(range);
                            if ((double)values[i] % m_axis.VisibleInterval == 0)
                                continue;
                            else
                                break;
                        }
                        else if (!m_breakSegments.Contains(range))
                        {
                            m_breakSegments.Add(range);
                            if ((double)values[i] % m_axis.VisibleInterval == 0 && !isIntervalValue)
                            {
                                isIntervalValue = true;
                                continue;
                            }
                            else
                                break;
                        }
                        else if (m_breakSegments.Contains(range) && (double)values[i] % m_axis.VisibleInterval == 0)
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

            if (supported && values.Count > 0)
            {
                #region Compute segments
                m_segments.Clear();

                DoubleRange baseRange = new DoubleRange(m_axis.VisibleRange.Start, m_axis.VisibleRange.End);

                double start = (double)values[0];
                double last = double.NaN;
                int index = -1;

                foreach (double value in values)
                {
                    index++;
                    if (!double.IsNaN(last) && m_autoBreakThresholdCoefficient > 0)
                    {
                        if (value - last > m_autoBreakThresholdCoefficient * baseRange.Delta)
                        {
                            ChartAxisSegment segment = new ChartAxisSegment();

                            segment.Range = new DoubleRange(start, last);

                            double _start = segment.Range.Start - (m_axis.VisibleInterval / 2);
                            double _end = segment.Range.End + (m_axis.VisibleInterval / 2);

                            if(_start >= m_axis.m_visibleRange.Start && _end <= m_axis.m_visibleRange.End)
                            {
                                segment.Range = new DoubleRange( _start, _end);
                            }
                            m_segments.Add(segment);

                            start = value;
                        }
                    }

                    last = value;
                }

                if (m_segments.Count == 0)
                {
                    m_axis.BreakRange.m_breaksMode = ChartBreaksModes.None;
                }
                else
                {
                    m_axis.BreakRange.m_breaksMode = m_axis.BreakRange.BreaksMode;
                    ChartAxisSegment lastSegment = m_segments[m_segments.Count - 1] as ChartAxisSegment;
                    if (lastSegment.Range.End < m_axis.VisibleRange.End)
                    {
                        ChartAxisSegment axisSegment = new ChartAxisSegment();
                        axisSegment.m_remainingSegment = true;
                        axisSegment.Range = new DoubleRange(start, m_axis.VisibleRange.End);
                        axisSegment.Range = new DoubleRange(axisSegment.Range.Start - (m_axis.VisibleInterval / 2), m_axis.VisibleRange.End);
                        m_segments.Add(axisSegment);
                    }
                }

                for (int i = 0, ci = m_segments.Count - 1; i < ci; i++)
                {
                    ChartAxisSegment axisSegment1 = m_segments[i] as ChartAxisSegment;
                    ChartAxisSegment axisSegment2 = m_segments[i + 1] as ChartAxisSegment;

                    DoubleRange brkRange = new DoubleRange(axisSegment1.Range.End, axisSegment2.Range.Start);
                    bool isIntervalValue = false;
                    if (brkRange.Delta > 0)
                    {
                        foreach (DoubleRange range in yAxisSegments)
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
                        if (compare > 0 )
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
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
       private void OnChanged(EventArgs args)
        {
            this.CalculateSumBreakes();
            RaiseChanged(this, args);
        }

        /// <summary>
        /// Raises the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RaiseChanged(object sender, EventArgs args)
        {
            if (this.Changed != null)
            {
                this.Changed(sender, args);
            }
        }
        #endregion //Implementation

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            IChartTransformer transformer = null;
            ChartBreakRangeInfo brkRangeInfo = new ChartBreakRangeInfo();
            ChartArea parent = (this.YAxis!=null && this.YAxis.EnableBreaks) ? this.YAxis != null ? this.YAxis.Area :null : this.XAxis != null ? this.XAxis.Area : null;
            ChartAxis xAxis = null;
            ChartAxis yAxis = null;
            Point from, to;
            //Included null check to fix null reference exception when unchecking all legend's check box 
            if (parent !=null && parent.PrimarySeries != null)
            {
                xAxis = parent.PrimarySeries.ActualXAxis;
                yAxis = parent.PrimarySeries.ActualYAxis;
                transformer = ChartTransform.CreateCartesian(new Rect(new Point(0, 0), new Size(this.ActualWidth, this.ActualHeight)), parent.PrimarySeries);
            }

            if (yAxis != null && yAxis.EnableBreaks && yAxis.BreakRange.m_breaksMode == ChartBreaksModes.Manual)
            {
                foreach (KeyValuePair<DoubleRange, ChartBreakRangeInfo> brks in yAxis.BreakRange.Breaks)
                {                    
                    from = transformer.TransformToVisible(xAxis.VisibleRange.Start, brks.Key.End);
                    to = transformer.TransformToVisible(xAxis.VisibleRange.End, brks.Key.End);
                    brkRangeInfo.DrawBreakLine(drawingContext, from, to, brks.Value);                       
                }
            }
            if (yAxis != null && yAxis.EnableBreaks && yAxis.BreakRange.m_breaksMode == ChartBreaksModes.Auto)
            {                
                for (int i = 0, ci = yAxis.BreakRange.Segments.Count - 1; i < ci; i++)
                {
                    ChartAxisSegment axisSegment1 = yAxis.BreakRange.Segments[i] as ChartAxisSegment;
                    ChartAxisSegment axisSegment2 = yAxis.BreakRange.Segments[i + 1] as ChartAxisSegment;

                    DoubleRange brkRange = new DoubleRange(axisSegment1.Range.End, axisSegment2.Range.Start);

                    if (yAxis.VisibleRange.Inside(brkRange))
                    {
                        from = transformer.TransformToVisible(xAxis.VisibleRange.Start, brkRange.End);
                        to = transformer.TransformToVisible(xAxis.VisibleRange.End, brkRange.End);
                        brkRangeInfo.DrawBreakLine(drawingContext, from, to, new ChartBreakRangeInfo());
                    }
                }
            }
            base.OnRender(drawingContext);
        }
    }

    /// <summary>
    /// Specifies the single range segment of axis.
    /// </summary>
    class ChartAxisSegment
    {
        #region Members
        private DoubleRange m_range;
        internal bool m_remainingSegment;
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

    /// <summary>
    /// Specifies types of break line.
    /// </summary>
    public enum ChartBreakLineTypes
    {
        /// <summary>
        /// The straight line.
        /// </summary>
        StraightLine,
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
    public enum ChartBreaksModes
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
    /// Class declaratiopn for ChartBreakRangeInfo
    /// </summary>
    public class ChartBreakRangeInfo 
    {
        #region Members
        private ChartBreakRange breakRange;
        private ChartBreakLineTypes m_lineType = ChartBreakLineTypes.Wave;
        private Brush m_lineColor = Brushes.Black;
        private Brush m_spacingColor = Brushes.White;
        private System.Windows.Media.DashStyle m_lineStyle = DashStyles.Solid;
        private double m_lineWidth = 1d;
        private double m_SpacingWidth = 2d;
        #endregion //Members
        
        #region Constructor
        /// <summary>
        /// Constructor for ChartBreakRangeInfo
        /// </summary>
        public ChartBreakRangeInfo()
        {
        }
        /// <summary>
        /// Called when instance created for ChartBreakRangeInfo with single arguments
        /// </summary>
        /// <param name="brkRange"></param>
        public ChartBreakRangeInfo(ChartBreakRange brkRange)
        {
            this.breakRange = brkRange;
            this.Changed += new EventHandler(this.OnNeedRedraw);

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>The type of the line.</value>
        [DefaultValue(ChartBreakLineTypes.StraightLine)]
        [Description("Gets or sets the type of the line.")]
        public ChartBreakLineTypes LineType
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
        public Brush LineColor
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
        [Description("Gets or sets the line style.")]
        public System.Windows.Media.DashStyle LineStyle
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
        [DefaultValue(1d)]
        [Description("Gets or sets the width of the line.")]
        public double LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                value = ChartMath.MinMax(value, 0, 10);

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
        [DefaultValue(2d)]
        [Description("Gets or sets the line spacing width.")]
        public double SpacingWidth
        {
            get { return m_SpacingWidth; }
            set
            {
                value = ChartMath.MinMax(value, 0, 10);

                if (m_SpacingWidth != value)
                {
                    m_SpacingWidth = value;
                    this.OnChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the spacing.
        /// </summary>
        /// <value>The color of the spacing.</value>
        [DefaultValue(typeof(Brush), "White")]
        [Description("Gets or sets the color of the spacing.")]
        public Brush SpacingColor
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

        #region Events
        /// <summary>
        /// Occurs when range was changed.
        /// </summary>
        public event EventHandler Changed;
        /// <summary>
        /// Handles visual properties changed events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnNeedRedraw(object sender, EventArgs args)
        {
            this.breakRange.m_axis.OnBreakRangeChanged(sender, args);
        }
        #endregion //Events

        #region Implementation

        /// <summary>
        /// Draws the break line.
        /// </summary>
        /// <param name="dc">The <see cref="System.Drawing.Graphics"/>.</param>
        /// <param name="from">The start point of break line.</param>
        /// <param name="to">The end point of break line.</param>
        /// <param name="info"></param>
        public void DrawBreakLine(DrawingContext dc, Point from, Point to, ChartBreakRangeInfo info)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double d = (double)Math.Sqrt(dx * dx + dy * dy);
            double offset = (double)info.SpacingWidth;

            double offsetX = offset * dy / d;
            double offsetY = offset * dx / d;

            Point[] points = null;
            switch (info.LineType)
            {
                case ChartBreakLineTypes.StraightLine:
                    points = new Point[]{ from, new Point(from.X + 0.25f * dx, from.Y + 0.25f * dy),
						     new Point(from.X + 0.75f * dx, from.Y + 0.75f * dy),to};
                    break;

                case ChartBreakLineTypes.Wave:
                    points = GetWaveBeziersPoints(from, to, 20, 5);
                    break;

                case ChartBreakLineTypes.Randomize:
                    points = GetRandomBeziersPoints(from, to, 10, 3);
                    break;
            }
            if (info.SpacingWidth > 0)
            {
                Point[] nearPoints = new Point[points.Length];
                Point[] farPoints = new Point[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    nearPoints[i] = new Point(points[i].X - offsetX, points[i].Y - offsetY);
                    farPoints[points.Length - i - 1] = new Point(points[i].X + offsetX, points[i].Y + offsetY);
                }

                if (info.LineType == ChartBreakLineTypes.StraightLine)
                    dc.DrawRectangle(info.SpacingColor, new Pen() { Brush = info.LineColor, Thickness = info.LineWidth, DashStyle = info.LineStyle }, new Rect(nearPoints[0], farPoints[0]));

                else
                {
                    Geometry geometry;
                    PathFigure figure1 = new PathFigure();                   
                    figure1.StartPoint = new Point(nearPoints[0].X, nearPoints[0].Y);
                    Point point1, point2, point3;

                    for (int i = 1; i < nearPoints.Length ; i = i + 3 )
                    {
                        point1 = new Point(nearPoints[i].X, nearPoints[i].Y);
                        point2 = new Point(nearPoints[i + 1].X, nearPoints[i + 1].Y);
                        point3 = new Point(nearPoints[i + 2].X, nearPoints[i + 2].Y);
                        figure1.Segments.Add(new BezierSegment(point1, point2, point3, true));                        
                    }
                    figure1.Segments.Add(new BezierSegment(farPoints[0], farPoints[0], farPoints[0], true));
                    for (int i = 1; i < nearPoints.Length ; i = i + 3)
                    {
                        point1 = new Point(farPoints[i].X, farPoints[i].Y);
                        point2 = new Point(farPoints[i + 1].X, farPoints[i + 1].Y);
                        point3 = new Point(farPoints[i + 2].X, farPoints[i + 2].Y);
                        figure1.Segments.Add(new BezierSegment(point1, point2, point3, true));
                    }

                    geometry = new PathGeometry(new PathFigure[] { figure1 });
                    dc.DrawGeometry(info.SpacingColor, new Pen() { Brush = info.LineColor, Thickness = info.LineWidth, DashStyle = info.LineStyle }, geometry);
                }
            }
            else
            {
                Geometry geometry;
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(points[0].X, points[0].Y);

                Point point1, point2, point3;
                for (int k = 1; k < points.Length; k = k + 3)
                {
                    point1 = new Point(points[k].X, points[k].Y);
                    point2 = new Point(points[k + 1].X, points[k + 1].Y);
                    point3 = new Point(points[k + 2].X, points[k + 2].Y);
                    figure.Segments.Add(new BezierSegment(point1, point2, point3, true));
                }
                geometry = new PathGeometry(new PathFigure[] { figure });
                dc.DrawGeometry(Brushes.Transparent, new Pen() { Brush = info.LineColor, Thickness = info.LineWidth, DashStyle=info.LineStyle}, geometry);
            }

        }
        /// <summary>
        /// Gets the wave beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public static Point[] GetWaveBeziersPoints(Point pt1, Point pt2, int count, float fault)
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
        /// Gets the rendom beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public static Point[] GetRandomBeziersPoints(Point pt1, Point pt2, int count, float fault)
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
}
