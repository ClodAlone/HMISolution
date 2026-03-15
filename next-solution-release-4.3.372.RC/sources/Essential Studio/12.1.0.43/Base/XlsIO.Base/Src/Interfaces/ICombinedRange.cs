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
using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
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
  /// Represents a combined Range.
  /// </summary>
  public interface ICombinedRange : IRange
  {
    #region Interface methods
    /// <summary>
    /// Gets new address of range.
    /// </summary>
    /// <param name="names">Dictionary with Worksheet names.</param>
    /// <param name="strSheetName">String that sets as a worksheet name.</param>
    /// <returns>Returns string with new name.</returns>
    string GetNewAddress( Dictionary<string, string> names, out string strSheetName );
    /// <summary>
    /// Clones current IRange.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="hashNewNames">Dictionary with new names.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Returns clone of current instance.</returns>
    IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book );
    /// <summary>
    /// Clears conditional formats.
    /// </summary>
    void ClearConditionalFormats();
    /// <summary>
    /// Returns array that contains information about range.
    /// </summary>
    /// <returns>Rectangles that describes range</returns>
    Rectangle[] GetRectangles();
    /// <summary>
    /// Returns number of rectangles returned by GetRectangles method.
    /// </summary>
    /// <returns>Number of rectangles returned by GetRectangles method.</returns>
    int GetRectanglesCount();
    #endregion

    #region Interface properties
    /// <summary>
    /// Number of cells in the range. Read-only.
    /// </summary>
    int CellsCount { get; }
    /// <summary>
    /// Gets address global in the format required by Excel 2007.
    /// </summary>
    string AddressGlobal2007 { get; }
    /// <summary>
    /// Gets name of the parent worksheet.
    /// </summary>
    string WorksheetName { get; }
    #endregion
  }  
}
