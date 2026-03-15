using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using UFInterfaces.AuthenticationCredentialsProvider;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using DevExpress.Xpo;
using XpoHelpers;
using VFS;
using Toolbox.ComponentService;
using DevExpress.Xpf.NavBar;
using UFUAEditor.ComponentService;
using System.Windows.Threading;
#endif
#if !NET_STANDARD
using UIMsgBoxAlertService.ComponentService;
using LogicManager.PropertyDataTemplate;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using LogicManager.Document;
using Northwoods.GoXam.Model;
using LogicCore;
using Northwoods.GoXam;
using DocumentManager.ComponentService.Helpers;

namespace LogicManager.ComponentService
{
    public class DocumentEditorManagerComponent : ComponentBase<ILogicManager>, ILogicManager, IDocumentManager, IDisposable
#if !WINDOWS_UWP && !NET_STANDARD
        , ICrossReference
#endif
    {
#region Declaration

        readonly Object lockObject = new Object();
        
        readonly Dictionary<String, DocumentEditorDocument> mapActiveDocuments = new Dictionary<String, DocumentEditorDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<DocumentEditorDocument, String> mapActiveDocumentUris = new Dictionary<DocumentEditorDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<IDocument, List<DocumentEditorDocument>> mapRunningLogicsPerParent = new Dictionary<IDocument, List<DocumentEditorDocument>>();

        public static DocumentEditorManagerComponent documentEditorManagerComponent { get; protected set; }
#if !WINDOWS_UWP && !NET_STANDARD
        MenuControl menuControl;
#endif

#endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (documentEditorManagerComponent == null)
                documentEditorManagerComponent = this;
            
            GetComponentInterfaces();
        }

#endregion IUFInterfaceBase Members

#if !WINDOWS_UWP && !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }
#endif
        private void GetComponentInterfaces()
        {
#if !WINDOWS_UWP && !NET_STANDARD
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
                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;

                // Configuration Data Templates
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ScriptCodePropertyEditor));
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ScriptCode", typeof(String), typeof(GateData), null, dt, true);
            }
            if (ToolBox != null)
                ToolBox.PromptForControl += ToolBox_PromptForControl;
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD

        private void ToolBox_PromptForControl(object sender, PromptForControlEventArgs e)
        {
            if (e.TypeLabel != TypeLabel)
                return;

            var mapRibbonBars = new Dictionary<String, NavBarGroup>();
            var mapModels = new Dictionary<String, GraphLinksModel<GateData, String, String, WireData>>();
            var mapNodeSourceModels = new Dictionary<String, ObservableCollection<GateData>>();
            var mapDataTemplates = new Dictionary<String, DataTemplateDictionary>();
            var types = GateData.LoadGateDataTypes();
            types.ForEach(type =>
            {
                var gateData = GateData.CreateFrom(type);
                var category = gateData.GetCategory();
                if (!mapRibbonBars.ContainsKey(category))
                {
                    var ribbonBar = new NavBarGroup() { Header = category };
                    mapRibbonBars.Add(category, ribbonBar);

                    mapModels.Add(category, new GraphLinksModel<GateData, String, String, WireData>());
                    mapModels[category].NodeCategoryPath = GateData.NodeCategoryPath;

                    mapDataTemplates.Add(category, new DataTemplateDictionary());
                    mapNodeSourceModels.Add(category, new ObservableCollection<GateData>());
                    mapModels[category].NodesSource = mapNodeSourceModels[category];
                    mapModels[category].LinksSource = new ObservableCollection<WireData>();
                }
                var templates = gateData.GetDataTemplates();
                if (templates != null)
                {
                    foreach (var template in templates)
                    {
                        if (!mapDataTemplates[category].ContainsKey(template.Key))
                            mapDataTemplates[category].Add(template.Key, template.Value);
                    }
                }
                foreach(var nodesource in gateData.GetTypes())
                    mapNodeSourceModels[category].Add(nodesource);
            });
#if !WINDOWS_UWP && !NET_STANDARD
            if (menuControl != null)
            {
                var navbar = new NavBarControl() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                foreach (var key in mapRibbonBars)
                {
                    var category = key.Key;
                    var item = menuControl.TryFindResource("paletteDataTemplate") as DataTemplate;
                    var palette = item.LoadContent() as Palette;
                    palette.NodeTemplateDictionary = mapDataTemplates[category];
                    palette.Model = mapModels[category];
                    // palette.Height = 40 * mapNodeSourceModels[category].Count;
                    var navbarItem = new NavBarItem() { Content = palette };
                    key.Value.Items.Add(navbarItem);
                    navbar.Groups.Add(key.Value);
                }
                e.toolboxControl = navbar;
            }
#endif
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
            if (!(e.TargetItem is LogicEditorControl))
                return;

            var view = e.TargetItem as LogicEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(LogicEditorControl view, bool bSave = true)
        {
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
                        if (!view.SaveCurrentDocument())
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseView(LogicEditorControl view, bool bSave = true, bool bDispose = true)
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
                        view.SaveCurrentDocument();
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
                if (workspace.ContextObject == view.Document)
                    workspace.ContextObject = null;

                view.Document.PropertyChanged -= Document_PropertyChanged;

                workspace.RemoveDockingChildren(view);
                // view.Dispose();
                if (view.Document is IDisposable)
                    (view.Document as IDisposable).Dispose();
            }

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new DocumentEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as LogicEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new DocumentEditorDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as LogicEditorControl;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (sender is DocumentEditorDocument)
            {
                e.documentEditor = (sender as DocumentEditorDocument).ActiveView;
                return;
            }

            if (!(sender is GateData))
                return;
            GateData element = sender as GateData;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                var found = (from c in Doc.Model.NodesSource.OfType<GateData>() where c == element select c).ToList();
                if (Doc.ActiveView == null || found.Count == 0)
                    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is LogicEditorControl && viewOld != null)
            {
                var view = e.OldValue as LogicEditorControl;
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is LogicEditorControl && viewNew != null)
            {
                var view = e.NewValue as LogicEditorControl;
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
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        internal LogicEditorControl GetViewFromUri(Uri uri)
        {
            DocumentEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as LogicEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, DocumentEditorDocument> keyvaluepair in mapActiveDocuments)
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

        internal void CreateDefaultDocument(Uri uri, IDocument parent, bool encryptFile = false)
        {
            using (var newProject = new DocumentEditorDocument
            {
                FullPath = uri.GetPathString(),
                Parent = parent
            })
            {
                newProject.SaveToFile(forceEncryption: encryptFile);
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

        IToolbox toolBox;
        public IToolbox ToolBox
        {
            get
            {
                if (toolBox == null)
                    toolBox = GetService(typeof(IToolbox)) as IToolbox;
                return toolBox;
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
#endif

#if !WINDOWS_UWP && !NET_STANDARD
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
#endif
#endregion Properties

#if !WINDOWS_UWP && !NET_STANDARD
        private LogicManager.LogicEditorControl CreateDocView(DocumentEditorDocument doc, String title, IDocument parent)
        {
            var editor = new LogicEditorControl(doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("LGEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", title, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }
#endif
        private DocumentEditorDocument CreateDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = DocumentEditorDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }

#region IDocumentManager Members

#if !WINDOWS_UWP && !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    DocumentEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as LogicEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = CreateDoc(uri, parent);

                    if (doc != null && doc.ActiveView == null)
                        CreateDocView(doc, GetDocumentTitle(uri), parent);
                }
            }
        }

        public void Delete(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    DocumentEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is LogicEditorControl)
                            CloseView(doc.ActiveView as LogicEditorControl, false);
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

                    DocumentEditorDocument.RemoveFile(uri.GetPathString(), parent);
                }
            }
        }
