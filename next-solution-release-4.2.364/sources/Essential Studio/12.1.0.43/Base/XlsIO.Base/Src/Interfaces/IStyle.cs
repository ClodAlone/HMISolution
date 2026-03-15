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
  /// Represents a style description for a range. The Style object contains
  /// all style attributes (font, number format, alignment, and so on) as
  /// properties. There are several built-in styles, including Normal,
  /// Currency, and Percent. Using the Style object is a fast and efficient
  /// way to change several cell-formatting properties on multiple cells at
  /// the same time.
  /// For the Workbook object, the Style object is a member of the Styles
  /// collection. The Styles collection contains all the defined styles for
  /// the workbook
  /// </summary>
  public interface IStyle
    : IExtendedFormat
    , IOptimizedUpdate//IParentApplication
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    string _Default { get; }
    XlCreator Creator { get; }
    Interior Interior { get; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// The name of the specified style. Read-only String.
    /// </summary>
    string            Value { get; }
    /// <summary>
    /// Returns or sets the reading order for the specified object.
    /// Can be one of the following constants: xlRTL (right-to-left),
    /// xlLTR (left-to-right), or xlContext. Read/write Long.
    /// </summary>
    int ReadingOrder { get; set; }
    /// <summary>
    /// True if text is automatically indented when the text alignment
    /// in a cell is set to equal distribution either horizontally or
    /// vertically. Read/write Boolean.
    /// </summary>
    bool              AddIndent { get; set; }
    /// <summary>
    /// Removes current style.
    /// </summary>
    /// <returns></returns>
    object Delete();
    /// <summary>
    /// Returns or sets the name of the object, in the language of
    /// the user. Read / write String for Name, Read-only String for Style.
    /// </summary>
    string            NameLocal { get; }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read/write String.
    /// </summary>
    string            NumberFormatLocal { get; set; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// True if the style is a built-in style. Read-only Boolean.
    /// </summary>
    bool              BuiltIn { get; }
    /// <summary>
    /// Returns or sets the name of the object. Read-only String.
    /// </summary>
    string            Name { get; }
    /// <summary>
    /// Indicates whether style is initialized (differs from Normal style).
    /// Read-only.
    /// </summary>
    bool              IsInitialized { get; }
    /// <summary>
    /// Returns interior object for this extended format.
    /// </summary>
    IInterior Interior { get; }
    #endregion
  }
}
