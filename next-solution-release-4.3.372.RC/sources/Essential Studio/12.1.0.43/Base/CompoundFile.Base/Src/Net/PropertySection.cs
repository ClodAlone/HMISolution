#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  public class PropertySection
  {
    #region Constants
    private const int PropertyNamesId = int.MinValue;
    private const short UnicodeCodePage = 0x4B0;
    #endregion

    #region Members
    /// <summary>
    /// Offset to the section header.
    /// </summary>
    private int m_iOffset;
    /// <summary>
    /// Section id.
    /// </summary>
    private Guid m_id;
    /// <summary>
    /// Section size.
    /// </summary>
    private int m_iLength;
    /// <summary>
    /// Contains all section properties.
    /// </summary>
    private List<PropertyData> m_lstProperties = new List<PropertyData>();
    /// <summary>
    /// Code page of the section.
    /// </summary>
    private short m_sCodePage = -1;
    private DictionaryInfo m_dictionaryInfo;

    private class DictionaryInfo
    {
      public long StreamOffset;
      public int DataSize;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Offset to the section header.
    /// </summary>
    public int Offset
    {
      get
      {
        return m_iOffset;
      }
      set
      {
        m_iOffset = value;
      }
    }
    /// <summary>
    /// Section id.
    /// </summary>
    public Guid Id
    {
      get
      {
        return m_id;
      }
      set
      {
        m_id = value;
      }
    }
    /// <summary>
    /// Section size.
    /// </summary>
    public int Length
    {
      get
      {
        return m_iLength;
      }
      set
      {
        m_iLength = value;
      }
    }
    /// <summary>
    /// Properties count.
    /// </summary>
    public int Count
    {
      get
      {
        return m_lstProperties.Count;
      }
    }
    /// <summary>
    /// Gets list of all properties.
    /// </summary>
    public List<PropertyData> Properties
    {
      get
      {
        return m_lstProperties;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the section.
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="sectionOffset"></param>
    public PropertySection( Guid guid, int sectionOffset )
    {
      m_id = guid;
      m_iOffset = sectionOffset;
    }
    /// <summary>
    /// Extracts properties from the stream.
    /// </summary>
    /// <param name="stream">Stream to get section from.</param>
    public void Parse( Stream stream )
    {
      byte[] buffer = new byte[ 4 ];

      stream.Position = m_iOffset;
      m_iLength = StreamHelper.ReadInt32( stream, buffer );
      int iCount = StreamHelper.ReadInt32( stream, buffer );
      List<int> lstOffsets = new List<int>();

      for( int i = 0; i < iCount; i++ )
      {
        int id = StreamHelper.ReadInt32( stream, buffer );
        int iOffset = StreamHelper.ReadInt32( stream, buffer );

        m_lstProperties.Add( new PropertyData( id ) );
        lstOffsets.Add( iOffset );
      }

      lstOffsets.Add( ( int )stream.Length );
      Dictionary<int, string> dictNames = null;

      for( int i = 0; i < iCount; i++ )
      {
        PropertyData property = m_lstProperties[ i ];
        int iPropertyOffset = lstOffsets[ i ];
        int iNextPropertyOffset = lstOffsets[ i + 1 ];
        stream.Position = m_iOffset + lstOffsets[ i ];
        int iReservedSize = iNextPropertyOffset - iPropertyOffset;

        // This is some special kind of property and it should be visible for the user.
        if( property.Id < 2 )
        {
          ParseSpecialProperties( property, stream, iReservedSize, ref dictNames );
          m_lstProperties.RemoveAt( i );
          lstOffsets.RemoveAt( i );
          iCount--;
          i--;
        }
        else
        {
          ParseDictionary( stream, ref dictNames );

          property.Parse( stream, iReservedSize );
          string strName;

          if( dictNames  != null && dictNames.TryGetValue( property.Id, out strName ) )
            property.Name = strName;
        }
      }
    }
    /// <summary>
    /// Parses dictionary based on the internal variables value.
    /// </summary>
    /// <param name="stream">Stream to get dictionary from.</param>
    /// <param name="dictNames">Dictionary to fill if necessary.</param>
    private void ParseDictionary( Stream stream, ref Dictionary<int, string> dictNames )
    {
      if( m_dictionaryInfo != null )
      {
        dictNames = ParsePropertyNames( stream, m_dictionaryInfo );
        m_dictionaryInfo = null;
      }
    }
    /// <summary>
    /// Parses property names.
    /// </summary>
    /// <param name="stream">Stream to get property data from.</param>
    /// <param name="dictionaryInfo">Information about dictionary placement inside stream.</param>
    /// <returns>Parsed dictionary.</returns>
    private Dictionary<int, string> ParsePropertyNames( Stream stream, DictionaryInfo dictionaryInfo )
    {
      long currentPos = stream.Position;
      stream.Position = dictionaryInfo.StreamOffset;
      Dictionary<int, string> result = ParsePropertyNames( stream );
      stream.Position = currentPos;

      return result;
    }
    /// <summary>
    /// Parses special properties (they shouldn't be visible to user and they contain some information).
    /// </summary>
    /// <param name="property"></param>
    /// <param name="stream"></param>
    /// <param name="reservedSize"></param>
    /// <param name="dictNames"></param>
    private void ParseSpecialProperties( PropertyData property, Stream stream, int reservedSize,
      ref Dictionary<int, string> dictNames )
    {
      switch( property.Id )
      {
        case 0:
          // This property contains property names.
          //dictNames = ParsePropertyNames( stream );
          m_dictionaryInfo = new DictionaryInfo();
          m_dictionaryInfo.StreamOffset = stream.Position;
          m_dictionaryInfo.DataSize = reservedSize;
          stream.Position += reservedSize;
          break;

        default:
          property.Parse( stream, reservedSize );

          if( property.Id == 1 )
          {
            m_sCodePage = ( short )property.Value;
            ParseDictionary( stream, ref dictNames );
          }
          break;
      }
    }
    /// <summary>
    /// Parses property names based on previously stored internal information.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <returns>Parsed dictionary.</returns>
    private Dictionary<int, string> ParsePropertyNames( Stream stream )
    {
      byte[] buffer = new byte[ 4 ];
      int iCount = StreamHelper.ReadInt32( stream, buffer );
      Dictionary<int, string> result = new Dictionary<int,string>();

      for( int i = 0; i < iCount; i++ )
      {
        int index = StreamHelper.ReadInt32( stream, buffer );

        string propertyName = ( m_sCodePage != UnicodeCodePage ) ?
          StreamHelper.GetAsciiString( stream, -1 ) :
          StreamHelper.GetUnicodeString( stream, -1 );

        result.Add( index, propertyName );
      }

      return result;
    }
    /// <summary>
    /// Saves section into stream.
    /// </summary>
    /// <param name="stream">Stream to save section into.</param>
    public void Serialize( Stream stream )
    {
      byte[] buffer = new byte[ 4 ];

      m_iOffset = ( int )stream.Position;
      // reserve data for complete data length.
      /*m_iLength = */StreamHelper.WriteInt32( stream, 0 );

      Dictionary<int, string> dictNames = PrepareNames();

      PropertyData codepage = new PropertyData( 1 );
      codepage.PropertyType = PropertyType.Int16;
      codepage.Value = ( dictNames.Count == 0 ) ? ( short )1251 : ( short )-535;

      //if( dictNames.Count == 0 )
      //{
      //  m_lstProperties.Insert( 0, codepage );
      //}
      //else
      //{
      //  m_lstProperties.Insert( 1, codepage );
      //}

      //m_lstProperties.Sort();

      for( int i = m_lstProperties.Count - 1; i >= 0; i-- )
      {
        PropertyData propData = m_lstProperties[ i ];

        //if( ( propData.PropertyType & ( PropertyType.Vector | PropertyType.Object ) ) ==
        //  ( PropertyType.Vector | PropertyType.Object ) )
        //  m_lstProperties.RemoveAt( i );
      }

      int iCount = m_lstProperties.Count;
      StreamHelper.WriteInt32( stream, iCount );

      // reserve data for offsets and id's
      stream.Position += iCount * StreamHelper.IntSize * 2;
      List<int> lstOffsets = new List<int>();

      //for( int i = 0; i < iCount; i++ )
      //{
      //  int id = StreamHelper.ReadInt32( stream, buffer );
      //  int iOffset = StreamHelper.ReadInt32( stream, buffer );

      //  m_lstProperties.Add( new PropertyData( id ) );
      //  lstOffsets.Add( iOffset );
      //}

      //lstOffsets.Add( ( int )stream.Length );

      for( int i = 0; i < iCount; i++ )
      {
        PropertyData property = m_lstProperties[ i ];
        //int iPropertyOffset = lstOffsets[ i ];
        //int iNextPropertyOffset = lstOffsets[ i + 1 ];
        //stream.Position = m_iOffset + lstOffsets[ i ];
        lstOffsets.Add( ( int )stream.Position );
        property.Serialize( stream );
      }

      //StreamHelper.AddPadding( stream, stream.Position );

      // Store current stream position before updating previously preserved space.
      long lCurrentPosition = stream.Position;
      stream.Position = m_iOffset + StreamHelper.IntSize * 2;

      // Write offsets table.
      for( int i = 0, len = lstOffsets.Count; i < len; i++ )
      {
        int iOffset = lstOffsets[ i ] - m_iOffset;
        StreamHelper.WriteInt32( stream, m_lstProperties[ i ].Id );
        StreamHelper.WriteInt32( stream, iOffset );
      }

      // Update length and restore stream position.
      m_iLength = ( int )( lCurrentPosition - m_iOffset );
      stream.Position = m_iOffset;
      StreamHelper.WriteInt32( stream, m_iLength );
      stream.Position = lCurrentPosition;
    }
    /// <summary>
    /// Prepares property name dictionary.
    /// </summary>
    /// <returns>Dictionary with property names. Key - property id, value - property name.</returns>
    private Dictionary<int, string> PrepareNames()
    {
      Dictionary<int, string> result = new Dictionary<int, string>();

      for( int i = 0, len = m_lstProperties.Count; i < len; i++ )
      {
        PropertyData property = m_lstProperties[ i ];

        if( property.Name != null )
        {
          result.Add( property.Id, property.Name );
        }
      }

      if( result.Count > 0 )
      {
        PropertyData namesProperty = new PropertyData( 0 );
        namesProperty.Value = result;
        m_lstProperties.Insert( 0, namesProperty );
      }

      return result;
    }
    #endregion
  }
}
