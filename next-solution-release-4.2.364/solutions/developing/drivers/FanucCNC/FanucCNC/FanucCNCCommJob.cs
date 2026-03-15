using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace FanucCNC
{
    public class FanucCNCCommJob : CommJob
    {
        #region Constructors
        public FanucCNCCommJob(Station station, FanucCNCCommJobSettings settings)
            : base(station, settings)
        {
            _FunctionCode = settings.FunctionCode;
            _FunctionSettings = settings.FunctionSettings;
            if (settings.FunctionSettings != null)
                _StringLength = settings.FunctionSettings.StringLength;

            _CNCPath = settings.CNCPath;

            CheckJobValid();
        }

        public FanucCNCCommJob(Station station, FanucCNCTag defTag)
            : base(station, defTag)
        {
            _FunctionCode = defTag.FanucCNCDynSettings.FunctionCode;
            _FunctionSettings = defTag.FanucCNCDynSettings.FunctionSettings;
            if (_FunctionSettings != null)
                _StringLength = _FunctionSettings.StringLength;

            _CNCPath = defTag.FanucCNCDynSettings.CNCPath;         

            CheckJobValid();
        }

        public FanucCNCCommJob(Station station)
            : base(station)
        {
            ElementNumber = 0;
            CheckJobValid();
        }

        protected FanucCNCCommJob()
        {
            ElementNumber = 0;
            CheckJobValid();
        }
        #endregion

        #region data Member
        #endregion

        #region Methods

        public bool IsTypeAdmitted(NodeId type)
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
                        return (ushort)_StringLength;
                    default:
                        return 0;
                }
            }
            return 0;
        }
      
        private void CheckJobValid()
        {
            if (TagsList.Count() == 0)
            {
                IsValid = false;
                InvalidReason = "Job not contain tag";
                return;
            }

            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }
                size += GetTagSize(t);
            }

            if (!IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format("{0}", Properties.Resources.ParseKo);
                return;
            }            

            if (this.FunctionSettings != null)
            {
                this.FunctionSettings.SetChannelStationInfo(((FanucCNCChannel)Station.GetChannel()).MachineSerie, this);
                if (!this.FunctionSettings.IsValid)
                {
                    IsValid = false;
                    InvalidReason = string.Format("{0} {1} {2}", Properties.Resources.ErrorTagNotValid, TagsList[0].TagNode.NodeId, this.FunctionSettings.InvalidReason);
                    return;
                }
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        // non cancellare
        //public override uint getProtocolDataType()
        //{
        //    return (uint)_FocasAddress.VarType;
        //    //switch (_FocasAddress.VarType))
        //    //{
        //    //    case UFUAModel.DataType.Boolean:
        //    //        return (uint)BuiltInType.Boolean;
        //    //    case UFUAModel.DataType.Byte:
        //    //        return (uint)BuiltInType.Byte;
        //    //    case UFUAModel.DataType.SByte:
        //    //        return (uint)BuiltInType.SByte;
        //    //    case UFUAModel.DataType.Int16:
        //    //        return (uint)BuiltInType.Int16;
        //    //    case UFUAModel.DataType.UInt16:
        //    //        return (uint)BuiltInType.UInt16;
        //    //    case UFUAModel.DataType.Int32:
        //    //        return (uint)BuiltInType.Int32;
        //    //    case UFUAModel.DataType.UInt32:
        //    //        return (uint)BuiltInType.UInt32;
        //    //    case UFUAModel.DataType.Int64:
        //    //        return (uint)BuiltInType.Int64;
        //    //    case UFUAModel.DataType.UInt64:
        //    //        return (uint)BuiltInType.UInt64;
        //    //    case UFUAModel.DataType.Float:
        //    //        return (uint)BuiltInType.Float;
        //    //    case UFUAModel.DataType.Double:
        //    //        return (uint)BuiltInType.Double;
        //    //    default:
        //    //        return 0;
        //    //}
        //}

        public override uint GetMaxJobSize()
        {
            return FanucCNCProtocol.MAX_DATA_BYTES;
        }

        private bool TagAndDynVarCollide(uint DynVarStartAdd, uint DynVarEndAdd,
            uint JobStartAddress, Tag tag, uint elementsize)
        {
            uint TagStartAdd = 0;
            uint TagEndAdd = 0;

            TagStartAdd = JobStartAddress + tag.ByteOffset / elementsize;
            TagEndAdd = TagStartAdd + tag.Size / elementsize - 1;

            if (((DynVarStartAdd < TagStartAdd) && (DynVarEndAdd < TagStartAdd)) ||
                ((DynVarStartAdd > TagEndAdd) && (DynVarEndAdd > TagEndAdd)))
            {
                return false;
            }

            return (true);
        }

        private uint GetAggregateMaxJobSize(bool bitAddress)
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = Focas_Library.Focas1.IODBPMCXX_CDATA_SIZE;// GetMaxJobSize();
            if (bitAddress)
            {
                AggregLimit *= 8;
                JobMaxSize *= 8;
            }
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            FanucCNCCommJob testJob = candJob as FanucCNCCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;
            
            if (_FunctionCode != FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng || testJob.FunctionCode != FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).FunctionCode != ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)testJob.FunctionSettings).FunctionCode)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (_FunctionSettings == null || !_FunctionSettings.IsValid || testJob.FunctionSettings == null || !testJob.FunctionSettings.IsValid)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).RDataType != ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)testJob.FunctionSettings).RDataType)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).IsArray() || ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)testJob.FunctionSettings).IsArray())
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!string.IsNullOrEmpty(this.OffsetVariableName) || !string.IsNullOrEmpty(testJob.OffsetVariableName))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (_FunctionSettings.IsStructType() || ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)testJob.FunctionSettings).IsStructType())
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).IsBitAddress() && !((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)testJob.FunctionSettings).IsBitAddress())
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (testJob.TagsList[0].TagNode.ArrayDimension != 0)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            //// ElementNumber can be used to read more data elements ( >=1 ) so don't allow aggregations
            //if (ElementNumber != candJob.ElementNumber)
            //    return JobAggregationType.JobAggregImpossible;

            FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng thisFunctionSettingsPmc = (this.FunctionSettings) as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng;
            FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng testJobFunctionSettingsPmc = (testJob.FunctionSettings) as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng;            

            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint candJobStartAdd;
            uint JobStartAdd;
            uint candJobEndAdd;
            uint JobEndAdd;

            //BuiltInType bi = (BuiltInType)(Convert.ToInt32(testJob.TagsList[0].TagNode.DataType.Identifier));
            //uint tagByteSize = (uint)FanucCNCProtocol.GetDataTypeSize(FanucCNCProtocol.GetDataType(bi));

            uint elementsize = 1;
            if (thisFunctionSettingsPmc.IsBitAddress())
            {                
                Granularity *= 8;
                candJobStartAdd = (uint)((testJobFunctionSettingsPmc.AddressNumber * 8) + testJobFunctionSettingsPmc.AddressBitNumber);
                JobStartAdd = (uint)((thisFunctionSettingsPmc.AddressNumber * 8) + thisFunctionSettingsPmc.AddressBitNumber);
            }
            else
            {
                candJobStartAdd = (uint)testJobFunctionSettingsPmc.AddressNumber;
                JobStartAdd = (uint)thisFunctionSettingsPmc.AddressNumber;                
            }

            candJobEndAdd = candJobStartAdd + testJob.TotalJobSize / elementsize - 1;
            JobEndAdd = JobStartAdd + this.TotalJobSize / elementsize - 1;

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
                this.TagsList[0].ByteOffset = 0;
                if (thisFunctionSettingsPmc.IsBitAddress())
                {
                    this.TagsList[0].BitOffset = (uint)(thisFunctionSettingsPmc.AddressBitNumber);

                    testJob.TagsList[0].ByteOffset = (uint)(testJobFunctionSettingsPmc.AddressNumber - thisFunctionSettingsPmc.AddressNumber);
                    testJob.TagsList[0].BitOffset = (uint)(testJobFunctionSettingsPmc.AddressBitNumber);
                    ExtraBytes = (uint)(Math.DivRem((int)(candJobEndAdd - JobEndAdd), 8, out int bitRest));
                }
                else
                {
                    testJob.TagsList[0].ByteOffset = (uint)(testJobFunctionSettingsPmc.AddressNumber - thisFunctionSettingsPmc.AddressNumber);
                    ExtraBytes = (candJobEndAdd - JobEndAdd);
                }
                return JobAggregationType.JobAggregFits;
            }

            if (candJobStartAdd >= JobStartAdd && candJobStartAdd <= JobEndAdd + Granularity)
            {
                if (((candJobEndAdd - JobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize(thisFunctionSettingsPmc.IsBitAddress()))
                {
                    this.TagsList[0].ByteOffset = 0;
                    if (thisFunctionSettingsPmc.IsBitAddress())
                    {
                        this.TagsList[0].BitOffset = (uint)(thisFunctionSettingsPmc.AddressBitNumber);

                        testJob.TagsList[0].ByteOffset = (uint)(testJobFunctionSettingsPmc.AddressNumber - thisFunctionSettingsPmc.AddressNumber);
                        testJob.TagsList[0].BitOffset = (uint)(testJobFunctionSettingsPmc.AddressBitNumber);
                        ExtraBytes = (uint)(Math.DivRem((int)(candJobEndAdd - JobEndAdd), 8, out int bitRest));
                    }
                    else
                    {
                        testJob.TagsList[0].ByteOffset = (uint)(testJobFunctionSettingsPmc.AddressNumber - thisFunctionSettingsPmc.AddressNumber);
                        ExtraBytes = (candJobEndAdd - JobEndAdd);
                    }

                    return JobAggregationType.JobAggregForward;
                }
            }
            //uint startBkw;
            //if (JobStartAdd > Granularity)
            //    startBkw = JobStartAdd - Granularity;
            //else
            //    startBkw = 0;
            //if (candJobEndAdd >= startBkw && candJobEndAdd <= JobEndAdd)
            //    if (((JobEndAdd - candJobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize())
            //    {
            //        candJob.TagsList[0].ByteOffset = 0;
            //        ExtraBytes = (JobStartAdd - candJobStartAdd) * elementsize;
            //        return JobAggregationType.JobAggregBackward;
            //    }

            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new FanucCNCTag(tag.TagNode, tag.ByteOffset, tag.BitOffset);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).CalculateInternalParameters(TagsList,0,true);
                    _IsAggregateJob = true;
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new FanucCNCTag(tag.TagNode, tag.ByteOffset, tag.BitOffset);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)_FunctionSettings).CalculateInternalParameters(TagsList, 0, true);
                    _IsAggregateJob = true;

                    break;
                //case JobAggregationType.JobAggregBackward:
                //    foreach (var tag in TagsList)
                //        tag.ByteOffset += ExtraBytes;
                //    foreach (var tag in candJob.TagsList)
                //    {
                //        var t = new FanucCNCTag(tag.TagNode, tag.ByteOffset, 0);
                //        t.Value.Value = Utils.Clone(tag.Value.Value);
                //        TagsList.Add(t);
                //    }
                //    TotalJobSize += ExtraBytes;
                //    //Address = ((OmronFinsEthernetCommJob)candJob).Address;
                //    //AddressObj = ((OmronFinsEthernetCommJob)candJob).AddressObj;
                //    break;
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
            List<byte> outData = new List<byte>();
            //prepare a write request
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;

            lock (lockListObject)
            {
                if (TagsListToWrite.Count == 0)
                {
                    TagsListOnWriting.Clear();
                    return;
                }

                cand = TagsListToWrite[0];
                TagsListToWrite.Remove(cand);
                if (!TagsListOnWriting.Contains(cand))
                    TagsListOnWriting.Add(cand);
            }

            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                nData = (UInt16)((cand.Size + 7) / 8);
            }
            //else
            //if (ElementNumber > 0 && !ProtocolDataSizeBig())
            //{
            //    if (cand.TagNode.ArrayDimension == 0)
            //        nData = (ushort)(GetProtocolDataByteSize());
            //    else
            //        nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            //}
            else
            {
                nData = (UInt16)cand.Size;
            }
            lock (lockListObject)
            {
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
                            return;
                        }
                    }
                }

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

            outData.AddRange(jobdata);
            jobData = outData.ToArray();
        }


        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            lock (lockListObject)
            {
                //UInt16 sizeProtocolData = 1; // (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        //if (isProtocolBool())
                        //{
                        //    if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                        //        changed.Add(TagsList[TagIndex]);
                        //}
                        //else
                        //{
                            int bitRest;

                            uint arraySize = (TagsList[TagIndex].TagNode.ArrayDimension == 0 ? 1 : TagsList[TagIndex].TagNode.ArrayDimension);
                            // calculate the nr of bytes used by array of bit
                            int nrBytes = Math.DivRem((int)(TagsList[TagIndex].BitOffset + arraySize -1), 8, out bitRest);
                            if (bitRest > 0 || nrBytes == 0)
                                nrBytes++;

                            byte[] tmpData = new byte[arraySize];

                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, nrBytes);
                            for (int arrayIndex = 0; arrayIndex < arraySize; arrayIndex++)
                            {
                                //int ByteNr = Math.DivRem((int)(TagsList[TagIndex].BitOffset + ArrayIndex), 8, out BitRest);
                                //tmpData[ArrayIndex] = (byte)(TagsList[TagIndex].MemRW[ByteNr] & (1 << BitRest));

                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW(0, (int)TagsList[TagIndex].BitOffset + arrayIndex);
                                tmpData[arrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        //}
                    }
                    else
                    {
                        //if (isProtocolBool())
                        //{
                        //    if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                        //        changed.Add(TagsList[TagIndex]);
                        //}
                        //else
                        //{
                            // string data type
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
                                uint ArraySize = (TagsList[TagIndex].TagNode.ArrayDimension == 0 ? 1 : TagsList[TagIndex].TagNode.ArrayDimension);
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];

                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)ArraySize);
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + (ArrayIndex * sizeTmpData), tmpData, (ArrayIndex * sizeTmpData), sizeTmpData);
                                }

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        //}
                    }
                }

                FirstTime = false;
            }
        }
               
        //public void ResetDiagnRxTxBytes() {
        //    DiagnRxBytes  = 0;            
        //    DiagnTxBytes = 0;
        //}

        public override void ManageUpdatedValueForTheOffsetVariable(DataValue value)
        {
            base.ManageUpdatedValueForTheOffsetVariable(value);

            if (GetOffsetVariableIntNumericValue(out Int64 offsetValue))
            {
                if (_FunctionSettings != null)
                    _FunctionSettings.OffsetVariableValueChanged((int)offsetValue, this.TagsList);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the conditional variable of the job. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetConditionalVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }
            if (bitIndex > 31)
            {
                return (false);
            }

            //Check up if Conditional variable was inizialized
            if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
            {
                return (false);
            }

            object value = jobConditionalVariable.varValue.Value;

            if (value is Array)
            {
                return (false);
            }
            Type systemType = value.GetType();
            BuiltInType builtInType = Station.GetBuiltInType(systemType);
            uint uintValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        if (bitIndex > 0)
                        {
                            return (false);
                        }

                        bool boolValue = (bool)value;
                        if (boolValue == true)
                        {
                            uintValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        sbyte sbyteValue = (sbyte)value;
                        uintValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        byte byteValue = (byte)value;
                        uintValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        short shortValue = (short)value;
                        uintValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        ushort ushortValue = (ushort)value;
                        uintValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        if (bitIndex > 31)
                        {
                            return (false);
                        }

                        int intValue = (int)value;
                        uintValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.UInt32:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }

                    uintValue = (uint)value;
                    break;

                default:
                    return (false);
            }

            uint bitMask = (uint)Math.Pow(2, bitIndex);
            bitValue = false;
            if ((uintValue & bitMask) != 0)
            {
                bitValue = true;
            }

            return (true);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the conditional variable of the job. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetConditionalVariableBit(bool bitValue, UInt16 bitIndex, bool sendValueToMovicon = true)
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }
            bool bitCurrentValue = false;
            if (GetConditionalVariableBit(ref bitCurrentValue, bitIndex) == false)
            {
                return (false);
            }
            if (bitCurrentValue == bitValue)
            {
                return (true);
            }

            //Check up if Conditional variable was inizialized
            if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
            {
                return (false);
            }

            object currentValue = jobConditionalVariable.varValue.Value;

            if (currentValue is Array)
            {
                return (false);
            }

            Type systemType = currentValue.GetType();
            BuiltInType builtInType = Station.GetBuiltInType(systemType);
            uint uintValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        if (bitIndex > 0)
                        {
                            return (false);
                        }

                        bool boolValue = (bool)currentValue;
                        if (boolValue == true)
                        {
                            uintValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        sbyte sbyteValue = (sbyte)currentValue;
                        uintValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        byte byteValue = (byte)currentValue;
                        uintValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        short shortValue = (short)currentValue;
                        uintValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        ushort ushortValue = (ushort)currentValue;
                        uintValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        if (bitIndex > 31)
                        {
                            return (false);
                        }

                        int intValue = (int)currentValue;
                        uintValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.UInt32:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }

                    uintValue = (uint)currentValue;
                    break;

                default:
                    return (false);
            }

            uint uintNewValue = uintValue;
            uint bitMask = (uint)Math.Pow(2, bitIndex);
            if (bitValue == false)
            {
                uintNewValue &= ~bitMask;
            }
            else
            {
                uintNewValue |= bitMask;
            }

            //jobConditionalVariable.varValue = null;
            jobConditionalVariable.varValue = new DataValue(new Variant(uintNewValue));

            if (sendValueToMovicon)
                Station.GetCommDriver().OnTagChanged(jobConditionalVariable.varNodeId, jobConditionalVariable.varValue);

            return (true);
        }

        //private BuiltInType GetBuiltInType(Type systemType)
        //{
        //    throw new NotImplementedException();
        //}
        

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Checks if the value of the conditional variable is different from 0 (or false). </summary>
        ///
        /// <returns>   true if the value of the conditional variable is different from 0 or if the conditional variable has not been defined. </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsConditionalVariableOn()
        {
            bool returnValue = true;

            if (conditionalVariableHasBeenSet == true)
            {
                //Check up if Conditional variable was inizialized
                if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
                {
                    return (returnValue);
                }
                object value = jobConditionalVariable.varValue.Value;
                if ((value != null) && !(value is Array))
                {
                    uint uintValue = ConvertValueToUint(value);

                    if (FanucCNCProtocol.IsUploadDownloadFunctionCode(_FunctionCode)) {
                        returnValue = (((uintValue >> (byte)FanucCNCProtocol.ConditionalVariableState.ACTIVATION_REQUEST) & 1) == 1);
                    }
                    else
                    {
                        if (uintValue == 0)
                        {
                            returnValue = false;
                        }
                    }
                }
            }

            return (returnValue);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Resets the value of the conditional variable. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ResetConditionalVariable()
        {
            if (conditionalVariableHasBeenSet)
            {                
                if (FanucCNCProtocol.IsUploadDownloadFunctionCode(_FunctionCode))
                {                    
                    SetConditionalVariableBit(false, (int)FanucCNCProtocol.ConditionalVariableState.ACTIVATION_REQUEST, false);
                }
                else
                {
                    uint uintValue = 0;
                    jobConditionalVariable.varValue = new DataValue(new Variant(uintValue));
                }
                
                Station.GetCommDriver().OnTagChanged(jobConditionalVariable.varNodeId, jobConditionalVariable.varValue);
            }
        }

        public bool IsCNCPathChangeRequired(short currentCNCPath)
        {
            return (_CNCPath != 0 && _CNCPath != currentCNCPath);
        }

        public bool IsStructType()
        {
            return (_FunctionSettings != null && _FunctionSettings.IsStructType());
        }

        public bool IsStructOrAggregateJob()
        {
            return (_FunctionSettings != null && (_FunctionSettings.IsStructType() || _IsAggregateJob));
        }
        #endregion

        #region Properties
        private FanucCNCProtocol.FunctionCode _FunctionCode;
        public FanucCNCProtocol.FunctionCode FunctionCode
        {
            get { return _FunctionCode; }
            set { _FunctionCode = value; }
        }

        private FanucCNCDynTag_BaseFunction _FunctionSettings;
        public FanucCNCDynTag_BaseFunction FunctionSettings
        {
            get { return _FunctionSettings; }
            set { _FunctionSettings = value; }
        }

        private uint _StringLength;

        private short _CNCPath;
        public short CNCPath
        {
            get { return _CNCPath; }
            set { _CNCPath = value; }
        }

        bool _IsAggregateJob;

        //// for statistic use --> calculate rx bytes
        //public long DiagnRxBytes { set; get; } = 0;
        //// for statistic use --> calculate tx bytes
        //public long DiagnTxBytes { set; get; } = 0;
        #endregion
    }
}
