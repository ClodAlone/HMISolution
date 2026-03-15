using DevExpress.Xpf.WindowsUI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Utilities;

namespace SQLDatabaseConfiguration
{
    /// <summary>
    /// Interaction logic for TraceExecution.xaml
    /// </summary>
    public partial class TraceExecution : UserControl, INavigationAware
    {
        #region Declarations
        CommandExecution command = CommandExecution.None;
        Timer executeTimer;
        #endregion

        public TraceExecution()
        {
            InitializeComponent();

            Loaded += (s, e) => 
            {
                if (executeTimer == null)
                {
                    busyControl.Visibility = Visibility.Visible;
                    tableView.MoveLastRow();
                    executeTimer = new Timer(ExecuteTimer, DataContext, 500, Timeout.Infinite);
                }
            };
        }

        private void ExecuteTimer(object state)
        {
            if (executeTimer != null)
                executeTimer.Dispose();

            var sqlConfigViewModel = state as SqlConfigViewModel;
            if (sqlConfigViewModel == null)
                return;

            if (command == CommandExecution.ExecuteAggregation)
                sqlConfigViewModel.Execute(OperationType.AggregatesTables, CommandType.Add);
            else if (command == CommandExecution.RemoveAggregation)
                sqlConfigViewModel.Execute(OperationType.AggregatesTables, CommandType.Remove);
            else if (command == CommandExecution.UpdateAggregation)
                sqlConfigViewModel.Execute(OperationType.AggregatesTables, CommandType.Update);
            else if (command == CommandExecution.ExecutePartition)
                sqlConfigViewModel.Execute(OperationType.PatitionTables, CommandType.Add);
            else if (command == CommandExecution.RemovePartition)
                sqlConfigViewModel.Execute(OperationType.PatitionTables, CommandType.Remove);
            else if (command == CommandExecution.UpdatePartition)
                sqlConfigViewModel.Execute(OperationType.PatitionTables, CommandType.Update);
            else if (command == CommandExecution.ParseProjectForAggregation)
                sqlConfigViewModel.Execute(OperationType.AggregatesTables, CommandType.Check);
            else
                sqlConfigViewModel.Execute();

            Dispatcher.BeginInvokeAsynchronously(() => 
            {
                tableView.MoveLastRow();
                busyControl.Visibility = Visibility.Collapsed;
            });
        }

        #region INavigationAware
        public void NavigatedFrom(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        { }

        public void NavigatedTo(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter is CommandExecution)
                command = (CommandExecution)e.Parameter;

            if (command == CommandExecution.ExecuteAggregation ||
                command == CommandExecution.RemoveAggregation ||
                command == CommandExecution.UpdateAggregation)
                DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(backButton, "AggregatesTables");
            else if (command == CommandExecution.ParseProjectForAggregation || command == CommandExecution.ParseProjectForPartition)
                DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(backButton, "SelectOperation");
            else
                DevExpress.Xpf.WindowsUI.Navigation.Navigation.SetNavigateTo(backButton, "");
        }

        public void NavigatingFrom(NavigatingEventArgs e)
        { }
        #endregion
    }
}
