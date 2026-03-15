using DataReader;
using DevExpress.Xpf.Core;
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
using ViewModelLib;

namespace SQLDatabaseConfiguration
{
    /// <summary>
    /// Interaction logic for AggregatesTables.xaml
    /// </summary>
    public partial class AggregatesTables : UserControl
    {
        #region Declarations
        bool isSyncronizing;
        #endregion

        #region Constructors
        public AggregatesTables()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                SyncronizeSelectedColumns();
            };
        }
        #endregion

        #region Methods
        void SyncronizeSelectedColumns()
        {
            var sqlConfigViewModel = DataContext as SqlConfigViewModel;
            if (sqlConfigViewModel == null)
                return;

            try
            {
                isSyncronizing = true;
                listColumns.SelectedItems.Clear();
                foreach (var column in sqlConfigViewModel.Columns)
                    listColumns.SelectedItems.Add(column);
                sqlConfigViewModel.UpdateColumns(listColumns.SelectedItems);
            }
            finally
            {
                isSyncronizing = false;
            }
        }
        #endregion

        #region Commands
        private void SelectDataSource_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var sqlConfigViewModel = DataContext as SqlConfigViewModel;
            if (sqlConfigViewModel == null)
                return;

            String connection = null;
            if (sqlConfigViewModel.DataReaderModel != null)
            {
                connection = String.Format("{0}={1};{2}", 
                    DevExpress.Xpo.DB.DataStoreBase.XpoProviderTypeParameterName,
                    DevExpress.Xpo.DB.MSSqlConnectionProvider.XpoProviderTypeString, 
                    sqlConfigViewModel.DataReaderModel.Connection);
            }

            var wizard = new CommonControls.ConnectionWizard(CommonControls.ProviderTypes.SQLServer, null, null)
            {
                ConnectionString = connection
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(wizard)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ConnectionWizard"
            };

            Dialog.Closing += (o, ea) =>
            {
                if (Dialog.DialogResult.HasValue && Dialog.DialogResult.Value)
                    ea.Cancel = wizard.IsValid(null);
            };

            if (Dialog.ShowDialog() == true)
            {
                if (DataReader.Helpers.XpoConversionHelper.IsDotNetConvertible(wizard.ConnectionString))
                {
                    var dataReaderModel = new DataReaderModel()
                    {
                        DataProvider = DataReader.Helpers.XpoConversionHelper.GetDataProviderFromXpoConnection(wizard.ConnectionString),
                        Connection = DataReader.Helpers.XpoConversionHelper.GetConnectionStringFromXpoConnection(wizard.ConnectionString)
                    };
                    sqlConfigViewModel.DataReaderModel = dataReaderModel;
                }
                else
                    sqlConfigViewModel.DataReaderModel = null;
            }
        }

        private void listColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (!isSyncronizing)
            {
                var sqlConfigViewModel = DataContext as SqlConfigViewModel;
                if (sqlConfigViewModel == null)
                    return;

                sqlConfigViewModel.UpdateColumns(listColumns.SelectedItems);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Dispatcher.InvokeShutdown();
        }

        RelayCommand selectAll;
        public ICommand SelectAll
        {
            get
            {
                if (selectAll == null)
                {
                    selectAll = new RelayCommand(
                        param =>
                        {
                            listColumns.SelectAll();
                        },
                        param =>
                        {
                            var sqlConfigViewModel = DataContext as SqlConfigViewModel;
                            if (sqlConfigViewModel == null)
                                return false;

                            return sqlConfigViewModel.NumericColumnNames.Count > listColumns.SelectedItems.Count;
                        });
                }
                return selectAll;
            }
        }

        RelayCommand selectNone;
        public ICommand SelectNone
        {
            get
            {
                if (selectNone == null)
                {
                    selectNone = new RelayCommand(
                        param =>
                        {
                            listColumns.SelectedItems.Clear();
                        },
                        param =>
                        {
                            return listColumns.SelectedItems.Count > 0;
                        });
                }
                return selectNone;
            }
        }
        #endregion
    }
}
