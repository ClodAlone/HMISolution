using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
#if !WINDOWS_UWP
#if !NETSTANDARD
using System.ServiceModel.Discovery;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UFProjectWizard.ComponentService;
using System.Windows.Controls;
using UFInterfaces.StartupWelcome;
using System.Windows.Threading;
using PropertyControl.ComponentService;
using ScriptManager.ComponentService;
using UFProjectManager.PropertyDataTemplate;
using VFS;
using UFUAEditor.ComponentService;
using UFUserEditor.ComponentService;
using Utilities.ProgressDialog;
#endif
using log4net;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
#if !NET_STANDARD
using StringManager.ComponentService;
using UFInterfaces.AuthenticationCredentialsProvider;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using UFProjectManager.Document;
using System.Windows.Input;
using WPFUtilities.PropertyDataTemplate;
using ScreenManager.ComponentService;
using System.Xml.Linq;
using DevExpress.Xpf.Bars;
using UFInterfaces.Editors;
using DocumentManager.ComponentService.Helpers;
using MSSchedulerSettings.ComponentService;
using System.Diagnostics;
using AppNameSettingService;
using ClientEditor.ComponentService;
using Opc.Ua;
using WPFUtilities;
#endif

namespace UFProjectManager.ComponentService
{
    public class UFProjectManagerComponent : ComponentBase<IUFProjectManager>, IUFProjectManager, IDocumentManager, IDisposable
#if !WINDOWS_UWP && !NET_STANDARD
        , ICrossReference
#endif
    {
#region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UFProjectDocument> mapActiveDocuments = new Dictionary<String, UFProjectDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFProjectDocument, String> mapActiveDocumentUris = new Dictionary<UFProjectDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        public static UFProjectManagerComponent projectManagerComponent { get; protected set; }
#if !WINDOWS_UWP && !NET_STANDARD
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
        bool bIsWorkspaceLoaded;
        bool bReloadDockingOnWorkspaceLoaded;
        bool bToolbarInitialized;
        MenuControl menuControl;
#elif NET_STANDARD
        private static readonly ILog logLicense = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LicenseManager);
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (projectManagerComponent == null)
                projectManagerComponent = this;

            GetComponentInterfaces();
        }

#endregion IUFInterfaceBase Members

        public IEnumerable<String> GetActiveProjects()
        {
            return mapActiveDocuments.Keys;
        }

        public Uri GetStartupScreen(String project)
        {
            if (mapActiveDocuments.ContainsKey(project))
                return mapActiveDocuments[project].GetStartupScreen();
            return null;
            // throw new ArgumentException("Project cannot be found !");
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal static BitmapImage GetControlImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(projectManagerComponent.TypeLabel, image, bShared);
            return bm;
        }
