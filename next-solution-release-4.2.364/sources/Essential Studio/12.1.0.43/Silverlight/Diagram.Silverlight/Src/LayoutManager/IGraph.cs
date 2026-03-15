#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Provides an interface to the tree graph .
    /// </summary>
    public interface IGraph
    {
        #region Properties            

        /// <summary>
        /// Gets a value indicating whether this instance is directed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        bool IsDirected { get; }

        /// <summary>
        /// Gets a collection of nodes.
        /// </summary>
        CollectionExt Nodes { get; }

        /// <summary>
        /// Gets the Spanning tree.
        /// </summary>
        ITree Tree { get; }

        /// <summary>
        /// Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        CollectionExt Edges { get; }

        /// <summary>
        /// Gets the edge of the Node from which the connection started.
        /// </summary>
        /// <param name="edge">The edge of the node</param>
        /// <returns>Edge of the head node.</returns>
        IShape FromNode(IEdge edge);

        /// <summary>
        /// Gets the edge of the Node to which the connection ended. 
        /// </summary>
        /// <param name="edge">The edge of the node</param>
        /// <returns>Edge of the tail node.</returns>
        IShape ToNode(IEdge edge);

        /// <summary>
        /// Given a Node upon which this Edge is incident, the opposite incident
        /// Node is returned. Throws an exception if the input node is not incident
        /// on this Edge.
        /// </summary>
        /// <param name="edge">The edge object.</param>
        /// <param name="node">The node object.</param>
        /// <returns>The adjacent node</returns>
        IShape AdjacentNode(IEdge edge, IShape node);

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The in coming neighbor nodes</returns>
        CollectionExt InNeighbors(IShape node);

        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The out going neighbor nodes</returns>
        CollectionExt OutNeighbors(IShape node);

        /// <summary>
        /// Get an iterator over all nodes connected to this node.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The neighbor nodes</returns>
        CollectionExt Neighbors(IShape node);

        /// <summary>
        /// Returns a collection of the edges of the node.
        /// </summary>
        /// <param name="node">The node object</param>
        /// <returns>The total number of edges.</returns>
        CollectionExt EdgesOf(IShape node);

        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The outgoing edges</returns>
        CollectionExt InEdges(IShape node);

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The outgoing edges</returns>
        CollectionExt OutEdges(IShape node);

        /// <summary>
        /// Gets the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The in degree .</returns>
        int InDegree(IShape node);

        /// <summary>
        /// Gets the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The out degree.</returns>
        int OutDegree(IShape node);

        /// <summary>
        /// Get the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The degree value.</returns>
        int Degree(IShape node);
       
        #endregion

        #region Methods
        /// <summary>
        /// Traverse all the internal nodes of the specified parent node and also their edges.
        /// </summary>
        /// <param name="node">Parent Node</param>
        void MakeTraversing(IShape node);

        /// <summary>
        /// Clear the traversed tree.
        /// </summary>
        void ClearTraversing();
        #endregion
    }
}
