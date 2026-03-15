using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using IpDriverCodeBase;
using System.Threading;

namespace SNMP
{
    public class SNMPChannel : UdpChannel
    {
        public const uint MAX_REQUEST_NUMBER = 500;

        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SNMPChannel(CommunicationDriver commdriver, SNMPChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _snmpVersion = settings.snmpVersion;
            _snmpMaxNumberOfAggregatedRequests = settings.snmpMaxNumberOfAggregatedRequests;
            if (_snmpMaxNumberOfAggregatedRequests == 0)
            {
                _snmpMaxNumberOfAggregatedRequests = 1;
            }
        }

        #endregion

        #region Data Members

        uint snmpRequestID = 0;
        uint snmpLastRequestID = 0;
        List<SNMPCommJob> nextlist = new List<SNMPCommJob>();
        byte[] requestBuffer = new byte[SNMPCommJob.MAX_SNMP_PACKET];
        List<ReceivedPacket> trapList = new List<ReceivedPacket>();
        protected Object lockTrapListObject = new Object();
        Dictionary<string, List<SNMPCommJob>> mapOIDJobs = new Dictionary<string, List<SNMPCommJob>>();
        protected Object lockMapOIDJobsObject = new Object();


        #endregion

        #region Override Methods

        private void ProcessDataFormatErrors()
        {
            List<SNMPCommJob> localListJobPendingCopy = new List<SNMPCommJob>();
            foreach(SNMPCommJob job in ListJobPending)
            {
                switch (job.badDataFormat) {
                    case SNMPCommJob.DataFormatErrors.NoError:
                        localListJobPendingCopy.Add(job);
                        break;

                    case SNMPCommJob.DataFormatErrors.DiscardIOEOUnchangedValue:
                        if ((SynchroJob != null) && (SynchroJob == job))
                        {
                            job.ResetSynchro.WaitOne(Timeout);
                            SynchroJob = null;
                            job.ResetSynchro.Reset();
                        }
                        // ListJobPending.Removed(..) will be execute below
                        break;

                    case SNMPCommJob.DataFormatErrors.GenericError:                    
                        if ((SynchroJob != null) && (SynchroJob == job))
                        {
                            job.ResetSynchro.WaitOne(Timeout);
                            SynchroJob = null;
                            job.ResetSynchro.Reset();
                        }
                                        
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = job;
                        eJob.ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                        OnJobExecuted(eJob);
                        if (job.TagsListToWrite.Count > 0)//error occurred prior to the write, nobody empty this list!
                        {
                            foreach (var tg in job.TagsListToWrite)
                            {
                                tg.SetReadValue(tg.Value.Value);
                            }
                            job.TagsListToWrite.Clear();
                        }
                        if (job.TagsListOnWriting.Count > 0)
                        {
                            foreach (var tg in job.TagsListOnWriting)
                            {
                                tg.SetReadValue(tg.Value.Value);
                            }
                            job.TagsListOnWriting.Clear();
                        }
                        //job.WriteExecuted = false;
                        break;
                }

                job.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;
            }

            ListJobPending.Clear();
            if(localListJobPendingCopy.Count > 0)
            {
                ListJobPending.AddRange(localListJobPendingCopy);
            }
        }
 
