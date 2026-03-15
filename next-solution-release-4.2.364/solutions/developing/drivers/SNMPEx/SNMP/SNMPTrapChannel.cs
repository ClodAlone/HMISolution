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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using System.Net.Sockets;
using Opc.Ua;

namespace SNMP
{
    public class SNMPTrapChannel : UdpChannel, IDisposable
    {
        public UdpClient ClientListeningOnPort_162;
        public bool IsClientListeningOnPort_162 = false;

        IAsyncResult readResul_Ex;
        protected bool beginDeviceReadAlreadyDone_Ex = false;
        public bool IsConnected = false;
        protected bool autoRestartBeginDeviceRead = false;


        //#region Constructors
        public SNMPTrapChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        public SNMPTrapChannel(CommunicationDriver commdriver)
            : base(commdriver)
        {

        }

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
                    SystemEvent(null, string.Format(IpDriverCodeBaseEx.Properties.Resources.UdpSocketError, s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
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
            byte[] replyBuffer = new byte[0];
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any , Properties.Settings.Default.TRAP_PORT);

            if(ClientListeningOnPort_162 == null)
            {
                return;
            }

            try
            {
                replyBuffer = ClientListeningOnPort_162.EndReceive(ar, ref remoteEP);
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

            if (replyBuffer.Count() != 0)
            {
                ReadAndParseMessage(replyBuffer, remoteEP);
            }
        }


        #endregion

        #region Methods

        DriverErrorCodes ParseReplyHeader(byte[] replyBuffer, int replyLength, ref int parsedBytes, ref SNMPVERSION protocolVersion)
        {
            protocolVersion = SNMPVERSION.SNMPv1;

            parsedBytes = 0;
            int tempParsedBytes = 0;

            // Check the first byte of the message
            if (replyLength < 1)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            if (replyBuffer[tempParsedBytes++] != 0x30)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }

            // Check the length of the message
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Check the SNMP version
            if (replyLength < (tempParsedBytes + 3))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            if ((replyBuffer[tempParsedBytes++] != 0x02) || (replyBuffer[tempParsedBytes++] != 0x01))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            if (replyBuffer[tempParsedBytes] == 0)
            {
                protocolVersion = SNMPVERSION.SNMPv1;
            }
            else if (replyBuffer[tempParsedBytes] == 1)
            {
                protocolVersion = SNMPVERSION.SNMPv2c;
            }
            else
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorUnsupportedVersion);
            }
            tempParsedBytes++;

            parsedBytes = tempParsedBytes;

            return (DriverErrorCodes.ErrorNoError);
        }

        DriverErrorCodes ParseCommunity(byte[] replyBuffer, int replyLength, ref int parsedBytes, ref string community)
        {
            int tempParsedBytes = parsedBytes;
            // Check the type of the following object: 0x04 = octet string
            if ((replyLength < (tempParsedBytes + 1)) || (replyBuffer[tempParsedBytes++] != 0x04))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }

            // Check the length of the community string
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength < (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Parse the community string
            byte[] asciiBytes = new byte[lengthValue];
            for (int i = 0; i < lengthValue; i++)
            {
                asciiBytes[i] = replyBuffer[tempParsedBytes++];
            }
            Encoding ascii = Encoding.ASCII;
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);
            community = asciiString;

            parsedBytes = tempParsedBytes;

            return (DriverErrorCodes.ErrorNoError);
        }

        private void ReplyToInformMessages(ReceivedPacket packet)
        {
            // Parse the message
            byte[] receivedMessage = packet.Data;
            int messageLength = receivedMessage.GetLength(0);
            if (messageLength <= 0)
            {
                return;
            }

            // Check the protocol Version: it should be SNMPv2c
            int parsedBytes = 0;
            SNMPVERSION protocolVersion = SNMPVERSION.SNMPv1;
            DriverErrorCodes errorCode = ParseReplyHeader(receivedMessage, messageLength, ref parsedBytes, ref protocolVersion);
            if ((errorCode != DriverErrorCodes.ErrorNoError) || (protocolVersion != SNMPVERSION.SNMPv2c))
            {
                SystemEvent(null, String.Format(Properties.Resources.SNMPErrorChannelTrapMessageSNMPv1, packet.RemoteEndPoint.Address), EventSeverity.Low);
                return;
            }

            // Get the community
            string receivedCommunity = String.Empty;
            errorCode = ParseCommunity(receivedMessage, messageLength, ref parsedBytes, ref receivedCommunity);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                return;
            }

            // Check the message type. It should be 0xa6 = inform-request
            if (messageLength < (parsedBytes + 1))
            {
                return;
            }
            if (receivedMessage[parsedBytes] != 0xa6)
            {
                return;
            }

            // Build the reply message
            byte[] replyMessage = new byte[messageLength];
            Array.Copy(receivedMessage, replyMessage, messageLength);
            replyMessage[parsedBytes] = 0xa2; // 0xa2 == get-response

            // Send the reply message
            IPEndPoint ipEndPoint = packet.RemoteEndPoint;
            DeviceWrite_EX(replyMessage, (uint)messageLength, ipEndPoint);
        }

        public  void ReadAndParseMessage(byte[] Data, IPEndPoint RemoteEndPoint)
        {
            ReceivedPacket packet;
            packet.Data = Data;
            packet.TimeStamp = DateTime.Now;
            packet.RemoteEndPoint = RemoteEndPoint;
               
            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
            String DbgTxt = String.Format("SNMP DBG - {0} Received Trap message of Length = {1}", curTimeTxt, packet.Data.Length);

            // If the message is of type Inform, send a reply
            System.Diagnostics.Trace.TraceInformation(DbgTxt);
            ReplyToInformMessages(packet);

            // Pass the message to the corresponding channel (if any)
            SNMPDriver snmpDriver = (SNMPDriver)CommDriver;
            if (snmpDriver != null)
            {
                snmpDriver.PassMessageToChannel(packet);
            }
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
                //RemoteEndPoint.Port = Properties.Settings.Default.TRAP_PORT;
                ClientListeningOnPort_162.Send(Buffer, size, RemoteEndPoint);
            }
            catch (Exception e)
            {
                return (false);
            }
           
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
            DeviceClose();
            base.Dispose();
        }
        #endregion
    }
}
