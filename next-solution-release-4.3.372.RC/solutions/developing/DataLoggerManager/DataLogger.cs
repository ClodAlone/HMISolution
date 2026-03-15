using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Amib.Threading;
using DataLoggerModel;
using DataReader;
using log4net;
using Utilities;
#if !NET_STANDARD
using DataLoggerManager.Redundancy;
#endif
using Utilities.DirectorySizeHelper;
using DataReader.Helpers;
using DataReader.Extensions;
using DataReader.SchemaInfo;

namespace DataLoggerManager
{
    public class DataLogger : IDisposable
    {
        #region Declarations
        readonly Object lockObject = new Object();
        readonly Dictionary<String, DataLoggerState> mapDataLoggerState = new Dictionary<String, DataLoggerState>();
        readonly Dictionary<String, Tuple<DataWriter.DataSetWriter, DateTime>> mapCacheDataWriter = new Dictionary<String, Tuple<DataWriter.DataSetWriter, DateTime>>();
        readonly Dictionary<String, DbSchemaInfo> mapDbSchemaInfo = new Dictionary<String, DbSchemaInfo>();

        readonly Dictionary<String, Timer> mapDataLoggerToHysteresisTimer = new Dictionary<String, Timer>();
        readonly Dictionary<String, Timer> mapDataLoggerToMaxTimer = new Dictionary<String, Timer>();
        readonly Dictionary<String, DataLoggerEntity> mapDataLoggerToEntity = new Dictionary<String, DataLoggerEntity>();
        readonly Dictionary<String, DateTime> mapLastTimeUpdated = new Dictionary<String, DateTime>();

        readonly Dictionary<String, List<String>> pendingColumnNames = new Dictionary<String, List<String>>();
        readonly Dictionary<String, DataLoggerEntity> pendingSnapshot = new Dictionary<String, DataLoggerEntity>();

        readonly List<DataLoggerState> pendingDataLoggers = new List<DataLoggerState>();
        readonly List<String> pendingCheckConnection = new List<String>();
        readonly List<String> applyCFR21Requirements = new List<String>();

        readonly DataLoggerConfiguration defaultConfiguration;

        readonly String defaultDataProvider;
        readonly String defaultConnectionString;
        readonly String defaultSafeDataPath;
        
        readonly SmartThreadPool smartThreadPool;
        readonly RestoreDataManager.RestoreDataHelper restoreDataManager;
#if !NET_STANDARD
        RedundancyHistory.IRedundancyHistory RedundancySyncData;
        Timer redundancyCheckDeleteOldData;
#endif
        Timer timerCloseConnection;
        bool isRunningAsCFR21UserName;

        IWorkItemsGroup startingWorkItemsGroup;

#if !NET_STANDARD
        static readonly ILog logDataLogger = LogManager.GetLogger(Properties.Resources.DataLoggerLogName);
#else
        static readonly ILog logDataLogger = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.DataLoggerLogName);
#endif


        Thread logThread;
        AutoResetEvent logEvent;
        bool bStarted;
        bool ExitMode;
        bool bDisposed;
        bool writeErrorForQualityCheckFail = true;

#endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of Data Logger Manager by pass XPObject connection string as default connection.
        /// </summary>
        public DataLogger(DataLoggerConfiguration configuration)
        {
            defaultConfiguration = configuration;
            defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(defaultConfiguration.xpoDataConnectionString);
            defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(defaultConfiguration.xpoDataConnectionString);
            defaultSafeDataPath = XpoHelpers.XpoHelper.GetDataSourceFilePath(defaultConfiguration.xpoSafeDataConnectionString);
            if (!String.IsNullOrEmpty(defaultSafeDataPath))
            {
                defaultSafeDataPath = String.Format("{0}{2}{1}{2}", 
                    System.IO.Path.GetDirectoryName(defaultSafeDataPath),
                    UFUAServerInfo.Properties.Settings.Default.DataLoggerFlushFolderName,
                    System.IO.Path.DirectorySeparatorChar);
            }
            // Create smart thread pool for handle deleting and flushing
            var startupInfo = new STPStartInfo()
            {
                ThreadPoolName = "DataLoggerThreadPool",
                ThreadPriority = System.Threading.ThreadPriority.BelowNormal,
                MaxWorkerThreads = SysInfo.GetNumberOfLogicalProcessors(),
                AreThreadsBackground = false
            };
            smartThreadPool = new SmartThreadPool(startupInfo);

            if (defaultConfiguration.MaxRestoreProcess < 0)
                defaultConfiguration.MaxRestoreProcess = SysInfo.GetNumberOfLogicalProcessors();

            if (defaultConfiguration.MaxRestoreProcess > 0)
            {
                // Create a new instance of RestoreDataManager.
                restoreDataManager = new RestoreDataManager.RestoreDataHelper(RestoreDataManager.DBSchemaType.DataLogger, logDataLogger);
                restoreDataManager.MaxRestoreProcess = defaultConfiguration.MaxRestoreProcess;
                restoreDataManager.IsRedundancyServer = defaultConfiguration.redundancyServerId >= 0;
            }

            try
            {
                isRunningAsCFR21UserName = CurrentUser.IsEqualTo(UFUAServerInfo.UFUAServerInfo.GetCFR21UserName(), UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName());
            }
            catch
            { }

#if !NET_STANDARD
            // Create a new instance of IRedundancySyncData
            if (defaultConfiguration.redundancyServerId >= 0)
            {
                RedundancySyncData = new DataLoggerManager.Redundancy.DataLoggerSyncronization(defaultConfiguration.redundancyMaxSyncRecords,
                    defaultConfiguration.redundancyHistoryThreadPool,
                    logDataLogger);
            }
#endif
        }
#endregion

#region Events
        public event EventHandler<FlushedDataSafelyEventArgs> FlushedDataSafely;

        void OnFlushedDataSafely(FlushedDataSafelyEventArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = FlushedDataSafely;
            if (e != null)
                e(this, ea);
        }

        public event EventHandler<ErrorStateChangedEventArgs> ErrorStateChanged;

