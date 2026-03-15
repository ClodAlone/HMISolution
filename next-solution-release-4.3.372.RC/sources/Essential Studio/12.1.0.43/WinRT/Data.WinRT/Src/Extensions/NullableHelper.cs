#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;

namespace Syncfusion.Data.Helper
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.IO;
    using System.Globalization;
    using System.Collections;
#if !WPF
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;

#endif

    /// <summary>
    /// A framework independent utility class for the new Nullable type in .NET Framework 2.0
    /// </summary>
    internal class NullableHelperInternal
    {
#if WPF
        /// <summary>
        /// Indicates whether the specified PropertyDescriptor has nested properties.
        /// </summary>
        /// <param name="pd">The PropertyDescriptor to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        internal static bool IsComplexType(PropertyDescriptor pd)
        {
            if (pd.ComponentType == typeof(Type))
                return false;

            Type t = pd.PropertyType;
            return IsComplexType(t);
        }
#else
        /// <summary>
        /// Indicates whether the specified PropertyDescriptor has nested properties.
        /// </summary>
        /// <param name="pd">The PropertyDescriptor to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        internal static bool IsComplexType(PropertyInfo pd)
        {
            Type t = pd.PropertyType;
            return IsComplexType(t);
        }
#endif

        /// <summary>
        /// Indicates whether the specified Type has nested properties.
        /// </summary>
        /// <param name="t">The Type to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        public static bool IsComplexType(Type t)
        {
            Type underlyingType = NullableHelperInternal.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;

            if (t != typeof(object)
                && t != typeof(Decimal)
                && t != typeof(DateTime)
                && t != typeof(Type)
                //&& t != typeof(System.Drawing.Color)
                && t != typeof(string)
                && t != typeof(Guid)
                && t.BaseType() != typeof(Enum)
                && !t.IsPrimitive())
                return true;

            return false;
        }

#if WPF
        public static bool IsIEnumerableType(PropertyDescriptor pd)
#else
        public static bool IsIEnumerableType(PropertyInfo pd)
#endif
        {
            if (NullableHelperInternal.IsComplexType(pd.PropertyType)
                && !typeof(byte[]).IsAssignableFrom(pd.PropertyType)
                && pd.PropertyType != typeof(string)
                && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
                && !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Use this method instead of Convert.ChangeType. Makes Convert.ChangeType work with Nullable types.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                if (value is string && nullableUnderlyingType != typeof(string))
                {
                    if (ValueConvert.IsEmpty((string)value))
                        return null;
                }

                value = ChangeType(value, nullableUnderlyingType);
#if WinRT
                if (value is Nullable)
#else
                if (value is DBNull)
#endif
                    return null;
                return value;
            }
#endif
            if (!type.IsInterface())
                return TypeConverterHelper.ChangeType(value, type);
            else
                return value;
        }

        /// <summary>
        /// Use this method instead of Convert.ChangeType. Makes Convert.ChangeType work with Nullable types.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                if (value is string && nullableUnderlyingType != typeof(string))
                {
                    if (ValueConvert.IsEmpty((string)value))
                        return null;
                }

                value = ChangeType(value, nullableUnderlyingType, provider);
#if WinRT
                if (value is Nullable)
#else
                if (value is DBNull)
#endif
                    return null;
                return value;
            }
#endif
            return TypeConverterHelper.ChangeType(value, type, provider);
        }

        public static bool IsNullableType(Type nullableType)
        {
            if (nullableType == null)
            {
                throw new ArgumentNullException("nullableType");
            }

            bool result = (nullableType.IsGenericType() && !nullableType.IsGenericTypeDefinition()) &&
                          (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>));
            return result;
        }

        public static Type GetNullableType(Type type)
        {
            if (type == null)
                return null;

            if (NullableHelperInternal.IsNullableType(type))
                return type;

            var underlyingType = Nullable.GetUnderlyingType(type);
            underlyingType = underlyingType ?? type;

#if WPF || SILVERLIGHT
            if (underlyingType.IsValueType)
#endif

#if WinRT
            if (underlyingType.IsValueType())
#endif
                //#if !SILVERLIGHT
                //                return typeof (Nullable<>).MakeGenericType(type);
                //#endif
                //#if SILVERLIGHT
                return typeof(Nullable<>).MakeGenericType(type);
            //#endif
            return type;
        }
        /// <summary>
        /// Returns null if value is DBNull and specified type is a Nullable type. Otherwise the value is returned unchanged.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FixDbNUllasNull(object value, Type type)
        {
            if (type == null)
                return value;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
#if WinRT
                if (value is Nullable)
#else
                if (value is DBNull)
#endif
                    return null;
            }
