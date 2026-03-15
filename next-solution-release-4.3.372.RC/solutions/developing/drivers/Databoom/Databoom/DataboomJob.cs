using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using Utilities;


namespace Databoom
{
    public enum DataboomCommJobStatus : byte
    {
        Idle,
        PublishRequestPending,
        PublishReplyReceived,
        PublishError
    }

    public class Signal
    {
        public string name { get; set; }

        public string value { get; set; }
    }


    public class DataboomCommJob : CommJob
    {
        #region Constructors
        public DataboomCommJob(Station station, DataboomCommJobSettings settings)
            : base(station, settings)
        {
            _TagName = settings.TagName;
            _signalTag = new Signal();
            _signalTag.name = _TagName;
            _signalTag.value = string.Empty;
            _lastExecutionTimeSched = DateTime.MinValue;
            _FrequencyOfSendingSignal = settings.FrequencyOfSendingSignal;
            CheckJobValid();
        }

        public DataboomCommJob(Station station, DataboomTag defTag)
            : base(station, defTag)
        {
            _signalTag = new Signal();
            _TagName = defTag.DataboomDynSettings.TagName;
            _FrequencyOfSendingSignal = defTag.DataboomDynSettings.FrequencyOfSendingSignal;
            _lastExecutionTimeSched = DateTime.MinValue;
            _signalTag.name = _TagName;
            _Status = DataboomCommJobStatus.Idle;

            CheckJobValid();
        }

        public DataboomCommJob(Station station)
            : base(station)
        {
            _signalTag = new Signal();
            _signalTag.name = string.Empty;
            _signalTag.value = string.Empty;
            _TagName = String.Empty;
            _lastExecutionTimeSched = DateTime.MinValue;
            _FrequencyOfSendingSignal = 0;
            _Status = DataboomCommJobStatus.Idle;

            CheckJobValid();
        }

        protected DataboomCommJob()
        {
            _signalTag = new Signal();
            _signalTag.name = string.Empty;
            _signalTag.value = string.Empty;
            _TagName = String.Empty;
            _lastExecutionTimeSched = DateTime.MinValue;
            _FrequencyOfSendingSignal = 0;
            _Status = DataboomCommJobStatus.Idle;

            CheckJobValid();
        }
        #endregion
        #region data Member



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

        #region override Methods

        #endregion

