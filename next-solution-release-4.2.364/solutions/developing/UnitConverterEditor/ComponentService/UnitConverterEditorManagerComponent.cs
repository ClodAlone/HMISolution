using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
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
using UnitConverterManager.Document;
using UFInterfaces.AuthenticationCredentialsProvider;
using PropertyControl.ComponentService;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Text;
using System.Windows.Input;
using System.Runtime.Serialization;
using WPFUtilities.Extensions;
using WPFUtilities.PropertyDataTemplate;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using UnitConverterManager.Controls;
using Utilities.WPF;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using UFInterfaces.Editors;

namespace UnitConverterManager.ComponentService
{
    public class UnitConverterEditorManagerComponent : ComponentBase<IUnitConverterEditorManager>, IUnitConverterEditorManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UnitConverterEditorDocument> mapActiveDocuments = new Dictionary<String, UnitConverterEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UnitConverterEditorDocument, String> mapActiveDocumentUris = new Dictionary<UnitConverterEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        
        public static UnitConverterEditorManagerComponent unitConverterEditorManager { get; protected set; }
        MenuControl menuControl;
        bool isGettingChilds;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (unitConverterEditorManager == null)
                unitConverterEditorManager = this;

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

            if (uFProjectManager == null)
                uFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;

            workspace.Closing += workspace_Closing;
            workspace.Closed += workspace_Closed;
            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            workspace.PromptDocumentEditorObject += workspace_PromptDocumentEditorObject;

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.UConverterPropertyEditor));
                factory.SetValue(UConverterPropertyEditor.WorkspaceProperty, workspace);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("InputExpression", typeof(String), typeof(UnitConverterModel.UFConverterItem), dt);
                PropertyControl.AddPropertyEditor("OutputExpression", typeof(String), typeof(UnitConverterModel.UFConverterItem), dt);

                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }

        }

        private void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (!(sender is IXPSimpleObject))
                return;

            IXPSimpleObject source = sender as IXPSimpleObject;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null) // || Doc.UowContext == null || !XpoHelper.IsSessionObject(source, Doc.UowContext))
                    continue;


                //var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface(TempVariablesManager.TempVariables.DataSynkName);

                //if (Doc.UowContext == null && dsInterface == null || (!XpoHelper.IsSessionObject(source, Doc.UowContext) && !dsInterface.CheckSource(Doc, source)))
                //    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        void PropertyControl_AcceptChanges(object sender, EventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var list = (from c in mapActiveDocuments.Values// .AsParallel()
                            where c.NeedsSave == true && c.ActiveView == null
                            select c).ToList();
                list.ForEach(doc => doc.SaveToFile(silent: true));
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is UnitConverterEditorControl))
                return;

            var view = e.TargetItem as UnitConverterEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(UnitConverterEditorControl view, bool bSave = true)
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

        private bool CloseView(UnitConverterEditorControl view, bool bSave = true)
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

            var array = new UnitConverterEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UnitConverterEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new UnitConverterEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UnitConverterEditorControl;
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

            if (e.OldValue != null && e.OldValue is UnitConverterEditorControl && viewOld != null)
            {
                var view = e.OldValue as UnitConverterEditorControl;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UnitConverterEditorControl && viewNew != null)
            {
                var view = e.NewValue as UnitConverterEditorControl;
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

                if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                    mapActiveDocumentTitles[uri.GetPathString()] = ret;
                else
                    mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        internal UnitConverterEditorControl GetViewFromUri(Uri uri)
        {
            UnitConverterEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UnitConverterEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UnitConverterEditorDocument> keyvaluepair in mapActiveDocuments)
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
                return uFProjectManager;
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

        #endregion Properties

        private UnitConverterManager.UnitConverterEditorControl CreateDocView(UnitConverterEditorDocument doc, String title, IDocument parent)
        {
            var editor = new UnitConverterEditorControl(doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("UCMEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }

        private UnitConverterEditorDocument CreateDoc(Uri uri, IDocument parent)
        {
            var doc = UnitConverterEditorDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            return doc;
        }

        private UnitConverterEditorDocument GetOrCreateDocument(IDocument parent)
        {
            lock (lockObject)
            {
                var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

                UnitConverterEditorDocument doc = null;
                if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    doc = UnitConverterEditorDocument.FromFile(uri.GetPathString(), this, parent);
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
        }


        #region IDocumentManager Members

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
                            if (!CloseView(doc.ActiveView as UnitConverterEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = GetOrCreateDocument(parent);

                    if (doc != null && doc.ActiveView == null)
                        CreateDocView(doc, GetDocumentTitle(uri), parent);
                }
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            var doc = GetOrCreateDocument(parent);

            String converter = null;
            var p = DocumentHelper.GetRootParent(parent, traverse: true);
            if (mode != ExecutionMode.Stop)
            {
                if (uri == null)
                {
                    var control = new ConverterSelection();
                    control.listbox.ItemsSource = doc.GetListLocalConverter();

                    var Dialog = new GeneralDialogContent(control);
                    Dialog.Title = Properties.Resources.SelectConverterTitle;
                    Dialog.HelpLink = "SelectConverter";

                    if (parent.ActiveView != null)
                        Dialog.Owner = parent.ActiveView.FindParent<Window>();

                    if (Dialog.ShowDialog() != true || control.listbox.SelectedItem == null)
                        return;
                    converter = control.listbox.SelectedItem as String;
                }
                else
                    converter = uri.OriginalString;

                converter = doc.MatchConverter(converter);
                if (String.IsNullOrEmpty(converter))
                {
                    converter = LoadCurrentConverter(p.Title);
                    if (String.IsNullOrEmpty(converter))
                    {
                        var controller = p as IScreenController;
                        if (controller == null)
                            return;
                        else
                        {
                            converter = controller.GetProjectConverter();
                            if (String.IsNullOrEmpty(converter))
                                return;
                        }
                    }
                }

                lock (mapActiveConverters)
                {
                    if (mapActiveConverters.ContainsKey(p))
                        mapActiveConverters.Remove(p);
                    mapActiveConverters.Add(p, converter);
                    SaveCurrentConverter(p.Title, converter);
                }
            }
            else
            {
                lock (mapActiveConverters)
                {
                    if (mapActiveConverters.ContainsKey(p))
                        mapActiveConverters.Remove(p);
                    SaveCurrentConverter(p.Title, converter);
                }
            }

            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentConverter, converter);
            OnConverterChanged(parent);
        }

        public void Terminate(Uri uri, IDocument parent)
        {

        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

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
                if (!CloseView(document.ActiveView as UnitConverterEditorControl))
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
                        UnitConverterEditorDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as UnitConverterEditorControl, false);
                    }

                    UnitConverterEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UnitConverterEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UnitConverterEditorControl, false);

                    UnitConverterEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
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
                    UnitConverterEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UnitConverterEditorControl, false);

                    UnitConverterEditorDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = UnitConverterEditorDocument.FromFile(uri.GetPathString(), this, parent);
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

        // ObservableCollection<IDocumentManager> list;

        static readonly int maxItems = Properties.Settings.Default.MaxItemsInTree;

        public event EventHandler CultureChanged;

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
                UnitConverterEditorDocument doc = null;
                if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    doc = CreateDoc(uri, parent);
                    if (doc == null)
                        return null;

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
                    foreach (var name in doc.GetListLocalConverter())
                    {
                        list.Add(new TreeDocumentManagers.TreeDocumentManager(this, doc, name));
                        if (!bGetAll && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.TreeDocumentManager(this,
                                                        doc, Properties.Resources.MaxItemCountVisibleReached, true));
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
                return GetBitmapImage("UCMEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("UCMEditor");
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
            if (document == null)
                return false;
            var doc = GetOrCreateDocument(document);
            return doc == null || doc.IsDisposed ? false : doc.GetListLocalConverter().Count > 0;
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
                return typeof(UnitConverterEditorDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
            mapActiveDocumentTitles.Clear();

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

        #region IUnitConverterEditorManager       
        public IEnumerable<string> GetListAvailableConverters(IDocument parent)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;

                try
                {
                    return doc.GetListLocalConverter();
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }

        public IDictionary<string, string> GetListIDsForCulture(IDocument parent, string converter)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetMapStrings(converter);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }

        public IEnumerable<string> GetListUnitConverterIDs(IDocument parent)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetListStringIDs();
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }
        
        public String GetUnitLabel(IDocument parent, string id, string converter)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetUnitLabel(converter, id);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }

        public String GetInputExpression(IDocument parent, string id, string converter)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetInputExpression(converter, id);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }

        public String GetOutputExpression(IDocument parent, string id, string converter)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                try
                {
                    return doc.GetOutputExpression(converter, id);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return null;
                }
            }
        }
        public bool AddListUnitConverterId(IDocument parent, IList<string> list)
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

            var control = doc.ActiveView as UnitConverterEditorControl;
            control.AddNewLocalIdList(list);

            return true;
        }
        Dictionary<IDocument, String> mapActiveConverters = new Dictionary<IDocument, String>();
        public string GetActiveConverter(IDocument parent)
        {
            string activeConverter = null;
            lock (mapActiveConverters)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: true);
                if (mapActiveConverters.ContainsKey(p))
                    activeConverter = mapActiveConverters[p];
            }

            //var list = GetListAvailableConverters(parent).ToList();
            //if (list != null && list.Count > 0)
            //    return list.First();
            if (String.IsNullOrEmpty(SysVariables.SysVariables.GetSysVariables().GetVariable(SysVariables.SysNames.CurrentConverter).Value))
                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentConverter, activeConverter);
            return activeConverter;
        }

        public event EventHandler CurrentConverterChanged;

        virtual public void OnConverterChanged(object sender)
        {
            EventHandler temp = CurrentConverterChanged;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public string GetConnectionStringFromFile(string file)
        {
            var fileBase = UnitConverterEditorDocument.GetBaseFilename(file, null);
            //var ServerFile = String.Format("{0}.Server", fileBase);

            return InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));
        }

        readonly Dictionary<UnitConverterEditorDocument, UnitConverterEditorControl> mapStringEditorControls = new Dictionary<UnitConverterEditorDocument, UnitConverterEditorControl>();
        public UserControl GetUnitConverterEditor(IDocument parent)
        {
            lock (lockObject)
            {
                var doc = GetOrCreateDocument(parent);
                if (doc == null)
                    return null;
                if (!mapStringEditorControls.ContainsKey(doc))
                    mapStringEditorControls.Add(doc, new UnitConverterEditorControl(doc, true));
                return mapStringEditorControls[doc];
            }
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

        void SaveCurrentConverter(String title, String Culture)
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

        String LoadCurrentConverter(String title)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return null;

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
        #region ICrossReference
        public bool NeedSingleThreadedApartment => false;

        public List<CrossReferenceResultModel> GetCRObjects(CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getTexts)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, true);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);
            string docType = DocManagerType.UnitConverters.ToString();
            using (UnitConverterEditorDocument doc = CreateDoc(uri, model.Parent))
            {
                List<string> stringIds = doc.GetListLocalConverter().ToList();
                stringIds.ForEach(textId =>
                {
                    List<UnitConverterModel.UFConverterItem> list = doc.GetListItems(textId).ToList();
                    foreach (UnitConverterModel.UFConverterItem item in list)
                    {
                        if(!string.IsNullOrEmpty(item.Description))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = $"{item.Description}",
                                Name = $"{item.Description}",
                                AppName = p.Title,
                                CReferenceType = CrossReferenceType.Strings,
                                Description = string.Format("{0}\\{1} ({2})", textId, item.Name, Properties.Resources.CRDescription),
                                Settings = string.Format("{0}|{1}", docType, p.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });

                        if (!string.IsNullOrEmpty(item.InputUnit))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = $"{item.InputUnit}",
                                Name = $"{item.InputUnit}",
                                AppName = p.Title,
                                CReferenceType = CrossReferenceType.Strings,
                                Description = string.Format("{0}\\{1} ({2})", textId, item.Name, Properties.Resources.CREngineerUnit),
                                Settings = string.Format("{0}|{1}", docType, p.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                    }
                });
            }

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
        #endregion
        #endregion
    }
}
