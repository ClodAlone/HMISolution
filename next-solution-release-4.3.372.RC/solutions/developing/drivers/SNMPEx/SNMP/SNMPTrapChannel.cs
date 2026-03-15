//  Flowchart SNMPTrapChannel
//  if the agent detects an emergency event on the device that it is monitoring,
//  it will send out a warning message to the manager without waiting to be polled for data.
//  This emergency message is called a trap.
//
//             ┌─────────────────────────────────┐
//             │   Class SNMPDriver              │
//             │                                 │
//             │                                 │
//             │ ┌─────────────────────────────┐ │
//             │ │ BuildSNMPChannelDictionary  │ │
//             │ └─────────────────────────────┘ │
//             │                                 │
//             │ ┌──────────────────────┐        │
//    ┌────────┼─► PassMessageToChannel ├────────┼────────────────┐
//    │        │ └──────────────────────┘        │                │
//    │        │                                 │                │
//    │        │ ┌───────────┐                   │                │
//    │        │ │ StartUp   │                   │                │
//    │        │ └─────┬─────┘                   │                │
//    │        │       │                         │                │
//    │        │ ┌─────▼─────────────┐           │                │
//    │   ┌────┼─┤ StartTrapChannel  │           │                │
//    │   │    │ └───────────────────┘           │                │
//    │   │    │                                 │                │
//    │   │    └─────────────────────────────────┘                │
//    │   │                                                       │
//    │   │                                                       │
//    │   │                                                       │
//    │   │                                                       │
//    │   │    ┌─────────────────────────────────┐                │
//    │   │    │  Class SNMPTrapChannel          │                │
//    │   │    │                                 │                │
//    │   │    │ ┌────────────┐                  │                │
//    │   └────┼─► Costructor │                  │                │
//    │        │ └─────┬──────┘                  │                │
//    │        │       │                         │                │
//    │        │ ┌─────▼──────┐                  │                │
//    │        │ │ DeviceOpen │                  │                │          ┌────────────────────────┐
//    │        │ └──────┬─────┘                  │                │          │                        │
//    │        │        │                        │                │          │    Class SNMPChannel   │
//    │        │ ┌──────▼─────────────┐          │                │          │                        │
//    │        │ │ BeginDeviceRead_Ex │          │                │          │                        │
//    │        │ └──────┬─────────────┘          │                │          │   ┌────────────────┐   │
//    │        │        │                        │                └──────────┼───► AddTrapMessage │   │
//    │        │ ┌──────▼───────────────┐        │                           │   └────────┬───────┘   │
//    │        │ │ Istanzia la CallBack │        │                           │            │           │
//    │        │ │                      │        │                           │            │           │
//    │        │ │ (ReadCallBack_Ex)    │        │                           │  ┌─────────▼─────────┐ │
//    │        │ └──────────────────────┘        │                           │  │ ManageTrapMessage │ │
//    │        │                                 │                           │  └───────────────────┘ │
//    │        │ ┌───────────────────────┐       │                           │                        │
//    │        │ │  ReadCallBack_Ex      │       │                           │                        │
//    │        │ └─────────┬─────────────┘       │                           └────────────────────────┘
//    │        │           │                     │
//    │        │ ┌─────────▼──────────────┐      │
//    └────────┼─┤ ReadAnParseMessage     │      │
//             │ └────────────────────────┘      │
//             │                                 │
//             └─────────────────────────────────┘
//      		   

using System;
using System.Linq;
using System.Net;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using System.Net.Sockets;
using Opc.Ua;
using System.Collections.Generic;
using System.Threading;

namespace SNMP
{
    public class SNMPTrapChannel : UdpChannel, IDisposable
    {
        public class TrapMessage
        {
            public byte[] Data { set; get; }
            public IPEndPoint RemoteEndPoint { set; get; }

            public TrapMessage()
            {
                Data = null;
                RemoteEndPoint = null;
            }

            public TrapMessage(byte[] data, IPEndPoint remoteEndPoint)
            {
                Data = data;
                RemoteEndPoint = remoteEndPoint;
            }
        }

