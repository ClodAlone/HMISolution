using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using DriverCodeBase;

namespace LacbusPC
{
    public enum LacbusPcUnderlyingProtocols : byte
    {
        LacbusPC = 0,
        LacbusRTU = 1,
        LacbusSofbusSMS = 2,
        SofbusPL = 3
    }

    public enum LacbusPcParsingErrorCode
    {
        ParsingErrorNoError = 0,
        ParsingErrorMessageTooShort = 1,
        ParsingErrorUnknownSMSVersion = 2,
        ParsingErrorUnknownSMSType = 3,
        ParsingErrorUnknownDataFormat = 4,
        ParsingErrorDataFormatVersionMismatch = 5,
        ParsingErrorArchiveTypeVersionMismatch = 6,
        ParsingErrorUnknownArchiveType = 7
    }

    public struct LacbusPCMessageHeader
    {
        public byte headerProtocolVersion;
        public LacbusPcUnderlyingProtocols headerUnderlyingProtocol;
        public byte headerMessageType;
        public UInt16 headerRtuNumber;
        public String headerPhoneNumber;
        public UInt16 headerDataLength;
    }

    public struct LacbusPCDataBlockHeader
    {
        public byte blockType;
        public UInt16 blockBodyLength;
    }

    public struct LacbusPCSequenceElementDatumNumFormatValue
    {
        public UInt16 datumNum;
        public byte datumFormat;
        public bool logicalValue;
        public float floatValue;
        public double doubleValue;
    }

    public struct LacbusSMSCurrentDataStatus
    {
        public UInt16 datumNum;
        public byte datumFormat;
        public bool logicalValue;
        public UInt32 intValue;
        public float floatValue;
        public double doubleValue;
    }

    public struct LacbusPCSequenceElementDatumNumFormatValueLock
    {
        public UInt16 datumNum;
        public byte datumFormat;
        public bool isOutput;
        public bool logicalValue;
        public float floatValue;
        public double doubleValue;
        public byte lockStatus;
    }

    public struct LacbusRtuDataUnitHeader
    {
        public UInt16 DUTotalLength;
        public UInt16 DURtuNumber;
        public byte DUType;
        public byte DUProtocolVersion;
    }

    public struct LacbusSofbusSmsHeader
    {
        public byte SHVersion;
        public byte SHSize;
        public byte SHManufacturer;
        public byte SHSmsNumber;
        public UInt16 SHSiteNumber;
        public byte SHMessageType;
        public byte SHDeviceProductVersion;
        public UInt16 SHDeviceProductType;
        public UInt32 SHDeviceRank;
        public byte SHBatteryType;
        public LacbusPcParsingErrorCode errorCode;
    }

    public struct LacbusSofbusSmsArchivalHeader
    {
        public DateTime SAHInitialValueTimeStamp;
        public uint SAHDataNumber;
        public byte SAHArchivalPeriod;
        public byte SAHNumberOfSamples;
        public byte SAHArchivalType;
    }

    public struct LacbusSofbusSmsReportArchivalHeader
    {
        public byte SRAHNumberOfReportBlocks;
        public byte SRAHHeaderSize;
        public DateTime SRAHTimeStamp;
        public byte SRAHNumberOfDiagnosticData;
    }

    public class LacbusPcProtocol
    {
        public const int LacbusPCMessageHeaderLength = 33;
        public const int LacbusRtuDUHeaderLength = 8;
        public const int LacbusRtuBlockHeaderLength = 3;
        public const int LacbusSofbusSmsVer0HeaderLength = 4;
        public const int LacbusSofbusSmsVer1HeaderLength = 6;
        public const int LacbusSofbusSmsVer2HeaderLength = 10;

        public static uint PrepareLacbusPCMessageHeader(ref byte[] requestBuffer, LacbusPCMessageHeader messageHeader)
        {
            uint headerSize = 0;

            requestBuffer[headerSize++] = (byte)LacbusPCMessageHeaderLength;
            requestBuffer[headerSize++] = messageHeader.headerProtocolVersion;
            requestBuffer[headerSize++] = (byte)messageHeader.headerUnderlyingProtocol;
            requestBuffer[headerSize++] = messageHeader.headerMessageType;
            ushort aux = messageHeader.headerRtuNumber;
            aux >>= 8;
            requestBuffer[headerSize++] = (byte)aux;
            requestBuffer[headerSize++] = (byte)messageHeader.headerRtuNumber;
            int phoneStringLength = 0;
            if(!String.IsNullOrEmpty(messageHeader.headerPhoneNumber))
            {
                phoneStringLength = messageHeader.headerPhoneNumber.Length;
            }
            if(phoneStringLength > 25)
            {
                phoneStringLength = 25;
            }
            int i = 0;
            for(i=0; i<phoneStringLength; i++)
            {
                requestBuffer[headerSize++] = (byte)messageHeader.headerPhoneNumber[i];
            }
            if(phoneStringLength < 25)
            {
                for (i = phoneStringLength; i < 25; i++)
                {
                    requestBuffer[headerSize++] = 0x20;
                }
            }
            aux = messageHeader.headerDataLength;
            aux >>= 8;
            requestBuffer[headerSize++] = (byte)aux;
            requestBuffer[headerSize++] = (byte)messageHeader.headerDataLength;

            return (headerSize);
        }

        public static void PrepareLacbusRtuDuHeader(ref byte[] requestBuffer, LacbusRtuDataUnitHeader duHeader, ref uint frameSize)
        {

            // Data Unit Length
            requestBuffer[frameSize++] = (byte)(duHeader.DUTotalLength >> 8);
            requestBuffer[frameSize++] = (byte)duHeader.DUTotalLength;
            // RTU Number
            requestBuffer[frameSize++] = (byte)(duHeader.DURtuNumber >> 8);
            requestBuffer[frameSize++] = (byte)duHeader.DURtuNumber;
            // Data Unit Type
            requestBuffer[frameSize++] = duHeader.DUType;
            // Reserved
            requestBuffer[frameSize++] = 0;
            // LACBUS-RTU Protocol Version
            requestBuffer[frameSize++] = duHeader.DUProtocolVersion;
            // Reserved
            requestBuffer[frameSize++] = 0;
        }

        public static int ParseSofbusPlDUHeader(byte[] receiveBuffer, int alreadyParsedBytes,
                                                ref LacbusRtuDataUnitHeader rtuHeader)
        {
            // Check the message header length
            if (receiveBuffer.Length < (LacbusRtuDUHeaderLength + alreadyParsedBytes))
            {
                return (0);
            }

            // Get the Data Unit total length
            rtuHeader.DUTotalLength = receiveBuffer[alreadyParsedBytes];
            rtuHeader.DUTotalLength <<= 8;
            rtuHeader.DUTotalLength += receiveBuffer[alreadyParsedBytes + 1];
            // The total length of the Data Unit is the number of words in the DU 
            rtuHeader.DUTotalLength *= 2;

            // Get the RTU number
            rtuHeader.DURtuNumber = receiveBuffer[alreadyParsedBytes + 2];
            rtuHeader.DURtuNumber <<= 8;
            rtuHeader.DURtuNumber += receiveBuffer[alreadyParsedBytes + 3];

            // Get and check the Data Unit Type
            rtuHeader.DUType = receiveBuffer[alreadyParsedBytes + 4];
            if ((rtuHeader.DUType != 'B') && (rtuHeader.DUType != 'H') && (rtuHeader.DUType != 'I'))
            {
                return (0);
            }

            // Get the service data
            rtuHeader.DUProtocolVersion = receiveBuffer[alreadyParsedBytes + 5];

            return (LacbusRtuDUHeaderLength);
        }

