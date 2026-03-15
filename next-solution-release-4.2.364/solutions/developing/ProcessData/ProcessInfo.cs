using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using DeployServer.Utils;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Management;

namespace DeployServer.Processes
{
    public class RuntimeInfo
    {
        public RuntimeInfo(string deployServerVersion)
        {
            DeployServerVersion = deployServerVersion;
            OSDescription = RuntimeInformation.OSDescription;
            FrameworkDescription = RuntimeInformation.FrameworkDescription;
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString();
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            IsOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public string DeployServerVersion { get; set; }
        public string FrameworkDescription { get; set; }
        public string OSArchitecture { get; set; }
        public string OSDescription { get; set; }
        public string ProcessArchitecture { get; set; }

        public bool IsLinux { get; set; }
        public bool IsOSX { get; set; }
        public bool IsWindows { get; set; }
    }

    public enum ProcessStatus
    {
        Stopped,
        Starting,
        Running
    }

    public class ProcessData
    {
        public String name { get; set; }
        public String fileName { get; set; }
        public String commandLine { get; set; }
        public bool autoRestart { get; set; }

        public ProcessStatus status { get; set; }
        public long physicalMemoryUsage { get; set; }
        public int basePriority { get; set; }
        public ProcessPriorityClass priorityClass { get; set; }
        public TimeSpan userProcessorTime { get; set; }
        public TimeSpan privilegedProcessorTime { get; set; }
        public TimeSpan totalProcessorTime { get; set; }
        public long pagedSystemMemorySize { get; set; }
        public long pagedMemorySize { get; set; }
        public double cpuUsage { get; set; }
        public String lastError { get; set; }
        public String processOutput { get; set; }
        public String documentTitle { get; set; }
    }

    public class ProcessInfo
    {
        public String name;
        public String fileName;
        public String commandLine;
        public String processOutput { get; set; }
        public String documentTitle { get; set; }
        public bool autoRestart;

        Process process;
        ProcessStatus status = ProcessStatus.Stopped;
        long physicalMemoryUsage;
        int basePriority;
        ProcessPriorityClass priorityClass;
        TimeSpan userProcessorTime;
        TimeSpan privilegedProcessorTime;
        TimeSpan totalProcessorTime;
        long pagedSystemMemorySize;
        long pagedMemorySize;
        double cpuUsage;
        String lastError;

        static bool bTerminate;
        static readonly int cycleTime = 1000;

        static readonly String fileSettingsName = "Processes.xml";
        static readonly String folderSettingsName = "Processes";
        static Timer CycleTimer;
        static Object lockObject = new object();
        static public int SettingsMaxLength;

        public static Dictionary<string, List<ProcessInfo>> processes = new Dictionary<string, List<ProcessInfo>>();

        class ProcessInfoComparer : IEqualityComparer<ProcessInfo>
        {
            public bool Equals(ProcessInfo x, ProcessInfo y)
            {
                return x.fileName == y.fileName && x.commandLine == y.commandLine;
            }

            public int GetHashCode(ProcessInfo obj)
            {
                return obj.fileName.GetHashCode() ^ obj.commandLine.GetHashCode();
            }
        }

        public static void LoadProcesses()
        {
            //Console.WriteLine("Waiting for debugger to attach");
            //while (!Debugger.IsAttached)
            //{
            //    Thread.Sleep(100);
            //}
            //Console.WriteLine("Debugger attached");

            try
            {
                if (Directory.Exists(folderSettingsName))
                {
                    (from projDir in Directory.GetDirectories(folderSettingsName) where File.Exists(String.Format("{0}{1}{2}", projDir, Path.DirectorySeparatorChar, fileSettingsName)) select projDir).ToList().ForEach(docTitle =>
                    {
                        var projXmlFile = String.Format("{0}{1}{2}", docTitle, Path.DirectorySeparatorChar, fileSettingsName);
                        var projProcesses = File.ReadAllText(projXmlFile).FromXml<List<ProcessInfo>>();
                        processes.Add(Path.GetFileName(docTitle), projProcesses);
                    });
                }
                var xml = File.ReadAllText(fileSettingsName);
                var uniqueGeneralProcesses = xml.FromXml<List<ProcessInfo>>().Except(processes.Values.SelectMany(x => x), new ProcessInfoComparer());
                if (uniqueGeneralProcesses.Count() > 0)
                    processes.Add(fileSettingsName, uniqueGeneralProcesses.ToList());
            }
            catch 
            {
                if (processes.Count == 0)
                    return;
            }

            var runningProcessesWin = new List<ManagementObject>();
            List<Process> runningProcesses = new List<Process>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ManagementClass mngmtClass = new ManagementClass("Win32_Process");
                runningProcessesWin = mngmtClass.GetInstances().Cast<ManagementObject>().ToList();
            }
            else
            {
                runningProcesses = Process.GetProcesses().ToList();
            }

