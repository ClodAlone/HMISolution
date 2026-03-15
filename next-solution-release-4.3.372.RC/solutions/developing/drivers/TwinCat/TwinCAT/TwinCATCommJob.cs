using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.IO;

namespace TwinCAT
{
    public class TwinCATCommJob : CommJob
    {
        #region Constructors

        public TwinCATCommJob(Station station, TwinCATCommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            _Length = settings.Length;
            _NotificationHandle = 0;
            TwinCATChannel tcChannel = station.GetChannel() as TwinCATChannel;
            _TwinCATVersion = tcChannel.TwinCATVersion;
            AddressObj = new TwinCATAddress(_Address, _TwinCATVersion);

            if (AddressObj.IsNumeric && ProtocolDataSizeSmall())
                ElementNumber = 1;

            CheckJobValid();
        }

        public TwinCATCommJob(Station station, TwinCATTag defTag)
            : base(station, defTag)
        {
            _Address = defTag.TwinCATDynSettings.Address;
            _Length = defTag.TwinCATDynSettings.Length;
            _NotificationHandle = 0;
            TwinCATChannel tcChannel = station.GetChannel() as TwinCATChannel;
            _TwinCATVersion = tcChannel.TwinCATVersion;
            AddressObj = new TwinCATAddress(_Address, _TwinCATVersion);

            if (AddressObj.IsNumeric && ProtocolDataSizeSmall())
                ElementNumber = 1;

            CheckJobValid();
        }

        public TwinCATCommJob(Station station)
            : base(station)
        {
            _Address = String.Empty;
            _Length = 0;
            _NotificationHandle = 0;
            TwinCATChannel tcChannel = station.GetChannel() as TwinCATChannel;
            _TwinCATVersion = tcChannel.TwinCATVersion;

            if (AddressObj.IsNumeric && ProtocolDataSizeSmall())
                ElementNumber = 1;
            
            CheckJobValid();
        }

        #endregion

        #region Abstract Methods