#endif
        internal object GetComponentService(Type service)
        {
            return GetService(service);
        }

        private void GetComponentInterfaces()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));

            if (propertyService == null)
                propertyService = GetService(typeof(IPropertyControl)) as IPropertyControl;
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;

            CreatePropertyDataTemplates();

            if (workspace != null)
            {
                workspace.Closing += workspace_Closing;
                workspace.Closed += workspace_Closed;
                workspace.CloseButtonClick += workspace_CloseButtonClick;
                workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects += workspace_PromptFriendObjects;
                workspace.EasyModeChanged += workspace_EasyModeChanged;
                workspace.WorkspaceLoaded += workspace_OnWorkspaceLoaded;
            }
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        void CreatePropertyDataTemplates()
        {
            if (propertyService == null)
                return;

            var dt1 = new DataTemplate();
            var factory1 = new FrameworkElementFactory(typeof(StartupScriptsPropertyEditor));
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("ListStartupScripts", typeof(List<StartupScript>), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(StartupLogicsPropertyEditor));
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("ListStartupLogics", typeof(List<StartupLogic>), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(AutoloadScreenListPropertyEditor));
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("AutoloadScreenList", typeof(List<AutoloadScreen>), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(LatitudePropertyEditor));
            dt1.DataType = typeof(double);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("Latitude", typeof(double), typeof(UFControllerData), dt1);
            propertyService.AddPropertyEditor("Latitude", typeof(double), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(CulturePropertyControlxaml));
            factory1.SetValue(CulturePropertyControlxaml.WorkspaceProperty, workspace);
            dt1.DataType = typeof(String);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("CultureName", typeof(String), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(ProjectFolderPropertyEditor));
            dt1.DataType = typeof(String);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("ProjectFolder", typeof(String), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(ConverterPropertyControl));
            factory1.SetValue(ConverterPropertyControl.WorkspaceProperty, workspace);
            dt1.DataType = typeof(String);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("ConverterName", typeof(String), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(LongitudePropertyEditor));
            dt1.DataType = typeof(double);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("Longitude", typeof(double), typeof(UFControllerData), dt1);
            propertyService.AddPropertyEditor("Longitude", typeof(double), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(ScreenUriPropertyEditor));
            dt1.DataType = typeof(Uri);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("MainScreen", typeof(Uri), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(StartTypePropertyEditor));
            dt1.DataType = typeof(StartType);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("StartType", typeof(StartType), typeof(UFProjectDocument), dt1);

            dt1 = new DataTemplate();
            factory1 = new FrameworkElementFactory(typeof(ProjectTypePropertyEditor));
            dt1.DataType = typeof(String);
            dt1.VisualTree = factory1;
            propertyService.AddPropertyEditor("ProjectType", typeof(String), typeof(UFProjectDocument), dt1);
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is UFProjectExplorerUI))
                return;

            UFProjectExplorerUI view = e.TargetItem as UFProjectExplorerUI;
            if (!CloseProject(view))
                e.Cancel.Cancel = true;
        }

        public void AddLogEntity(IDocument parent, string source, DateTime recordingTime, string message, System.Diagnostics.EventLogEntryType severity)
        {
            if (parent == null)
                return;
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            doc.AddLogEntity(source, recordingTime, message, severity);
        }

        public void AddLogEntity(IDocument parent, string source, DateTime recordingTime, string message, string details, string comment, string userName, System.Diagnostics.EventLogEntryType severity)
        {
            if (parent == null)
                return;
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            doc.AddLogEntity(source, recordingTime, message, details, comment, userName, severity);
        }

        bool CanClose(UFProjectExplorerUI view, bool bSave = true)
        {
            String uri;
            var doc = view.Document;
            if (doc == null)
                return true;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                if (!view.CanClose())
                    return false;

                if (bSave && doc.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            GetDocumentTitle(uri)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                    {
                        if (!doc.SaveToFile())
                            return false;
                    }
                }
            }

            return true;
        }

        internal bool CloseProject(UFProjectExplorerUI view, bool bSave = true)
        {
            if (view == null || view.Document == null)
                return true;

            String uri;
            var document = view.Document;
            if (mapActiveDocumentUris.TryGetValue(document, out uri))
            {
                if (!view.CanClose())
                    return false;

                if (bSave && document.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            GetDocumentTitle(uri)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                        document.SaveToFile();
                }

                Workspace.LayoutSaveRequest(String.Format("{0}{1}.{2}", document.GetSpecialFolder(SpecialFolders.Documents).GetPathString(), document.Title, Properties.Settings.Default.LayoutFileExtension), view, document.fileSystemProviderBase);

                CloseAllChild(document);

                mapActiveDocumentUris.Remove(document);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);

                //if (document is IDisposable)
                //    (document as IDisposable).Dispose();
            }
            document.PropertyChanged -= Document_PropertyChanged;

            // if (workspace.ContextDocument == document)
            workspace.ContextDocument = null;

            workspace.RemoveBarManagerCommands(view.CommandBindings);

            workspace.RemoveDockingChildren(view);
            if (document is IDisposable)
                (document as IDisposable).Dispose();

            if (!bClosed && mapActiveDocumentUris.Count == 0 && StartupWelcome != null)
                StartupWelcome.ShowOrActivateStartupWelcome();

            return true;
        }

        internal bool CloseProject(UFProjectDocument Document, bool bSave = true)
        {
            String uri;

            if (mapActiveDocumentUris.TryGetValue(Document, out uri))
            {
                if (bSave && Document.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            GetDocumentTitle(uri)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                        Document.SaveToFile();
                }

                CloseAllChild(Document);

                mapActiveDocumentUris.Remove(Document);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);

                if (Document is IDisposable)
                    (Document as IDisposable).Dispose();
            }
            Document.PropertyChanged -= Document_PropertyChanged;

            // if (workspace.ContextDocument == document)
            workspace.ContextDocument = null;

            if (!bClosed && mapActiveDocumentUris.Count == 0 && StartupWelcome != null)
                StartupWelcome.ShowOrActivateStartupWelcome();

            return true;
        }

        bool bClosed;
        void workspace_Closed(object sender, EventArgs e)
        {
            bClosed = true;
            if (mapActiveDocuments.Count == 0)
                return;

            UFProjectDocument[] array = new UFProjectDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                UFProjectExplorerUI view = doc.ActiveView as UFProjectExplorerUI;
                if (view != null)
                    CloseProject(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            UFProjectDocument[] array = new UFProjectDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                UFProjectExplorerUI view = doc.ActiveView as UFProjectExplorerUI;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }

                if (view != null && view.Document != null)
                {
                    if (!view.CloseAllChildProjects(view.Document))
                    {
                        e.Cancel = true;
                        break;
                    }
                }
                Workspace.LayoutSaveRequest(String.Format("{0}{1}.{2}", doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString(), doc.Title, Properties.Settings.Default.LayoutFileExtension), view, doc.fileSystemProviderBase);
            }
        }

        void workspace_PromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            if (!(sender is UFControllerData))
                return;
            var element = sender as UFControllerData;

            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.GetFriendObjects(element, e))
                    break;
            }
        }

        static List<IBarItem> easyModeBarItems = null;
        void workspace_EasyModeChanged(object sender, EventArgs e)
        {
            if (menuControl != null)
            {
                if (easyModeBarItems == null)
                {
                    var items = (from BarSubItem bi in menuControl.biProjectUFProject.Items where bi is BarSubItem && bi.Tag as String != null && workspace.IsComponentHidden((string)bi.Tag) select bi).ToList();
                    easyModeBarItems = new List<IBarItem>();
                    easyModeBarItems.AddRange(items);
                }
                foreach (var baritem in easyModeBarItems)
                    (baritem as BarItem).IsVisible = !workspace.IsInEasyMode;
            }

            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.ActiveView != null
                        select c).ToList();

            foreach (var document in list)
            {
                var ui = document.ActiveView as UFProjectExplorerUI;
                if (ui != null)
                {
                    if (workspace.IsInEasyMode)
                        ui.HideComponents();
                    else
                        ui.RestoreComponents();
                }
            }
        }

        void workspace_OnWorkspaceLoaded(object sender, EventArgs e)
        {
            bIsWorkspaceLoaded = true;
            if (bReloadDockingOnWorkspaceLoaded && mapActiveDocuments.Count > 0 && mapActiveDocuments.Values.First()?.ActiveView != null)
            {
                bReloadDockingOnWorkspaceLoaded = false;
                var doc = mapActiveDocuments.Values.First();
                workspace.ReloadDockState(doc.ActiveView, String.Format("{0}{1}.{2}", doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString(), doc.Title, Properties.Settings.Default.LayoutFileExtension), doc.fileSystemProviderBase);
            }
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ucOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var ucNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            var oldview = e.OldValue as UFProjectExplorerUI;
            var newview = e.NewValue as UFProjectExplorerUI;
            if (oldview != null && ucOld != null)
            {
                ////if (workspace.ContextDocument == view.Document)
                ////    workspace.ContextDocument = null;
                var dockstate = workspace.GetElementDockState(oldview);
                if (dockstate == UFInterfaces.DockState.Document && !oldview.IsLoaded)
                    oldview.Visibility = Visibility.Collapsed;
            }
            if (newview != null && e.NewValue is UFProjectExplorerUI && ucNew != null && mapActiveDocuments.Values.Contains(newview.Document))
            {
                workspace.ContextDocument = newview.Document;
                newview.OnActivate();

                OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(newview.Document);

                //OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(view.Document);

                var dockstate = workspace.GetElementDockState(newview);
                if (dockstate == UFInterfaces.DockState.Document && newview.IsLoaded)
                {
                    newview.Visibility = Visibility.Visible;
                }
            }
            else if (e.NewValue == null && mapActiveDocuments.Count > 0)
            {
                var view = mapActiveDocuments.Values.First().ActiveView;
                if (view != null)
                    workspace.ActiveWindow = view;
            }
        }

        String GetDocumentTitle(Uri uri)
        {
            lock (lockObject)
            {
                String ret = null;
                if (XpoHelpers.XpoHelper.IsDataSource(uri.GetPathString()))
                    ret = XpoHelpers.XpoHelper.GetDataSourceTitle(uri.GetPathString());
                else
                    ret = Path.GetFileNameWithoutExtension(uri.GetPathString());
                String sourcefmt = ret;
                int i = 1;
                while (mapActiveDocumentTitles.ContainsValue(ret))
                    ret = String.Format("{0}{1}", sourcefmt, i++);

                mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UFProjectDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
            else if (String.Compare(e.PropertyName, "ProjectType", false) == 0)
            {
                foreach (KeyValuePair<String, UFProjectDocument> keyvaluepair in mapActiveDocuments)
                {
                    if(keyvaluepair.Value.ActiveView != null && keyvaluepair.Value.ActiveView is UFProjectExplorerUI)
                        (keyvaluepair.Value.ActiveView as UFProjectExplorerUI).RestoreComponents();
                }
            }
        }

        String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

        void CreateDefaultDocument(Uri uri)
        {
            using (UFProjectDocument newProject = new UFProjectDocument(this))
            {
                newProject.ProjectPath = uri.GetPathString();
                if (newProject.SaveToFile())
                    newProject.CreateProjectFolders();
            }
        }
#endif

#region Properties

#if !WINDOWS_UWP && !NET_STANDARD
        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                return workspace;
            }
        }

        IUFUAEditorManager ufuaEditor;
        public IUFUAEditorManager UFUAEditor
        {
            get
            {
                if (ufuaEditor == null)
                    ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditor;
            }
        }

        ISchedulerEditorManager schedulerEditor;
        public ISchedulerEditorManager SchedulerEditor
        {
            get
            {
                if (schedulerEditor == null)
                    schedulerEditor = GetService(typeof(ISchedulerEditorManager)) as ISchedulerEditorManager;
                return schedulerEditor;
            }
        }

        IUFProjectWizard projectWizard;
        public IUFProjectWizard ProjectWizard
        {
            get
            {
                if (projectWizard == null)
                    projectWizard = GetService(typeof(IUFProjectWizard)) as IUFProjectWizard;
                return projectWizard;
            }
        }
