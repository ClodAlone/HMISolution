using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace S7TCP
{
    public class S7TCPChannel : TcpChannelList
    {
        #region Constructors
        /// <summary>
        /// Initializes the S7TCPChannel object.
        /// </summary>
        public S7TCPChannel(CommunicationDriver commdriver, S7TCPChannelSettings settings)
            : base(commdriver, settings)
        {
            _DeviceID = settings.DeviceID;
            _Rack = settings.Rack;
            _Slot = settings.Slot;
            _TcpChannelHostName = settings.TcpChannelSettingsHostName;
            _TcpChannelHostPort = settings.TcpChannelSettingsHostPort;
            SetMaxPduSize();
        }
        #endregion

        internal uint MIN_PLC_PDU_SIZE = 480; // minimum size of PLC PDU returned during Connect; under this value, use smaller frame size        
        internal uint PLC_PDU_SIZE = 0; // minimum size of PLC PDU returned during Connect; under this value, use smaller frame size        
        internal uint NEW_PLC_PDU_SIZE = 0; //(PLC_PDU_SIZE) use to manage frame data size change "event"
        internal uint MAX_TEL_LENGTH = 0; // 240; --> moved into S7Protocol() CTor()
        internal uint MAX_MPI_TLG_LEN = 0; // 240; --> moved into S7Protocol() CTor()
        internal uint MAX_DATA_BYTES = 0; // 212; --> moved into S7Protocol() CTor()        
        internal int MAX_TEL_LENGTH_BEGIN_DEVICE_READ = 0; // 259; --> moved into S7Protocol() CTor()
        internal uint MAX_BUFFER_SIZE = 0; // 256

        byte[] isoBuffer = null;
        byte[] requestBuffer = null;
        /// <summary>
        /// Flag the test connection
        /// </summary>
        public bool TestConnection = false;
        public bool TestConnectionReadFirmware = false;

        #region Override Methods
        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return false;
        }
        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {            
            int jobIndex = 0;
            while (jobIndex < jobList.Count())
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                bool bRW = false;
                List<CommJob> list = new List<CommJob>();
                S7Protocol.ReadWriteListLimitateSize frameSize = new S7Protocol.ReadWriteListLimitateSize();

                #region fill a list with job as long as size don't execed mamimux frame size MAX_MPI_TLG_LEN
                while (jobIndex < jobList.Count())
                {
                    S7TCPCommJob j = jobList.ElementAt(jobIndex) as S7TCPCommJob;
                    if (first)
                    {
                        write = ((j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput);                        
                        bRW = !j.IsReadRWReady();
                        station = j.Station.Name;
                        first = false;
                    }
                    bool jwrite = ((j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput);
                    bool jbRW = !j.IsReadRWReady();
                    
                    // custom job (StructAtomic, BigArray and BitArray) cannot be aggregate 
                    if (j.Station != null && j.Station.Name == station)
                    {                        
                        if (write && jwrite)
                        {
                            // "read before write condition" must be the same for all aggregated job
                            if (bRW == jbRW)
                            {
                                if (j.IsCustomJob())
                                {                                 
                                    if (TryToAddToWriteJobListCustomJob(ref frameSize, j, list))
                                    {
                                        list.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;
                                        // don't aggregable with other jobs
                                        break;
                                    }
                                }
                                else
                                {
                                    // try to add to write job's list a new job; it total size exceed maximum allowed, exit
                                    if (TryToAddToWriteJobList(ref frameSize, j, list))
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
                        else
                        {
                            if (!write && !jwrite)
                            {
                                if (j.IsCustomJob())
                                {
                                    if (TryToAddToReadJobListCustomJob(ref frameSize, j, list))
                                    {
                                        list.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;
                                    }
                                }
                                else
                                {
                                    // try to add to read job's list a new job; it total size exceed maximum allowed, exit
                                    if (TryToAddToReadJobList(ref frameSize, j, list))
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
                    jobIndex++;
                }

                if (list.Count > 0)
                {                   
                    exList.Add(list);
                    jobIndex = 0;
                }
                #endregion
            }
        }

        internal bool TryToAddToWriteJobList(ref S7Protocol.ReadWriteListLimitateSize frameSize, CommJob j, List<CommJob> list)
        {
            frameSize.AddPaddingByteIfOddAddressRequest();
            
            uint writeRequestLength = ((S7TCPCommJob)j).GetWriteRequestLength();

            if (!IsGetTotalWriteRequestLengthValid(frameSize.RequestLength + writeRequestLength) || !CheckReadReplyLengthForInputOutputJobs(ref list, j))
            {
                return false;
            }
            else
            {
                frameSize.RequestLength += writeRequestLength;

                return true;
            }
        }

        internal bool TryToAddToReadJobListCustomJob(ref S7Protocol.ReadWriteListLimitateSize frameSize, CommJob j, List<CommJob> list)
        {            
            S7Protocol.ReadWriteListLimitateSize frameSizeCustomJob = (S7Protocol.ReadWriteListLimitateSize)frameSize.Clone();
            bool canBeAdd = true;

            List<CommJob> childJobs = j.GetReadChildJobs();
            foreach (S7TCPCommJob childJob in childJobs)
            {                
                if (!TryToAddToReadJobList(ref frameSizeCustomJob, childJob, list))
                {
                    // don't aggregate custom job's child jobs only if another job was aggregated before : 1st job of list have to be added (big job, etc)
                    if (list.Count != 0)
                    {
                        canBeAdd = false;
                        break;
                    }
                }
            }
            if (canBeAdd)
                frameSize = (S7Protocol.ReadWriteListLimitateSize)frameSizeCustomJob.Clone(); 

            return canBeAdd;
        }

        internal bool TryToAddToWriteJobListCustomJob(ref S7Protocol.ReadWriteListLimitateSize frameSize, S7TCPCommJob j, List<CommJob> list)
        {
            // don't aggregable with other jobs
            return (list.Count == 0);
        }


        internal bool TryToAddToReadJobList(ref S7Protocol.ReadWriteListLimitateSize frameSize, S7TCPCommJob j, List<CommJob> list)
        {
            frameSize.AddPaddingByteIfOddAddressResponse();

            // calculated response length
            frameSize.ResponseLength += j.GetReadResponseLength();
            // calculated request length
            frameSize.RequestLength += j.GetReadRequestLength();
            if (frameSize.ResponseLength > MAX_MPI_TLG_LEN || frameSize.RequestLength > MAX_MPI_TLG_LEN)
                return false;
            else
                return true;
        }

        bool Connect(S7TCPStation s)
        {
            byte[] readBuffer = new byte[290];
            s.Connected = false;
    	    uint WriteCh = 0;
	        //Prepare ISO 8073 Connection Request
	        requestBuffer[WriteCh++] =	13 + 2 + 2; //Length indicator
	        requestBuffer[WriteCh++] =	0xE0; //conn request+credit
	        requestBuffer[WriteCh++] =	0x00; //DST-REF
	        requestBuffer[WriteCh++] = 0x00; //constant
	        requestBuffer[WriteCh++] =	0x00; //SRC-REF
	        requestBuffer[WriteCh++] =	0x01; //SRC-REF
	        requestBuffer[WriteCh++] = 0x00; //Class option
	        requestBuffer[WriteCh++] =	0xC1;	//Src TSAP Parameter
	        requestBuffer[WriteCh++] =	2;
	        requestBuffer[WriteCh++] = DeviceID;	//Local Device ID
	        requestBuffer[WriteCh++] = (byte) ((Rack << 5) | Slot) ;	//Local Rack/Slot
	        requestBuffer[WriteCh++] =	0xC2;	//Dst TSAP Parameter
	        requestBuffer[WriteCh++] =	2;
	        // Remote Device ID
	        requestBuffer[WriteCh++] = s.DeviceID;
	        // Remote Rack/Slot
	        requestBuffer[WriteCh++] = (byte)((s.Rack << 5) | s.Slot);
	        requestBuffer[WriteCh++] =	0xC0;
            requestBuffer[WriteCh++] = 1;
	        requestBuffer[WriteCh++] =9; //TPDU size = 512 (9)
            if (!WriteIso(requestBuffer, WriteCh))
                return false;
	        int nLen = ReadIso(readBuffer);
	        if (nLen <= 2) {
                //error connection
                return false;
	        }
            
            if ((readBuffer[1] & 0xF0) != 0xD0) {
                //error connection
                return false;
	        }
            //Prepare the Establish Association PDU
            if (!WriteIso(S7Protocol.connect_block2, (uint)S7Protocol.connect_block2.Length))
		        return false;
	        nLen = ReadIso(readBuffer);
	        if (nLen < 23) {
                //error connection
                return false;
	        }            
            NEW_PLC_PDU_SIZE = S7Protocol.GetConnect_block2_PduSize(readBuffer);
            //S7Protocol.SetMaxPduSize(S7Protocol.NEW_MIN_PLC_PDU_SIZE);
            //InitCommunicationBuffer();
            s.AppHandle = 0;
            s.Connected = true;
          
            return true;
        }
        bool WriteIso(byte[] buf, uint count)
        {
            isoBuffer[0] = 3;
            isoBuffer[1] = 0;
            isoBuffer[2] = (byte)((count + 4) / 256);
            isoBuffer[3] = (byte)((count + 4) & 0xff);
            buf.CopyTo(isoBuffer, 4);
            return DeviceWrite(isoBuffer, count + 4);
        }
        int ReadIso(byte[] buf)
        {
            byte[] header = new byte[4];
            if (!DeviceRead(header, 4))
            { 
                //error timeout RX
                return -1;
            }
            if (header[0] != 3)
                return -1;
            uint len = (uint)(header[2] * 256 + header[3] - 4);
            if (len <= 0)
            {
                //error timeout RX
                return -1;
            }
            if (len > buf.Length)
            { 
                //error buffer overflow
                return -1;
            }
            if(!DeviceRead(buf, len))
            {
                //error timeout RX
                return -1;
            }
            return (int)len;
        }
        bool CheckResponseHeader(List<byte> receiveBuffer, byte ServiceId, int Items, UInt16 apphandle)
        {
            if(TestConnection)
            {
                if ((receiveBuffer[S7Protocol.RESP_PDU_START] != S7Protocol.PROTO_ID))
                {
                    return false;
                }
                //SZL-ID: 0x0111, Diagnostic type: CPU, Number of the partial list extract: A single identification data record, Number of the partial list: Module identification
                //0000 .... .... .... = Diagnostic type: CPU (0x0)
                //0000 0001 0001 0001 = Number of the partial list extract: A single identification data record (0x0111)
                //........0001 0001 = Number of the partial list: Module identification(0x11)
                if ((receiveBuffer[S7Protocol.RESP_PDU_SZL_ID] != 0x01) && (receiveBuffer[S7Protocol.RESP_PDU_SZL_ID + 1] != 0x11))
                {
                    return false;
                }
                return true; 
            }
            if ((receiveBuffer[S7Protocol.RESP_PDU_START] != S7Protocol.PROTO_ID) || (receiveBuffer[1 + S7Protocol.RESP_PDU_START] != 0x03))
            {
                //error reply header
                return false;
            }
            if ((receiveBuffer[S7Protocol.PDUREF] != ((byte)(apphandle >> 8))) ||
                (receiveBuffer[1 + S7Protocol.PDUREF] != ((byte)(apphandle >> 0))))
            {
                //error invalid sequence number
                return false;
            }
            if (receiveBuffer[S7Protocol.ERR_CLS] != 0 || receiveBuffer[S7Protocol.ERR_COD] != 0)
            {
                //error NAK received
                return false;
            }
            if(receiveBuffer.Count < (S7Protocol.POINT_NUM + 1))
            {
                //error receiveBuffer
                return (false);
            }
            if (receiveBuffer[S7Protocol.SERVICE_ID] != ServiceId)
            {
                //error reply service ID
                return false;
            }
            if (receiveBuffer[S7Protocol.POINT_NUM] != Items)
            {
                //error reply pointer number
                return false;
            }
            return true;
        }

        //protected override bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        //{
        //    bool bRet = false;
        //    if(base.SetSynchroJobData(exjob, tagNodeId, value))
        //    {
        //        exjob.UpdateTagsListOnWriting();
        //        S7Protocol.ReadWriteListLimitateSize frameSize = new S7Protocol.ReadWriteListLimitateSize();
        //        List<CommJob> list = new List<CommJob>();
        //        if (exjob.GetTagListOnWritingCount() > 0)
        //            bRet = TryToAddToWriteJobList(ref frameSize, exjob, list);
        //        else
        //            bRet = true;
        //    }
        //    return bRet;
        //}

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            S7TCPStation s = list[0].Station as S7TCPStation;
            if (!s.Connected)
            {
                if (!Connect(s))
                    conn = DriverErrorCodes.ErrorTimeOut;
            }
            
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            List<CommJob> listJobExec = new List<CommJob>();
            uint byteNumber = 0;
            if (!TestConnection)
                byteNumber = S7Protocol.PrepareRequest(this, list, listJobExec, ref requestBuffer);
            else
                byteNumber = S7Protocol.PrepareReadRequestTestInformation(this, list,ref listJobExec, ref requestBuffer, TestConnectionReadFirmware);

            if (byteNumber == 0)
            {
                conn = (DriverErrorCodes)S7ErrorCodes.ErrorPrepareRequestFailed;
                return false;
            }
            // remove from job's list jobs not processed 
            if (list.Count != listJobExec.Count)
            {
                var l = (from j in list where !listJobExec.Contains(j) select j).ToList();
                foreach (S7TCPCommJob j in l)
                {
                    RemovePendingJob(j);
                    list.Remove(j);
                }
            }
            // prepare failed or no message to send
            if (byteNumber == 0 || list.Count == 0 || listJobExec.Count == 0)
            {
                while (list.Count > 0)
                {
                    RemovePendingJob(list[0]);
                    list.Remove(list[0]);
                }
                return false;
            }

            foreach (CommJob job in list)
                base.ExecuteJob(job);

            if (!WriteIso(requestBuffer, byteNumber))
                return false;
                        
            if (!BeginDeviceRead(MAX_TEL_LENGTH_BEGIN_DEVICE_READ))
                return false;

            return true;
        }      

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }
            
            // No active jobs? It should never happen. Anyway, in this case empty the receive buffer and quit 
            if (list.Count < 1)
            {
                Flush();
                return true;
            }
            bool fromWrite = ((S7TCPCommJob)list[0]).WriteExecuted;//Steve 201011
            S7TCPStation s = list[0].Station as S7TCPStation;
                        
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j };
                    OnJobExecuted(eJob);
                }
                // FOGBUGZ 11860
                if (conn == DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorTimeOut)
                {
                    s.Connected = false;
                }
                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }            
            if (receiveBuffer.Count < (S7Protocol.ERR_COD + 5))
            {
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                    OnJobExecuted(eJob);
                }
                Flush();
                s.Connected = false;
                return true;
            }
            if (receiveBuffer[0] != 3)
            {
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorUnrecognizedReply, Job = j };
                    OnJobExecuted(eJob);
                }
                Flush();
                s.Connected = false;
                return true;
            }

            // Check if the reply message is complete
            int replyLength = (receiveBuffer[2] << 8) + receiveBuffer[3];
            if (receiveBuffer.Count < replyLength)
            {
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                    OnJobExecuted(eJob);
                }

                Flush();
                s.Connected = false;
                return true;
            }

            receiveBuffer.RemoveRange(0, 4);
            int Items = 0;
            foreach (S7TCPCommJob j in list)
            {
                if (fromWrite)
                    Items += j.WriteItems;
                else
                    Items++;
            }
            if (!CheckResponseHeader(receiveBuffer, (fromWrite ? S7Protocol.WRITE_SERVICE_ID : S7Protocol.READ_SERVICE_ID), Items, s.AppHandle))
            {
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorWrongHeader, Job = j };
                    OnJobExecuted(eJob);
                }
                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                Flush();
                return true;
            }
            s.AppHandle++;
            int JobRespLen = 0;
            receiveBuffer.RemoveRange(0, S7Protocol.ACCESS_RESULT);
            if (receiveBuffer.Count <= 0)
            {
                s.Connected = false;
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                    OnJobExecuted(eJob);
                }
                return true;
            }
            int statusIndex = 0;
            try
            {           
                foreach (S7TCPCommJob j in list)
                {
                    // The flag WriteExecuted should be coherent for all the jobs
                    if (j.WriteExecuted != fromWrite)
                    {
                        continue;
                    }
                    if (j.WriteExecuted)
                    {
                        if (receiveBuffer[statusIndex] != 0xff)
                        {
                            //error Status not zero
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorStatusNotZero + receiveBuffer[statusIndex], Job = j };
                            OnJobExecuted(eJob);
                        }
                        else
                        {                        
                            // Statistics management
                            if (StatisticsData != null)
                            {
                                lock (lockStatisic)
                                {
                                    j.S7CommJobExchangedByte = 0;
                                    j.S7CommJobExchangedTag = (uint)j.GetTagListOnWritingCount();
                                    List<Tag> tagList = j.GetTagListOnWriting();
                                    foreach (var tag in tagList)
                                    {
                                        j.S7CommJobExchangedByte += tag.Size;
                                    }
                                }
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };
                            OnJobExecuted(eJob);
                        }
                        //statusIndex++;
                        statusIndex += j.WriteItems;
                    }
                    else
                    {
                        if (receiveBuffer.Count < 4)
                        {
                            Flush();
                            s.Connected = false;
                            //error too few data
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                            OnJobExecuted(eJob);
                            return true;
                        }
                        JobRespLen = 0;
                        if(TestConnection)
                        {
                       
                            // Statistics management
                            if (StatisticsData != null)
                            {
                                lock (lockStatisic)
                                {
                                    j.S7CommJobExchangedByte = 0;
                                    j.S7CommJobExchangedTag = (uint)j.TagsList.Count;
                                    foreach (var tag in j.TagsList)
                                        j.S7CommJobExchangedByte += tag.Size;
                                }
                            }

                            if ((receiveBuffer[14] == 0x00) && (receiveBuffer[15] == 0x01))
                            {
                                string plc = System.Text.Encoding.UTF8.GetString(receiveBuffer.ToArray(), 22, 20);
                                S7TCPDriver.Addinfo(string.Format(Properties.Resources.PLCFamily, plc));
                                TestConnectionReadFirmware = true;
                                return true;
                            }
                            if ((receiveBuffer[14] == 0x00) && (receiveBuffer[15] == 0x07))
                            {
                                string firmware = System.Text.Encoding.UTF8.GetString(receiveBuffer.ToArray(), 44, 1);
                                firmware = String.Format("{0}- {1}.{2}{3}", firmware, receiveBuffer[45], receiveBuffer[46], receiveBuffer[47]);
                                S7TCPDriver.Addinfo(string.Format(Properties.Resources.PLCFirmware, firmware));
                                TestConnectionReadFirmware = false;
                                JobRespLen = 4;
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                byte[] Answer;
                                Answer = new byte[JobRespLen];
                                receiveBuffer.CopyTo(0, Answer, 0, JobRespLen);
                                eJob.Values = Answer;
                                eJob.Job = j;
                                OnJobExecuted(eJob);
                            }
                        
                            if (receiveBuffer.Count > 0 )
                            {
                                receiveBuffer.Clear();
                            }
                            continue;
                        }
                        else if (S7Protocol.CheckReadResponse(receiveBuffer, receiveBuffer.Count, ref JobRespLen, j.GetDataLength(), (int)j.Format, j.Length))
                        {
                            if ((JobRespLen >= 0) && (receiveBuffer.Count >= 4 + JobRespLen))
                            {
                                //copy data, starting from receiveBuffer[4]
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                byte[] Answer;
                                Answer = new byte[JobRespLen];
                                receiveBuffer.CopyTo(4, Answer, 0, JobRespLen);

                                // Statistics management
                                if (StatisticsData != null)
                                {
                                    lock (lockStatisic)
                                    {
                                        j.S7CommJobExchangedByte = 0;
                                        j.S7CommJobExchangedTag = (uint)j.TagsList.Count;
                                        foreach (var tag in j.TagsList)
                                            j.S7CommJobExchangedByte += tag.Size;
                                    }
                                }
                                eJob.Values = Answer;
                                eJob.Job = j;
                                OnJobExecuted(eJob);
                            }
                            else
                            {
                                //error too few data
                                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                                OnJobExecuted(eJob);
                            }
                        }
                        else
                        {
                            // when offset variable is set, add to error message also the address used to comunicate (current address)
                            if (j.OffsetVariableSet)
                                j.Station.AdditionalError = j.GetOffsetVariableAddionalErrorInfo();
                            //Error code from the device, code in receiveBuffer[0]
                            ExecutedJobArgs eJob;
                            if (receiveBuffer.Count > 0)
                                eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(S7ErrorCodes.ErrorFromDevice + receiveBuffer[0]), Job = j };
                            else
                                eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                            OnJobExecuted(eJob);
                        }
                        if (receiveBuffer.Count >= 4 + JobRespLen)
                            receiveBuffer.RemoveRange(0, 4 + JobRespLen);
                        if (receiveBuffer.Count > 0 && (JobRespLen % 2 > 0))
                        {
                            receiveBuffer.RemoveRange(0, 1);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                string szAux = String.Format("Exception: {0} in ProcessNewDataList, station: {1}", ex.Message, s != null ? s.Name : String.Empty);
                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, EventSeverity.Medium);
                foreach (S7TCPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorExceptionParsingReply, Job = j };
                    OnJobExecuted(eJob);
                }
                Flush();
                if (s != null)
                {
                    s.Connected = false;
                }
            }            
            return true;
        }

        public override bool DeviceOpen()
        {
            // reset connection state flag
            foreach (S7TCPStation st in CommDriver.GetChannelStations(this))
                st.Connected = false;

            return base.DeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            // reset connection state flag
            foreach (S7TCPStation st in CommDriver.GetChannelStations(this))
                st.Connected = false;

            return base.DeviceClose();
        }

        private void InitCommunicationBuffer()
        {
            isoBuffer = new byte[4 + 12 + 3 + MAX_MPI_TLG_LEN];
            requestBuffer = new byte[12 + 3 + MAX_MPI_TLG_LEN];
        }

        private bool IsPduSizeChanged()
        {
            return (NEW_PLC_PDU_SIZE != PLC_PDU_SIZE);
        }

        private void UpdateMaxPduSize()
        {
            SetMaxPduSize(NEW_PLC_PDU_SIZE);
        }


        /// <summary>
        /// If Pdu size returned durring connection with PLC, uppdate pdu size (constant use the calculate mamimum frame size --> depend of connected PLC) 
        /// when job is not Child Job
        /// </summary>
        /// <param name="list"></param>
        private void TryToRefreshPduSize(List<CommJob> list)
        {
            if (IsPduSizeChanged())
            {
                // use look to avoid scheduler to work when driver update CustomJob
                CommDriver.SuspendScheduler();                
                UpdateMaxPduSize();
                // recreate Custom Jobs if initial protocol frame size if changed
                ReCreateCustomJobs();
                CommDriver.RestartScheduler();
            }
        }

        private bool IsGetTotalWriteRequestLengthValid(uint size)
        {
            // size + 3
            //return ((size + S7Protocol.RESP_PDU_START) < S7Protocol.MAX_MPI_TLG_LEN);
            return (size <= MAX_MPI_TLG_LEN);
        }


        private bool CheckReadReplyLengthForInputOutputJobs(ref List<CommJob> list, CommJob j)
        {
            if ((j.Type != DriverCodeBaseEx.Enumerators.LinkType.InputOutput) || (list.Count < 1) || (j.RWState == CommJob.RWStates.WriteForRW))
            {
                return (true);
            }

            uint TempResponseLength = S7Protocol.INITIAL_RESPONSE_LEN + ((S7TCPCommJob)j).GetReadResponseLength();
            foreach (CommJob job in list)
            {
                if (TempResponseLength > MAX_MPI_TLG_LEN)
                {
                    break;
                }
                if (job.Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput)
                {
                    S7TCPCommJob s7Job = (S7TCPCommJob)job;
                    TempResponseLength += s7Job.GetReadResponseLength();
                }
            }

            if (TempResponseLength > MAX_MPI_TLG_LEN)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("S7 TCP DBG - {0} - CheckReadReplyLengthForInputOutputJobs returning false - TempResponseLength: {1}",
                                                       currentTime, TempResponseLength));
                }
