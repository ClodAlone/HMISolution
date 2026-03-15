using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Opc.Ua.Configuration;
using Opc.Ua;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Windows;
using DevExpress.Xpo.DB.Helpers;
using System.IO;
using System.Xml;
using Utilities;
using Utilities.Logger;
using System.Reflection;
using System.Threading;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
#if !NET_STANDARD
using UFUAServerBase.ServerCSM;
#endif

namespace UFUAServerBase
{
    public partial class UFUAServer
#if !NET_STANDARD
        : System.ServiceProcess.ServiceBase
#endif
    {
        #region Declarations
        public readonly ApplicationInstance application = new ApplicationInstance();
        protected static readonly String DataSourceHeader = "data source";
        protected readonly List<string> serverListTempFile = new List<string>();
        protected static readonly int openServiceHostsMaxRetries = Properties.Settings.Default.OpenServiceHostsMaxRetries;
        protected virtual string logPath 
        { 
            get 
            {
#if !NET_STANDARD
                return Properties.Settings.Default.LogPath;
#else
                return Properties.Settings.Default.NetCoreLogPath;
#endif
            }
        }

#if !NET_STANDARD
        UFUAServerCSM serverCSM;
#endif
        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStarting;
        protected ManualResetEvent serverStopping;
        #endregion

        public UFUAServer()
        {
#if !DEBUG
            var value = Utilities.AssemblyResolver.AssemblyResolver.assemblyResolverValue; // bytes value for 'UFUAServerBase.dll'
#endif

#if !NET_STANDARD
            InitializeComponent();
#endif
            application.ApplicationType = ApplicationType.ClientAndServer;// Server;
#if !NET_STANDARD
            application.ConfigSectionName = "UFUAServer";
#else
            application.ConfigSectionName = "UFUAServer.UAServer";
#endif
        }

        internal void SetCurrentLogFileName(String projectPath)
        {
            var rootAppender = ((log4net.Repository.Hierarchy.Hierarchy)
#if !NET_STANDARD
                log4net.LogManager.GetRepository()
#else
                log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly())
#endif
                ).Root.Appenders.OfType<log4net.Appender.FileAppender>().FirstOrDefault();
            if (rootAppender == null)
                return;

            var logFile = String.Format("{0}\\{1}", projectPath, logPath);
#if !NET_STANDARD
            rootAppender.File = logFile;
#else
            rootAppender.File = logFile.Replace('\\', Path.DirectorySeparatorChar);
#endif
            rootAppender.ActivateOptions();
        }

#region Protected Methods
        protected virtual string GetConnectionString(Utility.CommandArgs commandArgs)
        {
            if (commandArgs.ArgPairs.ContainsKey("XmlFile"))
                return InMemoryDataStore.GetConnectionString(commandArgs.ArgPairs["XmlFile"]);
#if !NET_STANDARD
            else if (commandArgs.ArgPairs.ContainsKey("AccessFile"))
                return AccessConnectionProvider.GetConnectionString(commandArgs.ArgPairs["AccessFile"]);
#endif
            else if (commandArgs.ArgPairs.ContainsKey("SQLDatabase"))
            {
                if (commandArgs.ArgPairs.ContainsKey("SQLServer"))
                    return MSSqlConnectionProvider.GetConnectionString(commandArgs.ArgPairs["SQLServer"],
                        commandArgs.ArgPairs["SQLDatabase"]);
                else
                    return MSSqlConnectionProvider.GetConnectionString("(local)", commandArgs.ArgPairs["SQLDatabase"]);
            }
            else if (commandArgs.ArgPairs.ContainsKey("conn"))
                return commandArgs.ArgPairs["conn"];
            else
                return String.Empty;
        }
        #endregion


#region Virtual Methods
#if !NET_STANDARD
        public virtual void StartService(string[] args)
#else
        public virtual async Task StartService(string[] args)
#endif
        {
            OnStartingService(new EventArgs());

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

            lock (lockObject)
            {
                if (serverStopping == null)
                    serverStopping = new ManualResetEvent(false);
            }

            var appName = Assembly.GetEntryAssembly().GetName().Name;

            var conn = string.Empty;
            var uniqueStartupName = string.Empty;

            try
            {
                conn = GetConnectionString(commandArgs);

                if (String.IsNullOrEmpty(conn))
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        Properties.Resources.InvalidOptions,
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

#if !NET_STANDARD
                        if (Environment.UserInteractive)
                        {
                            MessageBox.Show(Properties.Resources.InvalidOptions, appName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
#endif

                    ExitAndRemoveTemporaryFiles(-10);
                }
                else
                {
                    uniqueStartupName = XpoHelpers.XpoHelper.GetUFUAServerUniqueStartupName(appName, conn);
                }
            }
            catch (AbortUFUAServerException)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    Properties.Resources.AbortStartingMessage,
                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
            }

            using (var Mutex = new Mutex(false, uniqueStartupName))
            {
                try
                {
                    while (!Mutex.WaitOne(1000))
                    {
                        if (serverStopping.WaitOne(1000))
                            return;

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            Properties.Resources.ProjectAlreadyRunning,
                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                    }
                }
                catch (AbandonedMutexException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                }

                try
                {
                    String xmlFile = null;
                    String originalxmlFile = null;

                    if (commandArgs.ArgPairs.ContainsKey("suspend"))
                        MSZ.MSZView.Suspend();

                    ProjectConnectionString = conn.Replace('/', '\\');
                    
                    try
                    {
                        var helper = new ConnectionStringParser(conn);
                        string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                        if (providerType == InMemoryDataStore.XpoProviderTypeString)
                        {
                            originalxmlFile = xmlFile = helper.GetPartByName(DataSourceHeader);
#if NET_STANDARD
                            String fileCore = xmlFile + UFUAServerInfo.UFUAServerInfo.GetServerCoreExtension();
                            try
                            {
                                if (File.Exists(fileCore))
                                {
                                    FileInfo infoFileCore = new FileInfo(fileCore);
                                    FileInfo infoXmlFile = new FileInfo(xmlFile);
                                    if (infoFileCore.LastWriteTime > infoXmlFile.LastWriteTime)
                                        xmlFile = fileCore;
                                }
                            }
                            catch { }
#endif
                            var tempFile = System.IO.Path.GetTempFileName();
                            System.IO.File.Copy(xmlFile, tempFile, true);
                            helper.UpdatePartByName(DataSourceHeader, tempFile);
                            conn = helper.GetConnectionString();
                            xmlFile = tempFile;
                            serverListTempFile.Add(tempFile);
                        }
                    }
                    catch (Exception ex)
                    { }

#if DEBUG
                    //conn = "XpoProvider=InMemoryDataStore;data source=C:/ProgramData/Progea/MyProject1/UFUAServer/Server.UFUAServer.Server;";
                    //conn = MSSqlConnectionProvider.GetConnectionString("(local)", "TestUFUAServer");
#endif

                    var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                    SetDataStoreSchema(dict);
                    dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

                    conn = conn.Replace('/', '\\');
                    if (String.IsNullOrEmpty(xmlFile))
                    {
                        try
                        {
                            var store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                            DefaultDataLayer = new ThreadSafeDataLayer(dict, store);
                        }
                        catch (Exception ex)
                        {
                            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource, message,
                                System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);

#if !NET_STANDARD
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show(message, appName, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
#endif

                            ExitAndRemoveTemporaryFiles(-10);
                        }
                    }
                    else
                    {
                        var path = System.IO.Path.GetDirectoryName(originalxmlFile);
                        var splits = path.Split('\\', '/');
                        if (splits.Length > 0)
                        {
                            var logPath = splits[0];
                            for (int i = 1; i < splits.Length - 2; ++i)
                                logPath = String.Format("{0}\\{1}", logPath, splits[i]);
                            SetCurrentLogFileName(logPath);

                        }
                        var InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        if (System.IO.File.Exists(xmlFile))
                        {
                            if (!Utilities.IO.FileSystem.IsXmlFile(xmlFile))
                            {
                                var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(xmlFile));
                                using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                                {
                                    var xmlreader = XmlReader.Create(reader);
                                    try
                                    {
                                        InMemory.ReadXml(xmlreader);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    InMemory.ReadXml(xmlFile);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        else
                        {
                            var missingProjectFile = String.Format(Properties.Resources.MissingProjectFile, xmlFile);
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource, missingProjectFile,
                                System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);

#if !NET_STANDARD
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show(missingProjectFile, appName, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
#endif

                            ExitAndRemoveTemporaryFiles(-10);
                        }

                        DefaultDataLayer = new ThreadSafeDataLayer(dict, InMemory);
                    }

#if DEBUG
                    //using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    //{
                    //    var configurations = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(ufw).AsParallel() select tag).ToList();
                    //    foreach (var conf in configurations)
                    //    conf.Delete();
                    //    ufw.CommitChanges();
                    //}

                    //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    //{
                    //    var conf = new UFUAModel.UFUAConfiguration(ufw);
                    //    conf.ComunicationDrivers.Add(new UFUAModel.UFUACommunicationDriver(ufw) { Name = "Modbus", Path = "E:\\PRIVATE\\12-0-Drivers\\ModBus\\bin\\Debug" });
                    //    ufw.CommitChanges();
                    //}

                    //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    //{
                    //    var prototype = new UFUAModel.UFUATagPrototype(ufw);
                    //    prototype.Name = "ObjectPrototype";
                    //    prototype.NodeId = Guid.NewGuid();
                    //    prototype.Members.Add(new UFUAModel.UFUATag(ufw) { Name = "Member1", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });
                    //    prototype.Members.Add(new UFUAModel.UFUATag(ufw) { Name = "Member2", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });

                    //    var folder1 = new UFUAModel.UFUAFolder(ufw);
                    //    folder1.Name = "FolderObject1";
                    //    folder1.NodeId = Guid.NewGuid();
                    //    folder1.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "MyObject", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.ObjectType, PrototypeModel = prototype });
                    //    folder1.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag1", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });
                    //    folder1.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag2", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });

                    //    var folder2 = new UFUAModel.UFUAFolder(ufw);
                    //    folder2.Name = "ChildFolderObject2";
                    //    folder2.NodeId = Guid.NewGuid();
                    //    folder2.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag1", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });
                    //    folder2.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag2", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });

                    //    folder1.UFUAFolders.Add(folder2);

                    //    var folder3 = new UFUAModel.UFUAFolder(ufw);
                    //    folder3.Name = "FolderObject3";
                    //    folder3.NodeId = Guid.NewGuid();
                    //    folder3.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag1", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });
                    //    folder3.UFUATags.Add(new UFUAModel.UFUATag(ufw) { Name = "Tag2", NodeId = Guid.NewGuid(), ModelType = UFUAModel.ModelType.Analog });

                    //    ufw.CommitChanges();
                    //}

                    //using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    //{
                    //    var dataloggers = (from tag in new XPQuery<DataLoggerModel.DataLoggerSettings>(ufw).AsParallel() select tag).ToList();
                    //    foreach (var dl in dataloggers)
                    //        dl.Delete();

                    //    var tags = (from tag in new XPQuery<UFUAModel.UFUATag>(ufw).AsParallel() orderby tag.Oid select tag).ToList();
                    //    if (tags.Count >= 2)
                    //    {
                    //        var datalogger = new DataLoggerModel.DataLoggerSettings(ufw)
                    //        {
                    //            Name = "TestDataLogger",
                    //            TableName = "MyTable",
                    //            MaxAge = TimeSpan.FromMinutes(1),
                    //            MaxLength = 255,
                    //            Enable = true,
                    //            RecordingTag = tags[0],
                    //            WaitBeforeRetry = 10,
                    //            FlushDataSafelyPath = @"C:\Temp\TestFlushing\"
                    //        };

                    //        if (tags.Count >= 3)
                    //            datalogger.ResettingTag = tags[2];

                    //        datalogger.Columns.Add(new DataLoggerModel.DataLoggerColumn(ufw)
                    //        {
                    //            ColumnName = "Column1",
                    //            DataLoggerReference = datalogger,
                    //            UFUATagReference = tags[1],
                    //            AddServerTimeStampColumn = true,
                    //            AddSourceTimeStampColumn = true,
                    //            AddStatusCodeColumn = true,
                    //            AddUserColumn = true
                    //        });
                    //    }

                    //    ufw.CommitChanges();
                    //}
#endif
#if !NET_STANDARD
                    GetApplicationConfiguration(appName);
#else
                    await GetApplicationConfiguration();
#endif

                    string arguments = string.Empty;
                    if (commandArgs.ArgPairs.ContainsKey("CCheck"))
                        arguments = commandArgs.ArgPairs["CCheck"];

                    // check the application certificate.
                    try
                    {
#if !NET_STANDARD
                        application.CheckApplicationInstanceCertificate(!Environment.UserInteractive, 0);
#else
                        await application.CheckApplicationInstanceCertificate(!Environment.UserInteractive, 0);
#endif
                    }
                    catch (Exception e)
                    {
                        // run the tool for renewing the application certificate
#if !NET_STANDARD
                        if (!Environment.UserInteractive || MessageBox.Show(Properties.Resources.AskForCertificateChecker, Properties.Resources.AskForCertificateCheckerCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
#endif
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                String.Format(Properties.Resources.MessageOnRenewCertificate, e.Message),
                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
#if !NET_STANDARD
                            string path = "CertificateChecker.exe";
                            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                            if (callingMainAssembly != null)
                                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
                            arguments = String.Format("{0} \"/A{1}\" \"/N\"", arguments, application.ApplicationConfiguration.ApplicationName);

                            try
                            {
                                Process p = Process.Start(path, arguments);
                                p.WaitForExit();
                                //p.ExitCode
                                p.Close();
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.ErrorOnRenewCertificate, ex.Message),
                                    System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                            }
#endif
                        }
#if !NET_STANDARD
                        else
                        {
#endif
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                Properties.Resources.InvalidCertificateTermination,
                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                            ExitAndRemoveTemporaryFiles(-1);
#if !NET_STANDARD
                        }
#endif
                    }

                    PreInitializeServerApplication();

                    if (Environment.UserInteractive && !application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                    {
                        application.ApplicationConfiguration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
                    }
                    /*application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.ForEach(s =>
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    String.Format("Listening at {0}", s),
                                                    System.Diagnostics.EventLogEntryType.Information);
                        });*/

                    //{
                    //    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    //                            String.Format("Listening at {0}", s),
                    //                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                    //    StopService();
                    //    return;
                    //}

                    application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.ForEach(s =>
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                String.Format(Properties.Resources.EndpointListeningMessage, s),
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                    });

                    /*
                    if (application.ApplicationConfiguration.ServerConfiguration.DiagnosticsEnabled)
                        System.Diagnostics.Trace.Listeners.Add(new Log4netTraceListener(log4net.LogManager.GetLogger(Properties.Resources.DiagnosticsLogName)));
                    */

                    // start the server.
                    //application.Start(new UAServer());
#if !NET_STANDARD
                    StartServerApplication();
                    DelayedStartScripts();
#else
                    await StartServerApplication();
#endif
                    DelayedStartDrivers();
                }
                catch (AbortUFUAServerException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        Properties.Resources.AbortStartingMessage,
                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
#if !NET_STANDARD
            // LastMessage = String.Format("{0} - Untrusted Certificate : {0}", Title, e.Certificate.Subject);
            StringBuilder buffer = new StringBuilder();

            buffer.AppendFormat("Certificate could not validated: {0}\r\n\r\n", e.Error.StatusCode);
            buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
            buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
            buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
            buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
            buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);

            buffer.AppendFormat("Accept anyways?");

            e.Accept = application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates;
            var appName = Assembly.GetCallingAssembly().GetName().Name;
            if (Environment.UserInteractive && 
                !application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates &&
                MessageBox.Show(buffer.ToString(), appName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Accept = true;
            }
#endif
        }

#if !NET_STANDARD
        private void GetApplicationConfiguration(string appName) 
#else
        private async Task GetApplicationConfiguration() 
#endif
        {
            string configFilePath = ApplicationConfiguration.GetFilePathFromAppConfig(application.ConfigSectionName);
            var configFilePathMutexString = configFilePath.Replace("/", ".").Replace("\\", ".").Replace(":", ".");

            using (var configFileMutex = new Mutex(false, configFilePathMutexString))
            {
                configFileMutex.WaitOne();

                try
                {
                    // load the application configuration.
#if !NET_STANDARD
                        application.LoadApplicationConfiguration(!Environment.UserInteractive);
#else
                    await application.LoadApplicationConfiguration(!Environment.UserInteractive);
#endif
                }
                catch (ServiceResultException)
                {
                    // save the missing application configuration file.
                    var attr = Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(),
                        typeof(DefaultNamespaceAttribute)) as DefaultNamespaceAttribute;
                    if (attr != null)
                    {
                        configFilePath = ApplicationConfiguration.GetFilePathFromAppConfig(application.ConfigSectionName);
                        string fileName = System.IO.Path.GetFileName(configFilePath);
                        string tmpFile = System.IO.Path.GetTempFileName();
                        serverListTempFile.Add(tmpFile);
                        try
                        {
                            ExtractFileFromResource.Extract(Assembly.GetEntryAssembly(), tmpFile, string.Format("{0}.{1}", attr.DefaultNamespace, fileName));
                            application.LoadApplicationConfiguration(tmpFile, true);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed to load configuration file : {0}", ex);
                        }
                    }

                    if (application.ApplicationConfiguration == null)
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.MissingDefaultConfigurationFile,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

#if !NET_STANDARD
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show(Properties.Resources.MissingDefaultConfigurationFile, appName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
#endif

                        ExitAndRemoveTemporaryFiles(-10);
                    }
                }

                configFileMutex.ReleaseMutex();
            }
        }

        void ExitAndRemoveTemporaryFiles(int exitCode)
        {
            serverListTempFile.ForEach(file =>
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                { }
            });

            System.Environment.Exit(exitCode);
        }

        protected void UFUAServer_BalloonEvent(object sender, BalloonEventArgs e)
        {
            if(e != null)
            {
                BalloonMessage = e.Message;
                BalloonIcon = e.Icon;
            }
        }

        protected void UFUAServer_AlertEvent(object sender, String error)
        {
            AlertMessage = error;
        }

#if !NET_STANDARD
        public virtual void StartServerApplication()
#else
        public virtual async Task StartServerApplication()
#endif
        {
            var uaserver = new UAServer() { ActiveConnectionString = ProjectConnectionString };
            uaserver.BalloonEvent += UFUAServer_BalloonEvent;
            uaserver.AlertEvent += UFUAServer_AlertEvent;
#if !NET_STANDARD
            application.Start(uaserver);
#else
            await application.Start(uaserver);
#endif
        }

        protected virtual void SetDataStoreSchema(DevExpress.Xpo.Metadata.ReflectionDictionary dict)
        {
            dict.GetDataStoreSchema(typeof(UFUAModel.UFUATag).Assembly, typeof(DataLoggerModel.DataLoggerSettings).Assembly);
        }

        protected virtual void PreInitializeServerApplication()
        {
            using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var configurations = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(ufw).AsParallel() select tag).ToList();
                if (configurations.Count == 0)
                    configurations.Add(new UFUAModel.UFUAConfiguration(ufw));

                if (configurations.Count > 0)
                {
                    if (configurations[0].MaxSessionTimeout > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSessionTimeout = configurations[0].MaxSessionTimeout;
                    if (configurations[0].MinSessionTimeout > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinSessionTimeout = configurations[0].MinSessionTimeout;

                    application.ApplicationConfiguration.ServerConfiguration.DiagnosticsEnabled = configurations[0].DiagnosticEnabled;
                    if (configurations[0].MaxSessionCount > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSessionCount = configurations[0].MaxSessionCount;
                    if (configurations[0].MaxBrowseContinuationPoints > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxBrowseContinuationPoints = configurations[0].MaxBrowseContinuationPoints;
                    if (configurations[0].MaxHistoryContinuationPoints > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxHistoryContinuationPoints = configurations[0].MaxHistoryContinuationPoints;
                    if (configurations[0].MaxRequestAge > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxRequestAge = configurations[0].MaxRequestAge;
                    if (configurations[0].MinPublishingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinPublishingInterval = configurations[0].MinPublishingInterval;
                    if (configurations[0].MaxPublishingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxPublishingInterval = configurations[0].MaxPublishingInterval;
                    if (configurations[0].PublishingResolution > 0)
                        application.ApplicationConfiguration.ServerConfiguration.PublishingResolution = configurations[0].PublishingResolution;
                    if (configurations[0].MaxSubscriptionLifetime > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSubscriptionLifetime = configurations[0].MaxSubscriptionLifetime;
                    if (configurations[0].MaxMessageQueueSize > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxMessageQueueSize = configurations[0].MaxMessageQueueSize;
                    if (configurations[0].MaxNotificationQueueSize > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxNotificationQueueSize = configurations[0].MaxNotificationQueueSize;
                    if (configurations[0].MaxNotificationsPerPublish > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxNotificationsPerPublish = configurations[0].MaxNotificationsPerPublish;
                    if (configurations[0].MinMetadataSamplingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinMetadataSamplingInterval = configurations[0].MinMetadataSamplingInterval;

                    if (configurations[0].BaseAddresses.Count == 0 && application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Count == 0)
                        configurations[0].EnsureDefaultBaseAddresses(ufw);

                    if (configurations[0].BaseAddresses.Count > 0)
                    {
                        application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Clear();
                        
                        var allocatedTransports = new List<String>();
                        foreach (var ba in configurations[0].BaseAddresses)
                        {
#if NET_STANDARD
                            if (ba.Transport != Utils.UriSchemeOpcTcp && ba.Transport != Utils.UriSchemeHttps)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.NotSupportedProtocol, ba.Transport),
                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                continue;
                            }
#endif
                            if (ba.Enabled)
                            {
                                if (ba.Transport != Opc.Ua.Utils.UriSchemeOpcTcp)
                                {
                                    if (allocatedTransports.Contains(ba.Transport))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(UFUAServerBase.Properties.Resources.DuplicatedProtocol, ba.Transport),
                                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                        continue;
                                    }
                                    else
                                        allocatedTransports.Add(ba.Transport);
                                }

                                application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Add(Utils.ReplaceLocalhost(ba.Path));
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(configurations[0].ApplicationName))
                    {
                        application.ApplicationName = configurations[0].ApplicationName;
                        if (!application.ApplicationConfiguration.ApplicationUri.Contains(String.Format(":{0}", configurations[0].ApplicationName)))
                        {
                            var builder = new StringBuilder(application.ApplicationConfiguration.ApplicationUri);
                            builder.AppendFormat(":{0}", configurations[0].ApplicationName);
                            application.ApplicationConfiguration.ApplicationUri = builder.ToString();
                        }

                        if (application.ApplicationConfiguration.TraceConfiguration != null)
                        {
                            var filename = Path.GetFileName(application.ApplicationConfiguration.TraceConfiguration.OutputFilePath);
                            application.ApplicationConfiguration.TraceConfiguration.OutputFilePath =
                                application.ApplicationConfiguration.TraceConfiguration.OutputFilePath.Replace(filename, String.Format("{0}.log", configurations[0].ApplicationName));
                        }
                    }

                    application.ApplicationConfiguration.ProductUri = configurations[0].ProductUri;

#if !NET_STANDARD
                    bool useDiscovery = true;
                    int delay = 0;
                    int retries = 0;
                    while (true)
                    {
                        if (serverStopping.WaitOne(delay))
                            return;

                        try
                        {
                            serverCSM = new UFUAServerCSM(configurations[0].ConfigurationId.ToString(), this);
                            serverCSM.HostServer(useDiscovery);
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (serverCSM != null)
                                Utils.SilentDispose(serverCSM);

                            if (ex is AddressAlreadyInUseException ||
                                ex is System.Net.Sockets.SocketException ||
                                ex is CommunicationException)
                            {
                                if (useDiscovery && !(ex is AddressAlreadyInUseException))
                                    useDiscovery = false;

                                if (++retries > openServiceHostsMaxRetries)
                                    retries = 1;

                                delay = Math.Max(retries * 1000, delay);
                                var str = String.Format(Properties.Resources.RetryOpenSocketWarningMessage, retries, openServiceHostsMaxRetries, delay);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                        str,
                                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);

                                var thisserver = application.Server as UAServer;
                                if (thisserver != null)
                                    thisserver.OnBalloonEvent(str, System.Windows.Forms.ToolTipIcon.Warning);

                                if ((retries % openServiceHostsMaxRetries) == 0)
                                {
                                    if (ex is System.ServiceModel.AddressAlreadyInUseException)
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ProjectAlreadyRunning,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                communicationErrorMessage,
                                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                                    }
                                }
                            }
                            else
                                throw;
                        }
                    }

                    if (!useDiscovery)
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.UnableToUseDiscoveryBehaviour,
                                                System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }
#endif
                }
            }
        }

#endregion

#region Override Methods
#if !NET_STANDARD
        protected override void OnStart(string[] args)
        {
            lock (lockObject)
            {
                if (serverThread == null)
                {
                    serverStarting = new ManualResetEvent(false);
                    serverStopping = new ManualResetEvent(false);
                    serverThread = new Thread(() =>
                    {
                        if (args.Length == 0)
                            args = Environment.GetCommandLineArgs();

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        Properties.Resources.ServiceStarting,
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                        StartService(args);

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ServiceStarted,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                        IsRunningAsService = true;
                        serverStarting.Set();

                        if (serverStopping.WaitOne())
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            Properties.Resources.ServiceStopping,
                                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                            StopService();

                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ServiceStopped,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                            IsRunningAsService = false;
                        }
                    });
                    serverThread.Name = ServiceName;
                    serverThread.IsBackground = true;
                }
                serverThread.Start();
            }

            while (!serverStarting.WaitOne(2000))
            {
                RequestAdditionalTime(5000);
            }
        }

        protected override void OnStop()
        {
            Thread thread = null;
            lock (lockObject)
            {
                thread = serverThread;
                if (serverStopping != null)
                    serverStopping.Set();
            }

            if (thread != null)
            {
                while (thread.IsAlive && !thread.Join(2000))
                {
                    RequestAdditionalTime(5000);
                }
            }

            lock (lockObject)
            {
                serverThread = null;
                if (serverStarting != null)
                {
                    serverStarting.Dispose();
                    serverStarting = null;
                }
                if (serverStopping != null)
                {
                    serverStopping.Dispose();
                    serverStopping = null;
                }
            }
        }
#endif
#endregion

#region Events

        public event EventHandler StartingService;
#region OnStartingService
        /// <summary>
        /// Triggers the StartingService event.
        /// </summary>
        public virtual void OnStartingService(EventArgs ea)
        {
            var t = StartingService;
            if (t != null)
                t(this, ea);
        }

#endregion

        public event EventHandler StoppingService;
#region OnStoppingService
        /// <summary>
        /// Triggers the StoppingService event.
        /// </summary>
        public virtual void OnStoppingService(EventArgs ea)
        {
            var t = StoppingService;
            if (t != null)
                t(this, ea);
        }

#endregion
#endregion

#region Methods
#if !NET_STANDARD
        public void DelayedStartScripts()
        {
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                Properties.Resources.StartingScripts,
                                                Properties.Resources.ScriptsStarted))
            {
                var server = application.Server as UAServer;
                server.UANodeManager.StartScripts();
            }
        }
#endif

        public void DelayedStartDrivers()
        {
            var thisserver = application.Server as UAServer;
            if(thisserver != null)
            {
#if !NET_STANDARD
                if (!thisserver.IsRedundancyEnabled || thisserver.ActiveServerManager.IsActiveServer)
#endif
                {
                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                    Properties.Resources.StartingDrivers,
                                                    Properties.Resources.StartedDrivers))
                    {
                        foreach (var drv in thisserver.CommDrivers.Values)
                        {
                            if (!thisserver.CommDriversFaulted.ContainsKey(drv.GetDriverName()) &&
                                !thisserver.CommDriversDisabled.ContainsKey(drv.GetDriverName()))
                            {
                                drv.Startup();
                            }
                        }
                    }

                    if (thisserver.CommDriversFaulted.Count > 0 || thisserver.CommDriversDisabled.Count > 0)
                    {
                        thisserver.OnBalloonEvent(Properties.Resources.WarningNotAllDriverStarted, System.Windows.Forms.ToolTipIcon.Info);
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            Properties.Resources.WarningNotAllDriverStarted,
                                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                    }
                }

#if !NET_STANDARD
                if (thisserver.IsRedundancyEnabled)
                {
                    thisserver.ActiveServerManager.ActiveServer += (o, e) =>
                    {
                        lock (lockObject)
                        {
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.StartingDrivers,
                                                        Properties.Resources.StartedDrivers))
                            {
                                foreach (var drv in thisserver.CommDrivers.Values)
                                {
                                    if (!thisserver.CommDriversFaulted.ContainsKey(drv.GetDriverName()) &&
                                        !thisserver.CommDriversDisabled.ContainsKey(drv.GetDriverName()))
                                    {
                                        drv.Startup();
                                    }
                                }
                            }

                            if (thisserver.CommDriversFaulted.Count > 0 || thisserver.CommDriversDisabled.Count > 0)
                            {
                                thisserver.OnBalloonEvent(Properties.Resources.WarningNotAllDriverStarted, System.Windows.Forms.ToolTipIcon.Info);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.WarningNotAllDriverStarted,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                            }
                        }
                    };

                    thisserver.ActiveServerManager.InactiveServer += (o, e) =>
                    {
                        lock (lockObject)
                        {
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.SuspendingDrivers,
                                                        Properties.Resources.SuspendedDrivers))
                            {
                                foreach (var drv in thisserver.CommDrivers.Values)
                                {
                                    if (!thisserver.CommDriversFaulted.ContainsKey(drv.GetDriverName()) &&
                                        !thisserver.CommDriversDisabled.ContainsKey(drv.GetDriverName()))
                                    {
                                        drv.Suspend();
                                    }
                                }
                            }
                        }
                    };
                }
#endif
            }
        }

        public void StopService()
        {
            if (!IsStarted)
                return;

            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                            Properties.Resources.StoppingServer,
                                            Properties.Resources.StoppedServer))
            {
                lock (lockObject)
                {
                    if (serverStopping != null)
                        serverStopping.Set();
                }

                OnStoppingService(new EventArgs());

                application.Stop();
#if !NET_STANDARD
                if (serverCSM != null)
                {
                    serverCSM.Dispose();
                    serverCSM = null;
                }
#endif
                if (DefaultDataLayer != null)
                {
                    DefaultDataLayer.Dispose();
                    DefaultDataLayer = null;
                }

                serverListTempFile.ForEach(file =>
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch
                    { }

                });
            }
        }

