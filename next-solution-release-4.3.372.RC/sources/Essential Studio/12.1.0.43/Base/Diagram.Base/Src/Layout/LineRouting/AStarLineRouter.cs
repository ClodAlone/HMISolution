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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Line Routing engine that use A* find path Algorithm to route orthogonal lines.
    /// </summary>
    [Serializable]
    [ToolboxItem(false)]
    public class AStarLineRouter
        : LineRouter
    {
        #region Class members
        private SearchNodeArray m_processedNodes;

        /// <summary>
        /// Store Search Grid.
        /// </summary>
        protected SearchGridNode[,] m_searchGrid;

        /// <summary>
        /// Store search node.
        /// </summary>
        protected SearchNodeArray m_lstSearchNodes;

        /// <summary>
        /// Helper class used while path find.
        /// </summary>
        private ArrayList m_lstObstacles;

        /// <summary>
        /// Store collection of the connectors to route.
        /// </summary>
        private NodeCollection m_nodesConnectors;

        /// <summary>
        /// Store region that represent all obstacles.
        /// </summary>
        private Region m_rgnObstacles;

        /// <summary>
        /// Store model bounds.
        /// </summary>
        private RectangleF m_modelBounds;

        /// <summary>
        /// Store search grid's columns count.
        /// </summary>
        private int m_columnsCount;

        /// <summary>
        /// Store search grid's rows count.
        /// </summary>
        private int m_rowsCount;
        #endregion

        #region Initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarLineRouter"/> class.
        /// </summary>
        public AStarLineRouter()
            : this(null)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AStarLineRouter"/> class.
        /// </summary>
        /// <param name="model">The reference to diagram model.</param>
        public AStarLineRouter(Model model)
            : base(model)
        {
            m_lstSearchNodes = new SearchNodeArray(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AStarLineRouter"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected AStarLineRouter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_lstSearchNodes = new SearchNodeArray(null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the reference to collection of connectors that will be routed by engine.
        /// </summary>
        /// <value>The reference to routing connector collection.</value>
        protected NodeCollection RoutingConnectors
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

        #region Overrides
        /// <summary>
        /// Reroutes all available model connectors.
        /// </summary>
        /// <remarks>
        /// Call ResetSearchCache() and Reroute() method.
        /// </remarks>
        protected override void RouteAllModelConnectorsInternal()
        {
            ResetSearchCache();
            Reroute();
        }

        /// <summary>
        /// Route given collection of connectors.
        /// </summary>
        /// <param name="connectors">Collections to route.</param>
        /// <remarks>
        /// Modified RoutingConnectors collection property.
        /// </remarks>
        protected override void RouteConnectorsInternal(ICollection connectors)
        {
            ResetSearchCache();
            
            // update RoutingConnectors collection
            this.RoutingConnectors.Clear();
            this.RoutingConnectors.AddRange(connectors);

            Reroute();
        }

        /// <summary>
        /// Route given connection.
        /// </summary>
        /// <param name="connector">Connection to route.</param>
        /// <remarks>
        /// Modified RoutingConnectors collection property.
        /// </remarks>
        protected override void RouteConnectorInternal(ConnectorBase connector)
        {
            ResetSearchCache();
            
            // update RoutingConnectors collection
            this.RoutingConnectors.Clear();
            this.RoutingConnectors.Add(connector);

            Reroute();
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Recreate obstacles collections and generate search grid to find all possible ways.
        /// </summary>
        /// <remarks>
        /// Collect all obstacles ( node with enable Node.TreatAsObstacle property ) in model and save it to m_lstObstacles member.
        /// Then generate search grid by calling InitSerarchGrid() that used last obstacles collection.
        /// </remarks>
        public void ResetSearchCache()
        {
            // recreate search grid
            // --------------------
            // get model nodes bounding rectangle - !!non line based!!
            m_lstObstacles = GetObstacles();
            
            // init search grid
            InitSearchGrid();
        }

        /// <summary>
        /// Reroutes connectors from RoutingConnectors collection.
        /// </summary>
        /// <remarks>
        /// Engine find short way and lay out connectors using SetPoints() method.
        /// </remarks>
        protected void Reroute()
        {
            this.Model.BridgeManager.BeginUpdateIntersection();
            this.Model.LinkManager.BeginSynchronization();

            // Array of points containing new connector shape.
            PointF[] ptsNew;
            
            // reroute node's edges
            foreach (ConnectorBase connector in this.RoutingConnectors)
            {
                // find new route from start point to end point
                ptsNew = FindPath(connector.HeadEndPoint, connector.TailEndPoint);

                if (ptsNew != null)
                {
                    // update connector with new points
                    connector.SetPoints(ptsNew, false);
                    
                    // synchronize end point
                    this.Model.LinkManager.SynchronizeEndPoint(connector.HeadEndPoint);
                }
            }

            this.RoutingConnectors.Clear();

            this.Model.BridgeManager.EndUpdateIntersection();
            this.Model.LinkManager.EndSynchronization();
        }

        /// <summary>
        /// Adds nodeToConnectWith to Neighbours collection of nodeToConnect.
        /// </summary>
        /// <param name="nodeToConnect">Node to add neighbour to.</param>
        /// <param name="nodeToConnectWith">Neighbour to add.</param>
        protected void ConnectNode(SearchNode nodeToConnect, SearchNode nodeToConnectWith)
        {
            if (!nodeToConnect.Neighbours.Contains(nodeToConnectWith))
                nodeToConnect.Neighbours.Add(nodeToConnectWith);

            if (!nodeToConnectWith.Neighbours.Contains(nodeToConnect))
                nodeToConnectWith.Neighbours.Add(nodeToConnect);
        }

        /// <summary>
        /// Removes neighbour node from nodeDisconnectFrom Neighbours.
        /// </summary>
        /// <param name="nodeDisconnectFrom">Node to remove neighbour from.</param>
        /// <param name="nodeDisconnecting">Neighbour to remove.</param>
        protected void DisconnectNode(SearchNode nodeDisconnectFrom, SearchNode nodeDisconnecting)
        {
            if (nodeDisconnectFrom.Neighbours.Contains(nodeDisconnecting))
                nodeDisconnectFrom.Neighbours.Remove(nodeDisconnecting);
        }

        /// <summary>
        /// Clear neighbours.
        /// </summary>
        /// <param name="lstDisconnecting">Nodes to clear neighbours</param>
        protected void DisconnectNodes(SearchNodeArray lstDisconnecting)
        {
            foreach (SearchNode node in lstDisconnecting)
            {
                foreach (SearchNode nodeNeighbour in node.Neighbours)
                {
                    if (nodeNeighbour.Neighbours.Contains(node))
                        nodeNeighbour.Neighbours.Remove(node);
                }

                node.Neighbours.Clear();
            }
        }

        /// <summary>
        /// Gets the search node array that intersect with rcBounds rectangle.
        /// </summary>
        /// <param name="ptEndPointLocation">The tested EndPoint location.</param>
        /// <param name="rcBounds">The bounds rectangle.</param>
        /// <param name="heading">The tested EndPoint heading.</param>
        /// <returns>The search node array.</returns>
        protected SearchNodeArray GetBoundaryIntersects(PointF ptEndPointLocation, RectangleF rcBounds, CompassHeading heading)
        {
            float fRight = (float)Math.Round(rcBounds.X + rcBounds.Width, 2);
            float fBottom = (float)Math.Round(rcBounds.Y + rcBounds.Height, 2);

            SearchNodeArray lstToReturn = new SearchNodeArray(null);            
            ptEndPointLocation = new PointF((float)Math.Round(ptEndPointLocation.X, 2), (float)Math.Round(ptEndPointLocation.Y, 2));
            rcBounds = new RectangleF((float)Math.Round(rcBounds.X, 2), (float)Math.Round(rcBounds.Y, 2), (float)Math.Round(rcBounds.Width, 2), (float)Math.Round(rcBounds.Height, 2));
            if (!rcBounds.Contains(ptEndPointLocation) || ptEndPointLocation.X == rcBounds.X 
                || ptEndPointLocation.Y == rcBounds.Y || ptEndPointLocation.X == fRight || ptEndPointLocation.Y == fBottom)
            {
                SearchNode node = CreateSearchNode(ptEndPointLocation);
                lstToReturn.Add(node);
            }
            else
            {
                PointF[] ptIntsct = Geometry.GetBoundaryIntercepts(rcBounds, ptEndPointLocation, heading);
                SearchNode nodeTemp;
                for (int i = 0, nLength = ptIntsct.Length; nLength > i; i++)
                {
                    nodeTemp = CreateSearchNode(ptIntsct[i]);

                    lstToReturn.Add(nodeTemp);
                }
            }

            return lstToReturn;
        }

        /// <summary>
        /// Creates the search node in given location.
        /// </summary>
        /// <param name="ptLoc">The search node location.</param>
        /// <returns><see cref="Syncfusion.Windows.Forms.Diagram.SearchNode" /> instance in given location.</returns>
        protected virtual SearchNode CreateSearchNode(PointF ptLoc)
        {
            return new SearchGridNode(ptLoc);
        }

        /// <summary>
        /// Gets intersection points of given search node with search grid.
        /// </summary>
        /// <param name="node">The search node.</param>
        /// <param name="endPoint">The end point to check.</param>
        /// <returns>The search node array.</returns>
        protected SearchNodeArray GetNearestSearchGridNodes(SearchNode node, EndPoint endPoint)
        {
            ConnectorBase conn = (ConnectorBase)endPoint.Container;
            
            // connected node bounds
            RectangleF rcBounds = RectangleF.Empty;

            if (endPoint.Port != null && endPoint.Port.Container != null)
            {
                Node nodeCtnr = GetTopParentNode(endPoint.Port.Container);
                SizeF szTemp =
                    MeasureUnitsConverter.Convert(nodeCtnr.Size, nodeCtnr.MeasurementUnit, MeasureUnits.Pixel);

                Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(nodeCtnr, true);

                PointF[] pts = new PointF[]
                {
                    PointF.Empty,
                    new PointF( szTemp.Width, 0 ),
                    new PointF( 0, szTemp.Height ),
                    new PointF( szTemp.Width, szTemp.Height )
                };

                mtxTemp.TransformPoints(pts);

                rcBounds = Geometry.CreateRect(pts);
                rcBounds.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);
            }

            CompassHeading heading = (endPoint is HeadEndPoint) ? conn.HeadingHead : conn.HeadingTail;
            
            // get EndPoint container boundary intersect points
            SearchNodeArray lstBI = GetBoundaryIntersects(node.Location, rcBounds, heading);

            SearchNodeArray nodesToReturn = new SearchNodeArray(null);
            float fMinDistance;
            float fDistance;
            PointF ptTo;
            SearchNode nodeNearest;

            foreach (SearchNode nodeCur in lstBI)
            {
                // connect to start/end SearchNodes
                ConnectNode(nodeCur, node);

                ptTo = nodeCur.Location;
                fMinDistance = float.MaxValue;
                nodeNearest = null;
                SearchNodeArray lst = new SearchNodeArray(null);

                int row = 0;
                int tempIndex = 0;

                for (int i = 0; i < m_rowsCount; i++)
                {
                    tempIndex = 0;

                    do
                    {
                        if (m_searchGrid[i, tempIndex] != null)
                        {
                            fDistance = Math.Abs(m_searchGrid[i, tempIndex].Location.Y - ptTo.Y);

                            if (fDistance < fMinDistance && !lst.Contains(m_searchGrid[i, tempIndex]))
                            {
                                fMinDistance = fDistance;
                                nodeNearest = m_searchGrid[i, tempIndex];
                                row = i;
                            }
                        }

                        tempIndex++;

                        if (tempIndex >= m_columnsCount)
                            break;
                    } 
                    while (m_searchGrid[i, tempIndex - 1] == null);
                }

                int col = 0;
                fMinDistance = float.MaxValue;

                for (int i = 0; i < m_columnsCount; i++)
                {
                    tempIndex = 0;

                    do
                    {
                        if (m_searchGrid[tempIndex, i] != null)
                        {
                            fDistance = Math.Abs(m_searchGrid[tempIndex, i].Location.X - ptTo.X);

                            if (fDistance < fMinDistance && !lst.Contains(m_searchGrid[tempIndex, i]))
                            {
                                fMinDistance = fDistance;
                                nodeNearest = m_searchGrid[tempIndex, i];
                                col = i;
                            }
                        }

                        tempIndex++;

                        if (tempIndex >= m_rowsCount)
                            break;
                    } 
                    while (m_searchGrid[tempIndex - 1, i] == null);
                }
                if (m_searchGrid.Length != 0)
                {
                    nodeNearest = m_searchGrid[row, col];
                }

                // connect grid node to nearest boundary intersecting node
                if (nodeNearest != null)
                {
                    lst.Add(nodeNearest);

                    PointF pt1 = nodeNearest.Location;
                    pt1.X = (float)Math.Round(pt1.X, 2);
                    pt1.Y = (float)Math.Round(pt1.Y, 2);

                    PointF pt2 = nodeCur.Location;
                    pt2.X = (float)Math.Round(pt2.X, 2);
                    pt2.Y = (float)Math.Round(pt2.Y, 2);

                    if (pt1 == pt2)
                    {
                        ConnectNode(node, nodeNearest);

                        if (!nodesToReturn.Contains(node))
                            nodesToReturn.Add(node);
                    }
                    else
                    {
                        // simply connect nodes
                        if ((pt1.X != pt2.X)
                            && (pt1.Y != pt2.Y))
                        {
                            PointF ptTmp = new PointF(nodeNearest.Location.X, nodeCur.Location.Y);
                            SearchNode nodeTmp;

                            if (!rcBounds.Contains(ptTmp))
                            {
                                nodeTmp = CreateSearchNode(ptTmp);

                                ConnectNode(node, nodeTmp);
                                ConnectNearestNodeNeighbour(nodeTmp, nodeNearest);
                                nodesToReturn.Add(nodeTmp);
                            }
                            else
                            {
                                ptTmp = new PointF(nodeCur.Location.X, nodeNearest.Location.Y);

                                if (!rcBounds.Contains(ptTmp))
                                {
                                    nodeTmp = CreateSearchNode(ptTmp);

                                    ConnectNode(node, nodeTmp);
                                    ConnectNearestNodeNeighbour(nodeTmp, nodeNearest);
                                    nodesToReturn.Add(nodeTmp);
                                }
                            }
                        }
                        else
                        {
                            ConnectNode(nodeCur, nodeNearest);
                        }
                    }
                }
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Connects the nearest node to Neighbour collection of nodeCur.
        /// </summary>
        /// <param name="nodeCur">The Neighbour collection container.</param>
        /// <param name="nodeNearest">The nearest search node.</param>
        private void ConnectNearestNodeNeighbour(SearchNode nodeCur, SearchNode nodeNearest)
        {
            foreach (SearchNode nodeSrch in nodeNearest.Neighbours)
            {
                if (nodeCur.Location.Y == nodeNearest.Location.Y
                    && nodeCur.Location.X > nodeSrch.Location.X
                    && nodeCur.Location.X < nodeNearest.Location.X)
                {
                    ConnectNode(nodeSrch, nodeCur);
                    break;
                }
                else if (nodeCur.Location.Y == nodeNearest.Location.Y
                    && nodeCur.Location.X < nodeSrch.Location.X
                    && nodeCur.Location.X > nodeNearest.Location.X)
                {
                    ConnectNode(nodeSrch, nodeCur);
                    break;
                }
                else if (nodeCur.Location.X == nodeNearest.Location.X
                    && nodeCur.Location.Y < nodeSrch.Location.Y
                    && nodeCur.Location.Y > nodeNearest.Location.Y)
                {
                    ConnectNode(nodeSrch, nodeCur);
                    break;
                }
                else if (nodeCur.Location.X == nodeNearest.Location.X
                    && nodeCur.Location.Y > nodeSrch.Location.Y
                    && nodeCur.Location.Y < nodeNearest.Location.Y)
                {
                    ConnectNode(nodeSrch, nodeCur);
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize search grid using collected obstacles.
        /// </summary>
        /// <remarks>
        /// Generate search grid from <see cref ="Syncfusion.Windows.Forms.Diagram.SearchGridNode"/>  
        /// using m_lstObstacles member as obstacles collection.
        /// </remarks>
        protected void InitSearchGrid()
        {
            m_modelBounds = MeasureUnitsConverter.Convert(this.Model.Bounds, this.Model.MeasurementUnits, MeasureUnits.Pixel);

            // current obstacle
            Obstacle obstacle;
            RectangleF rcObstacle;
            float nTemp;
            //// sorted list for X-axis values
            SortedList lstX = new SortedList();
            //// sorted list for Y-axis values
            SortedList lstY = new SortedList();
            //// search grid prepare routine
            for (int i = 0, nLength = m_lstObstacles.Count; nLength > i; i++)
            {
                // get current obstacle
                obstacle = (Obstacle)m_lstObstacles[i];
                rcObstacle = obstacle.BoundingRect;
                
                // X-axis values
                // -------------
                // Left
                nTemp = rcObstacle.X;

                if (!lstX.ContainsKey(nTemp))
                    lstX.Add(nTemp, null);

                // Right
                nTemp = rcObstacle.Right;

                if (!lstX.ContainsKey(nTemp))
                    lstX.Add(nTemp, null);

                // Y-axis values
                // -------------
                // Top
                nTemp = rcObstacle.Y;

                if (!lstY.ContainsKey(nTemp))
                    lstY.Add(nTemp, null);

                // Bottom
                nTemp = rcObstacle.Bottom;

                if (!lstY.ContainsKey(nTemp))
                    lstY.Add(nTemp, null);

                // if node has edges -> add center points to search grid
                if (obstacle.HasCentralPort)
                {
                    nTemp = rcObstacle.X + rcObstacle.Width / 2;

                    if (!lstX.ContainsKey(nTemp))
                        lstX.Add(nTemp, null);

                    nTemp = rcObstacle.Y + rcObstacle.Height / 2;

                    if (!lstY.ContainsKey(nTemp))
                        lstY.Add(nTemp, null);
                }
            }

            m_rowsCount = lstY.Count;
            m_columnsCount = lstX.Count;
            float fY;
            PointF ptTemp;
            
            // create search grid vertices
            m_searchGrid = new SearchGridNode[m_rowsCount, m_columnsCount];
            m_lstSearchNodes.Clear();
            SearchGridNode node;

            for (int i = 0; i < m_rowsCount; i++)
            {
                fY = (float)lstY.GetKey(i);

                for (int k = 0; k < m_columnsCount; k++)
                {
                    ptTemp = new PointF((float)lstX.GetKey(k), fY);

                    // check Model.BoundaryConstrains flag
                    if (IsValid(ptTemp))
                    {
                        node = new SearchGridNode(ptTemp);
                        m_lstSearchNodes.Add(node);
                        m_searchGrid[i, k] = node;
                    }
                    else
                    {
                        m_searchGrid[i, k] = null;
                    }
                }
            }

            // update grid node's neighbours connections
            int nCurRow = 0;
            while (nCurRow < m_rowsCount)
            {
                for (int nCurCol = 0; m_columnsCount > nCurCol; nCurCol++)
                {
                    if (m_columnsCount > (nCurCol + 1))
                    {
                        UpdateNeighbours(nCurRow, nCurCol, nCurRow, nCurCol + 1);
                    }

                    if ((nCurRow + 1) < m_rowsCount)
                    {
                        UpdateNeighbours(nCurRow, nCurCol, nCurRow + 1, nCurCol);
                    }
                }

                nCurRow++;
            }
        }

        /// <summary>
        /// Update nodes neighbours.
        /// </summary>
        /// <param name="nXNode1">The X axis position of first node.</param>
        /// <param name="nYNode1">The Y axis position of first node.</param>
        /// <param name="nXNode2">The X axis position of second node.</param>
        /// <param name="nYNode2">The Y axis position of second node.</param>
        protected void UpdateNeighbours(int nXNode1, int nYNode1, int nXNode2, int nYNode2)
        {
            SearchNode node1 = m_searchGrid[nXNode1, nYNode1];

            if (node1 != null)
            {
                SearchNode node2 = m_searchGrid[nXNode2, nYNode2];

                if (node2 != null && CanConnect(node1, node2))
                {
                    if (!node1.Neighbours.Contains(node2))
                        node1.Neighbours.Add(node2);

                    if (!node2.Neighbours.Contains(node1))
                        node2.Neighbours.Add(node1);
                }
            }
        }

        /// <summary>
        /// Check if two search nodes can be connected.
        /// </summary>
        /// <param name="node1">First node to check.</param>
        /// <param name="node2">Second node to check.</param>
        /// <returns>true, if can connect to the nodes.</returns>
        protected virtual bool CanConnect(SearchNode node1, SearchNode node2)
        {
            float centerX;
            float centerY;

            float p1X = node1.Location.X;
            float p2X = node2.Location.X;

            float p1Y = node1.Location.Y;
            float p2Y = node2.Location.Y;

            centerX = (p1X == p2X) ? p1X : (p1X + p2X) / 2;
            centerY = (p1Y == p2Y) ? p1Y : (p1Y + p2Y) / 2;

            return !m_rgnObstacles.IsVisible(centerX, centerY);

            // bool bSuccess = true;
            // // check whether line formed with node1.Location
            // // and node2.Location intersects with one obstacle.
            // foreach( Obstacle1 obstacle in m_lstObstacles )
            // {
            //      if( Geometry.RectIntersectsWithLineSegment( obstacle.BoundingRect, node1.Location, node2.Location ) )
            //      {
            //            bSuccess = false;
            //            break;
            //      }
            // }
            //
            // return bSuccess;
        }

        /// <summary>
        /// Gets value indicates that model's bounds constrains given point.
        /// </summary>
        /// <param name="ptTesting">Point to check.</param>
        /// <returns>TRUE in model's bounds constrains point, otherwise FALSE.</returns>
        protected virtual bool IsValid(PointF ptTesting)
        {
            bool bSuccess = false;

            if (this.Model != null)
            {
                if (!this.Model.BoundaryConstraintsEnabled)
                {
                    bSuccess = true;
                }
                else
                {
                    if (m_modelBounds.Contains(ptTesting))
                    {
                        bSuccess = true;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Creates array of obstacles.
        /// </summary>
        /// <returns>Array of obstacles.</returns>
        protected ArrayList GetObstacles()
        {
            // document non line based node's(obstacles) bounding rectangle
            ArrayList lstObstacles = new ArrayList();
            m_rgnObstacles = null;
            this.RoutingConnectors.Clear();
            RectangleF rcTemp;

            // iterate through model nodes updating
            // rcObstBounds with non line based node's bounding rectangle
            foreach (Node nodeCur in this.Model.Nodes)
            {
                ConnectorBase conn = nodeCur as ConnectorBase;

                if (conn == null && nodeCur.TreatAsObstacle)
                {
                    //// get node's bounding rectangle
                    rcTemp = MeasureUnitsConverter.Convert(
                        nodeCur.BoundingRectangle, nodeCur.MeasurementUnit, MeasureUnits.Pixel);
                    //// inflate node's bounding rect with DistanceToObstacle property value
                    rcTemp.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);

                    //// add to node bounds
                    lstObstacles.Add(new Obstacle(nodeCur, rcTemp));

                    rcTemp.Inflate(-1, -1);

                    if (m_rgnObstacles == null)
                    {
                        m_rgnObstacles = new Region(rcTemp);
                    }
                    else
                    {
                        m_rgnObstacles.Union(rcTemp);
                    }
                }
                else if (conn != null && this.RoutingMode != RoutingMode.SemiAutomatic)
                {
                    if ((conn.HeadEndPoint.Port != null || conn.TailEndPoint.Port != null) && conn.LineRoutingEnabled)
                    {
                        if (conn.HeadingHead != CompassHeading.None || conn.HeadingTail != CompassHeading.None)
                        {
                            this.RoutingConnectors.Add(nodeCur);
                        }
                        else
                        {
                            ConnectionPoint portHead = conn.HeadEndPoint.Port;
                            ConnectionPoint portTail = conn.TailEndPoint.Port;
                            RectangleF connBoundRect = conn.BoundingRectangle;
                            bool isObstacle = false;
                            foreach (LineSegment segment in conn.LineSegments)
                            {
                                PointF startPt = segment.Point1;
                                PointF endPt = segment.Point2;
                                startPt = new PointF(startPt.X + connBoundRect.X, startPt.Y + connBoundRect.Y);
                                endPt = new PointF(endPt.X + connBoundRect.X, endPt.Y + connBoundRect.Y);
                                RectangleF segBounds = Geometry.CreateRect(startPt, endPt);

                                foreach (Node node in this.Model.Nodes)
                                {
                                    if (!(node is ConnectorBase) && node.TreatAsObstacle)
                                    {
                                        RectangleF bounds = node.BoundingRectangle;
                                        if (portTail is CentralPort && portHead is CentralPort)
                                        {
                                            bounds.Inflate(-node.LineStyle.LineWidth, -node.LineStyle.LineWidth);
                                            if (node is RoundRect)
                                            {
                                                bounds.Inflate(-((RoundRect)node).CurveRadius / 4, -((RoundRect)node).CurveRadius / 4);
                                            }
                                            if (segBounds.IntersectsWith(bounds))
                                            {
                                                isObstacle = true;
                                                this.RoutingConnectors.Add(nodeCur);
                                            }
                                        }
                                        else if (segBounds.IntersectsWith(bounds))
                                        {
                                            isObstacle = true;
                                            this.RoutingConnectors.Add(nodeCur);
                                        }
                                    }
                                }
                            }
                            if (!isObstacle)
                            {
                                if (conn.LineSegments.Count > 1)
                                    this.RoutingConnectors.Add(nodeCur);
                            }
                        }
                    }
                }
            }
            return lstObstacles;
        }

        /// <summary>
        /// Searches for route from endPointHead to endPointTail.
        /// </summary>
        /// <param name="endPointHead">EndPoint to start search from.</param>
        /// <param name="endPointTail">EndPoint to search to.</param>
        /// <returns>Array of points that represent path</returns>
        protected virtual PointF[] FindPath(EndPoint endPointHead, EndPoint endPointTail)
        {
            ConnectorBase conn = (ConnectorBase)endPointHead.Container;
            
            // get parent transforms
            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(conn, false);
            
            // update endpoints
            PointF ptStartPoint = GetEndPointPosition(conn.HeadEndPoint);
            PointF ptEndPoint = GetEndPointPosition(conn.TailEndPoint);
            PointF ptStart = ptStartPoint;
            PointF ptEnd = ptEndPoint;
            ConnectionPoint portHead = conn.HeadEndPoint.Port;
            ConnectionPoint portTail = conn.TailEndPoint.Port;

            RectangleF connBounds = conn.BoundingRectangle;
            bool isObstacle = false;
            
            if (conn.HeadingTail != CompassHeading.None || conn.HeadingHead != CompassHeading.None)
            {
                isObstacle = true;
            }
            else
            foreach (Node node in this.Model.Nodes)
            {
                if(portHead != null && portTail != null)
                    if (!(node is ConnectorBase))
                    {
                        if (portTail is CentralPort && portHead is CentralPort)
                        {
                            RectangleF rect = node.BoundingRectangle;
                            rect.Inflate(-node.LineStyle.LineWidth, -node.LineStyle.LineWidth);
                            if (node is RoundRect)
                            {
                                rect.Inflate(-((RoundRect)node).CurveRadius / 4, -((RoundRect)node).CurveRadius / 4);
                            }
                            if (connBounds.IntersectsWith(rect))
                            {
                                isObstacle = true;
                                if (node != conn.FromNode && node != conn.ToNode)
                                    conn.ObstaclesInPath = true;
                                else
                                    conn.ObstaclesInPath = false;
                            }
                        }
                        else if (connBounds.IntersectsWith(node.BoundingRectangle))
                        {
                            isObstacle = true;
                            if (node != conn.FromNode && node != conn.ToNode)
                                conn.ObstaclesInPath = true;
                            else
                                conn.ObstaclesInPath = false;
                        }
                    }
            }
            if (portHead != null && isObstacle)
                ptStart = GetRouteHeadingPoint(portHead.Container, ptStart, conn.HeadingHead);

            if (portTail != null && isObstacle)
                ptEnd = GetRouteHeadingPoint(portTail.Container, ptEnd, conn.HeadingTail);

            PointF[] ptsToReturn = null;

            // calc grid cells corresponding to start/end search points
            SearchNode nodeStart = new SearchGridNode(ptStart);
            SearchNode nodeEnd = new SearchGridNode(ptEnd);

            SearchNodeArray lstStart = new SearchNodeArray(null);             
            SearchNodeArray lstEnd = new SearchNodeArray(null);
            if (isObstacle)
            {
                lstStart = GetNearestSearchGridNodes(nodeStart, endPointHead);
                lstEnd = GetNearestSearchGridNodes(nodeEnd, endPointTail);
            }

            if (FindPath(nodeStart, nodeEnd))
            {
                SearchNode nodeTemp = nodeEnd;
                ArrayList lstPoints = new ArrayList();

                lstPoints.Add(ptEndPoint);

                while (nodeTemp != null)
                {
                    lstPoints.Add(nodeTemp.Location);
                    nodeTemp = nodeTemp.Parent;
                }

                lstPoints.Add(ptStartPoint);

                // Normalize route path points
                PointF[] path = (PointF[])lstPoints.ToArray(typeof(PointF));
                ptsToReturn = Geometry.MergePointsInLine(path);
            }
            else
            {
                if (!(endPointHead.Container is PolyLineConnector))
                {
                    RectangleF conBounds = Geometry.CreateRect(ptStartPoint, ptEndPoint);
                    foreach (Node node in Model.Nodes)
                    {
                        if (!(node is ConnectorBase))
                        {
                            if (portHead != null && portTail != null)
                            {
                                if (node.BoundingRectangle.IntersectsWith(conBounds) && (node != portHead.Container && node != portTail.Container))
                                {
                                    isObstacle = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isObstacle && !(endPointHead.Container is OrgLineConnector))
                    {
                        ptsToReturn = new PointF[2];
                        ptsToReturn[0] = ptEndPoint;
                        ptsToReturn[1] = ptStartPoint;
                    }
                }
            }

            foreach (SearchNode node in m_processedNodes)
            {
                node.Processed = false;
                node.Parent = null;
            }

            DisconnectNodes(lstStart);
            DisconnectNodes(lstEnd);

            return ptsToReturn;
        }

        /// <summary>
        /// Gets the top parent node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The top parent node.</returns>
        private Node GetTopParentNode(Node node)
        {
            Node parent = node;

            while (parent.Parent != null && parent.Parent is Node)
                parent = parent.Parent as Node;

            return parent;
        }

        /// <summary>
        /// Check are points on one horizontal or vertical line.
        /// </summary>
        /// <param name="ptStart">Start point to check.</param>
        /// <param name="ptEnd">End point to check.</param>
        /// <returns>TRUE if points are on one horizontal or vertical line, otherwise false.</returns>
        protected bool CheckPointOnLine(PointF ptStart, PointF ptEnd)
        {
            Point ptStartPoint = new Point((int)Math.Round(ptStart.X), (int)Math.Round(ptStart.Y));
            Point ptEndtPoint = new Point((int)Math.Round(ptEnd.X), (int)Math.Round(ptEnd.Y));

            return (ptStartPoint.X == ptEndtPoint.X) || (ptStartPoint.Y == ptEndtPoint.Y) || (ptStartPoint == ptEndtPoint);
        }

        /// <summary>
        /// Searches for route from nodeStart to nodeEnd.
        /// </summary>
        /// <param name="nodeStart">Node to start search from.</param>
        /// <param name="nodeEnd">Node to search to.</param>
        /// <returns>True if route found, otherwise false.</returns>
        protected bool FindPath(SearchNode nodeStart, SearchNode nodeEnd)
        {
            m_processedNodes = new SearchNodeArray(null, m_lstSearchNodes.Count);
            //// indicates wheher route has been found
            bool bSuccess = false;
            //// SearchNodes priority heap sorted by F( G(Mode cost) + H(Heuristic cost) )
            SearchNodeArray lstPriorityHeap = new SearchNodeArray(nodeEnd);
            lstPriorityHeap.Add(nodeStart);
            SearchNode nodeCur;

            // loop while there are any SearchNodes in SearchNodes priority heap
            while (lstPriorityHeap.Count > 0)
            {
                // get top node.
                nodeCur = lstPriorityHeap[0];
                
                // remove it(pop)
                lstPriorityHeap.RemoveAt(0);

                // if path found -> break loop and exit
                if (nodeCur == nodeEnd)
                {
                    nodeEnd.Parent = nodeCur.Parent;
                    m_processedNodes.Add(nodeEnd);
                    bSuccess = true;
                    break;
                }

                // proceed with current node's neighbours
                foreach (SearchNode nodeSuccessor in nodeCur.Neighbours)
                {
                    // reopen closed nodes
                    if (nodeSuccessor.Processed || lstPriorityHeap.Contains(nodeSuccessor))
                    {
                        float fSuccessorMoveCost = nodeSuccessor.GetMoveCost();
                        float fGridMoveCost = nodeCur.GetMoveCost() + SearchGridNode.GetGridMoveCost(nodeCur, nodeSuccessor);

                        if (nodeSuccessor.Parent != nodeCur && fSuccessorMoveCost > fGridMoveCost)
                        {
                            nodeSuccessor.Parent = nodeCur;
                            m_processedNodes.Add(nodeSuccessor);
                        }
                    }
                    else
                    {
                        // add successor node to priority heap
                        nodeSuccessor.Parent = nodeCur;
                        m_processedNodes.Add(nodeSuccessor);
                        lstPriorityHeap.Add(nodeSuccessor);
                    }
                }

                // mark node as processed (closed)
                nodeCur.Processed = true;
            }

            return bSuccess;
        }

        /// <summary>
        /// Gets the end point position in model coordinates.
        /// </summary>
        /// <param name="endPoint">The end point in model coordinates.</param>
        /// <returns>The end points collection.</returns>
        private PointF GetEndPointPosition(EndPoint endPoint)
        {
            PointF ptToReturn = endPoint.Location;

            // if end point is connected to central port set endpoint location to port container
            if (endPoint.Port != null && endPoint.Port is CentralPort)
            {
                ptToReturn = endPoint.Port.GetPosition();

                // update to port container coordinates
                Node nodePortCnt = endPoint.Port.Container;

                if (nodePortCnt != null)
                {
                    //// get port container's transformations
                    Matrix mtx = HandlesHitTesting.GetParentsTransformations(nodePortCnt, true);
                    //// apply transformations to port position
                    ptToReturn = Geometry.AppendMatrix(ptToReturn, mtx);
                }
            }
            else
            {
                Matrix matrix = HandlesHitTesting.GetParentsTransformations(endPoint.Container, false);
                ptToReturn = Geometry.AppendMatrix(ptToReturn, matrix);
            }

            return ptToReturn;
        }

        /// <summary>
        /// Gets the bounds of end point port container .
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Bounds of port container node.</returns>
        protected RectangleF GetEndPointPortContainerBounds(EndPoint endPoint)
        {
            RectangleF rcToReturn = RectangleF.Empty;

            if (endPoint.Port != null)
            {
                Node nodeContainer = endPoint.Port.Container;
                rcToReturn = ((IUnitIndependent)nodeContainer).GetBoundingRectangle(MeasureUnits.Pixel, true);
            }

            return rcToReturn;
        }
        private PointF GetRouteHeadingPoint(Node endPointContainer, PointF ptLocation, CompassHeading heading)
        {
            PointF ptToReturn = ptLocation;

            if (endPointContainer != null)
            {
                // get node bounds in model coordinates
                RectangleF rcBounds = GetIntersectBounds(endPointContainer);

                // widen bounds to route distance
                rcBounds = Geometry.WidenRect(rcBounds, this.DistanceToObstacles);

                // get bounds point
                if (heading == CompassHeading.None)
                    heading = Geometry.GetCompassHeading(rcBounds, ptLocation);

                // get any compass heading to get out from node
                if (heading == CompassHeading.None)
                    heading = CompassHeading.North;

                ptToReturn = Geometry.GetBoundaryPoint(rcBounds, ptLocation, heading);
            }

            return ptToReturn;
        }

        /// <summary>
        /// Get node bounds will intersected nodes.
        /// </summary>
        /// <param name="node">Node that use to calculate related bounds.</param>
        /// <returns>The node bounds.</returns>
        private RectangleF GetIntersectBounds(Node node)
        {
            Node parent = GetTopParentNode(node);

            RectangleF rcBounds = ((IUnitIndependent)parent).GetBoundingRectangle(MeasureUnits.Pixel, true);
            Model model = node.Root;

            if (model != null)
            {
                int nCount = -1;
                NodeCollection nodes = model.Nodes;
                NodeCollection intersect = new NodeCollection();

                while (intersect.Count != nCount)
                {
                    nCount = intersect.Count;

                    rcBounds = Geometry.WidenRect(rcBounds, this.DistanceToObstacles);
                    intersect = GetNodesIntersecting(nodes, rcBounds);
                    rcBounds = GetBoundingRect(intersect);
                }
            }

            return rcBounds;
        }

        /// <summary>
        /// Gets the collection of intersecting obstacles in collection with given rectangle.
        /// </summary>
        /// <param name="nodes">The node collection.</param>
        /// <param name="recbBounding">The bounds rectangle to intersect with.</param>
        /// <returns>The intersecting nodes collection.</returns>
        public NodeCollection GetNodesIntersecting(NodeCollection nodes, RectangleF recbBounding)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            NodeCollection nodesToReturn = new NodeCollection();
            RectangleF rectNodeBounding;

            foreach (Node node in nodes)
            {
                if (node is IEndPointContainer || !node.TreatAsObstacle)
                    continue;

                rectNodeBounding = ((IUnitIndependent)node).GetBoundingRectangle(MeasureUnits.Pixel, true);
                rectNodeBounding = Geometry.WidenRect(rectNodeBounding, this.DistanceToObstacles);

                if (recbBounding.IntersectsWith(rectNodeBounding))
                    nodesToReturn.Add(node);
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Gets the bounding rectangle of node collection.
        /// </summary>
        /// <param name="nodes">The node collection.</param>
        /// <returns><see cref="System.Drawing.RectangleF"/> instance.</returns>
        public RectangleF GetBoundingRect(NodeCollection nodes)
        {
            RectangleF rcBounds = RectangleF.Empty;

            foreach (Node node in nodes)
            {
                RectangleF rcNodeBounds = ((IUnitIndependent)node).GetBoundingRectangle(MeasureUnits.Pixel, true);

                if (nodes.First == node)
                    rcBounds = rcNodeBounds;
                else
                    rcBounds = RectangleF.Union(rcBounds, rcNodeBounds);
            }

            return rcBounds;
        }
        #endregion

        #region Class internal declaration
        /// <summary>
        /// Simple obstance template that used to find short way. Contain the HasCentralPort flag
        /// and the BoundingRect properties.
        /// </summary>
        /// <remarks>
        /// Instance created from node that contain port collection and it bounding rectangle.
        /// </remarks>
        private class Obstacle
        {
            #region Class public members
            /// <summary>
            /// Store value indivates that the obstacle has enabled central port.
            /// </summary>
            public bool HasCentralPort;

            /// <summary>
            /// Store bounding rectangle of the obstacle.
            /// </summary>
            public RectangleF BoundingRect;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="Obstacle"/> class.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="rcBounding">The bounding rectangle.</param>
            /// <exception cref="System.ArgumentNullException">When node is equal null.</exception>
            public Obstacle(Node node, RectangleF rcBounding)
            {
                if (node == null) throw new ArgumentNullException("node");

                HasCentralPort = node.Edges.Count > 0 && node.EnableCentralPort;
                BoundingRect = rcBounding;
            }
            #endregion
        }
        #endregion
    }
}
