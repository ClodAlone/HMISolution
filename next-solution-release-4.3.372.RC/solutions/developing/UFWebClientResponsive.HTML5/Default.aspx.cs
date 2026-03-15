using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFWebClient.HTML5;

namespace UFWebClientResponsive_HTML5 {
    public partial class _Default : System.Web.UI.Page {

        const string DirImageUrl = "~/Images/Folder.png";

        protected void Page_Init(object sender, EventArgs e)
        {
            var startup = Global.projectDocument.GetStartType();
            if (startup == DocumentManager.ComponentService.StartType.TilePage)
            {
                /*
                foreach (var pair in Global.mapTiles)
                {
                    foreach (var tile in pair.Value)
                    {
                        var panel = new ASPxDockPanel();
                        panel.ID = tile.Url;
                        panel.Visible = true;
                        panel.HeaderText = tile.Name;
                        panel.BackColor = tile.ColorValue;
                        panel.Width = 150;
                        panel.Height = 150;
                        panel.ShowHeader = true;
                        panel.ShowCloseButton = false;
                        panel.HeaderText = tile.Name;
                        panel.AllowResize = true;
                        if (tile.IsExtraSmall)
                        {
                            panel.Width = 75;
                            panel.Height = 75;
                        }
                        else if (tile.IsExtraLarge)
                        {
                            panel.Width = 400;
                            panel.Height = 400;
                        }
                        else if (tile.IsLarge)
                        {
                            panel.Width = 300;
                            panel.Height = 300;
                        }
                        panel.OwnerZone = zone1;

                        var btn = new ASPxButton();
                        btn.ImageUrl = tile.ImageUrl;
                        btn.PostBackUrl = String.Format("~/Screen.aspx?url={0}", tile.Url);
                        btn.Height = panel.Height;
                        btn.Width = panel.Width;
                        btn.BackColor = panel.BackColor;
                        btn.EnableTheming = false;
                        panel.Controls.Add(btn);
                        holder.Controls.Add(panel);
                    }
                }
                foreach (var pair in Global.mapTiles)
                {
                    var group = new NavBarGroup();
                    group.Text = pair.Key;
                    nbMain.Groups.Add(group);

                    foreach (var tile in pair.Value)
                    {
                        var item = new NavBarItem(tile.Name, tile.Name, tile.ImageUrl,
                            String.Format("~/Screen.aspx?url={0}", tile.Url));
                        group.Items.Add(item);
                    }
                }
                */

                foreach (var tile in Global.listTiles)
                {
                    if (!HasUserAcess(tile))
                        continue;

                    var node = new TreeViewNode(tile.Section, tile.Section, DirImageUrl);
                    ASPxTreeView.Nodes.Add(node);

                    AddChildNodes(node, tile.childs);
                }

                if (!String.IsNullOrEmpty(Global.LogoUrl))
                    ASPxImage1.ImageUrl = Global.LogoUrl;
            }
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
                        var node = new TreeViewNode(tile.Section, tile.Section, DirImageUrl);
                        if (node.Image != null)
                            node.Image.Width = node.Image.Height = 32;

                        parent.Nodes.Add(node);
                        AddChildNodes(node, tile.childs);
                    }
                }
                else
                { 
                    var nodeChild = new TreeViewNode(tile.Name, tile.Name, tile.ImageUrl,
                                String.Format("~/Screen.aspx?url={0}", tile.Url));
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
    }
}