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

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
#endregion

namespace Syncfusion.XlsIO.Parser
{
  //using BufferedStg = Syncfusion.CompoundFile.XlsIO.Native.BufferedStreamEx;
  
  /// <summary>
  /// Class for extracting Biff records from the stream.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BiffReader : IDisposable
  {
    #region Class constants
    /// <summary>
    /// Default size of the buffer
    /// </summary>
    private const int DEF_BUFFER_SIZE = 256*1024;
    /// <summary>
    /// Version number that stands for BIFF8
    /// </summary>
    private const int BIFF8_VERSION = 0x600;
    #endregion

    #region Class members
    /// <summary>
    /// Stream of data which will be used by reader
    /// </summary>
    private Stream  m_stream;
    /// <summary>
    /// Internal binary reader.
    /// </summary>
    private BinaryReader  m_reader;
    /// <summary>
    /// TRUE - indicate that object was disposed and cannot be used,
    /// otherwise FALSE
    /// </summary>
    private bool    m_bDisposed;
    /// <summary>
    /// TRUE - destroy stream on own dispose, otherwise false.
    /// </summary>
    private bool    m_bDestroyStream;
    /// <summary>
    /// Minimal version that is accepted by the reader
    /// </summary>
    private int     m_iMinimalVersion = BIFF8_VERSION;
    /// <summary>
    /// Buffer for records data.
    /// </summary>
    private byte[]  m_arrBuffer = new byte[ BiffRecordRaw.DEF_RECORD_MAX_SIZE + 4 ];
    /// <summary>
    /// Object that provides access to the data.
    /// </summary>
    private DataProvider m_dataProvider;
    #endregion

    #region Class Properties
    /// <summary>
    /// Base stream.
    /// </summary>
    public Stream BaseStream
    {
      get
      {
        return m_stream;
      }
    }
    /// <summary>
    /// Returns base BinaryReader. Read-only.
    /// </summary>
    public BinaryReader BaseReader
    {
      get
      {
        return m_reader;
      }
    }
    /// <summary>
    /// Get / Set minimal version accepted by the reader
    /// </summary>
    public int    MinimalVersion
    {
      get
      {
        return m_iMinimalVersion;
      }
      set
      {
        m_iMinimalVersion = value;
      }
    }
    /// <summary>
    /// Returns reference to the internal buffer. Read-only.
    /// </summary>
    public byte[] Buffer
    {
      get
      {
        return m_arrBuffer;
      }
    }
    /// <summary>
    /// Returns object that provides access to the data. Read-only.
    /// </summary>
    public DataProvider DataProvider
    {
      get
      {
        return m_dataProvider;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent construct of object by default constructor.
    /// </summary>
    private BiffReader()
    {
      m_dataProvider = new ByteArrayDataProvider( m_arrBuffer );
    }
    /// <summary>
    /// Open stream for reading.
    /// </summary>
    /// <param name="stream">Input stream which contains data.</param>
    public BiffReader( Stream stream )
      : this()
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      // Use Buffer Stg wrapper only when input stream has not own cache.
      //m_stream = ( stream is MemoryStream || stream is FileStream || stream is Syncfusion.CompoundFile.XlsIO.ICo )
      //  ? stream
      //  : new BufferedStg( stream, DEF_BUFFER_SIZE );
      // TODO: investigate bufferesation necessity
      m_stream = stream;

      m_reader = new BinaryReader( m_stream );
    }
    /// <summary>
    /// Open stream for reading and control stream live time.
    /// </summary>
    /// <param name="stream">Input stream which contains data</param>
    /// <param name="bControlStream">TRUE - reader will dispose stream on own
    /// destroy, otherwise FALSE</param>
    public BiffReader( Stream stream, bool bControlStream )
      : this( stream )
    {
      m_bDestroyStream = bControlStream;
    }
    /// <summary>
    /// Free all resources used by this class
    /// </summary>
    public void Dispose()
    {
      if( m_bDisposed ) return;

      m_bDisposed = true;

      if( m_bDestroyStream )
      {
        ((IDisposable)m_reader).Dispose();
        ((IDisposable)m_stream).Dispose();
      }

      m_stream = null;
      m_reader = null;
      m_arrBuffer = null;

      if( m_dataProvider != null )
      {
        m_dataProvider.Dispose();
        m_dataProvider = null;
      }
    }
    #endregion

    #region Class Parse
    /// <summary>
    /// Read-only. Property return TRUE when reader cannot 
    /// extract no more records from stream.
    /// </summary>
    public bool IsEOF
    {
      get
      {
        if( m_stream == null )
          throw new ArgumentNullException( "internal stream" );

        try
        {
          // If we reach end of stream then return TRUE, otherwise
          // try to extract record from stream. If record extracted successfully
          // then return TRUE, otherwise FALSE.
          if( m_stream.Position == m_stream.Length )
          {
            return true;
          }
          else
          {
            int iCode = m_reader.ReadInt16();
            m_reader.BaseStream.Position -= 2;
            return iCode == 0;
          }
        }
        catch( Exception ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_stream.Position, "Current Position" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_stream.Length, "Stream Size" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );
          return true;
        }
      }
    }
    /// <summary>
    /// Gets next record from the stream.
    /// </summary>
    /// <returns>Extracted biff record.</returns>
    public BiffRecordRaw GetRecord()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      return BiffRecordFactory.GetRecord( m_reader, m_dataProvider, m_arrBuffer );
    }
    /// <summary>
    /// Gets next record from the stream.
    /// </summary>
    /// <param name="decryptor">Decryptor used to decrypt encrypted records.</param>
    /// <returns>Extracted biff record.</returns>
    public BiffRecordRaw GetRecord( IDecryptor decryptor )
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      //return GetRecord();
      // TODO: this functionality hasn't been implemented yet because we have no correct decryptor.
      return BiffRecordFactory.GetRecord( m_reader, m_dataProvider, decryptor, m_arrBuffer );
    }
    /// <summary>
    /// Gets record from the stream without changing stream position.
    /// </summary>
    /// <returns>Extracted biff record.</returns>
    public BiffRecordRaw PeekRecord()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      long lPos = m_stream.Position;
      BiffRecordRaw record = GetRecord();
      m_stream.Position = lPos;

      return record;
    }
    /// <summary>
    /// Gets record type from the stream without changing stream position.
    /// </summary>
    /// <returns>Extracted record type.</returns>
    public TBIFFRecord PeekRecordType()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      long lPos = m_stream.Position;
      TBIFFRecord result = ( TBIFFRecord )BiffRecordFactory.ExtractRecordType( m_reader );
      m_stream.Position = lPos;
      
      return result;
    }
    /// <summary>
    /// Searches for the next BOFRecord in the stream.
    /// </summary>
    /// <returns>Found BOFRecord or null if was not found.</returns>
    public BiffRecordRaw SeekOnBOFRecord()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      BiffRecordRaw record = null;
      long lPos = m_stream.Position;
      byte[] arrBuffer = new byte[ 2 ];

      while( record == null )
      {
        m_stream.Read( arrBuffer, 0, 2 );
        int iCode = arrBuffer[ 0 ] + ( arrBuffer[ 1 ] << 8 );
        TBIFFRecord biffCode = ( TBIFFRecord )iCode;

        if( biffCode == TBIFFRecord.BOF || biffCode == TBIFFRecord.BOF2 )
        {
          m_stream.Position += 2;
          m_stream.Read( arrBuffer, 0, 2 );
          int iVersion = arrBuffer[ 0 ] + ( arrBuffer[ 1 ] << 8 );
          
          if( iVersion < MinimalVersion )
            throw new FormatException( "Bad file version. Expected version is" 
              + (MinimalVersion) + " version found " + (iVersion) );

          m_stream.Position -= 6;
          record = BiffRecordFactory.GetRecord( m_reader, m_dataProvider, m_arrBuffer );
        }

        if( m_stream.Position >= m_stream.Length ) return null;
      }

      return record;
    }

    /// <summary>
    /// Seeks for the specified record.
    /// </summary>
    /// <param name="recordCode">Record to search.</param>
    /// <returns>Found record, or NULL if record was not found.</returns>
    public BiffRecordRaw SeekOnRecord( TBIFFRecord recordCode )
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      BiffRecordRaw record = null;
      long lPos = m_stream.Position;

      while( record == null )
      {
        int iCode = (m_stream.ReadByte() & 0xff) + ((m_stream.ReadByte() & 0xff) << 8);

        if( ( TBIFFRecord )iCode == recordCode )
          record = BiffRecordFactory.GetRecord( m_reader, m_dataProvider, m_arrBuffer );

        if( m_stream.Position >= m_stream.Length ) return null;
      }

      return record;
    }
    /// <summary>
    /// Extract Record from stream. This method used by IsEOF property
    /// to optimize its performance.
    /// </summary>
    /// <returns>Extracted record.</returns>
    protected BiffRecordRaw TestPeekRecord()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "internal stream" );

      Stream stream = m_reader.BaseStream;
      long lPos = stream.Position;
      
      BiffRecordRaw record = null;

#if STANDARD_PEEK
      record = BiffRecordFactory.GetUntypedRecord( m_reader );
#else
      int iCode = m_reader.ReadInt16();
      
      if( iCode > 0 ) record = UnknownRecord.Empty;
#endif
      
      stream.Position = lPos;

      return record;
    }
    #endregion
  }
}