        protected override void WorkingThread(object data)
        {
            int sleepCycle = (Int32)WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            //bool bForceProcessListJobs = false;
            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            int dbgCount = 0;
            while (true)
            {
                dbgCount++;

                nextlist.Clear();
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    lock (lockThreadObject)//201011
                    {
                        if (SynchroJob != null)
                            nextlist.Add(SynchroJob as SNMPCommJob);
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }

                if (nextlist.Count > 0)
                {
                    if (!IsDeviceOpen())
                    {
                        DeviceOpen();
                    }
                    lock (lockThreadObject)//201011
                    {
                        ExecuteJobList(nextlist);
                        ProcessDataFormatErrors();
                    }
                    dbgCount = 0;
                }

                lock (lockThreadObject)
                {
                    bool bNew = NewDataToAnlyze.WaitOne(0, false);
                    if (bNew || ReceiveBuffer.Count > 0)
                    {
                        NewDataToAnlyze.Reset();
                        if (ListJobPending.Count > 0)
                        {
                            if (ProcessNewDataList(ListJobPending))
                            {
                                ReceiveBuffer.Clear();
                                foreach (var job in ListJobPending)
                                {
                                    ListJobExecuted.Add(job);
                                }
                            }
                            foreach (var job in ListJobExecuted)
                            {
                                job.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == job && SynchroJob.TagsListToWrite.Count == 0)
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
                                //error
                                if(ListJobPending[0].TagsListOnWriting.Count > 0)
                                {
                                    LastErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorWriteFail;
                                    foreach(var tg in ListJobPending[0].TagsListOnWriting)
                                    {
                                        tg.SetReadValue(tg.Value.Value);
                                    }
                                    ListJobPending[0].TagsListOnWriting.Clear();
                                    ((SNMPCommJob)ListJobPending[0]).WriteExecuted = false;
                                        
                                }
                                else
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewDataList(ListJobPending);
                                ListJobPending.RemoveAt(0);
                                ReceiveBuffer.Clear();
                                ManageTimeoutError();
                            }
                        }
                    }
                }

                ManageTrapMessages();

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                {
                    DeviceClose();
                }

                if (nextlist.Count != 0 && StopWorkerThread.WaitOne(sleepCycle, false))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                }
                StopWorkerThread.WaitOne(0, false);
            }
        }



        DriverErrorCodes ParseCommunity(List<byte> replyBuffer, int replyLength, ref int parsedBytes, ref string community)
        {
            int tempParsedBytes = parsedBytes;
            // Check the type of the following object: 0x04 = octet string
            if((replyLength < (tempParsedBytes + 1)) || (replyBuffer[tempParsedBytes++] != 0x04))
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

        DriverErrorCodes ParseReplyHeader(List<byte> replyBuffer, int replyLength, ref int parsedBytes, ref SNMPVERSION protocolVersion)
        {
            protocolVersion = SNMPVERSION.SNMPv1;

            parsedBytes = 0;
            int tempParsedBytes = 0;

            // Check the first byte of the message
            if(replyLength < 1)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            if(replyBuffer[tempParsedBytes++] != 0x30)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }

            // Check the length of the message
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Check the SNMP version
            if(replyLength < (tempParsedBytes + 3))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            if((replyBuffer[tempParsedBytes++] != 0x02) || (replyBuffer[tempParsedBytes++] != 0x01))
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

        DriverErrorCodes ParseReplyID(List<byte> replyBuffer, int replyLength, ref int parsedBytes, ref uint replyID)
        {
            int tempParsedBytes = parsedBytes;

            // Check the length of the remaining part of the message 
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Parse the identifier of the reply
            int integerSize = 0;
            //int integerValue = SNMPProtocol.ASN1DecodifyInteger(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            uint unsignedValue = SNMPProtocol.ASN1DecodifyUnsignedValue(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;
            //replyID = (uint)integerValue;
            replyID = unsignedValue;

            parsedBytes = tempParsedBytes;
            return (DriverErrorCodes.ErrorNoError);
        }

        DriverErrorCodes ParseReplyID(byte[] replyBuffer, int replyLength, ref int parsedBytes, ref uint replyID)
        {
            int tempParsedBytes = parsedBytes;

            // Check the length of the remaining part of the message 
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Parse the identifier of the reply
            int integerSize = 0;
            //int integerValue = SNMPProtocol.ASN1DecodifyInteger(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            uint unsignedValue = SNMPProtocol.ASN1DecodifyUnsignedValue(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;
            //replyID = (uint)integerValue;
            replyID = unsignedValue;

            parsedBytes = tempParsedBytes;
            return (DriverErrorCodes.ErrorNoError);
        }

        DriverErrorCodes ParseErrorStatus(List<byte> replyBuffer, int replyLength, ref int parsedBytes, ref int errorStatus, ref int errorIndex)
        {
            int tempParsedBytes = parsedBytes;

            // Parse the error status
            int integerSize = 0;
            int integerValue1 = SNMPProtocol.ASN1DecodifyInteger(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;

            // Parse the error index
            integerSize = 0;
            int integerValue2 = SNMPProtocol.ASN1DecodifyInteger(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;

            // Check the length of the remaining part of the message
            if(replyLength < (tempParsedBytes + 1))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes++;
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Set the output parameters
            errorStatus = integerValue1;
            errorIndex = integerValue2;
            parsedBytes = tempParsedBytes;

            return (DriverErrorCodes.ErrorNoError);
        }

        DriverErrorCodes ParseErrorStatus(byte[] replyBuffer, int replyLength, ref int parsedBytes, ref int errorStatus, ref int errorIndex)
        {
            int tempParsedBytes = parsedBytes;

            // Parse the error status
            int integerSize = 0;
            int integerValue1 = SNMPProtocol.ASN1DecodifyIntegerValue(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;

            // Parse the error index
            integerSize = 0;
            int integerValue2 = SNMPProtocol.ASN1DecodifyIntegerValue(replyBuffer, replyLength, tempParsedBytes, ref integerSize);
            if (integerSize == 0)
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += integerSize;

            // Check the length of the remaining part of the message
            if (replyLength < (tempParsedBytes + 1))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes++;
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(replyBuffer, replyLength, tempParsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (replyLength != (lengthValue + tempParsedBytes + lengthSize)))
            {
                return ((DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply);
            }
            tempParsedBytes += lengthSize;

            // Set the output parameters
            errorStatus = integerValue1;
            errorIndex = integerValue2;
            parsedBytes = tempParsedBytes;

            return (DriverErrorCodes.ErrorNoError);
        }

        bool ProcessNewDataList(List<CommJob> list)
        {
            bool fromWrite = ((SNMPCommJob)list[0]).WriteExecuted;
            SNMPStation s = list[0].Station as SNMPStation;
            if (s == null)
            {
                ReceiveBuffer.Clear();
                Flush();
                return false;
            }
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Check the common part of the message

            // Check the reply header
            int replyLength = ReceiveBuffer.Count;
            if(replyLength <= 0)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            int parsedBytes = 0;
            SNMPVERSION protocolVersion = SNMPVERSION.SNMPv1;
            DriverErrorCodes errorCode = ParseReplyHeader(ReceiveBuffer, replyLength, ref parsedBytes, ref protocolVersion);
            if(errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Check the community
            string receivedCommunity = String.Empty;
            errorCode = ParseCommunity(ReceiveBuffer, replyLength, ref parsedBytes, ref receivedCommunity);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            string jobCommunity = ((SNMPCommJob)list[0]).snmpCommunity;
            if(!receivedCommunity.Equals(jobCommunity, StringComparison.Ordinal))
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorWrongCommunity, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Check the message type. It should be 0xa2 = get-response
            if (replyLength < (parsedBytes + 1))
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            if(ReceiveBuffer[parsedBytes++] != 0xa2)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorWrongReplyType, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Check the reply ID
            uint replyID = 0;
            errorCode = ParseReplyID(ReceiveBuffer, replyLength, ref parsedBytes, ref replyID);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            if(replyID != snmpLastRequestID)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorWrongReplyID, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Parse the error-status and the error-index
            int errorStatus = 0;
            int errorIndex = 0;
            errorCode = ParseErrorStatus(ReceiveBuffer, replyLength, ref parsedBytes, ref errorStatus, ref errorIndex);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            // Check the result and parse data for each job
            int currentJobIndex = 0;
            bool parsingError = false;
            foreach (SNMPCommJob j in list)
            {
                currentJobIndex++;
                // Error
                if((errorStatus != 0) && (currentJobIndex >= errorIndex))
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorNonZeroErrorStatus + errorStatus, Job = j };
                    OnJobExecuted(eJob);
                    if (j.WriteExecuted)
                    {
                        foreach (var tg in j.TagsListOnWriting)
                        {
                            tg.SetReadValue(tg.Value.Value);
                        }
                        j.TagsListOnWriting.Clear();
                        j.WriteExecuted = false;
                    }
                }

                // Parsing error occurred
                else if(parsingError == true)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                    OnJobExecuted(eJob);
                    if (j.WriteExecuted)
                    {
                        j.WriteExecuted = false;
                    }
                }

                // OK
                else
                {
                    // Output
                    if (j.WriteExecuted)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };
                        OnJobExecuted(eJob);
                        j.WriteExecuted = false;
                    }

                    // Input
                    else
                    {
                        // Check the length of the job data section
                        if(replyLength < (parsedBytes + 1))
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                            OnJobExecuted(eJob);
                            parsingError = true;
                        }
                        parsedBytes++;

                        //{
                        //    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        //    String DbgTxt = String.Format("SNMP DBG - {0} parsedBytes (1) = {1}", curTimeTxt, parsedBytes);
                        //    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        //}

                        int lengthSize = 0;
                        int lengthValue = SNMPProtocol.ASN1DecodifyLength(ReceiveBuffer, replyLength, parsedBytes, ref lengthSize);

                        //{
                        //    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        //    String DbgTxt = String.Format("SNMP DBG - {0} lengthSize = {1} lengthValue = {2}", curTimeTxt, lengthSize, lengthValue);
                        //    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        //}

                        if ((lengthValue == 0) || (replyLength < (lengthValue + parsedBytes + lengthSize)))
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                            OnJobExecuted(eJob);
                            parsingError = true;
                        }
                        else
                        {
                            parsedBytes += lengthSize;

                            //{
                            //    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            //    String DbgTxt = String.Format("SNMP DBG - {0} parsedBytes (2) = {1}", curTimeTxt, parsedBytes);
                            //    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                            //}

                            // Copy data from ReceiveBuffer
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            byte[] Answer;
                            Answer = new byte[lengthValue];
                            ReceiveBuffer.CopyTo(parsedBytes, Answer, 0, lengthValue);
                            eJob.Values = Answer;
                            eJob.Job = j;
                            eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                            OnJobExecuted(eJob);

                            parsedBytes += lengthValue;

                            //{
                            //    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            //    String DbgTxt = String.Format("SNMP DBG - {0} parsedBytes (3) = {1}", curTimeTxt, parsedBytes);
                            //    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                            //}

                        }
                    }
                }
            }

            LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            ReceiveBuffer.Clear();
            Flush();

            return true;
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
                SetStateCommandVariableBit(((!returnValue) || InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }
        }

        #endregion

        #region Methods
        public void AddToMapOIDJobs(string oidString, SNMPCommJob job)
        {
            lock (lockMapOIDJobsObject)
            {
                if (!mapOIDJobs.ContainsKey(oidString))
                {
                    List<SNMPCommJob> ListJobs = new List<SNMPCommJob>();
                    ListJobs.Add(job);
                    mapOIDJobs[oidString] = ListJobs;
                    String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    String dbgMsg = String.Format("SNMP DBG - {0} AddToMapOIDJobs add key {1}",
                                                  dbgTime, oidString);
                    System.Diagnostics.Trace.TraceInformation(dbgMsg);
                }
                else if (!mapOIDJobs[oidString].Contains(job))
                {
                    mapOIDJobs[oidString].Add(job);
                    String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    String dbgMsg = String.Format("SNMP DBG - {0} AddToMapOIDJobs add j to key {1}",
                                                  dbgTime, oidString);
                    System.Diagnostics.Trace.TraceInformation(dbgMsg);
                }
            }
        }

        public bool GetJobsFromMapOIDJobs(string oidString, ref List<SNMPCommJob> jobList)
        {
            // Returned value
            bool joblistFound = false;

            lock (lockMapOIDJobsObject)
            {
                if (mapOIDJobs.ContainsKey(oidString) && (mapOIDJobs[oidString] != null) && (mapOIDJobs[oidString].Count > 0))
                {
                    joblistFound = true;
                    jobList.AddRange(mapOIDJobs[oidString]);
                }
            }

            return (joblistFound);
        }

        public void AddTrapMessage(ReceivedPacket packet)
        {
            lock(lockTrapListObject)
            {
                trapList.Add(packet);
            }
        }

        private void PassTrapDataToJobs(byte[] receivedData, int dataLength, string receivedCommunity)
        {
            // Check the length of the received answer
            int dataSize = receivedData.GetLength(0);
            if (dataSize <= 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 1", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }

            // Parse the OID  
            string parsedOID = String.Empty;
            int parsedBytes = SNMPProtocol.ASN1DecodifyOID(receivedData, dataSize, 0, ref parsedOID);
            if ((parsedBytes == 0) || String.IsNullOrEmpty(parsedOID))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 2", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }

            // Get the list of jobs corresponding to the parsed OID
            List<SNMPCommJob> jobList = new List<SNMPCommJob>();
            if(!GetJobsFromMapOIDJobs(parsedOID, ref jobList))
            {
                String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs warning 1", curTimeTxt);
                System.Diagnostics.Trace.TraceInformation(DbgTxt);
                return;
            }

            // Check the community
            string jobCommunity = ((SNMPCommJob)jobList[0]).snmpCommunity;
            if (!receivedCommunity.Equals(jobCommunity, StringComparison.Ordinal))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs warning 3", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }

            // Get the data type
            if (dataSize < (parsedBytes + 1))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 3", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }
            SNMPDATATYPE dataType = SNMPDATATYPE.Integer;
            switch (receivedData[parsedBytes])
            {
                case 0x02:
                    dataType = SNMPDATATYPE.Integer;
                    break;

                case 0x04:
                    dataType = SNMPDATATYPE.OctetString;
                    break;

                case 0x40:
                    dataType = SNMPDATATYPE.IpAddress;
                    break;

                case 0x41:
                    dataType = SNMPDATATYPE.Counter32;
                    break;

                case 0x42:
                    dataType = SNMPDATATYPE.Gauge32;
                    break;

                case 0x43:
                    dataType = SNMPDATATYPE.TimeTicks;
                    break;

                case 0x47:
                    dataType = SNMPDATATYPE.Unsigned32;
                    break;

                default:
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 4", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        return;
                    }
            }

            // Check the decoded data length
            int decodedDataSize = SNMPProtocol.GetDecodedDataSize(receivedData, dataSize, parsedBytes);
            if (decodedDataSize == 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 5", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }

            // Parse data
            byte[] jobData = new byte[decodedDataSize];
            if (SNMPProtocol.ParseJobData(receivedData, dataSize, parsedBytes, ref jobData) == 0)
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs error 6", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return;
            }

            // Pass data to the jobs
            foreach(var job in jobList)
            {
                // Check the compatibility of the data type
                if (!SNMPProtocol.CompatibleDataTypes(dataType, job.snmpDataType))
                {

                    {
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs warning 4", curTimeTxt);
                        System.Diagnostics.Trace.TraceInformation(DbgTxt);
                    }

                    continue;
                }

                // Copy Data and pass them to the job
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = job;
                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                eJob.Values = receivedData;
                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} PassTrapDataToJobs trap message received for OID {1}",
                                                  curTimeTxt, parsedOID);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }
                OnJobExecuted(eJob);
            }
        }

        private bool ParseTrapMessageData(byte[] receivedMessage, int messageLength, ref int parsedBytes, string receivedCommunity)
        {
            // Check the length of the job data section
            if (messageLength < (parsedBytes + 1))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} ParseTrapMessageData error 1", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (false);
            }
            parsedBytes++;
            int lengthSize = 0;
            int lengthValue = SNMPProtocol.ASN1DecodifyLength(receivedMessage, messageLength, parsedBytes, ref lengthSize);
            if ((lengthValue == 0) || (messageLength < (lengthValue + parsedBytes + lengthSize)))
            {

                {
                    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgTxt = String.Format("SNMP DBG - {0} ParseTrapMessageData error 2", curTimeTxt);
                    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                }

                return (false);
            }
            parsedBytes += lengthSize;

            // Copy the part of the message with the data of a single object
            byte[] receivedData = new byte[lengthValue];
            Array.Copy(receivedMessage, parsedBytes, receivedData, 0, lengthValue);

            // Pass the data to the corresponding jobs (if any)
            PassTrapDataToJobs(receivedData, lengthValue, receivedCommunity);

            parsedBytes += lengthValue;

            return (true);
        }

        private void ManageTrapMessages()
        {
            // Any trap message received?
            List<ReceivedPacket> receivedTrapMessages = new List<ReceivedPacket>();
            lock (lockTrapListObject)
            {
                if(trapList.Count > 0)
                {
                    // Make a local copy of the list of traps and clear it 
                    receivedTrapMessages.AddRange(trapList);
                    // Clear the list of traps 
                    trapList.Clear();
                }
            }

            if(receivedTrapMessages.Count > 0)
            {
                foreach(var trapMsg in receivedTrapMessages)
                {
                    // Parse the message
                    byte[] receivedMessage = trapMsg.Data;
                    int messageLength = receivedMessage.GetLength(0);

                    {
                        String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages message length = {1}",
                                                      curTimeTxt, messageLength);
                        System.Diagnostics.Trace.TraceInformation(DbgTxt);
                    }

                    if (messageLength <= 0)
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages error 1", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }

                    // Check the protocol Version: it should be SNMPv2c
                    int parsedBytes = 0;
                    SNMPVERSION protocolVersion = SNMPVERSION.SNMPv1;
                    DriverErrorCodes errorCode = ParseReplyHeader(receivedMessage, messageLength, ref parsedBytes, ref protocolVersion);
                    if ((errorCode != DriverErrorCodes.ErrorNoError) || (protocolVersion != SNMPVERSION.SNMPv2c))
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages warning 1", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }

                    // Get the community
                    string receivedCommunity = String.Empty;
                    errorCode = ParseCommunity(receivedMessage, messageLength, ref parsedBytes, ref receivedCommunity);
                    if (errorCode != DriverErrorCodes.ErrorNoError)
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages error 2", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }

                    // Check the message type. It should be 0xa7 = snmpV2-trap or 0xa6 = inform-request
                    if (messageLength < (parsedBytes + 1))
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages error 3, {1}, {2}",
                                                          curTimeTxt, messageLength, parsedBytes);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }
                    if ((receivedMessage[parsedBytes] != 0xa7) && (receivedMessage[parsedBytes] != 0xa6))
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages warning 2", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }
                    parsedBytes++;

                    // Get the reply ID
                    uint replyID = 0;
                    errorCode = ParseReplyID(receivedMessage, messageLength, ref parsedBytes, ref replyID);
                    if (errorCode != DriverErrorCodes.ErrorNoError)
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages error 3", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }

                    // Parse the error-status and the error-index
                    int errorStatus = 0;
                    int errorIndex = 0;
                    errorCode = ParseErrorStatus(receivedMessage, messageLength, ref parsedBytes, ref errorStatus, ref errorIndex);
                    if ((errorCode != DriverErrorCodes.ErrorNoError) || (errorStatus != 0))
                    {

                        {
                            String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                            String DbgTxt = String.Format("SNMP DBG - {0} ManageTrapMessages error 4", curTimeTxt);
                            System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        }

                        continue;
                    }

                    // Parse the data and pass them to the corresponding jobs
                    bool parseTrapData = true;
                    while (parseTrapData == true)
                    {
                        parseTrapData = ParseTrapMessageData(receivedMessage, messageLength, ref parsedBytes, receivedCommunity);
                    }
                }
            }
        }

        protected void GetNextPendingList(ref List<SNMPCommJob> list)
        {
            bool write = false;
            LinkType jLink = LinkType.InputOutput;
            string station = string.Empty;
            bool first = true;
            string community = string.Empty;
            uint aggregatedRequestsCounter = 0;

            uint ReqDim = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
            uint ResponseLength = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)

            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return;

                while (!queue.IsEmpty)
                {
                    SNMPCommJob j = queue.ElementAt(0) as SNMPCommJob;

                    if (j != null && j.TagsListOnWriting.Count == 0)
                    {
                        if (first)
                        {
                            write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            community = j.snmpCommunity;
                            uint communityLength = j.GetCodifiedCommunityLength();
                            ReqDim += communityLength; // Community
                            ReqDim += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                            uint requestIDLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)snmpRequestID);
                            ReqDim += requestIDLength; // Request ID
                            ReqDim += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)
                            if (!write)
                            {
                                ResponseLength += communityLength; // Community
                                ResponseLength += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                                ResponseLength += requestIDLength; // Request ID
                                ResponseLength += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)
                            }
                            jLink = j.Type;
                            first = false;
                        }
                        bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if ((j.Station != null) &&
                                (j.Station.Name == station) &&
                                (j.snmpCommunity == community) &&
                                jwrite)
                            {
                                // In the case Unconditional Output, if necessary, fill the TagListToWrite of the Job
                                if (j.Type == LinkType.UnconditionalOutput)
                                {
                                    if (j.TagsListToWrite.Count == 0)
                                    {
                                        j.TagsListToWrite.AddRange(j.TagsList);
                                    }
                                }

                                // Chek the number of aggregated requests
                                if ((aggregatedRequestsCounter + 1) > snmpMaxNumberOfAggregatedRequests)
                                {
                                    break;
                                }
                                // Check the length of the request
                                uint jobRequestLength = j.GetEstimatedWriteRequestLength();
                                if ((ReqDim + 1 + jobRequestLength) > SNMPCommJob.MAX_SNMP_PACKET)
                                {
                                    break;
                                }
                                aggregatedRequestsCounter++;
                                ReqDim += jobRequestLength + 1;
                                list.Add(j);
                            }
                        }
                        else
                        {
                            if ((j.Station != null) &&
                                (j.Station.Name == station) &&
                                (j.snmpCommunity == community) &&
                                !jwrite &&
                                (j.Type != LinkType.ExceptionOutput))
                            {
                                // Chek the number of aggregated requests
                                if ((aggregatedRequestsCounter + 1) > snmpMaxNumberOfAggregatedRequests)
                                {
                                    break;
                                }
                                // Check the length of the replay
                                uint jobResponseLength = j.GetReadResponseLength();
                                if ((ResponseLength + 1 + jobResponseLength) > SNMPCommJob.MAX_SNMP_PACKET)
                                {
                                    break;
                                }
                                ResponseLength += j.GetReadResponseLength() + 1;

                                // Check the length of the request
                                uint jobRequestLength = j.GetReadRequestLength();
                                if ((ReqDim + jobRequestLength + 1) > SNMPCommJob.MAX_SNMP_PACKET)
                                {
                                    break;
                                }
                                aggregatedRequestsCounter++;
                                ReqDim += jobRequestLength + 1;
                                list.Add(j);
                            }
                        }

                    }
                    CommJob dequeuedJob = null;
                    queue.TryDequeue(out dequeuedJob);
                }
            }
        }

        protected void ExecuteJobList(List<SNMPCommJob> list)
        {
            // Prepare the request
            snmpLastRequestID = snmpRequestID;
            uint byteNumber = SNMPProtocol.PrepareRequest(list, ref requestBuffer, snmpRequestID++, snmpVersion);
            if (byteNumber == 0)
            {
                return;
            }

            ReceiveBuffer.Clear();
            Flush();

            if (!DeviceWrite(requestBuffer, byteNumber))
            {
                return;
            }

            DateTime startExecutionTime = DateTime.UtcNow;
            foreach (SNMPCommJob j in list)
            {
                base.ExecuteJob(j);
                j.LastExecutionTime = startExecutionTime;
                j.StartExecutionTime = startExecutionTime;
            }

            BeginDeviceRead(0);

            return;
        }

        public void Flush()
        {
            if(GetBytesToRead()!=0)
            {
                byte[] Buffer = new byte[0];
                DeviceRead(Buffer,0);
            }
        }

        public bool InErrorState()
        {
            bool InErrorState = false;
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                if (station.InErrorState && station.GetChannel() == this)
                {
                    InErrorState = true;
                    break;
                }
            }
            return InErrorState;
        }

        #endregion

        #region Properties

        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        private SNMPVERSION _snmpVersion;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        ///
        /// <value> SNMPv1 or SNMPv2c. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SNMPVERSION snmpVersion
        {
            get { return _snmpVersion; }
            set
            {
                _snmpVersion = value;
            }
        }

        /// <summary>   Gets or sets the value of the maximum number of variables that are aggregated in a single request. </summary>
        private uint _snmpMaxNumberOfAggregatedRequests;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the value of the maximum number of variables that are aggregated in a single request. </summary>
        ///
        /// <default value> 1. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint snmpMaxNumberOfAggregatedRequests
        {
            get
            {
                return _snmpMaxNumberOfAggregatedRequests;
            }
            set
            {
                snmpMaxNumberOfAggregatedRequests = value;
            }
        }

        #endregion
    }
}
