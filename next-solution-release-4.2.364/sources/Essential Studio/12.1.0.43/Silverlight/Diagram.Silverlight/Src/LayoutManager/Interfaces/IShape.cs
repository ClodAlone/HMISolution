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
    using System.Text;
    using System.Windows;
    using System.Windows.Shapes;

    /// <summary>
    /// Interface to a node in a hierarchy or graph of objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A node is a named object in a hierarchical tree structure. Each node
    /// has a child and a parent. A node's name must is unique within the scope of its
    /// parent node. The Full name of a node is unique within the scope of the entire node hierarchy.
    /// </para>
    /// </remarks>
    public interface IShape : ICommon, INodeGroup
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the object.</value>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        /// <remarks>
        /// The full name is the name of the node concatenated with the names
        /// of all parent nodes.
        /// </remarks>
        string FullName
        {
            get;
        }

        /// <summary>
        /// Gets or sets the logical offset X.
        /// </summary>
        /// <value>The logical offset X.</value>
        double OffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logical offset Y.
        /// </summary>
        /// <value>The logical offset Y.</value>
        double OffsetY
        {
            get;
            set;
        }

        Point Position
        {
            get;
        }
        /// <summary>
        /// Gets the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        int InDegree { get; }

        /// <summary>
        /// Gets the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        int OutDegree { get; }

        /// <summary>
        /// Gets the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the target.
        /// </summary>
        CollectionExt InEdges { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        bool IsFixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets the unique identifier of this node.
        /// </summary>
        /// <value>The unique identifier value.</value>
        new Guid ID { get; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        DiagramModel Model { get; set; }

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        CollectionExt OutEdges { get; }

        /// <summary>
        /// Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        CollectionExt Edges { get; }

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        CollectionExt InNeighbors { get; }

        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        CollectionExt OutNeighbors { get; }

        /// <summary>
        /// Gets the neighbors.
        /// </summary>
        /// <value>The neighbors.</value>
        CollectionExt Neighbors { get; }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        IShape ParentNode { get; set; }

        /// <summary>
        /// Gets or sets the edge between this node and its parent node in a tree
        /// structure.
        /// </summary>
        IEdge ParentEdge { get; set; }

        /// <summary>
        /// Gets or sets the tree depth of this node.
        /// <remarks>The root's tree depth is
        /// zero, and each level of the tree is one depth level greater.
        /// </remarks> 
        /// </summary>
        int Depth { get; set; }

        /// <summary>
        /// Gets the number of tree children of this node.
        /// </summary>
        int ChildCount { get; }

        /// <summary>
        /// Gets this node's first tree child.
        /// </summary>
        IShape FirstChild { get; }

        /// <summary>
        /// Gets this node's last tree child.
        /// </summary>
        IShape LastChild { get; }

        /// <summary>
        /// Gets this node's previous tree sibling.
        /// </summary>
        IShape PreviousSibling { get; }

        /// <summary>
        /// Gets this node's next tree sibling.
        /// </summary>
        IShape NextSibling { get; }

        /// <summary>
        /// Gets or sets an iterator over this node's tree children.
        /// </summary>
        CollectionExt Children { get; set; }

        ///// <summary>
        ///// Gets an iterator over the edges from this node to its tree children.
        ///// </summary>
        //CollectionExt ChildEdges { get; }

        /// <summary>
        /// Gets the Actual Width.
        /// </summary>
        double ActualWidth { get; }

        /// <summary>
        /// Gets the Actual Height.
        /// </summary>
        double ActualHeight { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag connection over.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drag connection over; otherwise, <c>false</c>.
        /// </value>
        bool IsDragConnectionOver { get; set; }

        /// <summary>
        /// Gets or sets the parent nodes  based on the connections in a hierarchical layout.
        /// </summary>
        CollectionExt Parents { get; set; }

        /// <summary>
        /// Gets the previous shape.
        /// </summary>
        /// <value>The previous shape.</value>
        IShape PreviousShape { get; }

        /// <summary>
        /// Gets or sets the row number.
        /// </summary>
        /// <value>The row number.</value>
        int Row { get; set; }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>The column number.</value>
        int Column { get; set; }

        #endregion

        #region Method
        /// <summary>
        /// Moves the node, the argument being the motion vector.
        /// </summary>
        /// <param name="p">The point p.</param>
#if WPF
        void Move(System.Drawing.Point p);
#endif

#if SILVERLIGHT
        void Move(Point p);
#endif

        #endregion
    }
}
