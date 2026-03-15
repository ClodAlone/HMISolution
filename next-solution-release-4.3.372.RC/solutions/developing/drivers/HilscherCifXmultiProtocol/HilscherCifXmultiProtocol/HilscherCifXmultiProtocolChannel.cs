using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.IO;
using System.Threading;

namespace HilscherCifXmultiProtocol
{
    public enum HilscherErrorCodes : int
    {
        ErrorBusStateOff = 1000,
        ErrorAddressOutOfRange = 1001
    }

    class HilscherCifXmultiProtocolChannel : Channel
    {
        
        #region Constructors

        /// <summary>
        /// Initializes the HilscherCifXmultiProtocolChannel object.
        /// </summary>
        public HilscherCifXmultiProtocolChannel(CommunicationDriver commdriver, HilscherCifXmultiProtocolChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _BoardNumber = settings.BoardNumber;
            _ChannelNumber = settings.ChannelNumber;
            _ForceOutputDataAtStartup = settings.ForceOutputDataAtStartup;
            hilscherDriverInitialized = false;
            hilscherBoardInitialized = false;
        }

        #endregion

        #region Data Members

        CifXDevMngr chCifXDevMngr = new CifXDevMngr();
        protected object lockStream = new object();
        bool hilscherDriverInitialized = false;
        bool hilscherBoardInitialized = false;
        int hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
        int inputAreaSize = 0;
        int outputAreaSize = 0;

        #endregion

        #region Abstracts Methods

