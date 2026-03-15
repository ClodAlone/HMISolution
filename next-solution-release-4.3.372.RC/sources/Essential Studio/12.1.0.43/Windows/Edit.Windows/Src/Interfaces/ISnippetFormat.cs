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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;

using System.Windows.Forms;
using System.IO;
using Syncfusion.Windows.Forms.Edit.Enums;

using Syncfusion.Windows.Forms.Edit;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Format is rendering utility object. It hold settings needed for proper rendering
  /// one or more snippets on user screen.
  /// </summary>
  public interface ISnippetFormat
    : IBackgroundFormat
  {
    #region interface properties
    /// <summary>
    /// Unique format name
    /// </summary>
    string Name{ get; }
    /// <summary>
    /// Font which must be used for rendering
    /// </summary>
    Font  Font{ get; set; }
    /// <summary>
    /// Color of snippet text.
    /// </summary>
    Color FontColor{ get; set; }
    /// <summary>
    /// Get or set color of underline.
    /// </summary>
    Color LineColor{ get; set; }
    /// <summary>
    /// Weight of snippet text underline drawing
    /// </summary>
    UnderlineWeight UnderlineWeight{ get; set; }
    /// <summary>
    /// Style of snippet text underline drawing
    /// </summary>
    UnderlineStyle  UnderlineStyle{ get; set; }
    /// <summary>
    /// Gets vaule indicating whether custom control should be used instead of rendering text.
    /// </summary>
    bool UseCustomControl{ get; }
    /// <summary>
    /// Gets vaule that specifies whether hatch style settings 
    /// should be applied on background filling or background should 
    /// be solid.
    /// </summary>
    bool UseHatchFill{ get; }
    /// <summary>
    /// Gets text striking out.
    /// </summary>
    bool StrikeOut{ get; }
		/// <summary>
		/// Gets or sets style of border.
		/// </summary>
		FrameBorderStyle BorderStyle{ get; set; }
		/// <summary>
		/// Gets or sets color of border line.
		/// </summary>
		Color BorderColor{ get; set; }
		/// <summary>
		/// Gets or sets weight of border line.
		/// </summary>
		BorderWeight BorderWeight{ get; set; }
    #endregion

    #region interfce events
    /// <summary>
    /// This event is raised by renderer when paint works started.
    /// user
    /// </summary>
    event CustomSnippetDrawEventHandler OnCustomDraw;
    #endregion
  }
}