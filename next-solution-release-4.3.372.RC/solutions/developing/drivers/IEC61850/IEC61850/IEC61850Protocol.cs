using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Text;

namespace IEC61850
{
    public enum FunctionalConstraints
    {
        None = 0,
        ST = 1,
        MX = 2,
        SG = 3,
        CO = 4,
        SP = 5,
        SV = 6,
        CF = 7,
        DC = 8,
        RP = 9,
        BR = 10
    }

    public enum MMSDataTypes
    {
        Boolean = 0,
        Integer8Bits = 1,
        UnsignedInteger8Bits = 2,
        Integer16Bits = 3,
        UnsignedInteger16Bits = 4,
        Integer32Bits = 5,
        UnsignedInteger32Bits = 6,
        FloatingPoint32Bits = 7,
        FloatingPoint64Bits = 8,
        BitString = 9,
        OctetString = 10,
        VisibleString = 11,
        MMSString = 12,
        BinaryTime = 13,
        UTCTime = 14,
        Structure = 15
    }

    public enum ReportTypes
    {
        None = 99,
        Unbuffered = 0,
        Buffered = 1        
    }

    public enum IEC61850ErrorCodes : int
    {
        ErrorConnectFailed = 1000,
        ErrorUnexpectedReply,
        ErrorParsingError,
        ErrorTimeout,
        ErrorDataAccessError = 10000
    }

    public enum RequestTypes
    {
        ReadValue = 0,
        ReadDataSet = 1,
        ReadDataSetDirectory = 2,
        ReadVariableAccessAttributes = 3,
        ReadIdentify = 4,
        WriteValue = 5
    };

    public enum ReplyType
    {
        Unrecognized,
        ConfirmedResponse,
        Unconfirmed
    };

    // CIEC61850Report command target
    public enum Mask_OptFlds : ushort
    {
        Reserved = 0x8000,
        SequenceNumber = 0x4000,
        ReportTimeStamp = 0x2000,
        ReasonForInclusion = 0x1000,
        DataSetName = 0x0800,
        DataReference = 0x0400,
        BufferOverflow = 0x0200,
        EntryID = 0x0100,
        ConfRevision = 0x0080,
        Segmentation = 0x0040
    };

    public class IEC61850Protocol
    {
        public const int INT_AnswerLen = 13;
        public const int PROTOCOL_ERROR = 1500;
        public const ushort COTP_MAX_TPDU_SIZE = 8192;
        public const byte MAX_SELECTOR_BYTE_SIZE = 50;
        public const byte MAX_OID_BYTE_SIZE = 10;
        public const byte MAX_DATASET_NESTINGLEVEL = 32;
        public const byte MAX_STRING_LENGTH = 255;        

        private const uint UINT_FileReq = 10;
        private const uint UINT_RequestLen = 6;
        private const uint UINT_WriteRequestLen = 7;
        private const uint UINT_WriteFileReqLen = 10;
                
        #region methods

        public byte[] PrepareRequest(IEC61850CommJob job, uint invokeID)
        {
            if (job == null || (job.Station as IEC61850Station == null))
            {
                return (null);
            }

            if (job.ReadRequest())
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - PrepareRequest called for Read Request");
                return (BuildReadRequest(job, invokeID));
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - PrepareRequest called for Write Request");
                return (BuildWriteRequest(job, invokeID));
            }
        }

        public byte[] BuildWriteRequest(IEC61850CommJob job, uint invokeID)
        {
            byte[] mmsRequestBuffer = null;
            if (job.MMSDataType != MMSDataTypes.Structure)
                mmsRequestBuffer = BuildMMSWriteRequest(job, invokeID);
            //else --> future not yet supported by driver
            //{
            //    nMessageLength = BuildMMSStructuredWriteRequest(pRequestBuffer, pJob,lpBuffer, nData,nStructureDataSize);
            //}

            if (mmsRequestBuffer == null)
                return (null);

            job.RequestType = RequestTypes.WriteValue;

            // Add the Presentation Layer
            byte[] presentationRequestBuffer = AddPresentationReadRequest(mmsRequestBuffer);

            // Add the Session Layer
            byte[] sessionRequestBuffer = AddSessionReadRequest(presentationRequestBuffer);

            // Add the COTP header
            byte[] cotpRequestBuffer = AddCOTPHeader(sessionRequestBuffer);

            // Add the ISO header
            byte[] isoRequestBuffer = AddISOHeader(cotpRequestBuffer);

            return (isoRequestBuffer);
        }

