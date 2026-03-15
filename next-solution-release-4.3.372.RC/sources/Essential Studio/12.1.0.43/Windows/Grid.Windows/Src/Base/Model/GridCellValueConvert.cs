//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellValueConvert.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// <see cref="GridCellValueConvert"/> provides conversion routines for cell values
    /// to convert them to another type and routines for formatting cell values.
    /// </summary>
    public sealed class GridCellValueConvert : Syncfusion.Styles.ValueConvert
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCellValueConvert()
            : base()
        {
        }
    }

#if obsolete // Moved to shared.base, styleinfostore.cs, class ValueConvert
    /// <summary>
    /// <see cref="GridCellValueConvert"/> provides conversion routines for cell values
    /// to convert them to another type and routines for formatting cell values.
    /// </summary>
    public sealed class GridCellValueConvert
    {
        GridCellValueConvert()
        {
        }

        /// <overload>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </overload>
        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider)
        {
            return ChangeType(value, type, provider, false);
        }
        
        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <param name="returnDbNUllIfNotValid">Set this true if exceptions should be avoided or catched and return value should be DBNull if
        /// it can not be converted to the target type.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider, bool returnDbNUllIfNotValid)
        {
            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                try
                {
                    if (value is string)
                        value = Parse((string) value, type, provider, "", returnDbNUllIfNotValid);
                    else if (value is System.DBNull)
                        value = null;
                    else if (type.IsEnum)
                    {
                        value = Convert.ChangeType(value, typeof(int), provider);
                        value = Enum.ToObject(type, (int) value);
                    }
                    else if (type == typeof(string) && !(value is IConvertible))
                    {
                        value = value != null ? value.ToString() : "";
                    }
                    else
                        value = Convert.ChangeType(value, type, provider);
                }
                catch (Exception ex)
                {
                    if (returnDbNUllIfNotValid)
                        return Convert.DBNull;

                    throw;
                }
            }

            if (value == null && type == typeof(string))
                return "";

            return value;
        }

        /// <summary>
        /// Cache for static parse method for each known type.
        /// </summary>
        static Hashtable cachedParseMethods = new Hashtable();
        static Hashtable cachedDefaultValues = new Hashtable();

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <returns>The new value in the target type.</returns>
        static object Parse(string s, Type resultType, IFormatProvider provider)
        {
             return Parse(s, resultType, provider, "");
        }

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="format">A format string used in a <see cref="System.Object.ToString()"/> call. Right now
        /// format is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format)
        {
            return Parse(s, resultType, provider, format, false);
        }

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="format">A format string used in a <see cref="System.Object.ToString()"/> call. Right now
        /// format is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <param name="returnDbNUllIfNotValid">Specifies if DbNull should be returned if value cannot be parsed. Otherwise an exception is thrown.</param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
            if (resultType == null || resultType == typeof(string))
                return s;

            object result;

            try
            {
                if (typeof(double).IsAssignableFrom(resultType))
                {
                    if (GridUtil.IsEmpty(s))
                        return Convert.DBNull;

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType == typeof(double) || resultType == typeof(float))
                            return Convert.DBNull;
                    }
                }
                else if (typeof(decimal).IsAssignableFrom(resultType))
                {
                    if (GridUtil.IsEmpty(s))
                        return Convert.DBNull;

                    System.Decimal d = decimal.Parse(s, NumberStyles.Any, provider);
                    result = Convert.ChangeType(d, resultType, provider);
                    return result;
                }
                else if (typeof(DateTime).IsAssignableFrom(resultType))
                {
                    if (GridUtil.IsEmpty(s))
                        return Convert.DBNull;

                    if (format.Length > 0)
                    {
                        string[] expectedFormats = { format, "G", "g", "f" ,"F"};
                        return DateTime.ParseExact(s, expectedFormats, provider, DateTimeStyles.AllowInnerWhite|DateTimeStyles.AllowLeadingWhite|DateTimeStyles.AllowTrailingWhite|DateTimeStyles.AllowWhiteSpaces);
                    }
                    else
                        return DateTime.Parse(s, provider, DateTimeStyles.AllowInnerWhite|DateTimeStyles.AllowLeadingWhite|DateTimeStyles.AllowTrailingWhite|DateTimeStyles.AllowWhiteSpaces);
                }
                else if (typeof(bool).IsAssignableFrom(resultType))
                {
                    if (GridUtil.IsEmpty(s))
                        return Convert.DBNull;

                    if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                        return true;
                    else if (s == "0" || s.ToUpper() == bool.TrueString.ToUpper())
                        return false;
                }
                else if (typeof(int).IsAssignableFrom(resultType)
                    || typeof(short).IsAssignableFrom(resultType)
                    || typeof(long).IsAssignableFrom(resultType)
                    )
                {
                    if (GridUtil.IsEmpty(s))
                        return Convert.DBNull;

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (resultType == typeof(Type))
                {
                    result = Type.GetType(s);
                    return result;
                }


                TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);
                if (typeConverter != null &&
                    typeConverter.CanConvertFrom(typeof(System.String)) &&
                    s != null && s.Length > 0
                    )
                {
                    if (provider is CultureInfo)
                        result = typeConverter.ConvertFrom(null, (CultureInfo) provider, s);
                    else
                        result = typeConverter.ConvertFrom(s);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (returnDbNUllIfNotValid)
                    return Convert.DBNull;

                throw;
            }

            // throw new InvalidCastException(SR.GetString("InvalidCast_IConvertible"));
            return Convert.DBNull;
        }

        /// <summary>
        /// Generates display text using a specified format, culture info, and number format.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="valueType">The value type on which formatting is based. The original value will first be converted to this type.</param>
        /// <param name="format">The format like in ToString(string format).</param>
        /// <param name="ci">The <see cref="CultureInfo"/> for formatting the value.</param>
        /// <param name="nfi">The <see cref="NumberFormatInfo"/> for formatting the value.</param>
        /// <returns>The string with the formatted text for the value.</returns>
        public static string FormatValue(object value, Type valueType, string format, CultureInfo ci, NumberFormatInfo nfi)
        {
            string strResult;
            object obj;
            try
            {
                if (value is string)
                    return (string)value;
                else if (value is byte[] || value is System.Drawing.Image) // Picture
                    return "";
                else if (value == null || valueType == null || value.GetType() == valueType)
                    obj = value;
                else
                {
                    try
                    {
                        obj = GridCellValueConvert.ChangeType(value, valueType, ci, true);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(Switches.ValueConversion.TraceWarning, ex.ToString());
                        obj = value;
                        if (!(ex is FormatException || ex.InnerException is FormatException))
                            throw;
                    }
                }

                if (obj == null || obj is System.DBNull)
                    strResult = String.Empty;    // or "NullString"
                else
                {
                    if (format.Length > 0 && obj is IFormattable)
                    {
                        if (nfi == null)
                        {
                            if (ci != null)
                                nfi = ci.NumberFormat;
                            else
                                nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo;
                        }
                        strResult = ((IFormattable) obj).ToString(format, nfi);
                    }
                    else
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
                        if (tc.CanConvertTo(typeof(string)))
                        {
                            strResult = (string) tc.ConvertTo(null, ci, obj, typeof(string));
                        }
                        else if (obj is IConvertible)
                            strResult = Convert.ToString(obj, ci);
                        else
                            strResult = obj.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(Switches.ValueConversion.TraceWarning, ex.ToString());
                strResult = String.Empty;
                throw;   // TODO: should I throw a more specific instead?
            }
            if (strResult == null)
                strResult = String.Empty;

            if (allowFormatValueTrimEnd)
                strResult = strResult.TrimEnd();
            return strResult;
        }

        static bool allowFormatValueTrimEnd = false;

        /// <summary>
        /// Defines whether <see cref="FormatValue"/> should trim whitespace characters from
        /// the end of the formatted text.
        /// </summary>
        public static bool AllowFormatValueTrimEnd
        {
            get
            {
                return allowFormatValueTrimEnd ;
            }
            set
            {
                allowFormatValueTrimEnd  = value;
            }
        }


        /// <summary>
        /// Returns a representative value for any given type. Is useful to preview
        /// result of a format in <see cref="System.Windows.Forms.PropertyGrid"/>. See <see cref="GridStyleInfo.FormatPreview"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>A value with the specified type.</returns>
        public static object GetDefaultValue(Type type)
        {
            object value;

            if (type == null)
                return "0";

            lock(cachedDefaultValues)
            {
                if (cachedDefaultValues.Contains(type))
                    value = cachedDefaultValues[type];
                else
                {
                    switch (type.FullName)
                    {
                        case "System.Double":
                        case "System.Single":
                        case "System.Decimal":
                            value = 123.4567;
                            break;

                        case "System.Boolean":
                            value = true;
                            break;

                        case "System.Drawing.Color":
                            value = System.Drawing.Color.Black;
                            break;

                        case "System.String":
                            value = String.Empty;
                            break;

                        case "System.DateTime":
                            value = DateTime.Now;
                            break;

                        case "System.Int32":
                        case "System.Int16":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Byte":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            value = 123;
                            break;

                        case "System.Char":
                            value = 'A';
                            break;

                        case "System.DBNull":
                            value = Convert.DBNull;
                            break;

                        default:
                            value = "";
                            break;
                    }
                    cachedDefaultValues[type] = value;
                }
                return value;
            }

        }

    }
#endif
}
