using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Utilities
{
    public static class SysInfo
    {
        private static ManagementObject GetComputerSystem()
        {
            try
            {
                using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                {
                    using (ManagementObjectCollection system = objSearcher.Get())
                    {
                        foreach (ManagementObject pc in system)
                        {
                            return pc;
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the number of physical processors on the system. 
        /// </summary>
        /// <returns>The number of physical processors</returns>
        public static int GetNumberOfProcessors()
        {
            ManagementObject system = GetComputerSystem();
            if (system != null)
            {
                object num = system.Properties["NumberOfProcessors"].Value;
                if (num != null)
                    return Convert.ToInt32(num);
            }
            return 1;
        }

        /// <summary>
        /// Gets the number of logical processors on the system. 
        /// </summary>
        /// <returns>The number of logical processors</returns>
        public static int GetNumberOfLogicalProcessors()
        {
            ManagementObject system = GetComputerSystem();
            if (system != null)
            {
                object num = system.Properties["NumberOfLogicalProcessors"].Value;
                if (num != null)
                    return Convert.ToInt32(num);
            }
            return 1;
        }

        static String IndexProcessor;
        static String IndexMemory;
        static String IndexGraphics;
        static String IndexGamingGraphics;
        static String IndexHardDisk;
        static String IndexBaseScore;
        static void ReadIndexPerformance()
        {
            if (!String.IsNullOrEmpty(IndexProcessor))
                return;
            var dirName = Environment.ExpandEnvironmentVariables(@"%WinDir%\Performance\WinSAT\DataStore\");
            var dirInfo = new DirectoryInfo(dirName);
            var file = dirInfo.EnumerateFileSystemInfos("*Formal.Assessment*.xml")
                .OrderByDescending(fi => fi.LastWriteTime)
                .FirstOrDefault();

            if (file == null)
                throw new FileNotFoundException("WEI assessment xml not found");

            var doc = XDocument.Load(file.FullName);

            IndexProcessor = doc.Descendants("CpuScore").First().Value;
            IndexMemory = doc.Descendants("MemoryScore").First().Value;
            IndexGraphics = doc.Descendants("GraphicsScore").First().Value;
            IndexGamingGraphics = doc.Descendants("GamingScore").First().Value;
            IndexHardDisk = doc.Descendants("DiskScore").First().Value;
            IndexBaseScore = doc.Descendants("SystemScore").First().Value;
        }

        public static double GetIndexPerformance()
        {
            try
            {
                ReadIndexPerformance();
                return Convert.ToDouble(IndexBaseScore, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return Double.NaN;
            }
        }

        public static double GetIndexGraphicsPerformance()
        {
            try
            {
                ReadIndexPerformance();
                return Convert.ToDouble(IndexGraphics, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return Double.NaN;
            }
        }
    }
}
