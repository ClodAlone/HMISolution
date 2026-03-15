using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFRecipeExecuter
{
    internal class SubscribeDataValues
    {
        #region Declarations
        readonly Dictionary<Guid, SubscribeTagReference> subscribedDataValues;
        
        Timer checkSubscribedDataValues;
        int waitTimeout;
        #endregion

        #region Constructors
        public SubscribeDataValues(IDictionary<Guid, SubscribeTagReference> subscribedTags, int waitTimeout)
        {
            subscribedDataValues = new Dictionary<Guid, SubscribeTagReference>(subscribedTags);
            this.waitTimeout = waitTimeout > 0 ? waitTimeout : Properties.Settings.Default.defaultSyncTimeout;
        }
        #endregion

        #region Events
        public EventHandler Timeout;
        void OnTimeout()
        {
            Timeout?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods
        public void Add(Guid guid, SubscribeTagReference subscribedTag)
        {
            lock (subscribedDataValues)
            {
                if (!subscribedDataValues.ContainsKey(guid))
                    subscribedDataValues.Add(guid, subscribedTag);
            }
        }

        public SubscribeTagReference Remove(Guid guid)
        {
            lock (subscribedDataValues)
            {
                SubscribeTagReference subscribedTag = null;
                if (subscribedDataValues.ContainsKey(guid))
                {
                    subscribedTag = subscribedDataValues[guid];
                    subscribedDataValues.Remove(guid);
                }
                return subscribedTag;
            }
        }

        public int Count
        {
            get
            {
                lock (subscribedDataValues)
                {
                    return subscribedDataValues.Count;
                }
            }
        }

        public void Subscribe()
        {
            List<SubscribeTagReference> values = null;
            lock (subscribedDataValues)
            {
                values = subscribedDataValues.Values.ToList();
                if (subscribedDataValues.Count > 0)
                {
                    if (checkSubscribedDataValues != null)
                        checkSubscribedDataValues.Dispose();
                    checkSubscribedDataValues = new Timer((o) =>
                    {
                        List<SubscribeTagReference> subscribed = null;
                        lock (subscribedDataValues)
                        {
                            subscribed = subscribedDataValues.Values.ToList();
                            if (subscribed != null && subscribed.Count == 0)
                            {
                                if (checkSubscribedDataValues != null)
                                {
                                    checkSubscribedDataValues.Dispose();
                                    checkSubscribedDataValues = null;
                                }
                            }
                        }

                        if (subscribed != null && subscribed.Count > 0)
                        {
                            foreach (var item in subscribed)
                            {
                                if (StatusCode.IsBad(item.Quality))
                                {
                                    OnTimeout();
                                    return;
                                }
                            }
                        }

                    }, this, TimeSpan.FromMilliseconds(waitTimeout), TimeSpan.FromMilliseconds(waitTimeout));
                }
            }

            if (values != null && values.Count > 0)
            {
                values.ForEach((item) =>
                {
                    item.Subscribe();
                });
            }
        }

        public void Unsubscribe()
        {
            ManualResetEvent notifyObject = null;
            List<SubscribeTagReference> values = null;
            lock (subscribedDataValues)
            {
                values = subscribedDataValues.Values.ToList();
                subscribedDataValues.Clear();

                if (checkSubscribedDataValues != null)
                {
                    notifyObject = new ManualResetEvent(false);
                    checkSubscribedDataValues.Dispose(notifyObject);
                    checkSubscribedDataValues = null;
                }
            }

            if (notifyObject != null)
            {
                notifyObject.WaitOne();
                notifyObject.Dispose();
            }

            if (values != null && values.Count > 0)
            {
                values.ForEach((item) =>
                {
                    item.Dispose();
                });
            }
        }
        #endregion
    }
}
