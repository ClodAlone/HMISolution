using DevExpress.Xpf.WindowsUI.Navigation;
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

namespace SQLDatabaseConfiguration
{
    /// <summary>
    /// Interaction logic for SelectOperation.xaml
    /// </summary>
    public partial class SelectOperation : UserControl, INavigationAware
    {
        SqlConfigViewModel sqlConfigViewModel;

        public SelectOperation()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (sqlConfigViewModel == null)
                    return;

                SetGoNavigation(sqlConfigViewModel);
            };

            DataContextChanged += (s, e) =>
            {
                sqlConfigViewModel = DataContext as SqlConfigViewModel;
                if (sqlConfigViewModel == null)
                    return;

                textProjectPassword.Password = sqlConfigViewModel.ProjectPassword;
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Dispatcher.InvokeShutdown();
        }

        private void SelectProjectPath_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sqlConfigViewModel == null)
                return;

            var projectFilePath = Helpers.ParseProjectHelper.SelectProjectPath();
            if (!String.IsNullOrEmpty(projectFilePath))
            {
                sqlConfigViewModel.ProjectFilePath = projectFilePath;
                SetGoNavigation(sqlConfigViewModel);
            }
        }

        private void SelectProjectPath_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sqlConfigViewModel == null)
                return;

            sqlConfigViewModel.ProjectFilePath = null;
            SetGoNavigation(sqlConfigViewModel);
        }

        private void radioAggregation_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sqlConfigViewModel == null)
                return;

            SetGoNavigation(sqlConfigViewModel);
        }

        private void radioPartition_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sqlConfigViewModel == null)
                return;

            SetGoNavigation(sqlConfigViewModel);
        }

        void SetGoNavigation(SqlConfigViewModel sqlConfigViewModel)
        {
            if (String.IsNullOrWhiteSpace(sqlConfigViewModel.ProjectFilePath))
            {
                if (radioAggregation.IsChecked == true)
                {
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(buttonGo, "AggregatesTables");
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigationParameter(buttonGo, null);
                }
                else
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(buttonGo, null);
            }
            else
            {
                if (radioAggregation.IsChecked == true)
                {
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(buttonGo, "TraceExecution");
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigationParameter(buttonGo, CommandExecution.ParseProjectForAggregation);
                }
                else
                    DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(buttonGo, null);
            }
        }

        #region INavigationAware
        public void NavigatedFrom(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        { }

        public void NavigatedTo(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        { }

        public void NavigatingFrom(NavigatingEventArgs e)
        {
            if (sqlConfigViewModel == null)
                return;

            sqlConfigViewModel.ProjectPassword = textProjectPassword.Password;
        }
        #endregion
    }
}
