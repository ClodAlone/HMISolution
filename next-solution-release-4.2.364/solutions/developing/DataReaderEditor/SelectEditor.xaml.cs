using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Data;
using DataReader.SchemaInfo;

namespace DataReaderEditor
{
    /// <summary>
    /// Interaction logic for SelectEditor.xaml
    /// </summary>
    public partial class SelectEditor : UserControl
    {
        #region Declarations
        readonly String ProviderName;
        readonly String ConnString;
        readonly DbSchemaInfo dbSchemaInfo;

        public String Select;
        public int MaxTake = 10;
        public string TableName;
        #endregion

        #region Constructors
        public SelectEditor(String providerName, String connString, String select, int max)
        {
            InitializeComponent();

            ProviderName = providerName;
            ConnString = connString;
            MaxTake = max;
            txtMaxTake.Text = MaxTake.ToString();

            DbSchemaInfoFactory.TryCreateSchemaInfo(ProviderName, ConnString, out dbSchemaInfo);

            try
            {
                listTables.ItemsSource = DataReader.DataReader.ListTables(providerName, connString, useSchemaQuotes: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.DBError, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ParseQuery(select);
        }
        #endregion

        #region Private Methods
        private void listTables_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listTables.SelectedValue == null || dbSchemaInfo == null)
                return;

            if (MaxTake == 0 || !dbSchemaInfo.IsSupportedTopKeyword)
                Select = String.Format("Select * from {0}", listTables.SelectedValue.ToString());
            else
                Select = String.Format("Select top {0} * from {1}", MaxTake, listTables.SelectedValue.ToString());

            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnString))
            {
                try
                {
                    connection.Open();

                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = Select;

                    var ds = new DataSet();
                    dbdapater.FillSchema(ds, SchemaType.Source);

                    var listString = new List<String>();
                    if (ds.Tables.Count > 0)
                    {
                        var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, ds.Tables[0], null);
                        if (columns != null)
                        {
                            foreach (DataColumnInfo column in columns)
                                listString.Add(dbSchemaInfo.WrapObjectName(column.Name));
                        }
                    }
                    listColums.ItemsSource = listString;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Properties.Resources.DBError, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void listColums_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CreateQuery();
        }

        private void btnSelectAllColumns_Click(object sender, RoutedEventArgs e)
        {
            listColums.SelectAll();
        }
        private void btnUnselectAllColumns_Click(object sender, RoutedEventArgs e)
        {
            listColums.UnselectAll();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            SetMaxTake();
            CreateQuery();
        }

        void SetMaxTake()
        {
            try
            {
                MaxTake = Convert.ToInt32(txtMaxTake.Text);
            }
            catch (Exception ex)
            {
                MaxTake = 0;
            }
        }
        
        void CreateQuery()
        {
            if (listTables.SelectedValue == null)
                return;

            if (listColums.SelectedItems.Count == 0)
            {
                if (MaxTake == 0 || dbSchemaInfo == null || !dbSchemaInfo.IsSupportedTopKeyword)
                    Select = String.Format("Select * from {0}", listTables.SelectedValue.ToString());
                else
                    Select = String.Format("Select top {0} * from {1}", MaxTake, listTables.SelectedValue.ToString());
            }
            else
            {
                String selectedItems = String.Format("{0}", listColums.SelectedItems[0]);
                for (int i = 1; i < listColums.SelectedItems.Count; ++i)
                {
                    selectedItems += String.Format(", {0}", listColums.SelectedItems[i]);
                }
                if (MaxTake == 0 || dbSchemaInfo == null || !dbSchemaInfo.IsSupportedTopKeyword)
                    Select = String.Format("Select {0} from {1}", selectedItems, listTables.SelectedValue.ToString());
                else
                    Select = String.Format("Select top {0} {1} from {2}", MaxTake, selectedItems, listTables.SelectedValue.ToString());
            }
        }

        void ParseQuery(string select)
        {
            if (String.IsNullOrEmpty(select))
                return;

            var sqlParser = new Helper.SqlParser();
            sqlParser.Parse(select);

            if (sqlParser.TableName != null)
            {
                listTables.SelectedItem = sqlParser.TableName;
                listTables.ScrollIntoView(sqlParser.TableName);
            }
            if (sqlParser.SelectColumnNames != null)
            {
                foreach (var column in sqlParser.SelectColumnNames)
                    listColums.SelectedItems.Add(column);

                if (listColums.SelectedItems.Count > 0)
                    listColums.ScrollIntoView(listColums.SelectedItems[listColums.SelectedItems.Count - 1]);
            }

            txtMaxTake.Text = sqlParser.TopClause ?? "0";
            SetMaxTake();
        }
        #endregion
    }
}
