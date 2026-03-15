using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace SQLDriver
{

    public class SQLDriverProtocol
    {        
        #region methods override

        // Added to solve FOGBUGZ 12571
        
        #endregion
        public static bool ParseData(byte[] receivebuffer, ref SQLDriverCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            uint recivedSize = (uint)receivebuffer.Length;

            if (areArguments)
            {
                if (items.Count == 0)
                    return false;

                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                if (items.Count < 2)
                {
                    items[0] = 11;
                    return false;
                }
            }

            byte[] tempBuffer = new byte[receivebuffer.Length];
            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

            job.SetJobData(tempBuffer, ref changed);
            if (areArguments)
            {
                if (job.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < job.TagsList.Count; k++)
                    {
                        items[k + 1] = job.TagsList[k].Value.Value;
                    }
                }
            }
            else
                items.AddRange(changed);

            return true;
        }

        public static byte[] ConvertStringToByteArray(string receivedString, BuiltInType destinationValueType, ref bool valueConverted)
        {
            valueConverted = true;

            // Non empty string?
            if (String.IsNullOrWhiteSpace(receivedString))
            {
                valueConverted = false;
                return (null);
            }


            switch ((uint)destinationValueType)
            {
                case (uint)BuiltInType.Boolean:
                    return (ConvertBooleanValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Byte:
                    return (ConvertByteValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Double:
                    return (ConvertDoubleValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Float:
                    return (ConvertFloatValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Int16:
                    return (ConvertInt16ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Int32:
                    return (ConvertInt32ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Int64:
                    return (ConvertInt64ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.Integer:
                    return (ConvertIntegerValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.SByte:
                    return (ConvertSByteValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.UInt16:
                    return (ConvertUInt16ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.UInt32:
                    return (ConvertUInt32ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.UInt64:
                    return (ConvertUInt64ValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.UInteger:
                    return (ConvertUIntegerValueToByteArray(receivedString, ref valueConverted));
                case (uint)BuiltInType.String:
                    return (ConvertStringValueToByteArray(receivedString, ref valueConverted));
                default:
                    valueConverted = false;
                    return (null);
            }
        }

        public static byte[] ConvertUIntegerValueToByteArray(string receivedString, ref bool valueConverted)
        {
            uint result = 0;
            valueConverted = true;
            if (uint.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertUInt64ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            UInt64 result = 0;
            valueConverted = true;
            if (UInt64.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertUInt32ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            UInt32 result = 0;
            valueConverted = true;
            if (UInt32.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertUInt16ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            UInt16 result = 0;
            valueConverted = true;
            if (UInt16.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertSByteValueToByteArray(string receivedString, ref bool valueConverted)
        {
            SByte result = 0;
            valueConverted = true;
            if (SByte.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertIntegerValueToByteArray(string receivedString, ref bool valueConverted)
        {
            int result = 0;
            valueConverted = true;
            if (int.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertInt64ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            Int64 result = 0;
            valueConverted = true;
            if (Int64.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertInt32ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            Int32 result = 0;
            valueConverted = true;
            if (Int32.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertInt16ValueToByteArray(string receivedString, ref bool valueConverted)
        {
            Int16 result = 0;
            valueConverted = true;
            if (Int16.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertFloatValueToByteArray(string receivedString, ref bool valueConverted)
        {
            float result = 0;
            valueConverted = true;
            if (float.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertDoubleValueToByteArray(string receivedString, ref bool valueConverted)
        {
            double result = 0;
            valueConverted = true;
            if (double.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertByteValueToByteArray(string receivedString, ref bool valueConverted)
        {
            byte result = 0;
            valueConverted = true;
            if (byte.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertBooleanValueToByteArray(string receivedString, ref bool valueConverted)
        {
            bool result = false;
            valueConverted = true;
            if (bool.TryParse(receivedString, out result) == true)
            {
                return (BitConverter.GetBytes(result));
            }
            else
            {
                valueConverted = false;
                return (null);
            }
        }

        public static byte[] ConvertStringValueToByteArray(string receivedString, ref bool valueConverted)
        {
            valueConverted = true;
            int stringLength = receivedString.Length;
            if(stringLength <= 0)
            {
                valueConverted = false;
                return (null);
            }

            byte[] byteArray = new byte[stringLength];

            for(int i=0; i<stringLength; i++)
            {
                byteArray[i] = (byte)receivedString[i];
            }

            return (byteArray);
        }

    }
}

