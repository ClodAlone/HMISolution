using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using PhoenixContact.PlciDotNet;
using PhoenixContact.PlciDotNet.Exception;

namespace PhoenixContactPLCI
{
    public class PhoenixContactPLCIChannel : Channel, IDisposable
    { 
        #region Constructors

        /// <summary>
        /// Initializes the EtherNetIPChannel object.
        /// </summary>
        public PhoenixContactPLCIChannel(CommunicationDriver commdriver, PhoenixContactPLCIChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _ServerAddress = settings.ServerAddress;
            _ServerPort = settings.ServerPort;
            _SubscribeVariables = settings.SubscribeVariables;

            _LastDeviceCompletedRequest = DateTime.MinValue;
        }
        #endregion

        #region Members
        private Plci _Plci = null;
        private IDeviceAttributeService _PlciDeviceAttributeService;
        private IDataAccessService _PlciDataAccessService;
        private Dictionary<int, Subscription> _PlciSubscription = new Dictionary<int, Subscription>();
        private PhoenixContactPLCIProtocol.ConnectionState _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Unknown;
        private PhoenixContactPLCIProtocol.ConnectionState _PreviousConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Unknown;
        private DateTime _LastDeviceCompletedRequest;
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

        List<PhoenixContactPLCICommJob> nextlist = new List<PhoenixContactPLCICommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();
        protected override void WorkingThread(object data)
        {            
            if (CommDriver.GetChannelStations(this).Count == 0)
                return;

            DeviceOpen();

            // init internal status (force 1st time/cycle 'mode')
            ManageDeviceConnection(true);

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
                    lock (lockThreadObject)
                    {
                        DriverErrorCodes ret = ExecuteJobList(nextlist);
                        // if an error occour try to reconnect
                        if (ret != DriverErrorCodes.ErrorNoError)
                            ManageDeviceConnection();
                    }
                }
                else
                {
                    // check device connection state (Connected and Running ?) only when no jobs was executed synce xxx time
                    if (IsTimeToCheckConnectionState())
                        ManageDeviceConnection();
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
        }

        public override bool TestChannelComm()
        {
            bool retValue = DeviceOpen();

            // launch DeviceClose to destroy PLCI objectes
            DeviceClose();
            
            return retValue;
        }

        /// <summary>
        /// Ubsubscribe jobs to Phoenix PLCI library
        /// </summary>
        private void UnSubscribeAllJobs()
        {

            if (_PlciSubscription.Count > 0)
            {

#if DEBUG
                System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} UnSubscribeAllJobs", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")));
