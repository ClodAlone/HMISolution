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
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Parser
{
  //using BufferedStg = Syncfusion.CompoundFile.XlsIO.Native.BufferedStreamEx;
  //using BufferedStg = System.IO.BufferedStream;

  /// <summary>
  ///
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BiffWriter : IDisposable
  {
    #region Class Constants
    /// <summary>
    /// Size of the buffer.
    /// </summary>
    private const int DEF_BUFFER_SIZE = 1024*1024;
    #endregion

    #region Class members
    /// <summary>
    /// Stream of data which will be used by reader.
    /// </summary>
    private Stream  m_stream;
    /// <summary>
    /// True indicates that the object was disposed and cannot be used;
    /// otherwise False.
    /// </summary>
    private bool    m_bDisposed;
    /// <summary>
    /// TRUE to destroy stream on own dispose; otherwise False.
    /// </summary>
    private bool    m_bDestroyStream;
    /// <summary>
    ///
    /// </summary>
    private BinaryWriter m_writer;
    /// <summary>
    /// Buffer for record data.
    /// </summary>
    private byte[] m_arrBuffer = new byte[ BiffRecordRaw.DEF_RECORD_MAX_SIZE_WITH_HADER ];
    /// <summary>
    /// Object that provides access to internal data.
    /// </summary>
    private ByteArrayDataProvider m_provider;
    #endregion

    #region Class properties
    /// <summary>
    ///
    /// </summary>
    public Stream BaseStream
    {
      get
      {
        return m_stream;
      }
    }
    /// <summary>
    /// Returns internal buffer. Read-only.
    /// </summary>
    public byte[] Buffer
    {
      get
      {
        return m_arrBuffer;
      }
    }
    #endregion

    #region Class Initialize/finilize methods
    /// <summary>
    ///
    /// </summary>
    private BiffWriter()
    {
      m_provider = new ByteArrayDataProvider( m_arrBuffer );
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="stream"></param>
    public BiffWriter( Stream stream )
      : this( stream, false )
    {
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bControlsStream"></param>
    public BiffWriter( Stream stream, bool bControlsStream )
    {
      m_provider = new ByteArrayDataProvider( m_arrBuffer );
      //m_stream = stream;
      m_bDestroyStream = bControlsStream;

      // Cache only stream which has no buffers.
      m_stream = stream;
      //m_stream = ( stream is FileStream || stream is MemoryStream )
      //  ? stream
      //  : new BufferedStg( stream, DEF_BUFFER_SIZE );

      m_writer = new BinaryWriter( m_stream );
    }
    /// <summary>
    ///
    /// </summary>
    public void Dispose()
    {
      if( m_bDisposed ) return;
      m_bDisposed = true;

      m_writer.Flush();

      if( m_bDestroyStream )
      {
        m_stream.SetLength( m_stream.Position );
        ((IDisposable)m_stream).Dispose();
      }

      m_stream = null;
      m_provider = null;
    }
    #endregion

    #region Class write methods
    /// <summary>
    ///
    /// </summary>
    /// <param name="raw"></param>
    /// <param name="encryptor">Object to encrypt data.</param>
    public void WriteRecord( BiffRecordRaw raw, IEncryptor encryptor )
    {
      if( raw == null )
        throw new ArgumentNullException( "raw" );

#if DEBUG
      //long lPosition = m_writer.BaseStream.Position;

      //if( raw.StreamPos != lPosition )
      //  Debugger.Break();
#endif

      raw.FillStream( m_writer, m_provider, encryptor, ( int )m_writer.BaseStream.Position );

      if( !raw.NeedDataArray )
      {
        raw.ClearData();
        raw.NeedInfill = true;
      }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="records"></param>
    /// <param name="encryptor">Object to encrypt data.</param>
    [ CLSCompliant( false ) ]
    public void WriteRecord( OffsetArrayList records, IEncryptor encryptor )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

#if DEBUG_PERFORMANCE
      Dictionary<int, TimeSpan> maxCount = new Dictionary<int, TimeSpan>();
      Dictionary<int, TimeSpan> hashTotalTime = new Dictionary<int, TimeSpan>();
#endif
      int iStreamPosition = ( int )m_writer.BaseStream.Position;

      for( int i = 0, len = records.Count; i < len; i++ )
      {
        IBiffStorage raw = records[ i ];// as BiffRecordRaw;

#if DEBUG_POSITION
        //long lPosition = m_writer.BaseStream.Position;

        //if( raw.StreamPos != lPosition && raw.StreamPos > 0 )
        //  Debugger.Break();
#endif

#if DEBUG_PERFORMANCE
        DateTime now  = DateTime.Now;
#endif
        iStreamPosition += raw.FillStream( m_writer, m_provider, encryptor, iStreamPosition );

        if( !raw.NeedDataArray && raw is BiffRecordRawWithArray )
        {
          BiffRecordRawWithArray rawWithArray = raw as BiffRecordRawWithArray;
          rawWithArray.Data = new byte[ 0 ];
          rawWithArray.NeedInfill = true;
        }

#if DEBUG
//        RowStorage rowStorage = raw as RowStorage;
//
//        if( rowStorage != null )
//        {
//          string strMessage = string.Format( "Record index: {0}, Storage size: {1}, Allocated size: {2}", i, rowStorage.UsedSize, rowStorage.DataSize );
//          Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, strMessage );
//        }
#endif

#if DEBUG_PERFORMANCE
        TimeSpan diff = DateTime.Now.Subtract( now );

        int iCode = raw.RecordCode;

        if( maxCount.ContainsKey( iCode ) )
        {
          TimeSpan maxTime = maxCount[ iCode ];

          if( maxTime < diff )
            maxCount[ iCode ] = diff;
        }
        else
        {
          maxCount[ iCode ] = diff;
        }

        if( hashTotalTime.ContainsKey( iCode ) )
        {
          TimeSpan curTime = hashTotalTime[ iCode ];
          diff +=curTime;
        }

        hashTotalTime[ iCode ] = diff;
#endif
      }

#if DEBUG_PERFORMANCE
      Debug.WriteLine( "Single call time:" );
      WriteTopTen( maxCount, 10 );

      Debug.WriteLine( "Total time:" );
      WriteTopTen( hashTotalTime, 10 );
#endif
    }
#if DEBUG_PERFORMANCE
    private void WriteTopTen( Dictionary<int, TimeSpan> hashData, int iTop )
    {
      TimeSpan[] times = new List<TimeSpan>( hashData.Values ).ToArray();
      int[] names = new List<int>( hashData.Keys ).ToArray();

      Array.Sort( times, names );

      Debug.WriteLine( "Top save time takers" );

      for( int j = 0, i = times.Length - 1; j < iTop ; i--, j++ )
      {
        Debug.WriteLine( 
          string.Format( "{0,2}. datatype: {1,40} save time: {2}", j, ( TBIFFRecord )names[i], times[i] ),
          "Performance" );
      }
    }
#endif
    /// <summary>
    ///
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="encryptor">Object to encrypt data.</param>
    public void WriteRecord( ICollection collection, IEncryptor encryptor )
    {
      if( collection == null )
        throw new ArgumentNullException( "collection" );

#if DEBUG_PERFORMANCE
      Dictionary<string, TimeSpan> maxCount = new Dictionary<string, TimeSpan>();
#endif

      int iOffset = ( int )m_writer.BaseStream.Position;

      foreach( BiffRecordRaw raw in collection )
      {
#if DEBUG_PERFORMANCE
        DateTime now  = DateTime.Now;
#endif
        iOffset += raw.FillStream( m_writer, m_provider, encryptor, iOffset );

        if( !raw.NeedDataArray )
        {
          raw.Data = new byte[ 0 ];
          raw.NeedInfill = true;
        }
#if DEBUG_PERFORMANCE
        TimeSpan diff = DateTime.Now.Subtract( now );

        string type = raw.GetType().Name;
        if( maxCount.ContainsKey( type ) )
        {
          TimeSpan maxTime = (TimeSpan)maxCount[ type ];
          if( maxTime < diff ) maxCount[ type ] = diff;
        }
        else
          maxCount[ type ] = diff;
#endif
      }

#if DEBUG_PERFORMANCE
      TimeSpan[] times = new List<string>( maxCount.Values ).ToArray( typeof( TimeSpan ) );
      string[] names = new List<string>( maxCount.Keys ).ToArray( typeof( string ) );

      Array.Sort( times, names );

      Debug.WriteLine( "Top Ten save time takers" );
      for( int j = 0, i = times.Length-1; j<10 ; i--, j++ )
      {
        Debug.WriteLine( 
          string.Format( "{0,2}. datatype: {1,40} save time: {2}", j, names[i], times[i] ),
          "Performance" );
      }
#endif
    }
    #endregion
  }
}
