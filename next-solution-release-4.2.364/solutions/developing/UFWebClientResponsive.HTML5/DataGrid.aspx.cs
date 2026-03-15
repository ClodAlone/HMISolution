using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.XtraReports.UI;
using ReportManager.ReportService;
using Utilities;
using log4net;
using System.Data;
using DataReader.Helpers;
using DataGridElementSettings;
using System.Configuration;
using UFWebClient.HTML5.UserControls;
using DevExpress.XtraPrinting;
using DevExpress.Export;

namespace UFWebClient.HTML5
{
    public partial class DataGrid : System.Web.UI.Page
    {
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        public static DataGridRetrieverSection _Config =
                ConfigurationManager.GetSection("DataGridRetriever") as DataGridRetrieverSection;

        protected void Page_Init(object sender, EventArgs e)
        {
            //ASPxWebControl.GlobalEmbedRequiredClientLibraries = true;
        }

        protected void GridData_Load(object sender, EventArgs e)
        {
            // set DataSource
            GridData.DataSource = GetCurrentTable();
            // create columns, if necessary
            EnsureColumns();
            // call DataBind()
            GridData.DataBind();
        }

        private void ShowError(String Error = null)
        {
            LabelErrorInfo.Visible = !String.IsNullOrEmpty(Error);
            LabelErrorInfo.Text = Error;
        }

        private DataTable GetCurrentTable()
        {
            ShowError();

            var dataSource = GetSelectedDataSourceName();
            if (dataSource == null)
            {
                ShowError(Properties.Resources.NoDataSourceSelected);
                return null;
            }

            return GetCurrentTable(dataSource);
        }

        private DataTable GetCurrentTable(DataGridElement dataSource)
        {
            //var dt = Session[dataSource.Name] as DataTable;
            //if (dt != null)
            //    return dt;

            string defaultDataProvider = null;
            string defaultConnectionString = null;
            if (XpoHelpers.XpoHelper.IsDataSource(dataSource.ConnectionString))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(dataSource.ConnectionString);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(dataSource.ConnectionString);
            }
            else
            {
                var helper = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(dataSource.ConnectionString);
                defaultDataProvider = helper.GetPartByName("DataProvider");
                helper.RemovePartByName("DataProvider");
                defaultConnectionString = helper.GetConnectionString();
            }

            using (var connection = DataReader.DataReader.CreateDbConnection(defaultDataProvider, defaultConnectionString))
            {
                try
                {
                    connection.Open();

                    var ret = new DataSet();
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(defaultDataProvider);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(defaultDataProvider);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = dataSource.Query;

                    dbdapater.Fill(ret);

                    DataTable dataTable = null;
                    if (ret.Tables.Count > 0)
                        dataTable = ret.Tables[0];
                    //Session.Add(dataSource.Name, dataTable);

                    return dataTable;
                }
                catch (Exception ex)
                {
                    var error = string.Format("{0}: {1}", Properties.Resources.ErrorAccessingDataSource, ex.Message);
                    ShowError(error);
                }
                finally
                {
                    connection.Close();
                }
            }

            return null;
        }

        private void EnsureColumns()
        {
            ReCreateColumns();
            /*
            if (GridData.Columns.Count == 0)
                ReCreateColumns();
            else
            {
                DataTable table = GetCurrentTable();
                // if the grid has other columns than the assigned data source, 
                // recreate columns
                if (GridData.Columns[table.Columns[0].ColumnName] == null)
                    ReCreateColumns();
            }
            */
        }

        private void ReCreateColumns()
        {
            GridData.Columns.Clear();

            DataTable table = GetCurrentTable();
            if (table == null)
                return;

            //var selectionColumn = new GridViewCommandColumn() { ShowSelectCheckbox = true };
            //GridData.Columns.Add(selectionColumn);

            bool bFirstLoop = true;
            foreach (DataColumn dataColumn in table.Columns)
            {
                GridViewDataTextColumn column = new GridViewDataTextColumn();
                if (bFirstLoop)
                {
                    GridData.KeyFieldName = dataColumn.ColumnName;
                    bFirstLoop = false;
                }
                column.FieldName = dataColumn.ColumnName;
                // set additional column properties
                column.Caption = dataColumn.ColumnName;
                GridData.Columns.Add(column);
            }
        }

        DataGridElement GetSelectedDataSourceName()
        {
            if (ASPxListBox1.SelectedItem == null)
                return null;

            foreach (DataGridElement element in _Config.DataGrids)
            {
                if (element.Name == ASPxListBox1.SelectedItem.Text)
                    return element;
            }
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if !DEBUG                            
            MSZ.MSZView.CheckState(true);
            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w=="/* REP */);
            if (mode == false)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "licence", String.Format("<script>alert('{0}');</script>", Properties.Resources.NoReportLicense));
                logLicense.Warn(Properties.Resources.NoReportLicense);
                return;
            }
#endif

            if (!IsPostBack)
            {
                var user = System.Web.HttpContext.Current.User;

                foreach (DataGridElement element in _Config.DataGrids)
                {
                    if (user != null && user.Identity != null && !String.IsNullOrEmpty(user.Identity.Name))
                    {
                        var users = element.Users.Split(';');
                        if (!String.IsNullOrEmpty(element.Users) && users.Length > 0 && !users.Contains(user.Identity.Name))
                            continue;
                        var roles = element.Roles.Split(';');
                        if (!String.IsNullOrEmpty(element.Roles) && roles.Length > 0)
                        {
                            bool bFound = false;
                            foreach(var role in roles)
                            {
                                if (user.IsInRole(role))
                                {
                                    bFound = true;
                                    break;
                                }
                            }
                            if (!bFound)
                                continue;
                        }
                    }
                    var item = new ListEditItem(element.Name);
                    ASPxListBox1.Items.Insert(0, item);
                }
            }
        }

        protected void ToolbarExport_ItemClick(object source, ExportItemClickEventArgs e)
        {
            switch (e.ExportType)
            {
                case DemoExportFormat.Pdf:
                    gridExport.WritePdfToResponse();
                    break;
                case DemoExportFormat.Xls:
                    gridExport.WriteXlsToResponse(new XlsExportOptionsEx { ExportType = ExportType.WYSIWYG });
                    break;
                case DemoExportFormat.Xlsx:
                    gridExport.WriteXlsxToResponse(new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
                    break;
                case DemoExportFormat.Rtf:
                    gridExport.WriteRtfToResponse();
                    break;
                case DemoExportFormat.Csv:
                    gridExport.WriteCsvToResponse(new CsvExportOptionsEx() { ExportType = ExportType.WYSIWYG });
                    break;
            }
        }
    }
}