using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DevExpress.Xpo;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Diagnostics;
using XpoHelpers;
using log4net;
using Amib.Threading;
using Utilities;
using Utilities.Logger;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using Utilities.DirectorySizeHelper;

namespace UFUAHistorian
{
    public class UFUAEventLogger : IDisposable
    {
        #region Declarations

        readonly Object lockObject = new Object();

        #region Recording
        readonly List<UFUAEventLogEntity> pendingEntries = new List<UFUAEventLogEntity>();
        readonly List<UFUAEventLogEntity> flushingEntries = new List<UFUAEventLogEntity>();
        readonly Dictionary<String, IDataLayer> mapDataLayers = new Dictionary<String, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapUoWs = new Dictionary<IDataLayer, UnitOfWork>();
        #endregion

        #region Errors
        readonly List<UFUAEventLogEntity> failsEntries = new List<UFUAEventLogEntity>();
        Timer errorTimer;
        long errorCounter;
        bool isInError;
        String lasterror = null;
        #endregion

        #region Deleting
        readonly List<UFUAEventLogEntity> deletingEntries = new List<UFUAEventLogEntity>();
        readonly List<String> deletingPending = new List<String>();
        readonly List<String> deletingRunning = new List<String>();
        DateTime NextDateTimeForDeleting = DateTime.MinValue;
        #endregion

        #region Schema
        readonly List<String> schemaUpdated = new List<String>();
        readonly List<String> schemaUpdating = new List<String>();
        #endregion

        Thread logThread;
        AutoResetEvent logEvent;
        bool ExitMode;
        volatile bool FlushMode;
        volatile bool DischargeMode;
        readonly Semaphore lockHistory;
        readonly UFUAHistorianConfiguration defaultConfig;
        readonly SmartThreadPool smartThreadPool;
        readonly RestoreDataManager.RestoreDataHelper restoreDataManager;
#if !NET_STANDARD
        RedundancyHistory.IRedundancyHistory RedundancySyncData;
        Timer redundancyCheckDeleteOldData;
#endif
        bool isRunningAsCFR21UserName;
        bool bDisposed;

        LoggerDataInfo LoggerInfoOld;
        LoggerDataInfo LoggerInfoNew;

        #endregion

        #region Events

        #region OnRecyclingEvent
        public event EventHandler<RecyclingDataArgs> RecyclingEvent;

        void OnRecyclingEvent(RecyclingDataArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = RecyclingEvent;
            if (e != null)
                e(this, ea);
        }
        #endregion

        #region OnFlushedEvent
        public event EventHandler<FlushedDataArgs> FlushedEvent;

        void OnFlushedEvent(FlushedDataArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = FlushedEvent;
            if (e != null)
                e(this, ea);
        }
        #endregion

        #region OnFlushedDataSafely
        public event EventHandler<FlushedDataSafelyArgs> FlushedDataSafely;

        void OnFlushedDataSafely(FlushedDataSafelyArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = FlushedDataSafely;
            if (e != null)
                e(this, ea);
        }
        #endregion

        #region OnErrorFlushingData
        public event EventHandler<ErrorFlushingDataArgs> ErrorFlushingData;

        void OnErrorFlushingData(ErrorFlushingDataArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = ErrorFlushingData;
            if (e != null)
                e(this, ea);
        }
        #endregion

        #region OnStatisticDataChanged
        public event EventHandler<StatisticDataArgs> StatisticDataChanged;

        void OnStatisticDataChanged(LoggerDataInfo oldData, LoggerDataInfo newData)
        {
            if (bDisposed || ExitMode)
                return;

            var e = StatisticDataChanged;
            if (e != null)
                e(this, new StatisticDataArgs() { oldData = oldData, newData = newData });
        }
        #endregion

        #endregion

        #region Constructors

        public UFUAEventLogger(UFUAHistorianConfiguration config)
        {
            defaultConfig = config;
            lockHistory = new Semaphore(defaultConfig.MaxConcurrentAccess, defaultConfig.MaxConcurrentAccess);
            
            // Create smart thread pool for handle deleting and flushing
            //int currentMaxWkThreads;
            //int currentMaxCompletionPortThreads;
            //ThreadPool.GetMaxThreads(out currentMaxWkThreads, out currentMaxCompletionPortThreads);
            var startupInfo = new STPStartInfo()
            {
                ThreadPoolName = "EventLoggerThreadPool",
                ThreadPriority = System.Threading.ThreadPriority.BelowNormal,
                MaxWorkerThreads = SysInfo.GetNumberOfLogicalProcessors() * 4,
                AreThreadsBackground = false
            };
            smartThreadPool = new SmartThreadPool(startupInfo);

            if (config.MaxDeleteProcess < 0)
                config.MaxDeleteProcess = SysInfo.GetNumberOfLogicalProcessors();
            if (config.MaxRestoreProcess < 0)
                config.MaxRestoreProcess = SysInfo.GetNumberOfLogicalProcessors();

            if (config.MaxRestoreProcess > 0)
            {
                // Create a new instance of RestoreDataManager.
                restoreDataManager = new RestoreDataManager.RestoreDataHelper(RestoreDataManager.DBSchemaType.EventLogger, Logger.GetDestinationLog(LoggerDestination.Historian));
                restoreDataManager.MaxRestoreProcess = config.MaxRestoreProcess;
                restoreDataManager.IsRedundancyServer = config.RedundancyServerId >= 0;

                CheckConnectionPoint();
            }

            try
            {
                isRunningAsCFR21UserName = CurrentUser.IsEqualTo(UFUAServerInfo.UFUAServerInfo.GetCFR21UserName(), UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName());
            }
            catch
            { }

#if !NET_STANDARD
            // Create a new instance of IRedundancySyncData
            if (config.RedundancyServerId >= 0)
            {
                RedundancySyncData = new UFUAHistorian.Redundancy.EventSyncronization(config.RedundancyMaxSyncEntities, 
                    config.RedundancyHistoryThreadPool,
                    Logger.GetDestinationLog(LoggerDestination.Historian));
            }
#endif

            smartThreadPool.QueueWorkItem(() =>
            {
                var result = UpdateDatabaseSchema(defaultConfig.DefaultSettings);
                // Start a complete restoring of all connection points.
                if (result && restoreDataManager != null && !restoreDataManager.IsEmpty)
                    restoreDataManager.StartRestoring();
            });
        }

