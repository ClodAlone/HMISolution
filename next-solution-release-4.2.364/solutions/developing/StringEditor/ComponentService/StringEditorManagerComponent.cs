using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using UFInterfaces.AuthenticationCredentialsProvider;
using StringManager.Controls;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using WPFUtilities.PropertyDataTemplate;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using StringManager.Document;
using System.Globalization;
using DevExpress.Xpo.DB;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Text;
using System.Windows.Input;
using System.Runtime.Serialization;
using log4net;
using DocumentManager.ComponentService.Helpers;
using UFInterfaces.Editors;

namespace StringManager.ComponentService
{
    public class StringEditorManagerComponent : ComponentBase<IStringEditorManager>, IStringEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, StringEditorDocument> mapActiveDocuments = new Dictionary<String, StringEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<StringEditorDocument, String> mapActiveDocumentUris = new Dictionary<StringEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        public static StringEditorManagerComponent stringEditorManagerComponent { get; protected set; }

#if !NET_STANDARD
		internal MenuControl menuControl;
#else
		static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);		
#endif

#endregion Declaration

#region Public Events
        public event EventHandler CultureChanged;
        public event EventHandler LocalesChanged;
#endregion

#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (stringEditorManagerComponent == null)
                stringEditorManagerComponent = this;

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

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Text", typeof(String), typeof(TextBlock), dt);
                PropertyControl.AddPropertyEditor("Text", typeof(String), typeof(TextBox), dt);

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
            if (!(e.TargetItem is StringEditorControl))
                return;

            var view = e.TargetItem as StringEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(StringEditorControl view, bool bSave = true)
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

        private bool CloseView(StringEditorControl view, bool bSave = true)
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

            var array = new StringEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as StringEditorControl;
                if (view != null)
                    CloseView(view, false);
            }

        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new StringEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as StringEditorControl;
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

            if (e.OldValue != null && e.OldValue is StringEditorControl && viewOld != null)
            {
                var view = e.OldValue as StringEditorControl;
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is StringEditorControl && viewNew != null)
            {
                var view = e.NewValue as StringEditorControl;
                workspace.ContextDocument = view.Document;
                view.OnActivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
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

        internal StringEditorControl GetViewFromUri(Uri uri)
        {
            StringEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as StringEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, StringEditorDocument> keyvaluepair in mapActiveDocuments)
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
#endif
#endregion Properties
#if !NET_STANDARD
        private StringManager.StringEditorControl CreateDocView(StringEditorDocument doc, String title, IDocument parent)
        {
            var editor = new StringEditorControl(doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("STMEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }

        private StringEditorDocument CreateDoc(Uri uri, IDocument parent)
        {
            var doc = StringEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(uri.GetPathString(), doc);
            mapActiveDocumentUris.Add(doc, uri.GetPathString());
            return doc;
        }
#endif
        private StringEditorDocument GetOrCreateDocument(IDocument parent)
        {
            lock (lockObject)
            {
                var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

                StringEditorDocument doc = null;
                if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    doc = StringEditorDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = DocumentHelper.GetRootParent(parent, traverse: false);
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    if (!mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
                return doc;
            }
        }


#region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    var doc = GetOrCreateDocument(parent);
                    if (doc == null)
                        return;

                    if (doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as StringEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = GetOrCreateDocument(parent);

                    if (doc != null && doc.ActiveView == null)
                        CreateDocView(doc, GetDocumentTitle(uri), parent);

                    if (menuControl != null)
                        menuControl.DataContext = doc;
                }
            }
        }
#endif

        public void LoadRuntimeStrings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            try
            {
                doc.LoadRuntimeStrings();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            var doc = GetOrCreateDocument(parent);

            String language = null;
            if (uri == null)
            {
#if !NET_STANDARD
                var control = new CultureSelection();
                control.listbox.ItemsSource = doc.GetListLocalCultures();

                var Dialog = new GeneralDialogContent(control);

                var active = GetActiveCulture(parent);
                var mapActive = GetListStringForCulture(parent, active);
                if (mapActive != null && mapActive.ContainsKey(Properties.Resources.SelectCultureTitle))
                    Dialog.Title = mapActive[Properties.Resources.SelectCultureTitle];
                else
                    Dialog.Title = Properties.Resources.SelectCultureTitle;

                Dialog.HelpLink = "SelectCulture";

                if (parent.ActiveView != null)
                    Dialog.Owner = parent.ActiveView.FindParent<Window>();

                if (Dialog.ShowDialog() != true || control.listbox.SelectedItem == null)
                    return;
                language = control.listbox.SelectedItem as String;
#else
                return;
#endif
            }
            else
                language = uri.OriginalString;

            doc.LoadRuntimeStrings();

            var p = DocumentHelper.GetRootParent(parent, traverse: true);
            language = doc.MatchLanguage(language);
            if (String.IsNullOrEmpty(language))
            {
                language = LoadCurrentLanguage(p.Title);
                if (String.IsNullOrEmpty(language))
                {
                    var controller = p as IScreenController;
                    if (controller == null)
                        return;
                    else
                    {
                        language = controller.GetProjectCulture();
                        if (String.IsNullOrEmpty(language))
                            return;
                    }
                }
            }

            

            lock (mapActiveLanguages)
            {
                if (mapActiveLanguages.ContainsKey(p))
                    mapActiveLanguages.Remove(p);
                mapActiveLanguages.Add(p, language);
                SaveCurrentLanguage(p.Title, language);
            }
#if !NET_STANDARD
            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentCulture, language);
#endif
            OnCultureChanged(parent);
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

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
                document.NeedsSave = false;
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseView(document.ActiveView as StringEditorControl))
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
                if (mapStringEditorControls.ContainsKey(pair.Value))
                    mapStringEditorControls.Remove(pair.Value);
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
                        StringEditorDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as StringEditorControl, false);
                    }

                    StringEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    StringEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as StringEditorControl, false);

                    StringEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
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
                    StringEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as StringEditorControl, false);

                    StringEditorDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = StringEditorDocument.FromFile(uri.GetPathString(), this, parent);
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

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            var listLocale = doc.GetListLocalCultures().ToList();

            var list = new ObservableCollection<IDocumentManager>();
            listLocale.ForEach(locale =>
                {
                    list.Add(new TreeDocumentManagers.TreeDocumentManager(this, doc, locale));
                });
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
                return GetBitmapImage("STMEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("STMEditor");
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
            return doc == null || doc.IsDisposed ? false : doc.GetListLocalCultures().Count > 0;
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
                return typeof(StringEditorDocument);
            }
        }

