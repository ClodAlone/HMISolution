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
using Syncfusion.DLS.XML;

#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// 
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class XDLSSerializableHelper
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private XDLSHolder m_XDLSHolder = null;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public XDLSHolder Holder
    {
      get
      {
        return m_XDLSHolder;
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    public bool CheckHolderEmpty()
    {
      if( m_XDLSHolder == null )
      {
        m_XDLSHolder = new XDLSHolder();
      }

      if( m_XDLSHolder.Cleared )
      {
        m_XDLSHolder.Cleared = false;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public abstract class XDLSSerializableBase
    : EntityBase, 
    IXDLSSerializable
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private XDLSHolder m_XDLSHolder;
    /// <summary>
    /// 
    /// </summary>
    protected int m_id;
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    protected XDLSSerializableBase()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    protected XDLSSerializableBase( IDocument doc )
      : base(doc)
    {
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    void IXDLSSerializable.WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      WriteXmlAttributes( writer );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    void IXDLSSerializable.WriteXmlContent( IXDLSContentWriter writer )
    {
#if DEBUG    
      DBG_WXC();
#endif
      XDLSHolder.WriteHolder( writer );
      WriteXmlContent( writer );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    void IXDLSSerializable.ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      ReadXmlAttributes( reader );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    bool IXDLSSerializable.ReadXmlContent( IXDLSContentReader reader )
    {
      if( !XDLSHolder.ReadHolder( reader ) )
      {
        return ReadXmlContent( reader );
      }

      return true;
    }
    /// <summary>
    /// 
    /// </summary>
    XDLSHolder IXDLSSerializable.XDLSHolder
    {
      get
      {
        if( m_XDLSHolder == null )
        {
          m_XDLSHolder = new XDLSHolder();
        }

        if( m_XDLSHolder.Cleared )
        {
          m_XDLSHolder.Cleared = false;
          InitXDLSHolder();
        }

        return m_XDLSHolder;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void IXDLSSerializable.RestoreReference( string name, int value )
    {
      //if( value > -1 )
      //{
        RestoreReference( name, value );
      //}
    }
    /// <summary>
    /// 
    /// </summary>
    protected XDLSHolder XDLSHolder
    {
      get
      {
        return ( this as IXDLSSerializable ).XDLSHolder;
      }
    }
    #endregion

    #region Class virtual / abstract methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected virtual void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected virtual void WriteXmlContent( IXDLSContentWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected virtual void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected virtual bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );
      
      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void InitXDLSHolder()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    protected virtual void RestoreReference( string name, int index )
    {
    }
#if DEBUG
    /// <summary>
    /// 
    /// </summary>
    protected virtual void DBG_WXC()
    {
    }
#endif 
    #endregion
  }
}