using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System.IO;
using System.Threading;

namespace CoDeSys
{
    public class CoDeSysChannel : ChannelList, IDisposable
    {
 
        #region Constructors

        /// <summary>
        /// Initializes the CoDeSysChannel object.
        /// </summary>
        public CoDeSysChannel(CommunicationDriver commdriver, CoDeSysChannelSettings settings)
            : base(commdriver, settings)
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
            _ConnectionType = settings.ConnectionType;
            _UserPLC = settings.UserPLC;
            _PasswordPLC = settings.PasswordPLC;
            _PasswordGateWay = settings.PasswordGateWay;

            _PLCHandler = new CoDeSysPLCHandlerWrapper();            
            _MapCycReadJobs = new Dictionary<string, List<CoDeSysCommJob>>();

            // 0 == no limit to job aggregation
            if (MaxDynamicAggregation == 0)
                MaxDynamicAggregation = uint.MaxValue;
        }

        #endregion

        #region Members                       
        private CoDeSysPLCHandlerWrapper _PLCHandler;
        bool? _LastIsDeviceOpen = null;
        bool _LastDeviceOpen = false;
        private Dictionary<string, List<CoDeSysCommJob>> _MapCycReadJobs;        
        private int _MaxRetriesBeforeError = 1;
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
        
        /// <summary>
        /// Retrieve info (type, lens, ecc) about tag's jobs directly from device 
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private CoDeSysProtocol.PLCHandlerErrors GetVarInfoFromDevice(ref CoDeSysCommJob j)
        {
            // get info about tag
            CoDeSysProtocol.PLCHandlerErrors Ret = _PLCHandler.GetItem(j.ShortAddress, out CoDeSysPLCHandlerWrapper.PlcSymbolDesc desc);
            if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {                              
                bool ArrayType = false;
                bool StructType = false;
                uint ulTypeId = desc.ulTypeId;

                j.MappingErrorCode = DriverErrorCodes.ErrorNoError;

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
                        j.MappingErrorCode = (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorArraySizeBigger;
                    }
                }                
            }

