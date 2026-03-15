#if !NET_STANDARD
using PropertyChanged;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.Performances
{
    #if !NET_STANDARD
    [AddINotifyPropertyChangedInterface]
    #endif
    public class StatisticData : NotifyingBase
    {
        public long PendingCount { get; internal set; }
        public long UiUpdates { get; internal set; }
        public long TicksReceived { get; internal set; }
        public string TotalLatency { get; internal set; }
        public long ThreadCount { get; internal set; }
        public string ServerClientLatency { get; internal set; }
        public long UiLatency { get; internal set; }
        public string Histogram { get; internal set; }
        public string CpuTime { get; internal set; }
        public string CpuPercent { get; internal set; }
    }
}
