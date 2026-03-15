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
using DevExpress.DashboardWeb.Designer;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DashboardWeb;

namespace UFWebClient.HTML5
{
    public partial class DashBoard : System.Web.UI.Page
    {
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        public static DataGridRetrieverSection _Config =
                ConfigurationManager.GetSection("DashBoardRetriever") as DataGridRetrieverSection;

        protected void Page_Init(object sender, EventArgs e)
        {
            ASPxWebControl.GlobalEmbedRequiredClientLibraries = true;
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

                var datasourceStorage = new DataSourceInMemoryStorage();

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
                            foreach (var role in roles)
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

                    try
                    {
                        var connectionString = new CustomStringConnectionParameters(element.ConnectionString);
                        var sqlDataSource = new DashboardSqlDataSource(element.Name, connectionString);
                        sqlDataSource.Name = element.Name;
                        var query = new CustomSqlQuery();
                        query.Name = element.Name;
                        query.Sql = element.Query;
                        sqlDataSource.Queries.Add(query);
                        sqlDataSource.Fill();
                        datasourceStorage.RegisterDataSource(sqlDataSource.Name, sqlDataSource.SaveToXml());
                    }
                    catch
                    {

                    }
                }

                DashboardConfigurator.Default.SetDataSourceStorage(datasourceStorage);

                ASPxDashboardDesigner1.AllowCreateNewDashboard = !Global.DisableCreateNewDashboard;
            }
        }

        public String SwitchToViewer()
        {
            return Global.SwitchToViewerDashboard.ToString().ToLower();
        }
    }
}