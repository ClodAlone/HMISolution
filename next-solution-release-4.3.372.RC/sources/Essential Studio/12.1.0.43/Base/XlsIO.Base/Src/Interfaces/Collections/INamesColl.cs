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
  /// A collection of all the Name objects in the application or
  /// workbook. Each Name object represents a defined name for a
  /// range of cells.
  /// </summary>
  public interface INames : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
    XlCreator Creator { get; }

    IName _Default(object Index, object IndexLocal, object RefersTo);
    /// <summary>
    /// Defines a new name. Returns a Name object.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="RefersTo"></param>
    /// <param name="Visible"></param>
    /// <param name="MacroType"></param>
    /// <param name="ShortcutKey"></param>
    /// <param name="Category"></param>
    /// <param name="NameLocal"></param>
    /// <param name="RefersToLocal"></param>
    /// <param name="CategoryLocal"></param>
    /// <param name="RefersToR1C1"></param>
    /// <param name="RefersToR1C1Local"></param>
    /// <returns></returns>
    IName Add( object Name, object RefersTo, object Visible,
      object MacroType, object ShortcutKey, object Category,
      object NameLocal, object RefersToLocal, object CategoryLocal,
      object RefersToR1C1, object RefersToR1C1Local );
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    IName this[ object Index, object IndexLocal, object RefersTo ]{ get; }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="IndexLocal"></param>
    /// <param name="RefersTo"></param>
    /// <returns></returns>
    IName Item( object Index, object IndexLocal, object RefersTo );
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
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    IName this[ int index ]{ get; }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    IName this[ string name ]{ get; }
    /// <summary>
    /// Returns parent worksheet of the collection.
    /// </summary>
    IWorksheet ParentWorksheet { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Defines a new name. 
    /// </summary>
    /// <param name="name">Name for the new Name object.</param>
    /// <returns>Returns a Name object.</returns>
    IName Add( string name );
    /// <summary>
    /// Defines a new name. 
    /// </summary>
    /// <param name="name">Name for the new Name object.</param>
    /// <param name="namedObject">Range that will be associated with the name.</param>
    IName Add( string name, IRange namedObject );
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name object to add.</param>
    IName Add( IName name );
    /// <summary>
    /// Removes Name object from the collection.
    /// </summary>
    /// <param name="name">Name of the object to remove from the collection.</param>
    void Remove( string name );
    /// <summary>
    /// Removes the element at the specified index of the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    void RemoveAt( int index );
    /// <summary>
    /// Checks whether the Name object is present in the collection or not
    /// </summary>
    /// <param name="name">Name object to check whether it is present or not.</param>
    /// <returns></returns>
    bool Contains( string name );
    #endregion
  }
}
