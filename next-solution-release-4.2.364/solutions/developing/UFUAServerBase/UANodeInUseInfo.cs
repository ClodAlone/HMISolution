using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFUAServerBase
{
    struct UANodeInUseInfo
    {
        public uint Occurrences;
        public double SamplingInterval;
        public Dictionary<int, double> SamplingIntervalCollection;
    }
}