        public static int ParseLacbusRtuDUHeader(byte[] receiveBuffer, int alreadyParsedBytes,
                                                 ref LacbusRtuDataUnitHeader rtuHeader)
        {
            // Check the message header length
            if (receiveBuffer.Length < (LacbusRtuDUHeaderLength + alreadyParsedBytes))
            {
                return (0);
            }

            // Get the Data Unit total length
            rtuHeader.DUTotalLength = receiveBuffer[alreadyParsedBytes];
            rtuHeader.DUTotalLength <<= 8;
            rtuHeader.DUTotalLength += receiveBuffer[alreadyParsedBytes + 1];
            // The total length of the Data Unit is the number of words in the DU 
            rtuHeader.DUTotalLength *= 2;

            // Get the RTU number
            rtuHeader.DURtuNumber = receiveBuffer[alreadyParsedBytes + 2];
            rtuHeader.DURtuNumber <<= 8;
            rtuHeader.DURtuNumber += receiveBuffer[alreadyParsedBytes + 3];

            // Get and check the Data Unit Type
            rtuHeader.DUType = receiveBuffer[alreadyParsedBytes + 4];
            if((rtuHeader.DUType != 'H') && (rtuHeader.DUType != 'I') && (rtuHeader.DUType != 'K') && (rtuHeader.DUType != 'P'))
            {
                return (0);
            }

            // Get the protocol version
            rtuHeader.DUProtocolVersion = receiveBuffer[alreadyParsedBytes + 6];

            return (LacbusRtuDUHeaderLength);
        }

        public static int ParseLacbusSofbusSmsHeader(byte[] receiveBuffer, int alreadyParsedBytes,
                                                     ref LacbusSofbusSmsHeader smsHeader)
        {
            // Get the version of the SMS protocol
            if (receiveBuffer.Length < (1 + alreadyParsedBytes))
            {
                smsHeader.errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }
            byte auxByte = receiveBuffer[alreadyParsedBytes];
            auxByte >>= 4;

            // Now that we know the version of the used protocol, we can complete the parsing of the message header  
            smsHeader.SHVersion = auxByte;
            switch(smsHeader.SHVersion)
            {
                case 0:
                    return (ParseLacbusSofbusSmsHeaderVer0(receiveBuffer, alreadyParsedBytes, ref smsHeader));

                case 1:
                    return (ParseLacbusSofbusSmsHeaderVer1(receiveBuffer, alreadyParsedBytes, ref smsHeader));

                case 2:
                    return (ParseLacbusSofbusSmsHeaderVer2(receiveBuffer, alreadyParsedBytes, ref smsHeader));

                default:
                    smsHeader.errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownSMSVersion;
                    return (0);
            }
        }

        public static int ParseLacbusSofbusSmsHeaderVer0(byte[] receiveBuffer, int alreadyParsedBytes,
                                                         ref LacbusSofbusSmsHeader smsHeader)
        {
            // Check the SMS header length
            if (receiveBuffer.Length < (LacbusSofbusSmsVer0HeaderLength + alreadyParsedBytes))
            {
                return (0);
            }

            // Set the header length
            smsHeader.SHSize = LacbusSofbusSmsVer0HeaderLength;

            // Get the manufacturer
            smsHeader.SHManufacturer = (byte)(receiveBuffer[alreadyParsedBytes] & 0x0f);

            // Get the SMS number
            smsHeader.SHSmsNumber = receiveBuffer[alreadyParsedBytes + 1];

            // Get the site number
            smsHeader.SHSiteNumber = receiveBuffer[alreadyParsedBytes + 2];
            smsHeader.SHSiteNumber <<= 4;
            byte auxByte = receiveBuffer[alreadyParsedBytes + 3];
            auxByte >>= 4;
            smsHeader.SHSiteNumber += auxByte;

            // Get the message type
            smsHeader.SHMessageType = (byte)(receiveBuffer[alreadyParsedBytes + 3] & 0x0f);

            // Set to 0 the other fields of the header structure (unused fields)
            smsHeader.SHDeviceProductType = 0;
            smsHeader.SHDeviceProductVersion = 0;
            smsHeader.SHDeviceRank = 0;
            smsHeader.SHBatteryType = 0;

            return (LacbusSofbusSmsVer0HeaderLength);
        }

        public static int ParseLacbusSofbusSmsHeaderVer1(byte[] receiveBuffer, int alreadyParsedBytes,
                                                         ref LacbusSofbusSmsHeader smsHeader)
        {
            // Check the SMS header length
            if (receiveBuffer.Length < (LacbusSofbusSmsVer1HeaderLength + alreadyParsedBytes))
            {
                return (0);
            }

            // Get the header length
            smsHeader.SHSize = (byte)(receiveBuffer[alreadyParsedBytes] & 0x0f);
            smsHeader.SHSize <<= 2;
            byte auxByte = receiveBuffer[alreadyParsedBytes + 1];
            auxByte >>= 6;
            smsHeader.SHSize += auxByte;
            if(smsHeader.SHSize != LacbusSofbusSmsVer1HeaderLength)
            {
                return (0);
            }

            // Get the manufacturer
            auxByte = (byte)(receiveBuffer[alreadyParsedBytes + 1] & 0x3f);
            auxByte >>= 2;
            smsHeader.SHManufacturer = auxByte;

            // Get the SMS number
            smsHeader.SHSmsNumber = (byte)(receiveBuffer[alreadyParsedBytes + 1] & 0x03);
            smsHeader.SHSmsNumber <<= 6;
            auxByte = receiveBuffer[alreadyParsedBytes + 2];
            auxByte >>= 2;
            smsHeader.SHSmsNumber += auxByte;

            // Get the site number
            smsHeader.SHSiteNumber = (byte)(receiveBuffer[alreadyParsedBytes + 2] & 0x03);
            smsHeader.SHSiteNumber <<= 14;
            UInt16 auxUInt16 = receiveBuffer[alreadyParsedBytes + 3];
            auxUInt16 <<= 6;
            smsHeader.SHSiteNumber += auxUInt16;
            auxByte = receiveBuffer[alreadyParsedBytes + 4];
            auxByte >>= 2;
            smsHeader.SHSiteNumber += auxByte;

            // Get the message type
            smsHeader.SHMessageType = (byte)(receiveBuffer[alreadyParsedBytes + 4] & 0x03);
            smsHeader.SHMessageType <<= 2;
            auxByte = receiveBuffer[alreadyParsedBytes + 5];
            auxByte >>= 6;
            smsHeader.SHMessageType += auxByte;

            // Set to 0 the other fields of the header structure (unused fields)
            smsHeader.SHDeviceProductType = 0;
            smsHeader.SHDeviceProductVersion = 0;
            smsHeader.SHDeviceRank = 0;
            smsHeader.SHBatteryType = 0;

            return (LacbusSofbusSmsVer1HeaderLength);
        }

