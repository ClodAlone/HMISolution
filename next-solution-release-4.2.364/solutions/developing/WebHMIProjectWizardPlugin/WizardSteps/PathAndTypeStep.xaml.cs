using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.IO;
using WebHMIProjectWizardPlugin.ComponentService;
using VFS;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Windows.Media;
using UFUAEditor.Document;
using System.Text;
using UFProjectManager;

namespace WebHMIProjectWizardPlugin
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
            WebHMIProjectWizardPluginComponent.NewProject.ProjectName = string.Empty;
            WebHMIProjectWizardPluginComponent.NewProject.ProjectPath = WebHMIProjectWizardPluginComponent.StartingFolder;
            textBoxProjectName.IsEnabled = true;
            staticPath = WebHMIProjectWizardPluginComponent.StartingFolder;
            dynamicPath = WebHMIProjectWizardPluginComponent.StartingDBFolder;
            this.DataContext = WebHMIProjectWizardPluginComponent.NewProject;

            textBoxProjectPath.Text = WebHMIProjectWizardPluginComponent.StartingFolder;
            InitLabels();

            WebHMIProjectWizardPluginComponent.NewProject.RedundancyFullSynchronizationStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            WebHMIProjectWizardPluginComponent.NewProject.RedundancyFullSynchronizationTimeSpan = new TimeSpan();
            WebHMIProjectWizardPluginComponent.NewProject.Transport = Opc.Ua.Utils.UriSchemeOpcTcp;
            WebHMIProjectWizardPluginComponent.NewProject.Server = Properties.Settings.Default.DefaultServerName;
            WebHMIProjectWizardPluginComponent.NewProject.Port = Properties.Settings.Default.DefaultOpcTcpPort;
            WebHMIProjectWizardPluginComponent.NewProject.Architecture = ArchType.distributed;
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            projectName.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectName);
            projectFolder.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectFolder);
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var path = String.IsNullOrEmpty(staticPath) ? ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder") : staticPath;
            //var path = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
            path = string.Format("{0}\\", path);

            try
            {
                String fileName = WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowBrowseFolderDialog(path);
                if (!(String.IsNullOrEmpty(fileName)))
                {
                    textBoxProjectPath.Text = WebHMIProjectWizardPluginComponent.StartingFolder = staticPath = fileName;
                    WebHMIProjectWizardPluginComponent.NewProject.ProjectPath = fileName;
                }
            }
            catch (Exception ex)
            {
                WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
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

                ProjectName = WebHMIProjectWizardPluginComponent.NewProject.ProjectName;
                string ProjectFolder = WebHMIProjectWizardPluginComponent.NewProject.ProjectPath;
                string newProjectName = string.Empty;

                if (!WebHMIProjectWizardPluginComponent.NewProject.IsDynamic)
                {
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        newProjectName = String.Format("{0}1", (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                        ProjectName = newProjectName;
                        int i = 1;
                        while (System.IO.File.Exists(ProjectFilePath))
                        {
                            i += 1;
                            newProjectName = String.Format("{0}{1}", (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, i);
                            ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                            ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName); 
                            ProjectName = newProjectName;
                        }
                    }
                    else
                    {
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, ProjectName, ProjectName, (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, ProjectName);
                    }

                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        ProjectPath, ProjectName, (WebHMIProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel, (WebHMIProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileName, (WebHMIProjectWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileType);

                    
                    if (System.IO.File.Exists(ProjectFilePath))
                    {
                        var ret = WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowYesNo(String.Format(Properties.Resources.WarningPathExistent),UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Warning);
                        if (ret == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                        {
                            var projectInfo = WizardPluginHelpers.Helper.GetNewPath(ProjectFolder, ProjectName, (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
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

                WebHMIProjectWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                WebHMIProjectWizardPluginComponent.NewProject.ProjectName = ProjectName;

                Uri _uri;
                if (ProjectFilePath.StartsWith("\\"))
                    _uri = new Uri(ProjectFilePath);
                else
                    _uri = _uri = new Uri(String.Format("{0}:{1}", WebHMIProjectWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                WebHMIProjectWizardPluginComponent.ProjectUri = (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);

                var doc = (UFProjectDocument)(WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).GetDocument(WebHMIProjectWizardPluginComponent.ProjectUri);
                doc.ProjectType = ProjectType.WebHMI.ToString();
                doc.SaveToFile();

                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (UnitOfWork uow = new UnitOfWork(dl))
                    {
                        UFUAModel.UFUAConfiguration configuration;
                        var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            configuration = new UFUAModel.UFUAConfiguration(uow);
                        else
                            configuration = list[0];

                        configuration.EnsureDefaultSettings($"{WebHMIProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.AppNameSuffix}");

                        var cba = configuration.BaseAddresses.ToList();
                        foreach (UFUAModel.AddressBase ab in cba)
                        {
                            ab.Delete();
                        }

                        string serverConnection = SQLiteConnectionProvider.GetConnectionString(
                                                        $"{doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString()}{WebHMIProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.AppNameSuffix}{Properties.Settings.Default.SQLiteExtension}");

                        string auditConnection = SQLiteConnectionProvider.GetConnectionString(
                                                        $"{doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString()}{WebHMIProjectWizardPluginComponent.NewProject.ProjectName}_{Properties.Settings.Default.AuditNameSuffix}{Properties.Settings.Default.SQLiteExtension}");

                        configuration.AuditTraceDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(auditConnection, doc.rootBase);
                        configuration.EventDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(serverConnection, doc.rootBase);
                        configuration.HistorianDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(serverConnection, doc.rootBase);

                        if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditLogItem>(serverConnection))
                            WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.UnableToOpenDatabaseConnection);
                        if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(serverConnection))
                            WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.UnableToOpenDatabaseConnection);
                        if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(auditConnection))
                            WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.UnableToOpenDatabaseConnection);

                        var ba = new UFUAModel.UFUABaseAddress(uow)
                        {
                            Enabled = true,
                            Transport = WebHMIProjectWizardPluginComponent.NewProject.Transport,
                            Server = WebHMIProjectWizardPluginComponent.NewProject.Server,
                            Port = WebHMIProjectWizardPluginComponent.NewProject.Port
                        };
                        configuration.BaseAddresses.Add(ba);
                        configuration.ListRedundancyServers = null;

                        var aa = new UFUAModel.UFUAArea(uow) { Name = Properties.Resources.AlarmAreaName, NodeId = Guid.NewGuid() };
                        var aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = Properties.Resources.AlarmSourceName, NodeId = Guid.NewGuid() };
                        if (aa != null)
                            aa.UFUAAlarmSources.Add(aSource);

                        uow.CommitChanges();
                    };
                }

                return false;    
            }
            catch (Exception ex)
            {
                WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;    
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
            ProjectName = WebHMIProjectWizardPluginComponent.NewProject.ProjectName;
            if (String.IsNullOrEmpty(ProjectName))
            {
                ProjectName = String.Format("{0}1", (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
            }

            try
            {

                string ProjectFolder = WebHMIProjectWizardPluginComponent.NewProject.ProjectPath;
                
                bool result = false;
                using (new WaitCursor())
                {
                    if (WebHMIProjectWizardPluginComponent.NewProject.IsDynamic)
                    {
                        var name = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString);
                        if (String.IsNullOrEmpty(name))
                        {
                            WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.InvalidConnectionString);
                            return true;
                        }
                        if (String.IsNullOrEmpty(ProjectName))
                        {
                            using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString })
                            {
                                var fmfPath = new FileManagerFolder(fileSystemProvider, WebHMIProjectWizardPluginComponent.wizard.StratingVFSFolder);

                                if (fileSystemProvider.Exists(fmfPath))
                                {
                                    WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string extension = (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType;
                        if (!System.IO.Directory.Exists(ProjectFolder))
                            System.IO.Directory.CreateDirectory(ProjectFolder);
                        string[] files = System.IO.Directory.GetFiles(ProjectFolder, string.Format("*{0}", extension), System.IO.SearchOption.TopDirectoryOnly);

                        if ((files.Length == 1 && !files[0].Equals(System.IO.Path.Combine(ProjectFolder, string.Format("{0}{1}", ProjectName, extension)))) || 
                                files.Length > 1)
                        {
                            WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        public bool Execute(String ProjectFilePath, String ProjectName)
        {
            try
            {
                var _uri = new Uri(String.Format("{0}:{1}", WebHMIProjectWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    WebHMIProjectWizardPluginComponent.ProjectUri = (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);
                    WebHMIProjectWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                    WebHMIProjectWizardPluginComponent.NewProject.ProjectName = ProjectName;
                }

                return false;
            }
            catch (Exception ex)
            {
                WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        bool FindVFSProjectName(DataSourceFileSystemProvider fileSystemProviderBase)
        {
            string newProjectName = string.Empty;

            var fmfPath = new FileManagerFolder(fileSystemProviderBase, WebHMIProjectWizardPluginComponent.wizard.StratingVFSFolder);

            if (fileSystemProviderBase.Exists(fmfPath))
            {
                WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                return true;
            }
            else
            {
                try
                {
                    fileSystemProviderBase.CreateFolder(null, WebHMIProjectWizardPluginComponent.wizard.StratingVFSFolder);
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        int i = 0;
                        do
                        {
                            newProjectName = String.Format("{0}{1}", (WebHMIProjectWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, ++i);

                        } while ((from c in fmfPath.GetFiles()
                                  where c.RelativeName == newProjectName
                                  select c).ToList().Count != 0);

                        ProjectName = newProjectName;
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    WebHMIProjectWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                    return true;
                }
            }

            return false;
        }
    }
}
