using Amib.Threading;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace RedundancyHistory
{
    public abstract class RedundancyHistory : IRedundancyHistory
    {
        #region Declarations
        bool AbortSync;
        int lastSyncId;
        
        List<String> syncHistoryDataQueues;

        Dictionary<String, IList<String>> globalPendingUniqueIds;
        Dictionary<String, DateTime> syncStartTimes;
        Dictionary<String, int> maxTakeRecordsOnRead;

        SmartThreadPool smartThreadPool;
        IWorkItemsGroup synchronizeHistoryDataWorkItemsGroup;

        readonly int redundancyMaxSyncEntities = 10000;
        readonly int redundancyThreadpool = -1;
        readonly protected ILog log;
        
        readonly object lockObject = new object();
        readonly object lockSyncData = new object();
        #endregion

        #region Constructors
        protected RedundancyHistory()
        {
            Initialize();
        }

        protected RedundancyHistory(int maxTake)
        {
            this.redundancyMaxSyncEntities = maxTake;

            Initialize();
        }

        protected RedundancyHistory(int maxTake, int threadpool)
        {
            this.redundancyMaxSyncEntities = maxTake;
            this.redundancyThreadpool = threadpool;

            Initialize();
        }

        protected RedundancyHistory(int maxTake, int threadpool, ILog log)
        {
            this.redundancyMaxSyncEntities = maxTake;
            this.redundancyThreadpool = threadpool;
            this.log = log;

            Initialize();
        }
        
        void Initialize()
        {
            var startupInfo = new STPStartInfo()
            {
                ThreadPoolName = "RedundancySyncDataThreadPool",
                ThreadPriority = System.Threading.ThreadPriority.BelowNormal,
                MaxWorkerThreads = redundancyThreadpool > 0 ? redundancyThreadpool : SysInfo.GetNumberOfLogicalProcessors(),
                AreThreadsBackground = false
            };
            smartThreadPool = new SmartThreadPool(startupInfo);

            if (synchronizeHistoryDataWorkItemsGroup == null)
            {
                WIGStartInfo wigStartInfo = new WIGStartInfo() { WorkItemPriority = WorkItemPriority.Normal };
                synchronizeHistoryDataWorkItemsGroup = smartThreadPool.CreateWorkItemsGroup(startupInfo.MaxWorkerThreads, wigStartInfo);
            }

            syncHistoryDataQueues = new List<String>();
            globalPendingUniqueIds = new Dictionary<String, IList<string>>();
            syncStartTimes = new Dictionary<String, DateTime>();
            maxTakeRecordsOnRead = new Dictionary<String, int>();
        }
        #endregion

        #region IRedundancySyncData
        public void SynchronizeHistoryData(String sourceConn, String destinationConn, DateTime startTime, DateTime endTime)
        {
            Debug.Assert(String.Compare(sourceConn, destinationConn, true) != 0, "SynchronizeHistoryData : Source and Destination connections cannot be equal !");

            lock (lockObject)
            {
                if (AbortSync)
                    return;

                var sourceKey = sourceConn.ToLower();
                if (XpoHelpers.XpoHelper.IsDataSource(sourceKey))
                    sourceKey = XpoHelpers.XpoHelper.GetDataSourceLockName(sourceKey);
                var destKey = destinationConn.ToLower();
                if (XpoHelpers.XpoHelper.IsDataSource(destKey))
                    destKey = XpoHelpers.XpoHelper.GetDataSourceLockName(destKey);

                var syncKey = String.Format("{0}.{1}", sourceKey, destKey);
                if (syncHistoryDataQueues.Count((item) => item == syncKey) <= synchronizeHistoryDataWorkItemsGroup.Concurrency)
                {
#if DEBUG
                    Stopwatch watch = new Stopwatch();
                    Stopwatch watchQueue = new Stopwatch();
                    watchQueue.Start();
#endif
                    syncHistoryDataQueues.Add(syncKey);
                    int syncId = unchecked(lastSyncId + 1);
                    lastSyncId = syncId;
                    synchronizeHistoryDataWorkItemsGroup.QueueWorkItem(() =>
                    {
                        DateTime newStartTime = DateTime.MinValue;
                        int currentMaxTake = redundancyMaxSyncEntities;
                        int readCounter = 0;
                        int writeCounter = 0;
                        int removeCounter = 0;
                        bool bError = false;
                        bool bContinue = false;

                        try
                        {
                            List<String> pendingUniqueIds;
                            lock (lockSyncData)
                            {
                                if (AbortSync)
                                    return;

                                if (startTime == DateTime.MinValue)
                                    startTime = GetSyncStartTime(sourceKey, destinationConn);
                                currentMaxTake = GetMaxTakeRecordsOnRead(sourceKey);
                                readCounter = ReadSyncDataValues(syncId, sourceConn, startTime, endTime, currentMaxTake);
                                ElaborateSyncDataValues(syncId, currentMaxTake);
                                pendingUniqueIds = GetGlobalPendingUniqueIds(destKey);
                                AddGlobalPendingSyncIds(destKey, GetPendingSyncIds(syncId));
                                newStartTime = GetNewSyncStartTime(syncId);
                                UpdateSyncStartTime(sourceKey, newStartTime);
                            }

                            bContinue = newStartTime != DateTime.MaxValue;
                            removeCounter = RemoveDuplicatedSyncDataValues(syncId, destinationConn, pendingUniqueIds);
                            writeCounter = WriteSyncDataValues(syncId, destinationConn);
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            WriteLogMessage(Properties.Resources.FailedSynchronizingDatabase,
                                GetConnectionStringDisplayName(sourceConn),
                                ex.InnerException != null ?
                                ex.InnerException.Message : ex.Message);
                        }
                        finally
                        {
                            lock (lockSyncData)
                            {
                                RemoveGlobalPendingSyncIds(destKey, GetPendingSyncIds(syncId));
                                // Start Time value can be cleaned because the records to synchronize are terminated.
                                if (!bError && !bContinue)
                                    RemoveSyncStartTimes(sourceKey);

                                if (bError && startTime != DateTime.MinValue)
                                {
                                    RestoreSyncStartTime(sourceKey, startTime);
                                }
                                else if (removeCounter == currentMaxTake && startTime == newStartTime)
                                {
                                    // Increase the MaxTake for taking other records with the same datetime that exceed from the current Max Take.
                                    currentMaxTake += redundancyMaxSyncEntities;
                                    maxTakeRecordsOnRead[sourceKey] = currentMaxTake;
                                }
                                else if (maxTakeRecordsOnRead.ContainsKey(sourceKey))
                                {
                                    // Now we can restore the initial MaxTake.
                                    maxTakeRecordsOnRead.Remove(sourceKey);
                                }

                                CleanSyncDataValues(syncId);
                            }

                            lock (lockObject)
                            {
#if DEBUG
                                StringBuilder msg = new StringBuilder();
                                msg.AppendLine("----------------------------------------------------------------------------------------------------");
                                msg.AppendFormat("Redundancy Job Name     = {0}\n", GetRedundancyName());
                                msg.AppendFormat("Synchronization id      = {0}\n", syncId);
                                msg.AppendFormat("Synchronization key     = {0}\n", syncKey);
                                msg.AppendFormat("Source Connection       = {0}\n", sourceConn);
                                msg.AppendFormat("Destination Connection  = {0}\n", destinationConn);
                                msg.AppendFormat("Start Time              = {0}\n", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                msg.AppendFormat("End Time                = {0}\n", endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                msg.AppendFormat("Time Execution          = {0}\n", watchQueue.ElapsedMilliseconds);
                                msg.AppendFormat("Will Continue           = {0}\n", bContinue);
                                msg.AppendFormat("Records Read            = {0}\n", readCounter);
                                msg.AppendFormat("Records Written         = {0}\n", writeCounter);
                                msg.AppendFormat("Records Ignored         = {0}\n", removeCounter);
                                msg.AppendLine("----------------------------------------------------------------------------------------------------");
                                System.Diagnostics.Debug.WriteLine(msg);
#endif
                                syncHistoryDataQueues.Remove(syncKey);
                                
                                if (bContinue)
                                    SynchronizeHistoryData(sourceConn, destinationConn, DateTime.MinValue, endTime);
                            }
                        }
                    });
                }
            }
        }

        public void StopSync()
        {
            try
            {
                AbortSync = true;
                if (synchronizeHistoryDataWorkItemsGroup != null)
                    synchronizeHistoryDataWorkItemsGroup.WaitForIdle();
            }
            finally
            {
                AbortSync = false;
            }

            ClearSyncHistory();
        }
        #endregion

        #region Public Properties
        bool isActiveServer;
        public bool IsActiveServer
        {
            get
            {
                return isActiveServer;
            }
            set
            {
                isActiveServer = value;
            }
        }

        #endregion

        #region Private Methods
        DateTime GetSyncStartTime(string key, string destinationConn)
        {
            DateTime startTime = DateTime.MinValue;
            if (syncStartTimes.ContainsKey(key))
                startTime = syncStartTimes[key];
            if (startTime == DateTime.MinValue)
                startTime = ReadSyncStartTime(destinationConn);
            if (startTime == DateTime.MinValue)
                startTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (startTime == DateTime.MaxValue)
                startTime = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            return startTime;
        }

        void RestoreSyncStartTime(string key, DateTime startTime)
        {
            if (!syncStartTimes.ContainsKey(key) || syncStartTimes[key] > startTime)
                syncStartTimes[key] = startTime;
        }

        void UpdateSyncStartTime(string key, DateTime startTime)
        {
            if (!syncStartTimes.ContainsKey(key) || syncStartTimes[key] < startTime)
                syncStartTimes[key] = startTime;
        }

        void RemoveSyncStartTimes(string key)
        {
            if (syncStartTimes.ContainsKey(key))
                syncStartTimes.Remove(key);
        }

        int GetMaxTakeRecordsOnRead(String key)
        {
            if (maxTakeRecordsOnRead.ContainsKey(key))
                return maxTakeRecordsOnRead[key];
            return redundancyMaxSyncEntities;
        }

        List<String> GetGlobalPendingUniqueIds(String key)
        {
            var pending = new List<String>();
            lock (lockSyncData)
            {
                if (globalPendingUniqueIds.ContainsKey(key))
                    pending.AddRange(globalPendingUniqueIds[key]);

                return pending;
            }
        }

        void AddGlobalPendingSyncIds(String key, IList<String> guids)
        {
            if (!globalPendingUniqueIds.ContainsKey(key))
                globalPendingUniqueIds[key] = new List<String>();
            var pendingUniqueIds = globalPendingUniqueIds[key] as List<String>;
            pendingUniqueIds.AddRange(guids);
        }

        void RemoveGlobalPendingSyncIds(String key, IList<String> guids)
        {
            if (globalPendingUniqueIds.ContainsKey(key))
            {
                var pendingUniqueIds = globalPendingUniqueIds[key];
                foreach (var guid in guids)
                    pendingUniqueIds.Remove(guid);
            }
        }

        void RemoveGlobalPendingSyncIds(String key)
        {
            if (globalPendingUniqueIds.ContainsKey(key))
            {
                var pendingUniqueIds = globalPendingUniqueIds[key] as List<String>;
                pendingUniqueIds.Clear();
            }
        }

        void WriteLogMessage(String format, params object[] args)
        {
            var logMessage = String.Format("{0} - {1}", DateTime.Now, String.Format(format, args));
            Console.WriteLine(logMessage);
            if (log != null)
                log.ErrorFormat(format, args);
        }

        void ClearSyncHistory()
        {
            syncStartTimes.Clear();
            globalPendingUniqueIds.Clear();
            maxTakeRecordsOnRead.Clear();
        }
        #endregion

        #region Protected Methods
        protected bool IsAborting()
        {
            return AbortSync;
        }
        #endregion

        #region Abstracts Methods
        /// <summary>
        /// Read the first datetime to use for synchronizing data.
        /// </summary>
        /// <param name="destinationConn"></param>
        /// <returns></returns>
        protected abstract DateTime ReadSyncStartTime(string destinationConn);

        /// <summary>
        /// Read the sync data values from historical database.
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="sourceConn"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="syncDirection"></param>
        /// <returns>
        /// Return the number of records readed.
        /// </returns>
        protected abstract int ReadSyncDataValues(int syncId, string sourceConn, DateTime startTime, DateTime endTime, int maxTake);

        /// <summary>
        /// Elaborate the sync data values for retreiving need informations.
        /// </summary>
        /// <param name="syncId"></param>
        protected abstract void ElaborateSyncDataValues(int syncId, int maxRecords);

        /// <summary>
        /// Get new synchronization start time to use for next read.
        /// </summary>
        /// <param name="SyncId"></param>
        /// <returns></returns>
        protected abstract DateTime GetNewSyncStartTime(int SyncId);

        /// <summary>
        /// Get the pending unique ids of the current operations.
        /// </summary>
        /// <param name="syncId"></param>
        /// <returns></returns>
        protected abstract IList<String> GetPendingSyncIds(int syncId);

        /// <summary>
        /// Remove any duplicated data values from cache.
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="destinationConn"></param>
        /// <returns>
        /// Return the number of elements removed.
        /// </returns>
        protected abstract int RemoveDuplicatedSyncDataValues(int syncId, string destinationConn, IList<String> pendingUniqueIds);

        /// <summary>
        /// Write the sync data values to historica databse.
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="destinationConn"></param>
        /// <returns>
        /// Return the number of elements flushed.
        /// </returns>
        protected abstract int WriteSyncDataValues(int syncId, string destinationConn);

        /// <summary>
        /// Clean all sync data values.
        /// </summary>
        /// <param name="syncId"></param>
        protected abstract void CleanSyncDataValues(int syncId);

        /// <summary>
        /// Get the connection string name to display for messages.
        /// </summary>
        /// <param name="sourceConn"></param>
        /// <returns></returns>
        protected abstract string GetConnectionStringDisplayName(string sourceConn);

#if DEBUG
        /// <summary>
        /// Get the connection string name to display for messages.
        /// </summary>
        protected abstract string GetRedundancyName();
#endif
        #endregion
    }
}