            return Ret;
        }

        private CoDeSysProtocol.PLCHandlerErrors TryToResolveUndefinedVarType(ref CoDeSysCommJob j)
        {
            CoDeSysProtocol.PLCHandlerErrors Ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

            // try to get device variable parameters only first time (or non result ErrorUnmappedTag --> not present onn device)
            if (j.CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN && j.MappingErrorCode != (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorUnmappedTag)
            {
                Ret = GetVarInfoFromDevice(ref j);
                switch (Ret)
                {
                    case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:
                        if (j.Type == LinkType.Input || j.Type == LinkType.InputOutput)
                        {
                            // more jobs can share the same address
                            if (!_MapCycReadJobs.ContainsKey(j.ShortAddress))
                                _MapCycReadJobs.Add(j.ShortAddress, new List<CoDeSysCommJob> { j });
                            else
                                _MapCycReadJobs[j.ShortAddress].Add(j);
                        }
                        break;
                    case CoDeSysProtocol.PLCHandlerErrors.RESULT_UNMAPPED_STATION:
                    case CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_OBJECT:
                        // unmapped tag on device (--> don't exist) will never be scheduled
                        j.MappingErrorCode = (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorUnmappedTag;
                        break;
                    default:
                        j.MappingErrorCode = (DriverErrorCodes)Ret;
                        break;
                }
            }

            return Ret;
        }

        /// <summary>
        /// Verify is tag is present/mapped on device; if not mark it unmapped and will never managed later; on CycRead read mode, init map of tags to be defined on device
        /// String/WString tag will be set with real size
        /// </summary>
        private bool CheckTagsOnDevice()
        {
            CoDeSysStation s = (CoDeSysStation)CommDriver.GetChannelStations(this)[0];
            CoDeSysProtocol.PLCHandlerState State = _PLCHandler.GetState();
            if (State == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
            {
                List<CommJob> JobsList = s.GetListWholeJobCopy();
                for (var i = 0; i < JobsList.Count; i++)
                {
                    CoDeSysCommJob j = (CoDeSysCommJob)JobsList[i];
                    CoDeSysProtocol.PLCHandlerErrors Ret = TryToResolveUndefinedVarType(ref j);
                    // error during retrive info of tag
                    if (Ret != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK && j.MappingErrorCode != (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorUnmappedTag)
                    {                            
                        _MapCycReadJobs.Clear();
                        break;
                    }
                }
            }

            return (_MapCycReadJobs.Count > 0);
        }
                

        // Always return true --> CoDeSysWrapper required a special/complex initilization --> look InitPlcConnection
        public override bool TestChannelComm()
        {
            // do not anything here 
            return true;
        }

        private bool _IsDeviceOpen(int timeout = 0)
        {
            if (!_PLCHandler.IsPlcHandlerInitialized())
                return false;

            bool isOpen = (_PLCHandler.GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING);
            // is plc is not running wait for a while --> PLC connection/reconnection is managed by PLCHandler library
            if (!isOpen && timeout>0)
            {
                DateTime sc = DateTime.UtcNow;
                do
                {
                    // request to stop thread ? Exit immediatly                    
                    if (LocalWaitOne(Properties.Settings.Default.DeviceOpenInternalWait))
                        break;

                    isOpen = (_PLCHandler.GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING);
                }
                while (!(isOpen || DateTime.UtcNow.Subtract(sc).TotalMilliseconds >= timeout));
            }

            if (_LastIsDeviceOpen == null || (isOpen != (bool)_LastIsDeviceOpen))
            {
                SetStateCommandVariableBit(!isOpen, (UInt16)ChannelVariableBits.ChannelUnconnected);
                foreach (var station in CommDriver.GetChannelStations(this))
                    ((CoDeSysStation)station).SetStateCommandVariableBit(!isOpen, (UInt16)StationVariableBits.StationErrorState);

                _LastIsDeviceOpen = isOpen;
            }

            return isOpen;
        }

        public override bool IsDeviceOpen()
        {                       
            // get current state and exit immediatly
            return _IsDeviceOpen(0);
        }                
        
        public override bool DeviceOpen()
        {            
            if (!_PLCHandler.IsStationCreated())
            { 
                /// Init CoDeSys wrapper library
                _PLCHandler.Init();

                _PLCHandler.Create();
            }

            _MaxRetriesBeforeError = GetMaxRetriesBeforeError();

            CoDeSysProtocol.PLCHandlerErrors ret = _PLCHandler.CheckConnectionAndRunningState(this.DeviceName, (ulong)this.ConnectionType, this.DeviceAddress, this.PlcPort, this.UserPLC, this.PasswordPLC, this.PasswordGateWay, this.Timeout, _MaxRetriesBeforeError, this);

            // if current state is not Running wait for a while (use the same TimeOut passed to PLCHandler library)
            bool isOpen = _IsDeviceOpen(GetTotalTimeOut());

            if (!isOpen && _LastDeviceOpen)                
                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.StationInError, CommDriver.DriverName, CommDriver.GetChannelStations(this)[0].Name, ret.ToString()), EventSeverity.Low);
            _LastDeviceOpen = isOpen;

            return isOpen;
        }

        public void ResetAllJobsCoDeSysParameters()
        {
            if (!this.ActiveUseDirectReading)
            {
                if (_PLCHandler.IsCycDefineVarListDefine())
                { 
                    _PLCHandler.CycDeleteVarList();
                    CommDriver.OnSystemEvent(ObjectIds.Server, "called CycDeleteVarList", EventSeverity.Max);
                }
            }

            foreach (CoDeSysStation st in CommDriver.GetChannelStations(this))
            {
                List<CommJob> list = st.GetListWholeJobCopy();
                Parallel.ForEach(list, job =>
                {
                    ((CoDeSysCommJob)job).ResetCoDeSysElement();
                });
            }

            _MapCycReadJobs.Clear();

            // reset active data exchange method to initial settings
            this.ActiveUseDirectReading = UseDirectReading;
        }

        public override bool DeviceClose()
        {
            ResetAllJobsCoDeSysParameters();

            _PLCHandler.Release();

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

        protected void SetJobsInError(List<CommJob> list, DriverErrorCodes error)
        {
            foreach (var j in list)
            {
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = j;
                e.ErrorCode = error;
                OnJobExecuted(e);
            }        
        }      
                
        /// <summary>
        /// Wait some time (msec) : use a manual event to exit before in case driver is closing
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        private bool LocalWaitOne(int waitTime)
        {
            bool result = false;
            
            ManualResetEvent newDataToAnlyze = null;
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze == null)
                    NewDataToAnlyze = new ManualResetEvent(false);
                newDataToAnlyze = NewDataToAnlyze;
            }
            if (newDataToAnlyze != null)
            {
                newDataToAnlyze.Reset();
                //wait for answer
                if (newDataToAnlyze.WaitOne(waitTime))
                    result = true;
            }

            return result;
        }

        #endregion

        #region methods        

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return (jobList.Count == this.MaxDynamicAggregation);
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count())
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;

                List<CommJob> list = new List<CommJob>();
                #region fill a list with job as long as size not exceed mamimux frame size
                while (jobIndex < jobList.Count())
                {
                    CoDeSysCommJob j = jobList.ElementAt(jobIndex) as CoDeSysCommJob;
                    if (j != null)
                    {
                        if (first)
                        {
                            write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                        }
                        bool jwrite = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;

                        if (j.Station != null && j.Station.Name == station)
                        {
                            if (write && jwrite)
                            {
                                if (list.Count() < this.MaxDynamicAggregation)
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
                                    if (list.Count() < this.MaxDynamicAggregation)
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
                #endregion
                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }

        private void CycDefineVarList()
        {
            CoDeSysProtocol.PLCHandlerErrors ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

            if (_PLCHandler.IsCycDefineVarListDefine())
                _PLCHandler.CycDeleteVarList();

            _PLCHandler.ResetCycDefineVarListPointer();
            if (CheckTagsOnDevice())
            {
                List<CoDeSysPLCHandlerWrapper.VarParam> varList = new List<CoDeSysPLCHandlerWrapper.VarParam>();
                int CycVarID = 0;
                foreach (List<CoDeSysCommJob> jobs in _MapCycReadJobs.Values)
                {
                    // store position of tag inside CycReadList for further access 
                    jobs.ForEach(job => job.CycVarListID = CycVarID);
                    CoDeSysCommJob j = jobs[0];
                    varList.Add(new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, j.CycVarListID, varList.Count));
                    CycVarID++;
                }
                ret = _PLCHandler.CycDefineVarList(varList, this.CyclicListUdateRate);
            }
            else
            {
                // some error during get variables's parameters
                ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_BUSY;
            }

            // if some error occurs during init cycling read, read items with SyncRead
            if (ret != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                _PLCHandler.ResetCycDefineVarListPointer();
                this.ActiveUseDirectReading = true;                
            }
        }

        protected void ReadData(List<CommJob> list)
        {
            CoDeSysProtocol.PLCHandlerErrors ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            Dictionary<string, CoDeSysPLCHandlerWrapper.VarParam> varReadList = new Dictionary<string, CoDeSysPLCHandlerWrapper.VarParam>();
            List<CoDeSysPLCHandlerWrapper.VarValue> varValueList = new List<CoDeSysPLCHandlerWrapper.VarValue>();
            Dictionary<string, List<CoDeSysCommJob>> mapSyncReadJobs = new Dictionary<string, List<CoDeSysCommJob>>();
            CoDeSysPLCHandlerWrapper.VarParam VarP = null;
            bool syncroJobs = false;
            bool activeUseDirectReading = true;

            CoDeSysStation s = list[0].Station as CoDeSysStation;
            if (s == null)
                return;

            // syncro jobs ara managed as syncro read
            syncroJobs = (list.Count(job => job.SyncroExec) > 0);
            if (syncroJobs)
                activeUseDirectReading = true;
            else
                activeUseDirectReading = this.ActiveUseDirectReading;

            #region prepare for CoDeSys Wrapper the list of vars to be readed
            int jobIndex = 0;
            while (jobIndex < list.Count)
            {
                CoDeSysCommJob j = (CoDeSysCommJob)list[jobIndex];

                base.ExecuteJob(j);

                TryToResolveUndefinedVarType(ref j);

                if (j.MappingError)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = j.MappingErrorCode;
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    list.RemoveAt(jobIndex);
                    jobIndex--;
                }
                else
                {
                    //SyncRead
                    if (activeUseDirectReading)
                    {
                        if (!mapSyncReadJobs.ContainsKey(j.ShortAddress))
                            mapSyncReadJobs.Add(j.ShortAddress, new List<CoDeSysCommJob>() { j });
                        else
                            mapSyncReadJobs[j.ShortAddress].Add(j);

                        if (!varReadList.ContainsKey(j.ShortAddress))
                        {
                            VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, jobIndex, varReadList.Count);
                            if (StatisticsData != null)
                                j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);
                            // increase send bytes diagnostic var
                            varReadList.Add(j.ShortAddress, VarP);
                        }
                    }
                    else
                    {
                        //CyclRead
                        if (_MapCycReadJobs.ContainsKey(j.ShortAddress))
                        {
                            if (!mapSyncReadJobs.ContainsKey(j.ShortAddress))
                                mapSyncReadJobs.Add(j.ShortAddress, new List<CoDeSysCommJob>() { j });
                            else
                                mapSyncReadJobs[j.ShortAddress].Add(j);

                            if (!varReadList.ContainsKey(j.ShortAddress))
                            {
                                VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, j.CycVarListID, varReadList.Count);
                                // increase send bytes diagnostic var
                                if (StatisticsData != null)
                                    j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);
                                varReadList.Add(j.ShortAddress, VarP);
                            }
                        }
                        else
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorUnmappedTag;
                            eJob.Job = j;
                            OnJobExecuted(eJob);
                            list.RemoveAt(jobIndex);
                            jobIndex--;
                        }
                    }
                }
                jobIndex++;
            }

            // no data to process
            if (varReadList.Count == 0)
                return;
            #endregion 

            // Sync read
            if (activeUseDirectReading)
            {
                ret = _PLCHandler.SyncReadVarsFromPlc(varReadList.Values.ToList(), out varValueList);
            }
            else //cycling read
            {
                // EnterVarAccess and LeaveVarAccess moved inside CSWCycReadVars
                ret = _PLCHandler.CycReadVars(varReadList.Values.ToList(), out varValueList);
                switch (ret)
                {
                    case CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_UPDATE:
                        //Cyclist list was defined and the update thread is running, but the first read of the variable list has not yet been finished.Try again later to read the variables                    
                        LocalWaitOne(Properties.Settings.Default.DeviceOpenInternalWait);
                        break;

                    case CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NO_CYCLIC_LIST_DEFINED:
                        // force now creation of (new) cycling list
                        CycDefineVarList();                        
                        break;
                }
            }

            switch (ret)
            {
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:
                    LastErrorCode = (DriverErrorCodes)ret;
                    break;
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NO_CYCLIC_LIST_DEFINED:
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_UPDATE:
                    LastErrorCode = (DriverErrorCodes)CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                    break;                
                default: //generic error                                        
                    ret = (CoDeSysProtocol.PLCHandlerErrors)CoDeSysProtocol.ShiftError(ret);
                    LastErrorCode = (DriverErrorCodes)CoDeSysProtocol.ShiftError(ret);
                    break;
            }

            foreach (CoDeSysPLCHandlerWrapper.VarParam Var in varReadList.Values.ToList())
            {
                List<CoDeSysCommJob> jobsWithSameAddress = null;
                // retrieve list of jobs associated to the same var --> more job access to same CoDeSys var
                mapSyncReadJobs.TryGetValue(Var.VarName, out jobsWithSameAddress);
                
                if (jobsWithSameAddress != null)
                {
                    foreach (CoDeSysCommJob j in jobsWithSameAddress)
                    {
                        switch (ret)
                        {
                            // data not ready --> don't process
                            case CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NO_CYCLIC_LIST_DEFINED:
                            case CoDeSysProtocol.PLCHandlerErrors.RESULT_NO_UPDATE:
                                RemovePendingJob(j);
                                break;

                            case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:

                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.ErrorCode = (DriverErrorCodes)ret;

                                CoDeSysPLCHandlerWrapper.VarValue DeviceValue = varValueList[Var.ValueIndex];

                                if (StatisticsData != null)
                                    // increase receive bytes diagnostic var
                                    j.DiagnRxBytes += CoDeSysPLCHandlerWrapper.VarValue.SizeOf(DeviceValue);

                                // good data ?
                                if (DeviceValue.HasData())
                                {
                                    eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                                    if (CoDeSysProtocol.IsStringType(j.CoDeSysVarType))
                                    {
                                        eJob.Values = CoDeSysProtocol.StringConverterFromByteData(j.CoDeSysVarType, DeviceValue.Data);
                                    }
                                    else if (j.TagsList[0].TagNode.ArrayDimension != 0)
                                    {
                                        eJob.Values = DeviceValue.Data;
                                    }
                                    else
                                    { // default --> number
                                        eJob.Values = DeviceValue.Data;
                                    }
                                }
                                else
                                {
                                    eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.PLCHandlerErrors.RESULT_BUFFER_TOO_SMALL;
                                }

                                eJob.Job = j;
                                OnJobExecuted(eJob);
                                break;

                            default:  // error conditions
                                ExecutedJobArgs errJob = new ExecutedJobArgs();
                                errJob.ErrorCode = (DriverErrorCodes)ret;
                                errJob.Job = j;
                                OnJobExecuted(errJob);
                                break;
                        }
                    }
                }
            }
        }

        protected void WriteData(List<CommJob> list)
        {
            CoDeSysProtocol.PLCHandlerErrors ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;            
            List<CoDeSysPLCHandlerWrapper.VarParam> varList = new List<CoDeSysPLCHandlerWrapper.VarParam>();
            List<CoDeSysPLCHandlerWrapper.VarWrite> valueList = new List<CoDeSysPLCHandlerWrapper.VarWrite>();            

            CoDeSysStation s = list[0].Station as CoDeSysStation;
            if (s == null)
                return;

            int jobIndex = 0;
            while (jobIndex < list.Count)
            { 
                CoDeSysPLCHandlerWrapper.VarWrite ValueWrite = null;
                CoDeSysCommJob j = list[jobIndex] as CoDeSysCommJob;

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
                    list.RemoveAt(jobIndex);
                    continue;
                }

                // check if element number is correct --> this operation can be perform only online base device data type is retrive by driver on fly
                if (!CoDeSysProtocol.IsValidElementNumber(j))
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                    list.RemoveAt(jobIndex);
                    continue;
                }
                #endregion

                object objectData = null;
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                {
                    RemovePendingJob(j);
                    list.RemoveAt(jobIndex);
                    continue;
                }
                byte[] jobdata = (byte[])objectData;
                
                if (CoDeSysProtocol.IsStringType(j.CoDeSysVarType))
                {
                    byte[] StringData = CoDeSysProtocol.StringConverterToData(j.CoDeSysVarType, j.TotalJobSize, jobdata);
                    ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, StringData, jobIndex);                    
                }
                else if (j.TagsList[0].TagNode.ArrayDimension != 0)
                {
                    // simple array
                    // write an array with less elements than declared on device is possible but CoDeSys function fill missing element with random values --> not allowed --> only read
                    if (j.TagsList[0].TagNode.ArrayDimension < j.CoDeSysArrayDimension)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.ErrorCode = (DriverErrorCodes)CoDeSysProtocol.CoDeSysErrorCodes.ErrorWriteArraySizeSmaller;
                        eJob.Job = j;
                        OnJobExecuted(eJob);

                        // don't write value
                        ValueWrite = null;
                    }
                    else
                    {
                        ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, jobdata, jobIndex);
                    }
                    //}
                }
                else
                { // default --> number
                    ValueWrite = new CoDeSysPLCHandlerWrapper.VarWrite(j.ShortAddress, jobdata, jobIndex);
                }

                if (ValueWrite != null)
                {
                    CoDeSysPLCHandlerWrapper.VarParam VarP = new CoDeSysPLCHandlerWrapper.VarParam(j.ShortAddress, j.CoDeSysVarType, j.TotalJobSize, jobIndex, varList.Count);
                    varList.Add(VarP);
                    // increase send bytes diagnostic var
                    if (StatisticsData != null)
                        j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarParam.SizeOf(VarP);

                    valueList.Add(ValueWrite);
                    // increase send bytes diagnostic var
                    if (StatisticsData != null)
                        j.DiagnTxBytes += CoDeSysPLCHandlerWrapper.VarWrite.SizeOf(ValueWrite);
                }
                jobIndex++;
            }

            if (varList.Count == 0)
                return;

            //Write 
            ret = _PLCHandler.SyncWriteVarValues(valueList);          

            switch (ret) { 
                case CoDeSysProtocol.PLCHandlerErrors.RESULT_OK:
                    LastErrorCode = (DriverErrorCodes)ret;
                    break;
                default:
                    string Aux = String.Format(Properties.Resources.ErrorCoDeSysWrapperGeneral, s.Name, ret.ToString());
                    CommDriver.OnSystemEvent(ObjectIds.Server, Aux, Opc.Ua.EventSeverity.High);
                    ret = (CoDeSysProtocol.PLCHandlerErrors)CoDeSysProtocol.ShiftError(ret);
                    LastErrorCode = (DriverErrorCodes)ret;
                    break;
            }

            foreach (CoDeSysPLCHandlerWrapper.VarParam Var in varList)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = (DriverErrorCodes)ret;
                eJob.Job = list[Var.VarIndex];
                OnJobExecuted(eJob);
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                SetJobsInError(list, DriverErrorCodes.ErrorTimeOut);
                return false;
            }

            if (list[0].Type == DriverCodeBaseEx.Enumerators.LinkType.Input ||
                (list[0].Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
                list[0].GetTagListOnWritingCount() == 0))
            {
                ReadData(list);
            }
            else
            {
                WriteData(list);
            }

            return false;
        }

        /// <summary>
        /// Get total timeout used by PLCHandler for TimeOutConnection
        /// </summary>
        /// <returns></returns>
        private int GetTotalTimeOut()
        {            
            return (Timeout * _MaxRetriesBeforeError);
        }

        private int GetMaxRetriesBeforeError()
        {
            int retry = 1;
            List<Station> stations = CommDriver.GetChannelStations(this);
            if (stations != null && stations.Count > 0)
                retry = (int)CommDriver.GetChannelStations(this)[0].MaxRetriesBeforeError;
            return retry;
        }

        public string GetLogFile()
        {
            // PLCHandler logging function require a file name "well formatted" : verify that Channel name is "short" and not contain special characters
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.PLCHandlerLogPath))
                return string.Empty;
            else
                return Path.Combine(Properties.Settings.Default.PLCHandlerLogPath, string.Format("CoDeSysLog_{0}.log", this.Name));
        }

        public string GetLogFileForWrapper()
        {
            string result = GetLogFile();
            if (!string.IsNullOrEmpty(result))
                result += "\0";

            return result;
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
        #endregion     
    }
}

