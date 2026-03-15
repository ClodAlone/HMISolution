using System;
using DriverCodeBaseEx.Enumerators;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnmpSharpNet;

namespace SNMP
{
    public partial class SNMPProtocol
    {
        public class SNMPUSM
        {
            public byte Type { get; set; }
            public SNMPProtocol.SecurityLevel SecurityLevel { get; set; }
            public byte[] EngineId { get; set; }

            private DateTime _EngineTimeDTStart;
            private Int32 _EngineTime;
            public Int32 EngineTime
            {
                get { return _EngineTime; }
                set
                {
                    _EngineTime = value;
                    _EngineTimeDTStart = DateTime.UtcNow;
                }
            }
            public Int32 EngineBoots { get; set; }

            public byte[] ContextName { get; set; }
            public byte[] UserName { get; set; }

            public AuthenticationProtocol AuthenticationProtocol { get; set; }
            public byte[] AuthenticationPassword { get; set; }

            public PrivacyEncryptionProtocol PrivacyEncryptionProtocol { get; set; }
            public byte[] PrivacyEncryptionPassword { get; set; }
            
            public byte[] PrivacyKey { get; set; }
            public byte[] AuthenticationKey { get; set; }
            public byte[] AuthenticationParameters { get; set; }
            public byte[] PrivacyEncryptionParameters { get; set; }
            public IAuthenticationDigest IAuthProtocol { get; set; }
            public IPrivacyProtocol IPrivacyProtocol { get; set; }

            public SNMPUSM()
            {
                Type = SNMP_SECURITY_MODEL_USM;
                SecurityLevel = SecurityLevel.NoAuthNoPriv;
                EngineId = new byte[0];
                _EngineTimeDTStart = DateTime.UtcNow;
                EngineTime = 0;
                EngineBoots = 0;
                ContextName = new byte[0];
                UserName = new byte[0];
                AuthenticationProtocol = AuthenticationProtocol.None;
                AuthenticationPassword = new byte[0];
                // used only when parsing message
                AuthenticationParameters = new byte[0];

                PrivacyEncryptionProtocol = PrivacyEncryptionProtocol.None;
                PrivacyEncryptionPassword = new byte[0];
                PrivacyEncryptionParameters = new byte[0];

                PrivacyKey = new byte[0];
                AuthenticationKey = new byte[0];
                IAuthProtocol = null;
                IPrivacyProtocol = null;
            }

            public byte[] GetAgentId()
            {
                if (EngineId.Length < 12)
                    return new byte[0];
                else
                    return EngineId.Skip(4).Take(8).ToArray();
            }

            public byte[] EnterpriseId()
            {
                if (EngineId.Length < 12)
                    return new byte[0];
                else
                    return EngineId.Take(4).ToArray();
            }

            public bool HasValidEngineId()
            {
                return (EngineId != new byte[0]);
            }

            public bool HasValidBootsTime()
            {
                return (EngineBoots != 0 || EngineTime != 0);
            }

            public bool HasValidEngineIdBootsTime()
            {
                return (HasValidEngineId() && HasValidBootsTime());
            }

