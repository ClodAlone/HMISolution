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
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface that represents bookmark with custom drawing.
  /// </summary>
  public interface ICustomBookmark
    : IBookmark
  {
    /// <summary>
    /// Event, that is raised when bookmark needs to be drawn.
    /// </summary>
    event BookmarkPaintEventHandler DrawBookmark;
		/// <summary>
    /// Gets or sets value indicating whether bookmark 
    /// can be found while searching for next/previous bookmark.
    /// </summary>
    bool UseInBookmarkSearch{ get; set; }

    /// <summary>
    /// Gets or Sets data about the bookmark
    /// </summary>
    object tag { get; set; }

  }

}