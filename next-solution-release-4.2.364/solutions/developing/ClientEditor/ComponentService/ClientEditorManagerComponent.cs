using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using HelpProvider.ComponentService;
using System.Windows.Threading;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using StringManager.ComponentService;
using DevExpress.Xpo;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;
using log4net;
using ClientEditor.Document;
using UFProjectManager.ComponentService;
using AppNameSettingService;

namespace ClientEditor.ComponentService
{
    public class ClientEditorManagerComponent : ComponentBase<IClientEditorManager>, IClientEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ClientDocument> mapActiveDocuments = new Dictionary<String, ClientDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ClientDocument, String> mapActiveDocumentUris = new Dictionary<ClientDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
#if !NET_STANDARD
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif
        public static ClientEditorManagerComponent clientereditorManagerComponent { get; protected set; }

#if !NET_STANDARD
        MenuControl menuControl;
        bool isGettingChilds;
#endif
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (clientereditorManagerComponent == null)
                clientereditorManagerComponent = this;

            GetComponentInterfaces();
        }

        #endregion IUFInterfaceBase Members
#if !NET_STANDARD
        public Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(IDocument parent, String flat)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            var active = Workspace.ActiveWindow;

            Edit(uri, p);

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            if (doc.ActiveView != null)
                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, doc.ActiveView);

            doc.VariableCreated += Doc_VariableCreated;
            doc.CreatingVariable += Doc_CreatingVariable;
            try
            {
                return doc.CheckAndUpdateVariableListSettingsFlat(flat);
            }
            finally
            {
                doc.VariableCreated -= Doc_VariableCreated;
                doc.CreatingVariable -= Doc_CreatingVariable;

                Workspace.ActiveWindow = active;
            }
        }


        private void Doc_CreatingVariable(object sender, VariableEventArgs e)
        {
            OnCreatingVariable(sender, e);
        }

        private void Doc_VariableCreated(object sender, VariableEventArgs e)
        {
            OnVariableCreated(sender, e);
        }

        virtual public void OnCreatingVariable(Object sender, VariableEventArgs args)
        {
            var t = CreatingVariable;
            if (t != null)
                t(sender, args);
        }

        virtual public void OnVariableCreated(Object sender, VariableEventArgs args)
        {
            var t = VariableCreated;
            if (t != null)
                t(sender, args);
        }
#endif

        Dictionary<String, IAppNameSettings> IClientEditorManager.GetAppNameSettings(IDocument document)
        {
            var doc = GetOrCreateDocument(document);
            if (doc == null)
                return null;
            return doc.MapAppNameSettings.ToDictionary(s => s.Key, s => (IAppNameSettings)s.Value);
        }

#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(ClientEditor.Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }
#endif

        private void GetComponentInterfaces()
        {
#if !NET_STANDARD
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = ClientEditor.Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
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
#endif
        }

#if !NET_STANDARD
        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is ClientEditorControl))
                return;

            var view = e.TargetItem as ClientEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(ClientEditorControl view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (bSave && (view.Document.NeedsSave || (dsInterface != null && dsInterface.NeedsSave(view.Document))))
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
                        if (view.Document.NeedsSave && !view.Document.SaveToFile())
                            return false;
                        if (dsInterface != null && dsInterface.NeedsSave(view.Document) && !dsInterface.Save(view.Document))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseView(ClientEditorControl view, bool bSave = true)
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
                (view.Document as IDisposable).DisposeInApplicationIdle();
            }

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new ClientDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as ClientEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new ClientDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as ClientEditorControl;
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

            if (e.OldValue != null && e.OldValue is ClientEditorControl && viewOld != null)
            {
                var view = e.OldValue as ClientEditorControl;

                view.OnDeactivate();
                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is ClientEditorControl && viewNew != null)
            {
                var view = e.NewValue as ClientEditorControl;
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
                if (Doc.ActiveView == null)
                    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        internal ClientEditorControl GetViewFromUri(Uri uri)
        {
            ClientDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as ClientEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, ClientDocument> keyvaluepair in mapActiveDocuments)
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

        IUFProjectManager uFProjectManager;
        public IUFProjectManager UFProjectManager
        {
            get
            {
                if (uFProjectManager == null)
                    uFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                return uFProjectManager;
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
                    ClientDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as ClientEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = ClientDocument.FromFile(uri.GetPathString(), this, parent);

                    if (doc != null && doc.ActiveView == null)
                    {
                        ClientEditorControl clientEditor = new ClientEditorControl(this, doc);
                        doc.Parent = parent;
                        doc.ActiveView = clientEditor;
                        if (doc.NeedsSave)
                            doc.SaveToFile();

                        workspace.SetDesiredHeightAndWidthInDockedMode(clientEditor, clientEditor.Height, clientEditor.Width);
                        clientEditor.ClearValue(FrameworkElement.WidthProperty);
                        clientEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("CEEditorSmall");

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        workspace.AddDockingChildren(clientEditor,
                            String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(clientEditor, new ImageBrush(bm));

                        workspace.FlashDockedElement(clientEditor);

                        clientEditor.Document.PropertyChanged += Document_PropertyChanged;
                    }
                }
            }
        }
#endif

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            
        }

        public void PreTerminate(Uri uri, IDocument parent)
        {
 
        }

        public void Terminate(Uri uri, IDocument parent)
        {
           
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

            var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
            if (dsInterface != null && dsInterface.NeedsSave(parent))
                dsInterface.Save(parent);
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var map = (from c in mapActiveDocuments// .AsParallel()
                        where c.Value.Parent == parent && c.Value.ActiveView != null
                        select c).ToList();

            foreach (var pair in map)
            {
                var document = pair.Value;
                if (!CloseView(document.ActiveView as ClientEditorControl))
                    return false;
            }

            if (bParentClosing)
                foreach (var pair in map)
                {
                    var document = pair.Value;
                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                    if (document.NeedsSave || dsInterface != null && dsInterface.NeedsSave(document))
                    {
                        bool bSave = true;
                        if (UIInterface != null)
                        {
                            var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                String.Format("{0} ({1})", TypeTitle, document.Parent.Title)), CustomDialogIcons.Question);
                            if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                                return false;
                            bSave = res == CustomDialogResults.Yes;
                        }

                        if (bSave)
                        {
                            if (document.NeedsSave && !document.SaveToFile())
                                return false;
                            if (dsInterface != null && dsInterface.NeedsSave(document) && !dsInterface.Save(document))
                                return false;
                        }
                    }

                    document.SaveToFile();
                    DisposeDocumentInApplicationIdle(document);

                    mapActiveDocuments.Remove(pair.Key);
                    pair.Value.Dispose();

                    if (mapActiveDocumentUris.ContainsKey(document))
                        mapActiveDocumentUris.Remove(document);
                    if (mapActiveDocumentTitles.ContainsKey(pair.Key))
                        mapActiveDocumentTitles.Remove(pair.Key);
                }

            return true;
        }
        
        void DisposeDocumentInApplicationIdle(ClientDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            doc.DisposeInApplicationIdle();
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

                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (dsInterface != null && dsInterface.NeedsSave(document))
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
                        ClientDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as ClientEditorControl, false);
                    }

                    ClientDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ClientDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as ClientEditorControl, false);

                    ClientDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
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
                    ClientDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as ClientEditorControl, false);

                    ClientDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = ClientDocument.FromFile(uri.GetPathString(), this, parent);
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
            return null;
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

