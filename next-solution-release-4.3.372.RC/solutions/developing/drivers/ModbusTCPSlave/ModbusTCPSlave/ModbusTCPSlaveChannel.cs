using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace ModbusTCPSlave
{
    // TcpServer 
    public class TcpServer : Channel, IDisposable
    {
        private Dictionary<int, ModbusTCPSlaveChannel> _connections;
        TcpListener _listener;
        TcpListener _listenerBackup;
        /// <summary>   The lock list object. </summary>
        readonly Object lockListObject = new Object();
        private ModbusTCPSlaveChannelSettings _settings;
        List<ModbusTCPSlaveStation> listStation;
        List<string> listIpAddress;
        private int clientID;

        public TcpServer(CommunicationDriver commdriver, ModbusTCPSlaveChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _settings = settings;
            _connections = new Dictionary<int, ModbusTCPSlaveChannel>();
            clientID = 0;
        }

        #region override Methods

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
            IPAddress resolvedIPAddress;
            IPAddress resolvedBackupIPAddress;
            listStation = new List<ModbusTCPSlaveStation>();
            listIpAddress = new List<string>();
            foreach (ModbusTCPSlaveStation st in CommDriver.GetChannelStations(this))
            {
                if (st.addressingMode != ModbusTCPSlaveStation.addressingModes.invalid)
                {
                    listStation.Add(st);
                    st.createMemoryData();
                    ModbusTCPSlaveDriver drv = (st.GetCommDriver() as ModbusTCPSlaveDriver);
                    List<ModbusTCPSlaveCommJob> totalJobs = st.totalJobs();
                    foreach (var job in totalJobs)
                    {
                        job.SetJobInitialValue(out bool initialValueForced);
                        List<Tag> lista = new List<Tag>();
                        lock (job.retLockList())
                        {
                            foreach (var tag in job.TagsList)
                            {
                                tag.Value.StatusCode = Opc.Ua.StatusCodes.Good;
                                lista.Add(tag);
                            }
                        }

                        foreach (var tag in lista)
                        {
                           if (initialValueForced)
                                drv.OnTagChanged(tag.TagNode.NodeId, tag.Value);
                            else
                                drv.OnTagChanged(tag.TagNode.NodeId, new DataValue(Opc.Ua.StatusCodes.Good));
                        }
                    }
                    if (st.addressingMode == ModbusTCPSlaveStation.addressingModes.IpAddres)
                    {
                        listIpAddress.Add(st.ipAddress);
                        if (!string.IsNullOrEmpty(st.backupIpAddress))
                            listIpAddress.Add(st.backupIpAddress);
                    }
                }
            }
            try
            {                
                if (String.IsNullOrEmpty(_settings.TcpChannelSettingsHostName) || !UdpChannel.GetResolvedConnecionIPAddress(_settings.TcpChannelSettingsHostName, out resolvedIPAddress))
                    UdpChannel.GetResolvedConnecionIPAddress("localhost", out resolvedIPAddress);
                _listener = new TcpListener(new IPEndPoint(resolvedIPAddress, _settings.TcpChannelSettingsHostPort));
                _listener.Start();
                _listenerBackup = null;
                if (!String.IsNullOrEmpty(_settings.TcpChannelSettingsBackupHostName) && UdpChannel.GetResolvedConnecionIPAddress(_settings.TcpChannelSettingsBackupHostName, out resolvedBackupIPAddress))
                {
                    if (resolvedBackupIPAddress != resolvedIPAddress)
                    {
                        _listenerBackup = new TcpListener(new IPEndPoint(resolvedBackupIPAddress, _settings.TcpChannelSettingsHostPort));
                        _listenerBackup.Start();
                    }
                }
                TcpClient client;
                ModbusTCPSlaveChannel channel;
                while (true)
                {                    
                    if (_listener.Pending())
                    {
                        client = _listener.AcceptTcpClient();
                        clientID++;
                        channel = new ModbusTCPSlaveChannel(CommDriver, _settings, client, this, clientID);
                        lock (lockListObject)
                            _connections[clientID] = (channel as ModbusTCPSlaveChannel);
                        if (StopWorkerThread.WaitOne(10))
                            break;
                        channel.Terminated += Channel_Terminated;
                        channel.Startup();
                    }
                    else if (_listenerBackup != null && _listenerBackup.Pending())
                    {
                        client = _listenerBackup.AcceptTcpClient();
                        clientID++;
                        channel = new ModbusTCPSlaveChannel(CommDriver, _settings, client, this, clientID);
                        lock (lockListObject)
                            _connections[clientID] = (channel as ModbusTCPSlaveChannel);
                        if (StopWorkerThread.WaitOne(10))
                            break;
                        channel.Terminated += Channel_Terminated;
                        channel.Startup();
                    }
                    else if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }                
            }
            catch (Exception ex)
            {
                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorGenericWorkingThread, _settings.TcpChannelSettingsHostName, _settings.TcpChannelSettingsHostPort,ex.Message), EventSeverity.Min);
                // report fatal error on channel putting in all tags in error
                SetChannelInFatalErrorState();
            }
        }

        //public abstract bool Init();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            return false;
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
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            return 0;
        }
        #endregion
        public ConcurrentQueue<CommJob> PublicGetNextPendingQueue()
        {
            return GetNextPendingQueue();
        }

        public void PublicOnJobExecuted(ExecutedJobArgs e)
        {
            OnJobExecuted(e);
        }

        public bool validateStation(ref ReceiveItem receiveItem)
        {
            if (listStation.Count() == 0)
                return true;
            else if (listStation.Count() == 1)
            {
                receiveItem.station = 0;
                return true;
            }
            else
            {
                for (byte i = 0; i < listStation.Count(); i++)
                {
                    if(listStation[i].addressingMode == ModbusTCPSlaveStation.addressingModes.unitID)
                    {
                        if (listStation[i].StationID == receiveItem.Pdu.unit &&
                            !listIpAddress.Contains(receiveItem.ipAddress))
                        {
                            receiveItem.station = i;
                            return true;
                        }
                    }
                    else if (listStation[i].ipAddress == receiveItem.ipAddress ||
                        listStation[i].backupIpAddress == receiveItem.ipAddress)
                    {
                        receiveItem.station = i;
                        return true;
                    }
                }
                return false;
            }
        }

        public byte[] getMemoryData(ref ReceiveItem receiveItem)
        {
            return listStation[receiveItem.station].getMemoryData(ref receiveItem);
        }
        public void setMemoryData(ref ReceiveItem receiveItem, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            listStation[receiveItem.station].setMemoryData(ref receiveItem, ref buffer, out changeJobs);
        }
        public void getChangedJobs(ref ReceiveItem receiveItem, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            listStation[receiveItem.station].getChangedJobs(ref receiveItem, out changeJobs);
        }

        public void ExecuteReadJob(ref List<ModbusTCPSlaveCommJob> readJobs)
        {
            if (readJobs.Count() == 0)
                return;
            foreach (ModbusTCPSlaveCommJob pendingjob in readJobs)
            {
                if (pendingjob.ReadRequest())
                {
                    lock (pendingjob.retLockList())
                    {
                        ExecuteJob(pendingjob);
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    pendingjob.LastExecutionTime = DateTime.UtcNow;
                    eJob.Values = (pendingjob.Station as ModbusTCPSlaveStation).getMemoryData(pendingjob);
                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);
                }
            }
        }

        public void ExecuteWriteJob(ref List<ModbusTCPSlaveCommJob> writeJobs)
        {
            if (writeJobs.Count() == 0)
                return;
            foreach (ModbusTCPSlaveCommJob pendingjob in writeJobs)
            {
                if (!pendingjob.ReadRequest())
                {
                    byte[] writeData;
                    if (pendingjob.PrepareData(out writeData))
                    {
                        lock (pendingjob.retLockList())
                        {
                            ExecuteJob(pendingjob);
                        }
                        (pendingjob.Station as ModbusTCPSlaveStation).setMemoryData(pendingjob, ref writeData);
                        pendingjob.LastExecutionTime = DateTime.UtcNow;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Values = new byte[0];
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                    }
                }
                else
                {
                    lock (pendingjob.retLockList())
                    {
                        ExecuteJob(pendingjob);
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    pendingjob.LastExecutionTime = DateTime.UtcNow;
                    eJob.Values = (pendingjob.Station as ModbusTCPSlaveStation).getMemoryData(pendingjob);
                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);
                }
            }
        }

        /// <summary>
        /// On fatal error (ex. tcp listner cannot be open on specificed port, all tags (and stata variables) were set in error
        /// </summary>
        private void SetChannelInFatalErrorState()
        {                       
            foreach (ModbusTCPSlaveStation st in CommDriver.GetChannelStations(this))
            {
                if (st.addressingMode != ModbusTCPSlaveStation.addressingModes.invalid)
                {
                    st.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                                        
                    List<ModbusTCPSlaveCommJob> totalJobs = st.totalJobs();
                    foreach (var job in totalJobs)
                        job.SetQuality(Opc.Ua.StatusCodes.BadConfigurationError);
                }
            }

            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        private void Channel_Terminated(object sender, EventArgs e)
        {
            StopClient((ModbusTCPSlaveChannel)sender);
        }

        // call this method only when driver is closing
        public void StopClients()
        {
            List<ModbusTCPSlaveChannel> clients = new List<ModbusTCPSlaveChannel>();
            lock (lockListObject)
            {
                clients.AddRange(_connections.Values);
            }
            while (clients.Count > 0)
            {
                StopClient(clients[0], true);
                clients.RemoveAt(0);
            }
        }


        public void StopClient(ModbusTCPSlaveChannel client, bool closing = false)
        {
            ModbusTCPSlaveChannel c = null;
            lock (lockListObject)
            {
                if (client == null || !_connections.ContainsKey(client.ClientID))
                    return;

                c = _connections[client.ClientID];
                c.Terminated -= Channel_Terminated;

                _connections.Remove(client.ClientID);
            }

            // when driver is closing dispose immediatly
            if (closing)
            {
                c.Dispose();
            }
            else
            {
                // otherwise postpone dispose using ThreadPool
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        c.Dispose();
                    }
                    catch (Exception ex) { }
                });
            }
        }

        #region IDisposable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            base.Dispose();

            _listener.Stop();
            _listener = null;

            StopClients();
        }
        #endregion

    }

    public struct ReceiveItem
    {
        public PDU Pdu;
        public bool isValid;
        public ushort transaction;
        public byte station;
        public string ipAddress;
        public ReceiveItem(PDU inPdu)
        {
            Pdu = inPdu;
            isValid = false;
            transaction = 0;
            station = 0;
            ipAddress = "";
        }
    }

    public class ModbusTCPSlaveChannel : TcpChannel
    {
        
        enum ReciveStates
        {
            Init,
            WaitHeader,
            WaitData,
        }

        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public ModbusTCPSlaveChannel(CommunicationDriver commdriver, ModbusTCPSlaveChannelSettings settings, TcpClient client, TcpServer callBackChannel, int clientID)
            : base(commdriver, settings, false, client)
        {
            InactivityTimeout = settings.InactivityTimeout;
            ReciveState = ReciveStates.Init;
            reciveTcp = false;
            CallBackChannel = callBackChannel;
            remoteIpAddress = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            ClientID = clientID;
        }

        #endregion

        #region Members
        // event generated when WorkingThead is terminated
        public event EventHandler<EventArgs> Terminated;
        ReciveStates ReciveState;
        bool reciveTcp;
        DateTime WaitDataTime;
        TcpServer CallBackChannel;
        public int ClientID;
        string remoteIpAddress;

        #endregion

        #region Override Methods
        /*
         * serial methods rule...
        public override void DeviceOpen()
        {
            base.DeviceOpen();
        }

        public override void DeviceClose()
        {
            base.DeviceClose();
        }

        public override bool DeviceRead(byte[] Buffer, uint Count)
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Working thread. </summary>
        ///
        /// <param name="data" type="object">   The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void WorkingThread(object data)
        {
            WaitDataTime = new DateTime();
            ReceiveItem receiveItem = new ReceiveItem(new PDU());
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;

            try
            {
                while (true)
                {
                    if (StopWorkerThread.WaitOne(0))
                        break;

                    // check device disconnection or communication inactivity for InactivityTimeout (sec)
                    if (!Connected(InactivityTimeout))
                        break;

                    ReceiveLoop(ref receiveItem);
                    Response(ref receiveItem);
                    receiveItem.isValid = false;

                    if (NewDataToAnlyze.WaitOne(sleepCycle))
                    {
                        reciveTcp = true;
                        lock (lockThreadObject)
                            NewDataToAnlyze.Reset();
                    }
                }
            }
            catch (Exception ex) { }

            // send event to TcpServer so it can dispose channel
            EventHandler<EventArgs> temp = Terminated;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }
        #endregion

        #region Properties

        /*private byte _FrameType;
        public byte FrameType
        {
            get { return _FrameType; }
            set
            {
                _FrameType = value;
            }
        }*/
        private uint _InactivityTimeout;
        public uint InactivityTimeout
        {
            get
            {
                return _InactivityTimeout;
            }
            set
            {
                _InactivityTimeout = value;
            }
        }
        
         
        #endregion

        #region methods

        private void ReceiveLoop(ref ReceiveItem receiveItem)
        {
            switch (ReciveState)
            {
                case ReciveStates.Init:
                    ReceiveBuffer.Clear();
                    BeginDeviceRead(ModbusSlaveProtocol.UINT_RequestLen);
                    ReciveState = ReciveStates.WaitHeader;
                    break;

                case ReciveStates.WaitHeader:
                    if (reciveTcp)
                    {
                        reciveTcp = false;
                        if (ReceiveBuffer.Count < MBAP.Size)
                        {
                            ReciveState = ReciveStates.Init;
                            ReceiveLoop(ref receiveItem);
                        }
                        else
                        {
                            int Offset = 0;
                            byte[] Header = new byte[MBAP.Size];
                            ReceiveBuffer.CopyTo(0, Header, 0, MBAP.Size);
                            MBAP Mbap = new MBAP(ref Header, ref Offset);

                            if (Mbap.protocol != MBAP.ModbusTcp ||
                                Mbap.pduLength > PDU.SizeMax ||
                                Mbap.pduLength < PDU.SizeMin)
                            {
                                ReciveState = ReciveStates.Init;
                                ReceiveLoop(ref receiveItem);
                            }
                            else
                            {
                                BeginDeviceRead(Mbap.pduLength);
                                ReciveState = ReciveStates.WaitData;
                                WaitDataTime = DateTime.UtcNow;
                            }

                        }
                    }
                    break;
                case ReciveStates.WaitData:
                    if (reciveTcp)
                    {
                        int Offset = 0;
                        byte[] Header = new byte[MBAP.Size];
                        ReceiveBuffer.CopyTo(0, Header, 0, MBAP.Size);
                        MBAP Mbap = new MBAP(ref Header, ref Offset);
                        reciveTcp = false;
                        if (ReceiveBuffer.Count < MBAP.Size + Mbap.pduLength)
                        {
                            ReciveState = ReciveStates.Init;
                            ReceiveLoop(ref receiveItem);
                        }
                        else
                        {
                            ReceiveBuffer.RemoveRange(0, MBAP.Size);
                            byte[] Frame = new byte[Mbap.pduLength];
                            ReceiveBuffer.CopyTo(0, Frame, 0, Mbap.pduLength);
                            ReceiveBuffer.RemoveRange(0, Mbap.pduLength);
                            Offset = 0;
                            receiveItem.Pdu = new PDU(ref Frame, ref Offset);
                            receiveItem.ipAddress = remoteIpAddress;
                            if (CallBackChannel.validateStation(ref receiveItem))
                            {
                                if (ModbusSlaveProtocol.validateFunctionCode(receiveItem.Pdu.functionCode))
                                {
                                    receiveItem.transaction = Mbap.transaction;
                                    receiveItem.isValid = true;
                                }
                                else
                                    receiveItem.Pdu.exception = 0x01;
                            }
                            BeginDeviceRead(MBAP.Size);
                            ReciveState = ReciveStates.WaitHeader;
                        }
                    }
                    else if ((DateTime.UtcNow - WaitDataTime).TotalMilliseconds > Timeout)
                    {
                        ReciveState = ReciveStates.Init;
                        ReceiveLoop(ref receiveItem);
                    }

                    break;

            }
        }

        private void Response(ref ReceiveItem receiveItem)
        {
            List<ModbusTCPSlaveCommJob> changedJob = new List<ModbusTCPSlaveCommJob>();
            if (receiveItem.isValid)
            {
                ADU Adu = new ADU();

                Adu.Mbap.transaction = receiveItem.transaction;
                Adu.Pdu.unit = receiveItem.Pdu.unit;
                Adu.Pdu.functionCode = receiveItem.Pdu.functionCode;
                if (receiveItem.Pdu.exception != 0x00)
                    Adu.Pdu.exception = receiveItem.Pdu.exception;
                else
                {
                    switch (receiveItem.Pdu.functionCode)
                    {
                        case FunctionCodes.ReadCoils:
                        case FunctionCodes.ReadDiscreteInputs:
                        case FunctionCodes.ReadHoldingRegisters:
                        case FunctionCodes.ReadInputRegisters:
                            CallBackChannel.getChangedJobs(ref receiveItem, out changedJob);
                            CallBackChannel.ExecuteWriteJob(ref changedJob);
                            if (receiveItem.Pdu.quantity >= 1 && receiveItem.Pdu.quantity <= ModbusSlaveProtocol.maxSizeOfFunctionCode(receiveItem.Pdu.functionCode))
                            {
                                Adu.Pdu.byteData = CallBackChannel.getMemoryData(ref receiveItem);
                                Adu.Pdu.byteCount = (byte)Adu.Pdu.byteData.Length;
                                if (Adu.Pdu.byteCount == 0)
                                    Adu.Pdu.exception = 0x02;
                            }
                            else
                                Adu.Pdu.exception = 0x03;
                            break;
                        case FunctionCodes.WriteSingleCoil:
                        case FunctionCodes.WriteSingleRegister:
                            if (receiveItem.Pdu.byteData.Length == receiveItem.Pdu.byteCount)
                            {
                                Adu.Pdu.startAddress = receiveItem.Pdu.startAddress;
                                Adu.Pdu.byteData = receiveItem.Pdu.byteData;
                                if (receiveItem.Pdu.functionCode == FunctionCodes.WriteSingleRegister)
                                    CallBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                else
                                {
                                    int Offset = 0;
                                    switch (BufferExpand.toUInt16(ref receiveItem.Pdu._byteData, ref Offset))
                                    {
                                        case 0x0000:
                                        case 0xFF00:
                                            CallBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                            break;
                                        default:
                                            Adu.Pdu.exception = 0x03;
                                            break;

                                    }
                                }
                                CallBackChannel.ExecuteReadJob(ref changedJob);
                            }
                            else
                                Adu.Pdu.exception = 0x03;
                            break;
                        case FunctionCodes.WriteMultipleCoils:
                        case FunctionCodes.WriteMultipleRegisters:
                            if (receiveItem.Pdu.byteData.Length == receiveItem.Pdu.byteCount)
                            {
                                Adu.Pdu.startAddress = receiveItem.Pdu.startAddress;
                                Adu.Pdu.quantity = receiveItem.Pdu.quantity;
                                Adu.Pdu.byteCount = receiveItem.Pdu.byteCount;
                                CallBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                CallBackChannel.ExecuteReadJob(ref changedJob);
                            }
                            else
                                Adu.Pdu.exception = 0x03;
                            break;
                        case FunctionCodes.MaskWriteRegister:
                            CallBackChannel.getChangedJobs(ref receiveItem, out changedJob);
                            CallBackChannel.ExecuteWriteJob(ref changedJob);
                            byte[] buffer = CallBackChannel.getMemoryData(ref receiveItem);
                            if (buffer.Length != 2)
                                Adu.Pdu.exception = 0x02;
                            else
                            {
                                int Offset = 0;
                                buffer = 
                                    BufferExpand.toArray((ushort)((
                                    BufferExpand.toUInt16(ref buffer, ref Offset) & receiveItem.Pdu.AndMask) |
                                    (receiveItem.Pdu.OrMask & (receiveItem.Pdu.AndMask ^ 0xFFFF))));
                                Adu.Pdu.startAddress = receiveItem.Pdu.startAddress;
                                Adu.Pdu.AndMask = receiveItem.Pdu.AndMask;
                                Adu.Pdu.OrMask = receiveItem.Pdu.OrMask;
                                CallBackChannel.setMemoryData(ref receiveItem, ref buffer, out changedJob);
                                CallBackChannel.ExecuteReadJob(ref changedJob);
                            }
                            break;
                    }
                }
                byte[] answer = Adu.Pack();
                DeviceWrite(answer, (uint)answer.Length);
            }

        }
        
        #endregion
    }
}
