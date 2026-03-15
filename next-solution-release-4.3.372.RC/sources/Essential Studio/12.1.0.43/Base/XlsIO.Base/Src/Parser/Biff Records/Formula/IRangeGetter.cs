#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This interface is implemented by formula tokens that can provide corresponding rectangle object.
  /// </summary>
  public interface IRectGetter
  {
    /// <summary>
    /// Returns rectangle represented by the token that implements this interface.
    /// All coordinates are zero-based.
    /// </summary>
    /// <returns>Rectangle represented by the token.</returns>
    Rectangle GetRectangle();
  }
  /// <summary>
  /// This interface should be implemented by those tokens
  /// that can be converted to IRange.
  /// </summary>
  public interface IRangeGetter
  {
    /// <summary>
    /// Returns range represented by the token that implements this interface.
    /// </summary>
    /// <param name="book">Workbook that contains range.</param>
    /// <param name="sheet">Worksheet that contains range.</param>
    /// <returns>Range represented by the token.</returns>
    IRange GetRange( IWorkbook book, IWorksheet sheet );
  }
  /// <summary>
  /// This interface should be implemented by those tokens
  /// that can be converted to IRange.
  /// </summary>
  public interface IRangeGetterToken :
    IRangeGetter,
    IRectGetter
  {
    /// <summary>
    /// Updates token using data from specified rectangle.
    /// </summary>
    /// <param name="rectangle">Rectangle with new token coordinates.</param>
    /// <returns>Updated token.</returns>
    Ptg UpdateRectangle( Rectangle rectangle );
    /// <summary>
    /// Converts current token into error token.
    /// </summary>
    /// <returns>Created token.</returns>
    Ptg ConvertToError();
  }
}
