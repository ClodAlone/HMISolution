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
  /// Represents a Merged Cell.
  /// </summary>
  public interface IMergeCells
  {
    /// <summary>
    /// Merges all cells in cellRange.
    /// </summary>
    /// <param name="RowFrom"></param>
    /// <param name="RowTo"></param>
    /// <param name="ColFrom"></param>
    /// <param name="ColTo"></param>
    /// <param name="operation"></param>
    void AddMerge( int RowFrom, int RowTo, int ColFrom, int ColTo, ExcelMergeOperation operation );
    /// <summary>
    /// If specified cell is contained in merged cell, delete that merge.
    /// </summary>
    /// <param name="CellIndex"></param>
    void DeleteMerge( int CellIndex );
  }
}
