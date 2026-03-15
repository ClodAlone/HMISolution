// <copyright file="ChartStackingColumn100Type.cs" company="Syncfusion">
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
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents stacking 100% column type.
    /// </summary>
    /// <remarks>
    /// In the 100 % Stacked Column Chart, the cumulative proportion of each stacked
    /// element always totals 100%. This type of chart is great to visualize the
    /// relative contribution of each series values to the whole.
    /// </remarks>
    /// <seealso cref="ChartStackingColumn100Segment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStackingColumn100Type : ChartStackingColumnType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies ShowValueAsProbability attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValueAsProbabilityProperty =
            DependencyProperty.RegisterAttached("ShowValueAsProbability", typeof(bool), typeof(ChartStackingColumn100Type), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueAsProbabilityChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Determines whether the this chart type is compatible with specified type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>
        /// <c>true</c> if the type is compatible; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCompatible(ChartType type)
        {
            return type is ChartStackingColumn100Type;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when [show value as probability changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowValueAsProbabilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            UpdateSeriesInArea(area);
        }
        
        /// <summary>
        /// Gets the show value as probability.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <returns>True is the value is to be displayed as probability</returns>
        public static bool GetShowValueAsProbability(ChartArea area)
        {
            return (bool)area.GetValue(ShowValueAsProbabilityProperty);
        }

        /// <summary>
        /// Sets the show value as probability.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/>.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowValueAsProbability(ChartArea area, bool value)
        {
            area.SetValue(ShowValueAsProbabilityProperty, value);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            int seriesIndex = series.Area.Series.IndexOf(series);

            bool isLower = seriesIndex == 0;
            bool isUpper = seriesIndex == series.Area.Series.Count - 1;

            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            ChartIndexedDataPoint[] StackPoints = points as ChartIndexedDataPoint[];

            for (int i = 0; i < points.Length; i++)
            {
                //if (points[i].DataPoint.EmptyPoint)
                //{
                //    points[i].DataPoint.Y = 0;
                //}


                double x1 = 0, x2 = 0;
                if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                {
                    x1 = points[i].DataPoint.X + sbsInfo.Start;
                    x2 = points[i].DataPoint.X + sbsInfo.End;
                }
                else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                {
                    var relativePos = points[i].DataPoint.Values.Length > 1 ? points[i].DataPoint.Values[1] * 0.5 : 0;
                    x1 = points[i].DataPoint.X - relativePos;
                    x2 = points[i].DataPoint.X + relativePos;
                }
                double x_m = points[i].DataPoint.X + sbsInfo.Median;
                DoubleRange yRange = series.Area.GetPercentageStackInfo(series, points[i]);
                ChartPoint cdpBottomLeft = new ChartPoint();
                ChartPoint cdpRightTop = new ChartPoint();
                if (points[i].DataPoint.EmptyPoint)
                {
                    cdpBottomLeft = new ChartPoint(x1, yRange.Start);
                    cdpRightTop = new ChartPoint(x2, yRange.End);
                }
                else
                {
                    cdpBottomLeft = new ChartPoint(x1, yRange.Start);
                    cdpRightTop = new ChartPoint(x2, yRange.End);
                }
                 ////If Emptypoint, make a difference in segment rendering
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            ChartPoint pp = new ChartPoint();
                            pp.X = x_m;
                            if (series.EmptyPointValue == EmptyPointValue.Zero)
                            {
                                pp.Y = cdpBottomLeft.Y;
                            }
                            else
                            {
                                pp.Y = cdpRightTop.Y;
                            } 
                            IChartDataPoint p = pp;
                            series.Segments.Add(new ChartEmptySymbolSegment(p, points[i], series, series.EmptyPointSymbolTemplate));
                            
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            ChartStackingColumn100Segment sector = new ChartStackingColumn100Segment(cdpBottomLeft, cdpRightTop, yRange.Delta, points[i], series);
                            sector.Interior = series.EmptyPointInterior;
                            //  StackPoints[i] = points[i];
                            // StackPoints[i].DataPoint.Y = cdpRightTop.Y;

                            series.Segments.Add(sector);

                        }
                        else
                        {
                            ChartPoint pp = new ChartPoint();
                            pp.X = x_m;
                            if (series.EmptyPointValue == EmptyPointValue.Zero)
                            {
                                pp.Y = cdpBottomLeft.Y;
                            }
                            else
                            {
                                pp.Y = cdpRightTop.Y;
                            }
                            IChartDataPoint p=pp;
                            
                             series.Segments.Add(new ChartEmptySymbolSegment(p, points[i], series, series.EmptyPointSymbolTemplate));
                            //series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                        }
                    }
                    else
                    {
                        ChartStackingColumn100Segment sector = new ChartStackingColumn100Segment(cdpBottomLeft, cdpRightTop, yRange.Delta, points[i], series);
                        sector.Interior = Brushes.Transparent;
                        sector.Stroke = Brushes.Transparent;
                        series.Segments.Add(sector);
                    }
                }
                else
                {
                    ChartStackingColumn100Segment sector = new ChartStackingColumn100Segment(cdpBottomLeft, cdpRightTop, yRange.Delta, points[i], series);
                    //  StackPoints[i] = points[i];
                    // StackPoints[i].DataPoint.Y = cdpRightTop.Y;

                    series.Segments.Add(sector);
                }
            }
            //if (series.ShowEmptyPoints)
            //{
            //    count = points.Length;
            //}
            //else
            //{
            //    count = series.Segments.Count;
            //}
            for (int i = 0; i < points.Length; i++)
            {

                DoubleRange yRange = series.Area.GetPercentageStackInfo(series, points[i]);
               // ChartSegment seg = series.Segments[i];
                if (series.AdornmentsInfo.Visible)
                {
                    double bottomPoint = (Math.Abs(yRange.Start) < Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double topPoint = (Math.Abs(yRange.Start) > Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double midPoint = yRange.Median;
                    if (yRange.Start < 0 || yRange.End < 0)
                        series.AdornmentsInfo.m_requiresSymmetricLabelling = true;
                          //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                AddAdornment(series, points[i], sbsInfo, yRange, bottomPoint);
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                AddAdornment(series, points[i], sbsInfo, yRange, topPoint);
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                AddAdornment(series, points[i], sbsInfo, yRange, midPoint);
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            AddAdornment(series, points[i], sbsInfo, yRange, bottomPoint);
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            AddAdornment(series, points[i], sbsInfo, yRange, topPoint);
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            AddAdornment(series, points[i], sbsInfo, yRange, midPoint);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the adornment.
        /// </summary>
        /// <param name="series">The target series.</param>
        /// <param name="point">The corresponding point.</param>
        /// <param name="sideBySideInfo">The side by side info.</param>
        /// <param name="percentageRange">The percentage range.</param>
        /// <param name="end"></param>
        /// <seealso cref="ChartStackingColumn100Type"/>
        protected virtual void AddAdornment(ChartSeries series, ChartIndexedDataPoint point, DoubleRange sideBySideInfo, DoubleRange percentageRange, double end)
        {
          double x = point.DataPoint.X + sideBySideInfo.Median;
          double y = percentageRange.Start + percentageRange.Delta / 2;

          series.Adornments.Add(new ChartAdornment(new ChartPoint(point.DataPoint.X, end), point, series, percentageRange));
        }

        /// <summary>
        /// Converts ChartColumnType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartStackingBar100Type"/>
        public override string ToString()
        {
            return "StackingColumn100";
        }
        #endregion
    }

    /// <summary>
    /// Represents stacking 100% column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartStackingColumn100Type"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStackingColumn100Segment : ChartColumnSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Percentage dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ChartStackingColumn100Segment), new UIPropertyMetadata(0d));
        #endregion

        #region Porperties
        /// <summary>
        /// Gets or sets the Percentage. This is a dependency property.
        /// </summary>
        /// <value>The Percentage.</value>
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStackingColumn100Segment"/> class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="percentage">The percentage.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        public ChartStackingColumn100Segment(IChartDataPoint bottomLeftPnt, IChartDataPoint topRightPnt, double percentage, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(bottomLeftPnt, topRightPnt, correspondingPoint, series)
        {
            this.Percentage = percentage;
        }
        #endregion
    }
}
