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
  /// The base implementation for style class
  /// </summary>
  public abstract class Style
    : XDLSSerializableBase,
      IStyle
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private string m_strName;
    /// <summary>
    /// 
    /// </summary>
    internal protected IStyle m_baseStyle = null;
    /// <summary>
    /// 
    /// </summary>
    internal protected string m_baseStyleName;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets style name
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null || value.Length == 0 )
        {
          throw new ArgumentNullException( "Name" );
        }
        if( Document.Styles.FindByName( "value" ) != null )
        {
          throw new ArgumentException( "Name of style already exists" );
        }
        m_strName = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    public Style( IDocument doc )
      : base( doc )
    {
      m_strName = "Style" + doc.Styles.Count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <param name="doc"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal  Style( IStyle style, IDocument doc )
      : this( doc )
    {
      m_strName = style.Name; 
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Clones itself
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public abstract IStyle Clone( IDocument document );
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      writer.WriteValue( XDLSConstants.StyleNameAttr, Name );
      if( m_baseStyle != null )
      {
        writer.WriteValue( XDLSConstants.StyleBaseNameAttr, m_baseStyle.Name );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      m_strName = reader.ReadString( XDLSConstants.StyleNameAttr );
      
      if( reader.HasAttribute( XDLSConstants.StyleBaseNameAttr ))
      {
        m_baseStyleName = reader.ReadString( XDLSConstants.StyleBaseNameAttr );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddRefElement( XDLSConstants.StyleBaseTag, m_baseStyle );
    }
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    protected override void DBG_WXC()
    {
      base.DBG_WXC();
    }
#endif
    #endregion
  }
}
