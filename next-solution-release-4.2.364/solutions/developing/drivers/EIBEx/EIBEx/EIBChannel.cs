using System;
using System.Collections.Generic;
using System.Threading;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.GroupValues;
using Knx.Falcon.Sdk;
using Amib.Threading;
using Opc.Ua;
using static EIB.EIBProtocol;

namespace EIB
{
    class EIBChannel : Channel
    {
        #region Constructors
        /// <summary>
        /// Initializes the EIBChannel object.
        /// </summary>
        public EIBChannel(CommunicationDriver commdriver, EIBChannelSettings settings)
            : base(commdriver, settings)
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
            connectionIsBroken = false;
            notRepeteConnectionKnxIpRouting = false;
        }

        #endregion

        #region Connection's check timer management

        private void StartTmrCheckDeviceCyclically()
        {
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclically == null)
                    _TmrCheckConnectionCyclically = new Timer(CheckDeviceCyclically, this, Properties.Settings.Default.ConnectionRepeatDelay, System.Threading.Timeout.Infinite);
            }
        }

        private void StopTmrCheckDeviceCyclically()
        {
            WaitHandle waitHandle = null;
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclically != null)
                {
                    _TmrCheckConnectionCyclically.Dispose();
                    _TmrCheckConnectionCyclically = null;
                }

                if ((currentThread != null) && (currentThread != Thread.CurrentThread))
                {
                    // waiting until timer callback execution was completed
                    waitHandle = _TmrCheckConnectionCyclicallyFinished;
                }
            }

            if (waitHandle != null && !waitHandle.SafeWaitHandle.IsClosed)
                waitHandle.WaitOne();
        }

        private void ReStartTmrCheckDeviceCyclically()
        {
            StopTmrCheckDeviceCyclically();
            StartTmrCheckDeviceCyclically();
        }

        Thread currentThread;
        private void CheckDeviceCyclically(Object state)
        {
            try
            {
                bool bExecute = false;
                lock (lockThreadObject)
                {
                    currentThread = System.Threading.Thread.CurrentThread;
                    // destroy timet object
                    if (_TmrCheckConnectionCyclically != null)
                    {
                        bExecute = true;
                        _TmrCheckConnectionCyclically.Dispose();
                        _TmrCheckConnectionCyclically = null;
                    }

                    // timer is running
                    if (_TmrCheckConnectionCyclicallyFinished == null)
                        _TmrCheckConnectionCyclicallyFinished = new ManualResetEvent(false);
                    else
                        _TmrCheckConnectionCyclicallyFinished.Reset();
                }

                if (!bDisposed && !bSuspended && bExecute)
                {
                    try
                    {
                        SuspendJobsExecution();

                        DriverErrorCodes conn = CheckDevice(null, this);
                        switch (conn)
                        {
                            case DriverErrorCodes.ErrorDeviceOpenFailed:
                                break;
                            case (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionBroken:
                                ManageConnectionBroken();
                                break;
                            case (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionRestored:
                                ManageConnectionRestored();
                                break;
                        }
                    }
                    finally
                    {
                        RestartJobsExecution();
                    }
                }
            }
            finally
            {
                lock (lockThreadObject)
                {
                    if (_TmrCheckConnectionCyclicallyFinished != null)
                    {
                        // timer execution complete
                        _TmrCheckConnectionCyclicallyFinished.Set();
                    }
                    currentThread = null;
                }
            }
        }

        protected override void StartTimers()
        {
            base.StartTimers();
            if (bChannelStarted)
            {
                StartTmrCheckDeviceCyclically();
            }
        }

        protected override void StopTimers(bool bTerminate = true)
        {
            base.StopTimers(bTerminate);
            StopTmrCheckDeviceCyclically();
        }
        #endregion


        #region Specific Methods

        private bool IsConnectionBrokenTimeoutElapsed()
        {
            if(lastConnectionOkTime == DateTime.MinValue)
            {
                return (true);
            }

            double dtime = (DateTime.UtcNow - lastConnectionOkTime).TotalMilliseconds;

            if(dtime > ConnectionResetTime*1000)
            {
                return (true);
            }

            return (false);
        }

        private bool IsCheckCommunicationTimeoutElapsed()
        {
            if (lastCheckCommunicationTime == DateTime.MinValue)
            {
                return (true);
            }

            double dtime = (DateTime.UtcNow - lastCheckCommunicationTime).TotalMilliseconds;

            if (dtime > TIMEOUT_CHECK_CONNECTION + Timeout)
            {
                return (true);
            }

            return (false);
        }

        private void ReadAsynchronous(EIBCommJob executedJob)
        {
            SmartThreadPool.QueueWorkItem(() =>
            {
                GroupValue v = null;
                lastSentRequestTime = DateTime.UtcNow;
                executedJob.Status = EibCommJobStatus.ReadRequestPending;
                try
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("EIB DBG - Read Gr {0}",
                                                        DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.PollingGroup));
                    GroupAddress address = new GroupAddress(executedJob.PollingGroup);
                    v = _bus.ReadValue(address, Priority.Low, Timeout);
                }
                catch (Exception ex)
                {
                    try
                    {
                        LastErrorCode = (DriverErrorCodes)EIB_ERROR_CODES.DeviceFalconErrorTimeOut;
                        executedJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
                        executedJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitReadError);
                        //executedJob.LastExecutionTime = DateTime.UtcNow;
                        //if (SynchroJob != null && SynchroJob == executedJob)
                        //{
                        //    executedJob.ResetSynchro.WaitOne(Timeout);
                        //    SynchroJob = null;
                        //    executedJob.ResetSynchro.Reset();
                        //}
                        v = null;
                        //ProcessError(executedJob, LastErrorCode);
                    }
                    catch
                    {
                    }
                }
                
                try
                {
                    executedJob.LastReadRequestTimeStamp = lastSentRequestTime;
                    executedJob.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - ReadAs ret t {0} Pol Gr {1}",
                                                    DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.PollingGroup));
                    LastErrorCode = DriverErrorCodes.ErrorNoError;
                    lastSentRequestTime = DateTime.UtcNow;
                    ProcessError(executedJob, LastErrorCode, v);
                }
                catch
                {
                }
                executedJob.ErrorCode = (EIB_ERROR_CODES)LastErrorCode;                

                System.Diagnostics.Debug.WriteLine(string.Format("EIB DBG - Read Gr {0} terminated",
                                                        DateTime.Now.ToString("HH:mm:ss.fff"), v));
                SetNewDataEvent();                
            });
        }

        private void WriteAsynchronous(EIBCommJob executedJob)
        {                        
            SmartThreadPool.QueueWorkItem(() =>
            {
                executedJob.Status = EibCommJobStatus.WriteRequestPending;
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
                        lastSentRequestTime = DateTime.UtcNow;
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
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - Write ret f {0} Out Gr {1} terminated",
                                                        DateTime.Now.ToString("HH:mm:ss.fff"), executedJob.OutputGroup));
                OnJobExecuted(eJob);

                SetNewDataEvent();
            });
        }

        private void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode, GroupValue value = null)
        {
            EIBCommJob EibPendingJob = (EIBCommJob)pendingjob;

            if (EibPendingJob.RetryOutput == false)
                EibPendingJob.ClearTagListOnWriting();
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = EibPendingJob;
            eJob.Values = value;
            eJob.ErrorCode = errorCode;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - {0} ProcErr call OnJobExd for {1}",
                                           DateTime.Now.ToString("HH:mm:ss.fff"), EibPendingJob.OutputGroup));
            