        void OnErrorStateChanged(ErrorStateChangedEventArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = ErrorStateChanged;
            if (e != null)
                e(this, ea);
        }
#endregion

#region Public Methods
        /// <summary>
        /// Initialize a data logger for properly working with data logger manager.
        /// </summary>
        /// <param name="settings"></param>
        public bool Init(DataLoggerSettings settings)
        {
            DataLoggerState dataLoggerState = null;
            lock (lockObject)
            {
                settings.IsRunningInstance = true;
                // Ensure valid connection settings by getting the default settings if necessary.
                EnsureValidConnection(settings);

                if (!mapDataLoggerState.ContainsKey(settings.Name))
                    mapDataLoggerState[settings.Name] = new DataLoggerState(settings, defaultConfiguration.projectRootFolder, defaultConfiguration.redundancyServerId);
                dataLoggerState = mapDataLoggerState[settings.Name];
                
                if (dataLoggerState != null && dataLoggerState.ValidFlag)
                {
                    if (startingWorkItemsGroup == null)
                    {
                        WIGStartInfo wigStartInfo = new WIGStartInfo() { WorkItemPriority = WorkItemPriority.BelowNormal };
                        startingWorkItemsGroup = smartThreadPool.CreateWorkItemsGroup(SysInfo.GetNumberOfLogicalProcessors(), wigStartInfo);
                    }

                    startingWorkItemsGroup.QueueWorkItem(() =>
                    {
                        CheckAndCreateDataLogger(dataLoggerState);
                        CheckConnectionPoint(dataLoggerState);
                    });
                }
            }

            return dataLoggerState.ValidFlag;
        }

#if !NET_STANDARD
        #region Redundancy
        /// <summary>
        /// Call for synchronizing data with the server.
        /// </summary>
        /// <param name="dataloggerName">
        /// The datalogger name for which need to synchronize data.
        /// </param>
        /// <param name="sourceConn">
        /// The source data reader model where read data.
        /// </param>
        /// <param name="endTime">
        /// The optional DateTime until read data from source (set to DateTime.MinValue if no end time required).
        /// </param>
        public void SynchronizeHistoryData(String dataloggerName, DataReader.DataReaderModel sourceConn, DateTime startTime, DateTime endTime)
        {
            if (RedundancySyncData != null)
            {
                DataLoggerState dataloggerState = null;
                lock (lockObject)
                {
                    if (mapDataLoggerState.ContainsKey(dataloggerName))
                        dataloggerState = mapDataLoggerState[dataloggerName];
                }

                if (dataloggerState != null)
                {
                    var source = new DataLoggerModel.Helpers.DataLoggerInfo()
                    {
                        DataProvider = sourceConn.DataProvider,
                        Connection = sourceConn.Connection,
                        TableName = dataloggerState.TableName,
                        UtcTimeColumnName = dataloggerState.UtcTimeColumnName,
                        MillisecondsColumnName = dataloggerState.MillisecondsColumnName,
                        RedundancyColumnName = dataloggerState.RedundancyColumnName,
                        MaxAge = dataloggerState.DataLoggerSettings.MaxAge.Value
                    };

                    var destination = new DataLoggerModel.Helpers.DataLoggerInfo()
                    {
                        DataProvider = dataloggerState.DataProvider,
                        Connection = dataloggerState.Connection,
                        TableSchema = dataloggerState.DataLoggerDataSetSchema.Tables[0],
                        TableName = dataloggerState.TableName,
                        UtcTimeColumnName = dataloggerState.UtcTimeColumnName,
                        MillisecondsColumnName = dataloggerState.MillisecondsColumnName,
                        AutoIncrementColumnName = dataloggerState.AutoIncrementColumnName,
                        RedundancyColumnName = dataloggerState.RedundancyColumnName,
                        MaxAge = dataloggerState.DataLoggerSettings.MaxAge.Value
                    };

                    RedundancySyncData.SynchronizeHistoryData(source.ToXml(), destination.ToXml(), startTime, endTime);
                }
            }
        }

        /// <summary>
        /// Stop the current redundancy synchronization.
        /// </summary>
        public void StopSync()
        {
            lock (lockObject)
            {
                if (RedundancySyncData != null)
                    RedundancySyncData.StopSync();
            }
        }

        /// <summary>
        /// Put in run state the data logger manager.
        /// </summary>
        public void Resume()
        {
            StopSync();

            StopCheckDeleteOldDataTimer();

            if (RedundancySyncData != null)
                RedundancySyncData.IsActiveServer = true;

            lock (lockObject)
            {
                foreach (var entity in mapDataLoggerToEntity.Values)
                {
                    CheckConnectionPoint(entity);
                    InitRecodingTimer(entity);
                }
            }
        }

        /// <summary>
        /// Put in pause state the data logger manager.
        /// </summary>
        public void Suspend()
        {
            StopSync();

            StartCheckDeleteOldDataTimer(TimeSpan.FromSeconds(60));

            if (RedundancySyncData != null)
                RedundancySyncData.IsActiveServer = false;

            ForceFlushOnDataChangeValues();

            lock (lockObject)
            {
                foreach (var timer in mapDataLoggerToMaxTimer.Values)
                    timer.Dispose();
                mapDataLoggerToMaxTimer.Clear();
            }
        }

        void StartCheckDeleteOldDataTimer(TimeSpan interval)
        {
            lock (lockObject)
            {
                if (redundancyCheckDeleteOldData == null)
                {
                    redundancyCheckDeleteOldData = new Timer((o) =>
                    {
                        CheckDeleteOldData();
                    }, this, TimeSpan.Zero, interval);
                }
            }
        }

        void StopCheckDeleteOldDataTimer()
        {
            lock (lockObject)
            {
                if (redundancyCheckDeleteOldData != null)
                {
                    redundancyCheckDeleteOldData.Dispose();
                    redundancyCheckDeleteOldData = null;
                }
            }
        }

        void CheckDeleteOldData()
        {
            lock (lockObject)
            {
                foreach (var dataloggerState in mapDataLoggerState.Values)
                {
                    if (dataloggerState.ValidFlag)
                    {
                        CheckAndCreateDataLogger(dataloggerState);
                        CheckAndResetDataLogger(dataloggerState);
                    }
                }
            }
        }
        #endregion
#endif
        /// <summary>
        /// Start the data logger manager.
        /// </summary>
        public void Start()
        {
            lock (lockObject)
            {
                if (bStarted)
                    return;
                bStarted = true;

#if !NET_STANDARD
                if (RedundancySyncData != null && !RedundancySyncData.IsActiveServer)
                    return;
#endif

                foreach (var entity in mapDataLoggerToEntity.Values)
                {
                    InitRecodingTimer(entity);
                }
            }
        }