            lock (processes)
            {
                processes.Values.SelectMany(x => x).ToList().ForEach(p =>
                {
                    try
                    {
                        bool bAlreadyRunning = false;
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            bAlreadyRunning = runningProcessesWin.Where(proc =>
                            {
                                var commandLine = proc["CommandLine"] != null ? proc["CommandLine"].ToString() : String.Empty;
                                var processNamePart = proc["Name"].ToString();
                                if (!String.IsNullOrEmpty(commandLine))
                                {
                                    try
                                    {
                                        processNamePart = System.Text.RegularExpressions.Regex.Matches(commandLine, "\".*?\"")[0].ToString();
                                    }
                                    catch { }
                                }
                                return processNamePart.Replace("\"", "") == p.fileName && commandLine.Replace(processNamePart, "").Trim() == p.commandLine;
                            }).FirstOrDefault() != null;
                        }
                        else
                        {
                            bAlreadyRunning = runningProcesses.Where(proc =>
                            {
                                bool bRet = false;
                                string processName = proc.ProcessName;
                                try
                                {
                                    string commandLine = File.ReadAllText($"/proc/{ proc.Id }/cmdline").Substring(processName.Length + 1).Replace("\0", " ").Trim();
                                    bRet = processName == p.fileName && commandLine == p.commandLine.Replace("\"", "");
                                }
                                catch { }
                                return bRet;
                            }).FirstOrDefault() != null;
                        }
                        if (!bAlreadyRunning) {

                            var processStartInfo = new ProcessStartInfo(p.fileName, p.commandLine)
                            {
                                //UseShellExecute = false,
                                RedirectStandardOutput = true
                            };
                            p.autoRestart = p.autoRestart && !CheckIsInDemo(p.fileName, p.commandLine);
                            p.process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
                            p.process.OutputDataReceived += (o, e) =>
                            {
                                var output = String.Format("{0}{1}{2}", p.processOutput, e.Data, Environment.NewLine);
                                int n = 1;
                                while (output.Length > ProcessInfo.SettingsMaxLength)
                                {
                                    var lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(n++).ToArray();
                                    output = String.Join(Environment.NewLine, lines);
                                }
                                p.processOutput = output;
                            };
                            p.process.Start();
                            p.process.BeginOutputReadLine();
                        }
                    }
                    catch(Exception e)
                    {
                        p.lastError = e.Message;
                    }
                });
            }

            StartCycleTimer();
        }

        public static List<ProcessData> GetProcessesData()
        {
            var ret = new List<ProcessData>();
            var listToProcess = new List<ProcessInfo>();
            lock (processes)
            {
                listToProcess.AddRange(processes.Values.SelectMany(x => x).Where(p => p.process != null));
            }
            foreach (var p in listToProcess)
            {
                if (bTerminate)
                    break;
                ret.Add(new ProcessData()
                {
                    name = p.name,
                    fileName = p.fileName,
                    commandLine = p.commandLine,
                    autoRestart = p.autoRestart,
                    status = p.status,
                    physicalMemoryUsage = p.physicalMemoryUsage,
                    basePriority = p.basePriority,
                    priorityClass = p.priorityClass,
                    userProcessorTime = p.userProcessorTime,
                    privilegedProcessorTime = p.privilegedProcessorTime,
                    totalProcessorTime = p.totalProcessorTime,
                    pagedSystemMemorySize = p.pagedMemorySize,
                    pagedMemorySize = p.pagedMemorySize,
                    cpuUsage = p.cpuUsage,
                    lastError = p.lastError,
                    processOutput = p.processOutput,
                    documentTitle = p.documentTitle
                });
            }

            return ret;
        }

