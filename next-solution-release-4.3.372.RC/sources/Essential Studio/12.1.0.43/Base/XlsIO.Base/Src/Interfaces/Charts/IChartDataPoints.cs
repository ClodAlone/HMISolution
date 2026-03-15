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
using System.Collections;
#endregion

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents a collection of data point in the series.
	/// </summary>
  public interface IChartDataPoints :
    IParentApplication,
    IEnumerable
  {
    /// <summary>
    /// Returns default data point. Read-only.
    /// </summary>
    IChartDataPoint DefaultDataPoint { get; }
    /// <summary>
    /// Returns single data point by its index. Read-only.
    /// </summary>
    IChartDataPoint this[ int index ] { get; }
  }
}