#endif
        IUriRisolver uriRisolver;
        public IUriRisolver UriRisolver
        {
            get
            {
                if (uriRisolver == null)
                    uriRisolver = GetService(typeof(IUriRisolver)) as IUriRisolver;
                return uriRisolver;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        IUFUserEditorManager userEditor;
        public IUFUserEditorManager UserEditor
        {
            get
            {
                if (userEditor == null)
                    userEditor = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return userEditor;
            }
        }
#endif

#if !NET_STANDARD
        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
        public IAuthenticationCredentialsProvider AuthenticationCredentialsProvider
        {
            get
            {
                if (authenticationCredentialsProvider == null)
                    authenticationCredentialsProvider = GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                return authenticationCredentialsProvider;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        IStringEditorManager stringEditor;
        public IStringEditorManager StringEditor
        {
            get
            {
                if (stringEditor == null)
                    stringEditor = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditor;
            }
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        IScriptManager scriptManager;
        public IScriptManager ScriptManager
        {
            get
            {
                if (scriptManager == null)
                    scriptManager = GetService(typeof(IScriptManager)) as IScriptManager;
                return scriptManager;
            }
        }

        IClientEditorManager clientEditorManager;
        public IClientEditorManager ClientEditorManager
        {
            get
            {
                if (clientEditorManager == null)
                    clientEditorManager = GetService(typeof(IClientEditorManager)) as IClientEditorManager;
                return clientEditorManager;
            }
        }


        IScreenManager screenManager;
        public IScreenManager ScreenManager
        {
            get
            {
                if (screenManager == null)
                    screenManager = GetService(typeof(IScreenManager)) as IScreenManager;
                return screenManager;
            }
        }


        IStartupWelcome startupWelcome;
        public IStartupWelcome StartupWelcome
        {
            get
            {
                if (startupWelcome == null)
                    startupWelcome = GetService(typeof(IStartupWelcome)) as IStartupWelcome;
                return startupWelcome;
            }
        }

        IPropertyControl propertyService;
        public IPropertyControl PropertyService
        {
            get
            {
                if (propertyService == null)
                    propertyService = GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyService;
            }
        }
#endif
        #endregion Properties

        #region IDocumentManager Members

#if !WINDOWS_UWP && !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (var cursor = new WaitCursor())
            {
                lock (lockObject)
                {
                    if (CheckSecurityAccess(uri))
                    {
                        throw new System.UriFormatException();
                    }

                    UFProjectDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        workspace.ActivateDockedElement(doc.ActiveView);
                    }
                    else
                    {
                        if (doc == null)
                        {
                            doc = UFProjectDocument.FromFile(uri.GetPathString(), this);
                            if (doc == null)
                            {
                                throw new ArgumentException(String.Format(Properties.Resources.CannotOpen, uri.GetPathString()));
                            }
                        }
                        if (doc.IsPasswordProtected())
                        {
                            cursor.Release();
                            workspace.ResetBusy();
                            bool bok = false;
                            while (!bok)
                            {
                                var keyControl = new Controls.EnterKey();
                                var newDialog = new GeneralDialogContent(keyControl)
                                {
                                    Owner = Application.Current.MainWindow,
                                    Title = Properties.Resources.EnterCurrentProjectPassword,
                                    HelpLink = "EnterCurrentProjectPassword"
                                };

                                if (newDialog.ShowDialog() == true)
                                {
                                    if (doc.MatchPassword(keyControl.keyBox.Password))
                                    {
                                        bok = true;
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                            cursor.Aquire();
                            workspace.RestoreBusy();

                            if (bok == false)
                            {
                                doc.Dispose();
                                return;
                            }
                        }
                        bool comesFromLowerVersion = doc.IsComeFromLowerVersion();
                        bool needsUpdate = /*ScreenManager != null && */doc.NeedToUpdateProject();
                        if (needsUpdate && UIInterface != null)
                        {
                            if (UIInterface.ShowYesNo(String.Format(Properties.Resources.ProjectNeedsToBeUpdated, doc.Title),
                                CustomDialogIcons.Question) == CustomDialogResults.Yes)
                            {
                                Workspace.BusyContent = Properties.Resources.SaveBackup;
                                projectManagerComponent.Workspace.IsBusy = true;
                                doc.SaveBackup(bForce: true);
                                    
                                Workspace.BusyContent = Properties.Resources.ProjectConversionInProgress;
                                string path = Properties.Settings.Default.ProjectUpdaterExe;
                                System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                if (callingMainAssembly != null)
                                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                                bool error = false;
                                if (File.Exists(path))
                                {
                                    var args = string.Format(Properties.Settings.Default.ProjectUpdaterArg, doc.FilePath);
                                    var startInfo = new System.Diagnostics.ProcessStartInfo(path, args)
                                    {
                                        RedirectStandardError = true,
                                        RedirectStandardOutput = false,
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
                                                    log.Error(e.Data);
                                                    error = true;
                                                }
                                            }
                                        };

                                        process.Start();
                                        process.BeginErrorReadLine();
                                        process.WaitForExit();
                                    }
                                    doc.SaveToFile();
                                }
                                else
                                {
                                    log.Error(string.Format(Properties.Resources.CannotOpen, path));
                                    error = true;
                                }

                                var title = doc.Title;
                                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    if (error)
                                    {
                                        if (UIInterface.ShowYesNo(Properties.Resources.ProjectConversionGenericError, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                                        {
                                            Workspace.ShowSystemLog();
                                        }
                                    }
                                    else
                                    {
                                        UIInterface.ShowInformation(String.Format(Properties.Resources.ProjectConversionCompleted, title));
                                    }
                                });

                                projectManagerComponent.Workspace.IsBusy = false;
                                projectManagerComponent.Workspace.BusyContent = null;
                            }
                        }
                        else if (comesFromLowerVersion)
                        {
                            var title = doc.Title;
                            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                doc.NeedsSave = true;
                                if (UIInterface != null)
                                    UIInterface.ShowInformation(String.Format(Properties.Resources.ProjectComeFromLowerVersion, title));
                            });
                        }
                        else if (doc.IsComeFromHigherVersion())
                        {
                            var title = doc.Title;
                            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                if (UIInterface != null)
                                    UIInterface.ShowInformation(String.Format(Properties.Resources.ProjectComeFromHigherVersion, title));
                            });
                        }

                        if (workspace.IsInEasyMode)
                            workspace_EasyModeChanged(null, null);

                        UFProjectExplorerUI projectEditor = new UFProjectExplorerUI(this, doc, menuControl.Cmds);

                        doc.Parent = parent;
                        doc.ActiveView = projectEditor;

                        UFInterfaces.DockSide side = UFInterfaces.DockSide.Tabbed;

                        workspace.SetDesiredHeightAndWidthInDockedMode(projectEditor, projectEditor.Height, projectEditor.Width);
                        projectEditor.ClearValue(FrameworkElement.WidthProperty);
                        projectEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = TypeIcon;

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        {
                            mapActiveDocuments.Add(uri.GetPathString(), doc);
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());
                        }
                        workspace.AddDockingChildren(projectEditor, GetDocumentTitle(uri), UFInterfaces.DockState.Dock, side, false);
                        workspace.SetDockedElementIcon(projectEditor, new ImageBrush(bm));
                        if (bIsWorkspaceLoaded)
                            workspace.ReloadDockState(projectEditor, String.Format("{0}{1}.{2}", doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString(), doc.Title, Properties.Settings.Default.LayoutFileExtension), doc.fileSystemProviderBase);
                        else
                            bReloadDockingOnWorkspaceLoaded = true;
                        workspace.FlashDockedElement(projectEditor);

                        projectEditor.Document.PropertyChanged += Document_PropertyChanged;
                    }
                }
            }
        }

        DispatcherTimer /*Timer*/  timer;
