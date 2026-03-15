using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amib.Threading;
using DevExpress.Xpo.DB;
using log4net;
using Utilities;
using XpoHelpers;
using System.Diagnostics;
using DataLoggerModel.Helpers;

namespace RestoreDataManager
{
    public class RestoreDataHelper : IDisposable
    {
        #region Declarations
        readonly Object lockObject = new Object();

        readonly DBSchemaType schemaType;
        readonly ILog log;

        Dictionary<String, Queue<String>> mapConnections;
        Dictionary<String, DataLoggerModel.Helpers.DataLoggerInfo> mapDataLoggerInfo;

        readonly List<String> pendingConnections = new List<String>();
        readonly List<String> runningSources = new List<String>();

        volatile bool isEmpty = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of RestoreDataHelper by specifying a schema and log manager.
        /// </summary>
        /// <param name="schema">
        /// The schema type to handle with this instance of ResoreDataHelper.
        /// </param>
        /// <param name="log">
        /// The log manager to use for sending errors on restore data.
        /// </param>
        public RestoreDataHelper(DBSchemaType schema, ILog log)
        {
            this.log = log;
            schemaType = schema;

            Initialize();
        }

        /// <summary>
        /// Initialize a new instance of RestoreDataHelper by specifying a schema.
        /// </summary>
        /// <param name="schema">
        /// The schema type to handle with this instance of ResoreDataHelper.
        /// </param>
        public RestoreDataHelper(DBSchemaType schema)
        {
            schemaType = schema;

            Initialize();
        }

        void Initialize()
        {
            if (MaxRestoreProcess <= 0)
                MaxRestoreProcess = 1;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check connection points to fill in the internal restore map by using the given path, base name and extension.
        /// </summary>
        /// <param name="connection">
        /// The destination connection string for founded connection points.
        /// </param>
        /// <param name="path">
        /// The path where search must be executed.
        /// </param>
        /// <param name="baseName">
        /// The base name of the files to search.
        /// </param>
        /// <param name="extension">
        /// The extension of the files to search.
        /// </param>
        public void CheckConnectionPoint(string connection, string path, string baseName, string extension)
        {
            CheckConnectionPoint(connection, path, baseName, extension, null);
        }


        /// <summary>
        /// Check connection points to fill in the internal restore map by using the given path, base name and extension.
        /// </summary>
        /// <param name="connection">
        /// The destination connection string for founded connection points.
        /// </param>
        /// <param name="path">
        /// The path where search must be executed.
        /// </param>
        /// <param name="baseName">
        /// The base name of the files to search.
        /// </param>
        /// <param name="extension">
        /// The extension of the files to search.
        /// </param>
        /// <param name="dataloggerInfo">
        /// The datalogger info.
        /// </param>
        public void CheckConnectionPoint(string connection, string path, string baseName, string extension, DataLoggerModel.Helpers.DataLoggerInfo dataloggerInfo)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            if (dir.Exists)
            {
                var pattern = String.Format("{0}*{1}", baseName, extension);
                System.IO.FileInfo[] files = dir.GetFiles(pattern);
                foreach (System.IO.FileInfo file in files)
                {
                    if (file.Length == 0)
                    {
                        try
                        {
                            // Remove empty files.
                            System.IO.File.Delete(file.FullName);
                        }
                        catch (Exception ex)
                        {
                            if (log != null)
                                log.ErrorFormat(Properties.Resources.ErrorRemovingRestoreFile, ex.Message);
                        }
                    }
                    else
                    {
                        String source = file.FullName;
                        if (schemaType != DBSchemaType.DataLogger)
                            source = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", file.FullName));
                        AddRestorePoint(source, connection, dataloggerInfo);
                    }
                }
            }
        }

        public void AddRestorePoint(String source, String destination)
        {
            AddRestorePoint(source, destination, null);
        }