        /// <summary>
        /// Enable or disable the recording of new data.
        /// </summary>
        /// <param name="entity"></param>
        public void EnableDataLoggerRecording(String dataLoggerName, bool enable)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[dataLoggerName];
                dataLoggerState.EnableFlag = enable;
                if (!enable && mapDataLoggerToHysteresisTimer.ContainsKey(dataLoggerName))
                    mapDataLoggerToHysteresisTimer[dataLoggerName].Change(0, System.Threading.Timeout.Infinite);
            }
        }

        /// <summary>
        /// Add a new job entry in order to record it on data base later.
        /// </summary>
        /// <param name="entity"></param>
        public void AddDataLoggerEntry(DataLoggerEntity entity)
        {
            if (bDisposed || ExitMode)
                return;
            entity.errorRecording = false;
            bool bNewFlushingEntries = false;

            lock (lockObject)
            {
                DataLoggerSettings dataLoggerSettings = mapDataLoggerState[entity.dataLoggerName].DataLoggerSettings;
                bool qualityStatus = GetQualityStatus(entity, dataLoggerSettings);
                var dataLoggerState = mapDataLoggerState[entity.dataLoggerName];
                if (dataLoggerState.EnableFlag && dataLoggerState.ValidFlag && qualityStatus)
                {
                    try
                    {
                        dataLoggerState.AddNewEntry(entity);
                        if (!pendingDataLoggers.Contains(dataLoggerState))
                            pendingDataLoggers.Add(dataLoggerState);
                        if (pendingColumnNames.ContainsKey(entity.dataLoggerName))
                            pendingColumnNames.Remove(entity.dataLoggerName);
                        if(!writeErrorForQualityCheckFail)
                        {
                            WriteLogInfoMessage(Properties.Resources.QualityRestored, entity);
                            writeErrorForQualityCheckFail = true;
                        }
                        bNewFlushingEntries = true;
                    }
                    catch (Exception ex)
                    {
                        WriteLogErrorMessage(Properties.Resources.FailedToAddDataLoggerEntry, entity, ex.Message);
                        entity.errorRecording = true;
                    }
                }
                else if(!qualityStatus)
                {
                    if(entity.recordingType == DataLoggerRecordingType.OnCommand || writeErrorForQualityCheckFail)
                    {
                        WriteLogErrorMessage(Properties.Resources.QualityCheckFailed, entity);
                        entity.errorRecording = true;
                        writeErrorForQualityCheckFail = false;
                    }
                }
            }

            if (bNewFlushingEntries)
                StartFlushing();
        }

        /// <summary>
        /// Add a new job entry in order to reset all data on database later.
        /// </summary>
        /// <param name="entity"></param>
        public void ResetDataLoggerEntries(String dataLoggerName)
        {
            if (bDisposed || ExitMode)
                return;

            bool bNewFlushingEntries = false;
            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[dataLoggerName];
                if (dataLoggerState.ValidFlag)
                {
                    dataLoggerState.SetResetFlag(true);
                    if (!pendingDataLoggers.Contains(dataLoggerState))
                        pendingDataLoggers.Add(dataLoggerState);
                    bNewFlushingEntries = true;
                }
            }

            if (bNewFlushingEntries)
                StartFlushing();
        }

        /// <summary>
        /// Update the job entry in order to record it on data base later.
        /// </summary>
        /// <param name="entity">Data Logger to update.</param>
        /// <param name="nodeId">NodeId of the changed column.</param>
        public void UpdateDataLoggerEntry(DataLoggerEntity entity, DataLoggerSettings dataLoggerSettings, params string[] columnNamesChanged)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                if (!mapDataLoggerToEntity.ContainsKey(entity.dataLoggerName))
                    mapDataLoggerToEntity.Add(entity.dataLoggerName, entity);
                else
                    mapDataLoggerToEntity[entity.dataLoggerName] = entity;
#if !NET_STANDARD
                if (RedundancySyncData == null || RedundancySyncData.IsActiveServer)
#endif
                {
                    CheckRecordOnDataChange(entity, dataLoggerSettings, columnNamesChanged);
                }
            }
        }
#endregion

