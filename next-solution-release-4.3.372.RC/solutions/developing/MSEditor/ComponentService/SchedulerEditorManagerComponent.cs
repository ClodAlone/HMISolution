using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using WPFUtilities.PropertyDataTemplate;
using CommonControls.PropertyDataTemplate;
using System.Windows.Threading;
using OPCUAViewModel.PropertyDataTemplate;
using MSEditor.ActionCommands;
using CommandManager;
using MSEditor.PropertyDataTemplate;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using MSSchedulerSettings.Document;
using UFInterfaces.AuthenticationCredentialsProvider;
using StringManager.ComponentService;
using MSSchedulerSettings.ComponentService;
using UFUserEditor.ComponentService;
using DevExpress.Xpo;
using XpoHelpers;
using System.Windows.Input;
using UFUAEditor.ComponentService;
using OPCUAViewModel;
using DocumentManager.ComponentService.Helpers;
using log4net;
#if !NET_STANDARD
using HelpProvider.ComponentService;
using MSModel;
#endif
namespace MSEditor.ComponentService
{
    public class SchedulerEditorManagerComponent : ComponentBase<ISchedulerEditorManager>, ISchedulerEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
#region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, SchedulerEditorDocument> mapActiveDocuments = new Dictionary<String, SchedulerEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<SchedulerEditorDocument, String> mapActiveDocumentUris = new Dictionary<SchedulerEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

#if !NET_STANDARD
        readonly Dictionary<String, ActionCommandsEngine> mapRunningActionCommandsEngine = new Dictionary<String, ActionCommandsEngine>(StringComparer.OrdinalIgnoreCase);
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif

        public static SchedulerEditorManagerComponent schedulereditorManagerComponent { get; protected set; }
#if !NET_STANDARD
        MenuControl menuControl;
#endif
        bool isGettingChilds;
#endregion Declaration

#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (schedulereditorManagerComponent == null)
                schedulereditorManagerComponent = this;

            GetComponentInterfaces();
        }

#endregion IUFInterfaceBase Members
#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(MSSchedulerSettings.Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }
#endif
        private void GetComponentInterfaces()
        {
#if !NET_STANDARD
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = MSSchedulerSettings.Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));

            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.Closing += workspace_Closing;
            workspace.Closed += workspace_Closed;
            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            workspace.PromptFriendObjects += workspace_PromptFriendObjects;
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
                PropertyControl.AddPropertyEditor("UserConnectionString", typeof(string), typeof(MSModel.MSGeneralSettings), dt);
                PropertyControl.AddPropertyEditor("HistorianDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("EventDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                factory.SetValue(OPCUAEntityReferencePropertyEditorFromXML.AllowDataSyncProperty, false);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ScheduleItem", typeof(string), typeof(MSModel.MSScheduledAction), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                factory.SetValue(OPCUAEntityReferencePropertyEditorFromXML.AllowDataSyncProperty, false);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("EnableVariable", typeof(string), typeof(MSModel.MSScheduledAction), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("AccessMasks", typeof(int), typeof(MSModel.MSScheduledAction), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimePropertyEditor));
                dt.DataType = typeof(DateTime);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Time", typeof(DateTime), typeof(MSModel.MSScheduledAction), dt);
                PropertyControl.AddPropertyEditor("TimeOff", typeof(DateTime), typeof(MSModel.MSScheduledAction), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("CommandsOn", typeof(string), typeof(MSModel.MSScheduledAction), dt);
                PropertyControl.AddPropertyEditor("CommandsOff", typeof(string), typeof(MSModel.MSScheduledAction), dt);
                PropertyControl.AddPropertyEditor("ExceptionCommandsOn", typeof(string), typeof(MSModel.MSScheduledAction), dt);
                PropertyControl.AddPropertyEditor("ExceptionCommandsOff", typeof(string), typeof(MSModel.MSScheduledAction), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SchedulerExceptionsPropertyEditor));
                factory.SetValue(SchedulerExceptionsPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("SchedulerExceptions", typeof(bool), typeof(MSModel.MSScheduledAction), dt);

                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (UserEditor != null)
                    {
                        Type accRoleType = UserEditor.GetAccessRoleEditorType();
                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(accRoleType);
                        dt.DataType = typeof(string);
                        dt.VisualTree = factory;
                        PropertyControl.AddPropertyEditor("AccessRole", typeof(string), typeof(MSModel.MSScheduledAction), dt);
                    }
                });
                
                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }
#endif                
        }

