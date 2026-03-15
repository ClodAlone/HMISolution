using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCUAViewModel;
using UFInterfaces;
using ViewModelLib;
using Opc.Ua;
using DocumentManager.ComponentService;
using System.Threading;

namespace UFProjectManager.ScriptHelpers
{
    public class Servers : IDisposable, IEntityReference
    {
        readonly Object lockObject = new Object();

        readonly IDocument Document;
        readonly List<OPCUAEntityReference> serverReferences = new List<OPCUAEntityReference>();
        readonly Dictionary<Opc.Ua.NodeId, NodeIdViewModel> mapNodeIdModels = new Dictionary<Opc.Ua.NodeId, NodeIdViewModel>();
        readonly List<PropertyObserver<OPCUAEntityReference>> observers = new List<PropertyObserver<OPCUAEntityReference>>();
        readonly Dictionary<MonitoredItemViewModel, PropertyObserver<MonitoredItemViewModel>> observersItems = new Dictionary<MonitoredItemViewModel, PropertyObserver<MonitoredItemViewModel>>();

        public Servers(IDocument d)
        {
            Document = d;
        }

        void Unsubscribe()
        {
            lock (lockObject)
            {
                serverReferences.ForEach(value =>
                    {
                        value.SetInUse(this, false);
                    });
                serverReferences.Clear();
                observers.ForEach(value =>
                    {
                        value.Dispose();
                    });
                observers.Clear();
                observersItems.Values.ToList().ForEach(value =>
                {
                    value.Dispose();
                });
                observersItems.Clear();

                mapNodeIdModels.Clear();
            }
        }

        public event EventHandler<VariableChangedEventArgs> VariableChanged;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnVariableChanged(VariableChangedEventArgs ea)
        {
            if (VariableChanged != null)
                VariableChanged(null/*this*/, ea);
        }

        public event EventHandler<ItemConnectedEventArgs> ItemConnected;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnItemConnected(ItemConnectedEventArgs ea)
        {
            if (ItemConnected != null)
                ItemConnected(null/*this*/, ea);
        }

        public event EventHandler<ItemDiscoveredEventArgs> ItemDiscovered;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnItemDiscovered(ItemDiscoveredEventArgs ea)
        {
            if (ItemDiscovered != null)
                ItemDiscovered(null/*this*/, ea);
        }

        public void SetUserIdentity(String sessionName, String user, String password)
        {
            RealTimeConnectionManagerViewModel.SetUserIdentity(sessionName, new UserIdentity(user, password), new StringCollection());
        }