#endregion IDocumentManager Members

#region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
#if !NET_STANDARD
            mapStringEditorControls.Clear();
            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
#endif
        }

#endregion IDisposable Members

#region IStringEditorManager       
        public IEnumerable<String> GetListAvailableCultures(IDocument parent)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;

                try
                {
                    return doc.GetListLocalCultures();
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
#if !NET_STANDARD
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                    log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                    return null;
                }
            }
        }

        public IEnumerable<String> GetListStringIDs(IDocument parent, bool inExecution = false)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;

                try
                {
                    return doc.GetListStringIDs(inExecution); 
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
#if !NET_STANDARD
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                    log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                    return null;
                }
            }
        }

#if !NET_STANDARD
        readonly Dictionary<StringEditorDocument, StringEditorControl> mapStringEditorControls = new Dictionary<StringEditorDocument, StringEditorControl>();
        public UserControl GetStringEditor(IDocument parent, bool allowMultiSelection = false)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                if (!mapStringEditorControls.ContainsKey(doc))
                    mapStringEditorControls.Add(doc, new StringEditorControl(doc, true, allowMultiSelection));
                return mapStringEditorControls[doc];
            }
        }
#endif

        public IDictionary<string, string> GetListStringForCulture(IDocument parent, String culture)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetMapStrings(culture);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
#if !NET_STANDARD
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                    log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                    return null;
                }
            }
        }

#if !NET_STANDARD
        public bool AddListStringId(IDocument parent, IList<String> list)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            if (doc.ActiveView != null)
                workspace.ActivateDockedElement(doc.ActiveView);
            else
                CreateDocView(doc, TypeTitle, parent);

            if (doc.ActiveView == null)
                return false;

            var control = doc.ActiveView as StringEditorControl;
            control.AddNewLocalIdList(list);

            return true;
        }
        public bool RemoveListStringId(IDocument parent, IList<String> list)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            try
            {
                doc.RemoveStringIdList(list);
                doc.SaveToFile(bForceSave: true);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorRemovingStrings, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return false;
            }
            return true;
        }
