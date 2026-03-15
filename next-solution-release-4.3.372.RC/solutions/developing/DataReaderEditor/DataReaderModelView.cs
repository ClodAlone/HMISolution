using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using DataReader;
using DataReader.Extensions;
using System.ComponentModel;
using ViewModelLib;
using System.Windows.Input;
using Microsoft.Data.ConnectionUI;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Diagnostics;

namespace DataReaderEditor
{
    public class DataReaderModelView : Observable
    {
        #region Declarations
        readonly DataReaderModel Model;
        readonly String ProjectRoot;
        #endregion

        #region Costructors
        public DataReaderModelView(DataReaderModel model, String projectRoot)
        {
            Model = model;
            ProjectRoot = projectRoot;
        }
        #endregion

        #region Properties

        public String DataProvider
        {
            get { return Model.DataProvider; }
            set
            {
                Set(ref Model.DataProvider, value, "DataProvider");
                ResetXMLProperties();
            }
        }

        public String Connection
        {
            get 
            { 
                return XpoHelpers.XpoHelper.NormalizeConnectionString(Model.Connection, ProjectRoot); 
            }
            set
            {
                value = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(value, ProjectRoot);
                Set(ref Model.Connection, value, "Connection");
                ResetXMLProperties();
            }
        }

        public String Select
        {
            get { return Model.Select; }
            set
            {
                Set(ref Model.Select, value, "Select");
                ResetXMLProperties();
            }
        }

        public String Where
        {
            get { return Model.Where; }
            set
            {
                Set(ref Model.Where, value, "Where");
                ResetXMLProperties();
            }
        }

        public String GroupBy
        {
            get { return Model.GroupBy; }
            set
            {
                Set(ref Model.GroupBy, value, "GroupBy");
                ResetXMLProperties();
            }
        }

        public String Sort
        {
            get { return Model.Sort; }
            set
            {
                Set(ref Model.Sort, value, "Sort");
                ResetXMLProperties();
            }
        }

        public String XmlUri
        {
            get { return Model.xmlUri; }
            set
            {
                Set(ref Model.xmlUri, value, "XmlUri");
            }
        }

        public String XmlItems
        {
            get { return Model.xmlItems; }
            set
            {
                Set(ref Model.xmlItems, value, "XmlItems");
            }
        }

        public int MaxTake
        {
            get { return Model.MaxTake; }
            set
            {
                Set(ref Model.MaxTake, value, "MaxTake");
            }
        }

        ObservableCollection<dynamic> _dataItems;
        public ObservableCollection<dynamic> DataItems
        {
            get
            {
                return _dataItems;
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                Set(ref _isBusy, value, "IsBusy");
            }
        }

        bool _isFault;
        public bool IsFault
        {
            get
            {
                return _isFault;
            }
            private set
            {
                Set(ref _isFault, value, "IsFault");
            }
        }

        string _lastError;
        public string LastError
        {
            get 
            {
                return _lastError;
            }
            set
            {
                Set(ref _lastError, value, "LastError");
            }
        }

        ObservableCollection<String> _timeColumns;
        public ObservableCollection<String> TimeColumns
        {
            get
            {
                return _timeColumns;
            }
        }

        ObservableCollection<String> _dataColumns;
        public ObservableCollection<String> DataColumns
        {
            get
            {
                return _dataColumns;
            }
        }

        public String TableName
        {
            get { return Model.TableName; }
            set
            {
                Set(ref Model.TableName, value, "TableName");
                ResetXMLProperties();
            }
        }
        public String DataColumn
        {
            get { return Model.DataColumn; }
            set
            {
                Set(ref Model.DataColumn, value, "DataColumn");
                ResetXMLProperties();
            }
        }
        public String TimeColumn
        {
            get { return Model.TimeColumn; }
            set
            {
                Set(ref Model.TimeColumn, value, "TimeColumn");
                ResetXMLProperties();
            }
        }

        #endregion

        #region Methods
        void ResetXMLProperties()
        {
            XmlUri = XmlItems = null;
        }

