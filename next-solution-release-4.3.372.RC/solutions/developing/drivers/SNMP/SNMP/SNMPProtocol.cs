using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Runtime.InteropServices;
using DriverCodeBase.Enumerators;

namespace SNMP
{
    #region enums

    //public enum DataAreas : sbyte
    //{
    //    Invalid = -1,
    //    IR = 0,
    //    LR,
    //    HR,
    //    AR,
    //    DM,
    //    T,
    //    C,
    //    EM,
    //    W

    //}

    //public enum DataFormats : int
    //{
    //    Invalid = -1,
    //    Bit,
    //    Word
    //};

    //public enum DataConversionTypes : int
    //{
    //    None = 0,
    //    BCD16Bits,
    //    BCD32Bits
    //};

    //public enum CommandCodes : byte
    //{
    //    Read,
    //    Write,
    //};

    public enum SNMPErrorCodes : int
    {
        SNMPErrorMalformedReply = 1000,
        SNMPErrorUnsupportedVersion,
        SNMPErrorWrongCommunity,
        SNMPErrorWrongReplyType,
        SNMPErrorWrongReplyID,
        SNMPErrorOidMismatch,
        SNMPErrorUnsupportedDataType,
        SNMPErrorDataTypeMismatch,
        SNMPErrorNoSuchObject,
        SNMPErrorInvalidDataFormat,
        SNMPErrorWriteFail,
        SNMPErrorNonZeroErrorStatus = 1500,
        
        //ErrorDriverError = 2000,
        //ErrorSid,
        //ErrorEndCode,
        //ErrorIncompleteFrame,
    }


    #endregion

    #region dataTypes

    //[StructLayout(LayoutKind.Explicit)]
    //public struct ushortUnion
    //{
    //    [FieldOffset(0)]
    //    public ushort USHORT;

    //    [FieldOffset(0)]
    //    public byte LOBYTE;
    //    [FieldOffset(1)]
    //    public byte HIBYTE;
    //    // Constructor:
    //    public ushortUnion(ushort USHORT)
    //    {
    //        this.LOBYTE = 0;
    //        this.HIBYTE = 0;
    //        this.USHORT = USHORT;
    //    }
    //    public ushortUnion(byte LOBYTE, byte HIBYTE)
    //    {
    //        this.USHORT = 0;
    //        this.LOBYTE = LOBYTE;
    //        this.HIBYTE = HIBYTE;
    //    }
    //    public ushortUnion(byte[] buffer, ushort index)
    //    {
    //        this.USHORT = 0;
    //        this.LOBYTE = buffer[index ];
    //        this.HIBYTE = buffer[index + 1];
    //    }
    //    public ushortUnion(List<byte> buffer, ushort index)
    //    {
    //        this.USHORT = 0;
    //        this.LOBYTE = buffer[index ];
    //        this.HIBYTE = buffer[index + 1];
    //    }

    //}

    //[StructLayout(LayoutKind.Explicit)]
    //public struct shortUnion
    //{
    //    [FieldOffset(0)]
    //    public short SHORT;

    //    [FieldOffset(0)]
    //    public byte LOBYTE;
    //    [FieldOffset(1)]
    //    public byte HIBYTE;
    //    // Constructor:
    //    public shortUnion(short SHORT)
    //    {
    //        this.LOBYTE = 0;
    //        this.HIBYTE = 0;
    //        this.SHORT = SHORT;
    //    }
    //    public shortUnion(byte LOBYTE, byte HIBYTE)
    //    {
    //        this.SHORT = 0;
    //        this.LOBYTE = LOBYTE;
    //        this.HIBYTE = HIBYTE;
    //    }
    //    public shortUnion(byte[] buffer, ushort index)
    //    {
    //        this.SHORT = 0;
    //        this.LOBYTE = buffer[index];
    //        this.HIBYTE = buffer[index + 1];
    //    }
    //    public shortUnion(List<byte> buffer, ushort index)
    //    {
    //        this.SHORT = 0;
    //        this.LOBYTE = buffer[index];
    //        this.HIBYTE = buffer[index + 1];
    //    }

    //}

    //[StructLayout(LayoutKind.Explicit)]
    //public struct uintUnion
    //{
    //    [FieldOffset(0)]
    //    public uint UINT;

    //    [FieldOffset(0)]
    //    public ushortUnion LOUSHORT;
    //    [FieldOffset(2)]
    //    public ushortUnion HIUSHORT;

    //    // Constructor:
    //    public uintUnion(uint UINT)
    //    {
    //        this.LOUSHORT = new ushortUnion(0x0000);
    //        this.HIUSHORT = new ushortUnion(0x0000);
    //        this.UINT = UINT;
    //    }
    //    public uintUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
    //    {
    //        this.UINT = 0;
    //        this.LOUSHORT = new ushortUnion(LOBYTE_LW, HIBYTE_LW);
    //        this.HIUSHORT = new ushortUnion(LOBYTE_HW, HIBYTE_HW);
    //    }
    //    public uintUnion(byte[] buffer, ushort index)
    //    {
    //        this.UINT = 0;
    //        this.LOUSHORT = new ushortUnion(buffer, index);
    //        this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
    //    }
    //    public uintUnion(List<byte> buffer, ushort index)
    //    {
    //        this.UINT = 0;
    //        this.LOUSHORT = new ushortUnion(buffer, index);
    //        this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
    //    }

    //}

    //[StructLayout(LayoutKind.Explicit)]
    //public struct intUnion
    //{
    //    [FieldOffset(0)]
    //    public int INT;

    //    [FieldOffset(0)]
    //    public shortUnion LOSHORT;
    //    [FieldOffset(2)]
    //    public shortUnion HISHORT;

    //    // Constructor:
    //    public intUnion(int INT)
    //    {
    //        this.LOSHORT = new shortUnion(0x0000);
    //        this.HISHORT = new shortUnion(0x0000);
    //        this.INT = INT;
    //    }
    //    public intUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
    //    {
    //        this.INT = 0;
    //        this.LOSHORT = new shortUnion(LOBYTE_LW, HIBYTE_LW);
    //        this.HISHORT = new shortUnion(LOBYTE_HW, HIBYTE_HW);
    //    }
    //    public intUnion(byte[] buffer, ushort index)
    //    {
    //        this.INT = 0;
    //        this.LOSHORT = new shortUnion(buffer, index);
    //        this.HISHORT = new shortUnion(buffer, (ushort)(index + 2));
    //    }
    //    public intUnion(List<byte> buffer, ushort index)
    //    {
    //        this.INT = 0;
    //        this.LOSHORT = new shortUnion(buffer, index);
    //        this.HISHORT = new shortUnion(buffer, (ushort)(index + 2));
    //    }

    //}



    #endregion

    public class SNMPProtocol
    {
        #region constants
        //public const uint BUFFER_SIZE = 1024;
        //public const uint MAX_DATA_SIZE = BUFFER_SIZE - 26;
        //public const uint MAX_DATA_SIZE_INPUT = BUFFER_SIZE - 22;
        //public const int PROTOCOL_ERROR = 1500;
        //public const byte ENCAPSULATION_HEADER_SIZE = 14;
        //public const byte SID = 9;
        //public const byte MRES = 12;
        //public const byte SRES = 13;
        #endregion

        #region methods
        public static uint ASN1GetCodifiedOctetStringLength(string stringToBeCodified)
        {
            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            if (!String.IsNullOrWhiteSpace(stringToBeCodified))
            {
                // Type of the object (1 byte)
                octetStringLength++;

                // Length of the string (variable numbers of byte)
                int stringLength = stringToBeCodified.Length;
                octetStringLength += ASN1GetCodifiedLengthLength(stringLength);

                // Octets of the string (length of the string)
                octetStringLength += (uint)stringLength;
            }

            return (octetStringLength);
        }

        public static uint ASN1StringToOctetString(string stringToBeCodified, ref byte[] octetString)
        {
            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            if (!String.IsNullOrWhiteSpace(stringToBeCodified))
            {
                // Type of the object: 0x04 = Octet string
                octetString[octetStringLength++] = 0x04;

                // Length of the string
                int stringLength = stringToBeCodified.Length;
                byte[] intValueBuffer = new byte[4];
                uint codifiedLength = ASN1CodifyLength(stringLength, ref intValueBuffer);
                int i = 0;
                for (i = 0; i < (int)codifiedLength; i++)
                {
                    octetString[octetStringLength++] = intValueBuffer[i];
                }

                // Octets of the string
                for (i=0; i<stringLength; i++)
                {
                    octetString[octetStringLength++] = (byte)stringToBeCodified[i];
                }
            }

            return (octetStringLength);
        }

