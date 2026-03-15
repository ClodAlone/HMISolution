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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a collection of ranges.
  /// </summary>
  public interface IRanges
    : IParentApplication
    , IRange
  {
    /// <summary>
    /// Adds new range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    void Add( IRange range );
    /// <summary>
    /// Removes range from the collection.
    /// </summary>
    /// <param name="range">Range to remove.</param>
    void Remove( IRange range );
    /// <summary>
    /// Returns item by index from the collection.
    /// </summary>
    IRange this[ int index ] { get; }
  }
}
