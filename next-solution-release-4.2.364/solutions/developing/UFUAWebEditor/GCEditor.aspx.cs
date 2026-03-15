using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Xpo;
using DevExpress.Web.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Configuration;
using DriverSettingsInterfaces;
using Utilities;

namespace UFUAWebEditor
{
    public partial class GCEditor : System.Web.UI.Page
    {
        readonly Session session = XpoHelper.GetNewSession();
        KeyValueConfigurationCollection settings;

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

        protected void Page_Init(object sender, EventArgs e)
        {
            XpoDataSource1.Session = XpoHelper.GetNewSession();
            var coll = new XPCollection<UFUAModel.UFUAConfiguration>(XpoDataSource1.Session);
            if (coll.Count == 0)
            {
                var newConf = new UFUAModel.UFUAConfiguration(XpoDataSource1.Session);
                newConf.Save();
            }

            Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            settings = rootWebConfig.AppSettings.Settings;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxGridViewDrivers.DataBind();
        }

        protected void ASPxGridViewDrivers_DataBinding(object sender, EventArgs e)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(session)/*.AsParallel()*/
                                              select tag.ComunicationDrivers).ToList();
            if (list.Count > 0)
                ASPxGridViewDrivers.DataSource = list[0];
            else
                ASPxGridViewDrivers.DataSource = null;
        }

        protected void ASPxGridViewDrivers_RowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data) 
                return;

            var path = (string)e.GetValue("Path");
            var fullpath = ASPxGridViewDrivers.FindRowCellTemplateControl(e.VisibleIndex, null, "txtFullPath") as ASPxLabel;
            if (!String.IsNullOrEmpty(path))
                fullpath.Text = String.Format("{0}\\{1}.dll", Path.GetFullPath(path), e.KeyValue);

            var version = ASPxGridViewDrivers.FindRowCellTemplateControl(e.VisibleIndex, null, "txtVersion") as ASPxLabel;

            if (!String.IsNullOrEmpty(fullpath.Text))
            {
                try
                {
                    var ver = Assembly.LoadFile(fullpath.Text).GetName().Version;
                    version.Text = String.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Revision);
                }
                catch (FileNotFoundException)
                {
                    version.ForeColor = System.Drawing.Color.Red;
                    version.Text = "Unable to load the dll (wrong path) !";
                }
            }
            else
            {
                version.ForeColor = System.Drawing.Color.Red;
                version.Text = "Unable to load the dll (missing path) !";
            }
        }

        protected void ASPxGridViewDrivers_StartRowEditing(object sender, ASPxStartRowEditingEventArgs e)
        {
            string key = e.EditingKeyValue.ToString();
            //TODO: Open the specific driver configuration window
            e.Cancel = true;
        }

        protected void ASPxCallback1_Callback(object sender, CallbackEventArgs e)
        {
            var drivers = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(session)/*.AsParallel()*/
                           select tag).ToList();

            if (drivers.Count == 0)
            {
                var newConf = new UFUAModel.UFUAConfiguration(session);
                drivers.Add(newConf);
            }

            Control drvlist = ASPxPopupDriversList.FindControl("DriversList1");
            ASPxGridView grid = drvlist.FindControl("ASPxGridView1") as ASPxGridView;
            foreach (var key in grid.GetCurrentPageRowValues(grid.KeyFieldName))
            {
                var dll = key as string;
                if (dll == null)
                    continue;

                var drv = (from t in drivers[0].ComunicationDrivers where t.Name == Path.GetFileNameWithoutExtension(dll) select t).ToList();
                if (grid.Selection.IsRowSelectedByKey(key) && drv.Count == 0)
                {
                    var friendlyname = grid.GetRowValuesByKeyValue(key, "FriendlyName").ToString();
                    var path = grid.GetRowValuesByKeyValue(key, "Path").ToString();
                    if (String.IsNullOrEmpty(path) && settings["driverspath"] != null)
                        path = settings["driverspath"].Value;
                    
                    drivers[0].ComunicationDrivers.Add(new UFUAModel.UFUACommunicationDriver(session)
                    {
                        Name = Path.GetFileNameWithoutExtension(dll),
                        FriendlyName = friendlyname,
                        Path = path
                    });
                }
                else if (!grid.Selection.IsRowSelectedByKey(key) && drv.Count > 0)
                    drivers[0].ComunicationDrivers.Remove(drv[0]);
            }

            drivers[0].Save();
        }
    }
}