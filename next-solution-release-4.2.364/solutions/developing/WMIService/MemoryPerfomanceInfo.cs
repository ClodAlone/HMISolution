using System;

namespace WMIService
{
    class MemoryPerfomanceInfo
    {
        string nameCore;
        int totalCore;
        int freeCore;
        //
        public string Name { get { return nameCore; } }
        public int Total { get { return totalCore; } }
        public int Free { get { return freeCore; } }
        public MemoryPerfomanceInfo(string name, int total, int free)
        {
            nameCore = name;
            totalCore = total;
            freeCore = free;
        }
    }
}