        /// <summary>
        /// Add a new restore point in the manager.
        /// </summary>
        /// <param name="source">
        /// The connection where unsaved data have been written.        
        /// </param>
        /// <param name="destination">
        /// The connection where unsaved should be wrote.
        /// </param>
        ///  <param name="dataloggerinfo">
        /// Datalogger information members used next for restoring data.
        /// </param>
        public void AddRestorePoint(String source, String destination, DataLoggerModel.Helpers.DataLoggerInfo dataloggerinfo)
        {
            System.Diagnostics.Debug.Assert(dataloggerinfo != null || schemaType != DBSchemaType.DataLogger);

            lock (lockObject)
            {
                if (mapConnections == null)
                    mapConnections = new Dictionary<String, Queue<String>>();

                if (!mapConnections.ContainsKey(destination))
                    mapConnections.Add(destination, new Queue<String>());

                if (!mapConnections[destination].Contains(source))
                    mapConnections[destination].Enqueue(source);

                if (dataloggerinfo != null)
                {
                    if (mapDataLoggerInfo == null)
                        mapDataLoggerInfo = new Dictionary<string, DataLoggerModel.Helpers.DataLoggerInfo>();

                    if (!mapDataLoggerInfo.ContainsKey(source))
                        mapDataLoggerInfo.Add(source, dataloggerinfo);
                }

                isEmpty = false;
            }
        }

        /// <summary>
        /// Start restoring a specific connection point.
        /// </summary>
        /// <param name="connection">
        /// The connection string to check in the manager.
        /// </param>
        public void StartRestoring(String connection)
        {
            lock (lockObject)
            {
                if (mapConnections == null || !mapConnections.Keys.Contains(connection))
                    return;

                var queue = mapConnections[connection];
                while (queue.Count > 0 && runningSources.Count < MaxRestoreProcess)
                {
                    var source = queue.Dequeue();

                    DataLoggerModel.Helpers.DataLoggerInfo dataloggerinfo = null;
                    if (mapDataLoggerInfo != null && mapDataLoggerInfo.ContainsKey(source))
                    {
                        dataloggerinfo = mapDataLoggerInfo[source];
                        mapDataLoggerInfo.Remove(source);
                    }

                    StartProcess(source, connection, dataloggerinfo);
                }
            }
        }

