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
  /// Represents pivot cache data field collection.
  /// </summary>
  public interface IPivotDataFields
  {
    /// <summary>
    /// Gets single entry from the collection.
    /// </summary>
    /// <param name="index">Zero-based index of the entry to get.</param>
    /// <returns>Requested data field object.</returns>
    IPivotDataField this[ int index ] { get; }
    /// <summary>
    /// Adds new data field to the collection.
    /// </summary>
    /// <param name="field">Parent field to use.</param>
    /// <param name="name">Name for the new data field.</param>
    /// <param name="subtotal">Subtotal function for the new data field.</param>
    /// <returns>Newly added data field.</returns>
    IPivotDataField Add( IPivotField field, string name, PivotSubtotalTypes subtotal );
      /// <summary>
      /// Gets the number of fields in the collection.
      /// </summary>
    int Count { get; }
  }
}
