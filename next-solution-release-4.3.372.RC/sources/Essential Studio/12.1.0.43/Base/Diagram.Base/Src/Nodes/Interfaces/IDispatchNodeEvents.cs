#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Provides nodes with a mechanism for forwarding events up the node hierarchy.
    /// </summary>
    /// <remarks>
    /// Nodes that implement this interface can be notified of events and will
    /// respond by either forwarding the event up the node hierarchy or handling the
    /// event.
    /// </remarks>
    public interface IDispatchNodeEvents
    {
        /// <summary>
        /// Called when a node is clicked.
        /// </summary>
        void Click( EventArgs e );

        /// <summary>
        /// Called when a node is double clicked.
        /// </summary>
        void DoubleClick( EventArgs e );

        /// <summary>
        /// Called when the mouse enters a node.
        /// </summary>
        void MouseEnter( EventArgs e );

        /// <summary>
        /// Called when the mouse leaves a node.
        /// </summary>
        void MouseLeave( EventArgs e );
    }
}
