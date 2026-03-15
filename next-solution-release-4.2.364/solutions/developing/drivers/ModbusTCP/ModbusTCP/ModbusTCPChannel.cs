using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using IpDriverCodeBase;
using System.Threading;

namespace ModbusTCP
{
    class ModbusTCPChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public ModbusTCPChannel(CommunicationDriver commdriver, ModbusTCPChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _TurnaroundDelay = settings.TurnaroundDelay;
        }

        #endregion

        UInt16 TransactionID = 0;

        #region Override Methods
        /*
         * serial methods rule...
        public override void DeviceOpen()
        {
            base.DeviceOpen();
        }*/

        /*public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToRead()
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToWrite()
        {
            throw new NotImplementedException();
        }
        */

        public override void ExecuteJob(CommJob job)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ExecuteJob begin", DateTime.Now.ToLongTimeString()));
            /*
             * so che tipo di frame devo preprare.
             * chiedo alla classe protocollo il messaggio da inviare
             * calcolo crc e spedisco
             */
            ModbusTCPCommJob mJob = job as ModbusTCPCommJob;
            if (mJob == null)
            {
                return;
            }
            
            var lkJob = mJob.retLockList();
            
            uint Truedim = 0;
            byte[] pdu = null;
            uint count = 0;
            bool isBroadcast = false;
            ModbusProtocol P = new ModbusProtocol();
            if (job.IsPending == true)
            {
                return;
            }
            lock (lkJob)
            {
                base.ExecuteJob(job);
            
                uint dim = P.GetFrameLength(mJob);
                pdu = new byte[dim];
                
                try
                {
                    Truedim = P.PrepareRequest(mJob, ref pdu, out isBroadcast);
                }
                catch
                {
                    Truedim = 0;
                }
            }
            if (Truedim == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }
            byte[] buf = new byte[Truedim + 6];
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }

            ++TransactionID;
            buf[count++] = Convert.ToByte(TransactionID >> 8);
            buf[count++] = Convert.ToByte(TransactionID & 0xff);
            buf[count++] = (byte)0;
            buf[count++] = (byte)0;
            buf[count++] = Convert.ToByte(Truedim >> 8);
            buf[count++] = Convert.ToByte(Truedim);
            Array.Copy(pdu, 0, buf, count, Truedim);
            count += Truedim;

            if (!DeviceWrite(buf, count))
            {
                return;
            }

            if(!isBroadcast)
            {
                if (!BeginDeviceRead(259))
                {
                    return;
                }
                job.LastExecutionTime = DateTime.UtcNow;
            }
            else
            {
                // No reply expected
                job.StartExecutionTime = DateTime.UtcNow;
                job.LastExecutionTime = DateTime.UtcNow;
                // Wait for the Turnaround delay
                int waitBroadCastTime = (int)TurnaroundDelay;
                StopWorkerThread.WaitOne(waitBroadCastTime);
                lock (lockThreadObject)
                {
                    job.TagsListOnWriting.Clear();
                    RemovePendingJob(job);
                    job.IsPending = false;
                    // Empty the receive buffer (just in case the gateway has sent a reply)
                    ReceiveBuffer.Clear();
                    Flush();
                }
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = job;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                OnJobExecuted(eJob);
            }
        }

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {

                CommJob nextjob = null;
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    if (SynchroJob != null)
                        nextjob = SynchroJob;
                    else
                        nextjob = GetNextPendingJob();
                    if (nextjob != null)
                    {
                        ListJobPending.Add(nextjob);
                    }
                }

                if (nextjob != null)
                {
                    if (StopWorkerThread.WaitOne(sleepCycle))
                    {
                        break;
                    }
                    if (!IsDeviceOpen())
                        DeviceOpen();

                    ExecuteJob(nextjob);
                }

                lock (lockThreadObject)
                {
                    if (NewDataToAnlyze.WaitOne(0) || ReceiveBuffer.Count > 0)
                    {
                        NewDataToAnlyze.Reset();
                   
                        if (ListJobPending.Count > 0)
                        {
                            foreach (var job in ListJobPending)
                            {
                                if (ProcessNewData(job))
                                    ListJobExecuted.Add(job);
                            }

                            foreach (var job in ListJobExecuted)
                            {
                                job.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == job)
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
                                if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                {
                                    ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    ListJobPending[0].ResetSynchro.Reset();
                                }
                                //error
                                LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewData(ListJobPending[0]);
                                ListJobPending.RemoveAt(0);
                                ReceiveBuffer.Clear();
                                ManageTimeoutError();
                            }
                        }
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    if (Flush() == false)
                    {
                        DeviceClose();
                    }

                if(StopWorkerThread.WaitOne(1))
                {
                    break;
                }
            }
        }


        public override bool ProcessNewData(CommJob pendingjob)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("ProcessNewData Enter:{0}", DateTime.Now.ToLongTimeString()));
                
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                ReceiveBuffer.Clear();              
                if (Flush() == false)
                {
                    DeviceClose();
                }           

                // In case of perduring timeout error close the socket
                if ((_CloseSocketAfterErrorTimeout > 0) && (LastErrorCode == DriverErrorCodes.ErrorTimeOut))
                {
                    if(_LastTimeoutError == DateTime.MinValue)
                    {
                        _LastTimeoutError = DateTime.UtcNow;
                    }
                    if((DateTime.UtcNow - _LastTimeoutError).TotalMilliseconds > _CloseSocketAfterErrorTimeout)
                    {
                        _LastTimeoutError = DateTime.MinValue;
                        DeviceClose();
                    }
                }
                else
                {
                    // No timeout error --> Reset the time of the last timeout error 
                    _LastTimeoutError = DateTime.MinValue;
                }

                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

                return true;
            }
            /*
             * Minimum length = 6 (MODBUS TCP header) + 3 (Unit Identifier, Function Code, Exception Code or, at least, 1 data byte)
             */
            Int32 minLen = 9;
            lock (lockList)
            {
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    return false;
                }

                // No timeout error --> Reset the time of the last timeout error 
                _LastTimeoutError = DateTime.MinValue;

                if (ReceiveBuffer.Count < minLen)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }
                //steve 300811
                uint tid = Convert.ToUInt16(ReceiveBuffer[0] * 256 + ReceiveBuffer[1]);
                if (tid != TransactionID)
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    if (!BeginDeviceRead(259))
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }
                    return false;
                }
                if (ReceiveBuffer[2] != 0 || ReceiveBuffer[3] != 0)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }

                uint remaining = Convert.ToUInt16(ReceiveBuffer[4] * 256 + ReceiveBuffer[5]);
                if(remaining < 3)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }
                if ((remaining + 6) > ReceiveBuffer.Count)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }
                byte[] msg;
                msg = new byte[remaining];
                //
                /*if (ReceiveBuffer.Count == minLen)
                { 
                    int remaining = ReceiveBuffer[4] * 256 + ReceiveBuffer[5];
                    BeginDeviceRead(remaining);
                    return false;
                }
                */

                byte replycode = 0;
                byte errorcode = 0;
                ExecutedJobArgs eJob = new ExecutedJobArgs();

                //steve 300811
                //lock (lockStream)
