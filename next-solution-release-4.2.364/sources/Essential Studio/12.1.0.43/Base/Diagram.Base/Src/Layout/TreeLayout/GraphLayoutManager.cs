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
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The GraphLayoutManager defines the base class used for implementing layout managers for diagrams composed 
    /// of nodes that form a connected graph. The <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/> 
    /// provides the necessary foundation for initializing, validating and creating the diagram graph. The GraphLayoutManager 
    /// also implements the infrastructure for positioning diagram nodes using the specialized layout strategies 
    /// provided by custom layout managers.
    /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutManager"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DirectedTreeLayoutManager"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.RadialTreeLayoutManager"/>
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public abstract class GraphLayoutManager : LayoutManager
    {
        #region Constants
        private const int c_nDEFAULT_OFFSET = 10;
        #endregion Constants

        #region Fields
        private float m_nVerticalSpacing = c_nDEFAULT_OFFSET;
        private float m_nHorizontalSpacing = c_nDEFAULT_OFFSET;
        private object m_objSelectedNode;
        private Hashtable m_hashPassedNodes;
        protected float m_fXBorder;
        protected float m_fYBorder;
        protected float m_fYHighest;
        private bool m_ImprovePerformance = false;
        
        /// <summary>
        /// Selected node what constraint graph in position.
        /// </summary>
        private object m_helperSelectedNode;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets the vertical offset between adjacent nodes.
        /// </summary>
        [DefaultValue(c_nDEFAULT_OFFSET)]
        public float VerticalSpacing
        {
            get 
            { 
                return m_nVerticalSpacing; 
            }
            set
            {
                if (m_nVerticalSpacing != value)
                    m_nVerticalSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset between adjacent nodes.
        /// </summary>
        [DefaultValue(c_nDEFAULT_OFFSET)]
        public float HorizontalSpacing
        {
            get 
            { 
                return m_nHorizontalSpacing; 
            }
            set
            {
                if (m_nHorizontalSpacing != value)
                    m_nHorizontalSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the passed node collection where key is node fullname and value - node reference.
        /// </summary>
        /// <value>The passed nodes.</value>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected Hashtable PassedNodes
        {
            get
            {
                if (m_hashPassedNodes == null)
                    m_hashPassedNodes = new Hashtable();
                return m_hashPassedNodes;
            }
            set
            {
                if (m_hashPassedNodes != value)
                    m_hashPassedNodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the current selected node while generation node graph.
        /// </summary>
        /// <value>The selected node.</value>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected object SelectedNode
        {
            get
            {
                return m_objSelectedNode;
            }
            set
            {
                if (m_objSelectedNode != value)
                    m_objSelectedNode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether performance can be improved.
        /// </summary>
        public bool ImprovePerformance
        {
            get
            {
                return m_ImprovePerformance;
            }
            set
            {
                m_ImprovePerformance = value;
            }
        }
        
        #endregion Properties

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLayoutManager"/> class.
        /// </summary>
        public GraphLayoutManager()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fHorizontalSpacing">The horizontal spacing between nodes.</param>
        /// <param name="fVerticalSpacing">The vertical spacing between nodes.</param>
        public GraphLayoutManager(Model model, float fHorizontalSpacing, float fVerticalSpacing)
            : base(model)
        {
            m_nVerticalSpacing = fVerticalSpacing;
            m_nHorizontalSpacing = fHorizontalSpacing;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Updates the layout of the nodes in the model.
        /// </summary>
        /// <param name="contextInfo">Provides context information to help with updating the layout.</param>
        /// <returns>True if changes were made; otherwise False.</returns>
        public override bool UpdateLayout(object contextInfo)
        {
            // get selected node list form given contextInfo
            NodeCollection selectionList = contextInfo as NodeCollection;

            if (selectionList != null && selectionList.Count > 0)
            {
                // get first node from selection list
                m_helperSelectedNode = (Node)selectionList[0].Clone();
            }

            return UpdateLayout(this.Nodes);
        }

        /// <summary>
        /// Occurs before a node(or graph) is positioned.
        /// </summary>
        /// <param name="evtargs">Event arguments.</param>
        /// <remarks>
        /// <para>This method can be overridden in derived classes.</para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.PreferredLayoutEventArgs"/>
        /// </remarks>
        protected virtual void OnPreferredLayout(PreferredLayoutEventArgs evtargs)
        {
            if (PreferredLayout != null)
                PreferredLayout(this, evtargs);
        }
        #endregion Overrides

        #region HelperMethods
        /// <summary>
        /// Updates the layout of the given nodes.
        /// </summary>
        /// <param name="nodesToLayout">The nodes to layout.</param>
        /// <returns>
        /// True if changes were made; otherwise False.
        /// </returns>
        protected bool UpdateLayout(NodeCollection nodesToLayout)
        {
            int nodesCount = nodesToLayout.Count;

            if (nodesCount > 0)
            {
                // save boundary contrains state
                bool bBoundaryConstraintsEnabled = this.Model.BoundaryConstraintsEnabled;
                // its do for nodes collection don't changes in build graph time
                NodeCollection nodes = new NodeCollection();
                NodeCollection nodeSymbols = new NodeCollection();
                foreach (Node nd in nodesToLayout)
                {
                    if (nd is IGraphEdge)
                    {
                        nodes.Add(nd);
                    }
                    else
                    {
                        nodeSymbols.Add(nd);
                    }
                }

                nodes.AddRange(nodeSymbols);

                BeginLayout(false);
                Hashtable editStyleTable = QuiteResetEditStyle(nodes);

                while (nodesCount > this.PassedNodes.Count)
                {
                    GetNodesToPosition(nodes);

                    if (this.SelectedNode == null)
                        continue;

                    DoLayout();

                    Graph graph = this.SelectedNode as Graph;

                    // append graphNode changes to its nodes.
                    if (graph != null)
                    {
                        graph.AppendTransfomChanges();
                    }

                    this.SelectedNode = null;
                }

                QuiteRestoreEditStyle(editStyleTable);

                // Reset layout border width and high.
                m_fXBorder = MeasureUnitsConverter.ConvertX(this.Model.LineStyle.LineWidth, this.Model.MeasurementUnits, MeasureUnits.Pixel);
                m_fYBorder = MeasureUnitsConverter.ConvertX(this.Model.LineStyle.LineWidth, this.Model.MeasurementUnits, MeasureUnits.Pixel);
                m_fYHighest = 0;

                m_hashPassedNodes = null;
                m_objSelectedNode = null;

                EndLayout(bBoundaryConstraintsEnabled);
            }

            return false;
        }

        /// <summary>
        /// Does the layout with Nodes.
        /// </summary>
        private void DoLayout()
        {
            object selectedNode = this.SelectedNode;
            Node node = selectedNode as Node;
            Graph graph = selectedNode as Graph;
            IGBounds bounds = selectedNode as IGBounds;

            if (node == null && graph == null)
                throw new ArgumentNullException("Invalid Graph!");
            
            // layout if selected node is graph
            if (IsGraph())
                DoGraphLayout();

            MeasureUnits units = MeasureUnits.Pixel;

            // get graph or node bounds
            RectangleF rectBounds = (graph != null) ? graph.Bounds
                : MeasureUnitsConverter.ToPixels(node.BoundingRectangle, node.MeasurementUnit);

            // get model bounds
            SizeF szModelSizePixel = MeasureUnitsConverter.ToPixels(this.Model.LogicalSize, this.Model.MeasurementUnits);
            RectangleF rectModel = new RectangleF(PointF.Empty, szModelSizePixel);

            // get default next node location
            PointF nodeLocation = PointF.Empty;
            DefaultNodePosition(rectBounds.Width, rectModel, ref nodeLocation);

            // raize Preferedlayout event
            PreferredLayoutEventArgs e = new PreferredLayoutEventArgs(true, nodeLocation, rectBounds.Size);
            //if (IsGraph())
                OnPreferredLayout(e);

            // update nodes size
            if (e.Size != rectBounds.Size)
            {
                // update resize nodes
                if (graph != null)
                    graph.ResizeNodes = e.ResizeGraphNodes;

                // update node size to graph/node default size
                if (bounds != null)
                    UpdateNodeSize(e, bounds);
            }

            PointF location = PointF.Empty;
            if (e.Location != nodeLocation)
            {
                float graphX = nodeLocation.X + e.Location.X;
                float graphY = nodeLocation.Y + e.Location.Y;

                location = new PointF(graphX, graphY);
            }
            else
            {
                location = nodeLocation;
            }

            // set graph/node start location
            if (node != null)
            {
                location = new PointF(location.X + this.LeftMargin, location.Y + this.TopMargin);
                SizeF szPinOffset = ((IUnitIndependent)node).GetPinPointOffset(units);
                PointF ptPinLocation = new PointF(location.X + szPinOffset.Width, location.Y + szPinOffset.Height);
                ((IUnitIndependent)node).SetPinPoint(ptPinLocation, units);
            }
            else if (graph != null)
            {
                graph.Location = location;
            }

            rectBounds.Location = nodeLocation;

            // place graph/node in row and column
            //if (e.Location == nodeLocation)
            //{
                // move graph/node down to previous graph/node
                if ((rectBounds.Width + m_fXBorder + this.LeftMargin) > rectModel.Right)
                {
                    m_fXBorder = rectModel.X + this.HorizontalSpacing + rectBounds.Width;

                    m_fYBorder = rectBounds.Y;
                    m_fYHighest = rectBounds.Height;
                }
                else
                {
                    // move graph/node left to previous graph/node
                    m_fXBorder += this.HorizontalSpacing + rectBounds.Width;

                    if (m_fYHighest < rectBounds.Height)
                        m_fYHighest = rectBounds.Height;
                }
            //}

            // update raph position to selected node
            PositionGraphToSelectNode(this.SelectedNode as IGBounds);
        }
        private void PositionGraphToSelectNode(IGBounds bounds)
        {
            if (bounds != null && m_helperSelectedNode != null)
            {
                MeasureUnits units = MeasureUnits.Pixel;

                Node node = m_helperSelectedNode as Node;
                GraphNode gnNode = m_helperSelectedNode as GraphNode;

                if (node == null && gnNode != null)
                {
                    node = gnNode.Node;
                }

                PointF ptPinPointOrigin = ((IUnitIndependent)node).GetPinPoint(units);
                PointF ptPinPointClone = ptPinPointOrigin;

                if (gnNode != null)
                {
                    SizeF szPinPointOffset = ((IUnitIndependent)node).GetPinPointOffset(units);
                    ptPinPointClone = new PointF(gnNode.X + szPinPointOffset.Width, gnNode.Y + szPinPointOffset.Height);
                }

                SizeF szOffset = new SizeF(ptPinPointOrigin.X - ptPinPointClone.X, ptPinPointOrigin.Y - ptPinPointClone.Y);

                if (!szOffset.IsEmpty)
                {
                    ptPinPointOrigin = bounds.Location;
                    ptPinPointOrigin.X += szOffset.Width;
                    ptPinPointOrigin.Y += szOffset.Height;
                    bounds.Location = ptPinPointOrigin;

                    m_helperSelectedNode = null;
                }
            }
        }

        #region Create Graph
        /// <summary>
        /// Selects nodes from Model children-node collection, adding selected to PassedNodes hashtable
        /// </summary>
        /// <param name="nodes">The nodes collection.</param>
        private void GetNodesToPosition(NodeCollection nodes)
        {
            foreach (INode node in nodes)
            {
                if (!this.PassedNodes.ContainsKey(node.FullName))
                {
                    if ((node is IGraphNode) || (node is IGraphEdge))
                    {
                        SelectNodes(node);
                    }
                    else if (AddNode(node, NodesCollection.Passed))
                    {
                        this.SelectedNode = node;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Select connected nodes, create graph and set as SelectedNode.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SelectNodes(INode node)
        {
            IGraphNode nodeGraph = node as IGraphNode;

            if (node is IGraphEdge)
            {
                ExploreGraphEdge(node);
            }
            else if (nodeGraph != null)
            {
                if (AddNode(node, NodesCollection.Passed))
                {
                    AddNode(node, NodesCollection.Selected);

                    if (IsConnectedToAnotherNode(nodeGraph))
                    {
                        // create the graph and set as Selected
                        this.SelectedNode = new Graph(m_ImprovePerformance, this.LeftMargin, this.TopMargin);
                        
                        // fill graph
                        SelectConnectedNodes(nodeGraph);
                    }
                    else
                    {
                        this.SelectedNode = node;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates through all edges ( leaving and entering ), seeking for connected nodes.
        /// If no connected node found -- no need to treat gnNode as graph.
        /// </summary>
        /// <param name="gnNode">node whose edges will be searched.</param>
        /// <returns>true, if connected to another node.</returns>
        private bool IsConnectedToAnotherNode(IGraphNode gnNode)
        {
            bool bFoundConnectedNode = false;

            if (gnNode.Edges.Count > 0)
            {
                if ((gnNode.EdgesEntering != null) && (gnNode.EdgesEntering.Count > 0))
                    bFoundConnectedNode = SearchEdgeCollection(gnNode.EdgesEntering, EdgeConnected.FromNode);

                if ((!bFoundConnectedNode) && (gnNode.EdgesLeaving != null) && (gnNode.EdgesLeaving.Count > 0))
                    bFoundConnectedNode = SearchEdgeCollection(gnNode.EdgesLeaving, EdgeConnected.ToNode);
            }

            return bFoundConnectedNode;
        }

        /// <summary>
        /// Looks through edges collection till first connected node found.
        /// </summary>
        /// <param name="edgesToSearchThrough">Edges to search collection.</param>
        /// <param name="connectionDirection">Connected edges</param>
        /// <returns>true, when first node is found.</returns>
        private bool SearchEdgeCollection(ICollection edgesToSearchThrough, EdgeConnected connectionDirection)
        {
            bool bFoundConnectedNode = false;

            foreach (IGraphEdge edge in edgesToSearchThrough)
            {
                if (!AddNode(edge, NodesCollection.Passed))
                    continue;

                if ((connectionDirection == EdgeConnected.FromNode && edge.FromNode != null)
                    || (connectionDirection == EdgeConnected.ToNode && edge.ToNode != null))
                {
                    bFoundConnectedNode = true;
                    break;
                }
            }

            return bFoundConnectedNode;
        }

        #endregion

        #region Relative mathods
        private void GetConnectedRelatives(GraphNode graphNode)
        {
            GetConnectedParents(graphNode);
            GetConnectedChildren(graphNode);
        }
        private void GetConnectedChildren(GraphNode graphNode)
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph == null)
                throw new ArgumentNullException("!!!!!!!!!!");

            IGraphNode nodeGraph = graphNode.Node as IGraphNode;

            foreach (IGraphEdge edge in nodeGraph.EdgesLeaving)
            {
                Node node = edge.ToNode as Node;

                if (this.Nodes.Contains(node) && node != null && node.Visible)
                {
                    GraphNode gnNode;

                    if (!graph.GraphNodes.ContainsKey(node.FullName))
                    {
                        gnNode = AddGraphNode(node);
                        gnNode.Added = true;
                    }
                    else
                    {
                        gnNode = (GraphNode)graph.GraphNodes[node.FullName];
                    }

                    // set Reference to parent node
                    SetNode(gnNode.Parents, graphNode);

                    // add new node as child to parent node
                    if (FindNode(graphNode.Children, gnNode.FullName) < 0)
                        graphNode.Children.Add(gnNode);
                }
            }
        }

        private GraphNode AddGraphNode(Node node)
        {
            Graph graph = this.SelectedNode as Graph;

            GraphNode gnNode = new GraphNode(node);

            if (graph != null)
            {
                graph.GraphNodes.Add(gnNode.Node.FullName, gnNode);

                Node nodeHelper = m_helperSelectedNode as Node;

                // update helperNode as Node to GraphNode
                if (nodeHelper != null && node.Name == nodeHelper.Name)
                    m_helperSelectedNode = gnNode;
            }

            return gnNode;
        }
        private void GetConnectedParents(GraphNode graphNode)
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph == null)
                throw new ArgumentNullException("!!!!!!!!!!");

            IGraphNode nodeGraph = graphNode.Node as IGraphNode;

            foreach (IGraphEdge edge in nodeGraph.EdgesEntering)
            {
                Node node = edge.FromNode as Node;
                if (this.Nodes.Contains(node) && node != null && node.Visible)
                {
                    GraphNode gnNode;

                    if (!graph.GraphNodes.ContainsKey(node.FullName))
                    {
                        gnNode = AddGraphNode(node);
                        gnNode.Added = true;
                    }
                    else
                    {
                        gnNode = (GraphNode)graph.GraphNodes[node.FullName];
                    }

                    // set Reference to child node
                    SetNode(gnNode.Children, graphNode);

                    // add new node as parent to child node
                    if (FindNode(graphNode.Parents, gnNode.FullName) < 0)
                        graphNode.Parents.Add(gnNode);
                }
            }
        }
        private void SetNode(ArrayList list, GraphNode node)
        {
            int nIndex = FindNode(list, node.FullName);

            if (nIndex >= 0 && nIndex < list.Count)
            {
                list[nIndex] = node;
            }
            else
            {
                list.Add(node);
            }
        }
        private int FindNode(ArrayList list, string fullName)
        {
            int nIndex = -1;

            if (list != null && fullName != string.Empty)
            {
                for (int i = 0, nLength = list.Count; i < nLength; i++)
                {
                    GraphNode gnNode = list[i] as GraphNode;

                    if (gnNode != null && gnNode.FullName == fullName)
                    {
                        nIndex = i;
                        break;
                    }
                }
            }

            return nIndex;
        }

        private void ExploreRelatives(IGraphNode nodeGraph)
        {
            ExploreRelatives(nodeGraph, Relatives.Parents);
            ExploreRelatives(nodeGraph, Relatives.Children);
        }

        private void ExploreRelatives(IGraphNode nodeGraph, Relatives relativesToExplore)
        {
            ICollection edges = null;
            Graph graph = this.SelectedNode as Graph;

            if (relativesToExplore == Relatives.Parents)
            {
                edges = nodeGraph.EdgesEntering;
            }
            else if (relativesToExplore == Relatives.Children)
            {
                edges = nodeGraph.EdgesLeaving;
            }

            foreach (IGraphEdge edge in edges)
            {
                if (AddNode(edge, NodesCollection.Passed))
                {
                    Node fromNode = (Node)edge.FromNode;
                    Node toNode = (Node)edge.ToNode;

                    if (relativesToExplore == Relatives.Parents && fromNode != null && this.Nodes.Contains(fromNode))
                    {
                        SelectConnectedNodes(edge.FromNode);
                    }
                    else if (relativesToExplore == Relatives.Children && toNode != null && this.Nodes.Contains(toNode))
                    {
                        SelectConnectedNodes(edge.ToNode);
                    }
                }
            }
        }
        #endregion

        private bool AddNode(object nodeToAdd, NodesCollection collectionToAdd)
        {
            bool bResult = true;
            Node node = nodeToAdd as Node;

            if (node == null)
                throw new InvalidCastException("This object does not implement INode interface!");

            if (collectionToAdd == NodesCollection.Passed || !node.Visible)
            {
                if (!this.PassedNodes.ContainsKey(node.FullName))
                    this.PassedNodes.Add(node.FullName, node);
            }

            // Check for node visible
            if (!node.Visible)
                return false;

            return bResult;
        }

        private void ExploreGraphEdge(INode node)
        {
            IGraphEdge nodeLink = node as IGraphEdge;

            if (nodeLink != null && AddNode(node, NodesCollection.Passed))
            {
                AddNode(node, NodesCollection.Selected);
                INode fromNode = nodeLink.FromNode as INode;
                INode toNode = nodeLink.ToNode as INode;

                if (fromNode != null)
                {
                    SelectNodes(fromNode);
                }
                else if (toNode != null)
                {
                    SelectNodes(toNode);
                }
                else
                    this.SelectedNode = node;
            }
        }

        /// <summary>
        /// Selects connected nodes to CurrentGraph hashtable.
        /// </summary>
        /// <param name="nodeGraph">Graph node.</param>
        private void SelectConnectedNodes(IGraphNode nodeGraph)
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph == null)
                throw new ArgumentNullException("!!!!!!!!!!");

            Node node = nodeGraph as Node;

            if (node != null && AddNode(node, NodesCollection.Passed))
            {
                string nodeName = node.FullName;

                if (!graph.GraphNodes.ContainsKey(nodeName))
                {
                    GraphNode gnNode = AddGraphNode(node);
                    GetConnectedRelatives(gnNode);
                    ExploreRelatives(nodeGraph);
                }
                else
                {
                    GraphNode graphNode = graph.GraphNodes[nodeName] as GraphNode;

                    if (graphNode.Added)
                    {
                        graphNode.Added = false;
                        GetConnectedRelatives(graphNode);
                        ExploreRelatives(nodeGraph);
                    }
                }
            }
        }

        private void UpdateNodeSize(PreferredLayoutEventArgs e, IGBounds bounds)
        {
            if (e.Size.Width > 0)
                bounds.Width = e.Size.Width;

            if (e.Size.Height > 0)
                bounds.Height = e.Size.Height;
        }

        private void UpdateNodeSize(PreferredLayoutEventArgs e, Node bounds)
        {
            SizeF szNew = ((IUnitIndependent)bounds).GetSize(MeasureUnits.Pixel);

            if (e.Size.Width > 0)
            {
                szNew.Width = e.Size.Width;
            }

            if (e.Size.Height > 0)
            {
                szNew.Height = e.Size.Height;
            }

            ((IUnitIndependent)bounds).SetSize(szNew, MeasureUnits.Pixel);
        }
        private void DefaultNodePosition(float nodeWidth, RectangleF rectModel, ref PointF nodeLocation)
        {
            if ((nodeWidth + m_fXBorder + this.LeftMargin) > rectModel.Right)
            {
                nodeLocation.X = rectModel.X;
				nodeLocation.Y = m_fYBorder + m_fYHighest + this.VerticalSpacing;
            }
            else
            {
                nodeLocation.X = m_fXBorder;
                nodeLocation.Y = m_fYBorder;
            }
        }

        private bool IsGraph()
        {
            bool bIsGraph = false;

            if (this.SelectedNode is Graph)
                bIsGraph = true;

            return bIsGraph;
        }

        private void BeginLayout(bool bBoundaryConstraintsEnabled)
        {
            // Set updating layout flag.
            this.UpdatingLayout = true;

            this.Model.HistoryManager.StartAtomicAction("Layout");
            this.Model.BeginUpdate();
            this.Model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;
        }
        private void EndLayout(bool bBoundaryConstraintsEnabled)
        {
            this.Model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;
            this.Model.EndUpdate();
            this.Model.HistoryManager.EndAtomicAction();

            // Reset update layouting flag.
            this.UpdatingLayout = false;
        }

        /// <summary>
        /// Quites the reset move locking.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The hashtable.</returns>
        private Hashtable QuiteResetEditStyle(NodeCollection nodes)
        {
            Hashtable table = new Hashtable();
            Model model = this.Model;

            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
            }

            foreach (Node node in nodes)
            {
                EditStyle styleToReturn = (EditStyle)node.EditStyle.Clone();
                table[node] = styleToReturn;

                node.EditStyle.AllowMoveX = true;
                node.EditStyle.AllowMoveY = true;
                node.EditStyle.AllowDelete = true;
                node.EditStyle.AllowRotate = true;
                node.EditStyle.AllowChangeWidth = true;
                node.EditStyle.AllowChangeHeight = true;
            }

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }

            return table;
        }

        /// <summary>
        /// Quites the restore move locking.
        /// </summary>
        /// <param name="table">The hashtable where key - node, value - editStyle.</param>
        private void QuiteRestoreEditStyle(Hashtable table)
        {
            Model model = this.Model;

            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
            }

            ICollection nodes = table.Keys;

            foreach (Node node in nodes)
            {
                EditStyle editStyle = table[node] as EditStyle;

                node.EditStyle.AllowMoveX = editStyle.AllowMoveX;
                node.EditStyle.AllowMoveY = editStyle.AllowMoveY;
                node.EditStyle.AllowDelete = editStyle.AllowDelete;
                node.EditStyle.AllowRotate = editStyle.AllowRotate;
                node.EditStyle.AllowChangeWidth = editStyle.AllowChangeWidth;
                node.EditStyle.AllowChangeHeight = editStyle.AllowChangeHeight;
            }

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Applies a custom layout management strategy on the diagram.
        /// </summary>
        protected abstract void DoGraphLayout();
        #endregion

        #region Events
        /// <summary>
        /// Occurs before a node graph is positioned. 
        /// </summary>
        /// <remarks>
        /// Handling the PreferredLayout event provides users with the ability to examine the bounds used for 
        /// laying out the node graph and specify a different bounds if necessary.
        /// </remarks>
        [Description("Occurs before a node is positioned.")]
        public event PreferredLayoutEventHandler PreferredLayout;

        #endregion
    }

    /// <summary>
    /// Event argument used for the <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager.PreferredLayout"/> event.
    /// </summary>
    public class PreferredLayoutEventArgs : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PreferredLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="bIsGraphUnderlayout">Specifies whether the particular graph is under layout.</param>
        /// <param name="location">Specifies the node or graph location.</param>
        /// <param name="size">Specifies the node or graph size.</param>
        public PreferredLayoutEventArgs(bool bIsGraphUnderlayout, PointF location, SizeF size)
        {
            m_bIsGraphUnderLayout = bIsGraphUnderlayout;
            m_ptLocation = location;
            m_szSize = size;
        }

        #endregion

        #region Fields
        private PointF m_ptLocation = PointF.Empty;
        private SizeF m_szSize = SizeF.Empty;
        private bool m_bIsGraphUnderLayout = false;
        private bool m_bResizeGraphNodes = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether graph nodes will be resized.
        /// </summary>
        public bool ResizeGraphNodes
        {
            get
            {
                return m_bResizeGraphNodes;
            }
            set
            {
                m_bResizeGraphNodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the preferred graph location.
        /// </summary>
        public PointF Location
        {
            get
            {
                return m_ptLocation;
            }
            set
            {
                if (m_ptLocation != value)
                    m_ptLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the preferred graph size.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_szSize;
            }
            set
            {
                if (m_szSize != value)
                    m_szSize = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the graph is under layout.
        /// </summary>
        public bool IsGraphUnderLayout
        {
            get
            {
                return m_bIsGraphUnderLayout;
            }
        }

        #endregion
    }

    /// <summary>
    /// Delegate used for the <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager.PreferredLayout"/> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="evtArgs">The evtargs.</param>
    public delegate void PreferredLayoutEventHandler(object sender, PreferredLayoutEventArgs evtArgs);
}
