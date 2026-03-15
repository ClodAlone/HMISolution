using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFWebClient.HTML5;

public partial class ServerStuff : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var startup = Global.projectDocument.GetStartType();
        if (startup != DocumentManager.ComponentService.StartType.TilePage &&
            startup != DocumentManager.ComponentService.StartType.GalleryPage &&
            startup != DocumentManager.ComponentService.StartType.GeoPage)
        {
            appnavbar.Visible = false;
            appnavbar_space.Visible = false;
        }
    }
}
