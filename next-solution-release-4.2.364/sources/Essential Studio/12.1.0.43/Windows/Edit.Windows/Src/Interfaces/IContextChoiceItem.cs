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

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Single context choice item.
  /// </summary>
  public interface IContextChoiceItem
  {
    /// <summary>
    /// Gets text of the context choice item.
    /// </summary>
    string Text{ get; }
    /// <summary>
    /// Gets or sets fore color of the context choice item.
    /// </summary>
    Color ForeColor{ get; set; }
    /// <summary>
    /// Gets or sets back color of the context choice item.
    /// </summary>
    Color BackColor{ get; set; }
    /// <summary>
    /// Gets tooltip, assigned to the context choice item.
    /// </summary>
    string ToolTip{ get; }
    /// <summary>
    /// Gets or sets named image, assigned to the context choice item.
    /// </summary>
    INamedImage Image{ get; set; }
    /// <summary>
    /// Gets or sets value that indicates whether context choice item is visible.
    /// </summary>
    bool Visible{ get; set; }
    /// <summary>
    /// Gets ID of the item.
    /// </summary>
    int ID{ get; }
		/// <summary>
		/// Gets or sets type of item.
		/// </summary>
		ContextChoiceItemType Type{ get; set; }
  }
}