        #region Methods
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
                    default:
                        return 1;
                }
            }
            return 0;
        }

        public bool MustPublish()
        {
            bool returnValue = false;
            if ((Type != LinkType.Input) &&
                ((TagsListToWrite.Count > 0) || (Type == LinkType.UnconditionalOutput)))
            {
                returnValue = true;
            }

            return (returnValue);
        }

        public void GetMessageToBePublished(ref string jobMessage, Guid token)
        {
            jobMessage = String.Empty;
            Tag cand;

            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    return;
                }
                cand = TagsListOnWriting[0];
            }

            if (cand == null)
            {
                return;
            }

            if (cand.DynSettings.MethodID != -1)
            {
                return;
            }

            // Prepare output data
            var extDataValue = new ExtDataValue(cand.Value, token);
            string outputData = extDataValue.ToXml();
            jobMessage = String.Format("{0}|{1}|{2}", TagName, cand.TagNode.NodeId.ToString(), outputData);
        }

        public string getTagNodeId()
        {
            string tagNodeId = String.Empty;
            Tag cand;
            lock (lockListObject)
            {
                if (TagsList.Count > 0)
                {
                    cand = TagsList[0];
                    tagNodeId = cand.TagNode.NodeId.ToString();
                }
            }

            return (tagNodeId);
        }

        public bool ConvertReceivedValue(string receivedValue, ref Object convertedValue)
        {
            bool returnValue = false;
            uint tagDataType = 0;
            lock (lockListObject)
            {
                if (TagsList.Count > 0)
                {
                    tagDataType = (uint)TagsList[0].TagNode.DataType.Identifier;
                    switch (tagDataType)
                    {
                        case (uint)BuiltInType.Boolean:
                            {
                                bool result = false;
                                if (bool.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Byte:
                            {
                                byte result = 0;
                                if (byte.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Double:
                            {
                                double result = 0;
                                if (double.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Float:
                            {
                                float result = 0;
                                if (float.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Int16:
                            {
                                Int16 result = 0;
                                if (Int16.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Int32:
                            {
                                Int32 result = 0;
                                if (Int32.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Int64:
                            {
                                Int64 result = 0;
                                if (Int64.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.Integer:
                            {
                                int result = 0;
                                if (int.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.SByte:
                            {
                                SByte result = 0;
                                if (SByte.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.UInt16:
                            {
                                UInt16 result = 0;
                                if (UInt16.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.UInt32:
                            {
                                UInt32 result = 0;
                                if (UInt32.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.UInt64:
                            {
                                UInt64 result = 0;
                                if (UInt64.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.UInteger:
                            {
                                uint result = 0;
                                if (uint.TryParse(receivedValue, out result) == true)
                                {
                                    convertedValue = result;
                                    returnValue = true;
                                }
                            }
                            break;
                        case (uint)BuiltInType.String:
                            convertedValue = receivedValue;
                            returnValue = true;
                            break;
                    }
                }
            }
            return (returnValue);
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

        // Added to solve FOGBUGZ 10394
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

        public void manageTagsListToWrite()
        {
            lock (lockListObject)
            {
                if((TagsListOnWriting.Count == 0) && (TagsList.Count > 0) && (TagsList[0] != null))
                {
                    TagsListOnWriting.Add(TagsList[0]);
                }
                TagsListToWrite.Clear();
            }
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
                jobdata = new byte[nData];
                cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
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
                        {
                            TagsListToWrite.Add(tag);
                        }
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                        {
                            TagsListToWrite.Add(tag);
                        }
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

        public void SetJobValue(string jobData, ref List<Tag> changed)
        {
            ExtDataValue extReceivedData = jobData.FromXml<ExtDataValue>();
            DataValue receivedData = (DataValue)extReceivedData;
            lock (lockListObject)
            {
                if (TagsList.Count > 0)
                {
                    if (!receivedData.Equals(TagsList[0].Value.Value))
                    {
                        TagsList[0].Value.Value = Utils.Clone(receivedData.Value);
                        TagsList[0].Value.StatusCode = receivedData.StatusCode;
                        TagsList[0].Value.SourceTimestamp = receivedData.SourceTimestamp;
                        TagsList[0].Value.ServerTimestamp = receivedData.ServerTimestamp;
                        changed.Add(TagsList[0]);
                    }
                }
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
                    DataboomTag DataboomTag = (DataboomTag)TagsList[TagIndex];
                    if (DataboomTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset))
                                changed.Add(DataboomTag);
                        }
                        else
                        {
                            uint ArraySize = DataboomTag.TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            DataboomTag.setMemRW(rec, (int)DataboomTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = DataboomTag.getBoolValueFromMemRW((int)DataboomTag.ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (DataboomTag.SetTagValue(ref tmpData, 0))
                                changed.Add(DataboomTag);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(DataboomTag);
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
                                if (DataboomTag.SetTagValue(ref rec, (int)DataboomTag.ByteOffset, elemsize))
                                    changed.Add(DataboomTag);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)DataboomTag.TagNode.DataType.Identifier);
                                uint ArraySize = DataboomTag.TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                DataboomTag.setMemRW(rec, (int)DataboomTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, DataboomTag.ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (DataboomTag.SetTagValue(ref tmpData, 0))
                                    changed.Add(DataboomTag);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public ushort elementOnWrite(IList<Tag> listOnWriting)
        {
            ushort ItemCount = 0;
            foreach (var tag in listOnWriting)
            {
                ItemCount += (ushort)(tag.TagNode.ArrayDimension == 0 ? 1 : tag.TagNode.ArrayDimension);
            }
            return ItemCount;
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

        #region Databoom CallBacks
        public void ManagePublishUserMessage(string userMessage)
        {
            //string resultMessage = String.Empty;
            //if (ParsePublishUserMessage(userMessage, out resultMessage) == true)
            //{
            //    AddPublishReplies(resultMessage);
            //}
            Status = DataboomCommJobStatus.PublishReplyReceived;
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = this;
            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            DataboomChannel pnChannel = (DataboomChannel)Station.GetChannel();
            pnChannel.OnJobPublished(eJob);
        }

        //public TimeSpan GetLastExecutionTimeAddWait()
        //{
        //    TimeSpan t = TimeSpan.FromSeconds(FrequencyOfSendingSignal);
        //    return (lastExecutionTimeSchede.Add(t));
        //}

        public Signal GetSignalValue()
        {
            if((TagsList.Count < 1) || (TagsList[0] == null))
            {
                _signalTag.value = String.Empty;
            }
            else
            {
                if(((uint)TagsList[0].TagNode.DataType.Identifier) == ((uint)BuiltInType.Boolean))
                {
                    if(TagsList[0].Value.Value.ToString() == "False")
                        _signalTag.value = "0";
                    else
                        _signalTag.value = "1";
                }
                else
                {
                    _signalTag.value = TagsList[0].Value.Value.ToString();
                }
                       
            }

            return (signalTag);
        }


        //        public void ManagePublishErrorMessage(DataboomClientError DataboomError)
        //        {
        //            string errorMessage = DataboomError.ToString();
        //            //string variableName = String.Empty;
        //            //if (ExtractVariableNameFromMessage(errorMessage, out variableName) == true)
        //            //{
        //            //    AddPublishErrors(variableName, errorMessage);
        //            //}
        //            Status = DataboomCommJobStatus.PublishError;
        //            ExecutedJobArgs eJob = new ExecutedJobArgs();
        //            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)DataboomErrorCodes.ErrorCodeErrorPublish;
        //            lock (retLockList())
        //            {
        //                TagsListOnWriting.Clear();
        //            }
        //            eJob.Job = this;
        //            DataboomChannel pnChannel = (DataboomChannel)Station.GetChannel();
        //            if(pnChannel != null)
        //            {
        //                pnChannel.OnJobPublished(eJob);
        //            }
        //#if DEBUG
        //            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
        //            System.Diagnostics.Debug.WriteLine(String.Format("Databoom DBG - {0} Publish Err Msg: {1} for tag {2}",
        //                                               currentTime, errorMessage, TagName));
        //#endif
        //        }

        #endregion

        #region Properties

        /// <summary>
        /// Tag Name
        /// </summary>
        private string _TagName;
        public string TagName
        {
            get
            {
                return _TagName;
            }

            set
            {
                _TagName = value;
            }
        }

        /// <summary>
        /// signalTag
        /// </summary>
        public Signal _signalTag;
        public Signal signalTag
        {
            get
            {
                return _signalTag;
            }
        }

        /// <summary>
        /// Status
        /// </summary>
        private DataboomCommJobStatus _Status;
        public DataboomCommJobStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        /// <summary>
        /// Tag Name
        /// </summary>
        private UInt32 _FrequencyOfSendingSignal;
        public UInt32 FrequencyOfSendingSignal
        {
            get
            {
                return _FrequencyOfSendingSignal;
            }

            set
            {
                _FrequencyOfSendingSignal = value;
            }
        }

        /// <summary>
        /// Tag Name
        /// </summary>
        private DateTime _lastExecutionTimeSched;
        public DateTime lastExecutionTimeSchede
        {
            get
            {
                return _lastExecutionTimeSched;
            }

            set
            {
                _lastExecutionTimeSched = value;
            }
        }
        #endregion
    }
}
