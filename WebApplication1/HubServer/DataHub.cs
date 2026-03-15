using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RealTimeData;
using RealTimeTags;

namespace HubServer
{
    public class DataHub : Hub
    {
        private readonly string _anonymous = "anonymous";
        static Dictionary<String, Session> mapActiveSessions = new Dictionary<String, Session>();

        static NodeManager activeNodeManager;
        static void SetActiveNodeManager(NodeManager anm)
        {
            if (anm == null)
                throw new ArgumentException("Node Manager cannot be null !");
            if (activeNodeManager != null)
                throw new Exception("Node Manager already Active !");
            activeNodeManager = anm;
        }

        public override Task OnConnectedAsync()
        {
            if (activeNodeManager == null)
                throw new Exception("Node Manager not defined yet !");

            String userName = _anonymous;
            if (Context.User != null && Context.User.Identity != null && 
                !String.IsNullOrEmpty(Context.User.Identity.Name))
                userName = Context.User.Identity.Name;

            var session = new Session(userName, Context.ConnectionId, Clients.Caller);
            lock (mapActiveSessions) 
            {
                if (mapActiveSessions.ContainsKey(Context.ConnectionId))
                    throw new Exception("Session already active !");

                mapActiveSessions.Add(Context.ConnectionId, session);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? ex)
        {
            Session session = null;
            lock (mapActiveSessions)
            {
                if (!mapActiveSessions.ContainsKey(Context.ConnectionId))
                    throw new Exception("Session not found !");

                session = mapActiveSessions[Context.ConnectionId];
                mapActiveSessions.Remove(Context.ConnectionId);
            }

            if (session != null)
                session.Dispose();

            return base.OnDisconnectedAsync(ex);
        }

        public DataConfiguration GetConfiguration(String hash)
        {
            var jsonString = JsonSerializer.Serialize(activeNodeManager);
            var hash = jsonString.GetHashcode
            return new DataConfiguration()  {
                
            };
        }
    }
}
