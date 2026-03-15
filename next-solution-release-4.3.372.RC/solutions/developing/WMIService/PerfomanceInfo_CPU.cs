using System;

namespace WMIService
{
    class PerfomanceInfo_CPU
    {
        string nameCore;
        float totalCore;
        float kernelCore;
        float userCore;
        //
        public string Name { get { return nameCore; } }
        public float Total { get { return totalCore; } }
        public float Kernel { get { return kernelCore; } }
        public float User { get { return userCore; } }
        public PerfomanceInfo_CPU(string name, float total, float kernel, float user)
        {
            nameCore = name;
            totalCore = total;
            kernelCore = kernel;
            userCore = user;
        }
    }
}
