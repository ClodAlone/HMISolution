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
using System.Diagnostics;

using Microsoft.Vsa;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Describes global objects added to the script engine.
  /// </summary>
  public class CSharpVsaGlobalItem
    : CSharpVsaItem
      , IVsaGlobalItem
  {
    #region Class members
    /// <summary>
    /// The value indicating whether the members of the global object 
    /// should be made available to the script engine. 
    /// </summary>
    private bool m_bExposeMembers;

    /// <summary>
    /// The type of the global item.
    /// </summary>
    private string m_sTypeString;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Construct a CSharpVsaGlobalItem given a script engine and item name.
    /// </summary>
    public CSharpVsaGlobalItem( CSharpScriptEngine engine, string name )
      : base( engine, name )
    {
    }
    #endregion

    #region IVsaGlobalItem members
    /// <summary>
    /// Sets a value indicating whether the members of the 
    /// global object should be made available to the script engine. 
    /// </summary>
    public bool ExposeMembers
    {
      get
      {
        if( m_engine.IsClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        return m_bExposeMembers;
      }
      set
      {
        if( m_engine == null )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_engine.IsRunning )
        {
          throw new VsaException( VsaError.EngineRunning );
        }

        if( m_engine.IsBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        m_bExposeMembers = value;
      }
    }

    /// <summary>
    /// Sets the type of the global item.
    /// </summary>
    public string TypeString
    {
      set
      {
        if( m_engine == null )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_engine.IsRunning )
        {
          throw new VsaException( VsaError.EngineRunning );
        }

        if( m_engine.IsBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        m_sTypeString = value;
      }
    }
    #endregion

    #region IVsaItem members
    /// <summary>
    /// Gets the specified object's type, 
    /// as determined by the IVsaItems.CreateItem method.
    /// </summary>
    public override VsaItemType ItemType
    {
      get
      {
        return VsaItemType.AppGlobal;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Gets the type of the global item.
    /// </summary>
    /// <returns></returns>
    internal string GetTypeString()
    {
      return m_sTypeString;
    }
    #endregion
  }
}