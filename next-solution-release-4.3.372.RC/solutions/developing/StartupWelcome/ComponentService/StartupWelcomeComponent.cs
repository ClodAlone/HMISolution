using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HelpProvider.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UFInterfaces.StartupWelcome;
using UFProjectWizard.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using UriResolver.ComponentService;
using ScreenManager.ComponentService;
using System.ComponentModel;
using UFProjectManager.ComponentService;
using UFUAEditor.ComponentService;
using StringManager.ComponentService;
using UFRecipeEditor.ComponentService;
using StartupWelcome.View_Model;
using Toolbox.ComponentService;
using MSSchedulerSettings.ComponentService;
using ADEditor.ComponentService;

namespace StartupWelcome.ComponentService
{
    public class StartupWelcomeComponent : ComponentBase<IStartupWelcome>, IStartupWelcome, IDisposable
    {
        #region Declaration
        StartupWelcomeUI startupWelcomeUI;
        internal MenuControl menuControl;
        internal RecentRepository Recent = new RecentRepository();
        public static StartupWelcomeComponent startupWelcomeComponent { get; protected set; }
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (startupWelcomeComponent == null)
                startupWelcomeComponent = this;

            GetComponentInterfaces();
        }

        #endregion

        void workspace_Loading(object sender, EventArgs e)
        {
            CreateStartupWelcome();
        }

        void workspace_EasyModeChanged(object sender, EventArgs e)
        {
            if (workspace == null)
                return;

            if (workspace.IsInEasyMode)
                menuControl.DisableLayoutToFile();
            else
                menuControl.EnableLayoutToFile();
        }

        void IDisposable.Dispose()
        {
            if (workspace != null)
            {
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.WorkspaceLoading -= workspace_Loading;
                workspace.EasyModeChanged -= workspace_EasyModeChanged;
            }
            if (startupWelcomeUI != null)
                startupWelcomeUI.Dispose();
        }

        internal static BitmapImage GetControlImage(string image, bool shared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("StartupWelcome", image, shared);
            return bm;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.WorkspaceLoading += workspace_Loading;
            workspace.EasyModeChanged += workspace_EasyModeChanged;
        }

