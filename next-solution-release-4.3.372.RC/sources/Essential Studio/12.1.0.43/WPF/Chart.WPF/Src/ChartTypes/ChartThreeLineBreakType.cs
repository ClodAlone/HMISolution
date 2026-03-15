// <copyright file="ChartThreeLineBreakType.cs" company="Syncfusion">
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
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Windows.Controls;

    /// <summary>
    /// Represents Line break chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartLineBreakSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Declares m_bottomLeftPoint
        /// </summary>
        private IChartDataPoint m_bottomLeftPoint;

        /// <summary>
        /// Declares m_topRightPoint
        /// </summary>
        private IChartDataPoint m_topRightPoint;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty =
            DependencyProperty.Register("IsPriceUp", typeof(bool), typeof(ChartLineBreakSegment), new PropertyMetadata(false));
       
        /// <summary>
        /// Identifies the IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty =
            DependencyProperty.Register("IsPriceDown", typeof(bool), typeof(ChartLineBreakSegment), new PropertyMetadata(false));
        
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartLineBreakSegment), new PropertyMetadata(0d));
        
        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartLineBreakSegment), new PropertyMetadata(0d));
      
        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartLineBreakSegment), new PropertyMetadata(0d));
       
        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartLineBreakSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X segment co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y segment co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the segment. This is a dependency property.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the segment. This is a dependency property.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

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

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartLineBreakSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is beibg assigned automatically.
        /// </remarks>
        static ChartLineBreakSegment()
        {
            Type type = typeof(ChartLineBreakSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLineBreakSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPoint">The bottom left point.</param>
        /// <param name="topRightPoint">The top right point.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartLineBreakSegment(IChartDataPoint bottomLeftPoint, IChartDataPoint topRightPoint, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            m_bottomLeftPoint = bottomLeftPoint;
            m_topRightPoint = topRightPoint;

            this.SetXRange(bottomLeftPoint.X, topRightPoint.X);
            this.SetYRange(bottomLeftPoint.Y, topRightPoint.Y);
        }
        #endregion

        #region Implmentation
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
            Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y);
            Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y);
            Rect columnRect = new Rect(blPoint, trPoint);

            this.X = columnRect.X;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
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
    /// Represents ChartThreeLineBreakType class
    /// </summary>
    /// <remarks>
    /// Three Line Break Chart is similar in concept to point and figure charts. The
    /// Three Line Break charting method is so-named because of the number of lines
    /// typically used. It displays a series of vertical boxes ("lines") that are based
    /// on changes in prices. It ignores the passage of time.
    /// The three-line break chart looks like a series of rising and falling lines of
    /// varying heights. Each new line, like the Xs and Os of a point and figure chart,
    /// occupies a new column. Based on closing prices (or highs and lows), a new rising
    /// line is drawn if the previous high is exceeded and a new falling line is drawn
    /// if the price hits a new low. Change in price trends are highlighted by changing
    /// colors. Use the PriceUpColor to indicate bullish trend and PriceDownColor to
    /// indicate bearish trend.
    /// The ReversalAmount specifies the threshold amount by which the price should
    /// change to begin rendering a new vertical box in the appropriate direction.
    /// </remarks>
    /// <seealso cref="ChartLineBreakSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartThreeLineBreakType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the BreakLineCount dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineCountProperty =
      DependencyProperty.RegisterAttached("BreakLineCount", typeof(int), typeof(ChartThreeLineBreakType), new ChartPropertyMetadata(3, ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartThreeLineBreakType"/> class.
        /// </summary>
        internal ChartThreeLineBreakType()
        {
        }

        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
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
        /// Gets the break line count.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>the BreakLineCount</returns>
        public static int GetBreakLineCount(ChartSeries series)
        {
            return (int)series.GetValue(BreakLineCountProperty);
        }

        /// <summary>
        /// Sets the break line count.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetBreakLineCount(ChartSeries series, int value)
        {
            series.SetValue(BreakLineCountProperty, value);
        }

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            int count = series.PointsCount;
            int breakLineCount = GetBreakLineCount(series);
            bool axisIgnoring = true;
            List<LineBreakInformation> infoList = new List<LineBreakInformation>(count);
            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];

            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            for (int i = 1; i < cdpwiA.Length; i++)
            {
                double prevX = axisIgnoring ? infoList.Count : cdpwiA[i - 1].DataPoint.X;
                double nextX = axisIgnoring ? infoList.Count + 1 : cdpwiA[i].DataPoint.X;
                if (infoList.Count == 0)
                {
                    bool isNegative = cdpwiA[i - 1].DataPoint.Y > cdpwiA[i].DataPoint.Y;

                    ChartPoint ltPoint = new ChartPoint(prevX, cdpwiA[i - 1].DataPoint.Y);
                    ChartPoint rbPoint = new ChartPoint(nextX, cdpwiA[i].DataPoint.Y);

                    infoList.Add(new LineBreakInformation(ltPoint, rbPoint, isNegative, new ChartIndexedDataPoint[] { cdpwiA[i - 1], cdpwiA[i] }));
                }
                else
                {
                    bool isBreakLine = false;
                    LineBreakInformation rCIDInfo = infoList[infoList.Count - 1];
                    LineBreakInformation lCIDInfo = null;

                    if (infoList.Count >= breakLineCount)
                    {
                        lCIDInfo = infoList[infoList.Count - breakLineCount];

                        for (int j = 1; j < breakLineCount; j++)
                        {
                            LineBreakInformation cCIDInfo = infoList[infoList.Count - j];
                            LineBreakInformation nCIDInfo = infoList[infoList.Count - j - 1];

                            if (cCIDInfo.IsNegative == nCIDInfo.IsNegative)
                            {
                                isBreakLine = true;
                            }
                            else
                            {
                                isBreakLine = false;
                                break;
                            }
                        }
                    }

                    if ((isBreakLine && (cdpwiA[i].DataPoint.Y < Math.Min(lCIDInfo.Low, rCIDInfo.Low)))
                      || ((!isBreakLine) && (cdpwiA[i].DataPoint.Y < rCIDInfo.Low)))
                    {
                        ChartIndexedDataPoint[] cidps = new ChartIndexedDataPoint[] { rCIDInfo.CorrespondingPoints[rCIDInfo.CorrespondingPoints.Length - 1], cdpwiA[i] };
                        ChartPoint tlPoint = new ChartPoint(prevX, rCIDInfo.Low);
                        ChartPoint brPoint = new ChartPoint(nextX, cdpwiA[i].DataPoint.Y);

                        infoList.Add(new LineBreakInformation(tlPoint, brPoint, true, cidps));
                    }
                    else if ((isBreakLine && (cdpwiA[i].DataPoint.Y > Math.Max(lCIDInfo.High, rCIDInfo.High)))
                      || ((!isBreakLine) && (cdpwiA[i].DataPoint.Y > rCIDInfo.High)))
                    {
                        ChartIndexedDataPoint[] cidps = new ChartIndexedDataPoint[] { rCIDInfo.CorrespondingPoints[rCIDInfo.CorrespondingPoints.Length - 1], cdpwiA[i] };
                        ChartPoint tlPoint = new ChartPoint(prevX, rCIDInfo.High);
                        ChartPoint brPoint = new ChartPoint(nextX, cdpwiA[i].DataPoint.Y);

                        infoList.Add(new LineBreakInformation(tlPoint, brPoint, false, cidps));
                    }
                    else
                    {
                        List<ChartIndexedDataPoint> cidps = new List<ChartIndexedDataPoint>(rCIDInfo.CorrespondingPoints);
                        IChartDataPoint tlPoint = rCIDInfo.LeftTopPoint;
                        IChartDataPoint rbPoint = new ChartPoint(axisIgnoring ? prevX : nextX, rCIDInfo.IsNegative ? Math.Min(cdpwiA[i].DataPoint.Y, rCIDInfo.Low) : Math.Max(cdpwiA[i].DataPoint.Y, rCIDInfo.High));

                        cidps.Add(cdpwiA[i]);
                        infoList[infoList.Count - 1] = new LineBreakInformation(tlPoint, rbPoint, rCIDInfo.IsNegative, cidps.ToArray());
                    }
                }
            }

            List<ChartSegment> drawingList = new List<ChartSegment>(count);

            for (int i = 0; i < infoList.Count; i++)
            {
                LineBreakInformation info = infoList[i];
                ChartLineBreakSegment segment = new ChartLineBreakSegment(info.LeftTopPoint, info.RightBottomPoint, info.CorrespondingPoints, series);

                segment.IsPriceUp = !info.IsNegative;
                segment.IsPriceDown = info.IsNegative;

                drawingList.Add(segment);
            }

            foreach (ChartSegment segment in drawingList)
            {
                series.Segments.Add(segment);
            }
        }

        /// <summary>
        /// Updates the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <seealso>
        ///     <cref>ChartSThreeLineBreakType</cref>
        /// </seealso>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Internal types

        /// <summary>
        /// Represents LineBreakInformation
        /// </summary>
        internal class LineBreakInformation
        {
            /// <summary>
            /// Declares LeftTopPoint
            /// </summary>
            public IChartDataPoint LeftTopPoint;

            /// <summary>
            /// Declares RightBottomPoint
            /// </summary>
            public IChartDataPoint RightBottomPoint;

            /// <summary>
            /// Declares IsNegative
            /// </summary>
            public bool IsNegative;

            /// <summary>
            /// Declares CorrespondingPoints
            /// </summary>
            public ChartIndexedDataPoint[] CorrespondingPoints;

            /// <summary>
            /// Gets the low.
            /// </summary>
            /// <value>The low value.</value>
            public double Low
            {
                get
                {
                    return Math.Min(LeftTopPoint.Y, RightBottomPoint.Y);
                }
            }

            /// <summary>
            /// Gets the high.
            /// </summary>
            /// <value>The high value.</value>
            public double High
            {
                get
                {
                    return Math.Max(LeftTopPoint.Y, RightBottomPoint.Y);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LineBreakInformation"/> class.
            /// </summary>
            /// <param name="ltPoint">The lt point.</param>
            /// <param name="rbPoint">The rb point.</param>
            /// <param name="negative">if set to <c>true</c> [negative].</param>
            /// <param name="correspondingPoints">The corresponding points.</param>
            public LineBreakInformation(IChartDataPoint ltPoint, IChartDataPoint rbPoint, bool negative, ChartIndexedDataPoint[] correspondingPoints)
            {
                LeftTopPoint = ltPoint;
                RightBottomPoint = rbPoint;
                IsNegative = negative;
                CorrespondingPoints = correspondingPoints;
            }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartThreeLineBreakType"/>
        public override string ToString()
        {
            return "ThreeLineBreak";
        }
    }
}
