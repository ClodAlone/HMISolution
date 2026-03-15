////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Channel.cs
//
// summary:	Implements the channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using DriverCodeBase.Enumerators;
using System.Globalization;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using System.Threading.Tasks;
using Opc.Ua;
using Utilities;
using Amib.Threading;

namespace DriverCodeBase
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
        /// <param name="multipoint" type="bool">           true to multipoint. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected Channel(CommunicationDriver commdriver, ChannelSettings settings, bool multipoint) 
            : this()
        {
            CommDriver = commdriver;
            _Name = settings.Name;
            _WaitTime = settings.WaitTime;
            _Timeout = settings.Timeout;
            _ScheduleTimeJobsList = settings.ScheduleTimeJobsList;
            _PollingTimeNotInUse = settings.PollingTimeNotInUse;
            _PollingTimeInError = settings.PollingTimeInError;
            _KeepOpened = settings.KeepOpened;
            _MultiPointProtocol = multipoint;
            _CurrentErrorCode = 0;
            _StateCommandTag = settings.StateCommandTag;
            if (_StateCommandTag != null && !NodeId.IsNull(_StateCommandTag.NodeId))
            {
                channelStateCommandVariable = new StateCommandVariable(_StateCommandTag.Name, _StateCommandTag.NodeId.ToString());
            }
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected Channel()
        {
            // default settings
            _ScheduleTimeJobsList = 100;
            _PollingTimeNotInUse = 10000;
            _PollingTimeInError = 10000;
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
            _ScheduleTimeJobsList = 100;
            _PollingTimeNotInUse = 10000;
            _PollingTimeInError = 10000;
            _KeepOpened = false;
            _MultiPointProtocol = false;
            _CurrentErrorCode = 0;
            _ConsecutiveCommErrors = 0;
            channelStateCommandVariable = new StateCommandVariable();
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
        
        /// <summary>   Buffer for lock receive data. </summary>
        protected object lockReceiveBuffer = new object();
        /// <summary>   memory buffer of received data. </summary>
        protected List<byte> ReceiveBuffer = new List<byte>();

        /// <summary>  The State/Command variable associated to the channel. </summary>
        protected StateCommandVariable channelStateCommandVariable;
        
        #endregion

        #region Private Fields/Methods

        /// <summary>   The maximum consecutive polling now jobs. </summary>
        const int MaxConsecutivePollingNowJobs = 5;
        /// <summary>   The maximum consecutive polling in use jobs. </summary>
        const int MaxConsecutivePollingInUseJobs = 2;
        /// <summary>   The maximum consecutive polling not in use jobs. </summary>
        const int MaxConsecutivePollingNotInUseJobs = 1;
        
        /// <summary>   The worker thread. </summary>
        Thread WorkerThread;
        /// <summary>   The stop worker thread. </summary>
        protected ManualResetEvent StopWorkerThread;
        /// <summary>   The new data to anlyze. </summary>
        protected ManualResetEvent NewDataToAnlyze;
        /// <summary>   The process list jobs. </summary>
        protected ManualResetEvent ProcessListJobs;

        /// <summary>   The lock list object. </summary>
        readonly Object lockListObject = new Object();
        /// <summary>   List of polling jobs. </summary>
        readonly Dictionary<CommJobState, List<CommJob>> PollingJobList = new Dictionary<CommJobState, List<CommJob>>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets polling job list. </summary>
        ///
        /// <param name="type" type="CommJobState"> The type. </param>
        ///
        /// <returns>   The polling job list. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<CommJob> GetPollingJobList(CommJobState type)
        {
            if (!PollingJobList.Keys.Contains(type))
                PollingJobList[type] = new List<CommJob>();

            return PollingJobList[type];
        }

        /// <summary>   The lock object for the queue dictionary (ScheduledJobQueue). </summary>
        readonly Object lockScheduledQueueObject = new Object();
        /// <summary>   Queue of scheduled jobs. </summary>
        readonly Dictionary<CommJobState, ConcurrentQueue<CommJob>> ScheduledJobQueue = new Dictionary<CommJobState, ConcurrentQueue<CommJob>>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets scheduled job queue. </summary>
        ///
        /// <param name="type" type="CommJobState"> The type. </param>
        ///
        /// <returns>   The scheduled job queue. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ConcurrentQueue<CommJob> GetScheduledJobQueue(CommJobState type)
        {
            lock(lockScheduledQueueObject)
            {
                if (!ScheduledJobQueue.Keys.Contains(type))
                    ScheduledJobQueue[type] = new ConcurrentQueue<CommJob>();
            }

            return ScheduledJobQueue[type];
        }

        /// <summary>   The lock schedule flag. </summary>
        protected readonly Object lockScheduleFlag = new Object();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets channel job quality. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetChannelJobQuality(uint quality)
        {
            for (CommJobState i = 0; i < CommJobState.PollingNow; i++)
            {
                var pollinglist = new List<CommJob>();
                lock (lockListObject)
                {
                    pollinglist.AddRange(GetPollingJobList(i));
                }

                if (pollinglist.Count > 0)
                    foreach (var j in pollinglist)
                        j.SetQuality(quality);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check if job is into PollingInError list
        /// 
        /// <param name="job"> the job be checked</param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsJobInPollingInErrorState(CommJob job)
        {
            lock (lockScheduledQueueObject)
            {
                return PollingJobList[CommJobState.PollingInError].Contains(job);
            }
        }

        #endregion

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

        /// <summary>   List of schedule time jobs. </summary>
        private readonly int _ScheduleTimeJobsList;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a list of schedule time jobs. </summary>
        ///
        /// <value> A List of schedule time jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ScheduleTimeJobsList
        {
            get { return _ScheduleTimeJobsList; }
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

        /// <summary>   true to multi point protocol. </summary>
        private readonly bool _MultiPointProtocol;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the multi point protocol. </summary>
        ///
        /// <value> true if multi point protocol, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MultiPointProtocol
        {
            get { return _MultiPointProtocol; }
        }

        /// <summary>   The synchro job. </summary>
        private CommJob _SynchroJob = null;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the synchro job. </summary>
        ///
        /// <value> The synchro job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommJob SynchroJob
        {
            get { return _SynchroJob; }
            set
            {
                _SynchroJob = value;
            }
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

        /// <summary>   The polling now jobs counter. </summary>
        int _PollingNowJobsCounter;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling now jobs. </summary>
        ///
        /// <value> true if polling now jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool PollingNowJobs
        {
            get
            {
                if (++_PollingNowJobsCounter < MaxConsecutivePollingNowJobs)
                    return true;

                _PollingNowJobsCounter = 0;

                return false;
            }
        }
        
        /// <summary>   The polling in use jobs counter. </summary>
        int _PollingInUseJobsCounter;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling in use jobs. </summary>
        ///
        /// <value> true if polling in use jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool PollingInUseJobs
        {
            get
            {
                if (++_PollingInUseJobsCounter < MaxConsecutivePollingInUseJobs)
                    return true;

                _PollingInUseJobsCounter = 0;

                return false;
            }
        }

        /// <summary>   The polling not in use jobs counter. </summary>
        int _PollingNotInUseJobsCounter;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the polling not in use jobs. </summary>
        ///
        /// <value> true if polling not in use jobs, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool PollingNotInUseJobs
        {
            get
            {
                if (++_PollingNotInUseJobsCounter < MaxConsecutivePollingNotInUseJobs)
                    return true;

                _PollingNotInUseJobsCounter = 0;

                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the smart thread pool. </summary>
        ///
        /// <value> The smart thread pool. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal SmartThreadPool SmartThreadPool
        {
            get
            {
                return CommDriver.SmartThreadPool;
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
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can thread running. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ThreadRunning()
        {
            return WorkerThread != null && (WorkerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Startup()
        {
            System.Diagnostics.Debug.WriteLine("{0} -- DEBUG -- Channel.Startup channel name: {1}",DateTime.UtcNow.ToString("HH:MM:ss.fff"), Name);
            if (bDisposed)
                return false;

            lock (lockPLJ)
            {
                if (ProcessListJobs == null)
                    ProcessListJobs = new ManualResetEvent(false);
                else
                    ProcessListJobs.Reset();
            }

            lock (lockThreadObject)
            {
                if (WorkerThread == null)
                {
                    WorkerThread = new Thread(StartWorkingThread);
                    WorkerThread.Name = "WorkingThread_" + Name; 
                }                    

                if (StopWorkerThread == null)
                    StopWorkerThread = new ManualResetEvent(false);
                else
                    StopWorkerThread.Reset();

                if (NewDataToAnlyze == null)
                    NewDataToAnlyze = new ManualResetEvent(false);
                else
                    NewDataToAnlyze.Reset();

                if (!WorkerThread.IsAlive)
                    WorkerThread.Start(0);

                return WorkerThread != null && (WorkerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
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
        /// <param name="pendingjob" type="CommJob">    The pendingjob. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ProcessNewData(CommJob pendingjob)
        {
            throw new NotImplementedException();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the job operation. </summary>
        ///
        /// <param name="job">  job to subscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ExecuteJob(CommJob job)
        {
            job.IsPending = true;
            job.GestRWState();
            job.StartExecutionTime = DateTime.UtcNow;
            if(job.ExecuteFirstTime)
                job.ExecuteFirstTime = false;

            job.IsRead = job.ReadRequest(); 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Calculate job's statistic values. </summary>
        ///
        /// <param name="job">  job to evaluate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void CalculateJobStatistic(CommJob job) {

            job.ExchangedByte = 0;
            job.ExchangedTag = 0;
            if (!job.IsRead)// && job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput)
            {
                lock (job.retLockList())
                {
                    job.ExchangedTag = (uint)job.TagsListOnWriting.Count;
                    job.ExchangedByte = (uint)job.TagsListOnWriting.AsParallel().Sum(tag => tag.Size); 
                }
            }
            else
            {                
                job.ExchangedTag = (uint)job.TagsList.Count;
                job.ExchangedByte = (uint)job.TagsList.AsParallel().Sum(tag => tag.Size);
            }
        }

        /// <summary>   Sets new data event. </summary>
        public virtual void SetNewDataEvent()
        {
            lock (lockThreadObject)
            {
                NewDataToAnlyze.Set();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the pending job described by job. </summary>
        ///
        /// <param name="job">  job to subscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void RemovePendingJob(CommJob job)
        {
            ListJobPending.Remove(job);
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
            if ((channelStateCommandVariable.hasBeenSet == true) && (node == channelStateCommandVariable.varNodeId))
            {
                channelStateCommandVariable.varValue = value;
            }

        }

        #endregion       

        #region WorkerThread

        /// <summary>   The lock thread object. </summary>
        protected readonly object lockThreadObject = new object();
        /// <summary>   The lock plj. </summary>
        protected readonly object lockPLJ = new object();
        /// <summary>   The list job pending. </summary>
        protected readonly List<CommJob> ListJobPending = new List<CommJob>();
        /// <summary>   The list job executed. </summary>
        protected readonly List<CommJob> ListJobExecuted = new List<CommJob>();
        /// <summary>   List of next schedule time jobs. </summary>
        protected DateTime NextScheduleTimeJobsList = new DateTime();
        /// <summary>   List of next schedule time jobs in error. </summary>
        protected DateTime NextScheduleTimeInErrorJobList = new DateTime();

        /// <summary>   true to force process list jobs. </summary>
        protected bool bForceProcessListJobs = false;
        /// <summary>   true to enable, false to disable the test polling now queue. </summary>
        protected bool bEnableTestPollingNowQueue = false;
        /// <summary>   true to enable, false to disable the test polling in use queue. </summary>
        protected bool bEnableTestPollingInUseQueue = false;
        /// <summary>   true to enable, false to disable the test polling not in use queue. </summary>
        protected bool bEnableTestPollingNotInUseQueue = false;
        /// <summary>   true to enable, false to disable the test polling in error queue. </summary>
        protected bool bEnableTestPollingInErrorQueue = false;
        /// <summary>   true to pending schedule list polling now. </summary>
        protected bool bPendingScheduleListPollingNow = false;
        /// <summary>   true to pending schedule list polling in use. </summary>
        protected bool bPendingScheduleListPollingInUse = false;
        /// <summary>   true to pending schedule list polling not in use. </summary>
        protected bool bPendingScheduleListPollingNotInUse = false;
        /// <summary>   true to pending schedule list polling in error. </summary>
        protected bool bPendingScheduleListPollingInError = false;
        /// <summary>   The process list jobs. </summary>
        protected volatile bool stopTestPollingQueue = false;

        /// <summary>   The lock statisic. </summary>
        protected readonly object lockStatisic = new object();

        void StartWorkingThread(object data)
        {
            if (!TestChannelComm())
            {
                //put all the tags to bad quality.
                SetChannelJobQuality(Opc.Ua.StatusCodes.BadNoCommunication);
            }

            WorkingThread(data);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Working thread. </summary>
        ///
        /// <param name="data" type="object">   The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {

                CommJob nextjob = null;
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    if (SynchroJob != null)
                        nextjob = SynchroJob;
                    else
                    nextjob = GetNextPendingJob();
                    if (nextjob != null)
                    {
                        ListJobPending.Add(nextjob);
                    }
                }

                if (nextjob != null)
                {
                    if (!IsDeviceOpen())
                        DeviceOpen();

                    ExecuteJob(nextjob);
                }

                lock (lockThreadObject)
                {
                    if (NewDataToAnlyze.WaitOne(0) || ReceiveBuffer.Count > 0)
                    {
                        NewDataToAnlyze.Reset();
                        if (ListJobPending.Count > 0)
                        {
                            foreach (var job in ListJobPending)
                            {
                                if (ProcessNewData(job))
                                    ListJobExecuted.Add(job);
                            }

                            foreach (var job in ListJobExecuted)
                            {
                                job.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == job)
                                {
                                    job.ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    job.ResetSynchro.Reset();
                                }
                                ListJobPending.Remove(job);
                                
                            }
                            ListJobExecuted.Clear();
                            
                        }
                    }

                    if (ListJobPending.Count > 0)
                    {
                        if (!MultiPointProtocol)
                        {
                            double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                            if (dtime > Timeout)
                            {
                                if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                {
                                    ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    ListJobPending[0].ResetSynchro.Reset();
                                }
                                //error
                                LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewData(ListJobPending[0]);
                                ListJobPending.RemoveAt(0);
                                ReceiveBuffer.Clear();
                                ManageTimeoutError();
                            }
                        }
                    }
                }

                    if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                        DeviceClose();

                    if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
                        break;
                    else if(++loop > 4)
                    {
                        loop = 0;
                        if (StopWorkerThread.WaitOne(sleepCycle))
                            break;
                    }
                    StopWorkerThread.WaitOne(0);
            }
        }

        /// <summary>   Manage the timeout error. </summary>
        protected virtual void ManageTimeoutError()
        {
        }
    
        /// <summary>   Schedule list job. </summary>
        protected virtual void ScheduleListJob(bool bAddPollingNow = false)
        {
            if (stopTestPollingQueue)
                return;

            lock (lockPLJ)
            {
                if (ProcessListJobs.WaitOne(0))
                {
                    bForceProcessListJobs = true;
                    ProcessListJobs.Reset();
                }
            }

            lock (lockScheduleFlag)
            {
                bool bExecutePollingNow = false;
                bool bExecutePollingInUse = false;
                bool bExecutePollingNotInUse = false;
                bool bExecutePollingInError = false;

                DateTime UtcNow = DateTime.UtcNow;
                if (bForceProcessListJobs || UtcNow >= NextScheduleTimeJobsList)
                {
                    if (!bForceProcessListJobs)
                    {
                        bEnableTestPollingNowQueue = true;
                        bEnableTestPollingInUseQueue = true;
                        bEnableTestPollingNotInUseQueue = true;
                        bEnableTestPollingInErrorQueue = true;
                        NextScheduleTimeJobsList = NextScheduleTimeJobsList.AddMilliseconds(ScheduleTimeJobsList);
                        if (UtcNow > NextScheduleTimeJobsList)
                            NextScheduleTimeJobsList = UtcNow;
                    }
                }

                if (bForceProcessListJobs && !bPendingScheduleListPollingNow)
                {
                    bForceProcessListJobs = false;
                    bExecutePollingNow = true;
                }
                else
                {
                    if (bEnableTestPollingNowQueue && !bPendingScheduleListPollingNow &&
                        GetPollingJobList(CommJobState.PollingNow).Count != 0)
                    {
                        if (GetScheduledJobQueue(CommJobState.PollingNow).IsEmpty || bAddPollingNow)
                        {
                            bEnableTestPollingNowQueue = false;
                            bExecutePollingNow = true;
                        }
                        else
                            NextScheduleTimeJobsList = UtcNow;
                    }
                    if (bEnableTestPollingInUseQueue && !bPendingScheduleListPollingInUse &&
                        GetPollingJobList(CommJobState.PollingInUse).Count != 0)
                    {
                        if (GetScheduledJobQueue(CommJobState.PollingInUse).IsEmpty)
                        {
                            bEnableTestPollingInUseQueue = false;
                            bExecutePollingInUse = true;
                        }
                        else
                            NextScheduleTimeJobsList = UtcNow;
                    }
                    if (bEnableTestPollingNotInUseQueue && !bPendingScheduleListPollingNotInUse &&
                        GetPollingJobList(CommJobState.PollingNotInUse).Count != 0)
                    {
                        if (GetScheduledJobQueue(CommJobState.PollingNotInUse).IsEmpty)
                        {
                            bEnableTestPollingNotInUseQueue = false;
                            bExecutePollingNotInUse = true;
                        }
                        else
                            NextScheduleTimeJobsList = UtcNow;
                    }
                    if (bEnableTestPollingInErrorQueue && !bPendingScheduleListPollingInError &&
                        GetPollingJobList(CommJobState.PollingInError).Count != 0)
                    {
                        if (GetScheduledJobQueue(CommJobState.PollingInError).IsEmpty)
                        {
                            bEnableTestPollingInErrorQueue = false;
                            bExecutePollingInError = true;
                        }
                        else
                            NextScheduleTimeJobsList = UtcNow;
                    }
                }

                if (bExecutePollingNow)
                {
                    bPendingScheduleListPollingNow = true;
                    SmartThreadPool.QueueWorkItem(() =>
                    {
                        try
                        {
                            List<CommJob> jList = null;
                            var pollinglist = new List<CommJob>();
                            lock (lockListObject)
                            {
                                pollinglist.AddRange(GetPollingJobList(CommJobState.PollingNow));
                            }
                            if (pollinglist.Count > 0)
                            {
                                var pendingWriteState = new Dictionary<CommJob, bool>();
                                pollinglist.ForEach((job) =>
                                {
                                    lock (job.retLockList())
                                    {
                                        pendingWriteState.Add(job, job.TagsListToWrite.Count != 0);
                                    }
                                });

                                jList = (from job in pollinglist/*.AsParallel()*/
                                         where !job.IsPending &&
                                                (job.Type != LinkType.ExceptionOutput || pendingWriteState[job])
                                         orderby job.LastExecutionTime ascending
                                         select job).ToList();
                            }

                            if (jList != null && jList.Count != 0)
                            {
                                ScheduleCommJobs(jList, CommJobState.PollingNow);
                            }
                        }
                        finally
                        {
                            bPendingScheduleListPollingNow = false;
                        }
                    });
                }

                if (bExecutePollingInUse)
                {
                    bPendingScheduleListPollingInUse = true;
                    SmartThreadPool.QueueWorkItem(() =>
                    {
                        try
                        {
                            List<CommJob> jList = null;
                            var pollinglist = new List<CommJob>();
                            lock (lockListObject)
                            {
                                pollinglist.AddRange(GetPollingJobList(CommJobState.PollingInUse));
                            }
                            if (pollinglist.Count > 0)
                            {
                                var pendingWriteState = new Dictionary<CommJob, bool>();
                                pollinglist.ForEach((job) =>
                                {
                                    lock (job.retLockList())
                                    {
                                        pendingWriteState.Add(job, job.TagsListToWrite.Count != 0);
                                    }
                                });

                                jList = (from job in pollinglist/*.AsParallel()*/
                                            where (((DateTime.UtcNow - job.LastExecutionTime).TotalMilliseconds > job.SamplingInterval
                                                    && !job.IsPending)
                                                    || job.ExecuteFirstTime == true) &&
                                                    (job.Type != LinkType.ExceptionOutput || pendingWriteState[job])
                                            orderby job.LastExecutionTime ascending
                                            select job).ToList();
                            }
                            if (jList != null && jList.Count != 0)
                            {
                                ScheduleCommJobs(jList, CommJobState.PollingInUse);
                            }
                        }
                        finally
                        {
                            bPendingScheduleListPollingInUse = false;
                        }
                    });
                }

                if (bExecutePollingNotInUse)
                {
                    bPendingScheduleListPollingNotInUse = true;
                    SmartThreadPool.QueueWorkItem(() =>
                    {
                        try
                        {
                            List<CommJob> jList = null;
                            var pollinglist = new List<CommJob>();
                            lock (lockListObject)
                            {
                                pollinglist.AddRange(GetPollingJobList(CommJobState.PollingNotInUse));
                            }
                            if (pollinglist.Count > 0)
                            {
                                var pendingWriteState = new Dictionary<CommJob, bool>();
                                pollinglist.ForEach((job) =>
                                {
                                    lock (job.retLockList())
                                    {
                                        pendingWriteState.Add(job, job.TagsListToWrite.Count != 0);
                                    }
                                });

                                jList = (from job in pollinglist/*.AsParallel()*/
                                         where ((PollingTimeNotInUse > 0 &&
                                                 (DateTime.UtcNow - job.LastExecutionTime).TotalMilliseconds > PollingTimeNotInUse &&
                                                  !job.IsPending)
                                                || job.ExecuteFirstTime == true) &&
                                                (job.Type != LinkType.ExceptionOutput || pendingWriteState[job])
                                         orderby job.LastExecutionTime ascending
                                         select job).ToList();
                            }
                            if (jList != null && jList.Count != 0)
                                ScheduleCommJobs(jList, CommJobState.PollingNotInUse);
                        }
                        finally
                        {
                            bPendingScheduleListPollingNotInUse = false;
                        }
                    });
                }

                //the condition has been inserted to regulate the commissioning of jobs in error, 
                //dictated by the PollingTimeInError property.
                if ((bExecutePollingInError) && ((DateTime.UtcNow - NextScheduleTimeInErrorJobList).TotalMilliseconds >= (double)PollingTimeInError))
                {
                    bPendingScheduleListPollingInError = true;
                    SmartThreadPool.QueueWorkItem(() =>
                    {
                        try
                        {
                            List<CommJob> jList = null;
                            var pollinglist = new List<CommJob>();
                            lock (lockListObject)
                            {
                                pollinglist.AddRange(GetPollingJobList(CommJobState.PollingInError));
                            }
                            if (pollinglist.Count > 0)
                            {
                                var pendingWriteState = new Dictionary<CommJob, bool>();
                                pollinglist.ForEach((job) =>
                                {
                                    lock (job.retLockList())
                                    {
                                        pendingWriteState.Add(job, job.TagsListToWrite.Count != 0);
                                    }
                                });

                                jList = (from job in pollinglist/*.AsParallel()*/
                                         where !job.IsPending &&
                                                (job.Type != LinkType.ExceptionOutput || pendingWriteState[job]) 
                                         orderby job.LastExecutionTime ascending
                                         select job).ToList();
                            }

                            if (jList != null && jList.Count != 0 )                    
                            {
                                ScheduleCommJobs(jList, CommJobState.PollingInError);
                            }
                        }
                        finally
                        {
                            bPendingScheduleListPollingInError = false;
                        }
                    });
                    NextScheduleTimeInErrorJobList = DateTime.UtcNow;
                }
            }
        }

        public void QuickPollingJobInError()
        {
            var jList = new List<CommJob>();
            lock (lockListObject)
            {
                jList.AddRange(GetPollingJobList(CommJobState.PollingInError));
            }
            DateTime firstTime = DateTime.UtcNow;
            lock (lockScheduleFlag)
            {
                foreach (CommJob j in jList)
                { 
                    if (j.InErrorState && j.LastExecutionTime < firstTime)
                    {
                        DateTime newTime = DateTime.UtcNow - new TimeSpan((long)(PollingTimeInError * 10000));
                        j.LastExecutionTime = newTime;
                    }
                }

                NextScheduleTimeInErrorJobList = DateTime.MinValue;
            }
        }

        public List<CommJob> GetJobStateList(CommJobState state)
        {
            var jList = new List<CommJob>();
            lock (lockListObject)
            {
                jList.AddRange(GetPollingJobList(state));
            }
            return jList;
        }

        public void RemoveStationJobsOnQueue(Station station)
        {
            lock (lockScheduleFlag)
            {
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingNow);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingInUse);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingNotInUse);
                RemoveJobOfStationOnSpecicQueue(station, CommJobState.PollingInError);

            }
        }

        private void RemoveJobOfStationOnSpecicQueue(Station station, CommJobState type)
        {
            ConcurrentQueue<CommJob> inputQueue;
            inputQueue = GetScheduledJobQueue(type);

            Queue<CommJob> queue = new Queue<CommJob>(inputQueue);
            if ((queue != null) && (queue.Count > 0))
            {
                while(!inputQueue.IsEmpty)
                {
                    CommJob j;
                    inputQueue.TryDequeue(out j);
                }
                while (queue.Count() != 0)
                {
                    CommJob j = queue.Dequeue();
                    if (j.Station != station)
                        inputQueue.Enqueue(j);
                }

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Schedule communications jobs. </summary>
        ///
        /// <param name="joblist" type="List<CommJob>"> The joblist. </param>
        /// <param name="type" type="CommJobState">     The type. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ScheduleCommJobs(List<CommJob> joblist, CommJobState type, bool bAddInError = false)
        {
            var invalidjobs = (from job in joblist/*.AsParallel()*/
                               where job.Type == LinkType.UnconditionalOutput &&
                               (job.TagsList.Count == 0 ||
                               (from t in job.TagsList where t.Value.Value == null select t).ToList().Count > 0)
                               select job).ToList();

            List<CommJob> queuedJoblist = new List<CommJob>();
            foreach (var job in joblist)
            {
                lock (job.retLockList())
                {
                    if (job.IsConditionalVariableOn() && job.TagsListOnWriting.Count == 0 &&
                        !invalidjobs.Contains(job))
                    {
                        queuedJoblist.Add(job);
                        if (type == CommJobState.PollingInError && !bAddInError)
                        {
                            break;
                        }
                            
                    }
                }
            }

            ConcurrentQueue<CommJob> queue = GetScheduledJobQueue(type);
            foreach (var job in queuedJoblist)
            {
                lock (lockScheduleFlag)
                {
                    if (!queue.Contains(job))
                    {
                        queue.Enqueue(job);
                    }
                }
            }
            foreach (var job in invalidjobs)
            {
                job.SetQuality(StatusCodes.BadWriteNotSupported);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected CommJob GetNextPendingJob()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    for (int i = queue.Count; i > 0; i--)
                    {
                        CommJob job = null;
                        if(!queue.TryDequeue(out job))
                        {
                            break;
                        }
                        if (job.IsPending == false)
                        {
                            return (job);
                        }
                        else
                        {
                            queue.Enqueue(job);
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
        protected ConcurrentQueue<CommJob> GetNextPendingQueue()
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
            return null;
        }

        public long InErrorJobs(Station st)
        {           
            long result = 0;
            List<CommJob> jList = null;
            lock (lockListObject)
            {
                jList = GetPollingJobList(CommJobState.PollingInError);
                result = (from job in jList/*.AsParallel()*/
                          where job.Station == st && job.InErrorState
                          select job).ToList().Count;
            }

            return result;         
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

            System.Diagnostics.Trace.TraceInformation("Channel.TerminateStatistics setting StatisticsData to null\n");
           
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
            lock (lockListObject)
            {
                if (!GetPollingJobList(state).Contains(job))
                    GetPollingJobList(state).Add(job);
            }

            if (SubscribedJobs > 0 && WorkerThread == null)
                Startup();

            lock (lockPLJ)
            {
                if (ProcessListJobs != null && state == CommJobState.PollingNow)
                    ProcessListJobs.Set();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Unsubscribe the job from channel. </summary>
        ///
        /// <param name="job">  job to unsubscribe. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UnsubscribeJob(CommJob job)
        {
            //System.Diagnostics.Trace.TraceInformation("Channel.UnsubscribeJob {0}", job.ToString());
            lock (lockListObject)
            {
                for (CommJobState i = 0; i < CommJobState.PollingNow; i++)
                    GetPollingJobList(i).Remove(job);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Suspend the working thread. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Suspend()
        {
            if (StopWorkerThread != null)
                StopWorkerThread.Set();
            if (WorkerThread != null)
                WorkerThread.Join();
            WorkerThread = null;
            DeviceClose();
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
            //System.Diagnostics.Trace.TraceInformation("Channel.ChangeStateJob {0}", job.ToString());
            bool bFound = false;
            lock (lockListObject)
            {
                for (CommJobState i = 0; i <= CommJobState.PollingNow; i++)
                    bFound |= GetPollingJobList(i).Remove(job);

                if (bFound)
                {
                    GetPollingJobList(state).Add(job);
                }
            }

            if (bFound)
            {
                if (SubscribedJobs > 0 && WorkerThread == null)
                {
                    Startup();
                }
                //SubscribeJob(job, state);
                lock (lockPLJ)
                {
                    if (ProcessListJobs != null && state == CommJobState.PollingNow)
                        ProcessListJobs.Set();
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
                long result = 0;
                lock (lockListObject)
                {
                    for (CommJobState i = 0; i < CommJobState.PollingNow; i++)
                        result += GetPollingJobList(i).Count;
                }

                return result;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the in use jobs. </summary>
        ///
        /// <value> The in use jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long InUseJobs
        {
            get
            {
                long result = 0;
                lock (lockListObject)
                {
                    result = GetPollingJobList(CommJobState.PollingInUse).Count +
                        GetPollingJobList(CommJobState.PollingInError).Count +
                        GetPollingJobList(CommJobState.PollingNow).Count;
                }

                return result;
            }
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
                CalculateJobStatistic(e.Job);

                lock (lockStatisic)
                {
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
                lock (e.Job.retLockList())
                {
                    if (e.Job.LocalMethod == false)
                    {
                        e.Job.SynchroValues = e.Values;
                        e.Job.SynchroError = (int)e.ErrorCode;
                        e.Job.ResetSynchro.Reset();
                    }
                    if (e.Job.TagsListToWrite.Count == 0)
                        e.Job.EndSynchroExec.Set();
                }
            }
#if DEBUG
            if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                System.Diagnostics.Trace.TraceError(string.Format("Error #{0}", e.ErrorCode));
#endif
            EventHandler<ExecutedJobArgs> temp = JobExecuted;
            if (temp != null)
                temp(this, e);
        }

        /// <summary>   Executes the refresh diagnostic action. </summary>
        public void OnRefreshDiagnostic()
        {
            if (StatisticsData != null)
            {
                lock (lockStatisic)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.JobRate.ToString(), TotalPartialJobs);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.TagRate.ToString(), TotalPartialTags);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.ByteRate.ToString(), TotalPartialBytes);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.CommunicationTaskRate.ToString(), TotalPartialTasks);

                    string CommunicationTaskTime = "uncertain";
                    if (TotalPartialTasks > 0)
                        CommunicationTaskTime = (DiagnLastTaskTime / TotalPartialTasks).ToString("F1");
                    StatisticsData.Update(StatisticSetting.NodeDataNames.TimeForCommunicationTask_mS.ToString(), CommunicationTaskTime);

                    TotalPartialJobs = 0;
                    TotalPartialTags = 0;
                    TotalPartialBytes = 0;
                    TotalPartialTasks = 0;
                    DiagnLastTaskTime = 0;

                    bool InErrorState = false;
                    foreach (var station in CommDriver.GetChannelStations(this))
                    {
                        if (station.InErrorState)
                        {
                            StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), station.LastStatisticErrorTime);
                            StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), station.LastStatisticError );
                            StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                            InErrorState = true;
                            break;
                        }
                    }
                    if (!InErrorState)
                        StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);
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
        #endregion

        #region IDisposable Interface

        bool bDisposed;
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

            // FOGBUGZ 11643: moved after the stop of WorkerThread 
            //if (StatisticsData != null)
            //{
            //    StatisticsData.Dispose();
            //    StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
            //    StatisticsData = null;
            //}

            Thread workerthread = null;
            lock (lockThreadObject)
            {
                if (StopWorkerThread != null)
                    StopWorkerThread.Set();
                if (WorkerThread != null)
                    workerthread = WorkerThread;
            }

            if (workerthread != null && workerthread.IsAlive)
                workerthread.Join();

            // FOGBUGZ 11643: moved after the stop of WorkerThread 
            if (StatisticsData != null)
            {
                StatisticsData.Dispose();
                StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;

                System.Diagnostics.Trace.TraceInformation("Channel.Dispose setting StatisticsData to null\n");

                StatisticsData = null;
            }

            stopTestPollingQueue = true;

            lock (lockListObject)
            {
                foreach (var key in PollingJobList.Keys)
                    PollingJobList[key].Clear();
            }

            if (NewDataToAnlyze != null)
                NewDataToAnlyze.Dispose();
            if (ProcessListJobs != null)
                ProcessListJobs.Dispose();
            if (StopWorkerThread != null)
                StopWorkerThread.Dispose();
        }

        #endregion

    }
}
