using DevExpress.Xpo;
using log4net;
using RedundancyHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFUAHistorian.Redundancy
{
    internal abstract class RedundancySyncronization<T> : RedundancyHistory.RedundancyHistory where T : UFUAHistorianModel.IRedundancyDataSync
    {
        #region Declarations
        readonly Dictionary<int, IList<T>> syncDataItems = new Dictionary<int, IList<T>>();
        readonly Dictionary<int, IList<String>> pendingUniqueIds = new Dictionary<int, IList<String>>();
        readonly Dictionary<int, DateTime> syncStartTimes = new Dictionary<int, DateTime>();

        readonly object lockObject = new object();
        #endregion

        #region Constructors
        protected RedundancySyncronization(int maxTake) : 
            base(maxTake)
        {
        }

        protected RedundancySyncronization(int maxTake, int threadpool) :
            base(maxTake, threadpool)
        {
        }

        protected RedundancySyncronization(int maxTake, int threadpool, ILog log) :
            base(maxTake, threadpool, log)
        {
        }
        #endregion

        #region Overrides
        protected override DateTime ReadSyncStartTime(string destinationConn)
        {
            using (var idldest = GetThreadSafeDataLayer(destinationConn))
            {
                using (var ufwdest = new UnitOfWork(idldest))
                {
                    return (from entry in new XPQuery<T>(ufwdest)/*.AsParallel()*/
                            where entry.RedundancySyncTime != null
                            orderby entry.RedundancySyncTime descending
                            select entry.RedundancySyncTime).FirstOrDefault();
                }
            }
        }

        protected override int ReadSyncDataValues(int syncId, string sourceConn, DateTime startTime, DateTime endTime, int maxTake)
        {
            using (var idlsource = GetThreadSafeDataLayer(sourceConn))
            {
                using (var ufwsource = new UnitOfWork(idlsource))
                {
                    List<T> dataitems = null;
                    if (endTime != DateTime.MinValue)
                    {
                        dataitems = (from entry in new XPQuery<T>(ufwsource)/*.AsParallel()*/
                                     where entry.RedundancySyncTime != null &&
                                     entry.RedundancySyncTime >= startTime && entry.RedundancySyncTime < endTime
                                     orderby entry.RedundancySyncTime ascending
                                     select entry).Take(maxTake).ToList();
                    }
                    else
                    {
                        dataitems = (from entry in new XPQuery<T>(ufwsource)/*.AsParallel()*/
                                     where entry.RedundancySyncTime != null &&
                                     entry.RedundancySyncTime >= startTime
                                     orderby entry.RedundancySyncTime ascending
                                     select entry).Take(maxTake).ToList();
                    }

                    lock (lockObject)
                    {
                        if (dataitems != null)
                        {
                            syncDataItems[syncId] = dataitems;
                        }
                    }

                    return dataitems.Count;
                }
            }
        }

        protected override void ElaborateSyncDataValues(int syncId, int maxRecords)
        {
            List<T> dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                    dataitems = syncDataItems[syncId] as List<T>;
            }

            DateTime newStartTime = DateTime.MaxValue;
            if (dataitems != null && dataitems.Count == maxRecords)
            {
                newStartTime = dataitems[dataitems.Count - 1].RedundancySyncTime;
            }

            List<String> pendingIds = null;
            if (dataitems != null)
                pendingIds = (from c in dataitems select c.RedundancyUniqueId.ToString()).ToList();

            lock (lockObject)
            {
                syncStartTimes[syncId] = newStartTime;
                pendingUniqueIds[syncId] = pendingIds ?? new List<String>();
            }
        }

        protected override DateTime GetNewSyncStartTime(int SyncId)
        {
            lock (lockObject)
            {
                if (syncStartTimes.ContainsKey(SyncId))
                    return syncStartTimes[SyncId];

                return DateTime.MinValue;
            }
        }

        protected override IList<String> GetPendingSyncIds(int syncId)
        {
            lock (lockObject)
            {
                List<String> guids = new List<String>();
                if (pendingUniqueIds.ContainsKey(syncId))
                    guids.AddRange(pendingUniqueIds[syncId]);

                return guids;
            }
        }

        protected override int RemoveDuplicatedSyncDataValues(int syncId, string destinationConn, IList<String> pendingUniqueIds)
        {
            int removedCounter = 0;
            List<T> dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                {
                    dataitems = syncDataItems[syncId] as List<T>;
                }
            }

            if (dataitems != null && dataitems.Count > 0)
            {
                removedCounter += dataitems.RemoveAll(c => pendingUniqueIds.Contains(c.RedundancyUniqueId.ToString()));
                if (dataitems.Count > 0)
                {
                    using (var idldest = GetThreadSafeDataLayer(destinationConn))
                    {
                        using (var ufwdest = new UnitOfWork(idldest))
                        {
                            var eventids = new List<Guid>();
                            var uniqueIds = new List<Guid>();
                            for (int ii = 0; ii < dataitems.Count; ii++)
                            {
                                uniqueIds.Add(dataitems[ii].RedundancyUniqueId);
                                if (uniqueIds.Count >= 50 || ii == dataitems.Count - 1)
                                {
                                    eventids.AddRange((from entry in new XPQuery<T>(ufwdest)/*.AsParallel()*/
                                                        where uniqueIds.Contains(entry.RedundancyUniqueId)
                                                        select entry.RedundancyUniqueId).ToList());
                                    uniqueIds.Clear();
                                }
                            }

                            removedCounter += dataitems.RemoveAll(c => eventids.Contains(c.RedundancyUniqueId));
                        }
                    }
                }
            }

            return removedCounter;
        }

        protected override int WriteSyncDataValues(int syncId, string destinationConn)
        {
            int writeCounter = 0;
            List<T> dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                {
                    dataitems = syncDataItems[syncId] as List<T>;
                }
            }

            if (dataitems != null && dataitems.Count > 0)
            {
                using (var idldest = GetThreadSafeDataLayer(destinationConn))
                {
                    using (var ufwdest = new UnitOfWork(idldest))
                    {
                        writeCounter = WriteSyncDataValues(dataitems, ufwdest);
                    }
                }
            }

            return writeCounter;
        }

        protected override string GetConnectionStringDisplayName(string sourceConn)
        {
            return XpoHelpers.XpoHelper.GetDataSourceTitle(sourceConn);
        }

        protected override void CleanSyncDataValues(int syncId)
        {
            lock (lockObject)
            {
                syncDataItems.Remove(syncId);
                pendingUniqueIds.Remove(syncId);
                syncStartTimes.Remove(syncId);
            }
        }

#if DEBUG
        protected override string GetRedundancyName()
        {
            if (typeof(T) == typeof(UFUAHistorianModel.UFUAAuditDataItem))
                return "HistorianSyncronization";
            else if (typeof(T) == typeof(UFUAHistorianModel.UFUAAuditLogItem))
                return "EventSyncronization";
            else
                return "Unknow";
        }
#endif
        #endregion

        #region Abstracts Methods
        protected abstract IDataLayer GetThreadSafeDataLayer(String settings);
        protected abstract int WriteSyncDataValues(List<T> dataitems, UnitOfWork ufw);
        #endregion
    }
}
