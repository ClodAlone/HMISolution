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
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a collection of conditional formats.
  /// </summary>
  public interface IConditionalFormats
    : IEnumerable
    , IParentApplication
    , IOptimizedUpdate
  {
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single element from the collection. Read-only.
    /// </summary>
    IConditionalFormat this[ int index ] { get; }
    /// <summary>
    /// Adds new condition to the collection.
    /// </summary>
    /// <returns>Newly added condition.</returns>
    IConditionalFormat AddCondition();
    /// <summary>
    /// Removes the conditional formatting.
    /// </summary>
    void Remove();
    /// <summary>
    /// Removes the conditional formatting at the specified Index.
    /// </summary>
    void RemoveAt(int index);

  }
}
