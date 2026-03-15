using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using TwinCAT.Ads;
using Opc.Ua;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace TwinCAT
{
    public enum TwinCATErrorCodes : int
    {
        ErrorCodeErrorClassDevice = 0x700,
        ErrorCodeServiceNotSupported = 0x701,
        ErrorCodeInvalidIndexGroup = 0x702,
        ErrorCodeInvalidIndexOffset = 0x703,
        ErrorCodeReadWriteNotPermitted = 0x704,
        ErrorCodeParameterSizeNotCorrect = 0x705,
        ErrorCodeInvalidParameterValue = 0x706,
        ErrorCodeDeviceNotReady = 0x707,
        ErrorCodeDeviceBusy = 0x708,
        ErrorCodeInvalidContext = 0x709,
        ErrorCodeOutOfMemory = 0x70A,
        ErrorCodeInvalidParameterValue2 = 0x70B,
        ErrorCodeNotFound = 0x70C,
        ErrorCodeSintaxError = 0x70D,
        ErrorCodeObjectsNotMatch = 0x70E,
        ErrorCodeObjectsAlreadyExist = 0x70F,
        ErrorCodeSymbolNotFound = 0x710,
        ErrorCodeSymbolVersionInvalid = 0x711,
        ErrorCodeServerInvalidState = 0x712,
        ErrorCodeAdsTransModeNotSupported = 0x713,
        ErrorCodeNotificationHandleInvalid = 0x714,
        ErrorCodeNotificationClientNotRegistered = 0x715,
        ErrorCodeNoMoreNotificationsHandles = 0x716,
        ErrorCodeSizeWatchTooBig = 0x717,
        ErrorCodeDeviceNotInitialized = 0x718,
        ErrorCodeDeviceTimeout = 0x719,
        ErrorCodeQueryInterfaceFailed = 0x71A,
        ErrorCodeWrongInterfaceRequired = 0x71B,
        ErrorCodeClassIdInvalid = 0x71C,
        ErrorCodeObjectIdInvalid = 0x71D,
        ErrorCodeRequestPending = 0x71E,
        ErrorCodeRequestAborted = 0x71F,
        ErrorCodeSignalWarning = 0x720,
        ErrorCodeInvalidArrayIndex = 0x721,
        ErrorCodeSymbolNotActive = 0x722,
        ErrorCodeAccessDenied = 0x723,
        ErrorCodeErrorClassClient = 0x740,
        ErrorCodeInvalidParameterAtService = 0x741,
        ErrorCodepollingListEmpty = 0x742,
        ErrorCodeVarConnectionAlreadyInUse = 0x743,
        ErrorCodeInvokeIdInUse = 0x744,
        ErrorCodeTimeoutElapsed = 0x745,
        ErrorCodeErrorInWin32 = 0x746,
        ErrorCodeInvalidClientTimeoutValue = 0x747,
        ErrorCodeAdsPortNotOpened = 0x748,
        ErrorCodeInternalErrorInAdsSync = 0x750,
        ErrorCodeHashTableOverflow = 0x751,
        ErrorCodeKeyNotFoundInHash = 0x752,
        ErrorCodeNoMoreSymbolsInCache = 0x753,
        ErrorCodeInvalidResponseReceived = 0x754,
        ErrorCodeSyncPortLocked = 0x755
    }

    public enum TwinCATVersions : byte
    {
        Version2x,
        Version3x
    }

    public struct TcVarInfo
    {
        public int indexGroup;
        public int indexOffset;
        public TwinCATDataFormat dataFormat;
        public int length;
    }

    public class TwinCATChannel : Channel
    {
        public const uint MAX_REQUEST_NUMBER = 500;
        
        #region Constructors

        /// <summary>
        /// Initializes the EIBChannel object.
        /// </summary>
        public TwinCATChannel(CommunicationDriver commdriver, TwinCATChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _TwinCATAdsAmsNetId = settings.TwinCATAdsAmsNetId;
            _TwinCATAdsAmsPortNum = settings.TwinCATAdsAmsPortNum;
            _TwinCATCommunicationTimeout = settings.TwinCATCommunicationTimeout;
            _TwinCATMaxNumberOfAggregatedRequests = settings.TwinCATMaxNumberOfAggregatedRequests;
            if (_TwinCATMaxNumberOfAggregatedRequests == 0)
            {
                _TwinCATMaxNumberOfAggregatedRequests = 1;
            }
            _TwinCATVersion = settings.TwinCATVersion;
            TcClientIsConnected = false;
            WaitInCaseOfConnectionError = new ManualResetEvent(false);
            lastConnectionErrorCode = AdsErrorCode.NoError;
        }

        #endregion

        #region Data Members

        TcAdsClient tcAds;
        bool TcClientIsConnected;
        protected object lockStream = new object();
        TcVarInfo[] ReadWriteRequestArray = new TcVarInfo[MAX_REQUEST_NUMBER];
        List<TwinCATCommJob> nextlist = new List<TwinCATCommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();
        //UInt16 TotalRequestLength = 0;
        //UInt16 TotalWriteRequestLength = 0;
        protected ManualResetEvent WaitInCaseOfConnectionError;
        AdsErrorCode lastConnectionErrorCode;
        bool jumpsTcAdsDllMessagesNotInstalled = false;

        #endregion

        #region Abstracts Methods

        public override bool TestChannelComm()
        {
            return DeviceOpen();
        }

        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                if (TcClientIsConnected)
                {
                    TcClientIsConnected = tcAds.IsConnected;
                }
                SetStateCommandVariableBit(!TcClientIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return TcClientIsConnected;
            }
        }

        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                m_CommunicationStartTime = DateTime.MinValue;
                if (tcAds == null)
                {
                    if(!CheckDll())
                    {
                        if (!jumpsTcAdsDllMessagesNotInstalled)
                        {
                            CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorAMSTwincatRouter), EventSeverity.Max);
                            string msgError = DateTime.Now.ToString() + " - " + Properties.Resources.ErrorAMSTwincatRouter;
                            Console.WriteLine(msgError);
                            jumpsTcAdsDllMessagesNotInstalled = true;
                        }
                        WaitInCaseOfConnectionError.WaitOne(10, false);
                        return (false);
                    }
                  
                    tcAds = new TcAdsClient();
                }

                if ((tcAds != null) && !TcClientIsConnected)
                {
                    // Connect to remote server
                    if (!String.IsNullOrWhiteSpace(_TwinCATAdsAmsNetId))
                    {
                        try
                        {
                            tcAds.Connect(_TwinCATAdsAmsNetId, (int)_TwinCATAdsAmsPortNum);                           
                        }
                        catch (Exception ex)
                        {
                            if (lastConnectionErrorCode == AdsErrorCode.NoError)
                                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ExceptionConnectionADS, ex.Message), EventSeverity.Min);
                            lastConnectionErrorCode = AdsErrorCode.HostUnreachable;
                            DeviceClose();
                            return false;
                        }
                    }

                    // Empty AmsNetId --> Connect to local server
                    else
                    {
                        try
                        {
                            tcAds.Connect((int)_TwinCATAdsAmsPortNum);
                        }
                        catch(Exception ex)
                        {
                            if (lastConnectionErrorCode == AdsErrorCode.NoError)
                                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ExceptionConnectionADS, ex.Message), EventSeverity.Min);
                            lastConnectionErrorCode = AdsErrorCode.HostUnreachable;
                            DeviceClose();
                            return false;
                        }
                    }


                    // Check connection
                    if (!tcAds.IsConnected)
                    {
                        DeviceClose();
                        return false;
                    }

                    TcClientIsConnected = true;

                    // Check Ads status
                    if (!ReadAdsStatus())
                    {
                        DeviceClose();
                        if (lastConnectionErrorCode == AdsErrorCode.NoError)
                            lastConnectionErrorCode = AdsErrorCode.PortConnected;
                        return false;
                    }

                    lastConnectionErrorCode = AdsErrorCode.NoError;

                    // Set the timeout for ADS communication
                    if (TwinCATCommunicationTimeout > 0)
                    {
                        tcAds.Timeout = (int)TwinCATCommunicationTimeout;
                    }
                }

                return (TcClientIsConnected && (tcAds != null));
            }
        }

        public override bool DeviceClose()
        {
            lock (lockStream)
            {
                TcClientIsConnected = false;
                try
                {
                    tcAds.Dispose();
                }
                catch
                {

                }
                return true;
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

            m_CommunicationTimeHasBeRecovered = Timeout * 10;
            if(m_CommunicationTimeHasBeRecovered < Properties.Settings.Default.ConstThePLCIsRunning)
            {
                m_CommunicationTimeHasBeRecovered = Properties.Settings.Default.ConstThePLCIsRunning;
            }
            
            bool bForceProcessListJobs = false;
            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            //int dbgCount = 0;
            while (true)
            {
                //dbgCount++;

                CommJob nextjob = null;
                nextlist.Clear();
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    lock (lockThreadObject)//201011
                    {
                        if (SynchroJob != null)
                        {
                            nextlist.Add(SynchroJob as TwinCATCommJob);
                            //TotalRequestLength = 1;
                        }
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }

                if (nextlist.Count > 0)
                {
                    if (!IsDeviceOpen())
                    {
                       bool bConnection = DeviceOpen();
                        if(!bConnection)
                        {
                            SetConnectionError(nextlist);
                        }
                    }
                    if (IsDeviceOpen())
                    {
                        lock (lockThreadObject)//201011
                        {
                            ExecuteJobList(nextlist);
                        }
                    }
                    //System.Diagnostics.Trace.TraceInformation("ExecuteJob {0} :{1} cnt={2}", nextjob.GroupString, DateTime.UtcNow.ToString("hh:mm:ss:ffff"), dbgCount);
                    //dbgCount = 0;
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
                {
                    DeviceClose();
                }

                if (nextjob != null && StopWorkerThread.WaitOne(WaitTime, false))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                }
                StopWorkerThread.WaitOne(0, false);
            }
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
             TwinCATCommJob JobTwincat = (TwinCATCommJob)e.Job;
            if (JobTwincat.AddressObj.IsValid == false)
            {
                UnsubscribeJob(e.Job);
                System.Diagnostics.Debug.WriteLine(String.Format("TCDBG - Unsubscribed {0}", JobTwincat.AddressObj.Address));
            }
            base.OnJobExecuted(e);           
        }

        #endregion

        #region Specific Methods

        bool ReadAdsStatus()
        {
            if (!TcClientIsConnected)
            {
                return false;
            }

            AdsErrorCode connectionErrorCode = AdsErrorCode.NoError;
            bool ReturnValue = false;
            try
            {
                StateInfo si = tcAds.ReadState();
                switch (si.AdsState)
                {
                    case AdsState.Run:
                        ReturnValue = true;
                        break;
                }
            }
            catch (AdsErrorException ex)
            {
                if (lastConnectionErrorCode == AdsErrorCode.NoError)
                    CommDriver.OnSystemEvent(
                       null,
                       String.Format(Properties.Resources.ExceptionReadState,
                                     ex.ErrorCode, ex.Message),
                       Opc.Ua.EventSeverity.Max);
                connectionErrorCode = ex.ErrorCode;
                WaitInCaseOfConnectionError.WaitOne(10, false);
            }

            lastConnectionErrorCode = connectionErrorCode;
            return ReturnValue;
        }

        protected void GetNextPendingList(ref List<TwinCATCommJob> list)
        {
            bool write = false;
            string station = string.Empty;
            bool first = true;
            uint ReqDim = 0;

            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return;

                while (!queue.IsEmpty)
                {
                    TwinCATCommJob j = queue.ElementAt(0) as TwinCATCommJob;

                    if ((j != null) && (j.AddressObj.IsValid))
                    {
                        if (first)
                        {
                            write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                        }
                        bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                if ((ReqDim >= _TwinCATMaxNumberOfAggregatedRequests) || (ReqDim >= MAX_REQUEST_NUMBER))
                                {
                                    break;
                                }
                                ReqDim++;
                                list.Add(j);
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {
                                if ((ReqDim >= _TwinCATMaxNumberOfAggregatedRequests) || (ReqDim >= MAX_REQUEST_NUMBER))
                                {
                                    break;
                                }
                                ReqDim++;
                                list.Add(j);
                            }
                        }
                    }
                    else if (j != null)
                    {
                        j.LastExecutionTime = DateTime.UtcNow;
                    }
                    CommJob dequeuedJob = null;
                    queue.TryDequeue(out dequeuedJob);
                }

            }
            //TotalRequestLength = (UInt16)list.Count;
        }

        protected bool InitGroupAndOffset(ref TwinCATAddress AddressObj)
        {
            if((AddressObj.IsNumeric == true) || (AddressObj.IsValid == false))
            {
                return true;
            }

            AdsErrorCode connectionErrorCode = AdsErrorCode.NoError;
            bool rt = true;
            try
            {
                //tcAds.CreateVariableHandle(AddressObj.Address); // for generate exception
                // Get symbol information and set the data format
                ITcAdsSymbol adsSymbolInfo = tcAds.ReadSymbolInfo(AddressObj.Address);

                //System.Diagnostics.Debug.WriteLine(String.Format("TCDBG - 00 InitGroupAndOffset {0}", AddressObj.Address));
                if (adsSymbolInfo == null)
                {
                    double result = (DateTime.UtcNow - m_CommunicationStartTime).TotalMilliseconds;
                    if ((m_CommunicationStartTime == DateTime.MinValue)|| 
                        (result <= m_CommunicationTimeHasBeRecovered))
                    {
                        //System.Diagnostics.Debug.WriteLine(String.Format("TCDBG - 01 InitGroupAndOffset {0}", AddressObj.Address));
                        return false;
                    }

                    AddressObj.IndexGroup = 0;
                    AddressObj.IndexOffset = 0;
                    AddressObj.IsValid = false;
                    AddressObj.InvalidErrorCode = AdsErrorCode.DeviceSymbolNotFound;
                    //System.Diagnostics.Debug.WriteLine(String.Format("TCDBG - 1 InitGroupAndOffset {0}", AddressObj.Address));
                    CommDriver.OnSystemEvent(
                        null,
                        String.Format(Properties.Resources.ErrorSymbolNotFound,
                                      AddressObj.Address), Opc.Ua.EventSeverity.Min);
                    return false;
                }

                if (m_CommunicationStartTime == DateTime.MinValue)
                {
                    m_CommunicationStartTime = DateTime.UtcNow;
                }
                AddressObj.IndexGroup = (int)adsSymbolInfo.IndexGroup;
                AddressObj.IndexOffset = (int)adsSymbolInfo.IndexOffset; 
                
                switch (adsSymbolInfo.Datatype)
                {
                    case AdsDatatypeId.ADST_INT16:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_INT;
                        break;

                    case AdsDatatypeId.ADST_INT32:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_DINT;
                        break;

                    case AdsDatatypeId.ADST_REAL32:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_REAL;
                        break;

                    case AdsDatatypeId.ADST_REAL64:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_LREAL;
                        break;

                    case AdsDatatypeId.ADST_INT8:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_SINT;
                        break;

                    case AdsDatatypeId.ADST_UINT8:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_Byte;
                        break;

                    case AdsDatatypeId.ADST_UINT16:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_Word;
                        break;

                    case AdsDatatypeId.ADST_UINT32:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_DWord;
                        break;

                    case AdsDatatypeId.ADST_STRING:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_STRING;
                        break;

                    case AdsDatatypeId.ADST_BIT:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_Bit;
                        break;

                    case AdsDatatypeId.ADST_INT64:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_LINT;
                        break;

                    case AdsDatatypeId.ADST_UINT64:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_ULINT;
                        break;

                    default:
                        AddressObj.DataFormat = TwinCATDataFormat.DataFormat_Invalid;

                        AddressObj.IndexGroup = 0;
                        AddressObj.IndexOffset = 0;
                        AddressObj.IsValid = false;
                        AddressObj.InvalidErrorCode = AdsErrorCode.DeviceInvalidData;
                        CommDriver.OnSystemEvent(
                            null,
                            String.Format(Properties.Resources.ErrorInvalidTagType,
                                          AddressObj.Address, adsSymbolInfo.Datatype), Opc.Ua.EventSeverity.Min);
                        rt = false;
                        break;
                }
            }
            catch (AdsErrorException ex)
            {
                AddressObj.IndexGroup = 0;
                AddressObj.IndexOffset = 0;
                if (ex.ErrorCode == AdsErrorCode.DeviceSymbolNotFound)
                {
                    //System.Diagnostics.Debug.WriteLine(String.Format("TCDBG - 2 InitGroupAndOffset {0}", AddressObj.Address));
                    AddressObj.IsValid = false;
                    AddressObj.InvalidErrorCode = AdsErrorCode.DeviceSymbolNotFound;
                    CommDriver.OnSystemEvent(
                        null,
                        String.Format(Properties.Resources.ExceptionInitGroupOffset,
                                      AddressObj.Address, ex.ErrorCode, ex.Message),
                        Opc.Ua.EventSeverity.Min);
                }
                else
                {
                    if (lastConnectionErrorCode == AdsErrorCode.NoError)
                        CommDriver.OnSystemEvent(
                        null,
                        String.Format(Properties.Resources.ExceptionReadState,
                                      ex.ErrorCode, ex.Message),
                        Opc.Ua.EventSeverity.Max);
                    WaitInCaseOfConnectionError.WaitOne(10, false);
                    connectionErrorCode = ex.ErrorCode;
                }
                m_CommunicationStartTime = DateTime.MinValue;
                rt = false;
            }
            catch (Exception ex)
            {
                AddressObj.IndexGroup = 0;
                AddressObj.IndexOffset = 0;
                AddressObj.IsValid = false;
                AddressObj.InvalidErrorCode = AdsErrorCode.UnknownCommandID;
                CommDriver.OnSystemEvent(
                    null,
                    String.Format(Properties.Resources.ExceptionReadSymbolInfo, AddressObj.Address, ex.HResult, ex.Message),
                    Opc.Ua.EventSeverity.Max);
                connectionErrorCode = AdsErrorCode.InternalError;
                WaitInCaseOfConnectionError.WaitOne(10, false);
                m_CommunicationStartTime = DateTime.MinValue;
                rt = false;
            }
            lastConnectionErrorCode = connectionErrorCode;
            return rt;
        }

        protected List<TwinCATCommJob> PrepareReadRequestParameters(List<TwinCATCommJob> list)
        {
            List<TwinCATCommJob> listToRead = new List<TwinCATCommJob>();
            int requestIndex = 0;
            int i = 0;

            while (i < list.Count)
            {
                // if not yet done, get info about tag
                if (list[i].AddressObj.IndexGroup == 0)
                    InitGroupAndOffset(ref list[i].AddressObj);

                // if tag if valid (exist on PLC)
                if (list[i].AddressObj.IndexGroup == 0)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)list[i].AddressObj.InvalidErrorCode;
                    eJob.Job = list[i];
                    OnJobExecuted(eJob);
                }
                else
                {
                    listToRead.Add(list[i]);

                    if (list[i].AddressObj.IsNumeric && list[i].ProtocolDataSizeSmall())
                        list[i].ElementNumber = 1;
                    uint TotalJobSize = list[i].TotalJobSize;
                    if (list[i].ElementNumber > 0 || list[i].ProtocolDataSizeBig())
                    {
                        if (list[i].TagsList[0].TagNode.ArrayDimension == 0)
                            TotalJobSize = list[i].GetProtocolDataByteSize();
                        else
                            TotalJobSize = list[i].GetProtocolDataByteSize() * list[i].TagsList[0].TagNode.ArrayDimension;
                    }

                    ReadWriteRequestArray[requestIndex].indexGroup = list[i].AddressObj.IndexGroup;
                    ReadWriteRequestArray[requestIndex].indexOffset = list[i].AddressObj.IndexOffset;
                    ReadWriteRequestArray[requestIndex].length = (int)TotalJobSize;
                    ReadWriteRequestArray[requestIndex].dataFormat = list[i].AddressObj.DataFormat;
                    requestIndex++;
                }
                i++;
            }

            return listToRead;
        }

        protected List<TwinCATCommJob> PrepareWriteRequestParameters(List<TwinCATCommJob> list)
        {
            Tag CurrentTag = null;
            int requestIndex = 0;
            List<TwinCATCommJob> listToWrite = new List<TwinCATCommJob>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].AddressObj.IndexGroup == 0)
                    InitGroupAndOffset(ref list[i].AddressObj);

                // if tag if valid (exist on PLC)
                if (list[i].AddressObj.IndexGroup == 0)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)list[i].AddressObj.InvalidErrorCode;
                    eJob.Job = list[i];
                    OnJobExecuted(eJob);
                }
                else
                {
                    lock (list[i].retLockList())
                    {
                        if (list[i].TagsListToWrite.Count == 0)
                        {
                            if (list[i].Type != DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                            {
                                continue;
                            }
                            else
                            {
                                list[i].TagsListToWrite.AddRange(list[i].TagsList);
                                if (list[i].TagsListToWrite.Count == 0)
                                {
                                    continue;
                                }
                            }
                        }

                        if ((list[i].Station.RewritingOfTheSameValue == false) &&
                            ((list[i].Type == LinkType.ExceptionOutput || list[i].Type == LinkType.InputOutput)))
                        {
                            var tempTagsListToWrite = (from tag in list[i].TagsListToWrite
                                                       where (!StatusCode.IsGood(tag.Value.StatusCode) ||
                                                              (tag.LastValue == null) ||
                                                              (!tag.LastValue.Equals(tag.Value.Value)))
                                                       select tag).ToList();

                            list[i].TagsListToWrite.Clear();
                            if (tempTagsListToWrite.Count == 0)
                            {
                                list[i].IsPending = false;
                                continue;
                            }
                            else
                            {
                                list[i].TagsListToWrite.AddRange(tempTagsListToWrite);
                            }
                        }

                        listToWrite.Add(list[i]);

                        if (list[i].AddressObj.IsNumeric && list[i].ProtocolDataSizeSmall())
                            list[i].ElementNumber = 1;

                        for (int j = 0; j < list[i].TagsListToWrite.Count; j++)
                        {
                            CurrentTag = list[i].TagsListToWrite[j];
                            ReadWriteRequestArray[requestIndex].indexGroup = list[i].AddressObj.IndexGroup;
                            ReadWriteRequestArray[requestIndex].indexOffset = list[i].AddressObj.IndexOffset + (int)CurrentTag.ByteOffset;
                            int CurrentTagSize = (int)CurrentTag.Size;
                            if (list[i].ElementNumber > 0 || list[i].ProtocolDataSizeBig())
                            {
                                if (list[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    CurrentTagSize = (int)list[i].GetProtocolDataByteSize();
                                else
                                    CurrentTagSize = (int)(list[i].GetProtocolDataByteSize() * list[i].TagsList[0].TagNode.ArrayDimension);
                            }
                            ReadWriteRequestArray[requestIndex].length = CurrentTagSize;
                            ReadWriteRequestArray[requestIndex].dataFormat = list[i].AddressObj.DataFormat;
                            requestIndex++;
                        }
                    }
                }
            }

            //TotalWriteRequestLength = (UInt16)requestIndex;

            return listToWrite;
        }

        protected void WriteData(List<TwinCATCommJob> list)
        {
            AdsErrorCode connectionErrorCode;
            List<TwinCATCommJob> listToWrite = PrepareWriteRequestParameters(list);
            if (listToWrite.Count == 0)
            {
                return;
            }

            //BinaryReader reader =
            //   new BinaryReader(WriteMultipleVariables(ReadWriteRequestArray, list, out connectionErrorCode));
            BinaryReader reader = new BinaryReader(WriteMultipleVariables(ReadWriteRequestArray, listToWrite, out connectionErrorCode));
            for (int i = 0; i < listToWrite.Count; i++)
            {
                int error = reader.ReadInt32();
                ExecutedJobArgs eJob = new ExecutedJobArgs();

                if (connectionErrorCode != AdsErrorCode.NoError)
                {
                    eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                }
                else if (error != (int)AdsErrorCode.NoError)
                {
                    eJob.ErrorCode = (DriverErrorCodes)error;
                    System.Diagnostics.Debug.WriteLine(
                    String.Format("Unable to write variable {0} (Error = {1})",
                                  i, error));
                }
                else
                {
                    eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                }

                if (!list[i].AddressObj.IsNumeric && eJob.ErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    list[i].AddressObj.IndexOffset = 0;
                    list[i].AddressObj.IndexGroup = 0;
                }

                eJob.Job = list[i];
                OnJobExecuted(eJob);
            }

            listToWrite.Clear();
        }

        private AdsStream WriteMultipleVariables(TcVarInfo[] VarInfo, List<TwinCATCommJob> list, out AdsErrorCode connectionErrorCode)
        {
            int totalWriteRequestLength = list.Count();
            connectionErrorCode = AdsErrorCode.NoError;

            // Allocate memory
            int rdLength = totalWriteRequestLength * 4;
            int wrLength = totalWriteRequestLength * 12;
            for (int i = 0; i < totalWriteRequestLength; i++)
            {
                wrLength += VarInfo[i].length;
            }

            // Write data for handles into the ADS Stream
            BinaryWriter writer = new BinaryWriter(new AdsStream(wrLength));
            for (int i = 0; i < totalWriteRequestLength; i++)
            {
                writer.Write(VarInfo[i].indexGroup);
                writer.Write(VarInfo[i].indexOffset);
                writer.Write(VarInfo[i].length);
            }

            // Write data to send to the PLC into the ADS Stream
            // TotalRequestLength is the number of jobs included in "list"
            Tag CurrentTag = null;
            //for (int i = 0; i < TotalRequestLength; i++)
            for (int i = 0; i < list.Count; i++)
            {
                CommJob job = list[i];
                var listToWrite = new List<Tag>();
                var listOnWriting = new List<Tag>();

                lock (job.retLockList())
                {
                    listToWrite.AddRange(job.TagsListToWrite);
                    job.TagsListToWrite.Clear();
                }

                while (listToWrite.Count > 0)
                {

                    CurrentTag = listToWrite[0];
                    listToWrite.Remove(CurrentTag);
                    if (!listOnWriting.Contains(CurrentTag))
                        listOnWriting.Add(CurrentTag);

                    UInt16 nData = 0;
                    uint ArraySize = CurrentTag.TagNode.ArrayDimension;
                    if (ArraySize == 0)
                        ArraySize = 1;

                    if ((uint)CurrentTag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        //The arrays of Boolean are not possible on Bit-Adress %MX8.0
                        if (CurrentTag.TagNode.ArrayDimension > 0)
                        {
                            nData = (UInt16)ArraySize;
                        }
                        else
                        {
                            nData = (UInt16)((ArraySize + 7) / 8);
                        }

                    }
                    else if (list[i].ElementNumber > 0 && !list[i].ProtocolDataSizeBig())
                    {
                        if (CurrentTag.TagNode.ArrayDimension == 0)
                            nData = (ushort)(list[i].GetProtocolDataByteSize());
                        else
                            nData = (ushort)(list[i].GetProtocolDataByteSize() * CurrentTag.TagNode.ArrayDimension);
                    }
                    else
                        nData = (UInt16)CurrentTag.Size;

                    byte[] jobdata;
                    lock (job.retLockList())
                    {
                        CurrentTag.LastValue = CurrentTag.Value.Value;
                        jobdata = new byte[nData];
                        ((TwinCATTag)CurrentTag).GetTagBuffer(ref jobdata, false, 0, (list[i].ElementNumber > 0 && !list[i].ProtocolDataSizeBig() ? list[i].GetProtocolDataByteSize() : 0));
                    }

                    bool bIsArrayOfBoolean = (CurrentTag.TagNode.ArrayDimension > 0) && ((uint)CurrentTag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean);

                    if (list[i].ProtocolDataSizeBig() && !(bIsArrayOfBoolean))
                    {
                        List<byte> correctData = new List<byte>();
                        UInt16 sizeDataType = (UInt16)list[i].GetDataTypeByteSize((uint)CurrentTag.TagNode.DataType.Identifier);
                        UInt16 sizeProtocolData = (UInt16)list[i].GetProtocolDataByteSize();
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            byte[] tmpdata = new byte[sizeDataType];
                            if ((uint)CurrentTag.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                                Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                            else
                            {
                                if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                                    tmpdata[0] = 0;
                                else
                                    tmpdata[0] = 1;
                            }
                            CurrentTag.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, list[i].ElementNumber, ArrayIndex);
                            correctData.AddRange(tmpdata);
                        }
                        jobdata = correctData.ToArray();
                    }
                    else if (list[i].isProtocolBool())
                    {
                        if (list[i].ElementNumber > 0)
                        {
                            byte[] tmpData = new byte[(ArraySize + 7) / 8];
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                if (jobdata[ArrayIndex] != 0)
                                    tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                            }
                            jobdata = tmpData;
                        }
                    }

                    if (!job.isProtocolBool())
                    {
                        if (job.SwapBytes)
                        {
                            CommJob.SwapByteBuffer(ref jobdata);
                        }

                        if (job.SwapWords)
                        {
                            CommJob.SwapWordBuffer(ref jobdata);
                        }
                    }

                    writer.Write(jobdata, 0, jobdata.Length);
                }

                lock (job.retLockList())
                {
                    listOnWriting.ForEach((tag) =>
                    {
                        if (!job.TagsListOnWriting.Contains(tag))
                            job.TagsListOnWriting.Add(tag);
                    });
                    listOnWriting.Clear();
                }
            }

            // Sum command to write variables to the PLC
            AdsStream rdStream = new AdsStream(rdLength);
            try
            {
                //tcAds.ReadWrite(0xF081, TotalRequestLength, rdStream,
                //                (AdsStream)writer.BaseStream);
                // this configuration (== 1) is for devices that not support multiple read/write request
                if (this.TwinCATMaxNumberOfAggregatedRequests == 1)
                    tcAds.Write(VarInfo[0].indexGroup, VarInfo[0].indexOffset, (AdsStream)writer.BaseStream, 12, VarInfo[0].length);
                else
                    tcAds.ReadWrite(0xF081, totalWriteRequestLength, rdStream, (AdsStream)writer.BaseStream);
            }
            catch (AdsErrorException ex)
            {
                connectionErrorCode = ex.ErrorCode;
                if (lastConnectionErrorCode == AdsErrorCode.NoError)
                    CommDriver.OnSystemEvent(
                    null,
                    String.Format(Properties.Resources.ExceptionReadState,
                                  ex.ErrorCode, ex.Message),
                    Opc.Ua.EventSeverity.Max);
                WaitInCaseOfConnectionError.WaitOne(10, false);
            }
            lastConnectionErrorCode = connectionErrorCode;

            // Return the ADS error codes
            return rdStream;
        }

        private AdsStream ReadMultipleVariables(TcVarInfo[] VarInfo, List<TwinCATCommJob> list, out AdsErrorCode connectionErrorCode)
        {
            int totalRequestLength = list.Count();
            connectionErrorCode = AdsErrorCode.NoError;

            // Allocate memory
            int rdLength = totalRequestLength * 4;
            int wrLength = totalRequestLength * 12;

            // Write data for handles into the ADS Stream
            BinaryWriter writer = new BinaryWriter(new AdsStream(wrLength));
            for (int i = 0; i < totalRequestLength; i++)
            {
                writer.Write(VarInfo[i].indexGroup);
                writer.Write(VarInfo[i].indexOffset);
                writer.Write(VarInfo[i].length);
                rdLength += VarInfo[i].length;
            }

            // Sum command to read variables from the PLC
            AdsStream rdStream = new AdsStream(rdLength);
            try
            {
                // this configuration (== 1) is for devices that not support multiple read/write request
                if (this.TwinCATMaxNumberOfAggregatedRequests == 1)
                    tcAds.Read(VarInfo[0].indexGroup, VarInfo[0].indexOffset, rdStream, 4, VarInfo[0].length);
                else
                    tcAds.ReadWrite(0xF080, totalRequestLength, rdStream, (AdsStream)writer.BaseStream);
            }
            catch (AdsErrorException ex)
            {
                connectionErrorCode = ex.ErrorCode;
                if (lastConnectionErrorCode == AdsErrorCode.NoError)
                    CommDriver.OnSystemEvent(
                    null,
                    String.Format(Properties.Resources.ExceptionReadState,
                                  ex.ErrorCode, ex.Message),
                    Opc.Ua.EventSeverity.Max);
                WaitInCaseOfConnectionError.WaitOne(10, false);
            }
            lastConnectionErrorCode = connectionErrorCode;

            // Return the ADS error codes
            return rdStream;
        }

        protected void ReadData(List<TwinCATCommJob> list)
        {
            AdsErrorCode connectionErrorCode;

            List<TwinCATCommJob> listToRead = PrepareReadRequestParameters(list);
            if (listToRead.Count == 0)
            {
                return;
            }

            BinaryReader reader = new BinaryReader(ReadMultipleVariables(ReadWriteRequestArray, listToRead, out connectionErrorCode));
            // Check error codes
            int[] ErrorCodes = new int[listToRead.Count];
            for (int i = 0; i < listToRead.Count; i++)
            {
                if (connectionErrorCode != AdsErrorCode.NoError)
                {
                    ErrorCodes[i] = (int)DriverErrorCodes.ErrorTimeOut;
                }
                else
                {
                    ErrorCodes[i] = reader.ReadInt32();
                    if (ErrorCodes[i] != (int)AdsErrorCode.NoError)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            String.Format("Unable to read variable {0} (Error = {1})", i, ErrorCodes[i]));
                    }
                    else
                        ErrorCodes[i] = (int)DriverErrorCodes.ErrorNoError;

                }
                if (!listToRead[i].AddressObj.IsNumeric && ErrorCodes[i] != (int)DriverErrorCodes.ErrorNoError)
                {
                    listToRead[i].AddressObj.IndexOffset = 0;
                    listToRead[i].AddressObj.IndexGroup = 0;
                }
            }

            // Read the data from the ADS stream
            for (int i = 0; i < listToRead.Count; i++)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                uint TotalJobSize = listToRead[i].TotalJobSize;
                if (listToRead[i].ElementNumber > 0 || listToRead[i].ProtocolDataSizeBig())
                {
                    if (listToRead[i].TagsList[0].TagNode.ArrayDimension == 0)
                        TotalJobSize = listToRead[i].GetProtocolDataByteSize();
                    else
                        TotalJobSize = listToRead[i].GetProtocolDataByteSize() * listToRead[i].TagsList[0].TagNode.ArrayDimension;
                }
                byte[] Answer;
                Answer = new byte[TotalJobSize];
                Answer = reader.ReadBytes((int)TotalJobSize);
                if (ErrorCodes[i] == (int)AdsErrorCode.NoError)
                {
                    eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                    if (ReadWriteRequestArray[i].dataFormat != TwinCATDataFormat.DataFormat_STRING)
                    {
                        eJob.Values = Answer;
                    }
                    else
                    {
                        uint numChar = 0;
                        while ((numChar < Answer.Length) && (Answer[numChar] != 0))
                        {
                            numChar++;
                        }
                        byte[] stringAnswer = new byte[numChar];
                        Array.Copy(Answer, 0, stringAnswer, 0, numChar);
                        // convert data from ASCII 8 bit to UTF8 string (Movicon format)                         
                        eJob.Values = Encoding.UTF8.GetBytes(Encoding.Default.GetString(stringAnswer));
                    }
                }
                else
                {
                    eJob.ErrorCode = (DriverErrorCodes)ErrorCodes[i];
                }
                eJob.Job = listToRead[i];
                OnJobExecuted(eJob);
            }
        }

        protected void ExchangeData(List<TwinCATCommJob> list)
        {
            if (list[0].Type == DriverCodeBase.Enumerators.LinkType.Input ||
               (list[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
               list[0].TagsListToWrite.Count == 0))
            {
                ReadData(list);
            }
            else
            {
                WriteData(list);
            }
        }

        protected void ExecuteJobList(List<TwinCATCommJob> list)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ExecuteJobList begin", DateTime.Now.ToLongTimeString()));
            if (!TcClientIsConnected)
            {
                //Error connection
                if (lastConnectionErrorCode == AdsErrorCode.NoError)
                {
                    CommDriver.OnSystemEvent(ObjectIds.Server, TwinCAT.Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High);
                    lastConnectionErrorCode = AdsErrorCode.PortConnected;
                }
                WaitInCaseOfConnectionError.WaitOne(10, false);
                return;
            }

            if (lastConnectionErrorCode != AdsErrorCode.NoError)
                if(!ReadAdsStatus())
                {
                    if (lastConnectionErrorCode == AdsErrorCode.NoError)
                    {
                        lastConnectionErrorCode = AdsErrorCode.PortConnected;
                        DeviceClose();
                        return;
                    }
                }

            ExchangeData(list);

            foreach (TwinCATCommJob j in list)
            {
                j.LastExecutionTime = DateTime.UtcNow;
            }
            list[0].StartExecutionTime = DateTime.UtcNow;

        }

        protected void SetConnectionError(List<TwinCATCommJob> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                eJob.GeneralError = true;
                eJob.Job = list[i];
                OnJobExecuted(eJob);
            }
            WaitInCaseOfConnectionError.WaitOne(10, false);
            return;
        }

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string strLibraryName);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern Int32 FreeLibrary(IntPtr hModule);

      
        protected bool CheckDll()
        {
            IntPtr hModuleDLL = LoadLibrary("TcAdsDll"); 

            if (hModuleDLL != IntPtr.Zero)
            {
                FreeLibrary(hModuleDLL);
                return (true);
            }
            return (false);
        }

        public bool AdsSyncReadReq(ref List<TcAdsSymbolInfo> ListTcAdsSymbo)
        {
            if (ListTcAdsSymbo == null)
            {
                return (false);
            }
            if (ListTcAdsSymbo.Count() > 0)
            {
                ListTcAdsSymbo.Clear();
            }            

            if (tcAds == null)
            {
                return(false);
            }

            TcAdsSymbolInfoLoader pp = tcAds.CreateSymbolInfoLoader();
            TcAdsSymbolInfoCollection ListaVar = pp.GetSymbols(false);
            System.Collections.IEnumerator myEnumerator = ListaVar.GetEnumerator();
            while ((myEnumerator.MoveNext()) && (myEnumerator.Current != null))
            {
                ListTcAdsSymbo.Add((TcAdsSymbolInfo)myEnumerator.Current);
            }

            return (true);
        }

        #endregion

        #region Properties

        private string _TwinCATAdsAmsNetId;
        public string TwinCATAdsAmsNetId
        {
            get
            {
                return _TwinCATAdsAmsNetId;
            }
            set
            {
                TwinCATAdsAmsNetId = value;
            }
        }

        private uint _TwinCATAdsAmsPortNum;
        public uint TwinCATAdsAmsPortNum
        {
            get
            {
                return _TwinCATAdsAmsPortNum;
            }
            set
            {
                TwinCATAdsAmsPortNum = value;
            }
        }

        private uint _TwinCATCommunicationTimeout;
        public uint TwinCATCommunicationTimeout
        {
            get
            {
                return _TwinCATCommunicationTimeout;
            }
            set
            {
                TwinCATCommunicationTimeout = value;
            }
        }

        private uint _TwinCATMaxNumberOfAggregatedRequests;
        public uint TwinCATMaxNumberOfAggregatedRequests
        {
            get
            {
                return _TwinCATMaxNumberOfAggregatedRequests;
            }
            set
            {
                TwinCATMaxNumberOfAggregatedRequests = value;
            }
        }

        private byte _TwinCATVersion;
        public byte TwinCATVersion
        {
            get { return _TwinCATVersion; }
            set
            {
                _TwinCATVersion = value;
            }
        }

        /*When the PLC restores communication, you may find that the "tcAds.ReadSymbolInfo (AddressObj.Address)" 
         * method requires a variable within the PLC, but returns NULL because the PLC has not finished initializing. 
         * The problem was that the first variables that were exchanged after an exception were the possibility 
         * that they would be discarded forever and never exchanged because they were considered invalid. 
         * Then, an exception memory flag was created for the "tcAds.ReadSymbolInfo (AddressObj.Address)" method, 
         * the exception memory flag is reset when the "tcAds.ReadSymbolInfo (AddressObj.Address)" method succeeds in swapping a variable.*/

        private int m_CommunicationTimeHasBeRecovered = Properties.Settings.Default.ConstThePLCIsRunning;
        private DateTime m_CommunicationStartTime = DateTime.MinValue;
        

        #endregion

    }
}

