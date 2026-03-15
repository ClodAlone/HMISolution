using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;

namespace UFUAHistorian.Redundancy
{
    internal class HistorianSyncronization : UFUAHistorian.Redundancy.RedundancySyncronization<UFUAAuditDataItem>
    {
        #region Declarations
        readonly Dictionary<int, IList<UFUAAuditDataLog>> syncDataLogs = new Dictionary<int, IList<UFUAAuditDataLog>>();
        readonly Dictionary<String, IDataLayer> mapDataLayers = new Dictionary<String, IDataLayer>();
        readonly List<String> checkedAuditDataLog = new List<String>();

        readonly object lockObject = new object();
        #endregion

        #region Constructors
        public HistorianSyncronization(int maxTake) : 
            base(maxTake)
        {
        }

        public HistorianSyncronization(int maxTake, int threadpool) :
            base(maxTake, threadpool)
        {
        }

        public HistorianSyncronization(int maxTake, int threadpool, ILog log) :
            base(maxTake, threadpool, log)
        {
        }
        #endregion

        #region Overrides
        protected override IDataLayer GetThreadSafeDataLayer(String settings)
        {
            lock (mapDataLayers)
            {
                if (!mapDataLayers.ContainsKey(settings))
                    mapDataLayers[settings] = HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(settings);

                return mapDataLayers[settings];
            }
        }

        protected override int ReadSyncDataValues(int syncId, string sourceConn, DateTime startTime, DateTime endTime, int maxTake)
        {
            using (var idlsource = GetThreadSafeDataLayer(sourceConn))
            {
                using (var ufwsource = new UnitOfWork(idlsource))
                {
                    List<UFUAAuditDataLog> datalogs = null;
                    datalogs = (from entry in new XPQuery<UFUAAuditDataLog>(ufwsource)/*.AsParallel()*/
                                select entry).ToList();

                    lock (lockObject)
                    {
                        if (datalogs != null)
                        {
                            syncDataLogs[syncId] = datalogs;
                        }
                    }
                }
            }

            return base.ReadSyncDataValues(syncId, sourceConn, startTime, endTime, maxTake);
        }

        protected override int WriteSyncDataValues(int syncId, string destinationConn)
        {
            List<UFUAAuditDataLog> datalogs = null;
            lock (lockObject)
            {
                if (syncDataLogs.ContainsKey(syncId))
                {
                    datalogs = syncDataLogs[syncId] as List<UFUAAuditDataLog>;
                    datalogs.RemoveAll(c => checkedAuditDataLog.Contains(c.NodeId));
                }
            }

            if (datalogs != null && datalogs.Count > 0)
            {
                using (var idldest = GetThreadSafeDataLayer(destinationConn))
                {
                    using (var ufwdest = new UnitOfWork(idldest))
                    {
                        var nodeIds = (from c in new XPQuery<UFUAAuditDataLog>(ufwdest)
                                       select c.NodeId).ToList();

                        foreach (var entry in datalogs)
                        {
                            if (!nodeIds.Contains(entry.NodeId))
                            {
                                nodeIds.Add(entry.NodeId);
                                var datalog = new UFUAAuditDataLog(ufwdest)
                                {
                                    NodeId = entry.NodeId,
                                    Name = entry.Name,
                                    Description = entry.Description,
                                    HistoricalName = entry.HistoricalName
                                };
                            }
                        }

                        ufwdest.CommitChanges();

                        lock (lockObject)
                        {
                            nodeIds.ForEach(nodeId =>
                            {
                                if (!checkedAuditDataLog.Contains(nodeId))
                                    checkedAuditDataLog.Add(nodeId);
                            });
                        }
                    }
                }
            }

            return base.WriteSyncDataValues(syncId, destinationConn);
        }

        protected override int WriteSyncDataValues(List<UFUAAuditDataItem> dataitems, UnitOfWork ufw)
        {
            int writeCounter = 0;

            while (dataitems.Count > 0 && !IsAborting())
            {
                var aggregateditems = (from entry in dataitems
                                        where entry.Name == dataitems[0].Name
                                        select entry).ToList();

                var datalog = (from c in new XPQuery<UFUAAuditDataLog>(ufw)
                               where c.Name == dataitems[0].Name
                               select c).First();

                foreach (var itemsource in aggregateditems)
                {
                    UFUAAuditDataItem itemdest = new UFUAAuditDataItem(ufw, itemsource)
                    {
                        DataLogRef = datalog.Oid
                    };

                    if (IsActiveServer)
                        itemdest.RedundancySyncTime = DateTime.UtcNow;

                    writeCounter++;
                }

                ufw.CommitChanges();

                string match = dataitems[0].Name;
                dataitems.RemoveAll(c => c.Name == match);
            }

            return writeCounter;
        }

        protected override void CleanSyncDataValues(int syncId)
        {
            lock (lockObject)
            {
                syncDataLogs.Remove(syncId);
            }

            base.CleanSyncDataValues(syncId);
        }
        #endregion
    }
}
