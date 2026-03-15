using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace PubNub
{
    
    //public enum PubNubErrorCodes : int
    //{
    //    ErrorCRCError = 1000,
    //    ErrorUnknownFunctionCode,
    //    ErrorReadError,
    //    ErrorReceiveFrameError,
    //    ErrorWrongSize,
    //    ErrorStationID,
    //    ErrorFunctionCode,
    //    ErrorProtocolIllegalFunction = PubNubProtocol.PROTOCOL_ERROR + 1,
    //    ErrorProtocolIllegalDataAddress = PubNubProtocol.PROTOCOL_ERROR + 2,
    //    ErrorProtocolIllegalDataValue = PubNubProtocol.PROTOCOL_ERROR + 3,
    //    ErrorProtocolSlaveDeviceFailure = PubNubProtocol.PROTOCOL_ERROR + 4,
    //    ErrorProtocolAcknowledge = PubNubProtocol.PROTOCOL_ERROR + 5,
    //    ErrorProtocolSlaveDeviceBusy = PubNubProtocol.PROTOCOL_ERROR + 6,
    //    ErrorProtocolMemoryParityError = PubNubProtocol.PROTOCOL_ERROR + 8,
    //    ErrorProtocolGatewayPathUnavailable = PubNubProtocol.PROTOCOL_ERROR + 10,
    //    ErrorProtocolGatewayTargetFailResponse = PubNubProtocol.PROTOCOL_ERROR + 11
    //}

    public enum FunctionCodes
    {
        Coils,
        DiscreteInputs,
        MultipleRegisters,
        InputRegisters,
        SingleCoil,
        SingleRegister,
        FileRecord,
        ExceptionStatus,
        MaskWriteRegister,
    }

    public class PubNubProtocol
    {
        public const int INT_AnswerLen = 13;
        public const int PROTOCOL_ERROR = 1500;

        private const uint UINT_FileReq = 10;
        private const uint UINT_RequestLen = 6;
        private const uint UINT_WriteRequestLen = 7;
        private const uint UINT_WriteFileReqLen = 10;
        #region methods override

        // Added to solve FOGBUGZ 12571
        private uint GetBitValues(List<Tag> tagList, UInt16 bitCount, ref byte[] buffer)
        {
            // Build an array of bytes with the bit values stored one bit per byte
            byte[] bitBuffer = new byte[bitCount];
            if(bitBuffer == null)
            {
                return 0;
            }
            uint processedBits = 0;
            for (int i = 0; (i < tagList.Count) && (processedBits < bitCount); i++)
            {
                uint k = tagList[i].Size;
                if ((processedBits + k) > bitCount)
                {
                    break;
                }
                object curVal = tagList[i].Value.Value;
                uint tagArrayDimension = tagList[i].TagNode.ArrayDimension;
                // Single bit
                if (tagArrayDimension == 0)
                {
                    if (Convert.ToSingle(curVal) > 0)
                    {
                        bitBuffer[processedBits++] = 1;
                    }
                    else
                    {
                        bitBuffer[processedBits++] = 0;
                    }
                }
                // Array of bits
                else
                {
                    Array b = curVal as Array;
                    if (b != null && b.GetLength(0) == tagArrayDimension)
                    {
                        for (uint j = 0; (j < tagArrayDimension); j++)
                        {
                            if (Convert.ToSingle(b.GetValue(j)) > 0)
                            {
                                bitBuffer[processedBits++] = 1;
                            }
                            else
                            {
                                bitBuffer[processedBits++] = 0;
                            }
                        }
                    }
                }
            }

            // Compact the bit values in the byte buffer passed as argument
            if (processedBits > 0)
            {   
                int byteDim = buffer.GetLength(0);
                for (int i = 0, j =0, byteIndex = 0; (i < (int)processedBits) && (byteIndex < byteDim); i++)
                {
                    if (bitBuffer[i] > 0)
                    {
                        buffer[byteIndex] += (byte)(1 << j);
                    }
                    if (++j > 7)
                    {
                        j = 0;
                        byteIndex++;
                    }
                }
            }

            return (processedBits);
       }

        public uint PrepareRequest(PubNubCommJob job, ref byte[] buffer)
        {
            return (0);
        }
        
        static int ChecKAnswer(byte[] receivebuffer, PubNubCommJob job)
        {
            return (int)DriverErrorCodes.ErrorNoError;
        }

        #endregion
        public static uint GetMaxJobSize(FunctionCodes FunctionCode, LinkType Type)
        {

            if ((Type == LinkType.Input) ||
               (FunctionCode == FunctionCodes.DiscreteInputs) ||
               (FunctionCode == FunctionCodes.InputRegisters) ||
               (FunctionCode == FunctionCodes.ExceptionStatus))
            {
                switch (FunctionCode)
                {
                    case FunctionCodes.Coils:
                    case FunctionCodes.DiscreteInputs:
                    case FunctionCodes.MultipleRegisters:
                    case FunctionCodes.InputRegisters:
                    case FunctionCodes.FileRecord:
                        return 250;
                    case FunctionCodes.SingleCoil:
                        return 250;
                    case FunctionCodes.SingleRegister:
                        return 250;
                    case FunctionCodes.ExceptionStatus:
                        return 1;
                }
            }
            else
            {
                switch (FunctionCode)
                {
                    case FunctionCodes.Coils:
                        return 100;
                    case FunctionCodes.MultipleRegisters:
                    case FunctionCodes.FileRecord:
                        return 200;
                    case FunctionCodes.SingleCoil:
                        return 100;
                    case FunctionCodes.SingleRegister:
                        return 200;
                    case FunctionCodes.MaskWriteRegister:
                        return 2;
                }
            }

            return 0;
        }

        public static bool ParseData(string receivedValue, ref PubNubCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool writeOperation = false;
            //if ((job.Status == PubNubCommJobStatus.PublishReplyReceived) ||
            //   (job.Status == PubNubCommJobStatus.PublishError))
            if(String.IsNullOrWhiteSpace(receivedValue))
            {
                writeOperation = true;
            }

            if (writeOperation)
            {
                // Done
                return (true);
            }

            job.SetJobValue(receivedValue, ref changed);
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} ParseData: processed subscription data for var {1}",
                                               currentTime, job.TagName));
#endif
            items.AddRange(changed);

            return (true);
        }

        public static bool ParseData(byte[] receivebuffer, ref PubNubCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            bool writeOperation = false;
            if ((job.Status == PubNubCommJobStatus.PublishReplyReceived) ||
               (job.Status == PubNubCommJobStatus.PublishError))
            {
                writeOperation = true;
            }

            if (areArguments)
            {
                if(items.Count == 0)
                {
                    return (false);
                }

                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger && bt != BuiltInType.String)
                {
                    return (false);
                }

                if (items.Count < (writeOperation ? 1 : 2))
                {

                    items[0] = 11; // Invalid number of arguments
                    return (false);
                }
                else
                {
                    items[0] = 0; // No error
                }
            }

            if (writeOperation)
            {
                // Done
                return (true);
            }

            job.SetJobData(receivebuffer, ref changed);
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} ParseData: processed subscription data for var {1}",
                                               currentTime, job.TagName));
#endif
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
            {
                items.AddRange(changed);
            }

            return (true);
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