        static void StartCycleTimer()
        {
            lock(lockObject)
            {
                if (bTerminate)
                    return;
                if (CycleTimer != null)
                    CycleTimer.Dispose();
                CycleTimer = new Timer(TimerCallback, null, cycleTime, Timeout.Infinite);
            }
        }

        public static void Terminate()
        {
            lock (lockObject)
            {
                bTerminate = true;
                if (CycleTimer == null)
                    return;
                CycleTimer.Dispose();
                CycleTimer = null;
            }

            lock (processes)
            {
                processes.Values.SelectMany(x => x).Where(p => p.process != null).ToList().ForEach(p =>
                {
                    try
                    {
                        p.process.CloseMainWindow();
                        p.process.Close();
                        p.process.Dispose();
                    }
                    catch(Exception e)
                    {
                        p.lastError = e.Message;
                    }
                });
            }
        }

        static void TimerCallback(object o)
        {
            var listToProcess = new List<ProcessInfo>();
            lock (processes)
            {
                listToProcess.AddRange(processes.Values.SelectMany(x => x).Where(p => p.process != null));
            }
            foreach(var p in listToProcess)
            {
                if (bTerminate)
                    break;
                p.UpdateProcessData();
            }

            StartCycleTimer();
        }

        async void UpdateProcessData()
        {
            try
            {
                process.Refresh();
            }
            catch (Exception e)
            {
                lastError = e.Message;
            }

            if (process.HasExited)
            {
                status = ProcessStatus.Stopped;
                if (autoRestart && status != ProcessStatus.Starting)
                {
                    try
                    {
                        process.Close();
                        process.Dispose();
                    }
                    catch { }

                    try
                    {
                        process = Process.Start(fileName, commandLine);
                    }
                    catch (Exception e)
                    {
                        lastError = e.Message;
                    }

                    status = ProcessStatus.Starting;
                }
                return;
            }

            status = ProcessStatus.Running;

            try
            {
                physicalMemoryUsage = process.WorkingSet64;
                basePriority = process.BasePriority;
                priorityClass = process.PriorityClass;
                userProcessorTime = process.UserProcessorTime;
                privilegedProcessorTime = process.PrivilegedProcessorTime;
                totalProcessorTime = process.TotalProcessorTime;
                pagedSystemMemorySize = process.PagedSystemMemorySize64;
                pagedMemorySize = process.PagedMemorySize64;
                cpuUsage = await CpuProcess.GetCpuUsageForProcess(process);
            }
            catch (Exception e)
            {
                lastError = e.Message;
            }
        }

        public static void SaveProcesses(string documentTitle)
        {
            if (!processes.ContainsKey(documentTitle))
                return;

            var bToDelete = false;
            lock (processes)
            {
                if (processes[documentTitle].Count == 0)
                {
                    processes.Remove(documentTitle);
                    bToDelete = true;
                }
            }
            if (bToDelete)
            {
                var projectDir = String.Format("{0}{1}{2}", folderSettingsName, Path.DirectorySeparatorChar, documentTitle);
                if (Directory.Exists(projectDir))
                    Directory.Delete(projectDir, true);
            }
            else
            {
                if (!Directory.Exists(folderSettingsName))
                    Directory.CreateDirectory(folderSettingsName);
                var projectProcessesDir = String.Format("{0}{1}{2}", folderSettingsName, Path.DirectorySeparatorChar, documentTitle);
                if (!Directory.Exists(projectProcessesDir))
                    Directory.CreateDirectory(projectProcessesDir);
                var projectProcessesFile = String.Format("{0}{1}{2}{1}{3}", folderSettingsName, Path.DirectorySeparatorChar, documentTitle, fileSettingsName);

                var xml = String.Empty;
                lock (processes)
                {
                    xml = processes[documentTitle].ToXml();
                }

                File.WriteAllText(projectProcessesFile, xml);
            }
        }

