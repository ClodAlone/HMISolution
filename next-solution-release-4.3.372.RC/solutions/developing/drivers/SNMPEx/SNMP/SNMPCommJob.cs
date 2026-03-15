using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace SNMP
{
    public enum SNMPDATATYPE : byte
    {
        Integer = 0,
        Integer32,
        OctetString,
        Counter32,
        Unsigned32,
        Gauge32,
        TimeTicks,
        IpAddress,

        // custom
        Null = 254,
        Unknown = 255,
    }

    public class SNMPCommJob : CommJob
    {        
        public enum SNMPState : int
        {
            None = 0,
            Discovery,
            Authentication,
            Polling,
        }        
        
        #region Constructors
        public SNMPCommJob(Station station, SNMPCommJobSettings settings)
            : base(station, settings)
        {
            _snmpDataType = settings.snmpDataType;
            _snmpOid_Address = settings.snmpOid_Address.Trim();
            _snmpCommunity = settings.snmpCommunity.Trim();
            oidObj = new SNMPOid(_snmpOid_Address);
            _snmpTrapOnly = settings.snmpTrapOnly;
            if (_snmpTrapOnly)
                this.Type = LinkType.Input;
            _FirstRequest = true;
            CheckJobValid();
        }

        public SNMPCommJob(Station station, SNMPTag defTag)
            : base(station, defTag)
        {
            _snmpDataType = defTag.SNMPDynSettings.snmpDataType;
            _snmpOid_Address = defTag.SNMPDynSettings.snmpOid_Address.Trim();
            _snmpCommunity = defTag.SNMPDynSettings.snmpCommunity.Trim();
            oidObj = new SNMPOid(_snmpOid_Address);
            _snmpTrapOnly = defTag.SNMPDynSettings.snmpTrapOnly;
            if (_snmpTrapOnly)
                this.Type = LinkType.Input;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            _FirstRequest = true;
            CheckJobValid();
        }

        public SNMPCommJob(Station station)
            : base(station)
        {
            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;

            CheckJobValid();
        }

        protected SNMPCommJob()
        {
            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;

            CheckJobValid();
        }
        #endregion

        #region data Member
        public SNMPOid oidObj;
        byte[] codifiedCommunity;
        uint codifiedCommunityLength = 0;        
        #endregion

        #region Static methods
        public static bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (nType == (uint)BuiltInType.Boolean ||
                    nType == (uint)BuiltInType.Byte ||
                    nType == (uint)BuiltInType.Double ||
                    nType == (uint)BuiltInType.Float ||
                    nType == (uint)BuiltInType.Int16 ||
                    nType == (uint)BuiltInType.Int32 ||
                    nType == (uint)BuiltInType.Int64 ||
                    nType == (uint)BuiltInType.Integer ||
                    nType == (uint)BuiltInType.SByte ||
                    nType == (uint)BuiltInType.UInt16 ||
                    nType == (uint)BuiltInType.UInt32 ||
                    nType == (uint)BuiltInType.UInt64 ||
                    nType == (uint)BuiltInType.UInteger ||
                    nType == (uint)BuiltInType.String)
                {
                    return true;
                }
                    
                return false;
            }
            
            return false;
        }

        #endregion

        #region Methods

        public uint GetEstimatedWriteRequestLength()
        {
            // If not yet done, codify the OID
            if (!oidObj.IsValid())
                return (0);            

            //  Estimate the length of the codified data
            uint estimatedDataLength = estimateTheDataLength();
            if (estimatedDataLength == 0)
                return (0);

            return (4 + oidObj.codifiedOidLength + estimatedDataLength);
        }

        public uint GetTotalWriteRequestLength(uint jobDataSize)
        {
            return (4 + oidObj.codifiedOidLength) + jobDataSize;
        }

        public uint GetReadResponseLength()
        {
            // If not yet done, codify the OID
            if (!oidObj.IsValid())
                return (0);            

            // Estimate the codified response length
            uint estimatedDataLength = estimateTheDataLength();
            if (estimatedDataLength == 0)
                return (0);

            return (4 + oidObj.codifiedOidLength + estimatedDataLength);
        }

        public uint GetReadRequestLength()
        {
            // If not yet done, codify the OID
            if (!oidObj.IsValid()) 
                return (0);
            
            return(6 + oidObj.codifiedOidLength);
        }          

        protected byte[] GetJobCodifiedDataWrite(byte[] jobData)
        {
            byte[] codifiedData = null;
            uint codifiedDatalength = 0;
            switch (_snmpDataType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIntValueLength(jobData, jobData.Length);
                    if (codifiedDatalength > 0)
                    {
                        codifiedData = new byte[codifiedDatalength];
                        codifiedDatalength = SNMPProtocol.ASN1CodifyIntValue(jobData, jobData.Length, _snmpDataType, ref codifiedData);
                    }
                    break;

                case SNMPDATATYPE.Counter32:
                case SNMPDATATYPE.Unsigned32:
                case SNMPDATATYPE.Gauge32:
                case SNMPDATATYPE.TimeTicks:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedUintValueLength(jobData, jobData.Length);
                    if (codifiedDatalength > 0)
                    {
                        codifiedData = new byte[codifiedDatalength];
                        codifiedDatalength = SNMPProtocol.ASN1CodifyUintValue(jobData, jobData.Length, _snmpDataType, ref codifiedData);
                    }
                    break;

                case SNMPDATATYPE.OctetString:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(jobData, jobData.Length);
                    if (codifiedDatalength > 0)
                    {
                        codifiedData = new byte[codifiedDatalength];
                        codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValue(jobData, (int)codifiedDatalength, _snmpDataType, codifiedData);
                    }                    
                    break;

                case SNMPDATATYPE.IpAddress:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValueLength(jobData, jobData.Length);
                    if (codifiedDatalength > 0)
                    {
                        codifiedData = new byte[codifiedDatalength];
                        codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValue(jobData, (int)codifiedDatalength, _snmpDataType, ref codifiedData);
                    }                    
                    break;
            }

            return codifiedData;
        }


        protected uint estimateTheDataLength()
        {
            uint estimatedDataLength = 0;
            switch(_snmpDataType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                case SNMPDATATYPE.Counter32:
                case SNMPDATATYPE.Unsigned32:
                case SNMPDATATYPE.Gauge32:
                case SNMPDATATYPE.TimeTicks:
                    estimatedDataLength = 6;
                    break;

                case SNMPDATATYPE.OctetString:
                    estimatedDataLength = 4 + TagsList[0].Size;
                    break;

                case SNMPDATATYPE.IpAddress:
                    estimatedDataLength = 6;
                    break;
            }

            return (estimatedDataLength);
        }
       
        public uint GetCodifiedCommunityLength()
        {
            // Community not yet codified?
            if (codifiedCommunity == null)
            {
                // Get the length of the codified octet string
                codifiedCommunityLength = SNMPProtocol.ASN1GetCodifiedOctetStringLength(snmpCommunity);
                if (codifiedCommunityLength > 0)
                {
                    // Codify the community string
                    codifiedCommunity = new byte[codifiedCommunityLength];
                    SNMPProtocol.ASN1StringToOctetString(snmpCommunity, ref codifiedCommunity);
                }
            }

            return (codifiedCommunityLength);
        }

        public void GetCodifiedCommunity(out byte[] communityBuffer, uint tempCommunityLength)
        {
            communityBuffer = new byte[tempCommunityLength];

            for(uint i=0; i<codifiedCommunityLength; i++)
                communityBuffer[i] = codifiedCommunity[i];
        }

        List<DriverBaseInterfaces.TagDefinition> GetSimpleTagList(DriverBaseInterfaces.TagDefinition t)
        {
            List<DriverBaseInterfaces.TagDefinition> l = new List<DriverBaseInterfaces.TagDefinition>();
            if (t.DataType.IdType == IdType.Guid)
            {
                //prototype?
                List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
                Station.GetCommDriver().OnTagPrototypeQuery(t.NodeId, ref tList);
                foreach (var a in tList)
                {
                    l.AddRange(GetSimpleTagList(a));
                }
            }
            else
                l.Add(t);
            return l;
        }

        private void CheckJobValid()
        {

            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList =
                                new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) &&
                    t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                                    Properties.Resources.ErrorInvalidTagType,
                                    t.TagNode.NodeId.ToString(),
                                    t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ?
                            ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if (String.IsNullOrEmpty(_snmpOid_Address) || (oidObj == null) || !oidObj.IsValid())
            {
                IsValid = false;
                InvalidReason = string.Format(
                              Properties.Resources.SNMPInvalidAssignedOid,
                              _snmpOid_Address);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.SNMPInvalidJobSize, tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public DriverErrorCodes CheckResponseGeneralData(SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            return CheckResponseGeneralData(messageGeneralData, out SNMPProtocol.SNMPVariableBinding dummy);
        }

        public DriverErrorCodes CheckResponseGeneralData(SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out SNMPProtocol.SNMPVariableBinding variable)
        {
            variable = messageGeneralData.GetVariableBindingByOID(snmpOid_Address);
            if (variable == null)
            {
                if (messageGeneralData.InError())
                    return (DriverErrorCodes)messageGeneralData.GetErrorCode();

                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorNoSuchObject;
            }

            if (variable.ErrorCode != DriverErrorCodes.ErrorNoError)
                return variable.ErrorCode;

            if (!SNMPProtocol.CompatibleDataTypes((SNMPDATATYPE)variable.snmpDataType, snmpDataType))
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorDataTypeMismatch;

            switch ((SNMPVERSION)messageGeneralData.snmpVersion)
            {
                case SNMPVERSION.SNMPv1:
                case SNMPVERSION.SNMPv2c:                   
                    if (!_snmpCommunity.Equals(messageGeneralData.Community, StringComparison.Ordinal))
                        return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongCommunity;
                    break;

                case SNMPVERSION.SNMPv3:                
                    break;
            }

            // write operation performed ? (only 1 value at time)
            if (WriteExecuted)
            {
                if (messageGeneralData.InError())
                    return (DriverErrorCodes)messageGeneralData.GetErrorCode();
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public override uint GetMaxJobSize()
        {
            return (SNMPProtocol.GetMaxPacketSize() / 2);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            // Driver specific: aggregation is performed run-time
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;            
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
            }

            //prepare a write request
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            do
            {
                if (cand != null)
                {
                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];                
                listToWrite.Remove(cand);
                if (!listOnWriting.Contains(cand))
                    listOnWriting.Add(cand);

                if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    nData = (UInt16)((cand.Size + 7) / 8);
                else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                {
                    if (cand.TagNode.ArrayDimension == 0)
                        nData = (ushort)(GetProtocolDataByteSize());
                    else
                        nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
                }
                else
                {
                    nData = (UInt16)cand.Size;
                }

                // Get the current tag value
                jobdata = new byte[nData];
                cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));

                byte[] dataBuffer = GetJobCodifiedDataWrite(jobdata);

                jobData = dataBuffer;

            } while (listToWrite.Count > 0);

            UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);

            listOnWriting.Clear();
            listToWrite.Clear();            
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            
            base.SetJobData(rec, ref changed);
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    SNMPTag snmpTag = TagsList[TagIndex] as SNMPTag;
                    if (snmpTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (snmpTag.SetTagValue(ref rec, (int)snmpTag.ByteOffset))
                                changed.Add(snmpTag);
                        }
                        else
                        {
                            uint ArraySize = snmpTag.TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            snmpTag.setMemRW(rec, (int)snmpTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = snmpTag.getBoolValueFromMemRW((int)snmpTag.ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (snmpTag.SetTagValue(ref tmpData, 0))
                                changed.Add(snmpTag);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (snmpTag.SetTagValue(ref rec, (int)snmpTag.ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(snmpTag);
                        }
                        else
                        {
                            if (!ProtocolDataSizeBig())
                            {
                                uint elemsize = 0;
                                if (ElementNumber > 0)
                                {
                                    elemsize = GetProtocolDataByteSize();
                                }
                                if (snmpTag.SetTagValue(ref rec, (int)snmpTag.ByteOffset, elemsize))
                                    changed.Add(snmpTag);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)snmpTag.TagNode.DataType.Identifier);
                                uint ArraySize = snmpTag.TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                snmpTag.setMemRW(rec, (int)snmpTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, snmpTag.ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (snmpTag.SetTagValue(ref tmpData, 0))
                                    changed.Add(snmpTag);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Methods
        public void ResetSNMPParameters()
        {
            _SnmpState = SNMPState.None;
        }
        #endregion

        #region Properties

        private SNMPDATATYPE _snmpDataType;
        public SNMPDATATYPE snmpDataType
        {
            get { return _snmpDataType; }
            set { _snmpDataType = value; }
        }

        private string _snmpOid_Address;
        public string snmpOid_Address
        {
            get { return _snmpOid_Address; }
            set { _snmpOid_Address = value; }
        }

        private string _snmpCommunity;
        public string snmpCommunity
        {
            get { return _snmpCommunity; }
            set { _snmpCommunity = value; }
        }

        private bool _snmpTrapOnly;
        public bool snmpTrapOnly
        {
            get { return _snmpTrapOnly; }
            set { _snmpTrapOnly = value; }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}CMN{1}DA{2:00}", ret, _snmpCommunity, (uint)_snmpDataType);
            }
        }

        public bool WriteExecuted
        {
            get { return this.GetTagListOnWritingCount() > 0; }            
        }        

        private SNMPState _SnmpState = SNMPState.None;
        public SNMPState SnmpState
        {
            get { return _SnmpState; }
            set { _SnmpState = value; }
        }

        /// <summary>
        /// To avoid problems with multiple oid requests, the tag is requested individually
        /// </summary>
        private bool _FirstRequest;
        public bool FirstRequest
        {
            get { return _FirstRequest; }
            set { _FirstRequest = value; }
        }


        public override uint SamplingInterval
        {
            get
            {
                if (_snmpTrapOnly)
                    return CommJob.JOB_NOT_SCHEDULABLE;
                else
                    return _SamplingInterval;
            }
        }
        #endregion
    }
}
