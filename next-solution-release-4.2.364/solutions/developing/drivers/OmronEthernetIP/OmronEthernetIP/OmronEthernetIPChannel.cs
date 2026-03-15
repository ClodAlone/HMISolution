using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.Threading.Tasks;

namespace OmronEthernetIP
{
    public class OmronEthernetIPChannel : TcpChannel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the OmronEthernetIPChannel object.
        /// </summary>
        public OmronEthernetIPChannel(CommunicationDriver commdriver, OmronEthernetIPChannelSettings settings)
            : base(commdriver, settings, false)
        {
            SessionHandle = new uintUnion(0);
        }

        #endregion

        #region Members

        byte[] requestBuffer = new byte[OmronEthernetIPProtocol.TCP_MAX_SEGMENT_SIZE];
        ushortUnion requestBufferPointer = new ushortUnion(0);
        List<OmronEthernetIPCommJob> dequeueListJob = new List<OmronEthernetIPCommJob>();
        private bool _DeviceOpenLogged = false;
        public uintUnion SessionHandle;

        #endregion

        #region Override Methods

        public override bool TestChannelComm()
        {
            bool retValue = DeviceOpen();
            if (retValue)
            {
                DeviceClose();
            }
            return retValue;
        }

        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            OmronEthernetIPCommJob j = job as OmronEthernetIPCommJob;
            j.ReadTagStart = 0;
            j.ReadTagEnd = 0;
            j.PartialArrayStart = 0;
            j.PartialArrayEnd = 0;
            j.ExecuteTask = false;
            j.TerminateTask = false;

