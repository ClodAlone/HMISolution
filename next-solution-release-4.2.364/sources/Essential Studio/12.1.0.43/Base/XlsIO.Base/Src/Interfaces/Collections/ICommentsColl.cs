#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// A collection of cell comments. Each comment is represented by a
  /// Comment object.
  /// </summary>
  public interface IComments : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    Comment _Default { get; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Microsoft Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    ICommentShape this[ int Index ]{ get; }
    /// <summary>
    /// Returns single entry from the collection by row and column one-based indexes. Read-only.
    /// </summary>
    ICommentShape this[ int iRow, int iColumn ]{ get; }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    ICommentShape this[ string name ] { get; }
    /// <summary>
    /// Remove all items from the collection.
    /// </summary>
    void Clear();
    #endregion
  }
}
