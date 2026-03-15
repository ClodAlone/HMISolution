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
using System.Diagnostics;

using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// 
  /// </summary>
  public class ScriptEventArgs : EventArgs
  {
    #region Constructors
    public ScriptEventArgs()
    {
    }

    public ScriptEventArgs( Script script )
    {
      this.script = script;
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// 
    /// </summary>
    public Script Script
    {
      get
      {
        return this.script;
      }
    }
    #endregion

    #region Fields
    private Script script = null;
    #endregion
  }

  /// <summary>
  /// Signature for script event handlers.
  /// </summary>
  public delegate void ScriptEventHandler( object sender, ScriptEventArgs evtArgs );

  /// <summary>
  /// Encapsulates arguments for VSA error events.
  /// </summary>
  public class VsaErrorEventArgs : EventArgs
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vsaError"></param>
    public VsaErrorEventArgs( IVsaError vsaError )
    {
      this.vsaError = vsaError;
    }

    /// <summary>
    /// 
    /// </summary>
    public IVsaError Error
    {
      get
      {
        return this.vsaError;
      }
    }

    private IVsaError vsaError = null;
  }

  /// <summary>
  /// Signature for VSA error event handlers.
  /// </summary>
  public delegate void VsaErrorEventHandler( object sender, VsaErrorEventArgs evtArgs );
}