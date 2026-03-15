using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.IO;
using EmptyProjectWizardPlugin.ComponentService;
using VFS;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Windows.Media;
using UFUAEditor.Document;
using System.Text;
using WPFUtilities.PropertyDataTemplate;

namespace EmptyProjectWizardPlugin
{
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class ProjectWizardPathAndType : UserControl, IWizardElement
    {
        public string ConnectionString { get; set; }
        public string ProjectName = string.Empty;
        string staticPath = string.Empty;
        string dynamicPath = string.Empty;

        public ProjectWizardPathAndType()
        {
            InitializeComponent();
            EmptyProjectWizardPluginComponent.NewProject.ProjectName = string.Empty;
            EmptyProjectWizardPluginComponent.NewProject.ProjectPath = EmptyProjectWizardPluginComponent.StartingFolder;
            textBoxProjectName.IsEnabled = true;
            staticPath = EmptyProjectWizardPluginComponent.StartingFolder;
            dynamicPath = EmptyProjectWizardPluginComponent.StartingDBFolder;
            this.DataContext = EmptyProjectWizardPluginComponent.NewProject;

            textBoxProjectPath.Text = EmptyProjectWizardPluginComponent.StartingFolder;
            InitLabels();
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            projectName.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectName);
            projectFolder.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectFolder);
            projectLocation.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectLocation);
            Static.Content = cultInfo.ToTitleCase(Properties.Resources.UseFolder);
            Dynamic.Content = cultInfo.ToTitleCase(Properties.Resources.UseConnectionWizard);
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var path = String.IsNullOrEmpty(staticPath) ? ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder") : staticPath;
            //var path = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
            path = string.Format("{0}\\", path);

            try
            {
                String fileName = EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowBrowseFolderDialog(path);
                if (!(String.IsNullOrEmpty(fileName)))
                {
                    textBoxProjectPath.Text = EmptyProjectWizardPluginComponent.StartingFolder = staticPath = fileName;
                    EmptyProjectWizardPluginComponent.NewProject.ProjectPath = fileName;
                }
            }
            catch (Exception ex)
            {
                EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
            }
        }
        public bool Execute()
        {
            try
            {
                string ProjectFilePath = string.Empty;
                string ProjectPath = string.Empty;

                //if(!CheckConsistency(out ProjectFilePath, out ProjectName))
                //    return Execute(ProjectFilePath, ProjectName);

                ProjectName = EmptyProjectWizardPluginComponent.NewProject.ProjectName;
                string ProjectFolder = EmptyProjectWizardPluginComponent.NewProject.ProjectPath;
                string newProjectName = string.Empty;

                if (!EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
                {
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        newProjectName = String.Format("{0}1", (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                        ProjectName = newProjectName;
                        int i = 1;
                        while (System.IO.File.Exists(ProjectFilePath))
                        {
                            i += 1;
                            newProjectName = String.Format("{0}{1}", (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, i);
                            ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                            ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                            ProjectName = newProjectName;
                        }
                    }
                    else
                    {
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, ProjectName, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, ProjectName);
                    }

                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        ProjectPath, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileName, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileType);


                    if (System.IO.File.Exists(ProjectFilePath))
                    {
                        var ret = EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowYesNo(String.Format(Properties.Resources.WarningPathExistent), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Warning);
                        if (ret == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                        {
                            var projectInfo = WizardPluginHelpers.Helper.GetNewPath(ProjectFolder, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                            ProjectName = projectInfo.Name;
                            ProjectFilePath = projectInfo.FilePath;
                            ProjectPath = projectInfo.Folder;
                        }
                    }

                    ConnectionString = InMemoryDataStore.GetConnectionString(xmlfile);
                }
                else
                {
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString })
                        {
                            if (FindVFSProjectName(fileSystemProvider))
                                return true;
                        }
                    }
                    ProjectFilePath = string.Format("{0}", ConnectionString);
                }

                EmptyProjectWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                EmptyProjectWizardPluginComponent.NewProject.ProjectName = ProjectName;

                Uri _uri;
                if (ProjectFilePath.StartsWith("\\"))
                    _uri = new Uri(ProjectFilePath);
                else
                    _uri = _uri = new Uri(String.Format("{0}:{1}", EmptyProjectWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                EmptyProjectWizardPluginComponent.ProjectUri = (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);

                CreateUFUAServerConfiguration(ProjectPath);

#if !CONNEXT
                CreateSchedulerConfiguration(ProjectPath);
                CreateRecipeConfiguration(ProjectPath);
                CreateADConfiguration(ProjectPath);
#endif
                return false;
            }
            catch (Exception ex)
            {
                EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        private void CreateUFUAServerConfiguration(string projectpath)
        {
            if (!EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
            {
                var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        projectpath, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileName, (EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileType);

                ConnectionString = InMemoryDataStore.GetConnectionString(xmlfile);
            }

            using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                //if (EmptyProjectWizardPluginComponent.NewProject.Architecture != ArchType.local)
                {
                    using (UnitOfWork uow = new UnitOfWork(dl))
                    {
                        UFUAServerDocument doc = (UFUAServerDocument)(EmptyProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).GetDocument(EmptyProjectWizardPluginComponent.ProjectUri);
                        UFUAModel.UFUAConfiguration configuration;
                        var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            configuration = new UFUAModel.UFUAConfiguration(uow);
                        else
                            configuration = list[0];

                        var title = EmptyProjectWizardPluginComponent.NewProject.ProjectName;
                        if (XpoHelpers.XpoHelper.IsSQlDataProvider(ConnectionString))
                            title = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString, onlytitle: true);
                        configuration.EnsureDefaultSettings($"{title}_{Properties.Settings.Default.AppNameSuffix}");

                        if (EmptyProjectWizardPluginComponent.NewProject.Architecture != ArchType.local)
                        {
                            var cba = configuration.BaseAddresses.ToList();
                            foreach (UFUAModel.AddressBase ab in cba)
                            {
                                ab.Delete();
                            }
                            if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                            {
                                configuration.RedundancyFullSynchronizationStartTime = EmptyProjectWizardPluginComponent.NewProject.RedundancyFullSynchronizationStartTime;
                                configuration.RedundancyFullSynchronizationTimeSpan = EmptyProjectWizardPluginComponent.NewProject.RedundancyFullSynchronizationTimeSpan;
                                EmptyProjectWizardPluginComponent.NewProject.Server = Properties.Settings.Default.DefaultServerName;
                            }
                            else if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.distributed &&
                                EmptyProjectWizardPluginComponent.NewProject.Server != Properties.Settings.Default.DefaultServerName)
                            {
                                string connection = configuration.AuditTraceDefaultConnection;
                                configuration.AuditTraceDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, EmptyProjectWizardPluginComponent.NewProject.Server);
                                connection = configuration.EventDefaultConnection;
                                configuration.EventDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, EmptyProjectWizardPluginComponent.NewProject.Server);
                                connection = configuration.HistorianDefaultConnection;
                                configuration.HistorianDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, EmptyProjectWizardPluginComponent.NewProject.Server);
                            }
                            var ba = new UFUAModel.UFUABaseAddress(uow)
                            {
                                Enabled = true,
                                Transport = EmptyProjectWizardPluginComponent.NewProject.Transport,
                                Server = EmptyProjectWizardPluginComponent.NewProject.Server,
                                Port = EmptyProjectWizardPluginComponent.NewProject.Port
                            };
                            configuration.BaseAddresses.Add(ba);

                            if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                            {
                                var server = new StringBuilder();
                                for (int i = 0; i < EmptyProjectWizardPluginComponent.NewProject.ServerList.Count; ++i)
                                {
                                    if (server.Length > 0)
                                        server.Append(",");
                                    server.Append(EmptyProjectWizardPluginComponent.NewProject.ServerList[i]);
                                }
                                configuration.ListRedundancyServers = server.ToString();
                                var netPipeBa = new UFUAModel.UFUABaseAddress(uow)
                                {
                                    Enabled = true,
                                    Transport = Opc.Ua.Utils.UriSchemeNetPipe,
                                    Server = Properties.Settings.Default.DefaultServerName
                                };
                                configuration.BaseAddresses.Add(netPipeBa);
                            }
                            else
                                configuration.ListRedundancyServers = null;
                        }

                        var aa = new UFUAModel.UFUAArea(uow) { Name = Properties.Resources.AlarmAreaName, NodeId = Guid.NewGuid() };
                        var aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = Properties.Resources.AlarmSourceName, NodeId = Guid.NewGuid() };
                        if (aa != null)
                            aa.UFUAAlarmSources.Add(aSource);

                        uow.CommitChanges();
                    };
                }
            }
        }

        private void CreateSchedulerConfiguration(string projectpath)
        {
            if (!EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
            { 
                var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        projectpath, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.SchedulerEditorManager as IDocumentManager).TypeLabel, (EmptyProjectWizardPluginComponent.ProjectView.SchedulerEditorManager as IDocumentManager).FileName, (EmptyProjectWizardPluginComponent.ProjectView.SchedulerEditorManager as IDocumentManager).FileType);

                ConnectionString = InMemoryDataStore.GetConnectionString(xmlfile);
            }
            using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork uow = new UnitOfWork(dl))
                {
                    MSModel.MSGeneralSettings configuration;
                    var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                    if (list.Count == 0)
                        configuration = new MSModel.MSGeneralSettings(uow);
                    else
                        configuration = list[0];

                    configuration.EnsureDefaultSettings($"{EmptyProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.SchedulerAppNameSuffix}");
                    if (EmptyProjectWizardPluginComponent.NewProject.Architecture != ArchType.local)
                    {
                        var cba = configuration.BaseAddresses.ToList();
                        foreach (UFUAModel.AddressBase ab in cba)
                        {
                            ab.Delete();
                        }

                        var ba = new MSModel.MSBaseAddress(uow)
                        {
                            Enabled = true,
                            Transport = EmptyProjectWizardPluginComponent.NewProject.Transport,
                            Server = EmptyProjectWizardPluginComponent.NewProject.Server,
                            Port = configuration.GetDefaultPort(EmptyProjectWizardPluginComponent.NewProject.Transport) //EmptyProjectWizardPluginComponent.NewProject.Port
                        };
                        configuration.BaseAddresses.Add(ba);

                        if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                        {
                            var server = new StringBuilder();
                            for (int i = 0; i < EmptyProjectWizardPluginComponent.NewProject.ServerList.Count; ++i)
                            {
                                if (server.Length > 0)
                                    server.Append(",");
                                server.Append(EmptyProjectWizardPluginComponent.NewProject.ServerList[i]);
                            }
                            configuration.ListRedundancyServers = server.ToString();
                            var netPipeBa = new MSModel.MSBaseAddress(uow)
                            {
                                Enabled = true,
                                Transport = Opc.Ua.Utils.UriSchemeNetPipe,
                                Server = Properties.Settings.Default.DefaultServerName
                            };
                            configuration.BaseAddresses.Add(netPipeBa);
                        }
                        else
                            configuration.ListRedundancyServers = null;
                    }
                    uow.CommitChanges();
                };
            }
        }

        private void CreateRecipeConfiguration(string projectpath)
        {
            if (!EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
            {
                var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        projectpath, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.recipeEditorManager as IDocumentManager).TypeLabel, (EmptyProjectWizardPluginComponent.ProjectView.recipeEditorManager as IDocumentManager).FileName, UFRecipeSettings.Properties.Settings.Default.DefaultUAFileExt);

                ConnectionString = InMemoryDataStore.GetConnectionString(xmlfile);
            }
            using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork uow = new UnitOfWork(dl))
                {
                    UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration configuration;
                    var list = (from tag in new XPQuery<UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                    if (list.Count == 0)
                        configuration = new UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration(uow);
                    else
                        configuration = list[0];

                    configuration.EnsureDefaultSettings($"{EmptyProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.RecipeAppNameSuffix}");

                    if (EmptyProjectWizardPluginComponent.NewProject.Architecture != ArchType.local)
                    {
                        var cba = configuration.BaseAddresses.ToList();
                        foreach (UFRecipeSettings.UFRecipeModel.RecipeUABaseAddress ab in cba)
                        {
                            ab.Delete();
                        }

                        var ba = new UFRecipeSettings.UFRecipeModel.RecipeUABaseAddress(uow)
                        {
                            Enabled = true,
                            Transport = EmptyProjectWizardPluginComponent.NewProject.Transport,
                            Server = EmptyProjectWizardPluginComponent.NewProject.Server,
                            Port = configuration.GetDefaultPort(EmptyProjectWizardPluginComponent.NewProject.Transport) //EmptyProjectWizardPluginComponent.NewProject.Port
                        };
                        configuration.BaseAddresses.Add(ba);

                        if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                        {
                            var server = new StringBuilder();
                            for (int i = 0; i < EmptyProjectWizardPluginComponent.NewProject.ServerList.Count; ++i)
                            {
                                if (server.Length > 0)
                                    server.Append(",");
                                server.Append(EmptyProjectWizardPluginComponent.NewProject.ServerList[i]);
                            }
                            configuration.ListRedundancyServers = server.ToString();
                            var netPipeBa = new UFRecipeSettings.UFRecipeModel.RecipeUABaseAddress(uow)
                            {
                                Enabled = true,
                                Transport = Opc.Ua.Utils.UriSchemeNetPipe,
                                Server = Properties.Settings.Default.DefaultServerName
                            };
                            configuration.BaseAddresses.Add(netPipeBa);
                        }
                        else
                            configuration.ListRedundancyServers = null;
                    }
                    uow.CommitChanges();
                };
            }
        }

        private void CreateADConfiguration(string projectpath)
        {
            if(EmptyProjectWizardPluginComponent.ProjectView.ADEditorManager != null)
            { 
                if (!EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
                {
                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                            projectpath, ProjectName, (EmptyProjectWizardPluginComponent.ProjectView.ADEditorManager as IDocumentManager).TypeLabel, (EmptyProjectWizardPluginComponent.ProjectView.ADEditorManager as IDocumentManager).FileName, (EmptyProjectWizardPluginComponent.ProjectView.ADEditorManager as IDocumentManager).FileType);

                    ConnectionString = InMemoryDataStore.GetConnectionString(xmlfile);
                }
                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (UnitOfWork uow = new UnitOfWork(dl))
                    {
                        ADModel.ADGeneralSettings configuration;
                        var list = (from tag in new XPQuery<ADModel.ADGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            configuration = new ADModel.ADGeneralSettings(uow);
                        else
                            configuration = list[0];

                        configuration.EnsureDefaultSettings($"{EmptyProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.ADAppNameSuffix}");

                        if (EmptyProjectWizardPluginComponent.NewProject.Architecture != ArchType.local)
                        {
                            var cba = configuration.BaseAddresses.ToList();
                            foreach (ADModel.ADBaseAddress ab in cba)
                            {
                                ab.Delete();
                            }

                            var ba = new ADModel.ADBaseAddress(uow)
                            {
                                Enabled = true,
                                Transport = EmptyProjectWizardPluginComponent.NewProject.Transport,
                                Server = EmptyProjectWizardPluginComponent.NewProject.Server,
                                Port = configuration.GetDefaultPort(EmptyProjectWizardPluginComponent.NewProject.Transport)
                            };
                            configuration.BaseAddresses.Add(ba);

                            if (EmptyProjectWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                            {
                                var server = new StringBuilder();
                                for (int i = 0; i < EmptyProjectWizardPluginComponent.NewProject.ServerList.Count; ++i)
                                {
                                    if (server.Length > 0)
                                        server.Append(",");
                                    server.Append(EmptyProjectWizardPluginComponent.NewProject.ServerList[i]);
                                }
                                configuration.ListRedundancyServers = server.ToString();
                                var netPipeBa = new ADModel.ADBaseAddress(uow)
                                {
                                    Enabled = true,
                                    Transport = Opc.Ua.Utils.UriSchemeNetPipe,
                                    Server = Properties.Settings.Default.DefaultServerName
                                };
                                configuration.BaseAddresses.Add(netPipeBa);
                            }
                            else
                                configuration.ListRedundancyServers = null;
                        }
                        uow.CommitChanges();
                    };
                }
            }
        }

        void EmptyPath(DirectoryInfo directory)
        {
            try
            {
                foreach (System.IO.FileInfo file in directory.GetFiles())
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }
            }
            catch
            {
            }
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
            {
                EmptyPath(subDirectory);
                try
                {
                    subDirectory.Delete(true);
                }
                catch
                {
                }
            }
        }

        public bool CheckConsistency()
        {
            var ProjectFilePath = string.Empty;
            ProjectName = EmptyProjectWizardPluginComponent.NewProject.ProjectName;
            if (String.IsNullOrEmpty(ProjectName))
            {
                ProjectName = String.Format("{0}1", (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
            }

            try
            {

                string ProjectFolder = EmptyProjectWizardPluginComponent.NewProject.ProjectPath;

                bool result = false;
                using (new WaitCursor())
                {
                    if (EmptyProjectWizardPluginComponent.NewProject.IsDynamic)
                    {
                        var name = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString);
                        if (String.IsNullOrEmpty(name))
                        {
                            EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.InvalidConnectionString);
                            return true;
                        }
                        if (String.IsNullOrEmpty(ProjectName))
                        {
                            using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString })
                            {
                                var fmfPath = new FileManagerFolder(fileSystemProvider, EmptyProjectWizardPluginComponent.wizard.StratingVFSFolder);

                                if (fileSystemProvider.Exists(fmfPath))
                                {
                                    EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string extension = (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType;
                        if (!System.IO.Directory.Exists(ProjectFolder))
                            System.IO.Directory.CreateDirectory(ProjectFolder);
                        string[] files = System.IO.Directory.GetFiles(ProjectFolder, string.Format("*{0}", extension), System.IO.SearchOption.TopDirectoryOnly);

                        if ((files.Length == 1 && !files[0].Equals(System.IO.Path.Combine(ProjectFolder, string.Format("{0}{1}", ProjectName, extension)))) ||
                                files.Length > 1)
                        {
                            EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        public bool Execute(String ProjectFilePath, String ProjectName)
        {
            try
            {
                var _uri = new Uri(String.Format("{0}:{1}", EmptyProjectWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    EmptyProjectWizardPluginComponent.ProjectUri = (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);
                    EmptyProjectWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                    EmptyProjectWizardPluginComponent.NewProject.ProjectName = ProjectName;
                }

                return false;
            }
            catch (Exception ex)
            {
                EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        bool FindVFSProjectName(DataSourceFileSystemProvider fileSystemProviderBase)
        {
            string newProjectName = string.Empty;

            var fmfPath = new FileManagerFolder(fileSystemProviderBase, EmptyProjectWizardPluginComponent.wizard.StratingVFSFolder);

            if (fileSystemProviderBase.Exists(fmfPath))
            {
                EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                return true;
            }
            else
            {
                try
                {
                    fileSystemProviderBase.CreateFolder(null, EmptyProjectWizardPluginComponent.wizard.StratingVFSFolder);
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        int i = 0;
                        do
                        {
                            newProjectName = String.Format("{0}{1}", (EmptyProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, ++i);

                        } while ((from c in fmfPath.GetFiles()
                                  where c.RelativeName == newProjectName
                                  select c).ToList().Count != 0);

                        ProjectName = newProjectName;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    EmptyProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                    return true;
                }
            }

            return false;
        }

        private void btnUseConnectionWizard_Click(object sender, RoutedEventArgs e)
        {
            string connection = dynamicPath;
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "EmptyWizard_ConnectionWizard", this.FindParent<Window>(), EmptyProjectWizardPluginComponent.ProjectView.UIInterface, EmptyProjectWizardPluginComponent.ProjectView.helpProvider))
            {
                EmptyProjectWizardPluginComponent.StartingDBFolder = EmptyProjectWizardPluginComponent.NewProject.ProjectPath = dynamicPath = ConnectionString = connection;
                textBoxProjectPath.Text = EmptyProjectWizardPluginComponent.NewProject.ProjectPath;
            }
            else
            {
                if (EmptyProjectWizardPluginComponent.NewProject.ProjectPath.Equals(string.Empty))
                {
                    EmptyProjectWizardPluginComponent.NewProject.IsDynamic = false;
                }
            }
        }

        private void Dynamic_Checked(object sender, RoutedEventArgs e)
        {
            EmptyProjectWizardPluginComponent.NewProject.ProjectPath = dynamicPath;
            ConnectionString = dynamicPath;
            textBoxProjectPath.Text = EmptyProjectWizardPluginComponent.NewProject.ProjectPath;
            textBoxProjectName.IsEnabled = false;
        }

        private void Static_Checked(object sender, RoutedEventArgs e)
        {
            EmptyProjectWizardPluginComponent.NewProject.ProjectPath = staticPath;
            textBoxProjectPath.Text = EmptyProjectWizardPluginComponent.NewProject.ProjectPath;
            textBoxProjectName.IsEnabled = true;
        }

        private void Static_Checked_1(object sender, RoutedEventArgs e)
        {
            if ((bool)Static.IsChecked)
                Static_Checked(null, null);
            else
                Dynamic_Checked(null, null);
        }
    }
}