        public static int ParseLacbusSofbusSmsHeaderVer2(byte[] receiveBuffer, int alreadyParsedBytes,
                                                         ref LacbusSofbusSmsHeader smsHeader)
        {
            // Check the SMS header length
            if (receiveBuffer.Length < (LacbusSofbusSmsVer2HeaderLength + alreadyParsedBytes))
            {
                return (0);
            }

            // Get the header length
            smsHeader.SHSize = (byte)(receiveBuffer[alreadyParsedBytes] & 0x0f);
            smsHeader.SHSize <<= 2;
            byte auxByte = receiveBuffer[alreadyParsedBytes + 1];
            auxByte >>= 6;
            smsHeader.SHSize += auxByte;
            if (smsHeader.SHSize != LacbusSofbusSmsVer2HeaderLength)
            {
                return (0);
            }

            // Get the manufacturer
            auxByte = (byte)(receiveBuffer[alreadyParsedBytes + 1] & 0x3f);
            auxByte >>= 2;
            smsHeader.SHManufacturer = auxByte;

            // Get the SMS number
            smsHeader.SHSmsNumber = (byte)(receiveBuffer[alreadyParsedBytes + 1] & 0x03);
            smsHeader.SHSmsNumber <<= 6;
            auxByte = receiveBuffer[alreadyParsedBytes + 2];
            auxByte >>= 2;
            smsHeader.SHSmsNumber += auxByte;

            // Get the site number
            smsHeader.SHSiteNumber = (byte)(receiveBuffer[alreadyParsedBytes + 2] & 0x03);
            smsHeader.SHSiteNumber <<= 14;
            UInt16 auxUInt16 = receiveBuffer[alreadyParsedBytes + 3];
            auxUInt16 <<= 6;
            smsHeader.SHSiteNumber += auxUInt16;
            auxByte = receiveBuffer[alreadyParsedBytes + 4];
            auxByte >>= 2;
            smsHeader.SHSiteNumber += auxByte;

            // Get the message type
            smsHeader.SHMessageType = (byte)(receiveBuffer[alreadyParsedBytes + 4] & 0x03);
            smsHeader.SHMessageType <<= 2;
            auxByte = receiveBuffer[alreadyParsedBytes + 5];
            auxByte >>= 6;
            smsHeader.SHMessageType += auxByte;

            // Get the device product version
            smsHeader.SHDeviceProductVersion = (byte)(receiveBuffer[alreadyParsedBytes + 5] & 0x3f);
            smsHeader.SHDeviceProductVersion <<= 1;
            auxByte = receiveBuffer[alreadyParsedBytes + 6];
            auxByte >>= 7;
            smsHeader.SHDeviceProductVersion += auxByte;

            // Get the device product type
            auxByte = (byte)(receiveBuffer[alreadyParsedBytes + 6] & 0x7f);
            smsHeader.SHDeviceProductType = auxByte;
            smsHeader.SHDeviceProductType <<= 3;
            auxByte = receiveBuffer[alreadyParsedBytes + 7];
            auxByte >>= 5;
            smsHeader.SHDeviceProductType += auxByte;

            // Get the device product rank
            auxByte = (byte)(receiveBuffer[alreadyParsedBytes + 7] & 0x1f);
            smsHeader.SHDeviceRank = auxByte;
            smsHeader.SHDeviceRank <<= 12;
            auxUInt16 = receiveBuffer[alreadyParsedBytes + 8];
            auxUInt16 <<= 4;
            auxByte = receiveBuffer[alreadyParsedBytes + 9];
            auxByte >>= 4;
            auxUInt16 += auxByte;
            smsHeader.SHDeviceRank += auxUInt16;

            // Get the battery type
            smsHeader.SHBatteryType = (byte)(receiveBuffer[alreadyParsedBytes + 9] & 0x0e);
            smsHeader.SHBatteryType >>= 1;

            return (LacbusSofbusSmsVer2HeaderLength);
        }

        public static int ParseLacbusSofbusSmsArchivingHeader(byte[] receiveBuffer, int alreadyParsedBytes,
                                                              ref LacbusSofbusSmsArchivalHeader archivalHeader)
        {
            // Parse the initial value timestamp
            archivalHeader.SAHInitialValueTimeStamp = ParseSMSTimeStamp(receiveBuffer, alreadyParsedBytes);

            // Parse the datum number
            int initialByte = alreadyParsedBytes + 3;
            int numOfBitsInTheInitialByte = 5;
            archivalHeader.SAHDataNumber = LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 10);

            // Parse the Archival Period
            archivalHeader.SAHArchivalPeriod = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 8);

            // Parse the Number of Samples
            archivalHeader.SAHNumberOfSamples = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 8);

            // Parse the Archival Type
            archivalHeader.SAHArchivalType = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 3);

            return (7);
        }

        public static int ParseLacbusSofbusSmsReportArchivingHeader(byte[] receiveBuffer, ref int initialByte, ref int numOfBitsInTheInitialByte,
                                                                    ref LacbusSofbusSmsReportArchivalHeader reportArchivalHeader)
        {
            // Parse the number of report blocks
            reportArchivalHeader.SRAHNumberOfReportBlocks = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 5);

            // Parse the header size
            reportArchivalHeader.SRAHHeaderSize = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 6);

            // Parse the timestamp of the diagnostic data
            reportArchivalHeader.SRAHTimeStamp = ParseSMSDate(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte);

            // Parse the number of diagnostic data
            reportArchivalHeader.SRAHNumberOfDiagnosticData = (byte)LacbusSofbusSmsParseIntFromBits(receiveBuffer, ref initialByte, ref numOfBitsInTheInitialByte, 3);

            return (30);
        }

        public static uint LacbusSofbusSmsParseIntFromBits(byte[] receivedMessage, ref int initialByte, ref int numOfBitsInTheInitialByte, int valueBitSize)
        {
            int i = 0;
            int j = 0;
            uint intValue = 0;
            for(i=numOfBitsInTheInitialByte-1, j=valueBitSize-1; j>=0; i--, j--)
            {
                if(TestBit(receivedMessage[initialByte], i))
                {
                    SetBit(ref intValue, j);
                }
                numOfBitsInTheInitialByte--;
                if((j >= 0) && (numOfBitsInTheInitialByte == 0))
                {
                    initialByte++;
                    i = 8;
                    numOfBitsInTheInitialByte = 8;
                }
            }

            return (intValue);
        }

        public static float LacbusSofbusSmsParseFloatFromBits(byte[] receivedMessage, ref int initialByte, ref int numOfBitsInTheInitialByte)
        {
            float floatValue = 0;
            byte[] buffer = new byte[4];
            int i = 0;
            for(i=0; i<4; i++)
            {
                buffer[i] = 0;
            }
            int j = 0;
            for (i = numOfBitsInTheInitialByte - 1, j = 31; j >= 0; i--, j--)
            {
                if (TestBit(receivedMessage[initialByte], i))
                {
                    SetBit(ref buffer, 4, j);
                }
                numOfBitsInTheInitialByte--;
                if ((j >= 0) && (numOfBitsInTheInitialByte == 0))
                {
                    initialByte++;
                    i = 8;
                    numOfBitsInTheInitialByte = 8;
                }
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseFloatFromBits: buffer = 0x[{0}][{1}][{2}][{3}]",
                                               buffer[3], buffer[2], buffer[1], buffer[0]);
#endif

            floatValue = BitConverter.ToSingle(buffer, 0);

            return (floatValue);
        }

        public static int LacbusSofbusSmsParseCurrentDataStatus(byte[] receivedMessage, ref int initialByte, ref int numOfBitsInTheInitialByte, ref LacbusSMSCurrentDataStatus currentDataStatus, ref LacbusPcParsingErrorCode errorCode)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: initialByte = {0}, numOfBitsInTheInitialByte = {1}",
                                               initialByte, numOfBitsInTheInitialByte);
#endif
            // Parse the datum number
            uint datumNumber = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 10);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: datumNumber = {0}, initialByte = {1}, numOfBitsInTheInitialByte = {2}",
                                               datumNumber, initialByte, numOfBitsInTheInitialByte);
#endif
            // Parse the format
            uint format = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 3);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: format = {0}, initialByte = {1}, numOfBitsInTheInitialByte = {2}",
                                               format, initialByte, numOfBitsInTheInitialByte);
#endif
            // Check the length of the remaining part of the message
            int dataBitLength = 0;
            switch(format)
            {
                case 0:
                    dataBitLength = 1;
                    break;
                case 1:
                    dataBitLength = 8;
                    break;
                case 2:
                    dataBitLength = 10;
                    break;
                case 3:
                    dataBitLength = 12;
                    break;
                case 4:
                    dataBitLength = 16;
                    break;
                case 5:
                case 6:
                    dataBitLength = 32;
                    break;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: dataBitLength = {0}",
                                               dataBitLength);
#endif
            if (dataBitLength == 0)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownDataFormat;
                System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: NULL data length!!!");
                return (0);
            }
            int numOfParsedBits = 13;
            int numOfRemainingBytes = receivedMessage.GetLength(0) - initialByte - 1;
            int numOfRemainingBits = numOfBitsInTheInitialByte;
            if(numOfRemainingBytes > 0)
            {
                numOfRemainingBits += numOfRemainingBytes * 8;
            }
            if(numOfRemainingBits < dataBitLength)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: numOfRemainingBits < dataBitLength !!!");
#endif
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: numOfParsedBits = {0}, numOfRemainingBits = {1}",
                                               numOfParsedBits, numOfRemainingBits);
