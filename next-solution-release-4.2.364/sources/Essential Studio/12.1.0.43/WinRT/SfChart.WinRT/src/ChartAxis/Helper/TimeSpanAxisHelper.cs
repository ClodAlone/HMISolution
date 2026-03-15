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
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    internal class TimeSpanAxisHelper
    {
        /// <summary>
        /// Generates the visible labels.
        /// </summary>
        /// <param name="axis">The axis.</param>
        internal static void GenerateVisibleLabels(ChartAxis axis)
        {
            double interval = axis.VisibleInterval;
            double position = axis.VisibleRange.Start;

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

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="avalableSize">Size of the avalable.</param>
        internal static void CalculateVisibleRange(ChartAxisBase2D axis, object interval, Size avalableSize)
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
