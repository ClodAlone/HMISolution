#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Force node for symmetric layout manager.
    /// Used only in <ref>Syncfusion.Windows.Forms.Diagram.SymmetricLayoutManager</ref>
    /// </summary>
    internal class GraphForceNode
    {
        #region Class members
        private float m_fVelocityX;
        private float m_fVelocityY;
        private PointF m_ptLocation;
        private ArrayList m_lstNodes;
        private GraphNode m_gnNode;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets reference to graph node what contain this forcenode.
        /// </summary>
        public GraphNode GraphNode
        {
            get { return m_gnNode; }
        }

        /// <summary>
        /// Gets all connected nodes to this node.
        /// </summary>
        public ArrayList Nodes
        {
            get { return m_lstNodes; }
        }

        /// <summary>
        /// Gets or sets current force node location.
        /// </summary>
        /// <remarks>
        /// This location can be not equal to graphNode location.
        /// But after call public AppendChanges() methods this location
        /// append to graphNode location.
        /// </remarks>
        public PointF Location
        {
            get { return m_ptLocation; }
            set { m_ptLocation = value; }
        }

        /// <summary>
        /// Gets or sets force velocity by x axis.
        /// </summary>
        public float VelocityX
        {
            get { return m_fVelocityX; }
            set { m_fVelocityX = value; }
        }

        /// <summary>
        /// Gets or sets force velocity by y axis.
        /// </summary>
        public float VelocityY
        {
            get { return m_fVelocityY; }
            set { m_fVelocityY = value; }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphForceNode"/> class.
        /// </summary>
        /// <param name="gnNode">The GraphNode container.</param>
        public GraphForceNode(GraphNode gnNode)
        {
            m_gnNode = gnNode;
            m_ptLocation = gnNode.Center;

            m_lstNodes = new ArrayList();
            m_lstNodes.AddRange(gnNode.Parents);
            m_lstNodes.AddRange(gnNode.Children);
        }
        #endregion

        #region Public mathods
        /// <summary>
        /// Apply force position changes to graph position.
        /// </summary>
        public void ApplyChanges()
        {
            m_gnNode.Center = m_ptLocation;
        }
        #endregion
    }
}
