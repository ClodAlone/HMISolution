using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DeployServer.Utils
{
    public static class CpuProcess
    {
        public static async Task<double> GetCpuUsageForAllProcesses()
        {
            var ret = 0.0;
            foreach(var process in Process.GetProcesses())
            {
                ret += await GetCpuUsageForProcess(process);
            }

            return ret;
        }

        public static async Task<double> GetCpuUsageForCurrentProcess()
        {
            return await GetCpuUsageForProcess(Process.GetCurrentProcess());
        }

        public static async Task<double> GetCpuUsageForProcess(Process process)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var startCpuUsage = process.TotalProcessorTime;

                await Task.Delay(500);

                var endTime = DateTime.UtcNow;
                var endCpuUsage = process.TotalProcessorTime;

                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;

                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                return cpuUsageTotal * 100;
            }
            catch
            {
                return 0;
            }
        }
    }
}
