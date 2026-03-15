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
using System.Windows.Input;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Controls;
using UFInterfaces.AuthenticationCredentialsProvider;
using VFS;
using HelpProvider.ComponentService;
using PropertyControl.ComponentService;
using UFRecipeEditor.PropertyDataTemplate;
using WPFUtilities.PropertyDataTemplate;
using OPCUAViewModel.PropertyDataTemplate;
using UFInterfaces.Editors;
using Utilities.WPF;
using WPFUtilities.Extensions;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using UFUAEditor.ComponentService;
using UFRecipeSettings.Documents;
using log4net;
using UFRecipeSettings;
using UFRecipeSettings.UFRecipeModel;
using StringManager.ComponentService;
using UFProjectManager.ComponentService;
using OPCUAViewModel;
using Utilities.Logger;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;
using XpoHelpers;
using DevExpress.Xpo;
using UFUserEditor.ComponentService;
using UFInterfaces.AuditTrace;

namespace UFRecipeEditor.ComponentService
{
    public class RecipeEditorManagerComponent : ComponentBase<IRecipeEditorManager>, IRecipeEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
#region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UFRecipeDocument> mapActiveDocuments = new Dictionary<String, UFRecipeDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFRecipeDocument, String> mapActiveDocumentUris = new Dictionary<UFRecipeDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<String, RecipeUAServerDocument> mapActiveUADocuments = new Dictionary<String, RecipeUAServerDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<RecipeUAServerDocument, String> mapActiveUADocumentUris = new Dictionary<RecipeUAServerDocument, String>();

        readonly Dictionary<String, RecipeUAServerDocument> mapRunningDocuments = new Dictionary<String, RecipeUAServerDocument>(StringComparer.OrdinalIgnoreCase);
        readonly List<RecipeUAServerDocument> listStartedDocuments = new List<RecipeUAServerDocument>();

        readonly Dictionary<UFRecipeDocument, UFRecipeExecuter.UFRecipeExecuter> mapRunningRecipes = new Dictionary<UFRecipeDocument, UFRecipeExecuter.UFRecipeExecuter>();
        readonly Dictionary<IDocument, List<UFRecipeExecuter.UFRecipeExecuter>> mapRunningRecipesPerParent = new Dictionary<IDocument, List<UFRecipeExecuter.UFRecipeExecuter>>();
        readonly Dictionary<String, int> mapRunningRecipesUriCounter = new Dictionary<String, int>();

#if !NET_STANDARD
        MenuControl menuControl;
        List<CommandBinding> globalCbs;

        public static RecipeEditorManagerComponent recipeEditorManagerComponent { get; protected set; }
#endif

        private static readonly ILog logLicense = Logger.GetDestinationLog(LoggerDestination.License);
        private static readonly ILog logRecipe = Logger.GetDestinationLog(LoggerDestination.RecipeService);
#endregion Declaration

#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
#if !NET_STANDARD
            if (recipeEditorManagerComponent == null)
                recipeEditorManagerComponent = this;

            GetComponentInterfaces();
#endif
        }

#endregion IUFInterfaceBase Members

#region Public Events
        public event EventHandler<BalloonEventArgs> BalloonEvent;
        public void OnBalloonEvent(String message, System.Windows.Forms.ToolTipIcon icon)
        {
            var temp = BalloonEvent;
            if (temp != null)
            {
                BalloonEventArgs e = new BalloonEventArgs() { Message = message, Icon = icon };
                temp(this, e);
            }
        }
        #endregion
