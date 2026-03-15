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
using Syncfusion.Drawing;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface that represents single bookmark.
  /// </summary>
  public interface IBookmark
  {
    /// <summary>
    /// Gets line, the bookmark points to.
    /// </summary>
    int Line{ get; }
    /// <summary>
    /// Gets index of the bookmark. If bookmark is not indexed, index will be negative.
    /// </summary>
    int Index{ get; }
		/// <summary>
		/// Gets BrushInfo object that is using for painting bookmarks. User can change it's members.
		/// </summary>
		BrushInfo BookmarkBrush{ get; set; }
    /// <summary>
    /// Gets or sets color of bookmark border.
    /// </summary>
    Color BorderColor{ get; set; }
  }
}