        DataSource GetDataSource()
        {
            if (String.IsNullOrEmpty(Model.DataSourceName))
                return null;

            if (Model.DataSourceName == DataSource.AccessDataSource.Name)
                return DataSource.AccessDataSource;
            else if (Model.DataSourceName == DataSource.OdbcDataSource.Name)
                return DataSource.OdbcDataSource;
            else if (Model.DataSourceName == DataSource.OracleDataSource.Name)
                return DataSource.OracleDataSource;
            else if (Model.DataSourceName == DataSource.SqlDataSource.Name)
                return DataSource.SqlDataSource;
            else if (Model.DataSourceName == DataSource.SqlFileDataSource.Name)
                return DataSource.SqlDataSource;
            else if (Model.DataSourceName == SqlCe.SqlCeDataSource.Name)
                return SqlCe.SqlCeDataSource;
            else if (Model.DataSourceName == DataSource.MySQLDataSource.Name)
                return DataSource.MySQLDataSource;
            else if (Model.DataSourceName == DataSource.SQLiteDataSource.Name)
                return DataSource.SQLiteDataSource;
            else if (Model.DataSourceName == DataSource.PostgreSQLDataSource.Name)
                return DataSource.PostgreSQLDataSource;
            else
                return null;
        }

        DataProvider GetDataProvider()
        {
            if (String.IsNullOrEmpty(Model.DataProvider))
                return null;

            if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.OdbcDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.OdbcDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.OleDBDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.OleDBDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.OracleDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.OracleDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.SqlDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.SqlDataProvider;
            else if (Model.DataProvider == SqlCe.SqlCeDataProvider.Name)
                return SqlCe.SqlCeDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.MySQLDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.MySQLDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.SQLiteDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.SQLiteDataProvider;
            else if (Model.DataProvider == Microsoft.Data.ConnectionUI.DataProvider.PostgreSQLDataProvider.Name)
                return Microsoft.Data.ConnectionUI.DataProvider.PostgreSQLDataProvider;
            else
                return null;
        }

        internal void ParseQuery()
        {
            if (String.IsNullOrEmpty(Select))
                return;

            var sqlParser = new Helper.SqlParser();
            sqlParser.Parse(Select);

            if (sqlParser.TableName != null)
                TableName = sqlParser.TableName;
            try
            {
                MaxTake = Convert.ToInt32(sqlParser.TopClause);
            }
            catch (Exception ex)
            {
                MaxTake = 0;
            }
        }
        #endregion

        #region Commands

        RelayCommand _editConnection;
        [Browsable(false)]
        public ICommand EditConnection
        {
            get
            {
                if (_editConnection == null)
                {
                    _editConnection = new RelayCommand(
                        param =>
                        {
			                DataConnectionDialog dcd = new DataConnectionDialog();
                            DataConnectionConfiguration dcs = new DataConnectionConfiguration(null);
			                dcs.LoadConfiguration(dcd);

                            var dataSource = GetDataSource();
                            var dataProvider = GetDataProvider();
                            if (dataSource != null && dataProvider != null)
                            {
                                try
                                {
                                    dcd.SelectedDataSource = dataSource;
                                    
                                    if (dcd.SelectedDataProvider == null)
                                        dcd.SelectedDataProvider = dataProvider;
                                    
                                    dcd.ConnectionString = Connection;
                                }
                                catch
                                { }
                            }

                            if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
                            {
                                Connection = dcd.ConnectionString;
                                DataProvider = dcd.SelectedDataProvider.Name;
                                Model.DataProviderDisplayName = dcd.SelectedDataProvider.DisplayName;
                                Model.DataProviderShortDisplayName = dcd.SelectedDataProvider.ShortDisplayName;
                                Model.DataProviderDescription = dcd.SelectedDataProvider.Description;
                                Model.DataSourceName = dcd.SelectedDataSource.Name;
                                Model.DataSourceDisplayName = dcd.SelectedDataSource.DisplayName;
                            }
                        },
                        param => true
                        );
                }
                return _editConnection;
            }
        }

        RelayCommand _editSelect;
        [Browsable(false)]
        public ICommand EditSelect
        {
            get
            {
                if (_editSelect == null)
                {
                    _editSelect = new RelayCommand(
                        param =>
                        {
                            var selectEditor = new SelectEditor(DataProvider, Connection, Select, MaxTake);
                            var dialog = new GeneralDialog(selectEditor) { Title = Properties.Resources.SelectTitle };
                            if (dialog.ShowDialog() == true)
                            {
                                Select = selectEditor.Select;
                                MaxTake = selectEditor.MaxTake;
                                TableName = selectEditor.TableName;
                            }
                        },
                        param => !String.IsNullOrEmpty(Connection)
                        );
                }
                return _editSelect;
            }
        }