#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(UFRecipeSettings.Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }

        private void GetComponentInterfaces()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = UFRecipeSettings.Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
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
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                dt.DataType = typeof(ConnectionSourceReference);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(ConnectionSourceReference), dt);
                PropertyControl.AddPropertyEditor("ConnectionString", typeof(string), typeof(UFRecipeSettings.UFRecipeModel.UFRecipeEntity), dt);      

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DynamicSettingsPropertyEditor));
                factory.SetValue(DynamicSettingsPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("StartingAddress", typeof(string), typeof(UFRecipeEntity), dt);
                PropertyControl.AddPropertyEditor("StartingAddress", typeof(string), typeof(UFGroupEntity), dt);
                PropertyControl.AddPropertyEditor("StartingAddress", typeof(string), typeof(UFDataValueEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Description", typeof(string), typeof(UFRecipeEntity), dt);
                PropertyControl.AddPropertyEditor("Description", typeof(string), typeof(UFGroupEntity), dt);
                PropertyControl.AddPropertyEditor("Description", typeof(string), typeof(UFDataValueEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Header", typeof(Object), typeof(DevExpress.Xpf.LayoutControl.LayoutGroup), dt);
                PropertyControl.AddPropertyEditor("Label", typeof(Object), typeof(DevExpress.Xpf.LayoutControl.LayoutItem), dt);
                PropertyControl.AddPropertyEditor("Caption", typeof(string), typeof(UFRecipeLayout.LayoutItemControls.RecipeCommandButtonLayoutItem), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(EnumOptionsPropertyEditor));
                factory.SetValue(EnumOptionsPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String[]);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("EnumOptions", typeof(String[]), typeof(UFRecipeLayout.LayoutItemControls.RecipeEditValueLayoutItem), dt);
                PropertyControl.AddPropertyEditor("EnumOptions", typeof(String[]), typeof(UFRecipeSettings.UFRecipeModel.UFDataValueEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                dt.DataType = typeof(Int32);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("DecimalDigits", typeof(Int32), typeof(UFRecipeLayout.LayoutItemControls.RecipeEditValueLayoutItem), dt);
                PropertyControl.AddPropertyEditor("DecimalDigits", typeof(Int32), typeof(UFRecipeSettings.UFRecipeModel.UFDataValueEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(EngineeringUnitNamePropertyEditor));
                factory.SetValue(EngineeringUnitNamePropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("EngineeringUnit", typeof(string), typeof(UFDataValueEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditor));
                factory.SetValue(OPCUAEntityReferencePropertyEditor.AllowDataSyncProperty, false);
                dt.DataType = typeof(OPCUAEntityReference);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(OPCUAEntityReference), typeof(UFRecipeEntity), dt);
                PropertyControl.AddPropertyEditor(typeof(OPCUAEntityReference), typeof(UFDataValueEntity), dt);

                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }
        }

        void PropertyControl_AcceptChanges(object sender, EventArgs e)
        {
            bool showWarning = false;
            using (var cursor = new WaitCursor())
            {
                var list = (from c in mapActiveDocuments.Values// .AsParallel()
                            where c.NeedsSave == true && c.ActiveView == null
                            select c).ToList();

                list.ForEach(doc =>
                {
                    showWarning |= doc.NeedsRebuild;
                    doc.SaveToFile();
                });
            }

            if (showWarning && UIInterface != null)
                UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);
        }

        private bool CloseView(RecipeUAEditorControl view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveUADocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!CanClose(view, bSave))
                {
                    return false;
                }

                CloseAllChild(view.Document);

                mapActiveUADocumentUris.Remove(view.Document);
                mapActiveUADocuments.Remove(uri);
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

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveUADocuments.Count != 0)
            {
                var array = new RecipeUAServerDocument[mapActiveUADocuments.Values.Count];
                mapActiveUADocuments.Values.CopyTo(array, 0);
                foreach (var doc in array)
                {
                    var view = doc.ActiveView as RecipeUAEditorControl;
                    if (view != null && !CanClose(view))
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
            if (!e.Cancel && mapActiveDocuments.Count != 0)
            {
                var array = new UFRecipeDocument[mapActiveDocuments.Values.Count];
                mapActiveDocuments.Values.CopyTo(array, 0);
                foreach (var doc in array)
                {
                    var view = doc.ActiveView as UFRecipeEditorUI;
                    if (view != null && !CanClose(view))
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveUADocuments.Count != 0)
            {
                var array = new RecipeUAServerDocument[mapActiveUADocuments.Values.Count];
                mapActiveUADocuments.Values.CopyTo(array, 0);
                foreach (var doc in array)
                {
                    var view = doc.ActiveView as RecipeUAEditorControl;
                    if (view != null)
                        CloseView(view, false);
                }
            }

            if (mapActiveDocuments.Count != 0)
            {
                var array = new UFRecipeDocument[mapActiveDocuments.Values.Count];
                mapActiveDocuments.Values.CopyTo(array, 0);
                foreach (var doc in array)
                {
                    var view = doc.ActiveView as UFRecipeEditorUI;
                    if (view != null)
                        CloseRecipe(view, false);
                }
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (e.TargetItem is RecipeUAEditorControl)
            {
                var view = e.TargetItem as RecipeUAEditorControl;
                if (!CloseView(view))
                    e.Cancel.Cancel = true;
            }
            else if (e.TargetItem is UFRecipeEditorUI)
            {
                var view = e.TargetItem as UFRecipeEditorUI;
                if (!CloseRecipe(view))
                    e.Cancel.Cancel = true;
            }
        }

        private bool CanClose(RecipeUAEditorControl view, bool bSave = true)
        {
            String uri;
            if (mapActiveUADocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!view.Document.CanClose())
                    return false;

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

        private bool CanClose(UFRecipeEditorUI view, bool bSave = true)
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
                        if (view.Document.NeedsRebuild && UIInterface != null)
                            UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);

                        if (!view.Document.SaveToFile())
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseRecipe(UFRecipeEditorUI view, bool bSave = true, bool bDispose = true)
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
                        if (view.Document.NeedsRebuild && UIInterface != null)
                            UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);

                        view.SaveRecipeLayout();
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

        void GetGlobalCbs(UFRecipeEditorUI view)
        {
            var globalCmds = (from DevExpress.Xpf.Bars.BarItem bi in menuControl.menuGeneralItems.GetChildrenOfType<DevExpress.Xpf.Bars.BarItem>() where bi.Command != null select bi.Command).ToList();
            globalCbs = (from CommandBinding cb in view.CommandBindings where globalCmds.Contains(cb.Command) select cb).ToList();
        }

        void GetGlobalCbs(RecipeUAEditorControl view)
        {
            var globalCmds = (from DevExpress.Xpf.Bars.BarItem bi in menuControl.menuGeneralItems.GetChildrenOfType<DevExpress.Xpf.Bars.BarItem>() where bi.Command != null select bi.Command).ToList();
            globalCbs = (from CommandBinding cb in view.CommandBindings where globalCmds.Contains(cb.Command) select cb).ToList();
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable<UserControl> viewOld = null;
            if (e.OldValue is UFRecipeEditorUI)
                viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            else if (e.OldValue is RecipeUAEditorControl)
                viewOld = from entry in mapActiveUADocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            
            IEnumerable<UserControl> viewNew = null;
            if (e.NewValue is UFRecipeEditorUI)
                viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;
            else if (e.NewValue is RecipeUAEditorControl)
                viewNew = from entry in mapActiveUADocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is UFRecipeEditorUI && viewOld != null)
            {
                var view = e.OldValue as UFRecipeEditorUI;

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
            if (e.OldValue != null && e.OldValue is RecipeUAEditorControl && viewOld != null)
            {
                var view = e.OldValue as RecipeUAEditorControl;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.RemoveBarManagerCommands(new CommandBindingCollection(globalCbs));

                //view.OnDeactivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UFRecipeEditorUI && viewNew != null)
            {
                var view = e.NewValue as UFRecipeEditorUI;

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
            if (e.NewValue != null && e.NewValue is RecipeUAEditorControl && viewNew != null)
            {
                var view = e.NewValue as RecipeUAEditorControl;

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

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (!(sender is FrameworkElement) &&
                !(sender is IXPSimpleObject) &&
                !(sender is UFRecipeSettings.UFRecipeModel.UFRecipeEntity) &&
                !(sender is UFRecipeSettings.UFRecipeModel.UFGroupEntity) && 
                !(sender is UFRecipeSettings.UFRecipeModel.UFDataValueEntity))
                return;

            if (sender is IXPSimpleObject)
            {
                IXPSimpleObject source = sender as IXPSimpleObject;
                foreach (var Doc in mapActiveUADocumentUris.Keys)
                {
                    if (Doc.ActiveView == null || Doc.UowContext == null || !XpoHelper.IsSessionObject(source, Doc.UowContext))
                        continue;

                    e.documentEditor = Doc.ActiveView;
                    break;
                }
            }
            else
            {
                foreach (var Doc in mapActiveDocumentUris.Keys)
                {
                    var recipeEditor = Doc.ActiveView as UFRecipeEditorUI;
                    if (recipeEditor == null || !recipeEditor.IsChildElement(sender))
                        continue;

                    e.documentEditor = Doc.ActiveView;
                    break;
                }
            }
        }

        void workspace_PromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            if (!(sender is UFRecipeDocument))
                return;
            UFRecipeDocument element = sender as UFRecipeDocument;

            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.GetFriendObjects(element, e))
                    break;
            }
        }
#endif

        void executer_StateChanged(object sender, UFRecipeExecuter.StateChangedArgs e)
        {
            var oldError = e.oldState & UFRecipeExecuter.RecipeExecutionStateEnum.ErrorMaskFlag;
            var newError = e.newState & UFRecipeExecuter.RecipeExecutionStateEnum.ErrorMaskFlag;

            if (oldError == 0 && newError != 0)
            {
                var msg = String.Format(Properties.Resources.RecipeErrorStateEntered, e.recipeName, newError);
                OnBalloonEvent(msg, System.Windows.Forms.ToolTipIcon.Error);
            }
            else if (oldError != 0 && newError == 0)
            {
                var msg = String.Format(Properties.Resources.RecipeErrorStateExited, e.recipeName, oldError);
                OnBalloonEvent(msg, System.Windows.Forms.ToolTipIcon.Info);
            }

            logRecipe.Info(String.Format(Properties.Resources.RecipeChangeStateEventLog, e.recipeName, e.oldState, e.newState));
        }

        String GetDocumentTitle(Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

#if !NET_STANDARD
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

        internal UFRecipeEditorUI GetViewFromUri(Uri uri)
        {
            UFRecipeDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UFRecipeEditorUI;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                if (sender is UFRecipeDocument)
                {
                    foreach (KeyValuePair<String, UFRecipeDocument> keyvaluepair in mapActiveDocuments)
                    {
                        if (keyvaluepair.Value != sender)
                            continue;

                        workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                        break;
                    }
                }
                else if (sender is RecipeUAServerDocument)
                {
                    foreach (KeyValuePair<String, RecipeUAServerDocument> keyvaluepair in mapActiveUADocuments)
                    {
                        if (keyvaluepair.Value != sender)
                            continue;

                        workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                        break;
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CreateDefaultDocument(Uri uri, IDocument parent, bool encryptFile = false)
        {
            using (var newProject = new UFRecipeDocument() { FullPath = uri.GetPathString(), Parent = parent})
            {
                newProject.SaveToFile(forceEncryption: encryptFile);
            }
        }

        internal bool IsLayoutEditorHidden(IDocument parent)
        {
            if (UFProjectManager != null)
            {
                var originalVersion = UFProjectManager.GetOriginalVersion(parent);

                Version version;
                if (Version.TryParse(originalVersion, out version))
                {
                    try
                    {
                        var hideVersion = new Version(Properties.Settings.Default.HideLayoutEditorVersion);
                        return version >= hideVersion;
                    }
                    catch
                    { }
                }
            }

            return false;
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
#endif

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

        IUFProjectManager ufprojectManager;
        public IUFProjectManager UFProjectManager
        {
            get
            {
                if (ufprojectManager == null)
                    ufprojectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                return ufprojectManager;
            }
        }
#endif

        #endregion Properties
#if !NET_STANDARD
        private UFRecipeEditor.UFRecipeEditorUI CreateDocView(UFRecipeDocument doc, String title, IDocument parent)
        {
            var editor = new UFRecipeEditorUI(this, doc);
            doc.ActiveView = editor;

            workspace.SetDesiredHeightAndWidthInDockedMode(editor, editor.Height, editor.Width);
            editor.ClearValue(FrameworkElement.WidthProperty);
            editor.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = GetBitmapImage("RCPMEditorSmall");

            workspace.AddDockingChildren(editor,
                String.Format("{0} ({1})", title, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
            workspace.SetDockedElementIcon(editor, new ImageBrush(bm));
            workspace.FlashDockedElement(editor);
            editor.Document.PropertyChanged += Document_PropertyChanged;
            return editor;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFRecipeDocument CreateDoc(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = UFRecipeDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFRecipeDocument CreateDocEx(Uri uri, IDocument parent)
        {
            var fullPath = uri.GetPathString();
            if (mapActiveDocuments.ContainsKey(fullPath))
                return mapActiveDocuments[fullPath];

            var doc = UFRecipeDocument.FromFile(fullPath, parent);
            if (doc == null)
                return null;
            doc.Parent = parent;

            mapActiveDocuments.Add(fullPath, doc);
            mapActiveDocumentUris.Add(doc, fullPath);
            return doc;
        }
#endif

        internal RecipeUAServerDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, RecipeUAServerDocument> map = mapActiveUADocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                RecipeUAServerDocument doc = null;
                // if (!mapActiveUADocuments.TryGetValue(uri.GetPathString(), out doc))
                if (list.Count == 0 || bRefresh)
                {
                    doc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = p;
                    if (!map.ContainsKey(uri.GetPathString()))
                        map.Add(uri.GetPathString(), doc);
                }
                else
                    doc = list[0];
                return doc;
            }
        }

        bool IsRecipeUri(Uri uri)
        {
            var path = uri.GetPathString();
            return path.ToLower().EndsWith(UFRecipeSettings.Properties.Settings.Default.DefaultFileExt.ToLower());
        }

        #region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    if (!IsRecipeUri(uri))
                    {
                        RecipeUAServerDocument doc = null;
                        if (mapActiveUADocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                        {
                            if (doc.Parent == parent)
                                workspace.ActivateDockedElement(doc.ActiveView);
                            else
                            {
                                if (!CloseView(doc.ActiveView as RecipeUAEditorControl))
                                    return;
                                doc = null;
                            }
                        }

                        if (doc == null)
                            doc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent);

                        if (doc != null && doc.ActiveView == null)
                        {
                            var recipeUAEditor = new RecipeUAEditorControl(doc);
                            doc.Parent = parent;
                            doc.ActiveView = recipeUAEditor;
                            if (doc.NeedsSave)
                                doc.SaveToFile();

                            workspace.SetDesiredHeightAndWidthInDockedMode(recipeUAEditor, recipeUAEditor.Height, recipeUAEditor.Width);
                            recipeUAEditor.ClearValue(FrameworkElement.WidthProperty);
                            recipeUAEditor.ClearValue(FrameworkElement.HeightProperty);

                            BitmapImage bm = GetBitmapImage("SSEditorSmall");

                            if (!mapActiveUADocuments.ContainsKey(uri.GetPathString()))
                                mapActiveUADocuments.Add(uri.GetPathString(), doc);

                            if (!mapActiveUADocumentUris.ContainsKey(doc))
                                mapActiveUADocumentUris.Add(doc, uri.GetPathString());

                            workspace.AddDockingChildren(recipeUAEditor,
                                String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                            workspace.SetDockedElementIcon(recipeUAEditor, new ImageBrush(bm));

                            workspace.FlashDockedElement(recipeUAEditor);

                            recipeUAEditor.Document.PropertyChanged += Document_PropertyChanged;
                        }
                    }
                    else
                    {
                        UFRecipeDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                        {
                            if (doc.Parent == parent)
                                workspace.ActivateDockedElement(doc.ActiveView);
                            else
                            {
                                if (!CloseRecipe(doc.ActiveView as UFRecipeEditorUI))
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
                    if (!IsRecipeUri(uri))
                    {
                        RecipeUAServerDocument doc = null;
                        if (mapActiveUADocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as RecipeUAEditorControl, false);

                        RecipeUAServerDocument.RemoveFile(uri.GetPathString(), parent, this);
                    }
                    else
                    {
                        UFRecipeDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        {
                            if (doc.ActiveView is UFRecipeEditorUI)
                                CloseRecipe(doc.ActiveView as UFRecipeEditorUI, false);
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

                        UFRecipeDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                    }
                }
            }
        }
#endif
        public void PreSubscribeServerSession(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            try
            {
                doc.PreSubscribeServerSession(parent.Title);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                logRecipe.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return;
            }
        }

        public void UnsubscribeServerSession(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
            try
            {
                doc.UnsubscribeServerSession();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                logRecipe.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return;
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            if (uri == null)
                return;

            if (!IsRecipeUri(uri))
            {
                RecipeUAServerDocument serverDoc = null;
                if (!mapRunningDocuments.TryGetValue(uri.GetPathString(), out serverDoc))
                {
                    serverDoc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent, bCreateNew: false);
#if !NET_STANDARD
                    if (serverDoc == null && UFProjectManager != null && UFProjectManager.GetResourceList(parent, TypeScheme).Count() > 0)
                    {
                        serverDoc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent, bCreateNew: true);
                        if (serverDoc != null)
                            serverDoc.SaveToFile(bForceSave: true);
                    }
#endif
                    if (serverDoc == null)
                        return;
                    serverDoc.Parent = parent;
                    mapRunningDocuments.Add(uri.GetPathString(), serverDoc);
                }
                else if (serverDoc.IsEmpty)
                    return;
                serverDoc.PreSubscribeServerSession(parent.Title);

#if !NET_STANDARD
                Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
                if (commandArgs.ArgPairs.ContainsKey("client"))
                    return;

                if (!serverDoc.ServerCMSHelperAsync.IsServerRunning)
                {
                    try
                    {
                        if (serverDoc.StartServer(/*startupControl.txtevent, startupControl.Scroll, */bSave: false))
                        {
                            var dateTime = DateTime.Now.AddSeconds(60);
                            while (!serverDoc.ServerCMSHelperAsync.IsServerRunning && dateTime > DateTime.Now)
                                WaitForPriority.DoEventsSync();
                            while (!serverDoc.ServerCMSHelperAsync.IsServerStarted && dateTime > DateTime.Now && serverDoc.ServerCMSHelperAsync.IsServerStartedManually)
                                WaitForPriority.DoEventsSync();

                            if (!listStartedDocuments.Contains(serverDoc))
                                listStartedDocuments.Add(serverDoc);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                                UFUAServerInfo.UFUAServerInfo.GetServerName(), ex.Message));
                        }
                    }
                }
#endif
            }
            else
            {
                var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                uri = rootParent.MakeAbosoluteUri(uri);
                var newParent = parent.UpdateParentFromUri(uri);
                UFRecipeDocument doc = null;
                UFRecipeExecuter.UFRecipeExecuter currentexecuter = null;
                lock (lockObject)
                {
                    if (mapRunningRecipesPerParent.ContainsKey(rootParent))
                    {
                        var executers = (from c in mapRunningRecipesPerParent[rootParent]
                                         where c.RecipeDocument.FullPath == uri.GetPathString()
                                         select c).ToList();
                        if (executers.Count > 0)
                            currentexecuter = executers[0];
                    }
                }

                if (currentexecuter != null)
                    doc = currentexecuter.RecipeDocument;
                else
                {
                    if (!UFRecipeDocument.ExistFile(uri.GetPathString()
#if !NET_STANDARD
                    , newParent.fileSystemProviderBase
#endif
                    ))
                    {
#if !NET_STANDARD
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(String.Format(Properties.Resources.DocNotFound,
                                GetDocumentTitle(uri)));
                        }
#else
                        logRecipe.ErrorFormat(Properties.Resources.DocNotFound, GetDocumentTitle(uri));
#endif
                        return;
                    }

                    doc = UFRecipeDocument.FromFile(uri.GetPathString(), newParent);
                    if (doc == null)
                        return;
                    doc.Parent = rootParent;
                }

#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxLpZ9HLAzv1ul2RsoVlDyXw=="/* RCP */);
                if (state == false)
                {
                    logLicense.Warn(Properties.Resources.NoRecipeLicense);
                    return;
                }
#endif

                UFRecipeExecuter.UFRecipeExecuter executer = null;
                if (!mapRunningRecipes.TryGetValue(doc, out executer))
                {
                    var connectorType = UFRecipeExecuter.OPCUA.ConnectorType.None;
                    if (mode != ExecutionMode.Synchro)
                        connectorType = UFRecipeExecuter.OPCUA.ConnectorType.UseStandardMethods;
                    var clientSessionName = Context as String;
#if !NET_STANDARD
                    var recipeRedundancyInitializationContext = Context as UFRecipeExecuter.TransactionLog.RecipeRedundancyInitializationContext;
                    executer = new UFRecipeExecuter.UFRecipeExecuter(doc, connectorType, clientSessionName, recipeRedundancyInitializationContext);
#else
                    executer = new UFRecipeExecuter.UFRecipeExecuter(doc, connectorType, clientSessionName);
#endif
                    executer.StateChanged += executer_StateChanged;
                    executer.Initialize();
                    mapRunningRecipes.Add(doc, executer);
                }

                lock (lockObject)
                {
                    if (!mapRunningRecipesPerParent.ContainsKey(rootParent))
                        mapRunningRecipesPerParent.Add(rootParent, new List<UFRecipeExecuter.UFRecipeExecuter>());
                    if (!mapRunningRecipesPerParent[rootParent].Contains(executer))
                        mapRunningRecipesPerParent[rootParent].Add(executer);
                }

                if (mode == ExecutionMode.Shared)
                {
                    var key = uri.GetPathString();
                    if (!mapRunningRecipesUriCounter.ContainsKey(key))
                        mapRunningRecipesUriCounter.Add(key, 0);
                    var counter = mapRunningRecipesUriCounter[key];
                    mapRunningRecipesUriCounter[key] = counter + 1;
                }

                var executionContext = Context as UFRecipeExecutionContext.RecipeExecutionContext;
#if !NET_STANDARD
                if ((IsLayoutEditorHidden(parent) || executer.RecipeDocument.IsLayoutEmpty) && executionContext != null && executionContext.CommandType == UFRecipeExecutionContext.RecipeCommandType.Show)
                {
                    var title = String.IsNullOrEmpty(executer.RecipeDocument.RecipeEntity.Description) ? executer.RecipeDocument.RecipeEntity.Name : executer.RecipeDocument.RecipeEntity.Description;
                    if (StringEditor != null)
                    {
                        var map = StringEditor.GetListStringForCulture(executer.RecipeDocument, StringEditor.GetActiveCulture(executer.RecipeDocument));
                        if (map != null && map.Count > 0 && map.ContainsKey(title))
                            title = map[title];
                    }

                    UserControl activeView = parent.ActiveView;
                    var recipeGrid = new RecipeViewerControl.RecipeGrid(parent)
                    {
                        RecipeName = uri
                    };
                    var dialog = new GeneralDialogContent(recipeGrid, GeneralDialogButtons.OkCancelButtons, bhandleEnterKey: false)
                    {
                        Owner = (activeView == null ? null : activeView.FindParent<Window>() ?? Application.Current.MainWindow),
                        Title = title
                    };
                    dialog.Closing += (s, e) =>
                    {
                        var bCheckPendingChanges = dialog.DialogResult == true;
                        if (!recipeGrid.CanClose(bCheckPendingChanges))
                            e.Cancel = true;
                    };
                    dialog.ShowDialog();
                }
                else
#endif
                if (executionContext != null)
                {
#if !NET_STANDARD
                    if (executer.RecipeUAViewModel != null &&
                        executer.RecipeUAViewModel.IsAuditTraceEnabled &&
                        executer.RecipeUAViewModel.IsCommentRequired &&
                        (executionContext.CommandType == UFRecipeExecutionContext.RecipeCommandType.Save ||
                        executionContext.CommandType == UFRecipeExecutionContext.RecipeCommandType.Remove ||
                        executionContext.CommandType == UFRecipeExecutionContext.RecipeCommandType.Activate))
                    {
                        var control = executionContext.Control as UIElement;
                        if (control != null)
                        {
                            executionContext.IsAuditTrace = true;
                            executer.Execute(newParent, ExecutionMode.Synchro, executionContext);
                            control.Dispatcher.InvokeIfRequired(() => 
                            {
                                var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(executer.RecipeUAViewModel)
                                {
                                    Control = control,
                                    RecipeIndex = executionContext.Index,
                                    RecipeCommand = executionContext.CommandType
                                };
                                auditTraceViewModel.Execute += (s, e) =>
                                {
                                    var context = new UFRecipeExecutionContext.RecipeExecutionContext()
                                    {
                                        IsSynchro = true,
                                        IsAuditTrace = true,
                                        Index = executionContext.Index,
                                        CommandType = executionContext.CommandType,
                                        Timeout = executionContext.Timeout,
                                        FilePathName = executionContext.FilePathName,
                                        Values = executionContext.Values,
                                        UserComment = auditTraceViewModel.AuditComment,
                                    };
                                    executer.Execute(newParent, mode, context);
                                    if (context.Exception != null)
                                        throw context.Exception;
                                };
                                auditTraceViewModel.AskUserComment();
                            });
                        }
                    }
                    else
#endif
                    {
                        executer.Execute(newParent, mode, executionContext);
                    }
                }
            }
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            List<UFRecipeExecuter.UFRecipeExecuter> recipeExecuters = null;
            lock (lockObject)
            {
                if (mapRunningRecipesPerParent.ContainsKey(rootParent))
                    recipeExecuters = new List<UFRecipeExecuter.UFRecipeExecuter>(mapRunningRecipesPerParent[rootParent]);
            }

            if (!IsRecipeUri(uri))
            {
                if (recipeExecuters != null)
                {
                    recipeExecuters.ForEach(executer =>
                    {
                        executer.StateChanged -= executer_StateChanged;
                        executer.Dispose();
                        var docs = (from c in mapRunningRecipes where c.Value == executer select c.Key).ToList();
                        if (docs.Count > 0)
                        {
                            mapRunningRecipes.Remove(docs[0]);
                            docs[0].Dispose();
                        }
                    });

                    lock (lockObject)
                        mapRunningRecipesPerParent.Remove(rootParent);
                }

                mapRunningRecipesUriCounter.Clear();
                UFRecipeExecuter.UFRecipeExecuter.ClearCheckedRecipes();

                RecipeUAServerDocument serverDoc = null;
                if (mapRunningDocuments.TryGetValue(uri.GetPathString(), out serverDoc))
                {
                    if (listStartedDocuments.Contains(serverDoc))
                    {
                        listStartedDocuments.Remove(serverDoc);
#if !NET_STANDARD
                        if (!serverDoc.ServerCMSHelperSync.IsServerRunningAsService)
                            serverDoc.ServerCMSHelperSync.StopServer();
#endif
                    }
                    mapRunningDocuments.Remove(uri.GetPathString());
                    serverDoc.UnsubscribeServerSession();
                    serverDoc.Dispose();
                }
            }
            else
            {
                uri = rootParent.MakeAbosoluteUri(uri);
                parent = parent.UpdateParentFromUri(uri);

                int counter = 0;
                var key = uri.GetPathString();
                if (mapRunningRecipesUriCounter.ContainsKey(key))
                {
                    counter = mapRunningRecipesUriCounter[key] - 1;
                    if (counter >= 0)
                        mapRunningRecipesUriCounter[key] = counter;
                }

                if (counter == 0 && recipeExecuters != null)
                {
                    var executers = (from c in recipeExecuters
                                     where c.RecipeDocument.FullPath == uri.GetPathString()
                                     select c).ToList();
                    if (executers.Count > 0)
                    {
                        executers[0].StateChanged -= executer_StateChanged;
                        executers[0].Dispose();
                        lock (lockObject)
                            mapRunningRecipesPerParent[rootParent].Remove(executers[0]);

                        var docs = (from c in mapRunningRecipes where c.Value == executers[0] select c.Key).ToList();
                        if (docs.Count > 0)
                        {
                            mapRunningRecipes.Remove(docs[0]);
                            docs[0].Dispose();

                            UFRecipeExecuter.UFRecipeExecuter.ClearCheckedRecipes(docs[0]);
                        }
                    }
                }
            }
        }

#if !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            bool showWarning = false;
            list.ForEach(document =>
            {
                showWarning |= document.NeedsRebuild;
                (document.ActiveView as UFRecipeEditorUI).SaveRecipeLayout();
                document.SaveToFile();
            });

            var list2 = (from c in mapActiveUADocuments.Values// .AsParallel()
                         where c.Parent == parent && c.ActiveView != null
                         select c).ToList();

            list2.ForEach(document =>
            {
                document.SaveToFile();
            });

            if (showWarning && UIInterface != null)
                UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseRecipe(document.ActiveView as UFRecipeEditorUI))
                    return false;
            }

            var list2 = (from c in mapActiveUADocuments.Values// .AsParallel()
                         where c.Parent == parent && c.ActiveView != null
                         select c).ToList();

            foreach (var document in list2)
            {
                if (!CloseView(document.ActiveView as RecipeUAEditorControl))
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

            var listToClean2 = (from c in mapActiveUADocuments// .AsParallel()
                                where c.Value.Parent == parent && c.Value.ActiveView == null
                                select c).ToList();
            listToClean2.ForEach(pair =>
            {
                mapActiveUADocuments.Remove(pair.Key);
                pair.Value.Dispose();

                if (mapActiveUADocumentUris.ContainsKey(pair.Value))
                    mapActiveUADocumentUris.Remove(pair.Value);
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

            var list2 = (from c in mapActiveUADocuments.Values// .AsParallel()
                         where c.Parent == parent && c.ActiveView != null
                         select c).ToList();
            foreach (var document in list2)
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
                    if (!IsRecipeUri(uri))
                    {
                        // if (!bCopy)
                        {
                            RecipeUAServerDocument doc = null;
                            if (mapActiveUADocuments.TryGetValue(uri.GetPathString(), out doc))
                                CloseView(doc.ActiveView as RecipeUAEditorControl, false);
                        }

                        RecipeUAServerDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null, this);
                    }
                    else
                    {
                        UFRecipeDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        {
                            if (!bCopy)
                                CloseRecipe(doc.ActiveView as UFRecipeEditorUI);
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
                                        var view = doc.ActiveView as UFRecipeEditorUI;
                                        if (view == null)
                                            return;

                                        if (view.Document.NeedsRebuild)
                                            UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);

                                        view.SaveRecipeLayout();
                                        doc.SaveCurrentDocument();
                                    }
                                }
                            }
                        }

                        UFRecipeDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                        if (bCopy)
                        {
                            using (var copyDoc = UFRecipeDocument.FromFile(newPath, parent))
                            {
                                if(copyDoc != null)
                                {
                                    copyDoc.RenewUniqueIndentifiers();
                                    copyDoc.SaveCurrentDocument();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    if (!IsRecipeUri(uri))
                    {
                        RecipeUAServerDocument doc = null;
                        if (mapActiveUADocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as RecipeUAEditorControl, false);

                        RecipeUAServerDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
                    }
                    else
                    {
                        UFRecipeDocument doc = null;
                        bool bReopen = false;
                        mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                        if (doc != null && doc.ActiveView is UFRecipeEditorUI)
                        {
                            bReopen = true;
                            if (!CloseRecipe(doc.ActiveView as UFRecipeEditorUI))
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

                        var newname = UFRecipeDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

                        if (bReopen)
                        {
                            Edit(new Uri(newname, UriKind.RelativeOrAbsolute), parent);
                        }
                    }
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (!IsRecipeUri(uri))
            {
                if (mapActiveUADocuments.ContainsKey(uri.GetPathString()))
                    return mapActiveUADocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
                var doc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent);
                if (doc != null)
                {
                    doc.Parent = parent;
                    using (doc)
                    {
                        return doc.SaveToFile(bForceSave: true, forceEncryption: encryptFile);
                    }
                }
            }
            else
            {
                if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                    return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
                var doc = UFRecipeDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = parent;
                    using (doc)
                    {
                        return doc.SaveToFile(forceEncryption: encryptFile);
                    }
                }
            }
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }
#endif

        public IDocument GetDocument(Uri uri)
        {
            if (!IsRecipeUri(uri))
            {
                if (mapActiveUADocuments.ContainsKey(uri.GetPathString()))
                    return mapActiveUADocuments[uri.GetPathString()];
            }
            else
            {
                if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                    return mapActiveDocuments[uri.GetPathString()];
            }
            
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
            if (!IsRecipeUri(uri))
                return null;

            UFRecipeDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFRecipeDocument.FromFile(uri.GetPathString(), parent);
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
            var doc = GetOrCreateDocument(parent);
            if (doc == null || doc.IsEmpty)
                return null;

            try
            {
                string projectConn = String.Empty;
                string stringConn = String.Empty;
                string userConn = String.Empty;
                if (doc.FilePath != null)
                {
                    var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                    projectConn = rootParent.FilePath;
                    if (StringEditor != null)
                        stringConn = StringEditor.GetConnectionStringFromFile(doc.rootBase);
                    if (UserEditor != null)
                        userConn = UserEditor.GetConnectionStringFromFile(doc.rootBase);
                }
                else
                    projectConn = userConn = stringConn = doc.ConnectionString;

                var docPath = doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString();
                docPath = docPath.Trim('\\', '/');

                var applicationName = doc.GetAplicationName();
                return new Service.ServiceControl(doc.GetAplicationName(), projectConn, stringConn, userConn, docPath);
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
                return UFRecipeSettings.Properties.Settings.Default.TypeLabel;
            }
        }

#if !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("RCPMEditorSmall");
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
                return GetBitmapImage("EditorRCPMEditor_32x32");
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
                return UFRecipeSettings.Properties.Settings.Default.DefaultFileExt;
            }
        }

        public string FileName
        {
            get { return UFRecipeSettings.Properties.Settings.Default.DefaultUAFileName; }
        }

        public String[] SaveAsFileExtensions
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(UFRecipeSettings.Properties.Settings.Default.SaveAsFileExtensions))
                    return UFRecipeSettings.Properties.Settings.Default.SaveAsFileExtensions.ToLower().Split(';');
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
                return true;
            }
        }

        public bool isServiceResource
        {
            get { return true; }
        }

#if !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            return false;
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
            String path, newReportName;
            int i = 0;
            Uri url;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newReportName = String.Format("{0}{1}", UFRecipeDocument.DefaultRecipeName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, UFRecipeSettings.Properties.Settings.Default.DefaultFileExt);
                } while (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)));

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(url, parent, encryptFile);
                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, UFRecipeSettings.Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, parent, encryptFile);
                return uri;
            }

            do
            {
                newReportName = String.Format("{0}{1}", UFRecipeDocument.DefaultRecipeName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, UFRecipeSettings.Properties.Settings.Default.DefaultFileExt);
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
                return typeof(UFRecipeDocument);
            }
        }