#if !NET_STANDARD
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
            if (!(e.TargetItem is SchedulerEditorControl))
                return;

            var view = e.TargetItem as SchedulerEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(SchedulerEditorControl view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!view.Document.CanClose())
                    return false;

                if (bSave && view.Document.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(MSSchedulerSettings.Properties.Resources.SaveDoc,
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

        private bool CloseView(SchedulerEditorControl view, bool bSave = true)
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

            var array = new SchedulerEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as SchedulerEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new SchedulerEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as SchedulerEditorControl;
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

            if (e.OldValue != null && e.OldValue is SchedulerEditorControl && viewOld != null)
            {
                var view = e.OldValue as SchedulerEditorControl;
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is SchedulerEditorControl && viewNew != null)
            {
                var view = e.NewValue as SchedulerEditorControl;
                workspace.ContextDocument = view.Document;
                view.OnActivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
            }
        }

        void workspace_PromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            if (!(sender is MSModel.MSScheduledAction))
                return;
            var source = sender as MSModel.MSScheduledAction;

            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null || Doc.UowContext == null || !XpoHelper.IsSessionObject(source, Doc.UowContext))
                    continue;

                if (e.friendList == null)
                    e.friendList = new List<Object>();

                e.friendList.Add(new ActionCommandsEditObject(source, ActionCommandsEventType.CommandsOn));
                e.friendList.Add(new ActionCommandsEditObject(source, ActionCommandsEventType.CommandsOff));
                e.friendList.Add(new ActionCommandsEditObject(source, ActionCommandsEventType.CommandsOnEx));
                e.friendList.Add(new ActionCommandsEditObject(source, ActionCommandsEventType.CommandsOffEx));

                break;
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
#endif

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

#if !NET_STANDARD
        internal SchedulerEditorControl GetViewFromUri(Uri uri)
        {
            SchedulerEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as SchedulerEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, SchedulerEditorDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
        }
#endif

        internal String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

        internal void CreateDefaultDocument(Uri uri, bool encryptFile = false)
        {
        }

#region Properties
#if !NET_STANDARD
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
#if !NET_STANDARD
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
#endif

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
#endif
        #endregion Properties

        #region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    SchedulerEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as SchedulerEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);

                    if (doc != null && doc.ActiveView == null)
                    { 
                        SchedulerEditorControl schedulerEditor = new SchedulerEditorControl(doc);
                        doc.Parent = parent;
                        doc.ActiveView = schedulerEditor;
                        if (doc.NeedsSave)
                            doc.SaveToFile();

                        workspace.SetDesiredHeightAndWidthInDockedMode(schedulerEditor, schedulerEditor.Height, schedulerEditor.Width);
                        schedulerEditor.ClearValue(FrameworkElement.WidthProperty);
                        schedulerEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("SSEditorSmall");

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        workspace.AddDockingChildren(schedulerEditor,
                            String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(schedulerEditor, new ImageBrush(bm));

                        workspace.FlashDockedElement(schedulerEditor);

                        schedulerEditor.Document.PropertyChanged += Document_PropertyChanged;
                    }
                }
            }
        }
#endif

        readonly List<SchedulerEditorDocument> listStartedDocuments = new List<SchedulerEditorDocument>();
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            SchedulerEditorDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent, false);
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

#if !NET_STANDARD
            var scheduledActions = (from c in doc.GetCompleteEventsList()
                                    where c.Enable.Value && c.HasCommands
                                    select c).ToList();

            if (scheduledActions.Count > 0)
            {
                var actionCommandsEngine = new ActionCommandsEngine(Dispatcher.CurrentDispatcher, this, parent);
                mapRunningActionCommandsEngine.Add(uri.GetPathString(), actionCommandsEngine);
                foreach (var action in scheduledActions)
                    actionCommandsEngine.Add(action.NodeId.ToString(), action);
                if (!actionCommandsEngine.IsEmpty)
                    actionCommandsEngine.Init(parent.Title);
            }

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
                            MSServerInfo.MSServerInfo.GetServerName(), ex.Message));
                    }
                }
            }
            //canClose = true;
            //wnd.Close();
#endif
        }

        public void PreTerminate(Uri uri, IDocument parent)
        {
#if !NET_STANDARD
            ActionCommandsEngine engine = null;
            if (mapRunningActionCommandsEngine.TryGetValue(uri.GetPathString(), out engine))
            {
                mapRunningActionCommandsEngine.Remove(uri.GetPathString());
                engine.Dispose();
            }
#endif
        }

        public void Terminate(Uri uri, IDocument parent)
        {
            SchedulerEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                if (listStartedDocuments.Contains(doc))
                {
                    listStartedDocuments.Remove(doc);
#if !NET_STANDARD
                    if (!doc.ServerCMSHelperSync.IsServerRunningAsService)
                        doc.ServerCMSHelperSync.StopServer();
#endif
                }
            }
        }

