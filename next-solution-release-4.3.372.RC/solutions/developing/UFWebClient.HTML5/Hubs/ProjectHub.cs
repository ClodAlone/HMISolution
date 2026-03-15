using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace UFWebClient.HTML5.Hubs
{
    public class ProjectHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            Debug.WriteLine("New connection : " + Context.ConnectionId);
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Disconnection : " + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public override System.Threading.Tasks.Task OnReconnected()
        {
            Debug.WriteLine("Reconnection : " + Context.ConnectionId);
            return base.OnReconnected();
        }

        public IDictionary<String, IList<TileInfo>> GetListTileInfo()
        {
            try
            {
                Clients.Caller.Done();
            }
            catch 
            { }

            if (Global.projectDocument == null)
                return null;

            var user = Context.User;
            if (user != null && user.Identity != null && !String.IsNullOrEmpty(user.Identity.Name))
            {
                var mapTiles = new Dictionary<String, IList<TileInfo>>();

                foreach (var pair in Global.mapTiles)
                {
                    mapTiles.Add(pair.Key, new List<TileInfo>());
                    foreach (var tileinfo in pair.Value)
                    {
                        if (!String.IsNullOrEmpty(tileinfo.UsersVisibility))
                        {
                            var users = tileinfo.UsersVisibility.Split(';');
                            if (users.Length > 0 && !users.Contains(user.Identity.Name))
                                continue;
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
                                    continue;
                            }
                        }

                        mapTiles[pair.Key].Add(tileinfo);
                    }
                }

                return mapTiles;
            }

            return Global.mapTiles;
        }
    }
}