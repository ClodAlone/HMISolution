using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;
using ServiceInstaller;
using WPFUtilities;
using Utilities;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Text;
using System.Threading;
using UFUAServiceLibrary;

namespace UFUAInstallDeployServerService
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

#if !DEBUG
            if (!hasAdministrativeRight)
            {
                RunElevated(System.Windows.Forms.Application.ExecutablePath);
                // Close();
                Application.Current.Shutdown();
                return;
            }
#else
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - UFUAInstallDeployServerService", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif  
            bool bexit = true;
            CommadLineOptions cl = new CommadLineOptions(e.Args);
            if (cl.CheckOptions(true))
            {
                if (cl.Operation == Operations.OpDialog)
                {
                    //acqire current user credentials...
                    ServiceUser su = new ServiceUser() { UserName = cl.UserName, Password = cl.Password };

                    MainWindow = new MainWindow() { DataContext = su, cmdOptions = cl };
                    MainWindow.Show();
                    bexit = false;
                }
                else if (cl.Operation == Operations.OpInstall)
                {
                    ServiceUser su = new ServiceUser() { UserName = cl.UserName, Password = cl.Password };
                    var controller = new ServiceController(cl, su);
                    controller.InstallAsService();
                }
                else if (cl.Operation == Operations.OpUninstall)
                {
                    var controller = new ServiceController(cl);
                    controller.UninstallAsService();
                }
                else if (cl.Operation == Operations.OpStart)
                {
                    var controller = new ServiceController(cl);
                    controller.StartService();
                }
                else if (cl.Operation == Operations.OpStop)
                {
                    var controller = new ServiceController(cl);
                    controller.StopService();
                }
            }

            if (bexit)
                Application.Current.Shutdown(-10);
        }

        private static bool RunElevated(string fileName)
        {
            //MessageBox.Show("Run: " + fileName);
            ProcessStartInfo processInfo = new ProcessStartInfo() { Verb = "runas", FileName = fileName };
            processInfo.Arguments = Environment.CommandLine;
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
    }
}
