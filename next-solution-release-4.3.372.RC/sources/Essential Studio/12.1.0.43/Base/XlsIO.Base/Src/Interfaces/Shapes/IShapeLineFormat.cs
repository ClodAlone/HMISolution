#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents interface for shape line format.
  /// </summary>
  public interface IShapeLineFormat
  {
    /// <summary>
    /// Represents weight of the line.
    /// </summary>
    double Weight { get; set; }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    Color  ForeColor { get; set; }
    /// <summary>
    /// Represents background color.
    /// </summary>
    Color  BackColor { get; set; }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    ExcelKnownColors ForeColorIndex { get; set; }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    ExcelKnownColors BackColorIndex { get; set; }
    /// <summary>
    /// Represents begin arrow head style.
    /// </summary>
    ExcelShapeArrowStyle BeginArrowHeadStyle { get; set; }
    /// <summary>
    /// Represents end arrow head style.
    /// </summary>
    ExcelShapeArrowStyle EndArrowHeadStyle { get; set; }
    /// <summary>
    /// Represents begin arrow head length.
    /// </summary>
    ExcelShapeArrowLength BeginArrowheadLength { get; set; }
    /// <summary>
    /// Represents end arrow head length.
    /// </summary>
    ExcelShapeArrowLength EndArrowheadLength { get; set; }
    /// <summary>
    /// Represents begin arrow head width.
    /// </summary>
    ExcelShapeArrowWidth BeginArrowheadWidth { get; set; }
    /// <summary>
    /// Represents end arrow head width.
    /// </summary>
    ExcelShapeArrowWidth EndArrowheadWidth { get; set; }
    /// <summary>
    /// Represents the dash style for the specified line.
    /// </summary>
    ExcelShapeDashLineStyle DashStyle { get; set; }
    /// <summary>
    /// Represents line style.
    /// </summary>
    ExcelShapeLineStyle Style { get; set; }
    /// <summary>
    /// Represents line transparency.
    /// </summary>
    double Transparency { get; set; }
    /// <summary>
    /// Represents if line format is visible.
    /// </summary>
    bool Visible { get; set; }
    /// <summary>
    /// Represents line pattern.
    /// </summary>
    ExcelGradientPattern Pattern { get; set; }
    /// <summary>
    /// Indicates if current line format contain pattern.
    /// </summary>
    bool HasPattern { get; set; }
  }
}
