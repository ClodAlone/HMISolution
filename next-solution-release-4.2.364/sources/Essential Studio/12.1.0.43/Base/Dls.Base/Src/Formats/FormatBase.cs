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
using System.Collections;
using System.Diagnostics;

using Syncfusion.DLS.XML;
using System.Collections.Specialized;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents Base Formatting.
  /// </summary>
  public abstract class FormatBase
    : XDLSSerializableBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_OFFSET_STEP = 8;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_OFFSET_LEVELS_MAX = 4;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_OFFSET_MAX = DEF_OFFSET_STEP * DEF_OFFSET_LEVELS_MAX; // = 32
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_KEY_MAX = 128;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private HybridDictionary m_propertiesHash;
    /// <summary>
    /// 
    /// </summary>
    private FormatBase m_baseFormat = null;
    /// <summary>
    /// 
    /// </summary>
    private FormatBase m_parentFormat;
    /// <summary>
    /// 
    /// </summary>
    private int m_parentKey = 0;
    /// <summary>
    /// 
    /// </summary>
    protected int m_keysOffset = 0;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bDefault = true;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets whether format is default.
    /// </summary>
    public bool IsDefault
    {
      get
      {
        return m_bDefault;
      }
      set
      {
        if( value == false )
        {
          MarkNoDefault();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public HybridDictionary PropertiesHash
    {
      get
      {
        return m_propertiesHash;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal FormatBase BaseFormat
    {
      get
      {
        return m_baseFormat;
      }
      set
      {
        m_baseFormat = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal int KeysOffset
    {
      get
      {
        return m_keysOffset;
      }
    } 
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected object this[ int key ]
    {
      get
      {
        int fullKey = GetFullKey( key );

        // 1. Gets SELF VALUE
        object value = PropertiesHash[ fullKey ];
        
        // 2.1. If VALUE is NULL get COMPOSITE DEFAULT VALUE
        if( value == null )
        {
          // Gets and updates composite value in hash.
          value = GetDefComposite( key );
        }

        // 2.2 If SELF VALUE is NULL get BASE VALUE
        if( value == null && BaseFormat != null )
        {
          value = BaseFormat[ key ];
        }

        // 3. If VALUE is NULL get DEFAULT VALUE
        if( value == null )
        {
          value = GetDefValue( key );
        }

        return value;
      }
      set
      {
        int fullkey = GetFullKey( key );

        //if( !this[ key ].Equals( value ) )
        {
          PropertiesHash[ fullkey ] = value;
          IsDefault = false;
        }
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates first level "format".
    /// </summary>
    public FormatBase()
    {
      m_propertiesHash = new HybridDictionary();
    }
    /// <summary>
    /// Creates first level "format".
    /// </summary>
    public FormatBase( IDocument doc )
    : base( doc )
    {
      m_propertiesHash = new HybridDictionary();
    }
    /// <summary>
    /// Creates child "format" ( composite property ).
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="parentKey"></param>
    public FormatBase( FormatBase parent, int parentKey )
    {
      if( parent.KeysOffset + DEF_OFFSET_STEP > DEF_OFFSET_MAX )
      {
        throw new ArgumentOutOfRangeException( "offset" );
      }
      if( parentKey > DEF_KEY_MAX )
      {
        throw new ArgumentOutOfRangeException( "parentKey" );
      }

      m_propertiesHash = parent.PropertiesHash;
      m_parentKey = parentKey;
      m_parentFormat = parent;
      m_keysOffset = parent.KeysOffset + DEF_OFFSET_STEP;
    }
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="parentKey"></param>
    /// <param name="parentOffset"></param>
    public FormatBase( FormatBase parent, int parentKey, int parentOffset )
      : this( parent, parentKey )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal protected void ImportContainer( FormatBase format )
    {
      HybridDictionary srcHD = format.PropertiesHash;
      m_propertiesHash = new HybridDictionary( srcHD.Count );

      IDictionaryEnumerator dicEn = srcHD.GetEnumerator();

      while( dicEn.MoveNext() )
      {
        m_propertiesHash.Add( dicEn.Key, dicEn.Value );
      }
//      DBG_TraceFormat( format );
//      DBG_TraceFormat( this );
      
      EnsureComposites();
//      DBG_CheckChildComposites( m_propertiesHash );
      IsDefault = false;

      ImportMembers( format );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    protected virtual void ImportMembers( FormatBase format )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseFormat"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal void ApplyBase( FormatBase baseFormat )
    {
      m_baseFormat = baseFormat;
    }
    /// <summary>
    /// Checks if Key exists.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool HasKey( int key )
    {
      return ( PropertiesHash[ GetFullKey( key ) ] != null );
    }
    #endregion

    #region Class virtual methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected abstract object GetDefValue( int key );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual FormatBase GetDefComposite( int key )
    {
      return null;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal virtual void EnsureComposites()
    {}
    /// <summary>
    /// 
    /// </summary>
    protected void EnsureComposites( params int[] keys )
    {
      foreach( int key in keys )
      {
        FormatBase format = GetDefComposite( key );
        format.EnsureComposites();
        format.IsDefault = false;
//        DBG_CheckChildComposites( format.PropertiesHash );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected int GetBaseKey( int key )
    {
      return key - ( m_parentKey << m_keysOffset );
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected int GetFullKey( int key )
    {
      if( key > DEF_KEY_MAX )
      {
        throw new ArgumentOutOfRangeException( "key" );
      }

      return key + ( m_parentKey << m_keysOffset );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected FormatBase GetDefComposite( int key, FormatBase value )
    {
      int fullKey = GetFullKey( key );
      
      // Updates composite value in hash
      PropertiesHash[ fullKey ] = value;

      if( BaseFormat != null )
      {
        // Makes link to base composite property
        value.ApplyBase( BaseFormat.PropertiesHash[ fullKey ] as FormatBase );
      }

      return value;
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private void MarkNoDefault()
    {
      m_bDefault = false;
        
      if( m_parentFormat != null )
      {
        m_parentFormat.IsDefault = false;
      }
    }
    #endregion

    #region DEBUG
    //    private void DBG_TraceFormat( FormatBase format )
//    {
//      Trace.WriteLine( "----------------------------" );
//      Trace.WriteLine( format.GetType().Name );
//      Trace.WriteLine( "----------------------------" );
//      Trace.WriteLine( string.Format( "IsDefault: {0}", format.IsDefault ) );
//      
//      IDictionaryEnumerator dicEn = format.PropertiesHash.GetEnumerator();
//      while(dicEn.MoveNext())
//      {
//        FormatBase fb = dicEn.Value as FormatBase;
//        if( fb != null )
//        {
//          Trace.WriteLine( string.Format( "{0} | IsDefault: {1}", 
//                                          fb.GetType().Name, 
//                                          fb.IsDefault) 
//            );
//        }
//      }
//    }
//    private void DBG_CheckChildComposites( Hashtable props )
//    {
//      IDictionaryEnumerator dicEn = props.GetEnumerator();
//      while(dicEn.MoveNext())
//      {
//        FormatBase fb = dicEn.Value as FormatBase;
//        if( fb != null )
//        {
//          if( fb.PropertiesHash != props )
//          {
//            DBG_TraceFormat( fb );
//            throw new InvalidOperationException( "Child format base must have the same Hash with parent!!!" );
//          }
//        }
//      }
    //    }
    #endregion DEBUG
  }
}