#endif
                if (_PlciDataAccessService != null)
                {
                    try
                    {
                        foreach (var sub in _PlciSubscription.Values)
                            _PlciDataAccessService.DestroySubscription(sub);
                    }
                    catch (Exception ex) { }
                }

                PhoenixContactPLCIStation s = (PhoenixContactPLCIStation)CommDriver.GetChannelStations(this)[0];
                List<PhoenixContactPLCICommJob> jobsList = (s.GetListWholeJobCopy().Cast<PhoenixContactPLCICommJob>().ToList());
                Parallel.ForEach(jobsList, j =>
                {
                    j.ResetSubscription();
                });
                _PlciSubscription.Clear();
            }
        }


        private void CreateSubscription(IList<PhoenixContactPLCICommJob> subscriptionJobsList)
        {
            IList<string> addressList = subscriptionJobsList.Select(x => x.Address).ToList();

            try
            {
                if (_PlciDataAccessService == null)
                    _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

                if (_PlciDataAccessService != null)
                {
                    Subscription subscription = _PlciDataAccessService.CreateSubscription(addressList);
                    // return null when some var are not present into PLC
                    if (subscription != null)
                    {
                        int subscriptionId = _PlciSubscription.Count + 1;
                        _PlciSubscription[subscriptionId] = subscription;
                        for (int i=0; i< subscriptionJobsList.Count; i++)
                        {
                            subscriptionJobsList[i].SubscriptionId = subscriptionId;
                            subscriptionJobsList[i].SubscriptionElementNumber = i;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }             
        }

        /// <summary>
        /// Subscribe input and input output jobs to Phoenix PLCI library
        /// </summary>
        private void SubscribeAllJobs()
        {            
            PhoenixContactPLCIStation s = (PhoenixContactPLCIStation)CommDriver.GetChannelStations(this)[0];
            List<PhoenixContactPLCICommJob> jobsList = (s.GetListWholeJobCopy().Cast<PhoenixContactPLCICommJob>().ToList()).FindAll(j=> (j.Type == LinkType.Input || j.Type == LinkType.InputOutput) && !j.IsReadWriteInvalid());
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} SubscribeAllJobs", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")));
#endif

            IList<PhoenixContactPLCICommJob> subscriptionJobsList = new List<PhoenixContactPLCICommJob>();
            uint nCurrentMessageByteSize = 0;

            int i = 0;
            while (i < jobsList.Count)
            {
                bool Added = false;
                PhoenixContactPLCICommJob j = jobsList[i];

                // for big size job create a single subscription
                if (PhoenixContactPLCIProtocol.IfTagSizeTooBigForSubScription(j.TotalJobSize))
                {
                    CreateSubscription(new List<PhoenixContactPLCICommJob>() { j });
                    Added = true;
                }
                else
                {
                    Added = PhoenixContactPLCIProtocol.CheckIfTagCanBeInsertTheSubScriptionList(j.TotalJobSize, ref nCurrentMessageByteSize);
                    if (Added)
                        subscriptionJobsList.Add(j);
                }

                // last element of list or list of variables complete (total request size < MAXIMUM SIZE)
                if (subscriptionJobsList.Count>0 && (!Added || i == (jobsList.Count - 1)))
                {
                    CreateSubscription(subscriptionJobsList);

                    nCurrentMessageByteSize = 0;
                    subscriptionJobsList.Clear();

                    // if not added, reprocess next cycle
                    if (!Added)
                        i--;
                }
 
                // move to next job
                i++;
            }            
        }

        private bool ConnectionStateChanged(out PhoenixContactPLCIProtocol.ConnectionState previuousConnectionState, bool firstTime = false)
        {
            bool statusChanged = false;

            bool IsRunning = (GetState() == PlcState.PlcRunning);

            previuousConnectionState = _PreviousConnectionState;

            if (_CurrentConnectionState != _PreviousConnectionState || firstTime)
            {
                // update status variable
                SetStateCommandVariableBit(!IsRunning, (UInt16)ChannelVariableBits.ChannelUnconnected);
                foreach (var station in CommDriver.GetChannelStations(this))
                    ((PhoenixContactPLCIStation)station).SetStateCommandVariableBit(!IsRunning, (UInt16)StationVariableBits.StationErrorState);                

                statusChanged = true;

                previuousConnectionState = _PreviousConnectionState;

                _PreviousConnectionState = _CurrentConnectionState;
            }
            else
            {
                // force status changed on Disconnected to enable driver to "retest" connection with devivceopen
                statusChanged = (_CurrentConnectionState == PhoenixContactPLCIProtocol.ConnectionState.Disconnected);
            }

            return statusChanged;
        }

        private PlcState GetState()
        {

            PlcState state = PlcState.PlcStop;

            try
            {
                if (_PlciDeviceAttributeService == null)
                {
                    _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;
                    state = PlcState.PlcStop;
                }
                else
                {
                    try
                    {
                        state = (PlcState)(_PlciDeviceAttributeService.GetAttribute<UInt32>(StandardDeviceAttribute.PlcState));
                        if (state == PlcState.PlcRunning)
                            _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.ConnectedAndRunning;
                        else
                            _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Connected;
                    }
                    catch (Exception ex)
                    {
                        _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;
                    }
                }
            }
            catch (Exception ex)
            {
                _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;
            }

            return state;
        }

        public override bool IsDeviceOpen()
        {
            return (_CurrentConnectionState == PhoenixContactPLCIProtocol.ConnectionState.ConnectedAndRunning);
        }

        public override bool DeviceOpen()
        {           
            PhoenixContactPLCIProtocol.ConnectionState connectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;
            bool fatalError = false;

            try
            {
                if (_Plci != null || _PlciDeviceAttributeService != null)
                    DeviceClose();

                _Plci = new Plci();
                _Plci.Connect(ServerAddress, (ushort)ServerPort, (ushort)Timeout);                
                _PlciDeviceAttributeService = _Plci.GetService<IDeviceAttributeService>();
                connectionState = PhoenixContactPLCIProtocol.ConnectionState.Connected;
            }
            catch (PlciException ex)
            {
                fatalError = true;
            }
            catch (Exception ex)
            {                
                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ErrorExceptionDeviceOpen,ex.Message), EventSeverity.High);
                fatalError = true;
            }

            if ((connectionState == PhoenixContactPLCIProtocol.ConnectionState.Disconnected) && connectionState != _CurrentConnectionState)
            {
                // import direct from PLC; no station's list available --> don't log error
                if (CommDriver.GetChannelStations(this).Count > 0)
                    CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.StationInError, CommDriver.DriverName, CommDriver.GetChannelStations(this)[0].Name, Properties.Resources.DeviceDisconnected), EventSeverity.Low);
            }

            if (fatalError)
            {
                // reset PLCI objects
                DeviceClose();
            }
            else
            {                
                PhoenixContactPLCIProtocol.ConnectionState previuousConnectionState;
                ConnectionStateChanged(out previuousConnectionState);
                _PreviousConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Unknown;
            }

            // reset last access to device
            _LastDeviceCompletedRequest = DateTime.MinValue;
            
            return (IsDeviceOpen());
        }
       
        public override bool DeviceClose()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} DeviceClose", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")));
