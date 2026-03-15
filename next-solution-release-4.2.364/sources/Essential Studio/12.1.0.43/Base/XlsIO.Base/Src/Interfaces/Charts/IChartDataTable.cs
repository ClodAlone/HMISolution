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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents IChartDataTable interface.
  /// </summary>
  public interface IChartDataTable
  {
    /// <summary>
    /// True if data table has horizontal border.
    /// </summary>
    bool HasHorzBorder { get; set; }
    /// <summary>
    /// True if data table has vertical border.
    /// </summary>
    bool HasVertBorder { get; set; }
    /// <summary>
    /// True if data table has borders.
    /// </summary>
    bool HasBorders { get; set; }
    /// <summary>
    /// True if there is series keys in the data table.
    /// </summary>
    bool ShowSeriesKeys { get; set; }
    /// <summary>
    /// Return text area of data table.
    /// </summary>
    IChartTextArea TextArea { get; }
  }
}
