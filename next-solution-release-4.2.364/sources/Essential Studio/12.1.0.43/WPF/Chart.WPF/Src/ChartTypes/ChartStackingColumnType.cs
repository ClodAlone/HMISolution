// <copyright file="ChartStackingColumnType.cs" company="Syncfusion">
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
    using System.Windows.Media;

    /// <summary>
    /// Represents column chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartStackingColumnType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartStackingColumnSegment : ChartColumnSegment
    {
        #region dependency properties
        /// <summary>
        /// Identifies the IsLower dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLowerProperty =
            DependencyProperty.Register("IsLower", typeof(bool), typeof(ChartStackingColumnSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsUpper dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUpperProperty =
            DependencyProperty.Register("IsUpper", typeof(bool), typeof(ChartStackingColumnSegment), new PropertyMetadata(false));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this segment is lower part of stack.
        /// </summary>
        /// <value><c>true</c> if this instance is lower; otherwise, <c>false</c>.</value>
        public bool IsLower
        {
            get { return (bool)GetValue(IsLowerProperty); }
            set { SetValue(IsLowerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is upper part of stack.
        /// </summary>
        /// <value><c>true</c> if this instance is upper; otherwise, <c>false</c>.</value>
        public bool IsUpper
        {
            get { return (bool)GetValue(IsUpperProperty); }
            set { SetValue(IsUpperProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStackingColumnSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartStackingColumnSegment(IChartDataPoint bottomLeftPnt, IChartDataPoint topRightPnt, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(bottomLeftPnt, topRightPnt, correspondingPoint, series)
        {
        }
        #endregion
    }

    /// <summary>
    /// Represents stacking column chart type.
    /// </summary>
    /// <remarks>
    /// Stacking Column Charts are similar to regular column charts except that the Y
    /// values stack on top of each other in the specified series order. This helps
    /// visualize the relationship of parts to the whole.
    /// </remarks>
    /// <seealso cref="ChartStackingColumnSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStackingColumnType : ChartColumnType
    {
        #region Properties

        /// <summary>
        /// Identifies the RequiresNegativeSeriesStack dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresNegativeSeriesStackProperty =
              DependencyProperty.RegisterAttached("RequiresNegativeSeriesStack", typeof(bool), typeof(ChartStackingColumnType), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRequiresNegativeSeriesStackChanged)));

        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Stacked | ChartTypeFlags.Indexed;
            }
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Called when [requires negative series stack changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRequiresNegativeSeriesStackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            ChartType.UpdateSeriesInArea(area);
        }

        /// <summary>
        /// Specifies that the positive and negative series should be separately stacked.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/> where the stacking should happen.</param>
        /// <param name="value">True to stack separately, false otherwise.</param>
        /// <seealso cref="ChartStackingColumnType"/>
        public static void SetRequiresNegativeSeriesStack(ChartArea area, bool value)
        {
            area.SetValue(RequiresNegativeSeriesStackProperty, value);
        }

        /// <summary>
        /// Returns whether the positive and negative series should be separately stacked.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/> where the stacking should happen.</param>
        /// <returns>True indicates the positive and negative series are stacked separately.</returns>
        public static bool GetRequiresNegativeSeriesStack(ChartArea area)
        {
            return (bool)area.GetValue(RequiresNegativeSeriesStackProperty);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if (series.Area != null)
            {
                int seriesIndex = series.Area.Series.IndexOf(series);

                bool isLower = seriesIndex == 0;
                bool isUpper = seriesIndex == series.Area.Series.Count - 1;

                DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                int firstStackedIndex = 0;

                for (int i = 0; i < series.Area.Series.Count; i++)
                {
                    if (series.Area.Series[i].Type == ChartTypes.StackingColumn)
                    {
                        firstStackedIndex = i;
                        break;
                    }

                }
                bool isFirstStackedLineSeries = false;
                if (series.Area != null)
                    if (series.Area.Series.Count >= firstStackedIndex)
                    {
                        if (series == series.Area.Series[firstStackedIndex])
                        {
                            isFirstStackedLineSeries = true;

                        }
                    }
                for (int i = 0; i < points.Length; i++)
                {
                    //if (points[i].DataPoint.EmptyPoint)
                    //{
                    //    points[i].DataPoint.Y = 0;
                    //}
                    double x1 = 0, x2 = 0;
                    if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative && !series.IsIndexed)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 1 ? points[i].DataPoint.Values[1] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    else
                    {
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                    bool? forPositive = null;
                    if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(series.Area))
                    {
                        forPositive = points[i].DataPoint.Y >= 0;
                    }

                    double y2 = series.Area.GetStackInfo(series, points[i].DataPoint.X, forPositive);
                    double y1 = 0d;
                    if (!isFirstStackedLineSeries)
                        y2 = y2 - series.Area.SecondaryAxis.Origin;
                    
                      
                    if (forPositive == null)
                        y1 = isFirstStackedLineSeries ? Math.Abs(points[i].DataPoint.Y) : y2 + Math.Abs(points[i].DataPoint.Y);
                    else
                        y1 = isFirstStackedLineSeries ? points[i].DataPoint.Y :points[i].DataPoint.Y + y2;

                    DoubleRange yRange = new DoubleRange(y1, y2);

                    ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                    ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                    ////If Emptypoint, make a difference in segment rendering
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                            {
                                series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                ChartStackingColumnSegment sector = new ChartStackingColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                                sector.Interior = series.EmptyPointInterior;
                                sector.IsLower = isLower;
                                sector.IsUpper = isUpper;

                                series.Segments.Add(sector);

                            }
                            else
                            {
                                series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                                series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                            }
                        }
                        else
                        {
                            ChartStackingColumnSegment sector = new ChartStackingColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                            sector.Interior = Brushes.Transparent;
                            sector.Stroke = Brushes.Transparent;
                            sector.IsLower = isLower;
                            sector.IsUpper = isUpper;

                            series.Segments.Add(sector);
                        }
                    }
                    else
                    {
                        ChartStackingColumnSegment sector = new ChartStackingColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);

                        sector.IsLower = isLower;
                        sector.IsUpper = isUpper;

                        series.Segments.Add(sector);
                    }
                    if (i == 0)
                        ChartType.SetColumnInitialSegmentWidthValue(series, x1);
                    if (i == points.Length - 1)
                        ChartType.SetEndSegmentWidth(series, x2);

                }
                ChartIndexedDataPoint[] StackPoints = points as ChartIndexedDataPoint[];

                if (series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        //if (points[i].DataPoint.EmptyPoint)
                        //{
                        //    points[i].DataPoint.Y = 0;
                        //}
                        //double x1 = points[i].DataPoint.X + sbsInfo.Start;
                        //double x2 = points[i].DataPoint.X + sbsInfo.End;

                        //bool? forPositive = null;
                        //if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(series.Area))
                        //{
                        //    forPositive = points[i].DataPoint.Y >= 0;
                        //}

                        //double y2 = series.Area.GetStackInfo(series, points[i].DataPoint.X, forPositive);
                        //double y1 = 0d;
                        //if (points[i].DataPoint.EmptyPoint)
                        //{
                        //    y1 = 0 + y2;
                        //}
                        //else
                        //{
                        //    y1 = points[i].DataPoint.Y + y2;
                        //}

                        //ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        //ChartPoint cdpRightTop = new ChartPoint(x2, y2);

                        //StackPoints[i].DataPoint.Y =y_Ad[i];
                        //Add for SD11357
                        bool? forPositive = null;
                        if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(series.Area))
                        {
                            forPositive = points[i].DataPoint.Y >= 0;
                        }

                        double y2 = series.Area.GetStackInfo(series, points[i].DataPoint.X, forPositive);
                        double y1 = 0d;

                        if (forPositive == null)
                            y1 = Math.Abs(points[i].DataPoint.Y) + y2;
                        else
                            y1 = points[i].DataPoint.Y + y2;

                        if (y1 < 0 || y2 < 0)
                            series.AdornmentsInfo.m_requiresSymmetricLabelling = true;

                        DoubleRange yRange = new DoubleRange(y1, y2);
                        double bottomPoint = (Math.Abs(yRange.Start) < Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                        double topPoint = (Math.Abs(yRange.Start) > Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                        double midPoint = yRange.Median;

                        if (points[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                    series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                                else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                    series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                                else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                    series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                            }
                        }
                        else
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                        }
                    }

                }
            }


        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartStackingColumnType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion

        /// <summary>
        /// Converts ChartColumnType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartStackingColumnType"/>
        public override string ToString()
        {
            return "StackingColumn";
        }
    }
}
