using System;
using System.Collections.Generic;
using System.Linq;
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

namespace UFUAServiceLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class InstallServiceControl : UserControl
    {
        #region Declarations
        public CommadLineOptions cmdOptions;
        bool bLoaded;
        #endregion

        #region Constructors
        public InstallServiceControl()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    var serviceUser = DataContext as ServiceUser;
                    if (!String.IsNullOrEmpty(serviceUser.UserName) &&
                        !String.IsNullOrEmpty(serviceUser.Password))
                    {
                        Username.IsEnabled = false;
                        Password.IsEnabled = false;
                    }
                }
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand InstallAsService = new RoutedCommand();
        public static readonly RoutedCommand UninstallAsService = new RoutedCommand();
        public static readonly RoutedCommand StartService = new RoutedCommand();
        public static readonly RoutedCommand StopService = new RoutedCommand();
        #endregion

        #region Properties
        public bool EnableInstall { get; set; } = true;
        public bool EnableUninstall { get; set; } = true;
        public bool EnableStart { get; set; } = true;
        public bool EnableStop { get; set; } = true;
        #endregion

        #region Command Handlers

        private void OnInstallAsService(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            ServiceUser serviceUser = DataContext as ServiceUser;
            if (serviceUser == null)
                return;

            var controller = new ServiceController(cmdOptions, serviceUser, this.FindParent<Window>());
            controller.InstallAsService();
        }

        private void CanInstallAsService(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EnableInstall && cmdOptions != null && !ServiceInstaller.ServiceInstaller.ServiceIsInstalled(cmdOptions.ServiceName);
        }

        private void OnUninstallAsService(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var controller = new ServiceController(cmdOptions, this.FindParent<Window>());
            controller.UninstallAsService();
        }

        private void CanUninstallAsService(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EnableUninstall && cmdOptions != null && ServiceInstaller.ServiceInstaller.ServiceIsInstalled(cmdOptions.ServiceName);
        }

        private void OnStartService(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var controller = new ServiceController(cmdOptions, this.FindParent<Window>());
            controller.StartService();
        }

        private void CanStartService(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EnableStart && cmdOptions != null && (ServiceInstaller.ServiceInstaller.GetServiceStatus(cmdOptions.ServiceName) == ServiceInstaller.ServiceState.Stopped);
        }

        private void OnStopService(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var controller = new ServiceController(cmdOptions, this.FindParent<Window>());
            controller.StopService();
        }

        private void CanStopService(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EnableStop && cmdOptions != null && (ServiceInstaller.ServiceInstaller.GetServiceStatus(cmdOptions.ServiceName) == ServiceInstaller.ServiceState.Running);
        }
        #endregion
    }
}
