#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Reflection;
#if !WinRT
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.ComponentModel
#else
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.ComponentModel
#endif
{

    /// <summary>
    /// A framework independent utility class for the new Nullable type in .NET Framework 2.0
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class NullableHelper
    {
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
#if !WinRT
                if (value is DBNull)
                    return null;
#endif
                return value;
            }
#endif

            return TypeConverterHelper.ChangeType(value, type);
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
#if !WinRT
                if (value is DBNull)
                    return null;
#endif
                return value;
            }
#endif
            return TypeConverterHelper.ChangeType(value, type, provider);
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
#if !WinRT
                if (value is DBNull)
                    return null;
#endif
            }
#endif

            /*
             * Do not return DBNull for strong typed properties of an object. For example, if Parsing a string failed 
             * (e.g. if an empty string was passed in as argument) we need to check if it as object and in that
             * case return null. Only if it is a ValueType type (that is not nullable) then we should return DBNull
             * so that it also works with DataRowView.
             * */
#if !WinRT
            if (!type.IsValueType)
            {
                if (value is DBNull)
                    return null;
            }
#endif
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

        public static bool IsNullableType(Type nullableType)
        {
            if (nullableType == null)
            {
                throw new ArgumentNullException("nullableType");
            }

            bool result = false;
#if !WinRT
            if ((nullableType.IsGenericType && !nullableType.IsGenericTypeDefinition) && (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)))
#else
            if ((nullableType.GetTypeInfo().IsGenericType && !nullableType.GetTypeInfo().IsGenericTypeDefinition) && (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)))
#endif
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Indicates whether the specified Type has nested properties.
        /// </summary>
        /// <param name="t">The Type to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        public static bool IsComplexType(Type t)
        {
            Type underlyingType = NullableHelper.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;
#if !WinRT
            if (t != typeof(object)
                && t != typeof(Decimal)
                && t != typeof(DateTime)
                && t != typeof(Type)
                //&& t != typeof(System.Drawing.Color)
                && t != typeof(string)
                && t != typeof(Guid)
                && t.BaseType != typeof(Enum)
                && !t.IsPrimitive)
                return true;
#else
            if (t != typeof(object)
                && t != typeof(Decimal)
                && t != typeof(DateTime)
                && t != typeof(Type)
                //&& t != typeof(System.Drawing.Color)
                && t != typeof(string)
                && t != typeof(Guid)
                && t.GetTypeInfo().BaseType != typeof(Enum)
                && !t.GetTypeInfo().IsPrimitive)
                return true;
#endif

            return false;
        }

        /// <exclude/>
        class TypeConverterHelper
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

                //TypeConverter typeConverter = TypeDescriptor.GetConverter(value.GetType());
                //if (typeConverter != null && typeConverter.CanConvertTo(type))
                //    return typeConverter.ConvertTo(value, type);
#if !WinRT
                if (value is DBNull)
                    return DBNull.Value;

                if (type.IsEnum)
                {
                    return Enum.Parse(type, Convert.ToString(value), true);
                }
#else
                if (value == null)
                    return null;
#endif
                return Convert.ChangeType(value, type, provider);
            }
        }

    }
}
