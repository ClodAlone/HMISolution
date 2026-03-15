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

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Type of change in stream.
  /// </summary>
  public enum ChangeType
  {
		/// <summary>
		/// Text was inserted.
		/// </summary>
    Insert,
		/// <summary>
		/// Text was replaced.
		/// </summary>
    Replace,
		/// <summary>
		/// Text was deleted.
		/// </summary>
    Delete
  }

  /// <summary>
  /// Change in the stream. Needed for rolling back.
  /// </summary>
  public interface IChange
  {
    /// <summary>
    /// Position in stream.
    /// </summary>
    long Position{ get; }
    /// <summary>
    /// Type of change.
    /// </summary>
    ChangeType Type{ get; }
    /// <summary>
    /// Data for change. (for replace and insert)
    /// </summary>
    byte[]     Data{ get; }
    /// <summary>
    /// Size of data to be affected. (for delete and replace)
    /// </summary>
    long       Size{ get; }
  }
}
