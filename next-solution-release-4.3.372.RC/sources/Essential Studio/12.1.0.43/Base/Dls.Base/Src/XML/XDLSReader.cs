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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// Summary description for XDLSReader.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class XDLSReader
    : IXDLSAttributeReader,
      IXDLSContentReader
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private XmlReader m_reader;
    private XDLSCustomRW m_customRW = new XDLSCustomRW();
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public XDLSReader( XmlReader reader )
    {
      m_reader = reader;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Deserialize( IXDLSSerializable value )
    {
      while( m_reader.NodeType != XmlNodeType.Element )
      {
        m_reader.Read();
      }

      ReadElement( value );
      value.XDLSHolder.AfterDeserialization( value );
    }
    #endregion

    #region IXDLSAttributeReader implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasAttribute( string name )
    {
      return ( m_reader.GetAttribute( name ) != null );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string ReadString( string name )
    {
      return m_reader.GetAttribute( name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int ReadInt( string name )
    {
      return XmlConvert.ToInt32( m_reader.GetAttribute( name ) );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public short ReadShort( string name )
    {
      return XmlConvert.ToInt16( m_reader.GetAttribute( name ) );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public float ReadFloat( string name )
    {
      return XmlConvert.ToSingle( m_reader.GetAttribute( name ) );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ReadBoolean( string name )
    {
      string s = m_reader.GetAttribute( name );
      return XmlConvert.ToBoolean( s );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public byte ReadByte( string name )
    {
      string s = m_reader.GetAttribute( name );
      return XmlConvert.ToByte( s );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public Enum ReadEnum( string name, Type enumType )
    {
      string s = m_reader.GetAttribute( name );
      return ( Enum )Enum.Parse( enumType, /*m_reader.GetAttribute( name )*/s );
    }
    /// <summary>
    /// Reads color from XML.
    /// </summary>
    /// <param name="name">Name of attribute.</param>
    /// <returns>Color structure.</returns>
    public Color ReadColor( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );
      if( name.Length == 0 )
        throw new ArgumentException( "name - string can not be empty" );

      string value = m_reader.GetAttribute( name );
      Color color = ColorTranslator.FromHtml( value );

      return color;
    }
    /// <summary>
    /// Reads color from XML.
    /// </summary>
    /// <param name="name">Name of attribute.</param>
    /// <returns>Color structure.</returns>
    public DateTime ReadDateTime( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );
      if( name.Length == 0 )
        throw new ArgumentException( "name - string can not be empty" );

      string value = m_reader.GetAttribute( name );
      DateTime time;
#if !SyncfusionFramework2_0
      time = XmlConvert.ToDateTime( value );
#else
      time = XmlConvert.ToDateTime( value, XmlDateTimeSerializationMode.Utc );
#endif

      return time;
    }
    #endregion

    #region IXDLSContentReader implement
    /// <summary>
    /// 
    /// </summary>
    public string TagName
    {
      get
      {
        return m_reader.LocalName;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public XmlNodeType NodeType
    {
      get
      {
        return m_reader.NodeType;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetAttributeValue( string name )
    {
      return m_reader.GetAttribute( name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ReadChildElement( object value )
    {
      IXDLSSerializable dlsSer = value as IXDLSSerializable;

      if( dlsSer != null )
      {
        ReadElement( dlsSer );
      }
      else
      {
        IXDLSSerializableCollection dlsSerColl =
          value as IXDLSSerializableCollection;

        if( dlsSerColl != null )
        {
          ReadElementCollection( dlsSerColl );
        }
        else
        {
          // We don't move to next element 
          return false;
        }
      }

      // We moved to next element
      return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object ReadChildElement( Type type )
    {
      object value = m_customRW.Read( m_reader, type );
      
      return value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ReadChildStringContent()
    {
      return m_reader.ReadString();
    }
    /// <summary>
    /// Reads binary value.
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    public byte[] ReadChildBinaryElement()
    {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      XmlTextReader reader = ( XmlTextReader )m_reader;
#elif SyncfusionFramework2_0
      XmlReader reader = m_reader;
#endif

      int base64len = 0;
      byte[] resData = new byte[ 0 ];
      byte[] base64 = new byte[ 1000 ];

      do
      {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
        base64len = reader.ReadBase64( base64, 0, base64.Length );
#elif SyncfusionFramework2_0        
        base64len = reader.ReadElementContentAsBase64( base64, 0, base64.Length );
#endif
        // Expands resData and copy to resData new portion from 
        // Base64 stream
        byte[] newData = new byte[ resData.Length + base64len ];
        resData.CopyTo( newData, 0 );
        Array.Copy( base64, 0, newData, resData.Length, base64len );
        resData = newData;

        if( base64len < base64.Length )
        {
          break;
        }
        else
        {
          base64 = new byte[ resData.Length * 2 ]; 
        }
      }
      while( !reader.EOF );

      return resData;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Image ReadImage()
    {
      return ReadImage( false );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Image ReadImage( bool isMetafile )
    {
      byte[] buf = ReadChildBinaryElement();
      Image image = null;
      if( buf.Length > 0 )
      {
        MemoryStream memStream = new MemoryStream( buf );
        if( isMetafile )
        {
          image = new Metafile( memStream );
        }
        else
        {
          image = new Bitmap( memStream );
        }
      }
      return image;
    }
    /// <summary>
    /// 
    /// </summary>
    public XmlReader InnerReader
    {
      get
      {
        return m_reader;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void ReadElement( IXDLSSerializable value )
    {
      if( value == null )
      {
        m_reader.Skip();
        return;
      }

      if( m_reader.HasAttributes )
      {
        if( m_reader.MoveToAttribute( "id" ) )
        {
          value.XDLSHolder.ID = XmlConvert.ToInt32( m_reader.GetAttribute( "id" ) );
        }
        value.ReadXmlAttributes( this );
        m_reader.MoveToElement();
      }

      bool hasChildElements = !m_reader.IsEmptyElement;
      int currDepth = m_reader.Depth;
      m_reader.ReadStartElement();
      
      if( hasChildElements )
      {
        while( m_reader.Depth > currDepth && !m_reader.EOF )
        {
          // Skips if node is not Element type
          if( m_reader.NodeType != XmlNodeType.Element )
          {
            m_reader.Read();
            continue;
          }

          if( !value.ReadXmlContent( this ) )
          {
            m_reader.Skip();
          }
        }
        
        m_reader.ReadEndElement();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="coll"></param>
    private void ReadElementCollection( IXDLSSerializableCollection coll )
    {
      bool hasChildElements = !m_reader.IsEmptyElement;
      int currDepth = m_reader.Depth;
      m_reader.ReadStartElement();

      if( hasChildElements )
      {
        while( m_reader.Depth > currDepth && !m_reader.EOF )
        {
          // Skips if node is not Element type
          if( m_reader.NodeType != XmlNodeType.Element )
          {
            m_reader.Read();
            continue;
          }

          if( m_reader.LocalName == coll.TagItemName )
          {
            IXDLSSerializable newItem = coll.AddNewItem( this );
            ReadElement( newItem );
          }
        }
        
        m_reader.ReadEndElement();
      }
    }
    #endregion
  }
}
