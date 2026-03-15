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
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;

using Syncfusion.Windows.Forms.Edit.Enums;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Lexem that is rendered.
  /// If column is -1 then lexem is considered to be unrendered.
  /// </summary>
  public interface IRenderedLexem
    : ILexem
  {
    /// <summary>
    /// Width of the lexem.
    /// </summary>
    float Width{ get; set; }
    /// <summary>
    /// If word-wrapping is enabled, then it is zero-based index of the sub line,
    /// where lexem is drawn.
    /// </summary>
    int SubLine{ get; set; }
    /// <summary>
    /// X offset of the lexem. Relative to the lexem's sub line.
    /// </summary>
    float XOffset{ get; set; }
    /// <summary>
    /// Y offset of the lexem.
    /// </summary>
    float YOffset{ get; set; }
  }
}
