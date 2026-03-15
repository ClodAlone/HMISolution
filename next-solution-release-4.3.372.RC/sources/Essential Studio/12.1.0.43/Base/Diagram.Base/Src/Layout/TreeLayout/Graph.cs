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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Graph is a collection of connected nodes ( basically SymbolBase derived objects linked connected with links ).
    /// </summary>
    public class Graph
        : IGBounds,
          IGTransform
    {
        #region Constants
        private float c_fMIN_SCALE_VALUE = 0.01F;
        private int c_nMAX_SCALE_VALUE = 6;
        #endregion

        #region Class members
        /// <summary>
        /// Contains the margin values for the Graph.
        /// </summary>
        private float m_fLeftMargin;
        private float m_fTopMargin;

        /// <summary>
        /// Determines whether the time consuming codes need to be blocked from executing.
        /// </summary>
        private bool m_improvePerformance = false;

        /// <summary>
        /// Internal flag indicating that Graph Type has been already determined.
        /// </summary>
        private bool m_bGraphTypeDetermined = false;

        /// <summary>
        /// Flag indicating whether graph nodes should resize when graph size changes.
        /// </summary>
        private bool m_bResizeNodes = true;

        /// <summary>
        /// Graph Type. 
        /// </summary>
        private GraphType m_graphType = GraphType.NonDirectedGraph;

        /// <summary>
        /// Hashtable containing nodes belonging to Graph.
        /// </summary>
        private Hashtable m_hashGraph;

        /// <summary>
        /// ArrayList containing GraphNodes ordered considering GraphType.
        /// </summary>
        private ArrayList m_lstRankOrdered;

        /// <summary>
        /// Graph bounding rectangle.
        /// </summary>
        private RectangleF m_rectBounds = new RectangleF(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

        /// <summary>
        /// Ranked Graph dimensions arranged by ranks.
        /// </summary>
        private ArrayList m_lstRankBiggestDimensions;

        /// <summary>
        /// List of fictitious nodes what insert between ranks.
        /// Used in FictitiousNodes property.
        /// </summary>
        private ArrayList m_lstFictitiousNodes;

        /// <summary>
        /// Internal usage flag indicating whether graph bounds changed.
        /// </summary>
        private bool m_bBoundsChanged = true;

        // Bounding Nodes
        private IGBounds m_gnLeftBounding = null;
        private IGBounds m_gnRightBounding = null;
        private IGBounds m_gnTopBounding = null;
        private IGBounds m_gnBottomBounding = null;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class.
        /// </summary>
        public Graph()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class.
        /// </summary>
        /// <param name="improvePerformance">Determines whether performance needs to be improved.</param>
        /// <param name="leftMargin">The left margin of the graph.</param>
        /// <param name="topMargin">The top margin of the graph.</param>
        public Graph(bool improvePerformance, float leftMargin, float topMargin)
        {
            m_improvePerformance = improvePerformance;
            m_fLeftMargin = leftMargin;
            m_fTopMargin = topMargin;
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether nodes should be resized when graph size changes.
        /// </summary>
        public bool ResizeNodes
        {
            get
            {
                return m_bResizeNodes;
            }
            set
            {
                m_bResizeNodes = value;
            }
        }

        /// <summary>
        /// Gets or sets hashtable containing graph nodes.
        /// </summary>
        public Hashtable GraphNodes
        {
            get
            {
                if (m_hashGraph == null)
                    m_hashGraph = new Hashtable();

                return m_hashGraph;
            }
            set
            {
                if (m_hashGraph != value)
                    m_hashGraph = value;
            }
        }

        /// <summary>
        /// Gets graph presentation as rank ordered hashtable according to 
        /// graph type.<seealso cref="Syncfusion.Windows.Forms.Diagram.GraphType"/>
        /// If 
        /// </summary>
        public ArrayList TypeOrdered
        {
            get
            {
                ArrayList lstTypeOrderedToReturn = null;

                // init rank order list
                if (m_lstRankOrdered == null)
                    m_lstRankOrdered = new ArrayList();

                // generate graph if needed
                if (m_bGraphTypeDetermined)
                    lstTypeOrderedToReturn = m_lstRankOrdered;
                else
                {
                    m_graphType = GetGraphType();
                    lstTypeOrderedToReturn = m_lstRankOrdered;
                }

                return lstTypeOrderedToReturn;
            }
        }

        /// <summary>
        /// Gets list of biggest dimensions in every level.
        /// </summary>
        public ArrayList BiggestDimensions
        {
            get
            {
                if (m_lstRankBiggestDimensions == null)
                {
                    m_lstRankBiggestDimensions = new ArrayList();
                }

                return m_lstRankBiggestDimensions;
            }
        }

        /// <summary>
        /// Gets graph type. <seealso cref="Syncfusion.Windows.Forms.Diagram.GraphType"/>
        /// </summary>
        public GraphType GraphType
        {
            get
            {
                if (!m_bGraphTypeDetermined)
                {
                    m_graphType = GetGraphType();
                }

                return m_graphType;
            }
        }

        #region BoundingNodes
        /// <summary>
        /// Gets graph left bounding node.
        /// </summary>
        public IGBounds LeftBoundingNode
        {
            get
            {
                if (m_gnLeftBounding == null)
                    m_gnLeftBounding = UpdateBoundaries(this, BoxSide.Left);

                return m_gnLeftBounding;
            }
        }

        /// <summary>
        /// Gets graph right bounding node.
        /// </summary>
        public IGBounds RightBoundingNode
        {
            get
            {
                if (m_gnRightBounding == null)
                    m_gnRightBounding = UpdateBoundaries(this, BoxSide.Right);

                return m_gnRightBounding;
            }
        }

        /// <summary>
        /// Gets graph top bounding node.
        /// </summary>
        public IGBounds TopBoundingNode
        {
            get
            {
                if (m_gnTopBounding == null)
                    m_gnTopBounding = UpdateBoundaries(this, BoxSide.Top);

                return m_gnTopBounding;
            }
        }

        /// <summary>
        /// Gets graph bottom bounding node.
        /// </summary>
        public IGBounds BottomBoundingNode
        {
            get
            {
                if (m_gnBottomBounding == null)
                    m_gnBottomBounding = UpdateBoundaries(this, BoxSide.Bottom);

                return m_gnBottomBounding;
            }
        }
        #endregion

        /// <summary>
        /// Gets fictitious nodes what insert between ranks.
        /// </summary>
        protected ArrayList FictitiousNodes
        {
            get
            {
                if (m_lstFictitiousNodes == null)
                    m_lstFictitiousNodes = new ArrayList();

                return m_lstFictitiousNodes;
            }
        }
        #endregion

        #region IGBounds interface
        /// <summary>
        /// Gets Graph bounding rectangle.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                if (m_bBoundsChanged)
                {
                    m_rectBounds = CalcGraphBounds();
                    m_bBoundsChanged = false;
                }

                return m_rectBounds;
            }
        }

        /// <summary>
        /// Gets or sets graph X coordinate.
        /// </summary>
        public float X
        {
            get
            {
                return this.Bounds.X;
            }
            set
            {
                if (this.Bounds.X != value)
                {
                    MoveGraph(value - m_rectBounds.X, 0);

                    m_rectBounds.X = value;
                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph Y coordinate.
        /// </summary>
        public float Y
        {
            get
            {
                return this.Bounds.Y;
            }
            set
            {
                if (this.Bounds.Y != value)
                {
                    MoveGraph(0, value - m_rectBounds.Y);

                    m_rectBounds.Y = value;
                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph location.
        /// </summary>
        public PointF Location
        {
            get
            {
                return this.Bounds.Location;
            }
            set
            {
                if (this.Bounds.Location != value)
                {
                    MoveGraph(value);
                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph Size.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return this.Bounds.Size;
            }
            set
            {
                if (this.Bounds.Size != value)
                {
                    this.Width = value.Width;
                    this.Height = value.Height;

                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph Width.
        /// </summary>
        public float Width
        {
            get
            {
                return this.Bounds.Width;
            }
            set
            {
                if (this.Bounds.Width != value)
                {
                    Resize(this.Bounds.Width, value, Dimension.Width);
                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph Height.
        /// </summary>
        public float Height
        {
            get
            {
                return this.Bounds.Height;
            }
            set
            {
                float fGraphHeight = this.Bounds.Height;
                if (fGraphHeight != value)
                {
                    Resize(fGraphHeight, value, Dimension.Height);
                    m_bBoundsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets graph Center location.
        /// </summary>
        public PointF Center
        {
            get
            {
                return new PointF(this.Bounds.X + this.Bounds.Width / 2, this.Bounds.Y + this.Bounds.Height / 2);
            }
            set
            {
                this.X = value.X - this.Width / 2;
                this.Y = value.Y - this.Height / 2;

                m_bBoundsChanged = true;
            }
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        /// Append all position and size transformation to graph nodes.
        /// </summary>s
        public void AppendTransfomChanges()
        {
            ICollection lstNodes = this.GraphNodes.Values;

            foreach (GraphNode node in lstNodes)
            {
                node.ApplyChanges();
            }
        }

        /// <summary>
        /// Recalculates graph bounds.
        /// </summary>
        public void RecalculateBounds()
        {
            m_rectBounds = CalcGraphBounds();
        }
        #endregion

        #region IGTransform interface
        /// <summary>
        /// Moves the node by the given X and Y offsets.
        /// </summary>
        /// <param name="dx">Distance to move along X axis.</param>
        /// <param name="dy">Distance to move along Y axis.</param>
        public void Translate(float dx, float dy)
        {
            MoveGraph(dx, dy);
        }

        /// <summary>
        /// Rotates the node a specified number of degrees about a given
        /// anchor point.
        /// </summary>
        /// <param name="ptAnchor">Fixed point about which to rotate.</param>
        /// <param name="fDegree">Number of degrees to rotate.</param>
        public void RotateAt(PointF ptAnchor, float fDegree)
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(fDegree, ptAnchor);
            PointF[] pts;

            foreach (DictionaryEntry entry in this.GraphNodes)
            {
                IGBounds bounds = entry.Value as IGBounds;
                pts = new PointF[] { bounds.Center };

                matrix.TransformPoints(pts);
                bounds.Center = pts[0];

                if (entry.Value is Graph)
                {
                    ((Graph)entry.Value).Rotate(fDegree);
                }

                m_bBoundsChanged = true;
            }

            foreach (GraphNode gnFict in this.FictitiousNodes)
            {
                pts = new PointF[] { gnFict.Center };

                matrix.TransformPoints(pts);
                gnFict.Center = pts[0];

                m_bBoundsChanged = true;
            }
        }

        /// <summary>
        /// Rotates the node a specified number of degrees about its center point.
        /// </summary>
        /// <param name="fDegree">Number of degrees to rotate.</param>
        public void Rotate(float fDegree)
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(fDegree, this.Center);
            PointF[] pts;

            foreach (DictionaryEntry entry in this.GraphNodes)
            {
                IGBounds bounds = entry.Value as IGBounds;
                pts = new PointF[] { bounds.Center };

                matrix.TransformPoints(pts);
                bounds.Center = pts[0];

                if (entry.Value is Graph)
                {
                    ((Graph)entry.Value).Rotate(fDegree);
                }

                m_bBoundsChanged = true;
            }

            foreach (GraphNode gnFict in this.FictitiousNodes)
            {
                pts = new PointF[] { gnFict.Center };

                matrix.TransformPoints(pts);
                gnFict.Center = pts[0];

                m_bBoundsChanged = true;
            }
        }

        /// <summary>
        /// Scales the specified f scale X.
        /// </summary>
        /// <param name="fScaleX">The X scale factor.</param>
        /// <param name="fScaleY">The Y scale factor.</param>
        public void Scale(float fScaleX, float fScaleY)
        {
            if ((fScaleX < c_fMIN_SCALE_VALUE) || (fScaleX > c_nMAX_SCALE_VALUE))
                throw new ArgumentOutOfRangeException("fScaleX", "Scale ratio must be more than 0.01 and less than 6.");

            if ((fScaleY < c_fMIN_SCALE_VALUE) || (fScaleY > c_nMAX_SCALE_VALUE))
                throw new ArgumentOutOfRangeException("fScaleY", "Scale ratio must be more than 0.01 and less than 6.");

            this.Width *= fScaleX;
            this.Height *= fScaleY;

            m_bBoundsChanged = true;
        }
        #endregion

        #region Helper Methods

        #region RankSorting
        /// <summary>
        /// Gets the graph node rank for rank order list.
        /// </summary>
        /// <param name="gnNode">The graph node.</param>
        /// <param name="nIndex">The index.</param>
        /// <returns>The rank.</returns>
        private int GetGraphNodeRank(GraphNode gnNode, out int nIndex)
        {
            int nRankToReturn = -1;
            bool bSuccess = false;
            nIndex = -1;

            if (m_lstRankOrdered != null)
            {
                foreach (ArrayList lstRank in m_lstRankOrdered)
                {
                    if (bSuccess)
                        break;

                    foreach (GraphNode gnRank in lstRank)
                    {
                        if (gnRank.Node.FullName == gnNode.Node.FullName)
                        {
                            nRankToReturn = m_lstRankOrdered.IndexOf(lstRank);
                            nIndex = lstRank.IndexOf(gnRank);

                            bSuccess = true;
                            break;
                        }
                    }
                }
            }

            return nRankToReturn;
        }
        private void RankGraphNodes()
        {
            if (!IsCycleGraph())
            {
                GraphNode gnTopNode = GetGraphFirstTopNode();
                RankGraphNodes(gnTopNode, 0, false);

                // remove emptyLevels
                ArrayList lstEmpty = new ArrayList();
                foreach (ArrayList list in m_lstRankOrdered)
                {
                    if (list.Count == 0)
                        lstEmpty.Add(list);
                }

                foreach (ArrayList list in lstEmpty)
                {
                    m_lstRankOrdered.Remove(list);
                }
            }
        }
        private int RankGraphNodes(GraphNode gnNode, int nRank, bool toParent)
        {
            if (gnNode != null)
            {
                int nIndex;
                int nNodeRank = GetGraphNodeRank(gnNode, out nIndex);

                // if node is exist in rank list
                if (nNodeRank >= 0)
                {
                    // if him level right then return current rank
                    if (CheckRankPosition(gnNode, nNodeRank, toParent, out nRank)
                        || nRank == nNodeRank)
                    {
                        return nRank;
                    }

                    // else remove this node from his rankLevel list
                    ArrayList lstRank = (ArrayList)m_lstRankOrdered[nNodeRank];
                    lstRank.Remove(gnNode);
                }

                // add and get current node rank
                nRank = AddNode(gnNode, nRank);

                foreach (GraphNode gnChild in gnNode.Children)
                {
                    RankGraphNodes(gnChild, nRank + 1, true);
                }
                if (!this.m_improvePerformance)
                {
                    foreach (GraphNode gnParent in gnNode.Parents)
                    {
                        RankGraphNodes(gnParent, nRank - 1, false);
                    }
                }
            }

            return nRank;
        }

        /// <summary>
        /// Check if node rank can be changed.
        /// Checking by maximum parent rank.
        /// </summary>
        /// <param name="gnNode">Checking graphNode </param>
        /// <param name="nNodeRank">GraphNode rank in order list.</param>
        /// <param name="bParentChild">If <b>true</b> given graphNode checked to parents rank
        /// otherwise - to children rank. </param>
        /// <param name="nRank">The rank.</param>
        /// <returns>true, if node rank can be changed.</returns>
        private bool CheckRankPosition(GraphNode gnNode, int nNodeRank, bool bParentChild, out int nRank)
        {
            nRank = nNodeRank;
            bool bCorrectPosition = true;

            int nIndex;
            int nMaxParentIndex = int.MinValue;
            int nParentRank = -1;
            int nMinChildrenIndex = int.MaxValue;
            int nChildRank = -1;

            foreach (GraphNode gnParent in gnNode.Parents)
            {
                nParentRank = GetGraphNodeRank(gnParent, out nIndex);

                if (nParentRank < 0)
                    nParentRank = 0;

                nMaxParentIndex = Math.Max(nMaxParentIndex, nParentRank);
            }

            foreach (GraphNode gnChild in gnNode.Children)
            {
                nChildRank = GetGraphNodeRank(gnChild, out nIndex);

                if (nIndex != -1)
                    nMinChildrenIndex = Math.Min(nMinChildrenIndex, nChildRank);
            }

            if (nMaxParentIndex != int.MinValue)
            {
                nRank = nMaxParentIndex + 1;
                bCorrectPosition = (nNodeRank >= nRank);
            }
            else if (nMinChildrenIndex != int.MaxValue && nMinChildrenIndex != 0)
            {
                nRank = nMinChildrenIndex - 1;
                bCorrectPosition = (nNodeRank == nRank);
            }

            return bCorrectPosition;
        }

        /// <summary>
        /// Determines whether graph has cycle.
        /// </summary>
        /// <returns>
        /// <c>true</c> if graph has cycle; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCycleGraph()
        {
            bool bSuccess = false;
            ICollection lstNodes = this.GraphNodes.Values;
            ArrayList lstChecked = new ArrayList();

            foreach (GraphNode gnNode in lstNodes)
            {
                if (IsCycleConnections(gnNode, gnNode, lstChecked))
                {
                    bSuccess = true;
                    break;
                }

                lstChecked.Clear();
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether node connected in cycle.
        /// </summary>
        /// <param name="gnNode">The graphNode.</param>
        /// <param name="gnParent">The parentNode.</param>
        /// <param name="lstChecked">The collection of checked nodes.</param>
        /// <returns>
        /// <c>true</c> if node conected in cycle; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCycleConnections(GraphNode gnNode, GraphNode gnParent, ArrayList lstChecked)
        {
            ArrayList listTemp = new ArrayList();
            bool bSuccess = lstChecked.Contains(gnNode);

            if (!bSuccess)
            {
                lstChecked.Add(gnNode);

                foreach (GraphNode firstChild in gnNode.Children)
                {
                    listTemp.AddRange(lstChecked);

                    if (IsCycleConnections(firstChild, gnParent, listTemp))
                    {
                        bSuccess = true;
                        break;
                    }

                    listTemp.Clear();
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// OneParentDirectedTree routine. Adds scecified node to specified Rank.
        /// </summary>
        /// <param name="gnNodeToAdd">Node to add.</param>
        /// <param name="nRank">Rank to insert node into.</param>
        /// <returns>The value.</returns>
        private int AddNode(object gnNodeToAdd, int nRank)
        {
            if (m_lstRankOrdered == null)
                m_lstRankOrdered = new ArrayList();

            int nLength = m_lstRankOrdered.Count;

            while (nRank < 0)
            {
                m_lstRankOrdered.Insert(0, new ArrayList());
                nRank++;
            }

            while (nRank >= nLength)
            {
                m_lstRankOrdered.Add(new ArrayList());
                nLength++;
            }

            ArrayList lstRankToAddNodeTo = (ArrayList)m_lstRankOrdered[nRank];
            lstRankToAddNodeTo.Add(gnNodeToAdd);

            return nRank;
        }

        /// <summary>
        /// Gets first GraphNode which contains no parents.
        /// </summary>
        /// <returns>Node found.</returns>
        public GraphNode GetGraphFirstTopNode()
        {
            GraphNode gnTopToReturn = null;

            foreach (GraphNode gnNode in this.GraphNodes.Values)
            {
                if (gnNode != null)
                {
                    if (gnNode.Parents.Count == 0)
                    {
                        gnTopToReturn = gnNode;
                        break;
                    }
                }
            }

            return gnTopToReturn;
        }

        /// <summary>
        /// Sorts th nodes in rank order list by remove connector intersections.
        /// </summary>
        /// <remarks>
        /// Algorism:
        /// 1. Sort nodes in list from biggest to smaller child count.
        /// 2. Get parent position in his list by find percent
        /// 3. Calc child position by multiply percent to child count
        /// </remarks>
        private void SortRankNodes()
        {
            ArrayList lstParentsRankList;
            ArrayList lstChildrenRankList;

            for (int i = 0, nLength = m_lstRankOrdered.Count - 1; i < nLength; i++)
            {
                lstParentsRankList = (ArrayList)m_lstRankOrdered[i];
                lstChildrenRankList = (ArrayList)m_lstRankOrdered[i + 1];

                if (i == 0)
                {
                    m_lstRankOrdered[i + 1] = SortChildrenRankNodes(lstParentsRankList, lstChildrenRankList);

                    lstChildrenRankList = (ArrayList)m_lstRankOrdered[i + 1];
                    m_lstRankOrdered[i] = SortParentRankNodes(lstParentsRankList, lstChildrenRankList);
                    lstParentsRankList = (ArrayList)m_lstRankOrdered[i];

                    OptimizeTopRank();
                }

                m_lstRankOrdered[i + 1] = SortChildrenRankNodes(lstParentsRankList, lstChildrenRankList);
            }
        }

        /// <summary>
        /// Sorts the nodes in rank order list by remove connector intersections.
        /// </summary>
        /// <param name="lstParentsRankList">Parents rank list.</param>
        /// <param name="lstChildrenRankList">Children rank list.</param>
        /// <returns>The sorted nodes list.</returns>
        private ArrayList SortChildrenRankNodes(ArrayList lstParentsRankList, ArrayList lstChildrenRankList)
        {
            GraphNode[] arrChildrenRankList = (GraphNode[])lstChildrenRankList.ToArray(typeof(GraphNode));
            int nLength = arrChildrenRankList.Length;

            int[] arrChildCount = new int[nLength];
            double[] arrPosition = new double[nLength];

            for (int i = 0; i < nLength; i++)
            {
                GraphNode gnChild = lstChildrenRankList[i] as GraphNode;
                arrChildCount[i] = gnChild.Parents.Count;
            }

            Array.Sort(arrChildCount, arrChildrenRankList, new InvertIntCompare());
            ArrayList lstNewRankList = new ArrayList(lstChildrenRankList);

            for (int i = 0; i < nLength; i++)
            {
                GraphNode gnChild = arrChildrenRankList[i];

                int nParentCount = lstParentsRankList.Count;
                int nChildrenCount = lstChildrenRankList.Count;

                if (nChildrenCount > 0 && nParentCount > 0)
                {
                    double dIndex = lstChildrenRankList.IndexOf(gnChild) + 0.5d;
                    double dParentIndex = GetCentralNode(gnChild.Parents, lstParentsRankList);
                    double dChildIndex = GetChildIndexInParentList(dIndex, nChildrenCount, nParentCount);

                    dIndex = GetParentIndexInChildList(dParentIndex, nChildrenCount, nParentCount);

                    double dMoveNode = CompareChilds(gnChild, lstParentsRankList);
                    double dPalceNode = CompareChilds(arrChildrenRankList[(int)dIndex], lstParentsRankList);

                    // move left or right by remove interceptions
                    if (dMoveNode > 0 && dPalceNode < 0)
                        dIndex++;

                    arrPosition[i] = dIndex;
                    
                    // MoveNode( lstNewRankList, gnChild, (int)dIndex );
                }
            }

            Array.Sort(arrPosition, arrChildrenRankList);
 
            // return lstNewRankList;
            return new ArrayList(arrChildrenRankList);
        }

        /// <summary>
        /// Sorts the nodes in rank order list by remove connector intersections.
        /// </summary>
        /// <param name="lstParentsRankList">Parents rank list.</param>
        /// <param name="lstChildrenRankList">Children rank list.</param>
        /// <returns>The sorted nodes list.</returns>
        private ArrayList SortParentRankNodes(ArrayList lstParentsRankList, ArrayList lstChildrenRankList)
        {
            GraphNode[] arrParentsRankList = (GraphNode[])lstParentsRankList.ToArray(typeof(GraphNode));
            int[] arrParentCount = new int[lstParentsRankList.Count];
            
            // int nCount = 0;
            //
            // foreach( GraphNode gnParent in lstParentsRankList )
            //       arrParentCount[ nCount++ ] = gnParent.Children.Count;
            //
            // Array.Sort( arrParentCount, arrParentsRankList, new InvertIntCompare() );
            ArrayList lstNewRankList = new ArrayList(lstParentsRankList);
            double[] arrPosition = new double[arrParentsRankList.Length];

            // for( int i =  lstParentsRankList.Count - 1, nLength = 0; i >= nLength; i-- )
            for (int i = 0, nLength = arrParentsRankList.Length; i < nLength; i++)
            {
                GraphNode gnParent = arrParentsRankList[i];

                int nParentCount = lstParentsRankList.Count;
                int nChildrenCount = lstChildrenRankList.Count;
                double dIndex = lstParentsRankList.IndexOf(gnParent) + 0.5d;
                double dChildIndex = GetCentralNode(gnParent.Children, lstChildrenRankList);
                double dParentIndex = GetParentIndexInChildList(dIndex, nChildrenCount, nParentCount);

                dIndex = GetChildIndexInParentList(dChildIndex, nChildrenCount, nParentCount);

                double dMoveNode = CompareParents(gnParent, lstChildrenRankList);
                double dPalceNode = CompareParents(arrParentsRankList[(int)dIndex], lstChildrenRankList);

                // move left or right by remove interceptions
                if (dMoveNode > 0 && dPalceNode < 0)
                    dIndex++;

                arrPosition[i] = dIndex;

                // MoveNode( lstNewRankList, gnParent, (int)dIndex );
            }

            Array.Sort(arrPosition, arrParentsRankList);

            return new ArrayList(arrParentsRankList);

            // int nChildCount = gnParent.Children.Count;
            //
            // if( nChildCount > 0 )
            // {
            //    ArrayList lstChildren = gnParent.Children;
            //    GraphNode[] arrChildren = (GraphNode[])lstChildren.ToArray( typeof( GraphNode ) );
            //    int[] lstCount = new int[ nChildCount ];
            //    int nMaxCount = 0;
            //
            //    for( int j = 0; j < nChildCount; j++ )
            //    {
            //        lstCount[j] = arrChildren[j].Parents.Count;
            //        nMaxCount = Math.Max( nMaxCount, lstCount[j] );
            //    }
            //
            //    //Array.Sort( lstCount, lstChildren, new InvertIntCompare() );
            //    ArrayList lstChildrenMaxCount = new ArrayList();
            //
            //    for( int j = 0; j < nChildCount; j++ )
            //    {
            //       if( lstCount[j] == nMaxCount )
            //       {
            //           lstChildrenMaxCount.Add( arrChildren[ j ] );
            //       }
            //    }
            //
            //    float fIndex = 0f;
            //    foreach( GraphNode gnChildNode in lstChildrenMaxCount )
            //    {
            //        fIndex += lstChildrenRankList.IndexOf( gnChildNode );
            //    }
            //
            //    fIndex = ( fIndex / (float )gnParent.Children.Count);
            //    fIndex = (float)GetChildIndexInParentList( (double)fIndex, lstChildrenRankList.Count, lstParentsRankList.Count );
            //
            //    int nLevel = CompareParents( gnParent, fIndex, lstChildrenRankList );
            //
            //    // move left or right by remove interceptions
            //    if( nLevel > 0 )
            //       fIndex++;
            //    else if( nLevel < 0 )
            //       fIndex--;
            //
            //    MoveNode( lstNewRankList, gnParent, (int)fIndex );
            // }
            // }
            //
            // return lstNewRankList;
        }

        /// <summary>
        /// Move node inside list to current index.
        /// </summary>
        /// <param name="list">ArrayList what contain node.</param>
        /// <param name="gnNode">Node what will moving.</param>
        /// <param name="nIndex">Index where node will moved.</param>
        private void MoveNode(ArrayList list, GraphNode gnNode, int nIndex)
        {
            int nCurIndex = list.IndexOf(gnNode);

            if (nCurIndex != nIndex)
            {
                if (nIndex > nCurIndex)
                    nIndex--;

                if (nIndex >= 0)
                {
                    list.Remove(gnNode);
                    list.Insert(nIndex, gnNode);
                }
                else if (nIndex == list.Count)
                {
                    list.Add(gnNode);
                }
            }
        }
        private double GetChildIndexInParentList(double nChildIndex, int nChildCount, int nParentCount)
        {
            return (nChildIndex / (double)nChildCount) * ((double)nParentCount);
        }
        private double GetParentIndexInChildList(double nParentIndex, int nChildCount, int nParentCount)
        {
            return (nParentIndex / (double)nParentCount) * ((double)nChildCount);
        }

        /// <summary>
        /// Adds the fictitious nodes.
        /// </summary>
        private void AddFictitiousNodes()
        {
            ArrayList lstParentsRank;
            ArrayList lstChildrenRank;

            for (int nLevel = 0, nLevelCount = m_lstRankOrdered.Count - 1; nLevel < nLevelCount; nLevel++)
            {
                lstParentsRank = (ArrayList)m_lstRankOrdered[nLevel];
                lstChildrenRank = (ArrayList)m_lstRankOrdered[nLevel + 1];

                foreach (GraphNode gnNode in lstParentsRank)
                {
                    ArrayList lstChildren = gnNode.Children;

                    for (int i = 0, nLength = lstChildren.Count; i < nLength; i++)
                    {
                        GraphNode gnChild = (GraphNode)lstChildren[i];

                        // if child rank don't contains node child then add fictition node
                        if (!lstChildrenRank.Contains(gnChild))
                        {
                            GraphNode gnNewNode = new GraphNode(null);
                            gnNewNode.Size = GetMinimumDimension(lstChildrenRank);

                            int nChildIndex = gnNode.Children.IndexOf(gnChild);
                            int nParentIndex = gnChild.Parents.IndexOf(gnNode);

                            //// parent change Reference to child
                            gnNode.Children[nChildIndex] = gnNewNode;
                            //// child change reference to parent
                            gnChild.Parents[nParentIndex] = gnNewNode;
                            //// Ficted node set new  parent and child
                            gnNewNode.Parents.Add(gnNode);
                            gnNewNode.Children.Add(gnChild);
                            lstChildrenRank.Add(gnNewNode);
                            //// add node to storage list
                            this.FictitiousNodes.Add(gnNewNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum dimension in nodes list.
        /// </summary>
        /// <param name="lstRankNodes">The rank nodes list.</param>
        /// <returns>The dimension</returns>
        private SizeF GetMinimumDimension(ArrayList lstRankNodes)
        {
            bool bSucces = false;
            SizeF szSize = new SizeF(float.MaxValue, float.MaxValue);

            foreach (GraphNode gnNode in lstRankNodes)
            {
                szSize.Width = Math.Min(szSize.Width, gnNode.Size.Width);
                szSize.Height = Math.Min(szSize.Height, gnNode.Size.Height);

                bSucces = true;
            }

            if (!bSucces)
                szSize = SizeF.Empty;

            return szSize;
        }

        /// <summary>
        /// Removes the fictitious nodes.
        /// </summary>
        private void RemoveFictitiousNodes()
        {
            ArrayList lstToRemove = new ArrayList();

            foreach (ArrayList lstRank in m_lstRankOrdered)
            {
                // On fictitious node Node property equals null;
                foreach (GraphNode gnNode in lstRank)
                {
                    if (gnNode.Node == null)
                        lstToRemove.Add(gnNode);
                }

                if (lstToRemove.Count > 0)
                {
                    foreach (GraphNode gnNode in lstToRemove)
                    {
                        lstRank.Remove(gnNode);
                    }

                    lstToRemove.Clear();
                }
            }
        }

        /// <summary>
        /// Remove finded interception if can.
        /// </summary>
        /// <param name="interceptions">The interceptions.</param>
        private void RemoveInterceptions(Interception[] interceptions)
        {
            Interception crossing;
            ArrayList lstRank = null;
            ArrayList lstChildrenRank = null;
            ArrayList lstPassedNodes = new ArrayList();
            int nChildrenRank = -1;

            for (int i = 0, nLength = interceptions.Length; i < nLength; i++)
            {
                crossing = interceptions[i];

                if (!crossing.CanFixed)
                    continue;

                // get parent and children rank for interception.
                for (int n = 0, length = m_lstRankOrdered.Count - 1; n < length; n++)
                {
                    lstRank = m_lstRankOrdered[n] as ArrayList;

                    if (lstRank.Contains(crossing.LeftParent))
                    {
                        nChildrenRank = n + 1;
                        lstChildrenRank = m_lstRankOrdered[nChildrenRank] as ArrayList;
                        break;
                    }
                }

                int nLeftParentIndex = lstRank.IndexOf(crossing.LeftParent);
                int nRightParentIndex = lstRank.IndexOf(crossing.RightParent);
                int nLeftChildIndex = lstChildrenRank.IndexOf(crossing.LeftChild);
                int nRightChildIndex = lstChildrenRank.IndexOf(crossing.RightChild);

                if (nRightParentIndex < nLeftParentIndex)
                {
                    GraphNode gnTemp = crossing.LeftParent;
                    crossing.LeftParent = crossing.RightParent;
                    crossing.RightParent = gnTemp;

                    gnTemp = crossing.LeftChild;
                    crossing.LeftChild = crossing.RightChild;
                    crossing.RightChild = gnTemp;
                }

                if (!lstPassedNodes.Contains(crossing.RightParent))
                {
                    // move right parent to left
                    lstRank.Remove(crossing.RightParent);
                    lstRank.Insert(nLeftParentIndex, crossing.RightParent);

                    // update children rank
                    m_lstRankOrdered[nChildrenRank] = SortChildrenRankNodes(lstRank, lstChildrenRank);

                    lstPassedNodes.Add(crossing.RightParent);
                    lstPassedNodes.Add(crossing.LeftParent);
                }
                else if (!lstPassedNodes.Contains(crossing.LeftParent))
                {
                    lstRank.Remove(crossing.LeftParent);

                    if (lstRank.Count <= nRightParentIndex + 1)
                        lstRank.Add(crossing.LeftParent);
                    else
                        lstRank.Insert(nRightParentIndex + 1, crossing.LeftParent);

                    // update children rank
                    m_lstRankOrdered[nChildrenRank] = SortChildrenRankNodes(lstRank, lstChildrenRank);

                    lstPassedNodes.Add(crossing.RightParent);
                    lstPassedNodes.Add(crossing.LeftParent);
                }
            }
        }
        private bool HasUnnecessaryChilds(GraphNode gnParent, GraphNode gnIgnore)
        {
            bool bSuccess = false;
            int nMaxCount = 1;

            foreach (GraphNode node in gnParent.Children)
            {
                if (node != gnIgnore)
                {
                    if (node.Parents.Count > nMaxCount)
                    {
                        bSuccess = false;
                        break;
                    }
                    else
                        bSuccess = true;
                }
            }

            return bSuccess;
        }
        private double CompareChilds(GraphNode gnChild, ArrayList lstParentsRankList)
        {
            double dToReturn = 0d;
            double dCenterParent = GetCentralNode(gnChild.Parents, lstParentsRankList);

            if (gnChild != null)
            {
                int nUpperIndexCount = 0;
                double dIndex = 0d;

                foreach (GraphNode gnParentNode in gnChild.Parents)
                {
                    dIndex = lstParentsRankList.IndexOf(gnParentNode) + 0.5d;

                    if (dIndex > dCenterParent)
                        nUpperIndexCount++;
                }

                dToReturn = (double)nUpperIndexCount - (double)gnChild.Parents.Count / 2d;
            }

            return dToReturn;
        }
        private double CompareParents(GraphNode gnParent, ArrayList lstChildrenRankList)
        {
            double dToReturn = 0;
            double dCenterChild = GetCentralNode(gnParent.Children, lstChildrenRankList);

            if (gnParent != null)
            {
                int nUpperIndexCount = 0;
                double dIndex = 0f;

                foreach (GraphNode gnParentNode in gnParent.Children)
                {
                    dIndex = lstChildrenRankList.IndexOf(gnParentNode) + 0.5d;

                    if (dIndex > dCenterChild)
                        nUpperIndexCount++;
                }

                dToReturn = (double)nUpperIndexCount - (double)gnParent.Children.Count / 2d;
            }

            return dToReturn;
        }
        private double GetCentralNode(ArrayList lstRealtive, ArrayList lstRankList)
        {
            double dToReturn = 0d;
            double dIndex = 0d;

            foreach (GraphNode gnNode in lstRealtive)
            {
                int index = lstRankList.IndexOf(gnNode);

                if (index >= 0)
                {
                    dIndex += index + 0.5d;
                }
            }

            if (lstRealtive.Count > 0)
            {
                dToReturn = dIndex / (double)lstRealtive.Count;
            }

            return dToReturn;
        }
        #endregion

        #region BiggestDimensions
        /// <summary>
        /// Checks the rank dimensions.
        /// </summary>
        public void CheckRankDimensions()
        {
            for (int nRankCounter = 0; nRankCounter < this.TypeOrdered.Count; nRankCounter++)
            {
                ArrayList lstRank = this.TypeOrdered[nRankCounter] as ArrayList;

                for (int nCounter = 0; nCounter < lstRank.Count; nCounter++)
                {
                    IGBounds nodeToCompare = lstRank[nCounter] as IGBounds;
                    CheckDimensions(nodeToCompare, nRankCounter);
                }
            }
        }

        /// <summary>
        /// Checks the dimensions.
        /// </summary>
        /// <param name="nodeToCompare">The node to compare.</param>
        /// <param name="nRank">The rank number.</param>
        public void CheckDimensions(IGBounds nodeToCompare, int nRank)
        {
            int nLength = this.BiggestDimensions.Count;
            SizeF szRankBiggestDimension;

            if (nLength > nRank && nRank >= 0)
            {
                szRankBiggestDimension = (SizeF)this.BiggestDimensions[nRank];

                if (IsCurrentNodeDimensionBigger(szRankBiggestDimension, nodeToCompare, Dimension.Width))
                    Update(ref szRankBiggestDimension, nodeToCompare, Dimension.Width);

                if (IsCurrentNodeDimensionBigger(szRankBiggestDimension, nodeToCompare, Dimension.Height))
                    Update(ref szRankBiggestDimension, nodeToCompare, Dimension.Height);
                
            }
            else
            {
                while (nRank >= nLength)
                {
                    this.BiggestDimensions.Add(SizeF.Empty);
                    nLength++;
                }

                while (nRank < 0)
                {
                    this.BiggestDimensions.Insert(0, SizeF.Empty);
                    nRank++;
                }

                szRankBiggestDimension = new SizeF();
                szRankBiggestDimension.Width = nodeToCompare.Width;
                szRankBiggestDimension.Height = nodeToCompare.Height;                
            }
            this.BiggestDimensions[nRank] = szRankBiggestDimension;
        }
        private void Update(ref SizeF szDimensionToUpdate, IGBounds node, Dimension dimension)
        {
            switch (dimension)
            {
                case Dimension.Width:
                    szDimensionToUpdate.Width = node.Width;
                    break;
                case Dimension.Height:
                    szDimensionToUpdate.Height = node.Height;
                    break;
            }
        }

        private bool IsCurrentNodeDimensionBigger(SizeF szRankBiggestDimension, IGBounds boundsNode, Dimension dimension)
        {
            bool bIsCurrentNodeDimensionBigger = false;

            switch (dimension)
            {
                case Dimension.Width:
                    if (boundsNode.Width > szRankBiggestDimension.Width)
                        bIsCurrentNodeDimensionBigger = true;

                    break;
                case Dimension.Height:
                    if (boundsNode.Height > szRankBiggestDimension.Height)
                        bIsCurrentNodeDimensionBigger = true;

                    break;
            }

            return bIsCurrentNodeDimensionBigger;
        }

        #endregion

        #region Interceprions
        /// <summary>
        /// Gets the interception count into ranks.
        /// </summary>
        /// <returns>Count of links interception.</returns>
        private int GetInterceptionCount()
        {
            return GetInterception().Length;
        }
        private Interception[] GetInterception()
        {
            ArrayList lstInterceptions = new ArrayList();
            ArrayList lstParents;
            ArrayList lstChildren;
            int nCurIndex = -1;

            for (int i = 0, nLength = m_lstRankOrdered.Count - 1; i < nLength; i++)
            {
                lstParents = (ArrayList)m_lstRankOrdered[i];
                lstChildren = (ArrayList)m_lstRankOrdered[i + 1];

                foreach (GraphNode leftChild in lstChildren)
                {
                    int nIndex = -1;
                    int nParentIndex = -1;
                    nCurIndex = lstChildren.IndexOf(leftChild);

                    foreach (GraphNode rightParent in leftChild.Parents)
                    {
                        nIndex = lstParents.IndexOf(rightParent);

                        foreach (GraphNode rightChild in lstChildren)
                        {
                            if (leftChild == rightChild || nCurIndex >= lstChildren.IndexOf(rightChild))
                                continue;

                            foreach (GraphNode leftParent in rightChild.Parents)
                            {
                                nParentIndex = lstParents.IndexOf(leftParent);

                                if (nParentIndex >= 0 && nIndex >= 0 && nParentIndex < nIndex)
                                {
                                    bool bSuccess = true;

                                    foreach (Interception interception in lstInterceptions)
                                    {
                                        if ((interception.LeftParent == leftParent && interception.RightParent == rightParent
                                            && interception.LeftChild == leftChild && interception.RightChild == rightChild)
                                            || (interception.LeftParent == rightParent && interception.RightParent == leftParent
                                            && interception.LeftChild == rightChild && interception.RightChild == leftChild))
                                        {
                                            bSuccess = false;
                                            break;
                                        }
                                    }

                                    if (bSuccess)
                                        lstInterceptions.Add(new Interception(leftParent, rightChild, rightParent, leftChild));
                                }
                            }
                        }
                    }
                }
            }

            return (Interception[])lstInterceptions.ToArray(typeof(Interception));
        }

        /// <summary>
        /// Gets the interception count what imposible to remove.
        /// </summary>
        /// <returns>The interception count.</returns>
        private int GetAlwaysInterceptionCount()
        {
            int nCrossCount = 0;
            int nXCrossCount = 0;

            ArrayList lstCrossedNodes = new ArrayList();
            ArrayList lstParentsPassed = new ArrayList();
            ArrayList lstChildrenPassed = new ArrayList();

            // get current and next list in rank list Collection
            for (int n = 0, nLstLegth = m_lstRankOrdered.Count - 1; n < nLstLegth; n++)
            {
                // parent list
                ArrayList lstParentRank = m_lstRankOrdered[n] as ArrayList;
                
                // children list
                ArrayList lstChildrenRank = m_lstRankOrdered[n + 1] as ArrayList;

                // look throwparent
                for (int i = 0, nLength = lstParentRank.Count; i < nLength; i++)
                {
                    GraphNode gnParent = lstParentRank[i] as GraphNode;
                    lstParentsPassed.Add(gnParent);

                    // X CROSS CONDITION
                    // if parent has child, who has a parent, who has a child, who has a parent 
                    // what equals start parent then we have a interception.
                    // check left children
                    foreach (GraphNode gnLeftChild in gnParent.Children)
                    {
                        if (!lstChildrenRank.Contains(gnLeftChild))
                            continue;

                        // chek right parents
                        foreach (GraphNode gnRightParent in gnLeftChild.Parents)
                        {
                            if (gnRightParent != gnParent && !lstParentsPassed.Contains(gnRightParent))
                            {
                                // check right children
                                foreach (GraphNode gnRightChild in gnRightParent.Children)
                                {
                                    // if that child already used skip it
                                    if (!lstChildrenRank.Contains(gnRightChild))
                                        continue;

                                    if (gnRightChild != gnLeftChild)
                                    {
                                        // chek left parents
                                        foreach (GraphNode gnLeftParent in gnRightChild.Parents)
                                        {
                                            // if start parent equals end parent then we have a link interception
                                            if (gnLeftParent == gnParent)
                                            {
                                                nXCrossCount++;
                                                lstCrossedNodes.Add(gnLeftParent);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                lstChildrenPassed.Clear();
                for (int j = 0, count = lstChildrenRank.Count; j < count; j++)
                {
                    GraphNode gnChild = lstChildrenRank[j] as GraphNode;

                    // CHILD PARENTS CONDITION
                    // if we has a node who has three and more parent with 2 
                    // and more children then we have a interception.
                    foreach (GraphNode gnParentNode in gnChild.Parents)
                    {
                        if (lstCrossedNodes.Contains(gnParentNode))
                        {
                            lstChildrenPassed.Clear();
                            break;
                        }

                        int nCount = gnParentNode.Children.Count;

                        if (nCount > 1)
                            lstChildrenPassed.Add(nCount);
                    }

                    if (lstChildrenPassed.Count > 2)
                    {
                        int[] counts = (int[])lstChildrenPassed.ToArray(typeof(int));
                        Array.Sort(counts);

                        for (int k = 0, length = counts.Length - 2; k < length; k++)
                            nCrossCount += counts[k] - 1;
                    }

                    lstChildrenPassed.Clear();
                }
            }

            nCrossCount += nXCrossCount / 2;

            return nCrossCount;
        }

        /// <summary>
        /// Remove all possible interception.
        /// </summary>
        private void OptimizeGraph()
        {
            int nBeforeIterceptionCount = GetInterceptionCount();
            int nAlwaysIterceptionCount = GetAlwaysInterceptionCount();
            int nAfterIterceptionCount = nAlwaysIterceptionCount;
            int nFinallyInterceptionCount = nAfterIterceptionCount;

            ArrayList lstOrderedClone = GetListOrderedClone();

            // optimize by sort nodes in list
            if (nAlwaysIterceptionCount < nBeforeIterceptionCount)
            {
                SortRankNodes();
            }

            OptimizeTopRank();

            Interception[] arrInterceptions = GetInterception();
            nAfterIterceptionCount = arrInterceptions.Length;

            if (nAfterIterceptionCount < nBeforeIterceptionCount)
            {
                lstOrderedClone = GetListOrderedClone();
                nBeforeIterceptionCount = nAfterIterceptionCount;
            }

            if (nAlwaysIterceptionCount < nBeforeIterceptionCount)
            {
                RemoveInterceptions(arrInterceptions);
            }

            // get current interceptions
            nAfterIterceptionCount = GetInterceptionCount();

            // if interception increase then roll back last ordered
            if (nAfterIterceptionCount > nBeforeIterceptionCount)
                m_lstRankOrdered = lstOrderedClone;

            nFinallyInterceptionCount = GetInterceptionCount();
        }

        /// <summary>
        /// Remove interception in first rank list.
        /// </summary>
        private void OptimizeTopRank()
        {
            if (m_lstRankOrdered.Count > 0)
            {
                ArrayList lstRank = m_lstRankOrdered[0] as ArrayList;
                ArrayList lstRankCheck = new ArrayList(lstRank);

                foreach (GraphNode gnNode in lstRankCheck)
                {
                    if (gnNode.Parents.Count == 0 && gnNode.Children.Count == 1)
                    {
                        GraphNode gnChild = (GraphNode)gnNode.Children[0];
                        
                        // place node in center of children parents
                        double dCenterIndex = GetCentralNode(gnChild.Parents, lstRank);

                        MoveNode(lstRank, gnNode, (int)dCenterIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets list ordered clone.
        /// </summary>
        /// <returns>The cloned list.</returns>
        private ArrayList GetListOrderedClone()
        {
            ArrayList lstToReturn = new ArrayList();

            foreach (ArrayList list in m_lstRankOrdered)
            {
                lstToReturn.Add((ArrayList)list.Clone());
            }

            return lstToReturn;
        }
        #endregion

        /// <summary>
        /// Moves Graph to new Location.
        /// </summary>
        /// <param name="ptNewLocation">Location to move to.</param>
        private void MoveGraph(PointF ptNewLocation)
        {
            MoveGraph(ptNewLocation.X - this.Bounds.X + m_fLeftMargin, ptNewLocation.Y - this.Bounds.Y + m_fTopMargin);
        }

        /// <summary>
        /// Enumerates through Graph nodes movig its specified coordinate by specified offset.
        /// </summary>
        /// <param name="fOffsetX">Offset to move across X axis.</param>
        /// <param name="fOffsetY">Offset to move across Y axis.</param>
        private void MoveGraph(float fOffsetX, float fOffsetY)
        {
            Hashtable graphNodes = this.GraphNodes;

            foreach (DictionaryEntry entry in graphNodes)
            {
                IGBounds bounds = entry.Value as IGBounds;

                if (bounds != null)
                {
                    bounds.X += fOffsetX;
                    bounds.Y += fOffsetY;
                }
            }

            // move fictitious nodes
            foreach (GraphNode gnFict in this.FictitiousNodes)
            {
                gnFict.Translate(fOffsetX, fOffsetY);
            }

            m_bBoundsChanged = true;
        }

        /// <summary>
        /// Calculates Graph bounds.
        /// </summary>
        /// <returns>The graph bounds.</returns>
        private RectangleF CalcGraphBounds()
        {
            RectangleF rcToReturn = RectangleF.Empty;
            Hashtable graphNodes = this.GraphNodes;

            if (graphNodes != null && graphNodes.Count > 0)
            {
                ICollection keys = graphNodes.Keys;
                IEnumerator enKeys = keys.GetEnumerator();
                enKeys.MoveNext();

                rcToReturn = ((IGBounds)graphNodes[enKeys.Current]).Bounds;
                RectangleF rcBounds;

                while (enKeys.MoveNext())
                {
                    rcBounds = ((IGBounds)graphNodes[enKeys.Current]).Bounds;
                    rcToReturn = RectangleF.Union(rcToReturn, rcBounds);
                }

                // union with fictitious nodes.
                foreach (GraphNode fictNode in this.FictitiousNodes)
                {
                    rcToReturn = RectangleF.Union(rcToReturn, fictNode.Bounds);
                }
            }

            return rcToReturn;
        }

        /// <summary>
        /// Calculating graph bounds helper method.
        /// </summary>
        /// <param name="gBounds">The bounds.</param>
        /// <param name="side">The box side.</param>
        /// <returns>The graph bounds.</returns>
        private IGBounds UpdateBoundaries(IGBounds gBounds, BoxSide side)
        {
            Hashtable graphNodes = this.GraphNodes;
            IGBounds objGBounds;
            IGBounds objGBoundsToReturn = gBounds;

            bool bSuccess = false;

            foreach (DictionaryEntry obj in graphNodes)
            {
                objGBounds = obj.Value as IGBounds;

                if (objGBoundsToReturn == null)
                    objGBoundsToReturn = objGBounds;

                if (side == BoxSide.Top && gBounds.Bounds.Top < objGBounds.Bounds.Top)
                    bSuccess = true;
                if (side == BoxSide.Bottom && gBounds.Bounds.Bottom > objGBounds.Bounds.Bottom)
                    bSuccess = true;
                if (side == BoxSide.Left && gBounds.Bounds.Left < objGBounds.Bounds.Left)
                    bSuccess = true;
                if (side == BoxSide.Right && gBounds.Bounds.Right > objGBounds.Bounds.Right)
                    bSuccess = true;

                if (bSuccess)
                {
                    objGBoundsToReturn = objGBounds;
                }
            }

            return objGBoundsToReturn;
        }

        /// <summary>
        /// Determines GraphType. Tries to make graph type ordered.
        /// </summary>
        /// <returns>The graph type.</returns>
        private GraphType GetGraphType()
        {
            m_bGraphTypeDetermined = true;

            GraphType typeGraphToReturn = GraphType.OneParentDirectedTree;
            Hashtable graphNodes = this.GraphNodes;
            GraphNode gnNode = null;
            bool bNoneDirected = true;
            bool bMultipleParent = false;

            // search for node without parents.
            foreach (DictionaryEntry entry in graphNodes)
            {
                gnNode = entry.Value as GraphNode;

                if (gnNode != null)
                {
                    if (gnNode.Parents.Count == 0)
                        bNoneDirected = false;

                    if (gnNode.Parents.Count > 1)
                    {
                        bMultipleParent = true;

                        if (!bNoneDirected)
                            break;
                    }
                }
            }

            if (bNoneDirected)
                typeGraphToReturn = GraphType.NonDirectedGraph;
            else if (bMultipleParent)
                typeGraphToReturn = GraphType.MultiParentDirectedTree;

            // 1 - sort nodes by y positions
            RankGraphNodes();

            if (m_lstRankOrdered != null && m_lstRankOrdered.Count > 0)
            {
                // 2 -  add fictitious nodes
                AddFictitiousNodes();

                // 3 - optimize graph. Remove intercetion.
                if (!this.m_improvePerformance)
                {
                    OptimizeGraph();
                }

                // 4 Sort relatives to rank order
                // SortRelatives();
                // 6 - check for dimetions
                CheckRankDimensions();

                // // remove fictitious nodes
                // RemoveFictitiousNodes();
            }

            return typeGraphToReturn;
        }

        /// <summary>
        /// Resizes graph's specified dimension.
        /// </summary>
        /// <param name="fOldValue">Old dimension value.</param>
        /// <param name="fNewValue">New dimension value.</param>
        /// <param name="dimension">Dimension to change.</param>
        private void Resize(float fOldValue, float fNewValue, Dimension dimension)
        {
            float fOffset = 0;
            float fCurrentValue = 0;
            float fChangingDimensionValue = 0;
            float fScaleFactor = fNewValue / fOldValue;

            // 1 - scale nodes
            if (m_bResizeNodes)
                ApplyNodesScaling(fScaleFactor, dimension);

            // 2 - update graph Bounds
            this.m_bBoundsChanged = true;
            RectangleF rectNewBounds = CalcGraphBounds();

            // 3 - change specified dimension value
            if (dimension == Dimension.Width)
                fOffset = fNewValue - rectNewBounds.Width;
            else if (dimension == Dimension.Height)
                fOffset = fNewValue - rectNewBounds.Height;

            if (dimension == Dimension.Width)
            {
                fCurrentValue = this.LeftBoundingNode.Center.X;
                fChangingDimensionValue = this.RightBoundingNode.Center.X - this.LeftBoundingNode.Center.X;
            }
            else if (dimension == Dimension.Height)
            {
                fCurrentValue = this.Bounds.Y;
                fChangingDimensionValue = this.BottomBoundingNode.Center.Y - this.TopBoundingNode.Center.Y;
            }

            foreach (DictionaryEntry entry in this.GraphNodes)
            {
                IGBounds nodeBounds = entry.Value as IGBounds;
                float fTemp = 0;

                if (dimension == Dimension.Width)
                    fTemp = nodeBounds.Center.X - fCurrentValue;
                else if (dimension == Dimension.Height)
                    fTemp = nodeBounds.Center.Y - fCurrentValue;

                float fRatio = 0;
                if (fChangingDimensionValue == 0)
                    fRatio = 0;
                else
                    fRatio = (fTemp * 100F) / fChangingDimensionValue;

                if (dimension == Dimension.Width)
                    nodeBounds.X += (fRatio * fOffset) / 100F;
                else if (dimension == Dimension.Height)
                    nodeBounds.Y += (fRatio * fOffset) / 100F;
            }
        }
        private void ApplyNodesScaling(float fScaleFactor, Dimension dimension)
        {
            foreach (DictionaryEntry entry in this.GraphNodes)
            {
                IGBounds nodeBounds = entry.Value as IGBounds;
                float fTemp = 0;

                if (dimension == Dimension.Width)
                {
                    fTemp = fScaleFactor * nodeBounds.Width;
                    nodeBounds.Width = fTemp;
                }
                else
                {
                    fTemp = fScaleFactor * nodeBounds.Height;
                    nodeBounds.Height = fTemp;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Interface representing Graph Bounds
    /// </summary>
    public interface IGBounds
    {
        /// <summary>
        /// Gets position and size of the Graph.
        /// </summary>
        RectangleF Bounds
        {
            get;
        }

        /// <summary>
        /// Gets or sets position of the Graph
        /// </summary>
        PointF Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets X-coordinate of the location.
        /// </summary>
        float X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Y-coordinate of the location.
        /// </summary>
        float Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        SizeF Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets width of the object.
        /// </summary>
        float Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the object.
        /// </summary>
        float Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets center position of the Graph.
        /// </summary>
        PointF Center
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface to Graph nodes that can translate and rotate.
    /// </summary>
    public interface IGTransform
    {
        /// <summary>
        /// Moves the node by the given X and Y offsets.
        /// </summary>
        /// <param name="dx">Distance to move along X axis.</param>
        /// <param name="dy">Distance to move along Y axis.</param>
        void Translate(float dx, float dy);

        /// <summary>
        /// Rotates the node a specified number of degrees about a given
        /// anchor point.
        /// </summary>
        /// <param name="ptAnchor">Fixed point about which to rotate.</param>
        /// <param name="fDegree">Number of degrees to rotate.</param>
        void RotateAt(PointF ptAnchor, float fDegree);

        /// <summary>
        /// Rotates the node a specified number of degrees about its center point.
        /// </summary>
        /// <param name="fDegree">Number of degrees to rotate.</param>
        void Rotate(float fDegree);
    }

    /// <summary>
    /// Crossing links in graph.
    /// </summary>
    internal class Interception
    {
        private bool bCanFixed;

        public GraphNode LeftParent;
        public GraphNode RightParent;
        public GraphNode LeftChild;
        public GraphNode RightChild;
        public bool CanFixed
        {
            get { return bCanFixed; }
        }

        public Interception(GraphNode leftParent, GraphNode rightChild, GraphNode rightParent, GraphNode leftChild)
        {
            LeftChild = leftChild;
            LeftParent = leftParent;
            RightChild = rightChild;
            RightParent = rightParent;

            bool bSuccess = XCrossCondition();

            if (!bSuccess)
                bSuccess = !ParentParentCondition();

            bCanFixed = !bSuccess;
        }

        /// <summary>
        /// If parent has child, who has a parent, who has a child, who has a parent 
        /// what equals start parent then we have a interception.
        /// </summary>
        /// <returns>true, if x cross condition.</returns>
        private bool XCrossCondition()
        {
            ArrayList lstLeftParents = new ArrayList(LeftChild.Parents);
            ArrayList lstRightParents = new ArrayList(RightChild.Parents);

            return (lstLeftParents.Contains(this.LeftParent) && lstRightParents.Contains(this.RightParent));
        }
        private bool ParentParentCondition()
        {
            bool bSuccess = false;

            if (this.LeftParent.Parents.Count == 1 && this.RightParent.Parents.Count == 1)
            {
                bSuccess = (this.LeftParent.Parents[0] == this.RightParent.Parents[0]);
            }

            return bSuccess;
        }
    }

    internal class InvertIntCompare : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            int nToReturn = 0;

            if (x is int && y is int)
            {
                nToReturn = (int)y - (int)x;
            }

            return nToReturn;
        }

        #endregion
    }
}