#if !NET_STANDARD
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
                if (!CloseView(document.ActiveView as SchedulerEditorControl))
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
                        SchedulerEditorDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as SchedulerEditorControl, false);
                    }

                    SchedulerEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    SchedulerEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as SchedulerEditorControl, false);

                    SchedulerEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
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
                    SchedulerEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as SchedulerEditorControl, false);

                    SchedulerEditorDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);
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
#endif

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

#if !NET_STANDARD
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        // ObservableCollection<IDocumentManager> list;

        static readonly int maxItems = Properties.Settings.Default.MaxItemsInTree;

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            return GetChilds(uri, parent);
        }

        internal ObservableCollection<IDocumentManager> GetChilds(Uri uri, IDocument parent, bool bCap = true)
        {
            if (isGettingChilds)
                return null;

            isGettingChilds = true;
            var list = new ObservableCollection<IDocumentManager>();
            try
            {
                SchedulerEditorDocument doc = null;
                if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                        return null;
                    doc.Parent = parent;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }

                using (new WaitCursor())
                {
                    bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                    bool bGetAll = bShiftDown || !bCap;

                    //if (list == null)
                    //    list = new ObservableCollection<IDocumentManager>();
                    //list.Clear();

                    int i = 0;
                    foreach (var name in doc.GetFolderCollection())
                    {
                        list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(this,
                                                    doc, name, this));
                        if (!bGetAll && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(this,
                                                        doc, this));
                            break;
                        }
                    }
                    i = 0;
                    foreach (var name in doc.GetEventsCollection())
                    {
                        list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(this,
                                                    doc, name, this));
                        if (!bGetAll && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(this,
                                                        doc, this));
                            break;
                        }
                    }
                }
            }
            finally
            {
                isGettingChilds = false;
            }
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
                if (UserEditor != null)
                    userConn = UserEditor.GetConnectionStringFromFile(doc.rootBase);
            }
            else
                serverConn = stringConn = userConn = doc.ConnectionString;

            try
            {
                var applicationName = doc.GetConfiguration().ApplicationName;
                return new Service.ServiceControl(applicationName, serverConn, stringConn, userConn);
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
                return MSSchedulerSettings.Properties.Settings.Default.TypeLabel;
            }
        }

#if !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("SSEditorSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                return TypeIcon;
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("SSEditor");
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
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
            }
        }

        public String FileType
        {
            get
            {
                return MSSchedulerSettings.Properties.Settings.Default.DefaultFileExt;
            }
        }

        public string FileName
        {
            get { return MSSchedulerSettings.Properties.Settings.Default.DefaultProjectName;}
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

#if !NET_STANDARD
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

#if !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            if (document == null)
                return false;
            var doc = GetOrCreateDocument(document);
            return doc == null || doc.IsDisposed ? false : doc.GetFolderCollection().Count > 0 || doc.GetEventsCollection().Count > 0;
        }
#endif

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

#if !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, MSSchedulerSettings.Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, encryptFile);
                return uri;
            }

            String path, newProjectName;
            int i = 0;
            do
            {
                newProjectName = String.Format("{0}{1}", MSSchedulerSettings.Properties.Settings.Default.DefaultProjectName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newProjectName, MSSchedulerSettings.Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            Uri url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, encryptFile);
            return url;
        }
#endif

        public Type DocumentType
        {
            get
            {
                return typeof(SchedulerEditorDocument);
            }
        }

#endregion IDocumentManager Members

#region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
#if !NET_STANDARD
            foreach (IDisposable engine in mapRunningActionCommandsEngine.Values)
                engine.Dispose();
            mapRunningActionCommandsEngine.Clear();

            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects -= workspace_PromptFriendObjects;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
#endif
        }

#endregion IDisposable Members

        private SchedulerEditorDocument GetOrCreateDocument(IDocument parent)
        {
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

            SchedulerEditorDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);
                if (doc == null)
                {
                    return null;
                }
                doc.Parent = DocumentHelper.GetRootParent(parent, traverse: false);
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }
            return doc;
        }

