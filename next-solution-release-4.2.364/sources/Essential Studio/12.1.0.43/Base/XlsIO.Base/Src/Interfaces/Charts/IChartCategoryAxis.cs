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
  /// Represents the chart Category Axis.
  /// </summary>
  public interface IChartCategoryAxis : IChartValueAxis
  {
    /// <summary>
    /// Represents the number of categories or series between tick-mark labels.
    /// </summary>
    [ Obsolete( "Please, use TickLabelSpacing instead of it" ) ]
    int LabelFrequency { get; set; }
    /// <summary>
    /// Represents the number of categories or series between tick-mark labels.
    /// </summary>
    int TickLabelSpacing { get; set; }
    /// <summary>
    /// Represents whether the tick label spacing is automatic or not
    /// </summary>
    bool AutoTickLabelSpacing { get; set; }
    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    [ Obsolete( "Please, use TickMarkSpacing instead of it" ) ]
    int TickMarksFrequency { get; set; }
    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    int TickMarkSpacing { get; set; }
    /// <summary>
    /// If true - cuts unused plot area. Default for area, surface charts.
    /// </summary>
    bool IsBetween { get; set; }
    /// <summary>
    /// Category labels for the chart.
    /// </summary>
    IRange CategoryLabels { get; set; }
    /// <summary>
    /// Entered directly category labels for the chart.
    /// </summary>
    object[] EnteredDirectlyCategoryLabels { get; set; }
    /// <summary>
    /// Represents axis category type.
    /// </summary>
    ExcelCategoryType CategoryType { get; set; }
    /// <summary>
    /// Represents distance between the labels and axis line.
    /// The value can be from 0 through 1000.
    /// </summary>
    int Offset { get; set; }
    /// <summary>
    /// Represents base unit for the specified category axis.
    /// </summary>
    ExcelChartBaseUnit BaseUnit { get; set; }
    /// <summary>
    /// True if use automatic base units for the specified category axis.
    /// </summary>
    bool BaseUnitIsAuto { get; set; }
    /// <summary>
    /// Represents the major unit scale value for the category axis
    ///  when the CategoryType property is set to TimeScale.
    /// </summary>
    ExcelChartBaseUnit MajorUnitScale { get; set; }
    /// <summary>
    /// Represents the minor unit scale value for the category axis
    ///  when the CategoryType property is set to TimeScale.
    /// </summary>
    ExcelChartBaseUnit MinorUnitScale { get; set; }
  }
}
