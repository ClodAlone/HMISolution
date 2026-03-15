#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

#region file using directives
using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Interfaces;
#endif

#if SILVERLIGHT || WP
using Syncfusion.XlsIO.Interfaces;
using System.Windows.Media;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif ( WINRT )
using Syncfusion.XlsIO;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;

#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  ///<exclude/>
  /// <summary>
  ///
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  //[ CLSCompliant( false ) ]
  public abstract class BiffRecordRaw
    : ICloneable
    , IBiffStorage
  {
    #region Class constants
    /// <summary>
    /// Default size of the inner collections.
    /// </summary>
    private const int DEF_RESERVE_SIZE = 100;
    /// <summary>
    /// Maximum size of a record.
    /// </summary>
    public const int DEF_RECORD_MAX_SIZE = 8228 - 4;
    /// <summary>
    /// Maximum size of a record with header.
    /// </summary>
    public const int DEF_RECORD_MAX_SIZE_WITH_HADER = 8228;
    /// <summary>
    /// Size of the records header.
    /// </summary>
    public const int DEF_HEADER_SIZE = 4;
    /// <summary>
    /// Number of bits in byte.
    /// </summary>
    public const int DEF_BITS_IN_BYTE = 8;
    /// <summary>
    /// Number of bits in short.
    /// </summary>
    private const int DEF_BITS_IN_SHORT = 16;
    /// <summary>
    /// Number of bits in int.
    /// </summary>
    private const int DEF_BITS_IN_INT = 32;
    #endregion

    #region Class static members
    /// <summary>
    /// Used to optimize reflection typed extracting.
    /// TBiffRecord - to - ReflectionCachePair
    /// </summary>
    static protected Dictionary<int, SortedList<BiffRecordPosAttribute, FieldInfo>> m_ReflectCache =
      new Dictionary<int, SortedList<BiffRecordPosAttribute, FieldInfo>>( DEF_RESERVE_SIZE );
    /// <summary>
    /// 
    /// </summary>
    static private readonly Encoding s_latin1 =

#if !(SILVERLIGHT)
      Encoding.GetEncoding( "latin1" );
#else
      new Syncfusion.Compression.LatinEncoding();
      //Encoding.UTF8;
#endif
    /// <summary>
    /// Skips begin - end block and sub blocks.
    /// </summary>
    /// <param name="recordList">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    /// <returns>Returns index after skipping.</returns>
    static public int SkipBeginEndBlock( IList<BiffRecordRaw> recordList, int iPos )
    {
      BiffRecordRaw record = recordList[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );

      int iCount = 1;

      iPos++;

      while( iCount > 0 )
      {
        record = recordList[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          default:
            break;
        }

        iPos++;
      }

      return iPos;
    }
    #endregion

    #region Class members
    /// <summary>
    /// Code of the Biff record.
    /// </summary>
    protected int     m_iCode = -1;
    /// <summary>
    /// Length of the Biff record data.
    /// </summary>
    protected int     m_iLength = -1;
    /// <summary>
    /// Indicates whether m_data array need to be Infilled before usage.
    /// </summary>
    private bool m_bNeedInfill = true;
#if DEBUG
    /// <summary>
    /// Position of the Biff record in the stream.
    /// </summary>
    protected long    m_lStreamPosition;
#endif
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Will get record type if known;
    /// otherwise it will get TBIFFRecord.Unknown.
    /// </summary>
    public TBIFFRecord TypeCode
    {
      get
      {
        return ( TBIFFRecord )m_iCode;
      }
    }
    /// <summary>
    /// Read-only. Returns integer value which is the unique identifier
    /// of Biff record.
    /// </summary>
    public int RecordCode
    {
      get
      {
        return m_iCode;
      }
    }
    /// <summary>
    /// Gets / sets length of internal data array.
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
    /// Read-only. Returns record data.
    /// </summary>
    public virtual byte[] Data
    {
      get
      {
        m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );
        byte[] arrBuffer = new byte[ m_iLength ];
        ByteArrayDataProvider provider = new ByteArrayDataProvider( arrBuffer );
        InfillInternalData( provider, 0, ExcelVersion.Excel97to2003 );
        return arrBuffer;
      }
      set
      {
        if( value != null )
        {
          int iLength = value.Length;
          ParseStructure( new ByteArrayDataProvider( value ), 0, iLength , ExcelVersion.Excel97to2003 );
        }
      }
    }
    /// <summary>
    /// If True, the array will automatically grow when the offset limit 
    /// is reached. This is required when the real record size is not known for an
    /// Infill operation. Will throw exception on buffer offset overrun
    /// when set to False. Default value is False.
    /// </summary>
    public virtual bool AutoGrowData
    {
      get
      {
        throw new NotImplementedException();
        //return m_bAutoGrow;
      }
      set
      {
        throw new NotImplementedException();
        //m_bAutoGrow = value;
      }
    }

    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public virtual long StreamPos
    {
      get
      {
#if DEBUG
        return m_lStreamPosition;
#else
        return -1;
#endif
      }
      set
      {
#if DEBUG
        m_lStreamPosition = value;
#endif
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    virtual public int MinimumRecordSize
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    virtual public int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_MAX_SIZE;
      }
    }
    /// <summary>
    /// Maximum memory size for internal buffer.
    /// </summary>
    virtual public int MaximumMemorySize
    {
      get
      {
        return int.MaxValue;
      }
    }
    /// <summary>
    /// Indicates whether record needs to infill internal data array.
    /// </summary>
    public bool NeedInfill
    {
      get
      {
        return m_bNeedInfill;
      }
      set
      {
        m_bNeedInfill = value;
      }
    }
    /// <summary>
    /// Indicates whether record needs internal data array
    /// or if it can be cleaned. Read-only.
    /// </summary>
    public virtual bool NeedDataArray
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Indicates whether record allows shorter data. Read-only.
    /// </summary>
    public virtual bool IsAllowShortData
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Indicates whether record need decoding when file is encoded or not. Read-only.
    /// </summary>
    public virtual bool NeedDecoding
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Returns offset in the data array where encoded/decoded data should start. Read-only.
    /// </summary>
    public virtual int StartDecodingOffset
    {
      get
      {
        return 0;
      }
    }
    #endregion

    #region Class internal methods
    /// <summary>
    /// Gets value with all bits that do not correspond
    /// to the specified mask to zero.
    /// </summary>
    /// <param name="value">Unsigned Int16 value.</param>
    /// <param name="BitMask">Bit mask.</param>
    /// <returns>
    /// Value with all bits that do not correspond
    /// to the specified mask to zero.
    /// </returns>
    static internal ushort GetUInt16BitsByMask( ushort value, ushort BitMask )
    {
      return ( ushort )( value & BitMask );
    }
    /// <summary>
    /// Sets value with all bits that correspond to the specified
    /// mask of zero to the same values as in the value.
    /// </summary>
    /// <param name="destination">Variable that bits of which will be set.</param>
    /// <param name="BitMask">Bit mask.</param>
    /// <param name="value">Value from which bit values will be taken.</param>
    static internal void SetUInt16BitsByMask( ref ushort destination, ushort BitMask, ushort value )
    {
      destination &= (ushort) (~BitMask );
      destination += (ushort) ( value & BitMask );
    }
    /// <summary>
    /// Gets value with all bits that do not correspond
    /// to the specified mask to zero.
    /// </summary>
    /// <param name="value">Unsigned Int32 value.</param>
    /// <param name="BitMask">Bit mask.</param>
    /// <returns>
    /// Value with all bits that do not correspond
    /// to the specified mask to zero.
    /// </returns>
    static internal uint GetUInt32BitsByMask( uint value, uint BitMask )
    {
      return value & BitMask;
    }

    /// <summary>
    /// Sets value with all bits that correspond to the specified
    /// mask to zero to the same values as in value.
    /// </summary>
    /// <param name="destination">Variable that bits of which will be set.</param>
    /// <param name="BitMask">Bit mask.</param>
    /// <param name="value">Value from which bit values will be taken.</param>
    static internal void SetUInt32BitsByMask( ref uint destination, uint BitMask, uint value )
    {
      destination &= ~BitMask;
      destination += value & BitMask;
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, gets code value using reflection and attributes.
    /// </summary>
    protected  BiffRecordRaw()
    {
      Type type = GetType();
      object[] allBiffAttributes = type.GetCustomAttributes( typeof( BiffAttribute ), true );

      if( allBiffAttributes.Length != 0 )
      {
        BiffAttribute decl = ( BiffAttribute )allBiffAttributes[ 0 ];
        m_iCode = (int) decl.Code;
      }
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    protected  BiffRecordRaw( Stream stream, out int itemSize )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="reader">BinaryReader from which record data should be read.</param>
    /// <param name="itemSize">Size of the read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is NULL.
    /// </exception>
    protected  BiffRecordRaw( BinaryReader reader, out int itemSize )
    {
      //throw new NotImplementedException();
      FillRecord( reader, null, null, null );
      itemSize = m_iLength;
    }
    /// <summary>
    /// Reserved for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    protected  BiffRecordRaw( int iReserve )
    {
    }
    #endregion

    #region Class Serialization
    /// <summary>
    /// Read from stream record data.
    /// </summary>
    /// <param name="reader">Stream with record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="decryptor">Object that should be used to parse encrypted records.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Size of the record data.</returns>
    /// <exception cref="System.ArgumentNullException">If reader is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    /// If stream is not big enough for data (end of stream
    /// reached and all data was not read).
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If record code is zero.
    /// </exception>
    public virtual int FillRecord( BinaryReader reader, DataProvider provider,
      IDecryptor decryptor, byte[] arrBuffer )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Stream stream = reader.BaseStream;
      long lPosStart = stream.Position;
      long lSize = stream.Length;

      provider.Read( reader, 0, BiffRecordRaw.DEF_HEADER_SIZE, arrBuffer );
      m_iCode = provider.ReadInt16( 0 );
      m_iLength = provider.ReadInt16( 2 );


      try
      {
        long lSpaceLeft = lSize - lPosStart - 4;

        if( lSpaceLeft < 0 )
          throw new ApplicationException( "Unexpected end of records stream - reached end of stream." );

        //m_iCode = ( int )reader.ReadInt16() & 0xffff;

        if( m_iCode == 0 )
          throw new ApplicationException( "Biff record identification code is wrong." );

        ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ( TBIFFRecord )m_iCode, "Record code to fill" );

        //m_iLength = ( int )reader.ReadInt16() & 0xffff;

        if( m_iLength < MinimumRecordSize && !IsAllowShortData )
          throw new SmallBiffRecordDataException( "Code :" + ( ( TBIFFRecord )m_iCode ).ToString()
            + "\n Real size: " + m_iLength + ". Expected size: " + MinimumRecordSize.ToString() );

//        if( m_iLength > MaximumRecordSize )
//          throw new LargeBiffRecordDataException( "Code :" + ((TBIFFRecord)m_iCode).ToString()
//            + "\n Real size: " + m_iLength + ". Expected size: " + MaximumRecordSize.ToString() );

        if( lSpaceLeft - m_iLength < 0 )
          throw new ApplicationException( "Unexpected end of records stream. Record data cannot " +
            "be readed - reached end of stream." );

        provider.Read( reader, 0, m_iLength, null );

        if( decryptor != null && NeedDecoding )
        {
          int iOffset = StartDecodingOffset;
          decryptor.Decrypt( provider, iOffset, m_iLength - iOffset, ( int )( lPosStart + 4 + iOffset ) );
        }

        // Store record stream position. Stream position of record can be
        // used by ParseStructure method. That is why we fill it first.
        StreamPos = lPosStart;

        if( m_iLength > MaximumRecordSize )
        {
          m_iLength = MaximumRecordSize;
        }

        ParseStructure( provider, 0, m_iLength, ExcelVersion.Excel97to2003 );

        return (int)( stream.Position - lPosStart );
      }
      catch( ApplicationException ex )
      {
#if DEBUG
        if( ex != null )
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );

        if( ex != null && ex.InnerException != null )
          //Debug.WriteLineIf( ex.InnerException != null, ex.InnerException.Message 
          //  + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception" );
#endif

        // Recover stream position on exception.
        stream.Position = lPosStart;
        throw;
      }
    }

    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    public virtual int FillStream( BinaryWriter writer, DataProvider provider, IEncryptor encryptor,
      int streamPosition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );

      if( m_iLength < 0 )
        throw new ApplicationException( "Wrong Record data infill. " + TypeCode.ToString() );

      //writer.Write( ( ushort )m_iCode );
      provider.WriteInt16( 0, ( short )m_iCode );
