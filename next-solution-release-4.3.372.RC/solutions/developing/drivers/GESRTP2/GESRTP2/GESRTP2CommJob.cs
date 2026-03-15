////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2CommJob.cs
//
// summary:	Implements the driver GESRTP2 communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace GESRTP2
{
    /// <summary>   Protocol's task of the GESRTP2 driver. </summary>
    public class GESRTP2CommJob : CommJob
    {
        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the GESRTP2CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="settings"> set with an object of type GESRTP2CommJobSettings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2CommJob(Station station, GESRTP2CommJobSettings settings)
            : base(station, settings)
        {
            _AreaType = settings.AreaType;
            _StartAddress = settings.StartAddress;
            _StringLength = settings.StringLength;

            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the GESRTP2CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="defTag">   set with an object of type GESRTP2Tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2CommJob(Station station, GESRTP2Tag defTag)
            : base(station, defTag)
        {
            _AreaType = defTag.GESRTP2DynSettings.AreaType;
            _StartAddress = defTag.GESRTP2DynSettings.StartAddress;
            _StringLength = defTag.GESRTP2DynSettings.StringLength;

            if (TotalJobSize == 0)
            {
                if (GetProtocolDataByteSize() == 1)
                    TotalJobSize = (uint)defTag.GESRTP2DynSettings.StringLength;
                else
                    TotalJobSize = (uint)((defTag.GESRTP2DynSettings.StringLength + 1) / 2) * 2;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the GESRTP2CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2CommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        /// <summary>   Initializes the GESRTP2CommJob. </summary>
        protected GESRTP2CommJob()
        {
            CheckJobValid();
        }
        #endregion
        #region data Member

        

        #endregion

        #region Static methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if NodeId type is admitted. </summary>
        ///
        /// <param name="type"> . </param>
        ///
        /// <returns>   true if type admitted, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                nType == (uint)BuiltInType.String ||
                nType == (uint)BuiltInType.UInteger
                )
                    return true;
                return false;
            }
            
            return false;
        }
        #endregion
        
        #region Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   return tag object memory size. </summary>
        ///
        /// <param name="t">    . </param>
        ///
        /// <returns>   The tag size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch((uint)t.DataType.Identifier)
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
                            return (uint)StringLength;
                        else
                            return (uint)((StringLength + 1) / 2) * 2;
                    default:
                        return 0;
                }            
            }
            return 0;
        }

        /// <summary>   Test if GESRTP2CommJob object is valid. </summary>
        private void CheckJobValid()
        {
                
            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            bool bit = false;
            bool num = false;
            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }

                if (GESRTP2Protocol.InvalidAreaTypeLinkType(_AreaType, Type))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                    return;
                }

                if (GESRTP2Protocol.InvalidAreaDataType(_AreaType, t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format(UFUAModel.Properties.Resources.DataTypeIncompatible);
                    return;
                }

                if (_StartAddress == 0)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.InvalidZeroStartAdd);
                    return;
                }
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
                size += GetTagSize(t);
            }

            //tag must be all bit or not
            if ((bit && num))
            {
                IsValid = false;
                InvalidReason = string.Format("The Tags type are inconsistent for the Function Code. (Tags: {0} Area Code: {1})", tagnamelist, AreaType);
                return;
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the AreaType. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, AreaType);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String)
            {
                if (TotalJobSize > GESRTP2Protocol.MAX_STRING_SIZE)
                {
                    IsValid = false;
                    InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                    return;
                }
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return maximum memory size for aggregation of GESRTP2CommJob objects.
        /// </summary>
        ///
        /// <returns>   The aggregate maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
            //switch (GESRTP2Protocol.DataType(AreaType))
            //{
            //    case UFUAModel.DataType.Byte:
            //        return (uint)BuiltInType.Byte;
            //    case UFUAModel.DataType.UInt16:
            //        return (uint)BuiltInType.UInt16;
            //    case UFUAModel.DataType.Boolean:
            //        return (uint)BuiltInType.Boolean;
            //    default:
            //        return 0;
            //}
            switch (AreaType)
            {
                case AreaTypes.DiscreteBytes_SA:
                case AreaTypes.DiscreteBytes_SB:
                case AreaTypes.DiscreteBytes_SC:
                case AreaTypes.DiscreteInputBytes_I:
                case AreaTypes.DiscreteInternalBytes_M:
                case AreaTypes.DiscreteOutputBytes_Q:
                case AreaTypes.DiscreteTemporaryBytes_T:
                case AreaTypes.GeniusGlobalDataBytes_G:
                case AreaTypes.DiscreteBytes_S_readOnly:
                    return (uint)BuiltInType.Byte;
                case AreaTypes.AnalogInputWords_AI:
                case AreaTypes.AnalogOutputWords_AQ:
                case AreaTypes.RegisterWords_R:
                    return (uint)BuiltInType.UInt16;
                case AreaTypes.DiscreteBits_SA:
                case AreaTypes.DiscreteBits_SB:
                case AreaTypes.DiscreteBits_SC:
                case AreaTypes.DiscreteInputBits_I:
                case AreaTypes.DiscreteInternalBits_M:
                case AreaTypes.DiscreteOutputBits_Q:
                case AreaTypes.DiscreteTemporaryBits_T:
                case AreaTypes.GeniusGlobalDataBits_G:
                case AreaTypes.DiscreteBits_S_readOnly:
                    return (uint)BuiltInType.Boolean;
                default:
                    return 0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of GESRTP2CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetMaxJobSize()
        {
            return GESRTP2Protocol.GetMaxJobSize();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if it can be aggregated candJob. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="ExtraBytes">   [out] additional byte size for the aggregation. </param>
        ///
        /// <returns>   A JobAggregationType. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            GESRTP2CommJob testJob = candJob as GESRTP2CommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (AreaType != testJob.AreaType)
                return JobAggregationType.JobAggregImpossible;

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            if (testJob.TagsList[0].TagNode.DataType == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;


            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;

            if (GESRTP2Protocol.DataType(AreaType) == UFUAModel.DataType.Boolean)
            {
                Granularity *= 8;
            }


            uint start = StartAddress * elementSize;
            uint startTest = testJob.StartAddress * elementSize;
            uint end = start + TotalJobSize - 1;
            uint endTest = startTest + testJob.TotalJobSize - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset ;
                uint endTag = startTag + tag.Size - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) ;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset = newoffset;
                    newoffset += candJob.TagsList[i].Size;
                }
                return JobAggregationType.JobAggregFits;
            }

            if (startTest >= start && startTest <= end + Granularity)
                if (endTest - start + 1 <= GetAggregateMaxJobSize())
                {
                    uint newoffset = (startTest - start) ;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = endTest - end ;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
                if (end - startTest + 1 <= GetAggregateMaxJobSize())
                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = start - startTest;
                    return JobAggregationType.JobAggregBackward;
                }

            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Aggregate candJob as specified by AggType and manage ExtraBytes. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="AggType">      . </param>
        /// <param name="ExtraBytes">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new GESRTP2Tag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new GESRTP2Tag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }

                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in candJob.TagsList)
                    {
                        tag.ByteOffset += ExtraBytes;
                        var t = new GESRTP2Tag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    StartAddress = ((GESRTP2CommJob)candJob).StartAddress;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   write the GESRTP2CommJob's tags with JobData. </summary>
        ///
        /// <param name="jobData">  data buffer to write. </param>
        /// <param name="changed">  [in,out] out list of changed tags. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                            if (((GESRTP2Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
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

                            if (((GESRTP2Tag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (((GESRTP2Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
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
                                if (((GESRTP2Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
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

                                if (((GESRTP2Tag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by offset. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        /// <summary>   The Area Type. </summary>
        private AreaTypes _AreaType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Memory area Property. </summary>
        ///
        /// <value> The Area Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public AreaTypes AreaType
        {
            get { return _AreaType; }
            set
            {
                _AreaType = value;
            }
        }

        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        /// <summary>   read Data. </summary>
        private List<byte> _readData = new List<byte>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Bufer for read data. </summary>
        ///
        /// <value> The read data Bufer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<byte> readData
        {
            get { return _readData; }
            set
            {
                _readData = value;
            }
        }

        /// <summary>   read/write Data. </summary>
        private List<byte> _writeData = new List<byte>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Bufer for write data. </summary>
        ///
        /// <value> The write data Bufer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<byte> writeData
        {
            get { return _writeData; }
            set
            {
                _writeData = value;
            }
        }

        /// <summary>   Write offset Data. </summary>
        private ushort _offset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   offset for write data. </summary>
        ///
        /// <value> The write data offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort writeOffset
        {
            get { return _offset; }
            set
            {
                _offset = value;
            }
        }

        /// <summary>   Write offset Data. </summary>
        private bool _onWrite = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   offset for write data. </summary>
        ///
        /// <value> The write data offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool onWrite
        {
            get { return _onWrite; }
            set
            {
                _onWrite = value;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String for grouping the GESRTP2CommJobs Property. </summary>
        ///
        /// <value> The group string. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}AT{1:00}", ret, (uint)AreaType);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   elementSize data. </summary>
        ///
        /// <value> The data elementSize. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint elementSize
        {
            get {
                    if (GESRTP2Protocol.DataType(AreaType) == UFUAModel.DataType.UInt16)
                        return 2;
                    else
                        return 1;
            }
       }
       #endregion
    }
}
