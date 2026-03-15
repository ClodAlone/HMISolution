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
using UFInterfaces.AuthenticationCredentialsProvider;
using VFS;
using UFUAEditor.ComponentService;
using UFShortcutSettings.Documents;
using PropertyControl.ComponentService;
using log4net;
using CommandExplorer.ComponentService;
using System.Windows.Input;
using UFShortcutEditor.PropertyDataTemplate;
using UFShortcutSettings.ShortcutModel;
using System.Windows.Threading;
using UFUserEditor.ComponentService;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;

namespace UFShortcutEditor.ComponentService
{
    public class ShortcutEditorManagerComponent : ComponentBase<IShortcutEditorManager>, IShortcutEditorManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UFShortcutDocument> mapActiveDocuments = new Dictionary<String, UFShortcutDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFShortcutDocument, String> mapActiveDocumentUris = new Dictionary<UFShortcutDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        public static ShortcutEditorManagerComponent shortcuteditorManagerComponent { get; protected set; }
        MenuControl menuControl;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (shortcuteditorManagerComponent == null)
                shortcuteditorManagerComponent = this;

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

            if (PropertyControl != null)
            {

                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ShortcutKeyPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ShortcutKey", typeof(string), typeof(UFKeyCommandEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("WritableAccessMask", typeof(int), typeof(UFKeyCommandEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Commands", typeof(bool), typeof(UFKeyCommandEntity), typeof(UFShortcutDocument), dt);


                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (UserEditor != null)
                    {
                        Type accRoleType = UserEditor.GetAccessRoleEditorType();
                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(accRoleType);
                        dt.DataType = typeof(string);
                        dt.VisualTree = factory;
                        PropertyControl.AddPropertyEditor("AccessRole", typeof(string), typeof(UFKeyCommandEntity), dt);
                    }
                });

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
            if (!(e.TargetItem is UFShortcutEditorUI))
                return;

            var view = e.TargetItem as UFShortcutEditorUI;
            if (!CloseShortcut(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(UFShortcutEditorUI view, bool bSave = true)
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

        private bool CloseShortcut(UFShortcutEditorUI view, bool bSave = true, bool bDispose = true)
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
                        //view.SaveRecipeLayout();
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
                // view.Dispose();
                if (view.Document is IDisposable)
                    (view.Document as IDisposable).Dispose();
            }

            return true;
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            var array = new UFShortcutDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UFShortcutEditorUI;
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

            if (e.OldValue != null && e.OldValue is UFShortcutEditorUI && viewOld != null)
            {
                var view = e.OldValue as UFShortcutEditorUI;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UFShortcutEditorUI && viewNew != null)
            {
                var view = e.NewValue as UFShortcutEditorUI;

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

        internal UFShortcutEditorUI GetViewFromUri(Uri uri)
        {
            UFShortcutDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UFShortcutEditorUI;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UFShortcutDocument> keyvaluepair in mapActiveDocuments)
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
            using (var newProject = new UFShortcutDocument() { FullPath = uri.GetPathString(), Parent = parent})
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

        private UFShortcutEditor.UFShortcutEditorUI CreateDocView(UFShortcutDocument doc, String title, IDocument parent)
        {
            var editor = new UFShortcutEditorUI(this, doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("SCTMEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", title, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFShortcutDocument CreateDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = UFShortcutDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }
        #region ICrossReference
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getScreen = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            bool getStrings = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getTags && !getScreen && !getConnections && !getStrings)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string docType = DocManagerType.ShortcutEditor.ToString();
            List<String> list = model.ResourceList as List<String>;
            //Parallel.ForEach(list, (resource, loopstate) =>
            for (int i = 0; i < list.Count(); i++)
            {
                var resource = list[i];
                if (model.QuitEvent.IsCancellationRequested)
                    break;
                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (UFShortcutDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        List<UFKeyCommandEntity> keytemlist = doc.GetShortcutItemCollection() as List<UFKeyCommandEntity>;
                        for (int j = 0; j < keytemlist.Count(); j++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                break;

                            var key = keytemlist[j];
                            if (getTags && key.EnableTag != null && key.EnableTag.IsValid)
                            {
                                var details = key.EnableTag;
                                lock (result)
                                {
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = details.RelativePath,
                                        Name = details.Name,
                                        AppName = details.AppName,
                                        ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                        EndpointUrl = details.EndpointUrl,
                                        CReferenceType = CrossReferenceType.Tags,
                                        Description = string.Format("{0}\\{1} ({2})", doc.Title, key.Name, Properties.Resources.NewKeyCommandDefinitionEnableTag),
                                        Settings = string.Format("{0}|{1}", DocManagerType.ShortcutEditor, uri.GetPathString()),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                                }
                            }
                            if(getStrings && !string.IsNullOrEmpty(key.SpeechCommand))
                                lock (result)
                                {
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = key.SpeechCommand,
                                        Name = key.SpeechCommand,
                                        AppName = doc.Title,
                                        CReferenceType = CrossReferenceType.Strings,
                                        Description = string.Format("{0}\\{1} ({2})", doc.Title, key.Name, Properties.Resources.NewKeyCommandDefinitionSpeech),
                                        Settings = string.Format("{0}|{1}", DocManagerType.ShortcutEditor, uri.GetPathString()),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                                }

                            IUFUAEditorManager service = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                            var cRMapsHeler = new CRMapsHelper() { CrossReferenceTypes  = model.CRManagement.CrossReferenceTypeList};
                            key.GetAllSourceEntityReferencesDetails(doc, service, cRMapsHeler);
                            if (cRMapsHeler.Tags != null)
                            {
                                foreach (var tag in cRMapsHeler.Tags)
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        break;

                                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                                    if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value.RelativePath))
                                    {
                                        var details = refdetails.First().Value;
                                        //lock (result)
                                        {
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = details.RelativePath,
                                                Name = details.Name,
                                                AppName = details.AppName,
                                                ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                                EndpointUrl = details.EndpointUrl,
                                                CReferenceType = CrossReferenceType.Tags,
                                                Description = string.Format("{0}\\{1} ({2})", doc.Title, key.Name, refdetails.First().Key),
                                                Settings = string.Format("{0}|{1}", key.Name, uri.GetPathString()),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                        }
                                    }
                                }
                            }

                            if (cRMapsHeler.ScreenLinks != null)
                            {
                                foreach (var tag in cRMapsHeler.ScreenLinks)
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        break;

                                    IDictionary<String, String> refdetails = tag as IDictionary<String, String>;
                                    if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value))
                                    {
                                        var name = System.IO.Path.GetFileNameWithoutExtension(refdetails.First().Value);
                                        //lock (result)
                                        {
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = refdetails.First().Value,
                                                Name = name,
                                                AppName = p.Title,
                                                CReferenceType = CrossReferenceType.Resources,
                                                Description = string.Format("{0}\\{1} ({2})", doc.Title, key.Name, refdetails.First().Key),
                                                Settings = string.Format("{0}|{1}", key.Name, uri.GetPathString()),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }//);

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
        
        private UFShortcutDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            UFShortcutDocument doc = null;
            doc = UFShortcutDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        private UFShortcutDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            UFShortcutDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFShortcutDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
        }

        private UFShortcutEditorUI GetActiveView(Uri uri)
        {
            UFShortcutDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as UFShortcutEditorUI;
            return null;
        }


        #region IDocumentManager Members

        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFShortcutDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseShortcut(doc.ActiveView as UFShortcutEditorUI))
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
                    UFShortcutDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is UFShortcutEditorUI)
                            CloseShortcut(doc.ActiveView as UFShortcutEditorUI, false);
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

                    UFShortcutDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                }
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
//            var Parent = parent.Parent;
//            if (Parent == null)
//                Parent = parent;
//            UFRecipeDocument doc = null;
//            uri = parent.MakeAbosoluteUri(uri);

