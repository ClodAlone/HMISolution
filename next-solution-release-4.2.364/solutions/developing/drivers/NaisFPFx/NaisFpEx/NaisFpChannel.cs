using System;
using System.Collections.Generic;
using System.Text;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using System.Threading;
using DriverCodeBaseEx.Enumerators;

namespace NaisFp
{
    class NaisFpChannel : Channel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the Channel object.
        /// </summary>
        public NaisFpChannel(CommunicationDriver commdriver, NaisFpChannelSettings settings)
            : base(commdriver, settings)
        {
            _ChannelType = settings.ChannelType;
            switch (_ChannelType)
            {
                case ChannelTypes.Serial:
                    NaisFpSerialChannelSettings SerialSetting = new NaisFpSerialChannelSettings();
                    SerialSetting.CopyProperties(settings);
                    ChildChannel = new NaisFpSerialChannel(commdriver, SerialSetting);
                    ((NaisFpSerialChannel)ChildChannel).ParentChannel = this;
                    USBconnection = settings.USBconnection;
                break;
                case ChannelTypes.Socket:
                    NaisFpTcpChannelSettings TcpSetting = new NaisFpTcpChannelSettings();
                    TcpSetting.CopyProperties(settings);
                    ChildChannel = new NaisFpTcpChannel(commdriver, TcpSetting);
                    ((NaisFpTcpChannel)ChildChannel).ParentChannel = this;
                    FP2ETLAN = settings.FP2ETLAN;
                    SupervisorID = settings.SupervisorID;
                    break;
            }
           
        }

        #endregion

        #region Methods
        public void OnJobExecuted(ExecutedJobArgs e)
        {
            DiagnLastTaskRxBytes += e.RxBytes;
            DiagnLastTaskTxBytes += e.TxBytes;

            base.OnJobExecuted(e);
        }

        bool GetUsbConnection()
        {
            if (_ChannelType == ChannelTypes.Socket)
            {
                return false ;
            }
            else 
            {
                return USBconnection;
            }
        }

        public bool IsFP2()
        {
            if (_ChannelType == ChannelTypes.Serial)
            {
                return false;
            }
            else
            {
                return FP2ETLAN;
            }
        }

        public bool InsertHeader(NaisFpCommJob job,ref ComBuffer inTxBuffer)
        {
            NaisFpStation s = (NaisFpStation)job.Station;
            if (s == null)
                return false ;

            if (s.FrameFormat == FrameFormats.Normal)
            {
                inTxBuffer.insert('%');
            }
            else
            {
                inTxBuffer.insert('<');
            }
            if (GetUsbConnection())
            {
                inTxBuffer.insert('E');
                inTxBuffer.insert('E');
            }
            else
            {
                inTxBuffer.insert(String.Format("{0:X2}", s.StationID));
            }
            inTxBuffer.insert('#');

            return true;
         
        }

        byte CalculateBCC(byte[] inTxBuffer, ushort pointer)
        {
            ushort i;
            byte BccValue = 0;
            for (i = 0; i < pointer; i++) 
            {
                BccValue ^= inTxBuffer[i];
            }
            return BccValue;
        }

        void insertBcc(ref ComBuffer inTxBuffer)
        {
            inTxBuffer.insert(String.Format("{0:X2}", CalculateBCC(inTxBuffer.buffer, inTxBuffer.pointer)));
        }

        public bool BCCIsOk(ref ComBuffer inTxBuffer)
        {
            inTxBuffer.pointer -= 3; 
            byte[] BccBuffer = new byte[2];
            BccBuffer[0] = inTxBuffer.buffer[inTxBuffer.pointer];
            BccBuffer[1] = inTxBuffer.buffer[inTxBuffer.pointer + 1];
            insertBcc(ref inTxBuffer);
            return BccBuffer[0] == inTxBuffer.buffer[inTxBuffer.pointer - 2] 
                && BccBuffer[1] == inTxBuffer.buffer[inTxBuffer.pointer - 1];

        }

