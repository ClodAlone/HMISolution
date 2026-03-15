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
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit;

namespace Syncfusion.Shared.Utils.KeyBinding
{
  /// <summary>
  /// Single command.
  /// </summary>
  public interface IKeyCommand
  {
    #region Interface Properties
    /// <summary>
    /// GET name of the command.
    /// </summary>
    string Name{ get; }
    #endregion

    #region Interface Events
    /// <summary>
    /// Event, that is raised when command must be processed.
    /// </summary>
    event ProcessCommandEventHandler ProcessCommand;
    #endregion

    #region Interface Methods
    /// <summary>
    /// Executes command.
    /// </summary>
    void Execute();
    #endregion
  }
}
