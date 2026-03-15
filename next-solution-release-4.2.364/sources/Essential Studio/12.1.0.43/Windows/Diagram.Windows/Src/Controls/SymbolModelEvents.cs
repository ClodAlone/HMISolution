#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Event argument class for events associated with symbol models loaded
    /// in a PaletteGroupBar object.
    /// </summary>
    public class NodeEventArgs
        : EventArgs
    {
        #region Class members
        private Node m_node;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeEventArgs(Node node)
        {
            m_node = node;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the node that generated the event.
        /// </summary>
        public Node Node
        {
            get { return m_node; }
        }
        #endregion
    }

    /// <summary>
    /// Delegate for events associated with symbol models loaded
    /// in a PaletteGroupBar object.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="evtArgs">The node event args</param>
    public delegate void NodeEventHandler(object sender, NodeEventArgs evtArgs);
}
