//-------------------------------------------------------------------------------------------------
// <copyright file="QueryDesigner.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Relational.Sql;
using System.Data;
using System.Data.SqlClient;
using Syncfusion.Windows.Reports.Sql;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Reports.Common;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.Data.Common;
using Syncfusion.RDL.Data;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    [ObsoleteAttribute("OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    internal partial class QueryDesigner
        : ChromelessWindow
    {
        #region Members
      
        private string reportDataSourceName;

        private string ConnectionString;

        private string error_title;

        //private bool textChanged ;

        private System.Data.Common.DbConnection dbConnection;

        List<SchemaInfo> listOfSchema;

        List<SchemaInfo> selectedTables;

        bool storedProcedure;

        DataTable dataTableRelations;

        internal DataTable queryResult;

        #endregion        

        #region Public Properties

        /// <summary>
        /// Gets or sets the report data source.
        /// </summary>
        /// <value>The report data source.</value>
        public Syncfusion.RDL.DOM.DataSource DataSource { get; set; }

        /// <summary>
        /// Gets or sets the query report settings.
        /// </summary>
        /// <value>The query report settings.</value>
        public ReportDefinition QueryReportSettings { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString { get; set; }

        public object FolderImage { get; set; }

        #endregion

        #region Constructors

        public QueryDesigner(string QueryString, RDL.DOM.DataSource dataSource, bool storedProcedure, string visualStyle)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(QueryDesigner_Loaded);
            this.tgbtn_EditText.Click += new RoutedEventHandler(tgbtn_EditText_Click);
            this.btn_RunQuery.Click += new RoutedEventHandler(btn_RunQuery_Click);
            this.btn_QueryDesignerOk.Click += new RoutedEventHandler(btn_QueryDesignerOk_Click);
            this.btn_QueryDesignerCancel.Click += new RoutedEventHandler(btn_QueryDesignerCancel_Click);

            this.storedProcedure = storedProcedure;
            this.QueryString = QueryString;
            this.QueryReportSettings = new ReportDefinition();
            reportDataSourceName = string.Empty;
            //this.ConnectionString = string.Empty;
            if (visualStyle == "Metro")
            {
                this.FolderImage = this.Resources["MetroFolderClose"];
            }
            else if (visualStyle == "Blend")
            {
                this.FolderImage = this.Resources["BlendFolderClose"];
            }
            else
            {
                this.FolderImage = this.Resources["FolderClose"];
            }
            if (ReportDesignView.CurrentPanel != null)
            {
                if (!ReportDesignView.CurrentPanel.ShowHelp)
                {
                    this.btn_HelponQueryDesigner.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            this.listOfSchema = new List<SchemaInfo>();
            this.DataSource = dataSource;
            this.ConnectionString = dataSource.ConnectionProperties.ConnectString; ;
            this.OpenConnection();
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleQueryDesigner");
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleQueryDesigner");
        }

        private void OpenConnection()
        {
            try
            {
                switch (this.DataSource.ConnectionProperties.DataProvider)
                {
                    case DataProviders.SQLServer:
                    case DataProviders.SQLAzure:
                        {
                            this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;
                            if (this.DataSource.ConnectionProperties.UserName != null)
                            {
                                this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName.ToString();
                                this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord.ToString();
                            }
                            if (this.DataSource.ConnectionProperties.IntegratedSecurity)
                            {
                                this.ConnectionString += "; Trusted_Connection=true";
                            }
                            OpenSqlDBConnection();
                        }
                        break;
                    case DataProviders.SQLServerCe:
                        {
                            OpenSqlCeDBConnection();
                        }
                        break;
                }

                PopulateSchemas();
                trvw_Schemas.ItemsSource = this.listOfSchema;
            }
            catch { }

            if ((this.listOfSchema == null || (this.listOfSchema != null && this.listOfSchema.Count == 0)))
            {
                this.tgbtn_EditText.IsChecked = true;
                this.tgbtn_EditText.IsEnabled = false;
                rtbox_TextView.Text = this.QueryString;
                this.grd_TreeView.Visibility = System.Windows.Visibility.Collapsed;
                this.grd_TextView.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.storedProcedure == true)
            {
                this.tgbtn_EditText.IsChecked = true;
                rtbox_TextView.Text = this.QueryString;
                this.grd_TreeView.Visibility = System.Windows.Visibility.Collapsed;
                this.grd_TextView.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.tgbtn_EditText.IsChecked = false;
                this.tgbtn_EditText.IsEnabled = true;
                this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void OpenSqlCeDBConnection()
        {
            this.dbConnection = new System.Data.SqlServerCe.SqlCeConnection(this.ConnectionString);
            dbConnection.Open();
        }

        private void OpenSqlDBConnection()
        {
            this.dbConnection = new SqlConnection(this.ConnectionString);
            dbConnection.Open();
        }

        #endregion

        #region Helping Methods

        void QueryDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.listOfSchema == null || (this.listOfSchema != null && this.listOfSchema.Count == 0))
            {
                try
                {
                    if (!this.DataSource.ConnectionProperties.IntegratedSecurity)
                    {
                        Credentials Credentials = new Credentials(this.DataSource);
                        Credentials.Owner = this;
                        SkinStorage.SetVisualStyle(Credentials, SkinStorage.GetVisualStyle(this));
                        if (Credentials.ShowDialog() == true)
                        {
                            this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName;
                            this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord;
                        }
                        if (Credentials.isCancel)
                        {
                            this.DialogResult = false;
                            this.Close();
                        }
                    }

                    this.OpenConnection();
                }
                catch { }
            }
        }

        private void chck_TableNode_Click(object sender, RoutedEventArgs e)
        {
            selectedTables = new List<SchemaInfo>();
            lvw_SelectedFields.Items.Clear();
            selectedTables = GetSelectedNode(listOfSchema);

            try
            {
                switch (this.DataSource.ConnectionProperties.DataProvider)
                {
                    case DataProviders.SQLServer:
                    case DataProviders.SQLAzure:
                        {
                            OpenSqlDBConnection();
                        }
                        break;
                    case DataProviders.SQLServerCe:
                        {
                            OpenSqlCeDBConnection();
                        }
                        break;
                    case DataProviders.ORACLE:
                        {
                            OpenOracleConnection();
                        }
                        break;
                } 

                if (selectedTables.Count > 0)
                {
                    switch (this.DataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                        case DataProviders.SQLServerCe:
                            {
                                SetQueryForSql();
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                SetQueryForOracle();
                            }
                            break;
                    } 
                }

                foreach (var item in selectedTables)
                {
                    if (item.IsSelected)
                    {
                        foreach (var tableColumns in item.SchemaInfos)
                        {
                            if (tableColumns.IsSelected)
                            {
                                Field dataSetField = new Field();
                                dataSetField.Name = tableColumns.Key;
                                dataSetField.DataField = tableColumns.Key;
                                dataSetField.TypeName = tableColumns.DataType;

                                var columns = from column in lvw_SelectedFields.Items.Cast<string>()
                                              where column.Equals(tableColumns.Key)
                                              select column;

                                if (columns.Count()>0)
                                {
                                    dataSetField.Name = tableColumns.Key + "1";
                                    dataSetField.DataField = dataSetField.Name;
                                    lvw_SelectedFields.Items.Add(dataSetField.Name);
                                }

                                else
                                {
                                    dataSetField.Name = tableColumns.Key;
                                    dataSetField.DataField = tableColumns.Key;
                                    lvw_SelectedFields.Items.Add(tableColumns.Key);
                                }
                            }
                        }
                    }
                }

                dataTableRelations = new DataTable();

                switch (this.DataSource.ConnectionProperties.DataProvider)
                {
                    case DataProviders.SQLServer:
                    case DataProviders.SQLAzure:
                        {
                            try
                            {
                                if (this.selectedTables.Count > 0)
                                {
                                    dataTableRelations = GetTableRelations();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, this.error_title);
                            }
                            if (dataTableRelations.Rows.Count > 0)
                            {
                                this.ListViewRelationships.DataContext = dataTableRelations;
                            }
                            else
                            {
                                DataTable dataTableNoRelations = new DataTable("Relations");

                                dataTableNoRelations.Columns.Add("LeftTable", typeof(string));
                                dataTableNoRelations.Columns.Add("RelationShip", typeof(string));
                                dataTableNoRelations.Columns.Add("RightTable", typeof(string));

                                foreach (var item in this.selectedTables)
                                {
                                    dataTableNoRelations.Rows.Add(item.Key, "Unrelated", string.Empty);
                                }
                                this.ListViewRelationships.DataContext = dataTableNoRelations;
                            }
                        }

                        break;                    
                } 

                dbConnection.Close();
                dbConnection.Dispose();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, this.error_title);
            }
        }

        private DataTable GetTableRelations()
        {
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            return sqlSchemaProvider.GetTableRelations(GetFields(this.selectedTables));
        }

        private void SetQueryForOracle()
        {
            OracleSchemaProvider MySchemaProvider = new OracleSchemaProvider(dbConnection);
            MySchemaProvider.GetQueryResults(selectedTables);
            this.QueryString = MySchemaProvider.QueryString;
        }

        private void SetQueryForSql()
        {
            SqlSchemaProvider MySchemaProviderquery = new SqlSchemaProvider(dbConnection);
            MySchemaProviderquery.GetQueryResults(selectedTables);
            this.QueryString = MySchemaProviderquery.QueryString;
        }

        private void OpenOracleConnection()
        {
            dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            dbConnection.Open();
        }

        private string GetFields(List<SchemaInfo> list)
        {
            StringBuilder tempString = new StringBuilder();
            int i = 0;
            foreach (SchemaInfo schemaInfo in list)
            {
                if (i > 0)
                {
                    tempString.Append(",");
                }
                tempString.Append("'" + schemaInfo.Key + "'");
                i++;
            }
            return tempString.ToString();
        }

        void btn_QueryDesignerCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void btn_QueryDesignerOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.RunQueryResult() || this.storedProcedure)
            {
                if (!(bool)this.tgbtn_EditText.IsChecked)
                {
                    if (this.lvw_SelectedFields.Items.Count > 0)
                    {
                        //                    this.QueryString = this.rtbox_TextView.Text;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectTableOrQuery"), this.error_title);
                    }
                }
                else if ((bool)this.tgbtn_EditText.IsChecked)
                {
                    this.QueryString = this.rtbox_TextView.Text;
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void btn_RunQuery_Click(object sender, RoutedEventArgs e)
        {
            this.dgrd_QueryResults.IsHitTestVisible = true;
            this.RunQueryResult();
            this.dgrd_QueryResults.Model.TableStyle.Foreground = ((Syncfusion.Windows.Controls.Grid.IGridDataVisualStyle)(this.dgrd_QueryResults.Model.GridVisualStyle)).ValueForegroundBrush;
        }

        private bool RunQueryResult()
        {
            if ((bool)this.tgbtn_EditText.IsChecked)
            {
                this.QueryString = this.rtbox_TextView.Text;

                if (this.QueryString != string.Empty)
                {
                    this.queryResult = new DataTable();
                    try
                    {
                        switch (this.DataSource.ConnectionProperties.DataProvider)
                        {
                            case DataProviders.SQLServer:
                            case DataProviders.SQLAzure:
                                {
                                    this.queryResult = GetSqlTable();
                                }
                                break;
                            case DataProviders.ORACLE:
                                {
                                    this.queryResult = GetOracleTable();
                                }
                                break;
                            case DataProviders.SQLServerCe:
                                {
                                    this.queryResult = GetSqlceTable();
                                }
                                break;
                        }

                        this.queryResult.TableName = "QueryResults"; //Setting TableName as QueryResults
                        this.dgrd_QueryResults.ItemsSource = this.queryResult;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, this.error_title);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyQuery"), this.error_title);
                }
            }
            else
            {
                //this.textChanged = false;
                if (this.ConnectionString != null)
                {
                    List<SchemaInfo> tempSchemaInfos = new List<SchemaInfo>();
                    tempSchemaInfos = this.GetSelectedNode(this.listOfSchema);
                    if (tempSchemaInfos.Count > 0)
                    {
                        switch (this.DataSource.ConnectionProperties.DataProvider)
                        {
                            case DataProviders.SQLServer:
                            case DataProviders.SQLAzure:
                                {
                                    UpdateSqlSchema(tempSchemaInfos);
                                }
                                break;
                            case DataProviders.ORACLE:
                                {
                                    UpdateOracleSchema(tempSchemaInfos);
                                }
                                break;
                            case DataProviders.SQLServerCe:
                                {
                                    UpdateSQLCeSchema(tempSchemaInfos);
                                }
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectQueryToRun"), this.error_title);
                    }
                }
            }

            return true;
        }

        private void UpdateSQLCeSchema(List<SchemaInfo> tempSchemaInfos)
        {
            DataTable dataTable = this.GetSqlceTable();
            dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults
            this.queryResult = dataTable;
            this.dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateOracleSchema(List<SchemaInfo> tempSchemaInfos)
        {
            System.Data.OracleClient.OracleConnection dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            dbConnection.Open();
            OracleSchemaProvider oracleSchemaProvider = new OracleSchemaProvider(dbConnection);
            DataTable dataTable = new DataTable();

            oracleSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = oracleSchemaProvider.QueryString;
            dataTable = oracleSchemaProvider.GetQueryTables(this.QueryString);
            dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults
            this.queryResult = dataTable;
            this.dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateSqlSchema(List<SchemaInfo> tempSchemaInfos)
        {
            using (SqlConnection dbConnection = new SqlConnection(this.ConnectionString))
            {
                dbConnection.Open();
                SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
                DataTable dataTable = new DataTable();

                sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
                this.QueryString = sqlSchemaProvider.QueryString;
                dataTable = sqlSchemaProvider.GetQueryTables(Util.ReturnSQLTop(this.QueryString));
                dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults
                this.queryResult = dataTable;
                this.dgrd_QueryResults.ItemsSource = dataTable;
            }
        }

        private DataTable GetOracleTable()
        {
            using (System.Data.OracleClient.OracleConnection dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString))
            {
                dbConnection.Open();

                OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);
                DataTable dataTable = sqlSchemaProvider.GetQueryTables(this.QueryString);
                return dataTable;
            }
        }

        private DataTable GetSqlTable()
        {
            using (SqlConnection dbConnection = new SqlConnection(this.ConnectionString))
            {
                dbConnection.Open();

                SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
                DataTable dataTable = sqlSchemaProvider.GetQueryTables(Util.ReturnSQLTop(this.QueryString));
                return dataTable;
            }
        }

        private DataTable GetSqlceTable()
        {
            using (System.Data.SqlServerCe.SqlCeConnection connection = new System.Data.SqlServerCe.SqlCeConnection(this.ConnectionString))
            {
                connection.Open();
                DataTable dataTable = this.GetsqlceTable(this.QueryString, connection);
                return dataTable;
            }
        }
        
        private DataTable GetsqlceTable(string query, DbConnection connection)
        {
            using (System.Data.SqlServerCe.SqlCeCommand command = new System.Data.SqlServerCe.SqlCeCommand(query, (System.Data.SqlServerCe.SqlCeConnection)connection))
            {
                using (System.Data.SqlServerCe.SqlCeDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(sqlDataReader);
                    return table;
                }
            }
        }

        private void tgbtn_EditText_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConnectionString != null)
            {
                List<SchemaInfo> tempSchemaInfos = new List<SchemaInfo>();
                tempSchemaInfos = this.GetSelectedNode(this.listOfSchema);

                if (tempSchemaInfos.Count > 0)
                {
                    switch (this.DataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                UpdateSqlQueryText(tempSchemaInfos);
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                UpdateOracleQueryText(tempSchemaInfos);
                            }
                            break;
                    } 
                }
            }
            //rtbox_TextView.Text = this.QueryString;
            if ((bool)tgbtn_EditText.IsChecked)
            {
                rtbox_TextView.Text = this.QueryString;
                this.grd_TreeView.Visibility = System.Windows.Visibility.Collapsed;
                this.grd_TextView.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (rtbox_TextView.Text  != this.QueryString)
                {
                    if (MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxQueryDesignerSupport"), this.error_title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (this.rtbox_TextView.Text.ToString() == string.Empty)
                        {
                            UnCheckSelectedNodes();
                            this.trvw_Schemas.ItemsSource = null;
                            this.trvw_Schemas.ItemsSource = this.listOfSchema;
                            this.lvw_SelectedFields.Items.Clear();
                            this.QueryString = string.Empty;
                        }
                        this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                        this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
                        //this.textChanged = false;
                    }
                    else
                    {
                        tgbtn_EditText.IsChecked = true;
                    }
                }
                else
                {
                    if (this.rtbox_TextView.Text.ToString() == string.Empty)
                    {
                        UnCheckSelectedNodes();
                        this.trvw_Schemas.ItemsSource = null;
                        this.trvw_Schemas.ItemsSource = this.listOfSchema;
                        this.lvw_SelectedFields.Items.Clear();
                        this.QueryString = string.Empty;
                    }
                  
                    this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                    this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
                    //this.textChanged = false;
                }
            }
        }

        private void UpdateOracleQueryText(List<SchemaInfo> tempSchemaInfos)
        {
            System.Data.OracleClient.OracleConnection dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            dbConnection.Open();
            OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);
            sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = string.Empty;
            this.QueryString = sqlSchemaProvider.QueryString;
        }

        private void UpdateSqlQueryText(List<SchemaInfo> tempSchemaInfos)
        {
            SqlConnection dbConnection = new SqlConnection(this.ConnectionString);
            dbConnection.Open();
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = string.Empty;
            this.QueryString = sqlSchemaProvider.QueryString;
        }

        private void UnCheckSelectedNodes()
        {
            if (this.listOfSchema != null)
            {
                foreach (var item in this.listOfSchema)
                {
                    foreach (var childItem in item.SchemaInfos)
                    {
                        foreach (var tableItem in childItem.SchemaInfos)
                        {
                            if (tableItem.TreeNodeType == NodeType.Table
                                || tableItem.TreeNodeType == NodeType.View)
                            {
                                if (tableItem.IsSelected)
                                {
                                    tableItem.IsSelected = false;
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private void PopulateSchemas()
        {
            switch (this.DataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        PopulateSqlSchemas();
                    }
                    break;

                case DataProviders.SQLServerCe:
                    {
                        PopulateSqlCeSchemas();
                    }
                    break;

                case DataProviders.ORACLE:
                    {
                        PopulateOracleSchemas();
                    }
                    break;
            }

            ArrangeViewOrder(this.listOfSchema);
        }

        private List<SchemaInfo> ArrangeViewOrder(List<SchemaInfo> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (SchemaInfo schemaInfo in list.ToList())
                {
                    if (schemaInfo.SchemaInfos.Count > 0)
                    {
                        if (schemaInfo.TreeNodeType == NodeType.Folder)
                        {
                            schemaInfo.SchemaInfos = ArrangeViewOrder(schemaInfo.SchemaInfos);
                        }
                        else if (schemaInfo.TreeNodeType == NodeType.Table)
                        {
                            return list.OrderBy(schema => schema.Key).ToList();
                        }
                    }
                }
            }

            return list;
        }

        private void PopulateOracleSchemas()
        {
            string UserID = GetUserID(this.DataSource.ConnectionProperties.ConnectString).ToUpper();

            OracleSchemaProvider MySchemaProvider = new OracleSchemaProvider(this.dbConnection, UserID);
            DataTable dataTableSchemas = new DataTable();
            dataTableSchemas = MySchemaProvider.GetSchemas();
            dataTableSchemas.TableName = "Schemas";
            DataTable dataTable = new DataTable();
            dataTable = MySchemaProvider.GetTables();
            dataTable.TableName = "Tables";
            DataTable dataTableViews = new DataTable();
            dataTableViews = MySchemaProvider.GetViews();
            dataTableViews.TableName = "Views";
            DataTable dataTableStoredProc = new DataTable();
            //dataTableStoredProc = MySchemaProvider.GetStoredProcedures();
            dataTableStoredProc.TableName = "Stored Procedures";
            DataTable dataTableTableFunc = new DataTable();
            //dataTableTableFunc = MySchemaProvider.GetTableValueFunctions();
            dataTableTableFunc.TableName = "Table Valued Functions";
            DataTable dataTableTableColumns = new DataTable();
            dataTableTableColumns = MySchemaProvider.GetTableColumns();
            dataTableTableColumns.TableName = "Table Columns";
            DataTable dataTableViewColumns = new DataTable();
            dataTableViewColumns = MySchemaProvider.GetViewColumns();
            dataTableViewColumns.TableName = "View Columns";
            dbConnection.Close();
            listOfSchema = GetSchemaInfo(dataTableSchemas);
            UpdateSchemaDefaultChild(listOfSchema);
            //iterating the child nodes and updating the tables, stored procedures, views and functions
            foreach (var item in listOfSchema)
            {
                GetTableInfo("Table", dataTable, item, NodeType.Table);
                GetTableInfo("Views", dataTableViews, item, NodeType.View);
                //GetTableInfo("Stored Procedures", dataTableStoredProc, item, NodeType.StoredProcedure);
                //GetTableInfo("Table Value Functions", dataTableTableFunc, item, NodeType.TableValuedFunction);
                GetTableColumnInfo(item.Key, "Table", dataTableTableColumns, item, NodeType.TableColumn);
                GetTableColumnInfo(item.Key, "Views", dataTableViewColumns, item, NodeType.TableColumn);
            }
            RemoveEmptyChildNodes(this.listOfSchema);
        }

        private void PopulateSqlCeSchemas()
        {
            System.Data.SqlServerCe.SqlCeConnection con = this.dbConnection as System.Data.SqlServerCe.SqlCeConnection;
            DataTable dataTable = con.GetSchema("Tables");
            dataTable.TableName = "Table";
            DataTable dataTableTableColumns = con.GetSchema("Columns");//this.GetsqlceTable("SELECT TABLE_NAME,COLUMN_NAME ,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS;", this.dbConnection);
            dataTableTableColumns.TableName = "Table Columns";
            SchemaInfo info = new SchemaInfo { Key = "Tables", Parent = null, ImageSource = this.FolderImage };
            listOfSchema.Add(info);
            //  UpdateSchemaDefaultChild(listOfSchema);
            dbConnection.Close();
            foreach (var item in listOfSchema)
            {
                GetTableInfoSqlce("Table", dataTable, item, NodeType.Table);
                //GetTableInfo("Views", dataTableViews, item, NodeType.View);
                //GetTableInfo("Stored Procedures", dataTableStoredProc, item, NodeType.StoredProcedure);
                //GetTableInfo("Table Value Functions", dataTableTableFunc, item, NodeType.TableValuedFunction);
                GetTableColumnInfosqlce("Table", dataTableTableColumns, item, NodeType.TableColumn);
                //  GetTableColumnInfo(item.Key, "Views", dataTableViewColumns, item, NodeType.TableColumn);
            }
            RemoveEmptyChildNodes(this.listOfSchema);

        }

        private void PopulateSqlSchemas()
        {
            SqlSchemaProvider MySchemaProvider = new SqlSchemaProvider(this.dbConnection);
            DataTable dataTableSchemas = new DataTable();
            dataTableSchemas = MySchemaProvider.GetSchemas();
            dataTableSchemas.TableName = "Schemas";
            DataTable dataTable = new DataTable();
            dataTable = MySchemaProvider.GetTables();
            dataTable.TableName = "Tables";
            DataTable dataTableViews = new DataTable();
            dataTableViews = MySchemaProvider.GetViews();
            dataTableViews.TableName = "Views";
            DataTable dataTableStoredProc = new DataTable();
            //dataTableStoredProc = MySchemaProvider.GetStoredProcedures();
            dataTableStoredProc.TableName = "Stored Procedures";
            DataTable dataTableTableFunc = new DataTable();
            //dataTableTableFunc = MySchemaProvider.GetTableValueFunctions();
            dataTableTableFunc.TableName = "Table Valued Functions";
            DataTable dataTableTableColumns = new DataTable();
            dataTableTableColumns = MySchemaProvider.GetTableColumns();
            dataTableTableColumns.TableName = "Table Columns";
            DataTable dataTableViewColumns = new DataTable();
            dataTableViewColumns = MySchemaProvider.GetViewColumns();
            dataTableViewColumns.TableName = "View Columns";
            dbConnection.Close();
            listOfSchema = GetSchemaInfo(dataTableSchemas);
            UpdateSchemaDefaultChild(listOfSchema);
            //iterating the child nodes and updating the tables, stored procedures, views and functions
            foreach (var item in listOfSchema)
            {
                GetTableInfo("Table", dataTable, item, NodeType.Table);
                GetTableInfo("Views", dataTableViews, item, NodeType.View);
                //GetTableInfo("Stored Procedures", dataTableStoredProc, item, NodeType.StoredProcedure);
                //GetTableInfo("Table Value Functions", dataTableTableFunc, item, NodeType.TableValuedFunction);
                GetTableColumnInfo(item.Key, "Table", dataTableTableColumns, item, NodeType.TableColumn);
                GetTableColumnInfo(item.Key, "Views", dataTableViewColumns, item, NodeType.TableColumn);
            }
            RemoveEmptyChildNodes(this.listOfSchema);
        }

        void GetTableInfoSqlce(string key, DataTable tableInfo, SchemaInfo item, NodeType nodeType)
        {
            foreach (System.Data.DataRow tableRow in tableInfo.Rows)
            {
                item.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[2].ToString(), Parent = item, TreeNodeType = nodeType, ImageSource = this.FolderImage });
            }

        }

        void GetTableColumnInfosqlce(string keytype, DataTable tableColumnInfo, SchemaInfo item, NodeType nodeType)
        {
            foreach (var objectItem in item.SchemaInfos)
            {
                foreach (System.Data.DataRow tableRow in tableColumnInfo.Rows)
                {
                    if (objectItem.Key == tableRow.ItemArray[2].ToString())
                    {
                        objectItem.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[3].ToString(), Parent = objectItem, TreeNodeType = nodeType, DataType = tableRow.ItemArray[11].ToString(), ImageSource = this.FolderImage });
                    }
                }
            }

        }

        private string GetUserID(string fieldNameContainer)
        {
            string parsedFieldName = "";
            string startString = "User ID=";
            string endString = ";Password";
            if (fieldNameContainer.Contains(startString) && fieldNameContainer.Contains(endString))
            {
                int indexOfStarting = fieldNameContainer.IndexOf(startString);
                int endingIndex = fieldNameContainer.IndexOf(endString);
                int startingIndex = indexOfStarting + startString.Length;
                int stringLength = (endingIndex - startingIndex);
                parsedFieldName = fieldNameContainer.Substring(startingIndex, stringLength);
            }
            return parsedFieldName;
        }

        List<SchemaInfo> GetSchemaInfo(DataTable schemaTable)
        {
            List<SchemaInfo> schemaInfo = new List<SchemaInfo>();
            foreach (System.Data.DataRow item in schemaTable.Rows)
            {
                schemaInfo.Add(new SchemaInfo { Key = item.ItemArray[0].ToString(), Parent = null, ImageSource = this.FolderImage });
            }
            return schemaInfo;
        }

        void GetTableInfo(string key, DataTable tableInfo, SchemaInfo schemaInfo, NodeType nodeType)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == key)
                    {
                        //Perform population of tables
                        if (item.SchemaInfos == null)
                        {
                            item.SchemaInfos = new List<SchemaInfo>();
                        }
                        foreach (System.Data.DataRow tableRow in tableInfo.Rows)
                        {
                            if (item.Parent.Key == tableRow.ItemArray[0].ToString())
                            {
                                item.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[1].ToString(), Parent = item, TreeNodeType = nodeType, ImageSource = this.FolderImage });
                            }
                        }
                        return;
                    }
                    else
                    {
                        //pass it to the child nodes update function and retrieve the childs
                        GetTableInfo(key, tableInfo, UpdateChildNodes(key, item), nodeType);
                        //List<SchemaInfo> schemaTableInfo = UpdateChildNodes("Table", item.SchemaInfos);
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }

        void GetTableColumnInfo(string schemaKey, string keyType, DataTable tableColumnInfo, SchemaInfo schemaInfo, NodeType nodeType)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == keyType)
                    {
                        foreach (var objectItem in item.SchemaInfos)
                        {
                            foreach (System.Data.DataRow tableRow in tableColumnInfo.Rows)
                            {
                                if (item.Parent.Key == tableRow.ItemArray[0].ToString())
                                {
                                    if (objectItem.Key == tableRow.ItemArray[1].ToString())
                                    {
                                        objectItem.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[2].ToString(), Parent = objectItem, TreeNodeType = nodeType, DataType = tableRow.ItemArray[3].ToString(), ImageSource = this.FolderImage });
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }

        void UpdateSchemaDefaultChild(List<SchemaInfo> listOfSchema)
        {
            foreach (var item in listOfSchema)
            {
                item.SchemaInfos.Add(new SchemaInfo { Key = "Table", Parent = item, ImageSource = this.FolderImage });
                item.SchemaInfos.Add(new SchemaInfo { Key = "Views", Parent = item, ImageSource = this.FolderImage });
                //item.SchemaInfos.Add(new SchemaInfo { Key = "Stored Procedures", Parent = item });
                //item.SchemaInfos.Add(new SchemaInfo { Key = "Table Value Functions", Parent = item });
            }
        }

        SchemaInfo UpdateChildNodes(string key, SchemaInfo schemaInfo)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == key)
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        private void RemoveEmptyChildNodes(List<SchemaInfo> schemaInfos)
        {
            for (int i = (schemaInfos.Count - 1); i >= 0; i--)
            {
                SchemaInfo item = schemaInfos[i];
                for (int j = (item.SchemaInfos.Count - 1); j >= 0; j--)
                {
                    SchemaInfo objectItem = item.SchemaInfos[j];
                    if (objectItem.SchemaInfos.Count <= 0 && objectItem.TreeNodeType == NodeType.Folder)
                    {
                        item.SchemaInfos.Remove(objectItem);
                    }
                }
            }
        }

        //This method returns all the key collection containing the name of tables
        private List<SchemaInfo> GetSelectedNode(List<SchemaInfo> schemaInfos)
        {
            List<SchemaInfo> tempSchemaInfo = new List<SchemaInfo>();

            if (this.DataSource.ConnectionProperties.DataProvider == DataProviders.SQLServerCe)
            {
                foreach (var tableItem in schemaInfos)
                {
                    foreach (var tableitems in tableItem.SchemaInfos)
                    {
                        if (tableitems.IsSelected && (tableitems.TreeNodeType == NodeType.Table || tableitems.TreeNodeType == NodeType.View))
                        {
                            tempSchemaInfo.Add(tableitems);
                        }
                    }

                }
            }

            else
            {
                foreach (var item in schemaInfos)
                {
                    foreach (var tableItem in item.SchemaInfos)
                    {
                        foreach (var tableitems in tableItem.SchemaInfos)
                        {
                            if (tableitems.IsSelected && (tableitems.TreeNodeType == NodeType.Table || tableitems.TreeNodeType == NodeType.View))
                            {
                                tempSchemaInfo.Add(tableitems);
                            }
                        }
                    }
                }
            }
            return tempSchemaInfo;
        } 

        #endregion

        #region EventHandler Methods

        private void rtbox_TextView_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.rtbox_TextView.Text != string.Empty)
            {
                //textChanged = true;
            }
            else
            {
                //textChanged = false;
            }
        }

        private void btn_HelponQueryDesigner_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report%20Designer/WPF");
        }

        #endregion

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.trvw_Schemas.Focus();
        }

        private void ChromelessWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
