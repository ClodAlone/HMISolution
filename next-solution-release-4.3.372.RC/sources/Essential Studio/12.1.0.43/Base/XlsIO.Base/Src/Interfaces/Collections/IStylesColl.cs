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
  /// A collection of all the Style objects in the specified or active
  /// workbook. Each Style object represents a style description for a
  /// range. The Style object contains all style attributes (font,
  /// number format, alignment, and so on) as properties. There are
  /// several built-in styles including Normal, Currency, and Percent
  ///  which are listed in the Style name box in the Style dialog box.
  /// (Format menu).
  /// </summary>
  public interface IStyles : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    /// <summary>
    ///
    /// </summary>
    IStyle _Default { get; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Microsoft Excel application.
    /// </summary>
    IApplication Application{ get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only, Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IStyle this[ int Index ]{ get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IStyle this[ string name ]{ get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Creates a new style and adds it to the list of styles that are
    /// available for the current workbook. Returns a Style object.
    /// </summary>
    /// <param name="Name">Name of the newly created style.</param>
    /// <param name="BasedOn">Prototype for the style.</param>
    /// <returns>Newly created style.</returns>
    IStyle Add( string Name, object BasedOn );
    /// <summary>
    /// Creates a new style and adds it to the list of styles that are
    /// available for the current workbook. Returns a Style object.
    /// </summary>
    /// <param name="Name">Name of the created style.</param>
    /// <returns>Newly created style.</returns>
    IStyle Add( string Name );
    /// <summary>
    /// Merges the styles from another workbook into the Styles collection.
    /// </summary>
    /// <param name="Workbook">Workbook from which all styles will be added.</param>
    /// <param name="overwrite">True - overwrite styles with the same names, otherwise.</param>
    /// <returns>Merged collection.</returns>
    IStyles Merge( object Workbook, bool overwrite );
    /// <summary>
    /// Merges the styles from another workbook into the Styles collection. 
    /// Keep only unique style in collection.
    /// </summary>
    /// <param name="Workbook">Workbook from which all styles will be added.</param>
    /// <returns>Merged collection.</returns>
    IStyles Merge( object Workbook );
    /// <summary>
    /// Method return true if collection contains style 
    /// with specified by user name.
    /// </summary>
    /// <param name="name">Name to check.</param>
    /// <returns>True - if style exists, otherwise False.</returns>
    bool  Contains( string name );
    /// <summary>
    /// Removes style from the collection.
    /// </summary>
    /// <param name="styleName">Style name to remove.</param>
    void Remove( string styleName );
    #endregion
  }
}
