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
  /// Represents chart border interface.
  /// </summary>
  public interface IChartBorder
  {
    /// <summary>
    /// Color of line.
    /// </summary>
    Color LineColor { get; set; }
    /// <summary>
    /// Line pattern.
    /// </summary>
    ExcelChartLinePattern LinePattern { get; set; }
    /// <summary>
    /// Weight of line.
    /// </summary>
    ExcelChartLineWeight LineWeight { get; set; }
    /// <summary>
    /// If true - default format; otherwise custom.
    /// </summary>
    bool AutoFormat { get; set; }
    /// <summary>
    /// Custom format for line color.
    /// </summary>
    bool IsAutoLineColor { get; set; }
    /// <summary>
    /// Line color index.
    /// </summary>
    ExcelKnownColors ColorIndex { get; set; }
    /// <summary>
    /// True to draw tick labels on this axis.
    /// </summary>
    bool DrawTickLabels { get; set; }
    /// <summary>
    /// Returns the transparency level of the specified Solid color shaded fill as a floating-point
    /// value from 0.0 (Clear) through 1.0(Opaque)
    /// </summary>
    double Transparency { get; set; }
  }
}
