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
  /// The collection of horizontal page breaks within the print area.
  /// Each horizontal page break is represented by an HPageBreak object.
  /// </summary>
  public interface IHPageBreaks : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    IHPageBreak _Default { get; }
    XlCreator Creator { get; }
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
    /// Returns a single object from a collection. Read-only.
    /// </summary>
    IHPageBreak this[ int Index ] { get; }
    /// <summary>
    /// Returns a single object from a collection. Read-only.
    /// </summary>
    IHPageBreak this[ IRange location ] { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Adds a horizontal page break. Returns an HPageBreak object.
    /// </summary>
    /// <param name="location">Location of the page break.</param>
    /// <returns>HPageBreak which was added.</returns>
    IHPageBreak Add( IRange location );
    /// <summary>
    /// Removes a pagebreak from the specified location.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    IHPageBreak Remove( IRange location );
    /// <summary>
    /// Returns page break at the specified column.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <returns>Page break with corresponding row or null if not found.</returns>
    IHPageBreak GetPageBreak( int iRow );
    /// <summary>
    /// Clears horizontal page breaks from HPageBreaks collection.
    /// </summary>
    /// <returns></returns>
    void Clear();
    #endregion
  }
}
