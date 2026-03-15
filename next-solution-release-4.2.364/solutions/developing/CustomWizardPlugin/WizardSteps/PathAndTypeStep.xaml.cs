using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.IO;
using CustomWizardPlugin.ComponentService;
using VFS;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Windows.Media;
using UFUAEditor.Document;
using UFUAModel;
using System.ComponentModel;
using System.Text;

namespace CustomWizardPlugin
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
            CustomWizardPluginComponent.NewProject.ProjectName = string.Empty;
            CustomWizardPluginComponent.NewProject.ProjectPath = CustomWizardPluginComponent.StartingFolder;
            textBoxProjectName.IsEnabled = true;
            staticPath = CustomWizardPluginComponent.StartingFolder;
            dynamicPath = CustomWizardPluginComponent.StartingDBFolder;
            this.DataContext = CustomWizardPluginComponent.NewProject;

            textBoxProjectPath.Text = CustomWizardPluginComponent.StartingFolder;
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
                String fileName = CustomWizardPluginComponent.ProjectView.UIInterface.ShowBrowseFolderDialog(path);
                if (!(String.IsNullOrEmpty(fileName)))
                {
                    textBoxProjectPath.Text = CustomWizardPluginComponent.StartingFolder = staticPath = fileName;
                    CustomWizardPluginComponent.NewProject.ProjectPath = fileName;
                }
            }
            catch (Exception ex)
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
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

                ProjectName = CustomWizardPluginComponent.NewProject.ProjectName;
                string ProjectFolder = CustomWizardPluginComponent.NewProject.ProjectPath;
                string newProjectName = string.Empty;
                DataSourceFileSystemProvider fileSystemProvider = null;

                if (!CustomWizardPluginComponent.NewProject.UseFileSystemProvider)
                {
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        newProjectName = String.Format("{0}1", (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                        ProjectName = newProjectName;
                        int i = 1;
                        while (System.IO.File.Exists(ProjectFilePath))
                        {
                            i += 1;
                            newProjectName = String.Format("{0}{1}", (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, i);
                            ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                            ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                            ProjectName = newProjectName;
                        }
                    }
                    else
                    {
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, ProjectName, ProjectName, (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, ProjectName);
                    }

                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        ProjectPath, ProjectName, (CustomWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel, (CustomWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileName, (CustomWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileType);


                    if (System.IO.File.Exists(ProjectFilePath))
                    {
                        var ret = CustomWizardPluginComponent.ProjectView.UIInterface.ShowYesNo(String.Format(Properties.Resources.WarningPathExistent), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Warning);
                        if (ret == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                        {
                            var projectInfo = WizardPluginHelpers.Helper.GetNewPath(ProjectFolder, ProjectName, (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
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
                        fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString };
                        if (FindVFSProjectName(fileSystemProvider))
                            return true;
                    }
                    ProjectFilePath = string.Format("{0}", ConnectionString);
                }

                CustomWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                CustomWizardPluginComponent.NewProject.ProjectName = ProjectName;

                Uri _uri;
                if (ProjectFilePath.StartsWith("\\"))
                    _uri = new Uri(ProjectFilePath);
                else
                    _uri = _uri = new Uri(String.Format("{0}:{1}", CustomWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));

                CustomWizardPluginComponent.ProjectUri = (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);


                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (UnitOfWork uow = new UnitOfWork(dl))
                    {
                        UFUAServerDocument doc = (UFUAServerDocument)(CustomWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).GetDocument(CustomWizardPluginComponent.ProjectUri);
                        UFUAModel.UFUAConfiguration configuration;
                        var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            configuration = new UFUAModel.UFUAConfiguration(uow);
                        else
                            configuration = list[0];

                        var title = CustomWizardPluginComponent.NewProject.ProjectName;
                        if (XpoHelpers.XpoHelper.IsSQlDataProvider(ConnectionString))
                            title = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString, onlytitle: true);
                        configuration.EnsureDefaultSettings($"{title}_{Properties.Settings.Default.AppNameSuffix}");

                        if (CustomWizardPluginComponent.NewProject.Architecture != ArchType.local)
                        {
                            var cba = configuration.BaseAddresses.ToList();
                            foreach (UFUAModel.AddressBase ab in cba)
                            {
                                ab.Delete();
                            }
                            if (CustomWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                            {
                                configuration.RedundancyFullSynchronizationStartTime = CustomWizardPluginComponent.NewProject.RedundancyFullSynchronizationStartTime;
                                configuration.RedundancyFullSynchronizationTimeSpan = CustomWizardPluginComponent.NewProject.RedundancyFullSynchronizationTimeSpan;
                                CustomWizardPluginComponent.NewProject.Server = Properties.Settings.Default.DefaultServerName;
                            }
                            else if (CustomWizardPluginComponent.NewProject.Architecture == ArchType.distributed &&
                                CustomWizardPluginComponent.NewProject.Server != Properties.Settings.Default.DefaultServerName)
                            {
                                string connection = configuration.AuditTraceDefaultConnection;
                                configuration.AuditTraceDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, CustomWizardPluginComponent.NewProject.Server);
                                connection = configuration.EventDefaultConnection;
                                configuration.EventDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, CustomWizardPluginComponent.NewProject.Server);
                                connection = configuration.HistorianDefaultConnection;
                                configuration.HistorianDefaultConnection = connection.Replace(Properties.Settings.Default.XpoDefaultConnectionServer, CustomWizardPluginComponent.NewProject.Server);
                            }

                            var ba = new UFUAModel.UFUABaseAddress(uow)
                            {
                                Enabled = true,
                                Transport = CustomWizardPluginComponent.NewProject.Transport,
                                Server = CustomWizardPluginComponent.NewProject.Server,
                                Port = CustomWizardPluginComponent.NewProject.Port
                            };
                            configuration.BaseAddresses.Add(ba);

                            if (CustomWizardPluginComponent.NewProject.Architecture == ArchType.redundancy)
                            {
                                var server = new StringBuilder();
                                for (int i = 0; i < CustomWizardPluginComponent.NewProject.ServerList.Count; ++i)
                                {
                                    if (server.Length > 0)
                                        server.Append(",");
                                    server.Append(CustomWizardPluginComponent.NewProject.ServerList[i]);
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

                        uow.CommitChanges();
                    };
                }

                return false;
            }
            catch (Exception ex)
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
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
            ProjectName = CustomWizardPluginComponent.NewProject.ProjectName;
            if (String.IsNullOrEmpty(ProjectName))
            {
                ProjectName = String.Format("{0}1", (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
            }

            try
            {

                string ProjectFolder = CustomWizardPluginComponent.NewProject.ProjectPath;
                
                bool result = false;
                using (new WaitCursor())
                {
                    if (CustomWizardPluginComponent.NewProject.UseFileSystemProvider)
                    {
                        var name = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString);
                        if (String.IsNullOrEmpty(name))
                        {
                            CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.InvalidConnectionString);
                            return true; 
                        }
                        if (String.IsNullOrEmpty(ProjectName))
                        {
                            using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString })
                            {
                                var fmfPath = new FileManagerFolder(fileSystemProvider, CustomWizardPluginComponent.wizard.StratingVFSFolder);

                                if (fileSystemProvider.Exists(fmfPath))
                                {
                                    CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string extension = (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType;

                        if (!System.IO.Directory.Exists(ProjectFolder))
                            System.IO.Directory.CreateDirectory(ProjectFolder);

                        string[] files = System.IO.Directory.GetFiles(ProjectFolder, string.Format("*{0}", extension), System.IO.SearchOption.TopDirectoryOnly);

                        if ((files.Length == 1 && !files[0].Equals(System.IO.Path.Combine(ProjectFolder, string.Format("{0}{1}", ProjectName, extension)))) || 
                                files.Length > 1)
                        {
                            CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        public bool Execute(String ProjectFilePath, String ProjectName)
        {
            try
            {
                var _uri = new Uri(String.Format("{0}:{1}", CustomWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {

                    CustomWizardPluginComponent.ProjectUri = (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);
                    CustomWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                    CustomWizardPluginComponent.NewProject.ProjectName = ProjectName;
                }

                return false;
            }
            catch (Exception ex)
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        bool FindVFSProjectName(DataSourceFileSystemProvider fileSystemProviderBase)
        {
            string newProjectName = string.Empty;

            var fmfPath = new FileManagerFolder(fileSystemProviderBase, CustomWizardPluginComponent.wizard.StratingVFSFolder);

            if (fileSystemProviderBase.Exists(fmfPath))
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                return true;
            }
            else
            {
                try
                {
                    fileSystemProviderBase.CreateFolder(null, CustomWizardPluginComponent.wizard.StratingVFSFolder);
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        int i = 0;
                        do
                        {
                            newProjectName = String.Format("{0}{1}", (CustomWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, ++i);

                        } while ((from c in fmfPath.GetFiles()
                                  where c.RelativeName == newProjectName
                                  select c).ToList().Count != 0);

                        ProjectName = newProjectName;
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                    return true;
                }
            }

            return false;
        }

        private void btnUseConnectionWizard_Click(object sender, RoutedEventArgs e)
        {
            string connection = dynamicPath;
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "CustomWizard_ConnectionWizard", this.FindParent<Window>(), CustomWizardPluginComponent.ProjectView.UIInterface, CustomWizardPluginComponent.ProjectView.helpProvider))
            {
                CustomWizardPluginComponent.StartingDBFolder = CustomWizardPluginComponent.NewProject.ProjectPath = dynamicPath = ConnectionString = connection;
                textBoxProjectPath.Text = CustomWizardPluginComponent.NewProject.ProjectPath;
            }
            else
            {
                if (CustomWizardPluginComponent.NewProject.ProjectPath.Equals(string.Empty))
                {
                    CustomWizardPluginComponent.NewProject.UseFileSystemProvider = false;
                }
            }
        }

        private void Dynamic_Checked(object sender, RoutedEventArgs e)
        {
            CustomWizardPluginComponent.NewProject.ProjectPath = dynamicPath;
            ConnectionString = dynamicPath;
            textBoxProjectPath.Text = CustomWizardPluginComponent.NewProject.ProjectPath;
            textBoxProjectName.IsEnabled = false;
        }

        private void Static_Checked(object sender, RoutedEventArgs e)
        {
            CustomWizardPluginComponent.NewProject.ProjectPath = staticPath;
            textBoxProjectPath.Text = CustomWizardPluginComponent.NewProject.ProjectPath;
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
