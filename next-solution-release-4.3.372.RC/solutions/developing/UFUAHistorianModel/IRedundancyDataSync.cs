using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAHistorianModel
{
    public interface IRedundancyDataSync
    {
        DateTime UtcRecordingTime { get; }
        Guid RedundancyUniqueId { get; }
        DateTime RedundancySyncTime { get; set; }
    }
}
