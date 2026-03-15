using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    public class SynchronizeHistoryDataEvent : EventArgs
    {
        public bool IsStarting;
        public bool IsExecuted;
        public HistorySettings historySettings;
        public DateTime startTime = DateTime.MinValue;
        public DateTime endTime = DateTime.MinValue;
        public String hostName;
        public SynchronizeHistoryDataEvent()
        {
        }
    }
}
