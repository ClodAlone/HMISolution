using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SNMP
{
    public class SNMPChannel : UdpChannelList
    {
        public const uint MAX_REQUEST_NUMBER = 500;

        #region Constructors

        public SNMPDriver snmpDriver;

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SNMPChannel(CommunicationDriver commdriver, SNMPChannelSettings settings)
            : base(commdriver, settings)
        {
            _snmpVersion = settings.snmpVersion;
            _snmpMaxNumberOfAggregatedRequests = settings.snmpMaxNumberOfAggregatedRequests;
            if (_snmpMaxNumberOfAggregatedRequests == 0)
            {
                _snmpMaxNumberOfAggregatedRequests = 1;
            }
            snmpDriver = commdriver as SNMPDriver;
            _UDPManager = new UDPManagerSNMP(this, settings);
        }

        #endregion

        #region Data Members

        uint snmpRequestID = 0;
        uint snmpLastRequestID = 0;
        byte[] requestBuffer = new byte[SNMPCommJob.MAX_SNMP_PACKET];
        List<ReceivedPacket> trapList = new List<ReceivedPacket>();
        protected Object lockTrapListObject = new Object();
        Dictionary<string, List<SNMPCommJob>> mapOIDJobs = new Dictionary<string, List<SNMPCommJob>>();
        protected Object lockMapOIDJobsObject = new Object();
        UDPManagerSNMP _UDPManager;


        #endregion

        #region Override Methods

        private void ProcessDataFormatErrors(List<CommJob> list)
        {
            //List<SNMPCommJob> localListJobPendingCopy = new List<SNMPCommJob>();
            foreach (SNMPCommJob job in list)
            {
                switch (job.badDataFormat)
                {
                    //case SNMPCommJob.DataFormatErrors.NoError:
                    //    localListJobPendingCopy.Add(job);
                    //    break;

                    case SNMPCommJob.DataFormatErrors.DiscardTooMuchJobs:
                    case SNMPCommJob.DataFormatErrors.DiscardIOEOUnchangedValue:
                        RemovePendingJob(job);
                        // ListJobPending.Removed(..) will be execute below
                        break;

                    case SNMPCommJob.DataFormatErrors.GenericError:

                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = job;
                        eJob.ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                        OnJobExecuted(eJob);

                        if (job.GetTagListToWriteCount() > 0)//error occurred prior to the write, nobody empty this list!
                        {
                            foreach (var tg in job.GetTagListToWrite())
                            {
                                tg.SetReadValue(tg.Value.Value);
                            }
                            job.ClearTagListToWrite();
                        }
                        if (job.GetTagListOnWritingCount() > 0)
                        {
                            foreach (var tg in job.GetTagListOnWriting())
                            {
                                tg.SetReadValue(tg.Value.Value);
                            }
                            job.ClearTagListOnWriting();
                        }

                        //job.WriteExecuted = false;
                        break;
                }

                job.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;
            }

            list.RemoveAll(j => !j.IsPending);
        }

        //public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        //{
        //    int jobIndex = 0;


        //    if (exList.Count > 0)
        //    {
        //        jobList.InsertRange(0, exList[exList.Count - 1]);
        //        exList.RemoveAt(exList.Count - 1);
        //    }

        //    while (jobIndex < jobList.Count)
        //    {
        //        //append a new list to the end of exList
        //        List<CommJob> list = new List<CommJob>();
        //        bool first = true;
        //        bool write = false;
        //        string station = string.Empty;
        //        uint ReqDim = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
        //        uint ResponseLength = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
        //        while (jobIndex < jobList.Count)
        //        {
        //            SNMPCommJob j = jobList.ElementAt(jobIndex) as SNMPCommJob;
        //            if (j != null && j.TagsListOnWriting.Count == 0)
        //            {
        //                if (first)
        //                {
        //                    uint communityLength = j.GetCodifiedCommunityLength();
        //                    ReqDim += communityLength; // Community
        //                    ReqDim += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
        //                    uint requestIDLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)snmpRequestID);
        //                    ReqDim += requestIDLength; // Request ID
        //                    ReqDim += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)

        //                    //if (lastNotFilled && exList[nCount].Count > 0)
        //                    //{
        //                    //    //take write, station and jLink from last job in partial list
        //                    //    write = (exList[nCount][0].TagsListToWrite.Count > 0 &&
        //                    //            (exList[nCount][0].Type == LinkType.ExceptionOutput || exList[nCount][0].Type == LinkType.InputOutput)) ||
        //                    //            exList[nCount][0].Type == LinkType.UnconditionalOutput;
        //                    //    station = exList[nCount][0].Station.Name;
        //                    //    jLink = exList[nCount][0].Type;
        //                    //}
        //                    //else
        //                    //{
        //                    write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
        //                    station = j.Station.Name;
        //                    //jLink = j.Type;
        //                    //}

        //                    if (!write)
        //                    {
        //                        ResponseLength += communityLength; // Community
        //                        ResponseLength += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
        //                        ResponseLength += requestIDLength; // Request ID
        //                        ResponseLength += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)
        //                    }

        //                    first = false;
        //                }
        //                bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
        //                if (write)
        //                {
        //                    if (j.Station != null && j.Station.Name == station && jwrite)
        //                    {
        //                        // aggregate only 1 job at time for write operation
        //                        if (list.Count > snmpMaxNumberOfAggregatedRequests)
        //                            break;

        //                        list.Add(j);
        //                        jobList.RemoveAt(jobIndex);
        //                        jobIndex--;
        //                        //Aggregation limit 
        //                        // Check the length of the request
        //                        uint jobRequestLength = j.GetEstimatedWriteRequestLength();
        //                        if ((ReqDim + 1 + jobRequestLength) > SNMPCommJob.MAX_SNMP_PACKET)
        //                        {
        //                            break;
        //                        }

        //                        ReqDim += jobRequestLength + 1;
        //                    }
        //                }
        //                else
        //                {
        //                    if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
        //                    {
        //                        //Aggregation limit 
        //                        if (list.Count > snmpMaxNumberOfAggregatedRequests)
        //                            break;

        //                        list.Add(j);
        //                        jobList.RemoveAt(jobIndex);
        //                        jobIndex--;

        //                        // Check the length of the replay
        //                        uint jobResponseLength = j.GetReadResponseLength();
        //                        if ((ResponseLength + 1 + jobResponseLength) > SNMPCommJob.MAX_SNMP_PACKET)
        //                        {
        //                            break;
        //                        }
        //                        ResponseLength += j.GetReadResponseLength() + 1;

        //                        // Check the length of the request
        //                        uint jobRequestLength = j.GetReadRequestLength();
        //                        if ((ReqDim + jobRequestLength + 1) > SNMPCommJob.MAX_SNMP_PACKET)
        //                        {
        //                            break;
        //                        }
        //                        ReqDim += jobRequestLength + 1;
        //                    }
        //                }
        //            }
        //            jobIndex++;
        //        }
        //        if (list.Count > 0)
        //        {
        //            //filled the list, a new one is required...                    
        //            exList.Add(list);                  
        //        }
        //    }
        //}

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return (jobList.Count == snmpMaxNumberOfAggregatedRequests);
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {            
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                uint ReqDim = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
                uint ResponseLength = 7; // 0x30 + Length of the remaining part of the message (3 bytes) + SNMP Version (3 bytes)
                List<CommJob> list = new List<CommJob>();

                while (jobIndex < jobList.Count)
                {
                    SNMPCommJob j = jobList.ElementAt(jobIndex) as SNMPCommJob;
                    if (j != null)
                    {
                        if (first)
                        {
                            uint communityLength = j.GetCodifiedCommunityLength();
                            ReqDim += communityLength; // Community
                            ReqDim += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                            uint requestIDLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)snmpRequestID);
                            ReqDim += requestIDLength; // Request ID
                            ReqDim += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)

                            write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;

                            if (!write)
                            {
                                ResponseLength += communityLength; // Community
                                ResponseLength += 4;  // MessageType (1 byte) + Length of the remaining part of the message (3 bytes)
                                ResponseLength += requestIDLength; // Request ID
                                ResponseLength += 10; // Error Status (3 bytes) + Error Index (3 bytes) + 0x30 + Length of the remaining part of the message (3 bytes)
                            }

                            first = false;
                        }
                        bool jwrite = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                //Aggregation limit 
                                // Check the length of the request
                                uint jobRequestLength = j.GetEstimatedWriteRequestLength();
                                if ((ReqDim + 1 + jobRequestLength) > SNMPCommJob.MAX_SNMP_PACKET)
                                    break;

                                //Chek the number of aggregated requests
                                if ((list.Count + 1) > snmpMaxNumberOfAggregatedRequests)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;

                                ReqDim += jobRequestLength + 1;
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {
                                //Aggregation limit 
                                if ((list.Count + 1) > snmpMaxNumberOfAggregatedRequests)
                                    break;

                                // Check the length of the replay
                                uint jobResponseLength = j.GetReadResponseLength();
                                if ((ResponseLength + 1 + jobResponseLength) > SNMPCommJob.MAX_SNMP_PACKET)
                                    break;

                                // Check the length of the request
                                uint jobRequestLength = j.GetReadRequestLength();
                                if ((ReqDim + jobRequestLength + 1) > SNMPCommJob.MAX_SNMP_PACKET)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;

                                ResponseLength += j.GetReadResponseLength() + 1;
                                ReqDim += jobRequestLength + 1;
                            }
                        }
                    }
                    jobIndex++;                    
                }

                if (list.Count > 0)
                {
                    //filled the list, a new one is required...                    
                    exList.Add(list);                    
                }
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

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            bool fromWrite = ((SNMPCommJob)list[0]).WriteExecuted;
            
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                lock(lockThreadObject)
                    ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            // Check the common part of the message

            // Check the reply header
            int replyLength = receiveBuffer.Count;
            if(replyLength <= 0)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorMalformedReply, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }
            int parsedBytes = 0;
            SNMPVERSION protocolVersion = SNMPVERSION.SNMPv1;
            DriverErrorCodes errorCode = ParseReplyHeader(receiveBuffer, replyLength, ref parsedBytes, ref protocolVersion);
            if(errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }

            // Check the community
            string receivedCommunity = String.Empty;
            errorCode = ParseCommunity(receiveBuffer, replyLength, ref parsedBytes, ref receivedCommunity);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
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

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;                
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

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }
            if(receiveBuffer[parsedBytes++] != 0xa2)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)SNMPErrorCodes.SNMPErrorWrongReplyType, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }

            // Check the reply ID
            uint replyID = 0;
            errorCode = ParseReplyID(receiveBuffer, replyLength, ref parsedBytes, ref replyID);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
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

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }

            // Parse the error-status and the error-index
            int errorStatus = 0;
            int errorIndex = 0;
            errorCode = ParseErrorStatus(receiveBuffer, replyLength, ref parsedBytes, ref errorStatus, ref errorIndex);
            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = j };
                    OnJobExecuted(eJob);
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
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
                        if (j.GetTagListOnWritingCount() > 0)
                        {
                            foreach (var tg in j.GetTagListOnWriting())
                            {
                                tg.SetReadValue(tg.Value.Value);
                            }
                            j.ClearTagListOnWriting();
                        }
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
                        int lengthValue = SNMPProtocol.ASN1DecodifyLength(receiveBuffer, replyLength, parsedBytes, ref lengthSize);

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

                            // Copy data from receiveBuffer
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            byte[] Answer;
                            Answer = new byte[lengthValue];
                            receiveBuffer.CopyTo(parsedBytes, Answer, 0, lengthValue);
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

            //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
            //Flush();

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
            SetStateCommandVariableBit(!returnValue || IsDisconnected(), (UInt16)ChannelVariableBits.ChannelUnconnected);
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
            return _UDPManager.DeviceClose();
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
            return _UDPManager.DeviceRead(Buffer, Count);
        }

        public override bool BeginDeviceRead(int Count)
        {
            return _UDPManager.BeginDeviceRead(Count);
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
        #endregion

        #region Methods
        public List<Station> GetStations()
        {
            return CommDriver.GetChannelStations(this);
        }

        private bool IsDisconnected()
        {
            // set channel in error only if all station are in error
            return (GetStations().Count(s => ((SNMPStation)s).Disconnected) > 0);
        }

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
                ManageTrapMessages();
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

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            //List<SNMPCommJob> list = exjoblist.OfType<SNMPCommJob>().ToList();
            // Prepare the request
            snmpLastRequestID = snmpRequestID;
            uint byteNumber = PrepareRequest(exjoblist, ref requestBuffer, snmpRequestID++, snmpVersion);
            if (byteNumber == 0 || exjoblist.Count == 0)
                return false;
            
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();

            foreach (CommJob j in exjoblist)
                base.ExecuteJob(j);

            if (!DeviceWrite(requestBuffer, byteNumber))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            BeginDeviceRead(0);

            return true;
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

        private uint PrepareRequest(List<CommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            if (jList[0].Type == LinkType.Input || (jList[0].Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput && jList[0].GetTagListOnWritingCount() == 0))
                return PrepareReadRequest(jList, ref buffer, snmpRequestID, snmpVersion);
            else
                return PrepareWriteRequest(jList, ref buffer, snmpRequestID, snmpVersion);
        }


        public uint PrepareReadRequest(List<CommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            // Returned value
            uint requestTotalLength = 0;

            // Calculate the total length of the job requests and the length of the codified community string 
            uint tempJobsRequestLength = 0;
            uint tempCommunityLength = 0;
            bool firstIteration = true;
            foreach (SNMPCommJob j in jList)
            {
                j.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;
                if (firstIteration == true)
                {
                    tempCommunityLength = j.GetCodifiedCommunityLength();
                    firstIteration = false;
                }
                tempJobsRequestLength += j.GetReadRequestLength();
            }

            ProcessDataFormatErrors(jList);

            if (tempJobsRequestLength == 0)
            {
                return (0);
            }

            // Get the length of the codified request ID
            uint codifiedRequestIdLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)snmpRequestID);

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + tempJobsRequestLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            buffer[requestTotalLength++] = 0x30;
            // Length of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength02 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength02;
            // Protocol version
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            if (snmpVersion == SNMPVERSION.SNMPv1)
            {
                buffer[requestTotalLength++] = 0;
            }
            else
            {
                buffer[requestTotalLength++] = 0x01;
            }
            // Community
            CommJob job = jList[0];
            byte[] communityBuffer = new byte[tempCommunityLength];
            ((SNMPCommJob)job).GetCodifiedCommunity(ref communityBuffer);
            uint i = 0;
            for (i = 0; i < tempCommunityLength; i++)
            {
                buffer[requestTotalLength++] = communityBuffer[i];
            }
            // Message type (get-request)
            buffer[requestTotalLength++] = 0xa0;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength01 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength01;
            // Request-ID
            byte[] codifiedRequestID = new byte[codifiedRequestIdLength];
            SNMPProtocol.ASN1CodifyInt((int)(snmpRequestID++), ref codifiedRequestID);
            for (i = 0; i < codifiedRequestIdLength; i++)
            {
                buffer[requestTotalLength++] = codifiedRequestID[i];
            }
            // Error status
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Error index
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Job requests
            buffer[requestTotalLength++] = 0x30;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempJobsRequestLength >> 8);
            buffer[requestTotalLength++] = (byte)tempJobsRequestLength;
            foreach (SNMPCommJob jb in jList)
            {
                uint jobCodifiedOidLength = jb.GetCodifiedOIDLength();
                if (jobCodifiedOidLength > 0)
                {
                    // Job request
                    buffer[requestTotalLength++] = 0x30;
                    // Request length
                    buffer[requestTotalLength++] = 0x82;
                    uint jobRequestLength = jobCodifiedOidLength + 2;
                    buffer[requestTotalLength++] = (byte)(jobRequestLength >> 8);
                    buffer[requestTotalLength++] = (byte)jobRequestLength;
                    // Codified OID
                    byte[] codifiedOid = new byte[jobCodifiedOidLength];
                    jb.GetCodifiedOID(ref codifiedOid);
                    for (i = 0; i < jobCodifiedOidLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedOid[i];
                    }
                    // Null value
                    buffer[requestTotalLength++] = 0x05;
                    buffer[requestTotalLength++] = 0;
                }
            }

            return (requestTotalLength);
        }

        public uint PrepareWriteRequest(List<CommJob> jList, ref byte[] buffer, uint snmpRequestID, SNMPVERSION snmpVersion)
        {
            // Returned value
            uint requestTotalLength = 0;

            // Calculate the total length of the job requests and the length of the codified community string 
            uint tempJobsRequestLength = 0;
            uint tempJobRequestLength = 0;
            uint tempCommunityLength = 0;
            bool firstIteration = true;

            for (int jobIndex = 0; jobIndex < jList.Count; jobIndex++)
            {
                SNMPCommJob j = jList[jobIndex] as SNMPCommJob;
                // write only 1st tag; next, are discard and will be write later
                if (jobIndex == 0)
                {
                    j.badDataFormat = SNMPCommJob.DataFormatErrors.NoError;
                    if (firstIteration == true)
                    {
                        tempCommunityLength = j.GetCodifiedCommunityLength();
                        firstIteration = false;
                    }

                    tempJobsRequestLength = j.GetWriteRequestLength();

                    tempJobsRequestLength += (uint)tempJobRequestLength;
                }
                else
                {
                    j.badDataFormat = SNMPCommJob.DataFormatErrors.DiscardTooMuchJobs;
                }
            }

            ProcessDataFormatErrors(jList);

            if (tempJobsRequestLength == 0)
            {
                return (0);
            }

            // Get the length of the codified request ID
            uint codifiedRequestIdLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)snmpRequestID);

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + tempJobsRequestLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            buffer[requestTotalLength++] = 0x30;
            // Length of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength02 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength02;
            // Protocol version
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            if (snmpVersion == SNMPVERSION.SNMPv1)
            {
                buffer[requestTotalLength++] = 0;
            }
            else
            {
                buffer[requestTotalLength++] = 0x01;
            }
            // Community
            CommJob job = jList[0];
            byte[] communityBuffer = new byte[tempCommunityLength];
            ((SNMPCommJob)job).GetCodifiedCommunity(ref communityBuffer);
            uint i = 0;
            for (i = 0; i < tempCommunityLength; i++)
            {
                buffer[requestTotalLength++] = communityBuffer[i];
            }
            // Message type (set-request)
            buffer[requestTotalLength++] = 0xa3;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempPartialLength01 >> 8);
            buffer[requestTotalLength++] = (byte)tempPartialLength01;
            // Request-ID
            byte[] codifiedRequestID = new byte[codifiedRequestIdLength];
            SNMPProtocol.ASN1CodifyInt((int)(snmpRequestID++), ref codifiedRequestID);
            for (i = 0; i < codifiedRequestIdLength; i++)
            {
                buffer[requestTotalLength++] = codifiedRequestID[i];
            }
            // Error status
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Error index
            buffer[requestTotalLength++] = 0x02;
            buffer[requestTotalLength++] = 0x01;
            buffer[requestTotalLength++] = 0;
            // Job requests
            buffer[requestTotalLength++] = 0x30;
            // Length of the remaining part of the message
            buffer[requestTotalLength++] = 0x82;
            buffer[requestTotalLength++] = (byte)(tempJobsRequestLength >> 8);
            buffer[requestTotalLength++] = (byte)tempJobsRequestLength;
            foreach (SNMPCommJob jb in jList)
            {
                uint jobCodifiedOidLength = jb.GetCodifiedOIDLength();
                //jobDataLength = jb.GetTheDataLength();
                if ((jobCodifiedOidLength > 0) && (jb.WriteDataLength > 0))
                {
                    // Job request
                    buffer[requestTotalLength++] = 0x30;
                    // Request length
                    buffer[requestTotalLength++] = 0x82;
                    uint jobRequestLength = jobCodifiedOidLength + (uint)jb.WriteDataLength;
                    buffer[requestTotalLength++] = (byte)(jobRequestLength >> 8);
                    buffer[requestTotalLength++] = (byte)jobRequestLength;
                    // Codified OID
                    byte[] codifiedOid = new byte[jobCodifiedOidLength];
                    jb.GetCodifiedOID(ref codifiedOid);
                    for (i = 0; i < jobCodifiedOidLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedOid[i];
                    }
                    // Codified value
                    byte[] codifiedValue = new byte[jb.WriteDataLength];
                    jb.GetCodifiedData(ref codifiedValue);
                    for (i = 0; i < jb.WriteDataLength; i++)
                    {
                        buffer[requestTotalLength++] = codifiedValue[i];
                    }

                    jb.WriteExecuted = true;
                }
            }

            return (requestTotalLength);
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
            StopTimers();

            _UDPManager.Dispose();
            DeviceClose();
            base.Dispose();
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
