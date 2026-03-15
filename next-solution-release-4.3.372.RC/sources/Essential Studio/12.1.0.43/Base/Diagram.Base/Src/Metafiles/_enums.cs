#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Type of the object.
    /// </summary>
    internal enum ObjectType
    {
        /// <summary>
        /// Invalid object type.
        /// </summary>
        Invalid = 0x000,

        /// <summary>
        /// Brush object.
        /// </summary>
        Brush = 0x100,

        /// <summary>
        /// Pen object.
        /// </summary>
        Pen = 0x200,

        /// <summary>
        /// Path object.
        /// </summary>
        Path = 0x300,

        /// <summary>
        /// Region object.
        /// </summary>
        Region = 0x400,

        /// <summary>
        /// Image object.
        /// </summary>
        Image = 0x500,

        /// <summary>
        /// Font object.
        /// </summary>
        Font = 0x600,

        /// <summary>
        /// String format object.
        /// </summary>
        StringFormat = 0x700,

        /// <summary>
        /// Image attributes object.
        /// </summary>
        ImageAttributes = 0x800,

        /// <summary>
        /// Custom line cap object.
        /// </summary>
        CustomLineCap = 0x900,
    }

    /// <summary>
    /// Type of the brush.
    /// </summary>
    internal enum BrushType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        SolidBrush = 0,

        /// <summary>
        /// Hatch brush.
        /// </summary>
        HatchBrush = 1,

        /// <summary>
        /// Texture brush.
        /// </summary>
        TextureBrush = 2,

        /// <summary>
        /// Path gradient brush.
        /// </summary>
        PathGradientBrush = 3,

        /// <summary>
        /// Linear gradient brush.
        /// </summary>
        LienarGradientBrush = 4
    }

    /// <summary>
    /// Flags for a linear gradient brush.
    /// </summary>
    [Flags]
    internal enum GradientBrushFlags
    {
        /// <summary>
        /// Minimal data are present.
        /// </summary>
        Default = 0x0000,

        /// <summary>
        /// The brush applies a transformation matrix to the source image.
        /// </summary>
        Matrix = 0x0002,

        /// <summary>
        /// The brush contains a ColorBlend object for use with its InterpolationColors property.
        /// </summary>
        ColorBlend = 0x0004,

        /// <summary>
        /// The brush contains a Blend object for use with its Blend property.
        /// </summary>
        Blend = 0x0008,

        /// <summary>
        /// The brush has a non-default value for the FocusScales property.
        /// </summary>
        FocusScales = 0x0040,

        /// <summary>
        /// The brush uses gamma correction.
        /// </summary>
        GammaCorrection = 0x0080
    }

    /// <summary>
    /// Represents pen flags.
    /// </summary>
    [Flags]
    internal enum PenFlags
    {
        /// <summary>
        /// Pen just with color set.
        /// </summary>
        Default = 0x0000,

        /// <summary>
        /// Transformation set. (20-... - float )
        /// </summary>
        Transform = 0x0001,

        /// <summary>
        /// StartCap set. ( 20 - int )
        /// </summary>
        StartCap = 0x0002,

        /// <summary>
        /// EndCap set. ( 20 - int )
        /// </summary>
        EndCap = 0x0004,

        /// <summary>
        /// LineJoin set. ( 20 - int )
        /// </summary>
        LineJoin = 0x0008,

        /// <summary>
        /// MiterLimit set. ( 20 - float )
        /// </summary>
        MiterLimit = 0x0010,

        /// <summary>
        /// Pen has DashStyle defined.
        /// </summary>
        DashStyle = 0x0020,

        /// <summary>
        /// DashCap set. ( 20 - int )
        /// </summary>
        DashCap = 0x0040,

        /// <summary>
        /// DashOffset is defined. (20 - float)
        /// </summary>
        DashOffset = 0x0080,

        /// <summary>
        /// DashPattern is defined. (20 - int: numArray; 24-... - float: DashPattern )
        /// </summary>
        DashPattern = 0x0100,

        /// <summary>
        /// Alignment set. (20 - int )
        /// </summary>
        Alignment = 0x0200,

        /// <summary>
        /// CompoundArray set. (20 - int: numArray; 24-... - float: compoundArray )
        /// </summary>
        CompoundArray = 0x0400,

        /// <summary>
        /// The pen uses a custom start cap.
        /// </summary>
        CustomStartCap = 0x800,

        /// <summary>
        /// The pen uses a custom end cap.
        /// </summary>
        CustomEndCap = 0x1000
    }

    /// <summary>
    /// Indicates types of the images in the Object record.
    /// </summary>
    internal enum ObjectImageFormat
    {
        /// <summary>
        /// Unknown format.
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// Bitmap image.
        /// </summary>
        Bitmap = 0x01,

        /// <summary>
        /// Metafile image.
        /// </summary>
        Metafile = 0x02
    }

    /// <summary>
    /// Initial state of the region.
    /// </summary>
    internal enum ObjectRegionInitState
    {
        /// <summary>
        /// Region is from rectangle.
        /// </summary>
        Rectangle = 0x10000000,

        /// <summary>
        /// Region is from graphics path.
        /// </summary>
        GraphpicsPath = 0x10000001,

        /// <summary>
        /// Region is empty.
        /// </summary>
        Empty = 0x10000002,

        /// <summary>
        /// Region is infinity.
        /// </summary>
        Infinity = 0x10000003
    }
}
