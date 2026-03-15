using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LanguagePreferences
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !", "DebugMe", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            if (e.Args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(e.Args);
                if (cl.IsValid)
                {
                    if (cl.SilentMode)
                    {
                        LanguageViewModel vm = new LanguageViewModel();
                        vm.SetNewLanguage(cl.CultureName);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        LanguageViewModel vm = new LanguageViewModel(cl.CallingProcessId, cl.CurrentSkin);
                        MainWindow = new MainWindow() { DataContext = vm };
                        MainWindow.Show();

                        var dispatcher = MainWindow.Dispatcher;
                        vm.CallingProcessExited += (s, ev) =>
                        {
                            dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
                        };
                    }
                }
                else
                {
                    if (!cl.SilentMode)
                        System.Windows.MessageBox.Show(LanguagePreferences.Properties.Resources.InvalidOptions);
                    Application.Current.Shutdown(-10);
                }

            }
            else
            {
                LanguageViewModel vm = new LanguageViewModel();
                MainWindow = new MainWindow() { DataContext = vm };
                MainWindow.Show();
            }
        }
    }
}
