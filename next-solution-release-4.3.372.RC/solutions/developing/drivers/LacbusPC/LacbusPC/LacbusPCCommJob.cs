using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.IO;

namespace LacbusPC
{
    public enum LacbusPCCommJobStatus : byte
    {
        Idle,
        RequestPending,
        ReplyReceived
    }

    public class LacbusPCCommJob : CommJob
    {
        #region Constructors

        public LacbusPCCommJob(Station station, LacbusPCCommJobSettings settings)
            : base(station, settings)
        {
            _LacbusPCDatumNumber = settings.LacbusPCDatumNumber;
            _LacbusPCDatumType = settings.LacbusPCDatumType;
            _LacbusPCDatumFormat = settings.LacbusPCDatumFormat;
            _LacbusPCCommunicationDuration = settings.LacbusPCCommunicationDuration;
            _LacbusPCDatumCategory = settings.LacbusPCDatumCategory;
            _LacbusPCStatus = LacbusPCCommJobStatus.Idle;
            _LacbusPCConversionMinRawValue = settings.LacbusPCConversionMinRawValue;
            _LacbusPCConversionMaxRawValue = settings.LacbusPCConversionMaxRawValue;
            _LacbusPCConversionMinValue = settings.LacbusPCConversionMinValue;
            _LacbusPCConversionMaxValue = settings.LacbusPCConversionMaxValue;
            _NotificationHandle = 0;

            CheckJobValid();
        }

        public LacbusPCCommJob(Station station, LacbusPCTag defTag)
            : base(station, defTag)
        {
            _LacbusPCDatumNumber = defTag.LacbusPCDynSettings.LacbusPCDatumNumber;
            _LacbusPCDatumType = defTag.LacbusPCDynSettings.LacbusPCDatumType;
            _LacbusPCDatumFormat = defTag.LacbusPCDynSettings.LacbusPCDatumFormat;
            _LacbusPCCommunicationDuration = defTag.LacbusPCDynSettings.LacbusPCCommunicationDuration;
            _LacbusPCDatumCategory = defTag.LacbusPCDynSettings.LacbusPCDatumCategory;
            _LacbusPCStatus = LacbusPCCommJobStatus.Idle;
            _LacbusPCConversionMinRawValue = defTag.LacbusPCDynSettings.LacbusPCConversionMinRawValue;
            _LacbusPCConversionMaxRawValue = defTag.LacbusPCDynSettings.LacbusPCConversionMaxRawValue;
            _LacbusPCConversionMinValue = defTag.LacbusPCDynSettings.LacbusPCConversionMinValue;
            _LacbusPCConversionMaxValue = defTag.LacbusPCDynSettings.LacbusPCConversionMaxValue;

            CheckJobValid();
        }

        public LacbusPCCommJob(Station station)
            : base(station)
        {
            _LacbusPCDatumNumber = 0;
            _LacbusPCDatumType = DatumTypes.DigitalInput;
            _LacbusPCDatumFormat = DatumFormats.Logical;
            _LacbusPCStatus = LacbusPCCommJobStatus.Idle;
            _LacbusPCCommunicationDuration = 0;
            _LacbusPCDatumCategory = DatumCategories.Instantaneous;
            _LacbusPCConversionMinRawValue = 0.0;
            _LacbusPCConversionMaxRawValue = 0.0;
            _LacbusPCConversionMinValue = 0.0;
            _LacbusPCConversionMaxValue = 0.0;

            CheckJobValid();
        }

        #endregion

        #region Abstract Methods

