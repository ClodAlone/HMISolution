using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAHistorian
{
    public struct LoggerDataInfo
    {
        public long RecordEntriesPending;
        public long RecordEntriesRunning;
        public long FailsEntriesPending;
        public long FailsEntriesRunning;
        public long DeleteEntriesPending;
        public long DeleteEntriesRunning;
        public long FlushEntriesPending;
        public long FlushEntriesRunning;
        public bool DischargingEntriesMode;
    }
}
