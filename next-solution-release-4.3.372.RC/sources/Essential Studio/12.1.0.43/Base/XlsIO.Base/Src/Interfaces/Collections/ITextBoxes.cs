#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This interface represents TextBoxes collection inside single worksheet.
  /// </summary>
  public interface ITextBoxes
  {
    /// <summary>
    /// Returns number of items in the collection.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single item from the collection.
    /// </summary>
    /// <param name="index">Item's index to get.</param>
    /// <returns>Single item from the collection.</returns>
    ITextBoxShape this[ int index ] { get; }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    ITextBoxShape this[ string name ] { get; }
    /// <summary>
    /// Adds new item to the collection. Coordinate means
    /// a) in the case of worksheet - row, column - are corresponding row,
    /// column indexes of the top-left corner, width and height are measured in pixels
    /// b) in the case of chart in Excel 97 all coordinates are measured in 1/4000 of the chart width/height
    /// c) in the case of chart in Excel 2007 all coordinates are measured in 1/1000 of the chart width/height
    /// </summary>
    /// <param name="row">One-based row index of the top-left corner of the new item.</param>
    /// <param name="column">One-based column index of the top-left corner of the new item.</param>
    /// <param name="height">Height of the new item.</param>
    /// <param name="width">Width of the new item.</param>
    /// <returns>Newly added item.</returns>
    ITextBoxShape AddTextBox( int row, int column, int height, int width );
  }
}
