// <copyright file="ChartRangeColumnType.cs" company="Syncfusion">
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
    using System.Windows.Media;

    /// <summary>
    /// Represents ChartRangeColumnType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartRangeColumnType : ChartColumnType
    {
        #region Properties
        /// <summary>
        /// Gets the require data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 2;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            for (int i = 0; i < points.Length; i++)
            {
                double x1 = 0, x2 = 0;
                if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                {
                    x1 = points[i].DataPoint.X + sbsInfo.Start;
                    x2 = points[i].DataPoint.X + sbsInfo.End;
                }
                else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                {
                    var relativePos = points[i].DataPoint.Values.Length > 2 ? points[i].DataPoint.Values[2] * 0.5 : 0;
                    x1 = points[i].DataPoint.X - relativePos;
                    x2 = points[i].DataPoint.X + relativePos;
                }
                double y1 = points[i].DataPoint.Values[0];
                double y2 = points[i].DataPoint.Values[1];

                ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                if (points[i].DataPoint.EmptyPoint)
                {
                    double centerpoint1 = x1 + ((x2 - x1) / 2);
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate));
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                    }
                    else
                    {
                        series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                        series.Segments[i].Interior = new SolidColorBrush(Colors.Transparent);
                        series.Segments[i].Stroke = new SolidColorBrush(Colors.Transparent);
                    }
                }
                else
                    series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));

                   if (i == 0)
                    ChartType.SetColumnInitialSegmentWidthValue(series,x1);
                if (i == points.Length - 1)
                    ChartType.SetEndSegmentWidth(series, x2);
            }
            if (series.Segments.Count > points.Length)
            {
                for (int i = series.Segments.Count - points.Length; i < series.Segments.Count; )
                {
                    series.Segments.RemoveAt(i);
                }
            }
            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                            else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                     (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                        else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                 (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartRangeColumnType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            for (int i = 0; i < points.Length; i++)
            {
                double x1 = 0, x2 = 0;
                if (series.Segments.Count <= i)
                {
                   
                    if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                    {
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                    else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 2 ? points[i].DataPoint.Values[2] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];

                    ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                    ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        double centerpoint1 = x1 + ((x2 - x1) / 2);
                        if (series.ShowEmptyPoints)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                            {
                                series.Segments.Add(new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate));
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                if (series.EmptyPointValue == EmptyPointValue.Average)
                                {
                                    series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                                    series.Segments[i].Interior = series.EmptyPointInterior;
                                }
                                else
                                {
                                    series.Segments[i] = new ChartColumnSegment(new ChartPoint(0, 0), new ChartPoint(0, 0), points[i], series);
                                }
                            }
                            else
                            {
                                series.Segments.Add(new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate));
                                series.Segments[i].Interior = series.EmptyPointInterior;
                            }
                        }
                        else
                        {
                            series.Segments[i]=new ChartColumnSegment(new ChartPoint(centerpoint1, 0), new ChartPoint(centerpoint1, 0), points[i], series);
                        }
                    }
                    else
                        series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                }
                else if ((!(series.Segments[i].CorrespondingPoints[0].DataPoint.X == points[i].DataPoint.X
                            && series.Segments[i].CorrespondingPoints[0].DataPoint.Values[0] == points[i].DataPoint.Values[0] && series.Segments[i].CorrespondingPoints[0].DataPoint.Values[1] == points[i].DataPoint.Values[1])) || series.Area.AllowSegmentDragDrop)
                {
                   
                    if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                    {
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                    else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 2 ? points[i].DataPoint.Values[2] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    ChartPoint cdpBottomLeft;
                    ChartPoint cdpRightTop;
                    if (x1 < x2)
                    {
                        cdpBottomLeft = new ChartPoint(x1, y1);
                        cdpRightTop = new ChartPoint(x2, y2);
                    }
                    else
                    {
                        cdpBottomLeft = new ChartPoint(x1, y2);
                        cdpRightTop = new ChartPoint(x2, y1);
                    }
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        double centerpoint1 = x1 + ((x2 - x1) / 2);
                        if (series.ShowEmptyPoints)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                            {
                                series.Segments[i] = new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate);
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                if (series.EmptyPointValue == EmptyPointValue.Average)
                                {
                                    series.Segments[i] = new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                                    series.Segments[i].Interior = series.EmptyPointInterior;
                                }
                                else
                                {
                                    series.Segments[i] = new ChartColumnSegment(new ChartPoint(0, 0), new ChartPoint(0, 0), points[i], series);
                                }
                            }
                            else
                            {
                                series.Segments[i] = new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate);
                                series.Segments[i].Interior = series.EmptyPointInterior;
                            }
                        }
                        else
                        {
                            series.Segments[i]=new ChartColumnSegment(new ChartPoint(centerpoint1, 0), new ChartPoint(centerpoint1, 0), points[i], series);
                        }
                    }
                    else
                        series.Segments.Insert(i, new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                    if (series.Segments.Count < i)
                    {
                        series.Segments.RemoveAt(i + 1);
                    }
                }
                else
                {
                    //double x1 = 0, x2 = 0;
                    if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                    {
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                    else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 2 ? points[i].DataPoint.Values[2] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    ChartPoint cdpBottomLeft;
                    ChartPoint cdpRightTop;
                    if (x1 < x2)
                    {
                        cdpBottomLeft = new ChartPoint(x1, y1);
                        cdpRightTop = new ChartPoint(x2, y2);
                    }
                    else
                    {
                        cdpBottomLeft = new ChartPoint(x1, y2);
                        cdpRightTop = new ChartPoint(x2, y1);
                    }
                    double centerpoint1 = x1 + ((x2 - x1) / 2);
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                            {
                                series.Segments[i] = new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate);
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                if (series.EmptyPointValue == EmptyPointValue.Average)
                                {
                                    series.Segments[i] = new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                                    series.Segments[i].Interior = series.EmptyPointInterior;
                                }
                                else
                                {
                                    series.Segments[i] = new ChartColumnSegment(new ChartPoint(0, 0), new ChartPoint(0, 0), points[i], series);
                                }

                            }
                            else
                            {
                                series.Segments[i] = new ChartEmptySymbolSegment(new ChartPoint(centerpoint1, y1), points[i], series, series.EmptyPointSymbolTemplate);
                                series.Segments[i].Interior = series.EmptyPointInterior;
                            }
                        }
                        else
                        {
                            series.Segments[i] = new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                            series.Segments[i].Interior = new SolidColorBrush(Colors.Transparent);
                            series.Segments[i].Stroke = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                    else
                        series.Segments[i]=new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                }
            }

            if (series.Segments.Count > points.Length)
            {
                for (int i = series.Segments.Count - points.Length; i < series.Segments.Count; )
                {
                    series.Segments.RemoveAt(i);
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                if (series.EmptyPointValue == EmptyPointValue.Average)
                                {
                                    if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                                    {
                                        series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                        series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                    }
                                    else
                                    {
                                        series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                    }
                                }
                                else
                                {
                                    series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                }
                            }
                            else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                     (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                        else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                 (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Converts ChartColumnType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartRangeColumnType"/>
        public override string ToString()
        {
            return "RangeColumn";
        }
    }
}
