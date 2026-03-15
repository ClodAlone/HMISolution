////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Station.cs
//
// summary:	Implements the driver IEC60870_5_104 station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace IEC60870_5_104
{
    class MemoryDataAreas
    {
        #region Constructors

        public MemoryDataAreas(List<CommJob> ListJob)
        {
            MapIOAM = new Dictionary<UInt32, UInt64>();
            MapIOAC = new Dictionary<UInt32, UInt64>();
            MapCombinateAddress = new Dictionary<UInt64, int>();

            Jobs = new List<List<IEC60870_5_104CommJob>>();
            TimeStamps = new List<DateTime>();

            foreach (CommJob job in ListJob)
                addToMemory(job as IEC60870_5_104CommJob);
        }
        #endregion

        #region Data Members


        Dictionary<UInt32, UInt64> MapIOAM;
        Dictionary<UInt32, UInt64> MapIOAC;
        Dictionary<UInt64, int> MapCombinateAddress;

        List<List<IEC60870_5_104CommJob>> Jobs;
        List<DateTime> TimeStamps;

        #endregion

        #region Methods
        UInt32 CombinateIOA(UInt32 StartAdd, ASDUTypes ASDUType)
        {
            return (StartAdd & 0xffffff) + (UInt32)((byte)ASDUType << 24);
        }
        UInt32 CombinateAddress(UInt32 StartAddMon, UInt32 StartAddCtrl, ASDUTypes ASDUType)
        {
            return (StartAddMon & 0xffffff) + ((StartAddCtrl & 0xffffff) << 24) + (UInt32)((byte)ASDUType << 48);
        }
        void addToMemory(IEC60870_5_104CommJob job)
        {
            UInt64 Address = CombinateAddress(job.StartAddMon, job.StartAddCtrl, (ASDUTypes)job.ASDUType);
            if (!MapCombinateAddress.ContainsKey(Address))
            {
                MapCombinateAddress[Address] = Jobs.Count();
                MapIOAM[CombinateIOA(job.StartAddMon, (ASDUTypes)job.ASDUType)] = Address;
                MapIOAC[CombinateIOA(job.StartAddCtrl, (ASDUTypes)job.ASDUType)] = Address;
                Jobs.Add(new List<IEC60870_5_104CommJob>());
                TimeStamps.Add(default(DateTime));
            }
            Jobs[MapCombinateAddress[Address]].Add(job);
        }

        void ListAddRange(ref List<IEC60870_5_104CommJob> Jobs, List<IEC60870_5_104CommJob> addJobs)
        {
            foreach (IEC60870_5_104CommJob job in addJobs)
                if (!Jobs.Contains(job))
                    Jobs.Add(job);
        }

        public List<IEC60870_5_104CommJob> getChangedJobs(iecInfoObj InfoObj)
        {
            List<IEC60870_5_104CommJob> changeJobs = new List<IEC60870_5_104CommJob>();
            try
            {
                int memoryAddress = MapCombinateAddress[MapIOAM[CombinateIOA(InfoObj.informationAddress, InfoObj.basicType())]];
                ListAddRange(ref changeJobs, Jobs[memoryAddress]);
            }
            catch
            {
                changeJobs = new List<IEC60870_5_104CommJob>();
            }

            return changeJobs;

        }

        public void SetTimeStamp(iecInfoObj InfoObj, DateTime TimeStamp)
        {
            try
            {
                int memoryAddress = MapCombinateAddress[MapIOAM[CombinateIOA(InfoObj.informationAddress, InfoObj.basicType())]];
                if (TimeStamp == default(DateTime))
                    TimeStamps[memoryAddress] = DateTime.Now;
                else
                    TimeStamps[memoryAddress] = TimeStamp;
            }
            catch
            {
            }
        }

        public bool AllCounterInitialized()
        {
            for (int index = 0; index < TimeStamps.Count(); index++)
            {
                if (TimeStamps[index] == default(DateTime) && Jobs[index][0].ASDUType == ASDUSelectableTypes.IntegratedTotals &&
                    Jobs[index][0].Type != LinkType.ExceptionOutput && Jobs[index][0].Type != LinkType.UnconditionalOutput)
                    return false;
            }
            return true;
        }
        public bool AllGeneralInitialized()
        {
            for (int index = 0; index < TimeStamps.Count(); index++)
            {
                if (TimeStamps[index] == default(DateTime) && Jobs[index][0].ASDUType != ASDUSelectableTypes.IntegratedTotals &&
                    Jobs[index][0].Type != LinkType.ExceptionOutput && Jobs[index][0].Type != LinkType.UnconditionalOutput)
                    return false;
            }
            return true;
        }
        public void ClearTimeStamps()
        {
            for (int index = 0; index < TimeStamps.Count(); index++)
                TimeStamps[index] = default(DateTime);
        }

        #endregion
    }

    /// <summary>   Communication target device. </summary>
    class IEC60870_5_104Station : Station
    {
        public enum LinkStates
        {
            BEGIN,
            //WaitCONNECT,
            //StartTIMESYNC,
            CONNECT,
            TIMESYNC,
            //WaitTIMESYNC,
            //StartGENINTERR,
            GENINTERRConf,
            GENINTERREnd,
            COUNTINTERR,
            COUNTINTERREnd,
            EndInit,
            WaitTestConf,
            END
        }


        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="IEC60870_5_104StationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104Station(CommunicationDriver commdriver, IEC60870_5_104StationSettings settings)
            : base(commdriver, settings)
        {
            _CommonAddress = settings.CommonAddress;
            _MaxOutAPDU = settings.MaxOutAPDU;
            _MaxInAPDU = settings.MaxInAPDU;
            _MissingAckTimeout = settings.MissingAckTimeout;
            _NoInMsgTimeout = settings.NoInMsgTimeout;
            _NoActivityTimeout = settings.NoActivityTimeout;
            _EnablePeriodicReset = settings.EnablePeriodicReset;
            _FileTransferDirectory = settings.FileTransferDirectory;

            _LinkState = LinkStates.BEGIN;            
        }

        #endregion

        #region Data Members

        MemoryDataAreas memory;
        protected Object lockProcessJob = new Object();

        #endregion

        #region Methods

        public void createMemoryData()
        {
            memory = new MemoryDataAreas(ListWholeJob);
        }
        public List<IEC60870_5_104CommJob> getChangedJobs(iecInfoObj InfoObj)
        {
            return memory.getChangedJobs(InfoObj);
        }
        public void SetTimeStamp(iecInfoObj InfoObj, DateTime TimeStamp)
        {
            memory.SetTimeStamp(InfoObj, TimeStamp);
        }
        public bool AllCounterInitialized()
        {
            return memory.AllCounterInitialized();
        }
        public bool AllGeneralInitilized()
        {
            return memory.AllGeneralInitialized();
        }
        public void ClearTimeStamps()
        {
            if (memory != null)
                memory.ClearTimeStamps();
        }

        public ExecutedJobArgs ManageConnectionBroken(IEC60870_5_104ErrorCodes error = IEC60870_5_104ErrorCodes.ErrorConnectionBroken)
        {
            ExecutedJobArgs e = null;
            LastErrorCode = (DriverErrorCodes)error;
            if (InErrorState != true)
                InErrorState = true;
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var job in listJob)
                {
                    job.SetQuality(StatusCodes.BadCommunicationError);
                }
                e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)error;
                // Put all the jobs in error -- moved into Station.ProcessJobValues()
                //e.GeneralError = true;

                base.ProcessJobValues(e);
            }
            return e;
        }

        public void ManageConnectionRestored()
        {
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            if (InErrorState != false)
            {
                Channel.ConsecutiveCommErrors = 0;
                InErrorState = false;
            }
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var job in listJob)
                {
                    job.SetQuality(StatusCodes.Uncertain);
                    ((IEC60870_5_104CommJob)job).SetJobNotInErrorState();
                }
            }
        }

        public bool IsInitialized()
        {
            return (_LinkState == LinkStates.END);
        }

        internal IEC60870_5_104CommJob.IECStates CheckInitializationState()
        {
            IEC60870_5_104CommJob.IECStates state = IEC60870_5_104CommJob.IECStates.StationInitialization;

            if (_LinkState == LinkStates.BEGIN)
            {
                ((IEC60870_5_104Channel)GetChannel()).InitLinkLoop();

                _LinkState = LinkStates.CONNECT;
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.CONNECT");
            }

            if (_LinkState == LinkStates.CONNECT)
            {
                if (_StartDTReceived)
                {
                    if (((IEC60870_5_104Channel)GetChannel()).EnableInitialClockSynchronization)
                    {
                        _LinkState = LinkStates.TIMESYNC;
                        System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.TIMESYNC");
                    }
                    else
                    {
                        _LinkState = LinkStates.GENINTERRConf;
                        System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.GENINTERRConf");
                    }
                }
                else
                {
                    return state; // establish connection (StartDT)
                }
            }

            if (_LinkState == LinkStates.TIMESYNC)
            {
                if (_TimeSyncronized)
                {
                    _LinkState = LinkStates.GENINTERRConf;
                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.GENINTERRConf");
                }
                else
                    return state; // syncronize remote date-time 
            }

            if (_LinkState == LinkStates.GENINTERRConf)
            {
                if (_GeneralInterrConfReceived)
                {
                    _LinkState = LinkStates.GENINTERREnd;
                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.GENINTERREnd");
                }
                else
                    return state; // request a general interrogation
            }

            if (_LinkState == LinkStates.GENINTERREnd)
            {
                if (_GeneralInterrEndReceived)
                {
                    _LinkState = LinkStates.COUNTINTERR;
                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-CheckInitializationState-->set LinkStates.COUNTINTERR");
                }
                else
                    return state; // wait for the end of general interrogation
            }
            //COUNTINTERR,
            //COUNTINTERREnd,
            //EndInit,
            //WaitTestConf,
            if (_LinkState == LinkStates.END)
            {

            }

            return state;
        }

        public void PutAllJobsInError()
        {
            List<IEC60870_5_104CommJob> listJobs;
            lock (lockListObject)
            {
                listJobs = ListWholeJob.ConvertAll(x => (IEC60870_5_104CommJob)x).Where(a => a.InUse).ToList();
            }

            if (listJobs.Count > 0)
            {
                // deinitialize station
                ((IEC60870_5_104Channel)GetChannel()).InitLinkLoop();
                //_RunTimeDeviceInstance = -1;            

                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);

                foreach (IEC60870_5_104CommJob job in listJobs)
                    job.SetIecNetStateInError();
            }
        }

        #endregion

        #region Abstract Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from CommJobSettings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="JobSettings">  . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as IEC60870_5_104CommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new IEC60870_5_104CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from Tag. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="defTag">   . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as IEC60870_5_104Tag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new IEC60870_5_104CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new variable instance from TagDefinition. </summary>
        ///
        /// <param name="td">   . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new IEC60870_5_104Tag(td);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new settings instance for protocol's task. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as IEC60870_5_104CommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new IEC60870_5_104CommJobSettings(session, commJob);
        }

        #endregion

        #region Override Methods        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Processed received data in ExecutedJobArgs. To call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            IEC60870_5_104CommJob mJ = e.Job as IEC60870_5_104CommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (IEC60870_5_104Protocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                else
                    e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
            }

            e.GeneralError = (e.ErrorCode == (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorConnectionBroken);

            base.ProcessJobValues(e);

            if (e.ChangedTags.Count > 0)
            {
                foreach (var tag in e.ChangedTags)
                {
                    mJ.setQualityVariable((tag as IEC60870_5_104Tag).Quality);
                    mJ.setCotVariable((tag as IEC60870_5_104Tag).Cot);
                }
            }
        }        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivedbuffer">   . </param>
        /// <param name="job">              . </param>
        /// <param name="arguments">        [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            IEC60870_5_104CommJob mj = job as IEC60870_5_104CommJob;

            IEC60870_5_104Protocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Startup()
        {
            //ManageConnectionRestored();
            SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
            ClearTimeStamps();
            //_LinkState = LinkStates.GENINTERRConf;
            return base.Startup();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the value of an observed node id. </summary>
        ///
        /// <returns>   Updates the value of an observed node id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public override void UpdateObservedTag(NodeId node, DataValue value)
        //{
        //    base.UpdateObservedTag(node, value);

        //    // Update the value of file name variables of jobs
        //    if (ObservedTagToJobsMap.Keys.Contains(node) == true)
        //    {
        //        List<CommJob> jobList = ObservedTagToJobsMap[node];
        //        foreach (var job in jobList)
        //        {
        //            IEC60870_5_104CommJob iecJob = job as IEC60870_5_104CommJob;
        //            if (iecJob.FileNameVariableHasBeenSet && node.ToString() == iecJob.FileNameVariableId)
        //            { 
        //                iecJob.ManageUpdatedValueForTheFileNameVariable(value);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Properties

        /// <summary>   Common Address. </summary>
        private ushort _CommonAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Controlled station's Common Address. </summary>
        ///
        /// <value> Common Address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort CommonAddress
        {
            get
            {
                return _CommonAddress;
            }
            set
            {
                _CommonAddress = value;
            }
        }

        /// <summary>   Max Outstanding APDUs. </summary>
        private uint _MaxOutAPDU;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Max number of I outstanding APDUs before stopping transmission (k). </summary>
        ///
        /// <value> Max Outstanding APDUs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxOutAPDU
        {
            get
            {
                return _MaxOutAPDU;
            }
            set
            {
                _MaxOutAPDU = value;
            }
        }

        /// <summary>   Max Incoming APDUs. </summary>
        private uint _MaxInAPDU;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of I incoming APDUs after which an S acknowledge is sent (w). </summary>
        ///
        /// <value> Max Incoming APDUs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxInAPDU
        {
            get
            {
                return _MaxInAPDU;
            }
            set
            {
                _MaxInAPDU = value;
            }
        }

        /// <summary>   Missing ACK Timeout in ms. </summary>
        private uint _MissingAckTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Confirm timeout of send or test APDUs (t1). </summary>
        ///
        /// <value> Missing ACK Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MissingAckTimeout
        {
            get
            {
                return _MissingAckTimeout;
            }
            set
            {
                _MissingAckTimeout = value;
            }
        }

        /// <summary>   No Incoming Msg Timeout in ms. </summary>
        private uint _NoInMsgTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Maximum amount of time after receiving an I-Type data APDU before sending an S-Type 
        ///             confirm APDU (t2) </summary>
        ///
        /// <value> No Incoming Msg Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint NoInMsgTimeout
        {
            get
            {
                return _NoInMsgTimeout;
            }
            set
            {
                _NoInMsgTimeout = value;
            }
        }

        /// <summary>   No Activity Timeout in ms. </summary>
        private uint _NoActivityTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Timeout for sending test frames in case of a long idle state (t3) </summary>
        ///
        /// <value> No Activity Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint NoActivityTimeout
        {
            get
            {
                return _NoActivityTimeout;
            }
            set
            {
                _NoActivityTimeout = value;
            }
        }

        /// <summary>   Connection periodic reset. </summary>
        private bool _EnablePeriodicReset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enable periodically stop and restart of the connection </summary>
        ///
        /// <value> Connection periodic reset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnablePeriodicReset
        {
            get
            {
                return _EnablePeriodicReset;
            }
            set
            {
                _EnablePeriodicReset = value;
            }
        }

        /// <summary>   Folder for File Transfer. </summary>
        private string _FileTransferDirectory;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Folder for File Transfer </summary>
        ///
        /// <value> Name of the folder for File Transfer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileTransferDirectory
        {
            get
            {
                return _FileTransferDirectory;
            }
            set
            {
                _FileTransferDirectory = value;
            }
        }

        private LinkStates _LinkState;
        public LinkStates LinkState
        {
            get { return _LinkState; }
            set { _LinkState = value; }
        }

        private bool _StartDTReceived;
        public bool StartDTReceived
        {
            get { return _StartDTReceived; }
            set { _StartDTReceived = value; }
        }

        private bool _TimeSyncronized;
        public bool TimeSyncronized
        {
            get { return _TimeSyncronized; }
            set { _TimeSyncronized = value; }
        }

        private bool _GeneralInterrConfReceived;
        public bool GeneralInterrConfReceived
        {
            get { return _GeneralInterrConfReceived; }
            set { _GeneralInterrConfReceived = value; }
        }

        private bool _GeneralInterrEndReceived;
        public bool GeneralInterrEndReceived
        {
            get { return _GeneralInterrEndReceived; }
            set { _GeneralInterrEndReceived = value; }
        }
        #endregion
    }
}
