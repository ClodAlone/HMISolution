using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.Threading;
using System.Threading.Tasks;

namespace S7TCP
{
    public class S7TCPChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public S7TCPChannel(CommunicationDriver commdriver, S7TCPChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _DeviceID = settings.DeviceID;
            _Rack = settings.Rack;
            _Slot = settings.Slot;
        }

        #endregion
        
        byte[] isoBuffer = new byte [4 + 12 + 3 + S7Protocol.MAX_MPI_TLG_LEN];
        byte[] requestBuffer = new byte[12 + 3 + S7Protocol.MAX_MPI_TLG_LEN];
        byte[] readBuffer = new byte[290];
        
        #region Override Methods        
        protected override void WorkingThread(object data)
        {
            int sleepCycle = (Int32)WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            
            while (true)
            {
                if ((ListJobPending.Count == 0 && ListJobExecuted.Count == 0) || MultiPointProtocol)
                {
                    List<S7TCPCommJob> nextlist = new List<S7TCPCommJob>();
                    ScheduleListJob();
                    lock (lockThreadObject)//201011
                    {
                        if (SynchroJob != null)
                        {
                            nextlist.Add(SynchroJob as S7TCPCommJob);
                        }
                        else
                        GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                    {
                        ListJobPending.AddRange(nextlist);
                        ListJobPending[0].StartExecutionTime = DateTime.UtcNow;
                        foreach (S7TCPCommJob j in ListJobPending)
                        {
                            j.LastExecutionTime = ListJobPending[0].StartExecutionTime;
                            j.StartExecutionTime = ListJobPending[0].StartExecutionTime;
                        }
                    }
                }

                if (ListJobPending.Count > 0 && ListJobExecuted.Count == 0)
                {
                    List<CommJob> ListJobExec = new List<CommJob>();
                    if (!IsDeviceOpen())
                        DeviceOpen();
                        
                    lock (lockThreadObject)//201011
                    {
                        ExecuteJobList(ListJobPending, ref ListJobExec);
                    }
                    foreach (var job in ListJobExec)
                    {                            
                        (job as S7TCPCommJob).removeFromPending = false;
                        ListJobExecuted.Add(job);
                    }
                }

                lock (lockThreadObject)
                {
                    bool bNew = NewDataToAnlyze.WaitOne(0, false);
                    if (bNew || ReceiveBuffer.Count > 0)
                    {
                        NewDataToAnlyze.Reset();
                        if (ListJobExecuted.Count > 0)
                        {
                            if (ProcessNewDataList(ListJobExecuted))
                            {
                                ReceiveBuffer.Clear();
                                //bool ClearListJobExecuted = false;
                                foreach (var job in ListJobExecuted)
                                {
                                    //management for large arrays
                                    if ((((S7TCPCommJob)job).ExecutionOffset == 0) && (((S7TCPCommJob)job).ExecutionOffsetWrite == 0))
                                    {
                                        //ClearListJobExecuted = true;
                                        job.LastExecutionTime = DateTime.UtcNow;
                                        if (SynchroJob != null && SynchroJob == job && (SynchroJob as S7TCPCommJob).tmpTagsListToWrite.Count == 0)
                                        {
                                            job.ResetSynchro.WaitOne(Timeout);
                                            SynchroJob = null;
                                            job.ResetSynchro.Reset();
                                        }
                                        if((job as S7TCPCommJob).removeFromPending)
                                        {
                                            ListJobPending.Remove(job);
                                        }
                                    }
                                    else
                                    {
                                        if((((S7TCPCommJob)job).ExecutionOffsetWrite != 0))
                                        {
                                            lock (job.retLockList()) 
                                            {
                                                if (!(ListJobPending[0] as S7TCPCommJob).tmpTagsListToWrite.Contains(job.TagsList[0]))
                                                    (ListJobPending[0] as S7TCPCommJob).tmpTagsListToWrite.Insert(0, job.TagsList[0]);
                                            }
                                        }
                                        job.IsPending = false;
                                        //if(!ListJobPending.Contains(job))
                                        //{
                                        //    ListJobPending.Add(job);
                                        //}
                                    }
                                }
                                //if(ClearListJobExecuted)
                                //{
                                    ListJobExecuted.Clear();
                                //}                                
                            }
                        }
                    }

                    if (ListJobPending.Count > 0 || ListJobExecuted.Count() > 0)
                    {
                        List<CommJob> tmpListJobPending = ListJobPending.Count > 0 ? ListJobPending : ListJobExecuted;
                        if (!MultiPointProtocol)
                        {
                            double dtime = (DateTime.UtcNow - tmpListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                            if (dtime > Timeout)
                            {
                                //error
                                LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewDataList(tmpListJobPending);
                                //ProcessNewData(ListJobPending[0]);
                                if(tmpListJobPending == ListJobPending)
                                {
                                    ListJobExecuted.Remove(ListJobPending[0]);
                                    ListJobPending.RemoveAt(0);
                                }
                                else
                                {
                                    ListJobPending.Remove(ListJobExecuted[0]);
                                    ListJobExecuted.RemoveAt(0);
                                }
                                ReceiveBuffer.Clear();
                            }
                        }
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    DeviceClose();

                if (ListJobPending.Count != 0 && StopWorkerThread.WaitOne(sleepCycle, false))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                }
                if(StopWorkerThread.WaitOne(0, false))
                    break;
            }
        }

        protected void GetNextPendingList(ref List<S7TCPCommJob> list)
        {
            bool write = false;
            string station = string.Empty;
            bool first = true;
            uint ReqDim = 12;
            int TempResponseLength = 14;

            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return;

                int cnt = queue.Count;
                while (cnt-- > 0)
                {
                    CommJob dequeuedJob = null;
                    S7TCPCommJob j = null;
                    if (queue.TryDequeue(out dequeuedJob) == true)
                    {
                        j = (S7TCPCommJob)dequeuedJob;
                    }
                    if (j == null)
                    {
                        continue;
                    }
                    if (j.IsPending)
                    {
                        queue.Enqueue(j);
                        continue;
                    }
                    if (j.OffsetVariableSet)
                        ((S7TCPCommJob)j).CalculateAddressWithOffsetValues();

                    if (j.TagsListOnWriting.Count == 0)
                    {
                        if (first)
                        {
                            write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                        }
                        bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                // try to add to write job's list a new job; it total size exceed maximum allowed, exit
                                if (!TryToAddToWriteJobList(ref ReqDim, ref list, j))
                                    break;
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {
                                //Checked fill byte
                                if ((TempResponseLength % 2) != 0)
                                {
                                    ++TempResponseLength;
                                }

                                int nTempRestLen = (int)j.GetReadResponseLength() + TempResponseLength;
                                if (nTempRestLen > S7Protocol.MAX_MPI_TLG_LEN)
                                    break;
                                TempResponseLength = nTempRestLen;

                                if (ReqDim + j.GetReadRequestLength() > S7Protocol.MAX_MPI_TLG_LEN)
                                    break;
                                //Added Any pointer header into request
                                ReqDim += j.GetReadRequestLength();
                                list.Add(j);
                            }
                        }
                    }
                }
            }
        }
        
        internal bool TryToAddToWriteJobList(ref uint ReqDim, ref List<S7TCPCommJob> list, S7TCPCommJob j)
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
            if (!S7Protocol.IsGetTotalWriteRequestLengthValid(ReqDim + writeRequestLength))
            {
                // if job cannot be schedule, reload tag's write values into TagsListToWrite
                lock (j.retLockList())
                {
                    j.TagsListToWrite.AddRange(listToWrite);
                    j.TagsListToWrite.AddRange(listToWriteLater);
                }
                return false;
            }
            ReqDim += writeRequestLength;
            lock (j.retLockList())
            {
                j.tmpTagsListToWrite.Clear();
                j.tmpTagsListToWrite.AddRange(listToWrite);
                j.tmpTagsListToWrite.Sort(S7TCPCommJob.CompareTagByOffset);
                // Remove from list only scheduled tags (j.TagsListToWrite has been cleared in j.GetWriteRequestLength)
                if (listToWriteLater.Count > 0)
                {
                    j.TagsListToWrite.AddRange(listToWriteLater);
                }
            }

            list.Add(j);

            // dont' allow to aggregate BigArray and BitArray jobs
            if (j.IsBigArray() || j.IsBitArray())
                return false;

            return true;
        }