#endif
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            UFProjectDocument doc = null;
            bool bFirstCheck = true;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFProjectDocument.FromFile(uri.GetPathString(), this);
                if (doc == null)
                    throw new ArgumentException(String.Format(Properties.Resources.CannotOpen, uri.GetPathString()));
                doc.Parent = parent;
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }


#if !NET_STANDARD
            var p = DocumentHelper.GetRootParent(doc, true);
            if(p == doc)
                ApplicationPropertiesHelper.SetProperty("CurrentSkin", p.Theme);
#endif

#if !NET_STANDARD
            var busyComponent = doc.GetService(typeof(IBusyComponent)) as IBusyComponent;
#endif
            doc.ProjectStatus.Starting = true;
#if !WINDOWS_UWP
            if (mode != ExecutionMode.Shared)
                doc.SetCurrentLogFileName();
            log.Info(String.Format(Properties.Resources.StartingProject, doc.Title));
#endif
            doc.UpdateSessionSettings();
            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(doc);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
#if !DEBUG
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
            };
#endif
#if !WINDOWS_UWP && !NET_STANDARD
            doc.PreSubscribeServerSession();
            doc.SubscribeAuthenticationEvents();
            doc.SubscribePadsEvents();
#if !NET_STANDARD
            doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.StartingProject, doc.Title), System.Diagnostics.EventLogEntryType.Information);
#endif
#endif
            if (mode != ExecutionMode.Shared)
            {   
#if !NET_STANDARD
                var eventDocumentManager = doc.GetEventDocumentManager();
#endif
                var listsingle = (from c in UriRisolver.GetListInstalledDocumentManagers()// .AsParallel()
                                  where (!c.isMultipleResource || c.isServiceResource) && c.GetType() != GetType()
                                  orderby c.TypeScheme descending
                                  select c).ToList();
                foreach (var docManager in listsingle)
                {
#if !NET_STANDARD
                    if (eventDocumentManager != null && docManager.GetType() == eventDocumentManager.GetType())
                        continue;

                    if (busyComponent != null)
                        busyComponent.BusyContent = String.Format(Properties.Resources.IsStartingText, docManager.TypeTitle);
#endif
                    // var uriToOpen = new Uri(String.Format("{0}:{1}", docManager.TypeScheme, doc.ProjectFolder));
                    try
                    {
                        docManager.Execute(new Uri(doc.ProjectFolder, UriKind.RelativeOrAbsolute), doc, ExecutionMode.Synchro, null);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message, ex);
#if !WINDOWS_UWP && !NET_STANDARD
                        doc.AddLogEntity(null, DateTime.UtcNow, ex.Message, System.Diagnostics.EventLogEntryType.Error);
#endif
                    }
                }

                Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
                bool bClient = commandArgs.ArgPairs.ContainsKey("client");
#if !WINDOWS_UWP && !NET_STANDARD
                if (busyComponent != null)
                {
                    var manager = doc.GetScriptDocumentManager();
                    busyComponent.BusyContent = String.Format(Properties.Resources.IsStartingText, manager?.TypeTitle);
                }

                if (!bClient)
                    doc.StartupServiceScripts();

                var listStartupNoService = (from c in doc.ListStartupScripts where c.ExecuteAsService == false select c).ToList();
                listStartupNoService.ForEach(script =>
                {
                    var managerscript = uriRisolver.ResolveUri(script.Uri) as IDocumentManager;
                    if (managerscript != null)
                        managerscript.Execute(script.Uri, doc, script.ExecutionMode, doc);
                    else
                    {
                        log.Error(String.Format(Properties.Resources.StartupScriptInvalid,
                            script.Uri.GetPathString()));
                        doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.StartupScriptInvalid,
                                                    script.Uri.GetPathString()), System.Diagnostics.EventLogEntryType.Error);
                    }

                });
#endif

#if !WINDOWS_UWP && !NET_STANDARD
                if (busyComponent != null)
                {
                    var manager = doc.GetLogicDocumentManager();
                    busyComponent.BusyContent = String.Format(Properties.Resources.IsStartingText, manager?.TypeTitle);
                }

                if (!bClient)
                    doc.StartupServiceLogics();
