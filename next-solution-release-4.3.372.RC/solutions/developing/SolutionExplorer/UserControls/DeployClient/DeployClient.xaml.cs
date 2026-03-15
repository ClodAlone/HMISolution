using DeployClientLib;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Threading.Tasks;
using UIMsgBoxAlertService.ComponentService;
using DeployServer.Processes;
using Utilities.WPF;
using Utilities;
using System.Collections.Generic;
using System.IO.Compression;
using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using System.IO.IsolatedStorage;

namespace UFProjectManager.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DeployClient : UserControl
    {
        #region Declarations
        DeployServerClient deployServerClient = new DeployServerClient();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DeployClientVM vm;
        UFProjectDocument document;
        RuntimeInfo targetMachineRuntimeInfo;
        string targetServerVersion = Properties.Resources.NotFound;
        public IUIMsgBoxAlertService uiInterface;
        string remoteProjectDirectory;
        string remoteImagesDirectory;
        string remoteDocumentsDirectory;
        string connextPath;
        string moviconPath;
        string projectRoot;
        string projectPath;
        string projectFilePath;
        Tuple<string, string> licenseFolders;
        bool bUpdatingSource;
        bool bLoaded;
        enum ServerType
        {
            IODataServer,
            WebHMIServer,
            SchedulerServer,
            RecipesServer,
            LogicsServer,
            ADServer
        }
        string selectedProcessName;
        string projectDownloadPath;
        string ADServerConfigurationID;
        readonly string downloadPathFilename = String.Format("{0}.dat", Properties.Settings.Default.DownloadPathFileName);
        ProcessOutput processOutputControl;
        #endregion

        #region Properties
        string TargetServerVersion 
        {
            get
            {
                return targetServerVersion;
            }
            set
            {
                if (value != targetServerVersion)
                {
                    targetServerVersion = value;
                    UpdateTargetMachineInfo();
                }
            }
        }
        string targetWebHMIVersion = Properties.Resources.NotFound;
        string TargetWebHMIVersion
        {
            get
            {
                return targetWebHMIVersion;
            }
            set
            {
                if (value != targetWebHMIVersion)
                {
                    targetWebHMIVersion = value;
                    UpdateTargetMachineInfo();
                }
            }
        }
        #endregion

        #region Ctor
        public DeployClient(UFProjectDocument doc, IUIMsgBoxAlertService alertService)
        {
            InitializeComponent();

            document = doc;
            DirectoryInfo di = new DirectoryInfo(ApplicationPropertiesHelper.GetProperty("CommonFolder").ToString());
            licenseFolders = new Tuple<string, string>(di.Parent.Name, di.Name);
            remoteProjectDirectory = String.Format("{0}/{1}", Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            remoteImagesDirectory = String.Format("{0}/{1}/{2}/{3}", Properties.Settings.Default.ServerWebHMIRootFolder, Properties.Settings.Default.ClientWebHMIwwwFolder, DocumentManager.ComponentService.SpecialFolders.Images.ToString().ToLower(), document.Title);
            remoteDocumentsDirectory = String.Format("{0}/{1}/{2}/{3}", Properties.Settings.Default.ServerWebHMIRootFolder, Properties.Settings.Default.ClientWebHMIwwwFolder, DocumentManager.ComponentService.SpecialFolders.Documents.ToString().ToLower(), document.Title);
            projectRoot = String.Format("{0}{1}/{2}", DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            projectPath = String.Format("{0}/{1}", projectRoot, document.Title);
            projectFilePath = String.Format("{0}/{1}.UFProject", projectRoot, document.Title);
            uiInterface = alertService;
            DataContext = vm = new DeployClientVM();
            GetProjectDownloadLocalPath();
            LoadSavedProfiles();
            CheckForNeededServers();

            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Closing += (s, ea) =>
                        {
                            CustomDialogResults? res = null;
                            if (vm.DeployProfiles.bNeedsSave)
                            {
                                res = uiInterface.ShowYesNoCancel(Properties.Resources.WebHMISaveProfilesQuestion, CustomDialogIcons.Question);
                                if (res == CustomDialogResults.Cancel)
                                {
                                    ea.Cancel = true;
                                    return;
                                }
                            }
                            dispatcherTimer.Tick -= DispatcherTimer_Tick;
                            Disconnect();
                            vm.DeployProfiles.Dispose();
                            if (res == CustomDialogResults.Yes)
                                PersistProfiles(true);
                        };
                    }
                }
            };
        }
        #endregion

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void CheckForNeededServers()
        {
#if !NET_STANDARD
            List<Document.StartupLogic> logicsList = null;
            if (document.GetLogicDocumentManager() != null)
                logicsList = (from c in document.ListStartupLogics where c.ExecuteAsService == true select c).ToList();
            else
            {
                btnStartLogicServer.Visibility = Visibility.Collapsed;
                btnStopLogicServer.Visibility = Visibility.Collapsed;
            }

            List<Uri> recipesList = null;
            if (document.GetRecipeDocumentManager() != null)
                recipesList = document.GetWholeRecipeLists();
            else
            {
                btnStartRecipeServer.Visibility = Visibility.Collapsed;
                btnStopRecipeServer.Visibility = Visibility.Collapsed;
            }

            bool serverDocEmpty = true;
            var serverComponent = document.GetService(typeof(UFUAEditor.ComponentService.IUFUAEditorManager)) as DocumentManager.ComponentService.IDocumentManager;
            if (serverComponent != null)
            {
                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                var serverDoc = serverComponent.GetDocument(uri);
                if (serverDoc == null)
                {
                    serverComponent.GetChildDocumentManagers(uri, document);
                    serverDoc = serverComponent.GetDocument(uri);
                }
                serverDocEmpty = serverDoc == null || serverDoc.IsEmpty;
            }
            else
            {
                btnStartIOServer.Visibility = Visibility.Collapsed;
                btnStopIOServer.Visibility = Visibility.Collapsed;
            }

            bool schedulerDocEmpty = true;
            var schedulerComponent = document.GetService(typeof(MSSchedulerSettings.ComponentService.ISchedulerEditorManager)) as DocumentManager.ComponentService.IDocumentManager;
            if (schedulerComponent != null)
            {
                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                var schedulerDoc = schedulerComponent.GetDocument(uri);
                if (schedulerDoc == null)
                {
                    schedulerComponent.GetChildDocumentManagers(uri, document);
                    schedulerDoc = schedulerComponent.GetDocument(uri);
                }
                schedulerDocEmpty = schedulerDoc == null || schedulerDoc.IsEmpty;
            }
            else
            {
                btnStartSchedulerServer.Visibility = Visibility.Collapsed;
                btnStopSchedulerServer.Visibility = Visibility.Collapsed;
            }

            bool ADDocEmpty = true;
            var ADComponent = document.GetService(typeof(ADEditor.ComponentService.IADEditorManager)) as DocumentManager.ComponentService.IDocumentManager;
            if (ADComponent != null)
            {
                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                var ADDoc = ADComponent.GetDocument(uri);
                if (ADDoc == null)
                {
                    ADComponent.GetChildDocumentManagers(uri, document);
                    ADDoc = ADComponent.GetDocument(uri);
                }
                ADDocEmpty = ADDoc == null || ADDoc.IsEmpty;
            }
            else
            {
                btnStartADServer.Visibility = Visibility.Collapsed;
                btnStopADServer.Visibility = Visibility.Collapsed;
            }

            if (document.GetScreenDocumentManager() == null)
            {
                btnStartWebHMIServer.Visibility = Visibility.Collapsed;
                btnStopWebHMIServer.Visibility = Visibility.Collapsed;
                btnStartTargetBrowser.Visibility = Visibility.Collapsed;
                btnStopTargetBrowser.Visibility = Visibility.Collapsed;
            }

            vm.IsDataServerNeeded = !serverDocEmpty;
            vm.IsSchedulerServerNeeded = !schedulerDocEmpty;
            vm.IsRecipesServerNeeded = recipesList != null && recipesList.Count > 0;
            vm.IsLogicsServerNeeded = logicsList != null && logicsList.Count > 0;
            vm.IsADServerNeeded = !ADDocEmpty;

            //TargetMachineCommandsLabel.Text = "LOGIC "+ logicsList.Count + " RECPIES " + recipesList.Count + " SERVER " + (serverDocEmpty == false).ToString() + " SCHED " + (schedulerDocEmpty == false).ToString();
#endif
        }

        private bool CheckReplaceOldProjectProcesses(List<ProcessData> processData)
        {
            var bRet = false;
            var oldProcessExecuting = (from p in processData where p.documentTitle != document.Title select p.documentTitle).FirstOrDefault();
            if (oldProcessExecuting == null)
                return bRet;

            if (uiInterface.ShowYesNo(String.Format(Properties.Resources.ReplaceExecutingProject, oldProcessExecuting), CustomDialogIcons.Question) == CustomDialogResults.Yes)
                bRet = true;
            return bRet;
        }

        private async Task DispatcherTimer_Tick(bool bFirstTick = false)
        {
            //System.Diagnostics.Debug.WriteLine("*********** DEPLOYCLIENT TICK...");
            if (!deployServerClient.Connected)
            {
                uiInterface.ShowError(Properties.Resources.WebHMITargetMachineConnectionLost);
                Disconnect();
                return;
            }
            if (!deployServerClient.Active)
                return;

            List<ProcessData> processData = null;
            try
            {
                processData = await deployServerClient.GetProcessesData();
            }
            catch
            {
                return;
            }

            if (bFirstTick && CheckReplaceOldProjectProcesses(processData))
            {
                var ret = await deployServerClient.RemoveProcessesExcept(document.Title);
                if (ret.Success)
                    processData?.Clear();
                else if (!ret.SoftFail) {
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
                    try
                    {
                        processData = await deployServerClient.GetProcessesData();
                    }
                    catch
                    {
                        return;
                    }
                }
            }

            bUpdatingSource = true;
            dataGrid.ItemsSource = processData;
            bUpdatingSource = false;

            if (processData != null)
            {
                ProcessData pdata = (from pd in processData where pd.name == selectedProcessName select pd).FirstOrDefault();
                if (pdata != null)
                {
                    if (processOutputControl != null)
                        processOutputControl.ProcessOut = pdata.processOutput;
                    dataGrid.SelectedItem = pdata;
                }
            }
            vm.IsWebHMIRunning = vm.IsConnected && (from ProcessData p in processData where p.name == String.Format("{0}_{1}", Properties.Settings.Default.ClientWebHMIRootFolder, document.Title) select p).FirstOrDefault() != null;
            vm.IsDataServerRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.ServerServiceNamePrefix select p).FirstOrDefault() != null;
            vm.IsBrowserRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.WebBrowserProcessName select p).FirstOrDefault() != null;
            vm.IsLogicsServerRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.LogicsServiceNamePrefix select p).FirstOrDefault() != null;
            vm.IsRecipesServerRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.RecipesServiceNamePrefix select p).FirstOrDefault() != null;
            vm.IsSchedulerServerRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.SchedulerServiceNamePrefix select p).FirstOrDefault() != null;
            vm.IsADServerRunning = vm.IsConnected && (from ProcessData p in processData where p.name == Properties.Settings.Default.ADServiceNamePrefix select p).FirstOrDefault() != null;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer_Tick();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs ea)
        {
            bool bConnected = false;
            var host = String.Format("https://{0}", textboxHost.Text);
            if (vm.ActiveProfile.Port != null)
                host = String.Format("{0}:{1}", host, vm.ActiveProfile.Port);
            string errMsg = String.Format(Properties.Resources.ConnectionFailedErrMsg, host);
            using (var cursor = new WaitCursor())
            {
                try
                {
                    await Disconnect();
                    btnConnect.IsEnabled = false;
                    bConnected = await deployServerClient.InitiateConnection(host, textboxUser.Text, textboxPassword.Text, (int)spineditTimeout.EditValue);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.InnerException is System.Security.Authentication.AuthenticationException)
                    {
                        if (uiInterface.ShowYesNo(Properties.Resources.AcceptUnsecureConnections, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                        {
                            try
                            {
                                await Disconnect();
                                btnConnect.IsEnabled = false;
                                bConnected = await deployServerClient.InitiateConnection(host, textboxUser.Text, textboxPassword.Text, (int)spineditTimeout.EditValue, true);
                            }
                            catch (Exception e)
                            {
                                errMsg = String.Format("{0}\n{1}: {2}", errMsg, e.GetType().FullName, e.Message);
                            }
                        }
                        else
                        {
                            await Disconnect();
                            return;
                        }
                    }
                    else
                        errMsg = String.Format("{0}\n{1}: {2}", errMsg, ex.GetType().FullName, ex.Message);
                }
                if (!bConnected)
                {
                    await Disconnect();
                    uiInterface.ShowError(errMsg);
                    return;
                }
                else
                {
                    connextPath = moviconPath = null;
                    dispatcherTimer.Stop();
                    await DispatcherTimer_Tick(true);
                    dispatcherTimer.Start();
                    try
                    {
                        targetMachineRuntimeInfo = await deployServerClient.GetPlatformDescription();
                        CheckDeployServerVersion();
                    }
                    catch
                    {
                        uiInterface.ShowError(Properties.Resources.WebHMIErrorGettingPlatformInfo);
                    }
                    bool bInfoSet = false;
                    try
                    {
                        await RefreshTargetServerVersion(ServerType.IODataServer);
                        bInfoSet = true;
                    }
                    catch { }
                    try
                    {
                        await RefreshTargetServerVersion(ServerType.WebHMIServer);
                        bInfoSet = true;
                    }
                    catch { }

                    if (!bInfoSet)
                        UpdateTargetMachineInfo();

                    vm.IsConnected = true;

                    if (await deployServerClient.DirectoryExist(remoteProjectDirectory))
                    {
                        btnDeleteRemoteProject.IsEnabled = true;
                        vm.CanDownload = true;
                    }
                    vm.CanScanProjects = await deployServerClient.DirectoryExist(Properties.Settings.Default.ServerProjectsRootFolder);
                }
            }

            btnDeployServers_Click(this, null);
        }

        void CheckDeployServerVersion()
        {
            try
            {
                var clientVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
                if (clientVersion != targetMachineRuntimeInfo.DeployServerVersion) {
                    var deployServerVersion = targetMachineRuntimeInfo.DeployServerVersion;
                    if (String.IsNullOrEmpty(deployServerVersion))
                        deployServerVersion = Properties.Resources.VersionUndefined;
                    uiInterface.ShowHidingInformation(String.Format(Properties.Resources.DeployServerMisalignedMessageInfo, deployServerVersion, clientVersion), "ApplicationDeployServerVersionInfo");
                }
            }
            catch { }
        }

        void UpdateTargetMachineInfo()
        {
            if (targetMachineRuntimeInfo == null)
                return;

            TargetMachineInfo.Text = String.Format("{1} {2}, {3} {4}{0}{9} : {10}{0}{0}OS Description : {5}{0}OS Architecture : {6}{0}Framework Description : {7}{0}Process Architecture : {8}", Environment.NewLine, Properties.Resources.IODataServerVersion, TargetServerVersion, Properties.Resources.WebHMIServerVersion, TargetWebHMIVersion, targetMachineRuntimeInfo.OSDescription, targetMachineRuntimeInfo.OSArchitecture, targetMachineRuntimeInfo.FrameworkDescription, targetMachineRuntimeInfo.ProcessArchitecture, Properties.Resources.WebHMIDeployServerVersionLabel, String.IsNullOrEmpty(targetMachineRuntimeInfo.DeployServerVersion) ? Properties.Resources.VersionUndefined : targetMachineRuntimeInfo.DeployServerVersion);
        }

        async Task RefreshTargetServerVersion(ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.IODataServer:
                    TargetServerVersion = Properties.Resources.NotFound;
                    try
                    {
                        TargetServerVersion = await deployServerClient.FileVersion(String.Format("{0}/{1}", Properties.Settings.Default.ServerIODataServerRootFolder, Properties.Settings.Default.CheckedAssemblyName_IODataServer));
                    }
                    catch (Exception ex)
                    {
                        if (targetMachineRuntimeInfo.IsWindows)
                        {
                            if (String.IsNullOrEmpty(moviconPath))
                                await GetMoviconPath();
                            if (String.IsNullOrEmpty(moviconPath) && String.IsNullOrEmpty(connextPath))
                                await GetConnextPath();
                        }
                        if (String.IsNullOrEmpty(connextPath) && String.IsNullOrEmpty(moviconPath))
                            throw (ex);
                    }
                    vm.IsDataServerInstalled = true;
                    vm.IsLogicsServerInstalled = true;
                    vm.IsRecipesServerInstalled = true;
                    vm.IsSchedulerServerInstalled = true;
                    vm.IsADServerInstalled = true;
                break;
                case ServerType.WebHMIServer:
                    TargetWebHMIVersion = Properties.Resources.NotFound;
                    TargetWebHMIVersion = await deployServerClient.FileVersion(String.Format("{0}/{1}", Properties.Settings.Default.ServerWebHMIRootFolder, Properties.Settings.Default.CheckedAssemblyName_WebHMI));
                    vm.IsWebHMIServerInstalled = true;
                break;
            }
        }

        private async void btnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (uiInterface.ShowYesNo(Properties.Resources.ConfirmProjectDeletion, CustomDialogIcons.Question) == CustomDialogResults.Yes) 
            {
                using (var cursor = new WaitCursor())
                {
                    var errorMsg = "";
                    if (!await deployServerClient.DeleteDirectory(remoteProjectDirectory))
                        errorMsg = String.Format("{0}{1}", errorMsg, Properties.Resources.ErrorDeletingProject);
                    if (!await deployServerClient.DeleteDirectory(remoteImagesDirectory))
                        errorMsg = String.Format("{0}\r\n{1}", errorMsg, Properties.Resources.ErrorDeletingProjectImages);
                    if (!await deployServerClient.DeleteDirectory(remoteDocumentsDirectory))
                        errorMsg = String.Format("{0}\r\n{1}", errorMsg, Properties.Resources.ErrorDeletingProjectDocuments);
                    if (!String.IsNullOrEmpty(errorMsg))
                        uiInterface.ShowError(errorMsg);
                    else
                    {
                        btnDeleteRemoteProject.IsEnabled = false;
                        uiInterface.ShowInformation(Properties.Resources.RemoteProjectDeletionSuccess);
                    }
                }
            }
        }

        private void btnStartAllServers_Click(object sender, RoutedEventArgs e)
        {
            if (vm.IsDataServerNeeded && vm.IsDataServerInstalled && !vm.IsDataServerStarting && !vm.IsDataServerRunning)
                Button_AddProcess_DataServer_Click(this, null);
            if (vm.IsLogicsServerNeeded && vm.IsLogicsServerInstalled && !vm.IsLogicsServerStarting && !vm.IsLogicsServerRunning)
                Button_AddProcess_Logics_Click(this, null);
            if (vm.IsRecipesServerNeeded && vm.IsRecipesServerInstalled && !vm.IsRecipesServerStarting && !vm.IsRecipesServerRunning)
                Button_AddProcess_Recipes_Click(this, null);
            if (vm.IsSchedulerServerNeeded && vm.IsSchedulerServerInstalled && !vm.IsSchedulerServerStarting && !vm.IsSchedulerServerRunning)
                Button_AddProcess_SchedulerServer_Click(this, null);
            if (vm.IsADServerNeeded && vm.IsADServerInstalled && !vm.IsADServerStarting && !vm.IsADServerRunning)
                Button_AddProcess_ADServer_Click(this, null);
            if (document.GetScreenDocumentManager() != null && !vm.IsWebHMIStarting && !vm.IsWebHMIRunning)
                Button_AddProcess_WebHMI_Click(this, null);
        }

        private async void btnDeployServers_Click(object sender, RoutedEventArgs e)
        {
            bool bUploadCommitted = false;
            string deployErrors = String.Empty;
            string localServerAssemblyRelativePath = null;
            string localClientAssemblyRelativePath = null;
            string localServerVersion = null;
            string localClientVersion = null;
            bool bTransferClient = false;
            bool bTransferServer = false;
            bool bTransferClientZip = false;
            bool bTransferServerZip = false;
            string serverZipPath = String.Empty;
            string clientZipPath = String.Empty;

            using (var cursor = new WaitCursor())
            {
                var serverComponent = document.GetService(typeof(UFUAEditor.ComponentService.IUFUAEditorManager)) as DocumentManager.ComponentService.IDocumentManager;
                bTransferServer = serverComponent != null;
                bTransferClient = document.GetScreenDocumentManager() != null;
                if (bTransferClient)
                {
                    try
                    {
                        clientZipPath = String.Format("{0}{1}.zip", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientWebHMIRootFolder);
                        using (var zipFile = ZipFile.OpenRead(clientZipPath))
                        {
                            var clientAssembly = (from entry in zipFile.Entries where entry.Name == Properties.Settings.Default.CheckedAssemblyName_WebHMI select entry).FirstOrDefault();
                            if (clientAssembly != null)
                            {
                                var tempFile = Path.GetTempFileName();
                                File.Delete(tempFile);
                                clientAssembly.ExtractToFile(tempFile);
                                localClientVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(tempFile).FileVersion;
                                bTransferClientZip = true;
                                File.Delete(tempFile);
                            }
                        }
                    }
                    catch { }
                }
                if (bTransferServer)
                {
                    try
                    {
                        serverZipPath = String.Format("{0}{1}.zip", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientIODataServerRootFolder);
                        using (var zipFile = ZipFile.OpenRead(serverZipPath))
                        {
                            var serverAssembly = (from entry in zipFile.Entries where entry.Name == Properties.Settings.Default.CheckedAssemblyName_IODataServer select entry).FirstOrDefault();
                            if (serverAssembly != null)
                            {
                                var tempFile = Path.GetTempFileName();
                                File.Delete(tempFile);
                                serverAssembly.ExtractToFile(tempFile);
                                localServerVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(tempFile).FileVersion;
                                bTransferServerZip = true;
                                File.Delete(tempFile);
                            }
                        }
                    }
                    catch { }
                }

                if (bTransferServer && !bTransferServerZip)
                {
                    localServerAssemblyRelativePath = String.Format("{0}{1}{2}", Properties.Settings.Default.ClientIODataServerRootFolder, Path.DirectorySeparatorChar, Properties.Settings.Default.CheckedAssemblyName_IODataServer);
                    try
                    {
                        localServerVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, localServerAssemblyRelativePath)).FileVersion;
                    }
                    catch { }
                }
                if (bTransferClient && !bTransferClientZip)
                {
                    localClientAssemblyRelativePath = String.Format("{0}{1}{2}", Properties.Settings.Default.ClientWebHMIRootFolder, Path.DirectorySeparatorChar, Properties.Settings.Default.CheckedAssemblyName_WebHMI);
                    try
                    {
                        localClientVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, localClientAssemblyRelativePath)).FileVersion;
                    }
                    catch { }
                }
                
                if ((bTransferClient && localClientVersion == null) || (bTransferServer && localServerVersion == null))
                {
                    uiInterface.ShowError(String.Format(Properties.Resources.WebHMIInstallationFilesNotFound, localServerVersion == null ? Properties.Settings.Default.CheckedAssemblyName_IODataServer : String.Empty, localClientVersion == null ? Properties.Settings.Default.CheckedAssemblyName_WebHMI : String.Empty));
                    return;
                }

                //Checking I/O Data Server on target machine
                if (localServerVersion != null)
                {
                    try
                    {
                        await RefreshTargetServerVersion(ServerType.IODataServer);
                        if (TargetServerVersion != localServerVersion)
                        {
                            if (uiInterface.ShowYesNo(String.Format(Properties.Resources.IODataServerUpdateTargetQuestion, TargetServerVersion, localServerVersion), CustomDialogIcons.Question) == CustomDialogResults.Yes)
                            {
                                bUploadCommitted = true;
                                bool bUploadRet = false;
                                if (bTransferServerZip && !String.IsNullOrEmpty(serverZipPath))
                                    bUploadRet = await TransferFile(serverZipPath, Properties.Settings.Default.ServerIODataServerRootFolder);
                                else
                                    bUploadRet = await StartUploading(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientIODataServerRootFolder), Properties.Settings.Default.ServerIODataServerRootFolder);
                                if (bUploadRet)
                                    await RefreshTargetServerVersion(ServerType.IODataServer);
                                else
                                    deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, Properties.Resources.WebHMIServerDeployFailed);
                            }
                        }
                    }
                    catch
                    {
                        if (uiInterface.ShowYesNo(Properties.Resources.IODataServerNotFoundOnTargetError, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                        {
                            bUploadCommitted = true;
                            bool bUploadRet = false;
                            if (bTransferServerZip && !String.IsNullOrEmpty(serverZipPath))
                                bUploadRet = await TransferFile(serverZipPath, Properties.Settings.Default.ServerIODataServerRootFolder);
                            else
                                bUploadRet = await StartUploading(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientIODataServerRootFolder), Properties.Settings.Default.ServerIODataServerRootFolder);
                            if (bUploadRet)
                                try { await RefreshTargetServerVersion(ServerType.IODataServer); } catch { }
                            else
                                deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, Properties.Resources.WebHMIServerDeployFailed);
                        }
                    }
                }

                //Checking WebHMI Server on target machine
                if (localClientVersion != null)
                {
                    try
                    {
                        await RefreshTargetServerVersion(ServerType.WebHMIServer);
                        if (TargetWebHMIVersion != localClientVersion)
                        {
                            if (uiInterface.ShowYesNo(String.Format(Properties.Resources.WebHMIServerUpdateTargetQuestion, TargetWebHMIVersion, localClientVersion), CustomDialogIcons.Question) == CustomDialogResults.Yes)
                            {
                                bUploadCommitted = true;
                                bool bUploadRet = false;
                                if (bTransferClientZip && !String.IsNullOrEmpty(clientZipPath))
                                    bUploadRet = await TransferFile(clientZipPath, Properties.Settings.Default.ServerWebHMIRootFolder);
                                else
                                    bUploadRet = await StartUploading(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientWebHMIRootFolder), Properties.Settings.Default.ServerWebHMIRootFolder);
                                if (bUploadRet)
                                    await RefreshTargetServerVersion(ServerType.WebHMIServer);
                                else
                                    deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, Properties.Resources.WebHMIClientDeployFailed);
                            }
                        }
                    }
                    catch
                    {
                        if (uiInterface.ShowYesNo(Properties.Resources.WebHMIServerNotFoundOnTargetError, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                        {
                            bUploadCommitted = true;
                            bool bUploadRet = false;
                            if (bTransferClientZip && !String.IsNullOrEmpty(clientZipPath))
                                bUploadRet = await TransferFile(clientZipPath, Properties.Settings.Default.ServerWebHMIRootFolder);
                            else
                                bUploadRet = await StartUploading(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ClientWebHMIRootFolder), Properties.Settings.Default.ServerWebHMIRootFolder);
                            if (bUploadRet)
                                try { await RefreshTargetServerVersion(ServerType.WebHMIServer); } catch { }
                            else
                                deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, Properties.Resources.WebHMIClientDeployFailed);
                        }
                    }
                }

                if (uiInterface.ShowYesNo(Properties.Resources.UploadProjectOnTargetMachine, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                {
                    bUploadCommitted = true;
                    //Copying project to a temporary folder and Uploading on target machine
                    string tempProjectFolder = null;
                    try
                    {
                        var ext = Path.GetExtension(document.ProjectPath);
                        if (String.IsNullOrEmpty(ext))
                            ext = Properties.Settings.Default.DefaultFileExt;
                        var prjName = document.Title;
                        tempProjectFolder = String.Format("{0}{1}", Path.GetRandomFileName(), document.Title);
                        var tempProjectPath = string.Format("{0}{1}\\{2}{3}", Path.GetTempPath(),
                            tempProjectFolder, prjName, ext);
                        tempProjectFolder = Path.GetDirectoryName(tempProjectPath);
                        if (Directory.Exists(tempProjectFolder))
                            Directory.Delete(tempProjectFolder, true);
                        Directory.CreateDirectory(tempProjectFolder);
                        document.SaveAs(tempProjectPath);

                        Utilities.IO.FileSystem.CopyTo(GetRetentiveFolders(document.ProjectFolder), String.Format("{0}{1}{2}{1}{3}", tempProjectFolder, Path.DirectorySeparatorChar, document.Title, DocManagerType.UFUAServer.ToString()));

                        var tilesFilePath = String.Format("{0}/{1}/{2}", Path.GetDirectoryName(document.ProjectPath), SpecialFolders.Documents.ToString(), Properties.Settings.Default.TileDetailsFilename);
                        Utilities.IO.FileSystem.CopyTo(new List<string>() { tilesFilePath }, String.Format("{0}/{1}", tempProjectFolder, SpecialFolders.Documents.ToString()));
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (Directory.Exists(tempProjectFolder))
                                Directory.Delete(tempProjectFolder, true);
                        }
                        catch { }
                        uiInterface.ShowError(String.Format(Properties.Resources.WebHMIDeployErrorCopyingProject, ex.Message));
                        return;
                    }

                    if (!await StartUploading(tempProjectFolder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title))
                        deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, Properties.Resources.WebHMIProjectDeployFailed);
                    else
                    {
                        var projectMainFolder = String.Format("{0}/{1}", Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
                        var imgTransfer = await deployServerClient.CopyFiles(String.Format("{0}/{1}", projectMainFolder, DocumentManager.ComponentService.SpecialFolders.Images.ToString()), remoteImagesDirectory);
                        if (!imgTransfer.Success)
                            deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, String.Format(Properties.Resources.WebHMIProjectImagesDeployFailed, imgTransfer.Message));

                        var docTransfer = await deployServerClient.CopyFiles(String.Format("{0}/{1}", projectMainFolder, DocumentManager.ComponentService.SpecialFolders.Documents.ToString()), remoteDocumentsDirectory, Properties.Settings.Default.WebHMIWwwrootDocumentsExtensionsPattern);
                        
                        if (!docTransfer.Success)
                            deployErrors = String.Format("{0}{1}{2}", deployErrors, Environment.NewLine, String.Format(Properties.Resources.WebHMIProjectDocumentsDeployFailed, docTransfer.Message));
                    }

                    try
                    {
                        if (Directory.Exists(tempProjectFolder))
                            Directory.Delete(tempProjectFolder, true);
                    }
                    catch { }
                }

                if (await deployServerClient.DirectoryExist(remoteProjectDirectory))
                {
                    btnDeleteRemoteProject.IsEnabled = true;
                    vm.CanDownload = true;
                }
            }

            if (bUploadCommitted)
            {
                if (String.IsNullOrEmpty(deployErrors))
                {
                    var message = String.Format("{0}{1}{1}{2}", Properties.Resources.WebHMIDeploySuccess, Environment.NewLine, Properties.Resources.ApplicationCertificateMassageInfo);
                    if (!uiInterface.ShowHidingInformation(message, "ApplicationCertificateMassageInfo"))
                        uiInterface.ShowInformation(Properties.Resources.WebHMIDeploySuccess);
                }
                else
                    uiInterface.ShowError(String.Format(Properties.Resources.WebHMIDeployFailed, deployErrors));
            }
        }

        private async Task<bool> StartUploading(string sourcePath, string targetPath, string copyInsideNewDirOnTarget = null)
        {
            vm.UploadStatusDisplayString = null;
            vm.IsUploading = true;
            bool ret = false;
            using (var cursor = new WaitCursor())
            {
                try
                {
                    ret = await TransferFolderTree(sourcePath, targetPath, copyInsideNewDirOnTarget);
                }
                catch (Exception ex)
                {
                    if (ex is Microsoft.AspNetCore.SignalR.HubException)
                        uiInterface.ShowError(String.Format("{0} {1}", ex.Message, Properties.Resources.WebHMIHubException));
                    else
                        uiInterface.ShowError(ex.Message);
                    return false;
                }
                finally
                {
                    vm.IsUploading = false;
                    vm.UploadStatusDisplayString = ret ? Properties.Resources.UploadCompleted : Properties.Resources.UploadInterrupted;
                }
            }
            return ret;
        }

        async Task<bool> TransferFile(string sourcePath, string targetPath, bool bDeleteOldFile = true)
        {
            if (bDeleteOldFile)
                if (!await deployServerClient.DeleteFile(targetPath))
                    return false;

            var fileSize = new FileInfo(sourcePath).Length;

            var singleFileProgress = new Progress<int>(value => {
                singleTransferProgressBar.StyleSettings = null;
                if (value == 100)
                {
                    singleTransferProgressBar.StyleSettings = new ProgressBarMarqueeStyleSettings() { AccelerateRatio = 3 };
                    vm.UploadingFileLabelText = Properties.Resources.WebHMIWaitRemoteUnzipping;
                }
                vm.SingleFileUploadProgress = Math.Min(100, value);
            });
            var totalFilesProgress = new Progress<long>(value =>
            {
                var percentage = (int)(value * 100 / fileSize);
                if (value <= fileSize && percentage == 100)
                {
                    percentage = 99;
                    vm.UploadingFileLabelText = Properties.Resources.WebHMIWaitRemoteUnzipping;
                }
                vm.TotalFilesUploadProgress = Math.Min(percentage, 100);
            });
            long totalUploadedBytes = 0;
            
            var targetSubFilePath = String.Format("{0}/{1}{2}", targetPath, sourcePath.Substring(sourcePath.Length).TrimStart(new[] { '/', '\\' }), Path.GetFileName(sourcePath)).Replace(Path.DirectorySeparatorChar, '/');
            vm.UploadingFileLabelText = Path.GetFileName(sourcePath);
            bool ret = false;
            try
            {
                ret = await deployServerClient.UploadFile(sourcePath, targetSubFilePath, Properties.Settings.Default.WebHMIMaxUploadChunkSize, totalUploadedBytes, singleFileProgress, totalFilesProgress);
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.AspNetCore.SignalR.HubException)
                    uiInterface.ShowError(String.Format("{0} {1}", ex.Message, Properties.Resources.WebHMIHubException));
                else
                    uiInterface.ShowError(ex.Message);
                return false;
            }
            totalUploadedBytes += new FileInfo(sourcePath).Length;
            vm.UploadingFileLabelText = String.Empty;
            if (!ret)
                return false;

            vm.SingleFileUploadProgress = vm.TotalFilesUploadProgress = 100;
            return true;
        }

        async Task<bool> TransferFolderTree(string sourcePath, string targetPath, string copyInsideNewDirOnTarget = null)
        {
            var bKeepRetentiveFiles = false;
            if (!String.IsNullOrEmpty(copyInsideNewDirOnTarget))
                targetPath = String.Format("{0}/{1}", targetPath, copyInsideNewDirOnTarget);

            var tempFolderName = String.Format("{0}{1}", targetPath, Path.GetRandomFileName());
            BoolTaskResult rt = await deployServerClient.CopyDirectory(targetPath, tempFolderName);
            if (!rt.Success)
            {
                if (!rt.SoftFail)
                    return false;
                if (!await deployServerClient.DeleteDirectory(targetPath))
                    return false;
                tempFolderName = null;
            }

            List<string> pathsNotUploaded = new List<string>();
            if (rt.ReturnValue && checkboxRetentive.IsChecked != true)
            {
                bKeepRetentiveFiles = true;
                pathsNotUploaded = GetRetentiveFolders(String.Format("{0}{1}{2}", sourcePath, Path.DirectorySeparatorChar, document.Title));
            }

            string[] allfiles;
            long totalFilesSize = 0;
            try
            {
                allfiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                if (pathsNotUploaded.Count > 0)
                    allfiles = allfiles.Where(file => {
                        foreach (var path in pathsNotUploaded)
                            if (file.Contains(path))
                                return false;
                        return true;
                    }).ToArray();
                
                totalFilesSize = allfiles.Select(file => new FileInfo(file).Length).Sum();
            }
            catch
            {
                if (tempFolderName != null)
                    await deployServerClient.DeleteDirectory(tempFolderName);
                return false;
            }

            var singleFileProgress = new Progress<int>(value => {
                singleTransferProgressBar.StyleSettings = null;
                if (value == 100)
                {
                    singleTransferProgressBar.StyleSettings = new ProgressBarMarqueeStyleSettings() { AccelerateRatio = 3 };
                    vm.UploadingFileLabelText = Properties.Resources.WebHMIWaitRemoteUnzipping;
                }
                vm.SingleFileUploadProgress = Math.Min(100, value);
            });
            var totalFilesProgress = new Progress<long>(value =>
            {
                var percentage = (int)(value * 100 / totalFilesSize);
                if (value <= totalFilesSize && percentage == 100)
                {
                    percentage = 99;
                    vm.UploadingFileLabelText = Properties.Resources.WebHMIWaitRemoteUnzipping;
                }
                vm.TotalFilesUploadProgress = Math.Min(percentage, 100);
            });
            long totalUploadedBytes = 0;

            foreach (var file in allfiles)
            {
                var targetSubPath = String.Format("{0}/{1}", targetPath, file.Substring(sourcePath.Length).TrimStart(new[] { '/', '\\' })).Replace(Path.DirectorySeparatorChar, '/');
                vm.UploadingFileLabelText = Path.GetFileName(file);
                var ret = await deployServerClient.UploadFile(file, targetSubPath, Properties.Settings.Default.WebHMIMaxUploadChunkSize, totalUploadedBytes, singleFileProgress, totalFilesProgress);
                totalUploadedBytes += new FileInfo(file).Length;
                vm.UploadingFileLabelText = String.Empty;
                if (!ret)
                {
                    if (tempFolderName != null)
                        await deployServerClient.MoveDirectory(tempFolderName, targetPath);
                    return false;
                }
            }
            vm.SingleFileUploadProgress = vm.TotalFilesUploadProgress = 100;

            if (tempFolderName != null)
            {
                if (bKeepRetentiveFiles)
                {
                    var oldFolders = await deployServerClient.CopyRetentiveData(
                        tempFolderName,
                        String.Format("{0}{1}{2}{1}{3}", targetPath, Path.DirectorySeparatorChar, document.Title, DocManagerType.UFUAServer.ToString()),
                        GetRetentiveFolders(String.Format("{0}{1}{2}", tempFolderName, Path.DirectorySeparatorChar, document.Title))
                    );
                }
                await deployServerClient.DeleteDirectory(tempFolderName);
            }
            return true;
        }

        static List<string> GetRetentiveFolders(string projectPath)
        {
            var serverBasePath = String.Format("{0}{1}{2}", projectPath, Path.DirectorySeparatorChar, DocManagerType.UFUAServer.ToString());

            var tagsFolder = String.Format("{0}{1}{2}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.TagsRootName);
            var alrFolder = String.Format("{0}{1}{2}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.AlarmsRootName);
            var histFolder = String.Format("{0}{1}{2}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.HistorianFlushFolderName);
            var dlFolder = String.Format("{0}{1}{2}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.DataLoggerFlushFolderName);
            var evFolder = String.Format("{0}{1}{2}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.EventFlushFolderName);
            var runtimeAlarmsFile = String.Format("{0}{1}{2}{3}", serverBasePath, Path.DirectorySeparatorChar, UFUAServerInfo.Properties.Settings.Default.RuntimeAlrFileName, UFUAServerInfo.Properties.Settings.Default.DefaultFileExtAlr);

            return new List<string>() { tagsFolder, alrFolder, histFolder, dlFolder, evFolder, runtimeAlarmsFile };
        }

        private async void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            await Disconnect();
        }

        async Task Disconnect()
        {
            using (var cursor = new WaitCursor())
            {
                var ret = await deployServerClient.CloseConnection();
                vm.IsConnected = false;
                TargetMachineInfo.Text = "";
                btnConnect.IsEnabled = true;
                btnDeleteRemoteProject.IsEnabled = false;
                dispatcherTimer.Stop();
            }
        }

        private async void Button_AddProcess_WebHMI_Click(object sender, RoutedEventArgs e)
        {
            var projectFilePath = String.Format("{0}{1}/{2}/{2}.UFProject", DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            using (var cursor = new WaitCursor())
            {
                vm.IsWebHMIStarting = true;
                bool ret;
                string processTitle = String.Format("{0}_{1}", Properties.Settings.Default.ClientWebHMIRootFolder, document.Title);
                
                ret = await deployServerClient.AddProcess(
                    processTitle,
                    "dotnet",
                    String.Format
                    (
                        "\"{0}{1}/{2}\" --server.urls \"{3}\" --WebNExTHMISettings:ProxyPath=\"{4}\" project=\"{5}\"",
                        DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                        Properties.Settings.Default.ServerWebHMIRootFolder,
                        Properties.Settings.Default.CheckedAssemblyName_WebHMI,
                        Properties.Settings.Default.WebHMIServerUrls,
                        DeployServer.Utils.PlaceHolders.DeployWebHMIProxyPathPlaceholder,
                        projectFilePath
                    ),
                    true, document.Title, false
                );
                if (!ret)
                {
                    uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                    vm.IsWebHMIStarting = false;
                }
            }
        }

        private async void Button_RemoveProcess_WebHMI_Click(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var ret = await deployServerClient.RemoveProces(String.Format("{0}_{1}", Properties.Settings.Default.ClientWebHMIRootFolder, document.Title), document.Title, true);
            }
        }

        private async void Button_AddProcess_Logics_Click(object sender, RoutedEventArgs e)
        {
            if (targetMachineRuntimeInfo == null)
                return;

            var projectFilePath = String.Format("{0}{1}/{2}/{2}.UFProject", DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            string processName;

            using (var cursor = new WaitCursor())
            {
                vm.IsLogicsServerStarting = true;
                if (targetMachineRuntimeInfo.IsWindows)
                {
                    if (String.IsNullOrEmpty(moviconPath))
                        await GetMoviconPath();
                    if (!String.IsNullOrEmpty(moviconPath))
                    {
                        processName = String.Format("{0}{1}{2}{1}{2}.exe", Path.GetDirectoryName(moviconPath), Path.DirectorySeparatorChar, Properties.Settings.Default.LogicsServiceNamePrefix);
                        var startCommandLineArgs = String.Format("-noservice \"-conn={0}\"", projectFilePath);

                        if (!await deployServerClient.AddProcess(Properties.Settings.Default.LogicsServiceNamePrefix, processName, startCommandLineArgs, true, document.Title))
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsLogicsServerStarting = false;
                        }
                        return;
                    }
                }

                processName = "dotnet";

                var LogicsServerArguments = String.Format("\"-conn={0}\"", projectFilePath);

                var commandLineArgs = String.Format
                (
                    "\"{0}{1}/{2}\" {3}",
                    DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                    Properties.Settings.Default.ServerIODataServerRootFolder,
                    Properties.Settings.Default.CheckedAssemblyName_LogicsServer,
                    LogicsServerArguments
                );
                var ret = await deployServerClient.AddProcess(Properties.Settings.Default.LogicsServiceNamePrefix, processName, commandLineArgs, true, document.Title);
                if (!ret)
                {
                    uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                    vm.IsLogicsServerStarting = false;
                }
            }
        }

        private async void Button_RemoveProcess_Logics_Click(object sender, RoutedEventArgs e) 
        {
            using (var cursor = new WaitCursor())
            {
                if (!await deployServerClient.RemoveProces(Properties.Settings.Default.LogicsServiceNamePrefix, document.Title, true))
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
            }
        }
        private async void Button_AddProcess_Recipes_Click(object sender, RoutedEventArgs e) 
        {
            if (targetMachineRuntimeInfo == null)
                return;

            string processName;

            using (var cursor = new WaitCursor())
            {
                vm.IsRecipesServerStarting = true;
                if (targetMachineRuntimeInfo.IsWindows)
                {
                    if (String.IsNullOrEmpty(moviconPath))
                        await GetMoviconPath();
                    if (!String.IsNullOrEmpty(moviconPath))
                    {
                        processName = String.Format("{0}{1}{2}.exe", Path.GetDirectoryName(moviconPath), Path.DirectorySeparatorChar, Properties.Settings.Default.RecipesServiceNamePrefix);
                        var startCommandLineArgs = String.Format("-noservice \"-project={1}\" \"-StringConn=XpoProvider={2};data source={0}/{3};\" \"-UserConn=XpoProvider={2};data source={0}/{4};\" \"-DocPath={5}/Documents\"", projectPath, projectFilePath, Properties.Settings.Default.XpoMemoryProvider, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.UsersServerFilePath, projectRoot);

                        if (!await deployServerClient.AddProcess(Properties.Settings.Default.RecipesServiceNamePrefix, processName, startCommandLineArgs, true, document.Title))
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsRecipesServerStarting = false;
                        }
                        return;
                    }
                }

                processName = "dotnet";

                var RecipeNetCoreServerArguments = String.Format("\"-project={1}\" \"-StringConn=XpoProvider={2};data source={0}/{3};\" \"-UserConn=XpoProvider={2};data source={0}/{4};\" \"-DocPath={5}/Documents\"", projectPath, projectFilePath, Properties.Settings.Default.XpoMemoryProvider, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.UsersServerFilePath, projectRoot);

                var commandLineArgs = String.Format
                (
                    "\"{0}{1}/{2}\" {3}",
                    DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                    Properties.Settings.Default.ServerIODataServerRootFolder,
                    Properties.Settings.Default.CheckedAssemblyName_RecipesServer,
                    RecipeNetCoreServerArguments
                );

                if (!await deployServerClient.AddProcess(Properties.Settings.Default.RecipesServiceNamePrefix, processName, commandLineArgs, true, document.Title))
                {
                    uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                    vm.IsRecipesServerStarting = false;
                }
            }
        }
        private async void Button_RemoveProcess_Recipes_Click(object sender, RoutedEventArgs e) 
        {
            using (var cursor = new WaitCursor())
            {
                if (!await deployServerClient.RemoveProces(Properties.Settings.Default.RecipesServiceNamePrefix, document.Title, true))
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
            }
        }
        private async void Button_AddProcess_SchedulerServer_Click(object sender, RoutedEventArgs e)
        {
            if (targetMachineRuntimeInfo == null)
                return;

            var projectPath = String.Format("{0}{1}/{2}/{2}", DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            string processName;

            using (var cursor = new WaitCursor())
            {
                vm.IsSchedulerServerStarting = true;
                if (targetMachineRuntimeInfo.IsWindows)
                {
                    if (String.IsNullOrEmpty(moviconPath))
                        await GetMoviconPath();
                    if (!String.IsNullOrEmpty(moviconPath))
                    {
                        processName = String.Format("{0}{1}{2}.exe", Path.GetDirectoryName(moviconPath), Path.DirectorySeparatorChar, Properties.Settings.Default.SchedulerServiceNamePrefix);
                        var startCommandLineArgs = String.Format("-noservice \"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.MSServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath);

                        if (!await deployServerClient.AddProcess(Properties.Settings.Default.SchedulerServiceNamePrefix, processName, startCommandLineArgs, true, document.Title)) 
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsSchedulerServerStarting = false;
                        }
                        return;
                    }
                }
                
                processName = "dotnet";

                var IOServerArguments = String.Format("\"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.MSServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath);

                var commandLineArgs = String.Format
                (
                    "\"{0}{1}/{2}\" {3}",
                    DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                    Properties.Settings.Default.ServerIODataServerRootFolder,
                    Properties.Settings.Default.CheckedAssemblyName_SchedulerServer,
                    IOServerArguments
                );
                if (!await deployServerClient.AddProcess(Properties.Settings.Default.SchedulerServiceNamePrefix, processName, commandLineArgs, true, document.Title))
                {
                    uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                    vm.IsSchedulerServerStarting = false;
                }
            }
        }

        private async void Button_RemoveProcess_SchedulerServer_Click(object sender, RoutedEventArgs e) 
        {
            using (var cursor = new WaitCursor())
            {
                if (!await deployServerClient.RemoveProces(Properties.Settings.Default.SchedulerServiceNamePrefix, document.Title, true))
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
            }
        }
        private async void Button_AddProcess_ADServer_Click(object sender, RoutedEventArgs e)
        {
            if (targetMachineRuntimeInfo == null)
                return;

            var projectFilePath = String.Format("{0}{1}/{2}/{2}.UFProject", DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder, Properties.Settings.Default.ServerProjectsRootFolder, document.Title);
            string processName;

            using (var cursor = new WaitCursor())
            {
                if (ADServerConfigurationID == null)
                {
                    ADServerConfigurationID = string.Empty;
                    var ADComponent = document.GetService(typeof(ADEditor.ComponentService.IADEditorManager)) as DocumentManager.ComponentService.IDocumentManager;
                    if (ADComponent != null)
                    {
                        var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                        var ADDoc = ADComponent.GetDocument(uri);
                        if (ADDoc != null)
                        {
                            var serverComponent = document.GetService(typeof(UFUAEditor.ComponentService.IUFUAEditorManager)) as UFUAEditor.ComponentService.IUFUAEditorManager;
                            if (serverComponent != null)
                                ADServerConfigurationID = serverComponent.GetServerConfigurationId(ADDoc);
                        }
                    }
                }

                vm.IsADServerStarting = true;
                if (targetMachineRuntimeInfo.IsWindows)
                {
                    if (String.IsNullOrEmpty(moviconPath))
                        await GetMoviconPath();
                    if (!String.IsNullOrEmpty(moviconPath))
                    {
                        processName = String.Format("{0}{1}{2}{1}{2}.exe", Path.GetDirectoryName(moviconPath), Path.DirectorySeparatorChar, Properties.Settings.Default.ADServiceNamePrefix);
                        var startCommandLineArgs = String.Format("-noservice \"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\" \"-SrvrConfigId={9}\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.ADServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath, ADServerConfigurationID);

                        if (!await deployServerClient.AddProcess(Properties.Settings.Default.ADServiceNamePrefix, processName, startCommandLineArgs, true, document.Title))
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsADServerStarting = false;
                        }
                        return;
                    }
                }

                processName = "dotnet";

                var ADServerArguments = String.Format("\"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\" \"-SrvrConfigId={9}\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.ADServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath, ADServerConfigurationID);
                
                var commandLineArgs = String.Format
                (
                    "\"{0}{1}/{2}\" {3}",
                    DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                    Properties.Settings.Default.ServerIODataServerRootFolder,
                    Properties.Settings.Default.CheckedAssemblyName_ADServer,
                    ADServerArguments
                );
                var ret = await deployServerClient.AddProcess(Properties.Settings.Default.ADServiceNamePrefix, processName, commandLineArgs, true, document.Title);
                if (!ret)
                {
                    uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                    vm.IsADServerStarting = false;
                }
            }
        }

        private async void Button_RemoveProcess_ADServer_Click(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                if (!await deployServerClient.RemoveProces(Properties.Settings.Default.ADServiceNamePrefix, document.Title, true))
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
            }
        }

        private async void Button_AddProcess_Browser_Click(object sender, RoutedEventArgs e)
        {
            if (targetMachineRuntimeInfo == null)
                return;

            var process = targetMachineRuntimeInfo.IsWindows ? "explorer" : targetMachineRuntimeInfo.IsLinux ? Properties.Settings.Default.LinuxWebHMIBrowserCommand : "open";

            using (var cursor = new WaitCursor())
            {
                vm.IsBrowserStarting = true;
                BoolTaskResult ret;

                ret = await deployServerClient.StartBrowser(
                        Properties.Settings.Default.WebBrowserProcessName,
                        process,
                        Properties.Settings.Default.WebHMIServerUrls.Split(';').First(),
                        document.Title,
                        Properties.Settings.Default.ServerWebHMIRootFolder
                    );
                if (!ret.Success)
                {
                    if (ret.SoftFail)
                    {
                        var bRet = await deployServerClient.AddProcess(
                            Properties.Settings.Default.WebBrowserProcessName,
                            process,
                            Properties.Settings.Default.WebHMIServerUrls.Split(';').First(),
                            false,
                            document.Title
                        );
                        if (!bRet)
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsBrowserStarting = false;
                        }
                    }
                    else
                    {
                        uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                        vm.IsBrowserStarting = false;
                    }
                }
            }
        }

        private async void Button_RemoveProcess_Browser_Click(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var ret = await deployServerClient.RemoveProces(Properties.Settings.Default.WebBrowserProcessName, document.Title);
            }
        }

        private async Task GetConnextPath()
        {
            try
            {
                connextPath = await deployServerClient.GetWinConnextMoviconPath(Properties.Settings.Default.IODataServerProcessName, Properties.Settings.Default.ConNextSubKey, Properties.Settings.Default.ConNextKeyName, Properties.Settings.Default.VersionKeyName);
                //if (connextPath != null)
                //    serviceManagerPath = String.Format("{0}\\{1}", Path.GetDirectoryName(connextPath), Properties.Settings.Default.ServiceManagerExecutable);
            }
            catch { }
        }

        private async Task GetMoviconPath()
        {
            try
            {
                moviconPath = await deployServerClient.GetWinConnextMoviconPath(Properties.Settings.Default.IODataServerProcessName, Properties.Settings.Default.MovNextSubKey, Properties.Settings.Default.MovNextKeyName, Properties.Settings.Default.VersionKeyName);
                //if (moviconPath != null)
                //    serviceManagerPath = String.Format("{0}\\{1}", Path.GetDirectoryName(moviconPath), Properties.Settings.Default.ServiceManagerExecutable);
            }
            catch { }
        }

        private async void Button_AddProcess_DataServer_Click(object sender, RoutedEventArgs e)
        {
            if (targetMachineRuntimeInfo == null)
                return;

            string processName = null;

            using (var cursor = new WaitCursor())
            {
                vm.IsDataServerStarting = true;
                if (targetMachineRuntimeInfo.IsWindows)
                {
                    if (String.IsNullOrEmpty(moviconPath))
                        await GetMoviconPath();
                    if (!String.IsNullOrEmpty(moviconPath))
                        processName = moviconPath;
                    else
                    {
                        if (String.IsNullOrEmpty(connextPath))
                            await GetConnextPath();
                        if (!String.IsNullOrEmpty(connextPath))
                            processName = connextPath;
                    }
                    if (!String.IsNullOrEmpty(processName))
                    {
                        var startCommandLineArgs = String.Format("-noservice \"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\" \"-DocPath={9}/Documents\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UFUAServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath, projectRoot);

                        var ret = await deployServerClient.AddProcess(Properties.Settings.Default.ServerServiceNamePrefix, processName, startCommandLineArgs, true, document.Title);
                        if (!ret)
                        {
                            uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                            vm.IsDataServerStarting = false;
                        }
                        return;
                    }
                    //else
                    //{
                    //    uiInterface.ShowError(Properties.Resources.MoviconInstallationNotFound);
                    //    return;
                    //}
                }

                if (String.IsNullOrEmpty(processName))
                {
                    processName = "dotnet";

                    var IOServerArguments = String.Format("\"-conn=XpoProvider={0};data source={1}/{2};\" \"-StringConn=XpoProvider={3};data source={4}/{5};\" \"-UserConn=XpoProvider={6};data source={7}/{8};\"", Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UFUAServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.StringServerFilePath, Properties.Settings.Default.XpoMemoryProvider, projectPath, Properties.Settings.Default.UsersServerFilePath);

                    var commandLineArgs = String.Format
                    (
                        "\"{0}{1}/{2}\" {3}",
                        DeployServer.Utils.PlaceHolders.DeployRootPathPlaceholder,
                        Properties.Settings.Default.ServerIODataServerRootFolder,
                        Properties.Settings.Default.CheckedAssemblyName_IODataServer,
                        IOServerArguments
                    );
                    var ret = await deployServerClient.AddProcess(Properties.Settings.Default.ServerServiceNamePrefix, processName, commandLineArgs, true, document.Title);
                    if (!ret)
                    {
                        uiInterface.ShowError(Properties.Resources.FailedStartingRemoteProcess);
                        vm.IsDataServerStarting = false;
                    }
                }
            }
        }

        private async void Button_RemoveProcess_DataServer_Click(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                if (!await deployServerClient.RemoveProces(Properties.Settings.Default.ServerServiceNamePrefix, document.Title, true))
                    uiInterface.ShowError(Properties.Resources.FailedStoppingRemoteProcess);
            }
        }

        private void OnSaveProfiles(object sender, RoutedEventArgs e)
        {
            var name = textboxProfileName.EditValue as string;
            if (String.IsNullOrEmpty(name))
                return;

            if (vm.ActiveProfile == null)
                AddNewProfile(name, textboxHost.Text, textboxUser.Text, textboxPassword.Text);

            vm.ActiveProfile.Name = name;

            PersistProfiles();
        }

        void PersistProfiles(bool bSilent = false)
        {
            try
            {
                var profilesFile = Path.Combine(String.Format("{0}.{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), Utilities.AssemblyInfo.FileFormatMainVersion), Properties.Settings.Default.DeployProfilesFileName);
                using (FileStream writer = new FileStream(profilesFile, FileMode.Create))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(DeployProfiles));
                    ser.WriteObject(writer, vm.DeployProfiles);
                }
                vm.DeployProfiles.bNeedsSave = false;
                if (!bSilent)
                    uiInterface.ShowInformation(Properties.Resources.DeployProfilesSaved);
            }
            catch
            {
                uiInterface.ShowError(String.Format(Properties.Resources.ErrorSavingProfiles, Properties.Settings.Default.DeployProfilesFileName));
            }
        }

        void LoadSavedProfiles()
        {
            try
            {
                var profilesFile = Path.Combine(String.Format("{0}.{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), Utilities.AssemblyInfo.FileFormatMainVersion), Properties.Settings.Default.DeployProfilesFileName);
                if (!File.Exists(profilesFile))
                {
                    profilesFile = Path.Combine(Utilities.ApplicationPropertiesHelper.GetProperty("CommonFolder").ToString(), Properties.Settings.Default.DeployProfilesFileName);
                    if (!File.Exists(profilesFile))
                        return;
                }
                
                using (var fs = new FileStream(profilesFile, FileMode.Open))
                {
                    using (var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(DeployProfiles));
                        vm.DeployProfiles = (DeployProfiles)ser.ReadObject(reader, true);
                    }
                }
            }
            catch
            {
                uiInterface.ShowError(String.Format(Properties.Resources.ErrorLoadingProfiles, Properties.Settings.Default.DeployProfilesFileName));
            }
            finally
            {
                if (vm.DeployProfiles == null || vm.DeployProfiles.Profiles.Count == 0)
                    vm.DeployProfiles = new DeployProfiles(new DeployClientProfile("DefaultProfile"));
                else
                    vm.DeployProfiles.Sort();
                var currentProfile = vm.DeployProfiles.Profiles.First();
                var lastProfileName = ApplicationPropertiesHelper.GetProperty("LastConnectedDeployProfile") as string;
                if (!String.IsNullOrEmpty(lastProfileName))
                {
                    var lastProfile = (from p in vm.DeployProfiles.Profiles where p.Name == lastProfileName select p).FirstOrDefault();
                    if (lastProfile != null)
                        currentProfile = lastProfile;
                }
                vm.ActiveProfile = currentProfile;
            }
        }

        private void OnDeleteProfile(object sender, RoutedEventArgs e)
        {
            if (vm.ActiveProfile != null && uiInterface.ShowYesNo(String.Format(Properties.Resources.DeleteProfileConfirmation, vm.ActiveProfile.Name), CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                var newIndex = vm.DeployProfiles.Profiles.IndexOf(vm.ActiveProfile) - 1;
                vm.DeployProfiles.Profiles.Remove(vm.ActiveProfile);
                ProfilesCombo.SelectedIndex = Math.Max(newIndex, 0);
            }
        }

        //private void OnProcessNewValue(DependencyObject sender, DevExpress.Xpf.Editors.ProcessNewValueEventArgs e)
        //{
        //    if (vm.ActiveProfile == null)
        //        return;

        //    var oldName = vm.ActiveProfile.Name;
        //    vm.ActiveProfile.Name = e.DisplayText;
        //    if (vm.ActiveProfile["Name"] != null)
        //        vm.ActiveProfile.Name = oldName;
        //}

        private void OnAddProfile(object sender, RoutedEventArgs e)
        {
            AddNewProfile(PickNextValidNewName());
        }

        void AddNewProfile(string name, string host = "", string user = "", string password = "")
        {
            var newProfile = new DeployClientProfile(name, host, user, password);
            vm.DeployProfiles.Profiles.Add(newProfile);
            ProfilesCombo.SelectedItem = newProfile;
        }

        string PickNextValidNewName()
        {
            var newName = "NewProfile";
            var i = 1;
            while (vm.DeployProfiles.FindProfileByName(newName) != null)
            {
                newName = String.Format("NewProfile{0}", i);
                i++;
            }
            return newName;
        }

        private void OnValidateName(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            vm.IsInError = false;
            if (vm.ActiveProfile == null || (sender as DevExpress.Xpf.Editors.TextEdit).EditValue == null)
                return;

            vm.ActiveProfile.Name = e.Value?.ToString() ?? String.Empty;
            var error = vm.ActiveProfile["Name"];

            if (!String.IsNullOrEmpty(error))
            {
                vm.IsInError = true;
                e.SetError(error);
                e.IsValid = false;
            }
        }

        private async void checkRemoteLicense_Click(object sender, RoutedEventArgs e)
        {
            var layoutcontrol = new MSZui.UserControl3(false, true);
            var dialog = new GeneralDialogContent(layoutcontrol, GeneralDialogButtons.CloseHelpButtons)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.RemoteLicenseReaderTitle,
                HelpLink = "DongleOption"
            };
            try
            {
                deployServerClient.LoadLicenseData(layoutcontrol.LicenseModel.OptionsTags, licenseFolders).ContinueWith(model =>
                {
                    layoutcontrol.DataContext = model.Result;
                    layoutcontrol.UpdateUI();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                uiInterface.ShowError(Properties.Resources.WebHMIErrorGettingRemoteLicenseInfo);
                return;
            }
            dialog.ShowDialog();
        }

        private async void installLicense_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".mlztrack";
            dlg.Filter = "MLZTrack Files|*.mlztrack";
            var result = dlg.ShowDialog();

            if (result == true)
            {
                if (await deployServerClient.FileExists(dlg.SafeFileName, Properties.Settings.Default.RemoteLicenseFileName, licenseFolders))
                    if (uiInterface.ShowYesNo(Properties.Resources.OverwriteLicenseFile, CustomDialogIcons.Question) != CustomDialogResults.Yes)
                        return;

                try
                {
                    await deployServerClient.UploadLicense(dlg.FileName, Properties.Settings.Default.RemoteLicenseFileName, Properties.Settings.Default.WebHMIMaxUploadChunkSize, licenseFolders);
                }
                catch (Exception ex)
                {
                    uiInterface.ShowError(String.Format(Properties.Resources.RemoteLicenseInstallationFailed, ex.Message));
                    return;
                }
                uiInterface.ShowInformation(Properties.Resources.RemoteLicenseInstallationSuccess);
                checkRemoteLicense_Click(null, null);
            }
        }

        private void OnProcessSelectionChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            if (!bUpdatingSource)
                selectedProcessName = (e.NewItem as ProcessData)?.name;
            else if (selectedProcessName == null && dataGrid.SelectedItem != null)
                selectedProcessName = (dataGrid.SelectedItem as ProcessData)?.name;

            processOutputButton.IsEnabled = e.NewItem as ProcessData != null;
        }

        private void OnProcessOutputButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedProcessData = dataGrid.SelectedItem as ProcessData;
            if (selectedProcessData == null)
                return;

            processOutputControl = new ProcessOutput(selectedProcessData.processOutput);
            var dialog = new GeneralDialogContent(processOutputControl, GeneralDialogButtons.CloseButton)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = selectedProcessData.name
            };
            dialog.ShowDialog();
        }

        private async void OnComboSelectionChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            var combo = (DevExpress.Xpf.Editors.ComboBoxEdit)sender;
            if (vm.IsConnected)
            {
                if (uiInterface.ShowYesNo(Properties.Resources.ConfirmProfileDisconnection, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    await Disconnect();
                else
                {
                    e.IsCancel = true;
                    e.Handled = true;
                }
            }
        }
        private async void btnScanProjects_Click(object sender, RoutedEventArgs e)
        {
            if (!await deployServerClient.DirectoryExist(Properties.Settings.Default.ServerProjectsRootFolder))
            {
                btnDeleteRemoteProject.IsEnabled = false;
                vm.CanScanProjects = false;
                return;
            }

            vm.DownloadStatusDisplayString = null;
            vm.IsDownloading = true;

            try
            {
                GeneralDialogContent dialog;
                RemoteProjectsList remoteProjectsList;
                using (var cursor = new WaitCursor())
                {
                    var projectsList = await deployServerClient.ScanRemoteProjects(Properties.Settings.Default.ServerProjectsRootFolder);
                    if (projectsList.Count == 0)
                    {
                        uiInterface.ShowInformation(Properties.Resources.NoRemoteProjectsFound);
                        return;
                    }
                    remoteProjectsList = new RemoteProjectsList(projectsList, document.Title);
                    dialog = new GeneralDialogContent(remoteProjectsList)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.RemoteProjectsPopupTitle
                    };
                }

                if (remoteProjectsList != null && dialog.ShowDialog() == true)
                {
                    var projectTitle = remoteProjectsList.GetSelectedProject();
                    if (!String.IsNullOrEmpty(projectTitle))
                    {
                        var projectDir = String.Format("{0}/{1}", Properties.Settings.Default.ServerProjectsRootFolder, projectTitle);
                        await ZipAndDownloadProject(projectDir, projectTitle);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.DownloadStatusDisplayString = Properties.Resources.DownloadInterrupted;
                uiInterface.ShowError(String.Format(Properties.Resources.RemoteScanFailed, ex.Message));
                return;
            }
            finally
            {
                vm.IsDownloading = false;
            }
        }

        private async Task ZipAndDownloadProject(string projectDir, string zipName)
        {
            if (!await deployServerClient.DirectoryExist(projectDir))
            {
                if (projectDir == remoteProjectDirectory)
                    btnDeleteRemoteProject.IsEnabled = false;
                uiInterface.ShowInformation(String.Format(Properties.Resources.RemoteProjectNotFound, zipName));
                return;
            }

            vm.DownloadStatusDisplayString = null;
            vm.IsDownloading = true;

            using (var cursor = new WaitCursor())
            {
                try
                {
                    var zipFileInfo = await deployServerClient.CreateProjectZipFile(projectDir);
                    var zipFileName = zipFileInfo.FullName;
                    var zipFileLength = zipFileInfo.Length;

                    int percentage = 0;
                    int offset = 0;
                    byte[] lastChunk;
                    List<byte> fileChunks = new List<byte>();
                    do
                    {
                        lastChunk = await deployServerClient.DownloadFileChunk(zipFileName, 3000, offset);
                        offset += lastChunk.Length;
                        fileChunks.AddRange(lastChunk);
                        percentage = (int)(offset * 100 / zipFileLength);
                        vm.TotalFilesDownloadProgress = Math.Min(percentage, 100);
                    }
                    while (lastChunk.Length != 0);

                    var finalZipPath = Path.Combine(projectDownloadPath, String.Format("{0}.zip", zipName));
                    var i = 0;
                    while (File.Exists(finalZipPath))
                    {
                        zipName = String.Format("{0}_{1}", zipName, ++i);
                        finalZipPath = Path.Combine(projectDownloadPath, String.Format("{0}.zip", zipName));
                    }
                    File.WriteAllBytes(finalZipPath, fileChunks.ToArray());
                    vm.DownloadStatusDisplayString = Properties.Resources.DownloadCompleted;
                    uiInterface.ShowInformation(String.Format(Properties.Resources.ProjectArchiveDownloaded, finalZipPath));
                }
                catch (Exception ex)
                {
                    vm.DownloadStatusDisplayString = Properties.Resources.DownloadInterrupted;
                    uiInterface.ShowError(String.Format(Properties.Resources.RemoteProjectDownloadFailed, ex.Message));
                    return;
                }
                finally
                {
                    vm.IsDownloading = false;
                }
            }
        }

        private void btnDownloadProjectPath_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.SelectedPath = String.Format("{0}\\", projectDownloadPath);
            if (dialog.ShowDialog() == true)
                SetProjectDownloadLocalPath(dialog.SelectedPath);
        }

        void GetProjectDownloadLocalPath()
        {
            projectDownloadPath = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
            try
            {
                var storage = GetStorage();
                if (storage != null)
                {
                    if (storage.FileExists(downloadPathFilename))
                    {
                        using (var stream = new IsolatedStorageFileStream(downloadPathFilename, FileMode.Open, storage))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                var savedPath = reader.ReadToEnd();
                                if (Directory.Exists(savedPath))
                                    projectDownloadPath = savedPath;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        void SetProjectDownloadLocalPath(string path)
        {
            if (path == projectDownloadPath)
                return;

            projectDownloadPath = path;
            try
            {
                var storage = GetStorage();
                if (storage != null)
                {
                    using (var stream = new IsolatedStorageFileStream(downloadPathFilename, FileMode.Create, storage))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(path);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
