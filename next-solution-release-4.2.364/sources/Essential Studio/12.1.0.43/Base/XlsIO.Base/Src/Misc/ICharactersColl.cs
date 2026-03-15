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

namespace Syncfusion.XlsIO.Interfaces.Collections
{
  /// <summary>
  /// Represents characters in an object that contains text. The
  /// characters object allows modification of any sequence of characters
  /// contained in the full text string.
  /// </summary>
  public interface ICharacters
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
    XlCreator Creator { get; }
    string PhoneticCharacters { get; set; }
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// The text of this range of characters. Read-only String.
    /// </summary>
    string Caption { get; set; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a Font object that represents the font of the specified object.
    /// </summary>
    IFont Font { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    /// <summary>
    /// Returns or sets the text for the specified object.
    /// Read-only String for the Range object, read / write String for
    /// all other objects.
    /// </summary>
    string Text { get; set; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Deletes the object.
    /// </summary>
    /// <returns></returns>
    object Delete();
    /// <summary>
    /// Inserts a string preceding the selected characters.
    /// </summary>
    /// <param name="String"></param>
    /// <returns></returns>
    object Insert(string String);
    #endregion
  }
}