#endif

            /*
             * Do not return DBNull for strong typed properties of an object. For example, if Parsing a string failed 
             * (e.g. if an empty string was passed in as argument) we need to check if it as object and in that
             * case return null. Only if it is a ValueType type (that is not nullable) then we should return DBNull
             * so that it also works with DataRowView.
             * */
            if (!type.IsValueType())
            {
#if WinRT
                if (value is Nullable)
#else
                if (value is DBNull)
#endif
                    return null;
            }
            return value;
        }

        /// <summary>
        /// Returns the underlying type of a Nullable type. For .NET 1.0 and 1.1 this method will always return null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType(Type type)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            return type == null ? null : Nullable.GetUnderlyingType(type);
#else
            return null;
#endif
        }

        /// <exclude/>
        private class TypeConverterHelper
        {
            public static object ChangeType(object value, Type type)
            {
                return ChangeType(value, type, null);
            }

            public static object ChangeType(object value, Type type, IFormatProvider provider)
            {
                //Fix for defects: 13036, 13024, 12601  & 12716
                if (value == null)
                    return null;
#if WPF
                TypeConverter typeConverter = TypeDescriptor.GetConverter(value.GetType());
                if (typeConverter != null && typeConverter.CanConvertTo(type))
                    return typeConverter.ConvertTo(value, type);
#endif
#if WinRT
                if (value is Nullable)
                    return null;
#else
                if (value is DBNull)
                    return DBNull.Value;
#endif

#if !SILVERLIGHT && !WP7
                if (type.IsEnum())
                {
                    return Enum.Parse(type, Convert.ToString(value));
                }
#endif
                return Convert.ChangeType(value, type, provider);
            }
        }
    }

    /// <summary>
    /// <see cref="ValueConvert"/> provides conversion routines for values
    /// to convert them to another type and routines for formatting values.
    /// </summary>
    public class ValueConvert
    {
        private ValueConvert()
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
        public static object ChangeType(object value, Type type, IFormatProvider provider, string format,
                                        bool returnDbNUllIfNotValid)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                value = ChangeType(value, nullableUnderlyingType, provider, true);
                return NullableHelperInternal.FixDbNUllasNull(value, type);
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
#if WinRT
                    else if (value is Nullable)
#else
                    else if (value is System.DBNull)
#endif
                    {
                        // value = null; changed after 4.1.0.50: do not set it to null - this causes then issues
                        // if you have a DataTable and the key is used for lookups, e.g.
                        // see sample in http://www.syncfusion.com/support/forums/message.aspx?MessageID=40207
                        // For NullableTypes the above call to NullableHelper.FixDbNUllasNull will
                        // take care of converting DbNull to null for nullable types only.
                    }
                    else if (type.IsEnum())
                    {
                        value = Convert.ChangeType(value, typeof(int), provider);
                        value = Enum.ToObject(type, (int)value);
                    }
#if WinRT
                    else if (type == typeof(string))
#else
                    else if (type == typeof(string) && !(value is IConvertible))
#endif
                    {
                        value = value != null ? value.ToString() : "";
                    }
                    else
                        value = NullableHelperInternal.ChangeType(value, type, provider);
                }
                catch
                {
                    if (returnDbNUllIfNotValid)
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif
                    throw;
                }
            }

#if WinRT
            if ((value == null || value is Nullable) && type == typeof(string))
#else
            if ((value == null || value is DBNull) && type == typeof(string))
#endif
                return "";

            return value;
        }

        private static Hashtable cachedDefaultValues = new Hashtable();

        /// <summary>
        /// Overloaded. Parses the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <returns>The new value in the target type.</returns>
        private static object Parse(string s, Type resultType, IFormatProvider provider)
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
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format,
                                   bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, format, returnDbNUllIfNotValid);
            return NullableHelperInternal.FixDbNUllasNull(value, resultType);
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
        public static object Parse(string s, Type resultType, IFormatProvider provider, string[] formats,
                                   bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, "", formats, returnDbNUllIfNotValid);
            return NullableHelperInternal.FixDbNUllasNull(value, resultType);
        }

        private static object _Parse(string s, Type resultType, IFormatProvider provider, string format,
                                     bool returnDbNUllIfNotValid)
        {
            return _Parse(s, resultType, provider, format, null, returnDbNUllIfNotValid);
        }

        private static object _Parse(string s, Type resultType, IFormatProvider provider, string format,
                                     string[] formats, bool returnDbNUllIfNotValid)
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
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType == typeof(double) || resultType == typeof(float))
#if WinRT
                            return null;
#else
                            return Convert.DBNull;
