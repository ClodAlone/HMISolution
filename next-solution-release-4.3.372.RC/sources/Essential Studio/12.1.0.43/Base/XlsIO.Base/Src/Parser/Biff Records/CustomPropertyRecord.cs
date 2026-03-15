#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.IO;
using System.Text;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for CustomPropertyRecord.
  /// </summary>
  [ Biff( TBIFFRecord.CustomProperty ) ]
  [ CLSCompliant( false ) ]
  public class CustomPropertyRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Size of fixed data.
    /// </summary>
    private const int DEF_FIXED_SIZE = 2 + 4 + 1;// size of unknown + size of data length + size of name length
    /// <summary>
    /// Unknown record header.
    /// </summary>
    private static readonly byte[] DEF_HEADER = new byte[ 2 ] { 0, 0x10 };
    /// <summary>
    /// Max length of name string.
    /// </summary>
    private const int DEF_MAX_NAME_LENGTH = byte.MaxValue;
    #endregion

    #region Class members
    /// <summary>
    /// Property name.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Property value.
    /// </summary>
    private string m_strValue;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  CustomPropertyRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CustomPropertyRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CustomPropertyRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets property name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty." );

        if( value.Length > DEF_MAX_NAME_LENGTH )
          throw new ArgumentException( "value - string is too long." );

        m_strName = value;
      }
    }
    /// <summary>
    /// Gets / sets property value.
    /// </summary>
    public string Value
    {
      get
      {
        return m_strValue;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty." );

        m_strValue = value;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      // Skip unknown header 00 10
      iOffset += 2;

      int iDataSize = provider.ReadInt32( iOffset );
      iOffset += 4;

      int iBytes = provider.ReadByte( iOffset );
      iOffset++;
     
      m_strName = provider.ReadString( iOffset, iBytes, Encoding.UTF8, true );

      iOffset += iBytes;
      m_strValue = provider.ReadString( iOffset, iDataSize, Encoding.Unicode, true );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version  );
      //m_data = new byte[ m_iLength ];

      //DEF_HEADER.CopyTo( m_data, 0 );
      provider.WriteBytes( iOffset, DEF_HEADER, 0, DEF_HEADER.Length );
      iOffset += DEF_HEADER.Length;

      int iValueLengthOffset = iOffset;
      iOffset += 4;
      iOffset++;

      byte[] arrData = Encoding.UTF8.GetBytes(m_strName);
      //arrData.CopyTo( m_data, iOffset );
      int iLength = arrData.Length;
      provider.WriteBytes( iOffset, arrData, 0, iLength );
      provider.WriteByte( iValueLengthOffset + 4, ( byte )iLength );
      //m_data[ iValueLengthOffset + 4 ] = ( byte )iLength;
      iOffset += iLength;

      arrData = Encoding.Unicode.GetBytes( m_strValue );
      //arrData.CopyTo( m_data, iOffset );
      iLength = arrData.Length;
      provider.WriteBytes( iOffset, arrData, 0, iLength );
      provider.WriteInt32( iValueLengthOffset, iLength );
    }
    /// <summary>
    /// Returns size of the required data array.
    /// </summary>
    /// <returns>Size of the required data array.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_SIZE + Encoding.UTF8.GetByteCount( m_strName )
        + Encoding.Unicode.GetByteCount( m_strValue );
    }
    #endregion
  }
}