/*                { steve 140911
                    if (!IsDeviceOpen() || !DeviceRead(msg, remaining))
                    {
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReadError;
                        ReceiveBuffer.Clear();
                        Flush();
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                        return true;
                    }
                    ReceiveBuffer.AddRange(msg);
                }*/
                //****

                replycode = ReceiveBuffer[7];
                

                if ((replycode & 0x80) > 0)
                {
                    //error
                    errorcode = ReceiveBuffer[8];
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)(ModbusProtocol.PROTOCOL_ERROR + errorcode);
                    ReceiveBuffer.RemoveRange(0, 9);
                    Flush();
                }
                else
                {
                    int count = 0;// Length from Unit ID to data
                    int totcount = 0;// Message total length
                    //waited chars...
                    switch (replycode)
                    { 
                        case 1://Read Coils
                        case 2://Read Discrete Input
                        case 3://Read Multiple Register
                        case 4://Read Input Register
                            count = ReceiveBuffer[8] + 2 + 1;
                            totcount = count + 6;
                            break;
                        case 5://Write Single Coil
                        case 6://Write Single Register
                        case 15://Write Multiple Coils
                        case 16://Write Multiple Register
                            count = 5 + 1;
                            totcount = count + 6;
                            break;
                        case 20://Read File Record
                        case 21://Write File Record
                            count = ReceiveBuffer[8] + 2 + 1;
                            totcount = count + 6;
                            break;
                        case 7://Read Excetion Status
                            count = 2 + 1;
                            totcount = count + 6;
                            break;
                        case 22://Mask Write Register
                            count = 7 + 1;
                            totcount = count + 6;
                            break;
                        default:
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorUnknownFunctionCode;
                            ReceiveBuffer.Clear();
                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                    }
                    //frame completa se...
                    if (ReceiveBuffer.Count < totcount)
                    {
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReadError;
                        ReceiveBuffer.Clear();
                        Flush();
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                        return true;
                    }
                    LastErrorMessage = "";
                    
                    //copio da Function code in avanti...
                    byte[] Answer;
                    Answer = new byte[count];
                    ReceiveBuffer.CopyTo(6, Answer, 0, count);
                    eJob.Values = Answer;

                    ReceiveBuffer.RemoveRange(0, totcount);
                    Flush();
                    //System.Diagnostics.Trace.TraceInformation("Processed data {0} :{1}", pendingjob.GroupString, DateTime.UtcNow.ToString("hh:mm:ss:ffff"));
                }

                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                return true;
            }
        }
        #endregion

        #region Properties

        private uint _TurnaroundDelay;
        public uint TurnaroundDelay
        {
            get { return _TurnaroundDelay; }
            set
            {
                _TurnaroundDelay = value;
            }
        }

        // In case of perduring timeout error close the socket
        private uint _CloseSocketAfterErrorTimeout = Properties.Settings.Default.CloseSocketAfterErrorTimeout;
        private DateTime _LastTimeoutError = DateTime.MinValue;

        #endregion

        #region methods

        #endregion
    }
}
