#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for BiffRecordRawWithArray.
	/// </summary>
	[ CLSCompliant( false ) ]
	public abstract class BiffRecordRawWithArray
    : BiffRecordRaw
    , IDisposable
	{
    #region Class members
    /// <summary>
    /// Array that contains record data.
    /// </summary>
    internal protected  byte[]  m_data;// = new byte[]{};
    /// <summary>
    /// True if internal data array will be automatically resized
    /// if there is not enough space for record data.
    /// </summary>
    private   bool    m_bAutoGrow;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, gets code value using reflection and attributes.
    /// </summary>
    protected  BiffRecordRawWithArray()
    {
      if( NeedDataArray ) m_data = new byte[ 0 ];
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    protected  BiffRecordRawWithArray( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="reader">BinaryReader from which record data should be read.</param>
    /// <param name="itemSize">Size of the read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is NULL.
    /// </exception>
    protected  BiffRecordRawWithArray( BinaryReader reader, out int itemSize )
      : base( reader, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    protected  BiffRecordRawWithArray( int iReserve )
      : base( iReserve )
    {
      if( iReserve < 0 )
        throw new ArgumentOutOfRangeException( "iReserve", "Reserved memory count must be greater than zero." );

      m_data = new byte[ iReserve ];
    }
    /// <summary>
    /// 
    /// </summary>
    ~BiffRecordRawWithArray()
    {
      Dispose();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Returns record data.
    /// </summary>
    public override byte[] Data
    {
      get
      {
        if( NeedInfill )
        {
          InfillInternalData( ExcelVersion.Excel97to2003 );
        }

        return m_data;
      }
      set
      {
        if( value != null )
          m_data = value;
      }
    }
    /// <summary>
    /// If True, the array will automatically grow when the offset limit 
    /// is reached. This is required when the real record size is not known for an
    /// Infill operation. Will throw exception on buffer offset overrun
    /// when set to False. Default value is False.
    /// </summary>
    public override bool AutoGrowData
    {
      get
      {
        return m_bAutoGrow;
      }
      set
      {
        m_bAutoGrow = value;
      }
    }
    #endregion

    #region Class methods
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
    public override int FillStream( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      return FillStream( writer, encryptor, streamPosition );
    }
    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream, used to reduce Flush
    /// calls of the writer.BaseStream.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    public virtual int FillStream( BinaryWriter writer, IEncryptor encryptor, int streamPosition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( NeedInfill )
      {
        InfillInternalData( ExcelVersion.Excel97to2003 );
      }
      else
      {
        NeedInfill = true;
      }

      if( m_iLength < 0 )
        throw new ApplicationException( "Wrong Record data infill. " + TypeCode.ToString() );

      writer.Write( ( ushort )m_iCode );
      writer.Write( ( ushort )m_iLength );
      streamPosition += DEF_HEADER_SIZE;

      int iLength = ( m_data == null ) ? 0 : m_data.Length;

      if( iLength < m_iLength )
        throw new ApplicationException( "Length of data is greater than internal storage contains." +
          "Object Type is " + this.GetType().Name );

      if( iLength > 0 )
      {
        if( encryptor != null )
        {
          int iOffset = StartDecodingOffset;
          encryptor.Encrypt( m_data, iOffset, m_iLength - iOffset, streamPosition + iOffset );
        }

        writer.Write( m_data, 0, m_iLength );
      }

      return ( int )( m_iLength + DEF_HEADER_SIZE );
    }
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_data = new byte[ iLength ];
      m_iLength = iLength;
      provider.CopyTo( iOffset, m_data, 0, iLength );
      ParseStructure();

      if( !NeedDataArray )
      {
        m_data = new byte[ 0 ];
        AutoGrowData = true;
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into
    /// an internal Data array: m_data. This method is called by
    /// FillStream, when the record must be serialized into stream.
    /// </summary>
    public abstract void InfillInternalData( ExcelVersion version );
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      InfillInternalData( ExcelVersion.Excel97to2003 );

      if( m_iLength > 0 )
      {
        //Buffer.BlockCopy( m_data, 0, arrBuffer, iOffset, m_iLength );
        provider.WriteBytes( iOffset, m_data, 0, m_iLength );
      }
      //throw new NotImplementedException( TypeCode.ToString() );
    }
    #endregion

    #region Class Read Helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    protected void CheckOffsetAndLength( int offset, int length )
    {
      int iLen = m_data.Length;

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
    /// <param name="offset">Offset of first byte of data to get.</param>
    /// <param name="length">Length of required array.</param>
    /// <returns>Array of bytes from internal record data.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If offset is less than zero or more than internal record data array size
    ///   or length is less than zero or more than internal record data array size
    ///   or length plus offset is more than internal record data array size.
    /// </exception>
    protected byte[] GetBytes( int offset, int length )
    {
      if( length == 0 ) return new byte[ 0 ];

      CheckOffsetAndLength( offset, length );

      byte[] retValue = new byte[ length ];

      Buffer.BlockCopy( m_data, offset, retValue, 0, length );
      return retValue;
    }

    /// <summary>
    /// Gets single byte from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of byte to get.</param>
    /// <returns>Single byte from internal record data.</returns>
    protected byte   GetByte( int offset )
    {
      CheckOffsetAndLength( offset, 1 );
      return m_data[ offset ];
    }

    /// <summary>
    /// Gets ushort from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of ushort to get.</param>
    /// <returns>Ushort from internal record data.</returns>
    protected ushort GetUInt16( int offset )
    {
      CheckOffsetAndLength( offset, 2 );
      return BitConverter.ToUInt16( m_data, offset );
    }

    /// <summary>
    /// Gets short from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of short to get.</param>
    /// <returns>Short from internal record.</returns>
    protected short  GetInt16( int offset )
    {
      CheckOffsetAndLength( offset, 2 );
      return BitConverter.ToInt16( m_data, offset );
    }

    /// <summary>
    /// Gets int from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of int to get.</param>
    /// <returns>Int from internal record.</returns>
    protected int    GetInt32( int offset )
    {
      CheckOffsetAndLength( offset, 4 );
      return BitConverter.ToInt32( m_data, offset );
    }

    /// <summary>
    /// Gets uint from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of uint to get.</param>
    /// <returns>Uint from internal record data.</returns>
    protected uint   GetUInt32( int offset )
    {
      CheckOffsetAndLength( offset, 4 );
      return BitConverter.ToUInt32( m_data, offset );
    }

    /// <summary>
    /// Gets long from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of long to get.</param>
    /// <returns>Long from internal record data.</returns>
    protected long   GetInt64( int offset )
    {
      CheckOffsetAndLength( offset, 28);
      return BitConverter.ToInt64( m_data, offset );
    }

    /// <summary>
    /// Gets ulong from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of ulong to get.</param>
    /// <returns>Ulong from internal record data.</returns>
    protected ulong  GetUInt64( int offset )
    {
      CheckOffsetAndLength( offset, 8 );
      return BitConverter.ToUInt64( m_data, offset );
    }

    /// <summary>
    /// Gets float from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of float to get.</param>
    /// <returns>Float from internal record data.</returns>
    protected float  GetFloat( int offset )
    {
      CheckOffsetAndLength( offset, 4 );
      return BitConverter.ToSingle( m_data, offset );
    }

    /// <summary>
    /// Gets double from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset in bytes of double to get.</param>
    /// <returns>Double from internal record data.</returns>
    protected double GetDouble( int offset )
    {
      CheckOffsetAndLength( offset, 8 );
      return BitConverter.ToDouble( m_data, offset );
    }

    /// <summary>
    /// Gets single bit from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of the byte that contains needed bit.</param>
    /// <param name="bitPos">Position of bit in the byte.</param>
    /// <returns>True if specified bit is set to 1; otherwise False.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///  bitPos argument should be not less than zero and not more than 7.
    /// </exception>
    protected bool   GetBit( int offset, int bitPos )
    {
      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zeroless or greater than 7." );

      CheckOffsetAndLength( offset, 1 );

      return ( m_data[ offset ] & ( 1 << bitPos ) ) != 0;// ( 1 << bitPos );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes,
    /// and it increases the offset by string size in bytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="asciiString">Indicates whether extracted string is ascii string (not unicode).</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected string GetString16BitUpdateOffset( ref int offset, out bool asciiString )
    {
      int iStrLen = GetUInt16( offset );
      offset += 2;
      asciiString = false;

      if( iStrLen > 0 )
      {
        int iBytes;
        string result = GetString( offset, iStrLen, out iBytes );
        offset += iBytes + 1;
        asciiString = iBytes == iStrLen;
        return result;
      }

      offset++;

      return string.Empty;
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes,
    /// and it increases the offset by string size in bytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected string GetString16BitUpdateOffset( ref int offset )
    {
      bool bAscii;
      return GetString16BitUpdateOffset( ref offset, out bAscii );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes
    /// and it increases offset by string size in bytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected string GetStringUpdateOffset( ref int offset, int iStrLen )
    {
      if( iStrLen > 0 )
      {
        int iBytes;
        string result = GetString( offset, iStrLen, out iBytes );
        offset += iBytes + 1;
        return result;
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected string GetStringByteLen( int offset )
    {
      int iLen = m_data[ offset ];//GetByte( offset );
      return GetString( offset + 1, iLen );
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iBytes">Returns size of the read data.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected string GetStringByteLen( int offset, out int iBytes )
    {
      int iLen = m_data[ offset ];//GetByte( offset );
      return GetString( offset + 1, iLen, out iBytes );
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected internal string GetString( int offset, int iStrLen )
    {
      int Bytes;
      return GetString( offset, iStrLen, out Bytes );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <returns>Retrieved string.</returns>
    protected internal string GetString( int offset, int iStrLen, out int iBytesInString )
    {
      return GetString( offset, iStrLen, out iBytesInString, false );
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <param name="isByteCounted">Flag for is bytes count available.</param>
    /// <returns>Retrieved string.</returns>
    protected internal string GetString( int offset, int iStrLen, out int iBytesInString, bool isByteCounted )
    {
      byte iCompressUnicode = m_data[ offset ];//GetByte( offset );
      int  iLast = ( iCompressUnicode != 0 && !isByteCounted ) ? 2 * iStrLen : iStrLen;

      iLast += offset + 1;

      if( iLast > m_iLength )
        throw new WrongBiffRecordDataException( 
          string.Format( "String and m_data array do not fit each other {0}.", TypeCode ));

      if( iCompressUnicode == 0 )
      {
        iBytesInString = iStrLen;
        CheckOffsetAndLength( offset + 1, iStrLen );
        return BiffRecordRaw.LatinEncoding.GetString( m_data, offset + 1, iStrLen );
      }
      else
      {
        iBytesInString = isByteCounted ? iStrLen : iStrLen * 2;
        CheckOffsetAndLength( offset + 1, iBytesInString );
        return Encoding.Unicode.GetString( m_data, offset + 1, iBytesInString );
        //return Encoding.Unicode.GetString( GetBytes( offset + 1, 
        //  iBytesInString ), 0, iBytesInString );
      }
    }
    /// <summary>
    /// Detect type of string and extracts it.
    /// </summary>
    /// <param name="offset">Record data offset.</param>
    /// <param name="continuePos">Contain next position.</param>
    /// <param name="continueCount">Number of elements in the continuePos collection.</param>
    /// <param name="iBreakIndex">Current index in the continuePos array.</param>
    /// <param name="length">Length of string record.</param>
    /// <param name="rich">Array of rich formatting values.</param>
    /// <param name="extended">Array of unknown FarEast data.</param>
    /// <returns>Extracted string.</returns>
    protected string GetUnkTypeString( int offset, IList<int> continuePos, int continueCount,
      ref int iBreakIndex, out int length, out byte[] rich, out byte[] extended )
    {
      string  retValue = null;//string.Empty;
      int     retLength = 3;

      rich = null;
      extended = null;

      int iCurPos = offset;

      ushort iLen = BitConverter.ToUInt16( m_data, iCurPos );//GetUInt16( iCurPos );
      byte btFlags = m_data[ iCurPos + 2 ];//GetByte( iCurPos + 2 );

      bool bIsUnicode    = ( btFlags & 0x1 ) == 1;
      bool bIsUniFarEast = ( ( btFlags & 4 ) != 0 );
      bool bIsUniRich    = ( ( btFlags & 8 ) != 0 );

      int iStrOffset = 3;
      short sRichRuns = 0;
      int iFarEastSize = 0;

      if( bIsUniRich )
      {
        sRichRuns = GetInt16( iCurPos + iStrOffset );
        iStrOffset += 2;
        retLength += 2;
      }
      
      if( bIsUniFarEast )
      {
        iFarEastSize = GetInt32( iCurPos + iStrOffset );
        iStrOffset += 4;
        retLength += 4;
      }

      // TODO: continue to fix this method.
      int iStringStart = iCurPos + iStrOffset;
      int iCurChar = 0;

      Encoding encoding = bIsUnicode
        ? Encoding.Unicode
        : BiffRecordRaw.LatinEncoding;

      while( iCurChar < iLen )
      {
        int iDesiredLen = bIsUnicode ? ( iLen - iCurChar ) * 2 : iLen - iCurChar;
        
        // Get length of string or part of it in continue record.
        int iBreakPos = FindNextBreak( continuePos, continueCount, iStringStart, ref iBreakIndex );
        int iBytesLeft  = iBreakPos - iStringStart;

        if( iDesiredLen <= iBytesLeft )
        {
          string strPart = encoding.GetString( m_data, iStringStart, iDesiredLen );
          retValue = ( retValue == null )
            ? strPart
            : retValue + strPart;

          retLength += iDesiredLen;
          break;
        }
        else
        {
          // Get part of string.
          string strPart = encoding.GetString( m_data, iStringStart, iBytesLeft );
          retValue = ( retValue == null )
            ? strPart
            : retValue + strPart;

//          retValue += encoding.GetString( m_data, iStringStart, iBytesLeft );
          iCurChar += bIsUnicode ? iBytesLeft / 2 : iBytesLeft;

          if( m_data[ iStringStart + iBytesLeft ] == 0
            || m_data[ iStringStart + iBytesLeft ] == 1 )
          {
            bIsUnicode = ( m_data[ iStringStart + iBytesLeft ] == 1 );

            encoding = bIsUnicode
              ? Encoding.Unicode
              : BiffRecordRaw.LatinEncoding;

            iStringStart++;
            retLength++;
          }

          iStringStart += iBytesLeft;
          retLength += iBytesLeft;
        }
      }

      if( bIsUniRich )
      {
        int iSize = sRichRuns * 4;
        rich = GetBytes( offset + retLength, iSize );
        retLength += iSize;
      }

      if( bIsUniFarEast )
      {
        extended = GetBytes( offset + retLength, iFarEastSize );
        retLength += iFarEastSize;
      }

      length = retLength;

      return ( retValue != null )
        ? retValue
        : string.Empty;
    }
    /// <summary>
    /// Gets TAddr structure from internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to get.</param>
    /// <returns>Retrieved TAddr structure.</returns>
    protected TAddr  GetAddr( int offset )
    {
      TAddr addr = new TAddr();

      addr.FirstRow = GetUInt16( offset );
      addr.LastRow  = GetUInt16( offset + 2 );
      addr.FirstCol = GetUInt16( offset + 4 );
      addr.LastCol  = GetUInt16( offset + 6 );

      return addr;
    }
    /// <summary>
    /// Gets TAddr structure from internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to get.</param>
    /// <returns>Retrieved TAddr structure.</returns>
    protected Rectangle  GetAddrAsRectangle( int offset )
    {
      int iFirstRow = GetUInt16( offset );
      int iLastRow  = GetUInt16( offset + 2 );
      int iFirstCol = GetUInt16( offset + 4 );
      int iLastCol  = GetUInt16( offset + 6 );

      return Rectangle.FromLTRB( iFirstCol, iFirstRow, iLastCol, iLastRow );
    }
    #endregion

    #region Class Write Helper methods
    /// <summary>
    /// Enlarges the internal record data array to facilitate 
    /// at least offset + length of data to be placed.
    /// </summary>
    /// <param name="offset">Offset to the accessed data.</param>
    /// <param name="length">Length of the needed data.</param>
    protected   void EnlargeDataStorageIfNeeded( int offset, int length )
    {
      if( m_data == null || offset + length > m_data.Length )
      {
        int iLength = ( m_data == null ) ? 0 : m_data.Length;
        int iNewSize = Math.Min( offset * 2 + length + 16, MaximumMemorySize );
        // Make reservation for data plus 16 reserved bytes.
        //byte[] buffer = new byte[ offset + length + 16 ];
        if( iNewSize > iLength )
        {
          byte[] buffer = new byte[ iNewSize ];

          if( iLength > 0 )
            Buffer.BlockCopy( m_data, 0, buffer, 0, iLength );

          m_data = buffer;
        }
      }
    }
    /// <summary>
    /// Reserve memory for internal array.
    /// </summary>
    /// <param name="length">Number of bytes that should be reserved.</param>
    internal protected void Reserve( int length )
    {
      if( m_data.Length > length ) return;

      byte[] buffer = new byte[ length ];
      Buffer.BlockCopy( m_data, 0, buffer, 0, m_data.Length );
      m_data = buffer;
    }

    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
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
    internal protected void SetBytes( int offset, byte[] value, int pos, int length )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( pos < 0 )
        throw new ArgumentOutOfRangeException( "pos", "Position cannot be zeroless." );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length", "Length of data to copy must be greater then zero." );

      if( pos + length > value.Length )
        throw new ArgumentOutOfRangeException( "value", "Position or length has wrong value." );

      if( AutoGrowData )
      {
        EnlargeDataStorageIfNeeded( offset, length );
      }
      else
      {
        if( offset + length > m_data.Length )
          throw new ArgumentOutOfRangeException( "m_data", "Internal array size is too small." );
      }

      Buffer.BlockCopy( value, pos, m_data, offset, length );
    }

    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Array of bytes to set.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If value array is NULL.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If pos or length would be less than zero  or their sum would be more
    ///   than size of value array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If internal record data array is too small for receiving value array
    ///   and AutoGrowData is False.
    /// </exception>
    internal protected void SetBytes( int offset, byte[] value )
    {
      SetBytes( offset, value, 0, value.Length );
    }

    /// <summary>
    /// Sets single byte in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required byte.</param>
    /// <param name="value">New value for the byte.</param>
    internal protected void SetByte( int offset, byte value )
    {
      //SetBytes( offset, new byte[]{ value }, 0, 1 );
      if( AutoGrowData )
      {
        EnlargeDataStorageIfNeeded( offset, 1 );
      }

      m_data[ offset ] = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="value"></param>
    /// <param name="count"></param>
    internal protected void SetByte( int offset, byte value, int count )
    {
      byte[] buffer = new byte[ count ];

      for( int i = 0; i < count; i++ )
      {
        buffer[ i ] = value;
      }

      SetBytes( offset, buffer, 0, count );
    }

    /// <summary>
    /// Sets ushort in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified ushort.</param>
    internal protected void SetUInt16( int offset, ushort value )
    {
      //SetBytes( offset, BitConverter.GetBytes( value ), 0, 2 );
      if( AutoGrowData )
        EnlargeDataStorageIfNeeded( offset, 2 );

      byte byte1 = ( byte )( value & 0xFF );
      byte byte2 = ( byte )( ( value >> 8 ) & 0xFF );

      m_data[ offset ] = byte1;
      m_data[ offset + 1 ] = byte2;
      //Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, m_data, offset, 2 );
    }

    /// <summary>
    /// Sets short in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified short.</param>
    internal protected void SetInt16( int offset, short value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 2 );
    }

    /// <summary>
    /// Sets int in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified int.</param>
    internal protected void SetInt32( int offset, int value )
    {
      //SetBytes( offset, BitConverter.GetBytes( value ), 0, 4 );
      if( AutoGrowData )
        EnlargeDataStorageIfNeeded( offset, 4 );

      Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, m_data, offset, 4 );
    }

    /// <summary>
    /// Sets uint in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified uint.</param>
    internal protected void SetUInt32( int offset, uint value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 4 );

//      if( AutoGrowData )
//      {
//        EnlargeDataStorageIfNeeded( offset, 4 );
//      }
//
//      unsafe
//      {
//        fixed( byte* numRef1 = m_data )
//        {
//          //byte* numRef2 = numRef1 + offset;
//          *( ( uint* ) numRef1 ) = value;
//        }
//      }


//      if( AutoGrowData )
//      {
//        EnlargeDataStorageIfNeeded( offset, 4 );
//      }
//
//      byte btValue;
//      for( int i = 0; i < 4; i++, offset++ )
//      {
//        btValue = ( byte )( value & 0xFF );
//        m_data[ offset ] = btValue;
//        value >>= 8;
//      }
    }

    /// <summary>
    /// Sets long in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified long.</param>
    internal protected void SetInt64( int offset, long value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 8 );
    }

    /// <summary>
    /// Sets ulong in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified ulong.</param>
    internal protected void SetUInt64( int offset, ulong value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 8 );
    }

    /// <summary>
    /// Sets float in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified float.</param>
    internal protected void SetFloat( int offset, float value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 4 );
    }

    /// <summary>
    /// Sets double in internal record data array using SetBytes method.
    /// </summary>
    /// <param name="offset">Offset to the required value.</param>
    /// <param name="value">New value for the specified double.</param>
    internal protected void SetDouble( int offset, double value )
    {
      SetBytes( offset, BitConverter.GetBytes( value ), 0, 8 );
    }

    /// <summary>
    /// Sets one bit in specified byte in internal record data array.
    /// </summary>
    /// <param name="offset">Offset of the byte in the internal record data array.</param>
    /// <param name="value">Value of bit.</param>
    /// <param name="bitPos">Bit position in the byte.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7.
    /// </exception>
    internal protected void SetBit( int offset, bool value, int bitPos )
    {
      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zero or greater than 7." );

      if( AutoGrowData )
      {
        EnlargeDataStorageIfNeeded( offset, 1 );
      }

      if( value )
      {
        m_data[ offset ] |= (byte)( 1 << bitPos );
      }
      else
      {
        m_data[ offset ] &= (byte)(~( 1 << bitPos ));
      }
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length, updates offset parameter (adds string length).
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="isCompression">Indicates whether string should be compressed or not.</param>
    internal protected void SetStringNoLenUpdateOffset( ref int offset, string value, bool isCompression )
    {
      if( value == null || value.Length == 0 )
        return;

      Encoding encoding =
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.Default;
#else
        Encoding.UTF8;
#endif

      encoding = isCompression ? encoding : Encoding.Unicode;
      byte[] tmpData = encoding.GetBytes( value );

      // we always save strings in Unicode
      byte btCompression = ( byte )( isCompression ? 0 : 1 );
      SetByte( offset, btCompression );

      // save string
      SetBytes( offset + 1, tmpData, 0, tmpData.Length );

      offset += tmpData.Length + 1;
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetStringNoLenDetectEncoding( int offset, string value )
    {
      return ( !IsAsciiString( value ) ) ?
        SetStringNoLen( offset, value ) :
        SetStringNoLen( offset, value, true, true );
    }
    /// <summary>
    /// Detects whether specified string contains only characters supported by ASCII encoding.
    /// </summary>
    /// <param name="strTextPart">String to check.</param>
    /// <returns>True if this string is fully supported by ASCII encoding.</returns>
    public static bool IsAsciiString( string strTextPart )
    {
      bool bResult = true;
      int len = ( strTextPart != null ) ?
        strTextPart.Length :
        0;

      for( int i = 0; i < len; i++ )
      {
        if( strTextPart[ i ] > 127 )
        {
          bResult = false;
          break;
        }
      }

      return bResult;
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetStringNoLen( int offset, string value )
    {
      return SetStringNoLen( offset, value, false, false );
    }
    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="bEmptyCompressed">Indicates whether write compressed attribute for empty strings.</param>
    /// <param name="bCompressed">Indicates whether to write compressed (ascii) string or not.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetStringNoLen( int offset, string value, bool bEmptyCompressed, bool bCompressed )
    {
      if( value == null || value.Length == 0 )
      {
        if( bEmptyCompressed )
        {
          SetByte( offset, 0 );
          return 1;
        }

        return 0;
      }

      Encoding encoding = bCompressed ?
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.Default :
#else
        Encoding.UTF8 :
#endif
        Encoding.Unicode;

      byte[] tmpData = encoding.GetBytes( value );

      // we always save strings in Unicode
      byte btCompression = ( byte )( bCompressed ? 0 : 1 );
      SetByte( offset, btCompression );

      // save string
      SetBytes( offset + 1, tmpData, 0, tmpData.Length );

      return tmpData.Length + 1;
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in one byte.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the string data.</returns>
    internal protected int SetStringByteLen( int offset, string value )
    {
      // save length
      SetByte( offset, (byte)value.Length );

      return SetStringNoLen( offset + 1, value ) + 1;
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetString16BitLen( int offset, string value )
    {
      // save length
      ushort usLength = ( ushort )( ( value != null )
        ? value.Length
        : 0 );
      SetUInt16( offset, usLength );

      return 2 + SetStringNoLen( offset + 2, value );
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="bEmptyCompressed">Indicates whether write compressed attribute for empty strings.</param>
    /// <param name="isCompressed">Indicates whether string should be compressed or not.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetString16BitLen( int offset, string value, bool bEmptyCompressed, bool isCompressed )
    {
      // save length
      SetUInt16( offset, (ushort)value.Length );

      return 2 + SetStringNoLen( offset + 2, value, bEmptyCompressed, isCompressed );
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in two bytes
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    internal protected void SetString16BitUpdateOffset( ref int offset, string value )
    {
      // save length
      SetUInt16( offset, (ushort)value.Length );

      offset += 2;
      SetStringNoLenUpdateOffset( ref offset, value, false );
    }
    /// <summary>
    /// Sets string in internal record data array using SetBytes method.
    /// The String length is saved in two bytes
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="isCompressed">Indicates whether string should be compressed or not.</param>
    internal protected void SetString16BitUpdateOffset( ref int offset, string value, bool isCompressed )
    {
      // save length
      SetUInt16( offset, ( ushort )value.Length );

      offset += 2;
      SetStringNoLenUpdateOffset( ref offset, value, isCompressed );
    }
    /// <summary>
    /// Sets TAddr structure in internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to set.</param>
    /// <param name="addr">New value of the TAddr at the specified position.</param>
    internal protected void SetAddr( int offset, TAddr addr )
    {
      SetUInt16( offset    , ( ushort )addr.FirstRow );
      SetUInt16( offset + 2, ( ushort )addr.LastRow );
      SetUInt16( offset + 4, ( ushort )addr.FirstCol );
      SetUInt16( offset + 6, ( ushort )addr.LastCol  );
    }
    /// <summary>
    /// Sets TAddr structure in internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to set.</param>
    /// <param name="addr">New value of the Rectangle at the specified position.</param>
    internal protected void SetAddr( int offset, Rectangle addr )
    {
      SetUInt16( offset    , ( ushort )addr.Top );
      SetUInt16( offset + 2, ( ushort )addr.Bottom  );
      SetUInt16( offset + 4, ( ushort )addr.Left );
      SetUInt16( offset + 6, ( ushort )addr.Right  );
    }
    #endregion

    #region Automatic extracting and saving
    /// <summary>
    /// Method extracts from class its fields which have the custom attribute:
    /// BiffRecordPosAttribute. Method returns two arrays sorted in special
    /// order, the order of extraction and saving from/to stream.
    /// </summary>
    /// <returns>Returns Sorted Fields.</returns>
    protected SortedList<BiffRecordPosAttribute, FieldInfo> GetSortedFields()
    {
      SortedList<BiffRecordPosAttribute, FieldInfo> result;

      if( !m_ReflectCache.TryGetValue( m_iCode, out result ) )
      {
        Type type = GetType();
        FieldInfo[] fields = type.GetFields( BindingFlags.NonPublic | BindingFlags.Instance );
        result = new SortedList<BiffRecordPosAttribute, FieldInfo>( new RecordsPosComparer() );

        for( int i = 0, len = fields.Length; i < len; i++ )
        {
          FieldInfo field = fields[ i ];
          object[] attribs =
#if ( WINRT )
              field.GetCustomAttributes( typeof( BiffRecordPosAttribute ), true ).ToArray<object>();
#else
            field.GetCustomAttributes( typeof( BiffRecordPosAttribute ), true );
#endif

          if( attribs.Length > 0 )
          {
            result.Add( ( BiffRecordPosAttribute )attribs[ 0 ], field );
          }
        }
        // Store in cache.
        m_ReflectCache[ m_iCode ] = result;
      }

      return result;
    }
    /// <summary>
    /// Algorithm extracts field values from internal data storage.
    /// </summary>
    protected void AutoExtractFields()
    {
      //      BiffRecordPosAttribute[] attr;
      //      FieldInfo[] fields;

#if SHOW_EXTRACTING
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Extracting " + this.TypeCode );
#endif

      SortedList<BiffRecordPosAttribute, FieldInfo> list = GetSortedFields();// out attr, out fields );

#if !SILVERLIGHT && !WINRT && !WP
      Debug.IndentLevel = 1;
#endif
      IList<BiffRecordPosAttribute> keys = list.Keys;
      IList<FieldInfo> values = list.Values;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        BiffRecordPosAttribute currAttr = keys[ i ];
        FieldInfo field = values[ i ];


#if DEBUG
        try
        {
#if SHOW_EXTRACTING
          Debug.WriteIf( ApplicationImpl.IsDebugInfoEnabled, field.Name + " = " );
#endif
          //fields[ i ].SetValue( this, GetValueByAttributeType( currAttr ) );
          field.SetValue( this, GetValueByAttributeType( currAttr ) );
        }
        catch( Exception ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "-= Read fields =-" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, this.GetType().FullName, "Class Type" );
          ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, fields[ i ].Name, "Field Name" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, field.Name, "Field Name" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );

          if( !IsAllowShortData )
          {
            throw;
          }
          else
          {
            break;
          }
        }
#else
        field.SetValue( this, GetValueByAttributeType( currAttr ) );
#endif
      }
#if !SILVERLIGHT && !WINRT && !WP
      Debug.IndentLevel = 0;
#endif
    }

    /// <summary>
    /// Gets value of the field by BiffRecordPosAttribute.
    /// </summary>
    /// <param name="attr">BiffRecordPosAttribute that describes required field.</param>
    /// <returns>Value of the field.</returns>
    protected object GetValueByAttributeType( BiffRecordPosAttribute attr )
    {
      int offset = attr.Position;

      if( attr.IsBit )
      {
#if SHOW_EXTRACTING
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetBit( " + attr.Position + ", " + attr.SizeOrBitPosition + " );" );
#endif
        return GetBit( attr.Position, attr.SizeOrBitPosition );
      }
      else if( attr.IsString )
      {
#if SHOW_EXTRACTING
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "// 'Len' GetString( " + attr.Position + " );" );
#endif
        byte iLen = m_data[ attr.Position ];//GetByte( attr.Position );
        return GetString( attr.Position + 1, iLen );
      }
      else if( attr.IsString16Bit )
      {
        ushort usLen = GetUInt16( attr.Position );
        return ( usLen > 0 )
          ? GetString( attr.Position + 2, usLen )
          : string.Empty;
      }
      else if( attr.IsOEMString )
      {
#if SHOW_EXTRACTING
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "// 'Len' GetOEMString( " + attr.Position + " );" );
#endif
        byte iLen = m_data[ attr.Position ];//GetByte( attr.Position );
        int  iLast = attr.Position + iLen;

        if( iLast > m_data.Length )
          throw new WrongBiffRecordDataException( "Wrong Record data: string is too long." );

        return ( iLen == 0 ) ? ""  :
          BiffRecordRaw.LatinEncoding.GetString( GetBytes( attr.Position + 1, (int)iLen ), 0, iLen );
      }
      else if( attr.IsOEMString16Bit )
      {
#if SHOW_EXTRACTING
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "// 'Len' GetOEMString( " + attr.Position + " );" );
#endif
        ushort iLen = GetUInt16( attr.Position );
        int  iLast = attr.Position + iLen + 2;

        if( iLast > m_data.Length )
          throw new WrongBiffRecordDataException( "Wrong Record data: string is too long." );

        return ( iLen == 0 ) ? ""  :
          BiffRecordRaw.LatinEncoding.GetString( GetBytes( attr.Position + 2, ( int )iLen ), 0, iLen );
      }
      else
      {
        switch( attr.SizeOrBitPosition )
        {
          case 1:
#if SHOW_EXTRACTING
            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetByte( " + offset + " );" );
#endif
            return GetByte( offset );

          case 2:
            if( attr.IsSigned )
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetInt16( " + offset + " );" );
#endif
              return GetInt16( offset );
            }
            else
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetUInt16( " + offset + " );" );
#endif
              return GetUInt16( offset );
            }

          case 4:
            if( attr.IsFloat )
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetFloat( " + offset + " );" );
#endif
              return GetFloat( offset );
            }
            else if( attr.IsSigned )
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetInt32( " + offset + " );" );
#endif
              return GetInt32( offset );
            }
            else
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetUInt32( " + offset + " );" );
#endif
              return GetUInt32( offset );
            }

          case 8:
            if( attr.IsFloat )
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetDouble( " + offset + " );" );
#endif
              return GetDouble( offset );
            }
            else if( attr.IsSigned )
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetInt64( " + offset + " );" );
#endif
              return GetInt64( offset );
            }
            else
            {
#if SHOW_EXTRACTING
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetUInt64( " + offset + " );" );
#endif
              return GetUInt64( offset );
            }
        }
      }

      throw new ApplicationException( "AutoReader - Unknown size of item field. Record." 
        + (TypeCode) + ". Code " + (RecordCode) );
    }
    /// <summary>
    /// Method tries by metadata information fill internal array
    /// of record.
    /// </summary>
    /// <returns>Size of the filled data.</returns>
    protected int  AutoInfillFromFields()
    {
      //      BiffRecordPosAttribute[] attr;
      //      FieldInfo[] fields;

      SortedList<BiffRecordPosAttribute, FieldInfo> list = GetSortedFields();// out attr, out fields );

      // Make internal data array auto grow according to save into it data.
      bool bOldAutoGrowData = AutoGrowData;
      AutoGrowData = true;

      int iRetSize = 0;
      int iMaxSize = 0;

      IList<BiffRecordPosAttribute> keys = list.Keys;
      IList<FieldInfo> values = list.Values;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        BiffRecordPosAttribute currAttr = keys[ i ];
        FieldInfo field = values[ i ];

#if DEBUG
        try
        {
          object value = field.GetValue( this );

          iRetSize = SetValueByAttributeType( currAttr, value );
          iMaxSize = Math.Max( iMaxSize, currAttr.Position + iRetSize );
        }
        catch( Exception ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "-= Write fields =-" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, this.GetType().FullName, "Class Type" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, field.Name, "Field Name" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );
          throw;
        }
#else
        object value = field.GetValue( this );
        iRetSize = SetValueByAttributeType( currAttr, value );
        iMaxSize = Math.Max( iMaxSize, currAttr.Position + iRetSize );
#endif
      }

      AutoGrowData = bOldAutoGrowData;

      return iMaxSize;//iRetSize;
    }
    /// <summary>
    /// Sets value of the specified field.
    /// </summary>
    /// <param name="attr">BiffRecordPosAttribute that describes required field.</param>
    /// <param name="data">New field value.</param>
    /// <returns>Size of the used data.</returns>
    protected int  SetValueByAttributeType( BiffRecordPosAttribute attr, object data )
    {
      int iRetSize = 0;
      int offset = attr.Position;

      if( attr.IsOEMString )
      {
        byte[] tmpData = BiffRecordRaw.LatinEncoding.GetBytes( (string)data );

        // save length
        SetByte( offset, (byte)tmpData.Length );

        // save string
        SetBytes( offset + 1, tmpData, 0, tmpData.Length );

        iRetSize = 1 + tmpData.Length;
      }
      if( attr.IsOEMString16Bit )
      {
        byte[] tmpData = BiffRecordRaw.LatinEncoding.GetBytes( ( string )data );

        // save length
        SetUInt16( offset, ( ushort )tmpData.Length );

        // save string
        SetBytes( offset + 2, tmpData, 0, tmpData.Length );

        iRetSize = 2 + tmpData.Length;
      }
      else if( attr.IsString )
      {
        string value = (string)data;
        byte[] tmpData = Encoding.Unicode.GetBytes( value );

        // save length
        SetByte( offset, (byte)value.Length );

        // we always save strings in Unicode
        SetByte( offset + 1, 1 );

        // save string
        SetBytes( offset + 2, tmpData, 0, tmpData.Length );

        iRetSize = 2 + tmpData.Length;
      }
      else if( attr.IsString16Bit )
      {
        string value = ( string )data;
        int iLength = ( value != null ) ? value.Length : 0;

        // save length
        SetUInt16( offset, ( ushort )iLength );
        iRetSize = 2;

        if( iLength > 0 )
        {
          byte[] tmpData = Encoding.Unicode.GetBytes( value );
          // we always save strings in Unicode
          SetByte( offset + 2, 1 );

          // save string
          SetBytes( offset + 3, tmpData, 0, tmpData.Length );

          iRetSize = 3 + tmpData.Length;
        }
      }
      else if( attr.IsBit )
      {
        // We do not change iRetSize value because the bit field does
        // not reserve any space in internal array; it always uses
        // memory reserved by other fields.
        SetBit( offset, (bool)data, attr.SizeOrBitPosition );
      }
      else
      {
        switch( attr.SizeOrBitPosition )
        {
          case 1:
            SetByte( offset, (byte)data );
            break;

          case 2:
            if( attr.IsSigned )
              SetInt16( offset, (short)data );
            else
              SetUInt16( offset, (ushort)data );
            break;

          case 4:
            if( attr.IsFloat )
              SetFloat( offset, (float)data );
            else if( attr.IsSigned )
              SetInt32( offset, (int)data );
            else
              SetUInt32( offset, (uint)data );
            break;

          case 8:
            if( attr.IsFloat )
              SetDouble( offset, (double)data );
            else if( attr.IsSigned )
              SetInt64( offset, (long)data );
            else
              SetUInt64( offset, (ulong)data );
            break;
        }

        iRetSize = attr.SizeOrBitPosition;
      }

      return iRetSize;
    }
    #endregion

    #region Class Complementary methods
    /// <summary>
    /// Clears internal data array.
    /// </summary>
    public override void ClearData()
    {
      m_data = new byte[ 0 ];
    }

    /// <summary>
    /// Compares two Biff records.
    /// </summary>
    /// <param name="raw">Biff record that should be compared with this Biff record.</param>
    /// <returns>True if this instance and extFormat contain the same data.</returns>
    public override bool IsEqual( BiffRecordRaw raw )
    {
      BiffRecordRawWithArray twin = raw as BiffRecordRawWithArray;

      if( twin != null )
      {
        InfillInternalData( ExcelVersion.Excel2007 );
        twin.InfillInternalData( ExcelVersion.Excel2007 );

        if( m_iLength == twin.m_iLength )
        {
          for(int i = 0; i < m_iLength; i++ )
          {
            if( m_data[ i ] != twin.m_data[ i ] ) return false;
          }
        }

        return true;
      }

      return false;
    }
    /// <summary>
    /// Copies data from the current Biff record to the specified Biff record.
    /// </summary>
    /// <param name="raw">Biff record that will receive data from the current record.</param>
    /// <exception cref="System.ArgumentException">
    /// When this record and parameter have different types.
    /// </exception>
    public override void CopyTo( BiffRecordRaw raw )
    {
      if( this.RecordCode != raw.RecordCode )
        throw new ArgumentException( "Records should have same type for copy." );

      BiffRecordRawWithArray destination = raw as BiffRecordRawWithArray;

      InfillInternalData( ExcelVersion.Excel2007 );
      destination.m_data = new byte[ Length ];
      Array.Copy( m_data, 0, destination.m_data, 0, Length );
      destination.m_iLength = m_iLength;
      destination.ParseStructure();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    protected internal void SetInternalData( byte[] arrData )
    {
      SetInternalData( arrData, true );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    /// <param name="bNeedInfill"></param>
    protected void SetInternalData( byte[] arrData, bool bNeedInfill )
    {
      m_data = arrData;
      NeedInfill = bNeedInfill;
    }
    #endregion

    #region Class Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public abstract void ParseStructure();
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      //        return MinimumRecordSize;
      int iMinSize = MinimumRecordSize;

      if( iMinSize == MaximumRecordSize )
        return iMinSize;

      if( NeedInfill )
      {
        InfillInternalData( version );
        NeedInfill = false;
      }

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "GetStoreSize should be overloaded:" + "  " + TypeCode.ToString() );
      return m_iLength;
    }
    #endregion

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
      OnDispose();
      m_data = null;
      GC.SuppressFinalize( this );
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnDispose()
    {
    }
    #endregion
  }
}
