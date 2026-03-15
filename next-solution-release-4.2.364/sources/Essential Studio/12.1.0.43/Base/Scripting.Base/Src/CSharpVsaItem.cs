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
using System.Diagnostics;

using Microsoft.Vsa;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Implements an interface for all items added to the CSharpVsaEngine, 
  /// including code items, reference items, and global items. 
  /// It defines generic properties and methods that apply to all item types 
  /// recognized by the engine.
  /// </summary>
  public abstract class CSharpVsaItem : IVsaItem
  {
    #region Class members
    /// <summary>
    /// The reference to script engine.
    /// </summary>
    protected CSharpScriptEngine m_engine;

    /// <summary>
    /// The name of item.
    /// </summary>
    protected string m_name = string.Empty;
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Disable default constructor
    /// </summary>
    private CSharpVsaItem()
    {
    }

    /// <summary>
    /// Construct a CSharpVsaItem given a script engine
    /// </summary>
    protected CSharpVsaItem( CSharpScriptEngine engine )
    {
      if( engine == null )
      {
        throw new ArgumentNullException( "engine" );
      }

      m_engine = engine;
    }

    /// <summary>
    /// Construct a CSharpVsaItem given a script engine and item name
    /// </summary>
    protected CSharpVsaItem( CSharpScriptEngine engine, string name )
    {
      if( engine == null )
      {
        throw new ArgumentNullException( "engine" );
      }

      m_engine = engine;
      this.Name = name;
    }
    #endregion

    #region IVsaItem implementations
    /// <summary>
    /// Gets the specified object's type, as determined by the IVsaItems.CreateItem method.
    /// </summary>
    public abstract VsaItemType ItemType { get; }

    /// <summary>
    /// Gets implementation-specific options for a script engine
    /// </summary>
    /// <param name="name">The name of the option to retrieve</param>
    /// <returns>Returns the value of the specified option</returns>
    public virtual object GetOption( string name )
    {
      return m_engine.GetOption( name );
    }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_name;
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

        if( m_name != value )
        {
          if( value == null || value.Trim() == string.Empty )
          {
            throw new VsaException( VsaError.ItemNameInvalid );
          }

          CSharpVsaItems items = m_engine.Items as CSharpVsaItems;
          if( items.ContainsName( value ) )
          {
            throw new VsaException( VsaError.ItemNameInUse );
          }

          m_name = value;
        }
      }
    }

    /// <summary>
    /// Returns a value indicating whether the current 
    /// in-memory representation of the item differs from the persisted representation.
    /// </summary>
    public bool IsDirty
    {
      get
      {
        // TODO:  Add CSharpItem.IsDirty getter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets implementation-specific options for a script engine.
    /// </summary>
    /// <param name="name">The name of the option to set. </param>
    /// <param name="value">A new value for the option.</param>
    public virtual void SetOption( string name, object value )
    {
      m_engine.SetOption( name, value );
    }
    #endregion
  }
}