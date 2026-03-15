#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This interface represents TextBox form control.
  /// </summary>
  public interface ITextBox : IParentApplication
  {
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    ExcelCommentHAlign HAlignment { get; set; }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    ExcelCommentVAlign VAlignment { get; set; }
    /// <summary>
    /// Text rotation.
    /// </summary>
    ExcelTextRotation TextRotation { get; set; }
    /// <summary>
    /// Indicates whether comment text is locked.
    /// </summary>
    bool IsTextLocked { get; set; }
    /// <summary>
    /// Text of the comment. Read-only.
    /// </summary>
    IRichTextString RichText { get; set; }
    /// <summary>
    /// Text of the comment.
    /// </summary>
    string  Text { get; set; }
  }
}
