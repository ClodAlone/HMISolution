using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.Runtime.InteropServices;

namespace CoDeSys
{

    public enum CallingFunctions : int
    {
        Channel_ReadData,
        Channel_ReadData_OK,
        Channel_ReadData_Error,
        Channel_WaitingCoDeSysAutoRestartFromError,
        CoDeSysPLCHandlerWrapper_Disconnect,
    }

    public class CoDeSysChannel : Channel, IDisposable
    {
 
        #region Constructors

        /// <summary>
        /// Initializes the EtherNetIPChannel object.
        /// </summary>
        public CoDeSysChannel(CommunicationDriver commdriver, CoDeSysChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _DeviceName = settings.DeviceName;
            _DeviceAddress = settings.DeviceAddress;
            _PlcVersion = settings.PlcVersion;
            _PlcPort = settings.PlcPort;
            _CyclicListUdateRate = settings.CyclicListUdateRate;
            _Motorola = settings.Motorola;
            _BufferSize = settings.BufferSize;
            //_LogIn = settings.LogIn;
            _Protocol = settings.Protocol;
            _UseDirectReading = settings.UseDirectReading;
            _ActiveUseDirectReading = _UseDirectReading;
            _MaxDynamicAggregation = settings.MaxDynamicAggregation;
            _UsePing = settings.UsePing;
            _ConnectionType = settings.ConnectionType;
            _UserPLC = settings.UserPLC;
            _PasswordPLC = settings.PasswordPLC;
            _PasswordGateWay = settings.PasswordGateWay;

            _PLCHandler = new CoDeSysPLCHandlerWrapper();            
            _MapCycReadJobs = new Dictionary<string, List<CoDeSysCommJob>>();
        }

        #endregion

        #region Members

        public enum CoDeSysErrorCodes : int
        {
            ErrorWrapperNotInstalled = 10000,
            ErrorUnmappedTag,
            ErrorArraySizeBigger,
            ErrorWriteArraySizeSmaller,
            ErrorWrapperCannotCreateStation,
            ErrorConnectionBroken,
            ErrorNoUpdate,
            ErrorNoVariablListRead
        }
                       
        private CoDeSysPLCHandlerWrapper _PLCHandler;
        bool? _LastIsDeviceOpen = null;
        bool _LastDeviceOpen = false;
        private Dictionary<string, List<CoDeSysCommJob>> _MapCycReadJobs;
        #endregion

        #region Override Methods

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }

