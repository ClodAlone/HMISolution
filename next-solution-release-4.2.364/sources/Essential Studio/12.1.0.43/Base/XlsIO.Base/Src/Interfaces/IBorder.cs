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
using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Windows.UI;
#endif


#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
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
  /// Represents the border of an object.
  /// </summary>
  public interface IBorder : IParentApplication
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    /// <summary>
    /// Returns or sets the weight of the border. Read / write ExcelBorderWeight.
    /// </summary>
    ExcelBorderWeight  Weight { get; set; }
    XlCreator Creator { get; }
    /// <summary>
    /// Returns or sets the color of the border. The color is specified as an
    /// index value into the current color palette, or as one of the
    /// following ExcelColorIndex constants. Read/write ushort.
    /// </summary>
    ExcelColorIndex    ColorIndex { get; set; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write ExcelKnownColors.
    /// </summary>
    ExcelKnownColors    Color { get; set; }
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write ExcelKnownColors.
    /// </summary>
    ColorObject    ColorObject { get; }
    /// <summary>
    /// Returns color of the border.
    /// </summary>
    Color               ColorRGB { get; set; }
    /// <summary>
    /// Returns or sets the line style for the border. Read/write ExcelLineStyle.
    /// </summary>
    ExcelLineStyle      LineStyle { get; set; }
    /// <summary>
    /// This property is used only by Diagonal borders. For any other border
    /// index property will have no influence.
    /// </summary>
    bool                ShowDiagonalLine{ get; set; }
    #endregion
  }
}
