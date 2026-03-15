using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;

namespace UFUAWebEditor
{
    public partial class LTEditor : System.Web.UI.Page
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

            string clientScriptBlock = "var DXCurrentThemeCookieName = \"" + SiteMaster.GetThemeCookieName() + "\";";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DXCurrentThemeCookieName", clientScriptBlock, true);

            this.Theme = themeName;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            XpoDataSource1.Session = XpoHelper.GetNewSession();
            XpoDataSource2.Session = XpoHelper.GetNewSession();
        }

        protected void ASPxGridView2_BeforePerformDataSelect(object sender, EventArgs e)
        {
            Session["UFUALocaleKey"] = ((ASPxGridView)sender).GetMasterRowKeyValue(); ;
        }

        protected void ASPxGridView2_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            // Do not use this event to init values of a new row!
            // It can be only used to initialize visible editors on the EditForm.
            // To assign data row values, use the RowInserting event
        }

        protected void ASPxGridView2_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            e.NewValues["UFUALocale!Key"] = ((ASPxGridView)sender).GetMasterRowKeyValue();
        }
    }
}