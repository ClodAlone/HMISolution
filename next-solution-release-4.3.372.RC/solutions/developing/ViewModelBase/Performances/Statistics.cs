using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.Performances
{
    public class Statistics
    {
        public long UiLatencyMax { get; set; }
        public long ServerLatencyMax { get; set; }
        public long TotalLatencyMax { get; set; }
        public long ReceivedCount { get; set; }
        public long RenderedCount { get; set; }
        public string Histogram { get; set; }
    }
}
