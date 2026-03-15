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
    internal class EventSyncronization : UFUAHistorian.Redundancy.RedundancySyncronization<UFUAAuditLogItem>
    {
        #region Declarations
        readonly bool isProtected;
		
        readonly Dictionary<String, IDataLayer> mapDataLayers = new Dictionary<String, IDataLayer>();
        #endregion
        
        #region Constructors
        public EventSyncronization(int maxTake) : 
            base(maxTake)
        {
        }

        public EventSyncronization(int maxTake, int threadpool) :
            base(maxTake, threadpool)
        {
        }

        public EventSyncronization(int maxTake, int threadpool, ILog log) :
            base(maxTake, threadpool, log)
        {
        }

        public EventSyncronization(int maxTake, int threadpool, ILog log, bool isProtected) :
            base(maxTake, threadpool, log)
        {
            this.isProtected = isProtected;
        }
        #endregion

        #region Overrides
        protected override IDataLayer GetThreadSafeDataLayer(String settings)
        {
            lock (mapDataLayers)
            {
                if (!mapDataLayers.ContainsKey(settings))
                    mapDataLayers[settings] = HistorianHelper.CreateDataLayer<UFUAAuditLogItem>(settings);

                return mapDataLayers[settings];
            }
        }

        protected override int WriteSyncDataValues(List<UFUAAuditLogItem> dataitems, UnitOfWork ufw)
        {
            int writeCounter = 0;
            int commitThreshold = System.Math.Max(dataitems.Count / 100, 1);
            while (dataitems.Count > 0 && !IsAborting())
            {
                UFUAAuditLogItem itemsource = dataitems[0];
                UFUAAuditLogItem itemdest = new UFUAAuditLogItem(ufw, itemsource);
                if (IsActiveServer)
                    itemdest.RedundancySyncTime = DateTime.UtcNow;
                
                writeCounter++;

                if (writeCounter > 0 && (writeCounter % commitThreshold) == 0)
                    ufw.CommitChanges();

                dataitems.RemoveAt(0);
            }

            ufw.CommitChanges();

            return writeCounter;
        }
        #endregion
    }
}
