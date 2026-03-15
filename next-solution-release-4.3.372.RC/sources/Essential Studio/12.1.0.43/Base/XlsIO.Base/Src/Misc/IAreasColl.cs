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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// A collection of the areas or contiguous blocks of cells
  /// within a selection. There is no singular Area object.
  /// Individual members of the Areas collection are Range objects.
  /// The Areas collection contains one Range object for each discrete,
  /// contiguous range of cells within the selection. If the selection
  /// contains only one area, the Areas collection contains a single
  /// Range object that corresponds to that selection.
  /// </summary>
  public interface IAreas : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    Range _Default { get; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IRange this[ int Index ] { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    #endregion
  }
}