        RelayCommand _editWhere;
        [Browsable(false)]
        public ICommand EditWhere
        {
            get
            {
                if (_editWhere == null)
                {
                    _editWhere = new RelayCommand(
                        param =>
                        {
                            var whereEditor = new WhereEditor(DataProvider, Connection, Select, Where);
                            var dialog = new GeneralDialog(whereEditor) { Title = Properties.Resources.WhereTitle };
                            if (dialog.ShowDialog() == true)
                            {
                                Where = whereEditor.GetSelectedFilter();
                            }
                        },
                        param => !String.IsNullOrEmpty(Connection) && !String.IsNullOrEmpty(Select)
                        );
                }
                return _editWhere;
            }
        }

        RelayCommand _editGroupBy;
        [Browsable(false)]
        public ICommand EditGroupBy
        {
            get
            {
                if (_editGroupBy == null)
                {
                    _editGroupBy = new RelayCommand(
                        param =>
                        {
                            var groupByEditor = new GroupByEditor(DataProvider, Connection, Select, GroupBy);
                            var dialog = new GeneralDialog(groupByEditor) { Title = Properties.Resources.GroupByTitle };
                            if (dialog.ShowDialog() == true)
                            {
                                GroupBy = groupByEditor.GetSelectedGroupBy();
                            }
                        },
                        param => !String.IsNullOrEmpty(Connection) && !String.IsNullOrEmpty(Select)
                        );
                }
                return _editGroupBy;
            }
        }

        RelayCommand _editSort;
        [Browsable(false)]
        public ICommand EditSort
        {
            get
            {
                if (_editSort == null)
                {
                    _editSort = new RelayCommand(
                        param =>
                        {
                            var sortEditor = new SortEditor(DataProvider, Connection, Select, Sort);
                            var dialog = new GeneralDialog(sortEditor) { Title = Properties.Resources.SortTitle };
                            if (dialog.ShowDialog() == true)
                            {
                                Sort = sortEditor.GetSelectedSort();
                            }
                        },
                        param => !String.IsNullOrEmpty(Connection) && !String.IsNullOrEmpty(Select)
                        );
                }
                return _editSort;
            }
        }