        public UdpClient ClientListeningOnPort_162;
        public bool IsClientListeningOnPort_162 = false;

        IAsyncResult readResul_Ex;
        protected bool beginDeviceReadAlreadyDone_Ex = false;
        public bool IsConnected = false;
        protected bool autoRestartBeginDeviceRead = false;

        private List<SNMPStation> allStations = null;

        #region Constructors
        public SNMPTrapChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        public SNMPTrapChannel(CommunicationDriver commdriver)
            : base(commdriver)
        {

        }
        #endregion

        #region Override Methods
        public bool BeginDeviceRead_Ex(int Count)
        {            
            if (ClientListeningOnPort_162 == null)
                return (false);

            if (beginDeviceReadAlreadyDone_Ex == true)
            {
                return (true);
            }

            beginDeviceReadAlreadyDone_Ex = true;
            EndDeviceRead_Ex();
           
            try
            {
                readResul_Ex = ClientListeningOnPort_162.BeginReceive(new AsyncCallback(ReadCallBack_Ex), this);
            }
            catch
            {
                beginDeviceReadAlreadyDone_Ex = false;
                //https://support.microsoft.com/en-us/kb/260018
                EndDeviceRead_Ex();
                return false;
            }
           
            return (true);
        }

        public void EndDeviceRead_Ex()
        {            
            if (readResul_Ex != null)
            {
                readResul_Ex.AsyncWaitHandle.Close();
                readResul_Ex.AsyncWaitHandle.Dispose();
                readResul_Ex = null;
            }         
        }

        public bool Connected()
        {
            return IsConnected;
        }

        public override void Suspend()
        {
            DeviceClose();
        }