            public static void InitAuthenticationParameters(ref byte[] authenticationParameters)
            {
                if (authenticationParameters == null || authenticationParameters.Length == 0)
                    authenticationParameters = new byte[SNMPProtocol.SNMP_USM_AUTHENTICATIONPARAMETERS_SIZE]; // { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            }

            public static void InitPrivacyParameters(IPrivacyProtocol privacyProtocol, ref byte[] privacyParameters)
            {
                if (privacyParameters == null || privacyParameters.Length == 0)
                    privacyParameters = new byte[privacyProtocol.PrivacyParametersLength];
            }

            public bool IsUserNameEmpty()
            {
                return (UserName == null || UserName.Length == 0);
            }

            public string GetUserNameString()
            {
                return Encoding.ASCII.GetString(UserName);
            }

            public void SetUserName(string userName)
            {
                UserName = Encoding.ASCII.GetBytes(userName);
            }

            public void SetContextName(string contextName)
            {
                ContextName = Encoding.ASCII.GetBytes(contextName);
            }

            public void SetAuthenticationPassword(string authenticationPassword)
            {
                AuthenticationPassword = Encoding.ASCII.GetBytes(authenticationPassword);
            }

            public void SetPrivacyEncryptionPassword(string privacyEncryptionPassword)
            {
                PrivacyEncryptionPassword = Encoding.ASCII.GetBytes(privacyEncryptionPassword);
            }

            public bool IsValidAuthenticationKey()
            {
                return AuthenticationKey.Length != 0;
            }

            public bool IsValidPrivacyKey()
            {
                return PrivacyKey.Length != 0;
            }

            public void SetPrivacy(IPrivacyProtocol privacyProtocol, byte[] privacyKey = null)
            {
                IPrivacyProtocol = privacyProtocol;
                if (privacyKey != null)
                {
                    PrivacyKey = new byte[privacyKey.Length];
                    Array.Copy(privacyKey, PrivacyKey, privacyKey.Length);
                }
            }

            public void SetAuthentication(IAuthenticationDigest authProtocol, byte[] authenticationKey)
            {
                IAuthProtocol = authProtocol;
                if (authenticationKey.Length > 0)
                {
                    AuthenticationKey = new byte[authenticationKey.Length];
                    Array.Copy(authenticationKey, AuthenticationKey, authenticationKey.Length);
                }
            }

            public Int32 GetNextEngineTime()
            {
                if (EngineTime == 0)
                    return 0;
                else
                    return (EngineTime + (Int32)DateTime.UtcNow.Subtract(_EngineTimeDTStart).TotalSeconds);
            }

            public void SetEngine(SNMPUSM usm)
            {
                EngineId = usm.EngineId;
                EngineTime = usm.EngineTime;
                EngineBoots = usm.EngineBoots;
            }

            public void ResetEngine()
            {
                EngineId = new byte[0];
                _EngineTimeDTStart = DateTime.UtcNow;
                EngineTime = 0;
                EngineBoots = 0;
            }

            public void ResetAutenticationAndPrivacyKey()
            {
                AuthenticationKey = new byte[0];
                PrivacyKey = new byte[0];
            }

            public bool IsCompatibleAutenticationAndPrivacy(byte[] userName, bool flagAuthentication, bool flagPrivacy)
            {
                if (!UserName.SequenceEqual(userName))
                    return false;

                if (flagAuthentication)
                {
                    if (!(this.SecurityLevel == SecurityLevel.AuthNoPriv || this.SecurityLevel == SecurityLevel.AuthPriv))
                        return false;
                }

                if (flagPrivacy)
                {
                    if (!(this.SecurityLevel == SecurityLevel.AuthPriv))
                        return false;
                }

                return true;
            }
        }

        public enum SecurityLevel : byte
        {
            NoAuthNoPriv = 0,   // no authentication and no privacy
            AuthNoPriv,         // authentication but no privacy
            AuthPriv,           // authentication and privacy
        }

        [Flags]
        public enum SecurityFlags : byte
        {
            // packet has been authenticated
            Authenticated = 1,
            // packet has been privacy protected
            Privacy = 2,
            //sender of the packet expects report packet to be sent by the agent on error
            Reportable = 4
        }

        public enum AuthenticationProtocol : byte
        {
            None = 0,
            MD5,
            SHA1
        }

        public enum PrivacyEncryptionProtocol : byte
        {
            None = 0,
            Des
        }

        public const int SNMP_USM_AUTHENTICATIONPARAMETERS_SIZE = 12;

        public static DriverErrorCodes ParseGlobalDataResponsev3(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData replyMainData)
        {
            if (receiveBuffer[parsedBytes++] != SNMPProtocol.SEQUENCE)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);

