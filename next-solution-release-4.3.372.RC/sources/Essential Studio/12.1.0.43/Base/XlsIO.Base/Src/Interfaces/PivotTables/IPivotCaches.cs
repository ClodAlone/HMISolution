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
  /// Represents collection of workbook pivot caches.
  /// </summary>
  public interface IPivotCaches
  {
    /// <summary>
    /// Returns number of items in the collection. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    /// <param name="index">Zero-based index of the item to return.</param>
    /// <returns>Single entry from the collection.</returns>
    IPivotCache this[ int index ] { get; }
    /// <summary>
    /// Creates new chache object inside this collection.
    /// </summary>
    /// <param name="range">Range that contains data to cache.</param>
    /// <returns>Newly created object.</returns>
    IPivotCache Add( IRange range );
  }
}
