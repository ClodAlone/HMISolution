#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents ChartTrendLine interface.
  /// </summary>
  public interface IChartTrendLine
  {
    #region IChartTrendLine properties
    /// <summary>
    /// Represents border object. Read-only.
    /// </summary>
    IChartBorder Border { get; }
    /// <summary>
    /// Represents number of periods that the trend line extends backward.
    /// </summary>
    double Backward { get; set; }
    /// <summary>
    ///Represents number of periods that the trend line extends forward.
    /// </summary>
    double Forward { get; set; }
    /// <summary>
    /// True if the equation for the trend line is displayed on the chart.
    /// </summary>
    bool DisplayEquation { get; set; }
    /// <summary>
    /// True if the R-squared value of the trend line is displayed on the chart.
    /// </summary>
    bool DisplayRSquared { get; set; }
    /// <summary>
    /// Represents point where the trend line crosses the value axis.
    /// </summary>
    double Intercept { get; set; }
    /// <summary>
    /// True if the point where the trend line crosses the value
    ///  axis is automatically determined by the regression.
    /// </summary>
    bool InterceptIsAuto { get; set; }
    /// <summary>
    /// Represents trend line type.
    /// </summary>
    ExcelTrendLineType Type { get; set; }
    /// <summary>
    /// Represents for Moving Average and Polynomial trend line type order value.
    /// </summary>
    int Order { get; set; }
    /// <summary>
    /// Indicates if name is default.
    /// </summary>
    bool NameIsAuto { get; set; }
    /// <summary>
    /// Represents trend line name.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Returns data label. Read-only.
    /// </summary>
    IChartTextArea DataLabel { get; }
    /// <summary>
    /// Gets the shadow.
    /// </summary>
    /// <value>The shadow.</value>
    IShadow Shadow { get; }
    /// <summary>
    /// Gets the chart3 D options.
    /// </summary>
    /// <value>The chart3 D options.</value>
    IThreeDFormat Chart3DOptions { get; }
    #endregion

    #region IChartTrendLine methods
    /// <summary>
    /// Clears current trend line.
    /// </summary>
    void ClearFormats();
    #endregion
  }
}
