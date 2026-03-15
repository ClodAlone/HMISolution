using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAHistorian
{
    public class StatisticDataArgs : EventArgs
    {
        public LoggerDataInfo oldData { get; set; }
        public LoggerDataInfo newData { get; set; }
    }
}
