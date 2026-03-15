using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAHistorian
{
    public class UFUAHistorianConfiguration
    {
        public String DefaultSettings;
        public String SafelySettings;
        public int RedundancyServerId;
        public int RedundancyHistoryThreadPool;
        public int RedundancyMaxSyncEntities;
        public int MaxConcurrentAccess;
        public TimeSpan ErrorTimeInterval;
        public TimeSpan defaultMaxAge;
        public byte MaxErrorBeforeFlush;
        public uint MaxErrorCacheSize;
        public int MinPendingEntities;
        public int MaxPendingEntities;
        public int MaxDeletingEntities;
        public int MaxDeleteProcess;
        public int MaxRestoreProcess;
        public long MaxTotalSafelyFilesSize;
        public bool EnableEventDataProtection;
    }
}
