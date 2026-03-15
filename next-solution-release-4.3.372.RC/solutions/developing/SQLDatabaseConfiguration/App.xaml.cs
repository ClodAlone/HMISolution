using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utilities;

namespace SQLDatabaseConfiguration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

            DXGridDataController.DisableThreadingProblemsDetection = true;

            SetPropertyHelper();

#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !", "DebugMe", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            if (e.Args.Length > 0)
            {
                CommandLineOptions options = new CommandLineOptions(e.Args);
                if (options.IsValid)
                {
                    var sqlConfigViewModel = new SqlConfigViewModel(options.DataSource, !options.Silent)
                    {
                        OperationType = options.OperationType,
                        CommandType = options.CommandType,
                        TableName = options.TableName,
                        UtcTimeColName = options.UtcTimeColName,
                        LocalTimeColName = options.LocalTimeColName,
                        ProjectFilePath = options.ProjectPath,
                        ProjectPassword = options.ProjectPassword
                    };

                    if (!String.IsNullOrWhiteSpace(options.ProjectPath))
                        sqlConfigViewModel.CommandType = CommandType.Check;

                    if (!String.IsNullOrWhiteSpace(options.Columns))
                        sqlConfigViewModel.UpdateColumns(options.Columns.Split(',').ToList());

                    if (!String.IsNullOrWhiteSpace(options.HideColumns))
                        sqlConfigViewModel.UpdateHideColumns(options.HideColumns.Split(',').ToList());

                    if (options.Silent)
                    {
                        sqlConfigViewModel.Execute();
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        if (options.OperationType == OperationType.None)
                        {
                            MainWindow = new MainWindow(options.CallingProcessId) { DataContext = sqlConfigViewModel };
                            MainWindow.Show();
                        }
                        else if (options.OperationType == OperationType.AggregatesTables)
                        {
                            MainWindow = new AggregatesMainWindow(options.CallingProcessId) { DataContext = sqlConfigViewModel };
                            MainWindow.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(SQLDatabaseConfiguration.Properties.Resources.InvalidOptions,
                        SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown(-10);
                }
            }
            else
            {
                MainWindow = new MainWindow() { DataContext = new SqlConfigViewModel(true) { OperationType = OperationType.None, CommandType = CommandType.None } };
                MainWindow.Show();
            }
        }

        static void SetPropertyHelper()
        {
            Utilities.ApplicationPropertiesHelper.SetProperty("CurrentSkin", "Blend");

            var commonFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                SQLDatabaseConfiguration.Properties.Settings.Default.CompanyName,
                SQLDatabaseConfiguration.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            var userFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                SQLDatabaseConfiguration.Properties.Settings.Default.CompanyName,
                SQLDatabaseConfiguration.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            var projectFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                SQLDatabaseConfiguration.Properties.Settings.Default.CompanyName,
                SQLDatabaseConfiguration.Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
        }
    }
}
