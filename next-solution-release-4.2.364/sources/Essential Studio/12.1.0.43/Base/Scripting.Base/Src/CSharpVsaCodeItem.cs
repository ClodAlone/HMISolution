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
using System.CodeDom;
using System.Collections;
using System.Diagnostics;

using Microsoft.Vsa;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Summary description for CSharpCodeItem.
  /// </summary>
  public class CSharpVsaCodeItem
    : CSharpVsaItem
      , IVsaCodeItem
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private string m_sSourceText = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    private Hashtable m_hashEventSource = new Hashtable();

    /// <summary>
    /// 
    /// </summary>
    private VsaItemFlag m_flag = VsaItemFlag.None;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    ///
    /// </summary>
    public CSharpVsaCodeItem( CSharpScriptEngine engine, string name )
      : base( engine, name )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public CSharpVsaCodeItem( CSharpScriptEngine engine, string name, VsaItemFlag flag )
      :
        this( engine, name )
    {
      m_flag = flag;
    }
    #endregion

    #region IVsaCodeItem properties
    /// <summary>
    /// 
    /// </summary>
    public CodeObject CodeDOM
    {
      get
      {
        throw new NotImplementedException( "Not implemented CSharpCodeItem.CodeDOM propety" );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string SourceText
    {
      get
      {
        if( m_engine.IsClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        return m_sSourceText;
      }
      set
      {
        if( m_engine.IsClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_engine.IsRunning )
        {
          throw new VsaException( VsaError.EngineRunning );
        }

        m_sSourceText = value;
      }
    }
    #endregion

    #region IVsaCodeItem methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    public void AppendSourceText( string text )
    {
      if( m_engine.IsClosed )
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

      m_sSourceText += text;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventSourceName"></param>
    /// <param name="eventSourceType"></param>
    public void AddEventSource( string eventSourceName, string eventSourceType )
    {
      if( m_engine.IsClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_engine.IsRunning )
      {
        throw new VsaException( VsaError.EngineRunning );
      }

      if( eventSourceName == null || eventSourceName.Trim() == string.Empty )
      {
        throw new VsaException( VsaError.EventSourceNameInvalid );
      }

      if( eventSourceType == null || eventSourceType.Trim() == string.Empty )
      {
        throw new VsaException( VsaError.EventSourceTypeInvalid );
      }

      if( m_hashEventSource.ContainsKey( eventSourceName ) )
      {
        throw new VsaException( VsaError.EventSourceNameInUse );
      }

      m_hashEventSource.Add( eventSourceName, eventSourceType );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventSourceName"></param>
    /// <param name="eventSourceType"></param>
    public void RemoveEventSource( string eventSourceName )
    {
      if( m_engine.IsClosed )
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

      if( !m_hashEventSource.ContainsKey( eventSourceName ) )
      {
        throw new VsaException( VsaError.EventSourceNotFound );
      }

      m_hashEventSource.Remove( eventSourceName );
    }
    #endregion

    #region IVsaItem members
    /// <summary>
    /// 
    /// </summary>
    public override VsaItemType ItemType
    {
      get
      {
        return VsaItemType.Code;
      }
    }
    #endregion
  }
}