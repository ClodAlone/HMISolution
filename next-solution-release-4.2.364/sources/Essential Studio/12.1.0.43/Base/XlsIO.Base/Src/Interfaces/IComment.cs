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
  /// Represents a cell comment. The Comment object is a member
  /// of the Comments collection.
  /// </summary>
  public interface IComment
    : ITextBox
  {
    #region Not supported
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns a Shape object that represents the shape attached to the
    /// specified comment, diagram node, or hyperlink.
    /// </summary>
    IShape   Shape { get; }
    /// <summary>
    /// Deletes the object.
    /// </summary>
    void Delete();
    /// <summary>
    /// Returns a Comment object that represents the next comment.
    /// </summary>
    /// <returns></returns>
    IComment Next();
    /// <summary>
    /// Returns a Comment object that represents the previous comment.
    /// </summary>
    /// <returns></returns>
    IComment Previous();
    /// <summary>
    /// Sets comment text.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="Overwrite"></param>
    /// <returns></returns>
    string Text(object Text, object Start, object Overwrite);
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns or sets the author of the comment. Read-only String.
    /// </summary>
    string  Author { get; }
    /// <summary>
    /// Determines whether the object is visible. Read / write Boolean.
    /// </summary>
    bool    IsVisible { get; set; }
    /// <summary>
    /// Row of the commented cell. Read-only. 
    /// </summary>
    int     Row { get; }
    /// <summary>
    /// Column of the commented cell. Read-only.
    /// </summary>
    int     Column { get; }
    /// <summary>
    /// True if the size of the specified object is changed automatically
    /// to fit text within its boundaries. Read/write Boolean.
    /// </summary>
    bool AutoSize { get; set; }
    #endregion

//    #region Interface methods
//    /// <summary>
//    /// Removes this comment from the collection.
//    /// </summary>
//    void Remove();
//    #endregion
  }
}