        public override void GetJobData(ref object jobData)
        {
            switch(LacbusPCDatumType)
            {
                case DatumTypes.SetDateTime:
                case DatumTypes.RTUPollRequest:
                case DatumTypes.ShutdownFR1000FrontEnd:
                    {
                        Tag cand;
                        lock (lockListObject)
                        {
                            if (TagsListToWrite.Count == 0)
                            {
                                return;
                            }
                            cand = TagsListToWrite[0];
                            TagsListToWrite.Remove(cand);
                            if (!TagsListOnWriting.Contains(cand))
                            {
                                TagsListOnWriting.Add(cand);
                            }
                        }
                    }
                    break;

                case DatumTypes.DigitalOutput:
                case DatumTypes.AnalogOutput:
                    if(!LockMustBeManaged())
                    {
                        GetJobDataEx(ref jobData);
                    }
                    else
                    {
                        // Special case: send value and lock status
                        lock (lockListObject)
                        {
                            TagsListToWrite.Clear();
                            var listToWrite = new List<Tag>();
                            listToWrite.AddRange(TagsList);
                            listToWrite.Sort(CompareTagByOffset);
                            do
                            {
                                Tag cand = listToWrite[0];
                                listToWrite.Remove(cand);
                                if (!TagsListOnWriting.Contains(cand))
                                {
                                    TagsListOnWriting.Add(cand);
                                }
                            } while (listToWrite.Count > 0);
                        }
                    }
                    break;

                default:
                    GetJobDataEx(ref jobData);
                    break;
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

        void GetJobDataEx(ref object jobData)
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

        public uint GetAggregateMaxJobSize()
        {
            uint aggLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((aggLimit != 0) && (aggLimit < JobMaxSize))
            {
                return aggLimit;
            }

            return JobMaxSize;
        }

        public override uint GetMaxJobSize()
        {
            return 2048;
        }

        public ushort modbusElementOnWrite(IList<Tag> listOnWriting)
        {
            ushort ItemCount = 0;
            foreach (var tag in listOnWriting)
            {
                ItemCount += (ushort)(tag.TagNode.ArrayDimension == 0 ? 1 : tag.TagNode.ArrayDimension);
            }
            return ItemCount;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            if (base.TestAggregateJob(candJob, out ExtraBytes) ==
                JobAggregationType.JobAggregImpossible)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            LacbusPCCommJob testJob = candJob as LacbusPCCommJob;
            if (testJob == null)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (LacbusPCDatumType != testJob.LacbusPCDatumType)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if((LacbusPCDatumType != DatumTypes.ModbusCoilSetpoints) && (LacbusPCDatumType != DatumTypes.ModbusRegisterSetpoints))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == nBool
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                return JobAggregationType.JobAggregImpossible;

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = LacbusPCDatumNumber;
            uint startTest = testJob.LacbusPCDatumNumber;
            uint elementsize = 2;
            if(LacbusPCDatumType == DatumTypes.ModbusCoilSetpoints)
            {
                if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                {
                    if (System.Math.Abs(start - startTest) % 8 != 0)
                        return JobAggregationType.JobAggregImpossible;

                    start /= 8;
                    startTest /= 8;
                }
                else
                    Granularity *= 8;

                elementsize = 1;
            }

            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / elementsize;
                int endTag =(int) (startTag + (tag.Size / elementsize) - 1);
                if ((startTest >= startTag && (int)startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (LacbusPCDatumType == DatumTypes.ModbusRegisterSetpoints)
            {
                if (SwapWords)
                    if (start % 2 != startTest % 2)
                        return JobAggregationType.JobAggregImpossible;
            }

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

            if (startTest >= start && startTest <= end + Granularity)
                //if (((endTest - startTest + 1) * elementsize) <= GetMaxJobSize())
                // Modified to solve FOGBUGZ 10394
                //if (((endTest - start + 1) * elementsize) <= GetMaxJobSize())
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize())

                {
                    uint newoffset = (startTest - start) * elementsize;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
                // Modified to solve FOGBUGZ 10394
                //if (((endTest - startBkw + 1) * elementsize) <= GetMaxJobSize())
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())

                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (start - startTest) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;//steve 280711
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new LacbusPCTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    break;

                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new LacbusPCTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }

                    TotalJobSize += ExtraBytes;
                    break;

                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in candJob.TagsList)
                    {
                        tag.ByteOffset += ExtraBytes;
                        var t = new LacbusPCTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    LacbusPCDatumNumber = ((LacbusPCCommJob)candJob).LacbusPCDatumNumber;
                    break;

                default:
                    return false;
            }
            return true;
        }

        #endregion

        #region Data members

        public int requestMessageNumber = 0;

        #endregion

        #region Static methods

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
                    nType == (uint)BuiltInType.String)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        #endregion

        #region Specific Methods

