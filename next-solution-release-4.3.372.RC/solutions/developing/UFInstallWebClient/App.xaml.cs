using Campari.Software;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Utilities;

namespace UFInstallWebClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
#if !DEBUG
                RunElevated(System.Windows.Forms.Application.ExecutablePath);
                Application.Current.Shutdown();
#else
                MessageBox.Show("The application have not Administrator right!", "Install Web Client", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
#endif
                return;
            }

            SetPropertyHelper();

            if (!InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS7))
            {
                MessageBox.Show(UFInstallWebClient.Properties.Resources.MissingIIS7, UFInstallWebClient.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown(-10);
            }
            else if (e.Args.Length > 0)
            {
                var viewModel = new IISViewModel();
                CommandLineOptions.Parse(e.Args, viewModel);
                if (viewModel.IsValid)
                {
                    MainWindow = new MainWindow() { DataContext = viewModel };
                    viewModel.owner = MainWindow;
                    MainWindow.Show();
                }
                else
                {
                    MessageBox.Show(UFInstallWebClient.Properties.Resources.InvalidOptions, UFInstallWebClient.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Application.Current.Shutdown(-10);
                }

            }
            else
            {
                MessageBox.Show(UFInstallWebClient.Properties.Resources.InvalidOptions, UFInstallWebClient.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown(-10);
            }
        }

        private static bool RunElevated(string fileName)
        {
            //MessageBox.Show("Run: " + fileName);
            ProcessStartInfo processInfo = new ProcessStartInfo() 
            { 
                Verb = "runas", 
                FileName = fileName,
                Arguments = Environment.CommandLine
            };
            
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }

        private static void SetPropertyHelper()
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                UFInstallWebClient.Properties.Settings.Default.CompanyName,
                UFInstallWebClient.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            var userFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                UFInstallWebClient.Properties.Settings.Default.CompanyName,
                UFInstallWebClient.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            var projectFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                UFInstallWebClient.Properties.Settings.Default.CompanyName,
                UFInstallWebClient.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
        }
    }
}
