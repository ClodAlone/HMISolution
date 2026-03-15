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
    /// Specifies grid style.
    /// </summary>
    public enum GridStyle
    {
        /// <summary>
        /// Point grid style.
        /// </summary>
        Point,

        /// <summary>
        /// Line grid style.
        /// </summary>
        Line
    }

    /// <summary>
    /// Specifies semi cirlce type.
    /// </summary>
    public enum SemiCircleType
    {
        /// <summary>
        /// open type.
        /// </summary>
        Open,

        /// <summary>
        /// closed type.
        /// </summary>
        Closed
    }

    /// <summary>
    /// Specifies arc type.
    /// </summary>
    public enum ArcType
    {
        /// <summary>
        /// open type.
        /// </summary>
        Open,

        /// <summary>
        /// closed type.
        /// </summary>
        Closed
    }

    /// <summary>
    /// Specifies the property the label value is bound to.
    /// </summary>
    public enum LabelPropertyBinding
    {
        /// <summary>
        /// Text property of the label.
        /// </summary>
        Text,

        /// <summary>
        /// Name property in the container.
        /// </summary>
        ContainerName
    }

    /// <summary>
    /// Connection point types.
    /// </summary>
    public enum ConnectionPointType
    {
        /// <summary>
        /// Specifies whether port accepts outgoing and incoming connections.
        /// </summary>
        IncomingOutgoing,

        /// <summary>
        /// Specifies whether port accepts outgoing connections.
        /// </summary>
        Outgoing,

        /// <summary>
        /// Specifies whether port accepts incoming connections.
        /// </summary>
        Incoming,

        /// <summary>
        /// Specifies whether the port rejects the incoming/outgoing connections or not.
        /// </summary>
        Reject
    }

    /// <summary>
    /// Specifies one of nine relative positions on a box.
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// Center of the label.
        /// </summary>
        Center,

        /// <summary>
        /// Top left point of the label.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Top center point of the label.
        /// </summary>
        TopCenter,

        /// <summary>
        /// Top right point of the label.
        /// </summary>
        TopRight,

        /// <summary>
        /// Middle left point of the label.
        /// </summary>
        MiddleLeft,

        /// <summary>
        /// Middle left point of the label.
        /// </summary>
        MiddleRight,

        /// <summary>
        /// Bottom left point of the label.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Bottom center point of the label.
        /// </summary>
        BottomCenter,

        /// <summary>
        /// Bottom right point of the label.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Custom position of the label.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies parent position in the layout
    /// </summary>
    public enum ParentPositions
    {
        /// <summary>
        /// Center of the parent.
        /// </summary>
        Center,

        /// <summary>
        /// Default position of the parent.
        /// </summary>
        Default
    }


    /// <summary>
    /// Specifies the orientation of label.
    /// </summary>
    public enum LabelOrientation
    {
        /// <summary>
        /// Auto alignment.
        /// </summary>
        Auto,

        /// <summary>
        /// Horizontal alignment.
        /// </summary>
        Horizontal
    }
}
