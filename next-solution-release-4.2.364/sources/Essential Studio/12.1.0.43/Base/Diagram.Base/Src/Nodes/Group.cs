#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A group is a node that acts as a transparent container for other nodes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A group is a composite node that controls a set of child nodes. The
    /// bounding rectangle of a group is the union of the bounds of its
    /// children. The group renders itself by iterating through its children
    /// and rendering them.
    /// </para>
    /// <para>
    /// Members of the group are added and removed through the ICompositeNode
    /// interface.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ICompositeNode"/>
    /// </remarks>
    [Serializable]
    public class Group
        : Node,
          ICompositeNode,
          IZOrderContainer
    {
        #region Class members
        private bool m_bCanUngroup;
		private GroupNodePositions m_GroupNodePosition = GroupNodePositions.Relative;
        /// <summary>
        /// Collection of children nodes
        /// </summary>
        protected NodeCollection m_nodesChildren;

        /// <summary>
        /// Indicated whether   group is empty( without children ).
        /// </summary>
        private bool m_bInitialize;
        private LabelCollection m_labels;

        /// <summary>
        /// Cached Group visual representation
        /// </summary>
        protected Image m_bmpCache;

        /// <summary>
        /// Helper field.
        /// Used when node representation is rendered as bitmap.
        /// Used to check whether node's bitmap representation must be regenerated.
        /// </summary>
        private RenderingStyle m_styleRender;

        /// <summary>
        /// Maps node names to node objects.
        /// </summary>
        [DocumentationExclude()]
        private Hashtable m_tableName;

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
        {
            m_bInitialize = true;
            m_bCanUngroup = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="nodesChildren">The nodes children.</param>
        public Group(NodeCollection nodesChildren)
            : this()
        {
            int n;
            AppendChildren(nodesChildren, out n);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public Group(Group src)
            : base(src)
        {
            m_bInitialize = true;
            NodeCollection nodes = (NodeCollection)src.m_nodesChildren.Clone();
            m_nodesChildren = new NodeCollection(this);
            m_nodesChildren.AddRange(nodes);

            bool bLockUpdate = m_bLockUpdate;
            m_bLockUpdate = true;

            UpdateChildrenParents();

            m_bLockUpdate = bLockUpdate;

            m_bCanUngroup = src.m_bCanUngroup;

            if (src.Labels.Count > 0)
            {
                LabelCollection labels = new LabelCollection(this);
                Label labelTemp;

                foreach (Label label in src.Labels)
                {
                    labelTemp = (Label)label.Clone();
                    labelTemp.Container = this;

                    labels.Add(labelTemp);
                }

                m_labels = labels;
            }

            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected Group(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_bInitialize = true;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "children":
                        m_nodesChildren = (NodeCollection)info.GetValue("children", typeof(NodeCollection));
                        break;
                    case "labels":
                        LabelCollection labels = (LabelCollection)info.GetValue("labels", typeof(LabelCollection));

                        // clone control points only if there is more than 1
                        if (labels != null && labels.Count > 0)
                        {
                            foreach (object obj in labels)
                            {
                                Label label = obj as Label;

                                if (label != null)
                                {
                                    label.Container = this;
                                }
                            }
                        }
                        else
                        {
                            labels = new LabelCollection(this);
                        }
                        m_labels = labels;
                        break;
                    case "CanUngroup":
                        m_bCanUngroup = info.GetBoolean("CanUngroup");
                        break;
                }
            }
        }
        #endregion

        #region Class properies
		/// <summary>
        /// Gets or sets the child nodes position.
        /// </summary>
        public GroupNodePositions GroupNodePosition
        {
            get { return m_GroupNodePosition; }
            set
            {
                if (value != m_GroupNodePosition)
                {
                    m_GroupNodePosition = value;
                }
            }
        }
        /// <summary>
        /// Gets the collection of <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> items.
        /// </summary>
        /// <value>
        /// The collection of <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> items.
        /// </value>
        [Browsable(true)]
        [Description("Collection of labels.")]
        public LabelCollection Labels
        {
            get
            {
                if (m_labels == null)
                {
                    m_labels = new LabelCollection(this);
                    m_labels.UpdateServiceReferences(this);
                }

                return m_labels;
            }
        }

        /// <summary>
        /// Gets or sets node's visibility.
        /// </summary>
        /// <value>
        /// Set to <c>true</c> to draw node and handles, otherwise - <c>false</c>.
        /// </value>
        /// <remarks>
        /// This flag can be changed to True only is one of its owner layer are visible.
        /// </remarks>
        public override bool Visible
        {
            get
            {
                bool bSuccess = base.Visible;

                // check for visibility
                if (bSuccess && (this.Nodes == null || this.Nodes.Count == 0))
                    bSuccess = false;

                return bSuccess;
            }
            set
            {
                base.Visible = value;
            }
        }

        /// <summary>
        /// Get's node logical GraphicsPath without scale transformations.
        /// </summary>
        /// <value>The logical graphics path.</value>
        [Browsable(false)]
        protected override GraphicsPath LogicalGraphicsPath
        {
            get
            {
                // create group GraphicsPath
                GraphicsPath pathToReturn = new GraphicsPath(FillMode.Winding);
                
                // create graphics path for clone of child graphics path
                GraphicsPath pathTemp;
                
                // create child matrix
                Matrix matrixTemp;

                foreach (Node nodeCur in this.Nodes)
                {
                    // get cloned child graphics path
                    pathTemp = nodeCur.GraphicsPath;

                    if (!pathTemp.GetBounds().Size.IsEmpty)
                    {
                        // get child transformations
                        matrixTemp = nodeCur.GetTransformations();
                        nodeCur.AppendFlipTransforms(matrixTemp);

                        // append trasformation
                        pathTemp.Transform(matrixTemp);

                        // add to group GraphicsPath
                        pathToReturn.AddPath(pathTemp, false);
                    }
                }

                matrixTemp = GetScaleTransformation();
                matrixTemp.Invert();
                pathToReturn.Transform(matrixTemp);

                return pathToReturn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ports is drawed.
        /// </summary>
        /// <value><c>true</c> if ports is draw; otherwise, <c>false</c>.</value>
        public override bool DrawPorts
        {
            get 
            { 
                return base.DrawPorts; 
            }
            set
            {
                HistoryManager mgrHistory = this.HistoryManager;

                if (mgrHistory != null)
                    mgrHistory.StartAtomicAction("Property Changed");

                foreach (Node node in Nodes)
                {
                    node.DrawPorts = value;
                }

                base.DrawPorts = value;

                if (mgrHistory != null)
                    mgrHistory.EndAtomicAction();
            }
        }

        /// <summary>
        /// Gets the number of child nodes contained by this group.
        /// </summary>
        /// <value></value>
        public int ChildCount
        {
            get { return this.Nodes.Count; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether composite node can be ungrouped.
        /// </summary>
        /// <value></value>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether composite node can be ungrouped")]
        [DefaultValue(true)]
        public bool CanUngroup
        {
            get 
            { 
                return m_bCanUngroup; 
            }
            set
            {
                if (m_bCanUngroup != value && OnPropertyChanging(this.FullContainerName, DPN.CanUngroup, value))
                {
                    RecordPropertyChanged(DPN.CanUngroup);

                    m_bCanUngroup = value;

                    OnPropertyChanged(this.FullContainerName, DPN.CanUngroup);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether update is locked.
        /// </summary>
        /// <value><c>true</c> if update is locked; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool LockUpdate
        {
            get
            {
                return m_bLockUpdate;
            }
            set
            {
                m_bLockUpdate = value;
            }
        }

        /// <summary>
        /// Gets the group children collection.
        /// </summary>
        [Browsable(false)]
        public NodeCollection Nodes
        {
            get
            {
                if (m_nodesChildren == null)
                {
                    m_nodesChildren = new NodeCollection(this);
                    m_nodesChildren.UpdateServiceReferences(this);
                }

                return m_nodesChildren;
            }
        }

        /// <summary>
        /// Gets names hash where key - node name, value - available node index.
        /// Using for generate unique name with indexing.
        /// </summary>
        protected Hashtable NameTable
        {
            get
            {
                if (m_tableName == null)
                    m_tableName = new Hashtable();

                return m_tableName;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new Group(this);
        }

        /// <summary>
        /// Performs given point hit test.
        /// </summary>
        /// <param name="ptTest">Point to test</param>
        /// <returns>true, if contains the point.</returns>
        /// <remarks>
        /// Point must be in pixel units.
        /// </remarks>
        public override bool ContainsPoint(PointF ptTest)
        {
            bool bSuccess = this.Nodes.Count > 0;

            if (bSuccess)
                bSuccess = base.ContainsPoint(ptTest);

            return bSuccess;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates region used for hit testing.
        /// </summary>
        protected override void UpdateHelperRegion()
        {
            if (this.Nodes.Count > 0)
            {
                base.UpdateHelperRegion();
            }
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
            {
                m_mgrLink.BeginSynchronization();
                m_mgrLink.SynchronizeNodeConnections(this);
            }

            base.DoMoveRelatedActions(fX, fY);

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.EndSynchronization();
        }

        /// <summary>
        /// Used to update child nodes sizes.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            // get group children
            NodeCollection nodes = this.Nodes;

            // pause history recording
            SafeHistoryPause();

            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.BeginSynchronization();

            // resize group children to calculated scale factor
            ResizeChildren(nodes, szOldSize, ref szNewSize);

            // resume history recording
            SafeHistoryResume();

            ResetRenderCache();

            // call base to update port position, graphics path and update connections
            base.DoSizeRelatedActions(szOldSize, szNewSize);

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.EndSynchronization();
        }

        /// <summary>
        /// Saves the connections.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Connections list.</returns>
        protected ArrayList SaveConnections(NodeCollection nodes)
        {
            // save port connection before remove node
            ArrayList portConnections = new ArrayList();
            ArrayList containerConnections;
            EndPointConnection connection;
            ICompositeNode container;

            foreach (Node node in nodes)
            {
                container = node as ICompositeNode;

                if (container != null)
                {
                    NodeCollection containerNodes = new NodeCollection();

                    Node nodeTemp;
                    int nLength = container.ChildCount;
                    int nCounter = 0;

                    while (nCounter < nLength)
                    {
                        nodeTemp = container.GetChild(nCounter);
                        containerNodes.Add(nodeTemp);

                        nCounter++;
                    }

                    containerConnections = SaveConnections(containerNodes);

                    if (containerConnections != null && containerConnections.Count > 0)
                        portConnections.AddRange(containerConnections);
                }

                IEndPointContainer endPointContainer = node as IEndPointContainer;
                if (endPointContainer != null)
                {
                    if (endPointContainer.HeadEndPoint.Port != null)
                    {
                        connection = new EndPointConnection();
                        connection.Port = endPointContainer.HeadEndPoint.Port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.Add(endPointContainer.HeadEndPoint);
                        connection.EndPoints = endPoints;
                        portConnections.Add(connection);
                        connection.Port.DisconnectAll();
                    }

                    if (endPointContainer.TailEndPoint.Port != null)
                    {
                        connection = new EndPointConnection();
                        connection.Port = endPointContainer.TailEndPoint.Port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.Add(endPointContainer.TailEndPoint);
                        connection.EndPoints = endPoints;
                        portConnections.Add(connection);
                        connection.Port.DisconnectAll();
                    }
                }

                foreach (ConnectionPoint port in node.Ports)
                {
                    if (port.Connections.Count > 0)
                    {
                        connection = new EndPointConnection();
                        connection.Port = port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.AddRange(port.Connections);
                        connection.EndPoints = endPoints;

                        portConnections.Add(connection);
                        port.DisconnectAll();
                    }
                }
            }

            return portConnections;
        }

        /// <summary>
        /// Restores the connections.
        /// </summary>
        /// <param name="portConnections">The port connections.</param>
        protected void RestoreConnections(ArrayList portConnections)
        {
            // reconnect disconnecter connections
            foreach (EndPointConnection portConnection in portConnections)
            {
                portConnection.Port.Connections.QuietMode = true;
                foreach (EndPoint endPoint in portConnection.EndPoints)
                {
                    portConnection.Port.Connect(endPoint);
                }
                portConnection.Port.Connections.QuietMode = false;
            }
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            if (m_eventSink != null && provider != null)
            {
                m_eventSink.NodeCollectionChanging += new CollectionExEventHandler(EventSink_NodeCollectionChanging);
                m_eventSink.NodeCollectionChanged += new CollectionExEventHandler(EventSink_NodeCollectionChanged);
            }
            else if (m_eventSink != null && provider == null)
            {
                m_eventSink.NodeCollectionChanging -= new CollectionExEventHandler(EventSink_NodeCollectionChanging);
                m_eventSink.NodeCollectionChanged -= new CollectionExEventHandler(EventSink_NodeCollectionChanged);
            }

            base.UpdateReferences(provider);

            this.Nodes.UpdateServiceReferences(provider);

            if (!this.BoundsInfo.IsResizing)
            {
                foreach (Node node in this.Nodes)
                {
                    node.UpdateServiceReferences(provider);
                }
            }

            this.Labels.UpdateServiceReferences(provider);

            foreach (Label label in this.Labels)
            {
                label.UpdateServiceReferences(provider);
                label.Container = this;
            }
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("children", this.Nodes);
            info.AddValue("labels", m_labels);
            info.AddValue("CanUngroup", m_bCanUngroup);
        }

        /// <summary>
        /// Renders shapes visual representation.
        /// on given graphics
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            base.Render(gfx);

            RenderChildren(gfx);
        }

        /// <summary>
        /// Renders the children.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void RenderChildren(Graphics gfx)
        {
            // TryEnhanceRendering( gfx );

            // render cached image only with 100% zoom
            if (m_bmpCache != null && gfx.PageScale == 1f)
            {
                RenderCachedImage(gfx);
            }
            else
            {
                foreach (Node nodeCur in this.Nodes)
                {
                    nodeCur.Draw(gfx);
                }
            }
        }

        /// <summary>
        /// Called when parent changed.
        /// </summary>
        protected override void OnParentChanged()
        {
            bool bLock = m_bLockUpdate;
            m_bLockUpdate = true;

            // update children's parent
            foreach (Node node in this.Nodes)
            {
                node.Parent = this;
            }

            m_bLockUpdate = bLock;
        }

        /// <summary>
        /// methods used to draw contiguous date. Such as labels or ports
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void RenderContiguousData(Graphics gfx)
        {
            foreach (Label label in this.Labels)
            {
                label.Draw(gfx);
            }

            base.RenderContiguousData(gfx);
        }

        /// <summary>
        /// Called when node deserialized.
        /// </summary>
        protected override void OnDeserialized()
        {
            
            base.OnDeserialized();
            
            NodeCollection nodesNew = new NodeCollection(this);

            bool bLockUpdate = m_bLockUpdate;
            m_bLockUpdate = true;

            // update children service references
            foreach (object curChild in this.Nodes)
            {
                Node node = curChild as Node;

                if (node != null)
                {
                    node.Parent = this;
                    node.UpdateServiceReferences(this);
                    ((IDeserializationCallback)node).OnDeserialization(null);
                     nodesNew.Add(node);
                 
                 }
            }

            m_bLockUpdate = bLockUpdate;
            //UpdateCompositeBounds();

            // update collection referances 
            // after update child references
            nodesNew.UpdateServiceReferences(this);

            // swap new collection
            m_nodesChildren = nodesNew;

            foreach (Label label in this.Labels)
            {
                label.Container = this;
            }

        }

        /// <summary>
        /// Accumulates the refresh rect.
        /// </summary>
        /// <param name="rcRefresh">The refresh rectangle.</param>
        protected override void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            rcRefresh = RectangleF.Empty;
            RectangleF rcTmp;

            foreach (Node node in this.Nodes)
            {
                rcTmp = node.RefreshRect;

                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcTmp;
                }
                else if (!rcTmp.Size.IsEmpty)
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcTmp);
                }
            }
            
            // LABELS
            // label location
            PointF ptLLoc;
            
            // label's bounding rect
            RectangleF rcLBounds = RectangleF.Empty;

            // current label's bounding rect
            foreach (Label label in this.Labels)
            {
                if (label == null) continue;

                Font fntTemp = label.FontStyle.CreateFont();
                //// calc label rect
                rcLBounds.Size = label.Size;
                //// get label location
                ptLLoc = label.GetPosition();
                //// update label location
                rcLBounds.Location = new PointF(ptLLoc.X - rcLBounds.Width / 2, ptLLoc.Y - rcLBounds.Height / 2);

                // merge port bounding rect with current refresh rect
                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcLBounds;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcLBounds);
                }
            }

            // call base to consider shadown and ports
            base.AccumulateRefreshRect(ref rcRefresh);
        }

        /// <summary>
        /// Methods used to get node's GraphicsPath
        /// </summary>
        /// <returns>node's GraphicsPath</returns>
        /// <remarks>
        /// Used primarily for Group node to get
        /// all children paths union with transformations.
        /// </remarks>
        protected override RectangleF GetPathBounds()
        {
            RectangleF rcToReturn = RectangleF.Empty;
            
            // current node's bounding rect(parent relative)
            RectangleF rcNBounds;

            foreach (Node nodeCur in this.Nodes)
            {
                // get node bounding rect
                rcNBounds = GetBoundsRect(nodeCur);

                // merge current node's bounding rect with current rect
                if (rcToReturn.Size.IsEmpty)
                {
                    rcToReturn = rcNBounds;
                }
                else
                {
                    rcToReturn = RectangleF.Union(rcToReturn, rcNBounds);
                }
            }

            return rcToReturn;
        }

        /// <summary>
        /// Raise when group property changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void EventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            base.EventSink_PropertyChanged(evtArgs);
            if (evtArgs.PropertyName == this.BoundsInfo.FullContainerName + "." + DPN.IsResizing)
            {
                foreach (Node node in this.Nodes)
                    node.BoundsInfo.IsResizing = this.BoundsInfo.IsResizing;
            }
        }
        #endregion

        #region Class helper methods
        private void ResizeChildren(NodeCollection nodes, SizeF szOldSize, ref SizeF szNewSize)
        {
            if (szOldSize == szNewSize)
                return;

            Group parent = this.Parent as Group;
            bool bParentLock = false;

            if (parent != null)
            {
                bParentLock = parent.LockUpdate;
                parent.LockUpdate = true;
            }

            // check if need to affect aspect ratio
            bool bAspectRatio = !this.EditStyle.AspectRatio && !HandlesHitTesting.CanResizeChildren(this);

            // create scale matrix
            Matrix mtxScale = new Matrix();
            RectangleF rcNewBounds = RectangleF.Empty;
            bool bFirst = true;

            if (bAspectRatio)
            {
                // calc group scale factor
                float fFactor = (Math.Round(szNewSize.Width, 0) > Math.Round(szOldSize.Width, 0)
                    || Math.Round(szNewSize.Height, 0) > Math.Round(szOldSize.Height, 0))
                    ? Math.Max(szNewSize.Width / szOldSize.Width, szNewSize.Height / szOldSize.Height)
                    : Math.Min(szNewSize.Width / szOldSize.Width, szNewSize.Height / szOldSize.Height);

                mtxScale.Scale(fFactor, fFactor);
            }
            else
            {
                // calc group scale factor
                float fGroupScaleFactorX = (szNewSize.Width != 0) ? szNewSize.Width / szOldSize.Width : 1;
                float fGroupScaleFactorY = (szNewSize.Height != 0) ? szNewSize.Height / szOldSize.Height : 1;

                mtxScale.Scale(fGroupScaleFactorX, fGroupScaleFactorY);
            }

            foreach (Node nodeCur in nodes)
            {
                UpdateChildBoundsInfo(nodeCur, mtxScale);

                RectangleF rcBounds = ((IUnitIndependent)nodeCur).GetBoundingRectangle(MeasureUnits.Pixel, false);
                rcNewBounds = bFirst ? rcBounds : RectangleF.Union(rcNewBounds, rcBounds);
                if (nodeCur.RotationAngle != 0)
                    nodeCur.Translate(-rcNewBounds.X, -rcNewBounds.Y, MeasureUnits.Pixel);
                bFirst = false;
            }
            if (!this.BoundsInfo.IsResizing && szNewSize != rcNewBounds.Size)
            {
                m_bLockUpdate = false;
                UpdateCompositeBounds();
                m_bLockUpdate = true;
            }
            // update bounds if need
            if (bAspectRatio)
            {
                // get graphics path bounds
                RectangleF rcBounds = rcNewBounds;

                if (!Geometry.EqualPoints(szNewSize.ToPointF(), rcBounds.Size.ToPointF(), 2))
                {
                    this.BoundsInfo.SetSize(rcBounds.Size, MeasureUnits.Pixel);
                    szNewSize = rcBounds.Size;
                }
            }

            if (parent != null)
            {
                parent.LockUpdate = bParentLock;
            }
        }

        /// <summary>
        /// Updates the children parents.
        /// </summary>
        protected virtual void UpdateChildrenParents()
        {
            // update parent children
            foreach (Node child in m_nodesChildren)
            {
                child.Parent = this;
            }
        }

        #region Transformations
        /// <summary>
        /// Gets the bounding rect in local coordinates.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The bounding rect.</returns>
        protected virtual RectangleF GetBoundingRect(NodeCollection nodes)
        {
            int nLength = nodes.Count;
            RectangleF rcBoundsToReturn = RectangleF.Empty;

            if (nLength > 0)
            {
                RectangleF rcBounds;
                if (this.GroupNodePosition == GroupNodePositions.Relative)
                {
                    rcBoundsToReturn = ((IUnitIndependent)nodes[0]).GetBoundingRectangle(MeasureUnits.Pixel, false);
                    for (int i = 1; i < nLength; i++)
                    {
                        rcBounds = ((IUnitIndependent)nodes[i]).GetBoundingRectangle(MeasureUnits.Pixel, false);
                        rcBoundsToReturn = RectangleF.Union(rcBoundsToReturn, rcBounds);
                    }
                }
                else
                {
                    for (int i = 0; i < nLength; i++)
                    {
                        rcBounds = ((IUnitIndependent)nodes[i]).GetBoundingRectangle(MeasureUnits.Pixel, false);
                        rcBoundsToReturn = RectangleF.Union(rcBoundsToReturn, rcBounds);
                    }
                }
            }

            return rcBoundsToReturn;
        }

        /// <summary>
        /// Updates the child bounds info.
        /// </summary>
        /// <param name="nodeCur">The node cur.</param>
        /// <param name="mtxScale">The matrix scale.</param>
        private void UpdateChildBoundsInfo(Node nodeCur, Matrix mtxScale)
        {
            // get node pin and size position
            PointF ptPinPoint = ((IUnitIndependent)nodeCur).GetPinPoint(MeasureUnits.Pixel);
            SizeF szSize = ((IUnitIndependent)nodeCur).GetSize(MeasureUnits.Pixel);

            // append scale transformation
            ptPinPoint = Geometry.AppendMatrix(ptPinPoint, mtxScale);

            // append rotation transformation
            Matrix mtxTmp = new Matrix();
            mtxTmp.Rotate(nodeCur.RotationAngle);
            PointF[] pts = new PointF[2];
            pts[0] = new PointF(szSize.Width, 0);
            pts[1] = new PointF(0, szSize.Height);

            //// scale
            mtxScale.TransformPoints(pts);
            // rotate
            mtxTmp.TransformPoints(pts);            
            //// revert rotation
            mtxTmp.Invert();
            mtxTmp.TransformPoints(pts);

            // update node's size
            SizeF szNew = SizeF.Empty;
            szNew.Width = szSize.Width + ((float)Geometry.PointDistance(PointF.Empty, pts[0]) - szSize.Width);
            szNew.Height = szSize.Height + ((float)Geometry.PointDistance(PointF.Empty, pts[1]) - szSize.Height);

            // set new pin and size value
            UpdatePinAndSize(nodeCur, ptPinPoint, szNew);
        }

        /// <summary>
        /// Updates the size and pin position.
        /// </summary>
        /// <param name="nodeCur">Node to change PinPoint and Size to.</param>
        /// <param name="ptPinPoint">The pin point.</param>
        /// <param name="szSize">The node size.</param>
        private void UpdatePinAndSize(Node nodeCur, PointF ptPinPoint, SizeF szSize)
        {
            Model model = this.Root;
            bool bBoundaryConstains = false;

            // Quite disable boudary constraints
            if (model != null)
            {
                bBoundaryConstains = model.BoundaryConstraintsEnabled;
                QuiteBoundarySet(false);
            }

            ((IUnitIndependent)nodeCur).SetSize(szSize, MeasureUnits.Pixel);
            ((IUnitIndependent)nodeCur).SetPinPoint(ptPinPoint, MeasureUnits.Pixel);

            // Quite restore boundary constrains value
            if (model != null)
                QuiteBoundarySet(bBoundaryConstains);
        }

        /// <summary>
        /// Update group pin position and size to children,
        /// without visual transformation and history recording.
        /// </summary>
        protected virtual void UpdateGroupInfo()
        {
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;

                // pause history
                SafeHistoryPause();

                EditStyle groupEditStyle = QuiteResetEditStyle(this);
                
                // get group children
                NodeCollection children = this.Nodes;

                // get children bounds in local coordinates
                RectangleF rcNewBounding = GetBoundingRect(children);

                // update pin position, pin offset anf size

                UpdateBoundsInfo(rcNewBounding);

                EditStyle editStyle;
                
                // iterate through child nodes updating their pin positions
                foreach (Node nodeCur in children)
                {
                    editStyle = QuiteResetEditStyle(nodeCur);

                    // translate node to group origin
                    nodeCur.Translate(-rcNewBounding.X, -rcNewBounding.Y, MeasureUnits.Pixel);

                    QuiteRestoreEditStyle(nodeCur, editStyle);
                }

                QuiteRestoreEditStyle(this, groupEditStyle);

                // resume history
                SafeHistoryResume();

                m_bLockUpdate = false;
                
            }

            // UpdateHelperRegion();
        }

        /// <summary>
        /// Update group pin position, pin offset and size.
        /// </summary>
        /// <param name="rcBounds">Nodes bounds in group coordinates,</param>
        protected virtual void UpdateBoundsInfo(RectangleF rcBounds)
        {
            SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);
            SizeF szSize = this.BoundsInfo.GetSize(MeasureUnits.Pixel);
            float fAngle = this.RotationAngle;

            // get old Upper left point of group
            PointF ptUpperLeft = GetUpperLeftPoint(MeasureUnits.Pixel);
            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            rcBounds.Offset(ptUpperLeft);

            // calc pin offset
            SizeF szPinOffsetUnitIndependent;

            // if group is empty set pin point on center by default.
            if (m_bInitialize)
            {
                szPinOffsetUnitIndependent = new SizeF(rcBounds.Width / 2, rcBounds.Height / 2);
                m_bInitialize = false;
            }
            else
            {
                // calc pin offset by offset factor.
                // 0.5 - coef, to set pin point in center.
                float fWidthFactor = (szSize.Width != 0) ? szPinOffset.Width / szSize.Width : 0.5f;
                float fHeightFactor = (szSize.Height != 0) ? szPinOffset.Height / szSize.Height : 0.5f;

                // calc pin offset
                szPinOffsetUnitIndependent = new SizeF(rcBounds.Width * fWidthFactor, rcBounds.Height * fHeightFactor);
            }

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(rcBounds.X + szPinOffsetUnitIndependent.Width, rcBounds.Y + szPinOffsetUnitIndependent.Height);

            // rotate new pin point around old pin
            PointF[] pts = new PointF[] { ptPinPointUnitIndependent };

            Matrix mtxTransform = new Matrix();
            mtxTransform.RotateAt(fAngle, ptPinPoint, MatrixOrder.Append);
            AppendFlipTransforms(mtxTransform, ptPinPoint, this.FlipX, this.FlipY);
            mtxTransform.TransformPoints(pts);

            ptPinPointUnitIndependent = pts[0];

            // assign node size value
            SizeF szSizeUnitIndependent = rcBounds.Size;

            // Update BoundsInfo
            this.BoundsInfo.SetSize(szSizeUnitIndependent, MeasureUnits.Pixel);
            this.BoundsInfo.SetPinOffset(szPinOffsetUnitIndependent, MeasureUnits.Pixel);
            this.BoundsInfo.SetPinPoint(ptPinPointUnitIndependent, MeasureUnits.Pixel);

            if (!m_bLockUpdate)
            {
                // this.Root.BridgeManager.AddToIntersectCollection( this );
            }
        }
        #endregion

        /// <summary>
        /// Quites the reset move locking.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The edit style.</returns>
        protected EditStyle QuiteResetEditStyle(Node node)
        {
            EditStyle styleToReturn = (EditStyle)node.EditStyle.Clone();
            Model model = this.Root;

            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
            }

            node.EditStyle.AllowMoveX = true;
            node.EditStyle.AllowMoveY = true;
            node.EditStyle.AllowDelete = true;
            node.EditStyle.AllowRotate = true;
            node.EditStyle.AllowChangeWidth = true;
            node.EditStyle.AllowChangeHeight = true;

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }

            return styleToReturn;
        }

        /// <summary>
        /// Quites the restore move locking.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="editStyle">The edit style.</param>
        protected void QuiteRestoreEditStyle(Node node, EditStyle editStyle)
        {
            Model model = this.Root;

            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
            }

            node.EditStyle.AllowMoveX = editStyle.AllowMoveX;
            node.EditStyle.AllowMoveY = editStyle.AllowMoveY;
            node.EditStyle.AllowDelete = editStyle.AllowDelete;
            node.EditStyle.AllowRotate = editStyle.AllowRotate;
            node.EditStyle.AllowChangeWidth = editStyle.AllowChangeWidth;
            node.EditStyle.AllowChangeHeight = editStyle.AllowChangeHeight;

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }

        /// <summary>
        /// Determines whether this instance can append child the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="nodeAppending">The node appending.</param>
        /// <returns>
        /// <c>true</c> if this instance can append child the specified parent; otherwise, <c>false</c>.
        /// </returns>
        public bool CanAppendChild(ICompositeNode parent, Node nodeAppending)
        {
            bool bSuccess = true;
            ICompositeNode nodeComposite;

            for (int i = 0; i < parent.ChildCount; i++)
            {
                Node node = parent.GetChild(i);
                nodeComposite = node as ICompositeNode;

                if (nodeComposite != null)
                {
                    bSuccess = CanAppendChild(nodeComposite, nodeAppending);
                }
                else
                {
                    if (node.Equals(nodeAppending))
                    {
                        bSuccess = false;
                        break;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Renders the cached image.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void RenderCachedImage(Graphics gfx)
        {
            float fX = this.RefreshRect.X - this.BoundingRect.X;
            float fY = this.RefreshRect.Y - this.BoundingRect.Y;
            RectangleF rect = new RectangleF(2 * fX, 2 * fY, this.BoundingRect.Width - 4 * fX, this.BoundingRect.Height - 4 * fY);
            gfx.DrawImage(m_bmpCache, rect);
        }

        /// <summary>
        /// Resets the render cache.
        /// </summary>
        protected void ResetRenderCache()
        {
            if (m_bmpCache != null)
            {
                m_bmpCache.Dispose();
                m_bmpCache = null;
            }
        }

        /// <summary>
        /// If there are more than 7 nodes to render during MoveTool rendering
        /// render moveing nodes to bitmap in order to enhance MoveTool rendering
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        private void TryEnhanceRendering(Graphics gfx)
        {
            // if nodes bounding rectangle exceeds max Bitmap size - skip caching
            RectangleF rcBounds = new HandleRenderer().GetBoundingRect(m_nodesChildren);
            SizeF szMaximum = CommonUsedValues.MAX_IMAGE_SIZE;

            if (rcBounds.Width > szMaximum.Width || rcBounds.Height > szMaximum.Height) return;

            if (m_styleRender == null)
                m_styleRender = new RenderingStyle();

            if (m_bmpCache == null || (m_styleRender.SmoothingMode != gfx.SmoothingMode || m_styleRender.TextRenderingHint
                != gfx.TextRenderingHint || m_styleRender.InterpolationMode != gfx.InterpolationMode))
            {
                int nCounter = 0;
                ICompositeNode nodeComposite;
                
                // count moving nodes
                foreach (Node node in m_nodesChildren)
                {
                    nodeComposite = node as ICompositeNode;

                    if (nodeComposite != null)
                    {
                        nCounter += nodeComposite.ChildCount;
                    }

                    nCounter++;

                    if (nCounter > 6)
                    {
                        // remember graphics settings
                        m_styleRender.SmoothingMode = gfx.SmoothingMode;
                        m_styleRender.TextRenderingHint = gfx.TextRenderingHint;
                        m_styleRender.InterpolationMode = gfx.InterpolationMode;

                        m_bmpCache = RenderingHelper.RenderToImage(this, m_styleRender);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Called when Z order changing.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The new Z order.</param>
        /// <returns>true, if Z order is changing.</returns>
        protected virtual bool OnZOrderChanging(Node node, ZOrderUpdate changeType, int nNewZOrder)
        {
            ZOrderChangingEventArgs evtArgs = new ZOrderChangingEventArgs(node, changeType, nNewZOrder);

            if (this.EventSink != null)
            {
                this.EventSink.RaiseZOrderChanging(evtArgs);
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Called when Z order changed.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The n new Z order.</param>
        protected virtual void OnZOrderChanged(Node node, ZOrderUpdate changeType, int nNewZOrder)
        {
            ZOrderChangedEventArgs evtArgs = new ZOrderChangedEventArgs(node, changeType, nNewZOrder);

            if (this.EventSink != null)
            {
                this.EventSink.RaiseZOrderChanged(evtArgs);
            }
        }

        /// <summary>
        /// Records the Z order changed.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        protected void RecordZOrderChanged(Node node, ZOrderUpdate changeType)
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordZorderChanged(node, changeType);
            }
        }
        private bool IsBridgable(Node node)
        {
            return (node is ConnectorBase);
        }

        /// <summary>
        /// Update bridges given node connectors.
        /// </summary>
        /// <param name="node">The node.</param>
        private void UpdateBridges(Node node)
        {
            ConnectorBase lineNode = node as ConnectorBase;
            Group group = node as Group;

            // Update bridges for current line connector
            if (lineNode != null && m_mgrBridge != null)
            {
                m_mgrBridge.AddToIntersectCollection(lineNode);
            }
            else if (group != null)
            {
                // group.UpdateConnectorsIntersecting();
            }
        }

        /// <summary>
        /// Raise when node collection changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        private void EventSink_NodeCollectionChanging(CollectionExEventArgs evtArgs)
        {
            // check if this instance is  collection owner
            if (!this.Equals(evtArgs.Owner))
                return;

            if (evtArgs.ChangeType == CollectionExChangeType.Insert ||
                evtArgs.ChangeType == CollectionExChangeType.Set)
            {
                EditStyle editStyle;

                if (this.GroupNodePosition == GroupNodePositions.Absolute)
                {
                    foreach (Node element in evtArgs.Elements)
                    {
                        editStyle = QuiteResetEditStyle(element);
                        HandlesHitTesting.ConvertToParentCoordinates(element, this);
                        QuiteRestoreEditStyle(element, editStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Raise when node collection changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        private void EventSink_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (this.Equals(evtArgs.Element))
            {
                GenerateChildrenUniqueNames();
            }

            // check if this instance is  collection owner
            if (!this.Equals(evtArgs.Owner))
                return;

            if (evtArgs.ChangeType == CollectionExChangeType.Remove ||
                evtArgs.ChangeType == CollectionExChangeType.Clear)
            {
                EditStyle editStyle;
                if (this.LockUpdate)
                {
                    foreach (Node element in evtArgs.Elements)
                    {
                        editStyle = QuiteResetEditStyle(element);
                        HandlesHitTesting.ConvertToModelCoordinates(element, this);
                        QuiteRestoreEditStyle(element, editStyle);
                    }
                }
            }

            // update bounds
            UpdateCompositeBounds();
        }

        /// <summary>
        /// Generate unique names for all children.
        /// </summary>
        private void GenerateChildrenUniqueNames()
        {
            if (this.NameTable.Count > 0)
                return;

            foreach (Node child in this.Nodes)
            {
                GenerateUniqueName(child, HandlesHitTesting.NameIndex);
            }
        }

        /// <summary>
        /// Generate the unique name for node.
        /// </summary>
        /// <param name="curNode">The current node.</param>
        /// <param name="regex">The regular expression.</param>
        private void GenerateUniqueName(Node curNode, Regex regex)
        {
            if (curNode.Parent != this)
                return;

            // Generate unique name for node.
            string nodeName;

            if (HandlesHitTesting.GenerateUniqueNodeName(this.NameTable, curNode, out nodeName, regex))
            {
                curNode.Name = nodeName;
            }

            // Register the name in model name table.
            HandlesHitTesting.RegisterNode(this.NameTable, curNode, regex);
        }
        #endregion

        #region ICompositeNode Members
        /// <summary>
        /// Returns the child node at the given index position.
        /// </summary>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        /// <returns>
        /// Child node at the given position or NULL if the index is out of range.
        /// </returns>
        public Node GetChild(int childIndex)
        {
            if ((0 > childIndex) || (childIndex > this.Nodes.Count - 1))
                throw new ArgumentOutOfRangeException("childindex");

            return this.Nodes[childIndex];
        }

        /// <summary>
        /// Returns the child node matching the given name.
        /// </summary>
        /// <param name="childName">Name of node to return.</param>
        /// <returns>Node matching the given name.</returns>
        public Node GetChildByName(string childName)
        {
            return this.Nodes[childName];
        }

        /// <summary>
        /// Returns the index position of the given child node.
        /// </summary>
        /// <param name="child">Child node to query.</param>
        /// <returns>
        /// Zero-based index into the collection of child nodes.
        /// </returns>
        public int GetChildIndex(Node child)
        {
            return this.Nodes.IndexOf(child);
        }

        /// <summary>
        /// Appends the given node to the collection of child nodes.
        /// </summary>
        /// <param name="child">Node to append.</param>
        /// <returns>
        /// Zero-based index at which the node was added to the collection or -1 for failure.
        /// </returns>
        public int AppendChild(Node child)
        {
            int nIndexToReturn = -1;

            // add node only if is not contained in one of group children.
            if (CanAppendChild(this, child))
            {
                child.Parent = this;

                GenerateUniqueName(child, HandlesHitTesting.NameIndex);

                // add node to group
                nIndexToReturn = this.Nodes.Add(child);

                // if root is null call update
                UpdateCompositeBounds();
            }

            return nIndexToReturn;
        }

        /// <summary>
        /// Appends the given collection of nodes as child nodes.
        /// </summary>
        /// <param name="children">Nodes to append.</param>
        /// <param name="startIdx">Zero-based index at which the first node was added to the collection of child nodes.</param>
        /// <returns>Number of child nodes appended.</returns>
        public int AppendChildren(NodeCollection children, out int startIdx)
        {
            startIdx = this.Nodes.Count;

            bool bLock = m_bLockUpdate;
            m_bLockUpdate = true;

            // add nodes
            this.Nodes.AddRange(children);

            m_bLockUpdate = bLock;

            // update group
            UpdateCompositeBounds();

            return children.Count;
        }

        /// <summary>
        /// Insert the given node in local coordinates into the collection of child nodes at a
        /// specific position.
        /// </summary>
        /// <param name="child">Node to insert.</param>
        /// <param name="childIndex">Zero-based index at which to insert the node.</param>
        public void InsertChild(Node child, int childIndex)
        {
            child.Parent = this;

            // insert node
            this.Nodes.Insert(childIndex, child);

            GenerateUniqueName(child, HandlesHitTesting.NameIndex);

            // if root is null call update
            if (this.Root == null)
                UpdateCompositeBounds();
        }

        /// <summary>
        /// Removes the child node at the given position.
        /// </summary>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        /// <returns>
        /// True if the node was successfully removed; otherwise False.
        /// </returns>
        public bool RemoveChild(int childIndex)
        {
            // remove node
            Node nodeToRemove = this.Nodes[childIndex];
            if (nodeToRemove != null && EditStyle.CanDelete(nodeToRemove))
            {
                this.Nodes.RemoveAt(childIndex);

                if (this.Nodes.Count == 0 && this.Parent != null)
                {
                    this.Parent.RemoveChild(this);
                }

                HandlesHitTesting.UnregisterNode(this.NameTable, nodeToRemove, HandlesHitTesting.NameIndex);

                // if root is null call update
                if (this.Root == null)
                    UpdateCompositeBounds();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="nodeToRemove">The node to remove.</param>
        /// <returns>true, if remove child.</returns>
        public bool RemoveChild(Node nodeToRemove)
        {
            // remove node from collection
            if (nodeToRemove != null && EditStyle.CanDelete(nodeToRemove))
            {
                this.Nodes.Remove(nodeToRemove);

                if (this.Nodes.Count == 0 && this.Parent != null)
                {
                    this.Parent.RemoveChild(this);
                }

                HandlesHitTesting.UnregisterNode(this.NameTable, nodeToRemove, HandlesHitTesting.NameIndex);

                // if root is null call update
                if (this.Root == null)
                    UpdateCompositeBounds();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all child nodes from the node.
        /// </summary>
        public void RemoveAllChildren()
        {
            this.Nodes.Clear();

            // reset render cache
            ResetRenderCache();

            this.NameTable.Clear();

            if (this.Parent != null)
                this.Parent.RemoveChild(this);
        }

        /// <summary>
        /// Tests to see if the given node falls within the constraining region
        /// of the composite node.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>
        /// True if node falls within the constraining region; False if it does
        /// not.
        /// </returns>
        public bool CheckConstrainingRegion(Node node)
        {
            return false;
        }

        /// <summary>
        /// Returns all children that are intersected by the given point.
        /// </summary>
        /// <param name="childNodes">Collection in which to add the children hit by the given point.</param>
        /// <param name="ptModel">Point to test.</param>
        /// <returns>
        /// The number of child nodes that intersect the given point.
        /// </returns>
        public int GetChildrenAtPoint(NodeCollection childNodes, PointF ptModel)
        {
            int nNodeIndex = -1;

            PointF[] ptUpperLeftPoint = new PointF[] { GetUpperLeftPoint(MeasureUnits.Pixel) };

            Matrix mtxParentTransform = GetParentTransformation(false);
            mtxParentTransform.TransformPoints(ptUpperLeftPoint);

            PointF[] pts = new PointF[] { ptModel };

            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(this, true);
            mtxTemp.Invert();
            mtxTemp.TransformPoints(pts);

            foreach (Node node in this.Nodes)
            {
                if (node.ContainsPoint(pts[0]))
                {
                    childNodes.Add(node);
                    nNodeIndex = this.Nodes.IndexOf(node);
                }
            }

            return nNodeIndex;
        }

        /// <summary>
        /// Returns all children that intersect the given rectangle.
        /// </summary>
        /// <param name="childNodes">Collection in which to add the children hit by the given point.</param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>
        /// The number of child nodes that intersect the given rectangle.
        /// </returns>
        public int GetChildrenIntersecting(NodeCollection childNodes, RectangleF rcModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all children inside the given rectangle.
        /// </summary>
        /// <param name="childNodes">Collection in which to add the children inside the specified rectangle.</param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>
        /// The number of child nodes added to the collection.
        /// </returns>
        public int GetChildrenContainedBy(NodeCollection childNodes, RectangleF rcModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update bounds size to content size.
        /// </summary>
        public void UpdateCompositeBounds()
        {
            if (!m_bLockUpdate)
            {
                // update group to children
                UpdateGroupInfo();

                if (m_mgrLink != null)
                    m_mgrLink.SynchronizeNodeConnections(this);

                // update parent bounds
                UpdateContainerBounds();

                // update region helper
                m_rgnCache = null;

                ResetRenderCache();

                // update node bounding rect
                UpdateBoundingRectangle();
            }

            GenerateChildrenUniqueNames();
        }
        #endregion

        #region IZOrderContainer interface
        /// <summary>
        /// Gets number of items in the Z-order for this container.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ZOrderDepth
        {
            get { return this.Nodes.Count; }
        }

        /// <summary>
        /// Returns the Z-order value of the given node.
        /// </summary>
        /// <param name="node">Node to get Z-order for.</param>
        /// <returns>
        /// Zero-based Z-order value of the node or -1 if the node
        /// does not exist.
        /// </returns>
        public int GetZOrder(Node node)
        {
            return this.Nodes.IndexOf(node);
        }

        /// <summary>
        /// Sets the Z-order of the given node.
        /// </summary>
        /// <param name="node">Node to set Z-order for.</param>
        /// <param name="zOrder">Zero-based Z-order value.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SetZOrder(Node node, int zOrder)
        {
            // Cache ZDepth
            int nZDepth = this.Nodes.Count;

            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;
            ZOrderUpdate changeType = ZOrderUpdate.Set;

            if ((nZDepth > zOrder) && (zOrder >= 0) && OnZOrderChanging(node, changeType, zOrder))
            {
                // ensure thread safety
                lock (this.Nodes)
                {
                    //// make history record
                    RecordZOrderChanged(node, changeType);
                    //// 1 - disable events raising on collection changing
                    bool bCurrentMode = this.Nodes.QuietMode;
                    this.Nodes.QuietMode = true;
                    //// 2 - get previous ZOrder value
                    prevZOrder = this.Nodes.IndexOf(node);
                    //// 3 - set new ZOrder value
                    //// 3a - try to remove node at prevZOrder location
                    this.Nodes.RemoveAt(prevZOrder);
                    //// 3b - insert node at zOrder location
                    this.Nodes.Insert(zOrder, node);
                    //// 4 - enable events raising on collection changing
                    this.Nodes.QuietMode = bCurrentMode;
                }

                // Update bridges connector by witch removed node is interecions.
                if (IsBridgable(node))
                {
                    UpdateBridges(node as ConnectorBase);
                }

                // raise PropertyChanged event
                OnZOrderChanged(node, changeType, zOrder);
            }

            return prevZOrder;
        }

        /// <summary>
        /// Moves the specified node forward in the Z-order.
        /// </summary>
        /// <param name="node">Node to move forward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int BringForward(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;
            int nZDepth = this.Nodes.Count;
            ZOrderUpdate changeType = ZOrderUpdate.Forward;
            int nNewZOrder = this.Nodes.IndexOf(node) + 1;

            if (nZDepth > nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
            {
                // ensure thread safety
                lock (this.Nodes)
                {
                    //// make history record
                    RecordZOrderChanged(node, changeType);
                    //// 1 - disable events raising on collection changing
                    bool bCurrentMode = this.Nodes.QuietMode;
                    this.Nodes.QuietMode = true;
                    //// 2 - get previous ZOrder value
                    prevZOrder = this.Nodes.IndexOf(node);
                    //// 3 - set new ZOrder value
                    //// 3a - try to remove node at prevZOrder location
                    if (prevZOrder < this.Nodes.Count - 1)
                    {
                        this.Nodes.RemoveAt(prevZOrder);
                        
                        // 3b - bring given node forward
                        this.Nodes.Insert(prevZOrder + 1, node);
                    }
                    else
                        prevZOrder = -1;

                    // 4 - enable events raising on collection changing
                    this.Nodes.QuietMode = bCurrentMode;
                }

                // Update bridges connector by witch removed node is interecions.
                if (IsBridgable(node))
                {
                    UpdateBridges(node as ConnectorBase);
                }

                // raise PropertyChanges event
                OnZOrderChanged(node, changeType, nNewZOrder);
            }

            return prevZOrder;
        }

        /// <summary>
        /// Sends the specified node back in the Z-order.
        /// </summary>
        /// <param name="node">Node to move backward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SendBackward(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;
            ZOrderUpdate changeType = ZOrderUpdate.Backward;
            int nNewZOrder = this.Nodes.IndexOf(node) - 1;

            if (nNewZOrder >= 0 && OnZOrderChanging(node, changeType, nNewZOrder))
            {
                // ensure thread safety
                lock (this.Nodes)
                {
                    //// make history record
                    RecordZOrderChanged(node, changeType);
                    //// 1 - disable events raising on collection changing
                    bool bCurrentMode = this.Nodes.QuietMode;
                    this.Nodes.QuietMode = true;
                    //// 2 - get previous ZOrder value
                    prevZOrder = this.Nodes.IndexOf(node);
                    //// 2 - set new ZOrder value
                    //// 3a - try to remove node at prevZOrder location
                    if (prevZOrder > 0)
                    {
                        this.Nodes.RemoveAt(prevZOrder);
                        //// 3b - bring given node forward
                        this.Nodes.Insert(prevZOrder - 1, node);
                    }
                    else
                        prevZOrder = -1;

                    // 4 - enable events raising on collection changing
                    this.Nodes.QuietMode = bCurrentMode;
                }

                // Update bridges connector by witch removed node is interecions.
                if (IsBridgable(node))
                {
                    UpdateBridges(node as ConnectorBase);
                }

                // raise PropertyChanges event
                OnZOrderChanged(node, changeType, nNewZOrder);
            }

            return prevZOrder;
        }

        /// <summary>
        /// Brings the specified node to the front of the Z-order.
        /// </summary>
        /// <param name="node">Node to bring to the front.</param>
        /// <returns>Previous Z-order position.</returns>
        public int BringToFront(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;
            
            // Cache ZDepth value
            int nZDepth = this.Nodes.Count - 1;
            ZOrderUpdate changeType = ZOrderUpdate.Front;
            int nNewZOrder = nZDepth;

            if (nZDepth >= nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
            {
                // ensure thread safety
                lock (this.Nodes)
                {
                    //// make history record
                    RecordZOrderChanged(node, changeType);
                    //// 1 - disable events raising on collection changing
                    bool bCurrentMode = this.Nodes.QuietMode;
                    this.Nodes.QuietMode = true;
                    //// 2 - get previous ZOrder value
                    prevZOrder = this.Nodes.IndexOf(node);
                    //// 3 - set new ZOrder value
                    //// 3a - try to remove node at prevZOrder location
                    if (prevZOrder < nZDepth)
                    {
                        this.Nodes.RemoveAt(prevZOrder);
                        
                        // 3b - bring given node to front
                        this.Nodes.Insert(nNewZOrder, node);
                    }
                    else
                        prevZOrder = -1;

                    // 4 - enable events raising on collection changing
                    this.Nodes.QuietMode = bCurrentMode;
                }

                // Update bridges connector by witch removed node is interecions.
                if (IsBridgable(node))
                {
                    UpdateBridges(node as ConnectorBase);
                }

                // raise PropertyChanges event
                OnZOrderChanged(node, changeType, nNewZOrder);
            }

            return prevZOrder;
        }

        /// <summary>
        /// Sends the specified node to the back of the Z-order.
        /// </summary>
        /// <param name="node">Node to send to the back.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SendToBack(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;
            
            // Cache ZDepth value
            int nZDepth = this.Nodes.Count - 1;
            ZOrderUpdate changeType = ZOrderUpdate.Back;
            int nNewZOrder = nZDepth;

            if (nZDepth >= nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
            {
                // ensure thread safety
                lock (this.Nodes)
                {
                    //// make history record
                    RecordZOrderChanged(node, changeType);
                    //// 1 - disable events raising on collection changing
                    bool bCurrentMode = this.Nodes.QuietMode;
                    this.Nodes.QuietMode = true;
                    //// 2 - get previous ZOrder value
                    prevZOrder = this.Nodes.IndexOf(node);
                    //// 3 - set new ZOrder value
                    //// 3a - try to remove node at prevZOrder location
                    if (prevZOrder != 0)
                    {
                        this.Nodes.RemoveAt(prevZOrder);
                        
                        // 3b - send given node to back
                        this.Nodes.Insert(0, node);
                    }
                    else
                        prevZOrder = -1;

                    // 4 - enable events raising on collection changing
                    this.Nodes.QuietMode = bCurrentMode;
                }

                // Update bridges connector by witch removed node is interecions.
                if (IsBridgable(node))
                {
                    UpdateBridges(node as ConnectorBase);
                }

                // raise PropertyChanges event
                OnZOrderChanged(node, changeType, nNewZOrder);
            }

            return prevZOrder;
        }
        #endregion
    }
	public enum GroupNodePositions
    {
        Absolute,
        Relative
    }
}
