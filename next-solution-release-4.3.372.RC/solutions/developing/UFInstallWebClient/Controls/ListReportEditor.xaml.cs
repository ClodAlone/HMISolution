using DataGridElementSettings;
using DataReader;
using DataReader.Extensions;
using DataReader.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInstallWebClient.ViewModel;
using Utilities;
using Utilities.WPF;
using ViewModelLib;

namespace UFInstallWebClient.Controls
{
    /// <summary>
    /// Interaction logic for ListReportEditor.xaml
    /// </summary>
    public partial class ListReportEditor : UserControl
    {
        #region Declaration
        public ObservableCollection<ReportViewModel> ViewModels { get; set; }
        private ReportViewModel reportVM;
        #endregion

        #region Constructors

        public ListReportEditor(DataGridElement[] sources, ReportViewModel reportViewModel)
        {
            InitializeComponent();
            ViewModels = new ObservableCollection<ReportViewModel>();
            reportVM = reportViewModel;

            foreach (var source in sources)
            {
                ViewModels.Add(new ReportViewModel(reportVM.Users, reportVM.Roles, reportVM.Reports)
                {
                    SelectedUsers = source.Users,
                    SelectedRoles = source.Roles,
                    Report = source.Name
                });
            }
            
            layoutGrid.DataContext = ViewModels;
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand RemoveCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveUpCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveDownCommand = new RoutedCommand();

        RelayCommand addNew;
        public ICommand AddNew
        {
            get
            {
                if (addNew == null)
                {
                    addNew = new RelayCommand(
                        param =>
                        {
                            ViewModels.Add(new ReportViewModel(reportVM.Users, reportVM.Roles, reportVM.Reports));                            
                        });
                }
                return addNew;
            }
        }

        RelayCommand removeAll;
        public ICommand RemoveAll
        {
            get
            {
                if (removeAll == null)
                {
                    removeAll = new RelayCommand(
                        param => 
                        {
                            ViewModels.Clear(); 
                        },
                        param => ViewModels != null && ViewModels.Count > 0
                        );
                }
                return removeAll;
            }
        }
        #endregion

        #region Methods
        void OnCanExecuteRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnRemoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (ReportViewModel)(button.Tag);
            if (value != null)
                ViewModels.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (ReportViewModel)(button.Tag);

            e.CanExecute = value != null && ViewModels.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (ReportViewModel)(button.Tag);
            if (value != null)
            {
                var index = ViewModels.IndexOf(value);
                if (index > 0)
                    ViewModels.Move(index, index - 1);
            }
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            var value = (ReportViewModel)(button.Tag);

            e.CanExecute = value != null && ViewModels.IndexOf(value) < ViewModels.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (ReportViewModel)(button.Tag);
            if (value != null)
            {
                var index = ViewModels.IndexOf(value);
                if (index < ViewModels.Count - 1)
                    ViewModels.Move(index, index + 1);
            }
        }
        #endregion

        #region Properties
        public DataGridElement[] CurrentDataSource
        {
            get
            {
                var dataSources = new ObservableCollection<DataGridElement>();
                
                foreach(var v in ViewModels)
                {
                    dataSources.Add(new DataGridElement()
                    {
                        Name = v.Report,
                        Users = v.SelectedUsers,
                        Roles = v.SelectedRoles
                    });
                }
               
                return dataSources.ToArray();
            }
        }
        #endregion
    }
}
