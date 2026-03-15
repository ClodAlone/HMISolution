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
using System.Collections;
using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Contains the font attributes (font name, font size,
  /// color and so on) for an object.
  /// </summary>
  public interface IFont
    : IParentApplication
    , IOptimizedUpdate
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns or sets the color of the font. The color is specified as
    /// an index value into the current color palette, or as one of the
    /// following ExcelColorIndex constants. Read / write Integer.
    /// </summary>
    ExcelColorIndex ColorIndex { get; set; }
    /// <summary>
    /// Returns or sets the name of the object. The name of a Range object
    /// is a Name object. For every other type of object, the name is a
    /// string. Read / write Variant.
    /// </summary>
    object Name { get; set; }
    /// <summary>
    /// Returns or sets the text background type. This property is used for
    /// text on charts. Read / write Variant.
    /// </summary>
    object Background { get; set; }
    /// <summary>
    /// Returns or sets the font style. Read / write String.
    /// </summary>
    object FontStyle { get; set; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// True if the font is bold. Read / write Boolean.
    /// </summary>
    bool              Bold { get; set; }
    /// <summary>
    /// Returns or sets the primary color of the object. Read / write ExcelKnownColors.
    /// </summary>
    ExcelKnownColors  Color { get; set; }
    /// <summary>
    /// Gets / sets font color. Searches for the closest color in 
    /// the workbook palette.
    /// </summary>
    Color             RGBColor{ get; set; }
    /// <summary>
    /// True if the font style is italic. Read / write Boolean.
    /// </summary>
    bool              Italic { get; set; }
    /// <summary>
    /// True if the font is an outline font. Read / write Boolean.
    /// </summary>
    bool              MacOSOutlineFont { get; set; }
    /// <summary>
    /// True if the font is a shadow font or if the object has
    /// a shadow. Read / write Boolean.
    /// </summary>
    bool              MacOSShadow{ get; set; }
    /// <summary>
    /// Returns or sets the size of the font. Read / write Variant.
    /// </summary>
    double            Size { get; set; }
    /// <summary>
    /// True if the font is struck through with a horizontal line.
    /// Read / write Boolean
    /// </summary>
    bool              Strikethrough { get; set; }
    /// <summary>
    /// True if the font is formatted as subscript.
    /// False by default. Read / write Boolean.
    /// </summary>
    bool              Subscript { get; set; }
    /// <summary>
    /// True if the font is formatted as superscript. False by default.
    /// Read/write Boolean
    /// </summary>
    bool              Superscript { get; set; }
    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can
    /// be one of the following ExcelUnderlineStyle constants.
    /// Read / write ExcelUnderline.
    /// </summary>
    ExcelUnderline    Underline { get; set; }
    /// <summary>
    /// Returns or sets the font name. Read / write string.
    /// </summary>
    string            FontName{ get; set; }
    /// <summary>
    /// Gets / sets font vertical alignment.
    /// </summary>
    ExcelFontVertialAlignment VerticalAlignment { get; set; }
    /// <summary>
    /// Indicates whether color is automatically selected. Read-only.
    /// </summary>
    bool IsAutoColor { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    Font GenerateNativeFont();
    #endregion
  }
}