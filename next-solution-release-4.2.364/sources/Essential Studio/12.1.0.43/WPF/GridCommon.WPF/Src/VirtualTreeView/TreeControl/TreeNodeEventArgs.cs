#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// Provides event data for TreeNode events.
    /// </summary>
    public class TreeNodeEventArgs : EventArgs
    {
        TreeNode node;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public TreeNodeEventArgs(TreeNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public TreeNode Node
        {
            get { return node; }
            set { node = value; }
        }
    }

    /// <summary>
    /// An event handler for TreeNode events.
    /// </summary>
    public delegate void TreeNodeEventHandler(object sender, TreeNodeEventArgs e);

}