//            UFRecipeExecuter.UFRecipeExecuter currentexecuter = null;
//            if (mapRunningRecipesPerParent.ContainsKey(Parent))
//            {
//                var executers = (from c in mapRunningRecipesPerParent[Parent]
//                                 where c.RecipeDocument.FullPath == uri.GetPathString()
//                                 select c).ToList();
//                if (executers.Count > 0)
//                    currentexecuter = executers[0];
//            }

//            if (currentexecuter != null)
//                doc = currentexecuter.RecipeDocument;
//            else if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
//            {
//                if (!UFRecipeDocument.ExistFile(uri.GetPathString(), parent.fileSystemProviderBase))
//                {
//                    if (UIInterface != null)
//                    {
//                        UIInterface.ShowError(String.Format(Properties.Resources.DocNotFound,
//                            GetDocumentTitle(uri)));
//                    }
//                    return;
//                }

//                doc = UFRecipeDocument.FromFile(uri.GetPathString(), parent.fileSystemProviderBase);
//                if (doc == null)
//                    return;
//                doc.Parent = Parent;
//                //mapActiveDocuments.Add(uri.GetPathString(), doc);
//            }

//#if !DEBUG
//            var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxLpZ9HLAzv1ul2RsoVlDyXw=="/* RCP */);
//            if (state == false)
//            {
//                logLicense.Error(Properties.Resources.NoRecipeLicense);
//                return;
//            }
//#endif