        #endregion

        #region Public Properties



        #endregion

        #region Public Methods

#if !NET_STANDARD
        #region Redundancy
        public void SynchronizeHistoryData(String sourceConn, String destinationConn, DateTime startTime, DateTime endTime)
        {
            if (RedundancySyncData != null)
                RedundancySyncData.SynchronizeHistoryData(sourceConn, destinationConn, startTime, endTime);
        }

        public void StopSync()
        {
            lock (lockObject)
            {
                if (RedundancySyncData != null)
                    RedundancySyncData.StopSync();
            }
        }

        public void Resume()
        {
            StopSync();

            StopCheckDeleteOldDataTimer();

            if (RedundancySyncData != null)
                RedundancySyncData.IsActiveServer = true;

            CheckConnectionPoint();
            // Start a complete restoring of all connection points.
            if (restoreDataManager != null && !restoreDataManager.IsEmpty)
                restoreDataManager.StartRestoring();
        }

        public void Suspend()
        {
            StopSync();

            StartCheckDeleteOldDataTimer(TimeSpan.FromSeconds(60));

            if (RedundancySyncData != null)
                RedundancySyncData.IsActiveServer = false;
         }

        void StartCheckDeleteOldDataTimer(TimeSpan interval)
        {
            if (defaultConfig.MaxDeleteProcess == 0)
                return;

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
                var entity = new UFUAEventLogEntity()
                {
                    EventConnection = defaultConfig.DefaultSettings,
                    MaxAge = defaultConfig.defaultMaxAge
                };

                if (NeedToDeleteOldData(entity) && !deletingRunning.Contains(entity.SourceNode))
                {
                    if (!deletingPending.Contains(entity.SourceNode))
                    {
                        deletingEntries.Add(entity);
                        deletingPending.Add(entity.SourceNode);
                    }
                    StartDeleting();
                } 
            }
        }
        #endregion
#endif
        public void AddLogEntity(UFUAEventLogEntity entity)
        {
            if (bDisposed || ExitMode)
                return;

            bool bNewPendingEntries = false;
            bool bNewFlushingEntries = false;
            bool bNewDeletingEntries = false;
            lock (lockObject)
            {
                if ((FlushMode && pendingEntries.Count < defaultConfig.MinPendingEntities) ||
                        (!FlushMode && pendingEntries.Count < defaultConfig.MaxPendingEntities))
                {
                    if (FlushMode)
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, 
                            Properties.Resources.MaxPendingEntitiesStopped, 
                            EventLogEntryType.Error, 
                            LoggerDestination.Historian, 
                            defaultConfig.MinPendingEntities);
                    }
                    FlushMode = false;
                    pendingEntries.Add(entity);
                    bNewPendingEntries = true;
                    DischargeMode = false;

                    if (!ExitMode && NeedToDeleteOldData(entity) && !deletingRunning.Contains(entity.SourceNode))
                    {
                        if (!deletingPending.Contains(entity.SourceNode))
                        {
                            deletingEntries.Add(entity);
                            deletingPending.Add(entity.SourceNode);
                        }
                        bNewDeletingEntries = true;
                    }
                }
                else if (flushingEntries.Count < defaultConfig.MaxPendingEntities)
                {
                    if (!FlushMode)
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            Properties.Resources.MaxPendingEntitiesEntered,
                            EventLogEntryType.Error,
                            LoggerDestination.Historian,
                            defaultConfig.MaxPendingEntities);
                    }
                    FlushMode = true;
                    flushingEntries.Add(entity);
                    bNewFlushingEntries = true;
                    DischargeMode = false;
                }
                else if (!DischargeMode)
                {
                    DischargeMode = true;
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        Properties.Resources.FlushDataSafelyCacheExceeded,
                        EventLogEntryType.Error,
                        LoggerDestination.Historian,
                        defaultConfig.MaxPendingEntities);
                }

                UpdateLoggerInfo();
            }

            if (bNewPendingEntries)
                StartRecording();
            if (bNewFlushingEntries)
                StartFlushing();
            if (bNewDeletingEntries)
                StartDeleting();
        }

        #endregion

        bool AddFailsEntries(IList<UFUAEventLogEntity> entries, bool timer = false, bool checkfails = false)
        {
            bool bNewPendingEntries = false;
            bool bNewFlushingEntries = false;
            bool bNewFailsEntries = false;
            bool bEnterInError = false;
            lock (lockObject)
            {
                if (ExitMode)
                {
                    flushingEntries.AddRange(entries);
                    bNewFlushingEntries = true;
                }
                else
                {
                    if (!timer && failsEntries.Count < defaultConfig.MaxPendingEntities)
                    {
                        failsEntries.AddRange(entries);
                        bNewFailsEntries = true;
                    }

                    if (timer || defaultConfig.ErrorTimeInterval == TimeSpan.Zero)
                    {
                        if (errorTimer != null)
                        {
                            errorTimer.Dispose();
                            errorTimer = null;
                        }

                        pendingEntries.AddRange(failsEntries);
                        bNewPendingEntries = true;
                        failsEntries.Clear();
                        bEnterInError = !timer;
                    }
                    else if (errorTimer == null)
                    {
                        errorTimer = new Timer((o) => { AddFailsEntries(entries, true); },
                            entries[0],
                            defaultConfig.ErrorTimeInterval,
                            TimeSpan.FromMilliseconds(-1));
                        bEnterInError = true;
                    }
                }

                UpdateLoggerInfo();
            }

            if (bNewPendingEntries)
                StartRecording();
            if (bNewFlushingEntries)
                StartFlushing();
            if (!checkfails && bNewFailsEntries)
                StartCheckFails();

            return bEnterInError;
        }

        void CheckConnectionPoint()
        {
            if (restoreDataManager == null)
                return;

            var conn = XpoHelper.GetConnectionString(defaultConfig.SafelySettings, UFUAServerInfo.Properties.Settings.Default.EventFlushFolderName, 
                Properties.Settings.Default.EventFlushBaseName, Properties.Settings.Default.EventFlushFileExt);
            var file = XpoHelper.GetDataSourceFilePath(conn);
            if (file != null)
            {
                var path = System.IO.Path.GetDirectoryName(file);
                restoreDataManager.CheckConnectionPoint(defaultConfig.DefaultSettings, path,
                    Properties.Settings.Default.EventFlushBaseName, Properties.Settings.Default.EventFlushFileExt);
            }
            else
            {
                using (var idlSafely = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var ufwSafely = new UnitOfWork(idlSafely))
                    {
                        var list = (from entry in new XPQuery<UFUAAuditLogItem>(ufwSafely)/*.AsParallel()*/
                                    select entry).Take(1).ToList();
                        if (list.Count > 0)
                        {
                            restoreDataManager.AddRestorePoint(conn, defaultConfig.DefaultSettings);
                        }
                    }
                }
            }
        }

        void ExitPendingThreads()
        {
            ExitMode = true;

            // thread for flushing records data
            Thread logthread = logThread;
            lock (lockObject)
            {
                if (logEvent != null)
                {
                    logEvent.Set();
                }

#if !NET_STANDARD
                StopCheckDeleteOldDataTimer();
#endif
            }

#if !NET_STANDARD
            StopSync();
#endif

            DisposeErrorTimers();

            if (logthread != null)
                logthread.Join();

            smartThreadPool.WaitForIdle();
            smartThreadPool.Shutdown();
        }

        void DisposeErrorTimers()
        {
            Debug.Assert(ExitMode, "Exit mode state not entered");

            WaitHandle ev = null;
            lock (lockObject)
            {
                if (errorTimer != null)
                {
                    ev = new AutoResetEvent(false);
                    errorTimer.Change(0, System.Threading.Timeout.Infinite);
                    errorTimer.Dispose(ev);
                }
            }

            if (ev != null)
            {
                ev.WaitOne();
                ev.Dispose();
            }
        }

        private void SetEventLogEntityData(UFUAAuditLogItem item, UFUAEventLogEntity entity)
        {
            item.EventId = entity.EventId;
            item.EventType = entity.EventType.ToString();
            item.SourceNode = entity.SourceNode;
            item.SourceName = entity.SourceName;
            item.EventDateTime = entity.EventDateTime.ToLocalTime();
            item.EventDateTimeUtc = entity.EventDateTime;
            item.EventMessage = entity.EventMessage;
            item.EventDetails = entity.EventDetails;
            item.EventComment = entity.EventComment;
            item.EventState = entity.EventState;
            item.EventDuration = entity.DurationTime;
            //item.EventUniqueId = entity.EventUniqueId;
            item.EventOccurence = entity.EventOccurence;
            item.EventSequence = entity.EventSequence;
            item.Severity = entity.Severity;
            item.UserName = entity.UserName;


            if (defaultConfig.RedundancyServerId >= 0)
                item.RedundancySyncTime = DateTime.UtcNow;
        }

        void UpdateLoggerInfo()
        {
            // Take care we use Count insted of LongCount() becase it causes a cpu overload due to Linq
            LoggerInfoNew.RecordEntriesPending = pendingEntries.Count;
            LoggerInfoNew.FailsEntriesPending = failsEntries.Count;
            LoggerInfoNew.DeleteEntriesPending = deletingPending.Count;
            LoggerInfoNew.DeleteEntriesRunning = deletingRunning.Count;
            LoggerInfoNew.FlushEntriesPending = flushingEntries.Count;
            LoggerInfoNew.DischargingEntriesMode = DischargeMode;

            OnStatisticDataChanged(LoggerInfoOld, LoggerInfoNew);

            LoggerInfoOld = LoggerInfoNew;
        }

        void StartRecording()
        {
            lock (lockObject)
            {
                if (logThread == null)
                {
                    logEvent = new AutoResetEvent(false);
                    logThread = new Thread((o) =>
                    {
#if DEBUG
                        var watcher = new Stopwatch();
#endif
                        var applyCFR21Requirements = new List<String>();
                        var list = new List<UFUAEventLogEntity>();

                        while (true)
                        {
                            if (!ExitMode)
                                logEvent.WaitOne();

                            list.Clear();
                            lock (lockObject)
                            {
                                if (ExitMode)
                                {
                                    flushingEntries.AddRange(failsEntries);
                                    failsEntries.Clear();
                                    StartFlushing();
                                    
                                    if (pendingEntries.Count == 0)
                                        break;
                                }
#if DEBUG
                                Debug.WriteLine(String.Format("Event Logger - Pending Entries={0}", pendingEntries.LongCount()));
#endif

                                list.AddRange(pendingEntries);
                                pendingEntries.Clear();
                                LoggerInfoNew.RecordEntriesRunning = list.Count;
                                UpdateLoggerInfo();
                            }

                            while (list.Count > 0)
                            {
                                var aggregatedEntities = (from c in list/*.AsParallel()*/
                                                            where c.EventConnection == list[0].EventConnection
                                                            select c).ToList();
                                if (aggregatedEntities.Count > 0)
                                {
                                    bool bError = false;

                                    var idl = GetDataLayer(aggregatedEntities[0]);
                                    var ufw = GetUnitOfWork(idl);

                                    if (ufw != null)
                                    {
                                        ufw.BeginTransaction();

                                        try
                                        {
                                            if (defaultConfig.EnableEventDataProtection && !applyCFR21Requirements.Contains(aggregatedEntities[0].EventConnection))
                                            {
                                                HistorianHelper.ApplyCFR21Requirements(aggregatedEntities[0].EventConnection, ufw);
                                                applyCFR21Requirements.Add(aggregatedEntities[0].EventConnection);
                                            }

                                            aggregatedEntities.ForEach(entity =>
                                            {
                                                UFUAAuditLogItem item = new UFUAAuditLogItem(ufw);
                                                SetEventLogEntityData(item, entity);
                                            });

                                            ufw.CommitTransaction();

                                            OnFlushedEvent(new FlushedDataArgs()
                                            {
                                                connection = aggregatedEntities[0].EventConnection,
                                                count = aggregatedEntities.Count
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            bError = true;
                                            lasterror = ex.Message;
                                            ufw.RollbackTransaction();
                                        }

                                        ufw.DropIdentityMap();
                                    }
                                    else
                                        bError = true;

                                    if (bError)
                                    {
                                        isInError = true;
                                        bool bEnterInError = AddFailsEntries(aggregatedEntities);

                                        if (bEnterInError && Interlocked.Increment(ref errorCounter) != 1 && ufw != null)
                                        {
                                            HistorianHelper.TestConnection<UFUAAuditLogItem>(ufw);
                                        }

                                        list.RemoveAll(c => c.EventConnection == aggregatedEntities[0].EventConnection);
                                        continue;
                                    }

                                    if (restoreDataManager != null && !restoreDataManager.IsEmpty)
                                    {
                                        restoreDataManager.StartRestoring(aggregatedEntities[0].EventConnection);
                                    }

                                    Interlocked.Exchange(ref errorCounter, 0);
                                    isInError = false;
                                }

                                string match = list[0].EventConnection;
                                list.RemoveAll(c => c.EventConnection == match);
                                LoggerInfoNew.RecordEntriesRunning = list.Count;
                            }
                        }
                    });
                    logThread.Start();
                }
                logEvent.Set();
            }
        }

        void StartDeleting()
        {
            lock (lockObject)
            {
                if (ExitMode || deletingEntries.Count == 0 || deletingRunning.Count >= defaultConfig.MaxDeleteProcess)
                    return;

#if DEBUG
                Debug.WriteLine(String.Format("Event Logger - Deleting Entries={0}", deletingEntries.LongCount()));
#endif
                var ufuaEventLogEntity = deletingEntries[0];
                deletingEntries.RemoveAt(0);
                if (!deletingRunning.Contains(ufuaEventLogEntity.SourceNode))
                    deletingRunning.Add(ufuaEventLogEntity.SourceNode);
                deletingPending.Remove(ufuaEventLogEntity.SourceNode);
                UpdateLoggerInfo();

                if (!StartDeleteProcess(ufuaEventLogEntity))
                    deletingRunning.Remove(ufuaEventLogEntity.SourceNode);
            }
        }

        IWorkItemsGroup flushingWorkItemsGroup;
        void StartFlushing()
        {
            lock (lockObject)
            {
                if (flushingEntries.Count == 0 || 
                    (flushingWorkItemsGroup != null && 
                    flushingWorkItemsGroup.WaitingCallbacks >= flushingWorkItemsGroup.Concurrency))
                    return;

                if (flushingWorkItemsGroup == null)
                {
                    WIGStartInfo wigStartInfo = new WIGStartInfo() { WorkItemPriority = ExitMode ? WorkItemPriority.AboveNormal : WorkItemPriority.Normal };
                    flushingWorkItemsGroup = smartThreadPool.CreateWorkItemsGroup(1, wigStartInfo);
                    flushingWorkItemsGroup.OnIdle += FlushingWorkItemsGroup_OnIdle;
                }
                flushingWorkItemsGroup.QueueWorkItem(() =>
                {
                    var flushing = new List<UFUAEventLogEntity>();

                    lock (lockObject)
                    {
                        if (!isInError)
                        {
                            pendingEntries.AddRange(flushingEntries);
                            flushingEntries.Clear();
                            StartRecording();
                            UpdateLoggerInfo();
                            return;
                        }
#if DEBUG
                        Debug.WriteLine(String.Format("Event Logger - Flushing Entries={0}", flushingEntries.LongCount()));
#endif
                        flushing.AddRange(flushingEntries);
                        flushingEntries.Clear();
                        LoggerInfoNew.FlushEntriesRunning += flushing.Count;
                        UpdateLoggerInfo();
                    }

                    // cycle for handle the flushing data
                    while (flushing.Count > 0)
                    {
                        // Agregate data using the same same SourceNode
                        var aggregatedEntities = (from c in flushing/*.AsParallel()*/
                                                    where c.SourceNode == flushing[0].SourceNode
                                                    select c).ToList();

                        if (aggregatedEntities.Count > 0)
                        {
                            FlushDataSafely(aggregatedEntities);
                        }

                        string match = flushing[0].SourceNode;
                        var totalRemoved = flushing.RemoveAll(c => c.SourceNode == match);
                        lock (lockObject)
                        {
                            LoggerInfoNew.FlushEntriesRunning -= totalRemoved;
                            UpdateLoggerInfo();
                        }
                    }

                    var safelyPath = System.IO.Path.GetDirectoryName(XpoHelper.GetDataSourceFilePath(defaultConfig.SafelySettings));
                    if (safelyPath != null)
                    {
                        safelyPath = System.IO.Path.Combine(safelyPath, UFUAServerInfo.Properties.Settings.Default.EventFlushFolderName);
                        var searchPattern = String.Format("*{0}", Properties.Settings.Default.EventFlushFileExt);
                        var directorySize = new DirectorySize(safelyPath, searchPattern);
                        if (directorySize.LastBytesSize > defaultConfig.MaxTotalSafelyFilesSize)
                        {
                            var exceededBytesSize = directorySize.LastBytesSize - defaultConfig.MaxTotalSafelyFilesSize;
                            var bytesRemoved = directorySize.DischargeOldestFiles(exceededBytesSize);
                            if (bytesRemoved > 0)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, 
                                    Properties.Resources.SafelyFilesRemoved, 
                                    EventLogEntryType.Information, 
                                    LoggerDestination.Historian,
                                    XpoHelper.GetConnectionStringWithoutPassword(safelyPath));
                            }
                        }
                    }
                });
            }
        }

        void FlushingWorkItemsGroup_OnIdle(IWorkItemsGroup workItemsGroup)
        {
            lock (lockObject)
            {
                if (flushingEntries.Count > 0)
                    StartFlushing();
            }
        }

        IWorkItemsGroup checkingWorkItemsGroup;
        void StartCheckFails()
        {
            lock (lockObject)
            {
                if (failsEntries.Count == 0 || 
                    (checkingWorkItemsGroup != null && 
                    checkingWorkItemsGroup.WaitingCallbacks >= checkingWorkItemsGroup.Concurrency))
                    return;

                if (checkingWorkItemsGroup == null)
                {
                    WIGStartInfo wigStartInfo = new WIGStartInfo() { WorkItemPriority = ExitMode ? WorkItemPriority.AboveNormal : WorkItemPriority.Normal };
                    checkingWorkItemsGroup = smartThreadPool.CreateWorkItemsGroup(1, wigStartInfo);
                    checkingWorkItemsGroup.OnIdle += CheckingWorkItemsGroup_OnIdle;
                }
                checkingWorkItemsGroup.QueueWorkItem(() =>
                {
                    var fails = new List<UFUAEventLogEntity>();

                    lock (lockObject)
                    {
#if DEBUG
                        Debug.WriteLine(String.Format("Event Logger - Fails Entries={0}", failsEntries.LongCount()));
#endif
                        fails.AddRange(failsEntries);
                        failsEntries.Clear();
                        LoggerInfoNew.FailsEntriesRunning += fails.Count;
                        UpdateLoggerInfo();
                    }

                    // cycle for handle the write error
                    while (fails.Count > 0)
                    {
                        // Agregate data using the same same SourceNode
                        var aggregatedEntities = (from c in fails/*.AsParallel()*/
                                                    where c.EventConnection == fails[0].EventConnection
                                                    select c).ToList();

                        if (aggregatedEntities.Count > 0)
                        {
                            //  Check if the max error cache was exeeded
                            if (ExitMode || aggregatedEntities.Count > defaultConfig.MaxErrorCacheSize ||
                                Interlocked.Read(ref errorCounter) >= defaultConfig.MaxErrorBeforeFlush)
                            {
                                lock (lockObject)
                                {
                                    flushingEntries.AddRange(aggregatedEntities);
                                    UpdateLoggerInfo();
                                }

                                StartFlushing();

                                OnErrorFlushingData(new ErrorFlushingDataArgs()
                                {
                                    connection = aggregatedEntities[0].EventConnection,
                                    exception = lasterror ?? String.Empty
                                });
                                lasterror = null;

                                Interlocked.Exchange(ref errorCounter, 0);
                            }
                            else
                                AddFailsEntries(aggregatedEntities, false, true);
                        }

                        string match = fails[0].EventConnection;
                        var totalRemoved = fails.RemoveAll(c => c.EventConnection == match);
                        lock (lockObject)
                        {
                            LoggerInfoNew.FailsEntriesRunning -= totalRemoved;
                            UpdateLoggerInfo();
                        }
                    }
                });
            }
        }

        void CheckingWorkItemsGroup_OnIdle(IWorkItemsGroup workItemsGroup)
        {
            lock (lockObject)
            {
                if (failsEntries.Count > defaultConfig.MaxErrorCacheSize)
                    StartCheckFails();
            }
        }

        private bool NeedToDeleteOldData(UFUAEventLogEntity uFUAEventLogEntity)
        {
            if (uFUAEventLogEntity.MaxAge == TimeSpan.Zero || defaultConfig.MaxDeleteProcess == 0)
                return false;

            bool bRet = NextDateTimeForDeleting < DateTime.UtcNow;
            if (bRet)
            {
                // randomizing the deletion of the records for avoiding too many concurrencies
                TimeSpan span;
                var rnd = new Random();
                if (uFUAEventLogEntity.MaxAge.Days > 0)
                    span = TimeSpan.FromHours(1) + TimeSpan.FromMinutes((double)rnd.Next(30));
                else if (uFUAEventLogEntity.MaxAge.Hours > 0)
                    span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                else if (uFUAEventLogEntity.MaxAge.Minutes > 0)
                    span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                else
                    span = uFUAEventLogEntity.MaxAge + TimeSpan.FromMilliseconds((double)rnd.Next(500)); ;

                NextDateTimeForDeleting = DateTime.UtcNow + span;
            }

            return bRet;
        }

        bool StartDeleteProcess(UFUAEventLogEntity uFUAEventLogEntity)
        {
            Process deleteProcess = null;
            if (IsSchemaUpdated(uFUAEventLogEntity))
            {
                try
                {
#if DEBUG
                    var watcher = new Stopwatch();
                    watcher.Start();
#endif

                    String arguments = String.Format("/C\"{0}\" /A\"{1}\" /T\"{2}\" /P\"{3}\"",
                        uFUAEventLogEntity.EventConnection.Replace("\"", ""), uFUAEventLogEntity.MaxAge, defaultConfig.MaxDeletingEntities, Process.GetCurrentProcess().Id);
                    if (defaultConfig.EnableEventDataProtection)
                        arguments = String.Format("/L {0}", arguments);
#if !NET_STANDARD
                    String path = Properties.Settings.Default.DeleteHistoryDataToolName;
#else
                    String path = Properties.Settings.Default.DeleteHistoryDataToolName.Replace(".exe", ".dll");
#endif
                    System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                    //using (var deleteProcess = new System.Diagnostics.Process())
                    {
                        deleteProcess = new System.Diagnostics.Process();
                        deleteProcess.ErrorDataReceived += (o, e) =>
                        {
                            if (!ExitMode && !String.IsNullOrEmpty(e.Data))
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    Properties.Resources.FailedDeleteEntities,
                                    EventLogEntryType.Error,
                                    LoggerDestination.Historian,
                                    XpoHelper.GetConnectionStringWithoutPassword(uFUAEventLogEntity.EventConnection), 
                                    uFUAEventLogEntity.SourceName, e.Data);
                            }
                        };

                        deleteProcess.OutputDataReceived += (o, e) =>
                        {
                            if (!String.IsNullOrEmpty(e.Data))
                            {
                                int count = 0;
                                if (int.TryParse(e.Data, out count))
                                {
                                    OnRecyclingEvent(new RecyclingDataArgs()
                                    {
                                        nodeId = uFUAEventLogEntity.SourceNode,
                                        name = uFUAEventLogEntity.SourceName,
                                        count = count
                                    });
                                }
                            }
                        };

                        deleteProcess.Exited += (o, e) =>
                        {
#if DEBUG
                            watcher.Stop();
                            Debug.WriteLine(String.Format("Event Logger - Deletion of records for Source Name {0} executed in {1}", uFUAEventLogEntity.SourceName, watcher.Elapsed));
#endif
                            ProcessExited(deleteProcess, uFUAEventLogEntity.SourceNode);
                        };

#if !NET_STANDARD
                        deleteProcess.StartInfo.FileName = path;
                        deleteProcess.StartInfo.Arguments = arguments.ToString();
#else
                        deleteProcess.StartInfo.FileName = "dotnet";
                        deleteProcess.StartInfo.Arguments = String.Format("\"{0}\" {1}", path, arguments);
                        deleteProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
#endif
                        deleteProcess.StartInfo.RedirectStandardError = true;
                        deleteProcess.StartInfo.RedirectStandardOutput = true;
                        deleteProcess.StartInfo.UseShellExecute = false;
                        deleteProcess.StartInfo.CreateNoWindow = true;
                        deleteProcess.Start();
                        deleteProcess.EnableRaisingEvents = true;
                        deleteProcess.BeginErrorReadLine();
                        deleteProcess.BeginOutputReadLine();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                Properties.Resources.ErrorStartingDeleteTool,
                                EventLogEntryType.Error,
                                LoggerDestination.Historian,
#if !NET_STANDARD
                                Properties.Settings.Default.DeleteHistoryDataToolName,
#else
                                Properties.Settings.Default.DeleteHistoryDataToolName.Replace(".exe", ".dll"),
#endif
                                uFUAEventLogEntity.SourceNode,
                                ex.Message);

                    if (deleteProcess != null)
                        ProcessExited(deleteProcess, uFUAEventLogEntity.SourceNode);
                }
            }

            return false;
        }

        void ProcessExited(Process process, String sourceNode)
        {
            lock (lockObject)
            {
                process.Dispose();
                deletingRunning.Remove(sourceNode);
                UpdateLoggerInfo();

                if (deletingPending.Count > 0)
                {
                    StartDeleting();
                    //smartThreadPool.QueueWorkItem(() => StartDeleting());
                }
            }
        }

        void FlushDataSafely(List<UFUAEventLogEntity> entries)
        {
#if DEBUG
            var watcher = new Stopwatch();
            watcher.Start();
#endif
            var fileName = String.Format("{0}_{1}{2}{3}{4}{5}",
                                        Properties.Settings.Default.EventFlushBaseName,
                                        DateTime.UtcNow.Year.ToString("D4"),
                                        DateTime.UtcNow.Month.ToString("D2"),
                                        DateTime.UtcNow.Day.ToString("D2"),
                                        DateTime.UtcNow.Hour.ToString("D2"),
                                        DateTime.UtcNow.Minute.ToString("D2"));

            int suffix = 0;
            string conn = null;
            string fileBase = null;
            do
            {
                conn = XpoHelper.GetConnectionString(defaultConfig.SafelySettings,
                    UFUAServerInfo.Properties.Settings.Default.EventFlushFolderName,
                    String.Format("{0}_{1}", fileName, suffix++), 
                    Properties.Settings.Default.EventFlushFileExt);
                fileBase = XpoHelper.GetDataSourceFilePath(conn);
            } while (fileBase != null && System.IO.File.Exists(fileBase));

            try
            {
                var inMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                using (var idlSafely = new SimpleDataLayer(inMemory))
                {
                    using (var ufwSafely = new UnitOfWork(idlSafely))
                    {
                        entries.ForEach(entity =>
                        {
                            var sleepTime = Properties.Settings.Default.FlushDataSleepTime;
                            if (sleepTime > 0 && !ExitMode && entries.Count > 0)
                                Thread.Sleep(sleepTime);

                            UFUAAuditLogItem item = new UFUAAuditLogItem(ufwSafely);
                            SetEventLogEntityData(item, entity);
                        });

                        ufwSafely.CommitChanges();

                        if (isRunningAsCFR21UserName)
                        {
                            using (var memoryStream = new System.IO.MemoryStream())
                            {
                                using (var writer = System.Xml.XmlWriter.Create(memoryStream))
                                {
                                    inMemory.WriteXml(writer);
                                    writer.Flush();
                                    writer.Close();
                                    var str = Convert.ToBase64String(memoryStream.ToArray());
                                    var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                                    System.IO.File.WriteAllText(fileBase, toWrite);
                                }
                            }
                        }
                        else
                            inMemory.WriteXml(fileBase);

                        OnFlushedDataSafely(new FlushedDataSafelyArgs()
                        {
                            Name = entries[0].SourceName,
                            Count = entries.Count,
                            Connection = conn,
                            Result = true
                        });
                    }
                }

                if (!ExitMode && restoreDataManager != null)
                    restoreDataManager.AddRestorePoint(conn, entries[0].EventConnection);
            }
            catch (Exception ex)
            {
                OnFlushedDataSafely(new FlushedDataSafelyArgs()
                {
                    Name = entries[0].SourceName,
                    Count = entries.Count,
                    Connection = conn,
                    Result = false
                });
            }
#if DEBUG
            watcher.Stop();
            Debug.WriteLine(String.Format("Event Logger - Flush Data Safely for File Name {0} executed in {1}", fileName, watcher.Elapsed));
#endif
            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                Properties.Resources.FlushDataSafely,
                EventLogEntryType.Information,
                LoggerDestination.Historian,
                conn, entries.Count);
        }

        static IDataLayer CreateDataLayer(String settings)
        {
            return HistorianHelper.CreateDataLayer<UFUAAuditLogItem>(settings);
        }

        IDataLayer GetDataLayer(UFUAEventLogEntity uFUAEventLogEntity, bool bThrow = false)
        {
            lock (mapDataLayers)
            {
                try
                {
                    if (!ExitMode && !mapDataLayers.ContainsKey(uFUAEventLogEntity.EventConnection))
                        mapDataLayers[uFUAEventLogEntity.EventConnection] = CreateDataLayer(uFUAEventLogEntity.EventConnection);

                    if (mapDataLayers.ContainsKey(uFUAEventLogEntity.EventConnection))
                        return mapDataLayers[uFUAEventLogEntity.EventConnection];
                }
                catch (Exception e)
                {
                    var msg = String.Format("{0} - {1}", DateTime.Now, String.Format(Properties.Resources.FailedCreateDataLayer, 
                        uFUAEventLogEntity.EventConnection,
                        e.InnerException != null ? e.InnerException.Message : e.Message));
                
                    Utils.Trace(Utils.TraceMasks.Error, msg);

                    if (bThrow)
                        throw;
                }

                return null;
            }
        }

        UnitOfWork GetUnitOfWork(IDataLayer dataLayer)
        {
            if (dataLayer == null)
                return null;

            lock (mapUoWs)
            {
                if (!mapUoWs.ContainsKey(dataLayer))
                    mapUoWs[dataLayer] = new UnitOfWork(dataLayer) { LockingOption = LockingOption.None };

                return mapUoWs[dataLayer];
            }
        }

        bool IsSchemaUpdated(UFUAEventLogEntity uFUAEventLogEntity)
        {
            lock (lockObject)
            {
                if (schemaUpdated.Contains(uFUAEventLogEntity.EventConnection))
                    return true;

                if (!schemaUpdating.Contains(uFUAEventLogEntity.EventConnection))
                {
                    schemaUpdating.Add(uFUAEventLogEntity.EventConnection);
                    smartThreadPool.QueueWorkItem(() =>
                    {
                        var successfull = HistorianHelper.TryUpdateSchema<UFUAAuditLogItem>(uFUAEventLogEntity.EventConnection);
                        lock (lockObject)
                        {
                            schemaUpdated.Remove(uFUAEventLogEntity.EventConnection);
                            schemaUpdating.Remove(uFUAEventLogEntity.EventConnection);
                            if (successfull)
                                schemaUpdated.Add(uFUAEventLogEntity.EventConnection);
                        }
                    });
                }
            }

            return false;
        }

        bool UpdateDatabaseSchema(String settings)
        {
            try
            {
                HistorianHelper.UpdateSchema<UFUAAuditLogItem>(settings);
                return true;
            }
            catch (Exception e)
            {
                if (!(e is UpdateSchemaTimeoutException))
                {
                    string message = String.Format(Properties.Resources.FailedUpdateDatabase, XpoHelper.GetConnectionStringWithoutPassword(settings),
                        e.InnerException != null ? e.InnerException.Message : e.Message);
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, EventLogEntryType.Error, LoggerDestination.Historian);
                    Utils.Trace(Utils.TraceMasks.Error, String.Format("{0} - {1}", DateTime.Now, message));
                }
            }

            return false;
        }

        #region IDisposable

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            ExitPendingThreads();

            lock (lockObject)
            {
                if (logEvent != null)
                {
                    logEvent.Dispose();
                }

                if (flushingWorkItemsGroup != null)
                    flushingWorkItemsGroup.OnIdle -= FlushingWorkItemsGroup_OnIdle;

                if (checkingWorkItemsGroup != null)
                    checkingWorkItemsGroup.OnIdle -= CheckingWorkItemsGroup_OnIdle;

                smartThreadPool.Dispose();

                foreach (var dl in mapUoWs.Values)
                    dl.Dispose();
                mapUoWs.Clear();

                foreach (var dl in mapDataLayers.Values)
                    dl.Dispose();
                mapDataLayers.Clear();

                if (restoreDataManager != null)
                    restoreDataManager.Dispose();
            }
        }

        #endregion

        #region Query Helpers

        public IList<UFUAAuditLogItem> ReadHistory(List<UFUAEventLogEntity> uFUAEventLogEntityList,
                                                    DateTime start, DateTime end, int maxValues = 0)
        {
            var ret = new List<UFUAAuditLogItem>();
            uFUAEventLogEntityList.ForEach((uFUAEventLogEntity) => ret.AddRange(ReadHistory(uFUAEventLogEntity, start, end, maxValues)));

            return ret;
        }

        public IList<UFUAAuditLogItem> ReadHistory(UFUAEventLogEntity uFUAEventLogEntity,
                                                    DateTime start, DateTime end, int maxValues = 0)
        {
            if (!IsSchemaUpdated(uFUAEventLogEntity))
                throw new ServiceResultException(StatusCodes.BadOutOfService);

            lockHistory.WaitOne();

            try
            {
                using (var idl = GetDataLayer(uFUAEventLogEntity, bThrow: true))
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        /////////////////////////////////////////////////////////////////////////////////
                        // By Maurizio Zaniboni - Progea Srl.
                        // Check start and end time for passing CTT unit tests : Aggregates*
                        if (start != DateTime.MinValue)
                        {
                            if (start < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                                start = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                            else if (start > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                                start = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                        }
                        if (end != DateTime.MinValue)
                        {
                            if (end < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                                end = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                            else if (end > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                                end = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                        }
                        /////////////////////////////////////////////////////////////////////////////////

                        if (maxValues > 0)
                        {
                            return (from entry in new XPQuery<UFUAAuditLogItem>(ufw)/*.AsParallel()*/
                                    where entry.SourceNode == uFUAEventLogEntity.SourceNode &&
                                    (start == DateTime.MinValue || entry.EventDateTimeUtc >= start) &&
                                    (end == DateTime.MinValue || entry.EventDateTimeUtc <= end)
                                    orderby entry.EventDateTimeUtc
                                    select entry).Take(maxValues).ToList();
                        }
                        else
                        {
                            return (from entry in new XPQuery<UFUAAuditLogItem>(ufw)/*.AsParallel()*/
                                    where entry.SourceNode == uFUAEventLogEntity.SourceNode &&
                                    (start == DateTime.MinValue || entry.EventDateTimeUtc >= start) &&
                                    (end == DateTime.MinValue || entry.EventDateTimeUtc <= end)
                                    orderby entry.EventDateTimeUtc
                                    select entry).ToList();
                        }
                    }
                }
            }
            finally
            {
                lockHistory.Release();
            }
        }

        #endregion

    }
}
