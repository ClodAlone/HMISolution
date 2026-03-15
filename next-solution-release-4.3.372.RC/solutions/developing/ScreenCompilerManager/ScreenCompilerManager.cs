using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace ScreenCompilerManager
{
    static public class ScreenCompilerManager
    {
        static Thread processManager;
        readonly static Object lockObject = new Object();
        readonly static AutoResetEvent ready = new AutoResetEvent(false);
        readonly static ConcurrentDictionary<String, String> mapPending = new ConcurrentDictionary<String, String>();
        readonly static bool bEnabled;

        readonly static ILog logCompiler = LogManager.GetLogger(Properties.Resources.ScreenCompilerLogName);

        private static ConcurrentQueue<string> compiledFilesToRemove = new ConcurrentQueue<string>();

        static ScreenCompilerManager()
        {
            bEnabled = Properties.Settings.Default.ScreenCompilerEnabled && 
                System.Diagnostics.Process.GetCurrentProcess().ProcessName != System.IO.Path.GetFileNameWithoutExtension(Properties.Settings.Default.ScreenCompilerExe);
        }

        static public string GetCompiledExtension()
        {
            return Properties.Settings.Default.ScreenCompiledExtension;
        }

        static void EnsureProcessManager()
        {
            lock(lockObject)
            {
                if (processManager == null)
                {
                    processManager = new Thread((o) =>
                    {
                        while (ready.WaitOne())
                        {
                            if (mapPending.Count > 0)
                            {
                                String source, dest, rootBase, value;
                                
                                source = mapPending.First().Key;
                                dest = $"{source}{Properties.Settings.Default.ScreenCompiledExtension}";
                                rootBase = mapPending.First().Value;
                                mapPending.TryRemove(source, out value);
                                    
                                DateTime dtSource = DateTime.MinValue;
                                DateTime dtdest = DateTime.MinValue;
                                try
                                {
                                    dtSource = File.GetLastWriteTime(source);
                                }
                                catch { }
                                try
                                {
                                    dtdest = File.GetLastWriteTime(dest);
                                }
                                catch { }

                                if (dtSource > dtdest)
                                {
                                    var arg = String.Format(Properties.Settings.Default.ScreenCompilerArg, rootBase, source, dest);
                                    int nRetry = 0;
                                    if (!compiledFilesToRemove.Contains(source))
                                    {
                                        while (StartProcess(arg) != 0)
                                        {
                                            if (++nRetry > Properties.Settings.Default.MaxRetryOnError)
                                                break;
                                        }
                                    }
                                }

                                if (mapPending.Count > 0)
                                    ready.Set();                                
                            }

                            if (mapPending.Count == 0)
                            {
                                //if the queue is not empty, a process will be run to delete compiled files that must be removed
                                if (!compiledFilesToRemove.IsEmpty)
                                {
                                    string file;
                                    if (compiledFilesToRemove.TryDequeue(out file))
                                    {
                                        var arg = String.Format(Properties.Settings.Default.ScreenCompilerRemoveCompiledParameter, file);
                                        StartProcessToRemoveFiles(arg);

                                        if (!compiledFilesToRemove.IsEmpty)
                                            ready.Set();
                                    }
                                }
                            }
                        }
                    })
                    {
                        Name = "ScreenCompilerManager",
                        Priority = ThreadPriority.Lowest,
                        IsBackground = true
                    };
                    processManager.Start();
                }
            }
        }

        static int StartProcess(String arguments)
        {
            string path = Properties.Settings.Default.ScreenCompilerExe;
            System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

            if (!File.Exists(path))
                return 0;

            var startInfo = new System.Diagnostics.ProcessStartInfo(path, arguments)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = Properties.Settings.Default.ScreenCompilerTraceEnabled,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;
                process.ErrorDataReceived += (o, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            logCompiler.Error(e.Data);
                        }
                    }
                };
                process.OutputDataReceived += (o, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        logCompiler.Info(e.Data);
                    }
                };

                process.Start();
                process.BeginErrorReadLine();
                if (startInfo.RedirectStandardOutput)
                    process.BeginOutputReadLine();
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        private static void StartProcessToRemoveFiles(String arguments)
        {
            string path = Properties.Settings.Default.ScreenCompilerExe;
            System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

            var startInfo = new System.Diagnostics.ProcessStartInfo(path, arguments)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = Properties.Settings.Default.ScreenCompilerTraceEnabled,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;

                process.Start();
                process.BeginErrorReadLine();
                if (startInfo.RedirectStandardOutput)
                    process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }

        static public void CompileScreen(string source, String rootBase)
        {
            if (!bEnabled || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(rootBase))
                return;
            
            mapPending.TryAdd(source, rootBase);
            ready.Set();
            

            EnsureProcessManager();
        }

        /// <summary>
        /// Add .compiled files to a queue and wake up the thread to remove these files 
        /// </summary> 
        /// <param name="fullPath"></param>
        /// <param name="anyFile"></param>
        static public void RemoveCompiledFiles(string fullPath)
        {
            compiledFilesToRemove.Enqueue(fullPath);                
            ready.Set();

            if(processManager == null)
                EnsureProcessManager();
        }

        /// <summary>
        /// Indicates whether the screen compiler is enabled.
        /// </summary>
        /// <returns></returns>
        static public bool IsEnabled()
        {
            return bEnabled;
        }
    }
}
