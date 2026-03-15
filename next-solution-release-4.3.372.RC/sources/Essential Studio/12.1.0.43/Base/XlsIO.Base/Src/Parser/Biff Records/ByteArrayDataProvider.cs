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

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for ByteArrayDataProvider.
	/// </summary>
	public class ByteArrayDataProvider : DataProvider
	{
    #region Class members
    /// <summary>
    /// Pointer to the data array.
    /// </summary>
    private byte[] m_arrData;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor to prevent creation of items without arguments.
    /// </summary>
    public ByteArrayDataProvider()
      : this( new byte[ 128 ] )
    {
    }
    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="arrData">Array to read data from.</param>
    public ByteArrayDataProvider( byte[] arrData )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      m_arrData = arrData;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Return internal buffer. Read-only.
    /// </summary>
    public byte[] InternalBuffer
    {
      get
      {
        return m_arrData;
      }
    }
    /// <summary>
    /// Returns size of the internal buffer. Read-only.
    /// </summary>
    public override int Capacity
    {
      get
      {
        return m_arrData.Length;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override bool IsCleared
    {
      get
      {
        return m_arrData == null;
      }
    }
    #endregion

    #region Read methods
    /// <summary>
    /// Returns byte value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Byte at the specified position.</returns>
    public override byte ReadByte( int iOffset )
    {
#if DEBUG
      if( m_arrData.Length <= iOffset )
        throw new ArgumentOutOfRangeException( "iOffset" );
#endif
      return m_arrData[ iOffset ];
    }
    /// <summary>
    /// Returns Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int16 value at the specified position.</returns>
    public override short ReadInt16( int iOffset )
    {
      return BitConverter.ToInt16( m_arrData, iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override int ReadInt32( int iOffset )
    {
      return BitConverter.ToInt32( m_arrData, iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override long ReadInt64( int iOffset )
    {
      return BitConverter.ToInt64( m_arrData, iOffset );
    }
    /// <summary>
    /// Copies data from internal storage into specified
    /// </summary>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="arrDestination">Destination array.</param>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iLength">Size in bytes of the data to copy.</param>
    public override void CopyTo( int iSourceOffset, byte[] arrDestination, int iDestOffset, int iLength )
    {
      Buffer.BlockCopy( m_arrData, iSourceOffset, arrDestination, iDestOffset, iLength );
    }
    /// <summary>
    /// Copies data from internal storage into specified provider.
    /// </summary>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="destination">Destination provider.</param>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iLength">Size in bytes of the data to copy.</param>
    public override void CopyTo( int iSourceOffset, DataProvider destination, int iDestOffset, int iLength )
    {
      if( iLength > 0 )
        destination.WriteBytes( iDestOffset, m_arrData, iSourceOffset, iLength );
    }
    /// <summary>
    /// Reads data from BinaryReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="iOffset">Offset in the internal data to start filling from.</param>
    /// <param name="iLength">Number of bytes to read.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    public override void Read( BinaryReader reader, int iOffset, int iLength, byte[] arrBuffer )
    {
      if( iOffset + iLength > m_arrData.Length )
        throw new ArgumentOutOfRangeException();

      reader.Read( m_arrData, iOffset, iLength );
    }
    /// <summary>
    /// Reads string from data provider.
    /// </summary>
    /// <param name="offset">Offset to the first character.</param>
    /// <param name="stringLength">Number of bytes in the string.</param>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="isUnicode">Indicates is unicode encoding.</param>
    /// <returns>Extracted string.</returns>
    public override string ReadString( int offset, int stringLength, Encoding encoding, bool isUnicode )
    {
      if( encoding == null )
      {
        encoding = isUnicode ?
          Encoding.Unicode :
#if !SILVERLIGHT && !WINRT && !WP
          Encoding.ASCII;
#else
          Encoding.UTF8;
#endif
      }

      return encoding.GetString( m_arrData, offset, stringLength );
    }
    /// <summary>
    /// Resizes internal storage if necessary.
    /// </summary>
    /// <param name="size">Required size.</param>
    public override int EnsureCapacity( int size )
    {
        return EnsureCapacity(size, 0);
    }
    public override int EnsureCapacity(int size, int forceAdd)
    {
        int iLength = (m_arrData != null) ? m_arrData.Length : 0;

        if (iLength < size)
        {
            byte[] newData = new byte[size];

            if (iLength > 0)
                Buffer.BlockCopy(m_arrData, 0, newData, 0, iLength);

            m_arrData = newData;
        }
        return iLength;
    }
    /// <summary>
    /// Writes zeros inside memory block.
    /// </summary>
    public override void ZeroMemory()
    {
      for( int i = 0, len = m_arrData.Length; i < len; i++ )
        m_arrData[ i ] = 0;
    }
    #endregion

    #region Write methods
    /// <summary>
    /// Writes byte value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Byte value to write at the specified position.</param>
    public override void WriteByte( int iOffset, byte value )
    {
      m_arrData[ iOffset ] = value;
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    public override void WriteInt16( int iOffset, short value )
    {
      BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    [ CLSCompliant( false ) ]
    public override void WriteUInt16( int iOffset, ushort value )
    {
#if DEBUG
      if( m_arrData.Length < iOffset + 2 )
        throw new ArgumentOutOfRangeException( "iOffset or length" );
#endif
      BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public override void WriteInt32( int iOffset, int value )
    {
      BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public override void WriteInt64( int iOffset, long value )
    {
      BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
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
    public override void WriteBit( int offset, bool value, int bitPos )
    {
      if( bitPos < 0 || bitPos > 7 )
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position can be zero or greater than 7." );
    
      if( value )
      {
        m_arrData[ offset ] |= ( byte )( 1 << bitPos );
      }
      else
      {
        m_arrData[ offset ] &= ( byte )( ~( 1 << bitPos ) );
      }
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Double value to write at the specified position.</param>
    public override void WriteDouble( int iOffset, double value )
    {
      BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
    }
    /// <summary>
    /// Sets string in internal data without string length,
    /// updates offset parameter (adds string length).
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="unicode">Indicates whether to use unicode encoding or not.</param>
    public override void WriteStringNoLenUpdateOffset( ref int offset, string value, bool unicode )
    {
      if( value == null || value.Length == 0 ) return;

      Encoding encoding = unicode ?
        Encoding.Unicode :
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.ASCII;
#else
        Encoding.UTF8;
#endif

      byte[] tmpData = encoding.GetBytes( value );

      // we always save strings in Unicode
      m_arrData[ offset ] = unicode ? ( byte )1 : ( byte )0;
      offset++;

      // save string
      //SetBytes( arrData, offset + 1, tmpData, 0, tmpData.Length );
      int iLength = tmpData.Length;
      Buffer.BlockCopy( tmpData, 0, m_arrData, offset, iLength );
      offset += iLength;
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
    public override void WriteBytes( int offset, byte[] value, int pos, int length )
    {
      if( length == 0 )
        return;

      if( value == null )
        throw new ArgumentNullException( "value" );

      if( pos < 0 )
        throw new ArgumentOutOfRangeException( "pos", "Position cannot be zeroless." );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length", "Length of data to copy must be greater then zero." );

      if( pos + length > value.Length )
        throw new ArgumentOutOfRangeException( "value", "Position or length has wrong value." );

      Buffer.BlockCopy( value, pos, m_arrData, offset, length );
    }

    /// <summary>
    /// Saves data into binary writer.
    /// </summary>
    /// <param name="writer">Writer to save data into.</param>
    /// <param name="iOffset">Start offset in the internal storage.</param>
    /// <param name="iSize">Number of bytes to save.</param>
    /// <param name="arrBuffer">Temporary buffer to use.</param>
    public override void WriteInto( BinaryWriter writer, int iOffset, int iSize, byte[] arrBuffer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.Write( m_arrData, iOffset, iSize );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Updates internal buffer.
    /// </summary>
    /// <param name="arrNewBuffer">New buffer to set.</param>
    internal void SetBuffer( byte[] arrNewBuffer )
    {
      if( arrNewBuffer == null )
        throw new ArgumentNullException( "arrNewBuffer" );

      m_arrData = arrNewBuffer;
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void OnDispose()
    {
      m_arrData = null;
    }
    /// <summary>
    /// Clears internal data.
    /// </summary>
    public override void Clear()
    {
      m_arrData = null;
    }
    /// <summary>
    /// Moves memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public override void MoveMemory( int iDestOffset, int iSourceOffset, int iMemorySize )
    {
      //throw new NotImplementedException();
      //IntPtr ptrDest = ( IntPtr )( m_lPointer + iDestOffset );
      //IntPtr ptrSource = ( IntPtr )( m_lPointer + iSourceOffset );
      //Memory.RtlMoveMemory( ptrDest, ptrSource, iMemorySize );
      Buffer.BlockCopy( m_arrData, iSourceOffset, m_arrData, iDestOffset, iMemorySize );
    }
#if !(WINRT )
    /// <summary>
    /// Copies memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public override void CopyMemory( int iDestOffset, int iSourceOffset, int iMemorySize )
    {
      //IntPtr ptrDest = ( IntPtr )( m_lPointer + iDestOffset );
      //IntPtr ptrSource = ( IntPtr )( m_lPointer + iSourceOffset );
      //Memory.CopyMemory( ptrDest, ptrSource, iMemorySize );
      Buffer.BlockCopy( m_arrData, iSourceOffset, m_arrData, iDestOffset, iMemorySize );
    }
#endif
    /// <summary>
    /// Creates provider of the same type.
    /// </summary>
    /// <returns>Created provider object.</returns>
    public override DataProvider CreateProvider()
    {
      return new ByteArrayDataProvider();
    }
    #endregion
  }
}
