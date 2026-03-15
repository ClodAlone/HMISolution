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

namespace UFWebClient.HTML5
{
    public partial class Report : System.Web.UI.Page
    {
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        ReportServiceHelper helper;

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
                if (Global.projectDocument == null || Global.ReportComponent == null)
                    return;

                var list = Global.projectDocument.GetWholeDocumentLists(Global.ReportComponent);
#if !DEBUG                            
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx/IoqCaj13T3tqZw/KR6gYQ=="/* STA */);
                if (state != false)
#endif
                list.AddRange(ReportAlarms.ReportAlarms.GetWholeAlarmReportDocumentList(Global.projectDocument));
                list.ForEach(uri =>
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                    var item = new ListEditItem(name, uri.GetPathString());
                    ASPxListBox1.Items.Insert(0, item);
                });
            }

            var report = CreateReport();
            if (report != null)
            {
                ASPxDocumentViewer1.OpenReport(report);
                ASPxDocumentViewer1.Visible = String.IsNullOrEmpty(helper.LastErrorInfo);
                LabelErrorInfo.Visible = !String.IsNullOrEmpty(helper.LastErrorInfo);
                LabelErrorInfo.Text = helper.LastErrorInfo;
            }
        }

        XtraReport CreateReport()
        {
            if (ASPxListBox1.SelectedItem == null || Global.ReportComponent == null || Global.projectDocument == null)
                return null;

            var uri = ASPxListBox1.SelectedItem.Value as String;
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
            }
            else
            {
                report = ReportSettings.Documents.ReportDocument.FromFile(uri, Global.projectDocument);
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