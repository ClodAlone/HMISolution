// <copyright file="PathNode.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region File using derectives

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#endregion

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// The helper point that the grid of the findpaths' possible ways consists of.
    /// </summary>
    internal abstract class SearchNode
    {
        #region Fields
        private bool m_bProcessed;

        /// <summary>
        /// Indicates whether F or G value will be calculated in
        /// <see cref="GetMoveCost"/> method.
        /// </summary>
        private bool m_bGCalcFlag;

        /// <summary>
        /// Indicates whether F or G value will be calculated in
        /// <see cref="GetHeuristicCost"/> method.
        /// </summary>
        private bool m_bHCalcFlag;

        /// <summary>
        /// Heuristic.Estimate of what it will cost to get to the goal node.
        /// Sum of all the costs it will take to get to the goal( Total path estimate ).
        /// </summary>
        /// <remarks>
        /// F = G + H = gone + heuristic.
        /// </remarks>
        private double m_fH;

        /// <summary>
        /// Sum off all the costs it took to get here.
        /// </summary>
        private double m_fG;

        /// <summary>
        /// PathNode location. In model coordinates.
        /// </summary>
        private System.Windows.Point m_ptLocation;

        /// <summary>
        /// Parent node.
        /// </summary>
        /// <remarks>
        /// Used to get route vertices when path found.
        /// </remarks>
        private SearchNode m_sgnParent;

        /// <summary>
        /// Possible route succeding nodes.
        /// </summary>        
        private List<SearchNode> m_lstNeighbours;
        #endregion

        #region Initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchNode"/> class.
        /// </summary>
        /// <param name="ptLocation">The point location.</param>
        public SearchNode(System.Windows.Point ptLocation)
        {
            m_ptLocation = ptLocation;
            m_bGCalcFlag = true;
            m_bHCalcFlag = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchNode"/> is processed.
        /// </summary>
        /// <value><c>true</c> if processed; otherwise, <c>false</c>.</value>
        public bool Processed
        {
            get { return m_bProcessed; }
            set { m_bProcessed = value; }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public SearchNode Parent
        {
            get 
            { 
                return m_sgnParent; 
            }
            set
            {
                m_sgnParent = value;
                ResetFGCalculations();

                if (m_sgnParent != null)
                {
                    foreach (SearchNode node in m_sgnParent.Neighbours)
                    {
                        node.ResetFGCalculations();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the neighbours.
        /// </summary>
        /// <value>The neighbours.</value>
        public List<SearchNode> Neighbours
        {
            get
            {
                if (m_lstNeighbours == null)
                {
                    m_lstNeighbours = new List<SearchNode>();
                }

                return m_lstNeighbours;
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public System.Windows.Point Location
        {
            get { return m_ptLocation; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the grid heuristic cost.
        /// </summary>
        /// <param name="from">Node to calc heuristic from.</param>
        /// <param name="to">Node to calc heuristic to.</param>
        /// <returns>Heuristic from from to to.</returns>
        public static double GetGridHeuristicCost(SearchNode from, SearchNode to)
        {
            return Math.Abs(from.Location.X - to.Location.X) + Math.Abs(from.Location.Y - to.Location.Y);
        }

        /// <summary>
        /// Gets the grid move cost.
        /// </summary>
        /// <param name="from">From node.</param>
        /// <param name="to">To node.</param>
        /// <returns>The value.</returns>
        public static double GetGridMoveCost(SearchNode from, SearchNode to)
        {
            double fReturnValue = Math.Abs(from.Location.X - to.Location.X) + Math.Abs(from.Location.Y - to.Location.Y);

            if (from.Parent != null)
            {
                bool b = (from.Location.Y == from.Parent.Location.Y)
                    ? (from.Location.Y != to.Location.Y)
                    : (from.Location.X != to.Location.X);

                fReturnValue += b ? 10 : 0;
            }
            return fReturnValue;
        }

        /// <summary>
        /// Gets the graph move cost.
        /// </summary>
        /// <param name="from">The source node.</param>
        /// <param name="to">The target node.</param>
        /// <returns>The value.</returns>
        public static float GetGraphMoveCost(SearchNode from, SearchNode to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the graph heuristic cost.
        /// </summary>
        /// <param name="from">From node.</param>
        /// <param name="to">To node.</param>
        /// <returns>The value.</returns>
        public static float GetGraphHeuristicCost(SearchNode from, SearchNode to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets CalcFlag to true.
        /// </summary>
        public void ResetFGCalculations()
        {
            m_bGCalcFlag = true;
            m_bHCalcFlag = true;
        }

        /// <summary>
        /// Calculates G value for this node from given node.
        /// </summary>
        /// <returns>G value.</returns>
        public double GetMoveCost()
        {
            if (m_bGCalcFlag)
            {
                if (this.Parent == null)
                {
                    m_fG = 0;
                }
                else
                {
                    m_fG = this.Parent.GetMoveCost() + CalcMoveCost(this.Parent);
                }

                m_bGCalcFlag = false;
            }

            return m_fG;
        }

        /// <summary>
        /// Gets the heuristic cost.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The value.</returns>
        public double GetHeuristicCost(SearchNode node)
        {
            if (m_bHCalcFlag)
            {
                m_fH = CalcHeuristicCost(node);
                m_bHCalcFlag = false;
            }

            return m_fH;
        }

        /// <summary>
        /// Gets all costs.
        /// </summary>
        /// <remarks>Always return 0.</remarks>
        /// <returns>The value.</returns>
        public int GetAllCosts()
        {
            return 0;
        }

        /// <summary>
        /// Calculates the move cost.
        /// </summary>
        /// <param name="nodeFrom">The node from.</param>
        /// <returns>The value.</returns>
        protected abstract double CalcMoveCost(SearchNode nodeFrom);

        /// <summary>
        /// Calculates the heuristic cost.
        /// </summary>
        /// <param name="nodeTo">The node to.</param>
        /// <returns>The value.</returns>
        protected abstract double CalcHeuristicCost(SearchNode nodeTo);
        #endregion
    }

    /// <summary>
    /// Search grid node.
    /// </summary>
    internal class SearchGridNode : SearchNode
    {
        #region Initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchGridNode"/> class.
        /// </summary>
        /// <param name="ptLocation">The point location.</param>
        public SearchGridNode(System.Windows.Point ptLocation)
            : base(ptLocation)
        { 
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Calculates the heuristic cost.
        /// </summary>
        /// <param name="nodeTo">The node to.</param>
        /// <returns>The value.</returns>
        protected override double CalcHeuristicCost(SearchNode nodeTo)
        {
            return GetGridHeuristicCost(this, nodeTo);
        }

        /// <summary>
        /// Calcs the move cost.
        /// </summary>
        /// <param name="nodeFrom">The node from.</param>
        /// <returns>The value.</returns>
        protected override double CalcMoveCost(SearchNode nodeFrom)
        {
            return GetGridMoveCost(nodeFrom, this);
        }
        #endregion
    }
}
