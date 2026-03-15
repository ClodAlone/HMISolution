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
    /// The DirectedTreeLayoutManager class, a specialization of the <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/> 
    /// base, implements a layout manager for arranging nodes in a tree-like structure. The <see cref="DirectedTreeLayoutManager"/> 
    /// can be applied to any diagram that is composed as a directed tree graph with a unique root and child nodes. The layout manager 
    /// lets you orient the tree in just about any direction around the root, and can be used for creating arrangements such as 
    /// top-to-bottom vertical trees, bottom-to-top vertical trees, right-to-left horizontal trees, left-to-right horizontal trees, 
    /// angular trees etc.,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.RadialTreeLayoutManager"/>
    /// </summary>
    [ToolboxItem(false)]
    public class DirectedTreeLayoutManager : GraphLayoutManager
    {
        #region Fields
        private const float nodeOffset = 25;
        
        /// <summary>
        /// Graph Rotation degree.
        /// </summary>
        private float m_fRotationDegree = 0;
        private int m_TreeLevel = 0;
        private bool m_SingleChildLayout = false;
        protected LayoutType m_LayoutType = LayoutType.Waterfall;
        private float lastX = 0;
        private float lastY = 0;
        #endregion Fields

        #region Class properties
        /// <summary>
        /// Gets or sets the rotation angle for graph.
        /// </summary>
        /// <value>The rotation angle.</value>
        public float RotationAngle
        {
            get { return m_fRotationDegree; }
            set { m_fRotationDegree = value; }
        }
        
        #endregion

        #region Initialize / Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayoutManager"/> class.
        /// </summary>
        public DirectedTreeLayoutManager()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotationDegree">The root to child orientation to be used for the tree.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        public DirectedTreeLayoutManager(Model model, float fRotationDegree, float fVerticalOffset, float fHorizontalOffset)
            : base(model, fVerticalOffset, fHorizontalOffset)
        {
            m_fRotationDegree = fRotationDegree;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotationDegree">The root to child orientation to be used for the tree.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="layoutType">Specifies the layout type of tree</param>
        /// <param name="nTreeLevel">Specifies the tree level upto which the default layout has to be maintained</param>
        public DirectedTreeLayoutManager(Model model, float fRotationDegree, float fVerticalOffset, float fHorizontalOffset, LayoutType layoutType, int nTreeLevel)
            : base(model, fHorizontalOffset, fVerticalOffset)
        {
            m_fRotationDegree = fRotationDegree;
            if (layoutType != LayoutType.Horizontal)
                m_TreeLevel = nTreeLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotationDegree">The root to child orientation to be used for the tree.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="layoutType">Specifies the layout type of tree</param>
        /// <param name="nTreeLevel">Specifies the tree level upto which the default layout has to be maintained</param>
        /// <param name="bSingleChildLayout">Determines whether the layout needs to done on single child node</param>
        public DirectedTreeLayoutManager(Model model, float fRotationDegree, float fVerticalOffset, float fHorizontalOffset, LayoutType layoutType, int nTreeLevel, bool bSingleChildLayout)
            : base(model, fHorizontalOffset, fVerticalOffset)
        {
            m_fRotationDegree = fRotationDegree;
            if (layoutType != LayoutType.Horizontal)
                m_TreeLevel = nTreeLevel;

            m_SingleChildLayout = bSingleChildLayout;
        }
        #endregion Initialize / Finalize

        #region Overrides

        /// <summary>
        /// Applies the directed tree layout strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph.GraphType == GraphType.OneParentDirectedTree)
                MakeLayout(graph);
        }

        #endregion Overrides

        #region Helper Methods
        /// <summary>
        /// Performs the graph layout.
        /// </summary>
        /// <param name="graphSorting">The graph.</param>
        [Documentation.DocumentationExclude()]
        protected void MakeLayout(Graph graphSorting)
        {
            MakeGraphLayout(graphSorting);
            ApplyRotation(graphSorting);
        }

        /// <summary>
        /// Rotates Graph.
        /// </summary>
        /// <param name="graphSorting">The graph to rotate.</param>
        protected void ApplyRotation(Graph graphSorting)
        {
            // Graph graph = this.SelectedNode as Graph;
            // if ( graph.Equals( graphSorting ) )
            graphSorting.Rotate(m_fRotationDegree);
        }

        /// <summary>
        /// Applies Layout Manager logic to given graph.
        /// </summary>
        /// <param name="graphSorting">Affecting graph.</param>
        protected void MakeGraphLayout(Graph graphSorting)
        {
            DoInitXLayout(graphSorting);
            MakeXLayout(graphSorting.TypeOrdered);
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

        #region RankSorting
        private void MakeXLayout(ArrayList lstToSort)
        {
            int nCounter = 0;

            foreach (ArrayList lstSort in lstToSort)
            {
                // MakeXLayoutRelative( lstToSort, lstSort, false, nCounter );
                if (m_TreeLevel == 0 || m_LayoutType != LayoutType.Waterfall)
                    m_TreeLevel = lstToSort.Count;
                if (nCounter < m_TreeLevel || ((nCounter == m_TreeLevel) && m_LayoutType != LayoutType.Waterfall))
                    UpdateTreeToTop(lstToSort, nCounter);
                else
                    return;
                nCounter++;
            }
        }
        private void UpdateTreeToTop(ArrayList lstSorting, int nRank)
        {
            int nCounter = nRank;

            while (nCounter >= 0)
            {
                ArrayList lstToSort = lstSorting[nCounter] as ArrayList;
                MakeXLayoutRelative(lstSorting, lstToSort, true, nCounter);
                nCounter--;
            }
        }

        private void MakeXLayoutRelative(ArrayList lstRankedNodes, ArrayList lstToSort, bool bUpdateTree, int nRank)
        {
            ArrayList lstToNormalize = lstToSort;
            int nNormalizeRank = nRank;

            foreach (object entry in lstToSort)
            {
                if (entry is GraphNode)
                {
                    GraphNode node = entry as GraphNode;

                    if (node.Children.Count > 0)
                    {
                        if (bUpdateTree)
                            PlaceRelativeToChildren(node.Children, node);
                        else
                        {
                            PlaceRelativeToParent(node.Children, node);
                            lstToNormalize = (ArrayList)lstRankedNodes[nRank + 1];
                            nNormalizeRank = nRank + 1;
                        }
                    }
                }
            }

            NormalizeNodes(lstToNormalize, nNormalizeRank);
        }

        private void PlaceRelativeToChildren(ICollection lstRelatives, GraphNode node)
        {
            float fLeftChildPosition = 0;
            float fRightChildPosition = 0;

            IEnumerator vals = lstRelatives.GetEnumerator();
            vals.MoveNext();
            IGBounds nodeLeft = vals.Current as IGBounds;
            IGBounds nodeRight = nodeLeft;
            IGBounds bounds;

            while (vals.MoveNext())
            {
                bounds = vals.Current as IGBounds;

                if (nodeLeft.X > bounds.X)
                    nodeLeft = bounds;

                if (nodeRight.X < bounds.X)
                    nodeRight = bounds;
            }

            fLeftChildPosition = nodeLeft.X;
            fRightChildPosition = nodeRight.X + nodeRight.Width;

            float fMoveTo = (fLeftChildPosition + (fRightChildPosition - fLeftChildPosition) / 2) - node.Width / 2;
            float fOffset = fMoveTo - node.X;
            node.X = fMoveTo;
        }

        /// <summary>
        /// Normalizes the nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="nRank">The n rank.</param>
        protected virtual void NormalizeNodes(ArrayList nodes, int nRank)
        {
            for (int nCounter = 0; nCounter < nodes.Count - 1; nCounter++)
            {
                float nodeBound = 0;
                IGBounds node = nodes[nCounter] as IGBounds;
                IGBounds nodeNext = nodes[nCounter + 1] as IGBounds;

                nodeBound = node.X + node.Width + this.HorizontalSpacing;
                if (nodeBound > nodeNext.X)
                {
                    float fOffset = nodeBound - nodeNext.X;
                    nodeNext.X = nodeBound;

                    MoveChildren(nodeNext as GraphNode, fOffset);
                }
            }
        }

        private static void MoveChildren(GraphNode nodeNext, float fOffset)
        {
            // Move children
            foreach (GraphNode gnChild in nodeNext.Children)
            {
                gnChild.X += fOffset;

                if (gnChild.Children.Count > 0)
                {
                    MoveChildren(gnChild, fOffset);
                }
            }
        }

        private void PlaceRelativeToParent(ICollection hashChildren, GraphNode nodeParent)
        {
            float fParentMedian = nodeParent.Center.X;
            RectangleF rcRankBounds = RectangleF.Empty;

            foreach (IGBounds node in hashChildren)
            {
                RectangleF rcNode = new RectangleF(node.Bounds.X, node.Bounds.Y, node.Bounds.Width + this.HorizontalSpacing, node.Bounds.Height);
                rcRankBounds = rcRankBounds.IsEmpty ? rcNode : RectangleF.Union(rcNode, rcRankBounds);
            }

            float fOffset = fParentMedian - (rcRankBounds.X + rcRankBounds.Width / 2);
            MoveChildren(nodeParent, fOffset);
        }

        /// <summary>
        /// Does the init X layout.
        /// </summary>
        /// <param name="graphToSort">The graph to sort.</param>
        protected void DoInitXLayout(Graph graphToSort)
        {
            lastX = 0;
            lastY = 0;
            ArrayList lstToSort = graphToSort.TypeOrdered;
            if (m_TreeLevel == 0 || m_LayoutType != LayoutType.Waterfall)
                m_TreeLevel = graphToSort.TypeOrdered.Count;

            int nCounter = 0;

            foreach (ArrayList lstSort in lstToSort)
            {
                MakeLayout(lstSort, nCounter, graphToSort);
                nCounter++;
            }
        }

        /// <summary>
        /// X coordinate routine.
        /// </summary>
        /// <param name="gnBounds">The bounds.</param>
        /// <param name="fX">The value.</param>
        protected virtual void XCoordRoutine(IGBounds gnBounds, ref float fX)
        {
            gnBounds.X = fX;
            fX += gnBounds.Width + this.HorizontalSpacing;
            gnBounds.X += this.HorizontalSpacing;
        }

        /// <summary>
        /// Makes the layout.
        /// </summary>
        /// <param name="lstRank">The last rank.</param>
        /// <param name="nRank">The rank.</param>
        /// <param name="graph">The graph.</param>
        protected virtual void MakeLayout(ArrayList lstRank, int nRank, Graph graph)
        {
            float fX = this.HorizontalSpacing;
            float fY = this.VerticalSpacing;
            if (nRank <= m_TreeLevel)
            {
                // Pass the nodes that are in and before to m_TreeLevel to XCoordRoutine method.
                foreach (IGBounds bounds in lstRank)
                {
                    if (bounds == null)
                        throw new ArgumentNullException("NULL");

                    XCoordRoutine(bounds, ref fX);
                    bounds.Y = (this.VerticalSpacing + bounds.Height) * nRank;
                }
                
                // Pass the nodes that are in the m_TreeLevel to WaterFallNodePosition method.
                if ((nRank == m_TreeLevel) && (m_LayoutType == LayoutType.Waterfall))
                {
                    foreach (GraphNode levelGraphNode in lstRank)
                    {
                        if (levelGraphNode == null)
                            throw new ArgumentNullException("NULL");

                        levelGraphNode.X = lastX + this.HorizontalSpacing + levelGraphNode.Width;
                        lastY = levelGraphNode.Y;

                        if (m_LayoutType == LayoutType.Waterfall)
                            WaterFallNodePosition(levelGraphNode);
                    }
                }
                else
                {
                    // ============== Y Coordinate Routine ================
                    float fYRank = CalcRankPositionVerticalLayout(nRank, graph);
                    YCoordinateRoutine(lstRank, fYRank);
                }
            }
        }

        /// <summary>
        /// Method that sets the location of nodes in Waterfall layout.
        /// </summary>
        /// <param name="currentNode">The node whose child has to be positioned,</param>
        protected void WaterFallNodePosition(GraphNode currentNode)
        {
            if (currentNode != null)
            {
                // Set the highest X position attained by a child.
                if (lastX < currentNode.X)
                    lastX = currentNode.X;

                // Set the highest Y position attained by a child
                if (lastY < currentNode.Y)
                    lastY = currentNode.Y;

                // Passing the nodes in the level mentioned to the WaterFallNodePosition method.
                SizeF szRankDimensions = (SizeF)(this.SelectedNode as Graph).BiggestDimensions[0];
                ArrayList edgesLeaving = currentNode.Children;
                if (edgesLeaving != null && edgesLeaving.Count != 0)
                {
                    foreach (GraphNode currentEdge in edgesLeaving)
                    {
                        currentEdge.Location = new PointF(currentNode.Location.X + ((edgesLeaving.Count ==1 && m_SingleChildLayout)? 0f: szRankDimensions.Width / 2 + nodeOffset), lastY + (this.VerticalSpacing + szRankDimensions.Height));
                        WaterFallNodePosition(currentEdge);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the rank position vertical layout.
        /// </summary>
        /// <param name="nRank">The rank.</param>
        /// <param name="graph">The graph.</param>
        /// <returns>The value.</returns>
        protected float CalcRankPositionVerticalLayout(int nRank, Graph graph)
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
        /// <param name="nFromRank">The rank from.</param>
        /// <param name="nToRank">The rank to.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="graph">The graph.</param>
        /// <returns>The value.</returns>
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

        /// <summary>
        /// Y coordinate routine.
        /// </summary>
        /// <param name="lstRank">The last rank.</param>
        /// <param name="fYRank">The rank.</param>
        protected virtual void YCoordinateRoutine(ArrayList lstRank, float fYRank)
        {
            foreach (IGBounds bounds in lstRank)
            {
                bounds.Y = fYRank - bounds.Height / 2;
            }
        }

        #endregion RankSorting

        #endregion Helper Methods
    }

    /// <summary>
    /// Specifies coordinate to use.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum Coordinate
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        X = 1,

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        Y
    }

    /// <summary>
    /// Specifies dimension to use.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum Dimension
    {
        /// <summary>
        /// Width of the layout.
        /// </summary>
        Width = 1,

        /// <summary>
        /// Height of the layout.
        /// </summary>
        Height
    }

    /// <summary>
    /// Internal usage helper enum.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum EdgeConnected
    {
        /// <summary>
        /// The node to which the connector is to connect.
        /// </summary>
        ToNode = 1,

        /// <summary>
        /// The node from which the connector should start its connection.
        /// </summary>
        FromNode
    }

    /// <summary>
    /// Specifies node relatives.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum Relatives
    {
        /// <summary>
        /// Node relatives are parents.
        /// </summary>
        Parents = 1,

        /// <summary>
        /// Node relatives are children.
        /// </summary>
        Children
    }

    /// <summary>
    /// Specifies collection of nodes to use.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum NodesCollection
    {
        /// <summary>
        /// The passed nodes.
        /// </summary>
        Passed = 1,

        /// <summary>
        /// The selected nodes.
        /// </summary>
        Selected
    }

    /// <summary>
    /// Specifies Graph type.
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Directed Graph in which nodes can contain one parent and several children.
        /// </summary>
        OneParentDirectedTree = 1,

        /// <summary>
        /// Directed Graph in which nodes can contain several parents and several children.
        /// </summary>
        MultiParentDirectedTree,

        /// <summary>
        /// Non directed graph.
        /// </summary>
        NonDirectedGraph
    }

    /// <summary>
    /// Specifies Layout type.
    /// </summary>
    public enum LayoutType
    {
        /// <summary>
        /// Directed Graph in which nodes are laid out horizontally.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Directed Graph in which nodes after treelevel are laid out in waterfall model.
        /// </summary>
        Waterfall
    }
}