        public static bool AddProcess(String name, String filename, string commandline, bool autorestart, string documentTitle)
        {
            var p = new ProcessInfo()
            {
                name = name,
                fileName = filename,
                commandLine = commandline,
                autoRestart = autorestart,
                documentTitle = documentTitle
            };
            try
            {
                var processStartInfo = new ProcessStartInfo(p.fileName, p.commandLine)
                {
                    //UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                p.process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
                p.process.OutputDataReceived += (o, e) =>
                {
                    var output = String.Format("{0}{1}{2}", p.processOutput, e.Data, Environment.NewLine);
                    int n = 1;
                    while (output.Length > ProcessInfo.SettingsMaxLength)
                    {
                        var lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(n++).ToArray();
                        output = String.Join(Environment.NewLine, lines);
                    }
                    p.processOutput = output;
                };
                p.process.Start();
                p.process.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                p.lastError = e.Message;
            }
            
            lock (processes)
            {
                if (!processes.ContainsKey(documentTitle))
                    processes.Add(documentTitle, new List<ProcessInfo>());
                processes[documentTitle].Add(p);
            }
            SaveProcesses(documentTitle);

            if (CycleTimer == null)
                StartCycleTimer();

            return true;
        }

        static void StopProcess(List<ProcessInfo> listToDelete, List<string> deletedProjectProcesses, bool bForceKill)
        {
            var dirtyProjectsList = new List<string>();
            listToDelete.ForEach(p =>
            {
                if (!dirtyProjectsList.Contains(p.documentTitle))
                    dirtyProjectsList.Add(p.documentTitle);
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            p.process.Refresh();
                            if (!p.process.HasExited)
                            {
                                if (bForceKill)
                                    p.process.Kill();
                                else
                                {
                                    p.process.CloseMainWindow();
                                    p.process.Close();
                                }
                                Thread.Sleep(i * 500);
                            }
                        }
                    }

                    if (!p.process.HasExited)
                        p.process.Kill();
                }
                catch (Exception e)
                {
                    p.lastError = e.Message;
                }
            });

            dirtyProjectsList.ForEach(docTitle =>
            {
                SaveProcesses(docTitle);
            });
        }

        public static bool RemoveProcessesExcept(String documentTitle)
        {
            List<ProcessInfo> listToDelete = new List<ProcessInfo>();
            List<string> deletedProjectProcesses = new List<string>();
            lock (processes)
            {
                foreach (var docTitle in processes.Keys.ToList())
                {
                    if (docTitle != documentTitle)
                    {
                        listToDelete.AddRange(processes[docTitle]);
                        processes[docTitle].Clear();
                        if (!deletedProjectProcesses.Contains(docTitle))
                            deletedProjectProcesses.Add(docTitle);
                    }
                }
            }
            
            StopProcess(listToDelete, deletedProjectProcesses, true);

            return true;
        }

        public static bool RemoveProcess(String name, String documentTitle, bool bForceKill = false)
        {
            var listToDelete = new List<ProcessInfo>();
            List<string> deletedProjectProcesses = new List<string>();
            lock (processes)
            {
                foreach (var projName in processes.Keys)
                {
                    var l = (from c in processes[projName] where c.name == name select c).ToList();
                    if (l.Count > 0)
                    {
                        listToDelete.AddRange(l);
                        l.ForEach(p =>
                        {
                            processes[projName].Remove(p);
                        });
                        if (processes[projName].Count == 0 && !deletedProjectProcesses.Contains(projName))
                            deletedProjectProcesses.Add(projName);
                    }
                }
            }

            StopProcess(listToDelete, deletedProjectProcesses, bForceKill);

            return true;
        }

        public static bool CheckIsInDemo(string filename, string commandline)
        {
            FileVersionInfo assemblyInfo;
            if (File.Exists(filename))
                assemblyInfo = FileVersionInfo.GetVersionInfo(filename);
            else
                assemblyInfo = FileVersionInfo.GetVersionInfo(new System.Text.RegularExpressions.Regex("\"(.*?)\"").Match(commandline).Groups[1].ToString());
            var bIsInDemo = true;
            lock (lockObject)
            {
                MSZ.MSZView.SetLicensePath(new Tuple<string, string>(assemblyInfo.CompanyName, assemblyInfo.ProductName));
                bIsInDemo = MSZ.MSZView.CheckState(true);
            }
            return bIsInDemo;
        }
    }
}