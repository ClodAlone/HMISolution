using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetCommJob : CommJob
    {
        
        #region Constructors
        public OmronFinsEthernetCommJob(Station station, OmronFinsEthernetCommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            _DataConversionType = settings.DataConversionType;
            AddressObj = new OmronAddress(_Address);
  
            CheckJobValid();
        }

        public OmronFinsEthernetCommJob(Station station, OmronFinsEthernetTag defTag)
            : base(station, defTag)
        {
            _Address = defTag.OmronFinsEthernetDynSettings.Address;
            _DataConversionType = defTag.OmronFinsEthernetDynSettings.DataConversionType;
            AddressObj = new OmronAddress(_Address);
            int stringLength = AddressObj.StringLength;
            if((TotalJobSize == 0) && (stringLength > 0))
            {
                if (GetProtocolDataByteSize() == 1)
                {
                    TotalJobSize = (uint)stringLength;
                }
                else
                {
                    TotalJobSize = (uint)((stringLength + 1)/2)*2;
                }
            }

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public OmronFinsEthernetCommJob(Station station)
            : base(station)
        {
            _Address = String.Empty;
            _DataConversionType = DataConversionTypes.None;
            AddressObj = new OmronAddress();
            CheckJobValid();
        }

        protected OmronFinsEthernetCommJob()
        {
            _Address = String.Empty;
            _DataConversionType = DataConversionTypes.None;
            AddressObj = new OmronAddress();
            CheckJobValid();
        }
        #endregion
        #region data Member

        public OmronAddress AddressObj;

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
                nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }
            
            return false;
        }

        #endregion
        
        #region Methods
        public static uint GetNodeSize(NodeId DataType)
        {
            if (DataType.IdType == IdType.Numeric)
            {
                return GetDataTypeSize((uint)DataType.Identifier);
            }
            return 0;
        }

        public static uint GetDataTypeSize(uint Type)
        {
            switch (Type)
            {
                case (uint)BuiltInType.Boolean:
                case (uint)BuiltInType.SByte:
                case (uint)BuiltInType.Byte:
                case (uint)BuiltInType.String:
                    return 1;
                case (uint)BuiltInType.Int16:
                case (uint)BuiltInType.UInt16:
                    return 2;
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.UInt32:
                case (uint)BuiltInType.Int32:
                    return 4;
                case (uint)BuiltInType.UInt64:
                case (uint)BuiltInType.Int64:
                case (uint)BuiltInType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        public static uint GetDataTypeSize(UFUAModel.DataType Type)
        {
            switch (Type)
            {
                case UFUAModel.DataType.Boolean:
                case UFUAModel.DataType.SByte:
                case UFUAModel.DataType.Byte:
                    return 1;
                case UFUAModel.DataType.Int16:
                case UFUAModel.DataType.UInt16:
                    return 2;
                case UFUAModel.DataType.Float:
                case UFUAModel.DataType.UInt32:
                case UFUAModel.DataType.Int32:
                    return 4;
                case UFUAModel.DataType.UInt64:
                case UFUAModel.DataType.Int64:
                case UFUAModel.DataType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        return 1;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        return 2;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        return 4;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        return 8;
                    case (uint)BuiltInType.String:
                        if (GetProtocolDataByteSize() == 1)
                            return (uint)AddressObj.StringLength;
                        else
                            return (uint)((AddressObj.StringLength + 1) / 2) * 2;
                    default:
                        return 0;
                }
            }
            return 0;
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

            if (!AddressObj.IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format(
                              Properties.Resources.ErrorInvalidAssignedAddress,
                              Address);
            }

            bool bit = false;
            bool num = false;
            uint size = 0;
            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                {
                    bit = true;
                }
                else
                {
                    num = true;
                }
                size += GetTagSize(t);

                if (((uint)t.DataType.Identifier == (uint)BuiltInType.String) &&
                    ((size < 1)||(size > Properties.Settings.Default.MaxStringSize)))
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                        Properties.Resources.ErrorInvalidAddressString,
                         Properties.Settings.Default.MaxStringSize);
                    return;
                }
            }

            // Do not mix bit and non bit variables
            if (bit && num)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorValidateJob,
                    tagnamelist,
                    Properties.Resources.ErrorMixBitsAndOthers);
                return;
            }

            
            // Type bits variable cannot be converted to BCD 
            if  (bit && DataConversionType != DataConversionTypes.None)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorValidateJob,
                    tagnamelist,
                    Properties.Resources.ErrorConvertBitBCD);
                return;
            }
            
            if (DataConversionType == DataConversionTypes.BCD16Bits)
            {
                if (TotalJobSize < 2 || TotalJobSize % 2 != 0)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                        Properties.Resources.ErrorValidateJob,
                        tagnamelist,
                        Properties.Resources.ErrorDataConvNotValid);
                    return;
                }
            }

            if (DataConversionType == DataConversionTypes.BCD32Bits)
            {
                if (TotalJobSize < 4 || TotalJobSize % 4 != 0)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                        Properties.Resources.ErrorValidateJob,
                        tagnamelist,
                        Properties.Resources.ErrorDataConvNotValid);
                    return;
                }
            }


            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Address. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, Address);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public override uint getProtocolDataType()
        {
            switch (OmronFinsEthernetProtocol.DataType(AddressObj))
            {
                case UFUAModel.DataType.UInt16:
                    switch (DataConversionType) {
                        case DataConversionTypes.None:
                            switch (ElementNumber)
                            {
                                case 2:
                                    return ((uint)BuiltInType.UInt32);
                                case 4:
                                    return ((uint)BuiltInType.UInt64);
                                default:
                                    return (uint)BuiltInType.UInt16;
                            }
                        case DataConversionTypes.BCD16Bits:
                            return (uint)BuiltInType.Int16;
                        case DataConversionTypes.BCD32Bits:
                            return (uint)BuiltInType.Int32;
                        default:
                            return 0;
                    }
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.String:
                    return (uint)BuiltInType.String;
                default:
                    return 0;
            }
        }

        public override uint GetMaxJobSize()
        {
            return OmronFinsEthernetProtocol.GetMaxJobSize(Type);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            OmronFinsEthernetCommJob testJob = candJob as OmronFinsEthernetCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (!AddressObj.IsValid || !testJob.AddressObj.IsValid)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataArea != testJob.AddressObj.DataArea)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (DataConversionType != testJob.DataConversionType)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataArea == DataAreas.EM && AddressObj.BankNumber != testJob.AddressObj.BankNumber)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            bool bBit = AddressObj.isBit();
            bool bTagBit = testJob.AddressObj.isBit();
            if ((!bBit && bTagBit) || (bBit && !bTagBit))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataFormat != testJob.AddressObj.DataFormat)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if(AddressObj.DataFormat == DataFormats.String)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType))) 
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // ElementNumber can be used to read more data elements ( >=1 ) so don't allow aggregations
            if (ElementNumber != candJob.ElementNumber)
                return JobAggregationType.JobAggregImpossible;

                        //ExtraBytes = 0;
                        //uint Granularity = Station.GetCommDriver().AggregationThreshold;
                        //uint start = (uint)AddressObj.Value;
                        //uint startTest = (uint)testJob.AddressObj.Value;
                        //uint elementsize = 2;

                        //uint end = start + (TotalJobSize / elementsize) - 1;
                        //uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;


                        uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint candJobStartAdd;
            uint JobStartAdd;
            uint candJobEndAdd;
            uint JobEndAdd;
            bool BitFormat = false;
            uint elementsize = 2;

            if (AddressObj.DataFormat == DataFormats.Bit)
            {
                BitFormat = true;
                elementsize = 1;
                Granularity *= 16;
            }


            if (BitFormat)
            {
                candJobStartAdd = (uint)((testJob.AddressObj.Address.USHORT * 16) + testJob.AddressObj.BitNumber);
                JobStartAdd = (uint)((AddressObj.Address.USHORT * 16) + AddressObj.BitNumber );
            }
            else
            {
                candJobStartAdd = (uint)testJob.AddressObj.Address.USHORT;
                JobStartAdd = (uint)AddressObj.Address.USHORT;
            }

            candJobEndAdd = candJobStartAdd + testJob.TotalJobSize / elementsize - 1;
            JobEndAdd = JobStartAdd + TotalJobSize / elementsize - 1;

            if (DataConversionType == DataConversionTypes.BCD32Bits)
            {
                uint TagAddAlign = candJobStartAdd % 2;
                uint JobAddAlign = JobStartAdd % 2;
                if (JobAddAlign != TagAddAlign)
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */

            // If the intersection between the address ranges of the tag and of the job
            // is not empty, check that there are not collisions among the addresses of
            // the job tags and the ones of the tag.
            if (((candJobStartAdd >= JobStartAdd) && (candJobStartAdd <= JobEndAdd)) ||
                ((candJobEndAdd >= JobStartAdd) && (candJobEndAdd <= JobEndAdd)))
            {
                /*
                 * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
                 */
                foreach (var tag in TagsList)
                {
                    if (TagAndDynVarCollide(candJobStartAdd, candJobEndAdd,
                                           JobStartAdd, tag, elementsize))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            // If the address range of the job is strictly included in the range of the
            // tag, the job cannot aggregate. 
            else if ((candJobStartAdd < JobStartAdd) && (candJobEndAdd > JobEndAdd))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // If there is no intersection between the address ranges of the tag and of
            // the job, check the granularity.
            else
            {
                // Check the granularity.
                if (candJobEndAdd < JobStartAdd)
                {
                    if ((JobStartAdd - candJobEndAdd) > (Granularity + 1))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else if ((candJobStartAdd - JobEndAdd) > (Granularity + 1))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }


            if (candJobStartAdd >= JobStartAdd && candJobEndAdd <= JobEndAdd)
            {
                candJob.TagsList[0].ByteOffset = (candJobStartAdd - JobStartAdd) * elementsize;
                return JobAggregationType.JobAggregFits;
            }

            if (candJobStartAdd >= JobStartAdd && candJobStartAdd <= JobEndAdd + Granularity)
                //if (((endTest - startTest + 1) * elementsize) <= GetMaxJobSize())
                if (((candJobEndAdd - JobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = (candJobStartAdd - JobStartAdd) * elementsize;
                    ExtraBytes = (candJobEndAdd - JobEndAdd) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (JobStartAdd > Granularity)
                startBkw = JobStartAdd - Granularity;
            else
                startBkw = 0;
            if (candJobEndAdd >= startBkw && candJobEndAdd <= JobEndAdd)
                if (((JobEndAdd - candJobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = 0;
                    ExtraBytes = (JobStartAdd - candJobStartAdd) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }

        bool TagAndDynVarCollide(uint DynVarStartAdd, uint DynVarEndAdd,
            uint JobStartAddress, Tag tag, uint elementsize)
        {
            uint TagStartAdd = 0;
            uint TagEndAdd = 0;

            TagStartAdd = JobStartAddress + tag.ByteOffset / elementsize;
            TagEndAdd = TagStartAdd + tag.Size / elementsize - 1;
            
            if( ((DynVarStartAdd < TagStartAdd) && (DynVarEndAdd < TagStartAdd)) ||
                ((DynVarStartAdd > TagEndAdd) && (DynVarEndAdd > TagEndAdd)) ) {
                return false;
            }
            
            return( true );
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new OmronFinsEthernetTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new OmronFinsEthernetTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                        tag.ByteOffset += ExtraBytes;
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new OmronFinsEthernetTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    Address = ((OmronFinsEthernetCommJob)candJob).Address;
                    AddressObj = ((OmronFinsEthernetCommJob)candJob).AddressObj;
                    break;
                default:
                    return false;
            }
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            return true;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListToWrite);
                TagsListToWrite.Clear();
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
                    // Special case: for the strings an even number of characters must be written (16 bits data area)
                    if (((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.String) && ((nData % 2) != 0))
                    {
                        jobdata = new byte[nData + 1];
                    }
                    else
                    {
                        jobdata = new byte[nData];
                    }
                    //jobdata = new byte[nData];
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

                if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
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
                listOnWriting.ForEach((tag) =>
                {
                    if (!TagsListOnWriting.Contains(tag))
                        TagsListOnWriting.Add(tag);
                });
                listOnWriting.Clear();

                if (listToWrite.Count > 0)
                {
                    var tempListToWrite = new List<Tag>();
                    tempListToWrite.AddRange(TagsListToWrite);
                    TagsListToWrite.Clear();
                    listToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    tempListToWrite.Clear();
                }
            }

            jobData = outData.ToArray();
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
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(TagsList[TagIndex]);
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
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    changed.Add(TagsList[TagIndex]);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public CommandCodes CommandCodeOnExecute()
        {
            if (Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                TagsListOnWriting.Count == 0))
            {
                return CommandCodes.Read;
            }
            else
            {
                return CommandCodes.Write;
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

        #endregion

        #region Properties
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
            }
        }

        private DataConversionTypes _DataConversionType;
        public DataConversionTypes DataConversionType
        {
            get { return _DataConversionType; }
            set { _DataConversionType = value; }
        }


        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}DA{1:00}", ret, (uint)AddressObj.DataArea);
            }
        }
 
        #endregion
    }
}
