#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Node Layout styles
    /// </summary>
    public enum LayoutStyle
    {
        /// <summary>
        /// Arrange in a table like layout with rows and columns.
        /// </summary>
        Table,

        /// <summary>
        /// Layout nodes in directed tree graph fashion.
        /// </summary>
        DirectedTree,

        /// <summary>
        /// Layout nodes in hierarchical way.
        /// </summary>
        Hierarchic,

        /// <summary>
        /// Layout nodes in radial (circular) fashion.
        /// </summary>
        Radial,

        /// <summary>
        /// Layout nodes in symmetrical manner.
        /// </summary>
        Symmetric
    }

    /// <summary>
    /// Four direction of tree where value 
    /// indicate angle, and angle start from TopToBottom by clockwise.
    /// </summary>
    public enum TreeDirection
    {
        /// <summary>
        /// Top to bottom.
        /// </summary>
        TopToBottom = 0,

        /// <summary>
        /// Top left to bottom right.
        /// </summary>
        TopLeftToBottomRight = 45,

        /// <summary>
        /// Right to left.
        /// </summary>
        RightToLeft = 90,

        /// <summary>
        /// Bottom left to top right.
        /// </summary>
        BottomLeftToTopRight = 135,

        /// <summary>
        /// Bottom to top.
        /// </summary>
        BottomToTop = 180,

        /// <summary>
        /// Bottom right to top left.
        /// </summary>
        BottomRightToTopLeft = 225,

        /// <summary>
        /// Left to right.
        /// </summary>
        LeftToRight = 270,

        /// <summary>
        /// Top right to bottom left.
        /// </summary>
        TopRightToBottomLeft = 315
    }
}
