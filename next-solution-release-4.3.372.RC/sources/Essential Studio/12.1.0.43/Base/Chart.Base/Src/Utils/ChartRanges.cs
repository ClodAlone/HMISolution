#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Closed range.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public struct DoubleRange
    {
        #region Members
        private double m_start;
        private double m_end;
        private readonly static DoubleRange c_empty = new DoubleRange(double.NaN, double.NaN);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end.</value>
        public double End
        {
            get
            {
                return m_end;
            }
        }

        /// <summary>
        /// Gets the delta.
        /// </summary>
        /// <value>The delta.</value>
        public double Delta
        {
            get
            {
                return m_end - m_start;
            }
        }

        /// <summary>
        /// Gets the median.
        /// </summary>
        /// <value>The median.</value>
        public double Median
        {
            get
            {
                return (m_start + m_end) / 2d;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return double.IsNaN(m_start) || double.IsNaN(m_end);
            }
        }

        /// <summary>
        /// Gets the empty.
        /// </summary>
        /// <value>The empty.</value>
        public static DoubleRange Empty
        {
            get
            {
                return c_empty;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public DoubleRange(double start, double end)
        {
            if (start > end)
            {
                m_start = end;
                m_end = start;
            }
            else
            {
                m_start = start;
                m_end = end;
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Union operator
        /// </summary>
        /// <param name="leftRange">First double range</param>
        /// <param name="rightRange">Second double range</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange operator +(DoubleRange leftRange, DoubleRange rightRange)
        {
            return Union(leftRange, rightRange);
        }

        /// <summary>
        /// Union operator
        /// </summary>
        /// <param name="range">First double range</param>
        /// <param name="value">Second double range</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange operator +(DoubleRange range, double value)
        {
            return Union(range, value);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(DoubleRange range, double value)
        {
            return range.m_start > value;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(DoubleRange range, double value)
        {
            return range.m_end < value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(DoubleRange range, double value)
        {
            return range.m_start >= value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(DoubleRange range, double value)
        {
            return range.m_end <= value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DoubleRange leftRange, DoubleRange rightRange)
        {
            return leftRange.Equals(rightRange);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DoubleRange leftRange, DoubleRange rightRange)
        {
            return !leftRange.Equals(rightRange);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Create the <see cref="DoubleRange"/> from the median.
        /// </summary>
        /// <param name="median">The median.</param>
        /// <param name="size">The size.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange FromMedian(double median, double size)
        {
            return new DoubleRange(median - 0.5 * size, median + 0.5 * size);
        }

        /// <summary>
        /// Create range by array of double.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Union(double[] values)
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (double val in values)
            {
                if (min > val) min = val;
                if (max < val) max = val;
            }

            return new DoubleRange(min, max);
        }

        /// <summary>
        /// Unions the specified left range with right range.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Union(DoubleRange leftRange, DoubleRange rightRange)
        {
            if (leftRange.IsEmpty)
            {
                return rightRange;
            }
            else if (rightRange.IsEmpty)
            {
                return leftRange;
            }

            return new DoubleRange(Math.Min(leftRange.m_start, rightRange.m_start),
                Math.Max(leftRange.m_end, rightRange.m_end));
        }

        /// <summary>
        /// Unions the specified range with value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Union(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return new DoubleRange(value, value);
            }

            return new DoubleRange(Math.Min(range.m_start, value), Math.Max(range.m_end, value));
        }

        /// <summary>
        /// Scales the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Scale(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            double radius = 0.5 * value * range.Delta;
            double median = range.Median;

            return new DoubleRange(median - radius, median + radius);
        }

        /// <summary>
        /// Multiplies the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Multiply(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(value * range.m_start, value * range.m_end);
        }

        /// <summary>
        /// Inflates the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Inflate(DoubleRange range, double value)
        {
            return new DoubleRange(range.m_start - value, range.m_end + value);
        }

        /// <summary>
        /// Offsets the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Offset(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(range.m_start + value, range.m_end + value);
        }

        /// <summary>
        /// Intersects the specified left range.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>Returns DoubleRange.</returns>
        public static DoubleRange Intersect(DoubleRange leftRange, DoubleRange rightRange)
        {
            if (leftRange.IsIntersects(rightRange))
            {
                return new DoubleRange(Math.Max(leftRange.m_start, rightRange.m_start), Math.Min(leftRange.m_end, rightRange.m_end));
            }

            return DoubleRange.c_empty;
        }

        /// <summary>
        /// Excludes the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="excluder">The excluder.</param>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns></returns>
        public static bool Exclude(DoubleRange range, DoubleRange excluder, out DoubleRange leftRange, out DoubleRange rightRange)
        {
            leftRange = DoubleRange.Empty;
            rightRange = DoubleRange.Empty;

            if (!(range.IsEmpty || excluder.IsEmpty))
            {
                if (excluder.m_start < range.m_start)
                {
                    if (excluder.m_end > range.m_start)
                    {
                        leftRange = new DoubleRange(excluder.m_start, range.m_start);
                    }
                    else
                    {
                        leftRange = excluder;
                    }
                }

                if (excluder.m_end > range.m_end)
                {
                    if (excluder.m_start < range.m_end)
                    {
                        rightRange = new DoubleRange(range.m_end, excluder.m_end);
                    }
                    else
                    {
                        rightRange = excluder;
                    }
                }
            }

            return !(leftRange.IsEmpty && rightRange.IsEmpty);
        }

        /// <summary>
        /// Checks whether intersection region of two ranges is not empty.
        /// </summary>
        /// <param name="range"></param>
        /// <returns> <b>true</b> if  intersection is not empty</returns>
        public bool IsIntersects(DoubleRange range)
        {
            if (this.IsEmpty || this.IsEmpty)
                return false;

            return (range.m_start <= m_end) && (range.m_end >= m_start);
        }

        /// <summary>
        /// Checks whether intersection region of two ranges is not empty.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns> true if  intersection is not empty</returns>
        public bool IsIntersects(double start, double end)
        {
            return this.IsIntersects(new DoubleRange(start, end));
        }

        /// <summary>
        /// Check the value whether it lies inside the end value or not.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the ChartRanges is not Empty otherwise False.</returns>
        public bool Inside(double value)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return (value <= m_end) && (value >= m_start);
        }

        /// <summary>
        /// Insides the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="equal">if set to <c>true</c> value can be equal with range.</param>
        /// <returns>True if the ChartRanges is not Empty otherwise False.</returns>
        public bool Inside(double value, bool equal)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            if (equal)
            {
                return (value <= m_end) && (value >= m_start);
            }

            return (value < m_end) && (value > m_start);
        }

        /// <summary>
        /// Insides the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>True if the ChartRanges is not Empty otherwise False.</returns>
        public bool Inside(DoubleRange range)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return m_start <= range.m_start && m_end >= range.m_end;
        }

        /// <summary>
        /// Interpolates the specified value.
        /// </summary>
        /// <param name="interpolator">The interpolator.</param>
        /// <returns>Returns Double.</returns>
        public double Interpolate(double interpolator)
        {
            return m_start + interpolator * (m_end - m_start);
        }

        /// <summary>
        /// Extrapolates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns Double.</returns>
        public double Extrapolate(double value)
        {
            return (value - m_start) / (m_end - m_start);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is DoubleRange)
            {
                DoubleRange range = (DoubleRange)obj;
                return ((m_start == range.m_start) && (m_end == range.m_end));
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return m_start.GetHashCode() ^ m_end.GetHashCode();
        }
        #endregion
    }
}