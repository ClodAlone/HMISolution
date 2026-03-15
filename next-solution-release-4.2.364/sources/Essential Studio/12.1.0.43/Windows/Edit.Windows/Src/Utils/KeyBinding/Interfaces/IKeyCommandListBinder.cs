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
  /// Collection of the bindings.
  /// </summary>
  /// <remarks>
  /// Command is always null and Key is always Keys.None.
  /// </remarks>
  public interface IKeyCommandListBinder
    : IKeyCommandBinder
  {
    #region Bindings Add/Remove operations
    /// <summary>
    /// Sets binding of the key to specified command.
    /// </summary>
    /// <param name="key">Key to be binded.</param>
    /// <param name="command">Name of the command, 
    /// the key is to be binded to.</param>
    /// <returns>Command if binding 
    /// completed successfully, or null of binding failed.</returns>
    /// <remarks>
    /// It is not necessary to create command before binding. 
    /// If it does not exists, it will be created.
    /// </remarks>
    IKeyCommand BindToCommand( Keys key, string command );
    /// <summary>
    /// Sets binding for the key to the new command.
    /// </summary>
    /// <param name="key">Key to be binded.</param>
    /// <returns>Returns existing binder, or creates new if 
    /// key was not binded before or was binded to command.</returns>
    IKeyCommandListBinder BindToBinder( Keys key );
    /// <summary>
    /// Removes any associated binding for the specified key.
    /// </summary>
    /// <param name="key">Key to be unbinded.</param>
    void RemoveBinding( Keys key );
    #endregion
    
    #region Bindings Search Operations
    /// <summary>
    /// Searches for bindings of the command.
    /// </summary>
    /// <param name="command">Name of the command.</param>
    /// <returns>Bindings, that are assigned to that command.</returns>
    IKeyCommandBinder[] FindBindings( string command );
    /// <summary>
    /// Searches for binding of the keys sequence.
    /// </summary>
    /// <param name="keySequence"></param>
    /// <param name="iStart"></param>
    /// <returns>Bindings, that are assigned to that command.</returns>
    IKeyCommandBinder FindBinding( Keys[] keySequence, int iStart );
    /// <summary>
    /// GET binding for the key.
    /// </summary>
    IKeyCommandBinder this[ Keys key ]{ get; }
    #endregion
  }
}
