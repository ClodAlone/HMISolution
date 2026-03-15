using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities.Extensions;
using CommandExplorer.ComponentService;
using PropertyControl.ComponentService;
using UFEventEditor.PropertyDataTemplate;
using OPCUAViewModel.PropertyDataTemplate;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using UFEventEditor.Document;
using UFInterfaces.AuthenticationCredentialsProvider;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;
using WPFUtilities;
using DevExpress.Xpo;
using XpoHelpers;
using System.Windows.Input;
using OPCUAViewModel;
using UFEventModel;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;
using System.Threading.Tasks;
#if !NET_STANDARD
using WPFUtilities.PropertyDataTemplate;
#endif
namespace UFEventEditor.ComponentService
{
    public class EventEditorManagerComponent : ComponentBase<IEventEditorManager>, IEventEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, EventEditorDocument> mapActiveDocuments = new Dictionary<String, EventEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<EventEditorDocument, String> mapActiveDocumentUris = new Dictionary<EventEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<String, EventEditorDocument> mapRunningDocuments = new Dictionary<String, EventEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<String, EventsThread> mapRunningEngine = new Dictionary<String, EventsThread>();

        public static EventEditorManagerComponent eventeditorManagerComponent { get; protected set; }

#if !NET_STANDARD
        MenuControl menuControl;
        bool isGettingChilds;
#endif
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (eventeditorManagerComponent == null)
                eventeditorManagerComponent = this;

            GetComponentInterfaces();
        }

        #endregion IUFInterfaceBase Members
#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }
#endif
        private void GetComponentInterfaces()
        {
#if !NET_STANDARD
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
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Tag", typeof(string), typeof(UFEventModel.UFEventObject), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("EnableTag", typeof(string), typeof(UFEventModel.UFEventObject), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ValueTag", typeof(string), typeof(UFEventModel.UFEventObject), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(UFEventEditor.PropertyDataTemplate.TimePropertyEditor));
                dt.DataType = typeof(DateTime);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Time", typeof(DateTime), typeof(UFEventModel.UFEventObject), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DatePropertyEditor));
                dt.DataType = typeof(DateTime);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Date", typeof(DateTime), typeof(UFEventModel.UFEventObject), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Commands", typeof(bool), typeof(UFEventModel.UFEventObject), typeof(EventEditorDocument), dt);


                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ExpressionPropertyEditor));
                factory.SetValue(ExpressionPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                propertyControl.AddPropertyEditor("Expression", typeof(String), typeof(UFEventModel.UFEventObject), dt);
                propertyControl.AddPropertyEditor("EnableExpression", typeof(String), typeof(UFEventModel.UFEventObject), dt);
                propertyControl.AddPropertyEditor("ValueExpression", typeof(String), typeof(UFEventModel.UFEventObject), dt);

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
            if (!(e.TargetItem is EventEditorControl))
                return;

            var view = e.TargetItem as EventEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(EventEditorControl view, bool bSave = true)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
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

        private bool CloseView(EventEditorControl view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (view != null && !CanClose(view, bSave))
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

            var array = new EventEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as EventEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new EventEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as EventEditorControl;
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

            if (e.OldValue != null && e.OldValue is EventEditorControl && viewOld != null)
            {
                var view = e.OldValue as EventEditorControl;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is EventEditorControl && viewNew != null)
            {
                var view = e.NewValue as EventEditorControl;
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

        internal EventEditorControl GetViewFromUri(Uri uri)
        {
            EventEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as EventEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, EventEditorDocument> keyvaluepair in mapActiveDocuments)
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
#endif

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

        ICommandExplorer commandExplorer;
        public ICommandExplorer CommandExplorer
        {
            get
            {
                if (commandExplorer == null)
                    commandExplorer = GetService(typeof(ICommandExplorer)) as ICommandExplorer;
                return commandExplorer;
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
                    EventEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as EventEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = EventEditorDocument.FromFile(uri.GetPathString(), this, parent);

                    if (doc != null && doc.ActiveView == null)
                    { 
                        EventEditorControl eventEditor = new EventEditorControl(this, doc);
                        doc.Parent = parent;
                        doc.ActiveView = eventEditor;
                        if (doc.NeedsSave)
                            doc.SaveToFile();

                        workspace.SetDesiredHeightAndWidthInDockedMode(eventEditor, eventEditor.Height, eventEditor.Width);
                        eventEditor.ClearValue(FrameworkElement.WidthProperty);
                        eventEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("EVMEditorSmall");

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        workspace.AddDockingChildren(eventEditor,
                            String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(eventEditor, new ImageBrush(bm));
                        workspace.FlashDockedElement(eventEditor);

                        eventEditor.Document.PropertyChanged += Document_PropertyChanged;
                    }
                }
            }
        }
#endif

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            EventEditorDocument doc = null;
            if (!mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = EventEditorDocument.FromFile(uri.GetPathString(), this, parent, false);
                if (doc == null)
                    return;
                doc.Parent = parent;
                mapRunningDocuments.Add(uri.GetPathString(), doc);
            }
            else if (doc.IsEmpty)
                return;

            if (!mapRunningEngine.ContainsKey(uri.GetPathString()))
            {
                var eventEngine = new EventsThread();
                eventEngine.Startup(doc);
                mapRunningEngine.Add(uri.GetPathString(), eventEngine);
            }
        }

        public void PreTerminate(Uri uri, IDocument parent)
        {
            EventsThread eventEngine = null;
            if (mapRunningEngine.TryGetValue(uri.GetPathString(), out eventEngine))
            {
                mapRunningEngine.Remove(uri.GetPathString());
                eventEngine.Dispose();
            }
        }

        public void Terminate(Uri uri, IDocument parent)
        {
            EventEditorDocument doc = null;
            if (mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                mapRunningDocuments.Remove(uri.GetPathString());
                doc.Dispose();
            }
        }

#if !NET_STANDARD        
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent
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
                if (!CloseView(document.ActiveView as EventEditorControl))
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
                        EventEditorDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as EventEditorControl, false);
                    }

                    EventEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    EventEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as EventEditorControl, false);

                    EventEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
                }
            }
        }

        public void Delete(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    EventEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as EventEditorControl, false);

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    EventEditorDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = EventEditorDocument.FromFile(uri.GetPathString(), this, parent);
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
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;

                using (new WaitCursor())
                {
                    bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                    bool bGetAll = bShiftDown || !bCap;

                    // if (list == null)
                    //  list = new ObservableCollection<IDocumentManager>();
                    // list.Clear();

                    int i = 0;
                    foreach (var name in doc.GetFolderCollection())
                    {
                        list.Add(new TreeDocumentManagers.EventListDocumentManager(this,
                                                    doc, name, this));
                        if (!bGetAll && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.EventListDocumentManager(this, doc, this));
                            break;
                        }
                    }
                    i = 0;
                    foreach (var name in doc.GetEventsCollection())
                    {
                        list.Add(new TreeDocumentManagers.EventListDocumentManager(this,
                                                    doc, name, this));
                        if (!bGetAll && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.EventListDocumentManager(this, doc, this));
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
                return Properties.Settings.Default.TypeLabel;
            }
        }

