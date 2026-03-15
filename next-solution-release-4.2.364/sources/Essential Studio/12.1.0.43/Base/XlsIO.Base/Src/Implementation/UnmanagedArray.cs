#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.InteropServices;
//using Syncfusion.XlsIO.IO.Stream.Win32;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for UnmanagedArray.
	/// </summary>
	public class UnmanagedArray : IDisposable
	{
    #region Class members
    /// <summary>
    /// Memory block that contains array data.
    /// </summary>
    private IntPtr m_ptrMemory = IntPtr.Zero;
    /// <summary>
    /// Size of the memory block.
    /// </summary>
    private int m_iMemorySize = 0;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public UnmanagedArray()
    {
    }
    /// <summary>
    /// Initializes new instance of the array.
    /// </summary>
    /// <param name="memorySize">Memory size in bytes.</param>
    /// <param name="bZeroMemory">Indicates whether to zero memory.</param>
    public UnmanagedArray( int memorySize, bool bZeroMemory )
    {
      Resize( memorySize, bZeroMemory );
    }
    /// <summary>
    /// Frees all allocated resources.
    /// </summary>
    ~UnmanagedArray()
    {
      Dispose();
    }
    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
      if( m_ptrMemory != IntPtr.Zero )
      {
        Marshal.FreeHGlobal( m_ptrMemory );
        m_ptrMemory = IntPtr.Zero;
        m_iMemorySize = 0;
        GC.SuppressFinalize( this );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns Int32 value from the array.
    /// </summary>
    /// <param name="index">Index of the Int32 value in the array (offset will be 4 * index).</param>
    /// <returns>Int32 value from the array.</returns>
    public int GetInt32( int index )
    {
      int iOffset = ExcelConstants.IntSize * index;

      if( iOffset > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      return Marshal.ReadInt32( m_ptrMemory, iOffset );
    }
    /// <summary>
    /// Returns Int16 value from the array.
    /// </summary>
    /// <param name="index">Index of the Int16 value in the array (offset will be 2 * index).</param>
    /// <returns>Int16 value from the array.</returns>
    public int GetInt16( int index )
    {
      int iOffset = ExcelConstants.ShortSize * index;

      if( iOffset > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      return Marshal.ReadInt16( m_ptrMemory, iOffset );
    }
    /// <summary>
    /// Returns Byte value from the array.
    /// </summary>
    /// <param name="index">Index of the Byte value in the array.</param>
    /// <returns>Int32 value from the array.</returns>
    public byte GetByte( int index )
    {
      if( index > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      return Marshal.ReadByte( m_ptrMemory, index );
    }
    /// <summary>
    /// Sets Int32 value into the array.
    /// </summary>
    /// <param name="index">Index of the Int32 value in the array (offset will be 4 * index).</param>
    /// <param name="value">Value to set.</param>
    public void SetInt32( int index, int value )
    {
      int iOffset = ExcelConstants.IntSize * index;

      if( iOffset + ExcelConstants.IntSize > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      Marshal.WriteInt32( m_ptrMemory, iOffset, value );
    }
    /// <summary>
    /// Sets Int16 value into the array.
    /// </summary>
    /// <param name="index">Index of the Int16 value in the array (offset will be 2 * index).</param>
    /// <param name="value">Value to set.</param>
    public void SetInt16( int index, short value )
    {
      int iOffset = ExcelConstants.ShortSize * index;

      if( iOffset + ExcelConstants.ShortSize > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      Marshal.WriteInt16( m_ptrMemory, iOffset, value );
    }
    /// <summary>
    /// Sets Byte value into the array.
    /// </summary>
    /// <param name="index">Index of the Byte value in the array.</param>
    /// <param name="value">Value to set.</param>
    public void SetByte( int index, byte value )
    {
      if( index > m_iMemorySize )
        throw new ArgumentOutOfRangeException( "index" );

      Marshal.WriteByte( m_ptrMemory, index, value );
    }
    /// <summary>
    /// Resizes current array.
    /// </summary>
    /// <param name="iDesiredSize">Desired size in bytes.</param>
    /// <param name="bZeroMemory">Indicates whether to zero memory.</param>
    public void Resize( int iDesiredSize, bool bZeroMemory )
    {
      if( iDesiredSize <= 0 )
        throw new ArgumentOutOfRangeException( "iDesiredSize" );

      m_ptrMemory = ( m_ptrMemory == IntPtr.Zero )
        ? m_ptrMemory = Marshal.AllocHGlobal( iDesiredSize )
        : Marshal.ReAllocHGlobal( m_ptrMemory, ( IntPtr )iDesiredSize );

      if( bZeroMemory )
      {
        IntPtr ptrDest = ( IntPtr )( m_ptrMemory.ToInt64() + m_iMemorySize );
        Memory.RtlZeroMemory( ptrDest, iDesiredSize - m_iMemorySize );
      }

      m_iMemorySize = iDesiredSize;
    }
    /// <summary>
    /// Copies memory from source unmanaged array to current unmanaged array.
    /// </summary>
    /// <param name="source">Represents source array.</param>
    /// <returns>Returns updated current array.</returns>
    public void CopyFrom( UnmanagedArray source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      Memory.CopyMemory( m_ptrMemory, source.m_ptrMemory, m_iMemorySize );
    }
    /// <summary>
    /// Clears current array.
    /// </summary>
    public void Clear()
    {
      Memory.RtlZeroMemory( m_ptrMemory, m_iMemorySize );
    }
    #endregion
  }
}
