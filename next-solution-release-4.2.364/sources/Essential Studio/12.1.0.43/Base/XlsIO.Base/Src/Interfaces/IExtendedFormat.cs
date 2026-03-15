#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;


#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Contains the font attributes (font name, size,
  /// color, and so on) for an object.
  /// </summary>
  public interface IExtendedFormat : IParentApplication
  {
    #region Interface properties
    /// <summary>
    /// Returns borders object for this extended format.
    /// </summary>
    IBorders Borders { get; }
    /// <summary>
    /// Gets / Sets index of fill background color. Obsolete, will be removed in next release.
    /// </summary>
    [ Obsolete( "Use ColorIndex instead of this property." ) ]
    ExcelKnownColors FillBackground{ get; set; }
    /// <summary>
    /// Gets / Sets fill background color. Obsolete, will be removed in next release.
    /// </summary>
    [ Obsolete( "Use Color instead of this property." ) ]
    Color   FillBackgroundRGB{ get; set; }
    /// <summary>
    /// Gets / Sets index of fill foreground color. Obsolete, will be removed in next release.
    /// </summary>
    [ Obsolete( "Use PatternColorIndex instead of this property." ) ]
    ExcelKnownColors FillForeground{ get; set; }
    /// <summary>
    /// Gets / Sets fill foreground color. Obsolete, will be removed in next release.
    /// </summary>
    [ Obsolete( "Use PatternColor instead of this property." ) ]
    Color   FillForegroundRGB{ get; set; }
    /// <summary>
    /// Gets / Sets fill pattern.
    /// </summary>
    ExcelPattern FillPattern{ get; set; }
    /// <summary>
    /// Returns font object for this extended format.
    /// </summary>
    IFont   Font  { get; }
    /// <summary>
    /// True if formula is hidden.
    /// </summary>
    bool    FormulaHidden{ get; set; }
    /// <summary>
    /// Horizontal alignment.
    /// </summary>
    ExcelHAlign HorizontalAlignment{ get; set; }
    /// <summary>
    /// True if the style includes the AddIndent, HorizontalAlignment,
    /// VerticalAlignment, WrapText, and Orientation properties.
    /// Read / write Boolean.
    /// </summary>
    bool    IncludeAlignment { get; set; }
    /// <summary>
    /// True if the style includes the Color, ColorIndex, LineStyle,
    /// and Weight border properties. Read / write Boolean.
    /// </summary>
    bool    IncludeBorder { get; set; }
    /// <summary>
    /// True if the style includes the Background, Bold, Color,
    /// ColorIndex, FontStyle, Italic, Name, OutlineFont, Shadow,
    /// Size, Strikethrough, Subscript, Superscript, and Underline
    /// font properties. Read / write Boolean.
    /// </summary>
    bool    IncludeFont { get; set; }
    /// <summary>
    /// True if the style includes the NumberFormat property.
    /// Read / write Boolean.
    /// </summary>
    bool    IncludeNumberFormat { get; set; }
    /// <summary>
    /// True if the style includes the Color, ColorIndex,
    /// InvertIfNegative, Pattern, PatternColor, and PatternColorIndex
    /// interior properties. Read / write Boolean.
    /// </summary>
    bool    IncludePatterns { get; set; }
    /// <summary>
    /// True if the style includes the FormulaHidden and Locked protection
    /// properties. Read / write Boolean.
    /// </summary>
    bool    IncludeProtection { get; set; }
    /// <summary>
    /// Indent level.
    /// </summary>
    int     IndentLevel{ get; set; }
    /// <summary>
    /// If true then first symbol in cell is apostrophe.
    /// </summary>
    bool    IsFirstSymbolApostrophe { get; set; }
    /// <summary>
    /// True if cell is locked.
    /// </summary>
    bool    Locked{ get; set; }
    /// <summary>
    /// For far east languages. Supported only for format. Always 0 for US.
    /// </summary>
    bool    JustifyLast{ get; set; }
    /// <summary>
    /// Returns or sets the format code for the object. Read / write String.
    /// </summary>
    string  NumberFormat { get; set; }
    /// <summary>
    /// Gets / Sets format index.
    /// </summary>
    int     NumberFormatIndex{ get; set; }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read / write String.
    /// </summary>
    string  NumberFormatLocal { get; set; }
    /// <summary>
    /// Returns object that describes number format. Read-only.
    /// </summary>
    INumberFormat NumberFormatSettings { get; }
    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>
    ExcelReadingOrderType ReadingOrder { get; set; }
    /// <summary>
    /// Text rotation angle:
    /// 0 Not rotated
    /// 1-90 1 to 90 degrees counterclockwise
    /// 91-180 1 to 90 degrees clockwise
    /// 255 Letters are stacked top-to-bottom, but not rotated.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when value is more than 0xFF.</exception>
    int     Rotation  {get; set;}
    /// <summary>
    /// True - shrink content to fit into cell.
    /// </summary>
    bool    ShrinkToFit{ get; set; }
    /// <summary>
    /// Vertical alignment.
    /// </summary>
    ExcelVAlign VerticalAlignment{ get; set; }
    /// <summary>
    /// True - Text is wrapped at right border.
    /// </summary>
    bool    WrapText{ get; set; }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    ExcelKnownColors PatternColorIndex{ get; set; }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    Color   PatternColor{ get; set; }
    /// <summary>
    /// Returns or sets the color of the interior. The color is specified as
    /// an index value into the current color palette.
    /// </summary>
    ExcelKnownColors ColorIndex{ get; set; }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    Color   Color{ get; set; }
    ///// <summary>
    ///// Returns gradient object for this extended format.
    ///// </summary>
    //IGradient Gradient { get; }
    /// <summary>
    /// Gets value indicating whether format was modified, compared to parent format.
    /// </summary>
    bool IsModified { get; }    
    /// <summary>
    /// Returns the Cell has border
    /// </summary>
    bool HasBorder { get; }
    #endregion
  }
}