#region ISchedulerEditorManager Members

        public String GetServerEntityReference(IDocument parent, bool bCheckEmpty = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null || (bCheckEmpty && doc.IsEmpty))
                return null;
            try
            {
                return doc.GetServerOPCUAEntityReference().ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#endif
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }

        public String GetSchedulerEntityReference(IDocument parent, String nodeId)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                var action = doc.GetSchedulerEntityReference(nodeId);
                if (action == null)
                    return null;

                return action.ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#endif
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }

        public IList<String> GetFlatEventsList(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetFlatEventsList(true);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#endif
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }
#endregion


#region ICrossReference
#if !NET_STANDARD
        internal SchedulerEditorDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            SchedulerEditorDocument doc = null;
            doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
        internal SchedulerEditorDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, SchedulerEditorDocument> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                SchedulerEditorDocument doc = null;
                // if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                if (bRefresh && list.Count > 0)
                {
                    list.ForEach(d =>
                    {
                        DisposeDocument(d);
                    });
                    list.Clear();
                }
                if (list.Count == 0)
                {
                    doc = SchedulerEditorDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
                else
                    doc = list[0];
                return doc;
            }
        }
        void DisposeDocument(SchedulerEditorDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            if (doc is IDisposable)
                (doc as IDisposable).Dispose();
        }
        void RemoveActiveDocumentUri(SchedulerEditorDocument doc)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                mapActiveDocumentUris.Remove(doc);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
            }
        }
        
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            if (!getScreens && !getTags && !getConnections)
                return result;
            string docType = DocManagerType.SchedulerEditor.ToString();
            using (SchedulerEditorDocument doc = CreateDocument(model.Parent))
            {
                if (doc != null)
                {
                    string appname = doc.GetAplicationName();
                    List<MSModel.MSScheduledAction> taglist = doc.GetCRFlatTagCollection() as List<MSModel.MSScheduledAction>;
                    for (int j = 0; j < taglist.Count(); j++)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return result;

                        var tag = taglist[j];
                        if (getTags && !string.IsNullOrEmpty(tag.ScheduleItem))
                        {
                            string relativePath = string.Empty;
                            string nodeid = string.Empty;
                            string endpointUrl = string.Empty;
                            try
                            {
                                OPCUAEntityReference item = tag.ScheduleItem.FromXml<OPCUAEntityReference>();
                                nodeid = item.ResolvedNodeId?.Identifier.ToString();
                                endpointUrl = item.EndpointUrl;
                                relativePath = item.RelativePath;
                                appname = item.AppName;
                            }
                            catch
                            {
                                continue;
                            }

                            var name = relativePath.Split('/').LastOrDefault();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = relativePath,
                                Name = name,
                                AppName = appname,
                                ReferencedNodeId = nodeid,
                                EndpointUrl = endpointUrl,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0} ({1})", tag.Name, Properties.Resources.NewEventTag),
                                Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            }); 
                        }

                        if (getTags && !string.IsNullOrEmpty(tag.EnableVariable))
                        {
                            string relativePath = string.Empty;
                            string nodeid = string.Empty;
                            string endpointUrl = string.Empty;
                            try
                            {
                                OPCUAEntityReference item = tag.EnableVariable.FromXml<OPCUAEntityReference>();
                                nodeid = item.ResolvedNodeId?.Identifier.ToString();
                                endpointUrl = item.EndpointUrl;
                                relativePath = item.RelativePath;
                                appname = item.AppName;
                            }
                            catch
                            {
                                continue;
                            }

                            var name = relativePath.Split('/').LastOrDefault();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = relativePath,
                                Name = name,
                                AppName = appname,
                                ReferencedNodeId = nodeid,
                                EndpointUrl = endpointUrl,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0} ({1})", tag.Name, Properties.Resources.NewEventEnableTag),
                                Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                        }

                        if (!string.IsNullOrEmpty(tag.CommandsOn))
                            result.AddRange(GetCommandsTagToList(model, tag.CommandsOn.FromXml<CommandManagerList>(), doc, Properties.Resources.NewEventCommandTagOn, tag.Name));
                        if (!string.IsNullOrEmpty(tag.CommandsOff))
                            result.AddRange(GetCommandsTagToList(model, tag.CommandsOff.FromXml<CommandManagerList>(), doc, Properties.Resources.NewEventCommandTagOff, tag.Name));
                        if (!string.IsNullOrEmpty(tag.ExceptionCommandsOn))
                            result.AddRange(GetCommandsTagToList(model, tag.ExceptionCommandsOn.FromXml<CommandManagerList>(), doc, Properties.Resources.NewEventExceptionCommandTagOn, tag.Name));
                        if (!string.IsNullOrEmpty(tag.ExceptionCommandsOff))
                            result.AddRange(GetCommandsTagToList(model, tag.ExceptionCommandsOff.FromXml<CommandManagerList>(), doc, Properties.Resources.NewEventExceptionCommandTagOff, tag.Name));
                    }

                    if (getConnections)
                    {
                        var settings = doc.GetGeneralSettings();
                        if (settings != null && !string.IsNullOrEmpty(settings.EventDefaultConnection))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = XpoHelper.NormalizeConnectionString(settings.EventDefaultConnection, doc.rootBase), //$"{TypeTitle}",
                                Name = $"{Properties.Resources.DefConnectionString}",
                                AppName = $"{TypeTitle}",
                                CReferenceType = CrossReferenceType.Connections,
                                Description = TypeTitle,
                                Settings = string.Format("{0}|{1}|{2}", docType, doc.rootBase, Properties.Resources.RefrencedBy),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                    }
                }
            }

            return result;
        }

        private List<UFInterfaces.Editors.CrossReferenceResultModel> GetCommandsTagToList(UFInterfaces.Editors.CrossReferenceModel model, CommandManagerList commandsList, IDocument doc, string type, string schedName)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            if (!getTags && !getScreens)
                return result;

            var appName = UfuaEditorService.GetAplicationName(doc);
            var parent = DocumentHelper.GetRootParent(doc.Parent, traverse: false);
            string root = doc.rootBase;
            string docType = DocManagerType.SchedulerEditor.ToString();
            if (commandsList != null)
                commandsList.ForEach(command =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;
                    
                    if (getTags && !string.IsNullOrEmpty(command.Expression))
                    {
                        List<string> expressionTags = Utilities.Converters.ExpressionValueConverterHelper.GetListVarInExpression(command.Expression);
                        //Parallel.ForEach(expressionTags, expression => 
                        for (int j = 0; j < expressionTags.Count; j++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var expression = expressionTags[j];
                            OPCUAEntityReference tag = GetReferenceTag(doc, expression);

                            if (tag == null)
                                tag = new OPCUAEntityReference() { RelativePath = expression, AppName = appName };

                            //lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = tag.RelativePath,
                                    Name = tag.Name,
                                    AppName = tag.AppName,
                                    ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                                    EndpointUrl = tag.EndpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{0} ({1} - {2} {3})", schedName, type, Properties.Resources.ItemsExpressionTagHeader, expressionTags.IndexOf(expression)),
                                    Settings = string.Format("{0}|{1}", DocManagerType.EventEditor, doc.rootBase),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                        }//);
                    }
                    if (getScreens)
                    {
                        var properties = CommandManager.Extensions.CommandManagerExtensions.GetBrowsablePropertiesOfType<Uri>(command);
                        foreach (PropertyInfo prop in properties)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var uri = prop.GetValue(command) as Uri;
                            if (uri != null && uri.GetPathString() != null)
                            {
                                string relativePath = uri.GetPathString();
                                var name = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = parent.Title,
                                    CReferenceType = CrossReferenceType.Resources,
                                    Description = string.Format("{0} ({1})", schedName, type),
                                    Settings = string.Format("{0}|{1}", docType, root),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                            }
                        }
                    }
                    if(getTags)
                    {
                        var list = command.ListTags;
                        foreach (var tag in list)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            string appname = tag.AppName;
                            string relativePath = tag.RelativePath;
                            var name = relativePath.Split('/').LastOrDefault();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = relativePath,
                                Name = name,
                                AppName = appname,
                                ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                                EndpointUrl = tag.EndpointUrl,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0} ({1})", schedName, type),
                                Settings = string.Format("{0}|{1}", docType, root),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                        }
                    }
                });
            return result;
        }

        private OPCUAEntityReference GetReferenceTag(IDocument doc, string reference)
        {
            OPCUAEntityReference tag = null;
            var split = reference.Split('-');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;
            var xml = UfuaEditorService.GetTagEntityReference(doc, name, instance, useCachedUow: true);
            if (!String.IsNullOrEmpty(xml))
                tag = xml.FromXml<OPCUAEntityReference>();

            return tag;
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
        public SchedulerEditorControl GetActiveView(Uri uri)
        {
            SchedulerEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as SchedulerEditorControl;
            return null;
        }
#endif
#endregion
#if !NET_STANDARD
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
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            //using (SchedulerEditorDocument doc = CreateDocument(model.Parent))
            var doc = GetOrCreateDocument(model.Parent);
            if(doc != null)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                if (doc != null)
                    doc.RenameReferences(model);
            }
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
#endif
    }
}