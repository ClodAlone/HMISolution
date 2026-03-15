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
  /// A collection of four Border objects that represent the four
  /// borders of a Range or Style object.
  /// </summary>
  public interface IBorders
    : IEnumerable
    , IParentApplication
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    /// <summary>
    ///  Returns or sets the weight of the border. Read / write ExcelBorderWeight.
    /// </summary>
    ExcelBorderWeight Weight { get; set; }
    Border _Default { get; }
    XlCreator Creator { get; }
    /// <summary>
    /// Returns or sets the color of all four borders. Returns NULL if all
    /// four borders aren't the same color. The color is specified as an
    /// index value into the current color palette, or as one of the
    /// following ExcelColorIndex constants. Read / write ExcelColorIndex.
    /// </summary>
    ExcelColorIndex ColorIndex { get; set; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write ExcelKnownColors.
    /// </summary>
    ExcelKnownColors  Color { get; set; }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write Color.
    /// </summary>
    Color             ColorRGB { get; set; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only, Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a Border object that represents one of the borders of either a
    /// range of cells or a style.
    /// </summary>
    IBorder this[ ExcelBordersIndex Index ] { get; }
    /// <summary>
    /// Returns or sets the line style for the border. Read / write ExcelLineStyle.
    /// </summary>
    ExcelLineStyle LineStyle { get; set; }
    /// <summary>
    /// Synonym for Borders.LineStyle. Read / write.
    /// </summary>
    ExcelLineStyle Value { get; set; }
    #endregion
  }
}