#endif
                var listStartupLogicNoService = (from c in doc.ListStartupLogics where c.ExecuteAsService == false select c).ToList();
                listStartupLogicNoService.ForEach(logic =>
                {
                    var managerLogict = uriRisolver.ResolveUri(logic.Uri) as IDocumentManager;
                    if (managerLogict != null)
                        managerLogict.Execute(logic.Uri, doc, logic.ExecutionMode, doc);
                    else
                    {
                        log.Error(String.Format(Properties.Resources.StartupLogicInvalid,
                                logic.Uri.GetPathString()));
#if !WINDOWS_UWP && !NET_STANDARD
                        doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.StartupLogicInvalid,
                                                    logic.Uri.GetPathString()), System.Diagnostics.EventLogEntryType.Error);
#endif
                    }
                });
            }

            doc.ListChildProjectPaths.ForEach(project =>
            {
                var controller = doc.GetChildProjectData(project);
                var abs = project;
#if !WINDOWS_UWP && !NET_STANDARD
                var relative = project.GetPathString();
                if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                {
                    var match = String.Format("{0}/", doc.Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", doc.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    abs = doc.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                }
#endif
                try
                {
                    Execute(abs, doc, controller.IsStartable ? ExecutionMode.Normal : ExecutionMode.Shared, Context);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
#if !WINDOWS_UWP && !NET_STANDARD
                    doc.AddLogEntity(null, DateTime.UtcNow, ex.Message, System.Diagnostics.EventLogEntryType.Error);
#if !NET_STANDARD
                    if (Environment.UserInteractive)
#endif
#endif
#if !NET_STANDARD
                        UIInterface.ShowError(String.Format(Properties.Resources.ProjectCannotStart,
                                                project.GetPathString()));
#endif
                }
            });

            if (mode != ExecutionMode.Shared)
            {
#if !NET_STANDARD
                // var startupscreen = doc.GetStartupScreen();
                if (parent == null)
                {
                    var managerscreen = doc.GetScreenDocumentManager();
                    if (managerscreen != null)
                    {
                        if (busyComponent != null)
                            busyComponent.BusyContent = String.Format(Properties.Resources.IsStartingText, managerscreen.TypeTitle);
                        managerscreen.Execute(null, doc, ExecutionMode.Normal, doc);
                    }
                }
                var eventDocumentManager = doc.GetEventDocumentManager();
                if (eventDocumentManager != null)
                {
                    if (busyComponent != null)
                        busyComponent.BusyContent = String.Format(Properties.Resources.IsStartingText, eventDocumentManager.TypeTitle);
                    eventDocumentManager.Execute(new Uri(doc.ProjectFolder, UriKind.RelativeOrAbsolute), doc, ExecutionMode.Synchro, doc);
                }
#endif
#if !WINDOWS_UWP && !NET_STANDARD
#if !DEBUG
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }
                
                var bmode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxSRSfdPty4M/h8yKtCnKfpw=="/* CMD */);
                if (bmode == false)
                {
                    logLicense.Warn(Properties.Resources.NoRuntimeLicense);
                    doc.AddLogEntity(null, DateTime.UtcNow, Properties.Resources.NoRuntimeLicense, 
                    System.Diagnostics.EventLogEntryType.Warning);

                    var counter = 0;
                    if (timer == null)
                    {

                        var control = new MSZui.UserControl1(0, String.Format(Properties.Resources.NoRuntime, 120));
                        var wnd = new Window()
                        {
                            //Owner = this,
                            Topmost = true,
                            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                            Content = control,
                            WindowStyle = System.Windows.WindowStyle.None,
                            SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                            WindowState = System.Windows.WindowState.Normal,
                            ShowInTaskbar = false,
                            ResizeMode = System.Windows.ResizeMode.NoResize
                        };

                        wnd.ShowDialog();
                        wnd.Close();

                        if (bFirstCheck && !MSZ.MSZView.IsSuspended() && !MSZ.MSZView.IsForciblySuspended())
                            {
                                bFirstCheck = false;
                                var siteCode = MSZ.MSZUtils.GetPrevious();
                                if (!String.IsNullOrEmpty(siteCode))
                                {
                                    logLicense.Warn(String.Format(Properties.Resources.LicenseSiteCode, siteCode));
                                    doc.AddLogEntity(null, DateTime.UtcNow, 
                                        String.Format(Properties.Resources.LicenseSiteCode, siteCode), 
                                        System.Diagnostics.EventLogEntryType.Information);
                                }
                            }

                        //MessageBox.Show(String.Format(Properties.Resources.NoRuntimeLicenseTerminating, 120 - counter * 10));
                        logLicense.Warn(String.Format(Properties.Resources.NoRuntimeLicenseTerminating, 120 - counter * 10));
                        doc.AddLogEntity(null, DateTime.UtcNow, 
                            String.Format(Properties.Resources.NoRuntimeLicenseTerminating, 120 - counter * 10), 
                            System.Diagnostics.EventLogEntryType.Warning);
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromMinutes(10);
                        timer.IsEnabled = true;
                        timer.Start();
                        timer.Tick += (to, te) =>
                        {
                            if (++counter >= 12)
                            {
                                timer.Stop();

                                //signal demo terminated!!!
                                Terminate(uri, parent);

                                //MessageBox.Show(Properties.Resources.NoRuntimeLicenseTerminated);
                                var control1 = new MSZui.UserControl1(0, Properties.Resources.NoRuntimeLicenseTerminated/*String.Format(Properties.Resources.NoRuntime, 120)*/);
                                var wnd1 = new Window()
                                {
                                    //Owner = this,
                                    Topmost = true,
                                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                                    Content = control1,
                                    WindowStyle = System.Windows.WindowStyle.None,
                                    SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                                    WindowState = System.Windows.WindowState.Normal,
                                    ShowInTaskbar = false,
                                    ResizeMode = System.Windows.ResizeMode.NoResize
                                };

                                wnd1.ShowDialog();
                                wnd1.Close();
                                logLicense.Warn(Properties.Resources.NoRuntimeLicenseTerminated);
                                doc.AddLogEntity(null, DateTime.UtcNow, Properties.Resources.NoRuntimeLicenseTerminated, 
                                    System.Diagnostics.EventLogEntryType.Warning);

                                System.Environment.Exit(-10);
                            }
                            else
                            {
                                logLicense.Warn(String.Format(Properties.Resources.NoRuntimeLicenseTerminating, 120 - counter * 10));
                                doc.AddLogEntity(null, DateTime.UtcNow, 
                                    String.Format(Properties.Resources.NoRuntimeLicenseTerminating, 120 - counter * 10), 
                                    System.Diagnostics.EventLogEntryType.Warning);
                            }
                        };
                    }
                }
#endif
#endif
            }

#if !WINDOWS_UWP
            log.Info(String.Format(Properties.Resources.ProjectStarted, doc.Title));
#if !NET_STANDARD
#if !DEBUG
            var dongleInfo = MSZ.MSZView.GetSerialInfo();
            doc.AddLogEntity(null, DateTime.UtcNow, dongleInfo, System.Diagnostics.EventLogEntryType.Information);
            log.Info(dongleInfo);
#endif
            doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.ProjectStarted, doc.Title), System.Diagnostics.EventLogEntryType.Information);
#endif
            if (doc.IsComeFromHigherVersion())
            {
                log.Info(String.Format(Properties.Resources.ProjectComeFromHigherVersion, doc.Title));
#if !WINDOWS_UWP && !NET_STANDARD
                doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.ProjectComeFromHigherVersion, doc.Title), System.Diagnostics.EventLogEntryType.Information);
