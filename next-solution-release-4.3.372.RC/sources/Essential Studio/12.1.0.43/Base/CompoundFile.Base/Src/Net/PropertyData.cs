#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

#if DOCIO
using Syncfusion.CompoundFile.DocIO.Native;
#if (SILVERLIGHT) && !(WINRT ) 
using Syncfusion.DocIO.Implementation.Silverlight;
#elif WP
using Syncfusion.DocIO.Implementation.WP;
#endif
#endif

#if !DOCIO

using Syncfusion.CompoundFile.XlsIO.Native;

#if (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#endif

#endif


#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  public class PropertyData : IPropertyData, IComparable
  {
    #region Consts
    /// <summary>
    /// Bit mask for LinkToContent property of the DocumentProperty class. 
    /// </summary>
    private const int LinkBit = 0x1000000;
    private const int NamesDictionaryId = 0;
    #endregion

    #region Member
    private int m_iId;
    private string m_strName;
    public PropertyType PropertyType;
    public object Data;
    #endregion

    #region Properties
    /// <summary>
    /// Indicates whether it is property or just link to source of some property. Read-only.
    /// </summary>
    public bool IsLinkToSource
    {
      get
      {
        return ( ( Id  & LinkBit ) != 0 );
      }
    }
    /// <summary>
    /// Returns id of the parent property. Read-only.
    /// </summary>
    public int ParentId
    {
      get
      {
        return ( int )( IsLinkToSource
          ? Id - LinkBit
          : Id );
      }
    }
    /// <summary>
    /// GEts or sets property id.
    /// </summary>
    public int Id
    {
      get
      {
        return m_iId;
      }
      set
      {
        m_iId = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new isntance of the PropertyData.
    /// </summary>
    internal PropertyData()
    {
    }
    /// <summary>
    /// Initializes new instance of the property data.
    /// </summary>
    /// <param name="id">Id for the new property..</param>
    public PropertyData( int id )
    {
      Id = id;
    }
    /// <summary>
    /// Extracts property data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public void Parse( Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 4 ];
      PropertyType = ( PropertyType )StreamHelper.ReadInt32( stream, buffer );

      if( ( PropertyType & PropertyType.Vector ) != 0 )
      {
          Data = ParseVector(stream, roundedSize);
      }
      else
      {
        Data = ParseSingleValue( PropertyType, stream, roundedSize );
      }

      //if( ( PropertyType & PropertyType.AsciiString ) == PropertyType.AsciiString )
      //{
      //  PropertyType &= ~PropertyType.AsciiString;
      //  PropertyType |= PropertyType.String;
      //}
    }
    /// <summary>
    /// Extracts vector data from the stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="roundedSize"></param>
    /// <returns></returns>
    private IList ParseVector( Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 4 ];

      // Number of items in the stream.
      int iCount = StreamHelper.ReadInt32( stream, buffer );
      //object[] arrResult = new object[ iCount ];

      PropertyType itemType = PropertyType & ~PropertyType.Vector;
      IList arrResult = CreateArray( itemType, iCount );

      for( int i = 0; i < iCount; i++ )
      {
        arrResult[ i ] = ParseSingleValue( itemType, stream, roundedSize - 4 );
      }

      return arrResult;
    }
    /// <summary>
    /// Creates array of the specified type.
    /// </summary>
    /// <param name="itemType">Item type.</param>
    /// <param name="count">Number of elements in the array.</param>
    /// <returns>Created array object.</returns>
    private IList CreateArray( PropertyType itemType, int count )
    {
      switch( itemType )
      {
        case PropertyType.AsciiString:
        case PropertyType.String:
          return new string[ count ];

        case PropertyType.Int32:
        case PropertyType.Int:
          return new int[ count ];

        case PropertyType.Object:
        default:
          return new object[ count ];
      }
    }
    /// <summary>
    /// Extracts single value from the stream.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="stream"></param>
    /// <param name="roundedSize"></param>
    /// <returns></returns>
    private object ParseSingleValue( PropertyType itemType, Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 8 ];
      object result = null;

      switch( itemType )
      {
        case PropertyType.Bool:
          result = StreamHelper.ReadInt32( stream, buffer ) != 0;
          break;

        case PropertyType.Blob:
          result = GetBlob( stream, buffer );
          break;

        case PropertyType.ClipboardData:
          result = GetClipboardData( stream, buffer );
          break;

        case PropertyType.DateTime:
          result = GetDateTime( stream, buffer );
          break;

        case PropertyType.Double:
          result = StreamHelper.ReadDouble( stream, buffer );
          break;

        case PropertyType.Int:
        case PropertyType.Int32:
          result = StreamHelper.ReadInt32( stream, buffer );
          break;

        case PropertyType.UInt32:
          result = ( uint )StreamHelper.ReadInt16( stream, buffer );
          break;

        case PropertyType.Int16:
          result = StreamHelper.ReadInt16( stream, buffer );
          stream.Position += 2;
          break;

        case PropertyType.AsciiString:
          result = StreamHelper.GetAsciiString( stream, roundedSize - 4 );
          break;

        case PropertyType.String:
          result = StreamHelper.GetUnicodeString( stream, roundedSize - 4 );
          break;

        case PropertyType.Empty:
        case PropertyType.Null:
          result = null;
          break;

        case PropertyType.Object:
          result = GetObject( stream, roundedSize - 4 );
          break;

        default:
          throw new NotImplementedException();
      }

      return result;
    }
    /// <summary>
    /// Gets DateTime object data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Buffer used to extract blob parts.</param>
    /// <returns>Extracted DateTime data.</returns>
    private object GetDateTime( Stream stream, byte[] buffer )
    {
      //System.Runtime.InteropServices.ComTypes.FILETIME fileTime = new System.Runtime.InteropServices.ComTypes.FILETIME();
      //fileTime.dwLowDateTime = StreamHelper.ReadInt32( stream, buffer );
      //fileTime.dwHighDateTime = StreamHelper.ReadInt32( stream, buffer );

      stream.Read( buffer, 0, 8 );
      long lTicksCount = BitConverter.ToInt64( buffer, 0 ) + PropVariant.DEF_FILETIME_TICKS_DIFFERENCE;
      DateTime date = new DateTime( lTicksCount );

      if( Id != ( int )PIDSI.EditTime )
        date = date.ToLocalTime();

      return date;
    }
    /// <summary>
    /// Gets blob data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get blob data from.</param>
    /// <param name="buffer">Buffer used to extract blob parts.</param>
    /// <returns>Blob data.</returns>
    private object GetBlob( Stream stream, byte[] buffer )
    {
      int iSize = StreamHelper.ReadInt32( stream, buffer );
      byte[] arrResult = new byte[ iSize ];

      if( stream.Read( arrResult, 0, iSize ) != iSize )
        throw new Exception();

      return arrResult;
    }
    /// <summary>
    /// Gets clipboard data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="buffer">Buffer that can be used to extract clipboard data parts.</param>
    /// <returns>Clipboard data.</returns>
    private object GetClipboardData( Stream stream, byte[] buffer )
    {
      ClipboardData result = new ClipboardData();
      result.Parse( stream );
      return result;
    }
    /// <summary>
    /// Gets object value from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="roundedSize">Maximum size of the data to extract.</param>
    /// <returns>Extracted object.</returns>
    private object GetObject( Stream stream, int roundedSize )
    {
      byte[] buffer = new byte[ 4 ];
      PropertyType itemType = ( PropertyType )StreamHelper.ReadInt32( stream, buffer );
      return ParseSingleValue( itemType, stream, roundedSize - 4 );
    }
    /// <summary>
    /// Writes object into the stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <param name="value">Object to write.</param>
    /// <returns>Size of the written data.</returns>
    private int WriteObject( Stream stream, object value )
    {
      PropertyType valueType;

      if( value is int )
      {
        valueType = PropertyType.Int32;
      }
      else if( value is double )
      {
        valueType = PropertyType.Double;
      }
      else if( value is bool )
      {
        valueType = PropertyType.Bool;
      }
      else if( value is string )
      {
        valueType = PropertyType.String;
      }
      else
      {
        throw new NotImplementedException();
      }

      StreamHelper.WriteInt32( stream, ( int )valueType );
      return SerializeSingleValue( stream, value, valueType ) + StreamHelper.IntSize;
    }
    /// <summary>
    /// Writes property data into the stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <returns>Number of written bytes.</returns>
    public int Serialize( Stream stream )
    {
      byte[] buffer = new byte[ 8 ];
      int iWrittenSize = StreamHelper.WriteInt32( stream, ( int )PropertyType );

      if( ( PropertyType & PropertyType.Vector ) == PropertyType.Vector )
      {
        iWrittenSize += SerializeVector( stream, ( IList )Data );
      }
      else if( Id == NamesDictionaryId )
      {
        stream.Position -= StreamHelper.IntSize;
        iWrittenSize += SerializeDictionary( stream, ( Dictionary<int, string> )Data );
      }
      else
      {
        iWrittenSize += SerializeSingleValue( stream, Data, PropertyType );
      }

      if( PropertyType != PropertyType.AsciiString )
        StreamHelper.AddPadding( stream, ref iWrittenSize );

      return iWrittenSize;
    }
    /// <summary>
    /// Serializes names dictionary.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="dictionary">Dictionary to serialize.</param>
    /// <returns>Size of the serialized data.</returns>
    private int SerializeDictionary( Stream stream, Dictionary<int, string> dictionary )
    {
      int iWrittenData = 0;
      int iCount = dictionary.Count;
      iWrittenData += StreamHelper.WriteInt32( stream, iCount );

      foreach( KeyValuePair<int, string> pair in dictionary )
      {
        iWrittenData += StreamHelper.WriteInt32( stream, pair.Key );
        iWrittenData += StreamHelper.WriteAsciiString( stream, pair.Value, false );
      }

      return iWrittenData;
    }
    /// <summary>
    /// Serializes vector data.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private int SerializeVector( Stream stream, IList data )
    {
      int iCount = data.Count;
      StreamHelper.WriteInt32( stream, iCount );
      int iWrittenData = StreamHelper.IntSize;
      PropertyType itemType = PropertyType & ~PropertyType.Vector;

      for( int i = 0; i < iCount; i++ )
      {
        iWrittenData += SerializeSingleValue( stream, data[ i ], itemType );
      }

      return iWrittenData;
    }
    /// <summary>
    /// Serializes single value into the stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="value">Value to serialize.</param>
    /// <param name="valueType">Value type.</param>
    /// <returns>Size of the written data.</returns>
    private int SerializeSingleValue( Stream stream, object value, PropertyType valueType )
    {
      int iWrittenSize = 0;

      switch( valueType )
      {
        case PropertyType.Bool:
          bool bValue = ( bool )value;
          iWrittenSize += StreamHelper.WriteInt32( stream, bValue ? 1 : 0 );
          break;

        case PropertyType.Blob:
          byte[] arrValue = ( byte[] )value;
          iWrittenSize += SerializeBlob( stream, arrValue );
          break;

        case PropertyType.ClipboardData:
          ClipboardData data = ( ClipboardData )value;
          iWrittenSize += SerializeClipboardData( stream, data );
          break;

        case PropertyType.DateTime:
            DateTime date;
#if DOCIO
            if(value is TimeSpan)
            {
                TimeSpan timeSpan = (TimeSpan)value;
                date = DateTime.FromBinary(timeSpan.Ticks);
            }
            else
#endif
                date = ( DateTime )value;

          if( Id != ( int )PIDSI.EditTime )
            date = date.ToUniversalTime();

          ulong ulFileTime = ( ulong )( date.Ticks - PropVariant.DEF_FILETIME_TICKS_DIFFERENCE );
          byte[] buffer = BitConverter.GetBytes( ulFileTime );
          stream.Write( buffer, 0, buffer.Length );
          iWrittenSize += buffer.Length;
          break;

        case PropertyType.Double:
          iWrittenSize += StreamHelper.WriteDouble( stream, ( double )value );
          break;

        case PropertyType.Int:
        case PropertyType.Int32:
          iWrittenSize += StreamHelper.WriteInt32( stream, ( int )value );
          break;

        case PropertyType.UInt32:
          iWrittenSize += StreamHelper.WriteInt32( stream, ( int )( uint )value );
          break;

        case PropertyType.Int16:
          iWrittenSize += StreamHelper.WriteInt16( stream, ( short )value );
          break;

        case PropertyType.AsciiString:
          iWrittenSize += StreamHelper.WriteAsciiString( stream, ( string )value, false );
          break;

        case PropertyType.String:
          iWrittenSize += StreamHelper.WriteUnicodeString( stream, ( string )value );
          break;

        case PropertyType.Null:
        case PropertyType.Empty:
          break;

        case PropertyType.Object:
          iWrittenSize += WriteObject( stream, value );
          break;

        default:
          throw new NotImplementedException();
      }

      return iWrittenSize;
    }

    private int SerializeClipboardData( Stream stream, ClipboardData data )
    {
      return data.Serialize( stream );
    }
    /// <summary>
    /// Serializes blob data.
    /// </summary>
    /// <param name="stream">Stream to write blob into.</param>
    /// <param name="value">Blob value to write.</param>
    /// <returns>Size of the written data.</returns>
    private int SerializeBlob( Stream stream, byte[] value )
    {
      int iWrittenSize = 0;
      int iSize = value.Length;
      iWrittenSize += StreamHelper.WriteInt32( stream, iSize );
      stream.Write( value, 0, iSize );
      iWrittenSize += iSize;

      return iWrittenSize;
    }
    /// <summary>
    /// Sets property value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="type">Type of the property to set.</param>
    public bool SetValue( object value, PropertyType type )
    {
      bool result = false;

      switch( type )
      {
        case PropertyType.Bool:
        case PropertyType.Int:
        case PropertyType.Int32:
        case PropertyType.Int16:
        case PropertyType.UInt32:
        case PropertyType.String:
        case PropertyType.AsciiString:
        case PropertyType.DateTime:
        case PropertyType.Blob:
        case PropertyType.Vector:
        case PropertyType.Object:
        case PropertyType.Double:
        case PropertyType.Empty:
        case PropertyType.Null:
        case PropertyType.AsciiStringArray:
        case PropertyType.StringArray:
        case PropertyType.ObjectArray:
        case PropertyType.ClipboardData:
          Value = value;
          Type = ( VarEnum )type;
          result = true;
          break;
      }

      return result;
    }
    #endregion

    #region IPropertyData Members

    public object Value
    {
      get
      {
        return Data;
      }
      set
      {
        Data = value;
      }
    }
    public VarEnum Type
    {
      get
      {
        return ( VarEnum )PropertyType;
      }
      set
      {
        PropertyType = ( PropertyType )value;
      }
    }
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    #endregion

    #region IComparable Members

    public int CompareTo( object obj )
    {
      PropertyData data = ( PropertyData )obj;

      return Id - data.Id;
    }

    #endregion
  }
}
