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
using DevExpress.Data.Filtering;
using DataReader.SchemaInfo;

namespace DataReaderEditor
{
    /// <summary>
    /// Interaction logic for WhereEditor.xaml
    /// </summary>
    public partial class WhereEditor : UserControl
    {
        #region Declarations
        readonly String ProviderName;
        readonly String ConnString;
        readonly DbSchemaInfo dbSchemaInfo;
        #endregion

        #region Constructors
        public WhereEditor(String providerName, String connString, String select, String where)
        {
            InitializeComponent();

            ProviderName = providerName;
            ConnString = connString;

            DbSchemaInfoFactory.TryCreateSchemaInfo(ProviderName, ConnString, out dbSchemaInfo);

            FillColumns(select);
            ParseQuery(where);
        }
        #endregion

        #region Public Methods
        public String GetSelectedFilter()
        {
            if (dbSchemaInfo != null && dbSchemaInfo.DataSourceProductName.Contains("Microsoft Access"))
                return CriteriaToWhereClauseHelper.GetAccessWhere(filterControl.ActualFilterCriteria);
            else if (dbSchemaInfo != null && dbSchemaInfo.DataSourceProductName.Contains("Microsoft SQL Server"))
                return CriteriaToWhereClauseHelper.GetMsSqlWhere(filterControl.ActualFilterCriteria);
            else if (dbSchemaInfo != null && dbSchemaInfo.DataSourceProductName.Contains("Oracle"))
                return CriteriaToWhereClauseHelper.GetOracleWhere(filterControl.ActualFilterCriteria);
            else
                return CriteriaToWhereClauseHelper.GetDataSetWhere(filterControl.ActualFilterCriteria);
        }
        #endregion

        #region Private Methods
        void FillColumns(String select)
        {
            try
            {
                using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnString))
                {
                    connection.Open();

                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = select;

                    var ds = new DataSet();
                    dbdapater.FillSchema(ds, SchemaType.Source);

                    var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, ds.Tables[0], null);
                    var list = new List<FilterColumn>();
                    var listString = new List<String>();
                    if (columns != null)
                    {
                        foreach (DataColumnInfo column in columns)
                        {
                            list.Add(new DataColumnInfoFilterColumn(column));
                            listString.Add(column.Name);
                        }
                        filterControl.FilterColumns = list;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.DBError, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void ParseQuery(string where)
        {
            if (where == null)
                return;

            where = where.Replace("\"", "");
            filterControl.FilterCriteria = CriteriaOperator.TryParse(where);
        }
        #endregion
    }
}
