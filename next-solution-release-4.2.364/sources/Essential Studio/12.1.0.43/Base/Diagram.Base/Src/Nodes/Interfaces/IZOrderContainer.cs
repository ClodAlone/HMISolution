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
    /// Z Order Container.
    /// </summary>
    public interface IZOrderContainer
    {
        /// <summary>
        /// Gets number of items in the Z-order for this container.
        /// </summary>
        int ZOrderDepth
        {
            get;
        }

        /// <summary>
        /// Returns the Z-order value of the given node.
        /// </summary>
        /// <param name="node">Node to get Z-order for.</param>
        /// <returns>
        /// Zero-based Z-order value of the node or -1 if the node
        /// does not exist.
        /// </returns>
        int GetZOrder(Node node);

        /// <summary>
        /// Sets the Z-order of the given node.
        /// </summary>
        /// <param name="node">Node to set Z-order for.</param>
        /// <param name="zOrder">Zero-based Z-order value.</param>
        /// <returns>The z-order.</returns>
        int SetZOrder(Node node, int zOrder);

        /// <summary>
        /// Moves the specified node forward in the Z-order.
        /// </summary>
        /// <param name="node">Node to move forward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        int BringForward(Node node);

        /// <summary>
        /// Sends the specified node back in the Z-order.
        /// </summary>
        /// <param name="node">Node to move backward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        int SendBackward(Node node);

        /// <summary>
        /// Brings the specified node to the front of the Z-order.
        /// </summary>
        /// <param name="node">Node to bring to the front.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        int BringToFront(Node node);

        /// <summary>
        /// Sends the specified node to the back of the Z-order.
        /// </summary>
        /// <param name="node">Node to send to the back.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        int SendToBack(Node node);
    }
}
