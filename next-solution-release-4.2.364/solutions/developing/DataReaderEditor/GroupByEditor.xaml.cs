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
    /// Interaction logic for GroupByEditor.xaml
    /// </summary>
    public partial class GroupByEditor : UserControl
    {
        #region Declarations
        readonly String ProviderName;
        readonly String ConnString;
        readonly DbSchemaInfo dbSchemaInfo;
        #endregion

        #region Constructors
        public GroupByEditor(String providerName, String connString, String select, String groupby)
        {
            InitializeComponent();

            ProviderName = providerName;
            ConnString = connString;

            DbSchemaInfoFactory.TryCreateSchemaInfo(ProviderName, ConnString, out dbSchemaInfo);

            FillColumns(select);
            ParseQuery(groupby);            
        }
        #endregion

        #region Public Methods
        public String GetSelectedGroupBy()
        {
            if (listColums.SelectedItems.Count == 0)
                return String.Empty;
            else
            {
                String selectedItems = String.Format("{0}", listColums.SelectedItems[0]);
                for (int i = 1; i < listColums.SelectedItems.Count; ++i)
                {
                    selectedItems += String.Format(", {0}", listColums.SelectedItems[i]);
                }
                return selectedItems;
            }
        }
        #endregion

        #region Private Methods
        void FillColumns(String select)
        {
            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnString))
            {
                try
                {
                    connection.Open();

                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = select;

                    var ds = new DataSet();
                    dbdapater.FillSchema(ds, SchemaType.Source);

                    var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, ds.Tables[0], null);
                    var listString = new List<String>();
                    if (columns != null)
                    {
                        foreach (DataColumnInfo column in columns)
                        {
                            var columnName = column.Name;
                            if (dbSchemaInfo != null)
                                columnName = dbSchemaInfo.WrapObjectName(columnName);
                            listString.Add(columnName);
                        }
                        listColums.ItemsSource = listString;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Properties.Resources.DBError, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void ParseQuery(string groupby)
        {
            if (groupby == null)
                return;

            var columnNames = groupby.Split(',');
            foreach (var column in columnNames)
                listColums.SelectedItems.Add(column.Trim());

            if (listColums.SelectedItems.Count > 0)
                listColums.ScrollIntoView(listColums.SelectedItems[listColums.SelectedItems.Count - 1]);
        }
        #endregion
    }
}