#endif

            if (SubscribeVariables)
                UnSubscribeAllJobs();

            if (_PlciDeviceAttributeService != null)
            {
                _PlciDeviceAttributeService.Dispose();
                _PlciDeviceAttributeService = null;
            }

            if (_PlciDataAccessService != null)
            {
                _PlciDataAccessService.Dispose();
                _PlciDataAccessService = null;
            }

            if (_Plci != null)
            {
                _Plci.Dispose();
                _Plci = null;
            }

            _CurrentConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;
            _PreviousConnectionState = PhoenixContactPLCIProtocol.ConnectionState.Disconnected;

            return true;
        }

        /// <summary>
        /// Check of device communication state (and reset timeout) when no jobs are executed since xxx time
        /// </summary>
        /// <returns></returns>
        private bool IsTimeToCheckConnectionState()
        {
            // if no pending jobs, check every xxx time if still connected (and running)
            if (DateTime.UtcNow.Subtract(_LastDeviceCompletedRequest).TotalMilliseconds > Properties.Settings.Default.CheckStateNoJobsPollingTime)
            {
                SetLastTimeCompletedRequest();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetLastTimeCompletedRequest()
        {
            _LastDeviceCompletedRequest = DateTime.UtcNow;
        }

        private void ManageDeviceConnection(bool firstTime = false)
        {
            if (ConnectionStateChanged(out PhoenixContactPLCIProtocol.ConnectionState previousConnectionState, firstTime))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} changed state from {1} to {2}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), previousConnectionState, _CurrentConnectionState));
