#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Styles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Globalization;
    using System.ComponentModel;
    using System.IO;
    using Syncfusion.Windows.ComponentModel;
#if SILVERLIGHT
    using ArrayList = System.Collections.Generic.List<object>;
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;
#endif

    /// <summary>
    /// <see cref="ValueConvert"/> provides conversion routines for values
    /// to convert them to another type and routines for formatting values.
    /// </summary>
    /// <summary>
    /// <see cref="ValueConvert"/> provides conversion routines for values
    /// to convert them to another type and routines for formatting values.
    /// </summary>
    public class ValueConvert
    {
        ValueConvert()
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
        /// <param name="returnDbNUllIfNotValid">Indicates whether exceptions should be avoided or catched and return value should be DBNull if
        /// it cannot be converted to the target type.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider, bool returnDbNUllIfNotValid)
        {

            return ChangeType(value, type, provider, "", returnDbNUllIfNotValid);
        }

        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <param name="format">Format string.</param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether exceptions should be avoided or catched and return value should be DBNull if
        /// it cannot be converted to the target type.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                value = ChangeType(value, nullableUnderlyingType, provider, true);
                return NullableHelper.FixDbNUllasNull(value, type);
            }
#endif

            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                try
                {
                    if (value is string)
                    {
                        if (format != null && format.Length > 0)
                            value = Parse((string)value, type, provider, format, returnDbNUllIfNotValid);
                        else
                            value = Parse((string)value, type, provider, "", returnDbNUllIfNotValid);

                    }
                    else if (value is System.DBNull)
                    {
                        // value = null; changed after 4.1.0.50: do not set it to null - this causes then issues
                        // if you have a DataTable and the key is used for lookups, e.g.
                        // see sample in http://www.syncfusion.com/support/forums/message.aspx?MessageID=40207
                        // For NullableTypes the above call to NullableHelper.FixDbNUllasNull will
                        // take care of converting DbNull to null for nullable types only.
                    }
                    else if (type.IsEnum)
                    {
                        value = Convert.ChangeType(value, typeof(int), provider);
                        value = Enum.ToObject(type, (int)value);
                    }
                    else if (type == typeof(string) && !(value is IConvertible))
                    {
                        value = value != null ? value.ToString() : "";
                    }
                    else
                        value = NullableHelper.ChangeType(value, type, provider);
                }
                catch
                {
                    if (returnDbNUllIfNotValid)
                        return Convert.DBNull;

                    throw;
                }
            }

            if ((value == null || value is DBNull) && type == typeof(string))
                return "";

            return value;
        }

        static Hashtable cachedDefaultValues = new Hashtable();

        /// <summary>
        /// Overloaded. Parses the given text using the resultTypes "Parse" method or using a type converter.
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
        /// Parses the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="format">A format string used in a <see cref="System.Object.ToString"/> call. Right now
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
        /// <param name="format">A format string used in a <see cref="System.Object.ToString"/> call. Right now
        /// format is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether DbNull should be returned if value cannot be parsed. Otherwise an exception is thrown.</param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, format, returnDbNUllIfNotValid);
            return NullableHelper.FixDbNUllasNull(value, resultType);
        }

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="formats">A string array holding permissible formats used in a <see cref="System.Object.ToString"/> call. Right now
        /// formats is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether DbNull should be returned if value cannot be parsed. Otherwise an exception is thrown.</param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string[] formats, bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, "", formats, returnDbNUllIfNotValid);
            return NullableHelper.FixDbNUllasNull(value, resultType);
        }

        static object _Parse(string s, Type resultType, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
            return _Parse(s, resultType, provider, format, null, returnDbNUllIfNotValid);
        }
        static object _Parse(string s, Type resultType, IFormatProvider provider, string format, string[] formats, bool returnDbNUllIfNotValid)
        {

            //Fix for defect #12619.
            if (resultType == null) //|| resultType == typeof(string))
                return s;

            object result;

            try
            {
                if (typeof(double).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
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
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    decimal d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (decimal.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    d = decimal.Parse(s, NumberStyles.Any, provider);
                    result = Convert.ChangeType(d, resultType, provider);
#endif
                }
                else if (typeof(DateTime).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    if (formats == null || formats.GetLength(0) == 0 && format.Length > 0)
                        formats = new string[] { format, "G", "g", "f", "F", "d", "D" };

                    if (formats != null && formats.GetLength(0) > 0)
                    {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                        DateTime dtresult;
                        if (DateTime.TryParseExact(s, formats, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out dtresult))
                            return dtresult;
#else
                        try
                        {
                        return DateTime.ParseExact(s, formats, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces);
                    }
                        catch
                    {
                        }
#endif
                    }

                    return DateTime.Parse(s, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces);
                }
                else if (typeof(bool).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                        return true;
                    else if (s == "0" || s.ToUpper() == bool.TrueString.ToUpper())
                        return false;
                }
                else if (typeof(long).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    long d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (long.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    try
                    {
                        d = long.Parse(s, NumberStyles.Any, provider);
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
                    catch
                    {
                    }
#endif
                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (typeof(ulong).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    ulong d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (ulong.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    try
                    {
                        d = ulong.Parse(s, NumberStyles.Any, provider);
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
                    catch
                    {
                    }
#endif

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (typeof(int).IsAssignableFrom(resultType)
                    || typeof(short).IsAssignableFrom(resultType)
                    || typeof(float).IsAssignableFrom(resultType)
                    || typeof(uint).IsAssignableFrom(resultType)
                    || typeof(ushort).IsAssignableFrom(resultType)
                    || typeof(byte).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
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
                else if (typeof(System.Nullable<int>).IsAssignableFrom(resultType)
          || typeof(System.Nullable<short>).IsAssignableFrom(resultType)
          || typeof(System.Nullable<float>).IsAssignableFrom(resultType)
          || typeof(System.Nullable<uint>).IsAssignableFrom(resultType)
          || typeof(System.Nullable<ushort>).IsAssignableFrom(resultType)
          || typeof(System.Nullable<byte>).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    if (string.IsNullOrEmpty(s))
                        return null;
                    else
                    {
                        IConvertible convertibleString = (IConvertible)s;
                        result = new Nullable<double>((double)convertibleString.ToType(typeof(double), CultureInfo.CurrentCulture));
                        return result;
                    }
                }
                else if (typeof(Enum).IsAssignableFrom(resultType))
                {
                    //Enum type ValueConversion
                    result = Enum.Parse(resultType, s);
                    return result;
                }
                else if (resultType == typeof(Type))
                {
                    result = Type.GetType(s);
                    return result;
                }

                result = Convert.ChangeType(s, resultType, provider);
                return result;
                //TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);
                //if (typeConverter != null &&
                //    typeConverter.CanConvertFrom(typeof(System.String)) &&
                //    s != null && s.Length > 0
                //    )
                //{
                //    if (provider is CultureInfo)
                //        result = typeConverter.ConvertFrom(null, (CultureInfo) provider, s);
                //    else
                //        result = typeConverter.ConvertFrom(s);
                //    return result;
                //}
            }
            catch
            {
                if (returnDbNUllIfNotValid)
                    return Convert.DBNull;

                throw;
            }

            // throw new InvalidCastException(SR.GetString("InvalidCast_IConvertible"));
            // return Convert.DBNull;
        }

        /// <summary>
        /// Generates display text using the specified format, culture info and number format.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="valueType">The value type on which formatting is based. The original value will first be converted to this type.</param>
        /// <param name="format">The format like in ToString(string format).</param>
        /// <param name="ci">The <see cref="CultureInfo"/> for formatting the value.</param>
        /// <param name="nfi">The <see cref="NumberFormatInfo"/> for formatting the value.</param>
        /// <returns>The string with the formatted text for the value.</returns>
        public static string FormatValue(object value, Type valueType, string format, CultureInfo ci, NumberFormatInfo nfi, IFormatProvider FormatProvider)
        {
            string strResult;
            object obj;
            try
            {
                if (value is string)
                {
                    if (!string.IsNullOrEmpty((string)value) && ((string)value)[0] == '\'')
                    {
                        string _value = ((string)value).Substring(1);
                        if (!_value.Contains('\''))
                        {
                            //To remove the apostrophe symbol in the text as like excel
                            return ((string)value).Substring(1);
                        }
                    }
                    if (FormatProvider != null)
                        return string.Format(FormatProvider, format, value);
                    return (string)value;
                }
                else if (value is byte[]) // Picture
                    return "";
                else if (value == null || valueType == null || value.GetType() == valueType)
                    obj = value;
                else
                {
                    try
                    {
                        obj = ValueConvert.ChangeType(value, valueType, ci, true);
                    }
                    catch (Exception ex)
                    {
                        obj = value;
                        if (!(ex is FormatException || ex.InnerException is FormatException))
                            throw;
                    }
                }
                if (format != string.Empty)
                {
                    if (value != null && !(obj is DateTime) && System.Text.RegularExpressions.Regex.IsMatch(format, @"([mdMy][,][ mdMy])|([mdMy][-/][mdMy])|([mdMy][-/](.*)[-/][mdMy])", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace))
                    {
                        double d;
                        if (double.TryParse(value.ToString(), out d))
                            obj = DateTime.FromOADate(d);
                    }
                }

                if (obj == null || obj is System.DBNull)
                    strResult = String.Empty;	// or "NullString"
                else
                {
                    if (FormatProvider != null)
                    {
                        strResult = string.Format(FormatProvider, format, obj);
                    }
                    else if (obj is IFormattable)
                    {
                        IFormattable formattableValue = (IFormattable)obj;
                        IFormatProvider provider = null;
                        if (nfi != null && !(obj is DateTime))
                            provider = nfi;
                        else if (ci != null)
                            provider = obj is DateTime ? (IFormatProvider)ci.DateTimeFormat : (IFormatProvider)ci.NumberFormat;

                        if (format.Length > 0 || nfi != null)
                            strResult = formattableValue.ToString(format, provider);
                        else
                            strResult = formattableValue.ToString();
                    }
                    else
                    {
                        /*TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
                        if (tc.CanConvertTo(typeof(string)))
                        {
                            strResult = (string) tc.ConvertTo(null, ci, obj, typeof(string));
                        }
                        else */
                        if (obj is IConvertible)
                            strResult = Convert.ToString(obj, ci);
                        else
                            strResult = obj.ToString();
                    }
                }
            }
            catch
            {
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
        /// Indicates whether <see cref="FormatValue"/> should trim whitespace characters from
        /// the end of the formatted text.
        /// </summary>
        public static bool AllowFormatValueTrimEnd
        {
            get
            {
                return allowFormatValueTrimEnd;
            }
            set
            {
                allowFormatValueTrimEnd = value;
            }
        }

        /// <summary>
        /// Returns a representative value for any given type. 
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>A value with the specified type.</returns>
        public static object GetDefaultValue(Type type)
        {
            object value;

            if (type == null)
                return "0";

            lock (cachedDefaultValues)
            {
                if (cachedDefaultValues.ContainsKey(type))
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

        /// <summary>
        /// Overloaded. Parses the given string including type information. String can be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        static bool ParseValueWithTypeInformation(string valueAsString, out object retVal)
        {
            return ParseValueWithTypeInformation(valueAsString, out retVal);
        }

        /// <summary>
        /// Parses the given string including type information. String can be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <param name="allowConvertFromBase64">Indicates whether TypeConverter should be checked whether the type to be
        /// parsed supports conversion to/from byte array (e.g. an Image)</param>
        /// <returns></returns>
        public static bool ParseValueWithTypeInformation(string valueAsString, out object retVal, bool allowConvertFromBase64)
        {
            retVal = null;
            if (valueAsString.StartsWith("'") && valueAsString.EndsWith("'"))
            {
                retVal = valueAsString.Substring(1, valueAsString.Length - 2);
                return true;
            }
            else if (valueAsString.StartsWith("<"))
            {
                int closeBracket = valueAsString.IndexOf(">");
                if (closeBracket > 1)
                {
                    string typeName = valueAsString.Substring(1, closeBracket - 1);
                    if (typeName == "null")
                    {
                        retVal = null;
                        return true;
                    }
                    else if (typeName == "System.DBNull")
                    {
                        retVal = System.DBNull.Value;
                        return true;
                    }
                    else
                    {
                        valueAsString = valueAsString.Substring(closeBracket + 1).Trim();
                        if (valueAsString.StartsWith("'") && valueAsString.EndsWith("'"))
                        {
                            valueAsString = valueAsString.Substring(1, valueAsString.Length - 2);
                            Type type = ValueConvert.GetType(typeName);
                            if (type != null)
                            {
                                bool handled = false;
                                if (allowConvertFromBase64)
                                    handled = TryConvertFromBase64String(type, valueAsString, out retVal);

                                if (!handled)
                                    retVal = ValueConvert.Parse(valueAsString, type, System.Globalization.CultureInfo.InvariantCulture, "");
                                return true;
                            }
                        }
                    }
                }
            }

            retVal = valueAsString;
            return false;
        }

        /// <summary>
        /// Indicates whether the TypeConverter associated with the type supports conversion to/from a byte array (e.g. an Image). 
        /// If that is the case the string is converted to a byte array from a base64 string.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static bool TryConvertFromBase64String(Type type, string valueAsString, out object retVal)
        {
            retVal = null;
            return false;
            //bool handled = false;
            //retVal = null;
            //TypeConverter tc = TypeDescriptor.GetConverter(type);
            //if (tc != null)
            //{
            //    // e.g. an Image
            //    if (tc.CanConvertFrom(typeof(byte[])))
            //    {
            //        byte[] byteArray = (byte[]) Convert.FromBase64String(valueAsString);
            //        retVal = tc.ConvertFrom(byteArray);
            //        handled = true;
            //    }
            //    else if (tc.CanConvertFrom(typeof(MemoryStream)))
            //    {
            //        MemoryStream ms = new MemoryStream((byte[]) Convert.FromBase64String(valueAsString));
            //        retVal = tc.ConvertFrom(ms);
            //        handled = true;
            //    }

            //}
            //return handled;
        }

        /// <summary>
        /// Overloaded. Formats the given value as string including type information. String will be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static string FormatValueWithTypeInformation(object value)
        {
            return FormatValueWithTypeInformation(value, false);
        }

        /// <summary>
        /// Formats the given value as string including type information. String will be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allowConvertToBase64">Indicates whether TypeConverter should be checked whether the type to be
        /// parsed supports conversion to/from byte array (e.g. an Image)</param>
        /// <returns></returns>
        public static string FormatValueWithTypeInformation(object value, bool allowConvertToBase64)
        {
            if (value is string)
                return "'" + (string)value + "'";
            else if (value is DBNull)
            {
                return "<System.DBNull>";
            }
            else if (value == null)
            {
                return "<null>";
            }
            else
            {
                string valueAsString = null;
                if (allowConvertToBase64)
                    valueAsString = TryConvertToBase64String(value);

                if (valueAsString == null)
                    valueAsString = ValueConvert.FormatValue(value, typeof(string), "", System.Globalization.CultureInfo.InvariantCulture, null, null);

                return "<" + GetTypeName(value.GetType()) + "> '" + valueAsString + "'";
            }
        }

        /// <summary>
        /// Indicates whether the TypeConverter associated with the type supports conversion to/from a byte array (e.g. an Image). 
        /// If that is the case the string is converted to a base64 string from a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryConvertToBase64String(object value)
        {
            string valueAsString = null;
            return valueAsString;
        }

        /// <summary>
        /// Returns the type name. If type is not in mscorlib, the assembly name is appended.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            //if (!type.IsPrimitive && type.Module != typeof(object).Module)
            //    return type.FullName + ", " + System.IO.Path.GetFileNameWithoutExtension(type.Module.ScopeName);
            return type.FullName;
        }

        /// <summary>
        /// Returns the type from the specified name. If an assembly name is appended the list of currently loaded
        /// assemblies in the current AppDomain are checked.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            string[] parts = typeName.Split(',');
            //if (parts.Length == 2)
            //{
            //    // Module name without version information.
            //    ResolveEventArgs e = new ResolveEventArgs(parts[1].Trim());
            //    Assembly assembly = AssemblyInfo.AssemblyResolver(null, e);
            //    if (assembly != null)
            //        return assembly.GetType(parts[0]);
            //}

            return Type.GetType(typeName);
        }

        /// <summary>
        /// Indicates whether string is null or empty.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(string str)
        {
            return str == null || str.Length == 0;
        }

    }
}
