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
	/// Represents single data point in the chart.
	/// </summary>
	public interface IChartDataPoint : IParentApplication
	{
    /// <summary>
    /// Returns data labels object for the data point. Read-only.
    /// </summary>
    IChartDataLabels DataLabels { get; }
    /// <summary>
    /// Gets index of the point in the points collection.
    /// </summary>
    int Index { get; }
    /// <summary>
    /// Gets / sets data format.
    /// </summary>
    IChartSerieDataFormat DataFormat{ get; }
    /// <summary>
    /// Indicates whether this data point is default data point. Read-only.
    /// </summary>
    bool IsDefault { get; }
    /// <summary>
    /// Indicates whether it's default
    /// </summary>
    bool IsDefaultmarkertype { get; set; }
  }
}
