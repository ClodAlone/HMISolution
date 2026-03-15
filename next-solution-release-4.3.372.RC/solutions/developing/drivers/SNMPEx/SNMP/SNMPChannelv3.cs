using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;

namespace SNMP
{
    public partial class SNMPChannel : UdpChannelList
    {                
        public DriverErrorCodes PrepareDiscoveryRequestv3(List<CommJob> list, out byte[] requestBuffer)
        {
            SNMPStation st = list[0].Station as SNMPStation;

            SNMPProtocol.SNMPMessageGeneralData messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            messageGeneralData.PduType = (byte)SNMPProtocol.SnmpPduType.Get;
            messageGeneralData.MsgID = GetNewRequestId();
            // during discovery use default value --> agent should respond with supported max size
            messageGeneralData.MaxPacket = SNMPProtocol.GetMaxPacketSize(); 
            messageGeneralData.snmpVersion = snmpVersion;
            
            return SNMPProtocol.PrepareBaseRequestv3(st, messageGeneralData, out requestBuffer);
        }

        public DriverErrorCodes PrepareAutenticationRequestv3(List<CommJob> jList, out byte[] requestBuffer)
        {
            SNMPStation st = jList[0].Station as SNMPStation;

            SNMPProtocol.SNMPMessageGeneralData messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            messageGeneralData.PduType = (byte)SNMPProtocol.SnmpPduType.Get;
            messageGeneralData.MsgID = GetNewRequestId();
            messageGeneralData.MaxPacket = snmpMaxPacket;
            messageGeneralData.snmpVersion = snmpVersion;
            messageGeneralData.FlagReportable = true;
            messageGeneralData.SetUsmSecurity(st);            
            return SNMPProtocol.PrepareBaseRequestv3(st, messageGeneralData, out requestBuffer);
        }

        public DriverErrorCodes PrepareReadRequestv3(List<CommJob> jList, out byte[] requestBuffer)
        {
            SNMPStation st = jList[0].Station as SNMPStation;

            SNMPProtocol.SNMPMessageGeneralData messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            messageGeneralData.PduType = (byte)SNMPProtocol.SnmpPduType.Get;
            messageGeneralData.MsgID = GetNewRequestId();
            messageGeneralData.MaxPacket = snmpMaxPacket;
            messageGeneralData.snmpVersion = snmpVersion;
            messageGeneralData.FlagReportable = true;
            messageGeneralData.SetUsmSecurity(st);

            int jobIndex = 0;
            while (jobIndex < jList.Count)
            {
                SNMPCommJob j = jList[jobIndex] as SNMPCommJob;
                if (!j.oidObj.IsValid())
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    jList.RemoveAt(jobIndex);
                    jobIndex--;
                }
                else
                {
                    messageGeneralData.VariableBindings.Add(new SNMPProtocol.SNMPVariableBinding(j.snmpOid_Address));
                }
                jobIndex++;
            }

            if (jList.Count == 0)
            {
                requestBuffer = null;
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
            }
            
            return SNMPProtocol.PrepareBaseRequestv3(st, messageGeneralData, out requestBuffer);
        }

        public DriverErrorCodes PrepareWriteRequestv3(List<CommJob> jList, out byte[] requestBuffer)
        {
            SNMPStation st = jList[0].Station as SNMPStation;

            SNMPProtocol.SNMPMessageGeneralData messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            messageGeneralData.PduType = (byte)SNMPProtocol.SnmpPduType.Set;
            messageGeneralData.MsgID = GetNewRequestId();
            messageGeneralData.MaxPacket = snmpMaxPacket;
            messageGeneralData.snmpVersion = snmpVersion;
            messageGeneralData.FlagReportable = true;
            messageGeneralData.SetUsmSecurity(st);

            int jobIndex = 0;
            while (jobIndex < jList.Count)
            {
                SNMPCommJob j = jList[jobIndex] as SNMPCommJob;
                if (!j.oidObj.IsValid())
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    jList.RemoveAt(jobIndex);
                    jobIndex--;
                }
                else
                {
                    object objectData = null;
                    j.GetJobData(ref objectData);
                    if (j.TagsListOnWriting.Count == 0)
                    {
                        // remove job with no data to write
                        RemovePendingJob(j);
                        jList.RemoveAt(jobIndex);
                        jobIndex--;
                    }
                    else
                    {
                        messageGeneralData.VariableBindings.Add(new SNMPProtocol.SNMPVariableBinding(j.snmpOid_Address, (byte[])objectData));
                    }
                }
                jobIndex++;
            }

            if (jList.Count == 0)
            {
                requestBuffer = null;
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
            }

            return SNMPProtocol.PrepareBaseRequestv3(st, messageGeneralData, out requestBuffer);
        }
        
        private DriverErrorCodes PrepareRequestv3(List<CommJob> jList, out byte[] buffer)
        {
            if (jList[0].ReadRequest())
                return PrepareReadRequestv3(jList, out buffer);
            else
                return PrepareWriteRequestv3(jList, out buffer);
        }

        private DriverErrorCodes ParseDiscoveryResponse(List<byte> receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            DriverErrorCodes errorCode = SNMPProtocol.ParseBaseResponse(receiveBuffer, null, messageGeneralData);
            byte replyMessage = (byte)SNMPProtocol.SnmpPduType.Report;
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                if (messageGeneralData.InError())
                    errorCode = messageGeneralData.GetErrorCode();
                else if (messageGeneralData.PduType != replyMessage)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyType;
                else if (!IsEqualToLastRequestId(messageGeneralData.MsgID))
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyID;
            }

            return errorCode;
        }

        private DriverErrorCodes ParseAuthenticationResponse(List<CommJob> jList, List<byte> receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();
            SNMPStation st = jList[0].Station as SNMPStation;
            messageGeneralData.SetUsmSecurity(st);
            DriverErrorCodes errorCode = SNMPProtocol.ParseBaseResponse(receiveBuffer, new List<SNMPStation>() { st }, messageGeneralData);
            byte replyMessage = (byte)SNMPProtocol.SnmpPduType.Response;
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                if (messageGeneralData.PduType != replyMessage)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyType;
                //else if (!IsEqualToLastRequestId(messageGeneralData.MsgID))
                //    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyID;
            }

            return errorCode;
        }

        public uint PrepareInformReplyRequestv3(SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out byte[] replyMessage)
        {
            // Build the reply message
            replyMessage = new byte[messageGeneralData.TrapRawData.Length];
            Array.Copy(messageGeneralData.TrapRawData.ToArray(), replyMessage, messageGeneralData.TrapRawData.Length);
            replyMessage[messageGeneralData.ReplyBufferIndex] = (byte)SNMPProtocol.SnmpPduType.Response;

            return (uint)replyMessage.Length;
        }
    }
}