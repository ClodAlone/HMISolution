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
  /// Represents a defined name for a range of cells. Names can be
  /// either built-in names such as Database, Print_Area, and
  /// Auto_Open or custom names.
  /// </summary>
  public interface IName
    : IParentApplication
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
    XlCreator Creator { get; }
    string _Default { get; }
    /// <summary>
    /// Returns or sets the category for the specified name in the language of
    /// the macro. The name must refer to a custom function or command.
    /// Read / write String.
    /// </summary>
    string Category { get; set; }
    /// <summary>
    /// Returns or sets the category for the specified name, in the language of
    /// the user, if the name refers to a custom function or command.
    /// Read / write String.
    /// </summary>
    string CategoryLocal { get; set; }
    /// <summary>
    /// Returns or sets what the name refers to. Read / write XlXLMMacroType.
    /// </summary>
    XlXLMMacroType MacroType { get; set; }
    /// <summary>
    /// Returns or sets the formula that the name is defined to refer to,
    /// in the language of the macro and in A1-style notation, beginning
    /// with an equal sign. Read / write String.
    /// </summary>
    object RefersTo { get; set; }
    /// <summary>
    /// Returns or sets the formula that the name refers to. The formula is
    /// in the language of the user, and it is in A1-style notation, beginning
    /// with an equal sign. Read / write String.
    /// </summary>
    object RefersToLocal { get; set; }
    /// <summary>
    /// Returns or sets the formula that the name refers to. The formula is in
    /// the language of the macro, and it is in R1C1-style notation, beginning
    /// with an equal sign. Read / write String.
    /// </summary>
    object RefersToR1C1 { get; set; }
    /// <summary>
    /// Returns or sets the formula that the name refers to. This formula is in
    /// the language of the user, and it is in R1C1-style notation, beginning
    /// with an equal sign. Read / write String.
    /// </summary>
    object RefersToR1C1Local { get; set; }
    /// <summary>
    /// Returns or sets the shortcut key for a name defined as a custom
    /// Microsoft Excel 4.0 macro command. Read / write String.
    /// </summary>
    string ShortcutKey { get; set; }
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns the index number of the object within the collection of similar
    /// objects. Read-only Long.
    /// </summary>
    int     Index { get; }
    /// <summary>
    /// Returns or sets the name of the object. Read / write String.
    /// </summary>
    string  Name { get; set; }
    /// <summary>
    /// Returns or sets the name of the object, in the language of the user.
    /// Read / write String for Name.
    /// </summary>
    string  NameLocal { get; set; }
    /// <summary>
    /// Gets / sets Range associated with the Name object.
    /// </summary>
    IRange  RefersToRange { get; set; }
    /// <summary>
    /// For the Name object, a string containing the formula that the name is
    /// defined to refer to. The string is in A1-style notation in the language
    /// of the macro, without an equal sign.
    /// </summary>
    string  Value { get; set; }
    /// <summary>
    /// Determines whether the object is visible. Read / write Boolean.
    /// </summary>
    bool    Visible { get; set; }
    /// <summary>
    /// Indicates whether name is local.
    /// </summary>
    bool    IsLocal { get; }
    /// <summary>
    /// Gets named range Value in R1C1 style. Read-only.
    /// </summary>
    string ValueR1C1 { get; }
    /// <summary>
    /// Gets named range RefersTo. Read-only.
    /// </summary>
    string RefersTo { get; }
    /// <summary>
    /// Gets named range RefersTo in R1C1 style. Read-only.
    /// </summary>
    string RefersToR1C1 { get; }
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    IWorksheet Worksheet { get; }
    /// <summary>
    /// Returns string representation of the name's scope. Read-only.
    /// </summary>
    string Scope { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Deletes the object.
    /// </summary>
    void Delete();
    #endregion
  }
}
