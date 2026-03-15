#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT && !WINRT && !WP
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

//using Syncfusion.XlsIO.IO.Stream.Win32;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for IntPtrDataProvider.
	/// </summary>
  public class IntPtrDataProvider
    : DataProvider
    , IDisposable
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected IntPtr m_ptrHeapHandle;
    /// <summary>
    /// Pointer to the data array.
    /// </summary>
    protected IntPtr m_ptrData;
    /// <summary>
    /// m_ptrData converted into Int64 value.
    /// </summary>
    protected long m_lPointer;
    /// <summary>
    /// Indicates whether this instance is responsible for memory allocation and freeing.
    /// </summary>
    private bool m_bControlPointer;
    /// <summary>
    /// Size of the allocated data.
    /// </summary>
    private int m_iSize;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor to prevent creation of items without arguments.
    /// </summary>
    public IntPtrDataProvider( IntPtr heapHandle )
    {
      m_ptrHeapHandle = heapHandle;
      m_bControlPointer = true;
    }
    /// <summary>
    /// 
    /// </summary>
    ~IntPtrDataProvider()
    {
      if( m_ptrData != IntPtr.Zero )
      {
        Dispose(false);
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets internal data storage.
    /// </summary>
    public IntPtr DataPointer
    {
      get
      {
        return m_ptrData;
      }
      set
      {
        m_ptrData = value;
        m_lPointer = m_ptrData.ToInt64();
      }
    }
    /// <summary>
    /// Returns size of the internal buffer. Read-only.
    /// </summary>
    public override int Capacity
    {
      get
      {
        return m_iSize;
      }
    }
    /// <summary>
    /// Returns heap handle.
    /// </summary>
    public IntPtr HeapHandle
    {
      get
      {
        return m_ptrHeapHandle;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override bool IsCleared
    {
      get
      {
        return m_ptrData == IntPtr.Zero;
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
      return Marshal.ReadByte( m_ptrData, iOffset );
    }
    /// <summary>
    /// Returns Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int16 value at the specified position.</returns>
    public override short ReadInt16( int iOffset )
    {
      return Marshal.ReadInt16( m_ptrData, iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override int ReadInt32( int iOffset )
    {
      return Marshal.ReadInt32( m_ptrData, iOffset );
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override long ReadInt64( int iOffset )
    {
      return Marshal.ReadInt64( m_ptrData, iOffset );
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
      IntPtr ptrSource = ( IntPtr )( m_lPointer + iSourceOffset );
      Marshal.Copy( ptrSource, arrDestination, iDestOffset, iLength );
    }
    /// <summary>
    /// Reads data from BinaryReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="iOffset">Offset in the internal data to start filling from.</param>
    /// <param name="iLength">Number of bytes to read.</param>
    /// <param name="arrBuffer">Temporary buffer to use.</param>
    public override void Read( BinaryReader reader, int iOffset, int iLength, byte[] arrBuffer )
    {
      int iBufferSize = arrBuffer.Length;
      long lPointerStart = m_lPointer;;

      while( iLength > 0 )
      {
        int iBytesToRead = ( iBufferSize > iLength )
          ? iLength
          : iBufferSize;

        reader.Read( arrBuffer, 0, iBytesToRead );

        IntPtr ptrDest = ( IntPtr )( lPointerStart + iOffset );
        Marshal.Copy( arrBuffer, 0, ptrDest, iBytesToRead );
        iLength -= iBytesToRead;
        iOffset += iBytesToRead;
      }
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
      // TODO: this can be optimized - use single buffer.
      //byte[] arrString = new byte[ stringLength ];
      //ReadArray( offset, arrString, stringLength );
      IntPtr ptrStringStart = ( IntPtr )( m_lPointer + offset );
      string strResult;

      if( isUnicode )
      {
        strResult = Marshal.PtrToStringUni( ptrStringStart, stringLength / 2 );
      }
      else
      {
        if( encoding == Encoding.Default )
        {
          strResult = Marshal.PtrToStringAnsi( ptrStringStart, stringLength );
        }
        else
        {
          // TODO: this can be optimized if we re-use the same buffer each time.
          byte[] arrString = new byte[ stringLength ];
          ReadArray( offset, arrString, stringLength );
          strResult = encoding.GetString( arrString );
        }
      }

      return strResult;

      //return encoding.GetString( arrString, 0, stringLength );
      //Marshal.PtrToStringUni( ptrStringStart, stringLen / 2 );
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
      Marshal.WriteByte( m_ptrData, iOffset, value );
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    public override void WriteInt16( int iOffset, short value )
    {
      Marshal.WriteInt16( m_ptrData, iOffset, value );
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    [ CLSCompliant( false ) ]
    public override void WriteUInt16( int iOffset, ushort value )
    {
      WriteInt16( iOffset, ( short )value );
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public override void WriteInt32( int iOffset, int value )
    {
      Marshal.WriteInt32( m_ptrData, iOffset, value );
    }
    /// <summary>
    /// Writes Int64 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int64 value to write at the specified position.</param>
    public override void WriteInt64( int iOffset, long value )
    {
      Marshal.WriteInt64( m_ptrData, iOffset, value );
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

      byte btValue = ReadByte( offset );
    
      if( value )
      {
        btValue |= ( byte )( 1 << bitPos );
      }
      else
      {
        btValue &= ( byte )( ~( 1 << bitPos ) );
      }

      WriteByte( offset, btValue );
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Double value to write at the specified position.</param>
    public override void WriteDouble( int iOffset, double value )
    {
      //BitConverter.GetBytes( value ).CopyTo( m_arrData, iOffset );
      const int DEF_DOUBLE_SIZE = 8;
      byte[] arrBuffer = BitConverter.GetBytes( value );
      IntPtr ptrDest = ( IntPtr )( m_lPointer + iOffset );
      Marshal.Copy( arrBuffer, 0, ptrDest, DEF_DOUBLE_SIZE );
    }
    /// <summary>
    /// Sets string in internal data without string length,
    /// updates offset parameter (adds string length).
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="unicode">Indicates whether string must be stored using Unicode encoding or not.</param>
    public override void WriteStringNoLenUpdateOffset( ref int offset, string value, bool unicode )
    {
      if( value == null || value.Length == 0 ) return;

      Encoding encoding = unicode
        ? Encoding.Unicode
        : Encoding.ASCII;

      byte[] tmpData = encoding.GetBytes( value );

      // we always save strings in Unicode
      Marshal.WriteByte( m_ptrData, offset, unicode ? ( byte )1 : ( byte )0 );
      offset++;

      // save string
      //SetBytes( arrData, offset + 1, tmpData, 0, tmpData.Length );
      int iLength = tmpData.Length;
      IntPtr ptrDest = ( IntPtr )( m_lPointer + offset );
      Marshal.Copy( tmpData, 0, ptrDest, iLength );
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

#if DEBUG
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( pos < 0 )
        throw new ArgumentOutOfRangeException( "pos", "Position cannot be zeroless." );

      if( length < 0 )
        throw new ArgumentOutOfRangeException( "length", "Length of data to copy must be greater then zero." );

      if( m_bControlPointer && pos + length > m_iSize )
        throw new ArgumentOutOfRangeException( "value", "Position or length has wrong value." );
#endif

      IntPtr ptrDest = ( IntPtr )( m_lPointer + offset );
      //IntPtr ptrDest = m_ptrData + ( IntPtr )offset;
      Marshal.Copy( value, pos, ptrDest, length );
    }
    #endregion

    #region IDisposable members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    protected override void OnDispose()
    {
      if( m_ptrData != IntPtr.Zero )
      {
        if( m_bControlPointer )
        {
          ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "IntPtrDataProvider: memory freeing." );
          //Marshal.FreeHGlobal( m_ptrData );
          //API.GlobalUnlock( m_ptrHandle );
          //API.GlobalFree( m_ptrHandle );

          if( m_ptrHeapHandle != IntPtr.Zero )
          {
            //Console.WriteLine( "HeapFree: OnDispose {1} {0}", m_ptrHeapHandle, m_ptrData );
            Heap.HeapFree( m_ptrHeapHandle, 0, m_ptrData );

            //if( m_ptrHeapHandle != IntPtr.Zero )
            //  Heap.HeapCompact( m_ptrHeapHandle, 0 );
          }
          else
          {
            Marshal.FreeHGlobal( m_ptrData );
          }

          if (m_iSize > 0)
              GC.RemoveMemoryPressure( m_iSize );
        }

        m_ptrHeapHandle = IntPtr.Zero;
        m_ptrData = IntPtr.Zero;
        m_lPointer = 0;
        m_iSize = 0;
      }

      //GC.SuppressFinalize( this );
    }
    #endregion

    #region Class methods
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
        if (m_bControlPointer)  // Check whether memory handling by this instance..
        {
            if (size > m_iSize) //Check the available size and requested size.
            {
                //Allocating Memory
                if (m_ptrHeapHandle == IntPtr.Zero)
                {
                    m_ptrData = (m_iSize > 0)
                      ? Marshal.ReAllocHGlobal(m_ptrData, (IntPtr)size)
                      : Marshal.AllocHGlobal(size);
                }
                else
                {
                    size += forceAdd;
                    m_ptrData = (m_iSize > 0) ?
                      Heap.HeapReAlloc(m_ptrHeapHandle, 0, m_ptrData, size) :
                      Heap.HeapAlloc(m_ptrHeapHandle, 0, size);

                    //Console.WriteLine( "HeapAlloc {0}, heap hanlde {1}", m_ptrData, m_ptrHeapHandle );
                }

                m_lPointer = m_ptrData.ToInt64();

                if (m_lPointer == 0) //Throws out of Memory exception Exception in case of memory not available.
                    throw new OutOfMemoryException();

#if !SILVERLIGHT && !WINRT && !WP
                GC.AddMemoryPressure(size - m_iSize);
#endif
                m_iSize = size; //Capacity has been changed.
                return size; //Returns the current size.
            }
        }
        return m_iSize;
    }

    /// <summary>
    /// Clears internal storage if allowed.
    /// </summary>
    public override void Clear()
    {
      if( m_bControlPointer && m_iSize > 0 )
      {
        //Marshal.FreeHGlobal( m_ptrData );
        if( m_ptrHeapHandle != IntPtr.Zero )
        {
          //Console.WriteLine( "HeapFree: {0}", m_ptrData );
          Heap.HeapFree( m_ptrHeapHandle, 0, m_ptrData );
        }
        else
        {
          Marshal.FreeHGlobal( m_ptrData );
        }
        m_ptrData = IntPtr.Zero;
        m_lPointer = 0;
        m_iSize = 0;
      }
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
      long lvalue = m_lPointer + iSourceOffset;
      IntPtr sourcePointer = ( IntPtr )lvalue;

#if DEBUG
      if( destination is ByteArrayDataProvider )
      {
        byte[] destArr = new byte[ iLength ];

        Marshal.Copy( sourcePointer, destArr, 0, iLength );
        destination.WriteBytes( iDestOffset, destArr, 0, iLength );
      }
      else
#endif
      {
        IntPtrDataProvider provider = ( IntPtrDataProvider )destination;
        //provider.EnsureCapacity( iLength );
        IntPtr destPointer;// = provider.m_ptrData;

        //if( iDestOffset > 0 )
      {
        long lValue = provider.m_lPointer + iDestOffset;
        destPointer = ( IntPtr )lValue;
      }

        Memory.CopyMemory( destPointer, sourcePointer, iLength );
      }
    }
    /// <summary>
    /// Writes zeros inside memory block.
    /// </summary>
    public override void ZeroMemory()
    {
      if( m_iSize > 0 )
      {
        Memory.RtlZeroMemory( m_ptrData, m_iSize );
      }
    }
    /// <summary>
    /// Moves memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public override void MoveMemory(int iDestOffset, int iSourceOffset, int iMemorySize )
    {
      if( iMemorySize < 0 )
        Debugger.Break();

      IntPtr ptrDest = ( IntPtr )( m_lPointer + iDestOffset );
      IntPtr ptrSource = ( IntPtr )( m_lPointer + iSourceOffset );
      Memory.RtlMoveMemory( ptrDest, ptrSource, iMemorySize );
    }
    /// <summary>
    /// Copies memory inside internal buffer.
    /// </summary>
    /// <param name="iDestOffset">Destination offset.</param>
    /// <param name="iSourceOffset">Source offset.</param>
    /// <param name="iMemorySize">Memory size.</param>
    public override void CopyMemory( int iDestOffset, int iSourceOffset, int iMemorySize )
    {
      IntPtr ptrDest = ( IntPtr )( m_lPointer + iDestOffset );
      IntPtr ptrSource = ( IntPtr )( m_lPointer + iSourceOffset );
      Memory.CopyMemory( ptrDest, ptrSource, iMemorySize );
    }
    /// <summary>
    /// Creates provider of the same type.
    /// </summary>
    /// <returns>Created provider object.</returns>
    public override DataProvider CreateProvider()
    {
      return new IntPtrDataProvider( HeapHandle );
    }
    #endregion
  }
}
#endif