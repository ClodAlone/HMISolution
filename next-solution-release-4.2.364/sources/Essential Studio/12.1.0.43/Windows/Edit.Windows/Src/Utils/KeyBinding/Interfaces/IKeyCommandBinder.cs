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

namespace Syncfusion.Shared.Utils.KeyBinding
{
  /// <summary>
  /// Single key=command binding.
  /// </summary>
  public interface IKeyCommandBinder
  {
    #region Interface Properties
    /// <summary>
    /// Gets parent list, binding belongs to.
    /// </summary>
    IKeyCommandListBinder Parent{ get; }
    /// <summary>
    /// Gets command, that is binded. 
    /// </summary>
    IKeyCommand Command{ get; }
    /// <summary>
    /// Gets key, that is binded. 
    /// </summary>
    Keys Key{ get; }
    #endregion

    #region Interface Methods
    /// <summary>
    /// Tries to process key.
    /// </summary>
    /// <param name="key">Key to be processed.</param>
    /// <returns>True if key was processed, otherwise false.</returns>
    bool ProcessKey( Keys key );
    /// <summary>
    /// Gets full name of the combination, current binding is related to.
    /// </summary>
    /// <returns>String that represents currently used combination.</returns>
    string GetCombinationName();
    #endregion
  }
}
