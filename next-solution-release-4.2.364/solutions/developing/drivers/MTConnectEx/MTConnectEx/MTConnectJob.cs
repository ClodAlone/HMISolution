using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using Utilities;


namespace MTConnect
{

    public class MTConnectCommJob : CommJob
    {
        #region Constructors
        public MTConnectCommJob(Station station, MTConnectCommJobSettings settings)
            : base(station, settings)
        {
            _TagName = settings.TagName;
            _PathParameter = settings.PathParameter;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<MTConnectProtocol.DataValue>();
            CheckJobValid();
        }

        public MTConnectCommJob(Station station, MTConnectTag defTag)
            : base(station, defTag)
        {
            _TagName = defTag.MTConnectDynSettings.TagName;
            _PathParameter = defTag.MTConnectDynSettings.PathParameter;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<MTConnectProtocol.DataValue>();
            CheckJobValid();
        }

        public MTConnectCommJob(Station station)
            : base(station)
        {
            _TagName = String.Empty;
            PathParameter = String.Empty;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<MTConnectProtocol.DataValue>();

            CheckJobValid();
        }

        protected MTConnectCommJob()
        {
            _TagName = String.Empty;
            _PathParameter = String.Empty;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<MTConnectProtocol.DataValue>();

            CheckJobValid();
        }
        #endregion

        #region override Methods
        public override uint GetMaxJobSize()
        {
            //return MTConnectProtocol.GetMaxJobSize(FunctionCode, Type);
            return 2048;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {

        }
        //public override void GetJobData(ref object jobData)
        //{
        //    var listToWrite = new List<Tag>();
        //    var listOnWriting = new List<Tag>();

        //    lock (lockListObject)
        //    {
        //        listToWrite.AddRange(TagsListToWrite);
        //        TagsListToWrite.Clear();
        //    }

        //    List<byte> outData = new List<byte>();
        //    //prepare a write request
        //    listToWrite.Sort(CompareTagByOffset);
        //    Tag cand = null;
        //    byte[] jobdata;
        //    UInt16 nData = 0;
        //    do
        //    {
        //        if (cand != null)
        //        {
        //            if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
        //                break;
        //        }
        //        cand = listToWrite[0];
        //        listToWrite.Remove(cand);
        //        if (!listOnWriting.Contains(cand))
        //            listOnWriting.Add(cand);
        //        if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
        //            nData = (UInt16)((cand.Size + 7) / 8);
        //        else if (ElementNumber > 0 && !ProtocolDataSizeBig())
        //        {
        //            if (cand.TagNode.ArrayDimension == 0)
        //                nData = (ushort)(GetProtocolDataByteSize());
        //            else
        //                nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
        //        }
        //        else
        //            nData = (UInt16)cand.Size;
        //        jobdata = new byte[nData];
        //        cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
        //        uint ArraySize = cand.TagNode.ArrayDimension;
        //        if (ArraySize == 0)
        //            ArraySize = 1;
        //        if (ProtocolDataSizeBig())
        //        {
        //            List<byte> correctData = new List<byte>();
        //            UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
        //            UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
        //            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
        //            {
        //                byte[] tmpdata = new byte[sizeDataType];
        //                if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
        //                    Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
        //                else
        //                {
        //                    if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
        //                        tmpdata[0] = 0;
        //                    else
        //                        tmpdata[0] = 1;
        //                }
        //                cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
        //                correctData.AddRange(tmpdata);
        //            }
        //            jobdata = correctData.ToArray();
        //        }
        //        else if (isProtocolBool())
        //        {
        //            if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
        //            {
        //                byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
        //                for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
        //                {
        //                    if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
        //                        tmpData[ArrayIndex] = 1;
        //                }
        //                jobdata = tmpData;
        //            }
        //        }

        //        outData.AddRange(jobdata);

        //    } while (listToWrite.Count > 0);

        //    lock (lockListObject)
        //    {
        //        listOnWriting.ForEach((tag) =>
        //        {
        //            if (!TagsListOnWriting.Contains(tag))
        //                TagsListOnWriting.Add(tag);
        //        });
        //        listOnWriting.Clear();

        //        if (listToWrite.Count > 0)
        //        {
        //            var tempListToWrite = new List<Tag>();
        //            tempListToWrite.AddRange(TagsListToWrite);
        //            TagsListToWrite.Clear();
        //            listToWrite.ForEach((tag) =>
        //            {
        //                if (!TagsListToWrite.Contains(tag))
        //                {
        //                    TagsListToWrite.Add(tag);
        //                }
        //            });
        //            listToWrite.Clear();
        //            tempListToWrite.ForEach((tag) =>
        //            {
        //                if (!TagsListToWrite.Contains(tag))
        //                {
        //                    TagsListToWrite.Add(tag);
        //                }
        //            });
        //            tempListToWrite.Clear();
        //        }
        //    }

        //    if (isProtocolBool())
        //    {
        //        byte[] tmpData = new byte[(outData.Count() + 7) / 8];
        //        for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
        //        {
        //            if (outData[ArrayIndex] != 0)
        //                tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
        //        }
        //        jobData = tmpData;
        //    }
        //    else
        //        jobData = outData.ToArray();
        //}

        public override uint OnWriteTag(NodeId tagnodeid, ref object value)
        {
            lock (lockListObject)
            {
                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == tagnodeid; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                tag.Value.Value = Utils.Clone(value);
                if (StatusCode.IsBad(tag.Value.StatusCode))
                    tag.Value.StatusCode = StatusCodes.Uncertain;

                return StatusCodes.Good;
            }
        }

        public override bool SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            bool inUse = base.SetInUse(tagnodeid, bInUse, samplinginterval);
            StartStopSampling(inUse);
            return inUse;
        }

        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Methods     

