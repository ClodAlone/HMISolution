using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataReader.Extensions
{
    public static class TypeExtensions
    {
        const String openArrayValue = "{";
        const String closeArrayValue = "}";
        const String separatorArrayValue = " |";

        #region Type to DbType Converter
        static readonly Dictionary<Type, DbType> typeToDbType = new Dictionary<Type, DbType>()
        {
            { typeof(byte) , DbType.Byte },
            { typeof(sbyte) , DbType.SByte },
            { typeof(ushort) , DbType.UInt16 },
            { typeof(short) , DbType.Int16 },
            { typeof(uint) , DbType.UInt32 },
            { typeof(int) , DbType.Int32 },
            { typeof(ulong) , DbType.UInt64 },
            { typeof(long) , DbType.Int64 },
            { typeof(float) , DbType.Single },
            { typeof(double) , DbType.Double },
            { typeof(decimal) , DbType.Decimal },
            { typeof(bool) , DbType.Boolean },
            { typeof(string) , DbType.String },
            { typeof(char) , DbType.StringFixedLength },
            { typeof(Guid) , DbType.Guid },
            { typeof(DateTime) , DbType.DateTime },
            { typeof(DateTimeOffset) , DbType.DateTimeOffset },
            { typeof(byte[]) , DbType.Binary },
            { typeof(byte?) , DbType.Byte },
            { typeof(sbyte?) , DbType.SByte },
            { typeof(ushort?) , DbType.UInt16 },
            { typeof(short?) , DbType.Int16 },
            { typeof(uint?) , DbType.UInt32 },
            { typeof(int?) , DbType.Int32 },
            { typeof(ulong?) , DbType.UInt64 },
            { typeof(long?) , DbType.Int64 },
            { typeof(float?) , DbType.Single },
            { typeof(double?) , DbType.Double },
            { typeof(decimal?) , DbType.Decimal },
            { typeof(bool?) , DbType.Boolean },
            { typeof(char?) , DbType.StringFixedLength },
            { typeof(Guid?) , DbType.Guid },
            { typeof(DateTime?) , DbType.DateTime },
            { typeof(DateTimeOffset?) , DbType.DateTimeOffset },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , DbType.Binary },
#endif
            { typeof(TimeSpan) , DbType.Time }
        };
        
        public static DbType ToDbType(this Type type)
        {
            if (!typeToDbType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type conversion ('{0}') !", type.FullName));

            return typeToDbType[type];
        }
        #endregion

        #region Sign/Unsign Helpers
        static readonly Dictionary<Type, Type> unsignedToSigned = new Dictionary<Type, Type>()
        {
            { typeof(byte) , typeof(sbyte) },
            { typeof(ushort) , typeof(short) },
            { typeof(uint) , typeof(int) },
            { typeof(ulong) , typeof(long) },
            { typeof(byte[]) , typeof(sbyte[]) },
            { typeof(byte?) , typeof(sbyte?) },
            { typeof(ushort?) , typeof(short?) },
            { typeof(uint?) , typeof(int?) },
            { typeof(ulong?) , typeof(long?) }
        };

        static readonly Dictionary<Type, Type> signedToUnsigned = new Dictionary<Type, Type>()
        {
            { typeof(sbyte) , typeof(byte) },
            { typeof(short) , typeof(ushort) },
            { typeof(int) , typeof(uint) },
            { typeof(long) , typeof(ulong) },
            { typeof(sbyte[]) , typeof(byte[]) },
            { typeof(sbyte?) , typeof(byte?) },
            { typeof(short?) , typeof(ushort?) },
            { typeof(int?) , typeof(uint?) },
            { typeof(long?) , typeof(ulong?) }
        };

        public static bool IsUnsigned(this Type type)
        {
            return unsignedToSigned.ContainsKey(type);
        }

        public static Type ConvertSignedOrUnsigned(this Type type)
        {
            if (unsignedToSigned.ContainsKey(type))
                return unsignedToSigned[type];
            else if (signedToUnsigned.ContainsKey(type))
                return signedToUnsigned[type];

            return type;
        }
        #endregion

        #region Value's Ranges Helpers
        static readonly Dictionary<Type, object> minValues = new Dictionary<Type, object>()
        {
            {typeof(sbyte), sbyte.MinValue },
            {typeof(byte), byte.MinValue },
            {typeof(short), short.MinValue },
            {typeof(ushort), ushort.MinValue },
            {typeof(int), int.MinValue },
            {typeof(uint), uint.MinValue },
            {typeof(long), long.MinValue },
            {typeof(ulong), ulong.MinValue },
            {typeof(float), float.MinValue },
            {typeof(double), double.MinValue },
            {typeof(decimal), decimal.MinValue }
        };

        static readonly Dictionary<Type, object> maxValues = new Dictionary<Type, object>()
        {
            {typeof(sbyte), sbyte.MaxValue },
            {typeof(byte), byte.MaxValue },
            {typeof(short), short.MaxValue },
            {typeof(ushort), ushort.MaxValue },
            {typeof(int), int.MaxValue },
            {typeof(uint), uint.MaxValue },
            {typeof(long), long.MaxValue },
            {typeof(ulong), ulong.MaxValue },
            {typeof(float), float.MaxValue },
            {typeof(double), double.MaxValue },
            {typeof(decimal), decimal.MaxValue }
        };

        public static object GetMinValue(DbType dbType)
        {
            var type = dbType.ToNetType();
            return GetMinValue(type);
        }

        public static object GetMinValue(Type type)
        {
            if (minValues.ContainsKey(type))
                return minValues[type];
            return null;
        }

        public static object GetMaxValue(DbType dbType)
        {
            var type = dbType.ToNetType();
            return GetMaxValue(type);
        }

        public static object GetMaxValue(Type type)
        {
            if (maxValues.ContainsKey(type))
                return maxValues[type];
            return null;
        }
        #endregion


        #region Conversion Helpers
        public static object ChangeType(Object v, Type destType, bool force = false, uint arraySizeOneDimension = 0)
        {
            object value = v;

            if (arraySizeOneDimension > 0)
            {
                var array = Array.CreateInstance(destType, arraySizeOneDimension);
                var defaultValues = GetDefaultValues(value as String);
                if (defaultValues != null)
                {
                    for (int ii = 0; ii < array.Length; ii++)
                    {
                        if (ii < defaultValues.Length)
                        {
                            var defaultValue = ChangeType(defaultValues[ii], destType, force: false);
                            array.SetValue(defaultValue, ii);
                        }
                    }
                }
                else
                {
                    var defaultValue = ChangeType(value, destType, force: false);
                    if (defaultValue != null)
                    {
                        for (int ii = 0; ii < array.Length; ii++)
                            array.SetValue(defaultValue, ii);
                    }
                }

                return new Opc.Ua.Variant(array).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (value != null)
            {
                try
                {
                    var sourceType = v.GetType();
                    if (destType == typeof(Boolean))
                        return ToBoolean(v, sourceType);
                    else if (destType == typeof(SByte))
                        return ToSByte(v, sourceType);
                    else if (destType == typeof(Byte))
                        return ToByte(v, sourceType);
                    else if (destType == typeof(Int16))
                        return ToInt16(v, sourceType);
                    else if (destType == typeof(UInt16))
                        return ToUInt16(v, sourceType);
                    else if (destType == typeof(Int32))
                        return ToInt32(v, sourceType);
                    else if (destType == typeof(UInt32))
                        return ToUInt32(v, sourceType);
                    else if (destType == typeof(Int64))
                        return ToInt64(v, sourceType);
                    else if (destType == typeof(UInt64))
                        return ToUInt64(v, sourceType);
                    else if (destType == typeof(Decimal))
                        return ToDecimal(v, sourceType);
                    else if (destType == typeof(Single))
                        return ToSingle(v, sourceType);
                    else if (destType == typeof(Double))
                        return ToDouble(v, sourceType);
                    else if (destType == typeof(String))
                        return ToString(v, sourceType);
                    else if (destType == typeof(DateTime))
                        return ToDateTime(v, sourceType);
                    else if (destType == typeof(DateTimeOffset))
                        return ToDateTimeOffset(v, sourceType);
                    else if (destType == typeof(Guid))
                        return ToGuid(v, sourceType);
                    else if (destType == typeof(TimeSpan))
                        return ToTimeSpan(v, sourceType);
                }
                catch
                {
                    if (!force)
                        return null;
                }
            }

            if (force)
            {
                if (destType == typeof(Boolean))
                    return Boolean.Parse(Boolean.FalseString);
                else if (destType == typeof(SByte))
                    return SByte.Parse("0");
                else if (destType == typeof(Byte))
                    return Byte.Parse("0");
                else if (destType == typeof(Int16))
                    return Int16.Parse("0");
                else if (destType == typeof(UInt16))
                    return UInt16.Parse("0");
                else if (destType == typeof(Int32))
                    return Int32.Parse("0");
                else if (destType == typeof(UInt32))
                    return UInt32.Parse("0");
                else if (destType == typeof(Int64))
                    return Int64.Parse("0");
                else if (destType == typeof(UInt64))
                    return UInt64.Parse("0");
                else if (destType == typeof(Decimal))
                    return Decimal.Parse("0");
                else if (destType == typeof(Single))
                    return Single.Parse("0");
                else if (destType == typeof(Double))
                    return Double.Parse("0");
                else if (destType == typeof(String))
                    return String.Empty;
                else if (destType == typeof(DateTime))
                    return DateTime.MinValue;
                else if (destType == typeof(DateTimeOffset))
                    return DateTimeOffset.MinValue;
                else if (destType == typeof(Guid))
                    return Guid.NewGuid();
                else if (destType == typeof(TimeSpan))
                    return new TimeSpan(0,0,0,0);
            }

            return value;
        }

        public static object ChangeType(Object v, DbType dbType, bool force = false)
        {
            var value = ChangeType(v, dbType.ToNetType(), force);
            if (value is Single && (Single.IsNaN((Single)value) || Single.IsInfinity((Single)value)))
                value = null;
            if (value is Double && (Double.IsNaN((Double)value) || Double.IsInfinity((Double)value)))
                value = null;
            if (value != null)
                return value;

            return System.DBNull.Value;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Converts a value to a Boolean
        /// </summary>
        private static bool ToBoolean(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Boolean": return (bool)value;
                case "SByte": return Convert.ToBoolean((sbyte)value);
                case "Byte": return Convert.ToBoolean((byte)value);
                case "Int16": return Convert.ToBoolean((short)value);
                case "UInt16": return Convert.ToBoolean((ushort)value);
                case "Int32": return Convert.ToBoolean((int)value);
                case "UInt32": return Convert.ToBoolean((uint)value);
                case "Int64": return Convert.ToBoolean((long)value);
                case "UInt64": return Convert.ToBoolean((ulong)value);
                case "Decimal": return Convert.ToBoolean((decimal)value);
                case "Single": return Convert.ToBoolean((float)value);
                case "Double": return Convert.ToBoolean((double)value);
                case "String": return Convert.ToBoolean((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a SByte
        /// </summary>
        private static sbyte ToSByte(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "SByte": return (sbyte)value;
                case "Boolean": return Convert.ToSByte((bool)value);
                case "Byte": return (sbyte)((byte)value);
                case "Int16": return (sbyte)((short)value);
                case "UInt16": return (sbyte)((ushort)value);
                case "Int32": return (sbyte)((int)value);
                case "UInt32": return (sbyte)((uint)value);
                case "Int64": return (sbyte)((long)value);
                case "UInt64": return (sbyte)((ulong)value);
                case "Decimal": return Convert.ToSByte((decimal)value);
                case "Single": return Convert.ToSByte((float)value);
                case "Double": return Convert.ToSByte((double)value);
                case "String": return Convert.ToSByte((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Byte
        /// </summary>
        private static byte ToByte(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Byte": return (byte)value;
                case "Boolean": return Convert.ToByte((bool)value);
                case "SByte": return (byte)((sbyte)value);
                case "Int16": return (byte)((short)value);
                case "UInt16": return (byte)((ushort)value);
                case "Int32": return (byte)((int)value);
                case "UInt32": return (byte)((uint)value);
                case "Int64": return (byte)((long)value);
                case "UInt64": return (byte)((ulong)value);
                case "Decimal": return Convert.ToByte((decimal)value);
                case "Single": return Convert.ToByte((float)value);
                case "Double": return Convert.ToByte((double)value);
                case "String": return Convert.ToByte((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Int16
        /// </summary>
        private static short ToInt16(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Int16": return (short)value;
                case "Boolean": return Convert.ToInt16((bool)value);
                case "SByte": return (short)((sbyte)value);
                case "Byte": return (short)((byte)value);
                case "UInt16": return (short)((ushort)value);
                case "Int32": return (short)((int)value);
                case "UInt32": return (short)((uint)value);
                case "Int64": return (short)((long)value);
                case "UInt64": return (short)((ulong)value);
                case "Decimal": return Convert.ToInt16((decimal)value);
                case "Single": return Convert.ToInt16((float)value);
                case "Double": return Convert.ToInt16((double)value);
                case "String": return Convert.ToInt16((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a UInt16
        /// </summary>
        private static ushort ToUInt16(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "UInt16": return (ushort)value;
                case "Boolean": return Convert.ToUInt16((bool)value);
                case "SByte": return (ushort)((sbyte)value);
                case "Byte": return (ushort)((byte)value);
                case "Int16": return (ushort)((short)value);
                case "Int32": return (ushort)((int)value);
                case "UInt32": return (ushort)((uint)value);
                case "Int64": return (ushort)((long)value);
                case "UInt64": return (ushort)((ulong)value);
                case "Decimal": return Convert.ToUInt16((decimal)value);
                case "Single": return Convert.ToUInt16((float)value);
                case "Double": return Convert.ToUInt16((double)value);
                case "String": return Convert.ToUInt16((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Int32
        /// </summary>
        private static int ToInt32(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Int32": return (int)value;
                case "Boolean": return Convert.ToInt32((bool)value);
                case "SByte": return (int)((sbyte)value);
                case "Byte": return (int)((byte)value);
                case "Int16": return (int)((short)value);
                case "UInt16": return (int)((ushort)value);
                case "UInt32": return (int)((uint)value);
                case "Int64": return (int)((long)value);
                case "UInt64": return (int)((ulong)value);
                case "Decimal": return Convert.ToInt32((decimal)value);
                case "Single": return Convert.ToInt32((float)value);
                case "Double": return Convert.ToInt32((double)value);
                case "String": return Convert.ToInt32((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a UInt32
        /// </summary>
        private static uint ToUInt32(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "UInt32": return (uint)value;
                case "Boolean": return Convert.ToUInt32((bool)value);
                case "SByte": return (uint)((sbyte)value);
                case "Byte": return (uint)((byte)value);
                case "Int16": return (uint)((short)value);
                case "UInt16": return (uint)((ushort)value);
                case "Int32": return (uint)((int)value);
                case "Int64": return (uint)((long)value);
                case "UInt64": return (uint)((ulong)value);
                case "Decimal": return Convert.ToUInt32((decimal)value);
                case "Single": return Convert.ToUInt32((float)value);
                case "Double": return Convert.ToUInt32((double)value);
                case "String": return Convert.ToUInt32((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Int64
        /// </summary>
        private static long ToInt64(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Int64": return (long)value;
                case "Boolean": return Convert.ToInt64((bool)value);
                case "SByte": return (long)((sbyte)value);
                case "Byte": return (long)((byte)value);
                case "Int16": return (long)((short)value);
                case "UInt16": return (long)((ushort)value);
                case "Int32": return (long)((int)value);
                case "UInt32": return (long)((uint)value);
                case "UInt64": return (long)((ulong)value);
                case "Decimal": return Convert.ToInt64((decimal)value);
                case "Single": return Convert.ToInt64((float)value);
                case "Double": return Convert.ToInt64((double)value);
                case "String": return Convert.ToInt64((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a UInt64
        /// </summary>
        private static ulong ToUInt64(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "UInt64": return (ulong)value;
                case "Boolean": return Convert.ToUInt64((bool)value);
                case "SByte": return (ulong)((sbyte)value);
                case "Byte": return (ulong)((byte)value);
                case "Int16": return (ulong)((short)value);
                case "UInt16": return (ulong)((ushort)value);
                case "Int32": return (ulong)((int)value);
                case "UInt32": return (ulong)((uint)value);
                case "Int64": return (ulong)((long)value);
                case "Decimal": return Convert.ToUInt64((decimal)value);
                case "Single": return Convert.ToUInt64((float)value);
                case "Double": return Convert.ToUInt64((double)value);
                case "String": return Convert.ToUInt64((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Decimal
        /// </summary>
        private static Decimal ToDecimal(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Decimal": return (decimal)value;
                case "Boolean": return Convert.ToDecimal((bool)value);
                case "SByte": return Convert.ToDecimal((sbyte)value);
                case "Byte": return Convert.ToDecimal((byte)value);
                case "Int16": return Convert.ToDecimal((short)value);
                case "UInt16": return Convert.ToDecimal((ushort)value);
                case "Int32": return Convert.ToDecimal((int)value);
                case "UInt32": return Convert.ToDecimal((uint)value);
                case "Int64": return Convert.ToDecimal((long)value);
                case "UInt64": return Convert.ToDecimal((ulong)value);
                case "Single": return Convert.ToDecimal((float)value);
                case "Double": return Convert.ToDecimal((double)value);
                case "String": return Convert.ToDecimal((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Float
        /// </summary>
        private static float ToSingle(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Single": return (float)value;
                case "Boolean": return Convert.ToSingle((bool)value);
                case "SByte": return Convert.ToSingle((sbyte)value);
                case "Byte": return Convert.ToSingle((byte)value);
                case "Int16": return Convert.ToSingle((short)value);
                case "UInt16": return Convert.ToSingle((ushort)value);
                case "Int32": return Convert.ToSingle((int)value);
                case "UInt32": return Convert.ToSingle((uint)value);
                case "Int64": return Convert.ToSingle((long)value);
                case "UInt64": return Convert.ToSingle((ulong)value);
                case "Decimal": return Convert.ToSingle((decimal)value);
                case "Double": return Convert.ToSingle((double)value);
                case "String": return Convert.ToSingle((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Double
        /// </summary>
        private static double ToDouble(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Double": return (double)value;
                case "Boolean": return Convert.ToDouble((bool)value);
                case "SByte": return Convert.ToDouble((sbyte)value);
                case "Byte": return Convert.ToDouble((byte)value);
                case "Int16": return Convert.ToDouble((short)value);
                case "UInt16": return Convert.ToDouble((ushort)value);
                case "Int32": return Convert.ToDouble((int)value);
                case "UInt32": return Convert.ToDouble((uint)value);
                case "Int64": return Convert.ToDouble((long)value);
                case "UInt64": return Convert.ToDouble((ulong)value);
                case "Decimal": return Convert.ToDouble((decimal)value);
                case "Single": return Convert.ToDouble((float)value);
                case "String": return Convert.ToDouble((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a String
        /// </summary>
        private static string ToString(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "String": return (string)value;
                case "Boolean": return Convert.ToString((bool)value);
                case "SByte": return Convert.ToString((sbyte)value);
                case "Byte": return Convert.ToString((byte)value);
                case "Int16": return Convert.ToString((short)value);
                case "UInt16": return Convert.ToString((ushort)value);
                case "Int32": return Convert.ToString((int)value);
                case "UInt32": return Convert.ToString((uint)value);
                case "Int64": return Convert.ToString((long)value);
                case "UInt64": return Convert.ToString((ulong)value);
                case "Decimal": return Convert.ToString((decimal)value);
                case "Single": return Convert.ToString((float)value);
                case "Double": return Convert.ToString((double)value);
                case "DateTime": return Convert.ToString((DateTime)value);
                case "TimeSpan": return Convert.ToString((TimeSpan)value);
                case "Guid": return Convert.ToString((Guid)value);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a TimeSpan
        /// </summary>
        private static TimeSpan ToTimeSpan(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "TimeSpan": return (TimeSpan)value;
                case "String": return TimeSpan.Parse((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }


        /// <summary>
        /// Converts a value to a DateTime
        /// </summary>
        private static DateTime ToDateTime(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "DateTime": return (DateTime)value;
                case "String": return Convert.ToDateTime((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a DateTime
        /// </summary>
        private static DateTimeOffset ToDateTimeOffset(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "DateTimeOffset": return (DateTimeOffset)value;
                case "String": return DateTimeOffset.Parse((string)value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to a Guid
        /// </summary>
        private static Guid ToGuid(object value, Type sourceType)
        {
            // handle for supported conversions.
            switch (sourceType.Name)
            {
                case "Guid": return (Guid)value;
                case "String": return Guid.Parse((string)value);
            }

            // conversion not supported.
            throw new InvalidCastException();
        }

        static String[] GetDefaultValues(String value)
        {
            if (!String.IsNullOrEmpty(value) && value.StartsWith(openArrayValue) && value.EndsWith(closeArrayValue))
            {
                var values = value.Substring(1, value.Length - 2);
                return values.Split(new String[] { separatorArrayValue }, StringSplitOptions.None);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
