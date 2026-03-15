using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using IpDriverCodeBase;
using System.Threading;
using System.Threading.Tasks;

namespace IEC61850
{
    public class IEC61850Channel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the IEC61850Channel object.
        /// </summary>
        public IEC61850Channel(CommunicationDriver commdriver, IEC61850ChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _ClientSessionSelector = settings.ClientSessionSelector;
            _ClientPresentationSelector = settings.ClientPresentationSelector;
            _ClientApplicationID = settings.ClientApplicationID;
            _ClientAEQualifier = settings.ClientAEQualifier;
            _ReportAutomaticActivation = settings.ReportAutomaticActivation;
            _TimeZone = settings.TimeZone;
            InvokeID = 0;            
        }

        #endregion

        #region Data Elements

        bool doBeginDeviceRead = true;
        bool setReportObjectsDone = false;

        public byte[] ClientSessionSelectorArray = null;
        public byte[] ClientPresentationSelectorArray = null;
        public byte[] ClientApplicationIDArray = null;
        public uint InvokeID = 0;
        IEC61850Station iec61850Station = null;
        public Dictionary<string, IEC61850Report> MapReports = new Dictionary<string, IEC61850Report>();
        public Dictionary<string, string> MapRptID = new Dictionary<string, string>();
        TimeZoneInfo channelTimeZoneInfo;
        enum UnconfirmedMessageError
        {
            NoError,
            MalformedMessage,
            JobPending
        }
        private DateTime pendingUnconfirmedMessageLastCheck = DateTime.UtcNow;
        private DateTime pendingUnconfirmedMessageLastRemoved = DateTime.UtcNow;
        public List<byte[]> pendingUnconfirmedMessage = new List<byte[]>();
        public object lockPendingUnconfirmedMessages = new object();
        private DateTime lastCommunicationTime = DateTime.UtcNow;
        const uint maxNumberOfConsecutiveUnsolicitedMessages = 100;

        #endregion

        #region Override Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Working thread. </summary>
        ///
        /// <param name="data" type="object">   The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;

            // Set the station of the channel
            List<Station> stationList = CommDriver.GetChannelStations(this);
            if((stationList != null) && (stationList.Count == 1))            
                iec61850Station = (IEC61850Station)stationList[0];

            // Set the TimeZoneInfo object for time conversion
            SetChannelTimeZoneInfo();

            pendingUnconfirmedMessageLastCheck = DateTime.UtcNow;

            while (true)
            {
                CommJob nextjob = null;
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    if (SynchroJob != null)
                        nextjob = SynchroJob;
                    else
                        nextjob = GetNextPendingJobIEC81650();
                    if (nextjob != null)
                        ListJobPending.Add(nextjob);
                }

