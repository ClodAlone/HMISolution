using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
using System.Collections.Generic;
using System.Threading;
using ScreenManager.ComponentService;
using UFProjectManager.ComponentService;
using System.ComponentModel;
using DocumentManager.ComponentService.Helpers;
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using log4net;
using System.Linq;
using DocumentManager.ComponentService;
using WPFUtilities.ImportExportHelpers;

namespace UFProjectManager.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProgressDialog : UserControl, INotifyPropertyChanged
    {
#region Declarations
        string currentProject = "";
        bool bLoaded;
        public event PropertyChangedEventHandler PropertyChanged;
        UFProjectDocument document;
        UFProjectManagerComponent projectManagerComponent;
        bool bSuccess = true;
        bool bHasErrors = false;
        bool bChildAborted = false;
        bool bSilent;
        CancellationTokenSource tokenSource;
        Window wnd;
#if !NET_STANDARD && !WINDOWS_UWP
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#endif
#endregion

        #region Properties
        double currentProgress;
        public double CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                if (value != currentProgress)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }
        string currentState = "";
        public string CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value != currentState)
                {
                    currentState = value;
                    OnPropertyChanged("CurrentState");
                }
            }
        }
        public UIMsgBoxAlertService.ComponentService.CustomDialogResults DeployServerRemote
        {
            get;
            set;
        }

        public string ExportedProjectFilePath { get; private set; }

        public bool ExportResult
        {
            get
            {
                return !bHasErrors;
            }
        }
#endregion

#region Ctor
        public ProgressDialog(UFProjectDocument doc, UFProjectManagerComponent comp, bool bSilent = false)
        {
            InitializeComponent();

            document = doc;
            projectManagerComponent = comp;
            DataContext = this;
            this.bSilent = bSilent;
            
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        projectManagerComponent.Workspace.ProgressStateChanged += OnProgressStateChanged;
                        wnd.ContentRendered += OnContentRendered;
                        wnd.Closing += (s, ea) =>
                        {
                            wnd.ContentRendered -= OnContentRendered;
                            projectManagerComponent.Workspace.ProgressStateChanged -= OnProgressStateChanged;
                        };
                    }
                }
            };
        }
#endregion
#region Methods
        void OnContentRendered(object sender, EventArgs e)
        {
            wnd.ContentRendered -= OnContentRendered;
            StartExport();
        }

        void StartExport()
        {
            tokenSource = new CancellationTokenSource();
            try
            {
                var parent = DocumentHelper.GetRootParent(document, traverse: true) as UFProjectDocument;
                var svgPath = parent.rootBase;

                if (!ExportProjectWithChilds(parent, true))
                    bSuccess = false;                
            }
            catch (Exception ex)
            {
                if (!bSilent && projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.WebClientExpressErrorOnCreating, ex.Message));
            }
            finally
            {
                var isAborted = tokenSource.IsCancellationRequested;
                tokenSource.Dispose();
                if (wnd != null)
                    wnd.Close();

                if (!bSilent && projectManagerComponent.UIInterface != null)
                {
                    if(bHasErrors)
                    {
                        if (projectManagerComponent.UIInterface.ShowYesNo(
                            Properties.Resources.ErrorSavingSVGDocument, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                            projectManagerComponent.Workspace?.ShowSystemLog(); 
                    }

                    if(bChildAborted)
                    {
                        if (projectManagerComponent.UIInterface.ShowYesNo(
                            Properties.Resources.ExportingSVGSomeChildsHaveBeenSkipped, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                            projectManagerComponent.Workspace?.ShowSystemLog();
                    }

                    if (bSuccess)
                    {
                        //projectManagerComponent.UIInterface.ShowInformation(Properties.Resources.SVGSavingCompleted);
                        DeployServerRemote = projectManagerComponent.UIInterface.ShowYesNoCancel(Properties.Resources.WebHMIRemoteOrLocal, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                    }
                    else if (isAborted)
                    {
                        projectManagerComponent.UIInterface.ShowInformation(Properties.Resources.SVGSavingAborted);
                    }
                }
            }
        }
        bool ExportProjectWithChilds(UFProjectDocument document, bool isMainProject = false)
        {
            bool bSuccess = ExportProject(document, isMainProject);
            if (!bSuccess && isMainProject)
                return false;
            foreach (UFProjectDocument child in document.GetAllChilds())
            {
                var ret = ExportProjectWithChilds(child);
                bSuccess = bSuccess && ret;
            }
            return bSuccess;
        }

        bool ExportProject(UFProjectDocument document, bool isMainProject = false)
        {
            if (document == null)
                return false;
            string projectFolder = document.rootBase;
            currentProject = document.Title;
            if (document.fileSystemProviderBase != null)
            {
                if (isMainProject && !bSilent && projectManagerComponent.UIInterface != null)
                {
                    string message = Properties.Resources.ExportSVGProjectFromDB; 
                    if (projectManagerComponent.UIInterface.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                        projectFolder = projectManagerComponent.UIInterface.ShowBrowseFolderDialog(ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder"));
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    logGeneral.Info(string.Format(Properties.Resources.ExportSVGChildProjectFromDBHasBeenSkipped, document.Title));
                    bChildAborted = true;
                    return false;
                }
            }
            var screenManager = document.GetScreenDocumentManager();
            IEnumerable<string> resourcelist = projectManagerComponent.GetResourceList(document, document.GetScreenDocumentManager().TypeScheme, false);
            var svgModel = new UFInterfaces.Editors.SvgModel(document, tokenSource.Token, resourcelist, projectFolder);
            var bSuccess = (screenManager as IScreenManager).SaveToSvg(svgModel, document.IdentityColor);
            CleanCoreFiles(svgModel.ProjectFolder);
            if (svgModel.IsInError)
                bHasErrors = true;
            if (isMainProject)
                ExportedProjectFilePath = svgModel.ExportedProjectFilePath;
            return bSuccess;
        }

        void CleanCoreFiles(string projectPath)
        {
            if (String.IsNullOrEmpty(projectPath) || !System.IO.Directory.Exists(projectPath))
                return;

            if (XpoHelpers.XpoHelper.IsDataSource(document.FilePath))
                projectPath = String.Format("{0}\\{1}", projectPath, XpoHelpers.XpoHelper.GetDataSourceTitle(document.FilePath, onlytitle: true));

            projectManagerComponent.UriRisolver.GetListInstalledDocumentManagers().ForEach(im =>
            {
                try
                {
                    im.CleanCoreFiles(projectPath);
                }
                catch (Exception ex)
                {
                    logGeneral.Info(string.Format(Properties.Resources.ErrorCleaningSVGCoreFiles, im.TypeTitle, ex.Message));
                }
            });
        }

        void OnProgressStateChanged(object sender, UFInterfaces.ProgressStateChangedEventArgs e)
        {
            CurrentProgress = (int)(e.Value * 100);
            CurrentState = $"{currentProject} - {e.Description}" + " {0}%";
            progressBar.EditValue = currentProgress;
        }

        private void OnAbortBtnClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }

        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
#endregion
    }
}
