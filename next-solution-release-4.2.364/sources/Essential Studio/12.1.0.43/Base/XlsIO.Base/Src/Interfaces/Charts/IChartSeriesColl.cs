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
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the collection of chart series.
  /// </summary>
  public interface IChartSeries :
    IParentApplication,
    ICollection<IChartSerie>
  {
    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int           Count { get; }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    IChartSerie   this[ int index ] { get; }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    IChartSerie   this[ string name ] { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    IChartSerie Add();
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="serieType">Type of new series.</param>
    /// <returns>Newly created series object.</returns>
    IChartSerie Add( ExcelChartType serieType );
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="name">Name of the new series.</param>
    /// <returns>Newly created series object.</returns>
    IChartSerie Add( string name );
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="name">Name of the new series.</param>
    /// <param name="type">Type of new series.</param>
    /// <returns>Newly created series object.</returns>
    IChartSerie Add( string name, ExcelChartType type );
    /// <summary>
    /// Removes Series object from the collection.
    /// </summary>
    /// <param name="index">Index of the series to remove.</param>
    void        RemoveAt( int index );
    /// <summary>
    /// Removes series by name.
    /// </summary>
    /// <param name="serieName">Series name to remove.</param>
    void Remove( string serieName );
    #endregion
  }
}