        List<CoDeSysCommJob> nextlist = new List<CoDeSysCommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();
        protected override void WorkingThread(object data)
        {            
            if (!InitPLCConnect())
                return;
            bool bDeviceOpen = false;

            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;

            int loop = 0;
            while (true)
            {
                nextlist.Clear();
                if (ListJobPending.Count == 0 || MultiPointProtocol) {
                    
                    ScheduleListJob();
                    lock (lockThreadObject)
                    {
                        if (SynchroJob != null)
                            GetNextSynchroJob(ref nextlist);                        
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }

                if (nextlist.Count > 0)
                {
                    WaitingCoDeSysAutoRestartFromError();
                    bDeviceOpen = IsDeviceOpen();
                    if (!bDeviceOpen)
                    {
                        //if (DeviceOpened())
                        SetJobsInError((DriverErrorCodes)CoDeSysChannel.CoDeSysErrorCodes.ErrorConnectionBroken);

                        DeviceClose();
                        bDeviceOpen = DeviceOpen();
                    }

                    lock (lockThreadObject)
                    {
                        if (bDeviceOpen)
                            ExecuteJobList(nextlist);
                    }
                }

                lock (lockThreadObject)
                {
                    if (ListJobPending.Count > 0)
                    {
                        ListJobExec.Clear();
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
                    DeviceClose();

                if (StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
            }

            ClosePLCConnect();
        }

        /// <summary>
        /// Retrieve info (type, lens, ecc) about tag's jobs directly from device 
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private CoDeSysProtocol.PLCHandlerErrors GetVarInfoFromDevice(ref CoDeSysCommJob j)
        {
            CoDeSysPLCHandlerWrapper.PlcSymbolDesc desc;

            // get info about tag
            CoDeSysProtocol.PLCHandlerErrors Ret = _PLCHandler.GetItem(j.ShortAddress, out desc);
            if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {                              
                bool ArrayType = false;
                bool StructType = false;
                uint ulTypeId = desc.ulTypeId;                

                // remove array and struct info to obtain data type (int, bool, ecc)                        
                if ((ulTypeId & ((uint)CoDeSysProtocol.VarType.VAR_TYPE_ARRAY)) == (uint)CoDeSysProtocol.VarType.VAR_TYPE_ARRAY)
                {
                    ulTypeId ^= (uint)(CoDeSysProtocol.VarType.VAR_TYPE_ARRAY);
                    ArrayType = true;
                }

                if ((ulTypeId & ((uint)CoDeSysProtocol.VarType.VAR_TYPE_STRUCT)) == (uint)CoDeSysProtocol.VarType.VAR_TYPE_STRUCT)
                {
                    ulTypeId ^= (uint)(CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);
                    StructType = true;
                }

                j.CoDeSysVarType = (CoDeSysProtocol.VarType)ulTypeId;
                if (CoDeSysProtocol.IsStringType(j.CoDeSysVarType))
                    j.StringLength = CoDeSysProtocol.StringConverterCalculateSizeFromItem(j.CoDeSysVarType, desc.ulSize);

                j.TotalJobSize = desc.ulSize;
                Parallel.ForEach(j.TagsList, t =>
                {
                    t.Size = desc.ulSize;
                });

                // check if the nr of arrays' element declared in movicon is the same of var declared into PLC
                if (ArrayType && !StructType)
                {
                    uint VarSize = CoDeSysProtocol.GetByteSizeOfVarType(j.CoDeSysVarType);
                    j.CoDeSysArrayDimension = desc.ulSize / VarSize;
                    if (j.CoDeSysArrayDimension != j.TagsList[0].TagNode.ArrayDimension)
                    {
                        string Warning = String.Format(Properties.Resources.ErrorCoDeSysVarArraySizeIncorrect, j.ShortAddress, j.CoDeSysArrayDimension, j.TagsList[0].TagNode.ArrayDimension);
                        CommDriver.OnSystemEvent(ObjectIds.Server, Warning, Opc.Ua.EventSeverity.High);
                    }

                    // if array dimension of Tag is bigger than dimension declared on device disable tag
                    if (j.CoDeSysArrayDimension < j.TagsList[0].TagNode.ArrayDimension)
                    {
                        // unmapped tag on device (--> not esixt) will never be scheduled
                        j.MappingErrorCode = (DriverErrorCodes)CoDeSysErrorCodes.ErrorArraySizeBigger;
                    }
                }
            }

            return Ret;
        }


        private void TryToResolveUndefinedVarType(ref CoDeSysCommJob j)
        {
            if (j.CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN && !j.MappingError)
            {
                CoDeSysProtocol.PLCHandlerErrors Ret = GetVarInfoFromDevice(ref j);
                if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK) 
                {
                    //if (!this.UseDirectReading)
                    //{
                    if (j.Type == LinkType.Input || j.Type == LinkType.InputOutput)
                    {
                        // more jobs can share the same address
                        if (!_MapCycReadJobs.ContainsKey(j.ShortAddress))
                            _MapCycReadJobs.Add(j.ShortAddress, new List<CoDeSysCommJob> { j });
                        else
                            _MapCycReadJobs[j.ShortAddress].Add(j);
                    }
                    //}
                }
                else if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_UNMAPPED_STATION || Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_OBJECT)
                {
                    // unmapped tag on device (--> don't exist) will never be scheduled
                    j.MappingErrorCode = (DriverErrorCodes)CoDeSysErrorCodes.ErrorUnmappedTag;
                }
            }
        }

        /// <summary>
        /// Verify is tag is present/mapped on device; if not mark it unmapped and will never managed later; on CycRead read mode, init map of tags to be defined on device
        /// String/WString tag will be set with real size
        /// </summary>
        public void CheckTagsOnDevice()
        {            
            if (CommDriver.GetChannelStations(this).Count == 0)
                return;

            CoDeSysStation s = (CoDeSysStation)CommDriver.GetChannelStations(this)[0];
            CoDeSysProtocol.PLCHandlerState State = _PLCHandler.GetState();
            if (State == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
            {
                List<CommJob> JobsList = s.GetListWholeJobCopy();
                for (var i = 0; i < JobsList.Count; i++) {
                    CoDeSysCommJob j = (CoDeSysCommJob)JobsList[i];
                    TryToResolveUndefinedVarType(ref j);                    
                }
            }
        }
                

        // Always return true --> CoDeSysWrapper required a special/complex initilization --> look InitPlcConnection
        public override bool TestChannelComm()
        {
            // do not anything here 
            return true;
        }

        /// <summary>
        /// Init CoDeSys wrapper library and 
        /// </summary>
        /// <returns></returns>
        public bool InitPLCConnect()
        {
            // 0 == no limit to job aggregation
            if (this.MaxDynamicAggregation == 0)
                this.MaxDynamicAggregation = uint.MaxValue;

            /// Init CoDeSys wrapper library
            _PLCHandler.Init();

            string Error = string.Empty;
            // Get handle of station for CoDeSys Wrapper
            if (_PLCHandler.Create() != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                // fatal error
                CoDeSysStation s = (CoDeSysStation)CommDriver.GetChannelStations(this)[0];

                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorCoDeSysWrapperCannotCreateStation, s.Name), Opc.Ua.EventSeverity.High);
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                SetJobsInError((DriverErrorCodes)CoDeSysChannel.CoDeSysErrorCodes.ErrorWrapperCannotCreateStation);
                
                return false;
            }

            //enable wrapper to ping plc before to establish connection (to speed up operation with remote device)
            if (_UsePing)
                _PLCHandler.EnableToPingHostBeforeConnection(this.Timeout);

            DeviceOpen();
            if (!IsDeviceOpen())
                SetJobsInError((DriverErrorCodes)CoDeSysChannel.CoDeSysErrorCodes.ErrorConnectionBroken);
            
            return true;
        }

        /// <summary>
        /// When a read/write error occoured, CoDeSys disconnect and riconnect automatically; try to wait some time (TimeOut) 
        /// until State return to CoDeSysProtocol.PLCHandlerState.STATE_RUNNING; if some CycleDefineVar as defined, remove it
        /// </summary>
        /// <returns></returns>
        private void WaitingCoDeSysAutoRestartFromError()
        {
            // if no error, don't check
            if (LastErrorCode == 0)
                return;

            CoDeSysProtocol.PLCHandlerState State = _PLCHandler.GetState();
            if (State != CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
            {
                // remove subribed item's list from library
                if (_PLCHandler.IsCycDefineVarListDefine())
                {
                    _PLCHandler.CycLeaveVarAccess(CallingFunctions.Channel_WaitingCoDeSysAutoRestartFromError);
                    _PLCHandler.CycDeleteVarList(CallingFunctions.Channel_WaitingCoDeSysAutoRestartFromError);
                }

                DateTime StartWaitCycle = DateTime.UtcNow;
                // wait some time (TimeOut) to verify if CoDeSys return automatically to RUNNING
                while (State != CoDeSysProtocol.PLCHandlerState.STATE_RUNNING && DateTime.UtcNow.Subtract(StartWaitCycle).TotalMilliseconds < this.Timeout)
                {                    
                    if (StopWorkerThread.WaitOne(200))
                        return;

                    State = _PLCHandler.GetState();
                }
            }
        }
        
        public override bool IsDeviceOpen()
        {
            CoDeSysProtocol.PLCHandlerState State = _PLCHandler.GetState();

            bool IsOpen = (DeviceOpened() && State == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING);

            if (_LastIsDeviceOpen == null || (IsOpen != (bool)_LastIsDeviceOpen))
            {
                SetStateCommandVariableBit(!IsOpen, (UInt16)ChannelVariableBits.ChannelUnconnected);
                foreach (var station in CommDriver.GetChannelStations(this))
                    ((CoDeSysStation)station).SetStateCommandVariableBit(!IsOpen, (UInt16)StationVariableBits.StationErrorState);

                _LastIsDeviceOpen = IsOpen;
            }

            return IsOpen;
        }                
        
        public override bool DeviceOpen()
        {
            if((ConnectionType == CoDeSysChannelSettings.CONNECTION.DIRECT) && 
               (string.IsNullOrWhiteSpace(DeviceAddress) ))
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorConnectionParameter, Name), EventSeverity.Low);
                return (false);
            }
            else if ((ConnectionType == CoDeSysChannelSettings.CONNECTION.GATEWAY) &&
               (string.IsNullOrWhiteSpace(DeviceAddress) || string.IsNullOrWhiteSpace(DeviceName)))
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorConnectionParameter, Name), EventSeverity.Low);
                return (false);
            }
            CoDeSysProtocol.PLCHandlerErrors ret = _PLCHandler.CheckConnectionAndRunningState(this.DeviceName, (ulong)this.ConnectionType, this.DeviceAddress, this.PlcPort, this.UserPLC, this.PasswordPLC, this.PasswordGateWay, StopWorkerThread);
            if (ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- DeviceOpen  {1}  ", DateTime.Now.ToString("hh:mm:ss.fff"), Name);
                if (!this.UseDirectReading)
                    CheckTagsOnDevice();
            }
            else if(ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_RECONNECTTHREAD_STILL_ACTIVE)
            {
                _PLCHandler.Release();

                if (this.MaxDynamicAggregation == 0)
                    this.MaxDynamicAggregation = uint.MaxValue;

                /// Init CoDeSys wrapper library
                _PLCHandler.Init();

                string Error = string.Empty;
                // Get handle of station for CoDeSys Wrapper
                if (_PLCHandler.Create() != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                {
                    // fatal error
                    CoDeSysStation s = (CoDeSysStation)CommDriver.GetChannelStations(this)[0];

                    CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorCoDeSysWrapperCannotCreateStation, s.Name), Opc.Ua.EventSeverity.High);
                    SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                    SetJobsInError((DriverErrorCodes)CoDeSysChannel.CoDeSysErrorCodes.ErrorWrapperCannotCreateStation);

                    return false;
                }
                if (_UsePing)
                {
                    _PLCHandler.EnableToPingHostBeforeConnection(this.Timeout);
                }

                ret = _PLCHandler.CheckConnectionAndRunningState(this.DeviceName, (ulong)this.ConnectionType, this.DeviceAddress, this.PlcPort, this.UserPLC, this.PasswordPLC, this.PasswordGateWay, StopWorkerThread);
                if (ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                {
                    System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- DeviceOpen  {1}  ", DateTime.Now.ToString("hh:mm:ss.fff"), Name);
                    if (!this.UseDirectReading)
                        CheckTagsOnDevice();
                }
            }

            bool IsOpen = IsDeviceOpen();

            if (!IsOpen && _LastDeviceOpen)                
                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.StationInError, CommDriver.DriverName, CommDriver.GetChannelStations(this)[0].Name, Properties.Resources.DeviceDisconnected), EventSeverity.Low);
            _LastDeviceOpen = IsOpen;

            return IsOpen;
        }

        private bool DeviceOpened()
        {
            return (_PLCHandler.ConnectState == CoDeSysPLCHandlerWrapper.ConnectionState.Connected);
        }

        public void ResetJobs()
        {            
            if (CommDriver.GetChannelStations(this).Count == 0)
                return;
            
            CoDeSysStation s = (CoDeSysStation)CommDriver.GetChannelStations(this)[0];
            
            List<CommJob> list = s.GetListWholeJobCopy();
            Parallel.ForEach(list, job =>
            {
                ((CoDeSysCommJob)job).ResetCoDeSysElement();
                lock (job.retLockList())
                    job.IsPending = false;
            });


            if (!this.UseDirectReading)
            {
                System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- ResetJobs Calling MapCycReadJobs.Clear {1}  ", DateTime.Now.ToString("hh:mm:ss.fff"), Name);
                _MapCycReadJobs.Clear();
            }
        }

        public override bool DeviceClose()
        {
            System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- DeviceClose  {1}  ", DateTime.Now.ToString("hh:mm:ss.fff"), Name);
            ResetJobs();
                
            _PLCHandler.Disconnect();

            // reset active data exchange method to initial settings
            this.ActiveUseDirectReading = UseDirectReading;

            return true;
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if (StatisticsData != null)
            {
                // set exchanged quantity of data for specific job
                DiagnLastTaskRxBytes = ((CoDeSysCommJob)e.Job).DiagnRxBytes;
                DiagnLastTaskTxBytes = ((CoDeSysCommJob)e.Job).DiagnTxBytes;
                ((CoDeSysCommJob)e.Job).ResetDiagnRxTxBytes();
            }
            base.OnJobExecuted(e);            
        }

        /// <summary>
        /// Close connection and release all CoDeSysWrapper handle
        /// </summary>
        public void ClosePLCConnect()
        {
            _PLCHandler.Disconnect();

            // Release handle of station for CoDeSys Wrapper
            _PLCHandler.Release();
        }

        protected void SetJobsInError(DriverErrorCodes error)
        {
            foreach (var station in CommDriver.GetChannelStations(this))
                ((CoDeSysStation)station).SetGeneralError(error);

            LastErrorCode = error;
        }
        #endregion

        #region methods
        protected void GetNextPendingList(ref List<CoDeSysCommJob> list)
        {
            bool write = false;
            string station = string.Empty;
            bool first = true;

            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return;

                // limit the nr max of vars 
                while (!queue.IsEmpty && list.Count() < this.MaxDynamicAggregation) {
                    CoDeSysCommJob j = queue.ElementAt(0) as CoDeSysCommJob;
                    // ignore job with unmapped tag on device (checked on DeviceOpen)
                    if (j != null && j.TagsListOnWriting.Count == 0)
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
                                list.Add(j);
                        }
                        else
                        {                            
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                                list.Add(j);
                        }

                    }
                    CommJob dequeuedJob = null;
                    queue.TryDequeue(out dequeuedJob);
                }
            }
        }

        private void GetNextSynchroJob(ref List<CoDeSysCommJob> list)
        {
            CoDeSysCommJob job = SynchroJob as CoDeSysCommJob;

            list.Add(job);
        }
                
        protected void ReadData(List<CoDeSysCommJob> list)
        {            
            CoDeSysProtocol.PLCHandlerErrors Ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            Dictionary<string, CoDeSysPLCHandlerWrapper.VarParam> VarReadList = new Dictionary<string, CoDeSysPLCHandlerWrapper.VarParam>();
            List<CoDeSysPLCHandlerWrapper.VarValue> VarValueList = new List<CoDeSysPLCHandlerWrapper.VarValue>();            
            Dictionary<string, List<CoDeSysCommJob>> MapSyncReadJobs = new Dictionary<string, List<CoDeSysCommJob>>();
            CoDeSysPLCHandlerWrapper.VarParam VarP = null;

            CoDeSysStation s = list[0].Station as CoDeSysStation;
            if (s == null)
                return;
                        
            #region Init CycRead
            //bool LocalUseDirectReading = this.UseDirectReading;
            
            //init cycling read
            if (!this.ActiveUseDirectReading)
            {
                Ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

                // if not present, define the list of vars to be cycled continuously by CoDeSys 
                if (!_PLCHandler.IsCycDefineVarListDefine())
                {
                    if (_MapCycReadJobs.Count > 0)
                    {
                        List<CoDeSysPLCHandlerWrapper.VarParam> VarList = new List<CoDeSysPLCHandlerWrapper.VarParam>();
                        int CycVarID = 0;
                        foreach (List<CoDeSysCommJob> jobs in _MapCycReadJobs.Values)
                        {
                            CoDeSysCommJob j = jobs[0];
                            // store position of tag inside CycReadList for further access 
                            j.CycVarListID = CycVarID;
                            VarList.Add(new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, j.CycVarListID, VarList.Count));
                            CycVarID++;
                        }
                        Ret = _PLCHandler.CycDefineVarList(VarList, this.CyclicListUdateRate);
                    }
                }
                else
                {
                    // if some error occurs during init cycling read, read items with SyncRead
                    Ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                }

                // if some error occurs during init cycling read, read items with SyncRead
                if (Ret != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                    this.ActiveUseDirectReading = true;
            }

            // syncro jobs ara managed as syncro read
            if (!this.ActiveUseDirectReading)
            {
                // jobs list contain syncro jobs ?
                if (list.Count(job => job.SyncroExec) > 0)
                    this.ActiveUseDirectReading = true;
            }

            #endregion

            #region prepare for CoDeSys Wrapper the list of vars to be readed
            for (int i = 0; i < list.Count; i++)
            {
                CoDeSysCommJob j = (CoDeSysCommJob)list[i];
                //base.ExecuteJob(j);
                //System.Diagnostics.Debug.WriteLine("-- DEBUG -- OnJobExecuted IsPending = True {0} ", j.TagsList[0].TagNode.NodeId);

                TryToResolveUndefinedVarType(ref j);

                if (j.MappingError)
                {
                    // log error on db only 1st time
                    //if (!j.MappingErrorReported)
                    //{
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.ErrorCode = (DriverErrorCodes)j.MappingErrorCode;                        
                        eJob.Job = j;
                        OnJobExecuted(eJob);
                    //j.MappingErrorReported = true;
                    //}
                }
                else
                { 
                    //SynRead
                    if (this.ActiveUseDirectReading)
                    {                        
                        if (!MapSyncReadJobs.ContainsKey(j.ShortAddress))
                            MapSyncReadJobs.Add(j.ShortAddress, new List<CoDeSysCommJob>() { j });
                        else
                            MapSyncReadJobs[j.ShortAddress].Add(j);

                        if (!VarReadList.ContainsKey(j.ShortAddress))
                        {
                            VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, i, VarReadList.Count);
                            if (StatisticsData != null)
                                j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);
                            // increase send bytes diagnostic var
                            VarReadList.Add(j.ShortAddress, VarP);                            
                        }
                        base.ExecuteJob(j);
                    }
                    else
                    {                        
                        if (_MapCycReadJobs.ContainsKey(j.ShortAddress))
                        {
                            if (!VarReadList.ContainsKey(j.ShortAddress))
                            {
                                VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, j.CycVarListID, VarReadList.Count);
                                // increase send bytes diagnostic var
                                if (StatisticsData != null)
                                    j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);
                                VarReadList.Add(j.ShortAddress, VarP);
                            }
                            base.ExecuteJob(j);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- ReadData Step1 {1} - {2} ", DateTime.Now.ToString("hh:mm:ss.fff"), Name, j.ShortAddress);
                        }
                    }
                }
            }

            if (VarReadList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- ReadData Step2 {1}  ", DateTime.Now.ToString("hh:mm:ss.fff"),  Name);
                return;
            }
            #endregion 

            // Sync read
            if (this.ActiveUseDirectReading)
            {
                Ret = _PLCHandler.SyncReadVarsFromPlc(VarReadList.Values.ToList(), out VarValueList);
            }
            else //cycling read
            {                
                // if list of item present
                if (_PLCHandler.IsCycDefineVarListDefine())
                {
                    // lock plc to prevent memory corruption
                    Ret = _PLCHandler.CycEnterVarAccess(CallingFunctions.Channel_ReadData);
                    if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                    {
                        // read only defined subset of vars
                        //In asynchronous management, it is necessary to wait a few msec on the startup
                        DateTime StartWaitCycle = DateTime.UtcNow;
                        // wait some time (TimeOut) to verify if CoDeSys return automatically to RUNNING
                        while ( DateTime.UtcNow.Subtract(StartWaitCycle).TotalMilliseconds < this.Timeout)
                        {
                            Ret = _PLCHandler.CycReadVars(VarReadList.Values.ToList(), out VarValueList);
                            if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                            {
                                break;
                            }
                            StopWorkerThread.WaitOne(Properties.Settings.Default.AsyncReadWaitTimeNoUpdate);
                        }
                        
                        //Ret = _PLCHandler.CycReadVars(VarReadList.Values.ToList(), out VarValueList);
                        //        _PLCHandler.GetCysReadCallBackInfo(out bCallBackUpdated, out nCallBackNotifyCounter);
                        // release lock on PLC
                        _PLCHandler.CycLeaveVarAccess(CallingFunctions.Channel_ReadData_OK);
                    }
                }
            }

            switch (Ret) {
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:
                    LastErrorCode = (DriverErrorCodes)Ret;
                    break;
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_UPDATE:
                    //Cyclist list was defined and the update thread is running, but the first read of the variable list has not yet been finished.Try again later to read the variables
                    StopWorkerThread.WaitOne(Properties.Settings.Default.AsyncReadWaitTimeNoUpdate);
                    LastErrorCode = (DriverErrorCodes)CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                    System.Diagnostics.Debug.WriteLine("@@@ -- DEBUG -- ReadData Step3");
                    ResetReadPending(VarReadList, MapSyncReadJobs, (DriverErrorCodes)CoDeSysErrorCodes.ErrorNoUpdate);                   
                    return;                    
                default: //generic error                    
                    // error occurred on brutal disconnect --> in the next cycle rebuilt CycRead will re rebuilt
                    if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NO_CYCLIC_LIST_DEFINED)
                    {
                        _PLCHandler.CycLeaveVarAccess(CallingFunctions.Channel_ReadData_Error);
                        _PLCHandler.CycDeleteVarList(CallingFunctions.Channel_ReadData_Error);
                    }

                    string Aux = String.Format(Properties.Resources.ErrorCoDeSysWrapperGeneral, s.Name, Ret.ToString());
                    CommDriver.OnSystemEvent(ObjectIds.Server, Aux, Opc.Ua.EventSeverity.High);
                    System.Diagnostics.Debug.WriteLine("{0} - @@@ -- DEBUG -- ReadData Step4 {1} - {2}", DateTime.Now.ToString("hh:mm:ss.fff"), Name, Ret);
                    Ret = (CoDeSysProtocol.PLCHandlerErrors)CoDeSysProtocol.ShiftError(Ret);
                    LastErrorCode = (DriverErrorCodes)CoDeSysProtocol.ShiftError(Ret);
                    break;
            }

            if ((Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK) &&
                ((VarValueList == null) || (VarValueList.Count == 0)))
            {
                ResetReadPending(VarReadList, MapSyncReadJobs, (DriverErrorCodes)CoDeSysErrorCodes.ErrorNoVariablListRead);
                System.Diagnostics.Debug.WriteLine("@@@ -- DEBUG -- ReadData Step5");
                return;
            }

            foreach (CoDeSysPLCHandlerWrapper.VarParam Var in VarReadList.Values.ToList()) { 
                List<CoDeSysCommJob> JobsWithSameAddress = null;
                // retrieve list of jobs associated to the same var --> more job access to same CoDeSys var
                if (this.ActiveUseDirectReading)
                    MapSyncReadJobs.TryGetValue(Var.VarName, out JobsWithSameAddress);
                else
                    _MapCycReadJobs.TryGetValue(Var.VarName, out JobsWithSameAddress);

                if (JobsWithSameAddress != null) {
                    foreach (CoDeSysCommJob j in JobsWithSameAddress)
                    {                        
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.ErrorCode = (DriverErrorCodes)Ret;
                        if ((Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK) && (VarValueList.Count < Var.ValueIndex))
                        {
                            eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.ShiftError(CoDeSysProtocol.PLCHandlerErrors.RESULT_BUFFER_TOO_SMALL);
                        }
                        else if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                        {
                            CoDeSysPLCHandlerWrapper.VarValue DeviceValue = VarValueList[Var.ValueIndex];

                            if (StatisticsData != null)
                                // increase receive bytes diagnostic var
                                j.DiagnRxBytes += CoDeSysPLCHandlerWrapper.VarValue.SizeOf(DeviceValue);

                            if (DeviceValue.HasData())
                            {
                                eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                                if (CoDeSysProtocol.IsStringType(j.CoDeSysVarType))
                                {
                                    eJob.Values = CoDeSysProtocol.StringConverterFromByteData(j.CoDeSysVarType, DeviceValue.Data);
                                }
                                //else if (j.CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_STRUCT)
                                //{
                                //    int o = 0;
                                //}
                                else if (j.TagsList[0].TagNode.ArrayDimension != 0)
                                {
                                    eJob.Values = DeviceValue.Data;
                                }
                                else
                                { // default --> number
                                    eJob.Values = DeviceValue.Data;
                                }
                            } else
                            {
                                System.Diagnostics.Debug.WriteLine("@@@ -- DEBUG -- ReadData Step6");
                            }                           
                        }

                        //base.ExecuteJob(j);
                        eJob.Job = j;
                        OnJobExecuted(eJob);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("@@@ -- DEBUG -- ReadData Step7");
                }
            }
        }

        protected void WriteData(List<CoDeSysCommJob> list) {

            CoDeSysProtocol.PLCHandlerErrors Ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            CoDeSysCommJob j = null;
            List<CoDeSysPLCHandlerWrapper.VarParam> VarList = new List<CoDeSysPLCHandlerWrapper.VarParam>();
            List<CoDeSysPLCHandlerWrapper.VarWrite> ValueList = new List<CoDeSysPLCHandlerWrapper.VarWrite>();
            CoDeSysPLCHandlerWrapper.VarParam VarP = null;

            CoDeSysStation s = list[0].Station as CoDeSysStation;
            if (s == null)
                return;
                        
            for (int i = 0; i < list.Count; i++)
            {
                CoDeSysPLCHandlerWrapper.VarWrite ValueWrite = null;
                j = list[i];

                base.ExecuteJob(j);

                // try to recognize undefined var type
                TryToResolveUndefinedVarType(ref j);

                #region check if job is valid
                if (j.MappingError)
                {                    
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)j.MappingErrorCode;
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    continue;
                }

                // check if element number is correct --> this operation can be perform only online base device data type is retrive by driver on fly
                if (!CoDeSysProtocol.IsValidElementNumber(j))
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    continue;
                }
                #endregion

                object objectData = null;
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                {
                    j.IsPending = false;
                    continue;
                }
                byte[] jobdata = (byte[])objectData;
                int lengthBuff = jobdata.Length;
                
                if (CoDeSysProtocol.IsStringType(j.CoDeSysVarType))
                {
                    byte[] StringData = CoDeSysProtocol.StringConverterToData(j.CoDeSysVarType, j.TotalJobSize, jobdata);
                    ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, StringData, i);                    
                }
                //else if (j.CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_STRUCT)
                //{
                //    int o = 0;
                //}
                else if (j.TagsList[0].TagNode.ArrayDimension != 0)
                {
                    //if (j.CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_STRUCT)
                    //{
                    //    int o = 0;
                    //}
                    //else
                    /*{ */

                    // simple array
                    // write an array with less elements than declared on device is possible but CoDeSys function fill missing element with random values --> not allowed --> only read
                    if (j.TagsList[0].TagNode.ArrayDimension < j.CoDeSysArrayDimension)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.ErrorCode = (DriverErrorCodes)CoDeSysErrorCodes.ErrorWriteArraySizeSmaller;
                        eJob.Job = j;
                        OnJobExecuted(eJob);

                        // don't write value
                        ValueWrite = null;
                    }
                    else
                    {
                        ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, jobdata, i);
                    }
                    //}
                }
                else
                { // default --> number
                    ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, jobdata, i);
                }

                if (ValueWrite != null)
                {                    
                    VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, i, VarList.Count);
                    VarList.Add(VarP);
                    // increase send bytes diagnostic var
                    if (StatisticsData != null)
                        j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);

                    ValueList.Add(ValueWrite);
                    // increase send bytes diagnostic var
                    if (StatisticsData != null)
                        j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarWrite.SizeOf(ValueWrite);
                }

                if (VarList.Count > CoDeSysProtocol.MAX_AGGREGATED_JOBS)
                    break;
                
            }

            if (VarList.Count == 0)
                return;

            //Write 
            Ret = _PLCHandler.SyncWriteVarValues(ValueList);          

            switch (Ret) { 
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:
                    LastErrorCode = (DriverErrorCodes)Ret;
                    break;
                default:
                    string Aux = String.Format(Properties.Resources.ErrorCoDeSysWrapperGeneral, s.Name, Ret.ToString());
                    CommDriver.OnSystemEvent(ObjectIds.Server, Aux, Opc.Ua.EventSeverity.High);
                    Ret = (CoDeSysProtocol.PLCHandlerErrors)CoDeSysProtocol.ShiftError(Ret);
                    LastErrorCode = (DriverErrorCodes)Ret;
                    break;
            }

            foreach (CoDeSysPLCHandlerWrapper.VarParam Var in VarList) {
                j = list[Var.VarIndex];

                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = (DriverErrorCodes)Ret;
                eJob.Job = list[Var.VarIndex];
                OnJobExecuted(eJob);
            }
        }

        protected void ExecuteJobList(List<CoDeSysCommJob> list)
        {
            for (int index = 0; index < list.Count; index++)
            {
                if (list[index].Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (list[index].TagsListToWrite.Count == 0)
                    {
                        list[index].TagsListToWrite.AddRange(list[index].TagsList);
                    }
                }
            }
            if (list[0].Type == DriverCodeBase.Enumerators.LinkType.Input || (list[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput && list[0].TagsListToWrite.Count == 0))
                ReadData(list);
            else
                WriteData(list);
        }

        private void ResetReadPending(Dictionary<string, CoDeSysPLCHandlerWrapper.VarParam> VarReadList, Dictionary<string, List<CoDeSysCommJob>> MapSyncReadJobs, DriverErrorCodes ErrorCode)
        {

            foreach (CoDeSysPLCHandlerWrapper.VarParam Var in VarReadList.Values.ToList())
            {
                List<CoDeSysCommJob> JobsWithSameAddress = null;
                // retrieve list of jobs associated to the same var --> more job access to same CoDeSys var
                if (this.ActiveUseDirectReading)
                    MapSyncReadJobs.TryGetValue(Var.VarName, out JobsWithSameAddress);
                else
                    _MapCycReadJobs.TryGetValue(Var.VarName, out JobsWithSameAddress);

                if (JobsWithSameAddress != null)
                {
                    System.Threading.Tasks.Parallel.ForEach(JobsWithSameAddress, j =>
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        j.MappingErrorCode = ErrorCode;
                        eJob.ErrorCode = (DriverErrorCodes)j.MappingErrorCode;
                        eJob.Job = j;
                        OnJobExecuted(eJob);
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("@@@ -- DEBUG -- ReadData Step8");
                }
            }
        }

        public CommunicationDriver getCommDriver()
        {
            return CommDriver;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Connection Type
        /// </summary>
        private CoDeSysChannelSettings.CONNECTION _ConnectionType;
        public CoDeSysChannelSettings.CONNECTION ConnectionType
        {
            get { return _ConnectionType; }
            set { _ConnectionType = value; }
        }

        /// <summary>
        /// Device Name
        /// </summary>
        private string _DeviceName;
        public string DeviceName
        {
            get { return _DeviceName; }
            set { _DeviceName = value; }
        }

        /// <summary>
        /// Device Address
        /// </summary>
        private string _DeviceAddress;
        public string DeviceAddress
        {
            get { return _DeviceAddress; }
            set { _DeviceAddress = value; }
        }

        /// <summary>
        /// Enter the CoDeSys PLC Version
        /// </summary>
        private CoDeSysProtocol.PlcVersion _PlcVersion;
        public CoDeSysProtocol.PlcVersion PlcVersion
        {
            get { return _PlcVersion; }
            set { _PlcVersion = value; }
        }

        /// <summary>
        /// Port Number
        /// </summary>
        private uint _PlcPort;
        public uint PlcPort
        {
            get { return _PlcPort; }
            set { _PlcPort = value; }
        }

        /// <summary>
        /// User PLC
        /// </summary>
        private string _UserPLC;
        public string UserPLC
        {
            get { return _UserPLC; }
            set { _UserPLC = value; }
        }

        // <summary>
        /// Password PLC
        /// </summary>
        private string _PasswordPLC;
        public string PasswordPLC
        {
            get { return _PasswordPLC; }
            set { _PasswordPLC = value; }
        }

        // <summary>
        /// Password GateWay
        /// </summary>
        private string _PasswordGateWay;
        public string PasswordGateWay
        {
            get { return _PasswordGateWay; }
            set { _PasswordGateWay = value; }
        }

        /// <summary>
        /// Cycling update rate
        /// </summary>
        private uint _CyclicListUdateRate;
        public uint CyclicListUdateRate
        {
            get { return _CyclicListUdateRate; }
            set { _CyclicListUdateRate = value; }
        }

        /// <summary>
        /// Motorola byte order = default value = False
        /// </summary>
        private bool _Motorola;
        public bool Motorola
        {
            get { return _Motorola; }
            set { _Motorola = value; }
        }

        /// <summary>
        /// Communication BufferSize
        /// </summary>
        private uint _BufferSize;
        public uint BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; }
        }

        /// <summary>
        /// LogIn to PLC
        /// </summary>
        private bool _LogIn;
        public bool LogIn
        {
            get { return _LogIn; }
            set { _LogIn = value; }
        }

        /// <summary>
        /// Protocol Type
        /// </summary>
        private CoDeSysProtocol.Protocol _Protocol;
        public CoDeSysProtocol.Protocol Protocol
        {
            get { return _Protocol; }
            set { Protocol = value; }
        }

        /// <summary>
        /// Enable to retrive data from PLC plling device
        /// </summary>
        private bool _UseDirectReading;
        public bool UseDirectReading
        {
            get { return _UseDirectReading; }
            set { _UseDirectReading = value; }
        }
                
        private bool _ActiveUseDirectReading;
        public bool ActiveUseDirectReading
        {
            get { return _ActiveUseDirectReading; }
            set { _ActiveUseDirectReading = value; }
        }

        /// <summary>
        /// Task aggregation threshold limit
        /// </summary>
        private uint _MaxDynamicAggregation;
        public uint MaxDynamicAggregation
        {
            get { return _MaxDynamicAggregation; }
            set { _MaxDynamicAggregation = value; }
        }

        /// <summary>
        /// Use ping to test presence of PLC 
        /// </summary>
        private bool _UsePing;
        public bool UsePing
        {
            get { return _UsePing; }
            set { _UsePing = value; }
        }


        #endregion
        
        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {

            if (StopWorkerThread != null)
                StopWorkerThread.Set();
            base.Dispose();

        }

        #endregion
    }
}

