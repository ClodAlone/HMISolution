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

using System;
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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents interface for chart interior.
  /// </summary>
  public interface IChartInterior
  {
    ///// <summary>
    ///// Foreground color (RGB).
    ///// </summary>
    //ColorObject    ForegroundColorObject { get; }
    ///// <summary>
    ///// Background color (RGB).
    ///// </summary>
    //ColorObject    BackgroundColorObject { get; }
    /// <summary>
    /// Foreground color (RGB).
    /// </summary>
    Color    ForegroundColor { get; set; }
    /// <summary>
    /// Background color (RGB).
    /// </summary>
    Color    BackgroundColor { get; set; }
    /// <summary>
    /// Area pattern.
    /// </summary>
    ExcelPattern Pattern { get; set; }
    /// <summary>
    /// Index of foreground color.
    /// </summary>
    ExcelKnownColors ForegroundColorIndex { get; set; }
    /// <summary>
    /// Background color index.
    /// </summary>
    ExcelKnownColors BackgroundColorIndex { get; set; }
    /// <summary>
    /// If true - use automatic format; otherwise custom.
    /// </summary>
    bool   UseAutomaticFormat { get; set; }
    /// <summary>
    /// Foreground and background are swapped when the data value is negative.
    /// </summary>
    bool   SwapColorsOnNegative { get; set; }
  }
}
