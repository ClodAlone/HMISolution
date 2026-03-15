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
    /// This interface describes the additional members useful if the underlying graph is a tree.
    /// </summary>
    public interface ITree : IGraph
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is directed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        new bool IsDirected { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root node.</value>
        IShape Root { get; set; }

        /// <summary>
        /// Gets or sets the type of the layout.
        /// </summary>
        /// <value>The type of the layout.</value>
        LayoutType LayoutType { get; set; }

        /// <summary>
        /// Gets the Collection of the Node's Children.
        /// </summary>
        /// <param name="node">Parent Node</param>
        /// <returns>The Children of the node</returns>
        CollectionExt Children(IShape node);

        /// <summary>
        /// Gets a collection of Node's Children's edges.
        /// </summary>
        /// <param name="node">The node object</param>
        /// <returns>The Child edges of the node</returns>
        CollectionExt ChildEdges(IShape node);

        /// <summary>
        /// Gets the  node placed next to this node and at the same level as this node.
        /// </summary>
        /// <param name="node">Node object</param>
        /// <returns>The next sibling.</returns>
        IShape NextSibling(IShape node);

        /// <summary>
        /// Gets the node placed previous to this node and at the same level as this node.
        /// </summary>
        /// <param name="node">Node object</param>
        /// <returns>The previous sibling</returns>
        IShape PreviousSibling(IShape node);

        /// <summary>
        /// Gets the last child of the ParentNode.
        /// </summary>
        /// <param name="node">Parent Node.</param>
        /// <returns>The last child</returns>
        IShape LastChild(IShape node);

        /// <summary>
        /// Gets the immediate child as the First child of the ParentNode.
        /// </summary>
        /// <param name="node">Parent Node.</param>
        /// <returns>The First Child</returns>
        IShape FirstChild(IShape node);

        /// <summary>
        /// Gets a count of the number of children of the parent node.
        /// </summary>
        /// <param name="node">Parent Node</param>
        /// <returns>The child count</returns>
        int ChildCount(IShape node);

        /// <summary>
        /// Gets the number of levels from this Node as the Depth of the current node.
        /// </summary>
        /// <param name="node">Node object.</param>
        /// <returns>The depth value.</returns>
        int Depth(IShape node);

        /// <summary>
        /// Parents the edge.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The parent edge</returns>
        IEdge ParentEdge(IShape node);

        /// <summary>
        /// Takes a specified action on each node starting from the start node.
        /// </summary>
        /// <typeparam name="T">The type of action</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="startNode">The start node.</param>
        void ForEach<T>(Action<T> action, IShape startNode) where T : IShape;       
    }
}
