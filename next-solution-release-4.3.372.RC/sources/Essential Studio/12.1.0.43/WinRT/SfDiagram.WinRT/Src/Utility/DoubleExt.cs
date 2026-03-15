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
using System.Security;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks; 
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram
{
    public struct DoubleExt : IComparable, IComparable<DoubleExt>, IEquatable<DoubleExt>, IFormattable
    {
        private double _mMin;
        private double _mMax;
        private double _mValue;
        private double _mActualValue;

        public double Min
        {
            get { return _mMin; }
            set
            {
                _mMin = value;
                UpdateActualValue();
            }
        }
        public double Max
        {
            get { return _mMax; }
            set
            {
                _mMax = value;
                UpdateActualValue();
            }
        }
        public double Value
        {
            get { return _mValue; }
            set
            {
                _mValue = value;
                UpdateActualValue();
            }
        }
        public double ActualValue
        {
            get { return _mActualValue; }
            //private set { _mActualValue = value; }
        }
        private double _mDesiredValue;

        public double DesiredValue
        {
            get { return _mDesiredValue; }
            internal set
            {
                _mDesiredValue = value;
                UpdateActualValue();
            }
        }

        public DoubleExt(double min, double max, double value)
            : this()
        {
            Min = min;
            Max = max;
            Value = value;
        }

        public DoubleExt(double value)
            : this(double.MinValue, double.MaxValue, value)
        {
        }

        private void UpdateActualValue()
        {
            if (!_mValue.IsValid())
            {
                _mActualValue = DesiredValue;
                return;
            }
            double current = _mValue;
            double final = current;
            if (_mMin > current)
            {
                final = _mMin;
            }
            if (_mMax < current)
            {
                final = _mMax;
            }
            _mActualValue = final;
        }

        internal bool IsValid()
        {
            return Value.IsValid() || Min >= 0;
        }

        /// <summary>Compares this instance to a specified DoubleExt-precision floating-point number and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified DoubleExt-precision floating-point number.</summary>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="value" />.Return Value Description Less than zero This instance is less than <paramref name="value" />.-or- This instance is not a number (<see cref="F:DoubleExt.NaN" />) and <paramref name="value" /> is a number. Zero This instance is equal to <paramref name="value" />.-or- Both this instance and <paramref name="value" /> are not a number (<see cref="F:DoubleExt.NaN" />), <see cref="F:DoubleExt.PositiveInfinity" />, or <see cref="F:DoubleExt.NegativeInfinity" />. Greater than zero This instance is greater than <paramref name="value" />.-or- This instance is a number and <paramref name="value" /> is not a number (<see cref="F:DoubleExt.NaN" />). </returns>
        /// <param name="value">A DoubleExt-precision floating-point number to compare. </param>
        public int CompareTo(DoubleExt value)
        {
            return ActualValue.CompareTo(value.ActualValue);
        }
        /// <summary>Returns a value indicating whether this instance and a specified <see cref="T:DoubleExt" /> object represent the same value.</summary>
        /// <returns>true if <paramref name="obj" /> is equal to this instance; otherwise, false.</returns>
        /// <param name="obj">A <see cref="T:DoubleExt" /> object to compare to this instance.</param>
        public bool Equals(DoubleExt obj)
        {
            return ActualValue.Equals(obj.ActualValue);
        }
        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
        /// <returns>true if <paramref name="obj" /> is an instance of <see cref="T:DoubleExt" /> and equals the value of this instance; otherwise, false.</returns>
        /// <param name="obj">An object to compare with this instance. </param>
        public override bool Equals(object obj)
        {
            return ActualValue.Equals(obj);
        }
        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [SecuritySafeCritical]
        public override int GetHashCode()
        {
            return ActualValue.GetHashCode();
        }
        /// <summary>Returns a value indicating whether the specified number evaluates to negative or positive infinity </summary>
        /// <returns>true if <paramref name="d" /> evaluates to <see cref="F:DoubleExt.PositiveInfinity" /> or <see cref="F:DoubleExt.NegativeInfinity" />; otherwise, false.</returns>
        /// <param name="d">A DoubleExt-precision floating-point number. </param>
        [SecuritySafeCritical]
        public static bool IsInfinity(DoubleExt d)
        {
            return double.IsInfinity(d.ActualValue);
        }
        /// <summary>Returns a value that indicates whether the specified value is not a number (<see cref="F:DoubleExt.NaN" />).</summary>
        /// <returns>true if <paramref name="d" /> evaluates to <see cref="F:DoubleExt.NaN" />; otherwise, false.</returns>
        /// <param name="d">A DoubleExt-precision floating-point number. </param>
        [SecuritySafeCritical]
        public static bool IsNaN(DoubleExt d)
        {
            return double.IsNaN(d.ActualValue);
        }
        /// <summary>Returns a value indicating whether the specified number evaluates to negative infinity.</summary>
        /// <returns>true if <paramref name="d" /> evaluates to <see cref="F:DoubleExt.NegativeInfinity" />; otherwise, false.</returns>
        /// <param name="d">A DoubleExt-precision floating-point number. </param>
        public static bool IsNegativeInfinity(DoubleExt d)
        {
            return double.IsNegativeInfinity(d.ActualValue);
        }
        /// <summary>Returns a value indicating whether the specified number evaluates to positive infinity.</summary>
        /// <returns>true if <paramref name="d" /> evaluates to <see cref="F:DoubleExt.PositiveInfinity" />; otherwise, false.</returns>
        /// <param name="d">A DoubleExt-precision floating-point number. </param>
        public static bool IsPositiveInfinity(DoubleExt d)
        {
            return double.IsPositiveInfinity(d.ActualValue);
        }
        /// <summary>Returns a value that indicates whether two specified <see cref="T:DoubleExt" /> values are equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare. </param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue == right.ActualValue;
        }
        /// <summary>Returns a value that indicates whether a specified <see cref="T:DoubleExt" /> value is greater than another specified <see cref="T:DoubleExt" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue > right.ActualValue;
        }
        /// <summary>Returns a value that indicates whether a specified <see cref="T:DoubleExt" /> value is greater than or equal to another specified <see cref="T:DoubleExt" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue >= right.ActualValue;
        }
        /// <summary>Returns a value that indicates whether two specified <see cref="T:DoubleExt" /> values are not equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue != right.ActualValue;
        }
        /// <summary>Returns a value that indicates whether a specified <see cref="T:DoubleExt" /> value is less than another specified <see cref="T:DoubleExt" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue < right.ActualValue;
        }
        /// <summary>Returns a value that indicates whether a specified <see cref="T:DoubleExt" /> value is less than or equal to another specified <see cref="T:DoubleExt" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(DoubleExt left, DoubleExt right)
        {
            return left.ActualValue <= right.ActualValue;
        }

        public static DoubleExt operator +(DoubleExt left, double right)
        {
            return new DoubleExt(left.Min, left.Max, left.Value + right);
        }
        public static DoubleExt operator -(DoubleExt left, double right)
        {
            return new DoubleExt(left.Min, left.Max, left.Value - right);
        }
        public static DoubleExt operator *(DoubleExt left, double right)
        {
            return new DoubleExt(left.Min, left.Max, left.Value * right);
        }
        public static DoubleExt operator /(DoubleExt left, double right)
        {
            return new DoubleExt(left.Min, left.Max, left.Value / right);
        }
        public static DoubleExt operator %(DoubleExt left, double right)
        {
            return new DoubleExt(left.Min, left.Max, left.Value % right);
        }
        public static DoubleExt operator ++(DoubleExt left)
        {
            return new DoubleExt(left.Min, left.Max, left.Value + 1);
        }
        public static DoubleExt operator --(DoubleExt left)
        {
            return new DoubleExt(left.Min, left.Max, left.Value - 1);
        }
        public static bool operator ==(DoubleExt left, double right)
        {
            return left.Value == right;
        }

        public static bool operator !=(DoubleExt left, double right)
        {
            return left.Value != right;
        }

        public static bool operator >=(DoubleExt left, double right)
        {
            return left.Value >= right;
        }

        public static bool operator <=(DoubleExt left, double right)
        {
            return left.Value <= right;
        }

        public static bool operator >(DoubleExt left, double right)
        {
            return left.Value > right;
        }

        public static bool operator <(DoubleExt left, double right)
        {
            return left.Value < right;
        }

        public static bool operator true(DoubleExt left)
        {
            return left.ActualValue.IsValid();
        }

        public static bool operator false(DoubleExt left)
        {
            return left.ActualValue.IsValid();
        }

        public static implicit operator DoubleExt(double d)
        {
            return new DoubleExt(d);
        }

        //public static bool operator <=(DoubleExt left, double right)
        //{
        //    return left.Value <= right;
        //}
        /// <summary>Converts the string representation of a number to its DoubleExt-precision floating-point number equivalent.</summary>
        /// <returns>A DoubleExt-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s" />.</returns>
        /// <param name="s">A string that contains a number to convert. </param>
        /// <exception cref="T:ArgumentNullException">
        ///   <paramref name="s" /> is null. </exception>
        /// <exception cref="T:FormatException">
        ///   <paramref name="s" /> does not represent a number in a valid format. </exception>
        /// <exception cref="T:OverflowException">
        ///   <paramref name="s" /> represents a number that is less than <see cref="F:DoubleExt.MinValue" /> or greater than <see cref="F:DoubleExt.MaxValue" />. </exception>
        public static DoubleExt Parse(string s)
        {
            return new DoubleExt(double.Parse(s));
        }
        /// <summary>Converts the string representation of a number in a specified style to its DoubleExt-precision floating-point number equivalent.</summary>
        /// <returns>A DoubleExt-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s" />.</returns>
        /// <param name="s">A string that contains a number to convert. </param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in <paramref name="s" />. A typical value to specify is a combination of <see cref="F:Globalization.NumberStyles.Float" /> combined with <see cref="F:Globalization.NumberStyles.AllowThousands" />.</param>
        /// <exception cref="T:ArgumentNullException">
        ///   <paramref name="s" /> is null. </exception>
        /// <exception cref="T:FormatException">
        ///   <paramref name="s" /> does not represent a number in a valid format. </exception>
        /// <exception cref="T:OverflowException">
        ///   <paramref name="s" /> represents a number that is less than <see cref="F:DoubleExt.MinValue" /> or greater than <see cref="F:DoubleExt.MaxValue" />. </exception>
        /// <exception cref="T:ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:Globalization.NumberStyles" /> value. -or-<paramref name="style" /> includes the <see cref="F:Globalization.NumberStyles.AllowHexSpecifier" /> value. </exception>
        public static DoubleExt Parse(string s, NumberStyles style)
        {
            return new DoubleExt(double.Parse(s, style));
        }
        /// <summary>Converts the string representation of a number in a specified style and culture-specific format to its DoubleExt-precision floating-point number equivalent.</summary>
        /// <returns>A DoubleExt-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s" />.</returns>
        /// <param name="s">A string that contains a number to convert. </param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in <paramref name="s" />. A typical value to specify is <see cref="F:Globalization.NumberStyles.Float" /> combined with <see cref="F:Globalization.NumberStyles.AllowThousands" />.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s" />. </param>
        /// <exception cref="T:ArgumentNullException">
        ///   <paramref name="s" /> is null. </exception>
        /// <exception cref="T:FormatException">
        ///   <paramref name="s" /> does not represent a numeric value. </exception>
        /// <exception cref="T:ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:Globalization.NumberStyles" /> value. -or-<paramref name="style" /> is the <see cref="F:Globalization.NumberStyles.AllowHexSpecifier" /> value.</exception>
        /// <exception cref="T:OverflowException">
        ///   <paramref name="s" /> represents a number that is less than <see cref="F:DoubleExt.MinValue" /> or greater than <see cref="F:DoubleExt.MaxValue" />. </exception>
        public static DoubleExt Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            return new DoubleExt(double.Parse(s, provider));
        }
        /// <summary>Converts the string representation of a number in a specified culture-specific format to its DoubleExt-precision floating-point number equivalent.</summary>
        /// <returns>A DoubleExt-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s" />.</returns>
        /// <param name="s">A string that contains a number to convert. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s" />. </param>
        /// <exception cref="T:ArgumentNullException">
        ///   <paramref name="s" /> is null. </exception>
        /// <exception cref="T:FormatException">
        ///   <paramref name="s" /> does not represent a number in a valid format. </exception>
        /// <exception cref="T:OverflowException">
        ///   <paramref name="s" /> represents a number that is less than <see cref="F:DoubleExt.MinValue" /> or greater than <see cref="F:DoubleExt.MaxValue" />. </exception>
        public static DoubleExt Parse(string s, IFormatProvider provider)
        {
            return new DoubleExt(double.Parse(s, provider));
        }
        int IComparable.CompareTo(object value)
        {
            return ActualValue.CompareTo((double)value);
        }
        /// <summary>Converts the numeric value of this instance to its equivalent string representation.</summary>
        /// <returns>The string representation of the value of this instance.</returns>
        [SecuritySafeCritical]
        public override string ToString()
        {
            return ActualValue.ToString();
        }
        /// <summary>Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.</summary>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider" />.</returns>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        [SecuritySafeCritical]
        public string ToString(IFormatProvider provider)
        {
            return ActualValue.ToString(provider);
        }
        /// <summary>Converts the numeric value of this instance to its equivalent string representation, using the specified format.</summary>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format" />.</returns>
        /// <param name="format">A numeric format string.</param>
        /// <exception cref="T:FormatException">
        ///   <paramref name="format" /> is invalid. </exception>
        [SecuritySafeCritical]
        public string ToString(string format)
        {
            return ActualValue.ToString(format);
        }
        /// <summary>Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.</summary>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format" /> and <paramref name="provider" />.</returns>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        [SecuritySafeCritical]
        public string ToString(string format, IFormatProvider provider)
        {
            return ActualValue.ToString(format, provider);
        }
        /// <summary>Converts the string representation of a number to its DoubleExt-precision floating-point number equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <returns>true if <paramref name="s" /> was converted successfully; otherwise, false.</returns>
        /// <param name="s">A string containing a number to convert. </param>
        /// <param name="result">When this method returns, contains the DoubleExt-precision floating-point number equivalent to the <paramref name="s" /> parameter, if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s" /> parameter is null, is not a number in a valid format, or represents a number less than <see cref="F:DoubleExt.MinValue" /> or greater than <see cref="F:DoubleExt.MaxValue" />. This parameter is passed uninitialized. </param>
        public static bool TryParse(string s, out DoubleExt result)
        {
            double value = 0;
            bool ret = double.TryParse(s, out value);
            result = new DoubleExt(value);
            return ret;
        }
        /// <summary>Converts the string representation of a number in a specified style and culture-specific format to its DoubleExt-precision floating-point number equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <returns>true if <paramref name="s" /> was converted successfully; otherwise, false.</returns>
        /// <param name="s">A string containing a number to convert. </param>
        /// <param name="style">A bitwise combination of <see cref="T:Globalization.NumberStyles" /> values that indicates the permitted format of <paramref name="s" />. A typical value to specify is <see cref="F:Globalization.NumberStyles.Float" /> combined with <see cref="F:Globalization.NumberStyles.AllowThousands" />.</param>
        /// <param name="provider">An <see cref="T:IFormatProvider" /> that supplies culture-specific formatting information about <paramref name="s" />. </param>
        /// <param name="result">When this method returns, contains a DoubleExt-precision floating-point number equivalent to the numeric value or symbol contained in <paramref name="s" />, if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s" /> parameter is null, is not in a format compliant with <paramref name="style" />, represents a number less than <see cref="F:SByte.MinValue" /> or greater than <see cref="F:SByte.MaxValue" />, or if <paramref name="style" /> is not a valid combination of <see cref="T:Globalization.NumberStyles" /> enumerated constants. This parameter is passed uninitialized. </param>
        /// <exception cref="T:ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:Globalization.NumberStyles" /> value. -or-<paramref name="style" /> includes the <see cref="F:Globalization.NumberStyles.AllowHexSpecifier" /> value.</exception>
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out DoubleExt result)
        {
            double value = 0;
            bool ret = double.TryParse(s, style, provider, out value);
            result = new DoubleExt(value);
            return ret;
        }
    }
}
