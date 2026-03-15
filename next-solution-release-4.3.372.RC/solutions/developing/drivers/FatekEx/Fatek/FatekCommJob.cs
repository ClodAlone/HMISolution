using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace Fatek
{
    public class FatekCommJob : CommJob
    {
        public enum CommandTypes
        {
            ReadCmd,
            WriteCmd,
            Invalid,
        }

        #region Constructors
        public FatekCommJob(Station station, FatekCommJobSettings settings)
            : base(station, settings)
        {
            _StartAddress = settings.StartAddress;
            FatekProtocol.SplitStartAddress(_StartAddress, out _Area, out _AreaAddress, out string errorCode);            
            //_SwapDWords = settings.SwapDWords;
            //_StringLength = settings.StringLength;
            _CommandType = CommandTypes.Invalid;

            CheckJobValid();
        }

        public FatekCommJob(Station station, FatekTag defTag)
            : base(station, defTag)
        {
            _StartAddress = defTag.FatekDynSettings.StartAddress;
            FatekProtocol.SplitStartAddress(_StartAddress, out _Area, out _AreaAddress, out string errorCode);
            //_SwapDWords = defTag.FatekDynSettings.SwapDWords;
            //_StringLength = defTag.FatekDynSettings.StringLength;
            _CommandType = CommandTypes.Invalid;

            if ((ElementNumber > 0 && !isProtocolBool()) || (ElementNumber >= 0 && ProtocolDataSizeBig()))
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            if (ElementNumber < 0 && ProtocolDataSizeBig())
            {
                uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    TotalJobSize = ((ArraySize + 15) / 16) * 2;
                else
                    TotalJobSize = ((ArraySize + 1) / 2) * 2;

            }

            if (TagsList.Count>0)
                ((FatekTag)TagsList[0]).TotalJobSize = TotalJobSize;

            CheckJobValid();
        }

        public FatekCommJob(Station station)
            : base(station)
        {                        
            CheckJobValid();
        }

        protected FatekCommJob()
        {
            CheckJobValid();
        }
        #endregion

        #region Static methods        
        #endregion

        #region override Methods

        public override bool ProtocolDataSizeIsValid(out string errorDesc)
        {
            errorDesc = null;
            if (ProtocolDataSizeBig())
            {
                if (ElementNumber > (GetProtocolDataBitSize() / GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) - 1))
                    errorDesc = "ElementNumber";
            }
            if (errorDesc == null)
                return true;
            else
                return false;
        }

        public override bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return base.InputOutputRequiresReadAfterContinuosWriteBase();
        }

        #endregion

        #region Methods
        private void CheckJobValid()
        {                
            bool bit = false;
            bool num = false;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!FatekProtocol.IsTypeAdmitted(this.Area, t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
            }

            //tag must be all bit or not
            if (bit && num)
            {
                IsValid = false;
                InvalidReason = string.Format("The Tags type are inconsistent for the Function Code. (Tags: {0} F.Code: {1})", tagnamelist, _StartAddress);
                return;
            }            

            //if (!ProtocolDataSizeIsValid(out string errorDesc))
            //{
            //    IsValid = false;
            //    InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, _StartAddress);
            //    return;
            //}

            if (CalculateTotalBytesJobSize(bit) > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        private uint GetAggregateMaxJobSize(bool bitArea) //uint totalJobSizeCorr)//Boolean bitArea, object candJobDataType)
        {
            uint aggregLimit = Station.GetCommDriver().AggregationLimit;
            
            uint jobMaxSize = GetMaxJobSize();
            if (bitArea)
                jobMaxSize *= 8;

            //// manage only register area to bit size conversion
            //if (bitArea)
            //{
            //    // for data type bigger the 1 bit, set parameter that correct TotalJobSize from byte to bit
            //    if ((uint)candJobDataType != (uint)BuiltInType.Boolean)
            //        jobMaxSize = jobMaxSize * 16;
            //}

            if ((aggregLimit != 0) && (aggregLimit < jobMaxSize))
                return aggregLimit;
            else
                return jobMaxSize;
        }
        
        public override uint getProtocolDataType()
        {
            if (FatekProtocol.IsBitDataArea(_Area))            
                return (uint)BuiltInType.Boolean;
            else
            {
                //return (uint)BuiltInType.Int16;
                switch (ElementNumber)
                {
                    case 2:
                        return ((uint)BuiltInType.UInt32);
                    case 4:
                        return ((uint)BuiltInType.UInt64);
                    default:
                        return (uint)BuiltInType.UInt16;
                }
            }
        }

        private uint CalculateTotalBytesJobSize(bool bit)
        {            
            // for data type bigger the 1 bit, set parameter that correct TotalJobSize from byte to bit
            //if ((uint)TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
            if (bit)
            {
                uint tot = (uint)Math.DivRem((int)TotalJobSize, 8, out int bitRest);
                if (bitRest > 0)
                    tot++;

                return tot;
            }            
            else
            {
                return TotalJobSize;
            }
        }

        public override uint GetMaxJobSize()
        {
            return FatekProtocol.GetMaxJobSize(Area);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint extraBytes)
        {
            extraBytes = 0;

            FatekCommJob testJob = candJob as FatekCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (base.TestAggregateJob(candJob, out extraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            if (this.Area != testJob.Area)
                return JobAggregationType.JobAggregImpossible;

            if (this.TagsList[0].TagNode.ArrayDimension != 0 || testJob.TagsList[0].TagNode.ArrayDimension != 0)
                return JobAggregationType.JobAggregImpossible;

            // ElementNumber can be used to read more data elements ( >=1 ) so don't allow aggregations 
            // because creating a new job only has one ElementNumber property   
            if (ElementNumber != candJob.ElementNumber)
                return JobAggregationType.JobAggregImpossible;

            uint granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = (uint)AreaAddress;
            uint startTest = (uint)testJob.AreaAddress;
            
            bool bitArea = FatekProtocol.IsBitDataArea(this.Area);
            uint elementsize = 2;

            if (bitArea)
            {
                elementsize = 1;                
            }

            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;

            // Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
            foreach (var tag in TagsList)
            {
                uint startTag = startTest + tag.ByteOffset / elementsize;
                //uint endTag = startTag + (tag.Size / elementsize) - 1;
                uint endTag = startTag + (((FatekTag)TagsList[0]).TotalJobSize / elementsize) - 1;
                if ((start >= startTag && start <= endTag) || (end >= startTag && end <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }
            
            // If the address range of the job is strictly included in the range of the
            // tag, the job cannot aggregate. 
            if (startTest < start && endTest > end)
            {
                return JobAggregationType.JobAggregImpossible;
            }           

            // AggregFits --> inside
            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) * elementsize;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset = newoffset;
                    newoffset += candJob.TagsList[i].Size;
                }
                return JobAggregationType.JobAggregFits;
            }

            // AggregForward --> after
            if (startTest >= start && startTest <= end + granularity)
            {
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize(bitArea))
                {
                    //candJob.TagsList[0].ByteOffset = ((candJobStartAdd - jobStartAdd) / totalJobSizeCorr) * elementsize;
                    //extraBytes = ((candJobEndAdd - jobEndAdd) / totalJobSizeCorr) * elementsize;
                    uint newoffset = (startTest - start) * elementsize;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    extraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }
            }

            uint startBkw = 0;
            if (start > granularity)
                startBkw = start - granularity;
           
            // AggregBackward --> before
            if (endTest >= startBkw && endTest <= end)
            {
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize(bitArea))// totalJobSizeCorr)) //bitArea, candJobDataType))
                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    extraBytes = (start - startTest) * elementsize;

                    return JobAggregationType.JobAggregBackward;
                }
            }
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new FatekTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }                    
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new FatekTag(tag.TagNode, tag.ByteOffset, 0);
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
                        var t = new FatekTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    StartAddress = ((FatekCommJob)candJob).StartAddress;
                    AreaAddress = ((FatekCommJob)candJob).AreaAddress;
                    break;
                default:
                    return false;
            }
            return true;
        }
        
        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();
            Tag startTag = null;
            List<byte> outData = new List<byte>();
            
            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
                //TagsListToWrite.Clear();
                startTag = listToWrite[0];
                //}

                //List<byte> outData = new List<byte>();
                //prepare a write request
                listToWrite.Sort(CompareTagByOffset);
                Tag cand = null;
                byte[] jobdata;
                UInt16 nData = 0;
                bool bLookForFirstTag = true;
                do
                {
                    if (cand != null)
                    {
                        if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                            break;
                    }
                    cand = listToWrite[0];
                    listToWrite.Remove(cand);

                    if (bLookForFirstTag && cand != startTag)
                    {
                        cand = null;
                        continue;
                    }
                    bLookForFirstTag = false;

                    if (!listOnWriting.Contains(cand))
                        listOnWriting.Add(cand);
                    if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        nData = (UInt16)((cand.Size + 7) / 8);
                    else if ((ElementNumber > 0 && !ProtocolDataSizeBig()))
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

                    lock (lockListObject)
                    {
                        jobdata = new byte[nData];
                        cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                    }

                    uint ArraySize = cand.TagNode.ArrayDimension;
                    if (ArraySize == 0)
                        ArraySize = 1;
                    if (ProtocolDataSizeBig())
                    {
                        List<byte> correctData = new List<byte>();
                        if (ElementNumber >= 0)
                        {
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
                        }
                        else
                        {
                            UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                            if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                            {
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    byte[] tmpdata = new byte[sizeDataType];
                                    Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                                    correctData.AddRange(tmpdata);
                                }
                            }
                            else
                            {
                                byte[] tmpdata = new byte[TotalJobSize];
                                Array.Copy(jobdata, tmpdata, TotalJobSize);
                                correctData.AddRange(tmpdata);
                            }
                            if ((correctData.Count & 1) == 1)
                                correctData.Add(0);
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

                } while (listToWrite.Count > 0);

                //lock (lockListObject)
                //{

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();
                listToWrite.Clear();

                //if (listToWrite.Count > 0)
                //{
                //    var tempListToWrite = new List<Tag>();
                //    tempListToWrite.AddRange(TagsListToWrite);
                //    TagsListToWrite.Clear();
                //    listToWrite.ForEach((tag) =>
                //    {
                //        if (!TagsListToWrite.Contains(tag))
                //        {
                //            TagsListToWrite.Add(tag);
                //        }
                //    });
                //    listToWrite.Clear();
                //    tempListToWrite.ForEach((tag) =>
                //    {
                //        if (!TagsListToWrite.Contains(tag))
                //        {
                //            TagsListToWrite.Add(tag);
                //        }
                //    });
                //    tempListToWrite.Clear();
                //}
                //}
            }
            //if (isProtocolBool())
            //{
            //    byte[] tmpData = new byte[(outData.Count() + 7) / 8];
            //    for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
            //    {
            //        if (outData[ArrayIndex] != 0)
            //            tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
            //    }
            //    jobData = tmpData;
            //}
            //else
            byte[] Data = outData.ToArray();
            if (SwapBytes)
                SwapByteBuffer(ref Data);
            if (SwapWords)
                SwapWordBuffer(ref Data);
            if (Data.Length > 0)
            {
                jobData = Data;
            }
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
                            if (((FatekTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];

                            if (ElementNumber >= 0)
                            {
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    tmpData[ArrayIndex] = (byte)(TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber) ? 1 : 0);
                                }
                            }
                            else
                            {
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)TotalJobSize);
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    tmpData[ArrayIndex] = (byte)(TagsList[TagIndex].getBoolValueFromMemRW(0, ArrayIndex) ? 1 : 0);
                                }
                            }
                            if (((FatekTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (((FatekTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
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
                                if (((FatekTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    changed.Add(TagsList[TagIndex]);
                            }
                            else
                            {
                                byte[] tmpData;
                                if (ElementNumber >= 0)
                                {
                                    UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                    uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                    if (ArraySize == 0)
                                        ArraySize = 1;
                                    tmpData = new byte[sizeTmpData * ArraySize];
                                    int indexTmpData = 0;
                                    TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                    {
                                        Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                        indexTmpData += sizeTmpData;
                                    }
                                }
                                else
                                {
                                    tmpData = new byte[TotalJobSize];
                                    TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, tmpData.Length);
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset, tmpData,0, TotalJobSize);
                                }

                                if (((FatekTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
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
                        return -1;

                }
            }
        }
#endregion

#region Properties               
private string _StartAddress;
        public string StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        private FatekProtocol.DataArea _Area;
        public FatekProtocol.DataArea Area
        {
            get { return _Area; }
            set { _Area = value; }
        }

        private ushort _AreaAddress;
        public ushort AreaAddress
        {
            get { return _AreaAddress; }
            set { _AreaAddress = value; }
        }

        private CommandTypes _CommandType;
        public CommandTypes CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        /// <summary>   true to swap dwords. </summary>
        //private bool _SwapDWords;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the swap dwords. </summary>
        ///
        /// <value> true if swap dwords, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool SwapDWords
        //{
        //    get { return _SwapDWords; }
        //    set
        //    {
        //        _SwapDWords = value;
        //    }
        //}

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}A{1:00}", ret, (uint)Area);
            }
        }

        ///// <summary>   The String Length. </summary>
        //private uint _StringLength;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   String Length Property. </summary>
        /////
        ///// <value> The String Length. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public uint StringLength
        //{
        //    get { return _StringLength; }
        //    set { _StringLength = value; }
        //}

        #endregion
    }
}
