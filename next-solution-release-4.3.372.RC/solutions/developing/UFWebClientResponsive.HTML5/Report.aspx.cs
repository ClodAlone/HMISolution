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
using UFWebClientResponsive_HTML5;
using DataGridElementSettings;
using System.Configuration;
using DocumentManager.ComponentService;
using DevExpress.XtraReports.Parameters;

namespace UFWebClient.HTML5
{
    public partial class Report : System.Web.UI.Page
    {
        const string DirImageUrl = "~/Images/Folder.png";
        const string RptImageUrl = "~/Images/Report.png";
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        public static DataGridRetrieverSection _Config =
        ConfigurationManager.GetSection("ReportRetriever") as DataGridRetrieverSection;

        ReportServiceHelper helper;
        static StaticListLookUpSettings alarmSources;

        protected void Page_Init(object sender, EventArgs e)
        {
            //ASPxWebControl.GlobalEmbedRequiredClientLibraries = true;
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
                if (Global.projectDocument == null || Global.ReportComponent == null)
                    return;

                var list = Global.projectDocument.GetScreenControllers(Global.ReportComponent);
                if (list != null && list.Count > 0)
                    AddChildNodes(list);
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx/IoqCaj13T3tqZw/KR6gYQ=="/* STA */);
                if (state != false)
#endif
                {
                    var appDataPath = String.Format("{0}\\", Server.MapPath("App_Data"));
                    var reports = ReportAlarms.ReportAlarms.GetWholeAlarmReportDocumentList(Global.projectDocument, appDataPath);
                    if (reports != null && reports.Count > 0)
                    {
                        var root = new TreeViewNode("Statistics", "Statistics", DirImageUrl);
                        reports.ForEach(uri =>
                        {
                            var name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                            if (HasUserAcess(name))
                            {
                                var child = new TreeViewNode(name, uri.GetPathString(), RptImageUrl);
                                root.Nodes.Add(child);
                            }
                        });

                        if (root.Nodes.Count > 0)
                            ASPxTreeView.Nodes.Add(root);
                    }

                    if (Global.UFUAEditorComponent != null)
                    {
                        var sources = Global.UFUAEditorComponent.GetFlatListAlarmSources(Global.projectDocument);
                        if (sources != null)
                        {
                            var listSources = sources.OrderBy(x => x).ToList();
                            if (listSources.Count > 0)
                            {
                                alarmSources = new StaticListLookUpSettings();
                                alarmSources.LookUpValues.Add(new LookUpValue(String.Empty, String.Empty));
                                foreach (var item in sources)
                                    alarmSources.LookUpValues.Add(new LookUpValue(item, String.Empty));
                            }
                        }
                    }
                }
            }
        }

        void AddChildNodes(List<IScreenController> list, TreeViewNode parent = null)
        {
            list.ForEach(item =>
            {
                var root = new TreeViewNode(item.GetTitle(), item.GetTitle(), DirImageUrl);
                item.GetScreenLists(Global.ReportComponent).ForEach(uri =>
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                    if (HasUserAcess(name))
                    {
                        var node = new TreeViewNode(name, uri.GetPathString(), RptImageUrl);
                        root.Nodes.Add(node);
                    }
                });

                var childs = item.GetScreenControllers(Global.ReportComponent);
                if (childs != null && childs.Count > 0)
                    AddChildNodes(childs, root);

                if (root.Nodes.Count > 0)
                {
                    if (parent == null)
                        ASPxTreeView.Nodes.Add(root);
                    else
                        parent.Nodes.Add(root);
                }
            });
        }

        bool HasUserAcess(string name)
        {
            var configElement = (from DataGridElement element in _Config.DataGrids where element.Name == name select element).FirstOrDefault();

            var user = System.Web.HttpContext.Current.User;
            if (user != null && user.Identity != null && !String.IsNullOrEmpty(user.Identity.Name))
            {
                if (configElement != null)
                {
                    var users = configElement.Users.ToLower().Split(';');
                    if (!String.IsNullOrEmpty(configElement.Users) && users.Length > 0 && !users.Contains(user.Identity.Name.ToLower()))
                        return false;
                    else
                    {
                        var roles = configElement.Roles.Split(';');
                        if (!String.IsNullOrEmpty(configElement.Roles) && roles.Length > 0)
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
                                return false;
                        }
                    }
                }
            }
            else if (configElement != null)
                return false;

            return true;
        }

        protected void ASPxTreeView_NodeClick(object source, TreeViewNodeEventArgs e)
        {
            var report = CreateReport(e.Node);
            if (report != null)
            {
                if (alarmSources != null && ReportAlarms.ReportAlarms.IsAlarmReportFile(e.Node.Name))
                {
                    if (report.Parameters != null)
                    {
                        foreach (var par in report.Parameters)
                        {
                            if (par.Name == ReportAlarms.ParameterNames.Source)
                            {
                                par.ValueSourceSettings = alarmSources;
                                par.Visible = true;
                                break;
                            }
                        }
                    }
                }

                ASPxDocumentViewer1.OpenReport(report);
                ASPxDocumentViewer1.Visible = String.IsNullOrEmpty(helper.LastErrorInfo);
                LabelErrorInfo.Visible = !String.IsNullOrEmpty(helper.LastErrorInfo);
                LabelErrorInfo.Text = helper.LastErrorInfo;
            }
        }

        XtraReport CreateReport(TreeViewNode selectedNode)
        {
            if (selectedNode == null || Global.ReportComponent == null || Global.projectDocument == null)
                return null;

            var uri = selectedNode.Name;
                if (uri == null)
                return null;

            ReportSettings.Documents.ReportDocument report = null;
            var parameters = new List<ReportParameters.Parameter>();
            if (ReportAlarms.ReportAlarms.IsAlarmReportFile(uri))
            {
                report = ReportSettings.Documents.ReportDocument.FromFile(uri, Global.projectDocument, true);
                if (report == null)
                    return null;

                report.Parent = Global.projectDocument;
                helper = new ReportServiceHelper(report, Global.UFUAEditorComponent, DefaultSourceType.EventLog);
                helper.MaxTake = Global.MaxAlarmsStatisticData;
            }
            else
            {
                report = ReportSettings.Documents.ReportDocument.FromFile(uri,
                    Global.projectDocument);
                if (report == null)
                    return null;

                report.Parent = Global.projectDocument;
                helper = new ReportServiceHelper(report, Global.UFUAEditorComponent);
                parameters.Add(new ReportParameters.Parameter()
                {
                    Name = helper.ReportUserNameParameter,
                    Type = ReportParameters.ParameterType.String,
                    Value = Context.User.Identity.Name
                });
            }

            return helper.PrepareXtraReportDocument(parameters);
        }
    }
}