#endif

        Dictionary<IDocument, String> mapActiveLanguages = new Dictionary<IDocument, String>();
        public String GetActiveCulture(IDocument parent, bool bAlwaysReturnCulture = true)
        {
            string culture = String.Empty;
            bool bFound = false;
            lock (mapActiveLanguages)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: true);
                if (mapActiveLanguages.ContainsKey(p))
                {
                    culture = mapActiveLanguages[p];
                    bFound = true;
                }
            }
            if (!bFound && bAlwaysReturnCulture)
            {
                var ret = GetListAvailableCultures(parent);
                if (ret != null)
                {
                    var list = ret.ToList();
                    if (list.Contains(CultureInfo.CurrentCulture.Name) || list.Count == 0)
                        culture = CultureInfo.CurrentCulture.Name;
                    else
                        culture = list.First();
                }
            }
#if !NET_STANDARD
            if (String.IsNullOrEmpty(SysVariables.SysVariables.GetSysVariables().GetVariable(SysVariables.SysNames.CurrentCulture).Value))
                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentCulture, culture);
#endif
            return culture;
        }
        public void ClearActiveCulture(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: true);
            lock (mapActiveLanguages)
            {
                if (mapActiveLanguages.ContainsKey(p))
                    mapActiveLanguages.Remove(p);
            }
        }
        public void SetActiveCulture(IDocument parent, string Culture, bool bDesign = false)
        {
            if (string.IsNullOrEmpty(Culture))
                return;

            var doc = GetOrCreateDocument(parent);

            String language = null;
            var p = DocumentHelper.GetRootParent(parent, traverse: true);
            language = doc.MatchLanguage(Culture);
            bool untranslated = language == string.Empty;

            lock (mapActiveLanguages)
            {
                if (mapActiveLanguages.ContainsKey(p))
                {
                    if (mapActiveLanguages[p] == language)
                        return;
                    mapActiveLanguages.Remove(p);
                }
                if (!untranslated)
                    mapActiveLanguages.Add(p, language);
            }
#if !NET_STANDARD
            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentCulture, language);
#endif
            OnCultureChanged(parent);
        }

        virtual public void OnCultureChanged(object sender)
        {
            CultureChanged?.Invoke(sender, EventArgs.Empty);
        }

        virtual public void OnLocalesChanged(object sender)
        {
            LocalesChanged?.Invoke(sender, EventArgs.Empty);
        }


        public String GetConnectionStringFromFile(String file)
        {
            var fileBase = StringEditorDocument.GetBaseFilename(file
#if !NET_STANDARD
                , null
#endif
                );
            //var ServerFile = String.Format("{0}.Server", fileBase);

            return InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));
        }


#endregion

#region Storage

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

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.Locale.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        void SaveCurrentLanguage(String title, String Culture)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(String));
                            serializer.WriteObject(writer, Culture);
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        String LoadCurrentLanguage(String title)
        {
#if !NET_STANDARD
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return null;
#endif
            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(String));
                            return serializer.ReadObject(reader) as String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

#if !NET_STANDARD
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

        public List<CrossReferenceResultModel> GetCRObjects(CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getTexts)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, true);
            string pTitle = p.Title;
            string applicationName = p.Title;
            string docType = DocManagerType.StringManager.ToString();
            var list = GetListStringIDs(model.Parent);
            if (list == null || list.Count() == 0)
                return result;
            foreach(string textId in list)
                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                {
                    RelativePath = $"{textId}",
                    Name = $"{textId}",
                    AppName = p.Title,
                    CReferenceType = CrossReferenceType.Strings,
                    Description = string.Format("{0}", Properties.Resources.CrossReferenceStringDefined),
                    Settings = string.Format("{0}|{1}", docType, p.rootBase),
                    ContainerDoc = TypeScheme,
                    IconType = docType
                });

            return result;
        }

        public void RenameCRObjects(CrossReferenceModel model)
        {
           
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
#endif
        #endregion
    }
}