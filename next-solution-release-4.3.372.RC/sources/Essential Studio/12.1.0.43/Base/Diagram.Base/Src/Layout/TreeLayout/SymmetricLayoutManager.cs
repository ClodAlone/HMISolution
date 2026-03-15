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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The SymmetricTreeLayoutManager is a specialization of the and employs a 
    /// force-directed layout algorithm for laying out the diagram nodes. 
    /// Where nodes positions by spring, angle and neighbour forces and moved to vector force sum.
    /// </summary>
    /// <remarks>
    /// This layout use special force node <see cref="Syncfusion.Windows.Forms.Diagram.GraphForceNode"/>
    /// what added to graphNode tag.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutManager"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/>
    /// </remarks>
    [ToolboxItem(false)]
    public class SymmetricLayoutManager : GraphLayoutManager
    {
        #region Class constants
        /// <summary>
        /// Spring node constant;
        /// </summary>
        /// <remarks>
        /// Value generate by testing.
        /// </remarks>
        private const double c_dCOEF = 0.442;
        private const float c_fMAX_VELOCITY = 50f;
        private const int c_nMAX_ITERACTION = 1000;
        private const int c_nSPRING_LENGTH = 100;
        #endregion

        #region Class members
        private SizeF m_szMaxForceVelocity = new SizeF(c_fMAX_VELOCITY, c_fMAX_VELOCITY);

        /// <summary>
        /// Minimum distance between connected nodes.
        /// </summary>
        private float m_fVertexDistance;

        private double m_dSpringFactor = c_dCOEF;

        /// <summary>
        /// Max count of iteractions.
        /// </summary>
        private int m_nMaxIteraction = c_nMAX_ITERACTION;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the length of the spring.
        /// </summary>
        /// <value>The length of the spring.</value>
        public float SpringLength
        {
            get { return m_fVertexDistance; }
            set { m_fVertexDistance = value; }
        }

        /// <summary>
        /// Gets or sets the spring factor.
        /// </summary>
        /// <value>The spring factor.</value>
        public double SpringFactor
        {
            get { return m_dSpringFactor; }
            set { m_dSpringFactor = value; }
        }

        /// <summary>
        /// Gets or sets the max count of iteration.
        /// </summary>
        /// <value>The max iteration.</value>
        public int MaxIteraction
        {
            get { return m_nMaxIteraction; }
            set { m_nMaxIteraction = value; }
        }
        #endregion

        #region Initialize / Finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricLayoutManager"/> class.
        /// </summary>
        public SymmetricLayoutManager()
            : base()
        {
            m_fVertexDistance = c_nSPRING_LENGTH;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fVerticalDistance">Specifies the distance between adjacent nodes.</param>
        public SymmetricLayoutManager(Model model, float fVerticalDistance)
            : base(model, 0, 0)
        {
            m_fVertexDistance = fVerticalDistance;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Applies the radial directed tree layout strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            Graph graph = this.SelectedNode as Graph;
            ICollection lstGraphNodes = graph.GraphNodes.Values;

            // update graphNode by add forceNode parameter as tag
            ArrayList lstNodes = ConvertGraphNodes(lstGraphNodes);
            int count = lstNodes.Count;
            count = Math.Min(m_nMaxIteraction, count * count * count);

            // move nodes by circle before start main lay out
            PreLayoutNodes(lstNodes, graph.Bounds);

            // layout nodes in collection
            for (int i = 0, nLenght = count; i < nLenght; i++)
            {
                // calc nodes force
                MakeSymmetricLayout(lstNodes);
                
                // append changes
                AppendForces(lstNodes);
            }

            ResetGraphPosition(lstNodes, graph);
            graph.RecalculateBounds();
        }

        /// <summary>
        /// Place nodes in circle.
        /// </summary>
        /// <param name="lstNodes">The nodes to move.</param>
        /// <param name="rcBounds">The nodes bounds.</param>
        private void PreLayoutNodes(ArrayList lstNodes, RectangleF rcBounds)
        {
            float fMaxSize = Math.Max(rcBounds.Width, rcBounds.Height);
            PointF ptCenter = new PointF(fMaxSize / 2, fMaxSize / 2);
            double dRotateAngle = 2f * Math.PI / lstNodes.Count;
            double dAngle = dRotateAngle;

            foreach (GraphNode gnNode in lstNodes)
            {
                GraphForceNode forceNode = GetForceNode(gnNode);
                forceNode.Location = new PointF(ptCenter.X + fMaxSize * (float)Math.Cos(dAngle), ptCenter.Y + fMaxSize * (float)Math.Sin(dAngle));
                dAngle -= dRotateAngle;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Applies the symmetric layout to given nodes.
        /// </summary>
        /// <param name="lstNodes">The nodes list.</param>
        private void MakeSymmetricLayout(ArrayList lstNodes)
        {
            GraphForceNode forceNode;
            GraphForceNode force;

            foreach (GraphNode gnNode in lstNodes)
            {
                forceNode = GetForceNode(gnNode);
                ArrayList nodes = forceNode.Nodes;

                // calc spring forces
                foreach (GraphNode gnChild in nodes)
                {
                    if (lstNodes.Contains(gnChild))
                    {
                        CalcNodesForce(forceNode, GetForceNode(gnChild));
                    }
                }

                // calc forces for relates
                for (int i = 0, length = nodes.Count; i < length; i++)
                {
                    if (length < 2)
                        break;

                    GraphForceNode vtx1 = GetForceNode(nodes[i] as GraphNode);
                    GraphForceNode vtx2 = (i + 1 >= length)
                        ? GetForceNode(nodes[0] as GraphNode)
                        : GetForceNode(nodes[i + 1] as GraphNode);

                    double angle = (double)(360 / nodes.Count / 2) * Math.PI / 180;
                    double distance = Geometry.PointDistance(vtx1.Location, vtx2.Location);
                    double normalDistance = 2 * m_fVertexDistance * Math.Sin(angle);

                    CalcRelatesForce(vtx1, vtx2, normalDistance);
                }

                // calc neibour forces
                foreach (GraphNode gnChild in lstNodes)
                {
                    if (!nodes.Contains(gnChild) && gnChild != gnNode)
                    {
                        force = GetForceNode(gnChild);
                        UpdateNeigbour(forceNode, force);
                    }
                }
            }
        }

        /// <summary>
        /// Offset nodes to its force vectors.
        /// </summary>
        /// <param name="lstNodes">The nodes list.</param>
        private void AppendForces(ArrayList lstNodes)
        {
            GraphForceNode gfnNode = null;

            foreach (GraphNode gnNode in lstNodes)
            {
                gfnNode = GetForceNode(gnNode);

                PointF ptPoint = gfnNode.Location;
                
                // offset location to velocity
                ptPoint.X += Math.Min(gfnNode.VelocityX, m_szMaxForceVelocity.Width);
                ptPoint.Y += Math.Min(gfnNode.VelocityY, m_szMaxForceVelocity.Height);

                // reset velocity values
                gfnNode.VelocityX = 0;
                gfnNode.VelocityY = 0;

                // set new position
                gfnNode.Location = ptPoint;
            }
        }

        /// <summary>
        /// Place graph to 0,0 position by move graphNodes.
        /// </summary>
        /// <param name="lstNodes">The nodes list.</param>
        /// <param name="graph">The graph.</param>
        private void ResetGraphPosition(ArrayList lstNodes, Graph graph)
        {
            SizeF szMin = new SizeF(float.MaxValue, float.MaxValue);
            GraphForceNode gfnNode = null;

            // calc minimum position in graph
            foreach (GraphNode gnNode in lstNodes)
            {
                gfnNode = GetForceNode(gnNode);
                PointF ptLocation = new PointF(gfnNode.Location.X - gnNode.Size.Width / 2, gfnNode.Location.Y - gnNode.Size.Height / 2);
                szMin.Width = Math.Min(szMin.Width, ptLocation.X);
                szMin.Height = Math.Min(szMin.Height, ptLocation.Y);
            }

            // append all changes with graph offset
            foreach (GraphNode gnNode in lstNodes)
            {
                gfnNode = GetForceNode(gnNode);

                PointF ptLocation = gfnNode.Location;
                ptLocation.X -= szMin.Width - graph.Location.X;
                ptLocation.Y -= szMin.Height - graph.Location.Y;
                gfnNode.Location = ptLocation;

                gfnNode.ApplyChanges();
            }
        }

        /// <summary>
        /// Convert the graph ordered list node one list with adding force parameter as tag..
        /// </summary>
        /// <param name="lstNodes">The rank order list nodes.</param>
        /// <returns>The graph ordered</returns>
        private ArrayList ConvertGraphNodes(ICollection lstNodes)
        {
            ArrayList lstToReturn = new ArrayList();

            foreach (GraphNode gnNode in lstNodes)
            {
                GraphForceNode forceNode = new GraphForceNode(gnNode);
                gnNode.Tag = forceNode;
                lstToReturn.Add(gnNode);
            }

            return lstToReturn;
        }

        /// <summary>
        /// Get force node from graphNode.
        /// </summary>
        /// <remarks>
        /// Force node contain parameter needed for this layout type.
        /// </remarks>
        /// <param name="gnNode">The graph node.</param>
        /// <returns>The force node</returns>
        private GraphForceNode GetForceNode(GraphNode gnNode)
        {
            return (GraphForceNode)gnNode.Tag;
        }

        /// <summary>
        /// Calc force for neigbours.
        /// </summary>
        /// <param name="vtSource">The source force node.</param>
        /// <param name="vtTarget">The target force node.</param>
        private void UpdateNeigbour(GraphForceNode vtSource, GraphForceNode vtTarget)
        {
            if (vtTarget == null || vtSource == null)
                return;

            double distanse = Geometry.PointDistance(vtSource.Location, vtTarget.Location);
            double angle = Geometry.LineAngle(vtSource.Location, vtTarget.Location);

            int normalDistance = (int)(m_fVertexDistance * 0.9);

            if (distanse < normalDistance)
            {
                CalcForce(distanse, normalDistance, angle, vtTarget);
            }
        }

        /// <summary>
        /// Calcs the relates force.
        /// </summary>
        /// <param name="vtSource">The source force node.</param>
        /// <param name="vtTarget">The target force node.</param>
        /// <param name="normalDistance">The minimum distance.</param>
        private void CalcRelatesForce(GraphForceNode vtSource, GraphForceNode vtTarget, double normalDistance)
        {
            double distance = Geometry.PointDistance(vtSource.Location, vtTarget.Location);
            double angle = Geometry.LineAngle(vtSource.Location, vtTarget.Location);

            if (distance < normalDistance)
            {
                CalcForce(distance, normalDistance, angle, vtTarget);
            }
        }

        /// <summary>
        /// Calcs the spring force for nodes.
        /// </summary>
        /// <param name="vtSource">The source force node.</param>
        /// <param name="vtTarget">The target force node.</param>
        private void CalcNodesForce(GraphForceNode vtSource, GraphForceNode vtTarget)
        {
            double distanse = Geometry.PointDistance(vtSource.Location, vtTarget.Location);
            double angle = Geometry.LineAngle(vtSource.Location, vtTarget.Location);

            if (distanse > m_fVertexDistance || distanse < m_fVertexDistance)
            {
                CalcForce(distanse, m_fVertexDistance, angle, vtTarget);
            }
        }

        /// <summary>
        /// Calc force for node.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="minDist">The min distance.</param>
        /// <param name="angle">The angle between two edges.</param>
        /// <param name="vtTarget">The one of edge node.</param>
        private void CalcForce(double distance, double minDist, double angle, GraphForceNode vtTarget)
        {
            double count = (double)vtTarget.Nodes.Count;
            double length = distance - minDist;
            double factor = m_dSpringFactor / (count * count) * Math.Sqrt(count);

            if (length != 0)
            {
                double fVelocity = length * factor;

                // append valocity to position
                double fOffset = fVelocity;
                double offsetX = Math.Cos(angle) * fOffset;
                double offsetY = Math.Sin(angle) * fOffset;

                vtTarget.VelocityX -= (float)offsetX;
                vtTarget.VelocityY -= (float)offsetY;
            }
        }

        #endregion
    }
}