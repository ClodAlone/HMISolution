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
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The RadialTreeLayoutManager is a specialization of the <see cref="Syncfusion.Windows.Forms.Diagram.DirectedTreeLayoutManager"/> 
    /// and employs a circular layout algorithm for laying out the diagram nodes. The <see cref="Syncfusion.Windows.Forms.Diagram.RadialTreeLayoutManager"/> 
    /// positions the root node at the center of the graph, and locates the child nodes in a circular fashion around the 
    /// root. Sub-trees formed by the branching of child nodes are located radially around the child nodes. This 
    /// arrangement results in an ever-expanding concentric arrangement with radial proximity to the root node indicating 
    /// the node level in the hierarchy.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutManager"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.DirectedTreeLayoutManager"/>
    /// </remarks>
    [ToolboxItem(false)]
    public class RadialTreeLayoutManager : DirectedTreeLayoutManager
    {
        #region Initialize / Finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialTreeLayoutManager"/> class.
        /// </summary>
        public RadialTreeLayoutManager()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialTreeLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotationDegree">The angular orientation of the tree.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        public RadialTreeLayoutManager(Model model, float fRotationDegree, float fVerticalOffset, float fHorizontalOffset)
            : base(model, fRotationDegree, fVerticalOffset, fHorizontalOffset)
        { 
        }
        #endregion

        #region Fields
        private PointF m_ptCurrGraphTopNode = PointF.Empty;
        private ArrayList m_hashRankDimensions = null;
        private ArrayList m_lstSectors = null;
        private ArrayList m_lstCurrentSubTree = null;
        #endregion

        #region Properties
        private ArrayList CurrentSubTree
        {
            get
            {
                if (m_lstCurrentSubTree == null)
                    m_lstCurrentSubTree = new ArrayList();

                return m_lstCurrentSubTree;
            }
            set
            {
                if (m_lstCurrentSubTree != value)
                    m_lstCurrentSubTree = value;
            }
        }
        private PointF TopNodeCenter
        {
            get
            {
                return m_ptCurrGraphTopNode;
            }
            set
            {
                if (m_ptCurrGraphTopNode != value)
                    m_ptCurrGraphTopNode = value;
            }
        }
        private ArrayList Sectors
        {
            get
            {
                if (m_lstSectors == null)
                    m_lstSectors = new ArrayList();

                return m_lstSectors;
            }
            set
            {
                if (m_lstSectors != value)
                    m_lstSectors = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Applies the radial directed tree layout strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            Graph graph = this.SelectedNode as Graph;

            if (graph.GraphType == GraphType.OneParentDirectedTree)
            {
                base.MakeGraphLayout(graph);
                m_hashRankDimensions = graph.BiggestDimensions;

                PriorRoutine(graph);
                MakeRadialLayout(graph);
                ClearHeplerDataStructures();

                ApplyRotation(graph);
            }
        }

        #endregion

        #region Helper Methods
        private void ClearHeplerDataStructures()
        {
            this.m_hashRankDimensions.Clear();
            this.Sectors.Clear();
            this.CurrentSubTree.Clear();
            this.TopNodeCenter = PointF.Empty;
        }

        #region Prior routine
        private void PriorRoutine(Graph graph)
        {
            FakeRadialLayout(graph);
            UpdateSectorsData();
        }

        private void UpdateSectorsData()
        {
            float angleRank = 0;
            float fIncreaseTreeHeight = 0;
            for (int nCounter = 1; nCounter < m_hashRankDimensions.Count; nCounter++)
            {
                angleRank = GetRankSectorsAngle(nCounter - 1);

                if (angleRank > 360)
                {
                    float fRadius = GetSubGraphDimension(0, nCounter, Dimension.Height);
                    float fArcLength = (float)((2 * Math.PI * fRadius * angleRank) / 360F);
                    fIncreaseTreeHeight = (float)((fArcLength / (2 * Math.PI)) - fRadius);

                    UpdateSectorsRanksData(fIncreaseTreeHeight, nCounter - 1);
                    IncreaseRanksDimensions(fIncreaseTreeHeight, nCounter);
                }
            }
        }

        private void UpdateSectorsRanksData(float fIncreaseValue, int nRanks)
        {
            Sector sectorCurrent = null;

            for (int nSectorCounter = 0; nSectorCounter < this.Sectors.Count; nSectorCounter++)
            {
                sectorCurrent = this.Sectors[nSectorCounter] as Sector;
                UpdateSectorRanksData(sectorCurrent.Ranking, fIncreaseValue, nRanks);
            }
        }

        private void UpdateSectorRanksData(Hashtable hashSectorRanksData, float fIncreaseValue, int nRanks)
        {
            float fRadiusCurrent = 0;

            if (nRanks > hashSectorRanksData.Count)
                nRanks = hashSectorRanksData.Count;

            for (int nRankCounter = 0; nRankCounter < nRanks; nRankCounter++)
            {
                SectorInfo infoRankSector = hashSectorRanksData[nRankCounter] as SectorInfo;
                fRadiusCurrent = GetSubGraphDimension(0, nRankCounter + 1, Dimension.Height);

                UpdateLeftAngle(infoRankSector, fRadiusCurrent, fIncreaseValue / (nRanks - nRankCounter));
                UpdateRightAngle(infoRankSector, fRadiusCurrent, fIncreaseValue / (nRanks - nRankCounter));
            }

            UpdateRestRanksData(nRanks, hashSectorRanksData, fIncreaseValue);
        }

        private void UpdateRestRanksData(int nRanks, Hashtable hashSectorRanksData, float fIncreaseValue)
        {
            float fRadiusCurrent;
            
            // Update rest ranks
            for (int nCounter = nRanks; nCounter < hashSectorRanksData.Count; nCounter++)
            {
                SectorInfo infoRankSector = hashSectorRanksData[nCounter] as SectorInfo;
                fRadiusCurrent = GetSubGraphDimension(0, nCounter + 1, Dimension.Height);

                UpdateLeftAngle(infoRankSector, fRadiusCurrent, fIncreaseValue);
                UpdateRightAngle(infoRankSector, fRadiusCurrent, fIncreaseValue);
            }
        }

        private void UpdateRightAngle(SectorInfo infoRankSector, float fRadiusCurrent, float fIncreaseValue)
        {
            float fArcLength;
            float fRadiusNew;
            fArcLength = CalcArcLength(-infoRankSector.RightAngle, fRadiusCurrent);
            fRadiusNew = fRadiusCurrent + fIncreaseValue;
            infoRankSector.RightAngle = -CalcAngle(fArcLength, fRadiusNew);
        }

        private void UpdateLeftAngle(SectorInfo infoRankSector, float fRadiusCurrent, float fIncreaseValue)
        {
            float fArcLength;
            float fRadiusNew;
            fArcLength = CalcArcLength(infoRankSector.LeftAngle, fRadiusCurrent);
            fRadiusNew = fRadiusCurrent + fIncreaseValue;
            infoRankSector.LeftAngle = CalcAngle(fArcLength, fRadiusNew);
        }

        private float CalcAngle(float fArcLength, float fRadius)
        {
            return (float)((fArcLength * 360F) / (2 * Math.PI * fRadius));
        }

        private float CalcArcLength(float fAngle, float fRadius)
        {
            return (float)((2 * Math.PI * fRadius * fAngle) / 360F);
        }

        private float GetRankSectorsAngle(int nRank)
        {
            float fAngleToReturn = 0;
            foreach (Sector currentSector in this.Sectors)
            {
                SectorInfo sectorInfo = null;
                if (currentSector.Ranking.Count <= nRank)
                    sectorInfo = currentSector.Ranking[currentSector.Ranking.Count - 1] as SectorInfo;
                else
                    sectorInfo = currentSector.Ranking[nRank] as SectorInfo;

                fAngleToReturn += sectorInfo.SectorAngle;
            }

            return fAngleToReturn;
        }

        private void IncreaseRanksDimensions(float fValueToIncreaseBy, int nRank)
        {
            float fIncreaseBy = fValueToIncreaseBy / nRank;

            for (int nCounter = 0; nCounter < nRank; nCounter++)
            {
                // get size
                SizeF szRankSize = (SizeF)m_hashRankDimensions[nCounter];

                if (nCounter == 0)
                    szRankSize.Height += fIncreaseBy * 2;
                else
                    szRankSize.Height += fIncreaseBy;

                // set size
                m_hashRankDimensions[nCounter] = szRankSize;
            }
        }

        private void FakeRadialLayout(Graph graphSorting)
        {
            GetTopNodeCenter(graphSorting);

            ArrayList lstOrdered = graphSorting.TypeOrdered;

            if (lstOrdered.Count > 1)
            {
                float fRadius = GetSubGraphDimension(0, 1, Dimension.Height);

                ArrayList lstRank = (ArrayList)lstOrdered[1];
                FakeRankRadialLayout(lstRank, fRadius, 1);
            }
        }

        private void FakeRankRadialLayout(ArrayList lstCurrRank, float fRadius, int nRank)
        {
            float halfLength = 0;
            float angle = 0;
            foreach (GraphNode rankMember in lstCurrRank)
            {
                Sector sectorCurrent = new Sector();
                this.Sectors.Add(sectorCurrent);

                halfLength = (rankMember.Width + this.HorizontalSpacing) / 2;
                angle = (float)(Math.Atan(halfLength / fRadius) * (180 / Math.PI));

                SectorInfo sectorRankInfo = new SectorInfo();
                sectorRankInfo.RightAngle = -angle;
                sectorRankInfo.LeftAngle = angle;

                sectorCurrent.Ranking.Add(0, sectorRankInfo);
                sectorCurrent.SectorHeight = 2;

                if (rankMember.Children.Count > 0)
                {
                    FakeChildrenRadialLayout(rankMember, nRank + 1, 0, sectorCurrent);
                }

                CheckSector(sectorCurrent);
            }
        }

        private void CheckSector(Sector sectorCurrent)
        {
            for (int nCounter = 0; nCounter < sectorCurrent.Ranking.Count - 1; nCounter++)
            {
                SectorInfo sectorInfo = sectorCurrent.Ranking[nCounter] as SectorInfo;
                SectorInfo sectorInfoNext = sectorCurrent.Ranking[nCounter + 1] as SectorInfo;

                if (sectorInfo.LeftAngle > sectorInfoNext.LeftAngle)
                    sectorInfoNext.LeftAngle = sectorInfo.LeftAngle;

                if (sectorInfo.RightAngle < sectorInfoNext.RightAngle)
                    sectorInfoNext.RightAngle = sectorInfo.RightAngle;
            }
        }

        private void FakeChildrenRadialLayout(GraphNode dtgnNode, int nRank, float angleParent, Sector sectorCurrent)
        {
            float angle = 0;
            float rotate = 0;

            SectorInfo sectorPreviuosInfo = sectorCurrent.Ranking[nRank - 2] as SectorInfo;
            SectorInfo sectorCurrentInfo = new SectorInfo();
            sectorCurrentInfo.LeftAngle = sectorPreviuosInfo.LeftAngle;
            sectorCurrentInfo.RightAngle = sectorPreviuosInfo.RightAngle;
            if (!sectorCurrent.Ranking.ContainsKey(nRank - 1))
            {
                sectorCurrent.Ranking.Add(nRank - 1, sectorCurrentInfo);
                sectorCurrent.SectorHeight++;
            }

            float fRadius = GetSubGraphDimension(0, nRank, Dimension.Height);

            foreach (GraphNode nodeChild in dtgnNode.Children)
            {
                angle = CalculateRotationAngle(dtgnNode, fRadius, nodeChild);
                rotate = angleParent + angle;

                CheckLeftAngle(dtgnNode, fRadius, nodeChild, angleParent, sectorCurrentInfo, sectorCurrent, nRank);
                CheckRightAngle(dtgnNode, fRadius, nodeChild, angleParent, sectorCurrentInfo, sectorCurrent, nRank);

                if (nodeChild.Children.Count > 0)
                {
                    FakeChildrenRadialLayout(nodeChild, nRank + 1, rotate, sectorCurrent);
                }
            }
        }

        private void CheckRightAngle(GraphNode dtgnNode, float fRadius, IGBounds nodeChild, float angleParent, SectorInfo sectorCurrentInfo, Sector sectorCurrent, int nRank)
        {
            float fSectorAngle = CalculateFakeRightRotationAngle(dtgnNode, fRadius, nodeChild);
            fSectorAngle += angleParent;

            if ((sectorCurrentInfo.RightAngle < 0) && (sectorCurrentInfo.RightAngle > fSectorAngle))
                sectorCurrentInfo.RightAngle = fSectorAngle;
        }

        private void CheckLeftAngle(GraphNode dtgnNode, float fRadius, IGBounds nodeChild, float angleParent, SectorInfo sectorCurrentInfo, Sector sectorCurrent, int nRank)
        {
            float fSectorAngle = CalculateFakeLeftRotationAngle(dtgnNode, fRadius, nodeChild);
            fSectorAngle += angleParent;

            if ((sectorCurrentInfo.LeftAngle > 0) && (sectorCurrentInfo.LeftAngle < fSectorAngle))
                sectorCurrentInfo.LeftAngle = fSectorAngle;
        }

        private float CalculateFakeLeftRotationAngle(GraphNode dtgnNode, float fRadius, IGBounds nodeChild)
        {
            float fArcLength = dtgnNode.Center.X - (nodeChild.X - this.HorizontalSpacing / 2);
            return (float)((360 * fArcLength) / (2 * Math.PI * fRadius));
        }

        private float CalculateFakeRightRotationAngle(GraphNode dtgnNode, float fRadius, IGBounds nodeChild)
        {
            float fArcLength = dtgnNode.Center.X - ((nodeChild.X + nodeChild.Width) + this.HorizontalSpacing / 2);
            return (float)((360 * fArcLength) / (2 * Math.PI * fRadius));
        }

        #endregion Prior routine

        #region Radial Layout
        private void MakeRadialLayout(Graph graphSorting)
        {
            if (graphSorting == null)
                throw new ArgumentNullException("graphSorting", "This argument cannot be null.");

            GetTopNodeCenter(graphSorting);

            ArrayList lstOrdered = graphSorting.TypeOrdered;

            if (lstOrdered.Count > 1)
            {
                float fRadius = 0;
                ArrayList lstRank = null;

                fRadius = GetSubGraphDimension(0, 1, Dimension.Height);
                lstRank = (ArrayList)lstOrdered[1];
                MakeRankRadialLayout(lstRank, fRadius, 1);
            }
        }

        private void MakeRankRadialLayout(ArrayList lstCurrRank, float fRadius, int nRank)
        {
            if (lstCurrRank == null)
                throw new ArgumentNullException("lstCurrRank", "lstCurrRank argument cannot be null.");

            PointF ptChorda = new PointF(this.TopNodeCenter.X, this.TopNodeCenter.Y + fRadius);
            float angle = 0;

            for (int nCounter = 0; nCounter < lstCurrRank.Count; nCounter++)
            {
                IGBounds rankMember = lstCurrRank[nCounter] as IGBounds;
                Sector sectorCurrent = this.Sectors[nCounter] as Sector;
                float fCurrentSectorAngle = sectorCurrent.SectorAngle;

                this.CurrentSubTree.Add(rankMember);
                GraphNode gnNode = rankMember as GraphNode;

                if (gnNode != null && gnNode.Children.Count > 0)
                {
                    MakeChildrenRadialLayout(gnNode, nRank + 1, 0);
                }

                rankMember.Center = ptChorda;

                float fright = sectorCurrent.LeftAngle;

                RotateSubTree(angle + fright, this.TopNodeCenter);
                this.CurrentSubTree.Clear();
                angle += fCurrentSectorAngle;
            }
        }

        private void RotateSubTree(float fRotationAngle, PointF ptRotateAt)
        {
            foreach (IGTransform gnSubTreeMember in this.CurrentSubTree)
            {
                gnSubTreeMember.RotateAt(ptRotateAt, fRotationAngle);
                
                // INode node = ( ( DirectedTreeGraphNode )gnSubTreeMember ).Node as INode;
                // ( ( ITransform ) node ).Rotate( fRotationAngle );
            }
        }

        private void MakeChildrenRadialLayout(GraphNode dtgnNode, int nRank, float angleParent)
        {
            if (dtgnNode == null)
                throw new ArgumentNullException("dtgnNode", "dtgnNode argument cannot be null.");

            float fRadius = GetSubGraphDimension(0, nRank, Dimension.Height);
            foreach (IGBounds nodeChild in dtgnNode.Children)
            {
                if (nodeChild == null)
                    throw new NullReferenceException("Graph node relatives must implement IGBounds interface.");

                // sub tree
                this.CurrentSubTree.Add(nodeChild);

                float angle = CalculateRotationAngle(dtgnNode, fRadius, nodeChild);
                float rotate = angleParent + angle;
                GraphNode gnNode = nodeChild as GraphNode;

                if (gnNode != null && gnNode.Children.Count > 0)
                {
                    MakeChildrenRadialLayout(gnNode, nRank + 1, rotate);
                }

                nodeChild.Center = new PointF(this.TopNodeCenter.X, this.TopNodeCenter.Y + fRadius);
                ((IGTransform)nodeChild).RotateAt(this.TopNodeCenter, -rotate);

                // INode node = ( ( DirectedTreeGraphNode )nodeChild ).Node as INode;
                // ( ( ITransform ) node ).Rotate( -rotate );
            }
        }

        private float CalculateRotationAngle(GraphNode dtgnNode, float fRadius, IGBounds nodeChild)
        {
            float fArcLength = dtgnNode.Center.X - nodeChild.Center.X;
            return (float)((fArcLength * 360F) / (2 * Math.PI * fRadius));
        }

        /// <summary>
        /// Gets the sub graph dimension.
        /// </summary>
        /// <param name="nFromRank">The rank from.</param>
        /// <param name="nToRank">The rank to.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns>The dimension.</returns>
        protected float GetSubGraphDimension(int nFromRank, int nToRank, Dimension dimension)
        {
            if (nFromRank < 0)
                throw new ArgumentOutOfRangeException("nFromRank", "nFromRank value cannot be less than zero.");

            if (nToRank < 0)
                throw new ArgumentOutOfRangeException("nToRank", "nToRank value cannot be less than zero.");

            float fGraphDimensionToReturn = 0;
            int nCounter = nFromRank;
            int nIncrease = 1;

            if (nFromRank > nToRank)
                nIncrease = -nIncrease;

            SizeF szRankBiggestDimensions = (SizeF)m_hashRankDimensions[nCounter];
            fGraphDimensionToReturn += szRankBiggestDimensions.Height / 2;
            nCounter += nIncrease;

            while (nCounter <= nToRank)
            {
                if (m_hashRankDimensions.Count <= nCounter)
                    break;

                szRankBiggestDimensions = (SizeF)m_hashRankDimensions[nCounter];

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

            szRankBiggestDimensions = (SizeF)m_hashRankDimensions[nCounter - 1];
            fGraphDimensionToReturn -= szRankBiggestDimensions.Height / 2;

            return fGraphDimensionToReturn;
        }

        /// <summary>
        /// Gets top node from th given graph.
        /// </summary>
        /// <param name="graphSorting">graph to search top node in.</param>
        private void GetTopNodeCenter(Graph graphSorting)
        {
            if (graphSorting == null)
                throw new ArgumentNullException("graphSorting", "This argument cannot be null.");

            GraphNode gnTop = graphSorting.GetGraphFirstTopNode();
            this.TopNodeCenter = gnTop.Center;
        }

        #endregion Radial Layout

        #endregion
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Sector
    {
        #region Fields
        public Hashtable m_hashRanks = null;
        public int m_nHeight = 0;
        #endregion Fields

        #region Properties
        public Hashtable Ranking
        {
            get
            {
                if (m_hashRanks == null)
                    m_hashRanks = new Hashtable();

                return m_hashRanks;
            }
            set
            {
                if (m_hashRanks != value)
                    m_hashRanks = value;
            }
        }

        public float LeftAngle
        {
            get
            {
                return GetLeftAngle();
            }
        }

        public float RightAngle
        {
            get
            {
                return GetRightAngle();
            }
        }

        public float SectorAngle
        {
            get
            {
                return this.LeftAngle + (-this.RightAngle);
            }
        }

        public int SectorHeight
        {
            get
            {
                return m_nHeight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Height cannot be less than zero.");

                if (m_nHeight != value)
                    m_nHeight = value;
            }
        }
        #endregion

        #region Public methods

        #endregion Public methods

        #region Helper methods
        private float GetLeftAngle()
        {
            float fAngleToReturn = 0;
            SectorInfo currentInfo = null;

            for (int nCounter = 0; nCounter < this.Ranking.Count; nCounter++)
            {
                currentInfo = this.Ranking[nCounter] as SectorInfo;
                if (fAngleToReturn < currentInfo.LeftAngle)
                    fAngleToReturn = currentInfo.LeftAngle;
            }

            return fAngleToReturn;
        }
        private float GetRightAngle()
        {
            float fAngleToReturn = 0;
            SectorInfo currentInfo = null;

            for (int nCounter = 0; nCounter < this.Ranking.Count; nCounter++)
            {
                currentInfo = this.Ranking[nCounter] as SectorInfo;
                if (fAngleToReturn > currentInfo.RightAngle)
                    fAngleToReturn = currentInfo.RightAngle;
            }

            return fAngleToReturn;
        }
        #endregion Helper methods
    }

    /// <summary>
    /// Helper class containing sector angle values
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SectorInfo
    {
        #region Fields
        private float m_fRightAngle = 0;
        private float m_fLeftAngle = 0;
        #endregion Fields

        #region Properties
        public float LeftAngle
        {
            get
            {
                return m_fLeftAngle;
            }
            set
            {
                if (m_fLeftAngle != value)
                    m_fLeftAngle = value;
            }
        }
        public float RightAngle
        {
            get
            {
                return m_fRightAngle;
            }
            set
            {
                if (m_fRightAngle != value)
                    m_fRightAngle = value;
            }
        }
        public float SectorAngle
        {
            get { return m_fLeftAngle + (-m_fRightAngle); }
        }
        #endregion Properties
    }
}