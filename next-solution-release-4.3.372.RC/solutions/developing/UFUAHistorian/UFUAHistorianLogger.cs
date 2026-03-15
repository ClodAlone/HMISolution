using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
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
    public class UFUAHistorianLogger : IDisposable
    {
        #region Declarations

        readonly Object lockObject = new Object();

        #region Recording
        readonly List<UFUAHistorianLogEntity> pendingEntries = new List<UFUAHistorianLogEntity>();
        readonly List<UFUAHistorianLogEntity> flushingEntries = new List<UFUAHistorianLogEntity>();
        readonly Dictionary<String, IDataLayer> mapDataLayers = new Dictionary<String, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapUoWs = new Dictionary<IDataLayer, UnitOfWork>();

        readonly Dictionary<String, Double> mapCheckHysteresis = new Dictionary<String, Double>();
        readonly Dictionary<String, DateTime> mapLastTimeUpdated = new Dictionary<String, DateTime>();

        readonly Dictionary<String, Timer> mapNodeToMaxTimer = new Dictionary<String, Timer>();
        readonly Dictionary<String, Timer> mapNodeToMinTimer = new Dictionary<String, Timer>();

        readonly Dictionary<String, UFUAHistorianLogEntity> mapNodeToEntity = new Dictionary<String, UFUAHistorianLogEntity>();
        #endregion

        #region Errors
        readonly List<UFUAHistorianLogEntity> failsEntries = new List<UFUAHistorianLogEntity>();
        readonly Dictionary<String, Timer> mapNodeToErrorTimer = new Dictionary<String, Timer>();
        readonly Dictionary<String, Byte> mapErrorCounter = new Dictionary<String, Byte>();
        readonly Dictionary<String, String> mapLastError = new Dictionary<String, String>();
        #endregion

        #region Deleting
        readonly List<UFUAHistorianLogEntity> deletingEntries = new List<UFUAHistorianLogEntity>();
        readonly List<String> deletingPending = new List<String>();
        readonly List<String> deletingRunning = new List<String>();
        readonly Dictionary<String, DateTime> mapCheckMaxAges = new Dictionary<String, DateTime>();
        #endregion

        #region Restoring
        readonly RestoreDataManager.RestoreDataHelper restoreDataManager;
        readonly List<String> checkedConnectionPoints = new List<String>();
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
#if !NET_STANDARD
        RedundancyHistory.IRedundancyHistory RedundancySyncData;
        Timer redundancyCheckDeleteOldData;
#endif
        bool isActiveServer;
        bool isRunningAsCFR21UserName;
        bool bDisposed;

        LoggerDataInfo LoggerInfoOld;
        LoggerDataInfo LoggerInfoNew;

        IWorkItemsGroup startingWorkItemsGroup;

        #endregion

        #region Events
        #region OnRecyclingData
        public event EventHandler<RecyclingDataArgs> RecyclingData;

        void OnRecyclingData(RecyclingDataArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = RecyclingData;
            if (e != null)
                e(this, ea);
        }
        #endregion

        #region OnFlushedData
        public event EventHandler<FlushedDataArgs> FlushedData;

        void OnFlushedData(FlushedDataArgs ea)
        {
            if (bDisposed || ExitMode)
                return;

            var e = FlushedData;
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

        public UFUAHistorianLogger(UFUAHistorianConfiguration config)
        {
            Enabled = false;
            defaultConfig = config;
            lockHistory = new Semaphore(defaultConfig.MaxConcurrentAccess, defaultConfig.MaxConcurrentAccess);

            // Create smart thread pool for handle deleting and flushing
            //int currentMaxWkThreads;
            //int currentMaxCompletionPortThreads;
            //ThreadPool.GetMaxThreads(out currentMaxWkThreads, out currentMaxCompletionPortThreads);
            var startupInfo = new STPStartInfo()
            {
                ThreadPoolName = "HistorianLoggerThreadPool",
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
                restoreDataManager = new RestoreDataManager.RestoreDataHelper(RestoreDataManager.DBSchemaType.Historian, Logger.GetDestinationLog(LoggerDestination.Historian));
                restoreDataManager.MaxRestoreProcess = config.MaxRestoreProcess;
                restoreDataManager.IsRedundancyServer = config.RedundancyServerId >= 0;
            }
            isActiveServer = config.RedundancyServerId == -1;

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
                RedundancySyncData = new UFUAHistorian.Redundancy.HistorianSyncronization(config.RedundancyMaxSyncEntities, 
                    config.RedundancyHistoryThreadPool,
                    Logger.GetDestinationLog(LoggerDestination.Historian));
            }
#endif
        }

        #endregion

        #region Public Properties

        bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled == value)
                    return;

                enabled = value;
            }
        }

        #endregion

        #region Public Methods

        public void Init(UFUAHistorianLogEntity entity)
        {
            mapNodeToEntity[entity.NodeId] = new UFUAHistorianLogEntity(entity);
            CheckAndUpdateLastTimeUpdatedValue(entity.NodeId);
            SetMaxTimer(entity.NodeId);

            lock (lockObject)
            {
                if (startingWorkItemsGroup == null)
                {
                    WIGStartInfo wigStartInfo = new WIGStartInfo() { WorkItemPriority = WorkItemPriority.BelowNormal };
                    startingWorkItemsGroup = smartThreadPool.CreateWorkItemsGroup(SysInfo.GetNumberOfLogicalProcessors(), wigStartInfo);
                }

                startingWorkItemsGroup.QueueWorkItem(() =>
                {
                    CheckConnectionPoint(entity);
                    CheckAndUpdateHistorianAuditDataLog(entity);
                    bool bNeedToCheckConnection = false;
                    lock (lockObject)
                    {
                        if (!checkedConnectionPoints.Contains(entity.HistoricalName))
                        {
                            bNeedToCheckConnection = true;
                            checkedConnectionPoints.Add(entity.HistoricalName);
                        }
                    }

                    if (bNeedToCheckConnection)
                    {
                        var result = UpdateDatabaseSchema(entity.HistoricalConnection);
                        if (result && restoreDataManager != null && !restoreDataManager.IsEmpty)
                        {
                            restoreDataManager.StartRestoring(entity.HistoricalConnection);
                        }
                    }
                });
            }
        }

        public void Suspend(String nodeId)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                if (!mapNodeToEntity.ContainsKey(nodeId) || !mapNodeToEntity[nodeId].Enabled)
                    return;
                var entity = mapNodeToEntity[nodeId];

                entity.Enabled = false;
                if (isActiveServer)
                {
                    if (mapNodeToMaxTimer.ContainsKey(entity.NodeId))
                    {
                        mapNodeToMaxTimer[entity.NodeId].Dispose();
                        mapNodeToMaxTimer.Remove(entity.NodeId);
                    }

                    if (mapNodeToMinTimer.ContainsKey(entity.NodeId))
                    {
                        mapNodeToMinTimer[entity.NodeId].Dispose();
                        mapNodeToMinTimer.Remove(entity.NodeId);
                    }
                }
            }
        }

        public void Resume(String nodeId)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                if (!mapNodeToEntity.ContainsKey(nodeId) || mapNodeToEntity[nodeId].Enabled)
                    return;
                var entity = mapNodeToEntity[nodeId];

                entity.Enabled = true;
                if (isActiveServer)
                {
                    if (!checkedConnectionPoints.Contains(entity.HistoricalName))
                    {
                        checkedConnectionPoints.Add(entity.HistoricalName);
                        CheckConnectionPoint(entity);
                    }
                    SetMaxTimer(entity.NodeId);
                }
            }
        }

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

            lock (lockObject)
            {
                isActiveServer = true;
                mapCheckHysteresis.Clear();
                checkedConnectionPoints.Clear();
                foreach (var entity in mapNodeToEntity.Values)
                {
                    if (!entity.Enabled)
                        continue;

                    if (!checkedConnectionPoints.Contains(entity.HistoricalName))
                    {
                        checkedConnectionPoints.Add(entity.HistoricalName);
                        CheckConnectionPoint(entity);
                    }
                    SetMaxTimer(entity.NodeId);
                }
            }

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

            lock (lockObject)
            {
                isActiveServer = false;
                foreach (var timer in mapNodeToMaxTimer.Values)
                    timer.Dispose();
                mapNodeToMaxTimer.Clear();

                foreach (var timer in mapNodeToMinTimer.Values)
                    timer.Dispose();
                mapNodeToMinTimer.Clear();

                foreach (var timer in mapNodeToErrorTimer.Values)
                    timer.Dispose();
                mapNodeToErrorTimer.Clear();
            }
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
                foreach (var entity in mapNodeToEntity.Values)
                {
                    if (NeedToDeleteOldData(entity) && !deletingRunning.Contains(entity.NodeId))
                    {
                        if (!deletingPending.Contains(entity.NodeId))
                        {
                            deletingEntries.Add(entity);
                            deletingPending.Add(entity.NodeId);
                        }
                        StartDeleting();
                    }
                }
            }
        }
        #endregion