#endregion

#region Properties

        public bool IsStarted
        {
            get
            {
                try
                {
                    var thisserver = application.Server as UAServer;
                    return thisserver != null && thisserver.IsStarted;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool IsActiveServer
        {
            get
            {
#if !NET_STANDARD
                try
                {
                    var thisserver = application.Server as UAServer;
                    return thisserver != null && 
                        ((thisserver.ActiveServerManager != null && thisserver.ActiveServerManager.IsActiveServer) || 
                        (!thisserver.IsRedundancyEnabled));
                }
                catch (Exception ex)
                {
                    return false;
                }
#else
                return true;
#endif
            }
        }


        private bool _IsRunningAsService;
        public bool IsRunningAsService
        {
            get { return _IsRunningAsService; }
            internal set
            {
                _IsRunningAsService = value;
            }
        }

        public static IDataLayer DefaultDataLayer { get; private set; }

        protected string ProjectConnectionString { get; private set; }

        public String Title
        {
            get
            {
                return application.ApplicationName;
            }
        }

        public bool CommunicationStatus
        {
            get
            {
                try
                {
                    var server = application.Server as UAServer;
                    if (server == null)
                        return true;
                    return server.GetServerState() == ServerState.Running;
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }

        public String StatusText
        {
            get
            {
                try
                {
                    var server = application.Server as UAServer;
                    if (server == null)
                        return ServerState.Unknown.ToString();
                    return server.GetServerState().ToString();
                }
                catch (Exception ex)
                {
                    return ServerState.Unknown.ToString();
                }
            }
        }
        String _BalloonMessage = String.Empty;
        public String BalloonMessage
        {
            get { return _BalloonMessage;}
            set { _BalloonMessage = value; }
        }
        System.Windows.Forms.ToolTipIcon _BalloonIcon = System.Windows.Forms.ToolTipIcon.None;
        public System.Windows.Forms.ToolTipIcon BalloonIcon
        {
            get { return _BalloonIcon; }
            set { _BalloonIcon = value; }
        }

        String _AlertMessage = String.Empty;
        public String AlertMessage
        {
            get { return _AlertMessage; }
            set { _AlertMessage = value; }
        }
#endregion

        public void SetSecondsAndReason(uint i, string p)
        {
            var server = application.Server as UAServer;
            if (server != null)
                server.SetSecondsAndReason(i, p);
        }

        bool bUpdatingServiceLevel;
        public void UpdateServiceLevelAsync()
        {
            if (!bUpdatingServiceLevel)
            {
                bUpdatingServiceLevel = true;
                ThreadPool.QueueUserWorkItem((s) =>
                {
                    try
                    {
                        UpdateServiceLevel();
                    }
                    catch
                    { }
                    finally
                    {
                        bUpdatingServiceLevel = false;
                    }
                });
            }
        }

        public void UpdateServiceLevel()
        {
            var server = application.Server as UAServer;
            if (server != null)
            {
                server.UpdateServiceLevel(GetCpuUsage());
            }
        }
        

        #region CPU Usage
        byte GetCpuUsage()
        {
            byte serviceLevelBase = 200;
            var server = application.Server as UAServer;
            if (server != null)
            {
                var level = server.GetServerState();
                switch(level)
                {
                    case ServerState.Running: break;
                    case ServerState.CommunicationFault:
                        serviceLevelBase = 100;
                        break;
                    default: return 1;
                }
            }
            else 
                return 0;

            int cpupercentfree = 100;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var cpuCounter = new PerformanceCounter())
                {
                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";
                    // var ramCounter = new PerformanceCounter("Memory", "Available MBytes");

                    var unused = cpuCounter.NextValue(); // first call will always return 0
                    System.Threading.Thread.Sleep(100); // wait a second, then try again
                                                        // Console.WriteLine("Cpu usage: " + cpuCounter.NextValue() + "%");
                                                        // Console.WriteLine("Free ram : " + ramCounter.NextValue() + "MB");
                    cpupercentfree = 100 - (int)(cpuCounter.NextValue());
                }
            }
            return (byte)(55 * (cpupercentfree / 100.0) + serviceLevelBase);
        }
#endregion

#if !NET_STANDARD
#region Script Remote Debugging

        internal bool EnableScriptDebugging(Guid id, bool bEnable)
        {
            var server = application.Server as UAServer;
            return server.UANodeManager.EnableScriptRemoteDebugging(id, bEnable);
        }

        internal List<string> ScriptSynchronizing(Guid id, List<string> data)
        {
            var server = application.Server as UAServer;
            return server.UANodeManager.SynchronizeScriptRemoteDebugging(id, data);
        }

#endregion
#endif
    }
}
