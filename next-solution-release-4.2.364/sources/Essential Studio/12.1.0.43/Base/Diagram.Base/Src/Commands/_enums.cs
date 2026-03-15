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
    /// Specifies relative Z-order movement.
    /// </summary>
    public enum ZOrderUpdate
    {
        /// <summary>
        /// Move to front of Z-order.
        /// </summary>
        Front,

        /// <summary>
        /// Move to back of Z-order.
        /// </summary>
        Back,

        /// <summary>
        /// Move forward in Z-order by 1.
        /// </summary>
        Forward,

        /// <summary>
        /// Move backward in Z-order by 1.
        /// </summary>
        Backward,

        /// <summary>
        /// Sets specified Z-order.
        /// </summary>
        Set
    }

    /// <summary>
    /// Specifies vertex change type.
    /// </summary>
    public enum VertexChangeType
    {
        /// <summary>
        /// Insert vertex.
        /// </summary>
        Insert,

        /// <summary>
        /// Remove vertex.
        /// </summary>
        Remove,

        /// <summary>
        /// Set vertex.
        /// </summary>
        Set
    }
}
