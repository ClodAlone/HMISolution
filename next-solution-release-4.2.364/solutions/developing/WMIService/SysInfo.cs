using System;
using System.Management;
using System.Threading;

namespace WMIService
{
    public class SysInfo : IDisposable
    {
        PerfomanceInfo_CPU[] buffer;
        WMIService wmiService;
        public SysInfo()
        {
            buffer = new PerfomanceInfo_CPU[3];
            // using (WaitDialogForm dlg = new WaitDialogForm("Please Wait", "Connecting WMI Service ..."))
            {
                wmiService = WMIService.GetInstance(null);
                if (wmiService.Connected)
                {
                    string[] processors = GetProcessorNames(wmiService);
                }
            }
        }
        private PerfomanceInfo_CPU GetBufferedPerfomanceInfo(PerfomanceInfo_CPU currentValue)
        {
            for (int i = 1; i < buffer.Length; i++) buffer[i - 1] = buffer[i];
            buffer[buffer.Length - 1] = currentValue;

            float total = 0; float kernel = 0; float user = 0;
            int n = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != null)
                {
                    total += buffer[i].Total;
                    kernel += buffer[i].Kernel;
                    user += buffer[i].User;
                    n++;
                }
            }
            return new PerfomanceInfo_CPU(currentValue.Name, total / (float)n, kernel / (float)n, user / (float)n);
        }
        static PerfomanceInfo_CPU[] GetPerfomanceInfo_CPU(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects(
                    "SELECT Name,PercentProcessorTime,PercentPrivilegedTime,PercentUserTime " +
                    "FROM Win32_PerfFormattedData_PerfOS_Processor " +
                    "WHERE Name=\'_Total\'",
                    false
                );
            PerfomanceInfo_CPU[] result = new PerfomanceInfo_CPU[collection.Length];
            for (int i = 0; i < collection.Length; i++)
            {
                result[i] = new PerfomanceInfo_CPU(
                        (string)collection[i].Properties["Name"].Value,
                        (float)(UInt64)collection[i].Properties["PercentProcessorTime"].Value,
                        (float)(UInt64)collection[i].Properties["PercentPrivilegedTime"].Value,
                        (float)(UInt64)collection[i].Properties["PercentUserTime"].Value
                    );
            }
            return result;
        }
        static PerfomanceInfo_OS[] GetPerfomanceInfo_OS(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects(
                    "SELECT Name,Processes,Threads " +
                    "FROM Win32_PerfFormattedData_PerfOS_System",
                    false
                );
            PerfomanceInfo_OS[] result = new PerfomanceInfo_OS[collection.Length];
            for (int i = 0; i < collection.Length; i++)
            {
                result[i] = new PerfomanceInfo_OS(
                        (string)collection[i].Properties["Name"].Value,
                        (int)(UInt32)collection[i].Properties["Processes"].Value,
                        (int)(UInt32)collection[i].Properties["Threads"].Value
                    );
            }
            return result;
        }
        static int GetTotalMemorySizeMB(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select TotalVisibleMemorySize From Win32_OperatingSystem", true);
            return (collection.Length == 1) ? (int)((UInt64)collection[0].Properties["TotalVisibleMemorySize"].Value / 1024) : 4096;
        }
        static int GetFreeMemorySizeMB(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select FreePhysicalMemory From Win32_OperatingSystem", false);
            return (collection.Length == 1) ? (int)((UInt64)collection[0].Properties["FreePhysicalMemory"].Value / 1024) : 4096;
        }
        static string GetOSName(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select Caption From Win32_OperatingSystem", true);
            return (collection.Length == 1) ? (string)collection[0].Properties["Caption"].Value : string.Empty;
        }
        static string[] GetProcessorNames(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select Name From Win32_Processor", true);
            string[] result = new string[collection.Length];
            for (int i = 0; i < collection.Length; i++)
            {
                result[i] = (string)collection[i].Properties["Name"].Value;
            }
            return result;
        }
        static int GetTotalHDDSizeGB(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select Size From Win32_LogicalDisk ", true);
            UInt64 size = 0;
            for (int i = 0; i < collection.Length; i++)
            {
                PropertyData pData = collection[i].Properties["Size"];
                size += ((pData != null && pData.Value != null) ? (UInt64)pData.Value : 0u);
            }
            return (int)(size >> 30);
        }
        static int GetFreeHDDSizeGB(WMIService wmiService)
        {
            ManagementObject[] collection = wmiService.GetObjects("Select FreeSpace From Win32_LogicalDisk ", false);
            UInt64 size = 0;
            for (int i = 0; i < collection.Length; i++)
            {
                PropertyData pData = collection[i].Properties["FreeSpace"];
                size += ((pData != null && pData.Value != null) ? (UInt64)pData.Value : 0u);
            }
            return (int)(size >> 30);
        }

        public void Dispose()
        {
            if (wmiService != null)
            {
                wmiService.Dispose();
                wmiService = null;
            }
        }
    }
}
