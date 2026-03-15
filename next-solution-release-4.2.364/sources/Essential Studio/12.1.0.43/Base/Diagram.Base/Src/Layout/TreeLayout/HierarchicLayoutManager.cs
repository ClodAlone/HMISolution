#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Inherits from GraphLayoutManager. Sort nodes by rank and place nodes to it parent/children relations.
    /// </summary>
    [ToolboxItem(false)]
    public class HierarchicLayoutManager : GraphLayoutManager
    {
        #region Class members
        /// <summary>
        /// Graph Rotation degree.
        /// </summary>
        private float m_fRotationDegree = 0;
        private bool m_SingleChildLayout = false;
        private RotateDirection m_Rotate;
        private ParentPositions m_parentPosition = ParentPositions.Center;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the parent position.
        /// </summary>
        /// <value>The parent position.</value>
        public ParentPositions ParentPosition
        {
            get { return m_parentPosition; }
            set { m_parentPosition = value; }
        }

        /// <summary>
        /// Gets or sets the rotation angle for graph.
        /// </summary>
        /// <value>The rotation angle.</value>
        public float RotationAngle
        {
            get { return m_fRotationDegree; }
            set
            {
                m_fRotationDegree = value;
                m_Rotate = (RotateDirection)Enum.Parse(typeof(RotateDirection), m_fRotationDegree.ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets the rotate direction.
        /// </summary>
        public RotateDirection RotateDirection
        {
            get
            {
                return m_Rotate;
            }
            set
            {
                m_Rotate = value;
                this.RotationAngle = (float)m_Rotate;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicLayoutManager"/> class.
        /// </summary>
        public HierarchicLayoutManager()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="fRotationDegree">The Rotation degree.</param>
        /// <param name="fVerticalOffset">The n vertical offset.</param>
        /// <param name="fHorizontalOffset">The n horizontal offset.</param>
        public HierarchicLayoutManager(Model model, float fRotationDegree, float fVerticalOffset, float fHorizontalOffset)
            : base(model, fHorizontalOffset, fVerticalOffset)
        {
            m_fRotationDegree = fRotationDegree;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies a custom layout management strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph != null)
                MakeLayout(graph);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Performs the graph layout.
        /// </summary>
        /// <param name="graphSorting">The graph.</param>
        protected void MakeLayout(Graph graphSorting)
        {
            if (graphSorting != null)
            {
                DoInitXLayout(graphSorting);
                MakeXLayout(graphSorting.TypeOrdered);
                
                // BreakConnectors( graphSorting.TypeOrdered );
                // apply rotation
                graphSorting.Rotate(m_fRotationDegree);

                IEndPointContainer container;
                Graph graph = this.SelectedNode as Graph;
                ArrayList lstToSort = graph.TypeOrdered;
                int m_TreeLevel = lstToSort.Count;
                // update connector heading
                foreach (Node node in this.Nodes)
                {
                    container = node as IEndPointContainer;

                    if (container == null)
                        continue;

                    if (container is OrgLineConnector)
                    {
                        OrgLineConnector orgLine = container as OrgLineConnector;
                        if (orgLine == null)
                        {
                            container = ReplaceConnector(container);
                            orgLine = container as OrgLineConnector;
                            continue;

                        }

                        Node toNode = orgLine.ToNode as Node;
                        int nCounter = 0;

                        // Get the level or rank to which the toNode belong.
                        foreach (ArrayList lstSort in lstToSort)
                        {
                            ArrayList alRankedNode = new ArrayList();
                            foreach (GraphNode gn in lstSort)
                            {
                                alRankedNode.Add(gn.Node);
                            }
                            if (alRankedNode.Contains(toNode))
                                break;
                            nCounter++;
                        }

                        if (nCounter >= lstToSort.Count)
                            continue;

                        if (nCounter <= m_TreeLevel)
                        {
                            CompassHorizontalTree(ref container);
                        }
                        else
                        {
                            CompassVerticalTree(ref container);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the HeadingHead and HeadingTail for the connectors in Horizontal tree
        /// </summary>
        /// <param name="container">The container</param>
        private void CompassHorizontalTree(ref IEndPointContainer container)
        {
            OrgLineConnector orgLine = container as OrgLineConnector;
            switch (m_Rotate)
            {
                case RotateDirection.BottomToTop:
                    orgLine.HeadingHead = CompassHeading.South;
                    orgLine.HeadingTail = CompassHeading.North;
                    break;
                case RotateDirection.LeftToRight:
                    orgLine.HeadingHead = CompassHeading.West;
                    orgLine.HeadingTail = CompassHeading.East;
                    break;
                case RotateDirection.RightToLeft:
                    orgLine.HeadingHead = CompassHeading.East;
                    orgLine.HeadingTail = CompassHeading.West;
                    break;
                case RotateDirection.TopToBottom:
                    orgLine.HeadingHead = CompassHeading.North;
                    orgLine.HeadingTail = CompassHeading.South;
                    break;
                default:
                    orgLine.HeadingHead = CompassHeading.None;
                    orgLine.HeadingTail = CompassHeading.None;
                    break;
            }
        }

        /// <summary>
        /// Sets the HeadingHead and HeadingTail for the connectors in vertical tree
        /// </summary>
        /// <param name="container">The container</param>
        private void CompassVerticalTree(ref IEndPointContainer container)
        {
            OrgLineConnector orgLine = container as OrgLineConnector;
            Node fromNode = orgLine.FromNode as Node;
            bool IsSingleChild = false;
            if (fromNode != null)
            {
                IsSingleChild = (fromNode.EdgesLeaving.Count == 1) && m_SingleChildLayout;
            }
            switch (m_Rotate)
            {
                case RotateDirection.BottomToTop:
                    orgLine.HeadingHead = CompassHeading.South;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.North : CompassHeading.East);
                    break;
                case RotateDirection.LeftToRight:
                    orgLine.HeadingHead = CompassHeading.West;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.East : CompassHeading.South);
                    break;
                case RotateDirection.RightToLeft:
                    orgLine.HeadingHead = CompassHeading.East;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.West : CompassHeading.South);
                    break;
                case RotateDirection.TopToBottom:
                    orgLine.HeadingHead = (IsSingleChild ? CompassHeading.North : CompassHeading.West);
                    orgLine.HeadingTail = CompassHeading.South;
                    break;
                default:
                    orgLine.HeadingHead = CompassHeading.None;
                    orgLine.HeadingTail = CompassHeading.None;
                    break;
            }
        }

        /// <summary>
        /// Replace the current connector to OrgLineconnector.
        /// </summary>
        /// <param name="container">The end point container.</param>
        /// <returns>Replaced orthogonal connector.</returns>
        private IEndPointContainer ReplaceConnector(IEndPointContainer container)
        {
            Node node = container as Node;
            LineBase line = container as LineBase;

            if (node == null)
                return container;

            // safe node name and parent
            string name = node.Name;
            ICompositeNode parent = node.Parent;

            PointF ptStart = node.BoundingRectangle.Location;
            PointF ptEnd = new PointF(node.BoundingRectangle.Right, node.BoundingRectangle.Bottom);

            ConnectionPoint headPort = container.HeadEndPoint.Port;
            ConnectionPoint tailPort = container.TailEndPoint.Port;

            // remove itself from parent
            int index = this.Nodes.IndexOf(node);
            parent.RemoveChild(node);
            this.Nodes.Remove(node);

            // create orthogonal connector
            // OrthogonalConnector ortho = new OrthogonalConnector( ptStart, ptEnd );
            OrgLineConnector ortho = new OrgLineConnector(ptStart, ptEnd);
            ortho.Name = name;

            // add orthogonal connector to parent node
            this.Nodes.Insert(index, ortho);
            parent.AppendChild(ortho);
            (parent as Model).SendToBack(ortho);

            // customize connector
            if (line != null)
            {
                ortho.HeadDecorator.DecoratorShape = line.HeadDecorator.DecoratorShape;
                ortho.TailDecorator.DecoratorShape = line.TailDecorator.DecoratorShape;
            }

            // connect connector
            if (headPort != null)
            {
                headPort.TryConnect(ortho.HeadEndPoint);
            }

            if (tailPort != null)
            {
                tailPort.TryConnect(ortho.TailEndPoint);
            }

            return ortho;
        }

        /// <summary>
        /// Extracts subGraph from specified node.
        /// </summary>
        /// <param name="dtgnFrom">new graph TopNode.</param>
        /// <param name="nRank">current Rank.</param>
        /// <param name="graphToFill">Graph to extract to.</param>
        protected void ExtractSubGraph(GraphNode dtgnFrom, int nRank, ref Graph graphToFill)
        {
            Graph graphCurrent = this.SelectedNode as Graph;
            GraphNode gnTop = graphCurrent.GraphNodes[dtgnFrom.Node.FullName] as GraphNode;
            gnTop.Parents = null;
            graphToFill.GraphNodes.Add(gnTop.Node.FullName, gnTop);

            ExploreTree(dtgnFrom, ref graphToFill, nRank);
        }

        /// <summary>
        /// Extracting subGraph routine. Enumerates through graphNode children changing relatives relations. Filling new graph.
        /// </summary>
        /// <param name="dtgnNode">The graph node.</param>
        /// <param name="graphToFill">The graph to fill.</param>
        /// <param name="nRank">The rank.</param>
        private void ExploreTree(GraphNode dtgnNode, ref Graph graphToFill, int nRank)
        {
            Graph graphCurrent = this.SelectedNode as Graph;

            GraphNode gnChild = graphCurrent.GraphNodes[dtgnNode.Node.FullName] as GraphNode;
            if (!graphToFill.GraphNodes.ContainsKey(gnChild.Node.FullName))
            {
                // add node to infilling graph
                graphToFill.GraphNodes.Add(gnChild.Node.FullName, gnChild);
                
                // remove node from current graph
                graphCurrent.GraphNodes.Remove(gnChild.Node.FullName);
            }

            if (dtgnNode.Parents.Count > 0)
            {
                ArrayList lst = graphCurrent.TypeOrdered[nRank] as ArrayList;
                int index = lst.IndexOf(dtgnNode);
                if (index != -1)
                    lst.RemoveAt(index);
            }

            foreach (object dtgnChild in dtgnNode.Children)
            {
                if (dtgnChild is GraphNode)
                {
                    GraphNode dtgnNo = dtgnChild as GraphNode;
                    ExploreTree(dtgnNo, ref graphToFill, nRank + 1);
                }
                else if (dtgnChild is Graph)
                {
                    // must change relatives relations.
                    graphToFill.GraphNodes.Add(((Graph)dtgnChild).GetGraphFirstTopNode().Node.FullName, dtgnChild);
                    graphCurrent.GraphNodes.Remove(((Graph)dtgnChild).GetGraphFirstTopNode().Node.FullName);
                    ArrayList lst = graphCurrent.TypeOrdered[nRank + 1] as ArrayList;
                    int index = lst.IndexOf(dtgnChild);
                    if (index != -1)
                        lst.RemoveAt(index);
                }
            }
        }

        private void MakeXLayout(ArrayList lstToSort)
        {
            for (int nCounter = 1, nLength = lstToSort.Count; nCounter < nLength; nCounter++)
            {
                ArrayList lstSort = (ArrayList)lstToSort[nCounter];

                UpdateTreeToTop(lstToSort, nCounter - 1);
                MakeXLayoutRelative(lstToSort, lstSort, false, nCounter);
            }
            if (this.m_parentPosition == ParentPositions.Center)
            {
                for (int nCounter = 0; nCounter < lstToSort.Count; nCounter++)
                {
                    ArrayList hashToSort = lstToSort[nCounter] as ArrayList;
                    foreach (GraphNode node in hashToSort)
                    {
                        if (node.Parents.Count > 1)
                        {
                            GraphNode startNode = (GraphNode)node.Parents[0];
                            float fMinX = startNode.X;
                            float fMaxX = startNode.X;
                            for (int i = 0; i < node.Parents.Count; i++)
                            {
                                GraphNode currentNode = node.Parents[i] as GraphNode;
                                if (currentNode.X < fMinX)
                                    fMinX = currentNode.X;
                                if (currentNode.X > fMaxX)
                                    fMaxX = currentNode.X;
                            }
                            float fCenter = (fMaxX - fMinX) / 2;
                            node.X = fMinX;
                            node.X = node.X + fCenter;
                            foreach (GraphNode child in node.Children)
                            {
                                child.X = child.X + fCenter + node.Distance;
                                child.Distance = node.Distance;
                                foreach (GraphNode innerChild in child.Children)
                                {
                                    innerChild.Distance = fCenter;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTreeToTop(ArrayList hashSorting, int nRank)
        {
            int nCounter = nRank;

            while (nCounter >= 0)
            {
                ArrayList hashToSort = hashSorting[nCounter] as ArrayList;
                MakeXLayoutRelative(hashSorting, hashToSort, true, nCounter);
                nCounter--;
            }

            // if( hashSorting.Count == nRank + 2 )
            // {
            //     ArrayList hashToSort = hashSorting[ nRank + 1 ] as ArrayList;
            //     MakeXLayoutRelative( hashSorting, hashToSort, false, nRank + 1 );
            // }
        }

        private void MakeXLayoutRelative(ArrayList lstRankedNodes, ArrayList lstToSort, bool bUpdateTree, int nRank)
        {
            if (lstToSort == null)
                return;

            ArrayList lstToNormalize = lstToSort;

            foreach (GraphNode node in lstToSort)
            {
                if (bUpdateTree)
                {
                    PlaceRelativeToChildren(node.Children);
                }
                else if (nRank > 0)
                {
                    PlaceRelativeToParent(node.Parents);
                }
                NormalizeNodes(lstToNormalize);
            }
            
        }

        /// <summary>
        /// Places the parents to its children.
        /// </summary>
        /// <param name="lstRelatives">The relatives list.</param>
        private void PlaceRelativeToChildren(ICollection lstRelatives)
        {
            float fLeftChildPosition = 0;
            float fRightChildPosition = 0;

            ArrayList lstParents = new ArrayList();

            foreach (GraphNode node in lstRelatives)
            {
                foreach (GraphNode parent in node.Parents)
                {
                    if (!lstParents.Contains(parent))
                        lstParents.Add(parent);
                }
            }

            foreach (GraphNode parent in lstParents)
            {
                ArrayList lstChildren = parent.Children;
                int nCount = lstChildren.Count;

                if (nCount > 0)
                {
                    GraphNode gnNode = (GraphNode)lstChildren[0];

                    ArrayList lstChildParents = gnNode.Parents;
                    float fParentsCenter = gnNode.X + gnNode.Width / 2;
                    float fParetnsWidth = GetNodesWidth(lstParents);

                    // if( !( nCount == 1 && lstParents.Count > 1 ) )
                    {
                        IGBounds nodeLeft = (IGBounds)lstChildren[0];
                        IGBounds nodeRight = nodeLeft;
                        IGBounds bounds;

                        for (int n = 1; n < nCount; n++)
                        {
                            bounds = (IGBounds)lstChildren[n];

                            if (nodeLeft.X > bounds.X)
                                nodeLeft = bounds;

                            if (nodeRight.X < bounds.X)
                                nodeRight = bounds;
                        }

                        fLeftChildPosition = nodeLeft.X;
                        fRightChildPosition = nodeRight.X + nodeRight.Width;

                        float fMoveTo = (fLeftChildPosition + (fRightChildPosition - fLeftChildPosition) / 2) - parent.Width / 2;
                        float fOffset = fMoveTo - parent.X;
                        parent.X = fMoveTo;

                        // MoveLeftRelatives( node, fOffset );
                    }
                }
            }
        }

        /// <summary>
        /// Places the children to its parents.
        /// </summary>
        /// <param name="lstParents">Array list of parents.</param>
        private void PlaceRelativeToParent(ArrayList lstParents)
        {
            float fParentMedian = 0;
            float nPosition = 0;
            float nChildrenWidth = 0;

            ArrayList lstChildren;

            foreach (GraphNode nodeParent in lstParents)
            {
                fParentMedian = nodeParent.X + (nodeParent.Width / 2);
                nChildrenWidth = -this.HorizontalSpacing;

                lstChildren = nodeParent.Children;

                foreach (GraphNode node in lstChildren)
                {
                    nChildrenWidth += node.Width + this.HorizontalSpacing;
                }

                nPosition = -(nChildrenWidth) / 2;

                foreach (GraphNode node in lstChildren)
                {
                    node.X = fParentMedian + nPosition;
                    node.Visited = true;
                    nPosition += node.Width;
                }

                foreach (GraphNode node in lstChildren)
                {
                    ArrayList parents = node.Parents;
                    if (parents.Count > 1)
                    {
                        float fX = GetCenterXPosition(parents);
                        node.X = fX - node.Width / 2;
                    }
                }
            }
        }
        private float GetNodesWidth(ArrayList list)
        {
            float fWidth = 0;
            int nCount = list.Count;

            if (nCount > 0)
            {
                GraphNode gnFirstNode = (GraphNode)list[0];
                GraphNode gnLastNodet = (GraphNode)list[nCount - 1];

                fWidth = gnLastNodet.Bounds.Right - gnFirstNode.Bounds.Left;
            }

            return fWidth;
        }
        private float GetCenterXPosition(ArrayList list)
        {
            float fCenter = 0;
            if (this.m_parentPosition == ParentPositions.Center)
            {
                GraphNode startNode = (GraphNode)list[0];
                float fMinX = startNode.X;
                float fMaxX = startNode.X;
                for (int i = 0; i < list.Count; i++)
                {
                    GraphNode currentNode = list[i] as GraphNode;
                    if (currentNode.X < fMinX)
                        fMinX = currentNode.X;
                    if (currentNode.X > fMaxX)
                        fMaxX = currentNode.X;
                }
                fCenter = (fMaxX - fMinX) / 2;
            }
            else
            {
                float fWidth = 0;
                int nCount = list.Count;
                if (nCount > 0)
                {
                    GraphNode gnFirstNode = (GraphNode)list[0];
                    GraphNode gnLastNodet = (GraphNode)list[nCount - 1];

                    fWidth = gnLastNodet.Bounds.Right - gnFirstNode.Bounds.Left;
                    fCenter = gnFirstNode.X + fWidth / 2f;
                }
            }
            return fCenter;
        }
        private ArrayList GetChildByParentCount(ArrayList lstChildren, int nParentCount)
        {
            ArrayList lstToReturn = new ArrayList();

            foreach (GraphNode gnChild in lstChildren)
            {
                if (gnChild.Parents.Count >= nParentCount)
                    lstToReturn.Add(gnChild);
            }

            return lstToReturn;
        }

        private void MoveLeftRelatives(IGBounds node, float fOffset)
        {
            if (node is GraphNode)
            {
                GraphNode dtgnNode = node as GraphNode;

                foreach (GraphNode dtgnParent in dtgnNode.Parents)
                {
                    foreach (IGBounds nodeChild in dtgnParent.Children)
                    {
                        if (!node.Equals(nodeChild))
                            break;

                        nodeChild.X += fOffset;
                        MoveChildren(nodeChild as IGBounds, fOffset);
                    }
                }
            }
        }

        private bool NormalizeNodes(ArrayList nodes)
        {
            bool bSuccess = false;
            float nodeBound = 0;

            for (int nCounter = 0; nCounter < nodes.Count - 1; nCounter++)
            {
                GraphNode node = nodes[nCounter] as GraphNode;
                GraphNode nodeNext = nodes[nCounter + 1] as GraphNode;

                nodeBound = node.X + node.Width + this.HorizontalSpacing;

                if (nodeBound > nodeNext.X)
                {
                    float fOffset = nodeBound - nodeNext.X;
                    nodeNext.X = nodeBound;
                    bSuccess = true;

                    //MoveChildren(nodeNext, fOffset);
                }
            }
            return bSuccess;
        }

        private void MoveChildren(IGBounds nodeNext, float fOffset)
        {
            // Move children
            if (nodeNext is GraphNode)
            {
                GraphNode dtgnNodeNext = nodeNext as GraphNode;

                foreach (GraphNode gnChild in dtgnNodeNext.Children)
                {
                    gnChild.X += fOffset;
                }
            }
        }

        /// <summary>
        /// Does the init X layout.
        /// </summary>
        /// <param name="graphToSort">The graph to sort.</param>
        protected void DoInitXLayout(Graph graphToSort)
        {
            ArrayList lstToSort = graphToSort.TypeOrdered;
            int nCounter = 0;

            foreach (ArrayList lstSort in lstToSort)
            {
                MakeLayout(lstSort, nCounter, graphToSort);
                nCounter++;
            }
        }

        /// <summary>
        /// X coordinate
        /// </summary>
        /// <param name="gnBounds">The bounds.</param>
        /// <param name="fX">The X coordinate.</param>
        private void XCoordRoutine(IGBounds gnBounds, ref float fX)
        {
            gnBounds.X = fX;
            fX += gnBounds.Width + this.HorizontalSpacing;
            gnBounds.X += this.HorizontalSpacing;
        }

        /// <summary>
        /// Makes the layout.
        /// </summary>
        /// <param name="lstRank">The LST rank.</param>
        /// <param name="nRank">The n rank.</param>
        /// <param name="graph">The graph.</param>
        private void MakeLayout(ArrayList lstRank, int nRank, Graph graph)
        {
            float fX = this.HorizontalSpacing;

            foreach (IGBounds bounds in lstRank)
            {
                if (bounds == null)
                    throw new ArgumentNullException("NULL");

                XCoordRoutine(bounds, ref fX);
            }

            // ============== Y Coordinate Routine ================
            float fYRank = CalcRankPositionVerticalLayout(nRank, graph);
            YCoordinateRoutine(lstRank, fYRank);
        }

        /// <summary>
        /// Calcs the rank position vertical layout.
        /// </summary>
        /// <param name="nRank">The n rank.</param>
        /// <param name="graph">The graph.</param>
        /// <returns>The value.</returns>
        private float CalcRankPositionVerticalLayout(int nRank, Graph graph)
        {
            float fRankPositionToReturn = 0;
            SizeF szRankDimensions = (SizeF)graph.BiggestDimensions[nRank];

            if (nRank == 0)
                fRankPositionToReturn = szRankDimensions.Height / 2;
            else
            {
                float fGraphHeightTillCurrentRank = GetSubGraphDimension(0, nRank, Dimension.Height, graph);
                fRankPositionToReturn = fGraphHeightTillCurrentRank + szRankDimensions.Height / 2;
            }

            return fRankPositionToReturn + m_fYBorder;
        }

        /// <summary>
        /// Gets the sub graph dimension.
        /// </summary>
        /// <param name="nFromRank">The n from rank.</param>
        /// <param name="nToRank">The n to rank.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="graph">The graph.</param>
        /// <returns>The value</returns>
        protected float GetSubGraphDimension(int nFromRank, int nToRank, Dimension dimension, Graph graph)
        {
            float fGraphDimensionToReturn = 0;
            int nCounter = nFromRank;
            int nIncrease = 1;

            if (nFromRank > nToRank)
                nIncrease = -nIncrease;

            while (nCounter != nToRank)
            {
                SizeF szRankBiggestDimensions = (SizeF)graph.BiggestDimensions[nCounter];

                switch (dimension)
                {
                    case Dimension.Height:
                        fGraphDimensionToReturn += szRankBiggestDimensions.Height;
                        fGraphDimensionToReturn += this.VerticalSpacing;
                        break;
                    case Dimension.Width:
                        fGraphDimensionToReturn += szRankBiggestDimensions.Width;
                        fGraphDimensionToReturn += this.HorizontalSpacing;
                        break;
                }
                nCounter += nIncrease;
            }

            return fGraphDimensionToReturn;
        }

        private void YCoordinateRoutine(ArrayList lstRank, float fYRank)
        {
            foreach (object entry in lstRank)
            {
                IGBounds bounds = entry as IGBounds;
                bounds.Y = fYRank - bounds.Height / 2;
            }
        }

        #endregion Helper Methods

        #region FictitiousNode methods
        private void BreakConnectors(ArrayList typeOrdered)
        {
            ArrayList lstNodePassed = new ArrayList();
            ArrayList lstNodesChain = new ArrayList();

            foreach (ArrayList lstRank in typeOrdered)
            {
                foreach (GraphNode gnNode in lstRank)
                {
                    // find fictitious nodes
                    if (gnNode.Node == null && !lstNodePassed.Contains(gnNode))
                    {
                        lstNodePassed.Add(gnNode);
                        lstNodesChain.Clear();

                        // get fict nodes chain and chain connector
                        GenerateFictitiousNodeChain(gnNode, lstNodesChain);
                        ConnectorBase chainConnector = GetChainConnector(lstNodesChain) as ConnectorBase;

                        if (lstNodesChain.Count > 2)
                        {
                            GraphNode gnStart = lstNodesChain[0] as GraphNode;
                            GraphNode gnEnd = lstNodesChain[lstNodesChain.Count - 1] as GraphNode;

                            // get reference to model. Only first and last graphNode has node
                            Model model = gnStart.Node.Root;
                            ConnectorBase line = CreateBrokenConnector(lstNodesChain, chainConnector);

                            if (line != null)
                            {
                                // remove old connector from model
                                if (chainConnector != null)
                                {
                                    gnStart.Node.CentralPort.Disconnect(chainConnector.TailEndPoint);
                                    gnEnd.Node.CentralPort.Disconnect(chainConnector.HeadEndPoint);
                                    
                                    // model.RemoveChild( chainConnector );
                                }

                                // add new connector to model
                                // model.AppendChild( line );
                                // connect new connector
                                gnStart.Node.CentralPort.TryConnect(line.TailEndPoint);
                                gnEnd.Node.CentralPort.TryConnect(line.HeadEndPoint);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get endpoint container in generate list nodes chain.
        /// </summary>
        /// <param name="lstNodesChain">List nodes chain generated in GenerateFictitiousNodeChain() method</param>
        /// <returns>The end point container.</returns>
        private IEndPointContainer GetChainConnector(ArrayList lstNodesChain)
        {
            IEndPointContainer connector = null;

            // fict node can be only between start and end node if chain list
            // [0] - start node; [1 ;(n-1) ] - fict node; [ n ] - end node
            if (lstNodesChain.Count > 2)
            {
                GraphNode gnStartNode = lstNodesChain[0] as GraphNode;
                GraphNode gnEndNode = lstNodesChain[lstNodesChain.Count - 1] as GraphNode;
                
                // get first fict node
                GraphNode gnFirstFictNode = lstNodesChain[1] as GraphNode;

                if (gnStartNode == null || gnStartNode.Node == null ||
                    gnFirstFictNode == null || gnEndNode == null || gnEndNode.Node == null)
                    throw new ArgumentException("Chain list generate not correct.");

                // find old connector
                foreach (ConnectionPoint port in gnStartNode.Node.Ports)
                {
                    foreach (EndPoint endPoint in port.Connections)
                    {
                        connector = endPoint.Container as IEndPointContainer;
                        EndPoint secondEndPoint = (endPoint is HeadEndPoint) ? connector.TailEndPoint : connector.HeadEndPoint;

                        if (secondEndPoint.Port.Container != gnEndNode.Node)
                        {
                            // reset connector
                            connector = null;
                        }
                        else
                        {
                            // return finded connector
                            return connector;
                        }
                    }
                }

                // end iterate ports
            }

            return connector;
        }

        /// <summary>
        /// Create broken connector from source connector and nodes chain collection,
        /// where first and last node - nodes what must to be connected.
        /// </summary>
        /// <param name="lstNodesChain">Nodes chain collection. Can be generate in GenerateFictitiousNodeChain().</param>
        /// <param name="src">The source node what will broken.</param>
        /// <returns>Broken line connector.</returns>
        private ConnectorBase CreateBrokenConnector(ArrayList lstNodesChain, ConnectorBase src)
        {
            // line to return
            LineConnector line = null;

            if (lstNodesChain.Count > 2)
            {
                // get start and end nodes what need to be connected
                GraphNode gnStart = lstNodesChain[0] as GraphNode;
                GraphNode gnEnd = lstNodesChain[lstNodesChain.Count - 1] as GraphNode;

                // create line connector
                line = new LineConnector(gnStart.Location, gnEnd.Location);

                // get invert connector transform
                System.Drawing.Drawing2D.Matrix mtxTransform = line.GetTransformations();
                line.AppendFlipTransforms(mtxTransform);
                mtxTransform.Invert();

                // broke line by add point from fictitious node
                for (int i = 1, nLength = lstNodesChain.Count - 1; i < nLength; i++)
                {
                    GraphNode gnFict = lstNodesChain[i] as GraphNode;

                    PointF[] pts = new PointF[] { gnFict.Center };
                    mtxTransform.TransformPoints(pts);

                    line.InsertPoint(i, pts[0]);
                }
            }

            return line;
        }

        /// <summary>
        /// Get line nodes chain where between start and end nodes must be broken connector.
        /// </summary>
        /// <param name="gnStartFictitiousNode">The graph node.</param>
        /// <param name="lstNodesChain">Line nodes list.</param>
        private void GenerateFictitiousNodeChain(GraphNode gnStartFictitiousNode, ArrayList lstNodesChain)
        {
            if (gnStartFictitiousNode == null || lstNodesChain == null)
                return;

            lstNodesChain.Add(gnStartFictitiousNode);

            // fictitious node can contain only one parent and one children
            // because it is created between two connected nodes
            if (gnStartFictitiousNode.Parents.Count > 0)
            {
                GraphNode gnParent = gnStartFictitiousNode.Parents[0] as GraphNode;

                if (!lstNodesChain.Contains(gnParent))
                {
                    // if parent node is fictitious node continue fill collection
                    if (gnParent.Node == null)
                    {
                        GenerateFictitiousNodeChain(gnParent, lstNodesChain);
                    }
                    else
                    {
                        lstNodesChain.Insert(0, gnParent);
                    }
                }
            }

            if (gnStartFictitiousNode.Children.Count > 0)
            {
                GraphNode gnChild = gnStartFictitiousNode.Children[0] as GraphNode;

                if (!lstNodesChain.Contains(gnChild))
                {
                    // if child node is fictitious node continue fill collection
                    if (gnChild.Node == null)
                    {
                        GenerateFictitiousNodeChain(gnChild, lstNodesChain);
                    }
                    else
                    {
                        lstNodesChain.Add(gnChild);
                    }
                }
            }
        }
        #endregion
    }
}