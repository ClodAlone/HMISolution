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
using UFInterfaces.CoreHostComponents;
using Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using UFInterfaces;
using ScreenParametersSettings.Documents;
using log4net;
using PropertyControl.ComponentService;
using UriResolver.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using StringManager.ComponentService;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUAEditor.ComponentService;
using CommandExplorer.ComponentService;
using VFS;
using ScreenParameterSettings;
using ScreenParametersEditor.PropertyDataTemplate;
using System.Windows.Threading;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;

namespace ScreenParametersEditor.ComponentService
{
    public class ScreenParametersEditorComponent : ComponentBase<IScreenParametersEditorManager>, IScreenParametersEditorManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ScreenParametersDocument> mapActiveDocuments = new Dictionary<String, ScreenParametersDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ScreenParametersDocument, String> mapActiveDocumentUris = new Dictionary<ScreenParametersDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);


        public static ScreenParametersEditorComponent screenParametersEditorComponent { get; protected set; }
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        MenuControl menuControl;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (screenParametersEditorComponent == null)
                screenParametersEditorComponent = this;

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
            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            workspace.PromptDocumentEditorObject += workspace_PromptDocumentEditorObject;

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ParameterItemPropertyEditorCombo));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ID", typeof(string), typeof(ParameterItem), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ParameterItemPropertyEditorCombo));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("text", typeof(string), typeof(ParameterItem), dt);

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
            if (!(e.TargetItem is ScreenParametersEditorUI))
                return;

            var view = e.TargetItem as ScreenParametersEditorUI;
            if (!CloseScreenParameters(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(ScreenParametersEditorUI view, bool bSave = true)
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

        private bool CloseScreenParameters(ScreenParametersEditorUI view, bool bSave = true, bool bDispose = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (bSave && view.Document.NeedsSave)
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
                        view.Document.SaveCurrentDocument();
                    }
                }

                if (bDispose)
                {
                    mapActiveDocumentUris.Remove(view.Document);
                    mapActiveDocuments.Remove(uri);
                    mapActiveDocumentTitles.Remove(uri);
                }
            }

            if (bDispose)
            {
                if (workspace.ContextDocument == view.Document)
                    workspace.ContextDocument = null;

                view.Document.PropertyChanged -= Document_PropertyChanged;

                workspace.RemoveDockingChildren(view);
                if (view.Document is IDisposable)
                    (view.Document as IDisposable).Dispose();
            }

            return true;
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new ScreenParametersDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as ScreenParametersEditorUI;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is ScreenParametersEditorUI && viewOld != null)
            {
                var view = e.OldValue as ScreenParametersEditorUI;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is ScreenParametersEditorUI && viewNew != null)
            {
                var view = e.NewValue as ScreenParametersEditorUI;
                workspace.ContextDocument = view.Document;
                //view.OnActivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
            }
        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (sender is ScreenParametersDocument)
            {
                e.documentEditor = (sender as ScreenParametersDocument).ActiveView;
                return;
            }

            if (!(sender is ParameterItem))
                return;
            ParameterItem element = sender as ParameterItem;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null || !Doc.GetParametersList().Contains(element))
                    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        String GetDocumentTitle(Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

        String CreateDocumentTitle(Uri uri, IDocument parent)
        {
            lock (lockObject)
            {
                var relative = parent.MakeRelativeUri(new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute));
                String name = Path.GetFileNameWithoutExtension(relative.GetPathString());
                String folder = Path.GetDirectoryName(relative.GetPathString());
                folder = folder.Replace(String.Format("{0}\\", TypeLabel), "");
                String ret = null;
                if (String.IsNullOrEmpty(folder) || folder == TypeLabel)
                    ret = name;
                else
                    ret = String.Format("{0}\\{1}", folder, name);
                //String sourcefmt = ret;
                //int i = 1;
                //while (mapActiveDocumentTitles.ContainsValue(ret))
                //    ret = String.Format("{0}{1}", sourcefmt, i++);

                mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        internal ScreenParametersEditorUI GetViewFromUri(Uri uri)
        {
            ScreenParametersDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as ScreenParametersEditorUI;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, ScreenParametersDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CreateDefaultDocument(Uri uri, IDocument parent, bool encryptFile = false)
        {
            using (var newProject = new ScreenParametersDocument() { FullPath = uri.GetPathString(), Parent = parent})
            {
                newProject.SaveToFile(forceEncryption: encryptFile);
            }
        }

        #region Properties

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

        #endregion Properties

        private ScreenParametersEditor.ScreenParametersEditorUI CreateDocView(ScreenParametersDocument doc, String title, IDocument parent)
        {
            var editor = new ScreenParametersEditorUI(this, doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("SPEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", title, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScreenParametersDocument CreateDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = ScreenParametersDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }

        #region IDocumentManager Members

        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenParametersDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseScreenParameters(doc.ActiveView as ScreenParametersEditorUI))
                                return;
                            doc = null;
                        }
                    }
                    
                    if (doc == null)
                        doc = CreateDoc(uri, parent);

                    if (doc != null && doc.ActiveView == null)
                        CreateDocView(doc, CreateDocumentTitle(uri, parent), parent);
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
                    ScreenParametersDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is ScreenParametersEditorUI)
                            CloseScreenParameters(doc.ActiveView as ScreenParametersEditorUI, false);
                        else
                        {
                            doc.PropertyChanged -= Document_PropertyChanged;
                            if (doc is IDisposable)
                                (doc as IDisposable).Dispose();
                        }
                    }

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    ScreenParametersDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                }
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {


        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;
        }

        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values
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
                if (!CloseScreenParameters(document.ActiveView as ScreenParametersEditorUI))
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
                    ScreenParametersDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseScreenParameters(doc.ActiveView as ScreenParametersEditorUI);
                        else if (doc.NeedsSave)
                        {
                            if (UIInterface != null)
                            {
                                var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                    GetDocumentTitle(uri)), CustomDialogIcons.Question);
                                if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                                    return;
                                if (res == CustomDialogResults.Yes)
                                {
                                    doc.SaveCurrentDocument();
                                }
                            }
                        }
                    }

                    ScreenParametersDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenParametersDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is ScreenParametersEditorUI)
                    {
                        bReopen = true;
                        if (!CloseScreenParameters(doc.ActiveView as ScreenParametersEditorUI))
                            return;
                    }
                    else if (doc != null)
                    {
                        doc.PropertyChanged -= Document_PropertyChanged;
                        if (doc is IDisposable)
                            (doc as IDisposable).Dispose();
                    }

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    var newname = ScreenParametersDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

                    if (bReopen)
                    {
                        Edit(new Uri(newname, UriKind.RelativeOrAbsolute), parent);
                    }
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = ScreenParametersDocument.FromFile(uri.GetPathString(), parent);
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
            ScreenParametersDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScreenParametersDocument.FromFile(uri.GetPathString(), parent);
                if (doc == null)
                    return null;
                doc.Parent = parent;
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }

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
                return GetBitmapImage("SPEditorSmall");
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
                return GetBitmapImage("SPEditor");
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

        public string FileName
        {
            get { return String.Empty; }
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
                return true;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

        public bool IsResourceExpandable(IDocument document = null)
        {
            return false;
        }

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

        public Uri CreateNewDocument(Uri relative, string name, IDocument parent)
        {
            String path;
            Uri url;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                path = String.Format("{0}{1}{2}", relative.OriginalString, name, Properties.Settings.Default.DefaultFileExt);
                if (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)))
                    return null;

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(url, parent);
                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, parent);
                return uri;
            }

            path = String.Format("{0}{1}{2}", relative.OriginalString, name, Properties.Settings.Default.DefaultFileExt);
            if (File.Exists(path))
                return null;
            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, parent);
            return url;

        }

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            String path, newReportName;
            int i = 0;
            Uri url;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newReportName = String.Format("{0}{1}", ScreenParametersDocument.DefaultParameterName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, Properties.Settings.Default.DefaultFileExt);
                } while (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)));

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(url, parent, encryptFile);
                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, parent, encryptFile);
                return uri;
            }

            do
            {
                newReportName = String.Format("{0}{1}", ScreenParametersDocument.DefaultParameterName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, parent, encryptFile);
            return url;
        }

        public Type DocumentType
        {
            get
            {
                return typeof(ScreenParametersDocument);
            }
        }
        #endregion IDocumentManager Members

        #region IMenuEditorManager Members

        readonly Dictionary<MenuBase, ScreenParametersDocument> mapActiveMenus = new Dictionary<MenuBase, ScreenParametersDocument>();
        //public MenuBase GetMenu(IDocument parent, Uri uri, String sessionName, bool bLookForDefault, bool bContextMenu, bool bTest = false)
        //{
        //    MenuBase ret = null;
        //    var Parent = parent.Parent;
        //    if (Parent == null)
        //        Parent = parent;

        //    var uriAbsolute = Parent.MakeAbosoluteUri(uri);
        //    var doc = CreateDoc(uriAbsolute, Parent);
        //    if (doc == null)
        //    {
        //        var FullPath = String.Format("{0}\\{1}", TypeLabel, uri.GetPathString());
        //        var name = System.IO.Path.ChangeExtension(FullPath, Properties.Settings.Default.DefaultFileExt);

        //        uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
        //        doc = CreateDoc(uriAbsolute, Parent);
        //        if (doc == null && bLookForDefault)
        //        {
        //            name = System.IO.Path.ChangeExtension(String.Format("{0}\\Main", TypeLabel), Properties.Settings.Default.DefaultFileExt);
        //            uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
        //            doc = CreateDoc(uriAbsolute, Parent);
        //        }
        //    }

        //    if (doc != null)
        //    {
        //        IDictionary<String, String> mapCulture = null;
        //        if (StringEditor != null)
        //        {
        //            var culture = StringEditor.GetActiveCulture(parent);
        //            mapCulture = StringEditor.GetListStringForCulture(parent, culture);
        //        }

        //        ret = doc.PrepareActive(sessionName, mapCulture, bContextMenu, bTest);
        //        if (ret != null && !bTest)
        //        {
        //            lock(lockObject)
        //                mapActiveMenus.Add(ret, doc);
        //        }
        //    }

        //    return ret;
        //}

        //public void TerminateMenu(MenuBase menu)
        //{
        //    lock (lockObject)
        //    {
        //        if (mapActiveMenus.ContainsKey(menu))
        //        {
        //            mapActiveMenus[menu].TerminateActive();
        //            mapActiveMenus.Remove(menu);
        //        }
        //    }
        //}

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;                
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
        }

        #endregion IDisposable Members       
        #region ICrossReference
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            if (!getTags)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            var path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            string docType = DocManagerType.ParameterEditor.ToString();
            string appname = UfuaEditorService.GetAplicationName(p);
            var endpoint = UfuaEditorService.GetDefaultLocalEndpoint(p);
            List<String> list = model.ResourceList as List<String>;
            Parallel.ForEach(list, (resource, loopstate) =>
            //for (int i = 0; i < list.Count(); i++)
            {
                //var resource = list[i];
                if (model.QuitEvent.IsCancellationRequested)
                    loopstate.Break();
                var uri = new Uri($"{path}\\{resource}", UriKind.RelativeOrAbsolute);
                using (ScreenParametersDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        List<ParameterItem> taglist = doc.GetParametersList() as List<ParameterItem>;
                        string docTitle = doc.Title;
                        //Parallel.ForEach(taglist, (tag, loopstate1) =>
                        for (int j = 0; j < taglist.Count(); j++)
                        {
                            var tag = taglist[j];
                            if (model.QuitEvent.IsCancellationRequested)
                                loopstate.Break();

                            if (!string.IsNullOrEmpty(tag.text))
                            {
                                var text = NamespaceTableConverter.GetSanitizedValue(tag.text);
                                var relativePath = text;
                                var name = relativePath?.Split('/').LastOrDefault();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    lock (result)
                                    {
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = relativePath,
                                            Name = name,
                                            AppName = appname,
                                            EndpointUrl = endpoint,
                                            CReferenceType = CrossReferenceType.Tags,
                                            Description = string.Format("{0} ({1})", docTitle, Properties.Resources.text),
                                            Settings = string.Format("{0}|{1}", DocManagerType.ParameterEditor, uri.GetPathString()),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(tag.ID))
                            {
                                var ID = NamespaceTableConverter.GetSanitizedValue(tag.ID);
                                var relativePath = ID;
                                var name = relativePath?.Split('/').LastOrDefault();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    lock (result)
                                    {
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = relativePath,
                                            Name = name,
                                            AppName = appname,
                                            EndpointUrl = endpoint,
                                            CReferenceType = CrossReferenceType.Tags,
                                            Description = string.Format("{0} ({1})", docTitle, Properties.Resources.ID),
                                            Settings = string.Format("{0}|{1}", DocManagerType.ParameterEditor, uri.GetPathString()),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                    }
                                }
                            }
                        }//);
                    }

                }
            });

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
            }
        }
        #endregion
        
        private ScreenParametersDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            ScreenParametersDocument doc = null;
            doc = ScreenParametersDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        private ScreenParametersDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            ScreenParametersDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScreenParametersDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
        }

        private ScreenParametersEditorUI GetActiveView(Uri uri)
        {
            ScreenParametersDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as ScreenParametersEditorUI;
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
    }
}