        public byte[] BuildMMSWriteRequest(IEC61850CommJob job, uint invokeID)
        {
            // Calculate the message length            
            int encodedDataLength = job.CalculateWriteDataMMSLength();

            // discard write request because data is not change for inputoutput / exception output
            if (encodedDataLength == -1)
            {
                // excute GetJobData to remove pending writing value
                object writingData = null;
                job.GetJobData(ref writingData);
                return (null);
            }

            // encodedDataLength == 0 is valid only for string
            if (encodedDataLength == 0 && !IEC61850DynTagSettings.IsMMSDataStringType(job.MMSDataType))
                return (null);
            
            uint dataLength = (uint)encodedDataLength + 1 + BerEncoderCheckLengthSize((uint)encodedDataLength);
            uint dataTagLength = dataLength + 1 + BerEncoderCheckLengthSize(dataLength);
            // Item ID
            uint itemIdLength = (uint)job.MMSDataItemId.Length;
            uint varSpecificLength = itemIdLength + BerEncoderCheckLengthSize(itemIdLength) + 1;
            // Domain ID
            uint domainIdLength = (uint)job.LogicalDeviceName.Length;
            varSpecificLength += domainIdLength + BerEncoderCheckLengthSize(domainIdLength) + 1;
            uint listOfVarItemLength = varSpecificLength + BerEncoderCheckLengthSize(varSpecificLength) + 1;
            uint listOfVarLength = listOfVarItemLength + BerEncoderCheckLengthSize(listOfVarItemLength) + 1;
            uint writeServiceLength = listOfVarLength + BerEncoderCheckLengthSize(listOfVarLength) + 1;
            uint confirmedServiceLength = writeServiceLength + BerEncoderCheckLengthSize(writeServiceLength) + 1;
            uint serviceLength = confirmedServiceLength + dataTagLength;
            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS message
            // MMS confirmed write request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength);
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);
            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;
            // Write service
            requestBuffer[messageLength++] = 0xa5;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, writeServiceLength);
            requestBuffer[messageLength++] = 0x30;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarItemLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, varSpecificLength);
            // Domain ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, domainIdLength);
            for (int i = 0; i < (int)domainIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)job.LogicalDeviceName[i];
            }

            // Item ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, itemIdLength);
            for (int i = 0; i < (int)itemIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)job.MMSDataItemId[i];
            }

            // Data
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, dataLength);
            if (!AddWriteData(ref requestBuffer, ref messageLength, job, (uint)encodedDataLength))
            {
                return (null);
            }

            return (requestBuffer);
        }

        public bool AddWriteData(ref byte[] requestBuffer, ref uint requestLength, IEC61850CommJob job, uint encodedDataLength)
        {
            // Get the data to be written
            object writingData = null;
            job.GetJobData(ref writingData);
            lock (job.retLockList())
            {
                if ((job.TagsListOnWriting.Count == 0) || (writingData == null))
                {
                    // System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - BuildMMSWriteRequest: nothing to write");
                    return (false);
                }
            }

            byte[] dataBuffer = (byte[])writingData;
            int dataBufferLength = dataBuffer.Length;
            switch (job.MMSDataType)
            {
                case MMSDataTypes.Boolean:
                    {
                        byte byteAux = 0;
                        if (dataBuffer[0] != 0)
                        {
                            byteAux = 1;
                            //byteAux = 0xff;
                        }
                        requestBuffer[requestLength++] = 0x83;
                        requestBuffer[requestLength++] = 1;
                        requestBuffer[requestLength++] = byteAux;
                    }
                    break;

                case MMSDataTypes.Integer8Bits:
                    requestBuffer[requestLength++] = 0x85;
                    requestBuffer[requestLength++] = 1;
                    requestBuffer[requestLength++] = dataBuffer[0];
                    break;

                case MMSDataTypes.UnsignedInteger8Bits:
                    requestBuffer[requestLength++] = 0x86;
                    requestBuffer[requestLength++] = 1;
                    requestBuffer[requestLength++] = dataBuffer[0];
                    break;

                case MMSDataTypes.Integer16Bits:
                case MMSDataTypes.Integer32Bits:
                    requestBuffer[requestLength++] = 0x85;
                    requestBuffer[requestLength++] = (byte)encodedDataLength;
                    if (encodedDataLength > 0)
                    {
                        int intAux = 0;
                        for (int i = 0; i < dataBufferLength; i++)
                        {
                            intAux |= (dataBuffer[i] << i * 8);
                        }
                        byte[] valueBuffer = AsnEncoderLong(intAux);
                        for (int i = 0; i < encodedDataLength; i++)
                        {
                            requestBuffer[requestLength++] = valueBuffer[i];
                        }
                    }
                    break;

                case MMSDataTypes.UnsignedInteger16Bits:
                case MMSDataTypes.UnsignedInteger32Bits:
                    requestBuffer[requestLength++] = 0x86;
                    requestBuffer[requestLength++] = (byte)encodedDataLength;
                    if (encodedDataLength > 0)
                    {
                        uint uintAux = 0;
                        for (int i = 0; (i < dataBufferLength); i++)
                        {
                            uintAux |= (uint)(dataBuffer[i] << i * 8);
                        }
                        byte[] valueBuffer = AsnEncoderUnsigned(uintAux);
                        for (int i = 0; i < encodedDataLength; i++)
                        {
                            requestBuffer[requestLength++] = valueBuffer[i];
                        }
                    }
                    break;

                case MMSDataTypes.FloatingPoint32Bits:
                    {
                        if (dataBufferLength >= 4)
                        {
                            byte[] auxdata = new byte[dataBufferLength];
                            Array.Copy(dataBuffer, 0, auxdata, 0, dataBufferLength);
                            byte[] valueBuffer = ReverseBytes(auxdata);
                            requestBuffer[requestLength++] = 0x87;
                            requestBuffer[requestLength++] = 5;
                            requestBuffer[requestLength++] = 8; // Exponent width
                            for (int i = 0; i < 4; i++)
                            {
                                requestBuffer[requestLength++] = valueBuffer[i];
                            }
                        }
                    }
                    break;

                case MMSDataTypes.FloatingPoint64Bits:
                    {
                        if (dataBufferLength >= 8)
                        {
                            byte[] auxdata = new byte[dataBufferLength];
                            Array.Copy(dataBuffer, 0, auxdata, 0, dataBufferLength);
                            byte[] valueBuffer = ReverseBytes(auxdata);
                            requestBuffer[requestLength++] = 0x87;
                            requestBuffer[requestLength++] = 9;
                            requestBuffer[requestLength++] = 11; // Exponent width
                            for (int i = 0; i < 8; i++)
                            {
                                requestBuffer[requestLength++] = valueBuffer[i];
                            }
                        }
                    }
                    break;

                case MMSDataTypes.BitString:
                    {                        
                        requestBuffer[requestLength++] = 0x84;
                        BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                        uint byteLength = encodedDataLength - 1;
                        if(byteLength > dataBufferLength)
                        {
                            byteLength = (uint)dataBufferLength;
                        }
                        uint padding = 0;
                        if (byteLength * 8 >= job.DataMaximumLength)
                        {
                            padding = byteLength * 8 - job.DataMaximumLength;
                        }
                        padding &= 0x07;
                        requestBuffer[requestLength++] = (byte)padding;

                        if ((uint)job.TagsList[0].TagNode.DataType.Identifier != (uint)Opc.Ua.DataTypes.String)
                        {
                            // The supervisor variable is an integer
                            if (byteLength == 1)
                            {
                                byte aux = dataBuffer[0];
                                aux <<= (byte)padding;
                                requestBuffer[requestLength++] = aux;
                            }
                            else if (byteLength == 2)
                            {
                                UInt16 aux = dataBuffer[1];
                                aux <<= 8;
                                aux += dataBuffer[0];
                                aux <<= (byte)padding;
                                requestBuffer[requestLength++] = (byte)(aux >> 8);
                                requestBuffer[requestLength++] = (byte)aux;
                            }
                            else if (byteLength <= 4)
                            {
                                uint value = 0;
                                for (int i = (int)(byteLength - 1); i >= 0; i--)
                                {
                                    value = (value << 8) | dataBuffer[i];
                                }
                                value <<= (byte)padding;
                                UInt16 aux = (UInt16)(value >> 16);
                                if (byteLength == 4)
                                {
                                    requestBuffer[requestLength++] = (byte)(aux >> 8);
                                }
                                requestBuffer[requestLength++] = (byte)aux;
                                aux = (UInt16)value;
                                requestBuffer[requestLength++] = (byte)(aux >> 8);
                                requestBuffer[requestLength++] = (byte)aux;
                            }
                        }
                        // The supervisor variable is a string composed by binary digits
                        else
                        {
                            // check that string contain only valid chars (0 or 1) and that length match with settings
                            if (dataBuffer.Count(b => (b == 0x30 || b == 0x31)) != job.DataMaximumLength)
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - BuildMMSWriteRequest: nothing to write");
                                return (false);
                            }

                            byte byteValue = 0;
                            uint bitIndexLimit = 0;
                            uint j = 0;
                            for (uint i = 0, bitIndex = 0; (i < byteLength) && (bitIndex < dataBufferLength);
                                i++)
                            {
                                byteValue = 0;
                                if (i == (byteLength - 1))
                                {
                                    bitIndexLimit = padding;
                                }
                                for (j = 7; j >= bitIndexLimit; j--, bitIndex++)
                                {
                                    if (dataBuffer[bitIndex] == 0)
                                    {
                                        break;
                                    }
                                    else if (dataBuffer[bitIndex] != 0x30)
                                    {
                                        byteValue |= (byte)(1 << (byte)j);
                                    }
                                }
                                requestBuffer[requestLength++] = byteValue;
                            }
                        }

                        byte paddingMask = 0;
                        for (uint i = 0; i < padding; i++)
                        {
                            paddingMask += (byte)(1 << (byte)i);
                        }
                        paddingMask = (byte)~paddingMask;
                        requestBuffer[requestLength - 1] &= paddingMask;
                    }
                    break;

                case MMSDataTypes.OctetString:
                    requestBuffer[requestLength++] = 0x89;
                    BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                    if (encodedDataLength > 0)
                    {
                        int charIndex = 0;
                        uint byteValue = 0;
                        string stringByteValue = String.Empty;
                        for (int i = 0; (i < encodedDataLength) && (charIndex < (int)dataBufferLength); i++)
                        {
                            byteValue = 0;
                            stringByteValue = String.Empty;
                            while (dataBuffer[charIndex] != 0)
                            {
                                if (dataBuffer[charIndex] != 0x20)
                                {
                                    stringByteValue += (char)dataBuffer[charIndex];
                                    charIndex++;
                                }
                                else
                                {
                                    charIndex++;
                                    break;
                                }
                            }
                            if (!String.IsNullOrWhiteSpace(stringByteValue))
                            {
                                bool isHexadecimalValue = false;
                                for(int j=0; j < stringByteValue.Length; j++)
                                {
                                    if(!IsHexadecimalDigit(stringByteValue[j]))
                                    {
                                        isHexadecimalValue = false;
                                        break;
                                    }
                                    else
                                    {
                                        isHexadecimalValue = true;
                                    }
                                }
                                if(isHexadecimalValue)
                                {
                                    byteValue = Convert.ToByte(stringByteValue, 16);
                                }
                                requestBuffer[requestLength++] = (byte)byteValue;
                            }
                        }
                    }
                    break;

                case MMSDataTypes.VisibleString:
                    requestBuffer[requestLength++] = 0x8a;
                    BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                    if (encodedDataLength > 0)
                    {
                        for (int i = 0; (i < encodedDataLength) && (i < dataBufferLength); i++)
                        {
                            requestBuffer[requestLength++] = dataBuffer[i];
                        }
                    }
                    break;

                case MMSDataTypes.MMSString:
                    requestBuffer[requestLength++] = 0x90;
                    BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                    if (encodedDataLength > 0)
                    {
                        for (int i = 0; (i < encodedDataLength) && (i < dataBufferLength); i++)
                        {
                            requestBuffer[requestLength++] = dataBuffer[i];
                        }
                    }
                    break;

                case MMSDataTypes.UTCTime:
                    {
                        requestBuffer[requestLength++] = 0x91;
                        BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                        byte[] utcDataBuffer = new byte[encodedDataLength];
                        string timeValue = String.Empty; ;
                        for (int i = 0; (i < dataBufferLength) && (dataBuffer[i]) != 0; i++)
                        {
                            timeValue += (char)dataBuffer[i];
                        }
                        if (!String.IsNullOrWhiteSpace(timeValue))
                        {
                            ConvertASCIIStringToUTCTime(timeValue, ref utcDataBuffer, encodedDataLength);
                        }
                        for (int i = 0; i < encodedDataLength; i++)
                        {
                            requestBuffer[requestLength++] = utcDataBuffer[i];
                        }
                    }
                    break;

                case MMSDataTypes.BinaryTime:
                    {
                        requestBuffer[requestLength++] = 0x8c;
                        BerEncoderAddLength(ref requestBuffer, ref requestLength, encodedDataLength);
                        byte[] binaryDataBuffer = new byte[encodedDataLength];
                        string timeValue = String.Empty; ;
                        for (int i = 0; (i < dataBufferLength) && (dataBuffer[i]) != 0; i++)
                        {
                            timeValue += (char)dataBuffer[i];
                        }
                        if (!String.IsNullOrWhiteSpace(timeValue))
                        {
                            ConvertASCIIStringToBinaryTime(timeValue, ref binaryDataBuffer, encodedDataLength);
                        }
                        for (int i = 0; i < encodedDataLength; i++)
                        {
                            requestBuffer[requestLength++] = binaryDataBuffer[i];
                        }
                    }
                    break;

                default: // Unsupported type?
                    return (false);
            }

            return (true);
        }

        void ConvertASCIIStringToBinaryTime(string timeString, ref byte[] timeByteBuffer, uint bufferLength)
        {
            if (String.IsNullOrWhiteSpace(timeString) || (bufferLength != 6))
            {
                return;
            }

            // Divide the string in two substrings: one for date and time and the second
            // for the milliseconds
            string milliseconds = String.Empty;
            string timeWithoutMilliseconds = String.Empty;
            int pointIndex = timeString.IndexOf('.');
            if ((pointIndex > 0) && (pointIndex < (timeString.Length - 1)))
            {
                timeWithoutMilliseconds = timeString.Substring(0, pointIndex);
                milliseconds = timeString.Substring(pointIndex + 1);
            }
            else
            {
                timeWithoutMilliseconds = timeString;
            }

            DateTime dt = DateTime.MinValue;
            try
            {
                // Set the time in a DateTime without milliseconds
                dt = DateTime.Parse(timeWithoutMilliseconds);
            }
            catch (Exception e)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ConvertASCIIStringToUTCTime: exception 1 parsing {0}", timeWithoutMilliseconds);
                return;
            }

            // Set the milliseconds in the DateTime object
            uint millisecondsValue = 0;
            if (!String.IsNullOrWhiteSpace(milliseconds))
            {
                if (uint.TryParse(milliseconds, out millisecondsValue))
                {
                    ulong ticks = (ulong)(TimeSpan.TicksPerMillisecond * millisecondsValue);
                    TimeSpan millisecondTicks = new TimeSpan((long)ticks);
                    dt += millisecondTicks;
                }
            }

            // Base Date: GMT midnight January 1, 1984
            DateTime baseDT = new DateTime(1984, 1, 1, 0, 0, 0);
            TimeSpan elapsedTime = dt - baseDT;
            // Get the number of elapsed days since GMT midnight January 1, 1984
            uint elapsedDays = (uint)elapsedTime.Days;
            // Get the number of elapsed milliseconds since GMT midnight
            TimeSpan auxTime = new TimeSpan(elapsedTime.Days, 0, 0, 0);
            TimeSpan remainingTime = elapsedTime - auxTime;
            double millisecondsFromMidnight = remainingTime.TotalMilliseconds;
            uint msSinceMidnight = (uint)millisecondsFromMidnight;

            // Copy the Binary time into the byte buffer
            UInt16 wordValue = 0;
            byte byteValue = 0;
            wordValue = (UInt16)(msSinceMidnight >> 16);
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[0] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[1] = byteValue;
            wordValue = (UInt16)msSinceMidnight;
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[2] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[3] = byteValue;
            wordValue = (UInt16)elapsedDays;
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[4] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[5] = byteValue;
        }

        void ConvertASCIIStringToUTCTime(string timeString, ref byte[] timeByteBuffer, uint bufferLength)
        {
            if (String.IsNullOrWhiteSpace(timeString) || (bufferLength != 8))
            {
                return;
            }

            // Divide the string in two substrings: one for date and time and the second
            // for the milliseconds
            string milliseconds = String.Empty;
            string timeWithoutMilliseconds = String.Empty;
            int pointIndex = timeString.IndexOf('.');
            if ((pointIndex > 0) && (pointIndex < (timeString.Length - 1)))
            {
                timeWithoutMilliseconds = timeString.Substring(0, pointIndex);
                milliseconds = timeString.Substring(pointIndex + 1);
            }
            else
            {
                timeWithoutMilliseconds = timeString;
            }

            DateTime dt = DateTime.MinValue;
            try
            {
                // Set the time in a DateTime without milliseconds
                dt = DateTime.Parse(timeWithoutMilliseconds);
            }
            catch(Exception e)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ConvertASCIIStringToUTCTime: exception 1 parsing {0}", timeWithoutMilliseconds);
                return;
            }

            // Set the milliseconds in the DateTime object
            uint millisecondsValue = 0;
            if (!String.IsNullOrWhiteSpace(milliseconds))
            {
                if(uint.TryParse(milliseconds, out millisecondsValue))
                {
                    ulong ticks = (ulong) (TimeSpan.TicksPerMillisecond * millisecondsValue);
                    TimeSpan millisecondTicks = new TimeSpan((long)ticks);
                    dt += millisecondTicks;
                }
            }

            // Copy the UTC time into the byte buffer
            // Base Date: GMT midnight January 1, 1970
            DateTime baseDT = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan elapsedTime = dt - baseDT;
            // Get the number of elapsed seconds since GMT midnight January 1, 1970
            double elapsedSeconds = elapsedTime.TotalSeconds;
            uint totalSeconds = (uint)elapsedSeconds;

            // Copy the UTC time into the byte buffer
            byte byteValue = 0;
            UInt16 wordValue = 0;
            wordValue = (UInt16)(totalSeconds >> 16);
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[0] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[1] = byteValue;
            wordValue = (UInt16)totalSeconds;
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[2] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[3] = byteValue;
            wordValue = (UInt16)(millisecondsValue >> 16);
            byteValue = (byte)wordValue;
            timeByteBuffer[4] = byteValue;
            wordValue = (UInt16)millisecondsValue;
            byteValue = (byte)(wordValue >> 8);
            timeByteBuffer[5] = byteValue;
            byteValue = (byte)wordValue;
            timeByteBuffer[6] = byteValue;

            // Set the quality byte
            timeByteBuffer[7] = 0x0a;
        }

        public byte[] BuildReadRequest(IEC61850CommJob job, uint invokeID)
        {
            // Select the type of the request
            job.RequestType = RequestTypes.ReadValue;
            RequestTypes jobReadRequestType = GetJobReadRequestType(job);
            return(BuildReadRequest(job.MMSDataItemId, job.LogicalDeviceName, jobReadRequestType, invokeID));
        }

        public byte[] BuildReadRequest(string mmsDataItemId, string LogicalDeviceName, RequestTypes requestType, uint invokeID)
        {
            // Build the MMS read request
            byte[] mmsRequestBuffer = null;
            switch (requestType)
            {
                case RequestTypes.ReadDataSet:
                    break;

                case RequestTypes.ReadDataSetDirectory:
                    mmsRequestBuffer = BuildMMSReadDataSetDirectoryRequest(mmsDataItemId, LogicalDeviceName, invokeID);
                    break;

                case RequestTypes.ReadVariableAccessAttributes:
                    mmsRequestBuffer = BuildMMSReadVariableAccessAttributesRequest(mmsDataItemId, LogicalDeviceName, invokeID);
                    break;

                case RequestTypes.ReadIdentify:
                    mmsRequestBuffer = BuildMMSReadIdentifyRequest(invokeID);
                    break;

                default: // RequestTypes.ReadValue
                    mmsRequestBuffer = BuildMMSReadRequest(mmsDataItemId, LogicalDeviceName, invokeID);
                    //job.RequestType = RequestTypes.ReadValue;
                    break;
            }
            if (mmsRequestBuffer == null)
            {
                return (null);
            }

            // Add the Presentation Layer
            byte[] presentationRequestBuffer = AddPresentationReadRequest(mmsRequestBuffer);

            // Add the Session Layer
            byte[] sessionRequestBuffer = AddSessionReadRequest(presentationRequestBuffer);

            // Add the COTP header
            byte[] cotpRequestBuffer = AddCOTPHeader(sessionRequestBuffer);

            // Add the ISO header
            byte[] isoRequestBuffer = AddISOHeader(cotpRequestBuffer);

            return (isoRequestBuffer);
        }

        byte[] BuildMMSReadVariableAccessAttributesRequest(string mmsDataItemId, string logicalDeviceName, uint invokeID)
        {
            // Calculate the message length
            // Item ID
            uint itemIdLength = (uint)mmsDataItemId.Length;
            uint varSpecificLength = itemIdLength + BerEncoderCheckLengthSize(itemIdLength) + 1;
            // Domain ID
            uint domainIdLength = (uint)logicalDeviceName.Length;
            varSpecificLength += domainIdLength + BerEncoderCheckLengthSize(domainIdLength) + 1;
            uint nVarNameLength = varSpecificLength + BerEncoderCheckLengthSize(varSpecificLength) + 1;
            uint serviceLength = nVarNameLength + BerEncoderCheckLengthSize(nVarNameLength) + 1;
            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS confirmed read request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength);
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;

            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);
            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;

            // GetVariableAccessAttributes service
            requestBuffer[messageLength++] = 0xa6;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, nVarNameLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, varSpecificLength);

            // Domain ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, domainIdLength);
            for (int i = 0; i < (int)domainIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)logicalDeviceName[i];
            }

            // Item ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, itemIdLength);
            for (int i = 0; i < (int)itemIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)mmsDataItemId[i];
            }

            return (requestBuffer);
        }

        byte[] BuildMMSReadDataSetDirectoryRequest(string mmsDataItemId, string logicalDeviceName, uint invokeID)
        {
            // Calculate the message length
            // Item ID
            uint itemIdLength = (uint)mmsDataItemId.Length;
            uint varSpecificLength = itemIdLength + BerEncoderCheckLengthSize(itemIdLength) + 1;
            // Domain ID
            uint domainIdLength = (uint)logicalDeviceName.Length;
            varSpecificLength += domainIdLength + BerEncoderCheckLengthSize(domainIdLength) + 1;
            uint serviceLength = varSpecificLength + BerEncoderCheckLengthSize(varSpecificLength) + 1;
            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS confirmed read request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength);
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;

            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);
            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;

            // GetNamedVariableListAttributes service
            requestBuffer[messageLength++] = 0xac;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, varSpecificLength);

            // Domain ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, domainIdLength);
            for (int i = 0; i < (int)domainIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)logicalDeviceName[i];
            }

            // Item ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, itemIdLength);
            for (int i = 0; i < (int)itemIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)mmsDataItemId[i];
            }

            return (requestBuffer);
        }

        byte[] BuildMMSReadRequest(string mmsDataItemId, string logicalDeviceName, uint invokeID)
        {
            // Calculate the message length
            // Item ID
            uint itemIdLength = (uint)mmsDataItemId.Length;
            uint varSpecificLength = itemIdLength + BerEncoderCheckLengthSize(itemIdLength) + 1;
            // Domain ID
            uint domainIdLength = (uint)logicalDeviceName.Length;
            varSpecificLength += domainIdLength + BerEncoderCheckLengthSize(domainIdLength) + 1;
            uint listOfVarItemLength = varSpecificLength + BerEncoderCheckLengthSize(varSpecificLength) + 1;
            uint listOfVarLength = listOfVarItemLength + BerEncoderCheckLengthSize(listOfVarItemLength) + 1;
            uint readServiceLength = listOfVarLength + BerEncoderCheckLengthSize(listOfVarLength) + 1;
            uint confirmedServiceLength = readServiceLength + BerEncoderCheckLengthSize(readServiceLength) + 1;
            uint serviceLength = confirmedServiceLength + BerEncoderCheckLengthSize(confirmedServiceLength) + 1;
            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS confirmed read request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength);
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;

            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);
            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;
            requestBuffer[messageLength++] = 0xa4;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, confirmedServiceLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, readServiceLength);
            requestBuffer[messageLength++] = 0x30;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarItemLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, varSpecificLength);
            // Domain ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, domainIdLength);
            for (int i = 0; i < (int)domainIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)logicalDeviceName[i];
            }
            // Item ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, itemIdLength);
            for (int i = 0; i < (int)itemIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)mmsDataItemId[i];
            }

            return (requestBuffer);
        }

        byte[] BuildMMSReadIdentifyRequest(uint invokeID)
        {
            // Calculate the message length
            uint serviceLength = 0;
            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS confirmed read request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength);
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;

            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);
            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;
            requestBuffer[messageLength++] = 0x82;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);

            return (requestBuffer);
        }

        void AddRCBItemValueToWriteRequest(ref byte[] pRequestBuffer, MMSDataTypes nItemType,uint nEncodedDataLength,byte nPadding, byte[] pDataBuffer, uint nDataOffset)
        {
            switch (nItemType)
            {
                case MMSDataTypes.Boolean:
                    pRequestBuffer[nDataOffset++] = 0x83;
                    pRequestBuffer[nDataOffset++] = 1;
                    pRequestBuffer[nDataOffset++] = (pDataBuffer[0] != 0 ? (byte)0xff : (byte)0);
                    break;

                case MMSDataTypes.BitString:
                    {
                        pRequestBuffer[nDataOffset++] = 0x84;
                        pRequestBuffer[nDataOffset++] = (byte)nEncodedDataLength;
                        pRequestBuffer[nDataOffset++] = nPadding;
                        for (uint i = 0; i < nEncodedDataLength - 1; i++)
                            pRequestBuffer[nDataOffset++] = pDataBuffer[i];
                    }
                    break;

                case MMSDataTypes.VisibleString:
                    {
                        pRequestBuffer[nDataOffset++] = 0x8a;
                        pRequestBuffer[nDataOffset++] = (byte)nEncodedDataLength;                        
                        for (uint i = 0; i < nEncodedDataLength; i++)
                            pRequestBuffer[nDataOffset++] = pDataBuffer[i];
                    }
                    break;
            }
        }


        byte[] BuildMMSWriteRCBItemRequest(string szDeviceID,string szItemID, MMSDataTypes nItemType, byte[] pDataBuffer, byte nPadding, uint invokeID)
        {
            // Calculate the message length
            // Data
            uint nEncodedDataLength = (uint)pDataBuffer.Length;
            if (nItemType == MMSDataTypes.BitString)
            {
                nEncodedDataLength++; // Padding byte
            }
            uint nDataLength = nEncodedDataLength + 1 + BerEncoderCheckLengthSize(nEncodedDataLength);
            uint nDataTagLength = nDataLength + 1 + BerEncoderCheckLengthSize(nDataLength);
            // Item ID
            uint itemIdLength = (uint)szItemID.Length;
            uint varSpecificLength = itemIdLength + BerEncoderCheckLengthSize(itemIdLength) + 1;

            // Domain ID
            uint domainIdLength = (uint)szDeviceID.Length;
            varSpecificLength += domainIdLength + BerEncoderCheckLengthSize(domainIdLength) + 1;
            uint listOfVarItemLength = varSpecificLength + BerEncoderCheckLengthSize(varSpecificLength) + 1;
            uint listOfVarLength = listOfVarItemLength + BerEncoderCheckLengthSize(listOfVarItemLength) + 1;
            uint writeServiceLength = listOfVarLength + BerEncoderCheckLengthSize(listOfVarLength) + 1;
            uint confirmedServiceLength = writeServiceLength + BerEncoderCheckLengthSize(writeServiceLength) + 1;
            uint serviceLength = confirmedServiceLength + nDataTagLength;

            // Invoke ID
            byte[] invokeIDBuffer = AsnEncoderLong((int)invokeID);
            uint invokeIdLength = (uint)invokeIDBuffer.Length;
            uint pduLength = serviceLength + BerEncoderCheckLengthSize(serviceLength) + 1;
            pduLength += invokeIdLength + BerEncoderCheckLengthSize(invokeIdLength) + 1;

            // Build the MMS confirmed read request
            uint messageTotalLength = 1 + pduLength + BerEncoderCheckLengthSize(pduLength); // not present in Mov11
            byte[] requestBuffer = new byte[messageTotalLength]; // not present in Mov11

            // MMS confirmed write request
            uint messageLength = 0;
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, pduLength);

            // Invoke ID
            requestBuffer[messageLength++] = 0x02;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, invokeIdLength);
            Array.Copy(invokeIDBuffer, 0, requestBuffer, messageLength, invokeIdLength);
            messageLength += invokeIdLength;

            // Write service
            requestBuffer[messageLength++] = 0xa5;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, serviceLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, writeServiceLength);
            requestBuffer[messageLength++] = 0x30;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarLength);
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, listOfVarItemLength);
            requestBuffer[messageLength++] = 0xa1;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, varSpecificLength);

            // Domain ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, domainIdLength);
            for (int i = 0; i < (int)domainIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)szDeviceID[i];
            }

            // Item ID
            requestBuffer[messageLength++] = 0x1a;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, itemIdLength);
            for (int i = 0; i < (int)itemIdLength; i++)
            {
                requestBuffer[messageLength++] = (byte)szItemID[i];
            }

            // Data
            requestBuffer[messageLength++] = 0xa0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, nDataLength);            
            AddRCBItemValueToWriteRequest(ref requestBuffer, nItemType, nEncodedDataLength, nPadding, pDataBuffer, messageLength);

            return (requestBuffer);
        }

        public byte[] BuildWriteRCBItemRequest(string szDeviceID, string szItemID, MMSDataTypes nItemType, byte[] pDataBuffer, byte nPadding, uint invokeID)
        {
            byte[] RCBRequestBuffer = BuildMMSWriteRCBItemRequest(szDeviceID, szItemID,nItemType, pDataBuffer, nPadding, invokeID);
            if (RCBRequestBuffer == null)
                return null;

            // Add the Presentation Layer
            byte[] presentationRequestBuffer = AddPresentationReadRequest(RCBRequestBuffer);

            // Add the Session Layer
            byte[] sessionRequestBuffer = AddSessionReadRequest(presentationRequestBuffer);

            // Add the COTP header
            byte[] cotpRequestBuffer = AddCOTPHeader(sessionRequestBuffer);

            // Add the ISO header
            byte[] isoRequestBuffer = AddISOHeader(cotpRequestBuffer);

            return (isoRequestBuffer);
        }

        void BerEncoderAddLength(ref byte[] buffer, ref uint bufferSize, uint length)
        {
            byte[] lengthBuffer = BerEncoderLength((int)length);
            Array.Copy(lengthBuffer, 0, buffer, bufferSize, lengthBuffer.Length);
            bufferSize += (uint)lengthBuffer.Length;
        }

        RequestTypes GetJobReadRequestType(IEC61850CommJob job)
        {
            RequestTypes requestType = RequestTypes.ReadValue;
            if((job.FunctionalConstraint == FunctionalConstraints.None) && (job.MMSDataType == MMSDataTypes.Structure))
            {
                requestType = RequestTypes.ReadDataSet;
            }
            return (requestType);
        }

        public uint CheckMMSInitiatePDU(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked)
        {
            if (bytesToBeChecked < 1)
            {
                return (0);
            }

            uint checkedBytes = 0;
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0xa9)
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (length != (bytesToBeChecked - checkedBytes))
            {
                return (0);
            }

            checkedBytes += length;

            return (checkedBytes);
        }

        public uint CheckAssociationControlService(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked)
        {
            if (bytesToBeChecked < 1)
            {
                return (0);
            }

            uint checkedBytes = 0;
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0x61)
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }
            if (length != (bytesToBeChecked - checkedBytes))
            {
                return (0);
            }

            // Check PDU items
            bool upperLayerData = false;
            uint itemLength = 0;
            while ((bytesToBeChecked > checkedBytes) && !upperLayerData)
            {
                switch (replyMessage[totalCheckedBytes + checkedBytes++])
                {
                    case 0xa2:
                        if (bytesToBeChecked <= checkedBytes)
                        {
                            return (0);
                        }

                        itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                        if (bytesToBeChecked < (checkedBytes + itemLength))
                        {
                            return (0);
                        }
                        if (replyMessage[totalCheckedBytes + checkedBytes + itemLength - 1] != 0)
                        {
                            return (0);
                        }
                        checkedBytes += itemLength;
                        break;

                    case 0xbe:
                    case 0x28:
                        {
                            if (bytesToBeChecked <= checkedBytes)
                            {
                                return (0);
                            }
                            uint itemLengthSize = 0;
                            itemLength = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out itemLengthSize);
                            if (itemLength == 0)
                            {
                                return (0);
                            }
                            checkedBytes += itemLengthSize;

                            if (bytesToBeChecked <= checkedBytes)
                            {
                                return (0);
                            }
                            if (itemLength != (bytesToBeChecked - checkedBytes))
                            {
                                return (0);
                            }
                        }
                        break;

                    // User data
                    case 0xa0:
                        {
                            if (bytesToBeChecked <= checkedBytes)
                            {
                                return (0);
                            }
                            uint itemLengthSize = 0;
                            itemLength = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out itemLengthSize);
                            if (itemLength == 0)
                            {
                                return (0);
                            }
                            checkedBytes += itemLengthSize;

                            if (bytesToBeChecked <= checkedBytes)
                            {
                                return (0);
                            }
                            if (itemLength != (bytesToBeChecked - checkedBytes))
                            {
                                return (0);
                            }
                            upperLayerData = true;
                        }
                        break;

                    default:
                        if (bytesToBeChecked <= checkedBytes)
                        {
                            return (0);
                        }
                        itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                        if (bytesToBeChecked < (checkedBytes + itemLength))
                        {
                            return (0);
                        }
                        checkedBytes += itemLength;
                        break;
                }
            }

            return (checkedBytes);
        }

        public uint CheckPresentationLayer(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, byte expectedPDUType, IEC61850Station station)
        {
            uint checkedBytes = 0;
            // Check the PDU type
            if (replyMessage[totalCheckedBytes + checkedBytes++] != expectedPDUType)
            {
                return (0);
            }
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if(length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }
            if (length != (bytesToBeChecked - checkedBytes))
            {
                return (0);
            }

            uint partialCheckedBytes = 0;

            switch (expectedPDUType)
            {
                // Initiate
                case 0x31:
                    // Skip mode - selector
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    if (bytesToBeChecked < (checkedBytes + replyMessage[totalCheckedBytes + checkedBytes]))
                    {
                        return (0);
                    }
                    checkedBytes += (uint)(replyMessage[totalCheckedBytes + checkedBytes] + 1);
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }

                    partialCheckedBytes = CheckPresentationPDU(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, station);
                    if (partialCheckedBytes == 0)
                    {
                        return (0);
                    }
                    checkedBytes += partialCheckedBytes;
                    break;

                // Fully - encoded data
                case 0x61:
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    lengthSize = 0;
                    length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
                    if (length == 0)
                    {
                        return (0);
                    }
                    checkedBytes += lengthSize;
                    if (bytesToBeChecked <= checkedBytes)
                    {
                        return (0);
                    }
                    if (length != (bytesToBeChecked - checkedBytes))
                    {
                        return (0);
                    }
                    if (bytesToBeChecked <= checkedBytes + 3)
                    {
                        return (0);
                    }
                    // Check the presentation context identifier (3 = MMS) 
                    if (replyMessage[totalCheckedBytes + checkedBytes + 2] != 0x03)
                    {
                        return (0);
                    }
                    checkedBytes += 3;
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    lengthSize = 0;
                    length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
                    if (length == 0)
                    {
                        return (0);
                    }
                    checkedBytes += lengthSize;
                    if (bytesToBeChecked <= checkedBytes)
                    {
                        return (0);
                    }
                    if (length != (bytesToBeChecked - checkedBytes))
                    {
                        return (0);
                    }
                    break;
            }

            return (checkedBytes);
        }

        public uint CheckMMSConfirmedReadResponse(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            uint checkedBytes = 0;
            byte responseType = 0;
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }
            responseType = replyMessage[totalCheckedBytes + checkedBytes++];
            if ((bytesToBeChecked - checkedBytes) == 0)
            {
                return (0);
            }
            uint partialCheckedBytes = 0;
            switch (responseType)
            {
                case 0xa4: // Service == Read Reply
                    partialCheckedBytes = CheckMMSReadReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out dataLength);
                    break;
            }
            if (partialCheckedBytes == 0)
            {
                return (0);
            }
            checkedBytes += partialCheckedBytes;
            if (((bytesToBeChecked - checkedBytes) == 0) || (dataLength == 0) || ((bytesToBeChecked - checkedBytes) < dataLength))
            {
                return (0);
            }

            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            if (!CheckMMSResult(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out errorCode))
            {
                return (0);
            }

            return (checkedBytes);
        }

        public uint CheckMMSIdentifyResponse(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked)
        {
            uint checkedBytes = 0;
            byte responseType = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }
            responseType = replyMessage[totalCheckedBytes + checkedBytes++];
            if ((bytesToBeChecked - checkedBytes) == 0)
            {
                return (0);
            }
            uint partialCheckedBytes = 0;
            switch (responseType)
            {
                case 0xa2: // Service == Identify Reply
                    partialCheckedBytes = CheckMMSIdentifyReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes);
                    break;
            }
            if (partialCheckedBytes == 0)
            {
                return (0);
            }
            checkedBytes += partialCheckedBytes;

            return (checkedBytes);
        }

        public uint CheckMMSVarAccessReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }

            uint checkedBytes = 0;
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes, bytesToBeChecked, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if ((length <= 4) || (bytesToBeChecked < (checkedBytes + length)))
            {
                return (0);
            }

            // Skip the Deletable attribute
            checkedBytes += 3;

            // Check the access result
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0xa2)
            {
                return (0);
            }
            if (bytesToBeChecked < (checkedBytes + 2))
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;

            dataLength = length;

            return (checkedBytes);
        }

        public uint CheckMMSNamedVarListReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }

            uint checkedBytes = 0;
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes, bytesToBeChecked, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if ((length <= 4) || (bytesToBeChecked < (checkedBytes + length)))
            {
                return (0);
            }

            // Skip the Deletable attribute
            checkedBytes += 3;

            // Check the access result
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0xa1)
            {
                return (0);
            }
            if (bytesToBeChecked < (checkedBytes + 2))
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;

            dataLength = length;

            return (checkedBytes);
        }

        public uint CheckMMSGetGetVarAccessAttribReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            uint checkedBytes = 0;
            byte responseType = 0;
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }
            responseType = replyMessage[totalCheckedBytes + checkedBytes++];
            if ((bytesToBeChecked - checkedBytes) == 0)
            {
                return (0);
            }
            uint partialCheckedBytes = 0;
            switch (responseType)
            {
                case 0xa6: // Service == GetVariableAccessAttributes
                    partialCheckedBytes = CheckMMSVarAccessReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out dataLength);
                    break;
            }
            if (partialCheckedBytes == 0)
            {
                return (0);
            }
            checkedBytes += partialCheckedBytes;
            if (((bytesToBeChecked - checkedBytes) == 0) || (dataLength == 0) || ((bytesToBeChecked - checkedBytes) < dataLength))
            {
                return (0);
            }

            return (checkedBytes);
        }

        public uint CheckMMSGetNamedVarListAttribReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            uint checkedBytes = 0;
            byte responseType = 0;
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }
            responseType = replyMessage[totalCheckedBytes + checkedBytes++];
            if ((bytesToBeChecked - checkedBytes) == 0)
            {
                return (0);
            }
            uint partialCheckedBytes = 0;
            switch (responseType)
            {
                case 0xac: // Service == GetNamedVariableListAttributes
                    partialCheckedBytes = CheckMMSNamedVarListReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out dataLength);
                    break;
            }
            if (partialCheckedBytes == 0)
            {
                return (0);
            }
            checkedBytes += partialCheckedBytes;
            if (((bytesToBeChecked - checkedBytes) == 0) || (dataLength == 0) || ((bytesToBeChecked - checkedBytes) < dataLength))
            {
                return (0);
            }

            return (checkedBytes);
        }

        public uint CheckMMSConfirmedResponse(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, IEC61850CommJob job, out uint dataLength, out DriverErrorCodes errorCode)
        {
            uint checkedBytes = 0;
            byte responseType = 0;
            dataLength = 0;
            errorCode = DriverErrorCodes.ErrorNoError;
            if (bytesToBeChecked < 2)
            {
                errorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                return (0);
            }
            responseType = replyMessage[totalCheckedBytes + checkedBytes++];
            if((bytesToBeChecked - checkedBytes) == 0)
            {
                return (0);
            }
            uint partialCheckedBytes = 0;
            switch(responseType)
            {
                case 0xa4: // Service == Read Reply
                    switch(job.RequestType)
                    {
                        case RequestTypes.ReadValue:
                            {
                                partialCheckedBytes = CheckMMSReadReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out dataLength);
                            }
                            break;
                    }
                    break;

                case 0xa5: // Service == Write Reply
                    switch (job.RequestType)
                    {
                        case RequestTypes.WriteValue:
                            {
                                //partialCheckedBytes = CheckMMSWriteReply(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes);
                                partialCheckedBytes = CheckMMSWriteReply(replyMessage, totalCheckedBytes, bytesToBeChecked - checkedBytes);
                            }
                            break;
                    }
                    break;
            }
            if (partialCheckedBytes == 0)
            {
                return (0);
            }
            checkedBytes += partialCheckedBytes;
            if(job.RequestType == RequestTypes.ReadValue)
            {
                if (((bytesToBeChecked - checkedBytes) == 0) || (dataLength == 0) || ((bytesToBeChecked - checkedBytes) < dataLength))
                {
                    return (0);
                }

                if (!CheckMMSResult(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out errorCode))
                {
                    return (0);
                }
            }
            else if(job.RequestType == RequestTypes.WriteValue)
            {
                if ((bytesToBeChecked - checkedBytes) == 0)
                {
                    return (0);
                }

                if (!CheckMMSWriteResult(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out errorCode))
                {
                    return (0);
                }
            }

            return (checkedBytes);
        }

        bool CheckMMSWriteResult(byte[] replyMessage, uint initialIndex, uint bytesToBeChecked, out DriverErrorCodes errorCode)
        {
            bool returnValue = true;
            errorCode = DriverErrorCodes.ErrorNoError;
            switch (replyMessage[initialIndex])
            {
                // Data Access Error
                case 0x80:
                    if (bytesToBeChecked > 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(replyMessage, initialIndex + 1, bytesToBeChecked - 1, out lengthSize);
                        if ((length > 0) && (bytesToBeChecked > (lengthSize + length)))
                        {
                            errorCode = (DriverErrorCodes)((uint)IEC61850ErrorCodes.ErrorDataAccessError + BerDecodeUint32(replyMessage, initialIndex + 1 + lengthSize, length));

                        }
                    }
                    returnValue = false;
                    break;

                // OK
                case 0x81:
                    returnValue = true;
                    break;

                // Unknown or unsupported
                default:
                    returnValue = false;
                    break;
            }

            return (returnValue);
        }

        bool CheckMMSResult(byte[] replyMessage, uint initialIndex, uint bytesToBeChecked, out DriverErrorCodes errorCode)
        {
            bool returnValue = true;
            errorCode = DriverErrorCodes.ErrorNoError;
            switch(replyMessage[initialIndex])
            {
                // Data Access Error
                case 0x80:
                    if (bytesToBeChecked > 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(replyMessage, initialIndex + 1, bytesToBeChecked - 1, out lengthSize);
                        if ((length > 0) && (bytesToBeChecked > (lengthSize + length)))
                        {
                            errorCode = (DriverErrorCodes)((uint)IEC61850ErrorCodes.ErrorDataAccessError + BerDecodeUint32(replyMessage, initialIndex + 1 + lengthSize, length));

                        }
                    }
                    returnValue = false;
                    break;

                // Visible String
                case 0x8a:
                // Unsigned
                case 0x86:
                // Integer
                case 0x85:
                // UTC Time
                case 0x91:
                // Bit String
                case 0x84:
                // Boolean
                case 0x83:
                // Binary Time
                case 0x8c:
                // Octet String
                case 0x89:
                // Float
                case 0x87:
                // String
                case 0x90:
                // Structure
                case 0xa2:
                    returnValue = true;
                    break;

                // Unknown or unsupported
                default:
                    returnValue = false;
                    break;
            }

            return (returnValue);
        }

        public bool CheckMMSWriteResult(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint errorCode)
        {            
            bool bReturnValue = true;
            uint nReplyLength = totalCheckedBytes;

            errorCode = 0;

            switch (replyMessage[totalCheckedBytes])
            {
                // Data Access Error
                case 0x80:
                    if (nReplyLength > 2)
                    {                        
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(replyMessage, totalCheckedBytes, bytesToBeChecked, out lengthSize);                        
                        if (length > 0 && (nReplyLength > (lengthSize + length)))
                            errorCode = BerDecodeUint32(replyMessage, totalCheckedBytes + bytesToBeChecked, length);
                    }
                    bReturnValue = false;
                    break;

                // OK
                case 0x81:
                    bReturnValue = true;
                    break;

                // Unknown or unsupported
                default:
                    bReturnValue = false;
                    break;
            }

            return (bReturnValue);
        }

        public uint ParseInfoReportID(byte[] replyBuffer, uint totalCheckedBytes, uint nReplyLength, out string szReportID)
        {
            szReportID = string.Empty;

            uint nParsingIndex = totalCheckedBytes;
            if (replyBuffer[nParsingIndex++] != 0xa0)
            {
                return (0);
            }

            uint lengthSize = 0;
            uint length = BerDecodeLength(replyBuffer, nParsingIndex, nReplyLength - nParsingIndex, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            nParsingIndex += lengthSize;
            //if (nReplyLength != (nParsingIndex + length))
            //{
            //    return (0);
            //}

            // The item should be a visible-string
            if (replyBuffer[nParsingIndex++] != 0x8a)
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyBuffer, nParsingIndex, nReplyLength - nParsingIndex, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            nParsingIndex += lengthSize;
            //if (nReplyLength != (nParsingIndex + length))
            //{
            //    return (0);
            //}

            for (uint i = 0; i < length; i++)
            {
                szReportID += (char)replyBuffer[nParsingIndex++];
            }

            return (nParsingIndex - totalCheckedBytes);
        }

        public uint CheckMMSReadReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out uint dataLength)
        {
            dataLength = 0;
            if (bytesToBeChecked < 2)
            {
                return (0);
            }

            uint checkedBytes = 0;
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes, bytesToBeChecked, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if ((length <= 2) || (bytesToBeChecked < (checkedBytes + length)))
            {
                return (0);
            }

            // Check the access result
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0xa1)
            {
                return (0);
            }
            if (bytesToBeChecked < (checkedBytes + 2))
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;

            dataLength = length;

            return (checkedBytes);
        }

        public uint CheckMMSIdentifyReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked)
        {
            if (bytesToBeChecked < 2)
            {
                return (0);
            }

            uint checkedBytes = 0;
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes, bytesToBeChecked, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if ((length <= 2) || (bytesToBeChecked < (checkedBytes + length)))
            {
                return (0);
            }

            // Ignore the remaining part of the message
            checkedBytes += length;

            return (checkedBytes);
        }

        public uint CheckMMSWriteReply(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked)//, out uint dataLength)
        {
            uint nReplyLength = (uint)replyMessage.Length;

            //dataLength = 0;
            if (bytesToBeChecked < 2)
                return (0);

            uint checkedBytes = 0;
            uint lengthSize = 0;
            
            // Check the access result
            if (replyMessage[totalCheckedBytes++] != 0xa5)
                return (0);

            if (bytesToBeChecked < (checkedBytes + 2))
                return (0);

            lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
                return (0);

            checkedBytes += lengthSize;

            if (nReplyLength < (totalCheckedBytes + length))
                return (0);

            if (length < 1)
                return (0);

            return (checkedBytes);
        }

        public uint CheckInformationReport(byte[] replyBuffer)
        {
            // Check the Object name of the variable list: it must be "RPT"
            uint nTotalCheckedBytes = 0;
            uint nReplyLength = (uint)replyBuffer.Length;
            if (replyBuffer[nTotalCheckedBytes++] != 0xa0)
            {
                return (0);
            }
            if (nReplyLength <= nTotalCheckedBytes)
            {
                return (0);
            }            
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyBuffer, nTotalCheckedBytes, nReplyLength - nTotalCheckedBytes, out lengthSize);
            if (length==0)
            {
                return (0);
            }
            nTotalCheckedBytes += lengthSize;
            if (nReplyLength <= nTotalCheckedBytes)
            {
                return (0);
            }
            if (length != (nReplyLength - nTotalCheckedBytes))
            {
                return (0);
            }
            if (replyBuffer[nTotalCheckedBytes++] != 0xa1)
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyBuffer, nTotalCheckedBytes, nReplyLength - nTotalCheckedBytes, out lengthSize);            
            if (length == 0)
            {
                return (0);
            }
            nTotalCheckedBytes += lengthSize;
            if (nReplyLength <= (nTotalCheckedBytes + length))
            {
                return (0);
            }
            if (replyBuffer[nTotalCheckedBytes++] != 0x80)
            {
                return (0);
            }
            lengthSize = 0;
            length = BerDecodeLength(replyBuffer, nTotalCheckedBytes, nReplyLength - nTotalCheckedBytes, out lengthSize);
            if (length != 3)
            {
                return (0);
            }
            nTotalCheckedBytes += lengthSize;
            if (nReplyLength <= (nTotalCheckedBytes + length))
            {
                return (0);
            }
            if (replyBuffer[nTotalCheckedBytes++] != 0x52) // 'R'
            {
                return (0);
            }
            if (replyBuffer[nTotalCheckedBytes++] != 0x50) // 'P'
            {
                return (0);
            }
            if (replyBuffer[nTotalCheckedBytes++] != 0x54) // 'T'
            {
                return (0);
            }

            return (nTotalCheckedBytes);
        }

        public uint CheckMMSLayer(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, IEC61850Channel channel, out byte mpduType)
        {
            uint checkedBytes = 0;
            // Parse and check the PDU type
            mpduType = replyMessage[totalCheckedBytes + checkedBytes++];
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }
            if (length != (bytesToBeChecked - checkedBytes))
            {
                return (0);
            }

            uint partialCheckedBytes = 0;

            switch (mpduType)
            {
                // Confirmed response PDU
                case 0xa1:
                    // Check the InvokeID
                    partialCheckedBytes = CheckMMSInvokeId(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, channel);
                    if (partialCheckedBytes == 0)
                    {
                        return (0);
                    }
                    checkedBytes += partialCheckedBytes;
                    break;
            }

            return (checkedBytes);
        }

        public uint CheckMMSInvokeId(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, IEC61850Channel channel)
        {
            uint checkedBytes = 0;
            if (replyMessage[totalCheckedBytes + checkedBytes++] != 0x02)
            {
                return (0);
            }
            if (bytesToBeChecked < (checkedBytes + 2))
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (bytesToBeChecked <= (checkedBytes + length))
            {
                return (0);
            }

            uint invokeID = BerDecodeUint32(replyMessage, totalCheckedBytes + checkedBytes, length);
            if (invokeID != channel.InvokeID)
            {
                return (0);
            }

            checkedBytes += length;
            return (checkedBytes);
        }

        public uint CheckPresentationLayer(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, IEC61850Station station, out byte ppduType)
        {
            uint checkedBytes = 0;
            // Parse and check the PDU type
            ppduType = replyMessage[totalCheckedBytes + checkedBytes++];
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }

            // Check the PDU length
            uint lengthSize = 0;
            uint length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
            if (length == 0)
            {
                return (0);
            }
            checkedBytes += lengthSize;
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }
            if (length != (bytesToBeChecked - checkedBytes))
            {
                return (0);
            }

            uint partialCheckedBytes = 0;

            switch (ppduType)
            {
                // Initiate
                case 0x31:
                    // Skip mode - selector
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    if (bytesToBeChecked < (checkedBytes + replyMessage[totalCheckedBytes + checkedBytes]))
                    {
                        return (0);
                    }
                    checkedBytes += (uint)(replyMessage[totalCheckedBytes + checkedBytes] + 1);
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }

                    partialCheckedBytes = CheckPresentationPDU(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, station);
                    if (partialCheckedBytes == 0)
                    {
                        return (0);
                    }
                    checkedBytes += partialCheckedBytes;
                    break;

                // Fully - encoded data
                case 0x61:
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    lengthSize = 0;
                    length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
                    if (length == 0)
                    {
                        return (0);
                    }
                    checkedBytes += lengthSize;
                    if (bytesToBeChecked <= checkedBytes)
                    {
                        return (0);
                    }
                    if (length != (bytesToBeChecked - checkedBytes))
                    {
                        return (0);
                    }
                    if (bytesToBeChecked <= checkedBytes + 3)
                    {
                        return (0);
                    }
                    // Check the presentation context identifier (3 = MMS) 
                    if (replyMessage[totalCheckedBytes + checkedBytes + 2] != 0x03)
                    {
                        return (0);
                    }
                    checkedBytes += 3;
                    if (bytesToBeChecked < (checkedBytes + 2))
                    {
                        return (0);
                    }
                    checkedBytes++;
                    lengthSize = 0;
                    length = BerDecodeLength(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out lengthSize);
                    if (length == 0)
                    {
                        return (0);
                    }
                    checkedBytes += lengthSize;
                    if (bytesToBeChecked <= checkedBytes)
                    {
                        return (0);
                    }
                    if (length != (bytesToBeChecked - checkedBytes))
                    {
                        return (0);
                    }
                    break;
            }

            return (checkedBytes);
        }

        uint CheckPresentationPDU(byte[] replyMessage, uint initIndex, uint bytesToBeChecked, IEC61850Station station)
        {
            uint totalCheckedBytes = 0;
            switch (replyMessage[initIndex + totalCheckedBytes++])
            {
                // Normal mode parameters
                case 0xa2:
                    {
                        // Check the PDU length
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(replyMessage, initIndex + totalCheckedBytes, bytesToBeChecked - totalCheckedBytes, out lengthSize);
                        if (length == 0)
                        {
                            return (0);
                        }
                        totalCheckedBytes += lengthSize;
                        if (bytesToBeChecked <= totalCheckedBytes)
                        {
                            return (0);
                        }
                        if (length != (bytesToBeChecked - totalCheckedBytes))
                        {
                            return (0);
                        }

                        // Check PDU items
                        bool upperLayerData = false;
                        uint itemLength = 0;
                        while ((bytesToBeChecked > totalCheckedBytes) && !upperLayerData)
                        {
                            switch (replyMessage[initIndex + totalCheckedBytes++])
                            {
                                // Called Presentation Selector
                                case 0x83:
                                    if (bytesToBeChecked <= totalCheckedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[initIndex + totalCheckedBytes++];
                                    if (bytesToBeChecked < (totalCheckedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    if (station.PresentationSelectorArray == null)
                                    {
                                        return (0);
                                    }
                                    for (uint i = 0; (i < itemLength) && (i < MAX_SELECTOR_BYTE_SIZE) && (i < station.PresentationSelectorArray.Length); i++)
                                    {
                                        if (station.PresentationSelectorArray[i] != replyMessage[initIndex + totalCheckedBytes++])
                                        {
                                            return (0);
                                        }
                                    }
                                    if (itemLength > MAX_SELECTOR_BYTE_SIZE)
                                    {
                                        totalCheckedBytes += (uint)(itemLength - MAX_SELECTOR_BYTE_SIZE);
                                    }
                                    break;

                                case 0x61:
                                case 0x30:
                                    {
                                        if (bytesToBeChecked <= totalCheckedBytes)
                                        {
                                            return (0);
                                        }
                                        uint itemLengthSize = 0;
                                        itemLength = BerDecodeLength(replyMessage, initIndex + totalCheckedBytes, bytesToBeChecked - totalCheckedBytes, out itemLengthSize);
                                        if (itemLength == 0)
                                        {
                                            return (0);
                                        }
                                        totalCheckedBytes += itemLengthSize;
                                        if (bytesToBeChecked <= totalCheckedBytes)
                                        {
                                            return (0);
                                        }
                                        if (itemLength != (bytesToBeChecked - totalCheckedBytes))
                                        {
                                            return (0);
                                        }
                                    }
                                    break;

                                // User data
                                case 0xa0:
                                    {
                                        if (bytesToBeChecked <= totalCheckedBytes)
                                        {
                                            return (0);
                                        }
                                        uint itemLengthSize = 0;
                                        itemLength = BerDecodeLength(replyMessage, initIndex + totalCheckedBytes, bytesToBeChecked - totalCheckedBytes, out itemLengthSize);
                                        if (itemLength == 0)
                                        {
                                            return (0);
                                        }
                                        totalCheckedBytes += itemLengthSize;
                                        if (bytesToBeChecked <= totalCheckedBytes)
                                        {
                                            return (0);
                                        }
                                        if (itemLength != (bytesToBeChecked - totalCheckedBytes))
                                        {
                                            return (0);
                                        }
                                        upperLayerData = true;
                                    }
                                    break;

                                default:
                                    if (bytesToBeChecked <= totalCheckedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[initIndex + totalCheckedBytes++];
                                    if (bytesToBeChecked < (totalCheckedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    totalCheckedBytes += itemLength;
                                    break;
                            }
                        }
                    }
                    break;
            }

            return (totalCheckedBytes);
        }

        uint BerDecodeUint32(byte[] replyMessage, uint initIndex, uint valueSize)
        {
            uint value = 0;

            for (int i = 0; i < (int)valueSize; i++)
            {
                value <<= 8;
                value += replyMessage[initIndex + i];
            }

            return (value);
        }

        public uint BerDecodeLength(byte[] replyMessage, uint initIndex, uint bytesToBeChecked, out uint lengthSize)
        {
            lengthSize = 0;
            if(bytesToBeChecked < 1)
            {
                return (0);
            }
            uint length = 0;
            lengthSize = replyMessage[initIndex];
            if((lengthSize & 0x80) == 0)
            {
                length = lengthSize;
                lengthSize = 1;
            }
            else
            {
                lengthSize &= 0x7f;
                if (bytesToBeChecked < (lengthSize + 1))
                {
                    lengthSize = 0;
                    return (0);
                }

                switch (lengthSize)
                {
                    case 1:
                        length = replyMessage[initIndex + 1];
                        lengthSize++;
                        break;

                    case 2:
                        length = replyMessage[initIndex + 1];
                        length <<= 8;
                        length += replyMessage[initIndex + 2];
                        lengthSize++;
                        break;

                    case 3:
                        length = replyMessage[initIndex + 1];
                        length <<= 8;
                        length += replyMessage[initIndex + 2];
                        length <<= 8;
                        length += replyMessage[initIndex + 3];
                        lengthSize++;
                        break;

                    case 4:
                        length = replyMessage[initIndex + 1];
                        length <<= 8;
                        length += replyMessage[initIndex + 2];
                        length <<= 8;
                        length += replyMessage[initIndex + 3];
                        length <<= 8;
                        length += replyMessage[initIndex + 4];
                        lengthSize++;
                        break;

                    default:
                        lengthSize = 0;
                        break;
                }
            }

            return (length);
        }
        
        public uint CheckSessionLayer(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, byte expectedPDUType, IEC61850Station station)
        {
            uint checkedBytes = 0;
            // Check the PDU type
            if (replyMessage[totalCheckedBytes + checkedBytes++] != expectedPDUType)
            {
                return (0);
            }
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }

            switch (expectedPDUType)
            {
                // Give Tokens
                case 0x01:
                    if (bytesToBeChecked <= (checkedBytes + 2))
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 0)
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 1)
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 0)
                    {
                        return (0);
                    }
                    break;

                // ACCEPT (AC)
                case 0x0e:
                    {
                        // Check the PDU length
                        if (replyMessage[totalCheckedBytes + checkedBytes++] != (bytesToBeChecked - 2))
                        {
                            return (0);
                        }
                        if (bytesToBeChecked <= checkedBytes)
                        {
                            return (0);
                        }

                        // Check PDU items
                        bool upperLayerData = false;
                        byte itemLength = 0;
                        while ((bytesToBeChecked > checkedBytes) && !upperLayerData)
                        {
                            switch (replyMessage[totalCheckedBytes + checkedBytes++])
                            {
                                // Called Session Selector
                                case 0x34:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    if (bytesToBeChecked < (checkedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    if(station.SessionSelectorArray == null)
                                    {
                                        return (0);
                                    }
                                    for (uint i = 0; (i < itemLength) && (i < MAX_SELECTOR_BYTE_SIZE) && (i < station.SessionSelectorArray.Length); i++)
                                    {
                                        if (station.SessionSelectorArray[i] != replyMessage[totalCheckedBytes + checkedBytes++])
                                        {
                                            return (0);
                                        }
                                    }
                                    if (itemLength > MAX_SELECTOR_BYTE_SIZE)
                                    {
                                        checkedBytes += (uint)(itemLength - MAX_SELECTOR_BYTE_SIZE);
                                    }
                                    break;

                                // Session User Data
                                case 0xc1:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    upperLayerData = true;
                                    break;

                                default:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    if (bytesToBeChecked < (checkedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    checkedBytes += itemLength;
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    return (0);
            }

            return (checkedBytes);
        }

        public uint CheckSessionLayer(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, IEC61850Station station, out byte spduType)
        {
            uint checkedBytes = 0;
            // Check the PDU type
            spduType = replyMessage[totalCheckedBytes + checkedBytes++];
            if (bytesToBeChecked <= checkedBytes)
            {
                return (0);
            }

            switch (spduType)
            {
                // Give Tokens
                case 0x01:
                    if (bytesToBeChecked <= (checkedBytes + 2))
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 0) // Length must be 0
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 1) // 1 == DATA TRANSFER
                    {
                        return (0);
                    }
                    if (replyMessage[totalCheckedBytes + checkedBytes++] != 0) // Length must be 0
                    {
                        return (0);
                    }
                    break;

                // ACCEPT (AC)
                case 0x0e:
                    {
                        // Check the PDU length
                        if (replyMessage[totalCheckedBytes + checkedBytes++] != (bytesToBeChecked - 2))
                        {
                            return (0);
                        }
                        if (bytesToBeChecked <= checkedBytes)
                        {
                            return (0);
                        }

                        // Check PDU items
                        bool upperLayerData = false;
                        byte itemLength = 0;
                        while ((bytesToBeChecked > checkedBytes) && !upperLayerData)
                        {
                            switch (replyMessage[totalCheckedBytes + checkedBytes++])
                            {
                                // Called Session Selector
                                case 0x34:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    if (bytesToBeChecked < (checkedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    if (station.SessionSelectorArray == null)
                                    {
                                        return (0);
                                    }
                                    for (uint i = 0; (i < itemLength) && (i < MAX_SELECTOR_BYTE_SIZE) && (i < station.SessionSelectorArray.Length); i++)
                                    {
                                        if (station.SessionSelectorArray[i] != replyMessage[totalCheckedBytes + checkedBytes++])
                                        {
                                            return (0);
                                        }
                                    }
                                    if (itemLength > MAX_SELECTOR_BYTE_SIZE)
                                    {
                                        checkedBytes += (uint)(itemLength - MAX_SELECTOR_BYTE_SIZE);
                                    }
                                    break;

                                // Session User Data
                                case 0xc1:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    upperLayerData = true;
                                    break;

                                default:
                                    if (bytesToBeChecked <= checkedBytes)
                                    {
                                        return (0);
                                    }
                                    itemLength = replyMessage[totalCheckedBytes + checkedBytes++];
                                    if (bytesToBeChecked < (checkedBytes + itemLength))
                                    {
                                        return (0);
                                    }
                                    checkedBytes += itemLength;
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    return (0);
            }

            return (checkedBytes);
        }

        public uint CheckCOTPHeader(byte[] replyMessage, uint totalCheckedBytes, uint bytesToBeChecked, out byte cotpSegmentType)
        {
            cotpSegmentType = 0;

            if (bytesToBeChecked < 3)
            {
                return (0);
            }

            if((replyMessage[totalCheckedBytes] != 0x02) || (replyMessage[totalCheckedBytes + 1] != 0xf0))
            {
                return (0);
            }

            cotpSegmentType = replyMessage[totalCheckedBytes + 2];

            return (3);
        }


        public byte[] BuildCOTPConnectionRequest(IEC61850Station station)
        {
            // Set the array of the station transport selector
            byte[] selectorArray = SetSelectorArray(station.TransportSelector);
            if((selectorArray == null) || (selectorArray.Count() < 1))
            {
                return (null);
            }
            station.SetTransportSelectorArray(selectorArray);

            byte[] connectionRequestBuffer = new byte[2*selectorArray.Count() + 14];
            int bufferLength = 0;
            connectionRequestBuffer[bufferLength++] = 0x11; // Length (temporary)
            connectionRequestBuffer[bufferLength++] = 0xE0; // PDU Type = Connect Request
            connectionRequestBuffer[bufferLength++] = 0; // Destination Reference (High byte)
            connectionRequestBuffer[bufferLength++] = 0; // Destination Reference (Low byte)
            connectionRequestBuffer[bufferLength++] = 0; // Source Reference (High byte)
            connectionRequestBuffer[bufferLength++] = 0x02; // Source Reference (Low byte)
            connectionRequestBuffer[bufferLength++] = 0; // Class
            connectionRequestBuffer[bufferLength++] = 0xC0; // Parameter code = TPDU size
            connectionRequestBuffer[bufferLength++] = 0x01; // Parameter length
            connectionRequestBuffer[bufferLength++] = 0x0D; // 2^13 = 8192
            connectionRequestBuffer[bufferLength++] = 0xC2; // Parameter = Destination TSAP
            connectionRequestBuffer[bufferLength++] = (byte)selectorArray.Count(); // Parameter length
            Array.Copy(selectorArray, 0, connectionRequestBuffer, bufferLength, selectorArray.Count());
            bufferLength += selectorArray.Count();
            connectionRequestBuffer[bufferLength++] = 0xC1; // Parameter code = Source TSAP
            connectionRequestBuffer[bufferLength++] = (byte)selectorArray.Count(); // Parameter length
            for (int i = 0; i < selectorArray.Count(); i++)
            {
                connectionRequestBuffer[bufferLength++] = 0;
            }

            // Set the true message length
            connectionRequestBuffer[0] = (byte)(bufferLength - 1);

            return (connectionRequestBuffer);
        }
 
        public byte[] BuildAssociateRequest(IEC61850Station station, IEC61850Channel channel)
        {
            byte[] mmsInitiateRequest = BuildMMSInitiateRequest();
            byte[] associationControlRequest = AddAssociationControlRequest(station, channel, mmsInitiateRequest);
            byte[] presentationConnectionRequest = AddPresentationConnectionRequest(station, channel, associationControlRequest);
            byte[] sessionConnectionRequest = AddSessionConnectionRequest(station, channel, presentationConnectionRequest);
            byte[] cotpRequest = AddCOTPHeader(sessionConnectionRequest);
            byte[] isoRequest = AddISOHeader(cotpRequest);

            return (isoRequest);
        }

        byte[] AddSessionConnectionRequest(IEC61850Station station, IEC61850Channel channel, byte[] requestDataPart)
        {
            // Set the session selector arrays of station and channel
            byte[] calledSelectorArray = SetSelectorArray(station.SessionSelector);
            if ((calledSelectorArray == null) || (calledSelectorArray.Length < 1))
            {
                return (null);
            }
            uint calledSelectorSize = (uint)calledSelectorArray.Length;
            byte[] callingSelectorArray = SetSelectorArray(channel.ClientSessionSelector);
            if ((callingSelectorArray == null) || (callingSelectorArray.Length < 1))
            {
                return (null);
            }
            uint callingSelectorSize = (uint)callingSelectorArray.Length;
            station.SetSessionSelectorArray(calledSelectorArray);
            channel.SetSessionSelectorArray(callingSelectorArray);

            // Build the request message
            uint dataLength = (uint)requestDataPart.Length;
            uint messageTotalLength = 20 + calledSelectorSize + callingSelectorSize + dataLength;
            byte[] requestMessage = new byte[messageTotalLength];
            // ISO 8327-1 OSI Session Protocol
            uint messageLength = 0;
            requestMessage[messageLength++] = 0x0D; // SPDU Type = Connect
            requestMessage[messageLength++] = 0; // Length (to be set later)

            // Connect Accept Item
            requestMessage[messageLength++] = 0x05; // Parameter = Connect Accept Item
            requestMessage[messageLength++] = 0x06; // Parameter Length
            requestMessage[messageLength++] = 0x13; // Parameter = Protocol Options
            requestMessage[messageLength++] = 0x01; // Parameter Length
            requestMessage[messageLength++] = 0; // Flags = Cannot receive concat. SPDU
            requestMessage[messageLength++] = 0x16; // Parameter = Version Number
            requestMessage[messageLength++] = 0x01; // Parameter Length
            requestMessage[messageLength++] = 0x02; // Flags = Only Protocol version 2

            // Session Requirement
            requestMessage[messageLength++] = 0x14; // Parameter = Session Requirement
            requestMessage[messageLength++] = 0x02; // Parameter Length
            requestMessage[messageLength++] = 0; // Flags
            requestMessage[messageLength++] = 0x02; // Flags = Duplex Functional Unit

            // Calling Session Selector
            requestMessage[messageLength++] = 0x33; // Param. = Calling Session Selector
            requestMessage[messageLength++] = (byte)callingSelectorSize; // Parameter length
            Array.Copy(callingSelectorArray, 0, requestMessage, messageLength, callingSelectorSize);
            messageLength += callingSelectorSize;

            // Called Session Selector
            requestMessage[messageLength++] = 0x34; // Param. = Called Session Selector
            requestMessage[messageLength++] = (byte)calledSelectorSize; // Parameter length
            Array.Copy(calledSelectorArray, 0, requestMessage, messageLength, calledSelectorSize);
            messageLength += calledSelectorSize;

            // Session User Data
            requestMessage[messageLength++] = 0xC1; // Parameter = Session User Data
            requestMessage[messageLength++] = (byte)dataLength; // Parameter length

            // Set the true length
            requestMessage[1] = (byte)(dataLength + messageLength - 2); // Length

            if(dataLength > 0)
            {
                // Copy the data part of the message
                Array.Copy(requestDataPart, 0, requestMessage, messageLength, dataLength);
                messageLength += (uint)dataLength;
            }

            return (requestMessage);
        }

        byte[] AddSessionReadRequest(byte[] requestDataPart)
        {
            // Calculate the length of the encoded message
            uint dataLength = (uint)requestDataPart.Length;
            uint messageTotalLength = 4 + dataLength;

            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;
            requestBuffer[messageLength++] = 0x01; // SPDU Type = Give tokens
            requestBuffer[messageLength++] = 0; // Length
            requestBuffer[messageLength++] = 0x01; // SPDU Type = DATA TRANSFER (DT)
            requestBuffer[messageLength++] = 0; // Length

            if (dataLength > 0)
            {
                // Copy the data part of the message
                Array.Copy(requestDataPart, 0, requestBuffer, messageLength, dataLength);
                messageLength += dataLength;
            }

            return (requestBuffer);
        }

        byte[] AddPresentationReadRequest(byte[] requestDataPart)
        {
            // Calculate the length of the encoded message
            uint dataLength = (uint)requestDataPart.Length;
            uint encodedDataLength = 4 + dataLength;
            encodedDataLength += BerEncoderCheckLengthSize(dataLength);
            uint fullyEncodedDataLength = encodedDataLength + 1;
            fullyEncodedDataLength += BerEncoderCheckLengthSize(encodedDataLength);
            uint messageTotalLength = 1 + fullyEncodedDataLength + BerEncoderCheckLengthSize(fullyEncodedDataLength); //1 + fullyEncodedDataLength + BerEncoderCheckLengthSize(fullyEncodedDataLength);

            // ISO 8823 OSI Presentation Protocol
            byte[] requestBuffer = new byte[messageTotalLength];
            uint messageLength = 0;
            // Fully encoded data
            requestBuffer[messageLength++] = 0x61;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, fullyEncodedDataLength);
            requestBuffer[messageLength++] = 0x30;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, encodedDataLength);
            requestBuffer[messageLength++] = 0x02;
            requestBuffer[messageLength++] = 0x01;
            // 0x03 = Presentation Context Identifier = MMS abstract syntax version 1
            requestBuffer[messageLength++] = 0x03;
            // Application data length
            requestBuffer[messageLength++] = 0xA0;
            BerEncoderAddLength(ref requestBuffer, ref messageLength, dataLength);
            if (dataLength > 0)
            {
                // Copy the data part of the message
                Array.Copy(requestDataPart, 0, requestBuffer, messageLength, dataLength);
                messageLength += dataLength;
            }

            return (requestBuffer);
        }

        public byte[] AddPresentationConnectionRequest(IEC61850Station station, IEC61850Channel channel, byte[] requestDataPart)
        {
            // Set the presentation selector arrays of station and channel
            byte[] calledSelectorArray = SetSelectorArray(station.PresentationSelector);
            if ((calledSelectorArray == null) || (calledSelectorArray.Length < 1))
            {
                return (null);
            }
            uint calledSelectorSize = (uint)calledSelectorArray.Length;
            byte[] callingSelectorArray = SetSelectorArray(channel.ClientPresentationSelector);
            if ((callingSelectorArray == null) || (callingSelectorArray.Length < 1))
            {
                return (null);
            }
            uint callingSelectorSize = (uint)callingSelectorArray.Length;
            station.SetPresentationSelectorArray(calledSelectorArray);
            channel.SetPresentationSelectorArray(callingSelectorArray);

            // Calculate the length of the encoded message
            // Mode-selector
            uint contentLength = 5;
            // Presentation selectors
            uint normalModeLength = 4 + calledSelectorSize + callingSelectorSize;
            // PCL length
            normalModeLength += 35;
            // Presentation data length
            uint dataLength = (uint)requestDataPart.Length;
            uint encodedDataLength = 4 + dataLength;
            encodedDataLength += BerEncoderCheckLengthSize(dataLength);
            uint fullyEncodedDataLength = encodedDataLength + 1;
            fullyEncodedDataLength += BerEncoderCheckLengthSize(encodedDataLength);
            uint encodedUserDataLength = fullyEncodedDataLength + 1;
            encodedUserDataLength += BerEncoderCheckLengthSize(fullyEncodedDataLength);
            normalModeLength += encodedUserDataLength + 2;
            contentLength += normalModeLength + 1;
            contentLength += BerEncoderCheckLengthSize(normalModeLength);

            // Build the request message
            uint messageTotalLength = 1 + contentLength + BerEncoderCheckLengthSize(contentLength);
            byte[] requestMessage = new byte[messageTotalLength];
            // ISO 8823 OSI Presentation Protocol
            uint messageLength = 0;
            requestMessage[messageLength++] = 0x31;
            byte[] length1Buffer = BerEncoderLength((int)contentLength);
            Array.Copy(length1Buffer, 0, requestMessage, messageLength, length1Buffer.Length);
            messageLength += (uint)length1Buffer.Length;
            // Mode-selector
            requestMessage[messageLength++] = 0xA0;
            requestMessage[messageLength++] = 0x03;
            requestMessage[messageLength++] = 0x80;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x01; // Normal mode
            // Normal-mode parameters
            requestMessage[messageLength++] = 0xA2;
            byte[] length2Buffer = BerEncoderLength((int)normalModeLength);
            Array.Copy(length2Buffer, 0, requestMessage, messageLength, length2Buffer.Length);
            messageLength += (uint)length2Buffer.Length;
            // Calling presentation selector
            requestMessage[messageLength++] = 0x81;
            requestMessage[messageLength++] = (byte)callingSelectorSize;
            Array.Copy(callingSelectorArray, 0, requestMessage, messageLength, callingSelectorSize);
            messageLength += callingSelectorSize;
            // Called presentation selector
            requestMessage[messageLength++] = 0x82;
            requestMessage[messageLength++] = (byte)calledSelectorSize;
            Array.Copy(calledSelectorArray, 0, requestMessage, messageLength, calledSelectorSize);
            messageLength += calledSelectorSize;
            // Presentation Context ID List
            requestMessage[messageLength++] = 0xA4;
            requestMessage[messageLength++] = 0x23;
            // ACSE Context List Item
            requestMessage[messageLength++] = 0x30;
            requestMessage[messageLength++] = 0x0F;
            requestMessage[messageLength++] = 0x02;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x06;
            requestMessage[messageLength++] = 0x04;
            // 0x52, 0x01, 0x00, 0x01 = 2.2.1.0.1 = ID-AS-ACSE
            requestMessage[messageLength++] = 0x52;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x00;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x30;
            requestMessage[messageLength++] = 0x04;
            requestMessage[messageLength++] = 0x06;
            requestMessage[messageLength++] = 0x02;
            // 0x51, 0x01 = 2.1.1 = Basic Encoding (BER ID)
            requestMessage[messageLength++] = 0x51;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x30;
            requestMessage[messageLength++] = 0x10;
            requestMessage[messageLength++] = 0x02;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x03;
            requestMessage[messageLength++] = 0x06;
            requestMessage[messageLength++] = 0x05;
            // 0x28, 0xca, 0x22, 0x02, 0x01 = 1.0.9506.2.1 = MMS abstract syntax ver. 1
            requestMessage[messageLength++] = 0x28;
            requestMessage[messageLength++] = 0xca;
            requestMessage[messageLength++] = 0x22;
            requestMessage[messageLength++] = 0x02;
            requestMessage[messageLength++] = 0x01;
            requestMessage[messageLength++] = 0x30;
            requestMessage[messageLength++] = 0x04;
            requestMessage[messageLength++] = 0x06;
            requestMessage[messageLength++] = 0x02;
            // 0x51, 0x01 = 2.1.1 = Basic Encoding (BER ID)
            requestMessage[messageLength++] = 0x51;
            requestMessage[messageLength++] = 0x01;
            // Fully encoded data
            requestMessage[messageLength++] = 0x61;
            byte[] length3Buffer = BerEncoderLength((int)fullyEncodedDataLength);
            Array.Copy(length3Buffer, 0, requestMessage, messageLength, length3Buffer.Length);
            messageLength += (uint)length3Buffer.Length;
            requestMessage[messageLength++] = 0x30;
            byte[] length4Buffer = BerEncoderLength((int)encodedDataLength);
            Array.Copy(length4Buffer, 0, requestMessage, messageLength, length4Buffer.Length);
            messageLength += (uint)length4Buffer.Length;
            requestMessage[messageLength++] = 0x02;
            requestMessage[messageLength++] = 0x01;
            // 0x01 = Presentation Context Identifier = ID AS ACSE
            requestMessage[messageLength++] = 0x01;
            // Application data length
            requestMessage[messageLength++] = 0xA0;
            byte[] length5Buffer = BerEncoderLength((int)dataLength);
            Array.Copy(length5Buffer, 0, requestMessage, messageLength, length5Buffer.Length);
            messageLength += (uint)length5Buffer.Length;
            if (dataLength > 0)
            {
                // Copy the data part of the message
                Array.Copy(requestDataPart, 0, requestMessage, messageLength, dataLength);
                messageLength += (uint)dataLength;
            }

            return (requestMessage);
        }

        public byte[] AddAssociationControlRequest(IEC61850Station station, IEC61850Channel channel, byte[] requestDataPart)
        {
            if(requestDataPart == null)
            {
                return (null);
            }

            // Set the Application ID arrays of station and channel
            byte[] calledApplicationIdArray = SetOidArray(station.ApplicationID);
            uint calledApplicationIDLength = 0;
            if(calledApplicationIdArray != null)
            {
                calledApplicationIDLength = (uint)calledApplicationIdArray.Length;
            }
            byte[] callingApplicationIdArray = SetOidArray(channel.ClientApplicationID);
            uint callingApplicationIDLength = 0;
            if (callingApplicationIdArray != null)
            {
                callingApplicationIDLength = (uint)callingApplicationIdArray.Length;
            }
            station.SetApplicationIDArray(calledApplicationIdArray);
            channel.SetApplicationIDArray(callingApplicationIdArray);

            // Calculate the data length
            // Application Context Name
            uint contentLength = 9;
            uint calledAEQualifierLength = 0;
            if (calledApplicationIDLength > 0)
            {
                // Called AP Title
                contentLength += 4 + calledApplicationIDLength;
                // Called AP Qualifier
                calledAEQualifierLength = BerEncoderCheckUint32Size(station.AEQualifier);
                contentLength += 4 + calledAEQualifierLength;
            }
            uint callingAEQualifierLength = 0;
            if (callingApplicationIDLength > 0)
            {
                // Calling AP Title
                contentLength += 4 + callingApplicationIDLength;
                // Calling AP Qualifier
                callingAEQualifierLength = BerEncoderCheckUint32Size(channel.ClientAEQualifier);
                contentLength += 4 + callingAEQualifierLength;
            }
            // Authentication Parameters
            uint passwordLength = 0;
            uint encodedPasswordLengthSize = 0;
            if (station.AuthenticationEnabled)
            {
                contentLength += 11;
                string password = station.AuthenticationPassword;
                password.Trim();
                passwordLength = (uint)password.Length;
                if (passwordLength > 0)
                {
                    encodedPasswordLengthSize = BerEncoderCheckLengthSize(passwordLength);
                    contentLength += passwordLength + encodedPasswordLengthSize;
                    uint authenticationValueLength = BerEncoderCheckLengthSize(passwordLength + encodedPasswordLengthSize + 1);
                    contentLength += authenticationValueLength;
                }
            }
            // User Information
            uint userInfoLength = (uint)(requestDataPart.Length + 1);
            userInfoLength += BerEncoderCheckLengthSize((uint)requestDataPart.Length);
            userInfoLength += 7;
            uint associationDataLength = userInfoLength;
            userInfoLength += BerEncoderCheckLengthSize(associationDataLength) + 1;
            uint userLength = userInfoLength;
            userInfoLength += BerEncoderCheckLengthSize(userLength) + 1;
            contentLength += userInfoLength;

            // Build the request message
            uint messageTotalLength = 1 + contentLength + BerEncoderCheckLengthSize(contentLength);
            byte[] requestMessage = new byte[messageTotalLength];
            uint messageLength = 0;
            // ISO 8650-1 OSI Association Control Service
            requestMessage[messageLength++] = 0x60;
            // Content Length
            byte[] contentLengthBuffer = BerEncoderLength((int)contentLength);
            Array.Copy(contentLengthBuffer, 0, requestMessage, messageLength, contentLengthBuffer.Length);
            messageLength += (uint)contentLengthBuffer.Length;
            // Application Context Name
            requestMessage[messageLength++] = 0xa1;
            requestMessage[messageLength++] = 0x07;
            requestMessage[messageLength++] = 0x06;
            requestMessage[messageLength++] = 0x05;
            // 0x28, 0xca, 0x22, 0x02, 0x03 = 1.0.9506.2.3 = MMS
            requestMessage[messageLength++] = 0x28;
            requestMessage[messageLength++] = 0xca;
            requestMessage[messageLength++] = 0x22;
            requestMessage[messageLength++] = 0x02;
            requestMessage[messageLength++] = 0x03;

            if (calledApplicationIDLength > 0)
            {
                // Called AP Title
                requestMessage[messageLength++] = 0xa2;
                byte[] calledIdBuffer1 = BerEncoderLength((int)(calledApplicationIDLength + 2));
                Array.Copy(calledIdBuffer1, 0, requestMessage, messageLength, calledIdBuffer1.Length);
                messageLength += (uint)calledIdBuffer1.Length;
                requestMessage[messageLength++] = 0x06;
                byte[] calledIdBuffer2 = BerEncoderLength((int)(calledApplicationIDLength));
                Array.Copy(calledIdBuffer2, 0, requestMessage, messageLength, calledIdBuffer2.Length);
                messageLength += (uint)calledIdBuffer2.Length;
                Array.Copy(calledApplicationIdArray, 0, requestMessage, messageLength, calledApplicationIdArray.Length);
                messageLength += (uint)calledApplicationIdArray.Length;

                // Called AE Qualifier
                requestMessage[messageLength++] = 0xa3;
                byte[] calledAeBuffer1 = BerEncoderLength((int)(calledAEQualifierLength + 2));
                Array.Copy(calledAeBuffer1, 0, requestMessage, messageLength, calledAeBuffer1.Length);
                messageLength += (uint)calledAeBuffer1.Length;
                requestMessage[messageLength++] = 0x02;
                byte[] calledAeBuffer2 = BerEncoderLength((int)(calledAEQualifierLength));
                Array.Copy(calledAeBuffer2, 0, requestMessage, messageLength, calledAeBuffer2.Length);
                messageLength += (uint)calledAeBuffer2.Length;
                byte[] calledAeValueBuffer = BerEncoderUint32Value(station.AEQualifier);
                Array.Copy(calledAeValueBuffer, 0, requestMessage, messageLength, calledAeValueBuffer.Length);
                messageLength += (uint)calledAeValueBuffer.Length;
            }

            if (callingApplicationIDLength > 0)
            {
                // Calling AP Title
                requestMessage[messageLength++] = 0xa6;
                byte[] callingIdBuffer1 = BerEncoderLength((int)(callingApplicationIDLength + 2));
                Array.Copy(callingIdBuffer1, 0, requestMessage, messageLength, callingIdBuffer1.Length);
                messageLength += (uint)callingIdBuffer1.Length;
                requestMessage[messageLength++] = 0x06;
                byte[] callingIdBuffer2 = BerEncoderLength((int)(callingApplicationIDLength));
                Array.Copy(callingIdBuffer2, 0, requestMessage, messageLength, callingIdBuffer2.Length);
                messageLength += (uint)callingIdBuffer2.Length;
                Array.Copy(callingApplicationIdArray, 0, requestMessage, messageLength, callingApplicationIdArray.Length);
                messageLength += (uint)callingApplicationIdArray.Length;

                // Calling AE Qualifier
                requestMessage[messageLength++] = 0xa7;
                byte[] callingAeBuffer1 = BerEncoderLength((int)(callingAEQualifierLength + 2));
                Array.Copy(callingAeBuffer1, 0, requestMessage, messageLength, callingAeBuffer1.Length);
                messageLength += (uint)callingAeBuffer1.Length;
                requestMessage[messageLength++] = 0x02;
                byte[] callingAeBuffer2 = BerEncoderLength((int)(callingAEQualifierLength));
                Array.Copy(callingAeBuffer2, 0, requestMessage, messageLength, callingAeBuffer2.Length);
                messageLength += (uint)callingAeBuffer2.Length;
                byte[] callingAeValueBuffer = BerEncoderUint32Value(channel.ClientAEQualifier);
                Array.Copy(callingAeValueBuffer, 0, requestMessage, messageLength, callingAeValueBuffer.Length);
                messageLength += (uint)callingAeValueBuffer.Length;
            }

            // Authentication Parameters
            if (station.AuthenticationEnabled)
            {
                // Sender requirements
                requestMessage[messageLength++] = 0x8a;
                requestMessage[messageLength++] = 0x02;
                requestMessage[messageLength++] = 0x04;

                if (passwordLength > 0)
                {
                    requestMessage[messageLength++] = 0x80;
                    requestMessage[messageLength++] = 0x8b;
                    requestMessage[messageLength++] = 0x03;
                    // 0x52, 0x03, 0x01 = Authentication mechanism = Password
                    requestMessage[messageLength++] = 0x52;
                    requestMessage[messageLength++] = 0x03;
                    requestMessage[messageLength++] = 0x01;
                    // Authentication Password
                    requestMessage[messageLength++] = 0xac;
                    byte[] length1Buffer = BerEncoderLength((int)(encodedPasswordLengthSize + passwordLength + 1));
                    Array.Copy(length1Buffer, 0, requestMessage, messageLength, length1Buffer.Length);
                    messageLength += (uint)length1Buffer.Length;
                    requestMessage[messageLength++] = 0x80;
                    byte[] length2Buffer = BerEncoderLength((int)(passwordLength));
                    Array.Copy(length2Buffer, 0, requestMessage, messageLength, length2Buffer.Length);
                    messageLength += (uint)length2Buffer.Length;
                    string password = station.AuthenticationPassword;
                    password.Trim();
                    for (int i = 0; i < passwordLength; i++)
                    {
                        requestMessage[messageLength++] = (byte)password[i];
                    }
                }
                else
                {
                    requestMessage[messageLength++] = 0;
                }
            }

            // Data
            uint dataLength = (uint)requestDataPart.Length;
            if(dataLength > 0)
            {
                // User Information
                requestMessage[messageLength++] = 0xbe;
                byte[] length1Buffer = BerEncoderLength((int)userLength);
                Array.Copy(length1Buffer, 0, requestMessage, messageLength, length1Buffer.Length);
                messageLength += (uint)length1Buffer.Length;

                // Association Data
                requestMessage[messageLength++] = 0x28;
                byte[] length2Buffer = BerEncoderLength((int)associationDataLength);
                Array.Copy(length2Buffer, 0, requestMessage, messageLength, length2Buffer.Length);
                messageLength += (uint)length2Buffer.Length;

                // Direct reference = 0x51, 01 = 2.1.1 = BER
                requestMessage[messageLength++] = 0x06;
                requestMessage[messageLength++] = 0x02;
                requestMessage[messageLength++] = 0x51;
                requestMessage[messageLength++] = 0x01;

                // Indirect reference = 3
                requestMessage[messageLength++] = 0x02;
                requestMessage[messageLength++] = 0x01;
                requestMessage[messageLength++] = 0x03;

                // Type Single ASN1
                requestMessage[messageLength++] = 0xa0;
                byte[] length3Buffer = BerEncoderLength((int)dataLength);
                Array.Copy(length3Buffer, 0, requestMessage, messageLength, length3Buffer.Length);
                messageLength += (uint)length3Buffer.Length;

                // Copy the data part of the message
                Array.Copy(requestDataPart, 0, requestMessage, messageLength, dataLength);
                messageLength += (uint)dataLength;
            }

            return (requestMessage);
        }

        public uint AsnEncoderCheckLongSize(int intValue)
        {
            byte[] buffer = BitConverter.GetBytes(intValue);
            int bufferIndex = 0;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (buffer[bufferIndex])
                {
                    case 0:
                        if ((buffer[bufferIndex - 1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((buffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            return ((uint)(bufferIndex + 1));
        }

        public uint AsnEncoderCheckUnsignedSize(uint uintValue)
        {
            byte[] buffer = BitConverter.GetBytes(uintValue);
            int bufferIndex = 0;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (buffer[bufferIndex])
                {
                    case 0:
                        continue;
                    default:
                        break;
                }

                break;
            }

            return ((uint)(bufferIndex + 1));
        }

        public byte[] AsnEncoderLong(int intValue)
        {
            byte[] buffer = BitConverter.GetBytes(intValue);
            int bufferIndex = 0;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (buffer[bufferIndex])
                {
                    case 0:
                        if ((buffer[bufferIndex - 1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((buffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            // Copy the integer octets
            byte[] encodedValueBuffer = new byte[bufferIndex + 1];
            for (int nOctetsNumber = 0; bufferIndex >= 0; bufferIndex--, nOctetsNumber++)
            {
                encodedValueBuffer[nOctetsNumber] = buffer[bufferIndex];
            }

            return (encodedValueBuffer);
        }

        public byte[] AsnEncoderUnsigned(uint uintValue)
        {
            byte[] buffer = BitConverter.GetBytes(uintValue);
            int bufferIndex = 0;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (buffer[bufferIndex])
                {
                    case 0:
                        continue;
                    default:
                        break;
                }

                break;
            }

            // Copy the integer octets
            byte[] encodedValueBuffer = new byte[bufferIndex + 1];
            for (int nOctetsNumber = 0; bufferIndex >= 0; bufferIndex--, nOctetsNumber++)
            {
                encodedValueBuffer[nOctetsNumber] = buffer[bufferIndex];
            }

            return (encodedValueBuffer);
        }

        byte[] BerEncoderUint32Value(uint value)
        {
            byte[] valueBuffer = new byte[5];
            valueBuffer[0] = 0;
            ushort auxWord = (ushort)(value >> 16);
            valueBuffer[1] = (byte)(auxWord >> 8);
            valueBuffer[2] = (byte)(auxWord);
            auxWord = (ushort)(value);
            valueBuffer[3] = (byte)(auxWord >> 8);
            valueBuffer[4] = (byte)(auxWord);

            uint compressedSize = BerEncoderCheckCompressedIntegerSize(valueBuffer);

            if(compressedSize < 5)
            {
                // Compress the buffer
                uint nOffset = 5 - compressedSize;
                for (uint i = 0; i < compressedSize; i++)
                {
                    valueBuffer[i] = valueBuffer[i + nOffset];
                }
            }
            byte[] compressedValueBuffer = new byte[compressedSize];
            Array.Copy(valueBuffer, 0, compressedValueBuffer, 0, compressedSize);
            return (compressedValueBuffer);
        }

        // Returns the length of an UInt32 value encoded in BER format
        uint BerEncoderCheckUint32Size(uint value)
        {
            byte[] valueBuffer = new byte[5];
            valueBuffer[0] = 0;
            ushort auxWord = (ushort)(value >> 16);
            valueBuffer[1] = (byte)(auxWord >> 8);
            valueBuffer[2] = (byte)(auxWord);
            auxWord = (ushort)(value);
            valueBuffer[3] = (byte)(auxWord >> 8);
            valueBuffer[4] = (byte)(auxWord);

            uint compressedSize = BerEncoderCheckCompressedIntegerSize(valueBuffer);

            return (compressedSize);
        }

        uint BerEncoderCheckCompressedIntegerSize(byte[] valueBuffer)
        {
            if((valueBuffer == null) || (valueBuffer.Count() < 1))
            {
                return (0);
            }

            int originalSize = valueBuffer.Count();
            int i = 0;
            for (i = 0; i < (originalSize - 1); i++)
            {
                if (valueBuffer[i] == 0)
                {
                    if ((valueBuffer[i + 1] & 0x80) == 0)
                    {
                        continue;
                    }
                }
                else if (valueBuffer[i] == 0xFF)
                {
                    if ((valueBuffer[i + 1] & 0x80) != 0)
                    {
                        continue;
                    }
                }

                break;
            }

            uint nNewSize = (uint)(originalSize - i);

            return (nNewSize);
        }

        uint BerEncoderCheckLengthSize(uint length)
        {
            uint lengthSize = 0;

            if (length < 128)
            {
                lengthSize = 1;
            }
            else if (length < 256)
            {
                lengthSize = 2;
            }
            else
            {
                lengthSize = 3;
            }

            return (lengthSize);
        }

        public byte[] BuildMMSInitiateRequest()
        {
            // MMS Initiate PDU
            byte[] mmsInitiatePdu = BuildMMSInitiatePDU();

            // Encode the length value in BER format
            byte[] encodedDataLength = BerEncoderLength(mmsInitiatePdu.Count());

            // Prepare the complete message
            byte[] completeMessage = new byte[1 + encodedDataLength.Count() + mmsInitiatePdu.Count()];
            completeMessage[0] = 0xa8;
            Array.Copy(encodedDataLength, 0, completeMessage, 1, encodedDataLength.Count());
            Array.Copy(mmsInitiatePdu, 0, completeMessage, 1 + encodedDataLength.Count(), mmsInitiatePdu.Count());

            return (completeMessage);
        }

        public byte[] BuildMMSInitiatePDU()
        {
            uint pduLength = 0;
            byte[] pduHeader = new byte[15];

            // Local Detail Calling = 65000
            pduHeader[pduLength++] = 0x80;
            pduHeader[pduLength++] = 0x03;
            pduHeader[pduLength++] = 0;
            pduHeader[pduLength++] = 0xfd;
            pduHeader[pduLength++] = 0xe8;

            // Proposed Max Server Outstanding Calling = 5
            pduHeader[pduLength++] = 0x81;
            pduHeader[pduLength++] = 0x01;
            pduHeader[pduLength++] = 0x05;

            // Proposed Max Server Outstanding Called = 5
            pduHeader[pduLength++] = 0x82;
            pduHeader[pduLength++] = 0x01;
            pduHeader[pduLength++] = 0x05;

            // Proposed Data Structure Nesting Level = 10
            pduHeader[pduLength++] = 0x83;
            pduHeader[pduLength++] = 0x01;
            pduHeader[pduLength++] = 0x0a;

            // MMS Initiate Details
            pduHeader[pduLength++] = 0xa4;
            byte[] pduDataPart = BuildMMSInitiateDetails();

            // Encode the length value in BER format
            byte[] encodedDataLength = BerEncoderLength(pduDataPart.Count());

            // Prepare the complete PDU
            byte[] completePDU = new byte[pduHeader.Count() + encodedDataLength.Count() + pduDataPart.Count()];
            Array.Copy(pduHeader, 0, completePDU, 0, 15);
            Array.Copy(encodedDataLength, 0, completePDU, 15, encodedDataLength.Count());
            Array.Copy(pduDataPart, 0, completePDU, 15 + encodedDataLength.Count(), pduDataPart.Count());

            return (completePDU);
        }

        // Encode a length value in BER format
        // NB: only lengths between 0 and 65535 are managed
        public byte[] BerEncoderLength(int length)
        {
            uint bufferLength = 0;
            if (length < 128)
            {
                bufferLength = 1;
            }
            else if (length < 256)
            {
                bufferLength = 2;
            }
            else
            {
                bufferLength = 3;
            }

            byte[] encodedLength = new byte[bufferLength];

            switch(bufferLength)
            {
                case 1:
                    encodedLength[0] = (byte)length;
                    break;
                case 2:
                    encodedLength[0] = 0x81;
                    encodedLength[1] = (byte)length;
                    break;
                default:
                    encodedLength[0] = 0x82;
                    encodedLength[1] = (byte)(length / 256);
                    encodedLength[2] = (byte)(length % 256);
                    break;
            }

            return (encodedLength);
        }

        public byte[] BuildMMSInitiateDetails()
        {
            uint pduLength = 0;
            byte[] pduDetails = new byte[22];

            // Proposed Version Number = 1
            pduDetails[pduLength++] = 0x80;
            pduDetails[pduLength++] = 0x01;
            pduDetails[pduLength++] = 0x01;

            // Proposed Parameter CBB = 0xf100 (Bit Padding = 5)
            pduDetails[pduLength++] = 0x81;
            pduDetails[pduLength++] = 0x03;
            pduDetails[pduLength++] = 0x05;
            pduDetails[pduLength++] = 0xf1;
            pduDetails[pduLength++] = 0x00;

            // Services Supported Calling = 0x0e08000000000000000000 (Bit Padding = 3)
            pduDetails[pduLength++] = 0x82;
            pduDetails[pduLength++] = 0x0c; // Length
            pduDetails[pduLength++] = 0x03; // Padding
            // Identify, Read, Write, getVariableAccessAttributes
            pduDetails[pduLength++] = 0x2e;
            // getNamedVariableListAttributes
            pduDetails[pduLength++] = 0x08;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;
            pduDetails[pduLength++] = 0;

            return (pduDetails);
        }

        public byte[] AddISOHeader(byte[] requestBuffer)
        {
            int dataLength = 0;
            if((requestBuffer != null) && (requestBuffer.Count() > 0))
            {
                dataLength = requestBuffer.Count();
            }

            byte[] isoRequest = new byte[4 + dataLength];

            // ISO header (4 bytes)
            int messageLength = 0;
            isoRequest[messageLength++] = 0x03; // Version
            isoRequest[messageLength++] = 0; // Reserved
            ushort auxUShort = (ushort)dataLength;
            auxUShort += 4;
            isoRequest[messageLength++] = (byte)(auxUShort >> 8); // Length (High byte)
            isoRequest[messageLength++] = (byte)auxUShort; // Length (Low byte)

            // Add the data part of the message
            if(dataLength > 0)
            {
                Array.Copy(requestBuffer, 0, isoRequest, messageLength, requestBuffer.Count());
            }

            return (isoRequest);
        }

        public byte[] AddCOTPHeader(byte[] requestBuffer)
        {
            int dataLength = 0;
            if ((requestBuffer != null) && (requestBuffer.Length > 0))
            {
                dataLength = requestBuffer.Length;
            }

            byte[] cotpRequest = new byte[3 + dataLength];

            // COTP header (3 bytes)
            int messageLength = 0;
            cotpRequest[messageLength++] = 0x02; // Length
            cotpRequest[messageLength++] = 0xF0; // PDU Type = Data
            cotpRequest[messageLength++] = 0x80; // EOT

            // Add the data part of the message
            if (dataLength > 0)
            {
                Array.Copy(requestBuffer, 0, cotpRequest, messageLength, dataLength);
            }

            return (cotpRequest);
        }

        byte[] SetSelectorArray(string selectorString)
        {
            // Check the selector string
            string outSelector = String.Empty;
            if (CheckSelectorString(selectorString, ref outSelector, MAX_SELECTOR_BYTE_SIZE*2) == false)
            {
                return (null);
            }
            string[] selectorDigits = selectorString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if((selectorDigits == null) || (selectorDigits.Count() < 1))
            {
                return (null);
            }
            byte selectorLength = 0;
            List<byte> selectorBuffer = new List<byte>();
            foreach (string hexVal in selectorDigits)
            {
                if(String.IsNullOrWhiteSpace(hexVal))
                {
                    continue;
                }
                if(hexVal.Length != 2)
                {
                    continue;
                }
                if(!IsHexadecimalDigit(hexVal[0]) || !IsHexadecimalDigit(hexVal[1]))
                {
                    continue;
                }

                try
                {
                    selectorBuffer.Add(Convert.ToByte(hexVal, 16));
                }
                catch
                {
                    return(null);
                }
                selectorLength++;
                if(selectorLength >= MAX_SELECTOR_BYTE_SIZE)
                {
                    break;
                }
            }

            if(selectorBuffer.Count < 1)
            {
                return (null);
            }
            else
            {
                return (selectorBuffer.ToArray());
            }
        }

        // Convert an OID string in a byte array
        byte[] SetOidArray(string oidString)
        {
            uint oidItemCount = CheckApplicationIdString(oidString);
            if(oidItemCount < 2)
            {
                return (null);
            }
            if(oidItemCount > MAX_OID_BYTE_SIZE)
            {
                oidItemCount = MAX_OID_BYTE_SIZE;
            }

            // Convert the OID string in an array of unsigned integer values
            string[] itemArray = oidString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (itemArray == null)
            {
                return (null);
            }
            uint[] uintValues = new uint[oidItemCount];
            uint itemIndex = 0;
            foreach(string item in itemArray)
            {
                if(String.IsNullOrWhiteSpace(item))
                {
                    return (null);
                }

                try
                {
                    uintValues[itemIndex++] = Convert.ToUInt32(item, 10);
                }
                catch
                {
                    return (null);
                }

                if (itemIndex >= oidItemCount)
                {
                    // Done
                    break;
                }
            }
            if(itemIndex < 2)
            {
                return (null);
            }

            // Convert the array of unsigned integer values of the OID in a byte array 
            oidItemCount = itemIndex;
            List<byte> codedValues = new List<byte>();
            itemIndex = 0;
            uint value1 = uintValues[itemIndex++];
            uint value2 = uintValues[itemIndex++];
            value1 = value1 * 40 + value2;
            codedValues.Add((byte)value1);
            while(itemIndex < oidItemCount)
            {
                value1 = uintValues[itemIndex++];
                uint requiredBytes = 0;
                value2 = value1;
                while (value2 > 0)
                {
                    requiredBytes++;
                    value2 >>= 7;
                }
                while (requiredBytes > 0)
                {
                    value2 = value1 >> (byte)(7 * (requiredBytes - 1));
                    value2 &= 0x7f;
                    if (requiredBytes > 1)
                    {
                        value2 += 128;
                    }
                    if (codedValues.Count() == MAX_OID_BYTE_SIZE)
                    {
                        return (null);
                    }
                    codedValues.Add((byte)value2);
                    requiredBytes--;
                }
            }

            return (codedValues.ToArray());
        }

        // Count the number of items (integer numbers) in an OID string
        uint CheckApplicationIdString(string oidString)
        {
            // Check the format of the OID string
            if (String.IsNullOrWhiteSpace(oidString))
            {
                return (0);
            }

            string oid = oidString;
            oid.Trim();
            if(String.IsNullOrWhiteSpace(oid))
            {
                return (0);
            }

            int index = oid.IndexOf(',');
            if(index <= 0)
            {
                return (0);
            }

            // Count the number of items (integer numbers) in an OID string
            string[] itemArray = oid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(itemArray == null)
            {
                return (0);
            }

            return ((uint)itemArray.Count());
        }

        public uint GetFrameLength(IEC61850CommJob job)
        {
            return (0);
        }

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
        
        static int ChecKAnswer(byte[] receivebuffer, IEC61850CommJob job)
        {
            int returnValue = (int)IEC61850ErrorCodes.ErrorUnexpectedReply;
            switch(job.RequestType)
            {
                case RequestTypes.ReadValue:
                    returnValue = (int)DriverErrorCodes.ErrorNoError;
                    break;
                case RequestTypes.WriteValue:
                    returnValue = (int)DriverErrorCodes.ErrorNoError;
                    break;
            }
            return (returnValue);
        }

        public byte[] ReverseBytes(byte[] dataBuffer)
        {
            if((dataBuffer == null) || (dataBuffer.Length < 1))
            {
                return (null);
            }
            byte[] reversedBuffer = new byte[dataBuffer.Length];
            for (int initIndex = 0, endIndex = dataBuffer.Length - 1; endIndex >= 0; initIndex++, endIndex--)
            {
                reversedBuffer[initIndex] = dataBuffer[endIndex];
            }
            return (reversedBuffer);
        }

        public bool ParseData(byte[] receivebuffer, ref IEC61850CommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            bool writeOperation = false;
            if (job.RequestType == RequestTypes.WriteValue)
            {
                writeOperation = true;
            }
            
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
                    return false;
                if (items.Count < (writeOperation ? 1 :2))
                {
                    items[0] = 11;
                    return false;
                }
            }
            
            //check if correct amount of data have been received
            int error = ChecKAnswer(receivebuffer, job);
            if (error != (int)DriverErrorCodes.ErrorNoError && !areArguments)
            { 
                //error
                items.Add(error);
                return false;
            }

            if (areArguments)
            {
                items[0] = error;
            }

            switch(job.RequestType)
            {
                case RequestTypes.ReadValue:
                    error = (int)ParseJobMMSData(receivebuffer, areArguments, ref job, ref items, ref changed);
                    break;
                case RequestTypes.WriteValue:
                    error = (int)DriverErrorCodes.ErrorNoError;
                    break;
                default:
                    error = (int)IEC61850ErrorCodes.ErrorUnexpectedReply;
                    break;
            }

            if (error != (int)DriverErrorCodes.ErrorNoError && !areArguments)
            {
                //error
                items.Add(error);
                return false;
            }

            if (areArguments)
            {
                items[0] = error;
            }
            if(changed.Count > 0)
            {
                items.AddRange(changed);
            }

            return true;
        }

        public bool ParseMMSData(byte[] receivebuffer, out MMSDataTypes dataType, out uint dataBufferLength, out uint dataOffset, out uint padding)
        {
            bool bReturnValue = false;
            dataOffset = 1;
            dataBufferLength = 0;
            dataType = MMSDataTypes.Boolean;
            padding = 0;
            // receivebuffer[0] == Data Access Type
            switch (receivebuffer[0])
            {
                // Float Success
                case 0x87:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        switch (length)
                        {
                            case 0x5:
                                dataOffset += lengthSize;
                                if (receivebuffer.Length >= (dataOffset + length))
                                {
                                    dataOffset++;
                                    dataBufferLength = 4;
                                    dataType = MMSDataTypes.FloatingPoint32Bits;
                                    bReturnValue = true;
                                }
                                break;

                            case 0x9:
                                dataOffset += lengthSize;
                                if (receivebuffer.Length >= (dataOffset + length))
                                {
                                    dataOffset++;
                                    dataBufferLength = 8;
                                    dataType = MMSDataTypes.FloatingPoint64Bits;
                                    bReturnValue = true;
                                }
                                break;
                        }
                    }
                    break;

                // Boolean Success
                case 0x83:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length == 1)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = 1;
                                dataType = MMSDataTypes.Boolean;
                                bReturnValue = true;
                            }
                        }
                    }
                    break;

                // Integer Success
                case 0x85:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if ((length >= 1) && (length <= 4))
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                                if (length == 1)
                                {
                                    dataType = MMSDataTypes.Integer8Bits;
                                    bReturnValue = true;
                                }
                                else if (length == 2)
                                {
                                    dataType = MMSDataTypes.Integer16Bits;
                                    bReturnValue = true;
                                }
                                else
                                {
                                    dataType = MMSDataTypes.Integer32Bits;
                                    bReturnValue = true;
                                }
                            }
                        }
                    }
                    break;

                // Unsigned Integer Success
                case 0x86:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length >= 1)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                                if (length == 1)
                                {
                                    dataType = MMSDataTypes.UnsignedInteger8Bits;
                                    bReturnValue = true;
                                }
                                else if (length == 2)
                                {
                                    dataType = MMSDataTypes.UnsignedInteger16Bits;
                                    bReturnValue = true;
                                }
                                else
                                {
                                    dataType = MMSDataTypes.UnsignedInteger32Bits;
                                    bReturnValue = true;
                                }
                            }
                        }
                    }
                    break;

                // Octet String Success
                case 0x89:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        // Empty string is an acceptable case
                        dataType = MMSDataTypes.OctetString;
                        bReturnValue = true;
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                            }
                        }
                    }
                    break;

                // Visible String Success
                case 0x8a:
                // MMS String Success
                case 0x90:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        // Empty string is an acceptable case
                        dataType = MMSDataTypes.VisibleString;
                        bReturnValue = true;
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                            }
                        }
                    }
                    break;

                // Bit String Success
                case 0x84:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                padding = receivebuffer[dataOffset];
                                dataOffset++;
                                dataBufferLength = length - 1;
                                dataType = MMSDataTypes.BitString;
                                bReturnValue = true;
                            }
                        }
                    }
                    break;

                // UTC Time Success
                case 0x91:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                                dataType = MMSDataTypes.UTCTime;
                                bReturnValue = true;
                            }
                        }
                    }
                    break;

                // Binary Time Success
                case 0x8c:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                                dataType = MMSDataTypes.BinaryTime;
                                bReturnValue = true;
                            }
                        }
                    }
                    break;

                // Structure Success
                case 0xa2:
                    if (receivebuffer.Length >= 2)
                    {
                        uint lengthSize = 0;
                        uint length = BerDecodeLength(receivebuffer, 1, (uint)(receivebuffer.Length - 1), out lengthSize);
                        if (length > 0)
                        {
                            dataOffset += lengthSize;
                            if (receivebuffer.Length >= (dataOffset + length))
                            {
                                dataBufferLength = length;
                                dataType = MMSDataTypes.Structure;
                                bReturnValue = true;
                            }
                        }
                    }
                    break;
            }

            return (bReturnValue);
        }

        public DriverErrorCodes ParseJobMMSData(byte[] receivebuffer, bool areArguments, ref IEC61850CommJob job, ref List<object> items, ref List<Tag> changed)
        {
            DriverErrorCodes returnValue = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
            if((receivebuffer == null) || (receivebuffer.Length == 0))
            {
                if(job.RequestType == RequestTypes.WriteValue)
                {
                    return (DriverErrorCodes.ErrorNoError);
                }
                else
                {
                    return ((DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply);
                }
            }

            uint dataOffset = 1;
            uint dataBufferLength = 0;
            MMSDataTypes dataType = MMSDataTypes.Boolean;
            uint padding = 0;
            // receivebuffer[0] == Data Access Type
            if(!ParseMMSData(receivebuffer, out dataType, out dataBufferLength, out dataOffset, out padding))
            {
                return ((DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply);
            }

            // Copy the received data to the tags of the pending job
            // Empty string is an acceptable case
            if ((dataBufferLength > 0) || (dataType == MMSDataTypes.VisibleString || dataType == MMSDataTypes.OctetString || dataType == MMSDataTypes.MMSString))
            {
                byte[] auxdata = new byte[dataBufferLength];
                receivebuffer.ToList().CopyTo((int)dataOffset, auxdata, 0, (int)dataBufferLength);
                byte[] jobdata = GetDataBuffer(dataType, auxdata, ref job, padding, ref changed);

                if (dataType != MMSDataTypes.Structure)
                {
                    job.SetJobData(jobdata, ref changed);
                }
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
                returnValue = DriverErrorCodes.ErrorNoError;
            }
            return (returnValue);
        }

        byte[] GetDataBuffer(MMSDataTypes receivedDataType, byte[] dataBuffer, ref IEC61850CommJob job, uint padding, ref List<Tag> changed)
        {
            byte[] jobData = null;
            MMSDataTypes expectedDataType = job.MMSDataType;

            switch (expectedDataType)
            {
                case MMSDataTypes.Boolean:
                    if(receivedDataType == MMSDataTypes.Boolean)
                    {
                        jobData = new byte[1];
                        if(dataBuffer.Length > 0)
                        {
                            jobData[0] = dataBuffer[0];
                        }
                    }
                    break;

                case MMSDataTypes.Integer8Bits:
                    {
                        SByte sbyteAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.Integer8Bits:
                                {
                                    jobData = new byte[1];
                                    if (dataBuffer.Length > 0)
                                    {
                                        jobData[0] = dataBuffer[0];
                                    }
                                }
                                break;

                            case MMSDataTypes.Integer16Bits:
                                {
                                    jobData = new byte[1];
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    Int16 shortAux = BitConverter.ToInt16(reversedDataBuffer, 0);
                                    sbyteAux = (SByte)shortAux;
                                    jobData[0] = (byte)sbyteAux;
                                }
                                break;

                            case MMSDataTypes.Integer32Bits:
                                jobData = new byte[1];
                                if (dataBuffer.Length == 4)
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    int intAux = BitConverter.ToInt32(reversedDataBuffer, 0);
                                    sbyteAux = (SByte)intAux;
                                    jobData[0] = (byte)sbyteAux;
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    int intAux = 0;
                                    // Negative value?
                                    if ((dataBuffer[0] & 0x80) != 0)
                                    {
                                        intAux = -1;
                                    }
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        intAux = (intAux << 8) | dataBuffer[i];
                                    }
                                    sbyteAux = (SByte)intAux;
                                    jobData[0] = (byte)sbyteAux;
                                }
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.UnsignedInteger8Bits:
                    {
                        byte byteAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.Integer8Bits:
                            case MMSDataTypes.UnsignedInteger8Bits:
                                {
                                    jobData = new byte[1];
                                    if (dataBuffer.Length > 0)
                                    {
                                        jobData[0] = dataBuffer[0];
                                    }
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger16Bits:
                                {
                                    jobData = new byte[1];
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    UInt16 ushortAux = BitConverter.ToUInt16(reversedDataBuffer, 0);
                                    byteAux = (byte)ushortAux;
                                    jobData[0] = byteAux;
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger32Bits:
                                jobData = new byte[1];
                                if (dataBuffer.Length > 4)
                                {
                                    byte[] auxDataBuffer = new byte[4];
                                    Array.Copy(dataBuffer, dataBuffer.Length - 4, auxDataBuffer, 0, 4);
                                    byte[] reversedDataBuffer = ReverseBytes(auxDataBuffer);
                                    uint uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                    byteAux = (byte)uintAux;
                                    jobData[0] = byteAux;
                                }
                                else if (dataBuffer.Length == 4)
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    uint uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                    byteAux = (byte)uintAux;
                                    jobData[0] = byteAux;
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    uint uintAux = 0;
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        uintAux = (uintAux << 8) | dataBuffer[i];
                                    }
                                    byteAux = (byte)uintAux;
                                    jobData[0] = byteAux;
                                }
                                break;
                            case MMSDataTypes.BitString:
                                jobData = new byte[1];
                                byte aux = dataBuffer[0];
                                aux >>= (byte)padding;
                                jobData[0] = aux;
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.Integer16Bits:
                    {
                        Int16 shortAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.Integer8Bits:
                                {
                                    SByte sbyteAux = (SByte)dataBuffer[0];
                                    shortAux = sbyteAux;
                                    jobData = BitConverter.GetBytes(shortAux);
                                }
                                break;

                            case MMSDataTypes.Integer16Bits:
                                {
                                    jobData = ReverseBytes(dataBuffer);
                                }
                                break;

                            case MMSDataTypes.Integer32Bits:
                                if (dataBuffer.Length == 4)
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    int intAux = BitConverter.ToInt32(reversedDataBuffer, 0);
                                    shortAux = (Int16)intAux;
                                    jobData = BitConverter.GetBytes(shortAux);
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    int intAux = 0;
                                    // Negative value?
                                    if ((dataBuffer[0] & 0x80) != 0)
                                    {
                                        intAux = -1;
                                    }
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        intAux = (intAux << 8) | dataBuffer[i];
                                    }
                                    shortAux = (Int16)intAux;
                                    jobData = BitConverter.GetBytes(shortAux);
                                }
                                break;
                            case MMSDataTypes.BitString:
                                ushort aux = dataBuffer[0];
                                aux <<= 8;
                                aux += dataBuffer[1];
                                aux >>= (byte)padding;
                                jobData = BitConverter.GetBytes(aux);
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.UnsignedInteger16Bits:
                    {
                        UInt16 ushortAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.UnsignedInteger8Bits:
                                {
                                    byte byteAux = dataBuffer[0];
                                    ushortAux = byteAux;
                                    jobData = BitConverter.GetBytes(ushortAux);
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger16Bits:
                                {
                                    jobData = ReverseBytes(dataBuffer);
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger32Bits:
                                if (dataBuffer.Length > 4)
                                {
                                    byte[] auxDataBuffer = new byte[4];
                                    Array.Copy(dataBuffer, dataBuffer.Length - 4, auxDataBuffer, 0, 4);
                                    byte[] reversedDataBuffer = ReverseBytes(auxDataBuffer);
                                    uint uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                    ushortAux = (UInt16)uintAux;
                                    jobData = BitConverter.GetBytes(ushortAux);
                                }
                                else if (dataBuffer.Length == 4)
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    uint uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                    ushortAux = (UInt16)uintAux;
                                    jobData = BitConverter.GetBytes(ushortAux);
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    uint uintAux = 0;
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        uintAux = (uintAux << 8) | dataBuffer[i];
                                    }
                                    ushortAux = (UInt16)uintAux;
                                    jobData = BitConverter.GetBytes(ushortAux);
                                }
                                break;
                            case MMSDataTypes.BitString:
                                ushort aux = dataBuffer[0];
                                aux <<= 8;
                                aux += dataBuffer[1];
                                aux >>= (byte)padding;
                                jobData = BitConverter.GetBytes(aux);
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.Integer32Bits:
                    {
                        Int32 intAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.Integer8Bits:
                                {
                                    SByte sbyteAux = (SByte)dataBuffer[0];
                                    intAux = sbyteAux;
                                    jobData = BitConverter.GetBytes(intAux);
                                }
                                break;

                            case MMSDataTypes.Integer16Bits:
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    Int16 shortAux = BitConverter.ToInt16(reversedDataBuffer, 0);
                                    intAux = shortAux;
                                    jobData = BitConverter.GetBytes(intAux);
                                }
                                break;

                            case MMSDataTypes.Integer32Bits:
                                if (dataBuffer.Length == 4)
                                {
                                    jobData = ReverseBytes(dataBuffer);
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    intAux = 0;
                                    // Negative value?
                                    if ((dataBuffer[0] & 0x80) != 0)
                                    {
                                        intAux = -1;
                                    }
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        intAux = (intAux << 8) | dataBuffer[i];
                                    }
                                    jobData = BitConverter.GetBytes(intAux);
                                }
                                break;
                            case MMSDataTypes.BitString:
                                uint value = 0;
                                for (uint i = 0; (i < dataBuffer.Length) && (i < 4); i++)
                                {
                                    value = (value << 8) | dataBuffer[i];
                                }
                                value >>= (byte)padding;
                                jobData = BitConverter.GetBytes(value);
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.UnsignedInteger32Bits:
                    {
                        UInt32 uintAux = 0;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.UnsignedInteger8Bits:
                                {
                                    byte byteAux = dataBuffer[0];
                                    uintAux = byteAux;
                                    jobData = BitConverter.GetBytes(uintAux);
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger16Bits:
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    UInt16 ushortAux = BitConverter.ToUInt16(reversedDataBuffer, 0);
                                    uintAux = ushortAux;
                                    jobData = BitConverter.GetBytes(uintAux);
                                }
                                break;

                            case MMSDataTypes.UnsignedInteger32Bits:
                                if(dataBuffer.Length > 4)
                                {
                                    byte[] auxDataBuffer = new byte[4];
                                    Array.Copy(dataBuffer, dataBuffer.Length - 4, auxDataBuffer, 0, 4);
                                    jobData = ReverseBytes(auxDataBuffer);
                                }
                                else if (dataBuffer.Length == 4)
                                {
                                    jobData = ReverseBytes(dataBuffer);
                                }
                                else if (dataBuffer.Length == 3)
                                {
                                    uintAux = 0;
                                    for (uint i = 0; i < dataBuffer.Length; i++)
                                    {
                                        uintAux = (uintAux << 8) | dataBuffer[i];
                                    }
                                    jobData = BitConverter.GetBytes(uintAux);
                                }
                                break;
                            case MMSDataTypes.BitString:
                                uint value = 0;
                                for (uint i = 0; (i < dataBuffer.Length) && (i < 4); i++)
                                {
                                    value = (value << 8) | dataBuffer[i];
                                }
                                value >>= (byte)padding;
                                jobData = BitConverter.GetBytes(value);
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.FloatingPoint32Bits:
                    {
                        float floatAux;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.FloatingPoint32Bits:
                                jobData = ReverseBytes(dataBuffer);
                                break;

                            case MMSDataTypes.FloatingPoint64Bits:
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    double doubleAux = BitConverter.ToDouble(reversedDataBuffer, 0);
                                    floatAux = (float)doubleAux;
                                    jobData = BitConverter.GetBytes(floatAux);
                                }
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.FloatingPoint64Bits:
                    {
                        double doubleAux;
                        switch (receivedDataType)
                        {
                            case MMSDataTypes.FloatingPoint32Bits:
                                {
                                    byte[] reversedDataBuffer = ReverseBytes(dataBuffer);
                                    float floatAux = BitConverter.ToSingle(reversedDataBuffer, 0);
                                    doubleAux = (double)floatAux;
                                    jobData = BitConverter.GetBytes(doubleAux);
                                }
                                break;

                            case MMSDataTypes.FloatingPoint64Bits:
                                jobData = ReverseBytes(dataBuffer);
                                break;
                        }
                    }
                    break;

                case MMSDataTypes.BitString:
                    if(receivedDataType == MMSDataTypes.BitString)
                    {
                        // Empty bit-string?
                        if (dataBuffer.Length == 0)
                        {
                            jobData = new byte[dataBuffer.Length];
                        }
                        else
                        {
                            if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String)
                            {
                                jobData = CopyBitStringToASCIIString((uint)dataBuffer.Length, dataBuffer, padding);
                            }
                            else if (job.TagsList[0].TagNode.ArrayDimension > 0)
                            {
                                // Appling padding '0' bits to the last byte of the data buffer
                                jobData = new byte[dataBuffer.Length];
                                Array.Copy(dataBuffer, 0, jobData, 0, dataBuffer.Length);
                                uint aux = dataBuffer[dataBuffer.Length - 1];
                                aux >>= (byte)padding;
                                jobData[dataBuffer.Length - 1] = (byte)aux;
                            }
                            else
                            {
                                if (dataBuffer.Length == 1)
                                {
                                    jobData = new byte[dataBuffer.Length];
                                    byte aux = dataBuffer[0];
                                    aux >>= (byte)padding;
                                    jobData[0] = aux;
                                }
                                else if (dataBuffer.Length == 2)
                                {
                                    ushort aux = dataBuffer[0];
                                    aux <<= 8;
                                    aux += dataBuffer[1];
                                    aux >>= (byte)padding;
                                    jobData = BitConverter.GetBytes(aux);
                                }
                                else
                                {
                                    uint value = 0;
                                    for (uint i = 0; (i < dataBuffer.Length) && (i < 4); i++)
                                    {
                                        value = (value << 8) | dataBuffer[i];
                                    }
                                    value >>= (byte)padding;
                                    jobData = BitConverter.GetBytes(value);
                                }
                            }
                        }
                        break;

                    }
                    break;

                case MMSDataTypes.OctetString:
                    if(receivedDataType == MMSDataTypes.OctetString)
                    {
                        if (job.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.String)
                        {
                            jobData = new byte[dataBuffer.Length];

                            // If not empty string, copy the received data
                            if (dataBuffer.Length > 0)
                            {
                                Array.Copy(dataBuffer, 0, jobData, 0, dataBuffer.Length);
                            }
                        }
                        else
                        {
                            jobData = CopyOctetStringToASCIIString((uint)dataBuffer.Length, dataBuffer);
                        }
                    }
                    break;

                case MMSDataTypes.VisibleString:
                case MMSDataTypes.MMSString:
                    if ((receivedDataType == MMSDataTypes.VisibleString) || (receivedDataType == MMSDataTypes.MMSString))
                    {
                        jobData = new byte[dataBuffer.Length];

                        // If not empty string, copy the received data
                        if (dataBuffer.Length > 0)
                        {
                            Array.Copy(dataBuffer, 0, jobData, 0, dataBuffer.Length);
                        }

                    }
                    break;

                case MMSDataTypes.BinaryTime:
                    if(receivedDataType == MMSDataTypes.BinaryTime)
                    {
                        if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String)
                        {
                            jobData = CopyBinaryTimeToASCIIString((uint)dataBuffer.Length, dataBuffer);
                        }
                        //else if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
                        //{
                        //    CopyStructuredData(nDataType, nDataBufferLength, pDataBuffer,
                        //                       pTag);
                        //    m_bStructuredDataCopied = TRUE;
                        //    return;
                        //}
                        //else
                        //{
                        //    return;
                        //}
                    }
                    break;

                case MMSDataTypes.UTCTime:
                    if (receivedDataType == MMSDataTypes.UTCTime)
                    {
                        if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String)
                        {
                            jobData = CopyUTCTimeToASCIIString((uint)dataBuffer.Length, dataBuffer);
                        }
                        //else if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
                        //{
                        //    CopyStructuredData(nDataType, nDataBufferLength, pDataBuffer,
                        //                       pTag);
                        //    m_bStructuredDataCopied = TRUE;
                        //    return;
                        //}
                        //else
                        //{
                        //    return;
                        //}
                    }
                    break;

                case MMSDataTypes.Structure:
                    if (receivedDataType == MMSDataTypes.Structure)
                    {
                        uint fieldIndex = 0;
                        jobData = CopyStructuredData(receivedDataType, (uint)dataBuffer.Length, dataBuffer, ref job, ref fieldIndex, ref changed);
                    }
                    break;
            }

            return (jobData);
        }

        byte[] CopyStructuredData(MMSDataTypes dataType, uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] structureBuffer = null;
            uint dataOffset = 0;
            switch (dataType)
            {
                case MMSDataTypes.Structure:
                    structureBuffer = CopyStructureData(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                    break;

                //case MMSDataTypes.UTCTime:
                //    structureBuffer = CopyUTCTimeData(dataBufferLength, dataBuffer, ref job, ref fieldIndex);
                //    break;
            }
            return (structureBuffer);
        }

        //byte[] CopyUTCTimeData(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint fieldIndex)
        //{
        //    DateTime dt = ConvertUTCTimeToDateTime(dataBufferLength, dataBuffer);
        //    List<byte> copiedData = new List<byte>();
        //    uint fieldNumber = (uint)job.TagsList.Count();
        //    uint fieldPartialIndex = 0;

        //    while ((fieldIndex + fieldPartialIndex) < fieldNumber)
        //    {
        //        switch(fieldPartialIndex)
        //        {
        //            case 0:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Year);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 1:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Month);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 2:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes((int)dt.DayOfWeek);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 3:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Day);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 4:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Hour);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 5:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Minute);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 6:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Second);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 7:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Millisecond);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //            case 8:
        //                {
        //                    byte[] auxBuffer = BitConverter.GetBytes(dt.Year);
        //                    copiedData.AddRange(auxBuffer);
        //                }
        //                break;
        //        }
        //        fieldPartialIndex++;
        //    }
        //    fieldIndex += fieldPartialIndex;

        //    return (copiedData.ToArray());
        //}

        byte[] CopyStructureData(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            List<byte> copiedData = new List<byte>();
            uint fieldNumber = (uint)job.TagsList.Count();
            while((fieldIndex < fieldNumber) && (dataOffset < dataBufferLength))
            {
                byte[] auxBuffer = null;
                int tagIndex = (int)fieldIndex;
                switch (dataBuffer[dataOffset])
                {
                    // Float
                    case 0x87:
                        dataOffset++;
                        auxBuffer = CopyStructureFloat(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if(auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Boolean
                    case 0x83:
                        dataOffset++;
                        auxBuffer = CopyStructureBoolean(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Integer
                    case 0x85:
                        dataOffset++;
                        auxBuffer = CopyStructureInteger(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Unsigned Integer
                    case 0x86:
                        dataOffset++;
                        auxBuffer = CopyStructureUnsignedInteger(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Octet String
                    case 0x89:
                        dataOffset++;
                        auxBuffer = CopyStructureOctetString(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Visible String
                    case 0x8a:
                    // MMS String
                    case 0x90:
                        dataOffset++;
                        auxBuffer = CopyStructureVisibleString(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Bit String
                    case 0x84:
                        dataOffset++;
                        auxBuffer = CopyStructureBitString(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // UTC Time
                    case 0x91:
                        dataOffset++;
                        auxBuffer = CopyStructureUTCTime(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Binary Time
                    case 0x8c:
                        dataOffset++;
                        auxBuffer = CopyStructureBinaryTime(dataBufferLength, dataBuffer, ref job, ref dataOffset, ref fieldIndex, ref changed);
                        if (auxBuffer != null)
                        {
                            copiedData.AddRange(auxBuffer);
                        }
                        else
                        {
                            // Terminate the parsing loop
                            fieldIndex = fieldNumber;
                        }
                        break;

                    // Structure
                    case 0xa2:
                        if ((dataBufferLength - dataOffset) >= 2)
                        {
                            dataOffset++;
                            uint lengthSize = 0;
                            uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                            if (length > 0)
                            {
                                dataOffset += lengthSize;
                                if (dataBufferLength >= (dataOffset + length))
                                {
                                    // Continue the parsing operation from the next
                                    // field of the nested structure 
                                    //fieldIndex--;
                                }
                            }
                            else
                            {
                                // Parsing operation terminated
                                fieldIndex = fieldNumber;
                            }
                        }
                        else
                        {
                            // Parsing operation terminated
                            fieldIndex = fieldNumber;
                        }
                        break;

                    default: // ?
                        // Parsing operation terminated
                        fieldIndex = fieldNumber;
                        break;
                }
            }
            return (copiedData.ToArray());
        }

        byte[] CopyStructureBinaryTime(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if (length > 0)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = length;
                    }
                    if (dataLength > 0)
                    {
                        byte[] auxdata = new byte[dataLength];
                        Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                        if (job.TagsList[(int)fieldIndex].TagNode.DataType == Opc.Ua.DataTypes.String)
                        {
                            copiedData = CopyBinaryTimeToASCIIString(dataLength, auxdata);
                            dataOffset += dataLength;
                            job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.BinaryTime, ref changed);
                            fieldIndex++;
                        }
                        //else
                        //{
                        //    copiedData = CopyStructuredData(MMSDataTypes.BinaryTime, dataLength, auxdata, ref job, ref fieldIndex);
                        //}
                    }
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureUTCTime(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if (length > 0)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = length;
                    }
                    if (dataLength > 0)
                    {
                        byte[] auxdata = new byte[dataLength];
                        Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                        if (job.TagsList[(int)fieldIndex].TagNode.DataType == Opc.Ua.DataTypes.String)
                        {
                            copiedData = CopyUTCTimeToASCIIString(dataLength, auxdata);
                            dataOffset += dataLength;
                            job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.UTCTime, ref changed);
                            fieldIndex++;
                        }
                        //else
                        //{
                        //    copiedData = CopyStructuredData(MMSDataTypes.UTCTime, dataLength, auxdata, ref job, ref fieldIndex);
                        //}
                    }
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureBitString(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            int dataLength = -1;
            uint padding = 0;
            if ((dataBufferLength - dataOffset) >= 1) // Empty bit string is an acceptable case
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if (length > 0)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        padding = dataBuffer[dataOffset];
                        dataOffset++;
                        dataLength = (int)(length - 1);
                    }
                }
                if(dataLength >= 0)
                {
                    // Empty bit-string?
                    if (dataLength == 0)
                    {
                        copiedData = new byte[dataLength];
                    }
                    else
                    {
                        byte[] auxdata = new byte[dataLength];
                        Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                        if (job.TagsList[(int)fieldIndex].TagNode.DataType == Opc.Ua.DataTypes.String)
                        {
                            copiedData = CopyBitStringToASCIIString((uint)dataLength, auxdata, padding);
                        }
                        else if (job.TagsList[(int)fieldIndex].TagNode.ArrayDimension > 0)
                        {
                            // Appling padding '0' bits to the last byte of the data buffer
                            copiedData = new byte[dataLength];
                            Array.Copy(auxdata, 0, copiedData, 0, dataLength);
                            uint aux = auxdata[dataLength - 1];
                            aux >>= (byte)padding;
                            copiedData[dataLength - 1] = (byte)aux;
                        }
                        else
                        {
                            if (dataLength == 1)
                            {
                                copiedData = new byte[dataLength];
                                byte aux = auxdata[0];
                                aux >>= (byte)padding;
                                copiedData[0] = aux;
                            }
                            else if (dataLength == 2)
                            {
                                ushort aux = auxdata[0];
                                aux <<= 8;
                                aux += auxdata[1];
                                aux >>= (byte)padding;
                                copiedData = BitConverter.GetBytes(aux);
                            }
                            else
                            {
                                uint value = 0;
                                for (uint i = 0; (i < dataLength) && (i < 4); i++)
                                {
                                    value = (value << 8) | auxdata[i];
                                }
                                value >>= (byte)padding;
                                copiedData = BitConverter.GetBytes(value);
                            }
                        }
                    }

                    if(dataLength > 0)
                    {
                        dataOffset += (uint)dataLength;
                    }
                    else
                    {
                        dataOffset++;
                    }
                    job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.BitString, ref changed);
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureVisibleString(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            int dataLength = -1;
            if ((dataBufferLength - dataOffset) >= 1) // Empty string is an acceptable case
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                // Empty string is an acceptable case
                if (length > 0)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = (int)length;
                    }
                }
                else
                {
                    dataLength = 0;
                }

                if (dataLength >= 0)
                {
                    copiedData = new byte[dataLength];
                    if (dataLength > 0)
                    {
                        Array.Copy(dataBuffer, dataOffset, copiedData, 0, dataLength);
                    }

                    if(dataLength > 0)
                    {
                        dataOffset += (uint)dataLength;
                    }
                    else
                    {
                        dataOffset++;
                    }
                    job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.VisibleString, ref changed);
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureOctetString(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            int dataLength = -1;
            if ((dataBufferLength - dataOffset) >= 1) // Empty string is an acceptable case
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                // Empty string is an acceptable case
                if (length > 0)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = (int)length;
                    }
                }
                else
                {
                    dataLength = 0;
                }

                if (dataLength >= 0)
                {
                    byte[] auxdata = new byte[dataLength];
                    if(dataLength > 0)
                    {
                        Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                    }
                    if (job.TagsList[(int)fieldIndex].TagNode.DataType != Opc.Ua.DataTypes.String)
                    {
                        copiedData = new byte[dataBufferLength];

                        // If not empty string, copy the received data
                        if (dataLength > 0)
                        {
                            Array.Copy(auxdata, 0, copiedData, 0, dataLength);
                        }
                    }
                    else
                    {
                        copiedData = CopyOctetStringToASCIIString((uint)dataLength, auxdata);
                    }

                    if(dataLength > 0)
                    {
                        dataOffset += (uint)dataLength;
                    }
                    else
                    {
                        dataOffset++;
                    }
                    job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.OctetString, ref changed);
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] GetUintStructTagBuffer(uint dataValue, ref IEC61850CommJob job, int tagIndex)
        {
            byte[] dataArray = null;
            uint tagType = (uint)Opc.Ua.DataTypes.UInt32;
            lock (job.retLockList())
            {
                if(tagIndex >= job.TagsList.Count)
                {
                    return(null);
                }
                tagType = (uint)job.TagsList[tagIndex].TagNode.DataType.Identifier;
            }

            switch (tagType)
            {
                case (uint)Opc.Ua.DataTypes.SByte:
                    {
                        SByte auxValue = (SByte)dataValue;
                        dataArray = new byte[1];
                        dataArray[0] = (byte)auxValue;
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Byte:
                    {
                        Byte auxValue = (Byte)dataValue;
                        dataArray = new byte[1];
                        dataArray[0] = auxValue;
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int16:
                    {
                        Int16 auxValue = (Int16)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt16:
                    {
                        UInt16 auxValue = (UInt16)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int32:
                    {
                        Int32 auxValue = (Int32)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt32:
                    {
                        dataArray = BitConverter.GetBytes(dataValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int64:
                    {
                        Int64 auxValue = (Int64)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt64:
                    {
                        UInt64 auxValue = (UInt64)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;
            }

            return (dataArray);
        }

        byte[] GetIntStructTagBuffer(int dataValue, ref IEC61850CommJob job, int tagIndex)
        {
            byte[] dataArray = null;
            uint tagType = (uint)Opc.Ua.DataTypes.UInt32;
            lock (job.retLockList())
            {
                if (tagIndex >= job.TagsList.Count)
                {
                    return (null);
                }
                tagType = (uint)job.TagsList[tagIndex].TagNode.DataType.Identifier;
            }

            switch (tagType)
            {
                case (uint)Opc.Ua.DataTypes.SByte:
                    {
                        SByte auxValue = (SByte)dataValue;
                        dataArray = new byte[1];
                        dataArray[0] = (byte)auxValue;
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Byte:
                    {
                        Byte auxValue = (Byte)dataValue;
                        dataArray = new byte[1];
                        dataArray[0] = auxValue;
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int16:
                    {
                        Int16 auxValue = (Int16)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt16:
                    {
                        UInt16 auxValue = (UInt16)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int32:
                    {
                        dataArray = BitConverter.GetBytes(dataValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt32:
                    {
                        UInt32 auxValue = (UInt32)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int64:
                    {
                        Int64 auxValue = (Int64)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt64:
                    {
                        UInt64 auxValue = (UInt64)dataValue;
                        dataArray = BitConverter.GetBytes(auxValue);
                    }
                    break;
            }

            return (dataArray);
        }

        byte[] CopyStructureUnsignedInteger(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            MMSDataTypes dataType = MMSDataTypes.Boolean;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if ((length >= 1) && (length <= 4))
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = length;
                        if (length == 1)
                        {
                            dataType = MMSDataTypes.UnsignedInteger8Bits;
                        }
                        else if (length == 2)
                        {
                            dataType = MMSDataTypes.UnsignedInteger16Bits;
                        }
                        else
                        {
                            dataType = MMSDataTypes.UnsignedInteger32Bits;
                        }
                    }
                }
                if (dataLength > 0)
                {
                    byte[] auxdata = new byte[dataLength];
                    Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                    //switch (dataType)
                    //{
                    //    case MMSDataTypes.UnsignedInteger16Bits:
                    //        copiedData = ReverseBytes(auxdata);
                    //        break;

                    //    case MMSDataTypes.UnsignedInteger32Bits:
                    //        if (dataLength == 4)
                    //        {
                    //            copiedData = ReverseBytes(auxdata);
                    //        }
                    //        else if (dataLength == 3)
                    //        {
                    //            int value = 0;
                    //            for (uint i = 0; i < dataLength; i++)
                    //            {
                    //                value = (value << 8) | auxdata[i];
                    //            }
                    //            copiedData = BitConverter.GetBytes(value);
                    //        }
                    //        break;
                    UInt32 uintAux = 0;
                    switch (dataType)
                    {
                        case MMSDataTypes.UnsignedInteger8Bits:
                            {
                                byte byteAux = auxdata[0];
                                uintAux = byteAux;
                                copiedData = GetUintStructTagBuffer(uintAux, ref job, (int)fieldIndex);
                            }
                            break;

                        case MMSDataTypes.UnsignedInteger16Bits:
                            {
                                byte[] reversedDataBuffer = ReverseBytes(auxdata);
                                UInt16 ushortAux = BitConverter.ToUInt16(reversedDataBuffer, 0);
                                uintAux = ushortAux;
                                copiedData = GetUintStructTagBuffer(uintAux, ref job, (int)fieldIndex);
                            }
                            break;

                        case MMSDataTypes.UnsignedInteger32Bits:
                            if (dataBuffer.Length > 4)
                            {
                                byte[] auxDataBuffer = new byte[4];
                                Array.Copy(dataBuffer, dataBuffer.Length - 4, auxDataBuffer, 0, 4);
                                byte[] reversedDataBuffer = ReverseBytes(auxDataBuffer);
                                uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                copiedData = GetUintStructTagBuffer(uintAux, ref job, (int)fieldIndex);
                            }
                            else if (dataBuffer.Length == 4)
                            {
                                byte[] reversedDataBuffer = ReverseBytes(auxdata);
                                uintAux = BitConverter.ToUInt32(reversedDataBuffer, 0);
                                copiedData = GetUintStructTagBuffer(uintAux, ref job, (int)fieldIndex);
                            }
                            else if (dataBuffer.Length == 3)
                            {
                                uintAux = 0;
                                for (uint i = 0; i < dataBuffer.Length; i++)
                                {
                                    uintAux = (uintAux << 8) | dataBuffer[i];
                                }
                                copiedData = GetUintStructTagBuffer(uintAux, ref job, (int)fieldIndex);
                            }
                            break;

                        default:
                            copiedData = new byte[dataLength];
                            Array.Copy(auxdata, 0, copiedData, 0, dataLength);
                            break;
                    }
                    dataOffset += dataLength;
                    if (copiedData != null)
                    {
                        job.SetJobTagData(copiedData, (int)fieldIndex, dataType, ref changed);
                    }
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureInteger(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            MMSDataTypes dataType = MMSDataTypes.Boolean;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if ((length >= 1) && (length <= 4))
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = length;
                        if (length == 1)
                        {
                            dataType = MMSDataTypes.Integer8Bits;
                        }
                        else if (length == 2)
                        {
                            dataType = MMSDataTypes.Integer16Bits;
                        }
                        else
                        {
                            dataType = MMSDataTypes.Integer32Bits;
                        }
                    }
                }
                if (dataLength > 0)
                {
                    byte[] auxdata = new byte[dataLength];
                    Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                    //switch (dataType)
                    //{
                        //case MMSDataTypes.Integer16Bits:
                        //    copiedData = ReverseBytes(auxdata);
                        //    break;

                        //case MMSDataTypes.Integer32Bits:
                        //    if (dataLength == 4)
                        //    {
                        //        copiedData = ReverseBytes(auxdata);
                        //    }
                        //    else if (dataLength == 3)
                        //    {
                        //        int value = 0;
                        //        // Negative value?
                        //        if ((auxdata[0] & 0x80) != 0)
                        //        {
                        //            value = -1;
                        //        }
                        //        for (uint i = 0; i < dataLength; i++)
                        //        {
                        //            value = (value << 8) | auxdata[i];
                        //        }
                        //        copiedData = BitConverter.GetBytes(value);
                        //    }
                        //    break;
                    Int32 intAux = 0;
                    switch (dataType)
                    {
                        case MMSDataTypes.Integer8Bits:
                            {
                                SByte sbyteAux = (SByte)auxdata[0];
                                intAux = sbyteAux;
                                copiedData = GetIntStructTagBuffer(intAux, ref job, (int)fieldIndex);
                            }
                            break;

                        case MMSDataTypes.Integer16Bits:
                            {
                                byte[] reversedDataBuffer = ReverseBytes(auxdata);
                                Int16 shortAux = BitConverter.ToInt16(reversedDataBuffer, 0);
                                intAux = shortAux;
                                copiedData = GetIntStructTagBuffer(intAux, ref job, (int)fieldIndex);
                            }
                            break;

                        case MMSDataTypes.Integer32Bits:
                            if (dataBuffer.Length == 4)
                            {
                                byte[] reversedDataBuffer = ReverseBytes(auxdata);
                                intAux = BitConverter.ToInt32(reversedDataBuffer, 0);
                                copiedData = GetIntStructTagBuffer(intAux, ref job, (int)fieldIndex);
                            }
                            else if (dataBuffer.Length == 3)
                            {
                                intAux = 0;
                                // Negative value?
                                if ((auxdata[0] & 0x80) != 0)
                                {
                                    intAux = -1;
                                }
                                for (uint i = 0; i < dataLength; i++)
                                {
                                    intAux = (intAux << 8) | auxdata[i];
                                }
                                for (uint i = 0; i < dataBuffer.Length; i++)
                                {
                                    intAux = (intAux << 8) | dataBuffer[i];
                                }
                                copiedData = GetIntStructTagBuffer(intAux, ref job, (int)fieldIndex);
                            }
                            break;


                        default:
                            copiedData = new byte[dataLength];
                            Array.Copy(auxdata, 0, copiedData, 0, dataLength);
                            break;
                    }
                    dataOffset += dataLength;
                    if(copiedData != null)
                    {
                        job.SetJobTagData(copiedData, (int)fieldIndex, dataType, ref changed);
                    }
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureBoolean(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                if (length == 1)
                {
                    dataOffset += lengthSize;
                    if (dataBufferLength >= (dataOffset + length))
                    {
                        dataLength = 1;
                    }
                }
                if (dataLength > 0)
                {
                    copiedData = new byte[dataLength];
                    Array.Copy(dataBuffer, dataOffset, copiedData, 0, dataLength);
                    dataOffset += dataLength;
                    job.SetJobTagData(copiedData, (int)fieldIndex, MMSDataTypes.Boolean, ref changed);
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyStructureFloat(uint dataBufferLength, byte[] dataBuffer, ref IEC61850CommJob job, ref uint dataOffset, ref uint fieldIndex, ref List<Tag> changed)
        {
            byte[] copiedData = null;
            uint dataLength = 0;
            MMSDataTypes mmsType = MMSDataTypes.FloatingPoint32Bits;
            if ((dataBufferLength - dataOffset) >= 2)
            {
                uint lengthSize = 0;
                uint length = BerDecodeLength(dataBuffer, dataOffset, (uint)(dataBufferLength - dataOffset), out lengthSize);
                switch (length)
                {
                    case 0x5:
                        dataOffset += lengthSize;
                        if (dataBufferLength >= (dataOffset + length))
                        {
                            dataOffset++;
                            dataLength = 4;
                            mmsType = MMSDataTypes.FloatingPoint32Bits;
                        }
                        break;

                    case 0x9:
                        dataOffset += lengthSize;
                        if (dataBufferLength >= (dataOffset + length))
                        {
                            dataOffset++;
                            dataLength = 8;
                            mmsType = MMSDataTypes.FloatingPoint64Bits;
                        }
                        break;
                }
                if (dataLength > 0)
                {
                    byte[] auxdata = new byte[dataLength];
                    Array.Copy(dataBuffer, dataOffset, auxdata, 0, dataLength);
                    copiedData = ReverseBytes(auxdata);
                    dataOffset += dataLength;
                    job.SetJobTagData(copiedData, (int)fieldIndex, mmsType, ref changed);
                    fieldIndex++;
                }
            }

            return (copiedData);
        }

        byte[] CopyBitStringToASCIIString(uint dataBufferLength, byte[] dataBuffer, uint padding)
        {
            uint bufferSize = dataBufferLength * 8;
            byte[] buffer = new byte[bufferSize];

            if (dataBufferLength > 0)
            {
                uint bitNumber = 0;
                int bitLimit = 0;
                for (uint i = 0; i < dataBufferLength; i++)
                {
                    if (i == (dataBufferLength - 1) && (padding < 8))
                    {
                        bitLimit = (int)padding;
                    }
                    for (int j = 7; j >= bitLimit; j--)
                    {
                        if (TestBit(dataBuffer[bitNumber / 8], j))
                        {
                            buffer[bitNumber++] = 0x31; // '1'
                        }
                        else
                        {
                            buffer[bitNumber++] = 0x30; // '0'
                        }
                    }
                }
            }

            return (buffer);
        }

        public static bool TestBit(byte byteToBeTested, int bitIndex)
        {
            bool returnValue = false;

            switch (bitIndex)
            {
                case 0:
                    if ((byteToBeTested & 0x01) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 1:
                    if ((byteToBeTested & 0x02) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 2:
                    if ((byteToBeTested & 0x04) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 3:
                    if ((byteToBeTested & 0x08) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 4:
                    if ((byteToBeTested & 0x10) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 5:
                    if ((byteToBeTested & 0x20) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 6:
                    if ((byteToBeTested & 0x40) > 0)
                    {
                        returnValue = true;
                    }
                    break;
                case 7:
                    if ((byteToBeTested & 0x80) > 0)
                    {
                        returnValue = true;
                    }
                    break;
            }

            return (returnValue);
        }

        byte[] CopyBinaryTimeToASCIIString(uint dataBufferLength, byte[] dataBuffer)
        {
            byte[] buffer = new byte[0];

            if (dataBufferLength > 0)
            {
                try
                {
                    DateTime dt = ConvertBinaryTimeToDateTime(dataBufferLength, dataBuffer);
                    string dtString = string.Format("{0}.{1:D3}", dt.ToString(), dt.Millisecond);
                    buffer = ASCIIEncoding.ASCII.GetBytes(dtString);
                }
                catch
                {
                    return (buffer);
                }
            }

            return (buffer);
        }

        byte[] CopyUTCTimeToASCIIString(uint dataBufferLength, byte[] dataBuffer)
        {            
            byte[] buffer = new byte[0];

            if (dataBufferLength > 0)
            {
                try
                {
                    DateTime dt = ConvertUTCTimeToDateTime(dataBufferLength, dataBuffer);
                    string dtString = string.Format("{0}.{1:D3}", dt.ToString(), dt.Millisecond);
                    buffer = ASCIIEncoding.ASCII.GetBytes(dtString);
                }
                catch (Exception ex)
                {
                    return (buffer);
                }
            }

           return(buffer);
        }

        public void ConvertBinaryTimeToDateTime(byte[] dataBuffer, ref DateTime dt)
        {
            uint dataBufferLength = (uint)dataBuffer.Length;
            if (dataBufferLength <= 2)
            {
                return;
            }
            uint timeBufferLength = dataBufferLength - 2;
            byte[] timeBuffer = new byte[timeBufferLength];
            Array.Copy(dataBuffer, 2, timeBuffer, 0, timeBufferLength);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ConvertBinaryTimeToDateTime - timeBufferLength: {1} - timeBuffer: {2}", currentTime, timeBufferLength, string.Join(",", timeBuffer.Select(b => b.ToString("X2")))));
            }
#endif
            dt = ConvertBinaryTimeToDateTime(timeBufferLength, timeBuffer);
        }

        public DateTime ConvertBinaryTimeToDateTime(uint dataBufferLength, byte[] dataBuffer)
        {            
            // Get the number of milliseconds since midnight
            uint dataToBeCopied = 4;
            if (dataToBeCopied >= dataBufferLength)
            {
                dataToBeCopied = dataBufferLength;
            }
            uint elapsedMilliseconds = 0;
            for (uint i = 1; i <= dataToBeCopied; i++)
            {
                elapsedMilliseconds += (uint)(dataBuffer[i - 1] << (byte)((dataToBeCopied - i) * 8));
            }

            // Get the number of days elapsed from GMT midnight January 1, 1984
            uint daysDiff = 0;
            if (dataBufferLength > 4)
            {
                dataToBeCopied = dataBufferLength - 4;
            }
            else
            {
                dataToBeCopied = 0;
            }
            for (uint i = 1; i <= dataToBeCopied; i++)
            {
                daysDiff += (uint)(dataBuffer[4 + i - 1] << (byte)((dataToBeCopied - i) * 8));
            }

            // Calculate the number of ticks elapsed since GMT midnight January 1, 1984 
            DateTime dt = new DateTime(1984, 1, 1).AddDays(daysDiff).AddMilliseconds(elapsedMilliseconds);

            return (dt);
        }

        public void ConvertUTCTimeToDateTime(byte[] dataBuffer, ref DateTime dt)
        {
            uint dataBufferLength = (uint)dataBuffer.Length;
            if(dataBufferLength <= 2)
            {
                return;
            }
            uint timeBufferLength = dataBufferLength - 2;
            byte[] timeBuffer = new byte[timeBufferLength];
            Array.Copy(dataBuffer, 2, timeBuffer, 0, timeBufferLength);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ConvertUTCTimeToDateTime - timeBufferLength: {1} - timeBuffer: {2}", currentTime, timeBufferLength, string.Join(",", timeBuffer.Select(b => b.ToString("X2")))));
            }
#endif
            dt = ConvertUTCTimeToDateTime(timeBufferLength, timeBuffer);
        }

        DateTime ConvertUTCTimeToDateTime(uint dataBufferLength, byte[] dataBuffer)
        {            
            // Get the elapsed number of whole seconds since GMT midnight January 1, 1970
            uint dataToBeCopied = 4;
            if (dataToBeCopied >= dataBufferLength)
            {
                dataToBeCopied = dataBufferLength - 1;
            }
            uint elapsedSeconds = 0;
            for (uint i = 1; i <= dataToBeCopied; i++)
            {
                elapsedSeconds += (uint)(dataBuffer[i - 1] << (byte)((dataToBeCopied - i) * 8));
            }

            // Get the the portion of a second elapsed since the last whole second
            uint fractionOfSecond = 0;
            if (dataBufferLength > 5)
            {
                dataToBeCopied = dataBufferLength - 5;
            }
            else
            {
                dataToBeCopied = 0;
            }
            for (uint i = 1; i <= dataToBeCopied; i++)
            {
                fractionOfSecond += (uint)(dataBuffer[4 + i - 1] << (byte)((dataToBeCopied - i) * 8));
            }

            // Calculate the number of ticks elapsed since GMT midnight January 1, 1970 
            ulong remainder = (ulong)(((fractionOfSecond * 1000.0) / 16777216.0) + 0.5);
                                    
            // Base Date: GMT midnight January 1, 1970
            DateTime dt = new DateTime(1970, 1, 1).AddSeconds(elapsedSeconds).AddMilliseconds(remainder);

            return (dt);
        }

        byte[] CopyOctetStringToASCIIString(uint dataBufferLength, byte[] dataBuffer)
        {
            string byteHexValue = string.Empty;

            for (uint i = 0; i < dataBufferLength; i++)
            {
                uint byteValue = (uint)dataBuffer[i];
                if (i != 0)
                    byteHexValue += string.Format(" {0:X2}", byteValue);
                else
                    byteHexValue += string.Format("{0:X2}", byteValue);
            }

            return (ASCIIEncoding.ASCII.GetBytes(byteHexValue));
        }
        #endregion

        #region static methods

        public static bool IsHexadecimalDigit(Char character)
        {
            switch(character)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'A':
                case 'b':
                case 'B':
                case 'c':
                case 'C':
                case 'd':
                case 'D':
                case 'e':
                case 'E':
                case 'f':
                case 'F':
                    return (true);
                default:
                    return (false);
            }
        }
 
        public static bool CheckSelectorString(string inSelector, ref string outSelector, uint selectorMaxLength)
        {
            outSelector = String.Empty;

            if (String.IsNullOrWhiteSpace(inSelector))
            {
                return (false);
            }

            for(int index=0; index<inSelector.Length; index++)
            {
                if(!Char.IsWhiteSpace(inSelector[index]))
                {
                    if (IsHexadecimalDigit(inSelector[index]))
                    {
                        outSelector += inSelector[index];
                    }
                    else
                    {
                        outSelector = String.Empty;
                        return (false);
                    }
                }
            }
 
            if((outSelector.Length == 0) || ((outSelector.Length%2) > 0) || (outSelector.Length > selectorMaxLength))
            {
                outSelector = String.Empty;
                return (false);
            }
 
            return (true);
        }

        public static bool CheckAPTitleString(string apTitle)
        {
            if (String.IsNullOrWhiteSpace(apTitle))
            {
                return (true);
            }

            for (int index = 0; index < apTitle.Length; index++)
            {
                if (!Char.IsDigit(apTitle[index]))
                {
                    if(apTitle[index] != ',')
                    {
                        return (false);
                    }
                }
            }

            return (true);
        }
        #endregion

    }
}