#endif
                return (false);
            }
            else
            {
                return (true);
            }
        }

        private void SetMaxPduSize(uint len = 0)
        {
            // 480 = typical min value returned from PLC indicating PDU Size
            if (len < MIN_PLC_PDU_SIZE)
            {
                MAX_TEL_LENGTH = 240;
                MAX_MPI_TLG_LEN = 240;
                MAX_DATA_BYTES = 212;
                MAX_TEL_LENGTH_BEGIN_DEVICE_READ = 259;
                MAX_BUFFER_SIZE = 256;
            }
            else
            {
                // max data size calculated during test
                MAX_TEL_LENGTH = 480;
                MAX_MPI_TLG_LEN = 480;
                // starting from 490, MAX_DATA_BYTES and MAX_TEL_LENGTH_BEGIN_DEVICE_READ are calculated using same "offset" as standard ( < 480) value
                MAX_DATA_BYTES = 452;
                MAX_TEL_LENGTH_BEGIN_DEVICE_READ = 509;
                MAX_BUFFER_SIZE = 512;
            }

            InitCommunicationBuffer();

            PLC_PDU_SIZE = len;
        }

        #endregion

        #region Properties
        private byte _DeviceID;
        public byte DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }
        private byte _Rack;
        public byte Rack
        {
            get { return _Rack; }
            set { _Rack = value; }
        }
        private byte _Slot;
        public byte Slot
        {
            get { return _Slot; }
            set { _Slot = value; }
        }
        string _TcpChannelHostName;            
        public string TcpChannelHostName
        {
            get {
                return _TcpChannelHostName; 
            }
        }
        int _TcpChannelHostPort;
        public int TcpChannelHostPort
        {
            get
            {
                return _TcpChannelHostPort;
            }
        }
        #endregion

        #region methods
        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            // Statistics management
            if (StatisticsData != null)
            {
                lock (lockStatisic)
                {
                    e.Job.ExchangedByte = ((S7TCPCommJob)e.Job).S7CommJobExchangedByte;
                    e.Job.ExchangedTag = ((S7TCPCommJob)e.Job).S7CommJobExchangedTag;
                }
            }

            e.Job.IsRead = !((S7TCPCommJob)e.Job).WriteExecuted;

            base.OnJobExecuted(e);
        }
        #endregion

        #region Custom Job
        protected override void GetCustomJobChildJobs(List<CommJob> exjobList, out List<List<CommJob>> allChildJobs, out bool write)
        {            
            List<CommJob> resultJobs = new List<CommJob>();
            bool writeResult = false;

            allChildJobs = new List<List<CommJob>>();
            bool someCustomJobs = false;

            foreach (CommJob exjob in exjobList)
            {
                if (!exjob.IsCustomJob())
                {
                    resultJobs.Add(exjob);
                }
                else
                {
                    someCustomJobs = true;
                    S7TCPCommJob exjobS7 = exjob as S7TCPCommJob;
                    writeResult = (exjobS7.GetTagListOnWritingCount() > 0 || exjobS7.Type == LinkType.UnconditionalOutput);
                    if (writeResult)
                    {
                        if (exjobS7.Type == LinkType.UnconditionalOutput)
                            exjobS7.FillWholeTagsListOnWriting();

                        if (exjobS7.IsStructAtomic())
                        {
                            foreach (var tag in exjobS7.GetTagListOnWriting())
                            {
                                if (exjobS7.ChildJobsWrite.ContainsKey(tag.TagNode.NodeId))
                                {
                                    S7TCPCommJob childJob = exjobS7.ChildJobsWrite[tag.TagNode.NodeId] as S7TCPCommJob;
                                    childJob.ClearTagListWrite();                                    
                                    childJob.TagsList[0].SetWriteVal(tag.WriteVal);
                                    childJob.AddTagListOnWriting(childJob.TagsList[0]);
                                    if (childJob.IsCustomJob())
                                    {
                                        GetCustomJobChildJobs(new List<CommJob>() { childJob }, out List<List<CommJob>> customChildJobs, out bool writeChildJob);
                                        if (writeChildJob)
                                        {
                                            foreach (var subList in customChildJobs)
                                                resultJobs.AddRange(subList);
                                        }
                                    }
                                    else
                                    {
                                        resultJobs.Add(childJob);
                                    }
                                }
                            }
                        }
                        else if (exjobS7.IsBitArray())
                        {
                            exjobS7.ChildJobsWrite.Clear();

                            foreach (S7TCPTag tagToWrite in exjobS7.GetTagListOnWriting())
                            {
                                List<S7TCPTag.ChangedBit> changedBits = tagToWrite.GetTagBufferChangedBits();
                                foreach (S7TCPTag.ChangedBit changedBit in changedBits)
                                {
                                    S7TCPCommJob childJob = exjobS7.CreateChildJobWriteArrayBit(changedBit) as S7TCPCommJob;
                                    childJob.ClearTagListWrite();
                                    childJob.TagsList[0].SetWriteVal(changedBit.BitValue);
                                    childJob.AddTagListOnWriting(childJob.TagsList[0]);

                                    exjobS7.ChildJobsWrite[childJob.TagsList[0].TagNode.NodeId] = childJob;

                                    resultJobs.Add(childJob);
                                }
                            }
                        }
                        else if (exjobS7.IsBigArray())
                        {
                            foreach (S7TCPTag tagToWrite in exjobS7.GetTagListOnWriting())
                            {
                                int startIndex = 0;
                                foreach (var job in exjobS7.ChildJobsWrite.Values)
                                {
                                    S7TCPCommJob childJob = job as S7TCPCommJob;
                                    childJob.ClearTagListWrite();
                                    childJob.TagsList[0].SetWriteVal(Tag.ArrayGetTagValue(tagToWrite, tagToWrite.WriteVal, startIndex, (int)childJob.TagsList[0].TagNode.ArrayDimension));
                                    childJob.AddTagListOnWriting(childJob.TagsList[0]);

                                    resultJobs.Add(childJob);

                                    startIndex += (int)childJob.TagsList[0].TagNode.ArrayDimension;
                                }
                            }

                        }                        
                    }
                    else
                    {
                        if (exjobS7.IsStructAtomic())
                        {
                            foreach (var childJob in exjobS7.ChildJobsRead.Values)
                                resultJobs.Add(childJob);
                        }
                        else if (exjobS7.IsBitArray())
                        {
                            foreach (var childJob in exjobS7.ChildJobsRead.Values)
                                resultJobs.Add(childJob);
                        }
                        else if (exjobS7.IsBigArray())
                        {
                            foreach (var childJob in exjobS7.ChildJobsRead.Values)
                                resultJobs.Add(childJob);
                        }
                    }
                }
            }

            if (someCustomJobs)
            {
                foreach (CommJob j in resultJobs)
                {
                    if (j.IsChildJob)
                    {
                        S7TCPCommJob exjobS7 = j.ParentJob as S7TCPCommJob;
                        // transfert to child offset variable value
                        if (exjobS7.OffsetVariableSet)
                            ((S7TCPCommJob)j).CalculateAddressWithOffsetValues(exjobS7.DBOffsetValue, exjobS7.AddressOffsetValue);
                        j.ResetInitialValueChildJob();
                    }
                }

                // try to aggregate more jobs in single request frame
                SplitInExecutionLists(resultJobs, ref allChildJobs);
            }
            else
            {
                allChildJobs.Add(resultJobs);
            }

            write = writeResult;
        }

        /// <summary>
        /// Merge the result of "Big job child jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="childJobs"></param>
        /// <param name="write"></param>
        protected override void MergeCustomJobChildJobs(List<CommJob> exjobList, List<List<CommJob>> allChildJobs, bool write)
        {
            foreach (CommJob exjob in exjobList)
            {
                if (!exjob.IsCustomJob())
                {
                    // do nothing here ! a standard job (not childJob) was already processed
                }
                else
                {
                    S7TCPCommJob exjobS7 = exjob as S7TCPCommJob;

                    ExecutedJobArgs e = new ExecutedJobArgs();
                    e.Job = exjob;
                    e.ErrorCode = DriverErrorCodes.ErrorNoError;

                    List<CommJob> jobChildJobs = new List<CommJob>();
                    foreach (List<CommJob> childJobs in allChildJobs)
                    {
                        foreach (CommJob j in childJobs)
                        {
                            if (j.ParentJob == exjob)
                            {
                                jobChildJobs.Add(j);
                                if (!j.IsPending && j.ChildJobErrorCode != DriverErrorCodes.ErrorNoError)
                                    e.ErrorCode = j.ChildJobErrorCode;
                            }
                        }                        
                    }

                    if (exjobS7.IsStructAtomic())
                    {
                        if (write)
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (Tag tag in exjobS7.GetTagListOnWriting())
                                    tag.SetValue(tag.WriteVal);
                            }
                        }
                        else
                        {
                            // all read jobs are ok
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                object subArraySum = null;
                                foreach (var j in jobChildJobs)
                                    subArraySum = Tag.ArraySumTagValue(j.TagsList[0], j.TagsList[0].GetValue(), subArraySum);

                                byte[] mainAnswer = (byte[])subArraySum;
                                // put a not null value in the answer only to enable driver to publish data 
                                int startTagOffset = S7Protocol.GetJobStartAddress(exjobS7.ParseChildJobsRead[exjobS7.TagsList[0].TagNode.NodeId]);

                                Parallel.ForEach(exjob.TagsList, tag =>                                
                                {
                                    if (exjobS7.ParseChildJobsRead.ContainsKey(tag.TagNode.NodeId))
                                    {
                                        S7TCPCommJob readJob = exjobS7.ParseChildJobsRead[tag.TagNode.NodeId] as S7TCPCommJob;
                                        //S7Protocol.GetAddressOffsetAndSize(readJob.TagsList[0], out int readTagOffset, out uint readTagSizeByte);
                                        S7Protocol.GetJobAddressOffsetAndSize(readJob, out int readTagOffset, out uint readTagSizeByte);

                                        byte[] answer = new byte[readTagSizeByte];
                                        Array.Copy(mainAnswer, readTagOffset - startTagOffset, answer, 0, (int)readTagSizeByte);

                                        List<object> dummy = new List<object>();
                                        S7Protocol.ParseData(answer, ref readJob, ref dummy);
                                        foreach (var cTag in readJob.TagsList)
                                        {
                                            if (tag.GetValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(tag, tag.GetValue(), cTag.GetValue()))
                                                tag.SetValueFromChild(cTag.GetValue());
                                        }
                                    }                                    
                                });                                
                            }
                        }
                    }
                    else if (exjobS7.IsBitArray())
                    {
                        object subArraySum = null;
                        if (write)
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (Tag tag in exjobS7.GetTagListOnWriting())
                                    tag.SetValue(tag.WriteVal);
                            }
                        }
                        else
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (var j in jobChildJobs)
                                    subArraySum = Tag.ArraySumTagValue(j.TagsList[0], j.TagsList[0].GetValue(), subArraySum);

                                // array of bytes
                                byte[] answer = (byte[])subArraySum;
                                List<object> ChangedTags = new List<object>();
                                S7Protocol.ParseData(answer, ref exjobS7, ref ChangedTags);

                                Tag tag = exjobS7.TagsList[0];
                                // ChangedTags.Count > 0 --> values changed
                                if (tag.GetValue() == null || !tag.IsQualityGood() || ChangedTags.Count > 0)                                
                                    tag.SetValueFromChild(tag.GetValue());                                    
                            }
                        }
                    }
                    else if (exjobS7.IsBigArray())
                    {
                        object subArraySum = null;
                        if (write)
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (Tag tag in exjobS7.GetTagListOnWriting())
                                    tag.SetValue(tag.WriteVal);
                            }
                        }
                        else
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (var j in jobChildJobs)
                                    subArraySum = Tag.ArraySumTagValue(j.TagsList[0], j.TagsList[0].GetValue(), subArraySum);

                                Tag tag = exjobS7.TagsList[0];
                                if (tag != null)
                                {
                                    if (tag.GetValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(tag, tag.GetValue(), subArraySum))
                                        tag.SetValueFromChild(subArraySum);
                                }
                            }
                        }
                    }

                    exjob.RWState = CommJob.RWStates.Standard;
                    OnJobExecuted(e);
                }
            }

            // update pdu size (constant use the calculate mamimum frame size --> depend of connected PLC)
            TryToRefreshPduSize(exjobList);
        }

        /// <summary>
        /// Force custom job to be recreated 
        /// </summary>
        private void ReCreateCustomJobs()
        {            
            foreach (S7TCPStation st in CommDriver.GetChannelStations(this))
                st.ReCreateCustomJobs();            
        }
        #endregion
    }
}