#endif

            // Parse the value (it can be an integer or a float)
            uint intValue = 0;
            float floatValue = 0;
            double doubleValue = 0; 
            switch (format)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    intValue = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, dataBitLength);
                    // Calculate the converted value
                    //if (format == 0) // 1 bit
                    //{
                    //    floatValue = (float)intValue;
                    //}
                    //else if (format == 1) // 8 bit integer
                    //{
                    //    floatValue = (float)intValue / (float)0xFF * 100.0F;
                    //}
                    //else if (format == 2) // 10 bit integer
                    //{
                    //    floatValue = (float)intValue / (float)0x3FF * 100.0F;
                    //}
                    //else if (format == 3) // 12 bit integer
                    //{
                    //    floatValue = (float)intValue / (float)0xFFF * 100.0F;
                    //}
                    //else if (format == 4) // 16 bit integer
                    //{
                    //    floatValue = (float)intValue / (float)0xFFFF * 100.0F;
                    //}
                    //else // 32 bit integer
                    //{
                    //    floatValue = (float)intValue / (float)0xFFFFFFFF * 100.0F;
                    //}
                    floatValue = (float)intValue;
                    doubleValue = (double)intValue;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: after LacbusSofbusSmsParseIntFromBits initialByte = {0}, numOfBitsInTheInitialByte = {1}",
                                                       initialByte, numOfBitsInTheInitialByte);
#endif
                    break;
                case 6:
                    floatValue = LacbusSofbusSmsParseFloatFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte);
                    doubleValue = (double)floatValue;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: after LacbusSofbusSmsParseFloatFromBits initialByte = {0}, numOfBitsInTheInitialByte = {1}",
                                                       initialByte, numOfBitsInTheInitialByte);
#endif
                    break;
            }
            numOfParsedBits += dataBitLength;
            currentDataStatus.datumNum = (ushort)datumNumber;
            currentDataStatus.datumFormat = (byte)format;
            currentDataStatus.logicalValue = false;
            if((format == 0) && (intValue > 0))
            {
                currentDataStatus.logicalValue = true;
            }
            currentDataStatus.intValue = intValue;
            currentDataStatus.floatValue = floatValue;
            currentDataStatus.doubleValue = doubleValue;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseCurrentDataStatus: Data Status: datum num = {0}, format = {1} logic Val = {2} int val = {3} float val = {4}, double val = {5}, numOfParsedBits = {6}",
                                               currentDataStatus.datumNum, currentDataStatus.datumFormat, currentDataStatus.logicalValue, currentDataStatus.intValue, currentDataStatus.floatValue, currentDataStatus.doubleValue, numOfParsedBits);
