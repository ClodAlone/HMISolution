using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace SaiaDataMode
{
    
    class SaiaDataModeChannel : Channel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the Channel object.
        /// </summary>
        public SaiaDataModeChannel(CommunicationDriver commdriver, SaiaDataModeChannelSettings settings)
            : base(commdriver, settings)
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
            LastErrorCode = e.ErrorCode;
            //if(e.ErrorCode == DriverErrorCodes.ErrorNoError)
            //    LastErrorMessage = "";
            base.OnJobExecuted(e);
        }

        public DriverErrorCodes GetLastErrorCode()
        {
            return LastErrorCode;
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
        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            if (executedJob == null)
            {                
                ChildChannel.ExecuteJob(ref conn, job);
                // discard here input output/exception output unchanged write value
                if (((SaiaDataModeCommJob)job).RemoveFromPendingJob)
                {
                    ((SaiaDataModeCommJob)job).RemoveFromPendingJob = false;
                    RemovePendingJob(job);                    
                    return false;
                }
            }

            return true;
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                myOnJobExecuted(eAJob);
                executedJob = null;
                return false;
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
        public Channel ChildChannel;
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
            base.Dispose();
            ChildChannel.DeviceClose();
            ChildChannel.Dispose();
            
        }
        #endregion

    }
 
    class SaiaDataModeUdpChannel : Channel // UdpChannel
    {
        #region Properties
        /// <summary>
        /// Flag the test connection
        /// </summary>
        public bool TestConnection = false;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SaiaDataModeUdpChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings)
        {
            _UDPManager = new UDPManagerSaiaDataMode(this, settings);
        }

        #endregion

        #region Override Methods
        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            SaiaDataModeCommJob mJob = job as SaiaDataModeCommJob;

            //if (job.IsPending == true)
            //{
            //    return;
            //}

            base.ExecuteJob(job);

            GestReadCommand = job.ReadRequest();

            uint count = GetAnswerFrameLength(mJob);
            byte[] pdu = new byte[count];

            if(!TestConnection)
            {
                count = PrepareRequest(mJob, ref pdu);
            }
            else
            {
                count = PrepareRequestTestInformation(mJob, ref pdu);
            }
            if (count == 0)
            {
                // child channel don't contain jobs inside PendingList --> will be removed from main channel
                ((SaiaDataModeCommJob)job).RemoveFromPendingJob = true;
                return false;
            }

            //Flush();
            if (!DeviceWrite(pdu, count))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            _UDPManager.BeginDeviceRead(SaiaDataModeProtocol.ReplyOverhead);

            //job.LastExecutionTime = DateTime.UtcNow;
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
            SaiaDataModeStation s = pendingjob.Station as SaiaDataModeStation;
            SaiaDataModeCommJob mJob = pendingjob as SaiaDataModeCommJob;
            List<Byte> receiveBuffer = new List<byte>();
            WaitCompleteMessage(ref receiveBuffer);
            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };

            if (receiveBuffer.Count == 0)
            {
                System.Diagnostics.Trace.TraceInformation("ErrorInvalidLenght");
                eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eJob);
                ParentChannel.executedJob = null;
                return true;
            }
            uintUnion ReplyLength = new uintUnion(receiveBuffer, 0);

            if ((ReplyLength.UINT < SaiaDataModeProtocol.ReplyOverhead) || (ReplyLength.UINT > SaiaDataModeProtocol.MaxMessageLength))
            {
                eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eJob);
                //Flush();
                ParentChannel.executedJob = null;
                return true;
            }

            byte[] pdu = new byte[ReplyLength.UINT];
            receiveBuffer.CopyTo(0, pdu, 0, (int)ReplyLength.UINT);

            // Check the reply
            if (!CheckReply(conn, pdu, mJob, s))
            {
                //Flush();
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

            //Flush();
            OnJobExecuted(eJob);

            ParentChannel.executedJob = null;

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            bool returnValue = _UDPManager.IsDeviceOpen();
            SetStateCommandVariableBit(((!returnValue) || ParentChannel.InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            return _UDPManager.DeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return _UDPManager.DeviceClose(bDisposed || CommDriver.bDisposed);
        }

        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            return _UDPManager.DeviceRead(Buffer, Count);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            return _UDPManager.DeviceWrite(Buffer, Count);
        }        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return _UDPManager.GetBytesToRead();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            return (0);
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

        private void Flush()
        {
            if (GetBytesToRead() != 0)
            {
                byte[] Buffer = new byte[0];
                DeviceRead(Buffer, 0);
            }
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }
        }

        public uint PrepareRequest(SaiaDataModeCommJob j, ref byte[] buffer)
        {
            if (j == null || (j.Station as SaiaDataModeStation == null))
                return 0;

            //lock (j.retLockList())
            //{
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
                    // Data
                    object jobData = null;
                    j.GetJobData(ref jobData);
                    // discard inputoutput/exception output unchanged write value operation                    
                    if (j.TagsListOnWriting.Count == 0 || jobData == null)
                    {
                        return (0);
                    }

                    // Base address
                    ushortUnion StartAddress = new ushortUnion(j.GetWriteAddres());
                    buffer[Count.UINT++] = StartAddress.HIBYTE;
                    buffer[Count.UINT++] = StartAddress.LOBYTE;

                    // Quantity of bits
                    if (j.IsBitDataFormat())
                    {
                        buffer[Count.UINT++] = j.GetWriteBitQuantity();
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
            //}
        }

        public uint PrepareRequestTestInformation(SaiaDataModeCommJob j, ref byte[] buffer)
        {
            if (j == null || (j.Station as SaiaDataModeStation == null))
                return 0;

            //lock (j.retLockList())
            //{
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
            buffer[Count.UINT++] = 0xab;//j.GetCommand()
            //System information Number
            buffer[Count.UINT++] = 0x00;
            buffer[Count.UINT++] = 0x06;

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
            //}
        }

        bool CheckReply(DriverErrorCodes conn, byte[] pdu, SaiaDataModeCommJob pendingjob, SaiaDataModeStation s)
        {
            ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
            eAJob.Job = pendingjob;
            // Check the reply length
            if (pdu.Count() < MinMessageLength)
            {
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
                OnJobExecuted(eAJob);
                return false;
            }

            // Check the CRC
            if (!SaiaDataModeProtocol.CheckCRC(pdu))
            {
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorCrc;
                OnJobExecuted(eAJob);
                return false;
            }
            
            // Check the sequence number
            if (!s.CheckRequestSequence((new ushortUnion(pdu, (ushort)SaiaDataModeProtocol.SequenceOffs).USHORT)))
            {
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWrongSequence;
                OnJobExecuted(eAJob);
                return false;
            }
            if(TestConnection)
            {
                string plc = System.Text.Encoding.UTF8.GetString(pdu, 26, 4);
                string cpuType = System.Text.Encoding.UTF8.GetString(pdu, 31, 5);
                string firmware = System.Text.Encoding.UTF8.GetString(pdu, 37, 7);
                SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCFamily, plc));
                SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCSerial, cpuType));
                SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCFirmware, firmware));
                return true;

            }

            // Check data length
            if (pendingjob.GetExpectedDataLength(GestReadCommand) != pdu.Count() - SaiaDataModeProtocol.ReplyOverhead)
            {
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseLength;
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
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }

                case ReplyAttributes.ACK_NAK:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
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
                            eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseNack;
                            OnJobExecuted(eAJob);
                            return false;
                        }
                    }

                // Unexpected attribute
                default:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
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
        UDPManagerSaiaDataMode _UDPManager;
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
        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            SaiaDataModeCommJob mJob = job as SaiaDataModeCommJob;

            base.ExecuteJob(job);

            GestReadCommand = job.ReadRequest();

            uint count = GetAnswerFrameLength(mJob);
            byte[] pdu = new byte[count];

            if (!TestConnection)
            {
                count = PrepareRequest(mJob, ref pdu);
            }
            else
            {
                count = PrepareRequestTestInformation(mJob, ref pdu);
            }
            //count = PrepareRequest(mJob, ref pdu);

            if (count == 0)
            {
                // child channel don't contain jobs inside PendingList --> will be removed from main channel
                ((SaiaDataModeCommJob)job).RemoveFromPendingJob = true;
                return false;
            }

            // use last communiction error code to decide if Flush operation is required 
            if (ParentChannel.GetLastErrorCode() != DriverErrorCodes.ErrorNoError)
                Flush();            

            if (!DeviceWrite(pdu, count))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            // reset internal buffer/pointer that manage buffer parsing after message request was sent to device
            ResetCommunicationVars();
            
            return true;
        }


        private void WaitCompleteMessage(SaiaDataModeCommJob job)
        {
            lock (lockThreadObject)
            {
                if (ReceiveBuffer.Count > 0)
                {
                    ReceiveBufferBuffer.AddRange(ReceiveBuffer);
                    ReceiveBuffer.Clear();
                }
            }

            // Read the initial character
            if (MessageLength < 2)
            {
                if (MessageLength == 0)
                {
                    if (!ReceiveBufferPull(MessageBuffer, ref MessageLength))
                    {
                        return;
                    }
                    // 1st char is start of message ?
                    if (MessageBuffer[0] != 0xB5)
                    {
                        MessageLength = 0;
                        return;
                    }
                }

                //// Read the telegram attribute
                if (!ReceiveBufferPull(MessageBuffer, ref MessageLength))
                {
                    return;
                }
                // ar is corret ?
                if ((MessageBuffer[1] != 1) && (MessageBuffer[1] != 2))
                {
                    MessageLength = 0;
                    return;
                }
                ReceivedChars = 0;
            }

            // Read response with data
            if (MessageBuffer[1] == 1)
            {
                if(TestConnection)
                {

                    if (!ReceiveBufferPull(MessageBuffer, ref MessageLength))
                    {
                        return;
                    }
                    if (MessageBuffer[2]== 0)
                    {
                        return;
                    }
                    ushort index = (ushort)(MessageBuffer[2] + MessageLength);
                    ReceivedChars = (ushort)(MessageLength);
                    while (ReceivedChars < index)
                    {
                        if (!ReceiveBufferPull(MessageBuffer, ref ReceivedChars))
                        {
                            return;
                        }
                    }
                    string plc = System.Text.Encoding.UTF8.GetString(MessageBuffer, 19, 4);
                    string cpuType = System.Text.Encoding.UTF8.GetString(MessageBuffer, 24, 5);
                    string firmware = System.Text.Encoding.UTF8.GetString(MessageBuffer, 30, 7);
                    SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCFamily, plc));
                    SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCSerial, cpuType));
                    SaiaDataModeDriver.Addinfo(string.Format(Properties.Resources.PLCFirmware, firmware));
                    return;
                }

                if (job.GetExpectedDataLength(GestReadCommand) == 0)
                {
                    while (ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars)) ;
                }
                else
                {
                    while (ReceivedChars < job.GetExpectedDataLength(GestReadCommand) + 2)
                    {
                        if (!ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars))
                        {
                            return;
                        }
                    }
                }
            }
            else
            { 
                // Read ACK/NAK reply
                while (ReceivedChars < 4)
                {
                    if (!ReceiveByteOrDLEsequence(MessageBuffer, ref MessageLength, ref ReceivedChars))
                    {
                        return;
                    }
                }
            }
        }


        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {            
            SaiaDataModeStation s = pendingjob.Station as SaiaDataModeStation;
            SaiaDataModeCommJob mJob = pendingjob as SaiaDataModeCommJob;

            WaitCompleteMessage(mJob);

            ExecutedJobArgs eJob = new ExecutedJobArgs { Job = pendingjob };            
            byte[] Answer;
            if (MessageLength != 0)
            {
                // Check the reply
                if (!CheckReply(MessageBuffer, MessageLength, mJob, s) && !TestConnection)
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
        
        bool CheckReply( byte[] Buffer, uint ReplyLength, SaiaDataModeCommJob pendingjob, SaiaDataModeStation s)
        {
            ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
            eAJob.Job = pendingjob;
            // Check the reply length
            if (ReplyLength < MinMessageLength)
            {
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorInvalidLenght;
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
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorCrc;
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
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                        OnJobExecuted(eAJob);
                        return false;
                    }

                case ReplyAttributes.ACK_NAK:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
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
                            eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseNack;
                            OnJobExecuted(eAJob);
                            return false;
                        }
                    }

                // Unexpected attribute
                default:
                    if (GestReadCommand)
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorReadResponseAttribute;
                    }
                    else
                    {
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)SaiaDataModetErrorCodes.ErrorWriteResponseAttribute;
                    }
                    OnJobExecuted(eAJob);
                    return false;
            }
        }

        //protected override void WorkingThread(object data)
        //{
        //}

        #endregion

        #region Properties
        /// <summary>
        /// Flag the test connection
        /// </summary>
        public bool TestConnection = false;
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

            //lock (j.retLockList())
            //{
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
                    object jobData = null;
                    j.GetJobData(ref jobData);
                    // discard inputoutput/exception output unchanged write value operation
                    if (j.TagsListOnWriting.Count == 0 || jobData == null)
                    {
                        return (0);
                    }
                    tmpBuffer = (byte[])jobData;
                    // Base address
                    StartAddress = new ushortUnion(j.GetWriteAddres());
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
            //}
        }

        public uint PrepareRequestTestInformation(SaiaDataModeCommJob j, ref byte[] buffer)
        {
            if (j == null || (j.Station as SaiaDataModeStation == null))
                return 0;

            //lock (j.retLockList())
            //{
            SaiaDataModeStation s = j.Station as SaiaDataModeStation;
            uint Count = 0;
            // Frame Synchronization
            buffer[Count++] = 0xB5;
            // Attribute (0 = request)
            buffer[Count++] = 0;
            // Station ID
            AddByteOrDLEsequence(s.StationID, ref buffer, ref Count);
            // Command
            buffer[Count++] = 0xab;
            buffer[Count++] = 0x00;
            buffer[Count++] = 0x06;
            //AddByteOrDLEsequence(0xab, ref buffer, ref Count);
            //byte[] tmpBuffer = new byte[0];
            //ushortUnion StartAddress = new ushortUnion();

            //if (!GestReadCommand)
            //{
            //    object jobData = null;
            //    j.GetJobData(ref jobData);
            //    // discard inputoutput/exception output unchanged write value operation
            //    if (j.TagsListOnWriting.Count == 0 || jobData == null)
            //    {
            //        return (0);
            //    }
            //    tmpBuffer = (byte[])jobData;
            //    // Base address
            //    StartAddress = new ushortUnion(j.GetWriteAddres());
            //}
            // Quantity
            //AddByteOrDLEsequence(j.GetQuantity(GestReadCommand, (uint)tmpBuffer.Count()), ref buffer, ref Count);
            //// DB number
            //if (j.AreaType == AreaTypes.DataBlock)
            //{
            //    AddByteOrDLEsequence(j.DbNumber.HIBYTE, ref buffer, ref Count);
            //    AddByteOrDLEsequence(j.DbNumber.LOBYTE, ref buffer, ref Count);
            //}

            //if (!GestReadCommand)
            //{
            //    AddByteOrDLEsequence(StartAddress.HIBYTE, ref buffer, ref Count);
            //    AddByteOrDLEsequence(StartAddress.LOBYTE, ref buffer, ref Count);

            //    // Quantity of bits
            //    if (j.IsBitDataFormat())
            //    {
            //        AddByteOrDLEsequence(j.GetWriteBitQuantity(), ref buffer, ref Count);
            //    }

            //    for (uint i = 0; i < tmpBuffer.Count(); i++)
            //    {
            //        AddByteOrDLEsequence(tmpBuffer[i], ref buffer, ref Count);
            //    }
            //}
            //else
            //{
                // Base address
                //AddByteOrDLEsequence(0x00, ref buffer, ref Count);
                //AddByteOrDLEsequence(0x06, ref buffer, ref Count);
            //}

            // Calculate the CRC
            ushortUnion nCRC = SaiaDataModeProtocol.CalculateCRC(buffer, Count);
            // Append the CRC to the message
            AddByteOrDLEsequence(nCRC.HIBYTE, ref buffer, ref Count);
            AddByteOrDLEsequence(nCRC.LOBYTE, ref buffer, ref Count);

            return Count;
            //}
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
                // if no data was recived (or data in the temporary buffer was consumed by parsed), check if some data arrived in the while
                if (ReceiveBufferBuffer.Count == 0)
                {
                    DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
                    WaitNewDataEvent(ref conn);
                    if (conn == DriverErrorCodes.ErrorNoError)
                    {
                        lock (lockThreadObject)
                        {
                            if (ReceiveBuffer.Count > 0)
                            {
                                ReceiveBufferBuffer.AddRange(ReceiveBuffer);
                                ReceiveBuffer.Clear();
                            }
                        }
                    }
                }

                if (ReceiveBufferBuffer.Count > 0)
                {
                    ReceiveBufferBuffer.CopyTo(0, inBuffer, MessageLength, 1);
                    ReceiveBufferBuffer.RemoveRange(0, 1);
                    RetValue = true;
                    MessageLength++;
                }

            }
            return RetValue;
        }

        private void Flush()
        {
            uint BytesToRead = GetBytesToRead();
            while (BytesToRead != 0)
            {
                byte[] Buffer = new byte[BytesToRead];
                DeviceRead(Buffer, BytesToRead);
                BytesToRead = GetBytesToRead();
            }
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }
        }

        private void ResetCommunicationVars()
        {
            ReceiveBufferBuffer.Clear();
            MessageLength = 0;
            ReceivedChars = 0;
            Array.Clear(AnswerBuffer, 0, AnswerBuffer.Length);
            Array.Clear(MessageBuffer, 0, MessageBuffer.Length);
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
        List<byte> ReceiveBufferBuffer = new List<byte>();
        ushort MessageLength = 0;
        ushort ReceivedChars = 0;
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
