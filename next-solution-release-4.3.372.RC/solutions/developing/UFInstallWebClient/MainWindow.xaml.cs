using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Utilities.WPF;
using Utilities.ProgressDialog;
using System.Security.AccessControl;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Windows.Threading;
using Campari.Software;
using DevExpress.Xpf.Core;
using WPFUtilities;
using DataGridElementSettings;
using UFInstallWebClient.Controls;
using System.Security.Principal;
using System.DirectoryServices;
using DevExpress.Data.Helpers;
using UFInstallWebClient.ViewModel;

namespace UFInstallWebClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXTabbedWindow
    {
        #region Declarations
        IISViewModel viewModel;

        bool applicationPoolLoading;
        bool viewModelLoading;

        bool bLoaded;

        static string[] performanceGroupNames = { "Performance Log Users", "Performance Monitor Users" };
        #endregion
        
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            ShowIcon = false;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    viewModel = DataContext as IISViewModel;
                    ApplicationPropertiesHelper.SetProperty("CurrentSkin", viewModel.CurrentSkin);
                    ThemeHelper.SetTheme(this);

                    LoadViewModelSettings();
                    viewModel.PropertyChanged += ViewModel_PropertyChanged;
                    SubscribeToCallingProcessShutdown();
                }
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;

                    viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
            };
        }
        #endregion

        #region ViewModel Manager
        void LoadViewModelSettings()
        {
            if (!IIS7Manager.IISWebsite.IsIIS7Available())
                return;

            if (IIS7Manager.IISWebsite.Exist(viewModel.WebSite))
            {
                var website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                if (website != null && website.ExistApplication(viewModel.AliasName))
                {
                    try
                    {
                        viewModelLoading = true;
                        var application = website.OpenApplication(viewModel.AliasName);
                        var index = application.PhisicalPath.IndexOf(String.Format("\\{0}", viewModel.ProjectName), StringComparison.OrdinalIgnoreCase);
                        if (index != -1)
                            viewModel.DeployPath = application.PhisicalPath.Substring(0, index);
                        viewModel.ApplicationPoolName = application.ApplicationPoolName;

                        var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(String.Format("\\{0}", viewModel.AliasName), viewModel.WebSite);
                        System.Configuration.KeyValueConfigurationCollection settings = config.AppSettings.Settings;
                        foreach (System.Configuration.KeyValueConfigurationElement keyValueElement in settings)
                        {
                            if (keyValueElement.Key == "DisableStaticOptimization")
                            {
                                bool disableStaticOptimization;
                                if (bool.TryParse(keyValueElement.Value, out disableStaticOptimization))
                                    viewModel.StaticOptimization = !disableStaticOptimization;
                            }
                            else if (keyValueElement.Key == "RefreshPollingTime")
                            {
                                int refreshPollingTime;
                                if (int.TryParse(keyValueElement.Value, out refreshPollingTime))
                                    viewModel.RefreshPollingTime = refreshPollingTime;
                            }
                            else if (keyValueElement.Key == "RefreshPollingTimeCount")
                            {
                                int refreshPollingTimeCount;
                                if (int.TryParse(keyValueElement.Value, out refreshPollingTimeCount))
                                    viewModel.RefreshPollingTimeCount = refreshPollingTimeCount;
                            }
                            else if (keyValueElement.Key == "DelayBroadcaster")
                            {
                                int delayBroadcaster;
                                if (int.TryParse(keyValueElement.Value, out delayBroadcaster))
                                    viewModel.DelayBroadcaster = delayBroadcaster;
                            }
                            else if (keyValueElement.Key == "SessionTimeout")
                            {
                                int sessionTimeout;
                                if (int.TryParse(keyValueElement.Value, out sessionTimeout))
                                    viewModel.SessionTimeout = sessionTimeout;
                            }
                            else if (keyValueElement.Key == "ConcurrentRenderingPipeline")
                            {
                                int concurrentRenderingPipeline;
                                if (int.TryParse(keyValueElement.Value, out concurrentRenderingPipeline))
                                    viewModel.ConcurrentRenderingPipeline = concurrentRenderingPipeline;
                            }
                            else if (keyValueElement.Key == "LowResolution")
                            {
                                bool lowResolution;
                                if (bool.TryParse(keyValueElement.Value, out lowResolution))
                                    viewModel.LowResolution = lowResolution;
                            }
                            else if (keyValueElement.Key == "DisablePopupScreen")
                            {
                                bool disablePopupScreen;
                                if (bool.TryParse(keyValueElement.Value, out disablePopupScreen))
                                    viewModel.DisablePopupScreen = disablePopupScreen;
                            }
                            else if (keyValueElement.Key == "ShowProjectTitle")
                            {
                                bool showProjectTitle;
                                if (bool.TryParse(keyValueElement.Value, out showProjectTitle))
                                    viewModel.ShowProjectTitle = showProjectTitle;
                            }
                            else if (keyValueElement.Key == "DisableCreateNewDashboard")
                            {
                                bool disableCreateNewDashboard;
                                if (bool.TryParse(keyValueElement.Value, out disableCreateNewDashboard))
                                    viewModel.DisableCreateNewDashboard = disableCreateNewDashboard;
                            }
                            else if (keyValueElement.Key == "SwitchToViewerDashboard")
                            {
                                bool switchToViewerDashboard;
                                if (bool.TryParse(keyValueElement.Value, out switchToViewerDashboard))
                                    viewModel.SwitchToViewerDashboard = switchToViewerDashboard;
                            }
                            else if (keyValueElement.Key == "ShowHeader")
                            {
                                bool showHeader;
                                if (bool.TryParse(keyValueElement.Value, out showHeader))
                                    viewModel.ShowHeader = showHeader;
                            }
                            else if (keyValueElement.Key == "ShowScreenNavigator")
                            {
                                bool showScreenNavigator;
                                if (bool.TryParse(keyValueElement.Value, out showScreenNavigator))
                                    viewModel.ShowScreenNavigator = showScreenNavigator;
                            }
                            else if (keyValueElement.Key == "HideUserRegister")
                            {
                                bool hideUserRegister;
                                if (bool.TryParse(keyValueElement.Value, out hideUserRegister))
                                    viewModel.ShowUserRegisterLink = !hideUserRegister;
                            }
                            else if (keyValueElement.Key == "SoftwareRendering")
                            {
                                bool softwareRendering;
                                if (bool.TryParse(keyValueElement.Value, out softwareRendering))
                                    viewModel.SoftwareRendering = softwareRendering;
                            }
                            else if (keyValueElement.Key == "ProjectTitle")
                            {
                                if (keyValueElement.Value != null)
                                    viewModel.ProjectTitle = keyValueElement.Value;
                            }
                            else if (keyValueElement.Key == "LogoUrl")
                            {
                                if (keyValueElement.Value != null)
                                    viewModel.LogoUrl = keyValueElement.Value;
                            }
                            else if (keyValueElement.Key == "HideDashboardMenuItem")
                            {
                                bool hideDashboardMenuItem;
                                if (bool.TryParse(keyValueElement.Value, out hideDashboardMenuItem))
                                    viewModel.ShowDashboardMenuItem = !hideDashboardMenuItem;
                            }
                            else if (keyValueElement.Key == "HideAlarmMenuItem")
                            {
                                bool hideAlarmMenuItem;
                                if (bool.TryParse(keyValueElement.Value, out hideAlarmMenuItem))
                                    viewModel.ShowAlarmMenuItem = !hideAlarmMenuItem;
                            }
                            else if (keyValueElement.Key == "HideDataAnalisysMenuItem")
                            {
                                bool hideDataAnalisysMenuItem;
                                if (bool.TryParse(keyValueElement.Value, out hideDataAnalisysMenuItem))
                                    viewModel.ShowDataAnalisysMenuItem = !hideDataAnalisysMenuItem;
                            }
                            else if (keyValueElement.Key == "HideDataGridMenuItem")
                            {
                                bool hideDataGridMenuItem;
                                if (bool.TryParse(keyValueElement.Value, out hideDataGridMenuItem))
                                    viewModel.ShowDataGridMenuItem = !hideDataGridMenuItem;
                            }
                            else if (keyValueElement.Key == "HideReportMenuItem")
                            {
                                bool hideReportMenuItem;
                                if (bool.TryParse(keyValueElement.Value, out hideReportMenuItem))
                                    viewModel.ShowReportMenuItem = !hideReportMenuItem;
                            }
                        }

                        var list = new List<DataGridElement>();
                        var section = config.GetSection("DataGridRetriever") as DataGridRetrieverSection;
                        if (section != null)
                        {
                            foreach (DataGridElement element in section.DataGrids)
                                list.Add(element);
                            viewModel.DataGridSources = list.ToArray();
                        }

                        list.Clear();
                        section = config.GetSection("DataAnalisysRetriever") as DataGridRetrieverSection;
                        if (section != null)
                        {
                            foreach (DataGridElement element in section.DataGrids)
                                list.Add(element);
                            viewModel.DataAnalysisSources = list.ToArray();
                        }

                        list.Clear();
                        section = config.GetSection("DashBoardRetriever") as DataGridRetrieverSection;
                        if (section != null)
                        {
                            foreach (DataGridElement element in section.DataGrids)
                                list.Add(element);
                            viewModel.DashBoardSources = list.ToArray();
                        }

                        list.Clear();
                        section = config.GetSection("ReportRetriever") as DataGridRetrieverSection;
                        if (section != null)
                        {
                            foreach (DataGridElement element in section.DataGrids)
                                list.Add(element);
                            viewModel.ReportSources = list.ToArray();
                        }

                        viewModel.IsPublished = true;
                    }
                    finally
                    {
                        viewModelLoading = false;
                    }
                }
            }

            ViewModel_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("ApplicationPoolName"));
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ApplicationPoolName")
            {
                try
                {
                    applicationPoolLoading = true;
                    if (IIS7Manager.IISWebsite.IsIIS7Available() &&
                    IIS7Manager.IISAppPool.Exsit(viewModel.ApplicationPoolName))
                    {

                        var appPool = IIS7Manager.IISAppPool.OpenAppPool(viewModel.ApplicationPoolName);
                        viewModel.IndentityValue = appPool.ProcessModelIdentityType;
                        viewModel.IndentityName = appPool.ProcessModelUserName;
                        viewModel.IndentityPassword = appPool.ProcessModelPassword;
                    }
                }
                catch (Exception ex)
                { }
                finally
                {
                    applicationPoolLoading = false;
                }
            }
        }
        #endregion

        #region Command Handler

        private void OnPublish_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            AppendTraceMessage(Properties.Resources.DeployStarted, Colors.Green, clear: true, changeColor: true);
            if (viewModel != null && viewModel.IsValid)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    ProgressBarIndeterminate = true,
                    DialogText = Properties.Resources.IISWebDeploying,
                    IsCancellingEnabled = false
                };

                //start processing and submit the start value
                Exception exception = null;
                bool result = false;
                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    //using (new WaitCursor())
                    {
                        try
                        {
                            if (IIS7Manager.IISWebsite.IsIIS7Available())
                                result = IIS7_Deploy();
                            else
                                exception = new InvalidOperationException("Cannot connect to IIS7 manager.");
                                //result = IIS6_Deploy();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }
                });

                if (exception != null)
                {
                    AppendTraceMessage(String.Format(Properties.Resources.DeployFailure, exception), Colors.Red);
                }
                else if (!result)
                {
                    AppendTraceMessage(Properties.Resources.DeployCanceled, Colors.Yellow);
                }
                else
                {
                    AppendTraceMessage(Properties.Resources.DeployTerminated);
                }
            }
            else
            {
                AppendTraceMessage(Properties.Resources.InvalidOptions, Colors.Red);
            }
        }

        private void OnUpdate_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            AppendTraceMessage(Properties.Resources.UpdateStarted, Colors.Green, clear: true, changeColor:true);
            if (viewModel != null && viewModel.IsValid)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    ProgressBarIndeterminate = true,
                    DialogText = Properties.Resources.IISWebUpdating,
                    IsCancellingEnabled = false
                };

                //start processing and submit the start value
                Exception exception = null;
                bool result = false;
                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    //using (new WaitCursor())
                    {
                        try
                        {
                            if (IIS7Manager.IISWebsite.IsIIS7Available())
                                result = IIS7_Update();
                            else
                                exception = new InvalidOperationException("Cannot connect to IIS7 manager.");
                            //result = IIS6_Deploy();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }
                });

                if (exception != null)
                {
                    AppendTraceMessage(String.Format(Properties.Resources.UpdateFailure, exception), Colors.Red);
                }
                else if (!result)
                {
                    AppendTraceMessage(Properties.Resources.UpdateCanceled, Colors.Yellow);
                }
                else
                {
                    AppendTraceMessage(Properties.Resources.UpdateTerminated);
                }
            }
            else
            {
                AppendTraceMessage(Properties.Resources.InvalidOptions, Colors.Red);
            }
        }

        private void checkStaticOptimization_Checked(object sender, RoutedEventArgs e)
        {
            if (!viewModelLoading)
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (viewModel != null && viewModel.StaticOptimization)
                        MessageBox.Show(Properties.Resources.StaticOptimizationWarning, Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                });
            }
        }

        private void OnBrowse_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            AppendTraceMessage(Properties.Resources.OpenBrowserStarted, Colors.Green, clear: true, changeColor: true);
            if (viewModel != null)
            {
                
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    ProgressBarIndeterminate = true,
                    DialogText = Properties.Resources.IISBrowserStarting,
                    IsCancellingEnabled = false
                };

                //start processing and submit the start value
                Exception exception = null;
                bool result = false;
                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    try
                    {
                        //using (new WaitCursor())
                        {
                            if (IIS7Manager.IISWebsite.IsIIS7Available())
                                result = StartIIS7();
                            else
                                exception = new InvalidOperationException("Cannot connect to IIS7 manager.");
                                //result = StartIIS6();

                            if (result)
                            {
                                using (var process = Process.Start(viewModel.IISUrl))
                                {
                                    if (process != null)
                                        process.WaitForInputIdle();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });

                if (exception != null)
                {
                    AppendTraceMessage(String.Format(Properties.Resources.OpenBrowserFailed, exception), Colors.Red);
                }
                else if (!result)
                {
                    AppendTraceMessage(Properties.Resources.OpenBrowserCanceled, Colors.Yellow);
                }
                else if (result)
                {
                    AppendTraceMessage(Properties.Resources.OpenBrowserExecuted);
                }
            }
            else
            {
                AppendTraceMessage(Properties.Resources.InvalidOptions, Colors.Red);
            }
        }

        private void OnClose_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Application.Current.Shutdown();
        }

        private void DeployPath_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.SelectedPath = String.Format("{0}\\", viewModel.DeployPath);
            if (dialog.ShowDialog() == true)
                viewModel.DeployPath = dialog.SelectedPath;
        }

        private void LogoUrlEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.DefaultExt = "Portable Network Graphics (*.png)|*.png";
            dialog.Filter = Properties.Settings.Default.SelectImageFilter;
            dialog.InitialDirectory = String.Format("{0}\\", viewModel.DeployPath);
            if (dialog.ShowDialog() == true)
                viewModel.LogoUrl = dialog.FileName;
        }

        private void LogoUrlCancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.LogoUrl = null;
        }

        private void SetDefaultValues_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.SessionTimeout = Properties.Settings.Default.SessionTimeoutDefaultValue;
            viewModel.RefreshPollingTime = Properties.Settings.Default.RefreshPollingTimeDefaultValue;
            viewModel.RefreshPollingTimeCount = Properties.Settings.Default.RefreshPollingTimeCountDefaultValue;
            viewModel.DelayBroadcaster = Properties.Settings.Default.DelayBroadcasterDefaultValue;
            viewModel.ConcurrentRenderingPipeline = Properties.Settings.Default.ConcurrentRenderingPipelineValue;
            viewModel.LowResolution = Properties.Settings.Default.LowResolutionDefaultValue;
            viewModel.SoftwareRendering = Properties.Settings.Default.SoftwareRenderingDefaultValue;
        }

        private void Identity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (!applicationPoolLoading && 
                (IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType)txtIdentityValue.SelectedValue == IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.SpecificUser
                && String.IsNullOrEmpty(viewModel.IndentityName) && !ShowChangeUserDialog())
                    viewModel.IndentityValue = (IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType)e.RemovedItems[0];
        }

        private void IdentityButton_ChangeUser(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ShowChangeUserDialog();
        }

        private void IdentityButton_RemoveUser(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.IndentityValue = IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.ApplicationPoolIdentity;
        }

        private bool ShowChangeUserDialog()
        {
            using (Ookii.Dialogs.Wpf.CredentialDialog dialog = new Ookii.Dialogs.Wpf.CredentialDialog())
            {
                // The window title will not be used on Vista and later; there the title will always be "Windows Security".
                dialog.WindowTitle = Properties.Resources.ShowCredentialTitle;
                dialog.MainInstruction = Properties.Resources.ShowCredentialMainInstruction;
                dialog.Content = Properties.Resources.ShowCredentialContent;
                dialog.ShowSaveCheckBox = false;
                dialog.ShowUIForSavedCredentials = false;
                // The target is the key under which the credentials will be stored.
                // It is recommended to set the target to something following the "Company_Application_Server" pattern.
                // Targets are per user, not per application, so using such a pattern will ensure uniqueness.
                dialog.Target = App.Current.MainWindow.Title;
                bool bContinue = true;
                while (bContinue)
                {
                    if (dialog.ShowDialog() == true)
                    {
                        ProgressDialog dlg = new ProgressDialog()
                        {
                            AutoShowDelay = 0,
                            Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                            ProgressBarIndeterminate = true,
                            DialogText = Properties.Resources.ShowCredentialValidating,
                            IsCancellingEnabled = false
                        };

                        dlg.RunWorkerThread(null, (o, ev) =>
                        {
                            var userName = dialog.Credentials.UserName;
                            var password = dialog.Credentials.Password;

                            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                            {
                                var domainName = String.Empty;
                                if (userName.Contains('@'))
                                {
                                    domainName = userName.Substring(userName.IndexOf('@') + 1);
                                    userName = userName.Substring(0, userName.IndexOf('@'));
                                    
                                }
                                else if (userName.Contains('\\'))
                                {
                                    domainName = userName.Substring(0, userName.IndexOf('\\'));
                                    userName = userName.Substring(userName.IndexOf('\\') + 1);
                                }

                                using (PrincipalContext context = new PrincipalContext(
                                    String.IsNullOrEmpty(domainName) ? ContextType.Machine : ContextType.Domain,
                                    String.IsNullOrEmpty(domainName) ? System.Environment.MachineName : domainName))
                                {
                                    if (context.ValidateCredentials(userName, password))
                                    {
                                        viewModel.IndentityName = dialog.Credentials.UserName;
                                        viewModel.IndentityPassword = dialog.Credentials.Password;
                                        viewModel.IndentityValue = IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.SpecificUser;
                                        // Normally, you should verify if the credentials are correct before calling ConfirmCredentials.
                                        // ConfirmCredentials will save the credentials if and only if the user checked the save checkbox.
                                        // dialog.ConfirmCredentials(true);
                                        bContinue = false;
                                    }
                                }
                            }
                        });
                    }
                    else
                        return false;
                }
            }

            return true;
        }

        private void DataGridSourcesClear_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.DataGridSources = null;
        }

        private void DataGridSourcesEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var control = new ListDataSourceEditor(viewModel.DataGridSources);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.DataGridSourcesTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "WebClientDataGridSourcesEditor"
            };
            if (Dialog.ShowDialog() == true)
                viewModel.DataGridSources = control.CurrentDataSource;
        }

        private void DataAnalysisSourcesClear_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.DataAnalysisSources = null;
        }

        private void DataAnalysisSourcesEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var control = new ListDataSourceEditor(viewModel.DataAnalysisSources);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.DataAnalysisSourcesTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "WebClientDataAnalysisSourcesEditor"
            };
            if (Dialog.ShowDialog() == true)
                viewModel.DataAnalysisSources = control.CurrentDataSource;
        }

        private void DashBoardSourcesClear_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.DashBoardSources = null;
        }

        private void DashBoardSourcesEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var control = new ListDataSourceEditor(viewModel.DashBoardSources, useXpoProvider: true);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.DashBoardSourcesTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "WebClientDashBoardSourcesEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                viewModel.DashBoardSources = control.CurrentDataSource;
            }
        }

        private void ReportSourcesClear_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.ReportSources = null;
        }

        private void ReportSourcesEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var reportVM = new ReportViewModel(viewModel.Users, viewModel.Roles, viewModel.ReportList);
            var control = new ListReportEditor(viewModel.ReportSources, reportVM);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.ReportSourcesTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "WebClientReportSourcesEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                viewModel.ReportSources = control.CurrentDataSource;
            }
        }

        #endregion

        #region Methods

        private bool IIS7_Deploy()
        {
            if (IIS7Manager.IISWebsite.Exist(viewModel.WebSite))
            {
                var website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                if (website != null)
                {
#if DEBUG
                    string sourcePath = @"C:\Documents\Progetti\mbNExT_001";
#else
                    string sourcePath = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Properties.Settings.Default.WebClientPathSuffix);
#endif
                    string destPath = string.Format("{0}\\{1}", viewModel.DeployPath, viewModel.ProjectName);

                    string copyAppDataPath = null;

                    var webstatus = website.Status;
                    var application = website.OpenApplication(viewModel.AliasName);
                    try
                    {
                        if (application != null)
                        {
                            var appNewPool = IIS7Manager.IISAppPool.OpenAppPool(viewModel.ApplicationPoolName);
                            var appOldPool = IIS7Manager.IISAppPool.OpenAppPool(application.ApplicationPoolName);

                            var appOldPoolState = appOldPool != null ? appOldPool.State : IIS7Manager.IISWebsiteStatus.Unknown;

                            try
                            {
                                if ((appNewPool != null && appNewPool.State == IIS7Manager.IISWebsiteStatus.Started) ||
                                    (appOldPool != null && appOldPool.State == IIS7Manager.IISWebsiteStatus.Started))
                                {
                                    if (MessageBox.Show(String.Format(Properties.Resources.IISVirtualDirRunning.Replace("'newline'", Environment.NewLine),
                                        viewModel.AliasName, viewModel.WebSite), Properties.Resources.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                                        return false;

                                    if (appNewPool != null)
                                    {
                                        ChangeAppPoolState(appNewPool, IIS7Manager.IISWebsiteStatus.Stopped);
                                    }

                                    if (appOldPool != null)
                                    {
                                        ChangeAppPoolState(appOldPool, IIS7Manager.IISWebsiteStatus.Stopped);
                                    }
                                }

                                viewModel.IsPublished = false;

                                // check valid deploy path
                                int counter = 0;
                                while (System.IO.Directory.Exists(destPath) && !DirectoryHelper.CanRemoveDirectory(destPath))
                                    destPath = string.Format("{0}\\{1}.{2}", viewModel.DeployPath, viewModel.ProjectName, ++counter);

                                var origPath = string.Format("{0}\\{1}", viewModel.DeployPath, viewModel.ProjectName);
                                if (destPath != origPath)
                                {

                                    if (MessageBox.Show(String.Format(Properties.Resources.IISDeployPathInUse.Replace("'newline'", Environment.NewLine), origPath, destPath),
                                                        Properties.Resources.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                                        return false;
                                }

                                // try to remove existing physical directories link to the application
                                string[] virtualDirNames = application.EnumSubVirtualDirs();
                                foreach (var virtualDirName in virtualDirNames)
                                {
                                    var virtualDir = application.OpenSubVirtualDir(virtualDirName);
                                    if (System.IO.Directory.Exists(virtualDir.PhysicalPath) && DirectoryHelper.CanRemoveDirectory(virtualDir.PhysicalPath))
                                    {
                                        var appData = String.Format("{0}\\{1}", virtualDir.PhysicalPath, "App_Data");
                                        if (copyAppDataPath == null && System.IO.Directory.Exists(appData))
                                        {
                                            copyAppDataPath = String.Format("{0}{1}", System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                                            Utilities.DirectoryHelper.DirectoryCopy(appData, copyAppDataPath, false, true);
                                        }

                                        try
                                        {
                                            System.IO.Directory.Delete(virtualDir.PhysicalPath, true);
                                        }
                                        catch
                                        { }
                                    }
                                }

                                // remove existing application
                                website.DeleteApplication(viewModel.AliasName);
                                website.CommitChanges();
                            }
                            finally
                            {
                                if (application.ApplicationPoolName != viewModel.ApplicationPoolName)
                                {
                                    if (appOldPool != null && appOldPool.State != appOldPoolState)
                                    {
                                        ChangeAppPoolState(appOldPool, IIS7Manager.IISWebsiteStatus.Started);
                                    }
                                }
                            }
                        }

                        viewModel.IsPublished = false;

                        // copy files to physical path
                        PreparePhysicalPath(sourcePath, destPath);
                        if (copyAppDataPath != null && System.IO.Directory.Exists(copyAppDataPath))
                        {
                            var appData = String.Format("{0}\\{1}", destPath, "App_Data");
                            Utilities.DirectoryHelper.DirectoryCopy(copyAppDataPath, appData, false, true);
                        }
                        if (!IIS7Manager.IISAppPool.Exsit(viewModel.ApplicationPoolName))
                        {
                            // create application pool
                            IIS7Manager.IISAppPool.CreateAppPool(viewModel.ApplicationPoolName);
                        }

                        try
                        {
                            AddIndentityToPerformanceCounters();
                        }
                        catch (Exception ex)
                        {
                            AppendTraceMessage(String.Format(Properties.Resources.AddIndentityToPerformanceCountersError,
                                String.Format("IIS APPPOOL\\{0}", viewModel.ApplicationPoolName), 
                                ex.Message).Replace("--newline--", Environment.NewLine), Colors.Yellow);
                        }

                        // create virtual directory
                        website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                        website.CreateApplication(viewModel.AliasName, destPath, viewModel.ApplicationPoolName);
                        website.CommitChanges();

                        // change application pool settings
                        var appPool = IIS7Manager.IISAppPool.OpenAppPool(viewModel.ApplicationPoolName);
                        appPool.ManagedRuntimeVersion = "v4.0";
                        appPool.ManagedPipelineMode = IIS7Manager.IISAppPool.IISAppPoolManagedPipelineMode.Integrated;
                        appPool.ProcessModelIdentityType = viewModel.IndentityValue;
                        appPool.ProcessModelUserName = viewModel.IndentityName;
                        appPool.ProcessModelPassword = viewModel.IndentityPassword;
                        appPool.DisallowOverlappingRotation = true;
                        appPool.CommitChanges();
                        ChangeAppPoolState(appPool, IIS7Manager.IISWebsiteStatus.Started);

                        // change web.config file 
                        if (viewModel.WebClientType == WebClientType.Html5)
                            ChangeWebConfigFile(destPath);

                        // change folder permissions
                        GrantFolderPermissions();

                        viewModel.IsPublished = true;

                        if (!AreAllRequiredComponentsInstalled())
                        {
                            AppendTraceMessage(Properties.Resources.MissingIISComponents, Colors.Yellow);
                        }

                        return true;
                    }
                    finally 
                    {
                        try
                        {
                            if (copyAppDataPath != null)
                                System.IO.Directory.Delete(copyAppDataPath, true);
                        }
                        catch
                        { }

                        if (website.Status != webstatus && webstatus == IIS7Manager.IISWebsiteStatus.Started)
                        {
                            ChangeWebSiteStatus(website, IIS7Manager.IISWebsiteStatus.Started);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteOpenError, viewModel.WebSite));
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteFindError, viewModel.WebSite));
            }
        }

        private bool IIS7_Update()
        {
            if (IIS7Manager.IISWebsite.Exist(viewModel.WebSite))
            {
                var website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                if (website != null && website.ExistApplication(viewModel.AliasName))
                {
                    // try to remove existing physical directories link to the application
                    var application = website.OpenApplication(viewModel.AliasName);
                    string[] virtualDirNames = application.EnumSubVirtualDirs();
                    if (virtualDirNames.Length > 0)
                    {
                        var virtualDir = application.OpenSubVirtualDir(virtualDirNames[0]);
                        var projectPath = string.Format("{0}\\{1}", virtualDir.PhysicalPath, Properties.Settings.Default.ProjectPathSuffix);

                        var appPool = IIS7Manager.IISAppPool.OpenAppPool(application.ApplicationPoolName);
                        var appPoolState = appPool != null ? appPool.State : IIS7Manager.IISWebsiteStatus.Unknown;
                        try
                        {
                            if (appPool != null)
                            {
                                if (appPoolState == IIS7Manager.IISWebsiteStatus.Started)
                                {
                                    if (MessageBox.Show(String.Format(Properties.Resources.IISVirtualDirRunning.Replace("'newline'", Environment.NewLine),
                                        viewModel.AliasName, viewModel.WebSite), Properties.Resources.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                                        return false;
                                }

                                ChangeAppPoolState(appPool, IIS7Manager.IISWebsiteStatus.Stopped);

                                // copy project files
                                if (System.IO.Directory.Exists(projectPath))
                                {
                                    System.IO.Directory.Delete(projectPath, true);
                                }
                                CopyProjectResources(projectPath);
                            }
                        }
                        finally
                        {
                            if (appPool != null && appPool.State != appPoolState)
                            {
                                ChangeAppPoolState(appPool, IIS7Manager.IISWebsiteStatus.Started);
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteOpenError, viewModel.WebSite));
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteFindError, viewModel.WebSite));
            }
        }

        private void PreparePhysicalPath(string sourcePath, string destPath)
        {
            // copy files to virtual path
            if (System.IO.Directory.Exists(destPath))
            {
                //if (!DirectoryHelper.CanRemoveDirectory(destPath))
                //{
                //    MessageBox.Show(String.Format(Properties.Resources.IISDeployPathInUse.Replace("'newline'", Environment.NewLine), destPath));
                //    return String.Empty;
                //}
                System.IO.Directory.Delete(destPath, true);
            }
            CopyProjectResources(string.Format("{0}\\{1}", destPath, Properties.Settings.Default.ProjectPathSuffix));
            DirectoryHelper.DirectoryCopy(sourcePath, destPath, true, true);
            var appData = string.Format("{0}\\{1}", destPath, "App_Data");
            if (!System.IO.Directory.Exists(appData))
                System.IO.Directory.CreateDirectory(appData);
        }

        private bool CopyProjectResources(string destPath)
        {
            if (viewModel.IsDataSourceProjectBase)
                return true; // Nothing to do for data source project base.

            var sourcePath = System.IO.Path.GetDirectoryName(viewModel.UriProjectPath);
            // copy files to destination path
            if (System.IO.Directory.Exists(destPath))
            {
                if (!DirectoryHelper.CanRemoveDirectory(destPath))
                {
                    MessageBox.Show(String.Format(Properties.Resources.IISCannotRemovePath, destPath));
                    return false;
                }
                System.IO.Directory.Delete(destPath, true);
            }

            System.IO.Directory.CreateDirectory(destPath);
            System.IO.File.Copy(viewModel.UriProjectPath,
                String.Format("{0}\\{1}", destPath, System.IO.Path.GetFileName(viewModel.UriProjectPath)));
            DirectoryHelper.DirectoryCopy(String.Format("{0}\\{1}", sourcePath, viewModel.ProjectName), 
                String.Format("{0}\\{1}", destPath, viewModel.ProjectName), true, false, true);
            if (viewModel.UriSpecialFolders != null)
            {
                foreach (var path in viewModel.UriSpecialFolders)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        var combinedDest = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(path));
                        DirectoryHelper.DirectoryCopy(path, combinedDest, copySubDirs: true, continueOnError: true);
                    }
                }
            }

            if (viewModel.UriChildProjects != null)
            {
                string deployBasePath = string.Format("{0}\\{1}", viewModel.DeployPath, viewModel.ProjectName);
                foreach (var childPath in viewModel.UriChildProjects)
                {
                    if (System.IO.Directory.Exists(childPath))
                    {
                        var sourceUri = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
                        var childUri = new Uri(childPath, UriKind.RelativeOrAbsolute);

                        var relativeUri = sourceUri.MakeRelativeUri(childUri);
                        var checkUri = new Uri(relativeUri.GetPathString(), UriKind.RelativeOrAbsolute);
                        if (checkUri.IsAbsoluteUri || checkUri.GetPathString().StartsWith("."))
                            continue;

                        var relativePath = relativeUri.GetPathString().Replace("/", "\\");
                        var index = relativePath.ToLower().IndexOf(String.Format("{0}\\", viewModel.ProjectName.ToLower()));
                        if (index == 0)
                            relativePath = relativePath.Substring(viewModel.ProjectName.Length + 1);
                        var combinedDest = System.IO.Path.Combine(index == 0 ? destPath : deployBasePath, relativePath);
                        if (System.IO.Directory.Exists(combinedDest))
                        {
                            if (!DirectoryHelper.CanRemoveDirectory(combinedDest))
                            {
                                MessageBox.Show(String.Format(Properties.Resources.IISCannotRemovePath, combinedDest));
                                return false;
                            }
                            System.IO.Directory.Delete(combinedDest, true);
                        }

                        DirectoryHelper.DirectoryCopy(childPath, combinedDest, copySubDirs: true, continueOnError: true);
                    }
                }
            }
            return true;
        }

        void AddIndentityToPerformanceCounters()
        {
            try
            { 
                using (PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName))
                {
                    string sid = null;
                    var account = new System.Security.Principal.NTAccount(String.Format("IIS APPPOOL\\{0}", viewModel.ApplicationPoolName));
                    try
                    {
                        sid = account.Translate(typeof(SecurityIdentifier)).Value;
                    }
                    catch (IdentityNotMappedException)
                    { }

                    if (sid != null)
                    {
                        foreach (var groupName in performanceGroupNames)
                        {
                            using (var group = GroupPrincipal.FindByIdentity(context, groupName))
                            {
                                if (group != null && !group.Members.Contains(context, IdentityType.Sid, sid))
                                {
                                    group.Members.Add(context, IdentityType.Sid, sid);
                                    group.Save();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                String userPath = string.Format("WinNT://{0}/IIS APPPOOL/{1}", Environment.MachineName, viewModel.ApplicationPoolName);
                foreach (var groupName in performanceGroupNames)
                {
                    var groupPath = string.Format("WinNT://{0}/{1}", Environment.MachineName, groupName);
                    using (DirectoryEntry userGroup = new DirectoryEntry(groupPath))
                    {
                        try
                        {
                            userGroup.Invoke("Remove", userPath);
                        }
                        catch
                        { }

                        userGroup.Invoke("Add", userPath);
                        userGroup.CommitChanges();
                    }
                }
            }
        }

        private void GrantFolderPermissions()
        {
            // Project Folder
            string projectPath = string.Format("{0}\\{1}\\{2}\\", viewModel.DeployPath, viewModel.ProjectName, Properties.Settings.Default.ProjectPathSuffix);
            if (System.IO.Directory.Exists(projectPath))
            {
                GrantFolderPermissions(projectPath, FileSystemRights.ReadData, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit);
            }

            // App_Data Folder
            string projectAppDataPath = string.Format("{0}\\{1}\\{2}\\", viewModel.DeployPath, viewModel.ProjectName, Properties.Settings.Default.AppDataPathSuffix);
            if (!System.IO.Directory.Exists(projectAppDataPath))
            {
                System.IO.Directory.CreateDirectory(projectAppDataPath);
            }
            GrantFolderPermissions(projectAppDataPath, FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.CreateFiles | FileSystemRights.Delete, InheritanceFlags.ObjectInherit);

            // Project Log Folder
            string projectLogPath = string.Format("{0}\\{1}\\{2}\\{3}", viewModel.DeployPath, viewModel.ProjectName, 
                Properties.Settings.Default.ProjectPathSuffix, Properties.Settings.Default.LogPathSuffix);
            if (!System.IO.Directory.Exists(projectLogPath))
            {
                System.IO.Directory.CreateDirectory(projectLogPath);
            }
            GrantFolderPermissions(projectLogPath, FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.CreateFiles | FileSystemRights.Delete, InheritanceFlags.ObjectInherit);
        }

        private void GrantFolderPermissions(string folderName, FileSystemRights rights)
        {
            GrantFolderPermissions(folderName, rights, InheritanceFlags.None, PropagationFlags.None);
        }

        private void GrantFolderPermissions(string folderName, FileSystemRights rights, InheritanceFlags inheritanceFlags)
        {
            GrantFolderPermissions(folderName, rights, inheritanceFlags, PropagationFlags.None);
        }

        private void GrantFolderPermissions(string folderName, FileSystemRights rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
        {
            try
            {
                System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(folderName);
                System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                    String.Format("IIS APPPOOL\\{0}", viewModel.ApplicationPoolName),
                    rights, inheritanceFlags, propagationFlags, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception ex)
            {
                AppendTraceMessage(String.Format(Properties.Resources.GrantFolderPermissionsWarning, folderName, ex.Message), Colors.Yellow);
            }
        }

        private void ChangeAppPoolState(IIS7Manager.IISAppPool appPool, IIS7Manager.IISWebsiteStatus newState, long timeout = 30000)
        {
            var dtTimeOut = DateTime.UtcNow.AddMilliseconds(timeout);
            while (appPool.State != newState)
            {
                try
                {
                    switch (newState)
                    {
                        case IIS7Manager.IISWebsiteStatus.Started: appPool.Start(); break;
                        case IIS7Manager.IISWebsiteStatus.Stopped: appPool.Stop(); break;
                        default: throw new ArgumentOutOfRangeException("Invalid value for 'IISWebsiteStatus' parameter");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ChangeAppPoolState '{0}'", ex.Message);
                }

                if (dtTimeOut < DateTime.UtcNow)
                    break;

                WaitForPriority.DoEventsSync();
            }

            if (appPool.State != newState)
            {
                switch (newState)
                {
                    case IIS7Manager.IISWebsiteStatus.Started:
                        AppendTraceMessage(String.Format(Properties.Resources.IISAppPoolStartedError, appPool.Name), Colors.Yellow);
                        break;
                    default:
                        AppendTraceMessage(String.Format(Properties.Resources.IISAppPoolStopedError, appPool.Name), Colors.Yellow);
                        break;
                }
            }
        }

        private void ChangeWebSiteStatus(IIS7Manager.IISWebsite webSite, IIS7Manager.IISWebsiteStatus newStatus, long timeout = 30000)
        {
            var dtTimeOut = DateTime.UtcNow.AddMilliseconds(timeout);
            while (webSite.Status != newStatus)
            {
                try
                {
                    switch (newStatus)
                    {
                        case IIS7Manager.IISWebsiteStatus.Started: webSite.Start(); break;
                        case IIS7Manager.IISWebsiteStatus.Stopped: webSite.Stop(); break;
                        default: throw new ArgumentOutOfRangeException("Invalid value for 'IISWebsiteStatus' parameter");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ChangeWebSiteStatus '{0}'", ex.Message);
                }

                if (dtTimeOut < DateTime.UtcNow)
                    break;

                WaitForPriority.DoEventsSync();
            }

            if (webSite.Status != newStatus)
            {
                switch (newStatus)
                {
                    case IIS7Manager.IISWebsiteStatus.Started:
                        AppendTraceMessage(String.Format(Properties.Resources.IISWebSiteStartedError, webSite.Name), Colors.Yellow);
                        break;
                    default:
                        AppendTraceMessage(String.Format(Properties.Resources.IISWebSiteStopedError, webSite.Name), Colors.Yellow);
                        break;
                }
            }
        }

        private void ChangeWebConfigFile(string path)
        {
            // change 'appSettings' entries inside the web.config file
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(String.Format("\\{0}", viewModel.AliasName), viewModel.WebSite);
            System.Configuration.KeyValueConfigurationCollection settings = config.AppSettings.Settings;
            bool bFoundUri = false;
            bool bFoundTheme = false;
            bool bFoundClientSessionName = false;
            bool bFoundStaticOptimization = false;
            bool bFoundRefreshPollingTime = false;
            bool bFoundRefreshPollingTimeCount = false;
            bool bFoundDelayBroadcaster = false;
            bool bFoundSessionTimeout = false;
            bool bFoundConcurrentRenderingPipeline = false;
            bool bFoundLowResolution = false;
            bool bFoundDisablePopupScreen = false;
            bool bFoundShowProjectTitle = false;
            bool bFoundDisableCreateNewDashboard = false;
            bool bFoundSwitchToViewerDashboard = false;
            bool bFoundShowHeader = false;
            bool bFoundShowScreenNavigator = false;
            bool bFoundHideUserRegister = false;
            bool bFoundSoftwareRendering = false;
            bool bFoundProjectTitle = false;
            bool bFoundLogoUrl = false;
            bool bFoundHideDashboardMenuItem = false;
            bool bFoundHideDataAnalysisMenuItem = false;
            bool bFoundHideAlarmMenuItem = false;
            bool bFoundHideDataGridMenuItem = false;
            bool bFoundHideReportMenuItem = false;

            string projectUri;
            if (viewModel.IsDataSourceProjectBase)
                projectUri = viewModel.UriProjectPath;
            else
                projectUri = String.Format("{0}\\{1}\\{2}", path, Properties.Settings.Default.ProjectPathSuffix, System.IO.Path.GetFileName(viewModel.UriProjectPath));

            foreach (System.Configuration.KeyValueConfigurationElement keyValueElement in settings)
            {
                if (keyValueElement.Key == "Uri")
                {
                    //Debug.WriteLine("change {0}: Uri {1}, new {2}", config.FilePath, keyValueElement.Value, viewModel.UriProjectPath);
                    keyValueElement.Value = projectUri;
                    bFoundUri = true;
                }
                else if (keyValueElement.Key == "Theme")
                {
                    keyValueElement.Value = viewModel.ApplicationTheme;
                    bFoundTheme = true;
                }
                else if (keyValueElement.Key == "ClientSessionName")
                {
                    keyValueElement.Value = viewModel.AliasName;
                    bFoundClientSessionName = true;
                }
                else if (keyValueElement.Key == "DisableStaticOptimization")
                {
                    keyValueElement.Value = !viewModel.StaticOptimization ? bool.TrueString : bool.FalseString;
                    bFoundStaticOptimization = true;
                }
                else if (keyValueElement.Key == "RefreshPollingTime")
                {
                    keyValueElement.Value = viewModel.RefreshPollingTime.ToString();
                    bFoundRefreshPollingTime = true;
                }
                else if (keyValueElement.Key == "RefreshPollingTimeCount")
                {
                    keyValueElement.Value = viewModel.RefreshPollingTimeCount.ToString();
                    bFoundRefreshPollingTimeCount = true;
                }
                else if (keyValueElement.Key == "DelayBroadcaster")
                {
                    keyValueElement.Value = viewModel.DelayBroadcaster.ToString();
                    bFoundDelayBroadcaster = true;
                }
                else if (keyValueElement.Key == "SessionTimeout")
                {
                    keyValueElement.Value = viewModel.SessionTimeout.ToString();
                    bFoundSessionTimeout = true;
                }
                else if (keyValueElement.Key == "ConcurrentRenderingPipeline")
                {
                    keyValueElement.Value = viewModel.ConcurrentRenderingPipeline.ToString();
                    bFoundConcurrentRenderingPipeline = true;
                }
                else if (keyValueElement.Key == "LowResolution")
                {
                    keyValueElement.Value = viewModel.LowResolution ? bool.TrueString : bool.FalseString;
                    bFoundLowResolution = true;
                }
                else if (keyValueElement.Key == "DisablePopupScreen")
                {
                    keyValueElement.Value = viewModel.DisablePopupScreen ? bool.TrueString : bool.FalseString;
                    bFoundDisablePopupScreen = true;
                }
                else if (keyValueElement.Key == "ShowProjectTitle")
                {
                    keyValueElement.Value = viewModel.ShowProjectTitle ? bool.TrueString : bool.FalseString;
                    bFoundShowProjectTitle = true;
                }
                else if (keyValueElement.Key == "DisableCreateNewDashboard")
                {
                    keyValueElement.Value = viewModel.DisableCreateNewDashboard ? bool.TrueString : bool.FalseString;
                    bFoundDisableCreateNewDashboard = true;

                }
                else if (keyValueElement.Key == "SwitchToViewerDashboard")
                {
                    keyValueElement.Value = viewModel.SwitchToViewerDashboard ? bool.TrueString : bool.FalseString;
                    bFoundSwitchToViewerDashboard = true;
                }
                else if (keyValueElement.Key == "ShowHeader")
                {
                    keyValueElement.Value = viewModel.ShowHeader ? bool.TrueString : bool.FalseString;
                    bFoundShowHeader = true;
                }
                else if (keyValueElement.Key == "ShowScreenNavigator")
                {
                    keyValueElement.Value = viewModel.ShowScreenNavigator ? bool.TrueString : bool.FalseString;
                    bFoundShowScreenNavigator = true;
                }
                else if (keyValueElement.Key == "HideUserRegister")
                {
                    keyValueElement.Value = !viewModel.ShowUserRegisterLink ? bool.TrueString : bool.FalseString;
                    bFoundHideUserRegister = true;
                }
                else if (keyValueElement.Key == "SoftwareRendering")
                {
                    keyValueElement.Value = viewModel.SoftwareRendering ? bool.TrueString : bool.FalseString;
                    bFoundSoftwareRendering = true;
                }
                else if (keyValueElement.Key == "ProjectTitle")
                {
                    keyValueElement.Value = viewModel.ProjectTitle;
                    bFoundProjectTitle = true;
                }
                else if (keyValueElement.Key == "LogoUrl")
                {
                    keyValueElement.Value = viewModel.LogoUrl;
                    bFoundLogoUrl = true;
                }
                else if (keyValueElement.Key == "HideDashboardMenuItem")
                {
                    bool hideDashboardMenuItem;
                    if (bool.TryParse(keyValueElement.Value, out hideDashboardMenuItem))
                        viewModel.ShowDashboardMenuItem = !hideDashboardMenuItem;
                }
                else if (keyValueElement.Key == "HideAlarmMenuItem")
                {
                    bool hideAlarmMenuItem;
                    if (bool.TryParse(keyValueElement.Value, out hideAlarmMenuItem))
                        viewModel.ShowAlarmMenuItem = !hideAlarmMenuItem;
                }
                else if (keyValueElement.Key == "HideDataAnalisysMenuItem")
                {
                    bool hideDataAnalisysMenuItem;
                    if (bool.TryParse(keyValueElement.Value, out hideDataAnalisysMenuItem))
                        viewModel.ShowDataAnalisysMenuItem = !hideDataAnalisysMenuItem;
                }
                else if (keyValueElement.Key == "HideDataGridMenuItem")
                {
                    bool hideDataGridMenuItem;
                    if (bool.TryParse(keyValueElement.Value, out hideDataGridMenuItem))
                        viewModel.ShowDataGridMenuItem = !hideDataGridMenuItem;
                }
                else if (keyValueElement.Key == "HideReportMenuItem")
                {
                    bool hideReportMenuItem;
                    if (bool.TryParse(keyValueElement.Value, out hideReportMenuItem))
                        viewModel.ShowReportMenuItem = !hideReportMenuItem;
                }
            }

            if (!bFoundUri)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("Uri", projectUri));
            if (!bFoundTheme)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("Theme", viewModel.ApplicationTheme));
            if (!bFoundClientSessionName)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ClientSessionName", viewModel.AliasName));
            if (!bFoundStaticOptimization)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("DisableStaticOptimization", !viewModel.StaticOptimization ? bool.TrueString : bool.FalseString));
            if (!bFoundRefreshPollingTime)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("RefreshPollingTime", viewModel.RefreshPollingTime.ToString()));
            if (!bFoundRefreshPollingTimeCount)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("RefreshPollingTimeCount", viewModel.RefreshPollingTimeCount.ToString()));
            if (!bFoundDelayBroadcaster)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("DelayBroadcaster", viewModel.DelayBroadcaster.ToString()));
            if (!bFoundSessionTimeout)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("SessionTimeout", viewModel.SessionTimeout.ToString()));
            if (!bFoundConcurrentRenderingPipeline)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ConcurrentRenderingPipeline", viewModel.ConcurrentRenderingPipeline.ToString()));
            if (!bFoundLowResolution)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("LowResolution", viewModel.LowResolution ? bool.TrueString : bool.FalseString));
            if (!bFoundDisablePopupScreen)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("DisablePopupScreen", viewModel.DisablePopupScreen ? bool.TrueString : bool.FalseString));
            if (!bFoundShowProjectTitle)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ShowProjectTitle", viewModel.ShowProjectTitle ? bool.TrueString : bool.FalseString));
            if (!bFoundDisableCreateNewDashboard)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("DisableCreateNewDashboard", viewModel.DisableCreateNewDashboard ? bool.TrueString : bool.FalseString));
            if (!bFoundSwitchToViewerDashboard)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("SwitchToViewerDashboard", viewModel.SwitchToViewerDashboard ? bool.TrueString : bool.FalseString));
            if (!bFoundShowHeader)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ShowHeader", viewModel.ShowHeader ? bool.TrueString : bool.FalseString));
            if (!bFoundShowScreenNavigator)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ShowScreenNavigator", viewModel.ShowScreenNavigator ? bool.TrueString : bool.FalseString));
            if (!bFoundHideUserRegister)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideUserRegister", !viewModel.ShowUserRegisterLink ? bool.TrueString : bool.FalseString));
            if (!bFoundSoftwareRendering)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("SoftwareRendering", viewModel.SoftwareRendering ? bool.TrueString : bool.FalseString));
            if (!bFoundProjectTitle)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("ProjectTitle", viewModel.ProjectTitle));
            if (!bFoundLogoUrl)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("LogoUrl", viewModel.LogoUrl));
            if (!bFoundHideDashboardMenuItem)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideDashboardMenuItem", !viewModel.ShowDashboardMenuItem ? bool.TrueString : bool.FalseString));
            if (!bFoundHideDataAnalysisMenuItem)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideDataAnalisysMenuItem", !viewModel.ShowDataAnalisysMenuItem ? bool.TrueString : bool.FalseString));
            if (!bFoundHideAlarmMenuItem)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideAlarmMenuItem", !viewModel.ShowAlarmMenuItem ? bool.TrueString : bool.FalseString));
            if (!bFoundHideDataGridMenuItem)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideDataGridMenuItem", !viewModel.ShowDataGridMenuItem ? bool.TrueString : bool.FalseString));
            if (!bFoundHideReportMenuItem)
                settings.Add(new System.Configuration.KeyValueConfigurationElement("HideReportMenuItem", !viewModel.ShowReportMenuItem ? bool.TrueString : bool.FalseString));

            // update ApplicationName entry inside the web.config file
            {
                dynamic section = (System.Web.Configuration.MembershipSection)config.GetSection("system.web/membership");
                if (section != null)
                {
                    foreach (System.Configuration.ProviderSettings providerSettings in section.Providers)
                    {
                        if (!String.IsNullOrEmpty(viewModel.ApplicationName))
                        {
                            if (providerSettings.Parameters.AllKeys.Contains("applicationName"))
                                providerSettings.Parameters["applicationName"] = viewModel.ApplicationName;
                        }
                        if (viewModel.MaxInvalidPasswordAttempts > 0)
                        {
                            if (providerSettings.Parameters.AllKeys.Contains("maxInvalidPasswordAttempts"))
                                providerSettings.Parameters["maxInvalidPasswordAttempts"] = viewModel.MaxInvalidPasswordAttempts.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(viewModel.ApplicationName))
                {
                    section = (System.Web.Configuration.ProfileSection)config.GetSection("system.web/profile");
                    if (section != null)
                    {
                        foreach (System.Configuration.ProviderSettings providerSettings in section.Providers)
                        {
                            if (providerSettings.Parameters.AllKeys.Contains("applicationName"))
                                providerSettings.Parameters["applicationName"] = viewModel.ApplicationName;
                        }
                    }

                    section = (System.Web.Configuration.RoleManagerSection)config.GetSection("system.web/roleManager");
                    if (section != null)
                    {
                        foreach (System.Configuration.ProviderSettings providerSettings in section.Providers)
                        {
                            if (providerSettings.Parameters.AllKeys.Contains("applicationName"))
                                providerSettings.Parameters["applicationName"] = viewModel.ApplicationName;
                        }
                    }
                }
            }

            // update AutoLogoutSeconds entry inside the web.config file
            //if (viewModel.AutoLogoutSeconds > 0)
            //{
            //    var section = (System.Web.Configuration.AuthenticationSection)config.GetSection("system.web/authentication");
            //    if (section != null && section.Forms != null)
            //    {
            //        section.Forms.Timeout = TimeSpan.FromSeconds(Math.Max(viewModel.AutoLogoutSeconds, 60));
            //    }
            //}

            // update Authorization Rules entries inside the web.config file
            {
                var section = (System.Web.Configuration.AuthorizationSection)config.GetSection("system.web/authorization");
                if (section != null && section.Rules != null && section.Rules.Count > 0 && 
                    section.Rules[0].Users != null && section.Rules[0].Users.Count > 0)
                {
                    var rule = section.Rules[0];
                    rule.Action = viewModel.EnableUserManager ? System.Web.Configuration.AuthorizationRuleAction.Deny : System.Web.Configuration.AuthorizationRuleAction.Allow;
                    section.Rules[0].Users[0] = viewModel.EnableUserManager ? "?" : "*";
                }
            }

            // DataGridRetriever
            {
                var section = config.GetSection("DataGridRetriever") as DataGridRetrieverSection;
                if (section == null)
                    config.Sections.Add("DataGridRetriever", new DataGridRetrieverSection());
                section = config.GetSection("DataGridRetriever") as DataGridRetrieverSection;
                section.DataGrids.Clear();
                foreach (var element in viewModel.DataGridSources)
                    section.DataGrids.Add(element);
            }

            // DataAnalisysRetriever
            {
                var section = config.GetSection("DataAnalisysRetriever") as DataGridRetrieverSection;
                if (section == null)
                    config.Sections.Add("DataAnalisysRetriever", new DataGridRetrieverSection());
                section = config.GetSection("DataAnalisysRetriever") as DataGridRetrieverSection;
                section.DataGrids.Clear();
                foreach (var element in viewModel.DataAnalysisSources)
                    section.DataGrids.Add(element);
            }

            // DashBoardRetriever
            {
                var section = config.GetSection("DashBoardRetriever") as DataGridRetrieverSection;
                if (section == null)
                    config.Sections.Add("DashBoardRetriever", new DataGridRetrieverSection());
                section = config.GetSection("DashBoardRetriever") as DataGridRetrieverSection;
                section.DataGrids.Clear();
                foreach (var element in viewModel.DashBoardSources)
                    section.DataGrids.Add(element);
            }

            // ReportRetriever
            {
                var section = config.GetSection("ReportRetriever") as DataGridRetrieverSection;
                if (section == null)
                    config.Sections.Add("ReportRetriever", new DataGridRetrieverSection());
                section = config.GetSection("ReportRetriever") as DataGridRetrieverSection;
                section.DataGrids.Clear();
                foreach (var element in viewModel.ReportSources)
                    section.DataGrids.Add(element);
            }

            // when web application is running the config.FilePath is different from new virtual path
            // see: https://support.progea.com/Products/default.asp?11060
            string tmpFile = System.IO.Path.GetTempFileName();
            string destFile = String.Format("{0}\\web.config", path);
            config.SaveAs(tmpFile, System.Configuration.ConfigurationSaveMode.Full);
            if (System.IO.File.Exists(destFile))
                System.IO.File.Delete(destFile);
            System.IO.File.Copy(tmpFile, destFile);
        }
        private bool StartIIS7()
        {
            if (IIS7Manager.IISWebsite.Exist(viewModel.WebSite))
            {
                var website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                if (website != null)
                {
                    bool started = website.Status == IIS7Manager.IISWebsiteStatus.Started;
                    if (started || MessageBox.Show(
                            String.Format(Properties.Resources.IISWebSiteNotStarted, viewModel.WebSite),
                            Properties.Resources.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (!started)
                        {
                            website.Start();
                        }

                        return true;
                    }
                }
                else
                {
                    throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteOpenError, viewModel.WebSite));
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(Properties.Resources.IISWebSiteFindError, viewModel.WebSite));
            }

            return false;
        }

        private void AppendTraceMessage(String message)
        {
            AppendTraceMessage(message, Colors.Green, false, false);
        }

        private void AppendTraceMessage(String message, Color color)
        {
            AppendTraceMessage(message, color, false, true);
        }

        private void AppendTraceMessage(String message, Color color, bool clear)
        {
            AppendTraceMessage(message, color, clear, false);
        }

        private void AppendTraceMessage(String message, Color color, bool clear, bool changeColor)
        {
            txtOutput.Dispatcher.InvokeIfRequired(() => 
            {
                if (!clear)
                    txtOutput.Text += String.Format("\n{0}", message);
                else
                    txtOutput.Text = message;
                if (changeColor)
                    txtOutput.Foreground = new SolidColorBrush(color);
                Scroll.ScrollToBottom();
            });
        }

        private bool AreAllRequiredComponentsInstalled()
        {
            if (!InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx45))
                return false;

            return true;
        }

        void SubscribeToCallingProcessShutdown()
        {
            if (viewModel.CallingProcessId > 0)
            {
                try
                {
                    var process = Process.GetProcessById(viewModel.CallingProcessId);
                    if (process != null)
                    {
                        process.EnableRaisingEvents = true;
                        process.Exited += (o, e) =>
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                                (Action)(() => Application.Current.MainWindow.Close() ));
                        };
                    }
                }
                catch { }
            }
        }

        #endregion
    }

    
}
