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
  /// Represents interface for chart LegendEntries collection.
  /// </summary>
  public interface IChartLegendEntries
  {
    /// <summary>
    /// Represents count of legend entries in collection. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Gets legend entry object by index. Read-only.
    /// </summary>
    IChartLegendEntry this[ int iIndex ] { get; }
  }
}
