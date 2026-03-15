#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Summary description for IWorksheetCustomProperties.
  /// </summary>
  public interface IWorksheetCustomProperties
  {
    #region Interface properties
    /// <summary>
    /// Returns single entry from the collection by index. Read-only.
    /// </summary>
    ICustomProperty this[ int index ]{ get; }
    /// <summary>
    /// Returns single entry from the collection by index. Read-only.
    /// </summary>
    ICustomProperty this[ string strName ]{ get; }
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Count { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Adds new property to the collection.
    /// </summary>
    /// <param name="strName">Name of the new property.</param>
    /// <returns>Newly created property.</returns>
    ICustomProperty Add( string strName );
    /// <summary>
    /// Determines whether collection contains property with a specific name.
    /// </summary>
    /// <param name="strName">The name of the property to locate.</param>
    /// <returns>True if collection contains required element.</returns>
    bool Contains( string strName );
    #endregion

  }
}
