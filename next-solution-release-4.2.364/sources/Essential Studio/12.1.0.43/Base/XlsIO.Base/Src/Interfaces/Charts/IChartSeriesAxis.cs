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
  /// Represents the chart series Axis.
  /// </summary>
  public interface IChartSeriesAxis : IChartAxis
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
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    [ Obsolete( "Please, use TickMarkSpacing instead of it" ) ]
    int TickMarksFrequency { get; set; }
    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    int TickMarkSpacing { get; set; }
  }
}
