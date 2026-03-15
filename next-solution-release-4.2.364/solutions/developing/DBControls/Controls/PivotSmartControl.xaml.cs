using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DataReader;
using log4net;
using Utilities;
using Utilities.WPF;
using DevExpress.Data;
using System.ComponentModel;
using System.Threading;
using UIMsgBoxAlertService.ComponentService;
using System.Collections.ObjectModel;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;

namespace DBControls.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class PivotSmartControl : UserControl
    {
        #region Declarations
        PivotGrid control;
        ControlDataSourceItem dataSourceItem;
        ObservableCollection<ColumnItem> datalist;
        Window parentWindow;

        IUIMsgBoxAlertService uiMsgBox;

        string tableName;
        string historianDefaultConnection;
        string historianDefaultProvider;
        IUFProjectManager iUFProjectManager;
        IDocument document;
        bool bLoaded;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.DBControlLog);
        #endregion

        #region Constructors
        public PivotSmartControl(PivotGrid c)
        {
            InitializeComponent();

            control = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                document = control.Document;
                if (document != null)
                {
                    iUFProjectManager = document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    uiMsgBox = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                }

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                historianDefaultProvider = control.DefaultDataProvider;
                historianDefaultConnection = control.DefaultConnectionString;
                if (control.ControlDataSource != null)
                    tableName = control.ControlDataSource.TableName;

                dataSourceItem = new ControlDataSourceItem(control.ControlDataSource);
                if (dataSourceItem.ColumnListSettings == null)
                    dataSourceItem.ColumnListSettings = new ColumnItemList();
                datalist = new ObservableCollection<ColumnItem>(dataSourceItem.ColumnListSettings);
                gridControl.ItemsSource = datalist;
            };
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataReaderModel dataReaderModel = null;
            if (dataSourceItem != null && dataSourceItem.ControlDataSource != null)
                dataReaderModel = dataSourceItem.ControlDataSource;
            if (dataReaderModel == null)
                dataReaderModel = new DataReaderModel();

            var dataReaderEditor = new DataReaderEditor.DataReaderEditor(dataReaderModel, control.Document?.rootBase, false);
            var Dialog = new GeneralDialogContent(dataReaderEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DataSourceSelector,
                HelpLink = "DataSourceSelector"
            };
            Dialog.Closing += (o, ev) =>
            {
                if (Dialog.DialogResult == true)
                {
                    if (String.IsNullOrEmpty(dataReaderModel.Select))
                    {
                        if (uiMsgBox != null)
                            uiMsgBox.ShowWarning(Properties.Resources.InvalidDataSource);
                        ev.Cancel = true;
                    }
                }
            };
            if (Dialog.ShowDialog() == true)
            {
                //if (dataSourceItem == null || dataSourceItem.ControlDataSource == null ||
                //    !dataSourceItem.ControlDataSource.Equals(dataReaderModel))
                {
                    dataSourceItem = new ControlDataSourceItem()
                    {
                        ControlDataSource = dataReaderModel,
                        ColumnListSettings = InitColumnList(dataReaderModel),
                        TableName = tableName
                    };
                    datalist = new ObservableCollection<ColumnItem>(dataSourceItem.ColumnListSettings);
                    gridControl.ItemsSource = datalist;
                }
            }
        }

        private ColumnItemList InitColumnList(DataReaderModel DataSource)
        {
            if ((DataSource == null && (string.IsNullOrEmpty(historianDefaultProvider) || string.IsNullOrEmpty(historianDefaultConnection)))
                || string.IsNullOrEmpty(DataSource.Select))
                return new ColumnItemList();

            ColumnItemList _oldlist = new ColumnItemList(datalist.ToList());
            ColumnItemList _list = GetDataSetSchema(DataSource, out tableName);

            if (_oldlist != null && _oldlist.Count > 0)
                foreach (ColumnItem s in _list)
                {
                    var item = _oldlist.Find(x => x.ColName.Equals(s.ColName));
                    if (item != null)
                    {
                        s.Caption = item.Caption;
                        s.IsEditable = item.IsEditable;
                        s.IsVisible = item.IsVisible;
                    }
                }

            return _list;
        }

        private ColumnItemList GetDataSetSchema(DataReaderModel DataSource, out string tableName)
        {
            ColumnItemList res = new ColumnItemList();

            var defaultDataProvider = historianDefaultProvider;
            var defaultConnectionString = historianDefaultConnection;

            if (DataSource != null &&
                !string.IsNullOrEmpty(DataSource.DataProvider) &&
                !string.IsNullOrEmpty(DataSource.Connection))
            {
                defaultDataProvider = DataSource.DataProvider;
                defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(DataSource.Connection, document?.rootBase);
            }

            if (DataSource == null && string.IsNullOrEmpty(DataSource.Select))
            {
               log.Error(Properties.Resources.ErrDataSource);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(document, Properties.Resources.DBControlLog, DateTime.UtcNow, 
                    $"{Properties.Resources.ErrDataSource}", System.Diagnostics.EventLogEntryType.Error);
                tableName = string.Empty;
                return res;
            }

            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                using (var connection = DataReader.DataReader.CreateDbConnection(defaultDataProvider, defaultConnectionString))
                {
                    try
                    {
                        connection.Open();

                        var dbdapater = DataReader.DataReader.CreateDbDataAdapter(defaultDataProvider);
                        dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(defaultDataProvider);
                        dbdapater.SelectCommand.Connection = connection;
                        if (!String.IsNullOrEmpty(DataSource.GroupBy))
                            dbdapater.SelectCommand.CommandText = String.Format("{0} Group By {1}", DataSource.Select, DataSource.GroupBy);
                        else
                            dbdapater.SelectCommand.CommandText = DataSource.Select;

                        dbdapater.FillSchema(dataTable, SchemaType.Source);
                        dbdapater.Fill(ds);
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        tableName = string.Empty;
                        return res;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(Name, ex);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(document, Properties.Resources.DBControlLog, DateTime.UtcNow,
                    $"{Name}: {ex.Message} - {ex.StackTrace}", System.Diagnostics.EventLogEntryType.Error);
                tableName = string.Empty;
                return res;
            }

            tableName = dataTable.TableName;

            var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(ds.Tables[0]);
            if (columns != null)
            {
                foreach (DataColumnInfo column in columns)
                {
                    res.Add(new ColumnItem()
                    {
                        ColName = column.Name,
                        Caption = column.Name,
                        IsEditable = true,
                        IsVisible = true
                    });
                }
            }

            return res;
        }
        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                dataSourceItem.ColumnListSettings = new ColumnItemList(datalist.ToList());
                control.NeedsUpdate = true;
                control.ControlDataSource = dataSourceItem;
            }
        }
    }
}