#endregion IDocumentManager Members

#region IRecipeEditorManager Members
#if !NET_STANDARD
        public object AddRecipe(Uri relative, IDocument parent)
        {

            Uri _relative = CreateNewDocument(relative, parent);
            UFRecipeDocument doc = CreateDoc(_relative, parent);
            doc.SaveToFile();
            return (object)doc.RecipeEntity;
        }


        public object AddGroup(string name, IDocument parent)
        {
            UFRecipeDocument doc = parent as UFRecipeDocument;
            if (doc != null)
                return (object)doc.AddNewGroup(name);
            else
                return null;
        }


        public void SaveToFile(IDocument parent)
        {
            UFRecipeDocument doc = parent as UFRecipeDocument;
            if (doc != null)
                doc.SaveToFile();
        }
#endif
        public IAuditTrace GetAuditTraceInterface(Uri uri, IDocument parent)
        {
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            uri = rootParent.MakeAbosoluteUri(uri);

            List<UFRecipeExecuter.UFRecipeExecuter> recipeExecuters = null;
            lock (lockObject)
            {
                if (mapRunningRecipesPerParent.ContainsKey(rootParent))
                    recipeExecuters = new List<UFRecipeExecuter.UFRecipeExecuter>(mapRunningRecipesPerParent[rootParent]);
            }

            UFRecipeExecuter.UFRecipeExecuter executer = null;
            if (recipeExecuters != null)
            {
                var executers = (from c in recipeExecuters
                                 where c.RecipeDocument.FullPath == uri.GetPathString()
                                 select c).ToList();
                if (executers.Count > 0)
                    executer = executers[0];
            }

            if (executer != null && executer.RecipeUAViewModel != null)
                return executer.RecipeUAViewModel;
            return null;
        }

        public bool IsReady(Uri uri, int commandType, IDocument parent)
        {
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            uri = rootParent.MakeAbosoluteUri(uri);

            List<UFRecipeExecuter.UFRecipeExecuter> recipeExecuters = null;
            lock (lockObject)
            {
                if (mapRunningRecipesPerParent.ContainsKey(rootParent))
                    recipeExecuters = new List<UFRecipeExecuter.UFRecipeExecuter>(mapRunningRecipesPerParent[rootParent]);
            }

            UFRecipeExecuter.UFRecipeExecuter executer = null;
            if (recipeExecuters != null)
            {
                var executers = (from c in recipeExecuters
                                 where c.RecipeDocument.FullPath == uri.GetPathString()
                                 select c).ToList();
                if (executers.Count > 0)
                    executer = executers[0];
            }

            return executer != null && executer.IsReady((UFRecipeExecutionContext.RecipeCommandType)commandType);
        }

        public String GetRecipeUAServerEntityReference(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null/* || (bCheckEmpty && doc.IsEmpty)*/)
                return null;
            try
            {
                return doc.GetRecipeUAServerEntityReference().ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                logRecipe.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetRecipeUAServerEntityReference(IDocument parent, String relativePath, String nodeID)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null/* || (bCheckEmpty && doc.IsEmpty)*/)
                return null;
            try
            {
                return doc.GetRecipeUAServerEntityReference(relativePath, nodeID).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                logRecipe.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public UFRecipeExecuter.UFRecipeExecuter GetRecipeExecuter(Uri uri, IDocument parent)
        {
            lock (lockObject)
            {
                var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                if (mapRunningRecipesPerParent.ContainsKey(rootParent))
                {
                    return (from c in mapRunningRecipesPerParent[rootParent]
                            where c.RecipeDocument.FullPath == uri.GetPathString()
                            select c).FirstOrDefault();
                }

                return null;
            }
        }
#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            mapActiveUADocuments.Clear();
            mapActiveUADocumentUris.Clear();

            foreach (UFRecipeExecuter.UFRecipeExecuter recipe in mapRunningRecipes.Values)
            {
                recipe.StateChanged -= executer_StateChanged;
                recipe.Dispose();
            }
            mapRunningRecipes.Clear();
            lock (lockObject)
                mapRunningRecipesPerParent.Clear();

#if !NET_STANDARD
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

#region ICrossReference
#if !NET_STANDARD
        public List<CrossReferenceResultModel> GetCRObjects(CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            if (!getTexts && !getTags && !getConnections)
                return result;

            IUFUAEditorManager uFUAEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            string histDefConnectionString = null;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string pTitle = p.Title;
            var path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            List<String> list = model.ResourceList as List<String>;
            string docType = DocManagerType.RecipeEditor.ToString();
            Parallel.ForEach(list, (resource, loopstate) =>
            {
                if (model.QuitEvent.IsCancellationRequested)
                    loopstate.Break();

                var uri = new Uri($"{path}\\{resource}", UriKind.RelativeOrAbsolute);
                var pathString = uri.GetPathString();
                using (UFRecipeDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null && doc.RecipeEntity != null)
                    {
                        if (!string.IsNullOrEmpty(doc.RecipeEntity.Description) && getTexts)
                            lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = doc.RecipeEntity.Description,
                                    Name = doc.RecipeEntity.Description,
                                    AppName = $"{p.Title}",
                                    CReferenceType = CrossReferenceType.Strings,
                                    Description = string.Format("{0} ({1})", doc.Title, Properties.Resources.CRDescription),
                                    Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });

                        if (getTexts || getTags)
                        {
                            if (getTexts)
                            {
                                List<UFGroupEntity> groupList = doc.GetGroupsCollection();
                                groupList.ForEach(g =>
                                {
                                    if (!string.IsNullOrEmpty(g.Description))
                                        lock (result)
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = g.Description,
                                                Name = g.Description,
                                                AppName = $"{p.Title}",
                                                CReferenceType = CrossReferenceType.Strings,
                                                Description = string.Format("{0}\\{1} ({2})", doc.Title, g.Name, Properties.Resources.CRGroupDescription),
                                                Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                });
                            }
                            List<UFDataValueEntity> itemlist = doc.GetCRDataValuesCollection() as List<UFDataValueEntity>;
                            for (int j = 0; j < itemlist.Count(); j++)
                            {
                                var item = itemlist[j];
                                if (model.QuitEvent.IsCancellationRequested)
                                    loopstate.Break();
                                if (getTags)
                                {
                                    AddEntityTag(result, item.TagDataValue, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagDataValue);
                                    AddEntityTag(result, item.TagIODataValue, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagIODataValue);
                                }
                                if (getTexts && !string.IsNullOrEmpty(item.Description))
                                    lock (result)
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = item.Description,
                                            Name = item.Description,
                                            AppName = $"{p.Title}",
                                            CReferenceType = CrossReferenceType.Strings,
                                            Description = string.Format("{0}\\{1} ({2})", doc.Title, item.Name, Properties.Resources.CRDescription),
                                            Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                if (getTexts && !string.IsNullOrEmpty(item.UnitName))
                                    lock (result)
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = item.UnitName,
                                            Name = item.UnitName,
                                            AppName = $"{p.Title}",
                                            CReferenceType = CrossReferenceType.Strings,
                                            Description = string.Format("{0}\\{1} ({2})", doc.Title, item.Name, Properties.Resources.CRUnitName),
                                            Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                            }
                            if (getTags)
                            {
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeDelete, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeDelete);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeIndex, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeIndex);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeList, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeList);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeLoad, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeLoad);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeRead, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeRead);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeSave, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeSave);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeState, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeState);
                                AddEntityTag(result, doc.RecipeEntity.TagRecipeWrite, doc.RecipeEntity.Name, pathString, doc.Title, Properties.Resources.CRTagRecipeWrite);
                            }
                        }

                        if (getConnections)
                        {
                            string connectionString = doc.RecipeEntity.ReadableConnectionString;
                            if(string.IsNullOrEmpty(connectionString))
                            {
                                if(string.IsNullOrEmpty(histDefConnectionString))
                                    histDefConnectionString = uFUAEditorManager?.GetHistorianDefaultConnection(doc);
                                connectionString = histDefConnectionString;
                            }

                            if(!string.IsNullOrEmpty(connectionString))
                                lock (result)
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = connectionString, //$"{TypeTitle}\\{doc.Title}",
                                        Name = Properties.Resources.RefrencedBy, //$"{doc.Title} - {Properties.Resources.DefConnectionString}",
                                        AppName = $"{TypeTitle}",
                                        CReferenceType = CrossReferenceType.Connections,
                                        Description = doc.Title, //doc.RecipeEntity.ConnectionString,
                                        Settings = string.Format("{0}|{1}|{2}", docType, doc.rootBase, Properties.Resources.RefrencedBy),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                        }
                    }
                }
            });

            using (RecipeUAServerDocument doc = CreateDocument(model.Parent))
            {
                if (doc != null)
                {
                    if (getConnections)
                    {
                        var settings = doc.GetConfiguration();
                        if (settings != null && !string.IsNullOrEmpty(settings.EventDefaultConnection))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = XpoHelper.NormalizeConnectionString(settings.EventDefaultConnection, doc.rootBase), //$"{TypeTitle}",
                                Name = Properties.Resources.DefConnectionString,
                                AppName = $"{TypeTitle}",
                                CReferenceType = CrossReferenceType.Connections,
                                Description = TypeTitle, //settings.EventDefaultConnection,
                                Settings = string.Format("{0}|{1}", docType, doc.rootBase),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                    }
                }
            }

            return result;
        }

        private void AddEntityTag(List<UFInterfaces.Editors.CrossReferenceResultModel> result,OPCUAEntityReference tagRecipe, string name, string pathString, string title, string tagType)
        {
            if (IsTagReferenceValid(tagRecipe))
            {
                UFInterfaces.Editors.CrossReferenceResultModel resultModel = GetCREntityTagModel(tagRecipe, name, pathString, title, tagType);
                lock (result)
                    result.Add(resultModel);
            }
        }

        private CrossReferenceResultModel GetCREntityTagModel(OPCUAEntityReference tagRecipe, string name, string pathString, string title, string tagType)
        {
            string docType = DocManagerType.RecipeEditor.ToString();
            return new CrossReferenceResultModel()
            {
                RelativePath = tagRecipe.RelativePath,
                Name = tagRecipe.Name,
                AppName = tagRecipe.AppName,
                ReferencedNodeId = tagRecipe.ResolvedNodeId?.Identifier.ToString(),
                EndpointUrl = tagRecipe.EndpointUrl,
                CReferenceType = CrossReferenceType.Tags,
                Description = string.Format("{0}\\{1} ({2})", title, name, tagType),
                Settings = string.Format("{0}|{1}", docType, pathString),
                ContainerDoc = TypeScheme,
                IconType = docType
            };
        }
        public bool IsTagReferenceValid(OPCUAEntityReference tagReference)
        {
            return tagReference != null && tagReference.IsValid;
        }
        private List<UFInterfaces.Editors.CrossReferenceResultModel> GetDataValueCRList(UFDataValueEntity entity, UFRecipeDocument doc, string pathstring, IDocument p)
        {
            string docType = DocManagerType.RecipeEditor.ToString();
            List<UFInterfaces.Editors.CrossReferenceResultModel> result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (entity.IsTagReferenceValid())
            {
                var details = entity.TagDataValue;
                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                {
                    RelativePath = details.RelativePath,
                    Name = details.Name,
                    AppName = details.AppName,
                    ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                    EndpointUrl = details.EndpointUrl,
                    CReferenceType = CrossReferenceType.Tags,
                    Description = string.Format("{0}\\{1} ({2})", doc.Title, entity.Name, Properties.Resources.CRTagDataValue),
                    Settings = string.Format("{0}|{1}", docType, pathstring),
                    ContainerDoc = TypeScheme,
                    IconType = docType
                });
            }
            if (entity.IsTagIOReferenceValid())
            {
                var details = entity.TagIODataValue;
                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                {
                    RelativePath = details.RelativePath,
                    Name = details.Name,
                    AppName = details.AppName,
                    ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                    EndpointUrl = details.EndpointUrl,
                    CReferenceType = CrossReferenceType.Tags,
                    Description = string.Format("{0}\\{1} ({2})", doc.Title, entity.Name, Properties.Resources.CRTagIODataValue),
                    Settings = string.Format("{0}|{1}", docType, pathstring),
                    ContainerDoc = TypeScheme,
                    IconType = docType
                });
            }
            return result;
        }

        private RecipeUAServerDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            RecipeUAServerDocument doc = null;
            doc = RecipeUAServerDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        private UFRecipeDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            UFRecipeDocument doc = null;
            doc = UFRecipeDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        public bool NeedToCloseCRDocuments()
        {
            return mapActiveDocuments.Count > 0 && (from d in mapActiveDocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null ||
                mapActiveUADocuments.Count > 0 && (from d in mapActiveUADocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null;
        }

        public bool NeedSingleThreadedApartment
        {
            get
            {
                return false;
            }
        }

        public void RenameCRObjects(CrossReferenceModel model)
        {
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            foreach (var resource in model.ResourceList)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;
                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (UFRecipeDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
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
#endregion
    }
}