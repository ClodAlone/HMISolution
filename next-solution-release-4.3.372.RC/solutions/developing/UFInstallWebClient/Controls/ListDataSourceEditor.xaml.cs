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
using Utilities;
using Utilities.WPF;
using ViewModelLib;

namespace UFInstallWebClient.Controls
{
    /// <summary>
    /// Interaction logic for ListDataSourceEditor.xaml
    /// </summary>
    public partial class ListDataSourceEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<DataGridElement> ListDataSource;
        readonly bool useXpoProvider;
        #endregion

        #region Constructors
        public ListDataSourceEditor(IList<DataGridElement> listDataSource) : this (listDataSource, useXpoProvider: false)
        { }

        public ListDataSourceEditor(IList<DataGridElement> listDataSource, bool useXpoProvider)
        {
            InitializeComponent();

            var dataSources = listDataSource.ToArray();
            this.useXpoProvider = useXpoProvider;

            if (useXpoProvider)
            {
                for (int ii = 0; ii < dataSources.Length; ii++)
                {
                    if (XpoConversionHelper.IsDotNetConvertible(dataSources[ii].ConnectionString))
                    {
                        dataSources[ii].ConnectionString = String.Format("{0}={1};{2}", "DataProvider",
                            XpoConversionHelper.GetDataProviderFromXpoConnection(dataSources[ii].ConnectionString),
                            XpoConversionHelper.GetConnectionStringFromXpoConnection(dataSources[ii].ConnectionString));
                    }
                }
            }

            layoutGrid.DataContext = ListDataSource = new ObservableCollection<DataGridElement>(dataSources);
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand EditCommand = new RoutedCommand();
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
                            var dataReaderModel = new DataReaderModel();
                            var control = new DataReaderEditor.DataReaderEditor(dataReaderModel, null);
                            var dialog = new GeneralDialogContent(control)
                            {
                                DialogKeepContent = true,
                                Owner = this.FindParent<Window>(),
                                HelpLink = "DataReaderEditor"
                            };

                            dialog.Closing += (o, e) =>
                            {
                                if (dialog.DialogResult == true)
                                {
                                    if (!dataReaderModel.IsValid())
                                    {
                                        MessageBox.Show(Properties.Resources.InvalidDataSource,
                                            Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                                        e.Cancel = true;
                                    }
                                    else if (useXpoProvider && !XpoConversionHelper.IsXpoConvertible(dataReaderModel.GetFullConnection()))
                                    {
                                        MessageBox.Show(Properties.Resources.InvalidXpoDataSource,
                                            Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                                        e.Cancel = true;
                                    }
                                }
                            };

                            if (dialog.ShowDialog() == true)
                            {
                                ListDataSource.Add(new DataGridElement()
                                {
                                    Name = Properties.Resources.NewDataSourceLabel,
                                    ConnectionString = dataReaderModel.GetFullConnection(),
                                    Query = dataReaderModel.GetFullQuery()
                                });
                            }
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
                            ListDataSource.Clear(); 
                        },
                        param => ListDataSource != null && ListDataSource.Count > 0
                        );
                }
                return removeAll;
            }
        }
        #endregion

        #region Methods
        void OnCanExecuteEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnEditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);
            if (value != null)
            {
                var dataReaderModel = new DataReaderModel(value.Name, value.ConnectionString, value.Query);
                var control = new DataReaderEditor.DataReaderEditor(dataReaderModel, null);
                var dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "DataReaderEditor"
                };

                dialog.Closing += (o, ev) =>
                {
                    if (dialog.DialogResult == true && !dataReaderModel.IsValid())
                    {
                        MessageBox.Show(Properties.Resources.InvalidDataSource, 
                            Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        ev.Cancel = true;
                    }
                };

                if (dialog.ShowDialog() == true)
                {
                    var index = ListDataSource.IndexOf(value);
                    ListDataSource.Insert(index, new DataGridElement()
                    {
                        Name = value.Name,
                        ConnectionString = dataReaderModel.GetFullConnection(),
                        Query = dataReaderModel.GetFullQuery()
                    });
                    ListDataSource.Remove(value);
                }
            }
        }

        void OnCanExecuteRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnRemoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);
            if (value != null)
                ListDataSource.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);

            e.CanExecute = value != null && ListDataSource.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);
            if (value != null)
            {
                var index = ListDataSource.IndexOf(value);
                if (index > 0)
                    ListDataSource.Move(index, index - 1);
            }
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);

            e.CanExecute = value != null && ListDataSource.IndexOf(value) < ListDataSource.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (DataGridElement)(button.Tag);
            if (value != null)
            {
                var index = ListDataSource.IndexOf(value);
                if (index < ListDataSource.Count - 1)
                    ListDataSource.Move(index, index + 1);
            }
        }
        #endregion

        #region Properties
        public DataGridElement[] CurrentDataSource
        {
            get
            {
                var dataSources = ListDataSource.ToArray();
                if (useXpoProvider)
                {
                    for (int ii = 0; ii < dataSources.Length; ii++)
                    {
                        if (XpoConversionHelper.IsXpoConvertible(dataSources[ii].ConnectionString))
                        {
                            dataSources[ii].ConnectionString = String.Format("{0}={1};{2}", DataStoreBase.XpoProviderTypeParameterName,
                                XpoConversionHelper.GetXpoProviderFromNetConnection(dataSources[ii].ConnectionString),
                                XpoConversionHelper.GetXpoConnectionStringFromNetConnection(dataSources[ii].ConnectionString));
                        }
                    }
                }

                return dataSources;
            }
        }
        #endregion
    }
}