        public bool MustSendARequest()
        {
            bool returnValue = false;
            if((Type != LinkType.Input) && (TagsListToWrite.Count > 0))
            {
                returnValue = true;
            }
            return (returnValue);
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
            //uint size = 0;
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
            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
                //size += GetTagSize(t);
            }
            if ((bit && num) &&
                (LacbusPCDatumCategory != DatumCategories.Instantaneous) &&
                (LacbusPCDatumType != DatumTypes.DigitalOutput) &&
                (LacbusPCDatumType != DatumTypes.AnalogOutput))
            {
                IsValid = false;
                InvalidReason = string.Format(
                                Properties.Resources.ErrorBitAndOtherTagTypes,
                                tagnamelist);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(
                                           Properties.Resources.ErrorJobTooBig,
                                           tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public bool LockMustBeManaged()
        {
            bool managelockStatus = false;
            LacbusPCStation lacbusStation = Station as LacbusPCStation;

            if ((lacbusStation.LacbusPCProtocolType == LacbusPcUnderlyingProtocols.LacbusRTU) &&
               (LacbusPCDatumCategory == DatumCategories.Instantaneous) &&
               ((LacbusPCDatumType == DatumTypes.DigitalOutput) ||
                (LacbusPCDatumType == DatumTypes.AnalogOutput)) &&
               (TagsList.Count == 2))
            {
                managelockStatus = true;
            }

            return (managelockStatus);
        }

        public void SetJobValueAndLockStatus(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
            {
                return;
            }

            lock (lockListObject)
            {
                bool forceValueUpdate = false;
                if (InErrorState == true)
                {
                    forceValueUpdate = true;
                }
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, forceValueUpdate))
                    {
                        changed.Add(TagsList[TagIndex]);
                    }
                }
            }
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                bool forceValueUpdate = false;
                if ((LacbusPCDatumCategory != DatumCategories.Instantaneous) || (InErrorState == true))
                {
                    forceValueUpdate = true;
                }
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, forceValueUpdate))
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

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0, forceValueUpdate))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, forceValueUpdate, (uint)(ElementNumber > 0 ? 1 : 0)))
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
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, forceValueUpdate, elemsize))
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

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0, forceValueUpdate))
                                {
                                    TagsList[TagIndex].Value.SourceTimestamp = DateTime.UtcNow;
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public void SetLacbusJobErrorState(uint quality, string error, DateTime Timestamp)
        {
            if (InErrorState != StatusCode.IsBad(quality))
            {
                InErrorState = StatusCode.IsBad(quality);
                LastErrorMessage = error;
                LastErrorTime = DateTime.UtcNow;
            }

            SetLacbusJobQuality(quality, Timestamp);
        }

        public void SetLacbusJobQuality(uint quality, DateTime Timestamp)
        {
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    if (tag.Value.StatusCode.Code != quality)
                    {
                        tag.Value.StatusCode = quality;
                    }
                }
            }
        }

        public bool ConversionCanBeApplied()
        {
            bool canBeApplied = false;
            LacbusPCStation lacbusStation = Station as LacbusPCStation;
            // Automatic data conversion is performed only for analog and count data of RTUs that use the SOFBUS PL protocol 
            if(lacbusStation.LacbusPCProtocolType == LacbusPcUnderlyingProtocols.SofbusPL)
            {
                switch (LacbusPCDatumType)
                {
                    case DatumTypes.AnalogInput:
                    case DatumTypes.AnalogOutput:
                    case DatumTypes.CountInput:
                        if (((LacbusPCConversionMaxRawValue - LacbusPCConversionMinRawValue) != 0.0) &&
                           ((LacbusPCConversionMaxValue - LacbusPCConversionMinValue) != 0.0) &&
                           (LacbusPCConversionMaxRawValue > LacbusPCConversionMinRawValue) &&
                           (LacbusPCConversionMaxValue > LacbusPCConversionMinValue))
                        {
                            canBeApplied = true;
                        }
                    break;

                    case DatumTypes.DigitalInput:
                        if ((LacbusPCDatumCategory == DatumCategories.ReportDIEventCount) ||
                            (LacbusPCDatumCategory == DatumCategories.ReportDIActiveStateTimeCount))
                        {
                            if (((LacbusPCConversionMaxRawValue - LacbusPCConversionMinRawValue) != 0.0) &&
                               ((LacbusPCConversionMaxValue - LacbusPCConversionMinValue) != 0.0) &&
                               (LacbusPCConversionMaxRawValue > LacbusPCConversionMinRawValue) &&
                               (LacbusPCConversionMaxValue > LacbusPCConversionMinValue))
                            {
                                canBeApplied = true;
                            }
                        }
                    break;
                }
            }

            return (canBeApplied);
        }

        public double ApplyDataConversion(UInt16 valueToBeConverted)
        {
            return (LacbusPCConversionMinValue + (valueToBeConverted - LacbusPCConversionMinRawValue) * (LacbusPCConversionMaxValue - LacbusPCConversionMinValue) / (LacbusPCConversionMaxRawValue - LacbusPCConversionMinRawValue));           
        }

        public double ApplyDataConversion(UInt32 valueToBeConverted)
        {
            return (LacbusPCConversionMinValue + (valueToBeConverted - LacbusPCConversionMinRawValue) * (LacbusPCConversionMaxValue - LacbusPCConversionMinValue) / (LacbusPCConversionMaxRawValue - LacbusPCConversionMinRawValue));
        }

        public double ApplyDataInverseConversion(double valueToBeConverted)
        {
            return (LacbusPCConversionMinRawValue + (valueToBeConverted - LacbusPCConversionMinValue) * (LacbusPCConversionMaxRawValue - LacbusPCConversionMinRawValue) / (LacbusPCConversionMaxValue - LacbusPCConversionMinValue));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private UInt16 _LacbusPCDatumNumber;
        public UInt16 LacbusPCDatumNumber
        {
            get
            {
                return _LacbusPCDatumNumber;
            }

            set
            {
                _LacbusPCDatumNumber = value;
            }
        }

        /// <summary>
        /// LacbusPCDatumType
        /// </summary>
        private DatumTypes _LacbusPCDatumType;
        public DatumTypes LacbusPCDatumType
        {
            get { return _LacbusPCDatumType; }
            set { _LacbusPCDatumType = value; }
        }

        /// <summary>
        /// LacbusPCDatumFormat
        /// </summary>
        private DatumFormats _LacbusPCDatumFormat;
        public DatumFormats LacbusPCDatumFormat
        {
            get { return _LacbusPCDatumFormat; }
            set { _LacbusPCDatumFormat = value; }
        }

        /// <summary>
        /// Communication Duration
        /// </summary>
        private UInt16 _LacbusPCCommunicationDuration;
        public UInt16 LacbusPCCommunicationDuration
        {
            get
            {
                return _LacbusPCCommunicationDuration;
            }

            set
            {
                _LacbusPCCommunicationDuration = value;
            }
        }

        /// <summary>
        /// LacbusPCDatumCategory
        /// </summary>
        private DatumCategories _LacbusPCDatumCategory;
        public DatumCategories LacbusPCDatumCategory
        {
            get { return _LacbusPCDatumCategory; }
            set { _LacbusPCDatumCategory = value; }
        }

        /// <summary>
        /// Minimum Raw Value
        /// </summary>
        private double _LacbusPCConversionMinRawValue;
        public double LacbusPCConversionMinRawValue
        {
            get
            {
                return _LacbusPCConversionMinRawValue;
            }

            set
            {
                _LacbusPCConversionMinRawValue = value;
            }
        }

        /// <summary>
        /// Maximum Raw Value
        /// </summary>
        private double _LacbusPCConversionMaxRawValue;
        public double LacbusPCConversionMaxRawValue
        {
            get
            {
                return _LacbusPCConversionMaxRawValue;
            }

            set
            {
                _LacbusPCConversionMaxRawValue = value;
            }
        }

        /// <summary>
        /// Minimum Converted Value
        /// </summary>
        private double _LacbusPCConversionMinValue;
        public double LacbusPCConversionMinValue
        {
            get
            {
                return _LacbusPCConversionMinValue;
            }

            set
            {
                _LacbusPCConversionMinValue = value;
            }
        }

        /// <summary>
        /// Maximum Converted Value
        /// </summary>
        private double _LacbusPCConversionMaxValue;
        public double LacbusPCConversionMaxValue
        {
            get
            {
                return _LacbusPCConversionMaxValue;
            }

            set
            {
                _LacbusPCConversionMaxValue = value;
            }
        }

        /// <summary>
        /// Notification Handle
        /// </summary>
        private int _NotificationHandle;
        public int NotificationHandle
        {
            get { return _NotificationHandle; }
            set { _NotificationHandle = value; }
        }

        /// <summary>
        /// Status
        /// </summary>
        private LacbusPCCommJobStatus _LacbusPCStatus;
        public LacbusPCCommJobStatus LacbusPCStatus
        {
            get
            {
                return _LacbusPCStatus;
            }

            set
            {
                _LacbusPCStatus = value;
            }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}DT{1:00}", ret, (uint)LacbusPCDatumType);
            }
        }

        #endregion
    }
}