//      byte byte1 = ( byte )( m_iCode & 0xFF );
//      byte byte2 = ( byte )( ( m_iCode >> 8 ) & 0xFF );
//      arrBuffer[ 0 ] = byte1;
//      arrBuffer[ 1 ] = byte2;

      //writer.Write( ( ushort )m_iLength );
      provider.WriteInt16( 2, ( short )m_iLength );
//      byte1 = ( byte )( m_iLength & 0xFF );
//      byte2 = ( byte )( ( m_iLength >> 8 ) & 0xFF );
//      arrBuffer[ 2 ] = byte1;
//      arrBuffer[ 3 ] = byte2;

      int iTotalLength = m_iLength + DEF_HEADER_SIZE;

      if( m_iLength > 0 )
      {
        InfillInternalData( provider, DEF_HEADER_SIZE, ExcelVersion.Excel97to2003 );
      }

      byte[] arrBuffer = ( ( ByteArrayDataProvider )provider ).InternalBuffer;

      if( encryptor != null && NeedDecoding )
      {
        int iOffset = StartDecodingOffset;
        encryptor.Encrypt( provider, DEF_HEADER_SIZE + iOffset, m_iLength - iOffset,
          streamPosition + DEF_HEADER_SIZE + iOffset );
      }

      provider.WriteInto( writer, 0, iTotalLength, arrBuffer );

      return ( int )( iTotalLength );
    }
    /// <summary>
    /// Method which updates the fields of record which contain stream offset
    /// or other data. This method must be called before save operation.
    /// </summary>
    /// <param name="records">Array with all records.</param>
    /// <exception cref="System.ApplicationException">
    /// When not overridden in descendant class.
    /// </exception>
    public virtual void UpdateOffsets( List<BiffRecordRaw> records )
    {
      throw new ApplicationException( "Class marked as offset contains field but " +
        "does not provide override of UpdateOffset method. Or you try to call " +
        "parent class virtual method. Please check code." );
    }
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="arrData">Array that contains record's data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public virtual void ParseStructure( DataProvider arrData, int iOffset, int iLength, ExcelVersion version )
    {
      throw new NotImplementedException( TypeCode.ToString() );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public virtual void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      throw new NotImplementedException( TypeCode.ToString() );
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public virtual int GetStoreSize( ExcelVersion version )
    {
      int iMinSize = MinimumRecordSize;

      if( iMinSize == MaximumRecordSize )
        return iMinSize;

      throw new ApplicationException( "StoreSize should be overloaded " + TypeCode.ToString() );
    }
    #endregion

    #region Static Read Helper methods
    /// <summary>
    /// Checks offset and length of the array.
    /// </summary>
    /// <param name="arrData">Array to check.</param>
    /// <param name="offset">Start offset.</param>
    /// <param name="length">Length of the data.</param>
    public static void CheckOffsetAndLength( byte[] arrData, int offset, int length )
    {
      int iLen = arrData.Length;
      if( offset < 0 || offset > iLen )
        throw new ArgumentOutOfRangeException( "offset", "" );

      if( length < 0 || length > iLen )
        throw new ArgumentOutOfRangeException( "length", "" );

      if( ( length + offset ) > iLen )
        throw new ArgumentException( "Length or offset has wrong value.", "length & offset" );
    }
    /// <summary>
    /// Get array of bytes from internal record data.
    /// </summary>
    /// <param name="arrData"></param>
    /// <param name="offset">Offset of first byte of data to get.</param>
    /// <param name="length">Length of required array.</param>
    /// <returns>Array of bytes from internal record data.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If offset is less than zero or more than internal record data array size
    ///   or length is less than zero or more than internal record data array size
    ///   or length plus offset is more than internal record data array size.
    /// </exception>
    public static byte[] GetBytes( byte[] arrData, int offset, int length )
    {
      CheckOffsetAndLength( arrData, offset, length );

      byte[] retValue = new byte[ length ];

      Buffer.BlockCopy( arrData, offset, retValue, 0, length );
      return retValue;
    }

    /// <summary>
    /// Gets single byte from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of byte to get.</param>
    /// <returns>Single byte from internal record data.</returns>
    public static byte   GetByte( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 1 );
      return arrData[ offset ];//BitConverter.ToUInt16( arrData, offset );
      //      return GetBytes( offset, 1 )[0];
    }

    /// <summary>
    /// Gets ushort from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of ushort to get.</param>
    /// <returns>Ushort from internal record data.</returns>
    [ CLSCompliant( false ) ]
    public static ushort GetUInt16( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 2 );
      return BitConverter.ToUInt16( arrData, offset );
      //return BitConverter.ToUInt16( GetBytes( offset, 2 ), 0 );
    }

    /// <summary>
    /// Gets short from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of short to get.</param>
    /// <returns>Short from internal record.</returns>
    [ CLSCompliant( false ) ]
    public static short  GetInt16( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 2 );
      return BitConverter.ToInt16( arrData, offset );
      //      return BitConverter.ToInt16( GetBytes( offset, 2 ), 0 );
    }

    /// <summary>
    /// Gets int from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of int to get.</param>
    /// <returns>Int from internal record.</returns>
    public static int    GetInt32( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 4 );
      return BitConverter.ToInt32( arrData, offset );
      //      return BitConverter.ToInt32( GetBytes( offset, 4 ), 0 );
    }

    /// <summary>
    /// Gets uint from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of uint to get.</param>
    /// <returns>Uint from internal record data.</returns>
    [ CLSCompliant( false ) ]
    public static uint   GetUInt32( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 4 );
      return BitConverter.ToUInt32( arrData, offset );
      //      return BitConverter.ToUInt32( GetBytes( offset, 4 ), 0 );
    }

    /// <summary>
    /// Gets long from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of long to get.</param>
    /// <returns>Long from internal record data.</returns>
    public static long   GetInt64( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 28);
      return BitConverter.ToInt64( arrData, offset );
      //      return BitConverter.ToInt64( GetBytes( offset, 8 ), 0 );
    }

    /// <summary>
    /// Gets ulong from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of ulong to get.</param>
    /// <returns>Ulong from internal record data.</returns>
    [ CLSCompliant( false ) ]
    public static ulong  GetUInt64( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 8 );
      return BitConverter.ToUInt64( arrData, offset );
      //      return BitConverter.ToUInt64( GetBytes( offset, 8 ), 0 );
    }

    /// <summary>
    /// Gets float from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of float to get.</param>
    /// <returns>Float from internal record data.</returns>
    public static float  GetFloat( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 4 );
      return BitConverter.ToSingle( arrData, offset );
      //      return BitConverter.ToSingle( GetBytes( offset, 4 ), 0 );
    }

    /// <summary>
    /// Gets double from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of double to get.</param>
    /// <returns>Double from internal record data.</returns>
    public static double GetDouble( byte[] arrData, int offset )
    {
      CheckOffsetAndLength( arrData, offset, 8 );
      return BitConverter.ToDouble( arrData, offset );
      //      return BitConverter.ToDouble( GetBytes( offset, 8 ), 0 );
    }

    /// <summary>
    /// Gets single bit from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of the byte that contains needed bit.</param>
    /// <param name="bitPos">Position of bit in the byte.</param>
    /// <returns>True if specified bit is set to 1; otherwise False.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///  bitPos argument should be not less than zero and not more than 7.
    /// </exception>
    public static bool   GetBit( byte[] arrData, int offset, int bitPos )
    {
      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater than 7." );

      if( arrData.Length <= offset )
        throw new ArgumentOutOfRangeException( "offset" );

      return ( arrData[ offset ] & ( 1 << bitPos ) ) == ( 1 << bitPos );

      //return ( GetBytes( arrData, offset, 1 )[0] & ( 1 << bitPos ) ) == ( 1 << bitPos );
    }

    /// <summary>
    /// Gets single bit from internal record data using GetBytes.
    /// </summary>
    /// <param name="ptrData">Source memory block.</param>
    /// <param name="offset">Offset of the byte that contains needed bit.</param>
    /// <param name="bitPos">Position of bit in the byte.</param>
    /// <returns>True if specified bit is set to 1; otherwise False.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///  bitPos argument should be not less than zero and not more than 7.
    /// </exception>
    public static bool   GetBit( IntPtr ptrData, int offset, int bitPos )
    {
      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater than 7." );

      byte btValue = Marshal.ReadByte( ptrData, offset );
      return ( btValue & ( 1 << bitPos ) ) == ( 1 << bitPos );

      //return ( GetBytes( arrData, offset, 1 )[0] & ( 1 << bitPos ) ) == ( 1 << bitPos );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes,
    /// and it increases the offset by string size in bytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of starting byte.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    public static string GetString16BitUpdateOffset( byte[] arrData, ref int offset )
    {
      int iStrLen = GetUInt16( arrData, offset );
      offset += 2;
      if( iStrLen > 0 )
      {
        int iBytes;
        string result = GetString( arrData, offset, iStrLen, out iBytes );
        offset += iBytes;
        return result;
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes
    /// and it increases offset by string size in bytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    public static string GetStringUpdateOffset( byte[] arrData, ref int offset, int iStrLen )
    {
      if( iStrLen > 0 )
      {
        int iBytes;
        string result = GetString( arrData, offset, iStrLen, out iBytes );
        offset += iBytes;
        return result;
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of starting byte.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    public static string GetStringByteLen( byte[] arrData, int offset )
    {
      int iLen = GetByte( arrData, offset );
      return GetString( arrData, offset + 1, iLen );
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    public static string GetString( byte[] arrData, int offset, int iStrLen )
    {
      int Bytes;
      return GetString( arrData, offset, iStrLen, out Bytes );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <param name="isByteCounted">Flags that represent is bytes count available.</param>
    /// <returns>Retrieved string.</returns>
    public static string GetString( byte[] arrData, int offset, int iStrLen, out int iBytesInString, bool isByteCounted )
    {
      byte iCompressUnicode = GetByte( arrData, offset );
      int  iLast = ( iCompressUnicode != 0 && !isByteCounted ) ? 2 * iStrLen : iStrLen;

      iLast += offset + 1;

      if( iLast > arrData.Length )
        throw new WrongBiffRecordDataException( 
          string.Format( "String and arrData array do not fit each other {0}." ));

      if( iCompressUnicode == 0 )
      {
        iBytesInString = iStrLen;
        return BiffRecordRaw.LatinEncoding.GetString( GetBytes( arrData, offset + 1,  iStrLen ), 0, iStrLen );
      }
      else
      {
        iBytesInString = isByteCounted ? iStrLen : iStrLen * 2;
        return Encoding.Unicode.GetString( GetBytes( arrData, offset + 1, 
          iBytesInString ), 0, iBytesInString );
      }
    }
    /// <summary>
    /// Detect type of string and extracts it.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Record data offset.</param>
    /// <param name="continuePos">Position of next object.</param>
    /// <param name="length">Length of string record.</param>
    /// <param name="rich">Array of rich formatting values.</param>
    /// <param name="extended">Array of unknown FarEast data.</param>
    /// <returns>Extracted string.</returns>
    public static string GetUnkTypeString( byte[] arrData, int offset, int[] continuePos,
      out int length, out byte[] rich, out byte[] extended )
    {
      string  retValue = string.Empty;
      int     retLength = 3;

      rich = null;
      extended = null;

      int iCurPos = offset;

      ushort iLen = GetUInt16( arrData, iCurPos );
      byte btFlags = GetByte( arrData, iCurPos + 2 );

      bool bIsUnicode    = ( btFlags & 0x1 ) == 1;
      bool bIsUniWithout = ( btFlags <= 1 );
      bool bIsUniRich    = ( btFlags == 8 || btFlags == 9 );
      bool bIsUniFarEast = ( btFlags == 4 || btFlags == 5 );
      bool bIsUniRichEast= ( btFlags == 0xC || btFlags == 0xD );

      int strOffset = 3;
      //retLength += ( bIsUnicode ) ? iLen * 2 : iLen;
      short sRichRuns = 0;

      if( bIsUniRich )
      {
        sRichRuns = GetInt16( arrData, iCurPos + 3 );
        strOffset = 5;
        extended = null;

        retLength += 2;
      }
      else if( bIsUniFarEast )
      {
        int iSize = GetInt32( arrData, iCurPos + 3 );
        strOffset = 7;
        rich = null;

        retLength += 4;
        extended = GetBytes( arrData, retLength, iSize );

        retLength += iSize;
      }
      else if( bIsUniRichEast )
      {
        sRichRuns = GetInt16( arrData, iCurPos + 3 );
        int iSize = GetInt32( arrData, iCurPos + 5 );
        strOffset = 9;

        retLength += 6;
        rich = GetBytes( arrData, retLength, sRichRuns * 4 );

        retLength += sRichRuns * 4;
        extended = GetBytes( arrData, retLength, iSize );

        retLength += iSize;
      }

      int iStringStart = iCurPos + strOffset;
      int iCurChar = 0;
      int iBreakIndex = 0;

      while( iCurChar < iLen )
      {
        int iDesiredLen = bIsUnicode ? ( iLen - iCurChar ) * 2 : iLen - iCurChar;
        
        // Get length of string or part of it in continue record.
        int iBreakPos = FindNextBreak( continuePos, continuePos.Length, iStringStart, ref iBreakIndex );
        int iBytesLeft  = iBreakPos - iStringStart;

        if( iDesiredLen <= iBytesLeft )
        {
          retValue += bIsUnicode ?
            Encoding.Unicode.GetString( GetBytes( arrData, iStringStart,  iDesiredLen ), 0, iDesiredLen ) :
            BiffRecordRaw.LatinEncoding.GetString( GetBytes( arrData, iStringStart,  iDesiredLen ), 0, iDesiredLen ) ;
          retLength += iDesiredLen;
          break;
        }
        else
        {
          // Get part of string.
          if( iBytesLeft > 0 )
          {
            retValue += bIsUnicode ?
              Encoding.Unicode.GetString( GetBytes( arrData, iStringStart, iBytesLeft ), 0, iBytesLeft ) :
              BiffRecordRaw.LatinEncoding.GetString( GetBytes( arrData, iStringStart, iBytesLeft ), 0, iBytesLeft );

            iCurChar += bIsUnicode ? iBytesLeft / 2 : iBytesLeft;
          }

          if( arrData[ iStringStart + iBytesLeft ] == 0
            || arrData[ iStringStart + iBytesLeft ] == 1 )
          {
            bIsUnicode = ( arrData[ iStringStart + iBytesLeft ] == 1 );
            iStringStart++;
            retLength++;
          }

          iStringStart += iBytesLeft;
          retLength += iBytesLeft;
        }
      }

      if( bIsUniRich )
      {
        rich = GetBytes( arrData, offset + retLength, sRichRuns * 4 );

        retLength += sRichRuns * 4;
      }

      length = retLength;

      return retValue;
    }
    /// <summary>
    /// Gets TAddr structure from internal record data.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset in bytes of TAddr structure to get.</param>
    /// <returns>Retrieved TAddr structure.</returns>
    [ CLSCompliant( false ) ]
    public static TAddr  GetAddr( byte[] arrData, int offset )
    {
      TAddr addr = new TAddr();

      addr.FirstRow = GetUInt16( arrData, offset );
      addr.LastRow  = GetUInt16( arrData, offset + 2 );
      addr.FirstCol = GetUInt16( arrData, offset + 4 );
      addr.LastCol  = GetUInt16( arrData, offset + 6 );

      return addr;
    }
    /// <summary>
    /// Extract the Reverse-Polish Notation formula from the data array.
    /// </summary>
    /// <param name="arrData">Source byte array.</param>
    /// <param name="offset">Offset to the RPN data.</param>
    /// <param name="length">Length of the data.</param>
    /// <returns>Extracted RPN data.</returns>
    public static byte[] GetRPNData( byte[] arrData, int offset, int length )
    {
      if( length == 0 ) return new byte[]{};

      ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, DumpArrayToHex( arrData ) );

      List<byte> formula = new List<byte>( length * 2 );
      int iLen = length;
      int offsetDelta = 0;

      do
      {
        byte btTest = GetByte( arrData, offset + offsetDelta );

        switch( btTest )
        {
          case 0x20:
          case 0x40:
          case 0x60: // tArray
            int len = ( GetByte( arrData, offset + offsetDelta + 1 ) + 1 ) *
              ( GetInt16( arrData, offset + offsetDelta + 2 ) + 1 ) + 1;

            int iPos = 0;
            while( len > 0 )
            {
              byte tmpId = GetByte( arrData, offset + length + iPos );

              if( tmpId == 0x02 ) // string
              {
                iPos += GetInt16( arrData, offset + length + iPos + 1 ) + 4;
              }
              else if( tmpId == 0x04 ) iPos += 3;
              else iPos += 9;

              len--;
            }

            for( int i=offset + offsetDelta; i < offset + length + iPos; i++ )
            {
              formula.Add( arrData[ i ] );
            }

            return formula.ToArray();

          default:
            return GetBytes( arrData, offset, length );
        }
      }
      while( true );

      //return ( byte[] )formula.ToArray( typeof(byte) );
    }
    /// <summary>
    /// Searches for the next break.
    /// </summary>
    /// <param name="arrBreaks">List of breaks.</param>
    /// <param name="iCount">Number of elements in the list.</param>
    /// <param name="curPos">Current break position.</param>
    /// <param name="iStartIndex">Start index in the array.</param>
    /// <returns>Next break.</returns>
    protected static int FindNextBreak( IList<int> arrBreaks, int iCount, int curPos, ref int iStartIndex )
    {
      for( int i = iStartIndex/*, len = arrBreaks.Count*/; i < iCount; i++ )
      {
        int curBreak = arrBreaks[ i ];

        if( curPos <= curBreak )
        {
          iStartIndex = i;
          return curBreak;
        }
      }

      return -1;
    }
    #endregion

    #region Debug/Temporary methods
#if DEBUG
    /// <summary>
    /// Dumps Array to Hex.
    /// </summary>
    /// <param name="array">Source array.</param>
    /// <returns>Result string.</returns>
    public static string DumpArrayToHex( byte[] array )
    {
      StringBuilder builder = new StringBuilder( 8192 );
      int iPos = array.Length;

      builder.Append( "-= HEX Dump =-\n" );
      int iCount = 0;
      string strOut = "";

      for( int i=0; i<iPos; i++ )
      {
        int bt = (int)array[i];
        strOut += ( bt < 32 ) ? ' ' : Convert.ToChar( bt );
        string tmpOut = string.Format( "{0:X} ", bt );
        builder.Append( ( tmpOut.Length == 2 ) ? "0" + tmpOut : tmpOut );
        iCount++;

        if( iCount == 16 )
        {
          builder.Append( string.Format( "| {0}\n", strOut ) );
          iCount = 0;
          strOut = "";
        }
      }

      int iLen = (16 - (iPos % 16))*3;
      for( int i=0; i<iLen; i++ )
      {
        builder.Append( " " );
      }

      if( iLen > 0 )
      {
        builder.Append( string.Format( "| {0}", strOut ) );
      }

      builder.Append( "\n-= HEX Dump =-" );

      return builder.ToString();
    }
#endif
    #endregion

    #region Static Write Helper methods

    /// <summary>
    /// Sets ushort in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="arrData">Byte array list.</param>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified ushort.</param>
    [ CLSCompliant( false ) ]
    public static void SetUInt16( byte[] arrData, int offset, ushort value )
    {
      byte byte1 = ( byte )/*value;/*/( value & 0xFF );
      byte byte2 = ( byte )/*( value >> 8 );/*/( ( value >> 8 ) & 0xFF );
      arrData[ offset ] = byte1;
      arrData[ offset + 1 ] = byte2;
      //Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, arrData, offset, 2 );
    }

    /// <summary>
    /// Sets one bit in specified byte in internal record data array.
    /// </summary>
    /// <param name="arrData">Array where byte is located..</param>
    /// <param name="offset">Offset of the byte in the data array.</param>
    /// <param name="bitPos">Bit position in the byte.</param>
    /// <param name="value">Value of bit.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7.
    /// </exception>
    public static void SetBit( byte[] arrData, int offset, bool value, int bitPos )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zero or greater than 7." );
    
      if( value )
      {
        arrData[ offset ] |= ( byte )( 1 << bitPos );
      }
      else
      {
        arrData[ offset ] &= ( byte )( ~( 1 << bitPos ) );
      }
    }
    /// <summary>
    /// Sets short in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="arrData">Array where value should be placed located.</param>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified short.</param>
    public static void SetInt16( byte[] arrData, int offset, short value )
    {
      Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, arrData, offset, 2 );
    }

    /// <summary>
    /// Sets int in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="arrData">Array where value should be placed located.</param>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified int.</param>
    public static void SetInt32( byte[] arrData, int offset, int value )
    {
      Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, arrData, offset, 4 );
    }

    /// <summary>
    /// Sets uint in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="arrData">Array where value should be placed located.</param>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified uint.</param>
    [ CLSCompliant( false ) ]
    public static void SetUInt32( byte[] arrData, int offset, uint value )
    {
      Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, arrData, offset, 4 );
    }

//    /// <summary>
//    /// Sets long in internal record data array using SetBytes method.
//    /// </summary>
//    /// <param name="offset">Offset to the required value.</param>
//    /// <param name="value">New value for the specified long.</param>
//    public static void SetInt64( int offset, long value )
//    {
//      SetBytes( offset, BitConverter.GetBytes( value ), 0, 8 );
//    }
//
//    /// <summary>
//    /// Sets ulong in internal record data array using SetBytes method.
//    /// </summary>
//    /// <param name="offset">Offset to the required value.</param>
//    /// <param name="value">New value for the specified ulong.</param>
//    public static void SetUInt64( int offset, ulong value )
//    {
//      SetBytes( offset, BitConverter.GetBytes( value ), 0, 8 );
//    }
//
//    /// <summary>
//    /// Sets float in internal record data array using SetBytes method.
//    /// </summary>
//    /// <param name="offset">Offset to the required value.</param>
//    /// <param name="value">New value for the specified float.</param>
//    public static void SetFloat( int offset, float value )
//    {
//      SetBytes( offset, BitConverter.GetBytes( value ), 0, 4 );
//    }
//
    /// <summary>
    /// Sets double in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="arrData">Array where value should be placed.</param>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified double.</param>
    public static void SetDouble( byte[] arrData, int offset, double value )
    {
      Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, arrData, offset, 8 );
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length, updates offset parameter (adds string length).
    /// </summary>
    /// <param name="arrData">Array where value should be placed located.</param>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public static void SetStringNoLenUpdateOffset( byte[] arrData, ref int offset, string value )
    {
      if( value == null || value.Length == 0 ) return;

      byte[] tmpData = Encoding.Unicode.GetBytes( value );

      // we always save strings in Unicode
      arrData[ offset ] = 1;

      // save string
      SetBytes( arrData, offset + 1, tmpData, 0, tmpData.Length );

      offset += tmpData.Length + 1;
    }
//
//    /// <summary>
//    /// Sets string in internal record data array using SetBytes method
//    /// without string length.
//    /// </summary>
//    /// <param name="offset">Offset to the string.</param>
//    /// <param name="value">Value of the string.</param>
//    /// <returns>Size of the string in bytes.</returns>
//    public static int SetStringNoLen( int offset, string value )
//    {
//      if( value == null || value.Length == 0 ) return 0;
//
//      byte[] tmpData = Encoding.Unicode.GetBytes( value );
//
//      // we always save strings in Unicode
//      SetByte( offset, 1 );
//
//      // save string
//      SetBytes( offset + 1, tmpData, 0, tmpData.Length );
//
//      return tmpData.Length + 1;
//    }
//
    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in one byte.
    /// </summary>
    /// <param name="arrData">Array where value should be placed located.</param>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public static void SetStringByteLen( byte[] arrData, int offset, string value )
    {
      // save length
      arrData[ offset ] = ( byte )value.Length;

      SetStringNoLen( arrData, offset + 1, value );
    }

//    /// <summary>
//    /// Sets string in internal record data array using SetBytes method.
//    /// The String length is saved in two bytes.
//    /// </summary>
//    /// <param name="offset">Offset to the string.</param>
//    /// <param name="value">Value of the string.</param>
//    /// <returns>Size of the string in bytes.</returns>
//    public static int SetString16BitLen( int offset, string value )
//    {
//      // save length
//      SetUInt16( offset, (ushort)value.Length );
//
//      return 2 + SetStringNoLen( offset + 2, value );
//    }
//    /// <summary>
//    /// Sets string in internal record data array using SetBytes method.
//    /// The String length is saved in two bytes
//    /// </summary>
//    /// <param name="offset">Offset to the string.</param>
//    /// <param name="value">Value of the string.</param>
//    public static void SetString16BitUpdateOffset( ref int offset, string value )
//    {
//      // save length
//      SetUInt16( offset, (ushort)value.Length );
//
//      offset += 2;
//      SetStringNoLenUpdateOffset( ref offset, value );
//    }
//    /// <summary>
//    /// Sets TAddr structure in internal record data.
//    /// </summary>
//    /// <param name="offset">Offset in bytes of TAddr structure to set.</param>
//    /// <param name="addr">New value of the TAddr at the specified position.</param>
//    public static void SetAddr( int offset, TAddr addr )
//    {
//      SetUInt16( offset    , addr.FirstRow );
//      SetUInt16( offset + 2, addr.LastRow  );
//      SetUInt16( offset + 4, addr.FirstCol );
//      SetUInt16( offset + 6, addr.LastCol  );
//    }
//    /// <summary>
//    /// Sets one bit in specified ushort.
//    /// </summary>
//    /// <param name="offset">Offset of the byte in the internal record data array.</param>
//    /// <param name="value">Value of bit.</param>
//    /// <param name="bitPos">Bit position in the byte.</param>
//    /// <exception cref="System.ArgumentOutOfRange">
//    /// If bitPos is less than zero or more than 7.
//    /// </exception>
//    public static void SetBitInVar( ref ushort variable, bool value, int bitPos )
//    {
//      if( bitPos < 0 || bitPos > 15 )
//        throw new ArgumentOutOfRangeException( "bitPos", bitPos, "Bit Position can be zero or greater than 7." );
//
//      if( value )
//      {
//        variable |= ( ushort )( 1 << bitPos );
//      }
//      else
//      {
//        variable &= ( ushort )(~( 1 << bitPos ));
//      }
//    }
//
    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
    /// <param name="arrBuffer">Buffer to copy bytes into.</param>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Array of bytes to set.</param>
    /// <param name="pos">Position in value array to the data that will be set.</param>
    /// <param name="length">Length of the data.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If value array is NULL.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If pos or length would be less than zero  or their sum would be more
    ///   than size of value array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If internal record data array is too small for receiving value array
    ///   and AutoGrowData is False.
    /// </exception>
    internal protected static void SetBytes( byte[] arrBuffer, int offset, byte[] value, int pos, int length )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( pos < 0 )
        throw new ArgumentOutOfRangeException( "pos", "Position cannot be zeroless." );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length", "Length of data to copy must be greater then zero." );

      if( pos + length > value.Length )
        throw new ArgumentOutOfRangeException( "value", "Position or length has wrong value." );

      Buffer.BlockCopy( value, pos, arrBuffer, offset, length );
    }

    /// <summary>
    /// Sets one bit in specified ushort.
    /// </summary>
    /// <param name="variable">Offset of the byte in the internal record data array.</param>
    /// <param name="value">Value of bit.</param>
    /// <param name="bitPos">Bit position in the byte.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7.
    /// </exception>
    [ CLSCompliant( false ) ]
    internal protected void SetBitInVar( ref ushort variable, bool value, int bitPos )
    {
      if( bitPos < 0 || bitPos > 15 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zero or greater than 7." );

      if( value )
      {
        variable |= ( ushort )( 1 << bitPos );
      }
      else
      {
        variable &= ( ushort )(~( 1 << bitPos ));
      }
    }

    /// <summary>
    /// Sets one bit in specified uint.
    /// </summary>
    /// <param name="variable">Offset of the byte in the internal record data array.</param>
    /// <param name="value">Value of bit.</param>
    /// <param name="bitPos">Bit position in the byte.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7.
    /// </exception>
    [CLSCompliant( false )]
    internal protected void SetBitInVar( ref uint variable, bool value, int bitPos )
    {
      if( bitPos < 0 || bitPos > 31 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zero or greater than 7." );

      if( value )
      {
        variable |= ( uint )( 1 << bitPos );
      }
      else
      {
        variable &= ( uint )( ~( 1 << bitPos ) );
      }
    }

    /// <summary>
    /// Return size in bytes for string with 16-bit length field.
    /// </summary>
    /// <param name="strValue">String to measure.</param>
    /// <param name="isCompressed">Indicates whether string should be compressed or not.</param>
    /// <returns>Size in bytes for string with 16-bit length field.</returns>
    public int Get16BitStringSize( string strValue, bool isCompressed )
    {
      if( strValue == null || strValue.Length == 0 )
        return 2;

      Encoding encoding = isCompressed ?
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.ASCII :
#else
        Encoding.UTF8 :
#endif
        Encoding.Unicode;
      return 3 + encoding.GetByteCount( strValue );
    }
    #endregion

    #region Class Complementary methods
    /// <summary>
    /// Clears data.
    /// </summary>
    public virtual void ClearData()
    {
    }
    /// <summary>
    /// Compares two Biff records.
    /// </summary>
    /// <param name="raw">Biff record that should be compared with this Biff record.</param>
    /// <returns>True if this instance and extFormat contain the same data.</returns>
    public virtual bool IsEqual( BiffRecordRaw raw )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Copies data from the current Biff record to the specified Biff record.
    /// </summary>
    /// <param name="raw">Biff record that will receive data from the current record.</param>
    /// <exception cref="System.ArgumentException">
    /// When this record and parameter have different types.
    /// </exception>
    public virtual void CopyTo( BiffRecordRaw raw )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Checked typeCode.
    /// </summary>
    /// <param name="typeCode">TypeCode to check.</param>
    public void CheckTypeCode( TBIFFRecord typeCode )
    {
      if( TypeCode != typeCode )
        throw new ArgumentOutOfRangeException( typeCode.ToString() + " record was expected" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="iStartIndex1"></param>
    /// <param name="array2"></param>
    /// <param name="iStartIndex2"></param>
    /// <param name="iLength"></param>
    /// <returns></returns>
    static public bool CompareArrays( byte[] array1, int iStartIndex1, byte[] array2,
      int iStartIndex2, int iLength )
    {
      int k = 0;
      for( int i = iStartIndex1, j = iStartIndex2;
        k < iLength && i < array1.Length && j < array2.Length;
        k++, i++, j++ )
      {
        if( array1[ i ] != array2[ j ] )
          return false;
      }

      if( k == iLength && k != 0 ) return true;

      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="array2"></param>
    /// <returns></returns>
    static public bool CompareArrays( byte[] array1, byte[] array2 )
    {
      if( array1 == null && array2 == null ) return true;

      if( array1 == null || array2 == null ) return false;

      int iArray1Len = array1.Length;
      int iArray2Len = array2.Length;

      if( iArray1Len != iArray2Len ) return false;

      if( iArray1Len == 0 && iArray2Len == 0 ) return true;

      return CompareArrays( array1, 0, array2, 0, array1.Length );
    }
    /// <summary>
    /// Sets record code.
    /// </summary>
    /// <param name="code">Code to set.</param>
    internal void SetRecordCode( int code )
    {
      m_iCode = code;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Clone current Record.
    /// </summary>
    /// <returns>Returns memberwise clone on current object.</returns>
    public virtual object Clone()
    {
      return this.MemberwiseClone();
    }
    #endregion

    #region Class static properties
    /// <summary>
    /// 
    /// </summary>
    public static Encoding LatinEncoding
    {
      get
      {
        return s_latin1;
      }
    }
    #endregion

    #region Class Static methods
    /// <summary>
    /// Combines several byte arrays into one.
    /// </summary>
    /// <param name="iCombinedLength">Length of combined array.</param>
    /// <param name="arrCombined">List that contains byte arrays to combine.</param>
    /// <returns>Combined array.</returns>
    public static byte[] CombineArrays( int iCombinedLength, List<byte[]> arrCombined )
    {
      if( arrCombined == null || arrCombined.Count == 0 )
        return new byte[ 0 ];

      int iLength = arrCombined.Count;

      byte[] arrResult = new byte[ iCombinedLength ];
      int iOffset = 0;

      for( int i = 0; i < iLength; i++ )
      {
        byte[] arrCurrent = arrCombined[ i ];
        int iPartLength = arrCurrent.Length;
        Buffer.BlockCopy( arrCurrent, 0, arrResult, iOffset, iPartLength );
        iOffset += iPartLength;
      }

      return arrResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    /// <param name="iOffset"></param>
    /// <param name="iLength"></param>
    /// <param name="iReadBytes"></param>
    /// <returns></returns>
    public static string GetString( byte[] arrData, int iOffset, int iLength,
      out int iReadBytes )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      if( iOffset < 0 || iOffset + iLength > arrData.Length )
        throw new ArgumentOutOfRangeException( "iOffset" );

      if( iLength < 0 )
        throw new ArgumentOutOfRangeException( "iLength" );

      byte iCompressUnicode = arrData[ iOffset ];//GetByte( offset );
      int  iLast = ( iCompressUnicode != 0 ) ? 2 * iLength : iLength;

      iLast += iOffset + 1;

      if( iLast > arrData.Length )
      {
        throw new WrongBiffRecordDataException( 
          string.Format( "String and m_data array do not fit each other" ));
      }

      string strResult;
      
      Encoding encoding;

      if( iCompressUnicode == 0 )
      {
        iReadBytes = iLength;
        encoding = LatinEncoding;
      }
      else
      {
        iReadBytes = iLength * 2;
        encoding = Encoding.Unicode;
      }

      strResult = encoding.GetString( arrData, iOffset + 1, iReadBytes );
      iReadBytes++;

      return strResult;
    }
    /// <summary>
    /// Copies string data into array of bytes without string length field.
    /// </summary>
    /// <param name="arrData">Destination array.</param>
    /// <param name="iOffset">Offset to the string.</param>
    /// <param name="strValue">Value of the string.</param>
    /// <returns>Size of the string in bytes.</returns>
    public static int SetStringNoLen( byte[] arrData, int iOffset, string strValue )
    {
      if( strValue == null || strValue.Length == 0 ) return 0;

      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      byte[] tmpData = Encoding.Unicode.GetBytes( strValue );

      if( iOffset < 0 || iOffset + tmpData.Length + 1 > arrData.Length )
        throw new ArgumentOutOfRangeException( "iOffset" );

      // we always save strings in Unicode
      arrData[ iOffset ] = 1;
      iOffset++;

      // save string
      tmpData.CopyTo( arrData, iOffset );

      return tmpData.Length + 1;
    }
    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in two bytes
    /// </summary>
    /// <param name="arrData">Destination array.</param>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public static void SetString16BitUpdateOffset( byte[] arrData, ref int offset, string value )
    {
      // save length
      SetUInt16( arrData, offset, (ushort)value.Length );

      offset += 2;
      SetStringNoLenUpdateOffset( arrData, ref offset, value );
    }
    /// <summary>
    /// Returns value of the single bit from byte.
    /// </summary>
    /// <param name="btOptions">Byte to get bit value from.</param>
    /// <param name="bitPos">Bit index.</param>
    /// <returns>Value of the single bit from byte.</returns>
    public static bool GetBitFromVar( byte btOptions, int bitPos )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_BYTE )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 7." );

      return GetBitFromVar( ( int )btOptions, bitPos );
    }
    /// <summary>
    /// Returns value of the single bit from byte.
    /// </summary>
    /// <param name="sOptions">Int16 to get bit value from.</param>
    /// <param name="bitPos">Bit index.</param>
    /// <returns>Value of the single bit from byte.</returns>
    public static bool GetBitFromVar( short sOptions, int bitPos )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_SHORT )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 15." );

      if( bitPos == DEF_BITS_IN_SHORT - 1 ) return ( sOptions < 0 );

      return GetBitFromVar( ( int )sOptions, bitPos );
    }
    /// <summary>
    /// Returns value of the single bit from byte.
    /// </summary>
    /// <param name="usOptions">UInt16 to get bit value from.</param>
    /// <param name="bitPos">Bit index.</param>
    /// <returns>Value of the single bit from byte.</returns>
    [ CLSCompliant( false ) ]
    public static bool GetBitFromVar( ushort usOptions, int bitPos )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_SHORT )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 15." );

      return GetBitFromVar( ( int )usOptions, bitPos );
    }
    /// <summary>
    /// Returns value of the single bit from byte.
    /// </summary>
    /// <param name="iOptions">Byte to get bit value from.</param>
    /// <param name="bitPos">Bit index.</param>
    /// <returns>Value of the single bit from byte.</returns>
    public static bool GetBitFromVar( int iOptions, int bitPos )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_INT )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 31." );

      if( bitPos == DEF_BITS_IN_INT - 1 ) return ( iOptions < 0 );

      return ( iOptions & ( 1 << bitPos ) ) != 0;
    }

    /// <summary>
    /// Returns value of the single bit from UInt32.
    /// </summary>
    /// <param name="uiOptions">UInt32 to get bit value from.</param>
    /// <param name="bitPos">Bit index.</param>
    /// <returns>Value of the single bit from byte.</returns>
    [CLSCompliant( false )]
    public static bool GetBitFromVar( uint uiOptions, int bitPos )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_INT )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 31." );

      return ( uiOptions & ( 1 << bitPos ) ) != 0;
    }
    /// <summary>
    /// Sets one bit in specified Int32.
    /// </summary>
    /// <param name="iValue">Int32 to set bit.</param>
    /// <param name="bitPos">Bit position in the byte</param>
    /// <param name="value">Value of bit</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7
    /// </exception>
    /// <returns>Value after</returns>
    public static int SetBit( int iValue, int bitPos, bool value )
    {
      if( bitPos < 0 || bitPos >= DEF_BITS_IN_INT )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 32." );

      if( bitPos == DEF_BITS_IN_INT - 1 )
      {
        iValue = Math.Abs( iValue );

        if( !value ) iValue = -iValue;
      }
      else if( value )
      {
        iValue |= ( 1 << bitPos );
      }
      else
      {
        iValue &= ( ~( 1 << bitPos ) );
      }

      return iValue;
    }
    /// <summary>
    /// Reads data array from another data array.
    /// </summary>
    /// <param name="arrSource">Source array.</param>
    /// <param name="iOffset">Source offset.</param>
    /// <param name="arrDest">Destination array.</param>
    /// <returns>Updated source offset.</returns>
    public static int ReadArray( byte[] arrSource, int iOffset, byte[] arrDest )
    {
      if( arrSource == null )
        throw new ArgumentNullException( "arrSource" );

      if( arrDest == null )
        throw new ArgumentNullException( "arrDest" );

      int iLength = arrDest.Length;
      Buffer.BlockCopy( arrSource, iOffset, arrDest, 0, iLength );

      return iOffset + iLength;
    }
    #endregion
  }

  #region Internal Comparer class
  /// <summary>
  /// Class used for sorting. Compare method organizes the array in special
  /// order: first the large fields, followed by one bit, and then the bit fields.
  /// Such an order is required by the auto extraction algorithm.
  /// </summary>
  internal class RecordsPosComparer : IComparer, IComparer<BiffRecordPosAttribute>
  {
    /// <summary>
    /// Compares two objects and returns a value indicating whether
    /// one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">First object to compare.</param>
    /// <param name="y">Second object to compare.</param>
    /// <returns>
    /// Value less than zero if x is less than y,
    /// zero                 if x equals y,
    /// greater than zero    if x is greater than y
    /// </returns>
    public int Compare( object x, object y )
    {
      BiffRecordPosAttribute attr1 = ( BiffRecordPosAttribute )x;
      BiffRecordPosAttribute attr2 = ( BiffRecordPosAttribute )y;

      int retValue = attr1.Position.CompareTo( attr2.Position );

      if( retValue == 0 && ( attr1.IsBit || attr2.IsBit ) )
      {
        if( attr1.IsBit && !attr2.IsBit ) return 1;
        if( !attr1.IsBit && attr2.IsBit ) return -1;

        if( attr1.IsBit && attr2.IsBit )
        {
          return attr1.SizeOrBitPosition.CompareTo( attr2.SizeOrBitPosition );
        }
      }

      return retValue;
    }

    #region IComparer<BiffRecordPosAttribute> Members

    public int Compare( BiffRecordPosAttribute x, BiffRecordPosAttribute y )
    {
      int retValue = x.Position.CompareTo( y.Position );

      if( retValue == 0 && ( x.IsBit || y.IsBit ) )
      {
        if( x.IsBit && !y.IsBit )
          return 1;
        if( !x.IsBit && y.IsBit )
          return -1;

        if( x.IsBit && y.IsBit )
        {
          return x.SizeOrBitPosition.CompareTo( y.SizeOrBitPosition );
        }
      }

      return retValue;
    }

    #endregion
  }
  #endregion

  #region internal Pair class
  /// <summary>
  /// This internal class that is used for automatic extraction and
  /// save of the Biff records.
  /// </summary>
  internal class ReflectionCachePair
  {
    #region class members
    /// <summary>
    /// Key value of the pair.
    /// </summary>
    private BiffRecordPosAttribute[] m_key;
    /// <summary>
    /// Tag value of the pair.
    /// </summary>
    private FieldInfo[] m_tag;
    #endregion

    #region class events
    /// <summary>
    /// Event that will be raised after key value changes.
    /// </summary>
    public event EventHandler KeyChanged;
    /// <summary>
    /// Event that will be raised after tag value changes.
    /// </summary>
    public event EventHandler TagChanged;
    #endregion

    #region class properties
    /// <summary>
    /// Gets / sets key value.
    /// </summary>
    public BiffRecordPosAttribute[] Key
    {
      get
      {
        return m_key;
      }
      set
      {
        if( value != m_key )
        {
          m_key = value;
          OnKeyChanged();
        }
      }
    }

    /// <summary>
    /// Gets / sets tag value.
    /// </summary>
    public FieldInfo[] Tag
    {
      get
      {
        return m_tag;
      }
      set
      {
        if( value != m_tag )
        {
          m_tag = value;
          OnTagChanged();
        }
      }
    }
    #endregion

    #region class event raisers
    /// <summary>
    /// This method is called when key value is changed.
    /// </summary>
    private void OnKeyChanged()
    {
      if( KeyChanged != null )
      {
        KeyChanged( this, EventArgs.Empty );
      }
    }

    /// <summary>
    /// This method is called when tag value is changed.
    /// </summary>
    private void OnTagChanged()
    {
      if( TagChanged != null )
      {
        TagChanged( this, EventArgs.Empty );
      }
    }
    #endregion

    #region class constructors
    /// <summary>
    /// Default constructor. To prevent construction without parameters.
    /// </summary>
    private ReflectionCachePair()
    {
    }

    /// <summary>
    /// Constructs pair by key and tag values.
    /// </summary>
    /// <param name="key">Key value.</param>
    /// <param name="tag">Tag value.</param>
    public ReflectionCachePair( BiffRecordPosAttribute[] key, FieldInfo[] tag )
    {
      m_key = key;
      m_tag = tag;
    }
    #endregion
  }
  #endregion

  #region Internal utility classes
  /// <summary>
  /// Helper class which allows extract continue records from stream.
  /// </summary>
  [ CLSCompliant( false ) ]
  public class ContinueRecordExtractor : IEnumerator
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private static readonly int[] DEF_RECORDS = new int[] { ( int )TBIFFRecord.Continue };
    #endregion

    #region Class members
    /// <summary>
    /// Reference on reader.
    /// </summary>
    private BinaryReader    m_tmpReader;
    /// <summary>
    /// Reference on start position in stream.
    /// </summary>
    private long            m_lStartPos;
    /// <summary>
    /// Current record extracted from stream.
    /// </summary>
    //private ContinueRecord  m_continue;
    private BiffRecordRaw   m_continue;
    /// <summary>
    /// Indicate whether the Reset method needs to be called.
    /// </summary>
    private bool            m_bReset;
    /// <summary>
    /// 
    /// </summary>
    private List<int> m_arrAllowedRecords = new List<int>( DEF_RECORDS );
    /// <summary>
    /// 
    /// </summary>
    private byte[] arrBuffer = new byte[ 4096 ];
    /// <summary>
    /// 
    /// </summary>
    private DataProvider m_provider;
    /// <summary>
    /// Object that can be used to decrypt record's content.
    /// </summary>
    private IDecryptor m_decryptor;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Disable access to default constructor for users.
    /// </summary>
    private ContinueRecordExtractor()
    {
    }

    /// <summary>
    /// Main constructor for class.
    /// </summary>
    /// <param name="reader">Reference on stream BinaryReader.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <param name="provider">Represents data provider.</param>
    public ContinueRecordExtractor( BinaryReader reader, IDecryptor decryptor, DataProvider provider )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      m_tmpReader = reader;
      m_lStartPos = reader.BaseStream.Position;
      m_provider = provider;
      m_decryptor = decryptor;
    }

    #endregion

    #region Class helper methods/properties
    /// <summary>
    /// Check if is the end of stream.
    /// </summary>
    protected bool          IsStreamEOF
    {
      get
      {
        if( m_tmpReader == null )
          throw new ArgumentNullException( "m_tmpReader" );

        try
        {
          if( m_tmpReader.BaseStream.Position == m_tmpReader.BaseStream.Length )
            return true;
          else if( PeekRecord() == null )
            return true;
          else
            return false;
        }
        catch( Exception ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "EOF Exception" );
          return true;
        }
      }
    }

    /// <summary>
    /// Peek at one record from stream.
    /// </summary>
    /// <returns>Peeked Biff record.</returns>
    protected BiffRecordRaw PeekRecord()
    {
      if( m_tmpReader == null )
        throw new ArgumentNullException( "m_tmpReader" );

      long lPos = m_tmpReader.BaseStream.Position;
      BiffRecordRaw record = BiffRecordFactory.GetUntypedRecord( m_tmpReader );
      m_tmpReader.BaseStream.Position = lPos;

      return record;
    }

    /// <summary>
    /// Type safe Current record.
    /// </summary>
    public /*ContinueRecord*/BiffRecordRaw   Current
    {
      get
      {
        if( m_tmpReader == null )
          throw new ArgumentNullException( "m_tmpReader" );

        if( m_continue == null || m_bReset == false )
          throw new ArgumentException( "First call Reset method and then MoveNext. Wrong enumerator initialization." );

        return m_continue;
      }
    }
    /// <summary>
    /// Class to store current stream position as start point.
    /// </summary>
    /// <returns>New position which stored as start point.</returns>
    public long             StoreStreamPosition()
    {
      m_lStartPos = m_tmpReader.BaseStream.Position;

      m_continue = null;
      m_bReset = false;

      return m_lStartPos;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="recordType"></param>
    public void             AddRecordType( TBIFFRecord recordType )
    {
      int iRecordType = ( int )recordType;

      if( m_arrAllowedRecords.IndexOf( iRecordType ) == -1 )
        m_arrAllowedRecords.Add( iRecordType );
    }
    #endregion

    #region IEnumerator Members
    /// <summary>
    /// Return stream to the start position and reset enumerator.
    /// </summary>
    void   IEnumerator.Reset()
    {
      if( m_tmpReader == null )
        throw new ArgumentNullException( "m_tmpReader" );

      m_tmpReader.BaseStream.Position = m_lStartPos;
      m_bReset = true;
    }

    /// <summary>
    /// Get current extracted record from stream.
    /// </summary>
    object IEnumerator.Current
    {
      get
      {
        if( m_tmpReader == null )
          throw new ArgumentNullException( "m_tmpReader" );

        if( m_continue == null || m_bReset == false )
          throw new ArgumentException( "First call Reset method and then MoveNext. Wrong enumerator initialization." );

        return m_continue;
      }
    }

    /// <summary>
    /// Move to the next item in stream.
    /// </summary>
    /// <returns>True if record was extracted successfully; otherwise False.</returns>
    bool   IEnumerator.MoveNext()
    {
      if( m_tmpReader == null )
        throw new ArgumentNullException( "m_tmpReader" );

      if( !IsStreamEOF )
      {
        int tag = m_tmpReader.ReadInt16();//BiffRecordFactory.ExtractRecordType( m_tmpReader );
        int length = m_tmpReader.ReadInt16();

        if( m_arrAllowedRecords.IndexOf( tag ) != -1 )// tag == TBIFFRecord.Continue )
        {
          //m_continue = ( ContinueRecord )BiffRecordFactory.GetRecord( m_tmpReader );
          m_continue = BiffRecordFactory.GetRecord( tag );//, m_tmpReader, m_provider, null, arrBuffer );
          m_continue.Length = length;
          byte[] arrData = m_tmpReader.ReadBytes( length );

          if( m_decryptor != null )
          {
            ByteArrayDataProvider provider = new ByteArrayDataProvider( arrData );
            m_decryptor.Decrypt( provider, 0, length, m_tmpReader.BaseStream.Position - length );
          }

          m_continue.Data = arrData;
          m_bReset = true;
          return true;
        }
        else
        {
          m_tmpReader.BaseStream.Position -= 4;
        }
      }

      m_continue = null;
      m_bReset = false;
      return false;
    }
    #endregion
  }

  /// <summary>
  /// Special class for data publishing as Continue Records.
  /// </summary>
  internal class ContinueRecordPublisher
  {
    #region Class members
    /// <summary>
    /// Reference to writer
    /// </summary>
    private BinaryWriter m_writer;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Constructs class and sets reference to a writer.
    /// </summary>
    /// <param name="writer">
    /// Writer for which utility class will be constructed.
    /// </param>
    public ContinueRecordPublisher( BinaryWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      m_writer = writer;
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Publish / save data as Continue Records into writer stream.
    /// </summary>
    /// <param name="data">Data which must be published as Continue Records.</param>
    /// <param name="start">Start point in an array.</param>
    /// <returns>Size of records and data in bytes.</returns>
    public int PublishContinue( byte[] data, int start )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      return PublishContinue( data, start, data.Length - start,
        BiffRecordRaw.DEF_RECORD_MAX_SIZE );
    }
    /// <summary>
    /// Publish / save data as Continue Records into writer stream.
    /// </summary>
    /// <param name="data">Data which must be published as Continue Records.</param>
    /// <param name="start">Start point in an array.</param>
    /// <param name="length">Length of data from start point.</param>
    /// <returns>Size of records and data in bytes.</returns>
    public int PublishContinue( byte[] data, int start, int length )
    {
      return PublishContinue( data, start, length, BiffRecordRaw.DEF_RECORD_MAX_SIZE );
    }
    /// <summary>
    /// Publish / save data as Continue Records into writer stream.
    /// </summary>
    /// <param name="data">Data which must be published as Continue Records.</param>
    /// <param name="start">Start point in an array.</param>
    /// <param name="length">Length of data from start point.</param>
    /// <param name="maxSize">Maximum size of Continue Record size.</param>
    /// <returns>Size of records and data in bytes.</returns>
    public int PublishContinue( byte[] data, int start, int length, int maxSize )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( data.Length < start || start < 0 )
        throw new ArgumentOutOfRangeException( "start" );

      if( data.Length < start + length || length < 0 )
        throw new ArgumentOutOfRangeException( "length" );

      if( maxSize < 0 || maxSize > BiffRecordRaw.DEF_RECORD_MAX_SIZE )
        throw new ArgumentOutOfRangeException( "maxSize" );

      int retCount = 0;
      int endPoint = start + length;
      int i = start;

      for( ; i < endPoint; i += maxSize )
      {
        int iLen = ( endPoint - i < maxSize ) ? endPoint - i : maxSize;
        m_writer.Write( ( ushort )TBIFFRecord.Continue );
        m_writer.Write( ( ushort )iLen );
        m_writer.Write( data, i, iLen );

        retCount += iLen + 4;
      }

      if( endPoint > i )
      {
        int iLen = endPoint - i;

        m_writer.Write( ( ushort )TBIFFRecord.Continue );
        m_writer.Write( ( ushort )iLen );
        m_writer.Write( data, i, iLen );
        retCount += 4 + iLen;
      }

      return retCount;
    }

    /// <summary>
    /// Publish Continue Record into internal data array of destination record.
    /// </summary>
    /// <param name="data">Data to publish.</param>
    /// <param name="start">Start point.</param>
    /// <param name="destination">Destination.</param>
    /// <param name="offset">Offset in the destination's data array.</param>
    /// <returns>Size of the published data.</returns>
    public int PublishContinue( byte[] data, int start, BiffRecordRawWithArray destination, int offset )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      return PublishContinue( data, start, data.Length - start,
        BiffRecordRaw.DEF_RECORD_MAX_SIZE, destination, offset );
    }
    /// <summary>
    /// Publish Continue Record into internal data array of destination record.
    /// </summary>
    /// <param name="data">Data to publish.</param>
    /// <param name="start">Start point.</param>
    /// <param name="length">Length of data to publish.</param>
    /// <param name="destination">Destination.</param>
    /// <param name="offset">Offset in the destination's data array.</param>
    /// <returns>Size of the published data.</returns>
    public int PublishContinue( byte[] data, int start, int length
      , BiffRecordRawWithArray destination, int offset )
    {
      return PublishContinue( data, start, length,
        BiffRecordRaw.DEF_RECORD_MAX_SIZE, destination, offset );
    }
    /// <summary>
    /// Publish Continue Record into internal data array of destination record.
    /// </summary>
    /// <param name="data">Data to publish.</param>
    /// <param name="start">Start point.</param>
    /// <param name="length">Length of data to publish.</param>
    /// <param name="maxSize">Max size for continue record.</param>
    /// <param name="destination">Destination.</param>
    /// <param name="offset">Offset in the destination's data array.</param>
    /// <returns>Size of the published data.</returns>
    public int PublishContinue( byte[] data, int start, int length, int maxSize
      , BiffRecordRawWithArray destination, int offset )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( data.Length < start || start < 0 )
        throw new ArgumentOutOfRangeException( "start" );

      if( data.Length < start + length || length < 0 )
        throw new ArgumentOutOfRangeException( "length" );

      if( maxSize < 0 || maxSize > BiffRecordRaw.DEF_RECORD_MAX_SIZE )
        throw new ArgumentOutOfRangeException( "maxSize" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );

      int retCount = 0;
      int endPoint = start + length;
      int i = start;

      bool bOld = destination.AutoGrowData;
      destination.AutoGrowData = true;

      for( ; i < endPoint; i += maxSize )
      {
        int iLen = ( endPoint - i < maxSize ) ? endPoint - i : maxSize;
        destination.SetUInt16( offset, (ushort)TBIFFRecord.Continue );
        destination.SetUInt16( offset + 2, (ushort)iLen );
        destination.SetBytes( offset + 4, data, i, iLen );

        retCount += iLen + 4;
        offset += iLen + 4;
      }

      if( endPoint > i )
      {
        int iLen = endPoint - i;

        destination.SetUInt16( offset, (ushort)TBIFFRecord.Continue );
        destination.SetUInt16( offset + 2, (ushort)iLen );
        destination.SetBytes( offset + 4, data, i, iLen );

        retCount += iLen + 4;
        offset += iLen + 4;
      }

      destination.AutoGrowData = bOld;

      return retCount;
    }
    #endregion
  }

  ///<exclude/>
  /// <summary>
  /// Special class for building Continue Records.
  /// </summary>
  [ CLSCompliant( false ) ]
  public class ContinueRecordBuilder
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    //protected BiffRecordRawWithArray m_parent;
    protected BiffContinueRecordRaw m_parent;
    /// <summary>
    /// 
    /// </summary>
    protected int m_iPos;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinuePos = -1;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinueSize;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTotal;
    /// <summary>
    /// 
    /// </summary>
    protected int m_iMax;
    /// <summary>
    /// 
    /// </summary>
    private TBIFFRecord m_firstContinueType = TBIFFRecord.Continue;
    /// <summary>
    /// Type of additional records.
    /// </summary>
    private TBIFFRecord m_continueType = TBIFFRecord.Continue;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinueCount = 0;
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns the unused bytes.
    /// </summary>
    public int FreeSpace
    {
      get
      {
        return m_iMax - m_iContinueSize;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Total
    {
      get
      {
        return m_iTotal;
      }
      set
      {
        m_iTotal = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Position
    {
      get
      {
        return m_iPos;
      }
      set
      {
        m_iPos = value;
      }
    }

    /// <summary>
    /// Return maximum size of record.
    /// </summary>
    public int Max
    {
      get
      {
        return m_iMax;
      }
    }
    /// <summary>
    /// Type of the first additional record.
    /// </summary>
    public TBIFFRecord FirstContinueType
    {
      get
      {
        return m_firstContinueType;
      }
      set
      {
        m_firstContinueType = value;
      }
    }
    /// <summary>
    /// Type of additional records.
    /// </summary>
    public TBIFFRecord ContinueType
    {
      get
      {
        return m_continueType;
      }
      set
      {
        m_continueType = value;
      }
    }
    /// <summary>
    /// Maximum size of the continue record data.
    /// </summary>
    public virtual int MaximumSize
    {
      get
      {
        //return BiffRecordRaw.DEF_RECORD_MAX_SIZE;// -HeaderFooterImageRecord.DEF_DATA_OFFSET;
          return BiffRecordRaw.DEF_RECORD_MAX_SIZE - HeaderFooterImageRecord.DEF_DATA_OFFSET;
      }
    }
    #endregion

    #region Class Events
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler OnFirstContinue;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public ContinueRecordBuilder( /*BiffRecordRawWithArray*/BiffContinueRecordRaw parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
      m_iMax = m_parent.MaximumRecordSize;

      //m_iContinuePos = 2;

      m_iContinueSize = m_parent.Length;
      m_iPos = m_iContinueSize; // Set offset position in data array.
      m_iTotal = m_iContinueSize;
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void AppendByte( byte value )
    {
      if( CheckIfSpaceNeeded( 1 ) )
      {
        UpdateContinueRecordSize();
        StartContinueRecord();
      }

      m_parent.SetByte( m_iPos, value );
      UpdateCounters( 1 );

      //UpdateContinueRecordSize();
    }

    /// <summary>
    /// Write array of data into output stream.
    /// </summary>
    /// <param name="data">Array of data.</param>
    /// <param name="start">Start index of an array.</param>
    /// <param name="length">Length of data to copy.</param>
    /// <returns>Quantity of created Continue Records.</returns>
    public virtual int AppendBytes( byte[] data, int start, int length )
    {
      int counter = 0;

      // If data array is too large, then save it by parts.
      if( CheckIfSpaceNeeded( length ) )
      {
        int endPoint = start + length;
        int i = start;

        for( ; i < endPoint; i += m_iMax )
        {
          UpdateContinueRecordSize();
          StartContinueRecord();
          counter++;         

          int iLen = ( endPoint - i < m_iMax ) ? endPoint - i : m_iMax;
          m_parent.SetBytes( m_iPos, data, i, iLen );
          UpdateCounters( iLen );
        }
      }
      else
      {
        m_parent.SetBytes( m_iPos, data, start, length );
        UpdateCounters( length );
      }

      UpdateContinueRecordSize();

      return counter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void AppendUInt16( ushort value )
    {
      if( CheckIfSpaceNeeded( 2 ) )
      {
        UpdateContinueRecordSize();
        StartContinueRecord();
      }

      m_parent.SetUInt16( m_iPos, value );
      UpdateCounters( 2 );

      //UpdateContinueRecordSize();
    }

    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Method that checks if Continue Record is needed.
    /// </summary>
    /// <param name="length">Length of data that needs to be stored.</param>
    /// <returns>True if Continue Record will be needed for data storage;
    /// otherwise False.</returns>
    public bool CheckIfSpaceNeeded( int length )
    {
      return ( m_iContinueSize + length > m_iMax );
    }
    /// <summary>
    /// 
    /// </summary>
    public void StartContinueRecord()
    {
      if( OnFirstContinue != null )
      {
        OnFirstContinue( this, EventArgs.Empty );
      }

      m_iContinueCount++;
      m_parent.m_arrContinuePos.Add( m_iPos );
      TBIFFRecord recordType = ( m_iContinueCount == 1 )
        ? FirstContinueType
        : ContinueType;

      //m_parent.m_arrCon
      m_parent.SetUInt16( m_iPos, ( ushort )recordType );

      m_iPos += 2;
      m_iContinuePos = m_iPos;
      m_iContinueSize = 0;

      m_parent.SetUInt16( m_iPos, ( ushort )m_iContinueSize );

      m_iPos += 2;
      m_iTotal += 4;

      // Set maximum to continue record max Size.
      //m_iMax = BiffRecordRaw.DEF_RECORD_MAX_SIZE;// MaximumSize;
      m_iMax = MaximumSize;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateContinueRecordSize()
    {
      if( m_iContinuePos >= 0 )
      {
        m_parent.SetUInt16( m_iContinuePos, ( ushort )m_iContinueSize );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iLen"></param>
    protected void UpdateCounters( int iLen )
    {
      m_iPos += iLen;
      m_iTotal += iLen;
      m_iContinueSize += iLen;
    }
    #endregion
  }
  #endregion
}