#endif
#if !NET_STANDARD
                if (Environment.UserInteractive)
                    UIInterface.ShowWarning(String.Format(Properties.Resources.ProjectComeFromHigherVersion, doc.Title));
#endif
            }
#endif
            doc.ProjectStatus.Running = true;
            doc.ProjectStatus.Starting = false;
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            UFProjectDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return;

            doc.ProjectStatus.Terminating = true;
#if !WINDOWS_UWP
            log.Info(String.Format(Properties.Resources.StoppingProject, doc.Title));
#if !NET_STANDARD
            doc.AddLogEntity(null, DateTime.UtcNow, String.Format(Properties.Resources.StoppingProject, doc.Title), System.Diagnostics.EventLogEntryType.Information);
            doc.UnsubscribeServerSession();
            doc.UnsubscribeAuthenticationEvents();
            doc.UnsubscribePadsEvents();
#endif
#endif
            doc.ListChildProjectPaths.ForEach(project =>
            {
                var abs = project;
#if !WINDOWS_UWP && !NET_STANDARD
                var relative = project.GetPathString();
                if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                {
                    var match = String.Format("{0}/", doc.Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", doc.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    abs = doc.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                }
#endif
                Terminate(abs, doc);
            });

            var listsingle = (from c in UriRisolver.GetListInstalledDocumentManagers()// .AsParallel()
                              where (!c.isMultipleResource || c.isServiceResource) && c.GetType() != GetType()
                              orderby c.TypeScheme descending
                              select c).ToList();

            PreTerminate(doc, listsingle);
            Terminate(doc, listsingle);

            OPCUAViewModel.OPCUAEntityReference.StopDataSinkInterfaces();

            doc.Terminate();
            doc.ProjectStatus.Running = false;
            doc.ProjectStatus.Terminating = false;

#if !WINDOWS_UWP
            log.Info(String.Format(Properties.Resources.ProjectStopped, doc.Title));
            doc.RestoreCurrentLogFileName();

            //if (timer != null)
            //{
            //    timer.Dispose();
            //    timer = null;
            //}
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            throw new NotImplementedException();
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            throw new NotImplementedException();
        }

        public void Delete(Uri uri, IDocument parent)
        {
        }

        internal IDocument GetOpenDocument(Uri uri)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()];
            return null;
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = UFProjectDocument.FromFile(uri.GetPathString(), this);
            if (doc != null)
            {
                doc.Parent = parent;
                using (doc)
                {
                    return doc.SaveToFile(forceEncryption: encryptFile);
                }
            }
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }
#endif
        public IDocument GetDocument(Uri uri)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()];

            var doc = UFProjectDocument.FromFile(uri.GetPathString(), this);
            if (doc == null)
                return null;
            
            mapActiveDocuments.Add(uri.GetPathString(), doc);
            mapActiveDocumentUris.Add(doc, uri.GetPathString());
            return doc;
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                document.SaveToFile();
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseProject(document.ActiveView as UFProjectExplorerUI))
                    return false;
            }

            if (bParentClosing)
                CleanOnClose(parent);

            return true;
        }

        void CleanOnClose(IDocument parent)
        {
            var listToClean = (from c in mapActiveDocuments// .AsParallel()
                               where c.Value.Parent == parent && c.Value.ActiveView == null
                               select c).ToList();
            listToClean.ForEach(pair =>
            {
                mapActiveDocuments.Remove(pair.Key);
                pair.Value.Dispose();

                if (mapActiveDocumentUris.ContainsKey(pair.Value))
                    mapActiveDocumentUris.Remove(pair.Value);
                if (mapActiveDocumentTitles.ContainsKey(pair.Key))
                    mapActiveDocumentTitles.Remove(pair.Key);
            });
        }

        public bool IsAnyChildNeedsSave(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent /*&& c.ActiveView != null*/ 
                        select c).ToList();
            foreach (var document in list)
            {
                if (document.NeedsSave)
                    return true;
            }

            return false;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            return null;
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            return null;
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }
#endif
        public String TypeTitle
        {
            get
            {
                return Properties.Resources.TypeTitle;
            }
        }


        public String TypeLabel
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.TypeLabel;
#else
                return "UFProject";
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetControlImage("UFPRJEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetControlImage("UFPRJEditor");
            }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }
#endif
        public String TypeScheme
        {
            get
            {
#if !WINDOWS_UWP
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
#else
                Assembly assembly = this.GetType().GetTypeInfo().Assembly;
                return Path.GetFileNameWithoutExtension(assembly.GetName().Name);
#endif
            }
        }

        public String FileType
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.DefaultFileExt;
#else
                return ".UFProject";
#endif
            }
        }
        public String FileName
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.DefaultProjectName;
#else
                return "Project";
#endif
            }
        }

        public String[] SaveAsFileExtensions
        {
            get { return null; }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool RegisterFileType
        {
            get { return true; }
        }

        public bool CanBeDragged
        {
            get
            {
                return false;
            }
        }

        public Object DragContent
        {
            get
            {
                return null;
            }
        }

        public Object BrowsableContent
        {
            get
            {
                return null;
            }
        }
#endif
        public bool isMultipleResource
        {
            get
            {
                return false;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            var doc = document as UFProjectDocument;
            if (doc == null)
                return false;
            return doc.ListChildProjectPaths == null ? false : doc.ListChildProjectPaths.Count > 0;
        }
#endif
        public bool IsStartupControllerAware
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            if (parent != null)
                return ProjectWizard.CreateNewProject(relative);

            if (!relative.IsFile)
            {
                try
                {
                    var ret = UFProjectDocument.FromFile(relative.OriginalString, this);
                    if (ret != null)
                    {
                        ret.Dispose();
                        UIInterface.ShowError(String.Format(Properties.Resources.ProjectExists,
                                                relative));
                        return null;
                    }
                }
                catch (Exception ex)
                {

                }

                CreateDefaultDocument(relative);
                return relative;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri);
                return uri;
            }

            String path = String.Format("{0}{1}", relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
            //int i = 0;
            //while (File.Exists(path))
            //{
            //    path = String.Format("{0}{1}{2}", relative.LocalPath, ++i, Properties.Settings.Default.DefaultFileExt);
            //}
            if (File.Exists(path))
            {
                UIInterface.ShowError(String.Format(Properties.Resources.ProjectExists,
                                        path));
                return null;
            }

            Uri url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url);
            return url;
        }
#endif
        public Type DocumentType
        {
            get
            {
                return typeof(UFProjectDocument);
            }
        }

#endregion IDocumentManager Members

#region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
#if !WINDOWS_UWP && !NET_STANDARD
            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects -= workspace_PromptFriendObjects;
                workspace.EasyModeChanged -= workspace_EasyModeChanged;
                workspace.WorkspaceLoaded -= workspace_OnWorkspaceLoaded;
            }
#endif
        }