#region Private Methods
        void EnsureValidConnection(DataLoggerSettings settings)
        {
            if (String.IsNullOrEmpty(settings.ConnectionSettings.DataSourceName))
                settings.ConnectionSettings.DataSourceName = settings.Name;
            if (String.IsNullOrEmpty(settings.ConnectionSettings.DataProvider))
                settings.ConnectionSettings.DataProvider = defaultDataProvider;
            if (String.IsNullOrEmpty(settings.ConnectionSettings.Connection))
                settings.ConnectionSettings.Connection = defaultConnectionString;
            if (isRunningAsCFR21UserName)
                settings.ConnectionSettings.Connection = DbSchemaInfoFactory.EnsureTrustedConnectionStrings(settings.ConnectionSettings.DataProvider, settings.ConnectionSettings.Connection);
        }

        DbSchemaInfo GetDbSchemaInfo(DataLoggerState dataLoggerState)
        {
            lock (lockObject)
            {
                var key = String.Format("{0}-{1}", dataLoggerState.DataProvider, dataLoggerState.Connection);
                if (!mapDbSchemaInfo.ContainsKey(key))
                    mapDbSchemaInfo.Add(key, DbSchemaInfoFactory.CreateSchemaInfo(dataLoggerState.DataProvider, dataLoggerState.Connection));
                return mapDbSchemaInfo[key];
            }
        }

        DataWriter.DataSetWriter GetCachedDataWriter(DataLoggerState dataLoggerState)
        {
            if (Properties.Settings.Default.WaitCloseConnectionSeconds == 0)
                return new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection, dataLoggerState.DataLoggerSettings.MaxTransactionsBeforeCommit.Value);

            lock (lockObject)
            {
                var key = GetDataWriterKey(dataLoggerState);
                DataWriter.DataSetWriter dataWriter = null;
                if (!mapCacheDataWriter.ContainsKey(key))
                    dataWriter = new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection, dataLoggerState.DataLoggerSettings.MaxTransactionsBeforeCommit.Value);
                else
                    dataWriter = mapCacheDataWriter[key].Item1;

                mapCacheDataWriter[key] = new Tuple<DataWriter.DataSetWriter, DateTime>(dataWriter, DateTime.UtcNow.AddSeconds(Properties.Settings.Default.WaitCloseConnectionSeconds));
 
                StartCloseConnectionTimer();

                return mapCacheDataWriter[key].Item1;
            }
        }

        void CleanCachedDataWriter(DataLoggerState dataLoggerState)
        {
            if (Properties.Settings.Default.WaitCloseConnectionSeconds == 0)
                return;

            lock (lockObject)
            {
                var key = GetDataWriterKey(dataLoggerState);
                if (mapCacheDataWriter.ContainsKey(key))
                {
                    mapCacheDataWriter[key].Item1.Dispose();
                    mapCacheDataWriter.Remove(key);
                }

                if (mapCacheDataWriter.Count == 0 && timerCloseConnection != null)
                {
                    timerCloseConnection.Dispose();
                    timerCloseConnection = null;
                }
            }
        }

        static string GetDataWriterKey(DataLoggerState dataLoggerState)
        {
            return String.Format("{0}-{1}-{2}", dataLoggerState.DataProvider, dataLoggerState.Connection, dataLoggerState.DataLoggerSettings.MaxTransactionsBeforeCommit.Value);
        }

        void StartCloseConnectionTimer()
        {
            if (timerCloseConnection == null && !bDisposed)
            {
                timerCloseConnection = new Timer((o) =>
                {
                    var toDispose = new List<IDisposable>();
                    lock (lockObject)
                    {
                        if (timerCloseConnection != null)
                        {
                            timerCloseConnection.Dispose();
                            timerCloseConnection = null;
                        }

                        foreach (var key in mapCacheDataWriter.Keys.ToList())
                        {
                            if (DateTime.UtcNow >= mapCacheDataWriter[key].Item2)
                            {
                                toDispose.Add(mapCacheDataWriter[key].Item1);
                                mapCacheDataWriter.Remove(key);
                            }
                        }

                        if (mapCacheDataWriter.Count > 0)
                            StartCloseConnectionTimer();
                    }

                    foreach (var disposable in toDispose)
                        disposable.Dispose();

                }, null, TimeSpan.FromSeconds(Properties.Settings.Default.WaitCloseConnectionSeconds), TimeSpan.FromMilliseconds(-1));
            }
        }

        void CheckRecordOnDataChange(DataLoggerEntity entity, DataLoggerSettings dataLoggerSettings, params string[] columnNamesChanged)
        {
            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[entity.dataLoggerName];
                if (!bStarted || !dataLoggerState.EnableFlag || !dataLoggerState.DataLoggerSettings.RecordOnDataChange)
                    return;

                if (columnNamesChanged == null || columnNamesChanged.Length == 0)
                {
                    if (pendingSnapshot.ContainsKey(entity.dataLoggerName))
                        pendingSnapshot[entity.dataLoggerName] = entity.CreateSnapshot();
                    return;
                }

                DateTime utcNow = DateTime.UtcNow;
                var dueTime = dataLoggerState.DataLoggerSettings.HysteresisTimeInterval;
                var snapshot = entity.CreateSnapshot();
                var action = new System.Action(() =>
                {
                    lock (lockObject)
                    {
                        if (mapDataLoggerToHysteresisTimer.ContainsKey(snapshot.dataLoggerName))
                        {
                            mapDataLoggerToHysteresisTimer[snapshot.dataLoggerName].Dispose();
                            mapDataLoggerToHysteresisTimer.Remove(snapshot.dataLoggerName);
                        }

                        if (pendingSnapshot.ContainsKey(snapshot.dataLoggerName))
                        {
                            snapshot = pendingSnapshot[snapshot.dataLoggerName];
                            pendingSnapshot.Remove(snapshot.dataLoggerName);
                        }

                        snapshot.recordingTime = utcNow + dueTime; // DateTime.UtcNow; 
                        snapshot.recordingType = DataLoggerRecordingType.OnChange;
                        AddDataLoggerEntry(snapshot);
                    }
                });

                if (dataLoggerState.DataLoggerSettings.HysteresisTimeInterval == TimeSpan.Zero)
                {
                    action();
                }
                else
                {
                    bool bRestartHysteresis = false;
                    if (columnNamesChanged != null && columnNamesChanged.Length > 0)
                    {
                        if (pendingColumnNames.ContainsKey(entity.dataLoggerName) && 
                            pendingSnapshot.ContainsKey(entity.dataLoggerName))
                        {
                            foreach (var columnName in columnNamesChanged)
                            {
                                if (pendingColumnNames[entity.dataLoggerName].Contains(columnName))
                                {
                                    DataLoggerEntity e = pendingSnapshot[entity.dataLoggerName];
                                    pendingSnapshot.Remove(entity.dataLoggerName);
                                    e.recordingTime = DateTime.UtcNow;
                                    e.recordingType = DataLoggerRecordingType.OnChange;
                                    AddDataLoggerEntry(e);
                                    bRestartHysteresis = true;
                                    break;
                                }
                            }
                        }

                        if (!pendingColumnNames.ContainsKey(entity.dataLoggerName))
                            pendingColumnNames.Add(entity.dataLoggerName, new List<String>());
                        pendingColumnNames[entity.dataLoggerName].AddRange(columnNamesChanged);
                        pendingSnapshot[entity.dataLoggerName] = snapshot;
                    }                    

                    if (bRestartHysteresis || !mapDataLoggerToHysteresisTimer.ContainsKey(entity.dataLoggerName))
                    {
                        var t = new Timer((o) =>
                        action(),
                        snapshot,
                        dueTime,
                        TimeSpan.FromMilliseconds(-1));

                        if (mapDataLoggerToHysteresisTimer.ContainsKey(entity.dataLoggerName))
                        {
                            mapDataLoggerToHysteresisTimer[entity.dataLoggerName].Dispose();
                            mapDataLoggerToHysteresisTimer.Remove(entity.dataLoggerName);
                        }

                        mapDataLoggerToHysteresisTimer.Add(entity.dataLoggerName, t);
                    }
                }
            }
        }

        void InitRecodingTimer(DataLoggerEntity entity)
        {
            lock (lockObject)
            {
                SetMaxTimer(entity);
                if (mapDataLoggerToMaxTimer.ContainsKey(entity.dataLoggerName))
                {
                    mapLastTimeUpdated[entity.dataLoggerName] = entity.recordingTime;
                }
            }
        }

        void SetMaxTimer(DataLoggerEntity entity)
        {
            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[entity.dataLoggerName];
                if (mapDataLoggerToMaxTimer.ContainsKey(entity.dataLoggerName))
                {
                    mapDataLoggerToMaxTimer[entity.dataLoggerName].Dispose();
                    mapDataLoggerToMaxTimer.Remove(entity.dataLoggerName);
                }

                if (dataLoggerState.DataLoggerSettings.RecordingTimeInterval == TimeSpan.Zero)
                    return;

                DateTime utcNow = DateTime.UtcNow;
                var span = new TimeSpan(utcNow.Ticks % dataLoggerState.DataLoggerSettings.RecordingTimeInterval.Ticks);
                var dueTime = dataLoggerState.DataLoggerSettings.RecordingTimeInterval - span;

                var t = new Timer((o) =>
                {
                    lock (lockObject)
                    {
                        if (mapDataLoggerToEntity.ContainsKey(entity.dataLoggerName))
                        {
                            DataLoggerEntity e = mapDataLoggerToEntity[entity.dataLoggerName];
                            e.recordingTime = utcNow + dueTime; // DateTime.UtcNow; 
                            e.recordingType = DataLoggerRecordingType.OnTime;
                            e.userName = String.Empty;
                            AddRecordingTimeEntry(e, dataLoggerState.DataLoggerSettings);
                        }
                    }
                },
                entity,
                dueTime,
                TimeSpan.FromMilliseconds(-1));

                mapDataLoggerToMaxTimer[entity.dataLoggerName] = t;
            }
        }

        void AddRecordingTimeEntry(DataLoggerEntity entity, DataLoggerSettings dataLoggerSettings)
        {
            if (bDisposed || ExitMode)
                return;

            SetMaxTimer(entity);

            bool bNewFlushingEntries = false;
            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[entity.dataLoggerName];
                bool qualityStatus = GetQualityStatus(entity, dataLoggerSettings); 

                if (dataLoggerState.EnableFlag && dataLoggerState.ValidFlag && qualityStatus)
                {
                    if ((mapLastTimeUpdated[entity.dataLoggerName] + dataLoggerState.DataLoggerSettings.RecordingTimeInterval) <= entity.recordingTime)
                    {
                        try
                        {
                            dataLoggerState.AddNewEntry(entity);
                            if (!pendingDataLoggers.Contains(dataLoggerState))
                                pendingDataLoggers.Add(dataLoggerState);
                            if (pendingColumnNames.ContainsKey(entity.dataLoggerName))
                                pendingColumnNames.Remove(entity.dataLoggerName);
                            mapLastTimeUpdated[entity.dataLoggerName] = entity.recordingTime;
                            if (!writeErrorForQualityCheckFail)
                            {
                                WriteLogInfoMessage(Properties.Resources.QualityRestored, entity);
                                writeErrorForQualityCheckFail = true;
                            }
                            bNewFlushingEntries = true;
                        }
                        catch (Exception ex)
                        {
                            WriteLogErrorMessage(Properties.Resources.FailedToAddDataLoggerEntry, entity, ex.Message);
                        }
                    }
                }
                else if(!qualityStatus && writeErrorForQualityCheckFail)
                {
                    WriteLogErrorMessage(Properties.Resources.QualityCheckFailed, entity);
                    entity.errorRecording = true;
                    writeErrorForQualityCheckFail = false;
                }
            }

            if (bNewFlushingEntries)
                StartFlushing();
        }

        void StartFlushing()
        {
            lock (lockObject)
            {
                if (logThread == null)
                {
                    logEvent = new AutoResetEvent(false);
                    logThread = new Thread((o) =>
                    {
                        var list = new List<DataLoggerState>();

                        while (true)
                        {
                            if (!ExitMode)
                                logEvent.WaitOne();

                            list.Clear();
                            lock (lockObject)
                            {
                                if (ExitMode && pendingDataLoggers.Count == 0)
                                    break;

#if DEBUG
                                Debug.WriteLine(String.Format("Data Logging Thread - Pending Data Loggers={0}", pendingDataLoggers.LongCount()));
#endif

                                list.AddRange(pendingDataLoggers);
                                pendingDataLoggers.Clear();

                                logEvent.Reset();
                            }

                            var listrenew = new List<DataLoggerState>();
                            while (list.Count > 0)
                            {
                                var dataLoggerState = list[0];
                                list.RemoveAt(0);
                                
                                if (ExitMode && (dataLoggerState.ErrorFlag || !dataLoggerState.CheckedFlag))
                                {
                                    if (dataLoggerState.DataSetSize > 0)
                                        FlushDataSafely(dataLoggerState);
                                }
                                else if (ExitMode || dataLoggerState.RetryTimeElapsed)
                                {
                                    dataLoggerState.RetryTimeElapsed = false;

                                    CheckAndCreateDataLogger(dataLoggerState);
                                    CheckAndResetDataLogger(dataLoggerState);
                                    WriteDataLoggerEntries(dataLoggerState);
                                }

                                if (dataLoggerState.WriteErrorCounter >= dataLoggerState.DataLoggerSettings.MaxErrorBeforeFlush || 
                                    dataLoggerState.DataSetSize >= dataLoggerState.DataLoggerSettings.MaxCacheSize)
                                {
                                    dataLoggerState.WriteErrorCounter = 0;
                                    FlushDataSafely(dataLoggerState);

                                    OnErrorStateChanged(new ErrorStateChangedEventArgs()
                                    {
                                        DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                                        bErrorState = dataLoggerState.ErrorFlag,
                                        ErrorMessage = dataLoggerState.LastErrorMessage
                                    });
                                }
                                else if (dataLoggerState.DataSetSize > 0)
                                {
                                    // Add the data logger to the end of the list in order to process later.
                                    listrenew.Add(dataLoggerState);
                                }
                            }

                            if (listrenew.Count > 0)
                            {
                                lock (lockObject)
                                {
                                    listrenew.RemoveAll(c => pendingDataLoggers.Contains(c));
                                    pendingDataLoggers.AddRange(listrenew);
                                    logEvent.Set();
                                }
                            }
                        }
                    });
                    logThread.Start();
                }
                logEvent.Set();
            }
        }

        void CheckConnectionPoint(DataLoggerEntity entity)
        {
            lock (lockObject)
            {
                var dataLoggerState = mapDataLoggerState[entity.dataLoggerName];
                CheckConnectionPoint(dataLoggerState);
            }
        }        

        void CheckConnectionPoint(DataLoggerState dataLoggerState)
        {
            if (restoreDataManager == null)
                return;

            if (!String.IsNullOrEmpty(defaultSafeDataPath))
            {
                restoreDataManager.CheckConnectionPoint(dataLoggerState.DataLoggerSettings.ConnectionSettings.Connection,
                    defaultSafeDataPath,
                    dataLoggerState.DataLoggerSettings.Name,
                    Properties.Settings.Default.DataLoggerFlushFileExt,
                    new DataLoggerModel.Helpers.DataLoggerInfo(dataLoggerState.DataLoggerSettings));
            }
        }

        void CheckAndCreateDataLogger(DataLoggerState dataLoggerState)
        {
            if (ExitMode || dataLoggerState.CheckedFlag || dataLoggerState.CheckingFlag)
                return;

            var key = GetConnectionKey(dataLoggerState);
            lock (pendingCheckConnection)
            {
                if (pendingCheckConnection.Contains(key))
                    return;
                pendingCheckConnection.Add(key);
            }

            try
            {
                bool addAggregatedTables = false;
                using (var writer = new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection))
                {
                    dataLoggerState.CheckingFlag = true;
                    if (!writer.TryOpenConnection())
                    {
                        if (ExitMode)
                            return;

                        writer.TryCreateDataBase();
                        writer.OpenConnection();
                        addAggregatedTables = true;
                    }

                    try
                    {
                        writer.CheckTables(dataLoggerState.DataLoggerDataSetSchema, dataLoggerState.IndexColumns, dataLoggerState.DataLoggerSettings.SkipCheckColumnsType);
                    }
                    catch (DataWriter.InvalidSchemaTableException ex)
                    {
                        if (ex.FaultOperation == DataWriter.FaultOperation.ChangeSchema ||
                            ex.FaultOperation == DataWriter.FaultOperation.RebuildPrimaryKeys)
                        {
                            writer.TryDropConstraints(dataLoggerState.TableName);

                            var newTableName = string.Format("{0}.{1}", dataLoggerState.TableName, Guid.NewGuid());
                            writer.RenameTable(dataLoggerState.TableName, newTableName);

                            OnErrorStateChanged(new ErrorStateChangedEventArgs()
                            {
                                DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                                bErrorState = false,
                                ErrorMessage = String.Format(
                                    Properties.Resources.FailedToChangeDataLoggerSchema.Replace("-newline-", Environment.NewLine), 
                                    ex.Message, dataLoggerState.TableName, newTableName)
                            });

                            writer.CheckTables(dataLoggerState.DataLoggerDataSetSchema, dataLoggerState.IndexColumns, dataLoggerState.DataLoggerSettings.SkipCheckColumnsType);
                        }
                        else
                            throw;
                    }
                }

                if (dataLoggerState.DataLoggerSettings.EnableDataProtection && !applyCFR21Requirements.Contains(dataLoggerState.Connection))
                {
                    DataReader.SchemaInfo.DbSchemaInfoFactory.ApplyCFR21Requirements(dataLoggerState.DataProvider, dataLoggerState.Connection);
                    applyCFR21Requirements.Add(dataLoggerState.Connection);
                }

                if (dataLoggerState.DataLoggerSettings.UseAggregatedTables)
                {
                    string arguments = string.Format(UFUAServerInfo.Properties.Settings.Default.SQLDatabaseConfigurationArgs, /*"/O{0}" "/P{1}" ""/D"{2}" "/T{3}" "/U{4}" "/L{5}" "/C{6}" "/H{7}"*/
                    1, /* AggregatesTables */
                    addAggregatedTables ? 1 : 4, /* Add or Update */
                    String.Format("DataProvider={0};{1}", dataLoggerState.DataProvider, dataLoggerState.Connection),
                    dataLoggerState.TableName,
                    dataLoggerState.UtcTimeColumnName,
                    dataLoggerState.LocalTimeColumnName,
                    String.Join(",", dataLoggerState.ValidColumnNames),
                    dataLoggerState.MillisecondsColumnName);

                    arguments = string.Format("/S {0}", arguments);

                    string path = UFUAServerInfo.Properties.Settings.Default.SQLDatabaseConfigurationTool; /*"SQLDatabaseConfiguration.exe"*/
                    System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                    var startInfo = new System.Diagnostics.ProcessStartInfo(path, arguments)
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = new System.Diagnostics.Process())
                    {
                        List<Exception> exceptions = null;
                        process.StartInfo = startInfo;
                        process.ErrorDataReceived += (o, e) =>
                        {
                            if (!String.IsNullOrEmpty(e.Data))
                            {
                                var message = e.Data.Replace("-newline-", Environment.NewLine);
                                if (exceptions == null)
                                    exceptions = new List<Exception>();
                                exceptions.Add(new Exception(message));
                                logDataLogger.Error(message);
                            }
                        };
                        process.OutputDataReceived += (o, e) =>
                        {
                            if (!String.IsNullOrEmpty(e.Data))
                            {
                                var message = e.Data.Replace("-newline-", Environment.NewLine);
                                logDataLogger.Info(message);
                            }
                        };

                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();

                        if (exceptions != null)
                        {
                            if (exceptions.Count == 1)
                                throw exceptions[0];
                            else if (exceptions.Count > 1)
                                throw new AggregateException(exceptions);
                        }
                    }
                }

                dataLoggerState.CheckedFlag = true;
            }
            catch (Exception ex)
            {
                dataLoggerState.WriteErrorCounter++;
                dataLoggerState.SetErrorFlag(true, String.Format(Properties.Resources.FailedToVerifyDataLogger, ex.Message));
            }
            finally
            {
                dataLoggerState.CheckingFlag = false;
                lock (pendingCheckConnection)
                {
                    pendingCheckConnection.Remove(key);
                }
            }
        }

        void CheckAndResetDataLogger(DataLoggerState dataLoggerState)
        {
            if (ExitMode || !dataLoggerState.CheckedFlag || dataLoggerState.ErrorFlag)
                return;

            if (dataLoggerState.ResetFlag)
            {
                using (var writer = new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection))
                {
                    try
                    {
                        var dbSchemaInfo = GetDbSchemaInfo(dataLoggerState);
                        writer.ApplyDbSchemaInfo(dbSchemaInfo);
                        var dataTable = dataLoggerState.DataLoggerDataSetSchema.Tables[0];
                        DateTime dateTime = dataLoggerState.LastTimeReset;
                        if (dataLoggerState.DataLoggerSettings.UseAggregatedTables)
                        {
                            try
                            {
                                writer.CleanTable(Properties.Settings.Default.AggregatedTablesDeleteStoreProcedure, dataTable.TableName, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                            }
                            catch
                            {
                                writer.CleanTable(dataTable, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                            }
                        }
                        else
                            writer.CleanTable(dataTable, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                        dataLoggerState.SetResetFlag(false);
                        dataLoggerState.NeedToDeleteOldData = false;
                    }
                    catch (Exception ex)
                    {
                        if (dataLoggerState.SetErrorFlag(true))
                        {
                            OnErrorStateChanged(new ErrorStateChangedEventArgs()
                            {
                                DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                                bErrorState = true,
                                ErrorMessage = String.Format(Properties.Resources.FailedToResetDataLogger, ex.Message)
                            });
                        }
                    }
                }
            }
            else if (dataLoggerState.NeedToDeleteOldData)
            {
                using (var writer = new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection))
                {
                    try
                    {
                        var dbSchemaInfo = GetDbSchemaInfo(dataLoggerState);
                        writer.ApplyDbSchemaInfo(dbSchemaInfo);
                        var dataTable = dataLoggerState.DataLoggerDataSetSchema.Tables[0];
                        DateTime dateTime = DateTime.UtcNow - dataLoggerState.DataLoggerSettings.MaxAge.Value;
                        if (dataLoggerState.DataLoggerSettings.UseAggregatedTables)
                        {
                            try
                            {
                                writer.CleanTable(Properties.Settings.Default.AggregatedTablesDeleteStoreProcedure, dataTable.TableName, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                            }
                            catch
                            {
                                writer.CleanTable(dataTable, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                            }
                        }
                        else
                            writer.CleanTable(dataTable, dataTable.Columns[dataLoggerState.UtcTimeColumnName], dateTime);
                        dataLoggerState.NeedToDeleteOldData = false;
                    }
                    catch (Exception ex)
                    {
                        if (dataLoggerState.SetErrorFlag(true))
                        {
                            OnErrorStateChanged(new ErrorStateChangedEventArgs()
                            {
                                DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                                bErrorState = true,
                                ErrorMessage = String.Format(Properties.Resources.FailedToCleanDataLogger, ex.Message)
                            });
                        }
                    }
                }
            }
        }

        void WriteDataLoggerEntries(DataLoggerState dataLoggerState)
        {
            if (!dataLoggerState.CheckedFlag)
                return;

            DataView dataView = null;
            bool bError = false;
            String lasterror = null;
            var writer = GetCachedDataWriter(dataLoggerState);
            {
                try
                {
                    var dbSchemaInfo = GetDbSchemaInfo(dataLoggerState);
                    writer.ApplyDbSchemaInfo(dbSchemaInfo);
                    // Retreive a copy of current added rows in the data logger state instance.
                    dataView = dataLoggerState.AddedRows;
                    writer.InsertRows(dataView);
                    writer.Commit();
                }
                catch (Exception ex)
                {
                    bError = true;
                    lasterror = ex.Message;
                    writer.TryRollback();
                    CleanCachedDataWriter(dataLoggerState);
                }
                finally
                {
                    if (Properties.Settings.Default.WaitCloseConnectionSeconds == 0)
                        writer.Dispose();
                    else
                        writer.Dispose(keepConnectionOpen: true);
                }
            }

            if (bError)
            {
                if (dataView != null)
                    dataLoggerState.AddRows(dataView, bThrow: false);
                dataLoggerState.WriteErrorCounter++;
                dataLoggerState.SetErrorFlag(true, String.Format(Properties.Resources.FailedToWriteDataLoggerEntries, lasterror ?? String.Empty));
            }
            else
            {
                if (dataLoggerState.SetErrorFlag(false))
                {
                    OnErrorStateChanged(new ErrorStateChangedEventArgs()
                    {
                        DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                        bErrorState = false,
                        ErrorMessage = Properties.Resources.ResumeFromErrorState
                    });

                    CheckConnectionPoint(dataLoggerState);
                }

                if (restoreDataManager != null && !restoreDataManager.IsEmpty)
                {
                    restoreDataManager.StartRestoring(dataLoggerState.Connection);
                }
            }
        }

        void FlushDataSafely(DataLoggerState dataLoggerState)
        {
#if DEBUG
            Debug.WriteLine("Data Logging Thread - Data Logger '{0}' Flush Data Safely for max cache size (size='{1}')", dataLoggerState.DataLoggerSettings.Name, dataLoggerState.DataSetSize);
#endif
            int suffix = 0;
            String errorMessage = null;
            String filePath = String.Empty;
            DataView dataView = null;
            using (var writer = new DataWriter.DataSetWriter(dataLoggerState.DataProvider, dataLoggerState.Connection))
            {
                try
                {
                    // Retreive a copy of current added rows in the data logger state instance.
                    dataView = dataLoggerState.FullAddedRows;
                    if (!String.IsNullOrEmpty(defaultSafeDataPath))
                    {
                        filePath = GetNewFlushDataFilePath(dataLoggerState);

                        while (System.IO.File.Exists(filePath))
                            filePath = GetNewFlushDataFilePath(dataLoggerState, ++suffix);

                        if (isRunningAsCFR21UserName)
                        {
                            var str = writer.ExportDataToXmlString(dataView);
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            System.IO.File.WriteAllText(filePath, toWrite);
                        }
                        else
                            writer.ExportDataToXmlFile(dataView, filePath);
                    }
                    else
                        errorMessage = Properties.Resources.UriFlushDataSafelyIsEmpty;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            OnFlushedDataSafely(new FlushedDataSafelyEventArgs()
            {
                DataLoggerName = dataLoggerState.DataLoggerSettings.Name,
                RecordsCounter = dataView != null ? dataView.Count : 0,
                FilePath = filePath,
                ErrorMessage = errorMessage
            });

            if (errorMessage == null)
            {
                if (!String.IsNullOrEmpty(defaultSafeDataPath))
                {
                    var searchPattern = String.Format("*{0}", Properties.Settings.Default.DataLoggerFlushFileExt);
                    var directorySize = new DirectorySize(defaultSafeDataPath, searchPattern);
                    if (directorySize.LastBytesSize > defaultConfiguration.MaxTotalSafelyFilesSize)
                    {
                        var exceededBytesSize = directorySize.LastBytesSize - defaultConfiguration.MaxTotalSafelyFilesSize;
                        var bytesRemoved = directorySize.DischargeOldestFiles(exceededBytesSize);
                        if (bytesRemoved > 0)
                        {
                            WriteLogInfoMessage(Properties.Resources.SafelyFilesRemoved, defaultSafeDataPath);
                        }
                    }
                }

                if (!ExitMode && restoreDataManager != null)
                {
                    restoreDataManager.AddRestorePoint(filePath, dataLoggerState.Connection,
                        new DataLoggerModel.Helpers.DataLoggerInfo(dataLoggerState.DataLoggerSettings));
                }
            }
        }

        string GetNewFlushDataFilePath(DataLoggerState dataLoggerState, int suffix = -1)
        {
            return String.Format("{0}{1}_{2}{3}{4}{5}{6}{7}{8}",
                defaultSafeDataPath,
                dataLoggerState.DataLoggerSettings.Name,
                DateTime.UtcNow.Year.ToString("D4"),
                DateTime.UtcNow.Month.ToString("D2"),
                DateTime.UtcNow.Day.ToString("D2"),
                DateTime.UtcNow.Hour.ToString("D2"),
                DateTime.UtcNow.Minute.ToString("D2"),
                suffix != -1 ? String.Format("_{0}", suffix) : String.Empty,
                Properties.Settings.Default.DataLoggerFlushFileExt);
        }

        static string GetConnectionKey(DataLoggerState dataLoggerState)
        {
            return String.Format("{0}.{1}.{2}", dataLoggerState.DataProvider.ToLower(), 
                XpoHelpers.XpoHelper.GetDataSourceServer(dataLoggerState.Connection, false).ToLower(), dataLoggerState.TableName);
        }

        static string WriteLogErrorMessage(String format, params object[] args)
        {
            var logMessage = String.Format("{0} - {1}", DateTime.Now, String.Format(format, args));
            Console.WriteLine(logMessage);

            logDataLogger.ErrorFormat(format, args);

            return logMessage;
        }

        static string WriteLogInfoMessage(String format, params object[] args)
        {
            var logMessage = String.Format("{0} - {1}", DateTime.Now, String.Format(format, args));
            Console.WriteLine(logMessage);

            logDataLogger.InfoFormat(format, args);

            return logMessage;
        }

        void ForceFlushOnDataChangeValues()
        {
            var waitEvents = new List<WaitHandle>();
            lock (lockObject)
            {
                foreach (var timer in mapDataLoggerToHysteresisTimer.Values)
                {
                    var notify = new ManualResetEvent(false);
                    timer.Dispose(notify);
                    waitEvents.Add(notify);
                }
            }

            if (waitEvents.Count > 0)
            {
                WaitHandle.WaitAll(waitEvents.ToArray());
                waitEvents.ForEach((notify) => notify.Dispose());
            }

            var actions = new List<System.Action>();
            lock (lockObject)
            {
                pendingColumnNames.Clear();
                pendingSnapshot.Clear();
            }

            foreach (var action in actions)
                action();
        }

        void ExitPendingThreads()
        {
            ExitMode = true;

            IWorkItemsGroup workItem = null;
            lock (lockObject)
            {
                if (logEvent != null)
                {
                    logEvent.Set();
                }
#if !NET_STANDARD
                StopCheckDeleteOldDataTimer();
#endif

                if (startingWorkItemsGroup != null)
                    workItem = startingWorkItemsGroup;
            }

            if (workItem != null)
                workItem.Cancel(true);

#if !NET_STANDARD
            StopSync();
#endif

            if (logThread != null)
                logThread.Join();

            smartThreadPool.WaitForIdle();
#if !NET_STANDARD
            smartThreadPool.Shutdown();
#endif
        }

        /// <summary>
        /// Return a boolean which represent the quality status related to the quality enum
        /// set on the datalogger property
        /// </summary>
        /// <param name="entity">DataLogger entity</param>
        /// <param name="dataLoggerSettings">DataLogger settings</param>
        /// <returns>True if the check is ok, false otherwise</returns>
        bool GetQualityStatus(DataLoggerEntity entity, DataLoggerSettings dataLoggerSettings)
        {
            if (dataLoggerSettings.QualityCheck == QualityCheckType.Any)
                return true;
            else
            {
                foreach (var dl in entity.listDataColumnEntities)
                {
                    if (dl.columnValue.IsQualityGood())
                    {
                        if (dataLoggerSettings.QualityCheck == QualityCheckType.AtLeastOneGood)
                            return true;
                    }
                    else
                    {
                        if (dataLoggerSettings.QualityCheck == QualityCheckType.AllGood)
                            return false;
                    }
                }
                if (dataLoggerSettings.QualityCheck == QualityCheckType.AllGood)
                    return true;
            }
            return false;
        }
#endregion

#region IDisposable Members
        public void Dispose()
        {
            if (bDisposed)
                return;

            ForceFlushOnDataChangeValues();

            bDisposed = true;
            ExitPendingThreads();

            lock (lockObject)
            {
                if (logEvent != null)
                    logEvent.Dispose();
                logEvent = null;

                smartThreadPool.Dispose();

                foreach (var key in mapDataLoggerState.Keys)
                    mapDataLoggerState[key].Dispose();
                mapDataLoggerState.Clear();

                foreach (var timer in mapDataLoggerToMaxTimer.Values)
                    timer.Dispose();
                mapDataLoggerToMaxTimer.Clear();
                mapDataLoggerToEntity.Clear();

                if (restoreDataManager != null)
                    restoreDataManager.Dispose();

                if (timerCloseConnection != null)
                    timerCloseConnection.Dispose();

                foreach (var keyValue in mapCacheDataWriter.Values)
                    keyValue.Item1.Dispose();
                mapCacheDataWriter.Clear();

                foreach (var dbSchemaInfo in mapDbSchemaInfo.Values)
                    dbSchemaInfo.Dispose();
                mapDbSchemaInfo.Clear();
            }
        }
#endregion
    }
}
