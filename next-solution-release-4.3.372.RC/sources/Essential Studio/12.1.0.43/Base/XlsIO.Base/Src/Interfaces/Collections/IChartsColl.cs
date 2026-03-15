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

using Syncfusion.XlsIO;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a collection of charts.
  /// </summary>
  public interface ICharts
    : IEnumerable
    , IParentApplication
  {
    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single Chart object from a Charts collection
    /// </summary>
    IChart this[ int index ]{ get; }
    /// <summary>
    /// Returns a single Chart object from a Charts collection.
    /// </summary>
    IChart this[ string name ]{ get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Creates a new chart.
    /// </summary>
    /// <returns>Newly created chart object.</returns>
    IChart Add();
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name of the new chart's sheet.</param>
    /// <returns>Newly created chart object.</returns>
    IChart Add( string name );
    /// <summary>
    /// Removes Chart object from the collection.
    /// </summary>
    /// <param name="name">Name of the object to remove from the collection.</param>
    IChart Remove( string name );
    /// <summary>
    /// Adds copy of the specified chart to the collection.
    /// </summary>
    /// <param name="chartToCopy">Chart to copy.</param>
    void AddCopy( IChart chartToCopy );
    #endregion
  }
}
