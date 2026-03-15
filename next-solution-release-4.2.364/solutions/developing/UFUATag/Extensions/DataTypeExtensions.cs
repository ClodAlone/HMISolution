using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel.Extensions
{
    public static class DataTypeExtensions
    {
        #region DataType to Type Converter
        static readonly Dictionary<DataType, Type> dbTypeToNetType = new Dictionary<DataType, Type>()
        {
            { DataType.Boolean , typeof(Boolean) },
            { DataType.SByte , typeof(SByte) },
            { DataType.Byte , typeof(Byte) },
            { DataType.Int16 , typeof(Int16) },
            { DataType.UInt16 , typeof(UInt16) },
            { DataType.Int32 , typeof(Int32) },
            { DataType.UInt32 , typeof(UInt32) },
            { DataType.Int64 , typeof(Int64) },
            { DataType.UInt64 , typeof(UInt64) },
            { DataType.Float , typeof(Single) },
            { DataType.Double , typeof(Double) },
            { DataType.String , typeof(String) }
        };

        public static Type ToNetType(this DataType dbType)
        {
            if (!dbTypeToNetType.ContainsKey(dbType))
                throw new ArgumentException(String.Format("Unsupported data type conversion ('{0}') !", dbType));

            return dbTypeToNetType[dbType];
        }

        public static Type ToNetType(this DataType? dbType)
        {
            if (!dbType.HasValue)
                throw new ArgumentException("dbType cannot be null");

            return dbType.Value.ToNetType();
        }
        #endregion

        #region BuiltInType to DataType Converter
        static readonly Dictionary<Opc.Ua.BuiltInType, DataType> builtInTypeToDataType = new Dictionary<Opc.Ua.BuiltInType, DataType>()
        {
            { Opc.Ua.BuiltInType.Boolean , DataType.Boolean },
            { Opc.Ua.BuiltInType.SByte , DataType.SByte },
            { Opc.Ua.BuiltInType.Byte , DataType.Byte },
            { Opc.Ua.BuiltInType.Int16 , DataType.Int16 },
            { Opc.Ua.BuiltInType.UInt16 , DataType.UInt16 },
            { Opc.Ua.BuiltInType.Int32 , DataType.Int32 },
            { Opc.Ua.BuiltInType.UInt32 , DataType.UInt32 },
            { Opc.Ua.BuiltInType.Int64 , DataType.Int64 },
            { Opc.Ua.BuiltInType.UInt64 , DataType.UInt64 },
            { Opc.Ua.BuiltInType.Float , DataType.Float },
            { Opc.Ua.BuiltInType.Double , DataType.Double },
            { Opc.Ua.BuiltInType.String , DataType.String }
        };

        public static DataType ToDataType(this Opc.Ua.BuiltInType builtInType)
        {
            if (!builtInTypeToDataType.ContainsKey(builtInType))
                throw new ArgumentException(String.Format("Unsupported data type conversion ('{0}') !", builtInType));

            return builtInTypeToDataType[builtInType];
        }

        public static DataType ToDataType(this Opc.Ua.BuiltInType? builtInType)
        {
            if (!builtInType.HasValue)
                throw new ArgumentException("builtInType cannot be null");

            return builtInType.Value.ToDataType();
        }
        #endregion

        public static bool IsVariableLenght(this DataType dbType)
        {
            return dbType == DataType.String;
        }

        public static bool IsVariableLenght(this DataType? dbType)
        {
            if (!dbType.HasValue)
                throw new ArgumentException("dbType cannot be null");

            return dbType.Value.IsVariableLenght();
        }

        public static bool IsMinMaxType(this DataType dbType)
        {
            return dbType == DataType.SByte ||
                dbType == DataType.Byte ||
                dbType == DataType.Int16 ||
                dbType == DataType.UInt16 ||
                dbType == DataType.Int32 ||
                dbType == DataType.UInt32 ||
                dbType == DataType.Int64 ||
                dbType == DataType.UInt64 ||
                dbType == DataType.Float ||
                dbType == DataType.Double;
        }

        public static bool IsMinMaxType(this DataType? dbType)
        {
            if (!dbType.HasValue)
                throw new ArgumentException("dbType cannot be null");

            return dbType.Value.IsMinMaxType();
        }

        public static bool IsDecimalType(this DataType dbType)
        {
            return dbType == DataType.Float ||
                dbType == DataType.Double;
        }

        public static bool IsDecimalType(this DataType? dbType)
        {
            if (!dbType.HasValue)
                throw new ArgumentException("dbType cannot be null");

            return dbType.Value.IsDecimalType();
        }

        #region Helpers

        public static object ChangeType(Object v, DataType dbType, int arraySizeOneDimension = 0, bool force = false)
        {
            object value = v;
            Type type = dbType.ToNetType();
            if (arraySizeOneDimension > 0)
            {
                if (value is Array)
                {
                    var values = value as Array;
                    var array = Array.CreateInstance(type, arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        try
                        {
                            array.SetValue(ChangeType(values.GetValue(ii), dbType), ii);
                        }
                        catch { }

                    }

                    return array;
                }
                else if (value is String &&
                    (value as String).Length > 2 && (value as String)[0] == '{' &&
                    (value as String)[(value as String).Length - 1] == '}')
                {
                    var values = (value as String).Substring(1, (value as String).Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                    var array = Array.CreateInstance(type, arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        try
                        {
                            array.SetValue(ChangeType(values[ii], dbType), ii);
                        }
                        catch { }
                    }

                    return array;
                }
            }

            try
            {
                if (value is String && type != typeof(String))
                {
                    if (String.Compare(value as String, "True", true) == 0)
                        v = 1;
                    else if (String.Compare(value as String, "False", true) == 0)
                        v = 0;
                    else
                    {
                        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                        v = Convert.ToDouble(v, info);
                    }
                }
            }
            catch { }

            try
            {
                if (type == typeof(Boolean))
                    return Convert.ToBoolean(v);
                else if (type == typeof(SByte))
                    return Convert.ToSByte(v);
                else if (type == typeof(Byte))
                    return Convert.ToByte(v);
                else if (type == typeof(Int16))
                    return Convert.ToInt16(v);
                else if (type == typeof(UInt16))
                    return Convert.ToUInt16(v);
                else if (type == typeof(Int32))
                    return Convert.ToInt32(v);
                else if (type == typeof(UInt32))
                    return Convert.ToUInt32(v);
                else if (type == typeof(Int64))
                    return Convert.ToInt64(v);
                else if (type == typeof(UInt64))
                    return Convert.ToUInt64(v);
                else if (type == typeof(Decimal))
                    return Convert.ToSingle(v);
                else if (type == typeof(Single))
                    return Convert.ToSingle(v);
                else if (type == typeof(Double))
                    return Convert.ToDouble(v);
            }
            catch (Exception ex)
            {
                if (force)
                {
                    if (type == typeof(Boolean))
                        return Boolean.Parse(Boolean.FalseString);
                    else if (type == typeof(SByte))
                        return SByte.Parse("0");
                    else if (type == typeof(Byte))
                        return Byte.Parse("0");
                    else if (type == typeof(Int16))
                        return Int16.Parse("0");
                    else if (type == typeof(UInt16))
                        return UInt16.Parse("0");
                    else if (type == typeof(Int32))
                        return Int32.Parse("0");
                    else if (type == typeof(UInt32))
                        return UInt32.Parse("0");
                    else if (type == typeof(Int64))
                        return Int64.Parse("0");
                    else if (type == typeof(UInt64))
                        return UInt64.Parse("0");
                    else if (type == typeof(Decimal))
                        return Decimal.Parse("0");
                    else if (type == typeof(Single))
                        return Single.Parse("0");
                    else if (type == typeof(Double))
                        return Double.Parse("0");
                }

                return null;
            }

            return value;
        }

        public static object ChangeType(Object v, DataType? dbType, int arraySizeOneDimension = 0, bool force = false)
        {
            if (!dbType.HasValue)
                throw new ArgumentException("dbType cannot be null");

            return ChangeType(v, dbType.Value, arraySizeOneDimension, force);
        }
        #endregion
    }
}