#endregion IDisposable Members

        void PreTerminate(UFProjectDocument doc, IList<IDocumentManager> listsingle)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            var managerscript = doc.GetScriptDocumentManager();
            if (managerscript != null)
                managerscript.PreTerminate(null, doc);
#endif
#if !NET_STANDARD
            var managerlogic = doc.GetLogicDocumentManager();
            if (managerlogic != null)
                managerlogic.PreTerminate(null, doc);

            var managerscreen = doc.GetScreenDocumentManager();
            if (managerscreen != null)
                managerscreen.PreTerminate(null, doc);

            var managershortcut = doc.GetShortcutDocumentManager();
            if (managershortcut != null)
                managershortcut.PreTerminate(null, doc);

            var managerreport = doc.GetReportDocumentManager();
            if (managerreport != null)
                managerreport.PreTerminate(null, doc);
#endif
            foreach (var docManager in listsingle)
                docManager.PreTerminate(new Uri(doc.ProjectFolder, UriKind.RelativeOrAbsolute), doc);
        }

        void Terminate(UFProjectDocument doc, IList<IDocumentManager> listsingle)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            doc.StopServiceScript();
            var managerscript = doc.GetScriptDocumentManager();
            if (managerscript != null)
                managerscript.Terminate(null, doc);

            doc.StopServiceLogic();
#endif
#if !NET_STANDARD
            var managerlogic = doc.GetLogicDocumentManager();
            if (managerlogic != null)
                managerlogic.Terminate(null, doc);
#endif
            foreach (var docManager in listsingle)
            {
                try
                {
                    docManager.Terminate(new Uri(doc.ProjectFolder, UriKind.RelativeOrAbsolute), doc);
                }
                catch (Exception ex)
                {
#if !WINDOWS_UWP && !NET_STANDARD
                    MessageBox.Show(ex.ToString());
#endif
                }
            }
#if !NET_STANDARD
            var managerscreen = doc.GetScreenDocumentManager();
            if (managerscreen != null)
                managerscreen.Terminate(null, doc);

            var managershortcut = doc.GetShortcutDocumentManager();
            if (managershortcut != null)
                managershortcut.Terminate(null, doc);

            var managerreport = doc.GetReportDocumentManager();
            if (managerreport != null)
                managerreport.Terminate(null, doc);
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        private bool CheckSecurityAccess(Uri uri)
        {
            try
            {
                Uri _uri;
                if (!XpoHelpers.XpoHelper.IsDataSource(uri.GetPathString()))
                {
                    try
                    {
                        System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(uri.LocalPath);
                        System.Security.AccessControl.AuthorizationRuleCollection rules = ds.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                        if (HasFileOrDirectoryAccess(System.Security.AccessControl.FileSystemRights.Write, rules))
                            return false;
                        else
                        {
                            log.Error(String.Format(Properties.Resources.CannotOpenProject, uri.OriginalString));
                            if (UIInterface != null)
                                UIInterface.ShowError(String.Format(Properties.Resources.CannotOpenProject, uri.OriginalString));
                            return true;
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        log.Error(ex.Message, ex);
                        if (UIInterface != null)
                            UIInterface.ShowError(String.Format(Properties.Resources.CannotOpenProject, uri.OriginalString));

                        return true;
                    }
                }
                else
                    return false;
            }
            catch (System.UriFormatException ex)
            {
                log.Error(ex.Message, ex);
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.CannotOpenProject, uri.OriginalString));

                return true;
            }
        }
        private bool HasFileOrDirectoryAccess(System.Security.AccessControl.FileSystemRights right,
                                     System.Security.AccessControl.AuthorizationRuleCollection acl)
        {
            bool allow = false;
            bool inheritedAllow = false;
            bool inheritedDeny = false;
            System.Security.Principal.WindowsIdentity _currentUser;
            System.Security.Principal.WindowsPrincipal _currentPrincipal;
            _currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
            _currentPrincipal = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
            for (int i = 0; i < acl.Count; i++)
            {
                System.Security.AccessControl.FileSystemAccessRule currentRule = (System.Security.AccessControl.FileSystemAccessRule)acl[i];
                // If the current rule applies to the current user.
                if (_currentUser.User.Equals(currentRule.IdentityReference) ||
                    _currentPrincipal.IsInRole(currentRule.IdentityReference.ToString()))
                {

                    if (currentRule.AccessControlType.Equals(System.Security.AccessControl.AccessControlType.Deny))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            if (currentRule.IsInherited)
                            {
                                inheritedDeny = true;
                            }
                            else { // Non inherited "deny" takes overall precedence.
                                return false;
                            }
                        }
                    }
                    else if (currentRule.AccessControlType
                                                    .Equals(System.Security.AccessControl.AccessControlType.Allow))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            if (currentRule.IsInherited)
                            {
                                inheritedAllow = true;
                            }
                            else {
                                allow = true;
                            }
                        }
                    }
                }
            }

            if (allow)
            { // Non inherited "allow" takes precedence over inherited rules.
                return true;
            }
            return inheritedAllow && !inheritedDeny;
        }
        private UFProjectDocument GetOrCreateDocument(IDocument parent)
        {
            IDocument root = parent.Parent;
            if (root == null)
                root = parent;

            Uri uri = null;
            if (root.fileSystemProviderBase != null && root.fileSystemProviderBase is DataSourceFileSystemProvider)
            {
                var path = (root.fileSystemProviderBase as DataSourceFileSystemProvider).ConnectionString;
                uri = new Uri(String.Format("{0}:{1}", UriRisolver.GetOpenFileScheme(), path));
            }
            else
            {
                uri = new Uri(String.Format("{0}{1}", root.rootBase, FileType), UriKind.RelativeOrAbsolute);
            }

            lock (mapActiveDocuments)
            {
                UFProjectDocument doc = null;
                if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    doc = UFProjectDocument.FromFile(uri.GetPathString(), this);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = parent.Parent ?? parent;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
                return doc;
            }
        }
#endif

