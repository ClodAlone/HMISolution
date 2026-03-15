using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using DevExpress.Web;

namespace UFUAWebEditor
{
    public partial class DCEditor : System.Web.UI.Page
    {
        #region Theme
        /* Page PreInit */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string themeName = "Glass";
            if (Page.Request.Cookies[SiteMaster.GetThemeCookieName()] != null)
            {
                themeName = Page.Request.Cookies[SiteMaster.GetThemeCookieName()].Value;
            }

            string clientScriptBlock = String.Format("var DXCurrentThemeCookieName = \"{0}\";", SiteMaster.GetThemeCookieName());
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DXCurrentThemeCookieName", clientScriptBlock, true);

            this.Theme = themeName;
        }
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            XpoDataSource1.Session = XpoHelper.GetNewSession();
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}