        public void StartStopSampling(bool inUse)
        {
            // reset data value buffer all time Job is SubScribed or change InUse state
            lock (lockValuesToWrite)
                _ValuesToWrite.Clear();

            if (inUse)
            {
                // create a date time variable without msec
                DateTime dt = MTConnectProtocol.GetDateTimeUtcNowNoMSec();

                // when tag/job is in use, init data/time sampling 
                _StartSamplingTime = dt.Subtract(_FrequencyOfSendingSignal);
                _LastDataWrittenTime = _StartSamplingTime;
                LastExecutionTime = _StartSamplingTime;
            }
        }

        public bool GetInUseState()
        {
            // reset data value buffer all time Job is SubScribed or change InUse state
            lock (lockListObject)
            {
                if (TagsList.Count == 0)
                    return false;
                else
                    return TagsList[0].InUse;
            }
        }

        private bool IsTypeAdmitted(NodeId type)
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

        private void CheckJobValid()
        {
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }

                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if(Type != LinkType.Input)
            {
                IsValid = false;
                InvalidReason = string.Format("The Tag type is invalid for its Type. (Name: {0} Data Type: {1}) only Read task.", TagName, Type.ToString());
                return;
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1})", errorDesc, tagnamelist);
                return;
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

        //public override void SetJobData(object jobData, ref List<Tag> changed)
        //{
        //    byte[] rec = jobData as byte[];
        //    if (rec == null)
        //        return;

        //    base.SetJobData(rec, ref changed);
        //    lock (lockListObject)
        //    {
        //        UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
        //        for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
        //        {
        //            MTConnectTag MTConnectTag = (MTConnectTag)TagsList[TagIndex];
        //            if (MTConnectTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
        //            {
        //                if (isProtocolBool())
        //                {
        //                    if (MTConnectTag.SetTagValue(ref rec, (int)MTConnectTag.ByteOffset))
        //                        changed.Add(MTConnectTag);
        //                }
        //                else
        //                {
        //                    uint ArraySize = MTConnectTag.TagNode.ArrayDimension;
        //                    if (ArraySize == 0)
        //                        ArraySize = 1;
        //                    byte[] tmpData = new byte[ArraySize];
        //                    MTConnectTag.setMemRW(rec, (int)MTConnectTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
        //                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
        //                    {
        //                        bool valBool = MTConnectTag.getBoolValueFromMemRW((int)MTConnectTag.ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
        //                        tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
        //                    }

