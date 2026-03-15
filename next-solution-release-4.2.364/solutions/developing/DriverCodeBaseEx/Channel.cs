////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Channel.cs
//
// summary:	Implements the channel class
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using System.Threading.Tasks;
using Opc.Ua;
using DevExpress.Xpo.DB;
namespace DriverCodeBaseEx
{
    /// <summary>   communication channel of the base communication driver. </summary>
    public abstract class Channel : IChannelBase, IStatistics, IDisposable
    {
        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the Channel object. </summary>
        ///
        /// <param name="commdriver">                       The communications driver. </param>
        /// <param name="settings" type="ChannelSettings">  Options for controlling the operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected Channel(CommunicationDriver commdriver, ChannelSettings settings) 
            : this()
        {
            CommDriver = commdriver;
            _Name = settings.Name;
            _WaitTime = settings.WaitTime;
            _Timeout = settings.Timeout;
            _PollingTimeNotInUse = settings.PollingTimeNotInUse;
            _PollingTimeInError = settings.PollingTimeInError;
            _KeepOpened = settings.KeepOpened;
            _CurrentErrorCode = 0;
            _StateCommandTag = settings.StateCommandTag;
            if (StateCommandVariableTag.IsTagsSet(_StateCommandTag))
            {
                StateCommandVariableDuplicated = settings.StateCommandVariableAlreadyUsed();
                channelStateCommandVariable = new StateCommandVariable(_StateCommandTag.Name, _StateCommandTag.NodeId.ToString());
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            }
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected Channel()
        {
            // default settings
            _PollingTimeNotInUse = 0;
            _PollingTimeInError = 5000;
            _KeepOpened = false;
            _CurrentErrorCode = 0;
            _ConsecutiveCommErrors = 0;
            channelStateCommandVariable = new StateCommandVariable();
        }
        protected Channel(CommunicationDriver commdriver)
        {
            CommDriver = commdriver;
            // default settings
            _Name = String.Empty;
            _WaitTime = 0;
            _Timeout = 10000;
            _PollingTimeNotInUse = 10000;
            _PollingTimeInError = 10000;
            _KeepOpened = false;
            _CurrentErrorCode = 0;
            _ConsecutiveCommErrors = 0;
            channelStateCommandVariable = new StateCommandVariable();
        }

        static Channel()
        {
            lockScheduler = new AutoResetEvent(false);
        }
        #endregion
        #region Abstracts Methods
        //public abstract bool Init();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool IsDeviceOpen();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool DeviceOpen();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool DeviceClose();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool DeviceRead(byte[] Buffer, uint Count);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool DeviceWrite(byte[] Buffer, uint Count);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract uint GetBytesToRead();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract uint GetBytesToWrite();
        #endregion
        #region Data Members
        protected static AutoResetEvent lockScheduler;
        protected static long CurrentCommunicationThreadsRunning;
        protected static long MaxCommunicationThreadsAllowed = Math.Max(1, Properties.Settings.Default.MaxRunningJobs);

        List<CommJob> channelJobList = new List<CommJob>();
        /// <summary>
        /// Communication procedure timer
        /// </summary>
        protected Timer communicationProcedureTimer;
        protected bool bCommunicationProcedure = false;
        protected bool bRepromote;
        int lastDelay;
        protected int repromoteDelay = -1;
        DateTime timeDelay;
        /// <summary>   The lock thread object. </summary>
        protected readonly object lockThreadObject = new object();
        
        /// <summary>   The lock statisic. </summary>
        protected readonly object lockStatisic = new object();
        /// <summary>   The Schedule thread. </summary>
        Thread SchedulerThread;
        protected ManualResetEvent ScheduleEvent;
        bool ExitScheduling;
        protected bool bChannelStarted = false;
        protected ManualResetEvent CommunicationProcedureFinished = null;
        /// <summary>   The communications driver. </summary>
        protected readonly CommunicationDriver CommDriver;
        /// <summary>   The last error time. </summary>
        protected DateTime LastErrorTime;
        /// <summary>   Message describing the last error. </summary>
        protected String LastErrorMessage;
        /// <summary>   The last error code. </summary>
        protected DriverErrorCodes LastErrorCode = DriverErrorCodes.ErrorNoError;
        /// <summary>   The diagn last task receive in bytes. </summary>
        protected long DiagnLastTaskRxBytes;
        /// <summary>   The diagn last task transmit in bytes. </summary>
        protected long DiagnLastTaskTxBytes;
        /// <summary>   The diagn last task time. </summary>
        protected double DiagnLastTaskTime = 0;
        /// <summary>   The total partial jobs. </summary>
        protected long TotalPartialJobs = 0;
        /// <summary>   The total partial tags. </summary>
        protected long TotalPartialTags = 0;
        /// <summary>   The total partial in bytes. </summary>
        protected long TotalPartialBytes = 0;
        /// <summary>   The total partial tasks. </summary>
        protected long TotalPartialTasks = 0;
        protected DateTime DiagnTaskTime = DateTime.MinValue;
        
        /// <summary>   memory buffer of received data. </summary>
        protected List<byte> ReceiveBuffer = new List<byte>();
        /// <summary>  The State/Command variable associated to the channel. </summary>
        protected StateCommandVariable channelStateCommandVariable;
        bool StateCommandVariableDuplicated = false;
        #endregion
        #region Private Fields/Methods
        /// <summary>   The new data to anlyze. </summary>
        protected ManualResetEvent NewDataToAnlyze;
        /// <summary>   Queue of scheduled jobs. </summary>
        readonly Dictionary<CommJobState, List<CommJob>> ScheduledJobQueue = new Dictionary<CommJobState, List<CommJob>>();
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets scheduled job queue. </summary>
        ///
        /// <param name="type" type="CommJobState"> The type. </param>
        ///
        /// <returns>   The scheduled job queue. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<CommJob> GetScheduledJobQueue(CommJobState type)
        {
            lock(lockThreadObject)
            {
                if (!ScheduledJobQueue.Keys.Contains(type))
                    ScheduledJobQueue[type] = new List<CommJob>();
                return ScheduledJobQueue[type];
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check if job is into PollingInError list
        /// 
        /// <param name="job"> the job be checked</param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsJobInPollingInErrorState(CommJob job)
        {
            return job.InErrorState;
        }
        
        
        #region Properties
        /// <summary>   The consecutive communications errors. </summary>
        private int _ConsecutiveCommErrors;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the consecutive communications errors. </summary>
        ///
        /// <value> The consecutive communications errors. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ConsecutiveCommErrors
        {
            get { return _ConsecutiveCommErrors; }
            set
            {                
                if(_ConsecutiveCommErrors != value) 
                { 
                    _ConsecutiveCommErrors = value;
                    if (value == 0)
                        CommDriver.OnStateChanged(ComunicationState.Running);
                    else
                        CommDriver.OnStateChanged(ComunicationState.Fault);
                }
            }
        }
        
        /// <summary>   The current error code. </summary>
        private int _CurrentErrorCode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the current error code. </summary>
        ///
        /// <value> The current error code. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CurrentErrorCode
        {
            get { return _CurrentErrorCode; }
            set
            {
                _CurrentErrorCode = value;
            }
        }
        
        /// <summary>   The name. </summary>
        private readonly string _Name;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Name
        {
            get { return _Name; }
        }
        /// <summary>   The wait time. </summary>
        private readonly int _WaitTime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the wait time. </summary>
        ///
        /// <value> The wait time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int WaitTime
        {
            get { return _WaitTime; }
        }
        /// <summary>   The timeout. </summary>
        private readonly int _Timeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the timeout. </summary>
        ///
        /// <value> The timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Timeout
        {
            get { return _Timeout; }
        }
        /// <summary>   The polling time not in use. </summary>
        private readonly int _PollingTimeNotInUse;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the polling time not in use. </summary>
        ///
        /// <value> The polling time not in use. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int PollingTimeNotInUse
        {
            get { return _PollingTimeNotInUse; }
        }
        /// <summary>   The polling time in error. </summary>
        private readonly int _PollingTimeInError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the polling time in error. </summary>
        ///
        /// <value> The polling time in error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int PollingTimeInError
        {
            get { return _PollingTimeInError; }
        }
        /// <summary>   The keep opened. </summary>
        private readonly bool _KeepOpened = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the keep opened. </summary>
        ///
        /// <value> true if keep opened, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool KeepOpened
        {
            get { return _KeepOpened; }
        }
        /// <summary>  The State-Command variable of the channel. </summary>
        private UFUAModel.TagEntityReference _StateCommandTag;
        /// <summary>
        /// Gets or sets the the State-Command variable of the channel.
        /// </summary>
        public UFUAModel.TagEntityReference StateCommandTag
        {
            get
            {
                return _StateCommandTag;
            }
            set
            {
                _StateCommandTag = value;
            }
        }
        #endregion
        #region runtime properties
        protected bool bChannelCloseBySuspend = false;
        public bool ChannelCloseBySuspend
        {
            get { return bChannelCloseBySuspend; }
            set { bChannelCloseBySuspend = value; }
        }
        /// <summary>   The polling now jobs counter. </summary>
        int _PollingNowJobsCounter;
        protected int PollingNowJobsCounter
        {
            get { return _PollingNowJobsCounter; }
            set { _PollingNowJobsCounter = value; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling now jobs. </summary>
        ///
        /// <value> true if polling now jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool PollingNowJobs
        {
            get
            {
                if (++_PollingNowJobsCounter <= Properties.Settings.Default.MaxConsecutivePollingHiPriJobs)
                    return true;
                _PollingNowJobsCounter = 0;
                return false;
            }
        }
        
        /// <summary>   The polling in use jobs counter. </summary>
        int _PollingInUseJobsCounter;
        protected int PollingInUseJobsCounter
        {
            get { return _PollingInUseJobsCounter; }
            set { _PollingInUseJobsCounter = value; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling in use jobs. </summary>
        ///
        /// <value> true if polling in use jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool PollingInUseJobs
        {
            get
            {
                if (++_PollingInUseJobsCounter <= Properties.Settings.Default.MaxConsecutivePollingInUseJobs)
                    return true;
                _PollingInUseJobsCounter = 0;
                return false;
            }
        }
        /// <summary>   The polling not in use jobs counter. </summary>
        int _PollingNotInUseJobsCounter;
        protected int PollingNotInUseJobsCounter
        {
            get { return _PollingNotInUseJobsCounter; }
            set { _PollingNotInUseJobsCounter = value; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling not in use jobs. </summary>
        ///
        /// <value> true if polling not in use jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool PollingNotInUseJobs
        {
            get
            {
                if (++_PollingNotInUseJobsCounter <= Properties.Settings.Default.MaxConsecutivePollingNotInUseJobs)
                    return true;
                _PollingNotInUseJobsCounter = 0;
                return false;
            }
        }
        #endregion
        #region methods
        public void SystemEvent(object nodeId, string message, EventSeverity severity)
        {
            CommDriver.OnSystemEvent(nodeId, message, severity);
        }
        #endregion
        #region Communication Procedures
        protected virtual bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            if (tagNodeId != null && value != null)
            {
                var ret = exjob.OnWriteTag(tagNodeId, ref value, true);
                if (ret != StatusCodes.Good)
                {
                    exjob.SynchroError = (int)ret;
                    return false;
                }
            }
            return true;
        }
        long nJobsExecutionSuspended;
        protected void SuspendJobsExecution()
        {
            Interlocked.Increment(ref nJobsExecutionSuspended);
            StopTimers(CommDriver.bTerminating);
        }
        protected void RestartJobsExecution()
        {
            if (Interlocked.Decrement(ref nJobsExecutionSuspended) == 0 && (!bDisposed && !bSuspended))
                StartTimers();
        }
        internal uint NrActiveStations()
        {
            return (uint)CommDriver.GetChannelStations(this).Count(s => !s.IsStateCommandVariableBit((UInt16)StationVariableBits.StationActiveCommand, true) && GetNrSubscribeJob(s) != 0);
        }
        public virtual void commExecuteSyncro(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            try
            {
               SuspendJobsExecution();
                if (SetSynchroJobData(exjob, tagNodeId, value))
                {
                    exjob.UpdateTagsListOnWriting();
                    exjob.SyncroExec = true;
                    exjob.IsPending = true;
                    commExecution(exjob, this);
                }
            }
            finally
            {
                exjob.SyncroExec = false;
                RestartJobsExecution();
            }
        }

        public virtual void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            ManualResetEvent newDataToAnlyze = null;
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze == null)
                    NewDataToAnlyze = new ManualResetEvent(false);
                newDataToAnlyze = NewDataToAnlyze;
            }
            if (newDataToAnlyze != null)
            {
                //wait for answer
                if (!newDataToAnlyze.WaitOne(Timeout))
                {
                    //Timeout
                    lock (lockThreadObject)
                    {
                        //LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                        conn = DriverErrorCodes.ErrorTimeOut;
                        ReceiveBuffer.Clear();
                    }
                }
                else
                {
                    newDataToAnlyze.Reset();
                }
            }
        }

        public virtual bool ResetConnection()
        {
            return false;
        }
        public virtual bool ExecuteJob(ref DriverErrorCodes conn, CommJob exjob)
        {            
            ExecuteJob(exjob);
            return true;
        }
        public virtual DriverErrorCodes CheckDevice(CommJob exjob, object thischannel)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
            }
            return conn;
        }
        
        public void commExecution(CommJob exjob, object thischannel)
        {
            if (exjob.IsCustomJob())
            {
                // struct atomic job contain a separate jobs list to manage read/write
                GetCustomChildJobs(exjob, out List<CommJob> childJobs, out bool write);
                foreach (var subAtomicJob in childJobs)
                {
                    bool inErrorState = false;
                    while (!inErrorState && !bDisposed && !CommDriver.bTerminating && subAtomicJob.IsPending)
                    {
                        DriverErrorCodes conn = CheckDevice(subAtomicJob, thischannel);

                        ResetNewDataEvent();

                        if (ExecuteJob(ref conn, subAtomicJob))
                            WaitNewDataEvent(ref conn);

                        if (!bDisposed && !CommDriver.bTerminating && subAtomicJob.IsPending)
                            ProcessNewData(conn, subAtomicJob);

                        if (ResetConnection())
                            DeviceClose();

                        inErrorState = subAtomicJob.InErrorState;
                    }
                    if (inErrorState)
                        break;
                }
                // merge result of separate jobs list into "main" job
                MergeCustomJobChildJobs(exjob, childJobs, write);
            }
            else
            {
                while (!bDisposed && !CommDriver.bTerminating && exjob.IsPending)
                {
                    DriverErrorCodes conn = CheckDevice(exjob, thischannel);
                    ResetNewDataEvent();
                    if (ExecuteJob(ref conn, exjob))
                        WaitNewDataEvent(ref conn);
                    if (!bDisposed && !CommDriver.bTerminating && exjob.IsPending)
                        ProcessNewData(conn, exjob);
                    if (ResetConnection())
                        DeviceClose();
                }
            }
        }

        /// <summary>
        /// Return the list of jobs (instead of use exJob job) to manage read/write operation
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="resultJobs"></param>
        /// <param name="write"></param>
        protected virtual void GetCustomChildJobs(CommJob exjob, out List<CommJob> resultJobs, out bool write)
        {
            resultJobs = new List<CommJob>();

            // check if is a read or write operation
            write = (exjob.GetTagListOnWritingCount() > 0 || exjob.Type == LinkType.UnconditionalOutput);
            if (write)
            {
                if (exjob.Type == LinkType.UnconditionalOutput)
                    exjob.FillWholeTagsListOnWriting();

                foreach (var tag in exjob.TagsListOnWriting)
                {
                    if (exjob.ChildJobsWrite.ContainsKey(tag.TagNode.NodeId))
                    {
                        // copy each tag to write from exJob (main) to sub jobs (ChlildJobsWrite)
                        CommJob childJob = exjob.ChildJobsWrite[tag.TagNode.NodeId];
                        childJob.TagsListOnWriting.Clear();
                        childJob.TagsListToWrite.Clear();
                        childJob.TagsList[0].Value.Value = Utils.Clone(tag.Value.Value);
                        childJob.AddTagListOnWriting(childJob.TagsList[0]);
                        if (childJob.IsCustomJob())
                        {
                            GetCustomChildJobs(childJob, out List<CommJob> customChildJobs, out bool childWrite);
                            if (childWrite)
                                resultJobs.AddRange(customChildJobs);
                        }
                        else
                        {
                            resultJobs.Add(childJob);
                        }
                    }
                }
            }
            else
            {
                foreach (var childJob in exjob.ChildJobsRead.Values)
                {
                    if (childJob.IsCustomJob())
                    {
                        GetCustomChildJobs(childJob, out List<CommJob> resultChildJobs, out bool childWrite);
                        if (!childWrite)
                            resultJobs.AddRange(resultChildJobs);
                    }
                    else
                    {
                        resultJobs.Add(childJob);
                    }
                }
            }

            // reset each jobs before to be executed
            Parallel.ForEach(resultJobs, j =>
            {
                j.InitChildJobBeforeExecution();
            });
        }

        /// <summary>
        /// Merge the result of "Struct Atomic sub jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="childJobs"></param>
        /// <param name="write"></param>
        protected virtual void MergeCustomJobChildJobs(CommJob exjob, List<CommJob> childJobs, bool write)
        {
            ExecutedJobArgs e = new ExecutedJobArgs();
            e.Job = exjob;
            e.ErrorCode = DriverErrorCodes.ErrorNoError;

            foreach (CommJob j in childJobs)
            {
                if (!j.IsPending && j.ChildJobErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    e.ErrorCode = j.ChildJobErrorCode;
                    break;
                }
            }

            if (exjob.IsStructAtomic())
            {                
                if (write)
                {
                    if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        foreach (Tag tag in exjob.TagsList)
                            tag.SetIniternalValue(tag.Value.Value);
                    }
                }
                else
                {
                    // all read jobs are ok
                    if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        foreach (CommJob j in childJobs)
                        {
                            Tag tag = exjob.TagsList.Find(t => t.TagNode.NodeId == j.TagsList[0].TagNode.NodeId);
                            if (tag != null)
                            {
                                if (tag.GetReadValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(j.TagsList[0], j.TagsList[0].GetReadValue(), tag.GetReadValue()))
                                    tag.SetReadValueChildJob(j.TagsList[0].GetReadValue());
                            }
                        }
                    }
                }
            }

            exjob.RWState = CommJob.RWStates.Standard;
            OnJobExecuted(e);
        }

        private void communicationProcedure(object state)
        {
            Channel me = (Channel)state;
            bool bDecrementCounter = false;
            try
            {
                CommJob nextjob = null;
                lock (lockThreadObject)
                {
                    bCommunicationProcedure = true;
                    if (communicationProcedureTimer != null)
                    {
                        bDecrementCounter = true;
                        communicationProcedureTimer.Dispose();
                        communicationProcedureTimer = null;
                    }
                    if (bDisposed)
                        return;
                    nextjob = GetNextPendingJob();
                }
                
                if (nextjob != null)
                {
                    commExecution(nextjob, me);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally 
            {
                lock (lockThreadObject)
                {
                    bCommunicationProcedure = false;
                
                    if (!KeepOpened && IsDeviceOpen())
                        DeviceClose();
                    if (!bDisposed && !bStopping)
                    {
                        if (ScheduleEvent != null && NeedNewSchedule())
                            ScheduleEvent.Set();
                        bool bRestart = RemainingJobInQueues();
                        if (bRestart || bRepromote)
                        {
                            //choose the correct delay for the new timer instance!!!!!
                            int nNextRun = 0;
                            if (repromoteDelay != -1 && !bRestart)
                                nNextRun = repromoteDelay;

                            if (InGeneralErrorState)
                                nNextRun = (Timeout > nNextRun ? Timeout : nNextRun);

                            bRepromote = false;
                            StartTimerExecution(communicationProcedure, me, Math.Max(nNextRun, WaitTime));
                            repromoteDelay = -1;
                        }
                    }
                    if (CommunicationProcedureFinished != null)
                        CommunicationProcedureFinished.Set();
                    if (bDecrementCounter)
                    {
                        if (Interlocked.Decrement(ref CurrentCommunicationThreadsRunning) <= MaxCommunicationThreadsAllowed)
                            lockScheduler.Set();
                    }
                }
            }
        }
        protected int nInUseMax = 0;
        protected int nInErrorMax = 0;
        protected int nNotInUseMax = 0;
        protected int nNowMax = 0;
        protected virtual bool NeedNewSchedule()
        {
            lock (lockThreadObject)
            {
                return (ScheduledJobQueue[CommJobState.PollingInUse].Count <= (nInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledJobQueue[CommJobState.PollingInError].Count <= (nInErrorMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledJobQueue[CommJobState.PollingNotInUse].Count <= (nNotInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledJobQueue[CommJobState.PollingNow].Count <= (nNowMax * Properties.Settings.Default.ScheduleRemainingPercent));
            }
        }
        protected virtual bool RemainingJobInQueues()
        {
            lock (lockThreadObject)
            {
                return (ScheduledJobQueue[CommJobState.PollingInUse].Count > 0 ||
                ScheduledJobQueue[CommJobState.PollingInError].Count > 0 ||
                ScheduledJobQueue[CommJobState.PollingNotInUse].Count > 0 ||
                ScheduledJobQueue[CommJobState.PollingNow].Count > 0);
            }
        }
        #endregion
        #region Virtual methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialises this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Init()
        {
            if (StateCommandVariableDuplicated)
            {
                channelStateCommandVariable.hasBeenSet = false;
                SystemEvent(null, string.Format(Properties.Resources.StateCommandTagNameAlreadyInUse, _StateCommandTag.StringRepresentation), EventSeverity.Medium);
            }
            return true;
        }
        
        public virtual void InitData()
        {
            lock (lockThreadObject)
            {
                if (!ScheduledJobQueue.Keys.Contains(CommJobState.PollingInError))
                    ScheduledJobQueue[CommJobState.PollingInError] = new List<CommJob>();
                if (!ScheduledJobQueue.Keys.Contains(CommJobState.PollingInUse))
                    ScheduledJobQueue[CommJobState.PollingInUse] = new List<CommJob>();
                if (!ScheduledJobQueue.Keys.Contains(CommJobState.PollingNotInUse))
                    ScheduledJobQueue[CommJobState.PollingNotInUse] = new List<CommJob>();
                if (!ScheduledJobQueue.Keys.Contains(CommJobState.PollingNow))
                    ScheduledJobQueue[CommJobState.PollingNow] = new List<CommJob>();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Startup()
        {
            if (bDisposed)
                return false;
            if (bChannelStarted)
                return true;
            lock (lockThreadObject)
            {
                InitData();
                if (ScheduleEvent == null)
                    ScheduleEvent = new ManualResetEvent(false);
                else
                    ScheduleEvent.Reset();
                if(SchedulerThread == null)
                {
                    SchedulerThread = new Thread(SchedulingThread);
                    SchedulerThread.IsBackground = true;
                    SchedulerThread.Priority = ThreadPriority.BelowNormal;
                    SchedulerThread.Name = "Scheduler_" + Name;
                }
                //if (NewDataToAnlyze == null)
                //    NewDataToAnlyze = new ManualResetEvent(false);
                //else
                //    NewDataToAnlyze.Reset();
                if (!SchedulerThread.IsAlive)
                    SchedulerThread.Start(0);
                bChannelStarted = SchedulerThread != null && (SchedulerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
                return true;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests channel communications. </summary>
        ///
        /// <returns>   true if the test passes, false if the test fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool TestChannelComm()
        {
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Call with point-to-point protocol. </summary>
        ///
        /// <exception cref="NotImplementedException">  Thrown when the requested operation is
        ///                                             unimplemented. </exception>
        ///
        /// <param name="conn">                         Communication error code. </param>
        /// <param name="pendingjob" type="CommJob">    The pendingjob. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the job operation. </summary>
        ///
        /// <param name="conn"> communication error code. </param>
        /// <param name="job">  job to subscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ExecuteJob(CommJob job)
        {
            job.BasicExecute();
        }
        public virtual void SetNewDataEvent()
        {
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze != null)
                    NewDataToAnlyze.Set();
            }
        }
        public virtual void ResetNewDataEvent()
        {
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze != null)
                    NewDataToAnlyze.Reset();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the pending job described by job. </summary>
        ///
        /// <param name="job">  job to subscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void RemovePendingJob(CommJob job)
        {
            lock (lockThreadObject)
                job.IsPending = false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the list of node ids to be observed for this channel. </summary>
        ///
        /// <returns>   Gets the list of node ids to be observed for this channel. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<NodeId> GetObservingNodes()
        {
            List<NodeId> nodeIDs = new List<NodeId>();
            // Add to the list the Node ID of the State\Command Variable of the channel
            if (channelStateCommandVariable.hasBeenSet == true)
            {
                nodeIDs.Add(channelStateCommandVariable.varNodeId);
            }
            return nodeIDs;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the value of an observed node id. </summary>
        ///
        /// <returns>   Updates the value of an observed node id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateObservedTag(NodeId node, DataValue value)
        {
                // Update the value of the state/command variable
            if (node == channelStateCommandVariable.varNodeId)
            {
                if(channelStateCommandVariable.varDataType == BuiltInType.Null)
                    channelStateCommandVariable.varDataType = value.WrappedValue.TypeInfo.BuiltInType;
                channelStateCommandVariable.SetValue(value);
            }
        }
        public virtual void ForceExecution()
        {
            StartTimerExecution(communicationProcedure, this, 0);
        }
        protected virtual void StartTimers()
        {
            StartTimerExecution(communicationProcedure, this, WaitTime);
        }
        public virtual void StartTimerExecution(TimerCallback clbk, object state, int dueTime)
        {
            lock (lockThreadObject)
            {
                if (communicationProcedureTimer != null)
                {
                    if (dueTime >= lastDelay || (DateTime.UtcNow - timeDelay).TotalMilliseconds < dueTime && dueTime != lastDelay)
                        return;
                    communicationProcedureTimer.Dispose();
                    communicationProcedureTimer = null;
                    if (Interlocked.Decrement(ref CurrentCommunicationThreadsRunning) <= MaxCommunicationThreadsAllowed)
                        lockScheduler.Set();
                }
                // nJobsExecutionSuspended used to manage SyncroJob and Driver's timer for cyclic "connection" test
                if (bDisposed || bSuspended || Interlocked.Read(ref nJobsExecutionSuspended) > 0)
                    return;
                if (bCommunicationProcedure)
                {
                    if (repromoteDelay != -1)
                        repromoteDelay = Math.Min(repromoteDelay, dueTime);
                    else
                        repromoteDelay = dueTime;
                    bRepromote = true;
                    return;
                }
                lastDelay = dueTime;
                timeDelay = DateTime.UtcNow;
                Interlocked.Increment(ref CurrentCommunicationThreadsRunning);
                communicationProcedureTimer = new Timer(clbk, state, dueTime, System.Threading.Timeout.Infinite);
            }
        }
        #endregion
        protected bool bStopping;
        protected virtual void StopTimers(bool bTerminate = true)
        {
            try
            {
                AutoResetEvent waitHandle = null;
                lock (lockThreadObject)
                {
                    bStopping = true;
                    if (bTerminate && NewDataToAnlyze != null)
                        NewDataToAnlyze.Set();
                    if (communicationProcedureTimer != null || bCommunicationProcedure)
                    {
                        if (CommunicationProcedureFinished == null)
                            CommunicationProcedureFinished = new ManualResetEvent(false);
                        else
                            CommunicationProcedureFinished.Reset();
                    }
                        
                    if (communicationProcedureTimer != null)
                    {
                        waitHandle = new AutoResetEvent(false);
                        communicationProcedureTimer.Dispose(waitHandle);
                        communicationProcedureTimer = null;
                        if (Interlocked.Decrement(ref CurrentCommunicationThreadsRunning) <= MaxCommunicationThreadsAllowed)
                            lockScheduler.Set();
                    }
                }                
                if (waitHandle != null)
                {
                    waitHandle.WaitOne();
                    waitHandle.Dispose();
                }
                if (CommunicationProcedureFinished != null && bCommunicationProcedure)
                {
                    CommunicationProcedureFinished.WaitOne(); //Timeout
                    //CommunicationProcedureFinished.Dispose();
                    //CommunicationProcedureFinished = null;
                }
            }
            finally
            {
                bStopping = false;
            }
        }
        #region Communication related Methods                   
        public List<CommJob> GetJobStateList(CommJobState state)
        {
            var jList = new List<CommJob>();
            if(state == CommJobState.PollingInError)
            {
                lock (lockThreadObject)
                    jList = (from job in channelJobList.AsParallel() where job.InErrorState select job).ToList();
            }
            else if(state == CommJobState.PollingInUse)
            {
                lock (lockThreadObject)
                    jList = (from job in channelJobList.AsParallel() where job.InUse == true && job.InErrorState == false && job.GetTagListToWriteCount() == 0 select job).ToList();
            }
            else if(state == CommJobState.PollingNotInUse)
            {
                lock (lockThreadObject)
                    jList = (from job in channelJobList.AsParallel() where job.InUse == false && job.InErrorState == false && job.GetTagListToWriteCount() == 0 select job).ToList();
            }
            else if(state == CommJobState.PollingNow)
            {
                lock (lockThreadObject)
                    jList = (from job in channelJobList.AsParallel() where job.InErrorState == false && job.GetTagListToWriteCount() > 0 select job).ToList();
            }
            
            return jList;
        }
        public void RemoveStationJobs(Station station)
        {
            try
            {
                SuspendJobsExecution();
                lock (lockThreadObject)
                {
                    RemoveStationJobsOnQueue(station);
                    channelJobList.RemoveAll(j => j.Station == station);
                }
            }
            finally
            {
                RestartJobsExecution();
            }
        }
        public void RemoveStationJobsOnQueue(Station station)
        {
            lock (lockThreadObject)
            {
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingNow);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingInUse);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingNotInUse);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingInError);
            }
        }
        protected virtual void RemoveJobOfStationOnSpecicQueue(Station station, CommJobState type)
        {
            var inputQueue = GetScheduledJobQueue(type);
            var lToRemove = (from job in inputQueue.AsParallel() where job.Station == station select job).ToList();
            foreach (var jr in lToRemove)
            {
                jr.IsQueued = false;
                inputQueue.Remove(jr);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected CommJob GetNextPendingJob()
        {
            lock (lockThreadObject)
            {
                var queue = GetNextPendingQueue();
                if (queue != null && queue.Count > 0)
                {
                    for (int i = 0; i < queue.Count; i++)
                    {
                        CommJob job = queue[i];
                        if (job.IsPending == false)
                        {
                            job.IsQueued = false;
                            job.IsPending = true;
                            queue.Remove(job);
                            return (job);
                        }
                    }
                }
            }
            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending queue. </summary>
        ///
        /// <returns>   The next pending queue. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected List<CommJob> GetNextPendingQueue()
        {
            if (GetScheduledJobQueue(CommJobState.PollingNow).Count > 0 && _PollingInUseJobsCounter == 0 && _PollingNotInUseJobsCounter == 0)
            {
                if (PollingNowJobs ||
                    (GetScheduledJobQueue(CommJobState.PollingInUse).Count == 0 &&
                    GetScheduledJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                    GetScheduledJobQueue(CommJobState.PollingInError).Count == 0))
                {
                    return GetScheduledJobQueue(CommJobState.PollingNow);
                }
            }
            if (GetScheduledJobQueue(CommJobState.PollingInUse).Count > 0 && _PollingNotInUseJobsCounter == 0)
            {
                if (PollingInUseJobs ||
                    (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                    GetScheduledJobQueue(CommJobState.PollingInError).Count == 0)
                    && (GetScheduledJobQueue(CommJobState.PollingNow).Count == 0))
                {
                    return GetScheduledJobQueue(CommJobState.PollingInUse);
                }
            }
            if (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count > 0)
            {
                if (PollingNotInUseJobs ||
                    GetScheduledJobQueue(CommJobState.PollingInError).Count == 0)
                {
                    return GetScheduledJobQueue(CommJobState.PollingNotInUse);
                }
            }
            _PollingNowJobsCounter = 0;
            _PollingInUseJobsCounter = 0;
            _PollingNotInUseJobsCounter = 0;
            if (GetScheduledJobQueue(CommJobState.PollingInError).Count > 0)
            {
                return GetScheduledJobQueue(CommJobState.PollingInError);
            }
            //nothing to do, as the regular sequence
            if (GetScheduledJobQueue(CommJobState.PollingNow).Count > 0)
            {
                _PollingNowJobsCounter++;
                return GetScheduledJobQueue(CommJobState.PollingNow);
            }
            if (GetScheduledJobQueue(CommJobState.PollingInUse).Count > 0)
            {
                _PollingInUseJobsCounter++;
                return GetScheduledJobQueue(CommJobState.PollingInUse);
            }
            if (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count > 0)
            {
                _PollingNotInUseJobsCounter++;
                return GetScheduledJobQueue(CommJobState.PollingNotInUse);
            }
            if (GetScheduledJobQueue(CommJobState.PollingInError).Count > 0)
            {
                return GetScheduledJobQueue(CommJobState.PollingInError);
            }
            return null;
        }
        /// <summary>
        /// Represent the method that will signal  new data arrival from the proper communication channel
        /// </summary>
        public event EventHandler<NewDataReceivedArgs> NewDataReceived;
        public virtual void UpdateReceiveBuffer(object sender, byte[] rec, DateTime dt, out bool setNewDataEvent, out int beginDeviceReadSize)
        {
            setNewDataEvent = true;
            beginDeviceReadSize = 0;
            EventHandler<NewDataReceivedArgs> temp = NewDataReceived;
            if (temp != null)
            {
                NewDataReceivedArgs e = new NewDataReceivedArgs() { Sender = sender, RxBytes = rec, Timestamp = dt };
                temp(this, e);
                if (!e.SetNewDataEvent)
                    setNewDataEvent = false;
            } 
            else
            {
                lock (lockThreadObject)
                {
                    ReceiveBuffer.AddRange(rec);
                }
            }
        }     
        #endregion
        #region IStatistics Interface
        /// <summary>   Information describing the statistics. </summary>
        public StatisticCounters StatisticsData;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to know if the statistics are available for this object. </summary>
        ///
        /// <returns>   true if statistics are available; otherwise, false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsStatisticsAvailable()
        {
            return StatisticsData != null;
        }
        /// <summary>   Initialize statistics. </summary>
        public void StartStatistics()
        {
            if (StatisticsData == null)
            {
                StatisticsData = new StatisticCounters();
                StatisticsData.ChangedCounter += StatisticsData_ChangedCounter;
                StatisticsData.StartWatch();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by StatisticsData for changed counter events. </summary>
        ///
        /// <param name="sender" type="object">             Source of the event. </param>
        /// <param name="e" type="ChangedCounterEventArgs"> Changed counter event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StatisticsData_ChangedCounter(object sender, ChangedCounterEventArgs e)
        {
                CommDriver.OnTagChanged(new NodeId(string.Format(
                    "{0}?{1}Statistics/Channels/{2}/{3}", CommDriver.RootDriversGuid, CommDriver.DriverName, Name, e.Key),
                    CommDriver.NamespaceIndex), new DataValue(e.newValue));
        }
        /// <summary>   Terminate statistics and free the counters. </summary>
        public void TerminateStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.Dispose();
           
            StatisticsData = null;
        }
        /// <summary>   Suspend statistics watcher. </summary>
        public void SuspendStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StopWatch();
        }
        /// <summary>   Resum statistics watcher. </summary>
        public void ResumeStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StartWatch();
        }
        /// <summary>   Reset whole statistic counters to zero. </summary>
        public void ResetStatistcs()
        {
            if (StatisticsData != null)
                StatisticsData.ResetStatistcs();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to retrieve a copy of the current statistics dictionary. </summary>
        ///
        /// <param name="totaltimeon" type="out TimeSpan">  [out] out value with the elapsed total time
        ///                                                 with statistics enabled. </param>
        ///
        /// <returns>   An IDictionary. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IDictionary RetrieveStatisticCounters(out TimeSpan totaltimeon)
        {
            if (StatisticsData != null)
                return StatisticsData.RetrieveStatistcs(out totaltimeon);
            totaltimeon = new TimeSpan();
            return new Dictionary<string, long>();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the number of statistic values recorded. </summary>
        ///
        /// <returns>   The total counters. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetTotalCounters()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalCounters();
            return 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return a list with the name of each statistic counter. </summary>
        ///
        /// <returns>   The list of counters name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IList GetListOfCountersName()
        {
            if (StatisticsData != null)
                return StatisticsData.GetListOfCountersName();
            return new List<String>();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the value of a counter. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   The counter value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long GetCounterValue(string key)
        {
            if (StatisticsData != null)
                return StatisticsData.GetCounterValue(key);
            return 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the elapsed total time with statistics enabled. </summary>
        ///
        /// <returns>   The total time on. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TimeSpan GetTotalTimeOn()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalTimeOn();
            return new TimeSpan();
        }
        public void IncrementLastRxByte(int count)
        {
            if (StatisticsData != null)
                lock (lockStatisic)
                    DiagnLastTaskRxBytes += count;
        }
        public void IncrementLastTxByte(int count)
        {
            if (StatisticsData != null)
                lock (lockStatisic)
                    DiagnLastTaskTxBytes += count;
        }
        /// <summary>   The statistic nodes. </summary>
        public static readonly StatisticSetting.NodeDataNames[] StatisticNodes = new StatisticSetting.NodeDataNames[]
        {
            StatisticSetting.NodeDataNames.TotalJobsRead,
            StatisticSetting.NodeDataNames.TotalJobsWrite,
            StatisticSetting.NodeDataNames.JobRate,
            StatisticSetting.NodeDataNames.TimeForCommunicationTask_mS,
            StatisticSetting.NodeDataNames.CommunicationTaskRate,
            StatisticSetting.NodeDataNames.TotalTasks,
            StatisticSetting.NodeDataNames.TagRate,
            StatisticSetting.NodeDataNames.ByteRate,
            StatisticSetting.NodeDataNames.TotalTagsRead,
            StatisticSetting.NodeDataNames.TotalTagsWrite,
            StatisticSetting.NodeDataNames.TotalBytesRead,
            StatisticSetting.NodeDataNames.TotalBytesWrite,
            StatisticSetting.NodeDataNames.TotalRxBytes,
            StatisticSetting.NodeDataNames.TotalTxBytes,
            StatisticSetting.NodeDataNames.TotalJobsError,
            StatisticSetting.NodeDataNames.LastErrorTime,
            StatisticSetting.NodeDataNames.LastError,
            StatisticSetting.NodeDataNames.InErrorState,
        };
        #endregion
        #region IChannelBase Interface
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Subscribe a new job inside channel. </summary>
        ///
        /// <param name="job">      job to subscribe. </param>
        /// <param name="state">    initial state of the job (in use, not in use, etc.) </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void SubscribeJob(CommJob job, CommJobState state)
        {
            bSuspended = false;
            Startup();
            lock (lockThreadObject)
            {
                channelJobList.Add(job);
                if(ScheduleEvent != null)
                    ScheduleEvent.Set();
            }
        }
        
        protected void SchedulingThread(object data)
        {
            int schedTimeout = System.Threading.Timeout.Infinite;
            while (!ExitScheduling)
            {
                if (!ExitScheduling)
                    ScheduleEvent.WaitOne(schedTimeout);
                if (ExitScheduling)
                    break;
                
                List<CommJob> startList = new List<CommJob>();
                
                lock (lockThreadObject)
                {
                    ScheduleEvent.Reset();
                    if (channelJobList.Count > 0)
                        startList.AddRange(channelJobList);
                }
                
                schedTimeout = System.Threading.Timeout.Infinite;
                if (startList.Count > 0)
                    schedTimeout = ChannelScheduleProcedure(startList);
                if (RemainingJobInQueues())
                {
                    if (Interlocked.Read(ref CurrentCommunicationThreadsRunning) > MaxCommunicationThreadsAllowed)
                        lockScheduler.WaitOne();

                    RestartCommunicationTimer(WaitTime);
                }
                    
            }

            lock (lockThreadObject)
            {
                if (ScheduleEvent != null)
                {
                    ScheduleEvent.Dispose();
                    ScheduleEvent = null;
                }
            }
        }
        protected virtual void RestartCommunicationTimer(int dueTime)
        {
            StartTimerExecution(communicationProcedure, this, dueTime);
        }

        protected virtual int ChannelScheduleProcedure(List<CommJob> startList)
        {
            int nNextScheduleInterval = -1;
            int nNextDelayUse = -1;
            int nNextDelayError = -1;
            int nNextDelayNotUse = -1;

            DateTime dtNow = DateTime.UtcNow;
                        
            lock (lockThreadObject)
            {                
                List<CommJob> wList = null;
                wList = (from job in startList
                         where  !job.IsPending && job.InUse && !job.SyncroExec 
                                && job.IsConditionalVariableOn() 
                                && (job.Type == LinkType.UnconditionalOutput || job.GetTagListToWriteCount() > 0)
                         orderby job.LastExecutionTime ascending
                         select job).ToList();
                
                if (wList != null && wList.Count > 0)
                {
                    bool anyQueued = (wList.Count(j => j.IsQueued) > 0);
                    if (anyQueued)
                    {
                        if (ScheduledJobQueue[CommJobState.PollingNow].Count > 0)
                            wList.RemoveAll((o) => ScheduledJobQueue[CommJobState.PollingNow].Contains(o));

                        foreach (var job in wList)
                        {
                            if (job.IsQueued)
                            {
                                bool removed = false;
                                foreach (var jobState in new List<CommJobState> { CommJobState.PollingInUse, CommJobState.PollingInError, CommJobState.PollingNotInUse })
                                {
                                    if (ScheduledJobQueue[jobState].Count > 0)
                                    {
                                        for (int jobIndex = 0; jobIndex < ScheduledJobQueue[jobState].Count; jobIndex++)
                                        {
                                            if (ScheduledJobQueue[jobState][jobIndex] == job)
                                            {
                                                removed = true;
                                                ScheduledJobQueue[jobState].RemoveAt(jobIndex);
                                                break;
                                            }
                                        }
                                    }
                                    if (removed)
                                        break;
                                }
                            }
                        }
                    }
                    
                    if (wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        ScheduledJobQueue[CommJobState.PollingNow].AddRange(wList);
                        nNowMax = ScheduledJobQueue[CommJobState.PollingNow].Count;
                    }
                }

                wList = (from job in startList
                            where !job.IsPending && !job.IsQueued && job.InUse && !job.InErrorState
                                && job.IsConditionalVariableOn()
                                && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                                && (((dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval) || job.ExecuteFirstTime == true)
                            orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                            select job).ToList();
                if (wList != null && wList.Count > 0)
                {
                    wList.ForEach(job =>
                    {
                        job.IsQueued = true;
                        job.UpdateTagsListOnWriting();
                    });
                    ScheduledJobQueue[CommJobState.PollingInUse].AddRange(wList);
                    nInUseMax = ScheduledJobQueue[CommJobState.PollingInUse].Count;
                }

                Dictionary<Station, List<CommJob>> stationsInError = (from job in startList
                                                                        where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                                                                && job.IsConditionalVariableOn()
                                                                                && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                                                        //&& (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime
                                                                        orderby job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime) ascending
                                                                        group job by job.Station into jobsInError
                                                                        select new { Station = jobsInError.Key, Values = jobsInError.ToList() }).ToDictionary(t => t.Station, t => t.Values);
                if (stationsInError != null)
                {
                    wList = new List<CommJob>();
                    foreach (var stInErr in stationsInError)
                    {
                        Station st = stInErr.Key;
                        List<CommJob> jobs = stInErr.Value;
                        // get jobs to schedule now
                        List<CommJob> jobsToScheduleNow = jobs.FindAll(job => (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime);
                        if (jobsToScheduleNow == null)
                            jobsToScheduleNow = new List<CommJob>();
                        // station in error and no jobs to execute now ?
                        if (st.InGeneralErrorState && jobsToScheduleNow.Count == 0)
                        {
                            // if first of list (all jobs is error) exceed PollingTimeInError force execution
                            if ((dtNow - st.StationLastErrorTime).TotalMilliseconds >= PollingTimeInError)
                                wList.Add(jobs[0]);
                        }
                        else
                        {
                            wList.AddRange(jobsToScheduleNow);
                        }
                    }
                    if (wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        ScheduledJobQueue[CommJobState.PollingInError].AddRange(wList);
                        nInErrorMax = ScheduledJobQueue[CommJobState.PollingInError].Count;
                    }
                }

                if (PollingTimeNotInUse > 0)
                {
                    wList = (from job in startList
                                where !job.IsPending && !job.InUse && !job.IsQueued && !job.InErrorState
                                    && job.IsConditionalVariableOn()
                                    && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                    && (((dtNow - job.LastExecutionTime).TotalMilliseconds >= PollingTimeNotInUse) || job.ExecuteFirstTime == true)
                                orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                                select job).ToList();
                    if (wList != null && wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        ScheduledJobQueue[CommJobState.PollingNotInUse].AddRange(wList);
                        nNotInUseMax = ScheduledJobQueue[CommJobState.PollingNotInUse].Count;
                    }
                }


                List<CommJob> rList;
                rList = (from job in startList
                            where !job.IsPending && !job.IsQueued && job.InUse && !job.InErrorState
                                && job.IsConditionalVariableOn()
                                && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)                                    
                                && (((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval && job.SamplingInterval != CommJob.JOB_NOT_SCHEDULABLE) || job.ExecuteFirstTime == true)
                            orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                            select job).ToList();
                if (rList.Count > 0)
                {
                    var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(rList[0].SamplingInterval);
                    if (dtNextSchedule > dtNow)
                        nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    else
                        return 0;
                }

                if (PollingTimeInError > 0)
                {
                    rList = (from job in startList
                                where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                        && job.IsConditionalVariableOn()
                                        && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                        && (dtNow - job.LastErrorTime).TotalMilliseconds <= job.ErrorPollingTime
                                orderby job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime) ascending
                                select job).ToList();
                    if (rList.Count > 0)
                    {
                        var dtNextSchedule = rList[0].LastErrorTime.AddMilliseconds(rList[0].ErrorPollingTime);
                        if (dtNextSchedule > dtNow)
                        {
                            nNextDelayError = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                            // if channel (all stations) is in general error and something were scheduled, wait to reschedule that all jobs in error are processed
                            if (InGeneralErrorState && ScheduledJobQueue[CommJobState.PollingInError].Count > 0)
                                nNextDelayError = -1;
                            // if channel or station is in general error, force to restart not far than PollingTimeInError
                            else if (nNextDelayError > PollingTimeInError && (rList[0].Station.InGeneralErrorState || InGeneralErrorState))
                                nNextDelayError = PollingTimeInError;
                        }
                        else
                            return 0;
                    }
                }

                if (PollingTimeNotInUse > 0)
                {
                    rList = (from job in startList
                                where !job.IsPending && !job.IsQueued && !job.InUse && !job.InErrorState
                                    && job.IsConditionalVariableOn()
                                    && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                    && (((dtNow - job.LastExecutionTime).TotalMilliseconds < PollingTimeNotInUse) || job.ExecuteFirstTime == true)
                                orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                                select job).ToList();
                    if (rList.Count > 0)
                    {
                        var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(PollingTimeNotInUse);
                        if (dtNextSchedule > dtNow)
                            nNextDelayNotUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                        else
                            return 0;
                    }
                }                               
                
                //search lower delays
                if (nNextDelayUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayUse < nNextScheduleInterval))
                {
                    nNextScheduleInterval = nNextDelayUse;
                }
                if (nNextDelayError >= 0 && (nNextScheduleInterval == -1 || nNextDelayError < nNextScheduleInterval))
                {
                    nNextScheduleInterval = nNextDelayError;
                }
                if (nNextDelayNotUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayNotUse < nNextScheduleInterval))
                {
                    nNextScheduleInterval = nNextDelayNotUse;
                }
                
                return nNextScheduleInterval;
            }
        }

        /// <summary>
        /// Verify if a job was scheduled to write
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        protected virtual bool IsJobScheduledToWrite(CommJob job)
        {
            bool scheduled = false;

            lock (lockThreadObject)
            {
                if (job.IsQueued)
                {                    
                    if (ScheduledJobQueue[CommJobState.PollingNow].Count > 0)
                    {
                        foreach (CommJob scheduledJob in ScheduledJobQueue[CommJobState.PollingNow])
                        {
                            if (scheduledJob == job)
                            {
                                scheduled = true;
                                break;
                            }
                        }                            
                    }
                }
            }

            return scheduled;
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Unsubscribe the job from channel. </summary>
        ///
        /// <param name="job">  job to unsubscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UnsubscribeJob(CommJob job)
        {
            lock (lockThreadObject)
            {
                channelJobList.Remove(job);
            }
        }
        public int GetNrSubscribeJob(Station station)
        {
            lock (lockThreadObject)
            {
                return channelJobList.Count(s=> s.Station == station);
            }
        }
        protected bool bSuspended;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Suspend the working thread. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Suspend()
        {
            bSuspended = true;
            // wait until communicationProcedureTimer and commExecution terminate
            StopTimers(false);
            lock (lockThreadObject) 
                channelJobList.Clear();
            
            DeviceClose();
            bChannelCloseBySuspend = true;
            bChannelStarted = false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Notify at the channel the new job state value. </summary>
        ///
        /// <param name="job">      job who has changed its state value. </param>
        /// <param name="state">    the new job state (in use, non in use, etc.) </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ChangeStateJob(CommJob job, CommJobState state)
        {
            bool bFound = false;
            lock (lockThreadObject)
            {
                bFound = channelJobList.Contains(job);
                
            }
            if (bFound && !job.SyncroExec)
            {
                Startup();
                lock(lockThreadObject)
                {                    
                    if (ScheduleEvent != null)
                    {
                        bool set = NeedNewSchedule();
                        // before to force scheduler execution for a write request, verify that is not yet scheduled
                        if (!set && state == CommJobState.PollingNow)
                            set = !IsJobScheduledToWrite(job);
                        
                        if (set)                        
                            ScheduleEvent.Set();
                    }
                }
            }
            return bFound;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of subscribed job in the channel. </summary>
        ///
        /// <value> The subscribed jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long SubscribedJobs
        {
            get
            {
                lock (lockThreadObject)
                {
                    return channelJobList.Count;
                }
            }
        }
        private long _InUseJobs = 0;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the in use jobs. </summary>
        ///
        /// <value> The in use jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long InUseJobs
        {
            get { return Interlocked.Read(ref _InUseJobs); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Represents the method that that will handle the job executed event (after the execution)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public event EventHandler<ExecutedJobArgs> JobExecuted;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   create event for process ExecutedJobArgs. </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnJobExecuted(ExecutedJobArgs e)
        {
            
            if (StatisticsData != null)
            {
                lock (lockStatisic)
                {
                    e.Job.BasicCalculateStatistic();
                    if (DiagnTaskTime != e.Job.StartExecutionTime)
                    {
                        DiagnLastTaskTime += (DateTime.UtcNow - e.Job.StartExecutionTime).TotalMilliseconds;
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTasks.ToString());
                        TotalPartialTasks++;
                    }
                    DiagnTaskTime = e.Job.StartExecutionTime;
                    e.RxBytes = DiagnLastTaskRxBytes;
                    DiagnLastTaskRxBytes = 0;
                    e.TxBytes = DiagnLastTaskTxBytes;
                    DiagnLastTaskTxBytes = 0;
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalRxBytes.ToString(), e.RxBytes);
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTxBytes.ToString(), e.TxBytes);
                    if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                    {
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                        uint quality;
                        String DiagnErrorMessage = "";
                        CommDriver.GetDriverErrorInfo((int)e.ErrorCode, out quality, out DiagnErrorMessage);
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
                    }
                    else
                    {
                        if (e.Job.IsRead)
                        {
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsRead.ToString());
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsRead.ToString(), e.Job.ExchangedTag);
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesRead.ToString(), e.Job.ExchangedByte);
                        }
                        else
                        {
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsWrite.ToString());
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsWrite.ToString(), e.Job.ExchangedTag);
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesWrite.ToString(), e.Job.ExchangedByte);
                        }
                        TotalPartialJobs++;
                        TotalPartialTags += e.Job.ExchangedTag;
                        TotalPartialBytes += e.Job.ExchangedByte;
                    }
                }
            }
            if (e.Job.SyncroExec == true)
            {
                if (e.Job.LocalMethod == false)
                {
                    e.Job.SynchroValues = e.Values;
                    e.Job.SynchroError = (int)e.ErrorCode;
                }
            }
            EventHandler<ExecutedJobArgs> temp = JobExecuted;
            if (temp != null)
                temp(this, e);
            lock (lockThreadObject)
            {
                e.Job.LastExecutionTime = e.LastExecutionTime; // DateTime.UtcNow;
                if (!e.Job.SetRWState(e.ErrorCode))//Have to write, keep job active
                    e.Job.IsPending = false;
            }
        }
        /// <summary>   Executes the refresh diagnostic action. </summary>
        public void OnRefreshDiagnostic()
        {
            if (StatisticsData != null)
            {
                long lTotalPartialJobs;
                long lTotalPartialTags;
                long lTotalPartialBytes;
                long lTotalPartialTasks;
                string CommunicationTaskTime = "uncertain";
                double dDiagnLastTaskTime;
                bool InErrorState = false;
                string sLastStatisticErrorTime = string.Empty;
                string sLastStatisticError = string.Empty;
                lock (lockStatisic)
                {
                    lTotalPartialJobs = TotalPartialJobs;
                    lTotalPartialTags = TotalPartialTags;
                    lTotalPartialBytes = TotalPartialBytes;
                    lTotalPartialTasks = TotalPartialTasks;
                    dDiagnLastTaskTime = DiagnLastTaskTime;
                    if (TotalPartialTasks > 0)
                        CommunicationTaskTime = (dDiagnLastTaskTime / lTotalPartialTasks).ToString("F1");
                    TotalPartialJobs = 0;
                    TotalPartialTags = 0;
                    TotalPartialBytes = 0;
                    TotalPartialTasks = 0;
                    DiagnLastTaskTime = 0;
                    foreach (var station in CommDriver.GetChannelStations(this))
                    {
                        if (station.InErrorState)
                        {
                            sLastStatisticErrorTime = station.LastStatisticErrorTime;
                            sLastStatisticError = station.LastStatisticError;
                            InErrorState = true;
                            break;
                        }
                    }
                }
                StatisticsData.Update(StatisticSetting.NodeDataNames.JobRate.ToString(), lTotalPartialJobs, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.TagRate.ToString(), lTotalPartialTags, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.ByteRate.ToString(), lTotalPartialBytes, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.CommunicationTaskRate.ToString(), lTotalPartialTasks, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.TimeForCommunicationTask_mS.ToString(), CommunicationTaskTime, send: true);
                if (InErrorState)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), sLastStatisticErrorTime, send: true);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), sLastStatisticError, send: true);
                }
                StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), InErrorState, send: true);
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTasks.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTagsRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTagsWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalBytesRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalBytesWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalRxBytes.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTxBytes.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
            }
        }
        public void UnsubscribeJobs(List<CommJob> jobs)
        {
            lock (lockThreadObject)
            {
                foreach (CommJob job in jobs)
                {
                    if (channelJobList.Contains(job))
                        channelJobList.Remove(job);
                }
            }
        }
        protected virtual void RemoveScheduledJobs(Station s, List<CommJob> jobs)
        {
            lock (lockThreadObject)
            {
                foreach (var state in ScheduledJobQueue.Keys)
                {
                    ScheduledJobQueue[state].RemoveAll(j => j.Station == s && jobs.Contains(j));
                }
                Parallel.ForEach(jobs, job =>
                {
                    job.IsQueued = false;
                });
            }
        }
        public void InUseJobsIncrement()
        {
            Interlocked.Increment(ref _InUseJobs);
        }
        public void InUseJobsDecrement()
        {
            Interlocked.Decrement(ref _InUseJobs);
        }
        #endregion
        #region StateCommandVariable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetStateCommandVariableBit(bool bitValue, UInt16 bitIndex)
        {          
            return channelStateCommandVariable.SetStateCommandVariableBit(bitValue, bitIndex, CommDriver);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            return channelStateCommandVariable.GetStateCommandVariableBit(ref bitValue, bitIndex);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Check all stations general error state and set result in InGeneralErrorState</summary>
        ///        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CheckInGeneralErrorState()
        {
            _InGeneralErrorState = CommDriver.GetChannelStations(this).Count(s=> s.InGeneralErrorState) == CommDriver.GetChannelStations(this).Count();
        }

        private bool _InGeneralErrorState = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether all station is in general error state. </summary>
        ///
        /// <value> true if in general error state, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InGeneralErrorState
        {
            get { return _InGeneralErrorState; }
            set { _InGeneralErrorState = value; }
        }


        #endregion
        #region IDisposable Interface
        protected bool bDisposed;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            // Driver is closing. 
            StopTimers();
            // FOGBUGZ 11643: moved after the stop of WorkerThread 
            //if (StatisticsData != null)
            //{
            //    StatisticsData.Dispose();
            //    StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
            //    StatisticsData = null;
            //}
            // FOGBUGZ 11643: moved after the stop of WorkerThread 
            if (StatisticsData != null)
            {
                StatisticsData.Dispose();
                StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
                StatisticsData = null;
            }
            
            ExitScheduling = true;
            //Thread scheduleThread = SchedulerThread;
            lock (lockThreadObject)
            {
                if (ScheduleEvent != null)
                    ScheduleEvent.Set();
            }
            //if (scheduleThread != null)
            //    scheduleThread.Join();
            lock (lockThreadObject)
            {
                //if (ScheduleEvent != null)
                //    ScheduleEvent.Dispose();
                //ScheduleEvent = null;

                if (NewDataToAnlyze != null)
                {
                    NewDataToAnlyze.Set();
                    NewDataToAnlyze.Dispose();
                    NewDataToAnlyze = null;
                }
                channelJobList.Clear();
            }
        }
        #endregion
    }
}
