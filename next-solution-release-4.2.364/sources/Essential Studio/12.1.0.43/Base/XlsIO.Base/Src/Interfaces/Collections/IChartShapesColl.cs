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
  /// Represents a collection of embedded charts.
  /// </summary>
  public interface IChartShapes
    : IEnumerable
    , IParentApplication
  {
    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single Chart object from a Charts collection.
    /// </summary>
    IChartShape this[ int index ]{ get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Creates a new chart.
    /// </summary>
    /// <returns>Newly created chart object.</returns>
    IChartShape Add();
    /// <summary>
    /// Removes Chart object from the collection.
    /// </summary>
    /// <param name="index">Index of the chart to remove.</param>
    void RemoveAt( int index );
    #endregion
  }
}
