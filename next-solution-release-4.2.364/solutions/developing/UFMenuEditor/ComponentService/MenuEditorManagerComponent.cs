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
using MenuSettings.Documents;
using PropertyControl.ComponentService;
using log4net;
using CommandExplorer.ComponentService;
using StringManager.ComponentService;
using System.Windows.Controls.Primitives;
using UFMenuEditor.PropertyDataTemplate;
using System.Windows.Threading;
using WPFUtilities.PropertyDataTemplate;
using CommonControls.PropertyDataTemplate;
using UFUserEditor.ComponentService;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;
using Utilities.WPF;
using System.Windows.Input;

namespace UFMenuEditor.ComponentService
{
    public class MenuEditorManagerComponent : ComponentBase<IMenuEditorManager>, IMenuEditorManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UFMenuDocument> mapActiveDocuments = new Dictionary<String, UFMenuDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFMenuDocument, String> mapActiveDocumentUris = new Dictionary<UFMenuDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        
        public static MenuEditorManagerComponent menueditorManagerComponent { get; protected set; }
        MenuControl menuControl;
        List<CommandBinding> globalCbs;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (menueditorManagerComponent == null)
                menueditorManagerComponent = this;

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
                var factory = new FrameworkElementFactory();

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, workspace);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Images);
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("MenuItemImage", typeof(Uri), typeof(MenuSettings.MenuModel.UFMenuItemEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(MenuTypePropertyEditor));
                dt.DataType = typeof(MenuSettings.MenuModel.MenuType);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(MenuSettings.MenuModel.MenuType), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(MenuItemTypePropertyEditor));
                dt.DataType = typeof(MenuSettings.MenuModel.MenuType);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("MenuItemType", typeof(MenuSettings.MenuModel.MenuType), typeof(MenuSettings.MenuModel.UFMenuItemEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Name", typeof(string), typeof(MenuSettings.MenuModel.UFMenuItemEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("WritableAccessMask", typeof(int), typeof(MenuSettings.MenuModel.UFMenuItemEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Commands", typeof(bool), typeof(MenuSettings.MenuModel.UFMenuItemEntity), typeof(UFMenuDocument), dt);


                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (UserEditor != null)
                    {
                        Type accRoleType = UserEditor.GetAccessRoleEditorType();
                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(accRoleType);
                        dt.DataType = typeof(string);
                        dt.VisualTree = factory;
                        PropertyControl.AddPropertyEditor("AccessRole", typeof(string), typeof(MenuSettings.MenuModel.UFMenuItemEntity), dt);
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
            if (!(e.TargetItem is UFMenuEditorUI))
                return;

            var view = e.TargetItem as UFMenuEditorUI;
            if (!CloseMenu(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(UFMenuEditorUI view, bool bSave = true)
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

        private bool CloseMenu(UFMenuEditorUI view, bool bSave = true, bool bDispose = true)
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

            var array = new UFMenuDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UFMenuEditorUI;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void GetGlobalCbs(UFMenuEditorUI view)
        {
            var globalCmds = (from DevExpress.Xpf.Bars.BarItem bi in menuControl.menuGeneralItems.GetChildrenOfType<DevExpress.Xpf.Bars.BarItem>() where bi.Command != null select bi.Command).ToList();
            globalCbs = (from CommandBinding cb in view.CommandBindings where globalCmds.Contains(cb.Command) select cb).ToList();
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is UFMenuEditorUI && viewOld != null)
            {
                var view = e.OldValue as UFMenuEditorUI;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.RemoveBarManagerCommands(new CommandBindingCollection(globalCbs));

                view.OnDeactivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UFMenuEditorUI && viewNew != null)
            {
                var view = e.NewValue as UFMenuEditorUI;
                workspace.ContextDocument = view.Document;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.AddBarManagerGlobalCommands(new CommandBindingCollection(globalCbs));

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

        internal UFMenuEditorUI GetViewFromUri(Uri uri)
        {
            UFMenuDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UFMenuEditorUI;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UFMenuDocument> keyvaluepair in mapActiveDocuments)
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
            using (var newProject = new UFMenuDocument() { FullPath = uri.GetPathString(), Parent = parent})
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

        public IPropertyControl propertyControl;
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

        private UFMenuEditor.UFMenuEditorUI CreateDocView(UFMenuDocument doc, String title, IDocument parent)
        {
            var editor = new UFMenuEditorUI(this, doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("MMEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", title, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFMenuDocument CreateDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = UFMenuDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            if (!mapActiveDocuments.ContainsKey(fullPath))
            mapActiveDocuments.Add(fullPath, doc);
            if (!mapActiveDocumentUris.ContainsValue(fullPath))
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
            bool getScreen = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens);
            bool getStrings = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            if (!getScreen && !getTags && !getStrings)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            List<String> list = model.ResourceList as List<String>;
            Parallel.ForEach(list, (resource, loopstate) =>
            //for (int i = 0; i < list.Count(); i++)
            {
                //var resource = list[i];
                if (model.QuitEvent.IsCancellationRequested)
                    loopstate.Break();
                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (UFMenuDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        List<MenuSettings.MenuModel.UFMenuItemEntity> menuitemlist = doc.GetMenuItemCollection() as List<MenuSettings.MenuModel.UFMenuItemEntity>;
                        for (int j = 0; j < menuitemlist.Count(); j++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                loopstate.Break();

                            var menu = menuitemlist[j];
                            lock (result)
                            {
                                result.AddRange(GetMenuItemsCRList(model, menu, doc, uri.GetPathString(), p, getScreen, getTags, getStrings));
                            }
                        }
                    }
                }
            });

            return result;
        }

        private List<UFInterfaces.Editors.CrossReferenceResultModel> GetMenuItemsCRList(UFInterfaces.Editors.CrossReferenceModel model, MenuSettings.MenuModel.UFMenuItemEntity menu, UFMenuDocument doc, string pathstring, IDocument p, bool getScreen, bool getTags, bool getStrings)
        {
            string docType = DocManagerType.MenuEditor.ToString();
            List<UFInterfaces.Editors.CrossReferenceResultModel> result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();

            if (getStrings && !string.IsNullOrEmpty(menu.Name))
            {
                var details = menu.Name;
                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                {
                    RelativePath = details,
                    Name = details,
                    AppName = p.Title,
                    CReferenceType = CrossReferenceType.Strings,
                    Description = string.Format("{0}\\{1} ({2})", doc.Title, details, Properties.Resources.ItemName),
                    Settings = string.Format("{0}|{1}", docType, pathstring),
                    ContainerDoc = TypeScheme,
                    IconType = docType
                });
            }
            if (getTags || getScreen)
            {
                if (menu.EnableTag != null && menu.EnableTag.IsValid)
                {
                    var details = menu.EnableTag;
                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = details.RelativePath,
                        Name = details.Name,
                        AppName = details.AppName,
                        ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                        EndpointUrl = details.EndpointUrl,
                        CReferenceType = CrossReferenceType.Tags,
                        Description = string.Format("{0}\\{1} ({2})", doc.Title, menu.Name, Properties.Resources.NewMenuItemEnableTag),
                        Settings = string.Format("{0}|{1}", docType, pathstring),
                        ContainerDoc = TypeScheme,
                        IconType = docType
                    });
                }

                if (menu.MarkTag != null && menu.MarkTag.IsValid)
                {
                    var details = menu.MarkTag;
                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = details.RelativePath,
                        Name = details.Name,
                        AppName = details.AppName,
                        ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                        EndpointUrl = details.EndpointUrl,
                        CReferenceType = CrossReferenceType.Tags,
                        Description = string.Format("{0}\\{1} ({2})", doc.Title, menu.Name, Properties.Resources.NewMenuItemMarkTag),
                        Settings = string.Format("{0}|{1}", docType, pathstring),
                        ContainerDoc = TypeScheme,
                        IconType = docType
                    });
                }
                IUFUAEditorManager service = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                var cRMapsHeler = new DocumentManager.ComponentService.CRMapsHelper() { CrossReferenceTypes = model.CRManagement.CrossReferenceTypeList };
                menu.GetAllSourceEntityReferencesDetails(doc, service, cRMapsHeler, getScreen, getTags);
                if (cRMapsHeler.Tags != null)
                {
                    foreach (var tag in cRMapsHeler.Tags)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return result;

                        IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                        if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value.RelativePath))
                        {
                            var details = refdetails.First().Value;
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = details.RelativePath,
                                Name = details.Name,
                                AppName = details.AppName,
                                ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                EndpointUrl = details.EndpointUrl,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0}\\{1} ({2})", doc.Title, menu.Name, refdetails.First().Key),
                                Settings = string.Format("{0}|{1}", menu.Name, pathstring),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                        }
                    };
                }
                if (cRMapsHeler.ScreenLinks != null)
                {
                    foreach (var tag in cRMapsHeler.ScreenLinks)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return result;

                        IDictionary<String, String> refdetails = tag as IDictionary<String, String>;
                        if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value))
                        {
                            var relativePath = refdetails.First().Value.Remove(refdetails.First().Value.LastIndexOf(".xaml"));
                            var name = relativePath.Split('/').LastOrDefault();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = relativePath,
                                Name = name,
                                AppName = p.Title,
                                CReferenceType = CrossReferenceType.Screens,
                                Description = string.Format("{0}\\{1} ({2})", doc.Title, menu.Name, refdetails.First().Key),
                                Settings = string.Format("{0}|{1}", menu.Name, pathstring),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                        }
                    };
                }
            }
            menu.MenuItems.ForEach(item =>
                {
                    result.AddRange(GetMenuItemsCRList(model, item, doc, pathstring, p, getScreen, getTags, getStrings));
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
        
        private UFMenuDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            UFMenuDocument doc = null;
            doc = UFMenuDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        private UFMenuDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            UFMenuDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFMenuDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
        }
        private UFMenuEditorUI GetActiveView(Uri uri)
        {
            UFMenuDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as UFMenuEditorUI;
            return null;
        }

        #region IDocumentManager Members

        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFMenuDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == null)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseMenu(doc.ActiveView as UFMenuEditorUI))
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
                    UFMenuDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is UFMenuEditorUI)
                            CloseMenu(doc.ActiveView as UFMenuEditorUI, false);
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

                    UFMenuDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
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

            TerminateAllRuntimeMenu();
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
                if (!CloseMenu(document.ActiveView as UFMenuEditorUI))
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
                    UFMenuDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseMenu(doc.ActiveView as UFMenuEditorUI);
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

                    UFMenuDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFMenuDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is UFMenuEditorUI)
                    {
                        bReopen = true;
                        if (!CloseMenu(doc.ActiveView as UFMenuEditorUI))
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

                    var newname = UFMenuDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

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
            var doc = UFMenuDocument.FromFile(uri.GetPathString(), parent);
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
            UFMenuDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFMenuDocument.FromFile(uri.GetPathString(), parent);
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
                return GetBitmapImage("MMEditorSmall");
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("MMEditor");
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
                    newReportName = String.Format("{0}{1}", UFMenuDocument.DefaultRecipeName, ++i);
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
                newReportName = String.Format("{0}{1}", UFMenuDocument.DefaultRecipeName, ++i);
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
                return typeof(UFMenuDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IMenuEditorManager Members

        private UFMenuDocument OpenNewDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();

            var doc = UFMenuDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            return doc;
        }

        readonly Dictionary<MenuBase, UFMenuDocument> mapActiveMenus = new Dictionary<MenuBase, UFMenuDocument>();
        public MenuBase GetMenu(IDocument parent, Uri uri, String sessionName, bool bLookForDefault, bool bContextMenu, bool bTest = false)
        {
            MenuBase ret = null;
            var Parent = parent.Parent;
            if (Parent == null)
                Parent = parent;

            var uriAbsolute = Parent.MakeAbosoluteUri(uri);
            var doc = OpenNewDoc(uriAbsolute, Parent);
            if (doc == null)
            {
                var FullPath = uri.GetPathString();
                var name = System.IO.Path.ChangeExtension(FullPath, Properties.Settings.Default.DefaultFileExt);
                uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                doc = OpenNewDoc(uriAbsolute, Parent);
                if (doc == null)
                {
                    FullPath = String.Format("{0}\\{1}", TypeLabel, uri.GetPathString());
                    name = System.IO.Path.ChangeExtension(FullPath, Properties.Settings.Default.DefaultFileExt);

                    uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                    doc = OpenNewDoc(uriAbsolute, Parent);
                    if (doc == null && bLookForDefault)
                    {
                        name = System.IO.Path.ChangeExtension(String.Format("{0}\\Main", TypeLabel), Properties.Settings.Default.DefaultFileExt);
                        uriAbsolute = Parent.MakeAbosoluteUri(new Uri(name, UriKind.RelativeOrAbsolute));
                        doc = OpenNewDoc(uriAbsolute, Parent);
                    }
                }
            }

            if (doc != null)
            {
                IDictionary<String, String> mapCulture = null;
                if (StringEditor != null)
                {
                    var culture = StringEditor.GetActiveCulture(parent);
                    mapCulture = StringEditor.GetListStringForCulture(parent, culture);
                }
                ret = doc.PrepareActive(sessionName, mapCulture, bContextMenu, bTest);
                if (ret != null && !bTest)
                {
                    lock(lockObject)
                        mapActiveMenus.Add(ret, doc);
                }
                
                if(ret!=null)
                {
                    var p = Parent;
                    var screenController = (p as IScreenController);
                    while (p != null && screenController == null)
                    {
                        p = p.Parent;
                        screenController = (p as IScreenController);
                    }
                    if (screenController != null)
                    {
                        var style = screenController.GetTheme();
                        WPFUtilities.ThemeHelper.SetTheme(ret, style.ToString());
                    }
                }
            }

            return ret;
        }

        public void TerminateMenu(MenuBase menu)
        {
            lock (lockObject)
            {
                if (mapActiveMenus.ContainsKey(menu))
                {
                    mapActiveMenus[menu].TerminateActive();
                    mapActiveMenus[menu].Dispose();
                    mapActiveMenus.Remove(menu);
                }
            }
        }

        void TerminateAllRuntimeMenu()
        {
            lock (lockObject)
            {
                while (mapActiveMenus.Count > 0)
                    TerminateMenu(mapActiveMenus.Last().Key);
            }
        }
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
                using (UFMenuDocument doc = CreateDocument(model.Parent, uri))
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
    public class CRMapsHelper
    {
        public IEnumerable<dynamic> ScreenLinks { get; set; }
        public IEnumerable<dynamic> Tags { get; set; }
    }
}