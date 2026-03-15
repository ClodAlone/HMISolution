using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.GroupValues;
using Knx.Falcon.Sdk;
using Amib.Threading;
using Opc.Ua;
using System.Threading.Tasks;

namespace EIB
{
    class EIBChannel : Channel
    {
        public const int EIB_EXTRA_TIMEOUT = 200;
        public const int TIMEOUT_CHECK_CONNECTION = 5000;
        public const int REPETE_CHECK_CONNECTION = 6;
        #region Constructors

        /// <summary>
        /// Initializes the EIBChannel object.
        /// </summary>
        public EIBChannel(CommunicationDriver commdriver, EIBChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _MinBusInactivityTime = settings.MinBusInactivityTime;
            _SerialCommPort = settings.SerialCommPort;
            _ConnectorType = settings.ConnectorType;
            _MulticastAddress = settings.MulticastAddress;
            _EIBNetFriendlyName = settings.EIBNetFriendlyName;
            _LocalHostName = settings.LocalHostName;
            _EIBNetIpAddress = settings.EIBNetIpAddress;
            _EIBNetTcpPort = settings.EIBNetTcpPort;
            _ConnectionResetTime = settings.ConnectionResetTime;
            ConnectionIsBroken = false;
            executedJob = null;
            notRepeteConnection = false;
        }

        #endregion

        #region Overrides
        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            List<Tag> tl = (from t in job.TagsList.AsParallel()
                            where (t.DynSettings.MethodID != -1)
                            select t).ToList();

            bool exclude = (tl.Count == job.TagsList.Count);

            if (!exclude)
            {
                base.SubscribeJob(job, state);
            }
        }