#if !NET_STANDARD
        internal void OnChangedDocument(Object sender, ChangedType type, System.Collections.ICollection changedObjects)
        {
            ClientDocument doc = sender as ClientDocument;
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                ObservableCollection<IDocumentManager> list = new ObservableCollection<IDocumentManager>();
                if (mapChildDocumentManagers.ContainsKey(uri))
                {
                    var founds = (from c in mapChildDocumentManagers[uri] where c is TreeDocumentManagers.TreeChangedDocument select c as TreeDocumentManagers.TreeChangedDocument).ToList();
                    foreach (var found in founds)
                        found.OnChangedDocument(sender, type, changedObjects);
                }
            }
        }
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }
        
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            return GetChildDocumentManagers(doc, uri.GetPathString());
        }
        readonly Dictionary<String, ObservableCollection<IDocumentManager>> mapChildDocumentManagers = new Dictionary<String, ObservableCollection<IDocumentManager>>();

        public event EventHandler<VariableEventArgs> CreatingVariable;
        public event EventHandler<VariableEventArgs> VariableCreated;

        ObservableCollection<IDocumentManager> GetChildDocumentManagers(ClientDocument doc, String uri, bool bCreate = true)
        {
            if (!bCreate && !mapChildDocumentManagers.ContainsKey(uri))
                return null;

            ObservableCollection<IDocumentManager> list = new ObservableCollection<IDocumentManager>();
            if (mapChildDocumentManagers.ContainsKey(uri))
            {
                list = mapChildDocumentManagers[uri];
                //while (list.Count > 0)
                //    list.RemoveAt(0);
                //doc = null;
            }
            else
            {
                mapChildDocumentManagers.Add(uri, list);
                list.Add(new TreeDocumentManagers.TreeGeneralSettingsDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(this, doc));
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
                return GetBitmapImage("CEEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("CEEditor");
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

        public string FileName
        {
            get { return Properties.Settings.Default.DefaultProjectName; }
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
            return true;
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
                return typeof(ClientDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

#if !NET_STANDARD
            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
            }
#endif
        }

        #endregion IDisposable Members

        #region ICrossReference
#if !NET_STANDARD
        internal ClientDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            ClientDocument doc = null;
            doc = ClientDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
#endif
        internal ClientDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, ClientDocument> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                ClientDocument doc = null;
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
                    doc = ClientDocument.FromFile(uri.GetPathString(), this, parent);
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

        void DisposeDocument(ClientDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            if (doc is IDisposable)
                (doc as IDisposable).Dispose();
        }
        void RemoveActiveDocumentUri(ClientDocument doc)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                mapActiveDocumentUris.Remove(doc);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
#if !NET_STANDARD
                mapChildDocumentManagers.Remove(uri);
#endif
            }
            else
            {
                var listuri = (from c in mapActiveDocuments where c.Value == doc select c.Key).ToList();
                listuri.ForEach(u =>
                {
#if !NET_STANDARD
                    mapChildDocumentManagers.Remove(u);
#endif
                    mapActiveDocuments.Remove(u);
                    mapActiveDocumentTitles.Remove(u);
                });
            }
        }

#if !NET_STANDARD
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            return result;
        }

        public void EditCRObject(IDocument parent, string settings)
        {
        }
        public ClientEditorControl GetActiveView(Uri uri)
        {
            ClientDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as ClientEditorControl;
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
        }

        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);
            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            var list = new List<System.Windows.Input.ICommand>();
            list.Add(UIGeneralCommands.AddNewTag);
            return list;
        }
#endif
    }
}