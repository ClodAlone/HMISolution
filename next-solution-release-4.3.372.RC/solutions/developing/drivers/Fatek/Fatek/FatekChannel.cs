using System;
using System.Collections.Generic;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using IpDriverCodeBase;

namespace Fatek
{
    class FatekChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the FatekChannel object.
        /// </summary>
        public FatekChannel(CommunicationDriver commdriver, FatekChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        #endregion

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
            FatekCommJob mJob = job as FatekCommJob;
            if (mJob == null)
            {
                return;
            }

            if (job.IsPending == true)
                return;
                       
            base.ExecuteJob(job);

            List<byte> pdu = new List<byte>();
            uint Truedim = FatekProtocol.PrepareRequest(mJob, ref pdu);            
            if (Truedim == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }

            lock (lockList)
            {
                ReceiveBuffer.Clear();
            }
            
            // if write fail, try to handle "Unable to write data to the transport connection: An existing connection was forcibly closed by the remote host." error
            // try to redo DeviceOpen(), if write failed again, manage with TimeOut
            if (!DeviceWrite(pdu.ToArray(), (uint)pdu.Count))
            {
                bool writeOk = false;
                DeviceOpen();
                if (IsDeviceOpen())
                    writeOk = DeviceWrite(pdu.ToArray(), (uint)pdu.Count);
                
                if (!writeOk)
                {
                    // leave job into PendingJob to generatore a TimeOut (General Error)
                    job.IsPending = false;                    
                    return;
                }
            }
            
            if (!BeginDeviceRead(FatekProtocol.PROTOCOL_FRAME_MAX_SIZE))
            {
                return;
            }
            job.LastExecutionTime = DateTime.UtcNow;
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
                                lock (lockList)
                                {
                                    ReceiveBuffer.Clear();
                                }
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

                // if no job was executed for more than XX times, wait 1msec to limit CPU usage
                if (nextjob == null)
                {
                    if (++loop > 4)
                    {
                        loop = 0;
                        if (StopWorkerThread.WaitOne(1))
                            break;
                    }
                }
            }
        }


        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs timeoutJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(timeoutJob);
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
                        
            // No timeout error --> Reset the time of the last timeout error 
            _LastTimeoutError = DateTime.MinValue;

            DriverErrorCodes errorCode;
            byte[] dataBuffer;
            lock (lockThreadObject)
            {
                try
                {
                    errorCode = FatekProtocol.ParseAnswer((FatekCommJob)pendingjob, ReceiveBuffer, out dataBuffer);
                    //// reviced data incomplete --> wait more
                    //if (errorCode == (DriverCodeBase.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError)
                    //    return false;
                } 
                catch (Exception ex)
                {
                    errorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError;
                    dataBuffer = null;
                }
                ReceiveBuffer.Clear();
            }            

            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = pendingjob };
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                LastErrorMessage = string.Empty;
                eJob.Values = dataBuffer;
            }
            else
            {
                Flush();
            }

            OnJobExecuted(eJob);
            return true;            
        }
        #endregion

        #region Properties

        // In case of perduring timeout error close the socket
        private uint _CloseSocketAfterErrorTimeout = Properties.Settings.Default.CloseSocketAfterErrorTimeout;
        private DateTime _LastTimeoutError = DateTime.MinValue;

        #endregion

        #region methods

        #endregion
    }
}
