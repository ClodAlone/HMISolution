#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for BytesList.
  /// </summary>
  public class BytesList
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_CAPACITY_STEP = 512;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_DEFAULT_CAPACITY = 512;
    /// <summary>
    /// Default size of the internal array.
    /// </summary>
    private const int DEF_RECORD_SIZE = 20;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_arrBuffer;
    /// <summary>
    /// First free position.
    /// </summary>
    private int m_iCurPos = 0;
    /// <summary>
    /// Indicates whether list can reserve some space.
    /// </summary>
    private bool m_bExactSize = true;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public BytesList()
      : this( DEF_DEFAULT_CAPACITY )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bExactSize"></param>
    public BytesList( bool bExactSize )
    {
      m_bExactSize = bExactSize;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCapacity"></param>
    public BytesList( int iCapacity )
    {
      EnsureFreeSpace( iCapacity );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrData"></param>
    public BytesList( byte[] arrData )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      EnsureFreeSpace( arrData.Length );
      AddRange( arrData );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bToAdd"></param>
    public void Add( byte bToAdd )
    {
      EnsureFreeSpace( 1 );
      m_arrBuffer[ m_iCurPos ] = bToAdd;
      m_iCurPos++;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrToAdd"></param>
    public void AddRange( byte[] arrToAdd )
    {
      if( arrToAdd == null )
        throw new ArgumentNullException( "arrToAdd" );

      int iLength = arrToAdd.Length;

      if( iLength > 0 )
      {
        EnsureFreeSpace( iLength );
        Buffer.BlockCopy( arrToAdd, 0, m_arrBuffer, m_iCurPos, iLength );
        m_iCurPos += iLength;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    public void AddRange( BytesList list )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      int iLength = list.Count;

      if( iLength > 0 )
      {
        EnsureFreeSpace( iLength );
        Buffer.BlockCopy( list.m_arrBuffer, 0, m_arrBuffer, m_iCurPos, iLength );
        m_iCurPos += iLength;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iStartIndex"></param>
    /// <param name="arrDest"></param>
    /// <param name="iDestIndex"></param>
    /// <param name="iCount"></param>
    public void CopyTo( int iStartIndex, byte[] arrDest, int iDestIndex, int iCount )
    {
      if( arrDest == null )
        throw new ArgumentNullException( "arrDest" );

      int iDestLength = arrDest.Length;

      if( iStartIndex < 0 || iStartIndex + iCount > m_iCurPos )
        throw new ArgumentOutOfRangeException( "iStartIndex" );

      if( iDestIndex < 0 || iDestIndex + iCount > iDestLength )
        throw new ArgumentOutOfRangeException( "iDestIndex" );

      Buffer.BlockCopy( m_arrBuffer, iStartIndex, arrDest, iDestIndex, iCount );
    }
    /// <summary>
    /// 
    /// </summary>
    public void EnsureFreeSpace( int iSize )
    {
      int iLength = ( m_arrBuffer == null ) ? 0 : m_arrBuffer.Length;
      int iNeededSize = m_iCurPos + iSize;

      if( iLength >= iNeededSize ) return;
      int iNewSize = iNeededSize;

      if( !m_bExactSize )
      {
        iNewSize = ( iLength == 0 ) ? DEF_RECORD_SIZE : iLength * 2;

        if ( iNewSize < iNeededSize )
        {
          iNewSize = iNeededSize;
        }
      }

      byte[] arrNewBuffer = new byte[ iNewSize ];

      if( iLength > 0 )
      {
        Buffer.BlockCopy( m_arrBuffer, 0, arrNewBuffer, 0, iLength );
      }

      m_arrBuffer = arrNewBuffer;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    internal byte[] InnerBuffer
    {
      get
      {
        return m_arrBuffer;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Count
    {
      get
      {
        return m_iCurPos;
      }
    }
    #endregion
  }
}
