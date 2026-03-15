#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interface to a node in a graph.
    /// </summary>
    /// <remarks>
    /// A node is an object in a graph that can have edges entering
    /// and leaving. Nodes are connected to other nodes by edges.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IGraphEdge"/>
    /// </remarks>
    public interface IGraphNode
    {
        /// <summary>
        /// Gets collection of all edges entering or leaving the node.
        /// </summary>
        ICollection Edges
        {
            get;
        }

        /// <summary>
        /// Gets collection of edges entering the node.
        /// </summary>
        ICollection EdgesEntering
        {
            get;
        }

        /// <summary>
        /// Gets collection of edges leaving the node.
        /// </summary>
        ICollection EdgesLeaving
        {
            get;
        }
    }
}