        public MonitoredItemViewModel ConnectToServer(String applicationName, String endpoint, String path, 
            Opc.Ua.NodeId nodeId, bool bAllowAutoRedundancy = true, int timeout = 0)
        {
            MonitoredItemViewModel ret = null;
            ManualResetEvent me = null;
            if (timeout > 0)
                me = new ManualResetEvent(false);

            var entityreference = new OPCUAEntityReference(null, applicationName, endpoint,
                                            path, nodeId, null, null, path, bAllowAutoRedundancy);

            var observer = new PropertyObserver<OPCUAEntityReference>(entityreference);
            var mi = entityreference.MonitoredItemViewModel;
            if (mi != null)
            {
                OnItemConnected(new ItemConnectedEventArgs(applicationName, endpoint, path, nodeId, mi));

                lock (lockObject)
                {
                    if (!observersItems.ContainsKey(mi))
                    {
                        var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(mi);
                        observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                        {
                            OnVariableChanged(new VariableChangedEventArgs(applicationName, endpoint, path, nodeId, m.DataValue));
                        });

                        observersItems.Add(mi, observerMonitoredModel);
                    }
                }

                return mi;
            }
            else
            {
                observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                // observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                    OnItemConnected(new ItemConnectedEventArgs(applicationName, endpoint, path, nodeId, n.MonitoredItemViewModel));

                    lock (lockObject)
                    {
                        if (!observersItems.ContainsKey(n.MonitoredItemViewModel))
                        {
                            var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                            observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                            {
                                OnVariableChanged(new VariableChangedEventArgs(applicationName, endpoint, path, nodeId, m.DataValue));
                            });

                            observersItems.Add(n.MonitoredItemViewModel, observerMonitoredModel);
                        }

                        if (me != null)
                            me.Set();
                    }
                });
            }

            observer.RegisterHandler(n => n.NodeIdViewModel, n =>
            {
                // observer.UnregisterHandler(p => p.NodeIdViewModel);

                Opc.Ua.NodeId nodeid = null;
                if (n.NodeIdViewModel != null)
                {
                    lock (lockObject)
                    {
                        nodeid = (Opc.Ua.NodeId)n.NodeIdViewModel.nodeId;
                        if (mapNodeIdModels.ContainsKey(nodeid))
                            mapNodeIdModels.Remove(nodeid);
                        mapNodeIdModels.Add(nodeid, n.NodeIdViewModel);
                    }
                }
                OnItemDiscovered(new ItemDiscoveredEventArgs(applicationName, endpoint, path, nodeid, n.NodeIdViewModel));
            });

            entityreference.Resolve(SessionName, Document);
            entityreference.SetInUse(this, true);
            lock (lockObject)
            {
                serverReferences.Add(entityreference);
            }

            lock (lockObject)
            {
                observers.Add(observer);
            }

            if (me != null)
            {
                if (me.WaitOne(timeout))
                    ret = entityreference.MonitoredItemViewModel;

                lock (lockObject)
                {
                    me.Dispose();
                    me = null;
                }
            }

            return ret;
        }

        public Object CallMethod(Opc.Ua.NodeId nodeId, params object[] args)
        {
            NodeIdViewModel nodeIdModel = null;
            lock(lockObject)
            {
                if (!mapNodeIdModels.ContainsKey(nodeId))
                    throw new ArgumentException("Cannot find the argument nodeid viewmodel");
                nodeIdModel = mapNodeIdModels[nodeId];
            }

            var parameters = new Opc.Ua.VariantCollection();
            foreach (var arg in args)
                if (arg != null)
                    parameters.Add(new Opc.Ua.Variant(arg));

            return nodeIdModel.CallMethod(parameters.ToArray());
        }

        public Object CallMethod2(Opc.Ua.NodeId nodeId, Opc.Ua.NodeId objectId, params object[] args)
        {
            NodeIdViewModel nodeIdModel = null;
            lock (lockObject)
            {
                if (!mapNodeIdModels.ContainsKey(nodeId))
                    throw new ArgumentException("Cannot find the argument nodeid viewmodel");
                nodeIdModel = mapNodeIdModels[nodeId];
            }

            var parameters = new Opc.Ua.VariantCollection();
            foreach (var arg in args)
                if (arg != null)
                    parameters.Add(new Opc.Ua.Variant(arg));

            return nodeIdModel.CallMethod2(objectId, parameters.ToArray());
        }

        public bool IsMethodExecutable(Opc.Ua.NodeId nodeId)
        {
            NodeIdViewModel nodeIdModel = null;
            lock (lockObject)
            {
                if (!mapNodeIdModels.ContainsKey(nodeId))
                    throw new ArgumentException("Cannot find the argument nodeid viewmodel");
                nodeIdModel = mapNodeIdModels[nodeId];
            }

            return nodeIdModel.IsMethodExecutable;
        }

        String sessionName = "ScriptServerSession";
        public String SessionName
        {
            get
            {
                return sessionName;
            }
            set
            {
                if (value != Document.Title)
                    sessionName = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Unsubscribe();
        }

        #region EnityReference

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            ImageSource CollapsedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Controls.
#endif
            ContextMenu contextMenu
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get { return Document; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            // get { return Properties.Resources.ServersScript; }
            get { return String.Empty; }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion

    }
}
