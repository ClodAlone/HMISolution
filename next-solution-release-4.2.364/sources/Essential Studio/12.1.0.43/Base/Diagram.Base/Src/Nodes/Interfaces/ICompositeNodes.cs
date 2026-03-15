#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A composite node is a node that contains children.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface has methods for adding and removing child
    /// nodes.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.INode"/>
    /// </remarks>
    public interface ICompositeNode
    {
        /// <summary>
        /// Gets or sets a value indicating whether composite node can be ungrouped.
        /// </summary>
        bool CanUngroup { get; set; }

        /// <summary>
        /// Gets the number of child nodes contained by this node.
        /// </summary>
        int ChildCount { get; }

        /// <summary>
        /// Update bounds size to content size.
        /// </summary>
        /// <remarks>
        /// Used to update composite node bounds 
        /// if children position or size changed.
        /// </remarks>
        void UpdateCompositeBounds();

        /// <summary>
        /// Returns the child node at the given index position.
        /// </summary>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        /// <returns>Child node at the given position or NULL if the index is out of range.</returns>
        Node GetChild(int childIndex);

        /// <summary>
        /// Returns the child node matching the given name.
        /// </summary>
        /// <param name="childName">Name of node to return.</param>
        /// <returns>Node matching the given name.</returns>
        Node GetChildByName(string childName);

        /// <summary>
        /// Returns the index position of the given child node.
        /// </summary>
        /// <param name="child">Child node to query.</param>
        /// <returns>Zero-based index into the collection of child nodes.</returns>
        int GetChildIndex(Node child);

        /// <summary>
        /// Appends the given node to the collection of child nodes.
        /// </summary>
        /// <param name="child">Node to append.</param>
        /// <returns>
        /// Zero-based index at which the node was added to the collection or -1 for failure.
        /// </returns>
        int AppendChild(Node child);

        /// <summary>
        /// Appends the given collection of nodes as child nodes.
        /// </summary>
        /// <param name="children">Nodes to append.</param>
        /// <param name="startIdx">
        /// Zero-based index at which the first node was added to the collection of child nodes.
        /// </param>
        /// <returns>Number of child nodes appended.</returns>
        int AppendChildren(NodeCollection children, out int startIdx);

        /// <summary>
        /// Insert the given node into the collection of child nodes at a
        /// specific position.
        /// </summary>
        /// <param name="child">Node to insert.</param>
        /// <param name="childIndex">Zero-based index at which to insert the node.</param>
        void InsertChild(Node child, int childIndex);

        /// <summary>
        /// Removes the child node at the given position.
        /// </summary>
        /// <returns>True if the node was successfully removed; otherwise False.</returns>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        bool RemoveChild(int childIndex);

        /// <summary>
        /// Removes the child node at the given position.
        /// </summary>
        /// <returns>True if the node was successfully removed; otherwise False.</returns>
        /// <param name="child">Node to remove.</param>
        bool RemoveChild(Node child);

        /// <summary>
        /// Removes all child nodes from the node.
        /// </summary>
        void RemoveAllChildren();

        /// <summary>
        /// Tests to see if the given node falls within the constraining region
        /// of the composite node.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>
        /// True if node falls within the constraining region; False if it does
        /// not.
        /// </returns>
        bool CheckConstrainingRegion(Node node);

        /// <summary>
        /// Returns all children that are intersected by the given point.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children hit by the given point.
        /// </param>
        /// <param name="ptModel">Point to test.</param>
        /// <returns>The number of child nodes that intersect the given point.</returns>
        int GetChildrenAtPoint(NodeCollection childNodes, PointF ptModel);

        /// <summary>
        /// Returns all children that intersect the given rectangle.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children hit by the given point.
        /// </param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>The number of child nodes that intersect the given rectangle.</returns>
        int GetChildrenIntersecting(NodeCollection childNodes, RectangleF rcModel);

        /// <summary>
        /// Returns all children inside the given rectangle.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children inside the specified rectangle.
        /// </param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>The number of child nodes added to the collection.</returns>
        int GetChildrenContainedBy(NodeCollection childNodes, RectangleF rcModel);
    }
}