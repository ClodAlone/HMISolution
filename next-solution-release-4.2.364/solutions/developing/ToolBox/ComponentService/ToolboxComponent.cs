using System;
using UFInterfaces.CoreHostComponents;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using UFInterfaces;
using Tracing.ComponentService;
using Utilities.WPF;
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using System.Collections.Generic;
using UriResolver.ComponentService;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Linq;

namespace Toolbox.ComponentService
{
    public class ToolboxComponent : ComponentBase<IToolbox>, IToolbox, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        Uri baseFolder;
        ToolBoxUI toolboxUI;
        Object PromoteSelectingObject;
        Object PromoteSelectingDocument;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;

        const String xamlSignature = "xmlns=";
        const string hiddenCategoriesFileName = "HiddenToolboxCategories";
        internal static List<string> HiddenCategories = null;
        #endregion
        bool IsWebHMIToolbox{ get; set; }

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            Dispatcher.CurrentDispatcher.InvokeIfRequired(() =>
                {
                    GetComponentInterfaces();
                    CreateToolbox();
                });
        }
        #endregion

        internal static BitmapImage GetControlImage(string image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("Toolbox", image, bShared);
            return bm;
        }

        internal bool IsWebHMIProject()
        {
            return SelectDocument is IDocument && !string.IsNullOrEmpty((SelectDocument as IDocument).ProjectType) && (SelectDocument as IDocument).ProjectType == ProjectType.WebHMI.ToString();
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
        }

        ContentControl emptyControl;
        public void CreateToolbox()
        {
            lock (lockObject)
            {
                if (toolboxUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.ContextDocumentChanged += workspace_ContextDocumentChanged;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ContentRendered += workspace_ContentRendered;
                    workspace.EasyModeChanged += workspace_EasyModeChanged;

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage("TBEditor");

                    workspace.AddDockingChildren(emptyControl, Toolbox.Properties.Resources.Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Right, false, itemID: nameof(ToolboxComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));

                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        toolboxUI = new ToolBoxUI(this);

                        toolboxUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && toolboxUI != null && toolboxUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(toolboxUI) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        toolboxUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, toolboxUI.Height, toolboxUI.Width);
                        toolboxUI.ClearValue(FrameworkElement.WidthProperty);
                        toolboxUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = toolboxUI;

                        //if (workspace.ActiveWindow != null &&
                        //    workspace.GetElementDockState(workspace.ActiveWindow) == DockState.Document)
                        //    SelectDocument = workspace.ActiveWindow;
                        //else
                        //    SelectDocument = null;
                    }
                }
            }
        }

        private void UpdateHiddenCategories()
        {
            if (IsWebHMIProject() != IsWebHMIToolbox)
            {
                HiddenCategoriesLoadCheck();
                toolboxUI?.ResetToolbox();
            }
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            bEnableIdleCode = true;
            if (bPendingIdleCode)
            {
                bPendingIdleCode = false;
                PromoteCodeToIdle(false);
            }
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (workspace.ContextObject != null)
            {
                SelectObject = workspace.ContextObject;
            }
            else
            {
                SelectObject = null;
            }
            bContextObjectChanged = true;
        }

        void workspace_ContextDocumentChanged(object sender, EventArgs e)
        {
            if (workspace.ContextDocument != null)
            {
                SelectDocument = workspace.ContextDocument;
                UpdateHiddenCategories();
            }
            else
            {
                SelectDocument = null;
            }
            bContextObjectChanged = true;
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == emptyControl && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (emptyControl != null)
                    emptyControl.Visibility = Visibility.Visible;
                PromoteCodeToIdle(true);
            }
            else if (sender == emptyControl && (e.NewState == UFInterfaces.DockState.Hidden || e.NewState == UFInterfaces.DockState.AutoHidden))
            {
                if (emptyControl != null)
                {
                    emptyControl.Visibility = Visibility.Collapsed;
                    bAutoHideVisible = false;
                }
            }
        }

        UFInterfaces.DockSide lastDockSide;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == emptyControl)
            {
                UpdateHiddenCategories();
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && workspace.GetElementIsSelectedTab(emptyControl))
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;

                    PromoteCodeToIdle(true);
                }
            }
            if (e.OldValue == emptyControl)
            {
                lastDockSide = workspace.GetElementDockSide(emptyControl);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(emptyControl) &&
                    workspace.GetElementDockState(emptyControl) != DockState.Float)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Collapsed;
                }
            }

            if (emptyControl != null && (e.OldValue == emptyControl || e.NewValue == emptyControl))
            {
                var dockstate = workspace.GetElementDockState(emptyControl);
                if (dockstate == UFInterfaces.DockState.Document)
                {
                    if (e.NewValue == emptyControl && bLoaded)
                        emptyControl.Visibility = Visibility.Visible;
                    //else if (e.OldValue == emptyControl)
                    //    emptyControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreateToolbox();

                bAutoHideVisible = true;
                if (idleOperation != null)
                {
                    idleOperation.Abort();
                    idleOperation = null;
                }
                // if (bAutoHideVisible)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                }
            }
        }

        DispatcherOperation idleOperation;
        void workspace_AutoHideAnimationStop(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                if (emptyControl != null/* && workspace.ActiveWindow != emptyControl*/)
                {
                    //var dockstate = workspace.GetElementDockState(emptyControl);
                    //if (dockstate == DockState.AutoHidden)
                    {
                        if (bWasAutoHideVisible)
                        {
                            bWasAutoHideVisible = false;
                            if (idleOperation == null)
                            {
                                idleOperation = emptyControl.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                {
                                    if (!DockingHelper.GetLayoutItemVisible(emptyControl))
                                        emptyControl.Visibility = Visibility.Collapsed;
                                    else
                                        bAutoHideVisible = bWasAutoHideVisible = true;
                                });

                                idleOperation.Completed += (o, ev) =>
                                {
                                    idleOperation = null;
                                };
                            }
                            bAutoHideVisible = false;
                        }
                        else
                        {
                            bWasAutoHideVisible = true;
                            PromoteCodeToIdle(false);
                        }
                    }
                }
                else
                    PromoteCodeToIdle(true);
            }
        }

        private void PromoteSelectionObject()
        {
            lock (lockObject)
            {
                PromoteCodeToIdle(false);
            }
        }

        private void PromoteCodeToIdle(bool bSynchro)
        {
            if (!bEnableIdleCode)
            {
                bPendingIdleCode = true;
                return;
            }

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (IdleExecutionPending == null && (bSynchro || bVisible))
            {
                Action action = () => IdleExecution();
                Dispatcher disp = emptyControl != null ? emptyControl.Dispatcher : Dispatcher.CurrentDispatcher;
                IdleExecutionPending = disp.BeginInvoke(action, bSynchro ? DispatcherPriority.Send : DispatcherPriority.Background);
                IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
        }

        private IDocumentManager GetCurrentSelectionType()
        {
            if (SelectDocument is DocumentManager.ComponentService.IDocument && UriRisolver != null)
            {
                var manager = UriRisolver.GetManagerFromDocument(SelectDocument as DocumentManager.ComponentService.IDocument);
                if (manager != null)
                    return manager;
            }

            return null;
        }

        private void IdleExecution()
        {
            CreateToolbox();
            if (!bContextObjectChanged)
                return;
            bContextObjectChanged = false;

            if (toolboxUI != null)
            {
                //String subType = String.Empty;
                //if (SelectObject != null && SelectObject is IEntityReference)
                //    subType = (SelectObject as IEntityReference).TypeDefinitionString;
                var manager = GetCurrentSelectionType();
                if (manager != null)
                {
                    var arg = new PromptForControlEventArgs() { TypeLabel = manager.TypeLabel };
                    OnPromptFriendObjects(this, arg);
                    if (arg.toolboxControl != null)
                        toolboxUI.LoadToolboxItems(manager.TypeLabel, arg.toolboxControl);
                    else
                        toolboxUI.LoadToolboxItems(manager.TypeLabel, manager.FileType);
                }
            }
        }

        IEnumerable<String> SearchForFolder(String folder, String search)
        {
            if (Directory.Exists(folder))
            {
                var list = Directory.GetDirectories(folder);
                foreach (var f in list)
                {
                    foreach (var found in SearchForFolder(f, search))
                        yield return found;

                    string[] directoryGetFiles = Directory.GetFiles(f, search);
                    if (directoryGetFiles.Length > 0)
                        yield return f;
                }
            }
        }

        IEnumerable<String> SearchForFolder(String folder, String search, String categoryRelativePath, bool deepsearch, bool bIsInEasyMode)
        {
            if (Directory.Exists(folder))
            {
                var list = Directory.GetDirectories(folder);
                foreach (var f in list)
                {
                    var relativeFolder = !String.IsNullOrEmpty(categoryRelativePath) ? Path.Combine(categoryRelativePath, Path.GetFileName(f)) : Path.GetFileName(f);
                    if (bIsInEasyMode)
                    {
                        if (HiddenCategories != null && HiddenCategories.Contains(relativeFolder))
                            continue;
                    }
                    if (IsWebHMIProject())
                    {
                        if (!WebHMIDesignHelper.WebHMIHelper.VisibleHMIToolboxCategories.Contains(relativeFolder.Replace('/', '\\')))
                            continue;
                    }
                    if (deepsearch)
                    {
                        foreach (var found in SearchForFolder(f, search, categoryRelativePath, deepsearch, bIsInEasyMode))
                            yield return found;
                        string[] directoryGetFiles = Directory.GetFiles(f, search);
                        if (directoryGetFiles.Length > 0)
                            yield return f;
                    }
                    else
                        yield return f;
                }
            }
        }

        #region Properties

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

        public object SelectObject
        {
            get
            {
                return PromoteSelectingObject;
            }
            set
            {
                PromoteSelectingObject = value;
                PromoteSelectionObject();
            }
        }

        public object SelectDocument
        {
            get
            {
                return PromoteSelectingDocument;
            }
            set
            {
                if (value != null)
                    PromoteSelectingDocument = value;
                PromoteSelectionObject();
            }
        }

        public IWorkspace WorkSpace
        {
            get
            {
                return workspace;
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

        #endregion

        #region IToolbox interface

        public ToolBoxData ActiveToolCode
        {
            get
            {
                if (toolboxUI == null)
                    CreateToolbox();
                return toolboxUI.ActiveToolCode;
            }

            set
            {
                if (toolboxUI == null)
                    CreateToolbox();
                toolboxUI.ActiveToolCode = value;
            }
        }

        public String GetCodeFromHash(String hash, IDocumentManager manager, out string fullPath)
        {
            fullPath = null;
            try
            {
                string xaml = null;
                var categories = GetListCategories(manager.TypeLabel, manager.FileType).ToList();
                if (categories.Count == 0)
                    return null;
                for (int i = 0; i < categories.Count; i++)
                {
                    string[] directoryGetFiles = Directory.GetFiles(categories[i], $"*{manager.FileType}");
                    if (directoryGetFiles.Length > 0)
                    {
                        fullPath = (from f in directoryGetFiles
                                       where Path.GetFileNameWithoutExtension(f).Equals(hash)
                                       select f).FirstOrDefault();
                        if (!string.IsNullOrEmpty(fullPath))
                        {
                            xaml = GetCodeFromHash(fullPath);
                            return xaml;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public String GetCodeFromHash(String hash)
        {
            try
            {
                return File.ReadAllText(hash);
            }
            catch (Exception ex)
            {
                
            }

            return null;
        }

        const String settingsExt = ".settings";
        public String GetCurrentDropSettings(String hash)
        {
            try
            {
                String fileSettings = hash + settingsExt;
                if (File.Exists(fileSettings))
                    return File.ReadAllText(fileSettings);
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public String GetCurrentSourceSymbolProvider(String hash)
        {
            //if (fileSystemProvider != null)
            //{
            //    var ret = (fileSystemProvider as DataSourceFileSystemProvider).ConnectionString;
            //    return WPFUtilities.CryptString.CryptString.EncryptString(ret);
            //}
            //else
            {
                return String.Empty;
            }
        }

        const String pathData = "{0}@{1}?{2}";
        const String dataExt = ".data";
        public String GetCurrentSourceSymbolPath(String hash)
        {
            try
            {
                String fileSettings = hash + dataExt;
                if (File.Exists(fileSettings))
                    return File.ReadAllText(fileSettings);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        
        public string GetToolboxPath()
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var folder = String.Format("{0}.{2}\\{1}\\", startingPath, Properties.Settings.Default.ToolBoxFolder, mainversion);
            return folder;
        }

        static void LoadHiddenCategories()
        {
            HiddenCategories = new List<string>();
            string filepath = string.Format("{0}\\{1}.xml", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), hiddenCategoriesFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    var xml = XDocument.Load(@filepath);
                    foreach (XElement node in xml.Root.Descendants("Name").ToList())
                    {
                        var componentName = node.Value as String;
                        if (!String.IsNullOrEmpty(componentName))
                            HiddenCategories.Add(componentName.Trim());
                    }
                }
                catch (Exception ex) { }
            }
        }
        
        public IEnumerable<String> GetListCategories(String Type, String fileType, bool deepsearch)
        {
            if (workspace.IsInEasyMode || IsWebHMIProject())
                HiddenCategoriesLoadCheck();

            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var folder = String.Format("{0}.{3}\\{1}\\{2}", startingPath, Properties.Settings.Default.ToolBoxFolder, Type, mainversion);

            var rootFolder = GetRootFolder(Type);
            string categoryRelativePath = null;
            if (Type != rootFolder)
            {
                baseFolder = new Uri(String.Format("{0}.{3}\\{1}\\{2}\\", startingPath, Properties.Settings.Default.ToolBoxFolder, GetRootFolder(Type), mainversion));
                categoryRelativePath = baseFolder.MakeRelativeUri(new Uri(folder)).GetPathString();
            }

            var search = String.Format("*{0}", fileType);
            return SearchForFolder(folder, search, categoryRelativePath, deepsearch, workspace.IsInEasyMode);
        }

        void workspace_EasyModeChanged(object sender, EventArgs e)
        {
            HiddenCategoriesLoadCheck();
            toolboxUI?.ResetToolbox();
        }

        void HiddenCategoriesLoadCheck()
        {
            if (HiddenCategories == null)
                LoadHiddenCategories();
            IsWebHMIToolbox = IsWebHMIProject();
        }

        static string GetRootFolder(string path)
        {
            while (true)
            {
                string temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;
                path = temp;
            }
            return path;
        }

        public IEnumerable<String> GetListCategories(String Type, String fileType)
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var folder = String.Format("{0}.{3}\\{1}\\{2}", startingPath, Properties.Settings.Default.ToolBoxFolder, Type, mainversion);

            var search = String.Format("*{0}", fileType);
            return SearchForFolder(folder, search);
        }

        public IEnumerable<String> GetQuickListCategories(String Type, String fileType)
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var folder = String.Format("{0}.{3}\\{1}\\{2}", startingPath, Properties.Settings.Default.QuickToolBoxFolder, Type, mainversion);

            var search = String.Format("*{0}", fileType);
            return SearchForFolder(folder, search);
        }

        public IEnumerable<ToolBoxData> GetListTools(String Type, String fileType, String Category)
        {
            var search = String.Format("*{0}", fileType);
            string[] directoryGetFiles = Directory.GetFiles(Category, search);
            foreach (var file in directoryGetFiles)
            {
                if (IsWebHMIProject())
                {
                    string categoryRelativePath = null;
                    categoryRelativePath = baseFolder.MakeRelativeUri(new Uri(Category)).GetPathString();
                    var relativeFile = !String.IsNullOrEmpty(categoryRelativePath) ? Path.Combine(categoryRelativePath, Path.GetFileNameWithoutExtension(file)) : Path.GetFileNameWithoutExtension(file);
                    if (!WebHMIDesignHelper.WebHMIHelper.VisibleHMIToolboxCategories.Contains(relativeFile.Replace('/','\\')))
                        continue;
                }

                var toolBoxData = new ToolBoxData();
                String imagepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file),
                    $"{System.IO.Path.GetFileNameWithoutExtension(file)}_{workspace.ThemeKey}{Properties.Settings.Default.ToolboxIconExtension}");
                if (!File.Exists(imagepath))
                    imagepath = System.IO.Path.ChangeExtension(file, Properties.Settings.Default.ToolboxIconExtension);
                if (File.Exists(imagepath))
                {
                    // Create source.
                    var bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(imagepath, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    toolBoxData.image = bi;
                }
                else
                    toolBoxData.image = GetControlImage("TBTools");

                toolBoxData.Title = System.IO.Path.GetFileNameWithoutExtension(file);
                toolBoxData.Hash = file;
                yield return toolBoxData;
            }
        }

        public bool IsToolboxDragging(Object obj)
        {
            return obj is ToolControlTagHelper;
        }

        #endregion

        #region Events
        public event EventHandler<PromptForControlEventArgs> PromptForControl;

        virtual public void OnPromptFriendObjects(object sender, PromptForControlEventArgs e)
        {
            EventHandler<PromptForControlEventArgs> temp = PromptForControl;
            if (temp != null)
                temp(sender, e);
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (toolboxUI != null)
            {
                toolboxUI.Dispose();
                toolboxUI = null;
            }

            PromoteSelectingObject = null;
            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.ContextDocumentChanged -= workspace_ContextDocumentChanged;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
                workspace.EasyModeChanged -= workspace_EasyModeChanged;
            }

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }

            lockObject = null;
        }

        #endregion
    }
}
