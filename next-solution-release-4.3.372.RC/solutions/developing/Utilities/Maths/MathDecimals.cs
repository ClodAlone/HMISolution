using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Maths
{
    public static class MathDecimals
    {
        /// <summary>
        /// Get the number of decimal digits of a floatting point value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(float value)
        {
            return GetDecimalPlaces(Convert.ToDouble(value));
        }

        /// <summary>
        /// Get the number of decimal digits of a decimal value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(decimal value)
        {
            return GetDecimalPlaces(Convert.ToDouble(value));
        }

        /// <summary>
        /// Get the number of decimal digits of a double point value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(double value)
        {
            int expValue = 0;
            var stringValue = value.ToString("R", CultureInfo.InvariantCulture);
            var indexExp = stringValue.IndexOf('E');
            if (indexExp != -1)
            {
                expValue = Convert.ToInt32(stringValue.Substring(indexExp + 1));
                stringValue = stringValue.Substring(0, indexExp);
            }
            
            var splitValue = stringValue.Split('.');
            var decimals = splitValue.Length > 1 ? splitValue[1].Length - expValue : -expValue;
            return decimals > 0 ? decimals : 0;
        }

        /// <summary>
        /// Get the string rappresantation of decimal value by checking min/max limits of the orginal type.
        /// </summary>
        /// <param name="dValue"></param>
        /// <param name="typeName"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string ToString(decimal dValue, string typeName, string format, IFormatProvider provider)
        {
            if (typeName != null)
            {
                switch (typeName)
                {
                    case "SByte":
                        if (dValue >= SByte.MaxValue)
                            return SByte.MaxValue.ToString(format, provider);
                        else if (dValue <= SByte.MinValue)
                            return SByte.MinValue.ToString(format, provider);
                        break;
                    case "Byte":
                        if (dValue >= Byte.MaxValue)
                            return Byte.MaxValue.ToString(format, provider);
                        else if (dValue <= Byte.MinValue)
                            return Byte.MinValue.ToString(format, provider);
                        break;
                    case "Int16":
                        if (dValue >= Int16.MaxValue)
                            return Int16.MaxValue.ToString(format, provider);
                        else if (dValue <= Int16.MinValue)
                            return Int16.MinValue.ToString(format, provider);
                        break;
                    case "UInt16":
                        if (dValue >= UInt16.MaxValue)
                            return UInt16.MaxValue.ToString(format, provider);
                        else if (dValue <= UInt16.MinValue)
                            return UInt16.MinValue.ToString(format, provider);
                        break;
                    case "Int32":
                        if (dValue >= Int32.MaxValue)
                            return Int32.MaxValue.ToString(format, provider);
                        else if (dValue <= Int32.MinValue)
                            return Int32.MinValue.ToString(format, provider);
                        break;
                    case "UInt32":
                        if (dValue >= UInt32.MaxValue)
                            return UInt32.MaxValue.ToString(format, provider);
                        else if (dValue <= UInt32.MinValue)
                            return UInt32.MinValue.ToString(format, provider);
                        break;
                    case "Int64":
                        if (dValue >= Int64.MaxValue)
                            return Int64.MaxValue.ToString(format, provider);
                        else if (dValue <= Int64.MinValue)
                            return Int64.MinValue.ToString(format, provider);
                        break;
                    case "UInt64":
                        if (dValue >= UInt64.MaxValue)
                            return UInt64.MaxValue.ToString(format, provider);
                        else if (dValue <= UInt64.MinValue)
                            return UInt64.MinValue.ToString(format, provider);
                        break;
                    default:
                        break;
                }
            }

            return dValue.ToString(format, provider);
        }
    }
}
