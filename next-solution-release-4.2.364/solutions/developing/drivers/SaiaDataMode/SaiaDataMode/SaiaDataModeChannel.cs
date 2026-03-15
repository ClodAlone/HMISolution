using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using IpDriverCodeBase;
using SerialDriverCodeBase;
using System.Threading;
using DriverCodeBase.Enumerators;

namespace SaiaDataMode
{
    class SaiaDataModeChannel : Channel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the Channel object.
        /// </summary>
        public SaiaDataModeChannel(CommunicationDriver commdriver, SaiaDataModeChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _ChannelType = settings.ChannelType;
            if(_ChannelType == ChannelTypes.Socket)
            {
                SaiaDataModeUdpChannelSettings UdpSetting = new SaiaDataModeUdpChannelSettings();
                UdpSetting.CopyProperties(settings);
                ChildChannel = new SaiaDataModeUdpChannel(commdriver, UdpSetting);
                ((SaiaDataModeUdpChannel)ChildChannel).ParentChannel = this;
            }
            else
            {
                SaiaDataModeSerialChannelSettings SerialSetting = new SaiaDataModeSerialChannelSettings();
                SerialSetting.CopyProperties(settings);
                ChildChannel = new SaiaDataModeSerialChannel(commdriver, SerialSetting);
                ((SaiaDataModeSerialChannel)ChildChannel).ParentChannel = this;
            }
            
        }

        #endregion

        #region Methods
        public void myOnJobExecuted(ExecutedJobArgs e)
        {
            DiagnLastTaskRxBytes = e.RxBytes;
            DiagnLastTaskTxBytes = e.TxBytes;
            if(e.ErrorCode == DriverErrorCodes.ErrorNoError)
                LastErrorMessage = "";
            base.OnJobExecuted(e);
        }

        #endregion

        #region Properties

        private ChannelTypes _ChannelType;
        public ChannelTypes ChannelType
        {
            get { return _ChannelType; }
            set
            {
                _ChannelType = value;
            }
        }
        
        #endregion

