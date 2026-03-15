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
  /// This interface represents TextBox form control shape.
  /// </summary>
  public interface ITextBoxShape :
    ITextBox,
    IShape
  {
  }
  /// <summary>
  /// This interface represents TextBox form control shape.
  /// </summary>
  public interface ITextBoxShapeEx:ITextBoxShape
  {
      /// <summary>
      /// Text of the comment.
      /// </summary>
      string TextLink { get; set; }
  }
}
