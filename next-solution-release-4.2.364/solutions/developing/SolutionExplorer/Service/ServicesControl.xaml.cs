using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceProcess;
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
using System.Windows.Threading;
using System.Xml;
using UFInterfaces.Service;
using UFProjectManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.ProgressDialog;
using Utilities.WPF;
using ViewModelLib;

namespace UFProjectManager.Service
{
    /// <summary>
    /// Interaction logic for ServicesControl.xaml
    /// </summary>
    public partial class ServicesControl : UserControl
    {
        #region Declarations
        readonly ObservableCollection<ServiceViewModel> servicesViewModel;
        readonly List<String> projectServiceNames;
        List<ServiceViewModel> discoveredServices;
        DispatcherTimer refreshTimer;
        readonly IDocument parent;

        readonly static string WebHMI_ServiceName = System.IO.Path.GetFileNameWithoutExtension(Properties.Settings.Default.CheckedAssemblyName_WebHMI);
        #endregion

        #region Constructors
        public ServicesControl(IList<IServiceControl> servicesControl, IDocument parent)
        {
            InitializeComponent();
            
            this.parent = parent;
            if (servicesControl != null && servicesControl.Count > 0)
            {
                projectServiceNames = new List<String>();
                servicesViewModel = new ObservableCollection<ServiceViewModel>();
                foreach (var service in servicesControl)
                {
                    servicesViewModel.Add(new ServiceViewModel(service));
                    projectServiceNames.Add(service.Name);
                }
            }

            gridControl.ItemsSource = servicesViewModel;

            Loaded += (o, e) =>
            {
                if (servicesViewModel != null && servicesViewModel.Count > 0)
                {
                    if (refreshTimer == null)
                    {
                        refreshTimer = new DispatcherTimer();
                        refreshTimer.Interval = TimeSpan.FromSeconds(1);
                        refreshTimer.Tick += RefreshTimer_Tick;
                    }
                    refreshTimer.Start();
                }

                LoadLayout();
            };

            Unloaded += (o, e) => 
            {
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Tick -= RefreshTimer_Tick;
                    refreshTimer = null;
                }

                SaveLayout();
            };
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (servicesViewModel != null)
            {
                RemoveNotAvailableDiscoveredServices();

                foreach (var viewModel in servicesViewModel)
                    viewModel.ForceUpdateCurrentStatus();
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion

        #region Methods
        ServiceViewModel GetSelectedServiceViewModel()
        {
            var viewModel = gridControl.SelectedItem as ServiceViewModel;
            if (viewModel != null)
                return viewModel;
            return null;
        }

        void OpenLogInDialog(ServiceViewModel viewModel)
        {
            var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
            if (uIInterface == null)
                return;
            
            ShowCredentialOptions options = new ShowCredentialOptions();
            options.WindowTitle = Properties.Resources.ShowCredentialTitle;
            options.MainInstruction = Properties.Resources.ShowCredentialMainInstruction;
            options.Content = String.Format(Properties.Resources.ShowCredentialContent, viewModel.FriendlyName);
            options.SavedCredentialsBucket = Properties.Resources.ShowCredentialTitle;

            ShowCredentialResults res = uIInterface.ShowCredentialDialog(options);
            if (res.result == CustomDialogResults.OK && !String.IsNullOrWhiteSpace(res.UserName))
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    ProgressBarIndeterminate = true,
                    DialogText = Properties.Resources.ShowCredentialValidating,
                    IsCancellingEnabled = false
                };

                bool userValidated = false;
                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    var userName = res.UserName;
                    var password = res.Password;
                    string domain = null;

                    if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                    {
                        var split = userName.Split(new char[] { '\\' });
                        if (split.Length > 1)
                        {
                            domain = split[0];
                            userName = split[1];
                        }
                        else
                        {
                            split = userName.Split(new char[] { '@' });
                            if (split.Length > 1)
                            {
                                domain = split[1];
                                userName = split[0];
                            }
                        }

                        if (!String.IsNullOrEmpty(domain))
                        {
                            using (var context = new PrincipalContext(ContextType.Domain, domain))
                            {
                                // validate the credentials
                                userValidated = context.ValidateCredentials(userName, password);
                            }
                        }
                        else
                        {
                            using (var context = new PrincipalContext(ContextType.Machine))
                            {
                                // validate the credentials
                                userValidated = context.ValidateCredentials(userName, password);
                            }
                        }
                    }
                });