        //                    if (MTConnectTag.SetTagValue(ref tmpData, 0))
        //                        changed.Add(MTConnectTag);
        //                }
        //            }
        //            else
        //            {
        //                if (isProtocolBool())
        //                {
        //                    if (MTConnectTag.SetTagValue(ref rec, (int)MTConnectTag.ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
        //                        changed.Add(MTConnectTag);
        //                }
        //                else
        //                {
        //                    if (!ProtocolDataSizeBig())
        //                    {
        //                        uint elemsize = 0;
        //                        if (ElementNumber > 0)
        //                        {
        //                            elemsize = GetProtocolDataByteSize();
        //                        }
        //                        if (MTConnectTag.SetTagValue(ref rec, (int)MTConnectTag.ByteOffset, elemsize))
        //                            changed.Add(MTConnectTag);
        //                    }
        //                    else
        //                    {
        //                        UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)MTConnectTag.TagNode.DataType.Identifier);
        //                        uint ArraySize = MTConnectTag.TagNode.ArrayDimension;
        //                        if (ArraySize == 0)
        //                            ArraySize = 1;
        //                        byte[] tmpData = new byte[sizeTmpData * ArraySize];
        //                        int indexTmpData = 0;
        //                        MTConnectTag.setMemRW(rec, (int)MTConnectTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
        //                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
        //                        {
        //                            Array.Copy(rec, MTConnectTag.ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
        //                            indexTmpData += sizeTmpData;
        //                        }

        //                        if (MTConnectTag.SetTagValue(ref tmpData, 0))
        //                            changed.Add(MTConnectTag);
        //                    }
        //                }
        //            }
        //        }

        //        FirstTime = false;
        //    }
        //}

       