        public override bool TestChannelComm()
        {
            bool retValue = DeviceOpen();
            if(retValue)
            {
                DeviceClose();
            }
            return retValue;
        }

        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool returnValue = (hilscherDriverInitialized && hilscherBoardInitialized);
                SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }
        }

        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                if (!hilscherDriverInitialized)
                {
                    hilscherLastError = chCifXDevMngr.xDriverOpen();
                    if (hilscherLastError == CifXDevMngr.CIFX_NO_ERROR)
                    {
                        hilscherDriverInitialized = true;
                    }
                    else
                    {
                        return (false);
                    }
                }

                if (!hilscherBoardInitialized)
                {
                    if (!SelectBoard())
                    {
                        DeviceClose();
                        return (false);
                    }

                    hilscherLastError = chCifXDevMngr.xChannelOpen(chCifXDevMngr.BoardName, (int)_ChannelNumber);
                    if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                    {
                        DeviceClose();
                        return (false);
                    }

                    hilscherBoardInitialized = true;

                    // Set output data at startup
                    if (ForceOutputDataAtStartup)
                    {
                        SetOutputDataAtStartup();
                    }

                    // Set the Hilscher board channel state to ready, if needed
                    UInt32 pulState = 0;
                    hilscherLastError = chCifXDevMngr.xChannelHostState(CifXDevMngr.CIFX_HOST_STATE_READ, ref pulState, Timeout);
                    if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                    {
                        DeviceClose();
                        return (false);
                    }
                    if (pulState != CifXDevMngr.CIFX_HOST_STATE_READY)
                    {
                        hilscherLastError = chCifXDevMngr.xChannelHostState(CifXDevMngr.CIFX_HOST_STATE_READY, ref pulState, Timeout);
                        if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                        {
                            DeviceClose();
                            return (false);
                        }
                    }

                    // FOGBUGZ 12127
                    hilscherLastError = chCifXDevMngr.xChannelIOInfo(CifXDevMngr.CIFX_IO_INPUT_AREA, 0);
                    if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                    {
                        DeviceClose();
                        return (false);
                    }
                    inputAreaSize = chCifXDevMngr.IOAreaSize;
                    hilscherLastError = chCifXDevMngr.xChannelIOInfo(CifXDevMngr.CIFX_IO_OUTPUT_AREA, 0);
                    if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                    {
                        DeviceClose();
                        return (false);
                    }
                    outputAreaSize = chCifXDevMngr.IOAreaSize;
                }

                return (true);
            }
        }

        public override bool DeviceClose()
        {
            lock (lockStream)
            {
                if (hilscherBoardInitialized)
                {
                    UInt32 pulState = 0;
                    chCifXDevMngr.xChannelHostState(CifXDevMngr.CIFX_HOST_STATE_NOT_READY, ref pulState, Timeout);
                    chCifXDevMngr.xChannelClose();
                    hilscherBoardInitialized = false;
                }

                if (hilscherDriverInitialized)
                {
                    chCifXDevMngr.xDriverClose();
                    hilscherDriverInitialized = false;
                }
                return (true);
            }
        }

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            //bool bForceProcessListJobs = false;
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
                    // FOGBUGZ 12127
                    //if (nextjob != null)
                    if ((nextjob != null) && nextjob.IsValid)
                        ListJobPending.Add(nextjob);
                }

                if (nextjob != null)
                {
                    // FOGBUGZ 12127
                    if (nextjob.IsValid)
                    {

                        if (!IsDeviceOpen())
                            DeviceOpen();

                        ExecuteJob(nextjob);

                    // FOGBUGZ 12127
                    }
                    else
                    {
                        UnsubscribeJob(nextjob);
                    }
 
                }

                lock (lockThreadObject)
                {
                    if (ListJobPending.Count > 0)
                    {
                        foreach (var job in ListJobPending)
                        {
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

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                {
                    DeviceClose();
                }

                if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle, false))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                    {
                        break;
                    }
                }
                StopWorkerThread.WaitOne(0, false);
            }
        }

        public override void ExecuteJob(CommJob job)
        {
            HilscherCifXmultiProtocolCommJob hJob = job as HilscherCifXmultiProtocolCommJob;
            if (hJob == null)
            {
                return;
            }

            if (job.IsPending == true)
            {
                return;
            }

            base.ExecuteJob(job);

            hJob.ExecutedInOutputMode = false;
            ExchangeData(hJob);
            hJob.LastExecutionTime = DateTime.UtcNow;
            hJob.StartExecutionTime = DateTime.UtcNow;
        }

        #endregion

        #region Specific Methods

        bool SelectBoard()
        {
            int currentBoardIndex = 0;
            while ((currentBoardIndex <= (int)_BoardNumber) &&
                  ((hilscherLastError = chCifXDevMngr.xDriverEnumBoards(currentBoardIndex)) == CifXDevMngr.CIFX_NO_ERROR))
            {
                currentBoardIndex++;
            }

            if (hilscherLastError == CifXDevMngr.CIFX_NO_ERROR)
            {
                return (true);
            }

            return (false);
        }

        bool GetBusState()
        {
            UInt32 pulState = 0;
            hilscherLastError = chCifXDevMngr.xChannelBusState(CifXDevMngr.CIFX_BUS_STATE_GETSTATE, ref pulState, (UInt32)Timeout);
            if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
            {
                return(false);
            }
            if (pulState == CifXDevMngr.CIFX_BUS_STATE_ON)
            {
                return(true);
            }
            return (false);
        }

        void SetOutputDataAtStartup()
        {
            foreach (Station st in CommDriver.GetChannelStations(this))
            {
                HilscherCifXmultiProtocolStation hSt = st as HilscherCifXmultiProtocolStation;
                hSt.SetOutputDataAtStartup();
            }
        }

        protected void ExchangeData(HilscherCifXmultiProtocolCommJob hJob)
        {
            if (hJob.Type == DriverCodeBase.Enumerators.LinkType.Input ||
               (hJob.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
               hJob.TagsListToWrite.Count == 0))
            {
                ReadData(hJob);
            }
            else
            {
                WriteData(hJob);
            }
        }

        protected void ReadData(HilscherCifXmultiProtocolCommJob hJob)
        {
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs();
                    eAJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
                    eAJob.Job = hJob;
                    hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
                    OnJobExecuted(eAJob);
                    return;
                }
            }

            if (!GetBusState())
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs();
                eAJob.Job = hJob;
                if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                {
                    eAJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
                }
                else
                {
                    eAJob.ErrorCode = (DriverErrorCodes)HilscherErrorCodes.ErrorBusStateOff;
                }
                hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
                OnJobExecuted(eAJob);
                return;
            }

            // FOGBUGZ 12127
            if (!hJob.addressChecked)
            {
                hJob.addressChecked = true;
                int maxSize = outputAreaSize;
                if (hJob.Type == DriverCodeBase.Enumerators.LinkType.Input)
                {
                    maxSize = inputAreaSize;
                }
                if ((hJob.DataAddress + hJob.TotalJobSize) > maxSize)
                {
                    hJob.SetIsValidState(false);
                    ExecutedJobArgs eAJob = new ExecutedJobArgs();
                    eAJob.Job = hJob;
                    eAJob.ErrorCode = (DriverErrorCodes)HilscherErrorCodes.ErrorAddressOutOfRange;
                    hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
                    ((HilscherCifXMultiProtocolDriver)CommDriver).LastOutOfRangeAddress = hJob.DataAddress + hJob.TotalJobSize - 1;
                    ((HilscherCifXMultiProtocolDriver)CommDriver).MaxOutOfRangeAddress = (UInt32) maxSize;
                    OnJobExecuted(eAJob);
                    return;
                }
            }

            byte[] Answer;
            Answer = new byte[hJob.TotalJobSize];

            if (hJob.Type == DriverCodeBase.Enumerators.LinkType.InputOutput)
            {
                hilscherLastError = chCifXDevMngr.xChannelIOReadSendData(0, (int)hJob.DataAddress, (int)hJob.TotalJobSize, ref Answer); 
            }
            else
            {
                hilscherLastError = chCifXDevMngr.xChannelIORead(0, (int)hJob.DataAddress, (int)hJob.TotalJobSize, ref Answer); 
            }

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            if (hilscherLastError == CifXDevMngr.CIFX_NO_ERROR)
            {
                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                eJob.Values = Answer;
            }
            else
            {
                eJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
            }
            eJob.Job = hJob;
            OnJobExecuted(eJob);
        }

        private static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        protected void WriteData(HilscherCifXmultiProtocolCommJob hJob)
        {
            hJob.ExecutedInOutputMode = true;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs();
                    eAJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
                    eAJob.Job = hJob;
                    hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
                    OnJobExecuted(eAJob);
                    return;
                }
            }

            if (!GetBusState())
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs();
                if (hilscherLastError != CifXDevMngr.CIFX_NO_ERROR)
                {
                    eAJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
                }
                else
                {
                    eAJob.ErrorCode = (DriverErrorCodes)HilscherErrorCodes.ErrorBusStateOff;
                }
                eAJob.Job = hJob;
                hilscherLastError = CifXDevMngr.CIFX_NO_ERROR;
                OnJobExecuted(eAJob);
                return;
            }

            // FOGBUGZ 12127
            if (!hJob.addressChecked)
            {
                hJob.addressChecked = true;
                int maxSize = outputAreaSize;
                if ((hJob.DataAddress + hJob.TotalJobSize) > maxSize)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs();
                    eAJob.ErrorCode = (DriverErrorCodes)HilscherErrorCodes.ErrorAddressOutOfRange;
                    eAJob.Job = hJob;
                    hJob.SetIsValidState(false);
                    ((HilscherCifXMultiProtocolDriver)CommDriver).LastOutOfRangeAddress = hJob.DataAddress + hJob.TotalJobSize - 1;
                    ((HilscherCifXMultiProtocolDriver)CommDriver).MaxOutOfRangeAddress = (UInt32)maxSize;
                    OnJobExecuted(eAJob);
                    return;
                }
            }

            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();
            lock (hJob.retLockList())
            {
                listToWrite.AddRange(hJob.TagsListToWrite);
                hJob.TagsListToWrite.Clear();
            }

            if (hJob.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            {
                if (listToWrite.Count == 0)
                    listToWrite.AddRange(hJob.TagsList);
            }
            if (listToWrite.Count == 0)
            {
                return;
            }
                
            listToWrite.Sort(CompareTagByOffset);
            Tag cand = null;
            UInt16 nData = 0;
            do
            {
                if (listOnWriting.Count != 0)
                {
                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];
                listToWrite.Remove(cand);
                if (!listOnWriting.Contains(cand))
                    listOnWriting.Add(cand);
                nData += (UInt16)(hJob.ElementNumber == 0 ? cand.Size : hJob.GetProtocolDataByteSize());
            } while (listToWrite.Count > 0);
            uint dataAddress;
            byte[] dataBuffer = null;
            lock (hJob.retLockList())
            {
                
                //if ((hJob.Type == LinkType.ExceptionOutput || hJob.Type == LinkType.InputOutput))
                //{
                //    List<Tag> TemplistToWrite = (from tag in listOnWriting.AsParallel()
                //                                 where !(StatusCode.IsGood(tag.Value.StatusCode) && 
                //                                         (tag.LastValue == null) && 
                //                                         (hJob.Station.RewritingOfTheSameValue == false) &&
                //                                         tag.LastValue.Equals(cand.Value.Value))
                //                                 select tag).ToList();
                //    if (TemplistToWrite.Count < 1)
                //    {
                //        return;
                //    }
                //    listOnWriting.Clear();
                //    listOnWriting.AddRange(TemplistToWrite);
                //} 

                listOnWriting.ForEach((tag) =>
                {
                    if (!hJob.TagsListOnWriting.Contains(tag))
                        hJob.TagsListOnWriting.Add(tag);
                });       

                dataAddress = hJob.DataAddress;
                dataAddress += listOnWriting[0].ByteOffset;
                dataBuffer = new byte[nData];
                int j = 0;

                for (int i = 0; i < listOnWriting.Count; i++)
                {                    
                    listOnWriting[i].LastValue = listOnWriting[i].Value.Value;
                    uint k = listOnWriting[i].Size;
                    listOnWriting[i].GetTagBuffer(ref dataBuffer, false, j, (hJob.ElementNumber > 0 ? hJob.GetProtocolDataByteSize() : 0));
                    if(k > 1)
                    {
                        if (hJob.SwapBytes)
                        {
                            CommJob.SwapByteBuffer(ref dataBuffer, j, (int)k);
                        }

                        if (hJob.SwapWords)
                        {
                            CommJob.SwapWordBuffer(ref dataBuffer, j, (int)k);
                        }
                    }
                    j += (int)k;
                }
            }
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            hilscherLastError = chCifXDevMngr.xChannelIOWrite(0, (int)dataAddress, (int)nData, ref dataBuffer);
            if (hilscherLastError == CifXDevMngr.CIFX_NO_ERROR)
            {
                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
            }
            else
            {
                eJob.ErrorCode = (DriverErrorCodes)hilscherLastError;
            }
            eJob.Job = hJob;
            OnJobExecuted(eJob);
        }

        #endregion

        #region Properties

        private uint _BoardNumber;
        public uint BoardNumber
        {
            get
            {
                return _BoardNumber;
            }
            set
            {
                _BoardNumber = value;
            }
        }

        private uint _ChannelNumber;
        public uint ChannelNumber
        {
            get
            {
                return _ChannelNumber;
            }
            set
            {
                _ChannelNumber = value;
            }
        }

        private bool _ForceOutputDataAtStartup;
        public bool ForceOutputDataAtStartup
        {
            get
            {
                return _ForceOutputDataAtStartup;
            }
            set
            {
                _ForceOutputDataAtStartup = value;
            }
        }

        #endregion
    }
}
