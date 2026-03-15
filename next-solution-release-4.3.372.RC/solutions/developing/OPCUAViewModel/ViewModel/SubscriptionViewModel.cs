using System;
using System.Linq;
using System.Collections.Generic;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using Utilities;
using System.Windows.Input;
using UFInterfaces;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
#endif

namespace OPCUAViewModel
{
    public class SubscriptionViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public Subscription subscription { get; protected set; }

        Dictionary<ReferenceDescription, MonitoredItemViewModel> mapMonitoredItems = new Dictionary<ReferenceDescription, MonitoredItemViewModel>();
        List<MonitoredItemViewModel> listReadBackAfterWriteMonitoredItems;

        public static SafeObservableCollection<SubscriptionViewModel> listActiveSubscriptions = new SafeObservableCollection<SubscriptionViewModel>();
        static Dictionary<String, SubscriptionViewModel> mapActiveSubscriptions = new Dictionary<String, SubscriptionViewModel>();

//#if !NET_STANDARD
//        static readonly log4net.ILog log = log4net.LogManager.GetLogger("MyDebug");
//#endif
#endregion

#region Constructor
        public SubscriptionViewModel(Subscription s, TreeViewItemViewModel parent, bool bNano)
            : base(parent, false)
        {
            if (s == null)
                throw new ArgumentNullException("Subscription");

            subscription = s;
            subscription.StateChanged += subscription_StateChanged;
            subscription.PublishStatusChanged += subscription_PublishStatusChanged;
            subscription.Session.Notification += Session_Notification;
            m_publishLateCount = 0;

            Title = s.DisplayName;
            CreateUniqueName();

            lock (mapActiveSubscriptions)
            {
                if (!listActiveSubscriptions.Contains(this))
                    listActiveSubscriptions.Add(this);

                mapActiveSubscriptions.Add(GetComposedTitle(), this);
            }

            bNanoServerFound = bNano;
            if (bNanoServerFound)
                StartPollingRead();
        }

        void Session_Notification(
#if !NET_STANDARD
            Session s, 
#else
            ISession s,
#endif
            NotificationEventArgs e)
        {
            if (subscription == null)
                return;
            if (!Object.ReferenceEquals(s, subscription.Session))
                return;

            SessionViewModel parent = Parent as SessionViewModel;
            if (parent != null && !parent.Connected)
                return;

            foreach (MonitoredItemNotification change in e.NotificationMessage.GetDataChanges(false))
            {
                MonitoredItem monitoredItem = subscription.FindItemByClientHandle(change.ClientHandle);
                if (monitoredItem == null || monitoredItem.Handle == null)
                    continue;

                MonitoredItemViewModel mvm = monitoredItem.Handle as MonitoredItemViewModel;
                if (mvm == null)
                    continue;

                // check of publishing has stopped for some reason.
                if (subscription.PublishingStopped)
                    change.Value.StatusCode = StatusCodes.UncertainNoCommunicationLastUsableValue;
//#if !NET_STANDARD
//                log.DebugFormat("Session_Notification : tag name = {0}, old value = {1}, new vaue = {2}, old quality = {3}, new quality = {4}, SequenceNumber = {5}, Time = {6}", 
//                    mvm.Title, mvm.DataValue, change.Value, mvm.DataValue.StatusCode, change.Value.StatusCode, change.Message.SequenceNumber, DateTime.UtcNow);
//#endif
                mvm.DataValue = change.Value;
            }

            List<MonitoredItemViewModel> listUpdated = new List<MonitoredItemViewModel>();
            foreach (EventFieldList change in e.NotificationMessage.GetEvents(false))
            {
                MonitoredItem monitoredItem = subscription.FindItemByClientHandle(change.ClientHandle);
                if (monitoredItem == null || monitoredItem.Handle == null)
                    continue;

                MonitoredItemViewModel mvm = monitoredItem.Handle as MonitoredItemViewModel;
                if (mvm == null)
                    continue;

                if (!listUpdated.Contains(mvm))
                {
                    listUpdated.Add(mvm);
                    mvm.PrepareListUpdates();
                }
                mvm.CheckAndUpdateEventList(change);
            }

            listUpdated.ForEach(value => value.EndListUpdates());
        }

        internal int m_publishLateCount;
        void subscription_PublishStatusChanged(object sender, EventArgs e)
        {
            // check for events from discarded sessions.
            if (!Object.ReferenceEquals(sender, subscription))
                return;

            if (subscription != null && subscription.PublishingStopped)
                m_publishLateCount++;
            else
                m_publishLateCount = 0;

            if (!IgnoreAllErrors)
            {
                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASubscriptionPublishStatusChanged);
            }
        }

        void subscription_StateChanged(Subscription s, SubscriptionStateChangedEventArgs e)
        {
            // check for events from discarded sessions.
            if (!Object.ReferenceEquals(s, subscription))
                return;

            if (!IgnoreAllErrors)
            {
                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASubscriptionStateChanged);
            }
        }
