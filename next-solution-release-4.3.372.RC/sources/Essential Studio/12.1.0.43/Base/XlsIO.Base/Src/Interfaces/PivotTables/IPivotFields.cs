#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents field collection inside pivot table.
  /// </summary>
  public interface IPivotFields:IEnumerable
  {
    /// <summary>
    /// Returns number of elements in the collection.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="index">Item index to return.</param>
    /// <returns>Single entry from the collection.</returns>
    IPivotField this[ int index ] { get; }
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single entry from the collection.</returns>
    IPivotField this[ string name ] { get; }
  }
}
