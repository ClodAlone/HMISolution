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
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface for dynamic formatting.
  /// </summary>
  public interface IDynamicFormat
  {
    /// <summary>
    /// Start point of the formatting.
    /// </summary>
    CoordinatePoint Start{ get; }
    /// <summary>
    /// End point of the formatting.
    /// It is not included into formatting range.
    /// </summary>
    CoordinatePoint End{ get; }
    /// <summary>
    /// Format to be used to draw text.
    /// Just FontColor, ForeColor and BackColor must be used.
    /// </summary>
    ISnippetFormat Format{ get; }
  }
}
