////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2CommJob.cs
//
// summary:	Implements the driver GESRTP2 communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using DriverBaseInterfaces;

namespace GESRTP2
{
    /// <summary>   Protocol's task of the GESRTP2 driver. </summary>
    public class GESRTP2CommJob : CommJob
    {
        // keep state in the right order (sequence of execution)
        public enum GEState : int
        {
            None = 0,
            SymbolicGetDir,     // request program list
            SymbolicGetInfo,    // request internal address for symbolic vars
            SymbolicPolling,    // read/write data to device 
            DataAreaPolling,    // read/write data to device 
        }

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
            _SymbolicAddress = settings.SymbolicAddress;
            if (!string.IsNullOrEmpty(_SymbolicAddress))
                SetSymbolicAddressDeviceAddressInfo();

            if (TotalJobSize == 0)
            {
                if (GetProtocolDataByteSize() == 1)
                    TotalJobSize = (uint)settings.StringLength;
                else
                    TotalJobSize = (uint)((settings.StringLength + 1) / 2) * 2;
            }

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (settings.Tags[0].ArrayDimension == 0)
                {
                    if (this.TagsList.Count == 1)
                        TotalJobSize = GetProtocolDataByteSize();
                }
                else
                {
                    TotalJobSize = GetProtocolDataByteSize() * settings.Tags[0].ArrayDimension;
                }
            }

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
            _SymbolicAddress = defTag.GESRTP2DynSettings.SymbolicAddress;
            if (!string.IsNullOrEmpty(_SymbolicAddress))
                SetSymbolicAddressDeviceAddressInfo();

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
                {
                    if (this.TagsList.Count == 1)
                        TotalJobSize = GetProtocolDataByteSize();
                }
                else
                {
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
                }
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
        private UInt32 _SymbolicAddressCoherencyCookie;
        private byte[] _SymbolicAddressDeviceInternalAddress;        
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
            if (AreaType == AreaTypes.Symbolic)
                CheckJobValidSymbolic();
            else
                CheckJobValidDataArea();
        }

        private void CheckJobValidSymbolic()
        {
            if (!GESRTP2Protocol.PlcSupportSymbolic(((GESRTP2Station)Station).PlcType))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorPlcDontSupportSymbolicAddress, Station.Name);
                return;
            }

            // don't check symbolic job size because if different from data area and from plc model
            if (TotalJobSize > GESRTP2Protocol.GetMaxJobSize(((GESRTP2Station)Station).PlcType))
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", TagsList[0].TagNode.NodeId.ToString());
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;            
        }

        private void CheckJobValidDataArea()
        {
            uint size = 0;
            string tagnamelist = string.Empty;

            // skip any kind of controls for Atomic Struct Job --> controls are valid only for standard job
            if (!(IsStructAtomic()))
            {
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
                        InvalidReason = string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
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
                        InvalidReason = string.Format(Properties.Resources.InvalidZeroStartAddress);
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

                if (TotalJobSize > GESRTP2Protocol.GetMaxJobSize(((GESRTP2Station)Station).PlcType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                    return;
                }

                if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String)
                {
                    if (TotalJobSize > GESRTP2Protocol.DATA_AREA_MAX_STRING_SIZE)
                    {
                        IsValid = false;
                        InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                        return;
                    }
                }
            }
                        
            IsValid = true;
            InvalidReason = string.Empty;
        }

        public Tag GetTagByNodeID(NodeId nodeId)
        {
            Tag tag = null;
            lock (lockListObject)
                tag = TagsList.Find(n => n.TagNode.NodeId == nodeId);

            return tag;
        }

        public override uint getProtocolDataType()
        {
            if (AreaType == AreaTypes.Symbolic)
            {
                if (base.getProtocolDataType() == (uint)BuiltInType.Boolean || base.getProtocolDataType() == (uint)BuiltInType.Byte || base.getProtocolDataType() == (uint)BuiltInType.SByte)
                    return (uint)BuiltInType.UInt16;
                else
                    return base.getProtocolDataType();
            }
            else
            {
                return (uint)GESRTP2Protocol.DataType(AreaType);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of GESRTP2CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetMaxJobSize()
        {
            return GESRTP2Protocol.GetMaxJobSize(((GESRTP2Station)Station).PlcType);
        }

        public override bool IsJobAggregable()
        {
            return (_AreaType != AreaTypes.Symbolic);
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

            if (AreaType == AreaTypes.Symbolic)
                return JobAggregationType.JobAggregImpossible;

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            if (testJob.TagsList[0].TagNode.DataType == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;

            // a custom job (bit array, atomic struc, etc) cannot be aggregated with other jobs
            if (IsCustomJob() || testJob.IsCustomJob())
                return JobAggregationType.JobAggregImpossible;

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;

            if (GESRTP2Protocol.DataType(AreaType) == BuiltInType.Boolean)
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
                int endTag =(int) (startTag + tag.Size - 1);
                if ((startTest >= startTag && (int)startTest <= endTag)
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
                if (endTest - start + 1 <= ((GESRTP2Station)Station).GetAggregateMaxJobSize())
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
                if (end - startTest + 1 <= ((GESRTP2Station)Station).GetAggregateMaxJobSize())
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
            Tag startTag = null;

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
            }
            if(listToWrite.Count == 0)
            {
                return;
            }
            startTag = listToWrite[0];
            List<byte> outData = new List<byte>();
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
                    if (_AreaType == AreaTypes.Symbolic && (uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        sizeProtocolData = 1;
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

            UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
            listOnWriting.Clear();
            listToWrite.Clear();

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
        
        public uint GetTagsListOnWritingByteOffset()
        {
            lock (lockListObject)
            {
                if(TagsListOnWriting.Count > 0)
                {
                    return (TagsListOnWriting[0].ByteOffset);
                }
                else
                {
                    return (0);
                }
            }

        }
                
        public bool HasSymbolicAddressDevideInfo()
        {
            return _SymbolicAddressDeviceInternalAddress != null;
        }

        public void ResetRunTimeSymbolicParameters()
        {            
            _GeState = GEState.None;
            SetSymbolicAddressDeviceAddressInfo();
        }

        public void SetSymbolicAddressDeviceAddressInfo(UInt32 coherencyCookie = 0, byte[] deviceInternalAddress = null, SymbolicDataType dataType = SymbolicDataType.Undefined, UInt32 variableLength = 0, UInt32 arrayLength = 0)
        {
            _SymbolicAddressCoherencyCookie = coherencyCookie;
            _SymbolicAddressDeviceInternalAddress = deviceInternalAddress;
            _SymbolicAddressVariableLength = variableLength;
            _SymbolicAddressDataType = dataType;
            _SymbolicAddressArrayLength = arrayLength;

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String)
                SetSymbolicStringInfo(variableLength);            
        }

        public GESRTP2ErrorCodes CheckSymbolicDeviceAddressInfo(UInt32 coherencyCookie, byte[] deviceInternalAddress, uint dataType, UInt32 variableLength, UInt32 arrayLength)
        {            
            // variable is not present on PLC
            if (variableLength == 0 && dataType == 0 && arrayLength == 0)
                return GESRTP2ErrorCodes.ErrorVarBadNotFound;

            // unsupported data type
            if (!Enum.IsDefined(typeof(SymbolicDataType), dataType) || (SymbolicDataType)dataType == SymbolicDataType.Undefined)
                return GESRTP2ErrorCodes.ErrorSymbolicUnsupportedDataType;
            
            // check if data type of plc match with tag data type
            if (!GESRTP2Protocol.IsSymbolicDataCompatibleWithMoviconDataType((SymbolicDataType)dataType, (BuiltInType)((uint)TagsList[0].TagNode.DataType.Identifier)))
                return GESRTP2ErrorCodes.ErrorDataTypeMisMatch;

            // invalid array dimension
            if (arrayLength != TagsList[0].TagNode.ArrayDimension)
                return GESRTP2ErrorCodes.ErrorArraySizeMismatch;

            return (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        public void GetSymbolicAddressDeviceAddress(out UInt32 coherencyCookie, out byte[] deviceInternalAddress)
        {
            coherencyCookie = _SymbolicAddressCoherencyCookie;
            deviceInternalAddress = _SymbolicAddressDeviceInternalAddress;
        }

        public void SetSymbolicreadData(List<byte> chReceiveBuffer, ref UInt32 offset)
        {
            readData.Clear();
            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                int nrValues = (TagsList[0].TagNode.ArrayDimension == 0 ? 1 : (int)TagsList[0].TagNode.ArrayDimension);
                for (int i = 0; i < nrValues; i++)
                {
                    readData.Add(chReceiveBuffer[(int)offset]);
                    readData.Add(0);
                    offset++;
                }
            }
            else
            {
                readData.AddRange(chReceiveBuffer.Skip((int)offset).Take((int)_SymbolicAddressVariableLength));
                offset += _SymbolicAddressVariableLength;
            }
        }

        public void ResetreadData()
        {
            _readData.Clear();
        }

        public void ResetwriteData()
        {
            _writeData.Clear();
        }

        public uint GetwriteDataLength()
        {
            return TotalJobSize;
        }

        public UInt32 GetSymbolicAddressVariableLengthFromTotalJobSize()
        {
            return TotalJobSize;
        }

        public void SetSymbolicStringInfo(uint stringLength)
        {
            uint arraySize = (TagsList[0].TagNode.ArrayDimension == 0 ? 1 : TagsList[0].TagNode.ArrayDimension);

            if (stringLength == 0)
                stringLength = (GESRTP2Protocol.SYMBOLIC_DEFAULT_STRING_SIZE) * arraySize;

            TagsList[0].Size = stringLength;
            StringLength = stringLength / arraySize;
            TotalJobSize = stringLength;
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

        /// <summary>   The Symbolic data type. </summary>
        private ushort _SymbolicType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort SymbolicType
        {
            get { return _SymbolicType; }
            set
            {
                _SymbolicType = value;
            }
        }

        /// <summary>   The Symbolic address. </summary>
        private string _SymbolicAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Symbolic address of variable. </summary>
        ///
        /// <value> The symbolic address . </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SymbolicAddress
        {
            get { return _SymbolicAddress; }
            set
            {
                _SymbolicAddress = value;
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
                    if (GESRTP2Protocol.DataType(AreaType) == BuiltInType.UInt16)
                        return 2;
                    else
                        return 1;
            }
        }

        private GEState _GeState = GEState.None;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Instance Number. </summary>
        ///
        /// <value> The Instance Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GEState GeState
        {
            get { return _GeState; }
            set { _GeState = value; }
        }

        private List<byte> _readData = new List<byte>();
        public List<byte> readData
        {
            get { return _readData; }
            set { _readData = value; }
        }
        
        private SymbolicDataType _SymbolicAddressDataType;
        private UInt32 _SymbolicAddressArrayLength;

        private UInt32 _SymbolicAddressVariableLength;
        public UInt32 SymbolicAddressVariableLength
        {
            get { return _SymbolicAddressVariableLength; }
            set { _SymbolicAddressVariableLength = value; }
        }
        #endregion
    }
}