            base.SubscribeJob(job, state);
        }

        List<OmronEthernetIPCommJob> nextlist = new List<OmronEthernetIPCommJob>();
        List<OmronEthernetIPCommJob> checkDataFormatJobList = new List<OmronEthernetIPCommJob>();
        List<OmronEthernetIPCommJob> repeatOutputJobList = new List<OmronEthernetIPCommJob>();

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;

            clearChannel();
            
            int loop = 0;
            //int dbgCount = 0;

            while (true)
            {
                // Some Output jobs have failed with error code 0x20 and Additional status 0x8022 (wrong data type)
                // so we must read the data in order to get the correct data type
                if(checkDataFormatJobList.Count > 0)
                {
                    // Force the command type of the jobs
                    foreach(OmronEthernetIPCommJob omronJob in checkDataFormatJobList)
                    {
                        omronJob.CommandType = CommandTypes.ReadDataFormatCmd;
                    }

                    if (ListJobPending.Count == 0)
                    {
                        nextlist.Clear();
                        nextlist.AddRange(checkDataFormatJobList);
                        bool isOpen = OmronEthernetIPIsDeviceOpen();
                        if (!isOpen)
                        {
                            isOpen = OmronEthernetIPDeviceOpen();
                            if (isOpen)
                                isOpen = OmronEthernetIPForwardOpen(nextlist);
                        }

                        if (isOpen)
                        {
                            lock (lockThreadObject)//201011
                            {
                                ListJobPending.AddRange(nextlist);
                                ExecuteJobList(ref nextlist);
                            }                            
                        }
                        else
                        {
                            ListJobPending.AddRange(nextlist);
                            clearChannel();
                        }
                    }

                    lock (lockThreadObject)
                    {
                        bool bNew = NewDataToAnlyze.WaitOne(0, false);
                        if (bNew || ReceiveBuffer.Count > 0)
                        {
                            NewDataToAnlyze.Reset();
                            if (ListJobPending.Count > 0)
                            {
                                if (ProcessNewDataList(ListJobPending))
                                {
                                    ReceiveBuffer.Clear();
                                    foreach (OmronEthernetIPCommJob job in ListJobPending)
                                    {
                                        job.CommandType = CommandTypes.WriteCmd;
                                        repeatOutputJobList.Add(job);
                                    }
                                }
                                foreach (OmronEthernetIPCommJob job in repeatOutputJobList)
                                {
                                    job.LastExecutionTime = DateTime.UtcNow;
                                    ListJobPending.Remove(job);
                                    checkDataFormatJobList.Remove(job);
                                }
                            }
                        }

                        if (ListJobPending.Count > 0)
                        {
                            if (!MultiPointProtocol)
                            {
                                bool TimeOutFlag = false;
                                foreach (var job in ListJobPending)
                                {
                                    if ((DateTime.UtcNow - job.StartExecutionTime).TotalMilliseconds > Timeout)
                                    {
                                        TimeOutFlag = true;
                                        break;
                                    }
                                }
                                if (TimeOutFlag == true)
                                {
                                    //error
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                    ListJobPending.AddRange(dequeueListJob);
                                    ProcessNewDataList(ListJobPending);
                                    ListJobPending.Clear();
                                    checkDataFormatJobList.Clear();
                                    dequeueListJob.Clear();
                                    ReceiveBuffer.Clear();
                                    System.Threading.Thread.Sleep(100);
                                }
                            }
                        }
                    }

                    if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                        DeviceClose();
                }

                // Some Output jobs have failed with error code 0x20 and Additional status 0x8022 (wrong data type)
                // and now they are ready to retry the output request for the correct data type
                else if (repeatOutputJobList.Count > 0)
                {
                    // Force the command type of the jobs
                    foreach (OmronEthernetIPCommJob omronJob in repeatOutputJobList)
                    {
                        omronJob.CommandType = CommandTypes.WriteCmd;
                        // Be sure we are trying again to write the tags
                        if(omronJob.TagsListToWrite.Count == 0)
                        {
                            omronJob.TagsListToWrite.AddRange(omronJob.TagsList);
                        }
                    }

                    if (ListJobPending.Count == 0)
                    {
                        nextlist.Clear();
                        nextlist.AddRange(repeatOutputJobList);
                        bool isOpen = OmronEthernetIPIsDeviceOpen();
                        if (!isOpen)
                        {
                            isOpen = OmronEthernetIPDeviceOpen();
                            if (isOpen)
                                isOpen = OmronEthernetIPForwardOpen(nextlist);
                        }

                        if (isOpen)
                        {
                           
                            lock (lockThreadObject)//201011
                            {
                                    
                                ListJobPending.AddRange(nextlist);
                                ExecuteJobList(ref nextlist);
                            }                            
                        }
                        else
                        {
                            ListJobPending.AddRange(nextlist);
                            clearChannel();
                        }
                    }

                    lock (lockThreadObject)
                    {
                        bool bNew = NewDataToAnlyze.WaitOne(0, false);
                        if (bNew || ReceiveBuffer.Count > 0)
                        {
                            NewDataToAnlyze.Reset();
                            if (ListJobPending.Count > 0)
                            {
                                if (ProcessNewDataList(ListJobPending))
                                {
                                    ReceiveBuffer.Clear();
                                    foreach (OmronEthernetIPCommJob job in ListJobPending)
                                    {
                                        ListJobExecuted.Add(job);
                                    }
                                }
                                foreach (OmronEthernetIPCommJob job in ListJobExecuted)
                                {
                                    job.LastExecutionTime = DateTime.UtcNow;
                                    if (SynchroJob != null && SynchroJob == job)
                                    {
                                        job.ResetSynchro.WaitOne(Timeout);
                                        SynchroJob = null;
                                        job.ResetSynchro.Reset();
                                    }
                                    ListJobPending.Remove(job);
                                    repeatOutputJobList.Remove(job);
                                    job.ClearWriteTagLists();
                                }
                                ListJobExecuted.Clear();
                            }
                        }

                        if (ListJobPending.Count > 0)
                        {
                            if (!MultiPointProtocol)
                            {
                                bool TimeOutFlag = false;
                                foreach (var job in ListJobPending)
                                {
                                    if ((DateTime.UtcNow - job.StartExecutionTime).TotalMilliseconds > Timeout)
                                    {
                                        TimeOutFlag = true;
                                        break;
                                    }
                                }

                                if (TimeOutFlag == true)
                                {
                                    //error
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                    ListJobPending.AddRange(dequeueListJob);
                                    ProcessNewDataList(ListJobPending);
                                    ListJobPending.Clear();
                                    repeatOutputJobList.Clear();
                                    dequeueListJob.Clear();
                                    ReceiveBuffer.Clear();
                                    System.Threading.Thread.Sleep(100);

                                }
                            }
                        }
                    }

                    if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                        DeviceClose();
                }

                // Normal state
                else
                {
                    if (ListJobPending.Count == 0)
                    {
                        nextlist.Clear();
                        ScheduleListJob();

                        lock (lockThreadObject)//201011
                        {
                            if (SynchroJob != null)
                            {
                                GetNextSynchroJob(ref nextlist);                                
                            }
                            else
                            {
                                GetNextPendingList(ref nextlist);
                            }
                        }
                        if (nextlist.Count > 0)
                        {
                            bool isOpen = OmronEthernetIPIsDeviceOpen();
                            if (!isOpen)
                            {
                                isOpen = OmronEthernetIPDeviceOpen();
                                if (isOpen)
                                    isOpen = OmronEthernetIPForwardOpen(nextlist);
                            }

                            if (isOpen)
                            {
                                lock (lockThreadObject)//201011
                                {
                                    ListJobPending.AddRange(nextlist);
                                    ExecuteJobList(ref nextlist);
                                }
                            }
                            else
                            {
                                ListJobPending.AddRange(nextlist);
                                clearChannel();
                            }
                        }
                    }

                    lock (lockThreadObject)
                    {
                        bool bNew = NewDataToAnlyze.WaitOne(0, false);
                        if (bNew || ReceiveBuffer.Count > 0)
                        {
                            NewDataToAnlyze.Reset();
                            if (ListJobPending.Count > 0)
                            {
                                if (ProcessNewDataList(ListJobPending))
                                {
                                    ReceiveBuffer.Clear();
                                    foreach (OmronEthernetIPCommJob job in ListJobPending)
                                    {
                                        ListJobExecuted.Add(job);
                                    }
                                }
                                foreach (OmronEthernetIPCommJob job in ListJobExecuted)
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
                                bool TimeOutFlag = false;
                                foreach (var job in ListJobPending)
                                {
                                    if ((DateTime.UtcNow - job.StartExecutionTime).TotalMilliseconds > Timeout)
                                    {
                                        TimeOutFlag = true;
                                        break;
                                    }
                                }

                                if (TimeOutFlag == true)
                                {
                                    //error
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                    ListJobPending.AddRange(dequeueListJob);
                                    ProcessNewDataList(ListJobPending);
                                    ListJobPending.Clear();
                                    dequeueListJob.Clear();
                                    ReceiveBuffer.Clear();
                                    System.Threading.Thread.Sleep(100);

                                }
                            }
                        }
                    }

                    if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                        DeviceClose();
                }

                if (StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
            }
        }
        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            OmronEthernetIPCommJob j = e.Job as OmronEthernetIPCommJob;
            j.ExecuteTask = false;
            j.ReadTagStart = 0;
            j.ReadTagEnd = 0;
            j.PartialArrayStart = 0;
            j.PartialArrayEnd = 0;
            j.TerminateTask = true;
            base.OnJobExecuted(e);
        }

        public bool SessionRegisterOk()
        {
            return SessionHandle.UINT != 0;
        }

        public void ClearSession()
        {
            SessionHandle.UINT = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OmronEthernetIPIsDeviceOpen()
        {
            if (!IsDeviceOpen())
                return false;
                        
            if (!SessionRegisterOk())
                return false;
            
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OmronEthernetIPDeviceOpen()
        {
            if (!IsDeviceOpen())
                if (!DeviceOpen())
                    return false;
            
            if (!OmronEthernetIPProtocol.RegisterSession(ref requestBuffer, ref requestBufferPointer, this))
            {
                //Error connection
                CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                return false;
            }
                        
            return true;
        }

        public bool OmronEthernetIPForwardOpen(List<OmronEthernetIPCommJob> list)
        {
            OmronEthernetIPStation s = list[0].Station as OmronEthernetIPStation;
            if (s == null)
                return false;

            if ((s.PlcType == PlcTypes.NJ) || (s.PlcType == PlcTypes.NX))
            {
                bool bRetryTheConnection = false;
                if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                {
                    if (bRetryTheConnection)
                    {
                        if (s.PlcType == PlcTypes.NX)
                        {
                            s.PlcType = PlcTypes.NJ;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //1994;
                            if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                            {
                                //Check invalid connection size (0x0109)
                                if (bRetryTheConnection)
                                {
                                    s.PlcType = PlcTypes.Other_PLC;
                                    s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType);// 502;
                                    if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                                    {
                                        //Error connection
                                        CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //Error connection
                                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            s.PlcType = PlcTypes.Other_PLC;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //502;
                            if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                            {
                                //Error connection
                                CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Error connection
                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                        return false;
                    }
                }

                // only on first connection good 
                if (!_DeviceOpenLogged)
                {
                    _DeviceOpenLogged = true;
                    // trace on db parameters used by driver to open connection with PLC
                    CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(OmronEthernetIP.Properties.Resources.DeviceOpenUsedParameters, (s.MaxPduSize == OmronEthernetIPProtocol.GetMaxPduSize(PlcTypes.Other_PLC) ? "Forwad Open" : "Large Forward Open"), s.MaxPduSize), Opc.Ua.EventSeverity.Min);
                }
            }
            else
            {
                if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                {
                    //Error connection
                    CommDriver.OnSystemEvent(ObjectIds.Server,
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                            Opc.Ua.EventSeverity.High);
                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region methods

        protected void GetNextPendingList(ref List<OmronEthernetIPCommJob> inList)
        {
            bool write = false;
            string station = string.Empty;
            bool first = true;

            inList.Clear();
            List<OmronEthernetIPCommJob> renoveListJob = new List<OmronEthernetIPCommJob>();
            foreach (OmronEthernetIPCommJob j in dequeueListJob)
            {
                if (j.ReadTagEnd == 0 && j.PartialArrayEnd == 0)
                    renoveListJob.Add(j);
            }
            foreach (OmronEthernetIPCommJob j in renoveListJob)
            {
                dequeueListJob.Remove(j);
            }

            if (dequeueListJob.Count() == 0)
            {
                lock (lockScheduleFlag)
                {
                    var queue = GetNextPendingQueue();

                    if (queue == null)
                        return;
                    int cnt = queue.Count;
                    while (cnt-- > 0)
                    {
                        CommJob dequeuedJob = null;
                        OmronEthernetIPCommJob j = null;
                        if (queue.TryDequeue(out dequeuedJob) == true)
                        {
                            j = (OmronEthernetIPCommJob)dequeuedJob;
                        }

                        if (j != null)
                        {
                            if (j.IsPending)
                            {
                                queue.Enqueue(j);
                                continue;
                            }
                            if (first)
                            {
                                write = ((j.TagsListToWrite.Count > 0 &&
                                          ((j.Type == LinkType.ExceptionOutput) || (j.Type == LinkType.InputOutput))) ||
                                           (j.Type == LinkType.UnconditionalOutput));
                                station = j.Station.Name;
                                first = false;
                            }

                            bool jwrite = ((j.TagsListToWrite.Count > 0 &&
                                           ((j.Type == LinkType.ExceptionOutput) || (j.Type == LinkType.InputOutput))) ||
                                            (j.Type == LinkType.UnconditionalOutput));

                            if (write)
                            {
                                if (j.Station != null && j.Station.Name == station && jwrite)
                                {
                                    dequeueListJob.Add(j);
                                }
                            }
                            else
                            {
                                if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                                {
                                    dequeueListJob.Add(j);
                                }
                            }
                        }
                    }

                    if (dequeueListJob.Count() == 0)
                        return;

                    inList = processDequeueListJob(inList, write, queue);
                }
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("{0} - GetNextPendingList - Warning: {1} jobs in dequeueListJob", DateTime.UtcNow.ToString(), dequeueListJob.Count.ToString("00000")));
#endif
                lock (lockScheduleFlag)
                {
                    var queue = new ConcurrentQueue<CommJob>();
                    if (dequeueListJob[0].CommandType == CommandTypes.WriteCmd)
                    {
                        write = true;
                    }
                    inList = processDequeueListJob(inList, write, queue);
                }
            }
        }

        private void GetNextSynchroJob(ref List<OmronEthernetIPCommJob> inList)
        {
            OmronEthernetIPCommJob job = SynchroJob as OmronEthernetIPCommJob;

            if ((job.TagsListToWrite.Count > 0 &&
                      ((job.Type == LinkType.ExceptionOutput) || (job.Type == LinkType.InputOutput))) ||
                       (job.Type == LinkType.UnconditionalOutput))
            {
                job.CommandType = CommandTypes.WriteCmd;
            }
            else
            {
                job.CommandType = CommandTypes.ReadCmd;
            }
            job.ExecuteTask = true;
            job.TerminateTask = false;
            if (job.answer == null)
                job.answer = new byte[job.TotalJobSize];

            inList.Add(job);
        }

        private List<OmronEthernetIPCommJob> processDequeueListJob(List<OmronEthernetIPCommJob> inList, bool write, ConcurrentQueue<CommJob> queue)
        {
            if (write)
            {
                OmronEthernetIPProtocol.getWriteListLimitate(dequeueListJob, ref inList);
            }
            else
                OmronEthernetIPProtocol.getReadListLimitate(dequeueListJob, ref inList);

            foreach (OmronEthernetIPCommJob j in inList)
            {
                j.ExecuteTask = true;
                j.TerminateTask = false;

                if (write)
                    j.CommandType = CommandTypes.WriteCmd;
                else
                {
                    j.CommandType = CommandTypes.ReadCmd;
                    if (j.answer == null)
                        j.answer = new byte[j.TotalJobSize];
                }

                dequeueListJob.Remove(j);
            }
            foreach (OmronEthernetIPCommJob j in dequeueListJob)
            {
                queue.Enqueue(j);
            }
            dequeueListJob.Clear();
            foreach (OmronEthernetIPCommJob j in inList)
            {
                if (j.ReadTagEnd != 0 || j.PartialArrayEnd != 0)
                    dequeueListJob.Add(j);
            }
            return inList;
        }

        public void clearChannel()
        {
            lock (lockThreadObject)
            {
                foreach (OmronEthernetIPCommJob j in nextlist)
                {
                    j.ExecuteTask = false;
                }
                nextlist.Clear();
                if (ListJobPending.Count > 0)
                {
                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                    ProcessNewDataList(ListJobPending);
                    ListJobPending.Clear();
                }
                ListJobExecuted.Clear();
                checkDataFormatJobList.Clear();
                repeatOutputJobList.Clear();
                NextScheduleTimeJobsList = DateTime.UtcNow;
                LastErrorCode = DriverErrorCodes.ErrorNoError;
            }

        }
 
        protected void ExecuteJobList(ref List<OmronEthernetIPCommJob> list)
        {
            foreach (OmronEthernetIPCommJob j in list)
            {
                if (j == null)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure in ExecuteJobList null job num. of jobs: {1}", DateTime.UtcNow.ToString(), list.Count.ToString("00000")));
#endif
                    return;
                }
            }

            DateTime ExecutionTime = DateTime.UtcNow;
            foreach (OmronEthernetIPCommJob j in list)
            {
                base.ExecuteJob(j);
                j.LastExecutionTime = ExecutionTime;
                j.StartExecutionTime = ExecutionTime;
            }

            OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
            if (s == null)
            {
                return;
            }

            ReceiveClear();
                    
            if (OmronEthernetIPProtocol.Logix5550PrepareRequest(ref list, this, ref s, ref requestBuffer, ref requestBufferPointer) == 0)
            {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure in Logix5550PrepareRequest num. of jobs: {1}", DateTime.UtcNow.ToString(), list.Count.ToString("00000")));
#endif
                foreach (OmronEthernetIPCommJob j in list)
                {
                    RemovePendingJob(j);
                    j.IsPending = false;
                }

                return;
            }
            if (!OmronEthernetIPDeviceWrite(ref requestBuffer, ref requestBufferPointer))
            {
                OmronEthernetIPProtocol.Logix5550ForwardClose(s, ref requestBuffer, ref requestBufferPointer, this);
                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                return;
            }

            BeginDeviceRead(OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
        }


        bool AtLeastOneStationIsConnected()
        {
            List<Station> connectedStationList = (from station in CommDriver.GetChannelStations(this).AsParallel()
                                                  where (((OmronEthernetIPStation)station).ConnectionIDSet == true)
                                                  select station).ToList();
            return (connectedStationList.Count() > 0);
        }

        public void InvalidateStationConnections()
        {
            Parallel.ForEach(CommDriver.GetChannelStations(this), s =>
            {
                ((OmronEthernetIPStation)s).ClearConnection();
            });
        }

        bool ProcessNewDataList(List<CommJob> list)
        {
            if (LastErrorCode != (int)DriverErrorCodes.ErrorNoError)
            {
                if (LastErrorCode == DriverErrorCodes.ErrorTimeOut)
                {
                    ((OmronEthernetIPStation)list[0].Station).ClearConnection();
                    if (!AtLeastOneStationIsConnected())
                    {
                        ClearSession();
                    }
                }

                foreach (OmronEthernetIPCommJob j in list)
                {
                    j.LastExecutionTime = DateTime.UtcNow;
                    j.TerminateTask = false;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = j };
                    OnJobExecuted(eJob);
                }
                LastErrorCode = DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }


            bool bRet = false;
            
            if (list.Count != 0)
            {
                lock (lockList)
                {
                    OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
                    if (s != null)
                    {
                        if (ReceiveBuffer.Count >= OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)
                        {
                            ushortUnion dim = new ushortUnion(ReceiveBuffer, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
                            if (dim.USHORT != 0)
                            {
                                byte[] pdu = new byte[OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + dim.USHORT];
                                byte[] pduData = new byte[dim.USHORT];
                                ReceiveBuffer.CopyTo(pdu, 0);

                                uint numberOfReceivedBytes = 0;
                                uint numberOfBytesToBeRead = (uint)dim.USHORT;
                                uint numberOfReadBytes = 0;
                                byte[] readData = new byte[dim.USHORT];
                                DateTime readStartTime = DateTime.UtcNow;
                                do
                                {
                                    numberOfReadBytes = DeviceReadSynchronous(readData, numberOfBytesToBeRead);
                                    if (numberOfReadBytes > 0)
                                    {
                                        Array.Copy(readData, 0, pduData, numberOfReceivedBytes, numberOfReadBytes);
                                        numberOfReceivedBytes += numberOfReadBytes;
                                        numberOfBytesToBeRead -= numberOfReadBytes;
                                        if ((numberOfReceivedBytes < (uint)dim.USHORT))
                                        {
                                            StopWorkerThread.WaitOne(1, false);
                                        }
                                    }
                                } while ((numberOfReceivedBytes < (uint)dim.USHORT) && (numberOfReadBytes > 0));
                                if (numberOfReceivedBytes == (uint)dim.USHORT)
                                {
                                    bRet = true;
                                }

                                if (bRet) //error timeout RX
                                {
                                    pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);

                                    if (testTagNameTransaction(ref pdu, list, s))
                                    {
                                        switch (((OmronEthernetIPCommJob)list[0]).CommandType) {
                                            case CommandTypes.ReadCmd:
                                                GetReadData(ref pdu, list, s);
                                                break;
                                            case CommandTypes.ReadDataFormatCmd:
                                                GetReadDataFormat(ref pdu, list, s);
                                                ((OmronEthernetIPCommJob)list[0]).CommandType = CommandTypes.WriteCmd;
                                                break;
                                            case CommandTypes.WriteCmd:
                                                CheckTheWritingResponse(ref pdu, list, s);                                                
                                                break;
                                        }
                                    }
                                }
                            }
                            if (bRet == false)
                            {
                                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);                                                                
                                foreach (OmronEthernetIPCommJob j in list)
                                {
                                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorNoRep, Job = j };
                                    OnJobExecuted(eJob);
                                }
                            }
                        }
                    }
                }
            }

            ReceiveClear();
            return bRet;
        }

        private bool testTagNameTransaction(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            if (s.GetTransaction().USHORT == new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_TNS_OFFS).USHORT)
            {
                // when return false, all jobs are already set in error with OnJobExecuted(...);
                s.LastTransOK = Logix5550TagsCheckReply(ref pdu, s, list);
            }
            else
            {
                s.LastTransOK = false;
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorWrongTransaction, Job = j };
                    OnJobExecuted(eJob);
                }
            }
            return s.LastTransOK;
        }

        void GetReadData(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {            
            ushort readJobTag;
            ushort totalPduTag = 0;

            ExecutedJobArgs eJob;
            foreach (OmronEthernetIPCommJob j in list)
            {                
                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;

                readJobTag = 0;
                for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) && (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
                {
                    if (outErrorCode != DriverErrorCodes.ErrorNoError)
                        break;

                    Tag tag = (Tag)j.TagsList[tagIndex];
                    outErrorCode = copyTagData(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j);
                    readJobTag++;
                }
                totalPduTag += readJobTag;

                if (outErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    j.ReadTagStart = j.ReadTagEnd;
                    j.PartialArrayStart = j.PartialArrayEnd;

                    if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                    {
                        eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };

                        if (j.TagFormat == TagFormats.STRING)
                        {
                            byte[] jobAnswer = j.answer;
                            byte[] answer;
                            if (jobAnswer != null)
                            {
                                ushort answerSize = 0;
                                while (jobAnswer[answerSize] != 0 && answerSize < jobAnswer.Length)
                                    answerSize++;
                                answer = new byte[answerSize];
                                Array.Copy(jobAnswer, 0, answer, 0, answerSize);

                            }
                            else
                                answer = new byte[0];

                            eJob.Values = answer;
                        }
                        else
                        {
                            eJob.Values = j.answer;
                        }

                        OnJobExecuted(eJob);
                    }
                }
                else
                {
                    j.ReadTagStart = 0;
                    j.ReadTagEnd = 0;
                    j.PartialArrayStart = 0;
                    j.PartialArrayEnd = 0;

                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j };
                    OnJobExecuted(eJob);
                }
            }

            //ushort readJobTag;
            //ushort totalPduTag = 0;

            //foreach (OmronEthernetIPCommJob j in list)
            //{
            //    if (outErrorCode != DriverErrorCodes.ErrorNoError)
            //        break;

            //    readJobTag = 0;
            //    for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) &&
            //        (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
            //    {
            //        if (outErrorCode != DriverErrorCodes.ErrorNoError)
            //            break;

            //        Tag tag = (Tag)j.TagsList[tagIndex];
            //        outErrorCode = copyTagData(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j);
            //        readJobTag++;
            //    }
            //    totalPduTag += readJobTag;

            //    if (outErrorCode == DriverErrorCodes.ErrorNoError)
            //    {
            //        j.ReadTagStart = j.ReadTagEnd;
            //        j.PartialArrayStart = j.PartialArrayEnd;
            //    }
            //    else
            //    {
            //        j.ReadTagStart = 0;
            //        j.ReadTagEnd = 0;
            //        j.PartialArrayStart = 0;
            //        j.PartialArrayEnd = 0;
            //        break;
            //    }
            //}

            //ExecutedJobArgs eJob;
            //foreach (OmronEthernetIPCommJob j in list)
            //{
            //    eJob = new ExecutedJobArgs { ErrorCode = outErrorCode, Job = j };

            //    if (outErrorCode == DriverErrorCodes.ErrorNoError && j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
            //    {
            //        if (j.TagFormat == TagFormats.STRING)
            //        {
            //            byte[] jobAnswer = j.answer;
            //            byte[] answer;
            //            if (jobAnswer != null)
            //            {
            //                ushort answerSize = 0;
            //                while (jobAnswer[answerSize] != 0 && answerSize < jobAnswer.Length)
            //                    answerSize++;
            //                answer = new byte[answerSize];
            //                Array.Copy(jobAnswer, 0, answer, 0, answerSize);

            //            }
            //            else
            //                answer = new byte[0];

            //            eJob.Values = answer;
            //        }
            //        else
            //            eJob.Values = j.answer;
            //    }
            //    if (outErrorCode != DriverErrorCodes.ErrorNoError || (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0))
            //    {
            //        OnJobExecuted(eJob);
            //    }
            //}
        }

        /// <summary>
        /// Manage the request of correct data type of variable when write of value return specific error (used in 2 differents call)
        /// </summary>
        /// <param name="pdu"></param>
        /// <param name="list"></param>
        /// <param name="s"></param>
        void GetReadDataFormat(ref byte[] pdu,List<CommJob> list, OmronEthernetIPStation s)
        {            
            ushort totalPduTag = 0;
            
            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

            for (int i = 0; i < recieveReply.USHORT; i++)
            {                
                OmronEthernetIPCommJob j = (OmronEthernetIPCommJob)list[i];

                ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, ((OmronEthernetIPCommJob)list[0]).CommandType);

                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;

                // Special case (0x12018022): Invalid Parameter, Additional status = 0x8022 (wrong data type)
                // don't manage cipErrorCode with a different error code, probably is a different job on error
                if (cipErrorCode == 0 || cipErrorCode == 0x12018022) {
                    ushort readJobTag = 0;
                    for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) && (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
                    {
                        if (outErrorCode != DriverErrorCodes.ErrorNoError)
                            break;

                        Tag tag = (Tag)j.TagsList[tagIndex];
                        outErrorCode = copyTagDataFormat(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j, out bool tagFormatChanged);
                        // 1st call : write operation failed to because data type don't match
                        if (cipErrorCode == 0x12018022)
                            j.CommandType = CommandTypes.WriteCmd;                            

                        // 2st call : retrive from read correct data type
                        if (tagFormatChanged)
                        {
                            j.CommandType = CommandTypes.WriteCmd;
                            // force in error to keep alive job
                            outErrorCode = DriverErrorCodes.ErrorOverrunError;
                        }
                        readJobTag++;
                    }
                    totalPduTag += readJobTag;
                }

                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j };
                OnJobExecuted(eJob);                 
            }
        }

        private DriverErrorCodes copyTagDataFormat(Tag pendingTag, ref byte[] pdu, ushort tagIndex, OmronEthernetIPCommJob j, out bool tagFormatChanged)
        {
            tagFormatChanged = false;

            ushortUnion responseTag = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }

            TagFormats tagFormat = TagFormats.BOOL;
            DriverErrorCodes outErrorCode = getDataFormat(pdu, ref ReplyOffset, ref ReplyLength, ref tagFormat);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
                return outErrorCode;
            }

            if(j.TagFormat != tagFormat)
            {
                j.TagFormat = tagFormat;
                tagFormatChanged = true;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes copyTagData(Tag pendingTag, ref byte[] pdu, ushort tagIndex, OmronEthernetIPCommJob j)
        {
            ushortUnion responseTag = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }

            DriverErrorCodes outErrorCode = getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
                return outErrorCode;
            ushort tagReplyLength;
            if (j.PartialArrayEnd == j.PartialArrayStart)
                tagReplyLength = j.adjSize(pendingTag) ;
            else
            {
                if (j.PartialArrayEnd != 0)
                    tagReplyLength = (ushort)((j.PartialArrayEnd - j.PartialArrayStart) * j.ElemSize);
                else
                    tagReplyLength = (ushort)(j.adjSize(pendingTag) - j.PartialArrayStart * j.ElemSize);
            }

            ushort dataLength = ReplyLength;
            if(dataLength > tagReplyLength)
            {
                dataLength = tagReplyLength;
            }
            if (j.answer == null)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTagSize;
            }

            if (j.TagFormat == TagFormats.STRING)
            {
                // If the tag is a string, reset the contents of the job buffer, before copying the new value
                j.answer = new byte[j.TotalJobSize];
            }

            if (pendingTag.ByteOffset + dataLength > j.answer.Length)
            {
                dataLength = (ushort)(j.answer.Length - pendingTag.ByteOffset);
            }

            Array.Copy(pdu, ReplyOffset.USHORT, j.answer, pendingTag.ByteOffset + j.PartialArrayStart * j.ElemSize, dataLength);

            return DriverErrorCodes.ErrorNoError;
        }
        private DriverErrorCodes getDataBufferInit(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength)
        {
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // Structure element
                case 0xCC:
                    if (ReplyLength < 5)
                    {
                        //Path segment error
                        if(ReplyLength == 4)
                        {
                            return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorPathSegmentError;
                        }
                        else
                        {
                            return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                        }                        
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    return getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);


                // Structure
                case 0xA0:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    break;

                // BOOL
                case 0xC1:

                // SINT
                case 0xC2:

                // INT
                case 0xC3:

                // DINT
                case 0xC4:

                // USINT
                case 0xC6:

                // UINT
                case 0xC7:

                // UDINT
                case 0xC8:

                // REAL
                case 0xCA:

                // LREAL
                case 0xCB:

                // BYTE
                case 0xD1:

                // WORD
                case 0xD2:

                // DWORD
                case 0xD3:

                // LWORD
                case 0xD4:

                // LINT
                case 0xC5:

                // ULINT
                case 0xC9:

                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    ReplyOffset.USHORT += 2;
                    ReplyLength -= 2;
                    break;

                // STRING
                case 0xD0:
                    if (ReplyLength < 4)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    break;

                // ?
                default:
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
            }
            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes getDataFormat(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength, ref TagFormats tagFormat)
        {
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // Structure element
                case 0xCC:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    return getDataFormat( pdu, ref ReplyOffset, ref ReplyLength, ref tagFormat);                  

                // Structure
                case 0xA0:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.STRUCTURE;                    
                    break;

                // BOOL
                case 0xC1:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.BOOL;
                    break;

                // SINT
                case 0xC2:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.SINT;
                    break;

                // INT
                case 0xC3:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.INT;
                    break;

                // DINT
                case 0xC4:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.DINT;
                    break;

                // USINT
                case 0xC6:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.USINT;
                    break;

                // UINT
                case 0xC7:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.UINT;
                    break;

                // UDINT
                case 0xC8:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.UDINT;
                    break;

                // REAL
                case 0xCA:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.REAL;
                    break;

                // LREAL
                case 0xCB:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LREAL;
                    break;

                // BYTE
                case 0xD1:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.BYTE;
                    break;

                // WORD
                case 0xD2:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.WORD;
                    break;

                // DWORD
                case 0xD3:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.DWORD;
                    break;

                // LWORD
                case 0xD4:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LWORD;
                    break;

                // LINT
                case 0xC5:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LINT;
                    break;

                // ULINT
                case 0xC9:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.ULINT;
                    break;

                // STRING
                case 0xD0:
                    if (ReplyLength < 4)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.STRING;
                    break;

                // ?
                default:
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        bool Logix5550TagsCheckReply(ref byte[] pdu, OmronEthernetIPStation s, List<CommJob> list)
        {            
            OmronEthernetIPErrorCodes encapsulationErrorCode = AnalyzeReplyHeader(ref pdu, OmronEthernetIPProtocol.ecSendUnitData.USHORT, SessionHandle.UINT);
            if (encapsulationErrorCode != (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)encapsulationErrorCode, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            byte MRStatus = pdu[OmronEthernetIPProtocol.LOGIX5550_REP_GENSTS_OFFS];
            if ((MRStatus != 0) && (MRStatus != 0x1E)) // 0x1E == Embedded service error
            {
                int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
                switch((int)MRStatus)
                {
                    case 0x02:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus02;
                        break;

                    case 0x04:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus04;
                        break;

                    case 0x05:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus05;
                        break;

                    case 0x0C:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus0C;
                        break;

                    case 0x11:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus11;
                        break;

                    case 0x13:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus13;
                        break;

                    case 0x15:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus15;
                        break;

                    case 0x1F:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus1F;
                        break;

                    case 0x20:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus20;
                        break;

                    default:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus;
                        break;
                }

                // Length (in words) of the additional status  
                byte MRAddStatusSize = pdu[OmronEthernetIPProtocol.LOGIX5550_REP_GENSTSSZ_OFFS];

                if ((MRAddStatusSize > 0) && (omronErrorCode != (int)OmronEthernetIPErrorCodes.ErrorCipStatus))
                {
                    ushortUnion addInfo = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_GENSTSSZ_OFFS + 1));
                    switch((int)addInfo.USHORT)
                    {
                        case 0x102:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus102;
                            break;
                        case 0x104:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus104;
                            break;
                        case 0x1103:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus1103;
                            break;
                        case 0x2103:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2103;
                            break;
                        case 0x2104:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2104;
                            break;
                        case 0x8001:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8001;
                            break;
                        case 0x8007:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8007;
                            break;
                        case 0x8009:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8009;
                            break;
                        case 0x800F:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus800F;
                            break;
                        case 0x8010:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8010;
                            break;
                        case 0x8011:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8011;
                            break;
                        case 0x8017:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8017;
                            break;
                        case 0x8018:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8018;
                            break;
                        case 0x8021:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8021;
                            break;
                        case 0x8022:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8022;
                            break;
                        case 0x8023:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8023;
                            break;
                        case 0x8024:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8024;
                            break;
                        case 0x8025:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8025;
                            break;
                        case 0x8028:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8028;
                            break;
                        case 0x8029:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8029;
                            break;
                        case 0x8031:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8031;
                            break;
                    }
                }

                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)omronErrorCode, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

            if (recieveReply.USHORT != s.lastProcessedTags.USHORT)
            {
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepSerNum, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }


            // Special case when write values: Invalid Parameter, Additional status = 0x8022 (wrong data type)
            // put job in a separate list to obtain information about tag (ReadDataFormatCmd)
            if (list.Count> 0 && ((OmronEthernetIPCommJob)list[0]).CommandType == CommandTypes.WriteCmd)
            {
                for (int i = 0; i < recieveReply.USHORT; i++)
                {
                    ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                    ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                    int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, ((OmronEthernetIPCommJob)list[0]).CommandType);
                    if (cipErrorCode == 0x12018022)
                    {
                        if (i < list.Count)
                        {
                            OmronEthernetIPCommJob omronJob = (OmronEthernetIPCommJob)list[i];
                            omronJob.CommandType = CommandTypes.ReadDataFormatCmd;
                            checkDataFormatJobList.Add(omronJob);
                        }
                    }
                }               
            }

            return true;
        }
        
        void CheckTheWritingResponse(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            LastErrorMessage = "";

            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

            for (int i = 0; i < recieveReply.USHORT; i++)
            {
                OmronEthernetIPCommJob j = list[i] as OmronEthernetIPCommJob;

                ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, j.CommandType);
                // Special case: Invalid Parameter, Additional status = 0x8022 (wrong data type)
                // put job in a separate list to obtain information about tag
                if (cipErrorCode == 0x12018022)
                {

                }
                else
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)cipErrorCode, Job = j };
                    if (((DriverErrorCodes)cipErrorCode) == DriverErrorCodes.ErrorNoError)
                    {
                        if ((j.PartialArrayEnd != 0) && (j.CommandType == CommandTypes.WriteCmd) && (j.TagsList[0].TagNode.ArrayDimension > 0))
                        {
                            //System.Diagnostics.Debug.WriteLine("--- Debug --- j.PartialArrayStart:{0} j.PartialArrayEnd:{1}", job.PartialArrayStart, job.PartialArrayEnd);
                            j.PartialArrayStart = j.PartialArrayEnd;
                        }
                    } 
                    else
                    {
                        j.PartialArrayStart = 0;
                        j.PartialArrayEnd = 0;
                    }                    
                    // don't execute (close) job is incomplete
                    if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse OnJobExecuted {0}", j.TagsList[0].DynSettings.ToString()));
                        OnJobExecuted(eJob);
                    }
                }
            }
        }

        private int Logix5550TagsCheckSingleReply(ref byte[] pdu, List<CommJob> list, ushortUnion ReplyOffset, CommandTypes CommandType)
        {
            int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
            //String addressInfo = string.Format(OmronEthernetIP.Properties.Resources.AddressInfo,
            //        ((OmronEthernetIPCommJob)list[0]).ABAddress,
            //        ((OmronEthernetIPCommJob)list[0]).TotalJobSize,
            //        ((OmronEthernetIPCommJob)list[0]).TagsList[0].ByteOffset);	//Remember that the transaction was OK

            if ((CommandType == CommandTypes.ReadCmd && pdu[ReplyOffset.USHORT] != 0xCC) ||
                (CommandType == CommandTypes.WriteCmd && pdu[ReplyOffset.USHORT] != 0xCD))
            {
                omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorRepSerCode;
            }
            else
            {
                byte ReplyStatus = pdu[ReplyOffset.USHORT + 2];
                if (ReplyStatus != 0)
                {
                    switch ((int)ReplyStatus)
                    {
                        case 0x02:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus02;
                            break;

                        case 0x04:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus04;
                            break;

                        case 0x05:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus05;
                            break;

                        case 0x0C:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus0C;
                            break;

                        case 0x11:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus11;
                            break;

                        case 0x13:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus13;
                            break;

                        case 0x15:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus15;
                            break;

                        case 0x1F:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus1F;
                            break;

                        case 0x20:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus20;
                            break;

                        default:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus;
                            break;
                    }

                    // Length (in words) of the additional status  
                    byte AddStatusSize = pdu[ReplyOffset.USHORT + 3];
                    if ((AddStatusSize > 0) && (omronErrorCode != (int)OmronEthernetIPErrorCodes.ErrorCipStatus))
                    {
                        ushortUnion addInfo = new ushortUnion(pdu, (ushort)(ReplyOffset.USHORT + 4));
                        switch ((int)addInfo.USHORT)
                        {
                            case 0x102:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus102;
                                break;
                            case 0x104:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus104;
                                break;
                            case 0x1103:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus1103;
                                break;
                            case 0x2103:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2103;
                                break;
                            case 0x2104:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2104;
                                break;
                            case 0x8001:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8001;
                                break;
                            case 0x8007:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8007;
                                break;
                            case 0x8009:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8009;
                                break;
                            case 0x800F:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus800F;
                                break;
                            case 0x8010:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8010;
                                break;
                            case 0x8011:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8011;
                                break;
                            case 0x8017:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8017;
                                break;
                            case 0x8018:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8018;
                                break;
                            case 0x8021:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8021;
                                break;
                            case 0x8022:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8022;
                                break;
                            case 0x8023:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8023;
                                break;
                            case 0x8024:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8024;
                                break;
                            case 0x8025:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8025;
                                break;
                            case 0x8028:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8028;
                                break;
                            case 0x8029:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8029;
                                break;
                            case 0x8031:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8031;
                                break;
                        }
                    }
                }
            }

            return (omronErrorCode);
        }

        void PlcResponseError(byte stsCode, byte extStsCode, OmronEthernetIPCommJob mJob)
        {
            //String addressInfo = string.Format(OmronEthernetIP.Properties.Resources.AddressInfo,
            //        mJob.ABAddress, mJob.TotalJobSize, mJob.TagsList[0].ByteOffset);

            //String str;
            OmronEthernetIPErrorCodes OmronEthernetIPErrorCode;
            
            if (stsCode > 0 && stsCode < 10)
            {
            //    str = String.Format(OmronEthernetIP.Properties.Resources.ErrorStatusNotZero, stsCode.ToString("X"), PlcResponseErrorList1[stsCode - 1]);
            //}
            //else if (stsCode >= 0x10  && stsCode <= 0xE0)
            //{
            //    int idx = (stsCode >> 4);
            //    str = String.Format(OmronEthernetIP.Properties.Resources.ErrorStatusNotZero, stsCode.ToString("X"), PlcResponseErrorList2[idx]);
                OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorStatusNotZero;
            }
            else if (stsCode == 0xF0)
            {
                if (extStsCode >= 0x01 && extStsCode <= 0x24)
                {
                    //str = String.Format(OmronEthernetIP.Properties.Resources.ErrorEXTSTSCodeExt, extStsCode.ToString("X"), PlcResponseErrorList3[extStsCode]);
                    OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorEXTSTSCodeExt;
                }
                else
                {
                    //str = String.Format(OmronEthernetIP.Properties.Resources.ErrorUnknSTSEXTCode, extStsCode.ToString("X"));
                    OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorUnknSTSEXTCode;
                }
            }
            else
            {
                //str = String.Format(OmronEthernetIP.Properties.Resources.ErrorUnknSTSCode, stsCode.ToString("X"));
                OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorUnknSTSCode;
            }

            //str += addressInfo;

            //CommDriver.OnSystemEvent(ObjectIds.Server, str, Opc.Ua.EventSeverity.High);
            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCode, Job = mJob };
            OnJobExecuted(eJob);
        }

        public void ReceiveClear()
        {
            ReceiveBuffer.Clear();
            Flush();
        }
        
        public bool OmronEthernetIPDeviceWrite(ref byte[] TcpBuffer, ref ushortUnion TcpBufferSize)
        {
            //fill in packet data length
            if (TcpBufferSize.USHORT < OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - OmronEthernetIPDeviceWrite TcpBufferSize too short");
#endif
                return false;
            }
            ushort TcpDataSize = TcpBufferSize.USHORT;

            TcpBufferSize.USHORT -= OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE;  
            TcpBuffer[OmronEthernetIPProtocol.EDATA_LEN_OFFS] = TcpBufferSize.LOBYTE;				//Enc.data length lobyte
            TcpBuffer[OmronEthernetIPProtocol.EDATA_LEN_OFFS + 1] = TcpBufferSize.HIBYTE;			//Enc.data length hibyte

            return DeviceWrite(TcpBuffer, TcpDataSize);
        }

        public OmronEthernetIPErrorCodes AnalyzeReplyHeader(ref byte[] pdu, ushort Cmd, uint stationSHandle)    
        {
            ushortUnion CmdHeader = new ushortUnion(pdu, 0);
            uintUnion Status = new uintUnion(pdu, OmronEthernetIPProtocol.EDATA_STS_OFFS);
            uintUnion replySHandle = new uintUnion(pdu, OmronEthernetIPProtocol.EDATA_SESS_HND_OFFS);

            if( CmdHeader.USHORT != Cmd || stationSHandle !=replySHandle.UINT )
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorOutOfSync, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorOutOfSync;
            }
            //Examine the Encapsulation command status

            if (Status.UINT == 1)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus1, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus1;
            }
            else if (Status.UINT == 2)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus2, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus2;
            }
            else if (Status.UINT == 3)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus3, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus3;
            }
            else if (Status.UINT == 0x64)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus64, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus64;
            }
            else if (Status.UINT == 0x65)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus65, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus65;
            }
            else if (Status.UINT == 0x69)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, OmronEthernetIP.Properties.Resources.ErrorEncapStatus69, Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorEncapStatus69;
            }
            else if (Status.UINT != 0)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(OmronEthernetIP.Properties.Resources.ErrorUnknEncapStatus, Status.UINT), Opc.Ua.EventSeverity.High);
                return OmronEthernetIPErrorCodes.ErrorUnknEncapStatus;
            }
            return (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        public CommunicationDriver getCommDriver()
        {
            return CommDriver;
        }

        #endregion

        #region ErrorCodeArrayString

        protected string[] PlcResponseErrorList1 = 
        {
            OmronEthernetIP.Properties.Resources.ErrorSTSCode01,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode02,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode03,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode04,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode05,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode06,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode07,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode08,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode09Spare,
        };

        protected string[] PlcResponseErrorList2 = 
        {
            OmronEthernetIP.Properties.Resources.ErrorSTSCode10,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode20,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode30,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode40,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode50,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode60,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode70,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode80,
            OmronEthernetIP.Properties.Resources.ErrorSTSCode90,
            OmronEthernetIP.Properties.Resources.ErrorSTSCodeA0,
            OmronEthernetIP.Properties.Resources.ErrorSTSCodeB0,
            OmronEthernetIP.Properties.Resources.ErrorSTSCodeC0,
            OmronEthernetIP.Properties.Resources.ErrorSTSCodeD0,
            OmronEthernetIP.Properties.Resources.ErrorSTSCodeE0,
        };

        protected string[] PlcResponseErrorList3 = 
        {
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS01,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS02,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS03,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS04,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS05,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS06,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS07,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS08,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS09,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0A,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0B,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0C,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0D,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0E,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS0F,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS10,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS11,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS12,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS13,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS14,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS15,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS16,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS17,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS18,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS19,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1A,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1B,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1C,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1D,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1E,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS1F,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS20,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS21,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS22,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS23,
            OmronEthernetIP.Properties.Resources.ErrorEXTSTS24,
         };



        #endregion

        #region Properties

        #endregion
        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {

            if (StopWorkerThread != null)
                StopWorkerThread.Set();

            OmronEthernetIPProtocol.CloseSession(ref requestBuffer, ref requestBufferPointer, this);

            base.Dispose();

        }

        #endregion

    }
}