#endif

        static int counterWindowDebuggers = 0;

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            var docParent = DocumentHelper.GetRootParent(parent, traverse: false);

            uri = docParent.MakeAbosoluteUri(uri);
            docParent = docParent.UpdateParentFromUri(uri);

            DocumentEditorDocument doc = null;
            if (mapRunningLogicsPerParent.ContainsKey(docParent))
            {
                var executers = (from c in mapRunningLogicsPerParent[docParent]
                                 where c.FullPath == uri.GetPathString()
                                 select c).ToList();
                if (executers.Count > 0)
                {
                    doc = executers[0];
                    System.Diagnostics.Debug.WriteLine(String.Format("Executer Found Logic {0}, {1}, mode {2}", uri, docParent.Title, mode));
                }
            }
            else
                mapRunningLogicsPerParent.Add(docParent, new List<DocumentEditorDocument>());

            if (doc == null)
            {
                doc = DocumentEditorDocument.FromFile(uri.GetPathString(), parent);
                if (doc == null)
                    return;
                doc.Parent = docParent;
                doc.Model.HasUndoManager = false;
                mapRunningLogicsPerParent[docParent].Add(doc);
            }

#if !WINDOWS_UWP && !NET_STANDARD
            if (Application.Current == null && mode == ExecutionMode.Shared)
                mode = ExecutionMode.Normal;