        /// <summary>
        /// Start restoring all connection points (cannot be used for datalogger).
        /// </summary>
        /// </param>
        public void StartRestoring()
        {
            lock (lockObject)
            {
                if (mapConnections == null || mapConnections.Keys.Count == 0)
                    return;

                foreach (var connection in mapConnections.Keys)
                    StartRestoring(connection);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get or Set the server running as redundancy server.
        /// </summary>
        public bool IsRedundancyServer { get; set; }

        /// <summary>
        /// Get or Set the maximum number of process to start for restoring data.
        /// </summary>
        public int MaxRestoreProcess { get; set; }

        /// <summary>
        /// Return true if the connections list to restore is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
        }
        #endregion

        #region Private Methods
        void StartProcess(String source, String destination, DataLoggerModel.Helpers.DataLoggerInfo dataloggerinfo = null)
        {
            Process restoreProcess = null;
            string tmpFilePath = null;
            try
            {
                bool error = false;
                StringBuilder arguments = new StringBuilder(string.Format("/S\"{0}\" /D\"{1}\" /O\"{2}\" /I\"{3}\"", 
                    source.Replace("\"", ""), destination.Replace("\"", ""), (int)schemaType, Process.GetCurrentProcess().Id));
                if (dataloggerinfo != null)
                {
                    tmpFilePath = System.IO.Path.GetTempFileName();
                    System.IO.File.WriteAllText(tmpFilePath, dataloggerinfo.ToXml());
                    arguments.AppendFormat(" /R\"{0}\"", tmpFilePath);
                }
                
                if (IsRedundancyServer)
                {
                    arguments.AppendFormat(" /Y");
                }

#if !NET_STANDARD
                String path = Properties.Settings.Default.RestoreToolName;
#else
                String path = Properties.Settings.Default.RestoreToolName.Replace(".exe", ".dll");
#endif
                Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                //using (var restoreProcess = new System.Diagnostics.Process())
                {
                    restoreProcess = new System.Diagnostics.Process();
                    restoreProcess.ErrorDataReceived += (o, e) =>
                    {
                        if (!error && !String.IsNullOrEmpty(e.Data))
                        {
                            error = true;
                            if (log != null)
                                log.ErrorFormat(Properties.Resources.ErrorRunningRestoreTool,
#if !NET_STANDARD
                                    Properties.Settings.Default.RestoreToolName,
#else
                                    Properties.Settings.Default.RestoreToolName.Replace(".exe", ".dll"),
#endif
                                    source, e.Data);
                        }
                    };

                    restoreProcess.OutputDataReceived += (o, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            if (log != null)
                                log.InfoFormat(Properties.Resources.RestoreToolInfoMessage,
#if !NET_STANDARD
                                    Properties.Settings.Default.RestoreToolName,
#else
                                    Properties.Settings.Default.RestoreToolName.Replace(".exe", ".dll"),
#endif
                                    source, e.Data);
                        }
                    };

                    restoreProcess.Exited += (o, e) =>
                    {
                        var exitCode = restoreProcess.ExitCode;

                        ProcessExited(restoreProcess, source, destination);

                        if (tmpFilePath != null && System.IO.File.Exists(tmpFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(tmpFilePath);
                            }
                            catch
                            { }
                        }

                        if (!bDisposed && exitCode == 0)
                        {
                            String file = source;
                            if (schemaType != DBSchemaType.DataLogger)
                            {
                                file = XpoHelper.GetDataSourceFilePath(source);
                            }
                            if (file != null)
                            {
                                try
                                {
                                    long size = 0;
                                    if (System.IO.File.Exists(file))
                                    {
                                        var fileInfo = new System.IO.FileInfo(file);
                                        size = fileInfo.Length;
                                    }

                                    if (size == 0)
                                    {
                                        System.IO.File.Delete(file);
                                    }
                                    //else if (error)
                                    //{
                                    //    System.IO.File.Move(file, string.Format("{0}.invalid", file));
                                    //}
                                    else
                                    {
                                        // The unhandled files will be check after next startup.
                                        //AddRestorePoint(source, destination);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (log != null)
                                        log.ErrorFormat(Properties.Resources.ErrorRemovingRestoreFile, ex.Message);
                                }
                            }
                        }
                    };

                    lock (lockObject)
                    {
                        if (!runningSources.Contains(source))
                            runningSources.Add(source);
                    }

#if !NET_STANDARD
                    restoreProcess.StartInfo.FileName = path;
                    restoreProcess.StartInfo.Arguments = arguments.ToString();
#else
                    restoreProcess.StartInfo.FileName = "dotnet";
                    restoreProcess.StartInfo.Arguments = String.Format("\"{0}\" {1}", path, arguments);
                    restoreProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
#endif
                    restoreProcess.StartInfo.RedirectStandardError = true;
                    restoreProcess.StartInfo.RedirectStandardOutput = true;
                    restoreProcess.StartInfo.UseShellExecute = false;
                    restoreProcess.StartInfo.CreateNoWindow = true;
                    restoreProcess.Start();
                    restoreProcess.EnableRaisingEvents = true;
                    restoreProcess.BeginErrorReadLine();
                    restoreProcess.BeginOutputReadLine();
                }
            }
            catch (Exception ex)
            {
                if (log != null)
                    log.ErrorFormat(Properties.Resources.ErrorStartingRestoreTool,
#if !NET_STANDARD
                        Properties.Settings.Default.RestoreToolName,
#else
                        Properties.Settings.Default.RestoreToolName.Replace(".exe", ".dll"),
#endif
                        source, ex.Message);

                if (restoreProcess != null)
                    ProcessExited(restoreProcess, source, destination);

                if (tmpFilePath != null && System.IO.File.Exists(tmpFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(tmpFilePath);
                    }
                    catch
                    { }
                }
            }
        }

        void ProcessExited(Process process, String source, String connection)
        {
            lock (lockObject)
            {
                process.Dispose();
                runningSources.Remove(source);

                Queue<String> queue = null;
                if (mapConnections != null && mapConnections.ContainsKey(connection))
                    queue = mapConnections[connection];

                if (queue != null && queue.Count > 0)
                {
                    StartRestoring(connection);
                    //ThreadPool.QueueUserWorkItem((s) => StartRestoring(connection));
                }
                else
                {
                    if (mapConnections != null && mapConnections.ContainsKey(connection))
                    {
                        mapConnections.Remove(connection);
                        pendingConnections.Remove(connection);
                        isEmpty = mapConnections.Keys.Count == 0;
                    }

                    if (pendingConnections.Count > 0)
                    {
                        var pending = pendingConnections[0];
                        pendingConnections.Remove(pending);
                        StartRestoring(pending);
                        //ThreadPool.QueueUserWorkItem((s) => StartRestoring(pending));
                    }
                }
            }
        }
#endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            lock (lockObject)
            {
                if (mapConnections != null)
                    mapConnections.Clear();

                if (mapDataLoggerInfo != null)
                    mapDataLoggerInfo.Clear();
            }
        }
    }
}
