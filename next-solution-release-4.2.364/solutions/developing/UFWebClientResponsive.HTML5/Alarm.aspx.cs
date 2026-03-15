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

        protected string GetTimeText(GridViewDataItemTemplateContainer container, string fieldname)
        {
            var model = grid.GetRow(container.VisibleIndex) as ConditionStateViewModel;
            if (model == null)
                return null;
            DateTime? date = null;
            switch (fieldname)
            {
                case "Time":
                    date = model.Time;
                    break;
                case "ActiveTransitionTime":
                    date = model.ActiveTransitionTime;
                    break;
                case "AckedTransitionTime":
                    date = model.AckedTransitionTime;
                    break;
                case "ConfirmedTransitionTime":
                    date = model.ConfirmedTransitionTime;
                    break;
                case "ShelvingTransitionTime":
                    date = model.ShelvingTransitionTime;
                    break;
            }
            int clientoffset = AlarmProvider.GetClientTimeOffset();
            if (date != null)
                return clientoffset != 0 ? ((DateTime)date).AddMinutes(-clientoffset).ToString() : ((DateTime)date).ToString();

            return null;
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}