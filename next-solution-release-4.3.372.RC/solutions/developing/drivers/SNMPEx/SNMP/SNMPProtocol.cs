using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace SNMP
{
    public partial class SNMPProtocol
    {        
        public class SNMPMessageGeneralData
        {
            public byte[] TrapRawData { get; set; }
            public IPEndPoint TrapRemoteEndPoint { get; set; }

            public byte PduType { get; set; }
            public SNMPVERSION snmpVersion { get; set; }
            public uint MaxPacket { get; set; }
            // global data
            public int MsgID { get; set; }
            
            public bool FlagAuthentication { get; set; }
            public bool FlagPrivacy { get; set; }
            public bool FlagReportable { get; set; }
            
            public SNMPUSM ParsedUsm { get; set; }

            //public byte ReplyType { get; set; }
            public int ReplyBufferIndex { get; set; }
            public string Community { get; set; }
            
            public int ErrorStatus { get; set; }
            public int ErrorIndex { get; set; }

            public List<SNMPVariableBinding> VariableBindings { get; set; }

            public bool IsMessageFromTrap { get; set; }

            public SNMPMessageGeneralData()
            {
                TrapRawData = new byte[0];
                TrapRemoteEndPoint = null;

                PduType = 0;
                snmpVersion = SNMPVERSION.SNMPv1;
                MaxPacket = SNMPProtocol.GetMaxPacketSize();
                MsgID = 0;
                FlagAuthentication = false;
                FlagPrivacy = false;
                FlagReportable = false;

                // during message receive used to assign some user and security property
                ParsedUsm = new SNMPUSM();

                VariableBindings = new List<SNMPVariableBinding>();
                ReplyBufferIndex = 0;
                Community = string.Empty;
                ErrorStatus = 0;
                ErrorIndex = 0;
                IsMessageFromTrap = false;
            }

            public SNMPVariableBinding GetVariableBindingByOID(string oidAddress)
            {
                return VariableBindings.Find(v => v.oID.stringValue == oidAddress);
            }

            public bool InError()
            {
                return (ErrorStatus != 0 || ErrorIndex != 0);
            }

            public DriverErrorCodes GetErrorCode()
            {
                if (ErrorStatus != 0 || ErrorIndex != 0)
                    return (DriverErrorCodes)(int)(SNMPProtocol.SNMPErrorNonZeroErrorStatus + ErrorStatus);
                else
                    return DriverErrorCodes.ErrorNoError;
            }

            public void SetTrapData(IPEndPoint remoteEndPoint, byte[] trapRawData)
            {
                TrapRemoteEndPoint = remoteEndPoint;
                TrapRawData = new byte[trapRawData.Length];
                Array.Copy(trapRawData, TrapRawData, trapRawData.Length);
            }

            public void SetUsmSecurity(SNMPStation st)
            {           
                switch (st.Usm.SecurityLevel)
                {
                    case SecurityLevel.NoAuthNoPriv:
                        FlagAuthentication = false;
                        FlagPrivacy = false;
                        break;
                    case SecurityLevel.AuthNoPriv:
                        FlagAuthentication = true;
                        FlagPrivacy = false;
                        break;
                    case SecurityLevel.AuthPriv:
                        FlagAuthentication = true;
                        FlagPrivacy = true;
                        break;
                }
            }

            public bool IsTrapOfInformPduType()
            {
                return ((SNMPProtocol.SnmpPduType)PduType == SNMPProtocol.SnmpPduType.Trap || (SNMPProtocol.SnmpPduType)PduType == SNMPProtocol.SnmpPduType.Inform || (SNMPProtocol.SnmpPduType)PduType == SNMPProtocol.SnmpPduType.V2Trap);
            }
        }

        public class SNMPVariableBinding
        {
            #region Data Members
            public SNMPOid oID { get; set; }
            public byte ValueType { get; set; }
            public int ValueSize { get; set; }
            public SNMPDATATYPE snmpDataType { get; set; }
            public int snmpDataSize { get; set; }
            public byte[] Value { get; set; }
            public byte[] ValueToWrite { get; set; }
            public byte[] TagValue { get; set; }
            public DriverErrorCodes ErrorCode { get; set; }
            #endregion

            public SNMPVariableBinding()
            {
                oID = new SNMPOid();
                ValueType = 0;
                ValueSize = 0;
                Value = null;
                ValueToWrite = null;
                TagValue = new byte[0];
                snmpDataType = SNMPDATATYPE.Unknown;
                snmpDataSize = 0;
                ErrorCode = DriverErrorCodes.ErrorNoError;
            }

            public SNMPVariableBinding(string oidAddress) : base()
            {
                oID = new SNMPOid(oidAddress);                
            }

            public SNMPVariableBinding(string oidAddress, byte[] valueToWrite)
            {
                oID = new SNMPOid(oidAddress);
                ValueToWrite = valueToWrite;
            }
        }

        public class ReadWriteListLimitateSize
        {
            #region Data Members
            uint reqDim;
            uint responseLength;            
            private int nrAggregatedJobs;
            private SNMPVERSION protocolVersion = SNMPVERSION.SNMPv1;
            private uint maxPacket = SNMPProtocol.GetMaxPacketSize();
            #endregion

            public ReadWriteListLimitateSize(SNMPVERSION version, uint maxpacket)
            {
                protocolVersion = version;
                nrAggregatedJobs = 0;
                reqDim = 0;
                responseLength = 0;
                maxPacket = maxpacket;
            }

            public void InitAggregation(SNMPCommJob j, bool write)
            {
                switch (protocolVersion)
                {
                    case SNMPVERSION.SNMPv1:
                    case SNMPVERSION.SNMPv2c:                                            
                        reqDim = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
                        responseLength = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (

                        uint communityLength = j.GetCodifiedCommunityLength();
                        reqDim += communityLength; // Community
                        reqDim += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                        uint requestIDLength = SNMPProtocol.ASN1GetCodifiedIntLength(0);
                        reqDim += requestIDLength; // Request ID
                        reqDim += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)

                        write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;

                        if (!write)
                        {
                            responseLength += communityLength; // Community
                            responseLength += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                            responseLength += requestIDLength; // Request ID
                            responseLength += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)
                        }
                        break;
                    case SNMPVERSION.SNMPv3:
                        // PrepareHeaderRequestv3 + PrepareUserSecurityModelRequestv3 + PreparePduRequestv3 + PrepareProtocolVersionRequestv3
                        uint userNameLen = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(((SNMPStation)j.Station).Usm.UserName);
                        reqDim = 16 + 30 + 16 + 6 + userNameLen;
                        responseLength = 16 + 30 + 16 + 6 + userNameLen;
                        break;
                }
            }

            public bool getReadListLimitate(SNMPCommJob j)
            {
                switch (protocolVersion)
                {
                    case SNMPVERSION.SNMPv1:
                    case SNMPVERSION.SNMPv2c:
                        {
                            // Check the length of the reply
                            uint jobResponseLength = j.GetReadResponseLength();
                            if ((responseLength + 1 + jobResponseLength) > maxPacket)
                                return false;

                            // Check the length of the request
                            uint jobRequestLength = j.GetReadRequestLength();
                            if ((reqDim + jobRequestLength + 1) > maxPacket)
                                return false;

                            responseLength += jobResponseLength + 1;
                            reqDim += jobRequestLength + 1;

                            nrAggregatedJobs += 1;
                        }
                        break;

                    case SNMPVERSION.SNMPv3:
                        {
                            // Check the length of the reply
                            uint jobResponseLength = j.GetReadResponseLength();
                            if ((responseLength + 1 + jobResponseLength) > maxPacket)
                                return false;

                            // Check the length of the request
                            uint jobRequestLength = j.GetReadRequestLength();
                            if ((reqDim + jobRequestLength + 1) > maxPacket)
                                return false;

                            responseLength += jobResponseLength + 1;
                            reqDim += jobRequestLength + 1;

                            nrAggregatedJobs += 1;
                        }

                        break;
                }

                return true;
            }

            public bool getWriteListLimitate(SNMPCommJob j)
            {
                // for now aggregatre only 1 job at time
                if (nrAggregatedJobs >= 1)
                    return false;

                nrAggregatedJobs++;

                return true;
            }
        }

        #region constants        

        public const byte SEQUENCE = 0x30;
        public const byte HIGH_BIT = 0x80;
        public const byte EXTENSION_ID = (byte)0x1F;
        public const byte SNMP_SECURITY_MODEL_USM = 3;
                
        public const int STRING_DEFAULT_SIZE = 255;
        public const int SNMP_IPADDRESS_DEFAULT_SIZE = 15;

        public const int TRAP_MESSAGES_QUEUE_MAX_SIZE = 2000;

        public const string SNMP_COMUNITY_PUBLIC = "public";

        public enum SnmProtocolDatatype : byte
        {
            Integer = 0x02,
            OctetString = 0x04,
            Null = 0x05,

            IpAddress = 0x40,
            Counter32 = 0x41,
            Gauge32 = 0x42,
            TimeTicks = 0x43,
            Unsigned32 = 0x47,
        }

        public enum SnmpPduType : byte
        {
            // Get request
            Get = 0xa0,
            // GetNext request
            GetNext = 0xa1,
            /// Response
            Response = 0xa2,
            // Set request
            Set = 0xa3,
            // Trap notification
            Trap = 0xa4,
            // GetBulk request
            GetBulk = 0xa5,
            // Inform notification
            Inform = 0xa6,
            // ver 2 Trap notification
            V2Trap = 0xa7,
            // ver 3 Report notification
            Report = 0xa8
        }

        public enum SnmpErrorStatusCode : int
        {
            noError = 0,
            tooBig = 1,
            NoSuchName = 2,
            BadValue = 3,
            ReadOnly = 4,
            genError = 5,
            noAccess = 6,
            wrongType = 7,
            wrongLength = 8,
            wrongEncoding = 9,
            wrongValue = 10,
            noCreation = 11,
            inconsistentValue = 12,
            resourceUnavailable = 13,
            commitFailed = 14,
            undoFailed = 15,
            authorizationError = 16,
            notWritable = 17,
            inconsistentName = 18,
        }

        public enum SnmpErrorVariableBindings : int
        {
            noError = 0,
            ErrornoSuchObject = 0x80,
            ErrornoSuchInstance = 0x81,
            ErrorendOfMIBView = 0x82,
        }

        public const int SNMPErrorNonZeroErrorStatus = 1500;
        public const int SNMPErrorNonZeroErrorBindVariables = 2000;

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
            SNMPErrorChannelTrapMessageSNMPv1,
            SNMPErrorChannelTrapWrongReply,
            SNMPErrorUserInvalid,
            SNMPErrorTrapOIDNoMatchAnyJobs,
            SNMPErrorEncryptDecriptFailed,
        }
        
        //public const uint MAX_REQUEST_NUMBER = 500;
        #endregion

        #region methods
        public static uint GetMaxPacketSize()
        {
            return Properties.Settings.Default.SNMP_PACKET_MAX_SIZE;
        }

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
                octetString[octetStringLength++] = (byte)SNMPProtocol.SnmProtocolDatatype.OctetString;

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
                        if ((byteBuffer[bufferIndex - 1] & SNMPProtocol.HIGH_BIT) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & SNMPProtocol.HIGH_BIT) != 0)
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
                        if((byteBuffer[bufferIndex-1] & HIGH_BIT) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & HIGH_BIT) != 0)
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
                        if ((byteBuffer[bufferIndex - 1] & HIGH_BIT) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & HIGH_BIT) != 0)
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

        public static uint ASN1CodifyInt(int intValue, out byte[] codifiedValue, uint codifiedValueSize)
        {
            codifiedValue = new byte[codifiedValueSize];

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
                        if ((byteBuffer[bufferIndex - 1] & HIGH_BIT) == 0)
                        {
                            continue;
                        }
                        break;

                    case 0xff:
                        if ((byteBuffer[bufferIndex - 1] & HIGH_BIT) != 0)
                        {
                            continue;
                        }
                        break;
                }

                break;
            }

            // Type (integer)
            codifiedValue[codifiedValueLength++] = (byte)SNMPProtocol.SnmProtocolDatatype.Integer;
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

        public static int ASN1DecodifyLength(List<byte> buffer, int bufferLength, int lengthIndex, out int lengthSize)
        {
            lengthSize = 0;

            // Returned value
            int lengthValue = 0;

            if(bufferLength > lengthIndex)
            {
                // Short form
                if (buffer[lengthIndex] < HIGH_BIT)
                {
                    lengthValue = (int)buffer[lengthIndex];
                    lengthSize = 1;
                }

                // short form with 2 bytes
                else if (buffer[lengthIndex] == 0x81)
                {
                    if (bufferLength > lengthIndex + 1)
                    {
                        lengthValue = (int)buffer[lengthIndex + 1];                        
                        lengthSize = 2;
                    }
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

        public static int ASN1DecodifyLength(byte[] buffer, int bufferLength, int lengthIndex, out int lengthSize)
        {
            lengthSize = 0;

            // Returned value
            int lengthValue = 0;

            if (bufferLength > lengthIndex)
            {
                // Short form
                if (buffer[lengthIndex] < HIGH_BIT)
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

        public static int ASN1DecodifyInteger(List<byte> buffer, int bufferLength, int integerIndex, out int integerSize)
        {
            integerSize = 0;

            // Returned value
            int integerValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object == 0x02 == Integer?
                if (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Integer)
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
                                    if ((buffer[integerIndex + integerSize + i] & HIGH_BIT) != 0)
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

        public static int ASN1DecodifyIntegerValue(List<byte> buffer, int bufferLength, int integerIndex, out int integerSize)
        {
            integerSize = 0;

            // Returned value
            int integerValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object
                if ((buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Integer) || (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Counter32) || (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.TimeTicks))
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
                            if((buffer[integerIndex + integerSize] & HIGH_BIT) != 0)
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

        public static uint ASN1DecodifyUnsignedValue(List<byte> buffer, int bufferLength, int integerIndex, out int integerSize)
        {
            integerSize = 0;

            // Returned value
            uint unsignedValue = 0;

            if (bufferLength > integerIndex)
            {
                // Type of the object
                if ((buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Integer) ||
                    (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Counter32) ||
                    (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Gauge32) ||
                    (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.TimeTicks) ||
                    (buffer[integerIndex] == (byte)SNMPProtocol.SnmProtocolDatatype.Unsigned32))
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
        
        public static byte[] ASN1DecodifyOctetString(List<byte> buffer, int bufferLength, int octetStringIndex, out int size)
        {
            byte[] octetString = null;

            int objectType = ASN1ParseDataBlock(buffer, bufferLength, octetStringIndex, out int dataBlockSize, out size);
            if (size > 0)
            {
                octetStringIndex += dataBlockSize;
                // The type of the object must be Octet string (0x04) or IP address (0x40)
                if (objectType == (byte)SnmProtocolDatatype.OctetString || objectType == (byte)SnmProtocolDatatype.IpAddress)
                {
                    octetString = new byte[size];
                    for (int i = 0; i < size; i++)
                        octetString[i] = buffer[octetStringIndex + i];
                }                
            }
            size += dataBlockSize;
            // empty string
            if (size > 0 && octetString == null)
                octetString = new byte[0];

            return octetString;
        }

        public static byte[] ASN1DecodifyIpAddress(List<byte> buffer, int bufferLength, int octetStringIndex, out int stringSize)
        {
            // Returned value
            stringSize = 0;
            byte[] octetString = null;

            if (bufferLength > octetStringIndex)
            {
                // The type of the object must be IP address (0x40)
                byte objectType = buffer[octetStringIndex];
                if (objectType == (byte)SNMPProtocol.SnmProtocolDatatype.IpAddress)
                {
                    // Get the length of the string                     
                    int objectSize = ASN1DecodifyLength(buffer, bufferLength, octetStringIndex + 1, out int lengthSize);
                    if (lengthSize > 0 && objectSize == 4) 
                    {
                        // Convert the octets of the IP Address in a string
                        string ipAddressString = String.Format("{0}.{1}.{2}.{3}",
                                                               buffer[octetStringIndex + 1 + lengthSize],
                                                               buffer[octetStringIndex + 2 + lengthSize],
                                                               buffer[octetStringIndex + 3 + lengthSize],
                                                               buffer[octetStringIndex + 4 + lengthSize]);

                        // Get the characters of the string as bytes                        
                        octetString = Encoding.ASCII.GetBytes(ipAddressString);
                        
                        stringSize = objectSize;
                    }
                }
            }

            return octetString;
        }

        public static int CountCodifiedOIDComponents(List<byte> buffer, int bufferLength, int oidIndex, int oidLength)
        {
            if(oidLength <= 0)
                return (0);

            if(bufferLength < (oidIndex + oidLength))
                return (0);

            int oidComponentsCount = 2;
            int tempOidIndex = oidIndex + 1;
            int tempOidLength = oidIndex + oidLength;
            while (tempOidIndex < tempOidLength)
            {
                int integerSize = 1;
                while((tempOidIndex + integerSize) < tempOidLength)
                {
                    if((buffer[tempOidIndex + integerSize - 1] & HIGH_BIT) != 0)
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

        public static string ASN1DecodifyOID(List<byte> buffer, int bufferLength, int oidIndex, out int dataSize)
        {
            string oidString = String.Empty;
            dataSize = 0;

            // The returned value;
            int codifiedOidSize = 0;

            // Check the object type (6 == OID)
            int tempIndex = oidIndex;
            if (bufferLength < (oidIndex + 1))
                return string.Empty;

            if (buffer[tempIndex++] != 0x06)
                return string.Empty;
            
            codifiedOidSize++;

            // Check the length of the OID
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(buffer, bufferLength, tempIndex, out int lengthSize);
            if ((lengthValue == 0) || (bufferLength < (lengthValue + tempIndex + lengthSize)))
                return string.Empty;
                        
            tempIndex += lengthSize;
            codifiedOidSize += lengthSize + lengthValue;

            // Count the number of integers that compose the OID
            int oidComponentsCount = CountCodifiedOIDComponents(buffer, bufferLength, tempIndex, lengthValue);
            if(oidComponentsCount < 2)
                return string.Empty;

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
                    if ((buffer[tempIndex + tempOidIndex + integerSize - 1] & HIGH_BIT) != 0)
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
                    if((tempIntegerValue & HIGH_BIT) != 0)
                    {
                        tempIntegerValue -= HIGH_BIT;
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

            dataSize = codifiedOidSize;

            return oidString;
        }

        public static uint ASN1GetCodifiedLengthValueLength(int intValue)
        {
            // The returned value: length of the codified value
            uint codifiedValueLength = 0;

            if(intValue < HIGH_BIT)
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

            if (intValue < HIGH_BIT)
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

        //public static uint ASN1GetCodifiedOctetStringValueLength(string valueToBeCodified)
        //{
        //    return ASN1GetCodifiedOctetStringValueLength(ASCIIEncoding.ASCII.GetBytes(valueToBeCodified), valueToBeCodified.Length);
        //}

        public static uint ASN1GetCodifiedOctetStringValueLength(byte[] valueToBeCodified, int valueByteLength = 0)
        {
            // The returned value: length of the octet string
            uint octetStringLength = 0;

            if (valueByteLength == 0)
                valueByteLength = valueToBeCodified.Length;

            //// The string must not be empty
            //if (valueByteLength > 0)
                //{
                // Type of the object (1 byte)
                octetStringLength++;

                // Length of the string (variable numbers of byte)
                octetStringLength += ASN1GetCodifiedLengthValueLength(valueByteLength);

                // Octets of the string (length of the string)
                octetStringLength += (uint)valueByteLength;
            //}

            return (octetStringLength);
        }

        public static uint ASN1GetCodifiedOctetStringValue(byte[] valueToBeCodified, int valueByteLength, SNMPDATATYPE intType, byte[] codifiedValue)
        {
            // Check the integer type
            byte codifiedType = (byte)SnmProtocolDatatype.OctetString;
            switch (intType)
            {
                case SNMPDATATYPE.OctetString:
                    codifiedType = (byte)SnmProtocolDatatype.OctetString;
                    break;

                case SNMPDATATYPE.IpAddress:
                    codifiedType = (byte)SnmProtocolDatatype.IpAddress;
                    break;

                default:
                    return (0);
            }

            // The returned value: length of the octet string
            int octetStringLength = 0;
                        
            // Type of the object (1 byte)
            codifiedValue[octetStringLength++] = codifiedType;

            // The string must not be empty
            uint codifiedStringLength = ASN1GetCodifiedLengthValueLength(valueToBeCodified.Length);
            if (valueToBeCodified.Length > 0)
            {
                // Allocate a temporary buffer for the codified length of the string
                byte[] codifiedLengthValue = new byte[codifiedStringLength];

                // Codified Length
                ASN1GetCodifiedLengthValue((uint)valueToBeCodified.Length, ref codifiedLengthValue);
                Array.Copy(codifiedLengthValue, 0, codifiedValue, octetStringLength, codifiedLengthValue.Length);
                octetStringLength += codifiedLengthValue.Length;

                Array.Copy(valueToBeCodified, 0, codifiedValue, octetStringLength, valueToBeCodified.Length);
                octetStringLength += valueToBeCodified.Length;
            }

            return (uint)octetStringLength;
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

        public static int ASN1DecodifyUsm(List<byte> buffer, int bufferLength, int integerIndex, out int size)
        {
            int initialIntegerIndex = integerIndex;
            int usmValue = ASN1ParseDataBlock(buffer, bufferLength, integerIndex, out int dataBlockSize, out size);
            if (size > 0)
            {
                integerIndex += dataBlockSize;
                bool isNegative = false;
                if ((buffer[integerIndex] & HIGH_BIT) != 0)
                {
                    isNegative = true;
                }
                if (buffer[integerIndex] == 0x80 && size > 2 && (buffer[integerIndex + 1] == 0xff && (buffer[integerIndex + 2] & 0x80) != 0))
                {
                    // this is a filler byte to comply with no 9 x consecutive 1s
                    integerIndex += 1;
                    size -= 1; // we've used one byte of the encoded length
                }
                usmValue = (isNegative ? -1 : 0);
                for (int i = 0; i < size; i++)
                {
                    usmValue <<= 8;
                    usmValue = usmValue | buffer[integerIndex++];
                }
                size += dataBlockSize;
            }

            return usmValue;
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

        public static byte ASN1ParseDataBlock(List<byte> buffer, int bufferLength, int offset, out int headerSize, out int dataSize)
        {
            headerSize = 0;
            dataSize = 0;
            if ((buffer.Count - offset) < 1)
                return 0;

            // ASN.1 type
            byte asnType = buffer[offset++];
            if ((asnType & EXTENSION_ID) == EXTENSION_ID)
                return 0;

            // length
            dataSize = ASN1ParseDataBlockLength(buffer, offset, out headerSize);            
            headerSize++;

            return asnType;
        }

        private static int ASN1ParseDataBlockLength(List<byte> buffer, int offset, out int size)
        {
            size = 0;
            int value = 0;

            if ((buffer[offset] & HIGH_BIT) == 0)
            {
                // short form encoding                
                value = buffer[offset];
                size++;                
            }
            else
            {
                int dataLen = buffer[offset + size] & ~HIGH_BIT; // store byte length of the encoded length value
                size++;                
                for (int i = 0; i < dataLen; i++)
                {
                    value <<= 8;
                    value |= buffer[offset + size];
                    size++;
                    if (offset > buffer.Count || (i < (dataLen - 1) && offset == buffer.Count))
                        throw new OverflowException("Buffer is too small !!");
                }
            }
            return value;
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
                codifiedValue[octetStringLength++] = (byte)SNMPProtocol.SnmProtocolDatatype.IpAddress;

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
            byte codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Integer;
            switch (intType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Integer;
                    break;

                case SNMPDATATYPE.Counter32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Counter32;
                    break;

                case SNMPDATATYPE.Gauge32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Gauge32;
                    break;

                case SNMPDATATYPE.TimeTicks:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.TimeTicks;
                    break;

                case SNMPDATATYPE.Unsigned32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Unsigned32;
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
                if(((unsignedValue >> (8 + (integerSize - 1))) & HIGH_BIT) != 0)
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
            byte codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Integer;
            switch (intType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Integer;
                    break;

                case SNMPDATATYPE.Counter32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Counter32;
                    break;

                case SNMPDATATYPE.Gauge32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Gauge32;
                    break;

                case SNMPDATATYPE.TimeTicks:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.TimeTicks;
                    break;

                case SNMPDATATYPE.Unsigned32:
                    codifiedIntType = (byte)SNMPProtocol.SnmProtocolDatatype.Unsigned32;
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
                if (((unsignedValue >> (8 + (integerSize - 1))) & HIGH_BIT) != 0)
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
            return (SNMPProtocol.GetMaxPacketSize() / 2);
        }

        public static SNMPCommJob.SNMPState GetLowerJobState(List<CommJob> list)
        {
            var firstJob = (from job in list
                            orderby ((SNMPCommJob)job).SnmpState ascending
                            select job).First();

            return ((SNMPCommJob)firstJob).SnmpState;
        }

        #endregion

        public static string GetErrorStatusCodeString(int err)
        {
            String str = string.Empty;
            switch (err)
            {
                case (int)SnmpErrorStatusCode.tooBig:
                    str = Properties.Resources.SNMPErrorStatusTOOBIG;
                    break;
                case (int)SnmpErrorStatusCode.NoSuchName:
                    str = Properties.Resources.SNMPErrorStatusNOSUCHNAME;
                    break;
                case (int)SnmpErrorStatusCode.BadValue:
                    str = Properties.Resources.SNMPErrorStatusBADVALUE;
                    break;
                case (int)SnmpErrorStatusCode.ReadOnly:
                    str = Properties.Resources.SNMPErrorStatusREADONLY;
                    break;
                case (int)SnmpErrorStatusCode.genError:
                    str = Properties.Resources.SNMPErrorStatusGENERR;
                    break;
                case (int)SnmpErrorStatusCode.noAccess:
                    str = Properties.Resources.SNMPErrorStatusNOACCESS;
                    break;
                case (int)SnmpErrorStatusCode.wrongType:
                    str = Properties.Resources.SNMPErrorStatusWRONGTYPE;
                    break;
                case (int)SnmpErrorStatusCode.wrongLength:
                    str = Properties.Resources.SNMPErrorStatusWRONGLENGTH;
                    break;
                case (int)SnmpErrorStatusCode.wrongEncoding:
                    str = Properties.Resources.SNMPErrorStatusWRONGENCODING;
                    break;
                case (int)SnmpErrorStatusCode.wrongValue:
                    str = Properties.Resources.SNMPErrorStatusWRONGVALUE;
                    break;
                case (int)SnmpErrorStatusCode.noCreation:
                    str = Properties.Resources.SNMPErrorStatusNOCREATION;
                    break;
                case (int)SnmpErrorStatusCode.inconsistentValue:
                    str = Properties.Resources.SNMPErrorStatusINCONSISTENTVALUE;
                    break;
                case (int)SnmpErrorStatusCode.resourceUnavailable:
                    str = Properties.Resources.SNMPErrorStatusRESOURCEUNAVAILABLE;
                    break;
                case (int)SnmpErrorStatusCode.commitFailed:
                    str = Properties.Resources.SNMPErrorStatusCOMMITFAILED;
                    break;
                case (int)SnmpErrorStatusCode.undoFailed:
                    str = Properties.Resources.SNMPErrorStatusUNDOFAILED;
                    break;
                case (int)SnmpErrorStatusCode.authorizationError:
                    str = Properties.Resources.SNMPErrorStatusAUTHORIZATIONERROR;
                    break;
                case (int)SnmpErrorStatusCode.notWritable:
                    str = Properties.Resources.SNMPErrorStatusNOTWRITABLE;
                    break;
                case (int)SnmpErrorStatusCode.inconsistentName:
                    str = Properties.Resources.SNMPErrorStatusINCONSISTENTNAME;
                    break;
                default:
                    str = Properties.Resources.SNMPErrorStatusUNKNOWN;
                    break;
            }
            return str;
        }

        public static string GetErrorBindingVariablesString(int err)
        {
            String str = string.Empty;
            switch (err)
            {
                case (int)SnmpErrorVariableBindings.ErrornoSuchObject:
                    str = Properties.Resources.SNMPErrorNoSuchObject;
                    break;
                case (int)SnmpErrorVariableBindings.ErrornoSuchInstance:
                    str = Properties.Resources.SNMPErrorNoSuchInstance;
                    break;
                case (int)SnmpErrorVariableBindings.ErrorendOfMIBView:
                    str = Properties.Resources.SNMPErrorendOfMIBView;
                    break;
            }

            return str;
        }

        public static bool CompatibleDataTypes(SNMPDATATYPE dataType1, SNMPDATATYPE dataType2)
        {
            // Returned value
            bool dataTypesAreCompatible = false;

            if (dataType1 == dataType2)
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

        public static byte[] ParseIntegerData(List<byte> buffer, int bufferLength, int dataIndex, out int dataSize)
        {
            byte[] dataBuffer = null;

            int integerValue = ASN1DecodifyIntegerValue(buffer, bufferLength, dataIndex, out dataSize);
            if (dataSize > 0)
                dataBuffer = BitConverter.GetBytes(integerValue);

            return dataBuffer;
        }

        public static byte[] ParseUnsignedData(List<byte> buffer, int bufferLength, int dataIndex, out int dataSize)
        {
            byte[] dataBuffer = null;

            uint unsignedValue = ASN1DecodifyUnsignedValue(buffer, bufferLength, dataIndex, out dataSize);
            if (dataSize > 0)
                dataBuffer = BitConverter.GetBytes(unsignedValue);

            return dataBuffer;
        }
                
        public static byte[] ParseIpAddressData(List<byte> buffer, int bufferLength, int dataIndex, out int dataSize)
        {
            // Returned value
            byte[] dataBuffer = ASN1DecodifyIpAddress(buffer, bufferLength, dataIndex, out dataSize);
            if (dataSize == 0)
                dataBuffer = null;

            return dataBuffer;
        }

        public static void ASN1InsertHeader(byte headerCode, List<byte> dataBuffer, int len)
        {
            SNMPProtocol.ASN1BuildLength(len, out byte[] buf);

            List<byte> header = new List<byte>();
            header.Add(headerCode);
            header.AddRange(buf);

            dataBuffer.InsertRange(0, header);
        }

        public static void ASN1AddHeader(byte headerCode, List<byte> dataBuffer, int len)
        {            
            SNMPProtocol.ASN1BuildLength(len, out byte[] buf);

            dataBuffer.Add(headerCode);
            dataBuffer.AddRange(buf);
        }

        private static void ASN1BuildLength(int length, out byte[] dataBuffer)
        {
            List<byte> resultBuffer = new List<byte>();
            List<byte> buffer = new List<byte>();
            byte[] len = BitConverter.GetBytes(length);            
            for (int i = 3; i >= 0; i--)
            {
                if (len[i] != 0 || buffer.Count > 0)
                    buffer.Add(len[i]);
            }
            if (buffer.Count == 0)
                buffer.Add(0);

            // 0x80 --> high byte
            // short encoding
            if (buffer.Count == 1 && (buffer[0] & (byte)HIGH_BIT) == 0)
            {
                resultBuffer.AddRange(buffer); // done
            }
            else
            {
                // long encoding                
                resultBuffer.Add((byte)((byte)buffer.Count | (byte)HIGH_BIT));
                resultBuffer.AddRange(buffer);
            }

            dataBuffer = resultBuffer.ToArray();
        }

        public static byte[] ParseJobData(List<byte> buffer, int bufferLength, int dataIndex, out int dataSize, out int variableSize)
        {
            byte[] dataBuffer = null;
            // Returned value
            dataSize = 0;
            variableSize = 0;

            if (bufferLength > dataIndex)
            {
                byte dataType = buffer[dataIndex];
                switch (dataType)
                {
                    case (byte)SnmProtocolDatatype.Integer: // Integer/Integer32
                        dataBuffer = ParseIntegerData(buffer, bufferLength, dataIndex, out dataSize);
                        break;

                    case (byte)SnmProtocolDatatype.Counter32: // Counter32
                    case (byte)SnmProtocolDatatype.Gauge32: // Gauge32
                    case (byte)SnmProtocolDatatype.TimeTicks: // TimeTicks
                    case (byte)SnmProtocolDatatype.Unsigned32: // Unsigned32
                        dataBuffer = ParseUnsignedData(buffer, bufferLength, dataIndex, out dataSize);
                        break;

                    case (byte)SnmProtocolDatatype.OctetString: // Octet string
                        dataBuffer = ASN1DecodifyOctetString(buffer, bufferLength, dataIndex, out dataSize);
                        break;

                    case (byte)SnmProtocolDatatype.IpAddress: // IP address
                        dataBuffer = ParseIpAddressData(buffer, bufferLength, dataIndex, out dataSize);
                        break;
                }
            }

            if (dataSize > 0)
                variableSize = dataBuffer.Length;

            return dataBuffer;
        }

        public static bool ParseData(byte[] receivedbuffer, ref SNMPCommJob j, ref List<object> items)
        {
            List<byte> receiveBuffer = receivedbuffer.ToList();

            bool areArguments = (items.Count > 0);
            if (areArguments)
            {
                BuiltInType bt = j.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger && bt != BuiltInType.String)
                {
                    return false;
                }
                               
                if (items.Count < 2)
                {
                    items[0] = 11;
                    return false;
                }                
            }

            //// Parse data
            //int parsedBytes = 0;
            //byte[] jobData = ParseJobData(receiveBuffer, receiveBuffer.Count, parsedBytes, out int dataSize);
            
            // Copy parsed data
            List<Tag> changed = new List<Tag>();
            j.SetJobData(receivedbuffer, ref changed);

            if (areArguments)
            {
                if (j.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < j.TagsList.Count; k++)
                    {
                        items[k + 1] = j.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }


            return true;
        }

        public static DriverErrorCodes ParseCommunityResponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {            
            int tempParsedBytes = parsedBytes;
            // Check the type of the following object: 0x04 = octet string
            if ((receiveLength < (tempParsedBytes + 1)) || (receiveBuffer[tempParsedBytes++] != (byte)SnmProtocolDatatype.OctetString))
            {
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            }

            // Check the length of the community string
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(receiveBuffer, receiveLength, tempParsedBytes, out int lengthSize);
            if ((lengthValue == 0) || (receiveLength < (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            replyMainData.Community = Encoding.ASCII.GetString(receiveBuffer.Skip(tempParsedBytes).Take(lengthValue).ToArray());

            parsedBytes = tempParsedBytes + lengthValue;

            return (DriverErrorCodes.ErrorNoError);
        }

        public static DriverErrorCodes ParseHeaderResponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {
            replyMainData.snmpVersion = SNMPVERSION.SNMPv1;
            parsedBytes = 0;
            
            // Check the first byte of the message
            if (receiveLength < 1)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);

            if (receiveBuffer[parsedBytes++] != SEQUENCE)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);

            // Check the length of the message
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(receiveBuffer, receiveLength, parsedBytes, out int valueSize);
            if ((valueSize == 0) || (receiveLength != (lengthValue + parsedBytes + valueSize)))
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            
            parsedBytes += valueSize;
            
            uint protocol = ASN1DecodifyUnsignedValue(receiveBuffer, receiveBuffer.Count, parsedBytes, out valueSize);
            if (valueSize > 0)
            {
                switch (protocol)
                {
                    case (byte)SNMPVERSION.SNMPv1:
                        replyMainData.snmpVersion = SNMPVERSION.SNMPv1;
                        break;
                    case (byte)SNMPVERSION.SNMPv2c:
                        replyMainData.snmpVersion = SNMPVERSION.SNMPv2c;
                        break;
                    case (byte)SNMPVERSION.SNMPv3:
                        replyMainData.snmpVersion = SNMPVERSION.SNMPv3;
                        break;
                    default:
                        return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUnsupportedVersion);
                }
            }
            else
            {
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUnsupportedVersion);
            }
            parsedBytes += valueSize;

            return (DriverErrorCodes.ErrorNoError);
        }

        public static DriverErrorCodes ParsePduTypeReponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {
            replyMainData.ReplyBufferIndex = parsedBytes;
            replyMainData.PduType = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize, out int valueSize);
            if (valueSize == 0)
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyType;
            parsedBytes += dataBlockSize;

            return (DriverErrorCodes.ErrorNoError);
        }

        public static DriverErrorCodes ParseIDReponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {
            // reply ID
            replyMainData.MsgID = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, parsedBytes, out int integerSize);
            if (integerSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += integerSize;

            return DriverErrorCodes.ErrorNoError;
        }

        public static DriverErrorCodes ParseErrorStatusReponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {
            int tempParsedBytes = parsedBytes;

            // Parse the error status
            int integerValue1 = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, tempParsedBytes, out int integerSize);
            if (integerSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += integerSize;

            // Parse the error index
            int integerValue2 = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, tempParsedBytes, out integerSize);
            if (integerSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += integerSize;
                    
            // Set the output parameters
            replyMainData.ErrorStatus = integerValue1;
            replyMainData.ErrorIndex = integerValue2;

            return (DriverErrorCodes.ErrorNoError);
        }

                

        public static SNMPProtocol.SNMPVariableBinding ParseReplyDiscoveryDataVb(List<byte> receiveBuffer, int receiveLength, ref int internalParsedBytes, out int vbSize)
        {
            SNMPProtocol.SNMPVariableBinding vb = null;
            vbSize = 0;

            int parsedBytes = internalParsedBytes;
            byte type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize, out int dbSize);
            if (dbSize != 0 && type == SNMPProtocol.SEQUENCE)
            {
                parsedBytes += dataBlockSize;
                // Get the number of bytes requested for the codified OID
                string oidAddress = SNMPProtocol.ASN1DecodifyOID(receiveBuffer, receiveLength, parsedBytes, out int oidSize);
                if (oidSize != 0)
                {
                    parsedBytes += oidSize;
                    type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int headerSize, out int dataSize);
                    if (type != 0)
                    {
                        vb = new SNMPProtocol.SNMPVariableBinding();
                        vb.oID = new SNMPOid(oidAddress);
                        vb.ValueType = type;
                        vb.ErrorCode = SNMPProtocol.GetErrorCodeFromDataSNMPDataType(type);
                        if (vb.ErrorCode == DriverErrorCodes.ErrorNoError)
                        {
                            vb.snmpDataType = SNMPProtocol.ConvertSNMPDataTypeToTagType(type);
                            vb.Value = receiveBuffer.Skip(parsedBytes + headerSize).Take(dataSize).ToArray();
                            List<byte> typeAndValue = receiveBuffer.Skip(parsedBytes).Take(headerSize + dataSize).ToList();
                            vb.TagValue = ParseJobData(typeAndValue, typeAndValue.Count, 0, out int tagSize, out int variableSize);
                            vb.snmpDataSize = variableSize;
                        }
                        vbSize = dbSize;
                    }
                }
            }

            return vb;
        }

        public static DriverErrorCodes ParseDataVbsResponse(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            byte type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize, out int dataSize);
            // no data in not a framing error
            if (dataSize > 0)
            {
                if (type != SNMPProtocol.SEQUENCE)
                    return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
                                
                int totalDataBlockSize = parsedBytes + (dataBlockSize + dataSize);

                parsedBytes += dataBlockSize;
                while (parsedBytes < totalDataBlockSize)
                {
                    type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out dataBlockSize, out dataSize);
                    if (dataSize == 0 || type != SNMPProtocol.SEQUENCE)
                        return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);

                    SNMPProtocol.SNMPVariableBinding variableBindings = ParseReplyDiscoveryDataVb(receiveBuffer, receiveLength, ref parsedBytes, out int vbSize);
                    if (vbSize != 0)
                    {
                        messageGeneralData.VariableBindings.Add(variableBindings);
                        parsedBytes += (dataBlockSize + vbSize);
                    }
                    else
                    {
                        return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
                    }
                }
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public static DriverErrorCodes GetErrorCodeFromDataSNMPDataType(byte snmpType)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            switch (snmpType)
            {
                case (byte)SnmpErrorVariableBindings.ErrornoSuchInstance:
                    result = (DriverErrorCodes)(SNMPErrorNonZeroErrorBindVariables + SNMPProtocol.SnmpErrorVariableBindings.ErrornoSuchInstance);
                    break;
                case (byte)SnmpErrorVariableBindings.ErrornoSuchObject:
                    result = (DriverErrorCodes)(SNMPErrorNonZeroErrorBindVariables + SNMPProtocol.SnmpErrorVariableBindings.ErrornoSuchObject);
                    break;
                case (byte)SnmpErrorVariableBindings.ErrorendOfMIBView:
                    result = (DriverErrorCodes)(SNMPErrorNonZeroErrorBindVariables + SNMPProtocol.SnmpErrorVariableBindings.ErrorendOfMIBView);
                    break;
            }

            return result;
        }

        public static SNMPDATATYPE ConvertSNMPDataTypeToTagType(byte snmpType)
        {
            switch (snmpType)
            {
                case (byte)SnmProtocolDatatype.Integer:
                    return SNMPDATATYPE.Integer;
                case (byte)SnmProtocolDatatype.OctetString:
                    return SNMPDATATYPE.OctetString;
                case (byte)SnmProtocolDatatype.IpAddress:
                    return SNMPDATATYPE.IpAddress;
                case (byte)SnmProtocolDatatype.Counter32:
                    return SNMPDATATYPE.Counter32;
                case (byte)SnmProtocolDatatype.Gauge32:
                    return SNMPDATATYPE.Gauge32;
                case (byte)SnmProtocolDatatype.TimeTicks:
                    return SNMPDATATYPE.TimeTicks;
                case (byte)SnmProtocolDatatype.Unsigned32:
                    return SNMPDATATYPE.Unsigned32;
                default:
                    return SNMPDATATYPE.Unknown;
            }
        }

        public static DriverErrorCodes ParseBaseResponse(List<byte> receiveBuffer, List<SNMPStation> listSt, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            int receiveLength = receiveBuffer.Count;
            int parsedBytes = 0;

            DriverErrorCodes errorCode = SNMPProtocol.ParseHeaderResponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                switch (messageGeneralData.snmpVersion)
                {
                    case SNMPVERSION.SNMPv1:
                    case SNMPVERSION.SNMPv2c:                        
                        errorCode = SNMPProtocol.ParseCommunityResponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                        if (errorCode == DriverErrorCodes.ErrorNoError)
                        {
                            errorCode = SNMPProtocol.ParsePduTypeReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                            if (errorCode == DriverErrorCodes.ErrorNoError)
                            {
                                errorCode = SNMPProtocol.ParseIDReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                                if (errorCode == DriverErrorCodes.ErrorNoError)
                                {
                                    errorCode = SNMPProtocol.ParseErrorStatusReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                                    if (errorCode == DriverErrorCodes.ErrorNoError)
                                        errorCode = SNMPProtocol.ParseDataVbsResponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                                }
                            }
                        }
                        break;

                    case SNMPVERSION.SNMPv3:

                        errorCode = SNMPProtocol.ParseGlobalDataResponsev3(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                        if (errorCode == DriverErrorCodes.ErrorNoError)
                        {                           
                            errorCode = SNMPProtocol.ParseUserSecuriyModelResponsev3(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                            if (errorCode == DriverErrorCodes.ErrorNoError)
                            {
                                bool pduResponseParsed = false;
                                // no security check required
                                if (listSt != null)
                                {                                    
                                    if (messageGeneralData.ParsedUsm.HasValidEngineId())
                                    {
                                        // trap inform message ?
                                        if (!messageGeneralData.ParsedUsm.IsUserNameEmpty())
                                        {                                         
                                            List<SNMPStation> compatibleStations = listSt.FindAll(st => st.Usm != null && st.Usm.IsCompatibleAutenticationAndPrivacy(messageGeneralData.ParsedUsm.UserName, messageGeneralData.FlagAuthentication, messageGeneralData.FlagPrivacy));
                                            if (compatibleStations.Count > 0)
                                            {
                                                foreach (SNMPStation station in compatibleStations)
                                                {
                                                    // use data from trap to calculate every time the encryption parameters and privacy
                                                    if (messageGeneralData.IsMessageFromTrap)
                                                    {
                                                        station.UsmTrap.ResetEngine();
                                                        station.UsmTrap.ResetAutenticationAndPrivacyKey();
                                                        station.UsmTrap.SetEngine(messageGeneralData.ParsedUsm);                                                        
                                                    }

                                                    if (errorCode == DriverErrorCodes.ErrorNoError && messageGeneralData.FlagAuthentication)
                                                        errorCode = station.VerifyAutenticationParameters(receiveBuffer.ToArray(), messageGeneralData);

                                                    if (errorCode == DriverErrorCodes.ErrorNoError && messageGeneralData.FlagPrivacy)
                                                    {
                                                        pduResponseParsed = true;
                                                        errorCode = station.VerifyPrivacyParameters(receiveBuffer, receiveLength, parsedBytes, messageGeneralData);
                                                    }

                                                    if (errorCode == DriverErrorCodes.ErrorNoError)
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid;
                                    }
                                }

                                if (errorCode == DriverErrorCodes.ErrorNoError && !pduResponseParsed)
                                    errorCode = SNMPProtocol.ParsePduResponsev3(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);                                
                            }                            
                        }
                        break;
                }
            }

            return errorCode;
        }

        public static DriverErrorCodes GetErrorReplyTypeMismatch(byte responseReplyType, byte expectedReplyType)
        {
            if (expectedReplyType == (byte)SnmpPduType.Response && responseReplyType == (byte)SnmpPduType.Report)
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid;
            else
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyType;
        }

        public static string SNMPByteMessageToHexString(byte[] message)
        {
            return ("0x" + string.Join(",0x", message.Select(b => b.ToString("X2"))));
        }
    }
}