        public bool GetDataValue(DateTime currentExecutionTime, out string signalValue)
        {
            signalValue = string.Empty;

            // requesting a data already published (--> not present) !!! 
            if (currentExecutionTime < _LastDataWrittenTime)
                return false;

            MTConnectProtocol.DataValue dataValue = null;

            lock (lockValuesToWrite)
            {
                if (_ValuesToWrite.Count == 0)
                    dataValue = new MTConnectProtocol.DataValue(TagsList[0], MTConnectProtocol.GetDateTimeUtcNowNoMSec());
                else
                {
                    dataValue = _ValuesToWrite[0];
                    foreach (MTConnectProtocol.DataValue tagValue in _ValuesToWrite)
                    {
                        if (tagValue.SamplingTime < currentExecutionTime)
                        {
                            dataValue = tagValue;
                        }
                        else
                            break;
                    }
                }
            }

            signalValue = dataValue.Value;

            return true;
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
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                {
                    if (isProtocolBool())
                    {
                        if (((MTConnectTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset))
                            changed.Add(TagsList[0]);
                    }
                    else
                    {
                        uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                        if (ArraySize == 0)
                            ArraySize = 1;
                        byte[] tmpData = new byte[ArraySize];
                        TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            bool valBool = TagsList[0].getBoolValueFromMemRW((int)TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                            tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                        }

                        if (((MTConnectTag)(TagsList[0])).SetTagValue(ref tmpData, 0))
                            changed.Add(TagsList[0]);
                    }
                }
                else
                {
                    if (isProtocolBool() && ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.Structure))
                    {
                        if (((MTConnectTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, false, (uint)(ElementNumber > 0 ? 1 : 0)))
                            changed.Add(TagsList[0]);
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
                            //if (MTConnectTag == S7DataFormats.S7_DTL)
                            //{
                            //    for (int nCicli = 0, offset = 0; nCicli < TagsList.Count; nCicli++)
                            //    {
                            //        if (nCicli > 7)
                            //        {
                            //            break;
                            //        }
                            //        if (nCicli > 0)
                            //        {
                            //            offset = nCicli + 1;
                            //        }
                            //        if (((S7TIATag)(TagsList[nCicli])).SetTagValue(ref rec, offset, UpdateTimeStamp, elemsize))
                            //        {
                            //            changed.Add(TagsList[nCicli]);
                            //        }
                            //    }
                            //}

                            //Dynamically adjust the size of the string variable
                            if ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                                ((MTConnectTag)(TagsList[0])).Size = (uint)Buffer.ByteLength(rec) + 1;

                            if (((MTConnectTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, false, elemsize))
                            {
                                changed.Add(TagsList[0]);
                            }
                            else
                            {//TODO
                                //if ((TagsList[0].TagNode.ArrayDimension != 0) && (S7DataFormat == S7DataFormats.String))
                                //{

                                //}
                            }
                        }
                        else
                        {
                            UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier);
                            uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[sizeTmpData * ArraySize];
                            int indexTmpData = 0;
                            TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                Array.Copy(rec, TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                indexTmpData += sizeTmpData;
                            }

                            if (((MTConnectTag)(TagsList[0])).SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[0]);
                        }
                    }
                }
                FirstTime = false;
            }
        }


        private bool HasSameSamplingInterval(MTConnectProtocol.DataValue value1, MTConnectProtocol.DataValue value2)
        {
            //DateTime currentExecutionTime1 = GetCurrentScheduleTime(value1.SamplingTime);
            //DateTime nextExecutionTime2 = GetNextScheduleTime(value1.SamplingTime);

            // if last added value is inside current sampling interval, remove it ad add current 
            // on sampling interval, only one value is admitted 
            //return (value2.SamplingTime >= currentExecutionTime && value2.SamplingTime < nextExecutionTime);
            return (GetCurrentScheduleTime(value1.SamplingTime) == GetCurrentScheduleTime(value2.SamplingTime) && GetNextScheduleTime(value1.SamplingTime) == GetNextScheduleTime(value2.SamplingTime));
        }

        public void RemoveDataValue(DateTime currentExecutionTime)
        {
            lock (lockValuesToWrite)
            {
                _ValuesToWrite.RemoveAll(v => v.SamplingTime < currentExecutionTime);
            }
        }

        public DateTime GetNextScheduleTime(DateTime dtNow)
        {
            return GetCurrentScheduleTime(dtNow).Add(_FrequencyOfSendingSignal);
        }

        public DateTime GetPreviousScheduleTime(DateTime dtNow)
        {
            return GetCurrentScheduleTime(dtNow).Subtract(_FrequencyOfSendingSignal);
        }

        public DateTime GetCurrentScheduleTime(DateTime dtNow)
        {
            TimeSpan sp = dtNow.Subtract(_StartSamplingTime);

            Math.DivRem((int)sp.TotalSeconds, (int)_FrequencyOfSendingSignal.TotalSeconds, out int remainder);

            DateTime result = _StartSamplingTime.AddSeconds(sp.TotalSeconds - remainder);

            return result;
        }

        public void RemovePendingData(DateTime currentExecutionTime)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("DBG - RemovePendingData ExecutionTime:{0}, TagName:{1}", currentExecutionTime, _TagName));

            RemoveDataValue(currentExecutionTime);

            _LastDataWrittenTime = currentExecutionTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tag Name
        /// </summary>
        private string _TagName;
        public string TagName
        {
            get { return _TagName; }
            set { _TagName = value; }
        }

        private string _PathParameter;
        public string PathParameter
        {
            get { return _PathParameter; }
            set { _PathParameter = value; }
        }

        //private MTConnectProtocol.Category_E _Category;
        //public MTConnectProtocol.Category_E Category
        //{
        //    get { return _Category; }
        //    set { _Category = value; }
        //}

        //private MTConnectProtocol.FormatDataType_E _SupervisonDataType;
        //public MTConnectProtocol.FormatDataType_E SupervisonDataType
        //{
        //    get { return _SupervisonDataType; }
        //    set { _SupervisonDataType = value; } }

        /// <summary>
        /// Data sampling "speed" (sec)
        /// </summary>
        private TimeSpan _FrequencyOfSendingSignal;
        public TimeSpan FrequencyOfSendingSignal
        {
            get { return _FrequencyOfSendingSignal; }
            set { _FrequencyOfSendingSignal = value; }
        }

        private DateTime _StartSamplingTime;
        public DateTime StartSamplingTime
        {
            get { return _StartSamplingTime; }
            set { _StartSamplingTime = value; }
        }

        private DateTime _LastDataWrittenTime;
        public DateTime LastDataWrittenTime
        {
            get { return _LastDataWrittenTime; }
            set { _LastDataWrittenTime = value; }
        }

        public override uint SamplingInterval
        {
            get { return (uint)FrequencyOfSendingSignal.TotalMilliseconds; }
        }

        private object lockValuesToWrite = new object();
        private List<MTConnectProtocol.DataValue> _ValuesToWrite;
        public List<MTConnectProtocol.DataValue> ValuesToWrite
        {
            get { return _ValuesToWrite; }
            set { _ValuesToWrite = value; }
        }

        private byte[] _RequestResultValue = null;
        public byte[] RequestResultValue
        {
            get { return _RequestResultValue; }
            set { _RequestResultValue = value; }
        }

        private DriverErrorCodes _RequestResultError = DriverErrorCodes.ErrorNoError;
        public DriverErrorCodes RequestResultError
        {
            get { return _RequestResultError; }
            set { _RequestResultError = value; }
        }

        #endregion
    }
}
