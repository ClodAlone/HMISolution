#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A layer is a collection of nodes that share a common set of default properties
    /// and the same Z-order relative to other layers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A layer contains zero or more nodes and is responsible for rendering
    /// those nodes. Since the nodes in a layer are rendered as a group, their
    /// Z-order is the same relative to other layers in the diagram. For example,
    /// if layer A has a higher Z-order than layer B, then all nodes in layer B
    /// will be rendered behind those in layer A. If the Visible flag on a layer
    /// is set to False, none of the nodes in the layer will be rendered.
    /// </para>
    /// <para>
    /// The nodes in the layer can inherit properties from the layer. If a
    /// property is not explicitly set in a node, the node inherits the property
    /// from the layer. If the layer does not have the property set, then the
    /// layer chains up to the model to get the property. This allows all nodes
    /// in a layer to share the same defaults.
    /// </para>
    /// <para>
    /// All nodes in the layer can be hidden by setting the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Layer.Visible"/> flag
    /// to False.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Layer
        : PropertyContainer,
          IServiceProvider
    {
        #region Class members
        /// <summary>
        /// Layer owner
        /// </summary>
        private ILayerContainer m_owner;

        /// <summary>
        /// Layer's NodeCollection
        /// </summary>
        private NodeCollection m_nodesMembers;

        /// <summary>
        /// Layer's alias
        /// </summary>
        private string m_strLayerName;

        /// <summary>
        /// Indicates whether layer is visible
        /// </summary>
        private bool m_bVisible;

        /// <summary>
        /// Indicates whether layer is Enabled
        /// </summary>
        /// <remarks>
        /// When layer is enabled nodes being added to layer's owner NodeCollection
        /// are added to enabled layer NodeCollection also
        /// </remarks>
        private bool m_bEnabled;

        private EventSink m_eventSink;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        public Layer()
            : this(null, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public Layer(Layer src)
            : base(src)
        {
            m_strLayerName = (string)src.m_strLayerName.Clone();
            m_bVisible = src.m_bVisible;
            m_bEnabled = src.m_bEnabled;
            m_owner = src.m_owner;

            if (src.Nodes != null && src.Nodes.Count > 0)
            {
                NodeCollection nodes = new NodeCollection();
                nodes.AddRange(src.Nodes);
                m_nodesMembers = nodes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        /// <param name="owner">Container to attach the layer to.</param>
        /// <param name="strLayerName">Name to give the layer.</param>
        public Layer(ILayerContainer owner, string strLayerName)
            : base()
        {
            m_owner = owner;
            m_strLayerName = strLayerName;
            m_bVisible = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// Serialization constructor for layers.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected Layer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "visible":
                        m_bVisible = info.GetBoolean("visible");
                        break;
                    case "enabled":
                        m_bEnabled = info.GetBoolean("enabled");
                        break;
                    case "layerName":
                        m_strLayerName = info.GetString("layerName");
                        break;
                    case "members":
                        m_nodesMembers = info.GetValue("members", typeof(NodeCollection)) as NodeCollection;
                        break;
                    case "owner":
                        m_owner = info.GetValue("owner", typeof(ILayerContainer)) as ILayerContainer;
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets object that contains the layer.
        /// </summary>
        /// <remarks>
        /// Each layer has a reference to a container object for the purpose of
        /// synchronizing changes in layers. This is necessary because a node
        /// can only belong to a single layer at a time. When a node is added to
        /// one layer, it must be removed from any other layer it might already
        /// belong to.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ILayerContainer"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayerCollection"/>
        /// </remarks>
        [Browsable(false)]
        public ILayerContainer Container
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        /// <summary>
        /// Gets or sets name of the layer.
        /// </summary>
        [Browsable(true)]
        [Description("Unique layer's name.")]
        public string Name
        {
            get 
            { 
                return m_strLayerName; 
            }
            set
            {
                if (m_strLayerName != value && OnPropertyChanging(DPN.Name, value))
                {
                    RecordPropertyChanged(DPN.Name);

                    m_strLayerName = value;

                    OnPropertyChanged(DPN.Name);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the layer is visible.
        /// </summary>
        /// <remarks>
        /// If this flag is set to False, none of the nodes belonging to the layer
        /// will be rendered.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Defines layer's visibility.")]
        public bool Visible
        {
            get 
            { 
                return m_bVisible; 
            }
            set
            {
                if (m_bVisible != value && OnPropertyChanging(DPN.Visible, value))
                {
                    if (this.HistoryService != null)
                        this.HistoryService.StartAtomicAction("Visible Change");

                    RecordPropertyChanged(DPN.Visible);

                    // Set value
                    m_bVisible = value;

                    // Iterate through nodes collection updating their visibility state
                    SetChildrenVisibility(m_bVisible);

                    if (this.HistoryService != null)
                        this.HistoryService.EndAtomicAction();

                    OnPropertyChanged(DPN.Visible);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the layer is enabled or disabled.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("indicates whether nodes can be added to layer.")]
        public bool Enabled
        {
            get 
            { 
                return m_bEnabled; 
            }
            set
            {
                if (m_bEnabled != value && OnPropertyChanging(DPN.Enabled, value))
                {
                    RecordPropertyChanged(DPN.Enabled);

                    m_bEnabled = value;

                    if (this.Container != null)
                    {
                        // if true -- add to ActiveLayers otherwise -- remove
                        if (value && !this.Container.ActiveLayers.Contains(this))
                            this.Container.ActiveLayers.Add(this);
                        else if (!value && this.Container.ActiveLayers.Contains(this))
                            this.Container.ActiveLayers.Remove(this);
                    }

                    OnPropertyChanged(DPN.Enabled);
                }
            }
        }

        /// <summary>
        /// Gets typed collection of nodes belonging to layer
        /// </summary>
        /// <remark>
        /// Clone of layer node's is returned.
        /// Any layer node's manipulation must be done throught layer's
        /// public methods
        /// </remark>
        [Browsable(false)]
        public NodeCollection Nodes
        {
            get
            {
                if (m_nodesMembers == null)
                    m_nodesMembers = new NodeCollection();

                return m_nodesMembers;
            }
        }
        #endregion

        #region IServiceProvider interface
        /// <summary>
        /// Gets the specified type of service object the caller.
        /// </summary>
        /// <param name="svcType">Type of service requested.</param>
        /// <returns>
        /// The object matching the service type requested or NULL if the
        /// service is not supported.
        /// </returns>
        object IServiceProvider.GetService(Type svcType)
        {
            return this.GetService(svcType);
        }
        #endregion

        #region ICollection
        /// <summary>
        /// Gets the number of nodes in the layer.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Count
        {
            get { return this.Nodes.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is thread-safe.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSynchronized
        {
            get { return this.Nodes.IsSynchronized; }
        }

        /// <summary>
        /// Gets the object that can be used to synchronize access to the collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SyncRoot
        {
            get { return this.Nodes.SyncRoot; }
        }

        /// <summary>
        /// Copies the list or a portion of the list to an array.
        /// </summary>
        /// <param name="array">Array in which to copy the items in the list.</param>
        /// <param name="index">Index at which to start copying in target array.</param>
        public void CopyTo(Array array, int index)
        {
            this.Nodes.CopyTo(array, index);
        }

        /// <summary>
        /// Determines if the specified node belongs to the layer.
        /// </summary>
        /// <param name="node">Node to search for.</param>
        /// <returns>
        /// True if the node belongs to the layer; otherwise False.
        /// </returns>
        public bool Contains(Node node)
        {
            return this.Nodes.Contains(node);
        }

        /// <summary>
        /// Adds a node the layer.
        /// </summary>
        /// <param name="node">Node to add.</param>
        /// <remarks>
        /// <para>
        /// If the node already belongs to this layer, this method does nothing.
        /// Before adding the node to this layer, this method 
        /// adds this layer to adding node's LayerCollection
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Layer.Remove"/>
        /// </remarks>
        public void Add(Node node)
        {
            // we can add nodes to layer only if it has owner
            if (m_owner == null)
                throw new NullReferenceException("owner");

            if (!this.Enabled)
                throw new LayerDisabledException(this);

            // 1 - Make sure adding node is not already added and exists in owner's NodeCollection
            if (!this.Nodes.Contains(node)
                && (((ICompositeNode)m_owner).GetChildIndex(node) != -1))
            {
                // append node visibility
                node.Visible = this.Visible;
                
                // 2 - Add layer to adding node's LayerCollection
                node.Layers.Add(this);

                // 2 - Add node to the members collection.
                this.Nodes.Add(node);
            }
        }

        /// <summary>
        /// Adds a collection of nodes to the layer.
        /// </summary>
        /// <param name="nodes">Nodes to add.</param>
        /// <remarks>
        /// <para>
        /// If the node already belongs to this layer, this method does nothing.
        /// Before adding the node to this layer, this method 
        /// adds this layer to adding node's LayerCollection
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Layer.Remove"/>
        /// </remarks>
        public void Add(NodeCollection nodes)
        {
            foreach (Node node in nodes)
            {
                Add(node);
            }
        }

        /// <summary>
        /// Removes the specified node from this layer.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void Remove(Node node)
        {
            // Remove node from layer's NodeCollection
            this.Nodes.Remove(node);
        }

        /// <summary>
        /// Removes all nodes from this layer.
        /// </summary>
        public void RemoveAll()
        {
            // 1 - Iterate through layer's NodeCollection
            // and remove layer from each node's LayerCollection
            foreach (Node node in this.Nodes)
            {
                node.Layers.Remove(this);
            }

            // 2 - Clear layer's NodeCollection
            this.Nodes.Clear();
        }
        #endregion

        #region Implementation Methods
        /// <summary>
        /// Returns the specified type of service object to the caller.
        /// </summary>
        /// <param name="svcType">Type of service requested.</param>
        /// <returns>
        /// The object matching the service type requested or NULL if the
        /// service is not supported.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is similar to COM's IUnknown::QueryInterface method,
        /// although more generic. Instead of just returning interfaces,
        /// this method can return any type of object.
        /// </para>
        /// <para>
        /// The following services are supported:
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>
        /// </para>
        /// </remarks>
        protected virtual object GetService(Type svcType)
        {
            if (svcType == typeof(IZOrderContainer))
                return this;

            return null;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected override void RecordPropertyChanged(string strPropertyName)
        {
            if (this.HistoryService != null)
            {
                this.HistoryService.RecordPropertyChanged(this, this.FullContainerName, strPropertyName);
            }
        }

        /// <summary>
        /// Sets the children visibility.
        /// </summary>
        /// <param name="bVisible">visible, if set to <c>true</c></param>
        private void SetChildrenVisibility(bool bVisible)
        {
            foreach (Node nodeTemp in this.Nodes)
            {
                nodeTemp.Visible = bVisible;
            }
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>The property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            base.UpdateServiceReferences(provider);

            if (provider != null)
            {
                m_eventSink = (DocumentEventSink)provider.ProvideServiceReference(typeof(DocumentEventSink).TypeHandle);
            }
            else
            {
                m_eventSink = null;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new Layer(this);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("visible", m_bVisible);
            info.AddValue("enabled", m_bEnabled);
            info.AddValue("layerName", m_strLayerName);
            info.AddValue("members", this.Nodes);
            info.AddValue("container", m_owner);
        }

        /// <summary>
        /// Called when property changing.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property changing otherwise false.</returns>
        protected override bool OnPropertyChanging(string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;
                string strPropertyFullPath = this.FullContainerName;

                if (strPropertyFullPath != string.Empty)
                {
                    strPropName = strPropertyFullPath + "." + strPropertyName;
                }

                bSuccess = m_eventSink.RaisePropertyChangingEvent(this, strPropName, newValue);
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected override void OnPropertyChanged(string strPropertyName)
        {
            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;
                string strPropertyFullPath = this.FullContainerName;

                if (strPropertyFullPath != string.Empty)
                {
                    strPropName = strPropertyFullPath + "." + strPropertyName;
                }

                m_eventSink.RaisePropertyChangedEvent(this, strPropName);
            }
        }
        #endregion
    }
}