using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace BrPvi
{
    public class BrPviCommJob : CommJob
    {
        public enum BrPviState : int
        {
            None = 0,
            StationPviObjectInitialization,     // subscribe station to PVI server
            JobPviObjectInitialization,         // subscribe job to PVI server
            JobStringLengthRequest,             // job's string's length request to PVI server
            JobInitialized,                     // job's subscription terminated --> active
            JobUnMapped,                        // job's subscription failed (tag don't exist into plc) --> stop to send new subscrition request because server will not respond
        }

        public enum CommandTypes : int
        {
            Invalid = -1,
            Init,       // bacnet object initialization
            ReadCmd,    // read polling
            WriteCmd,   // write
        }

        public enum PviObjectState
        {
            NotInitialized,
            Activated,
            DeActivated
        }

        #region Constructors

        public BrPviCommJob(Station station, BrPviCommJobSettings settings)
            : base(station, settings)
        {
            BrPviVariableName = settings.BrPviVariableName;
            BrPviTaskName = settings.BrPviTaskName;
            BrPviRefreshRate = settings.BrPviRefreshRate;
            BrPviArrayLength = settings.BrPviArrayLength;
            //BrPviChannel tcChannel = station.GetChannel() as BrPviChannel;
            _BrpviState = BrPviState.None;
            CheckJobValid();
        }

        public BrPviCommJob(Station station, BrPviTag defTag)
            : base(station, defTag)
        {
            BrPviVariableName = defTag.BrPviDynSettings.BrPviVariableName;
            BrPviTaskName = defTag.BrPviDynSettings.BrPviTaskName;
            BrPviRefreshRate = defTag.BrPviDynSettings.BrPviRefreshRate;
            BrPviArrayLength = defTag.BrPviDynSettings.BrPviArrayLength;
            _BrpviState = BrPviState.None;
            CheckJobValid();
        }

        public BrPviCommJob(Station station)
            : base(station)
        {
            BrPviVariableName = String.Empty;
            BrPviTaskName = String.Empty;
            BrPviRefreshRate = 0;
            BrPviArrayLength = 0;
            _BrpviState = BrPviState.None;
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
            return(Properties.Settings.Default.MaxJobSize);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob,
                                                           out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob,
                                          JobAggregationType AggType,
                                          uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new BrPviTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public override bool IsReadRWReady()
        {
            return (false);
        }

        #endregion

        #region Data members
        public PviObjectState pviObjectState = PviObjectState.NotInitialized; //false;
        public string BrPviTaskCompleteName = String.Empty;
        
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

        public void CheckBrPviState()
        {
            if (_BrpviState == BrPviState.JobInitialized || _BrpviState == BrPviState.JobUnMapped)
                return;

            if (_BrpviState == BrPviState.None)
                _BrpviState = BrPviState.StationPviObjectInitialization;

            if (_BrpviState == BrPviState.StationPviObjectInitialization)
            {
                if (((BrPviStation)Station).pviObjectsInitialized)
                    _BrpviState = BrPviState.JobPviObjectInitialization;
                else
                    return; // try to inizialize
            }
            
            if (_BrpviState == BrPviState.JobPviObjectInitialization)
            {
                if (this.pviObjectState == PviObjectState.Activated)
                    _BrpviState = BrPviState.JobStringLengthRequest;                
                else
                    return; // try to inizialize
            }

            if (_BrpviState == BrPviState.JobStringLengthRequest)
            {
                if (!this.MustRequestStringLength())
                    _BrpviState = BrPviState.JobInitialized;
                else
                    return; // send request to server to obtain string length
            }            
        }

        public bool IsBrPviJobInitialized()
        {
            return (_BrpviState == BrPviCommJob.BrPviState.JobInitialized);
        }

        public bool IsBrPviJobInitialiazionState()
        {
            return (_BrpviState == BrPviCommJob.BrPviState.JobPviObjectInitialization || _BrpviState == BrPviCommJob.BrPviState.JobStringLengthRequest);
        }

        public bool IsSubscribeState()
        {
            return (_BrpviState == BrPviCommJob.BrPviState.None || _BrpviState == BrPviCommJob.BrPviState.StationPviObjectInitialization || _BrpviState == BrPviCommJob.BrPviState.JobPviObjectInitialization);
        }

        public bool IsStationInitialized()
        {
            return (((BrPviStation)Station).pviObjectsInitialized);
        }

        public void GetReadWriteState()
        {
            if (ReadRequest())
            {
                if (IsBrPviObjectInitializationDone())
                {
                    _CommandType = BrPviCommJob.CommandTypes.ReadCmd;
                }
                else
                {
                    _CommandType = BrPviCommJob.CommandTypes.Init;
                }
            }
            else
            {
                if (IsBrPviObjectInitializationDone())
                    _CommandType = BrPviCommJob.CommandTypes.WriteCmd;
                else
                    _CommandType = BrPviCommJob.CommandTypes.Init;
            }
        }
        
        //public bool MustSendARequest()
        //{
        //    bool returnValue = false;
        //    if(pviObjectState != PviObjectState.Activated || conditionalVariableHasBeenSet || MustWrite() || MustRequestStringLength()) // || MustInitializeData) //!
        //    {
        //        System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviCommJob.MustSendARequest {0} - Returns true - pviObjectsInitialized: {1} - conditionalVariableHasBeenSet {2} - MustWrite(): {3} - MustRequestStringLength(): {4} - MustInitializeData: {5} - Pvi Obj: {6}",
        //                                           DateTime.Now.ToString("HH:mm:ss.fff"), pviObjectState, conditionalVariableHasBeenSet, MustWrite(), MustRequestStringLength(), "Not implemented", _BrPviVariableName);
        //        returnValue = true;
        //    }
        //    return (returnValue);
        //}

        //public bool MustWrite()
        //{
        //    bool returnValue = false;
        //    if ((Type == LinkType.UnconditionalOutput) || ((Type != LinkType.Input) && (TagsListToWrite.Count > 0)))
        //    {
        //        returnValue = true;
        //    }
        //    return (returnValue);
        //}

        //public bool MustRead()
        //{            
        //    return ((((Type == LinkType.InputOutput) && (TagsListToWrite.Count == 0)) || (Type == LinkType.Input)) && (conditionalVariableHasBeenSet || SyncroExec)); // || MustInitializeData
        //}

        public bool MustRequestStringLength()
        {
            lock (lockListObject)
            {
                return (TagsList.Count > 0 && (uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.String && BrPviArrayLength == 0);
            }
        }

        public bool CreatePviObjects()
        {
            return ((BrPviStation)Station).CreateJobPviObjects(this);
        }
        
        public void SetPviObjectEvMask(BrPviCommJob job, PviObjectState pviObjectState)
        {
            job.pviObjectState = pviObjectState;
            BrPviStation brStation = (BrPviStation)Station;
            brStation.SetPviObjectEvMask(this);
        }
        
        public void ManageCreationEvent(BrPviPviObject pviObj)
        {
            if (pviObj.LastError != 0 && !InErrorState)
            {
                //Status = BrPviCommJobStatus.Idle;
                if (pviObjectState != BrPviCommJob.PviObjectState.NotInitialized) //true
                    pviObjectState = BrPviCommJob.PviObjectState.NotInitialized; //false

                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = this;
                if (!pviObj.IsLastErrorFatal())
                    e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeCreateFailure;
                else
                {
                    e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                }
                ((BrPviChannel)this.Station.GetChannel()).OnJobExecuted(e);
            }
            //else
            //{
            //    if (pviObj.IsCpuMessage())
            //    {
            //        if (pviObjectState == BrPviCommJob.PviObjectState.NotInitialized)
            //            pviObjectState = BrPviCommJob.PviObjectState.Activated; // true
            //    }
            //}
        }

        public void ManagePviErrorEvent(BrPviPviObject pviObj, bool restartScheduler = false)
        {
            // Error
            if (pviObj.LastError != 0 && !InErrorState)
            {                
                //Status = BrPviCommJobStatus.Idle;
                BrPviDriver brCommDriver = (BrPviDriver)Station.GetCommDriver();
                brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorReceivedErrorEvent, pviObj.LastError, pviObj.Name);
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = this;
                if (!pviObj.IsLastErrorFatal())
                {
                    //The object (variable) can't be identified on the controller
                    if (pviObj.LastError == BrPviProcol.PVI_EVENT_IDENTIFICATION_ERROR || pviObj.LastError == BrPviProcol.PVI_EVENT_INVALID_OBJECT_INFORMATION_ERROR)
                    {
                        e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError;
                        pviObj.Status = PviObjectStatus.UnMappedOnPlc;                        
                    }
                    else
                    {
                        e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeEventError;
                    }
                }
                else
                {
                    e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                }
                ((BrPviChannel)this.Station.GetChannel()).OnJobExecuted(e);

                if (restartScheduler)
                    ((BrPviChannel)this.Station.GetChannel()).RestartScheduler();
            }
        }

        public uint GetExchangedBytesNumber()
        {
            uint exchangedBytes = 0;
            lock (lockListObject)
            {
                if (TagsList.Count > 0)
                {
                    exchangedBytes = TagsList[0].Size;
                }
            }

            return (exchangedBytes);
        }

        public void SetStringLength(uint stringLength, bool force = false)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviCommJob.SetStringLength {0} - Has been called for PviObj: {1} with String Length = {2}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), BrPviVariableCompleteName, stringLength);
            lock (lockListObject)
            {
                if(TagsList.Count > 0)
                {
                    if ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                    {
                        if((_BrPviArrayLength == 0) && (TagsList[0].Size == 0))
                        {
                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviCommJob.SetStringLength {0} - PviObj: {1} - Setting String Length to: {2}",
                                                               DateTime.Now.ToString("HH:mm:ss.fff"), BrPviVariableCompleteName, stringLength);
                            if (TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                TagsList[0].Size = stringLength;
                                BrPviArrayLength = stringLength;
                            }
                            else
                            {
                                TagsList[0].Size = stringLength * TagsList[0].TagNode.ArrayDimension;
                                BrPviArrayLength = stringLength * TagsList[0].TagNode.ArrayDimension;
                            }
                            //MustInitializeData = true;
                        }
                    }
                }
            }
        }

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
            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
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

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(
                                           Properties.Resources.ErrorJobTooBig,
                                           tagnamelist);
                return;
            }

            //if (!AddressObj.IsValid)
            //{
            //    IsValid = false;
            //    InvalidReason = string.Format(
            //                  Properties.Resources.ErrorInvalidAssignedAddress,
            //                  Address);
            //    return;
            //}

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public void ResetBrPviInternalSettings()
        {
            pviObjectState = PviObjectState.NotInitialized;
            _BrpviState = BrPviState.None;
            SetStringLength(0, true);
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

        private bool IsBrPviObjectInitializationDone()
        {
            return (_BrpviState == BrPviState.JobInitialized);
        }

        public bool HasPendingWriteCmd()
        {
            //return (_CommandType == CommandTypes.Init && TagsListToWrite.Count > 0 && IsBrPviObjectInitializationDone());
            return (TagsListToWrite.Count > 0 && IsBrPviObjectInitializationDone());
        }

        public override uint getProtocolDataType()
        {
            return (0);
        }

        public object retLockList()
        {
            return lockListObject;
        }

        public byte[] GetLastReadValue()
        {
            BrPviPviObject pviObj = ((BrPviChannel)this.Station.GetChannel()).GetPviObject(this.BrPviVariableCompleteName);

            if (pviObj != null)
                return pviObj.DataArray;
            else
                 return new byte[0];
        }

        public void CalculateBrPviVariableName(out string taskCompleteName, out string taskDescription, out string variableName, out string variableDescription)
        {
            // Task
            taskCompleteName = String.Empty;
            taskDescription = String.Empty;

            if (!String.IsNullOrWhiteSpace(BrPviTaskName))
            {
                int spaceIndex = BrPviTaskName.IndexOf(" ");
                if (spaceIndex < 0)
                {
                    taskCompleteName = String.Format("{0}/{1}", ((BrPviStation)Station).BrPviCpuName, BrPviTaskName);
                    taskDescription = String.Format("CD={0}", BrPviTaskName);
                }
                else
                {
                    taskCompleteName = String.Format("{0}/\"{1}\"", ((BrPviStation)Station).BrPviCpuName, BrPviTaskName);
                    taskDescription = String.Format("CD=\"{0}\"", BrPviTaskName);
                }                
            }


            // Variable
            variableName = String.Empty;
            variableDescription = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskCompleteName))
            {
                variableName = String.Format("{0}/{1}", taskCompleteName, BrPviVariableName);
            }
            else
            {
                variableName = String.Format("{0}/{1}", ((BrPviStation)Station).BrPviCpuName, BrPviVariableName);
            }
            //BrPviVariableCompleteName = variableName;

            if ((Type == LinkType.Input) || (Type == LinkType.InputOutput))
            {
                variableDescription = String.Format("CD=\"{0}\" RF={1}", BrPviVariableName, BrPviRefreshRate);
            }
            else
            {
                variableDescription = String.Format("CD=\"{0}\" AT=w RF={1}", BrPviVariableName, BrPviRefreshRate);
            }
        }

        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Properties

        /// <summary>
        /// PVI Variable Name
        /// </summary>
        private string _BrPviVariableName;
        public string BrPviVariableName
        {
            get
            {
                return _BrPviVariableName;
            }

            set
            {
                _BrPviVariableName = value;
            }
        }

        /// <summary>
        /// PVI Task Name
        /// </summary>
        private string _BrPviTaskName;
        public string BrPviTaskName
        {
            get
            {
                return _BrPviTaskName;
            }

            set
            {
                _BrPviTaskName = value;
            }
        }

        /// <summary>
        /// Refresh Rate
        /// </summary>
        private int _BrPviRefreshRate;
        public int BrPviRefreshRate
        {
            get { return _BrPviRefreshRate; }
            set { _BrPviRefreshRate = value; }
        }

        /// <summary>
        /// Array Length
        /// </summary>
        private uint _BrPviArrayLength;
        public uint BrPviArrayLength
        {
            get { return _BrPviArrayLength; }
            set { _BrPviArrayLength = value; }
        }

        ///// <summary>
        ///// Status
        ///// </summary>
        //private BrPviCommJobStatus _Status;
        //public BrPviCommJobStatus Status
        //{
        //    get
        //    {
        //        return _Status;
        //    }

        //    set
        //    {
        //        _Status = value;
        //    }
        //}

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return (ret);
            }
        }               

        private BrPviState _BrpviState = BrPviState.None;        
        public BrPviState BrpviState
        {
            get { return _BrpviState; }
            set { _BrpviState = value; }
        }

        CommandTypes _CommandType = CommandTypes.Invalid;
        public CommandTypes CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        public string _BrPviVariableCompleteName = String.Empty;
        public string BrPviVariableCompleteName
        {
            get {
                if (string.IsNullOrEmpty(_BrPviVariableCompleteName))
                {
                    CalculateBrPviVariableName(out string taskCompleteName, out string taskDescription, out string variableName, out string variableDescription);
                    
                    _BrPviVariableCompleteName = variableName;
                }
                return _BrPviVariableCompleteName;             
            }
            set { _BrPviVariableCompleteName = value; }
        }

        public override uint SamplingInterval
        {
            get
            {
                if (ConditionalVariableSet && IsConditionalVariableOn())
                    return _SamplingInterval;

                if (BrpviState == BrPviState.JobInitialized || BrpviState == BrPviState.JobUnMapped)
                    return CommJob.JOB_NOT_SCHEDULABLE;

                return _SamplingInterval;
            }
        }
        #endregion
    }
}
