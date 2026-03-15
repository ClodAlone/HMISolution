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

#region File using directives
using System;
using System.Collections.Generic;
using System.Text;
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

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Interface used to get interior settings.
  /// </summary>
  public interface IInterior
  {
    #region Interface properties
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    ExcelKnownColors PatternColorIndex{ get; set; }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    Color PatternColor{ get; set; }
    /// <summary>
    /// Returns or sets the color of the interior. The color is specified as
    /// an index value into the current color palette.
    /// </summary>
    ExcelKnownColors ColorIndex{ get; set; }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    Color Color{ get; set; }
    /// <summary>
    /// Returns gradient object for this extended format.
    /// </summary>
    IGradient Gradient { get; }
    /// <summary>
    /// Gets / Sets fill pattern.
    /// </summary>
    ExcelPattern FillPattern{ get; set; }
    #endregion
  }
}
