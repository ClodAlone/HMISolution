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
  /// Represents an Image in a worksheet.
  /// </summary>
  public interface IPictureShape
    : IShape
    , IParentApplication
  {
    #region IPictureShape properties
    /// <summary>
    /// Gets the Filename, Read only.
    /// </summary>
    string FileName { get; }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Gets the picture, Read only. 
    /// </summary>
    Image Picture { get; }
#endif
    /// <summary>
    /// Removes shape from the collection.
    /// </summary>
    /// <param name="removeImage">Removes image that is referenced by this shape from collection too,
    /// if we didn't detect image usage. XlsIO doesn't detect this situation correctly in all cases
    /// if there are shapes in charts in Excel 2007 or if some image shapes are grouped in any excel version.
    /// If you are not sure whether image is referenced in charts or grouped shapes and you are working with
    /// Excel 2007 version, set this argument to true (this could cause file size increase, but will keep
    /// document in the correct state).</param>
    void Remove( bool removeImage );
    #endregion
  }
}