#endif
        public void AddLogEntity(UFUAHistorianLogEntity entity)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                if (!mapNodeToEntity.ContainsKey(entity.NodeId))
                    return;

                mapNodeToEntity[entity.NodeId].UpdateRecordingValues(entity);
                CheckAndUpdateLastTimeUpdatedValue(entity.NodeId);

                if (isActiveServer && entity.Enabled &&
                    (entity.MinTimeInterval == TimeSpan.Zero || entity.MinTimeInterval != entity.MaxTimeInterval))
                {
                    SetMaxTimer(entity.NodeId);

                    var utcNow = DateTime.UtcNow;
                    if (entity.MinTimeInterval == TimeSpan.Zero || mapLastTimeUpdated[entity.NodeId] + entity.MinTimeInterval <= utcNow)
                    {
                        if (mapNodeToMinTimer.ContainsKey(entity.NodeId))
                        {
                            mapNodeToMinTimer[entity.NodeId].Dispose();
                            mapNodeToMinTimer.Remove(entity.NodeId);
                        }

                        AddLogEntity(entity.NodeId, utcNow);
                    }
                    else if (entity.MinTimeInterval != TimeSpan.Zero)
                    {
                        if (mapNodeToMinTimer.ContainsKey(entity.NodeId))
                        {
                            mapNodeToMinTimer[entity.NodeId].Dispose();
                            mapNodeToMinTimer.Remove(entity.NodeId);
                        }

                        var dueTime = entity.MinTimeInterval - (utcNow - mapLastTimeUpdated[entity.NodeId]);
                        var t = new Timer((o) =>
                        {
                            AddLogEntity(entity.NodeId, utcNow + dueTime);
                        },
                                    entity,
                                    dueTime,
                                    TimeSpan.FromMilliseconds(-1));
                        mapNodeToMinTimer[entity.NodeId] = t;
                    }
                }
            }
        }

        /// <summary>
        /// Reset all data on database belonging to a particular historian structure.
        /// </summary>
        /// <param name="entity"></param>
        public void ResetHistorianLoggerEntries(String historianName)
        {
            if (bDisposed || ExitMode)
                return;

            var entities = (from c in mapNodeToEntity.Values
                            where c.HistoricalName == historianName
                            select c).ToList();
            lock (lockObject)
            {
                foreach(var entity in entities)
                {
                    if(entity != null)
                    {
                        //It hasn't to check if the entity is already in the lists because it could be in them
                        //due to an automatic cancellation and in that case the MaxAge sent to the cancellation
                        //software may be configured to delete just old data and not all of them up to the time
                        //when the reset command is received
                        deletingEntries.Add(entity);
                        deletingPending.Add(entity.NodeId);

                        entity.SetResetFlag(true);
                        StartDeleting();
                    }
                            
                }
            }
        }

        #endregion

        #region Implementations

        void AddLogEntity(String nodeId, DateTime recordingTime, bool isMaxTimer = false)
        {
            if (bDisposed || ExitMode)
                return;

            bool bNewPendingEntries = false;
            bool bNewFlushingEntries = false;
            bool bNewDeletingEntries = false;
            SetMaxTimer(nodeId, recordingTime);
            lock (lockObject)
            {
                var entity = mapNodeToEntity[nodeId].Clone() as UFUAHistorianLogEntity;
                CheckAndUpdateLastTimeUpdatedValue(entity.NodeId);
                if (Enabled && isActiveServer && entity.Enabled)
                {
                    mapLastTimeUpdated[entity.NodeId] = entity.RecordDateTime = recordingTime;
                    if (isMaxTimer)
                        entity.ExceptionDeviation = 0; // doesn't check hysteresis.

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

                        if (!ExitMode && NeedToDeleteOldData(entity) && !deletingRunning.Contains(entity.NodeId))
                        {
                            if (!deletingPending.Contains(entity.NodeId))
                            {
                                deletingEntries.Add(entity);
                                deletingPending.Add(entity.NodeId);
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

        bool AddFailsEntries(IList<UFUAHistorianLogEntity> entries, bool timer = false, bool checkfails = false)
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

                    if (timer || entries[0].ErrorTimeInterval == TimeSpan.Zero)
                    {
                        if (mapNodeToErrorTimer.ContainsKey(entries[0].HistoricalConnection))
                        {
                            mapNodeToErrorTimer[entries[0].HistoricalConnection].Dispose();
                            mapNodeToErrorTimer.Remove(entries[0].HistoricalConnection);
                        }

                        var aggregatedEntities = (from c in failsEntries
                                                  where c.HistoricalConnection == entries[0].HistoricalConnection
                                                  select c).ToList();

                        pendingEntries.AddRange(aggregatedEntities);
                        bNewPendingEntries = true;
                        failsEntries.RemoveAll(c => c.HistoricalConnection == entries[0].HistoricalConnection);
                        bEnterInError = !timer;
                    }
                    else if (!mapNodeToErrorTimer.ContainsKey(entries[0].HistoricalConnection))
                    {
                        var t = new Timer((o) => { AddFailsEntries(entries, true); },
                            entries[0],
                            entries[0].ErrorTimeInterval,
                            TimeSpan.FromMilliseconds(-1));
                        mapNodeToErrorTimer[entries[0].HistoricalConnection] = t;
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

        void SetMaxTimer(String nodeId)
        {
            SetMaxTimer(nodeId, DateTime.MinValue);
        }

        void SetMaxTimer(String nodeId, DateTime recordingTime)
        {
            lock (lockObject)
            {
                var entity = mapNodeToEntity[nodeId];

                if (mapNodeToMaxTimer.ContainsKey(entity.NodeId))
                {
                    mapNodeToMaxTimer[entity.NodeId].Dispose();
                    mapNodeToMaxTimer.Remove(entity.NodeId);
                }

                if (!isActiveServer || !entity.Enabled || entity.MaxTimeInterval == TimeSpan.Zero)
                    return;

                DateTime utcNow = DateTime.UtcNow;
                var span = new TimeSpan(utcNow.Ticks % entity.MaxTimeInterval.Ticks);
                var dueTime = entity.MaxTimeInterval - span;
                if (utcNow < recordingTime)
                    dueTime += entity.MaxTimeInterval;

                //Debug.WriteLine("SetMaxTimer - dueTime = {0}, span = {1}, utcNow = {2}.{3}, last update {4}.{5}",
                //    dueTime.TotalMilliseconds, span.TotalMilliseconds,
                //    utcNow, utcNow.Millisecond,
                //    mapLastTimeUpdated[entity.NodeId], mapLastTimeUpdated[entity.NodeId].Millisecond);

                var t = new Timer((o) =>
                {
                    AddLogEntity(entity.NodeId, utcNow + dueTime, isMaxTimer: true);
                },
                            entity,
                            dueTime,
                            TimeSpan.FromMilliseconds(-1));
                mapNodeToMaxTimer[entity.NodeId] = t;
            }
        }

        void CheckAndUpdateLastTimeUpdatedValue(String nodeId)
        {
            lock (lockObject)
            {
                if (!mapLastTimeUpdated.ContainsKey(nodeId))
                    mapLastTimeUpdated[nodeId] = DateTime.UtcNow;
            }
        }

        void CheckAndUpdateHistorianAuditDataLog(UFUAHistorianLogEntity entity)
        {
            try
            {
                using (var idl = GetDataLayer(entity, bThrow: true))
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        var item = (from entry in new XPQuery<UFUAAuditDataLog>(ufw)/*.AsParallel()*/
                                    where entry.NodeId == entity.NodeId
                                    select entry).FirstOrDefault();

                        if (item != null)
                        {
                            item.Name = entity.Name;
                            item.Description = entity.Description;
                            item.HistoricalName = entity.HistoricalName;
                            item.NodeId = entity.NodeId;

                            ufw.CommitChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.UpdateHistoryDataFailed,
                                                    EventLogEntryType.Warning,
                                                    LoggerDestination.Historian,
                                                    entity.NodeId,
                                                    e.InnerException != null ? e.InnerException.Message : e.Message);
            }
        }

        bool CheckHysteresis(UFUAHistorianLogEntity entity)
        {
            if (entity.value == null)
                return false;
            if (entity.ExceptionDeviation == 0 ||
                !TypeInfo.IsNumericType(entity.value.WrappedValue.TypeInfo.BuiltInType) ||
                entity.value.WrappedValue.TypeInfo.ValueRank != ValueRanks.Scalar)
                return true;

            Double? doublePrevValue = null;
            lock (lockObject)
            {
                if (mapCheckHysteresis.ContainsKey(entity.NodeId))
                    doublePrevValue = mapCheckHysteresis[entity.NodeId];
            }

            if (!doublePrevValue.HasValue)
            {
                var lastvalue = ReadLastHistoryValue(entity);
                if (lastvalue != null && lastvalue.dValue.HasValue)
                    doublePrevValue = lastvalue.dValue;
                else
                {
                    try
                    {
                        doublePrevValue = Convert.ToDouble(entity.valueBefore.Value);
                    }
                    catch
                    {
                        doublePrevValue = Double.NaN;
                    }
                }

                lock (lockObject)
                {
                    mapCheckHysteresis[entity.NodeId] = doublePrevValue.Value;
                }
            }

            bool exceeded = false;
            if ((entity.ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfRange || entity.instrumentRange != null) &&
                (entity.ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfEURange || entity.range != null))
            {
                var doubleValue = Convert.ToDouble(entity.value.Value);
                var deadBand = new Utilities.Maths.Deadband(doublePrevValue.Value, doubleValue, entity.ExceptionDeviation) { AbsoluteDeadband = true };

                switch (entity.ExceptionDeviationFormat)
                {
                    case ExceptionDeviationFormat.AbsoluteValue:
                        exceeded = deadBand.IsExceeded();
                        break;
                    case ExceptionDeviationFormat.PercentOfRange:
                        exceeded = deadBand.IsExceeded(entity.instrumentRange.High - entity.instrumentRange.Low);
                        break;
                    case ExceptionDeviationFormat.PercentOfValue:
                        exceeded = deadBand.IsExceeded(doublePrevValue.Value);
                        break;
                    case ExceptionDeviationFormat.PercentOfEURange:
                        exceeded = deadBand.IsExceeded(entity.range.High - entity.range.Low);
                        break;
                }
            }

            if (exceeded)
            {
                lock (lockObject)
                {
                    mapCheckHysteresis[entity.NodeId] = Convert.ToDouble(entity.value.Value);
                }
            }

            return exceeded;
        }

        bool CheckQuality(UFUAHistorianLogEntity entity)
        {
            if (!entity.OnlyGood)
                return true;

            if (entity.value == null)
                return false;

            return StatusCode.IsGood(entity.value.StatusCode);
        }

        void CheckConnectionPoint(UFUAHistorianLogEntity entity)
        {
            if (restoreDataManager == null)
                return;

            var conn = XpoHelper.GetConnectionString(defaultConfig.SafelySettings,
                UFUAServerInfo.Properties.Settings.Default.HistorianFlushFolderName, entity.HistoricalName, Properties.Settings.Default.HistorianFlushFileExt);
            var file = XpoHelper.GetDataSourceFilePath(conn);
            if (file != null)
            {
                var path = System.IO.Path.GetDirectoryName(file);
                restoreDataManager.CheckConnectionPoint(entity.HistoricalConnection, path, entity.HistoricalName, Properties.Settings.Default.HistorianFlushFileExt);
            }
            else
            {
                using (var idlSafely = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var ufwSafely = new UnitOfWork(idlSafely))
                    {
                        var list = (from entry in new XPQuery<UFUAAuditDataLog>(ufwSafely)/*.AsParallel()*/
                                    where entry.NodeId == entity.NodeId
                                    select entry).ToList();
                        if (list.Count > 0)
                        {
                            var items = (from entry in new XPQuery<UFUAAuditDataItem>(ufwSafely)/*.AsParallel()*/
                                         where entry.DataLogRef == list[0].Oid
                                         select entry).Take(1).ToList();
                            if (items.Count > 0)
                            {
                                restoreDataManager.AddRestorePoint(conn, entity.HistoricalConnection);
                            }
                        }
                    }
                }
            }
        }

        UFUAAuditDataItem ReadLastHistoryValue(UFUAHistorianLogEntity uFUAHistorianLogEntity)
        {
            var idl = GetDataLayer(uFUAHistorianLogEntity);
            var ufw = GetUnitOfWork(idl);
            if (ufw != null)
            {
                try
                {
                    var info = HistorianHelper.GetAuditDataLogInfo(uFUAHistorianLogEntity.NodeId, ufw);
                    if (info != null)
                    {
                        return (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                where entry.DataLogRef == info.Oid
                                orderby entry.RecordDateTimeUtc descending
                                select entry).FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    Properties.Resources.ReadLastHistoryValueFailed,
                                    EventLogEntryType.Warning,
                                    LoggerDestination.Historian,
                                    uFUAHistorianLogEntity.NodeId, ex.Message);
                }
            }

            return null;
        }

        void ExitPendingThreads()
        {
            ExitMode = true;

            // thread for flushing records data
            Thread logthread = logThread;
            var workItems = new List<IWorkItemsGroup>();
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
                    workItems.Add(startingWorkItemsGroup);
            }

            workItems.ForEach((workItem) => workItem.Cancel(true));

#if !NET_STANDARD
            StopSync();
#endif

            DisposeErrorTimers();

            if (logthread != null)
                logthread.Join();

            smartThreadPool.WaitForIdle();
        }

        void DisposeErrorTimers()
        {
            Debug.Assert(ExitMode, "Exit mode state not entered");

            List<WaitHandle> waitHandles = new List<WaitHandle>();
            lock (lockObject)
            {
                foreach (var timer in mapNodeToErrorTimer.Values)
                {
                    var ev = new AutoResetEvent(false);
                    timer.Change(0, System.Threading.Timeout.Infinite);
                    timer.Dispose(ev);
                    waitHandles.Add(ev);
                }
                mapNodeToErrorTimer.Clear();
            }

            if (waitHandles.Count > 0)
            {
                WaitHandle.WaitAll(waitHandles.ToArray());
                waitHandles.ForEach((notify) => notify.Dispose());
            }
        }

        void SetHistorianLogEntityData(UFUAAuditDataItem item, UFUAHistorianLogEntity entity, int dataLogRef)
        {
            item.DataLogRef = dataLogRef;

            item.RecordDateTimeUtc = entity.RecordDateTime;
            item.RecordDateTimeMilliseconds = (ushort)entity.RecordDateTime.Millisecond;
            item.RecordDateTime = entity.RecordDateTime.ToLocalTime();
            item.SourceTimeStamp = entity.value.SourceTimestamp;
            item.SourcePicoseconds = entity.value.SourcePicoseconds;
            item.ServerTimeStamp = entity.value.ServerTimestamp;
            item.ServerPicoseconds = entity.value.ServerPicoseconds;
            item.StatusCode = entity.value.StatusCode.Code;

            try
            {
                item.dValue = Convert.ToDouble(entity.value.WrappedValue.Value);
                item.dValueBefore = Convert.ToDouble(entity.valueBefore.WrappedValue.Value);
            }
            catch (Exception ex)
            {

            }

            if (item.dValue.HasValue && (Double.IsNaN(item.dValue.Value) || Double.IsInfinity(item.dValue.Value)))
                item.dValue = null;
            if (item.dValueBefore.HasValue && (Double.IsNaN(item.dValueBefore.Value) || Double.IsInfinity(item.dValueBefore.Value)))
                item.dValueBefore = null;

            item.Value = entity.resolvedValue;
            item.ValueBefore = entity.resolvedValueBefore;
            item.Status = String.Format("{0}", entity.value.StatusCode);

            item.Reason = entity.Reason;
            item.UserName = entity.User;
            item.Name = entity.Name;
            item.EventId = Guid.NewGuid();

            if (defaultConfig.RedundancyServerId >= 0)
            {
                item.RedundancySyncTime = DateTime.UtcNow;
            }
        }

        void UpdateLoggerInfo()
        {
            // I used 'Count' insted of 'LongCount()' becase it causes a cpu overload due to Linq
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
                    if (logEvent == null)
                        logEvent = new AutoResetEvent(false);
                    logThread = new Thread((o) =>
                    {
                        var checkedAuditDataLog = new List<String>();
                        var applyCFR21Requirements = new List<String>();
                        var list = new List<UFUAHistorianLogEntity>();

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
                                Debug.WriteLine(String.Format("Historian Logger - Pending Entries={0}", pendingEntries.LongCount()));
#endif
                                list.AddRange(pendingEntries);
                                pendingEntries.Clear();
                                LoggerInfoNew.RecordEntriesRunning = list.Count;
                                UpdateLoggerInfo();
                            }

                            while (list.Count > 0)
                            {
                                var aggregatedEntities = (from c in list/*.AsParallel()*/
                                                          where c.HistoricalConnection == list[0].HistoricalConnection && CheckHysteresis(c) && CheckQuality(c)
                                                          select c).ToList();

                                if (aggregatedEntities.Count > 0)
                                {
                                    bool bError = false;
                                    String lasterror = null;

                                    var idl = GetDataLayer(aggregatedEntities[0]);
                                    var ufw = GetUnitOfWork(idl);

                                    if (ufw != null)
                                    {
                                        ufw.BeginTransaction();

                                        try
                                        {
                                            aggregatedEntities.ForEach(entity =>
                                            {
                                                var info = HistorianHelper.GetAuditDataLogInfo(entity.NodeId, ufw);
                                                if (info == null)
                                                {
                                                    var dataLog = HistorianHelper.GetOrCreateHistorianAuditDataLog(entity, ufw);
                                                    ufw.FlushChanges();
                                                    info = new AuditDataLogInfo() { Oid = dataLog.Oid };
                                                }
                                                else if (!checkedAuditDataLog.Contains(entity.NodeId))
                                                {
                                                    CheckAndUpdateHistorianAuditDataLog(entity);
                                                    checkedAuditDataLog.Add(entity.NodeId);
                                                }

                                                if (entity.EnableDataProtection && !applyCFR21Requirements.Contains(entity.HistoricalConnection))
                                                {
                                                    HistorianHelper.ApplyCFR21Requirements(entity.HistoricalConnection, ufw);
                                                    applyCFR21Requirements.Add(entity.HistoricalConnection);
                                                }

                                                var entry = new UFUAAuditDataItem(ufw);
                                                SetHistorianLogEntityData(entry, entity, info.Oid);
                                            });

                                            ufw.CommitChanges();

                                            OnFlushedData(new FlushedDataArgs()
                                            {
                                                connection = aggregatedEntities[0].HistoricalConnection,
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
                                        bool bEnterInError = AddFailsEntries(aggregatedEntities);

                                        byte errorCounter = 0;
                                        lock (mapErrorCounter)
                                        {
                                            if (mapErrorCounter.ContainsKey(aggregatedEntities[0].HistoricalConnection))
                                                errorCounter = mapErrorCounter[aggregatedEntities[0].HistoricalConnection];
                                            if (bEnterInError)
                                                mapErrorCounter[aggregatedEntities[0].HistoricalConnection] = ++errorCounter;
                                            if (!String.IsNullOrEmpty(lasterror))
                                                mapLastError[aggregatedEntities[0].HistoricalConnection] = lasterror;
                                        }

                                        if (bEnterInError && errorCounter != 1 && ufw != null)
                                        {
                                            HistorianHelper.TestConnection<UFUAAuditDataLog>(ufw);
                                        }

                                        list.RemoveAll(c => c.HistoricalConnection == aggregatedEntities[0].HistoricalConnection);
                                        continue;
                                    }

                                    if (restoreDataManager != null && !restoreDataManager.IsEmpty)
                                    {
                                        restoreDataManager.StartRestoring(aggregatedEntities[0].HistoricalConnection);
                                    }

                                    lock (mapErrorCounter)
                                    {
                                        mapErrorCounter.Remove(aggregatedEntities[0].HistoricalConnection);
                                    }
                                }
                                String match = list[0].HistoricalConnection;
                                list.RemoveAll(c => c.HistoricalConnection == match);
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
                Debug.WriteLine(String.Format("Historian Logger - Deleting Entries={0}", deletingEntries.LongCount()));
#endif
                var ufuaHistorianLogEntity = deletingEntries[0];
                deletingEntries.RemoveAt(0);
                if (!deletingRunning.Contains(ufuaHistorianLogEntity.NodeId))
                    deletingRunning.Add(ufuaHistorianLogEntity.NodeId);
                deletingPending.Remove(ufuaHistorianLogEntity.NodeId);
                UpdateLoggerInfo();

                if (!StartDeleteProcess(ufuaHistorianLogEntity))
                    deletingRunning.Remove(ufuaHistorianLogEntity.NodeId);
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
                    var flushing = new List<UFUAHistorianLogEntity>();

                    lock (lockObject)
                    {
#if DEBUG
                        Debug.WriteLine(String.Format("Historian Logger - Flushing Entries={0}", flushingEntries.LongCount()));
#endif
                        flushing.AddRange(flushingEntries);
                        flushingEntries.Clear();
                        LoggerInfoNew.FlushEntriesRunning += flushing.Count;
                        UpdateLoggerInfo();
                    }

                    // cycle for handle the flushing data
                    while (flushing.Count > 0)
                    {
                        // Agregate data using the same historical settings
                        var aggregatedEntities = (from c in flushing/*.AsParallel()*/
                                                  where c.HistoricalName == flushing[0].HistoricalName
                                                  select c).ToList();

                        if (aggregatedEntities.Count > 0)
                        {
                            FlushDataSafely(aggregatedEntities);
                        }


                        string match = flushing[0].HistoricalName;
                        var totalRemoved = flushing.RemoveAll(c => c.HistoricalName == match);
                        lock (lockObject)
                        {
                            LoggerInfoNew.FlushEntriesRunning -= totalRemoved;
                            UpdateLoggerInfo();
                        }
                    }

                    var safelyPath = System.IO.Path.GetDirectoryName(XpoHelper.GetDataSourceFilePath(defaultConfig.SafelySettings));
                    if (safelyPath != null)
                    {
                        safelyPath = System.IO.Path.Combine(safelyPath, UFUAServerInfo.Properties.Settings.Default.HistorianFlushFolderName);
                        var searchPattern = String.Format("*{0}", Properties.Settings.Default.HistorianFlushFileExt);
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
                                    safelyPath);
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
                    var fails = new List<UFUAHistorianLogEntity>();

                    lock (lockObject)
                    {
#if DEBUG
                        Debug.WriteLine(String.Format("Historian Logger - Fails Entries={0}", failsEntries.LongCount()));
#endif
                        fails.AddRange(failsEntries);
                        failsEntries.Clear();
                        LoggerInfoNew.FailsEntriesRunning += fails.Count;
                        UpdateLoggerInfo();
                    }

                    // cycle for handle the write error
                    while (fails.Count > 0)
                    {
                        // Agregate data using the same historical settings
                        var aggregatedEntities = (from c in fails/*.AsParallel()*/
                                                  where c.HistoricalConnection == fails[0].HistoricalConnection
                                                  select c).ToList();

                        if (aggregatedEntities.Count > 0)
                        {
                            byte errorCounter = 0;
                            String lasterror = null;
                            lock (mapErrorCounter)
                            {
                                if (mapErrorCounter.ContainsKey(aggregatedEntities[0].HistoricalConnection))
                                    errorCounter = mapErrorCounter[aggregatedEntities[0].HistoricalConnection];
                                if (mapLastError.ContainsKey(aggregatedEntities[0].HistoricalConnection))
                                {
                                    lasterror = mapLastError[aggregatedEntities[0].HistoricalConnection];
                                    mapLastError.Remove(aggregatedEntities[0].HistoricalConnection);
                                }
                            }

                            //  Check if the max error cache was exeeded
                            if (ExitMode || aggregatedEntities.Count > aggregatedEntities[0].MaxErrorCacheSize ||
                                errorCounter >= aggregatedEntities[0].MaxErrorBeforeFlush)
                            {
                                lock (lockObject)
                                {
                                    flushingEntries.AddRange(aggregatedEntities);
                                    UpdateLoggerInfo();
                                }

                                StartFlushing();

                                OnErrorFlushingData(new ErrorFlushingDataArgs()
                                {
                                    connection = aggregatedEntities[0].HistoricalConnection,
                                    exception = lasterror ?? String.Empty
                                });

                                lock (mapErrorCounter)
                                {
                                    if (mapErrorCounter.ContainsKey(aggregatedEntities[0].HistoricalConnection))
                                        mapErrorCounter[aggregatedEntities[0].HistoricalConnection] = 0;
                                }
                            }
                            else
                                AddFailsEntries(aggregatedEntities, false, true);
                        }

                        string match = fails[0].HistoricalConnection;
                        var totalRemoved = fails.RemoveAll(c => c.HistoricalConnection == match);
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

        bool NeedToDeleteOldData(UFUAHistorianLogEntity uFUAHistorianLogEntity)
        {
            if (uFUAHistorianLogEntity.MaxAge == TimeSpan.Zero || defaultConfig.MaxDeleteProcess == 0)
                return false;
            lock (mapCheckMaxAges)
            {
                bool bRet = !mapCheckMaxAges.ContainsKey(uFUAHistorianLogEntity.NodeId) || mapCheckMaxAges[uFUAHistorianLogEntity.NodeId] < DateTime.UtcNow;

                if (bRet)
                {
                    // randomizing the deletion of the records for avoiding too many concurrencies
                    TimeSpan span;
                    var rnd = new Random();
                    if (uFUAHistorianLogEntity.MaxAge.Days > 0)
                        span = TimeSpan.FromHours(1) + TimeSpan.FromMinutes((double)rnd.Next(30));
                    else if (uFUAHistorianLogEntity.MaxAge.Hours > 0)
                        span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                    else if (uFUAHistorianLogEntity.MaxAge.Minutes > 0)
                        span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                    else
                        span = uFUAHistorianLogEntity.MaxAge + TimeSpan.FromMilliseconds((double)rnd.Next(500)); ;

                    mapCheckMaxAges[uFUAHistorianLogEntity.NodeId] = DateTime.UtcNow + span;
                }

                return bRet;
            }
        }

        bool StartDeleteProcess(UFUAHistorianLogEntity uFUAHistorianLogEntity)
        {
            if (IsSchemaUpdated(uFUAHistorianLogEntity))
            {
                Process deleteProcess = null;
                try
                {
#if DEBUG
                    var watcher = new Stopwatch();
                    watcher.Start();
#endif
                    String arguments = string.Empty;
                    if (uFUAHistorianLogEntity.ResetFlag)
                    {
                        arguments = String.Format("/C\"{0}\" /R\"{1}\" /T\"{2}\" /N\"{3}\" /P\"{4}\"",
                        uFUAHistorianLogEntity.HistoricalConnection.Replace("\"", ""), uFUAHistorianLogEntity.LastTimeReset.ToString(CultureInfo.InvariantCulture),
                            defaultConfig.MaxDeletingEntities, uFUAHistorianLogEntity.NodeId, Process.GetCurrentProcess().Id);
                        uFUAHistorianLogEntity.SetResetFlag(false);
                    }
                    else
                    {
                        arguments = String.Format("/C\"{0}\" /A\"{1}\" /T\"{2}\" /N\"{3}\" /P\"{4}\"",
                        uFUAHistorianLogEntity.HistoricalConnection.Replace("\"", ""), uFUAHistorianLogEntity.MaxAge, defaultConfig.MaxDeletingEntities, uFUAHistorianLogEntity.NodeId, Process.GetCurrentProcess().Id);
                    }
                    
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
                                                            XpoHelper.GetConnectionStringWithoutPassword(uFUAHistorianLogEntity.HistoricalConnection),
                                                            uFUAHistorianLogEntity.Name, e.Data);
                            }
                        };

                        deleteProcess.OutputDataReceived += (o, e) =>
                        {
                            if (!String.IsNullOrEmpty(e.Data))
                            {
                                int count = 0;
                                if (int.TryParse(e.Data, out count))
                                {
                                    OnRecyclingData(new RecyclingDataArgs()
                                    {
                                        name = uFUAHistorianLogEntity.Name,
                                        nodeId = uFUAHistorianLogEntity.NodeId.ToString(),
                                        count = count
                                    });
                                }
                            }
                        };

                        deleteProcess.Exited += (o, e) =>
                        {
#if DEBUG
                            watcher.Stop();
                            Debug.WriteLine(String.Format("Historian Logger - Deletion of records for Tag Name {0} executed in {1}", uFUAHistorianLogEntity.Name, watcher.Elapsed));
#endif
                            ProcessExited(deleteProcess, uFUAHistorianLogEntity.NodeId);
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
                                uFUAHistorianLogEntity.NodeId,
                                ex.Message);

                    if (deleteProcess != null)
                        ProcessExited(deleteProcess, uFUAHistorianLogEntity.NodeId);
                }
            }

            return false;
        }

        void ProcessExited(Process process, String nodeId)
        {
            lock (lockObject)
            {
                process.Dispose();
                deletingRunning.Remove(nodeId);
                UpdateLoggerInfo();

                if (deletingPending.Count > 0)
                {
                    StartDeleting();
                    //smartThreadPool.QueueWorkItem(() => StartDeleting());
                }
            }
        }

        void FlushDataSafely(List<UFUAHistorianLogEntity> entries)
        {
#if DEBUG
            var watcher = new Stopwatch();
            watcher.Start();
#endif
            var fileName = String.Format("{0}_{1}{2}{3}{4}{5}",
                                        entries[0].HistoricalName,
                                        DateTime.UtcNow.Year.ToString("D4"),
                                        DateTime.UtcNow.Month.ToString("D2"),
                                        DateTime.UtcNow.Day.ToString("D2"),
                                        DateTime.UtcNow.Hour.ToString("D2"),
                                        DateTime.UtcNow.Minute.ToString("D2"));

            var total = entries.Count;
            var templist = new List<UFUAHistorianLogEntity>();
            int suffix = 0;
            while (entries.Count > 0)
            {
                bool isInError = false;
                lock (mapErrorCounter)
                {
                    isInError = mapErrorCounter.ContainsKey(entries[0].HistoricalConnection);
                }

                if (!isInError)
                {
                    lock (lockObject)
                    {
                        pendingEntries.AddRange(entries);
                        StartRecording();
                        break;
                    }
                }

                string conn = null;
                string fileBase = null;
                do
                {
                    conn = XpoHelper.GetConnectionString(defaultConfig.SafelySettings,
                        UFUAServerInfo.Properties.Settings.Default.HistorianFlushFolderName,
                        String.Format("{0}_{1}", fileName, suffix++),
                        Properties.Settings.Default.HistorianFlushFileExt);
                    fileBase = XpoHelper.GetDataSourceFilePath(conn);
                } while (fileBase != null && System.IO.File.Exists(fileBase));

                int counter = Math.Min(500, entries.Count);
                templist.AddRange(entries.GetRange(0, counter));
                entries.RemoveRange(0, counter);

                try
                {
                    var inMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                    using (var idlSafely = new SimpleDataLayer(inMemory))
                    {
                        using (var ufwSafely = new UnitOfWork(idlSafely))
                        {
                            templist.ForEach(entity =>
                            {
                                var dataLog = HistorianHelper.GetOrCreateHistorianAuditDataLog(entity, ufwSafely);
                                if (dataLog.Oid < 0)
                                    ufwSafely.FlushChanges();
                                UFUAAuditDataItem item = new UFUAAuditDataItem(ufwSafely);
                                SetHistorianLogEntityData(item, entity, dataLog.Oid);

                                var sleepTime = Properties.Settings.Default.FlushDataSleepTime;
                                if (sleepTime > 0 && !ExitMode && entries.Count > 0)
                                    Thread.Sleep(sleepTime);
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
                                Name = templist[0].HistoricalName,
                                Count = templist.Count,
                                Connection = conn,
                                Result = true
                            });
                        }
                    }

                    if (!ExitMode && restoreDataManager != null)
                    {
                        restoreDataManager.AddRestorePoint(conn, templist[0].HistoricalConnection);
                    }
                }
                catch (Exception ex)
                {
                    OnFlushedDataSafely(new FlushedDataSafelyArgs()
                    {
                        Name = templist[0].HistoricalName,
                        Count = templist.Count,
                        Connection = conn,
                        Result = false
                    });
                }
                finally
                {
                    templist.Clear();
                }
            }
#if DEBUG
            watcher.Stop();
            Debug.WriteLine(String.Format("Historian Logger - Flush Data Safely for File Name {0} executed in {1}", fileName, watcher.Elapsed));
#endif
            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                Properties.Resources.FlushDataSafely,
                EventLogEntryType.Information,
                LoggerDestination.Historian,
                fileName, total);
        }

        static IDataLayer CreateDataLayer(String settings)
        {
            return HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(settings);
        }

        IDataLayer GetDataLayer(UFUAHistorianLogEntity uFUAHistorianLogEntity, bool bThrow = false)
        {
            lock (mapDataLayers)
            {
                try
                {
                    if (!ExitMode && !mapDataLayers.ContainsKey(uFUAHistorianLogEntity.HistoricalConnection))
                        mapDataLayers[uFUAHistorianLogEntity.HistoricalConnection] = CreateDataLayer(uFUAHistorianLogEntity.HistoricalConnection);

                    if (mapDataLayers.ContainsKey(uFUAHistorianLogEntity.HistoricalConnection))
                        return mapDataLayers[uFUAHistorianLogEntity.HistoricalConnection];
                }
                catch (Exception e)
                {
                    var msg = String.Format("{0} - {1}", DateTime.Now, String.Format(Properties.Resources.FailedCreateDataLayer,
                        XpoHelper.GetConnectionStringWithoutPassword(uFUAHistorianLogEntity.HistoricalConnection),
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

        bool IsSchemaUpdated(UFUAHistorianLogEntity uFUAHistorianLogEntity)
        {
            lock (lockObject)
            {
                if (schemaUpdated.Contains(uFUAHistorianLogEntity.HistoricalConnection))
                    return true;

                if (!schemaUpdating.Contains(uFUAHistorianLogEntity.HistoricalConnection))
                {
                    schemaUpdating.Add(uFUAHistorianLogEntity.HistoricalConnection);
                    smartThreadPool.QueueWorkItem(() =>
                    {
                        var successfull = HistorianHelper.TryUpdateSchema<UFUAAuditDataItem>(uFUAHistorianLogEntity.HistoricalConnection, typeof(UFUAAuditDataLog));
                        lock (lockObject)
                        {
                            schemaUpdated.Remove(uFUAHistorianLogEntity.HistoricalConnection);
                            schemaUpdating.Remove(uFUAHistorianLogEntity.HistoricalConnection);
                            if (successfull)
                                schemaUpdated.Add(uFUAHistorianLogEntity.HistoricalConnection);
                        }
                    });
                }
            }

            return false;
        }

        bool UpdateDatabaseSchema(String settings)
        {
            lock (lockObject)
            {
                if (schemaUpdating.Contains(settings))
                    return false;

                schemaUpdating.Add(settings);
            }

            bool successfull = false;
            try
            {
                HistorianHelper.UpdateSchema<UFUAAuditDataItem>(settings, typeof(UFUAAuditDataLog));
                successfull = true;
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
            finally
            {
                lock (lockObject)
                {
                    schemaUpdating.Remove(settings);
                    if (successfull && !schemaUpdated.Contains(settings))
                        schemaUpdated.Add(settings);
                }
            }

            return successfull;
        }

        #endregion

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

                foreach (var timer in mapNodeToMaxTimer.Values)
                    timer.Dispose();
                mapNodeToMaxTimer.Clear();

                foreach (var timer in mapNodeToMinTimer.Values)
                    timer.Dispose();
                mapNodeToMinTimer.Clear();

                foreach (var timer in mapNodeToErrorTimer.Values)
                    timer.Dispose();
                mapNodeToErrorTimer.Clear();

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

        public IList<UFUAAuditDataItem> ReadHistory(UFUAHistorianLogEntity uFUAHistorianLogEntity,
                                                    DateTime start, DateTime end, bool bReadModifiedData, int maxValues = 0)
        {
            if (!IsSchemaUpdated(uFUAHistorianLogEntity))
                throw new ServiceResultException(StatusCodes.BadOutOfService);

            lockHistory.WaitOne();

            try
            {
                using (var idl = GetDataLayer(uFUAHistorianLogEntity, bThrow: true))
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

                        var info = HistorianHelper.GetAuditDataLogInfo(uFUAHistorianLogEntity.NodeId, ufw);
                        if (info == null)
                            return new List<UFUAAuditDataItem>();

                        if (maxValues > 0)
                        {
                            return (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                    where entry.DataLogRef == info.Oid &&
                                    (start == DateTime.MinValue || entry.SourceTimeStamp >= start) &&
                                    (end == DateTime.MinValue || entry.SourceTimeStamp <= end) &&
                                    (!bReadModifiedData || entry.ModificationType != HistoryUpdateType.Delete)
                                    orderby entry.SourceTimeStamp
                                    select entry).Take(maxValues).ToList();
                        }
                        else
                        {
                            return (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                    where entry.DataLogRef == info.Oid &&
                                    (start == DateTime.MinValue || entry.SourceTimeStamp >= start) &&
                                    (end == DateTime.MinValue || entry.SourceTimeStamp <= end) &&
                                    (!bReadModifiedData || entry.ModificationType != HistoryUpdateType.Delete)
                                    orderby entry.SourceTimeStamp
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

        public uint DeleteHistory(UFUAHistorianLogEntity uFUAHistorianLogEntity, DateTime value)
        {
            if (!IsSchemaUpdated(uFUAHistorianLogEntity))
                return StatusCodes.BadOutOfService;

            if (uFUAHistorianLogEntity.EnableDataProtection)
                return StatusCodes.BadUserAccessDenied;

            lockHistory.WaitOne();

            try
            {
                using (var idl = GetDataLayer(uFUAHistorianLogEntity, bThrow: true))
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        var info = HistorianHelper.GetAuditDataLogInfo(uFUAHistorianLogEntity.NodeId, ufw);
                        if (info == null)
                            return StatusCodes.BadNoEntryExists;

                        var listToDelete = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                            where entry.DataLogRef == info.Oid &&
                                            entry.SourceTimeStamp == value && entry.ModificationType != HistoryUpdateType.Delete
                                            select entry).ToList();

                        if (listToDelete.Count == 0)
                        {
                            return StatusCodes.BadNoEntryExists;
                        }

                        listToDelete.ForEach(entry =>
                        {
                            entry.ModificationTime = DateTime.UtcNow;
                            entry.ModificationType = HistoryUpdateType.Delete;

                            if (defaultConfig.RedundancyServerId >= 0)
                                entry.RedundancySyncTime = DateTime.UtcNow;

                            SetHistorianLogEntityData(entry, uFUAHistorianLogEntity, info.Oid);
                        });

                        ufw.CommitChanges();

                        return StatusCodes.Good;
                    }
                }
            }
            finally
            {
                lockHistory.Release();
            }
        }

        public uint UpdateHistory(UFUAHistorianLogEntity uFUAHistorianLogEntity, PerformUpdateType performUpdateType)
        {
            if (!IsSchemaUpdated(uFUAHistorianLogEntity))
                return StatusCodes.BadOutOfService;

            if (uFUAHistorianLogEntity.EnableDataProtection)
                return StatusCodes.BadUserAccessDenied;

            lockHistory.WaitOne();

            try
            {
                using (var idl = GetDataLayer(uFUAHistorianLogEntity, bThrow: true))
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        var info = HistorianHelper.GetAuditDataLogInfo(uFUAHistorianLogEntity.NodeId, ufw);
                        if (info == null)
                            return StatusCodes.BadNoEntryExists;

                        if (performUpdateType == PerformUpdateType.Replace)
                        {
                            var listToUpdate = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                                where entry.DataLogRef == info.Oid &&
                                                entry.SourceTimeStamp == uFUAHistorianLogEntity.value.SourceTimestamp &&
                                                entry.ModificationType != HistoryUpdateType.Delete
                                                select entry).ToList();

                            if (listToUpdate.Count == 0)
                            {
                                return StatusCodes.BadNoEntryExists;
                            }

                            listToUpdate.ForEach(entry =>
                            {
                                entry.ModificationTime = DateTime.UtcNow;
                                entry.ModificationType = HistoryUpdateType.Replace;

                                if (defaultConfig.RedundancyServerId >= 0)
                                    entry.RedundancySyncTime = DateTime.UtcNow;

                                SetHistorianLogEntityData(entry, uFUAHistorianLogEntity, info.Oid);
                            });

                            ufw.CommitChanges();
                        }
                        else if (performUpdateType == PerformUpdateType.Insert)
                        {
                            var listToUpdate = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                                                where entry.DataLogRef == info.Oid &&
                                                entry.SourceTimeStamp == uFUAHistorianLogEntity.value.SourceTimestamp &&
                                                entry.ModificationType != HistoryUpdateType.Delete
                                                select entry).ToList();

                            if (listToUpdate.Count > 0)
                            {
                                return StatusCodes.BadEntryExists;
                            }

                            UFUAAuditDataItem item = new UFUAAuditDataItem(ufw);
                            SetHistorianLogEntityData(item, uFUAHistorianLogEntity, info.Oid);

                            item.ModificationTime = DateTime.UtcNow;
                            item.ModificationType = HistoryUpdateType.Insert;

                            ufw.CommitChanges();
                        }

                        return StatusCodes.Good;
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
