using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace WMIService
{
    // TODO: Use PerformanceCounters
    public class ThreadMonitor
    {
        private static readonly object syncRoot = new object();
        private static readonly ThreadMonitor instance;
        private List<ThreadInfo> threads;
        private Thread worker;
        public static event EventHandler<ThreadMonitorEventArgs> CalculatedCpuUsage;

        static ThreadMonitor()
        {
            instance = new ThreadMonitor();
        }

        private ThreadMonitor()
        {
            threads = new List<ThreadInfo>();
            worker = new Thread(Start);
            worker.IsBackground = true;
            worker.Start();
        }

        public static void StartMonitor(Thread t, int threadId)
        {
            if (!instance.threads.Exists(p => p.Name == t.Name))
            {
                foreach (ProcessThread processThread in Process.GetCurrentProcess().Threads)
                {
                    if (processThread.Id == threadId)
                    {
                        lock (syncRoot)
                        {
                            instance.threads.Add(new ThreadInfo(t.Name, processThread));
                        }
                    }
                }
            }
        }

        public static void StopMonitor(String name, int threadId)
        {
            lock (syncRoot)
            {
                instance.threads.RemoveAll(p => p.Name == name && p.Thread.Id== threadId);
            }
        }

        private void Start()
        {
            Process process = Process.GetCurrentProcess();
            double processCpuTime = 0;

            while (true)
            {
                lock (syncRoot)
                {
                    processCpuTime = process.TotalProcessorTime.TotalMilliseconds - processCpuTime;
                    foreach (ThreadInfo info in threads)
                    {
                        double threadNewCpuTime = info.Thread.TotalProcessorTime.TotalMilliseconds;
                        /*
                                             threadCpuTime(t2) - threadCpuTime(t1)
                         cpuUsagePercent = ----------------------------------------- x 100
                                            processCpuTime(t2) - processCpuTime(t1)
                         */
                        int cpuUsagePercent = (int)Math.Round(100 * ((threadNewCpuTime - info.OldCpuTime) / processCpuTime));
                        if (cpuUsagePercent > 100)
                            cpuUsagePercent = 100;

                        // notify observers
                        OnCalculatedCpuUsage(new ThreadMonitorEventArgs(info.Name, cpuUsagePercent, DateTime.Now.Subtract(info.Thread.StartTime)));

                        info.OldCpuTime = threadNewCpuTime;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void OnCalculatedCpuUsage(ThreadMonitorEventArgs e)
        {
            EventHandler<ThreadMonitorEventArgs> handler = CalculatedCpuUsage;
            if (handler != null)
                handler(instance, e);
        }

        class ThreadInfo
        {
            String name;
            ProcessThread thread;

            public ThreadInfo(String name, ProcessThread thread)
            {
                this.name = name;
                this.thread = thread;
                OldCpuTime = 0;
            }

            public String Name
            {
                get { return name; }
            }

            public ProcessThread Thread
            {
                get { return thread; }
            }

            public double OldCpuTime
            {
                get;
                set;
            }
        }
    }
}
