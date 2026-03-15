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
using Syncfusion.Windows.Forms.Edit;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Controller of the long operations.
  /// </summary>
  public interface ILongOperationController
  {
		/// <summary>
		/// Event, that is raised when operation is to be started.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
    ILongOperation StartOperation( string name );
		/// <summary>
		/// Event that is raised when operation started.
		/// </summary>
    event LongOperationEventHandler OperationStarted;
		/// <summary>
		/// Event that is raised when operation stopped.
		/// </summary>
    event LongOperationEventHandler OperationStopped;
  }
  /// <summary>
  /// Controller of the long operations.
  /// </summary>
  internal interface ILongOperationControllerInternal
    : ILongOperationController
  {
		/// <summary>
		/// Raises when operation has started.
		/// </summary>
		/// <param name="operation"></param>
    void RaiseOperationStart( ILongOperation operation );
		/// <summary>
		/// Raises when operation has stoped.
		/// </summary>
		/// <param name="operation"></param>
    void RaiseOperationEnd( ILongOperation operation );
  }

}