#endif

            return (numOfParsedBits);
        }

        public static bool ParseLacbusPCMessageHeader(byte[] receiveBuffer,
                                                      ref LacbusPCMessageHeader messageHeader,
                                                      out int errorCode)
        {
            errorCode = 0;

            // Check the message header length
            if(receiveBuffer.Length < LacbusPCMessageHeaderLength)
            {
                errorCode = (int)LacbusPCErrorCodes.ErrorCodeReceivedIncompleteMessage;
                return (false);
            }
            if(receiveBuffer[0] != LacbusPCMessageHeaderLength)
            {
                errorCode = (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectLacbusPCHeaderLength;
                return (false);
            }

            // Get the protocol version
            if(receiveBuffer[1] != 1)
            {
                errorCode = (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectProtocolVersion;
                return (false);
            }
            messageHeader.headerProtocolVersion = receiveBuffer[1];

            // Check and get the underlying protocol
            if ((receiveBuffer[2] > (byte)LacbusPcUnderlyingProtocols.SofbusPL))
            {
                errorCode = (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectUnderlyingProtocol;
                return (false);
            }
            messageHeader.headerUnderlyingProtocol = (LacbusPcUnderlyingProtocols)receiveBuffer[2];

            // Check and get the message type
            if((receiveBuffer[3] != 'P') &&
               (receiveBuffer[3] != 'G') &&
               (receiveBuffer[3] != 'M') &&
               (receiveBuffer[3] != 'I') &&
               (receiveBuffer[3] != 'U') &&
               (receiveBuffer[3] != 'C') &&
               (receiveBuffer[3] != 'S') &&
               (receiveBuffer[3] != 'E'))
            {
                errorCode = (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectMessageType;
                return (false);
            }
            messageHeader.headerMessageType = receiveBuffer[3];

            // Get the RTU number
            messageHeader.headerRtuNumber = receiveBuffer[4];
            messageHeader.headerRtuNumber <<= 8;
            messageHeader.headerRtuNumber += receiveBuffer[5];

            // Get the phone number
            string phoneNumber = System.Text.Encoding.UTF8.GetString(receiveBuffer, 6, 25);
            messageHeader.headerPhoneNumber = phoneNumber.Trim();

            // Get the data length
            messageHeader.headerDataLength = receiveBuffer[31];
            messageHeader.headerDataLength <<= 8;
            messageHeader.headerDataLength += receiveBuffer[32];

            return (true);
        }

        public static int ParseLacbusPCDataBlockHeader(byte[] receiveBuffer, int alreadyParsedData, ref LacbusPCDataBlockHeader blockHeader)
        {
            int parsedBytes = 0;
            int totalMessageLength = receiveBuffer.GetLength(0);
            if(totalMessageLength >= (alreadyParsedData + 3))
            {
                blockHeader.blockType = receiveBuffer[alreadyParsedData];
                blockHeader.blockBodyLength = receiveBuffer[alreadyParsedData + 1];
                blockHeader.blockBodyLength <<= 8;
                blockHeader.blockBodyLength += receiveBuffer[alreadyParsedData + 2];
                parsedBytes = 3;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ParseLacbusPCDataBlockHeader - Error: Total Message Length = {0}, Parsed Bytes = {1}", totalMessageLength, alreadyParsedData);
            }
            return (parsedBytes);
        }

        public static int ParseLacbusPCSequenceElementDatumNumFormatValueLock(byte[] receiveBuffer, int alreadyParsedData, ref LacbusPCSequenceElementDatumNumFormatValueLock sequenceElement)
        {
            int parsedBytes = 0;
            int totalMessageLength = receiveBuffer.GetLength(0);
            if (totalMessageLength >= (alreadyParsedData + 3))
            {
                sequenceElement.datumNum = receiveBuffer[alreadyParsedData];
                sequenceElement.datumNum <<= 8;
                sequenceElement.datumNum += (ushort)(receiveBuffer[alreadyParsedData + 1] & 0xf0);
                sequenceElement.datumNum >>= 4;
                sequenceElement.datumFormat = (byte)(receiveBuffer[alreadyParsedData + 1] & 0x06);
                sequenceElement.datumFormat >>= 1;
                sequenceElement.lockStatus = 0;
                sequenceElement.isOutput = false;
                if((receiveBuffer[alreadyParsedData + 1] & 0x01) > 0)
                {
                    sequenceElement.isOutput = true;
                }
                switch (sequenceElement.datumFormat)
                {
                    case 1: // logical value
                        if (totalMessageLength >= (alreadyParsedData + 3))
                        {
                            if (receiveBuffer[alreadyParsedData + 2] == 0)
                            {
                                sequenceElement.logicalValue = false;
                            }
                            else
                            {
                                sequenceElement.logicalValue = true;
                            }
                            parsedBytes = 3;

                            // Lock status
                            if ((sequenceElement.isOutput == true) && (totalMessageLength >= (alreadyParsedData + parsedBytes + 1)))
                            {
                                sequenceElement.lockStatus = receiveBuffer[alreadyParsedData + parsedBytes];
                                parsedBytes++;
                            }
                        }
                        break;
                    case 2: // double value
                        if (totalMessageLength >= (alreadyParsedData + 10))
                        {
                            byte[] auxBuffer = new byte[8];
                            for (int i = 0, j = 9; i < 8; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.doubleValue = BitConverter.ToDouble(auxBuffer, 0);
                            parsedBytes = 10;

                            // Lock status
                            if ((sequenceElement.isOutput == true) && (totalMessageLength >= (alreadyParsedData + parsedBytes + 1)))
                            {
                                sequenceElement.lockStatus = receiveBuffer[alreadyParsedData + parsedBytes];
                                parsedBytes++;
                            }
                        }
                        break;
                    case 3: // float value
                        if (totalMessageLength >= (alreadyParsedData + 6))
                        {
                            byte[] auxBuffer = new byte[4];
                            for (int i = 0, j = 5; i < 4; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.floatValue = BitConverter.ToSingle(auxBuffer, 0);
                            parsedBytes = 6;

                            // Lock status
                            if ((sequenceElement.isOutput == true) && (totalMessageLength >= (alreadyParsedData + parsedBytes + 1)))
                            {
                                sequenceElement.lockStatus = receiveBuffer[alreadyParsedData + parsedBytes];
                                parsedBytes++;
                            }
                        }
                        break;
                }
            }
            return (parsedBytes);
        }

        public static int ParseLacbusRTUSequenceElementDatumNumFormatValue(byte[] receiveBuffer, int alreadyParsedData, ref LacbusPCSequenceElementDatumNumFormatValueLock sequenceElement)
        {
            int parsedBytes = 0;
            int totalMessageLength = receiveBuffer.GetLength(0);
            if (totalMessageLength >= (alreadyParsedData + 3))
            {
                sequenceElement.datumNum = receiveBuffer[alreadyParsedData];
                sequenceElement.datumNum <<= 8;
                sequenceElement.datumNum += (ushort)(receiveBuffer[alreadyParsedData + 1] & 0xf0);
                sequenceElement.datumNum >>= 4;
                sequenceElement.datumFormat = (byte)(receiveBuffer[alreadyParsedData + 1] & 0x06);
                sequenceElement.datumFormat >>= 1;
                sequenceElement.lockStatus = 0;
                sequenceElement.isOutput = false;
                if ((receiveBuffer[alreadyParsedData + 1] & 0x01) > 0)
                {
                    sequenceElement.isOutput = true;
                }
                switch (sequenceElement.datumFormat)
                {
                    case 1: // logical value
                        if (totalMessageLength >= (alreadyParsedData + 3))
                        {
                            if (receiveBuffer[alreadyParsedData + 2] == 0)
                            {
                                sequenceElement.logicalValue = false;
                            }
                            else
                            {
                                sequenceElement.logicalValue = true;
                            }
                            parsedBytes = 3;
                        }
                        break;
                    case 2: // double value
                        if (totalMessageLength >= (alreadyParsedData + 10))
                        {
                            byte[] auxBuffer = new byte[8];
                            for (int i = 0, j = 9; i < 8; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.doubleValue = BitConverter.ToDouble(auxBuffer, 0);
                            parsedBytes = 10;
                        }
                        break;
                    case 3: // float value
                        if (totalMessageLength >= (alreadyParsedData + 6))
                        {
                            byte[] auxBuffer = new byte[4];
                            for (int i = 0, j = 5; i < 4; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.floatValue = BitConverter.ToSingle(auxBuffer, 0);
                            parsedBytes = 6;
                        }
                        break;
                }
            }
            return (parsedBytes);
        }

        public static int ParseLacbusPCSequenceElementDatumNumFormatValue(byte[] receiveBuffer, int alreadyParsedData, ref LacbusPCSequenceElementDatumNumFormatValue sequenceElement)
        {
            int parsedBytes = 0;
            int totalMessageLength = receiveBuffer.GetLength(0);
            if (totalMessageLength >= (alreadyParsedData + 4))
            {
                sequenceElement.datumNum = receiveBuffer[alreadyParsedData];
                sequenceElement.datumNum <<= 8;
                sequenceElement.datumNum += receiveBuffer[alreadyParsedData + 1];
                sequenceElement.datumFormat = receiveBuffer[alreadyParsedData + 2];
                switch(sequenceElement.datumFormat)
                {
                    case 1: // logical value
                        if(receiveBuffer[alreadyParsedData + 3] == 0)
                        {
                            sequenceElement.logicalValue = false;
                        }
                        else
                        {
                            sequenceElement.logicalValue = true;
                        }
                        parsedBytes = 4;
                        break;
                    case 2: // double value
                        if(totalMessageLength >= (alreadyParsedData + 11))
                        {
                            byte[] auxBuffer = new byte[8];
                            for(int i=0, j=10; i<8; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.doubleValue = BitConverter.ToDouble(auxBuffer, 0);
                            parsedBytes = 11;
                        }
                        break;
                    case 3: // float value
                        if (totalMessageLength >= (alreadyParsedData + 7))
                        {
                            byte[] auxBuffer = new byte[4];
                            for (int i = 0, j = 6; i < 4; i++, j--)
                            {
                                auxBuffer[i] = receiveBuffer[alreadyParsedData + j];
                            }
                            sequenceElement.floatValue = BitConverter.ToSingle(auxBuffer, 0);
                            parsedBytes = 7;
                        }
                        break;
                }
            }
            return (parsedBytes);
        }

        public static UInt64 CalculateJobSearchKey(ushort rtuNumber, ushort datumNumber, byte datumType, byte datumCategory)
        {
            UInt64 searchKey = rtuNumber;
            searchKey <<= 32;
            UInt64 aux = datumType;
            aux <<= 24;
            searchKey += aux;
            aux = datumCategory;
            aux <<= 16;
            searchKey += aux;
            searchKey += datumNumber; 
            return searchKey;
        }

        public static bool ConvertByteArrayToDoubleValue(byte[] valueArray, BuiltInType sourceValueType, ref double doubleValue)
        {
            bool valueConverted = true;
            doubleValue = 0.0;

            switch (sourceValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolAux = BitConverter.ToBoolean(valueArray, 0);
                        if (boolAux == true)
                        {
                            doubleValue = 1.0;
                        }
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteAux = (SByte)valueArray[0];
                        doubleValue = (double)sbyteAux;
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteAux = valueArray[0];
                        doubleValue = (double)byteAux;
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Aux = BitConverter.ToInt16(valueArray, 0);
                        doubleValue = (double)int16Aux;
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Aux = BitConverter.ToUInt16(valueArray, 0);
                        doubleValue = (double)uint16Aux;
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float floatAux = BitConverter.ToSingle(valueArray, 0);
                        doubleValue = (double)floatAux;
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Aux = BitConverter.ToUInt32(valueArray, 0);
                        doubleValue = (double)uint32Aux;
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Aux = BitConverter.ToInt32(valueArray, 0);
                        doubleValue = (double)int32Aux;
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Aux = BitConverter.ToUInt64(valueArray, 0);
                        doubleValue = (double)uint64Aux;
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Aux = BitConverter.ToInt64(valueArray, 0);
                        doubleValue = (double)int64Aux;
                    }
                    break;
                case BuiltInType.Double:
                    {
                        doubleValue = BitConverter.ToDouble(valueArray, 0);
                    }
                    break;

                default:
                    valueConverted = false;
                    break;
            }

            return (valueConverted);
        }

        public static bool ConvertByteArrayToFloatValue(byte[] valueArray, BuiltInType sourceValueType, ref float floatValue)
        {
            bool valueConverted = true;
            floatValue = 0.0F;

            switch (sourceValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolAux = BitConverter.ToBoolean(valueArray, 0);
                        if (boolAux == true)
                        {
                            floatValue = 1.0F;
                        }
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteAux = (SByte)valueArray[0];
                        floatValue = (float)sbyteAux;
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteAux = valueArray[0];
                        floatValue = (float)byteAux;
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Aux = BitConverter.ToInt16(valueArray, 0);
                        floatValue = (float)int16Aux;
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Aux = BitConverter.ToUInt16(valueArray, 0);
                        floatValue = (float)uint16Aux;
                    }
                    break;
                case BuiltInType.Float:
                    {
                        floatValue = BitConverter.ToSingle(valueArray, 0);
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Aux = BitConverter.ToUInt32(valueArray, 0);
                        floatValue = (float)uint32Aux;
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Aux = BitConverter.ToInt32(valueArray, 0);
                        floatValue = (float)int32Aux;
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Aux = BitConverter.ToUInt64(valueArray, 0);
                        floatValue = (float)uint64Aux;
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Aux = BitConverter.ToInt64(valueArray, 0);
                        floatValue = (float)int64Aux;
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double doubleAux = BitConverter.ToDouble(valueArray, 0);
                        floatValue = (float)doubleAux;
                    }
                    break;

                default:
                    valueConverted = false;
                    break;
            }

            return (valueConverted);
        }

        public static bool ConvertByteArrayToInt16Value(byte[] valueArray, BuiltInType sourceValueType, ref Int16 int16Value)
        {
            bool valueConverted = true;
            int16Value = 0;

            switch (sourceValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolAux = BitConverter.ToBoolean(valueArray, 0);
                        if (boolAux == true)
                        {
                            int16Value = 1;
                        }
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteAux = (SByte)valueArray[0];
                        int16Value = (Int16)sbyteAux;
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteAux = valueArray[0];
                        int16Value = (Int16)byteAux;
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        int16Value = BitConverter.ToInt16(valueArray, 0);
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Aux = BitConverter.ToUInt16(valueArray, 0);
                        int16Value = (Int16)uint16Aux;
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float floatValue = BitConverter.ToSingle(valueArray, 0);
                        int16Value = (Int16)floatValue;
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Aux = BitConverter.ToUInt32(valueArray, 0);
                        int16Value = (Int16)uint32Aux;
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Aux = BitConverter.ToInt32(valueArray, 0);
                        int16Value = (Int16)int32Aux;
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Aux = BitConverter.ToUInt64(valueArray, 0);
                        int16Value = (Int16)uint64Aux;
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Aux = BitConverter.ToInt64(valueArray, 0);
                        int16Value = (Int16)int64Aux;
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double doubleAux = BitConverter.ToDouble(valueArray, 0);
                        int16Value = (Int16)doubleAux;
                    }
                    break;

                default:
                    valueConverted = false;
                    break;
            }

            return (valueConverted);
        }

        public static bool ConvertByteArrayToInt32Value(byte[] valueArray, BuiltInType sourceValueType, ref Int32 int32Value)
        {
            bool valueConverted = true;
            int32Value = 0;

            switch (sourceValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolAux = BitConverter.ToBoolean(valueArray, 0);
                        if (boolAux == true)
                        {
                            int32Value = 1;
                        }
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteAux = (SByte)valueArray[0];
                        int32Value = (Int32)sbyteAux;
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteAux = valueArray[0];
                        int32Value = (Int32)byteAux;
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = BitConverter.ToInt16(valueArray, 0);
                        int32Value = (Int32)int16Value;
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Aux = BitConverter.ToUInt16(valueArray, 0);
                        int32Value = (Int32)uint16Aux;
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float floatValue = BitConverter.ToSingle(valueArray, 0);
                        int32Value = (Int32)floatValue;
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Aux = BitConverter.ToUInt32(valueArray, 0);
                        int32Value = (Int32)uint32Aux;
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        int32Value = BitConverter.ToInt32(valueArray, 0);
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Aux = BitConverter.ToUInt64(valueArray, 0);
                        int32Value = (Int32)uint64Aux;
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Aux = BitConverter.ToInt64(valueArray, 0);
                        int32Value = (Int32)int64Aux;
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double doubleAux = BitConverter.ToDouble(valueArray, 0);
                        int32Value = (Int32)doubleAux;
                    }
                    break;

                default:
                    valueConverted = false;
                    break;
            }

            return (valueConverted);
        }

        private static byte[] AddLockStatusToByteArray(byte[] valueArray, byte lockStatus)
        {
            byte[] lockArray = null;
            int arrayLength = valueArray.GetLength(0);
            lockArray = new byte[arrayLength + 1];
            int i = 0;
            for (i = 0; i < arrayLength; i++)
            {
                lockArray[i] = valueArray[i];
            }
            lockArray[arrayLength] = lockStatus;

            return (lockArray);
        }

        public static byte[] ConvertBoolValueWithLockStatusToByteArray(BuiltInType destinationValueType, bool boolValue, byte lockStatus, ref bool valueConverted)
        {
            valueConverted = true;
            byte[] valueArray = null;
            byte[] valueLockArray = null;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        valueArray = BitConverter.GetBytes(boolValue);
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = 0;
                        if (boolValue == true)
                        {
                            sbyteValue = 1;
                        }
                        valueArray = BitConverter.GetBytes(sbyteValue);
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteValue = 0;
                        if (boolValue == true)
                        {
                            byteValue = 1;
                        }
                        valueArray = BitConverter.GetBytes(byteValue);
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = 0;
                        if (boolValue == true)
                        {
                            int16Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(int16Value);
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = 0;
                        if (boolValue == true)
                        {
                            uint16Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(uint16Value);
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float floatValue = 0.0F;
                        if (boolValue == true)
                        {
                            floatValue = 1.0F;
                        }
                        valueArray = BitConverter.GetBytes(floatValue);
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = 0;
                        if (boolValue == true)
                        {
                            uint32Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(uint32Value);
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = 0;
                        if (boolValue == true)
                        {
                            int32Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(int32Value);
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = 0;
                        if (boolValue == true)
                        {
                            uint64Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(uint64Value);
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = 0;
                        if (boolValue == true)
                        {
                            int64Value = 1;
                        }
                        valueArray = BitConverter.GetBytes(int64Value);
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double doubleValue = 0.0;
                        if (boolValue == true)
                        {
                            doubleValue = 1.0;
                        }
                        valueArray = BitConverter.GetBytes(doubleValue);
                    }
                    break;
                default:
                    valueConverted = false;
                    break;
            }

            if(valueArray != null)
            {
                valueLockArray = AddLockStatusToByteArray(valueArray, lockStatus);
            }

            return (valueLockArray);
        }

        public static byte[] ConvertBoolValueToByteArray(BuiltInType destinationValueType, bool boolValue, ref bool valueConverted)
        {
            valueConverted = true;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        return (BitConverter.GetBytes(boolValue));
                    }
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = 0;
                        if(boolValue == true)
                        {
                            sbyteValue = 1;
                        }
                        return (BitConverter.GetBytes(sbyteValue));
                    }
                case BuiltInType.Byte:
                    {
                        byte byteValue = 0;
                        if (boolValue == true)
                        {
                            byteValue = 1;
                        }
                        return (BitConverter.GetBytes(byteValue));
                    }
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = 0;
                        if (boolValue == true)
                        {
                            int16Value = 1;
                        }
                        return (BitConverter.GetBytes(int16Value));
                    }
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = 0;
                        if (boolValue == true)
                        {
                            uint16Value = 1;
                        }
                        return (BitConverter.GetBytes(uint16Value));
                    }
                case BuiltInType.Float:
                    {
                        float floatValue = 0.0F;
                        if (boolValue == true)
                        {
                            floatValue = 1.0F;
                        }
                        return (BitConverter.GetBytes(floatValue));
                    }
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = 0;
                        if (boolValue == true)
                        {
                            uint32Value = 1;
                        }
                        return (BitConverter.GetBytes(uint32Value));
                    }
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = 0;
                        if (boolValue == true)
                        {
                            int32Value = 1;
                        }
                        return (BitConverter.GetBytes(int32Value));
                    }
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = 0;
                        if (boolValue == true)
                        {
                            uint64Value = 1;
                        }
                        return (BitConverter.GetBytes(uint64Value));
                    }
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = 0;
                        if (boolValue == true)
                        {
                            int64Value = 1;
                        }
                        return (BitConverter.GetBytes(int64Value));
                    }
                case BuiltInType.Double:
                    {
                        double doubleValue = 0.0;
                        if (boolValue == true)
                        {
                            doubleValue = 1.0;
                        }
                        return (BitConverter.GetBytes(doubleValue));
                    }
                default:
                    valueConverted = false;
                    return(null);
            }
        }

        public static byte[] ConvertDoubleValueWithLockStatusToByteArray(BuiltInType destinationValueType, double doubleValue, byte lockStatus, ref bool valueConverted)
        {
            valueConverted = true;
            byte[] valueArray = null;
            byte[] valueLockArray = null;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if (doubleValue != 0.0)
                        {
                            boolValue = true;
                        }
                        valueArray = BitConverter.GetBytes(boolValue);
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)doubleValue;
                        valueArray = BitConverter.GetBytes(sbyteValue);
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)doubleValue;
                        valueArray = BitConverter.GetBytes(byteValue);
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)doubleValue;
                        valueArray = BitConverter.GetBytes(int16Value);
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = (UInt16)doubleValue;
                        valueArray = BitConverter.GetBytes(uint16Value);
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float floatValue = (float)doubleValue;
                        valueArray = BitConverter.GetBytes(floatValue);
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = (UInt32)doubleValue;
                        valueArray = BitConverter.GetBytes(uint32Value);
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)doubleValue;
                        valueArray = BitConverter.GetBytes(int32Value);
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)doubleValue;
                        valueArray = BitConverter.GetBytes(uint64Value);
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)doubleValue;
                        valueArray = BitConverter.GetBytes(int64Value);
                    }
                    break;
                case BuiltInType.Double:
                    {
                        valueArray = BitConverter.GetBytes(doubleValue);
                    }
                    break;
                default:
                    valueConverted = false;
                    break;
            }

            if (valueArray != null)
            {
                valueLockArray = AddLockStatusToByteArray(valueArray, lockStatus);
            }

            return (valueLockArray);
        }

        public static byte[] ConvertDoubleValueToByteArray(BuiltInType destinationValueType, double doubleValue, ref bool valueConverted)
        {
            valueConverted = true;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if(doubleValue != 0.0)
                        {
                            boolValue = true;
                        }
                        return (BitConverter.GetBytes(boolValue));
                    }
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)doubleValue;
                        return (BitConverter.GetBytes(sbyteValue));
                    }
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)doubleValue;
                        return (BitConverter.GetBytes(byteValue));
                    }
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)doubleValue;
                        return (BitConverter.GetBytes(int16Value));
                    }
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = (UInt16)doubleValue;
                        return (BitConverter.GetBytes(uint16Value));
                    }
                case BuiltInType.Float:
                    {
                        float floatValue = (float)doubleValue;
                        return (BitConverter.GetBytes(floatValue));
                    }
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = (UInt32)doubleValue;
                        return (BitConverter.GetBytes(uint32Value));
                    }
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)doubleValue;
                        return (BitConverter.GetBytes(int32Value));
                    }
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)doubleValue;
                        return (BitConverter.GetBytes(uint64Value));
                    }
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)doubleValue;
                        return (BitConverter.GetBytes(int64Value));
                    }
                case BuiltInType.Double:
                    {
                        return (BitConverter.GetBytes(doubleValue));
                    }
                default:
                    valueConverted = false;
                    return (null);
            }
        }

        public static byte[] ConvertFloatValueWithLockStatusToByteArray(BuiltInType destinationValueType, float floatValue, byte lockStatus, ref bool valueConverted)
        {
            valueConverted = true;
            byte[] valueArray = null;
            byte[] valueLockArray = null;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if (floatValue != 0.0F)
                        {
                            boolValue = true;
                        }
                        valueArray = BitConverter.GetBytes(boolValue);
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)floatValue;
                        valueArray = BitConverter.GetBytes(sbyteValue);
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)floatValue;
                        valueArray = BitConverter.GetBytes(byteValue);
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)floatValue;
                        valueArray = BitConverter.GetBytes(int16Value);
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = (UInt16)floatValue;
                        valueArray = BitConverter.GetBytes(uint16Value);
                    }
                    break;
                case BuiltInType.Float:
                    {
                        valueArray = BitConverter.GetBytes(floatValue);
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = (UInt32)floatValue;
                        valueArray = BitConverter.GetBytes(uint32Value);
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)floatValue;
                        valueArray = BitConverter.GetBytes(int32Value);
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)floatValue;
                        valueArray = BitConverter.GetBytes(uint64Value);
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)floatValue;
                        valueArray = BitConverter.GetBytes(int64Value);
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double doubleValue = (double)floatValue;
                        valueArray = BitConverter.GetBytes(doubleValue);
                    }
                    break;
                default:
                    valueConverted = false;
                    break;
            }

            if (valueArray != null)
            {
                valueLockArray = AddLockStatusToByteArray(valueArray, lockStatus);
            }

            return (valueLockArray);
        }

        public static byte[] ConvertFloatValueToByteArray(BuiltInType destinationValueType, float floatValue, ref bool valueConverted)
        {
            valueConverted = true;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if (floatValue != 0.0F)
                        {
                            boolValue = true;
                        }
                        return (BitConverter.GetBytes(boolValue));
                    }
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)floatValue;
                        return (BitConverter.GetBytes(sbyteValue));
                    }
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)floatValue;
                        return (BitConverter.GetBytes(byteValue));
                    }
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)floatValue;
                        return (BitConverter.GetBytes(int16Value));
                    }
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = (UInt16)floatValue;
                        return (BitConverter.GetBytes(uint16Value));
                    }
                case BuiltInType.Float:
                    {
                        return (BitConverter.GetBytes(floatValue));
                    }
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = (UInt32)floatValue;
                        return (BitConverter.GetBytes(uint32Value));
                    }
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)floatValue;
                        return (BitConverter.GetBytes(int32Value));
                    }
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)floatValue;
                        return (BitConverter.GetBytes(uint64Value));
                    }
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)floatValue;
                        return (BitConverter.GetBytes(int64Value));
                    }
                case BuiltInType.Double:
                    {
                        double doubleValue = (double)floatValue;
                        return (BitConverter.GetBytes(doubleValue));
                    }
                default:
                    valueConverted = false;
                    return (null);
            }
        }

        public static byte[] ConvertUint32ValueToByteArray(BuiltInType destinationValueType, UInt32 uint32Value, ref bool valueConverted)
        {
            valueConverted = true;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if (uint32Value != 0)
                        {
                            boolValue = true;
                        }
                        return (BitConverter.GetBytes(boolValue));
                    }
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)uint32Value;
                        return (BitConverter.GetBytes(sbyteValue));
                    }
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)uint32Value;
                        return (BitConverter.GetBytes(byteValue));
                    }
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)uint32Value;
                        return (BitConverter.GetBytes(int16Value));
                    }
                case BuiltInType.UInt16:
                    {
                        UInt16 uint16Value = (UInt16)uint32Value;
                        return (BitConverter.GetBytes(uint16Value));
                    }
                case BuiltInType.Float:
                    {
                        float floatValue = (float)uint32Value;
                        return (BitConverter.GetBytes(floatValue));
                    }
                case BuiltInType.UInt32:
                    {
                        return (BitConverter.GetBytes(uint32Value));
                    }
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)uint32Value;
                        return (BitConverter.GetBytes(int32Value));
                    }
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)uint32Value;
                        return (BitConverter.GetBytes(uint64Value));
                    }
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)uint32Value;
                        return (BitConverter.GetBytes(int64Value));
                    }
                case BuiltInType.Double:
                    {
                        double doubleValue = (double)uint32Value;
                        return (BitConverter.GetBytes(doubleValue));
                    }
                default:
                    valueConverted = false;
                    return (null);
            }
        }

        public static byte[] ConvertUint16ValueToByteArray(BuiltInType destinationValueType, UInt16 uint16Value, ref bool valueConverted)
        {
            valueConverted = true;
            switch (destinationValueType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = false;
                        if (uint16Value != 0)
                        {
                            boolValue = true;
                        }
                        return (BitConverter.GetBytes(boolValue));
                    }
                case BuiltInType.SByte:
                    {
                        SByte sbyteValue = (SByte)uint16Value;
                        return (BitConverter.GetBytes(sbyteValue));
                    }
                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)uint16Value;
                        return (BitConverter.GetBytes(byteValue));
                    }
                case BuiltInType.Int16:
                    {
                        Int16 int16Value = (Int16)uint16Value;
                        return (BitConverter.GetBytes(int16Value));
                    }
                case BuiltInType.UInt16:
                    {
                        return (BitConverter.GetBytes(uint16Value));
                    }
                case BuiltInType.Float:
                    {
                        float floatValue = (float)uint16Value;
                        return (BitConverter.GetBytes(floatValue));
                    }
                case BuiltInType.UInt32:
                    {
                        UInt32 uint32Value = (UInt32)uint16Value;
                        return (BitConverter.GetBytes(uint32Value));
                    }
                case BuiltInType.Int32:
                    {
                        Int32 int32Value = (Int32)uint16Value;
                        return (BitConverter.GetBytes(int32Value));
                    }
                case BuiltInType.UInt64:
                    {
                        UInt64 uint64Value = (UInt64)uint16Value;
                        return (BitConverter.GetBytes(uint64Value));
                    }
                case BuiltInType.Int64:
                    {
                        Int64 int64Value = (Int64)uint16Value;
                        return (BitConverter.GetBytes(int64Value));
                    }
                case BuiltInType.Double:
                    {
                        double doubleValue = (double)uint16Value;
                        return (BitConverter.GetBytes(doubleValue));
                    }
                default:
                    valueConverted = false;
                    return (null);
            }
        }

        public static bool TestBit(byte byteToBeTested, int bitIndex)
        {
            bool returnValue = false;

            switch (bitIndex)
            {
                case 0:
                    if((byteToBeTested & 0x01) > 0)
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

        public static void SetBit(ref uint intValue, int bitIndex)
        {
            switch (bitIndex)
            {
                case 0:
                    intValue |= 0x01;
                    break;
                case 1:
                    intValue |= 0x02;
                    break;
                case 2:
                    intValue |= 0x04;
                    break;
                case 3:
                    intValue |= 0x08;
                    break;
                case 4:
                    intValue |= 0x10;
                    break;
                case 5:
                    intValue |= 0x20;
                    break;
                case 6:
                    intValue |= 0x40;
                    break;
                case 7:
                    intValue |= 0x80;
                    break;
                case 8:
                    intValue |= 0x100;
                    break;
                case 9:
                    intValue |= 0x200;
                    break;
                case 10:
                    intValue |= 0x400;
                    break;
                case 11:
                    intValue |= 0x800;
                    break;
                case 12:
                    intValue |= 0x1000;
                    break;
                case 13:
                    intValue |= 0x2000;
                    break;
                case 14:
                    intValue |= 0x4000;
                    break;
                case 15:
                    intValue |= 0x8000;
                    break;
                case 16:
                    intValue |= 0x10000;
                    break;
                case 17:
                    intValue |= 0x20000;
                    break;
                case 18:
                    intValue |= 0x40000;
                    break;
                case 19:
                    intValue |= 0x80000;
                    break;
                case 20:
                    intValue |= 0x100000;
                    break;
                case 21:
                    intValue |= 0x200000;
                    break;
                case 22:
                    intValue |= 0x400000;
                    break;
                case 23:
                    intValue |= 0x800000;
                    break;
                case 24:
                    intValue |= 0x1000000;
                    break;
                case 25:
                    intValue |= 0x2000000;
                    break;
                case 26:
                    intValue |= 0x4000000;
                    break;
                case 27:
                    intValue |= 0x8000000;
                    break;
                case 28:
                    intValue |= 0x10000000;
                    break;
                case 29:
                    intValue |= 0x20000000;
                    break;
                case 30:
                    intValue |= 0x40000000;
                    break;
                case 31:
                    intValue |= 0x80000000;
                    break;
            }
        }

        public static void SetBit(ref byte[] buffer, int bufferLength, int bitIndex)
        {
            if((bitIndex < 0) || (bitIndex >= bufferLength*8))
            {
                return;
            }
            int byteIndex = bitIndex / 8;
            int bitOfTheByte = bitIndex % 8;
            switch(bitOfTheByte)
            {
                case 0:
                    buffer[byteIndex] |= 0x01;
                    break;
                case 1:
                    buffer[byteIndex] |= 0x02;
                    break;
                case 2:
                    buffer[byteIndex] |= 0x04;
                    break;
                case 3:
                    buffer[byteIndex] |= 0x08;
                    break;
                case 4:
                    buffer[byteIndex] |= 0x10;
                    break;
                case 5:
                    buffer[byteIndex] |= 0x20;
                    break;
                case 6:
                    buffer[byteIndex] |= 0x40;
                    break;
                case 7:
                    buffer[byteIndex] |= 0x80;
                    break;
            }
        }

        public static DateTime ParseSMSTimeStamp(byte[] receivedMessage, int alreadyParsedBytes)
        {
            int i = 0;
            int j = 0;
            // Day (5 bits)
            uint day = 0;
            for(i=7, j=4; i>2; i--, j--)
            {
                if(TestBit(receivedMessage[alreadyParsedBytes], i))
                {
                    SetBit(ref day, j);
                }
            }
            // Month (4 bits)
            uint month = 0;
            for (i = 2, j = 3; i >= 0; i--, j--)
            {
                if (TestBit(receivedMessage[alreadyParsedBytes], i))
                {
                    SetBit(ref month, j);
                }
            }
            if (TestBit(receivedMessage[alreadyParsedBytes+1], 7))
            {
                SetBit(ref month, 0);
            }
            // Year (7 bits)
            uint year = (uint)receivedMessage[alreadyParsedBytes + 1] & 0X7f;
            year += 2000;
            // Hour (5 bits)
            uint hour = 0;
            for (i = 7, j = 4; i > 2; i--, j--)
            {
                if (TestBit(receivedMessage[alreadyParsedBytes+2], i))
                {
                    SetBit(ref hour, j);
                }
            }
            // Minute (6 bits)
            uint minute = 0;
            for (i = 2, j = 5; i >= 0; i--, j--)
            {
                if (TestBit(receivedMessage[alreadyParsedBytes+2], i))
                {
                    SetBit(ref minute, j);
                }
            }
            for (i = 7, j = 2; i > 4; i--, j--)
            {
                if (TestBit(receivedMessage[alreadyParsedBytes + 3], i))
                {
                    SetBit(ref minute, j);
                }
            }

            int second = 0;
            DateTime parsedDateTime = new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, second);
            System.Diagnostics.Debug.WriteLine("ParseSMSTimeStamp: Timestamp = {0}-{1}-{2} {3}:{4}:{5}",
                                               day, month, year, hour, minute, second);
            return (parsedDateTime);
        }

        public static DateTime ParseSMSDate(byte[] receivedMessage, ref int initialByte, ref int numOfBitsInTheInitialByte)
        {
            // Day (5 bits)
            uint day = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 5);
            // Month (4 bits)
            uint month = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 4);
            // Year (7 bits)
            uint year = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 7);
            year += 2000;
            // Hour = 0
            uint hour = 0;
            // Minute = 0
            uint minute = 0;
            // Second = 0
            int second = 0;
            DateTime parsedDateTime = new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, second);
            System.Diagnostics.Debug.WriteLine("ParseSMSDate: Timestamp = {0}-{1}-{2} {3}:{4}:{5}",
                                               day, month, year, hour, minute, second);
            return (parsedDateTime);
        }

        public static DateTime ParseSMSTimeStamp(byte[] receivedMessage, ref int initialByte, ref int numOfBitsInTheInitialByte)
        {
            // Day (5 bits)
            uint day = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 5);
            // Month (4 bits)
            uint month = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 4);
            // Year (7 bits)
            uint year = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 7);
            year += 2000;
            // Hour (5 bits)
            uint hour = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 5);
            // Minute (6 bits)
            uint minute = LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);

            int second = 0;
            DateTime parsedDateTime = new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, second);
            System.Diagnostics.Debug.WriteLine("ParseSMSTimeStamp: Timestamp = {0}-{1}-{2} {3}:{4}:{5}",
                                               day, month, year, hour, minute, second);
            return (parsedDateTime);
        }
    }
}
