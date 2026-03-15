using System;

namespace WMIService
{
    class PerfomanceInfo_OS
    {
        string nameCore;
        int processesCore;
        int threadsCore;
        //
        public string Name { get { return nameCore; } }
        public int Processes { get { return processesCore; } }
        public int Threads { get { return threadsCore; } }
        public PerfomanceInfo_OS(string name, int processes, int threads)
        {
            nameCore = name;
            processesCore = processes;
            threadsCore = threads;
        }
    }
}
