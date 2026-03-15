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
    internal class LogarithmicAxisHelper
    {
        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal static double CalculateNiceInterval(ChartAxis axis, DoubleRange actualRange, Size availableSize)
        {
            double delta = actualRange.Delta;

            double actualDesiredIntervalsCount = axis.GetActualDesiredIntervalsCount(availableSize);

            double niceInterval = delta;

            double minInterval = Math.Pow(10, Math.Floor(Math.Log10(niceInterval)));

            foreach (int mul in ChartAxis.c_intervalDivs)
            {
                double currentInterval = minInterval * mul;
                if (actualDesiredIntervalsCount < (delta / currentInterval))
                {
                    break;
                }

                niceInterval = currentInterval;
            }

            return niceInterval;
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="actualInterval">The actual interval.</param>
        internal static void GenerateVisibleLabels(ChartAxis axis, object maximum, object minimum, object actualInterval, double logBase)
        {
            double interval = axis.VisibleInterval;

            double position;
            if (minimum != null && maximum != null && actualInterval != null)
                position = axis.VisibleRange.Start;
            else
                position = axis.VisibleRange.Start - (axis.VisibleRange.Start % axis.ActualInterval);

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetLabelContent(Math.Pow(logBase, position)), position));
                }
                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position, (axis as LogarithmicAxis).LogarithmicBase);
                }
            }
        }

        /// <summary>
        /// Method implementation for Add SmallTicks for axis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="position">The position.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void AddSmallTicksPoint(ChartAxis axis, double position, double logarithmicBase, double smallTicksPerInterval)
        {
            double logtickstart = Math.Pow(logarithmicBase, position - axis.VisibleInterval);
            double logtickend = Math.Pow(logarithmicBase, position);
            double logtickInterval = (logtickend - logtickstart) / (smallTicksPerInterval + 1);
            double logtickPos = logtickstart + logtickInterval;
            double logSmalltick = Math.Log(logtickPos, logarithmicBase);
            while (logtickPos < logtickend)
            {
                if (axis.VisibleRange.Inside(logSmalltick))
                {
                    axis.m_smalltickPoints.Add(logSmalltick);
                }
                logtickPos += logtickInterval;
                logSmalltick = Math.Log(logtickPos, logarithmicBase);
            }
        }

        /// <summary>
        /// Called when [minimum maximum changed].
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        internal static void OnMinMaxChanged(ChartAxis axis, object minimum, object maximum, double logarithmicBase)
        {
            if (minimum != null || maximum != null)
            {
#if NETFX_CORE
                double minimumValue = minimum == null ? double.NegativeInfinity : Convert.ToDouble(minimum);
                double maximumValue = maximum == null ? double.PositiveInfinity : Convert.ToDouble(maximum);
                axis.ActualRange = new DoubleRange(Math.Log(minimumValue, logarithmicBase), Math.Log(maximumValue, logarithmicBase));
#else
                double minimumValue=minimum==null ? double.NegativeInfinity : ((double?)minimum).Value;
                double maximumValue=maximum==null ? double.PositiveInfinity : ((double?)maximum).Value;
                axis.ActualRange = new DoubleRange(Math.Log(minimumValue, logarithmicBase), Math.Log(maximumValue, logarithmicBase));
#endif
            }
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="range">The range.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        /// <returns></returns>
        internal static DoubleRange CalculateActualRange(ChartAxis axis, DoubleRange range, double logarithmicBase)
        {
            double logStart = Math.Log(range.Start, logarithmicBase);
            logStart = double.IsInfinity(logStart) ? range.Start : logStart;
            double logEnd = Math.Log(range.End, logarithmicBase);
            logEnd = double.IsInfinity(logEnd) ? logarithmicBase : logEnd;

            double mulS = ChartMath.Round(logStart, 1, false);
            double mulE = ChartMath.Round(logEnd, 1, true);
            range = new DoubleRange(mulS, mulE);

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
                    axis.VisibleInterval = axis.EnableAutoIntervalOnZooming
                                            ? axis.CalculateNiceInterval(axis.VisibleRange, avalableSize)
                                            : axis.ActualInterval;
                }
                else
                {
                    axis.VisibleInterval = axis.CalculateNiceInterval(axis.VisibleRange, avalableSize);
                }
            }
        }

    }
}
