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
using System.Diagnostics;

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
            _StateTag = settings.StateTag;
            _CommandTag = settings.CommandTag;
            if (StateCommandVariable.IsTagsSet(_StateCommandTag, _StateTag, _CommandTag))
            {
                StateCommandVariableDuplicated = settings.StateCommandVariableAlreadyUsed();
                channelStateCommandVariable = new StateCommandVariable(StateCommandVariable.ObjectTypes.Channel, _StateCommandTag, _StateTag, _CommandTag);
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

        protected enum EventID : int
        {
            None = 0,
            NewJobExecution = 1,            
            CyclicCheckExecution = 2,
            ThreadTerminate = 3
        }

        //protected const int SLEEP_CYCLE_NOWAIT = 0;
        protected const int SLEEP_CYCLE_INFINITE = -1;

        protected List<CommJob> channelJobList = new List<CommJob>();
        protected Dictionary<CommJob, bool> channelJobMap = new Dictionary<CommJob, bool>();
        /// <summary> lock used to manage input/output with device </summary>
        protected readonly object lockThreadObject = new object();        
        /// <summary> lock used to protect internal list (scheduled jobs list included) </summary>
        protected readonly object lockListObject = new object();
        /// <summary> lock used protect job/task execution
        protected readonly object lockSuspendJobs = new object();
        /// <summary>   The lock statisic. </summary>
        protected readonly object lockStatisic = new object();
        /// <summary>   The Schedule thread. </summary>
        protected bool bChannelStarted = false;
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

        Thread CommunicationThread;
        protected ManualResetEvent CommunicationEvent = null;        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets scheduled job queue. </summary>
        ///
        /// <param name="type" type="CommJobState"> The type. </param>
        ///
        /// <returns>   The scheduled job queue. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<CommJob> GetScheduledJobQueue(CommJobState type)
        {
            lock(lockListObject)
            {
                if (!ScheduledJobQueue.Keys.Contains(type))
                    ScheduledJobQueue[type] = new List<CommJob>();
                return ScheduledJobQueue[type];
            }
        }

        List<CommJob> GetAndRemove1StScheduledJobQueue(CommJobState type)
        {
            List<CommJob> scheduledJobQueue = new List<CommJob>();
            lock (lockListObject)
            {
                if (!ScheduledJobQueue.Keys.Contains(type))
                    ScheduledJobQueue[type] = new List<CommJob>();

                if (ScheduledJobQueue[type].Count > 0)
                {
                    scheduledJobQueue.Add(ScheduledJobQueue[type][0]);
                    ScheduledJobQueue[type].RemoveAt(0);
                }

                foreach (CommJob job in scheduledJobQueue)
                {
                    job.ScheduleQueue = CommJobState.UnScheduled;
                    job.IsPending = true;
                }
            }

            return scheduledJobQueue;
        }

        void AddScheduledListJobQueue(CommJobState type, List<CommJob> list, out int scheduled)
        {
            lock (lockListObject)
            {
                foreach (var job in list)
                    job.ScheduleQueue = type;

                if (!ScheduledJobQueue.Keys.Contains(type))
                    ScheduledJobQueue[type] = new List<CommJob>();
                ScheduledJobQueue[type].AddRange(list);

                scheduled = ScheduledJobQueue[type].Count;
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

        private UFUAModel.TagEntityReference _StateTag;
        /// <summary>
        /// Gets or sets the the State variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference StateTag
        {
            get
            {
                return _StateTag;
            }
            set
            {
                _StateTag = value;
            }
        }

        private UFUAModel.TagEntityReference _CommandTag;
        /// <summary>
        /// Gets or sets the the Command variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference CommandTag
        {
            get
            {
                return _CommandTag;
            }
            set
            {
                _CommandTag = value;
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

        protected bool AcquireCommunicationThreadControlBlocking()
        {
            Monitor.Enter(lockSuspendJobs);

            return true;
        }

        protected bool AcquireCommunicationThreadControl(int timeout = 0)
        {
            if (RequestSupendOrTerminateCommunicationThread() || CommDriver.bDisposed)
                return false;            

            if (timeout == 0)
            {
                Monitor.Enter(lockSuspendJobs);
            }
            else
            {
                if (!Monitor.TryEnter(lockSuspendJobs, timeout))
                    return false;
            }

            if (RequestSupendOrTerminateCommunicationThread() || CommDriver.bDisposed)
            {
                ReleaseCommunicationThreadControl();

                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool ReleaseCommunicationThreadControl()
        {
            Monitor.Exit(lockSuspendJobs);

            return ((RequestSupendOrTerminateCommunicationThread() || CommDriver.bDisposed) ? false : true);
        }

        protected bool ReleaseCommunicationThreadControlBlocking()
        {
            return ReleaseCommunicationThreadControl();
        }

        internal uint NrActiveStations()
        {
            return (uint)CommDriver.GetChannelStations(this).Count(s => !s.IsStateCommandVariableBit((UInt16)StationVariableBits.StationActiveCommand, true) && GetNrSubscribeJob(s) != 0);
        }

        public virtual void commExecuteSyncro(CommJob exjob, NodeId tagNodeId = null, object value = null, int timeOut = 0)
        {
            if (AcquireCommunicationThreadControl())
            {
                // Nuovo
                CommDriver.SuspendScheduler();
                RemoveJobFromSchedulingQueue(exjob, (CommJob.ScheduleProperties.SetSyncroExec | CommJob.ScheduleProperties.SetIsPending));
                CommDriver.RestartScheduler();
                if (SetSynchroJobData(exjob, tagNodeId, value))
                {
                    WaitWaitTimeFromLastCommExecution();

                    exjob.UpdateTagsListOnWriting();                        
                    commExecution(exjob, this);

                    // reevaluate the status of the scheduling queue and restart if necessary
                    if (NeedNewSchedule(out int nrRemainingJobsInQueues))
                    {
                        //PerformanceLogToFile(this.Name, string.Format("communicationProcedure SetRequestSchedule : {0}", nrRemainingJobsInQueues));
                        SetRequestSchedule();
                    }
                }
                else
                {
                    exjob.SetScheduleProperties((CommJob.ScheduleProperties.UnSetSyncroExec | CommJob.ScheduleProperties.UnSetIsPending));
                }
                ReleaseCommunicationThreadControl();                
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

        private void internalCommExecution(CommJob exjob, object thischannel)
        {
            // struct atomic job contain a separate jobs list to manage read/write
            GetCustomChildJobs(exjob, out List<CommJob> childJobs, out bool write);
            foreach (var childJob in childJobs)
            {
                bool inErrorState = false;
                while (!inErrorState && !bDisposed && !CommDriver.bDisposed && childJob.IsPending)
                {
                    DriverErrorCodes conn = CheckDevice(childJob, thischannel);

                    ResetNewDataEvent();

                    if (ExecuteJob(ref conn, childJob))
                        WaitNewDataEvent(ref conn);

                    if (!bDisposed && !CommDriver.bDisposed && childJob.IsPending)
                        ProcessNewData(conn, childJob);

                    if (ResetConnection())
                        DeviceClose();

                    inErrorState = childJob.InErrorState;
                }
                if (inErrorState)
                    break;
            }
            if (!bDisposed && !CommDriver.bDisposed)
                // merge result of separate jobs list into "main" job
                MergeCustomJobChildJobs(exjob, childJobs, write);

            SetLastCommExecutionDateTime();
        }

        public void commExecution(CommJob exjob, object thischannel)
        {
            internalCommExecution(exjob, thischannel);

            if (!bDisposed && !CommDriver.bDisposed)
            {
                // some jobs are continuously doing writes but should they also read ?
                if (exjob.InputOutputRequiresReadAfterContinuosWrite())
                {
                    WaitWaitTimeFromLastCommExecution();

                    CommDriver.SuspendScheduler();
                    //if it has already been scheduled in the meantime, remove it from the queue
                    RemoveJobFromSchedulingQueue(exjob, CommJob.ScheduleProperties.SetIsPending);                    
                    // move the tags to be written so that the driver can be read
                    exjob.UpdateTagsListOnWritingAndTagsListToWrite(exjob.GetTagListOnWriting());
                    CommDriver.RestartScheduler();                                        

                    internalCommExecution(exjob, this);
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
            bool resultWrite = false;

            if (!exjob.IsCustomJob())
            {
                resultJobs.Add(exjob);
            }
            else
            {
                // check if is a read or write operation
                resultWrite = (exjob.GetTagListOnWritingCount() > 0 || exjob.Type == LinkType.UnconditionalOutput);
                if (resultWrite)
                {
                    if (exjob.Type == LinkType.UnconditionalOutput)
                        exjob.FillWholeTagsListOnWriting();

                    foreach (var tag in exjob.TagsListOnWriting)
                    {
                        if (exjob.ChildJobsWrite.ContainsKey(tag.TagNode.NodeId))
                        {
                            // copy each tag to write from exJob (main) to sub jobs (ChlildJobsWrite)
                            CommJob childJob = exjob.ChildJobsWrite[tag.TagNode.NodeId];
                            childJob.ClearTagListWrite();
                            childJob.TagsList[0].SetWriteVal(tag.Value.Value);
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
                            GetCustomChildJobs(childJob, out List<CommJob> resultChildJobs, out bool childRead);
                            if (!childRead)
                                resultJobs.AddRange(resultChildJobs);
                        }
                        else
                        {
                            resultJobs.Add(childJob);
                        }
                    }
                }

                // reset each jobs before to be executed
                foreach (CommJob j in resultJobs)
                {
                    if (j.IsChildJob)
                        j.ResetInitialValueChildJob();
                }
            }            

            write = resultWrite;            
        }

        /// <summary>
        /// Merge the result of "Struct Atomic sub jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="childJobs"></param>
        /// <param name="write"></param>
        protected virtual void MergeCustomJobChildJobs(CommJob exjob, List<CommJob> childJobs, bool write)
        {           
            if (!exjob.IsCustomJob())
            {
                // do nothing here ! a standard job (not childJob) was processed before
            }
            else
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
                            foreach (Tag tag in exjob.GetTagListOnWriting())
                                tag.SetValue(tag.WriteVal);
                        }
                    }
                    else
                    {
                        // all read jobs are ok
                        if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                        {
                            foreach (Tag tag in exjob.TagsList)
                            {
                                CommJob j = childJobs.Find(job => job.TagsList[0].TagNode.NodeId == tag.TagNode.NodeId);
                                Tag cTag = (j == null ? null : j.TagsList[0]);
                                if (cTag != null)
                                {
                                    if (tag.GetValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(tag, tag.GetValue(), cTag.GetValue()))
                                        tag.SetValueFromChild(cTag.GetValue());
                                }
                            }
                        }
                    }
                }

                exjob.RWState = CommJob.RWStates.Standard;
                OnJobExecuted(e);
            }
        }

        protected virtual bool ExecuteTask(object state)
        {
            CommJob nextjob = GetNextPendingJob();

            if (nextjob != null)
            {
                Channel me = (Channel)state;
                commExecution(nextjob, me);

                //Channel.PerformanceLogToFile("ExecuteTask ,channel=" + this.Name + ", Execute Comm");

                return true;
            }
            else
            {
                //Channel.PerformanceLogToFile("ExecuteTask ,channel=" + this.Name + ", Nothing to Execute Comm");

                return false;
            }
        }

        private int communicationProcedure(object state)
        {
            int sleepCycle = SLEEP_CYCLE_INFINITE;
            bool taskExecuted = false;

            //System.Diagnostics.Debug.WriteLine("communicationProcedure Channel communicationProcedure=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Enter");

            try
            {
                taskExecuted = ExecuteTask(state);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {                
                if (!bDisposed && !CommDriver.bDisposed)
                {
                    if (taskExecuted)
                    {
                        if (!KeepOpened && IsDeviceOpen())
                            DeviceClose();

                        // reevaluate the status of the scheduling queue and restart if necessary
                        if (NeedNewSchedule(out int nrRemainingJobsInQueues))
                        {
                            //PerformanceLogToFile(this.Name, string.Format("communicationProcedure SetRequestSchedule : {0}", nrRemainingJobsInQueues));
                            SetRequestSchedule();
                            sleepCycle = WaitTime;
                        }
                        else
                        {
                            sleepCycle = (nrRemainingJobsInQueues == 0 ? SLEEP_CYCLE_INFINITE : WaitTime);
                        }

                        // something to do ?
                        if (sleepCycle != SLEEP_CYCLE_INFINITE)
                        {
                            if (InGeneralErrorState)
                                sleepCycle = (Timeout > sleepCycle ? Timeout : sleepCycle);
                        }
                    }
                }

                //System.Diagnostics.Debug.WriteLine("communicationProcedure Channel communicationProcedure=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Job processed=" + (taskExecuted ? "Yes" : "No"));
            }

            return sleepCycle;
        }

        protected int nInUseMax = 0;
        protected int nInErrorMax = 0;
        protected int nNotInUseMax = 0;
        protected int nNowMax = 0;

        protected virtual bool NeedNewSchedule()
        {
            return NeedNewSchedule(out int dummy);
        }

        protected virtual bool NeedNewSchedule(out int nrRemainingJobsInQueues)
        {
            lock (lockListObject)
            {
                nrRemainingJobsInQueues = NrRemainingJobsInQueues();
                if (ScheduledJobQueue.Count > 0)
                {
                    return (ScheduledJobQueue[CommJobState.PollingInUse].Count <= (nInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledJobQueue[CommJobState.PollingInError].Count <= (nInErrorMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledJobQueue[CommJobState.PollingNotInUse].Count <= (nNotInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledJobQueue[CommJobState.PollingNow].Count <= (nNowMax * Properties.Settings.Default.ScheduleRemainingPercent));
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual int NrRemainingJobsInQueues()
        {
            lock (lockListObject)
            {
                if (ScheduledJobQueue.Count > 0)
                    return (ScheduledJobQueue[CommJobState.PollingInUse].Count + ScheduledJobQueue[CommJobState.PollingInError].Count + ScheduledJobQueue[CommJobState.PollingNotInUse].Count + ScheduledJobQueue[CommJobState.PollingNow].Count);
                else
                    return 0;
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
            lock (lockListObject)
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
                if (CommunicationEvent == null)
                    CommunicationEvent = new ManualResetEvent(false);
                else
                    CommunicationEvent.Reset();                

                if (CommunicationThread == null)
                {
                    CommunicationThread = new Thread(CommunicationToDeviceThread);
                    CommunicationThread.IsBackground = true;
                    CommunicationThread.Priority = ThreadPriority.BelowNormal;
                    CommunicationThread.Name = "CommunicationThread_" + Name;
                }

                if (!CommunicationThread.IsAlive)
                    CommunicationThread.Start(0);
                bChannelStarted = CommunicationThread != null && (CommunicationThread.ThreadState & (System.Threading.ThreadState.Stopped | System.Threading.ThreadState.Unstarted)) == 0;
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

        protected void StartCommunicationThread()
        {
            lock (lockThreadObject)
            {
                if (CommunicationEvent != null)
                    CommunicationEvent.Set();
            }
        }

        protected void ResetCommunicationThread()
        {
            lock (lockThreadObject)
            {
                if (CommunicationEvent != null)
                    CommunicationEvent.Reset();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the pending job described by job. </summary>
        ///
        /// <param name="job">  job to subscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void RemovePendingJob(CommJob job)
        {
            job.UpdateTagsListToWrite(CommJob.ScheduleProperties.UnSetIsPending);
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
                nodeIDs.AddRange(channelStateCommandVariable.GetNodesId());
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
            if (channelStateCommandVariable.ContainsNodeId(node))
            {
                channelStateCommandVariable.SetValueDataType(node, value.WrappedValue.TypeInfo.BuiltInType);
                channelStateCommandVariable.SetValue(node, value);
            }
        }
        public virtual void ForceExecution()
        {
            //PerformanceLogToFile(this.Name, "ForceExecution SetRequestSchedule");
            SetRequestSchedule();
        }
                
        #endregion
        #region Communication related Methods                   
        public List<CommJob> GetJobStateList(CommJobState state)
        {
            var jList = new List<CommJob>();
            if(state == CommJobState.PollingInError)
            {
                lock (lockListObject)
                    jList = (from job in channelJobList.AsParallel() where job.InErrorState select job).ToList();
            }
            else if(state == CommJobState.PollingInUse)
            {
                lock (lockListObject)
                    jList = (from job in channelJobList.AsParallel() where job.InUse == true && job.InErrorState == false && job.GetTagListToWriteCount() == 0 select job).ToList();
            }
            else if(state == CommJobState.PollingNotInUse)
            {
                lock (lockListObject)
                    jList = (from job in channelJobList.AsParallel() where job.InUse == false && job.InErrorState == false && job.GetTagListToWriteCount() == 0 select job).ToList();
            }
            else if(state == CommJobState.PollingNow)
            {
                lock (lockListObject)
                    jList = (from job in channelJobList.AsParallel() where job.InErrorState == false && job.GetTagListToWriteCount() > 0 select job).ToList();
            }
            
            return jList;
        }
        public void RemoveStationJobs(Station station)
        {
            if (AcquireCommunicationThreadControlBlocking())
            {
                CommDriver.SuspendScheduler();
                RemoveStationJobsOnQueue(station);
                lock (lockListObject)
                {
                    var stationJobs = channelJobList.FindAll(j => j.Station == station);
                    if (stationJobs != null && stationJobs.Count > 0)
                    {
                        channelJobList.RemoveAll(j => j.Station == station);
                        foreach (var job in stationJobs)
                        {
                            if (channelJobMap.ContainsKey(job))
                                channelJobMap.Remove(job);
                        }
                    }
                }
                CommDriver.RestartScheduler();
                ReleaseCommunicationThreadControlBlocking();
            }
        }

        public void RemoveStationJobsOnQueue(Station station)
        {
            lock (lockListObject)
            {
                RemoveJobOfStationOnSpecificQueue(station, CommJobState.PollingNow);
                RemoveJobOfStationOnSpecificQueue(station, CommJobState.PollingInUse);
                RemoveJobOfStationOnSpecificQueue(station, CommJobState.PollingNotInUse);
                RemoveJobOfStationOnSpecificQueue(station, CommJobState.PollingInError);
            }
        }
        protected virtual void RemoveJobOfStationOnSpecificQueue(Station station, CommJobState type)
        {
            lock (lockListObject)
            {
                var inputQueue = GetScheduledJobQueue(type);
            
                var lToRemove = (from job in inputQueue.AsParallel() where job.Station == station select job).ToList();
                foreach (var jr in lToRemove)
                {
                    jr.ScheduleQueue = CommJobState.UnScheduled;
                    inputQueue.Remove(jr);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected CommJob GetNextPendingJob()
        {
            var queue = GetNextPendingQueue();
            if (queue != null && queue.Count > 0)
                return queue[0];
            else
                return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending queue. </summary>
        ///
        /// <returns>   The next pending queue. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected List<CommJob> GetNextPendingQueue()
        {
            lock (lockListObject)
            {
                if (GetScheduledJobQueue(CommJobState.PollingNow).Count > 0 && _PollingInUseJobsCounter == 0 && _PollingNotInUseJobsCounter == 0)
                {
                    if (PollingNowJobs ||
                        (GetScheduledJobQueue(CommJobState.PollingInUse).Count == 0 &&
                        GetScheduledJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                        GetScheduledJobQueue(CommJobState.PollingInError).Count == 0))
                    {
                        return GetAndRemove1StScheduledJobQueue(CommJobState.PollingNow);
                    }
                }
                if (GetScheduledJobQueue(CommJobState.PollingInUse).Count > 0 && _PollingNotInUseJobsCounter == 0)
                {
                    if (PollingInUseJobs ||
                        (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                        GetScheduledJobQueue(CommJobState.PollingInError).Count == 0)
                        && (GetScheduledJobQueue(CommJobState.PollingNow).Count == 0))
                    {
                        return GetAndRemove1StScheduledJobQueue(CommJobState.PollingInUse);
                    }
                }
                if (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count > 0)
                {
                    if (PollingNotInUseJobs ||
                        GetScheduledJobQueue(CommJobState.PollingInError).Count == 0)
                    {
                        return GetAndRemove1StScheduledJobQueue(CommJobState.PollingNotInUse);
                    }
                }
                _PollingNowJobsCounter = 0;
                _PollingInUseJobsCounter = 0;
                _PollingNotInUseJobsCounter = 0;
                if (GetScheduledJobQueue(CommJobState.PollingInError).Count > 0)
                {
                    return GetAndRemove1StScheduledJobQueue(CommJobState.PollingInError);
                }
                //nothing to do, as the regular sequence
                if (GetScheduledJobQueue(CommJobState.PollingNow).Count > 0)
                {
                    _PollingNowJobsCounter++;
                    return GetAndRemove1StScheduledJobQueue(CommJobState.PollingNow);
                }
                if (GetScheduledJobQueue(CommJobState.PollingInUse).Count > 0)
                {
                    _PollingInUseJobsCounter++;
                    return GetAndRemove1StScheduledJobQueue(CommJobState.PollingInUse);
                }
                if (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count > 0)
                {
                    _PollingNotInUseJobsCounter++;
                    return GetAndRemove1StScheduledJobQueue(CommJobState.PollingNotInUse);
                }
                if (GetScheduledJobQueue(CommJobState.PollingInError).Count > 0)
                {
                    return GetAndRemove1StScheduledJobQueue(CommJobState.PollingInError);
                }
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
            //Startup();
            lock (lockListObject)
            {
                channelJobList.Add(job);
                channelJobMap[job] = false;
            }            
            //SetRequestSchedule();
        }

        protected EventID CheckPriorityEvents()
        {
            EventID eventID = EventID.None;
            if (RequestSupendOrTerminateCommunicationThread())
            {
                eventID = EventID.ThreadTerminate;
            }
            else
            {
                if (IsCyclicCheckSet())
                {
                    eventID = EventID.CyclicCheckExecution;
                    CyclicCheckReset();
                }
            }

            return eventID; 
        }

        protected EventID WaitNewEvent(int sleepCycle)
        {
            EventID eventID = CheckPriorityEvents();
            if (eventID == EventID.None)
            {
                ManualResetEvent communicationEvent = null;
                lock (lockThreadObject)
                    communicationEvent = CommunicationEvent;

                if (communicationEvent != null)
                {
                    Stopwatch startSleepCycle = Stopwatch.StartNew();
                    //wait for answer
                    communicationEvent.WaitOne(sleepCycle);
                    eventID = CheckPriorityEvents();
                    if (eventID == EventID.None)
                    {
                        if (sleepCycle > 0 && startSleepCycle.ElapsedMilliseconds < sleepCycle)
                        {
                            int remainTimeToSleep = (sleepCycle - (int)startSleepCycle.ElapsedMilliseconds);
                            if (remainTimeToSleep > 0)
                                System.Threading.Thread.Sleep(remainTimeToSleep);
                        }

                        eventID = EventID.NewJobExecution;

                        if (communicationEvent != null)
                            communicationEvent.Reset();
                    }
                }
            }            

            return eventID;
        }


        protected virtual bool RequestSupendOrTerminateCommunicationThread()
        {
            return (bDisposed || bSuspended);
        }

        protected virtual void CommunicationToDeviceThread(object data)
        {            
            int sleepCycle = SLEEP_CYCLE_INFINITE;

            //System.Diagnostics.Debug.WriteLine("CommunicationToDeviceThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Enter");

            while (true)
            {
                // wait some events/condition to proceed 
                EventID eventID = WaitNewEvent(sleepCycle);
                if (eventID == EventID.ThreadTerminate)
                    break;

                if (AcquireCommunicationThreadControl())
                {
                    // After return of SyncroJob, wait WaitTime if necessary
                    if (!CommDriver.WriteAsync)
                        WaitWaitTimeFromLastCommExecution();

                    //System.Diagnostics.Debug.WriteLine("CommunicationToDeviceThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Enter Comm");
                    //Channel.PerformanceLogToFile("CommunicationToDeviceThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Enter Comm");
                    switch (eventID)
                    {
                        case EventID.CyclicCheckExecution:
                            sleepCycle = CyclicCheckProcedure();
                            break;
                        case EventID.NewJobExecution:
                            // process ansyncro job (from queue)
                            sleepCycle = communicationProcedure(this);
                            break;
                    }
                    //System.Diagnostics.Debug.WriteLine("CommunicationToDeviceThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Exit Comm");
                    //Channel.PerformanceLogToFile("CommunicationToDeviceThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Exit Comm");

                    ReleaseCommunicationThreadControl();
                }
            }

            CyclicCheckTerminate();

            DeviceClose();

            //System.Diagnostics.Debug.WriteLine("DriverSchedulingThread Channel CommunicationToDeviceThread=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", Exit");
        }
        
        public virtual int ChannelScheduleProcedure(DateTime dtNow, out int nrRemainingJobsInQueues)
        {
            //Channel.PerformanceLogToFile(this.Name, string.Format("ChannelScheduleProcedure Start"));

            int nNextScheduleInterval = -1;

            int nNextDelayUse = -1;
            int nNextDelayError = -1;
            int nNextDelayNotUse = -1;
                        
            List<CommJob> startList = new List<CommJob>();
            List<CommJob> wListW = new List<CommJob>();
            List<CommJob> wListR = new List<CommJob>();
            List<CommJob> wListE = new List<CommJob>();
            List<CommJob> wListI = new List<CommJob>();
            List<CommJob> rListR = new List<CommJob>();
            List<CommJob> rListE = new List<CommJob>();
            List<CommJob> rListI = new List<CommJob>();
            Dictionary<Station, List<CommJob>> stationsInError = null;

            int nrRemainingJobsInQueuesInitial = NrRemainingJobsInQueues();
            //Stopwatch swStart = Stopwatch.StartNew();
            //Stopwatch swLockScheduler = Stopwatch.StartNew();

            lock (lockListObject)
            { 
                startList.AddRange(channelJobList);

                //write
                wListW = (from job in startList
                          where job.InUse && !job.IsPending && !job.SyncroExec
                              && job.IsConditionalVariableOn()
                              && (job.Type == LinkType.UnconditionalOutput || job.GetTagListToWriteCount() > 0)
                          orderby job.LastExecutionTime ascending
                          select job).ToList();
                bool anyQueued = (wListW.Count(j => j.IsQueued) > 0);
                if (anyQueued)
                {
                    wListW.RemoveAll(j => j.ScheduleQueue == CommJobState.PollingNow);
                    foreach (var job in wListW)
                        RemoveJobFromSchedulingQueue(job);
                }
                if (wListW.Count > 0)
                {
                    wListW.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingNow;
                        job.UpdateTagsListOnWriting();
                    });
                }

                //read
                wListR = (from job in startList
                          where (job.InUse || job.ExecuteFirstTime == true) && !job.IsPending && !job.IsQueued && !job.InErrorState
                              && job.IsConditionalVariableOn()
                              && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                              && ((dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval)
                          orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                          select job).ToList();
                if (wListR.Count > 0)
                {
                    wListR.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingInUse;
                    });
                }

                rListR = (from job in startList
                          where job.InUse && !job.IsPending && !job.IsQueued && !job.InErrorState
                              && job.IsConditionalVariableOn()
                              && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                              && ((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval && job.SamplingInterval != CommJob.JOB_NOT_SCHEDULABLE)
                          orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                          select job).ToList();

                // in error
                stationsInError = (from job in startList
                                   where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                           && job.IsConditionalVariableOn()
                                           && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                   //&& (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime
                                   orderby job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime) ascending
                                   group job by job.Station into jobsInError
                                   select new { Station = jobsInError.Key, Values = jobsInError.ToList() }).ToDictionary(t => t.Station, t => t.Values);
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
                            wListE.Add(jobs[0]);
                    }
                    else
                    {
                        wListE.AddRange(jobsToScheduleNow);
                    }
                }
                if (wListE.Count > 0)
                {
                    wListE.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingInError;
                        job.UpdateTagsListOnWriting();
                    });
                } 

                rListE = (from job in startList
                        where job.InUse && !job.IsPending && !job.IsQueued && job.InErrorState
                                && job.IsConditionalVariableOn()
                                && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                && (dtNow - job.LastErrorTime).TotalMilliseconds <= job.ErrorPollingTime
                        orderby job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime) ascending
                        select job).ToList();

                // not in use
                if (PollingTimeNotInUse > 0)
                {
                    wListI = (from job in startList
                              where !job.InUse && !job.IsPending && !job.IsQueued && !job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                  && ((dtNow - job.LastExecutionTime).TotalMilliseconds >= PollingTimeNotInUse)
                              orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                              select job).ToList();
                    if (wListI.Count > 0)
                    {
                        wListI.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.PollingNotInUse;
                            job.UpdateTagsListOnWriting();
                        });
                    }

                    rListI = (from job in startList
                              where !job.InUse && !job.IsPending && !job.IsQueued && !job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                  && ((dtNow - job.LastExecutionTime).TotalMilliseconds < PollingTimeNotInUse)
                              orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                              select job).ToList();
                }
            }
            //swLockScheduler.Stop();

            #region Write
            if (wListW.Count > 0)
            {
                if (wListW.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingNow, wListW, out nNowMax);
                else
                    nNowMax = GetScheduledJobQueue(CommJobState.PollingNow).Count();
            }
            else
            {
                nNowMax = GetScheduledJobQueue(CommJobState.PollingNow).Count();
            }
            #endregion

            #region Read            
            int nrRemainingJobsInQueuesBeforeRead = NrRemainingJobsInQueues();
            if (wListR.Count > 0)
                AddScheduledListJobQueue(CommJobState.PollingInUse, wListR, out nInUseMax);
            else
                nInUseMax = GetScheduledJobQueue(CommJobState.PollingInUse).Count();

            if (rListR.Count > 0)
            {
                var dtNextSchedule = rListR[0].LastExecutionTime.AddMilliseconds(rListR[0].SamplingInterval);
                if (dtNextSchedule > dtNow)
                    nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                if (nNextDelayUse <= 0)
                    nNextDelayUse = 0;
            }
            #endregion

            #region Error                        
            if (stationsInError != null)
            {                
                if (wListE.Count > 0)                                  
                    AddScheduledListJobQueue(CommJobState.PollingInError, wListE, out nInErrorMax);
                else
                    nInErrorMax = GetScheduledJobQueue(CommJobState.PollingInError).Count();
            }
            else
            {
                nInErrorMax = GetScheduledJobQueue(CommJobState.PollingInError).Count();
            }

            if (rListE.Count > 0)
            {
                var dtNextSchedule = rListE[0].LastErrorTime.AddMilliseconds(rListE[0].ErrorPollingTime);
                if (dtNextSchedule > dtNow)
                {
                    nNextDelayError = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    // if channel (all stations) is in general error and something were scheduled, wait to reschedule that all jobs in error are processed
                    if (InGeneralErrorState && ScheduledJobQueue[CommJobState.PollingInError].Count > 0)
                        nNextDelayError = -1;
                    // if channel or station is in general error, force to restart not far than PollingTimeInError
                    else if (nNextDelayError > PollingTimeInError && (rListE[0].Station.InGeneralErrorState || InGeneralErrorState))
                        nNextDelayError = PollingTimeInError;
                }
            }
            #endregion

            #region Not in Use
            if (PollingTimeNotInUse > 0)
            {                               
                if (wListI.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingNotInUse, wListI, out nNotInUseMax);
                else
                    nNotInUseMax = GetScheduledJobQueue(CommJobState.PollingNotInUse).Count();

                if (rListI.Count > 0)
                {
                    var dtNextSchedule = rListI[0].LastExecutionTime.AddMilliseconds(PollingTimeNotInUse);
                    if (dtNextSchedule > dtNow)
                        nNextDelayNotUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    if (nNextDelayNotUse <= 0)
                        nNextDelayNotUse = 0;
                }                               
            }
            #endregion

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

            nrRemainingJobsInQueues = NrRemainingJobsInQueues();

            nNextScheduleInterval = EvaluateMinScheduleInterval(nNextScheduleInterval, nrRemainingJobsInQueues);

            //Channel.PerformanceLogToFile(this.Name, string.Format("ChannelScheduleProcedure Initial Jobs:{0}, Total Jobs:{1}, TotalTime lockScheduler:{2}, TotalTime:{3}, NextScheduleTime:{4}", nrRemainingJobsInQueuesInitial, nrRemainingJobsInQueues, swLockScheduler.ElapsedMilliseconds, swStart.ElapsedMilliseconds, nNextScheduleInterval));

            return nNextScheduleInterval;            
        }

        protected int EvaluateMinScheduleInterval(int nNextScheduleInterval, int nrRemainingJobsInQueues)
        {
            if (nNextScheduleInterval != -1)
            {
                // considering 1 ms for each task, it forces the restart of the scheduler if the time is less than the nr of tasks
                if (nNextScheduleInterval < nrRemainingJobsInQueues)
                    nNextScheduleInterval = nrRemainingJobsInQueues;

                // forces the scheduler to be relaunch below a minimum threshold
                if (Properties.Settings.Default.MinScheduleInterval > 0 && nNextScheduleInterval < Properties.Settings.Default.MinScheduleInterval)
                    nNextScheduleInterval = Properties.Settings.Default.MinScheduleInterval;
            }

            return nNextScheduleInterval;
        }

        /// <summary>
        /// Verify if a job was scheduled to write
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        protected bool IsJobScheduledToWrite(CommJob job)
        {
            return (job.ScheduleQueue == CommJobState.PollingNow);
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Unsubscribe the job from channel. </summary>
        ///
        /// <param name="job">  job to unsubscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UnsubscribeJob(CommJob job)
        {
            lock (lockListObject)
            {
                channelJobList.Remove(job);
                if (channelJobMap.ContainsKey(job))
                    channelJobMap.Remove(job);
            }
        }
        public int GetNrSubscribeJob(Station station)
        {
            lock (lockListObject)
            {
                return channelJobList.Count(s=> s.Station == station);
            }
        }

        protected virtual void TerminateCommunicationThread()
        {
            Thread communicationThread = null;             
            ManualResetEvent communicationEvent = null;
            lock (lockThreadObject)
            {
                communicationThread = CommunicationThread;
                communicationEvent = CommunicationEvent;                
            }

            if (communicationEvent != null)
                communicationEvent.Set();

            if (communicationThread != null)
                communicationThread.Join();

            lock (lockThreadObject)
            {
                if (CommunicationEvent != null)
                {
                    CommunicationEvent.Dispose();
                    CommunicationEvent = null;
                }

                if (CommunicationThread != null)
                    CommunicationThread = null;

                channelJobList.Clear();
                channelJobMap.Clear();
            }

            CyclicCheckTerminate();

            DeviceClose();
        }

        protected bool bSuspended;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Suspend the working thread. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Suspend()
        {
            if (bSuspended)
                return;

            bSuspended = true;
            TerminateCommunicationThread();
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
            bool result = false;
            switch (state)
            {
                case CommJobState.PollingNow:
                    if (!job.SyncroExec)
                    {
                        lock (lockListObject)
                        {
                            result = channelJobMap.ContainsKey(job);
                        }

                        if (result)
                        {
                            // before to force scheduler execution for a write request, verify that is not yet scheduled                                
                            bool set = !IsJobScheduledToWrite(job);
                            if (set)
                            {
                                //PerformanceLogToFile(this.Name, "ChangeStateJob PollingNow SetRequestSchedule");
                                SetRequestSchedule();
                            }
                        }                        
                    }
                    break;
                case CommJobState.PollingInUse:
                    result = true;
                    break;
                case CommJobState.PollingNotInUse:
                    result = true;
                    break;
                case CommJobState.PollingInError:
                    result = true;
                    break;
            }
                        
            return result;
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
                lock (lockListObject)
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
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString(),true);
                        uint quality;
                        String DiagnErrorMessage = "";
                        CommDriver.GetDriverErrorInfo((int)e.ErrorCode, out quality, out DiagnErrorMessage);
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage,true);
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
            lock (lockListObject)
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
            lock (lockListObject)
            {
                foreach (CommJob job in jobs)
                {
                    if (channelJobMap.ContainsKey(job))
                    {
                        channelJobMap.ContainsKey(job);
                        channelJobList.Remove(job);
                    }
                }
            }
        }
        protected virtual void RemoveScheduledJobs(Station s, List<CommJob> jobs)
        {
            lock (lockListObject)
            {
                foreach (var state in ScheduledJobQueue.Keys)
                {
                    ScheduledJobQueue[state].RemoveAll(j => j.Station == s && jobs.Contains(j));
                }
                Parallel.ForEach(jobs, job =>
                {
                    job.ScheduleQueue = CommJobState.UnScheduled;
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

        /// <summary>
        /// Remove job from scheduling queue (is present)
        /// </summary>
        /// <param name="job"></param>
        protected virtual void RemoveJobFromSchedulingQueue(CommJob job, CommJob.ScheduleProperties schedulerProperties = CommJob.ScheduleProperties.None)
        {            
            lock (lockListObject)
            {
                try
                { 
                    if (ScheduledJobQueue.Count == 0)
                        return;

                    if (!job.IsQueued)
                        return;

                    CommJobState jobState = job.ScheduleQueue;
                    if (ScheduledJobQueue[jobState].Count > 0)
                    {
                        for (int jobIndex = 0; jobIndex<ScheduledJobQueue[jobState].Count; jobIndex++)
                        {
                            if (ScheduledJobQueue[jobState].Contains(job))
                            {
                                ScheduledJobQueue[jobState].Remove(job);
                                job.ScheduleQueue = CommJobState.UnScheduled;
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    if (schedulerProperties != CommJob.ScheduleProperties.None)
                        job.SetScheduleProperties(schedulerProperties);
                }
            }
        }


        protected DateTime lastCommExecutionDateTime = DateTime.MinValue;
        /// <summary>
        /// Set the last exection of a job/list or jobs
        /// </summary>
        protected void SetLastCommExecutionDateTime()
        {
            lastCommExecutionDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Wait time (msec) 'Wait Time' if it hasn't passed since the previous job
        /// </summary>
        protected void WaitWaitTimeFromLastCommExecution()
        {
            if (WaitTime > 0)
            {
                if (lastCommExecutionDateTime != DateTime.MinValue)
                {
                    int remainTimeToSleep = (WaitTime - (int)DateTime.UtcNow.Subtract(lastCommExecutionDateTime).TotalMilliseconds);
                    if (remainTimeToSleep > 0 && remainTimeToSleep <= WaitTime)
                        System.Threading.Thread.Sleep(remainTimeToSleep);
                }
            }
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
        #region Scheduling        
        public void InitScheduling()
        {
            // replace local sincronization object with driver lock object
            ResetRequestSchedule();
            ResetNextSchedule();
            InitData();
        }                

        private DateTime RequestSchedule;
        public void SetRequestSchedule()
        {
            RequestSchedule = DateTime.UtcNow;
            CommDriver.ScheduleChannelEventSet();
        }

        public bool IsRequestScheduleSet()
        {
            return (RequestSchedule != DateTime.MinValue);
        }

        protected void ResetRequestSchedule()
        {
            RequestSchedule = DateTime.MinValue;
        }

        private DateTime NextSchedule;
        protected void SetNextSchedule(DateTime dt, int nextScheduling = 0)
        {            
            switch (nextScheduling)
            {
                case -1: // stop scheduling                    
                    ResetNextSchedule();
                    break;

                case 0: // schedule now                    
                    SetNextSchedule(dt);
                    break;

                default:    // next scheduling
                    SetNextSchedule(dt.AddMilliseconds(nextScheduling));
                    break;
            }            
        }

        protected void SetNextSchedule(DateTime dt)
        {
            NextSchedule = dt;
        }

        protected void ResetNextSchedule()
        {
            SetNextSchedule(DateTime.MinValue);
        }

        public bool IsNextScheduleElapsed(DateTime dt)
        {
            return (NextSchedule != DateTime.MinValue && dt > NextSchedule);
        }

        public bool IsNextScheduleSet()
        {
            return (NextSchedule != DateTime.MinValue);
        }

        protected int GetNextSchedule(DateTime currentDT)
        {
            int nextSchedule = 0;
            if (NextSchedule != DateTime.MinValue)
            {
                nextSchedule = (int)NextSchedule.Subtract(currentDT).TotalMilliseconds;
                if (nextSchedule < 0)
                {
                    NextSchedule = currentDT;
                    nextSchedule = 0;
                }
            }
            else
            {
                nextSchedule = -1;
            }

            return nextSchedule;
        }

        /// <summary>
        /// Try to schedule jobs and/or retraive next schedule time
        /// </summary>
        /// <param name="scheduledDT"></param>
        /// <returns></returns>
        /// 
        public int Schedule(DateTime scheduledDT)
        {
            int nextSchedule = -1;
            int nrRemainingJobsInQueues = 0;
            if (IsRequestScheduleSet() || IsNextScheduleElapsed(scheduledDT))
            {
                nextSchedule = ChannelScheduleProcedure(scheduledDT, out nrRemainingJobsInQueues);
                ResetRequestSchedule();
                SetNextSchedule(scheduledDT, nextSchedule);
                //System.Diagnostics.Debug.WriteLine("DriverSchedulingThread DriverSchedulingThread process=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", nextScheduling=" + nextSchedule.ToString() + ",nrRemainingJobsInQueues=" + nrRemainingJobsInQueues.ToString());
            }
            else
            {
                nextSchedule = GetNextSchedule(scheduledDT);
                //System.Diagnostics.Debug.WriteLine("DriverSchedulingThread DriverSchedulingThread process=" + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + ",channel=" + this.Name + ", nextScheduling only=" + nextSchedule.ToString());
            }            

            // is some jobs pending, restart communication thread
            if (nrRemainingJobsInQueues > 0)
                StartCommunicationThread();

            return nextSchedule;
        }

        #endregion

        #region cyclically check's timer management
        private System.Threading.Timer tmrCyclically = null;
        private int tmrCyclicallyDueTime = 0;
        private int tmrCyclicallyPeriod = 0;
        private bool tmrCyclicallyIn = false;
        private object tmrCyclicallyLock = new object();
        private bool cyclicCheckRequest = false;
        
        protected virtual void CyclicCheckStart(int dueTime, int period)
        {
            if (bDisposed || CommDriver.bDisposed || tmrCyclicallyIn)
                return;

            lock (tmrCyclicallyLock)
            {            
                tmrCyclicallyIn = true;
                tmrCyclicallyDueTime = dueTime;
                tmrCyclicallyPeriod = period;
                if (tmrCyclically == null)
                    tmrCyclically = new System.Threading.Timer(CyclicCheckSet, this, tmrCyclicallyDueTime, tmrCyclicallyPeriod);
            }
        }

        protected virtual void CyclicCheckStop()
        {
            lock (tmrCyclicallyLock)
            {
                if (tmrCyclically != null)
                {
                    tmrCyclically.Dispose();
                    tmrCyclically = null;
                }
                tmrCyclicallyIn = false;
            }            
        }

        protected virtual void CyclicCheckTerminate()
        {
            CyclicCheckStop();
            CyclicCheckReset();
        }

        private void CyclicCheckSet(Object state)
        {
            cyclicCheckRequest = true;

            // force to re-start thread communication (unlock CommunicationEvent event)
            StartCommunicationThread();
        }

        private void CyclicCheckReset()
        {
            cyclicCheckRequest = false;
        }

        private bool IsCyclicCheckSet()
        {
            return cyclicCheckRequest;
        }

        protected int CyclicCheckProcedure()
        {            
            bool restartCheck = CyclicCheck();
            if (restartCheck)
                CyclicCheckReStart();
            
            // -1 : pause, 0 = start immediatly
            return (NrRemainingJobsInQueues() == 0 ? SLEEP_CYCLE_INFINITE : WaitTime);
        }

        protected virtual bool CyclicCheck()
        {
            return true;
        }

        protected virtual void CyclicCheckReStart()
        {
            CyclicCheckStop();
            CyclicCheckStart(tmrCyclicallyDueTime, tmrCyclicallyPeriod);
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

            // release communication wait answer from device
            this.SetNewDataEvent();

            TerminateCommunicationThread();                                               
            
            // FOGBUGZ 11643: moved after the stop of WorkerThread 
            if (StatisticsData != null)
            {
                StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
                StatisticsData.Dispose();                
                StatisticsData = null;
            }

            lock (lockThreadObject)
            {
                if (NewDataToAnlyze != null)
                {
                    NewDataToAnlyze.Dispose();
                    NewDataToAnlyze = null;
                }
            }

            //PerformanceLogToFileSaveToDisk();
        }
        #endregion

        #region log some scheduler operation to evaluate scheduler performance
        static string SchedulerLogFile = @"c:\temp\Scheduler.log";
        static List<String> performanceLogToFileMessages = new List<string>();
        public static void PerformanceLogToFile(String channelName, string message)
        {
            if (!String.IsNullOrEmpty(SchedulerLogFile))
            {
                try
                {
                    performanceLogToFileMessages.Add(string.Format("{0}-{1}-{2}", DateTime.UtcNow.ToString("HH:MM:ss.fff"), channelName, message));
                }
                catch (Exception ex) { }
            }
        }

        public static void PerformanceLogToFileSaveToDisk()
        {
            if (!String.IsNullOrEmpty(SchedulerLogFile))
            {
                try
                {
                    System.IO.File.AppendAllLines(SchedulerLogFile, performanceLogToFileMessages);
                }
                catch (Exception ex) { }
            }

        }
        #endregion 
    }
}
