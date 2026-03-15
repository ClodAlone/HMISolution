using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UFWebClient.HTML5
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Global.projectDocument == null)
                return;

            ASPxLabel1.Text = Global.projectDocument.Title;
        }
    }
}