                if (!userValidated)
                {
                    uIInterface.ShowInformation(String.Format(Properties.Resources.UserAuthenticationFailed, res.UserName));
                }
                else
                {
                    viewModel.UserName = res.UserName;
                    viewModel.Password = res.Password;
                }
            }
            
        }

        void RefreshDiscoveredServices()
        {
            RemoveDiscoveredServices();
            if (ShowAllServices)
                AddDiscoveredServices();
        }

        void AddDiscoveredServices()
        {
            if (servicesViewModel == null)
                return;

            using (var cursor = new WaitCursor())
            {
                var services = ServiceController.GetServices();
                if (services.Length > 0)
                {
                    if (discoveredServices == null)
                        discoveredServices = new List<ServiceViewModel>();

                    foreach (var service in services)
                    {
                        if (!IsRecognizedServiceName(service))
                            continue;

                        if (!String.IsNullOrEmpty(WebHMI_ServiceName) && service.ServiceName.StartsWith(WebHMI_ServiceName))
                        {
                            var serviceControl = new UFWebHMIServiceControl(service.ServiceName);
                            var viewModel = new ServiceViewModel(serviceControl);
                            discoveredServices.Add(viewModel);
                            servicesViewModel.Add(viewModel);
                            service.Dispose();
                        }
                        else
                        {
                            var discoveredService = new DiscoveredService(service.ServiceName, service.DisplayName);
                            var viewModel = new ServiceViewModel(discoveredService);
                            discoveredServices.Add(viewModel);
                            servicesViewModel.Add(viewModel);
                            service.Dispose();
                        }
                    }
                }
            }
        }

        void RemoveDiscoveredServices()
        {
            if (servicesViewModel == null)
                return;

            if (discoveredServices != null && discoveredServices.Count > 0)
            {
                using (var cursor = new WaitCursor())
                {
                    if (servicesViewModel != null && servicesViewModel.Count > 0)
                        discoveredServices.ForEach(viewModel => servicesViewModel.Remove(viewModel));
                    discoveredServices.Clear();
                }
            }
        }

        void RemoveNotAvailableDiscoveredServices()
        {
            if (discoveredServices == null)
                return;

            var services = discoveredServices.ToList();
            foreach (var viewModel in services)
            {
                ServiceController service = null;
                try
                {
                    service = new ServiceController(viewModel.Service.Name);
                    var status = service.Status;
                }
                catch
                {
                    discoveredServices.Remove(viewModel);
                    servicesViewModel.Remove(viewModel);
                }
                finally
                {
                    if (service != null)
                        service.Dispose();
                }
            }
        }

        bool IsRecognizedServiceName(ServiceController service)
        {
            if (projectServiceNames == null || projectServiceNames.Contains(service.ServiceName))
                return false;
            else if (service.DisplayName != null && service.DisplayName.StartsWith(Properties.Settings.Default.MovNextKeyName, StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }
        #endregion

        #region Properties
        bool showAllServices;
        public bool ShowAllServices
        {
            get 
            {
                return showAllServices;
            }
            set
            {
                if (showAllServices == value)
                    return;
                showAllServices = value;
                checkShowAllServices.IsChecked = value;
                RefreshDiscoveredServices();
            }
        }
        #endregion

        #region Isolated Storage

        static String GetStoreFileName()
        {
            return String.Format("{0}.ServiceControlPanel.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void SaveLayout()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = new SettingsStorage();
                        settingsStorage.ShowAllServices = ShowAllServices;
                        serializer.WriteObject(writer, settingsStorage);
                    }
                }
            }
            catch
            { }
        }

        void LoadLayout()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = serializer.ReadObject(reader) as SettingsStorage;
                        ShowAllServices = settingsStorage.ShowAllServices;
                    }
                }
            }
            catch
            { }
        }

        #endregion

        #region Commands
        RelayCommand logIn;
        public ICommand LogIn
        {
            get
            {
                if (logIn == null)
                {
                    logIn = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                                OpenLogInDialog(viewModel);
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && viewModel.Status == ServiceInstaller.ServiceState.NotFound;
                        });
                }
                return logIn;
            }
        }

        RelayCommand openServiceManager;
        public ICommand OpenServiceManager
        {
            get
            {
                if (openServiceManager == null)
                {
                    openServiceManager = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                                viewModel.Service.OpenServiceControl();
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && (discoveredServices == null || !discoveredServices.Contains(viewModel));
                        });
                }
                return openServiceManager;
            }
        }

        RelayCommand install;
        public ICommand Install
        {
            get
            {
                if (install == null)
                {
                    install = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                            {
                                try
                                {
                                    if (viewModel.Service.UseCredentialProvider && parent != null)
                                        UFProjectManagerComponent.projectManagerComponent.UserEditor.EnsureCredentialProvider(parent);
                                    if (String.IsNullOrEmpty(viewModel.UserName) || String.IsNullOrEmpty(viewModel.Password))
                                        viewModel.Service.Install();
                                    else
                                        viewModel.Service.Install(viewModel.UserName, viewModel.Password);
                                }
                                catch (Exception ex)
                                {
                                    var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
                                    if (uIInterface != null)
                                        uIInterface.ShowError(string.Format(Properties.Resources.InstallServiceFailed, viewModel.DisplayName, ex.Message));
                                }
                            }
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && viewModel.Status == ServiceInstaller.ServiceState.NotFound;
                        });
                }
                return install;
            }
        }

        RelayCommand uninstall;
        public ICommand Uninstall
        {
            get
            {
                if (uninstall == null)
                {
                    uninstall = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                            {
                                try
                                {
                                    viewModel.Service.Uninstall();
                                    viewModel.UserName = viewModel.Password = null;
                                }
                                catch (Exception ex)
                                {
                                    var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
                                    if (uIInterface != null)
                                        uIInterface.ShowError(string.Format(Properties.Resources.UninstallServiceFailed, viewModel.DisplayName, ex.Message));
                                }
                            }
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && viewModel.Status != ServiceInstaller.ServiceState.NotFound;
                        });
                }
                return uninstall;
            }
        }

        RelayCommand start;
        public ICommand Start
        {
            get
            {
                if (start == null)
                {
                    start = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                            {
                                try
                                {
                                    if (viewModel.Service is UFWebHMIServiceControl)
                                    {
                                        var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
                                        if (uIInterface != null)
                                            uIInterface.ShowHidingInformation(Properties.Resources.ApplicationCertificateMassageInfo, "UFWebHMIServiceControl-ApplicationCertificateMassageInfo");
                                    }
                                    viewModel.Service.Start();
                                }
                                catch (Exception ex)
                                {
                                    var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
                                    if (uIInterface != null)
                                        uIInterface.ShowError(string.Format(Properties.Resources.StartServiceFailed, viewModel.DisplayName, ex.Message));
                                }
                            }
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && viewModel.Status == ServiceInstaller.ServiceState.Stopped;
                        });
                }
                return start;
            }
        }

        RelayCommand stop;
        public ICommand Stop
        {
            get
            {
                if (stop == null)
                {
                    stop = new RelayCommand(
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            if (viewModel != null)
                            {
                                try
                                {
                                    viewModel.Service.Stop();
                                } 
                                catch (Exception ex)
                                {
                                    var uIInterface = UFProjectManagerComponent.projectManagerComponent.UIInterface;
                                    if (uIInterface != null)
                                        uIInterface.ShowError(string.Format(Properties.Resources.StopServiceFailed, viewModel.DisplayName, ex.Message));
                                }
                            }
                        },
                        param =>
                        {
                            var viewModel = GetSelectedServiceViewModel();
                            return viewModel != null && viewModel.Status == ServiceInstaller.ServiceState.Running;
                        });
                }
                return stop;
            }
        }
        #endregion
    }

    [DataContract(Name = "SettingsStorage")]
    class SettingsStorage
    {
        #region Members
        [DataMember]
        public bool ShowAllServices;
        #endregion
    }
}
