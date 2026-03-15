using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows;
using System.Linq;
using Utilities;
using UriResolver.ComponentService;
using ScreenManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.ComponentService;
using UFProjectManager.ComponentService;
using System.Diagnostics;
using WizardSettings;
using StringManager.ComponentService;
using UFRecipeEditor.ComponentService;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using HelpProvider.ComponentService;
using Toolbox.ComponentService;
using MSSchedulerSettings.ComponentService;
using ADEditor.ComponentService;

namespace UFProjectWizard.ComponentService
{
    public class UFProjectWizardComponent : ComponentBase<IUFProjectWizard>, IUFProjectWizard
    {
        #region Declarations
        public static UFProjectWizardComponent projectWizardComponent { get; protected set; }
        #endregion

        #region IUFInterfaceBase Members
        void IUFInterfaceBase.Initialize()
        {
            if (projectWizardComponent == null)
                projectWizardComponent = this;

            if (UriRisolver != null)
                UriRisolver.GetListInstalledDocumentManagers();
        }
        #endregion

        internal static BitmapImage GetControlImage(string image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("UFProjectWizard", image, bShared);
            return bm;
        }

        #region IProjectWizard Members

        public Uri CreateNewProject(Uri current)
        {
            ProjectViewModel NewProject = new ProjectViewModel
            {
                projectManagerService = ProjectManagerService,
                screenManagerService = ScreenManagerService,
                UFUAEditorManager = UFUAEditorManager,
                UIInterface = UIInterface,
                UriRisolver = UriRisolver,
                stringEditorManager = StringEditorManager,
                recipeEditorManager = RecipeEditorManager,
                toolboxManager = ToolboxManager,
                helpProvider = HelpProvider,
                SchedulerEditorManager = SchedulerEditorManager,
                ADEditorManager = ADEditorManager
            };

            var wizardUI = new WizardUI();
            var wnd = new GeneralDialogContent(wizardUI,
                WPFUtilities.Properties.Settings.Default.DialogFontSize,
                WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                GeneralDialogButtons.None, new Dictionary<GeneralDialogButtons, String>(), bmaximizeContent: true)
            {
                Owner = Application.Current.MainWindow,
                Title = " ",
                DialogKeepContent = true,
                HelpLink = "ProjectWizardPluginManager"
            };

            if (wnd.ShowDialog() == true)
            {
                try
                {
                    if (string.IsNullOrEmpty(wizardUI.AssemblyPath) || string.IsNullOrEmpty(wizardUI.AssemblyName))
                        return null;

                    var _assemblyPath = System.IO.Path.GetDirectoryName(wizardUI.AssemblyPath);
                    var _assemblyName = wizardUI.AssemblyName;

                    var list = FindAndLoadDLL.LoadDLLs<IUFProjectWizardPlugin>(_assemblyPath,
                                                        string.Format("{0}{1}", _assemblyName, ".dll"), false);
                    if (list.Count > 0)
                    {
                        return list[0].CreateNewProject(null, Application.Current.MainWindow, NewProject);
                    }

                }
                catch(Exception ex)
                {
                    Debug.WriteLine(String.Format("Plugin wizard error: {0}",
                        ex.Message));
                }
            }

            return null;
        }

        #endregion

        #region Properties
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

        IScreenManager screenManagerService;
        public IScreenManager ScreenManagerService
        {
            get
            {
                if (screenManagerService == null)
                    screenManagerService = GetService(typeof(IScreenManager)) as IScreenManager;
                return screenManagerService;
            }
        }

        IUFUAEditorManager ufuaEditorManager;
        public IUFUAEditorManager UFUAEditorManager
        {
            get
            {
                if (ufuaEditorManager == null)
                    ufuaEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditorManager;
            }
        }

        IUFProjectManager projectManagerService;
        public IUFProjectManager ProjectManagerService
        {
            get
            {
                if (projectManagerService == null)
                    projectManagerService = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                return projectManagerService;
            }
        }

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
        #endregion
    }
}
