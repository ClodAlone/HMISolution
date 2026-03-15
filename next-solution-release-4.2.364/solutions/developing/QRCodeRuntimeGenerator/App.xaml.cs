using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;
using Utilities;

namespace QRCodeRuntimeGenerator
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
                    switch (cl.Operation)
                    {
                        case Operations.OpSection:
                            {
                                MainWindow dm = new MainWindow();
                                dm.QRCode.Text = cl.QRCode;
                                dm.ImagePath = cl.ImagePath;
                                if (cl.DirectPrint)
                                {
                                    dm.PrintReport();
                                    Application.Current.Shutdown();
                                }
                                else
                                {
                                    dm.PreviewReport();
                                    Application.Current.Shutdown();
                                }
                            }
                            break;
                        case Operations.OpInteractive:
                            MainWindow = new MainWindow() { DataContext = cl };
                            MainWindow.Show();
                            break;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(QRCodeRuntimeGenerator.Properties.Resources.InvalidOptions);
                    Application.Current.Shutdown(-10);
                }

            }
            else
            {
                MainWindow = new MainWindow() { DataContext = null };
                MainWindow.Show();
            }
        }
    }
}
