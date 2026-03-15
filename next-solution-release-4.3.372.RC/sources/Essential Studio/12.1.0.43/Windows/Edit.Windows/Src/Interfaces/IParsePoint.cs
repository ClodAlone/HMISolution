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

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface used to get parse point from which lexem started in stream.
  /// </summary>
  public interface IParsePoint
  {
    /// <summary>
    /// Line from which parse point started
    /// </summary>
    int Line{ get; }
    /// <summary>
    /// Position in line where lexem started
    /// </summary>
    int Position{ get; }
    /// <summary>
    /// Offeset in the inner stream
    /// </summary>
    long Offset{ get; }
    /// <summary>
    /// GET sing of validity of parsepoint.
    /// </summary>
    bool IsValid{ get; }
		/// <summary>
		/// Raised when some parameter of parse point is changed.
		/// </summary>
		event ParsePointParameterChangedEventHandler ParsePointParameterChanged;
    /// <summary>
    /// Event that is raised when point is deleted from collection
    /// and became unreliable.
    /// </summary>
    event ParsePointDeletedEventHandler Deleted;
  }
}