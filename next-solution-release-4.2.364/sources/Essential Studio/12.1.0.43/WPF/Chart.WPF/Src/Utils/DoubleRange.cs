// <copyright file="DoubleRange.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Represents DoubleRangeConverter 
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DoubleRangeConverter : TypeConverter
    {
        #region Constants
        /// <summary>
        /// Initializes c_separator
        /// </summary>
        private const string C_separator = ",";
        #endregion

        #region Public methods
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <seealso cref="DoubleRangeConverter"/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <seealso cref="DoubleRangeConverter"/>        
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <seealso cref="DoubleRangeConverter"/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            DoubleRange result = DoubleRange.Empty;

            if (text.Length == 0)
                return value;

            if (text != null && text.Length != 0)
            {
                string[] textValues = text.Split(new string[] { C_separator }, StringSplitOptions.RemoveEmptyEntries);
                double[] doubleValues = new double[textValues.Length];

                for (int i = 0; i < doubleValues.Length; i++)
                {
                    doubleValues[i] = Convert.ToDouble(textValues[i].Trim(' '), CultureInfo.InvariantCulture);
                }

                if (doubleValues.Length == 2)
                {
                    result = new DoubleRange(doubleValues[0], doubleValues[1]);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType == typeof(string) && value is DoubleRange)
            {
                DoubleRange range = (DoubleRange)value;
                string[] textValues = new string[] { range.Start.ToString(CultureInfo.InvariantCulture), range.End.ToString(CultureInfo.InvariantCulture) };
                result = string.Join(C_separator, textValues);
            }

            return result != null ? result : base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }

    /// <summary>
    /// Represents DateTimeRangeConverter
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DateTimeRangeConverter : TypeConverter
    {
        #region Constants
        /// <summary>
        /// Initializes c_separator
        /// </summary>
        private const string C_separator = ",";
        #endregion

        #region Public methods
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <seealso cref="DateTimeRangeConverter"/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <seealso cref="DateTimeRangeConverter"/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <seealso cref="DateTimeRangeConverter"/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            DateTimeRange result = new DateTimeRange();

            if (text != null && text.Length != 0)
            {
                string[] textValues = text.Split(new string[] { C_separator }, StringSplitOptions.RemoveEmptyEntries);
                DateTime[] dateTimeValues = new DateTime[textValues.Length];

                for (int i = 0; i < dateTimeValues.Length; i++)
                {
                    dateTimeValues[i] = Convert.ToDateTime(textValues[i].Trim(' '), CultureInfo.InvariantCulture);
                }

                if (dateTimeValues.Length == 2)
                {
                    result = new DateTimeRange(dateTimeValues[0], dateTimeValues[1]);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        /// <seealso cref="DateTimeRangeConverter"/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType == typeof(string) && value is DateTimeRange)
            {
                DateTimeRange range = (DateTimeRange)value;
                string[] textValues = new string[] { range.Start.ToString(CultureInfo.InvariantCulture), range.End.ToString(CultureInfo.InvariantCulture) };

                result = string.Join(C_separator, textValues);
            }

            return result != null ? result : base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }

    /// <summary>
    /// Represents the DoubleRange structure
    /// </summary>
    [TypeConverter(typeof(DoubleRangeConverter))]
    public struct DoubleRange
    {
        #region Members
        /// <summary>
        /// Initializes c_empty
        /// </summary>
        private static readonly DoubleRange c_empty = new DoubleRange(double.NaN, double.NaN);
        private bool m_isempty;

        /// <summary>
        /// Initializes m_start
        /// </summary>
        private double m_start;

        /// <summary>
        /// Initializes m_end
        /// </summary>
        private double m_end;

        private static bool isAdditional=false;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the Empty value
        /// </summary>
        public static DoubleRange Empty
        {
            get
            {
                return c_empty;
            }
        }

        /// <summary>
        /// Gets the Start value
        /// </summary>
        public double Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the End value
        /// </summary>
        public double End
        {
            get
            {
                return m_end;
            }
        }

        /// <summary>
        /// Gets the Delta value
        /// </summary>
        public double Delta
        {
            get
            {
                return m_end - m_start;
            }
        }

        internal bool IsAdditional
        {
            get
            {
                return isAdditional;
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
        /// Gets a value indicating whether IsEmpty 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return m_isempty;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> struct.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        public DoubleRange(double start, double end)
        {
            if (!double.IsNaN(start) && !double.IsNaN(end))
            {
                this.m_isempty = false;
            }
            else
            {
                this.m_isempty = true;
            }

            if (start > end)
            {
                m_start = end;
                m_end = start;
                isAdditional = true;
            }
            else
            {
                m_start = start;
                m_end = end;
                isAdditional = false;
            }
        }
        /// <summary>
        /// DoubleRange method implementation for three arguments.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isSort"></param>
        public DoubleRange(double start, double end, bool isSort)
        {
            if (!double.IsNaN(start) && !double.IsNaN(end))
            {
                this.m_isempty = false;
            }
            else
            {
                this.m_isempty = true;
            }

            //if (start > end && isSort)
            //{
            //    m_start = end;
            //    m_end = start;
            //}
            //else
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
        /// <returns>The Union value</returns>
        public static DoubleRange operator +(DoubleRange leftRange, DoubleRange rightRange)
        {
            return Union(leftRange, rightRange);
        }

        /// <summary>
        /// Union operator
        /// </summary>
        /// <param name="range">First double range</param>
        /// <param name="value">Second double range</param>
        /// <returns>The Union value</returns>
        public static DoubleRange operator +(DoubleRange range, double value)
        {
            return Union(range, value);
        }

        /// <summary>
        /// The operator
        /// </summary>
        /// <param name="range">The DoubleRange </param>
        /// <param name="value">The double value</param>
        /// <returns>The range value</returns>
        public static bool operator >(DoubleRange range, double value)
        {
            return range.m_start > value;
        }

        /// <summary>
        /// Return Bool value based upon the DoubleRange value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator >(DoubleRange range, DoubleRange value)
        {
            return range.m_start > value.m_start && range.m_end > value.m_end;
        }

        /// <summary>
        /// Return Bool value based upon the DoubleRange value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator <(DoubleRange range, DoubleRange value)
        {
            return range.m_start < value.m_start && range.m_end < value.m_end;
        }


        /// <summary>
        /// The operator
        /// </summary>
        /// <param name="range">The DoubleRange </param>
        /// <param name="value">The double value</param>
        /// <returns>The range value</returns>
        public static bool operator <(DoubleRange range, double value)
        {
            return range.m_end < value;
        }

        /// <summary>
        /// The operator method
        /// </summary>
        /// <param name="leftRange">The left DoubleRange</param>
        /// <param name="rightRange">The right DoubleRange</param>
        /// <returns>The left range</returns>
        public static bool operator ==(DoubleRange leftRange, DoubleRange rightRange)
        {
            return leftRange.Equals(rightRange);
        }

        /// <summary>
        /// The operator method
        /// </summary>
        /// <param name="leftRange">The left range</param>
        /// <param name="rightRange">The right range</param>
        /// <returns>The inverse left range</returns>
        public static bool operator !=(DoubleRange leftRange, DoubleRange rightRange)
        {
            return !leftRange.Equals(rightRange);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Create range by array of double.
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Union(double[] values)
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (double val in values)
            {
                if (min > val)
                {
                    min = val;
                }

                if (max < val)
                {
                    max = val;
                }
            }

            return new DoubleRange(min, max);
        }

        /// <summary>
        /// Unions the specified left range with right range.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>The DoubleRange</returns>
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

            return new DoubleRange(Math.Min(leftRange.m_start, rightRange.m_start), Math.Max(leftRange.m_end, rightRange.m_end));
        }

        /// <summary>
        /// Unions the specified range with value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The DoubleRange</returns>
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
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Scale(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(range.m_start - value * range.Delta, range.m_end + value * range.Delta);
        }

        /// <summary>
        /// Offsets the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Offset(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(range.m_start + value, range.m_end + value);
        }

        /// <summary>
        /// Excludes the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="excluder">The excluder.</param>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>True if empty</returns>
        public static bool Exclude(DoubleRange range, DoubleRange excluder, out DoubleRange leftRange, out DoubleRange rightRange)
        {
            leftRange = DoubleRange.Empty;
            rightRange = DoubleRange.Empty;

            if (!(range.IsEmpty || excluder.IsEmpty))
            {
                if (excluder.m_end < range.m_start)
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
        /// <param name="range">the DoubleRange</param>
        /// <returns><b>true</b> if  intersection is not empty</returns>
        public bool Intersects(DoubleRange range)
        {
            if (this.IsEmpty || this.IsEmpty)
            {
                return false;
            }

            return this.Inside(range.m_start) || this.Inside(range.m_end) || range.Inside(this.m_start) || range.Inside(this.m_end);
        }

        /// <summary>
        /// Checks whether intersection region of two ranges is not empty.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <returns> true if  intersection is not empty</returns>
        public bool Intersects(double start, double end)
        {
            return this.Intersects(new DoubleRange(start, end));
        }

        /// <summary>
        /// Checks whether the given value is inside the axis range
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if value is inside</returns>
        public bool Inside(double value)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return (value <= m_end) && (value >= m_start);
        }

        /// <summary>
        /// Checks whether the given range is inside the axis range
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>True is range is inside</returns>
        public bool Inside(DoubleRange range)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return m_start <= range.m_start && m_end >= range.m_end;
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
                return (m_start == range.m_start) && (m_end == range.m_end);
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

    /// <summary>
    /// Ranging DataTime structure.
    /// </summary>
    [TypeConverter(typeof(DateTimeRangeConverter))]
    public struct DateTimeRange
    {
        #region Members
        /// <summary>
        /// Initilaizes m_start
        /// </summary>
        private DateTime m_start;

        /// <summary>
        /// Initilaizes m_end
        /// </summary>
        private DateTime m_end;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange"/> struct.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        public DateTimeRange(DateTime rangeStart, DateTime rangeEnd)
        {
            m_start = rangeStart;
            m_end = rangeEnd;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return m_end <= m_start;
            }
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        public DateTime Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end value.</value>
        public DateTime End
        {
            get
            {
                return m_end;
            }
        }
        #endregion
    }
}