            // Check the length of the message
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(receiveBuffer, receiveLength, parsedBytes, out int valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // Check the reply ID                        
            int id = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // MaxMessageSize
            replyMainData.MaxPacket = (uint)SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            byte[] flags = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            if (flags.Length > 0)
            {
                if ((flags[0] & (byte)SNMPProtocol.SecurityFlags.Authenticated) != 0)
                    replyMainData.FlagAuthentication = true;
                if ((flags[0] & (byte)SNMPProtocol.SecurityFlags.Privacy) != 0)
                    replyMainData.FlagPrivacy = true;
                if ((flags[0] & (byte)SNMPProtocol.SecurityFlags.Reportable) != 0)
                    replyMainData.FlagReportable = true;
            }

            // security model
            replyMainData.ParsedUsm.Type = (byte)SNMPProtocol.ASN1DecodifyUsm(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            return (DriverErrorCodes.ErrorNoError);
        }

        public static DriverErrorCodes ParseUserSecuriyModelResponsev3(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            byte type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize, out int valueSize);
            if (type != (byte)SNMPProtocol.SnmProtocolDatatype.OctetString)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += dataBlockSize;

            // sequence
            type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize2, out valueSize);
            if (type != SNMPProtocol.SEQUENCE)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += dataBlockSize2;

            // engine ID
            messageGeneralData.ParsedUsm.EngineId = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // engine Boot
            messageGeneralData.ParsedUsm.EngineBoots = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // engine Time
            messageGeneralData.ParsedUsm.EngineTime = SNMPProtocol.ASN1DecodifyInteger(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // Security Name
            messageGeneralData.ParsedUsm.UserName = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            //System.Diagnostics.Debug.WriteLine(SNMPProtocol.SNMPByteMessageToHexString(receiveBuffer.ToArray()));

            int parsedBytesAuthenticationParameters = parsedBytes;
            // Autentication parameters
            messageGeneralData.ParsedUsm.AuthenticationParameters = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            if (messageGeneralData.ParsedUsm.AuthenticationParameters.Length > 0)
            {
                parsedBytesAuthenticationParameters += 2; // Skip BER encoded variable type and length
                for (int i = 0; i < messageGeneralData.ParsedUsm.AuthenticationParameters.Length; i++)
                    receiveBuffer[parsedBytesAuthenticationParameters + i] = 0x00;
            }

            //Privacy EncryptionParameters parameters
            messageGeneralData.ParsedUsm.PrivacyEncryptionParameters = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            return (DriverErrorCodes.ErrorNoError);
        }

        public static DriverErrorCodes ParsePlainTextResponsev3(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            // sequence
            byte type = SNMPProtocol.ASN1ParseDataBlock(receiveBuffer, receiveLength, parsedBytes, out int dataBlockSize2, out int valueSize);
            if (type != SNMPProtocol.SEQUENCE)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += dataBlockSize2;

            // engine ID
            byte[] engineId = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            // Context name
            messageGeneralData.ParsedUsm.ContextName = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out valueSize);
            if (valueSize == 0)
                return ((DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply);
            parsedBytes += valueSize;

            return DriverErrorCodes.ErrorNoError;
        }

        public static DriverErrorCodes ParsePduResponsev3(List<byte> receiveBuffer, int receiveLength, ref int parsedBytes, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            DriverErrorCodes errorCode = SNMPProtocol.ParsePlainTextResponsev3(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                errorCode = SNMPProtocol.ParsePduTypeReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                if (errorCode == DriverErrorCodes.ErrorNoError)
                {
                    errorCode = SNMPProtocol.ParseIDReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                    if (errorCode == DriverErrorCodes.ErrorNoError)
                    {
                        // Check the error-status and the error-index                                            
                        errorCode = SNMPProtocol.ParseErrorStatusReponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                        if (errorCode == DriverErrorCodes.ErrorNoError)
                            errorCode = SNMPProtocol.ParseDataVbsResponse(receiveBuffer, receiveLength, ref parsedBytes, messageGeneralData);
                    }
                }
            }
            return errorCode;
        }