        public override void GetJobData(ref object jobData)
        {
            lock (lockListObject)
            {
                for (int i = 0; i < TagsList.Count; i++)
                {
                    uint dim = TagsList[i].Size;
                }
            }
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
            // also boolean/bit type are calculated as 1 tag = 1 byte
            return TwinCATProtocol.MAX_DATA_BYTES;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob,
                                                           out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) ==
                JobAggregationType.JobAggregImpossible)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            TwinCATCommJob testJob = candJob as TwinCATCommJob;
            if (testJob == null)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!AddressObj.IsValid || !testJob.AddressObj.IsValid)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!AddressObj.IsNumeric || !testJob.AddressObj.IsNumeric)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataArea != testJob.AddressObj.DataArea)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if ((AddressObj.DataFormat == TwinCATDataFormat.DataFormat_Bit) ||
                (testJob.AddressObj.DataFormat ==
                 TwinCATDataFormat.DataFormat_Bit))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataFormat != testJob.AddressObj.DataFormat)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if ((Length > 0) || (testJob.Length > 0))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 &&
                 testJob.TagsList[0].TagNode.DataType != nBool)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            ExtraBytes = 0;
            uint elementsize = 1;
            if(TwinCATVersion == (byte) TwinCATVersions.Version3x)
            {
                AddressObj.SetTwinCATVersion(TwinCATVersion);
                testJob.AddressObj.SetTwinCATVersion(TwinCATVersion);
                switch(AddressObj.DataFormat)
                {
                    case TwinCATDataFormat.DataFormat_Word:
                        elementsize = 2;
                        break;

                    case TwinCATDataFormat.DataFormat_DWord:
                        elementsize = 4;
                        break;

                    default:
                        elementsize = 1;
                        break;
                }
            }

            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            //uint start = (uint)AddressObj.ByteElementNumber;
            //uint startTest = (uint)testJob.AddressObj.ByteElementNumber;
            uint start = (uint)AddressObj.ElementNumber;
            uint startTest = (uint)testJob.AddressObj.ElementNumber;
            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;
            //uint end = start + TotalJobSize - 1;
            //uint endTest = startTest + testJob.TotalJobSize - 1;
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / elementsize;
                uint endTag = startTag + (tag.Size / elementsize) - 1;
                //uint startTag = start + tag.ByteOffset;
                //uint endTag = startTag + tag.Size - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            if (startTest < start && endTest > end)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // Aggregate
            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) * elementsize;
                //uint newoffset = startTest - start;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset += newoffset;
                }
                return JobAggregationType.JobAggregFits;
            }

            // Aggregate forward
            uint endForward = end + Granularity;
            if (startTest >= start && startTest <= endForward)
            {
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize())
                //if ((endTest - start + 1) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = (startTest - start) * elementsize;
                    //uint newoffset = startTest - start;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset += newoffset;
                    }
                    ExtraBytes = (endTest - end) * elementsize;
                    //ExtraBytes = endTest - end;
                    return JobAggregationType.JobAggregForward;
                }
            }

            //// Aggregate backward
            uint startBkw = 0;
            if (start > Granularity)
            {
                startBkw = start - Granularity;
            }
            if (endTest >= startBkw && endTest <= end)
            {
                if (((endTest - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())
                //if ((endTest - startBkw + 1) <= GetAggregateMaxJobSize())
                {
                    //uint newoffset = 0;
                    //for (int i = 0; i < candJob.TagsList.Count; i++)
                    //{
                    //    candJob.TagsList[i].ByteOffset = newoffset;
                    //    newoffset += candJob.TagsList[i].Size;
                    //}
                    ExtraBytes = (start - startTest) * elementsize;
                    //ExtraBytes = start - startTest;
                    return JobAggregationType.JobAggregBackward;
                }
            }

            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob,
                                          JobAggregationType AggType,
                                          uint ExtraBytes)
        {
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new
             *          job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new
             *           job size and new Start address.
             */
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new TwinCATTag(
                                                candJob.TagsList[i].TagNode,
                                                candJob.TagsList[i].ByteOffset,
                                                0));
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new TwinCATTag(
                                                candJob.TagsList[i].TagNode,
                                                candJob.TagsList[i].ByteOffset,
                                                0));
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    //for (int i = 0; i < candJob.TagsList.Count; i++)
                    //{
                    //    candJob.TagsList[i].ByteOffset += ExtraBytes;
                    //    TagsList.Add(new TwinCATTag(
                    //                            candJob.TagsList[i].TagNode,
                    //                            candJob.TagsList[i].ByteOffset,
                    //                            0));
                    //}
                    for (int i = 0; i < TagsList.Count; i++)
                    {
                        TagsList[i].ByteOffset += ExtraBytes;
                    }
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new TwinCATTag(
                                                candJob.TagsList[i].TagNode,
                                                candJob.TagsList[i].ByteOffset,
                                                0));
                    }
                    TotalJobSize += ExtraBytes;
                    Address = ((TwinCATCommJob)candJob).Address;
                    AddressObj.Set(Address);
                    break;
                default:
                    return false;
            }

            return true;
        }

        #endregion

        #region Data members

        public TwinCATAddress AddressObj;
        public object lockNewData = new object();
        public object NewDataValue = new object();
        
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
                    nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }

            return false;
        }

        #endregion

        #region Specific Methods

        public uint UpdateTagValue(NodeId tagnodeid, DataValue value)
        {
            lock (lockListObject)
            {
                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == tagnodeid; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                tag.Value.Value = Utils.Clone(value.Value);
                tag.Value.StatusCode = value.StatusCode;
                tag.Value.ServerTimestamp = value.ServerTimestamp;
                tag.Value.SourceTimestamp = value.SourceTimestamp;
                tag.SetInternalValues(Utils.Clone(value.Value));

                if (tag.DynSettings.OutputAtStartup &&
                    Type != LinkType.Input && !TagsListToWrite.Contains(tag))
                    TagsListToWrite.Add(tag);

                return StatusCodes.Good;
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
            UFUAModel.DataType VarType = UFUAModel.DataType.Byte;

            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                {
                    bit = true;
                    VarType = UFUAModel.DataType.Boolean;
                }
                else
                    num = true;
                //size += GetTagSize(t);
            }
            if ((bit && num))
            {
                IsValid = false;
                InvalidReason = string.Format(
                                Properties.Resources.ErrorBitAndOtherTagTypes,
                                tagnamelist);
                return;
            }
                        
            if (TotalJobSize > TwinCATProtocol.GetMaxJobSize(VarType))
            //if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(
                                           Properties.Resources.ErrorJobTooBig,
                                           tagnamelist);
                return;
            }

            if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String)
            {
                if (!TwinCATProtocol.IsValidStringSize(Length))
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                                           Properties.Resources.ErrorJobTooBig,
                                           tagnamelist);
                }
            }

            if (!AddressObj.IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format(
                              Properties.Resources.ErrorInvalidAssignedAddress,
                              Address);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            //for (int i = 0; i < TagsList.Count; i++)
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

        public override uint getProtocolDataType()
        {
            switch (AddressObj.DataFormat)
            {

                case TwinCATDataFormat.DataFormat_REAL:
                    return (uint)BuiltInType.Float;

                case TwinCATDataFormat.DataFormat_LREAL:
                    return (uint)BuiltInType.Double;

                case TwinCATDataFormat.DataFormat_SINT:
                case TwinCATDataFormat.DataFormat_Byte:
                    return (uint)BuiltInType.Byte;

                case TwinCATDataFormat.DataFormat_INT:
                case TwinCATDataFormat.DataFormat_Word:
                    return (uint)BuiltInType.UInt16;

                case TwinCATDataFormat.DataFormat_DINT:
                case TwinCATDataFormat.DataFormat_DWord:
                    return (uint)BuiltInType.UInt32;

                case TwinCATDataFormat.DataFormat_STRING:
                    return (uint)BuiltInType.String;

                case TwinCATDataFormat.DataFormat_Bit:
                    return (uint)BuiltInType.Boolean;

                case TwinCATDataFormat.DataFormat_LINT:
                    return (uint)BuiltInType.Int64;

                case TwinCATDataFormat.DataFormat_ULINT:
                    return (uint)BuiltInType.UInt64;

                default:
                    return 0;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = value;
            }
        }

        /// <summary>
        /// Length (property used for string tags)
        /// </summary>
        private UInt32 _Length;
        public UInt32 Length
        {
            get { return _Length; }
            set { _Length = value; }
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
        /// TwinCAT Version
        /// </summary>
        private byte _TwinCATVersion;
        public byte TwinCATVersion
        {
            get
            {
                return _TwinCATVersion;
            }

            set
            {
                _TwinCATVersion = value;
            }
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
