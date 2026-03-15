using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using UFWebClient.HTML5;

namespace UFWebClientResponsive_HTML5 {
    public partial class RootMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var startup = Global.projectDocument.GetStartType();
            if (!Global.ShowHeader/* || startup == DocumentManager.ComponentService.StartType.GeoPage*/)
                HeaderPane.Visible = false;
            else
            {
                if (Global.ShowProjectTitle)
                {
                    if (String.IsNullOrEmpty(Global.ProjectTitle))
                        TitleLink.InnerText = Global.projectDocument.Title;
                    else
                        TitleLink.InnerText = Global.ProjectTitle;
                }
                else
                    TitleLink.InnerText = String.Empty;

                if (Global.HideUserRegister)
                {
                    var control = HeadLoginView.FindControl("registerLink");
                    if (control != null)
                        control.Visible = false;
                }
            }

            if (Global.HideDashboardMenuItem)
                (from c in HeaderMenu.Items where c.NavigateUrl.Contains("DashBoard.aspx") select c).First().Visible = false;
            if (Global.HideAlarmMenuItem)
                (from c in HeaderMenu.Items where c.NavigateUrl.Contains("Alarm.aspx") select c).First().Visible = false;
            if (Global.HideDataAnalisysMenuItem)
                (from c in HeaderMenu.Items where c.NavigateUrl.Contains("DataAnalisys.aspx") select c).First().Visible = false;
            if (Global.HideDataGridMenuItem)
                (from c in HeaderMenu.Items where c.NavigateUrl.Contains("DataGrid.aspx") select c).First().Visible = false;
            if (Global.HideReportMenuItem)
                (from c in HeaderMenu.Items where c.NavigateUrl.Contains("Report.aspx") select c).First().Visible = false;
        }
    }
}