        #region Override Methods

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            bool bForceProcessListJobs = false;
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
                        ListJobPending.Add(nextjob);
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
                            }
                        }
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    DeviceClose();

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

        public override bool TestChannelComm()
        {
            return ChildChannel.TestChannelComm();
        }

        public override void ExecuteJob(CommJob job)
        {
            if (executedJob == null)
            {
                SaiaDataModeCommJob mJob = job as SaiaDataModeCommJob;
                executedJob = mJob;
                if (mJob == null)
                    return;
                ChildChannel.ExecuteJob(job);
                // discard here input output/exception output unchanged write value
                if (((SaiaDataModeCommJob)job).RemoveFromPendingJob)
                {
                    ((SaiaDataModeCommJob)job).RemoveFromPendingJob = false;
                    RemovePendingJob(job);
                }
            }
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {

                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eAJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                executedJob = null;
                return true;
            }

            return ChildChannel.ProcessNewData(pendingjob);
        }

        public override bool IsDeviceOpen()
        {
            return ChildChannel.IsDeviceOpen();
        }

        public override bool DeviceOpen()
        {
            return ChildChannel.DeviceOpen();
        }

        public override bool DeviceClose()
        {
            return ChildChannel.DeviceClose();
        }

        public override bool DeviceRead(byte[] buffer, uint count)
        {
            return ChildChannel.DeviceRead(buffer,count);
        }

        public override bool DeviceWrite(byte[] buffer, uint count)
        {
            return ChildChannel.DeviceWrite(buffer, count);
        }

        public override uint GetBytesToRead()
        {
            return ChildChannel.GetBytesToRead();
        }

        public override uint GetBytesToWrite()
        {
            return ChildChannel.GetBytesToWrite();
        }

        public override bool Startup()
        {
            return base.Startup();
        }

        #endregion
 
        #region Member
        Channel ChildChannel;
        public SaiaDataModeCommJob executedJob = null;

        #endregion

        #region Methods
        public bool InErrorState()
        {
            bool InErrorState = false;
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                if (station.InErrorState && station.GetChannel() == this)
                {
                    InErrorState = true;
                }
                else
                {
                    InErrorState = false;
                    break;
                }
            }
            return InErrorState;
        }
        #endregion

        #region IDisposable

        public override void Dispose()
        {
            ChildChannel.Dispose();
            base.Dispose();
        }
        #endregion

    }
 
    class SaiaDataModeUdpChannel : UdpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SaiaDataModeUdpChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        #endregion

        #region Override Methods
        public override void ExecuteJob(CommJob job)
        {
            SaiaDataModeCommJob mJob = job as SaiaDataModeCommJob;

            if (job.IsPending == true)
            {
                return;
            }

            if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            {
                lock (job.retLockList())
                {
                    if (job.TagsListToWrite.Count == 0)
                        job.TagsListToWrite.AddRange(job.TagsList);
                }
            }

            base.ExecuteJob(job);

            GestReadCommand = job.ReadRequest();

            uint count = GetAnswerFrameLength(mJob);
            byte[] pdu = new byte[count];
            count = PrepareRequest(mJob, ref pdu);
            if(count == 0)
            {
                lock (lockThreadObject)
                {
                    if (SynchroJob != null && SynchroJob == job)
                    {
                        job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        job.ResetSynchro.Reset();
                    }
                    // child channel don't contain jobs inside PendingList --> will be removed from main channel
                    ((SaiaDataModeCommJob)job).RemoveFromPendingJob = true;
                    job.IsPending = false;
                }
                return;
            }
            Flush();

            if (!DeviceWrite(pdu, count))
            {
                return;
            }

            BeginDeviceRead(SaiaDataModeProtocol.ReplyOverhead);

            job.LastExecutionTime = DateTime.UtcNow;
        }
 
        public override void SetNewDataEvent()
        {
            ParentChannel.SetNewDataEvent();
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (ReceiveBuffer.Count == 0)
                return false;

            lock (lockList)
            {
                if (pendingjob == null ||
                    pendingjob.Station as SaiaDataModeStation == null ||
                    ReceiveBuffer.Count < SaiaDataModeProtocol.ReplyOverhead)
                {
                    Flush();
                    return false;
                }

                SaiaDataModeStation s = pendingjob.Station as SaiaDataModeStation;
                SaiaDataModeCommJob mJob = pendingjob as SaiaDataModeCommJob;
                if (mJob == null)
                {
                    Flush();
                    return false;
                }

                ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };
                uintUnion ReplyLength = new uintUnion(ReceiveBuffer, 0);
                if ((ReplyLength.UINT < SaiaDataModeProtocol.ReplyOverhead)
                    || (ReplyLength.UINT > SaiaDataModeProtocol.MaxMessageLength))
                {
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                    OnJobExecuted(eJob);
                    Flush();
                    ParentChannel.executedJob = null;
                    return true;
                }

                byte[] pdu = new byte[ReplyLength.UINT];
                ReceiveBuffer.CopyTo(0,pdu,0, (int)ReplyLength.UINT);

                // Check the reply
                if (!CheckReply(pdu, mJob,s))
                {
                    Flush();
                    ParentChannel.executedJob = null;
                    return true;
                }

                byte[] Answer;
                if (GestReadCommand)
                {
                    // Copy received data
                    Answer = new byte[mJob.GetExpectedDataLength(GestReadCommand)];
                    Array.Copy(pdu, (int)SaiaDataModeProtocol.DataOffs, Answer, 0, (int)mJob.GetExpectedDataLength(GestReadCommand));
                }
                else
                {
                    Answer = new byte[0];
                }
                eJob.Values = Answer;

                Flush();
                OnJobExecuted(eJob);

                ParentChannel.executedJob = null;
                return true;
            }
        }

        protected override void WorkingThread(object data)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {

            lock (lockStream)
            {
                bool returnValue = base.IsDeviceOpen();
                SetStateCommandVariableBit(((!returnValue) || ParentChannel.InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }
        }

        #endregion

        #region static const

        public const uint AttributeOffs = 8;
        public const uint AckNakCodeOffs = 9;
        const ushort MaxBufferSize = 1024;
        const byte MinMessageLength = 12;

        #endregion

        #region Methods

        public uint GetAnswerFrameLength(SaiaDataModeCommJob job)
        {
            return MaxBufferSize;
        }

        public uint GetResponseFrameLength(SaiaDataModeCommJob job)
        {
            return MaxBufferSize;
        }

        public void Flush()
        {
            if (GetBytesToRead() != 0)
            {
                byte[] Buffer = new byte[0];
                DeviceRead(Buffer, 0);
            }
            ReceiveBuffer.Clear();
        }

        public uint PrepareRequest(SaiaDataModeCommJob j, ref byte[] buffer)
        {
            if (j == null || (j.Station as SaiaDataModeStation == null))
                return 0;

            lock (j.retLockList())
            {
                SaiaDataModeStation s = j.Station as SaiaDataModeStation;
                uintUnion Count = new uintUnion(0);
                uint indexQuantity;

                // Message length (to be set later)
                buffer[Count.UINT++] = 0;
                buffer[Count.UINT++] = 0;
                buffer[Count.UINT++] = 0;
                buffer[Count.UINT++] = 0;
                // Version
                buffer[Count.UINT++] = 1;
                // Protocol type
                buffer[Count.UINT++] = 0;
                // Sequence
                s.GetRequestSequence(ref buffer, ref Count);
                // Telegram attribute = request
                buffer[Count.UINT++] = 0;
                // Destination
                buffer[Count.UINT++] = s.StationID;
                // Command
                buffer[Count.UINT++] = j.GetCommand();
                // Quantity
                indexQuantity = Count.UINT;
                buffer[Count.UINT++] = j.GetQuantity(GestReadCommand, 0);
                // DB number
                if (j.AreaType == AreaTypes.DataBlock)
                {
                    buffer[Count.UINT++] = j.DbNumber.HIBYTE;
                    buffer[Count.UINT++] = j.DbNumber.LOBYTE;
                }

                if (!GestReadCommand)
                {
                    // Base address
                    ushortUnion StartAddress = new ushortUnion(j.GetWriteAddres());
                    buffer[Count.UINT++] = StartAddress.HIBYTE;
                    buffer[Count.UINT++] = StartAddress.LOBYTE;

                    // Quantity of bits
                    if (j.IsBitDataFormat())
                    {
                        buffer[Count.UINT++] = j.GetWriteBitQuantity();
                    }

                    // Data
                    object jobData = null;
                    j.GetJobData(ref jobData);
                    // discard inputoutput/exception output unchanged write value operation                    
                    if (j.TagsListOnWriting.Count == 0 || jobData == null)
                    {
                        return (0);
                    }

                    byte[] tmpBuffer = (byte[])jobData;
                    for (int i = 0; i < tmpBuffer.Count(); i++)
                        buffer[Count.UINT++] = tmpBuffer[i];
                    buffer[indexQuantity] = j.GetQuantity(false, (uint)tmpBuffer.Count());
                }
                else
                {
                    // Base address
                    buffer[Count.UINT++] = j.StartAddress.HIBYTE;
                    buffer[Count.UINT++] = j.StartAddress.LOBYTE;
                }

                Count.UINT += 2;
                // Set the message length
                buffer[0] = Count.HIUSHORT.HIBYTE;
                buffer[1] = Count.HIUSHORT.LOBYTE;
                buffer[2] = Count.LOUSHORT.HIBYTE;
                buffer[3] = Count.LOUSHORT.LOBYTE;

                Count.UINT -= 2;
                // Append the CRC to the message
                ushortUnion nCRC = SaiaDataModeProtocol.CalculateCRC(buffer, Count.UINT);
                buffer[Count.UINT++] = nCRC.HIBYTE;
                buffer[Count.UINT++] = nCRC.LOBYTE;

                return Count.UINT;
            }
        }

        bool CheckReply(byte[] pdu, SaiaDataModeCommJob pendingjob, SaiaDataModeStation s)
        {
            ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
            eAJob.Job = pendingjob;
            // Check the reply length
            if (pdu.Count() < MinMessageLength)
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eAJob);
                return false;
            }

            // Check the CRC
            if (!SaiaDataModeProtocol.CheckCRC(pdu))
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorCrc;
                OnJobExecuted(eAJob);
                return false;
            }
            
            // Check the sequence number
            if (!s.CheckRequestSequence((new ushortUnion(pdu, (ushort)SaiaDataModeProtocol.SequenceOffs).USHORT)))
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWrongSequence;
                OnJobExecuted(eAJob);
                return false;
            }

            // Check data length
            if (pendingjob.GetExpectedDataLength(GestReadCommand) != pdu.Count() - SaiaDataModeProtocol.ReplyOverhead)
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseLength;
                OnJobExecuted(eAJob);
                return false;
            }


            // Check the reply attribute
            switch ((ReplyAttributes)pdu[AttributeOffs])
            {
                case ReplyAttributes.Response:
                    if (GestReadCommand)
                    {
                        return true;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }

                case ReplyAttributes.ACK_NAK:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }
                    else
                    {
                        ushortUnion Aux = new ushortUnion(pdu, (ushort)AckNakCodeOffs);
                        if (Aux.USHORT == 0)
                        {
                            return true;
                        }
                        else
                        {
                            eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseNack;
                            OnJobExecuted(eAJob);
                            return false;
                        }
                    }

                // Unexpected attribute
                default:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                    }
                    OnJobExecuted(eAJob);
                    return false;
            }
        }

      #endregion

        #region Properties

        #endregion

        #region Member
        public SaiaDataModeChannel ParentChannel;
        bool GestReadCommand;

        #endregion

        #region IChannelBase Interface

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if (ParentChannel != null)
            {
                e.RxBytes = DiagnLastTaskRxBytes;
                DiagnLastTaskRxBytes = 0;
                e.TxBytes = DiagnLastTaskTxBytes;
                DiagnLastTaskTxBytes = 0;
                ParentChannel.myOnJobExecuted(e);
            }
        }
        
        #endregion

    }

    class SaiaDataModeSerialChannel : SerialChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SaiaDataModeSerialChannel(CommunicationDriver commdriver, SerialChannelSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region Override Methods
        public override void ExecuteJob(CommJob job)
        {
            SaiaDataModeCommJob mJob = job as SaiaDataModeCommJob;

            if (job.IsPending == true)
            {
                return;
            }

            if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            {
                lock (job.retLockList())
                {
                    if (job.TagsListToWrite.Count == 0)
                        job.TagsListToWrite.AddRange(job.TagsList);
                }
            }

            base.ExecuteJob(job);

            GestReadCommand = job.ReadRequest();

            uint count = GetAnswerFrameLength(mJob);
            byte[] pdu = new byte[count];
            count = PrepareRequest(mJob, ref pdu);
            if (count == 0)
            {
                lock (lockThreadObject)
                {
                    if (SynchroJob != null && SynchroJob == job)
                    {
                        job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        job.ResetSynchro.Reset();
                    }
                    // child channel don't contain jobs inside PendingList --> will be removed from main channel
                    ((SaiaDataModeCommJob)job).RemoveFromPendingJob = true;
                    job.IsPending = false;
                }
                return;
            }
            Flush();

            if (!DeviceWrite(pdu, count))
            {
                return;
            }

            job.LastExecutionTime = DateTime.UtcNow;
            MessageLength = 0;
        }

        public override void SetNewDataEvent()
        {
            ParentChannel.SetNewDataEvent();
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (ReceiveBuffer.Count == 0)
                return false;

            if (pendingjob as SaiaDataModeCommJob == null ||
               pendingjob.Station as SaiaDataModeStation == null)
            {
                ParentChannel.executedJob = null;
                return true;
            }

            SaiaDataModeStation s = pendingjob.Station as SaiaDataModeStation;
            SaiaDataModeCommJob mJob = pendingjob as SaiaDataModeCommJob;
            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };

            // Read the initial character
            if (MessageLength < 2)
            {
                if (MessageLength == 0)
                {
                    if (!ReceiveBufferPull(MessageBuffer, ref MessageLength))
                    {
                        return false;
                    }
                    if (MessageBuffer[0] != 0xB5)
                    {
                        MessageLength = 0;
                        return false;
                    }
                }

                // Read the telegram attribute
                if (!ReceiveBufferPull(MessageBuffer, ref MessageLength))
                {
                    return false;
                }
                if ((MessageBuffer[1] != 1) && (MessageBuffer[1] != 2))
                {
                    MessageLength = 0;
                    return false;
                }
                ReceivedChars = 0;
            }

            // Read response with data
            if (MessageBuffer[1] == 1)
            {
                if (mJob.GetExpectedDataLength(GestReadCommand) == 0)
                {
                    while (ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars)) ;
                }
                else
                {
                    while (ReceivedChars < mJob.GetExpectedDataLength(GestReadCommand) + 2)
                    {
                        if (!ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            { // Read ACK/NAK reply
                while (ReceivedChars < 4)
                {
                    if (!ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars))
                    {
                        return false;
                    }
                }
            }

            byte[] Answer;
            if (MessageLength != 0)
            {
                // Check the reply
                if (!CheckReply(MessageBuffer, MessageLength, mJob, s))
                {
                    ParentChannel.executedJob = null;
                    return true;
                }
                if (GestReadCommand)
                {
                    // Copy received data
                    Answer = new byte[mJob.GetExpectedDataLength(GestReadCommand)];
                    Array.Copy(AnswerBuffer,Answer,Answer.Length);
                }
                else
                    Answer = new byte[0];
            }
            else
                Answer = new byte[0];

            eJob.Values = Answer;

            OnJobExecuted(eJob);

            ParentChannel.executedJob = null;
            return true;

        }
        
        bool CheckReply(byte[] Buffer, uint ReplyLength, SaiaDataModeCommJob pendingjob, SaiaDataModeStation s)
        {
            ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
            eAJob.Job = pendingjob;
            // Check the reply length
            if (ReplyLength < MinMessageLength)
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eAJob);
                return false;
            }

            // Check the CRC
            uint MessageLength = ReplyLength - 2;
            // Escape sequences in the CRC bytes?
            if ((Buffer[ReplyLength - 4] == 0xC5) &&
               (Buffer[ReplyLength - 2] == 0xC5))
            {
                MessageLength = ReplyLength - 4;
            }
            else if ((Buffer[ReplyLength - 3] == 0xC5) ||
                    (Buffer[ReplyLength - 2] == 0xC5))
            {
                MessageLength = ReplyLength - 3;
            }

            ushortUnion CalculatedCRC = SaiaDataModeProtocol.CalculateCRC(Buffer, MessageLength);
            ushortUnion ReceivedCRC = new ushortUnion(Buffer,(ushort)(ReplyLength - 2));
            // Escape sequences in the CRC bytes?
            if (MessageLength == (ReplyLength - 3))
            {
                if (Buffer[ReplyLength - 3] == 0xC5)
                {
                    if (Buffer[ReplyLength - 2] == 0)
                    {
                        ReceivedCRC.HIBYTE = 0xB5;
                    }
                    else
                    {
                        ReceivedCRC.HIBYTE = 0xC5;
                    }
                }
                else
                {
                    ReceivedCRC.HIBYTE = Buffer[ReplyLength - 3];
                    if (Buffer[ReplyLength - 1] == 0)
                    {
                        ReceivedCRC.LOBYTE = 0xB5;
                    }
                    else
                    {
                        ReceivedCRC.LOBYTE = 0xC5;
                    }
                }
            }
            else if (MessageLength == (ReplyLength - 4))
            {
                if (Buffer[ReplyLength - 3] == 0)
                {
                    ReceivedCRC.HIBYTE = 0xB5;
                }
                else
                {
                    ReceivedCRC.HIBYTE = 0xC5;
                }

                if (Buffer[ReplyLength - 1] == 0)
                {
                    ReceivedCRC.LOBYTE = 0xB5;
                }
                else
                {
                    ReceivedCRC.LOBYTE = 0xC5;
                }
            }
            
            if (ReceivedCRC.USHORT != CalculatedCRC.USHORT)
            {
                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorCrc;
                OnJobExecuted(eAJob);
                return false;
            }


            // Check the reply attribute
            switch ((ReplyAttributes)Buffer[AttributeOffs])
            {
                case ReplyAttributes.Response:
                    if (GestReadCommand)
                    {
                        return true;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }

                case ReplyAttributes.ACK_NAK:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }
                    else
                    {
                        ushortUnion Aux = new ushortUnion(Buffer, (ushort)AckNakCodeOffs);
                        if (Aux.USHORT == 0)
                        {
                            return true;
                        }
                        else
                        {
                            eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseNack;
                            OnJobExecuted(eAJob);
                            return false;
                        }
                    }

                // Unexpected attribute
                default:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                    }
                    OnJobExecuted(eAJob);
                    return false;
            }
        }

        protected override void WorkingThread(object data)
        {
        }
       
        #endregion

        #region Properties
        #endregion


        #region static const

        public const uint AttributeOffs = 1;
        public const uint AckNakCodeOffs = 2;
        const ushort MaxBufferSize = 2048;
        const byte MinMessageLength = 5;

        #endregion

        #region methods


        public uint GetAnswerFrameLength(SaiaDataModeCommJob job)
        {
            return MaxBufferSize;
        }

        public uint GetResponseFrameLength(SaiaDataModeCommJob job)
        {
            return MaxBufferSize;
        }
        public uint PrepareRequest(SaiaDataModeCommJob j, ref byte[] buffer)
        {
            if (j == null || (j.Station as SaiaDataModeStation == null))
                return 0;

            lock (j.retLockList())
            {
                SaiaDataModeStation s = j.Station as SaiaDataModeStation;
                uint Count = 0;
                // Frame Synchronization
                buffer[Count++] = 0xB5;
                // Attribute (0 = request)
                buffer[Count++] = 0;
                // Station ID
                AddByteOrDLEsequence(s.StationID, ref buffer, ref Count);
                // Command
                AddByteOrDLEsequence(j.GetCommand(), ref buffer, ref Count);
                byte[] tmpBuffer = new byte[0];
                ushortUnion StartAddress = new ushortUnion();
            
                if (!GestReadCommand)
                {
                    // Base address
                    StartAddress = new ushortUnion(j.GetWriteAddres());
                    object jobData = null;
                    j.GetJobData(ref jobData);
                    // discard inputoutput/exception output unchanged write value operation
                    if (j.TagsListOnWriting.Count == 0 || jobData == null)
                    {
                        return (0);
                    }
                    tmpBuffer = (byte[])jobData;
                }
                // Quantity
                AddByteOrDLEsequence(j.GetQuantity(GestReadCommand, (uint)tmpBuffer.Count()), ref buffer, ref Count);
                // DB number
                if (j.AreaType == AreaTypes.DataBlock)
                {
                    AddByteOrDLEsequence(j.DbNumber.HIBYTE, ref buffer, ref Count);
                    AddByteOrDLEsequence(j.DbNumber.LOBYTE, ref buffer, ref Count);
                }

                if (!GestReadCommand)
                {
                    AddByteOrDLEsequence(StartAddress.HIBYTE, ref buffer, ref Count);
                    AddByteOrDLEsequence(StartAddress.LOBYTE, ref buffer, ref Count);

                    // Quantity of bits
                    if (j.IsBitDataFormat())
                    {
                        AddByteOrDLEsequence(j.GetWriteBitQuantity(), ref buffer, ref Count);
                    }

                    for (uint i = 0; i < tmpBuffer.Count(); i++)
                    {
                        AddByteOrDLEsequence(tmpBuffer[i], ref buffer, ref Count);
                    }
                }
                else
                {
                    // Base address
                    AddByteOrDLEsequence(j.StartAddress.HIBYTE, ref buffer, ref Count);
                    AddByteOrDLEsequence(j.StartAddress.LOBYTE, ref buffer, ref Count);
                }

                // Calculate the CRC
                ushortUnion nCRC = SaiaDataModeProtocol.CalculateCRC(buffer, Count);
                // Append the CRC to the message
                AddByteOrDLEsequence(nCRC.HIBYTE, ref buffer, ref Count);
                AddByteOrDLEsequence(nCRC.LOBYTE, ref buffer, ref Count);

                return Count;
            }

        }
        
        void AddByteOrDLEsequence(byte Value,ref byte[] buffer, ref uint Count)
        {
            if( (Value != 0xB5) && (Value != 0xC5) ) {
                buffer[Count++] = Value;
            }
            else if( Value == 0xB5 ) {
                buffer[Count++] = 0xC5;
                buffer[Count++] = 0;
            }
            else {
                buffer[Count++] = 0xC5;
                buffer[Count++] = 1;
            }
        }
       

        bool ReceiveBufferPull(byte[] inBuffer, ref ushort MessageLength)
        {
            bool RetValue = false;
            if (inBuffer.Length > MessageLength)
            {
                lock (lockList)
                {
                    if (ReceiveBuffer.Count >= 1)
                    {

                        ReceiveBufferBuffer.AddRange(ReceiveBuffer);
                        ReceiveBuffer.Clear();
                    }
                }

                if (ReceiveBufferBuffer.Count >= 1)
                {

                    ReceiveBufferBuffer.CopyTo(0, inBuffer, MessageLength, 1);
                    ReceiveBufferBuffer.RemoveRange(0, 1);
                    RetValue = true;
                    MessageLength++;
                }

            }
            return RetValue;
        }

        public void Flush()
        {
            uint BytesToRead = GetBytesToRead();
            while (BytesToRead != 0)
            {
                byte[] Buffer = new byte[BytesToRead];
                DeviceRead(Buffer, BytesToRead);
                BytesToRead = GetBytesToRead();
            }
            lock (lockList)
            {
                ReceiveBuffer.Clear();
            }
            ReceiveBufferBuffer.Clear();
        }

        bool ReceiveByteOrDLEsequence(byte[] Buffer, ref ushort MessageLength, ref ushort ReceiveChars)
        {
            bool ReadCharacter = false;
            // DLE sequence?
            if (Buffer[MessageLength - 1] != 0xC5)
            {
                ReadCharacter = true;
            }
            // Read a character
            if (ReadCharacter)
            {
                if (!ReceiveBufferPull(Buffer, ref MessageLength))
                {
                    return false;
                }
            }

            // DLE sequence?
            if (Buffer[MessageLength-1] == 0xC5)
            {
                if (!ReceiveBufferPull(Buffer, ref MessageLength))
                {
                    return false;
                }
                if (Buffer[MessageLength-1] == 0)
                {
                    AnswerBuffer[ReceiveChars] = 0xB5;
                    ReceiveChars++;
                }
                else if (Buffer[MessageLength-1] == 1)
                {
                    AnswerBuffer[ReceiveChars] = 0xC5;
                    ReceiveChars++;
                }
            }
            else
            {
                AnswerBuffer[ReceiveChars] = Buffer[MessageLength-1];
                ReceiveChars++;
            }

            return (true);
        }

        #endregion
        
        #region Member
        public SaiaDataModeChannel ParentChannel;

        byte[] AnswerBuffer = new byte[MaxBufferSize];
        byte[] MessageBuffer = new byte[MaxBufferSize];
        ushort MessageLength = 0;
        ushort ReceivedChars = 0;
        protected List<byte> ReceiveBufferBuffer = new List<byte>();
        bool GestReadCommand;

        #endregion
 
        #region IChannelBase Interface

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if (ParentChannel != null)
            {
                e.RxBytes = DiagnLastTaskRxBytes;
                DiagnLastTaskRxBytes = 0;
                e.TxBytes = DiagnLastTaskTxBytes;
                DiagnLastTaskTxBytes = 0;
                ParentChannel.myOnJobExecuted(e);
            }
        }
        
        #endregion
    
    }

}