        public static uint ASN1GetCodifiedLengthLength(int intValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            uint uintTempValue = (uint)intValue;
            byte[] byteBuffer = new byte[4];
            byteBuffer[3] = (byte)(uintTempValue >> 24);
            byteBuffer[2] = (byte)(uintTempValue >> 16);
            byteBuffer[1] = (byte)(uintTempValue >> 8);
            byteBuffer[0] = (byte)uintTempValue;
            int bufferIndex = 3;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (byteBuffer[bufferIndex])
                {
                    case 0:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            for (codifiedValueLength = 0; bufferIndex >= 0; bufferIndex--)
            {
                codifiedValueLength++;
            }

            return (codifiedValueLength);
        }

        public static uint ASN1CodifyLength(int intValue, ref byte[] codifiedValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            uint uintTempValue = (uint)intValue;
            byte[] byteBuffer = new byte[4];
            byteBuffer[3] = (byte)(uintTempValue >> 24);
            byteBuffer[2] = (byte)(uintTempValue >> 16);
            byteBuffer[1] = (byte)(uintTempValue >> 8);
            byteBuffer[0] = (byte)uintTempValue;
            int bufferIndex = 3;
            for(bufferIndex=3; bufferIndex>0; bufferIndex--)
            {
                switch(byteBuffer[bufferIndex])
                {
                    case 0:
                        if((byteBuffer[bufferIndex-1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            // Copy the integer octets
            for(codifiedValueLength = 0; bufferIndex>=0; bufferIndex--, codifiedValueLength++)
            {
                codifiedValue[codifiedValueLength] = byteBuffer[bufferIndex];
            }
 
            return (codifiedValueLength);
        }

        public static uint ASN1GetCodifiedIntLength(int intValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            uint uintTempValue = (uint)intValue;
            byte[] byteBuffer = new byte[4];
            byteBuffer[3] = (byte)(uintTempValue >> 24);
            byteBuffer[2] = (byte)(uintTempValue >> 16);
            byteBuffer[1] = (byte)(uintTempValue >> 8);
            byteBuffer[0] = (byte)uintTempValue;
            int bufferIndex = 3;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (byteBuffer[bufferIndex])
                {
                    case 0:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            for (codifiedValueLength = 0; bufferIndex >= 0; bufferIndex--)
            {
                codifiedValueLength++;
            }

            codifiedValueLength += 2; // Type (1 byte) + Number of octets (1 byte)

            return (codifiedValueLength);
        }

        public static uint ASN1CodifyInt(int intValue, ref byte[] codifiedValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            uint uintTempValue = (uint)intValue;
            byte[] byteBuffer = new byte[4];
            byteBuffer[3] = (byte)(uintTempValue >> 24);
            byteBuffer[2] = (byte)(uintTempValue >> 16);
            byteBuffer[1] = (byte)(uintTempValue >> 8);
            byteBuffer[0] = (byte)uintTempValue;
            int bufferIndex = 3;
            for (bufferIndex = 3; bufferIndex > 0; bufferIndex--)
            {
                switch (byteBuffer[bufferIndex])
                {
                    case 0:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & 0x80) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            // Type (integer)
            codifiedValue[codifiedValueLength++] = 0x02;
            // Codified value length
            byte valueLength = (byte)(bufferIndex + 1);
            codifiedValue[codifiedValueLength++] = valueLength;
            // Codified value
            for (; bufferIndex >= 0; bufferIndex--)
            {
                codifiedValue[codifiedValueLength++] = byteBuffer[bufferIndex];
            }

            return (codifiedValueLength);
        }

        //public static uint ASN1CodifyInt(int intValue, SNMPDATATYPE intType, ref byte[] codifiedValue)
        //{
        //    // Check the integer type
        //    byte codifiedIntType = 0x02;
        //    switch(intType)
        //    {
        //        case SNMPDATATYPE.Integer:
        //        case SNMPDATATYPE.Integer32:
        //            codifiedIntType = 0x02;
        //            break;

        //        case SNMPDATATYPE.Counter32:
        //            codifiedIntType = 0x41;
        //            break;

        //        case SNMPDATATYPE.Gauge32:
        //            codifiedIntType = 0x42;
        //            break;

        //        case SNMPDATATYPE.TimeTicks:
        //            codifiedIntType = 0x43;
        //            break;

        //        case SNMPDATATYPE.Unsigned32:
        //            codifiedIntType = 0x47;
        //            break;

        //        default:
        //            return (0);
        //    }

        //    // The returned value: length of the codified value
        //    uint codifiedValueLength = 0;

        //    int integerValue = intValue;
        //    int integerSize = 4;
        //    uint mask = 0x1FF00000;
        //    while((((integerValue & mask) == 0) || ((integerValue & mask) == mask)) && (integerSize > 1))
        //    {
        //        integerSize--;
        //        integerValue <<= 8;
        //    }

        //    // Type
        //    codifiedValue[codifiedValueLength++] = codifiedIntType;

        //    // Size
        //    codifiedValue[codifiedValueLength++] = (byte)integerSize;

        //    // Codified Value
        //    while((integerSize--) > 0)
        //    {
        //        codifiedValue[codifiedValueLength++] = (byte)((integerValue & mask) >> 24);
        //        integerValue <<= 8;                
        //    }

        //    return (codifiedValueLength);
        //}

        public static uint ASN1GetCodifiedOIDLength(uint[] oidIntValuesArray, int oidIntValuesArrayLength)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;
            if(oidIntValuesArrayLength >= 2)
            {
                int nodeIndex = 2;
                uint oidLength = 1; // Integer1*40 + Integer2 (1 byte)
                while (nodeIndex < oidIntValuesArrayLength)
                {
                    uint requiredBytes = 0;
                    uint val1 = oidIntValuesArray[nodeIndex++];
                    if (val1 > 0)
                    {
                        uint val2 = val1;
                        while (val2 > 0)
                        {
                            requiredBytes++;
                            val2 >>= 7;
                        }
                    }
                    else
                    {
                        requiredBytes++;
                    }
                    oidLength += requiredBytes;
                }

                codifiedValueLength = 1 + ASN1GetCodifiedLengthLength((int)oidLength) + oidLength;
            }

            return (codifiedValueLength);
        }

        public static uint ASN1CodifyOID(uint[] oidIntValuesArray, int oidIntValuesArrayLength, ref byte[] codifiedValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            // Minimum length of the OID = 2
            if (oidIntValuesArrayLength >= 2)
            {
                
                // Allocate a local buffer to store the cofified OID
                byte[] tempOIDBuffer = new byte[codifiedValue.Length];

                // First encoded byte = 40*<1st OID element> + <2nd OID element>
                tempOIDBuffer[0] = (byte)(oidIntValuesArray[0] * 40 + oidIntValuesArray[1]);

                uint nodeIndex = 2;
                uint oidLength = 1; // Integer1*40 + Integer2 (1 byte)
                while (nodeIndex < oidIntValuesArrayLength)
                {
                    int requiredBytes = 0;
                    uint val1 = oidIntValuesArray[nodeIndex++];
                    uint val2 = val1;
                    if (val1 > 0)
                    {
                        while (val2 > 0)
                        {
                            requiredBytes++;
                            val2 >>= 7;
                        }
                    }
                    else
                    {
                        requiredBytes++;
                    }

                    while (requiredBytes > 0)
                    {
                        val2 = val1 >> (7 * (requiredBytes - 1));
                        val2 &= 0x7f;
                        if (requiredBytes > 1)
                        {
                            val2 += 128;
                        }
                        tempOIDBuffer[oidLength++] = (byte)val2;
                        requiredBytes--;
                    }
                }

                // Calculate the size of the codified length of the OID
                uint oidLengthCodifiedSize = ASN1GetCodifiedLengthLength((int)oidLength);
                if(oidLengthCodifiedSize >= 1)
                {
                    // Allocate a temporary buffer for the codified length of the OID
                    byte[] codifiedOidLength = new byte[oidLengthCodifiedSize];

                    // Codify the length of the OID
                    ASN1CodifyLength((int)oidLength, ref codifiedOidLength);

                    // Fill the output buffer
                    // Type
                    codifiedValue[codifiedValueLength++] = 0x06;
                    // Codified Length
                    uint i = 0;
                    for(i=0; i< oidLengthCodifiedSize; i++)
                    {
                        codifiedValue[codifiedValueLength++] = codifiedOidLength[i];
                    }
                    // Codified OID                   
                    for (i = 0; i < oidLength; i++)
                    {
                        codifiedValue[codifiedValueLength++] = tempOIDBuffer[i];
                    }
                }
            }

            return (codifiedValueLength);
        }

        public static int ASN1DecodifyLength(List<byte> buffer, int bufferLength, int lengthIndex, ref int lengthSize)
        {
            lengthSize = 0;

            // Returned value
            int lengthValue = 0;

            if(bufferLength > lengthIndex)
            {
                // Short form
                if (buffer[lengthIndex] < 0x80)
                {
                    lengthValue = (int)buffer[lengthIndex];
                    lengthSize = 1;
                }
                // Long form with 2 bytes
                else if (buffer[lengthIndex] == 0x82)
                {
                    if(bufferLength > lengthIndex + 2)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthSize = 3;
                    }
                }
                // Long form with 3 bytes
                else if (buffer[lengthIndex] == 0x83)
                {
                    if (bufferLength > lengthIndex + 3)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 3];
                        lengthSize = 4;
                    }
                }
                // Long form with 4 bytes
                else if (buffer[lengthIndex] == 0x84)
                {
                    if (bufferLength > lengthIndex + 4)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 3];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 4];
                        lengthSize = 4;
                    }
                }
            }

            return (lengthValue);
        }

        public static int ASN1DecodifyLength(byte[] buffer, int bufferLength, int lengthIndex, ref int lengthSize)
        {
            lengthSize = 0;

            // Returned value
            int lengthValue = 0;

            if (bufferLength > lengthIndex)
            {
                // Short form
                if (buffer[lengthIndex] < 0x80)
                {
                    lengthValue = (int)buffer[lengthIndex];
                    lengthSize = 1;
                }
                // Long form with 2 bytes
                else if (buffer[lengthIndex] == 0x82)
                {
                    if (bufferLength > lengthIndex + 2)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthSize = 3;
                    }
                }
                // Long form with 3 bytes
                else if (buffer[lengthIndex] == 0x83)
                {
                    if (bufferLength > lengthIndex + 3)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 3];
                        lengthSize = 4;
                    }
                }
                // Long form with 4 bytes
                else if (buffer[lengthIndex] == 0x84)
                {
                    if (bufferLength > lengthIndex + 4)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 2];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 3];
                        lengthValue <<= 8;
                        lengthValue += (int)buffer[lengthIndex + 4];
                        lengthSize = 4;
                    }
                }
            }

            return (lengthValue);
        }

        public static int ASN1DecodifyInteger(List<byte> buffer, int bufferLength, int integerIndex, ref int integerSize)
        {
            integerSize = 0;

            // Returned value
            int integerValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object == 0x02 == Integer?
                if (buffer[integerIndex] == 0x02)
                {
                    integerSize++;
                    if (bufferLength > (integerIndex + integerSize))
                    {
                        // Check the size of the codified value
                        int numberOfOctets = buffer[integerIndex + integerSize];
                        if (bufferLength > (integerIndex + integerSize + numberOfOctets))
                        {
                            // Parse the integer value
                            integerSize++;
                            bool negativeValue = false;
                            for (int i = 0; i < numberOfOctets; i++)
                            {
                                integerValue <<= 8;
                                if (i == 0)
                                {
                                    if ((buffer[integerIndex + integerSize + i] & 0x80) != 0)
                                    {
                                        negativeValue = true;
                                    }
                                }
                                integerValue += buffer[integerIndex + integerSize + i];
                            }
                            if (negativeValue == true)
                            {
                                integerValue *= -1;
                            }
                            integerSize += numberOfOctets;
                        }
                    }
                }
            }

            return (integerValue);
        }

        public static int ASN1DecodifyIntegerValue(byte[] buffer, int bufferLength, int integerIndex, ref int integerSize)
        {
            integerSize = 0;

            // Returned value
            int integerValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object
                if ((buffer[integerIndex] == 0x02) || (buffer[integerIndex] == 0x41) || (buffer[integerIndex] == 0x43))
                {
                    integerSize++;
                    if (bufferLength > (integerIndex + integerSize))
                    {
                        // Check the size of the codified value
                        int numberOfOctets = buffer[integerIndex + integerSize];
                        if ((numberOfOctets > 0) && (numberOfOctets <= 4) && (bufferLength > (integerIndex + integerSize + numberOfOctets)))
                        {
                            // Parse the integer value
                            integerSize++;
                            // Negative value?
                            if((buffer[integerIndex + integerSize] & 0x80) != 0)
                            {
                                integerValue = -1;
                            }
                            while((numberOfOctets--) > 0)
                            {
                                integerValue = (integerValue <<= 8) | buffer[integerIndex + integerSize];
                                integerSize++;
                            }
                        }
                    }
                }
            }

            return (integerValue);
        }

        public static uint ASN1DecodifyUnsignedValue(byte[] buffer, int bufferLength, int integerIndex, ref int integerSize)
        {
            integerSize = 0;

            // Returned value
            uint unsignedValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object
                if ((buffer[integerIndex] == 0x02) ||
                    (buffer[integerIndex] == 0x41) ||
                    (buffer[integerIndex] == 0x42) ||
                    (buffer[integerIndex] == 0x43) ||
                    (buffer[integerIndex] == 0x47))
                {
                    integerSize++;
                    if (bufferLength > (integerIndex + integerSize))
                    {
                        // Check the size of the codified value
                        int numberOfOctets = buffer[integerIndex + integerSize];
                        if ((numberOfOctets > 0) && (bufferLength > (integerIndex + integerSize + numberOfOctets)))
                        {
                            integerSize++;
                            int maxNumOfOctets = 5;
                            if (buffer[integerIndex + integerSize] != 0)
                            {
                                maxNumOfOctets = 4;
                            }
                            if (numberOfOctets <= maxNumOfOctets)
                            {
                                // Skip the first byte if it is 0
                                if (buffer[integerIndex + integerSize] == 0)
                                {
                                    integerSize++;
                                    numberOfOctets--;
                                }
                                // Parse the unsigned value
                                for (int i=0; i<numberOfOctets; i++)
                                {
                                    unsignedValue = (unsignedValue << 8) + buffer[integerIndex + integerSize];
                                    integerSize++;
                                }
                            }
                        }
                    }
                }
            }

            return (unsignedValue);
        }

        public static uint ASN1DecodifyUnsignedValue(List<byte> buffer, int bufferLength, int integerIndex, ref int integerSize)
        {
            integerSize = 0;

            // Returned value
            uint unsignedValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object
                if ((buffer[integerIndex] == 0x02) ||
                    (buffer[integerIndex] == 0x41) ||
                    (buffer[integerIndex] == 0x42) ||
                    (buffer[integerIndex] == 0x43) ||
                    (buffer[integerIndex] == 0x47))
                {
                    integerSize++;
                    if (bufferLength > (integerIndex + integerSize))
                    {
                        // Check the size of the codified value
                        int numberOfOctets = buffer[integerIndex + integerSize];
                        if ((numberOfOctets > 0) && (bufferLength > (integerIndex + integerSize + numberOfOctets)))
                        {
                            integerSize++;
                            int maxNumOfOctets = 5;
                            if (buffer[integerIndex + integerSize] != 0)
                            {
                                maxNumOfOctets = 4;
                            }
                            if (numberOfOctets <= maxNumOfOctets)
                            {
                                // Skip the first byte if it is 0
                                if (buffer[integerIndex + integerSize] == 0)
                                {
                                    integerSize++;
                                    numberOfOctets--;
                                }
                                // Parse the unsigned value
                                for (int i = 0; i < numberOfOctets; i++)
                                {
                                    unsignedValue = (unsignedValue << 8) + buffer[integerIndex + integerSize];
                                    integerSize++;
                                }
                            }
                        }
                    }
                }
            }

            return (unsignedValue);
        }

        public static int ASN1DecodifyOctetString(byte[] buffer, int bufferLength, int octetStringIndex, ref byte[] octetString)
        {
            // Returned value
            int stringSize = 0;

            if (bufferLength > octetStringIndex)
            {
                // The type of the object must be Octet string (0x04) or IP address (0x40)
                byte objectType = buffer[octetStringIndex];
                if ((objectType == 0x04) || (objectType == 0x40))
                {
                    // Get the length of the string 
                    int lengthSize = 0;
                    int objectSize = ASN1DecodifyLength(buffer, bufferLength, octetStringIndex + 1, ref lengthSize);
                    if((lengthSize > 0) && (objectSize <= octetString.Count()) &&
                       (bufferLength >= (octetStringIndex + 1 + lengthSize + objectSize)))
                    {
                        for(int i=0; i<objectSize; i++)
                        {
                            octetString[i] = buffer[octetStringIndex + 1 + lengthSize + i];
                        }
                        stringSize = objectSize;
                    }
                }
            }

            return (stringSize);
        }

        public static int ASN1DecodifyIpAddress(byte[] buffer, int bufferLength, int octetStringIndex, ref byte[] octetString)
        {
            // Returned value
            int stringSize = 0;

            if (bufferLength > octetStringIndex)
            {
                // The type of the object must be IP address (0x40)
                byte objectType = buffer[octetStringIndex];
                if (objectType == 0x40)
                {
                    // Get the length of the string 
                    int lengthSize = 0;
                    int objectSize = ASN1DecodifyLength(buffer, bufferLength, octetStringIndex + 1, ref lengthSize);
                    if ((lengthSize > 0) && (objectSize <= octetString.Count()) && (objectSize == 4) &&
                       (bufferLength >= (octetStringIndex + 1 + lengthSize + objectSize)))
                    {
                        // Convert the octets of the IP Address in a string
                        string ipAddressString = String.Format("{0}.{1}.{2}.{3}",
                                                               buffer[octetStringIndex + 1 + lengthSize],
                                                               buffer[octetStringIndex + 2 + lengthSize],
                                                               buffer[octetStringIndex + 3 + lengthSize],
                                                               buffer[octetStringIndex + 4 + lengthSize]);

                        // Get the characters of the string as bytes
                        Encoding ascii = Encoding.ASCII;
                        byte[] ipAddressBytes = ascii.GetBytes(ipAddressString);

                        // Copy the bytes of the IP address
                        int maxNumOfBytes = octetString.Count();
                        if(ipAddressBytes.Count() < octetString.Count())
                        {
                            maxNumOfBytes = ipAddressBytes.Count();
                        }
                        for (int i = 0; i < maxNumOfBytes; i++)
                        {
                            octetString[i] = ipAddressBytes[i];
                        }

                        stringSize = objectSize;
                    }
                }
            }

            return (stringSize);
        }

        public static int CountCodifiedOIDComponents(byte[] buffer, int bufferLength, int oidIndex, int oidLength)
        {
            if(oidLength <= 0)
            {
                return (0);
            }

            if(bufferLength < (oidIndex + oidLength))
            {
                return (0);
            }

            int oidComponentsCount = 2;
            int tempOidIndex = oidIndex + 1;
            int tempOidLength = oidIndex + oidLength;
            while (tempOidIndex < tempOidLength)
            {
                int integerSize = 1;
                while((tempOidIndex + integerSize) < tempOidLength)
                {
                    if((buffer[tempOidIndex + integerSize - 1] & 0x80) != 0)
                    {
                        integerSize++;
                    }
                    else
                    {
                        break;
                    }
                }

                tempOidIndex += integerSize;
                oidComponentsCount++;
            }

            return (oidComponentsCount);
        }

        public static int ASN1DecodifyOID(byte[] buffer, int bufferLength, int oidIndex, ref string oidString)
        {
            oidString = String.Empty;

            // The returned value;
            int codifiedOidSize = 0;

            // Check the object type (6 == OID)
            int tempIndex = oidIndex;
            if(bufferLength < (oidIndex + 1))
            {
                return (0);
            }
            if(buffer[tempIndex++] != 0x06)
            {
                return (0);
            }
            codifiedOidSize++;

            // Check the length of the OID
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(buffer, bufferLength, tempIndex, ref lengthSize);
            if ((lengthValue == 0) || (bufferLength < (lengthValue + tempIndex + lengthSize)))
            {
                return (0);
            }
            tempIndex += lengthSize;
            codifiedOidSize += lengthSize + lengthValue;

            // Count the number of integers that compose the OID
            int oidComponentsCount = CountCodifiedOIDComponents(buffer, bufferLength, tempIndex, lengthValue);
            if(oidComponentsCount < 2)
            {
                return (0);
            }

            // Parse the OID
            int[] oidIntValues = new int[oidComponentsCount];

            // The first byte of the codified OID is x*40 + y
            oidIntValues[0] = buffer[tempIndex] / 40;
            oidIntValues[1] = buffer[tempIndex] % 40;
            int tempOidIndex = 1;
            int componentIndex = 2;
            while((tempOidIndex < lengthValue) && (componentIndex < oidComponentsCount))
            {
                // Calculate the size in bytes of the OID next component
                int integerSize = 1;
                while ((tempOidIndex + integerSize) < lengthValue)
                {
                    if ((buffer[tempIndex + tempOidIndex + integerSize - 1] & 0x80) != 0)
                    {
                        integerSize++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Parse the next OID component
                int integerValue = 0;
                for(int i=0; i<integerSize; i++)
                {
                    int tempIntegerValue = buffer[tempIndex + tempOidIndex + i];
                    if((tempIntegerValue & 0x80) != 0)
                    {
                        tempIntegerValue -= 0x80;
                    }
                    tempIntegerValue <<= (7 * (integerSize - 1 - i));
                    integerValue += tempIntegerValue;
                }
 
                tempOidIndex += integerSize;
                oidIntValues[componentIndex++] = integerValue;
            }

            // Set the OID string
            for(componentIndex=0; componentIndex<oidComponentsCount; componentIndex++)
            {
                if(componentIndex != 0)
                {
                    oidString += ".";
                }

                oidString += oidIntValues[componentIndex].ToString();
            }

            return (codifiedOidSize);
        }

        public static uint ASN1GetCodifiedLengthValueLength(int intValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            if(intValue < 0x80)
            {
                codifiedValueLength = 1;
            }
            else if(intValue <= 0xff)
            {
                codifiedValueLength = 2;
            }
            else if (intValue <= 0xffff)
            {
                codifiedValueLength = 3;
            }
            else if (intValue <= 0xffffff)
            {
                codifiedValueLength = 4;
            }
            else
            {
                codifiedValueLength = 5;
            }

            return (codifiedValueLength);
        }

        public static uint ASN1GetCodifiedLengthValue(uint intValue, ref byte[] codifiedValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            if (intValue < 0x80)
            {
                codifiedValue[codifiedValueLength++] = (byte)intValue;
            }
            else if (intValue <= 0xff)
            {
                codifiedValue[codifiedValueLength++] = 0x81;
                codifiedValue[codifiedValueLength++] = (byte)intValue;
            }
            else if (intValue <= 0xffff)
            {
                codifiedValue[codifiedValueLength++] = 0x82;
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 8) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)(intValue & 0xff);
            }
            else if (intValue <= 0xffffff)
            {
                codifiedValue[codifiedValueLength++] = 0x83;
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 16) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 8) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)(intValue & 0xff);
            }
            else
            {
                codifiedValue[codifiedValueLength++] = 0x84;
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 24) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 16) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)((intValue >> 8) & 0xff);
                codifiedValue[codifiedValueLength++] = (byte)(intValue & 0xff);
            }

            return (codifiedValueLength);
        }

        public static uint ASN1GetCodifiedOctetStringValueLength(byte[] valueToBeCodified, int valueByteLength)
        {
            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            if (valueByteLength > 0)
            {
                // Type of the object (1 byte)
                octetStringLength++;

                // Length of the string (variable numbers of byte)
                octetStringLength += ASN1GetCodifiedLengthValueLength(valueByteLength);

                // Octets of the string (length of the string)
                octetStringLength += (uint)valueByteLength;
            }

            return (octetStringLength);
        }

        public static uint ASN1GetCodifiedOctetStringValue(byte[] valueToBeCodified, int valueByteLength, SNMPDATATYPE intType, ref byte[] codifiedValue)
        {
            // Check the integer type
            byte codifiedType = 0x04;
            switch (intType)
            {
                case SNMPDATATYPE.OctetString:
                    codifiedType = 0x04;
                    break;

                case SNMPDATATYPE.IpAddress:
                    codifiedType = 0x40;
                    break;

                default:
                    return (0);
            }

            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            uint codifiedStringLength = ASN1GetCodifiedLengthValueLength(valueByteLength);
            if ((valueByteLength > 0))
            {
                // Allocate a temporary buffer for the codified length of the string
                byte[] codifiedLengthValue = new byte[codifiedStringLength];

                // Type of the object (1 byte)
                codifiedValue[octetStringLength++] = codifiedType;

                // Codified Length
                ASN1GetCodifiedLengthValue((uint)valueByteLength, ref codifiedLengthValue);
                uint i = 0;
                for (i = 0; i < codifiedStringLength; i++)
                {
                    codifiedValue[octetStringLength++] = codifiedLengthValue[i];
                }

                // Codified value
                for (i = 0; i < (uint)valueByteLength; i++)
                {
                    codifiedValue[octetStringLength++] = valueToBeCodified[i];
                }
            }

            return (octetStringLength);
        }

        public static uint ASN1GetCodifiedIpAddressValueLength(byte[] valueToBeCodified, int valueByteLength)
        {
            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            if ((valueByteLength >= 4) && CheckIpAddressString(valueToBeCodified))
            {
                // Type of the object (1 byte)
                octetStringLength++;

                // Length of the string (1 byte == 4)
                octetStringLength ++;

                // Octets of the string (4)
                octetStringLength += 4;
            }

            return (octetStringLength);
        }

        public static bool CheckIpAddressString(byte[] ipAddress)
        {
            // Encode the IP address as a string
            Encoding ascii = Encoding.ASCII;
            char[] asciiChars = new char[ascii.GetCharCount(ipAddress, 0, ipAddress.Length)];
            ascii.GetChars(ipAddress, 0, ipAddress.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            // Parse the string of the IP address
            char delimiter = '.';
            string[] nodeArray = asciiString.Split(delimiter);

            // Check the number of components of the OID address
            int nodeCounter = nodeArray.Count();
            if (nodeCounter != 4)
            {
                return (false);
            }

            // Check the octets of the IP Address 
            foreach (var substring in nodeArray)
            {
                if (String.IsNullOrWhiteSpace(substring))
                {
                    return (false);
                }

                // Check the integer value of the next octet 
                int tempElementNumber = 0;
                if (!int.TryParse(substring, out tempElementNumber) ||
                    (tempElementNumber < 0) || (tempElementNumber > 0xff))
                {
                    return (false);
                }
             }

            return (true);
        }

        public static bool ParseIpAddressString(byte[] ipAddress, ref byte[] ipAddressOctets)
        {
            // Encode the IP address as a string
            Encoding ascii = Encoding.ASCII;
            char[] asciiChars = new char[ascii.GetCharCount(ipAddress, 0, ipAddress.Length)];
            ascii.GetChars(ipAddress, 0, ipAddress.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            // Parse the string of the IP address
            char delimiter = '.';
            string[] nodeArray = asciiString.Split(delimiter);

            // Check the number of components of the OID address
            int nodeCounter = nodeArray.Count();
            if (nodeCounter != 4)
            {
                return(false);
            }

            // Get the octets of the IP Address 
            int i = 0;
            foreach (var substring in nodeArray)
            {
                if (String.IsNullOrWhiteSpace(substring))
                {
                    return(false);
                }

                // Get the integer value of the next octet 
                int tempElementNumber = 0;
                if (!int.TryParse(substring, out tempElementNumber) ||
                    (tempElementNumber < 0) || (tempElementNumber > 0xff))
                {
                    return(false);
                }
                ipAddressOctets[i++] = (byte)tempElementNumber;
            }

            return (true);
        }

        public static uint ASN1GetCodifiedIpAddressValue(byte[] valueToBeCodified, int valueByteLength, SNMPDATATYPE intType, ref byte[] codifiedValue)
        {
            // Allocate a temporary buffer for the codified value of the IP address
            byte[] codifiedIpAddress = new byte[4];

            // Parse the octets of the IP address string
            if (!ParseIpAddressString(valueToBeCodified, ref codifiedIpAddress))
            {
                return (0);
            }

            // The returned value: length of the octet string
            uint octetStringLength = 0;

            // The string must not be empty
            uint codifiedStringLength = ASN1GetCodifiedLengthValueLength(4);
            if ((valueByteLength > 0))
            {
                // Allocate a temporary buffer for the codified length of the string
                byte[] codifiedLengthValue = new byte[codifiedStringLength];

                // Type of the object (1 byte)
                codifiedValue[octetStringLength++] = 0x40;

                // Codified Length
                ASN1GetCodifiedLengthValue((uint)4, ref codifiedLengthValue);
                uint i = 0;
                for (i = 0; i < codifiedStringLength; i++)
                {
                    codifiedValue[octetStringLength++] = codifiedLengthValue[i];
                }

                // Codified value
                for (i = 0; i < (uint)4; i++)
                {
                    codifiedValue[octetStringLength++] = codifiedIpAddress[i];
                }
            }

            return (octetStringLength);
        }

        public static uint ASN1GetCodifiedIntValueLength(byte[] valueToBeCodified, int valueByteLength)
        {
            // The returned value: length of the octet string
            uint intLength = 0;

            // Check the data length
            if ((valueByteLength > 0) && (valueByteLength < 5))
            {
                // Initiate the data buffer
                byte[] valueBuffer = new byte[4];
                int i = 0;
                for(i=0; i<4; i++)
                {
                    valueBuffer[i] = 0;
                }

                // Copy the data bytes
                int j = 0;
                for (i = 0, j = 0; i < 4 && j < valueByteLength; i++, j++)
                {
                    valueBuffer[i] = valueToBeCodified[j];
                }

                // Get the integer value
                int integerValue = BitConverter.ToInt32(valueBuffer, 0);

                // Calculate the size of the codified value
                int integerSize = 4;
                uint mask = 0x1FF;
                mask <<= 23;
                while ((((integerValue & mask) == 0) || ((integerValue & mask) == mask)) && (integerSize > 1))
                {
                    integerSize--;
                    integerValue <<= 8;
                }

                // Type of the object (1 byte)
                intLength++;

                // Size (1 byte)
                intLength++;

                // Value
                intLength += (uint)integerSize;
            }

            return (intLength);
        }

        public static uint ASN1CodifyIntValue(byte[] valueToBeCodified, int valueByteLength, SNMPDATATYPE intType, ref byte[] codifiedValue)
        {
            // Check the integer type
            byte codifiedIntType = 0x02;
            switch (intType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedIntType = 0x02;
                    break;

                case SNMPDATATYPE.Counter32:
                    codifiedIntType = 0x41;
                    break;

                case SNMPDATATYPE.Gauge32:
                    codifiedIntType = 0x42;
                    break;

                case SNMPDATATYPE.TimeTicks:
                    codifiedIntType = 0x43;
                    break;

                case SNMPDATATYPE.Unsigned32:
                    codifiedIntType = 0x47;
                    break;

                default:
                    return (0);
            }

            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            // Check the data length
            if ((valueByteLength > 0) && (valueByteLength < 5))
            {
                // Initiate the data buffer
                byte[] valueBuffer = new byte[4];
                int i = 0;
                for (i = 0; i < 4; i++)
                {
                    valueBuffer[i] = 0;
                }

                // Copy the data bytes
                int j = 0;
                for (i = 0, j = 0; i < 4 && j < valueByteLength; i++, j++)
                {
                    valueBuffer[i] = valueToBeCodified[j];
                }

                // Get the integer value
                int integerValue = BitConverter.ToInt32(valueBuffer, 0);
                int integerSize = 4;
                uint mask = 0x1FF;
                mask <<= 23;

                while ((((integerValue & mask) == 0) || ((integerValue & mask) == mask)) && (integerSize > 1))
                {
                    integerSize--;
                    integerValue <<= 8;
                }

                // Type
                codifiedValue[codifiedValueLength++] = codifiedIntType;

                // Size
                codifiedValue[codifiedValueLength++] = (byte)integerSize;

                mask = 0xFF;
                mask <<= 24;
                // Codified Value
                while ((integerSize--) > 0)
                {
                    codifiedValue[codifiedValueLength++] = (byte)((integerValue & mask) >> 24);
                    integerValue <<= 8;
                }
            }

            return (codifiedValueLength);
        }

        public static uint ASN1GetCodifiedUintValueLength(byte[] valueToBeCodified, int valueByteLength)
        {
            // The returned value: length of the octet string
            uint intLength = 0;

            // Check the data length
            if ((valueByteLength > 0) && (valueByteLength < 5))
            {
                // Initial the data buffer
                byte[] valueBuffer = new byte[4];
                int i = 0;
                for (i = 0; i < 4; i++)
                {
                    valueBuffer[i] = 0;
                }

                // Copy the data bytes
                int j = 0;
                for (i = (4 - valueByteLength), j = 0; i < 4 && j < valueByteLength; i++, j++)
                {
                    valueBuffer[i] = valueToBeCodified[j];
                }

                // Get the integer value
                uint unsignedValue = BitConverter.ToUInt32(valueBuffer, 0);

                // Calculate the size of the codified value
                int integerSize = 4;
                if(((unsignedValue >> 24) & 0xff) != 0)
                {
                    integerSize = 4;
                }
                else if (((unsignedValue >> 16) & 0xff) != 0)
                {
                    integerSize = 3;
                }
                else if (((unsignedValue >> 8) & 0xff) != 0)
                {
                    integerSize = 2;
                }
                else
                {
                    integerSize = 1;
                }
                if(((unsignedValue >> (8 + (integerSize - 1))) & 0x80) != 0)
                {
                    integerSize++;
                }

                // Type of the object (1 byte)
                intLength++;

                // Size (1 byte)
                intLength++;

                // Value
                intLength += (uint)integerSize;
            }

            return (intLength);
        }

        public static uint ASN1CodifyUintValue(byte[] valueToBeCodified, int valueByteLength, SNMPDATATYPE intType, ref byte[] codifiedValue)
        {
            // Check the integer type
            byte codifiedIntType = 0x02;
            switch (intType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedIntType = 0x02;
                    break;

                case SNMPDATATYPE.Counter32:
                    codifiedIntType = 0x41;
                    break;

                case SNMPDATATYPE.Gauge32:
                    codifiedIntType = 0x42;
                    break;

                case SNMPDATATYPE.TimeTicks:
                    codifiedIntType = 0x43;
                    break;

                case SNMPDATATYPE.Unsigned32:
                    codifiedIntType = 0x47;
                    break;

                default:
                    return (0);
            }

            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            // Check the data length
            if ((valueByteLength > 0) && (valueByteLength < 5))
            {
                // Initiate the data buffer
                byte[] valueBuffer = new byte[4];
                int i = 0;
                for (i = 0; i < 4; i++)
                {
                    valueBuffer[i] = 0;
                }

                // Copy the data bytes
                int j = 0;
                for (i = (4 - valueByteLength), j = 0; i < 4 && j < valueByteLength; i++, j++)
                {
                    valueBuffer[i] = valueToBeCodified[j];
                }

                // Get the integer value
                uint unsignedValue = BitConverter.ToUInt32(valueBuffer, 0);

                // Calculate the size of the codified value
                int integerSize = 4;
                if (((unsignedValue >> 24) & 0xff) != 0)
                {
                    integerSize = 4;
                }
                else if (((unsignedValue >> 16) & 0xff) != 0)
                {
                    integerSize = 3;
                }
                else if (((unsignedValue >> 8) & 0xff) != 0)
                {
                    integerSize = 2;
                }
                else
                {
                    integerSize = 1;
                }
                if (((unsignedValue >> (8 + (integerSize - 1))) & 0x80) != 0)
                {
                    integerSize++;
                }

                // Type
                codifiedValue[codifiedValueLength++] = codifiedIntType;

                // Size
                codifiedValue[codifiedValueLength++] = (byte)integerSize;

                // Codified Value
                int firstIndex = 0;
                if (integerSize == 5)
                {
                    firstIndex = 1;
                    codifiedValue[codifiedValueLength++] = (byte)0;
                }

                for(i=firstIndex; i<integerSize; i++)
                {
                    codifiedValue[codifiedValueLength++] = (byte)(unsignedValue >> (8 * ((integerSize - 1) - i) & 0xff));
                }
            }

            return (codifiedValueLength);
        }

        public static uint GetMaxJobSize(LinkType Type)
        {
            //if (Type == DriverCodeBase.Enumerators.LinkType.Input)
            //    return  MAX_DATA_SIZE_INPUT;
            //else
            //    return  MAX_DATA_SIZE;
            return (SNMPCommJob.MAX_SNMP_PACKET / 2);
        }

        //public static UFUAModel.DataType DataType(SNMPAddress addObj)
        //{
        //    switch (addObj.DataFormat)
        //    {
        //        case DataFormats.Word:
        //            return UFUAModel.DataType.UInt16;
        //        case DataFormats.Bit:
        //            return UFUAModel.DataType.Boolean;
        //        default:
        //            return 0;
        //    }
        //}
        #endregion

        #region methods override
        //public static uint GetFrameLength(SNMPCommJob job)
        //{
        //    return BUFFER_SIZE;
        //}

        public static string GetErrorString(int err)
        {
            String str;
            switch (err)
            {
                case 0x01:
                    str = Properties.Resources.SNMPErrorStatusTOOBIG;
                    break;
                case 0x02:
                    str = Properties.Resources.SNMPErrorStatusNOSUCHNAME;
                    break;
                case 0x03:
                    str = Properties.Resources.SNMPErrorStatusBADVALUE;
                    break;
                case 0x04:
                    str = Properties.Resources.SNMPErrorStatusREADONLY;
                    break;
                case 0x05:
                    str = Properties.Resources.SNMPErrorStatusGENERR;
                    break;
                case 0x06:
                    str = Properties.Resources.SNMPErrorStatusNOACCESS;
                    break;
                case 0x07:
                    str = Properties.Resources.SNMPErrorStatusWRONGTYPE;
                    break;
                case 0x08:
                    str = Properties.Resources.SNMPErrorStatusWRONGLENGTH;
                    break;
                case 0x09:
                    str = Properties.Resources.SNMPErrorStatusWRONGENCODING;
                    break;
                case 0x0a:
                    str = Properties.Resources.SNMPErrorStatusWRONGVALUE;
                    break;
                case 0x0b:
                    str = Properties.Resources.SNMPErrorStatusNOCREATION;
                    break;
                case 0x0c:
                    str = Properties.Resources.SNMPErrorStatusINCONSISTENTVALUE;
                    break;
                case 0x0d:
                    str = Properties.Resources.SNMPErrorStatusRESOURCEUNAVAILABLE;
                    break;
                case 0x0e:
                    str = Properties.Resources.SNMPErrorStatusCOMMITFAILED;
                    break;
                case 0x0f:
                    str = Properties.Resources.SNMPErrorStatusUNDOFAILED;
                    break;
                case 0x10:
                    str = Properties.Resources.SNMPErrorStatusAUTHORIZATIONERROR;
                    break;
                case 0x11:
                    str = Properties.Resources.SNMPErrorStatusNOTWRITABLE;
                    break;
                case 0x12:
                    str = Properties.Resources.SNMPErrorStatusINCONSISTENTNAME;
                    break;
                default:
                    str = Properties.Resources.SNMPErrorStatusUNKNOWN;
                    break;
            }
            return str;
        }

        public static uint PrepareRequest(List<SNMPCommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            if (jList[0].Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (jList[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                jList[0].TagsListToWrite.Count == 0))
            {
                return PrepareReadRequest(jList, ref buffer, snmpRequestID, snmpVersion);
            }
            else
            {
                return PrepareWriteRequest(jList, ref buffer, snmpRequestID, snmpVersion);
            }
        }

        public static uint PrepareReadRequest(List<SNMPCommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            // Returned value
            uint requestTotalLength = 0;

            // Calculate the total length of the job requests and the length of the codified community string 
            uint tempJobsRequestLength = 0;
            uint tempCommunityLength = 0;
            bool firstIteration = true;
            foreach (SNMPCommJob j in jList)
            {
                j.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;
                if (firstIteration == true)
                {
                    tempCommunityLength = j.GetCodifiedCommunityLength();
                    firstIteration = false;
                }
                tempJobsRequestLength += j.GetReadRequestLength();  
            }
            if(tempJobsRequestLength == 0)
            {
                return (0);
            }

            // Get the length of the codified request ID
            uint codifiedRequestIdLength = ASN1GetCodifiedIntLength((int)snmpRequestID);

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + tempJobsRequestLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            buffer[requestTotalLength++] = 0x30;
            // Length of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength02 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength02;
            // Protocol version
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            if(snmpVersion == SNMPVERSION.SNMPv1)
            {
                buffer[requestTotalLength++] = 0;
            }
            else
            {
                buffer[requestTotalLength++] = 0x01;
            }
            // Community
            SNMPCommJob job = jList[0];
            byte[] communityBuffer = new byte[tempCommunityLength];
            job.GetCodifiedCommunity(ref communityBuffer);
            uint i = 0;
            for(i=0; i<tempCommunityLength; i++)
            {
                buffer[requestTotalLength++] = communityBuffer[i];
            }
            // Message type (get-request)
            buffer[requestTotalLength++] = 0xa0;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength01 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength01;
            // Request-ID
            byte[] codifiedRequestID = new byte[codifiedRequestIdLength];
            ASN1CodifyInt((int)(snmpRequestID++), ref codifiedRequestID);
            for (i = 0; i < codifiedRequestIdLength; i++)
            {
                buffer[requestTotalLength++] = codifiedRequestID[i];
            }
            // Error status
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Error index
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Job requests
            buffer[requestTotalLength++] = 0x30;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempJobsRequestLength >> 8);
            buffer[requestTotalLength++] = (byte)tempJobsRequestLength;
            foreach (SNMPCommJob jb in jList)
            {
                uint jobCodifiedOidLength = jb.GetCodifiedOIDLength();
                if(jobCodifiedOidLength > 0)
                {
                    // Job request
                    buffer[requestTotalLength++] = 0x30;
                    // Request length
                    buffer[requestTotalLength++] = 0x82;
                    uint jobRequestLength = jobCodifiedOidLength + 2;
                    buffer[requestTotalLength++] = (byte)(jobRequestLength >> 8);
                    buffer[requestTotalLength++] = (byte)jobRequestLength;
                    // Codified OID
                    byte[] codifiedOid = new byte[jobCodifiedOidLength];
                    jb.GetCodifiedOID(ref codifiedOid);
                    for(i=0; i< jobCodifiedOidLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedOid[i];
                    }
                    // Null value
                    buffer[requestTotalLength++] = 0x05;
                    buffer[requestTotalLength++] = 0;
                }
            }


            return (requestTotalLength);
        }

        public static uint PrepareWriteRequest(List<SNMPCommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            // Returned value
            uint requestTotalLength = 0;

            // Calculate the total length of the job requests and the length of the codified community string 
            uint tempJobsRequestLength = 0;
            uint tempJobRequestLength = 0;
            uint tempCommunityLength = 0;
            bool firstIteration = true;            

            foreach (SNMPCommJob j in jList)
            {
                j.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;                
                if (firstIteration == true)
                {
                    tempCommunityLength = j.GetCodifiedCommunityLength();
                    firstIteration = false;
                }

                tempJobsRequestLength = j.GetWriteRequestLength();

                tempJobsRequestLength += (uint)tempJobRequestLength;
            }
            if (tempJobsRequestLength == 0)
            {
                return (0);
            }

            // Get the length of the codified request ID
            uint codifiedRequestIdLength = ASN1GetCodifiedIntLength((int)snmpRequestID);

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + tempJobsRequestLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            buffer[requestTotalLength++] = 0x30;
            // Length of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength02 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength02;
            // Protocol version
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            if (snmpVersion == SNMPVERSION.SNMPv1)
            {
                buffer[requestTotalLength++] = 0;
            }
            else
            {
                buffer[requestTotalLength++] = 0x01;
            }
            // Community
            SNMPCommJob job = jList[0];
            byte[] communityBuffer = new byte[tempCommunityLength];
            job.GetCodifiedCommunity(ref communityBuffer);
            uint i = 0;
            for (i = 0; i < tempCommunityLength; i++)
            {
                buffer[requestTotalLength++] = communityBuffer[i];
            }
            // Message type (set-request)
            buffer[requestTotalLength++] = 0xa3;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength01 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength01;
            // Request-ID
            byte[] codifiedRequestID = new byte[codifiedRequestIdLength];
            ASN1CodifyInt((int)(snmpRequestID++), ref codifiedRequestID);
            for (i = 0; i < codifiedRequestIdLength; i++)
            {
                buffer[requestTotalLength++] = codifiedRequestID[i];
            }
            // Error status
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Error index
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Job requests
            buffer[requestTotalLength++] = 0x30;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempJobsRequestLength >> 8);
            buffer[requestTotalLength++] = (byte)tempJobsRequestLength;
            foreach (SNMPCommJob jb in jList)
            {
                uint jobCodifiedOidLength = jb.GetCodifiedOIDLength();
                //jobDataLength = jb.GetTheDataLength();
                if ((jobCodifiedOidLength > 0) && (jb.WriteDataLength > 0))
                {
                    // Job request
                    buffer[requestTotalLength++] = 0x30;
                    // Request length
                    buffer[requestTotalLength++] = 0x82;
                    uint jobRequestLength = jobCodifiedOidLength + (uint)jb.WriteDataLength;
                    buffer[requestTotalLength++] = (byte)(jobRequestLength >> 8);
                    buffer[requestTotalLength++] = (byte)jobRequestLength;
                    // Codified OID
                    byte[] codifiedOid = new byte[jobCodifiedOidLength];
                    jb.GetCodifiedOID(ref codifiedOid);
                    for (i = 0; i < jobCodifiedOidLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedOid[i];
                    }
                    // Codified value
                    byte[] codifiedValue = new byte[jb.WriteDataLength];
                    jb.GetCodifiedData(ref codifiedValue);
                    for (i = 0; i < jb.WriteDataLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedValue[i];
                    }

                    jb.WriteExecuted = true;
                }
            }

            return (requestTotalLength);
        }

        public static bool CompatibleDataTypes(SNMPDATATYPE dataType1, SNMPDATATYPE dataType2)
        {
            // Returned value
            bool dataTypesAreCompatible = false;

            if ( dataType1 == dataType2)
            {
                dataTypesAreCompatible = true;
            }
            else
            {
                switch(dataType1)
                {
                    case SNMPDATATYPE.Integer:
                        if((dataType2 == SNMPDATATYPE.Integer32) ||
                           (dataType2 == SNMPDATATYPE.Counter32) ||
                           (dataType2 == SNMPDATATYPE.Unsigned32) ||
                           (dataType2 == SNMPDATATYPE.Gauge32) ||
                           (dataType2 == SNMPDATATYPE.TimeTicks))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.Integer32:
                        if ((dataType2 == SNMPDATATYPE.Integer) ||
                           (dataType2 == SNMPDATATYPE.Counter32) ||
                           (dataType2 == SNMPDATATYPE.Unsigned32) ||
                           (dataType2 == SNMPDATATYPE.Gauge32) ||
                           (dataType2 == SNMPDATATYPE.TimeTicks))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.Counter32:
                        if ((dataType2 == SNMPDATATYPE.Integer) ||
                           (dataType2 == SNMPDATATYPE.Integer32) ||
                           (dataType2 == SNMPDATATYPE.Unsigned32) ||
                           (dataType2 == SNMPDATATYPE.Gauge32) ||
                           (dataType2 == SNMPDATATYPE.TimeTicks))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.Unsigned32:
                        if ((dataType2 == SNMPDATATYPE.Integer) ||
                           (dataType2 == SNMPDATATYPE.Integer32) ||
                           (dataType2 == SNMPDATATYPE.Counter32) ||
                           (dataType2 == SNMPDATATYPE.Gauge32) ||
                           (dataType2 == SNMPDATATYPE.TimeTicks))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.Gauge32:
                        if ((dataType2 == SNMPDATATYPE.Integer) ||
                           (dataType2 == SNMPDATATYPE.Integer32) ||
                           (dataType2 == SNMPDATATYPE.Counter32) ||
                           (dataType2 == SNMPDATATYPE.Unsigned32) ||
                           (dataType2 == SNMPDATATYPE.TimeTicks))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.TimeTicks:
                        if ((dataType2 == SNMPDATATYPE.Integer) ||
                           (dataType2 == SNMPDATATYPE.Integer32) ||
                           (dataType2 == SNMPDATATYPE.Counter32) ||
                           (dataType2 == SNMPDATATYPE.Unsigned32) ||
                           (dataType2 == SNMPDATATYPE.Gauge32))
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.OctetString:
                        if (dataType2 == SNMPDATATYPE.IpAddress)
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;

                    case SNMPDATATYPE.IpAddress:
                        if (dataType2 == SNMPDATATYPE.OctetString)
                        {
                            dataTypesAreCompatible = true;
                        }
                        break;
                }
            }

            return (dataTypesAreCompatible);
        }

        public static int GetDecodedOctetStringSize(byte[] buffer, int bufferLength, int typeIndex)
        {
            // Returned value
            int octetStringLength = 0;

            int lengthIndex = typeIndex + 1;
            int lengthSize = 0;
           octetStringLength = ASN1DecodifyLength(buffer, bufferLength, lengthIndex, ref lengthSize);

            return (octetStringLength);
        }

        public static int GetDecodedIpAddressSize(byte[] buffer, int bufferLength, int typeIndex)
        {
            // Returned value
            int octetStringLength = 0;

            // Check the length of the octet string (it must be 4)
            int lengthIndex = typeIndex + 1;
            if(bufferLength < (lengthIndex + 5))
            {
                return (0);
            }
            int length = buffer[lengthIndex];
            if(length != 4)
            {
                return (0);
            }

            // Convert the octets of the IP Address in a string
            int dataIndex = lengthIndex + 1;
            string ipAddressString = String.Format("{0}.{1}.{2}.{3}",
                                                   buffer[dataIndex],
                                                   buffer[dataIndex + 1],
                                                   buffer[dataIndex + 2],
                                                   buffer[dataIndex + 3]);

            // The length of the decoded IP address is the length of the string
            octetStringLength = ipAddressString.Length;

            return (octetStringLength);
        }

        public static int GetDecodedDataSize(byte[] buffer, int bufferLength, int typeIndex)
        {
            // Returned value
            int dataSize = 0;

            if (bufferLength > typeIndex)
            {
                byte dataType = buffer[typeIndex];
                switch(dataType)
                {
                    case 0x02: // Integer/Integer32
                    case 0x41: // Counter32
                    case 0x42: // Gauge32
                    case 0x43: // TimeTicks
                    case 0x47: // Unsigned32
                        dataSize = 4;
                        break;

                    case 0x04: // Octet String
                        dataSize = GetDecodedOctetStringSize(buffer, bufferLength, typeIndex);
                        break;

                    case 0x40: // IP address
                        dataSize = GetDecodedIpAddressSize(buffer, bufferLength, typeIndex);
                        break;
                }
            }

            return (dataSize);
        }

        public static int ParseIntegerData(byte[] buffer, int bufferLength, int dataIndex, ref byte[] dataBuffer)
        {
            // Returned value
            int dataSize = 0;

            int integerSize = 0;
            int integerValue = ASN1DecodifyIntegerValue(buffer, bufferLength, dataIndex, ref integerSize);
            if(integerSize > 0)
            {
                
                dataSize = 4;
                if(dataBuffer.Count() >= dataSize)
                {
                    dataBuffer = BitConverter.GetBytes(integerValue);
                }
            }

            return (dataSize);
        }

        public static int ParseUnsignedData(byte[] buffer, int bufferLength, int dataIndex, ref byte[] dataBuffer)
        {
            // Returned value
            int dataSize = 0;

            int integerSize = 0;
            uint unsignedValue = ASN1DecodifyUnsignedValue(buffer, bufferLength, dataIndex, ref integerSize);
            if (integerSize > 0)
            {
                dataSize = 4;
                if (dataBuffer.Count() >= dataSize)
                {
                    dataBuffer = BitConverter.GetBytes(unsignedValue);
                }
            }

            return (dataSize);
        }

        public static int ParseStringData(byte[] buffer, int bufferLength, int dataIndex, ref byte[] dataBuffer)
        {
            // Returned value
            int dataSize = ASN1DecodifyOctetString(buffer, bufferLength, dataIndex, ref dataBuffer);

            return (dataSize);
        }

        public static int ParseIpAddressData(byte[] buffer, int bufferLength, int dataIndex, ref byte[] dataBuffer)
        {
            // Returned value
            int dataSize = ASN1DecodifyIpAddress(buffer, bufferLength, dataIndex, ref dataBuffer);

            return (dataSize);
        }

        public static int ParseJobData(byte[] buffer, int bufferLength, int dataIndex, ref byte[] dataBuffer)
        {
            // Returned value
            int dataSize = 0;

            if (bufferLength > dataIndex)
            {
                byte dataType = buffer[dataIndex];
                switch (dataType)
                {
                    case 0x02: // Integer/Integer32
                        dataSize = ParseIntegerData(buffer, bufferLength, dataIndex, ref dataBuffer);
                        break;

                    case 0x41: // Counter32
                    case 0x42: // Gauge32
                    case 0x43: // TimeTicks
                    case 0x47: // Unsigned32
                        dataSize = ParseUnsignedData(buffer, bufferLength, dataIndex, ref dataBuffer);
                        break;

                    case 0x04: // Octet string
                        dataSize = ParseStringData(buffer, bufferLength, dataIndex, ref dataBuffer);
                        break;

                    case 0x40: // IP address
                        dataSize = ParseIpAddressData(buffer, bufferLength, dataIndex, ref dataBuffer);
                        break;
                }
            }

            return (dataSize);
        }

        public static DriverErrorCodes ParseData(byte[] receiveBuffer, ref SNMPCommJob j, ref List<object> items)
        {
            // Check the length of the received answer
            int dataSize = receiveBuffer.Length;
            if(dataSize <= 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 1", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (DriverErrorCodes.ErrorParsingAnswer);
            }

            // Parse the OID  
            string parsedOID = String.Empty;
            int parsedBytes = ASN1DecodifyOID(receiveBuffer, dataSize, 0, ref parsedOID);
            if((parsedBytes == 0) || String.IsNullOrEmpty(parsedOID))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 2", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (DriverErrorCodes.ErrorParsingAnswer);
            }

            // Check the OID
            if(!parsedOID.Equals(j.snmpOid_Address, StringComparison.Ordinal))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 4", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorOidMismatch);
            }
 
            // Get the data type
            if(dataSize < (parsedBytes + 1))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 5", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (DriverErrorCodes.ErrorParsingAnswer);
            }
            SNMPDATATYPE dataType = SNMPDATATYPE.Integer;
            switch(receiveBuffer[parsedBytes])
            {
                case 0x02:
                    dataType = SNMPDATATYPE.Integer;
                    break;

                case 0x04:
                    dataType = SNMPDATATYPE.OctetString;
                    break;

                case 0x40:
                    dataType = SNMPDATATYPE.IpAddress;
                    break;

                case 0x41:
                    dataType = SNMPDATATYPE.Counter32;
                    break;

                case 0x42:
                    dataType = SNMPDATATYPE.Gauge32;
                    break;

                case 0x43:
                    dataType = SNMPDATATYPE.TimeTicks;
                    break;

                case 0x47:
                    dataType = SNMPDATATYPE.Unsigned32;
                    break;

                case 0x80:
                    {
                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} parsingError 10", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorNoSuchObject);
                    }

                default:
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} parsingError 6", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorUnsupportedDataType);
                    }
            }

            // Check the data type
            if(!CompatibleDataTypes(dataType, j.snmpDataType))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 7", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorDataTypeMismatch);
            }

            // Check the decoded data length
            int decodedDataSize =  GetDecodedDataSize(receiveBuffer, dataSize, parsedBytes);
            if(decodedDataSize == 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 8", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (DriverErrorCodes.ErrorParsingAnswer);
            }

            // Parse data
            byte[] jobData = new byte[decodedDataSize];
            if(ParseJobData(receiveBuffer, dataSize, parsedBytes, ref jobData) == 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} parsingError 9", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (DriverErrorCodes.ErrorParsingAnswer);
            }
  
            // Copy parsed data
            List<Tag> changed = new List<Tag>();
            j.SetJobData(jobData, ref changed);
            items.AddRange(changed);

            return (DriverErrorCodes.ErrorNoError);
        }

        #endregion

    }
}
