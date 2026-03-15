using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UFUAWebEditor
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Theme
        /* Page PreInit */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string themeName = "Glass";
            if (Page.Request.Cookies[SiteMaster.GetThemeCookieName()] != null)
            {
                themeName = Page.Request.Cookies[SiteMaster.GetThemeCookieName()].Value;
            }

            string clientScriptBlock = "var DXCurrentThemeCookieName = \"" + SiteMaster.GetThemeCookieName() + "\";";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DXCurrentThemeCookieName", clientScriptBlock, true);

            this.Theme = themeName;
        }
        #endregion
    }
}
