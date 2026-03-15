using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using Opc.Ua;
using System;
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
        internal uint MAX_TEL_LENGTH = 0; // 240; --> moved into S7Protocol() CTor()
        internal uint MAX_MPI_TLG_LEN = 0; // 240; --> moved into S7Protocol() CTor()
        internal uint MAX_DATA_BYTES = 0; // 212; --> moved into S7Protocol() CTor()        
        internal int MAX_TEL_LENGTH_BEGIN_DEVICE_READ = 0; // 259; --> moved into S7Protocol() CTor()
        internal uint MAX_BUFFER_SIZE = 0; // 256

        byte[] isoBuffer = null;
        byte[] requestBuffer = null;

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

                uint ReqDim = S7Protocol.INITAL_REQUEST_DIM;
                uint ReqDimRW = S7Protocol.INITAL_REQUEST_DIM;
                bool bRW = false;
                List<CommJob> listRW = null;
                int TempResponseLength = 14;

                exList.Add(new List<CommJob>());
                List<CommJob> list = exList[exList.Count - 1];

                #region fill a list with job as long as size don't execed mamimux frame size MAX_MPI_TLG_LEN
                while (jobIndex < jobList.Count())
                {
                    S7TCPCommJob j = jobList.ElementAt(jobIndex) as S7TCPCommJob;
                    if (first)
                    {
                        write = ((j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput)
                            && !j.IsReadRWReady();
                        bRW = j.IsReadRWReady();
                        station = j.Station.Name;
                        first = false;
                    }
                    bool jwrite = ((j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput)
                        && !j.IsReadRWReady();

                    if (j.Station != null && j.Station.Name == station)
                    {
                        if (write)
                        {
                            if (jwrite)
                            {
                                // try to add to write job's list a new job; it total size exceed maximum allowed, exit
                                if (!TryToAddToWriteJobList(ref ReqDim, ref list, j))
                                    break;

                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                        else
                        {
                            if (!jwrite && j.Type != LinkType.ExceptionOutput && j.IsReadRWReady() == bRW)
                            {
                                //Checked fill byte
                                if ((TempResponseLength % 2) != 0)
                                    ++TempResponseLength;

                                // calculated response length
                                TempResponseLength += (int)j.GetReadResponseLength();
                                // calculated request length
                                ReqDim += j.GetReadRequestLength();

                                if (TempResponseLength > MAX_MPI_TLG_LEN || ReqDim > MAX_MPI_TLG_LEN)
                                    break;

                                if (bRW && listRW == null)
                                    listRW = new List<CommJob>();
                                if (bRW && !TryToAddToWriteJobList(ref ReqDimRW, ref listRW, j))
                                    break;

                                //Added Any pointer header into request       
                                j.GestRWState();
                                list.Add(j);

                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                    }

                    jobIndex++;
                }
                #endregion
            }
        }

        internal bool TryToAddToWriteJobList(ref uint ReqDim, ref List<CommJob> list, S7TCPCommJob j)
        {
            // dont' allow to aggregate BigArray and BitArray jobs
            if ((j.IsBigArray() || j.IsBitArray()) && list.Count != 0)
                return true;

            if ((ReqDim & 1) != 0)
            {
                ReqDim++;
            }

            List<Tag> listToWrite = new List<Tag>();
            List<Tag> listToWriteLater = new List<Tag>();
            uint writeRequestLength = j.GetWriteRequestLength(ref listToWrite, ref listToWriteLater);
            j.ClearTagListOnWriting();            
            if (!IsGetTotalWriteRequestLengthValid(ReqDim + writeRequestLength) || !CheckReadReplyLengthForInputOutputJobs(ref list, j))
            {
                // if job cannot be schedule, reload tag's write values into TagsListToWrite
                listToWrite.ForEach(tag =>
                {
                    j.AddTagListToWrite(tag);
                });
                listToWriteLater.ForEach(tag =>
                {
                    j.AddTagListToWrite(tag);
                });
                return false;
            }
            ReqDim += writeRequestLength;
            j.AddtmpTagsListToWrite(listToWrite);

            // Remove from list only scheduled tags (j.TagsListToWrite has been cleared in j.GetWriteRequestLength)
            if (listToWriteLater.Count > 0)
            {
                listToWriteLater.ForEach(tag =>
                {
                    j.AddTagListToWrite(tag);
                });
            }

            list.Add(j);

            //// dont' allow to aggregate BigArray and BitArray jobs
            //if (j.IsBigArray() || j.IsBitArray())
            //    return false;
 
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

            uint plcPduSize = S7Protocol.GetConnect_block2_PduSize(readBuffer);

            SetMaxPduSize(plcPduSize);

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

        protected override bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            bool bRet = false;
            if(base.SetSynchroJobData(exjob, tagNodeId, value))
            {
                exjob.UpdateTagsListOnWriting();
                uint ReqDim = S7Protocol.INITAL_REQUEST_DIM;
                List<CommJob> list = new List<CommJob>();
                bRet = TryToAddToWriteJobList(ref ReqDim, ref list, (exjob as S7TCPCommJob));
            }
            return bRet;
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            S7TCPStation s = list[0].Station as S7TCPStation;
            if (!s.Connected)
            {
                if (!Connect(s))
                {
                    conn = DriverErrorCodes.ErrorTimeOut;
                    return false;
                }
            }

            List<CommJob> ListJobExec = new List<CommJob>();
            uint byteNumber = S7Protocol.PrepareRequest(this, list, ref ListJobExec, ref requestBuffer);
            // exception occour during requestBuffer filling 
            if (byteNumber == 0)
            {
                conn = (DriverErrorCodes)S7ErrorCodes.ErrorPrepareRequestFailed;
                return false;
            }

            if (ListJobExec.Count() == 0)
            {
                foreach (S7TCPCommJob j in list)
                    RemovePendingJob(j);
                list.Clear();
                ListJobExec.Clear();
                return false;
            }

            if (list.Count > ListJobExec.Count)
            {
                var l = (from j in list where !ListJobExec.Contains(j) select j).ToList();
                foreach (S7TCPCommJob j in l)
                {
                    RemovePendingJob(j);
                    list.Remove(j);
                }
            }

            if (!WriteIso(requestBuffer, byteNumber))
                return false;

            ListJobExec[0].StartExecutionTime = DateTime.UtcNow;
            foreach (S7TCPCommJob j in ListJobExec)
            {
                j.LastExecutionTime = ListJobExec[0].StartExecutionTime;
                j.StartExecutionTime = ListJobExec[0].StartExecutionTime;
            }

            if (!BeginDeviceRead(MAX_TEL_LENGTH_BEGIN_DEVICE_READ))
                return false;

            return true;
        }


        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            S7TCPStation s = null;
            try
            {

            // No active jobs? It should never happen. Anyway, in this case empty the receive buffer and quit 
            if(list.Count < 1)
            {
                lock (lockThreadObject)
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            bool fromWrite = ((S7TCPCommJob)list[0]).WriteExecuted;//Steve 201011
            s = list[0].Station as S7TCPStation;
                        
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                foreach (S7TCPCommJob j in list)
                {
                    //j.removeFromPending = true;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j };
                    OnJobExecuted(eJob);
                }

                // FOGBUGZ 11860
                if (conn == DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorTimeOut)
                {
                    s.Connected = false;
                }

                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
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
            if(receiveBuffer.Count < replyLength)
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
                    //j.removeFromPending = true;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                    OnJobExecuted(eJob);
                }
                return true;
            }

            int statusIndex = 0;
            foreach (S7TCPCommJob j in list)
            {
                // The flag WriteExecuted should be coherent for all the jobs
                if (j.WriteExecuted != fromWrite)
                {
                    continue;
                }

                if (fromWrite)
                {
                    // Check the access result (more access results could correspond to the same job)
                    bool accessResultOK = true;
                    int accessResultIndex = 0;
                    for(accessResultIndex = 0; accessResultIndex < j.WriteItems; accessResultIndex++)
                    {
                        if ((receiveBuffer.Count < statusIndex + accessResultIndex + 1) || (receiveBuffer[statusIndex + accessResultIndex] != 0xff))
                        {
                            accessResultOK = false;
                            break;
                        }
                    }

                    if (!accessResultOK)
                    {
                        //j.removeFromPending = true;
                        //error Status not zero
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorStatusNotZero + receiveBuffer[statusIndex], Job = j };
                        OnJobExecuted(eJob);
                    }
                    else
                    {
                        bool JobCompleted = true;
                        //// if not all bit of array were written, keep job active and schedule one for next cycle
                        if (j.ArrayBoolWriteCheck)
                        {
                            JobCompleted = false;
                            //j.removeFromPending = false;
                        }
                        else
                        {
                            if (j.IsBigArray())
                            {
                                // if not completed, keep job alive/pending
                                if (j.ExecutionOffsetWrite != 0)
                                {
                                    //j.removeFromPending = false;
                                    JobCompleted = false;
                                    // if data to write was removed, add again
                                    if ((j.TagsList.Count > 0) && !j.tmpTagsListToWrite.Contains(j.TagsList[0]))
                                    {
                                        j.AddtmpTagsListToWrite(new List<Tag> { j.TagsList[0] });
                                    }
                                }
                            }
                        }

                        if (JobCompleted)
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
                            //j.removeFromPending = true;
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };
                            OnJobExecuted(eJob);
                        }
                    }
                    j.WriteExecuted = false;
                    statusIndex += j.WriteItems;
                }
                else
                {
                    //j.removeFromPending = true;
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
                    if (S7Protocol.CheckReadResponse(receiveBuffer, receiveBuffer.Count, ref JobRespLen, j.GetDataLength(), (int)j.Format, j.Length))
                    {
                        if ((JobRespLen >= 0) && (receiveBuffer.Count >= 4 + JobRespLen))
                        {
                            //copy data, starting from receiveBuffer[4]
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            byte[] Answer;
                            Answer = new byte[JobRespLen];
                            receiveBuffer.CopyTo(4, Answer, 0, JobRespLen);
                            if (j.IsBigArray())
                            {
                                j.ListRecievedBigArrary.AddRange(Answer);
                                j.ExecutionOffset += (uint)Answer.Length;

                                uint byteSize = j.TotalJobSize;
                                if ((j.TagsList.Count > 0) && (j.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean))
                                {
                                    byteSize = (((uint)(j.Bit % 8) + byteSize) / 8);
                                    if (((uint)(j.Bit % 8) + byteSize) > 0)
                                    {
                                        byteSize++;
                                    }
                                }
                                else if (j.isLenStringEnable == true)
                                {
                                    byteSize++;
                                }

                                if (j.ExecutionOffset == byteSize)
                                {
                                    // Statistics management
                                    if (StatisticsData != null)
                                    {
                                        lock (lockStatisic)
                                        {
                                            j.S7CommJobExchangedByte = 0;
                                            j.S7CommJobExchangedTag = (uint)j.GetTagListCount();
                                            List<Tag> tagList = j.GetTagList();
                                            foreach (var tag in tagList)
                                            {
                                                j.S7CommJobExchangedByte += tag.Size;
                                            }
                                        }
                                    }
                                    j.ExecutionOffset = 0;
                                    eJob.Values = j.ListRecievedBigArrary.ToArray();
                                    eJob.Job = j;
                                    OnJobExecuted(eJob);
                                    j.ListRecievedBigArrary.Clear();
                                }
                            }
                            else
                            {
                                // Statistics management
                                if (StatisticsData != null)
                                {
                                    lock (lockStatisic)
                                    {
                                        j.S7CommJobExchangedByte = 0;
                                        j.S7CommJobExchangedTag = (uint)j.GetTagListCount();
                                        List<Tag> tagList = j.GetTagList();
                                        foreach (var tag in tagList)
                                        {
                                            j.S7CommJobExchangedByte += tag.Size;
                                        }
                                    }
                                }
                                eJob.Values = Answer;
                                eJob.Job = j;
                                OnJobExecuted(eJob);
                            }
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
                        {
                            j.Station.AdditionalError = j.GetOffsetVariableAddionalErrorInfo();
                        }
                        //Error code from the device, code in receiveBuffer[0]
                        ExecutedJobArgs eJob;
                        if (receiveBuffer.Count > 0)
                        {
                            eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(S7ErrorCodes.ErrorFromDevice + receiveBuffer[0]), Job = j };
                        }
                        else
                        {
                            eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                        }
                        OnJobExecuted(eJob);
                    }
                    if (receiveBuffer.Count >= 4 + JobRespLen)
                    {
                        receiveBuffer.RemoveRange(0, 4 + JobRespLen);
                    }
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
                if(s != null)
                {
                    s.Connected = false;
                }
                return true;
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
       
        public override bool DeviceClose()
        {
            // reset connection state flag
            foreach (S7TCPStation st in CommDriver.GetChannelStations(this))
                st.Connected = false;

            return base.DeviceClose();
        }


        /// <summary>
        /// When strange error occours, data to write was not sent, so data were not moved to TagsListOnWriting. During OnJobExecuted, restoring data from TagsListOnWriting to TagsListToWrite failed
        /// </summary>
        /// <param name="ListJobPending"></param>
        private void RestoreTemporaryPendingWriteToMainTagsListToWrite(S7TCPCommJob job)
        {
            //// reset job's internal variable 
            if (job.Type == LinkType.InputOutput || job.Type == LinkType.UnconditionalOutput || job.Type == LinkType.ExceptionOutput)
            {
                List<Tag> tagList = job.GettmpTagListToWrite();
                while (tagList.Count > 0)
                {
                    job.AddTagListToWrite(tagList[0]);
                    tagList.RemoveAt(0);
                }
                job.CleartmpTagsListToWrite();
            }            
        }

        private void InitCommunicationBuffer()
        {
            isoBuffer = new byte[4 + 12 + 3 + MAX_MPI_TLG_LEN];
            requestBuffer = new byte[12 + 3 + MAX_MPI_TLG_LEN];
        }


        private bool IsGetTotalWriteRequestLengthValid(uint size)
        {
            // size + 3
            //return ((size + S7Protocol.RESP_PDU_START) < S7Protocol.MAX_MPI_TLG_LEN);
            return (size <= MAX_MPI_TLG_LEN);
        }


        private bool CheckReadReplyLengthForInputOutputJobs(ref List<CommJob> list, S7TCPCommJob j)
        {
            if ((j.Type != DriverCodeBaseEx.Enumerators.LinkType.InputOutput) || (list.Count < 1) || (j.RWState == CommJob.RWStates.WriteForRW))
            {
                return (true);
            }

            int TempResponseLength = 14 + (int)j.GetReadResponseLength();
            foreach (CommJob job in list)
            {
                if (TempResponseLength > MAX_MPI_TLG_LEN)
                {
                    break;
                }
                if (job.Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput)
                {
                    S7TCPCommJob s7Job = (S7TCPCommJob)job;
                    TempResponseLength += (int)s7Job.GetReadResponseLength();
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
            // on error reset array of bit flags management
            if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
            {
                RestoreTemporaryPendingWriteToMainTagsListToWrite((S7TCPCommJob)e.Job);

                // on error reset array of bit management's flags
                if (((S7TCPCommJob)e.Job).ArrayBoolWriteCheck)
                    ((S7TCPCommJob)e.Job).ResetArrayBoolWriteManualCheck();

            }
            else
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
            }

            if (((S7TCPCommJob)e.Job).WriteExecuted)
            {
                e.Job.IsRead = false;
            }
            else
            {
                e.Job.IsRead = true;
            }

            base.OnJobExecuted(e);
        }

        #endregion
    }
}
