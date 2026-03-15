using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Threading;
using DriverCodeBase.Enumerators;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetChannel : UdpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public OmronFinsEthernetChannel(CommunicationDriver commdriver, OmronFinsEthernetChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _SourceNetworkAddress = settings.SourceNetworkAddress;
            _SourceNode = settings.SourceNode;
            _SourceUnit = settings.SourceUnit;
        }

        #endregion

        #region Override Methods
        public override void ExecuteJob(CommJob job)
        {
            OmronFinsEthernetCommJob mJob = job as OmronFinsEthernetCommJob;
            if (mJob == null)
                return;

            if (job.IsPending == true)
            {
                return;
            }

            base.ExecuteJob(job);

            uint count = OmronFinsEthernetProtocol.GetFrameLength(mJob);
            byte[] pdu = new byte[count];
            count = OmronFinsEthernetProtocol.PrepareRequest(mJob, ref pdu, this);
            if(count == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }
            ReceiveBuffer.Clear();
            Flush();
            
            if (!DeviceWrite(pdu, count))
            {
                return;
            }

            BeginDeviceRead(100);

            job.LastExecutionTime = DateTime.UtcNow;
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            
            lock (lockList)
            {
                if (pendingjob == null ||
                    pendingjob.Station as OmronFinsEthernetStation == null )
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    return false;
                }

                if (ReceiveBuffer.Count < OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE)
                {
                    return false;
                }

                OmronFinsEthernetCommJob mJob = pendingjob as OmronFinsEthernetCommJob;
                if (mJob == null)
                {
                    Flush();
                    ReceiveBuffer.Clear();
                    return false;
                }

                OmronFinsEthernetStation s = pendingjob.Station as OmronFinsEthernetStation;

#if DEBUG
                System.Diagnostics.Debug.WriteLine("Debug SID - {0} - Station {1} - SID Req {2} - SID Rep {3} - {4}", DateTime.Now.ToString("HH:MM:ss.fff"), s.Name, s.GetSID(), ReceiveBuffer[OmronFinsEthernetProtocol.SID], string.Join(" ", ReceiveBuffer));
#endif

                //check sequence number
                if (!s.CheckSID(ReceiveBuffer[OmronFinsEthernetProtocol.SID]))
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    if (!BeginDeviceRead(100))
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)OmronFinsEthernetErrorCodes.ErrorSid;
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }
                    return false;
                }

                //check MRES and SRES
                if (ReceiveBuffer[OmronFinsEthernetProtocol.MRES] != 0 ||
                    (ReceiveBuffer[OmronFinsEthernetProtocol.SRES] & 0xBF) != 0)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)OmronFinsEthernetErrorCodes.ErrorEndCode;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }

                if (mJob.CommandCodeOnExecute() == CommandCodes.Read)
                {
                    if (ReceiveBuffer.Count < OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE + mJob.TotalJobSize)
                    {
                        return false;
                    }
                }

                LastErrorMessage = "";
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

                if (mJob.CommandCodeOnExecute() == CommandCodes.Read)
                {
                    // Copy received data
                    byte[] Answer;
                    uint copySize = (ushort)((mJob.isProtocolBool() && 
                        (mJob.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && mJob.ElementNumber == 0)) ? 
                        mJob.TotalJobSize * 8 : mJob.TotalJobSize);
;
                    if (mJob.AddressObj.DataFormat != DataFormats.Bit)
                        copySize += copySize % 2;
                    Answer = new byte[copySize];
                    ReceiveBuffer.CopyTo((int)OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE, Answer, 0, (int)copySize);
                    eJob.Values = Answer;
                }

                ReceiveBuffer.Clear();
                Flush();
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);

                return true;
            }
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
                SetStateCommandVariableBit(((!returnValue) || InErrorState()) , (UInt16)DriverCodeBase.Enumerators.ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
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
                    loop = 0;
                    // wait time
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
                    DeviceClose();
                
                // if no job was executed for more than XX times, wait 1msec to limit CPU usage
                if (nextjob == null)
                {
                    if (++loop > 4) {
                        loop = 0;
                        if (StopWorkerThread.WaitOne(1))
                            break;
                    }
                }
            }
        }

        #endregion

        #region Methods

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
                if (station.InErrorState && (station.GetChannel() == this))
                {
                        InErrorState = true;
                }
                else
                {
                    InErrorState = false;
                    break;
                }
            }
            return InErrorState;
        }

        #endregion

        #region Properties

        private byte _SourceNetworkAddress;
        public byte SourceNetworkAddress
        {
            get
            {
                return _SourceNetworkAddress;
            }
            set
            {
                _SourceNetworkAddress = value;
            }
        }

        private byte _SourceNode;
        public byte SourceNode
        {
            get
            {
                return _SourceNode;
            }
            set
            {
                _SourceNode = value;
            }
        }

        private byte _SourceUnit;
        public byte SourceUnit
        {
            get
            {
                return _SourceUnit;
            }
            set
            {
                _SourceUnit = value;
            }
        }
        
        
        #endregion
    }
}
