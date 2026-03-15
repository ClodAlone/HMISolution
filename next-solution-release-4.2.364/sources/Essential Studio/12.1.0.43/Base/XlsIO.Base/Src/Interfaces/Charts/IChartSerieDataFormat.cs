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
	/// Summary description for IChartSerieDataFormat.
	/// </summary>
  public interface IChartSerieDataFormat : IChartFillBorder
  {
    #region interface properties
    /// <summary>
    /// Returns object, that represents area properties. Read-only.
    /// </summary>
    IChartInterior AreaProperties { get; }
    /// <summary>
    /// Represents the base data format.
    /// </summary>
    ExcelBaseFormat BarShapeBase { get; set; }
    /// <summary>
    /// Represents the top data format.
    /// </summary>
    ExcelTopFormat BarShapeTop { get; set; }
    /// <summary>
    /// Foreground color: RGB value (high byte = 0).
    /// </summary>
    Color MarkerBackgroundColor { get; set; }
    /// <summary>
    /// Background color: RGB value (high byte = 0).
    /// </summary>
    Color MarkerForegroundColor { get; set; }
    /// <summary>
    /// Type of marker.
    /// </summary>
    ExcelChartMarkerType MarkerStyle { get; set; }
    /// <summary>
    /// Index to color of marker border.
    /// </summary>
    ExcelKnownColors MarkerForegroundColorIndex { get; set; }
    /// <summary>
    /// Index to color of marker fill.
    /// </summary>
    ExcelKnownColors MarkerBackgroundColorIndex { get; set; }
    /// <summary>
    /// Size of line markers.
    /// </summary>
    int MarkerSize { get; set; }
    /// <summary>
    /// Automatic color.
    /// </summary>
    bool IsAutoMarker { get; set; }
    /// <summary>
    /// Distance of pie slice from center of pie.
    /// </summary>
    int Percent { get; set; }
    /// <summary>
    /// True to draw bubbles with 3D effects.
    /// </summary>
    bool Is3DBubbles { get; set; }
    /// <summary>
    /// Gets common series options. Read-only.
    /// </summary>
    IChartFormat CommonSerieOptions { get; }
    /// <summary>
    /// Indicates whether marker is supported by this chart/series.
    /// </summary>
    bool IsMarkerSupported { get; }
    #endregion
  }
}
