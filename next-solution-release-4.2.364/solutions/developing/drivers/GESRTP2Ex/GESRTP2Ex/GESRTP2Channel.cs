////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Channel.cs
//
// summary:	Implements the driver GESRTP2 channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Threading;
using Opc.Ua;
using System.Threading.Tasks;

namespace GESRTP2
{
    /// <summary>   Communication channel of the GESRTP2 driver. </summary>
    public class GESRTP2Channel : TcpChannelList
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the GESRTP2Channel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Channel(CommunicationDriver commdriver, GESRTP2ChannelSettings settings)
            : base(commdriver, settings)
        {
            //IsConnect = false;
        }

        #endregion

        public enum ProcessNewDataStates : ushort
        {
            WaitPdu,
            WaitData,
            ReceivedPduData,
        }

        public UInt16 mInvokeId;
        public byte mSeq;

        // data area data type only
        private ProcessNewDataStates chProcessNewDataState;
        private EncapsPDU chServicePdu = new EncapsPDU(ReqTypes.ALL_TYPES);
        private List<byte> chReceiveBuffer = new List<byte>();
        public int chWaitBytes;    

        #region Override Methods

        public override bool DeviceWrite(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceWrite(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceWrite(Buffer, Count);

        }
        public override bool DeviceRead(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceRead(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceRead(Buffer, Count);
        }

        public override bool DeviceOpen()
        {
            base.DeviceOpen();
            if (IsDeviceOpen())
            {
                if (Connect() != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                {
                    DeviceClose();
                    return false;
                }
                if (Session() != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                {
                    DeviceClose();
                    return false;
                }
                return true;
            }

            return false;
        }

        public override void ResetNewDataEvent()
        {
            if (chWaitBytes == 0)
                base.ResetNewDataEvent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Starts sending phase of GESRTP2CommJob. DeviceWrite starts sending the frame.
        /// </summary>
        ///
        /// <param name="job">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            
            bool result = false;
            if (GESRTP2Protocol.IsSymbolic(list))
                result = ExecuteJobListSymbolic(ref conn, list);
            else
                result = ExecuteJobListDataArea(ref conn, list);
            return result;
        }
            
        private bool ExecuteJobListDataArea(ref DriverErrorCodes conn, List<CommJob> list)
        {
            GESRTP2CommJob.GEState state = CheckGEJobsState(list);            
            switch (state) 
            {
                case GESRTP2CommJob.GEState.DataAreaPolling:
                    // if WaitByte > 0, BeginDeviceRead() was launched previously in the ProcessNewData (same job) to wait data
                    if (chWaitBytes > 0)
                        return true;

                    foreach (CommJob job in list)
                        base.ExecuteJob(job);
                                        
                    if (!GESRTP2Protocol.PrepareRequestDataArea(this, list, out byte[] pdu, out chWaitBytes))
                    {
                        foreach (CommJob job in list)
                            RemovePendingJob(job);
                        list.Clear();

                        // don't wait any answer from device
                        return false;
                    }

                    if (!DeviceWrite(pdu))
                    {
                        conn = DriverErrorCodes.ErrorTimeOut;
                        return false;
                    }

                    if (!BeginDeviceRead(chWaitBytes))
                    {
                        conn = DriverErrorCodes.ErrorTimeOut;
                        return false;
                    }
                    break;
            }

            return true;
        }

        private bool ExecuteJobListSymbolic(ref DriverErrorCodes conn, List<CommJob> list)
        {            
            GESRTP2CommJob.GEState state = CheckGEJobsState(list);
            switch (state)
            {
                case GESRTP2CommJob.GEState.SymbolicGetDir:
                    {
                        // if WaitByte > 0, BeginDeviceRead() was launched previously in the ProcessNewData (same job) to wait data
                        if (chWaitBytes > 0)
                            return true;

                        foreach (CommJob job in list)
                            base.ExecuteJob(job);

                        if (!GESRTP2Protocol.PrepareRequestSymbolicDir(this, list[0] as GESRTP2CommJob, out byte[] pdu, out chWaitBytes))
                        {
                            foreach (CommJob job in list)
                                RemovePendingJob(job);
                            list.Clear();

                            // don't wait any answer from device
                            return false;
                        }

                        if (!DeviceWrite(pdu))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        if (!BeginDeviceRead(chWaitBytes))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicGetInfo:
                    {
                        // if WaitByte > 0, BeginDeviceRead() was launched previously in the ProcessNewData (same job) to wait data
                        if (chWaitBytes > 0)
                            return true;

                        foreach (CommJob job in list)
                            base.ExecuteJob(job);

                        if (!GESRTP2Protocol.PrepareRequestSymbolicInfo(this, list, out byte[] pdu, out chWaitBytes))
                        {
                            foreach (CommJob job in list)
                                RemovePendingJob(job);
                            list.Clear();

                            // don't wait any answer from device
                            return false;
                        }

                        if (!DeviceWrite(pdu))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        if (!BeginDeviceRead(chWaitBytes))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicPolling:
                    {
                        // if WaitByte > 0, BeginDeviceRead() was launched previously in the ProcessNewData (same job) to wait data
                        if (chWaitBytes > 0)
                            return true;

                        foreach (CommJob job in list)
                            base.ExecuteJob(job);

                        if (!GESRTP2Protocol.PrepareRequestSymbolicReadWriteVars(this, list, out byte[] pdu, out chWaitBytes))
                        {
                            foreach (CommJob job in list)
                                RemovePendingJob(job);
                            list.Clear();

                            // don't wait any answer from device
                            return false;
                        }
                        
                        if (!DeviceWrite(pdu))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }

                        if (!BeginDeviceRead(chWaitBytes))
                        {
                            conn = DriverErrorCodes.ErrorTimeOut;
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            if (GESRTP2Protocol.IsSymbolic(jobList))
                return false;
            else
                return true;    // data area address manage only 1 job at time
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                int rack = 0;
                int slot = 0;
                bool process = true;
                ReadWriteListLimitateSize frameSize = new ReadWriteListLimitateSize();

                //append a new list to the end of exList
                List<CommJob> list = new List<CommJob>();
                while (jobIndex < jobList.Count)
                {
                    GESRTP2CommJob j = jobList.ElementAt(jobIndex) as GESRTP2CommJob;
                    if (GESRTP2Protocol.IsSymbolic(j))
                    {
                        if (first)
                        {
                            write = !j.ReadRequest();
                            station = j.Station.Name;
                            rack = ((GESRTP2Station)j.Station).Rack;
                            slot = ((GESRTP2Station)j.Station).Slot;
                            first = false;
                        }

                        process = true;
                        // only 1 job at time --> protocol returns a unique error code for all the tags to be written
                        if (j.InErrorState)
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
                            if (j.Station.Name == station && ((GESRTP2Station)j.Station).Rack == rack && ((GESRTP2Station)j.Station).Slot == slot)
                            {
                                if (write && !j.ReadRequest())
                                {
                                    if (GESRTP2Protocol.getSymbolicReadWriteListLimitate(j, write, ref frameSize))
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
                                    if (!write && j.ReadRequest())
                                    {
                                        if (GESRTP2Protocol.getSymbolicReadWriteListLimitate(j, write, ref frameSize))
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
                    else
                    {
                        if (list.Count == 0)
                        {
                            // only 1 job at time
                            list.Add(j);
                            jobList.RemoveAt(jobIndex);
                            break;
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }
        #endregion

        private void ResetStateAndJobs(List<CommJob> list, bool fatalError = false)
        {
            mSeq++;
            mInvokeId++;
            chWaitBytes = 0;            
            chReceiveBuffer.Clear();
            chProcessNewDataState = ProcessNewDataStates.WaitPdu;

            if (fatalError)
            {
                if (GESRTP2Protocol.IsSymbolic(list))
                    ResetRunTimeSymbolicParameters();
            }
        }

        public void ResetRunTimeSymbolicParameters()
        {
            foreach (GESRTP2Station st in CommDriver.GetChannelStations(this))
                st.ResetRunTimeSymbolicParameters();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Perform the step of receive of the task GESRTP2CommJob. Until have been received all
        /// the data continue the reception exiting with false. When all data have been received execute
        /// OnJobExecuted(eJob) and going out with true.
        /// </summary>
        ///
        /// <param name="pendingjob">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {                
                ResetStateAndJobs(list, true);
                foreach (CommJob job in list)
                    OnJobExecuted(new ExecutedJobArgs { ErrorCode = conn, Job = job });

                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                DeviceClose();
                return true;
            }

            bool result = false;
            if (GESRTP2Protocol.IsSymbolic(list))
                result = ProcessNewDataListSymbolic(conn, list);
            else
                result = ProcessNewDataListDataArea(conn, list);
            return result;
        }

        private bool ProcessNewDataListDataArea(DriverErrorCodes conn, List<CommJob> list)
        {
            lock (lockThreadObject)
            {
                chReceiveBuffer.AddRange(ReceiveBuffer);
                chWaitBytes -= ReceiveBuffer.Count();
                ReceiveBuffer.Clear();
            }

            GESRTP2ErrorCodes error;
            GESRTP2CommJob mJob = list[0] as GESRTP2CommJob;
            if (chProcessNewDataState == ProcessNewDataStates.WaitPdu)
            {
                if (chReceiveBuffer.Count < GESRTP2Protocol.Pdu_SIZE)
                {
                    ResetNewDataEvent();
                    BeginDeviceRead(chWaitBytes);
                    return false;
                }

                chServicePdu.Expand((mJob.onWrite ? ReqTypes.WRITE_SMEM: ReqTypes.READ_SMEM), chReceiveBuffer.GetRange(0, GESRTP2Protocol.Pdu_SIZE).ToArray());
                if (chServicePdu.Mailbox.Typ == TrafficTypes.InvalidRequestNack)
                {
                    ResetStateAndJobs(list, IsFatalPduError(((GESRTP2Station)list[0].Station).PlcType, chServicePdu));
                    OnJobExecuted(new ExecutedJobArgs { ErrorCode = GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorReceiveNack, chServicePdu.Mailbox.Min, chServicePdu.Mailbox.Maj), Job = mJob });
                    DeviceClose();

                    return true;
                }
                chReceiveBuffer.RemoveRange(0, GESRTP2Protocol.Pdu_SIZE);
                chProcessNewDataState = ProcessNewDataStates.WaitData;
            }

            if (chProcessNewDataState == ProcessNewDataStates.WaitData)
            {
                if (GESRTP2Protocol.JobByteSize(mJob) > GESRTP2Protocol.ImediateBufferSize && !mJob.onWrite && (chServicePdu.Mailbox.Typ != TrafficTypes.CompletionAck && chReceiveBuffer.Count < GESRTP2Protocol.JobByteSize(mJob)))
                {
                    ResetNewDataEvent();
                    BeginDeviceRead(chWaitBytes);
                    return false;
                }
                else
                {
                    chProcessNewDataState = ProcessNewDataStates.ReceivedPduData;
                }
            }

            if (chProcessNewDataState == ProcessNewDataStates.ReceivedPduData)
            {
                if (mJob.onWrite)
                {
                    if (chServicePdu.Mailbox.Typ != TrafficTypes.CompletionAck)
                        error = GESRTP2ErrorCodes.ErrorUnexpectedWriteReply;
                    else
                        error = ReadPduOk(chServicePdu);
                }
                else
                {
                    mJob.ResetreadData();

                    // check pdu type and invokeId 
                    error = ReadPduOk(chServicePdu);
                    if (error == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                        error = ReadResponseDataArea(mJob);
                }

                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = (DriverErrorCodes)error;
                eJob.Job = mJob;
                if (!mJob.onWrite)
                    eJob.Values = mJob.readData.ToArray();
                // reset communication state only 
                ResetStateAndJobs(list);

                OnJobExecuted(eJob);

                if (error != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError && error != GESRTP2ErrorCodes.ErrorReceiveNack)
                    DeviceClose();

                return true;
            }

            return false;
        }

        private bool ProcessNewDataListSymbolic(DriverErrorCodes conn, List<CommJob> list)
        {
            lock (lockThreadObject)
            {
                chReceiveBuffer.AddRange(ReceiveBuffer);
                chWaitBytes -= ReceiveBuffer.Count();
                ReceiveBuffer.Clear();
            }

            List<GESRTP2ErrorCodes> errors;
            GESRTP2CommJob.GEState state = CheckGEJobsState(list);
            switch (state)
            {
                case GESRTP2CommJob.GEState.SymbolicGetDir:
                    if (chProcessNewDataState == ProcessNewDataStates.WaitPdu)
                    {
                        if (chReceiveBuffer.Count < GESRTP2Protocol.Pdu_SIZE)
                        {
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }

                        chServicePdu.Expand(ReqTypes.READ_DIR, chReceiveBuffer.GetRange(0, GESRTP2Protocol.Pdu_SIZE).ToArray());
                        if (chServicePdu.Mailbox.Typ != TrafficTypes.CompletionAckTB)
                        {
                            ResetStateAndJobs(list, IsFatalPduError(((GESRTP2Station)list[0].Station).PlcType, chServicePdu));
                            foreach (CommJob job in list)
                                OnJobExecuted(new ExecutedJobArgs { ErrorCode = GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorReceiveNack, chServicePdu.Mailbox.Min, chServicePdu.Mailbox.Maj), Job = job });
                            DeviceClose();

                            return true;
                        }

                        chReceiveBuffer.RemoveRange(0, GESRTP2Protocol.Pdu_SIZE);                        
                        chWaitBytes = (int)chServicePdu.DataLength;
                        chProcessNewDataState = ProcessNewDataStates.WaitData;
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.WaitData)
                    {
                        if (chWaitBytes > 0)
                        {
                            // wait data 
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }                    
                        else
                        {
                            chProcessNewDataState = ProcessNewDataStates.ReceivedPduData;
                        }
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.ReceivedPduData)
                    {
                        // check pdu type and invokeId 
                        errors = ReadPduOkList(chServicePdu, state, list);
                        if (errors[0] == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                            errors = ReadResponseSymbolic(chServicePdu, state, list);

                        if ((DriverErrorCodes)errors[0] != DriverErrorCodes.ErrorNoError)
                        { 
                            foreach (CommJob job in list)
                                OnJobExecuted(new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errors[0], Job = job });

                            if (errors[0] != GESRTP2ErrorCodes.ErrorReceiveNack)
                                DeviceClose();
                        }
                        else
                        {
                            // keep jobs alive 
                        }                        

                        ResetStateAndJobs(list);
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicGetInfo:                    
                    if (chProcessNewDataState == ProcessNewDataStates.WaitPdu)
                    {
                        if (chReceiveBuffer.Count < GESRTP2Protocol.Pdu_SIZE)
                        {
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }

                        chServicePdu.Expand(ReqTypes.READ_SYMBOL_LOOKUP, chReceiveBuffer.GetRange(0, GESRTP2Protocol.Pdu_SIZE).ToArray());
                        if (chServicePdu.Mailbox.Typ != TrafficTypes.CompletionAckTB)
                        {
                            ResetStateAndJobs(list, IsFatalPduError(((GESRTP2Station)list[0].Station).PlcType, chServicePdu));
                            foreach (CommJob job in list)
                                OnJobExecuted(new ExecutedJobArgs { ErrorCode = GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorReceiveNack, chServicePdu.Mailbox.Min, chServicePdu.Mailbox.Maj), Job = job });
                            DeviceClose();

                            return true;
                        }

                        chReceiveBuffer.RemoveRange(0, GESRTP2Protocol.Pdu_SIZE);
                        chWaitBytes = (int)chServicePdu.DataLength;
                        chProcessNewDataState = ProcessNewDataStates.WaitData;
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.WaitData)
                    {
                        if (chWaitBytes > 0)
                        {
                            // wait data 
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }
                        else
                        {
                            chProcessNewDataState = ProcessNewDataStates.ReceivedPduData;
                        }
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.ReceivedPduData)
                    {
                        List<CommJob> jobsNeedSymbolic = GESRTP2Protocol.SymbolicGetJobsNeedInfo(list);

                        // check pdu type and invokeId 
                        errors = ReadPduOkList(chServicePdu, state, jobsNeedSymbolic);
                        if (errors[0] == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                            errors = ReadResponseSymbolic(chServicePdu, state, jobsNeedSymbolic);
                        
                        for (int jobIndex = 0; jobIndex < jobsNeedSymbolic.Count; jobIndex++)
                        {
                            GESRTP2CommJob job = jobsNeedSymbolic[jobIndex] as GESRTP2CommJob;
                            if ((DriverErrorCodes)errors[jobIndex] != DriverErrorCodes.ErrorNoError)
                            {
                                OnJobExecuted(new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errors[jobIndex], Job = job });
                                // remove from job list --> in the same list jobs with valid device symbolic address could be presents
                                list.Remove(job);
                            }
                            else
                            {
                                // keep jobs alive 
                            }
                        }
                        
                        ResetStateAndJobs(list);

                        if (errors[0] != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError && errors[0] != GESRTP2ErrorCodes.ErrorReceiveNack)
                            DeviceClose();
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicPolling:
                    if (chProcessNewDataState == ProcessNewDataStates.WaitPdu)
                    {
                        if (chReceiveBuffer.Count < GESRTP2Protocol.Pdu_SIZE)
                        {
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }

                        chServicePdu.Expand((((GESRTP2CommJob)list[0]).onWrite ? ReqTypes.WRITE_ADDR_VAR : ReqTypes.READ_ADDR_VAR), chReceiveBuffer.GetRange(0, GESRTP2Protocol.Pdu_SIZE).ToArray());
                        if (!(chServicePdu.Mailbox.Typ == TrafficTypes.CompletionAck || chServicePdu.Mailbox.Typ == TrafficTypes.CompletionAckTB))
                        {                            
                            ResetStateAndJobs(list, IsFatalPduError(((GESRTP2Station)list[0].Station).PlcType, chServicePdu));
                            foreach (CommJob job in list)
                                OnJobExecuted(new ExecutedJobArgs { ErrorCode = GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorReceiveNack, chServicePdu.Mailbox.Min, chServicePdu.Mailbox.Maj), Job = job });
                            DeviceClose();
                            return true;
                        }

                        chReceiveBuffer.RemoveRange(0, GESRTP2Protocol.Pdu_SIZE);
                        chProcessNewDataState = ProcessNewDataStates.WaitData;
                        chWaitBytes = (int)chServicePdu.DataLength;

                        if (chWaitBytes == 0)                        
                            chReceiveBuffer.AddRange(chServicePdu.Mailbox.Data);
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.WaitData)
                    {
                        if (chWaitBytes > 0)
                        {
                            // wait data 
                            ResetNewDataEvent();
                            BeginDeviceRead(chWaitBytes);
                            return false;
                        }
                        else
                        {
                            chProcessNewDataState = ProcessNewDataStates.ReceivedPduData;
                        }
                    }

                    if (chProcessNewDataState == ProcessNewDataStates.ReceivedPduData)
                    {
                        //bool deviceClose = false;
                        // check pdu type and invokeId 
                        errors = ReadPduOkList(chServicePdu, state, list);
                        if (errors[0] == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                            errors = ReadResponseSymbolic(chServicePdu, state, list);

                        for (int jobIndex = 0; jobIndex < list.Count; jobIndex++)
                        {
                            GESRTP2CommJob job = list[jobIndex] as GESRTP2CommJob;
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.ErrorCode = (DriverErrorCodes)errors[jobIndex];
                            eJob.Job = job;
                            if ((DriverErrorCodes)errors[jobIndex] == DriverErrorCodes.ErrorNoError)
                            {
                                if (!job.onWrite)
                                    eJob.Values = job.readData.ToArray();
                            }
                            OnJobExecuted(eJob);
                        }

                        ResetStateAndJobs(list);

                        if (errors[0] != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError && errors[0] != GESRTP2ErrorCodes.ErrorReceiveNack)
                            DeviceClose();
                    }
                    break;
            }

            return false;
        }

        private GESRTP2ErrorCodes ReadResponseDataArea(GESRTP2CommJob job)
        {
            //if (chServicePdu.Mailbox.Seq != mSeq)
            //{
            //    return GESRTP2ErrorCodes.ErrorUnexpectedMailSeq;
            //}
            switch (chServicePdu.Mailbox.Typ)
            {
                case TrafficTypes.CompletionAck:
                    if (GESRTP2Protocol.JobByteSize(job) > GESRTP2Protocol.ImediateBufferSize)
                    {
                        return GESRTP2ErrorCodes.ErrorReceiveFewData;
                    }
                    byte[] buffer = new byte[GESRTP2Protocol.JobByteSize(job)];
                    Array.Copy(chServicePdu.Mailbox.Data, buffer, GESRTP2Protocol.JobByteSize(job));
                    job.readData.AddRange(buffer);
                    break;
                case TrafficTypes.CompletionAckTB:
                    if (GESRTP2Protocol.JobByteSize(job) > chReceiveBuffer.Count())
                    {
                        chReceiveBuffer.Clear();
                        return GESRTP2ErrorCodes.ErrorReceiveFewData;
                    }
                    job.readData.AddRange(chReceiveBuffer.GetRange(0, GESRTP2Protocol.JobByteSize(job)));
                    chReceiveBuffer.Clear();
                    break;
                case TrafficTypes.InvalidRequestNack:
                    return GESRTP2ErrorCodes.ErrorReceiveNack;
                default:
                    return GESRTP2ErrorCodes.ErrorUnexpectedReadReply;
            }

            return (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        private List<GESRTP2ErrorCodes> ReadResponseSymbolic(EncapsPDU pdu, GESRTP2CommJob.GEState state, List<CommJob> list)
        {
            List<GESRTP2ErrorCodes> results = new List<GESRTP2ErrorCodes>();
            
            switch (state)
            {
                case GESRTP2CommJob.GEState.SymbolicGetDir:
                    pdu.Mailbox.SymbolicDirread.Expand(ReqTypes.READ_DIR, chReceiveBuffer.ToArray());
                    if (chServicePdu.Mailbox.SymbolicDirread.HasADirName())
                    {
                        ((GESRTP2Station)((GESRTP2CommJob)list[0]).Station).Dir = pdu.Mailbox.SymbolicDirread.GetDirName();
                        results.Add((GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError);
                    }
                    else
                    {
                        results.Add(GESRTP2ErrorCodes.ErrorDirInvalid);
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicGetInfo:
                    pdu.Mailbox.SymbolicLookUp.Expand(ReqTypes.READ_SYMBOL_LOOKUP, chReceiveBuffer.ToArray());
                    for (int varIndex = 0; varIndex < list.Count; varIndex++)
                    {
                        GESRTP2CommJob job = list[varIndex] as GESRTP2CommJob;
                        pdu.Mailbox.SymbolicLookUp.GetSymbolicAddressInfo(varIndex, out UInt32 coherencyCookie, out byte[] internalAddress, out uint dataType, out UInt32 variableLength, out UInt32 arrayLength);
                        GESRTP2ErrorCodes error = job.CheckSymbolicDeviceAddressInfo(coherencyCookie, internalAddress, dataType, variableLength, arrayLength);
                        if ((DriverErrorCodes)error == DriverErrorCodes.ErrorNoError)
                            job.SetSymbolicAddressDeviceAddressInfo(coherencyCookie, internalAddress, (SymbolicDataType)dataType, variableLength, arrayLength);                        

                        results.Add(error);                                                
                    }
                    break;

                case GESRTP2CommJob.GEState.SymbolicPolling:
                    if (((GESRTP2CommJob)list[0]).onWrite)
                    {
                        foreach (GESRTP2CommJob job in list)
                            results.Add((GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError);
                    }
                    else
                    {
                        UInt32 offset = 0;
                        foreach (GESRTP2CommJob job in list)
                        {                            
                            job.SetSymbolicreadData(chReceiveBuffer, ref offset);
                            results.Add((GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError);
                        }
                    }                    
                    break;

            }            

            return results;
        }

        #region methods
        private byte[] GESRTP2Read()
        {
            byte[] buffer = new byte[GESRTP2Protocol.Pdu_SIZE];
            if (DeviceRead(buffer))
            {
                EncapsPDU Pdu = new EncapsPDU(ReqTypes.SESSION_CONTROL);
                Pdu.Expand(ReqTypes.SESSION_CONTROL, buffer);
                if (Pdu.DataLength > 0)
                {
                    byte[] bufferData = new byte[Pdu.DataLength];
                    if (DeviceRead(bufferData))
                    {
                        List<byte> retVal = new List<byte>(buffer);
                        retVal.AddRange(bufferData);
                        return retVal.ToArray();
                    }
                    else
                        return new byte[0];
                }
                return buffer;
            }
            else
                return new byte[0];
        }

        /// <summary>   Connect . </summary>
        /// 
        private GESRTP2ErrorCodes Connect()
        {
            GESRTP2ErrorCodes Error = (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
            // prepare ConnectRequest message
            byte[] buffer = EncapsPDUFactory.Connect().Pack();
            if (DeviceWrite(buffer))
            {
                buffer = GESRTP2Read();
                if (buffer.Length == GESRTP2Protocol.Pdu_SIZE)
                {
                    EncapsPDU Pdu = new EncapsPDU(ReqTypes.SESSION_CONTROL);
                    Pdu.Expand(ReqTypes.SESSION_CONTROL, buffer);
                    // check pdu type and invokeId 
                    if ((Pdu.PduType != PduTypes.ConnectResponse) ||
                        (Pdu.InvokeId != 0) || (Pdu.DataLength != 0))
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedConnectionReply;
                    if ((Pdu.Capability & (uint)Capabilities.SRTP_SRP_SERVER) == 0)
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedCapabilities;
                }
                else
                    Error = GESRTP2ErrorCodes.ErrorRxRead;
            }
            else
                Error = GESRTP2ErrorCodes.ErrorTxWrite;

            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            if (Error != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                DeviceClose();
            return Error;
        }
        private GESRTP2ErrorCodes Session()
        {
            GESRTP2ErrorCodes Error = (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
            mSeq = 1;
            mInvokeId = 1;
            List<Station> st = CommDriver.GetChannelStations(this);
            uint destination = (uint)(((GESRTP2Station)st[0]).Rack + (((GESRTP2Station)st[0]).Slot << 4)) + GESRTP2Protocol.DEFAULT_DESTINATION_2;
            byte[] buffer = EncapsPDUFactory.Session(SessionEnables.ESTABLISH, mSeq, mInvokeId, destination).Pack();
            if (DeviceWrite(buffer))
            {
                buffer = GESRTP2Read();
                if (buffer.Length == GESRTP2Protocol.Pdu_SIZE)
                {
                    EncapsPDU Pdu = new EncapsPDU(ReqTypes.SESSION_CONTROL);
                    Pdu.Expand(ReqTypes.SESSION_CONTROL, buffer);
                    // check pdu type and invokeId 
                    if ((Pdu.DataLength == 0) &&
                        (Pdu.Mailbox.Typ == TrafficTypes.CompletionAck))
                    {
                        Error = (ReadPduOk(Pdu));
                    }
                    else
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedSessionReply;

                }
                else
                    Error = GESRTP2ErrorCodes.ErrorRxRead;
            }
            else
                Error = GESRTP2ErrorCodes.ErrorTxWrite;

            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            if (Error == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                mSeq++;
                mInvokeId++;    // copied from KEPWare
            }
            else
            {
                DeviceClose();
            }

            return Error;
        }

        private bool IsFatalPduError(GESRTP2Protocol.PlcTypes plcTypes, EncapsPDU Pdu)
        {
            if (plcTypes == GESRTP2Protocol.PlcTypes.PacSystem)
            {
                return (
                    Pdu.Mailbox.Maj == (byte)GESRTP2Protocol.PAC_MAJOR_CODE_ERROR.QUEUE_FULL
                    || (Pdu.Mailbox.Maj == (byte)GESRTP2Protocol.PAC_MAJOR_CODE_ERROR.SRP_REQUEST_ERROR && Pdu.Mailbox.Min == (byte)GESRTP2Protocol.PAC_MINOR_CODE_SRP_REQUEST_ERROR.SRP_REQ_ERROR_POINT_FORMAT_OBSOLETE)
                    );

            }

            return false;
        }

        // check pdu type and invokeId 
        private GESRTP2ErrorCodes ReadPduOk(EncapsPDU Pdu)
        {
            if (Pdu.PduType != PduTypes.DataResponse)
            {
                return (GESRTP2ErrorCodes)GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorUnexpectedPduType, Pdu.Mailbox.Min, Pdu.Mailbox.Maj);
            }
            if (Pdu.InvokeId != mInvokeId)
            {
                return (GESRTP2ErrorCodes)GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorUnexpectedInvokeId, Pdu.Mailbox.Min, Pdu.Mailbox.Maj);
            }
            if (Pdu.Mailbox.Seq != mSeq)
            {
                return (GESRTP2ErrorCodes)GESRTP2Protocol.MergeMinMajErrorCodeToMainErrorCode((DriverErrorCodes)GESRTP2ErrorCodes.ErrorUnexpectedMailSeq, Pdu.Mailbox.Min, Pdu.Mailbox.Maj);
            }
            return (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        private List<GESRTP2ErrorCodes> ReadPduOkList(EncapsPDU Pdu, GESRTP2CommJob.GEState state, List<CommJob> list)
        {
            GESRTP2ErrorCodes error = ReadPduOk(Pdu);

            List<GESRTP2ErrorCodes> errors = new List<GESRTP2ErrorCodes>();
            foreach (CommJob job in list)
                errors.Add(error);

            return errors;
        }

        public GESRTP2CommJob.GEState CheckGEJobsState(List<CommJob> list)
        {
            GESRTP2CommJob.GEState state = GESRTP2CommJob.GEState.None;

            if (GESRTP2Protocol.IsSymbolic(list))
            {                
                foreach (GESRTP2CommJob job in list)
                {
                    if (job.GeState == GESRTP2CommJob.GEState.None)
                    {
                        job.GeState = GESRTP2CommJob.GEState.SymbolicGetDir;
                    }
                    if (job.GeState == GESRTP2CommJob.GEState.SymbolicGetDir)
                    { 
                        if (((GESRTP2Station)job.Station).HasDir())
                        {
                            job.GeState = GESRTP2CommJob.GEState.SymbolicGetInfo;
                        }
                        else
                        {
                            // send request
                        }
                    }
                    if (job.GeState == GESRTP2CommJob.GEState.SymbolicGetInfo)
                    {
                        // if all jobs has symbolic info to address symbolic vars 
                        if (job.HasSymbolicAddressDevideInfo())
                        {
                            job.GeState = GESRTP2CommJob.GEState.SymbolicPolling;
                        }
                        else
                        {
                            // send request
                        }
                    }

                    if (job.GeState == GESRTP2CommJob.GEState.SymbolicPolling)
                    {
                        // do nothing here                    
                    }
                }

                state = GESRTP2Protocol.GetLowerJobState(list);
            }
            else
            {
                state = GESRTP2CommJob.GEState.DataAreaPolling;
            }

            return state;
        }
        #endregion
    }
}

