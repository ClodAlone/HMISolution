#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    internal class NumericalAxisHelper
    {
        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void GenerateVisibleLabels(ChartAxis axis, double smallTicksPerInterval)
        {
            DoubleRange range = axis.VisibleRange;
            double interval = axis.VisibleInterval;
            double position = range.Start;

            for (; position <= range.End; position += interval)
            {
                if (range.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetLabelContent(position), position));
                }
                if (axis.smallTicksRequired)
                {
                    var tickInterval = axis.VisibleInterval / (smallTicksPerInterval + 1);
                    var tickpos = position + tickInterval;
                    var end = axis.VisibleRange.End;
                    position += axis.VisibleInterval;
                    while (tickpos < position && tickpos <= end)
                    {
                        if (axis.VisibleRange.Inside(tickpos))
                        {
                            axis.m_smalltickPoints.Add(tickpos);
                        }
                        tickpos += tickInterval;
                    };
                }
            }

        }

        /// <summary>
        /// Called when [minimum maximum changed].
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="minimum">The minimum.</param>
        internal static void OnMinMaxChanged(ChartAxis axis, object maximum, object minimum)
        {
            if (minimum != null || maximum != null)
            {
#if NETFX_CORE
                double minimumValue= minimum == null? double.NegativeInfinity : Convert.ToDouble(minimum);
                double maximumValue= maximum == null ? double.PositiveInfinity : Convert.ToDouble(maximum);
                axis.ActualRange = new DoubleRange(minimumValue,maximumValue);
#else
                double minimumValue=minimum==null? double.NegativeInfinity: ((double?)minimum).Value;
                double maximumValue=maximum==null? double.PositiveInfinity:((double?)maximum).Value;
                axis.ActualRange = new DoubleRange(minimumValue,maximumValue);
#endif
            }
            else
            {
                axis.ActualRange = DoubleRange.Empty;
            }
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="range">The range.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="rangePadding">The range padding.</param>
        /// <returns></returns>
        internal static DoubleRange ApplyRangePadding(ChartAxis axis, DoubleRange range, double interval, NumericalPadding rangePadding, int rangePaddingFactor)
        {
            if (rangePadding == NumericalPadding.Normal)
            {
                double minimum = 0,
                       remaining,
                       start = range.Start;

                if (range.Start < 0)
                {
                    start = 0;
                    minimum = range.Start + (range.Start / 20);

                    remaining = interval + (minimum % interval);

                    if ((0.365 * interval) >= remaining)
                    {
                        minimum -= interval;
                    }

                    if (minimum % interval < 0)
                    {
                        minimum = (minimum - interval) - (minimum % interval);
                    }
                }
                else
                {
                    minimum = range.Start < ((5.0 / 6.0) * range.End)
                                         ? 0
                                         : (range.Start - (range.End - range.Start) / 2);
                    if (minimum % interval > 0)
                    {
                        minimum -= (minimum % interval);
                    }
                }

                double maximum = (range.End + (range.End - start) / 20);

                remaining = interval - (maximum % interval);

                if ((0.365 * interval) >= remaining)
                {
                    maximum += interval;
                }

                if (maximum % interval > 0)
                {
                    maximum = (maximum + interval) - (maximum % interval);
                }

                range = new DoubleRange(minimum, maximum);
                if (minimum == 0d)
                {

                    axis.ActualInterval = axis.CalculateActualInterval(range, axis.AvailableSize);
                    return new DoubleRange(0, Math.Ceiling(maximum / axis.ActualInterval) * axis.ActualInterval);
                }
            }
            else if (rangePadding == NumericalPadding.Round
                || rangePadding == NumericalPadding.Additional)
            {
                double minimum = Math.Floor(range.Start / interval) * interval;
                double maximum = Math.Ceiling(range.End / interval) * interval;

                if (rangePadding == NumericalPadding.Additional)
                {
                    minimum -= interval;
                    maximum += interval;
                }

                return new DoubleRange(minimum, maximum);
            }
            else if (rangePadding == NumericalPadding.FixedAdditional)
            {
                var fixedpadding = (range.End - range.Start) * rangePaddingFactor/100;
                //double minimum = Math.Floor(range.Start / fixedpadding) * fixedpadding;
                //double maximum = Math.Ceiling(range.End / fixedpadding) * fixedpadding;

                //minimum -= fixedpadding;
                //maximum += fixedpadding;

                return new DoubleRange(range.Start - fixedpadding, range.End + fixedpadding);
            }

            return range;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="avalableSize">Size of the avalable.</param>
        /// <param name="interval">The interval.</param>
        internal static void CalculateVisibleRange(ChartAxisBase2D axis, Size avalableSize, object interval)
        {
            if (axis.ZoomFactor < 1 || axis.ZoomPosition > 0)
            {
                if (interval != null)
                {
#if NETFX_CORE
                    double actualInterval = Convert.ToDouble(interval);
#else
                    double actualInterval = ((double?)interval).Value;
#endif
                    axis.VisibleInterval = axis.EnableAutoIntervalOnZooming
                                          ? axis.CalculateNiceInterval(axis.VisibleRange, avalableSize)
                                          : actualInterval;
                }

                else if (interval == null)
                {
                    axis.VisibleInterval = axis.CalculateNiceInterval(axis.VisibleRange, avalableSize);
                }
            }
        }
    }
}
