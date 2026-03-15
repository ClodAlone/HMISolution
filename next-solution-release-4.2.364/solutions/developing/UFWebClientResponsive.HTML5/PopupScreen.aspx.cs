using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UFWebClient.HTML5
{
    public partial class PopupScreen : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Global.ShowPopupCloseBtn)
                ASPxButton4.Visible = false;
        }
    }
}