        public DriverErrorCodes CheckHeader(ref ComBuffer inRxBuffer, NaisFpCommJob pendingjob)
        {
            ComBuffer tmpTxBuffer = new ComBuffer(NaisFpProtocol.ReplyOverhead);
            InsertHeader(pendingjob, ref tmpTxBuffer);
            
            if(inRxBuffer.buffer[NaisFpProtocol.StartHeader] != tmpTxBuffer.buffer[NaisFpProtocol.StartHeader] ||
                (inRxBuffer.buffer[NaisFpProtocol.EndHeader] != ('$') &&
                inRxBuffer.buffer[NaisFpProtocol.EndHeader] != ('!')))
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            if (inRxBuffer.buffer[NaisFpProtocol.StationId] != tmpTxBuffer.buffer[NaisFpProtocol.StationId] || 
                inRxBuffer.buffer[NaisFpProtocol.StationId + 1] != tmpTxBuffer.buffer[NaisFpProtocol.StationId + 1])
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorStationUnmismatch;

            if (inRxBuffer.buffer[NaisFpProtocol.EndHeader] == ('!'))
            {
                //
                // Check the error code.
                //
                if (inRxBuffer.compare(NaisFpProtocol.ErrCode, '4'))
                {
                    if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '0'))
                        return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '1'))
                        return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorFormat;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '2'))
                        return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorWrongCommand;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '3'))
                        return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorProc;
                }

                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorReturnUnknownCode;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public bool PrepareTelegram(NaisFpCommJob job)
        {
            ComBuffer tmpTxBuffer = new ComBuffer(NaisFpProtocol.MAX_FRAME_LENGTH_LONG_MSG);
            DataFormats DataFormat = job.AddressObj.DataFormat;
            ExpCharsTotal = (ushort)(NaisFpProtocol.ReplyOverhead);

            InsertHeader(job,ref tmpTxBuffer);

            if (GestReadCommand)
            {
                ushort NumOfWord ;
                if (DataFormat == DataFormats.BOOL)
                {
                    if(job.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && job.ElementNumber == 0)
                        NumOfWord = (ushort)((job.TotalJobSize + 1) / 2);
                    else
                        NumOfWord = (ushort)((job.AddressObj.BitNumber + job.TagsList[0].BitOffset + job.TotalJobSize + 0x0f) / 16);
                    DataFormat = DataFormats.WORD;
                }
                else
                {
                    NumOfWord =(ushort) ((job.TotalJobSize + 1) / 2 );
                }

                ExpCharsTotal += (ushort)(4 * NumOfWord);

                tmpTxBuffer.insert('R');
                tmpTxBuffer.insert(job.GetCommandCode());
                tmpTxBuffer.insert(job.AddressObj.GetNaisAddress(DataFormat, (int)(job.TagsList[0].ByteOffset / 2)));
                tmpTxBuffer.insert(job.AddressObj.GetNaisAddress(DataFormat, (int)(job.TagsList[0].ByteOffset / 2 + NumOfWord - 1)));
            }
            else
            {              
                tmpTxBuffer.insert('W');
                tmpTxBuffer.insert(job.GetCommandCode());
                // when prepare/get data to write check case of exception output/input output with unchanged data
                bool WriteData = job.GetWriteData(ref tmpTxBuffer);
                if (!WriteData)
                    return false;
            }
            
            insertBcc(ref tmpTxBuffer);
            tmpTxBuffer.insert(0x0d);

            requestBuffer.pointer = 0;
            if (IsFP2())
            {
                NaisFpStation s = (NaisFpStation)job.Station;
                ushortUnion DataSize = new ushortUnion(tmpTxBuffer.pointer);
                requestBuffer.insert(0x10);
                requestBuffer.insert(0x00);
                requestBuffer.insert(DataSize.LOBYTE);
                requestBuffer.insert(DataSize.HIBYTE);
                requestBuffer.insert(0x00);
                requestBuffer.insert(0x00);
                requestBuffer.insert(0x00);
                requestBuffer.insert(0x00);
                requestBuffer.insert(0x00);
                requestBuffer.insert(0x00);
                requestBuffer.insert(s.StationID);
                requestBuffer.insert(SupervisorID);

                ExpCharsTotal += NaisFpProtocol.OverheadFP2;
            }
            requestBuffer.insert(tmpTxBuffer);
            return true;
        }

        public DriverErrorCodes CheckAnswer(ref ComBuffer pdu, NaisFpCommJob pendingjob, NaisFpStation s)
        {
            // Check the Header
            DriverErrorCodes ErrorCode = CheckHeader(ref pdu, pendingjob);
            if (ErrorCode != DriverErrorCodes.ErrorNoError)
                return ErrorCode;

            if (pdu.pointer == 0)
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            if (pdu.buffer[pdu.pointer - 1] != 0x0d)
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            // Check the BCC
            if (!BCCIsOk(ref pdu))
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBCC;

            return DriverErrorCodes.ErrorNoError;
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

        private byte _SupervisorID;
        public byte SupervisorID
        {
            get
            {
                return _SupervisorID;
            }
            set
            {
                _SupervisorID = value;
            }
        }

        private bool _FP2ETLAN;
        public bool FP2ETLAN
        {
            get
            {
                return _FP2ETLAN;
            }
            set
            {
                _FP2ETLAN = value;
            }
        }

        private bool _USBconnection;
        public bool USBconnection
        {
            get
            {
                return _USBconnection;
            }
            set
            {
                _USBconnection = value;
            }
        }
        
        #endregion

        #region Override Methods

        private int GetMaxReplyLength()
        {
            int returnValue = NaisFpProtocol.ReplyOverhead + (NaisFpProtocol.MAX_DATA_BYTES * 2);
            bool longFrameFormat = false;

            foreach (var station in CommDriver.GetChannelStations(this))
            {
                NaisFpStation naisStation = (NaisFpStation)station;
                if(naisStation.FrameFormat == FrameFormats.Long)
                {
                    longFrameFormat = true;
                    break;
                }
            }

            if(longFrameFormat)
            {
                returnValue = NaisFpProtocol.ReplyOverhead + (NaisFpProtocol.MAX_DATA_BYTES_LONG_MSG * 2);
            }

            return (returnValue);
        }

        //protected override void WorkingThread(object data)
        //{
        //    int sleepCycle = WaitTime;
        //    if (sleepCycle == 0)
        //        sleepCycle = 1;
        //    ListJobPending.Clear();
        //    ListJobExecuted.Clear();

        //    maxReplyLength = GetMaxReplyLength();
        //    bool bForceProcessListJobs = false;
        //    NextScheduleTimeJobsList = DateTime.UtcNow;
        //    int loop = 0;
        //    while (true)
        //    {
        //        CommJob nextjob = null;
        //        if (ListJobPending.Count == 0 || MultiPointProtocol)
        //        {
        //            ScheduleListJob();
        //            if (SynchroJob != null)
        //                nextjob = SynchroJob;
        //            else
        //                nextjob = GetNextPendingJob();
        //            if (nextjob != null)
        //                ListJobPending.Add(nextjob);
        //        }

        //        if (nextjob != null)
        //        {
        //            if (!IsDeviceOpen())
        //                DeviceOpen();

        //            ExecuteJob(nextjob);
        //        }

        //        lock (lockThreadObject)
        //        {
        //            if (NewDataToAnlyze.WaitOne(0) || ReceiveBuffer.Count > 0)
        //            {
        //                NewDataToAnlyze.Reset();
        //                if (ListJobPending.Count > 0)
        //                {
        //                    foreach (var job in ListJobPending)
        //                    {
        //                        if (ProcessNewData(job))
        //                            ListJobExecuted.Add(job);
        //                    }

        //                    foreach (var job in ListJobExecuted)
        //                    {
        //                        job.LastExecutionTime = DateTime.UtcNow;
        //                        if (SynchroJob != null && SynchroJob == job)
        //                        {
        //                            job.ResetSynchro.WaitOne(Timeout);
        //                            SynchroJob = null;
        //                            job.ResetSynchro.Reset();
        //                        }
        //                        ListJobPending.Remove(job);

        //                    }
        //                    ListJobExecuted.Clear();

        //                }
        //            }

        //            if (ListJobPending.Count > 0)
        //            {
        //                if (!MultiPointProtocol)
        //                {
        //                    double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
        //                    if (dtime > Timeout)
        //                    {
        //                        if (SynchroJob != null && SynchroJob == ListJobPending[0])
        //                        {
        //                            ListJobPending[0].ResetSynchro.WaitOne(Timeout);
        //                            SynchroJob = null;
        //                            ListJobPending[0].ResetSynchro.Reset();
        //                        }

        //                        //error
        //                        LastErrorCode = DriverErrorCodes.ErrorTimeOut;
        //                        ProcessNewData(ListJobPending[0]);
        //                        ListJobPending.RemoveAt(0);
        //                        ReceiveBuffer.Clear();
        //                    }
        //                }
        //            }
        //        }

        //        if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
        //            DeviceClose();

        //        if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
        //            break;
        //        else if (++loop > 4)
        //        {
        //            loop = 0;
        //            if (StopWorkerThread.WaitOne(sleepCycle))
        //                break;
        //        }
        //        StopWorkerThread.WaitOne(0);
        //    }
        //}

        //public override bool TestChannelComm()
        //{
        //    return ChildChannel.TestChannelComm();
        //}

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;
            
            //if (mJob == null)
            //    return;

            //if (job.IsPending == true)
            //{
            //    return;
            //}

            if (executedJob == null)
            {
                NaisFpCommJob mJob = job as NaisFpCommJob;
                base.ExecuteJob(job);
                executedJob = mJob;
                //GestReadCommand = mJob.IsReadCommand();
                GestReadCommand = mJob.ReadRequest();

                bool check = PrepareTelegram(mJob);
                if (check == false)
                {
                    RemovePendingJob(job);
                    //reset active job
                    executedJob = null;
                    return false;
                }
                else
                {
                    return ChildChannel.ExecuteJob(ref conn, mJob);
                }
            }            

            // to verify !!!!!
            return true;
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                OnJobExecuted(eAJob);
                executedJob = null;
                return true;
            }

            return ChildChannel.ProcessNewData(conn, pendingjob);
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
            if (!bChannelStarted)
            {
                maxReplyLength = GetMaxReplyLength();
                ChildChannel.Startup();
            }

            return base.Startup();
        }

        public override void Suspend()
        {
            base.Suspend();                        
        }

        public override void ResetNewDataEvent()
        {
            ChildChannel.ResetNewDataEvent();
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            ChildChannel.WaitNewDataEvent(ref conn);
        }

        public override void SetNewDataEvent()
        {
            ChildChannel.SetNewDataEvent();
        }

        #endregion

        #region Member
        public ComBuffer requestBuffer = new ComBuffer(NaisFpProtocol.MAX_FRAME_LENGTH_LONG_MSG);
        public Channel ChildChannel;
        public NaisFpCommJob executedJob = null;
        public bool GestReadCommand;
        public ushort ExpCharsTotal;
        public int maxReplyLength = 0;
        #endregion

        #region IDisposable

        public override void Dispose()
        {
            // first dispose base class to stop scheduler
            base.Dispose();

            // then dispose child to close device
            ChildChannel.Dispose();
        }
        #endregion

    }
 
    class NaisFpTcpChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public NaisFpTcpChannel(CommunicationDriver commdriver, TcpChannelSettings settings)
            : base(commdriver, settings)
        {
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        #endregion

        #region Override Methods

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            NaisFpCommJob mJob = job as NaisFpCommJob;

            //if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            //{
            //    lock (job.retLockList())
            //    {
            //        if (job.TagsListToWrite.Count == 0)
            //            job.TagsListToWrite.AddRange(job.TagsList);
            //    }
            //}
            
            if (!DeviceWrite(ParentChannel.requestBuffer.buffer, ParentChannel.requestBuffer.pointer))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            if (!BeginDeviceRead(ParentChannel.maxReplyLength))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }
            return true;
        }


        private void WaitCompleteMessage(ref List<Byte> receiveBuffer)
        {
            lock (lockThreadObject)
            {
                if (ReceiveBuffer.Count > 0)
                {
                    receiveBuffer.AddRange(ReceiveBuffer);
                    ReceiveBuffer.Clear();
                }
            }
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            NaisFpStation s = pendingjob.Station as NaisFpStation;
            NaisFpCommJob mJob = pendingjob as NaisFpCommJob;

            List<Byte> receiveBuffer = new List<byte>();
            // spool data from ethernet buffer
            WaitCompleteMessage(ref receiveBuffer);

            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };
            if (receiveBuffer.Count < NaisFpProtocol.ReplyOverhead || receiveBuffer.Count != ParentChannel.ExpCharsTotal)
            {
                eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eJob);
                ParentChannel.executedJob = null;
                return true;
            }

            ComBuffer pdu;
            if (ParentChannel.IsFP2())
            {
                if (ParentChannel.SupervisorID != receiveBuffer[NaisFpProtocol.SupervisorIDFP2])
                {
                    eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorNotExpectedMessage;
                    OnJobExecuted(eJob);                    
                    ParentChannel.executedJob = null;
                    return true;
                }
                pdu = new ComBuffer((ushort)(receiveBuffer.Count - NaisFpProtocol.OverheadFP2));
                receiveBuffer.CopyTo( NaisFpProtocol.OverheadFP2, pdu.buffer,0, pdu.size);
            }
            else
            {
                pdu = new ComBuffer((ushort)receiveBuffer.Count);
                receiveBuffer.CopyTo(pdu.buffer);
            }

            pdu.pointer = pdu.size;

            // Check the Answer
            DriverErrorCodes ErrorCode = ParentChannel.CheckAnswer(ref pdu, mJob, s);
            if (ErrorCode != DriverErrorCodes.ErrorNoError)
            {
                eJob.ErrorCode = ErrorCode;
                OnJobExecuted(eJob);
                //Flush();
                ParentChannel.executedJob = null;
                return true;
            }

            byte[] Answer;
            if (ParentChannel.GestReadCommand)
            {
                // Copy received data
                byte[] receivedFrame = new byte[ParentChannel.ExpCharsTotal - NaisFpProtocol.ReplyOverhead];
                Answer = new byte[receivedFrame.Length / 2];
                Array.Copy(pdu.buffer, (int)NaisFpProtocol.InitReadData, receivedFrame, 0, receivedFrame.Length);
                string auxString = Encoding.UTF8.GetString(receivedFrame);
                try
                {
                    for (int i = 0; i < Answer.Length; i++)
                        Answer[i] = Convert.ToByte(auxString.Substring(2 * i, 2), 16);                    
                }
                catch (Exception ex)
                {
                    Answer = null;
                    eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;
                }
            }
            else
            {
                Answer = new byte[0];
            }
            eJob.Values = Answer;

            //Flush();
            OnJobExecuted(eJob);

            ParentChannel.executedJob = null;
            return true;            
        }

        #endregion

        #region static const

        #endregion

        #region Methods

        //private void Flush()
        //{
        //    uint BytesToRead = GetBytesToRead();
        //    while (BytesToRead != 0)
        //    {
        //        byte[] Buffer = new byte[BytesToRead];
        //        DeviceRead(Buffer, BytesToRead);
        //        BytesToRead = GetBytesToRead();
        //    }
        //    lock (lockThreadObject)
        //    {
        //        ReceiveBuffer.Clear();
        //    }
        //}


        #endregion

        #region Properties

        #endregion

        #region Member
        public NaisFpChannel ParentChannel;

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
                ParentChannel.OnJobExecuted(e);
            }
        }
        
        #endregion

    }

    class NaisFpSerialChannel : SerialChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public NaisFpSerialChannel(CommunicationDriver commdriver, SerialChannelSettings settings)
            : base(commdriver, settings)
        {
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        #endregion

        #region Override Methods
        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            //if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            //{
            //    lock (job.retLockList())
            //    {
            //        if (job.TagsListToWrite.Count == 0)
            //            job.TagsListToWrite.AddRange(job.TagsList);
            //    }
            //}

            if (!DeviceWrite(ParentChannel.requestBuffer.buffer, ParentChannel.requestBuffer.pointer))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            return true;
        }

        private void WaitCompleteMessage(ref ComBuffer pdu)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;

            // Read the initial character
            if (pdu.pointer == 0)
            {
                if (!ReceiveBufferPull(ref pdu))
                {
                    // error
                    return;
                }
                if (!pdu.compare(0, '%') &&
                    !pdu.compare(0, '<'))
                {
                    // error
                    pdu.pointer = 0;                    
                }
            }

            while (conn == (int)DriverErrorCodes.ErrorNoError && pdu.pointer < ParentChannel.ExpCharsTotal)
            {
                //while (ReceiveBufferPull() && pdu.pointer < ParentChannel.ExpCharsTotal);
                WaitNewDataEvent(ref conn);

                ReceiveBufferPull(ref pdu);
            }
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            ComBuffer pdu = new ComBuffer(NaisFpProtocol.MAX_FRAME_LENGTH_LONG_MSG);

            NaisFpStation s = pendingjob.Station as NaisFpStation;
            NaisFpCommJob mJob = pendingjob as NaisFpCommJob;
            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };

            // spool data from serial buffer
            WaitCompleteMessage(ref pdu);

            // Check the Answer
            DriverErrorCodes ErrorCode = ParentChannel.CheckAnswer(ref pdu, mJob, s);
            if (ErrorCode != DriverErrorCodes.ErrorNoError)
            {
                eJob.ErrorCode = ErrorCode;
                OnJobExecuted(eJob);
                Flush();
                ParentChannel.executedJob = null;
                return true;
            }

            byte[] Answer;
            if (ParentChannel.GestReadCommand)
            {
                // Copy received data
                byte[] receivedFrame = new byte[ParentChannel.ExpCharsTotal - NaisFpProtocol.ReplyOverhead];
                Answer = new byte[receivedFrame.Length/2];
                Array.Copy(pdu.buffer, (int)NaisFpProtocol.InitReadData, receivedFrame, 0, receivedFrame.Length);
                string auxString = Encoding.UTF8.GetString(receivedFrame);
                try
                {
                    for (int i = 0; i < Answer.Length; i++)
                        Answer[i] = Convert.ToByte(auxString.Substring(2 * i, 2), 16);
                
                }
                catch (Exception ex)
                {
                    Answer = null;
                    eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;
                }
            }
            else
            {
                Answer = new byte[0];
            }
            eJob.Values = Answer;

            OnJobExecuted(eJob);

            ParentChannel.executedJob = null;
            return true;
        }

        #endregion

        #region Properties
        #endregion


        #region static const


        #endregion

        #region methods

        private void Flush()
        {
            // wait until all data in write buffer was sent
            uint WaitingBytes = GetBytesToWrite();
            while (WaitingBytes != 0)
                WaitingBytes = GetBytesToWrite();

            // read buffer is automatically spooled by Serial Class Base

            // ReceiveBuffer is filled async so, for security reason, if an error occour in the previous job's execution, clean up
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }        
        }

        bool ReceiveBufferPull(ref ComBuffer p)
        {            
            bool RetValue = false;
            
            lock (lockThreadObject)
            {
                List<byte> data = new List<byte>();
                if (ReceiveBuffer.Count > 0)
                {
                    data.AddRange(ReceiveBuffer);
                    ReceiveBuffer.Clear();
                }
                if (data.Count>0)
                {
                    foreach (var b in data)
                        p.insert(b);
                    RetValue = true;
                }
            }
            return RetValue;
        }
 
        #endregion
        
        #region Member        
        public NaisFpChannel ParentChannel;        
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
                ParentChannel.OnJobExecuted(e);
            }
        }
        
        #endregion
    
    }

}