        CancellationTokenSource cts;
        RelayCommand _getDataItems;
        [Browsable(false)]
        public ICommand GetDataItems
        {
            get
            {
                if (_getDataItems == null)
                {
                    _getDataItems = new RelayCommand(
                        param =>
                        {
                            IsBusy = true;
                            IsFault = false;

                            _dataItems = null;
                            OnPropertyChanged("DataItems");

                            var task3 = Task.Factory.StartNew(delegate
                            {
                                return DataReader.DataReader.ListColumns(DataProvider, Connection, Select, GroupBy);
                            });
                            var task4 = task3.ContinueWith(ret =>
                            {
                                var timecolslist = (from c in ret.Result.AsParallel()
                                                    where c.Value == typeof(DateTime)
                                                    select c.Key).ToList();

                                var datacolslist = (from c in ret.Result.AsParallel()
                                                    where c.Value == typeof(Double) ||
                                                          c.Value == typeof(float) ||
                                                          c.Value == typeof(short) ||
                                                          c.Value == typeof(int) ||
                                                          c.Value == typeof(long)
                                                    select c.Key).ToList();
                                _timeColumns = new ObservableCollection<string>(timecolslist);
                                _dataColumns = new ObservableCollection<string>(datacolslist);

                                OnPropertyChanged("TimeColumns");
                                OnPropertyChanged("DataColumns");
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            if (cts != null)
                                cts.Dispose();
                            cts = new CancellationTokenSource();
                            var task1 = Task.Factory.StartNew(delegate
                            {
                                if (!String.IsNullOrEmpty(XmlUri) && !String.IsNullOrEmpty(XmlItems))
                                    return Utilities.XmlHelper.GetExpandoCTSFromXml(XmlUri, XmlItems, cts);

                                if (String.IsNullOrEmpty(DataProvider) || String.IsNullOrEmpty(Connection) ||
                                    String.IsNullOrEmpty(Select))
                                    return null;
                                return DataReader.DataReader.GetDynamicSqlData(DataProvider, Connection,
                                                            Select, Model.WhereClause(), Model.GroupBy, Sort, cts);
                            });

                            var task2 = task1.ContinueWith(ret =>
                            {
                                // cts.Dispose();
                                using (var cursor = new WaitCursor())
                                {
                                    cts = null;
                                    _dataItems = new ObservableCollection<dynamic>(ret.Result);
                                    OnPropertyChanged("DataItems");
                                    IsBusy = false;
                                    IsFault = false;
                                }
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            task1.ContinueWith((t) =>
                                {
                                    // cts.Dispose();
                                    cts = null;
                                    IsBusy = false;
                                    IsFault = true;
                                    LastError = t.Exception.InnerException.Message;
                                    Debug.WriteLine("I have observed a {0}",
                                        t.Exception.InnerException.GetType().Name);
                                }, TaskContinuationOptions.OnlyOnFaulted);
                            task2.ContinueWith((t) =>
                            {
                                // cts.Dispose();
                                cts = null;
                                IsBusy = false;
                                IsFault = true;
                                LastError = t.Exception.InnerException.Message;
                                Debug.WriteLine("I have observed a {0}",
                                    t.Exception.InnerException.GetType().Name);
                            },TaskContinuationOptions.OnlyOnFaulted);
                            task3.ContinueWith((t) =>
                            {
                                // cts.Dispose();
                                cts = null;
                                IsBusy = false;
                                IsFault = true;
                                LastError = t.Exception.InnerException.Message;
                                Debug.WriteLine("I have observed a {0}",
                                    t.Exception.InnerException.GetType().Name);
                            },TaskContinuationOptions.OnlyOnFaulted);
                            task4.ContinueWith((t) =>
                            {
                                // cts.Dispose();
                                cts = null;
                                IsBusy = false;
                                IsFault = true;
                                LastError = t.Exception.InnerException.Message;
                                Debug.WriteLine("I have observed a {0}",
                                    t.Exception.InnerException.GetType().Name);
                            },TaskContinuationOptions.OnlyOnFaulted);

                        },

                        param => !IsBusy && !String.IsNullOrEmpty(Connection) && !String.IsNullOrEmpty(Select) ||
                                 !String.IsNullOrEmpty(XmlUri) && !String.IsNullOrEmpty(XmlItems)
                        );
                }
                return _getDataItems;
            }
        }

        RelayCommand _clearCommand;
        [Browsable(false)]
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand(
                        param =>
                        {
                            var str = param as String;
                            if (str == "DataItems")
                            {
                                if (cts != null)
                                    cts.Cancel();
                                _dataItems = null;
                                OnPropertyChanged("DataItems");
                            }
                            else if (str == "Connection")
                            {
                                Connection = null;
                                DataProvider = null;
                            }
                            else if (str == "Select")
                            {
                                Select = null;
                                MaxTake = 10;
                            }
                            else if (str == "Where")
                            {
                                Where = null;
                            }
                            else if (str == "GroupBy")
                            {
                                GroupBy = null;
                            }
                            else if (str == "Sort")
                            {
                                Sort = null;
                            }
                            else if (str == "XmlUri")
                            {
                                XmlUri = null;
                            }
                            else if (str == "XmlItems")
                            {
                                XmlItems = null;
                            }
                            else if (str == "DataColumn")
                            {
                                DataColumn = null;
                            }
                            else if (str == "TimeColumn")
                            {
                                TimeColumn = null;
                            }
                        },
                        param => 
                            {
                                var str = param as String;
                                if (str == "DataItems")
                                {
                                    return _dataItems != null;
                                }
                                else if (str == "Connection")
                                {
                                    return !String.IsNullOrEmpty(Connection);
                                }
                                else if (str == "Select")
                                {
                                    return !String.IsNullOrEmpty(Select);
                                }
                                else if (str == "Where")
                                {
                                    return !String.IsNullOrEmpty(Where);
                                }
                                else if (str == "GroupBy")
                                {
                                    return !String.IsNullOrEmpty(GroupBy);
                                }
                                else if (str == "Sort")
                                {
                                    return !String.IsNullOrEmpty(Sort);
                                }
                                else if (str == "XmlUri")
                                {
                                    return !String.IsNullOrEmpty(XmlUri);
                                }
                                else if (str == "XmlItems")
                                {
                                    return !String.IsNullOrEmpty(XmlItems);
                                }
                                else if (str == "DataColumn")
                                {
                                    return !String.IsNullOrEmpty(DataColumn);
                                }
                                else if (str == "TimeColumn")
                                {
                                    return !String.IsNullOrEmpty(TimeColumn);
                                }
                                return false;
                            }
                        );
                }
                return _clearCommand;
            }
        }
        #endregion

    }
}
