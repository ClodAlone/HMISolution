using System;
using System.Threading;

namespace WMIService
{
    public class ThreadMonitorEventArgs : EventArgs
    {
        private readonly String name;
        private int cpuUsage;
        private TimeSpan uptime;

        public ThreadMonitorEventArgs(String name, int cpuUsage, TimeSpan uptime)
        {
            this.name = name;
            this.cpuUsage = cpuUsage;
            this.uptime = uptime;
        }

        public String Name
        {
            get { return name; }
        }

        public int CpuUsage
        {
            get { return cpuUsage; }
        }

        public TimeSpan Uptime
        {
            get { return uptime; }
        }
    }
}