#endif
            switch (mode)
            {
#if WINDOWS_UWP || NET_STANDARD
                case ExecutionMode.Shared:
#endif
                case ExecutionMode.Normal:
                    doc.Stop();
                    doc.Start();
                    break;
#if !WINDOWS_UWP && !NET_STANDARD
                case ExecutionMode.Shared:
                    {
                        if (counterWindowDebuggers > Properties.Settings.Default.MaxRuntimeDebuggers)
                        {
                            if (UIInterface != null)
                            {
                                UIInterface.ShowError(String.Format(Properties.Resources.MaxRuntimeDebuggers,
                                    GetDocumentTitle(uri)));

                            }
                            break;
                        }

                        doc.Stop();
                        if (doc.CurrentRuntimeView != null)
                            doc.CurrentRuntimeView.Activate();
                        else
                        {
                            var uiView = new LogicEditorControl(doc, true);
                            var dialog = new GeneralDialogContent(uiView, GeneralDialogButtons.None)
                            {
                                Title = doc.Title,
                                HelpLink = "LogicManagerEditor"
                            };
                            doc.CurrentRuntimeView = dialog;
                            ++counterWindowDebuggers;
                            dialog.Show();

                            dialog.Closing += (o, e) =>
                            {
                                counterWindowDebuggers--;
                                doc.Stop();
                                doc.CurrentRuntimeView = null;
                            };
                        }
                        doc.Start(true);
                        break;
                    }
#endif
                case ExecutionMode.Synchro:
                    doc.Stop();
                    doc.CycleExecuted += doc_CycleExecuted;
                    doc.Start();
                    break;
                case ExecutionMode.Stop:
                    doc.Stop();
                    break;
            }
        }

        void doc_CycleExecuted(object sender, EventArgs e)
        {
            DocumentEditorDocument doc = sender as DocumentEditorDocument;
            doc.CycleExecuted -= doc_CycleExecuted;
            doc.Stop();
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            var docParent = DocumentHelper.GetRootParent(parent, traverse: false);
            
            if (mapRunningLogicsPerParent.ContainsKey(docParent))
            {
                mapRunningLogicsPerParent[docParent].ForEach(logic =>
                {
                    logic.Dispose();
                });
                mapRunningLogicsPerParent.Remove(docParent);
            }
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
                if (!CloseView(document.ActiveView as LogicEditorControl))
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

        public void Copy(Uri uri, String newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    DocumentEditorDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseView(doc.ActiveView as LogicEditorControl);
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

                    DocumentEditorDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, String oldName, String newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    DocumentEditorDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is LogicEditorControl)
                    {
                        bReopen = true;
                        if (!CloseView(doc.ActiveView as LogicEditorControl))
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

                    var newname = DocumentEditorDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

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
            var doc = DocumentEditorDocument.FromFile(uri.GetPathString(), parent);
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

#if !WINDOWS_UWP && !NET_STANDARD
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            /*
            DocumentEditorDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = CreateDoc(uri, parent);
                if (doc == null)
                    return null;
            }

            var list = new ObservableCollection<IDocumentManager>();
            list.Add(new TreeDocumentManagers.TreeDocumentManager(this, doc, "child tree item"));
            return list;
            */
            return null;
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            var dependencies = new List<String>();
            if (UFUAEditor != null)
                dependencies.Add(UFUAEditor.GetServiceName(parent));

            return new Service.ServiceControl(rootParent.Title, dependencies.ToArray(), rootParent.FilePath);
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
                return "Logic";
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("LGEditorSmall");
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
                return GetBitmapImage("LGEditor");
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
                return ".Logic";
#endif
            }
        }

        public String FileName
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
                return true;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

#if !WINDOWS_UWP && !NET_STANDARD
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

#if !WINDOWS_UWP && !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            String path, newScriptName;
            int i = 0;
            Uri url = null;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newScriptName = String.Format("{0}{1}", Properties.Settings.Default.DefaultLogicName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newScriptName, Properties.Settings.Default.DefaultFileExt);
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
                newScriptName = String.Format("{0}{1}", Properties.Settings.Default.DefaultLogicName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newScriptName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, parent, encryptFile);
            return url;
        }
#endif

        public Type DocumentType
        {
            get
            {
                return typeof(DocumentEditorDocument);
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
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
            if (ToolBox != null)
                ToolBox.PromptForControl -= ToolBox_PromptForControl;
#endif
        }
#endregion IDisposable Members
#region ICrossReference
#if !WINDOWS_UWP && !NET_STANDARD
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            if (!getTags)
                return result;
            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            List<String> list = model.ResourceList as List<String>;
            //Parallel.ForEach(list, (resource, loopstate) =>
            for (int i = 0; i < list.Count(); i++)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return result;

                var resource = list[i];
                var uri = new Uri(string.Format("{0}\\{1}", path, resource), UriKind.RelativeOrAbsolute);
                using (DocumentEditorDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc == null)
                        continue; 
                    var _list = doc.GetCReferenceList(model);
                    //lock (result)
                        result.AddRange(_list);
                }
            }//);

            return result;
        }

        public bool NeedToCloseCRDocuments()
        {
            return mapActiveDocuments.Count > 0 && (from d in mapActiveDocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null;
        }

        public bool NeedSingleThreadedApartment
        {
            get
            {
                return true;
            }
        }

        public void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            foreach (var resource in model.ResourceList)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (DocumentEditorDocument doc = CreateDocument(model.Parent, uri))
                {
                    if(doc != null)
                        doc.RenameReferences(model);
                }
            }
        }
        public void EditCRObject(IDocument parent, string settings)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            string[] path = settings.Split('|');
            if (path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                Edit(uri, p);
                try
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
                    {
                        GetActiveView(uri).SelectElement(path[0]);
                    });
                }
                catch (Exception)
                {
                }
            }
        }
        private LogicEditorControl GetActiveView(Uri uri)
        {
            DocumentEditorDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as LogicEditorControl;
            return null;
        }
        private DocumentEditorDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            DocumentEditorDocument doc = null;
            doc = DocumentEditorDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
        private DocumentEditorDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            DocumentEditorDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = DocumentEditorDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = parent;

                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
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
        #endregion
    }
}