#endif
                switch (_CurrentConnectionState)
                {
                    case PhoenixContactPLCIProtocol.ConnectionState.Disconnected:
                        if (previousConnectionState != PhoenixContactPLCIProtocol.ConnectionState.Disconnected)
                        {
                            ResetAllJobs();
                            SetJobsInError((DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken);
                            DeviceClose();
                        }
                        DeviceOpen();
                        break;

                    case PhoenixContactPLCIProtocol.ConnectionState.Connected:
                        ResetAllJobs();
                        SetJobsInError((DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken);                        
                        break;

                    case PhoenixContactPLCIProtocol.ConnectionState.ConnectedAndRunning:
                        ResetAllJobs();
                        if (SubscribeVariables)
                            SubscribeAllJobs();
                        break;         
                        
                    default:
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} unmanage state to {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), _CurrentConnectionState));
#endif
                        break;
                }
            }
        }

        public void ResetAllJobs()
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("ManageDeviceConnection {0} ResetAllJobs", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")));
#endif

            if (SubscribeVariables)
                UnSubscribeAllJobs();

            PhoenixContactPLCIStation s = (PhoenixContactPLCIStation)CommDriver.GetChannelStations(this)[0];
            
            List<CommJob> list = s.GetListWholeJobCopy();
            Parallel.ForEach(list, job =>
            {
                ((PhoenixContactPLCICommJob)job).ResetInternalElements();
            });
        }


        /// <summary>
        /// Get bin's file full path depending of Movicon project type format (db, file)
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private string AdjustBinFilePath(string conn, string fileNameOnly)
        {            
            ConnectionStringParser helper = new ConnectionStringParser(conn);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            string path = null;

            // from db
            if (providerType != InMemoryDataStore.XpoProviderTypeString)
            {
                path = Path.Combine(Path.GetTempPath(), fileNameOnly);
            }
            else // from file
            {
                string filebase = XpoHelpers.XpoHelper.GetDataSourceFilePath(conn);
                InMemoryDataStore InMemory = CommunicationDriver.GetDataStore(filebase);
                if (!string.IsNullOrWhiteSpace(filebase) && InMemory != null)
                    path = filebase;
            }

            return path;
        }

        /// <summary>
        /// Get bin's file with full path
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        private string GetBinFileFile(string stationName)
        {
            PhoenixContactPLCIDriver PlciCommDriver = (PhoenixContactPLCIDriver)CommDriver;

            string fileNameOnly = string.Format("{0}_PhoenixPlci_ImportFromPlc", stationName);

            string connect = CommunicationDriver.GetConnectionString(PlciCommDriver.strSettingPath, "Drivers", fileNameOnly, ".bin");

            string binFile = AdjustBinFilePath(connect, fileNameOnly);

            return binFile;
        }

        /// <summary>
        /// Get vars's list from plc downloading as .bin file (like .bin file created from project)
        /// </summary>
        /// <param name="binFile"></param>
        /// <returns></returns>
        public PhoenixContactPLCIProtocol.ErrorCodes GetBinFileFromDevice(out string binFile, string stationName, out string errorMessage)
        {
            PhoenixContactPLCIProtocol.ErrorCodes ret = (PhoenixContactPLCIProtocol.ErrorCodes)DriverErrorCodes.ErrorNoError;

            binFile = null;
            errorMessage = string.Empty;

            DeviceOpen();
            // not connected or plc is not in running
            if (!IsDeviceOpen())
            {
                DeviceClose();

                errorMessage = Properties.Resources.ErrorConnection;
                return PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken;
            }

            if (ret == (PhoenixContactPLCIProtocol.ErrorCodes)DriverErrorCodes.ErrorNoError) {
                // get target bin file
                binFile = GetBinFileFile(stationName);
                // invalid bin file target path
                if (string.IsNullOrEmpty(binFile)) {
                    errorMessage = string.Format(Properties.Resources.ErrorGetFromPlcBadBinFilePath, binFile);
                    ret = PhoenixContactPLCIProtocol.ErrorCodes.ErrorGetFromPlcBadBinFilePath;
                }
                else
                {
                    try
                    {
                        using (IFileService plcFileService = _Plci.GetService<IFileService>())
                        {
                            byte[] metadata = null;
                            if (plcFileService != null)
                            {
                                int readHandle = plcFileService.OpenRead(PhoenixContactPLCIProtocol.PLC_IMPORT_SOURCE_PATH);

                                if (readHandle > 0)
                                {
                                    MemoryStream memory = new MemoryStream();
                                    bool continueRead = true;
                                    while (continueRead)
                                    {
                                        byte[] byteData;
                                        continueRead = plcFileService.Read(readHandle, out byteData, 10000);
                                        memory.Write(byteData, 0, byteData.Length);
                                    }

                                    plcFileService.Close(readHandle);

                                    metadata = memory.ToArray();
                                }
                            }

                            if (metadata != null)
                            {
                                try
                                {
                                    // if target file already exist delete it
                                    if (File.Exists(binFile))
                                        File.Delete(binFile);

                                    // write to file imported data from PLC
                                    File.WriteAllBytes(binFile, metadata);
                                } catch (Exception ex)
                                {
                                    errorMessage = string.Format(Properties.Resources.ErrorGetFromPlcCannotWriteBinFile, binFile);
                                    ret = PhoenixContactPLCIProtocol.ErrorCodes.ErrorGetFromPlcCannotWriteBinFile;
                                }
                            }
                        }
                    } catch (PlciException ex)
                    {
                        errorMessage = string.Format(Properties.Resources.ErrorGetFromPlcGenericPlciException, binFile);
                        ret = PhoenixContactPLCIProtocol.ErrorCodes.ErrorGenericPlciException;
                    }
                }
            }

            DeviceClose();

            if (ret != (PhoenixContactPLCIProtocol.ErrorCodes)DriverErrorCodes.ErrorNoError)
                binFile = null;

            return ret;
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if (StatisticsData != null)
            {
                // set exchanged quantity of data for specific job
                DiagnLastTaskRxBytes = ((PhoenixContactPLCICommJob)e.Job).DiagnRxBytes;
                DiagnLastTaskTxBytes = ((PhoenixContactPLCICommJob)e.Job).DiagnTxBytes;
                ((PhoenixContactPLCICommJob)e.Job).ResetDiagnRxTxBytes();
            }
            base.OnJobExecuted(e);
        }

        protected void SetJobsInError(DriverErrorCodes error)
        {
            foreach (var station in CommDriver.GetChannelStations(this))
                ((PhoenixContactPLCIStation)station).SetGeneralError(error);

            LastErrorCode = error;
        }
        #endregion

        #region methods

        protected void GetNextPendingList(ref List<PhoenixContactPLCICommJob> list)
        {
            bool write = false;
            string station = string.Empty;
            bool first = true;
            bool? subscribeVariablesReadMode = null;
            

            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue(); 
                if (queue == null)
                    return;

                int nCurrentMessageSize = 0;
                int nCurrentMessageByteSize = 0;
                bool exitFromDeQueuingJob = false;
                // limit the nr max of vars 
                while (!queue.IsEmpty && !exitFromDeQueuingJob) {                    
                    PhoenixContactPLCICommJob j = queue.ElementAt(0) as PhoenixContactPLCICommJob;
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
                            {
                                // from 1st job set witch job's type (subscribed or not) add to list
                                if (!subscribeVariablesReadMode.HasValue)
                                    subscribeVariablesReadMode = j.IsSubscribed();

                                if (subscribeVariablesReadMode.HasValue && subscribeVariablesReadMode.Value)
                                {
                                    // if job was suscribed add to list
                                    if (j.IsSubscribed())
                                        list.Add(j);
                                }
                                else
                                {
                                    // try create a list of job that be contain in a single data frame
                                    if (PhoenixContactPLCIProtocol.CheckIfTagCanBeInsertTheReadList(j.Address, (int)j.TotalJobSize, ref nCurrentMessageSize, ref nCurrentMessageByteSize)) {
                                        list.Add(j);
                                    }
                                    else
                                    {
                                        if (list.Count == 0)
                                            list.Add(j);
                                        exitFromDeQueuingJob = true;                                            
                                    }
                                }
                            }
                        }
                    }
                    CommJob dequeuedJob = null;
                    queue.TryDequeue(out dequeuedJob);
                }
            }
        }

        private void GetNextSynchroJob(ref List<PhoenixContactPLCICommJob> list)
        {
            PhoenixContactPLCICommJob job = SynchroJob as PhoenixContactPLCICommJob;

            list.Add(job);
        }

        private DriverErrorCodes ReadDataSubscribedVars(ref List<PhoenixContactPLCIProtocol.VarRead> varReadList, ref IList<object> readedValues)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            Dictionary<int, List<PhoenixContactPLCIProtocol.VarRead>> mapSubscriptionToRead = new Dictionary<int, List<PhoenixContactPLCIProtocol.VarRead>>();

            readedValues = new List<object>();
            // init required array of values with "no data"
            for (int i = 0; i < varReadList.Count; i++)
                readedValues.Add(null);

            // create a map of subscription to be read based on suscription id
            foreach (var readVar in varReadList)
            {
                if (!mapSubscriptionToRead.ContainsKey(readVar.Job.SubscriptionId))
                    mapSubscriptionToRead[readVar.Job.SubscriptionId] = new List<PhoenixContactPLCIProtocol.VarRead>();
                mapSubscriptionToRead[readVar.Job.SubscriptionId].Add(readVar);
            }
            
            foreach (var subcription in mapSubscriptionToRead)
            {
                IList<object> subscriptionReadedValues = null;

                try
                {
                    if (_PlciDataAccessService == null)
                        _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

                    if (_PlciDataAccessService != null)
                    {
                        // get data from subscription
                        subscriptionReadedValues = _PlciDataAccessService.GetResponse(_PlciSubscription[subcription.Key]);
                    }
                }
                catch (FatalPlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorFatalPlciException;
                }
                catch (RecoverablePlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
                }
                catch (PlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorGenericPlciException;
                }

                // set in error all job 
                if (ret != DriverErrorCodes.ErrorNoError)
                {
                    Parallel.ForEach(varReadList, varRead =>
                    {
                        varRead.ErrorCode = ret;
                    });
                }
                else
                {
                    // for each jobs associated to subscription
                    foreach (PhoenixContactPLCIProtocol.VarRead varRead in subcription.Value) {
                        if (subscriptionReadedValues != null)
                            readedValues[varRead.ReadValueIndex] = subscriptionReadedValues[varRead.Job.SubscriptionElementNumber];
                        else
                            varRead.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
                    }
                }                        
            }

            return ret;
        }

        private DriverErrorCodes ReadDataUnSubscribedVars(ref List<PhoenixContactPLCIProtocol.VarRead> varReadList, ref IList<object> readedValues)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            try
            {                                
                if (_PlciDataAccessService == null)
                    _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

                if (_PlciDataAccessService != null)
                {                   
                    IList<string> addressList = varReadList.Select(x => x.Address).ToList();
                    readedValues = _PlciDataAccessService.ReadVariables(addressList);
                }
            }
            catch (FatalPlciException ex)
            {
                ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorFatalPlciException;
            }
            catch (RecoverablePlciException ex)
            {
                ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
            }
            catch (PlciException ex)
            {
                ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorGenericPlciException;
            }

            #region try to read again one value at time to manage unmapped var for not subscribed var
            //if (ret == (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException)
            //{
            //    init required array of values with "no data"
            //    readedValues = new List<object>();
            //    for (int i = 0; i < varReadList.Count; i++)
            //        readedValues.Add(null);

            //    if (ret == (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException)
            //    {
            //        try
            //        {
            //            if (_PlciDataAccessService == null)
            //                _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

            //            if (_PlciDataAccessService != null)
            //            {
            //                for (int i = 0; i < varReadList.Count; i++)
            //                {
            //                    PhoenixContactPLCIProtocol.VarRead varRead = varReadList[i];
            //                    try
            //                    {
            //                        object readedValue = _PlciDataAccessService.ReadVariables(new List<string> { varRead.Address });
            //                        if (readedValue != null)
            //                            readedValues[i] = readedValue;
            //                    }
            //                    catch (PlciException ex) { }
            //                }
            //            }
            //        }
            //        catch (Exception ex) { }

            //        ret = DriverErrorCodes.ErrorNoError;
            //    }
            //}
            #endregion

            if (ret != DriverErrorCodes.ErrorNoError)
            {
                Parallel.ForEach(varReadList, varRead =>
                {
                    varRead.ErrorCode = ret;
                });
            }

            return ret;
        }

        private DriverErrorCodes ReadData(List<PhoenixContactPLCICommJob> list)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            IList<object> readedValues = null;
            List<PhoenixContactPLCIProtocol.VarRead> varReadList = new List<PhoenixContactPLCIProtocol.VarRead>();
            bool subscribeVariablesReadMode = false;            

            // from 1st job set witch job's type (subscribed or not) add to list
            if (list.Count>0)
                subscribeVariablesReadMode = list[0].IsSubscribed();
           
            for (int i = 0; i < list.Count; i++)
            {
                PhoenixContactPLCICommJob j = list[i];

                base.ExecuteJob(j);

                //if (j.IsReadWriteInvalid())
                //{
                //    ExecutedJobArgs eJob = new ExecutedJobArgs();
                //    eJob.ErrorCode = (DriverErrorCodes)j.ReadWriteInvalidationErrorCode;
                //    j.LastExecutionTime = DateTime.UtcNow;
                //    eJob.Job = j;                    
                //    OnJobExecuted(eJob);
                //}
                //else
                //{                        
                    varReadList.Add(new PhoenixContactPLCIProtocol.VarRead(j, varReadList.Count));
                //}
            }

            if (!IsDeviceOpen())
            {
                ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken;
            }
            else
            {
                if (varReadList.Count > 0)
                {
                    if (subscribeVariablesReadMode)
                        ret = ReadDataSubscribedVars(ref varReadList, ref readedValues);
                    else
                        ret = ReadDataUnSubscribedVars(ref varReadList, ref readedValues);
                }
            }

            if (ret == DriverErrorCodes.ErrorNoError)
                SetLastTimeCompletedRequest();

            foreach (PhoenixContactPLCIProtocol.VarRead varRead in varReadList)
            {
                PhoenixContactPLCICommJob j = varRead.Job;
                ExecutedJobArgs eJob = new ExecutedJobArgs();

                // generic error
                if (ret == DriverErrorCodes.ErrorNoError)
                {
                    // specific value reading error
                    if (varRead.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        if (IsDeviceOpen())
                        {
                            if (StatisticsData != null)
                                // increase receive bytes diagnostic var
                                j.DiagnRxBytes += j.TotalJobSize;
                        }

                        // null value mean no value mapped 
                        object varValue = readedValues[varRead.ReadValueIndex];
                        if (varValue != null)
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension != 0)
                            {
                                if (PhoenixContactPLCIProtocol.GetByteFromReadedArrayOfValues(j.DataFormat, j.TagsList[0].TagNode.ArrayDimension, j.StringLength, varValue, j, out byte[] result, out PhoenixContactPLCIProtocol.ErrorCodes errorCode)) {
                                    eJob.Values = result;
                                }
                                else
                                {
                                    eJob.ErrorCode = (DriverErrorCodes)errorCode;
                                    // error in data conversion or array size don't match with tag's definition
                                    j.ReadWriteInvalidationErrorCode = errorCode;
                                }
                            }
                            else
                            {
                                if (PhoenixContactPLCIProtocol.GetByteFromReadedValue(j.DataFormat, varValue, out byte[] result, out PhoenixContactPLCIProtocol.ErrorCodes errorCode))
                                {
                                    eJob.Values = result;
                                }
                                else
                                {
                                    eJob.ErrorCode = (DriverErrorCodes)errorCode;
                                    // error in data conversion or array size don't match with tag's definition
                                    j.ReadWriteInvalidationErrorCode = errorCode;
                                }
                            }
                        }
                        else
                        {
                            j.ReadWriteInvalidationErrorCode = PhoenixContactPLCIProtocol.ErrorCodes.ErrorUnMappedTag;
                            eJob.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorUnMappedTag;
                        }
                    }
                    else
                    {
                        eJob.ErrorCode = varRead.ErrorCode;
                    }
                }
                else 
                {
                    eJob.ErrorCode = ret;
                }

                j.LastExecutionTime = DateTime.UtcNow;
                eJob.Job = j;
                OnJobExecuted(eJob);
            }

            return ret;
        }

        //protected void ReadData(List<PhoenixContactPLCICommJob> list)
        //{
        //    DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
        //    IList<object> readedValues = null;
        //    List<PhoenixContactPLCIProtocol.VarRead> varReadList = new List<PhoenixContactPLCIProtocol.VarRead>();

        //    try
        //    {
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            PhoenixContactPLCICommJob j = list[i];

        //            base.ExecuteJob(j);

        //            if (j.UnMapped)
        //            {
        //                ExecutedJobArgs eJob = new ExecutedJobArgs();
        //                eJob.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorUnMappedTag;
        //                eJob.Job = j;
        //                OnJobExecuted(eJob);
        //            }
        //            else
        //            {
        //                varReadList.Add(new PhoenixContactPLCIProtocol.VarRead(j, varReadList.Count));
        //            }
        //        }

        //        if (!IsDeviceOpen())
        //        {
        //            ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken;
        //        }
        //        else
        //        {

        //            if (_PlciDataAccessService == null)
        //                _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

        //            if (_PlciDataAccessService != null)
        //            {
        //                if (SubscribeVariables)
        //                {
        //                    readedValues = _PlciDataAccessService.GetResponse(_PlciSubscription[list[0].IndexDASubscription]);
        //                }
        //                else
        //                {
        //                    if (varReadList.Count > 0)
        //                    {
        //                        IList<string> addressList = varReadList.Select(x => x.Address).ToList();
        //                        readedValues = _PlciDataAccessService.ReadVariables(addressList);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (FatalPlciException ex)
        //    {
        //        ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorFatalPlciException;
        //    }
        //    catch (RecoverablePlciException ex)
        //    {
        //        ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
        //    }
        //    catch (PlciException ex)
        //    {
        //        ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorGenericPlciException;
        //    }


        //    #region try to read again one value at time to manage unmapped var for not subscribed var
        //    if (!SubscribeVariables && ret == (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException)
        //    {
        //        readedValues = new List<object>();
        //        try
        //        {
        //            if (_PlciDataAccessService == null)
        //                _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

        //            if (_PlciDataAccessService != null)
        //            {
        //                foreach (PhoenixContactPLCIProtocol.VarRead varRead in varReadList)
        //                {
        //                    bool readError = false;
        //                    try
        //                    {
        //                        IList<string> addressList = new List<string> { varRead.Address };

        //                        readedValues = _PlciDataAccessService.ReadVariables(addressList);
        //                    }
        //                    catch (PlciException ex)
        //                    {
        //                        readError = true;
        //                    }
        //                    if (readError)
        //                        readedValues.Add(null);
        //                    else
        //                        readedValues.Add(readedValues[0]);
        //                }
        //            }
        //        }
        //        catch (Exception ex) { }

        //        ret = DriverErrorCodes.ErrorNoError;
        //    }
        //    #endregion

        //    foreach (PhoenixContactPLCIProtocol.VarRead varRead in varReadList)
        //    {
        //        PhoenixContactPLCICommJob j = varRead.Job;
        //        ExecutedJobArgs eJob = new ExecutedJobArgs();

        //        if (ret == DriverErrorCodes.ErrorNoError)
        //        {
        //            object varValue = readedValues[varRead.ReadValueIndex];
        //            if (varValue != null)
        //            {
        //                //Console.WriteLine(varValue.ToString());
        //                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
        //                if (j.TagsList[0].TagNode.ArrayDimension != 0)
        //                {
        //                    eJob.Values = PhoenixContactPLCIProtocol.GetByteFromReadedArrayOfValues(j.DataFormat, j.TagsList[0].TagNode.ArrayDimension, varValue);
        //                }
        //                else
        //                { // default --> number
        //                    byte[] result = PhoenixContactPLCIProtocol.GetByteFromReadedValue(j.DataFormat, varValue);
        //                    // invalid conversion type --> 
        //                    if (result == null)
        //                    {
        //                        eJob.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorInvalidDataFormat;
        //                    }
        //                    else
        //                    {
        //                        eJob.Values = PhoenixContactPLCIProtocol.GetByteFromReadedValue(j.DataFormat, varValue);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                j.UnMapped = true;
        //                eJob.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorUnMappedTag;
        //            }
        //        }
        //        else
        //        {
        //            eJob.ErrorCode = ret;
        //        }

        //        //base.ExecuteJob(j);
        //        eJob.Job = j;
        //        OnJobExecuted(eJob);
        //    }
        //}

        private DriverErrorCodes WriteData(List<PhoenixContactPLCICommJob> list) {

            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            List<PhoenixContactPLCIProtocol.VarWrite> varWriteList = new List<PhoenixContactPLCIProtocol.VarWrite>();
            
            foreach (PhoenixContactPLCICommJob j in list)
            {
                PhoenixContactPLCIProtocol.VarWrite writeValue = null;

                base.ExecuteJob(j);

                object objectData = null;
                j.GetJobData(ref objectData);

                if (j.TagsListOnWriting.Count == 0)
                {
                    j.IsPending = false;
                    continue;
                }
                byte[] jobdata = (byte[])objectData;
                int lengthBuff = jobdata.Length;

                if (j.IsReadWriteInvalid())
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)j.ReadWriteInvalidationErrorCode;
                    j.LastExecutionTime = DateTime.UtcNow;
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                }
                else
                {
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                    {
                        uint stringLength = j.StringLength;
                        //With tag, of type string array, in writing the buffer coming from movicon is considered, 
                        //divided by the number of elements, to obtain the single dimension of the element
                        if (j.DataFormat == PhoenixContactPLCIProtocol.VarType.STRING )
                        {
                            stringLength = ((uint)jobdata.Length / j.TagsList[0].TagNode.ArrayDimension);
                        }
                        writeValue = new PhoenixContactPLCIProtocol.VarWrite(j, PhoenixContactPLCIProtocol.GetObjectFromWriteArratOfValues(j.DataFormat, jobdata, j.TagsList[0].TagNode.ArrayDimension, stringLength));
                    }
                    else
                        writeValue = new PhoenixContactPLCIProtocol.VarWrite(j, PhoenixContactPLCIProtocol.GetObjectFromWriteValue(j.DataFormat, jobdata));

                    if (writeValue != null)
                    {
                        varWriteList.Add(writeValue);

                        if (IsDeviceOpen())
                        {
                            if (StatisticsData != null)
                            {
                                j.DiagnTxBytes += j.TotalJobSize + j.Address.Length;
                            }
                        }
                    }
                }
            }

            if (!IsDeviceOpen())
            {
                ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken;
            }
            else
            {
                try
                {
                    if (_PlciDataAccessService == null)
                        _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

                    if (_PlciDataAccessService != null)
                    {
                        IList<string> addressList = varWriteList.Select(x => x.Address).ToList();
                        IList<object> valuesList = varWriteList.Select(x => x.Data).ToList();
                        _PlciDataAccessService.WriteVariables(addressList, valuesList);
                    }
                }
                catch (FatalPlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorFatalPlciException;
                }
                catch (RecoverablePlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
                }
                catch (PlciException ex)
                {
                    ret = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorGenericPlciException;
                }                
            }

            if (ret == DriverErrorCodes.ErrorNoError)
                SetLastTimeCompletedRequest();

            #region try to write again one value at time to manage unmapped var for not subscribed var
            if (!SubscribeVariables && ret == (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException)
            {                
                try
                {
                    if (_PlciDataAccessService == null)
                        _PlciDataAccessService = _Plci.GetService<IDataAccessService>();

                    if (_PlciDataAccessService != null)
                    {
                        foreach (PhoenixContactPLCIProtocol.VarWrite varWrite in varWriteList)
                        {
                            bool writeError = false;
                            try
                            {
                                IList<string> addressList = new List<string> { varWrite.Address };
                                IList<object> valuesList = new List<object> { varWrite.Data };
                                _PlciDataAccessService.WriteVariables(addressList, valuesList);
                            }
                            catch (PlciException ex)
                            {
                                writeError = true;
                            }
                            if (writeError)
                                varWrite.ErrorCode = (DriverErrorCodes)PhoenixContactPLCIProtocol.ErrorCodes.ErrorRecoverablePlciException;
                        }
                    }
                }
                catch (Exception ex) { }

                ret = DriverErrorCodes.ErrorNoError;
            }
            #endregion

            System.Diagnostics.Debug.WriteLine(string.Format("WriteData nr elements : {0}",varWriteList.Count()));

            foreach (PhoenixContactPLCIProtocol.VarWrite varWrite in varWriteList)
            {
                PhoenixContactPLCICommJob j = varWrite.Job;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                if (varWrite.ErrorCode != DriverErrorCodes.ErrorNoError)
                    eJob.ErrorCode = varWrite.ErrorCode;
                else
                    eJob.ErrorCode = ret;
                j.LastExecutionTime = DateTime.UtcNow;
                eJob.Job = j;
                OnJobExecuted(eJob);
            }

            return ret;
        }

        protected DriverErrorCodes ExecuteJobList(List<PhoenixContactPLCICommJob> list)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

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
                ret = ReadData(list);
            else
                ret = WriteData(list);

            return ret;
        }
        
        public CommunicationDriver getCommDriver()
        {
            return CommDriver;
        }

        #endregion

        #region Properties

        /// <summary>
        /// IP Address or name of Device
        /// </summary>
        private string _ServerAddress;
        public string ServerAddress
        {
            get { return _ServerAddress; }
            set { _ServerAddress = value; }
        }
        
        /// <summary>
        /// Port Number
        /// </summary>
        private uint _ServerPort;
        public uint ServerPort
        {
            get { return _ServerPort; }
            set { _ServerPort = value; }
        }

        /// <summary>
        /// Subscribe Variables to PLC 
        /// </summary>
        private bool _SubscribeVariables;
        public bool SubscribeVariables
        {
            get { return _SubscribeVariables; }
            set { _SubscribeVariables = value; }
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


