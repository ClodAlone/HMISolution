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
  /// A collection of vertical page breaks within the print area.
  /// Each vertical page break is represented by a VPageBreak object.
  /// </summary>
  public interface IVPageBreaks : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    VPageBreak _Default { get; }
    XlCreator Creator { get; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an
    /// Application object that represents the Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only, Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IVPageBreak this[ int Index ] { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Adds a vertical page break. Returns a VPageBreak object.
    /// </summary>
    /// <param name="location">Location of the break.</param>
    /// <returns>Newly added page break.</returns>
    IVPageBreak Add( IRange location );
    /// <summary>
    /// Removes a page break from the specified location.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    IVPageBreak Remove( IRange location );
    /// <summary>
    /// Returns page break at the specified column.
    /// </summary>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Page break with corresponding column or null if not found.</returns>
    IVPageBreak GetPageBreak( int iColumn );
    /// <summary>
    /// Clears vertical page breaks from VPageBreaks collection.
    /// </summary>
    /// <returns></returns>
    void Clear();
    #endregion
  }
}