#endregion

#region Methods

        void CreateUniqueName()
        {
            var s = subscription;
            lock (mapActiveSubscriptions)
            {
                String originalTitle = s.DisplayName;
                int i = 1;
                while (mapActiveSubscriptions.ContainsKey(GetComposedTitle()))
                    Title = s.DisplayName = String.Format("{0} {1}", originalTitle, i++);
            }
        }

        public String GetComposedTitle()
        {
            SessionViewModel parent = Parent as SessionViewModel;
            return String.Format("{0}.{1}", parent.GetComposedTitle(), Title);
        }

        public SessionViewModel GetSessionViewModelParent()
        {
            TreeViewItemViewModel parent = Parent;
            while (parent != null && !(parent is SessionViewModel))
                parent = Parent.Parent;

            return parent as SessionViewModel;
        }

        public static SubscriptionViewModel GetSubscriptionViewModel(String name)
        {
            SubscriptionViewModel ret = null;

            lock (mapActiveSubscriptions)
            {
                mapActiveSubscriptions.TryGetValue(name, out ret);
            }

            return ret;
        }

#if !NET_STANDARD
        long maxNodesPerRead = 0;
        internal void SetMaxNodesPerRead(long n)
        {
            maxNodesPerRead = n;
        }

        long maxMonitoredItemsPerCall = 0;
        internal void SetMaxMonitoredItemsPerCall(long n)
        {
            maxMonitoredItemsPerCall = n;
            if (subscription != null)
                subscription.MaxMonitoredItemsPerCall = n;
        }

        long maxNodesPerTranslateBrowsePathsToNodeIds = 0;
        internal void SetMaxNodesPerTranslateBrowsePathsToNodeIds(long n)
        {
            maxNodesPerTranslateBrowsePathsToNodeIds = n;
            GetSessionViewModelParent().SetMaxNodesPerTranslateBrowsePathsToNodeIds(n);
            if (subscription != null)
                subscription.MaxNodesPerTranslateBrowsePathsToNodeIds = n;
        }
