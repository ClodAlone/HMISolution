using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace SaiaDataMode
{
    public class SaiaDataModeCommJob : CommJob
    {
        #region Constructors

        public SaiaDataModeCommJob(Station station, SaiaDataModeCommJobSettings settings)
            : base(station, settings)
        {
            _StartAddress = new ushortUnion(settings.StartAddress);
            _AreaType = settings.AreaType;
            _DbNumber = new ushortUnion( settings.DbNumber);
            _DataConversionType = settings.DataConversionType;
            _RemoveFromPendingJob = false;

            CheckJobValid();
        }

        public SaiaDataModeCommJob(Station station, SaiaDataModeTag defTag)
            : base(station, defTag)
        {
            _StartAddress = new ushortUnion(defTag.SaiaDataModeDynSettings.StartAddress);
            _AreaType = defTag.SaiaDataModeDynSettings.AreaType;
            _DbNumber = new ushortUnion (defTag.SaiaDataModeDynSettings.DbNumber);
            _DataConversionType = defTag.SaiaDataModeDynSettings.DataConversionType;
            _RemoveFromPendingJob = false;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if(defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public SaiaDataModeCommJob(Station station)
            : base(station)
        {
            _StartAddress = new ushortUnion(0);
            _DbNumber = new ushortUnion(0);
            _AreaType = AreaTypes.Inputs;
            _DataConversionType = DataConversionTypes.None;
            _RemoveFromPendingJob = false;

            CheckJobValid();
        }

        protected SaiaDataModeCommJob()
        {
            _StartAddress = new ushortUnion(0);
            _DbNumber = new ushortUnion(0);
            _AreaType = AreaTypes.Inputs;
            _DataConversionType = DataConversionTypes.None;
            _RemoveFromPendingJob = false;

            CheckJobValid();
        }
        #endregion

        #region data Member
        

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

        public byte GetCommand()
        {
            if (ReadRequest())
            {
                switch (AreaType)
                {
                    case AreaTypes.Inputs:
                        return 3;
                    case AreaTypes.Outputs:
                        return 5;
                    case AreaTypes.Flags:
                        return 2;
                    case AreaTypes.Registers:
                        return 6;
                    case AreaTypes.Timers:
                        return 7;
                    case AreaTypes.Counters:
                        return 0;
                    case AreaTypes.DataBlock:
                        return 150;
                    default:
                        return 6;
                }
            }
            else
            {
                switch (AreaType)
                {
                    case AreaTypes.Inputs:
                        return 0;
                    case AreaTypes.Outputs:
                        return 13;
                    case AreaTypes.Flags:
                        return 11;
                    case AreaTypes.Registers:
                        return 14;
                    case AreaTypes.Timers:
                        return 15;
                    case AreaTypes.Counters:
                        return 10;
                    case AreaTypes.DataBlock:
                        return 151;
                    default:
                        return 0;
                }
            }
        }

        public byte GetQuantity(bool GestReadCommand,uint bufferWriteSize)
        {
            if (GestReadCommand)
            {
                if (IsBitDataFormat())
                {
                    if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || ElementNumber > 0)
                    {
                        return (byte)(TotalJobSize - 1);
                    }
                    else
                        return (byte)(TotalJobSize * 8 - 1);
                }
                else
                {
                    if (TotalJobSize > 4)
                    {
                        return (byte)((TotalJobSize - 4) / 4 );
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                if (IsBitDataFormat())
                {
                    return (byte)(bufferWriteSize + 2);
                }
                else
                {
                    if (AreaType == AreaTypes.DataBlock)
                    {
                        return (byte)(bufferWriteSize + 3);
                    }
                    else
                    {
                        return (byte)(bufferWriteSize + 1);
                    }
                }
            }
        }

        public ushort GetWriteAddres()
        {
            TagsListToWrite.Sort(CompareTagByOffset);
            if (IsBitDataFormat())
            {
                if ((uint)TagsListToWrite[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ||
                    ElementNumber != 0)
                {
                    return (ushort)(StartAddress.USHORT + TagsListToWrite[0].ByteOffset);
                }
                else
                {
                    return (ushort)(StartAddress.USHORT + TagsListToWrite[0].ByteOffset * 8);
                }
            }
            else
            {
                return (ushort)(StartAddress.USHORT + TagsListToWrite[0].ByteOffset / 4);
            }
        }
    
        public bool IsBitDataFormat()
        {
            return (AreaType < AreaTypes.Registers);
        }

        public bool IsOnlyInput()
        {
            return (AreaType == AreaTypes.Inputs);
        }
        
        public byte GetExpectedDataLength(bool GestReadCommand)
        {
            if (GestReadCommand)
            {
                if (IsBitDataFormat() &&
                    (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || ElementNumber > 0))
                    return (byte)((TotalJobSize + 7) / 8);
                else
                    return (byte)(TotalJobSize);
            }
            else
            {
                return SaiaDataModeProtocol.WriteReplyLen;
            }
        }
        public byte GetWriteBitQuantity()
        {
            byte ArrayDimension = (byte)(TagsListToWrite[0].TagNode.ArrayDimension == 0 ? 1 : TagsListToWrite[0].TagNode.ArrayDimension);
            if ((uint)TagsListToWrite[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ||
                ElementNumber > 0)
            {
                return (byte)(TagsListToWrite.Count * ArrayDimension - 1);
            }
            else
            {
                return (byte)(TagsListToWrite.Count * GetDataTypeBitSize((uint)TagsListToWrite[0].TagNode.DataType.Identifier) * ArrayDimension - 1);
            }
        }

        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                return GetDataTypeSize((uint)(t.DataType.Identifier));
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

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the AreaType. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, AreaType);
                return;
            }

            // Inputs can only be read
            if (IsOnlyInput() && Type != LinkType.Input)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorInputTypeRequired,
                    tagnamelist);
                return;
            }

            // Check the total size of the variables.
            int ElemSize = (IsBitDataFormat()) ? 1 : 4;

            if (TotalJobSize < ElemSize || TotalJobSize % ElemSize != 0)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorValidateJob,
                    tagnamelist,
                    Properties.Resources.ErrorTotalBytesNoMatch);
                return;
            }
            uint byteSize ;
            if (IsBitDataFormat() &&
                (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || ElementNumber > 0))
                byteSize = (uint)((TotalJobSize + 7) / 8);
            else
                byteSize = TotalJobSize;


            if (byteSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
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
            if (_AreaType < AreaTypes.Registers)
                return (uint)BuiltInType.Boolean;
            else
                return (uint)BuiltInType.UInt32;
        }

        public override uint GetMaxJobSize()
        {
            return SaiaDataModeProtocol.GetMaxJobSize(_AreaType);
        }
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            SaiaDataModeCommJob testJob = candJob as SaiaDataModeCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (testJob.AreaType != AreaType)
                return JobAggregationType.JobAggregImpossible;

            if (testJob.TagsList[0].TagNode.ArrayDimension != TagsList[0].TagNode.ArrayDimension)
                return JobAggregationType.JobAggregImpossible;

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                if (TagsList[0].TagNode.ArrayDimension % 8 != 0 )
                    return JobAggregationType.JobAggregImpossible;
            }
 
            if ( GetTagSize(TagsList[0].TagNode)  !=  GetTagSize(testJob.TagsList[0].TagNode) )
                return JobAggregationType.JobAggregImpossible;



            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted(testJob.TagsList[0].TagNode.DataType)) 
                return JobAggregationType.JobAggregImpossible;

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Float ^ 
                (uint)testJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Float )
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ^ 
                (uint)testJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                return JobAggregationType.JobAggregImpossible;
            }
            
            ExtraBytes = 0;
            uint elementsize = (uint)(IsBitDataFormat() ? 1 : 4);

            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            if (IsBitDataFormat() && ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ||
                ElementNumber > 0))
            {
                Granularity *= 8;
            }
            uint start = StartAddress.USHORT;
            uint startTest = testJob.StartAddress.USHORT;

            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / elementsize;
                uint endTag = startTag + (tag.Size / elementsize) - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                candJob.TagsList[0].ByteOffset = (startTest - start) * elementsize;
                return JobAggregationType.JobAggregFits;
            }

            //if (startTest >= start && endTest <= end + Granularity)
            if (startTest >= start && startTest <= end + Granularity)
                //if (((endTest - startTest + 1) * elementsize) <= GetMaxJobSize())
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = (startTest - start) * elementsize;
                    ExtraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            //if (startTest >= startBkw && endTest <= end)
            if (endTest >= startBkw && endTest <= end)
                //if (((endTest - startBkw + 1) * elementsize) <= GetMaxJobSize())
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = 0;
                    ExtraBytes = (start - startTest) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new SaiaDataModeTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    break;
                case JobAggregationType.JobAggregForward:
                    TagsList.Add(new SaiaDataModeTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                        tag.ByteOffset += ExtraBytes;
                    TagsList.Add(new SaiaDataModeTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    TotalJobSize += ExtraBytes;
                    StartAddress = ((SaiaDataModeCommJob)candJob).StartAddress;
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
                if (TagsListToWrite.Count == 0)
                {
                    TagsListOnWriting.Clear();
                    return;
                }
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

                if (isProtocolBool())
                {
                    outData.AddRange(jobdata);
                }
                else
                {
                    if (SwapBytes)
                    {
                        SwapByteBuffer(ref jobdata);
                    }

                    if (SwapWords)
                    {
                        SwapWordBuffer(ref jobdata);
                    }
                    byte[] tmpdata = new byte[jobdata.Count()];
                    for (int i = 0; (i + 3) < jobdata.Count(); i += 4)
                    {
                        tmpdata[i] = jobdata[i + 3];
                        tmpdata[i + 1] = jobdata[i + 2];
                        tmpdata[i + 2] = jobdata[i + 1];
                        tmpdata[i + 3] = jobdata[i];
                        if (AreaType == AreaTypes.Registers &&
                            DataConversionType == DataConversionTypes.FFP)
                        {
                            floatUnion Float = new floatUnion(tmpdata, 0);
                            uintUnion Result = new uintUnion(ConvertIEEEToFFP(Float.FLOAT));
                            tmpdata[i] = Result.HIUSHORT.HIBYTE;
                            tmpdata[i + 1] = Result.HIUSHORT.LOBYTE;
                            tmpdata[i + 2] = Result.LOUSHORT.HIBYTE;
                            tmpdata[i + 3] = Result.LOUSHORT.LOBYTE;
                        }
                    }
                    outData.AddRange(tmpdata);
                }

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

            // no data to write --> return null
            if (outData.Count == 0)
            {
                jobData = null;
                return;
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
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + ElementNumber * sizeTmpData + sizeProtocolData * ArrayIndex, tmpData, indexTmpData, sizeTmpData);
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

        #endregion

        #region Properties
        private ushortUnion _StartAddress;
        public ushortUnion StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }
        private AreaTypes _AreaType;
        public AreaTypes AreaType
        {
            get
            {
                return _AreaType;
            }
            set
            {
                _AreaType = value;
            }
        }

        private ushortUnion _DbNumber;
        public ushortUnion DbNumber
        {
            get
            {
                return _DbNumber;
            }
            set
            {
                _DbNumber = value;
            }
        }

        private DataConversionTypes _DataConversionType;
        public DataConversionTypes DataConversionType
        {
            get
            {
                return _DataConversionType;
            }
            set
            {
                _DataConversionType = value;
            }
        }

        /// <summary>
        /// Mark job to be removed from main channel PendingJobs list
        /// </summary>
        private bool _RemoveFromPendingJob;
        public bool RemoveFromPendingJob
        {
            get
            {
                return _RemoveFromPendingJob;
            }
            set
            {
                _RemoveFromPendingJob = value;
            }
        }
        #endregion

        #region Methods Float Conversion


        //FFP-Floating Point to support

        bool SetBit4IEEE754(ref floatUnion source, int StartBit, uint value)
        {
            bool Result = true;
            uint working = 0;
            if (StartBit < 0 || StartBit > 32)
                Result = false;
            else
            {
                working = value << StartBit;
                source.UINTUNION.UINT = source.UINTUNION.UINT | working;
            }
            return Result;
        }

        uint GetMantissaIEEE(floatUnion source)
        {
            uint Result = 0;
            int StartBit = 0;
            for (int i = 0; i < 23; i++)
            {
                if ((StartBit + i) < 32 && CheckBitSet(source, StartBit + i))
                    Result += (uint)(1 << (i));
            }
            Result += (1 << (23));
            return Result;
        }

        byte GetExponentsIEEE(floatUnion source)
        {
            byte Result = 0;
            int StartBit = 23;
            for (byte i = 0; i < sizeof(byte) * 8; i++)
            {
                if ((StartBit + i) < 32 && CheckBitSet(source, StartBit + i))
                {
                    Result += (byte)(1 << (i));
                }
            }
            return Result;
        }

        bool CheckBitSet(floatUnion source, int BitNumber)
        {
            bool Result = false;
            if (BitNumber < 32 && BitNumber > -1)
            {
                uint L = 1;
                if ((source.UINTUNION.UINT & (L << BitNumber)) > 0)
                    Result = true;
                else
                    Result = false;
            }
            return Result;
        }
        bool GetBit(floatUnion source, int StartBit)
        {
            bool bResult = false;
            if (CheckBitSet(source, StartBit))
                bResult = true;
            return bResult;
        }

        bool GetSignIEEE(floatUnion source)
        {
            return GetBit(source, 31);
        }

        uint ConvertIEEEToFFP(float ieeeSource)
        {
            floatUnion Result = new floatUnion(ieeeSource);
            int sign = GetSignIEEE(Result) ? -1 : 1;
            byte exponent = GetExponentsIEEE(Result);
            uint m = GetMantissaIEEE(Result);
            Result = new floatUnion(0);
            if (ieeeSource != 0.0)
            {
                SetBit4IEEE754(ref Result, 0, (uint)(exponent - 64 + 2));
                SetBit4IEEE754(ref Result, 7, (uint)(sign > 0 ? 0 : 1));
                SetBit4IEEE754(ref Result, 8, m);
            }
            return Result.UINTUNION.UINT;
        }

        //FFP format 
        bool GetSignFFP(uint source)
        {
            return GetBit(new floatUnion(source), 7);
        }
        byte GetExponentsFFP(uint source)
        {
            byte Result = 0;
            int StartBit = 0;
            for (int i = 0; i < sizeof(byte) * 7; i++)
            {
                if ((StartBit + i) < 32 && CheckBitSet(new floatUnion(source), StartBit + i))
                {
                    Result += (byte)(1 << (i));
                }
            }
            return Result;
        }
        uint GetMantissaFFP(uint source)
        {
            uint Result = 0;
            int StartBit = 8;
            for (int i = 0; i < 24; i++)
            {
                if ((StartBit + i) < 32 && CheckBitSet(new floatUnion(source), StartBit + i))
                    Result += (uint)(1 << (i));
            }
            return Result;
        }

        public float ConvertFFPToIEEE(uint fpSource)
        {
            float Result = 0;
            if (fpSource == 0)
                return Result;
            int sign = GetSignFFP(fpSource) ? -1 : 1;
            double exponent = (double)(GetExponentsFFP(fpSource) - 64);
            double m = (double)(GetMantissaFFP(fpSource)) / (double)0x1000000; //for FFP formula
            Result = (float)(sign * Math.Pow(2.0, exponent) * m);
            return Result;
        }

        #endregion

    }
}
