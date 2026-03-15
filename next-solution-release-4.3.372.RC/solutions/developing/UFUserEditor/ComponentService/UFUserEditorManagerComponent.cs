using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces.AuthenticationCredentialsProvider;
using PropertyControl.ComponentService;
using WPFUtilities.Extensions;
using UFUserEditor.PropertyDataTemplate;
using WPFUtilities.PropertyDataTemplate;
using StringManager.ComponentService;
using CommonControls.PropertyDataTemplate;
using ADEditor.ComponentService;
using UFUserEditor.Controls;
#endif
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using UFUserEditor.Document;
using UFUserModel;
using System.Threading.Tasks;
using XpoHelpers;
using log4net;
#if !NET_STANDARD
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using OPCUAViewModel.PropertyDataTemplate;
#endif

namespace UFUserEditor.ComponentService
{
    public class UFUserEditorManagerComponent : ComponentBase<IUFUserEditorManager>, IUFUserEditorManager, IDocumentManager, IDisposable
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly List<String> listRunningDocuments = new List<string>();
        readonly Dictionary<String, UFUserDocument> mapActiveDocuments = new Dictionary<String, UFUserDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<String, UFUserDocument> mapRunningDocuments = new Dictionary<String, UFUserDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFUserDocument, String> mapActiveDocumentUris = new Dictionary<UFUserDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        static public UFUserEditorManagerComponent userEditorManagerComponent;

#if NET_STANDARD
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.UserManager);
#endif

        static internal string stringPlaceolder = "UserManagement";
#if !NET_STANDARD
        MenuControl menuControl;
		String currentUserName;
