using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFWebClient.HTML5;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Global.projectDocument == null)
            return;

        var startup = Global.projectDocument.GetStartType();
        switch (startup)
        {
            case DocumentManager.ComponentService.StartType.MainScreen:
                Response.Redirect(String.Format("~/Screen.aspx?url={0}", 
                    Global.projectDocument.MakeRelativeUri(Global.currentPage, Global.ScreenComponent)));
                break;
            case DocumentManager.ComponentService.StartType.GeoPage:
                Response.Redirect("~/GeoPage.aspx");
                break;
        }
    }


    private bool IsCombinedJSOlder(string path)
    {
        var jsPath = Context.Server.MapPath(path);
        string[] files = Directory.GetFiles(jsPath);

        var combinedFileLastWrite = File.GetLastWriteTime(Server.MapPath("~/js/Combined.js"));

        return Array.Exists(files, file => (File.GetLastWriteTime(file) - combinedFileLastWrite).TotalSeconds > 1);
    }

    protected string GetAlerts()
    {
        if (!Request.IsLocal)
        {
            if (IsCombinedJSOlder("~/js/") || IsCombinedJSOlder("~/Tiles/"))
            {
                return "$('#CombinedScriptAlert').show();";
            }
            else
            {
                return string.Empty;
            }
        }
        else
        {
            return string.Empty;
        }
        
    }
}