#endif
                    }
                }
                else if (typeof(decimal).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    decimal d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
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
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    if (formats == null || formats.GetLength(0) == 0 && format.Length > 0)
                        formats = new string[] { format, "G", "g", "f", "F", "d", "D" };

                    if (formats != null && formats.GetLength(0) > 0)
                    {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                        DateTime dtresult;
                        if (DateTime.TryParseExact(s, formats, provider,
                                                   DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite |
                                                   DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
                                                   out dtresult))
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

                    DateTime validDateTime;
                    DateTime.TryParse(s, provider,
                                      DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite |
                                      DateTimeStyles.AllowTrailingWhite |
                                      DateTimeStyles.AllowWhiteSpaces, out validDateTime);
                    return validDateTime;
                }
                else if (typeof(TimeSpan).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    var isValid = false;
                    TimeSpan timespan;
                    if (TimeSpan.TryParse(s, out timespan))
                    {
                        isValid = true;
                    }
                    if (isValid)
                    {
                        return timespan;
                    }
                }
                else if (typeof(bool).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                        return true;
                    else if (s == "0" || s.ToUpper() == bool.FalseString.ToUpper())
                        return false;
                }
                else if (typeof(long).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

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
                        if (resultType.IsPrimitive() && !resultType.IsEnum())
#if WinRT
                            return null;
#else
                            return Convert.DBNull;
#endif
                    }
                }
                else if (typeof(ulong).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

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
                        if (resultType.IsPrimitive() && !resultType.IsEnum())
#if WinRT
                            return null;
#else
                            return Convert.DBNull;
#endif
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
#if WinRT
                        return null;
#else
                        return Convert.DBNull;
#endif

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive() && !resultType.IsEnum())
#if WinRT
                            return null;
#else
                            return Convert.DBNull;
#endif
                    }
                }
                else if (resultType == typeof(Type))
                {
                    result = Type.GetType(s);
                    return result;
                }

#if WPF
                TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);

                if (typeConverter is NullableConverter)
                {
                    Type nullableUnderlyingType = NullableHelperInternal.GetUnderlyingType(resultType);
                    if (nullableUnderlyingType != null)
                        return _Parse(s, nullableUnderlyingType, provider, format, formats, returnDbNUllIfNotValid);
                }

                if (typeConverter != null &&
                    typeConverter.CanConvertFrom(typeof(System.String)) &&
                    s != null && s.Length > 0
                    )
                {
                    if (provider is CultureInfo)
                        result = typeConverter.ConvertFrom(null, (CultureInfo)provider, s);
                    else
                        result = typeConverter.ConvertFrom(s);
                    return result;
                }
#endif
            }
            catch
            {
                if (returnDbNUllIfNotValid)
#if WinRT
                    return null;
#else
                    return Convert.DBNull;
#endif

                throw;
            }

            // throw new InvalidCastException(SR.GetString("InvalidCast_IConvertible"));
#if WinRT
            return null;
#else
            return Convert.DBNull;
#endif
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
        public static string FormatValue(object value, Type valueType, string format, CultureInfo ci,
                                         NumberFormatInfo nfi)
        {
            string strResult;
            object obj;
            try
            {
                if (value is string)
                    return (string)value;
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
#if WinRT
                if (obj == null || obj is Nullable)
#else
                if (obj == null || obj is System.DBNull)
#endif
                    strResult = String.Empty; // or "NullString"
                else
                {
                    if (obj is IFormattable)
                    {
                        IFormattable formattableValue = (IFormattable)obj;
                        IFormatProvider provider = null;
                        if (nfi != null && !(obj is DateTime))
                            provider = nfi;
                        else if (ci != null)
                            provider = obj is DateTime
                                           ? (IFormatProvider)ci.DateTimeFormat
                                           : (IFormatProvider)ci.NumberFormat;

                        if (format.Length > 0 || nfi != null)
                            strResult = formattableValue.ToString(format, provider);
                        else
                            strResult = formattableValue.ToString();
                    }
                    else
                    {
#if WPF
                        TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
                        if (tc.CanConvertTo(typeof(string)))
                        {
                            strResult = (string)tc.ConvertTo(null, ci, obj, typeof(string));
                        }
                        else if (obj is IConvertible)
#endif
#if SILVERLIGHT || WP
                        if (obj is IConvertible)
#endif
                        strResult = Convert.ToString(obj, ci);
#if !WinRT
                        else
                            strResult = obj.ToString();
#endif
                    }
                }
            }
            catch
            {
                strResult = String.Empty;
                throw; // TODO: should I throw a more specific instead?
            }
            if (strResult == null)
                strResult = String.Empty;

            if (allowFormatValueTrimEnd)
                strResult = strResult.TrimEnd();
            return strResult;
        }

        private static bool allowFormatValueTrimEnd = false;

        /// <summary>
        /// Indicates whether <see cref="FormatValue"/> should trim whitespace characters from
        /// the end of the formatted text.
        /// </summary>
        public static bool AllowFormatValueTrimEnd
        {
            get { return allowFormatValueTrimEnd; }
            set { allowFormatValueTrimEnd = value; }
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
#if WPF

                if (cachedDefaultValues.Contains(type))
#else
                if (cachedDefaultValues.ContainsKey(type))
#endif
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
#if !WinRT
                        case "System.DBNull":
                            value = Convert.DBNull;
                            break;
#endif
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
        private static bool ParseValueWithTypeInformation(string valueAsString, out object retVal)
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
        public static bool ParseValueWithTypeInformation(string valueAsString, out object retVal,
                                                         bool allowConvertFromBase64)
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
#if !WinRT
                    else if (typeName == "System.DBNull")
                    {
                        retVal = System.DBNull.Value;
                        return true;
                    }
#endif
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
#if WPF

                                if (allowConvertFromBase64)
                                    handled = TryConvertFromBase64String(type, valueAsString, out retVal);
#endif
                                if (!handled)
                                    retVal = ValueConvert.Parse(valueAsString, type,
                                                                System.Globalization.CultureInfo.InvariantCulture, "");
                                return true;
                            }
                        }
                    }
                }
            }

            retVal = valueAsString;
            return false;
        }

