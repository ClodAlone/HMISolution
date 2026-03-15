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
  /// List of the commands.
  /// </summary>
  public interface IKeyCommandList
    : ICollection
  {
    #region Interface Methods
    /// <summary>
    /// Creates new command and adds it to list.
    /// </summary>
    /// <param name="name">Name of the command.</param>
    /// <returns>Newly created command.</returns>
    IKeyCommand Add( string name );
    /// <summary>
    /// Removes command from list.
    /// </summary>
    /// <param name="name">Name of the command.</param>
    void Remove( string name );
    /// <summary>
    /// Clears list.
    /// </summary>
    void Clear();
    #endregion

    #region Interface Properties
    /// <summary>
    /// GET command by name.
    /// </summary>
    IKeyCommand this[ string name ]{ get; }
    #endregion
  }
}