#endif

        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (userEditorManagerComponent == null)
                userEditorManagerComponent = this;

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

            if (StringEditorManager != null)
                StringEditorManager.CultureChanged += StringEditor_CultureChanged;

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(PasswordPropertyEditor));
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Password", typeof(String), typeof(UFUserModel.UFUser), dt);
                PropertyControl.AddPropertyEditor("Password", typeof(String), typeof(UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PasswordPropertyEditor));
                factory.SetValue(PasswordPropertyEditor.WorkspaceProperty, workspace);
                factory.SetValue(PasswordPropertyEditor.ValidationNamesProperty, new String[] { "Password" });
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("PasswordConfirm", typeof(String), typeof(UFUserModel.UFUser), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("AccessMask", typeof(Nullable<int>), typeof(UFUserModel.UFRole), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                factory.SetValue(BitMaskPropertyEditor.ShowInheritedButtonProperty, true);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("AccessMask", typeof(Nullable<int>), typeof(UFUserModel.UFUser), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(AccessRolePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("DesktopSystemRole", typeof(string), typeof(UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(CulturePropertyControlxaml));
                factory.SetValue(CulturePropertyControlxaml.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("CultureName", typeof(String), typeof(UFUserModel.UFUser), dt);
                PropertyControl.AddPropertyEditor("CultureName", typeof(String), typeof(UFUserModel.UFRole), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ConverterPropertyControl));
                factory.SetValue(ConverterPropertyControl.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ConverterName", typeof(String), typeof(UFUserModel.UFUser), dt);
                PropertyControl.AddPropertyEditor("ConverterName", typeof(String), typeof(UFUserModel.UFRole), dt);

                // Configuration Data Templates
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SharedConnectionSourcePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("SharedConnectionRepository", typeof(string), typeof(UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SharedConnectionSourcePropertyEditor));
                factory.SetValue(SharedConnectionSourcePropertyEditor.IsBackupRepositoryProperty, true);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("SharedConnectionRepositoryBackup", typeof(string), typeof(UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 1.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int32.MaxValue);
                dt.DataType = typeof(Int32);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("MaxInvalidPasswordAttempts", typeof(Nullable<Int32>), typeof(UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int32.MaxValue);
                dt.DataType = typeof(Int32);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("PasswordExpiresInDays", typeof(Int32), typeof(UFUserModel.UFUser), dt);
                PropertyControl.AddPropertyEditor(nameof(UFUserModel.UFUserSettings.NotifyPasswordExpiresInDays), typeof(Int32), typeof(UFUserModel.UFUserSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditorFromXML));
                factory.SetValue(OPCUAEntityReferencePropertyEditorFromXML.AllowDataSyncProperty, false);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("TagPhoneNumber", typeof(string), typeof(UFUserModel.UFUser), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PasswordPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ClientSecret", typeof(string), typeof(UFUserSettings), dt);
                
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(AccessRolePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("DefaultExternalAuthenticationRole", typeof(string), typeof(UFUserSettings), dt);


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
            if (!(e.TargetItem is UFUserEditorControl))
                return;

            var view = e.TargetItem as UFUserEditorControl;
            if (!CloseView(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(UFUserEditorControl view, bool bSave = true)
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

        private bool CloseView(UFUserEditorControl view, bool bSave = true)
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

            UFUserDocument[] array = new UFUserDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UFUserEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            UFUserDocument[] array = new UFUserDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                var view = doc.ActiveView as UFUserEditorControl;
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

            if (e.OldValue != null && e.OldValue is UFUserEditorControl && viewOld != null)
            {
                var view = e.OldValue as UFUserEditorControl;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UFUserEditorControl && viewNew != null)
            {
                var view = e.NewValue as UFUserEditorControl;
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

        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
            var list = mapAddressSpaceControls.Keys.ToList();
            foreach (var doc in list)
            {
                var culture = StringEditorManager.GetActiveCulture(doc);
                var cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(culture);
                if (cultureInfo != mapAddressSpaceControls[doc].Dispatcher.Thread.CurrentUICulture)
                {
                    mapAddressSpaceControls[doc].Dispose();
                    mapAddressSpaceControls.Remove(doc);
                }
            }
            list = mapRuntimeAddressSpaceControls.Keys.ToList();
            foreach (var doc in list)
            {
                var culture = StringEditorManager.GetActiveCulture(doc);
                var cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(culture);
                if (cultureInfo != mapRuntimeAddressSpaceControls[doc].Dispatcher.Thread.CurrentUICulture)
                {
                    mapRuntimeAddressSpaceControls[doc].Dispose();
                    mapRuntimeAddressSpaceControls.Remove(doc);
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

        internal UFUserEditorControl GetViewFromUri(Uri uri)
        {
            UFUserDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UFUserEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UFUserDocument> keyvaluepair in mapActiveDocuments)
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
            //UFUAServerDocument newProject = new UFUAServerDocument(this);
            //newProject.ProjectPath = uri.GetPathString();
            //if (newProject.SaveToFile())
            //    newProject.CreateProjectFolders();
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

        IUFProjectManager projectManager;
        public IUFProjectManager ProjectManager
        {
            get
            {
                if (projectManager == null)
                    projectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                return projectManager;
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

        IStringEditorManager _stringEditorManager;
        public IStringEditorManager StringEditorManager
        {
            get
            {
                if (_stringEditorManager == null)
                    _stringEditorManager = userEditorManagerComponent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return _stringEditorManager;
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

        IADEditorManager _dispatcherEditorManager;
        public IADEditorManager DispatcherEditorManager
        {
            get
            {
                if (_dispatcherEditorManager == null)
                    _dispatcherEditorManager = userEditorManagerComponent.GetService(typeof(IADEditorManager)) as IADEditorManager;
                return _dispatcherEditorManager;
            }
        }
#endif
        #endregion Properties
#if !NET_STANDARD
        private UFUserEditor.UFUserEditorControl CreateDocView(UFUserDocument doc, String title, IDocument parent)
        {
            var ufuaserverEditor = new UFUserEditorControl(doc);
            doc.ActiveView = ufuaserverEditor;
            if (doc.NeedsSave)
                doc.SaveToFile();

            workspace.SetDesiredHeightAndWidthInDockedMode(ufuaserverEditor, ufuaserverEditor.Height, ufuaserverEditor.Width);
            ufuaserverEditor.ClearValue(FrameworkElement.WidthProperty);
            ufuaserverEditor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("UFUSREditorSmall");

            workspace.AddDockingChildren(ufuaserverEditor,
                String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(ufuaserverEditor, new ImageBrush(bm));
            workspace.FlashDockedElement(ufuaserverEditor);
            ufuaserverEditor.Document.PropertyChanged += Document_PropertyChanged;
            return ufuaserverEditor;
        }

        private UFUserDocument CreateDoc(Uri uri, IDocument parent, bool bCheckShared = true)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = UFUserDocument.FromFile(fullPath, this, parent, bCheckShared: bCheckShared);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }
#endif
        internal UFUserDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

            UFUserDocument doc = null;
            lock (lockObject)
            {
                if (bRefresh || !mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    if (mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        mapRunningDocuments.Remove(uri.GetPathString());
                        doc.Dispose();
                    }
                    doc = UFUserDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = parent.Parent ?? parent;
                    mapRunningDocuments.Add(uri.GetPathString(), doc);
                }
            }
            return doc;
        }

#if !NET_STANDARD
        internal bool SetNewSharedRepository(UFUserDocument doc, string sharedConnection, bool bSetBackupRepository)
        {
            if (mapActiveDocumentUris.ContainsKey(doc))
            {
                if (bSetBackupRepository)
                    doc.GetGeneralUserSettings().SharedConnectionRepositoryBackup = sharedConnection;
                else
                    doc.GetGeneralUserSettings().SharedConnectionRepository = sharedConnection;

                if (!doc.IsSharedConnectionRepository && !bSetBackupRepository)
                {
                    if (!String.IsNullOrEmpty(sharedConnection))
                        return ReloadSharedRepository(doc, XpoHelper.NormalizeConnectionString(sharedConnection, doc.rootBase));
                }
                else if (doc.IsSharedConnectionRepository)
                {
                    if (!bSetBackupRepository && String.IsNullOrEmpty(sharedConnection))
                        return ReloadLocalRepository(doc);
                    else
                    {
                        var uri = mapActiveDocumentUris[doc];
                        if (!CloseAllChild(doc.Parent))
                            return false;

                        try
                        {
                            Workspace.IsBusy = true;
                            using (var projectDoc = UFUserDocument.FromFile(new Uri(uri, UriKind.RelativeOrAbsolute).GetPathString(), this, doc.Parent, bCreateNew: false, bCheckEmpty: false, bCheckShared: false))
                            {
                                if (projectDoc == null)
                                    return false;

                                if (bSetBackupRepository)
                                    projectDoc.GetGeneralUserSettings().SharedConnectionRepositoryBackup = sharedConnection;
                                else
                                    projectDoc.GetGeneralUserSettings().SharedConnectionRepository = sharedConnection;
                                projectDoc.SaveToFile();
                            }

                            try
                            {
                                UFUserDocument.CopyFile(doc, XpoHelper.NormalizeConnectionString(sharedConnection, doc.rootBase));
                            }
                            catch (Exception ex)
                            { }
                            var newDoc = CreateDoc(new Uri(uri, UriKind.RelativeOrAbsolute), doc.Parent, bCheckShared: true);
                            if (newDoc != null && newDoc.ActiveView == null)
                                CreateDocView(newDoc, GetDocumentTitle(uri), doc.Parent);
                        }
                        catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                        {
                            if (UIInterface != null)
                                UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                            return false;
                        }
                        finally
                        {
                            Workspace.IsBusy = false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        bool ReloadLocalRepository(UFUserDocument doc)
        {
            if (mapActiveDocumentUris.ContainsKey(doc))
            {
                var uri = mapActiveDocumentUris[doc];
                if (!CloseAllChild(doc.Parent))
                    return false;

                try
                {
                    Workspace.IsBusy = true;
                    var newDoc = CreateDoc(new Uri(uri, UriKind.RelativeOrAbsolute), doc.Parent, bCheckShared: false);
                    if (newDoc != null && newDoc.ActiveView == null)
                    {
                        newDoc.GetGeneralUserSettings().SharedConnectionRepository = null;
                        CreateDocView(newDoc, GetDocumentTitle(uri), doc.Parent);
                    }
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return false;
                }
                finally
                {
                    Workspace.IsBusy = false;
                }

                return true;
            }

            return false;
        }

        bool ReloadSharedRepository(UFUserDocument doc, string sharedConnection)
        {
            if (mapActiveDocumentUris.ContainsKey(doc))
            {
                var uri = mapActiveDocumentUris[doc];
                if (!CloseAllChild(doc.Parent))
                    return false;

                MoveToSharedRepository(uri, sharedConnection, doc.Parent);

                try
                {
                    Workspace.IsBusy = true;
                    var newDoc = CreateDoc(new Uri(uri, UriKind.RelativeOrAbsolute), doc.Parent, bCheckShared: true);
                    if (newDoc != null && newDoc.ActiveView == null)
                        CreateDocView(newDoc, GetDocumentTitle(uri), doc.Parent);
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return false;
                }
                finally
                {
                    Workspace.IsBusy = false;
                }

                return true;
            }

            return false;
        }

        void MoveToSharedRepository(String sourcePath, String targetPath, IDocument parent)
        {
            var sharedRepositoryModel = new MergeRepositoryModel();
            var sharedRepositoryView = new MergeRepositoryDialog()
            {
                DataContext = sharedRepositoryModel
            };
            GeneralDialogContent Dialog = new GeneralDialogContent(sharedRepositoryView)
            {
                Title = Properties.Resources.MoveToSharedRepositoryTitle,
                HelpLink = "MoveToSharedRepository"
            };

            if (Dialog.ShowDialog() == true)
            {
                try
                {
                    Workspace.IsBusy = true;
                    UFUserDocument.MergeFile(sourcePath, targetPath, sharedRepositoryModel, parent, this);
                }
                catch (Exception ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.MoveToSharedRepositoryError, targetPath));
                }
                finally
                {
                    Workspace.IsBusy = false;
                }
            }
        }
#endif

        #region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    if (listRunningDocuments.Contains(uri.GetPathString()))
                    {
                        UIInterface.ShowError(Properties.Resources.CannotEditWhileRuntimeRunning);
                        return;
                    }

                    UFUserDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as UFUserEditorControl))
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
#endif
        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            lock (lockObject)
            {
                UFUserDocument doc = null;
                if (mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
#if !NET_STANDARD
                    doc.TerminateAndLogoff();
                    if (doc.IsSharedConnectionRepository && doc.GetGeneralUserSettings(true).UseSharedRepositoryTriggers)
                    {
                        try
                        {
                            doc.RemoveSharedRepositoryTriggers();
                        }
                        catch { }
                    }
#endif
                    doc.Dispose();
                    mapRunningDocuments.Remove(uri.GetPathString());
                }
#if !NET_STANDARD
                if (mapAddressSpaceControls.ContainsKey(parent))
                {
                    mapAddressSpaceControls[parent].Dispose();
                    mapAddressSpaceControls.Remove(parent);
                }

                if (mapRuntimeAddressSpaceControls.ContainsKey(parent))
                {
                    mapRuntimeAddressSpaceControls[parent].Dispose();
                    mapRuntimeAddressSpaceControls.Remove(parent);
                }
#endif
                if (listRunningDocuments.Contains(uri.GetPathString()))
                    listRunningDocuments.Remove(uri.GetPathString());
            }
        }
#if !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values/*.AsParallel()*/
                        where c.Parent == parent && c.ActiveView != null
                        select c).ToList();

            list.ForEach(document =>
            {
                document.SaveToFile();
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values/*.AsParallel()*/
                        where c.Parent == parent && c.ActiveView != null
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseView(document.ActiveView as UFUserEditorControl))
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
#endif
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            lock (lockObject)
            {
                if (!listRunningDocuments.Contains(uri.GetPathString()))
                    listRunningDocuments.Add(uri.GetPathString());

                UFUserDocument doc = null;
#if !NET_STANDARD
                if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                {
                    CloseView(doc.ActiveView as UFUserEditorControl, false);
                }
#endif
                doc = GetOrCreateDocument(parent, bRefresh: true);
                if (doc == null || doc.IsEmpty)
                    return;

#if !NET_STANDARD
                try
                {
                    doc.UpdateSharedRepository();
                    doc.MapToCredentialProvider();
                }
                catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return;
                }

                if (doc.IsSharedConnectionRepository && doc.GetGeneralUserSettings(true).UseSharedRepositoryTriggers)
                {
                    try
                    {
                        doc.AddSharedRepositoryTriggers();
                    }
                    catch (Exception ex)
                    {
                        if (UIInterface != null)
                            UIInterface.ShowError(String.Format(Properties.Resources.ErrorOnConfiguringSharedRepository, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                }
#endif
            }
        }
#if !NET_STANDARD
        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    // if (!bCopy)
                    {
                        UFUserDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as UFUserEditorControl, false);
                    }

                    UFUserDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFUserDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UFUserEditorControl, false);

                    UFUserDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
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
                    UFUserDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UFUserEditorControl, false);

                    UFUserDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()) && 
                !mapActiveDocuments[uri.GetPathString()].IsSharedConnectionRepository)
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = UFUserDocument.FromFile(uri.GetPathString(), this, parent, bCheckShared: false);
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
        {
            var fileCore = String.Format("{0}\\{1}\\{2}{3}{4}",
                                projectPath,
                                TypeLabel,
                                FileName,
                                FileType,
                                Properties.Settings.Default.CoreExt);

            if (System.IO.File.Exists(fileCore))
                System.IO.File.Delete(fileCore);
        }
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
            UFUserDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = CreateDoc(uri, parent);
                if (doc == null)
                    return null;
            }

            var listRoles = doc.GetRoles();
            if (listRoles == null || listRoles.Count == 0)
                return null;

            var list = new ObservableCollection<IDocumentManager>();
            foreach (var role in listRoles)
            {
                list.Add(new TreeDocumentManagers.TreeRolesDocManager(this, doc, role));
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
                return GetBitmapImage("UFUSREditorSmall");
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
                return GetBitmapImage("UFUSREditor");
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string StringPlaceolder
        {
            get { return stringPlaceolder; }
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

#if !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            if (document == null)
                return false;
            var doc = GetOrCreateDocument(document);
            return doc == null || doc.IsDisposed ? false : doc.GetRoles().Count > 0;
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
                return typeof(UFUserDocument);
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

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;

            if (StringEditorManager != null)
                StringEditorManager.CultureChanged -= StringEditor_CultureChanged;
#endif
        }

        #endregion IDisposable Members

        #region IUFUserEditorManager Members

        public IEnumerable<String> GetListRoleNames(IDocument parent, bool bRefresh = false, bool bRuntime = true)
        {
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

            UFUserDocument doc = null;
            if(!bRuntime)
            {
                doc = GetDocument(uri) as UFUserDocument;
#if !NET_STANDARD
                if (doc == null)
                    doc = CreateDoc(uri, parent);
#endif
            }
            else
                doc = GetOrCreateDocument(parent, bRefresh);
            
            if (doc == null)
                return null;

            try
            {
                return doc.GetRoleNames();
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
        public IEnumerable<String> GetListUserNames(IDocument parent, bool bRefresh = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh);
            if (doc == null)
                return null;

            try
            {
                return doc.GetUserNames();
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

        public String GetConnectionStringFromFile(String file)
        {
            var fileBase = UFUserDocument.GetBaseFilename(file
#if !NET_STANDARD
                , null
#endif
                );
            //var ServerFile = String.Format("{0}.Server", fileBase);

            return InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));
        }

        public String GetSharedApplicationName(IDocument parent)
        {
#if !NET_STANDARD
            var doc = GetOrCreateDocument(parent);
            if (doc == null || !doc.IsSharedConnectionRepository)
#endif
                return null;

#if !NET_STANDARD
            try
            {
                return doc.SharedApplicationName;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
#endif
        }

        public DateTime GetLastChangedTime(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return DateTime.MinValue;

            try
            {
                return doc.GetGeneralUserSettings(threadsafe: true).LastChangedTime;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return DateTime.MinValue;
            }
        }

        public String GetUserRole(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetUserRole(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public int GetRoleAccessMask(IDocument parent, String role)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetRoleAccessMask(role);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetRoleAutoLogoutSeconds(IDocument parent, String role)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                if (!String.IsNullOrEmpty(role))
                {
                    var autoLogoutSeconds = doc.GetRoleAutoLogoutSeconds(role);
                    if (autoLogoutSeconds > 0)
                        return autoLogoutSeconds;
                }

                return doc.GetGeneralUserSettings(true).AutoLogoutSeconds.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }


#if !NET_STANDARD
        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUserName = e.User;
        }
#endif

        public bool SetUserRole(IDocument parent, String user, String newRole)
        {
#if !NET_STANDARD
            if (AuthenticationCredentialsProvider == null)
                return false;

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            AuthenticationCredentialsProvider.UserOnline += AuthenticationProvider_UserOnline;
            AuthenticationCredentialsProvider.RefreshCurrentUser(doc.Parent.Title);
            AuthenticationCredentialsProvider.UserOnline -= AuthenticationProvider_UserOnline;

            var userSettings = GetGeneralUserSettings(parent);

            if (String.IsNullOrEmpty(currentUserName) || GetUserAccessLevel(parent, currentUserName) <= userSettings.MaxRuntimeEditAccessLevel)
                return false;

            String oldRole = GetUserRole(parent, user);

            int userAccessLevel = GetUserAccessLevel(parent, user);
            int oldRoleAccessLevel = GetRoleAccessLevel(parent, oldRole);
            int newRoleAccessLevel = GetRoleAccessLevel(parent, newRole);
            var maxEditedAccessLevel = new int[] { userAccessLevel, oldRoleAccessLevel, newRoleAccessLevel }.Max();

            if (userSettings == null || maxEditedAccessLevel > userSettings.MaxRuntimeEditAccessLevel)
                return false;

            UFUser usr;
            UFRole rl;
            UFRole oldRl;
            try
            {
                usr = doc.GetUserByName(user);
                rl = doc.GetRoleByName(newRole);
                oldRl = doc.GetRoleByName(oldRole);
            }
            catch
            {
                return false;
            }

            if (!rl.UFUsers.Contains(usr))
            {
                oldRl.UFUsers.Remove(usr);
                usr.UFRoleAss = rl;
                rl.UFUsers.Add(usr);
                doc.SaveToFile();
            }
            return true;
#else
            return false;
#endif
        }

        public bool VerifyPasswordExpired(IDocument parent, String user, bool bForce = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return true;

            try
            {
                return doc.VerifyPasswordExpired(user, bForce);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return true;
            }
        }

        public int VerifyUser(IDocument parent, String user, String password)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.VerifyUser(user, password);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                //if (UIInterface != null)
                //    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public bool UpdateUser(IDocument parent, String user, String oldpassword, String newpassword)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            try
            {
                return doc.UpdateUser(user, oldpassword, newpassword);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                //if (UIInterface != null)
                //    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
            }

            return false;
        }

        public String GetUserElectronicSignature(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetUserElectronicSignature(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public bool GetIsOldPassword(IDocument parent, String user, String password)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            try
            {
                return doc.GetIsOldPassword(user, password);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public int GetUserAccessLevel(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetUserAccessLevel(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        UFUserSettings GetGeneralUserSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            return doc.GetGeneralUserSettings(true);
        }

        public int GetRoleAccessLevel(IDocument parent, String role)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetRoleAccessLevel(role);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetUserAccessMask(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetUserAccessMask(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetAutoLogoutSeconds(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                if (!String.IsNullOrEmpty(user))
                {
                    var ret = doc.GetAutoLogoutSeconds(user);
                    if (ret > 0)
                        return ret;
                }

                return doc.GetGeneralUserSettings(true).AutoLogoutSeconds.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetDaysLeftPasswordExpires(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetDaysLeftPasswordExpires(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetMinRequiredPasswordLength(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetGeneralUserSettings(true).MinRequiredPasswordLength.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public int GetMaxInvalidPasswordAttempts(IDocument parent, bool bRefresh = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetGeneralUserSettings(true).MaxInvalidPasswordAttempts.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public bool GetEnableUserManager(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            try
            {
                return doc.GetGeneralUserSettings(true).EnableUserManager;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public bool GetLoginControlVisible(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;

            try
            {
                return doc.GetGeneralUserSettings(true).LoginControlVisible;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public String GetDesktopSystemRole(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            try
            {
                return doc.GetGeneralUserSettings(true).DesktopSystemRole;
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

        public String GetUserCultureName(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetUserCultureName(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetRoleCultureName(IDocument parent, String role)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetRoleCultureName(role);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetUserConverterName(IDocument parent, String user)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetUserConverterName(user);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetRoleConverterName(IDocument parent, String role)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;

            try
            {
                return doc.GetRoleConverterName(role);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

#if !NET_STANDARD
        readonly Dictionary<IDocument, UFUserEditorControl> mapAddressSpaceControls = new Dictionary<IDocument, UFUserEditorControl>();
        public UserControl GetUserEditControl(IDocument parent, bool bRefresh = false)
        {
            if (!bRefresh && mapAddressSpaceControls.ContainsKey(parent))
                return mapAddressSpaceControls[parent];
            else if (mapAddressSpaceControls.ContainsKey(parent))
            {
                mapAddressSpaceControls[parent].Dispose();
                mapAddressSpaceControls.Remove(parent);
            }

            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            UFUserDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return null;

            doc = GetOrCreateDocument(parent, bRefresh);
            if (doc == null)
                return null;

            try
            {
                if (!mapAddressSpaceControls.ContainsKey(parent))
                    mapAddressSpaceControls.Add(parent, new UFUserEditorControl(doc, true));
                return mapAddressSpaceControls[parent];
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        readonly Dictionary<IDocument, UFRuntimeUserEditorControl> mapRuntimeAddressSpaceControls = new Dictionary<IDocument, UFRuntimeUserEditorControl>();
        public UserControl GetRuntimeUserEditControl(IDocument parent)
        {
            if (mapRuntimeAddressSpaceControls.ContainsKey(parent))
            {
                mapRuntimeAddressSpaceControls[parent].Dispose();
                mapRuntimeAddressSpaceControls.Remove(parent);
            }

            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            UFUserDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return null;

            doc = GetOrCreateDocument(parent, true);
            if (doc == null)
                return null;
            
            try
            {
                if (!mapRuntimeAddressSpaceControls.ContainsKey(parent))
                    mapRuntimeAddressSpaceControls.Add(parent, new UFRuntimeUserEditorControl(doc));
                return mapRuntimeAddressSpaceControls[parent];
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public bool SaveAndRelease(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            if (!doc.NeedsSave)
                return false;

            var ret = doc.SaveToFile();
            if (ret == true)
            {
                doc.SaveToFile(discargechanges: true);
                doc.MapToCredentialProvider();

                var e = new UserSettingsChangedArg(parent);
                OnUserSettingsChanged(doc, e);
            }
            return ret;
        }
#endif

#if !NET_STANDARD
        public void EnsureCredentialProvider(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null || doc.IsEmpty)
                return;

            try
            {
                doc.MapToCredentialProvider();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return;
            }
        }

        public event EventHandler<UserSettingsChangedArg> UserSettingsChanged;

        virtual public void OnUserSettingsChanged(object sender, UserSettingsChangedArg e)
        {
            EventHandler<UserSettingsChangedArg> temp = UserSettingsChanged;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler<VerifyPasswordEventArgs> PromptVerifyPassword;
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        internal void OnPromptVerifyPassword(VerifyPasswordEventArgs loginInfo)
        {
            var e = PromptVerifyPassword;
            if (e != null)
            {
                e(this, loginInfo);
            }
        }

        public Type GetAccessRoleEditorType()
        {
            return typeof(AccessRolePropertyEditor);
        }
#endif
        public string GetExternalAuthenticationUserRole(IDocument parentDocument, string externalRole)
        {
            var doc = GetOrCreateDocument(parentDocument, false);
            if (doc == null)
                return null;

            return doc.GetExternalAuthenticationUserRole(externalRole);
        }

        #endregion

        public UserLockType GetUserLockMode(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return UserLockType.None;

            try
            {
                return doc.GetGeneralUserSettings(true).UserLockMode.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return UserLockType.None;
            }
        }

        public int GetMaxRuntimeEditAccessLevel(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return 0;

            try
            {
                return doc.GetGeneralUserSettings(true).MaxRuntimeEditAccessLevel.Value;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return 0;
            }
        }

        public IEnumerable<object> GetRoles(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            if (doc == null)
                return null;

            try
            {
                return doc.GetRoles();
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
        public Dictionary<String, String> GetUsersEmail(IDocument parent, String NodeId)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            try
            {
                return doc.GetUsersEmail(NodeId);
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

        public IList<String> GetSMTPSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            try
            {
                return doc.GetSMTPSettings();
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

        public Dictionary<String, String> GetExternalAuthenticationSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            try
            {
                return doc.GetExternalAuthenticationSettings();
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
#endif
    }
}