#if !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("EVMEditorSmall");
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
                return GetBitmapImage("EVMEditor");
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
#endif
        public Type DocumentType
        {
            get
            {
                return typeof(EventEditorDocument);
            }
        }

#if !NET_STANDARD
        public object BrowsableContent
        {
            get { return null; }
        }
#endif
        #endregion IDocumentManager Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            foreach (IDisposable document in mapRunningDocuments.Values)
                document.Dispose();
            mapRunningDocuments.Clear();

            foreach (IDisposable engine in mapRunningEngine.Values)
                engine.Dispose();
            mapRunningEngine.Clear();

#if !NET_STANDARD
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
#endif
        }

        #endregion IDisposable Members

        #region ICrossReference
#if !NET_STANDARD
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            if (!getScreens && !getTags)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string docType = DocManagerType.EventEditor.ToString();
            using (EventEditorDocument doc = CreateDocument(model.Parent))
            {
                if (doc != null)
                {
                    var appName = UfuaEditorService.GetAplicationName(doc);
                    List<UFEventModel.UFEventObject> eventlist = doc.GetCRFlatTagCollection();
                    //Parallel.ForEach(eventlist, (item, loopstate) =>
                    for (int i = 0; i < eventlist.Count(); i++)
                    {
                        var item = eventlist[i];
                        if (model.QuitEvent.IsCancellationRequested)
                            return result;
                            //loopstate.Break();

                        if (getTags)
                        {
                            var listCommand = item.CommandList as CommandManager.CommandManagerList;
                            if (listCommand != null && listCommand.Count() > 0)
                            {
                                foreach (var command in listCommand)
                                {
                                    if (!string.IsNullOrEmpty(command.Expression))
                                    {
                                        List<string> expressionTags = Utilities.Converters.ExpressionValueConverterHelper.GetListVarInExpression(command.Expression);
                                        //Parallel.ForEach(expressionTags, expression => 
                                        for (int j = 0; j < expressionTags.Count; j++)
                                        {
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
                                                    Description = $"{item.Name} ({Properties.Resources.CommandTagHeader}\\{command.Name} - {Properties.Resources.ItemsExpressionTagHeader} {expressionTags.IndexOf(expression)})",
                                                    Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                                    ContainerDoc = TypeScheme,
                                                    IconType = docType
                                                });
                                        }//);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.TagName))
                            {
                                OPCUAEntityReference tag = item.Tag.FromXml<OPCUAEntityReference>();
                                //lock (result)
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = tag.RelativePath,
                                        Name = tag.Name,
                                        AppName = tag.AppName,
                                        ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                                        EndpointUrl = tag.EndpointUrl,
                                        CReferenceType = CrossReferenceType.Tags,
                                        Description = string.Format("{0} ({1})", item.Name, Properties.Resources.NewEventTagName),
                                        Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                            }

                            if (!string.IsNullOrEmpty(item.EnableTagName))
                            {
                                OPCUAEntityReference tag = item.EnableTag.FromXml<OPCUAEntityReference>();
                                //lock (result)
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = tag.RelativePath,
                                        Name = tag.Name,
                                        AppName = tag.AppName,
                                        ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                                        EndpointUrl = tag.EndpointUrl,
                                        CReferenceType = CrossReferenceType.Tags,
                                        Description = string.Format("{0} ({1})", item.Name, Properties.Resources.NewEventEnableName),
                                        Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                            }

                            if (!string.IsNullOrEmpty(item.ValueTagName))
                            {
                                OPCUAEntityReference tag = item.ValueTag.FromXml<OPCUAEntityReference>();
                                //lock (result)
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = tag.RelativePath,
                                        Name = tag.Name,
                                        AppName = tag.AppName,
                                        ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                                        EndpointUrl = tag.EndpointUrl,
                                        CReferenceType = CrossReferenceType.Tags,
                                        Description = string.Format("{0} ({1})", item.Name, Properties.Resources.NewEventValueName),
                                        Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                            }
                        }

                        if (model.QuitEvent.IsCancellationRequested)
                            return result;
                            //loopstate.Break();

                        var cRMapsHeler = new CRMapsHelper() { CrossReferenceTypes = model.CRManagement.CrossReferenceTypeList };
                        item.GetAllSourceEntityReferencesDetails(cRMapsHeler);
                        if (getTags && cRMapsHeler.Tags != null)
                        {
                            foreach (var tag in cRMapsHeler.Tags)
                            {
                               if (model.QuitEvent.IsCancellationRequested)
                                    return result;

                                IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                                if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value.RelativePath))
                                {
                                    var details = refdetails.First().Value;
                                    //lock (result)
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = details.RelativePath,
                                            Name = details.Name,
                                            AppName = details.AppName,
                                            ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                            EndpointUrl = details.EndpointUrl,
                                            CReferenceType = CrossReferenceType.Tags,
                                            Description = string.Format("{0} ({1})", item.Name, refdetails.First().Key),
                                            Settings = string.Format("{0}|{1}", item.Name, doc.rootBase),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                }
                            }
                        }

                        if (getScreens && cRMapsHeler.ScreenLinks != null)
                        {
                            foreach (var tag in cRMapsHeler.ScreenLinks)
                            {
                               if (model.QuitEvent.IsCancellationRequested)
                                    return result;

                                IDictionary<String, String> refdetails = tag as IDictionary<String, String>;
                                if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value))
                                {
                                    var name = System.IO.Path.GetFileNameWithoutExtension(refdetails.First().Value);
                                    //lock (result)
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = refdetails.First().Value,
                                            Name = name,
                                            AppName = p.Title,
                                            CReferenceType = CrossReferenceType.Resources,
                                            Description = string.Format("{0} ({1})", item.Name, refdetails.First().Key),
                                            Settings = string.Format("{0}|{1}", item.Name, doc.rootBase),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                }
                            }
                        }
                    }//);
                }
            }

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
            }
        }
#endif
        #endregion
#if !NET_STANDARD
        internal EventEditorDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            EventEditorDocument doc = null;
            doc = EventEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
        internal EventEditorDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, EventEditorDocument> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                EventEditorDocument doc = null;
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
                    doc = EventEditorDocument.FromFile(uri.GetPathString(), this, parent);
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
        void DisposeDocument(EventEditorDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            if (doc is IDisposable)
                (doc as IDisposable).Dispose();
        }
        void RemoveActiveDocumentUri(EventEditorDocument doc)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                mapActiveDocumentUris.Remove(doc);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
            }
        }

        public EventEditorControl GetActiveView(Uri uri)
        {
            EventEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as EventEditorControl;
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

        public event EventHandler<EventEventArgs> FireUIEvent;
        virtual public void OnFireUIEvent(Object sender, EventEventArgs args)
        {
            var t = FireUIEvent;
            if (t != null)
                t(sender, args);
        }

    }
}