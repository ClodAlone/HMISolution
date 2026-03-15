using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace SNMP
{
    public enum SNMPDATATYPE : byte
    {
        Integer,
        Integer32,
        OctetString,
        Counter32,
        Unsigned32,
        Gauge32,
        TimeTicks,
        IpAddress
    }

    public class SNMPCommJob : CommJob
    {
        #region constants
        public const uint MAX_SNMP_PACKET = 4096;
        public const int DISCARD_IOEO_UNCHANGEDVALUE = -1;

        public enum DataFormatErrors
        {
            NoError,
            GenericError,
            DiscardIOEOUnchangedValue,
            DiscardTooMuchJobs
        }

        #endregion

        #region Constructors
        public SNMPCommJob(Station station, SNMPCommJobSettings settings)
            : base(station, settings)
        {
            ExecuteTask = false;
            _WriteExecuted = false;

            _snmpDataType = settings.snmpDataType;
            _snmpDataSize = settings.snmpDataSize;
            _snmpOid_Address = settings.snmpOid_Address.Trim();
            _snmpCommunity = settings.snmpCommunity.Trim();
            oidObj = new SNMPOid(_snmpOid_Address);
  
            CheckJobValid();
        }

        public SNMPCommJob(Station station, SNMPTag defTag)
            : base(station, defTag)
        {
            ExecuteTask = false;
            _WriteExecuted = false;

            _snmpDataType = defTag.SNMPDynSettings.snmpDataType;
            _snmpDataSize = defTag.SNMPDynSettings.snmpDataSize;
            _snmpOid_Address = defTag.SNMPDynSettings.snmpOid_Address.Trim();
            _snmpCommunity = defTag.SNMPDynSettings.snmpCommunity.Trim();
            oidObj = new SNMPOid(_snmpOid_Address);

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public SNMPCommJob(Station station)
            : base(station)
        {
            ExecuteTask = false;
            _WriteExecuted = false;

            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpDataSize = 4;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = "public";

            CheckJobValid();
        }

        protected SNMPCommJob()
        {
            ExecuteTask = false;
            _WriteExecuted = false;

            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpDataSize = 4;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = "public";

            CheckJobValid();
        }
        #endregion

        #region data Member

        public bool ExecuteTask;
        public SNMPOid oidObj;
        byte[] codifiedCommunity;
        uint codifiedCommunityLength = 0;
        uint codifiedOidLength = 0;
        byte[] codifiedOid;
        public DataFormatErrors badDataFormat = DataFormatErrors.NoError;

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
            if (codifiedOidLength == 0)
            {
                codifyOID();
            }

            if (codifiedOidLength == 0)
            {
                return (0);
            }

            //  Estimate the length of the codified data
            uint estimatedDataLength = estimateTheDataLength();
            if (estimatedDataLength == 0)
            {
                return (0);
            }
            return (4 + codifiedOidLength + estimatedDataLength);
        }

        public uint GetWriteRequestLength()
        {
            _WriteDataLength = 0;

            // If not yet done, codify the OID
            if (codifiedOidLength == 0)
            {
                codifyOID();
            }

            if (codifiedOidLength == 0)
            {
                return (0);
            }

            //  Calculate the length of the codified data
            _WriteDataLength = GetTheDataLength();
            // exception output/input output with unchanged value
            if (_WriteDataLength == DISCARD_IOEO_UNCHANGEDVALUE)
            {
                badDataFormat = DataFormatErrors.DiscardIOEOUnchangedValue;
                return (0);
            }
            // old badDataFormat = true
            if (_WriteDataLength == 0) { 
                badDataFormat = DataFormatErrors.GenericError;
                return (0);
            }

            return (uint)(4 + codifiedOidLength + _WriteDataLength);
        }

        public uint GetReadResponseLength()
        {
            // If not yet done, codify the OID
            if (codifiedOidLength == 0)
            {
                codifyOID();
            }

            if (codifiedOidLength == 0)
            {
                return (0);
            }

            // Estimate the codified response length
            uint estimatedDataLength = estimateTheDataLength();
            if(estimatedDataLength == 0)
            {
                return (0);
            }
            return (4 + codifiedOidLength + estimatedDataLength);
        }

        public uint GetReadRequestLength()
        {
            // If not yet done, codify the OID
            if(codifiedOidLength == 0)
            {
                codifyOID();
            }

            if (codifiedOidLength == 0)
            {
                return (0);
            }

            return(6 + codifiedOidLength);
        }

        protected uint GetJobCodifiedDataLength(byte[] jobData, uint dataSize)
        {
            // Returned value
            uint codifiedDatalength = 0;

            switch (_snmpDataType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIntValueLength(jobData, (int)dataSize);
                    break;

                case SNMPDATATYPE.Counter32:
                case SNMPDATATYPE.Unsigned32:
                case SNMPDATATYPE.Gauge32:
                case SNMPDATATYPE.TimeTicks:
                    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedUintValueLength(jobData, (int)dataSize);
                    break;

                case SNMPDATATYPE.OctetString:
                    //if ((_snmpDataSize > 0) && (_snmpDataSize >= dataSize))
                    //{
                    //    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(jobData, (int)dataSize);
                    //}
                    if (_snmpDataSize > 0)
                    {
                        if (_snmpDataSize >= dataSize)
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(jobData, (int)dataSize);
                        }
                        else
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValueLength(jobData, (int)_snmpDataSize);
                        }
                    }
                    break;

                case SNMPDATATYPE.IpAddress:
                    //if ((_snmpDataSize > 0) && (_snmpDataSize >= dataSize))
                    //{
                    //    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValueLength(jobData, (int)dataSize);
                    //}
                    if (_snmpDataSize > 0)
                    {
                        if (_snmpDataSize >= dataSize)
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValueLength(jobData, (int)dataSize);
                        }
                        else
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValueLength(jobData, (int)_snmpDataSize);
                        }
                    }
                    break;
            }

            return (codifiedDatalength);
        }

        protected uint GetJobCodifiedData(byte[] jobData, uint dataSize, ref byte[] codifiedData)
        {
            // Returned value
            uint codifiedDatalength = 0;
            switch (_snmpDataType)
            {
                case SNMPDATATYPE.Integer:
                case SNMPDATATYPE.Integer32:
                    codifiedDatalength = SNMPProtocol.ASN1CodifyIntValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                    break;

                case SNMPDATATYPE.Counter32:
                case SNMPDATATYPE.Unsigned32:
                case SNMPDATATYPE.Gauge32:
                case SNMPDATATYPE.TimeTicks:
                    codifiedDatalength = SNMPProtocol.ASN1CodifyUintValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                    break;

                case SNMPDATATYPE.OctetString:
                    //if ((_snmpDataSize > 0) && (_snmpDataSize >= dataSize))
                    //{
                    //    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                    //}
                    if (_snmpDataSize > 0)
                    {
                        if (_snmpDataSize >= dataSize)
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                        }
                        else
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedOctetStringValue(jobData, (int)_snmpDataSize, _snmpDataType, ref codifiedData);
                        }
                    }
                    break;

                case SNMPDATATYPE.IpAddress:
                    //if ((_snmpDataSize > 0) && (_snmpDataSize >= dataSize))
                    //{
                    //    codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                    //}
                    if (_snmpDataSize > 0)
                    {
                        if (_snmpDataSize >= dataSize)
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValue(jobData, (int)dataSize, _snmpDataType, ref codifiedData);
                        }
                        else
                        {
                            codifiedDatalength = SNMPProtocol.ASN1GetCodifiedIpAddressValue(jobData, (int)_snmpDataSize, _snmpDataType, ref codifiedData);
                        }
                    }
                    break;
            }

            return (codifiedDatalength);
        }

        public int GetTheDataLength()
        {
            // Returned value
            int dataLength = 0;

            lock (lockListObject)
            {
                if (Type == LinkType.UnconditionalOutput)
                {
                    if (TagsListOnWriting.Count == 0)
                        TagsListOnWriting.AddRange(TagsList);
                }
                if (TagsListOnWriting.Count > 0)
                {
                    SNMPTag tag = (SNMPTag)TagsListOnWriting[0];
                    if (tag != null)
                    {
                        uint tsize = 0;
                        byte[] jobdata;
                        UInt16 nData = 0;
                        Tag cand = (Tag)tag;

                        if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                            nData = (UInt16)((tag.Size + 7) / 8);
                        else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                        {
                            if (tag.TagNode.ArrayDimension == 0)
                                nData = (ushort)(GetProtocolDataByteSize());
                            else
                                nData = (ushort)(GetProtocolDataByteSize() * tag.TagNode.ArrayDimension);
                        }
                        else
                            nData = (UInt16)tag.Size;
                        jobdata = new byte[nData];

                        // manage input output/exception output data discard --> return special code
                        if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                        {
                            if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                            {
                                if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                                {
                                    if (TagsListOnWriting.Contains(cand))
                                    {
                                        TagsListOnWriting.Remove(cand);
                                    }
                                    return DISCARD_IOEO_UNCHANGEDVALUE;
                                }
                            }
                        }

                        cand.LastValue = cand.Value.Value;

                        tsize = tag.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        dataLength = (int)GetJobCodifiedDataLength(jobdata, tsize);
                    }
                }
            }
            return (dataLength);
        }

        public uint GetCodifiedData(ref byte[] dataBuffer)
        {
            // Returned value
            uint dataLength = 0;
            var listToWrite = new List<Tag>();
            lock (lockListObject)
            {
                if (TagsListOnWriting.Count > 0)
                {
                    listToWrite.Add(TagsListOnWriting[0]);
                }
            }

            if (listToWrite.Count > 0)
            {
                SNMPTag tag = (SNMPTag)listToWrite[0];
                if (tag != null)
                {
                    uint tsize = 0;
                    byte[] jobdata;
                    UInt16 nData = 0;

                    if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        nData = (UInt16)((tag.Size + 7) / 8);
                    else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                    {
                        if (tag.TagNode.ArrayDimension == 0)
                            nData = (ushort)(GetProtocolDataByteSize());
                        else
                            nData = (ushort)(GetProtocolDataByteSize() * tag.TagNode.ArrayDimension);
                    }
                    else
                    {
                        nData = (UInt16)tag.Size;
                    }

                    // Get the current tag value
                    jobdata = new byte[nData];
                    tsize = tag.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));

                    // Codify data
                    dataLength = GetJobCodifiedData(jobdata, tsize, ref dataBuffer);
                    AddTagListOnWriting(tag);
                    //lock (retLockList())
                    //{
                    //    if (!TagsListOnWriting.Contains(tag))
                    //        TagsListOnWriting.Add(tag);
                    //}
                }
            }

            return (dataLength);
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
                    if (_snmpDataSize > 0)
                    {
                        estimatedDataLength = 4 + _snmpDataSize;
                    }
                    break;

                case SNMPDATATYPE.IpAddress:
                    estimatedDataLength = 6;
                    break;
            }

            return (estimatedDataLength);
        }

        protected void codifyOID()
        {
            codifiedOid = null;
            codifiedOidLength = 0;

            // Get the number of bytes requested for the codified OID
            uint tempcodifiedOidLength = SNMPProtocol.ASN1GetCodifiedOIDLength(oidObj.oidIntValues, oidObj.oidLength);
            if (tempcodifiedOidLength > 0)
            {
                // Allocate the array of bytes for the codified OID
                codifiedOid = new byte[tempcodifiedOidLength];

                // Codify the OID
                codifiedOidLength = SNMPProtocol.ASN1CodifyOID(oidObj.oidIntValues, oidObj.oidLength, ref codifiedOid);
            }
        }
 
        public uint GetCodifiedOIDLength()
        {
            return (codifiedOidLength);
        }

        public uint GetCodifiedOID(ref byte[] codifiedValue)
        {
            uint i = 0;
            for(i=0; i<codifiedOidLength; i++)
            {
                codifiedValue[i] = codifiedOid[i];
            }

            return (codifiedOidLength);
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

        public void GetCodifiedCommunity(ref byte[] communityBuffer)
        {
            uint i = 0;
            for(i=0; i<codifiedCommunityLength; i++)
            {
                communityBuffer[i] = codifiedCommunity[i];
            }
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

            if (String.IsNullOrEmpty(_snmpOid_Address) || (oidObj == null) || !oidObj.IsValid)
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

            // Force a minimum length of 15 for the "Data Size" of an IP address
            if ((_snmpDataType == SNMPDATATYPE.IpAddress) && (_snmpDataSize < 15))
            {
                _snmpDataSize = 15;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override uint GetMaxJobSize()
        {
            return (MAX_SNMP_PACKET / 2);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            // Driver specific: aggregation is performed run-time
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                default:
                    return false;
            }
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            //return true;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
            }

            List<byte> outData = new List<byte>();
            //prepare a write request
            listToWrite.Sort(CompareTagByOffset);
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
                    nData = (UInt16)cand.Size;
                lock (lockListObject)
                {
                    cand.LastValue = cand.Value.Value;
                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                }
                uint ArraySize = cand.TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (ProtocolDataSizeBig())
                {
                    List<byte> correctData = new List<byte>();
                    UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                    UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        byte[] tmpdata = new byte[sizeDataType];
                        if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                            Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                        else
                        {
                            if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                                tmpdata[0] = 0;
                            else
                                tmpdata[0] = 1;
                        }
                        cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                        correctData.AddRange(tmpdata);
                    }
                    jobdata = correctData.ToArray();
                }
                else if (isProtocolBool())
                {
                    if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
                        for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
                        {
                            if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
                                tmpData[ArrayIndex] = 1;
                        }
                        jobdata = tmpData;
                    }
                }

                if (!isProtocolBool())
                {
                    if (SwapBytes)
                    {
                        SwapByteBuffer(ref jobdata);
                    }

                    if (SwapWords)
                    {
                        SwapWordBuffer(ref jobdata);
                    }
                }

                outData.AddRange(jobdata);

            } while (listToWrite.Count > 0);

            lock (lockListObject)
            {
                TagsListOnWriting.Clear();
                TagsListOnWriting.AddRange(listOnWriting);

                listToWrite.ForEach((tag) =>
                {
                    if (!TagsListToWrite.Contains(tag))
                        TagsListToWrite.Add(tag);
                });

                listOnWriting.ForEach((tag) =>
                {
                    TagsListToWrite.Remove(tag);
                });
                listOnWriting.Clear();
                listToWrite.Clear();
            }

            if (isProtocolBool())
            {
                byte[] tmpData = new byte[(outData.Count() + 7) / 8];
                for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
                {
                    if (outData[ArrayIndex] != 0)
                        tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                }
                jobData = tmpData;
            }
            else
                jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            //byte[] rec = jobData as byte[];
            //if (rec == null)
            //    return;

            byte[] tempRec = jobData as byte[];
            if (tempRec == null)
                return;
            uint dataSize = (uint)tempRec.GetLength(0);

            if (_snmpDataType == SNMPDATATYPE.OctetString || _snmpDataType == SNMPDATATYPE.IpAddress)
            {
                if (_snmpDataSize > 0)
                {
                    if (dataSize > snmpDataSize)
                        dataSize = snmpDataSize;
                }
            }

            byte[] rec = new byte[dataSize];
            Array.Copy(tempRec, rec, dataSize);

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

        private static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;// x < y

                }
            }
        }

        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Properties

        private SNMPDATATYPE _snmpDataType;
        public SNMPDATATYPE snmpDataType
        {
            get { return _snmpDataType; }
            set { _snmpDataType = value; }
        }

        private UInt32 _snmpDataSize;
        public UInt32 snmpDataSize
        {
            get { return _snmpDataSize; }
            set { _snmpDataSize = value; }
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

        private bool _WriteExecuted;//steve 201011
        public bool WriteExecuted
        {
            get { return _WriteExecuted; }
            set
            {
                _WriteExecuted = value;
            }
        }

        private int _WriteDataLength;
        public int WriteDataLength
        {
            get { return _WriteDataLength; }
            set
            {
                _WriteDataLength = value;
            }
        }
                
        #endregion
    }
}
