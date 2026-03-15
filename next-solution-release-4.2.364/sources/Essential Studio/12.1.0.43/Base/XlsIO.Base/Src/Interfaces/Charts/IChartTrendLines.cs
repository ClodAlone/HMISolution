#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Interface that represents trend line collection.
  /// </summary>
  public interface IChartTrendLines
  {
    /// <summary>
    /// Gets single trend line by index. Read-only.
    /// </summary>
    IChartTrendLine this[ int iIndex ] { get; }
    /// <summary>
    /// Adds new instance of trend line to collection.
    /// </summary>
    /// <returns>Returns added trend line object.</returns>
    IChartTrendLine Add();
    /// <summary>
    /// Adds new instance of trend line to collection.
    /// </summary>
    /// <param name="type">Represents type of trend line.</param>
    /// <returns>Returns added trend line object.</returns>
    IChartTrendLine Add( ExcelTrendLineType type );
    /// <summary>
    /// Removes trend line object from collection.
    /// </summary>
    /// <param name="index">Represents </param>
    void RemoveAt( int index );
    /// <summary>
    /// Clears current collection.
    /// </summary>
    void Clear();
    /// <summary>
    /// Represents count of trend lines.
    /// </summary>
    int Count { get; }
  }
}