#endif

        public void RemoveMonitoredItems()
        {
            var s = subscription;

            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                //if (PublishingEnabled)
                //{
                //    bReenablePublish = true;
                //    PublishingEnabled = false;
                //}

                if (hasChildren)
                {
                    var children = new List<TreeViewItemViewModel>(Children);
                    foreach (MonitoredItemViewModel mi in children)
                    {
                        if (mi == null)
                            continue;

                        if (s != null)
                            s.RemoveItem(mi.monitoredItem);
                        mi.Dispose();
                    }
                    Children.Clear();
                }

                if (mapMonitoredItems != null)
                    mapMonitoredItems.Clear();

                // subscription.DeleteItems();
            }
        }

        public void SetMonitoringMode(MonitoringMode monitoringMode,
                                      IList<MonitoredItem> monitoredItems)
        {
            try
            {
                subscription.SetMonitoringMode(monitoringMode, monitoredItems);
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("{0} - {1}", Title, ex.Message);
                return;
            }
        }

        bool bReenablePublish;
        public void ApplyChanges()
        {
            if (bNanoServerFound)
                return;

            try
            {
                subscription.ApplyChanges();
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("{0} - {1}", Title, ex.Message);
                return;
            }

            if (bReenablePublish)
            {
                bReenablePublish = false;
                PublishingEnabled = true;
                if (PublishingEnabled != true)
                    bReenablePublish = true;
            }
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddMonitoredItem(ExpandedNodeIdCollection list)
        {
            var ret = new Dictionary<ExpandedNodeId, MonitoredItemViewModel>();
            var s = subscription;
            if (s == null)
                return ret;
            var session = s.Session;
            if (session == null)
                return ret;

            using (var updater = new CollectionUpdater(Children))
            {
                lock (lockObject)
                {
                    List<MonitoredItem> listItems = new List<MonitoredItem>();
                    foreach (ExpandedNodeId item in list)
                    {
                        // toList.Add(refer.referenceDescription);
                        Node node = session.NodeCache.Find(item) as Node;
                        if (node == null)
                            continue;
                        var nodeName = String.Format("{0} ({1})", node.DisplayName, node.NodeId);

                        MonitoredItem monitoredItem = new MonitoredItem(s.DefaultItem)
                        {
                            AttributeId = Attributes.Value,
                            SamplingInterval = 0,
                            DisplayName = nodeName,
                            StartNodeId = node.NodeId,
                            MonitoringMode = MonitoringMode.Reporting,
                            NodeClass = node.NodeClass
                        };

                        listItems.Add(monitoredItem);

                        MonitoredItemViewModel mi = new MonitoredItemViewModel(monitoredItem, null, this);

                        Children.Add(mi);
                        ret.Add(item, mi);
                    }

                    s.AddItems(listItems);

                    return ret;
                }
            }
        }

        public Dictionary<NodeId, MonitoredItemViewModel> AddMonitoredItem(NodeIdCollection list)
        {
            var ret = new Dictionary<NodeId, MonitoredItemViewModel>();
            var s = subscription;
            if (s == null)
                return ret;
            var session = s.Session;
            if (session == null)
                return ret;

            using (var updater = new CollectionUpdater(Children))
            {
                lock (lockObject)
                {
                    List<MonitoredItem> listItems = new List<MonitoredItem>();
                    foreach (NodeId item in list)
                    {
                        Node node = session.NodeCache.Find(item) as Node;
                        var nodeName = item.ToString();
                        if (node != null)
                            nodeName = String.Format("{0} ({1})", node.DisplayName, node.NodeId);

                        MonitoredItem monitoredItem = new MonitoredItem(s.DefaultItem)
                        {
                            AttributeId = Attributes.Value,
                            SamplingInterval = 0,
                            DisplayName = nodeName,
                            StartNodeId = item,
                            MonitoringMode = MonitoringMode.Reporting,
                        };

                        listItems.Add(monitoredItem);

                        MonitoredItemViewModel mi = new MonitoredItemViewModel(monitoredItem, null, this);
                        if (node is ObjectNode && GetSessionViewModelParent().Connected)
                        {
                            var notifier = node as ObjectNode;
                            if ((notifier.EventNotifier & EventNotifiers.SubscribeToEvents) != 0 ||
                                (notifier.EventNotifier & EventNotifiers.HistoryRead) != 0 ||
                                (notifier.EventNotifier & EventNotifiers.HistoryWrite) != 0)
                            {
                                mi.DataValue = new DataValue(StatusCodes.Good);
                            }
                        }

                        Children.Add(mi);
                        if(!ret.ContainsKey(item))
                            ret.Add(item, mi);
                    }

                    s.AddItems(listItems);

                    return ret;
                }
            }
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddAlarmEventMonitoredItem(ExpandedNodeIdCollection list, int? accessMask, int? accessLevel)
        {
            return AddEventMonitoredItem(list, new NodeId[] { ObjectTypeIds.ConditionType }, accessMask, accessLevel,
                    ObjectTypeIds.DialogConditionType,
                    ObjectTypeIds.AlarmConditionType,
                    ObjectTypeIds.ExclusiveLevelAlarmType,
                    ObjectTypeIds.ExclusiveLimitAlarmType,
                    ObjectTypeIds.NonExclusiveLimitAlarmType,
                    ObjectTypeIds.NonExclusiveLevelAlarmType,
                    ObjectTypeIds.AuditEventType,
                    ObjectTypeIds.AuditUpdateMethodEventType,
                    ObjectTypeIds.LimitAlarmType,
                    ObjectTypeIds.ExclusiveRateOfChangeAlarmType,
                    ObjectTypeIds.ExclusiveDeviationAlarmType,
                    ObjectTypeIds.NonExclusiveRateOfChangeAlarmType,
                    ObjectTypeIds.NonExclusiveDeviationAlarmType,
                    ObjectTypeIds.DiscreteAlarmType,
                    ObjectTypeIds.OffNormalAlarmType,
                    ObjectTypeIds.TripAlarmType);
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddAuditingEventMonitoredItem(ExpandedNodeIdCollection list)
        {
            return AddEventMonitoredItem(list, new NodeId[] { ObjectTypeIds.AuditUpdateMethodEventType }, null, null);
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddSystemEventMonitoredItem(ExpandedNodeIdCollection list)
        {
            return AddEventMonitoredItem(list, new NodeId[] { ObjectTypeIds.SystemEventType }, null, null);
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddAllEventMonitoredItem(ExpandedNodeIdCollection list)
        {
            return AddEventMonitoredItem(list, new NodeId[] 
                                                { 
                                                    ObjectTypeIds.ConditionType, 
                                                    ObjectTypeIds.AuditUpdateMethodEventType
                                                }, null, null,
                    ObjectTypeIds.DialogConditionType,
                    ObjectTypeIds.AlarmConditionType,
                    ObjectTypeIds.ExclusiveLevelAlarmType,
                    ObjectTypeIds.ExclusiveLimitAlarmType,
                    ObjectTypeIds.NonExclusiveLimitAlarmType,
                    ObjectTypeIds.NonExclusiveLevelAlarmType,
                    ObjectTypeIds.AuditEventType,
                    ObjectTypeIds.AuditUpdateMethodEventType,
                    ObjectTypeIds.LimitAlarmType,
                    ObjectTypeIds.ExclusiveRateOfChangeAlarmType,
                    ObjectTypeIds.ExclusiveDeviationAlarmType,
                    ObjectTypeIds.NonExclusiveRateOfChangeAlarmType,
                    ObjectTypeIds.NonExclusiveDeviationAlarmType,
                    ObjectTypeIds.DiscreteAlarmType,
                    ObjectTypeIds.OffNormalAlarmType,
                    ObjectTypeIds.TripAlarmType);
        }

        public Dictionary<ExpandedNodeId, MonitoredItemViewModel> AddEventMonitoredItem(ExpandedNodeIdCollection list,
            NodeId[] conditionType, int? accessMask, int? accessLevel, params NodeId[] eventTypeIds)
        {
            var ret = new Dictionary<ExpandedNodeId, MonitoredItemViewModel>();

            var s = subscription;
            if (s == null)
                return ret;
            var session = s.Session;
            if (session == null)
                return ret;

            using (var updater = new CollectionUpdater(Children))
            {
                lock (lockObject)
                {
                    List<MonitoredItem> listItems = new List<MonitoredItem>();
                    foreach (ExpandedNodeId item in list)
                    {
                        // toList.Add(refer.referenceDescription);
                        Node node = session.NodeCache.Find(item) as Node;
                        if (node == null)
                            continue;
                        var nodeName = String.Format("{0} ({1})", node.DisplayName, node.NodeId);

                        MonitoredItem monitoredItem = new MonitoredItem(s.DefaultItem)
                        {
                            DisplayName = nodeName,
                            StartNodeId = node.NodeId,
                            RelativePath = null,
                            NodeClass = NodeClass.Object,
                            AttributeId = Attributes.EventNotifier,
                            IndexRange = null,
                            Encoding = null,
                            MonitoringMode = MonitoringMode.Reporting,
                            SamplingInterval = 0,
                            QueueSize = UInt32.MaxValue,
                            DiscardOldest = true,
                        };

                        listItems.Add(monitoredItem);

                        MonitoredItemViewModel mi = new MonitoredItemViewModel(monitoredItem, null, this, true);
                        mi.Severity = EventSeverity.Min;
                        mi.IgnoreSuppressedOrShelved = true;
                        mi.EventTypes = conditionType;
                        if (accessMask.HasValue)
                            mi.UserAccessMask = accessMask.Value;
                        if (accessLevel.HasValue)
                            mi.UserAccessLevel = accessLevel.Value;

                        // must specify the fields that the form is interested in.
                        mi.SelectClauses = MonitoredItemViewModel.ConstructSelectClauses(
                            session, eventTypeIds);
                        monitoredItem.Filter = mi.ConstructFilter();

                        Children.Add(mi);
                        ret.Add(item, mi);
                    }

                    s.AddItems(listItems);

                    return ret;
                }
            }
        }

        public void RemoveMonitoredItem(MonitoredItemViewModel mi)
        {
            //if (PublishingEnabled)
            //{
            //    bReenablePublish = true;
            //    PublishingEnabled = false;
            //}

            var s = subscription;
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (s != null)
                    s.RemoveItem(mi.monitoredItem);
                if (hasChildren)
                    Children.Remove(mi);
                mi.Dispose();
            }
        }

        public void RemoveMonitoredItem(List<MonitoredItemViewModel> list)
        {
            using (var updater = new CollectionUpdater(Children))
            {
                list.ForEach(mi => RemoveMonitoredItem(mi));
            }
        }

#if !WINDOWS_UWP
        public void AddMonitoredItem(SafeObservableCollection<TreeViewItemViewModel> list)
        {
            var s = subscription;
            if (s == null)
                return;
            var session = s.Session;
            if (session == null)
                return;

            // List<ReferenceDescription> toList = new List<ReferenceDescription>();
            using (var updater = new CollectionUpdater(Children))
            {
                lock (lockObject)
                {
                    List<MonitoredItem> listItems = new List<MonitoredItem>();
                    foreach (TreeViewItemViewModel item in list)
                    {
                        ReferenceDescriptionViewModel refer = item as ReferenceDescriptionViewModel;
                        if (refer.IsUserReadable)
                        {
                            MonitoredItem monitoredItem = null;
                            // toList.Add(refer.referenceDescription);
                            // Node node = subscription.Session.NodeCache.Find(refer.NodeId) as Node;
                            // if (node == null)
                            {
                                NodeId refNodeId = null;
                                if (s != null && session != null)
                                    refNodeId = ExpandedNodeId.ToNodeId(refer.NodeId, session.NamespaceUris);
                                else
                                    refNodeId = (NodeId)refer.NodeId;

                                monitoredItem = new MonitoredItem(s.DefaultItem)
                                {
                                    AttributeId = Attributes.Value,
                                    SamplingInterval = 0,
                                    DisplayName = String.Format("{0} ({1})", refer.DisplayName, refer.NodeId),
                                    MonitoringMode = MonitoringMode.Reporting,
                                    StartNodeId = refNodeId,
                                    NodeClass = refer.NodeClass
                                };
                            }
                            //else
                            //{
                            //    monitoredItem = new MonitoredItem(subscription.DefaultItem)
                            //    {
                            //        AttributeId = Attributes.Value,
                            //        SamplingInterval = 0,
                            //        DisplayName = String.Format("{0} ({1})", node.DisplayName, node.NodeId),
                            //        StartNodeId = node.NodeId,
                            //        MonitoringMode = MonitoringMode.Reporting,
                            //        NodeClass = node.NodeClass
                            //    };
                            //}

                            // add condition fields to any event filter.
                            EventFilter filter = monitoredItem.Filter as EventFilter;
                            if (filter != null)
                                monitoredItem.AttributeId = Attributes.EventNotifier;

                            listItems.Add(monitoredItem);

                            MonitoredItemViewModel mi = new MonitoredItemViewModel(monitoredItem, refer, this);
                            Children.Add(mi);
                            mapMonitoredItems.Add(refer.referenceDescription, mi);
                        }
                    }

                    s.AddItems(listItems);
                }
            }
            // AddMonitoredItem(toList);
        }

        public void RemoveMonitoredItem(ReferenceDescription refer)
        {
            var s = subscription;
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                MonitoredItemViewModel mi;
                if (!mapMonitoredItems.TryGetValue(refer, out mi))
                    return;

                if (s != null)
                    s.RemoveItem(mi.monitoredItem);
                mapMonitoredItems.Remove(refer);
                if (hasChildren)
                    Children.Remove(mi);
                mi.Dispose();
            }
        }

        public void RemoveMonitoredItem(List<ReferenceDescription> list)
        {
            using (var updater = new CollectionUpdater(Children))
            {
                list.ForEach(refer => RemoveMonitoredItem(refer));
            }
        }
#endif
        public void UpdateMonitoredItemNotConnected(StatusCode code)
        {
            var list = new List<MonitoredItemViewModel>();
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren)
                {
                    foreach (var v in Children)
                        list.Add(v as MonitoredItemViewModel);
                }
            }

            list.ForEach(mvm =>
            {
                DataValue dv = new DataValue(mvm.DataValue);
                // force good for event, nobody else will do it for us
                if (code == StatusCodes.UncertainLastUsableValue && mvm.NodeIdModel != null &&
                    (mvm.NodeIdModel.IsEventNotifier || mvm.NodeIdModel.IsEventHistoryWrite ||
                     mvm.NodeIdModel.IsEventHistoryRead))
                    code = StatusCodes.Good;
                dv.StatusCode = code;
                mvm.DataValue = dv;
            });
        }

        internal void RequeryNotConnectedItems()
        {
            var list = new List<MonitoredItemViewModel>();
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren)
                {
                    list = (from c in Children.OfType<MonitoredItemViewModel>()
                            where c.NodeIdModel != null && c.NodeIdModel.IsVariable/*
                        where c.DataValue == null || StatusCode.IsNotGood(c.DataValue.StatusCode) */
                            select c).ToList();
                }
            }

            try
            {
                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

                // AutoResetEvent h = null;
                foreach (var m in list)
                {
                    ReadValueId valueId = new ReadValueId { NodeId = m.monitoredItem.ResolvedNodeId, AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null };
                    nodesToRead.Add(valueId);
                }

                // read attributes.
                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                if (nodesToRead.Count > 0 && !bDisposed)
                {
                    try
                    {
#if !NET_STANDARD
                        if (maxNodesPerRead > 0 && nodesToRead.Count > maxNodesPerRead)
                        {
                            for (long i = 0; i < nodesToRead.Count; i += maxNodesPerRead)
                            {
                                var maxnodeList = new ReadValueIdCollection(nodesToRead.GetRange((int)i, Math.Min((int)maxNodesPerRead, (int)(nodesToRead.Count - i))));
                                var itemsRange = list.GetRange((int)i, Math.Min((int)maxNodesPerRead, (int)(list.Count - i)));

                                subscription.Session.Read(
                                null,
                                0,
                                TimestampsToReturn.Both,
                                maxnodeList,
                                out values,
                                out diagnosticInfos);

                                ClientBase.ValidateResponse(values, maxnodeList);
                                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, maxnodeList);

                                for (int ii = 0; ii < maxnodeList.Count; ii++)
                                {
                                    // check if node supports attribute.
                                    if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                                    {
                                        itemsRange[ii].DataValue = new DataValue(StatusCodes.Bad);
                                        continue;
                                    }

                                    itemsRange[ii].DataValue = values[ii];
                                    itemsRange[ii].ForceValuePropertyChanges();
                                    itemsRange[ii].ReloadNodeProperties();
                                }
                            }
                        }
                        else
#endif
                        {
                            subscription.Session.Read(
                            null,
                            0,
                            TimestampsToReturn.Both,
                            nodesToRead,
                            out values,
                            out diagnosticInfos);

                            ClientBase.ValidateResponse(values, nodesToRead);
                            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                            for (int ii = 0; ii < nodesToRead.Count; ii++)
                            {
                                // check if node supports attribute.
                                if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                                {
                                    list[ii].DataValue = new DataValue(StatusCodes.Bad);
                                    continue;
                                }

                                list[ii].DataValue = values[ii];
                                list[ii].ForceValuePropertyChanges();
                                list[ii].ReloadNodeProperties();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        public bool IsNanoServer
        {
            get
            {
                return bNanoServerFound;
            }
        }

        bool bNanoServerFound;
        public void RecreateMonitoredItems(Session session)
        {
            //if (PublishingEnabled)
            //{
            //    bReenablePublish = true;
            //    PublishingEnabled = false;
            //}

            var sub = subscription;
#if !NET_STANDARD
            Session sessionTemp = null;
#else
            ISession sessionTemp = null;
#endif
            if (sub != null)
                sessionTemp = sub.Session;

            lock (lockObject)
            {
                if (sub != null)
                {
                    sub.StateChanged -= subscription_StateChanged;
                    sub.PublishStatusChanged -= subscription_PublishStatusChanged;
                    if (sessionTemp != null)
                        sessionTemp.Notification -= Session_Notification;
                }

                Subscription s = new Subscription(sub);
                s.DisplayName = subscription.DisplayName;
                if (sub != null)
                {
                    session.RemoveSubscription(sub);
                    sub.Dispose();
                }
                subscription = s;

                foreach (var m in subscription.MonitoredItems)
                {
                    MonitoredItemViewModel mvm = m.Handle as MonitoredItemViewModel;
                    if (mvm == null)
                        continue;

                    mvm.monitoredItem = m;
                    mvm.MonitoringMode = m.MonitoringMode;
                }

                session.AddSubscription(subscription);

                subscription.StateChanged += subscription_StateChanged;
                subscription.PublishStatusChanged += subscription_PublishStatusChanged;
                subscription.Session.Notification += Session_Notification;
                m_publishLateCount = 0;
            }

            if (bNanoServerFound)
            {
                StartPollingRead();
                return;
            }

            try
            {
                subscription.Create();
            }
            catch
            {
                bNanoServerFound = true;
                StartPollingRead();
                return;
            }

            ApplyChanges();

            try
            {
                subscription.ConditionRefresh();
            }
            catch
            {

            }
            /*
            lock (lockObject)
            {
                foreach (var m in subscription.MonitoredItems)
                {
                    MonitoredItemViewModel mvm = m.Handle as MonitoredItemViewModel;
                    mvm.ForceValuePropertyChanges();
                }
            }
            */
        }

        Timer pollingTimer;
        void StartPollingRead()
        {
            lock(lockObject)
            {
                if (pollingTimer != null || bPendingPolling)
                    return;
                pollingTimer = new Timer(PollValues, this, 0, Timeout.Infinite);
            }
        }

        bool bPendingPolling;
        void PollValues(Object o)
        {
            if (!PublishingEnabled)
                return;

            var start = Stopwatch.GetTimestamp();

            var oldPriority = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            try
            {
                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

                var list = new List<MonitoredItemViewModel>();
                // AutoResetEvent h = null;
                lock (lockObject)
                {
                    //h = new AutoResetEvent(false);
                    pollingTimer.Dispose();

                    foreach (var v in Children)
                        list.Add(v as MonitoredItemViewModel);
                    bPendingPolling = true;
                }

                foreach (var m in list)
                {
                    ReadValueId valueId = new ReadValueId { NodeId = m.monitoredItem.ResolvedNodeId, AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null };
                    nodesToRead.Add(valueId);
                }

                // read attributes.
                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                if (nodesToRead.Count > 0 && !bDisposed)
                {
                    try
                    {
#if !NET_STANDARD
                        if (maxNodesPerRead > 0 && nodesToRead.Count > maxNodesPerRead)
                        {
                            for (long i = 0; i < nodesToRead.Count; i += maxNodesPerRead)
                            {
                                var maxnodeList = new ReadValueIdCollection(nodesToRead.GetRange((int)i, Math.Min((int)maxNodesPerRead, (int)(nodesToRead.Count - i))));
                                var itemsRange = list.GetRange((int)i, Math.Min((int)maxNodesPerRead, (int)(list.Count - i)));

                                subscription.Session.Read(
                                null,
                                0,
                                TimestampsToReturn.Both,
                                maxnodeList,
                                out values,
                                out diagnosticInfos);

                                ClientBase.ValidateResponse(values, maxnodeList);
                                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, maxnodeList);

                                for (int ii = 0; ii < maxnodeList.Count; ii++)
                                {
                                    // check if node supports attribute.
                                    if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                                    {
                                        itemsRange[ii].DataValue = new DataValue(StatusCodes.Bad);
                                        continue;
                                    }

                                    itemsRange[ii].DataValue = values[ii];
                                }
                            }
                        }
                        else
#endif
                        {
                            subscription.Session.Read(
                                null,
                                0,
                                TimestampsToReturn.Both,
                                nodesToRead,
                                out values,
                                out diagnosticInfos);

                            ClientBase.ValidateResponse(values, nodesToRead);
                            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                            for (int ii = 0; ii < nodesToRead.Count; ii++)
                            {
                                // check if node supports attribute.
                                if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                                {
                                    list[ii].DataValue = new DataValue(StatusCodes.Bad);
                                    continue;
                                }

                                list[ii].DataValue = values[ii];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateMonitoredItemNotConnected(StatusCodes.Bad);
                        goto exit;
                    }
                }

                //if (h != null)
                //{
                //    h.WaitOne();
                //    h.Dispose();
                //}
            }
            catch
            {
            }
            finally
            {
                Thread.CurrentThread.Priority = oldPriority;
            }

            exit:

            if (!bDisposed)
            {
                var startPublishingInterval = PublishingInterval;
                try
                {
                    var end = Stopwatch.GetTimestamp();
                    var elapsed = (int)((double)(end - start) / Stopwatch.Frequency * 1000);
                    startPublishingInterval = PublishingInterval - elapsed;
                    if (startPublishingInterval < 0)
                        startPublishingInterval = 0;
                }
                catch { }
                lock (lockObject)
                {
                    bPendingPolling = false;
                    if (!bDisposed)
                        pollingTimer = new Timer(PollValues, this, startPublishingInterval, Timeout.Infinite);
                }
            }
        }

        bool bDisposed;
        protected override void OnDispose()
        {
            base.OnDispose();
            if (bDisposed)
                return;

            AutoResetEvent h = null;
            lock (lockObject)
            {
                if (pollingTimer != null && !bPendingPolling)
                {
                    h = new AutoResetEvent(false);
                    pollingTimer.Dispose(h);
                }
            }

            //while (bPendingPolling)
            //    Thread.Sleep(100);

            if (h != null)
            {
                h.WaitOne();
                h.Dispose();
            }

            bDisposed = true;
            if (subscription != null)
            {
                try
                {
                    subscription.StateChanged -= subscription_StateChanged;
                    subscription.PublishStatusChanged -= subscription_PublishStatusChanged;
                    if (subscription.Session != null)
                        subscription.Session.Notification -= Session_Notification;

                    subscription.Dispose();
                    subscription = null;
                }
                catch
                { }
            }

            mapMonitoredItems = null;

            lock (mapActiveSubscriptions)
            {
                if (listActiveSubscriptions.Contains(this))
                    listActiveSubscriptions.Remove(this);

                try
                {
                    mapActiveSubscriptions.Remove(GetComposedTitle());
                }
                catch
                { }
            }
        }

#endregion

#region Commands
        RelayCommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(
                        param =>
                        {
                            try
                            {
                                subscription.Modify();
                                OnPropertyChanged("CurrentPublishingInterval");
                                OnPropertyChanged("CurrentKeepAliveCount");
                                OnPropertyChanged("CurrentLifeTimeCount");
                                OnPropertyChanged("CurrentPriority");

                                // Dirty the commands registered with CommandManager,
                                // such as our Save command, so that they are queried
                                // to see if they can execute now.
#if !WINDOWS_UWP && !NET_STANDARD
                                CommandManager.InvalidateRequerySuggested();
#endif
                            }
                            catch (Exception ex)
                            {
                                LastMessage = String.Format("{0} - {1}", Title, ex.Message);
                            }
                        },
                        param => CanApply
                        );
                }
                return _applyCommand;
            }
        }

        bool CanApply
        {
            get { return subscription != null && subscription.Session.Connected; }
        }
#endregion

#region Properties
        public bool ChangesPending
        {
            get
            {
                return subscription.ChangesPending;
            }
        }

        public uint CurrentKeepAliveCount
        {
            get
            {
                return subscription.CurrentKeepAliveCount;
            }
        }

        public uint CurrentLifetimeCount
        {
            get
            {
                return subscription.CurrentLifetimeCount;
            }
        }

        public byte CurrentPriority
        {
            get
            {
                return subscription.CurrentPriority;
            }
        }

        public bool CurrentPublishingEnabled
        {
            get
            {
                return subscription.CurrentPublishingEnabled;
            }
        }

        public double CurrentPublishingInterval
        {
            get
            {
                return subscription.CurrentPublishingInterval;
            }
        }

        public string DisplayName
        {
            get
            {
                return subscription.DisplayName;
            }
            set
            {
                if (String.Compare(value, subscription.DisplayName, false) == 0)
                    return;

                lock (mapActiveSubscriptions)
                {
                    mapActiveSubscriptions.Remove(GetComposedTitle());
                }

                Title = subscription.DisplayName = value;

                lock (mapActiveSubscriptions)
                {
                    mapActiveSubscriptions.Add(GetComposedTitle(), this);
                }

                OnPropertyChanged("DisplayName");
            }
        }

        public uint Id
        {
            get
            {
                return subscription.Id;
            }
        }

        public uint KeepAliveCount
        {
            get
            {
                return subscription.KeepAliveCount;
            }
            set
            {
                if (value == subscription.KeepAliveCount)
                    return;

                subscription.KeepAliveCount = value;
                OnPropertyChanged("KeepAliveCount");
            }
        }

        public uint LifetimeCount
        {
            get
            {
                return subscription.LifetimeCount;
            }
            set
            {
                if (value == subscription.LifetimeCount)
                    return;

                subscription.LifetimeCount = value;
                OnPropertyChanged("LifetimeCount");
            }
        }

        public int MaxMessageCount
        {
            get
            {
                return subscription.MaxMessageCount;
            }
            set
            {
                if (value == subscription.MaxMessageCount)
                    return;

                subscription.MaxMessageCount = value;
                OnPropertyChanged("MaxMessageCount");
            }
        }

        public uint MaxNotificationsPerPublish
        {
            get
            {
                return subscription.MaxNotificationsPerPublish;
            }
            set
            {
                if (value == subscription.MaxNotificationsPerPublish)
                    return;

                subscription.MaxNotificationsPerPublish = value;
                OnPropertyChanged("MaxNotificationsPerPublish");
            }
        }

        public uint MinLifetimeInterval
        {
            get
            {
                return subscription.MinLifetimeInterval;
            }
            set
            {
                if (value == subscription.MinLifetimeInterval)
                    return;

                subscription.MinLifetimeInterval = value;
                OnPropertyChanged("MinLifetimeInterval");
            }
        }

        public uint MonitoredItemCount
        {
            get
            {
                return subscription.MonitoredItemCount;
            }
        }

        public uint NotificationCount
        {
            get
            {
                return subscription.NotificationCount;
            }
        }

        public byte Priority
        {
            get
            {
                return subscription.Priority;
            }
            set
            {
                if (value == subscription.Priority)
                    return;

                subscription.Priority = value;
                OnPropertyChanged("Priority");
            }
        }

        public bool PublishingEnabled
        {
            get
            {
                return subscription != null && subscription.PublishingEnabled;
            }
            set
            {
                if (subscription == null || value == subscription.PublishingEnabled ||
                    !subscription.Session.Connected)
                    return;

                try
                {
                    subscription.PublishingEnabled = value;
                    subscription.SetPublishingMode(subscription.PublishingEnabled);
                    OnPropertyChanged("PublishingEnabled");
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public int PublishingInterval
        {
            get
            {
                return subscription.PublishingInterval;
            }
            set
            {
                if (value == subscription.PublishingInterval)
                    return;

                subscription.PublishingInterval = value;
                OnPropertyChanged("PublishingInterval");
            }
        }

        public bool PublishingStopped
        {
            get
            {
                return subscription.PublishingStopped;
            }
        }

        bool _ignoreAllErrors;
        public bool IgnoreAllErrors
        {
            get
            {
                return _ignoreAllErrors;
            }
            set
            {
                if (value == _ignoreAllErrors)
                    return;

                _ignoreAllErrors = value;
                OnPropertyChanged("IgnoreAllErrors");
            }
        }

#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations
        private static void ValidateSubscription(SubscriptionViewModel s)
        {
        }

        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/analog_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMAnalog", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                return new OPCUAViewModel.UserControls.SubscriptionViewModel();
#else
                return null;
#endif            
            }
        }

        public ImageSource ExpandedImageSource { get { return null; } }
        public ContextMenu contextMenu { get { return null; } }

        public object ContainedObject
        {
            get
            {
                return subscription;
            }
        }

        public object EntityParent
        {
            get
            {
                return Parent;
            }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

        #endregion
    }
}
