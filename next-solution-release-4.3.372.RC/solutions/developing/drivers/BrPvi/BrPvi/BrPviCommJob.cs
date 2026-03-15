using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.IO;

namespace BrPvi
{
    public enum BrPviCommJobStatus : byte
    {
        Idle,
        ReadRequestPending,
        WriteRequestPending,
        StringLengthRequestPending,
        WaitingToSendRequest
    }

    public class BrPviCommJob : CommJob
    {
        #region Constructors

        public BrPviCommJob(Station station, BrPviCommJobSettings settings)
            : base(station, settings)
        {
            BrPviVariableName = settings.BrPviVariableName;
            BrPviTaskName = settings.BrPviTaskName;
            BrPviRefreshRate = settings.BrPviRefreshRate;
            BrPviArrayLength = settings.BrPviArrayLength;
            //BrPviChannel tcChannel = station.GetChannel() as BrPviChannel;
            _Status = BrPviCommJobStatus.Idle;
            _IsSyncroJob = false;
            CheckJobValid();
        }

        public BrPviCommJob(Station station, BrPviTag defTag)
            : base(station, defTag)
        {
            BrPviVariableName = defTag.BrPviDynSettings.BrPviVariableName;
            BrPviTaskName = defTag.BrPviDynSettings.BrPviTaskName;
            BrPviRefreshRate = defTag.BrPviDynSettings.BrPviRefreshRate;
            BrPviArrayLength = defTag.BrPviDynSettings.BrPviArrayLength;
            _Status = BrPviCommJobStatus.Idle;
            _IsSyncroJob = false;
            CheckJobValid();
        }

        public BrPviCommJob(Station station)
            : base(station)
        {
            BrPviVariableName = String.Empty;
            BrPviTaskName = String.Empty;
            BrPviRefreshRate = 0;
            BrPviArrayLength = 0;
            _Status = BrPviCommJobStatus.Idle;
            _IsSyncroJob = false;
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

        #endregion

        #region Data members

        public enum PviObjectState
        {
            NotInitialized,
            Activated,
            DeActivated
        }

        public object lockNewData = new object();
        public object NewDataValue = new object();

        public PviObjectState pviObjectState = PviObjectState.NotInitialized; //false;
        public string BrPviVariableCompleteName = String.Empty;
        public string BrPviTaskCompleteName = String.Empty;
        public bool MustInitializeData = false;

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
        public bool MustSendARequest()
        {
            bool returnValue = false;
            if(pviObjectState != PviObjectState.Activated || conditionalVariableHasBeenSet || MustWrite() || MustRequestStringLength() || MustInitializeData) //!
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviCommJob.MustSendARequest {0} - Returns true - pviObjectsInitialized: {1} - conditionalVariableHasBeenSet {2} - MustWrite(): {3} - MustRequestStringLength(): {4} - MustInitializeData: {5} - Pvi Obj: {6}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObjectState, conditionalVariableHasBeenSet, MustWrite(), MustRequestStringLength(), MustInitializeData, _BrPviVariableName);
                returnValue = true;
            }
            return (returnValue);
        }

        public bool MustWrite()
        {
            bool returnValue = false;
            if ((Type == LinkType.UnconditionalOutput) || ((Type != LinkType.Input) && (TagsListToWrite.Count > 0)))
            {
                returnValue = true;
            }
            return (returnValue);
        }

        public bool MustRead()
        {
            bool returnValue = false;
            if ((((Type == LinkType.InputOutput) && (TagsListToWrite.Count == 0)) || (Type == LinkType.Input)) &&
                (conditionalVariableHasBeenSet || SyncroExec || MustInitializeData))
            {
                returnValue = true;
            }
            return (returnValue);
        }

        public bool MustRequestStringLength()
        {
            bool retValue = false;
            lock(lockListObject)
            {
                if (TagsList.Count > 0)
                {
                    if((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                    {
                        if(BrPviArrayLength == 0)
                        {
                            retValue = true;
                        }                        
                    }
                }
            }

            return (retValue);
        }

        public void CreatePviObjects()
        {
            BrPviStation brStation = (BrPviStation)Station;
            brStation.CreateJobPviObjects(this);
        }
        
        public void SetPviObjectEvMask(BrPviCommJob job, PviObjectState pviObjectState)
        {
            job.pviObjectState = pviObjectState;
            BrPviStation brStation = (BrPviStation)Station;
            brStation.SetPviObjectEvMask(this);
        }
        
        public void ManageCreationEvent(BrPviPviObject pviObj)
        {
            if (pviObj.LastError != 0)
            {
                Status = BrPviCommJobStatus.Idle;
                if (pviObjectState != BrPviCommJob.PviObjectState.NotInitialized) //true
                {
                    pviObjectState = BrPviCommJob.PviObjectState.NotInitialized; //false
                }
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = this;
                if (pviObj.LastError != (uint)DriverErrorCodes.ErrorTimeOut)
                {
                    e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeCreateFailure;
                }
                else
                {
                    e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                }
                BrPviStation brStation = (BrPviStation)Station;
                //brStation.BaseProcessJobValues(e);
                brStation.OnJobExecuted(e.Job,e);
            }
            else
            {
                if (pviObj.Type == PviObjectTypes.POBJ_PVAR)
                {
                    if  (pviObjectState == BrPviCommJob.PviObjectState.NotInitialized)
                        pviObjectState = BrPviCommJob.PviObjectState.Activated; // true
                }
            }
        }

        public void ManagePviErrorEvent(BrPviPviObject pviObj)
        {
            // Error
            if (pviObj.LastError != 0)
            {
                // if station was suspend don't report any communication error
                bool suspendBitNewValue = false;
                if (Station.GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
                {                    
                    if (suspendBitNewValue)
                    {
                        // Link broken --> enable driver to reload to PVIMonitor all tags 
                        if (pviObj.LastError == PviComManager.PVI_ERROR_LINK_BROKEN)
                            ((BrPviStation)Station).pviObjectsInitialized = false;
                        return;
                    }
                }

                Status = BrPviCommJobStatus.Idle;
                BrPviDriver brCommDriver = (BrPviDriver)Station.GetCommDriver();
                brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorReceivedErrorEvent, pviObj.LastError, pviObj.Name);
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = this;
                if (pviObj.LastError != (uint)DriverErrorCodes.ErrorTimeOut)
                {
                    //The object (variable) can't be identified on the controller
                    if (pviObj.LastError == 4813 || pviObj.LastError == 4806)
                    {
                        e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeIdentificationError;
                        pviObj.Status = PviObjectStatus.UnMappedOnPlc;                        
                    }
                    else
                    {
                        e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeEventError;
                    }
                }
                else
                {
                    e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                }
                BrPviStation brStation = (BrPviStation)Station;
                //brStation.BaseProcessJobValues(e);
                brStation.OnJobExecuted(e.Job,e);
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

        public void SetStringLength(uint stringLength)
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
                            MustInitializeData = true;
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
      
        public override uint getProtocolDataType()
        {
            return (0);
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

        /// <summary>
        /// Status
        /// </summary>
        private BrPviCommJobStatus _Status;
        public BrPviCommJobStatus Status
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

        private bool _IsSyncroJob;
        public bool IsSyncroJob
        {
            get
            {
                return _IsSyncroJob;
            }

            set
            {
                _IsSyncroJob = value;
            }
        }

        #endregion
    }
}