        bool Connect(S7TCPStation s)
        {
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
        bool CheckResponseHeader(byte ServiceId, int Items, UInt16 apphandle)
        {
            if ((ReceiveBuffer[S7Protocol.RESP_PDU_START] != S7Protocol.PROTO_ID) ||
        (ReceiveBuffer[1 + S7Protocol.RESP_PDU_START] != 0x03))
            {
                //error reply header
                return false;
            }

            if ((ReceiveBuffer[S7Protocol.PDUREF] != ((byte)(apphandle >> 8))) ||
                (ReceiveBuffer[1 + S7Protocol.PDUREF] != ((byte)(apphandle >> 0))))
            {
                //error invalid sequence number
                return false;
            }

            if (ReceiveBuffer[S7Protocol.ERR_CLS] != 0 || ReceiveBuffer[S7Protocol.ERR_COD] != 0)
            {
                //error NAK received
                return false;
            }

            if(ReceiveBuffer.Count < (S7Protocol.POINT_NUM + 1))
            {
                //error ReceiveBuffer
                return (false);
            }

            if (ReceiveBuffer[S7Protocol.SERVICE_ID] != ServiceId)
            {
                //error reply service ID
                return false;
            }

            if (ReceiveBuffer[S7Protocol.POINT_NUM] != Items)
            {
                //error reply pointer number
                return false;
            }

            return true;
        }

        protected void ExecuteJobList(List<CommJob> ListJobPending, ref List<CommJob> ListJobExec)
        {
            S7TCPStation s = ListJobPending[0].Station as S7TCPStation;
            if (s == null)
                return;

            if (!s.Connected && s.LastErrorCode == (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice &&
                (DateTime.UtcNow - s.LastErrorTime).TotalMilliseconds < 2000)
            {

                RestoreTemporaryPendingWriteToMainTagsListToWrite(ListJobPending);
                return;
            }


            if (!s.Connected && !Connect(s))
            {
                //Error connection
                if (s.LastErrorCode != (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice)
                {
                    //CommDriver.OnSystemEvent(ObjectIds.Server, S7TCP.Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High,true);
                    s.LastErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice;
                    s.LastErrorTime = DateTime.UtcNow;
                }

                RestoreTemporaryPendingWriteToMainTagsListToWrite(ListJobPending);

                return;
            }

            uint byteNumber = S7Protocol.PrepareRequest(ListJobPending, ref ListJobExec, ref requestBuffer);
            if (byteNumber == 0 || ListJobExec.Count() == 0)
            {
                foreach (S7TCPCommJob j in ListJobPending)
                {
                    j.IsPending = false;
                }
                ListJobPending.Clear();
                ListJobExec.Clear();
                return;
            }
            
            ReceiveBuffer.Clear();

            if (!WriteIso(requestBuffer, byteNumber))
                return;

            ListJobExec[0].StartExecutionTime = DateTime.UtcNow;
            foreach (S7TCPCommJob j in ListJobExec)
            {
                j.LastExecutionTime = ListJobExec[0].StartExecutionTime;
                j.StartExecutionTime = ListJobExec[0].StartExecutionTime;
            }

            if (!BeginDeviceRead(259))
            {
                return;
            }
            
        }

        bool ProcessNewDataList(List<CommJob> list)
        {
            bool fromWrite = ((S7TCPCommJob)list[0]).WriteExecuted;//steve 201011
            S7TCPStation s = list[0].Station as S7TCPStation;
            if (s == null)
            {
                ReceiveBuffer.Clear();
                Flush();
                return false;
            }
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                foreach(S7TCPCommJob j in list)
                {
                    j.removeFromPending = true;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = j};
                    OnJobExecuted(eJob);
                }

                // FOGBUGZ 11860
                if (LastErrorCode == DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut)
                {
                    s.Connected = false;
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            if ((ReceiveBuffer.Count < (S7Protocol.ERR_COD + 5)) || (ReceiveBuffer[0] != 3))
            {
                ReceiveBuffer.Clear();
                Flush();
                s.Connected = false;
                return false;
            }

            ReceiveBuffer.RemoveRange(0, 4);

            int Items = 0;
            foreach (S7TCPCommJob j in list)
            {
                if (fromWrite)
                    Items += j.WriteItems;
                else
                    Items++;
            }
            if (!CheckResponseHeader((fromWrite ? S7Protocol.WRITE_SERVICE_ID : S7Protocol.READ_SERVICE_ID), Items, s.AppHandle))
            {
                foreach (S7TCPCommJob j in list)
                {
                    j.removeFromPending = true;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorWrongHeader, Job = j };
                    OnJobExecuted(eJob);
                }
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            s.AppHandle++;
            int JobRespLen = 0;

            ReceiveBuffer.RemoveRange(0, S7Protocol.ACCESS_RESULT);
            if(ReceiveBuffer.Count <= 0)
            {
                s.Connected = false;
                return false;
            }

            int statusIndex = 0;
            foreach(S7TCPCommJob j in list)
            {
                if (j.WriteExecuted)
                {
                    if (ReceiveBuffer[statusIndex] != 0xff)
                    {
                        j.removeFromPending = true;
                        //error Status not zero
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorStatusNotZero + ReceiveBuffer[statusIndex], Job = j };
                        OnJobExecuted(eJob);
                    }
                    else
                    {
                        bool JobCompleted = true;
                        //// if not all bit of array were written, keep job active and schedule one for next cycle
                        if (j.ArrayBoolWriteCheck)
                        {
                            JobCompleted = false;
                            j.removeFromPending = false;
                        }
                        else
                        {                            
                            if (j.IsBigArray())
                            {
                                // if not completed, keep job alive/pending
                                if (j.ExecutionOffsetWrite != 0)
                                {
                                    j.removeFromPending = false;
                                    JobCompleted = false;
                                }
                            }
                        }

                        if (JobCompleted)
                        {
                            // Statistics management
                            if (StatisticsData != null)
                            {
                                lock (j.retLockList())
                                {
                                    j.S7CommJobExchangedByte = 0;
                                    j.S7CommJobExchangedTag = (uint)j.TagsListOnWriting.Count;
                                    foreach (var tag in j.TagsListOnWriting)
                                    {
                                        j.S7CommJobExchangedByte += tag.Size;
                                    }
                                }
                            }
                            j.removeFromPending = true;
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };
                            OnJobExecuted(eJob);
                        }
                    }
                    j.WriteExecuted = false;
                    statusIndex++;
                }
                else
                {
                    j.removeFromPending = true;
                    if(ReceiveBuffer.Count < 4)
                    {
                        ReceiveBuffer.Clear();
                        Flush();
                        s.Connected = false;
                        //error too few data
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j };
                        OnJobExecuted(eJob);
                        return false;
                    }
                    

                    if (S7Protocol.CheckReadResponse(ReceiveBuffer, ReceiveBuffer.Count, ref JobRespLen, j.GetDataLength(), (int)j.Format, j.Length))
                    {                        
                        if (ReceiveBuffer.Count >= 4 + JobRespLen)
                        {
                            //copy data, starting from ReceiveBuffer[4]
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            byte[] Answer;
                            Answer = new byte[JobRespLen];
                            ReceiveBuffer.CopyTo(4, Answer, 0, JobRespLen);
                            if(j.IsBigArray())
                            {
                                j.ListRecievedBigArrary.AddRange(Answer);
                                j.ExecutionOffset += (uint)Answer.Length;

                                uint byteSize = j.TotalJobSize;
                                if (j.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                                {
                                    byteSize /= 8;
                                    if ((j.TotalJobSize % 8) > 0)
                                    {
                                        byteSize++;
                                    }
                                }
                                else if(j.isLenStringEnable == true)
                                {
                                    byteSize++;
                                }

                                if (j.ExecutionOffset == byteSize)
                                {
                                    // Statistics management
                                    if (StatisticsData != null)
                                    {
                                        lock (j.retLockList())
                                        {
                                            j.S7CommJobExchangedByte = 0;
                                            j.S7CommJobExchangedTag = (uint)j.TagsList.Count;
                                            foreach (var tag in j.TagsList)
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
                                    lock (j.retLockList())
                                    {
                                        j.S7CommJobExchangedByte = 0;
                                        j.S7CommJobExchangedTag = (uint)j.TagsList.Count;
                                        foreach (var tag in j.TagsList)
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
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData, Job = j};
                            OnJobExecuted(eJob);
                        }
                    }
                    else
                    {
                        // when offset variable is set, add to error message also the address used to comunicate (current address)
                        if (j.OffsetVariableSet)
                            j.Station.AdditionalError = j.GetOffsetVariableAddionalErrorInfo();
                        //Error code from the device, code in ReceiveBuffer[0]
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(S7ErrorCodes.ErrorFromDevice + ReceiveBuffer[0]), Job = j };
                        OnJobExecuted(eJob);
                    }
                    if (ReceiveBuffer.Count >= 4 + JobRespLen)
                        ReceiveBuffer.RemoveRange(0, 4 + JobRespLen);
                    if (ReceiveBuffer.Count > 0 && (JobRespLen % 2 > 0))
                    {
                        ReceiveBuffer.RemoveRange(0, 1);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// When strange error occours, data to write was not sent, so data were not moved to TagsListOnWriting. During OnJobExecuted, restoring data from TagsListOnWriting to TagsListToWrite failed
        /// </summary>
        /// <param name="ListJobPending"></param>
        private void RestoreTemporaryPendingWriteToMainTagsListToWrite(List<CommJob> ListJobPending)
        {
            // reset job's internal variable 
            Parallel.ForEach(ListJobPending, job =>
            {                
                if (job.Type == LinkType.InputOutput || job.Type == LinkType.UnconditionalOutput || job.Type == LinkType.ExceptionOutput) 
                {
                    lock (job.retLockList())
                    {
                        while (((S7TCPCommJob)job).tmpTagsListToWrite.Count > 0)
                        {
                            if (!job.TagsListToWrite.Contains(((S7TCPCommJob)job).tmpTagsListToWrite[0]))
                                job.TagsListToWrite.Add(((S7TCPCommJob)job).tmpTagsListToWrite[0]);
                            ((S7TCPCommJob)job).tmpTagsListToWrite.RemoveAt(0);
                        }
                    }
                }
            });
        }

        public override void CalculateJobStatistic(CommJob job)
        {
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


        #endregion

        #region methods

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            // on error reset array of bit flags management
            if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
            {
                // on error reset array of bit management's flags
                if (((S7TCPCommJob)e.Job).ArrayBoolWriteCheck)
                    ((S7TCPCommJob)e.Job).ResetArrayBoolWriteManualCheck();                
            }

            // Statistics management
            e.Job.ExchangedByte = ((S7TCPCommJob)e.Job).S7CommJobExchangedByte;
            e.Job.ExchangedTag = ((S7TCPCommJob)e.Job).S7CommJobExchangedTag;           
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
