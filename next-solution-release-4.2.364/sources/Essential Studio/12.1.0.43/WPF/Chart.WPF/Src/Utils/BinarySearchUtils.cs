// <copyright file="BinarySearchUtils.cs" company="Syncfusion">t
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents BinarySearchUtils
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal static class BinarySearchUtils
    {
        /// <summary>
        /// Gets the index range by real range.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="range">The range.</param>
        /// <returns>The ChartIndexRange</returns>
        public static ChartIndexRange GetIndexRangeByRealRange(IList<IChartDataPoint> points, DoubleRange range)
        {
            int s = 0;
            int e = points.Count - 1;

            for (int i = 0; i < points.Count; i++)
            {
                if (range.Inside(points[i].X))
                {
                    s = i;
                    break;
                }
            }

            for (int i = points.Count - 1; i > 0; i--)
            {
                if (range.Inside(points[i].X))
                {
                    e = i;
                    break;
                }
            }

            return new ChartIndexRange(s, e, 1);
        }
        
        /// <summary>
        /// Serializes the collections.
        /// </summary>
        /// <typeparam name="T">The type param</typeparam>
        /// <param name="xList">The x list.</param>
        /// <param name="yList">The y list.</param>
        public static void SerializeCollection<T>(IList<T> xList, IList<T> yList)
        {
            for (int xi = xList.Count - 1; xi > -1; xi--)
            {
                if (!yList.Contains(xList[xi]))
                {
                    xList.RemoveAt(xi);
                }
            }

            for (int yi = yList.Count - 1; yi > -1; yi--)
            {
                if (!xList.Contains(yList[yi]))
                {
                    xList.Add(yList[yi]);
                }
            }
        }

        /// <summary>
        /// Gets the closest, smallest and equal value to specified value.
        /// </summary>
        /// <param name="sortedList">The sorted list.</param>
        /// <param name="from"> The From value.</param>
        /// <param name="to">The To value.</param>
        /// <param name="value">The value.</param>
        /// <param name="drawing">The drawing.</param>
        /// <returns>The Closest Smallest Equal Value</returns>
        private static int GetClosestSmallestEqualToValue(IList<ChartSegment> sortedList, int from, int to, double value, out ChartSegment drawing)
        {
            drawing = null;

            if (sortedList != null && sortedList.Count != 0)
            {
                for (int i = (to + from) / 2;; i = (to + from) / 2)
                {
                    ChartSegment mCid = sortedList[i];

                    if (value <= sortedList[from].XDataMeasure.Start)
                    {
                        drawing = sortedList[from];
                        return from;
                    }

                    if (value >= sortedList[to].XDataMeasure.End)
                    {
                        drawing = sortedList[to];
                        return to;
                    }

                    if (value < mCid.XDataMeasure.Start)
                    {
                        to = i - 1;
                    }
                    else if (value > mCid.XDataMeasure.End)
                    {
                        from = i + 1;
                    }
                    else
                    {
                        drawing = sortedList[i];
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}