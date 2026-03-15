#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A line router provides line routing services for a diagram.
    /// model.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(LineRouterConverter))]
    public abstract class LineRouter
        : Component,
          ISerializable
    {
        #region Fields
        /// <summary>
        /// Indicates whether line routing engine is updating routes right now.
        /// </summary>
        protected bool m_bInAction;

        /// <summary>
        /// Document to which LineRouting engine is attached.
        /// </summary>
        private Model m_model;

        /// <summary>
        /// Distance from routing line to obstacles.
        /// </summary>
        private int m_nDistance;

        /// <summary>
        /// Defines routing mode.
        /// </summary>
        private RoutingMode m_routingMode;

        /// <summary>
        /// Document update requests.
        /// </summary>
        private int m_nUpdateRequests;

        /// <summary>
        /// Collection of connectors to reroute in SemiAutomatic mode.
        /// </summary>
        private NodeCollection m_nodesConnectors;

        /// <summary>
        /// Indicate that line router update bridges after route connectors.
        /// </summary>
        private bool m_bGenerateBridges = false;
        #endregion

        #region Initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineRouter"/> class.
        /// </summary>
        public LineRouter()
        {
            m_nDistance = 5;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRouter"/> class.
        /// </summary>
        /// <param name="model">Model to attach line router to</param>
        public LineRouter(Model model)
            : this()
        {
            if (model == null)
                throw new ArgumentNullException("model");

            this.Model = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRouter"/> class.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected LineRouter(SerializationInfo info, StreamingContext context)
        {
            m_nDistance = info.GetInt32("nDistance");
            m_routingMode = (RoutingMode)info.GetValue("routingMode", typeof(RoutingMode));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets distance to obstacles.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Distance from routing connector to obstacles.")]
        [DefaultValue(5)]
        public int DistanceToObstacles
        {
            get 
            { 
                return m_nDistance; 
            }
            set
            {
                if (m_nDistance != value)
                {
                    m_nDistance = value;

                    // reroute document connectors
                    RouteAllModelConnectors();
                }
            }
        }

        /// <summary>
        /// Gets or sets model the line router is attached to.
        /// Increased when Model.BeginUpdate() is called
        /// and decreased when Model.EndUpdate() is called.
        /// </summary>
        [Browsable(false)]
        public Model Model
        {
            get
            {
                return m_model;
            }
            set
            {
                SetNewModel(value);
            }
        }

        /// <summary>
        /// Gets or sets routing mode.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines LineRouting engine routing mode.")]
        [DefaultValue(RoutingMode.Automatic)]
        public RoutingMode RoutingMode
        {
            get { return m_routingMode; }
            set { m_routingMode = value; }
        }

        /// <summary>
        /// Gets or sets the update requests.
        /// </summary>
        /// <value>The update requests.</value>
        protected int UpdateRequests
        {
            get 
            { 
                return m_nUpdateRequests; 
            }
            set
            {
                m_nUpdateRequests = value;
                Model model = this.Model;

                if (m_nUpdateRequests == 0 && model != null && model.LineRoutingEnabled)
                {
                    // pause history manager
                    model.HistoryManager.Pause();

                    if (!m_bInAction)
                    {
                        m_bInAction = true;
                        model.BeginUpdate();
                        model.LinkManager.BeginSynchronization();

                        if (!m_bGenerateBridges)
                        {
                            model.BridgeManager.BeginUpdateIntersection();
                        }

                        // reroute
                        if (this.RoutingMode == RoutingMode.Automatic)
                        {
                            RoutingMode mode = this.RoutingMode;
                           
                            // set line router mode to Automatic for a while to route all connectors
                            m_routingMode = RoutingMode.Automatic;

                            RouteAllModelConnectorsInternal();

                            // restore previous routing mode
                            m_routingMode = mode;
                        }
                        else if (this.RoutingMode == RoutingMode.SemiAutomatic)
                        {
                            RouteConnectorsInternal(this.ConnectorsToReroute);
                            this.ConnectorsToReroute.Clear();
                        }

                        // disable bridge generation flag
                        m_bGenerateBridges = false;

                        model.BridgeManager.EndUpdateIntersection();
                        model.LinkManager.EndSynchronization();
                        model.EndUpdate();
                        m_bInAction = false;

                        // refresh document view
                        // pause history manager
                        model.HistoryManager.Resume();
                        m_nUpdateRequests = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets collection of connectors to route.
        /// </summary>
        private NodeCollection ConnectorsToReroute
        {
            get
            {
                if (m_nodesConnectors == null)
                {
                    m_nodesConnectors = new NodeCollection();
                    m_nodesConnectors.UpdateReferences = false;
                }

                return m_nodesConnectors;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reroutes all model connectors.
        /// </summary>
        public void RouteAllModelConnectors()
        {
            Model model = this.Model;

            if (!m_bInAction && model != null)
            {
                m_bInAction = true;

                if (model.LinkManager.IsSynchronizing)
                    model.LinkManager.EndSynchronization();

                model.BeginUpdate();
                model.BridgeManager.BeginUpdateIntersection();
                model.LinkManager.BeginSynchronization();

                RoutingMode mode = this.RoutingMode;
                
                // set line router mode to Automatic for a while to route all connectors
                m_routingMode = RoutingMode.Automatic;

                RouteAllModelConnectorsInternal();

                // restore previous routing mode
                m_routingMode = mode;

                model.LinkManager.EndSynchronization();
                model.BridgeManager.EndUpdateIntersection();
                model.EndUpdate();
                m_bInAction = false;
            }
        }

        /// <summary>
        /// Route given collection of nodes
        /// </summary>
        /// <param name="connectors">Collections to route</param>
        public void RouteConnectors(ICollection connectors)
        {
            Model model = this.Model;

            if (!m_bInAction && model != null)
            {
                m_bInAction = true;

                model.BeginUpdate();
                model.BridgeManager.BeginUpdateIntersection();
                model.LinkManager.BeginSynchronization();

                RouteConnectorsInternal(connectors);

                model.LinkManager.EndSynchronization();
                model.BridgeManager.EndUpdateIntersection();
                model.EndUpdate();
                m_bInAction = false;
            }
        }

        /// <summary>
        /// Route given connection.
        /// </summary>
        /// <param name="connector">Connection to route.</param>
        public void RouteConnector(ConnectorBase connector)
        {
            Model model = this.Model;

            if (!m_bInAction && model != null)
            {
                m_bInAction = true;
                model.BeginUpdate();
                model.BridgeManager.BeginUpdateIntersection();
                model.LinkManager.BeginSynchronization();

                RouteConnectorInternal(connector);

                model.LinkManager.EndSynchronization();
                model.BridgeManager.EndUpdateIntersection();
                model.EndUpdate();
                m_bInAction = false;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Pauses the line router
        /// </summary>
        internal void Pause()
        {
            m_bInAction = true;
        }

        /// <summary>
        /// Resumes the line router
        /// </summary>
        internal void Resume()
        {
            m_bInAction = false;
        }

        private NodeCollection GetRoutableConnectors(Node sourceNode)
        {
            ICompositeNode compositeNode = sourceNode as ICompositeNode;
            ICollection nodes = sourceNode.Edges;
            NodeCollection edges = new NodeCollection();

            // look throw composite node
            if (compositeNode != null)
            {
                for (int i = 0, length = compositeNode.ChildCount; i < length; i++)
                {
                    edges.AddRange(GetRoutableConnectors(compositeNode.GetChild(i)));
                }
            }

            // filter routable nodes
            foreach (Node node in nodes)
            {
                ConnectorBase conn = node as ConnectorBase;

                if (conn != null && conn.LineRoutingEnabled)
                {
                    edges.Add(conn);
                }
            }

            return edges;
        }

        /// <summary>
        /// Route all connection from model.
        /// </summary>
        protected abstract void RouteAllModelConnectorsInternal();

        /// <summary>
        /// Route collection of connectors.
        /// </summary>
        /// <param name="connectors">Collection of connectors to route.</param>
        protected abstract void RouteConnectorsInternal(ICollection connectors);

        /// <summary>
        /// Route connector.
        /// </summary>
        /// <param name="connector">Connector to route.</param>
        protected abstract void RouteConnectorInternal(ConnectorBase connector);

        /// <summary>
        /// Set new model to the instance.
        /// </summary>
        /// <param name="newValue">The new model.</param>
        protected virtual void SetNewModel(Model newValue)
        {
            if (m_model != null)
            {
                Unsubscribe(m_model);
            }

            m_model = newValue;

            if (m_model != null)
            {
                Subscribe(m_model);
            }
        }

        /// <summary>
        /// Unsubscribe from all events when node could change it location or size.
        /// </summary>
        /// <param name="model">Model to unsubscribe from.</param>
        protected void Unsubscribe(Model model)
        {
            model.EventSink.PinOffsetChanged -= new PinOffsetChangedEventHandler(OnPinOffsetChanged);
            model.EventSink.PinPointChanged -= new PinPointChangedEventHandler(OnPinPointChanged);
            model.EventSink.RotationChanged -= new RotationChangedEventHandler(OnRotationChanged);
            model.EventSink.DocumentBeginUpdate -= new EventHandler(OnDocumentBeginUpdate);
            model.EventSink.DocumentEndUpdate -= new EventHandler(OnDocumentEndUpdate);
            model.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(OnNodeCollectionChanged);
            model.EventSink.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
        }

        /// <summary>
        /// Subscribe to events of the model
        /// </summary>
        /// <param name="model">Model to subscribe to.</param>
        protected void Subscribe(Model model)
        {
            model.EventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(OnPinOffsetChanged);
            model.EventSink.PinPointChanged += new PinPointChangedEventHandler(OnPinPointChanged);
            model.EventSink.RotationChanged += new RotationChangedEventHandler(OnRotationChanged);
            model.EventSink.DocumentBeginUpdate += new EventHandler(OnDocumentBeginUpdate);
            model.EventSink.DocumentEndUpdate += new EventHandler(OnDocumentEndUpdate);
            model.EventSink.NodeCollectionChanged += new CollectionExEventHandler(OnNodeCollectionChanged);
            model.EventSink.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
        }
        #endregion

        #region Event handlers
        [EventHandlerPriorityAttribute(true)]
        private void OnPinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;

            if (this.RoutingMode == RoutingMode.SemiAutomatic)
            {
                // get routable edges
                NodeCollection edges = GetRoutableConnectors(evtArgs.NodeAffected as Node);
                this.ConnectorsToReroute.Clear();
                this.ConnectorsToReroute.AddRange(edges);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnPinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;

            if (this.RoutingMode == RoutingMode.SemiAutomatic)
            {
                // get routable edges
                NodeCollection edges = GetRoutableConnectors(evtArgs.NodeAffected as Node);
                this.ConnectorsToReroute.Clear();
                this.ConnectorsToReroute.AddRange(edges);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnRotationChanged(RotationChangedEventArgs evtArgs)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;

            if (this.RoutingMode == RoutingMode.SemiAutomatic)
            {
                // get routable edges
                NodeCollection edges = GetRoutableConnectors(evtArgs.NodeAffected);
                this.ConnectorsToReroute.Clear();
                this.ConnectorsToReroute.AddRange(edges);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnDocumentBeginUpdate(object sender, EventArgs e)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;

            // begin update bridging - skip bridge generation before recoute connectors
            if (this.Model != null && this.UpdateRequests == 0 && !m_bGenerateBridges)
            {
                this.Model.BridgeManager.BeginUpdateIntersection();
                m_bGenerateBridges = true;
            }

            this.UpdateRequests++;
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnDocumentEndUpdate(object sender, EventArgs e)
        {
            if (this.UpdateRequests == 1 && m_bGenerateBridges)
            {
                this.Model.BridgeManager.EndUpdateIntersection();
                m_bGenerateBridges = false;
            }

            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction)
                return;

            this.UpdateRequests--;
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnPropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;
        }
        [EventHandlerPriorityAttribute(true)]
        private void OnNodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (this.RoutingMode == RoutingMode.Inactive || m_bInAction) return;
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
        /// with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("nDistance", m_nDistance);
            info.AddValue("routingMode", m_routingMode);
        }
        #endregion
    }
}
