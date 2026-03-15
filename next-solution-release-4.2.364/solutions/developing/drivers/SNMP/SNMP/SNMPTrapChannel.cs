using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using IpDriverCodeBase;

namespace SNMP
{
    class SNMPTrapChannel : UdpChannel
    {
        #region Constructors
        public SNMPTrapChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        public SNMPTrapChannel(CommunicationDriver commdriver)
            : base(commdriver)
        {
            UdpChannelLocalHostPort = 162;
        }
        #endregion

        #region Override Methods
        public override bool Startup()
        {
            return (base.Startup());
        }

        protected override void WorkingThread(object data)
        {

            {
                String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                String DbgTxt = String.Format("SNMP DBG - {0} SNMPTrapChannel.WorkingThread started", curTimeTxt);
                System.Diagnostics.Trace.TraceInformation(DbgTxt);
            }

            int sleepCycle = (Int32)WaitTime;
            if (sleepCycle == 0)
            {
                sleepCycle = 1;
            }

            while (true)
            {
                if(!IsDeviceOpen())
                {
                    if(DeviceOpen())
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} SNMPTrapChannel.WorkingThread calling BeginDeviceRead", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        BeginDeviceRead(0);
                    }
                }

                lock (lockThreadObject)
                {
                    bool bNew = NewDataToAnlyze.WaitOne(0, false);
                    if (bNew || ((ReceivePacketQueue != null) && (ReceivePacketQueue.Count > 0)))
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} SNMPTrapChannel.WorkingThread calling ReadAndParseMessage", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        NewDataToAnlyze.Reset();
                        ReadAndParseMessage();
                    }
                }

                if (StopWorkerThread.WaitOne(sleepCycle, false))
                {

                    {
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("SNMP DBG - {0} SNMPTrapChannel.WorkingThread stopped", curTimeTxt);
                        System.Diagnostics.Trace.TraceInformation(DbgTxt);
                    }

                    break;
                }
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
            DeviceWrite(replyMessage, (uint)messageLength, ipEndPoint);
        }

        private void ReadAndParseMessage()
        {
            ReceivedPacket packet;
            lock (lockThreadObject)
            {
                packet = ReceivePacketQueue.Dequeue();
                String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                String DbgTxt = String.Format("SNMP DBG - {0} Received Trap message of Length = {1}", curTimeTxt, packet.Data.Length);

                // If the message is of type Inform, send a reply
                System.Diagnostics.Trace.TraceInformation(DbgTxt);
                ReplyToInformMessages(packet);

                // Pass the message to the corresponding channel (if any)
                SNMPDriver snmpDriver = (SNMPDriver)CommDriver;
                if(snmpDriver != null)
                {
                    snmpDriver.PassMessageToChannel(packet);
                }
            }
        }
        #endregion
    }
}
