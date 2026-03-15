#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Text;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for DataProvider.
	/// </summary>
	public abstract class DataProvider : IDisposable
	{
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DataProvider()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    ~DataProvider()
    {
      Dispose(false);
    }
    #endregion

    #region Read methods
    /// <summary>
    /// Returns bit value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="iBit">Bit index in the byte.</param>
    /// <returns>Bit at the specified position.</returns>
    public bool ReadBit( int iOffset, int iBit )
    {
      if( iBit < 0 || iBit > 7 )
        throw new ArgumentOutOfRangeException( "iBit", "Bit Position cannot be less than 0 or greater than 7." );

      byte btValue = ReadByte( iOffset );
      return ( btValue & ( 1 << iBit ) ) == ( 1 << iBit );
    }
    /// <summary>
    /// Returns byte value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Byte at the specified position.</returns>
    public abstract byte ReadByte( int iOffset );
    /// <summary>
    /// Returns boolean value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Byte at the specified position.</returns>
    public bool ReadBoolean( int iOffset )
    {
      return ReadByte( iOffset ) != 0;
    }
    /// <summary>
    /// Returns Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int16 value at the specified position.</returns>
    public abstract short ReadInt16( int iOffset );
    /// <summary>
    /// Returns UInt16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int16 value at the specified position.</returns>
    [ CLSCompliant( false ) ]
    public ushort ReadUInt16( int iOffset )
    {
      return ( ushort )ReadInt16( iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public abstract int ReadInt32( int iOffset );
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    [ CLSCompliant( false ) ]
    public uint ReadUInt32( int iOffset )
    {
      return ( uint )ReadInt32( iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public abstract long ReadInt64( int iOffset );
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public virtual double ReadDouble( int iOffset )
    {
      return BitConverterGeneral.Int64BitsToDouble( ReadInt64( iOffset ) );
    }
    /// <summary>
    /// Copies data from internal storage into specified data array.
    /// </summary>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="arrDestination">Destination array.</param>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iLength">Size in bytes of the data to copy.</param>
    public abstract void CopyTo( int iSourceOffset, byte[] arrDestination, int iDestOffset, int iLength );
    /// <summary>
    /// Copies data from internal storage into specified provider.
    /// </summary>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="destination">Destination provider.</param>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iLength">Size in bytes of the data to copy.</param>
    public virtual void CopyTo( int iSourceOffset, DataProvider destination, int iDestOffset, int iLength )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Reads data from BinaryReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="iOffset">Offset in the internal data to start filling from.</param>
    /// <param name="iLength">Number of bytes to read.</param>
    /// <param name="arrBuffer">Temporary buffer to use.</param>
    public abstract void Read( BinaryReader reader, int iOffset, int iLength,
      byte[] arrBuffer );
    /// <summary>
    /// Reads data from BinaryReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="iOffset">Offset in the internal data to start filling from.</param>
    /// <param name="iLength">Number of bytes to read.</param>
    /// <param name="arrBuffer">Temporary buffer to use.</param>
    /// <param name="decryptor">Object used to decrypt data if necessary.</param>
    public void Read( BinaryReader reader, int iOffset, int iLength,
      byte[] arrBuffer, IDecryptor decryptor )
    {
      bool bNotNull = decryptor != null;

      long lPosition = bNotNull ? reader.BaseStream.Position : 0;
      Read( reader, iOffset, iLength, arrBuffer );

      if( bNotNull )
      {
        decryptor.Decrypt( this, iOffset, iLength, lPosition );
      }
    }
    /// <summary>
    /// Gets string from internal data and returns it's length in iFullLength parameter.
    /// </summary>
    /// <param name="iOffset">Offset to the string data.</param>
    /// <param name="iFullLength">Length of the string in bytes.</param>
    /// <returns>Parsed string.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is smaller than the string that should be in it.
    /// </exception>
    public virtual string ReadString16Bit( int iOffset, out int iFullLength )
    {
      ushort strLen = ReadUInt16( iOffset );
      iOffset += 2;
      bool bIs16Bit = ReadBoolean( iOffset );
      iOffset++;
      iFullLength = bIs16Bit ? 3 + strLen * 2 : 3 + strLen;

      int iSizeInBytes = bIs16Bit ? strLen * 2 : strLen;
      Encoding encoding = bIs16Bit ?
        Encoding.Unicode :
        BiffRecordRaw.LatinEncoding;

      return ReadString( iOffset, iSizeInBytes, encoding, bIs16Bit );
//      byte[] arrString = new byte[ iSizeInBytes ];
//      CopyTo( iOffset, arrString, 0, iSizeInBytes );
//      Encoding encoding = Is16Bit
//        ? Encoding.Unicode
//        : BiffRecordRawWithArray.LatinEncoding;
//
//      return encoding.GetString( arrString, 0, iSizeInBytes );
    }
    /// <summary>
    /// Gets string from internal data and updates offset.
    /// </summary>
    /// <param name="iOffset">Offset to the string data.</param>
    /// <returns>Parsed string.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is smaller than the string that should be in it.
    /// </exception>
    public virtual string ReadString16BitUpdateOffset( ref int iOffset )
    {
      int iFullLength;
      string strResult = ReadString16Bit( iOffset, out iFullLength );
      iOffset += iFullLength;

      return strResult;
    }
    /// <summary>
    /// Gets string from byte array and returns it's length in iFullLength parameter.
    /// </summary>
    /// <param name="iOffset">Offset to the string data.</param>
    /// <param name="iFullLength">Length of the string in bytes.</param>
    /// <returns>Parsed string.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is smaller than the string that should be in it.
    /// </exception>
    public virtual string ReadString8Bit( int iOffset, out int iFullLength )
    {
      ushort strLen = ReadByte( iOffset );
      iOffset++;
      bool Is16Bit = ReadBoolean( iOffset );
      iOffset++;
      int iSizeInBytes = Is16Bit ? strLen * 2 : strLen;
      iFullLength = 2 + iSizeInBytes;

      byte[] arrString = new byte[ iSizeInBytes ];
      CopyTo( iOffset, arrString, 0, iSizeInBytes );
      Encoding encoding = Is16Bit
        ? Encoding.Unicode
        : BiffRecordRawWithArray.LatinEncoding;

      return encoding.GetString( arrString, 0, iSizeInBytes );
    }
    /// <summary>
    /// Reads data array from another data array.
    /// </summary>
    /// <param name="iOffset">Source offset.</param>
    /// <param name="arrDest">Destination array.</param>
    /// <returns>Updated source offset.</returns>
    public int ReadArray( int iOffset, byte[] arrDest )
    {
      if( arrDest == null )
        throw new ArgumentNullException( "arrDest" );

      int iLength = arrDest.Length;
      CopyTo( iOffset, arrDest, 0, iLength );

      return iOffset + iLength;
    }
    /// <summary>
    /// Reads data array from another data array.
    /// </summary>
    /// <param name="iOffset">Source offset.</param>
    /// <param name="arrDest">Destination array.</param>
    /// <param name="size">Number of bytes to read.</param>
    /// <returns>Updated source offset.</returns>
    public int ReadArray( int iOffset, byte[] arrDest, int size )
    {
      if( arrDest == null )
        throw new ArgumentNullException( "arrDest" );

      CopyTo( iOffset, arrDest, 0, size );

      return iOffset + size;
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <param name="isByteCounted">Flags that represent is bytes count available.</param>
    /// <returns>Retrieved string.</returns>
    public string ReadString( int offset, int iStrLen, out int iBytesInString, bool isByteCounted )
    {
      byte iCompressUnicode = ReadByte( offset );
      int  iLast = ( iCompressUnicode != 0 && !isByteCounted ) ? 2 * iStrLen : iStrLen;

      iLast += offset + 1;
      bool bUnicode = iCompressUnicode != 0;

      iBytesInString = ( bUnicode && !isByteCounted )
        ? iStrLen * 2
        : iStrLen;

      byte[] arrData = new byte[ iBytesInString ];
      ReadArray( offset + 1, arrData );

      Encoding encoding = bUnicode
        ? Encoding.Unicode
        : BiffRecordRaw.LatinEncoding;

      return encoding.GetString( arrData, 0, arrData.Length );
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
    public string ReadStringUpdateOffset( ref int offset, int iStrLen )
    {
      if( iStrLen > 0 )
      {
        int iBytes;
        string result = ReadString( offset, iStrLen, out iBytes, false );
        offset += iBytes + 1;
        return result;
      }

      return string.Empty;
    }
    /// <summary>
    /// Reads string from data provider.
    /// </summary>
    /// <param name="offset">Offset to the first character.</param>
    /// <param name="stringLength">Number of bytes in the string.</param>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="isUnicode">Indicates whether string is unicode.</param>
    /// <returns>Extracted string.</returns>
    public abstract string ReadString( int offset, int stringLength, Encoding encoding, bool isUnicode );
    /// <summary>
    /// Gets TAddr structure from internal data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to get.</param>
    /// <returns>Retrieved TAddr structure.</returns>
    [ CLSCompliant( false ) ]
    public TAddr ReadAddr( int offset )
    {
      TAddr addr = new TAddr();

      addr.FirstRow = ReadUInt16( offset );
      addr.LastRow = ReadUInt16( offset + 2 );
      addr.FirstCol = ReadUInt16( offset + 4 );
      addr.LastCol = ReadUInt16( offset + 6 );

      return addr;
    }
    /// <summary>
    /// Gets TAddr structure from internal data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to get.</param>
    /// <returns>Retrieved TAddr structure.</returns>
    public Rectangle ReadAddrAsRectangle( int offset )
    {
      int iFirstRow = ReadUInt16( offset );
      int iLastRow = ReadUInt16( offset + 2 );
      int iFirstCol = ReadUInt16( offset + 4 );
      int iLastCol = ReadUInt16( offset + 6 );

      return Rectangle.FromLTRB( iFirstCol, iFirstRow, iLastCol, iLastRow );
    }
    #endregion

    #region Write methods
    /// <summary>
    /// Saves data into binary writer.
    /// </summary>
    /// <param name="writer">Writer to save data into.</param>
    /// <param name="iOffset">Start offset in the internal storage.</param>
    /// <param name="iSize">Number of bytes to save.</param>
    /// <param name="arrBuffer">Temporary buffer to use.</param>
    public virtual void WriteInto( BinaryWriter writer, int iOffset, int iSize, byte[] arrBuffer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      int iSizeLeft = iSize;
      int DEF_BUFFER_SIZE = arrBuffer.Length;

      while( iSizeLeft > 0 )
      {
        int iReadBytes = Math.Min( DEF_BUFFER_SIZE, iSizeLeft );
        CopyTo( iOffset, arrBuffer, 0, iReadBytes );
        writer.Write( arrBuffer, 0, iReadBytes );
        iOffset += iReadBytes;
        iSizeLeft -= iReadBytes;
      }
    }
    /// <summary>
    /// Writes byte value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Byte value to write at the specified position.</param>
    public abstract void WriteByte( int iOffset, byte value );
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    public abstract void WriteInt16( int iOffset, short value );
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    [ CLSCompliant( false ) ]
    public virtual void WriteUInt16( int iOffset, ushort value )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public abstract void WriteInt32( int iOffset, int value );
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public abstract void WriteInt64( int iOffset, long value );
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    [ CLSCompliant( false ) ]
    public void WriteUInt32( int iOffset, uint value )
    {
      WriteInt32( iOffset, ( int )value );
    }
    /// <summary>
    /// Sets one bit in specified byte in internal record data array.
    /// </summary>
    /// <param name="offset">Offset of the byte in the data array.</param>
    /// <param name="bitPos">Bit position in the byte.</param>
    /// <param name="value">Value of bit.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If bitPos is less than zero or more than 7.
    /// </exception>
    public abstract void WriteBit( int offset, bool value, int bitPos );
    /// <summary>
    /// Writes Double value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Double value to write at the specified position.</param>
    public abstract void WriteDouble( int iOffset, double value );
    /// <summary>
    /// Sets string in internal data. The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public void WriteString8BitUpdateOffset( ref int offset, string value )
    {
      // save length
      WriteByte( offset, ( byte )value.Length );

      offset += 1;
      WriteStringNoLenUpdateOffset( ref offset, value );
    }
    /// <summary>
    /// Sets string in internal data. The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public void WriteString16BitUpdateOffset( ref int offset, string value )
    {
      WriteString16BitUpdateOffset( ref offset, value, true );
    }
    /// <summary>
    /// Sets string in internal data. The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="isUnicode">Indicates whether string should be stored as unicode.</param>
    public void WriteString16BitUpdateOffset( ref int offset, string value, bool isUnicode )
    {
      // save length
      int iLength = ( value != null ) ? value.Length : 0;
      WriteUInt16( offset, ( ushort )iLength );
      offset += 2;

      //if( iLength == 0 )
      WriteStringNoLenUpdateOffset( ref offset, value, isUnicode );
    }
    /// <summary>
    /// Sets string in internal data. The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the written data.</returns>
    public int WriteString16Bit( int offset, string value )
    {
      return WriteString16Bit( offset, value, true );
    }
    /// <summary>
    /// Sets string in internal data. The String length is saved in two bytes.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="isUnicode">Indicates whether string should be stored in unicode format.</param>
    /// <returns>Size of the written data.</returns>
    public int WriteString16Bit( int offset, string value, bool isUnicode )
    {
      int iStartOffset = offset;
      WriteString16BitUpdateOffset( ref offset, value, isUnicode );
      return offset - iStartOffset;
    }
    /// <summary>
    /// Sets string in internal record data without string length,
    /// updates offset parameter (adds string length).
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    public virtual void WriteStringNoLenUpdateOffset( ref int offset, string value )
    {
      WriteStringNoLenUpdateOffset( ref offset, value, true );
    }
    /// <summary>
    /// Sets string in internal record data without string length,
    /// updates offset parameter (adds string length).
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="bUnicode">Indicates whether string should be stored in Unicode encoding or not.</param>
    public abstract void WriteStringNoLenUpdateOffset( ref int offset, string value, bool bUnicode );
    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="data">Array of bytes to set.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If value array is NULL.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If pos or length would be less than zero  or their sum would be more
    ///   than size of value array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If internal record data array is too small for receiving value array
    ///   and AutoGrowData is False.
    /// </exception>
    public void WriteBytes( int offset, byte[] data )
    {
      WriteBytes( offset, data, 0, data.Length );
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
    public abstract void WriteBytes( int offset, byte[] value, int pos, int length );
    /// <summary>
    /// Sets TAddr structure in internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to set.</param>
    /// <param name="addr">New value of the TAddr at the specified position.</param>
    [ CLSCompliant( false ) ]
    internal protected void WriteAddr( int offset, TAddr addr )
    {
      WriteUInt16( offset, ( ushort )addr.FirstRow );
      WriteUInt16( offset + 2, ( ushort )addr.LastRow );
      WriteUInt16( offset + 4, ( ushort )addr.FirstCol );
      WriteUInt16( offset + 6, ( ushort )addr.LastCol );
    }
    /// <summary>
    /// Sets TAddr structure in internal record data.
    /// </summary>
    /// <param name="offset">Offset in bytes of TAddr structure to set.</param>
    /// <param name="addr">New value of the Rectangle at the specified position.</param>
    internal protected void WriteAddr( int offset, Rectangle addr )
    {
      WriteUInt16( offset, ( ushort )addr.Top );
      WriteUInt16( offset + 2, ( ushort )addr.Bottom );
      WriteUInt16( offset + 4, ( ushort )addr.Left );
      WriteUInt16( offset + 6, ( ushort )addr.Right );
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns size of the internal buffer. Read-only.
    /// </summary>
    public abstract int Capacity { get; }
    /// <summary>
    /// Indicates whether data provider was cleared.
    /// </summary>
    public abstract bool IsCleared { get; }
    /// <summary>
    /// Moves memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public abstract void MoveMemory( int iDestOffset, int iSourceOffset, int iMemorySize );
#if !(WINRT )
    /// <summary>
    /// Copies memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public abstract void CopyMemory( int iDestOffset, int iSourceOffset, int iMemorySize );
#endif
    /// <summary>
    /// Resizes internal storage if necessary.
    /// </summary>
    /// <param name="size">Required size.</param>
    public abstract int EnsureCapacity( int size );
    public abstract int EnsureCapacity(int size, int forceAdd);
    /// <summary>
    /// Writes zeros inside memory block.
    /// </summary>
    public abstract void ZeroMemory();
    /// <summary>
    /// Clears internal storage if allowed.
    /// </summary>
    public abstract void Clear();
    /// <summary>
    /// Creates data provider of the same type.
    /// </summary>
    /// <returns>Created data provider.</returns>
    public abstract DataProvider CreateProvider();
    #endregion

    #region IDisposable Members

    /// <summary>
    /// 
    /// </summary>
    public void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            OnDispose();
        }
      GC.SuppressFinalize( this );
    }
    public void Dispose()
    {
        Dispose(true);
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