        public override void ExecuteJob(CommJob job)
        {
            if (executedJob != null) return;
            EIBCommJob ej = job as EIBCommJob;
            if (ej == null) return;

            // Check if it is elapsed enough time from the last sent request
            if ((DateTime.UtcNow - LastSentRequestTime).TotalMilliseconds > MinBusInactivityTime)
            {
                // Input request
                if (((ej.Type == DriverCodeBase.Enumerators.LinkType.Input) ||
                     (ej.Type == DriverCodeBase.Enumerators.LinkType.InputOutput)) &&
                     ej.PollingCanBePerformed())
                {
                    if (ej.EnableOnlyInitialPolling && !ej.RetryInitialPolling)
                    {
                        ej.PollingEnabled = false;
                    }

                    // Check if the polling address is valid
                    if (EIBCommJob.IsValidAddress(ej.PollingGroup))
                    {
                        lock (ej.retLockList())
                        {
                            base.ExecuteJob(job);
                        }

                        // Be sure the that the job has been inserted in the dictionary of the input groups
                        AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(ej.PollingGroup), ej);

                        ReadAsynchronous(ej);
                    }
                }

                // Output request
                else if ((ej.Type != DriverCodeBase.Enumerators.LinkType.Input) &&
                         ej.WriteCanBePerformed())
                {
                    // Check if the output address is valid
                    if (EIBCommJob.IsValidAddress(ej.OutputGroup))
                    {
                        lock (ej.retLockList())
                        {
                            base.ExecuteJob(job);
                        }

                        // Be sure the that the job has been inserted in the dictionary of the output groups
                        UInt16 convertedAddress = EIBCommJob.EibGroupAddressToUInt16(ej.OutputGroup);
                        AddToMapEIBOutputGroupAddJobs(convertedAddress, ej);

                        LastErrorCode = DriverErrorCodes.ErrorNoError;
                        // Send the write request
                        WriteAsynchronous(ej);
                    }
                }
            }
            else
            {
                // Still waiting for sending the request 
                ej.Status = EibCommJobStatus.WaitingToSendRequest;
                ej.StartExecutionTime = DateTime.UtcNow;
            }
        }
        #endregion

        #region Specific Methods
 
        private bool IsConnectionBrokenTimeoutElapsed()
        {
            if(LastConnectionOkTime == DateTime.MinValue)
            {
                return (true);
            }

            double dtime = (DateTime.UtcNow - LastConnectionOkTime).TotalMilliseconds;

            if(dtime > ConnectionResetTime*1000)
            {
                return (true);
            }

            return (false);
        }
        private bool IsCheckCommunicationTimeoutElapsed()
        {
            if (LastCheckCommunicationTime == DateTime.MinValue)
            {
                return (true);
            }

            double dtime = (DateTime.UtcNow - LastCheckCommunicationTime).TotalMilliseconds;

            if (dtime > TIMEOUT_CHECK_CONNECTION + Timeout)
            {
                return (true);
            }

            return (false);
        }

        public void ReadAsynchronous(EIBCommJob ej)
        {
            if (stopTestPollingQueue)
                return;
            if (executedJob != null)
                return;
            executedJob = ej;
            executedJob.Status = EibCommJobStatus.ReadRequestPending;
            SmartThreadPool.QueueWorkItem(() =>
            {
                lock (lockThreadObject)
                {
                    // Send the read request
                    GroupAddress address;
                    try
                    {
                        address = new GroupAddress(executedJob.PollingGroup);
                        if (_bus == null)
                            throw new ArgumentException("Error Driver Not Open");
                        _bus.ReadValue(address, Priority.Low, Timeout);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                           System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - ReadAs ret f {0} Pol Gr {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.PollingGroup));
                            if (ListJobPending.Contains(executedJob))
                            {
                                LastErrorCode = (DriverErrorCodes)EIB_ERROR_CODES.DeviceFalconErrorTimeOut;
                                executedJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
                                executedJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitReadError);
                                executedJob.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == executedJob)
                                {
                                    executedJob.ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    executedJob.ResetSynchro.Reset();
                                }
                                ProcessError(executedJob, LastErrorCode);
                                ListJobPending.Remove(executedJob);
                            }
                        }
                        catch
                        {
                        }
                        executedJob = null;
                        return;
                    }

                    try
                    {
                        executedJob.LastReadRequestTimeStamp = LastSentRequestTime;
                        executedJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
                       System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - ReadAs ret t {0} Pol Gr {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.PollingGroup));
                        LastErrorCode = DriverErrorCodes.ErrorNoError;
                        LastSentRequestTime = DateTime.UtcNow;
                    }
                    catch
                    {
                    }
                    executedJob.ErrorCode = (EIB_ERROR_CODES)LastErrorCode;
                    executedJob = null;
                }
            });
        }

        public void WriteAsynchronous(EIBCommJob ej)
        {
            if (stopTestPollingQueue)
                return;
            if (executedJob != null)
                return;
            executedJob = ej;
            executedJob.Status = EibCommJobStatus.WriteRequestPending;
            SmartThreadPool.QueueWorkItem(() =>
            {
                lock (lockThreadObject)
                {
                    if (_bus == null)
                    {
                       System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - Write ret f {0} Out Gr {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.OutputGroup));
                        LastErrorCode = (DriverErrorCodes)(EIB_ERROR_CODES.DeviceWriteErrorDriverNotOpen);
                    }
                    else
                    {
                        try
                        {
                            // Get the data to be written
                            object writingData = null;
                            executedJob.GetJobData(ref writingData);
                           System.Diagnostics.Debug.WriteLine(string.Format("EIB DBG - Write ret t {0} Out Gr {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.OutputGroup));
                            LastSentRequestTime = DateTime.UtcNow;
                            if (writingData != null)
                            {
                                if (executedJob.DataFormat != (int)EISDATAFORMAT.EISDFBit)
                                    _bus.WriteValue(new GroupAddress(executedJob.OutputGroup), new GroupValue(writingData as byte[]));
                                else
                                    _bus.WriteValue(new GroupAddress(executedJob.OutputGroup), new GroupValue(new SixBit((writingData as byte[])[0])));
                            }
                            LastErrorCode = DriverErrorCodes.ErrorNoError;
                        }
                        catch (Exception ex)
                        {
                           System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - Write ret f {0} Out Gr {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.OutputGroup));
                            LastErrorCode = (DriverErrorCodes)(EIB_ERROR_CODES.DeviceWriteErrorWriteError);
                        }
                    }
                    try
                    {
                        executedJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceWrite);
                        executedJob.SetConditionalVariableBit(LastErrorCode != DriverErrorCodes.ErrorNoError, (ushort)EIBConditionalVariableBits.BitWriteError);
                    }
                    catch
                    {
                    }


                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = executedJob;
                    eJob.ErrorCode = LastErrorCode;
                   System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - {0} AnNewWrDone call OnJobExd for {1}",
                                                  DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.OutputGroup));
                    OnJobExecuted(eJob);
                    executedJob = null;
                }
            });

        }

        public void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode)
        {
            EIBCommJob EibPendingJob = (EIBCommJob)pendingjob;
            
            if ((EibPendingJob.RetryOutput == false))
            {
                lock (pendingjob.retLockList())
                {
                    EibPendingJob.TagsListOnWriting.Clear();
                }
            }
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = EibPendingJob;
            eJob.ErrorCode = errorCode;
           System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - {0} ProcErr call OnJobExd for {1}",
                                          DateTime.Now.ToString("HH:mm:ss.fff"), EibPendingJob.OutputGroup));
            OnJobExecuted(eJob);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected EIBCommJob GetNextEibPendingJob()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    while (!queue.IsEmpty)
                    {
                        CommJob j = null;
                        if(queue.TryDequeue(out j) == true)
                        {
                            EIBCommJob ej = (EIBCommJob)j;
                            if (ej != null)
                            {
                                if (ej.MustSendARequest())
                                {
                                    return (ej);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public void AddToMapNewData(GroupValue value, GroupAddress address)
        {
            lock (lockmapEIBNewData)
            {
                mapEIBNewData[(UInt16)address] = value;
            }
        }


        public void AnalyzeNewData()
        {
            Dictionary<UInt16, GroupValue> mapEIBNewDataTmp = new Dictionary<UInt16, GroupValue>();
            lock (lockmapEIBNewData)
            {
                // No new data?
                if(mapEIBNewData.Count == 0)
                {
#if DEBUG
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("EIB DBG - {0} AnNewDa ret 1", curTimeTxt);
                    System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                    return;
                }

                // Make a copy of the new data
                mapEIBNewDataTmp = new Dictionary<ushort, GroupValue>(mapEIBNewData);
                // Clear the dictionary of the new data
                mapEIBNewData.Clear();
            }

            // Process new data
            List<EIBCommJob> listJobs;
            foreach (KeyValuePair<UInt16, GroupValue> item in mapEIBNewDataTmp)
            {
                if(item.Value == null)
                {
#if DEBUG
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("EIB DBG - {0} AnNewDa con 1", curTimeTxt);
                    System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                    continue;
                }
                // Get the list of jobs associated to the EIB address
                listJobs = GetJobListForNewData(item.Key);
                if (listJobs.Count == 0)
                {
#if DEBUG
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("EIB DBG - {0} AnNewDa con 2", curTimeTxt);
                    System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                    continue;
                }

                // Process new data
                foreach(var jobVal in listJobs)
                {
                    EIBCommJob eibJob = (EIBCommJob)jobVal;
                    if(eibJob == null)
                    {
#if DEBUG
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("EIB DBG - {0} AnNewDa con 3", curTimeTxt);
                        System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                        continue;
                    }
                    if (eibJob.ErrorCode == (EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError)
                        eibJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitNewData);


                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = eibJob;
                    eJob.ErrorCode = (DriverErrorCodes)eibJob.ErrorCode;
                    eJob.Values = item.Value;
                    eJob.Job.IsRead = true;
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - {0} AnNewDa call OnJobExd for {1}",
                                                  DateTime.Now.ToString("HH:mm:ss.fff"), new GroupAddress(item.Key).ToString()));

                    OnJobExecuted(eJob);
                }
            }
        }

        public void AddToMapEIBInputGroupAddJobs(UInt16 GroupAddress, EIBCommJob job)
        {
            if (GroupAddress > 0)
            {
                lock (lockAddressMaps)
                {
                    if (!mapEIBInputGroupAddJobs.ContainsKey(GroupAddress))
                    {
                        List<EIBCommJob> ListJobs = new List<EIBCommJob>();
                        ListJobs.Add(job);
                        mapEIBInputGroupAddJobs[GroupAddress] = ListJobs;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String dbgMsg = String.Format("EIB DBG - {0} AddToMapEIBInGr add key {1}",
                                                      dbgTime, GroupAddress);
                        System.Diagnostics.Debug.WriteLine(dbgMsg);
#endif
                    }
                    else if (!mapEIBInputGroupAddJobs[GroupAddress].Contains(job))
                    {
                        mapEIBInputGroupAddJobs[GroupAddress].Add(job);
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String dbgMsg = String.Format("EIB DBG - {0} AddToMapEIBInGr add j to key {1}",
                                                      dbgTime, GroupAddress);
                        System.Diagnostics.Debug.WriteLine(dbgMsg);
#endif
                    }
                }

            }
        }

        public void AddToMapEIBOutputGroupAddJobs(UInt16 GroupAddress, EIBCommJob job)
        {
            lock (lockAddressMaps)
            {
                if (!mapEIBOutputGroupAddJobs.ContainsKey(GroupAddress))
                {
                    List<EIBCommJob> ListJobs = new List<EIBCommJob>();
                    ListJobs.Add(job);
                    mapEIBOutputGroupAddJobs[GroupAddress] = ListJobs;
#if DEBUG
                    String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    String dbgMsg = String.Format("EIB DBG - {0} AddToMapEIBOutGr add key {1}",
                                                  dbgTime, GroupAddress);
                    System.Diagnostics.Debug.WriteLine(dbgMsg);
#endif
                }
                else if (!mapEIBOutputGroupAddJobs[GroupAddress].Contains(job))
                {
                    mapEIBOutputGroupAddJobs[GroupAddress].Add(job);
#if DEBUG
                    String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    String dbgMsg = String.Format("EIB DBG - {0} AddToMapEIBOutGr add j to key {1}",
                                                  dbgTime, GroupAddress);
                    System.Diagnostics.Debug.WriteLine(dbgMsg);
#endif
                }
            }
        }

        public List<EIBCommJob> GetJobListForNewData(int groupAddress)
        {
            List<EIBCommJob> jobList;
            lock (lockAddressMaps)
            {
                UInt16 Address = (UInt16)groupAddress;
                if (mapEIBInputGroupAddJobs.ContainsKey(Address))
                    jobList = new List<EIBCommJob>(mapEIBInputGroupAddJobs[Address]);
                else
                    jobList = new List<EIBCommJob>();
            }
            return jobList;
        }

        public void ManageConnectionRestored()
        {
            ConnectionIsBroken = false;
            CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionEstablished, EventSeverity.High);
            LastConnectionOkTime = DateTime.UtcNow;

            foreach (var station in CommDriver.GetChannelStations(this))
            {
                // Set the error state of the jobs
                EIBStation eibStation = (EIBStation)station;
                eibStation.ManageConnectionRestored();
            }
        }

        public void ManageConnectionBroken(List<CommJob> jobList)
        {
            try
            {
                ConnectionIsBroken = true;
                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorConnectionBroken, EventSeverity.High);

                Parallel.ForEach(jobList, job =>
                {
                    // Deactivate the job
                    EIBCommJob eibJob = (EIBCommJob)job;
                    if ((eibJob.Status == EibCommJobStatus.ReadRequestPending) ||
                       (eibJob.Status == EibCommJobStatus.WriteRequestPending))
                    {
                        eibJob.Status = EibCommJobStatus.Idle;
                    }
                });

                foreach (var station in CommDriver.GetChannelStations(this))
                {
                    // Set the error state of the jobs
                    EIBStation eibStation = (EIBStation)station;
                    eibStation.ManageConnectionBroken();
                }

            }
            catch(Exception ex)
            {
                ConnectionIsBroken = true;
            }
        }
        #endregion

        #region Abstracts Methods

        public override bool IsDeviceOpen()
        {
            bool returnValue; 
            if (_bus == null)
            {
                returnValue = false;
            }
            else if (ConnectionIsBroken)
            {
                returnValue = false;
            }
            else
                returnValue = _bus.IsConnected;
            if (!returnValue)
            {
                if (!IsConnectionBrokenTimeoutElapsed())
                {
                    returnValue = true;
                }
            }
            else
            {
                LastConnectionOkTime = DateTime.UtcNow;
            }

            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        public override bool DeviceOpen()
        {
            bool returnValue = false;
            if (_bus == null)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - CreateCommunicationBus {0} ",
                        DateTime.Now.ToString("HH:mm:ss.fff")));
                    _bus = CreateCommunicationBus();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - create bus Exception in DeviceOpen at {0}, Message: {1} ",
                        DateTime.Now.ToString("HH:mm:ss.fff"), exception.Message));
                }
            }

            if (_bus != null)
            {
                if(notRepeteConnection)
                {
                    returnValue = true;
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                        _bus.GroupValueReceived -= BusOnGroupValueReceived;
                        _bus.StateChanged -= BusOnStateChanged;
                        _bus.Connect();
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect Exception in DeviceOpen at {0}, Message: {1} ",
                            DateTime.Now.ToString("HH:mm:ss.fff"), exception.Message));
                        if (_bus != null)
                            _bus.Disconnect();
                    }
                    if (_bus != null && _bus.IsConnected)
                    {
                        _bus.GroupValueReceived += BusOnGroupValueReceived;
                        _bus.StateChanged += BusOnStateChanged;
                        LastConnectionOkTime = DateTime.UtcNow;
                        LastCheckCommunicationTime = DateTime.UtcNow;
                        CheckCommunicationRequest = 0;

                        CommDriver.OnSystemEvent(null, Properties.Resources.CreateCommunicationBus, EventSeverity.High);
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect OK {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                        ConnectionBroken.Reset();
                        ConnectionRestored.Reset();
                        returnValue = true;
                        if (ConnectorType == ConnectorTypes.KnxIpRouting)
                        {
                            notRepeteConnection = true;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect failed {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                    }
                }
            }
            LastDeviceOpenAttempt = DateTime.UtcNow;
            return (returnValue);
        }

        private void BusOnGroupValueReceived(GroupValueEventArgs e)
        {
            if (e != null)
            {
                lock (lockThreadObject)
                {
                   System.Diagnostics.Debug.WriteLine(string.Format(
                                   "Address {0}, IndividualAddress {1}, TelegramPriority {2}, Value {3}",
                                   e.Address,
                                   e.IndividualAddress,
                                   e.TelegramPriority,
                                   e.Value));
                    AddToMapNewData(e.Value, e.Address);
                    SetNewDataEvent();
                    LastCheckCommunicationTime = DateTime.UtcNow;
                    CheckCommunicationRequest = 0;
                    ConnectionRestored.Set();
                }
            }
        }

        private void BusOnStateChanged(BusConnectionStatus busConnectionStatus)
        {
           System.Diagnostics.Debug.WriteLine(string.Format("Connection state: {0}", busConnectionStatus));
            if(busConnectionStatus == BusConnectionStatus.Broken || busConnectionStatus == BusConnectionStatus.Closed)
            {
                ConnectionBroken.Set();
                ConnectionClose.Set();

            }
            else if(busConnectionStatus == BusConnectionStatus.Connected)
            {
                ConnectionRestored.Set();
                lock (lockThreadObject)
                {
                    LastCheckCommunicationTime = DateTime.UtcNow;
                    CheckCommunicationRequest = 0;
                }

            }

        }


        public override bool DeviceClose()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - DeviceClose start {0} ",
                DateTime.Now.ToString("HH:mm:ss.fff")));
            if (_bus != null)
            {
                if (ConnectionIsBroken == false)
                {
                    ManageConnectionBroken(ListJobPending);
                    LastConnectionOkTime = DateTime.UtcNow;
                }

                ConnectionClose.Reset();
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Disconnect() {0} ",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
                _bus.Disconnect();
                if (ConnectionClose.WaitOne(500))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG -  ConnectionClose.Reset() {0} ",
                        DateTime.Now.ToString("HH:mm:ss.fff")));
                    ConnectionClose.Reset();
                }
                _bus.GroupValueReceived -= BusOnGroupValueReceived;
                _bus.StateChanged -= BusOnStateChanged;
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Dispose() {0}",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
                _bus.Dispose();
                _bus = null;
                CommDriver.OnSystemEvent(null, Properties.Resources.CloseCommunicationBus, EventSeverity.High);
                ConnectionBroken.Reset();
                ConnectionRestored.Reset();
            }
            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - DeviceClose end {0} ",
                DateTime.Now.ToString("HH:mm:ss.fff")));
            executedJob = null;
            ListJobPending.Clear();
            return true;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Override Methods
        public override bool TestChannelComm()
        {
            return InitCheckBusCommunication();
        }

        public override bool Startup()
        {

            lock (lockThreadObject)
            {
                if(ConnectionBroken == null)
                {
                    ConnectionBroken = new ManualResetEvent(false);
                }
                else
                {
                    ConnectionBroken.Reset();
                }

                if (ConnectionRestored == null)
                {
                    ConnectionRestored = new ManualResetEvent(false);
                }
                else
                {
                    ConnectionRestored.Reset();
                }
                if (ConnectionClose == null)
                {
                    ConnectionClose = new ManualResetEvent(false);
                }
                else
                {
                    ConnectionClose.Reset();
                }
            }

            return (base.Startup());
        }
        public Bus CreateCommunicationBus()
        {
            Bus outBus = null;
            try
            {
                //Calls the falcon method
                ConnectorParameters connectorParameter;
                switch (_ConnectorType)
                {
                    case ConnectorTypes.KnxIpRouting:
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - CreateCommunicationBus start {0} ",
                               DateTime.Now.ToString("HH:mm:ss.fff")));
                            connectorParameter = new KnxIpRoutingConnectorParameters(_MulticastAddress, new IndividualAddress("0.0.1"));
                            (connectorParameter as KnxIpRoutingConnectorParameters).IpPort = (ushort)_EIBNetTcpPort;
                            if (!string.IsNullOrEmpty(_LocalHostName))
                                (connectorParameter as KnxIpRoutingConnectorParameters).LocalAddress = _LocalHostName;
                            break;
                        }
                    case ConnectorTypes.KnxIpTunneling:
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - CreateCommunicationBus start {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            connectorParameter = new KnxIpTunnelingConnectorParameters(_EIBNetIpAddress, (ushort)_EIBNetTcpPort, false);
                            break;
                        }
                    default:
                        {
                            connectorParameter = new KnxIpRoutingConnectorParameters("224.0.23.12", new IndividualAddress("0.0.1"));
                            (connectorParameter as KnxIpRoutingConnectorParameters).IpPort = (ushort)_EIBNetTcpPort;
                            if (!string.IsNullOrEmpty(_LocalHostName))
                                (connectorParameter as KnxIpRoutingConnectorParameters).LocalAddress = _LocalHostName;
                            break;
                        }
                }

                //connectorParameter.Name = EIBNetFriendlyName; //It has no relevance when opening a connection.
                outBus = new Bus(connectorParameter);
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - CreateCommunicationBus OK {0} ",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - CreateCommunicationBus err {0} exception {1} ",
                    DateTime.Now.ToString("HH:mm:ss.fff"), exception.Message));
                if (outBus != null)
                {
                    outBus.Dispose();
                    outBus = null;
                }
            }
            return outBus;
        }

        public bool InitCheckBusCommunication()
        {
            bool rt = false;
            Bus sut;
            switch (_ConnectorType)
            {
                case ConnectorTypes.KnxIpRouting:
                    sut = CreateCommunicationBus();
                    if (sut != null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.Connect {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            sut.Connect();
                            int cnt = 5;
                            do
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication start {0} ",
                                    DateTime.Now.ToString("HH:mm:ss.fff")));
                                CheckCommunicationResult Result = sut.CheckCommunication();
                                if (Result == CheckCommunicationResult.Ok)
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication OK {0} ",
                                       DateTime.Now.ToString("HH:mm:ss.fff")));
                                    rt = true;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting -initial bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
                                        DateTime.Now.ToString("HH:mm:ss.fff"), Result));
                                    cnt--;
                                }
                            } while (rt != true && cnt != 0);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication exception {0} ",
                               DateTime.Now.ToString("HH:mm:ss.fff")));
                            break;
                        }

                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.Disconnect {0} ",
                             DateTime.Now.ToString("HH:mm:ss.fff")));
                        sut.Disconnect();
                        sut.Dispose();
                        sut = null;
                    }

                    break;
                case ConnectorTypes.KnxIpTunneling:
                    sut = CreateCommunicationBus();
                    if (sut != null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.Connect {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            sut.Connect();

                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication start {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            CheckCommunicationResult Result = sut.CheckCommunication();
                            if (Result == CheckCommunicationResult.Ok)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication OK {0} ",
                                   DateTime.Now.ToString("HH:mm:ss.fff")));
                                rt = true;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling -initial bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
                                    DateTime.Now.ToString("HH:mm:ss.fff"), Result));
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication exception {0} ",
                               DateTime.Now.ToString("HH:mm:ss.fff")));
                            break;
                        }
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.Disconnect {0} ",
                             DateTime.Now.ToString("HH:mm:ss.fff")));
                        sut.Disconnect();
                        sut.Dispose();
                        sut = null;
                    }
                    break;
            }

            return rt;
        }
        public bool CheckBusCommunication()
        {
            bool rt = false;
            switch (_ConnectorType)
            {
                case ConnectorTypes.KnxIpRouting:
                    if (_bus != null)
                    {
                        try
                        {
                            if (CheckCommunicationRequest < REPETE_CHECK_CONNECTION)
                                rt = true;
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication start {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            CheckCommunicationResult Result = _bus.CheckCommunication();
                            if (Result != CheckCommunicationResult.Ok)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
                                    DateTime.Now.ToString("HH:mm:ss.fff"), Result));

                                CheckCommunicationRequest++;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication OK {0} ",
                                   DateTime.Now.ToString("HH:mm:ss.fff")));
                                CheckCommunicationRequest = 0;
                                rt = true;
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication exception {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            CheckCommunicationRequest++;
                        }
                        if (!rt)
                            CheckCommunicationRequest = REPETE_CHECK_CONNECTION;
                    }

                    break;
                default:
                    rt = true;
                    break;

            }

            return rt;
        }

        public bool InErrorState()
        {
            bool InErrorState = false;
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                if (station.InErrorState && station.GetChannel() == this)
                {
                    InErrorState = true;
                    break;
                }
            }
            return InErrorState;
        }

        DateTime LastDeviceOpenAttempt = DateTime.MinValue;

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            LastDeviceOpenAttempt = DateTime.MinValue;

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            ConnectionBroken.Reset();
            ConnectionRestored.Reset();
            bool isDeviceOpen =  DeviceOpen();
            if (_ConnectorType == ConnectorTypes.KnxIpRouting)
            {
                LastCheckCommunicationTime = DateTime.UtcNow;
                if (_bus == null || !CheckBusCommunication())
                {
                    isDeviceOpen = false;
                }
            }
            ConnectionIsBroken = !isDeviceOpen;

            while (true)
            {
                CommJob nextjob = null;
                bool CheckCommunication = false;
                lock (lockThreadObject)
                {
                    isDeviceOpen = IsDeviceOpen();
                    if (!isDeviceOpen && ((DateTime.UtcNow - LastDeviceOpenAttempt).TotalMilliseconds > Properties.Settings.Default.ConnectionRepeatDelay))
                    {
                        isDeviceOpen = DeviceOpen();
                    }
                    if (isDeviceOpen)
                    {
                        CheckCommunication = (executedJob == null && _ConnectorType == ConnectorTypes.KnxIpRouting &&
                            (CheckCommunicationRequest != 0 || IsCheckCommunicationTimeoutElapsed()));
                    }
                    else
                    {
                        List<Station> stationList = CommDriver.GetChannelStations(this);
                        foreach (Station station in stationList)
                        {
                            // Set the error state of the station and of its jobs
                            EIBStation eibStation = (EIBStation)station;
                            eibStation.ManageConnectionBroken();
                        }
                    }
                }

                if (CheckCommunication)
                {
                    LastCheckCommunicationTime = DateTime.UtcNow;
                    if (_bus == null || !CheckBusCommunication())
                    {
                        isDeviceOpen = false;
                        ConnectionBroken.Set();
                    }
                    else
                        ConnectionRestored.Set();
                }

                lock (lockThreadObject)
                {

                    // Connection broken?
                    if (ConnectionBroken.WaitOne(0))
                    {
                        ConnectionBroken.Reset();
                        if (ConnectionIsBroken == false)
                        {
                            ManageConnectionBroken(ListJobPending);
                            LastConnectionOkTime = DateTime.UtcNow;
                        }
#if DEBUG
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("EIB DBG - {0} WrThr Ev ConnBrok recv",
                                                      curTimeTxt);
                        System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                    }

                    // Connection restored?
                    if (ConnectionRestored.WaitOne(0))
                    {
                        ConnectionRestored.Reset();
                        if (ConnectionIsBroken)
                            ManageConnectionRestored();
#if DEBUG
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("EIB DBG - {0} WrThr Ev ConnRest recv",
                                                      curTimeTxt);
                        System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                    }


                    // New data received?
                    if (NewDataToAnlyze.WaitOne(0))
                    {
                        NewDataToAnlyze.Reset();
#if DEBUG
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("EIB DBG - {0} WrThr Ev NewDaToAn recv",
                                                      curTimeTxt);
                        System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                        LastConnectionOkTime = DateTime.UtcNow;
                        // Pass new data to the jobs
                        AnalyzeNewData();
                    }

                    if (CheckCommunicationRequest == 0 && !ConnectionIsBroken && executedJob == null)
                    {
                        if (ListJobPending.Count > 0)
                        {
                            foreach (var job in ListJobPending)
                            {
                                EIBCommJob eibJob = (EIBCommJob)job;
#if DEBUG
                                String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                                String DbgTxt = String.Format("EIB DBG - {0} WrThr j {1} - {2} - {3} st = {4}",
                                                              curTimeTxt,
                                                              eibJob.PollingGroup,
                                                              eibJob.InputGroups,
                                                              eibJob.OutputGroup,
                                                              eibJob.Status);
                                System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                                if (eibJob.Status == EibCommJobStatus.Idle)
                                {
                                    ListJobExecuted.Add(job);
                                }
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
                        if (ListJobPending.Count == 0)
                        {
                            ScheduleListJob();
                            if (SynchroJob != null)
                            {
                                nextjob = SynchroJob;
                            }
                            else
                            {
                                //nextjob = GetNextPendingJob();
                                nextjob = GetNextEibPendingJob();
                            }
                            if (nextjob != null)
                                ListJobPending.Add(nextjob);
                        }
                        else if (ListJobPending.Count > 0)
                        {
                            EIBCommJob ej = (EIBCommJob)ListJobPending[0];
                            if ((ej != null) && (ej.Status == EibCommJobStatus.WaitingToSendRequest))
                            {
                                nextjob = ListJobPending[0];
                            }
                        }
                        if (nextjob != null)
                        {
                            if (isDeviceOpen)
                                ExecuteJob(nextjob);
                        }
                    }

                    if (ListJobPending.Count > 0)
                    {
                        double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                        if (dtime > Timeout + EIB_EXTRA_TIMEOUT)
                        {
                            EIBCommJob exJob = ((EIBCommJob)ListJobPending[0]);
                            LastErrorCode = (DriverErrorCodes)(EIB_ERROR_CODES.DeviceFalconErrorTimeOut);
#if DEBUG
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("EIB DBG - {0} WrThr call PrErr {1}", curTimeTxt, exJob.PollingGroup);
                            System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                            if (exJob != null)
                            {
                                if (exJob.TagsListOnWriting.Count > 0)
                                {
                                    //error on writing
                                    exJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceWrite);
                                    exJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitWriteError);
                                }
                                else
                                {
                                    //error on reading
                                    exJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
                                    exJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitReadError);
                                }
                            }


                            ProcessError(exJob, LastErrorCode);
                            exJob.LastExecutionTime = DateTime.UtcNow;
                            if (SynchroJob != null && SynchroJob == exJob)
                            {
                                exJob.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                exJob.ResetSynchro.Reset();
                            }
                            ListJobPending.Remove(exJob);
                        }
                    }
                }

                if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
                StopWorkerThread.WaitOne(0);
            }
        }
        #endregion

        #region Data Members
        private Bus _bus = null;

        protected object lockmapEIBNewData = new object();
        protected object lockmapEIBErrors = new object();
        protected object lockmapEIBWriteDone = new object();
        protected object lockAddressMaps = new object();
        /// <summary> Event connection broken. </summary>
        protected ManualResetEvent ConnectionBroken;
        /// <summary> Event connection restored. </summary>
        protected ManualResetEvent ConnectionRestored;

        protected ManualResetEvent ConnectionClose;

        // Group Address Dictionaries
        public Dictionary<UInt16, GroupValue> mapEIBNewData = new Dictionary<UInt16, GroupValue>();
        public Dictionary<UInt16, List<EIBCommJob>> mapEIBInputGroupAddJobs = new Dictionary<UInt16, List<EIBCommJob>>();
        public Dictionary<UInt16, List<EIBCommJob>> mapEIBOutputGroupAddJobs = new Dictionary<UInt16, List<EIBCommJob>>();

        DateTime LastSentRequestTime = new DateTime();
        DateTime LastConnectionOkTime = new DateTime();
        DateTime LastCheckCommunicationTime = new DateTime();
        int CheckCommunicationRequest;
        bool ConnectionIsBroken;
        bool notRepeteConnection;

        private EIBCommJob executedJob = null;

        #endregion

        #region Properties
 
        private uint _MinBusInactivityTime;
        public uint MinBusInactivityTime
        {
            get { return _MinBusInactivityTime; }
            set { _MinBusInactivityTime = value; }
        }

        private COMMPORT _SerialCommPort;
        public COMMPORT SerialCommPort
        {
            get { return _SerialCommPort; }
            set
            {
                _SerialCommPort = value;
            }
        }

        private ConnectorTypes _ConnectorType;
        public ConnectorTypes ConnectorType
        {
            get { return _ConnectorType; }
            set
            {
                _ConnectorType = value;
            }
        }

        private string _MulticastAddress;
        public string MulticastAddress
        {
            get { return _MulticastAddress; }
            set
            {
                _MulticastAddress = value;
            }
        }

        private string _EIBNetFriendlyName;
        public string EIBNetFriendlyName
        {
            get
            {
                return _EIBNetFriendlyName;
            }
            set
            {
                _EIBNetFriendlyName = value;
            }
        }

        private string _LocalHostName;
        public string LocalHostName
        {
            get
            {
                return _LocalHostName;
            }
            set
            {
                _LocalHostName = value;
            }
        }

        private string _EIBNetIpAddress;
        public string EIBNetIpAddress
        {
            get
            {
                return _EIBNetIpAddress;
            }
            set
            {
                _EIBNetIpAddress = value;
            }
        }

        private uint _EIBNetTcpPort;
        public uint EIBNetTcpPort
        {
            get
            {
                return _EIBNetTcpPort;
            }
            set
            {
                _EIBNetTcpPort = value;
            }
        }

        private uint _ConnectionResetTime;
        public uint ConnectionResetTime
        {
            get
            {
                return _ConnectionResetTime;
            }
            set
            {
                _ConnectionResetTime = value;
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
                return (CommDriver as EIBDriver).SmartThreadPool;
            }
        }
        #endregion

        public override void Dispose()
        {
            DeviceClose();
            base.Dispose();
        }
    }

}