#if WPF
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
            bool handled = false;
            retVal = null;
            TypeConverter tc = TypeDescriptor.GetConverter(type);
            if (tc != null)
            {
                // e.g. an Image
                if (tc.CanConvertFrom(typeof(byte[])))
                {
                    byte[] byteArray = (byte[])Convert.FromBase64String(valueAsString);
                    retVal = tc.ConvertFrom(byteArray);
                    handled = true;
                }
                else if (tc.CanConvertFrom(typeof(MemoryStream)))
                {
                    MemoryStream ms = new MemoryStream((byte[])Convert.FromBase64String(valueAsString));
                    retVal = tc.ConvertFrom(ms);
                    handled = true;
                }

            }
            return handled;
        }
#endif

        /// <summary>
        /// Overloaded. Formats the given value as string including type information. String will be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string FormatValueWithTypeInformation(object value)
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
#if !WinRT
            else if (value is DBNull)
            {
                return "<System.DBNull>";
            }
#endif
            else if (value == null)
            {
                return "<null>";
            }
            else
            {
                string valueAsString = null;
#if WPF

                if (allowConvertToBase64)
                    valueAsString = TryConvertToBase64String(value);
#endif
                if (valueAsString == null)
                    valueAsString = ValueConvert.FormatValue(value, typeof(string), "",
                                                             System.Globalization.CultureInfo.InvariantCulture, null);

                return "<" + GetTypeName(value.GetType()) + "> '" + valueAsString + "'";
            }
        }

#if WPF
        /// <summary>
        /// Indicates whether the TypeConverter associated with the type supports conversion to/from a byte array (e.g. an Image). 
        /// If that is the case the string is converted to a base64 string from a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryConvertToBase64String(object value)
        {
            string valueAsString = null;
            TypeConverter tc = TypeDescriptor.GetConverter(value);
            if (tc != null)
            {
                // e.g. an Image
                if (tc.CanConvertTo(typeof(byte[])))
                {
                    byte[] byteArray = (byte[])tc.ConvertTo(value, typeof(byte[]));
                    valueAsString = Convert.ToBase64String(byteArray);
                }
                else if (tc.CanConvertTo(typeof(MemoryStream)))
                {
                    MemoryStream ms = (MemoryStream)tc.ConvertTo(value, typeof(MemoryStream));
                    valueAsString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return valueAsString;
        }
#endif

        /// <summary>
        /// Returns the type name. If type is not in mscorlib, the assembly name is appended.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
#if WPF
            if (!type.IsPrimitive && type.Module != typeof(object).Module)
                return type.FullName + ", " + System.IO.Path.GetFileNameWithoutExtension(type.Module.ScopeName);
#endif
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
#if WPF
            string[] parts = typeName.Split(',');
            if (parts.Length == 2)
            {
                // Module name without version information.
                ResolveEventArgs e = new ResolveEventArgs(parts[1].Trim());
                Assembly assembly = AssemblyResolver(null, e);
                if (assembly != null)
                    return assembly.GetType(parts[0]);
            }
#endif
            return Type.GetType(typeName);
        }

#if WPF
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int n = 0; n < assemblies.Length; n++)
            {
                if (assemblies[n].GetName().Name == e.Name)
                    return assemblies[n];
            }
            return null;
        }
#endif

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