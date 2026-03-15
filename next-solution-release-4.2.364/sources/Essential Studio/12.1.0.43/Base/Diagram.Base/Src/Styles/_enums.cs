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
    /// Modes for interactively editing objects with selection handles.
    /// </summary>
    public enum HandleEditMode
    {
        /// <summary>
        /// No edit mode allowed.
        /// </summary>
        None,

        /// <summary>
        /// Resize the object.
        /// </summary>
        Resize,

        /// <summary>
        /// Edit the object's vertices.
        /// </summary>
        Vertex
    }

    /// <summary>
    /// Types of brushes that can be created by a FillStyle.
    /// </summary>
    public enum FillStyleType
    {
        /// <summary>
        /// Solid brush.
        /// </summary>
        Solid,

        /// <summary>
        /// Text brush.
        /// </summary>
        Texture,

        /// <summary>
        /// Linear gradient brush.
        /// </summary>
        LinearGradient,

        /// <summary>
        /// Hatch brush.
        /// </summary>
        Hatch,

        /// <summary>
        /// Path gradient brush. Reserved for internal use.
        /// </summary>
        PathGradient
    }

    /// <summary>
    /// Path gradient brush style enumeration.
    /// </summary>
    [Documentation.DocumentationExclude()]
    public enum PathGradientBrushStyle
    {
        /// <summary>
        /// Path gradient brush style will be applied in center.
        /// </summary>
        RectangleCenter = 1,

        /// <summary>
        /// Path gradient brush style will be applied in left top.
        /// </summary>
        RectangleLeftTop,

        /// <summary>
        /// Path gradient brush style will be applied in left bottom.
        /// </summary>
        RectangleLeftBottom,

        /// <summary>
        /// Path gradient brush style will be applied in right top.
        /// </summary>
        RectangleRightTop,

        /// <summary>
        /// Path gradient brush style will be applied in right bottom.
        /// </summary>
        RectangleRightBottom,

        /// <summary>
        /// Path gradient brush style will be applied in circle center.
        /// </summary>
        CircleCenter,

        /// <summary>
        /// Path gradient brush style will be applied in circle left top.
        /// </summary>
        CircleLeftTop,

        /// <summary>
        /// Path gradient brush style will be applied in circle left bottom.
        /// </summary>
        CircleLeftBottom,

        /// <summary>
        /// Path gradient brush style will be applied in circle right top.
        /// </summary>
        CircleRightTop,

        /// <summary>
        /// Path gradient brush style will be applied in circle right bottom.
        /// </summary>
        CircleRightBottom
    }

    /// <summary>
    /// Guides enumeration.
    /// </summary>
    [System.Flags]
    public enum GuideTypes
    {
        /// <summary>
        /// Shows the guides for pinpoint of a node.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Shows the guides for boundary of a node. 
        /// </summary>
        Boundary = 2,

        /// <summary>
        /// Shows the Margin line to align shapes. 
        /// </summary>
        Margin = 4,

        /// <summary>
        /// Shows all the guides
        /// </summary>
        All = Center | Boundary | Margin
    }
}