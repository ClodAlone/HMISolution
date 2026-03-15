// <copyright file="AStarLineRouter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
#if WPF
using System.Drawing;
using System.Drawing.Drawing2D;
#endif

#endregion

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Line Routing engine that use A* find path Algorithm to route orthogonal lines.
    /// </summary>   
    internal class AStarLineRouter : LineRouter
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
        private List<object> m_lstObstacles;

#if SILVERLIGHT
        /// <summary>
        /// Store region that represent all obstacles.
        /// </summary>
        private Region m_rgnObstacles;
#endif

#if WPF
        /// <summary>
        /// Store region that represent all obstacles.
        /// </summary>
        private Region m_rgnObstacles;
#endif

        /// <summary>
        /// Store collection of the connectors to route.
        /// </summary>
        private List<LineConnector> m_nodesConnectors = new List<LineConnector>();       

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
        /// <param name="view">The reference to diagram view.</param>
        public AStarLineRouter(DiagramView view)
            : base(view)
        {
            m_lstSearchNodes = new SearchNodeArray(null);
            RouteAllViewConnectorsInternal();
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets the reference to collection of connectors that will be routed by engine.
        /// </summary>
        /// <value>The reference to routing connector collection.</value>
        protected List<LineConnector> RoutingConnectors
        {
            get
            {
                return m_nodesConnectors;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Reroutes all available view connectors.
        /// </summary>
        /// <remarks>
        /// Call ResetSearchCache() and Reroute() method.
        /// </remarks>
        protected override void RouteAllViewConnectorsInternal()
        {
            ResetSearchCache();
            Reroute();
        }
        
        #endregion

        #region Helper methods
        /// <summary>
        /// Recreate obstacles collections and generate search grid to find all possible ways.
        /// </summary>
        /// <remarks>
        /// Collect all obstacles ( node with enable Node.TreatAsObstacle property ) in view and save it to m_lstObstacles member.
        /// Then generate search grid by calling InitSerarchGrid() that used last obstacles collection.
        /// </remarks>
        public void ResetSearchCache()
        {
            // recreate search grid
            // --------------------
            // get view nodes bounding rectangle - !!non line based!!
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
        List<System.Windows.Point> tempConnectionPoints = new List<System.Windows.Point>();

        protected void Reroute()
        {
            System.Windows.Point[] ptsNew;

            // reroute node's edges
            foreach (LineConnector connector in this.RoutingConnectors)
            {
                // find new route from start point to end point
                if (connector.TailNode != null && connector.HeadNode != null)
                {
                    ptsNew = FindPath(connector);

                    if (ptsNew != null)
                    {
                        // update connector with new points                    
                        tempConnectionPoints.Clear();
                        connector.IntermediatePoints.Clear();

                        for (int i = 0; i < ptsNew.Length; i++)
                        {
                            if (i == 0 || i == ptsNew.Length - 1)
                                tempConnectionPoints.Add(ptsNew[i]);
                            else
                                connector.IntermediatePoints.Add(ptsNew[i]);
                        }

                        connector.isRouting = true;
                        UpdateRoutingConnectorPathGeometry(connector, tempConnectionPoints);
                    }
                    else
                    {
                        tempConnectionPoints.Clear();
#if WPF
                     tempConnectionPoints = connector.GetTerminalPoints();
#endif
#if SILVERLIGHT
                        tempConnectionPoints = connector.GetLinePoints((connector.HeadNode as Node).GetInfo(), (connector.TailNode as Node).GetInfo());
#endif
                        UpdateRoutingConnectorPathGeometry(connector, tempConnectionPoints);
                    }
                }
            }

            this.RoutingConnectors.Clear();
        }

        private void UpdateRoutingConnectorPathGeometry(LineConnector connector, List<System.Windows.Point> tempConnectionPoints)
        {
            //UpdateIntermediatePoints(connector, tempConnectionPoints);
            tempConnectionPoints = InsertIntermediatePoints(connector, tempConnectionPoints);
            connector.UpdatePathGeometry(tempConnectionPoints);            
        }

        private void UpdateIntermediatePoints(LineConnector connector, List<System.Windows.Point> tempPts)
        {
            if (connector.IntermediatePoints.Count > 0)
            {
                //System.Windows.Rect headRect = getRect(connector.HeadNode as Node);
                //if (tempPts[0].Y == headRect.BottomLeft.Y || tempPts[0].Y == headRect.TopLeft.Y)
                connector.IntermediatePoints[0] = new System.Windows.Point(tempPts[0].X, connector.IntermediatePoints[0].Y);
                //else
                //    connector.IntermediatePoints[0] = new System.Windows.Point(connector.IntermediatePoints[0].X, tempPts[0].Y);                
                //connector.IntermediatePoints[0] = new System.Windows.Point(tempPts[0].X, connector.IntermediatePoints[0].Y);

                //System.Windows.Rect tailRect = getRect(connector.TailNode as Node);
                //if (tempPts[tempPts.Count - 1].Y == tailRect.BottomLeft.Y || tempPts[tempPts.Count - 1].Y == tailRect.TopLeft.Y)
                connector.IntermediatePoints[connector.IntermediatePoints.Count - 1] = new System.Windows.Point(tempPts[tempPts.Count - 1].X, connector.IntermediatePoints[connector.IntermediatePoints.Count - 1].Y);
                //else
                //    connector.IntermediatePoints[connector.IntermediatePoints.Count - 1] = new System.Windows.Point(connector.IntermediatePoints[connector.IntermediatePoints.Count - 1].X, tempPts[tempPts.Count - 1].Y);
                //connector.IntermediatePoints[connector.IntermediatePoints.Count - 1] = new System.Windows.Point(tempPts[tempPts.Count - 1].X, connector.IntermediatePoints[connector.IntermediatePoints.Count - 1].Y);
            }
        }

        private List<System.Windows.Point> InsertIntermediatePoints(LineConnector connector, List<System.Windows.Point> tempPts)
        {
            if (tempPts.Count > 0)
            {
                if (connector.IntermediatePoints.Count >= 1)
                {
                    for (int i = connector.IntermediatePoints.Count - 1; i >= 0; i--)
                    {
                        tempPts.Insert(1, connector.IntermediatePoints[i]);
                    }
                }
            }
            return tempPts;
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
        /// <returns>The search node array.</returns>
        protected SearchNodeArray GetBoundaryIntersects(System.Windows.Point ptEndPointLocation, System.Windows.Rect rcBounds)
        {
            double fRight = Math.Round(rcBounds.X + rcBounds.Width, 2);
            double fBottom = Math.Round(rcBounds.Y + rcBounds.Height, 2);

            SearchNodeArray lstToReturn = new SearchNodeArray(null);
            ptEndPointLocation = new System.Windows.Point(Math.Round(ptEndPointLocation.X, 2), Math.Round(ptEndPointLocation.Y, 2));
            rcBounds = new System.Windows.Rect(Math.Round(rcBounds.X, 2), Math.Round(rcBounds.Y, 2), Math.Round(rcBounds.Width, 2), Math.Round(rcBounds.Height, 2));

            //if (!rcBounds.Contains(ptEndPointLocation) || ptEndPointLocation.X == rcBounds.X
            //    || ptEndPointLocation.Y == rcBounds.Y || ptEndPointLocation.X == fRight || ptEndPointLocation.Y == fBottom)
            {
                SearchNode node = CreateSearchNode(ptEndPointLocation);
                lstToReturn.Add(node);
            }            

            return lstToReturn;
        }

        /// <summary>
        /// Creates the search node in given location.
        /// </summary>
        /// <param name="ptLoc">The search node location.</param>
        
        protected virtual SearchNode CreateSearchNode(System.Windows.Point ptLoc)
        {
            return new SearchGridNode(ptLoc);
        }

        /// <summary>
        /// Gets intersection points of given search node with search grid.
        /// </summary>
        /// <param name="node">The search node.</param>
        /// <returns>The search node array.</returns>
        protected SearchNodeArray GetNearestSearchGridNodes(SearchNode node, Node n, LineConnector conn)
        {
            // connected node bounds
            System.Windows.Rect rcBounds = System.Windows.Rect.Empty;

            //if (n.centerport != null)
            {
                rcBounds = this.getRect(n);
#if WPF
                rcBounds.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);
#endif
#if SILVERLIGHT
                rcBounds = this.Inflate(rcBounds, this.DistanceToObstacles);
#endif
            }

            // get EndPoint container boundary intersect points
            SearchNodeArray lstBI = GetBoundaryIntersects(node.Location, rcBounds);

            SearchNodeArray nodesToReturn = new SearchNodeArray(null);
            double fMinDistance;
            double fDistance;
            System.Windows.Point ptTo;
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

                    System.Windows.Point pt1 = nodeNearest.Location;
                    pt1.X = Math.Round(pt1.X, 2);
                    pt1.Y = Math.Round(pt1.Y, 2);

                    System.Windows.Point pt2 = nodeCur.Location;
                    pt2.X = Math.Round(pt2.X, 2);
                    pt2.Y = Math.Round(pt2.Y, 2);

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
                            System.Windows.Point ptTmp = new System.Windows.Point(nodeNearest.Location.X, nodeCur.Location.Y);
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
                                ptTmp = new System.Windows.Point(nodeCur.Location.X, nodeNearest.Location.Y);

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
        /// using m_lstObstacles member as obstacles collection.
        /// </remarks>
        protected void InitSearchGrid()
        {
            // current obstacle
            Obstacle obstacle;
            System.Windows.Rect rcObstacle;
            double nTemp;            
                      
#if WPF
            // sorted list for X-axis values & Y-axis values   
            SortedList lstX = new SortedList();
            SortedList lstY = new SortedList();
            
            // search grid prepare routine
             for (int i = 0, nLength = m_lstObstacles.Count; nLength > i; i++)
            {
                // get current obstacle
                obstacle = (Obstacle)m_lstObstacles[i];
                rcObstacle = obstacle.BoundingRect;

                // X-axis values
                // -------------
                // Left
                nTemp = rcObstacle.Left;

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
#endif
            
#if SILVERLIGHT
            // sorted list for X-axis values & Y-axis values  
            List<double> lstX = new List<double>();             
            List<double> lstY = new List<double>();

            //// search grid prepare routine
            for (int i = 0, nLength = m_lstObstacles.Count; nLength > i; i++)
            {
                // get current obstacle
                obstacle = (Obstacle)m_lstObstacles[i];
                rcObstacle = obstacle.BoundingRect;

                // X-axis values
                // -------------
                // Left
                nTemp = rcObstacle.Left;

                if (!lstX.Contains(nTemp))
                    lstX.Add(nTemp);

                // Right
                nTemp = rcObstacle.Right;

                if (!lstX.Contains(nTemp))
                    lstX.Add(nTemp);

                // Y-axis values
                // -------------
                // Top
                nTemp = rcObstacle.Y;

                if (!lstY.Contains(nTemp))
                    lstY.Add(nTemp);

                // Bottom
                nTemp = rcObstacle.Bottom;

                if (!lstY.Contains(nTemp))
                    lstY.Add(nTemp);

                // if node has edges -> add center points to search grid
                if (obstacle.HasCentralPort)
                {
                    nTemp = rcObstacle.X + rcObstacle.Width / 2;

                    if (!lstX.Contains(nTemp))
                        lstX.Add(nTemp);

                    nTemp = rcObstacle.Y + rcObstacle.Height / 2;

                    if (!lstY.Contains(nTemp))
                        lstY.Add(nTemp);
                }
            }
#endif

            m_rowsCount = lstY.Count;
            m_columnsCount = lstX.Count;
            double fY;
            System.Windows.Point ptTemp;

            // create search grid vertices
            m_searchGrid = new SearchGridNode[m_rowsCount, m_columnsCount];
            m_lstSearchNodes.Clear();
            SearchGridNode node;

            for (int i = 0; i < m_rowsCount; i++)
            {
#if WPF
                fY = (double)lstY.GetKey(i);
#endif
#if SILVERLIGHT
                fY = (double)lstY[i];
#endif

                for (int k = 0; k < m_columnsCount; k++)
                {
#if WPF
                    ptTemp = new System.Windows.Point((double)lstX.GetKey(k), fY);
#endif
#if SILVERLIGHT
                    ptTemp = new System.Windows.Point((double)lstX[k], fY);
#endif

                    // check Model.BoundaryConstrains flag
                    //if (IsValid(ptTemp))
                    {
                        node = new SearchGridNode(ptTemp);
                        m_lstSearchNodes.Add(node);
                        m_searchGrid[i, k] = node;
                    }                   
                }
            }

            // update grid node's neighbours connections            
            for (int nCurRow = 0; m_rowsCount > nCurRow; nCurRow++)
            {
                for (int nCurCol = 0; m_columnsCount > nCurCol; nCurCol++)
                {
                    if (m_columnsCount > (nCurCol + 1))
                    {
                        UpdateNeighbours(nCurRow, nCurCol, nCurRow, nCurCol + 1);
                    }

                    if (m_rowsCount > (nCurRow + 1))
                    {
                        UpdateNeighbours(nCurRow, nCurCol, nCurRow + 1, nCurCol);
                    }
                }
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
            double centerX;
            double centerY;

            double p1X = node1.Location.X;
            double p2X = node2.Location.X;

            double p1Y = node1.Location.Y;
            double p2Y = node2.Location.Y;

            centerX = (p1X == p2X) ? p1X : (p1X + p2X) / 2;
            centerY = (p1Y == p2Y) ? p1Y : (p1Y + p2Y) / 2;

#if WPF
            return !m_rgnObstacles.IsVisible((float)centerX, (float)centerY);
#endif
#if SILVERLIGHT
            return !m_rgnObstacles.IsVisible(new System.Windows.Point(centerX, centerY));
#endif
        }        

        /// <summary>
        /// Creates list of obstacles.
        /// </summary>
        /// <returns>List of obstacles.</returns>
        protected List<object> GetObstacles()
        {
            // document non line based node's(obstacles) bounding rectangle
            List<object> lstObstacles = new List<object>();
            m_rgnObstacles = null;
            this.RoutingConnectors.Clear();
            System.Windows.Rect rcTemp;
#if WPF
            RectangleF rcTemp1;
#endif

            // iterate through view nodes updating
            // rcObstBounds with non line based node's bounding rectangle
            foreach (object nodeCur in this.View.Page.Children)
            {
                if (nodeCur is Node)
                {
                    if ((nodeCur as Node).TreatAsObstacle)
                    {
                        rcTemp = this.getRect(nodeCur as Node);
                        
                        // inflate node's bounding rect with DistanceToObstacle property value
#if WPF
                        if ((nodeCur as Node).Edges.Count > 0)
                            rcTemp.Inflate(this.DistanceToObstacles + 10, this.DistanceToObstacles + 10);
                        else
                            rcTemp.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);
#endif
#if SILVERLIGHT
                        if ((nodeCur as Node).Edges.Count > 0)
                            rcTemp = this.Inflate(rcTemp, this.DistanceToObstacles + 10);
                        else
                            rcTemp = this.Inflate(rcTemp, this.DistanceToObstacles);
#endif

                        // add to node bounds
                        lstObstacles.Add(new Obstacle(nodeCur as Node, rcTemp));
                        
                        // only add obstacles to region
#if WPF
                        rcTemp1 = new RectangleF((float)(nodeCur as Node).PxOffsetX, (float)(nodeCur as Node).PxOffsetY, (float)(nodeCur as Node).Width, (float)(nodeCur as Node).Height);
                        rcTemp1.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);                        
                        rcTemp1.Inflate(-1, -1);                        

                        if (m_rgnObstacles == null)
                        {
                            m_rgnObstacles = new Region(rcTemp1);
                        }
                        else
                        {
                            m_rgnObstacles.Union(rcTemp1);
                        }
#endif
#if SILVERLIGHT
                        rcTemp = this.Inflate(rcTemp, -1);

                        if (m_rgnObstacles == null)
                        {
                            m_rgnObstacles = new Region(rcTemp);
                        }
                        else
                        {
                            m_rgnObstacles.Union(rcTemp);
                        }
#endif
                    }
                }
                else if (nodeCur is LineConnector)
                {
                    if (((/*(nodeCur as LineConnector).ConnectorType == ConnectorType.Straight || */(nodeCur as LineConnector).ConnectorType == ConnectorType.Orthogonal))

                        && (nodeCur as LineConnector).LineRoutingEnabled)
                    {
                        //if (View.RoutingMode == RoutingMode.DragEnd)
                        //{
                        //    if (!View.IsDragged)
                        //    {
                        //        this.RoutingConnectors.Add(nodeCur as LineConnector);
                        //    }
                        //    else
                        //        if (View.DraggingElement is Node)
                        //        {
                        //            if ((View.DraggingElement as Node).InEdges.Contains(nodeCur as LineConnector) == true || (View.DraggingElement as Node).OutEdges.Contains(nodeCur as LineConnector) == true)
                        //            {
                        this.RoutingConnectors.Add(nodeCur as LineConnector);
                        //            }
                        //        }

                        //}

                        //if (View.RoutingMode == RoutingMode.Immediate)
                        //{
                        //    this.RoutingConnectors.Add(nodeCur as LineConnector);
                        //}
                    }
                }
            }

            return lstObstacles;
        }

        private Rect Inflate(Rect rect, double disToObs)
        {
            if (disToObs > 0)
            {
                return (new Rect(rect.X - disToObs, rect.Y - disToObs, rect.Width + (2 * disToObs), rect.Height + (2 * disToObs)));
            }
            else
            {
                return (new Rect(rect.X + (-1) * (disToObs), rect.Y + (-1) * disToObs, rect.Width + (2 * disToObs), rect.Height + (2 * disToObs)));
            }
        }

        /// <summary>
        /// Searches for route from endPointHead to endPointTail.
        /// </summary>
        /// <returns>Array of points that represent path</returns>
        protected virtual System.Windows.Point[] FindPath(LineConnector conn)
        {
            List<System.Windows.Point> tempConnectionPoints = new List<System.Windows.Point>();
#if WPF
            tempConnectionPoints = conn.GetTerminalPoints();
#endif
#if SILVERLIGHT
            tempConnectionPoints = conn.GetLinePoints((conn.HeadNode as Node).GetInfo(), (conn.TailNode as Node).GetInfo());
#endif

            System.Windows.Point ptStartPoint = tempConnectionPoints[tempConnectionPoints.Count - 1];
            System.Windows.Point ptEndPoint = tempConnectionPoints[0];

            System.Windows.Point ptStart = ptStartPoint;
            System.Windows.Point ptEnd = ptEndPoint;

            bool isObstacle = false;

            foreach (object node in this.View.Page.Children)
            {
                if (node is Node)
                {
                    if (((node as Node) != conn.HeadNode && (node as Node) != conn.TailNode) || (ptEndPoint.Y != ptStartPoint.Y))
                    {
                        List<System.Windows.Point> nodePts = new List<System.Windows.Point>();
                        nodePts = getNodePts((node as Node), conn);

                        List<System.Windows.Point> linePts = new List<System.Windows.Point>();
                        linePts = tempConnectionPoints;
                        linePts = InsertIntermediatePoints(conn, linePts);

                        List<System.Windows.Point> intersectPts = new List<System.Windows.Point>();

                        for (int i = 0; i < nodePts.Count - 1; i = i + 2)
                        {
                            intersectPts = ConnectorBase.FindPOIBetweenLineAndPolyLine(nodePts[i], nodePts[i + 1], linePts);
                            if (intersectPts.Count > 0)
                            {
                                isObstacle = true;
                                break;
                            }
                        }
                    }
                }
            } 

            if (isObstacle)
            {
                if(conn.TailNode!=null)
                ptStart = GetRouteHeadingPoint(conn.TailNode as Node, ptStart, conn);
                if(conn.HeadNode!=null)
                ptEnd = GetRouteHeadingPoint(conn.HeadNode as Node, ptEnd, conn);
            }

            System.Windows.Point[] ptsToReturn = null;

            // calc grid cells corresponding to start/end search points
            SearchNode nodeStart = new SearchGridNode(ptStart);
            SearchNode nodeEnd = new SearchGridNode(ptEnd);

            SearchNodeArray lstStart = new SearchNodeArray(null);
            SearchNodeArray lstEnd = new SearchNodeArray(null);

            if (isObstacle)
            {
                if ((conn.TailNode as Node) != null)
                    lstStart = GetNearestSearchGridNodes(nodeStart, conn.TailNode as Node, conn);
                if ((conn.HeadNode as Node) != null)
                    lstEnd = GetNearestSearchGridNodes(nodeEnd, conn.HeadNode as Node, conn);
            }

            if (FindPath(nodeStart, nodeEnd))
            {
                SearchNode nodeTemp = nodeEnd;
                List<System.Windows.Point> lstPoints = new List<System.Windows.Point>();

                lstPoints.Add(ptEndPoint);

                while (nodeTemp != null)
                {
                    lstPoints.Add(nodeTemp.Location);
                    nodeTemp = nodeTemp.Parent;
                }

                lstPoints.Add(ptStartPoint);

                // Normalize route path points
                System.Windows.Point[] path = (System.Windows.Point[])lstPoints.ToArray();
                ptsToReturn = this.MergePointsInLine(path);
                ptsToReturn = this.RemovePoints(ptsToReturn);

            }

            else
            {
                if (!(conn.ConnectorType == ConnectorType.Orthogonal))
                {
                    ptsToReturn = new System.Windows.Point[2];
                    ptsToReturn[0] = ptEndPoint;
                    ptsToReturn[1] = ptStartPoint;
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

        private List<System.Windows.Point> getNodePts(Node node, LineConnector con)
        {
            List<System.Windows.Point> nodePts = new List<System.Windows.Point>();
            Rect nodeBounds = getRect(node);           
#if WPF 
            nodeBounds.Inflate(this.DistanceToObstacles, this.DistanceToObstacles);
#endif
#if SILVERLIGHT
            nodeBounds = this.Inflate(nodeBounds, this.DistanceToObstacles);
#endif
            nodePts.Add(new System.Windows.Point(nodeBounds.Left, nodeBounds.Top));
            nodePts.Add(new System.Windows.Point(nodeBounds.Right, nodeBounds.Top));
            nodePts.Add(new System.Windows.Point(nodeBounds.Right, nodeBounds.Top));
            nodePts.Add(new System.Windows.Point(nodeBounds.Right, nodeBounds.Bottom));
            nodePts.Add(new System.Windows.Point(nodeBounds.Left, nodeBounds.Top));
            nodePts.Add(new System.Windows.Point(nodeBounds.Left, nodeBounds.Bottom));
            nodePts.Add(new System.Windows.Point(nodeBounds.Left, nodeBounds.Bottom));
            nodePts.Add(new System.Windows.Point(nodeBounds.Right, nodeBounds.Bottom));
            return(nodePts);
        }

        private System.Windows.Rect getRect(Node node)
        {
            System.Windows.Rect rect = new System.Windows.Rect(node.PxOffsetX, node.PxOffsetY, node.ActualWidth, node.ActualHeight);
            return (rect);
        }        

        private System.Windows.Point[] MergePointsInLine(System.Windows.Point[] pts)
        {
            List<System.Windows.Point> lstPts = new List<System.Windows.Point>(pts);

            for (int i = 0, nLength = lstPts.Count - 2; i < nLength; i++)
            {
                if (CheckPointOnLine((System.Windows.Point)lstPts[i], (System.Windows.Point)lstPts[i + 2], 0))
                {
                    // update path points
                    lstPts.RemoveAt(i + 1);

                    // convert to pointF array
                    pts = (System.Windows.Point[])lstPts.ToArray();

                    // call recursively itself
                    pts = MergePointsInLine(pts);
                    break;
                }
            }

            return pts;
        }

        private System.Windows.Point[] RemovePoints(System.Windows.Point[] pts)
        {
            #if WPF 
            List<System.Windows.Point> lstPts = new List<System.Windows.Point>(pts);

            for (int i = 1, nLength = lstPts.Count - 3; i < nLength; i++)
            {
                if (lstPts.Count > 4)
                {
                    if ((lstPts[i].X == lstPts[i + 1].X || Math.Abs(lstPts[i].X - lstPts[i + 1].X) < 0.1) && (lstPts[i + 1].Y == lstPts[i + 2].Y || Math.Abs(lstPts[i].Y - lstPts[i + 1].Y) < 0.1))
                    {
                        if (lstPts[i].Y < lstPts[i + 1].Y && lstPts[i + 3].Y < lstPts[i + 2].Y || lstPts[i].Y > lstPts[i + 1].Y && lstPts[i + 3].Y > lstPts[i + 2].Y)
                        {
                        }
                        else
                        {
                            System.Windows.Point pt = new System.Windows.Point(lstPts[i + 2].X, lstPts[i].Y);
                            Rect r = new Rect(pt, lstPts[i + 1]);
                            Rectangle rc = new Rectangle(new System.Drawing.Point((int)r.Left, (int)r.Top), new System.Drawing.Size((int)r.Width, (int)r.Height));
                            if (!m_rgnObstacles.IsVisible(rc))
                            {
                                lstPts.RemoveAt(i);
                                lstPts.RemoveAt(i);
                                lstPts.RemoveAt(i);
                                lstPts.Insert(i, pt);


                                // convert to pointF array
                                pts = (System.Windows.Point[])lstPts.ToArray();

                                // call recursively itself
                                pts = RemovePoints(pts);
                                break;
                            }
                        }
                    }
                }
                //else
                if (lstPts.Count > 4)
                {
                    if (lstPts[i].Y == lstPts[i + 1].Y && lstPts[i + 1].X == lstPts[i + 2].X)
                    {
                        if (lstPts[i].X < lstPts[i + 1].X && lstPts[i + 3].X < lstPts[i + 2].X || lstPts[i].X > lstPts[i + 1].X && lstPts[i + 3].X > lstPts[i + 2].X)
                        {
                        }
                        else
                        {
                            System.Windows.Point pt = new System.Windows.Point(lstPts[i].X, lstPts[i + 2].Y);
                            Rect r = new Rect(pt, lstPts[i + 1]);
                            Rectangle rc = new Rectangle(new System.Drawing.Point((int)r.Left, (int)r.Top), new System.Drawing.Size((int)r.Width, (int)r.Height));
                            if (!m_rgnObstacles.IsVisible(rc))
                            {
                                lstPts.RemoveAt(i);
                                lstPts.RemoveAt(i);
                                lstPts.RemoveAt(i);
                                lstPts.Insert(i, pt);

                                // convert to pointF array
                                pts = (System.Windows.Point[])lstPts.ToArray();

                                // call recursively itself
                                pts = RemovePoints(pts);
                                break;
                            }
                        }
                    }
                }
            }

            #endif 
            return pts;
        }
        private bool CheckPointOnLine(System.Windows.Point ptStart, System.Windows.Point ptEnd, int nDigits)
        {
            System.Windows.Point ptStartPoint = new System.Windows.Point((int)Math.Round(ptStart.X, nDigits), (int)Math.Round(ptStart.Y, nDigits));
            System.Windows.Point ptEndtPoint = new System.Windows.Point((int)Math.Round(ptEnd.X, nDigits), (int)Math.Round(ptEnd.Y, nDigits));

            return (ptStartPoint.X == ptEndtPoint.X) || (ptStartPoint.Y == ptEndtPoint.Y) || (ptStartPoint == ptEndtPoint);
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
                        double fSuccessorMoveCost = nodeSuccessor.GetMoveCost();
                        double fGridMoveCost = nodeCur.GetMoveCost() + SearchGridNode.GetGridMoveCost(nodeCur, nodeSuccessor);

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

        private System.Windows.Point GetRouteHeadingPoint(Node endPointContainer, System.Windows.Point ptLocation, LineConnector conn)
        {
            System.Windows.Point ptToReturn = ptLocation;

            if (endPointContainer != null)
            {
                // get node bounds in model coordinates
                System.Windows.Rect rcBounds = GetIntersectBounds(endPointContainer, conn);

                // widen bounds to route distance
                rcBounds = this.WidenRect(rcBounds, this.DistanceToObstacles);

                bool isTop = false;
                bool isBottom = false;
                bool isLeft = false;
                bool isRight = false;

                GetHeadingDirection(rcBounds, ptToReturn, out isTop, out isBottom, out isLeft, out isRight);

                // bottom, right, left, and top or others
                if (isBottom)
                    ptToReturn = new System.Windows.Point(ptLocation.X, rcBounds.Bottom);

                else if (isRight)
                    ptToReturn = new System.Windows.Point(rcBounds.Right, ptLocation.Y);

                else if (isLeft)
                    ptToReturn = new System.Windows.Point(rcBounds.X, ptLocation.Y);

                else if(isTop)
                    ptToReturn = new System.Windows.Point(ptLocation.X, rcBounds.Y);

                //if (ptLocation.Y == rcBounds.Bottom)
                //    ptToReturn = new System.Windows.Point(ptLocation.X, rcBounds.Bottom);

                //else if (ptLocation.X == rcBounds.Right)
                //    ptToReturn = new System.Windows.Point(rcBounds.Right, ptLocation.Y);

                //else if (ptLocation.X == rcBounds.Left)
                //    ptToReturn = new System.Windows.Point(rcBounds.X, ptLocation.Y);

                //else
                //    ptToReturn = new System.Windows.Point(ptLocation.X, rcBounds.Y);
            }

            return ptToReturn;
        }

        /// <summary>
        /// Gets the compass heading direction.
        /// Return only North, West, South, East or None value.
        /// </summary>
        /// <param name="rcBounds">The round area.</param>
        /// <param name="ptPoint">The outside point.</param>
        /// <returns>Heading direction.</returns>
        private void GetHeadingDirection(Rect rcBounds, System.Windows.Point ptPoint, out bool isTop, out bool isBottom, out bool isLeft, out bool isRight)
        {
            double fTop = ptPoint.Y - rcBounds.Y;
            double fBottom = rcBounds.Bottom - ptPoint.Y;
            double fLeft = ptPoint.X - rcBounds.X;
            double fRight = rcBounds.Right - ptPoint.X;
            isTop = false;
            isBottom = false;
            isLeft = false;
            isRight = false;

            // find inHeading direction            
            // bottom
            if (fBottom <= fTop && fBottom <= fLeft && fBottom <= fRight)
                isBottom = true;

            // left
            else if (fLeft <= fTop && fLeft <= fBottom && fLeft <= fRight)
                isLeft = true;

            // top
            else if (fTop <= fLeft && fTop <= fBottom && fTop <= fRight)
                isTop = true;

            // right
            else if (fRight <= fLeft && fRight <= fBottom && fRight <= fTop)
                isRight = true;
        }

        /// <summary>
        /// Get node bounds will intersected nodes.
        /// </summary>
        /// <param name="node">Node that use to calculate related bounds.</param>
        /// <returns>The node bounds.</returns>
        private System.Windows.Rect GetIntersectBounds(Node node, LineConnector conn)
        {
            Node parent = GetTopParentNode(node);
            System.Windows.Rect rcBounds = this.getRect(parent);            

            if (View != null)
            {
                int nCount = -1;
                CollectionExt intersect = new CollectionExt();

                while (intersect.Count != nCount)
                {
                    nCount = intersect.Count;

                    rcBounds = this.WidenRect(rcBounds, this.DistanceToObstacles);
                    List<Node> nodes = new List<Node>();
                    foreach (object o in this.View.Page.Children)
                    {
                        if(o is Node)
                            nodes.Add(o as Node);
                    }
                    if(nodes != null)
                        intersect = GetNodesIntersecting(nodes, rcBounds, conn);
                    rcBounds = GetBoundingRect(intersect, conn);
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
        public CollectionExt GetNodesIntersecting(List<Node> nodes, System.Windows.Rect recbBounding, LineConnector conn)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            CollectionExt nodesToReturn = new CollectionExt();
            System.Windows.Rect rectNodeBounding;            

            foreach (Node node in nodes)
            {
                if (!node.TreatAsObstacle)
                    continue;

                rectNodeBounding = this.getRect(node);
                rectNodeBounding = this.WidenRect(rectNodeBounding, this.DistanceToObstacles);               

#if WPF
                if (recbBounding.IntersectsWith(rectNodeBounding))
                    nodesToReturn.Add(node);
#endif
#if SILVERLIGHT
                rectNodeBounding.Intersect(recbBounding);
                if(rectNodeBounding != Rect.Empty)
                    nodesToReturn.Add(node);
#endif
            }

            return nodesToReturn;
        }

        private System.Windows.Rect WidenRect(System.Windows.Rect rcRect, float fWidth)
        {
            System.Windows.Rect rcToReturn = rcRect;

            // offset location
            rcToReturn.X -= fWidth;
            rcToReturn.Y -= fWidth;

            // scale size
            rcToReturn.Width += fWidth * 2;
            rcToReturn.Height += fWidth * 2;

            return rcToReturn;
        }

        /// <summary>
        /// Gets the bounding rectangle of node collection.
        /// </summary>
        /// <param name="nodes">The node collection.</param>
        
        public System.Windows.Rect GetBoundingRect(CollectionExt nodes, LineConnector conn)
        {
            System.Windows.Rect rcBounds = System.Windows.Rect.Empty;

            foreach (Node node in nodes)
            {
                System.Windows.Rect rcNodeBounds = this.getRect(node);

#if WPF
                if (nodes[0] == node)
                    rcBounds = rcNodeBounds;
                else
                    rcBounds = System.Windows.Rect.Union(rcBounds, rcNodeBounds);
#endif
#if SILVERLIGHT
                if (nodes[0] == node)
                    rcBounds = rcNodeBounds;
#endif
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
        internal class Obstacle
        {
            #region Class public members
            /// <summary>
            /// Store value indivates that the obstacle has enabled central port.
            /// </summary>
            public bool HasCentralPort;

            /// <summary>
            /// Store bounding rectangle of the obstacle.
            /// </summary>
            public System.Windows.Rect BoundingRect;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="Obstacle"/> class.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="rcBounding">The bounding rectangle.</param>
            /// <exception cref="System.ArgumentNullException">When node is equal null.</exception>
            public Obstacle(Node node, System.Windows.Rect rcBounding)
            {
                if (node == null) throw new ArgumentNullException("node");

                HasCentralPort = node.Edges.Count > 0;
                BoundingRect = rcBounding;
            }
            #endregion
        }

#if SILVERLIGHT
        /// <Summary>
        /// Describes the interior of a graphics shape composed of rectangles.
        /// This is like in System.Drawing.Region in WPF.
        /// </Summary>
        internal class Region : Collection<Rect>
        {
         
        // Summary:
        //     Initializes a new custom Region like in WPF System.Drawing.Region from the specified System.Windows.Rect
        //     structure.
        //
        // Parameters:
        //   rect:
        //     A System.Windows.Rect structure that defines the interior of the new
        //     custom Region as like in WPF System.Drawing.Region.    
        public Region(Rect rect)
            {
                this.Add(rect);
            }
        
        // Summary:
        //     Updates this custom Region to the union of itself and the specified
        //     System.Windows.Rect structure.
        //
        // Parameters:
        //   rect:
        //     The System.Windows.Rect structure to unite with this custom Region.    
        public void Union(Rect rect)
            {
                this.Add(rect);
            }
        
        // Summary:
        //     Tests whether the specified System.Windows.Point structure is contained within
        //     this custom Region.
        //
        // Parameters:
        //   point:
        //     The System.Windows.Point structure to test.
        //
        // Returns:
        //     true when point is contained within this custom Region; otherwise,
        //     false.    
        public bool IsVisible(System.Windows.Point pt)
            {
                foreach (Rect r in this)
                {
                    if (r.Contains(pt))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
#endif
        #endregion
    }
}