#endif
            OnJobExecuted(eJob);
        }
        
        private void AnalyzeNewData(GroupAddress reciveAddress, GroupValue reciveValue)
        {            
            // Process new data            
            if (reciveValue == null)
            {
#if DEBUG                
                String DbgTxt = String.Format("EIB DBG - {0} AnNewDa con 1", DateTime.Now.ToString("HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                return;
            }
            // Get the list of jobs associated to the EIB address
            List<EIBCommJob> listJobs = GetJobListForNewData(reciveAddress);
            if (listJobs.Count == 0)
            {
#if DEBUG                
                String DbgTxt = String.Format("EIB DBG - {0} AnNewDa con 2", DateTime.Now.ToString("HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine(DbgTxt);
#endif
                return;
            }

            // Process new data
            foreach (var eibJob in listJobs)
            {
                if (eibJob != null)
                {                    
                    if (eibJob.ErrorCode == (EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError)
                        eibJob.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitNewData);

                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = eibJob;
                    eJob.ErrorCode = DriverErrorCodes.ErrorNoError; // (DriverErrorCodes)eibJob.ErrorCode;
                    eJob.Values = reciveValue;
                    eJob.Job.IsRead = true;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - {0} AnalyzeNewData call OnJobExd for {1}, Status {2}", DateTime.Now.ToString("HH:mm:ss.fff"), reciveAddress.ToString(), eibJob.Status));
#endif
                    OnJobExecuted(eJob);
                }
            }            
        }

        private void AddToMapEIBInputGroupAddJobs(UInt16 GroupAddress, EIBCommJob job)
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

        private void AddToMapEIBOutputGroupAddJobs(UInt16 GroupAddress, EIBCommJob job)
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

        private List<EIBCommJob> GetJobListForNewData(ushort groupAddress)
        {
            lock (lockAddressMaps)
            {
                if (mapEIBInputGroupAddJobs.ContainsKey(groupAddress))
                    return new List<EIBCommJob>(mapEIBInputGroupAddJobs[groupAddress]);
                else
                    return new List<EIBCommJob>();
            }            
        }

        public void ManageConnectionBroken(CommJob job = null)
        {
            if (!connectionIsBroken)
            {
                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorConnectionBroken, EventSeverity.High);
                connectionIsBroken = true;
            }

            foreach (var station in CommDriver.GetChannelStations(this))
            {
                // Set the error state of the jobs
                EIBStation eibStation = (EIBStation)station;
                eibStation.ManageConnectionBroken(job);
            }

            //added 4.1 ex
            lastConnectionOkTime = DateTime.UtcNow;
        }

        private void ManageConnectionRestored()
        {
            if (connectionIsBroken)
            {
                CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionEstablished, EventSeverity.High);
                connectionIsBroken = false;
            }

            foreach (var station in CommDriver.GetChannelStations(this))
            {
                // Set the error state of the jobs
                EIBStation eibStation = (EIBStation)station;
                eibStation.ManageConnectionRestored();
            }

            lastConnectionOkTime = DateTime.UtcNow;
        }

        //private void ManageConnectionBroken(List<CommJob> jobList)
        //{
        //    try
        //    {
        //        connectionIsBroken = true;
        //        CommDriver.OnSystemEvent(null, Properties.Resources.ErrorConnectionBroken, EventSeverity.High);

        //        Parallel.ForEach(jobList, job =>
        //        {
        //            // Deactivate the job
        //            EIBCommJob eibJob = (EIBCommJob)job;
        //            if ((eibJob.Status == EibCommJobStatus.ReadRequestPending) ||
        //               (eibJob.Status == EibCommJobStatus.WriteRequestPending))
        //            {
        //                eibJob.Status = EibCommJobStatus.Idle;
        //            }
        //        });

        //        foreach (var station in CommDriver.GetChannelStations(this))
        //        {
        //            // Set the error state of the jobs
        //            EIBStation eibStation = (EIBStation)station;
        //            eibStation.ManageConnectionBroken();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        connectionIsBroken = true;
        //    }
        //}

        private Bus CreateCommunicationBus()
        {
            Bus outBus = null;
            try
            {
                //Calls the falcon method
                ConnectorParameters connectorParameter;
                switch (_ConnectorType)
                {
                    case EIBProtocol.ConnectorTypes.KnxIpRouting:
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - CreateCommunicationBus start {0} ",
                               DateTime.Now.ToString("HH:mm:ss.fff")));
#endif
                            connectorParameter = new KnxIpRoutingConnectorParameters(_MulticastAddress, new IndividualAddress("0.0.1"));
                            (connectorParameter as KnxIpRoutingConnectorParameters).IpPort = (ushort)_EIBNetTcpPort;
                            if (!string.IsNullOrEmpty(_LocalHostName))
                                (connectorParameter as KnxIpRoutingConnectorParameters).LocalAddress = _LocalHostName;
                            break;
                        }
                    case EIBProtocol.ConnectorTypes.KnxIpTunneling:
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - CreateCommunicationBus start {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
#endif
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
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - CreateCommunicationBus OK {0} ",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
#endif
            }
            catch (Exception exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - CreateCommunicationBus err {0} exception {1} ",
                    DateTime.Now.ToString("HH:mm:ss.fff"), exception.Message));
#endif
                if (outBus != null)
                {
                    outBus.Dispose();
                    outBus = null;
                }
            }
            return outBus;
        }

        //private bool InitCheckBusCommunication()
        //{
        //    bool rt = false;
        //    Bus sut;
        //    switch (_ConnectorType)
        //    {
        //        case EIBProtocol.ConnectorTypes.KnxIpRouting:
        //            sut = CreateCommunicationBus();
        //            if (sut != null)
        //            {
        //                try
        //                {
        //                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.Connect {0} ",
        //                        DateTime.Now.ToString("HH:mm:ss.fff")));
        //                    sut.Connect();
        //                    int cnt = 5;
        //                    do
        //                    {
        //                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication start {0} ",
        //                            DateTime.Now.ToString("HH:mm:ss.fff")));
        //                        CheckCommunicationResult Result = sut.CheckCommunication();
        //                        if (Result == CheckCommunicationResult.Ok)
        //                        {
        //                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication OK {0} ",
        //                               DateTime.Now.ToString("HH:mm:ss.fff")));
        //                            rt = true;
        //                        }
        //                        else
        //                        {
        //                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting -initial bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
        //                                DateTime.Now.ToString("HH:mm:ss.fff"), Result));
        //                            cnt--;
        //                        }
        //                    } while (rt != true && cnt != 0);
        //                }
        //                catch
        //                {
        //                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.CheckCommunication exception {0} ",
        //                       DateTime.Now.ToString("HH:mm:ss.fff")));
        //                    break;
        //                }

        //                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - initial bus.Disconnect {0} ",
        //                     DateTime.Now.ToString("HH:mm:ss.fff")));
        //                sut.Disconnect();
        //                sut.Dispose();
        //                sut = null;
        //            }

        //            break;
        //        case EIBProtocol.ConnectorTypes.KnxIpTunneling:
        //            sut = CreateCommunicationBus();
        //            if (sut != null)
        //            {
        //                try
        //                {
        //                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.Connect {0} ",
        //                        DateTime.Now.ToString("HH:mm:ss.fff")));
        //                    sut.Connect();

        //                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication start {0} ",
        //                        DateTime.Now.ToString("HH:mm:ss.fff")));
        //                    CheckCommunicationResult Result = sut.CheckCommunication();
        //                    if (Result == CheckCommunicationResult.Ok)
        //                    {
        //                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication OK {0} ",
        //                           DateTime.Now.ToString("HH:mm:ss.fff")));
        //                        rt = true;
        //                    }
        //                    else
        //                    {
        //                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling -initial bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
        //                            DateTime.Now.ToString("HH:mm:ss.fff"), Result));
        //                    }
        //                }
        //                catch
        //                {
        //                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.CheckCommunication exception {0} ",
        //                       DateTime.Now.ToString("HH:mm:ss.fff")));
        //                    break;
        //                }
        //                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpTunneling - initial bus.Disconnect {0} ",
        //                     DateTime.Now.ToString("HH:mm:ss.fff")));
        //                sut.Disconnect();
        //                sut.Dispose();
        //                sut = null;
        //            }
        //            break;
        //    }

        //    return rt;
        //}

        private bool CheckBusCommunication()
        {
            bool rt = false;
            switch (_ConnectorType)
            {
                case EIBProtocol.ConnectorTypes.KnxIpRouting:
                    if (_bus != null)
                    {
                        // current connection state
                        bool connectionInError = (checkCommunicationRequest != 0 || connectionIsBroken);

                        try
                        {
                            if (checkCommunicationRequest < REPETE_CHECK_CONNECTION)
                                rt = true;
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication start {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            CheckCommunicationResult Result = _bus.CheckCommunication();

                            // if connection previous state is an error state (cable disconnection ? eth card disabled ?), reopen connection
                            if (Result == CheckCommunicationResult.Ok && connectionInError)
                            {
                                DeviceClose();
                                Result = (DeviceOpen() ? CheckCommunicationResult.Ok : CheckCommunicationResult.MulticastNotUsed);
                            }

                            if (Result != CheckCommunicationResult.Ok)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication err {0} CheckCommunicationResult = {1}",
                                    DateTime.Now.ToString("HH:mm:ss.fff"), Result));

                                checkCommunicationRequest++;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication OK {0} ",
                                   DateTime.Now.ToString("HH:mm:ss.fff")));
                                checkCommunicationRequest = 0;
                                rt = true;
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG KnxIpRouting - bus.CheckCommunication exception {0} ",
                                DateTime.Now.ToString("HH:mm:ss.fff")));
                            checkCommunicationRequest++;
                        }
                        if (!rt)
                            checkCommunicationRequest = REPETE_CHECK_CONNECTION;
                    }

                    break;
                default:
                    rt = true;
                    break;

            }

            return rt;
        }

        //private bool InErrorState()
        //{
        //    bool InErrorState = false;
        //    foreach (var station in CommDriver.GetChannelStations(this))
        //    {
        //        if (station.InErrorState && station.GetChannel() == this)
        //        {
        //            InErrorState = true;
        //            break;
        //        }
        //    }
        //    return InErrorState;
        //}

        /// <summary>
        /// Call back device data changed
        /// </summary>
        /// <param name="e"></param>
        private void BusOnGroupValueReceived(GroupValueEventArgs e)
        {
            if (e == null)
                return;

            lastCheckCommunicationTime = DateTime.UtcNow;
            checkCommunicationRequest = 0;
            connectionRestored.Set();
            
            AnalyzeNewData(e.Address, e.Value);

            // restart tmr using to check connetion
            ReStartTmrCheckDeviceCyclically();
        }

        /// <summary>
        /// Call back device status changed
        /// </summary>
        /// <param name="busConnectionStatus"></param>
        private void BusOnStateChanged(BusConnectionStatus busConnectionStatus)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("Connection state: {0}", busConnectionStatus));
            if (busConnectionStatus == BusConnectionStatus.Broken || busConnectionStatus == BusConnectionStatus.Closed)
            {
                connectionBroken.Set();
                connectionClose.Set();                
            }
            else if (busConnectionStatus == BusConnectionStatus.Connected)
            {
                connectionRestored.Set();
                lastCheckCommunicationTime = DateTime.UtcNow;
                checkCommunicationRequest = 0;
            }
        }

        public void AddJobToInternalMap(CommJob job)
        {
            EIBCommJob EibJob = (EIBCommJob)job;

            // reset polling state when driver "restart"
            EibJob.ResetInternalState();
            if (!EibJob.AddedToMap)
            {
                EibJob.AddedToMap = true;
                if ((EibJob.Type == LinkType.Input) || (EibJob.Type == LinkType.InputOutput))
                {
                    if (!String.IsNullOrWhiteSpace(EibJob.InputGroups))
                    {
                        string[] ListAddressSplit = EibJob.InputGroups.Split(new Char[] { ';' });
                        foreach (var InAdd in ListAddressSplit)
                        {
                            AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(InAdd), EibJob);
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(EibJob.PollingGroup))
                    {
                        AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(EibJob.PollingGroup), EibJob);
                    }
                    if (EibJob.Type != LinkType.Input)
                    {
                        if (!String.IsNullOrWhiteSpace(EibJob.OutputGroup))
                        {
                            UInt16 ConvertedAddress = EIBCommJob.EibGroupAddressToUInt16(EibJob.OutputGroup);
                            if (ConvertedAddress > 0)
                            {
                                AddToMapEIBOutputGroupAddJobs(ConvertedAddress, EibJob);
                            }
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(EibJob.OutputGroup))
                    {
                        UInt16 ConvertedAddress = EIBCommJob.EibGroupAddressToUInt16(EibJob.OutputGroup);
                        if (ConvertedAddress > 0)
                        {
                            AddToMapEIBOutputGroupAddJobs(ConvertedAddress, EibJob);
                        }
                    }
                }
            }
        }

        public bool IsCommunicationInstable()
        {
            return (!(checkCommunicationRequest == 0 && !connectionIsBroken));
        }

        //public bool IsMinBusInactivityTime(out uint delay, uint _Polling)
        //{            
        //    if (lastSentRequestTime != DateTime.MinValue && (DateTime.UtcNow - lastSentRequestTime).TotalMilliseconds < (_MinBusInactivityTime + _Polling))
        //    {
        //        delay = (_MinBusInactivityTime + _Polling) - (uint)(DateTime.UtcNow - lastSentRequestTime).TotalMilliseconds;
        //        return true;                
        //    }
        //    else
        //    {
        //        delay = 0;
        //        return false;
        //    }
        //}

        #endregion

        #region Abstracts Methods

        public override bool IsDeviceOpen()
        {
            bool returnValue; 
            
            if (_bus == null)
            {
                returnValue = false;
            }
            else if (connectionIsBroken)
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
                lastConnectionOkTime = DateTime.UtcNow;
            }

            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        private DateTime lastDeviceOpenAttempt = DateTime.MinValue;
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
                if (notRepeteConnectionKnxIpRouting)
                {
                    returnValue = true;
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                        //_bus.GroupValueReceived -= BusOnGroupValueReceived;
                        //_bus.StateChanged -= BusOnStateChanged;
                        _bus.GroupValueReceived += BusOnGroupValueReceived;
                        _bus.StateChanged += BusOnStateChanged;
                        _bus.Connect();
                    }
                    catch (Exception exception)
                    {
                        _bus.GroupValueReceived -= BusOnGroupValueReceived;
                        _bus.StateChanged -= BusOnStateChanged;
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect Exception in DeviceOpen at {0}, Message: {1} ",
                            DateTime.Now.ToString("HH:mm:ss.fff"), exception.Message));
                        if (_bus != null)
                            _bus.Disconnect();
                    }
                    if (_bus != null && _bus.IsConnected)
                    {                        
                        lastConnectionOkTime = DateTime.UtcNow;
                        lastCheckCommunicationTime = DateTime.UtcNow;
                        checkCommunicationRequest = 0;

                        CommDriver.OnSystemEvent(null, Properties.Resources.CreateCommunicationBus, EventSeverity.High);
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect OK {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                        connectionBroken.Reset();
                        connectionRestored.Reset();
                        returnValue = true;
                        if (ConnectorType == EIBProtocol.ConnectorTypes.KnxIpRouting)
                        {
                            notRepeteConnectionKnxIpRouting = true;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Connect failed {0} ",
                            DateTime.Now.ToString("HH:mm:ss.fff")));
                    }
                }
            }
            lastDeviceOpenAttempt = DateTime.UtcNow;
            return (returnValue);
        }
        

        public override bool DeviceClose()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - DeviceClose start {0} ",
                DateTime.Now.ToString("HH:mm:ss.fff")));
            if (_bus != null)
            {
                //if (connectionIsBroken == false)
                //{
                //    //ManageConnectionBroken(ListJobPending);
                //    lastConnectionOkTime = DateTime.UtcNow;
                //}

                connectionClose.Reset();
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Disconnect() {0} ",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
                _bus.GroupValueReceived -= BusOnGroupValueReceived;
                _bus.StateChanged -= BusOnStateChanged;
                _bus.Disconnect();
                if (connectionClose.WaitOne(500))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG -  ConnectionClose.Reset() {0} ",
                        DateTime.Now.ToString("HH:mm:ss.fff")));
                    connectionClose.Reset();
                }
                //_bus.GroupValueReceived -= BusOnGroupValueReceived;
                //_bus.StateChanged -= BusOnStateChanged;
                System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - bus.Dispose() {0}",
                    DateTime.Now.ToString("HH:mm:ss.fff")));
                _bus.Dispose();
                _bus = null;
                CommDriver.OnSystemEvent(null, Properties.Resources.CloseCommunicationBus, EventSeverity.High);
                connectionBroken.Reset();
                connectionRestored.Reset();                
            }
            System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - DeviceClose end {0} ",
                DateTime.Now.ToString("HH:mm:ss.fff")));
            //ListJobPending.Clear();

            lastSentRequestTime = new DateTime();
            lastConnectionOkTime = new DateTime();
            lastCheckCommunicationTime = new DateTime();

            checkCommunicationRequest = 0;
            connectionIsBroken = false;
            notRepeteConnectionKnxIpRouting = false;
            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return true;
        }

        public override DriverErrorCodes CheckDevice(CommJob exjob, object thischannel)
        {
            bool checkCommunication = false;
            DriverErrorCodes conn = (IsDeviceOpen() ? DriverErrorCodes.ErrorNoError : DriverErrorCodes.ErrorDeviceOpenFailed);
            if (conn != DriverErrorCodes.ErrorNoError && ((DateTime.UtcNow - lastDeviceOpenAttempt).TotalMilliseconds > Properties.Settings.Default.ConnectionRepeatDelay))
            {
                conn = (DeviceOpen() ? DriverErrorCodes.ErrorNoError : DriverErrorCodes.ErrorDeviceOpenFailed);
            }
            if (conn == DriverErrorCodes.ErrorNoError)
            {
                checkCommunication = (_ConnectorType == EIBProtocol.ConnectorTypes.KnxIpRouting && (checkCommunicationRequest != 0 || IsCheckCommunicationTimeoutElapsed()));
            }
            else
            {
                //List<Station> stationList = CommDriver.GetChannelStations(this);
                //foreach (Station station in stationList)
                //{
                //    // Set the error state of the station and of its jobs
                //    EIBStation eibStation = (EIBStation)station;
                //    eibStation.ManageConnectionBroken();
                //}
                conn = (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionBroken;
            }        

            if (checkCommunication)
            {
                lastCheckCommunicationTime = DateTime.UtcNow;
                if (_bus == null || !CheckBusCommunication())
                {
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                    connectionBroken.Set();
                }
                else
                    connectionRestored.Set();
            }

            // Connection broken?
            if (connectionBroken.WaitOne(0))
            {
                connectionBroken.Reset();
                if (!connectionIsBroken)
                {
                    //ManageConnectionBroken();
                    conn = (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionBroken;
                    //LastConnectionOkTime = DateTime.UtcNow;
                }        
            }

            // Connection restored?
            if (connectionRestored.WaitOne(0))
            {
                connectionRestored.Reset();
                if (connectionIsBroken)
                    //ManageConnectionRestored();
                    conn = (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionRestored;
            }

            return conn;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Override Methods      


        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            //// job used to manage checkdevice connection
            //if (IsErrorJob(job))
            //{                
            //    ResetErrorJob();

            //    // if now connection is ok, exit
            //    if (conn == DriverErrorCodes.ErrorNoError)
            //    {
            //        job.IsPending = false;
            //        RemovePendingJob(job);
            //    }
            //}

            switch (conn)
            {
                case (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionRestored:            
                    ManageConnectionRestored();
                    // continue the job normal execution
                    conn = DriverErrorCodes.ErrorNoError;
                    break;

                case DriverErrorCodes.ErrorDeviceOpenFailed:
                case (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionBroken:
                    // put in error all jobs and then exit
                    ManageConnectionBroken(job);
                    return false;
            }

            EIBCommJob ej = job as EIBCommJob;

            //// Check if it is elapsed enough time from the last sent request
            //if ((DateTime.UtcNow - LastSentRequestTime).TotalMilliseconds > MinBusInactivityTime)
            //{
            // Input request
            if ((ej.Type == DriverCodeBaseEx.Enumerators.LinkType.Input || ej.Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput) && ej.PollingCanBePerformed())
            {
                if (ej.EnableOnlyInitialPolling && !ej.RetryInitialPolling)
                    ej.PollingEnabled = false;

                // Check if the polling address is valid
                if (EIBCommJob.IsValidAddress(ej.PollingGroup))
                {
                    base.ExecuteJob(job);

                    // Be sure the that the job has been inserted in the dictionary of the input groups
                    AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(ej.PollingGroup), ej);

                    ReadAsynchronous(ej);
                }
            }
            // Output request
            else if ((ej.Type != DriverCodeBaseEx.Enumerators.LinkType.Input) && ej.WriteCanBePerformed())
            {
                // Check if the output address is valid
                if (EIBCommJob.IsValidAddress(ej.OutputGroup))
                {
                    base.ExecuteJob(job);

                    // Be sure the that the job has been inserted in the dictionary of the output groups
                    AddToMapEIBOutputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(ej.OutputGroup), ej);

                    WriteAsynchronous(ej);
                }
            }            
            else
            {
                // unmanaged situation 
                RemovePendingJob(ej);
                ej.LastExecutionTime = DateTime.UtcNow;
                
                return false;
            }

            return true;
        }

        //public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        //{
        //    System.Diagnostics.Debug.WriteLine(String.Format("EIB DBG - ProcessNewData f {0} conn {1}", DateTime.Now.ToString("HH:mm:ss.fff"), conn));

        //    return true;
        //}
        //public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        //{
        //    //EIBCommJob job = (EIBCommJob)pendingjob;

        //    //switch (conn){
        //    //    case (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_ManageConnectionBroken:
        //    //        ManageConnectionBroken();
        //    //        return false;
        //    //        break;

        //    //    case DriverErrorCodes.ErrorNoError:            
        //    //        if (receiveItem == null)
        //    //            conn = DriverErrorCodes.ErrorTimeOut;
        //    //        else if (receiveItem.ErrorCode != DriverErrorCodes.ErrorNoError)
        //    //            conn = receiveItem.ErrorCode;
        //    //        break;
        //    //}

        //    //ExecutedJobArgs eJob = new ExecutedJobArgs();
        //    //eJob.Job = job;
        //    //eJob.ErrorCode = conn;
        //    //eJob.Job.IsRead = (job.Status == EibCommJobStatus.ReadRequestPending ? true : false);

        //    //switch (job.Status)
        //    //{
        //    //    case EibCommJobStatus.ReadRequestPending:
        //    //        if (conn == DriverErrorCodes.ErrorNoError)
        //    //        {
        //    //            eJob.Values = receiveItem.Value;                        
        //    //            job.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
        //    //        }
        //    //        else
        //    //        {
        //    //            LastErrorCode = (DriverErrorCodes)EIB_ERROR_CODES.DeviceFalconErrorTimeOut;
        //    //            job.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceRead);
        //    //            job.SetConditionalVariableBit(true, (ushort)EIBConditionalVariableBits.BitReadError);
        //    //        }

        //    //        job.LastReadRequestTimeStamp = lastSentRequestTime;
        //    //        break;

        //    //    case EibCommJobStatus.WriteRequestPending:
        //    //        if (conn == DriverErrorCodes.ErrorNoError)
        //    //        {
        //    //            job.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceWrite);
        //    //            job.SetConditionalVariableBit(LastErrorCode != DriverErrorCodes.ErrorNoError, (ushort)EIBConditionalVariableBits.BitWriteError);
        //    //        } else
        //    //        {
        //    //            job.SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitForceWrite);
        //    //            job.SetConditionalVariableBit(LastErrorCode != DriverErrorCodes.ErrorNoError, (ushort)EIBConditionalVariableBits.BitWriteError);
        //    //        }
        //    //        break;
        //    //    default:
        //    //        int i = 0;
        //    //        break;
        //    //}

        //    ////ProcessError
        //    //if (conn != DriverErrorCodes.ErrorNoError)
        //    //{
        //    //    if (job.RetryOutput == false)
        //    //        job.ClearTagListOnWriting();
        //    //}
        //    ////else 
        //    ////{
        //    ////    if (job.PollingTime > 0)
        //    ////        eJob.LastExecutionTime = DateTime.UtcNow.AddMilliseconds(job.PollingTime);                
        //    ////}
        //    //OnJobExecuted(eJob);

        //    return true;
        //}

        public void ChannelOnJobExecuted(ExecutedJobArgs e)
        {
            OnJobExecuted(e);
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                InitKNX();

                // start timer use to check communication periodically
                StartTmrCheckDeviceCyclically();
            }

            return base.Startup();
        }

        public void InitKNX()
        {
            if (connectionBroken == null)
            {
                connectionBroken = new ManualResetEvent(false);
            }
            else
            {
                connectionBroken.Reset();
            }

            if (connectionRestored == null)
            {
                connectionRestored = new ManualResetEvent(false);
            }
            else
            {
                connectionRestored.Reset();
            }
            if (connectionClose == null)
            {
                connectionClose = new ManualResetEvent(false);
            }
            else
            {
                connectionClose.Reset();
            }

            // use NewDataToAnlyze object to syncronize KNK library Read/Write Method (execute into ThreadPool --> async) with KNK call back events
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze == null)
                    NewDataToAnlyze = new ManualResetEvent(false);
                else
                    NewDataToAnlyze.Reset();
            }

            bool isDeviceOpen = DeviceOpen();

            if (_ConnectorType == EIBProtocol.ConnectorTypes.KnxIpRouting)
            {
                lastCheckCommunicationTime = DateTime.UtcNow;
                if (_bus == null || !CheckBusCommunication())
                {
                    isDeviceOpen = false;
                }
            }
            connectionIsBroken = !isDeviceOpen;
        }
        //private DriverErrorCodes CheckExecutionJob(CommJob exjob, out DateTime nextExecutionTime)
        //{
        //    nextExecutionTime = DateTime.MinValue;
        //    EIBCommJob job = (EIBCommJob)exjob;
        //    DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;


        //    bool execute = job.MustSendARequest(out nextExecutionTime);
        //    if (execute) 
        //    {
        //        //if (checkCommunicationRequest == 0 && !connectionIsBroken)
        //        //{
        //            //// Check if it is elapsed enough time from the last sent request
        //            //if ((DateTime.UtcNow - lastSentRequestTime).TotalMilliseconds > MinBusInactivityTime)
        //            //{
        //                // Input request
        //                if (((job.Type == DriverCodeBaseEx.Enumerators.LinkType.Input) ||
        //                     (job.Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput)) &&
        //                     job.PollingCanBePerformed())
        //                {
        //                    execute = true;
        //                }

        //                // Output repquest
        //                else if ((job.Type != DriverCodeBaseEx.Enumerators.LinkType.Input) &&
        //                         job.WriteCanBePerformed())
        //                {
        //                    execute = true;
        //                }
        //            //} 
        //            //else
        //            //{                        
        //            //    nextExecutionTime = DateTime.UtcNow.AddMilliseconds((DateTime.UtcNow - lastSentRequestTime).TotalMilliseconds);
        //            //    conn = (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_RescheduleJob;
        //            //}
        //        //}
        //        //else
        //        //{
        //        //    // Still waiting for sending the request 
        //        //    job.Status = EibCommJobStatus.WaitingToSendRequest;
        //        //    //job.StartExecutionTime = DateTime.UtcNow;
        //        //    nextExecutionTime = DateTime.UtcNow.AddMilliseconds(Properties.Settings.Default.ConnectionRepeatDelay);
        //        //    conn = (DriverErrorCodes)EIBProtocol.EIB_ERROR_CODES.DeviceFalconError_RescheduleJob;
        //        //}
        //    }
        //    else
        //    {
        //        int i= 1;
        //    }

        //    return conn;
        //}
        #endregion

        #region Data Members
        private Bus _bus = null;

        protected object lockAddressMaps = new object();
        
        /// <summary> Event connection broken. </summary>
        protected ManualResetEvent connectionBroken;
        /// <summary> Event connection restored. </summary>
        protected ManualResetEvent connectionRestored;
        protected ManualResetEvent connectionClose;

        // Group Address Dictionaries
        public Dictionary<UInt16, List<EIBCommJob>> mapEIBInputGroupAddJobs = new Dictionary<UInt16, List<EIBCommJob>>();
        public Dictionary<UInt16, List<EIBCommJob>> mapEIBOutputGroupAddJobs = new Dictionary<UInt16, List<EIBCommJob>>();

        public DateTime lastSentRequestTime = new DateTime();
        DateTime lastConnectionOkTime = new DateTime();
        DateTime lastCheckCommunicationTime = new DateTime();
        
        public int checkCommunicationRequest;
        public bool connectionIsBroken;
        bool notRepeteConnectionKnxIpRouting;

        private object checkConnectionCyclicallyLock = new object();
        private Timer tmrCheckConnectionCyclically = null;

        private Timer _TmrCheckConnectionCyclically = null;
        private ManualResetEvent _TmrCheckConnectionCyclicallyFinished = null;

        #endregion


        //private CommJob errorJob = null;
        //private object errorJobLock = new object();


        //private bool IsErrorJob(CommJob job)
        //{
        //    lock (errorJobLock)
        //    {
        //        return (errorJob != null && errorJob == job);
        //    }
        //}

        //private void ResetErrorJob()
        //{
        //    lock (errorJobLock)
        //    {
        //        errorJob = null;
        //    }
        //}

        //public bool SetErrorJob(CommJob job)
        //{
        //    if ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && !job.conditionalVariableHasBeenSet)
        //    {
        //        lock (errorJobLock)
        //        {
        //            if (errorJob == null)
        //            {
        //                errorJob = job;
        //                return true;
        //            } 
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

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

        private EIBProtocol.ConnectorTypes _ConnectorType;
        public EIBProtocol.ConnectorTypes ConnectorType
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
            base.Dispose();
            if (connectionBroken != null)
            {
                connectionBroken.Dispose();
                connectionBroken = null;
            }

            if (connectionRestored != null)
            {
                connectionRestored.Dispose();
                connectionRestored = null;
            }

            if (connectionClose != null)
            {
                connectionClose.Dispose();
                connectionClose = null;
            }
            #region Connection's check timer management
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclicallyFinished != null)
                    _TmrCheckConnectionCyclicallyFinished.Dispose();
            }
            #endregion
        }
    }
}
