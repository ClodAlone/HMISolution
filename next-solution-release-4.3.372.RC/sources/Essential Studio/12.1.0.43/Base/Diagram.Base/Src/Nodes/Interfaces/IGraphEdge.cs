#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interface to an edge in a graph.
    /// </summary>
    /// <remarks>
    /// An edge links together two nodes in a graph. It provides a path
    /// between two nodes.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IGraphNode"/>
    /// </remarks>
    public interface IGraphEdge
    {
        /// <summary>
        /// Gets Node connected to the tail of the edge.
        /// </summary>
        IGraphNode FromNode
        {
            get;
        }

        /// <summary>
        /// Gets Node connected to the head of the edge.
        /// </summary>
        IGraphNode ToNode
        {
            get;
        }

        /// <summary>
        /// Gets Weight value associated with the edge.
        /// </summary>
        int EdgeWeight
        {
            get;
        }

        /// <summary>
        /// Determines if this edge is leaving the given node.
        /// </summary>
        /// <param name="graphNode">Node to test.</param>
        /// <returns>True if edge is leaving the given node.</returns>
        bool IsNodeLeaving(IGraphNode graphNode);

        /// <summary>
        /// Determines if this edge is entering the given node.
        /// </summary>
        /// <param name="graphNode">Node to test.</param>
        /// <returns>True if edge is entering the given node.</returns>
        bool IsNodeEntering(IGraphNode graphNode);
    }
}