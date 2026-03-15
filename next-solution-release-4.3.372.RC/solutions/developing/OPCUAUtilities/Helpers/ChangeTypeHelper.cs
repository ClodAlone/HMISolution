using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opc.Ua.Helpers
{
    public class ChangeTypeHelper
    {
        public static object CastArrayElement(object source, BuiltInType srcType, BuiltInType dstType)
        {
            return ChangeType(source, dstType);
        }

        public static object ChangeType(Object v, BuiltInType builtinType, uint arraySizeOneDimension = 0)
        {
            object value = v;
            if (arraySizeOneDimension > 0)
            {
                if (value is Array)
                {
                    var values = value as Array;
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values.GetValue(ii), builtinType), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    (value as String).Length > 2 && (value as String)[0] == '{' &&
                    (value as String)[(value as String).Length - 1] == '}')
                {
                    var values = (value as String).Substring(1, (value as String).Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values[ii], builtinType), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    builtinType == BuiltInType.Byte &&
                    (value as String).Length >= (arraySizeOneDimension * 2))
                {
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                    for (int ii = 0; ii < array.Length; ii++)
                    {
                        array.SetValue(Byte.Parse((value as String).Substring(ii * 2, 2), System.Globalization.NumberStyles.HexNumber, info), ii);
                    }

                    return array;
                }
                else
                    throw new InvalidCastException(String.Format("Cannot cast the value '{0}' to type {1}({2})", v, builtinType, arraySizeOneDimension));
            }

            try
            {
                if (value is String && builtinType != BuiltInType.String)
                {
                    if (String.Compare(value as String, "True", true) == 0)
                        v = 1;
                    else if (String.Compare(value as String, "False", true) == 0)
                        v = 0;
                    else
                    {
                        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                        info.NumberDecimalSeparator = ".";
                        info.NumberGroupSeparator = ",";
                        dynamic dValue;
                        if (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64)
                            dValue = Convert.ToDecimal(v, info);
                        else
                            dValue = Convert.ToDouble(v, info);

                        switch (builtinType)
                        {
                            case BuiltInType.SByte:
                                if (dValue > SByte.MaxValue || dValue < SByte.MinValue)
                                {
                                    var hexValue = Convert.ToInt64(dValue).ToString("X");
                                    hexValue = hexValue.Substring(hexValue.Length - 2, 2);
                                    dValue = SByte.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                                }
                                break;
                            case BuiltInType.Int16:
                                if (dValue > Int16.MaxValue || dValue < Int16.MinValue)
                                {
                                    var hexValue = Convert.ToInt64(dValue).ToString("X");
                                    hexValue = hexValue.Substring(hexValue.Length - 4, 4);
                                    dValue = Int16.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                                }
                                break;
                            case BuiltInType.Int32:
                                if (dValue > Int32.MaxValue || dValue < Int32.MinValue)
                                {
                                    var hexValue = Convert.ToInt64(dValue).ToString("X");
                                    hexValue = hexValue.Substring(hexValue.Length - 8, 8);
                                    dValue = Int32.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                                }
                                break;
                            case BuiltInType.Int64:
                                if (dValue >= Int64.MaxValue)
                                    return Int64.MaxValue;
                                else if (dValue <= Int64.MinValue)
                                    return Int64.MinValue;
                                //else if (dValue > Int64.MaxValue || dValue < Int64.MinValue)
                                //{
                                //    var hexValue = Convert.ToInt64(dValue).ToString("X");
                                //    hexValue = hexValue.Substring(hexValue.Length - 16, 16);
                                //    dValue = Int64.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                                //}
                                break;
                            case BuiltInType.Byte:
                                if (dValue < 0)
                                    dValue = dValue + Byte.MaxValue;
                                break;
                            case BuiltInType.UInt16:
                                if (dValue < 0)
                                    dValue = dValue + UInt16.MaxValue;
                                break;
                            case BuiltInType.UInt32:
                                if (dValue < 0)
                                    dValue = dValue + UInt32.MaxValue;
                                break;
                            case BuiltInType.UInt64:
                                if (dValue < 0)
                                    dValue = dValue + UInt64.MaxValue;
                                if (dValue >= UInt64.MaxValue)
                                    return UInt64.MaxValue;
                                else if (dValue <= UInt64.MinValue)
                                    return UInt64.MinValue;
                                break;
                        }

                        v = dValue;
                    }
                }
            }
            catch { }

            switch (builtinType)
            {
                case BuiltInType.Boolean: value = Convert.ToBoolean(v); break;
                case BuiltInType.SByte: value = Convert.ToSByte(v); break;
                case BuiltInType.Byte: value = Convert.ToByte(v); break;
                case BuiltInType.Int16: value = Convert.ToInt16(v); break;
                case BuiltInType.UInt16: value = Convert.ToUInt16(v); break;
                case BuiltInType.Int32: value = Convert.ToInt32(v); break;
                case BuiltInType.UInt32: value = Convert.ToUInt32(v); break;
                case BuiltInType.Int64: value = Convert.ToInt64(v); break;
                case BuiltInType.UInt64: value = Convert.ToUInt64(v); break;
                case BuiltInType.Float: value = Convert.ToSingle(v); break;
                case BuiltInType.Double: value = Convert.ToDouble(v); break;
            }

            return value;
        }

    }
}
