using DocumentManager.ComponentService;
using log4net;
using StartupWelcome.ComponentService;
using StartupWelcome.Controls;
using StartupWelcome.Model;
using StartupWelcome.View_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using UFProjectWizard.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.WPF;
using static StartupWelcome.Controls.WorkspaceLayoutManager;

namespace StartupWelcome
{
    /// <summary>
    /// Helper class object used in the startup button's link.
    /// </summary>
    public class ButtonHelper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public BitmapImage Image { get; set; }
        public Action Action { get; set; }
    }
    /// <summary>
    /// Interaction logic for StartupWelcomeUI.xaml
    /// </summary>
    public partial class StartupWelcomeUI : UserControl/*, IDisposable (keep this comment)*/
    {

        StartupWelcomeComponent startupWelcomeComponent;
        Dictionary<ButtonHelper, String> mapWizardEntities = new Dictionary<ButtonHelper, String>();
        bool bLoaded;
        bool bScreenManagerLoaded;
        RecentRepository Recent = StartupWelcomeComponent.startupWelcomeComponent.Recent;
        ObservableCollection<string> listStatics = new ObservableCollection<string>();
        HelpInfo HelpDoc = new HelpInfo();
        object lockfile = new object();
        DispatcherOperation dpInitUI;
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.HelpManager);
        System.Globalization.TextInfo cultInfo;
        int nMaxUri = Properties.Settings.Default.MaxRecentFileNumber;
        const String fmtUri = "Recent{0}";
        string startingFolder = string.Empty;
        bool bDownPressed;

        public StartupWelcomeUI(StartupWelcomeComponent c)
        {
            InitializeComponent();

            startupWelcomeComponent = c;

            var title = GetValue(AutomationProperties.AutomationIdProperty) as String; // can be used in the control
            var storageName = String.Format("{0}_{1}", title, Name);

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        bScreenManagerLoaded = startupWelcomeComponent.ScreenManager != null;
                        startingFolder = String.Format("{0}", AppDomain.CurrentDomain.BaseDirectory);
                        dpInitUI = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            InitMainPage();
                            InitWizardList(startingFolder);
                            InitSupport();

                            RecentMenuHelper.LoadRecentFileList(recentProjects, Recent);
                        });
                    }
                };
        }

        private void recentProjects_Popup(object sender, EventArgs e)
        {
            RecentMenuHelper.LoadRecentFileList(recentProjects, Recent);
        }

        private void InitMainPage()
        {
            cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            createNewProjectTxt.Text = cultInfo.ToTitleCase(Properties.Resources.CreateNewProject);
            supportInstrumentsTxt.Text = cultInfo.ToTitleCase(Properties.Resources.SupportInstruments);
        }

        private void InitSupport()
        {
            List<ButtonHelper> list = new List<ButtonHelper>();
            if(Properties.Settings.Default.ShowSupportButton)
                list.Add(new ButtonHelper() { Name = "supportBtn", Description = cultInfo.ToTitleCase(Properties.Resources.SupportHeader), Image = StartupWelcomeComponent.GetControlImage("SWSupport"), Action = () => { Tile_Support(); } });
            if (Properties.Settings.Default.ShowHelpButton)
                list.Add(new ButtonHelper() { Name = "helpBtn", Description = cultInfo.ToTitleCase(Properties.Resources.HelpHeader), Image = StartupWelcomeComponent.GetControlImage("SWHelpOnLine"), Action = () => { Tile_Help(); } });
            if (Properties.Settings.Default.ShowCommunityButton)
                list.Add(new ButtonHelper() { Name = "communityBtn", Description = cultInfo.ToTitleCase(Properties.Resources.CommunityHeader), Image = StartupWelcomeComponent.GetControlImage("SWCommunity"), Action = () => { Tile_Community(); } });
            if (Properties.Settings.Default.ShowExampleButton)
                list.Add(new ButtonHelper() { Name = "exampleBtn", Description = cultInfo.ToTitleCase(Properties.Resources.ExamplesHeader), Image = StartupWelcomeComponent.GetControlImage("SWExampleProjects"), Action = () => { Tile_ExampleProjects(); } });
            if (Properties.Settings.Default.ShowTutorialButton)
                list.Add(new ButtonHelper() { Name = "tutorialBtn", Description = cultInfo.ToTitleCase(Properties.Resources.TutorialHeader), Image = StartupWelcomeComponent.GetControlImage("SWNews"), Action = () => { Tile_Tutorial(); } });
            if (Properties.Settings.Default.ShowWebinarButton)
                list.Add(new ButtonHelper() { Name = "webinarBtn", Description = cultInfo.ToTitleCase(Properties.Resources.WebinarHeader), Image = StartupWelcomeComponent.GetControlImage("SWWebinar"), Action = () => { Tile_Webinar(); } });
            if (Properties.Settings.Default.ShowKnowledgebaseButton)
                list.Add(new ButtonHelper() { Name = "knowledgebaseBtn", Description = cultInfo.ToTitleCase(Properties.Resources.KnowledgebaseHeader), Image = StartupWelcomeComponent.GetControlImage("SWKnowledgebase"), Action = () => { Tile_Knowledgebase(); } });
            listBoxSupportInstruments.ItemsSource = list;
        }

        private void InitWizardList(string Path)
        {
            List<ButtonHelper> list = new List<ButtonHelper>();
            list.Add(new ButtonHelper() { Name = "browseBtn", Description = cultInfo.ToTitleCase(Properties.Resources.OpenExistingProject), Image = StartupWelcomeComponent.GetControlImage("OpenFolderMaxi", true), Action = () => { FileOpen(null); } });
            list.Add(new ButtonHelper() { Name = "browseBataBtn", Description = cultInfo.ToTitleCase(Properties.Resources.OpenExistingDBProject), Image = StartupWelcomeComponent.GetControlImage("Data", true), Action = () => { FileOpen(null, useFileSystem:false); } });
            if (Directory.Exists(Path))
            {
                list.AddRange(GetDllList());
            }
            listBoxPlugins.ItemsSource = list;
        }

        public List<ButtonHelper> GetDllList()
        {
            String extension = Properties.Settings.Default.PluginsExtension;
            String folder = String.Format("{0}{1}\\", startingFolder, Properties.Settings.Default.PluginsFolder);
            var filter = String.Format("*{0}", extension);
            string[] directoryGetFiles = Directory.GetFiles(folder, filter);

            List<ButtonHelper> list = new List<ButtonHelper>();
            if (startupWelcomeComponent.ProjectWizard != null)
                foreach (var fileOn in directoryGetFiles)
                {
                    var filename = Path.GetFileNameWithoutExtension(fileOn);
                    if (!bScreenManagerLoaded && (filename == Properties.Settings.Default.ProLeanAssembly || filename == Properties.Settings.Default.ProEnergyAssembly))
                        continue;

                    FileInfo file = new FileInfo(fileOn);
                    if (file.Extension.Equals(Properties.Settings.Default.PluginsExtension))
                    {
                        var filePng = System.IO.Path.ChangeExtension(fileOn, "png");
                        BitmapImage img = new BitmapImage();
                        if (!File.Exists(filePng))
                            img = StartupWelcomeComponent.GetControlImage("SWWizardEditor");
                        else
                        {
                            img.BeginInit();
                            img.UriSource = new Uri(filePng, UriKind.RelativeOrAbsolute);
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.EndInit();
                        }


                        Image image = new Image() { Source = img, Stretch = Stretch.Uniform, Width = 64, Height = 64 };
                        var name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));

                        var fileDesc = GetFileDescription(fileOn);
                        string desc = string.Empty;
                        if (!string.IsNullOrEmpty(fileDesc))
                            desc = fileDesc;
                        else
                            desc = name;

                        var listDll = FindAndLoadDLL.LoadDLLs<IUFProjectWizardPlugin>(folder,
                                                    string.Format("{0}{1}", filename, ".dll"), false);

                        if (listDll.Count > 0 && listDll[0].IsStartable)
                        {
                            var button = new ButtonHelper()
                            {
                                Name = name,
                                Description = cultInfo.ToTitleCase(desc),
                                Image = img,
                                Action = () => { OpenWizard(); }
                            };
                            if (!mapWizardEntities.ContainsKey(button))
                                mapWizardEntities.Add(button, fileOn);
                            list.Add(button);
                        }
                    }
                }
            return list;
        }

        const String dataExt = "dat";
        private string GetFileDescription(String _file)
        {
            String _description = string.Empty;
            try
            {
                String fileName = System.IO.Path.GetFileNameWithoutExtension(_file);
                String fileSettings = System.IO.Path.ChangeExtension(_file, dataExt);
                String localizedFile = $"{System.IO.Path.GetDirectoryName(_file)}\\{System.Globalization.CultureInfo.CurrentUICulture.Name}\\{fileName}.{dataExt}";
                if (System.IO.File.Exists(localizedFile))
                    fileSettings = localizedFile;
                _description = System.IO.File.ReadAllText(fileSettings);
            }
            catch (Exception ex)
            {
                _description = System.IO.Path.GetFileNameWithoutExtension(_file);
            }

            return _description;
        }

        private void OpenWizard()
        {
            object selectedItem = listBoxPlugins.SelectedItem;
            if (selectedItem == null)
                return;
            if (mapWizardEntities.ContainsKey(selectedItem as ButtonHelper))
            {
                String fileName = mapWizardEntities[selectedItem as ButtonHelper];
                var assemblyPath = System.IO.Path.GetDirectoryName(fileName);
                var assemblyName = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileName));
                OpenPlugin(assemblyName, assemblyPath);
            }
        }

        #region New Open Delete GetKey OnStartUpPage
        private void OnFileNew(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            FileNew();
        }

        void FileNew()
        {
            // startupWelcomeComponent.Workspace.IsBusy = true;
            // try
            {
                if (startupWelcomeComponent.ProjectWizard == null)
                {
                    String fileName = StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowSaveFileDialog(startupWelcomeComponent.UriRisolver.GetOpenFileFilter());
                    if (!(String.IsNullOrEmpty(fileName)))
                    {
                        Uri uri = new Uri(fileName);
                        IDocumentManager manager = startupWelcomeComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                        try
                        {
                            manager.Edit(uri, null);
                            AddRecent(uri);
                            HydeStartUpPage();
                        }
                        catch (Exception ex)
                        {
                            StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, uri, ex.Message));                            
                        }
                    }
                }
                else
                {
                    Uri uri = startupWelcomeComponent.ProjectWizard.CreateNewProject(null);
                    if (uri != null)
                    {
                        IDocumentManager manager = startupWelcomeComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                        try
                        {
                            manager.Edit(uri, null);
                            AddRecent(uri);
                            HydeStartUpPage();
                        }
                        catch (Exception ex)
                        {
                            StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, uri, ex.Message));                            
                        }
                    }
                }
            }
            //finally
            //{
            //    startupWelcomeComponent.Workspace.IsBusy = false;
            //}
        }

        private void HydeStartUpPage()
        {
            startupWelcomeComponent.HideStartUp();
        }

        private void OnFileOpen(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if(e.Parameter is string && (e.Parameter as string) == "DB")
                FileOpen(null, useFileSystem:false);
            else
                FileOpen(e.Parameter as Uri);
            //FileOpen();
        }

        void FileOpen(Uri uri, bool recent = false, bool useFileSystem = true)
        {
            if (uri != null)
            {

                startupWelcomeComponent.Workspace.IsBusy = true;
                try
                {
                    try
                    {
                        IDocumentManager manager = startupWelcomeComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                        manager.Edit(uri, null);
                        AddRecent(uri);
                        HydeStartUpPage();
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is System.UnauthorizedAccessException))
                        {
                            if (recent)
                            {
                                if (StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowYesNo(String.Format(Properties.Resources.ErrorEditingorOpeningRecent, uri, ex.Message), CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                                    RemoveRecent(uri);
                            }
                            else
                                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, uri, ex.Message));
                        }
                        else
                        {
                            if (recent)
                            {
                                if (StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowYesNo(Properties.Resources.ErrorEditingorOpeningRecent1, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                                    RemoveRecent(uri);
                            }
                        }
                    }
                }
                finally
                {
                    startupWelcomeComponent.Workspace.IsBusy = false;
                }              
            }
            else
            {
                string currentUri = string.Empty;
                bool ret = false;
                if (!useFileSystem)
                {
                    var connectionstring = string.Empty;
                    if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connectionstring, "ConnectionWizard", Application.Current.MainWindow, StartupWelcomeComponent.startupWelcomeComponent.UIInterface, StartupWelcomeComponent.startupWelcomeComponent.HelpProvider))
                    {
                        currentUri = connectionstring;
                        ret = true;
                    }
                }
                else
                {
                    var filter = startupWelcomeComponent.UriRisolver.GetOpenFileFilter();
                    var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
                    dialog.Filter = filter;
                    if (dialog.ShowDialog() == true)
                    {
                        currentUri = dialog.FileName;
                        ret = true;
                    }
                }

                if (ret)
                {
                    String fileName = currentUri;
                    if (!(String.IsNullOrEmpty(fileName)))
                    {
                        startupWelcomeComponent.Workspace.IsBusy = true;
                        try
                        {
                            Uri _uri = null;
                            if (XpoHelpers.XpoHelper.IsDataSource(fileName))
                                Uri.TryCreate(String.Format("{0}:{1}", startupWelcomeComponent.UriRisolver.GetOpenFileScheme(), fileName), UriKind.RelativeOrAbsolute, out _uri);
                            else
                            {
                                Uri temp = null;
                                if (Uri.TryCreate(fileName, UriKind.RelativeOrAbsolute, out temp) && temp.IsUnc)
                                {
                                    _uri = temp;
                                }
                                else 
                                    Uri.TryCreate(String.Format("{0}://{1}", startupWelcomeComponent.UriRisolver.GetOpenFileScheme(), fileName), UriKind.RelativeOrAbsolute, out _uri);
                            }
                            if(_uri == null)
                            {
                                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, currentUri, Properties.Resources.BadUriFormat));
                                return;
                            }

                            IDocumentManager manager = startupWelcomeComponent.UriRisolver.ResolveUri(_uri) as IDocumentManager;
                            try
                            {
                                manager.Edit(_uri, null);
                                AddRecent(_uri);
                                HydeStartUpPage();
                            }
                            catch (Exception ex)
                            {
                                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, uri, ex.Message));
                            }
                        }
                        finally
                        {
                            startupWelcomeComponent.Workspace.IsBusy = false;
                        }
                    }
                }
            }
        }

        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void CanManageLayouts(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ApplicationPropertiesHelper.GetProperty("AutoLoadWorkspace") as bool? != true && ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? == true;
        }
        #endregion

        #region Events
        private void OnWhatsNew_Click(object sender, RoutedEventArgs e)
        {
            ManageUrl(Properties.Resources.WebUrl);
        }

        private void ManageUrl(string url)
        {
            string postData = url;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(postData);
            proc.StartInfo = startInfo;
            proc.Start();
        }
        
        #endregion


        [Browsable(false)]
        bool LocalHelp
        {
            get
            {
                if (startupWelcomeComponent.HelpProvider != null)
                {
                    return startupWelcomeComponent.HelpProvider.IsLocalHelpEnabled;
                }
                else
                    return true;
            }
        }

        #region Help links
        internal void FillLinks()
        {
            try
            {
                startupWelcomeComponent.HelpProvider.InitRootPath();
                //add uri to web based help pages
                ReadTopics();
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorFillingHelpLinks, ex);
            }

        }
        internal void ReadTopics()
        {
            lock (lockfile)
            {
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                var doc = XElement.Load(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion));
                var topics = from item in doc.Descendants("Topic")
                             where item.Attribute("Scope").Value == Properties.Settings.Default.HelpScope
                             select item;

                string filepath;
                if (LocalHelp)
                {
                    filepath = startupWelcomeComponent.HelpProvider.LocalFilePath;
                }
                else
                {
                    filepath = startupWelcomeComponent.HelpProvider.WebFilePath;
                }

                foreach (var i in topics)
                {
                    if (i.Attribute("Scope").Value == Properties.Settings.Default.HelpScope)
                    {
                        var run1 = new Run(i.Attribute("Title").Value);
                        string p = i.Attribute("Path").Value;

                        if (LocalHelp)
                            p = string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), i.Attribute("Path").Value, filepath,mainversion);
                        else
                            p = string.Format("{0}\\{1}", filepath, i.Attribute("Path").Value);

                        HelpDoc = new HelpInfo() { ProjectPath = new Uri(string.Format("{0}", p)), Title = i.Attribute("Title").Value };
                        return;
                    }
                }
            }
        }

        void ExecuteHelp()
        {
            if (startupWelcomeComponent.Workspace != null)
                startupWelcomeComponent.Workspace.IsBusy = true;

            startupWelcomeComponent.HelpProvider?.ExecuteHelp();

            if (startupWelcomeComponent.Workspace != null)
                startupWelcomeComponent.Workspace.IsBusy = false;
        }
        #endregion

        #region Save Load Recents

        void AddRecent(Uri uri)
        {
            try
            {
                var q = Recent.RecentList.Where(X => X.ProjectPath == uri).FirstOrDefault();
                if (q != null)
                {
                    Recent.RecentList.Remove(q);
                    Recent.RecentList.Insert(0, q);
                }
                else
                {
                    var c = new RecentInfo { ProjectPath = uri };
                    Recent.RecentList.Insert(0, c);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format(Properties.Resources.ErrorUpdatingRecent, ex.Message));
            }
            finally
            {
                while (Recent.RecentList.Count > nMaxUri)
                    Recent.RecentList.RemoveAt(Recent.RecentList.Count - 1);
                Recent.SaveRecentFileList();
            }
        }
        void RemoveRecent(Uri uri)
        {
            try
            {
                var q = Recent.RecentList.Where(X => X.ProjectPath == uri).FirstOrDefault();
                if (q != null)
                {
                    Recent.RecentList.Remove(q);
                }
                else
                {
                    // do other stuff
                }
                Recent.SaveRecentFileList();
            }
            catch (Exception ex)
            {
                log.Error(String.Format(Properties.Resources.ErrorUpdatingRecent, ex.Message));
            }
        }

        readonly String StoreFileName = String.Format("{0}.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        readonly String StoreLayoutFileName = String.Format("{0}Layout.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

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

        void InitStaticCommandList()
        {
            listStatics.Clear();
            try
            {
                listStatics.Add(Properties.Resources.NewProject);
                listStatics.Add(Properties.Resources.OpenProject);
            }
            catch (Exception ex)
            {
                listStatics.Clear();
            }
        }
        #endregion

        internal IEnumerable<Uri> GetLatestOpened()
        {
            var ret = new List<Uri>();
            foreach (var item in Recent.RecentList)
                ret.Add(item.ProjectPath);
            return ret;
        }

        internal void AddToLatestOpened(Uri uri)
        {
            AddRecent(uri);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as RecentInfo;
            if (item == null)
                return;

            RemoveRecent(item.ProjectPath);
        }

        private void Tile_New_Click(object sender, EventArgs e)
        {
            FileNew();
        }
        private void Tile_Open_Click(object sender, EventArgs e)
        {
            FileOpen(null);
        }

        private void Tile_Help()
        {
            FillLinks();
            HelpInfo selectedItem = HelpDoc;
            ExecuteHelp();
        }
        private void Tile_Tutorial()
        {
            ManageUrl(Properties.Resources.WebWNUrl);
        }
        private void Tile_Support()
        {
            ManageUrl(Properties.Resources.SupportUrl);
        }
        private void Tile_Community()
        {
            ManageUrl(Properties.Resources.CommunityUrl);
        }
        private void Tile_Webinar()
        {
            ManageUrl(Properties.Resources.WebinarUrl);
        }
        private void Tile_Knowledgebase()
        {
            ManageUrl(Properties.Resources.KnowledgeBaseUrl);
        }
        private void Tile_ExampleProjects()
        {
            var filter = startupWelcomeComponent.UriRisolver.GetOpenFileFilter();

            var path = String.Format("{0}\\{1} {2}",
                ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder"),
                Properties.Settings.Default.ExampleFolder,
                Utilities.AssemblyInfo.FileFormatMainVersion);

            if (Directory.Exists(path))
            {
                try
                {
                    System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

                    // Set filter for file extension and default file extension
                    dlg.DefaultExt = filter;
                    dlg.Filter = filter;

                    dlg.InitialDirectory = path;

                    // Display OpenFileDialog by calling ShowDialog method
                    var result = dlg.ShowDialog();

                    // Get the selected file name and display in a TextBox
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        startupWelcomeComponent.Workspace.IsBusy = true;
                        try
                        {
                            Uri _uri = null;
                            var temp = new Uri(dlg.FileName, UriKind.RelativeOrAbsolute);
                            if (temp.IsUnc)
                            {
                                _uri = temp;
                            }
                            else
                                _uri = new Uri(String.Format("{0}://{1}", startupWelcomeComponent.UriRisolver.GetOpenFileScheme(), dlg.FileName), UriKind.RelativeOrAbsolute);

                            IDocumentManager manager = startupWelcomeComponent.UriRisolver.ResolveUri(_uri) as IDocumentManager;
                            try
                            {
                                manager.Edit(_uri, null);
                                AddRecent(_uri);
                                HydeStartUpPage();
                            }
                            catch (Exception ex)
                            {
                                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, _uri, ex.Message));
                            }
                        }
                        finally
                        {
                            startupWelcomeComponent.Workspace.IsBusy = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("{0}: {1}", Properties.Resources.MissingExamples, ex.Message));
                }

            }
            else
                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowWarning(Properties.Resources.ExampleProjectsNotInstalled);
        }

        private void OnExitCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenPlugin(string assemblyName, string assemblyPath)
        {
            try
            {
                var list = FindAndLoadDLL.LoadDLLs<IUFProjectWizardPlugin>(assemblyPath,
                                                    string.Format("{0}{1}", assemblyName, ".dll"), false);
                if (list.Count > 0)
                {
                    WizardSettings.ProjectViewModel NewProject = new WizardSettings.ProjectViewModel()
                    {
                        UIInterface = StartupWelcomeComponent.startupWelcomeComponent.UIInterface,
                        UriRisolver = StartupWelcomeComponent.startupWelcomeComponent.UriRisolver,
                        projectManagerService = StartupWelcomeComponent.startupWelcomeComponent.UFProjectManager,
                        screenManagerService = StartupWelcomeComponent.startupWelcomeComponent.ScreenManager,
                        UFUAEditorManager = StartupWelcomeComponent.startupWelcomeComponent.UFUAEditorManager,
                        stringEditorManager = StartupWelcomeComponent.startupWelcomeComponent.StringEditorManager,
                        recipeEditorManager = StartupWelcomeComponent.startupWelcomeComponent.RecipeEditorManager,
                        toolboxManager = StartupWelcomeComponent.startupWelcomeComponent.ToolboxManager,
                        helpProvider = StartupWelcomeComponent.startupWelcomeComponent.HelpProvider,
                        SchedulerEditorManager = StartupWelcomeComponent.startupWelcomeComponent.SchedulerEditorManager,
                        ADEditorManager = StartupWelcomeComponent.startupWelcomeComponent.ADEditorManager
                        
                    };

                    list[0].Initialize();
                    Uri ProjectUri = list[0].CreateNewProject(null, Application.Current.MainWindow, NewProject);
                    if(ProjectUri != null)
                        FileOpen(ProjectUri, false);
                }
                else if (StartupWelcomeComponent.startupWelcomeComponent.UIInterface != null)
                    StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowWarning(Properties.Resources.PluginError);
            }
            catch (Exception ex)
            {

                StartupWelcomeComponent.startupWelcomeComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorEditingorOpening, assemblyName, ex.Message));
            }
        }

        #region CommandBindings callbacks
        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            if (startupWelcomeComponent.UFProjectManager != null)
            {
                var activeProject = startupWelcomeComponent.UFProjectManager.GetActiveProjects().FirstOrDefault();
                Uri path = new Uri(activeProject, UriKind.RelativeOrAbsolute);
                var doc = (startupWelcomeComponent.UFProjectManager as IDocumentManager).GetDocument(path);
                (startupWelcomeComponent.UFProjectManager as IDocumentManager).Edit(path, doc);
                ApplicationCommands.SaveAs.Execute(null, null);
            }
        }

        private void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = startupWelcomeComponent.UFProjectManager != null && startupWelcomeComponent.UFProjectManager.GetActiveProjects().Count() > 0;
        }

        private void OnStartUp(object sender, ExecutedRoutedEventArgs e)
        {
            startupWelcomeComponent.ShowOrActivateStartupWelcome();
        }

        private void CanOpenStartupPage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = startupWelcomeComponent != null && !startupWelcomeComponent.IsStartupOpened();
        }

        private void OnDongleService(object sender, ExecutedRoutedEventArgs e)
        {
            DongleService();
        }

        private void OnDeployServerService(object sender, ExecutedRoutedEventArgs e)
        {
            DeployServerService();
        }

        private void OnGetKey(object sender, ExecutedRoutedEventArgs e)
        {
            GetKey();
        }

        private void OnDongleOptions(object sender, ExecutedRoutedEventArgs e)
        {
            DongleOptions();
        }

        void DongleOptions()
        {
            var layoutcontrol = new MSZui.UserControl3(startupWelcomeComponent == null);
            var dialog = new GeneralDialogContent(layoutcontrol,GeneralDialogButtons.CloseHelpButtons)
            //var wnd = new Window
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.DongleRibbonLabelDongleOptions,
                HelpLink = "DongleOption"
            };

            dialog.ShowDialog();
        }

        private void OnDongleRequired(object sender, ExecutedRoutedEventArgs e)
        {
            DongleRequired();
        }
        void DongleRequired()
        {
            if (startupWelcomeComponent == null || startupWelcomeComponent.UriRisolver == null ||
                startupWelcomeComponent.Workspace == null || startupWelcomeComponent.Workspace.ContextDocument == null)
                return;

            GeneralDialogContent dialog = null;
            try
            {
                startupWelcomeComponent.Workspace.IsBusy = true;
                var document = startupWelcomeComponent.Workspace.ContextDocument;
                var dialogContent = new RequiredLicenseOptions();
                var docManagers = startupWelcomeComponent.UriRisolver.GetListInstalledDocumentManagers();
                foreach (var doc in docManagers)
                {
                    var options = doc.GetOptionsLicenseRequired(document);
                    if (options == null || options.Count == 0)
                        continue;

                    dialogContent.Options.Add(new OptionsModel { Title = doc.TypeTitle, Values = options });
                }
                               
                dialog = new GeneralDialogContent(dialogContent, GeneralDialogButtons.CloseHelpButtons)
                {
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    Title = String.Format(Properties.Resources.DongleRibbonLabelDongleRequired, document.Parent != null ? document.Parent.Title : document.Title),
                    HelpLink = "DongleRequired",
                };
            }
            finally
            {
                startupWelcomeComponent.Workspace.IsBusy = false;
            }

            if (dialog != null)
                dialog.ShowDialog();
        }

        private void CanDongleOptions(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =  startupWelcomeComponent != null && startupWelcomeComponent.UriRisolver != null && 
                startupWelcomeComponent.Workspace != null && startupWelcomeComponent.Workspace.ContextDocument != null;
        }

        private void DongleService()
        {
            try
            {
                string path = Properties.Settings.Default.ServiceInstallProcess/*"InstallDongleService.exe"*/;
                string prjpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MSZService.exe");
                var currentStyle = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                var arguments = string.Format("/F\"{1}\" /N\"{0}\" /Y\"{2}\" /Z\"{3}\" /J\"{4}\"", Properties.Settings.Default.DongleNetServiceName, prjpath, Properties.Resources.DongleNetServiceDisplayName, Properties.Resources.DongleNetServiceTitle, ApplicationPropertiesHelper.GetProperty("CurrentSkin"));
                RunElevated.Run(path, arguments);
            }
            catch (Exception ex)
            {
            }
        }

        private void DeployServerService()
        {
            try
            {
                string path = Properties.Settings.Default.ServiceInstallDeployServerProcess/*"InstallDeployServerService.exe"*/;
                string prjpath = System.IO.Path.Combine($"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\{Properties.Settings.Default.DeployServerServiceFolder}", "DeployServer.exe");
                var currentStyle = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                var arguments = string.Format("/F\"{1}\" /N\"{0}\" /Y\"{2}\" /Z\"{3}\" /J\"{4}\"", Properties.Settings.Default.DeployServerServiceName, prjpath, Properties.Resources.DeployServerServiceDisplayName, Properties.Resources.DeployServerServiceTitle, ApplicationPropertiesHelper.GetProperty("CurrentSkin"));
                RunElevated.Run(path, arguments);
            }
            catch (Exception ex)
            {
            }
        }

        void GetKey()
        {
            var uIMsgBoxAlertService = startupWelcomeComponent.UIInterface;
            var layoutcontrol = new MSZui.UserControl2(uIMsgBoxAlertService);
            var dialog = new GeneralDialogContent(layoutcontrol, GeneralDialogButtons.CloseHelpButtons)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.DongleRibbonLabelGetKey,
                HelpLink = "LicenceManager",
            };
            dialog.ShowDialog();
        }

        private void OnUseLayoutToFile(object sender, ExecutedRoutedEventArgs e)
        {
            var oldVal = ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? == true;
            ApplicationPropertiesHelper.SetProperty("UseLayoutToFile", !oldVal);
            startupWelcomeComponent.menuControl.useLayoutToFile.IsChecked = !oldVal;
        }

        private void OnManageLayouts(object sender, ExecutedRoutedEventArgs e)
        {
            var uIMsgBoxAlertService = startupWelcomeComponent.UIInterface;
            Enum.TryParse(e.Parameter.ToString(), out LayoutOperation mode);
            var control = new WorkspaceLayoutManager(uIMsgBoxAlertService, mode);
            string helpLink = mode == LayoutOperation.Load ? "WorkspaceLoadLayout" : "WorkspaceSaveLayout";
            var dialog = new GeneralDialogContent(control, GeneralDialogButtons.OkCancelHelpButtons)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = String.Format("{0} {1}", e.Parameter.ToString(), Properties.Resources.WorkspaceMenuHeader),
                HelpLink = helpLink
            };

            dialog.Closing += (o, ea) =>
            {
                if ((o as GeneralDialogContent).DialogResult == true)
                {
                    var selectedLayout = control.SelectedLayout;
                    if (selectedLayout == null || !control.IsValid)
                    {
                        ea.Cancel = true;
                        return;
                    }

                    if (mode == LayoutOperation.Save)
                    {
                        var bOverwriting = (from LayoutFile file in control.LayoutsList where file.Name == selectedLayout.Name select file).FirstOrDefault() != null;
                        if (bOverwriting)
                        {
                            var dialogRes = startupWelcomeComponent.UIInterface.ShowYesNo(String.Format(Properties.Resources.LayoutOverwriteConfirmation, selectedLayout.Name), CustomDialogIcons.Question);
                            if (dialogRes != CustomDialogResults.Yes)
                            {
                                ea.Cancel = true;
                                return;
                            }
                        }
                    }

                    bool bRes = true;
                    if (mode == LayoutOperation.Load)
                        bRes = startupWelcomeComponent.Workspace.LayoutLoadRequest(selectedLayout.Path);
                    else
                        bRes = startupWelcomeComponent.Workspace.LayoutSaveRequest(selectedLayout.Path);
                    ea.Cancel = !bRes;
                }
            };

            dialog.ShowDialog();
        }
        #endregion

        private void listBoxLink_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bDownPressed)
                return;
            bDownPressed = false;
            ButtonHelper selectedItem = (sender as FrameworkElement).DataContext as ButtonHelper;
            if (selectedItem == null)
                return;
            selectedItem.Action();
        }

        private void listBoxLink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonHelper selectedItem = (sender as FrameworkElement).DataContext as ButtonHelper;
            bDownPressed = selectedItem != null;
        }

        #region IDisposable
        public void Dispose()
        {
            if (dpInitUI != null &&
                    dpInitUI.Status != DispatcherOperationStatus.Aborted &&
                    dpInitUI.Status != DispatcherOperationStatus.Completed)
                dpInitUI.Abort();
        }
        #endregion
    }
}
