#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Specifies the type of Horizontal alignment.
    /// </summary>
    public enum PdfHorizontalAlignment
    {
        /// <summary>
        ///	Specifies the element is aligned to Left.
        /// </summary>
        Left,
        /// <summary>
        ///	Specifies the element is aligned to Center.
        /// </summary>
        Center,
        /// <summary>
        ///	Specifies the element is aligned to Right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the type of Vertical alignment.
    /// </summary>
    public enum PdfVerticalAlignment
    {
        /// <summary>
        /// Specifies the element is aligned to Top.
        /// </summary>
        Top,
        /// <summary>
        /// Specifies the element is aligned to Middle.
        /// </summary>
        Middle,
        /// <summary>
        /// Specifies the element is aligned to Bottom.
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// Specifies the type of horizontal text alignment.
    /// </summary>
    public enum PdfTextAlignment
    {
        /// <summary>
        /// Specifies the text is aligned to Left.
        /// </summary>
        Left,
        /// <summary>
        /// Specifies the text is aligned to Center.
        /// </summary>
        Center,
        /// <summary>
        /// Specifies the text is aligned to Right.
        /// </summary>
        Right,
        /// <summary>
        /// Specifies the text as Justified text.
        /// </summary>
        Justify
    }

    /// <summary>
    /// Specifies the text rendering mode.
    /// </summary>
    [Flags]
    internal enum TextRenderingMode
    {
        /// <summary>
        /// Fill text.
        /// </summary>
        Fill = 0,
        /// <summary>
        /// Stroke text.
        /// </summary>
        Stroke = 1,
        /// <summary>
        /// Fill, then stroke text.
        /// </summary>
        FillStroke = 2,
        /// <summary>
        /// Neither fill nor stroke text (invisible).
        /// </summary>
        None = 3,
        /// <summary>
        /// The flag showing that the text should be a part of a clipping path.
        /// </summary>
        ClipFlag = 4,
        /// <summary>
        /// Fill text and add to path for clipping (see above).
        /// </summary>
        ClipFill = Fill | ClipFlag,
        /// <summary>
        /// Stroke text and add to path for clipping.
        /// </summary>
        ClipStroke = Stroke | ClipFlag,
        /// <summary>
        /// Fill, then stroke text and add to path for clipping.
        /// </summary>
        ClipFillStroke = FillStroke | ClipFlag,
        /// <summary>
        /// Add text to path for clipping.
        /// </summary>
        Clip = None | ClipFlag,
    }

    /// <summary>
    /// Specifies the corner style of the shapes.
    /// </summary>
    public enum PdfLineJoin
    {
        /// <summary>
        /// The outer edges for the two segments are extended
        /// until they meet at an angle.
        /// </summary>
        Miter = 0,
        /// <summary>
        /// An arc of a circle with a diameter equal to the line width is drawn
        /// around the point where the two segments meet, connecting the outer edges for the two segments.
        /// </summary>
        Round = 1,
        /// <summary>
        /// The two segments are finished with caps
        /// and the resulting notch beyond the ends of the segments is filled
        /// with a triangle.
        /// </summary>
        Bevel = 2
    }

    /// <summary>
    ///Specifies the line cap style to be used at the ends of the lines.
    /// </summary>
    public enum PdfLineCap
    {
        /// <summary>
        /// The stroke is squared off at the endpoint of the path. There is no
        /// projection beyond the end of the path.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// A semicircular arc with a diameter equal to the line width is
        /// drawn around the endpoint and filled in.
        /// </summary>
        Round = 1,
        /// <summary>
        ///	 The stroke continues beyond the endpoint of the path
        /// for a distance equal to half the line width and is squared off.
        /// </summary>
        Square = 2
    }

    /// <summary>
    /// Possible dash styles of the pen.
    /// </summary>
    public enum PdfDashStyle
    {
        /// <summary>
        /// Solid line.
        /// </summary>
        Solid = 0,
        /// <summary>
        /// Dashed line.
        /// </summary>
        Dash = 1,
        /// <summary>
        /// Dotted line.
        /// </summary>
        Dot = 2,
        /// <summary>
        /// Dash-dot line.
        /// </summary>
        DashDot = 3,
        /// <summary>
        /// Dash-dot-dot line.
        /// </summary>
        DashDotDot = 4,
        /// <summary>
        /// User defined dash style.
        /// </summary>
        Custom = 5,
    }

    /// <property name="flag" value="Finished" />
    ///
    /// <summary>
    /// Specifies how the shapes are filled. 
    /// </summary>
    public enum PdfFillMode
    {
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Nonzero winding number rule of determining &quot;insideness&quot;
        /// of point.
        /// </summary>
        Winding,
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Even odd rule of determining &quot;insideness&quot; of point.
        /// </summary>
        Alternate

    }

    /// <property name="flag" value="Finished" />
    ///
    /// <summary>
    /// Defines set of color spaces.
    /// </summary>
    public enum PdfColorSpace
    {
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// RGB color space.
        /// </summary>
        RGB,
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// CMYK color space.
        /// </summary>
        CMYK,
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// GrayScale color space.
        /// </summary>
        GrayScale,
        /// <summary>
        /// Indexed color space used internally.
        /// </summary>
        Indexed
    }

    /// <summary>
    /// Describes the Color intents.
    /// </summary>
    internal enum ColorIntent
    {
        /// <summary>
        /// Colors are represented solely with respect to the light source;
        /// no correction is made for the output medium�s white point
        /// (such as the color of unprinted paper).
        /// </summary>
        AbsoluteColorimetric,
        /// <summary>
        /// Colors are represented with respect to the combination of
        /// the light source and the output medium�s white point
        /// (such as the color of unprinted paper).
        /// </summary>
        RelativeColorimetric,
        /// <summary>
        /// Colors are represented in a manner that preserves
        /// or emphasizes saturation.
        /// </summary>
        Saturation,
        /// <summary>
        /// Colors are represented in a manner that provides a pleasing
        /// perceptual appearance.
        /// </summary>
        Perceptual,
    }

    /// <summary>
    /// Specifies the blend mode for transparency.
    /// </summary>
    public enum PdfBlendMode
    {
        /// <summary>
        /// Selects the source color, ignoring the backdrop.
        /// </summary>
        Normal,
        /// <summary>
        /// Multiplies the backdrop and source color values.
        /// The result color is always at least as dark as either
        /// of the two constituent colors. Multiplying
        /// any color with black produces black; multiplying
        /// with white leaves the original color unchanged.
        /// Painting successive overlapping objects with a color
        /// other than black or white produces progressively darker colors.
        /// </summary>
        Multiply,
        /// <summary>
        /// Multiplies the complements of the backdrop and source
        /// color values, then complements the result. The result
        /// color is always at least as light as either of the two
        /// constituent colors. Screening any color with white
        /// produces white; screening with black leaves the original
        /// color unchanged. The effect is similar to projecting
        /// multiple photographic slides simultaneously onto a single screen.
        /// </summary>
        Screen,
        /// <summary>
        /// Multiplies or screens the colors, depending on
        /// the backdrop color value. Source colors overlay
        /// the backdrop while preserving its highlights and
        /// shadows. The backdrop color is not replaced but
        /// is mixed with the source color to reflect the
        /// lightness or darkness of the backdrop.
        /// </summary>
        Overlay,
        /// <summary>
        /// Selects the darker of the backdrop and source colors.
        /// The backdrop is replaced with the source where the source
        /// is darker; otherwise, it is left unchanged.
        /// </summary>
        Darken,
        /// <summary>
        /// Selects the lighter of the backdrop and source colors.
        /// The backdrop is replaced with the source where the source
        /// is lighter; otherwise, it is left unchanged.
        /// </summary>
        Lighten,
        /// <summary>
        /// Brightens the backdrop color to reflect the source color.
        /// Painting with black produces no changes.
        /// </summary>
        ColorDodge,
        /// <summary>
        /// Darkens the backdrop color to reflect the source color.
        /// Painting with white produces no change.
        /// </summary>
        ColorBurn,
        /// <summary>
        /// Multiplies or screens the colors, depending on the source color value.
        /// The effect is similar to shining a harsh spotlight on the backdrop.
        /// </summary>
        HardLight,
        /// <summary>
        /// Darkens or lightens the colors, depending on the source color value.
        /// The effect is similar to shining a diffused spotlight on the backdrop.
        /// </summary>
        SoftLight,
        /// <summary>
        /// Subtracts the darker of the two constituent colors from the lighter color.
        /// Painting with white inverts the backdrop color; painting with black produces no change.
        /// </summary>
        Difference,
        /// <summary>
        /// Produces an effect similar to that of the Difference mode
        /// but lower in contrast. Painting with white inverts 
        /// the backdrop color; painting with black produces no change.
        /// </summary>
        Exclusion,
        /// <summary>
        /// Creates a color with the hue of the source color and 
        /// the saturation and luminosity of the backdrop color.
        /// </summary>
        Hue,
        /// <summary>
        /// Creates a color with the saturation of the source color
        /// and the hue and luminosity of the backdrop color. Painting
        /// with this mode in an area of the backdrop that is a pure
        /// gray (no saturation) produces no change.
        /// </summary>
        Saturation,
        /// <summary>
        /// Creates a color with the hue and saturation of
        /// the source color and the luminosity of the backdrop
        /// color. This preserves the gray levels of the backdrop
        /// and is useful for coloring monochrome images or tinting color images.
        /// </summary>
        Color,
        /// <summary>
        /// Creates a color with the luminosity of the source color
        /// and the hue and saturation of the backdrop color. This
        /// produces an inverse effect to that of the Color mode.
        /// </summary>
        Luminosity,
    }

    /// <summary>
    /// Specifies the type of the PdfImage.
    /// </summary>
    public enum PdfImageType
    {
        /// <summary>
        /// Specifies the image is bitmap.
        /// </summary>
        Bitmap,
        /// <summary>
        /// Specifies the image is metafile.
        /// </summary>
        Metafile
    }

    /// <summary>
    /// Specifies the types of the page's logical units.
    /// </summary>
    public enum PdfGraphicsUnit
    {
        /// <summary>
        /// Specifies the Measurement is in centimeters.
        /// </summary>
        Centimeter = 0,
        /// <summary>
        ///	Specifies the Measurement is in picas. A pica represents 12 points.
        /// </summary>
        Pica = 1,
        /// <summary>
        /// Specifies the unit of measurement is 1 pixel.
        /// </summary>
        /// <remarks>Pixel unit is device dependent unit. The result depends on the default Dpi on the machine.</remarks>
        Pixel = 2,
        /// <summary>
        /// Specifies a printer's point (1/72 inch) as the unit of measure. 
        /// </summary>
        Point = 3,
        /// <summary>
        ///	Specifies the inch as the unit of measure. 
        /// </summary>
        Inch = 4,
        /// <summary>
        /// Specifies the document unit (1/300 inch) as the unit of measure.
        /// </summary>
        Document = 5,
        /// <summary>
        /// Specifies the Measurement is in millimeters.
        /// </summary>
        Millimeter = 6,
    }

    ///<summary>
    ///Specifies the alignment type.
    ///</summary>
    public enum PdfGridImagePosition
    {
        Fit,
        Center,
        Stretch,
        Tile,

    }
}