        private static void PrepareHeaderRequestv3(SNMPStation st, List<byte> requestBuffer, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            List<byte> resultBuffer = new List<byte>();
            uint len;
            byte[] buf;

            // message id value
            Random rndGen = new Random(Guid.NewGuid().GetHashCode());
            Int32 msgID = rndGen.Next(1, Int32.MaxValue);
            len = SNMPProtocol.ASN1GetCodifiedIntLength(msgID);
            SNMPProtocol.ASN1CodifyInt(msgID, out buf, len);
            resultBuffer.AddRange(buf);

            // MaxMessageSize
            len = SNMPProtocol.ASN1GetCodifiedIntLength((int)messageGeneralData.MaxPacket);
            SNMPProtocol.ASN1CodifyInt((int)messageGeneralData.MaxPacket, out buf, len);
            resultBuffer.AddRange(buf);

            // flags
            SNMPProtocol.SecurityFlags flag = 0;
            if (messageGeneralData.FlagAuthentication)
                flag |= SNMPProtocol.SecurityFlags.Authenticated;
            if (messageGeneralData.FlagPrivacy)
                flag |= SNMPProtocol.SecurityFlags.Privacy;
            if (messageGeneralData.FlagReportable)
                flag |= SNMPProtocol.SecurityFlags.Reportable;
            byte[] flagValue = new byte[] { (byte)flag };
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(flagValue, flagValue.Length);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(flagValue, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            // security model
            len = SNMPProtocol.ASN1GetCodifiedIntLength(st.Usm.Type);
            SNMPProtocol.ASN1CodifyInt(st.Usm.Type, out buf, len);
            resultBuffer.AddRange(buf);

            // start sequence + nr bytes
            SNMPProtocol.ASN1AddHeader(SNMPProtocol.SEQUENCE, requestBuffer, resultBuffer.Count);

            requestBuffer.AddRange(resultBuffer);
        }

        private static void PrepareUserSecurityModelRequestv3(SNMPStation st, List<byte> dataBuffer, ref byte[] authenticationParameters, ref byte[] privacyParameters, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            List<byte> resultBuffer = new List<byte>();
            uint len;
            byte[] buf;

            // Engine Id
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(st.Usm.EngineId);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(st.Usm.EngineId, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            // Engine Boots
            len = SNMPProtocol.ASN1GetCodifiedIntLength(st.Usm.EngineBoots);
            SNMPProtocol.ASN1CodifyInt((int)st.Usm.EngineBoots, out buf, len);
            resultBuffer.AddRange(buf);

            // Engine Time
            len = SNMPProtocol.ASN1GetCodifiedIntLength(st.Usm.EngineTime);
            SNMPProtocol.ASN1CodifyInt((int)st.Usm.EngineTime, out buf, len);
            resultBuffer.AddRange(buf);

            // User Name
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(st.Usm.UserName);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(st.Usm.UserName, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            if (messageGeneralData.FlagAuthentication)
                SNMPProtocol.SNMPUSM.InitAuthenticationParameters(ref authenticationParameters);

            // Autentication parameters
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(authenticationParameters);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(authenticationParameters, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            if (messageGeneralData.FlagPrivacy)
                SNMPProtocol.SNMPUSM.InitPrivacyParameters(st.Usm.IPrivacyProtocol, ref privacyParameters);

            // Privacy parameters
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(privacyParameters);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(privacyParameters, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            // start sequence + nr bytes
            SNMPProtocol.ASN1InsertHeader(SNMPProtocol.SEQUENCE, resultBuffer, resultBuffer.Count);

            // start sequence + nr bytes
            SNMPProtocol.ASN1AddHeader((byte)SNMPProtocol.SnmProtocolDatatype.OctetString, dataBuffer, resultBuffer.Count);
            dataBuffer.AddRange(resultBuffer);
        }

        private static void PreparePduRequestv3(SNMPStation st, List<byte> requestBuffer, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            uint len;
            byte[] buf;

            List<byte> resultBuffer = new List<byte>();
            // Engine Id
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(st.Usm.EngineId);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(st.Usm.EngineId, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            // Context Name
            len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(st.Usm.ContextName);
            buf = new byte[len];
            SNMPProtocol.ASN1GetCodifiedOctetStringValue(st.Usm.ContextName, (int)len, SNMPDATATYPE.OctetString, buf);
            resultBuffer.AddRange(buf);

            List<byte> resultBufferInternal = new List<byte>();
            // request ID
            len = SNMPProtocol.ASN1GetCodifiedIntLength(messageGeneralData.MsgID);
            SNMPProtocol.ASN1CodifyInt(messageGeneralData.MsgID, out buf, len);
            resultBufferInternal.AddRange(buf);

            //error state
            len = SNMPProtocol.ASN1GetCodifiedIntLength(0);
            SNMPProtocol.ASN1CodifyInt(0, out buf, len);
            resultBufferInternal.AddRange(buf);

            //error index
            len = SNMPProtocol.ASN1GetCodifiedIntLength(0);
            SNMPProtocol.ASN1CodifyInt(0, out buf, len);
            resultBufferInternal.AddRange(buf);

            //variable bindings
            List<byte> resultBufferVariables = new List<byte>();
            foreach (SNMPProtocol.SNMPVariableBinding variable in messageGeneralData.VariableBindings)
            {
                List<byte> variableBuffer = new List<byte>();
                variableBuffer.AddRange(variable.oID.codifiedOid);

                // write
                if (variable.ValueToWrite != null)
                {
                    variableBuffer.AddRange(variable.ValueToWrite);
                }
                else
                {
                    // read --> set Null value
                    variableBuffer.Add((byte)SNMPProtocol.SnmProtocolDatatype.Null);
                    variableBuffer.Add(0);
                }

                SNMPProtocol.ASN1InsertHeader(SNMPProtocol.SEQUENCE, variableBuffer, variableBuffer.Count);

                resultBufferVariables.AddRange(variableBuffer);
            }

            SNMPProtocol.ASN1AddHeader(SNMPProtocol.SEQUENCE, resultBufferInternal, resultBufferVariables.Count);
            resultBufferInternal.AddRange(resultBufferVariables);
                                    
            // start sequence + nr bytes
            SNMPProtocol.ASN1AddHeader(messageGeneralData.PduType, resultBuffer, resultBufferInternal.Count);
            resultBuffer.AddRange(resultBufferInternal);

            // start sequence + nr bytes
            SNMPProtocol.ASN1InsertHeader(SNMPProtocol.SEQUENCE, resultBuffer, resultBuffer.Count);
            requestBuffer.AddRange(resultBuffer);
        }

        public static DriverErrorCodes PreparePduCryptedWithPrivacyKeyRequestv3(SNMPStation st, List<byte> requestBuffer, ref byte[] privacyParameters, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            bool result = false;
            DriverErrorCodes errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;

            try
            {
                result = true;
                List<byte> resultBufferPdu = new List<byte>();
                PreparePduRequestv3(st, resultBufferPdu, messageGeneralData);

                byte[] encryptedBuffer = st.Usm.IPrivacyProtocol.Encrypt(resultBufferPdu.ToArray(), 0, resultBufferPdu.Count, st.Usm.PrivacyKey, st.Usm.EngineBoots, st.Usm.EngineTime, out privacyParameters, st.Usm.IAuthProtocol);

                uint len = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(encryptedBuffer);
                byte[] buf = new byte[len];
                SNMPProtocol.ASN1GetCodifiedOctetStringValue(encryptedBuffer, (int)len, SNMPDATATYPE.OctetString, buf);

                requestBuffer.AddRange(buf);
            }
            catch (Exception ex)
            {
                // catch SnmpSharpNet error
                result = false;
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                if (ex is SnmpSharpNet.SnmpException || ex is SnmpSharpNet.SnmpErrorStatusException)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;
            }

            return (result ? DriverErrorCodes.ErrorNoError : errorCode);
        }


        private static void PrepareProtocolVersionRequestv3(List<byte> requestBuffer, SNMPProtocol.SNMPMessageGeneralData requestMainData)
        {
            uint len;
            byte[] buf;

            // protocol version 
            len = SNMPProtocol.ASN1GetCodifiedIntLength((int)requestMainData.snmpVersion);
            SNMPProtocol.ASN1CodifyInt((int)requestMainData.snmpVersion, out buf, len);
            requestBuffer.InsertRange(0, buf);

            // start sequence + nr bytes
            SNMPProtocol.ASN1InsertHeader(SNMPProtocol.SEQUENCE, requestBuffer, requestBuffer.Count);
        }

        public static DriverErrorCodes PrepareBaseRequestv3(SNMPStation st, SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out byte[] requestBuffer)
        {
            List<byte> resultBuffer = new List<byte>();
            List<byte> resultBufferPdu = new List<byte>();
            List<byte> resultBufferHeader = new List<byte>();
            requestBuffer = null;
            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;

            if (messageGeneralData.FlagAuthentication || messageGeneralData.FlagPrivacy)
            {
                errorCode = st.CalculateAutenticationAndPrivacyKey(messageGeneralData);
                if (errorCode != DriverErrorCodes.ErrorNoError)
                    return errorCode;
            }

            // global message data
            SNMPProtocol.PrepareHeaderRequestv3(st, resultBufferHeader, messageGeneralData);
            resultBuffer.AddRange(resultBufferHeader);

            byte[] authenticationParameters = new byte[0];
            byte[] privacyParameters = new byte[0];

            // get authotiry, username, privacy paramters
            SNMPProtocol.PrepareUserSecurityModelRequestv3(st, resultBuffer, ref authenticationParameters, ref privacyParameters, messageGeneralData);

            if (messageGeneralData.FlagPrivacy)
            {
                resultBufferPdu.Clear();
                errorCode = SNMPProtocol.PreparePduCryptedWithPrivacyKeyRequestv3(st, resultBufferPdu, ref privacyParameters, messageGeneralData);
                if (errorCode != DriverErrorCodes.ErrorNoError)
                    return errorCode;

                resultBuffer.Clear();
                resultBuffer.AddRange(resultBufferHeader);

                SNMPProtocol.PrepareUserSecurityModelRequestv3(st, resultBuffer, ref authenticationParameters, ref privacyParameters, messageGeneralData);
                
                resultBuffer.AddRange(resultBufferPdu);
            }
            else
            {
                resultBufferPdu.Clear();
                SNMPProtocol.PreparePduRequestv3(st, resultBufferPdu, messageGeneralData);
                resultBuffer.AddRange(resultBufferPdu);
            }

            PrepareProtocolVersionRequestv3(resultBuffer, messageGeneralData);

            if (messageGeneralData.FlagAuthentication)
            {
                errorCode = st.CalculateAutenticationParameters(resultBuffer.ToArray(), messageGeneralData, out authenticationParameters);
                if (errorCode != DriverErrorCodes.ErrorNoError)
                    return errorCode;

                resultBuffer.Clear();
                resultBuffer.AddRange(resultBufferHeader);

                SNMPProtocol.PrepareUserSecurityModelRequestv3(st, resultBuffer, ref authenticationParameters, ref privacyParameters, messageGeneralData);
                resultBuffer.AddRange(resultBufferPdu);

                SNMPProtocol.PrepareProtocolVersionRequestv3(resultBuffer, messageGeneralData);
            }

            requestBuffer = resultBuffer.ToArray();

            return DriverErrorCodes.ErrorNoError;
        }
    }
}