//            UFRecipeExecuter.UFRecipeExecuter executer = null;
//            if (!mapRunningRecipes.TryGetValue(doc, out executer))
//            {
//                executer = new UFRecipeExecuter.UFRecipeExecuter(doc, UfuaEditorService, UIInterface);
//                mapRunningRecipes.Add(doc, executer);
//            }

//            if (!mapRunningRecipesPerParent.ContainsKey(Parent))
//                mapRunningRecipesPerParent.Add(Parent, new List<UFRecipeExecuter.UFRecipeExecuter>());
//            if (!mapRunningRecipesPerParent[Parent].Contains(executer))
//                mapRunningRecipesPerParent[Parent].Add(executer);

//            executer.Start(parent, mode, Context);
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            if (Parent != null && mapActiveShortcuts.ContainsKey(Parent))
            {
                mapActiveShortcuts[Parent].TerminateActive();
                mapActiveShortcuts.Remove(Parent);
            }

            //if (mapRunningRecipesPerParent.ContainsKey(Parent))
            //{
            //    mapRunningRecipesPerParent[Parent].ForEach(recipe =>
            //    {
            //        //recipe.Stop();
            //        var docs = (from c in mapRunningRecipes where c.Value == recipe select c.Key).ToList();
            //        if (docs.Count > 0)
            //        {
            //            mapRunningRecipes.Remove(docs[0]);
            //            docs[0].Dispose();
            //        }
            //    });
            //    mapRunningRecipesPerParent.Remove(Parent);
            //}
        }

        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                //(document.ActiveView as UFShortcutEditorUI).SaveRecipeLayout();
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
                if (!CloseShortcut(document.ActiveView as UFShortcutEditorUI))
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
                    UFShortcutDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseShortcut(doc.ActiveView as UFShortcutEditorUI);
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

                    UFShortcutDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFShortcutDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is UFShortcutEditorUI)
                    {
                        bReopen = true;
                        if (!CloseShortcut(doc.ActiveView as UFShortcutEditorUI))
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

                    var newname = UFShortcutDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

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
            var doc = UFShortcutDocument.FromFile(uri.GetPathString(), parent);
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
            UFShortcutDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFShortcutDocument.FromFile(uri.GetPathString(), parent);
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
                return GetBitmapImage("SCTMEditorSmall");
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
                return GetBitmapImage("SCTMEditor");
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

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            String path, newReportName;
            int i = 0;
            Uri url;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newReportName = String.Format("{0}{1}", UFShortcutDocument.DefaultRecipeName, ++i);
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
                newReportName = String.Format("{0}{1}", UFShortcutDocument.DefaultRecipeName, ++i);
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
                return typeof(UFShortcutDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IShortcutEditorManager Members

        readonly Dictionary<IDocument, UFShortcutDocument> mapActiveShortcuts = new Dictionary<IDocument,UFShortcutDocument>();
        readonly Dictionary<IDocument, IList<String>> mapActiveSpeechCommands = new Dictionary<IDocument, IList<String>>();
        public IList<String> Activate(IDocument parent, Uri uri, String sessionName, bool bLookForDefault)
        {
            IList<String> ret = null;
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            var uriAbsolute = Parent.MakeAbosoluteUri(uri);
            var doc = CreateDoc(uriAbsolute, Parent);
            if (doc == null)
            {
                var FullPath = uri.GetPathString();
                var name = System.IO.Path.ChangeExtension(FullPath, Properties.Settings.Default.DefaultFileExt);
                uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                doc = CreateDoc(uriAbsolute, Parent);
                if (doc == null)
                {
                    FullPath = String.Format("{0}\\{1}", TypeLabel, uri.GetPathString());
                    name = System.IO.Path.ChangeExtension(FullPath, Properties.Settings.Default.DefaultFileExt);

                    uri = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                    doc = CreateDoc(uri, Parent);
                    if (doc == null && bLookForDefault)
                    {
                        while (Parent.Parent != null)
                            Parent = Parent.Parent;

                        name = System.IO.Path.ChangeExtension(String.Format("{0}\\Main", TypeLabel), Properties.Settings.Default.DefaultFileExt);
                        uri = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                        doc = CreateDoc(uri, Parent);
                    }
                }
            }

            lock (lockObject)
            {
                if (doc != null)
                {
                    if (mapActiveShortcuts.ContainsKey(Parent))
                    {
                        if (mapActiveShortcuts[Parent] == doc)
                        {
                            if (mapActiveSpeechCommands.ContainsKey(Parent))
                                return mapActiveSpeechCommands[Parent];
                            return ret;
                        }
                        mapActiveShortcuts[Parent].TerminateActive();
                        mapActiveShortcuts.Remove(Parent);
                    }

                    ret = doc.PrepareActive(sessionName);
                    mapActiveShortcuts.Add(Parent, doc);
                    if (ret != null)
                    {
                        if (mapActiveSpeechCommands.ContainsKey(Parent))
                            mapActiveSpeechCommands.Remove(Parent);
                        mapActiveSpeechCommands.Add(Parent, ret);
                    }
                }
                else
                {
                    if (mapActiveShortcuts.ContainsKey(Parent))
                    {
                        mapActiveShortcuts[Parent].TerminateActive();
                        mapActiveShortcuts.Remove(Parent);
                    }
                }
            }

            return ret;
        }

        public void Terminate(IDocument parent)
        {
            var Parent = parent;
            //var Parent = parent.Parent;
            //if (Parent == null)
            //    Parent = parent;

            lock (lockObject)
            {
                if (Parent != null && mapActiveShortcuts.ContainsKey(Parent))
                {
                    mapActiveShortcuts[Parent].TerminateActive();
                    mapActiveShortcuts.Remove(Parent);
                    if (mapActiveSpeechCommands.ContainsKey(Parent))
                        mapActiveSpeechCommands.Remove(Parent);
                }
            }
        }

        public bool ExecuteCommand(IDocument parent, String Command)
        {
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            UFShortcutDocument doc = null;
            lock (lockObject)
            {
                if (mapActiveShortcuts.ContainsKey(Parent))
                    doc = mapActiveShortcuts[Parent];
            }

            if (doc != null)
                return doc.ExecuteCommand(Command);
            else
                return false;
        }

        public bool ExecuteGestureOnDown(IDocument parent, KeyEventArgs e)
        {
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            UFShortcutDocument doc = null;
            lock (lockObject)
            {
                if (mapActiveShortcuts.ContainsKey(Parent))
                    doc = mapActiveShortcuts[Parent];
            }

            if (doc != null)
                return doc.ExecuteDown(e);
            else
                return false;
        }

        public bool ExecuteGestureOnUp(IDocument parent, KeyEventArgs e)
        {
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            UFShortcutDocument doc = null;
            lock (lockObject)
            {
                if (mapActiveShortcuts.ContainsKey(Parent))
                    doc = mapActiveShortcuts[Parent];
            }

            if (doc != null)
                return doc.ExecuteUp(e);
            else
                return false;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            //foreach (IDisposable recipe in mapRunningRecipes.Values)
            //    recipe.Dispose();
            //mapRunningRecipes.Clear();
            //mapRunningRecipesPerParent.Clear();
            

            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
        }

        #endregion IDisposable Members       

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
            foreach (var resource in model.ResourceList)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (UFShortcutDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                        doc.RenameReferences(model);
                }
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
    }
}