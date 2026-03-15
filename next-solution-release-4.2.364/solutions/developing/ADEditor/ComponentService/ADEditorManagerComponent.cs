using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UIMsgBoxAlertService.ComponentService;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ADEditor.Document;
using UFInterfaces.AuthenticationCredentialsProvider;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;
using WPFUtilities;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using DevExpress.Xpo;
using XpoHelpers;
using ADEditor.PropertyDataTemplate;
using CommonControls.PropertyDataTemplate;
using System.Windows.Threading;
using UFUAEditor.ComponentService;
using OPCUAViewModel;
using DocumentManager.ComponentService.Helpers;
using DevExpress.Xpf.Bars;
using HelpProvider.ComponentService;

namespace ADEditor.ComponentService
{
    public class ADEditorManagerComponent : ComponentBase<IADEditorManager>, IADEditorManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ADEditorDocument> mapActiveDocuments = new Dictionary<String, ADEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ADEditorDocument, String> mapActiveDocumentUris = new Dictionary<ADEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        public static IWorkspace workspaceService { get; protected set; }
        public static ADEditorManagerComponent adeditorManagerComponent { get; protected set; }
        MenuControl menuControl;

        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (adeditorManagerComponent == null)
                adeditorManagerComponent = this;

            GetComponentInterfaces();
        }

        #endregion IUFInterfaceBase Members

        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }

        private void GetComponentInterfaces()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));

            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.Closing += workspace_Closing;
            workspace.Closed += workspace_Closed;
            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            workspace.PromptDocumentEditorObject += workspace_PromptDocumentEditorObject;

            if (PropertyControl != null)
            {
                // Configuration Data Templates
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, HelpProvider);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("HistorianDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("EventDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(RecipientPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Recipient", typeof(string), typeof(ADModel.ADNotification), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PluginNamePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("PluginName", typeof(string), typeof(ADModel.ADNotification), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(AttachmentsPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Attachments", typeof(string), typeof(ADModel.ADNotification), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NotificationItemPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("NotificationItem", typeof(string), typeof(ADModel.ADNotification), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TextPropertyEditor));
                factory.SetValue(WPFUtilities.PropertyDataTemplate.TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Message", typeof(string), typeof(ADModel.ADNotification), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TextPropertyEditor));
                factory.SetValue(WPFUtilities.PropertyDataTemplate.TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Name", typeof(string), typeof(ADModel.ADNotification), dt);

                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }
        }

        void PropertyControl_AcceptChanges(object sender, EventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var list = (from c in mapActiveDocuments.Values// .AsParallel()
                            where c.NeedsSave == true && c.ActiveView == null
                            select c).ToList();
                list.ForEach(doc => doc.SaveToFile());
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is ADEditorControl))
                return;

            var view = e.TargetItem as ADEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(ADEditorControl view, bool bSave = true)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!view.Document.CanClose())
                    return false;

                if (bSave && view.Document.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            String.Format("{0} ({1})", TypeTitle, view.Document.Parent.Title)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                    {
                        if (!view.Document.SaveToFile())
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseView(ADEditorControl view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!CanClose(view, bSave))
                {
                    return false;
                }

                CloseAllChild(view.Document);

                mapActiveDocumentUris.Remove(view.Document);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
            }

            if (workspace.ContextDocument == view.Document)
                workspace.ContextDocument = null;

            view.Document.PropertyChanged -= Document_PropertyChanged;

            workspace.RemoveDockingChildren(view);
            if (view.Document is IDisposable)
            {
                view.Document.SaveToFile(true);
                (view.Document as IDisposable).DisposeInApplicationIdle();
            }

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new ADEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as ADEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new ADEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as ADEditorControl;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is ADEditorControl && viewOld != null)
            {
                var view = e.OldValue as ADEditorControl;
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is ADEditorControl && viewNew != null)
            {
                var view = e.NewValue as ADEditorControl;
                workspace.ContextDocument = view.Document;
                view.OnActivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
            }
        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (!(sender is IXPSimpleObject))
                return;

            IXPSimpleObject source = sender as IXPSimpleObject;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null || Doc.UowContext == null || !XpoHelper.IsSessionObject(source, Doc.UowContext))
                    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        String GetDocumentTitle(Uri uri)
        {
            lock (lockObject)
            {
                String ret = Path.GetFileNameWithoutExtension(uri.GetPathString());
                String sourcefmt = ret;
                int i = 1;
                while (mapActiveDocumentTitles.ContainsValue(ret))
                    ret = String.Format("{0}{1}", sourcefmt, i++);

                mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        internal ADEditorControl GetViewFromUri(Uri uri)
        {
            ADEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as ADEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, ADEditorDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
        }

        internal String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

        internal void CreateDefaultDocument(Uri uri, bool encryptFile = false)
        {
        }

        #region Properties
        //IStringEditorManager stringEditorManager;
        //public IStringEditorManager StringEditorManager
        //{
        //    get { return stringEditorManager; }
        //}

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                return workspace;
            }
        }

        IPropertyControl propertyControl;
        public IPropertyControl PropertyControl
        {
            get
            {
                if (propertyControl == null)
                    propertyControl = GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyControl;
            }
        }

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
        IStringEditorManager stringEditor;
        public IStringEditorManager StringEditor
        {
            get
            {
                if (stringEditor == null)
                    stringEditor = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditor;
            }
            set { stringEditor = value; }
        }

        IUFUAEditorManager ufuaEditorService;
        public IUFUAEditorManager UfuaEditorService
        {
            get
            {
                if (ufuaEditorService == null)
                    ufuaEditorService = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                return ufuaEditorService;
            }
        }

        IUFUserEditorManager userManager;
        public IUFUserEditorManager UserManager
        {
            get
            {
                if (userManager == null)
                    userManager = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return userManager;
            }
        }

        IHelpProvider helpProvider;
        public IHelpProvider HelpProvider
        {
            get
            {
                if (helpProvider == null)
                    helpProvider = GetService(typeof(IHelpProvider)) as IHelpProvider;
                return helpProvider;
            }
        }
        #endregion Properties

        #region IDocumentManager Members

        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ADEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as ADEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = ADEditorDocument.FromFile(uri.GetPathString(), this, parent);

                    if (doc != null && doc.ActiveView == null)
                    { 
                        ADEditorControl adEditor = new ADEditorControl(doc);
                        doc.Parent = parent;
                        doc.ActiveView = adEditor;
                        if (doc.NeedsSave)
                            doc.SaveToFile();

                        workspace.SetDesiredHeightAndWidthInDockedMode(adEditor, adEditor.Height, adEditor.Width);
                        adEditor.ClearValue(FrameworkElement.WidthProperty);
                        adEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("ADEditorSmall");

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        workspace.AddDockingChildren(adEditor,
                            String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(adEditor, new ImageBrush(bm));

                        workspace.FlashDockedElement(adEditor);

                        adEditor.Document.PropertyChanged += Document_PropertyChanged;
                    }
                }
            }
        }

        readonly List<ADEditorDocument> listStartedDocuments = new List<ADEditorDocument>();
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            ADEditorDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ADEditorDocument.FromFile(uri.GetPathString(), this, parent, false);
                if (doc == null)
                    return;
                doc.Parent = parent;
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }
            else if (doc.IsEmpty)
                return;

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
            if (commandArgs.ArgPairs.ContainsKey("client"))
                return;

            //var startupControl = new Controls.StartupControl();
            //var wnd = new DevExpress.Xpf.Core.DXWindow()
            //{
            //    BorderEffect = DevExpress.Xpf.Core.BorderEffect.Default,
            //    ShowInTaskbar = false,
            //    ResizeMode = System.Windows.ResizeMode.NoResize,
            //    WindowStyle = WindowStyle.None,
            //    WindowState = WindowState.Normal,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Content = startupControl,
            //    SizeToContent = SizeToContent.WidthAndHeight
            //};

            //ThemeHelper.SetTheme(wnd);

            //var canClose = false;
            //wnd.Closing += (o, e) =>
            //{
            //    e.Cancel = !canClose;
            //};

            //wnd.Show();
            if (!doc.ServerCMSHelperSync.IsServerRunning)
            {
                try
                {
                    if (doc.StartServer(/*startupControl.txtevent, startupControl.Scroll, */false))
                    {
                        var dateTime = DateTime.Now.AddSeconds(10);
                        while (!doc.ServerCMSHelperSync.IsServerRunning && dateTime > DateTime.Now)
                            WaitForPriority.DoEventsSync();
                        while (!doc.ServerCMSHelperSync.IsServerStarted && dateTime > DateTime.Now)
                            WaitForPriority.DoEventsSync();

                        if (!listStartedDocuments.Contains(doc))
                            listStartedDocuments.Add(doc);
                    }
                }
                catch (Exception ex)
                {
                    if (UIInterface != null)
                    {
                        UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                            ADServerInfo.ADServerInfo.GetServerName(), ex.Message));
                    }
                }
            }
            //canClose = true;
            //wnd.Close();
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            ADEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                if (listStartedDocuments.Contains(doc))
                {
                    listStartedDocuments.Remove(doc);
                    if (!doc.ServerCMSHelperSync.IsServerRunningAsService)
                        doc.ServerCMSHelperSync.StopServer();
                }
            }  
        }

        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                var ret = document.SaveToFile();
                if (!ret)
                {
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocWithoutClosure,
                        String.Format("{0} ({1})", TypeTitle, document.Parent.Title)));
                }
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseView(document.ActiveView as ADEditorControl))
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
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();
            foreach (var document in list)
            {
                if (document.NeedsSave)
                    return true;
            }

            return false;
        }

        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    // if (!bCopy)
                    {
                        ADEditorDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as ADEditorControl, false);
                    }

                    ADEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ADEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as ADEditorControl, false);

                    ADEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
                }
            }
        }

        public void Delete(Uri uri, IDocument parent)
        {
            //if (UIInterface != null)
            //{
            //    if (UIInterface.ShowOkCancel(String.Format(Properties.Resources.ConfirmRemove,
            //        GetDocumentTitle(uri)), CustomDialogIcons.Exclamation) == CustomDialogResults.Cancel)
            //        return;
            //}

            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ADEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as ADEditorControl, false);

                    ADEditorDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = ADEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
            {
                doc.Parent = parent;
                using (doc)
                {
                    return doc.SaveToFile(bForceSave: true, forceEncryption: encryptFile);
                }
            }
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }

        public IDocument GetDocument(Uri uri)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()];
            return null;
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            var list = new ObservableCollection<IDocumentManager>();
            list.Add(new TreeDocumentManagers.PluginListDocumentManager(this, doc));
            list.Add(new TreeDocumentManagers.EventListDocumentManager(this, doc));
            return list;
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null || doc.IsEmpty)
                return null;

            String serverConn = String.Empty;
            String stringConn = String.Empty;
            String userConn = String.Empty;
            if (doc.FilePath != null)
            {
                serverConn = DevExpress.Xpo.DB.InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", doc.FilePath));

                if (StringEditor != null)
                    stringConn = StringEditor.GetConnectionStringFromFile(doc.rootBase);
                if (UserManager != null)
                    userConn = UserManager.GetConnectionStringFromFile(doc.rootBase);
            }
            else
                serverConn = userConn = stringConn = doc.ConnectionString;

            try
            {
                var srvrConfigurationId = doc.GetServerIOConfigurationId();
                var applicationName = doc.GetConfiguration().ApplicationName;
                return new Service.ServiceControl(applicationName, serverConn, stringConn, userConn, srvrConfigurationId);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }

        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);
            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }

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
                return Properties.Settings.Default.TypeLabel;
            }
        }

        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("ADEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("ADEditor");
            }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }

        public String TypeScheme
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
            }
        }

        public String FileType
        {
            get
            {
                return Properties.Settings.Default.DefaultFileExt;
            }
        }

        public String FileName
        {
            get
            {
                return Properties.Settings.Default.DefaultProjectName;
            }
        }

        public String[] SaveAsFileExtensions
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SaveAsFileExtensions))
                    return Properties.Settings.Default.SaveAsFileExtensions.ToLower().Split(';');
                return null;
            }
        }

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

        public bool IsResourceExpandable(IDocument document = null)
        {
            return true;
        }

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, encryptFile);
                return uri;
            }

            String path, newProjectName;
            int i = 0;
            do
            {
                newProjectName = String.Format("{0}{1}", Properties.Settings.Default.DefaultProjectName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newProjectName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            Uri url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, encryptFile);
            return url;
        }

        public Type DocumentType
        {
            get
            {
                return typeof(ADEditorDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
        }

        #endregion IDisposable Members
        internal ADEditorDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            ADEditorDocument doc = null;
            doc = ADEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
        internal ADEditorDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, ADEditorDocument> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                ADEditorDocument doc = null;
                // if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                if (list.Count == 0 || bRefresh)
                {
                    doc = ADEditorDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = p;
                    if (!map.ContainsKey(uri.GetPathString()))
                        map.Add(uri.GetPathString(), doc);
                }
                else
                    doc = list[0];
                return doc;
            }
        }
        
        #region ICrossReference
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            bool getStrings = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getTags && !getConnections && !getStrings)
                return result;
            string docType = DocManagerType.AlarmDispatcher.ToString();
            var p = DocumentHelper.GetRootParent(model.Parent, true);
            using (ADEditorDocument doc = CreateDocument(model.Parent))
            {
                if (doc != null)
                {
                    if(getTags || getStrings)
                    {
                        List<ADModel.ADNotification> taglist = doc.GetCRFlatTagCollection() as List<ADModel.ADNotification>;
                        for (int j = 0; j < taglist.Count(); j++)
                        {
                            var tag = taglist[j];
                            if(getTags)
                            {
                                if (tag.NotificationType == ADModel.NotificationTypes.Server)
                                    continue;
                                if (model.QuitEvent.IsCancellationRequested)
                                    return result;
                                if (string.IsNullOrEmpty(tag.NotificationItem))
                                    continue;
                                OPCUAEntityReference item = tag.NotificationItem.FromXml<OPCUAEntityReference>();
                                string relativePath = item.RelativePath;
                                var name = relativePath.Split('/').LastOrDefault();
                                var appname = item.AppName;
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = appname,
                                    ReferencedNodeId = item.ResolvedNodeId?.Identifier.ToString(),
                                    EndpointUrl = item.EndpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{0}", tag.Name),
                                    Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                            }
                            if (getStrings)
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = tag.Name,
                                    Name = tag.Name,
                                    AppName = p.Title,
                                    CReferenceType = CrossReferenceType.Strings,
                                    Description = string.Format("{0}\\{1} ({2})", doc.Title, tag.Name, Properties.Resources.NotificationTreeViewName),
                                    Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                                if (!string.IsNullOrEmpty(tag.Message))
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = tag.Message,
                                        Name = tag.Message,
                                        AppName = p.Title,
                                        CReferenceType = CrossReferenceType.Strings,
                                        Description = string.Format("{0}\\{1} ({2})", doc.Title, tag.Name, Properties.Resources.NotificationTreeViewMessage),
                                        Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                            }
                        }

                    }
                    if(getConnections)
                    {
                        var settings = doc.GetGeneralSettings();
                        if (settings != null && !string.IsNullOrEmpty(settings.EventDefaultConnection))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = XpoHelper.NormalizeConnectionString(settings.EventDefaultConnection, doc.rootBase), //$"{TypeTitle}",
                                Name = Properties.Resources.DefConnectionString,
                                AppName = $"{TypeTitle}",
                                CReferenceType = CrossReferenceType.Connections,
                                Description = TypeTitle, //settings.EventDefaultConnection,
                                Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                    }
                }
            }

            return result;
        }

        public void EditCRObject(IDocument parent, string settings)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            string[] path = settings.Split('|');
            if (path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                Edit(uri, p);
                Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
                {
                    GetActiveView(uri).SelectNotificationTab();
                });
            }
        }
        #endregion
        public ADEditorControl GetActiveView(Uri uri)
        {
            ADEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as ADEditorControl;
            return null;
        }

        public bool NeedToCloseCRDocuments()
        {
            return mapActiveDocuments.Count > 0 && (from d in mapActiveDocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null;
        }

        public bool NeedSingleThreadedApartment
        {
            get
            {
                return false;
            }
        }

        public void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.QuitEvent.IsCancellationRequested)
                return;
            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            using (ADEditorDocument doc = CreateDocument(model.Parent))
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                if (doc != null)
                    doc.RenameReferences(model);
            }
        }
    }
}