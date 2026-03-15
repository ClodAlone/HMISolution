using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Linq.Helpers;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using Opc.Ua;

namespace SNMP
{
    public partial class SNMPChannel : UdpChannelList
    {        
        #region Constructors        
        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public SNMPChannel(CommunicationDriver commdriver, SNMPChannelSettings settings)
            : base(commdriver, settings)
        {
            _snmpVersion = settings.snmpVersion;
            snmpDriver = commdriver as SNMPDriver;
            _UDPManager = new UDPManagerSNMP(this, settings);             
            SetSNMPMaxPacket(SNMPProtocol.GetMaxPacketSize());
        }

        #endregion

        #region Data Members
        int requestID = 0;
        int lastRequestID = 0;        
        Dictionary<string, List<SNMPCommJob>> mapOIDJobs = new Dictionary<string, List<SNMPCommJob>>();
        protected Object lockMapOIDJobsObject = new Object();
        UDPManagerSNMP _UDPManager;
        private uint snmpMaxPacket = SNMPProtocol.GetMaxPacketSize();
        private uint snmpMaxPacketAggregationLimit;
        #endregion

        #region Override Methods

        //public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        //{
        //    return (jobList.Count == snmpMaxNumberOfAggregatedRequests);
        //}

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {            
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                List<CommJob> list = new List<CommJob>();
                SNMPProtocol.ReadWriteListLimitateSize frameSize = new SNMPProtocol.ReadWriteListLimitateSize(snmpVersion, snmpMaxPacketAggregationLimit);
                bool process = true;

                while (jobIndex < jobList.Count)
                {
                    SNMPCommJob j = jobList.ElementAt(jobIndex) as SNMPCommJob;
                    if (j != null && !j.snmpTrapOnly)
                    {
                        if (first)
                        {
                            first = false;
                            write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;

                            frameSize.InitAggregation(j, write);
                        }
                        bool jwrite = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;

                        process = true;
                        // if the job has never been exchanged, exchange it individually (need especially for snmp v1)
                        if (j.FirstRequest)
                        {
                            process = false;
                            if (list.Count == 0)
                            {
                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                break;
                            }
                        }

                        if (process)
                        {
                            if (j.Station != null && j.Station.Name == station)
                            {
                                if (write && jwrite)
                                {
                                    if (frameSize.getWriteListLimitate(j))
                                    {
                                        list.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (!write && !jwrite)
                                    {
                                        if (frameSize.getReadListLimitate(j))
                                        {
                                            list.Add(j);
                                            jobList.RemoveAt(jobIndex);
                                            jobIndex--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    jobIndex++;                    
                }

                if (list.Count > 0)
                {
                    //filled the list, a new one is required...                    
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }


        private void RemoveMultipleFirstRequestJobs(List<CommJob> list)
        {
            // if there are multiple jobs that need to be requested individually, they are removed from the request
            List<CommJob> listFirstRequest = list.FindAll(j => ((SNMPCommJob)j).FirstRequest);
            if (listFirstRequest.Count > 1)
            {
                for (int i = 0; i < listFirstRequest.Count - 1; i++)
                {
                    if (list.Contains(listFirstRequest[i]))
                    {
                        list.Remove(listFirstRequest[i]);
                        RemovePendingJob(listFirstRequest[i]);
                    }
                }
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            RemoveMultipleFirstRequestJobs(list);
            if (list.Count == 0)
                return false;

            SNMPCommJob.SNMPState state = CheckSNMPJobsState(list);
            switch (state)
            {
                case SNMPCommJob.SNMPState.Discovery:
                    {
                        // only for v3
                        conn = PrepareDiscoveryRequestv3(list, out byte[] requestBuffer);
                        if (conn != DriverErrorCodes.ErrorNoError || list.Count == 0)
                            return false;

                        lock (lockThreadObject)
                            ReceiveBuffer.Clear();
                        Flush();

                        foreach (CommJob j in list)
                            base.ExecuteJob(j);

                        if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        BeginDeviceRead(0);
                    }
                    break;

                case SNMPCommJob.SNMPState.Authentication:
                    {
                        // only for v3
                        conn = PrepareAutenticationRequestv3(list, out byte[] requestBuffer);
                        if (conn != DriverErrorCodes.ErrorNoError || list.Count == 0)
                            return false;

                        lock (lockThreadObject)
                            ReceiveBuffer.Clear();
                        Flush();

                        foreach (CommJob j in list)
                            base.ExecuteJob(j);

                        if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        BeginDeviceRead(0);
                    }
                    break;

                case SNMPCommJob.SNMPState.Polling:
                    {                        
                        conn = PreparePollingRequest(list, out byte[] requestBuffer);
                        if (conn != DriverErrorCodes.ErrorNoError || list.Count == 0)
                            return false;

                        lock (lockThreadObject)
                            ReceiveBuffer.Clear();
                        Flush();

                        foreach (CommJob j in list)
                            base.ExecuteJob(j);

                        if (!DeviceWrite(requestBuffer, (uint)requestBuffer.Length))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        BeginDeviceRead(0);
                    }
                    break;
            }

            return true;
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            DateTime lastExecutionTime = DateTime.UtcNow;

            if (conn != (int)DriverErrorCodes.ErrorNoError)
            {
                foreach (SNMPCommJob job in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = job, LastExecutionTime = lastExecutionTime };
                    OnJobExecuted(eJob);
                }

                // reset jobs to initial state (necessary for Snmp v1)
                foreach (SNMPStation st in this.CommDriver.GetChannelStations(this))
                    st.ResetJobsToInitialState();
                
                if (this.snmpVersion == SNMPVERSION.SNMPv3)
                {
                    ResetSNMPMaxPacket();
                    foreach (SNMPStation st in this.CommDriver.GetChannelStations(this))
                        st.PutAllJobsInError(conn);
                }

                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
                //System.Diagnostics.Debug.WriteLine(string.Join(",", receiveBuffer.Select(b => b.ToString())));
            }

            SNMPCommJob.SNMPState state = CheckSNMPJobsState(list);
            switch (state)
            {
                case SNMPCommJob.SNMPState.Discovery:
                    {
                        DriverErrorCodes errorCode = ParseDiscoveryResponse(receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData);                        
                        if (errorCode == DriverErrorCodes.ErrorNoError)
                        {
                            SNMPStation st = list[0].Station as SNMPStation;
                            //vbs parse
                            SetSNMPMaxPacket(messageGeneralData.MaxPacket);
                            st.Usm.SetEngine(messageGeneralData.ParsedUsm);
                            st.Discovered = true;
                        }
                        else
                        {
                            ((SNMPStation)list[0].Station).PutAllJobsInError(errorCode);

                            foreach (SNMPCommJob job in list)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = job, LastExecutionTime = lastExecutionTime };
                                OnJobExecuted(eJob);
                            }
                            Flush();
                        }
                    }
                    break;

                case SNMPCommJob.SNMPState.Authentication:
                    {
                        DriverErrorCodes errorCode = ParseAuthenticationResponse(list, receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData);
                        if (errorCode == DriverErrorCodes.ErrorNoError)
                        {
                            SNMPStation st = list[0].Station as SNMPStation;
                            //vbs parse
                            st.Usm.SetEngine(messageGeneralData.ParsedUsm);
                            st.Authenticated = true;
                        }
                        else
                        {
                            ((SNMPStation)list[0].Station).PutAllJobsInError(errorCode);

                            foreach (SNMPCommJob job in list)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = job, LastExecutionTime = lastExecutionTime };
                                OnJobExecuted(eJob);
                            }
                            Flush();
                        }
                    }
                    break;

                case SNMPCommJob.SNMPState.Polling:
                    {
                        DriverErrorCodes errorCode = ParsePollingResponse(list, receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData);
                        if (messageGeneralData.snmpVersion == SNMPVERSION.SNMPv3 && messageGeneralData.ParsedUsm.HasValidEngineIdBootsTime())
                        {
                            SNMPStation st = list[0].Station as SNMPStation;
                            st.Usm.SetEngine(messageGeneralData.ParsedUsm);
                        }
                        
                        foreach (SNMPCommJob job in list)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.LastExecutionTime = lastExecutionTime;
                            eJob.ErrorCode = errorCode;
                            eJob.Job = job;

                            if (errorCode == DriverErrorCodes.ErrorNoError)
                            {
                                // Input
                                DriverErrorCodes error = job.CheckResponseGeneralData(messageGeneralData, out SNMPProtocol.SNMPVariableBinding variable);
                                if (error != DriverErrorCodes.ErrorNoError)
                                {
                                    eJob.ErrorCode = error;
                                }
                                else
                                {
                                    eJob.Values = variable.TagValue;
                                }
                            }
                            if (errorCode == DriverErrorCodes.ErrorNoError)
                                job.FirstRequest = false;
                            OnJobExecuted(eJob);
                        }

                        if (errorCode != DriverErrorCodes.ErrorNoError)
                            Flush();
                    }
                    break;
            }

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
            return _UDPManager.DeviceClose(bDisposed || CommDriver.bDisposed);
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
        public SNMPCommJob.SNMPState CheckSNMPJobsState(List<CommJob> list)
        {
            SNMPCommJob.SNMPState state = SNMPCommJob.SNMPState.None;
            switch (snmpVersion)
            {
                case SNMPVERSION.SNMPv1:
                case SNMPVERSION.SNMPv2c:
                    state = SNMPCommJob.SNMPState.Polling;
                    break;
                case SNMPVERSION.SNMPv3:
                    foreach (SNMPCommJob job in list)
                    {
                        if (job.SnmpState == SNMPCommJob.SNMPState.None)
                        {
                            job.SnmpState = SNMPCommJob.SNMPState.Discovery;                            
                        }

                        if (job.SnmpState == SNMPCommJob.SNMPState.Discovery)
                        {
                            if (((SNMPStation)job.Station).Discovered)
                            {
                                if (((SNMPStation)job.Station).Usm.SecurityLevel != SNMPProtocol.SecurityLevel.NoAuthNoPriv)
                                    job.SnmpState = SNMPCommJob.SNMPState.Authentication;
                                else
                                    job.SnmpState = SNMPCommJob.SNMPState.Polling;
                            }
                            else
                            {
                                // send request
                            }
                        }

                        if (job.SnmpState == SNMPCommJob.SNMPState.Authentication)
                        {
                            if (((SNMPStation)job.Station).Authenticated)
                            {                                
                                job.SnmpState = SNMPCommJob.SNMPState.Polling;
                            }
                            else
                            {
                                // send request
                            }
                        }

                        if (job.SnmpState == SNMPCommJob.SNMPState.Polling)
                        {
                            // do nothing here
                        }
                    }
                    state = SNMPProtocol.GetLowerJobState(list);
                    break;                    
            }            
            
            return state;
        }

        private DriverErrorCodes PreparePollingRequest(List<CommJob> jList, out byte[] buffer)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;
            buffer = null;

            switch (snmpVersion)
            {
                case SNMPVERSION.SNMPv1:
                case SNMPVERSION.SNMPv2c:
                    result = PrepareRequestv1v2c(jList, out buffer);
                    break;
                case SNMPVERSION.SNMPv3:
                    result = PrepareRequestv3(jList, out buffer);
                    break;
            }
                        
            return result;
        }

        private DriverErrorCodes PrepareRequestv1v2c(List<CommJob> jList, out byte[] buffer)
        {
            if (jList[0].Type == LinkType.Input || (jList[0].Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput && jList[0].GetTagListOnWritingCount() == 0))
                return PrepareReadRequestv1v2c(jList, out buffer );
            else
                return PrepareWriteRequestv1v2c(jList, out buffer);
        }

        public DriverErrorCodes PrepareReadRequestv1v2c(List<CommJob> jList, out byte[] buffer)
        {
            buffer = null;

            List<byte> resultBuffer = new List<byte>();
                        
            // Calculate the total length of the job requests and the length of the codified community string 
            uint tempJobsRequestLength = 0;
            uint tempCommunityLength = 0;
            bool firstIteration = true;

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
                    if (firstIteration)
                    {
                        tempCommunityLength = j.GetCodifiedCommunityLength();
                        firstIteration = false;
                    }
                    tempJobsRequestLength += j.GetReadRequestLength();
                }
                jobIndex++;
            }
            if (jList.Count == 0)
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;

            int requestid = GetNewRequestId();
            // Get the length of the codified request ID
            uint codifiedRequestIdLength = SNMPProtocol.ASN1GetCodifiedIntLength((int)requestid);

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + tempJobsRequestLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            resultBuffer.Add(SNMPProtocol.SEQUENCE);
            // Length of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(tempPartialLength02 >> 8));
            resultBuffer.Add((byte)tempPartialLength02);
            // Protocol version
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            
            resultBuffer.Add((byte)snmpVersion);
            
            // Community
            CommJob job = jList[0];            
            ((SNMPCommJob)job).GetCodifiedCommunity(out byte [] communityBuffer, tempCommunityLength);
            resultBuffer.AddRange(communityBuffer);
            // Message type (get-request)
            resultBuffer.Add(0xa0);
            // Length of the remaining part of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(tempPartialLength01 >> 8));
            resultBuffer.Add((byte)tempPartialLength01);
            // Request-ID            
            SNMPProtocol.ASN1CodifyInt((int)(requestid), out byte[] codifiedRequestID, codifiedRequestIdLength); // snmpRequestID
            resultBuffer.AddRange(codifiedRequestID);
            // Error status
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            resultBuffer.Add(0);
            // Error index
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            resultBuffer.Add(0);
            // Job requests
            resultBuffer.Add(SNMPProtocol.SEQUENCE);
            // Length of the remaining part of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(tempJobsRequestLength >> 8));
            resultBuffer.Add((byte)tempJobsRequestLength);
            foreach (SNMPCommJob jb in jList)
            {                
                // Job request
                resultBuffer.Add(SNMPProtocol.SEQUENCE);
                // Request length
                resultBuffer.Add(0x82);
                uint jobRequestLength = jb.oidObj.codifiedOidLength + 2;
                resultBuffer.Add((byte)(jobRequestLength >> 8));
                resultBuffer.Add((byte)jobRequestLength);
                // Codified OID                    
                resultBuffer.AddRange(jb.oidObj.codifiedOid);
                // Null value
                resultBuffer.Add((byte)SNMPProtocol.SnmProtocolDatatype.Null);
                resultBuffer.Add(0);
            }

            buffer = resultBuffer.ToArray();

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes PrepareWriteRequestv1v2c(List<CommJob> jList, out byte[] buffer)
        {
            buffer = null;

            int jobIndex = 0;
            SNMPCommJob j = null;

            uint writeTotalDataLength = 0;
            List<uint> writeDataLengths = new List<uint>();
            List<byte[]> codifiedValues = new List<byte[]>();

            // only 1 job can be write at time
            while (jobIndex < jList.Count)
            {
                j = jList[jobIndex] as SNMPCommJob;
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
                        codifiedValues.Add((byte[])objectData);
                        writeDataLengths.Add((uint)codifiedValues[codifiedValues.Count-1].Length);
                        writeTotalDataLength += j.GetTotalWriteRequestLength(writeDataLengths[writeDataLengths.Count-1]);
                    }
                }
                
                jobIndex++;
            }

            if (jList.Count == 0)
                return (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;

            j = jList[0] as SNMPCommJob;
            List<byte> resultBuffer = new List<byte>();
            int requestid = GetNewRequestId();
            // Get the length of the codified request ID
            uint codifiedRequestIdLength = SNMPProtocol.ASN1GetCodifiedIntLength(requestid);            
            uint tempCommunityLength = j.GetCodifiedCommunityLength();

            // Calculate the partial length of the jobs requests + request ID + Error status
            uint tempPartialLength01 = codifiedRequestIdLength + 10 + writeTotalDataLength;

            // Calculate the  length of the jobs requests + request ID + Error status + message type + community
            uint tempPartialLength02 = tempPartialLength01 + 4 + tempCommunityLength + 3;

            // Build the header of the message
            // Initial byte
            resultBuffer.Add(SNMPProtocol.SEQUENCE);
            // Length of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(tempPartialLength02 >> 8));
            resultBuffer.Add((byte)tempPartialLength02);
            // Protocol version
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            
            resultBuffer.Add((byte)snmpVersion);
            
            // Community
            ((SNMPCommJob)jList[0]).GetCodifiedCommunity(out byte[] communityBuffer, tempCommunityLength);
            resultBuffer.AddRange(communityBuffer);
            
            // Message type (set-request)
            resultBuffer.Add(0xa3);
            // Length of the remaining part of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(tempPartialLength01 >> 8));
            resultBuffer.Add((byte)tempPartialLength01);
            // Request-ID
            SNMPProtocol.ASN1CodifyInt(requestid, out byte[] codifiedRequestID, codifiedRequestIdLength); //snmpRequestID++)
            resultBuffer.AddRange(codifiedRequestID);
            // Error status
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            resultBuffer.Add(0);
            // Error index
            resultBuffer.Add(0x02);
            resultBuffer.Add(0x01);
            resultBuffer.Add(0);
            // Job requests
            resultBuffer.Add(SNMPProtocol.SEQUENCE);
            // Length of the remaining part of the message
            resultBuffer.Add(0x82);
            resultBuffer.Add((byte)(writeTotalDataLength >> 8));
            resultBuffer.Add((byte)writeTotalDataLength);
            for (int i = 0; i < jList.Count; i++)
            {
                SNMPCommJob job = jList[i] as SNMPCommJob;
                // Job request
                resultBuffer.Add(SNMPProtocol.SEQUENCE);
                // Request length
                resultBuffer.Add(0x82);
                uint jobRequestLength = job.oidObj.codifiedOidLength + (uint)writeDataLengths[i];
                resultBuffer.Add((byte)(jobRequestLength >> 8));
                resultBuffer.Add((byte)jobRequestLength);
                // Codified OID                    
                resultBuffer.AddRange(job.oidObj.codifiedOid);
                // Codified value                
                resultBuffer.AddRange(codifiedValues[i]);                
            }

            buffer = resultBuffer.ToArray();

            return DriverErrorCodes.ErrorNoError;
        }
       
        private int GetNewRequestId()
        {
            requestID++;
            if (requestID <= 0)
                requestID = 1;
            lastRequestID = requestID;

            return requestID;
        }

        private bool IsEqualToLastRequestId(int newRequestId)
        {
            return (lastRequestID == newRequestId);
        }

        public uint PrepareInformReplyRequest(SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out byte[] buffer)
        {
            uint byteNumber = 0;
            buffer = null;

            switch (messageGeneralData.snmpVersion)
            {
                case SNMPVERSION.SNMPv1:
                    // not supported
                    break;
                case SNMPVERSION.SNMPv2c:
                    byteNumber = PrepareInformReplyRequestv2c(messageGeneralData, out buffer);
                    break;
                case SNMPVERSION.SNMPv3:
                    byteNumber = PrepareInformReplyRequestv3(messageGeneralData, out buffer);
                    break;
            }

            return byteNumber;
        }

        public uint PrepareInformReplyRequestv2c(SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out byte[] replyMessage)
        {
            // Build the reply message
            replyMessage = new byte[messageGeneralData.TrapRawData.Length];
            Array.Copy(messageGeneralData.TrapRawData.ToArray(), replyMessage, messageGeneralData.TrapRawData.Length);
            replyMessage[messageGeneralData.ReplyBufferIndex] = (byte)SNMPProtocol.SnmpPduType.Response;

            return (uint)replyMessage.Length;        
        }

        public List<Station> GetStations()
        {
            return CommDriver.GetChannelStations(this);
        }
        #endregion

        #region Check Communication state

        private bool IsDisconnected()
        {
            // set channel in error only if all station are in error
            return (CommDriver.GetChannelStations(this).Count(s => ((SNMPStation)s).Disconnected) > 0);
        }

        public void Flush()
        {
            if (GetBytesToRead() != 0)
            {
                byte[] Buffer = new byte[0];
                DeviceRead(Buffer, 0);
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

        #region Trap
        public DriverErrorCodes GetJobsFromMapOIDJobs(string oidString, out List<SNMPCommJob> jobList)
        {
            // Returned value
            jobList = new List<SNMPCommJob>();
            lock (lockMapOIDJobsObject)
            {
                if (mapOIDJobs.ContainsKey(oidString))
                    jobList.AddRange(mapOIDJobs[oidString]);
            }
            return (jobList.Count > 0 ? DriverErrorCodes.ErrorNoError : (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorTrapOIDNoMatchAnyJobs);
        }

        public void ManageTrapMessage(SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            // Check the protocol Version: it should be SNMPv2c
            if (messageGeneralData.PduType != (byte)SNMPProtocol.SnmpPduType.Inform && messageGeneralData.PduType != (byte)SNMPProtocol.SnmpPduType.V2Trap)
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorChannelTrapWrongReply;

            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
                if (Properties.Settings.Default.TRAP_LOG_ERRORS)
                {
                    CommDriver.GetDriverErrorInfo((int)errorCode, out uint quality, out string error);
                    CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.SNMPErrorTrapErrorInFrame, messageGeneralData.TrapRemoteEndPoint.Address.ToString(), error, SNMPProtocol.SNMPByteMessageToHexString(messageGeneralData.TrapRawData)), System.Diagnostics.EventLogEntryType.Error);
                }
                return;
            }

            foreach (var variable in messageGeneralData.VariableBindings)
            {
                // Get the list of jobs corresponding to the parsed OID            
                errorCode = GetJobsFromMapOIDJobs(variable.oID.stringValue, out List<SNMPCommJob> jobList);
                if (errorCode == DriverErrorCodes.ErrorNoError)
                {
                    // Pass data to the jobs
                    foreach (SNMPCommJob job in jobList)
                    {
                        if (!((SNMPStation)job.Station).IsSuspended())
                        { 
                            errorCode = job.CheckResponseGeneralData(messageGeneralData);

                            if (errorCode == DriverErrorCodes.ErrorNoError)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.Job = job;
                                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                                eJob.Values = variable.TagValue;
                                if (job.snmpTrapOnly)
                                    OnJobExecutedTrap(eJob);
                                else
                                    OnJobExecuted(eJob);
                            }
                            else if (Properties.Settings.Default.TRAP_LOG_ERRORS)
                            {
                                CommDriver.GetDriverErrorInfo((int)errorCode, out uint quality, out string error);
                                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.SNMPErrorTrapMatchingToJobError, variable.oID.stringValue, error), EventSeverity.Min);
                            }
                        }
                    }
                }
            }
        }
        
        public void AddToMapOIDJobs(string oidString, SNMPCommJob job)
        {
            lock (lockMapOIDJobsObject)
            {
                if (!mapOIDJobs.ContainsKey(oidString))
                    mapOIDJobs[oidString] = new List<SNMPCommJob>();
                
                mapOIDJobs[oidString].Add(job);                
            }
        }

        private DriverErrorCodes ParsePollingResponse(List<CommJob> jList, List<byte> receiveBuffer, out SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            messageGeneralData = new SNMPProtocol.SNMPMessageGeneralData();            
            SNMPStation st = jList[0].Station as SNMPStation;
            if (snmpVersion == SNMPVERSION.SNMPv3)
                messageGeneralData.SetUsmSecurity(st);
            DriverErrorCodes errorCode = SNMPProtocol.ParseBaseResponse(receiveBuffer, new List<SNMPStation>() { st }, messageGeneralData);
            byte replyMessage = (byte)SNMPProtocol.SnmpPduType.Response;
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                if (messageGeneralData.PduType != replyMessage)
                    errorCode = SNMPProtocol.GetErrorReplyTypeMismatch(messageGeneralData.PduType, replyMessage);
                else if (!IsEqualToLastRequestId(messageGeneralData.MsgID))
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyID;
            }

            return errorCode;
        }

        private void SetSNMPMaxPacket(uint maxPacket)
        {
            snmpMaxPacket = maxPacket;
            if (snmpVersion == SNMPVERSION.SNMPv3)
                snmpMaxPacketAggregationLimit = (maxPacket / 100) * Properties.Settings.Default.SNMP_PACKET_MAX_PERC_AGGREATION_LIMIT;
            else
                snmpMaxPacketAggregationLimit = maxPacket;

        }

        private void ResetSNMPMaxPacket()
        {
            SetSNMPMaxPacket(snmpMaxPacket);
        }

        public void OnJobExecutedTrap(ExecutedJobArgs e)
        {     
            // update only job's tags values
            e.Job.Station.OnJobExecuted(null, e);
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
            base.Dispose();            
            if (_UDPManager != null) 
                _UDPManager.Dispose();
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

        public SNMPDriver snmpDriver { get; set; }

        #endregion
    }
}
