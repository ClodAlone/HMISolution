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
  /// Represents an Embedded Excel chart. [Chart embedded in a worksheet].
  /// </summary>
  public interface IChartShape
    : IShape
    , IChart
    , IParentApplication
  {
    /// <summary>
    /// Top row of the chart in the worksheet.
    /// </summary>
    int TopRow { get; set; }
    /// <summary>
    /// Bottom row of the chart in the worksheet.
    /// </summary>
    int BottomRow { get; set; }
    /// <summary>
    /// Left column of the chart in the worksheet.
    /// </summary>
    int LeftColumn { get; set; }
    /// <summary>
    /// Right column of the chart in the worksheet.
    /// </summary>
    int RightColumn { get; set; }
  }
}
