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
  /// Long operation interface.
  /// </summary>
  public interface ILongOperation
    : IDisposable
  {
    /// <summary>
    /// GET ID of the operation.
    /// </summary>
    Guid ID{ get; }
    /// <summary>
    /// GET time of operation activity.
    /// </summary>
    TimeSpan RunningTime{ get; }
    /// <summary>
    /// GET name of the operation.
    /// </summary>
    string Name{ get; }
    /// <summary>
    /// GET sing whether operation is running now.
    /// </summary>
    bool IsRunning{ get; }
    /// <summary>
    /// Stops operation.
    /// </summary>
    /// <remarks>
    /// Operation is no longer valid.
    /// </remarks>
    void Stop();
  }
}