        public override bool DeviceOpen()
        {
            autoRestartBeginDeviceRead = true;

            try
            {
                /* This is our own exclusive port. We'll receive everything sent to this. */
                /* So this is how we'll present our selves to the world */
                if (ClientListeningOnPort_162 == null)
                {
                    ClientListeningOnPort_162 = new UdpClient(Properties.Settings.Default.TRAP_PORT);
                    BeginDeviceRead_Ex(0);
                    SystemEvent(null, String.Format(Properties.Resources.SNMPChannelTrapActive), EventSeverity.Low);
                }

                IsConnected = true;
                CurrentErrorCode = 0;
            }
            catch(Exception e)
            {
                SocketException s = e as SocketException;
                if (s != null && CurrentErrorCode != s.ErrorCode)
                {
                    //log error...
                    CurrentErrorCode = s.ErrorCode;
                    SystemEvent(null, string.Format(Properties.Resources.SNMPErrorTrapDeviceOpen, Properties.Settings.Default.TRAP_PORT, s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                }
                ClientListeningOnPort_162 = null;
            }
            return true;
        }

        public override bool DeviceClose()
        {     
            EndDeviceRead_Ex();
                
            if (ClientListeningOnPort_162 != null)
            {
                try
                {
                    ClientListeningOnPort_162.Close();
                }
                catch (Exception e)
                {
                }
            }
            ClientListeningOnPort_162 = null;
            beginDeviceReadAlreadyDone_Ex = false;
           
            IsConnected = false;
            SystemEvent(null, String.Format(Properties.Resources.SNMPChannelTrapClose), EventSeverity.Low);

            return true;
        }

        public void ReadCallBack_Ex(IAsyncResult ar)
        {
            byte[] data = new byte[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any , Properties.Settings.Default.TRAP_PORT);

            if(ClientListeningOnPort_162 == null)
            {
                return;
            }

            try
            {
                data = ClientListeningOnPort_162.EndReceive(ar, ref remoteEndPoint);
            }
            catch (Exception e)
            {
                SocketException s = e as SocketException;
                if (s != null && CurrentErrorCode != s.ErrorCode)
                {
                    //log error...
                    CurrentErrorCode = s.ErrorCode;
                    SystemEvent(null, string.Format(IpDriverCodeBaseEx.Properties.Resources.UdpSocketError, s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                }
            }      

            beginDeviceReadAlreadyDone_Ex = false;

            if (autoRestartBeginDeviceRead)
                BeginDeviceRead_Ex(0);

            if (data.Length > 0)
            {
                // Check the protocol Version: it should be SNMPv2c            
                if (Properties.Settings.Default.TRAP_LOG_MESSAGES)
                {
                    string logMessage = string.Format("Incoming Trap message from {0} : {1}", remoteEndPoint.Address.ToString(), SNMPProtocol.SNMPByteMessageToHexString(data));
                    CommunicationDriver.OnLogEvent(logMessage, System.Diagnostics.EventLogEntryType.Information);
                }
                lock (lockTrapMessagesDispatcherEvents)
                {
                    trapMessagesList.Add(new TrapMessage(data, remoteEndPoint));                    
                    if (trapMessagesList.Count > SNMPProtocol.TRAP_MESSAGES_QUEUE_MAX_SIZE)
                        trapMessagesList.RemoveAt(0);
                    if (trapMessagesDispatcherEvents != null)
                        trapMessagesDispatcherEvents.Set();
                }
            }
        }        
        #endregion

        #region Methods

        public void SetAutenticationStations(List<SNMPStation> stations)
        { 
            allStations = stations;
        }

        private bool ReplyToInformTrapMessage(SNMPProtocol.SNMPMessageGeneralData messageGeneralData, byte[] replyBuffer)
        {            
            // Send the reply message
            DeviceWrite_EX(replyBuffer, (uint)replyBuffer.Length, messageGeneralData.TrapRemoteEndPoint);

            if (Properties.Settings.Default.TRAP_LOG_MESSAGES)
            {
                string logMessage = string.Format("Reply to {0} Inform Trap message", messageGeneralData.TrapRemoteEndPoint.Address.ToString());
                CommunicationDriver.OnLogEvent(logMessage, System.Diagnostics.EventLogEntryType.Information);
            }

            return true;            
        }

        private DriverErrorCodes ParseTrapResponse(byte[] data, IPEndPoint remoteEndPoint, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            messageGeneralData.IsMessageFromTrap = true;

            List<byte> receiveBuffer = data.ToList();
            int receiveBufferLength = data.Length;
            int parsedBytes = 0;
            DriverErrorCodes errorCode = SNMPProtocol.ParseHeaderResponse(receiveBuffer, receiveBufferLength, ref parsedBytes, messageGeneralData);
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                if (messageGeneralData.snmpVersion == SNMPVERSION.SNMPv1)
                    return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorChannelTrapMessageSNMPv1;                    
            } 

            errorCode = SNMPProtocol.ParseBaseResponse(data.ToList(), allStations, messageGeneralData);
            if (messageGeneralData != null)
                messageGeneralData.SetTrapData(remoteEndPoint, data);

            return errorCode;
        }

        public bool DeviceWrite_EX(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            int size = 0;
            
            if ((ClientListeningOnPort_162 == null) || RemoteEndPoint == null)
            {
                return (false);
            }

            size = (int)Count;
            try
            {
                ClientListeningOnPort_162.Send(Buffer, size, RemoteEndPoint);
            }
            catch (Exception e)
            {
                return (false);
            }
           
            return (true);
        }
        #endregion

        #region Trap messages dispacther
        Thread trapMessagesDispatcherThread;
        protected ManualResetEvent trapMessagesDispatcherEvents;
        protected object lockTrapMessagesDispatcherEvents = new object();
        List<TrapMessage> trapMessagesList = new List<TrapMessage>();
        bool bTrapMessagesDispatcherDisposed = false;

        public bool StartUpTrapMessagesDispatcherThread()
        {
            bool started = false;

            bTrapMessagesDispatcherDisposed = false;

            lock (lockTrapMessagesDispatcherEvents)
            {
                if (trapMessagesDispatcherEvents == null)
                    trapMessagesDispatcherEvents = new ManualResetEvent(false);
                else
                    trapMessagesDispatcherEvents.Reset();

                if (trapMessagesDispatcherThread == null)
                {
                    trapMessagesDispatcherThread = new Thread(TrapMessagesDispatcher);
                    trapMessagesDispatcherThread.IsBackground = true;
                    trapMessagesDispatcherThread.Priority = ThreadPriority.BelowNormal;
                    trapMessagesDispatcherThread.Name = "TrapMessagesDispatcher";

                    if (!trapMessagesDispatcherThread.IsAlive)
                        trapMessagesDispatcherThread.Start(0);
                    started = trapMessagesDispatcherThread != null && (trapMessagesDispatcherThread.ThreadState & (System.Threading.ThreadState.Stopped | System.Threading.ThreadState.Unstarted)) == 0;
                }
            }

            return started;
        }

        public void TerminateTrapMessagesDispatcherThread()
        {
            bTrapMessagesDispatcherDisposed = true;

            if (trapMessagesDispatcherEvents != null)
                trapMessagesDispatcherEvents.Set();

            if (trapMessagesDispatcherThread != null)
                trapMessagesDispatcherThread.Join();

            lock (lockTrapMessagesDispatcherEvents)
            {
                if (trapMessagesDispatcherThread != null)
                    trapMessagesDispatcherThread = null;

                if (trapMessagesDispatcherEvents != null)
                {
                    trapMessagesDispatcherEvents.Dispose();
                    trapMessagesDispatcherEvents = null;
                }
                trapMessagesList.Clear();
            }
        }                

        private void TrapMessagesDispatcher(object data)
        {
            int waitNewTrapMessage = 0;
            TrapMessage trapMessage = null;

            DeviceOpen();

            while (true)
            {
                trapMessagesDispatcherEvents.WaitOne(waitNewTrapMessage);
                if (bDisposed || bTrapMessagesDispatcherDisposed)
                    break;

                lock (lockTrapMessagesDispatcherEvents)
                {
                    if (trapMessagesDispatcherEvents != null)
                        trapMessagesDispatcherEvents.Reset();
                    if (trapMessagesList.Count > 0) 
                    {
                        trapMessage = trapMessagesList[0];
                        trapMessagesList.RemoveAt(0);                        
                    }
                    else
                    {
                        trapMessage = null;
                    }

                    waitNewTrapMessage = (trapMessagesList.Count > 0 ? 0 : -1); // 0 = execute now, -1 : wait for a new message
                }

                if (trapMessage != null)
                {
                    DriverErrorCodes errorCode = ParseTrapResponse(trapMessage.Data, trapMessage.RemoteEndPoint, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData);
                    if (errorCode != DriverErrorCodes.ErrorNoError)
                    {
                        if (Properties.Settings.Default.TRAP_LOG_ERRORS)
                        {
                            CommDriver.GetDriverErrorInfo((int)errorCode, out uint quality, out string error);
                            CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.SNMPErrorTrapErrorInFrame, trapMessage.RemoteEndPoint.Address, error, SNMPProtocol.SNMPByteMessageToHexString(trapMessage.Data)), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        List<SNMPChannel> channels = ((SNMPDriver)CommDriver).GetCompatibleTrapChannels(messageGeneralData);
                        if (channels.Count > 0)
                        {
                            bool informMessageReplied = false;
                            foreach (SNMPChannel ch in channels)
                            {
                                if (messageGeneralData.PduType == (byte)SNMPProtocol.SnmpPduType.Inform)
                                {
                                    if (!informMessageReplied)
                                    {
                                        uint byteNumber = ch.PrepareInformReplyRequest(messageGeneralData, out byte[] replyBuffer);
                                        if (byteNumber > 0)
                                            ReplyToInformTrapMessage(messageGeneralData, replyBuffer);
                                        informMessageReplied = true;
                                    }
                                }

                                ch.ManageTrapMessage(messageGeneralData);
                            }
                        }
                    }                    
                }
                waitNewTrapMessage = 0;
            }

            DeviceClose();
        }
        #endregion

        #region Disposse
        public override void Dispose()
        {
            TerminateTrapMessagesDispatcherThread();
            base.Dispose();
        }
        #endregion

    }
}
