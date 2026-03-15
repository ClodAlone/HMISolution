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
    /// Interface to a node in a hierarchy or graph of objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A node is a named object in a hierarchical tree structure. Each node
    /// has a <see cref="Syncfusion.Windows.Forms.Diagram.INode.Name"/>
    /// and a parent. A node's name must is unique within the scope of its
    /// parent node. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.INode.FullName"/>
    /// of a node is unique within the scope of the entire node hierarchy.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ICompositeNode"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents"/>
    /// </remarks>
    public interface INode : System.IServiceProvider, System.ICloneable
    {
        /// <summary>
        /// Gets or sets reference to the composite node this node is a child of.
        /// </summary>
        ICompositeNode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets root node in the node hierarchy.
        /// </summary>
        /// <remarks>
        /// The root node is found by following the chain of parent nodes until
        /// a node is found that has a NULL parent.
        /// </remarks>
        INode Root
        {
            get;
        }

        /// <summary>
        /// Gets or sets name of the node.
        /// </summary>
        /// <remarks>
        /// Must be unique within the scope of the parent node.
        /// </remarks>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets fully qualified name of the node.
        /// </summary>
        /// <remarks>
        /// The full name is the name of the node concatenated with the names
        /// of all parent nodes.
        /// </remarks>
        string FullName
        {
            get;
        }
    }
}