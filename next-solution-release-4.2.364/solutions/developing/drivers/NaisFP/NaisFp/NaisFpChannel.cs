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

namespace NaisFp
{
    class NaisFpChannel : Channel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the Channel object.
        /// </summary>
        public NaisFpChannel(CommunicationDriver commdriver, NaisFpChannelSettings settings)
            : base(commdriver, settings, false)
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
        public void myOnJobExecuted(ExecutedJobArgs e)
        {
            DiagnLastTaskRxBytes += e.RxBytes;
            DiagnLastTaskTxBytes += e.TxBytes;
            NaisFpCommJob job = e.Job as NaisFpCommJob;
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                LastErrorMessage = "";
            else
                LastErrorCode = e.ErrorCode;
            if (job.PartialWriteStart == 0)
            {
                if (LastErrorCode != DriverErrorCodes.ErrorNoError)
                    e.ErrorCode = LastErrorCode;
                LastErrorCode = DriverErrorCodes.ErrorNoError;
                base.OnJobExecuted(e);
            }
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
                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            if (inRxBuffer.buffer[NaisFpProtocol.StationId] != tmpTxBuffer.buffer[NaisFpProtocol.StationId] || 
                inRxBuffer.buffer[NaisFpProtocol.StationId + 1] != tmpTxBuffer.buffer[NaisFpProtocol.StationId + 1])
                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorStationUnmismatch;

            if (inRxBuffer.buffer[NaisFpProtocol.EndHeader] == ('!'))
            {
                //
                // Check the error code.
                //
                if (inRxBuffer.compare(NaisFpProtocol.ErrCode, '4'))
                {
                    if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '0'))
                        return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '1'))
                        return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorFormat;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '2'))
                        return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorWrongCommand;
                    else if (inRxBuffer.compare(NaisFpProtocol.ErrCode + 1, '3'))
                        return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorProc;
                }

                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorReturnUnknownCode;
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
                lock (job.retLockList())
                {
                    if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (job.TagsListToWrite.Count == 0)
                        {
                            job.TagsListToWrite.AddRange(job.TagsList);
                        }
                    }
                    if (job.TagsListToWrite.Count == 0)
                    {
                        return false;
                    }
                }
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
                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            if (pdu.buffer[pdu.pointer - 1] != 0x0d)
                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBadRxChars;

            // Check the BCC
            if (!BCCIsOk(ref pdu))
                return (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorBCC;

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

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            maxReplyLength = GetMaxReplyLength();
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
            NaisFpCommJob mJob = job as NaisFpCommJob;
            if (mJob == null)
                return;

            if (job.IsPending == true)
            {
                return;
            }

            if (executedJob == null)
            {
                base.ExecuteJob(job);
                executedJob = mJob;
                GestReadCommand = mJob.IsReadCommand();
                bool check = false;
               
                check = PrepareTelegram(mJob);                    
                
                if(check == false)
                {
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                        //reset active job
                        executedJob = null;
                    }
                }
                else
                {
                    ChildChannel.ExecuteJob(mJob);
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
            ChildChannel.Dispose();

            base.Dispose();
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
            : base(commdriver, settings, false)
        {
        }

        #endregion

        #region Override Methods

        public override void ExecuteJob(CommJob job)
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

            lock (lockThreadObject)
            {
                Flush();
            }
            if (!DeviceWrite(ParentChannel.requestBuffer.buffer, ParentChannel.requestBuffer.pointer))
            {
                return;
            }

            if (!BeginDeviceRead(ParentChannel.maxReplyLength))
            {
                return;
            }

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
                    pendingjob.Station as NaisFpStation == null ||
                    ReceiveBuffer.Count < NaisFpProtocol.ReplyOverhead)
                {
                    Flush();
                    return false;
                }

                NaisFpStation s = pendingjob.Station as NaisFpStation;
                NaisFpCommJob mJob = pendingjob as NaisFpCommJob;
                if (mJob == null)
                {
                    Flush();
                    return false;
                }

                ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };

                if (ReceiveBuffer.Count != ParentChannel.ExpCharsTotal)
                {
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorInvalidLenght;
                    OnJobExecuted(eJob);
                    Flush();
                    ParentChannel.executedJob = null;
                    return true;
                }

                ComBuffer pdu;
                if (ParentChannel.IsFP2())
                {
                    if (ParentChannel.SupervisorID != ReceiveBuffer[NaisFpProtocol.SupervisorIDFP2])
                    {
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)NaisFptErrorCodes.ErrorNotExpectedMessage;
                        OnJobExecuted(eJob);
                        Flush();
                        ParentChannel.executedJob = null;
                        return true;
                    }
                    pdu = new ComBuffer((ushort)(ReceiveBuffer.Count - NaisFpProtocol.OverheadFP2));
                    ReceiveBuffer.CopyTo( NaisFpProtocol.OverheadFP2, pdu.buffer,0, pdu.size);
                }
                else
                {
                    pdu = new ComBuffer((ushort)ReceiveBuffer.Count);
                    ReceiveBuffer.CopyTo(pdu.buffer);
                }

                pdu.pointer = pdu.size;

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
                    Answer = new byte[receivedFrame.Length / 2];
                    Array.Copy(pdu.buffer, (int)NaisFpProtocol.InitReadData, receivedFrame, 0, receivedFrame.Length);
                    string auxString = Encoding.UTF8.GetString(receivedFrame);
                    for (int i = 0; i < Answer.Length; i++)
                    {
                        Answer[i] = Convert.ToByte(auxString.Substring(2 * i, 2), 16);
                    }
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

        #endregion

        #region static const

        #endregion

        #region Methods

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
        }


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
                ParentChannel.myOnJobExecuted(e);
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
        }

        #endregion

        #region Override Methods
        public override void ExecuteJob(CommJob job)
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

            Flush();
            if (!DeviceWrite(ParentChannel.requestBuffer.buffer, ParentChannel.requestBuffer.pointer))
            {
                return;
            }

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

            if (pendingjob == null ||
                pendingjob.Station as NaisFpStation == null )
            {
                ParentChannel.executedJob = null;
                return true;
            }


            NaisFpStation s = pendingjob.Station as NaisFpStation;
            NaisFpCommJob mJob = pendingjob as NaisFpCommJob;
            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };

            // Read the initial character
            if (pdu.pointer == 0)
            {
                if (!ReceiveBufferPull())
                {
                    return false;
                }
                if (!pdu.compare(0,'%') &&
                    !pdu.compare(0,'<')  )
                {
                    pdu.pointer = 0;
                    return false;
                }
            }

            while (ReceiveBufferPull() && pdu.pointer < ParentChannel.ExpCharsTotal) ;

            if (pdu.pointer < ParentChannel.ExpCharsTotal)
                return false;


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
                for (int i = 0; i < Answer.Length; i++)
                {
                    Answer[i] = Convert.ToByte(auxString.Substring(2 * i, 2), 16);
                }
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

        #endregion

        #region Properties
        #endregion


        #region static const


        #endregion

        #region methods

        public void Flush()
        {
            uint WaitingBytes = GetBytesToWrite();
            while (WaitingBytes != 0)
            {
                Thread.Sleep(1);
                WaitingBytes = GetBytesToWrite();
            }
            WaitingBytes = GetBytesToRead();
            while (WaitingBytes != 0)
            {
                byte[] Buffer = new byte[WaitingBytes];
                DeviceRead(Buffer, WaitingBytes);
                Thread.Sleep(1);
                WaitingBytes = GetBytesToRead();
            }
            lock (lockList)
            {
                ReceiveBuffer.Clear();
            }
            ReceiveBufferBuffer.Clear();
            pdu.pointer = 0;
        }

        bool ReceiveBufferPull()
        {
            bool RetValue = false;
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
                if(pdu.insert(ReceiveBufferBuffer[0]))
                {
                    ReceiveBufferBuffer.RemoveRange(0, 1);
                    RetValue = true;
                }
            }
            return RetValue;
        }
 
        #endregion
        
        #region Member
        ComBuffer pdu = new ComBuffer(NaisFpProtocol.MAX_FRAME_LENGTH_LONG_MSG);
        public NaisFpChannel ParentChannel;
        protected List<byte> ReceiveBufferBuffer = new List<byte>();

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
