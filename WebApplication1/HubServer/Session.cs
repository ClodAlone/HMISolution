using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubServer
{
    internal class Session : IDisposable
    {
        IClientProxy Caller;
        String User;
        String Id;
        DateTime keepAlive;
        bool disposed;

        static TimeSpan keepAliveTimeout = new TimeSpan(0, 1, 0);
        static TimeSpan keepAliveTick = new TimeSpan(0, 1, 0);
        Timer keepAliveThread;

        List<Subscription> subscriptions = new List<Subscription>();

        internal Session(String user, String id, IClientProxy caller) 
        { 
            User = user; 
            Id = id;
            Caller = caller;

            keepAlive = DateTime.UtcNow;

            keepAliveThread = new Timer((o) =>
            {
                if (!disposed)
                    CheckSubscriptionAlive();
            }, null, keepAliveTick, keepAliveTick);
        }

        void CheckSubscriptionAlive()
        {
            var toClean = new List<Subscription>();
            lock (subscriptions)
            {
                (from c in subscriptions where !c.IsAlive() select c).ToList().ForEach(s =>
                    {
                        subscriptions.Remove(s);
                        s.Dispose();
                    });                
            }
        }

        internal int CreateSubscription()
        {
            return 0;
        }

        public void Dispose()
        {
            disposed = true;

            var timerDisposed = new ManualResetEvent(false);
            keepAliveThread.Dispose(timerDisposed);
            timerDisposed.WaitOne();

            lock (subscriptions)
            {
                subscriptions.ForEach(s => s.Dispose());
                subscriptions.Clear();
            }
        }
    }
}