                if (nextjob != null)
                {
                    bool isOpen = IEC61850IsDeviceOpen(iec61850Station);
                    if (!isOpen)
                        isOpen = IEC61850DeviceOpen(iec61850Station);

                    if(isOpen)
                    {
                        ExecuteJob(nextjob);
                    }
                    else
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = nextjob;
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)IEC61850ErrorCodes.ErrorConnectFailed;
                        OnJobExecuted(eJob);
                        ListJobPending.Clear();
                    }
                }

                bool dataReceived = false;
                lock (lockThreadObject)
                {
                    if (NewDataToAnlyze.WaitOne(0) || ReceiveBuffer.Count > 0)
                    {
                        dataReceived = true;
                        NewDataToAnlyze.Reset();
                        byte[] receivedMessage = null;
                        ReplyType replyType = ReplyType.Unrecognized;
                        if (ManageReceivedMessage(out receivedMessage, out replyType))
                        {
                            switch(replyType)
                            {
                                case ReplyType.ConfirmedResponse:
                                    if (ListJobPending.Count > 0)
                                    {
                                        foreach (var job in ListJobPending)
                                        {
                                            if (ProcessConfirmedReply(job, receivedMessage))
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
                                    break;

                                case ReplyType.Unconfirmed:
                                    ProcessUnconfirmedMessage(receivedMessage, ListJobPending.Count > 0);
                                    break;
                            }
                        }
                        else
                        {
                            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage failed");
                            if(ListJobPending.Count > 0)
                            {
                                ProcessNewData(ListJobPending[0]);
                                ListJobPending.RemoveAt(0);
                            }
                        }
                    }

                    if (ListJobPending.Count > 0)
                    {
                        if (!MultiPointProtocol)
                        {
                            double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                            if (dtime > Timeout)
                            {
                                IEC61850CommJob iecJob = (IEC61850CommJob)ListJobPending[0];
                                //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - Timeout - {0} - For {1} - Elapsed time = {2}", DateTime.Now, iecJob.MMSDataItemCompletePath, dtime));
                                if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                {
                                    ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    ListJobPending[0].ResetSynchro.Reset();
                                }
                                //error
                                if (GetPendingUnconfirmedMessageCount() > 0)
                                {
                                    LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorTimeout;
                                }
                                else
                                {
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                }
                                ProcessNewData(ListJobPending[0]);
                                ListJobPending.RemoveAt(0);
                                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - WorkingThread - {0} - Calling ReceiveBuffer.Clear", DateTime.Now));
                                ReceiveBuffer.Clear();
                                ManageTimeoutError();
                            }
                        }
                    }
                }

                // Check periodically if the connection is still alive, by sending an Identify request
                if ((_ActiveCommunicationCheckTime > 0) && !dataReceived && (nextjob == null) && (ListJobPending.Count == 0) && (iec61850Station != null))
                {
                    if ((DateTime.UtcNow - lastCommunicationTime).TotalMilliseconds > _ActiveCommunicationCheckTime)
                    {
                        if (IEC61850IsDeviceOpen(iec61850Station))
                        {
                            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Working Thread - {0} - Calling ReadIdentify", DateTime.Now);
                            bool identifyReceived = ReadIdentify();
                            lastCommunicationTime = DateTime.UtcNow;
                            if (identifyReceived == false)
                            {
                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Working Thread - {0} - Error in ReadIdentify", DateTime.Now);
                                iec61850Station.ManageGeneralError(DriverErrorCodes.ErrorTimeOut);
                                base.DeviceClose();
                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Working Thread - {0} - Calling 1 of DeviceClose", DateTime.Now);
                                DeviceClose();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Working Thread - {0} - ReadIdentify successfully performed", DateTime.Now);
                            }
                        }
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Working Thread - {0} - Calling 2 of DeviceClose", DateTime.Now);
                    DeviceClose();
                }

                if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }

                ProcessPedingUnconfirmedMessage(ListJobPending.Count > 0);
            }

            //// reset objects associated to reports and deactivate them when stopping communication 
            //UnSetReportObjects();
        }

        protected CommJob GetNextPendingJobIEC81650()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return null;

                for (int i = queue.Count; i > 0; i--)
                {
                    CommJob job = null;
                    if (!queue.TryDequeue(out job))
                        break;

                    if (!job.IsPending)
                    {
                        IEC61850CommJob iecJob = (IEC61850CommJob)job;
                        // process a report job only 1st time --> second time skip
                        if (!iecJob.bReportReaded)
                            return (job);
                    }
                    else
                    {
                        queue.Enqueue(job);
                    }
                }
            }

            return null;
        }

        void SetReportObjects()
        {
            if (iec61850Station == null)
                return;

            foreach(CommJob job in iec61850Station.GetListWholeJobCopy())
            {
                IEC61850CommJob iecJob = (IEC61850CommJob)job;
                if(!string.IsNullOrWhiteSpace(iecJob.MMSReportID))
                {
                    if (!MapReports.ContainsKey(iecJob.MMSReportCompletePath))
                        MapReports[iecJob.MMSReportCompletePath] = new IEC61850Report(iecJob.ReportLogicalDeviceName, iecJob.MMSReportID, iecJob.ReportType, this, iec61850Station);

                    //if(!MapReports[iecJob.MMSReportCompletePath].AddJob(iecJob))
                    if (!MapReports[iecJob.MMSReportCompletePath].GetStructAndAssignJobs(iecJob))
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - SetReportObjects failed AddJob for {0}", iecJob.MMSReportCompletePath));
                    }
                }
            }

            // Check if report ID is empty and force to a driver default value, then, if requested, activate the report 
            foreach (var report in MapReports.Values)
            {
                if ((report.SetReportID() == true) && _ReportAutomaticActivation)
                {
                    report.Activate();
                }
            }
        }


        void UnSetReportObjects()
        {
            setReportObjectsDone = false;

            if (iec61850Station == null)
                return;

            if (MapReports.Count > 0)
            {
                try
                {
                    if (_ReportAutomaticActivation && IsDeviceOpen())
                    {
                        foreach (var Rep in MapReports.Values)
                            Rep.DeActivate();
                    }
                }
                catch
                {
                }
                finally
                {
                    // reset job's internal variable 
                    Parallel.ForEach(iec61850Station.GetListWholeJobCopy(), job =>
                    {
                        ((IEC61850CommJob)job).ResetReportFlags();
                    });

                    ClearPendingUnconfirmedMessage();
                    MapReports.Clear();
                    MapRptID.Clear();
                }
            }
        }

        private bool isClosing = false;
        public override bool DeviceClose()
        {
            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - DeviceClose called", DateTime.Now);
            bool returnValue = true;
            try
            {
                if(!isClosing)
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - DeviceClose not already called", DateTime.Now);
                    isClosing = true;
                    // reset objects associated to reports and deactivate them when stopping communication 
                    UnSetReportObjects();
                    //setReportObjectsDone = false;
                    doBeginDeviceRead = true;
                    if (iec61850Station != null)
                    {
                        iec61850Station.IsConnected = false;
                    }
                    lastCommunicationTime = DateTime.UtcNow;
                    returnValue = base.DeviceClose();
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - DeviceClose done", DateTime.Now);
                    isClosing = false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - DeviceClose already called", DateTime.Now);
                    return (true);
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - DeviceClose catch block", DateTime.Now);
                isClosing = false;
            }

            return (returnValue);
        }

        /*public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToRead()
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToWrite()
        {
            throw new NotImplementedException();
        }
        */

        protected override void ManageTimeoutError()
        {
            doBeginDeviceRead = false;
        }

        uint GetNextInvokeID()
        {
            return (++InvokeID);
        }

        public void OnJobExecutedPublic(ExecutedJobArgs e)
        {
            OnJobExecuted(e);
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            // process a report job only 1st time --> second time
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && (!string.IsNullOrEmpty(((IEC61850CommJob)e.Job).MMSReportID) && !((IEC61850CommJob)e.Job).bReportReaded))
                ((IEC61850CommJob)e.Job).bReportReaded = true;
            base.OnJobExecuted(e);
        }

        public override void ExecuteJob(CommJob job)
        {
            IEC61850CommJob iec61850Job = job as IEC61850CommJob;
            if (iec61850Job == null)
                return;
            
            // The job has already sent its request --> Do nothing
            if (job.IsPending == true)
                return;

            var lkJob = iec61850Job.retLockList();
            byte[] requestBuffer = null;
            IEC61850Protocol P = new IEC61850Protocol();
            lock (lkJob)
            {
                // Job, associated to a report, that does not require the initialization of data --> Do nothing
                if (((iec61850Job.Type == LinkType.Input) || ((iec61850Job.Type == LinkType.InputOutput) && (iec61850Job.TagsListToWrite.Count == 0))) &&
                   !string.IsNullOrEmpty(iec61850Job.MMSReportID) &&
                   !iec61850Job.InitializeData)
                {
                    iec61850Job.bReportReaded = true;
                    if (job == SynchroJob)
                    {
                        iec61850Job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        iec61850Job.ResetSynchro.Reset();
                    }
                    ListJobPending.Remove(job);
                    iec61850Job.IsPending = false;
                    return;
                }

                base.ExecuteJob(job);
                if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (job.TagsListToWrite.Count == 0)
                        job.TagsListToWrite.AddRange(job.TagsList);
                }
                try
                {
                    requestBuffer = P.PrepareRequest(iec61850Job, GetNextInvokeID());
                }
                catch
                {
                    requestBuffer = null;
                }
            }
            if ((requestBuffer == null) || (requestBuffer.Length == 0))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ExecuteJob - Error in PrepareRequest");
                // Preparation of the request message failed --> Do nothing (job no more pending)
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }
            //lock (lockThreadObject)
            //{
            //    // Clear the receive buffer of the channel before sending the request
            //    ReceiveBuffer.Clear();
            //}
            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ExecuteJob - {0} - PrepareRequest OK for {1} - Request Length = {2}", DateTime.Now, iec61850Job.MMSDataItemCompletePath, requestBuffer.Length);

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ExecuteJob - Error in DeviceWrite");
                return;
            }

            if (doBeginDeviceRead)
            {
                lock (lockThreadObject)
                {
                    // Clear the receive buffer of the channel before sending the request
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - Execute - {0} - Calling ReceiveBuffer.Clear", DateTime.Now));
                    ReceiveBuffer.Clear();
                }
                if (!BeginDeviceRead(4)) // Read the header of the reply asynchronously
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ExecuteJob - Error in BeginDeviceRead");
                    return;
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ExecuteJob - {0} - BeginDeviceRead done", DateTime.Now));
                    doBeginDeviceRead = false;
                }
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ExecuteJob - {0} - BeginDeviceRead NOT done", DateTime.Now));
                doBeginDeviceRead = true;
            }
            job.LastExecutionTime = DateTime.UtcNow;
        }

        bool ManageReceivedMessage(out byte[] receivedMessage, out ReplyType replyType)
        {
            receivedMessage = null;
            replyType = ReplyType.Unrecognized;
            doBeginDeviceRead = true;
            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - Received {1} bytes", DateTime.Now, ReceiveBuffer.Count);
            if (ReceiveBuffer.Count != 4)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ManageReceivedMessage - {0} - Calling 1 ReceiveBuffer.Clear", DateTime.Now));
                ReceiveBuffer.Clear();
                Flush();
                if(!BeginDeviceRead(4)) // 4 == length of the header of an IEC 61850 message (TPKT)
                {
                    doBeginDeviceRead = true;
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - Error 1 in BeginDeviceRead");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ManageReceivedMessage - {0} - BeginDeviceRead 1 done", DateTime.Now));
                    doBeginDeviceRead = false;
                }
                LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 1", DateTime.Now);
                return (false);
            }

            // Read synchronously the rest of the reply message and then put the socket in listen mode, for receiving, possibly, unsolicited messages 
            byte[] header = new byte[4];
            Array.Copy(ReceiveBuffer.ToArray(), 0, header, 0, 4);
            byte[] replyMessage = ReadDeviceMessage(header);
            lock (lockThreadObject)
            {
                // Clear the receive buffer of the channel before sending the request
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ManageReceivedMessage - {0} - Calling 2 ReceiveBuffer.Clear", DateTime.Now));
                ReceiveBuffer.Clear();
            }
            if ((replyMessage == null) || (replyMessage.Length <= 4))
            {
                LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 2", DateTime.Now);
                return (false);
            }
            if (!BeginDeviceRead(4)) // 4 == length of the header of an IEC 61850 message (TPKT)
            {
                doBeginDeviceRead = true;
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - Error 2 in BeginDeviceRead", DateTime.Now);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ManageReceivedMessage - {0} - BeginDeviceRead 2 done", DateTime.Now));
                doBeginDeviceRead = false;
            }

            uint replyLength = (uint)replyMessage.Length;
            uint totalCheckedBytes = 4;
            uint bytesToBeChecked = replyLength - totalCheckedBytes;
            IEC61850Protocol protocolObject = new IEC61850Protocol();

            // Check the COTP header
            byte cotpSegmentType = 0;
            uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
            {
                LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 3", DateTime.Now);
                return (false);
            }

            // Check the session PDU
            byte spduType = 0;
            checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
            {
                LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 4", DateTime.Now);
                return (false);
            }

            byte ppduType = 0;
            //uint dataLength = 0;
            //DriverErrorCodes errorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
            switch (spduType)
            {
                case 1: // Give tokens
                    {
                        // Check the presentation layer
                        checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                        totalCheckedBytes += checkedBytes;
                        bytesToBeChecked -= checkedBytes;
                        if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                        {
                            LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 5", DateTime.Now);
                            return (false);
                        }

                        byte mpduType = 0;
                        switch (ppduType)
                        {
                            case 0x61: // Fully encoded data
                                checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                                totalCheckedBytes += checkedBytes;
                                bytesToBeChecked -= checkedBytes;
                                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                                {
                                    LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 6", DateTime.Now);
                                    return (false);
                                }

                                switch (mpduType)
                                {
                                    case 0xa1: // Confirmed Response
                                        receivedMessage = new byte[bytesToBeChecked];
                                        Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                                        replyType = ReplyType.ConfirmedResponse;
                                        break;

                                    case 0xa3: // Unconfirmed service
                                        receivedMessage = new byte[bytesToBeChecked];
                                        Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                                        replyType = ReplyType.Unconfirmed;
                                        break;

                                    default:
                                        LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 7", DateTime.Now);
                                        return (false);
                                }
                                break;

                            default:
                                LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 8", DateTime.Now);
                                return (false);
                        }
                    }
                    break;
                default:
                    LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ManageReceivedMessage - {0} - error 9", DateTime.Now);
                    return (false);
            }

            return true;
        }

        void ClearPendingUnconfirmedMessage()
        {
            lock (lockPendingUnconfirmedMessages)
            {
                if (pendingUnconfirmedMessage != null)
                {
                    pendingUnconfirmedMessage.Clear();
                }
            }
        }

        int GetPendingUnconfirmedMessageCount()
        {
            lock(lockPendingUnconfirmedMessages)
            {
                if(pendingUnconfirmedMessage == null)
                {
                    return (0);
                }
                else
                {
                    return (pendingUnconfirmedMessage.Count);
                }
            }
        }

        List<byte[]> GetAcopyAndEmptyPendingUnconfirmedMessage()
        {
            lock (lockPendingUnconfirmedMessages)
            {
                if (pendingUnconfirmedMessage == null)
                {
                    return (null);
                }
                List<byte[]> copyList = new List<byte[]>();
                copyList.AddRange(pendingUnconfirmedMessage);
                pendingUnconfirmedMessage.Clear();
                return (copyList);
            }
        }

        void AddToPendingUnconfirmedMessage(byte[] newMessage)
        {
            lock (lockPendingUnconfirmedMessages)
            {
                pendingUnconfirmedMessage.Add(newMessage);
            }
        }


        void AddAtTheBeginningOfPendingUnconfirmedMessage(List<byte[]> remainingMessages)
        {
            lock (lockPendingUnconfirmedMessages)
            {
                if ((pendingUnconfirmedMessage == null) || (remainingMessages == null) || (remainingMessages.Count < 1))
                {
                    return;
                }
                if(pendingUnconfirmedMessage.Count == 0)
                {
                    pendingUnconfirmedMessage.AddRange(remainingMessages);
                }
                else
                {
                    pendingUnconfirmedMessage.InsertRange(0, remainingMessages);
                }
            }
        }

        void ProcessPedingUnconfirmedMessage(bool waitingForConfirmedMessage = false)
        {
            if ((waitingForConfirmedMessage == false) &&
                (GetPendingUnconfirmedMessageCount() > 0) &&
                (setReportObjectsDone == true))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessPedingUnconfirmedMessage - {0} - Begin - pendingUnconfirmedMessage.Count = {1}", DateTime.Now, GetPendingUnconfirmedMessageCount()));
                int nextMessage = 0;
                int processedMessages = 0;
                uint processMaxTime = _PendingMessageProcessMaxTime;
                if(_PendingMessageProcessMaxTime < 1)
                {
                    processMaxTime = 1;
                }

                // Make a copy of the current list of received unconfirmed messages and then empty it
                List<byte[]> pendingMessages = GetAcopyAndEmptyPendingUnconfirmedMessage();
                if(pendingMessages == null)
                {
                    return;
                }

                DateTime loopStartingTime = DateTime.UtcNow;
                while ((nextMessage < pendingMessages.Count) && ((DateTime.UtcNow - loopStartingTime).TotalMilliseconds <= _PendingMessageProcessMaxTime))
                {
                    if (ProcessSingleUnconfirmedMessage(pendingMessages[nextMessage]) == UnconfirmedMessageError.NoError)
                    {
                        pendingUnconfirmedMessageLastRemoved = DateTime.UtcNow;
                        pendingMessages.RemoveAt(nextMessage);
                        processedMessages++;
                    }
                    else
                    {
                        nextMessage++;
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessPedingUnconfirmedMessage - {0} - End - pendingUnconfirmedMessage.Count = {1} - processedMessages = {2}", DateTime.Now, pendingMessages.Count, processedMessages));

                if (DateTime.UtcNow.Subtract(pendingUnconfirmedMessageLastRemoved) > new TimeSpan(0, 5, 0))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessPedingUnconfirmedMessage - {0} - Cleared pendingUnconfirmedMessage", DateTime.Now));
                }
                else if (pendingMessages.Count > 0)
                {
                    // Any messages still not processed? --> Process them later
                    AddAtTheBeginningOfPendingUnconfirmedMessage(pendingMessages);
                }
                pendingMessages.Clear();

                pendingUnconfirmedMessageLastCheck = DateTime.UtcNow;
            }
        }

        void ProcessUnconfirmedMessage(byte[] receivedMessage, bool waitingForConfirmedMessage = false)
        {
            // don't discard message --> process later
            AddToPendingUnconfirmedMessage(receivedMessage);
            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - ProcessUnconfirmedMessage - Message queued", DateTime.Now);
        }

        UnconfirmedMessageError ProcessSingleUnconfirmedMessage(byte[] receivedMessage)
        {
            pendingUnconfirmedMessageLastCheck = DateTime.UtcNow;

            //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessUnconfirmedMessage called - {0} - Message Length = {1}", DateTime.Now, receivedMessage.Length));

            if (receivedMessage == null || receivedMessage.Length == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage receivedMessage null or empty");
                return UnconfirmedMessageError.MalformedMessage;
            }

            uint totalCheckedBytes = 0;
            uint bytesToBeChecked = (uint)receivedMessage.Length;
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            IEC61850Protocol protocolObject = new IEC61850Protocol();
            uint checkedBytes = protocolObject.CheckInformationReport(receivedMessage);
            if (checkedBytes == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage error in CheckInformationReport");
                return UnconfirmedMessageError.MalformedMessage;
            }

            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage error in CheckInformationReport nr 1");
                return UnconfirmedMessageError.MalformedMessage;
            }

            string szReportID = string.Empty;
            checkedBytes = protocolObject.ParseInfoReportID(receivedMessage, totalCheckedBytes, bytesToBeChecked, out szReportID);
            if (checkedBytes == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage error in ParseInfoReportID");
                return UnconfirmedMessageError.MalformedMessage;
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage error in ParseInfoReportID nr 1");
                return UnconfirmedMessageError.MalformedMessage;
            }
                        
            if (string.IsNullOrEmpty(szReportID))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessUnconfirmedMessage error in ParseInfoReportID nr 2 --> Report ID Null");
                return UnconfirmedMessageError.MalformedMessage;
            }

            if (!MapRptID.ContainsKey(szReportID))
            {
                //// don't discard message --> process later
                //pendingUnconfirmedMessage.Add(receivedMessage);
                //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessUnconfirmedMessage {0} not found in m_mapRptID", szReportID));
                return UnconfirmedMessageError.JobPending;
            }

            string szReportControlBlock = MapRptID[szReportID];
            if (!MapReports.ContainsKey(szReportControlBlock))
            {
                //// don't discard message --> process later
                //pendingUnconfirmedMessage.Add(receivedMessage);
                //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessUnconfirmedMessage {0} not found in m_mapReports", szReportControlBlock));
                return UnconfirmedMessageError.JobPending;
            }

            IEC61850Report pReport = MapReports[szReportControlBlock];
            bool noActiveJobs;
            pReport.ParseInformationReport(receivedMessage, totalCheckedBytes+bytesToBeChecked, totalCheckedBytes, out noActiveJobs);
            if (noActiveJobs)
                return UnconfirmedMessageError.JobPending;

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessSingleUnconfirmedMessage OK - {0} - Message Length = {1}", DateTime.Now, receivedMessage.Length));

            return UnconfirmedMessageError.NoError;
        }

        bool ProcessConfirmedReply(CommJob pendingjob, byte[] receivedMessage)
        {
            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessConfirmedReply called - Message Length = {0}", receivedMessage.Length);
            lock (lockList)
            {
                if (pendingjob == null)
                {
                    return true;
                }

                IEC61850Protocol protocolObject = new IEC61850Protocol();
                uint dataLength = 0;
                DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
                uint checkedBytes = protocolObject.CheckMMSConfirmedResponse(receivedMessage, 0, (uint)receivedMessage.Length, (IEC61850CommJob)pendingjob, out dataLength, out errorCode);
                if (checkedBytes == 0)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessConfirmedReply - Error in CheckMMSConfirmedResponse");
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errorCode, Job = pendingjob };
                    OnJobExecuted(eJob);  
                    return true;
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessConfirmedReply - OK");
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    LastErrorMessage = String.Empty;
                    byte[] Answer;
                    Answer = new byte[dataLength];
                    Array.Copy(receivedMessage, checkedBytes, Answer, 0, dataLength);
                    eJob.Values = Answer;

                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);

                    return true;
                }
            }
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessNewData - {0} - Calling ReceiveBuffer.Clear", DateTime.Now));
                ReceiveBuffer.Clear();
                Flush();

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

                return true;
            }

            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessNewData - Shouldn't be performed!");

            lock (lockList)
            {
                // Check the length of the reply: it should be 4 == length of the ISO header
                if (ReceiveBuffer.Count != 4)
                {
                    if (pendingjob != null)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                        OnJobExecuted(eJob);
                    }
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessNewData - {0} - Calling 2 ReceiveBuffer.Clear", DateTime.Now));
                    ReceiveBuffer.Clear();
                    Flush();

                    return true;
                }

                // Read synchronously the rest of the reply message and then put the socket in listen mode, for receiving, possibly, unsolicited messages 
                byte[] header = new byte[4];
                Array.Copy(ReceiveBuffer.ToArray(), 0, header, 0, 4);
                byte[] replyMessage = ReadDeviceMessage(header);
                //ReceiveBuffer.Clear();
                //Flush();
                if(!BeginDeviceRead(4)) // 4 == length of the header of an IEC 61850 message (TPKT)
                {
                    doBeginDeviceRead = true;
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessNewData - {0} - BeginDeviceRead done", DateTime.Now));
                    doBeginDeviceRead = false;
                }
                if ((replyMessage == null) || (replyMessage.Length <= 4))
                {
                    if (pendingjob != null)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                        OnJobExecuted(eJob);
                    }

                    return true;
                }

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    if (pendingjob != null)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                        OnJobExecuted(eJob);
                    }

                    return true;
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    if (pendingjob != null)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                        OnJobExecuted(eJob);
                    }

                    return true;
                }

                byte ppduType = 0;
                uint dataLength = 0;
                DriverErrorCodes errorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                switch (spduType)
                {
                    case 1: // Give tokens
                        {
                            // Check the presentation layer
                            checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                            totalCheckedBytes += checkedBytes;
                            bytesToBeChecked -= checkedBytes;
                            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                            {
                                if (pendingjob != null)
                                {
                                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                                    OnJobExecuted(eJob);
                                }

                                return true;
                            }

                            byte mpduType = 0;
                            switch (ppduType)
                            {
                                case 0x61: // Fully encoded data
                                    checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                                    totalCheckedBytes += checkedBytes;
                                    bytesToBeChecked -= checkedBytes;
                                    if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                                    {
                                        if (pendingjob != null)
                                        {
                                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                                            OnJobExecuted(eJob);
                                        }

                                        return true;
                                    }

                                    switch (mpduType)
                                    {
                                        case 0xa1: // Confirmed Response
                                            // No pending job == unexpected Confirmed Response
                                            if (pendingjob == null)
                                            {
                                                if (pendingjob != null)
                                                {
                                                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                                                    OnJobExecuted(eJob);
                                                }

                                                return true;
                                            }
                                            else
                                            {
                                                checkedBytes = protocolObject.CheckMMSConfirmedResponse(replyMessage, totalCheckedBytes, bytesToBeChecked, (IEC61850CommJob)pendingjob, out dataLength, out errorCode);
                                                if(checkedBytes == 0)
                                                {
                                                    if (pendingjob != null)
                                                    {
                                                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errorCode, Job = pendingjob };
                                                        OnJobExecuted(eJob);
                                                    }

                                                    return true;
                                                }
                                                else
                                                {
                                                    totalCheckedBytes += checkedBytes;
                                                    bytesToBeChecked -= checkedBytes;
                                                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                                                    LastErrorMessage = "";

                                                    byte[] Answer;
                                                    Answer = new byte[dataLength];
                                                    Array.Copy(replyMessage, totalCheckedBytes, Answer, 0, dataLength);
                                                    eJob.Values = Answer;

                                                    eJob.Job = pendingjob;
                                                    OnJobExecuted(eJob);

                                                    return true;
                                                }
                                            }
                                    }
                                    break;
                            }
                        }
                        break;
                    default:
                        if (pendingjob != null)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply, Job = pendingjob };
                            OnJobExecuted(eJob);
                        }
                        return (true);
                }

                return true;
            }
        }
        #endregion

        #region Specific Methods

        const string NoTimeConversion = "(No Time Conversion)";

        bool UseUtcTimeForSourceTimestamp()
        {
            bool returnValue = true;
            if (_TimeZone == NoTimeConversion)
            {
                returnValue = false;
            }
            return (returnValue);
        }

        void SetChannelTimeZoneInfo()
        {
            if (String.IsNullOrWhiteSpace(_TimeZone) || (_TimeZone == NoTimeConversion))
            {
                channelTimeZoneInfo = TimeZoneInfo.Local;
            }
            else
            {
                try
                {
                    channelTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_TimeZone);
                }
                catch (Exception e)
                {
                    channelTimeZoneInfo = TimeZoneInfo.Local;
                }
            }
        }


        public DateTime ConvertTimeToUtc(DateTime timeToBeConverted)
        {
            DateTime convertedTime;
            try
            {
                if (UseUtcTimeForSourceTimestamp())
                {
                    convertedTime = TimeZoneInfo.ConvertTimeToUtc(timeToBeConverted, channelTimeZoneInfo);
                }
                else
                {
                    convertedTime = timeToBeConverted;
                }
            }
            catch (Exception e)
            {
                convertedTime = timeToBeConverted;
            }
            return (convertedTime);
        }

        private DateTime ConvertTimeFromUtc(DateTime timeToBeConverted)
        {
            DateTime convertedTime;
            try
            {
                convertedTime = TimeZoneInfo.ConvertTimeFromUtc(timeToBeConverted, channelTimeZoneInfo);
            }
            catch (Exception e)
            {
                convertedTime = timeToBeConverted;
            }
            return (convertedTime);
        }

        public void SetSessionSelectorArray(byte[] selectorArray)
        {
            if (selectorArray == null)
            {
                return;
            }
            int selectorLength = selectorArray.Length;
            ClientSessionSelectorArray = new byte[selectorLength];
            if (selectorLength > 0)
            {
                Array.Copy(selectorArray, 0, ClientSessionSelectorArray, 0, selectorLength);
            }
        }

        public void SetPresentationSelectorArray(byte[] selectorArray)
        {
            if (selectorArray == null)
            {
                return;
            }
            int selectorLength = selectorArray.Length;
            ClientPresentationSelectorArray = new byte[selectorLength];
            if (selectorLength > 0)
            {
                Array.Copy(selectorArray, 0, ClientPresentationSelectorArray, 0, selectorLength);
            }
        }

        public void SetApplicationIDArray(byte[] idArray)
        {
            if (idArray == null)
            {
                return;
            }
            int idLength = idArray.Length;
            ClientApplicationIDArray = new byte[idLength];
            if (idLength > 0)
            {
                Array.Copy(idArray, 0, ClientApplicationIDArray, 0, idLength);
            }
        }

        bool IEC61850IsDeviceOpen(IEC61850Station station)
        {
            if (station == null)
                return (false);
            
            if (!IsDeviceOpen())
            {
                return(false);
            }            
            else if(!station.IsConnected)
            {
                return (false);
            }

            return (true);
        }

        bool IEC61850DeviceOpen(IEC61850Station station)
        {
            lastCommunicationTime = DateTime.UtcNow;
            if (station == null)
                return (false);

            // If needed, open the socket
            if (!IsDeviceOpen())
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  IEC61850DeviceOpen - {0} - Calling DeviceClose", DateTime.Now);
                DeviceClose();                
                if (!DeviceOpen())
                    return false;
            }

            if (station.IsConnected)
            {
                return (true);
            }
            else
            {
                setReportObjectsDone = false;
            }

            // Station IEC-61850 connection
            if(!ConnectStation(station))
            {
                return (false);
            }
            else
            {                
                // Set the report objects
                SetReportObjects();
                doBeginDeviceRead = true;
                if(BeginDeviceRead(4) == true)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - IEC61850DeviceOpen - {0} - BeginDeviceRead done", DateTime.Now));
                    doBeginDeviceRead = false;
                }
                setReportObjectsDone = true;
                return (true);
            }
        }


        bool ConnectStation(IEC61850Station station)
        {
            if(station.IsConnected == true)
            {
                return (true);
            }

            // COTP connection
            if (!ConnectCOTP(station))
            {
                return (false);
            }

            // Establish association
            if (!Associate(station))
            {
                return (false);
            }

            station.IsConnected = true;
            InvokeID = 0;

            return (true);
        }

        bool Associate(IEC61850Station station)
        {
            // Build the Associate request message
            IEC61850Protocol protocolObject = new IEC61850Protocol();
            byte[] associateRequestBuffer = protocolObject.BuildAssociateRequest(station, this);

            // Send the Associate request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(associateRequestBuffer, (uint)associateRequestBuffer.Count()))
            {
                return (false);
            }

            // Synchronous read of the reply
            byte[] replyMessage = ReadDeviceReply();

            // Check the server reply
            if (replyMessage == null)
            {
                return (false);
            }
            uint replyLength = (uint)replyMessage.Length;
            if (replyLength <= 4)
            {
                return (false);
            }
            uint totalCheckedBytes = 4;
            uint bytesToBeChecked = replyLength - totalCheckedBytes;

            // Check the COTP header
            byte cotpSegmentType = 0;
            uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
            if(checkedBytes == 0)
            {
                return (false);
            }
            if(cotpSegmentType != 0x80)
            {
                return (false);
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                return (false);
            }

            // Check the session PDU
            checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, 0x0e, station);
            if (checkedBytes == 0)
            {
                return (false);
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                return (false);
            }

            // Check the presentation PDU
            checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, 0x31, station);
            if (checkedBytes == 0)
            {
                return (false);
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                return (false);
            }

            // Check the associate PDU
            checkedBytes = protocolObject.CheckAssociationControlService(replyMessage, totalCheckedBytes, bytesToBeChecked);
            if (checkedBytes == 0)
            {
                return (false);
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked == 0)
            {
                return (false);
            }

            // Check the initiate PDU
            checkedBytes = protocolObject.CheckMMSInitiatePDU(replyMessage, totalCheckedBytes, bytesToBeChecked);
            if (checkedBytes == 0)
            {
                return (false);
            }
            totalCheckedBytes += checkedBytes;
            bytesToBeChecked -= checkedBytes;
            if (bytesToBeChecked != 0)
            {
                return (false);
            }

            // Association successfully completed
            return (true);
        }

        public byte[] ReadVariableAccessAttributes(string szDeviceID, string szMMSDataItem)
        {
            byte[] dataBuffer = null;
            IEC61850Protocol P = new IEC61850Protocol();
            byte[] requestBuffer = P.BuildReadRequest(szMMSDataItem, szDeviceID, RequestTypes.ReadVariableAccessAttributes, GetNextInvokeID());
            if ((requestBuffer == null) || (requestBuffer.Length < 1))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Error in BuildReadRequest");
                return (null);
            }

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Error in DeviceWrite");
                return (null);
            }

            // Do a loop for receiving the reply because it is possible that the
            // received message be an unsolicited information report
            bool bReplyReceived = false;
            uint numberOfConsecutiveUnsolicitedMessages = 0;
            while (!bReplyReceived && (numberOfConsecutiveUnsolicitedMessages < maxNumberOfConsecutiveUnsolicitedMessages))
            {
                // Get the device reply

                // Synchronous read of the reply
                byte[] replyMessage = ReadDeviceReply();
                if (replyMessage == null)
                {
                    return (null);
                }

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                if (bytesToBeChecked == 0)
                {
                    return (null);
                }

                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error in CheckCOTPHeader");
                    return (null);
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error in CheckSessionLayer");
                    return (null);
                }
                if (spduType != 1)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - Wrong Session PDU type");
                    return (null);
                }

                // Check the presentation layer
                byte ppduType = 0;
                checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error in CheckPresentationLayer");
                    return (null);
                }
                if (ppduType != 0x61)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - Wrong presentation PDU type");
                    return (null);
                }

                byte mpduType = 0;
                checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error in CheckMMSLayer");
                    return (null);
                }

                switch (mpduType)
                {
                    case 0xa1: // Confirmed Response
                        {
                            uint dataBufferLength = 0;
                            // To be modified
                            checkedBytes = protocolObject.CheckMMSGetGetVarAccessAttribReply(replyMessage, totalCheckedBytes, bytesToBeChecked, out dataBufferLength);
                            totalCheckedBytes += checkedBytes;
                            bytesToBeChecked -= checkedBytes;
                            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error 6");
                                return (null);
                            }
                            // Copy the data part of the message
                            dataBuffer = new byte[dataBufferLength];
                            Array.Copy(replyMessage, totalCheckedBytes, dataBuffer, 0, dataBufferLength);
                            bReplyReceived = true;
                        }
                        break;

                    case 0xa3: // Unconfirmed service
                        {
                            byte[] receivedMessage = new byte[bytesToBeChecked];
                            Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                            ProcessUnconfirmedMessage(receivedMessage);
                        }
                        break;

                    default:
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadVariableAccessAttributes - error 7");
                        return (null);
                }
                numberOfConsecutiveUnsolicitedMessages++;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadVariableAccessAttributes - {0} - numberOfConsecutiveUnsolicitedMessages = {1}", DateTime.Now, numberOfConsecutiveUnsolicitedMessages));

            return (dataBuffer);
        }

        public byte[] ReadDataSetDirectory(string szDeviceID, string szDataSet)
        {
            byte[] dataBuffer = null;
            IEC61850Protocol P = new IEC61850Protocol();
            byte[] requestBuffer = P.BuildReadRequest(szDataSet, szDeviceID, RequestTypes.ReadDataSetDirectory, GetNextInvokeID());
            if ((requestBuffer == null) || (requestBuffer.Length < 1))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Error in BuildReadRequest");
                return (null);
            }

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Error in DeviceWrite");
                return (null);
            }

            // Do a loop for receiving the reply because it is possible that the
            // received message be an unsolicited information report
            bool bReplyReceived = false;
            uint nTotalReplyLength = 0;
            bool bFirstCOTPHeader = true;
            List<byte> m_pCotpBuffer = new List<byte>();
            uint numberOfConsecutiveUnsolicitedMessages = 0;
            while (!bReplyReceived && (numberOfConsecutiveUnsolicitedMessages < maxNumberOfConsecutiveUnsolicitedMessages))
            {
                // Get the device reply

                // Synchronous read of the reply
                byte[] replyMessage = ReadDeviceReply();
                if (replyMessage == null)
                {
                    return (null);
                }

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                if (bytesToBeChecked == 0)
                {
                    return (null);
                }
                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                if (checkedBytes == 0)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error in CheckCOTPHeader");
                    return (null);
                }

                switch (cotpSegmentType)
                {
                    // Following COTP segment
                    case 0:
                        if (bFirstCOTPHeader)
                        {
                            if (replyLength <= 13)
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 1");
                                return (null);
                            }
                            bFirstCOTPHeader = false;
                            nTotalReplyLength = replyLength;
                            m_pCotpBuffer.AddRange(replyMessage);
                            m_pCotpBuffer[6] = 0x80;
                            continue;
                        }
                        else
                        {
                            if (replyLength <= 7)
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 2");
                                return (null);
                            }
                            else if ((nTotalReplyLength + replyLength - 7) > (IEC61850Protocol.COTP_MAX_TPDU_SIZE + 4))
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 3");
                                return (null);
                            }
                            m_pCotpBuffer.AddRange(replyMessage.Skip(7));
                            nTotalReplyLength += replyLength - 7;
                            continue;
                        }

                    // Last COTP segment
                    case 0x80:
                        if (nTotalReplyLength > 0)
                        {
                            if (replyLength <= 7)
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 4");
                                return (null);
                            }
                            else if ((nTotalReplyLength + replyLength - 7) > (IEC61850Protocol.COTP_MAX_TPDU_SIZE + 4))
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 5");
                                return (null);
                            }
                            m_pCotpBuffer.AddRange(replyMessage.Skip(7));
                            nTotalReplyLength += replyLength - 7;
                            replyMessage = m_pCotpBuffer.ToArray();
                            ushort nMsgLength = (ushort)nTotalReplyLength;
                            replyMessage[2] = (byte)(nMsgLength >> 8);
                            replyMessage[3] = (byte)nMsgLength;
                            replyLength = nTotalReplyLength;
                            bytesToBeChecked = nTotalReplyLength - totalCheckedBytes;
                            bReplyReceived = true;
                        }
                        break;
                }

                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if (bytesToBeChecked == 0)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 6");
                    return (null);
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error in CheckSessionLayer");
                    return (null);
                }
                if (spduType != 1)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Wrong Session PDU type");
                    return (null);
                }

                // Check the presentation layer
                byte ppduType = 0;
                checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error in CheckPresentationLayer");
                    return (null);
                }
                if(ppduType != 0x61)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - Wrong presentation PDU type");
                    return (null);
                }

                byte mpduType = 0;
                checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error in CheckMMSLayer");
                    return (null);
                }

                switch (mpduType)
                {
                    case 0xa1: // Confirmed Response
                        {
                            uint dataBufferLength = 0;
                            // To be modified
                            checkedBytes = protocolObject.CheckMMSGetNamedVarListAttribReply(replyMessage, totalCheckedBytes, bytesToBeChecked, out dataBufferLength);
                            totalCheckedBytes += checkedBytes;
                            bytesToBeChecked -= checkedBytes;
                            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 6");
                                return (null);
                            }
                            dataBuffer = new byte[dataBufferLength];
                            Array.Copy(replyMessage, totalCheckedBytes, dataBuffer, 0, dataBufferLength);
                            bReplyReceived = true;
                        }
                        break;

                    case 0xa3: // Unconfirmed service
                        {
                            byte[] receivedMessage = new byte[bytesToBeChecked];
                            Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                            ProcessUnconfirmedMessage(receivedMessage);
                        }
                        break;

                    default:
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDataSetDirectory - error 7");
                        return (null);
                }
                numberOfConsecutiveUnsolicitedMessages++;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadDataSetDirectory - {0} - numberOfConsecutiveUnsolicitedMessages = {1}", DateTime.Now, numberOfConsecutiveUnsolicitedMessages));

            return (dataBuffer);
        }

        public byte[] ReadReportControlBlock(string deviceID, string mmsReportID)
        {
            byte[] dataBuffer = null;
            IEC61850Protocol P = new IEC61850Protocol();
            byte[] requestBuffer = P.BuildReadRequest(mmsReportID, deviceID, RequestTypes.ReadValue, GetNextInvokeID());
            if ((requestBuffer == null) || (requestBuffer.Length < 1))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - Error in BuildReadRequest");
                return (null);
            }

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - Error in DeviceWrite");
                return (null);
            }

            // Do a loop for receiving the reply because it is possible that the
            // received message be an unsolicited information report
            bool bReplyReceived = false;
            uint numberOfConsecutiveUnsolicitedMessages = 0;
            while (!bReplyReceived && (numberOfConsecutiveUnsolicitedMessages < maxNumberOfConsecutiveUnsolicitedMessages))
            {
                // Get the device reply

                // Synchronous read of the reply
                byte[] replyMessage = ReadDeviceReply();
                if (replyMessage == null)
                {
                    return (null);
                }

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                if (bytesToBeChecked == 0)
                {
                    return (null);
                }
                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error in CheckCOTPHeader");
                    return (null);
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error in CheckSessionLayer");
                    return (null);
                }

                byte ppduType = 0;
                switch (spduType)
                {
                    case 1: // Give tokens
                        {
                            // Check the presentation layer
                            checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                            totalCheckedBytes += checkedBytes;
                            bytesToBeChecked -= checkedBytes;
                            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error in CheckPresentationLayer");
                                return (null);
                            }

                            byte mpduType = 0;
                            switch (ppduType)
                            {
                                case 0x61: // Fully encoded data
                                    checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                                    totalCheckedBytes += checkedBytes;
                                    bytesToBeChecked -= checkedBytes;
                                    if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                                    {
                                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error 6");
                                        return (null);
                                    }

                                    switch (mpduType)
                                    {
                                        case 0xa1: // Confirmed Response
                                            {
                                                uint mmsDataLength = 0;
                                                checkedBytes = protocolObject.CheckMMSConfirmedReadResponse(replyMessage, totalCheckedBytes, bytesToBeChecked, out mmsDataLength);
                                                totalCheckedBytes += checkedBytes;
                                                bytesToBeChecked -= checkedBytes;
                                                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error 6");
                                                    return (null);
                                                }
                                                byte[] receivedMessage = new byte[mmsDataLength];
                                                Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, mmsDataLength);
                                                uint dataOffset = 1;
                                                uint dataBufferLength = 0;
                                                MMSDataTypes dataType = MMSDataTypes.Boolean;
                                                uint padding = 0;
                                                if (!protocolObject.ParseMMSData(receivedMessage, out dataType, out dataBufferLength, out dataOffset, out padding))
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error in ParseMMSData");
                                                    return (null);
                                                }
                                                if (dataType != MMSDataTypes.Structure)
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error unexpected Data Type");
                                                    return (null);
                                                }
                                                dataBuffer = new byte[dataBufferLength];
                                                Array.Copy(receivedMessage, dataOffset, dataBuffer, 0, dataBufferLength);
                                                bReplyReceived = true;
                                            }
                                            break;

                                        case 0xa3: // Unconfirmed service
                                            {
                                                byte[] receivedMessage = new byte[bytesToBeChecked];
                                                Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                                                ProcessUnconfirmedMessage(receivedMessage);
                                            }
                                            break;

                                        default:
                                            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - error 7");
                                            return (null);
                                    }
                                    break;

                                default:
                                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - wrong Presentation PDU type");
                                    return (null);
                            }
                        }
                        break;
                    default:
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadReportControlBlock - wrong Session PDU type");
                        return (null);
                }
                numberOfConsecutiveUnsolicitedMessages++;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadReportControlBlock - {0} - numberOfConsecutiveUnsolicitedMessages = {1}", DateTime.Now, numberOfConsecutiveUnsolicitedMessages));

            return (dataBuffer);
        }

        public bool ReadIdentify()
        {
            IEC61850Protocol P = new IEC61850Protocol();
            byte[] requestBuffer = P.BuildReadRequest(String.Empty, String.Empty, RequestTypes.ReadIdentify, GetNextInvokeID());
            if ((requestBuffer == null) || (requestBuffer.Length < 1))
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - Error in BuildReadRequest", DateTime.Now);
                return (false);
            }

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - Error in DeviceWrite", DateTime.Now);
                return (false);
            }

            // Do a loop for receiving the reply because it is possible that the
            // received message be an unsolicited information report
            bool bReplyReceived = false;
            uint numberOfConsecutiveUnsolicitedMessages = 0;
            while (!bReplyReceived && (numberOfConsecutiveUnsolicitedMessages < maxNumberOfConsecutiveUnsolicitedMessages))
            {
                // Get the device reply

                // Synchronous read of the reply
                byte[] replyMessage = ReadDeviceCompleteReply();
                if (replyMessage == null)
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - Error in ReadDeviceCompleteReply", DateTime.Now);
                    return (false);
                }

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                if (bytesToBeChecked == 0)
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - Error bytesToBeChecked == 0", DateTime.Now);
                    return (false);
                }
                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - error in CheckCOTPHeader", DateTime.Now);
                    return (false);
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadIdentify - {0} - error in CheckSessionLayer", DateTime.Now);
                    return (false);
                }

                byte ppduType = 0;
                switch (spduType)
                {
                    case 1: // Give tokens
                        {
                            // Check the presentation layer
                            checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                            totalCheckedBytes += checkedBytes;
                            bytesToBeChecked -= checkedBytes;
                            if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                            {
                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - error in CheckPresentationLayer", DateTime.Now);
                                return (false);
                            }

                            byte mpduType = 0;
                            switch (ppduType)
                            {
                                case 0x61: // Fully encoded data
                                    checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                                    totalCheckedBytes += checkedBytes;
                                    bytesToBeChecked -= checkedBytes;
                                    if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                                    {
                                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - in CheckMMSLayer", DateTime.Now);
                                        return (false);
                                    }

                                    switch (mpduType)
                                    {
                                        case 0xa1: // Confirmed Response
                                            {
                                                checkedBytes = protocolObject.CheckMMSIdentifyResponse(replyMessage, totalCheckedBytes, bytesToBeChecked);
                                                totalCheckedBytes += checkedBytes;
                                                bytesToBeChecked -= checkedBytes;
                                                if (checkedBytes == 0)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - error in CheckMMSIdentifyResponse", DateTime.Now);
                                                    return (false);
                                                }

                                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - Parsing successfully completed", DateTime.Now);
                                                bReplyReceived = true;
                                            }
                                            break;

                                        case 0xa3: // Unconfirmed service
                                            {
                                                byte[] receivedMessage = new byte[bytesToBeChecked];
                                                Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                                                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - Calling ProcessUnconfirmedMessage", DateTime.Now);
                                                ProcessUnconfirmedMessage(receivedMessage);
                                            }
                                            break;

                                        default:
                                            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - error unexpected MMS PDU Type", DateTime.Now);
                                            return (false);
                                    }
                                    break;

                                default:
                                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - wrong Presentation PDU type", DateTime.Now);
                                    return (false);
                            }
                        }
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  ReadIdentify - {0} - wrong Session PDU type", DateTime.Now);
                        return (false);
                }
                numberOfConsecutiveUnsolicitedMessages++;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadIdentify - {0} - numberOfConsecutiveUnsolicitedMessages = {1}", DateTime.Now, numberOfConsecutiveUnsolicitedMessages));

            if (bReplyReceived == true)
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} -  ReadIdentify successfully completed", DateTime.Now);
            }
            return (bReplyReceived);
        }

        bool ConnectCOTP(IEC61850Station station)
        {
            // Prepare the connection request
            IEC61850Protocol protocolObject = new IEC61850Protocol();
            byte[] cotpRequestBuffer = protocolObject.BuildCOTPConnectionRequest(station);
            if((cotpRequestBuffer == null) || (cotpRequestBuffer.Count() < 1))
            {
                return (false);
            }

            // Add the ISO header
            byte[] isoRequestBuffer = protocolObject.AddISOHeader(cotpRequestBuffer);
            if ((isoRequestBuffer == null) || (isoRequestBuffer.Count() < 1))
            {
                return (false);
            }

            // Send the connection request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(isoRequestBuffer, (uint)isoRequestBuffer.Count()))
            {
                return (false);
            }

            // Synchronous read of the reply
            byte[] replyMessage = ReadDeviceReply();
            if(replyMessage == null)
            {
                return (false);
            }

            // Check the reply of the server
            if((replyMessage.Count() <= 21) || (replyMessage[5] != 0xD0))
            {
                return (false);
            }

            // COTP connection successfully completed
            return (true);
        }

        // Asynchronous read of the header of a server message and synchronous read of the rest of the message
        byte[] ReadDeviceCompleteReply()
        {
            if (doBeginDeviceRead == false)
            {
                if (NewDataToAnlyze.WaitOne(TcpChannelReadTimeout) || ReceiveBuffer.Count > 0)
                {
                    NewDataToAnlyze.Reset();
                    doBeginDeviceRead = true;
                    if (ReceiveBuffer.Count != 4)
                    {
                        lock (lockThreadObject)
                        {
                            ReceiveBuffer.Clear();
                            Flush();
                        }
                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - Called ReceiveBuffer.Clear", DateTime.Now));
                        if (!BeginDeviceRead(4)) // 4 == length of the header of an IEC 61850 message (TPKT)
                        {
                            doBeginDeviceRead = true;
                            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - Error 1 in BeginDeviceRead", DateTime.Now);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - BeginDeviceRead 1 done", DateTime.Now));
                            doBeginDeviceRead = false;
                        }
                        LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - error 1", DateTime.Now);
                        return (null);
                    }

                    lastCommunicationTime = DateTime.UtcNow;

                    byte[] isoHeader = new byte[4];
                    Array.Copy(ReceiveBuffer.ToArray(), 0, isoHeader, 0, 4);

                    // Check the ISO header
                    if (isoHeader[0] != 3)
                    {
                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - error 2", DateTime.Now);
                        return (null);
                    }

                    // Read synchronously the rest of the reply message and then put the socket in listen mode, for receiving, possibly, unsolicited messages 
                    byte[] replyMessage = ReadDeviceMessage(isoHeader);
                    lock (lockThreadObject)
                    {
                        // Clear the receive buffer of the channel before sending a request
                        ReceiveBuffer.Clear();
                    }
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - Called 2 ReceiveBuffer.Clear", DateTime.Now));
                    if (!BeginDeviceRead(4)) // 4 == length of the header of an IEC 61850 message (TPKT)
                    {
                        doBeginDeviceRead = true;
                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - Error 2 in BeginDeviceRead", DateTime.Now);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - BeginDeviceRead 2 done", DateTime.Now));
                        doBeginDeviceRead = false;
                    }
                    if ((replyMessage == null) || (replyMessage.Length <= 4))
                    {
                        LastErrorCode = (DriverErrorCodes)IEC61850ErrorCodes.ErrorUnexpectedReply;
                        System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - error 3", DateTime.Now);
                        return (null);
                    }

                    return(replyMessage);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - error 4", DateTime.Now);
                    return (null);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ReadDeviceCompleteReply - {0} - Calling ReadDeviceReply", DateTime.Now);
                return (ReadDeviceReply());
            }
        }

        // Synchronous read of a server reply
        byte[] ReadDeviceReply()
        {
            // Read the ISO header (4 bytes)
            byte[] isoHeader = new byte[4];
            if(!DeviceRead(isoHeader, 4))
            {
                return (null);
            }
            lastCommunicationTime = DateTime.UtcNow;

            // Check the ISO header
            if (isoHeader[0] != 3)
            {
                return (null);
            }
            // Get the length of the reply
            uint replyLength = isoHeader[2];
            replyLength <<= 8;
            replyLength += isoHeader[3];
            // Check the reply length
            if((replyLength <= 4) || (replyLength > IEC61850Protocol.COTP_MAX_TPDU_SIZE + 4))
            {
                return (null);
            }

            // Read the remaining part of the reply
            byte[] replyBuffer = new byte[replyLength - 4];
            if (!DeviceRead(replyBuffer, replyLength - 4))
            {
                return (null);
            }
            lastCommunicationTime = DateTime.UtcNow;

            // Prepare the buffer with the complete message
            byte[] completeReply = new byte[replyLength];
            Array.Copy(isoHeader, 0, completeReply, 0, 4);
            Array.Copy(replyBuffer, 0, completeReply, 4, replyLength - 4);

            return (completeReply);
        }

        byte[] ReadDeviceMessage(byte[] header)
        {
            // Check the ISO header (4 bytes)
            if((header == null) || (header.Length != 4))
            {
                return (null);
            }
            byte[] isoHeader = new byte[4];
            Array.Copy(header, 0, isoHeader, 0, 4);
            if (isoHeader[0] != 3)
            {
                return (null);
            }
            // Get the length of the reply
            uint replyLength = isoHeader[2];
            replyLength <<= 8;
            replyLength += isoHeader[3];
            // Check the reply length
            if ((replyLength <= 4) || (replyLength > IEC61850Protocol.COTP_MAX_TPDU_SIZE + 4))
            {
                return (null);
            }

            // Read the remaining part of the reply
            byte[] replyBuffer = new byte[replyLength - 4];
            if (!DeviceRead(replyBuffer, replyLength - 4))
            {
                return (null);
            }
            lastCommunicationTime = DateTime.UtcNow;

            // Prepare the buffer with the complete message
            byte[] completeReply = new byte[replyLength];
            Array.Copy(isoHeader, 0, completeReply, 0, 4);
            Array.Copy(replyBuffer, 0, completeReply, 4, replyLength - 4);

            return (completeReply);
        }


        public bool WriteRCBItem(string szDeviceID, string szMMSDataItem, MMSDataTypes nItemType, byte[] pDataBuffer, byte nPadding /*= 0*/) {

            IEC61850Protocol P = new IEC61850Protocol();
            byte[] requestBuffer = P.BuildWriteRCBItemRequest(szMMSDataItem, szDeviceID, nItemType, pDataBuffer, nPadding, GetNextInvokeID());
            if ((requestBuffer == null) || (requestBuffer.Length < 1))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - Error in BuildWriteRCBItemRequest");
                return false;
            }

            // Send the request
            lastCommunicationTime = DateTime.UtcNow;
            if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - Error in DeviceWrite");
                return false;
            }

            // Do a loop for receiving the reply because it is possible that the
            // received message be an unsolicited information report
            bool bReplyReceived = false;
            uint numberOfConsecutiveUnsolicitedMessages = 0;
            while (!bReplyReceived && (numberOfConsecutiveUnsolicitedMessages < maxNumberOfConsecutiveUnsolicitedMessages))
            {
                // Get the device reply

                // Synchronous read of the reply
                byte[] replyMessage = ReadDeviceReply();
                if (replyMessage == null)
                    return false;

                uint replyLength = (uint)replyMessage.Length;
                uint totalCheckedBytes = 4;
                uint bytesToBeChecked = replyLength - totalCheckedBytes;
                if (bytesToBeChecked == 0)
                    return false;

                IEC61850Protocol protocolObject = new IEC61850Protocol();

                // Check the COTP header
                byte cotpSegmentType = 0;
                uint checkedBytes = protocolObject.CheckCOTPHeader(replyMessage, totalCheckedBytes, bytesToBeChecked, out cotpSegmentType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckCOTPHeader");
                    return false;
                }

                // Check the session PDU
                byte spduType = 0;
                checkedBytes = protocolObject.CheckSessionLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out spduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckSessionLayer");
                    return false;
                }
                if (spduType != 1)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - Wrong Session PDU type");
                    return false;
                }

                // Check the presentation layer
                byte ppduType = 0;
                checkedBytes = protocolObject.CheckPresentationLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, iec61850Station, out ppduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckPresentationLayer");
                    return false;
                }
                if (ppduType != 0x61)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - Wrong presentation PDU type");
                    return false;
                }

                byte mpduType = 0;
                checkedBytes = protocolObject.CheckMMSLayer(replyMessage, totalCheckedBytes, bytesToBeChecked, this, out mpduType);
                totalCheckedBytes += checkedBytes;
                bytesToBeChecked -= checkedBytes;
                if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckMMSLayer");
                    return false;
                }

                switch (mpduType)
                {
                    case 0xa1: // Confirmed Response
                        checkedBytes = protocolObject.CheckMMSWriteReply(replyMessage, totalCheckedBytes, bytesToBeChecked);
                        totalCheckedBytes += checkedBytes;
                        bytesToBeChecked -= checkedBytes;
                        if ((checkedBytes == 0) || (bytesToBeChecked == 0))
                        {
                            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckMMSWriteReply");
                            return false;
                        }

                        uint nErrorCode = 0;
                        if (!protocolObject.CheckMMSWriteResult(replyMessage, totalCheckedBytes + checkedBytes, bytesToBeChecked - checkedBytes, out nErrorCode))
                        {
                            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error in CheckMMSWriteResult");
                            return false;
                        }
                        bReplyReceived = true;

                        break;

                    case 0xa3: // Unconfirmed service
                        {
                            byte[] receivedMessage = new byte[bytesToBeChecked];
                            Array.Copy(replyMessage, totalCheckedBytes, receivedMessage, 0, bytesToBeChecked);
                            ProcessUnconfirmedMessage(receivedMessage);
                        }
                        break;

                    default:
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - WriteRCBItem - error 7");
                        return false;
                }
                numberOfConsecutiveUnsolicitedMessages++;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - WriteRCBItem - {0} - numberOfConsecutiveUnsolicitedMessages = {1}", DateTime.Now, numberOfConsecutiveUnsolicitedMessages));

            return (true);
        }

        #endregion

        #region IDisposable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG -  Dispose - {0} - Calling DeviceClose", DateTime.Now);
            DeviceClose();
            base.Dispose();
        }
        #endregion

        #region Properties

        private uint _TurnaroundDelay;
        /// <summary>
        /// Client Session Selector
        /// </summary>
        private string _ClientSessionSelector;
        public string ClientSessionSelector
        {
            get
            {
                return _ClientSessionSelector;
            }
            set
            {
                _ClientSessionSelector = value;
            }
        }

        /// <summary>
        /// Client Presentation Selector 
        /// </summary>
        private string _ClientPresentationSelector;
        public string ClientPresentationSelector
        {
            get
            {
                return _ClientPresentationSelector;
            }
            set
            {
                _ClientPresentationSelector = value;
            }
        }

        /// <summary>
        /// Client Application ID
        /// </summary>
        private string _ClientApplicationID;
        public string ClientApplicationID
        {
            get
            {
                return _ClientApplicationID;
            }
            set
            {
                _ClientApplicationID = value;
            }
        }

        /// <summary>
        /// Client AE Qualifier
        /// </summary>
        private uint _ClientAEQualifier;
        public uint ClientAEQualifier
        {
            get
            {
                return _ClientAEQualifier;
            }
            set
            {
                _ClientAEQualifier = value;
            }
        }

        /// <summary>
        /// Automatic Activation Of Reports
        /// </summary>
        private bool _ReportAutomaticActivation;
        public bool ReportAutomaticActivation
        {
            get
            {
                return _ReportAutomaticActivation;
            }
            set
            {
                _ReportAutomaticActivation = value;
            }
        }

        public uint TurnaroundDelay
        {
            get { return _TurnaroundDelay; }
            set
            {
                _TurnaroundDelay = value;
            }
        }

        private string _TimeZone;
        public string TimeZone
        {
            get { return _TimeZone; }
            set { _TimeZone = value; }
        }

        // Frequency of the periodically check of the connection when no messages are exchanged with the server.
        private uint _ActiveCommunicationCheckTime = Properties.Settings.Default.ActiveCommunicationCheckTime;

        // Maximum time of processing of the list of pending messages
        private uint _PendingMessageProcessMaxTime = Properties.Settings.Default.PendingMessageProcessMaxTime;

        #endregion

        #region methods

        #endregion
    }
}
