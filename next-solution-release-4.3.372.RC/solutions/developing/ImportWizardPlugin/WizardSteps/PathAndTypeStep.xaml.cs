using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.IO;
using ImportWizardPlugin.ComponentService;
using VFS;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Windows.Media;
using UFUAEditor.Document;
using UFUAModel;
using System.Reflection;
using System.Xml;

namespace ImportWizardPlugin
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
            ImportWizardPluginComponent.NewProject.ProjectName = string.Empty;
            ImportWizardPluginComponent.NewProject.ProjectPath = ImportWizardPluginComponent.StartingFolder;
            textBoxProjectName.IsEnabled = true;
            staticPath = ImportWizardPluginComponent.StartingFolder;
            dynamicPath = ImportWizardPluginComponent.StartingDBFolder;
            this.DataContext = ImportWizardPluginComponent.NewProject;

            textBoxProjectPath.Text = ImportWizardPluginComponent.StartingFolder;
            InitLabels();
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            sourceFileTitle.Content = cultInfo.ToTitleCase(Properties.Resources.SourceFileTitle);
            projectName.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectName);
            projectFolder.Content = cultInfo.ToTitleCase(Properties.Resources.ProjectFolder);
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
                String fileName = ImportWizardPluginComponent.ProjectView.UIInterface.ShowBrowseFolderDialog(path);
                if (!(String.IsNullOrEmpty(fileName)))
                {
                    textBoxProjectPath.Text = ImportWizardPluginComponent.StartingFolder = staticPath = fileName;
                    ImportWizardPluginComponent.NewProject.ProjectPath = fileName;
                }
            }
            catch (Exception ex)
            {
                ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
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

                ProjectName = ImportWizardPluginComponent.NewProject.ProjectName;
                string ProjectFolder = ImportWizardPluginComponent.NewProject.ProjectPath;
                string newProjectName = string.Empty;
                DataSourceFileSystemProvider fileSystemProvider = null;

                if (!ImportWizardPluginComponent.NewProject.IsDynamic)
                {
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        newProjectName = String.Format("{0}1", (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                        ProjectName = newProjectName;
                        int i = 1;
                        while (System.IO.File.Exists(ProjectFilePath))
                        {
                            i += 1;
                            newProjectName = String.Format("{0}{1}", (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, i);
                            ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, newProjectName, newProjectName, (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                            ProjectPath = string.Format("{0}\\{1}", ProjectFolder, newProjectName);
                            ProjectName = newProjectName;
                        }
                    }
                    else
                    {
                        ProjectFilePath = string.Format("{0}\\{1}\\{2}{3}", ProjectFolder, ProjectName, ProjectName, (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
                        ProjectPath = string.Format("{0}\\{1}", ProjectFolder, ProjectName);
                    }

                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        ProjectPath, ProjectName, (ImportWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel, (ImportWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileName, (ImportWizardPluginComponent.ProjectView.UFUAEditorManager as IDocumentManager).FileType);


                    if (System.IO.File.Exists(ProjectFilePath))
                    {
                        var ret = ImportWizardPluginComponent.ProjectView.UIInterface.ShowYesNo(String.Format(Properties.Resources.WarningPathExistent), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Warning);
                        if (ret == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                        {
                            var projectInfo = WizardPluginHelpers.Helper.GetNewPath(ProjectFolder, ProjectName, (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType);
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

                ImportWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                ImportWizardPluginComponent.NewProject.ProjectName = ProjectName;

                Uri _uri;
                if (ProjectFilePath.StartsWith("\\"))
                    _uri = new Uri(ProjectFilePath);
                else
                    _uri = _uri = new Uri(String.Format("{0}:{1}", ImportWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                ImportWizardPluginComponent.ProjectUri = (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);


                using(var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using(var uow = new UnitOfWork(dl))
                    {
                        UFUAModel.UFUAConfiguration configuration;
                        var _conf = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).FirstOrDefault();
                        if (_conf == null)
                            configuration = new UFUAModel.UFUAConfiguration(uow);
                        else
                            configuration = _conf;

                        var title = ProjectName;
                        if (XpoHelpers.XpoHelper.IsSQlDataProvider(ConnectionString))
                            title = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString, onlytitle: true);
                        configuration.EnsureDefaultSettings(String.Format("{0}_{1}", title, Properties.Settings.Default.AppNameSuffix));

                        var aa = new UFUAModel.UFUAArea(uow) { Name = Properties.Resources.AlarmAreaName, NodeId = Guid.NewGuid() };
                        var aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = Properties.Resources.AlarmSourceName, NodeId = Guid.NewGuid() };
                        if (aa != null)
                            aa.UFUAAlarmSources.Add(aSource);

                        uow.CommitChanges();
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
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
            ProjectName = ImportWizardPluginComponent.NewProject.ProjectName;
            if (String.IsNullOrEmpty(ProjectName))
            {
                ProjectName = String.Format("{0}1", (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName);
            }

            try
            {

                string ProjectFolder = ImportWizardPluginComponent.NewProject.ProjectPath;

                bool result = false;
                using (new WaitCursor())
                {
                    if (ImportWizardPluginComponent.NewProject.IsDynamic)
                    {
                        var name = XpoHelpers.XpoHelper.GetDataSourceTitle(ConnectionString);
                        if (String.IsNullOrEmpty(name))
                        {
                            ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.InvalidConnectionString);
                            return true;
                        }
                        if (String.IsNullOrEmpty(ProjectName))
                        {
                            using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = ConnectionString })
                            {
                                var fmfPath = new FileManagerFolder(fileSystemProvider, ImportWizardPluginComponent.wizard.StratingVFSFolder);

                                if (fileSystemProvider.Exists(fmfPath))
                                {
                                    ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string extension = (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileType;
                        if (!System.IO.Directory.Exists(ProjectFolder))
                            System.IO.Directory.CreateDirectory(ProjectFolder);
                        string[] files = System.IO.Directory.GetFiles(ProjectFolder, string.Format("*{0}", extension), System.IO.SearchOption.TopDirectoryOnly);

                        if ((files.Length == 1 && !files[0].Equals(System.IO.Path.Combine(ProjectFolder, string.Format("{0}{1}", ProjectName, extension)))) ||
                                files.Length > 1)
                        {
                            ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        public bool Execute(String ProjectFilePath, String ProjectName)
        {
            try
            {
                var _uri = new Uri(String.Format("{0}:{1}", ImportWizardPluginComponent.ProjectView.UriRisolver.GetOpenFileScheme(), ProjectFilePath));
                using (var dl = XpoDefault.GetDataLayer(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    ImportWizardPluginComponent.ProjectUri = (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).CreateNewDocument(_uri, null);
                    ImportWizardPluginComponent.NewProject.ProjectPath = ProjectFilePath;
                    ImportWizardPluginComponent.NewProject.ProjectName = ProjectName;
                }

                return false;
            }
            catch (Exception ex)
            {
                ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
        }

        bool FindVFSProjectName(DataSourceFileSystemProvider fileSystemProviderBase)
        {
            string newProjectName = string.Empty;

            var fmfPath = new FileManagerFolder(fileSystemProviderBase, ImportWizardPluginComponent.wizard.StratingVFSFolder);

            if (fileSystemProviderBase.Exists(fmfPath))
            {
                ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ErrorSaveAsCannotSharePath);
                return true;
            }
            else
            {
                try
                {
                    fileSystemProviderBase.CreateFolder(null, ImportWizardPluginComponent.wizard.StratingVFSFolder);
                    if (String.IsNullOrEmpty(ProjectName))
                    {
                        int i = 0;
                        do
                        {
                            newProjectName = String.Format("{0}{1}", (ImportWizardPluginComponent.ProjectView.projectManagerService as IDocumentManager).FileName, ++i);

                        } while ((from c in fmfPath.GetFiles()
                                  where c.RelativeName == newProjectName
                                  select c).ToList().Count != 0);

                        ProjectName = newProjectName;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                    return true;
                }
            }


            return false;
        }

        private void btnUseConnectionWizard_Click(object sender, RoutedEventArgs e)
        {
            string connection = dynamicPath;
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "ConnectionWizard", this.FindParent<Window>(), ImportWizardPluginComponent.ProjectView.UIInterface, ImportWizardPluginComponent.ProjectView.helpProvider))
            {
                ImportWizardPluginComponent.StartingDBFolder = ImportWizardPluginComponent.NewProject.ProjectPath = dynamicPath = ConnectionString = connection;
                textBoxProjectPath.Text = ImportWizardPluginComponent.NewProject.ProjectPath;
            }
            else
            {
                if (ImportWizardPluginComponent.NewProject.ProjectPath.Equals(string.Empty))
                {
                    ImportWizardPluginComponent.NewProject.IsDynamic = false;
                }
            }
        }

        private void Dynamic_Checked(object sender, RoutedEventArgs e)
        {
            ImportWizardPluginComponent.NewProject.ProjectPath = dynamicPath;
            ConnectionString = dynamicPath;
            textBoxProjectPath.Text = ImportWizardPluginComponent.NewProject.ProjectPath;
            textBoxProjectName.IsEnabled = false;
        }

        private void Static_Checked(object sender, RoutedEventArgs e)
        {
            ImportWizardPluginComponent.NewProject.ProjectPath = staticPath;
            textBoxProjectPath.Text = ImportWizardPluginComponent.NewProject.ProjectPath;
            textBoxProjectName.IsEnabled = true;
        }

        private void Static_Checked_1(object sender, RoutedEventArgs e)
        {
            if ((bool)Static.IsChecked)
                Static_Checked(null, null);
            else
                Dynamic_Checked(null, null);
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Uri value = ImportWizardPluginComponent.NewProject.SourceProject;

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = Properties.Resources.SourceFileTitle;
            // Set filter for file extension and default file extension
            dlg.DefaultExt = Properties.Resources.DeaultExtension;
            dlg.Filter = Properties.Resources.Filter;
            dlg.ValidateNames = false;
            dlg.CheckPathExists = false;
            if (value != null)
            {
                if (value.IsAbsoluteUri && value.IsFile)
                {
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(value.LocalPath);
                    dlg.FileName = System.IO.Path.GetFileName(value.LocalPath);
                }
                else
                {
                    dlg.FileName = System.IO.Path.GetFileName(value.OriginalString);
                }
            }

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                ImportWizardPluginComponent.NewProject.SourceProject = new Uri(dlg.FileName);
            }
        }
        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            ImportWizardPluginComponent.NewProject.SourceProject = null;
        }
    }

}
