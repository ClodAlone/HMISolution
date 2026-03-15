using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace UFWebClient.HTML5
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = Request.QueryString["url"];
            if (!String.IsNullOrEmpty(url))
            {
                var user = System.Web.HttpContext.Current.User;
                if (user != null && user.Identity != null && !String.IsNullOrEmpty(user.Identity.Name))
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri)
                        uri = Global.projectDocument.MakeRelativeUri(uri, Global.ScreenComponent);
                    bool bRedirect = false;
                    foreach (var pair in Global.mapTiles)
                    {
                        var found = (from tileinfo in pair.Value where tileinfo.Url == uri.GetPathString() select tileinfo).ToList();
                        if (found.Count > 0)
                        {
                            bRedirect = false;
                            var tileinfo = found[0];
                            if (!String.IsNullOrEmpty(tileinfo.UsersVisibility))
                            {
                                var users = tileinfo.UsersVisibility.Split(';');
                                if (!String.IsNullOrEmpty(tileinfo.UsersVisibility) && users.Length > 0 && !users.Contains(user.Identity.Name))
                                    bRedirect = true;
                            }
                            if (!String.IsNullOrEmpty(tileinfo.RolesVisibility))
                            {
                                var roles = tileinfo.RolesVisibility.Split(';');
                                if (!String.IsNullOrEmpty(tileinfo.RolesVisibility) && roles.Length > 0)
                                {
                                    bool bFound = false;
                                    foreach (var role in roles)
                                    {
                                        if (user.IsInRole(role))
                                        {
                                            bFound = true;
                                            break;
                                        }
                                    }
                                    if (!bFound)
                                        bRedirect = true;
                                }
                            }
                            break;
                        }
                        else
                        {
                            bRedirect = true;
                        }
                    }

                    //if (Global.projectDocument.IsVisible(uri))
                    //    Global.currentPage = uri;
                    if (bRedirect)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }
    }
}
