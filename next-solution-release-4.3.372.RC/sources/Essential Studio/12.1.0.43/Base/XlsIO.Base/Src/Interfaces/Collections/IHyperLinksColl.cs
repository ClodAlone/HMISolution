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
  /// Represents collection of hyperlinks.
  /// </summary>
  public interface IHyperLinks : IParentApplication
  {
    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single hyperlink object from the collection. Read-only.
    /// </summary>
    IHyperLink this[ int index ]{ get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Defines a new hyperlink.
    /// </summary>
    /// <param name="range">
    /// Range object that represents the range new hyperlink is attached to.
    /// </param>
    IHyperLink Add( IRange range );
    /// <summary>
    /// Removes HyperLink object from the collection.
    /// </summary>
    /// <param name="index">HyperLink index to remove.</param>
    void RemoveAt( int index );
    #endregion
  }
}
