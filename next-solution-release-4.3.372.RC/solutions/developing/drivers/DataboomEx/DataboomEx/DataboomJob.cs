using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using Utilities;


namespace Databoom
{

    public class DataboomCommJob : CommJob
    {
        #region Constructors
        public DataboomCommJob(Station station, DataboomCommJobSettings settings)
            : base(station, settings)
        {
            _TagName = settings.TagName;            
            _FrequencyOfSendingSignal = new TimeSpan(0,0, (int)settings.FrequencyOfSendingSignal);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<DataboomProtocol.DataValue>();
            CheckJobValid();
        }

        public DataboomCommJob(Station station, DataboomTag defTag)
            : base(station, defTag)
        {
            _TagName = defTag.DataboomDynSettings.TagName;
            _FrequencyOfSendingSignal = new TimeSpan(0, 0, (int)defTag.DataboomDynSettings.FrequencyOfSendingSignal);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<DataboomProtocol.DataValue>();
            CheckJobValid();
        }

        public DataboomCommJob(Station station)
            : base(station)
        {
            _TagName = String.Empty;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<DataboomProtocol.DataValue>();

            CheckJobValid();
        }

        protected DataboomCommJob()
        {
            _TagName = String.Empty;
            _FrequencyOfSendingSignal = new TimeSpan(0);
            _StartSamplingTime = DateTime.MinValue;
            _LastDataWrittenTime = DateTime.MinValue;
            _ValuesToWrite = new List<DataboomProtocol.DataValue>();

            CheckJobValid();
        }
        #endregion
        
        #region override Methods
        public override uint GetMaxJobSize()
        {
            //return DataboomProtocol.GetMaxJobSize(FunctionCode, Type);
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

                AddToValuesToWrite(tag);

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
                DateTime dt = DataboomProtocol.GetDateTimeUtcNowNoMSec();

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
        //            DataboomTag DataboomTag = (DataboomTag)TagsList[TagIndex];
        //            if (DataboomTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
        //            {
        //                if (isProtocolBool())
        //                {
        //                    if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset))
        //                        changed.Add(DataboomTag);
        //                }
        //                else
        //                {
        //                    uint ArraySize = DataboomTag.TagNode.ArrayDimension;
        //                    if (ArraySize == 0)
        //                        ArraySize = 1;
        //                    byte[] tmpData = new byte[ArraySize];
        //                    DataboomTag.setMemRW(rec, (int)DataboomTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
        //                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
        //                    {
        //                        bool valBool = DataboomTag.getBoolValueFromMemRW((int)DataboomTag.ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
        //                        tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
        //                    }

        //                    if (DataboomTag.SetTagValue(ref tmpData, 0))
        //                        changed.Add(DataboomTag);
        //                }
        //            }
        //            else
        //            {
        //                if (isProtocolBool())
        //                {
        //                    if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
        //                        changed.Add(DataboomTag);
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
        //                        if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset, elemsize))
        //                            changed.Add(DataboomTag);
        //                    }
        //                    else
        //                    {
        //                        UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)DataboomTag.TagNode.DataType.Identifier);
        //                        uint ArraySize = DataboomTag.TagNode.ArrayDimension;
        //                        if (ArraySize == 0)
        //                            ArraySize = 1;
        //                        byte[] tmpData = new byte[sizeTmpData * ArraySize];
        //                        int indexTmpData = 0;
        //                        DataboomTag.setMemRW(rec, (int)DataboomTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
        //                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
        //                        {
        //                            Array.Copy(rec, DataboomTag.ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
        //                            indexTmpData += sizeTmpData;
        //                        }

        //                        if (DataboomTag.SetTagValue(ref tmpData, 0))
        //                            changed.Add(DataboomTag);
        //                    }
        //                }
        //            }
        //        }

        //        FirstTime = false;
        //    }
        //}

        private void AddToValuesToWrite(Tag tag)
        {
            DataboomProtocol.DataValue currentDataValue = new DataboomProtocol.DataValue(tag, DataboomProtocol.GetDateTimeUtcNowNoMSec());

            lock (lockValuesToWrite)
            {
                if (_ValuesToWrite.Count == 0)
                {
                    _ValuesToWrite.Add(currentDataValue);
                    return;
                }

                // if last added value is inside current sampling interval, remove it ad add current 
                // on sampling interval, only one value is admitted                 
                if (HasSameSamplingInterval(currentDataValue, _ValuesToWrite[_ValuesToWrite.Count - 1]))
                    _ValuesToWrite.RemoveAt(_ValuesToWrite.Count - 1);
                
                _ValuesToWrite.Add(currentDataValue);
            }            
            
            if (!IsPending && IsNrPendindDataTooMuch(currentDataValue.SamplingTime))
                RemoveOldestDataValue();
        }                   

        public bool GetDataValue(DateTime currentExecutionTime, out string signalValue)
        {
            signalValue = string.Empty;

            // requesting a data already published (--> not present) !!! 
            if (currentExecutionTime < _LastDataWrittenTime)
                return false;

            DataboomProtocol.DataValue dataValue = null;
            
            lock (lockValuesToWrite)
            {
                if (_ValuesToWrite.Count == 0)
                    dataValue = new DataboomProtocol.DataValue(TagsList[0], DataboomProtocol.GetDateTimeUtcNowNoMSec());
                else
                {
                    dataValue = _ValuesToWrite[0];
                    foreach (DataboomProtocol.DataValue tagValue in _ValuesToWrite)
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


        private bool HasSameSamplingInterval(DataboomProtocol.DataValue value1, DataboomProtocol.DataValue value2)
        {
            //DateTime currentExecutionTime1 = GetCurrentScheduleTime(value1.SamplingTime);
            //DateTime nextExecutionTime2 = GetNextScheduleTime(value1.SamplingTime);

            // if last added value is inside current sampling interval, remove it ad add current 
            // on sampling interval, only one value is admitted 
            //return (value2.SamplingTime >= currentExecutionTime && value2.SamplingTime < nextExecutionTime);
            return (GetCurrentScheduleTime(value1.SamplingTime) == GetCurrentScheduleTime(value2.SamplingTime) && GetNextScheduleTime(value1.SamplingTime) == GetNextScheduleTime(value2.SamplingTime));
        }

        private bool IsNrPendindDataTooMuch(DateTime dt)
        {
            //uint nrPendingData = (uint)((GetCurrentScheduleTime(dt).Subtract(_LastDataWrittenTime).TotalSeconds) / FrequencyOfSendingSignal);
            //return (Properties.Settings.Default.QueueWriteValuesMaxSize != 0 && nrPendingData > Properties.Settings.Default.QueueWriteValueMaxSize);

            TimeSpan pendingData = GetCurrentScheduleTime(dt).Subtract(_LastDataWrittenTime);
            return (Properties.Settings.Default.QueueWriteValuesMaxSize != 0 && (uint)pendingData.TotalSeconds > Properties.Settings.Default.QueueWriteValuesMaxSize);
        }

        public void RemoveOldestDataValue()
        {
            RemoveDataValue(_LastDataWrittenTime);
            _LastDataWrittenTime = _LastDataWrittenTime.Add(_FrequencyOfSendingSignal);
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
        private List<DataboomProtocol.DataValue> _ValuesToWrite;
        public List<DataboomProtocol.DataValue> ValuesToWrite
        {
            get { return _ValuesToWrite; }
            set { _ValuesToWrite = value; }
        }
        #endregion
    }
}
