using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ModbusTCPSlave
{
    public class ModbusTCPSlaveChannel : TcpChannel
    {                
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public ModbusTCPSlaveChannel(CommunicationDriver commdriver, ModbusTCPSlaveChannelSettings settings, TcpClient client, TcpServer callbackchannel, int clientID)
            : base(commdriver, settings, client, true)
        {
            inactivityTimeout = settings.InactivityTimeout; // in sec
            callBackChannel = callbackchannel;
            remoteIpAddress = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            ClientID = clientID;
        }

        #endregion

        #region Members
        // event generated when WorkingThead is terminated
        public event EventHandler<EventArgs> Terminated;
        private TcpServer callBackChannel;
        public int ClientID;
        private string remoteIpAddress;
        private uint inactivityTimeout;
        #endregion

        #region methods
        private void Response(ref ReceiveItem receiveItem)
        {
            List<ModbusTCPSlaveCommJob> changedJob = new List<ModbusTCPSlaveCommJob>();
            if (receiveItem.IsValid)
            {
                ADU Adu = new ADU();

                Adu.Mbap.transaction = receiveItem.Transaction;
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
                            callBackChannel.getChangedJobs(ref receiveItem, out changedJob);
                            callBackChannel.ExecuteWriteJob(ref changedJob);
                            if (receiveItem.Pdu.quantity >= 1 && receiveItem.Pdu.quantity <= ModbusSlaveProtocol.maxSizeOfFunctionCode(receiveItem.Pdu.functionCode))
                            {
                                Adu.Pdu.byteData = callBackChannel.getMemoryData(ref receiveItem);
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
                                    callBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                else
                                {
                                    int Offset = 0;
                                    switch (BufferExpand.toUInt16(ref receiveItem.Pdu._byteData, ref Offset))
                                    {
                                        case 0x0000:
                                        case 0xFF00:
                                            callBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                            break;
                                        default:
                                            Adu.Pdu.exception = 0x03;
                                            break;

                                    }
                                }
                                callBackChannel.ExecuteReadJob(ref changedJob);
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
                                callBackChannel.setMemoryData(ref receiveItem, ref receiveItem.Pdu._byteData, out changedJob);
                                callBackChannel.ExecuteReadJob(ref changedJob);
                            }
                            else
                                Adu.Pdu.exception = 0x03;
                            break;
                        case FunctionCodes.MaskWriteRegister:
                            callBackChannel.getChangedJobs(ref receiveItem, out changedJob);
                            callBackChannel.ExecuteWriteJob(ref changedJob);
                            byte[] buffer = callBackChannel.getMemoryData(ref receiveItem);
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
                                callBackChannel.setMemoryData(ref receiveItem, ref buffer, out changedJob);
                                callBackChannel.ExecuteReadJob(ref changedJob);
                            }
                            break;
                    }
                }
                byte[] answer = Adu.Pack();
                DeviceWrite(answer, (uint)answer.Length);
            }
        }
        #endregion

        #region Server
        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                lock (lockThreadObject)
                {
                    this.ServerParseDataReceived += ModbusTCPSlaveChannel_NewDataReceivedRelanch;
                    SocketManager.ClientDisconnect += ModbusTCPSlaveChannel_ClientDisconnect;

                    BeginDeviceRead(ModbusSlaveProtocol.MBAPHeaderSize);

                    // time in msec
                    CyclicCheckStart((int)inactivityTimeout * 1000, (int)inactivityTimeout * 1000);
                }
            }
            return base.Startup();
        }

        public override void Suspend()
        {
            this.ServerParseDataReceived -= ModbusTCPSlaveChannel_NewDataReceivedRelanch;
            SocketManager.ClientDisconnect -= ModbusTCPSlaveChannel_ClientDisconnect;

            CyclicCheckStop();

            base.Suspend();
        }

        private void ModbusTCPSlaveChannel_NewDataReceivedRelanch(object sender, NewDataReceivedRelanchArgs e)
        {
            ReceiveItem receiveItem = null;

            CyclicCheckStop();

            lock (lockThreadObject)
            {
                if (ReceiveBuffer.Count < ModbusSlaveProtocol.MBAPHeaderSize)
                {
                    e.NextBeginDeviceReadSize = ModbusSlaveProtocol.MBAPHeaderSize;
                }
                else
                {
                    int Offset = 0;
                    byte[] Header = new byte[MBAP.Size];
                    ReceiveBuffer.CopyTo(0, Header, 0, MBAP.Size);
                    MBAP Mbap = new MBAP(ref Header, ref Offset);

                    if (Mbap.protocol != MBAP.ModbusTcp || Mbap.pduLength > PDU.SizeMax || Mbap.pduLength < PDU.SizeMin)
                    {
                        e.NextBeginDeviceReadSize = Mbap.pduLength;
                    }
                    else
                    {
                        if ((MBAP.Size + Mbap.pduLength) > ReceiveBuffer.Count)
                        {
                            e.NextBeginDeviceReadSize = Mbap.pduLength;
                        }
                        else
                        {
                            receiveItem = new ReceiveItem();
                            ReceiveBuffer.RemoveRange(0, MBAP.Size);
                            byte[] Frame = new byte[Mbap.pduLength];
                            ReceiveBuffer.CopyTo(0, Frame, 0, Mbap.pduLength);
                            ReceiveBuffer.RemoveRange(0, Mbap.pduLength);
                            Offset = 0;
                            receiveItem.Pdu = new PDU(ref Frame, ref Offset);
                            receiveItem.IpAddress = remoteIpAddress;
                            if (callBackChannel.ValidateStation(ref receiveItem))
                            {
                                if (ModbusSlaveProtocol.validateFunctionCode(receiveItem.Pdu.functionCode))
                                {
                                    receiveItem.Transaction = Mbap.transaction;
                                    receiveItem.IsValid = true;
                                }
                                else
                                {
                                    receiveItem.Pdu.exception = 0x01;
                                }
                            }

                            e.NextBeginDeviceReadSize = MBAP.Size;
                        }
                    }
                }
            }

            if (receiveItem != null)
                Response(ref receiveItem);

            CyclicCheckReStart();
        }

        private void ModbusTCPSlaveChannel_ClientDisconnect(object sender, EventArgs e)
        {
            CyclicCheckStop();

            // send event to TcpServer so it can dispose channel
            EventHandler<EventArgs> temp = Terminated;
            if (temp != null)
                temp(this, new EventArgs());
        }

        event EventHandler<NewDataReceivedRelanchArgs> ServerParseDataReceived;

        public override void UpdateReceiveBuffer(object sender, byte[] rec, DateTime dt, out bool setNewDataEvent, out int beginDeviceReadSize)
        {
            beginDeviceReadSize = rec.Length;

            beginDeviceReadSize = 0;
            lock (lockThreadObject)
                ReceiveBuffer.AddRange(rec);

            EventHandler<NewDataReceivedRelanchArgs> temp = ServerParseDataReceived;
            if (temp != null)
            {
                NewDataReceivedRelanchArgs e = new NewDataReceivedRelanchArgs() { Sender = sender, Timestamp = dt, NextBeginDeviceReadSize = beginDeviceReadSize };
                temp(this, e);
                if (e.NextBeginDeviceReadSize > 0)
                    beginDeviceReadSize = e.NextBeginDeviceReadSize;
                setNewDataEvent = false;
            }
            else
            {
                setNewDataEvent = true;
            }
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {            
            base.Dispose();
        }
        #endregion

        #region Connection's check timer management

        protected override bool CyclicCheck()
        {         
            bool disconnect = (!Connected(inactivityTimeout));
            if (disconnect)
            {
                // send event to TcpServer so it can dispose channel
                EventHandler<EventArgs> temp = Terminated;
                if (temp != null)
                    temp(this, new EventArgs());
            }

            return true;
        }        
        #endregion
    }
}