        private void workspace_CloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is StartupWelcomeUI))
                return;

            HideStartUp();
            e.Cancel.Cancel = true;
        }

        public void CreateStartupWelcome()
        {
            if (startupWelcomeUI != null)
                return;

            startupWelcomeUI = new StartupWelcomeUI(this);

            workspace.SetDesiredHeightAndWidthInDockedMode(startupWelcomeUI, startupWelcomeUI.Height, startupWelcomeUI.Width);
            startupWelcomeUI.ClearValue(FrameworkElement.WidthProperty);
            startupWelcomeUI.ClearValue(FrameworkElement.HeightProperty);

            menuControl = new MenuControl(Workspace.IsInEasyMode);
            Workspace.AddBarManagerItem(menuControl, startupWelcomeUI.CommandBindings);

            ShowStartUp();
        }

        public bool IsStartupOpened()
        {
            if (startupWelcomeUI == null)
                return false;
            return workspace.IsDockedChildren(startupWelcomeUI);
        }

        public void HideStartUp()
        {
            if (startupWelcomeUI == null)
                return;
            workspace.RemoveDockingChildren(startupWelcomeUI);
        }

        public void ShowStartUp()
        {
            if (startupWelcomeUI != null)
            {
                //var ret = workspace.GetElementDockState(startupWelcomeUI);
                workspace.AddDockingChildren(startupWelcomeUI, Properties.Resources.StartupWelcome_Title, DockState.Document, DockSide.Left, true, false, itemID: nameof(StartupWelcomeUI));
                workspace.SetDockedElementIcon(startupWelcomeUI, new ImageBrush(GetControlImage("SWEditor")));
                workspace.ActivateDockedElement(startupWelcomeUI);
                //workspace.FlashDockedElement(startupWelcomeUI);

                return;
            }

            startupWelcomeUI = new StartupWelcomeUI(this);

            workspace.SetDesiredHeightAndWidthInDockedMode(startupWelcomeUI, startupWelcomeUI.Height, startupWelcomeUI.Width);
            startupWelcomeUI.ClearValue(FrameworkElement.WidthProperty);
            startupWelcomeUI.ClearValue(FrameworkElement.HeightProperty);
            
            BitmapImage bm = GetControlImage("SWEditor");

            MenuControl menuControl = new MenuControl(workspace.IsInEasyMode);
            Workspace.AddBarManagerItem(menuControl, startupWelcomeUI.CommandBindings);

            workspace.AddDockingChildren(startupWelcomeUI, Properties.Resources.StartupWelcome_Title, DockState.Document, DockSide.Left, true, false, itemID: nameof(StartupWelcomeUI));
            workspace.SetDockedElementIcon(startupWelcomeUI, new ImageBrush(bm));
            workspace.ActivateDockedElement(startupWelcomeUI);
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

        IUFProjectWizard projectWizard;
        public IUFProjectWizard ProjectWizard
        {
            get
            {
                if (projectWizard == null)
                    projectWizard = GetService(typeof(IUFProjectWizard)) as IUFProjectWizard;
                return projectWizard;
            }
        }

        IUFProjectWizardPlugin projectWizardPlugin;
        public IUFProjectWizardPlugin ProjectWizardPlugin
        {
            get
            {
                if (projectWizardPlugin == null)
                    projectWizardPlugin = GetService(typeof(IUFProjectWizardPlugin)) as IUFProjectWizardPlugin;
                return projectWizardPlugin;
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

        IScreenManager screenManager;
        public IScreenManager ScreenManager
        {
            get
            {
                if (screenManager == null)
                    screenManager = GetService(typeof(IScreenManager)) as IScreenManager;
                return screenManager;
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

        IUFUAEditorManager uFUAEditorManager;
        public IUFUAEditorManager UFUAEditorManager
        {
            get
            {
                if (uFUAEditorManager == null)
                    uFUAEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return uFUAEditorManager;
            }
        }

        IStringEditorManager stringEditorManager;
        public IStringEditorManager StringEditorManager
        {
            get
            {
                if (stringEditorManager == null)
                    stringEditorManager = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditorManager;
            }
        }

        IRecipeEditorManager recipeEditorManager;
        public IRecipeEditorManager RecipeEditorManager
        {
            get
            {
                if (recipeEditorManager == null)
                    recipeEditorManager = GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                return recipeEditorManager;
            }
        }

        IToolbox toolboxManager;
        public IToolbox ToolboxManager
        {
            get
            {
                if (toolboxManager == null)
                    toolboxManager = GetService(typeof(IToolbox)) as IToolbox;
                return toolboxManager;
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

#if !CONNEXT
        ISchedulerEditorManager schedulerEditorManager;
        public ISchedulerEditorManager SchedulerEditorManager
        {
            get
            {
                if (schedulerEditorManager == null)
                    schedulerEditorManager = GetService(typeof(ISchedulerEditorManager)) as ISchedulerEditorManager;
                return schedulerEditorManager;
            }
        }

        IADEditorManager adEditorManager;
        public IADEditorManager ADEditorManager
        {
            get
            {
                if (adEditorManager == null)
                    adEditorManager = GetService(typeof(IADEditorManager)) as IADEditorManager;
                return adEditorManager;
            }
        }
#endif
#endregion

        public System.Collections.Generic.IEnumerable<Uri> GetLatestOpened()
        {
            return startupWelcomeUI.GetLatestOpened();
        }

        public void AddToLatestOpened(Uri uri)
        {
            startupWelcomeUI.AddToLatestOpened(uri);
        }

        public void ShowOrActivateStartupWelcome()
        {
            ShowStartUp();
        }
    }
}