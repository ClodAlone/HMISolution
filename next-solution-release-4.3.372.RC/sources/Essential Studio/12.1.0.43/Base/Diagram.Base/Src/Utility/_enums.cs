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
    /// Enum containing Rotation primitives
    /// </summary>
    public enum HandlePrimitive
    {
        /// <summary>
        /// Pinpoint primitives.
        /// </summary>
        PinPoint,

        /// <summary>
        /// Rotation handle primitive.
        /// </summary>
        RotationHandle,

        /// <summary>
        /// Control point primitive.
        /// </summary>
        ControlPoint
    }

    /// <summary>
    /// Real-world units of measurement:
    /// </summary>
    public enum MeasureUnits
    {
        /// <summary>
        /// Measurement in GDI 
        /// </summary>
        Pixel = 0,

        /// <summary>
        /// Measurement in Points 
        /// </summary>
        Point,

        /// <summary>
        /// Measurement in Document
        /// </summary>
        Document,

        /// <summary>
        /// Measurement in Display
        /// </summary>
        Display,

        // ENGLISH

        /// <summary>
        /// Measurement in Sixteenth Inches
        /// </summary>
        SixteenthInch,

        /// <summary>
        /// Measurement in Eighth Inches
        /// </summary>
        EighthInch,

        /// <summary>
        /// Measurement in Quarter Inches
        /// </summary>
        QuarterInch,

        /// <summary>
        /// Measurement in Half Inches
        /// </summary>
        HalfInch,

        /// <summary>
        /// Measurement in Inches
        /// </summary>
        Inch,

        /// <summary>
        /// Measurement in Feet
        /// </summary>
        Foot,

        /// <summary>
        /// Measurement in Yards
        /// </summary>
        Yard,

        /// <summary>
        /// Measurement in Miles
        /// </summary>
        Mile,

        // METRIC

        /// <summary>
        /// Measurement in Millimeters
        /// </summary>
        Millimeter,

        /// <summary>
        /// Measurement in Centimeters
        /// </summary>
        Centimeter,

        /// <summary>
        /// Measurement in Meters
        /// </summary>
        Meter,

        /// <summary>
        /// Measurement in Kilometers
        /// </summary>
        Kilometer,
    }

    /// <summary>
    /// Specifies one of nine relative positions on a box.
    /// </summary>
    public enum BoxPosition
    {
        /// <summary>
        /// Top center point of the box.
        /// </summary>
        TopCenter = 0,

        /// <summary>
        /// Top right point of the box.
        /// </summary>
        TopRight = 1,

        /// <summary>
        /// Middle left point of the box.
        /// </summary>
        MiddleRight = 2,

        /// <summary>
        /// Bottom right point of the box.
        /// </summary>
        BottomRight = 3,

        /// <summary>
        /// Bottom center point of the box.
        /// </summary>
        BottomCenter = 4,

        /// <summary>
        /// Bottom left point of the box.
        /// </summary>
        BottomLeft = 5,

        /// <summary>
        /// Middle left point of the box.
        /// </summary>
        MiddleLeft = 6,

        /// <summary>
        /// Top left point of the box.
        /// </summary>
        TopLeft = 7,

        /// <summary>
        /// Center of the box.
        /// </summary>
        Center = 8
    }

    /// <summary>
    /// Specifies a compass heading.
    /// </summary>
    /// <remarks>
    /// This enumeration is used to specify the direction of a vector. A vector
    /// can be generated given one point and a CompassHeading.
    /// </remarks>
    public enum CompassHeading
    {
        /// <summary>
        /// No heading.
        /// </summary>
        None = 0,

        /// <summary>
        /// North (up) heading.
        /// </summary>
        North = 1,

        /// <summary>
        /// South (down) heading.
        /// </summary>
        South = 2,

        /// <summary>
        /// East (right).
        /// </summary>
        East = 3,

        /// <summary>
        /// West (left).
        /// </summary>
        West = 4,

        /// <summary>
        /// Northwest (up and left).
        /// </summary>
        Northwest = 5,

        /// <summary>
        /// Northeast (up and right).
        /// </summary>
        Northeast = 6,

        /// <summary>
        /// Southwest (down and left).
        /// </summary>
        Southwest = 7,

        /// <summary>
        /// Southeast (down and right).
        /// </summary>
        Southeast = 8
    }

    /// <summary>
    /// The box side specifies a top bottom, 
    /// right and left sides of rectangle.
    /// </summary>
    public enum BoxSide
    {
        /// <summary>
        /// Right side of rectangle
        /// </summary>
        Right,

        /// <summary>
        /// Top side of rectangle
        /// </summary>
        Top,

        /// <summary>
        /// Left side of rectangle
        /// </summary>
        Left,

        /// <summary>
        /// Bottom side of rectangle
        /// </summary>
        Bottom
    }

    /// <summary>
    /// The BoxOrientation specifies a vertical or horizontal line passing
    /// through one of the four edges or through the center of a box.
    /// </summary>
    public enum BoxOrientation
    {
        /// <summary>
        /// Left edge of the box.
        /// </summary>
        Left,

        /// <summary>
        /// Top edge of the box.
        /// </summary>
        Top,

        /// <summary>
        /// Right edge of the box.
        /// </summary>
        Right,

        /// <summary>
        /// Bottom edge of the box.
        /// </summary>
        Bottom,

        /// <summary>
        /// Horizontal center.
        /// </summary>
        HCenter,

        /// <summary>
        /// Vertical center.
        /// </summary>
        VCenter
    }

    /// <summary>
    /// Specifies the position of scroll to node
    /// </summary>
    public enum Positions
    {
        /// <summary>
        /// Relative position.
        /// </summary>
        Relative,

        /// <summary>
        /// Absolute position
        /// </summary>
        Absolute
    }

    /// <summary>
    /// Specifies the text case of Text/RichText node
    /// </summary>
    public enum TextCases
    {
        /// <summary>
        /// upper case.
        /// </summary>
        AllUpper,

        /// <summary>
        /// lower case.
        /// </summary>
        AllLower,

        /// <summary>
        /// original text
        /// </summary>
        None
    }

}