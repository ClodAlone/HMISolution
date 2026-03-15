using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BrPvi
{
    public class BrPviChannel : ChannelList
    {
        private class ActiveJob
        {
            public PviObjEvent pviEvent { set; get; }
            public BrPviPviObject pviObj { set; get; }
            public CommJob Job { set; get; }
            public DriverErrorCodes Conn { set; get; }
            public bool Completed { set; get; }
            public ActiveJob(CommJob job)
            {
                pviEvent = null;
                pviObj = null;
                Job = job;
                Conn = DriverErrorCodes.ErrorNoError;
                Completed = false;
            }

            ///// <summary>
            ///// Method use to customize .Contain search method
            ///// </summary>
            ///// <param name="obj"></param>
            ///// <returns></returns>
            //public override bool Equals(Object obj)
            //{
            //    if (obj == null || !(obj is ActiveJob))
            //        return false;
            //    else
            //        return Job == ((ActiveJob)obj).Job;
            //}
        }

        #region Constructors

        /// <summary>
        /// Initializes the BrPviChannel object.
        /// </summary>
        public BrPviChannel(CommunicationDriver commdriver, BrPviChannelSettings settings)
            : base(commdriver, settings)
        {
            _BrPviServerAddress = settings.BrPviServerAddress;
            _BrPviServerPort = settings.BrPviServerPort;
            _BrPviServerCommunicationTimeout = settings.BrPviServerCommunicationTimeout;
            _BrPviRetryTime = settings.BrPviRetryTime;
            _BrPviMaxNumberSubscriptions = (uint)settings.BrPviMaxNumberSubscriptions;
            lastPviManagerConnectionErrorCode = 0;
            channelPviSink = new PviSink(this);
        }

        public void DeleteJobPviObjectes(string stationName, string pviobjName)
        {
            lock (lockmapPviObjects)
            {
                if (mapPviObjects.ContainsKey(pviobjName))
                    mapPviObjects.Remove(pviobjName);
            }
        }

        public void DisableJobPviObjectes(string stationName, string pviobjName)
        {
            BrPviPviObject requestedPviObject;
            lock (lockmapPviObjects)
            {
                if (mapPviObjects.TryGetValue(pviobjName, out requestedPviObject))
                {
                    requestedPviObject.Status = PviObjectStatus.NotCreated;
                    requestedPviObject.LinkID = 0;                    
                }
            }
        }        
        #endregion

        #region Data Members

        protected object lockStream = new object();
        PviSink channelPviSink;
        PviComManager pviCommunicationManager = new PviComManager();
        PviComEvents pviEventsObj;
        PviCallbackWithData64Bit pviConnectCallback64Bit;
        PviCallbackWithData32Bit pviConnectCallback32Bit;
        GCHandle gcHandleConnectCallbackFunc;
        IntPtr intptrConnectCallback;
        PviCallbackWithData64Bit pviDisconnectCallback64Bit;
        PviCallbackWithData32Bit pviDisconnectCallback32Bit;
        GCHandle gcHandleDisconnectCallbackFunc;
        IntPtr intptrDisconnectCallback;
        PviCallbackWithData64Bit pviArrangeCallback64Bit;
        PviCallbackWithData32Bit pviArrangeCallback32Bit;
        GCHandle gcHandleArrangeCallbackFunc;
        IntPtr intptrArrangeCallback;
        PviCallbackWithData64Bit pviDataCallback64Bit;
        PviCallbackWithData32Bit pviDataCallback32Bit;
        GCHandle gcHandleDataCallbackFunc;
        IntPtr intptrDataCallback;
        GCHandle gcHandleChannel;
        IntPtr intptrChannel;
        // List of PVI global events
        //List<PviEventInfo> listOfGlobalEvents = new List<PviEventInfo>();
        int lastPviManagerConnectionErrorCode;
        BrPviProcol.BrPviConnectionStatus lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.NotInitialized;  // connection's status to PVI manager
        bool pviManagerDeviceCloseRequest = false;
        ManualResetEvent lastPviManagerConnectionWait = new ManualResetEvent(false);
        BrPviProcol.DeviceStateCheckMode checkStateCaller = BrPviProcol.DeviceStateCheckMode.None;
        Object lockmapPviObjects = new Object();
        // General dictionary of PVI objects
        Dictionary<string, BrPviPviObject> mapPviObjects = new Dictionary<string, BrPviPviObject>();
        // Dictionary of PVI objects linked to stations
        Dictionary<string, List<BrPviStation>> mapPviObjStations = new Dictionary<string, List<BrPviStation>>();
        // Dictionary of PVI objects linked to jobs
        Dictionary<string, List<BrPviCommJob>> mapPviObjJobs = new Dictionary<string, List<BrPviCommJob>>();
        // List of PVI objects to be created
        List<BrPviPviObject> listOfPviObjectsToBeCreated = new List<BrPviPviObject>();
        // List of events on PVI objects
        // List of global handles for the PVI objects
        Dictionary<string, GCHandle> mapGlobalHandles = new Dictionary<string, GCHandle>();
        // List of write requests
        List<PviWriteObj> listOfPviWriteRequests = new List<PviWriteObj>();
        // List of write requests
        List<PviWriteObj> listOfPviMaskWriteRequests = new List<PviWriteObj>();
        // List of string length requests
        List<BrPviPviObject> listOfStringLengthRequests = new List<BrPviPviObject>();
        // List of delete requests
        List<BrPviPviObject> listOfDeleteRequests = new List<BrPviPviObject>();

        private object activeJobsLock = new object();
        private List<ActiveJob> activeJobs = new List<ActiveJob>();
        private bool activeJobsFilling = false;
        public bool TestCommMode = false;
        // list of jobs to be unsubscribe from PVIManager
        private Dictionary<CommJob, bool> mapJobsToUnSubScribe = new Dictionary<CommJob, bool>();

        #endregion

        #region Abstracts Methods

        //public override bool TestChannelComm()
        //{
        //    return DeviceOpen();
        //}

        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool connectionState = false;
                if (pviCommunicationManager != null)
                    connectionState = pviCommunicationManager.IsInitialized();

                if (connectionState)
                    connectionState = (lastPviManagerConnectionStatus == BrPviProcol.BrPviConnectionStatus.Arranged && !pviManagerDeviceCloseRequest);

                // channel state is the connection state with PVI Manager (not the connection state with Plc)
                SetStateCommandVariableBit((lastPviManagerConnectionStatus != BrPviProcol.BrPviConnectionStatus.Arranged), (UInt16)ChannelVariableBits.ChannelUnconnected);

                return connectionState;
            }
        }

        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                if (pviCommunicationManager == null)
                    pviCommunicationManager = new PviComManager();

                if (pviCommunicationManager.IsInitialized())
                    return (true);

                if (pviEventsObj == null)
                {
                    pviEventsObj = new PviComEvents(this);
                    unsafe
                    {
                        pviConnectCallback64Bit = pviEventsObj.PviConnectCallback64Bit;
                        pviDisconnectCallback64Bit = pviEventsObj.PviDisconnectCallback64Bit;
                        pviArrangeCallback64Bit = pviEventsObj.PviArrangeCallback64Bit;
                        pviDataCallback64Bit = pviEventsObj.PviDataCallback64Bit;
                        pviConnectCallback32Bit = pviEventsObj.PviConnectCallback32Bit;
                        pviDisconnectCallback32Bit = pviEventsObj.PviDisconnectCallback32Bit;
                        pviArrangeCallback32Bit = pviEventsObj.PviArrangeCallback32Bit;
                        pviDataCallback32Bit = pviEventsObj.PviDataCallback32Bit;
                    }
                    gcHandleChannel = GCHandle.Alloc(this);
                    intptrChannel = GCHandle.ToIntPtr(gcHandleChannel);
                    if (System.Environment.Is64BitProcess)
                    {
                        intptrConnectCallback = Marshal.GetFunctionPointerForDelegate(pviConnectCallback64Bit);
                        gcHandleConnectCallbackFunc = GCHandle.Alloc(pviConnectCallback64Bit);
                        intptrDisconnectCallback = Marshal.GetFunctionPointerForDelegate(pviDisconnectCallback64Bit);
                        gcHandleDisconnectCallbackFunc = GCHandle.Alloc(pviDisconnectCallback64Bit);
                        intptrArrangeCallback = Marshal.GetFunctionPointerForDelegate(pviArrangeCallback64Bit);
                        gcHandleArrangeCallbackFunc = GCHandle.Alloc(pviArrangeCallback64Bit);
                        intptrDataCallback = Marshal.GetFunctionPointerForDelegate(pviDataCallback64Bit);
                        gcHandleDataCallbackFunc = GCHandle.Alloc(pviDataCallback64Bit);
                    }
                    else
                    {
                        intptrConnectCallback = Marshal.GetFunctionPointerForDelegate(pviConnectCallback32Bit);
                        gcHandleConnectCallbackFunc = GCHandle.Alloc(pviConnectCallback32Bit);
                        intptrDisconnectCallback = Marshal.GetFunctionPointerForDelegate(pviDisconnectCallback32Bit);
                        gcHandleDisconnectCallbackFunc = GCHandle.Alloc(pviDisconnectCallback32Bit);
                        intptrArrangeCallback = Marshal.GetFunctionPointerForDelegate(pviArrangeCallback32Bit);
                        gcHandleArrangeCallbackFunc = GCHandle.Alloc(pviArrangeCallback32Bit);
                        intptrDataCallback = Marshal.GetFunctionPointerForDelegate(pviDataCallback32Bit);
                        gcHandleDataCallbackFunc = GCHandle.Alloc(pviDataCallback32Bit);
                    }
                }

                // Initialize the PVICOM interface, establish a PVICOM communication instance,
                // and initiate the registration of the communication instance (client) with PVI Manager(server)
                if (!channelPviSink.PviXInitialize())
                    return (false);

                if (lastPviManagerConnectionErrorCode != 0)
                    return (false);

                // Set the callback functions for receiving notifications for the global events POBJ_EVENT_PVI_CONNECT,
                // POBJ_EVENT_PVI_DISCONN and POBJ_EVENT_PVI_ARRANGE
                if (!channelPviSink.PviXSetGlobEventMsg())
                    return (false);

                if (lastPviManagerConnectionErrorCode != 0)
                {
                    channelPviSink.PviXDeinitialize();
                    return (false);
                }

                return (lastPviManagerConnectionErrorCode == 0 && lastPviManagerConnectionStatus == BrPviProcol.BrPviConnectionStatus.Arranged);
            }
        }

        public override bool DeviceClose()
        {
            lock (lockStream)
            {
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                if (pviCommunicationManager.IsInitialized())
                {
                    //channelPviSink.Close();

                    if (!channelPviSink.PviXDeinitialize() || (lastPviManagerConnectionErrorCode != 0))
                    {
                        DeallocateGlobalHandles();
                        return (false);
                    }
                }

                ResetListAndMaps();

                pviManagerDeviceCloseRequest = false;
            }

            return true;
        }

        public override DriverErrorCodes CheckDevice(List<CommJob> list, object thischannel)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (pviManagerDeviceCloseRequest)
                {
                    ResetJobsState();
                    DeviceClose();
                }

                if (!DeviceOpen())
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
            }

            return conn;
        }

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }

        /// <summary>
        /// Return the nr of jobs driver is currently processing (subscribre, write, ecc --> active operation)
        /// </summary>
        /// <returns></returns>
        private bool AnyActiveJobs()
        {
            lock (activeJobsLock)
            {
                return (activeJobs.Count > 0);
            }
        }

        /// <summary>
        /// Set ActiveJob filling state to manage data event while driver is in ExecutinJobs
        /// </summary>
        /// <param name="filling"></param>
        private void SetActiveJobsFilling(bool filling)
        {
            lock (activeJobsLock)
            {
                activeJobsFilling = filling;
            }
        }

        /// <summary>
        /// Return true if all active job are processed with ManagePviEvents()
        /// </summary>
        /// <returns></returns>
        private bool IsAllActiveJobsCompleted()
        {
            lock (activeJobsLock)
            {
                return (!activeJobsFilling && activeJobs.Count(aj => aj.Completed) == activeJobs.Count);
            }
        }

        private CommJob GetActiveJobOrFirst()
        {
            lock (activeJobsLock)
            {
                return (activeJobs.Count == 0 ? null : activeJobs[0].Job);
            }
        }

        private CommJob GetActiveJobOrFirst(string pviOjectName)
        {
            lock (activeJobsLock)
            {
                ActiveJob aj = activeJobs.Find(a => ((BrPviCommJob)a.Job).BrPviVariableCompleteName == pviOjectName);
                if (aj == null)
                    return null;
                else
                    return aj.Job;
            }
        }

        /// <summary>
        /// Return (ad remove from internal list) the list of active jobs
        /// </summary>
        /// <returns></returns>
        private List<ActiveJob> GetAndRemoveAllActiveJobs()
        {
            lock (activeJobsLock)
            {
                activeJobsFilling = false;
                List<ActiveJob> d = new List<ActiveJob>(activeJobs);
                activeJobs.Clear();
                return d;
            }
        }

        private void AddToActiveJobs(BrPviCommJob job)
        {
            lock (activeJobsLock)
            {
                activeJobs.Add(new ActiveJob(job));
            }
        }

        /// <summary>
        /// Remove job previous "scheduled"
        /// </summary>
        /// <param name="job"></param>
        private void RemoveFromActiveJobs(CommJob job)
        {
            lock (activeJobsLock)
            {
                activeJobs.RemoveAll(aj => aj.Job == job);
            }
        }

        private void UnSetActiveJob(CommJob job, PviObjEvent pvievent = null, BrPviPviObject pviPviObject = null, DriverErrorCodes? conn = null)
        {
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - UnSetActiveJob", DateTime.Now.ToString("HH:mm:ss.fff"));
            lock (activeJobsLock)
            {
                ActiveJob aj = activeJobs.Find(a => a.Job == job);
                if (aj == null)
                    return;

                //anyActiveJob = false;
                if (pvievent != null)
                    aj.pviEvent = (PviObjEvent)pvievent.Clone();
                else
                    aj.pviEvent = null;

                if (pviPviObject != null)
                    aj.pviObj = (BrPviPviObject)pviPviObject.Clone();
                else
                    aj.pviObj = null;

                if (conn.HasValue)
                    aj.Conn = (DriverErrorCodes)conn;

                aj.Completed = true;
            }
        }

        private void UnSetAllActiveJob(PviObjEvent newItem = null, BrPviPviObject newItem2 = null, DriverErrorCodes? conn = null)
        {
            lock (activeJobsLock)
            {
                foreach (ActiveJob aj in activeJobs)
                    UnSetActiveJob(aj.Job, newItem, newItem2, conn);
            }
        }

        private void GetConnFromActiveJob(ref DriverErrorCodes conn)
        {
            lock (activeJobsLock)
            {
                foreach (ActiveJob aj in activeJobs)
                {
                    if (aj.Completed && aj.Conn != DriverErrorCodes.ErrorNoError)
                    {
                        conn = aj.Conn;
                        break;
                    }
                }
            }
        }

        private void CheckTimeoutAllActiveJobs(ref DriverErrorCodes conn)
        {
            lock (activeJobsLock)
            {
                if (activeJobs.Count > 1)
                {   
                    if (activeJobs.Count(aj => aj.Conn == DriverErrorCodes.ErrorDeviceOpenFailed) > 0)
                        conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                    else if (activeJobs.Count(aj => aj.Completed) > 0)
                        conn = DriverErrorCodes.ErrorNoError;
                }
            }
        }
                
        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            // except tag subcription/unsubscription to PVI Manager, all others job's task is performe "individually"
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool subscribe = false;
                bool first = true;
                bool process = true;
                //append a new list to the end of exList
                Dictionary<string, CommJob> list = new Dictionary<string, CommJob>();

                while (jobIndex < jobList.Count)
                {
                    BrPviCommJob j = jobList.ElementAt(jobIndex) as BrPviCommJob;
                    if (first)
                    {
                        subscribe = j.IsSubscribeState();
                        first = false;
                    }

                    process = true;
                    // unsubscribe request to PVI are managed by SplitInExecutionListsUnsubscrive
                    // if connection is not yet initializad, schedule one job at time
                    // write operation,etc are performed "individually"
                    if (!((BrPviStation)j.Station).pviObjectsInitialized || !(subscribe && j.IsSubscribeState()))
                    {
                        process = false;
                        // if no jobs were "aggregated" before, create a "special" list with single job
                        if (list.Count == 0)
                        {
                            list[j.BrPviVariableCompleteName] = j;                            
                            jobList.RemoveAt(jobIndex);
                            break;
                        }
                    }

                    if (process)
                    {                       
                        if (subscribe && j.IsSubscribeState())
                        {
                            // don't try to subscribe a job already scheduled with the same Plc variable name during subscription --> limit of PVI Manager                                
                            if (!list.ContainsKey(j.BrPviVariableCompleteName))
                            {
                                if (list.Values.Count >= _BrPviMaxNumberSubscriptions)
                                    break;
                                list[j.BrPviVariableCompleteName] = j;
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }                        
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    // reset unsubscribe request if another operation have to be do with job
                    foreach (var job in list.Values)
                    {
                        if (IsJobToUnSubScribe((BrPviCommJob)job))
                            RemoveJobToUnSubScribe((BrPviCommJob)job);
                    }

                    exList.Add(list.Values.ToList());
                    jobIndex = 0;
                }
            }
        }

        /// <summary>
        /// Schedule job to be unsubscribe
        /// </summary>
        /// <param name="jobList"></param>
        /// <param name="exList"></param>
        private void SplitInExecutionListsUnSubScribe(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                //append a new list to the end of exList
                Dictionary<string, CommJob> list = new Dictionary<string, CommJob>();

                while (jobIndex < jobList.Count)
                {
                    BrPviCommJob j = jobList.ElementAt(jobIndex) as BrPviCommJob;
                    if (j != null)
                    {
                        if (!list.ContainsKey(j.BrPviVariableCompleteName))
                        {
                            if (list.Values.Count <= _BrPviMaxNumberSubscriptions)
                            {
                                list[j.BrPviVariableCompleteName] = j;
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    // scheduling of the job to be unsubscrive was moved from external list to job : remove from list but not reset internal state
                    foreach (var job in list.Values)
                    {
                        RemoveJobToUnSubScribe((BrPviCommJob)job);
                        ((BrPviCommJob)job).BrPviUnSubScribeRequested = BrPviCommJob.UnSubScribeState.Requested;
                    }

                    exList.Add(list.Values.ToList());
                    jobIndex = 0;
                }
            }
        }

        #endregion

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            // manage connection error into ProcessNewData
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            bool jobsList = (list.Count > 1);
            if (jobsList)
            {
                RemoveUnecessaryJobsDuringStationInitialization(ref list);
                jobsList = (list.Count > 1);
            }
            
            if (jobsList)
                SetActiveJobsFilling(true);                

            // specific case
            bool waitNewDataEvent = true;

            foreach (BrPviCommJob brJob in list)
            {
                if (brJob.BrPviUnSubScribeRequested == BrPviCommJob.UnSubScribeState.Requested)
                {                                       
                    // if in the while the job return in use abort unsubscribe operation
                    if (brJob.InUse && (brJob.pviObjectState == BrPviCommJob.PviObjectState.NotInitialized || brJob.pviObjectState == BrPviCommJob.PviObjectState.Activated))
                    {
                        brJob.BrPviUnSubScribeRequested = BrPviCommJob.UnSubScribeState.None;
                        RemovePendingJob(brJob);
                        continue;
                    }
                    else
                    {
                        brJob.BrpviState = BrPviCommJob.BrPviState.JobUnSubScribe;
                    }
                }

                brJob.CheckBrPviState();                

                // Pvi server don't accept/respond a request of subscription/string length of variable (name) already suscbribed; use settings/value of subscribed job (with same variable name)
                if (brJob.IsBrPviJobInitialiazionState() && MoreJobsHaveSamePlcVariable(brJob))
                    CopyJobSettings(GetLastUpdatedJobFromPviObj(brJob.BrPviVariableCompleteName), brJob);

                if (brJob.IsBrPviJobInitialized())
                    brJob.GetReadWriteState();

                switch (brJob.BrpviState)
                {
                    case BrPviCommJob.BrPviState.None:
                        // unmanaged
                        break;

                    case BrPviCommJob.BrPviState.StationPviObjectInitialization:
                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - InizializePviObject 1", DateTime.Now.ToString("HH:mm:ss.fff"));
                        // preperare list of elements to send to server to create Station
                        if (((BrPviStation)brJob.Station).CreatePviObjects())
                        {
                            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - InizializePviObject 2", DateTime.Now.ToString("HH:mm:ss.fff"));
                            AddToActiveJobs(brJob);
                            // send list of elements to server to create Station
                            if (channelPviSink.CreatePviObjects())
                            {
                                // wait result
                                //return true;
                                break;
                            }
                            else
                            {
                                //UnSetActiveJob(brJob);
                                RemoveFromActiveJobs(brJob);
                            }
                        }

                        //conn = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed;
                        OnJobExecuted(new ExecutedJobArgs() { Job = brJob, ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed });
                        //return false;
                        break;

                    case BrPviCommJob.BrPviState.JobPviObjectInitialization:
                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - InitializeJobPviObject 1", DateTime.Now.ToString("HH:mm:ss.fff"));
                        base.ExecuteJob(brJob);

                        if (brJob.CreatePviObjects())
                        {
                            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - InitializeJobPviObject 2", DateTime.Now.ToString("HH:mm:ss.fff"));
                            AddToActiveJobs(brJob);
                            if (channelPviSink.CreatePviObjects())
                            {
                                // wait answer from callback
                                //return true;
                                break;
                            }
                            else
                            {
                                //UnSetActiveJob(brJob);
                                RemoveFromActiveJobs(brJob);
                            }
                        }
                        //conn = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed;
                        OnJobExecuted(new ExecutedJobArgs() { Job = brJob, ErrorCode = (DriverErrorCodes)(DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed });
                        //return false;
                        break;

                    case BrPviCommJob.BrPviState.JobStringLengthRequest:
                        BrPviPviObject pviVarObject = GetPviObject(brJob.BrPviVariableCompleteName);
                        AddRequestToReadStringLengthList(pviVarObject);
                        AddToActiveJobs(brJob);
                        // Send the request
                        if (channelPviSink.SendStringLengthRequests())
                        {
                            // wait answer from callback
                            //return true;
                            break;
                        }
                        else
                        {
                            //UnSetActiveJob(brJob);
                            RemoveFromActiveJobs(brJob);
                        }
                        //conn = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed;
                        OnJobExecuted(new ExecutedJobArgs() { Job = brJob, ErrorCode = (DriverErrorCodes)(DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed });
                        //return false;
                        break;

                    case BrPviCommJob.BrPviState.JobInitialized:
                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - JobInitialized 1", DateTime.Now.ToString("HH:mm:ss.fff"));                    
                        switch (brJob.CommandType)
                        {
                            case BrPviCommJob.CommandTypes.WriteCmd:
                                {
                                    PviWriteObj pviWriteObject = PrepareWriteRequest(brJob);
                                    if (pviWriteObject != null && pviWriteObject.DataLength != 0)
                                    {
                                        base.ExecuteJob(brJob);
                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Calling AddRequestToWriteList", DateTime.Now.ToString("HH:mm:ss.fff"));
                                        // Add the request to the list of write requests
                                        AddRequestToWriteList(pviWriteObject);
                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Calling channelPviSink.WritePviObjects", DateTime.Now.ToString("HH:mm:ss.fff"));

                                        // Send the request
                                        AddToActiveJobs(brJob);
                                        if (channelPviSink.WritePviObjects())
                                        {
                                            // wait answer from callback
                                            //return true;
                                            break;
                                        }
                                        else
                                        {
                                            //UnSetActiveJob(brJob);
                                            RemoveFromActiveJobs(brJob);
                                        }
                                    }
                                    //conn = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodePrepareWriteRequest;
                                    //return false;
                                    OnJobExecuted(new ExecutedJobArgs() { Job = brJob, ErrorCode = (DriverErrorCodes)(DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodePrepareWriteRequest });
                                    break;
                                }
                            case BrPviCommJob.CommandTypes.ReadCmd:
                                {                                    
                                    base.ExecuteJob(brJob);
                                    waitNewDataEvent = false;
                                    // do nothing here --> use data stored in PviObj (populated by ManagePviEvent) into ProcessNewData()
                                }
                                //return false;
                                break;
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobUnMapped:
                        {
                            //ExecutedJobArgs e = new ExecutedJobArgs();
                            //e.Job = brJob;
                            //e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError;
                            //OnJobExecuted(e);
                            //conn = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError;
                            //return false;
                            OnJobExecuted(new ExecutedJobArgs() { Job = brJob, ErrorCode = (DriverErrorCodes)(DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError });
                            break;
                        }

                    case BrPviCommJob.BrPviState.JobUnSubScribe:
                        {                            
                            pviVarObject = GetPviObject(brJob.BrPviVariableCompleteName);
                            AddRequestToDeleteList(pviVarObject);
                            AddToActiveJobs(brJob);
                            if (channelPviSink.SendDeleteRequests())
                            {
                                // wait answer from callback
                                break;
                            }
                            else
                            {
                                brJob.BrPviUnSubScribeRequested = BrPviCommJob.UnSubScribeState.None;
                                RemovePendingJob(brJob);
                                RemoveFromActiveJobs(brJob);
                            }                                                        
                            break;
                        }
                }
            }
            list.RemoveAll(j => !j.IsPending);

            if (jobsList)
            {
                // reset job filling "lock" and check if all jobs are already proceseed
                SetActiveJobsFilling(false);
                if (IsAllActiveJobsCompleted())
                    SetNewDataEvent();
            }

            // specific case
            if (list.Count == 1 && !waitNewDataEvent)
                return false;
            else
                return (list.Count > 0);
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            base.WaitNewDataEvent(ref conn);
            // get connection error from PVI Manager
            if (conn == DriverErrorCodes.ErrorNoError)
                GetConnFromActiveJob(ref conn);

            // check if TimeOut error is "correct", or due the fact that all jobs are not processed
            if (conn == DriverErrorCodes.ErrorTimeOut)
                CheckTimeoutAllActiveJobs(ref conn);

            if (conn != DriverErrorCodes.ErrorNoError)
                UnSetAllActiveJob();
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            List<ActiveJob> activeJobs = GetAndRemoveAllActiveJobs();

            switch (conn)
            {
                case DriverErrorCodes.ErrorDeviceOpenFailed:
                    ManageConnectionBroken(list);
                    return true;
            }

            foreach (BrPviCommJob brJob in list)
            {
                ActiveJob aj = activeJobs.Find(a => a.Job == brJob);
                if (aj == null)
                    aj = new ActiveJob(brJob);

                switch (brJob.BrpviState)
                {
                    case BrPviCommJob.BrPviState.None:
                        // unmanaged
                        break;

                    case BrPviCommJob.BrPviState.StationPviObjectInitialization:
                        {
                            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ProcessNewData {0} - StationPviObjectInitialization 1", DateTime.Now.ToString("HH:mm:ss.fff"));
                            List<BrPviStation> stationList = GetStationsFromPviObj(((BrPviStation)brJob.Station).BrPviCpuName); //pviObj.Name
                            if (stationList != null)
                            {
                                if (conn == DriverErrorCodes.ErrorNoError && aj.pviObj.LastError == 0)
                                {
                                    foreach (BrPviStation station in stationList)
                                        station.ManageCreationEvent(aj.pviObj);
                                    // pviObjectsInitialized == true, kepp job alive, and continue with JobPviObjectInitialization

                                    // during Communication Test, once retrive station info --> connected with PLC was established, have to stop
                                    if (TestCommMode)
                                    {
                                        ExecutedJobArgs e = new ExecutedJobArgs();
                                        e.Job = brJob;
                                        e.Values = null;
                                        e.ErrorCode = conn; // (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError;
                                        OnJobExecuted(e);
                                    }
                                }
                                else
                                {
                                    if (conn != DriverErrorCodes.ErrorNoError)
                                    {
                                        aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                        aj.pviObj.LastError = (uint)conn;
                                    }

                                    foreach (BrPviStation station in stationList)
                                        station.ManagePviErrorEvent(aj.pviObj);
                                    brJob.ManagePviErrorEvent(aj.pviObj);
                                }
                            }
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobPviObjectInitialization:
                        {
                            // job not completed --> too many item to subscribe
                            if (!aj.Completed)
                            {
                                RemoveJobToPviObjDictionary(brJob.BrPviVariableCompleteName, brJob);
                                // don't process job (retry next scheduling)
                                RemovePendingJob(brJob);
                            }
                            else
                            {

                                //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ProcessNewData {0} - InitializeJobPviObject 1", DateTime.Now.ToString("HH:mm:ss.fff"));
                                if (conn == DriverErrorCodes.ErrorNoError && aj.pviObj.LastError == 0)
                                {
                                    List<BrPviCommJob> jobList = GetJobsFromPviObj(brJob.BrPviVariableCompleteName);
                                    if (jobList != null)
                                    {
                                        foreach (BrPviCommJob job in jobList)
                                        {
                                            //job.ManageCreationEvent(pviObj);

                                            if (brJob.pviObjectState != BrPviCommJob.PviObjectState.Activated) //false
                                                brJob.pviObjectState = BrPviCommJob.PviObjectState.Activated; //true

                                            //if (pviEvent.PviObj.IsNewDataEvent())
                                            //    PassReceivedDataToJob(pviEvent.PviObj.DataArray, job);
                                            // in the while some data arrived (IsPending = false); reevaluate job state
                                            job.CheckBrPviState();
                                        }
                                    }
                                }
                                else
                                {
                                    if (conn != DriverErrorCodes.ErrorNoError)
                                    {
                                        aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                        aj.pviObj.LastError = (uint)conn;
                                    }

                                    brJob.ManagePviErrorEvent(aj.pviObj);
                                    brJob.BrpviState = BrPviCommJob.BrPviState.JobUnMapped;
                                }
                            }
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobStringLengthRequest:
                        {
                            if (conn == DriverErrorCodes.ErrorNoError && aj.pviObj.LastError == 0)
                            {
                                List<BrPviCommJob> jobList = GetJobsFromPviObj(aj.pviObj.Name);
                                if (jobList != null)
                                {
                                    uint stringLength = ParseStringLength(aj.pviEvent.DataArray, aj.pviEvent.DataLength);
                                    foreach (BrPviCommJob job in jobList)
                                        job.SetStringLength(stringLength);
                                }
                                // evaluate next step
                                brJob.CheckBrPviState();
                            }
                            else
                            {
                                if (conn != DriverErrorCodes.ErrorNoError)
                                {
                                    aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                    aj.pviObj.LastError = (uint)conn;
                                }

                                brJob.ManagePviErrorEvent(aj.pviObj);
                            }
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobInitialized:
                        {
                            switch (brJob.CommandType)
                            {
                                case BrPviCommJob.CommandTypes.WriteCmd:
                                    if (conn == DriverErrorCodes.ErrorNoError && aj.pviObj.LastError == 0)
                                    {
                                        PassWriteOKNotificationToJob(brJob);
                                    }
                                    else
                                    {
                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Error preparing write request", DateTime.Now.ToString("HH:mm:ss.fff"));
                                        //BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                                        //brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorPreparingWriteRequest, brJob.BrPviVariableCompleteName);
                                        //ExecutedJobArgs e = new ExecutedJobArgs();
                                        //e.Job = brJob;
                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 3 - PVI Obj: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                        //e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodePrepareWriteRequest;
                                        //OnJobExecuted(e);

                                        if (conn != DriverErrorCodes.ErrorNoError)
                                        {
                                            aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                            aj.pviObj.LastError = (uint)conn;
                                        }

                                        brJob.ManagePviErrorEvent(aj.pviObj);
                                    }
                                    break;
                                case BrPviCommJob.CommandTypes.ReadCmd:
                                    // read value is always retrive from last Pvi Manager event
                                    aj.pviObj = GetPviObject(brJob.BrPviVariableCompleteName);
                                    if (aj.pviObj != null && conn == DriverErrorCodes.ErrorNoError && aj.pviObj.LastError == 0)
                                    {
                                        if (brJob.SyncroExec)
                                            PassReceivedDataToJob(brJob.GetLastReadValue(), brJob);
                                        else
                                            PassReceivedDataToJob(brJob.GetLastReadValue(), brJob);
                                    }
                                    else
                                    {
                                        if (aj.pviObj == null)
                                        {
                                            aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                            aj.pviObj.LastError = (uint)BrPviProcol.PVI_EVENT_IDENTIFICATION_ERROR;
                                        }
                                        else if (conn != DriverErrorCodes.ErrorNoError)
                                        {
                                            aj.pviObj = new BrPviPviObject(brJob.Station.Name);
                                            aj.pviObj.LastError = (uint)conn;
                                        }
                                        brJob.ManagePviErrorEvent(aj.pviObj);
                                    }
                                    break;
                            }
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobUnMapped:
                        {
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = brJob;
                            e.ErrorCode = conn; // (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError;
                            OnJobExecuted(e);
                        }
                        break;

                    case BrPviCommJob.BrPviState.JobUnSubScribe:
                        {
                            brJob.DeletePviObjects();

                            RemovePendingJob(brJob);
                            brJob.BrpviState = BrPviCommJob.BrPviState.None;
                        }
                        break;
                }

            }

            return true;
        }

        public void OnJobExecuted(ExecutedJobArgs e)
        {
            //    BrPviCommJob brJob = (BrPviCommJob)e.Job;
            //    //brJob.CommandType = BrPviCommJob.CommandTypes.Init;
            //    //if (brJob.IsSyncroJob)
            //    //    RemoveSynchroJob(brJob);
            base.OnJobExecuted(e);
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                lock (lockThreadObject)
                {
                    if (NewDataToAnlyze == null)
                        NewDataToAnlyze = new ManualResetEvent(false);
                    else
                        NewDataToAnlyze.Reset();
                }
            }

            return base.Startup();
        }

        public override void Suspend()
        {                        
            base.Suspend(); // contain DeviceClose();

            channelPviSink.Close();

            ResetJobsState();
        }

        public override void Dispose()
        {            
            base.Dispose();
            
            if (channelPviSink != null)
                channelPviSink.Close();
        }

        private DateTime lastRemoveDisabledItemAfterSecs = DateTime.MinValue;
        public override int ChannelScheduleProcedure(DateTime dtNow, out int nrRemainingJobsInQueues)
        {
            int nNextScheduleInterval = base.ChannelScheduleProcedure(dtNow, out nrRemainingJobsInQueues);

            //using the driver scheduler, managed the scheduling of the jobs that must be unsubscribed 
            uint removeDisabledItemAfterSecs = GetBrPviRemoveDisabledItemAfterSecs() * 1000;
            // if job's unsuscribe is enabled
            if (removeDisabledItemAfterSecs > 0)
            {
                int unSubScribeRequested = -1;
                if (lastRemoveDisabledItemAfterSecs > dtNow)
                    unSubScribeRequested = (int)lastRemoveDisabledItemAfterSecs.Subtract(dtNow).TotalMilliseconds;

                if (unSubScribeRequested <= 0)
                {
                    // get from a "separate" list the list of jobs to be process to schedule jobs to be unsubscribe from PVIManager
                    List<CommJob> startList = new List<CommJob>();
                    startList.AddRange(GetJobsToUnSubScribe());

                    List<CommJob> wList = null;
                    lock (lockListObject)
                    {
                        wList = (from job in startList
                                 where !job.IsPending && !job.IsQueued && !job.InErrorState
                                     && job.IsConditionalVariableOn()
                                 orderby ((BrPviCommJob)job).BrPviLastUnSubScribeExecutionTime ascending
                                 select job).ToList();
                        if (wList.Count > 0)
                        {
                            wList.ForEach(job =>
                            {
                                job.ScheduleQueue = CommJobState.PollingInUse;
                            });
                        }
                    }

                    if (wList.Count > 0)
                    {                        
                        RescheduleLastQueuedJobsList(CommJobState.PollingInUse, wList);
                        // custom split jobs to manage job unsubscrive to PVI manager
                        List<List<CommJob>> exList = new List<List<CommJob>>();
                        SplitInExecutionListsUnSubScribe(wList, ref exList);
                        if (exList.Count > 0)
                            AddScheduledListJobQueue(CommJobState.PollingInUse, exList, out nInUseMax);

                        if (wList.Count > 0)
                        {
                            wList.ForEach(job =>
                            {
                                job.ScheduleQueue = CommJobState.UnScheduled;
                            });                            
                        }
                        else
                        {
                            nInUseMax = GetScheduledListJobQueue(CommJobState.PollingInUse).Count();
                        }

                        unSubScribeRequested = (int)removeDisabledItemAfterSecs;
                        // some jobs (to unsubscribe) scheduled ? wait removeDisabledItemAfterSecs to process next
                        lastRemoveDisabledItemAfterSecs = dtNow.AddMilliseconds(removeDisabledItemAfterSecs);
                    }
                }

                //search lower delays
                if (unSubScribeRequested >= 0 && (nNextScheduleInterval == -1 || unSubScribeRequested < nNextScheduleInterval))
                {
                    nNextScheduleInterval = unSubScribeRequested;
                }

                nrRemainingJobsInQueues = NrRemainingJobsInQueues();

                nNextScheduleInterval = EvaluateMinScheduleInterval(nNextScheduleInterval, nrRemainingJobsInQueues);                
            }

            return nNextScheduleInterval;
        }                
        //#endregion

        #region Specific Methods

        public void ResetConnectionStatus()
        {
            checkStateCaller = BrPviProcol.DeviceStateCheckMode.Sync;
            lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.NotInitialized;
            lastPviManagerConnectionWait.Reset();
        }

        public void WaitConnectionResult()
        {
            // "try" to wait PVI manager connection's state event
            lastPviManagerConnectionWait.WaitOne(Timeout);

            if (lastPviManagerConnectionErrorCode != 0)
            {
                lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.Disconnected;
            }

            checkStateCaller = BrPviProcol.DeviceStateCheckMode.Async;
        }

        public void PviXInitialize()
        {
            lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXInitialize((int)_BrPviServerCommunicationTimeout, (int)_BrPviRetryTime, _BrPviServerAddress, (int)_BrPviServerPort);
        }

        public void SetPviGlobalEvents()
        {
            lastPviManagerConnectionErrorCode = pviCommunicationManager.SetPviGlobalEvents(intptrConnectCallback, intptrDisconnectCallback, intptrArrangeCallback, intptrChannel);
        }

        public void PviXDeinitialize()
        {
            lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXDeinitialize();
        }

        public void EmptyPviObjects()
        {
            lock (lockmapPviObjects)
                listOfPviObjectsToBeCreated.Clear();
        }

        public void CreatePviObjects()
        {
            List<BrPviPviObject> listOfPviObjectsToBeCreatedCopy = null;
            lock (lockmapPviObjects)
            {
                if (listOfPviObjectsToBeCreated.Count > 0)
                {
                    listOfPviObjectsToBeCreatedCopy = new List<BrPviPviObject>(listOfPviObjectsToBeCreated);
                    listOfPviObjectsToBeCreated.Clear();
                }
            }
            if (listOfPviObjectsToBeCreatedCopy != null)
            {
                foreach (var pviObj in listOfPviObjectsToBeCreatedCopy)
                {
                    BrPviPviObject pviObjToBeCreated = (BrPviPviObject)pviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObjToBeCreated, out handleOfPviObj);
                    lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviObjToBeCreated, GCHandle.ToIntPtr(handleOfPviObj));
                    if (lastPviManagerConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObjToBeCreated.LastError = (uint)lastPviManagerConnectionErrorCode;
                        pviObjToBeCreated.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildCreateErrorEvent(pviObjToBeCreated);
                        ManagePviEvent(errorEvent);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.CreatePviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2} - LinkDescr:{3}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObjToBeCreated.Name, lastPviManagerConnectionErrorCode, pviObj.LinkDescr));
                }
            }
        }

        public void WritePviObjects()
        {
            List<PviWriteObj> listOfPviWriteRequestsCopy = null;
            lock (lockmapPviObjects)
            {
                if (listOfPviWriteRequests.Count > 0)
                {
                    listOfPviWriteRequestsCopy = new List<PviWriteObj>(listOfPviWriteRequests);
                    listOfPviWriteRequests.Clear();
                }
            }
            if (listOfPviWriteRequestsCopy != null)
            {
                foreach (var pviWriteObj in listOfPviWriteRequestsCopy)
                {
                    PviWriteObj pviObjWriteRequest = (PviWriteObj)pviWriteObj;
                    BrPviPviObject pviObj = pviObjWriteRequest.PviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObj, out handleOfPviObj);
                    lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXWriteRequest(intptrDataCallback, pviObjWriteRequest, GCHandle.ToIntPtr(handleOfPviObj), PviComManager.POBJ_ACC_DATA);
                    if (lastPviManagerConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObj.LastError = (uint)lastPviManagerConnectionErrorCode;
                        pviObj.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildWriteErrorEvent(pviObj);
                        ManagePviEvent(errorEvent);
                    }
                    else
                    {
                        // Update value of the PVI Object
                        pviObj.UpdateValue(pviObjWriteRequest.DataArray, pviObjWriteRequest.DataLength);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.WritePviObjects {0} - return code of PviComManager.PviXWriteRequest for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lastPviManagerConnectionErrorCode));
                }
            }
        }

        //Activate/Deactivate item on PVI Monitor (do not remove items)
        public void WriteMaskPviObjects()
        {
            List<PviWriteObj> listOfPviMaskWriteRequestsCopy = null;
            lock (lockmapPviObjects)
            {
                if (listOfPviMaskWriteRequests.Count > 0)
                {
                    listOfPviMaskWriteRequestsCopy = new List<PviWriteObj>(listOfPviMaskWriteRequests);
                    listOfPviMaskWriteRequests.Clear();
                }
            }
            if (listOfPviMaskWriteRequestsCopy != null)
            {
                foreach (var pviWriteObj in listOfPviMaskWriteRequestsCopy)
                {
                    PviWriteObj pviObjWriteRequest = (PviWriteObj)pviWriteObj;
                    BrPviPviObject pviObj = pviObjWriteRequest.PviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObj, out handleOfPviObj);
                    lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXWriteRequest(intptrDataCallback, pviObjWriteRequest, GCHandle.ToIntPtr(handleOfPviObj), PviComManager.POBJ_ACC_EVMASK);
                    if (lastPviManagerConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObj.LastError = (uint)lastPviManagerConnectionErrorCode;
                        pviObj.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildWriteErrorEvent(pviObj);
                        ManagePviEvent(errorEvent);
                    }
                    //else
                    //{
                    //    // Update value of the PVI Object
                    //    pviObj.UpdateValue(pviObjWriteRequest.DataArray, pviObjWriteRequest.DataLength);
                    //}
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.WriteMaskPviObjects {0} - return code of PviComManager.WriteMaskPviObjects for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lastPviManagerConnectionErrorCode));
                }
            }
        }

        public void SendStringLengthRequests()
        {
            List<BrPviPviObject> listOfStringLengthRequestsCopy = null;
            lock (lockmapPviObjects)
            {
                if (listOfStringLengthRequests.Count > 0)
                {
                    listOfStringLengthRequestsCopy = new List<BrPviPviObject>(listOfStringLengthRequests);
                    listOfStringLengthRequests.Clear();
                }
            }
            if (listOfStringLengthRequestsCopy != null)
            {
                foreach (var pviObj in listOfStringLengthRequestsCopy)
                {
                    BrPviPviObject pviObject = (BrPviPviObject)pviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObject, out handleOfPviObj);
                    lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviObject, PviComManager.POBJ_ACC_TYPE_EXTERN, GCHandle.ToIntPtr(handleOfPviObj));
                    if (lastPviManagerConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObject.LastError = (uint)lastPviManagerConnectionErrorCode;
                        PviObjEvent errorEvent = BuildReadErrorEvent(pviObject);
                        ManagePviEvent(errorEvent);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.SendStringLengthRequests {0} - return code of PviComManager.PviXReadRequest for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObject.Name, lastPviManagerConnectionErrorCode));
                }
            }
        }

        public void SendDeleteRequests()
        {
            List<BrPviPviObject> listOfDeleteRequestsCopy = null;
            lock (lockmapPviObjects)
            {
                if (listOfDeleteRequests.Count > 0)
                {
                    listOfDeleteRequestsCopy = new List<BrPviPviObject>(listOfDeleteRequests);
                    listOfDeleteRequests.Clear();
                }
            }
            if (listOfDeleteRequestsCopy != null)
            {
                foreach (var pviObj in listOfDeleteRequestsCopy)
                {
                    BrPviPviObject pviObject = (BrPviPviObject)pviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObject, out handleOfPviObj);
                    lastPviManagerConnectionErrorCode = pviCommunicationManager.PviXDeleteRequest(intptrDataCallback, pviObject, GCHandle.ToIntPtr(handleOfPviObj));
                    if (lastPviManagerConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObject.LastError = (uint)lastPviManagerConnectionErrorCode;
                        PviObjEvent errorEvent = BuildReadErrorEvent(pviObject);
                        ManagePviEvent(errorEvent);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.SendDeleteRequests {0} - return code of PviComManager.PviXReadRequest for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObject.Name, lastPviManagerConnectionErrorCode));
                }
            }
        }

        PviWriteObj PrepareWriteRequest(BrPviCommJob brJob)
        {
            PviWriteObj returnedObj = null;
            BrPviPviObject pviVarObject = GetPviObject(brJob.BrPviVariableCompleteName);
            if (pviVarObject != null)
            {
                BrPviTag tagToWrite = null;
                var listOnWriting = new List<Tag>();
                lock (brJob.retLockList())
                {
                    if (brJob.TagsListOnWriting.Count == 0)
                    {
                        return(returnedObj);
                    }
                    tagToWrite = (BrPviTag)brJob.TagsListOnWriting[0];
                    brJob.ClearTagListOnWriting();
                }

                listOnWriting.Add(tagToWrite);
                byte[] jobdata;
                uint nData = 0;
                lock (brJob.retLockList())
                {                    
                    nData = tagToWrite.Size;
                    jobdata = new byte[nData];
                    tagToWrite.GetTagBuffer(ref jobdata, false, 0);
                    if (!brJob.isProtocolBool())
                    {
                        if (brJob.SwapBytes)
                        {
                            CommJob.SwapByteBuffer(ref jobdata);
                        }

                        if (brJob.SwapWords)
                        {
                            CommJob.SwapWordBuffer(ref jobdata);
                        }
                    }
                }

                returnedObj = new PviWriteObj(pviVarObject);
                if (returnedObj.SetData(jobdata, nData))
                {
                    lock (brJob.retLockList())
                    {
                        brJob.TagsListOnWriting.Clear();
                        brJob.TagsListOnWriting.AddRange(listOnWriting);
                        listOnWriting.ForEach((tag) =>
                        {
                            brJob.TagsListToWrite.Remove(tag);
                        });
                    }
                    listOnWriting.Clear();
                }
                else
                {
                    lock (brJob.retLockList())
                    {
                        brJob.TagsListOnWriting.Clear();
                        listOnWriting.ForEach((tag) =>
                        {
                            if (!brJob.TagsListToWrite.Contains(tag))
                                brJob.TagsListToWrite.Add(tag);
                        });
                    }
                    listOnWriting.Clear();
                    returnedObj = null;
                }
            }

            return (returnedObj);
        }

        public PviObjEvent BuildCreateErrorEvent(BrPviPviObject pviObjToBeCreated)
        {
            PviEventInfo eventInfo = new PviEventInfo();
            eventInfo.nMode = (uint)PviObjectModes.POBJ_MODE_CREATE;
            eventInfo.nType = BrPviProcol.POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObjToBeCreated.LastError;
            return (new PviObjEvent(eventInfo, pviObjToBeCreated));
        }

        public PviObjEvent BuildWriteErrorEvent(BrPviPviObject pviObjToBeWritten)
        {
            PviEventInfo eventInfo = new PviEventInfo();
            eventInfo.nMode = (uint)PviObjectModes.POBJ_MODE_WRITE;
            eventInfo.nType = BrPviProcol.POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObjToBeWritten.LastError;
            return (new PviObjEvent(eventInfo, pviObjToBeWritten));
        }

        public PviObjEvent BuildReadErrorEvent(BrPviPviObject pviObj)
        {
            PviEventInfo eventInfo = new PviEventInfo();
            eventInfo.nMode = (uint)PviObjectModes.POBJ_MODE_READ;
            eventInfo.nType = BrPviProcol.POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObj.LastError;
            return (new PviObjEvent(eventInfo, pviObj));
        }

        void ManageConnectionBroken(List<CommJob> list = null)
        {
            lock (lockStream)
            {
                pviManagerDeviceCloseRequest = true;
                //DeviceClose();
                BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorConnectionBroken, lastPviManagerConnectionErrorCode);
                // Set the error state of stations and jobs
                foreach (var station in CommDriver.GetChannelStations(this))
                    ((BrPviStation)station).ManageConnectionBroken(list);
            }
        }

        void ResetJobsState()
        {
            lock (lockStream)
            {
                foreach (var station in CommDriver.GetChannelStations(this))
                    ((BrPviStation)station).ResetJobsState();
            }
        }

        void ResetListAndMaps()
        {
            lock (lockmapPviObjects)
            {
                // Reset the status of all PVI objects
                foreach (KeyValuePair<string, BrPviPviObject> item in mapPviObjects)
                {
                    if (item.Value != null)
                    {
                        item.Value.LinkID = 0;
                        item.Value.Status = PviObjectStatus.MustBeCreated;
                    }
                }

                // Empty the list of write requests
                listOfPviWriteRequests.Clear();
                // Empty the dictionary of pending write requests
                //mapPendingWriteRequests.Clear();
                // Empty the list of string length requests
                listOfStringLengthRequests.Clear();
                // Empty the dictionary of pending string length requests
                //mapPendingStringLengthRequests.Clear();
                // Empty the list of objects to be created
                listOfPviObjectsToBeCreated.Clear();
                // Empty the dictionary of jobs associated to pvi object
                mapPviObjJobs.Clear();
                // Empty the dictionary of pending delete requests
                listOfDeleteRequests.Clear();
                // Empty list of jobs to be unsubscribe from PVIManager
                mapJobsToUnSubScribe.Clear();
            }

            listOfStringLengthRequests = new List<BrPviPviObject>();

            lock (activeJobsLock)
            {
                activeJobs.Clear();
                activeJobsFilling = false;
            }

            // Deallocate the handles for events
            DeallocateGlobalHandles();

            lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.NotInitialized;

            pviEventsObj = null;
        }


        /// <summary>
        /// Manage connect/disconnect events (strate event) generated by PVI callback
        /// </summary>
        /// <param name="info"></param>
        public void ManageGlobalEvents(PviEventInfo eventInfo)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManageGlobalEvents {0} - Received event {1} with Error Code {2}",
                                                DateTime.Now.ToString("HH:mm:ss.fff"), eventInfo.nType, eventInfo.ErrCode));
            switch (eventInfo.nType)
            {
                case PviComManager.POBJ_EVENT_PVI_CONNECT:
                    if (eventInfo.ErrCode == 0) // Connection OK
                    {
                        lastPviManagerConnectionErrorCode = 0;
                        if (lastPviManagerConnectionStatus != BrPviProcol.BrPviConnectionStatus.Arranged)
                            lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.Connected;
                    }
                    else // Connection error
                    {
                        lastPviManagerConnectionErrorCode = (int)eventInfo.ErrCode;
                        pviManagerDeviceCloseRequest = true;
                    }
                    break;
                case PviComManager.POBJ_EVENT_PVI_DISCONN:
                    lastPviManagerConnectionErrorCode = (int)eventInfo.ErrCode;
                    lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.Disconnected;
                    pviManagerDeviceCloseRequest = true;

                    switch (checkStateCaller)
                    {
                        // during DeviceOpen, error is manage by DeviceOpen code inside (sync)
                        case BrPviProcol.DeviceStateCheckMode.Sync:
                            lastPviManagerConnectionWait.Set();
                            break;
                        // during Driver normal operations (not DeviceOpen), 2 different situation
                        case BrPviProcol.DeviceStateCheckMode.Async:
                            // if any job is pending, manage it sync in ProcessNewData
                            if (AnyActiveJobs())
                            {
                                UnSetAllActiveJob(null, null, DriverErrorCodes.ErrorDeviceOpenFailed);
                                SetNewDataEvent();                                
                            }
                            else
                            {
                                // force scheduler to run
                                RestartScheduler();
                            }
                            break;
                    }                    

                    break;
                case PviComManager.POBJ_EVENT_PVI_ARRANGE:
                    if (eventInfo.ErrCode == 0) // Connection OK
                    {
                        lastPviManagerConnectionErrorCode = 0;
                        lastPviManagerConnectionStatus = BrPviProcol.BrPviConnectionStatus.Arranged;
                    }
                    else // Connection error
                    {
                        lastPviManagerConnectionErrorCode = (int)eventInfo.ErrCode;
                        pviManagerDeviceCloseRequest = true;
                    }
                    lastPviManagerConnectionWait.Set();
                    break;
            }            
        }

        /// <summary>
        /// Manage tag/variable event events (susbscribe/unsubscribre, read/write result) generated by PVI callback and unblock WaitDataEvent
        /// </summary>
        /// <param name="info"></param>
        public void ManagePviEvent(PviObjEvent pviEvent)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode));              
                // discard "error" message from subscribe
                if (pviEvent.EventInfo.ErrCode == 12040)
                    return;

                BrPviPviObject pviObj = GetPviObject(pviEvent.PviObj.Name);
                if (pviObj != null)
                {
                    bool matchToActiveJob = false;
                    BrPviCommJob activeJob = null;
                    List<BrPviStation> stationList = null;
                    List<BrPviCommJob> jobList = null;

                    switch (pviEvent.EventInfo.nMode)
                    {
                        case (uint)PviObjectModes.POBJ_MODE_CREATE:
                            #region POBJ_MODE_CREATE
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("PviObjectModes.POBJ_MODE_CREATE {0} Item:{1}, LinkID:{2}, ErrorCode:{3}", DateTime.Now, pviObj.Name, pviEvent.EventInfo.LinkID, pviEvent.EventInfo.ErrCode));                                
                                if (pviEvent.EventInfo.ErrCode != 0)
                                {
                                    pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                    pviObj.Status = PviObjectStatus.NotCreated;
                                }
                                // PVI object successfully created
                                else
                                {
                                    pviObj.LastError = 0;
                                    pviObj.LinkID = pviEvent.EventInfo.LinkID;
                                    if (pviObj.Status != PviObjectStatus.DataReceived)
                                        pviObj.Status = PviObjectStatus.Ready;
                                }
                            }
                            break;
                            #endregion

                        case (uint)PviObjectModes.POBJ_MODE_EVENT:  // handle data change (read) and error                            
                            #region POBJ_MODE_EVENT
                            {                                
                                // check error state 
                                bool error = ((pviEvent.EventInfo.nType == BrPviProcol.POBJ_EVENT_ERROR && pviEvent.EventInfo.ErrCode != 0) || pviEvent.EventInfo.ErrCode != 0);                                
                                
                                if (pviObj.IsStationObject())
                                {
                                    stationList = GetStationsFromPviObj(pviObj.Name);
                                    if (stationList != null)
                                    {
                                        if (AnyActiveJobs())
                                        {
                                            activeJob = (BrPviCommJob)GetActiveJobOrFirst();
                                            matchToActiveJob = stationList.Contains(activeJob.Station);
                                        }
                                    }
                                }
                                else
                                {
                                    jobList = GetJobsFromPviObj(pviObj.Name);
                                    if (jobList != null)
                                    {
                                        if (AnyActiveJobs())
                                        {
                                            activeJob = (BrPviCommJob)GetActiveJobOrFirst(pviObj.Name);
                                            matchToActiveJob = jobList.Contains(activeJob);
                                        }
                                    }
                                }

                                if (!error)
                                {
                                    pviObj.LastError = 0;
                                    // manage data recive event
                                    if (pviEvent.PviObj.IsNewDataEvent())
                                    {
                                        if (pviObj.IsStationObject())
                                        {
                                            pviObj.Status = PviObjectStatus.Ready;
                                            if (stationList != null)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.DataStation {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);

                                                foreach (BrPviStation station in stationList)
                                                    station.ManageDataReceivedEvent(pviEvent);
                                            }
                                        }
                                        else
                                        {
                                            if (jobList != null)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.DataJobs {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);
                                                foreach (BrPviCommJob job in jobList)
                                                {
                                                    if (job.BrpviState == BrPviCommJob.BrPviState.JobPviObjectInitialization)
                                                    {
                                                        if (matchToActiveJob)
                                                        {
                                                            if (job == activeJob)
                                                            {
                                                                //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.StationPviObjectInitialization Out {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);
                                                                UnSetActiveJob(job, pviEvent, pviObj);
                                                                if (IsAllActiveJobsCompleted())
                                                                    SetNewDataEvent();
                                                                continue;
                                                            }                                                            
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PassReceivedDataToJob(pviEvent.PviObj.DataArray, job);
                                                    }
                                                }
                                            }
                                        }
                                    } 
                                    else
                                    {
                                        // manage object creation event
                                        if (matchToActiveJob)
                                        {
                                            lock (activeJobsLock)
                                            {
                                                switch (activeJob.BrpviState)
                                                {                                                    
                                                    case BrPviCommJob.BrPviState.StationPviObjectInitialization:
                                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.StationPviObjectInitialization In {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);
                                                        if (pviObj.IsCpuMessage() && ((BrPviStation)activeJob.Station).BrPviCpuName == pviEvent.PviObj.Name)
                                                        {
                                                            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.StationPviObjectInitialization Out {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);
                                                            UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                            if (IsAllActiveJobsCompleted())
                                                                SetNewDataEvent();
                                                        }
                                                        break;
                                                    case BrPviCommJob.BrPviState.JobPviObjectInitialization:
                                                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents.JobPviObjectInitialization In {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode);
                                                        if (pviObj.IsVarObjectOnly() && activeJob.BrPviVariableCompleteName == pviEvent.PviObj.Name)
                                                        {
                                                            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents.JobPviObjectInitialization In {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString(), pviEvent.EventInfo.ErrCode));
                                                            UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                            if (IsAllActiveJobsCompleted())
                                                                SetNewDataEvent();                                                            
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (error)
                                {
                                    //bool proceed = true;
                                    //// A link to a process object made using a link object was broken. The following causes are possible for the interruption: 1) PVI Manager was shut down. 2) The process object was deleted (function PviDelete). 3) The trial time has passed.
                                    //if (pviEvent.EventInfo.ErrCode == 12040)
                                    //{
                                    //    // confirmation
                                    //    if (jobList != null && jobList.Count > 0 && jobList[0].UnSubScribeRequested == BrPviCommJob.UnSubScribeState.Performed)
                                    //    {                                            
                                    //        jobList[0].UnSubScribeRequested = BrPviCommJob.UnSubScribeState.None;

                                    //        if (AnyActiveJobs())
                                    //        {
                                    //            if (GetActiveJobOrFirst(jobList[0].BrPviVariableCompleteName) != null)
                                    //            {
                                    //                UnSetActiveJob(activeJob, pviEvent, pviObj);
                                    //                if (IsAllActiveJobsCompleted())
                                    //                    SetNewDataEvent();
                                    //            }
                                    //        }

                                    //        proceed = false;
                                    //        System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} 12040 confirm delete", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, ((PviObjectModes)pviEvent.EventInfo.nMode).ToString(), ((PviObjectTypes)pviEvent.EventInfo.nType).ToString()));
                                    //    }
                                    //}

                                    //if (proceed)
                                    //{
                                        pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                        // error not related to current job
                                        if (!matchToActiveJob)
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            if (pviObj.IsStationObject())
                                            {
                                                foreach (BrPviStation station in stationList)
                                                    station.ManagePviEvent(pviObj, true);
                                            }
                                            else
                                            {
                                                foreach (BrPviCommJob job in jobList)
                                                    job.ManagePviErrorEvent(pviObj, true);
                                            }
                                        }
                                        else
                                        {
                                            lock (activeJobsLock)
                                            {
                                                switch (activeJob.BrpviState)
                                                {
                                                    //pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                                    case BrPviCommJob.BrPviState.StationPviObjectInitialization:
                                                        if (pviObj.IsCpuMessage() && ((BrPviStation)activeJob.Station).BrPviCpuName == pviEvent.PviObj.Name)
                                                        {
                                                            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents {0} Err {1} - StationPviObjectInitialization", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, pviObj.LastError));
                                                            UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                            if (IsAllActiveJobsCompleted())
                                                                SetNewDataEvent();
                                                        }
                                                        break;
                                                    case BrPviCommJob.BrPviState.JobPviObjectInitialization:
                                                        if (pviObj.IsVarObjectOnly() && activeJob.BrPviVariableCompleteName == pviEvent.PviObj.Name)
                                                        {
                                                            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents {0} Err {1} - JobPviObjectInitialization", DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, pviObj.LastError));
                                                            //if (error == 12040)
                                                            UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                            if (IsAllActiveJobsCompleted())
                                                                SetNewDataEvent();
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    //}
                                }                            
                            }
                            break;
                            #endregion

                        case (uint)PviObjectModes.POBJ_MODE_READ:   // handle stringh data length request
                            #region POBJ_MODE_READ
                            {                                
                                if (AnyActiveJobs())
                                {
                                    jobList = GetJobsFromPviObj(pviObj.Name);
                                    if (jobList != null)
                                    {
                                        activeJob = (BrPviCommJob)GetActiveJobOrFirst(pviObj.Name);
                                        matchToActiveJob = jobList.Contains(activeJob);
                                    }
                                }

                                if (matchToActiveJob)
                                {
                                    // Error                                    
                                    if ((pviEvent.EventInfo.nType == BrPviProcol.POBJ_EVENT_ERROR && pviEvent.EventInfo.ErrCode != 0) || pviEvent.EventInfo.ErrCode != 0)
                                    {
                                        pviObj.LastError = pviEvent.EventInfo.ErrCode;                                            
                                    }// Data received
                                    else if ((pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_TYPE_EXTERN) && (pviEvent.PviObj.Status == PviObjectStatus.DataInfoReceived))
                                    {
                                        pviObj.LastError = 0;
                                        pviObj.Status = PviObjectStatus.Ready;
                                        if (pviObj.Type != PviObjectTypes.POBJ_PVAR)
                                            matchToActiveJob = false;
                                    }

                                    if (matchToActiveJob)
                                    {
                                        switch (activeJob.BrpviState)
                                        {
                                            case BrPviCommJob.BrPviState.JobStringLengthRequest:
                                                UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                if (IsAllActiveJobsCompleted())
                                                    SetNewDataEvent();
                                                return;
                                        }                                        
                                    }
                                }
                            }
                            break;                            
                            #endregion

                        case (uint)PviObjectModes.POBJ_MODE_WRITE:  // handle data write
                            #region POBJ_MODE_WRITE
                            {
                                if (AnyActiveJobs())
                                {
                                    jobList = GetJobsFromPviObj(pviObj.Name);
                                    if (jobList != null)
                                    {
                                        activeJob = (BrPviCommJob)GetActiveJobOrFirst(pviObj.Name);
                                        matchToActiveJob = jobList.Contains(activeJob);
                                    }
                                }

                                if (matchToActiveJob)
                                {
                                    // Error
                                    if ((pviEvent.EventInfo.nType == BrPviProcol.POBJ_EVENT_ERROR && pviEvent.EventInfo.ErrCode != 0) || pviEvent.EventInfo.ErrCode != 0)
                                    {
                                        pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                        switch (activeJob.BrpviState)
                                        {
                                            case BrPviCommJob.BrPviState.JobInitialized:
                                                UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                SetNewDataEvent();
                                                return;
                                        }
                                    }
                                    // Success reply received
                                    else if (pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_DATA)
                                    {
                                        pviObj.LastError = 0;
                                        if (pviObj.Status != PviObjectStatus.DataReceived)
                                            pviObj.Status = PviObjectStatus.Ready;

                                        lock (activeJobsLock)
                                        {
                                            switch (activeJob.BrpviState)
                                            {
                                                case BrPviCommJob.BrPviState.JobInitialized:
                                                    //PassWriteOKNotificationToJob(activeJob);
                                                    UnSetActiveJob(activeJob, pviEvent, pviObj);
                                                    SetNewDataEvent();
                                                    return;
                                            }
                                        }
                                    }
                                    else if (pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_EVMASK)
                                    {
                                        pviObj.LastError = 0;
                                        if (pviObj.Status != PviObjectStatus.DataReceived)
                                            pviObj.Status = PviObjectStatus.Ready;
                                    }
                                }
                            }
                            break;
                            #endregion
                        
                        case (uint)PviObjectModes.POBJ_MODE_DELETE: // used to remove not ia use tag from PviServer
                            jobList = GetJobsFromPviObj(pviObj.Name);
                            if (jobList != null)
                            {
                                if (AnyActiveJobs())
                                {
                                    activeJob = (BrPviCommJob)GetActiveJobOrFirst(pviObj.Name);

                                    if (jobList.Contains(activeJob))
                                    {
                                        UnSetActiveJob(activeJob, pviEvent, pviObj);
                                        if (IsAllActiveJobsCompleted())
                                            SetNewDataEvent();
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.ManagePviEvents {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message));
                if(lastPviManagerConnectionErrorCode == 0)
                {
                    CommDriver.OnSystemEvent(null,
                                             String.Format(Properties.Resources.BrExceptionInManagePviEvent, ex.Message, Name),
                                             Opc.Ua.EventSeverity.Max);
                    lastPviManagerConnectionErrorCode = -1;
                }
            }
        }

        uint ParseStringLength(byte[] dataArray, uint dataLength)
        {
            uint stringLength = 256;
            try
            {                

                if(dataLength > 1)
                {
                    String infoString = ASCIIEncoding.ASCII.GetString(dataArray);
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - ParseStringLength {0} - infoString: {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), infoString.ToString()));
                    StringComparison scomp = StringComparison.OrdinalIgnoreCase;
                    // Is the PVI variable a string or a wstring?
                    if((infoString.IndexOf("VT=string", scomp) >= 0) || (infoString.IndexOf("VT=wstring", scomp) >= 0))
                    {
                        // ù the info string for isolating the single parameters
                        char[] splitParameters = new char[1];
                        splitParameters[0] = ' ';
                        string[] pviVarInfo = infoString.Split(splitParameters);
                        foreach(string singleParameter in pviVarInfo)
                        {
                            // Search for the definiton of the string length
                            int searchIndex = singleParameter.IndexOf("VL=", scomp);
                            if (searchIndex >= 0)
                            {
                                string lengthString = singleParameter.Substring(3);
                                if(!String.IsNullOrWhiteSpace(lengthString))
                                {
                                    // Parse the string length
                                    stringLength = (uint)Convert.ToInt32(lengthString);
                                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - ParseStringLength {0} - parsed string length: {1}",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), stringLength));
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stringLength = 256;
                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - ParseStringLength {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message));
            }
            return (stringLength);
        }

        void PassReceivedDataToJob(byte[] dataArray, BrPviCommJob brJob)
        {
            //// was job subscribed ?
            //if (!brJob.IsBrPviJobState())
            //    return;

            if (brJob.Type == LinkType.ExceptionOutput || brJob.Type == LinkType.UnconditionalOutput)
                return;

            if (brJob.pviObjectState != BrPviCommJob.PviObjectState.Activated) //false
                brJob.pviObjectState = BrPviCommJob.PviObjectState.Activated; //true

            try
            {                
                brJob.IsRead = brJob.ReadRequest();
                brJob.ExchangedByte = (uint)dataArray.Count();
                brJob.ExchangedTag = 1;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = brJob;
                eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;

                if ((brJob.Type != LinkType.Input) && (brJob.Type != LinkType.InputOutput))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Called for PVI Tag {1} in output mode", DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Called for PVI Tag {1} in input mode",DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName));
                    
                    eJob.Values = dataArray;                    
                }
                OnJobExecuted(eJob);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message));
            }
        }

        void PassWriteOKNotificationToJob(BrPviCommJob brJob)
        {
            if(brJob.pviObjectState != BrPviCommJob.PviObjectState.Activated) //false
            {
                brJob.pviObjectState = BrPviCommJob.PviObjectState.Activated; //true
            }
            if (brJob.Type == LinkType.Input)
            {
                return;
            }

            try
            {                

                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassWriteOKNotificationToJob {0} - Called for: {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName));
                brJob.IsRead = false;
                brJob.ExchangedByte = brJob.GetExchangedBytesNumber();
                brJob.ExchangedTag = 1;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = brJob;
                eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassWriteOKNotificationToJob {0} - Calling OnJobExecuted", DateTime.Now.ToString("HH:mm:ss.fff")));
                OnJobExecuted(eJob);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassWriteOKNotificationToJob {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message));
                if(lastPviManagerConnectionErrorCode == 0)
                {
                    CommDriver.OnSystemEvent(null,
                                             String.Format(Properties.Resources.BrExceptionInPassWriteOKNotificationToJob,
                                                           ex.Message,
                                                           brJob.BrPviVariableCompleteName,
                                                           Name),
                                             Opc.Ua.EventSeverity.Max);
                    lastPviManagerConnectionErrorCode = -1;
                }
            }
        }

        void DeallocateGlobalHandles()
        {
            if (gcHandleConnectCallbackFunc.IsAllocated)
            {
                gcHandleConnectCallbackFunc.Free();
            }
            if (gcHandleDisconnectCallbackFunc.IsAllocated)
            {
                gcHandleDisconnectCallbackFunc.Free();
            }
            if (gcHandleArrangeCallbackFunc.IsAllocated)
            {
                gcHandleArrangeCallbackFunc.Free();
            }
            if (gcHandleChannel.IsAllocated)
            {
                gcHandleChannel.Free();
            }
            EmptyGCHandleDictionary();
        }

        public void SetPviObjectEvMask(String pviobjName, String evMask)
        {
            BrPviPviObject requestedPviObject;
            lock (lockmapPviObjects)
            {
                if (mapPviObjects.TryGetValue(pviobjName, out requestedPviObject))
                {                    
                    requestedPviObject = mapPviObjects[pviobjName];
                    requestedPviObject.EvMask = evMask;
                    mapPviObjects[pviobjName] = requestedPviObject;
                }
            }
        }

        public BrPviPviObject GetOrCreatePviObject(String stationName, String pviobjName, String pviobjConnDescr, String pviobjLinkDescr, PviObjectTypes pviobjType)
        {
            BrPviPviObject requestedPviObject;
            lock (lockmapPviObjects)
            {
                if(!mapPviObjects.TryGetValue(pviobjName, out requestedPviObject))
                {
                    requestedPviObject = new BrPviPviObject(stationName);
                    requestedPviObject.Name = pviobjName;
                    requestedPviObject.Type = pviobjType;
                    requestedPviObject.ConnDescr = pviobjConnDescr;
                    requestedPviObject.LinkDescr = pviobjLinkDescr;
                    mapPviObjects[pviobjName] = requestedPviObject;
                }
            }
            if((requestedPviObject.Status == PviObjectStatus.MustBeCreated) || (requestedPviObject.Status == PviObjectStatus.NotCreated))
            {
                requestedPviObject.Status = PviObjectStatus.CreationPending;
                AddPviObjectToCreateList(requestedPviObject);
            }
            return (requestedPviObject);
        }
        
        public BrPviPviObject GetPviObject(String pviObjName)
        {
            BrPviPviObject requestedPviObject = null;
            lock (lockmapPviObjects)
            {
                if (!mapPviObjects.TryGetValue(pviObjName, out requestedPviObject))
                {
                    requestedPviObject = null;
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.GetPviObject {0} - Pvi Obj {1} not found in the general dictionary",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObjName));
                }
            }
            return (requestedPviObject);
        }

        void GetOrAddGCHandleToDictionary(BrPviPviObject pviObj, out GCHandle handle)
        {
            lock (lockmapPviObjects)
            {
                if (!mapGlobalHandles.TryGetValue(pviObj.Name, out handle))
                {
                    handle = GCHandle.Alloc(pviObj);
                    mapGlobalHandles[pviObj.Name] = handle;
                }             
            }
        }

        void EmptyGCHandleDictionary()
        {
            lock (lockmapPviObjects)
            {
                Dictionary<string, GCHandle>.ValueCollection valueColl = mapGlobalHandles.Values;
                foreach (GCHandle handle in valueColl)
                {
                    if(handle.IsAllocated)
                    {
                        handle.Free();
                    }
                }
                mapGlobalHandles.Clear();
            }
        }

        void AddPviObjectToCreateList(BrPviPviObject pviObject)
        {
            lock (lockmapPviObjects)
            {
                if (!listOfPviObjectsToBeCreated.Contains(pviObject))
                {
                    listOfPviObjectsToBeCreated.Add(pviObject);
                }                
            }
        }

        void AddRequestToWriteList(PviWriteObj pviObjectToBeWritten)
        {
            lock (lockmapPviObjects)
            {
                listOfPviWriteRequests.Clear();
                listOfPviWriteRequests.Add(pviObjectToBeWritten);
            }
        }

        void AddRequestToWriteMaskList(PviWriteObj pviObjectToBeWritten)
        {
            lock (lockmapPviObjects)
            {
                if (!listOfPviMaskWriteRequests.Contains(pviObjectToBeWritten))
                    listOfPviMaskWriteRequests.Add(pviObjectToBeWritten);
            }
        }

        void AddRequestToReadStringLengthList(BrPviPviObject pviObject)
        {
            lock (lockmapPviObjects)
            {
                listOfStringLengthRequests.Clear();
                listOfStringLengthRequests.Add(pviObject);                
            }
        }

        void AddRequestToDeleteList(BrPviPviObject pviObject)
        {
            lock (lockmapPviObjects)
            {
                listOfDeleteRequests.Clear();
                listOfDeleteRequests.Add(pviObject);
            }
        }


        public void AddStationToPviObjDictionary(String pviobjName, BrPviStation brStation)
        {
            lock (lockmapPviObjects)
            {
                List<BrPviStation> stationList;
                if (!mapPviObjStations.TryGetValue(pviobjName, out stationList))
                {
                    stationList = new List<BrPviStation>();
                    mapPviObjStations[pviobjName] = stationList;
                }
                if(!mapPviObjStations[pviobjName].Contains(brStation))
                {
                    mapPviObjStations[pviobjName].Add(brStation);
                }
            }
        }
                
        public void AddJobToPviObjDictionary(String pviobjName, BrPviCommJob brJob)
        {
            lock (lockmapPviObjects)
            {
                if (!mapPviObjJobs.TryGetValue(pviobjName, out List<BrPviCommJob> jobList))
                {
                    jobList = new List<BrPviCommJob>();
                    mapPviObjJobs[pviobjName] = jobList;
                }

                if (!mapPviObjJobs[pviobjName].Contains(brJob))
                {
                    mapPviObjJobs[pviobjName].Add(brJob);
                }
            }
        }

        public void RemoveJobToPviObjDictionary(String pviobjName, BrPviCommJob brJob)
        {
            lock (lockmapPviObjects)
            {
                if (mapPviObjJobs[pviobjName].Contains(brJob))
                {
                    mapPviObjJobs[pviobjName].Remove(brJob);
                }                
            }
        }

        protected override bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            bool bRet = false;
            List<Station> stBrPvi = (from station in CommDriver.GetChannelStations(this).AsParallel() where (station.Name == exjob.Station.Name) select station).ToList();
            if (stBrPvi.Count > 0)
            {
                if (base.SetSynchroJobData(exjob, tagNodeId, value))
                {                                
                    BrPviCommJob job = exjob as BrPviCommJob;                    
                    ((BrPviStation)stBrPvi[0]).BuildJobPviObjNames(job);
                    bRet = true;
                }
            }
            return bRet;
        }
        
        List<BrPviStation> GetStationsFromPviObj(string key)
        {
            List<BrPviStation> list = null;
            lock (lockmapPviObjects)
            {
                mapPviObjStations.TryGetValue(key, out list);
            }
            return list;
        }

        List<BrPviCommJob> GetJobsFromPviObj(string key)
        {
            List<BrPviCommJob> list = null;
            lock (lockmapPviObjects)
            {
                mapPviObjJobs.TryGetValue(key, out list);
            }
            return list;
        }

        BrPviCommJob GetLastUpdatedJobFromPviObj(string key)
        {
            List<BrPviCommJob> list = null;
            lock (lockmapPviObjects)
            {
                mapPviObjJobs.TryGetValue(key, out list);
            }

            if (list != null)
                return list.OrderByDescending(x => x.LastExecutionTime).ToList()[0];
            else
                return null;
        }

        bool MoreJobsHaveSamePlcVariable(BrPviCommJob brJob)
        {
            var jobList = GetJobsFromPviObj(brJob.BrPviVariableCompleteName);
            if (jobList == null)
                return false;            
            else
                // conunt jobs different from current
                return (jobList.Count(j => j != brJob)>0);
        }

        public void CopyJobSettings(BrPviCommJob sourceJob, BrPviCommJob destJob)
        {
            // if source job (last updated) have an initialied state, allign destination, otherwise continue standard initiliazation
            switch (sourceJob.BrpviState)
            {
                case BrPviCommJob.BrPviState.JobInitialized:    // continue the job execution starting from OnJobExecute (JobInitialized)
                    destJob.CreatePviObjects();
                    // set initialized state
                    destJob.pviObjectState = BrPviCommJob.PviObjectState.Activated;
                    // force to align string length
                    destJob.SetStringLength(sourceJob.BrPviArrayLength);
                    destJob.BrpviState = BrPviCommJob.BrPviState.JobInitialized;
                    break;
                case BrPviCommJob.BrPviState.JobUnMapped:       // exit with error                    
                    destJob.CreatePviObjects();
                    // force to align string length                    
                    destJob.BrpviState = BrPviCommJob.BrPviState.JobUnMapped;
                    break;
            }
        }

        public void RestartScheduler()
        {
            if (NeedNewSchedule())
                SetRequestSchedule();
        }

        private void RemoveUnecessaryJobsDuringStationInitialization(ref List<CommJob> list)
        {
            if (!(((BrPviCommJob)list[0]).IsStationInitialized()))
            {
                while (list.Count > 1)
                {
                    RemovePendingJob(list[0]);
                    list.RemoveAt(0);
                }
            }
        }

        private uint GetBrPviRemoveDisabledItemAfterSecs()
        {
            List<Station> st = CommDriver.GetChannelStations(this);
            if (st != null && st.Count > 0)
                return ((BrPviStation)st[0]).BrPviRemoveDisabledItemAfterSecs;
            else
                return 0;
        }

        /// <summary>
        /// Add to list a job list to be unsubscribe from PVIManager
        /// </summary>
        /// <param name="job"></param>
        public void AddJobToUnSubScribe(BrPviCommJob job)
        {
            lock (lockmapPviObjects)
            {
                if (!mapJobsToUnSubScribe.ContainsKey(job))
                {
                    job.BrPviLastUnSubScribeExecutionTime = DateTime.UtcNow;
                    mapJobsToUnSubScribe[job] = true;
                }
            }
        }

        /// <summary>
        /// Remove from list a job that have been schedule to be unsubscribe from PVIManager
        /// </summary>
        /// <param name="job"></param>
        void RemoveJobToUnSubScribe(BrPviCommJob job)
        {
            lock (lockmapPviObjects)
            {
                if (mapJobsToUnSubScribe.ContainsKey(job))
                {
                    job.BrPviUnSubScribeRequested = BrPviCommJob.UnSubScribeState.None;
                    mapJobsToUnSubScribe.Remove(job);
                }
            }
        }

        /// <summary>
        /// Check if a job have to be unsubscribe from PVIManager
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        bool IsJobToUnSubScribe(BrPviCommJob job)
        {
            lock (lockmapPviObjects)
            {
                return mapJobsToUnSubScribe.ContainsKey(job);
            }
        }

        /// <summary>
        /// Get list of jobs that have to be unsubscribe from PVIManager
        /// </summary>
        /// <returns></returns>
        public List<CommJob> GetJobsToUnSubScribe()
        {
            List<CommJob> list = new List<CommJob>();
            lock (lockmapPviObjects)
            {
                list.AddRange(mapJobsToUnSubScribe.Keys.ToList());
            }

            return list;
        }


        #endregion

        #region Properties

        private string _BrPviServerAddress;
        public string BrPviAdsServerAddress
        {
            get
            {
                return _BrPviServerAddress;
            }
            set
            {
                _BrPviServerAddress = value;
            }
        }

        private uint _BrPviServerPort;
        public uint BrPviServerPort
        {
            get
            {
                return _BrPviServerPort;
            }
            set
            {
                BrPviServerPort = value;
            }
        }

        private uint _BrPviServerCommunicationTimeout;
        public uint BrPviServerCommunicationTimeout
        {
            get
            {
                return _BrPviServerCommunicationTimeout;
            }
            set
            {
                BrPviServerCommunicationTimeout = value;
            }
        }

        private uint _BrPviRetryTime;
        public uint BrPviRetryTime
        {
            get
            {
                return _BrPviRetryTime;
            }
            set
            {
                _BrPviRetryTime = value;
            }
        }

        private uint _BrPviMaxNumberSubscriptions;
        #endregion
    }
}