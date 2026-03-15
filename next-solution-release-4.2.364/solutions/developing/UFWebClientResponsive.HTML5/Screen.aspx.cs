using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFWebClientResponsive_HTML5;
using Utilities;

namespace UFWebClient.HTML5
{
    public partial class _Default : System.Web.UI.Page
    {
        const string DirImageUrl = "~/Images/Folder.png";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Global.ShowScreenNavigator)
            {
                var startup = Global.projectDocument.GetStartType();
                if (startup == DocumentManager.ComponentService.StartType.TilePage)
                {
                    var tileInfos = Global.listTiles;
                    if (tileInfos.Count > 0)
                    {
                        /*
                        foreach (var pair in Global.mapTiles)
                        {
                            var group = new NavBarGroup();
                            group.Text = pair.Key;
                            nbMain.Groups.Add(group);
                            foreach (var tile in pair.Value)
                            {
                                var item = new NavBarItem();
                                item.Text = tile.Name;
                                // item.NavigateUrl = tile.Url;
                                item.Name = tile.Url;
                                group.Items.Add(item);
                            }
                        }
                        */
                        foreach (var tile in tileInfos)
                        {
                            if (!HasUserAcess(tile))
                                continue;

                            var node = new TreeViewNode(tile.Section, "", DirImageUrl);
                            ASPxTreeView.Nodes.Add(node);

                            AddChildNodes(node, tile.childs);
                        }
                    }
                    else
                        LeftPanel.Visible = false;
                }
                else
                    LeftPanel.Visible = false;
            }
            else
                LeftPanel.Visible = false;
        }

        bool HasUserAcess(TileInfo tileinfo)
        {
            var user = Context.User;
            if (user != null && user.Identity != null && !String.IsNullOrEmpty(user.Identity.Name))
            {
                if (!String.IsNullOrEmpty(tileinfo.UsersVisibility))
                {
                    var users = tileinfo.UsersVisibility.Split(';');
                    if (users.Length > 0 && !users.Contains(user.Identity.Name))
                        return false;
                }
                if (!String.IsNullOrEmpty(tileinfo.RolesVisibility))
                {
                    var roles = tileinfo.RolesVisibility.Split(';');
                    if (roles.Length > 0)
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
                            return false;
                    }
                }
            }

            return true;
        }

        void AddChildNodes(TreeViewNode parent, List<TileInfo> list)
        {
            if (list == null)
                return;

            foreach (var tile in list)
            {
                if (!HasUserAcess(tile))
                    continue;

                if (String.IsNullOrEmpty(tile.Url))
                {
                    if (tile.childs != null && tile.childs.Count > 0)
                    {
                        var node = new TreeViewNode(tile.Section, "", DirImageUrl);
                        if (node.Image != null)
                            node.Image.Width = node.Image.Height = 32;
                        parent.Nodes.Add(node);
                        AddChildNodes(node, tile.childs);
                    }
                }
                else
                {
                    var nodeChild = new TreeViewNode(tile.Name, 
                                            tile.Url, 
                                            tile.ImageUrl);
                    if (nodeChild.Image != null)
                        nodeChild.Image.Width = nodeChild.Image.Height = 32;
                    parent.Nodes.Add(nodeChild);
                    AddChildNodes(nodeChild, tile.childs);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ASPxTreeView.ExpandToDepth(0);

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
                    var found = (from tileinfo in Global.listTilesFlat where tileinfo.Url == uri.GetPathString() select tileinfo).ToList();
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
                    }
                    else
                    {
                        bRedirect = true;
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