#region IUFProjectManager
#if !WINDOWS_UWP && !NET_STANDARD
        readonly Dictionary<UFProjectDocument, ResourcePicker> mapResourcePickerControls = new Dictionary<UFProjectDocument, ResourcePicker>();
        public UserControl GetResourcePickerUserControl(IDocument parent, String filter)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            if (!mapResourcePickerControls.ContainsKey(doc))
                mapResourcePickerControls.Add(doc, new ResourcePicker(doc));
            mapResourcePickerControls[doc].Filter = filter;
            return mapResourcePickerControls[doc];
        }

        public Uri GetResourcePickerUserControlUri(UserControl control)
        {
            return (control as ResourcePicker).Selected;
        }
        
        List<String> AddResources(IDocument parent, ResourceFolderWatcher folder, bool getRelativePath = true)
        {
            var list = getRelativePath ? (from c in folder.ListResources
                        select parent.MakeRelativeUri(c).GetPathString()).ToList() :
                        (from c in folder.ListResources
                         select c.GetPathString()).ToList();

            folder.ListFolders.ToList().ForEach(f =>
                {
                    var ret = AddResources(parent, f, getRelativePath);
                    list.AddRange(ret);
                });
            return list;
        }
        
        public Dictionary<String, String> GetRenamedResources(IDocument parent)
        {
            var list = new Dictionary<String, String>();
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return list;

            return doc.GetRenamedResources();
        }

        public void ClearRenamedResourcesMap(IDocument parent, List<string> typeLabelList)
        {
            var list = new Dictionary<String, String>();
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;

            doc.ClearRenamedResourcesMap(typeLabelList);
        }

        public IEnumerable<String> GetResourceList(IDocument parent, String filter, bool getRelativePath = true)
        {
            var list = new List<String>();
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return list;

            var docManager = doc.GetResourceDocumentManager(filter);
            if (!docManager.isMultipleResource)
                return list;

            var folder = doc.GetResourceFolderWatcher(filter);
            return AddResources(parent, folder);
        }

        public String GetConfigurationId(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return string.Empty;

            return doc.ConfigurationId.ToString();
        }

        public string GetOriginalVersion(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return string.Empty;

            return doc.OriginalVersion ?? String.Empty;
        }

        public string GetProjectVersion(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return string.Empty;

            return doc.ProjectVersion ?? String.Empty;
        }

        public IDocumentManager GetResourceDocumentManager(IDocument parent, String filter)
        {
            var list = new List<String>();
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            var docManager = doc.GetResourceDocumentManager(filter);
            return docManager;
        }

        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);

            if (!menuControl.IsToolbarShown && workspace.IsWorkspaceLoaded && !bToolbarInitialized)
            {
                bToolbarInitialized = true;
                menuControl.Show();
            }

            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }

        public string GetDefaultFileExt()
        {
            return Properties.Settings.Default.DefaultFileExt;
        }

        public void SetControllerDataActive(IDocument parent, Uri uri)
        {
            if (parent == null)
                return;
            UFProjectDocument doc;
            Uri docPath = new Uri(parent.FilePath, UriKind.RelativeOrAbsolute);
            if (mapActiveDocuments.TryGetValue(docPath.GetPathString(), out doc) && doc.ActiveView != null)
            {
                UFProjectExplorerUI editor = doc.ActiveView as UFProjectExplorerUI;
                editor.SelectNodeFromUri(uri);
            }
            return;
        }

        public List<CrossReferenceResultModel> GetCRObjects(CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getScreen = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens);
            bool getStrings = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getScreen && !getStrings)
                return result;

            var parent = DocumentHelper.GetRootParent(model.Parent, traverse: true) as UFProjectDocument;
            string docType = DocManagerType.ProjectManager.ToString();
            if(getScreen)
            {
                parent.AutoloadScreenList.ForEach(screen =>
                {
                    var pathString = screen.Uri.GetPathString();
                    var rpath = pathString.Remove(pathString.LastIndexOf(".xaml"));
                    var name = rpath.Split('/').LastOrDefault();
                    //lock (result)
                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = rpath,
                        Name = name,
                        AppName = parent.Title,
                        CReferenceType = CrossReferenceType.Screens,
                        Description = string.Format("{0}", Properties.Resources.AutoloadScreenList),
                        Settings = string.Format("{0}", parent.FilePath),
                        ContainerDoc = TypeScheme,
                        IconType = docType
                    });
                });
                if (parent.MainScreen != null)
                {
                    var pathString = parent.MainScreen.GetPathString();
                    var rpath = pathString.Remove(pathString.LastIndexOf(".xaml"));
                    var name = rpath.Split('/').LastOrDefault();
                    //lock (result)
                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = rpath,
                        Name = name,
                        AppName = parent.Title,
                        CReferenceType = CrossReferenceType.Screens,
                        Description = string.Format("{0}", Properties.Resources.MainScreen),
                        Settings = string.Format("{0}", parent.FilePath),
                        ContainerDoc = TypeScheme,
                        IconType = docType
                    });
                }
            }
            if(getStrings)
            {
                Dictionary<string, string> map = parent.GetTileDescriptions();
                map.Keys.ToList().ForEach(key =>
                {
                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = $"{map[key]}",
                        Name = $"{map[key]}",
                        AppName = parent.Title,
                        CReferenceType = CrossReferenceType.Strings,
                        Description = string.Format("{0} ({1})", key, Properties.Resources.CRTileDescription),
                        Settings = string.Format("{0}|{1}|{2}", null, map[key], null),
                        ContainerDoc = TypeScheme,
                        IconType = docType
                    });
                });
            }

            return result;
        }

        public bool NeedToCloseCRDocuments()
        {
            return false;
        }
        
        public bool NeedSingleThreadedApartment 
        {
            get
            {
                return false;
            }
        }

        public void RenameCRObjects(CrossReferenceModel model)
        {
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            //using (EventEditorDocument doc = CreateDocument(model.Parent))
            var doc = GetOrCreateDocument(model.Parent);
            if (doc != null)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                if (doc != null)
                    doc.RenameReferences(model);
            }
        }

        public void EditCRObject(IDocument parent, string settings)
        {
        }

        public IAppNameSettings GetDefaultAppNameSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            return new OPCUAViewModel.AppNameSettings()
            { 
                RemoveDisabledItemAfterSecs = doc.RemoveDisabledItemAfterSecs,
                MaxCleanCount = doc.MaxCleanCount,
                UseAlwaysSecureConnections = doc.UseAlwaysSecureConnections,
                FastSamplingInterval = doc.FastSamplingInterval,
                SlowSamplingInterval = doc.SlowSamplingInterval,
                DisableWhenNotUsed = doc.DisableWhenNotUsed,
                PublishingInterval = doc.PublishingInterval
            };
        }

        public void UpdateDefaultAppNameSettings(IDocument parent, IAppNameSettings settings)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null || settings == null)
                return;

            doc.RemoveDisabledItemAfterSecs = settings.RemoveDisabledItemAfterSecs;
            doc.MaxCleanCount = settings.MaxCleanCount;
            doc.UseAlwaysSecureConnections = settings.UseAlwaysSecureConnections;
            doc.FastSamplingInterval = settings.FastSamplingInterval;
            doc.SlowSamplingInterval = settings.SlowSamplingInterval;
            doc.DisableWhenNotUsed = settings.DisableWhenNotUsed;
            doc.PublishingInterval = settings.PublishingInterval;
        }

        public Dictionary<String, IAppNameSettings> GetMapAppNameSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            return doc.MapAppNameSettings.Keys.ToDictionary(k => k, v => (IAppNameSettings)doc.MapAppNameSettings[v]);
        }

        public void ClearMapAppNameSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            doc.MapAppNameSettings = new Dictionary<string, OPCUAViewModel.AppNameSettings>();
        }
#endif
        #endregion
    }
}