using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using OPCUAViewModel;

namespace UFWebClient.HTML5
{
    public partial class Alarm : System.Web.UI.Page
    {
        protected string GetIconStateImageUrl(GridViewDataItemTemplateContainer container)
        {
            var model = grid.GetRow(container.VisibleIndex) as ConditionStateViewModel;
            if (model == null)
                return null;
            return String.Format("Images/{0}.png", model.NeedsAcknoledge == true ? "Enable" : "Disable");
        }

        protected string GetStateText(GridViewDataItemTemplateContainer container)
        {
            var model = grid.GetRow(container.VisibleIndex) as ConditionStateViewModel;
            if (model == null)
                return null;
            return model.EnabledState;
        }

        protected string GetTimeText(GridViewDataItemTemplateContainer container)
        {
            var model = grid.GetRow(container.VisibleIndex) as ConditionStateViewModel;
            if (model == null || model.Time == null)
                return null;
            return ((DateTime)model.Time).ToLocalTime().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void gridAlarm_HtmlDataCellPrepared(object sender,DevExpress.Web.ASPxGridViewTableDataCellEventArgs e)
        {
            if (e.DataColumn.FieldName == "Time" && !string.IsNullOrEmpty(e.Cell.Text)) return;
                e.Cell.Text = (Convert.ToDateTime(e.Cell.Text).ToLocalTime()).ToString();
        }
    }
}