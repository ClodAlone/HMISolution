#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT && !WINRT && !WP
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
#if AllowUnsafeCode
  /// <summary>
  /// Summary description for UnsafeDataProvider.
  /// </summary>
  public class UnsafeDataProvider : IntPtrDataProvider
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor to prevent creation of items without arguments.
    /// </summary>
    public UnsafeDataProvider( IntPtr heapHandle )
      : base( heapHandle )
    {
    }
    /// <summary>
    /// Creates provider of the same type.
    /// </summary>
    /// <returns>Created provider object.</returns>
    public override DataProvider CreateProvider()
    {
      return new UnsafeDataProvider( HeapHandle );
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
      byte result;

      unsafe
      {
        byte* ptr = ( byte* )( m_lPointer + iOffset );
        result = *ptr;
      }

      return result;
    }
    /// <summary>
    /// Returns Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int16 value at the specified position.</returns>
    public override short ReadInt16( int iOffset )
    {
      short result;

      unsafe
      {
        short* ptr = ( short* )( m_lPointer + iOffset );
        result = *ptr;
      }

      return result;
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override int ReadInt32( int iOffset )
    {
      int result;

      unsafe
      {
        int* ptr = ( int* )( m_lPointer + iOffset );
        result = *ptr;
      }

      return result;
    }
    /// <summary>
    /// Returns Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Int32 value at the specified position.</returns>
    public override long ReadInt64( int iOffset )
    {
      long result;

      unsafe
      {
        long* ptr = ( long* )( m_lPointer + iOffset );
        result = *ptr;
      }

      return result;
    }
    /// <summary>
    /// Returns Double value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <returns>Double value at the specified position.</returns>
    public override double ReadDouble( int iOffset )
    {
      double result;

      unsafe
      {
        double* ptr = ( double* )( m_lPointer + iOffset );
        result = *ptr;
      }

      return result;
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
      unsafe
      {
        byte* pointer = ( byte* )( m_lPointer + iOffset );
        *pointer = value;
      }
    }
    /// <summary>
    /// Writes Int16 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int16 value to write at the specified position.</param>
    public override void WriteInt16( int iOffset, short value )
    {
      unsafe
      { 
        short* pointer = ( short* )( m_lPointer + iOffset );
        *pointer = value;
      }
    }
    /// <summary>
    /// Writes Int32 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int32 value to write at the specified position.</param>
    public override void WriteInt32( int iOffset, int value )
    {
      unsafe
      {
        int* pointer = ( int* )( m_lPointer + iOffset );
        *pointer = value;
      }
    }
    /// <summary>
    /// Writes Int64 value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Int64 value to write at the specified position.</param>
    public override void WriteInt64( int iOffset, long value )
    {
      unsafe
      {
        long* ptr = ( long* )( m_lPointer + iOffset );
        *ptr = value;
      }
    }
    /// <summary>
    /// Writes Double value at the specified position.
    /// </summary>
    /// <param name="iOffset">Offset to the required value.</param>
    /// <param name="value">Double value to write at the specified position.</param>
    public override void WriteDouble( int iOffset, double value )
    {
      unsafe
      {
        double* pointer = ( double* )( m_lPointer + iOffset );
        *pointer = value;
      }
    }
    #endregion
  }
#endif
}
#endif