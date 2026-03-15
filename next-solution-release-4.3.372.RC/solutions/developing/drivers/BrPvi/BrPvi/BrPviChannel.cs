using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace BrPvi
{
    public enum BrPviErrorCodes : int
    {
        ErrorCodeCreateFailure = 500,
        ErrorCodeEventError = 501,
        ErrorCodePrepareWriteRequest = 502,
        ErrorCodeConnectionBroken = 503,
        ErrorCodeIdentificationError = 504
    }

    public enum BrPviVersions : byte
    {
        Version2x,
        Version3x,
        Version4x
    }

    public enum BrPviConnectionStatus
    {
        NotInitialized,
        Disconnected,
        Connected,
        Arranged
    }

    public class BrPviChannel : Channel
    {
        public const uint MAX_REQUEST_NUMBER = 500;
        const uint POBJ_EVENT_ERROR = 3;

        #region Constructors

        /// <summary>
        /// Initializes the BrPviChannel object.
        /// </summary>
        public BrPviChannel(CommunicationDriver commdriver, BrPviChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _BrPviServerAddress = settings.BrPviServerAddress;
            _BrPviServerPort = settings.BrPviServerPort;
            _BrPviServerCommunicationTimeout = settings.BrPviServerCommunicationTimeout;
            _BrPviRetryTime = settings.BrPviRetryTime;
            lastConnectionErrorCode = 0;
            channelPviSink = new PviSink(this);
        }

        #endregion

        #region Data Members

        protected object lockStream = new object();
        int lastConnectionErrorCode;
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
        List<PviEventInfo> listOfGlobalEvents = new List<PviEventInfo>();
        BrPviConnectionStatus channelConnectionStatus = BrPviConnectionStatus.NotInitialized;
        Dictionary<string, BrPviStation> mapStations = new Dictionary<string, BrPviStation>();
        Object lockmapPviObjects = new Object();
        // General dictionary of PVI objects
        Dictionary<string, BrPviPviObject> mapPviObjects = new Dictionary<string, BrPviPviObject>();
        // Dictionary of PVI objects linked to stations
        Dictionary<string, List<BrPviStation>> mapPviObjStations = new Dictionary<string, List<BrPviStation>>();
        // Dictionary of PVI objects linked to jobs
        Dictionary<string, List<BrPviCommJob>> mapPviObjJobs = new Dictionary<string, List<BrPviCommJob>>();
        // List of PVI objects to be created
        List<BrPviPviObject> listOfPviObjectsToBeCreated = new List<BrPviPviObject>();
        // Dictionary of pending PVI object create requests
        Dictionary<string, DateTime> mapPendingCreateRequests = new Dictionary<string, DateTime>();
        Object pviEventLockObject = new Object();
        // List of events on PVI objects
        List<PviObjEvent> listOfPviEvents = new List<PviObjEvent>();
        // List of global handles for the PVI objects
        Dictionary<string, GCHandle> mapGlobalHandles = new Dictionary<string, GCHandle>();
        // List of write requests
        List<PviWriteObj> listOfPviWriteRequests = new List<PviWriteObj>();
        // List of write requests
        List<PviWriteObj> listOfPviMaskWriteRequests = new List<PviWriteObj>();
        // Dictionary of pending write requests
        Dictionary<string, DateTime> mapPendingWriteRequests = new Dictionary<string, DateTime>();
        // List of string length requests
        List<BrPviPviObject> listOfStringLengthRequests = new List<BrPviPviObject>();
        // Dictionary of pending string length requests
        Dictionary<string, DateTime> mapPendingStringLengthRequests = new Dictionary<string, DateTime>();
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
                if(pviCommunicationManager != null)
                {
                    connectionState = pviCommunicationManager.isInitialized();
                }
                //SetStateCommandVariableBit(!connectionState, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return connectionState;
            }
        }

        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                if(pviCommunicationManager == null)
                {
                    pviCommunicationManager = new PviComManager();
                }
                if(pviCommunicationManager.isInitialized())
                {
                    return (true);
                }
                if(pviEventsObj == null)
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
                {
                    return (false);
                }
                if (lastConnectionErrorCode != 0)
                {
                    return (false);
                }

                // Set the callback functions for receiving notifications for the global events POBJ_EVENT_PVI_CONNECT,
                // POBJ_EVENT_PVI_DISCONN and POBJ_EVENT_PVI_ARRANGE
                if (!channelPviSink.PviXSetGlobEventMsg())
                {
                    return (false);
                }
                if (lastConnectionErrorCode != 0)
                {
                    channelPviSink.PviXDeinitialize();
                    return (false);
                }

                return (true);
            }
        }

        public override bool DeviceClose()
        {
            lock (lockStream)
            {
                if (pviCommunicationManager.isInitialized())
                {
                    if(!channelPviSink.PviXDeinitialize() || (lastConnectionErrorCode != 0))
                    {
                        DeallocateGlobalHandles();
                        return (false);
                    }
                }

                ResetListAndMaps();

                return true;
            }
        }

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            // Fill the station dictionary
            foreach (BrPviStation stBrPvi in CommDriver.GetChannelStations(this))
            {
                if (stBrPvi != null)
                {
                    mapStations[stBrPvi.Name] = stBrPvi;
                }
            }

            //bool bForceProcessListJobs = false;
            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            //int dbgCount = 0;
            while (true)
            {
                CommJob nextjob = null;
                bool channelConnected = IsDeviceOpen();
                if(!channelConnected)
                {
                    channelConnected = DeviceOpen();
                }

                if(channelConnected)
                {
                    ManageGlobalEvents();
                }

                if(channelConnectionStatus == BrPviConnectionStatus.Arranged)
                {
                    ManagePviEvents();

                    if (ListJobPending.Count == 0)
                    {
                        ScheduleListJob();
                        lock (lockThreadObject)
                        {
                            if (SynchroJob != null)
                            {
                                nextjob = GetNextSynchroJob();
                            }
                            else
                            {
                                nextjob = GetNextBrPviPendingJob();
                            }
                            if (nextjob != null)
                            {
                                ListJobPending.Add(nextjob);
                            }
                        }
                    }

                    lock (lockThreadObject)
                    {
                        // Any pending job?
                        if (ListJobPending.Count > 0)
                        {
                            ExecuteJob(ListJobPending[0]);
                        }

                        if (ListJobPending.Count > 0)
                        {
                            // Timeout elapsed?
                            double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime).TotalMilliseconds;
                            if (dtime > Timeout)
                            {
                                BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.WorkingThread {0} - Timeout elapsed for PVI Obj: {1}",
                                                                   DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 1 - PVI Obj: {1}",
                                                                   DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
#endif                                
                                //error
                                LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewData(ListJobPending[0]);

                                if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                {
                                    brJob.ResetSynchro.WaitOne(Timeout);
                                    RemoveSynchroJob(brJob);
                                    brJob.ResetSynchro.Reset();
                                }

                                ListJobPending.RemoveAt(0);
                                ReceiveBuffer.Clear();
                                ManageTimeoutError();
                            }
                        }
                    }
                }

                if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                    {
                        bool closeOK = DeviceClose();
                        break;
                    }
                }
                StopWorkerThread.WaitOne(0, false);
            }

            channelPviSink.Close();
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
#if DEBUG
            BrPviCommJob brJob = (BrPviCommJob)pendingjob;
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ProcessNewData {0} - Called for PVI Obj: {1}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
#endif
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {

                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            }

            return (true);
        }

        public override void ExecuteJob(CommJob job)
        {
            BrPviCommJob brJob = (BrPviCommJob)job;
            if (brJob == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Null Job",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return;
            }

            // Pending job?
            if (brJob.IsPending && (brJob.Status == BrPviCommJobStatus.Idle))
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Job already pending",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return;
            }

            if (brJob.Status == BrPviCommJobStatus.Idle)
            {
                if (brJob.MustWrite() || brJob.MustRead() || brJob.MustRequestStringLength())
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Job status: WaitingToSendRequest",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"));
                    brJob.Status = BrPviCommJobStatus.WaitingToSendRequest;
                    base.ExecuteJob(job);
                }
            }
            
            BrPviStation brStation = (BrPviStation)brJob.Station;
            // Any station PVI object must be created?
            if (!brStation.pviObjectsInitialized)
            {
                brStation.CreatePviObjects();
                if (listOfPviObjectsToBeCreated.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - calling channelPviSink.CreatePviObjects for station",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"));
                    channelPviSink.CreatePviObjects();
                }
            }

            // Any job PVI object must be created?
            if (brJob.pviObjectState != BrPviCommJob.PviObjectState.Activated) //==false
            {
                brJob.CreatePviObjects();
                if (listOfPviObjectsToBeCreated.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - calling channelPviSink.CreatePviObjects for itself",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"));
                    channelPviSink.CreatePviObjects();
                } else {
                    //if no object need to be created means that all tag were already allocated
                    if (brJob.IsSyncroJob)
                        brJob.pviObjectState = BrPviCommJob.PviObjectState.Activated; //true
                }
            }

            // Is item unmapped on device ? terminate job
            BrPviPviObject pviObj;
            if (mapPviObjects.TryGetValue(brJob.BrPviVariableCompleteName, out pviObj))
            {
                if (pviObj.Status == PviObjectStatus.UnMappedOnPlc)
                {
                    BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                    brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorVariableUmappedOnDevice, brJob.BrPviVariableCompleteName);
                    ExecutedJobArgs e = new ExecutedJobArgs();
                    e.Job = brJob;
                    e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeIdentificationError;
                    OnJobExecuted(e);
                    if ((CommJob)brJob == SynchroJob)
                    {
                        brJob.ResetSynchro.WaitOne(Timeout);
                        RemoveSynchroJob(brJob);
                        brJob.ResetSynchro.Reset();
                    }
                    ListJobPending.Remove(job);
                    brJob.IsPending = false;
                    return;
                }
            }

            switch (brJob.Status)
            {
                // Job activated just to create PVI objects
                case BrPviCommJobStatus.Idle:
                    if ((CommJob)brJob == SynchroJob)
                    {
                        brJob.ResetSynchro.WaitOne(Timeout);
                        RemoveSynchroJob(brJob);
                        brJob.ResetSynchro.Reset();
                    }

                    ListJobPending.Remove(job);
                    brJob.IsPending = false;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Job activated just to create PVI objects for {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 2 - PVI Obj: {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                    break;

                // Job that must send a read or write or string length request
                case BrPviCommJobStatus.WaitingToSendRequest:
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Job that must send a request - brStation.pviObjectsInitialized: {1} - brJob.pviObjectsInitialized: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brStation.pviObjectsInitialized, brJob.pviObjectState);

                    if (brStation.pviObjectsInitialized && brJob.pviObjectState == BrPviCommJob.PviObjectState.Activated)
                    {
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Job that must send a request? - PVI OBJ: {1} - brJob.MustRequestStringLength(): {2} - brJob.MustWrite(): {3} - brJob.MustRead(): {4}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName, brJob.MustRequestStringLength(), brJob.MustWrite(), brJob.MustRead());
                        // String length not assigned --> Request it
                        if (brJob.MustRequestStringLength())
                        {
                            BrPviPviObject pviVarObject = GetPviObject(brJob.BrPviVariableCompleteName);
                            brJob.Status = BrPviCommJobStatus.StringLengthRequestPending;
                            AddRequestToReadStringLengthList(pviVarObject);
                            // Send the request
                            channelPviSink.SendStringLengthRequests();
                        }
                        // Create a write request
                        else if (brJob.MustWrite())
                        {
                            PviWriteObj pviWriteObject = PrepareWriteRequest(brJob);
                            if (pviWriteObject != null)
                            {
                                // no data to write --> inputoutput/exception out job
                                if (pviWriteObject.DataLength == 0)
                                {
                                    ListJobPending.Remove(job);
                                    brJob.IsPending = false;
                                }
                                else
                                {
                                    base.ExecuteJob(job);
                                    brJob.Status = BrPviCommJobStatus.WriteRequestPending;
                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Calling AddRequestToWriteList",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"));
                                    // Add the request to the list of write requests
                                    AddRequestToWriteList(pviWriteObject);

                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Calling channelPviSink.WritePviObjects",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"));

                                    // Send the request
                                    channelPviSink.WritePviObjects();
                                }
                            }

                            // Error preparing the request
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Error preparing write request",
                                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                                brJob.Status = BrPviCommJobStatus.Idle;
                                BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                                brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorPreparingWriteRequest, brJob.BrPviVariableCompleteName);
                                ExecutedJobArgs e = new ExecutedJobArgs();
                                e.Job = brJob;
                                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 3 - PVI Obj: {1}",
                                                                   DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);

                                //ListJobPending.Remove(job);
                                //brJob.IsPending = false;
                                e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodePrepareWriteRequest;
                                OnJobExecuted(e);
                                if ((CommJob)brJob == SynchroJob)
                                {
                                    brJob.ResetSynchro.WaitOne(Timeout);
                                    RemoveSynchroJob(brJob);
                                    brJob.ResetSynchro.Reset();
                                }
                                ListJobPending.Remove(job);
                                brJob.IsPending = false;
                            }
                        }
                        else if(brJob.MustRead())
                        {
                            BrPviPviObject jobPviObj = GetPviObject(brJob.BrPviVariableCompleteName);
                            if (jobPviObj != null)
                            {
                                //base.ExecuteJob(job);
                                if ((jobPviObj.Status == PviObjectStatus.DataReceived) || (jobPviObj.DataArray.Count() > 0))
                                {
#if DEBUG
                                    string debugMessage = String.Format("BR_DEBUG - BrPviChannel.ExecuteJob {0} - Data already read for PVI Obj: {1}",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                    if (brJob.TagsList.Count > 0)
                                    {
                                        debugMessage += String.Format(" - Tag: {0}", brJob.TagsList[0].TagNode.NodeId);
                                    }
                                    System.Diagnostics.Debug.WriteLine(debugMessage);
#endif
                                    brJob.MustInitializeData = false;
                                    brJob.Status = BrPviCommJobStatus.Idle;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.ErrorCode = (DriverErrorCodes)DriverErrorCodes.ErrorNoError;
                                    e.Job = brJob;
                                    e.Values = jobPviObj.DataArray;
                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 4 - PVI Obj: {1}",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                                        
                                    //ListJobPending.Remove(job);
                                    //brJob.IsPending = false;                                    
                                    OnJobExecuted(e);

                                    if ((CommJob)brJob == SynchroJob)
                                    {
                                        brJob.ResetSynchro.WaitOne(Timeout);
                                        RemoveSynchroJob(brJob);
                                        brJob.ResetSynchro.Reset();
                                    }
                                    ListJobPending.Remove(job);
                                    brJob.IsPending = false;
                                } else
                                {
                                    base.ExecuteJob(job);
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 5 - PVI Obj: {1}",
                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);

                            if ((CommJob)brJob == SynchroJob)
                            {
                                brJob.ResetSynchro.WaitOne(Timeout);
                                RemoveSynchroJob(brJob);
                                brJob.ResetSynchro.Reset();
                            }

                            ListJobPending.Remove(job);
                            brJob.IsPending = false;
                            brJob.Status = BrPviCommJobStatus.Idle;
                        }
                    }
                    break;

                // Job that has sent a request for getting the string length
                case BrPviCommJobStatus.StringLengthRequestPending:
                    if(!brJob.MustRequestStringLength())
                    {
                        if(!brJob.MustWrite() && !brJob.MustRead())
                        {
                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 6 - PVI Obj: {1}",
                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);

                            if ((CommJob)brJob == SynchroJob)
                            {
                                brJob.ResetSynchro.WaitOne(Timeout);
                                RemoveSynchroJob(brJob);
                                brJob.ResetSynchro.Reset();
                            }

                            ListJobPending.Remove(job);
                            brJob.IsPending = false;
                            brJob.Status = BrPviCommJobStatus.Idle;
                        }
                        else
                        {
                            brJob.Status = BrPviCommJobStatus.WaitingToSendRequest;
                        }
                    }
                    break;

            }
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            BrPviCommJob brJob = (BrPviCommJob)e.Job;
            brJob.Status = BrPviCommJobStatus.Idle;
            base.OnJobExecuted(e);           
        }

        #endregion

        #region Specific Methods

        public void PviXInitialize()
        {
            lastConnectionErrorCode = pviCommunicationManager.PviXInitialize((int)_BrPviServerCommunicationTimeout, (int)_BrPviRetryTime, _BrPviServerAddress, (int)_BrPviServerPort);
        }

        public void SetPviGlobalEvents()
        {
            lastConnectionErrorCode = pviCommunicationManager.SetPviGlobalEvents(intptrConnectCallback, intptrDisconnectCallback, intptrArrangeCallback, intptrChannel);
        }

        public void PviXDeinitialize()
        {
            lastConnectionErrorCode = pviCommunicationManager.PviXDeinitialize();
        }

        public void AddGlobalEvent(PviEventInfo globalEventInfo)
        {
            lock (pviEventLockObject)
            {
                listOfGlobalEvents.Add(globalEventInfo);
            }
        }

        public void AddPviEvent(PviObjEvent pviEvent)
        {
            lock (pviEventLockObject)
            {
                listOfPviEvents.Add(pviEvent);
            }
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
            if(listOfPviObjectsToBeCreatedCopy != null)
            {
                foreach (var pviObj in listOfPviObjectsToBeCreatedCopy)
                {
                    BrPviPviObject pviObjToBeCreated = (BrPviPviObject)pviObj;
                    GCHandle handleOfPviObj;
                    GetOrAddGCHandleToDictionary(pviObjToBeCreated, out handleOfPviObj);
                    lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviObjToBeCreated, GCHandle.ToIntPtr(handleOfPviObj));
                    if(lastConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObjToBeCreated.LastError = (uint)lastConnectionErrorCode;
                        pviObjToBeCreated.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildCreateErrorEvent(pviObjToBeCreated);
                        AddPviEvent(errorEvent);
                    }
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.CreatePviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2} - LinkDescr:{3}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObjToBeCreated.Name, lastConnectionErrorCode, pviObj.LinkDescr);
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
                    lastConnectionErrorCode = pviCommunicationManager.PviXWriteRequest(intptrDataCallback, pviObjWriteRequest, GCHandle.ToIntPtr(handleOfPviObj), PviComManager.POBJ_ACC_DATA);
                    if (lastConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObj.LastError = (uint)lastConnectionErrorCode;
                        pviObj.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildWriteErrorEvent(pviObj);
                        AddPviEvent(errorEvent);
                    }
                    else
                    {
                        // Update value of the PVI Object
                        pviObj.UpdateValue(pviObjWriteRequest.DataArray, pviObjWriteRequest.DataLength);
                    }
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.WritePviObjects {0} - return code of PviComManager.PviXWriteRequest for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lastConnectionErrorCode);
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
                    lastConnectionErrorCode = pviCommunicationManager.PviXWriteRequest(intptrDataCallback, pviObjWriteRequest, GCHandle.ToIntPtr(handleOfPviObj), PviComManager.POBJ_ACC_EVMASK);
                    if (lastConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObj.LastError = (uint)lastConnectionErrorCode;
                        pviObj.Status = PviObjectStatus.NotCreated;
                        PviObjEvent errorEvent = BuildWriteErrorEvent(pviObj);
                        AddPviEvent(errorEvent);
                    }
                    //else
                    //{
                    //    // Update value of the PVI Object
                    //    pviObj.UpdateValue(pviObjWriteRequest.DataArray, pviObjWriteRequest.DataLength);
                    //}
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.WriteMaskPviObjects {0} - return code of PviComManager.WriteMaskPviObjects for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lastConnectionErrorCode);
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
                    lastConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviObject, PviComManager.POBJ_ACC_TYPE_EXTERN, GCHandle.ToIntPtr(handleOfPviObj));
                    if (lastConnectionErrorCode != 0)
                    {
                        // Generate an error event
                        pviObject.LastError = (uint)lastConnectionErrorCode;
                        PviObjEvent errorEvent = BuildReadErrorEvent(pviObject);
                        AddPviEvent(errorEvent);
                    }
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.SendStringLengthRequests {0} - return code of PviComManager.PviXReadRequest for {1}: {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObject.Name, lastConnectionErrorCode);
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
                    if (brJob.TagsListToWrite.Count == 0)
                    {
                        if (brJob.Type != DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                        {
                            return (returnedObj);
                        }
                        else
                        {
                            brJob.TagsListToWrite.AddRange(brJob.TagsList);
                            if (brJob.TagsListToWrite.Count == 0)
                            {
                                return (returnedObj);
                            }
                        }
                    }

                    tagToWrite = (BrPviTag)brJob.TagsListToWrite[0];
                    brJob.TagsListToWrite.Clear();
                }

                listOnWriting.Add(tagToWrite);
                byte[] jobdata;
                uint nData = 0;
                lock (brJob.retLockList())
                {
                    if (StatusCode.IsGood(tagToWrite.Value.StatusCode) && (tagToWrite.LastValue != null))
                    {
                        if ((brJob.Type == LinkType.ExceptionOutput || brJob.Type == LinkType.InputOutput))
                        {
                            if ((brJob.Station.RewritingOfTheSameValue == false) && (tagToWrite.LastValue.Equals(tagToWrite.Value.Value)))
                            {
                                if (listOnWriting.Contains(tagToWrite))
                                {
                                    listOnWriting.Remove(tagToWrite);
                                    // return empty/not null "object" to mark as a no values to write jobs
                                    return (new PviWriteObj(pviVarObject));
                                }
                            }
                        }
                    }
                    tagToWrite.LastValue = tagToWrite.Value.Value;
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
                if(returnedObj.SetData(jobdata, nData))
                {
                    lock (brJob.retLockList())
                    {
                        listOnWriting.ForEach((tag) =>
                        {
                            if (!brJob.TagsListOnWriting.Contains(tag))
                                brJob.TagsListOnWriting.Add(tag);
                        });
                        listOnWriting.Clear();
                    }
                }
                else
                {
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
            eventInfo.nType = POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObjToBeCreated.LastError;
            return (new PviObjEvent(eventInfo, pviObjToBeCreated));
        }

        public PviObjEvent BuildWriteErrorEvent(BrPviPviObject pviObjToBeWritten)
        {
            PviEventInfo eventInfo = new PviEventInfo();
            eventInfo.nMode = (uint)PviObjectModes.POBJ_MODE_WRITE;
            eventInfo.nType = POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObjToBeWritten.LastError;
            return (new PviObjEvent(eventInfo, pviObjToBeWritten));
        }

        public PviObjEvent BuildReadErrorEvent(BrPviPviObject pviObj)
        {
            PviEventInfo eventInfo = new PviEventInfo();
            eventInfo.nMode = (uint)PviObjectModes.POBJ_MODE_READ;
            eventInfo.nType = POBJ_EVENT_ERROR;
            eventInfo.ErrCode = pviObj.LastError;
            return (new PviObjEvent(eventInfo, pviObj));
        }

        protected BrPviCommJob GetNextBrPviPendingJob()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    while (!queue.IsEmpty)
                    {
                        CommJob j = null;
                        if (queue.TryDequeue(out j) == true)
                        {
                            BrPviCommJob bj = (BrPviCommJob)j;
                            if (bj != null)
                            {
                                if (bj.MustSendARequest())
                                {
                                    return (bj);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        void ManageConnectionBroken()
        {
            lock (lockStream)
            {             
                BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorConnectionBroken, lastConnectionErrorCode);
                DeviceClose();
                channelConnectionStatus = BrPviConnectionStatus.NotInitialized;
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                foreach (var station in CommDriver.GetChannelStations(this))
                {
                    // Set the error state of stations and jobs
                    BrPviStation brStation = (BrPviStation)station;
                    brStation.ManageConnectionBroken();
                }
            }
        }

        void ResetListAndMaps()
        {
            lock(lockmapPviObjects)
            {
                // Reset the status of all PVI objects
                foreach (KeyValuePair<string, BrPviPviObject> item in mapPviObjects)
                {
                    if(item.Value != null)
                    {
                        item.Value.LinkID = 0;
                        item.Value.Status = PviObjectStatus.MustBeCreated;
                    }
                }
                
                // Empty the list of write requests
                listOfPviWriteRequests.Clear();
                // Empty the dictionary of pending write requests
                mapPendingWriteRequests.Clear();
                // Empty the list of string length requests
                listOfStringLengthRequests.Clear();
                // Empty the dictionary of pending string length requests
                mapPendingStringLengthRequests.Clear();
                // Empty the list of objects to be created
                listOfPviObjectsToBeCreated.Clear();
                // Empty the dictionary of pending create requests
                mapPendingCreateRequests.Clear();
                // Empty the dictionary of jobs associated to pvi object
                mapPviObjJobs.Clear();
            }

            // Empty the lists of events on PVI objects
            lock (pviEventLockObject)
            {
                listOfGlobalEvents.Clear();
                listOfPviEvents.Clear();
            }

            // Deallocate the handles for events
            DeallocateGlobalHandles();

            pviEventsObj = null;
        }


        private uint? _LastPVIGlobalEventsErrorCode;
        public void ManageGlobalEvents()
        {
            List<PviEventInfo> listOfGlobalEventsCopy = null;
            lock (pviEventLockObject)
            {
                if (listOfGlobalEvents.Count > 0)
                {
                    listOfGlobalEventsCopy = new List<PviEventInfo>(listOfGlobalEvents);
                    listOfGlobalEvents.Clear();
                }
            }
            if(listOfGlobalEventsCopy != null)
            {
                foreach(var info in listOfGlobalEventsCopy)
                {
                    PviEventInfo eventInfo = info;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManageGlobalEvents {0} - Received event {1} with Error Code {2}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), eventInfo.nType, eventInfo.ErrCode);
                    switch (eventInfo.nType)
                    {
                        case PviComManager.POBJ_EVENT_PVI_CONNECT:
                            if (eventInfo.ErrCode == 0) // Connection OK
                            {
                                lastConnectionErrorCode = 0;
                                if (channelConnectionStatus != BrPviConnectionStatus.Arranged)
                                {
                                    channelConnectionStatus = BrPviConnectionStatus.Connected;
                                }
                            }
                            else // Connection error
                            {
                                lastConnectionErrorCode = (int)eventInfo.ErrCode;
                                ManageConnectionBroken();
                                //DeviceClose();
                                //channelConnectionStatus = BrPviConnectionStatus.Disconnected;
                                return;
                            }
                            break;
                        case PviComManager.POBJ_EVENT_PVI_DISCONN:
                            lastConnectionErrorCode = (int)eventInfo.ErrCode;
                            ManageConnectionBroken();
                            //DeviceClose();
                            //channelConnectionStatus = BrPviConnectionStatus.Disconnected;
                            break;
                        case PviComManager.POBJ_EVENT_PVI_ARRANGE:
                            if (eventInfo.ErrCode == 0) // Connection OK
                            {
                                lastConnectionErrorCode = 0;
                                channelConnectionStatus = BrPviConnectionStatus.Arranged;
                            }
                            else // Connection error
                            {
                                lastConnectionErrorCode = (int)eventInfo.ErrCode;
                                ManageConnectionBroken();
                                //DeviceClose();
                                //channelConnectionStatus = BrPviConnectionStatus.Disconnected;
                                return;
                            }
                            break;
                    }
                    
                    // if error code is different from previous valie (also 0 == Good) update channel state
                    if (!_LastPVIGlobalEventsErrorCode.HasValue || eventInfo.ErrCode != _LastPVIGlobalEventsErrorCode)
                    {
                        bool NotConnected = (eventInfo.ErrCode != 0 ? true : false);
                        SetStateCommandVariableBit(NotConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                        _LastPVIGlobalEventsErrorCode = eventInfo.ErrCode;
                    }
                }
            }
        }

        public void ManagePviEvents()
        {
            List<PviObjEvent> listOfPviEventsCopy = null;
            lock (pviEventLockObject)
            {
                if (listOfPviEvents.Count > 0)
                {
                    listOfPviEventsCopy = new List<PviObjEvent>(listOfPviEvents);
                    listOfPviEvents.Clear();
                }
            }
            try
            {
                lock (lockmapPviObjects)
                {
                    if (listOfPviEventsCopy != null)
                    {
                        foreach (var eventObj in listOfPviEventsCopy)
                        {
                            PviObjEvent pviEvent = eventObj;
                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}",
                                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, pviEvent.EventInfo.nMode, pviEvent.EventInfo.nType, pviEvent.EventInfo.ErrCode);
                            BrPviPviObject pviObj;
                            if (mapPviObjects.TryGetValue(pviEvent.PviObj.Name, out pviObj))
                            {
                                switch (pviEvent.EventInfo.nMode)
                                {
                                    case (uint)PviObjectModes.POBJ_MODE_CREATE:

                                        System.Diagnostics.Debug.WriteLine(string.Format("PviObjectModes.POBJ_MODE_CREATE {0} Item:{1}, LinkID:{2}, ErrorCode:{3}", DateTime.Now, pviObj.Name, pviEvent.EventInfo.LinkID,pviEvent.EventInfo.ErrCode));

                                        // Remove the Pvi Object entry in the dictionary of pending create requests
                                        if(mapPendingCreateRequests.ContainsKey(pviObj.Name))
                                        {
                                            mapPendingCreateRequests.Remove(pviObj.Name);
                                        }

                                        // Error?
                                        if (pviEvent.EventInfo.ErrCode != 0)
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            pviObj.Status = PviObjectStatus.NotCreated;
                                            if (pviObj.IsStationObject())
                                            {
                                                List<BrPviStation> stationList = null;
                                                if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                                {
                                                    foreach(BrPviStation station in stationList)
                                                    {
                                                        station.ManageCreationEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (stationList.Contains(brJob.Station))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 7 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        job.ManageCreationEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (jobList.Contains(brJob))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 8 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // PVI object successfully created
                                        else
                                        {
                                            pviObj.LastError = 0;
                                            pviObj.LinkID = pviEvent.EventInfo.LinkID;
                                            if(pviObj.Status != PviObjectStatus.DataReceived)
                                            {
                                                pviObj.Status = PviObjectStatus.Ready;
                                            }
                                            if (pviObj.IsStationObject())
                                            {
                                                List<BrPviStation> stationList = null;
                                                if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                                {
                                                    foreach (BrPviStation station in stationList)
                                                    {
                                                        station.ManageCreationEvent(pviObj);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        job.ManageCreationEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (jobList.Contains(brJob))
                                                        {
                                                            brJob.StartExecutionTime = DateTime.UtcNow;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case (uint)PviObjectModes.POBJ_MODE_EVENT:
                                        // Error
                                        if(((pviEvent.EventInfo.nType == POBJ_EVENT_ERROR) && (pviEvent.EventInfo.ErrCode != 0)))
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            if (pviObj.IsStationObject())
                                            {
                                                List<BrPviStation> stationList = null;
                                                if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                                {
                                                    foreach (BrPviStation station in stationList)
                                                    {
                                                        station.ManagePviEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (stationList.Contains(brJob.Station))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 9 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        job.ManagePviErrorEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (jobList.Contains(brJob))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 10 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // Error
                                        else if(pviEvent.EventInfo.ErrCode != 0)
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            if (pviObj.IsStationObject())
                                            {
                                                List<BrPviStation> stationList = null;
                                                if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                                {
                                                    foreach (BrPviStation station in stationList)
                                                    {
                                                        station.ManagePviEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (stationList.Contains(brJob.Station))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 11 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        job.ManagePviErrorEvent(pviObj);
                                                    }
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (jobList.Contains(brJob))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 12 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // Data received
                                        else if ((pviEvent.PviObj.Status == PviObjectStatus.DataReceived) || (pviEvent.PviObj.DataArray.Count() > 0))
                                        {
                                            pviObj.LastError = 0;
                                            if (pviObj.IsStationObject())
                                            {
                                                pviObj.Status = PviObjectStatus.Ready;
                                                List<BrPviStation> stationList = null;
                                                if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                                {
                                                    foreach (BrPviStation station in stationList)
                                                    {
                                                        station.ManageDataReceivedEvent(pviEvent);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        //PassReceivedDataToJob(pviEvent, job);
                                                        PassReceivedDataToJob(pviEvent.PviObj.DataArray, job);
                                                    }
                                                    // below managed only syncrojob --> writing jobs management moved into PviObjectModes.POBJ_MODE_WRITE
                                                    if (ListJobPending.Count > 0)
                                                    {
                                                        BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                        if (jobList.Contains(brJob))
                                                        {
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            //}
                                                                brJob.IsPending = false;
                                                                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 13 - PVI Obj: {1}",
                                                                                                   DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                                ListJobPending.Remove(brJob);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case (uint)PviObjectModes.POBJ_MODE_WRITE:
                                        // Remove the Pvi Object entry in the dictionary of pending write requests
                                        if (mapPendingWriteRequests.ContainsKey(pviObj.Name))
                                        {
                                            mapPendingWriteRequests.Remove(pviObj.Name);
                                        }

                                        // Error
                                        if (((pviEvent.EventInfo.nType == POBJ_EVENT_ERROR) && (pviEvent.EventInfo.ErrCode != 0)))
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            List<BrPviCommJob> jobList = null;
                                            if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                            {
                                                if (ListJobPending.Count > 0)
                                                {
                                                    BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                    if (jobList.Contains(brJob))
                                                    {
                                                        if (brJob.Status == BrPviCommJobStatus.WriteRequestPending)
                                                        {
                                                            brJob.ManagePviErrorEvent(pviObj);
                                                        }
                                                        if (SynchroJob != null && SynchroJob == brJob)
                                                        {
                                                            brJob.ResetSynchro.WaitOne(Timeout);
                                                            RemoveSynchroJob(brJob);
                                                            brJob.ResetSynchro.Reset();
                                                        }
                                                        brJob.IsPending = false;
                                                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 14 - PVI Obj: {1}",
                                                                                           DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                        ListJobPending.Remove(brJob);
                                                    }
                                                }
                                            }
                                        }
                                        // Error
                                        else if (pviEvent.EventInfo.ErrCode != 0)
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            List<BrPviCommJob> jobList = null;
                                            if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                            {
                                                if (ListJobPending.Count > 0)
                                                {
                                                    BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                    if (jobList.Contains(brJob))
                                                    {
                                                        if (brJob.Status == BrPviCommJobStatus.WriteRequestPending)
                                                        {
                                                            brJob.ManagePviErrorEvent(pviObj);
                                                        }

                                                        if (SynchroJob != null && SynchroJob == brJob)
                                                        {
                                                            brJob.ResetSynchro.WaitOne(Timeout);
                                                            RemoveSynchroJob(brJob);
                                                            brJob.ResetSynchro.Reset();
                                                        }
                                                        brJob.IsPending = false;
                                                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 15 - PVI Obj: {1}",
                                                                                           DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                        ListJobPending.Remove(brJob);
                                                    }
                                                }
                                            }
                                        }
                                        // Success reply received
                                        else if (pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_DATA)
                                        {
                                            pviObj.LastError = 0;
                                            if(pviObj.Status != PviObjectStatus.DataReceived)
                                            {
                                                pviObj.Status = PviObjectStatus.Ready;
                                            }
                                            List<BrPviCommJob> jobList = null;
                                            if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                            {
                                                if (ListJobPending.Count > 0)
                                                {
                                                    BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                    if (jobList.Contains(brJob))
                                                    {
                                                        if(brJob.Status == BrPviCommJobStatus.WriteRequestPending)
                                                        {
                                                            PassWriteOKNotificationToJob(brJob);
                                                            if (SynchroJob != null && SynchroJob == brJob)
                                                            {
                                                                brJob.ResetSynchro.WaitOne(Timeout);
                                                                RemoveSynchroJob(brJob);
                                                                brJob.ResetSynchro.Reset();
                                                            }
                                                            brJob.IsPending = false;
                                                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 16 - PVI Obj: {1}",
                                                                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                            ListJobPending.Remove(brJob);
                                                        }
                                                    }
                                                }
                                            }                                        
                                        }
                                        else if (pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_EVMASK) {
                                            pviObj.LastError = 0;
                                            if (pviObj.Status != PviObjectStatus.DataReceived)
                                            {
                                                pviObj.Status = PviObjectStatus.Ready;
                                            }
                                        }
                                        break;

                                    case (uint)PviObjectModes.POBJ_MODE_READ:
                                        // Remove the Pvi Object entry in the dictionary of pending string length requests
                                        if (mapPendingStringLengthRequests.ContainsKey(pviObj.Name))
                                        {
                                            mapPendingStringLengthRequests.Remove(pviObj.Name);
                                        }

                                        // Error
                                        if (((pviEvent.EventInfo.nType == POBJ_EVENT_ERROR) && (pviEvent.EventInfo.ErrCode != 0)))
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            List<BrPviCommJob> jobList = null;
                                            if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                            {
                                                if (ListJobPending.Count > 0)
                                                {
                                                    BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                    if (jobList.Contains(brJob))
                                                    {
                                                        if (brJob.Status == BrPviCommJobStatus.StringLengthRequestPending)
                                                        {
                                                            brJob.ManagePviErrorEvent(pviObj);
                                                        }
                                                        if (SynchroJob != null && SynchroJob == brJob)
                                                        {
                                                            brJob.ResetSynchro.WaitOne(Timeout);
                                                            RemoveSynchroJob(brJob);
                                                            brJob.ResetSynchro.Reset();
                                                        }
                                                        brJob.IsPending = false;
                                                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 17 - PVI Obj: {1}",
                                                                                           DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                        ListJobPending.Remove(brJob);
                                                    }
                                                }
                                            }
                                        }
                                        // Error
                                        else if (pviEvent.EventInfo.ErrCode != 0)
                                        {
                                            pviObj.LastError = pviEvent.EventInfo.ErrCode;
                                            List<BrPviCommJob> jobList = null;
                                            if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                            {
                                                if (ListJobPending.Count > 0)
                                                {
                                                    BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                    if (jobList.Contains(brJob))
                                                    {
                                                        if (brJob.Status == BrPviCommJobStatus.StringLengthRequestPending)
                                                        {
                                                            brJob.ManagePviErrorEvent(pviObj);
                                                        }

                                                        if (SynchroJob != null && SynchroJob == brJob)
                                                        {
                                                            brJob.ResetSynchro.WaitOne(Timeout);
                                                            RemoveSynchroJob(brJob);
                                                            brJob.ResetSynchro.Reset();
                                                        }
                                                        brJob.IsPending = false;
                                                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 18 - PVI Obj: {1}",
                                                                                           DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                        ListJobPending.Remove(brJob);
                                                    }
                                                }
                                            }
                                        }
                                        // Data received
                                        else if ((pviEvent.EventInfo.nType == PviComManager.POBJ_ACC_TYPE_EXTERN) &&
                                                 (pviEvent.PviObj.Status == PviObjectStatus.DataInfoReceived))
                                        {
                                            pviObj.LastError = 0;
                                            pviObj.Status = PviObjectStatus.Ready;
                                            if(pviObj.Type == PviObjectTypes.POBJ_PVAR)
                                            {
                                                List<BrPviCommJob> jobList = null;
                                                if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                                {
                                                    uint stringLength = ParseStringLength(pviEvent.DataArray, pviEvent.DataLength);
                                                    foreach (BrPviCommJob job in jobList)
                                                    {
                                                        job.SetStringLength(stringLength);
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case (uint)PviObjectModes.POBJ_MODE_DELETE:
                                        break;
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed for any pending create request
                    Dictionary<string, DateTime> mapPendingCreateRequestsCopy = null;
                    if (mapPendingCreateRequests.Count > 0)
                    {
                        mapPendingCreateRequestsCopy = new Dictionary<string, DateTime>(mapPendingCreateRequests);
                        mapPendingCreateRequests.Clear();
                        foreach (string pviObjName in mapPendingCreateRequestsCopy.Keys)
                        {
                            double dtime = (DateTime.UtcNow - mapPendingCreateRequestsCopy[pviObjName]).TotalMilliseconds;
                            if (dtime < Timeout)
                            {
                                mapPendingCreateRequests[pviObjName] = mapPendingCreateRequestsCopy[pviObjName];
                            }
                            else
                            {
                                BrPviPviObject pviObj;
                                if (mapPviObjects.TryGetValue(pviObjName, out pviObj))
                                {
                                    pviObj.LastError = (uint)DriverErrorCodes.ErrorTimeOut;
                                    pviObj.Status = PviObjectStatus.NotCreated;
                                    if (pviObj.IsStationObject())
                                    {
                                        List<BrPviStation> stationList = null;
                                        if (mapPviObjStations.TryGetValue(pviObj.Name, out stationList))
                                        {
                                            foreach (BrPviStation station in stationList)
                                            {
                                                station.ManageCreationEvent(pviObj);
                                            }
                                            if (ListJobPending.Count > 0)
                                            {
                                                BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                if (stationList.Contains(brJob.Station))
                                                {
                                                    if (SynchroJob != null && SynchroJob == brJob)
                                                    {
                                                        brJob.ResetSynchro.WaitOne(Timeout);
                                                        RemoveSynchroJob(brJob);
                                                        brJob.ResetSynchro.Reset();
                                                    }
                                                    brJob.IsPending = false;
                                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 19 - PVI Obj: {1}",
                                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                    ListJobPending.Remove(brJob);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<BrPviCommJob> jobList = null;
                                        if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                        {
                                            foreach (BrPviCommJob job in jobList)
                                            {
                                                job.ManageCreationEvent(pviObj);
                                            }
                                            if (ListJobPending.Count > 0)
                                            {
                                                BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                                if (jobList.Contains(brJob))
                                                {
                                                    if (SynchroJob != null && SynchroJob == brJob)
                                                    {
                                                        brJob.ResetSynchro.WaitOne(Timeout);
                                                        RemoveSynchroJob(brJob);
                                                        brJob.ResetSynchro.Reset();
                                                    }
                                                    brJob.IsPending = false;
                                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 20 - PVI Obj: {1}",
                                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                    ListJobPending.Remove(brJob);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed for any pending write request
                    Dictionary<string, DateTime> mapPendingWriteRequestsCopy = null;
                    if (mapPendingWriteRequests.Count > 0)
                    {
                        mapPendingWriteRequestsCopy = new Dictionary<string, DateTime>(mapPendingWriteRequests);
                        mapPendingWriteRequests.Clear();
                        foreach (string pviObjName in mapPendingWriteRequestsCopy.Keys)
                        {
                            double dtime = (DateTime.UtcNow - mapPendingWriteRequestsCopy[pviObjName]).TotalMilliseconds;
                            if (dtime < Timeout)
                            {
                                mapPendingWriteRequests[pviObjName] = mapPendingWriteRequestsCopy[pviObjName];
                            }
                            else
                            {
                                BrPviPviObject pviObj;
                                if (mapPviObjects.TryGetValue(pviObjName, out pviObj))
                                {
                                    pviObj.LastError = (uint)DriverErrorCodes.ErrorTimeOut;
                                    pviObj.Status = PviObjectStatus.Ready;
                                    List<BrPviCommJob> jobList = null;
                                    if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                    {
                                        if (ListJobPending.Count > 0)
                                        {
                                            BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                            if (jobList.Contains(brJob))
                                            {
                                                if (brJob.Status == BrPviCommJobStatus.WriteRequestPending)
                                                {
                                                    brJob.ManagePviErrorEvent(pviObj);
                                                    if (SynchroJob != null && SynchroJob == brJob)
                                                    {
                                                        brJob.ResetSynchro.WaitOne(Timeout);
                                                        RemoveSynchroJob(brJob);
                                                        brJob.ResetSynchro.Reset();
                                                    }
                                                    brJob.IsPending = false;
                                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 21 - PVI Obj: {1}",
                                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                    ListJobPending.Remove(brJob);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed for any pending string length request
                    Dictionary<string, DateTime> mapPendingStringLengthRequestsCopy = null;
                    if (mapPendingStringLengthRequests.Count > 0)
                    {
                        mapPendingStringLengthRequestsCopy = new Dictionary<string, DateTime>(mapPendingStringLengthRequests);
                        mapPendingStringLengthRequests.Clear();
                        foreach (string pviObjName in mapPendingStringLengthRequestsCopy.Keys)
                        {
                            double dtime = (DateTime.UtcNow - mapPendingStringLengthRequestsCopy[pviObjName]).TotalMilliseconds;
                            if (dtime < Timeout)
                            {
                                mapPendingStringLengthRequests[pviObjName] = mapPendingStringLengthRequestsCopy[pviObjName];
                            }
                            else
                            {
                                BrPviPviObject pviObj;
                                if (mapPviObjects.TryGetValue(pviObjName, out pviObj))
                                {
                                    pviObj.LastError = (uint)DriverErrorCodes.ErrorTimeOut;
                                    pviObj.Status = PviObjectStatus.Ready;
                                    List<BrPviCommJob> jobList = null;
                                    if (mapPviObjJobs.TryGetValue(pviObj.Name, out jobList))
                                    {
                                        if (ListJobPending.Count > 0)
                                        {
                                            BrPviCommJob brJob = (BrPviCommJob)ListJobPending[0];
                                            if (jobList.Contains(brJob))
                                            {
                                                if (brJob.Status == BrPviCommJobStatus.StringLengthRequestPending)
                                                {
                                                    brJob.ManagePviErrorEvent(pviObj);
                                                    if (SynchroJob != null && SynchroJob == brJob)
                                                    {
                                                        brJob.ResetSynchro.WaitOne(Timeout);
                                                        RemoveSynchroJob(brJob);
                                                        brJob.ResetSynchro.Reset();
                                                    }
                                                    brJob.IsPending = false;
                                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - RemovePending 22 - PVI Obj: {1}",
                                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                                                    ListJobPending.Remove(brJob);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.ManagePviEvents {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                if(lastConnectionErrorCode == 0)
                {
                    CommDriver.OnSystemEvent(null,
                                             String.Format(Properties.Resources.BrExceptionInManagePviEvent, ex.Message, Name),
                                             Opc.Ua.EventSeverity.Max);
                    lastConnectionErrorCode = -1;
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
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ParseStringLength {0} - infoString: {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), infoString.ToString());
                    StringComparison scomp = StringComparison.OrdinalIgnoreCase;
                    // Is the PVI variable a string or a wstring?
                    if((infoString.IndexOf("VT=string", scomp) >= 0) || (infoString.IndexOf("VT=wstring", scomp) >= 0))
                    {
                        // Split the info string for isolating the single parameters
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
                                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ParseStringLength {0} - parsed string length: {1}",
                                                                       DateTime.Now.ToString("HH:mm:ss.fff"), stringLength);
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
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ParseStringLength {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
            }
            return (stringLength);
        }

        void PassReceivedDataToJob(byte[] dataArray, BrPviCommJob brJob)
        {
            if (brJob.Type == LinkType.ExceptionOutput || brJob.Type == LinkType.UnconditionalOutput)
                return;

            if (brJob.pviObjectState != BrPviCommJob.PviObjectState.Activated) //false
            {
                brJob.pviObjectState = BrPviCommJob.PviObjectState.Activated; //true
            }

            try
            {
                brJob.IsRead = brJob.ReadRequest();
                brJob.ExchangedByte = (uint)dataArray.Count();
                brJob.ExchangedTag = 1;
                if ((brJob.Type != LinkType.Input) && (brJob.Type != LinkType.InputOutput))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Called for PVI Tag {1} in output mode",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName));
                    brJob.IsPending = false;
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = brJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    OnJobExecuted(eJob);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Called for PVI Tag {1} in input mode",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName));
                    brJob.IsPending = false;
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = brJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    eJob.Values = dataArray;
                    OnJobExecuted(eJob);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.PassReceivedDataToJob {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
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
            brJob.Status = BrPviCommJobStatus.Idle;

            try
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.PassWriteOKNotificationToJob {0} - Called for: {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
                brJob.IsPending = false;
                brJob.IsRead = false;
                brJob.ExchangedByte = brJob.GetExchangedBytesNumber();
                brJob.ExchangedTag = 1;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = brJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                OnJobExecuted(eJob);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.PassWriteOKNotificationToJob {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                if(lastConnectionErrorCode == 0)
                {
                    CommDriver.OnSystemEvent(null,
                                             String.Format(Properties.Resources.BrExceptionInPassWriteOKNotificationToJob,
                                                           ex.Message,
                                                           brJob.BrPviVariableCompleteName,
                                                           Name),
                                             Opc.Ua.EventSeverity.Max);
                    lastConnectionErrorCode = -1;
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

        public BrPviPviObject GetOrCreatePviObject(String pviobjName, String pviobjConnDescr, String pviobjLinkDescr, PviObjectTypes pviobjType)
        {
            BrPviPviObject requestedPviObject;
            lock (lockmapPviObjects)
            {
                if(!mapPviObjects.TryGetValue(pviobjName, out requestedPviObject))
                {
                    requestedPviObject = new BrPviPviObject();
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
            if (!mapPviObjects.TryGetValue(pviObjName, out requestedPviObject))
            {
                requestedPviObject = null;
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviChannel.GetPviObject {0} - Pvi Obj {1} not found in the general dictionary",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObjName);
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
                // Dictionary of pending PVI object create requests
                if (!mapPendingCreateRequests.ContainsKey(pviObject.Name))
                {
                    mapPendingCreateRequests[pviObject.Name] = DateTime.UtcNow;
                }
            }
        }

        void AddRequestToWriteList(PviWriteObj pviObjectToBeWritten)
        {
            lock (lockmapPviObjects)
            {
                if (!listOfPviWriteRequests.Contains(pviObjectToBeWritten))
                {
                    listOfPviWriteRequests.Add(pviObjectToBeWritten);
                }

                // Dictionary of pending PVI object write requests
                if (!mapPendingWriteRequests.ContainsKey(pviObjectToBeWritten.PviObj.Name))
                {
                    mapPendingWriteRequests[pviObjectToBeWritten.PviObj.Name] = DateTime.UtcNow;
                }
            }
        }

        void AddRequestToWriteMaskList(PviWriteObj pviObjectToBeWritten)
        {
            lock (lockmapPviObjects)
            {
                if (!listOfPviMaskWriteRequests.Contains(pviObjectToBeWritten))
                {
                    listOfPviMaskWriteRequests.Add(pviObjectToBeWritten);
                }
            }
        }
                

        void AddRequestToReadStringLengthList(BrPviPviObject pviObject)
        {
            lock (lockmapPviObjects)
            {
                if (!listOfStringLengthRequests.Contains(pviObject))
                {
                    listOfStringLengthRequests.Add(pviObject);
                }
                // Dictionary of pending string length requests
                if (!mapPendingStringLengthRequests.ContainsKey(pviObject.Name))
                {
                    mapPendingStringLengthRequests[pviObject.Name] = DateTime.UtcNow;
                }
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
                List<BrPviCommJob> jobList;

                if (!mapPviObjJobs.TryGetValue(pviobjName, out jobList))
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

        public BrPviCommJob GetNextSynchroJob()
        {
            BrPviCommJob job = null;

            List<Station> stBrPvi = (from station in CommDriver.GetChannelStations(this).AsParallel() where (station.Name == SynchroJob.Station.Name) select station).ToList();
            if (stBrPvi.Count > 0) { 
                job = SynchroJob as BrPviCommJob;
                job.IsSyncroJob = true;
                ((BrPviStation)stBrPvi[0]).BuildJobPviObjNames(job);
            }

            return job;
        }


        private void RemoveSynchroJob(BrPviCommJob job)
        {
            RemoveJobToPviObjDictionary(job.BrPviVariableCompleteName, job);
            SynchroJob = null;
        }

        public bool AnyPVIObjectOfStationDeactivated(BrPviStation station)
        {
            int NrElements = 0;
            var listJob = station.GetListWholeJobCopy();

            Parallel.ForEach(listJob, job =>                
            {
                BrPviPviObject pviObj;
                if (mapPviObjects.TryGetValue(((BrPviCommJob)job).BrPviVariableCompleteName, out pviObj))
                    NrElements++;
            });

            return (NrElements>0);
        }

        /// <summary>
        /// Disable PVIMonitor's items associated to this station; tag state ACTIVE/DEACTIVE is setted immediatly, without waiting request result
        /// </summary>
        /// <param name="station"></param>
        /// <param name="pviObjectState"></param>
        public void ActivateDeactivateAllPVIObjectOfStation(BrPviStation station, BrPviCommJob.PviObjectState pviObjectState)
        {
            var listJob = station.GetListWholeJobCopy();

            foreach (BrPviCommJob job in listJob)
            {
                BrPviPviObject pviObj;
                if (mapPviObjects.TryGetValue(job.BrPviVariableCompleteName, out pviObj))
                {
                    if (pviObj.Type == PviObjectTypes.POBJ_PVAR)
                    {                        
                        job.SetPviObjectEvMask(job, pviObjectState);

                        PviWriteObj pviWriteObject = new PviWriteObj(pviObj);
                        pviWriteObject.SetEvMaskData(pviObj.EvMask);
                        
                        AddRequestToWriteMaskList(pviWriteObject);                        
                    }
                }
            }
            channelPviSink.WriteMaskPviObjects();
        }
                
        public void ReloadAllActivePVIObjectOfStation(BrPviStation station)
        {
            lock (lockmapPviObjects)
            {
                var listJob = station.GetListWholeJobCopy();

                foreach (BrPviCommJob job in listJob)
                {
                    foreach (BrPviTag tag in job.TagsList)
                    {
                        if (mapGlobalHandles.ContainsKey(job.BrPviVariableCompleteName))
                        {
                            GCHandle handle = mapGlobalHandles[job.BrPviVariableCompleteName];
                            if (handle.IsAllocated)
                            {
                                handle.Free();
                            }

                            mapGlobalHandles.Remove(job.BrPviVariableCompleteName);
                        }

                        // Dictionary of pending string length requests
                        if (mapPendingStringLengthRequests.ContainsKey(job.BrPviVariableCompleteName))
                            mapPendingStringLengthRequests.Remove(job.BrPviVariableCompleteName);

                        //RemovePVIObjectFromMap
                        if (mapPviObjects.ContainsKey(job.BrPviVariableCompleteName))
                            mapPviObjects.Remove(job.BrPviVariableCompleteName);

                        if (mapPviObjJobs[job.BrPviVariableCompleteName].Contains(job))
                            mapPviObjJobs[job.BrPviVariableCompleteName].Remove(job);
                    }

                    //RemoveStringLengthRequests(BrPviStation station)
                    listOfStringLengthRequests.RemoveAll(a => a.Name == job.BrPviVariableCompleteName);

                    job.pviObjectState = BrPviCommJob.PviObjectState.NotInitialized;
                }
            }
        }

        //public void RemoveAllPVIObjectFromStation(BrPviStation station)
        //{
        //    lock (lockmapPviObjects)
        //    {
        //        var listJob = station.GetListWholeJobCopy();

        //        foreach (BrPviCommJob job in listJob)
        //        {
        //            foreach (BrPviTag tag in job.TagsList)
        //            {
        //                if (mapGlobalHandles.ContainsKey(job.BrPviVariableCompleteName))
        //                {
        //                    GCHandle handle = mapGlobalHandles[job.BrPviVariableCompleteName];
        //                    if (handle.IsAllocated)
        //                    {
        //                        handle.Free();
        //                    }

        //                    mapGlobalHandles.Remove(job.BrPviVariableCompleteName);
        //                }

        //                // Dictionary of pending string length requests
        //                if (mapPendingStringLengthRequests.ContainsKey(job.BrPviVariableCompleteName))
        //                    mapPendingStringLengthRequests.Remove(job.BrPviVariableCompleteName);

        //                //RemovePVIObjectFromMap
        //                if (mapPviObjects.ContainsKey(job.BrPviVariableCompleteName))
        //                    mapPviObjects.Remove(job.BrPviVariableCompleteName);

        //                if (mapPviObjJobs[job.BrPviVariableCompleteName].Contains(job))
        //                    mapPviObjJobs[job.BrPviVariableCompleteName].Remove(job);
        //            }

        //            //RemoveStringLengthRequests(BrPviStation station)
        //            listOfStringLengthRequests.RemoveAll(a => a.Name == job.BrPviVariableCompleteName);

        //            job.pviObjectState = BrPviCommJob.PviObjectState.DeActivated;
        //        }
        //    }
        //}
        #endregion

        #region Override Methods
        public override void CalculateJobStatistic(CommJob job)
        {
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

#endregion

    }
}

