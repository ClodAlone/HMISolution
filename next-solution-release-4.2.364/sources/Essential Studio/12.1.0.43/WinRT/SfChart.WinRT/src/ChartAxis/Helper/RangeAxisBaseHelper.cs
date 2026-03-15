#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Charts
{
    internal class RangeAxisBaseHelper
    {
        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="position">The position.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void AddSmallTicksPoint(ChartAxis axis, double position, double interval, double smallTicksPerInterval)
        {
            var tickInterval = interval / (smallTicksPerInterval + 1);
            var tickpos = position + tickInterval;
            var end = axis.VisibleRange.End;
            position += interval;
            while (tickpos < position && tickpos <= end)
            {
                if (axis.VisibleRange.Inside(tickpos))
                {
                    axis.m_smalltickPoints.Add(tickpos);
                }
                tickpos += tickInterval;
            };
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void GenerateVisibleLabels(ChartAxis axis, double smallTicksPerInterval)
        {
            double interval = axis.VisibleInterval;
            double position = axis.VisibleRange.Start - (axis.VisibleRange.Start % interval);

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetLabelContent(position), position));
                }
                if (axis.smallTicksRequired)
                {
                   axis.AddSmallTicksPoint(position);
                }
            }

        }

    }
}
