using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Opc.Ua;
using ServerSignalR.Hubs.ConnectionManager;
using ServerSignalR.Simulator;
using System.Diagnostics;

namespace ServerSignalR.Hubs
{
    public class DataHub : Hub
    {
        private readonly string _anonymous = "anonymous";

        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        private readonly static Dictionary<string, Stopwatch> _stopWatches = new Dictionary<string, Stopwatch>();

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task<string> GetData()
        {
            await Task.Run(() =>
            {
                lock (_stopWatches)
                {
                    if (!_stopWatches.ContainsKey(Context.ConnectionId))
                    {
                        var stopwatch = new Stopwatch();
                        _stopWatches.Add(Context.ConnectionId, stopwatch);
                        stopwatch.Start();
                    }
                    else
                    {
                        var stopwatch = _stopWatches[Context.ConnectionId];
                        stopwatch.Stop();
                        Console.WriteLine("Elapsed Time between calls {0} ms, Connection {1}", 
                            stopwatch.ElapsedMilliseconds, Context.ConnectionId);
                        stopwatch.Restart();
                    }
                }
            });

            return Simulator.Simulator.GetData();
        }

        public override Task OnConnectedAsync()
        {
            //string name = Context.User.Identity.Name ?? _anonymous;

            _connections.Add(_anonymous, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? ex)
        {
            //string name = Context.User.Identity.Name ?? _anonymous;

            _connections.Remove(_anonymous, Context.ConnectionId);

            return base.OnDisconnectedAsync(ex);
        }
    }
}
