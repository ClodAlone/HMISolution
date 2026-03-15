using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        readonly static Dictionary<String, String> mapPending = new Dictionary<String, String>();
        readonly static bool bEnabled;

        readonly static ILog logCompiler = LogManager.GetLogger(Properties.Resources.ScreenCompilerLogName);

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
                            String source, dest, rootBase;
                            lock (lockObject)
                            {
                                if (mapPending.Count == 0)
                                    continue;

                                source = mapPending.First().Key;
                                dest = $"{source}{Properties.Settings.Default.ScreenCompiledExtension}";
                                rootBase = mapPending.First().Value;
                                mapPending.Remove(source);
                            }

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
                                while (StartProcess(arg) != 0)
                                {
                                    if (++nRetry > Properties.Settings.Default.MaxRetryOnError)
                                        break;
                                }
                            }

                            lock (lockObject)
                            {
                                if (mapPending.Count > 0)
                                    ready.Set();
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

        static public void CompileScreen(string source, String rootBase)
        {
            if (!bEnabled || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(rootBase))
                return;

            lock (lockObject)
            {
                if (mapPending.ContainsKey(source))
                    return;
                mapPending.Add(source, rootBase);
                ready.Set();
            }

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
