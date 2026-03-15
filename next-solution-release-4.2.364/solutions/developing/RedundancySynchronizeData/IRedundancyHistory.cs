using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyHistory
{
    public interface IRedundancyHistory
    {
        void SynchronizeHistoryData(String sourceConn, String destinationConn, DateTime startTime, DateTime endTime);
        void StopSync();
        bool IsActiveServer { get; set; }
    }
}
