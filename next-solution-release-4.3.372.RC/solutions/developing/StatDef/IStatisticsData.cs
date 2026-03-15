using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatDef
{
    public interface IStatisticsData
    {
        double Min { get; }

        double Max { get; }

        double CountUpdates { get; }

        double Average { get; }

        TimeSpan TotalTimeOn { get; }

        bool IsTotalTimeOnActive { get; }
    }
}
