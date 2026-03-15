#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    internal class CategoryAxisHelper
    {
        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <param name="labelPlacement"></param>
        /// <returns></returns>
        internal static DoubleRange ApplyRangePadding(ChartAxis axis, DoubleRange range, double interval, LabelPlacement labelPlacement)
        {
            var actualSeries =
                axis.Area.VisibleSeries
                .Where(series => series.ActualXAxis == axis)
                .Max(filteredSeries => filteredSeries.DataCount);
            if (!(actualSeries is PolarRadarSeriesBase) && labelPlacement == LabelPlacement.BetweenTicks)
            {
                return new DoubleRange(-0.5, (int)range.End + 0.5);
            }
            return range;
        }

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static object GetLabelContent(ChartAxis axis, double position)
        {
            ChartSeriesBase actualSeries =
                axis.Area.VisibleSeries
                .Where(series => series.ActualXAxis == axis)
                .Max(filteredSeries => filteredSeries.DataCount);

            if (actualSeries != null)
            {
                return GetLabelContent(axis, (int)Math.Round(position), actualSeries) ?? string.Empty;
            }

            return position;
        }

        internal static object GetLabelContent(ChartAxis axis, int pos, ChartSeriesBase actualSeries)
        {
            if (actualSeries != null)
            {
                var xValues = actualSeries.ActualXValues as List<double>;

                if (xValues != null && pos < xValues.Count && pos >= 0)
                {
                    switch (actualSeries.XAxisValueType)
                    {
                        case ChartValueType.DateTime:
                            {
                                DateTime xDateTime = xValues[pos].FromOADate();
                                return xDateTime.ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }
                        case ChartValueType.TimeSpan:
                            {
                                TimeSpan xTimeSpanValue = TimeSpan.FromMilliseconds(xValues[pos]);
                                return xTimeSpanValue.ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }

                        case ChartValueType.Double:
                        case ChartValueType.Logarithmic:
                            {
                                return xValues[pos].ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }
                    }
                }
                else
                {
                    var xStrValues = actualSeries.ActualXValues as List<string>;
                    if (xStrValues != null && pos < xStrValues.Count && pos >= 0)
                        return xStrValues[pos];
                }
            }

            return pos;
        }

        /// <summary>
        /// Method implementation for Generate Visiblie labels for CategoryAxis
        /// </summary>
        internal static void GenerateVisibleLabels(ChartAxis axis, LabelPlacement labelPlacement)
        {
            var actualSeries =
                 axis.Area.VisibleSeries
                .Where(series => series.ActualXAxis == axis)
                .Max(filteredSeries => filteredSeries.DataCount);

            if (actualSeries == null) return;
            var visibleRange = axis.VisibleRange;
            double actualInterval = axis.ActualInterval;
            double interval = axis.VisibleInterval;
            double position = visibleRange.Start - (visibleRange.Start % actualInterval);

            for (; position <= visibleRange.End; position += interval)
            {
                if (visibleRange.Inside(position) && position < actualSeries.DataCount && position >-1)
                {
                    int pos = ((int)Math.Round(position));
                    object obj = GetLabelContent(axis, pos, actualSeries);
                    axis.VisibleLabels.Add(new ChartAxisLabel(pos, obj, pos));
                }
            }
            position = visibleRange.Start - (visibleRange.Start % actualInterval);
            if (actualSeries is PolarRadarSeriesBase) return;
            for (; position <= visibleRange.End; position += 1)
            {
                if (labelPlacement != LabelPlacement.BetweenTicks) continue;
                if (position == 0)
                    axis.m_smalltickPoints.Add(-0.5);
                AddBetweenTicks(axis, position, 1d);
            }
        }

        internal static void AddBetweenTicks(ChartAxis axis, double position, double interval)
        {
            var tickInterval = interval / 2;
            var tickpos = position + tickInterval;
            var end = axis.VisibleRange.End;
            position += 1;
            while (tickpos < position && tickpos <= end)
            {
                if (axis.VisibleRange.Inside(tickpos))
                {
                    axis.m_smalltickPoints.Add(tickpos);
                }
                tickpos += tickInterval;
            }
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        internal static double CalculateActualInterval(ChartAxis axis, DoubleRange range, Size availableSize, object interval)
        {
            if (interval == null)
                return Math.Max(1d, Math.Floor(range.Delta / axis.GetActualDesiredIntervalsCount(availableSize)));

#if NETFX_CORE
            return Convert.ToDouble(interval);
#else
            return ((double?)interval).Value;
#endif
        }
    }
}
