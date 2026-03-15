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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a Excel chart Axis
  /// </summary>
  public interface IChartAxis
  {
    /// <summary>
    /// Gets or sets number format string.
    /// </summary>
    string NumberFormat { get; set; }
    /// <summary>
    /// Returns type of the axis. Read-only.
    /// </summary>
    ExcelAxisType AxisType { get; }
    /// <summary>
    /// Axis title.
    /// </summary>
    string Title { get; set; }
    /// <summary>
    /// Text rotation angle. Should be integer value between -90 and 90.
    /// </summary>
    int TextRotationAngle { get; set; }
    /// <summary>
    /// Returns text area for the axis title. Read-only.
    /// </summary>
    IChartTextArea TitleArea { get; }
    /// <summary>
    /// Returns font used for axis text displaying. Read-only.
    /// </summary>
    IFont Font { get; }
    /// <summary>
    /// Represents major gridLines. Read-only.
    /// </summary>
    IChartGridLine MajorGridLines { get; }
    /// <summary>
    /// Represents minor gridLines. Read-only.
    /// </summary>
    IChartGridLine MinorGridLines { get; }
    /// <summary>
    /// Gets or sets if axis has minor gridlines.
    /// </summary>
    bool HasMinorGridLines { get; set; }
    /// <summary>
    /// Gets or sets if axis has major gridlines.
    /// </summary>
    bool HasMajorGridLines { get; set; }
    /// <summary>
    /// Represents minor tick marks.
    /// </summary>
    ExcelTickMark MinorTickMark{ get; set; }
    /// <summary>
    /// Represents major tick marks.
    /// </summary>
    ExcelTickMark MajorTickMark{ get; set; }
    /// <summary>
    /// Represents chart border. Read-only.
    /// </summary>
    IChartBorder Border { get; }
    /// <summary>
    /// Represents whether the tick label position is automatic or not
    /// </summary>
    bool AutoTickLabelSpacing { get; set; }
    /// <summary>
    /// Represents tick label position.
    /// </summary>
    ExcelTickLabelPosition TickLabelPosition { get; set; }
    /// <summary>
    /// Indicates is axis is visible.
    /// </summary>
    bool Visible { get; set; }
    /// <summary>
    /// Represents alignment for the tick label.
    /// </summary>
    ExcelAxisTextDirection Alignment { get; set; }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    [ Obsolete( "Please use ReversePlotOrder property instead of this one." ) ]
    bool IsReversed { get; set; }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    bool ReversePlotOrder { get; set; }
    /// <summary>
    /// Returns the Shadow properties.Read-only
    /// </summary>
    IShadow Shadow { get; }

    /// <summary>
    /// Gets the three_ D.
    /// </summary>
    /// <value>The three_ D.</value>
    IThreeDFormat Chart3DOptions { get; }
  }
}
